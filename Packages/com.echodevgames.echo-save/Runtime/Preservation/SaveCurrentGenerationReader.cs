
using System;
using System.Collections.Generic;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Read-only M3-04 current-generation inspector.
    ///
    /// A generation must validate completely before the session unknown-payload
    /// store is replaced. Participant payload bodies are not interpreted here.
    /// </summary>
    internal sealed class SaveCurrentGenerationReader
    {
        private readonly ISaveStorageBackend storageBackend;
        private readonly ISaveSerializer serializer;
        private readonly IIntegrityProvider integrityProvider;
        private readonly SaveParticipantRegistry participantRegistry;
        private readonly SaveUnknownPayloadStore unknownPayloadStore;

        internal SaveCurrentGenerationReader(
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            IIntegrityProvider integrityProvider,
            SaveParticipantRegistry participantRegistry,
            SaveUnknownPayloadStore unknownPayloadStore)
        {
            this.storageBackend =
                storageBackend;

            this.serializer =
                serializer;

            this.integrityProvider =
                integrityProvider;

            this.participantRegistry =
                participantRegistry;

            this.unknownPayloadStore =
                unknownPayloadStore;
        }

        internal SaveCurrentGenerationReadResult
            ReadCurrent(
                SaveSlotId slotId)
        {
            if (storageBackend == null ||
                serializer == null ||
                integrityProvider == null ||
                participantRegistry == null ||
                unknownPayloadStore == null ||
                !SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return Failure(
                    SaveCurrentGenerationReadStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .CurrentReadInvalidRequest,
                    "Chronicle current-generation inspection requires valid providers, registry, unknown-payload store, and technical slot ID.",
                    slotId,
                    default);
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
                    SaveCurrentGenerationReadStatus.InvalidRequest,
                    headKeyResult.DiagnosticCode,
                    headKeyResult.Message,
                    validatedSlot,
                    default);
            }

            SaveStorageReadResult headRead =
                storageBackend.Read(
                    headKey);

            if (headRead.Result.Status ==
                SaveStorageStatus.NotFound)
            {
                return Failure(
                    SaveCurrentGenerationReadStatus.HeadUnavailable,
                    EchoSaveDiagnosticCodes
                        .CurrentReadHeadUnavailable,
                    "The requested Chronicle slot does not have a current head.",
                    validatedSlot,
                    default);
            }

            if (!headRead.Succeeded)
            {
                return Failure(
                    SaveCurrentGenerationReadStatus.HeadUnavailable,
                    EchoSaveDiagnosticCodes
                        .CurrentReadHeadUnavailable,
                    headRead.Result.ToString(),
                    validatedSlot,
                    default);
            }

            SaveSerializerResult headDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        headRead.Data),
                    out SaveHeadPointer head);

            if (!headDeserialize.Succeeded)
            {
                return Failure(
                    SaveCurrentGenerationReadStatus.HeadInvalid,
                    EchoSaveDiagnosticCodes
                        .CurrentReadHeadInvalid,
                    headDeserialize.Message,
                    validatedSlot,
                    default);
            }

            SaveDocumentValidationResult headValidation =
                SaveCommitDocumentValidator
                    .ValidateHead(
                        head);

            if (!headValidation.Succeeded ||
                !string.Equals(
                    head.slotId,
                    validatedSlot.Value,
                    StringComparison.Ordinal) ||
                !SaveGenerationId.TryParse(
                    head.currentGenerationId,
                    out SaveGenerationId currentGeneration))
            {
                return Failure(
                    SaveCurrentGenerationReadStatus.HeadInvalid,
                    EchoSaveDiagnosticCodes
                        .CurrentReadHeadInvalid,
                    "The Chronicle current head is invalid for the requested slot.",
                    validatedSlot,
                    default);
            }

            SaveStorageResult keysResult =
                SaveGenerationStorageKeys
                    .TryCreate(
                        validatedSlot,
                        currentGeneration,
                        out SaveGenerationStorageKeys keys);

            if (!keysResult.Succeeded)
            {
                return Failure(
                    SaveCurrentGenerationReadStatus.HeadInvalid,
                    EchoSaveDiagnosticCodes
                        .CurrentReadHeadInvalid,
                    keysResult.Message,
                    validatedSlot,
                    currentGeneration);
            }

            SaveStorageReadResult payloadRead =
                storageBackend.Read(
                    keys.GenerationPayload);

            if (!payloadRead.Succeeded)
            {
                return GenerationReadFailure(
                    validatedSlot,
                    currentGeneration,
                    payloadRead.Result,
                    "payload");
            }

            SaveStorageReadResult manifestRead =
                storageBackend.Read(
                    keys.GenerationManifest);

            if (!manifestRead.Succeeded)
            {
                return GenerationReadFailure(
                    validatedSlot,
                    currentGeneration,
                    manifestRead.Result,
                    "manifest");
            }

            SaveSerializerResult payloadDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        payloadRead.Data),
                    out SavePayloadDocument payload);

            if (!payloadDeserialize.Succeeded)
            {
                return GenerationInvalid(
                    validatedSlot,
                    currentGeneration,
                    payloadDeserialize.Message);
            }

            SaveSerializerResult manifestDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        manifestRead.Data),
                    out SaveManifest manifest);

            if (!manifestDeserialize.Succeeded)
            {
                return GenerationInvalid(
                    validatedSlot,
                    currentGeneration,
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
                    currentGeneration.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.generationId,
                    currentGeneration.Value,
                    StringComparison.Ordinal) ||
                manifest.commitState !=
                    SaveGenerationCommitState.Committed)
            {
                return GenerationInvalid(
                    validatedSlot,
                    currentGeneration,
                    "The Chronicle current generation does not match its selected slot/generation identity or committed state.");
            }

            SaveDocumentValidationResult documentAgreement =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadRead.Data,
                        integrityProvider);

            if (!documentAgreement.Succeeded)
            {
                return GenerationInvalid(
                    validatedSlot,
                    currentGeneration,
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
                        integrityProvider);

            if (!entryValidation.Succeeded)
            {
                return GenerationInvalid(
                    validatedSlot,
                    currentGeneration,
                    entryValidation.Message);
            }

            SaveValidatedParticipantSnapshot
                validatedParticipants =
                    new SaveValidatedParticipantSnapshot(
                        validatedSlot,
                        currentGeneration,
                        payloadEntries);

            List<SavePayloadEntry> unknownEntries =
                new List<SavePayloadEntry>();

            int knownCount =
                0;

            for (int i = 0;
                 i < payloadEntries.Length;
                 i++)
            {
                SavePayloadEntry entry =
                    payloadEntries[i];

                SaveParticipantId participantId =
                    new SaveParticipantId(
                        entry.participantId);

                if (participantRegistry.TryResolve(
                        participantId,
                        out _))
                {
                    knownCount++;

                    continue;
                }

                unknownEntries.Add(
                    SaveUnknownPayloadSnapshot
                        .CloneEntry(
                            entry));
            }

            SaveUnknownPayloadStoreResult storeResult =
                unknownPayloadStore.TryReplace(
                    unknownEntries,
                    validatedSlot,
                    currentGeneration);

            if (!storeResult.Succeeded)
            {
                return Failure(
                    SaveCurrentGenerationReadStatus.UnknownPayloadRejected,
                    storeResult.DiagnosticCode,
                    storeResult.Message,
                    validatedSlot,
                    currentGeneration);
            }

            return new SaveCurrentGenerationReadResult(
                SaveCurrentGenerationReadStatus.Succeeded,
                string.Empty,
                "The Chronicle current generation was validated and unknown payloads were preserved as opaque session data.",
                validatedSlot,
                currentGeneration,
                knownCount,
                unknownEntries.Count,
                validatedParticipants);
        }

        private static SaveCurrentGenerationReadResult
            GenerationReadFailure(
                SaveSlotId slotId,
                SaveGenerationId generationId,
                SaveStorageResult storageResult,
                string fileKind)
        {
            if (storageResult.Status ==
                SaveStorageStatus.NotFound)
            {
                return Failure(
                    SaveCurrentGenerationReadStatus.GenerationUnavailable,
                    EchoSaveDiagnosticCodes
                        .CurrentReadGenerationUnavailable,
                    $"The Chronicle current generation {fileKind} file is missing.",
                    slotId,
                    generationId);
            }

            return Failure(
                SaveCurrentGenerationReadStatus.GenerationUnavailable,
                EchoSaveDiagnosticCodes
                    .CurrentReadGenerationUnavailable,
                storageResult.ToString(),
                slotId,
                generationId);
        }

        private static SaveCurrentGenerationReadResult
            GenerationInvalid(
                SaveSlotId slotId,
                SaveGenerationId generationId,
                string message) =>
            Failure(
                SaveCurrentGenerationReadStatus.GenerationInvalid,
                EchoSaveDiagnosticCodes
                    .CurrentReadGenerationInvalid,
                message,
                slotId,
                generationId);

        private static SaveCurrentGenerationReadResult
            Failure(
                SaveCurrentGenerationReadStatus status,
                string diagnosticCode,
                string message,
                SaveSlotId slotId,
                SaveGenerationId generationId) =>
            new SaveCurrentGenerationReadResult(
                status,
                diagnosticCode,
                message,
                slotId,
                generationId,
                0,
                0);
    }
}
