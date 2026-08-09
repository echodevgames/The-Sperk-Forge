
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

        internal SaveUnknownPayloadSnapshot
            GetSnapshot() =>
            new SaveUnknownPayloadSnapshot(
                entries,
                totalPayloadBytes);

        internal SaveUnknownPayloadStoreResult
            TryReplace(
                IReadOnlyList<SavePayloadEntry> source)
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

            entries =
                candidate.ToArray();

            totalPayloadBytes =
                candidateBytes;

            return SaveUnknownPayloadStoreResult.Success(
                "The Chronicle unknown-payload session store was replaced atomically.");
        }

        internal void Clear()
        {
            entries =
                Array.Empty<SavePayloadEntry>();

            totalPayloadBytes =
                0L;
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
