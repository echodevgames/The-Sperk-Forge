//----- LaunchStepReport.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable public copy of one completed startup-step execution.
    ///
    /// The report owns copied metadata and immutable values only. It never
    /// exposes the internal mutable execution object or its executor.
    /// </summary>
    public sealed class LaunchStepReport
    {
        /// <summary>
        /// Creates one immutable report from a completed runtime execution.
        /// </summary>
        internal LaunchStepReport(
            StartupStepExecution execution)
        {
            if (execution == null)
            {
                throw new ArgumentNullException(
                    nameof(execution));
            }

            if (!execution.IsComplete ||
                execution.Result == null ||
                !execution.HasTiming)
            {
                throw new ArgumentException(
                    "A launch step report requires one completed execution with a terminal result and timing snapshot.",
                    nameof(execution));
            }

            StartupStepTiming timing =
                execution.Timing;

            EntryId = RequireText(
                execution.EntryId,
                nameof(execution.EntryId));

            StepId = RequireText(
                execution.StepId,
                nameof(execution.StepId));

            StepDisplayName =
                NormalizeText(
                    execution.StepDisplayName);

            StepIndex =
                execution.StepIndex;

            StepCount =
                execution.StepCount;

            Policy =
                execution.Policy;

            Result =
                execution.Result;

            Status =
                Result.Status;

            Progress =
                execution.LatestProgress;

            StartSeconds =
                timing.StartSeconds;

            SettlementSeconds =
                timing.SettlementSeconds;

            ElapsedSeconds =
                timing.ElapsedSeconds;

            TimeoutSeconds =
                timing.TimeoutSeconds;

            HasTimeout =
                timing.HasTimeout;

            TimedOut =
                timing.TimedOut;

            TimeoutCancellationRequested =
                timing.CancellationRequested;
        }

        /// <summary>
        /// Gets the copied stable startup-sequence entry identity.
        /// </summary>
        public string EntryId
        {
            get;
        }

        /// <summary>
        /// Gets the copied stable startup-step identity.
        /// </summary>
        public string StepId
        {
            get;
        }

        /// <summary>
        /// Gets the copied authored display label.
        /// </summary>
        public string StepDisplayName
        {
            get;
        }

        /// <summary>
        /// Gets the zero-based authored sequence position.
        /// </summary>
        public int StepIndex
        {
            get;
        }

        /// <summary>
        /// Gets the complete authored sequence entry count observed by the
        /// execution.
        /// </summary>
        public int StepCount
        {
            get;
        }

        /// <summary>
        /// Gets the copied authored execution policy.
        /// </summary>
        public StartupStepPolicy Policy
        {
            get;
        }

        /// <summary>
        /// Gets the final startup-step status.
        /// </summary>
        public StartupStepStatus Status
        {
            get;
        }

        /// <summary>
        /// Gets the immutable terminal result.
        /// </summary>
        public StartupStepResult Result
        {
            get;
        }

        /// <summary>
        /// Gets the final accepted executor progress value.
        /// </summary>
        public StartupStepProgress Progress
        {
            get;
        }

        /// <summary>
        /// Gets the monotonic unscaled attempt start time.
        /// </summary>
        public double StartSeconds
        {
            get;
        }

        /// <summary>
        /// Gets the monotonic unscaled settlement time.
        /// </summary>
        public double SettlementSeconds
        {
            get;
        }

        /// <summary>
        /// Gets the measured attempt duration.
        /// </summary>
        public double ElapsedSeconds
        {
            get;
        }

        /// <summary>
        /// Gets the configured timeout duration. Zero means disabled.
        /// </summary>
        public double TimeoutSeconds
        {
            get;
        }

        /// <summary>
        /// Gets whether a positive timeout was configured.
        /// </summary>
        public bool HasTimeout
        {
            get;
        }

        /// <summary>
        /// Gets whether the timeout deadline won before executor settlement.
        /// </summary>
        public bool TimedOut
        {
            get;
        }

        /// <summary>
        /// Gets whether timeout handling requested cooperative cancellation.
        /// </summary>
        public bool TimeoutCancellationRequested
        {
            get;
        }

        private static string RequireText(
            string value,
            string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "A nonblank stable identity is required.",
                    parameterName);
            }

            return value.Trim();
        }

        private static string NormalizeText(
            string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }
    }
}

//----- LaunchStepReport.cs END -----
