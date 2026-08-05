//----- NullLaunchStatusPresenter.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Logging-free headless fallback used when no visual presenter is assigned.
    /// </summary>
    internal sealed class NullLaunchStatusPresenter :
        ILaunchStatusPresenter
    {
        internal static NullLaunchStatusPresenter Shared
        {
            get;
        } = new NullLaunchStatusPresenter();

        private NullLaunchStatusPresenter()
        {
        }

        public void Bind(
            LaunchProgressSnapshot initialSnapshot)
        {
        }

        public void Present(
            LaunchProgressSnapshot snapshot)
        {
        }

        public void PresentTerminal(
            LaunchReport report)
        {
        }

        public void Unbind()
        {
        }
    }
}

//----- NullLaunchStatusPresenter.cs END -----
