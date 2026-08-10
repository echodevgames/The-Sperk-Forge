using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Bounded M4-03 manual-save transaction composition seam.
    ///
    /// This coordinator composes already-proven Chronicle primitives. It does
    /// not own public SaveAsync, generic operation admission, Busy/cancellation,
    /// autosave, retention, recovery, rename/duplicate/delete, persistent
    /// catalog cache, scene lifetime, bridges, or DDOL.
    /// </summary>
    internal sealed class SaveManualTransactionCoordinator
    {
        internal const int MaximumMetadataTextLength =
            256;

        private readonly SaveSlotCatalog catalog;
        private readonly SaveCurrentGenerationReader
            currentGenerationReader;
        private readonly SaveParticipantCaptureCoordinator
            captureCoordinator;
        private readonly SaveParticipantRegistry
            participantRegistry;
        private readonly SaveUnknownPayloadStore
            unknownPayloadStore;
        private readonly SaveUnknownPayloadCarryForwardCoordinator
            carryForwardCoordinator;

        internal SaveManualTransactionCoordinator(
            SaveSlotCatalog catalog,
            SaveCurrentGenerationReader currentGenerationReader,
            SaveParticipantCaptureCoordinator captureCoordinator,
            SaveParticipantRegistry participantRegistry,
            SaveUnknownPayloadStore unknownPayloadStore,
            SaveUnknownPayloadCarryForwardCoordinator carryForwardCoordinator)
        {
            this.catalog =
                catalog ??
                throw new ArgumentNullException(
                    nameof(catalog));

            this.currentGenerationReader =
                currentGenerationReader ??
                throw new ArgumentNullException(
                    nameof(currentGenerationReader));

            this.captureCoordinator =
                captureCoordinator ??
                throw new ArgumentNullException(
                    nameof(captureCoordinator));

            this.participantRegistry =
                participantRegistry ??
                throw new ArgumentNullException(
                    nameof(participantRegistry));

            this.unknownPayloadStore =
                unknownPayloadStore ??
                throw new ArgumentNullException(
                    nameof(unknownPayloadStore));

            this.carryForwardCoordinator =
                carryForwardCoordinator ??
                throw new ArgumentNullException(
                    nameof(carryForwardCoordinator));
        }

        internal SaveManualTransactionResult Save(
            SaveManualTransactionRequest request)
        {
            if (!ValidateRequest(
                    request,
                    out string requestMessage))
            {
                return Failure(
                    SaveManualTransactionStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .ManualSaveInvalidRequest,
                    requestMessage);
            }

            if (!catalog.HasActiveSlot)
            {
                return Failure(
                    SaveManualTransactionStatus.NoActiveSlot,
                    EchoSaveDiagnosticCodes
                        .ManualSaveNoActiveSlot,
                    "Chronicle manual save requires one explicitly selected active slot.");
            }

            SaveSlotId targetSlot =
                catalog.ActiveSlotId;

            SaveSlotCatalogRefreshResult preflight =
                catalog.Refresh();

            if (!preflight.Succeeded)
            {
                return Failure(
                    SaveManualTransactionStatus.CatalogUnavailable,
                    EchoSaveDiagnosticCodes
                        .ManualSaveCatalogUnavailable,
                    "Chronicle manual save requires one trustworthy current catalog snapshot before participant capture.",
                    targetSlot);
            }

            if (!preflight.Snapshot.TryGetEntry(
                    targetSlot,
                    out SaveSlotCatalogEntry targetEntry) ||
                targetEntry == null ||
                !targetEntry.IsSelectable)
            {
                return Failure(
                    SaveManualTransactionStatus.ActiveSlotUnavailable,
                    EchoSaveDiagnosticCodes
                        .ManualSaveActiveSlotUnavailable,
                    "The selected Chronicle slot is no longer present as one healthy selectable catalog entry.",
                    targetSlot);
            }

            SaveGenerationId catalogSourceGeneration =
                targetEntry.CurrentGenerationId;

            SaveCurrentGenerationReadResult sourceRead =
                currentGenerationReader.ReadCurrent(
                    targetSlot);

            if (!sourceRead.Succeeded)
            {
                return Failure(
                    SaveManualTransactionStatus.SourceReadFailed,
                    string.IsNullOrEmpty(
                        sourceRead.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .ManualSaveSourceReadFailed
                        : sourceRead.DiagnosticCode,
                    sourceRead.Message,
                    targetSlot,
                    sourceRead.GenerationId);
            }

            if (sourceRead.SlotId !=
                    targetSlot ||
                sourceRead.GenerationId !=
                    catalogSourceGeneration)
            {
                return Failure(
                    SaveManualTransactionStatus.SourceChanged,
                    EchoSaveDiagnosticCodes
                        .ManualSaveSourceChanged,
                    "The Chronicle active-slot catalog generation changed while establishing the manual-save source. Refresh and retry.",
                    targetSlot,
                    sourceRead.GenerationId);
            }

            SaveUnknownPayloadSnapshot unknownSnapshot =
                unknownPayloadStore.GetSnapshot();

            if (!unknownSnapshot.HasSourceProvenance ||
                unknownSnapshot.SourceSlotId !=
                    targetSlot ||
                unknownSnapshot.SourceGenerationId !=
                    sourceRead.GenerationId)
            {
                return Failure(
                    SaveManualTransactionStatus.SourceReadFailed,
                    EchoSaveDiagnosticCodes
                        .ManualSaveSourceReadFailed,
                    "Chronicle manual save could not establish exact unknown-payload source provenance from the validated current generation.",
                    targetSlot,
                    sourceRead.GenerationId);
            }

            SaveParticipantCaptureBatchResult freshCapture =
                captureCoordinator.Capture(
                    participantRegistry);

            if (!freshCapture.Succeeded)
            {
                return new SaveManualTransactionResult(
                    SaveManualTransactionStatus.CaptureFailed,
                    string.IsNullOrEmpty(
                        freshCapture.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .ManualSaveCaptureFailed
                        : freshCapture.DiagnosticCode,
                    freshCapture.Message,
                    targetSlot,
                    sourceRead.GenerationId,
                    default,
                    freshCapture.FailingParticipantId,
                    default,
                    0,
                    unknownSnapshot.Count,
                    0L,
                    false,
                    false,
                    false,
                    null);
            }

            SaveCarryForwardPublicationResult publication =
                carryForwardCoordinator.PublishNextGeneration(
                    targetSlot,
                    request.ProjectId,
                    request.ProjectVersion,
                    request.BuildId,
                    targetEntry.DisplayName,
                    freshCapture,
                    unknownSnapshot);

            if (!publication.Succeeded)
            {
                return PublicationFailure(
                    publication);
            }

            SaveSlotCatalogRefreshResult reconciliation =
                catalog.Refresh();

            if (!reconciliation.Succeeded)
            {
                return ReconciliationFailure(
                    publication,
                    "The Chronicle manual save committed a new head, but catalog reconciliation failed. The committed generation remains authoritative.");
            }

            if (!reconciliation.Snapshot.TryGetEntry(
                    targetSlot,
                    out SaveSlotCatalogEntry reconciledEntry) ||
                reconciledEntry == null ||
                !reconciledEntry.IsSelectable ||
                reconciledEntry.CurrentGenerationId !=
                    publication.PublishedGenerationId ||
                !string.Equals(
                    reconciledEntry.DisplayName,
                    targetEntry.DisplayName,
                    StringComparison.Ordinal) ||
                !catalog.HasActiveSlot ||
                catalog.ActiveSlotId !=
                    targetSlot)
            {
                return ReconciliationFailure(
                    publication,
                    "The Chronicle manual save committed a new head, but the refreshed catalog did not expose the matching healthy generation with the existing active selection and display name.");
            }

            return new SaveManualTransactionResult(
                SaveManualTransactionStatus.Succeeded,
                string.Empty,
                "The Chronicle active slot was saved as one verified immutable participant-backed generation, head was published last, and the catalog was reconciled.",
                targetSlot,
                publication.SourceGenerationId,
                publication.PublishedGenerationId,
                default,
                default,
                publication.FreshParticipantCount,
                publication.PreservedUnknownCount,
                publication.TotalPayloadBytes,
                publication.GenerationPublished,
                publication.HeadPublished,
                true,
                reconciledEntry);
        }

        private static bool ValidateRequest(
            SaveManualTransactionRequest request,
            out string message)
        {
            message =
                string.Empty;

            if (request == null)
            {
                message =
                    "Chronicle manual save requires one request.";

                return false;
            }

            if (!Bounded(
                    request.ProjectId) ||
                !Bounded(
                    request.ProjectVersion) ||
                !Bounded(
                    request.BuildId))
            {
                message =
                    "Chronicle manual-save metadata exceeds the bounded 256-character field limit.";

                return false;
            }

            return true;
        }

        private static bool Bounded(
            string value) =>
            value != null &&
            value.Length <=
                MaximumMetadataTextLength;

        private static SaveManualTransactionResult
            PublicationFailure(
                SaveCarryForwardPublicationResult publication)
        {
            SaveManualTransactionStatus status;
            string fallbackDiagnostic;

            switch (publication.Status)
            {
                case SaveCarryForwardPublicationStatus.StaleSource:
                    status =
                        SaveManualTransactionStatus.StaleSource;
                    fallbackDiagnostic =
                        EchoSaveDiagnosticCodes
                            .ManualSaveStaleSource;
                    break;

                case SaveCarryForwardPublicationStatus.PublicationFailed:
                    status =
                        SaveManualTransactionStatus.PublicationFailed;
                    fallbackDiagnostic =
                        EchoSaveDiagnosticCodes
                            .ManualSavePublicationFailed;
                    break;

                default:
                    status =
                        SaveManualTransactionStatus.CarryForwardFailed;
                    fallbackDiagnostic =
                        EchoSaveDiagnosticCodes
                            .ManualSaveCarryForwardFailed;
                    break;
            }

            return new SaveManualTransactionResult(
                status,
                string.IsNullOrEmpty(
                    publication.DiagnosticCode)
                    ? fallbackDiagnostic
                    : publication.DiagnosticCode,
                publication.Message,
                publication.SlotId,
                publication.SourceGenerationId,
                publication.PublishedGenerationId,
                publication.FailingPersistedId,
                publication.CurrentOwnerId,
                publication.FreshParticipantCount,
                publication.PreservedUnknownCount,
                publication.TotalPayloadBytes,
                publication.GenerationPublished,
                publication.HeadPublished,
                false,
                null);
        }

        private static SaveManualTransactionResult
            ReconciliationFailure(
                SaveCarryForwardPublicationResult publication,
                string message) =>
            new SaveManualTransactionResult(
                SaveManualTransactionStatus
                    .PublishedCatalogReconciliationFailed,
                EchoSaveDiagnosticCodes
                    .ManualSaveCatalogReconciliationFailed,
                message,
                publication.SlotId,
                publication.SourceGenerationId,
                publication.PublishedGenerationId,
                default,
                default,
                publication.FreshParticipantCount,
                publication.PreservedUnknownCount,
                publication.TotalPayloadBytes,
                publication.GenerationPublished,
                publication.HeadPublished,
                false,
                null);

        private static SaveManualTransactionResult Failure(
            SaveManualTransactionStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId = default,
            SaveGenerationId sourceGenerationId = default) =>
            new SaveManualTransactionResult(
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
                false,
                false,
                null);
    }
}
