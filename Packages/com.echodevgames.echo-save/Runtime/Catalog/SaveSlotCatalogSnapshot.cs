
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable deterministic in-memory slot-catalog snapshot.
    /// </summary>
    public sealed class SaveSlotCatalogSnapshot
    {
        private readonly ReadOnlyCollection<SaveSlotCatalogEntry> entries;

        internal SaveSlotCatalogSnapshot(
            SaveSlotCatalogEntry[] entries)
        {
            SaveSlotCatalogEntry[] copy =
                entries == null
                    ? Array.Empty<SaveSlotCatalogEntry>()
                    : (SaveSlotCatalogEntry[])entries.Clone();

            this.entries =
                Array.AsReadOnly(copy);

            int healthyCount = 0;

            for (int i = 0;
                 i < copy.Length;
                 i++)
            {
                if (copy[i] != null &&
                    copy[i].IsSelectable)
                {
                    healthyCount++;
                }
            }

            HealthyCount = healthyCount;
            DegradedCount =
                copy.Length - healthyCount;
        }

        public IReadOnlyList<SaveSlotCatalogEntry> Entries =>
            entries;

        public int Count =>
            entries.Count;

        public int HealthyCount { get; }

        public int DegradedCount { get; }

        internal static SaveSlotCatalogSnapshot Empty { get; } =
            new SaveSlotCatalogSnapshot(
                Array.Empty<SaveSlotCatalogEntry>());

        public bool TryGetEntry(
            SaveSlotId slotId,
            out SaveSlotCatalogEntry entry)
        {
            entry = null;

            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validated))
            {
                return false;
            }

            int low = 0;
            int high =
                entries.Count - 1;

            while (low <= high)
            {
                int middle =
                    low +
                    ((high - low) / 2);

                SaveSlotCatalogEntry candidate =
                    entries[middle];

                int comparison =
                    string.CompareOrdinal(
                        candidate.SlotId.Value,
                        validated.Value);

                if (comparison == 0)
                {
                    entry =
                        candidate;

                    return true;
                }

                if (comparison < 0)
                {
                    low =
                        middle + 1;
                }
                else
                {
                    high =
                        middle - 1;
                }
            }

            return false;
        }
    }
}
