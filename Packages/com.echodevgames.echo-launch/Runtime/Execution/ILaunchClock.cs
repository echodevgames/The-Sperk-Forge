//----- ILaunchClock.cs START -----

using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Supplies monotonic unscaled launch time and a non-blocking tick seam.
    ///
    /// Runtime code uses the default Unity implementation. Tests and custom
    /// composition may provide a deterministic clock without waiting for
    /// wall-clock time.
    /// </summary>
    public interface ILaunchClock
    {
        /// <summary>
        /// Gets the current monotonic unscaled time in seconds.
        /// </summary>
        double NowSeconds
        {
            get;
        }

        /// <summary>
        /// Yields until the clock's next observable tick without blocking
        /// the Unity player loop.
        /// </summary>
        Awaitable NextTickAsync(
            CancellationToken cancellationToken);
    }
}

//----- ILaunchClock.cs END -----
