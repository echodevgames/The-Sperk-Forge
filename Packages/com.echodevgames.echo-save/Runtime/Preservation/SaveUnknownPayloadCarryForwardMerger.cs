
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Side-effect-free M3-05 merge authority.
    ///
    /// Fresh known participant captures and preserved opaque unknown entries
    /// are validated, ownership-checked, combined, and sorted without storage
    /// mutation or unknown-payload interpretation.
    /// </summary>
    internal sealed class SaveUnknownPayloadCarryForwardMerger
    {
        private readonly IIntegrityProvider integrityProvider;
        private readonly SaveParticipantRegistry participantRegistry;

        internal SaveUnknownPayloadCarryForwardMerger(
            IIntegrityProvider integrityProvider,
            SaveParticipantRegistry participantRegistry)
        {
            this.integrityProvider =
                integrityProvider;

            this.participantRegistry =
                participantRegistry;
        }

        internal SaveUnknownPayloadMergeResult Merge(
            SaveParticipantCaptureBatchResult freshCapture,
            SaveUnknownPayloadSnapshot unknownSnapshot)
        {
            if (integrityProvider == null ||
                participantRegistry == null ||
                freshCapture == null ||
                unknownSnapshot == null)
            {
                return Failure(
                    SaveUnknownPayloadMergeStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .CarryForwardInvalidRequest,
                    "Chronicle carry-forward merge requires integrity, registry, one successful fresh capture, and one unknown snapshot.");
            }

            if (!unknownSnapshot.HasSourceProvenance ||
                !SaveSlotId.TryParse(
                    unknownSnapshot.SourceSlotId.Value,
                    out _) ||
                !SaveGenerationId.TryParse(
                    unknownSnapshot.SourceGenerationId.Value,
                    out _))
            {
                return Failure(
                    SaveUnknownPayloadMergeStatus.MissingProvenance,
                    EchoSaveDiagnosticCodes
                        .CarryForwardProvenanceMissing,
                    "Chronicle carry-forward merge requires valid source slot/generation provenance.");
            }

            SaveDocumentValidationResult freshValidation =
                SaveParticipantPublicationBatchValidator
                    .ValidateCaptureBatch(
                        freshCapture,
                        integrityProvider,
                        out SavePayloadEntry[] freshPayload,
                        out SavePayloadInventoryEntry[] freshInventory);

            if (!freshValidation.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadMergeStatus.FreshCaptureInvalid,
                    string.IsNullOrEmpty(
                        freshValidation.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .CarryForwardMergeInvalid
                        : freshValidation.DiagnosticCode,
                    freshValidation.Message);
            }

            IReadOnlyList<SavePayloadEntry> unknownSource =
                unknownSnapshot.Entries;

            SavePayloadEntry[] unknownPayload =
                SaveUnknownPayloadSnapshot
                    .CloneEntries(
                        unknownSource);

            SavePayloadInventoryEntry[] unknownInventory =
                new SavePayloadInventoryEntry[
                    unknownPayload.Length];

            long unknownBytes =
                0L;

            for (int i = 0;
                 i < unknownPayload.Length;
                 i++)
            {
                SavePayloadEntry entry =
                    unknownPayload[i];

                if (entry == null)
                {
                    return Failure(
                        SaveUnknownPayloadMergeStatus.UnknownPayloadInvalid,
                        EchoSaveDiagnosticCodes
                            .CarryForwardMergeInvalid,
                        "Chronicle carry-forward encountered a null preserved unknown entry.");
                }

                unknownInventory[i] =
                    InventoryFromPayload(
                        entry);

                try
                {
                    checked
                    {
                        unknownBytes +=
                            entry.byteLength;
                    }
                }
                catch (OverflowException)
                {
                    return Failure(
                        SaveUnknownPayloadMergeStatus.UnknownPayloadInvalid,
                        EchoSaveDiagnosticCodes
                            .CarryForwardMergeInvalid,
                        "Chronicle preserved unknown aggregate bytes exceed the supported range.");
                }
            }

            if (unknownBytes !=
                unknownSnapshot.TotalPayloadBytes)
            {
                return Failure(
                    SaveUnknownPayloadMergeStatus.UnknownPayloadInvalid,
                    EchoSaveDiagnosticCodes
                        .CarryForwardMergeInvalid,
                    "Chronicle preserved unknown aggregate bytes do not match snapshot metadata.");
            }

            SaveDocumentValidationResult unknownValidation =
                SaveParticipantPublicationBatchValidator
                    .ValidateStoredEntries(
                        unknownPayload,
                        unknownInventory,
                        integrityProvider);

            if (!unknownValidation.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadMergeStatus.UnknownPayloadInvalid,
                    string.IsNullOrEmpty(
                        unknownValidation.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .CarryForwardMergeInvalid
                        : unknownValidation.DiagnosticCode,
                    unknownValidation.Message);
            }

            for (int i = 0;
                 i < freshPayload.Length;
                 i++)
            {
                SaveParticipantId freshId =
                    new SaveParticipantId(
                        freshPayload[i].participantId);

                if (!participantRegistry
                        .TryResolveDescriptor(
                            freshId,
                            out SaveParticipantDescriptor owner) ||
                    owner.Id !=
                        freshId)
                {
                    return Failure(
                        SaveUnknownPayloadMergeStatus.FreshCaptureInvalid,
                        EchoSaveDiagnosticCodes
                            .CarryForwardMergeInvalid,
                        $"Fresh Chronicle capture '{freshId.Value}' is not currently owned as a canonical participant identity.",
                        freshId,
                        default);
                }
            }

            for (int i = 0;
                 i < unknownPayload.Length;
                 i++)
            {
                SaveParticipantId persistedId =
                    new SaveParticipantId(
                        unknownPayload[i]
                            .participantId);

                if (participantRegistry
                    .TryResolveDescriptor(
                        persistedId,
                        out SaveParticipantDescriptor owner))
                {
                    return Failure(
                        SaveUnknownPayloadMergeStatus.OwnershipCollision,
                        EchoSaveDiagnosticCodes
                            .CarryForwardOwnershipCollision,
                        $"Preserved unknown participant ID '{persistedId.Value}' is now claimed by active canonical participant '{owner.Id.Value}'.",
                        persistedId,
                        owner.Id);
                }
            }

            List<EntryPair> merged =
                new List<EntryPair>(
                    freshPayload.Length +
                    unknownPayload.Length);

            for (int i = 0;
                 i < freshPayload.Length;
                 i++)
            {
                merged.Add(
                    new EntryPair(
                        freshPayload[i],
                        freshInventory[i]));
            }

            for (int i = 0;
                 i < unknownPayload.Length;
                 i++)
            {
                merged.Add(
                    new EntryPair(
                        unknownPayload[i],
                        unknownInventory[i]));
            }

            merged.Sort(
                ComparePairs);

            SavePayloadEntry[] mergedPayload =
                new SavePayloadEntry[
                    merged.Count];

            SavePayloadInventoryEntry[] mergedInventory =
                new SavePayloadInventoryEntry[
                    merged.Count];

            long totalBytes =
                0L;

            for (int i = 0;
                 i < merged.Count;
                 i++)
            {
                mergedPayload[i] =
                    SaveUnknownPayloadSnapshot
                        .CloneEntry(
                            merged[i].Payload);

                mergedInventory[i] =
                    CloneInventory(
                        merged[i].Inventory);

                try
                {
                    checked
                    {
                        totalBytes +=
                            mergedPayload[i]
                                .byteLength;
                    }
                }
                catch (OverflowException)
                {
                    return Failure(
                        SaveUnknownPayloadMergeStatus.MergeInvalid,
                        EchoSaveDiagnosticCodes
                            .CarryForwardMergeInvalid,
                        "Chronicle merged participant aggregate bytes exceed the supported range.");
                }
            }

            SaveDocumentValidationResult mergedValidation =
                SaveParticipantPublicationBatchValidator
                    .ValidateStoredEntries(
                        mergedPayload,
                        mergedInventory,
                        integrityProvider);

            if (!mergedValidation.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadMergeStatus.MergeInvalid,
                    string.IsNullOrEmpty(
                        mergedValidation.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .CarryForwardMergeInvalid
                        : mergedValidation.DiagnosticCode,
                    mergedValidation.Message);
            }

            SaveMergedParticipantTransportBatch batch =
                new SaveMergedParticipantTransportBatch(
                    mergedPayload,
                    mergedInventory,
                    freshPayload.Length,
                    unknownPayload.Length,
                    totalBytes);

            return new SaveUnknownPayloadMergeResult(
                SaveUnknownPayloadMergeStatus.Succeeded,
                string.Empty,
                "Chronicle fresh known captures and opaque unknown payloads merged successfully.",
                default,
                default,
                batch);
        }

        private static SavePayloadInventoryEntry
            InventoryFromPayload(
                SavePayloadEntry payload) =>
            new SavePayloadInventoryEntry
            {
                participantId =
                    payload.participantId,
                participantSchemaVersion =
                    payload.participantSchemaVersion,
                serializerId =
                    payload.serializerId,
                required =
                    payload.required,
                byteLength =
                    payload.byteLength,
                checksum =
                    payload.checksum,
                flags =
                    payload.flags
            };

        private static SavePayloadInventoryEntry
            CloneInventory(
                SavePayloadInventoryEntry entry)
        {
            if (entry == null)
            {
                return null;
            }

            return new SavePayloadInventoryEntry
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

        private static int ComparePairs(
            EntryPair left,
            EntryPair right) =>
            string.Compare(
                left.Payload.participantId,
                right.Payload.participantId,
                StringComparison.Ordinal);

        private static SaveUnknownPayloadMergeResult
            Failure(
                SaveUnknownPayloadMergeStatus status,
                string diagnosticCode,
                string message,
                SaveParticipantId failingPersistedId = default,
                SaveParticipantId currentOwnerId = default) =>
            new SaveUnknownPayloadMergeResult(
                status,
                diagnosticCode,
                message,
                failingPersistedId,
                currentOwnerId,
                null);

        private readonly struct EntryPair
        {
            internal EntryPair(
                SavePayloadEntry payload,
                SavePayloadInventoryEntry inventory)
            {
                Payload =
                    payload;

                Inventory =
                    inventory;
            }

            internal SavePayloadEntry Payload { get; }

            internal SavePayloadInventoryEntry Inventory { get; }
        }
    }
}
