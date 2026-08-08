//----- SplashEntry.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores immutable authored data for one image startup splash,
    /// optional preferred audio-content intent, and bounded presentation
    /// motion metadata.
    ///
    /// The definition owns no playback index, elapsed time, alpha, skip
    /// request, cancellation state, or presenter state.
    /// </summary>
    [Serializable]
    public sealed class SplashEntry
    {
        private const int CanonicalIdLength = 32;

        [SerializeField]
        [HideInInspector]
        private string entryId =
            Guid.NewGuid().ToString("N");

        [SerializeField]
        private Sprite image;

        [SerializeField]
        private AudioClip preferredAudioClip;

        [SerializeField]
        private string displayLabel =
            string.Empty;

        [SerializeField]
        [Min(0f)]
        private float fadeInSeconds =
            0.25f;

        [SerializeField]
        [Min(0f)]
        private float holdSeconds =
            1f;

        [SerializeField]
        [Min(0f)]
        private float fadeOutSeconds =
            0.25f;

        [SerializeField]
        [Min(0f)]
        private float minimumDisplaySeconds;

        [SerializeField]
        private SplashSkipPolicy skipPolicy =
            SplashSkipPolicy
                .AfterMinimumDisplay;

        [SerializeField]
        private SplashMotionStyle motionStyle =
            SplashMotionStyle.None;

        [SerializeField]
        [Min(1f)]
        private float pulseMaximumScale =
            1.05f;

        [SerializeField]
        [Min(0.01f)]
        private float pulseCycleSeconds =
            1f;

        /// <summary>
        /// Gets the stable diagnostic identity of this entry.
        /// </summary>
        public string EntryId =>
            entryId ?? string.Empty;

        /// <summary>
        /// Gets the authored image.
        /// </summary>
        public Sprite Image =>
            image;

        /// <summary>
        /// Gets the optional project-owned audio content intended to
        /// accompany this splash. EchoLaunch does not play this clip.
        /// </summary>
        public AudioClip PreferredAudioClip =>
            preferredAudioClip;

        /// <summary>
        /// Gets the replaceable user-facing label.
        /// </summary>
        public string DisplayLabel =>
            string.IsNullOrWhiteSpace(
                displayLabel)
                ? string.Empty
                : displayLabel.Trim();

        /// <summary>
        /// Gets the authored fade-in duration.
        /// </summary>
        public double FadeInSeconds =>
            fadeInSeconds;

        /// <summary>
        /// Gets the authored full-opacity hold duration.
        /// </summary>
        public double HoldSeconds =>
            holdSeconds;

        /// <summary>
        /// Gets the authored fade-out duration.
        /// </summary>
        public double FadeOutSeconds =>
            fadeOutSeconds;

        /// <summary>
        /// Gets the minimum time this entry must remain active.
        /// </summary>
        public double MinimumDisplaySeconds =>
            minimumDisplaySeconds;

        /// <summary>
        /// Gets the authored skip policy.
        /// </summary>
        public SplashSkipPolicy SkipPolicy =>
            skipPolicy;

        /// <summary>
        /// Gets the bounded authored image motion style.
        /// </summary>
        public SplashMotionStyle MotionStyle =>
            motionStyle;

        /// <summary>
        /// Gets the authored maximum Pulse image scale.
        /// </summary>
        public double PulseMaximumScale =>
            pulseMaximumScale;

        /// <summary>
        /// Gets the authored Pulse cycle duration.
        /// </summary>
        public double PulseCycleSeconds =>
            pulseCycleSeconds;

        /// <summary>
        /// Gets the authored fade/hold/fade duration before minimum-time
        /// expansion.
        /// </summary>
        public double NaturalDurationSeconds =>
            FadeInSeconds +
            HoldSeconds +
            FadeOutSeconds;

        internal bool HasValidIdentity =>
            IsCanonicalId(entryId);

        internal bool HasValidDefinition =>
            HasValidIdentity &&
            image != null &&
            IsFiniteNonnegative(
                FadeInSeconds) &&
            IsFiniteNonnegative(
                HoldSeconds) &&
            IsFiniteNonnegative(
                FadeOutSeconds) &&
            IsFiniteNonnegative(
                MinimumDisplaySeconds) &&
            Enum.IsDefined(
                typeof(SplashSkipPolicy),
                skipPolicy) &&
            Enum.IsDefined(
                typeof(SplashMotionStyle),
                motionStyle) &&
            HasValidMotionDefinition;

        internal SplashEntry(
            string authoredEntryId,
            Sprite authoredImage,
            string authoredDisplayLabel,
            double authoredFadeInSeconds,
            double authoredHoldSeconds,
            double authoredFadeOutSeconds,
            double authoredMinimumDisplaySeconds,
            SplashSkipPolicy authoredSkipPolicy,
            AudioClip authoredPreferredAudioClip = null,
            SplashMotionStyle authoredMotionStyle =
                SplashMotionStyle.None,
            double authoredPulseMaximumScale = 1.05d,
            double authoredPulseCycleSeconds = 1d)
        {
            entryId =
                authoredEntryId;

            image =
                authoredImage;

            displayLabel =
                authoredDisplayLabel;

            fadeInSeconds =
                ToSerializedSeconds(
                    authoredFadeInSeconds,
                    nameof(
                        authoredFadeInSeconds));

            holdSeconds =
                ToSerializedSeconds(
                    authoredHoldSeconds,
                    nameof(
                        authoredHoldSeconds));

            fadeOutSeconds =
                ToSerializedSeconds(
                    authoredFadeOutSeconds,
                    nameof(
                        authoredFadeOutSeconds));

            minimumDisplaySeconds =
                ToSerializedSeconds(
                    authoredMinimumDisplaySeconds,
                    nameof(
                        authoredMinimumDisplaySeconds));

            if (!Enum.IsDefined(
                    typeof(SplashSkipPolicy),
                    authoredSkipPolicy))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authoredSkipPolicy));
            }

            skipPolicy =
                authoredSkipPolicy;

            preferredAudioClip =
                authoredPreferredAudioClip;

            if (!Enum.IsDefined(
                    typeof(SplashMotionStyle),
                    authoredMotionStyle))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authoredMotionStyle));
            }

            motionStyle =
                authoredMotionStyle;

            pulseMaximumScale =
                ToSerializedPulseMaximumScale(
                    authoredPulseMaximumScale);

            pulseCycleSeconds =
                ToSerializedPulseCycleSeconds(
                    authoredPulseCycleSeconds);
        }

        public SplashEntry()
        {
        }

        private bool HasValidMotionDefinition =>
            motionStyle !=
                SplashMotionStyle.Pulse ||
            (
                IsFinite(
                    PulseMaximumScale) &&
                PulseMaximumScale >= 1d &&
                IsFinite(
                    PulseCycleSeconds) &&
                PulseCycleSeconds > 0d
            );

        private static float ToSerializedPulseMaximumScale(
            double value)
        {
            if (!IsFinite(value) ||
                value < 1d ||
                value > float.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Pulse maximum scale must be finite, at least 1, and representable as a Unity float.");
            }

            return (float)value;
        }

        private static float ToSerializedPulseCycleSeconds(
            double value)
        {
            if (!IsFinite(value) ||
                value <= 0d ||
                value > float.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Pulse cycle duration must be finite, positive, and representable as a Unity float.");
            }

            return (float)value;
        }

        private static float ToSerializedSeconds(
            double value,
            string parameterName)
        {
            if (!IsFiniteNonnegative(value) ||
                value > float.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Splash timing must be finite, nonnegative, and representable as a Unity float.");
            }

            return (float)value;
        }

        private static bool IsFiniteNonnegative(
            double value)
        {
            return
                IsFinite(value) &&
                value >= 0d;
        }

        private static bool IsFinite(
            double value)
        {
            return
                !double.IsNaN(value) &&
                !double.IsInfinity(value);
        }

        private static bool IsCanonicalId(
            string value)
        {
            if (string.IsNullOrEmpty(value) ||
                value.Length != CanonicalIdLength)
            {
                return false;
            }

            for (int index = 0;
                 index < value.Length;
                 index++)
            {
                char character =
                    value[index];

                bool isNumber =
                    character >= '0' &&
                    character <= '9';

                bool isLowercaseHexLetter =
                    character >= 'a' &&
                    character <= 'f';

                if (!isNumber &&
                    !isLowercaseHexLetter)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

//----- SplashEntry.cs END -----
