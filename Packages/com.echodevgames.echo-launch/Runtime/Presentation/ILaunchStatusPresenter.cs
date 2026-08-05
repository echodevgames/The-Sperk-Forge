//----- ILaunchStatusPresenter.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Neutral presentation contract for startup-only launch status.
    ///
    /// Presenters observe immutable accepted state. They do not own launch
    /// authority, lifecycle transitions, startup work, destination loading,
    /// report finalization, or general UI navigation.
    /// </summary>
    public interface ILaunchStatusPresenter
    {
        /// <summary>
        /// Binds the presenter to one authoritative launch attempt.
        /// </summary>
        void Bind(
            LaunchProgressSnapshot initialSnapshot);

        /// <summary>
        /// Presents one accepted authoritative progress snapshot.
        /// </summary>
        void Present(
            LaunchProgressSnapshot snapshot);

        /// <summary>
        /// Presents the finalized immutable terminal report.
        /// </summary>
        void PresentTerminal(
            LaunchReport report);

        /// <summary>
        /// Releases startup-only presentation resources and subscriptions.
        /// </summary>
        void Unbind();
    }
}

//----- ILaunchStatusPresenter.cs END -----
