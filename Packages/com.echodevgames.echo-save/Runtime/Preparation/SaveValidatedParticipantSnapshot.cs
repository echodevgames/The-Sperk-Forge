using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable in-memory view of participant transport entries from one
    /// fully validated current generation.
    ///
    /// The snapshot is package-internal and never represents partially
    /// validated durable data. Entry accessors return defensive copies.
    /// </summary>
    internal sealed class SaveValidatedParticipantSnapshot
    {
        private readonly SavePayloadEntry[] entries;

        internal SaveValidatedParticipantSnapshot(
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            SavePayloadEntry[] entries)
        {
            SourceSlotId =
                sourceSlotId;

            SourceGenerationId =
                sourceGenerationId;

            this.entries =
                CloneEntries(
                    entries);
        }

        internal SaveSlotId SourceSlotId { get; }

        internal SaveGenerationId SourceGenerationId { get; }

        internal int Count =>
            entries.Length;

        internal IReadOnlyList<SavePayloadEntry>
            Entries =>
            Array.AsReadOnly(
                CloneEntries(
                    entries));

        private static SavePayloadEntry[] CloneEntries(
            SavePayloadEntry[] source)
        {
            if (source == null ||
                source.Length == 0)
            {
                return Array.Empty<
                    SavePayloadEntry>();
            }

            SavePayloadEntry[] copy =
                new SavePayloadEntry[
                    source.Length];

            for (int i = 0;
                 i < source.Length;
                 i++)
            {
                SavePayloadEntry entry =
                    source[i];

                copy[i] =
                    entry == null
                        ? null
                        : new SavePayloadEntry
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

            return copy;
        }
    }
}
