
using System;
using System.Collections.Generic;
using System.Globalization;

namespace EchoDevGames.EchoSave
{
    internal sealed class SaveSlotDeletionCoordinator
    {
        private readonly SaveSlotCatalog catalog;
        private readonly SaveSlotCatalogScanner scanner;
        private readonly ISaveStorageBackend storage;
        private readonly ISaveDeletionSourceReader sourceReader;
        private readonly SaveTrashRetentionCoordinator trashRetention;
        private readonly string sessionId;
        private readonly Func<DateTimeOffset> clock;
        private readonly TimeSpan planLifetime;
        private readonly int maxTrashRecords;
        private readonly int maxTrashIdAttempts;
        private readonly Func<string> trashTokenFactory;
        private readonly HashSet<string> consumedPlanIds =
            new HashSet<string>(
                StringComparer.Ordinal);

        internal SaveSlotDeletionCoordinator(
            SaveSlotCatalog catalog,
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            IIntegrityProvider integrity,
            string sessionId,
            Func<DateTimeOffset> clock,
            TimeSpan planLifetime,
            int catalogScanLimit,
            int trashDiscoveryLimit,
            int maxTrashRecords,
            int maxTrashIdAttempts,
            Func<string> trashTokenFactory = null,
            ISaveDeletionSourceReader sourceReader = null)
        {
            this.catalog =
                catalog ??
                throw new ArgumentNullException(nameof(catalog));

            this.storage =
                storage ??
                throw new ArgumentNullException(nameof(storage));

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (integrity == null)
            {
                throw new ArgumentNullException(nameof(integrity));
            }

            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException(
                    "Chronicle deletion session identity is required.",
                    nameof(sessionId));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            if (planLifetime <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(planLifetime));
            }

            if (catalogScanLimit <= 0 ||
                trashDiscoveryLimit <= 0 ||
                maxTrashRecords <= 0 ||
                maxTrashIdAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(catalogScanLimit));
            }

            scanner =
                new SaveSlotCatalogScanner(
                    storage,
                    serializer,
                    catalogScanLimit);

            this.sourceReader =
                sourceReader ??
                new SaveDeletionSourceReader(
                    storage,
                    serializer,
                    integrity);

            trashRetention =
                new SaveTrashRetentionCoordinator(
                    storage,
                    trashDiscoveryLimit);

