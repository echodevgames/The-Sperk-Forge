using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primary package-local Chronicle service surface.
    ///
    /// M4-10 adds two-step destructive slot deletion planning and confirmed
    /// recoverable trash while preserving root-local admission, source
    /// freshness, and truthful post-commit catalog/maintenance semantics.
    /// </summary>
    public interface IEchoSaveService
    {
        EchoSaveServiceState State { get; }

        EchoSaveConfiguration Configuration { get; }

        Awaitable<EchoSaveLifecycleResult> InitializeAsync();

        Awaitable<SaveOperationResult> SaveAsync(
            SaveRequest request);

        AutosaveSubmissionResult RequestAutosave(
            AutosaveRequest request);

        Awaitable<SaveRecoveryPlan> BuildRecoveryPlanAsync(
            SaveSlotId slotId);

        Awaitable<SaveRecoveryResult> ExecuteRecoveryAsync(
            SaveRecoveryPlan plan,
            SaveRecoveryCandidate candidate);

        Awaitable<SaveSlotRenameResult> RenameSlotAsync(
            SaveSlotRenameRequest request);

        Awaitable<SaveSlotDuplicateResult> DuplicateSlotAsync(
            SaveSlotDuplicateRequest request);

        Awaitable<SaveDeletionPlan> PrepareDeleteSlotAsync(
            SaveSlotId slotId);

        Awaitable<SaveSlotDeleteResult> ConfirmDeleteSlotAsync(
            SaveDeletionPlan plan);

        Awaitable<EchoSaveLifecycleResult> ShutdownAsync();
    }
}
