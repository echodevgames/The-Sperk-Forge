//----- SplashPresentationFrame.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Carries one immutable deterministic image-splash presentation frame.
    /// </summary>
    public sealed class SplashPresentationFrame
    {
        internal SplashPresentationFrame(
            string sequenceId,
            SplashEntry entry,
            int entryIndex,
            int entryCount,
            SplashPlaybackPhase phase,
            float alpha,
            double elapsedSeconds,
            double minimumDisplaySeconds,
            bool canSkipNow,
            bool reducedMotion)
            : this(
                sequenceId,
                entry,
                entryIndex,
                entryCount,
                phase,
                alpha,
                elapsedSeconds,
                minimumDisplaySeconds,
                SplashPresentationSettings.LegacyDefaults,
                1f,
                canSkipNow,
                canSkipNow,
                reducedMotion)
        {
        }

        internal SplashPresentationFrame(
            string sequenceId,
            SplashEntry entry,
            int entryIndex,
            int entryCount,
            SplashPlaybackPhase phase,
            float alpha,
            double elapsedSeconds,
            double minimumDisplaySeconds,
            SplashPresentationSettings presentationSettings,
            float imageScale,
            bool canAdvanceNow,
            bool canSkipNow,
            bool reducedMotion)
        {
            if (string.IsNullOrWhiteSpace(
                    sequenceId))
            {
                throw new ArgumentException(
                    "A splash frame requires sequence identity.",
                    nameof(sequenceId));
            }

            if (entry == null)
            {
                throw new ArgumentNullException(
                    nameof(entry));
            }

            if (entryIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(entryIndex));
            }

            if (entryCount <= 0 ||
                entryIndex >= entryCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(entryCount));
            }

            if (!Enum.IsDefined(
                    typeof(SplashPlaybackPhase),
                    phase) ||
                phase ==
                    SplashPlaybackPhase.None)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(phase));
            }

            if (float.IsNaN(alpha) ||
                float.IsInfinity(alpha) ||
                alpha < 0f ||
                alpha > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(alpha));
            }

            ValidateSeconds(
                elapsedSeconds,
                nameof(elapsedSeconds));

            ValidateSeconds(
                minimumDisplaySeconds,
                nameof(minimumDisplaySeconds));

            if (presentationSettings == null)
            {
                throw new ArgumentNullException(
                    nameof(presentationSettings));
            }

            if (!presentationSettings.HasValidDefinition)
            {
                throw new ArgumentException(
                    "Splash presentation settings are invalid.",
                    nameof(presentationSettings));
            }

            if (float.IsNaN(imageScale) ||
                float.IsInfinity(imageScale) ||
                imageScale < 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(imageScale));
            }

            SequenceId =
                sequenceId.Trim();

            EntryId =
                entry.EntryId;

            Image =
                entry.Image;

            DisplayLabel =
                entry.DisplayLabel;

            EntryIndex =
                entryIndex;

            EntryCount =
                entryCount;

            Phase =
                phase;

            Alpha =
                alpha;

            ElapsedSeconds =
                elapsedSeconds;

            MinimumDisplaySeconds =
                minimumDisplaySeconds;

            PresentationMode =
                presentationSettings
                    .PresentationMode;

            BackgroundColor =
                presentationSettings
                    .BackgroundColor;

            AllowUserAdvance =
                presentationSettings
                    .AllowUserAdvance;

            AdvancePolicy =
                entry.SkipPolicy;

            ImageScale =
                imageScale;

            CanAdvanceNow =
                canAdvanceNow;

            CanSkipNow =
                canSkipNow;

            ReducedMotion =
                reducedMotion;
        }

        public string SequenceId
        {
            get;
        }

        public string EntryId
        {
            get;
        }

        public Sprite Image
        {
            get;
        }

        public string DisplayLabel
        {
            get;
        }

        public int EntryIndex
        {
            get;
        }

        public int EntryCount
        {
            get;
        }

        public SplashPlaybackPhase Phase
        {
            get;
        }

        public float Alpha
        {
            get;
        }

        public double ElapsedSeconds
        {
            get;
        }

        public double MinimumDisplaySeconds
        {
            get;
        }

        public SplashPresentationMode PresentationMode
        {
            get;
        }

        public Color BackgroundColor
        {
            get;
        }

        public bool AllowUserAdvance
        {
            get;
        }

        public SplashSkipPolicy AdvancePolicy
        {
            get;
        }

        public float ImageScale
        {
            get;
        }

        public bool CanAdvanceNow
        {
            get;
        }

        public bool CanSkipNow
        {
            get;
        }

        public bool ReducedMotion
        {
            get;
        }

        private static void ValidateSeconds(
            double value,
            string parameterName)
        {
            if (double.IsNaN(value) ||
                double.IsInfinity(value) ||
                value < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName);
            }
        }
    }
}

//----- SplashPresentationFrame.cs END -----
