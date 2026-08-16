using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Authored transition policy layer. Empty/sentinel values inherit from a lower layer.
    /// Runtime overrides are snapshotted and never write back into authored data.
    /// </summary>
    [Serializable]
    public sealed class UITransitionProfile
    {
        [SerializeField] private string profileId = string.Empty;
        [SerializeField] private string enterDriverId = string.Empty;
        [SerializeField] private string exitDriverId = string.Empty;
        [SerializeField] private float enterDurationSeconds = -1f;
        [SerializeField] private float exitDurationSeconds = -1f;
        [SerializeField] private AnimationCurve enterCurve;
        [SerializeField] private AnimationCurve exitCurve;
        [SerializeField] private float hardTimeoutSeconds = -1f;
        [SerializeField] private UITransitionReducedMotionMode reducedMotionMode = UITransitionReducedMotionMode.Inherit;
        [SerializeField] private string reducedMotionDriverId = string.Empty;

        public UITransitionProfile()
        {
        }

        public UITransitionProfile(
            string profileId,
            string enterDriverId,
            string exitDriverId,
            float enterDurationSeconds = -1f,
            float exitDurationSeconds = -1f,
            AnimationCurve enterCurve = null,
            AnimationCurve exitCurve = null,
            float hardTimeoutSeconds = -1f,
            UITransitionReducedMotionMode reducedMotionMode = UITransitionReducedMotionMode.Inherit,
            string reducedMotionDriverId = "")
        {
            this.profileId = Normalize(profileId);
            this.enterDriverId = Normalize(enterDriverId);
            this.exitDriverId = Normalize(exitDriverId);
            this.enterDurationSeconds = enterDurationSeconds;
            this.exitDurationSeconds = exitDurationSeconds;
            this.enterCurve = CloneCurve(enterCurve);
            this.exitCurve = CloneCurve(exitCurve);
            this.hardTimeoutSeconds = hardTimeoutSeconds;
            this.reducedMotionMode = reducedMotionMode;
            this.reducedMotionDriverId = Normalize(reducedMotionDriverId);
        }

        public string ProfileId => Normalize(profileId);
        public string EnterDriverId => Normalize(enterDriverId);
        public string ExitDriverId => Normalize(exitDriverId);
        public float EnterDurationSeconds => enterDurationSeconds;
        public float ExitDurationSeconds => exitDurationSeconds;
        public AnimationCurve EnterCurve => CloneCurve(enterCurve);
        public AnimationCurve ExitCurve => CloneCurve(exitCurve);
        public float HardTimeoutSeconds => hardTimeoutSeconds;
        public UITransitionReducedMotionMode ReducedMotionMode => reducedMotionMode;
        public string ReducedMotionDriverId => Normalize(reducedMotionDriverId);

        public static UITransitionProfile CreateDefault() =>
            new UITransitionProfile(
                "default",
                UITransitionDriverIds.Immediate,
                UITransitionDriverIds.Immediate,
                0f,
                0f,
                AnimationCurve.Linear(0f, 0f, 1f, 1f),
                AnimationCurve.Linear(0f, 0f, 1f, 1f),
                5f,
                UITransitionReducedMotionMode.UseReplacement,
                UITransitionDriverIds.Immediate);

        internal UITransitionProfile Snapshot() =>
            new UITransitionProfile(
                ProfileId,
                EnterDriverId,
                ExitDriverId,
                EnterDurationSeconds,
                ExitDurationSeconds,
                enterCurve,
                exitCurve,
                HardTimeoutSeconds,
                ReducedMotionMode,
                ReducedMotionDriverId);

        internal static AnimationCurve CloneCurve(AnimationCurve source)
        {
            if (source == null)
            {
                return null;
            }

            AnimationCurve clone = new AnimationCurve(source.keys)
            {
                preWrapMode = source.preWrapMode,
                postWrapMode = source.postWrapMode
            };
            return clone;
        }

        private static string Normalize(string value) => value == null ? string.Empty : value.Trim();
    }
}
