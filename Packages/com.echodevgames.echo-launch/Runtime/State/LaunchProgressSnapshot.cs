//----- LaunchProgressSnapshot.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable observation of launch progress at one moment in time.
    /// </summary>
    public readonly struct LaunchProgressSnapshot
    {
        public LaunchMode Mode { get; }

        public LaunchStatus Status { get; }

        public string ActiveStepId { get; }

        public int ActiveStepIndex { get; }

        public int TotalStepCount { get; }

        public float Progress01 { get; }

        public bool IsProgressIndeterminate { get; }

        public string Message { get; }

        public double ElapsedSeconds { get; }

        public StartupStepResult LastResult { get; }

        public LaunchProgressSnapshot(
            LaunchMode mode,
            LaunchStatus status,
            string activeStepId,
            int activeStepIndex,
            int totalStepCount,
            float progress01,
            bool isProgressIndeterminate,
            string message,
            double elapsedSeconds,
            StartupStepResult lastResult)
        {
            ValidateStepCounts(
                activeStepIndex,
                totalStepCount);

            ValidateProgress(progress01);
            ValidateElapsedTime(elapsedSeconds);

            Mode = mode;
            Status = status;
            ActiveStepId = activeStepId ?? string.Empty;
            ActiveStepIndex = activeStepIndex;
            TotalStepCount = totalStepCount;
            Progress01 = progress01;
            IsProgressIndeterminate = isProgressIndeterminate;
            Message = message ?? string.Empty;
            ElapsedSeconds = elapsedSeconds;
            LastResult = lastResult;
        }

        private static void ValidateStepCounts(
            int activeStepIndex,
            int totalStepCount)
        {
            if (totalStepCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(totalStepCount),
                    totalStepCount,
                    "The total step count cannot be negative.");
            }

            if (activeStepIndex < -1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(activeStepIndex),
                    activeStepIndex,
                    "The active step index cannot be less than -1.");
            }

            if (activeStepIndex >= totalStepCount &&
                activeStepIndex != -1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(activeStepIndex),
                    activeStepIndex,
                    "The active step index must fall within the total step count.");
            }
        }

        private static void ValidateProgress(
            float progress01)
        {
            if (float.IsNaN(progress01) ||
                float.IsInfinity(progress01) ||
                progress01 < 0f ||
                progress01 > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(progress01),
                    progress01,
                    "Progress must be a finite value between 0 and 1.");
            }
        }

        private static void ValidateElapsedTime(
            double elapsedSeconds)
        {
            if (double.IsNaN(elapsedSeconds) ||
                double.IsInfinity(elapsedSeconds) ||
                elapsedSeconds < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedSeconds),
                    elapsedSeconds,
                    "Elapsed time must be a finite, nonnegative value.");
            }
        }
    }
}

//----- LaunchProgressSnapshot.cs END -----
