
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Bounded technical slot-creation foundation.
    ///
    /// This coordinator intentionally does not own production async operation
    /// admission, participant capture/apply, autosave, retention, recovery,
    /// rename, duplicate, delete, persistent catalog cache, or scene lifetime.
    /// </summary>
    internal sealed class SaveTechnicalSlotCreationCoordinator
    {
        internal const int MaximumMetadataTextLength =
            256;

        private readonly SaveSlotCatalog catalog;
        private readonly SaveGenerationPublicationCoordinator
            publicationCoordinator;
        private readonly int slotCapacity;
        private readonly int maxSlotIdAttempts;
        private readonly Func<SaveSlotId> slotIdFactory;

        internal SaveTechnicalSlotCreationCoordinator(
            SaveSlotCatalog catalog,
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            IIntegrityProvider integrityProvider,
            int slotCapacity,
            int maxSlotIdAttempts)
            : this(
                catalog,
                new SaveGenerationPublicationCoordinator(
                    storageBackend,
                    serializer,
                    integrityProvider),
                slotCapacity,
                maxSlotIdAttempts,
                SaveSlotId.NewId)
        {
        }

        internal SaveTechnicalSlotCreationCoordinator(
            SaveSlotCatalog catalog,
            SaveGenerationPublicationCoordinator
                publicationCoordinator,
            int slotCapacity,
            int maxSlotIdAttempts,
            Func<SaveSlotId> slotIdFactory)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(
                    nameof(catalog));
            }

            if (publicationCoordinator == null)
            {
                throw new ArgumentNullException(
                    nameof(publicationCoordinator));
            }

            if (slotCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(slotCapacity));
            }

            if (maxSlotIdAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxSlotIdAttempts));
            }

            if (slotIdFactory == null)
            {
                throw new ArgumentNullException(
                    nameof(slotIdFactory));
            }

            this.catalog =
                catalog;

            this.publicationCoordinator =
                publicationCoordinator;

            this.slotCapacity =
                slotCapacity;

            this.maxSlotIdAttempts =
                maxSlotIdAttempts;

            this.slotIdFactory =
                slotIdFactory;
        }

        internal SaveTechnicalSlotCreateResult Create(
            SaveTechnicalSlotCreateRequest request)
        {
            if (!ValidateRequest(
                    request,
                    out string requestMessage))
            {
                return Failure(
                    SaveTechnicalSlotCreateStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SlotCreateInvalidRequest,
                    requestMessage,
                    default,
                    default,
                    false,
                    false);
            }

            SaveSlotCatalogRefreshResult preflight =
                catalog.Refresh();

            if (!preflight.Succeeded)
            {
                return Failure(
                    SaveTechnicalSlotCreateStatus.CatalogUnavailable,
                    EchoSaveDiagnosticCodes
                        .SlotCreateCatalogUnavailable,
                    "Chronicle slot creation requires one trustworthy current catalog snapshot before durable mutation.",
                    default,
                    default,
                    false,
                    false);
            }

            SaveSlotCatalogSnapshot snapshot =
                preflight.Snapshot;

            if (snapshot.Count >=
                slotCapacity)
            {
                return Failure(
                    SaveTechnicalSlotCreateStatus.CapacityReached,
                    EchoSaveDiagnosticCodes
                        .SlotCreateCapacityReached,
                    "Chronicle technical slot capacity has been reached.",
                    default,
                    default,
                    false,
                    false);
            }

            SaveSlotId slotId = default;
            bool foundFreshId = false;

            for (int attempt = 0;
                 attempt < maxSlotIdAttempts;
                 attempt++)
            {
                SaveSlotId candidate =
                    slotIdFactory();

                if (!SaveSlotId.TryParse(
                        candidate.Value,
                        out candidate))
                {
                    return Failure(
                        SaveTechnicalSlotCreateStatus
                            .SlotIdGenerationFailed,
                        EchoSaveDiagnosticCodes
                            .SlotCreateIdGenerationFailed,
                        "The Chronicle technical slot ID factory returned an invalid canonical slot ID.",
                        default,
                        default,
                        false,
                        false);
                }

                if (snapshot.TryGetEntry(
                        candidate,
                        out _))
                {
                    continue;
                }

                slotId =
                    candidate;

                foundFreshId =
                    true;

                break;
            }

            if (!foundFreshId)
            {
                return Failure(
                    SaveTechnicalSlotCreateStatus
                        .SlotIdCollisionLimitExceeded,
                    EchoSaveDiagnosticCodes
                        .SlotCreateCollisionLimitExceeded,
                    "Chronicle could not allocate one fresh technical slot ID within the configured collision-attempt bound.",
                    default,
                    default,
                    false,
                    false);
            }

            SaveGenerationPublicationResult publication =
                publicationCoordinator
                    .PublishInitialEmptyTransportGeneration(
                        slotId,
                        request.ProjectId,
                        request.ProjectVersion,
                        request.BuildId,
                        request.DisplayName);

            if (!publication.Succeeded)
            {
                return Failure(
                    SaveTechnicalSlotCreateStatus.PublicationFailed,
                    string.IsNullOrEmpty(
                        publication.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .SlotCreatePublicationFailed
                        : publication.DiagnosticCode,
                    publication.Message,
                    slotId,
                    publication.GenerationId,
                    publication.HeadPublished,
                    false);
            }

            SaveSlotCatalogRefreshResult reconciliation =
                catalog.Refresh();

            if (!reconciliation.Succeeded)
            {
                return Failure(
                    SaveTechnicalSlotCreateStatus
                        .PublishedCatalogReconciliationFailed,
                    EchoSaveDiagnosticCodes
                        .SlotCreateCatalogReconciliationFailed,
                    "The Chronicle slot was durably published, but catalog reconciliation failed. The committed slot remains authoritative and must not be fictionalized as rolled back.",
                    slotId,
                    publication.GenerationId,
                    true,
                    false);
            }

            if (!reconciliation.Snapshot.TryGetEntry(
                    slotId,
                    out SaveSlotCatalogEntry createdEntry) ||
                createdEntry == null ||
                !createdEntry.IsSelectable ||
                createdEntry.CurrentGenerationId !=
                    publication.GenerationId)
            {
                return Failure(
                    SaveTechnicalSlotCreateStatus
                        .PublishedCatalogReconciliationFailed,
                    EchoSaveDiagnosticCodes
                        .SlotCreateCatalogReconciliationFailed,
                    "The Chronicle slot was durably published, but the refreshed catalog did not expose the matching healthy current generation.",
                    slotId,
                    publication.GenerationId,
                    true,
                    false);
            }

            return new SaveTechnicalSlotCreateResult(
                SaveTechnicalSlotCreateStatus.Succeeded,
                string.Empty,
                "The Chronicle technical slot was created as one verified immutable empty generation and reconciled into the current catalog.",
                slotId,
                publication.GenerationId,
                true,
                true,
                createdEntry);
        }

        private static bool ValidateRequest(
            SaveTechnicalSlotCreateRequest request,
            out string message)
        {
            message =
                string.Empty;

            if (request == null)
            {
                message =
                    "Chronicle technical slot creation requires one request.";

                return false;
            }

            if (!Bounded(
                    request.DisplayName) ||
                !Bounded(
                    request.ProjectId) ||
                !Bounded(
                    request.ProjectVersion) ||
                !Bounded(
                    request.BuildId))
            {
                message =
                    "Chronicle technical slot creation metadata exceeds the bounded 256-character field limit.";

                return false;
            }

            return true;
        }

        private static bool Bounded(
            string value) =>
            value != null &&
            value.Length <=
                MaximumMetadataTextLength;

        private static SaveTechnicalSlotCreateResult Failure(
            SaveTechnicalSlotCreateStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId generationId,
            bool slotPublished,
            bool catalogReconciled) =>
            new SaveTechnicalSlotCreateResult(
                status,
                diagnosticCode,
                message,
                slotId,
                generationId,
                slotPublished,
                catalogReconciled,
                null);
    }
}
