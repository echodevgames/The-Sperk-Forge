
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable M3-05 merged transport batch containing fresh known captures
    /// plus opaque preserved unknown entries.
    /// </summary>
    internal sealed class SaveMergedParticipantTransportBatch
    {
        private readonly SavePayloadEntry[] payloadEntries;
        private readonly SavePayloadInventoryEntry[] inventoryEntries;

        internal SaveMergedParticipantTransportBatch(
            SavePayloadEntry[] payloadEntries,
            SavePayloadInventoryEntry[] inventoryEntries,
            int freshParticipantCount,
            int preservedUnknownCount,
            long totalPayloadBytes)
        {
            this.payloadEntries =
                ClonePayloadEntries(
                    payloadEntries);

            this.inventoryEntries =
                CloneInventoryEntries(
                    inventoryEntries);

            FreshParticipantCount =
                freshParticipantCount;

            PreservedUnknownCount =
                preservedUnknownCount;

            TotalPayloadBytes =
                totalPayloadBytes;
        }

        internal int Count =>
            payloadEntries.Length;

        internal int FreshParticipantCount { get; }

        internal int PreservedUnknownCount { get; }

        internal long TotalPayloadBytes { get; }

        internal IReadOnlyList<SavePayloadEntry>
            PayloadEntries =>
            Array.AsReadOnly(
                ClonePayloadEntries(
                    payloadEntries));

        internal IReadOnlyList<SavePayloadInventoryEntry>
            InventoryEntries =>
            Array.AsReadOnly(
                CloneInventoryEntries(
                    inventoryEntries));

        internal SavePayloadEntry[]
            CopyPayloadEntries() =>
            ClonePayloadEntries(
                payloadEntries);

        internal SavePayloadInventoryEntry[]
            CopyInventoryEntries() =>
            CloneInventoryEntries(
                inventoryEntries);

        private static SavePayloadEntry[]
            ClonePayloadEntries(
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
                    SaveUnknownPayloadSnapshot
                        .CloneEntry(
                            source[i]);
            }

            return copy;
        }

        private static SavePayloadInventoryEntry[]
            CloneInventoryEntries(
                IReadOnlyList<SavePayloadInventoryEntry> source)
        {
            if (source == null ||
                source.Count == 0)
            {
                return Array.Empty<SavePayloadInventoryEntry>();
            }

            SavePayloadInventoryEntry[] copy =
                new SavePayloadInventoryEntry[
                    source.Count];

            for (int i = 0;
                 i < source.Count;
                 i++)
            {
                SavePayloadInventoryEntry entry =
                    source[i];

                copy[i] =
                    entry == null
                        ? null
                        : new SavePayloadInventoryEntry
                        {
                            participantId =
                                entry.participantId,
                            participantSchemaVersion =
                                entry.participantSchemaVersion,
                            serializerId =
                                entry.serializerId,
                            required =
                                entry.required,
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
