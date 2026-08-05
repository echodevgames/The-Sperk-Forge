//----- IImageSplashPresenter.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Receives deterministic image-splash frames without owning launch
    /// truth, timing, input bindings, or sequence traversal.
    /// </summary>
    public interface IImageSplashPresenter
    {
        /// <summary>
        /// Raised when presentation receives a project-routed skip request.
        /// </summary>
        event Action SkipRequested;

        /// <summary>
        /// Presents one accepted immutable splash frame.
        /// </summary>
        void PresentSplash(
            SplashPresentationFrame frame);

        /// <summary>
        /// Clears startup-only splash presentation.
        /// </summary>
        void ClearSplash();
    }
}

//----- IImageSplashPresenter.cs END -----
