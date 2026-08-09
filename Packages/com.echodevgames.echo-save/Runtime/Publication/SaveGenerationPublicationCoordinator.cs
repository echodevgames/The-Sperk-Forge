
using System;
using System.Globalization;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Bounded Chronicle generation publication coordinator.
    ///
    /// M2-04 established the empty/transport transaction. M3-03 adds a
    /// participant-backed technical integration entry point that reuses the
    /// same candidate-write, immutable-generation, verification, and head-last
    /// transaction. This remains a bounded technical publication seam.
    /// </summary>
    internal sealed class SaveGenerationPublicationCoordinator
    {
        private readonly ISaveStorageBackend storageBackend;
        private readonly ISaveSerializer serializer;
        private readonly IIntegrityProvider integrityProvider;
        private readonly ISaveClock clock;
        private readonly Func<SaveGenerationId> generationFactory;

        internal SaveGenerationPublicationCoordinator(
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            IIntegrityProvider integrityProvider)
            : this(
                storageBackend,
                serializer,
                integrityProvider,
                SystemSaveClock.Instance,
                SaveGenerationId.NewId)
        {
        }

        internal SaveGenerationPublicationCoordinator(
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            IIntegrityProvider integrityProvider,
            ISaveClock clock,
            Func<SaveGenerationId> generationFactory)
        {
            this.storageBackend =
                storageBackend;
            this.serializer =
                serializer;
            this.integrityProvider =
                integrityProvider;
            this.clock =
                clock;
            this.generationFactory =
                generationFactory;
        }

        internal SaveGenerationPublicationResult
            PublishEmptyTransportGeneration(
                SaveSlotId slotId,
                string projectId,
                string projectVersion,
                string buildId,
                string displayName) =>
            PublishTransportGeneration(
                slotId,
                projectId,
                projectVersion,
                buildId,
                displayName,
                Array.Empty<SavePayloadEntry>(),
                Array.Empty<SavePayloadInventoryEntry>(),
                false);

        internal SaveGenerationPublicationResult
            PublishParticipantTransportGeneration(
                SaveSlotId slotId,
                string projectId,
                string projectVersion,
                string buildId,
                string displayName,
                SaveParticipantCaptureBatchResult captureBatch)
        {
            SaveDocumentValidationResult batchValidation =
                SaveParticipantPublicationBatchValidator
                    .ValidateCaptureBatch(
                        captureBatch,
                        integrityProvider,
                        out SavePayloadEntry[] payloadEntries,
                        out SavePayloadInventoryEntry[] inventoryEntries);

            if (!batchValidation.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.InvalidRequest,
                    string.IsNullOrEmpty(
                        batchValidation.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .PublicationParticipantBatchInvalid
                        : batchValidation.DiagnosticCode,
                    batchValidation.Message,
                    slotId,
                    default,
                    false);
            }

            return PublishTransportGeneration(
                slotId,
                projectId,
                projectVersion,
                buildId,
                displayName,
                payloadEntries,
                inventoryEntries,
                true);
        }

        private SaveGenerationPublicationResult
            PublishTransportGeneration(
                SaveSlotId slotId,
                string projectId,
                string projectVersion,
                string buildId,
                string displayName,
                SavePayloadEntry[] payloadEntries,
                SavePayloadInventoryEntry[] inventoryEntries,
                bool participantBacked)
        {
            if (storageBackend == null ||
                serializer == null ||
                integrityProvider == null ||
                clock == null ||
                generationFactory == null ||
                payloadEntries == null ||
                inventoryEntries == null ||
                payloadEntries.Length !=
                    inventoryEntries.Length ||
                !SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return Failure(
                    SaveGenerationPublicationStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .PublicationInvalidRequest,
                    "Chronicle generation publication requires valid providers, matching payload/inventory arrays, and one valid slot ID.",
                    slotId,
                    default,
                    false);
            }

            if (!(storageBackend is
                ISaveStoragePublicationBackend publicationBackend))
            {
                return Failure(
                    SaveGenerationPublicationStatus.BackendUnsupported,
                    EchoSaveDiagnosticCodes
                        .PublicationBackendUnsupported,
                    "The active Chronicle storage backend does not provide the required publication capability seam.",
                    validatedSlot,
                    default,
                    false);
            }

            SaveStoragePublicationCapabilities capabilities =
                publicationBackend.PublicationCapabilities;

            if (!capabilities.SupportsNewTreePublication ||
                !capabilities.SupportsCurrentObjectPublication)
            {
                return Failure(
                    SaveGenerationPublicationStatus.BackendUnsupported,
                    EchoSaveDiagnosticCodes
                        .PublicationBackendUnsupported,
                    "The active Chronicle storage backend does not advertise all publication primitives required by Chronicle generation publication.",
                    validatedSlot,
                    default,
                    false);
            }

            SaveGenerationId generationId =
                generationFactory();

            if (!SaveGenerationId.TryParse(
                    generationId.Value,
                    out generationId))
            {
                return Failure(
                    SaveGenerationPublicationStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .PublicationInvalidRequest,
                    "The Chronicle generation factory returned an invalid generation ID.",
                    validatedSlot,
                    generationId,
                    false);
            }

            SaveStorageResult keyResult =
                SaveGenerationStorageKeys.TryCreate(
                    validatedSlot,
                    generationId,
                    out SaveGenerationStorageKeys keys);

            if (!keyResult.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.InvalidRequest,
                    keyResult.DiagnosticCode,
                    keyResult.Message,
                    validatedSlot,
                    generationId,
                    false);
            }

            SaveGenerationPublicationResult existingHeadResult =
                ReadExistingHead(
                    validatedSlot,
                    keys.Head,
                    out SaveHeadPointer previousHead);

            if (!existingHeadResult.Succeeded)
            {
                return existingHeadResult;
            }

            SavePayloadDocument payload =
                new SavePayloadDocument
                {
                    slotId =
                        validatedSlot.Value,
                    generationId =
                        generationId.Value,
                    entries =
                        payloadEntries
                };

            SaveSerializerResult serializedPayload =
                serializer.Serialize(
                    payload,
                    out string payloadJson);

            if (!serializedPayload.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.SerializationFailed,
                    EchoSaveDiagnosticCodes
                        .PublicationSerializationFailed,
                    serializedPayload.Message,
                    validatedSlot,
                    generationId,
                    false);
            }

            byte[] payloadBytes =
                Encoding.UTF8.GetBytes(
                    payloadJson);

            SaveIntegrityResult integrityResult =
                integrityProvider.Calculate(
                    payloadBytes,
                    out string payloadChecksum);

            if (!integrityResult.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.SerializationFailed,
                    integrityResult.DiagnosticCode,
                    integrityResult.Message,
                    validatedSlot,
                    generationId,
                    false);
            }

            string technicalTimestamp =
                clock.UtcNow
                    .ToUniversalTime()
                    .ToString(
                        "O",
                        CultureInfo.InvariantCulture);

            SaveManifest manifest =
                new SaveManifest
                {
                    slotId =
                        validatedSlot.Value,
                    generationId =
                        generationId.Value,
                    createdUtc =
                        technicalTimestamp,
                    updatedUtc =
                        technicalTimestamp,
                    saveKind =
                        participantBacked
                            ? "participant"
                            : "transport",
                    projectId =
                        projectId ?? string.Empty,
                    projectVersion =
                        projectVersion ?? string.Empty,
                    buildId =
                        buildId ?? string.Empty,
                    displayName =
                        displayName ?? string.Empty,
                    payloadByteLength =
                        payloadBytes.LongLength,
                    payloadChecksum =
                        payloadChecksum,
                    integrityAlgorithm =
                        integrityProvider.Id.Value,
                    payloadEntries =
                        inventoryEntries,
                    commitState =
                        SaveGenerationCommitState.Committed
                };

            SaveSerializerResult serializedManifest =
                serializer.Serialize(
                    manifest,
                    out string manifestJson);

            if (!serializedManifest.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.SerializationFailed,
                    EchoSaveDiagnosticCodes
                        .PublicationSerializationFailed,
                    serializedManifest.Message,
                    validatedSlot,
                    generationId,
                    false);
            }

            byte[] manifestBytes =
                Encoding.UTF8.GetBytes(
                    manifestJson);

            SaveStorageResult payloadWrite =
                storageBackend.WriteNew(
                    keys.CandidatePayload,
                    payloadBytes);

            if (!payloadWrite.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.CandidateWriteFailed,
                    EchoSaveDiagnosticCodes
                        .PublicationCandidateWriteFailed,
                    payloadWrite.ToString(),
                    validatedSlot,
                    generationId,
                    false);
            }

            SaveStorageResult manifestWrite =
                storageBackend.WriteNew(
                    keys.CandidateManifest,
                    manifestBytes);

            if (!manifestWrite.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.CandidateWriteFailed,
                    EchoSaveDiagnosticCodes
                        .PublicationCandidateWriteFailed,
                    manifestWrite.ToString(),
                    validatedSlot,
                    generationId,
                    false);
            }

            SaveGenerationPublicationResult candidateVerification =
                VerifyStoredGeneration(
                    validatedSlot,
                    generationId,
                    keys.CandidatePayload,
                    keys.CandidateManifest,
                    false);

            if (!candidateVerification.Succeeded)
            {
                return candidateVerification;
            }

            SaveStorageResult publishGeneration =
                publicationBackend.PublishNewTree(
                    keys.CandidateDirectory,
                    keys.GenerationDirectory);

            if (!publishGeneration.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.GenerationPublicationFailed,
                    EchoSaveDiagnosticCodes
                        .PublicationGenerationFailed,
                    publishGeneration.ToString(),
                    validatedSlot,
                    generationId,
                    false);
            }

            SaveGenerationPublicationResult finalVerification =
                VerifyStoredGeneration(
                    validatedSlot,
                    generationId,
                    keys.GenerationPayload,
                    keys.GenerationManifest,
                    true);

            if (!finalVerification.Succeeded)
            {
                return finalVerification;
            }

            if (previousHead != null &&
                previousHead.updateSequence ==
                    long.MaxValue)
            {
                return Failure(
                    SaveGenerationPublicationStatus.ExistingHeadInvalid,
                    EchoSaveDiagnosticCodes
                        .PublicationExistingHeadInvalid,
                    "The existing Chronicle head update sequence cannot advance safely.",
                    validatedSlot,
                    generationId,
                    true);
            }

            SaveHeadPointer newHead =
                new SaveHeadPointer
                {
                    slotId =
                        validatedSlot.Value,
                    currentGenerationId =
                        generationId.Value,
                    previousGenerationId =
                        previousHead == null
                            ? string.Empty
                            : previousHead
                                .currentGenerationId,
                    updateSequence =
                        previousHead == null
                            ? 1
                            : previousHead
                                .updateSequence + 1
                };

            SaveDocumentValidationResult headValidation =
                SaveCommitDocumentValidator
                    .ValidateHead(
                        newHead);

            if (!headValidation.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.HeadPublicationFailed,
                    EchoSaveDiagnosticCodes
                        .PublicationHeadFailed,
                    headValidation.Message,
                    validatedSlot,
                    generationId,
                    true);
            }

            SaveSerializerResult serializedHead =
                serializer.Serialize(
                    newHead,
                    out string headJson);

            if (!serializedHead.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.HeadPublicationFailed,
                    EchoSaveDiagnosticCodes
                        .PublicationHeadFailed,
                    serializedHead.Message,
                    validatedSlot,
                    generationId,
                    true);
            }

            byte[] headBytes =
                Encoding.UTF8.GetBytes(
                    headJson);

            SaveStorageResult headPublish =
                publicationBackend
                    .PublishCurrentObject(
                        keys.Head,
                        headBytes);

            if (!headPublish.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.HeadPublicationFailed,
                    EchoSaveDiagnosticCodes
                        .PublicationHeadFailed,
                    headPublish.ToString(),
                    validatedSlot,
                    generationId,
                    true);
            }

            return new SaveGenerationPublicationResult(
                SaveGenerationPublicationStatus.Succeeded,
                string.Empty,
                participantBacked
                    ? "The Chronicle participant generation was verified, published, and selected by a head-last commit."
                    : "The Chronicle transport generation was verified, published, and selected by a head-last commit.",
                validatedSlot,
                generationId,
                true,
                true);
        }

        private SaveGenerationPublicationResult
            ReadExistingHead(
                SaveSlotId slotId,
                SaveStorageKey headKey,
                out SaveHeadPointer previousHead)
        {
            previousHead = null;

            SaveStorageReadResult read =
                storageBackend.Read(
                    headKey);

            if (read.Result.Status ==
                SaveStorageStatus.NotFound)
            {
                return new SaveGenerationPublicationResult(
                    SaveGenerationPublicationStatus.Succeeded,
                    string.Empty,
                    "No previous Chronicle head exists.",
                    slotId,
                    default,
                    false,
                    false);
            }

            if (!read.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.ExistingHeadInvalid,
                    EchoSaveDiagnosticCodes
                        .PublicationExistingHeadInvalid,
                    read.Result.ToString(),
                    slotId,
                    default,
                    false);
            }

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveHeadPointer parsedHead);

            if (!deserialized.Succeeded)
            {
                return Failure(
                    SaveGenerationPublicationStatus.ExistingHeadInvalid,
                    EchoSaveDiagnosticCodes
                        .PublicationExistingHeadInvalid,
                    deserialized.Message,
                    slotId,
                    default,
                    false);
            }

            SaveDocumentValidationResult validation =
                SaveCommitDocumentValidator
                    .ValidateHead(
                        parsedHead);

            if (!validation.Succeeded ||
                !string.Equals(
                    parsedHead.slotId,
                    slotId.Value,
                    StringComparison.Ordinal))
            {
                return Failure(
                    SaveGenerationPublicationStatus.ExistingHeadInvalid,
                    EchoSaveDiagnosticCodes
                        .PublicationExistingHeadInvalid,
                    "The existing Chronicle head is not valid for the requested slot.",
                    slotId,
                    default,
                    false);
            }

            previousHead =
                parsedHead;

            return new SaveGenerationPublicationResult(
                SaveGenerationPublicationStatus.Succeeded,
                string.Empty,
                "The existing Chronicle head is structurally valid.",
                slotId,
                default,
                false,
                false);
        }

        private SaveGenerationPublicationResult
            VerifyStoredGeneration(
                SaveSlotId expectedSlot,
                SaveGenerationId expectedGeneration,
                SaveStorageKey payloadKey,
                SaveStorageKey manifestKey,
                bool generationPublished)
        {
            SaveStorageReadResult payloadRead =
                storageBackend.Read(
                    payloadKey);

            if (!payloadRead.Succeeded)
            {
                return VerificationFailure(
                    expectedSlot,
                    expectedGeneration,
                    generationPublished,
                    payloadRead.Result.ToString());
            }

            SaveStorageReadResult manifestRead =
                storageBackend.Read(
                    manifestKey);

            if (!manifestRead.Succeeded)
            {
                return VerificationFailure(
                    expectedSlot,
                    expectedGeneration,
                    generationPublished,
                    manifestRead.Result.ToString());
            }

            SaveSerializerResult payloadDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        payloadRead.Data),
                    out SavePayloadDocument payload);

            if (!payloadDeserialize.Succeeded)
            {
                return VerificationFailure(
                    expectedSlot,
                    expectedGeneration,
                    generationPublished,
                    payloadDeserialize.Message);
            }

            SaveSerializerResult manifestDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        manifestRead.Data),
                    out SaveManifest manifest);

            if (!manifestDeserialize.Succeeded)
            {
                return VerificationFailure(
                    expectedSlot,
                    expectedGeneration,
                    generationPublished,
                    manifestDeserialize.Message);
            }

            if (!string.Equals(
                    payload.slotId,
                    expectedSlot.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.slotId,
                    expectedSlot.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    payload.generationId,
                    expectedGeneration.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.generationId,
                    expectedGeneration.Value,
                    StringComparison.Ordinal) ||
                manifest.commitState !=
                    SaveGenerationCommitState.Committed)
            {
                return VerificationFailure(
                    expectedSlot,
                    expectedGeneration,
                    generationPublished,
                    "The stored Chronicle generation does not match its expected slot/generation identity or final commit state.");
            }

            SaveDocumentValidationResult agreement =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadRead.Data,
                        integrityProvider);

            if (!agreement.Succeeded)
            {
                return VerificationFailure(
                    expectedSlot,
                    expectedGeneration,
                    generationPublished,
                    agreement.Message);
            }

            SaveDocumentValidationResult participantEntries =
                SaveParticipantPublicationBatchValidator
                    .ValidateStoredEntries(
                        payload.entries ??
                            Array.Empty<SavePayloadEntry>(),
                        manifest.payloadEntries ??
                            Array.Empty<SavePayloadInventoryEntry>(),
                        integrityProvider);

            if (!participantEntries.Succeeded)
            {
                return VerificationFailure(
                    expectedSlot,
                    expectedGeneration,
                    generationPublished,
                    participantEntries.Message);
            }

            return new SaveGenerationPublicationResult(
                SaveGenerationPublicationStatus.Succeeded,
                string.Empty,
                generationPublished
                    ? "The published Chronicle generation was revalidated."
                    : "The candidate Chronicle generation was verified.",
                expectedSlot,
                expectedGeneration,
                generationPublished,
                false);
        }

        private static SaveGenerationPublicationResult
            VerificationFailure(
                SaveSlotId slotId,
                SaveGenerationId generationId,
                bool generationPublished,
                string message) =>
            Failure(
                SaveGenerationPublicationStatus.CandidateVerificationFailed,
                EchoSaveDiagnosticCodes
                    .PublicationCandidateVerificationFailed,
                message,
                slotId,
                generationId,
                generationPublished);

        private static SaveGenerationPublicationResult
            Failure(
                SaveGenerationPublicationStatus status,
                string diagnosticCode,
                string message,
                SaveSlotId slotId,
                SaveGenerationId generationId,
                bool generationPublished) =>
            new SaveGenerationPublicationResult(
                status,
                diagnosticCode,
                message,
                slotId,
                generationId,
                generationPublished,
                false);
    }
}
