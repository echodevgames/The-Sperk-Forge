using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primary package-local Chronicle service surface.
    ///
    /// M4-05 adds explicit caller-triggered autosave submission with one
    /// latest-wins pending request while reusing M4-04 admission and save truth.
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

        Awaitable<EchoSaveLifecycleResult> ShutdownAsync();
    }
}
