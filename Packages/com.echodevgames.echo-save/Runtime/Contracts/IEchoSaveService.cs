using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primary package-local Chronicle service surface.
    ///
    /// M4-04 adds the first public active-slot manual-save operation while
    /// autosave and the remaining mutating operations stay deferred.
    /// </summary>
    public interface IEchoSaveService
    {
        EchoSaveServiceState State { get; }

        EchoSaveConfiguration Configuration { get; }

        Awaitable<EchoSaveLifecycleResult> InitializeAsync();

        Awaitable<SaveOperationResult> SaveAsync(
            SaveRequest request);

        Awaitable<EchoSaveLifecycleResult> ShutdownAsync();
    }
}
