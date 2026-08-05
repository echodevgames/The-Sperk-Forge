//----- StartupSequenceRunResult.cs START -----

using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable runtime summary produced after one startup-sequence
    /// traversal completes.
    ///
    /// This result preserves attempted execution order and captured step
    /// results without interpreting authored failure policy or claiming
    /// final launch success.
    /// </summary>
    internal sealed class StartupSequenceRunResult
    {
        private readonly StartupStepExecution[] executions;

        /// <summary>
        /// Creates one immutable completed traversal summary.
        /// </summary>
        internal StartupSequenceRunResult(
            int authoredEntryCount,
            int disabledEntryCount,
            IReadOnlyList<StartupStepExecution>
                attemptedExecutions)
        {
            if (authoredEntryCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authoredEntryCount),
                    authoredEntryCount,
                    "Authored startup-sequence entry count must not be negative.");
            }

            if (disabledEntryCount < 0 ||
                disabledEntryCount >
                authoredEntryCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(disabledEntryCount),
                    disabledEntryCount,
                    "Disabled startup-sequence entry count must be within the authored entry count.");
            }

            if (attemptedExecutions == null)
            {
                throw new ArgumentNullException(
                    nameof(attemptedExecutions));
            }

            if (attemptedExecutions.Count +
                disabledEntryCount !=
                authoredEntryCount)
            {
                throw new ArgumentException(
                    "Completed startup-sequence results must account for every authored entry as either disabled or attempted.",
                    nameof(attemptedExecutions));
            }

            executions =
                new StartupStepExecution[
                    attemptedExecutions.Count];

            bool hasWarnings = false;
            bool hasFailures = false;
            bool hasBlockingFailures = false;

            for (int index = 0;
                 index < attemptedExecutions.Count;
                 index++)
            {
                StartupStepExecution execution =
                    attemptedExecutions[index];

                if (execution == null)
                {
                    throw new ArgumentException(
                        "Attempted startup-step executions must not contain null entries.",
                        nameof(attemptedExecutions));
                }

                if (!execution.IsComplete ||
                    execution.Result == null)
                {
                    throw new ArgumentException(
                        "A completed startup-sequence result requires every attempted execution to hold one terminal result.",
                        nameof(attemptedExecutions));
                }

                executions[index] = execution;

                StartupStepResult result =
                    execution.Result;

                if (result.Status ==
                    StartupStepStatus.Warning)
                {
                    hasWarnings = true;
                }

                if (result.IsFailure)
                {
                    hasFailures = true;
                }

                if (result.IsBlocking)
                {
                    hasBlockingFailures = true;
                }
            }

            AuthoredEntryCount =
                authoredEntryCount;

            DisabledEntryCount =
                disabledEntryCount;

            HasWarnings =
                hasWarnings;

            HasFailures =
                hasFailures;

            HasBlockingFailures =
                hasBlockingFailures;
        }

        /// <summary>
        /// Gets the complete authored sequence entry count.
        /// </summary>
        internal int AuthoredEntryCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of authored entries skipped because they were
        /// disabled.
        /// </summary>
        internal int DisabledEntryCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of enabled entries that produced attempted
        /// executions.
        /// </summary>
        internal int AttemptedExecutionCount =>
            executions.Length;

        /// <summary>
        /// Gets whether any captured result completed with a warning.
        /// </summary>
        internal bool HasWarnings
        {
            get;
        }

        /// <summary>
        /// Gets whether any captured result is a recoverable or blocking
        /// failure.
        /// </summary>
        internal bool HasFailures
        {
            get;
        }

        /// <summary>
        /// Gets whether any captured result explicitly blocks launch.
        ///
        /// FL-M3-01 records this fact but does not apply policy or stop
        /// traversal.
        /// </summary>
        internal bool HasBlockingFailures
        {
            get;
        }

        /// <summary>
        /// Gets one completed attempted execution in authored traversal
        /// order.
        /// </summary>
        internal StartupStepExecution GetExecution(
            int index)
        {
            if (index < 0 ||
                index >= executions.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    index,
                    "The attempted startup-step execution index is outside the completed run result.");
            }

            return executions[index];
        }
    }
}

//----- StartupSequenceRunResult.cs END -----
