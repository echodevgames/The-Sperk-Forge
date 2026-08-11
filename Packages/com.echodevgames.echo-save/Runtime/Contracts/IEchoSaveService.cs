using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primary package-local Chronicle service surface.
    ///
    /// M4-08 adds explicit admitted recovery execution over immutable M4-07
    /// plans while preserving generation immutability and truthful head/catalog
    /// commit semantics.
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

        Awaitable<EchoSaveLifecycleResult> ShutdownAsync();
    }
}
