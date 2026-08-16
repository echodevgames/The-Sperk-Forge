using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public sealed class UITransitionResolvedPolicy
    {
        internal UITransitionResolvedPolicy(
            string profileId,
            string driverId,
            float durationSeconds,
            AnimationCurve curve,
            float hardTimeoutSeconds,
            bool reducedMotionApplied)
        {
            ProfileId = profileId ?? string.Empty;
            DriverId = driverId ?? string.Empty;
            DurationSeconds = durationSeconds < 0f ? 0f : durationSeconds;
            Curve = UITransitionProfile.CloneCurve(curve) ?? AnimationCurve.Linear(0f, 0f, 1f, 1f);
            HardTimeoutSeconds = hardTimeoutSeconds <= 0f ? 5f : hardTimeoutSeconds;
            ReducedMotionApplied = reducedMotionApplied;
        }

        public string ProfileId { get; }
        public string DriverId { get; }
        public float DurationSeconds { get; }
        public AnimationCurve Curve { get; }
        public float HardTimeoutSeconds { get; }
        public bool ReducedMotionApplied { get; }
    }
}
