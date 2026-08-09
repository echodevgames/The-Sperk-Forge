
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable in-memory result of one participant capture batch.
    ///
    /// Entry accessors return defensive copies because SavePayloadEntry and
    /// SavePayloadInventoryEntry are mutable transport document records.
    /// </summary>
    internal sealed class SaveParticipantCaptureBatchResult
    {
        private readonly SavePayloadEntry[]
            payloadEntries;

        private readonly SavePayloadInventoryEntry[]
            inventoryEntries;

        internal SaveParticipantCaptureBatchResult(
            SaveParticipantCaptureBatchStatus status,
            SaveParticipantId failingParticipantId,
            string diagnosticCode,
            string message,
            SavePayloadEntry[] payloadEntries,
            SavePayloadInventoryEntry[] inventoryEntries,
            long totalPayloadBytes)
        {
            Status = status;
            FailingParticipantId =
                failingParticipantId;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;

            this.payloadEntries =
                ClonePayloadEntries(
                    payloadEntries);

            this.inventoryEntries =
                CloneInventoryEntries(
                    inventoryEntries);

            TotalPayloadBytes =
                totalPayloadBytes;
        }

        internal SaveParticipantCaptureBatchStatus
            Status
        {
            get;
        }

        internal SaveParticipantId FailingParticipantId
        {
            get;
        }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal long TotalPayloadBytes { get; }

        internal int Count =>
            payloadEntries.Length;

        internal bool Succeeded =>
            Status ==
            SaveParticipantCaptureBatchStatus.Succeeded;

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

        internal static SaveParticipantCaptureBatchResult
            Success(
                SavePayloadEntry[] payloadEntries,
                SavePayloadInventoryEntry[] inventoryEntries,
                long totalPayloadBytes) =>
            new SaveParticipantCaptureBatchResult(
                SaveParticipantCaptureBatchStatus.Succeeded,
                default,
                string.Empty,
                "The Chronicle participant capture batch was constructed successfully.",
                payloadEntries,
                inventoryEntries,
                totalPayloadBytes);

        internal static SaveParticipantCaptureBatchResult
            Failure(
                SaveParticipantCaptureBatchStatus status,
                SaveParticipantId participantId,
                string diagnosticCode,
                string message) =>
            new SaveParticipantCaptureBatchResult(
                status,
                participantId,
                diagnosticCode,
                message,
                Array.Empty<SavePayloadEntry>(),
                Array.Empty<SavePayloadInventoryEntry>(),
                0L);

        private static SavePayloadEntry[]
            ClonePayloadEntries(
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

        private static SavePayloadInventoryEntry[]
            CloneInventoryEntries(
                SavePayloadInventoryEntry[] source)
        {
            if (source == null ||
                source.Length == 0)
            {
                return Array.Empty<
                    SavePayloadInventoryEntry>();
            }

            SavePayloadInventoryEntry[] copy =
                new SavePayloadInventoryEntry[
                    source.Length];

            for (int i = 0;
                 i < source.Length;
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
