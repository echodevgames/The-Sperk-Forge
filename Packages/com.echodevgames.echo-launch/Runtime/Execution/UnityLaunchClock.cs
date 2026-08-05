//----- UnityLaunchClock.cs START -----

using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Supplies Unity's monotonic unscaled real time and one player-loop
    /// frame as the default launch-clock tick.
    ///
    /// The clock owns no launch state and does not interpret timeout policy.
    /// </summary>
    internal sealed class UnityLaunchClock :
        ILaunchClock
    {
        /// <summary>
        /// Shared stateless runtime clock instance.
        /// </summary>
        internal static UnityLaunchClock Shared
        {
            get;
        } =
            new UnityLaunchClock();

        private UnityLaunchClock()
        {
        }

        /// <summary>
        /// Gets Unity's double-precision real time since startup.
        /// </summary>
        public double NowSeconds =>
            Time.realtimeSinceStartupAsDouble;

        /// <summary>
        /// Yields until the next Unity frame.
        /// </summary>
        public Awaitable NextTickAsync(
            CancellationToken cancellationToken)
        {
            return Awaitable.NextFrameAsync(
                cancellationToken);
        }
    }
}

//----- UnityLaunchClock.cs END -----
