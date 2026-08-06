using System;
using System.Globalization;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationRequest
    {
        internal const int CurrentSchemaVersion = 1;
        internal const int MaximumMessageLength = 256;
        internal const double MaximumSeconds = 60d;
        internal const int MaximumProgressSamples = 120;

        internal LaunchSimulationRequest(
            LaunchSimulationPreset preset,
            double logicalDurationSeconds,
            int progressSampleCount,
            double timeoutSeconds,
            string message)
        {
            SchemaVersion = CurrentSchemaVersion;
            Preset = preset;
            LogicalDurationSeconds = logicalDurationSeconds;
            ProgressSampleCount = progressSampleCount;
            TimeoutSeconds = timeoutSeconds;
            Message = NormalizeMessage(message);
            RequestFingerprint =
                LaunchSimulationFingerprint.ComputeRequest(this);
        }

        internal int SchemaVersion { get; }

        internal LaunchSimulationPreset Preset { get; }

        internal double LogicalDurationSeconds { get; }

        internal int ProgressSampleCount { get; }

        internal double TimeoutSeconds { get; }

        internal string Message { get; }

        internal string RequestFingerprint { get; }

        internal bool TryValidate(
            out string validationMessage)
        {
            if (SchemaVersion != CurrentSchemaVersion)
            {
                validationMessage =
                    "The Launch Simulator request schema is unsupported.";
                return false;
            }

            if (!LaunchSimulationPresetUtility.IsDefined(Preset))
            {
                validationMessage =
                    "The selected Launch Simulator preset is unsupported.";
                return false;
            }

            if (!IsFiniteInRange(
                    LogicalDurationSeconds,
                    0d,
                    MaximumSeconds))
            {
                validationMessage =
                    "Logical duration must be finite and between 0 and 60 seconds.";
                return false;
            }

            if (ProgressSampleCount < 0 ||
                ProgressSampleCount > MaximumProgressSamples)
            {
                validationMessage =
                    "Progress sample count must be between 0 and 120.";
                return false;
            }

            if (!IsFiniteInRange(
                    TimeoutSeconds,
                    0d,
                    MaximumSeconds))
            {
                validationMessage =
                    "Timeout must be finite and between 0 and 60 seconds.";
                return false;
            }

            if (Message.Length > MaximumMessageLength)
            {
                validationMessage =
                    "The optional simulation message must not exceed 256 characters.";
                return false;
            }

            if (Preset ==
                    LaunchSimulationPreset.TimedProgressSuccess &&
                (LogicalDurationSeconds <= 0d ||
                 ProgressSampleCount <= 0))
            {
                validationMessage =
                    "Timed Progress Success requires a positive logical duration and at least one progress sample.";
                return false;
            }

            if (Preset ==
                    LaunchSimulationPreset.TimeoutStops &&
                TimeoutSeconds <= 0d)
            {
                validationMessage =
                    "Timeout Stops requires a positive timeout.";
                return false;
            }

            validationMessage = string.Empty;
            return true;
        }

        internal string ToCanonicalText()
        {
            return string.Join(
                "\n",
                "Schema=" +
                SchemaVersion.ToString(
                    CultureInfo.InvariantCulture),
                "Preset=" +
                ((int)Preset).ToString(
                    CultureInfo.InvariantCulture),
                "Duration=" +
                LogicalDurationSeconds.ToString(
                    "R",
                    CultureInfo.InvariantCulture),
                "Samples=" +
                ProgressSampleCount.ToString(
                    CultureInfo.InvariantCulture),
                "Timeout=" +
                TimeoutSeconds.ToString(
                    "R",
                    CultureInfo.InvariantCulture),
                "Message=" + Message);
        }

        private static bool IsFiniteInRange(
            double value,
            double minimum,
            double maximum)
        {
            return !double.IsNaN(value) &&
                   !double.IsInfinity(value) &&
                   value >= minimum &&
                   value <= maximum;
        }

        private static string NormalizeMessage(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Replace("\r\n", "\n")
                        .Replace('\r', '\n')
                        .Trim();
        }
    }
}
