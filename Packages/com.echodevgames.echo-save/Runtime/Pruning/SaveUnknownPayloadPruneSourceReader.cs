
using System;
using System.Text;

namespace EchoDevGames.EchoSave
{
    internal enum SaveUnknownPayloadPruneSourceStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        HeadUnavailable = 2,
        SourceInvalid = 3,
        SourceStale = 4
    }

    internal sealed class SaveUnknownPayloadPruneSourceSnapshot
    {
        internal SaveUnknownPayloadPruneSourceSnapshot(
            SaveSlotId slotId,
            SaveGenerationId generationId,
            long headUpdateSequence,
            string provenanceFingerprint,
            SaveManifest manifest,
            SavePayloadEntry[] payloadEntries,
            SavePayloadInventoryEntry[] inventoryEntries)
        {
            SlotId = slotId;
            GenerationId = generationId;
            HeadUpdateSequence = headUpdateSequence;
            ProvenanceFingerprint =
                provenanceFingerprint ?? string.Empty;
            Manifest = manifest;
            PayloadEntries =
                SaveUnknownPayloadSnapshot.CloneEntries(
                    payloadEntries);
            InventoryEntries =
                CloneInventory(
                    inventoryEntries);
        }

        internal SaveSlotId SlotId { get; }

        internal SaveGenerationId GenerationId { get; }

        internal long HeadUpdateSequence { get; }

        internal string ProvenanceFingerprint { get; }

        internal SaveManifest Manifest { get; }

        internal SavePayloadEntry[] PayloadEntries { get; }

        internal SavePayloadInventoryEntry[] InventoryEntries { get; }

        private static SavePayloadInventoryEntry[] CloneInventory(
            SavePayloadInventoryEntry[] source)
        {
            source =
                source ??
                Array.Empty<SavePayloadInventoryEntry>();

            SavePayloadInventoryEntry[] copy =
                new SavePayloadInventoryEntry[
                    source.Length];

            for (int i = 0;
                 i < source.Length;
                 i++)
            {
                SavePayloadInventoryEntry item =
                    source[i];

                copy[i] =
                    item == null
                        ? null
                        : new SavePayloadInventoryEntry
                        {
                            participantId =
                                item.participantId,
                            participantSchemaVersion =
                                item.participantSchemaVersion,
                            serializerId =
                                item.serializerId,
                            required =
                                item.required,
                            byteLength =
                                item.byteLength,
                            checksum =
                                item.checksum,
                            flags =
                                item.flags
                        };
            }

            return copy;
        }
    }

    internal sealed class SaveUnknownPayloadPruneSourceReadResult
    {
        internal SaveUnknownPayloadPruneSourceReadResult(
            SaveUnknownPayloadPruneSourceStatus status,
            string diagnosticCode,
            string message,
            SaveUnknownPayloadPruneSourceSnapshot snapshot)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            Snapshot = snapshot;
        }

        internal SaveUnknownPayloadPruneSourceStatus Status { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal SaveUnknownPayloadPruneSourceSnapshot Snapshot { get; }

        internal bool Succeeded =>
            Status ==
                SaveUnknownPayloadPruneSourceStatus.Succeeded &&
            Snapshot != null;
    }

    internal sealed class SaveUnknownPayloadPruneSourceReader
    {
        private readonly ISaveStorageBackend storage;
        private readonly IIntegrityProvider integrity;
        private readonly SavePackageDocumentReader packageDocumentReader;

        internal SaveUnknownPayloadPruneSourceReader(
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            IIntegrityProvider integrity)
        {
            this.storage =
                storage ??
                throw new ArgumentNullException(
                    nameof(storage));

            if (serializer == null)
            {
                throw new ArgumentNullException(
                    nameof(serializer));
            }

            this.integrity =
                integrity ??
                throw new ArgumentNullException(
                    nameof(integrity));

            packageDocumentReader =
                new SavePackageDocumentReader(
                    serializer,
                    SavePackageDocumentMigrationRegistry
                        .CreateProduction());
        }

        internal SaveUnknownPayloadPruneSourceReadResult Read(
            SaveSlotId slotId)
        {
            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.InvalidRequest,
                    "ECHOSAVE-PRUNE-SOURCE-REQUEST",
                    "Chronicle unknown-prune planning requires one valid technical slot ID.");
            }

            SaveStorageResult headKeyResult =
                SaveStorageKey.TryCreate(
                    "slots/" +
                    validatedSlot.Value +
                    "/head.json",
                    out SaveStorageKey headKey);

            if (!headKeyResult.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.InvalidRequest,
                    headKeyResult.DiagnosticCode,
                    headKeyResult.Message);
            }

            SaveStorageReadResult headRead =
                storage.Read(
                    headKey);

            if (!headRead.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.HeadUnavailable,
                    string.IsNullOrEmpty(
                        headRead.Result.DiagnosticCode)
                        ? "ECHOSAVE-PRUNE-HEAD"
                        : headRead.Result.DiagnosticCode,
                    "Chronicle unknown-prune planning could not read the authoritative slot head. " +
                    headRead.Result.Message);
            }

            SavePackageDocumentReadResult headDeserialize =
                packageDocumentReader.ReadCurrent(
                    Encoding.UTF8.GetString(
                        headRead.Data),
                    SaveDocumentKinds.HeadPointer,
                    out SaveHeadPointer head);

            if (!headDeserialize.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    "ECHOSAVE-PRUNE-HEAD-INVALID",
                    headDeserialize.Message);
            }

            SaveDocumentValidationResult headValidation =
                SaveCommitDocumentValidator.ValidateHead(
                    head);

            if (!headValidation.Succeeded ||
                !string.Equals(
                    head.slotId,
                    validatedSlot.Value,
                    StringComparison.Ordinal) ||
                !SaveGenerationId.TryParse(
                    head.currentGenerationId,
                    out SaveGenerationId generationId))
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    "ECHOSAVE-PRUNE-HEAD-INVALID",
                    "Chronicle unknown-prune planning requires one valid current head matching the requested slot.");
            }

            SaveStorageResult keysResult =
                SaveGenerationStorageKeys.TryCreate(
                    validatedSlot,
                    generationId,
                    out SaveGenerationStorageKeys keys);

            if (!keysResult.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    keysResult.DiagnosticCode,
                    keysResult.Message);
            }

            SaveStorageReadResult payloadRead =
                storage.Read(
                    keys.GenerationPayload);

            SaveStorageReadResult manifestRead =
                storage.Read(
                    keys.GenerationManifest);

            if (!payloadRead.Succeeded ||
                !manifestRead.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    "ECHOSAVE-PRUNE-SOURCE-READ",
                    "Chronicle unknown-prune planning requires the complete current payload and manifest.");
            }

            SavePackageDocumentReadResult payloadDeserialize =
                packageDocumentReader.ReadCurrent(
                    Encoding.UTF8.GetString(
                        payloadRead.Data),
                    SaveDocumentKinds.Payload,
                    out SavePayloadDocument payload);

            if (!payloadDeserialize.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    "ECHOSAVE-PRUNE-PAYLOAD",
                    payloadDeserialize.Message);
            }

            SavePackageDocumentReadResult manifestDeserialize =
                packageDocumentReader.ReadCurrent(
                    Encoding.UTF8.GetString(
                        manifestRead.Data),
                    SaveDocumentKinds.Manifest,
                    out SaveManifest manifest);

            if (!manifestDeserialize.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    "ECHOSAVE-PRUNE-MANIFEST",
                    manifestDeserialize.Message);
            }

            if (!string.Equals(
                    payload.slotId,
                    validatedSlot.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.slotId,
                    validatedSlot.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    payload.generationId,
                    generationId.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.generationId,
                    generationId.Value,
                    StringComparison.Ordinal) ||
                manifest.commitState !=
                    SaveGenerationCommitState.Committed)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    "ECHOSAVE-PRUNE-DOCUMENT",
                    "Chronicle unknown-prune planning requires one committed current generation whose document identities agree.");
            }

            SaveDocumentValidationResult documentAgreement =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadRead.Data,
                        integrity);

            if (!documentAgreement.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    documentAgreement.DiagnosticCode,
                    documentAgreement.Message);
            }

            SavePayloadEntry[] payloadEntries =
                payload.entries ??
                Array.Empty<SavePayloadEntry>();

            SavePayloadInventoryEntry[] inventoryEntries =
                manifest.payloadEntries ??
                Array.Empty<SavePayloadInventoryEntry>();

            SaveDocumentValidationResult entryValidation =
                SaveParticipantPublicationBatchValidator
                    .ValidateStoredEntries(
                        payloadEntries,
                        inventoryEntries,
                        integrity);

            if (!entryValidation.Succeeded)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    entryValidation.DiagnosticCode,
                    entryValidation.Message);
            }

            if (!TryFingerprint(
                    headRead.Data,
                    out string headHash) ||
                !TryFingerprint(
                    manifestRead.Data,
                    out string manifestHash) ||
                !TryFingerprint(
                    payloadRead.Data,
                    out string payloadHash))
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceInvalid,
                    "ECHOSAVE-PRUNE-FINGERPRINT",
                    "Chronicle unknown-prune planning could not fingerprint the complete source provenance.");
            }

            return new SaveUnknownPayloadPruneSourceReadResult(
                SaveUnknownPayloadPruneSourceStatus.Succeeded,
                string.Empty,
                "Chronicle unknown-prune source provenance is valid.",
                new SaveUnknownPayloadPruneSourceSnapshot(
                    validatedSlot,
                    generationId,
                    head.updateSequence,
                    headHash +
                    "|" +
                    manifestHash +
                    "|" +
                    payloadHash,
                    manifest,
                    payloadEntries,
                    inventoryEntries));
        }

        internal SaveUnknownPayloadPruneSourceReadResult Revalidate(
            SaveUnknownPayloadPruneSourceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.InvalidRequest,
                    "ECHOSAVE-PRUNE-PLAN",
                    "Chronicle unknown-prune source revalidation requires one prepared source snapshot.");
            }

            SaveUnknownPayloadPruneSourceReadResult fresh =
                Read(
                    snapshot.SlotId);

            if (!fresh.Succeeded)
            {
                return fresh;
            }

            SaveUnknownPayloadPruneSourceSnapshot current =
                fresh.Snapshot;

            if (current.GenerationId !=
                    snapshot.GenerationId ||
                current.HeadUpdateSequence !=
                    snapshot.HeadUpdateSequence ||
                !string.Equals(
                    current.ProvenanceFingerprint,
                    snapshot.ProvenanceFingerprint,
                    StringComparison.Ordinal))
            {
                return Failure(
                    SaveUnknownPayloadPruneSourceStatus.SourceStale,
                    "ECHOSAVE-PRUNE-STALE",
                    "The Chronicle unknown-prune source changed after Preview.");
            }

            return fresh;
        }

        private bool TryFingerprint(
            byte[] data,
            out string hash)
        {
            SaveIntegrityResult result =
                integrity.Calculate(
                    data,
                    out hash);

            return result.Succeeded;
        }

        private static SaveUnknownPayloadPruneSourceReadResult Failure(
            SaveUnknownPayloadPruneSourceStatus status,
            string diagnosticCode,
            string message) =>
            new SaveUnknownPayloadPruneSourceReadResult(
                status,
                diagnosticCode,
                message,
                null);
    }
}
