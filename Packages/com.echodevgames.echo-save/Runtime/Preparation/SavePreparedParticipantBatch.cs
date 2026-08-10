using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Deterministic all-or-nothing in-memory participant preparation batch.
    /// </summary>
    internal sealed class SavePreparedParticipantBatch
    {
        private readonly SavePreparedParticipantEntry[] entries;

        internal SavePreparedParticipantBatch(
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            SavePreparedParticipantEntry[] entries)
        {
            SourceSlotId =
                sourceSlotId;

            SourceGenerationId =
                sourceGenerationId;

            this.entries =
                entries == null
                    ? Array.Empty<
                        SavePreparedParticipantEntry>()
                    : (SavePreparedParticipantEntry[])
                        entries.Clone();
        }

        internal SaveSlotId SourceSlotId { get; }

        internal SaveGenerationId SourceGenerationId { get; }

        internal int Count =>
            entries.Length;

        internal IReadOnlyList<SavePreparedParticipantEntry>
            Entries =>
            Array.AsReadOnly(
                (SavePreparedParticipantEntry[])
                    entries.Clone());
    }
}
