//----- IDirectSceneRuntimeEnvironment.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Internal runtime environment seam used to make the direct-scene release
    /// gate deterministic under tests.
    /// </summary>
    internal interface IDirectSceneRuntimeEnvironment
    {
        bool IsEditor { get; }

        bool IsDevelopmentBuild { get; }
    }
}

//----- IDirectSceneRuntimeEnvironment.cs END -----
