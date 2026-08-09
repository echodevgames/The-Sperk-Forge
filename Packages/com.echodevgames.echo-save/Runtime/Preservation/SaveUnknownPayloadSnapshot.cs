
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable view of the current opaque unknown-payload session state.
    ///
    /// SavePayloadEntry is a mutable transport record, so every access returns
    /// defensive clones rather than the store's authoritative records.
    /// </summary>
    internal sealed class SaveUnknownPayloadSnapshot
    {
        private readonly SavePayloadEntry[] entries;

        internal SaveUnknownPayloadSnapshot(
            SavePayloadEntry[] entries,
            long totalPayloadBytes)
        {
            this.entries =
                CloneEntries(
                    entries);

            TotalPayloadBytes =
                totalPayloadBytes;
        }

        internal int Count =>
            entries.Length;

        internal long TotalPayloadBytes { get; }

        internal IReadOnlyList<SavePayloadEntry>
            Entries =>
            Array.AsReadOnly(
                CloneEntries(
                    entries));

        internal bool TryGet(
            SaveParticipantId participantId,
            out SavePayloadEntry entry)
        {
            entry =
                null;

            if (!SaveParticipantId.TryParse(
                    participantId.Value,
                    out SaveParticipantId validated))
            {
                return false;
            }

            for (int i = 0;
                 i < entries.Length;
                 i++)
            {
                if (string.Equals(
                        entries[i].participantId,
                        validated.Value,
                        StringComparison.Ordinal))
                {
                    entry =
                        CloneEntry(
                            entries[i]);

                    return true;
                }
            }

            return false;
        }

        internal static SavePayloadEntry CloneEntry(
            SavePayloadEntry entry)
        {
            if (entry == null)
            {
                return null;
            }

            return new SavePayloadEntry
            {
                participantId =
                    entry.participantId,
                participantSchemaVersion =
                    entry.participantSchemaVersion,
                serializerId =
                    entry.serializerId,
                required =
                    entry.required,
                serializedPayload =
                    entry.serializedPayload,
                byteProviderReference =
                    entry.byteProviderReference,
                byteLength =
                    entry.byteLength,
                checksum =
                    entry.checksum,
                flags =
                    entry.flags
            };
        }

        internal static SavePayloadEntry[] CloneEntries(
            IReadOnlyList<SavePayloadEntry> source)
        {
            if (source == null ||
                source.Count == 0)
            {
                return Array.Empty<SavePayloadEntry>();
            }

            SavePayloadEntry[] copy =
                new SavePayloadEntry[
                    source.Count];

            for (int i = 0;
                 i < source.Count;
                 i++)
            {
                copy[i] =
                    CloneEntry(
                        source[i]);
            }

            return copy;
        }
    }
}
