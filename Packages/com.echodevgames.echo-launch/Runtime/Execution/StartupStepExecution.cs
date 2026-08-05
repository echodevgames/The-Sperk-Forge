//----- StartupStepExecution.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores runtime-only state for one enabled startup-step attempt.
    ///
    /// Authored definitions and sequence entries remain immutable. The
    /// execution object owns copied metadata, an optional fresh executor,
    /// active progress, and one terminal result.
    /// </summary>
    internal sealed class StartupStepExecution :
        IStartupStepProgressReporter
    {
        private IStartupStepExecutor executor;

        private StartupStepStatus status =
            StartupStepStatus.NotStarted;

        private StartupStepProgress latestProgress =
            StartupStepProgress.Indeterminate();

        private StartupStepResult result;

        /// <summary>
        /// Creates one runtime execution from valid authored entry
        /// metadata before executor creation is attempted.
        /// </summary>
        internal StartupStepExecution(
            StartupSequenceEntry entry,
            int stepIndex,
            int stepCount)
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
        /// Creates one runtime execution and immediately attaches the fresh
        /// executor used by the attempt.
        ///
        /// This overload preserves the FL-M3-01 construction contract.
        /// </summary>
        internal StartupStepExecution(
            StartupSequenceEntry entry,
            int stepIndex,
            int stepCount,
            IStartupStepExecutor executor)
            : this(
                entry,
                stepIndex,
                stepCount)
        {
            AttachExecutor(executor);
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
        /// Gets whether a fresh executor has been attached to this attempt.
        /// </summary>
        internal bool HasExecutor =>
            executor != null;

        /// <summary>
        /// Gets the fresh single-use executor attached to this attempt.
        /// </summary>
        internal IStartupStepExecutor Executor =>
            executor;

        /// <summary>
        /// Attaches one fresh executor before the attempt begins.
        /// </summary>
        internal void AttachExecutor(
            IStartupStepExecutor freshExecutor)
        {
            if (freshExecutor == null)
            {
                throw new ArgumentNullException(
                    nameof(freshExecutor));
            }

            if (status !=
                    StartupStepStatus.NotStarted ||
                result != null)
            {
                throw new InvalidOperationException(
                    "A startup-step executor may be attached only before the attempt begins.");
            }

            if (executor != null)
            {
                throw new InvalidOperationException(
                    "A startup-step execution may own exactly one executor.");
            }

            executor = freshExecutor;
        }

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

            if (executor == null)
            {
                throw new InvalidOperationException(
                    "A startup-step execution requires an attached executor before it can begin.");
            }

            status =
                StartupStepStatus.Running;
        }

        /// <summary>
        /// Captures one blocking factory or contract failure before an
        /// executor begins.
        /// </summary>
        internal void CompleteBeforeStart(
            StartupStepResult blockingResult)
        {
            if (status !=
                    StartupStepStatus.NotStarted ||
                result != null)
            {
                throw new InvalidOperationException(
                    "A pre-start startup-step failure may be captured only once before execution begins.");
            }

            if (blockingResult == null)
            {
                throw new ArgumentNullException(
                    nameof(blockingResult));
            }

            if (!blockingResult.IsBlocking)
            {
                throw new ArgumentException(
                    "A pre-start startup-step failure must block launch.",
                    nameof(blockingResult));
            }

            result = blockingResult;
            status = blockingResult.Status;
        }

        /// <summary>
        /// Captures the terminal result exactly once after execution begins.
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
