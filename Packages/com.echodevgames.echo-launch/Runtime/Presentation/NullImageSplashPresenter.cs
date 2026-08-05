//----- NullImageSplashPresenter.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Logging-free headless image-splash presenter.
    /// </summary>
    internal sealed class NullImageSplashPresenter :
        IImageSplashPresenter
    {
        internal static NullImageSplashPresenter Shared
        {
            get;
        } =
            new NullImageSplashPresenter();

        private NullImageSplashPresenter()
        {
        }

        public event Action SkipRequested
        {
            add
            {
            }

            remove
            {
            }
        }

        public void PresentSplash(
            SplashPresentationFrame frame)
        {
        }

        public void ClearSplash()
        {
        }
    }
}

//----- NullImageSplashPresenter.cs END -----
