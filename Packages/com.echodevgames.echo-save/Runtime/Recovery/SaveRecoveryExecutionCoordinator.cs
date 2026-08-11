
using System;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// M4-08 explicit recovery mutation coordinator.
    ///
    /// Admission is owned by EchoSaveService. This coordinator assumes one
    /// admitted mutation, rebuilds M4-07 evidence, rejects stale plans before
    /// mutation, and republishes only head.json to an already verified
    /// immutable committed generation.
    /// </summary>
    internal sealed class SaveRecoveryExecutionCoordinator :
        ISaveRecoveryExecutor
    {
        private readonly ISaveStorageBackend storage;
        private readonly ISaveSerializer serializer;
        private readonly ISaveRecoveryPlanBuilder planBuilder;
        private readonly SaveSlotCatalog catalog;

        internal SaveRecoveryExecutionCoordinator(
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            ISaveRecoveryPlanBuilder planBuilder,
            SaveSlotCatalog catalog)
        {
            this.storage =
                storage ??
                throw new ArgumentNullException(
                    nameof(storage));

            this.serializer =
                serializer ??
                throw new ArgumentNullException(
                    nameof(serializer));

            this.planBuilder =
                planBuilder ??
                throw new ArgumentNullException(
                    nameof(planBuilder));

            this.catalog =
                catalog ??
                throw new ArgumentNullException(
                    nameof(catalog));
        }

        public SaveRecoveryResult Execute(
            SaveRecoveryPlan plan,
            SaveRecoveryCandidate candidate)
        {
            SaveRecoveryResult requestFailure =
                ValidateRequest(
                    plan,
                    candidate);

            if (requestFailure != null)
            {
                return requestFailure;
            }

            SaveSlotId slotId =
                plan.SlotId;

            SaveGenerationId generationId =
                candidate.GenerationId;

            SaveRecoveryPlan fresh =
                planBuilder.Build(
                    slotId);

            if (fresh == null ||
                !fresh.Succeeded)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .RevalidationFailed,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteRevalidationFailed,
                    fresh == null
                        ? "Chronicle recovery execution could not rebuild the source recovery plan."
                        : "Chronicle recovery execution could not revalidate the source recovery plan. " +
                          fresh.DiagnosticCode +
                          " " +
                          fresh.Message,
                    slotId,
                    generationId);
            }

            if (!string.Equals(
                    plan.SourceProvenanceFingerprint,
                    fresh.SourceProvenanceFingerprint,
                    StringComparison.Ordinal) ||
                fresh.Status !=
                    SaveRecoveryPlanStatus.RecoveryAvailable)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus.StalePlan,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteStalePlan,
                    "The Chronicle recovery plan no longer matches the current source evidence. Build a fresh recovery plan before retrying.",
                    slotId,
                    generationId);
            }

            if (!TryFindExactCandidate(
                    fresh,
                    candidate,
                    out SaveRecoveryCandidate freshCandidate))
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .CandidateInvalid,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteCandidateInvalid,
                    "The selected recovery generation is no longer one fully verified candidate in the freshly rebuilt recovery plan.",
                    slotId,
                    generationId);
            }

            if (!(storage is
                    ISaveStoragePublicationBackend publication))
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .BackendUnsupported,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteBackendUnsupported,
                    "The active Chronicle storage backend does not expose the small-current-object publication capability required for recovery execution.",
                    slotId,
                    generationId);
            }

            SaveStoragePublicationCapabilities capabilities =
                publication.PublicationCapabilities;

            if (!capabilities.SupportsCurrentObjectPublication)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .BackendUnsupported,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteBackendUnsupported,
                    "The active Chronicle storage backend does not advertise current-object publication required for recovery execution.",
                    slotId,
                    generationId);
            }

            SaveStorageResult keyResult =
                SaveGenerationStorageKeys.TryCreate(
                    slotId,
                    freshCandidate.GenerationId,
                    out SaveGenerationStorageKeys keys);

            if (!keyResult.Succeeded)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteInvalidRequest,
                    keyResult.Message,
                    slotId,
                    generationId);
            }

            if (!TryDetermineNextUpdateSequence(
                    slotId,
                    keys.Head,
                    out long updateSequence,
                    out string sequenceFailure))
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .HeadPublicationFailed,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteHeadPublicationFailed,
                    sequenceFailure,
                    slotId,
                    generationId);
            }

            SaveHeadPointer recoveredHead =
                new SaveHeadPointer
                {
                    slotId =
                        slotId.Value,
                    currentGenerationId =
                        generationId.Value,
                    previousGenerationId =
                        string.Empty,
                    updateSequence =
                        updateSequence
                };

            SaveDocumentValidationResult validation =
                SaveCommitDocumentValidator
                    .ValidateHead(
                        recoveredHead);

            if (!validation.Succeeded)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .HeadPublicationFailed,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteHeadPublicationFailed,
                    validation.Message,
                    slotId,
                    generationId);
            }

            SaveSerializerResult serialized =
                serializer.Serialize(
                    recoveredHead,
                    out string headJson);

            if (!serialized.Succeeded)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .HeadPublicationFailed,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteHeadPublicationFailed,
                    serialized.Message,
                    slotId,
                    generationId);
            }

            SaveStorageResult headPublish =
                publication.PublishCurrentObject(
                    keys.Head,
                    Encoding.UTF8.GetBytes(
                        headJson));

            if (!headPublish.Succeeded)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .HeadPublicationFailed,
                    string.IsNullOrEmpty(
                        headPublish.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .RecoveryExecuteHeadPublicationFailed
                        : headPublish.DiagnosticCode,
                    "Chronicle recovery head publication failed. " +
                    headPublish.Message,
                    slotId,
                    generationId);
            }

            bool hadActiveSlot =
                catalog.HasActiveSlot;

            SaveSlotId activeSlotBefore =
                hadActiveSlot
                    ? catalog.ActiveSlotId
                    : default;

            SaveSlotCatalogRefreshResult refresh =
                catalog.Refresh();

            if (!refresh.Succeeded)
            {
                return new SaveRecoveryResult(
                    SaveRecoveryExecutionStatus
                        .CatalogReconciliationFailed,
                    string.IsNullOrEmpty(
                        refresh.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .RecoveryExecuteCatalogReconciliationFailed
                        : refresh.DiagnosticCode,
                    "The recovery head is durably published, but Chronicle could not reconcile the derived slot catalog. " +
                    refresh.Message,
                    slotId,
                    generationId,
                    true,
                    false);
            }

            if (hadActiveSlot &&
                (!catalog.HasActiveSlot ||
                 catalog.ActiveSlotId !=
                    activeSlotBefore))
            {
                SaveActiveSlotSelectionResult restored =
                    catalog.SelectActiveSlot(
                        activeSlotBefore);

                if (!restored.Succeeded)
                {
                    return new SaveRecoveryResult(
                        SaveRecoveryExecutionStatus
                            .CatalogReconciliationFailed,
                        EchoSaveDiagnosticCodes
                            .RecoveryExecuteCatalogReconciliationFailed,
                        "The recovery head is durably published, but Chronicle could not preserve the pre-existing active-slot selection during catalog reconciliation.",
                        slotId,
                        generationId,
                        true,
                        false);
                }
            }

            if (!refresh.Snapshot.TryGetEntry(
                    slotId,
                    out SaveSlotCatalogEntry entry) ||
                entry == null ||
                !entry.IsSelectable ||
                entry.CurrentGenerationId !=
                    generationId)
            {
                return new SaveRecoveryResult(
                    SaveRecoveryExecutionStatus
                        .CatalogReconciliationFailed,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteCatalogReconciliationFailed,
                    "The recovery head is durably published, but the reconciled slot catalog does not report the selected generation as one healthy current entry.",
                    slotId,
                    generationId,
                    true,
                    false);
            }

            return new SaveRecoveryResult(
                SaveRecoveryExecutionStatus.Succeeded,
                EchoSaveDiagnosticCodes
                    .RecoveryExecuteSucceeded,
                "The Chronicle revalidated the recovery plan, repointed head.json to the selected verified generation, and reconciled the slot catalog.",
                slotId,
                generationId,
                true,
                true);
        }

        private SaveRecoveryResult ValidateRequest(
            SaveRecoveryPlan plan,
            SaveRecoveryCandidate candidate)
        {
            if (plan == null ||
                plan.Status !=
                    SaveRecoveryPlanStatus.RecoveryAvailable ||
                !plan.RecoveryRequired ||
                string.IsNullOrWhiteSpace(
                    plan.SourceProvenanceFingerprint) ||
                !SaveSlotId.TryParse(
                    plan.SlotId.Value,
                    out SaveSlotId validatedSlot) ||
                !SaveGenerationId.TryParse(
                    candidate.GenerationId.Value,
                    out SaveGenerationId validatedGeneration))
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteInvalidRequest,
                    "Chronicle recovery execution requires one successful recovery-required M4-07 plan with provenance and one valid selected generation.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    candidate.GenerationId);
            }

            if (!TryFindExactCandidate(
                    plan,
                    candidate,
                    out _))
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .CandidateInvalid,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteCandidateInvalid,
                    "The selected generation is not one exact immutable candidate in the supplied recovery plan.",
                    validatedSlot,
                    validatedGeneration);
            }

            return null;
        }

        private bool TryDetermineNextUpdateSequence(
            SaveSlotId slotId,
            SaveStorageKey headKey,
            out long sequence,
            out string failure)
        {
            sequence =
                1;

            failure =
                string.Empty;

            SaveStorageReadResult read =
                storage.Read(
                    headKey);

            if (!read.Succeeded)
            {
                return true;
            }

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveHeadPointer existing);

            if (!deserialized.Succeeded ||
                existing == null)
            {
                return true;
            }

            SaveDocumentValidationResult validation =
                SaveCommitDocumentValidator
                    .ValidateHead(
                        existing);

            if (!validation.Succeeded ||
                !string.Equals(
                    existing.slotId,
                    slotId.Value,
                    StringComparison.Ordinal))
            {
                return true;
            }

            if (existing.updateSequence ==
                long.MaxValue)
            {
                failure =
                    "The structurally valid Chronicle source head update sequence cannot advance safely.";

                return false;
            }

            sequence =
                existing.updateSequence + 1;

            return true;
        }

        private static bool TryFindExactCandidate(
            SaveRecoveryPlan plan,
            SaveRecoveryCandidate expected,
            out SaveRecoveryCandidate actual)
        {
            actual =
                default;

            if (plan == null)
            {
                return false;
            }

            for (int i = 0;
                 i < plan.Candidates.Count;
                 i++)
            {
                SaveRecoveryCandidate candidate =
                    plan.Candidates[i];

                if (CandidateMatches(
                        candidate,
                        expected))
                {
                    actual =
                        candidate;

                    return true;
                }
            }

            return false;
        }

        private static bool CandidateMatches(
            SaveRecoveryCandidate left,
            SaveRecoveryCandidate right) =>
            left.GenerationId ==
                right.GenerationId &&
            string.Equals(
                left.TechnicalTimestampUtc,
                right.TechnicalTimestampUtc,
                StringComparison.Ordinal) &&
            string.Equals(
                left.SaveKind,
                right.SaveKind,
                StringComparison.Ordinal) &&
            string.Equals(
                left.ProjectId,
                right.ProjectId,
                StringComparison.Ordinal) &&
            string.Equals(
                left.ProjectVersion,
                right.ProjectVersion,
                StringComparison.Ordinal) &&
            string.Equals(
                left.BuildId,
                right.BuildId,
                StringComparison.Ordinal);
    }
}
