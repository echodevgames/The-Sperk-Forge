//----- LaunchStatus.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Describes the current phase of an application launch attempt.
    /// </summary>
    public enum LaunchStatus
    {
        None = 0,
        AuthorityClaimed = 1,
        Validating = 2,
        Running = 3,
        Transitioning = 4,
        Completed = 5,
        Failed = 6,
        Interrupted = 7
    }
}

//----- LaunchStatus.cs END -----