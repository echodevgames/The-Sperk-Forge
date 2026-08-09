//----- EchoLaunchSetupSplashAuthoringRequest.cs START -----

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupSplashEntryRequest :
        IEquatable<EchoLaunchSetupSplashEntryRequest>
    {
        internal EchoLaunchSetupSplashEntryRequest(
            string imagePath,
            string audioClipPath,
            string displayLabel,
            double fadeInSeconds,
            double holdSeconds,
            double fadeOutSeconds,
            double minimumDisplaySeconds,
            SplashMotionStyle motionStyle,
            double pulseMaximumScale,
            double pulseCycleSeconds,
            SplashSkipPolicy advancePolicy)
        {
            ImagePath = NormalizeOptional(imagePath);
            AudioClipPath = NormalizeOptional(audioClipPath);
            DisplayLabel =
                string.IsNullOrWhiteSpace(displayLabel)
                    ? string.Empty
                    : displayLabel.Trim();

            FadeInSeconds = fadeInSeconds;
            HoldSeconds = holdSeconds;
            FadeOutSeconds = fadeOutSeconds;
            MinimumDisplaySeconds = minimumDisplaySeconds;
            MotionStyle = motionStyle;
            PulseMaximumScale = pulseMaximumScale;
            PulseCycleSeconds = pulseCycleSeconds;
            AdvancePolicy = advancePolicy;
        }

        internal string ImagePath { get; }
        internal string AudioClipPath { get; }
        internal string DisplayLabel { get; }
        internal double FadeInSeconds { get; }
        internal double HoldSeconds { get; }
        internal double FadeOutSeconds { get; }
        internal double MinimumDisplaySeconds { get; }
        internal SplashMotionStyle MotionStyle { get; }
        internal double PulseMaximumScale { get; }
        internal double PulseCycleSeconds { get; }
        internal SplashSkipPolicy AdvancePolicy { get; }

        internal bool TryValidate(
            out string message)
        {
            if (string.IsNullOrEmpty(ImagePath))
            {
                message =
                    "Every authored splash entry requires an Image.";

                return false;
            }

            if (!IsProjectAssetPath(ImagePath))
            {
                message =
                    "Splash Image must be a project asset under Assets/.";

                return false;
            }

            if (!string.IsNullOrEmpty(AudioClipPath) &&
                !IsProjectAssetPath(AudioClipPath))
            {
                message =
                    "Splash Audio Intent must be a project asset under Assets/.";

                return false;
            }

            if (!IsSerializableNonnegative(FadeInSeconds) ||
                !IsSerializableNonnegative(HoldSeconds) ||
                !IsSerializableNonnegative(FadeOutSeconds) ||
                !IsSerializableNonnegative(MinimumDisplaySeconds))
            {
                message =
                    "Splash timing values must be finite and nonnegative.";

                return false;
            }

            if (!Enum.IsDefined(
                    typeof(SplashMotionStyle),
                    MotionStyle))
            {
                message =
                    "Splash Motion contains an unsupported value.";

                return false;
            }

            if (!Enum.IsDefined(
                    typeof(SplashSkipPolicy),
                    AdvancePolicy))
            {
                message =
                    "Splash Advance contains an unsupported value.";

                return false;
            }

            if (MotionStyle ==
                    SplashMotionStyle.Pulse &&
                (
                    !IsFinite(PulseMaximumScale) ||
                    PulseMaximumScale < 1d ||
                    PulseMaximumScale > float.MaxValue ||
                    !IsFinite(PulseCycleSeconds) ||
                    PulseCycleSeconds <= 0d ||
                    PulseCycleSeconds > float.MaxValue
                ))
            {
                message =
                    "Pulse requires Maximum Scale >= 1 and a positive finite Cycle Seconds value.";

                return false;
            }

            message = string.Empty;
            return true;
        }

        internal string FingerprintValue
        {
            get
            {
                StringBuilder builder =
                    new StringBuilder();

                Append(builder, "image", ImagePath);
                Append(builder, "audio", AudioClipPath);
                Append(builder, "label", DisplayLabel);
                Append(builder, "fadeIn", Format(FadeInSeconds));
                Append(builder, "hold", Format(HoldSeconds));
                Append(builder, "fadeOut", Format(FadeOutSeconds));
                Append(builder, "minimum", Format(MinimumDisplaySeconds));
                Append(builder, "motion", ((int)MotionStyle).ToString(
                    CultureInfo.InvariantCulture));
                Append(builder, "pulseScale", Format(PulseMaximumScale));
                Append(builder, "pulseCycle", Format(PulseCycleSeconds));
                Append(builder, "advance", ((int)AdvancePolicy).ToString(
                    CultureInfo.InvariantCulture));

                return builder.ToString();
            }
        }

        public bool Equals(
            EchoLaunchSetupSplashEntryRequest other)
        {
            return
                other != null &&
                string.Equals(
                    ImagePath,
                    other.ImagePath,
                    StringComparison.Ordinal) &&
                string.Equals(
                    AudioClipPath,
                    other.AudioClipPath,
                    StringComparison.Ordinal) &&
                string.Equals(
                    DisplayLabel,
                    other.DisplayLabel,
                    StringComparison.Ordinal) &&
                FadeInSeconds.Equals(other.FadeInSeconds) &&
                HoldSeconds.Equals(other.HoldSeconds) &&
                FadeOutSeconds.Equals(other.FadeOutSeconds) &&
                MinimumDisplaySeconds.Equals(
                    other.MinimumDisplaySeconds) &&
                MotionStyle == other.MotionStyle &&
                PulseMaximumScale.Equals(
                    other.PulseMaximumScale) &&
                PulseCycleSeconds.Equals(
                    other.PulseCycleSeconds) &&
                AdvancePolicy == other.AdvancePolicy;
        }

        public override bool Equals(object obj)
        {
            return Equals(
                obj as EchoLaunchSetupSplashEntryRequest);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = ImagePath.GetHashCode();
                hash = (hash * 397) ^ AudioClipPath.GetHashCode();
                hash = (hash * 397) ^ DisplayLabel.GetHashCode();
                hash = (hash * 397) ^ MotionStyle.GetHashCode();
                hash = (hash * 397) ^ AdvancePolicy.GetHashCode();

                return hash;
            }
        }

        private static string NormalizeOptional(
            string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : EchoLaunchSetupPathUtility
                    .NormalizeSeparators(value.Trim());
        }

        private static bool IsProjectAssetPath(
            string path)
        {
            return
                !string.IsNullOrEmpty(path) &&
                (
                    path.StartsWith(
                        "Assets/",
                        StringComparison.Ordinal) ||
                    string.Equals(
                        path,
                        "Assets",
                        StringComparison.Ordinal)
                ) &&
                path.IndexOf(
                    "..",
                    StringComparison.Ordinal) < 0;
        }

        private static bool IsSerializableNonnegative(
            double value)
        {
            return
                IsFinite(value) &&
                value >= 0d &&
                value <= float.MaxValue;
        }

        private static bool IsFinite(
            double value)
        {
            return
                !double.IsNaN(value) &&
                !double.IsInfinity(value);
        }

        private static string Format(
            double value)
        {
            return value.ToString(
                "R",
                CultureInfo.InvariantCulture);
        }

        private static void Append(
            StringBuilder builder,
            string key,
            string value)
        {
            string safeValue =
                value ?? string.Empty;

            builder.Append(key);
            builder.Append(':');
            builder.Append(safeValue.Length);
            builder.Append(':');
            builder.Append(safeValue);
            builder.Append('\n');
        }
    }

    internal sealed class EchoLaunchSetupSplashAuthoringRequest :
        IEquatable<EchoLaunchSetupSplashAuthoringRequest>
    {
        private readonly ReadOnlyCollection<
            EchoLaunchSetupSplashEntryRequest> entries;

        internal EchoLaunchSetupSplashAuthoringRequest(
            SplashPresentationMode presentationMode,
            Color backgroundColor,
            bool allowUserAdvance,
            IEnumerable<EchoLaunchSetupSplashEntryRequest> entries)
        {
            PresentationMode = presentationMode;
            BackgroundColor = backgroundColor;
            AllowUserAdvance = allowUserAdvance;

            this.entries =
                new ReadOnlyCollection<
                    EchoLaunchSetupSplashEntryRequest>(
                    entries == null
                        ? new List<
                            EchoLaunchSetupSplashEntryRequest>()
                        : new List<
                            EchoLaunchSetupSplashEntryRequest>(
                                entries));
        }

        internal SplashPresentationMode PresentationMode { get; }
        internal Color BackgroundColor { get; }
        internal bool AllowUserAdvance { get; }

        internal IReadOnlyList<
            EchoLaunchSetupSplashEntryRequest> Entries =>
                entries;

        internal bool TryValidate(
            out string message)
        {
            if (!Enum.IsDefined(
                    typeof(SplashPresentationMode),
                    PresentationMode))
            {
                message =
                    "Splash presentation Mode contains an unsupported value.";

                return false;
            }

            if (!IsFiniteColor(BackgroundColor))
            {
                message =
                    "Splash Background must contain finite color values.";

                return false;
            }

            for (int index = 0;
                 index < entries.Count;
                 index++)
            {
                EchoLaunchSetupSplashEntryRequest entry =
                    entries[index];

                if (entry == null)
                {
                    message =
                        $"Splash entry {index + 1} is missing.";

                    return false;
                }

                if (!entry.TryValidate(
                        out string entryMessage))
                {
                    message =
                        $"Splash entry {index + 1}: " +
                        entryMessage;

                    return false;
                }

                if (!AllowUserAdvance &&
                    entry.AdvancePolicy ==
                        SplashSkipPolicy
                            .WaitForInputAfterMinimum)
                {
                    message =
                        $"Splash entry {index + 1} waits for input, but Allow Advancement is disabled.";

                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        internal string FingerprintValue
        {
            get
            {
                StringBuilder builder =
                    new StringBuilder();

                Append(
                    builder,
                    "mode",
                    ((int)PresentationMode)
                        .ToString(
                            CultureInfo.InvariantCulture));

                Append(
                    builder,
                    "background.r",
                    Format(BackgroundColor.r));

                Append(
                    builder,
                    "background.g",
                    Format(BackgroundColor.g));

                Append(
                    builder,
                    "background.b",
                    Format(BackgroundColor.b));

                Append(
                    builder,
                    "background.a",
                    Format(BackgroundColor.a));

                Append(
                    builder,
                    "allowAdvance",
                    AllowUserAdvance ? "1" : "0");

                Append(
                    builder,
                    "entryCount",
                    entries.Count.ToString(
                        CultureInfo.InvariantCulture));

                for (int index = 0;
                     index < entries.Count;
                     index++)
                {
                    Append(
                        builder,
                        "entry." + index.ToString(
                            CultureInfo.InvariantCulture),
                        entries[index] == null
                            ? "null"
                            : entries[index]
                                .FingerprintValue);
                }

                return builder.ToString();
            }
        }

        internal string Summary
        {
            get
            {
                return
                    $"Mode={GetPresentationLabel(PresentationMode)}; " +
                    $"Background=#{ColorUtility.ToHtmlStringRGBA(BackgroundColor)}; " +
                    $"Allow Advancement={(AllowUserAdvance ? "Yes" : "No")}; " +
                    $"Entries={entries.Count}";
            }
        }

        public bool Equals(
            EchoLaunchSetupSplashAuthoringRequest other)
        {
            if (other == null ||
                PresentationMode !=
                    other.PresentationMode ||
                BackgroundColor !=
                    other.BackgroundColor ||
                AllowUserAdvance !=
                    other.AllowUserAdvance ||
                entries.Count !=
                    other.entries.Count)
            {
                return false;
            }

            for (int index = 0;
                 index < entries.Count;
                 index++)
            {
                if (!Equals(
                        entries[index],
                        other.entries[index]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(
                obj as
                    EchoLaunchSetupSplashAuthoringRequest);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash =
                    PresentationMode.GetHashCode();

                hash =
                    (hash * 397) ^
                    BackgroundColor.GetHashCode();

                hash =
                    (hash * 397) ^
                    AllowUserAdvance.GetHashCode();

                for (int index = 0;
                     index < entries.Count;
                     index++)
                {
                    hash =
                        (hash * 397) ^
                        (
                            entries[index] == null
                                ? 0
                                : entries[index]
                                    .GetHashCode()
                        );
                }

                return hash;
            }
        }

        private static string GetPresentationLabel(
            SplashPresentationMode mode)
        {
            return mode ==
                SplashPresentationMode.SplashOnly
                    ? "Splash Only"
                    : "Splash + Status";
        }

        private static bool IsFiniteColor(
            Color value)
        {
            return
                IsFinite(value.r) &&
                IsFinite(value.g) &&
                IsFinite(value.b) &&
                IsFinite(value.a);
        }

        private static bool IsFinite(
            float value)
        {
            return
                !float.IsNaN(value) &&
                !float.IsInfinity(value);
        }

        private static string Format(
            float value)
        {
            return value.ToString(
                "R",
                CultureInfo.InvariantCulture);
        }

        private static void Append(
            StringBuilder builder,
            string key,
            string value)
        {
            string safeValue =
                value ?? string.Empty;

            builder.Append(key);
            builder.Append(':');
            builder.Append(safeValue.Length);
            builder.Append(':');
            builder.Append(safeValue);
            builder.Append('\n');
        }
    }
}

//----- EchoLaunchSetupSplashAuthoringRequest.cs END -----
