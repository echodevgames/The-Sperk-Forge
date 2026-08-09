using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primary package-local Chronicle lifecycle surface established in M1.
    /// Save/load operations are added by later checkpoints.
    /// </summary>
    public interface IEchoSaveService
    {
        EchoSaveServiceState State { get; }

        EchoSaveConfiguration Configuration { get; }

        Awaitable<EchoSaveLifecycleResult> InitializeAsync();

        Awaitable<EchoSaveLifecycleResult> ShutdownAsync();
    }
}
