//----- SplashPresentationMode.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Defines the default uGUI presentation profile used while a splash
    /// sequence is active.
    /// </summary>
    public enum SplashPresentationMode
    {
        /// <summary>
        /// Preserves the legacy status-visible presentation.
        /// </summary>
        SplashAndStatus = 0,

        /// <summary>
        /// Hides routine launch-status chrome on the normal success path and
        /// presents only the authored splash surface.
        /// </summary>
        SplashOnly = 1,
    }
}

//----- SplashPresentationMode.cs END -----
