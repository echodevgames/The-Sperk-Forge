//----- StartupStepTiming.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores one immutable runtime timing snapshot for a completed
    /// startup-step attempt.
    /// </summary>
    internal readonly struct StartupStepTiming
    {
        /// <summary>
        /// Creates one validated timing snapshot.
        /// </summary>
        internal StartupStepTiming(
            double startSeconds,
            double settlementSeconds,
            double timeoutSeconds,
            bool timedOut,
            bool cancellationRequested)
        {
            ValidateFiniteNonnegative(
                startSeconds,
                nameof(startSeconds));

            ValidateFiniteNonnegative(
                settlementSeconds,
                nameof(settlementSeconds));

            ValidateFiniteNonnegative(
                timeoutSeconds,
                nameof(timeoutSeconds));

            if (settlementSeconds < startSeconds)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(settlementSeconds),
                    settlementSeconds,
                    "Startup-step settlement time must not be earlier than its start time.");
            }

            bool hasTimeout =
                timeoutSeconds > 0d;

            if (timedOut &&
                !hasTimeout)
            {
                throw new ArgumentException(
                    "A startup-step attempt cannot be marked timed out when no positive timeout was configured.",
                    nameof(timedOut));
            }

            if (cancellationRequested &&
                !timedOut)
            {
                throw new ArgumentException(
                    "Timeout cancellation cannot be marked requested when the attempt did not time out.",
                    nameof(cancellationRequested));
            }

            StartSeconds = startSeconds;
            SettlementSeconds = settlementSeconds;
            TimeoutSeconds = timeoutSeconds;
            TimedOut = timedOut;
            CancellationRequested =
                cancellationRequested;
        }

        /// <summary>
        /// Gets a zero-duration snapshot used when no executor timing was
        /// measured.
        /// </summary>
        internal static StartupStepTiming NotMeasured =>
            new StartupStepTiming(
                0d,
                0d,
                0d,
                false,
                false);

        /// <summary>
        /// Gets the monotonic unscaled attempt start time.
        /// </summary>
        internal double StartSeconds
        {
            get;
        }

        /// <summary>
        /// Gets the monotonic unscaled time when the executor settled or a
        /// pre-start failure was captured.
        /// </summary>
        internal double SettlementSeconds
        {
            get;
        }

        /// <summary>
        /// Gets the measured attempt duration.
        /// </summary>
        internal double ElapsedSeconds =>
            SettlementSeconds -
            StartSeconds;

        /// <summary>
        /// Gets the configured timeout duration.
        ///
        /// Zero means timeout was disabled.
        /// </summary>
        internal double TimeoutSeconds
        {
            get;
        }

        /// <summary>
        /// Gets whether a positive timeout was configured.
        /// </summary>
        internal bool HasTimeout =>
            TimeoutSeconds > 0d;

        /// <summary>
        /// Gets whether the deadline won before executor settlement.
        /// </summary>
        internal bool TimedOut
        {
            get;
        }

        /// <summary>
        /// Gets whether timeout handling requested cooperative
        /// cancellation.
        /// </summary>
        internal bool CancellationRequested
        {
            get;
        }

        private static void ValidateFiniteNonnegative(
            double value,
            string parameterName)
        {
            if (double.IsNaN(value) ||
                double.IsInfinity(value) ||
                value < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Startup-step timing values must be finite and nonnegative.");
            }
        }
    }
}

//----- StartupStepTiming.cs END -----
