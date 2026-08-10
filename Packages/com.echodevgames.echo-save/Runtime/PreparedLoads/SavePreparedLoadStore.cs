
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Runtime-memory-only lifetime authority for prepared Chronicle loads.
    /// It owns no storage backend, scene, serializer, migration registry, or
    /// participant callback authority.
    /// </summary>
    internal sealed class SavePreparedLoadStore : IDisposable
    {
        internal const int DefaultMaxLiveHandles = 8;

        internal const long DefaultMaxSourceTransportBytes =
            32L * 1024L * 1024L;

        internal static readonly TimeSpan DefaultLifetime =
            TimeSpan.FromMinutes(5);

        private sealed class Entry
        {
            internal Entry(
                PreparedSaveLoad handle,
                SavePreparedParticipantBatch preparedParticipants,
                SaveUnknownPayloadSnapshot unknownPayloads,
                long sourceTransportBytes,
                long token,
                long epoch,
                DateTimeOffset expiresUtc)
            {
                Handle = handle;
                PreparedParticipants = preparedParticipants;
                UnknownPayloads = unknownPayloads;
                SourceTransportBytes = sourceTransportBytes;
                Token = token;
                Epoch = epoch;
                ExpiresUtc = expiresUtc;
            }

            internal PreparedSaveLoad Handle { get; }

            internal SavePreparedParticipantBatch PreparedParticipants { get; }

            internal SaveUnknownPayloadSnapshot UnknownPayloads { get; }

            internal long SourceTransportBytes { get; }

            internal long Token { get; }

            internal long Epoch { get; }

            internal DateTimeOffset ExpiresUtc { get; }
        }

        private readonly IPreparedLoadClock clock;
        private readonly TimeSpan lifetime;
        private readonly int maxLiveHandles;
        private readonly long maxSourceTransportBytes;

        private readonly Dictionary<long, Entry> entries =
            new Dictionary<long, Entry>();

        private long nextToken;
        private long epoch = 1L;
        private long liveSourceTransportBytes;
        private bool available = true;

        internal SavePreparedLoadStore()
            : this(
                SystemPreparedLoadClock.Instance,
                DefaultLifetime,
                DefaultMaxLiveHandles,
                DefaultMaxSourceTransportBytes)
        {
        }

        internal SavePreparedLoadStore(
            IPreparedLoadClock clock,
            TimeSpan lifetime,
            int maxLiveHandles,
            long maxSourceTransportBytes)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(
                    nameof(clock));
            }

            if (lifetime <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lifetime));
            }

            if (maxLiveHandles <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxLiveHandles));
            }

            if (maxSourceTransportBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxSourceTransportBytes));
            }

            this.clock = clock;
            this.lifetime = lifetime;
            this.maxLiveHandles = maxLiveHandles;
            this.maxSourceTransportBytes = maxSourceTransportBytes;
        }

        internal int LiveCount
        {
            get
            {
                SweepExpired();
                return entries.Count;
            }
        }

        internal long LiveSourceTransportBytes
        {
            get
            {
                SweepExpired();
                return liveSourceTransportBytes;
            }
        }

        internal long Epoch =>
            epoch;

        internal bool IsAvailable =>
            available;

        internal PreparedLoadCreationResult TryCreate(
            SaveCurrentGenerationReadResult readResult,
            SaveParticipantPreparationResult preparationResult,
            SaveUnknownPayloadSnapshot unknownPayloadSnapshot)
        {
            if (!available)
            {
                return Failure(
                    PreparedLoadCreationStatus.OwnerUnavailable,
                    EchoSaveDiagnosticCodes.PreparedLoadOwnerUnavailable,
                    "The Chronicle prepared-load owner is unavailable.");
            }

            SweepExpired();

            if (!TryValidateArtifacts(
                    readResult,
                    preparationResult,
                    unknownPayloadSnapshot,
                    out SaveSlotId sourceSlotId,
                    out SaveGenerationId sourceGenerationId,
                    out SavePreparedParticipantBatch preparedBatch,
                    out SaveUnknownPayloadSnapshot defensiveUnknownSnapshot,
                    out long sourceTransportBytes,
                    out PreparedLoadCreationResult validationFailure))
            {
                return validationFailure;
            }

            if (entries.Count >= maxLiveHandles)
            {
                return Failure(
                    PreparedLoadCreationStatus.CountLimitExceeded,
                    EchoSaveDiagnosticCodes.PreparedLoadCountLimitExceeded,
                    "The Chronicle prepared-load live-handle count limit is reached.");
            }

            long candidateAggregateBytes;

            try
            {
                checked
                {
                    candidateAggregateBytes =
                        liveSourceTransportBytes +
                        sourceTransportBytes;
                }
            }
            catch (OverflowException)
            {
                return Failure(
                    PreparedLoadCreationStatus.ByteLimitExceeded,
                    EchoSaveDiagnosticCodes.PreparedLoadByteLimitExceeded,
                    "The Chronicle prepared-load aggregate source-byte estimate exceeded the supported range.");
            }

            if (candidateAggregateBytes >
                maxSourceTransportBytes)
            {
                return Failure(
                    PreparedLoadCreationStatus.ByteLimitExceeded,
                    EchoSaveDiagnosticCodes.PreparedLoadByteLimitExceeded,
                    "The Chronicle prepared-load aggregate source-byte estimate limit would be exceeded.");
            }

            if (!TryAllocateToken(
                    out long token))
            {
                available = false;

                InvalidateAllCore(
                    PreparedLoadState.OwnerInvalidated,
                    advanceEpoch: false);

                return Failure(
                    PreparedLoadCreationStatus.OwnerUnavailable,
                    EchoSaveDiagnosticCodes.PreparedLoadOwnerUnavailable,
                    "The Chronicle prepared-load owner exhausted its runtime ownership-token space.");
            }

            DateTimeOffset createdUtc =
                clock.UtcNow;

            DateTimeOffset expiresUtc;

            try
            {
                expiresUtc =
                    createdUtc.Add(
                        lifetime);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Failure(
                    PreparedLoadCreationStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes.PreparedLoadInvalidRequest,
                    "The Chronicle prepared-load expiry time is outside the supported UTC range.");
            }

            PreparedSaveLoad handle =
                new PreparedSaveLoad(
                    this,
                    token,
                    epoch,
                    sourceSlotId,
                    sourceGenerationId,
                    preparedBatch.Count,
                    defensiveUnknownSnapshot.Count,
                    sourceTransportBytes,
                    createdUtc,
                    expiresUtc);

            entries.Add(
                token,
                new Entry(
                    handle,
                    preparedBatch,
                    defensiveUnknownSnapshot,
                    sourceTransportBytes,
                    token,
                    epoch,
                    expiresUtc));

            liveSourceTransportBytes =
                candidateAggregateBytes;

            return new PreparedLoadCreationResult(
                PreparedLoadCreationStatus.Succeeded,
                handle,
                string.Empty,
                "The Chronicle prepared-load handle was admitted successfully.");
        }

        internal bool TryGetPreparedParticipantBatch(
            PreparedSaveLoad handle,
            out SavePreparedParticipantBatch preparedParticipants)
        {
            preparedParticipants = null;

            if (!TryGetOwnedLiveEntry(
                    handle,
                    out Entry entry))
            {
                return false;
            }

            preparedParticipants =
                entry.PreparedParticipants;

            return true;
        }

        internal bool TryGetUnknownPayloadSnapshot(
            PreparedSaveLoad handle,
            out SaveUnknownPayloadSnapshot unknownPayloads)
        {
            unknownPayloads = null;

            if (!TryGetOwnedLiveEntry(
                    handle,
                    out Entry entry))
            {
                return false;
            }

            unknownPayloads =
                CloneUnknownSnapshot(
                    entry.UnknownPayloads);

            return true;
        }

        internal void RefreshState(
            PreparedSaveLoad handle,
            long token,
            long handleEpoch)
        {
            if (handle == null ||
                handle.UnsafeState !=
                    PreparedLoadState.Live)
            {
                return;
            }

            if (!entries.TryGetValue(
                    token,
                    out Entry entry) ||
                !ReferenceEquals(
                    entry.Handle,
                    handle) ||
                entry.Epoch !=
                    handleEpoch ||
                !handle.IsOwnedBy(
                    this,
                    token,
                    handleEpoch))
            {
                return;
            }

            if (clock.UtcNow >=
                entry.ExpiresUtc)
            {
                ReleaseOwned(
                    handle,
                    token,
                    handleEpoch,
                    PreparedLoadState.Expired);
            }
        }

        internal bool ReleaseOwned(
            PreparedSaveLoad handle,
            long token,
            long handleEpoch,
            PreparedLoadState terminalState)
        {
            if (terminalState ==
                    PreparedLoadState.Live ||
                handle == null ||
                !entries.TryGetValue(
                    token,
                    out Entry entry) ||
                !ReferenceEquals(
                    entry.Handle,
                    handle) ||
                entry.Epoch !=
                    handleEpoch ||
                !handle.IsOwnedBy(
                    this,
                    token,
                    handleEpoch))
            {
                return false;
            }

            entries.Remove(
                token);

            ReleaseBytes(
                entry.SourceTransportBytes);

            handle.SetTerminalState(
                terminalState);

            return true;
        }

        internal int SweepExpired()
        {
            if (entries.Count == 0)
            {
                return 0;
            }

            DateTimeOffset now =
                clock.UtcNow;

            List<long> expiredTokens =
                null;

            foreach (KeyValuePair<long, Entry> pair
                in entries)
            {
                if (now <
                    pair.Value.ExpiresUtc)
                {
                    continue;
                }

                if (expiredTokens == null)
                {
                    expiredTokens =
                        new List<long>();
                }

                expiredTokens.Add(
                    pair.Key);
            }

            if (expiredTokens == null)
            {
                return 0;
            }

            int expiredCount =
                0;

            for (int i = 0;
                 i < expiredTokens.Count;
                 i++)
            {
                long token =
                    expiredTokens[i];

                if (!entries.TryGetValue(
                        token,
                        out Entry entry))
                {
                    continue;
                }

                if (ReleaseOwned(
                        entry.Handle,
                        entry.Token,
                        entry.Epoch,
                        PreparedLoadState.Expired))
                {
                    expiredCount++;
                }
            }

            return expiredCount;
        }

        internal void InvalidateSession()
        {
            if (!available)
            {
                return;
            }

            InvalidateAllCore(
                PreparedLoadState.OwnerInvalidated,
                advanceEpoch: true);
        }

        public void Dispose()
        {
            if (!available)
            {
                return;
            }

            available = false;

            InvalidateAllCore(
                PreparedLoadState.OwnerInvalidated,
                advanceEpoch: true);
        }

        private bool TryGetOwnedLiveEntry(
            PreparedSaveLoad handle,
            out Entry entry)
        {
            entry = null;

            if (!available ||
                handle == null)
            {
                return false;
            }

            RefreshState(
                handle,
                handle.OwnershipToken,
                handle.OwnerEpoch);

            if (handle.UnsafeState !=
                    PreparedLoadState.Live ||
                !entries.TryGetValue(
                    handle.OwnershipToken,
                    out Entry candidate) ||
                !ReferenceEquals(
                    candidate.Handle,
                    handle) ||
                candidate.Epoch !=
                    handle.OwnerEpoch ||
                !handle.IsOwnedBy(
                    this,
                    candidate.Token,
                    candidate.Epoch))
            {
                return false;
            }

            entry =
                candidate;

            return true;
        }

        private void InvalidateAllCore(
            PreparedLoadState terminalState,
            bool advanceEpoch)
        {
            if (entries.Count != 0)
            {
                Entry[] currentEntries =
                    new Entry[
                        entries.Count];

                entries.Values.CopyTo(
                    currentEntries,
                    0);

                entries.Clear();

                liveSourceTransportBytes =
                    0L;

                for (int i = 0;
                     i < currentEntries.Length;
                     i++)
                {
                    currentEntries[i]
                        .Handle
                        .SetTerminalState(
                            terminalState);
                }
            }

            if (!advanceEpoch)
            {
                return;
            }

            if (epoch ==
                long.MaxValue)
            {
                available = false;
                return;
            }

            epoch++;
        }

        private bool TryAllocateToken(
            out long token)
        {
            token =
                0L;

            if (nextToken ==
                long.MaxValue)
            {
                return false;
            }

            token =
                ++nextToken;

            return true;
        }

        private void ReleaseBytes(
            long bytes)
        {
            liveSourceTransportBytes -=
                bytes;

            if (liveSourceTransportBytes <
                0L)
            {
                liveSourceTransportBytes =
                    0L;
            }
        }

        private static bool TryValidateArtifacts(
            SaveCurrentGenerationReadResult readResult,
            SaveParticipantPreparationResult preparationResult,
            SaveUnknownPayloadSnapshot unknownPayloadSnapshot,
            out SaveSlotId sourceSlotId,
            out SaveGenerationId sourceGenerationId,
            out SavePreparedParticipantBatch preparedBatch,
            out SaveUnknownPayloadSnapshot defensiveUnknownSnapshot,
            out long sourceTransportBytes,
            out PreparedLoadCreationResult failure)
        {
            sourceSlotId =
                default;

            sourceGenerationId =
                default;

            preparedBatch =
                null;

            defensiveUnknownSnapshot =
                null;

            sourceTransportBytes =
                0L;

            failure =
                default;

            if (!readResult.Succeeded ||
                readResult.ValidatedParticipants ==
                    null ||
                preparationResult ==
                    null ||
                !preparationResult.Succeeded ||
                preparationResult.Batch ==
                    null ||
                readResult.KnownParticipantCount <
                    0 ||
                readResult.UnknownParticipantCount <
                    0 ||
                !SaveSlotId.TryParse(
                    readResult.SlotId.Value,
                    out sourceSlotId) ||
                !SaveGenerationId.TryParse(
                    readResult.GenerationId.Value,
                    out sourceGenerationId))
            {
                failure =
                    Failure(
                        PreparedLoadCreationStatus.InvalidRequest,
                        EchoSaveDiagnosticCodes.PreparedLoadInvalidRequest,
                        "Chronicle prepared-load creation requires one successful validated current-generation read and one successful participant preparation batch.");

                return false;
            }

            SaveValidatedParticipantSnapshot validated =
                readResult.ValidatedParticipants;

            preparedBatch =
                preparationResult.Batch;

            if (!SameSlot(
                    sourceSlotId,
                    validated.SourceSlotId) ||
                !SameGeneration(
                    sourceGenerationId,
                    validated.SourceGenerationId) ||
                !SameSlot(
                    sourceSlotId,
                    preparedBatch.SourceSlotId) ||
                !SameGeneration(
                    sourceGenerationId,
                    preparedBatch.SourceGenerationId) ||
                validated.Count !=
                    readResult.KnownParticipantCount +
                    readResult.UnknownParticipantCount ||
                preparedBatch.Count !=
                    readResult.KnownParticipantCount)
            {
                failure =
                    Failure(
                        PreparedLoadCreationStatus.SourceProvenanceMismatch,
                        EchoSaveDiagnosticCodes.PreparedLoadSourceMismatch,
                        "Chronicle prepared-load artifacts do not agree on one exact source slot/generation and participant classification.");

                return false;
            }

            if (!TryComputeSourceBytes(
                    validated,
                    out sourceTransportBytes))
            {
                failure =
                    Failure(
                        PreparedLoadCreationStatus.InvalidRequest,
                        EchoSaveDiagnosticCodes.PreparedLoadInvalidRequest,
                        "Chronicle prepared-load source transport byte lengths are invalid or exceed the supported range.");

                return false;
            }

            if (!TryValidateUnknownSnapshot(
                    readResult,
                    sourceSlotId,
                    sourceGenerationId,
                    unknownPayloadSnapshot,
                    out defensiveUnknownSnapshot))
            {
                failure =
                    Failure(
                        PreparedLoadCreationStatus.UnknownPayloadProvenanceMismatch,
                        EchoSaveDiagnosticCodes.PreparedLoadUnknownProvenanceMismatch,
                        "Chronicle prepared-load unknown payload state does not match the exact validated source slot/generation.");

                return false;
            }

            return true;
        }

        private static bool TryValidateUnknownSnapshot(
            SaveCurrentGenerationReadResult readResult,
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            SaveUnknownPayloadSnapshot unknownPayloadSnapshot,
            out SaveUnknownPayloadSnapshot defensiveCopy)
        {
            defensiveCopy =
                null;

            if (readResult.UnknownParticipantCount ==
                    0 &&
                unknownPayloadSnapshot ==
                    null)
            {
                defensiveCopy =
                    new SaveUnknownPayloadSnapshot(
                        Array.Empty<
                            SavePayloadEntry>(),
                        0L,
                        sourceSlotId,
                        sourceGenerationId,
                        true);

                return true;
            }

            if (unknownPayloadSnapshot ==
                    null ||
                !unknownPayloadSnapshot
                    .HasSourceProvenance ||
                !SameSlot(
                    sourceSlotId,
                    unknownPayloadSnapshot
                        .SourceSlotId) ||
                !SameGeneration(
                    sourceGenerationId,
                    unknownPayloadSnapshot
                        .SourceGenerationId) ||
                unknownPayloadSnapshot.Count !=
                    readResult.UnknownParticipantCount ||
                unknownPayloadSnapshot
                    .TotalPayloadBytes <
                    0L)
            {
                return false;
            }

            long countedBytes =
                0L;

            IReadOnlyList<SavePayloadEntry>
                unknownEntries =
                    unknownPayloadSnapshot
                        .Entries;

            for (int i = 0;
                 i < unknownEntries.Count;
                 i++)
            {
                SavePayloadEntry entry =
                    unknownEntries[i];

                if (entry == null ||
                    entry.byteLength < 0)
                {
                    return false;
                }

                try
                {
                    checked
                    {
                        countedBytes +=
                            entry.byteLength;
                    }
                }
                catch (OverflowException)
                {
                    return false;
                }
            }

            if (countedBytes !=
                unknownPayloadSnapshot
                    .TotalPayloadBytes)
            {
                return false;
            }

            defensiveCopy =
                CloneUnknownSnapshot(
                    unknownPayloadSnapshot);

            return true;
        }

        private static bool TryComputeSourceBytes(
            SaveValidatedParticipantSnapshot validated,
            out long sourceBytes)
        {
            sourceBytes =
                0L;

            IReadOnlyList<SavePayloadEntry>
                sourceEntries =
                    validated.Entries;

            for (int i = 0;
                 i < sourceEntries.Count;
                 i++)
            {
                SavePayloadEntry entry =
                    sourceEntries[i];

                if (entry == null ||
                    entry.byteLength < 0)
                {
                    sourceBytes =
                        0L;

                    return false;
                }

                try
                {
                    checked
                    {
                        sourceBytes +=
                            entry.byteLength;
                    }
                }
                catch (OverflowException)
                {
                    sourceBytes =
                        0L;

                    return false;
                }
            }

            return true;
        }

        private static SaveUnknownPayloadSnapshot
            CloneUnknownSnapshot(
                SaveUnknownPayloadSnapshot source) =>
            new SaveUnknownPayloadSnapshot(
                SaveUnknownPayloadSnapshot
                    .CloneEntries(
                        source.Entries),
                source.TotalPayloadBytes,
                source.SourceSlotId,
                source.SourceGenerationId,
                source.HasSourceProvenance);

        private static bool SameSlot(
            SaveSlotId left,
            SaveSlotId right) =>
            string.Equals(
                left.Value,
                right.Value,
                StringComparison.Ordinal);

        private static bool SameGeneration(
            SaveGenerationId left,
            SaveGenerationId right) =>
            string.Equals(
                left.Value,
                right.Value,
                StringComparison.Ordinal);

        private static PreparedLoadCreationResult
            Failure(
                PreparedLoadCreationStatus status,
                string diagnosticCode,
                string message) =>
            new PreparedLoadCreationResult(
                status,
                null,
                diagnosticCode,
                message);
    }
}
