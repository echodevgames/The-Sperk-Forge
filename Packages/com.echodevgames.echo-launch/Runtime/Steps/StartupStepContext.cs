//----- StartupStepContext.cs START -----

using System;
using System.Threading;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable runtime metadata supplied to one fresh startup-step
    /// executor.
    ///
    /// The context exposes cancellation and progress-reporting seams but
    /// owns no launch authority and cannot mutate authored assets.
    /// </summary>
    public sealed class StartupStepContext
    {
        /// <summary>
        /// Creates one validated execution context.
        /// </summary>
        public StartupStepContext(
            LaunchMode launchMode,
            string configurationId,
            string sequenceId,
            string entryId,
            string stepId,
            int stepIndex,
            int stepCount,
            CancellationToken cancellationToken,
            IStartupStepProgressReporter progressReporter)
        {
            EnsureNonblank(
                configurationId,
                nameof(configurationId));

            EnsureNonblank(
                sequenceId,
                nameof(sequenceId));

            EnsureNonblank(
                entryId,
                nameof(entryId));

            EnsureNonblank(
                stepId,
                nameof(stepId));

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
                    "Startup-step index must be within the current sequence bounds.");
            }

            ProgressReporter =
                progressReporter ??
                throw new ArgumentNullException(
                    nameof(progressReporter));

            LaunchMode = launchMode;
            ConfigurationId = configurationId;
            SequenceId = sequenceId;
            EntryId = entryId;
            StepId = stepId;
            StepIndex = stepIndex;
            StepCount = stepCount;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Gets the launch mode accepted for this execution.
        /// </summary>
        public LaunchMode LaunchMode
        {
            get;
        }

        /// <summary>
        /// Gets the stable configuration identity.
        /// </summary>
        public string ConfigurationId
        {
            get;
        }

        /// <summary>
        /// Gets the stable sequence identity.
        /// </summary>
        public string SequenceId
        {
            get;
        }

        /// <summary>
        /// Gets the stable sequence-entry identity.
        /// </summary>
        public string EntryId
        {
            get;
        }

        /// <summary>
        /// Gets the stable step-definition identity.
        /// </summary>
        public string StepId
        {
            get;
        }

        /// <summary>
        /// Gets the zero-based authored step position.
        /// </summary>
        public int StepIndex
        {
            get;
        }

        /// <summary>
        /// Gets the total number of steps represented by this execution
        /// context.
        /// </summary>
        public int StepCount
        {
            get;
        }

        /// <summary>
        /// Gets the cooperative cancellation token supplied by the future
        /// sequence runner.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get;
        }

        /// <summary>
        /// Gets the package-owned progress-reporting seam.
        /// </summary>
        public IStartupStepProgressReporter ProgressReporter
        {
            get;
        }

        private static void EnsureNonblank(
            string value,
            string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Startup-step context identities must not be null, empty, or whitespace.",
                    parameterName);
            }
        }
    }
}

//----- StartupStepContext.cs END -----
