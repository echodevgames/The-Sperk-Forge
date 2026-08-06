//----- DirectSceneInitializationStatus.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stable one-shot settlement states for EchoDirectSceneInitializer.
    /// </summary>
    public enum DirectSceneInitializationStatus
    {
        NotStarted = 0,
        ReusedExistingAuthority = 1,
        CreatedDevelopmentAuthority = 2,
        BlockedByPolicy = 3,
        BlockedByEnvironment = 4,
        InvalidConfiguration = 5,
        InstantiationFailed = 6
    }
}

//----- DirectSceneInitializationStatus.cs END -----
