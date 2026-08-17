using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Replaceable monotonic unscaled time seam for notification lifetime.
    /// The clock owns no notification or game-time authority.
    /// </summary>
    public interface IUINotificationClock
    {
        double NowSeconds { get; }
    }

    internal sealed class UnityUINotificationClock :
        IUINotificationClock
    {
        internal static UnityUINotificationClock Shared
        {
            get;
        } =
            new UnityUINotificationClock();

        private UnityUINotificationClock()
        {
        }

        public double NowSeconds =>
            Time.realtimeSinceStartupAsDouble;
    }
}
