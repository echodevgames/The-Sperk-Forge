//----- LaunchReportBuilder.cs START -----

using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Internal single-use assembly boundary for one immutable launch report.
    ///
    /// The builder may retain a successful transition-pending run, but
    /// FL-M3-07 finalizes only failed or interrupted reports.
    /// </summary>
    internal sealed class LaunchReportBuilder
    {
        private readonly LaunchMode launchMode;
        private readonly double launchStartSeconds;
        private readonly List<LaunchStepReport>
            stepReports =
                new List<LaunchStepReport>();

        private readonly HashSet<string>
            capturedEntryIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

        private string configurationId;
        private string sequenceId;
        private int authoredEntryCount;
        private int disabledEntryCount;
        private StartupSequenceRunResult
            pendingRunResult;
        private bool isFinalized;

        /// <summary>
        /// Creates one report builder when an authoritative root accepts a
        /// launch start.
        /// </summary>
        internal LaunchReportBuilder(
            LaunchMode launchMode,
            EchoLaunchConfiguration configuration,
            double launchStartSeconds)
        {
            ValidateLaunchMode(
                launchMode);
            ValidateFiniteNonnegative(
                launchStartSeconds,
                nameof(launchStartSeconds));

            this.launchMode =
                launchMode;

            this.launchStartSeconds =
                launchStartSeconds;

            CaptureConfiguration(
                configuration);
        }

        /// <summary>
        /// Gets whether the builder has finalized one immutable report.
        /// </summary>
        internal bool IsFinalized =>
            isFinalized;

        /// <summary>
        /// Gets whether successful sequence work is retained for the later
        /// destination-handoff checkpoint.
        /// </summary>
        internal bool HasPendingSuccessfulRun =>
            pendingRunResult != null &&
            !isFinalized;

        /// <summary>
        /// Gets the number of completed step copies currently retained.
        /// </summary>
        internal int CapturedStepCount =>
            stepReports.Count;

        /// <summary>
        /// Records the accepted preflight sequence identity and authored
        /// accounting without mutating the sequence.
        /// </summary>
        internal void RecordSequenceValidated(
            StartupSequence sequence)
        {
            EnsureNotFinalized();

            if (sequence == null)
            {
                throw new ArgumentNullException(
                    nameof(sequence));
            }

            sequenceId =
                NormalizeText(
                    sequence.SequenceId);

            authoredEntryCount =
                sequence.EntryCount;

            disabledEntryCount =
                CountDisabledEntries(
                    sequence);
        }

        /// <summary>
        /// Copies one completed execution exactly once.
        /// </summary>
        internal void RecordStepCompleted(
            StartupStepExecution execution)
        {
            EnsureNotFinalized();

            if (execution == null)
            {
                throw new ArgumentNullException(
                    nameof(execution));
            }

            if (!execution.IsComplete)
            {
                throw new ArgumentException(
                    "Only completed startup-step executions may be copied into a launch report.",
                    nameof(execution));
            }

            if (!capturedEntryIds.Add(
                    execution.EntryId))
            {
                throw new InvalidOperationException(
                    "A startup-sequence entry may be copied into one launch report only once.");
            }

            stepReports.Add(
                new LaunchStepReport(
                    execution));
        }

        /// <summary>
        /// Reconciles one settled sequence result and copies any terminal
        /// executions not already observed by the root.
        /// </summary>
        internal void RecordRunResult(
            StartupSequenceRunResult runResult)
        {
            EnsureNotFinalized();

            if (runResult == null)
            {
                throw new ArgumentNullException(
                    nameof(runResult));
            }

            for (int index = 0;
                 index < runResult
                     .AttemptedExecutionCount;
                 index++)
            {
                StartupStepExecution execution =
                    runResult.GetExecution(index);

                if (!capturedEntryIds.Contains(
                        execution.EntryId))
                {
                    RecordStepCompleted(
                        execution);
                }
            }

            authoredEntryCount =
                runResult.AuthoredEntryCount;

            disabledEntryCount =
                runResult.DisabledEntryCount;
        }

        /// <summary>
        /// Retains one successful transition-pending run without producing a
        /// finalized report or terminal event.
        /// </summary>
        internal void MarkTransitionPending(
            StartupSequenceRunResult runResult)
        {
            RecordRunResult(
                runResult);

            pendingRunResult =
                runResult;
        }

        /// <summary>
        /// Finalizes one failed or interrupted immutable report exactly once.
        /// </summary>
        internal LaunchReport FinalizeReport(
            LaunchStatus finalStatus,
            StartupSequenceRunResult runResult,
            StartupStepResult finalResult,
            double finalizationSeconds)
        {
            EnsureNotFinalized();

            if (runResult != null)
            {
                RecordRunResult(
                    runResult);
            }

            ValidateFiniteNonnegative(
                finalizationSeconds,
                nameof(finalizationSeconds));

            if (finalizationSeconds <
                launchStartSeconds)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(finalizationSeconds),
                    finalizationSeconds,
                    "Launch-report finalization time must not be earlier than launch start time.");
            }

            stepReports.Sort(
                CompareByAuthoredIndex);

            int reportAuthoredEntryCount;
            int reportDisabledEntryCount;
            int reportUnvisitedEntryCount;
            bool wasCancelled;

            if (runResult != null)
            {
                reportAuthoredEntryCount =
                    runResult.AuthoredEntryCount;

                reportDisabledEntryCount =
                    runResult.DisabledEntryCount;

                reportUnvisitedEntryCount =
                    runResult.UnvisitedEntryCount;

                wasCancelled =
                    runResult.WasCancelled ||
                    finalStatus ==
                        LaunchStatus.Interrupted;
            }
            else
            {
                reportAuthoredEntryCount =
                    authoredEntryCount;

                reportDisabledEntryCount =
                    disabledEntryCount;

                reportUnvisitedEntryCount =
                    reportAuthoredEntryCount -
                    reportDisabledEntryCount -
                    stepReports.Count;

                if (reportUnvisitedEntryCount < 0)
                {
                    throw new InvalidOperationException(
                        "Captured report steps exceed the available authored entry accounting.");
                }

                wasCancelled =
                    finalStatus ==
                    LaunchStatus.Interrupted;
            }

            LaunchReport report =
                new LaunchReport(
                    launchMode,
                    configurationId,
                    sequenceId,
                    finalStatus,
                    launchStartSeconds,
                    finalizationSeconds,
                    reportAuthoredEntryCount,
                    reportDisabledEntryCount,
                    reportUnvisitedEntryCount,
                    wasCancelled,
                    finalResult,
                    stepReports);

            isFinalized = true;
            pendingRunResult = null;

            return report;
        }

        private void CaptureConfiguration(
            EchoLaunchConfiguration configuration)
        {
            if (configuration == null)
            {
                configurationId =
                    string.Empty;

                sequenceId =
                    string.Empty;

                authoredEntryCount = 0;
                disabledEntryCount = 0;
                return;
            }

            configurationId =
                NormalizeText(
                    configuration.ConfigurationId);

            StartupSequence sequence =
                configuration.StartupSequence;

            if (sequence == null)
            {
                sequenceId =
                    string.Empty;

                authoredEntryCount = 0;
                disabledEntryCount = 0;
                return;
            }

            sequenceId =
                NormalizeText(
                    sequence.SequenceId);

            authoredEntryCount =
                sequence.EntryCount;

            disabledEntryCount =
                CountDisabledEntries(
                    sequence);
        }

        private static int CountDisabledEntries(
            StartupSequence sequence)
        {
            int disabledCount = 0;

            for (int index = 0;
                 index < sequence.EntryCount;
                 index++)
            {
                StartupSequenceEntry entry =
                    sequence.GetEntry(index);

                if (entry != null &&
                    !entry.IsEnabled)
                {
                    disabledCount++;
                }
            }

            return disabledCount;
        }

        private void EnsureNotFinalized()
        {
            if (isFinalized)
            {
                throw new InvalidOperationException(
                    "A launch report builder may finalize exactly once.");
            }
        }

        private static int CompareByAuthoredIndex(
            LaunchStepReport left,
            LaunchStepReport right)
        {
            return left.StepIndex.CompareTo(
                right.StepIndex);
        }

        private static void ValidateLaunchMode(
            LaunchMode launchMode)
        {
            if (!Enum.IsDefined(
                    typeof(LaunchMode),
                    launchMode) ||
                launchMode ==
                    LaunchMode.Unknown)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(launchMode),
                    launchMode,
                    "A launch report builder requires a defined non-unknown launch mode.");
            }
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
                    "Launch-report timing values must be finite and nonnegative.");
            }
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

//----- LaunchReportBuilder.cs END -----
