//----- StartupStepProgress.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable progress value reported by one active startup-step
    /// executor.
    /// </summary>
    public readonly struct StartupStepProgress
    {
        private readonly float progress01;
        private readonly bool isIndeterminate;
        private readonly string message;

        private StartupStepProgress(
            float progress01,
            bool isIndeterminate,
            string message)
        {
            this.progress01 = progress01;
            this.isIndeterminate = isIndeterminate;
            this.message = NormalizeMessage(message);
        }

        /// <summary>
        /// Gets the normalized progress value.
        ///
        /// For indeterminate progress this value is zero and must not be
        /// interpreted as a percentage.
        /// </summary>
        public float Progress01 =>
            progress01;

        /// <summary>
        /// Gets whether the executor cannot currently report a meaningful
        /// numeric completion value.
        /// </summary>
        public bool IsIndeterminate =>
            isIndeterminate;

        /// <summary>
        /// Gets the normalized human-readable progress message.
        /// </summary>
        public string Message =>
            message ?? string.Empty;

        /// <summary>
        /// Creates determinate progress in the inclusive range zero
        /// through one.
        /// </summary>
        public static StartupStepProgress Determinate(
            float progress01,
            string message = null)
        {
            if (float.IsNaN(progress01) ||
                float.IsInfinity(progress01) ||
                progress01 < 0f ||
                progress01 > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(progress01),
                    progress01,
                    "Determinate startup-step progress must be finite and within the inclusive range zero through one.");
            }

            return new StartupStepProgress(
                progress01,
                false,
                message);
        }

        /// <summary>
        /// Creates indeterminate progress without inventing a numeric
        /// completion percentage.
        /// </summary>
        public static StartupStepProgress Indeterminate(
            string message = null)
        {
            return new StartupStepProgress(
                0f,
                true,
                message);
        }

        private static string NormalizeMessage(
            string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }
    }
}

//----- StartupStepProgress.cs END -----
