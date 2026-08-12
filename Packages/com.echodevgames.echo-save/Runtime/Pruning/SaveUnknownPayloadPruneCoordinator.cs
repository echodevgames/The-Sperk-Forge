
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Exact-ID, plan-bound unknown-payload prune.
    ///
    /// It never edits historical generation files. Successful confirmation
    /// republishes the validated source transport minus only explicitly named
    /// still-unclaimed entries and advances head through existing publication.
    /// </summary>
    internal sealed class SaveUnknownPayloadPruneCoordinator
    {
        private readonly SaveSlotCatalog catalog;
        private readonly SaveParticipantRegistry participantRegistry;
        private readonly SaveUnknownPayloadStore unknownPayloadStore;
        private readonly SaveUnknownPayloadPruneSourceReader sourceReader;
        private readonly SaveGenerationPublicationCoordinator publication;
        private readonly SaveGenerationRetentionCoordinator retention;
        private readonly SaveRetentionPolicy retentionPolicy;
        private readonly string sessionId;
        private readonly Func<DateTimeOffset> clock;
        private readonly TimeSpan planLifetime;

        private readonly HashSet<string> consumedPlanIds =
            new HashSet<string>(
                StringComparer.Ordinal);

        internal SaveUnknownPayloadPruneCoordinator(
            SaveSlotCatalog catalog,
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            IIntegrityProvider integrity,
            SaveParticipantRegistry participantRegistry,
            SaveUnknownPayloadStore unknownPayloadStore,
            SaveGenerationPublicationCoordinator publication,
            SaveGenerationRetentionCoordinator retention,
            SaveRetentionPolicy retentionPolicy,
            string sessionId,
            Func<DateTimeOffset> clock,
            TimeSpan planLifetime)
        {
            this.catalog =
                catalog ??
                throw new ArgumentNullException(
                    nameof(catalog));

            this.participantRegistry =
                participantRegistry ??
                throw new ArgumentNullException(
                    nameof(participantRegistry));

            this.unknownPayloadStore =
                unknownPayloadStore ??
                throw new ArgumentNullException(
                    nameof(unknownPayloadStore));

            this.publication =
                publication ??
                throw new ArgumentNullException(
                    nameof(publication));

            this.retention =
                retention ??
                throw new ArgumentNullException(
                    nameof(retention));

            if (!retentionPolicy.IsValid)
            {
                throw new ArgumentException(
                    "Chronicle unknown-prune requires one valid retention policy.",
                    nameof(retentionPolicy));
            }

            this.retentionPolicy =
                retentionPolicy;

            if (string.IsNullOrEmpty(
                    sessionId))
            {
                throw new ArgumentException(
                    "Chronicle unknown-prune session identity is required.",
                    nameof(sessionId));
            }

            this.sessionId =
                sessionId;

            this.clock =
                clock ??
                throw new ArgumentNullException(
                    nameof(clock));

            if (planLifetime <=
                TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(planLifetime));
            }

            this.planLifetime =
                planLifetime;

            sourceReader =
                new SaveUnknownPayloadPruneSourceReader(
                    storage,
                    serializer,
                    integrity);
        }

        internal SaveUnknownPayloadPrunePlan Prepare(
            SaveSlotId slotId,
            IReadOnlyList<SaveParticipantId> requestedIds)
        {
            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return SaveUnknownPayloadPrunePlan.Failure(
                    SaveUnknownPayloadPrunePlanStatus.InvalidRequest,
                    "ECHOSAVE-PRUNE-REQUEST",
                    "Chronicle unknown-prune Preview requires one valid technical slot ID.",
                    slotId);
            }

            if (!TryNormalizeRequestedIds(
                    requestedIds,
                    out SaveParticipantId[] normalized,
                    out string requestFailure))
            {
                return SaveUnknownPayloadPrunePlan.Failure(
                    SaveUnknownPayloadPrunePlanStatus.InvalidRequest,
                    "ECHOSAVE-PRUNE-REQUEST",
                    requestFailure,
                    validatedSlot);
            }

            SaveUnknownPayloadPruneSourceReadResult source =
                sourceReader.Read(
                    validatedSlot);

            if (!source.Succeeded)
            {
                return SaveUnknownPayloadPrunePlan.Failure(
                    source.Status ==
                        SaveUnknownPayloadPruneSourceStatus
                            .HeadUnavailable
                        ? SaveUnknownPayloadPrunePlanStatus
                            .SourceUnavailable
                        : SaveUnknownPayloadPrunePlanStatus
                            .SourceInvalid,
                    source.DiagnosticCode,
                    source.Message,
                    validatedSlot);
            }

            for (int i = 0;
                 i < normalized.Length;
                 i++)
            {
                SaveParticipantId requested =
                    normalized[i];

                if (participantRegistry.TryResolve(
                        requested,
                        out _))
                {
                    return SaveUnknownPayloadPrunePlan.Failure(
                        SaveUnknownPayloadPrunePlanStatus
                            .RequestedIdClaimed,
                        "ECHOSAVE-PRUNE-CLAIMED",
                        $"Chronicle unknown-prune refused '{requested.Value}' because an active participant currently claims that identity.",
                        validatedSlot);
                }

                if (!ContainsPayloadId(
                        source.Snapshot.PayloadEntries,
                        requested))
                {
                    return SaveUnknownPayloadPrunePlan.Failure(
                        SaveUnknownPayloadPrunePlanStatus
                            .RequestedIdNotFound,
                        "ECHOSAVE-PRUNE-NOT-FOUND",
                        $"Chronicle unknown-prune could not find requested opaque participant ID '{requested.Value}' in the current stored generation.",
                        validatedSlot);
                }
            }

            DateTimeOffset issued =
                clock();

            return new SaveUnknownPayloadPrunePlan(
                SaveUnknownPayloadPrunePlanStatus.Ready,
                string.Empty,
                "Chronicle prepared one immutable exact-ID unknown-payload prune plan. Preview performed no durable mutation.",
                Guid.NewGuid().ToString("N"),
                sessionId,
                validatedSlot,
                source.Snapshot.GenerationId,
                source.Snapshot.ProvenanceFingerprint,
                normalized,
                issued,
                issued + planLifetime,
                source.Snapshot);
        }

        internal SaveUnknownPayloadPruneResult Confirm(
            SaveUnknownPayloadPrunePlan plan)
        {
            if (plan == null ||
                !plan.Succeeded ||
                string.IsNullOrEmpty(
                    plan.PlanId) ||
                plan.SourceSnapshot == null ||
                plan.ParticipantIds.Count == 0)
            {
                return Failure(
                    SaveUnknownPayloadPruneStatus.InvalidPlan,
                    "ECHOSAVE-PRUNE-PLAN",
                    "Chronicle unknown-prune confirmation requires one successful immutable plan.",
                    plan);
            }

            if (!string.Equals(
                    plan.SessionId,
                    sessionId,
                    StringComparison.Ordinal))
            {
                return Failure(
                    SaveUnknownPayloadPruneStatus.ForeignSession,
                    "ECHOSAVE-PRUNE-SESSION",
                    "The unknown-prune plan belongs to a different Chronicle runtime session.",
                    plan);
            }

            if (consumedPlanIds.Contains(
                    plan.PlanId))
            {
                return Failure(
                    SaveUnknownPayloadPruneStatus.Consumed,
                    "ECHOSAVE-PRUNE-CONSUMED",
                    "The unknown-prune plan has already crossed its one-use confirmation boundary.",
                    plan);
            }

            if (clock() >
                plan.ExpiresUtc)
            {
                return Failure(
                    SaveUnknownPayloadPruneStatus.Expired,
                    "ECHOSAVE-PRUNE-EXPIRED",
                    "The unknown-prune plan expired before confirmation.",
                    plan);
            }

            SaveUnknownPayloadPruneSourceReadResult source =
                sourceReader.Revalidate(
                    plan.SourceSnapshot);

            if (!source.Succeeded)
            {
                return Failure(
                    source.Status ==
                        SaveUnknownPayloadPruneSourceStatus
                            .SourceStale
                        ? SaveUnknownPayloadPruneStatus
                            .SourceStale
                        : SaveUnknownPayloadPruneStatus
                            .SourceInvalid,
                    source.DiagnosticCode,
                    source.Message,
                    plan);
            }

            HashSet<string> remove =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int i = 0;
                 i < plan.ParticipantIds.Count;
                 i++)
            {
                SaveParticipantId requested =
                    plan.ParticipantIds[i];

                if (participantRegistry.TryResolve(
                        requested,
                        out _))
                {
                    return Failure(
                        SaveUnknownPayloadPruneStatus
                            .RequestedIdClaimed,
                        "ECHOSAVE-PRUNE-CLAIMED",
                        $"Chronicle unknown-prune confirmation refused '{requested.Value}' because that identity became claimed after Preview.",
                        plan);
                }

                if (!ContainsPayloadId(
                        source.Snapshot.PayloadEntries,
                        requested))
                {
                    return Failure(
                        SaveUnknownPayloadPruneStatus
                            .SourceStale,
                        "ECHOSAVE-PRUNE-STALE",
                        $"Chronicle unknown-prune confirmation could no longer find requested source ID '{requested.Value}'.",
                        plan);
                }

                remove.Add(
                    requested.Value);
            }

            SavePayloadEntry[] payload =
                FilterPayload(
                    source.Snapshot.PayloadEntries,
                    remove);

            SavePayloadInventoryEntry[] inventory =
                FilterInventory(
                    source.Snapshot.InventoryEntries,
                    remove);

            consumedPlanIds.Add(
                plan.PlanId);

            SaveManifest manifest =
                source.Snapshot.Manifest;

            SaveGenerationPublicationResult published =
                publication.PublishStoredTransportGeneration(
                    plan.SlotId,
                    manifest.projectId,
                    manifest.projectVersion,
                    manifest.buildId,
                    manifest.displayName,
                    payload,
                    inventory,
                    manifest.saveKind,
                    plan.SourceGenerationId);

            if (!published.Succeeded)
            {
                return SaveUnknownPayloadPruneResult.Failure(
                    published.GenerationPublished
                        ? SaveUnknownPayloadPruneStatus
                            .HeadPublicationFailed
                        : SaveUnknownPayloadPruneStatus
                            .PublicationFailed,
                    published.DiagnosticCode,
                    published.Message,
                    plan.SlotId,
                    plan.SourceGenerationId,
                    published.GenerationId,
                    remove.Count,
                    0,
                    published.GenerationPublished,
                    published.HeadPublished);
            }

            List<SavePayloadEntry> remainingUnknown =
                new List<SavePayloadEntry>();

            for (int i = 0;
                 i < payload.Length;
                 i++)
            {
                SavePayloadEntry entry =
                    payload[i];

                SaveParticipantId participantId =
                    new SaveParticipantId(
                        entry.participantId);

                if (!participantRegistry.TryResolve(
                        participantId,
                        out _))
                {
                    remainingUnknown.Add(
                        SaveUnknownPayloadSnapshot.CloneEntry(
                            entry));
                }
            }

            SaveUnknownPayloadStoreResult store =
                unknownPayloadStore.TryReplace(
                    remainingUnknown,
                    plan.SlotId,
                    published.GenerationId);

            if (!store.Succeeded)
            {
                return SaveUnknownPayloadPruneResult.Failure(
                    SaveUnknownPayloadPruneStatus
                        .PublishedSessionReconciliationFailed,
                    store.DiagnosticCode,
                    "The unknown-prune generation/head commit is authoritative, but the session unknown-payload snapshot could not reconcile. " +
                    store.Message,
                    plan.SlotId,
                    plan.SourceGenerationId,
                    published.GenerationId,
                    remove.Count,
                    remainingUnknown.Count,
                    true,
                    true);
            }

            SaveSlotCatalogRefreshResult reconciliation =
                catalog.Refresh();

            if (!reconciliation.Succeeded)
            {
                return SaveUnknownPayloadPruneResult.Failure(
                    SaveUnknownPayloadPruneStatus
                        .PublishedCatalogReconciliationFailed,
                    reconciliation.DiagnosticCode,
                    "The unknown-prune generation/head commit is authoritative, but the live catalog could not reconcile. " +
                    reconciliation.Message,
                    plan.SlotId,
                    plan.SourceGenerationId,
                    published.GenerationId,
                    remove.Count,
                    remainingUnknown.Count,
                    true,
                    true,
                    false);
            }

            SaveRetentionResult retentionResult =
                retention.Apply(
                    plan.SlotId,
                    retentionPolicy);

            bool maintenanceFailed =
                retentionResult.MaintenanceFailed ||
                reconciliation.CacheMaintenanceFailed;

            if (maintenanceFailed)
            {
                string diagnostic =
                    retentionResult.MaintenanceFailed
                        ? retentionResult.DiagnosticCode
                        : reconciliation.CacheDiagnosticCode;

                string maintenanceMessage =
                    retentionResult.MaintenanceFailed
                        ? retentionResult.Message
                        : reconciliation.CacheMessage;

                return new SaveUnknownPayloadPruneResult(
                    SaveUnknownPayloadPruneStatus
                        .PublishedMaintenanceFailed,
                    diagnostic,
                    "The unknown-prune generation/head commit and live catalog reconciliation succeeded, but bounded derived maintenance reported a failure. " +
                    maintenanceMessage,
                    plan.SlotId,
                    plan.SourceGenerationId,
                    published.GenerationId,
                    remove.Count,
                    remainingUnknown.Count,
                    true,
                    true,
                    true,
                    true);
            }

            return new SaveUnknownPayloadPruneResult(
                SaveUnknownPayloadPruneStatus.Succeeded,
                string.Empty,
                $"Chronicle published one new immutable generation after pruning exactly {remove.Count} explicitly named unknown payload entries. Historical generations were not rewritten.",
                plan.SlotId,
                plan.SourceGenerationId,
                published.GenerationId,
                remove.Count,
                remainingUnknown.Count,
                true,
                true,
                true,
                false);
        }

        private static bool TryNormalizeRequestedIds(
            IReadOnlyList<SaveParticipantId> requested,
            out SaveParticipantId[] normalized,
            out string failure)
        {
            normalized =
                Array.Empty<SaveParticipantId>();

            failure =
                string.Empty;

            if (requested == null ||
                requested.Count == 0)
            {
                failure =
                    "Chronicle unknown-prune requires at least one explicit participant ID.";
                return false;
            }

            if (requested.Count >
                SaveUnknownPayloadStore.DefaultMaxEntries)
            {
                failure =
                    "Chronicle unknown-prune request exceeds the bounded unknown-payload entry limit.";
                return false;
            }

            SortedSet<SaveParticipantId> unique =
                new SortedSet<SaveParticipantId>();

            for (int i = 0;
                 i < requested.Count;
                 i++)
            {
                if (!SaveParticipantId.TryParse(
                        requested[i].Value,
                        out SaveParticipantId validated))
                {
                    failure =
                        "Chronicle unknown-prune contains an invalid participant ID.";
                    return false;
                }

                unique.Add(
                    validated);
            }

            normalized =
                new SaveParticipantId[
                    unique.Count];

            unique.CopyTo(
                normalized);

            return normalized.Length > 0;
        }

        private static bool ContainsPayloadId(
            SavePayloadEntry[] entries,
            SaveParticipantId participantId)
        {
            for (int i = 0;
                 i < entries.Length;
                 i++)
            {
                if (entries[i] != null &&
                    string.Equals(
                        entries[i].participantId,
                        participantId.Value,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static SavePayloadEntry[] FilterPayload(
            SavePayloadEntry[] source,
            HashSet<string> remove)
        {
            List<SavePayloadEntry> kept =
                new List<SavePayloadEntry>();

            for (int i = 0;
                 i < source.Length;
                 i++)
            {
                SavePayloadEntry entry =
                    source[i];

                if (entry != null &&
                    !remove.Contains(
                        entry.participantId))
                {
                    kept.Add(
                        SaveUnknownPayloadSnapshot.CloneEntry(
                            entry));
                }
            }

            return kept.ToArray();
        }

        private static SavePayloadInventoryEntry[] FilterInventory(
            SavePayloadInventoryEntry[] source,
            HashSet<string> remove)
        {
            List<SavePayloadInventoryEntry> kept =
                new List<SavePayloadInventoryEntry>();

            for (int i = 0;
                 i < source.Length;
                 i++)
            {
                SavePayloadInventoryEntry entry =
                    source[i];

                if (entry == null ||
                    remove.Contains(
                        entry.participantId))
                {
                    continue;
                }

                kept.Add(
                    new SavePayloadInventoryEntry
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
                    });
            }

            return kept.ToArray();
        }

        private static SaveUnknownPayloadPruneResult Failure(
            SaveUnknownPayloadPruneStatus status,
            string diagnosticCode,
            string message,
            SaveUnknownPayloadPrunePlan plan) =>
            SaveUnknownPayloadPruneResult.Failure(
                status,
                diagnosticCode,
                message,
                plan == null
                    ? default
                    : plan.SlotId,
                plan == null
                    ? default
                    : plan.SourceGenerationId);
    }
}
