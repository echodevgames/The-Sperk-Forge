//----- StartupSequenceRunResult.cs START -----

using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable runtime summary produced after one startup-sequence
    /// traversal completes or stops.
    ///
    /// This result preserves attempted execution order, captured terminal
    /// results, and authored traversal accounting without claiming final
    /// launch lifecycle state.
    /// </summary>
    internal sealed class StartupSequenceRunResult
    {
        private const int NoStoppingIndex = -1;

        private readonly StartupStepExecution[] executions;

        /// <summary>
        /// Creates one immutable complete-traversal summary.
        ///
        /// This overload preserves the FL-M3-01 construction contract.
        /// </summary>
        internal StartupSequenceRunResult(
            int authoredEntryCount,
            int disabledEntryCount,
            IReadOnlyList<StartupStepExecution>
                attemptedExecutions)
            : this(
                authoredEntryCount,
                disabledEntryCount,
                attemptedExecutions,
                NoStoppingIndex)
        {
        }

        /// <summary>
        /// Creates one immutable traversal summary that may represent a
        /// policy or contract stop.
        /// </summary>
        internal StartupSequenceRunResult(
            int authoredEntryCount,
            int disabledEntryCount,
            IReadOnlyList<StartupStepExecution>
                attemptedExecutions,
            int stoppingAuthoredEntryIndex)
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

            int unvisitedEntryCount =
                authoredEntryCount -
                disabledEntryCount -
                attemptedExecutions.Count;

            if (unvisitedEntryCount < 0)
            {
                throw new ArgumentException(
                    "Attempted and disabled startup-sequence entries must not exceed the authored entry count.",
                    nameof(attemptedExecutions));
            }

            bool wasStopped =
                stoppingAuthoredEntryIndex !=
                NoStoppingIndex;

            if (!wasStopped &&
                unvisitedEntryCount != 0)
            {
                throw new ArgumentException(
                    "A complete startup-sequence traversal must account for every authored entry as disabled or attempted.",
                    nameof(attemptedExecutions));
            }

            if (wasStopped)
            {
                if (stoppingAuthoredEntryIndex < 0 ||
                    stoppingAuthoredEntryIndex >=
                    authoredEntryCount)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(stoppingAuthoredEntryIndex),
                        stoppingAuthoredEntryIndex,
                        "The stopping authored entry index must be within the sequence bounds.");
                }

                if (attemptedExecutions.Count == 0)
                {
                    throw new ArgumentException(
                        "A stopped startup-sequence traversal requires one attempted execution that caused the stop.",
                        nameof(attemptedExecutions));
                }
            }

            executions =
                new StartupStepExecution[
                    attemptedExecutions.Count];

            bool hasWarnings = false;
            bool hasFailures = false;
            bool hasBlockingFailures = false;
            bool wasCancelled = false;

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

                if (result.Status ==
                    StartupStepStatus.Cancelled)
                {
                    wasCancelled = true;
                }
            }

            if (wasStopped)
            {
                StartupStepExecution stoppingExecution =
                    executions[
                        executions.Length - 1];

                if (stoppingExecution.StepIndex !=
                    stoppingAuthoredEntryIndex)
                {
                    throw new ArgumentException(
                        "The stopping authored entry index must match the final attempted execution.",
                        nameof(stoppingAuthoredEntryIndex));
                }
            }

            AuthoredEntryCount =
                authoredEntryCount;

            DisabledEntryCount =
                disabledEntryCount;

            UnvisitedEntryCount =
                unvisitedEntryCount;

            StoppingAuthoredEntryIndex =
                stoppingAuthoredEntryIndex;

            HasWarnings =
                hasWarnings;

            HasFailures =
                hasFailures;

            HasBlockingFailures =
                hasBlockingFailures;

            WasCancelled =
                wasCancelled;
        }

        /// <summary>
        /// Gets the complete authored sequence entry count.
        /// </summary>
        internal int AuthoredEntryCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of authored entries inspected and skipped because
        /// they were disabled.
        /// </summary>
        internal int DisabledEntryCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of authored entries not inspected because
        /// traversal stopped.
        /// </summary>
        internal int UnvisitedEntryCount
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
        /// Gets whether traversal ended because one attempted entry stopped
        /// the sequence.
        /// </summary>
        internal bool WasStoppedEarly =>
            StoppingAuthoredEntryIndex !=
            NoStoppingIndex;

        /// <summary>
        /// Gets the authored index of the execution that stopped traversal,
        /// or minus one when traversal completed normally.
        /// </summary>
        internal int StoppingAuthoredEntryIndex
        {
            get;
        }

        /// <summary>
        /// Gets whether traversal captured a terminal cancellation result.
        /// </summary>
        internal bool WasCancelled
        {
            get;
        }

        /// <summary>
        /// Gets whether any effective captured result completed with a
        /// warning.
        /// </summary>
        internal bool HasWarnings
        {
            get;
        }

        /// <summary>
        /// Gets whether any effective captured result is a recoverable or
        /// blocking failure.
        /// </summary>
        internal bool HasFailures
        {
            get;
        }

        /// <summary>
        /// Gets whether any effective captured result explicitly blocks
        /// launch.
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
