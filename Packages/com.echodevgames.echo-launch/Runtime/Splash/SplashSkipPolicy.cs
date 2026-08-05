//----- SplashSkipPolicy.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Defines whether one authored image splash can be skipped.
    ///
    /// A permitted skip never bypasses the entry's minimum display time.
    /// Input binding remains project-owned; the package consumes only a
    /// neutral skip request.
    /// </summary>
    public enum SplashSkipPolicy
    {
        /// <summary>
        /// The entry always completes its authored timeline.
        /// </summary>
        Disallowed = 0,

        /// <summary>
        /// A skip request may end the entry after its minimum display time.
        /// Requests received earlier remain latched until that boundary.
        /// </summary>
        AfterMinimumDisplay = 1,
    }
}

//----- SplashSkipPolicy.cs END -----
