//----- UnityDirectSceneRootFactory.cs START -----

using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    internal sealed class UnityDirectSceneRootFactory :
        IDirectSceneRootFactory
    {
        internal static UnityDirectSceneRootFactory Shared { get; } =
            new UnityDirectSceneRootFactory();

        private UnityDirectSceneRootFactory()
        {
        }

        public EchoLaunchRoot Instantiate(EchoLaunchRoot prefab)
        {
            return Object.Instantiate(prefab);
        }

        public void Destroy(EchoLaunchRoot root)
        {
            if (root != null)
            {
                Object.Destroy(root.gameObject);
            }
        }
    }
}

//----- UnityDirectSceneRootFactory.cs END -----
