//----- IDirectSceneRootFactory.cs START -----

namespace EchoDevGames.EchoLaunch
{
    internal interface IDirectSceneRootFactory
    {
        EchoLaunchRoot Instantiate(EchoLaunchRoot prefab);

        void Destroy(EchoLaunchRoot root);
    }
}

//----- IDirectSceneRootFactory.cs END -----
