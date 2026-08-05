//----- StartupStepExecution.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores runtime-only state for one enabled startup-step attempt.
    ///
    /// Authored definitions and sequence entries remain immutable. The
    /// execution object owns the fresh executor, active progress, and one
    /// terminal result.
    /// </summary>
    internal sealed class StartupStepExecution :
        IStartupStepProgressReporter
    {
        private readonly IStartupStepExecutor executor;

        private StartupStepStatus status =
            StartupStepStatus.NotStarted;

        private StartupStepProgress latestProgress =
            StartupStepProgress.Indeterminate();

        private StartupStepResult result;

        /// <summary>
        /// Creates one runtime execution for one enabled sequence entry.
        /// </summary>
        internal StartupStepExecution(
            StartupSequenceEntry entry,
            int stepIndex,
            int stepCount,
            IStartupStepExecutor executor)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(
                    nameof(entry));
            }

            StartupStepDefinition definition =
                entry.StepDefinition;

            if (definition == null)
            {
                throw new ArgumentException(
                    "An enabled startup-sequence entry requires a step definition.",
                    nameof(entry));
            }

            if (stepCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(stepCount),
                    stepCount,
                    "Startup-step count must be greater than zero.");
            }

            if (stepIndex < 0 ||
                stepIndex >= stepCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(stepIndex),
                    stepIndex,
                    "Startup-step index must be within the authored sequence bounds.");
            }

            this.executor =
                executor ??
                throw new ArgumentNullException(
                    nameof(executor));

            EntryId = RequireText(
                entry.EntryId,
                nameof(entry.EntryId));

            StepId = RequireText(
                definition.StepId,
                nameof(definition.StepId));

            StepDisplayName =
                definition.DisplayName ?? string.Empty;

            StepIndex = stepIndex;
            StepCount = stepCount;
            Policy = entry.Policy;
        }

        /// <summary>
        /// Gets the copied stable sequence-entry identity.
        /// </summary>
        internal string EntryId
        {
            get;
        }

        /// <summary>
        /// Gets the copied stable step-definition identity.
        /// </summary>
        internal string StepId
        {
            get;
        }

        /// <summary>
        /// Gets the copied authored display label.
        /// </summary>
        internal string StepDisplayName
        {
            get;
        }

        /// <summary>
        /// Gets the zero-based authored sequence position.
        /// </summary>
        internal int StepIndex
        {
            get;
        }

        /// <summary>
        /// Gets the complete authored sequence entry count.
        /// </summary>
        internal int StepCount
        {
            get;
        }

        /// <summary>
        /// Gets the copied authored policy.
        /// </summary>
        internal StartupStepPolicy Policy
        {
            get;
        }

        /// <summary>
        /// Gets the current runtime step status.
        /// </summary>
        internal StartupStepStatus Status =>
            status;

        /// <summary>
        /// Gets the latest progress reported during this attempt.
        /// </summary>
        internal StartupStepProgress LatestProgress =>
            latestProgress;

        /// <summary>
        /// Gets the terminal result after completion.
        /// </summary>
        internal StartupStepResult Result =>
            result;

        /// <summary>
        /// Gets whether this attempt has captured a terminal result.
        /// </summary>
        internal bool IsComplete =>
            result != null;

        /// <summary>
        /// Gets the fresh single-use executor owned by this attempt.
        /// </summary>
        internal IStartupStepExecutor Executor =>
            executor;

        /// <summary>
        /// Moves this attempt from NotStarted to Running.
        /// </summary>
        internal void Begin()
        {
            if (status !=
                StartupStepStatus.NotStarted)
            {
                throw new InvalidOperationException(
                    "A startup-step execution may begin exactly once.");
            }

            status =
                StartupStepStatus.Running;
        }

        /// <summary>
        /// Captures the terminal result exactly once.
        /// </summary>
        internal void Complete(
            StartupStepResult terminalResult)
        {
            if (status !=
                StartupStepStatus.Running)
            {
                throw new InvalidOperationException(
                    "A startup-step execution may complete only while running.");
            }

            result =
                terminalResult ??
                throw new ArgumentNullException(
                    nameof(terminalResult));

            status =
                terminalResult.Status;
        }

        /// <summary>
        /// Captures meaningful progress only while this attempt is active.
        /// </summary>
        public void Report(
            StartupStepProgress progress)
        {
            if (status !=
                StartupStepStatus.Running)
            {
                throw new InvalidOperationException(
                    "Startup-step progress may be reported only while the execution is running.");
            }

            latestProgress = progress;
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
    }
}

//----- StartupStepExecution.cs END -----
