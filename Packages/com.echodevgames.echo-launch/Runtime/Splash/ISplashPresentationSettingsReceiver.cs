//----- ISplashPresentationSettingsReceiver.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Optional neutral contract for a launch presenter that can consume the
    /// effective sequence-level splash presentation settings before binding.
    ///
    /// The contract carries presentation intent only. It does not grant input,
    /// audio, lifecycle, or launch authority.
    /// </summary>
    public interface ISplashPresentationSettingsReceiver
    {
        /// <summary>
        /// Configures the effective splash presentation definition for the
        /// upcoming authoritative launch binding.
        /// </summary>
        void ConfigureSplashPresentation(
            SplashPresentationSettings settings);
    }
}

//----- ISplashPresentationSettingsReceiver.cs END -----
