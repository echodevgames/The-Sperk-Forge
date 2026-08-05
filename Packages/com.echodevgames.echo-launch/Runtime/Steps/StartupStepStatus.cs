//----- StartupStepStatus.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Describes the current or terminal state of one startup step.
    /// </summary>
    public enum StartupStepStatus
    {
        NotStarted = 0,
        Running = 1,
        Succeeded = 2,
        Warning = 3,
        RecoverableFailure = 4,
        BlockingFailure = 5,
        Skipped = 6,
        TimedOut = 7,
        Cancelled = 8
    }
}

//----- StartupStepStatus.cs END -----