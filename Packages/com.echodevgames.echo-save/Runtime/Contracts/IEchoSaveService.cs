using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primary package-local Chronicle service surface.
    ///
    /// M4-07 adds bounded read-only recovery-plan construction while keeping
    /// recovery execution and all durable repair mutation separately deferred.
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

        Awaitable<EchoSaveLifecycleResult> ShutdownAsync();
    }
}
