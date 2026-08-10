
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Session-scoped opaque storage for participant payloads that no active
    /// participant currently claims.
    ///
    /// Records remain package transport data. This store does not interpret
    /// serialized payload contents.
    ///
    /// M3-05 binds successful read/classification state to its exact source
    /// slot/generation so carry-forward cannot reuse a stale opaque snapshot.
    /// </summary>
    internal sealed class SaveUnknownPayloadStore
    {
        internal const int DefaultMaxEntries =
            256;

        internal const long DefaultMaxAggregateBytes =
            16L * 1024L * 1024L;

        private readonly int maxEntries;
        private readonly long maxAggregateBytes;

        private SavePayloadEntry[] entries =
            Array.Empty<SavePayloadEntry>();

        private long totalPayloadBytes;
        private bool hasSourceProvenance;
        private SaveSlotId sourceSlotId;
        private SaveGenerationId sourceGenerationId;

        internal SaveUnknownPayloadStore()
            : this(
                DefaultMaxEntries,
                DefaultMaxAggregateBytes)
        {
        }

        internal SaveUnknownPayloadStore(
            int maxEntries,
            long maxAggregateBytes)
        {
            if (maxEntries < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxEntries));
            }

            if (maxAggregateBytes < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxAggregateBytes));
            }

            this.maxEntries =
                maxEntries;

            this.maxAggregateBytes =
                maxAggregateBytes;
        }

        internal int Count =>
            entries.Length;

        internal long TotalPayloadBytes =>
            totalPayloadBytes;

        internal bool HasSourceProvenance =>
            hasSourceProvenance;

        internal SaveSlotId SourceSlotId =>
            sourceSlotId;

        internal SaveGenerationId SourceGenerationId =>
            sourceGenerationId;

        internal SaveUnknownPayloadSnapshot
            GetSnapshot() =>
            new SaveUnknownPayloadSnapshot(
                entries,
                totalPayloadBytes,
                sourceSlotId,
                sourceGenerationId,
                hasSourceProvenance);

        /// <summary>
        /// Compatibility/session seeding seam with no durable provenance.
        /// Carry-forward publication must reject snapshots produced this way.
        /// </summary>
        internal SaveUnknownPayloadStoreResult
            TryReplace(
                IReadOnlyList<SavePayloadEntry> source) =>
            TryReplaceCore(
                source,
                default,
                default,
                false);

        internal SaveUnknownPayloadStoreResult
            TryReplace(
                IReadOnlyList<SavePayloadEntry> source,
                SaveSlotId sourceSlotId,
                SaveGenerationId sourceGenerationId)
        {
            if (!SaveSlotId.TryParse(
                    sourceSlotId.Value,
                    out SaveSlotId validatedSlot) ||
                !SaveGenerationId.TryParse(
                    sourceGenerationId.Value,
                    out SaveGenerationId validatedGeneration))
            {
                return Failure(
                    SaveUnknownPayloadStoreStatus.InvalidEntry,
                    EchoSaveDiagnosticCodes
                        .CarryForwardProvenanceMissing,
                    "Chronicle unknown-payload provenance requires one valid source slot and generation.");
            }

            return TryReplaceCore(
                source,
                validatedSlot,
                validatedGeneration,
                true);
        }

        internal void Clear()
        {
            entries =
                Array.Empty<SavePayloadEntry>();

            totalPayloadBytes =
                0L;

            hasSourceProvenance =
                false;

            sourceSlotId =
                default;

            sourceGenerationId =
                default;
        }

        private SaveUnknownPayloadStoreResult
            TryReplaceCore(
                IReadOnlyList<SavePayloadEntry> source,
                SaveSlotId candidateSourceSlotId,
                SaveGenerationId candidateSourceGenerationId,
                bool candidateHasSourceProvenance)
        {
            if (source == null)
            {
                return Failure(
                    SaveUnknownPayloadStoreStatus.InvalidEntry,
                    EchoSaveDiagnosticCodes
                        .UnknownPayloadInvalid,
                    "A Chronicle unknown-payload candidate collection is required.");
            }

            if (source.Count >
                maxEntries)
            {
                return LimitFailure(
                    "The Chronicle unknown-payload candidate exceeds the bounded entry-count limit.");
            }

            List<SavePayloadEntry> candidate =
                new List<SavePayloadEntry>(
                    source.Count);

            HashSet<string> identities =
                new HashSet<string>(
                    StringComparer.Ordinal);

            long candidateBytes =
                0L;

            for (int i = 0;
                 i < source.Count;
                 i++)
            {
                SavePayloadEntry entry =
                    source[i];

                SaveUnknownPayloadStoreResult validation =
                    ValidateEntry(
                        entry);

                if (!validation.Succeeded)
                {
                    return validation;
                }

                SaveParticipantId participantId =
                    new SaveParticipantId(
                        entry.participantId);

                if (!identities.Add(
                        participantId.Value))
                {
                    return Failure(
                        SaveUnknownPayloadStoreStatus.DuplicateId,
                        EchoSaveDiagnosticCodes
                            .UnknownPayloadDuplicate,
                        $"Chronicle unknown-payload candidate contains duplicate participant ID '{participantId.Value}'.");
                }

                try
                {
                    checked
                    {
                        candidateBytes +=
                            entry.byteLength;
                    }
                }
                catch (OverflowException)
                {
                    return LimitFailure(
                        "The Chronicle unknown-payload aggregate byte count exceeded the supported range.");
                }

                if (candidateBytes >
                    maxAggregateBytes)
                {
                    return LimitFailure(
                        "The Chronicle unknown-payload candidate exceeds the bounded aggregate-byte limit.");
                }

                candidate.Add(
                    SaveUnknownPayloadSnapshot
                        .CloneEntry(
                            entry));
            }

            candidate.Sort(
                CompareEntries);

            // Atomic authoritative-state replacement occurs only after all
            // entry, bounds, and provenance validation succeeds.
            entries =
                candidate.ToArray();

            totalPayloadBytes =
                candidateBytes;

            hasSourceProvenance =
                candidateHasSourceProvenance;

            sourceSlotId =
                candidateHasSourceProvenance
                    ? candidateSourceSlotId
                    : default;

            sourceGenerationId =
                candidateHasSourceProvenance
                    ? candidateSourceGenerationId
                    : default;

            return SaveUnknownPayloadStoreResult.Success(
                candidateHasSourceProvenance
                    ? "The Chronicle unknown-payload session store and source provenance were replaced atomically."
                    : "The Chronicle unknown-payload session store was replaced atomically without durable source provenance.");
        }

        private static SaveUnknownPayloadStoreResult
            ValidateEntry(
                SavePayloadEntry entry)
        {
            if (entry == null ||
                !SaveParticipantId.TryParse(
                    entry.participantId,
                    out SaveParticipantId participantId) ||
                !string.Equals(
                    participantId.Value,
                    entry.participantId,
                    StringComparison.Ordinal) ||
                entry.participantSchemaVersion <= 0 ||
                !IsCanonicalSerializerId(
                    entry.serializerId) ||
                entry.serializedPayload == null ||
                !string.IsNullOrEmpty(
                    entry.byteProviderReference) ||
                entry.byteLength < 0 ||
                string.IsNullOrEmpty(
                    entry.checksum) ||
                entry.flags != 0)
            {
                return Failure(
                    SaveUnknownPayloadStoreStatus.InvalidEntry,
                    EchoSaveDiagnosticCodes
                        .UnknownPayloadInvalid,
                    "A Chronicle unknown-payload entry contains invalid or unsupported transport metadata.");
            }

            return SaveUnknownPayloadStoreResult.Success(
                "The Chronicle unknown-payload entry metadata is structurally valid.");
        }

        private static bool IsCanonicalSerializerId(
            string value)
        {
            if (string.IsNullOrEmpty(
                    value))
            {
                return false;
            }

            try
            {
                SaveSerializerId id =
                    new SaveSerializerId(
                        value);

                return string.Equals(
                    id.Value,
                    value,
                    StringComparison.Ordinal);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static int CompareEntries(
            SavePayloadEntry left,
            SavePayloadEntry right) =>
            string.Compare(
                left.participantId,
                right.participantId,
                StringComparison.Ordinal);

        private static SaveUnknownPayloadStoreResult
            LimitFailure(
                string message) =>
            Failure(
                SaveUnknownPayloadStoreStatus.LimitExceeded,
                EchoSaveDiagnosticCodes
                    .UnknownPayloadLimitExceeded,
                message);

        private static SaveUnknownPayloadStoreResult
            Failure(
                SaveUnknownPayloadStoreStatus status,
                string diagnosticCode,
                string message) =>
            new SaveUnknownPayloadStoreResult(
                status,
                diagnosticCode,
                message);
    }
}
