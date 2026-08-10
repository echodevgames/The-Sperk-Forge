
using System;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Bounded M3-05 technical publication seam.
    ///
    /// Proves source freshness, merges fresh known captures with opaque unknown
    /// entries, and forwards the complete batch into the established
    /// immutable-generation/head-last transaction.
    /// </summary>
    internal sealed class SaveUnknownPayloadCarryForwardCoordinator
    {
        private readonly ISaveStorageBackend storageBackend;
        private readonly ISaveSerializer serializer;
        private readonly SaveParticipantRegistry participantRegistry;
        private readonly SaveUnknownPayloadCarryForwardMerger merger;
        private readonly SaveGenerationPublicationCoordinator
            publicationCoordinator;

        internal SaveUnknownPayloadCarryForwardCoordinator(
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            IIntegrityProvider integrityProvider,
            SaveParticipantRegistry participantRegistry,
            SaveGenerationPublicationCoordinator publicationCoordinator)
        {
            this.storageBackend =
                storageBackend;

            this.serializer =
                serializer;

            this.participantRegistry =
                participantRegistry;

            this.publicationCoordinator =
                publicationCoordinator;

            merger =
                new SaveUnknownPayloadCarryForwardMerger(
                    integrityProvider,
                    participantRegistry);
        }

        internal SaveCarryForwardPublicationResult
            PublishNextGeneration(
                SaveSlotId slotId,
                string projectId,
                string projectVersion,
                string buildId,
                string displayName,
                SaveParticipantCaptureBatchResult freshCapture,
                SaveUnknownPayloadSnapshot unknownSnapshot)
        {
            if (storageBackend == null ||
                serializer == null ||
                participantRegistry == null ||
                publicationCoordinator == null ||
                freshCapture == null ||
                unknownSnapshot == null ||
                !SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .CarryForwardInvalidRequest,
                    "Chronicle carry-forward publication requires valid providers, registry, publication coordinator, target slot, fresh capture, and unknown snapshot.",
                    slotId,
                    default);
            }

            if (!unknownSnapshot.HasSourceProvenance ||
                !SaveSlotId.TryParse(
                    unknownSnapshot.SourceSlotId.Value,
                    out SaveSlotId sourceSlot) ||
                !SaveGenerationId.TryParse(
                    unknownSnapshot.SourceGenerationId.Value,
                    out SaveGenerationId sourceGeneration))
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.MissingProvenance,
                    EchoSaveDiagnosticCodes
                        .CarryForwardProvenanceMissing,
                    "Chronicle carry-forward snapshot does not contain valid source slot/generation provenance.",
                    validatedSlot,
                    default);
            }

            if (validatedSlot !=
                sourceSlot)
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.SlotMismatch,
                    EchoSaveDiagnosticCodes
                        .CarryForwardSlotMismatch,
                    "Chronicle carry-forward target slot does not match the preserved unknown snapshot source slot.",
                    validatedSlot,
                    sourceGeneration);
            }

            SaveCarryForwardPublicationResult freshness =
                ValidateSourceFreshness(
                    validatedSlot,
                    sourceGeneration);

            if (!freshness.Succeeded)
            {
                return freshness;
            }

            SaveUnknownPayloadMergeResult merge =
                merger.Merge(
                    freshCapture,
                    unknownSnapshot);

            if (!merge.Succeeded)
            {
                SaveCarryForwardPublicationStatus mappedStatus =
                    merge.Status ==
                    SaveUnknownPayloadMergeStatus.OwnershipCollision
                        ? SaveCarryForwardPublicationStatus
                            .OwnershipCollision
                        : SaveCarryForwardPublicationStatus
                            .MergeInvalid;

                return new SaveCarryForwardPublicationResult(
                    mappedStatus,
                    merge.DiagnosticCode,
                    merge.Message,
                    validatedSlot,
                    sourceGeneration,
                    default,
                    merge.FailingPersistedId,
                    merge.CurrentOwnerId,
                    0,
                    0,
                    0L,
                    false,
                    false);
            }

            SaveMergedParticipantTransportBatch batch =
                merge.Batch;

            SaveGenerationPublicationResult publication =
                publicationCoordinator
                    .PublishMergedParticipantTransportGeneration(
                        validatedSlot,
                        projectId,
                        projectVersion,
                        buildId,
                        displayName,
                        batch,
                        sourceGeneration);

            if (!publication.Succeeded)
            {
                SaveCarryForwardPublicationStatus mappedStatus =
                    string.Equals(
                        publication.DiagnosticCode,
                        EchoSaveDiagnosticCodes
                            .CarryForwardSourceStale,
                        StringComparison.Ordinal)
                        ? SaveCarryForwardPublicationStatus
                            .StaleSource
                        : SaveCarryForwardPublicationStatus
                            .PublicationFailed;

                return new SaveCarryForwardPublicationResult(
                    mappedStatus,
                    publication.DiagnosticCode,
                    publication.Message,
                    validatedSlot,
                    sourceGeneration,
                    publication.GenerationId,
                    default,
                    default,
                    batch.FreshParticipantCount,
                    batch.PreservedUnknownCount,
                    batch.TotalPayloadBytes,
                    publication.GenerationPublished,
                    publication.HeadPublished);
            }

            return new SaveCarryForwardPublicationResult(
                SaveCarryForwardPublicationStatus.Succeeded,
                string.Empty,
                "Chronicle fresh known captures and opaque unknown payloads were published as one verified immutable generation with head last.",
                validatedSlot,
                sourceGeneration,
                publication.GenerationId,
                default,
                default,
                batch.FreshParticipantCount,
                batch.PreservedUnknownCount,
                batch.TotalPayloadBytes,
                publication.GenerationPublished,
                publication.HeadPublished);
        }

        private SaveCarryForwardPublicationResult
            ValidateSourceFreshness(
                SaveSlotId slotId,
                SaveGenerationId sourceGeneration)
        {
            SaveStorageResult keyResult =
                SaveStorageKey.TryCreate(
                    "slots/" +
                    slotId.Value +
                    "/head.json",
                    out SaveStorageKey headKey);

            if (!keyResult.Succeeded)
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.InvalidRequest,
                    keyResult.DiagnosticCode,
                    keyResult.Message,
                    slotId,
                    sourceGeneration);
            }

            SaveStorageReadResult headRead =
                storageBackend.Read(
                    headKey);

            if (headRead.Result.Status ==
                SaveStorageStatus.NotFound)
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.SourceUnavailable,
                    EchoSaveDiagnosticCodes
                        .CarryForwardSourceUnavailable,
                    "Chronicle carry-forward source slot does not have a current head.",
                    slotId,
                    sourceGeneration);
            }

            if (!headRead.Succeeded)
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.SourceUnavailable,
                    EchoSaveDiagnosticCodes
                        .CarryForwardSourceUnavailable,
                    headRead.Result.ToString(),
                    slotId,
                    sourceGeneration);
            }

            SaveSerializerResult deserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        headRead.Data),
                    out SaveHeadPointer head);

            if (!deserialize.Succeeded)
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.SourceInvalid,
                    EchoSaveDiagnosticCodes
                        .CarryForwardSourceInvalid,
                    deserialize.Message,
                    slotId,
                    sourceGeneration);
            }

            SaveDocumentValidationResult validation =
                SaveCommitDocumentValidator
                    .ValidateHead(
                        head);

            if (!validation.Succeeded ||
                !string.Equals(
                    head.slotId,
                    slotId.Value,
                    StringComparison.Ordinal) ||
                !SaveGenerationId.TryParse(
                    head.currentGenerationId,
                    out SaveGenerationId currentGeneration))
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.SourceInvalid,
                    EchoSaveDiagnosticCodes
                        .CarryForwardSourceInvalid,
                    "Chronicle carry-forward current head is invalid for the preserved source slot.",
                    slotId,
                    sourceGeneration);
            }

            if (currentGeneration !=
                sourceGeneration)
            {
                return Failure(
                    SaveCarryForwardPublicationStatus.StaleSource,
                    EchoSaveDiagnosticCodes
                        .CarryForwardSourceStale,
                    "Chronicle preserved unknown payloads came from a generation that is no longer current. Refresh preservation before retrying.",
                    slotId,
                    sourceGeneration);
            }

            return new SaveCarryForwardPublicationResult(
                SaveCarryForwardPublicationStatus.Succeeded,
                string.Empty,
                "Chronicle preserved unknown-payload source generation is still current.",
                slotId,
                sourceGeneration,
                default,
                default,
                default,
                0,
                0,
                0L,
                false,
                false);
        }

        private static SaveCarryForwardPublicationResult
            Failure(
                SaveCarryForwardPublicationStatus status,
                string diagnosticCode,
                string message,
                SaveSlotId slotId,
                SaveGenerationId sourceGenerationId) =>
            new SaveCarryForwardPublicationResult(
                status,
                diagnosticCode,
                message,
                slotId,
                sourceGenerationId,
                default,
                default,
                default,
                0,
                0,
                0L,
                false,
                false);
    }
}
