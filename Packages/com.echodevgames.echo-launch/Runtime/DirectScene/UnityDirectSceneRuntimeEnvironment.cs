//----- UnityDirectSceneRuntimeEnvironment.cs START -----

using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    internal sealed class UnityDirectSceneRuntimeEnvironment :
        IDirectSceneRuntimeEnvironment
    {
        internal static UnityDirectSceneRuntimeEnvironment Shared { get; } =
            new UnityDirectSceneRuntimeEnvironment();

        private UnityDirectSceneRuntimeEnvironment()
        {
        }

        public bool IsEditor =>
            Application.isEditor;

        public bool IsDevelopmentBuild =>
            Debug.isDebugBuild;
    }
}

//----- UnityDirectSceneRuntimeEnvironment.cs END -----
