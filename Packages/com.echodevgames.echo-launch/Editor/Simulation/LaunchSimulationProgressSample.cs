using System;
using System.Globalization;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal readonly struct LaunchSimulationProgressSample
    {
        internal LaunchSimulationProgressSample(
            int authoredStepIndex,
            int sampleIndex,
            float progress01,
            bool isIndeterminate,
            string message,
            double logicalSeconds)
        {
            if (authoredStepIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authoredStepIndex));
            }

            if (sampleIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sampleIndex));
            }

            if (float.IsNaN(progress01) ||
                float.IsInfinity(progress01) ||
                progress01 < 0f ||
                progress01 > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(progress01));
            }

            if (double.IsNaN(logicalSeconds) ||
                double.IsInfinity(logicalSeconds) ||
                logicalSeconds < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(logicalSeconds));
            }

            AuthoredStepIndex = authoredStepIndex;
            SampleIndex = sampleIndex;
            Progress01 = progress01;
            IsIndeterminate = isIndeterminate;
            Message = string.IsNullOrWhiteSpace(message)
                ? string.Empty
                : message.Trim();
            LogicalSeconds = logicalSeconds;
        }

        internal int AuthoredStepIndex { get; }
        internal int SampleIndex { get; }
        internal float Progress01 { get; }
        internal bool IsIndeterminate { get; }
        internal string Message { get; }
        internal double LogicalSeconds { get; }

        internal string ToCanonicalText()
        {
            return string.Join(
                "|",
                AuthoredStepIndex.ToString(
                    CultureInfo.InvariantCulture),
                SampleIndex.ToString(
                    CultureInfo.InvariantCulture),
                Progress01.ToString(
                    "R",
                    CultureInfo.InvariantCulture),
                IsIndeterminate.ToString(),
                Message,
                LogicalSeconds.ToString(
                    "R",
                    CultureInfo.InvariantCulture));
        }
    }
}
