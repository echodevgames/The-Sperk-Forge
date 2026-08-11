using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primary package-local Chronicle service surface.
    ///
    /// M4-09 adds bounded non-destructive slot rename and full-state
    /// duplication while preserving stable slot identity, immutable generation
    /// publication, and truthful post-publication catalog semantics.
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

        Awaitable<EchoSaveLifecycleResult> ShutdownAsync();
    }
}
