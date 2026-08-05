//----- StartupStepFailureAction.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Defines the two failure actions supported by the First Light MVP.
    /// </summary>
    public enum StartupStepFailureAction
    {
        /// <summary>
        /// Stop startup and prevent launch handoff.
        /// </summary>
        BlockLaunch = 0,

        /// <summary>
        /// Record a warning and allow the sequence to continue.
        /// </summary>
        ContinueWithWarning = 1
    }
}

//----- StartupStepFailureAction.cs END -----
