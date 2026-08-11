
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// M4-09 non-destructive slot rename and full-state duplication engine.
    ///
    /// Public lifecycle/admission stays in EchoSaveService. This coordinator
    /// owns bounded source verification, immutable publication composition,
    /// capacity/identity rules, retention where applicable, and catalog truth.
    /// </summary>
    internal sealed class SaveSlotMutationCoordinator
    {
        internal const int MaximumDisplayNameLength = 256;

        private readonly SaveSlotCatalog catalog;
        private readonly ISaveSlotMutationSourceReader sourceReader;
        private readonly SaveGenerationPublicationCoordinator publication;
        private readonly ISaveGenerationRetentionExecutor retention;
        private readonly SaveRetentionPolicy retentionPolicy;
        private readonly int slotCapacity;
        private readonly int maxSlotIdAttempts;
        private readonly Func<SaveSlotId> slotIdFactory;

        internal SaveSlotMutationCoordinator(
            SaveSlotCatalog catalog,
            ISaveSlotMutationSourceReader sourceReader,
            SaveGenerationPublicationCoordinator publication,
            ISaveGenerationRetentionExecutor retention,
            SaveRetentionPolicy retentionPolicy,
            int slotCapacity,
            int maxSlotIdAttempts,
            Func<SaveSlotId> slotIdFactory)
        {
            this.catalog =
                catalog ??
                throw new ArgumentNullException(nameof(catalog));

            this.sourceReader =
                sourceReader ??
                throw new ArgumentNullException(nameof(sourceReader));

            this.publication =
                publication ??
                throw new ArgumentNullException(nameof(publication));

            this.retention = retention;
            this.retentionPolicy = retentionPolicy;

            if (slotCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slotCapacity));
            }

            if (maxSlotIdAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSlotIdAttempts));
            }

            this.slotCapacity = slotCapacity;
            this.maxSlotIdAttempts = maxSlotIdAttempts;
            this.slotIdFactory =
                slotIdFactory ??
                throw new ArgumentNullException(nameof(slotIdFactory));
        }

        internal SaveSlotRenameResult Rename(
            SaveSlotRenameRequest request)
        {
            if (!ValidateRenameRequest(
                    request,
                    out SaveSlotId slotId,
                    out string requestMessage))
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes.SlotRenameInvalidRequest,
                    requestMessage,
                    request == null
                        ? default
                        : request.SlotId);
            }

            SaveSlotCatalogRefreshResult preflight =
                catalog.Refresh();

            if (!preflight.Succeeded)
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.CatalogUnavailable,
                    EchoSaveDiagnosticCodes.SlotRenameCatalogUnavailable,
                    "Chronicle slot rename requires one trustworthy current catalog snapshot before durable mutation.",
                    slotId);
            }

            if (!preflight.Snapshot.TryGetEntry(
                    slotId,
                    out SaveSlotCatalogEntry entry) ||
                entry == null)
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.SlotNotFound,
                    EchoSaveDiagnosticCodes.SlotRenameNotFound,
                    "The requested Chronicle slot does not exist in the current canonical catalog.",
                    slotId);
            }

            if (!entry.IsSelectable)
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.SourceInvalid,
                    EchoSaveDiagnosticCodes.SlotRenameSourceInvalid,
                    "Chronicle slot rename requires one healthy fully selectable source slot.",
                    slotId,
                    entry.CurrentGenerationId);
            }

            if (string.Equals(
                    entry.DisplayName,
                    request.DisplayName,
                    StringComparison.Ordinal))
            {
                return new SaveSlotRenameResult(
                    SaveSlotRenameStatus.NoChange,
                    EchoSaveDiagnosticCodes.SlotRenameNoChange,
                    "The requested Chronicle slot already has that display name; no durable mutation was required.",
                    slotId,
                    entry.CurrentGenerationId,
                    default,
                    false,
                    false,
                    true,
                    entry,
                    SaveRetentionResult.NotRequired(
                        "No rename generation was published."));
            }

            SaveSlotMutationSourceReadResult source =
                sourceReader.Read(slotId);

            if (!source.Succeeded)
            {
                return SourceRenameFailure(
                    slotId,
                    entry.CurrentGenerationId,
                    source);
            }

            SaveSlotMutationSourceSnapshot snapshot =
                source.Snapshot;

            if (snapshot.GenerationId !=
                entry.CurrentGenerationId)
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.SourceStale,
                    EchoSaveDiagnosticCodes.SlotRenameSourceStale,
                    "The Chronicle catalog and fully verified source generation changed during rename preflight.",
                    slotId,
                    snapshot.GenerationId);
            }

            bool hadActive = catalog.HasActiveSlot;
            SaveSlotId activeBefore =
                hadActive
                    ? catalog.ActiveSlotId
                    : default;

            SaveSlotMutationSourceReadResult revalidated =
                sourceReader.Revalidate(snapshot);

            if (!revalidated.Succeeded)
            {
                return SourceRenameFailure(
                    slotId,
                    snapshot.GenerationId,
                    revalidated,
                    true);
            }

            SaveGenerationPublicationResult published =
                publication.PublishStoredTransportGeneration(
                    slotId,
                    snapshot.ProjectId,
                    snapshot.ProjectVersion,
                    snapshot.BuildId,
                    request.DisplayName,
                    snapshot.CopyPayloadEntries(),
                    snapshot.CopyInventoryEntries(),
                    snapshot.SaveKind,
                    snapshot.GenerationId);

            if (!published.Succeeded)
            {
                bool stale =
                    string.Equals(
                        published.DiagnosticCode,
                        EchoSaveDiagnosticCodes.CarryForwardSourceStale,
                        StringComparison.Ordinal);

                return SaveSlotRenameResult.Failure(
                    stale
                        ? SaveSlotRenameStatus.SourceStale
                        : SaveSlotRenameStatus.PublicationFailed,
                    stale
                        ? EchoSaveDiagnosticCodes.SlotRenameSourceStale
                        : (string.IsNullOrEmpty(published.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes.SlotRenamePublicationFailed
                            : published.DiagnosticCode),
                    published.Message,
                    slotId,
                    snapshot.GenerationId,
                    published.GenerationId,
                    published.GenerationPublished,
                    published.HeadPublished,
                    false);
            }

            SaveRetentionResult retentionResult =
                retention == null
                    ? SaveRetentionResult.NotRequired(
                        "Chronicle rename retention maintenance is not configured.")
                    : retention.Apply(
                        slotId,
                        retentionPolicy);

            SaveSlotCatalogRefreshResult reconciliation =
                catalog.Refresh();

            if (!reconciliation.Succeeded)
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.PublishedCatalogReconciliationFailed,
                    EchoSaveDiagnosticCodes.SlotRenameCatalogReconciliationFailed,
                    "The Chronicle rename head is durably published, but catalog reconciliation failed. The committed display-name change remains authoritative.",
                    slotId,
                    snapshot.GenerationId,
                    published.GenerationId,
                    true,
                    true,
                    false,
                    retentionResult);
            }

            if (!reconciliation.Snapshot.TryGetEntry(
                    slotId,
                    out SaveSlotCatalogEntry renamedEntry) ||
                renamedEntry == null ||
                !renamedEntry.IsSelectable ||
                renamedEntry.CurrentGenerationId !=
                    published.GenerationId ||
                !string.Equals(
                    renamedEntry.DisplayName,
                    request.DisplayName,
                    StringComparison.Ordinal))
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.PublishedCatalogReconciliationFailed,
                    EchoSaveDiagnosticCodes.SlotRenameCatalogReconciliationFailed,
                    "The Chronicle rename head is durably published, but the refreshed catalog does not expose the matching healthy renamed generation.",
                    slotId,
                    snapshot.GenerationId,
                    published.GenerationId,
                    true,
                    true,
                    false,
                    retentionResult);
            }

            if (hadActive &&
                (!catalog.HasActiveSlot ||
                 catalog.ActiveSlotId != activeBefore))
            {
                SaveActiveSlotSelectionResult restore =
                    catalog.SelectActiveSlot(activeBefore);

                if (!restore.Succeeded ||
                    !restore.HasActiveSlot ||
                    restore.ActiveSlotId != activeBefore)
                {
                    return SaveSlotRenameResult.Failure(
                        SaveSlotRenameStatus.PublishedCatalogReconciliationFailed,
                        EchoSaveDiagnosticCodes.SlotRenameCatalogReconciliationFailed,
                        "The Chronicle rename is durably committed, but the pre-existing active-slot selection could not be preserved after catalog reconciliation.",
                        slotId,
                        snapshot.GenerationId,
                        published.GenerationId,
                        true,
                        true,
                        false,
                        retentionResult);
                }
            }

            if (retentionResult.MaintenanceFailed)
            {
                return new SaveSlotRenameResult(
                    SaveSlotRenameStatus.PublishedRetentionMaintenanceFailed,
                    string.IsNullOrEmpty(retentionResult.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes.SlotRenameRetentionMaintenanceFailed
                        : retentionResult.DiagnosticCode,
                    "The Chronicle rename is durably committed and catalog-reconciled, but generation-retention maintenance did not fully complete. " +
                    retentionResult.Message,
                    slotId,
                    snapshot.GenerationId,
                    published.GenerationId,
                    true,
                    true,
                    true,
                    renamedEntry,
                    retentionResult);
            }

            return new SaveSlotRenameResult(
                SaveSlotRenameStatus.Succeeded,
                EchoSaveDiagnosticCodes.SlotRenameSucceeded,
                "The Chronicle slot display name was published as one new verified immutable generation while preserving technical slot identity and path.",
                slotId,
                snapshot.GenerationId,
                published.GenerationId,
                true,
                true,
                true,
                renamedEntry,
                retentionResult);
        }

        internal SaveSlotDuplicateResult Duplicate(
            SaveSlotDuplicateRequest request)
        {
            if (request == null ||
                !SaveSlotId.TryParse(
                    request.SourceSlotId.Value,
                    out SaveSlotId sourceSlot))
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes.SlotDuplicateInvalidRequest,
                    "Chronicle slot duplication requires one valid canonical source slot identity.",
                    request == null
                        ? default
                        : request.SourceSlotId);
            }

            SaveSlotCatalogRefreshResult preflight =
                catalog.Refresh();

            if (!preflight.Succeeded)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.CatalogUnavailable,
                    EchoSaveDiagnosticCodes.SlotDuplicateCatalogUnavailable,
                    "Chronicle slot duplication requires one trustworthy current catalog snapshot before durable mutation.",
                    sourceSlot);
            }

            SaveSlotCatalogSnapshot snapshotCatalog =
                preflight.Snapshot;

            if (!snapshotCatalog.TryGetEntry(
                    sourceSlot,
                    out SaveSlotCatalogEntry sourceEntry) ||
                sourceEntry == null)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.SlotNotFound,
                    EchoSaveDiagnosticCodes.SlotDuplicateNotFound,
                    "The requested Chronicle source slot does not exist in the current canonical catalog.",
                    sourceSlot);
            }

            if (!sourceEntry.IsSelectable)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.SourceInvalid,
                    EchoSaveDiagnosticCodes.SlotDuplicateSourceInvalid,
                    "Chronicle slot duplication requires one healthy fully selectable source slot.",
                    sourceSlot,
                    sourceEntry.CurrentGenerationId);
            }

            if (snapshotCatalog.Count >= slotCapacity)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.CapacityReached,
                    EchoSaveDiagnosticCodes.SlotDuplicateCapacityReached,
                    "Chronicle technical slot capacity has been reached. Healthy and degraded canonical slots both count against duplication capacity.",
                    sourceSlot,
                    sourceEntry.CurrentGenerationId);
            }

            SaveSlotMutationSourceReadResult source =
                sourceReader.Read(sourceSlot);

            if (!source.Succeeded)
            {
                return SourceDuplicateFailure(
                    sourceSlot,
                    sourceEntry.CurrentGenerationId,
                    source);
            }

            SaveSlotMutationSourceSnapshot sourceSnapshot =
                source.Snapshot;

            if (sourceSnapshot.GenerationId !=
                sourceEntry.CurrentGenerationId)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.SourceStale,
                    EchoSaveDiagnosticCodes.SlotDuplicateSourceStale,
                    "The Chronicle catalog and fully verified source generation changed during duplicate preflight.",
                    sourceSlot,
                    sourceSnapshot.GenerationId);
            }

            SaveSlotId duplicateSlot = default;
            bool freshIdentityFound = false;

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
                    return SaveSlotDuplicateResult.Failure(
                        SaveSlotDuplicateStatus.SlotIdGenerationFailed,
                        EchoSaveDiagnosticCodes.SlotDuplicateIdGenerationFailed,
                        "The Chronicle duplicate slot-ID factory returned an invalid canonical slot identity.",
                        sourceSlot,
                        sourceSnapshot.GenerationId);
                }

                if (snapshotCatalog.TryGetEntry(
                        candidate,
                        out _))
                {
                    continue;
                }

                duplicateSlot = candidate;
                freshIdentityFound = true;
                break;
            }

            if (!freshIdentityFound)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.SlotIdCollisionLimitExceeded,
                    EchoSaveDiagnosticCodes.SlotDuplicateCollisionLimitExceeded,
                    "Chronicle could not allocate one fresh duplicate slot identity within the bounded collision-attempt limit.",
                    sourceSlot,
                    sourceSnapshot.GenerationId);
            }

            bool hadActive = catalog.HasActiveSlot;
            SaveSlotId activeBefore =
                hadActive
                    ? catalog.ActiveSlotId
                    : default;

            SaveSlotMutationSourceReadResult revalidated =
                sourceReader.Revalidate(sourceSnapshot);

            if (!revalidated.Succeeded)
            {
                return SourceDuplicateFailure(
                    sourceSlot,
                    sourceSnapshot.GenerationId,
                    revalidated,
                    true,
                    duplicateSlot);
            }

            SaveGenerationPublicationResult published =
                publication.PublishInitialStoredTransportGeneration(
                    duplicateSlot,
                    sourceSnapshot.ProjectId,
                    sourceSnapshot.ProjectVersion,
                    sourceSnapshot.BuildId,
                    sourceSnapshot.DisplayName,
                    sourceSnapshot.CopyPayloadEntries(),
                    sourceSnapshot.CopyInventoryEntries(),
                    sourceSnapshot.SaveKind);

            if (!published.Succeeded)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.PublicationFailed,
                    string.IsNullOrEmpty(published.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes.SlotDuplicatePublicationFailed
                        : published.DiagnosticCode,
                    published.Message,
                    sourceSlot,
                    sourceSnapshot.GenerationId,
                    duplicateSlot,
                    published.GenerationId,
                    published.GenerationPublished,
                    published.HeadPublished,
                    false);
            }

            SaveSlotCatalogRefreshResult reconciliation =
                catalog.Refresh();

            if (!reconciliation.Succeeded)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.PublishedCatalogReconciliationFailed,
                    EchoSaveDiagnosticCodes.SlotDuplicateCatalogReconciliationFailed,
                    "The duplicate Chronicle slot is durably published, but catalog reconciliation failed. The committed duplicate remains authoritative.",
                    sourceSlot,
                    sourceSnapshot.GenerationId,
                    duplicateSlot,
                    published.GenerationId,
                    true,
                    true,
                    false);
            }

            if (!reconciliation.Snapshot.TryGetEntry(
                    duplicateSlot,
                    out SaveSlotCatalogEntry duplicateEntry) ||
                duplicateEntry == null ||
                !duplicateEntry.IsSelectable ||
                duplicateEntry.CurrentGenerationId !=
                    published.GenerationId ||
                !string.Equals(
                    duplicateEntry.DisplayName,
                    sourceSnapshot.DisplayName,
                    StringComparison.Ordinal))
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.PublishedCatalogReconciliationFailed,
                    EchoSaveDiagnosticCodes.SlotDuplicateCatalogReconciliationFailed,
                    "The duplicate Chronicle slot is durably published, but the refreshed catalog does not expose the matching healthy destination generation.",
                    sourceSlot,
                    sourceSnapshot.GenerationId,
                    duplicateSlot,
                    published.GenerationId,
                    true,
                    true,
                    false);
            }

            if (hadActive &&
                (!catalog.HasActiveSlot ||
                 catalog.ActiveSlotId != activeBefore))
            {
                SaveActiveSlotSelectionResult restore =
                    catalog.SelectActiveSlot(activeBefore);

                if (!restore.Succeeded ||
                    !restore.HasActiveSlot ||
                    restore.ActiveSlotId != activeBefore)
                {
                    return SaveSlotDuplicateResult.Failure(
                        SaveSlotDuplicateStatus.PublishedCatalogReconciliationFailed,
                        EchoSaveDiagnosticCodes.SlotDuplicateCatalogReconciliationFailed,
                        "The duplicate Chronicle slot is durably published, but the pre-existing active-slot selection could not be preserved after catalog reconciliation.",
                        sourceSlot,
                        sourceSnapshot.GenerationId,
                        duplicateSlot,
                        published.GenerationId,
                        true,
                        true,
                        false);
                }
            }

            return new SaveSlotDuplicateResult(
                SaveSlotDuplicateStatus.Succeeded,
                EchoSaveDiagnosticCodes.SlotDuplicateSucceeded,
                "The Chronicle duplicated one fully verified current slot state into a new package-generated slot/generation identity without mutating or selecting the source.",
                sourceSlot,
                sourceSnapshot.GenerationId,
                duplicateSlot,
                published.GenerationId,
                true,
                true,
                true,
                duplicateEntry);
        }

        private static bool ValidateRenameRequest(
            SaveSlotRenameRequest request,
            out SaveSlotId slotId,
            out string message)
        {
            slotId = default;
            message = string.Empty;

            if (request == null ||
                !SaveSlotId.TryParse(
                    request.SlotId.Value,
                    out slotId))
            {
                message =
                    "Chronicle slot rename requires one valid canonical slot identity.";

                return false;
            }

            if (request.DisplayName == null ||
                request.DisplayName.Length >
                    MaximumDisplayNameLength)
            {
                message =
                    "Chronicle slot rename display metadata must be non-null and no longer than 256 characters.";

                return false;
            }

            return true;
        }

        private static SaveSlotRenameResult SourceRenameFailure(
            SaveSlotId slotId,
            SaveGenerationId sourceGenerationId,
            SaveSlotMutationSourceReadResult source,
            bool duringRevalidation = false)
        {
            bool stale =
                source != null &&
                source.Status ==
                    SaveSlotMutationSourceStatus.SourceStale;

            return SaveSlotRenameResult.Failure(
                stale
                    ? SaveSlotRenameStatus.SourceStale
                    : SaveSlotRenameStatus.SourceInvalid,
                stale
                    ? EchoSaveDiagnosticCodes.SlotRenameSourceStale
                    : EchoSaveDiagnosticCodes.SlotRenameSourceInvalid,
                source == null
                    ? "Chronicle slot rename source verification returned no result."
                    : (duringRevalidation
                        ? "Chronicle slot rename source revalidation failed. "
                        : "Chronicle slot rename source verification failed. ") +
                      source.Message,
                slotId,
                sourceGenerationId);
        }

        private static SaveSlotDuplicateResult SourceDuplicateFailure(
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            SaveSlotMutationSourceReadResult source,
            bool duringRevalidation = false,
            SaveSlotId duplicateSlotId = default)
        {
            bool stale =
                source != null &&
                source.Status ==
                    SaveSlotMutationSourceStatus.SourceStale;

            return SaveSlotDuplicateResult.Failure(
                stale
                    ? SaveSlotDuplicateStatus.SourceStale
                    : SaveSlotDuplicateStatus.SourceInvalid,
                stale
                    ? EchoSaveDiagnosticCodes.SlotDuplicateSourceStale
                    : EchoSaveDiagnosticCodes.SlotDuplicateSourceInvalid,
                source == null
                    ? "Chronicle slot duplicate source verification returned no result."
                    : (duringRevalidation
                        ? "Chronicle slot duplicate source revalidation failed. "
                        : "Chronicle slot duplicate source verification failed. ") +
                      source.Message,
                sourceSlotId,
                sourceGenerationId,
                duplicateSlotId);
        }
    }
}