            this.sessionId = sessionId;
            this.clock = clock;
            this.planLifetime = planLifetime;
            this.maxTrashRecords = maxTrashRecords;
            this.maxTrashIdAttempts = maxTrashIdAttempts;
            this.trashTokenFactory =
                trashTokenFactory ??
                (() => Guid.NewGuid().ToString("N"));
        }

        internal SaveDeletionPlan Prepare(
            SaveSlotId slotId)
        {
            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return SaveDeletionPlan.Failure(
                    SaveDeletionPlanStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes.DeletePlanInvalidRequest,
                    "Chronicle prepare-delete requires one valid technical slot identity.",
                    slotId);
            }

            SaveSlotCatalogRefreshResult scan =
                scanner.Scan();

            if (!scan.Succeeded)
            {
                return SaveDeletionPlan.Failure(
                    SaveDeletionPlanStatus.CatalogUnavailable,
                    EchoSaveDiagnosticCodes.DeletePlanCatalogUnavailable,
                    "Chronicle prepare-delete requires one trustworthy live-slot snapshot. " +
                    scan.Message,
                    validatedSlot);
            }

            if (!scan.Snapshot.TryGetEntry(
                    validatedSlot,
                    out SaveSlotCatalogEntry entry) ||
                entry == null)
            {
                return SaveDeletionPlan.Failure(
                    SaveDeletionPlanStatus.SlotNotFound,
                    EchoSaveDiagnosticCodes.DeletePlanSlotNotFound,
                    "The requested Chronicle slot does not exist in the current live catalog.",
                    validatedSlot);
            }

            SaveDeletionSourceReadResult source =
                sourceReader.Read(
                    validatedSlot);

            if (!source.Succeeded ||
                source.Snapshot.GenerationId !=
                    entry.CurrentGenerationId)
            {
                return SaveDeletionPlan.Failure(
                    SaveDeletionPlanStatus.SourceInvalid,
                    EchoSaveDiagnosticCodes.DeletePlanSourceInvalid,
                    "Chronicle prepare-delete could not bind one trustworthy current source. " +
                    (source == null
                        ? string.Empty
                        : source.Message),
                    validatedSlot);
            }

            DateTimeOffset issued =
                clock();

            DateTimeOffset expires =
                issued + planLifetime;

            bool active =
                catalog.HasActiveSlot &&
                catalog.ActiveSlotId == validatedSlot;

            return new SaveDeletionPlan(
                SaveDeletionPlanStatus.Ready,
                EchoSaveDiagnosticCodes.DeletePlanPrepared,
                "Chronicle prepared one immutable bounded deletion plan. No durable mutation occurred.",
                Guid.NewGuid().ToString("N"),
                sessionId,
                validatedSlot,
                source.Snapshot.GenerationId,
                entry.DisplayName,
                active,
                issued,
                expires,
                source.Snapshot);
        }

        internal SaveSlotDeleteResult Confirm(
            SaveDeletionPlan plan)
        {
            if (plan == null ||
                !plan.Succeeded ||
                string.IsNullOrEmpty(plan.PlanId) ||
                plan.SourceSnapshot == null)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.InvalidPlan,
                    EchoSaveDiagnosticCodes.DeleteConfirmInvalidPlan,
                    "Chronicle confirm-delete requires one successful immutable deletion plan.",
                    plan == null
                        ? default
                        : plan.SlotId);
            }

            if (!string.Equals(
                    plan.SessionId,
                    sessionId,
                    StringComparison.Ordinal))
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.ForeignSession,
                    EchoSaveDiagnosticCodes.DeleteConfirmForeignSession,
                    "The deletion plan belongs to a different Chronicle runtime session.",
                    plan.SlotId,
                    plan.CurrentGenerationId);
            }

            if (consumedPlanIds.Contains(
                    plan.PlanId))
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.Consumed,
                    EchoSaveDiagnosticCodes.DeleteConfirmConsumed,
                    "The deletion plan has already crossed its one-use confirmation boundary.",
                    plan.SlotId,
                    plan.CurrentGenerationId);
            }

            DateTimeOffset now =
                clock();

            if (now > plan.ExpiresUtc)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.Expired,
                    EchoSaveDiagnosticCodes.DeleteConfirmExpired,
                    "The deletion plan expired before destructive confirmation.",
                    plan.SlotId,
                    plan.CurrentGenerationId);
            }

            SaveSlotCatalogRefreshResult scan =
                scanner.Scan();

            if (!scan.Succeeded)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.SourceInvalid,
                    EchoSaveDiagnosticCodes.DeleteConfirmSourceInvalid,
                    "Chronicle confirm-delete could not establish one trustworthy current live-slot snapshot. " +
                    scan.Message,
                    plan.SlotId,
                    plan.CurrentGenerationId);
            }

            if (!scan.Snapshot.TryGetEntry(
                    plan.SlotId,
                    out SaveSlotCatalogEntry currentEntry) ||
                currentEntry == null ||
                currentEntry.CurrentGenerationId !=
                    plan.CurrentGenerationId)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.SourceStale,
                    EchoSaveDiagnosticCodes.DeleteConfirmSourceStale,
                    "The planned Chronicle slot no longer matches the live catalog state.",
                    plan.SlotId,
                    plan.CurrentGenerationId);
            }

            SaveDeletionSourceReadResult revalidated =
                sourceReader.Revalidate(
                    plan.SourceSnapshot);

            if (!revalidated.Succeeded)
            {
                bool stale =
                    revalidated.Status ==
                    SaveDeletionSourceStatus.SourceStale;

                return SaveSlotDeleteResult.Failure(
                    stale
                        ? SaveSlotDeleteStatus.SourceStale
                        : SaveSlotDeleteStatus.SourceInvalid,
                    stale
                        ? EchoSaveDiagnosticCodes.DeleteConfirmSourceStale
                        : EchoSaveDiagnosticCodes.DeleteConfirmSourceInvalid,
                    "Chronicle confirm-delete source revalidation failed. " +
                    revalidated.Message,
                    plan.SlotId,
                    plan.CurrentGenerationId);
            }

            if (!(storage is
                ISaveStoragePublicationBackend publication))
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.BackendUnsupported,
                    EchoSaveDiagnosticCodes.DeleteConfirmBackendUnsupported,
                    "The active Chronicle storage provider cannot move one complete live slot tree into recoverable trash.",
                    plan.SlotId,
                    plan.CurrentGenerationId);
            }

            SaveStorageResult sourceKeyResult =
                SaveStorageKey.TryCreate(
                    "slots/" + plan.SlotId.Value,
                    out SaveStorageKey sourceSlotKey);

            if (!sourceKeyResult.Succeeded)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.SourceInvalid,
                    EchoSaveDiagnosticCodes.DeleteConfirmSourceInvalid,
                    sourceKeyResult.Message,
                    plan.SlotId,
                    plan.CurrentGenerationId);
            }

            string trashRecordId = string.Empty;
            SaveStorageResult move = default;
            bool moved = false;
            bool planConsumed = false;

            for (int attempt = 0;
                 attempt < maxTrashIdAttempts;
                 attempt++)
            {
                string token =
                    trashTokenFactory();

                if (!IsCanonicalToken(token))
                {
                    return SaveSlotDeleteResult.Failure(
                        SaveSlotDeleteStatus.TrashPublicationFailed,
                        EchoSaveDiagnosticCodes.DeleteConfirmTrashPublicationFailed,
                        "Chronicle trash identity generation returned a non-canonical token.",
                        plan.SlotId,
                        plan.CurrentGenerationId);
                }

                trashRecordId =
                    now.UtcDateTime.Ticks.ToString(
                        "D19",
                        CultureInfo.InvariantCulture) +
                    "-" +
                    token;

                SaveStorageResult destinationKeyResult =
                    SaveStorageKey.TryCreate(
                        "trash/" +
                        trashRecordId +
                        "/slot",
                        out SaveStorageKey destinationKey);

                if (!destinationKeyResult.Succeeded)
                {
                    return SaveSlotDeleteResult.Failure(
                        SaveSlotDeleteStatus.TrashPublicationFailed,
                        EchoSaveDiagnosticCodes.DeleteConfirmTrashPublicationFailed,
                        destinationKeyResult.Message,
                        plan.SlotId,
                        plan.CurrentGenerationId);
                }

                if (!planConsumed)
                {
                    consumedPlanIds.Add(
                        plan.PlanId);

                    planConsumed = true;
                }

                move =
                    publication.PublishNewTree(
                        sourceSlotKey,
                        destinationKey);

                if (move.Succeeded)
                {
                    moved = true;
                    break;
                }

                if (move.Status !=
                    SaveStorageStatus.Conflict)
                {
                    break;
                }
            }

            if (!moved)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.TrashPublicationFailed,
                    string.IsNullOrEmpty(move.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes.DeleteConfirmTrashPublicationFailed
                        : move.DiagnosticCode,
                    "Chronicle could not publish the complete live slot tree into recoverable trash. " +
                    move.Message,
                    plan.SlotId,
                    plan.CurrentGenerationId,
                    trashRecordId,
                    false,
                    false,
                    false);
            }

            bool activeCleared = false;

            if (catalog.HasActiveSlot &&
                catalog.ActiveSlotId == plan.SlotId)
            {
                SaveActiveSlotSelectionResult clear =
                    catalog.ClearActiveSlot();

                activeCleared =
                    clear.Succeeded &&
                    !clear.HasActiveSlot;
            }

            SaveSlotCatalogRefreshResult reconciliation =
                catalog.Refresh();

            if (!reconciliation.Succeeded ||
                reconciliation.Snapshot.TryGetEntry(
                    plan.SlotId,
                    out _))
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.PublishedCatalogReconciliationFailed,
                    EchoSaveDiagnosticCodes.DeleteConfirmCatalogReconciliationFailed,
                    "The Chronicle slot is durably in recoverable trash, but the live catalog could not be fully reconciled. The delete commit remains authoritative.",
                    plan.SlotId,
                    plan.CurrentGenerationId,
                    trashRecordId,
                    true,
                    false,
                    activeCleared,
                    SaveTrashRetentionResult.NotRequired(
                        "Trash maintenance was not attempted because live catalog reconciliation failed."));
            }

            SaveTrashRetentionResult retention =
                trashRetention.Apply(
                    maxTrashRecords);

            if (retention.MaintenanceFailed)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.PublishedTrashRetentionFailed,
                    string.IsNullOrEmpty(retention.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes.DeleteConfirmTrashRetentionFailed
                        : retention.DiagnosticCode,
                    "The Chronicle slot is durably deleted from the live catalog, but bounded recoverable-trash maintenance did not fully complete. " +
                    retention.Message,
                    plan.SlotId,
                    plan.CurrentGenerationId,
                    trashRecordId,
                    true,
                    true,
                    activeCleared,
                    retention);
            }

            return new SaveSlotDeleteResult(
                SaveSlotDeleteStatus.Succeeded,
                EchoSaveDiagnosticCodes.DeleteConfirmSucceeded,
                "The Chronicle slot was moved into recoverable trash, removed from the live catalog, and bounded trash maintenance completed.",
                plan.SlotId,
                plan.CurrentGenerationId,
                trashRecordId,
                true,
                true,
                activeCleared,
                retention);
        }

        private static bool IsCanonicalToken(
            string token)
        {
            if (string.IsNullOrEmpty(token) ||
                token.Length != 32)
            {
                return false;
            }

            for (int i = 0;
                 i < token.Length;
                 i++)
            {
                char c = token[i];

                if (!(c >= '0' && c <= '9' ||
                      c >= 'a' && c <= 'f'))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
