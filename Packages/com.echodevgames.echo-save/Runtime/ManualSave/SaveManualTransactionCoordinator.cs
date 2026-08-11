using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Bounded M4-03 manual-save transaction composition seam.
    ///
    /// This coordinator composes already-proven Chronicle primitives. M4-06
    /// adds post-publication retention maintenance through one injected
    /// provider-neutral executor. It still does not own public SaveAsync,
    /// generic operation queues, recovery, slot deletion/trash, persistent
    /// catalog cache, scene lifetime, bridges, or DDOL.
    /// </summary>
    internal sealed class SaveManualTransactionCoordinator :
        ISaveManualTransactionExecutor
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
        private readonly ISaveGenerationRetentionExecutor
            retentionExecutor;
        private readonly SaveRetentionPolicy
            retentionPolicy;

        internal SaveManualTransactionCoordinator(
            SaveSlotCatalog catalog,
            SaveCurrentGenerationReader currentGenerationReader,
            SaveParticipantCaptureCoordinator captureCoordinator,
            SaveParticipantRegistry participantRegistry,
            SaveUnknownPayloadStore unknownPayloadStore,
            SaveUnknownPayloadCarryForwardCoordinator carryForwardCoordinator)
            : this(
                catalog,
                currentGenerationReader,
                captureCoordinator,
                participantRegistry,
                unknownPayloadStore,
                carryForwardCoordinator,
                null,
                SaveRetentionPolicy.Default)
        {
        }

        internal SaveManualTransactionCoordinator(
            SaveSlotCatalog catalog,
            SaveCurrentGenerationReader currentGenerationReader,
            SaveParticipantCaptureCoordinator captureCoordinator,
            SaveParticipantRegistry participantRegistry,
            SaveUnknownPayloadStore unknownPayloadStore,
            SaveUnknownPayloadCarryForwardCoordinator carryForwardCoordinator,
            ISaveGenerationRetentionExecutor retentionExecutor,
            SaveRetentionPolicy retentionPolicy)
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

            this.retentionExecutor =
                retentionExecutor;

            this.retentionPolicy =
                retentionPolicy;
        }

        internal SaveManualTransactionResult Save(
            SaveManualTransactionRequest request) =>
            Save(
                request,
                null);

        internal SaveManualTransactionResult Save(
            SaveManualTransactionRequest request,
            SaveManualTransactionControl control)
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

            if (control != null &&
                control.IsCancellationRequested)
            {
                return Canceled(
                    "Chronicle manual save was canceled before active-slot processing began.");
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

            if (control != null &&
                control.IsCancellationRequested)
            {
                return Canceled(
                    "Chronicle manual save was canceled before participant capture began.",
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

            if (control != null &&
                control.IsCancellationRequested)
            {
                return Canceled(
                    "Chronicle manual save was canceled after participant capture and before publication.",
                    targetSlot,
                    sourceRead.GenerationId);
            }

            Func<bool> tryBeginPublication =
                control == null
                    ? null
                    : control.TryBeginPublication;

            SaveCarryForwardPublicationResult publication =
                carryForwardCoordinator.PublishNextGeneration(
                    targetSlot,
                    request.ProjectId,
                    request.ProjectVersion,
                    request.BuildId,
                    targetEntry.DisplayName,
                    freshCapture,
                    unknownSnapshot,
                    tryBeginPublication);

            if (!publication.Succeeded)
            {
                return PublicationFailure(
                    publication);
            }

            SaveRetentionResult retention =
                retentionExecutor == null
                    ? SaveRetentionResult.NotRequired(
                        "Chronicle retention maintenance is not configured for this transaction.")
                    : retentionExecutor.Apply(
                        targetSlot,
                        retentionPolicy);

            SaveSlotCatalogRefreshResult reconciliation =
                catalog.Refresh();

            if (!reconciliation.Succeeded)
            {
                return ReconciliationFailure(
                    publication,
                    retention,
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
                    retention,
                    "The Chronicle manual save committed a new head, but the refreshed catalog did not expose the matching healthy generation with the existing active selection and display name.");
            }

            string successDiagnostic =
                retention.MaintenanceFailed
                    ? retention.DiagnosticCode
                    : string.Empty;

            string successMessage =
                "The Chronicle active slot was saved as one verified immutable participant-backed generation, head was published last, and the catalog was reconciled." +
                (retention.MaintenanceFailed
                    ? " The committed save remains authoritative, but retention maintenance did not fully complete. " +
                      retention.Message
                    : string.Empty);

            return new SaveManualTransactionResult(
                SaveManualTransactionStatus.Succeeded,
                successDiagnostic,
                successMessage,
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
                reconciledEntry,
                retention);
        }

        SaveManualTransactionResult
            ISaveManualTransactionExecutor.Save(
                SaveManualTransactionRequest request,
                SaveManualTransactionControl control) =>
            Save(
                request,
                control);

        private static SaveManualTransactionResult Canceled(
            string message,
            SaveSlotId slotId = default,
            SaveGenerationId sourceGenerationId = default) =>
            new SaveManualTransactionResult(
                SaveManualTransactionStatus.Canceled,
                EchoSaveDiagnosticCodes
                    .ManualSaveCanceled,
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

                case SaveCarryForwardPublicationStatus.Canceled:
                    status =
                        SaveManualTransactionStatus.Canceled;
                    fallbackDiagnostic =
                        EchoSaveDiagnosticCodes
                            .ManualSaveCanceled;
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
                SaveRetentionResult retention,
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
                null,
                retention);

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
