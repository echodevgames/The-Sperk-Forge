//----- LaunchReport.cs START -----

using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable public summary of one finalized launch attempt.
    ///
    /// Failed, interrupted, and destination-activated completed attempts are
    /// represented without exposing live runtime execution objects.
    /// </summary>
    public sealed class LaunchReport
    {
        /// <summary>
        /// Identifies the currently supported report structure.
        ///
        /// This version is independent from package and authored asset schema
        /// versions because report export compatibility evolves separately.
        /// </summary>
        public const int CurrentSchemaVersion = 2;

        /// <summary>
        /// Gets the package version that produced this report.
        /// </summary>
        public const string CurrentPackageVersion = "0.1.0-beta.1";

        private readonly LaunchStepReport[] stepReports;

        /// <summary>
        /// Creates one validated immutable finalized report.
        /// </summary>
        internal LaunchReport(
            LaunchMode launchMode,
            string configurationId,
            string sequenceId,
            LaunchStatus finalStatus,
            double startSeconds,
            double finalizationSeconds,
            int authoredEntryCount,
            int disabledEntryCount,
            int unvisitedEntryCount,
            bool wasCancelled,
            StartupStepResult finalResult,
            IReadOnlyList<LaunchStepReport>
                completedStepReports)
            : this(
                launchMode,
                configurationId,
                sequenceId,
                string.Empty,
                string.Empty,
                finalStatus,
                startSeconds,
                finalizationSeconds,
                authoredEntryCount,
                disabledEntryCount,
                unvisitedEntryCount,
                wasCancelled,
                finalResult,
                completedStepReports)
        {
        }

        /// <summary>
        /// Creates one validated immutable finalized report with copied
        /// destination identity and display metadata.
        /// </summary>
        internal LaunchReport(
            LaunchMode launchMode,
            string configurationId,
            string sequenceId,
            string destinationId,
            string destinationDisplayName,
            LaunchStatus finalStatus,
            double startSeconds,
            double finalizationSeconds,
            int authoredEntryCount,
            int disabledEntryCount,
            int unvisitedEntryCount,
            bool wasCancelled,
            StartupStepResult finalResult,
            IReadOnlyList<LaunchStepReport>
                completedStepReports)
        {
            ValidateLaunchMode(launchMode);
            ValidateFinalStatus(finalStatus);
            ValidateFiniteNonnegative(
                startSeconds,
                nameof(startSeconds));
            ValidateFiniteNonnegative(
                finalizationSeconds,
                nameof(finalizationSeconds));

            if (finalizationSeconds <
                startSeconds)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(finalizationSeconds),
                    finalizationSeconds,
                    "Launch-report finalization time must not be earlier than launch start time.");
            }

            if (authoredEntryCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authoredEntryCount),
                    authoredEntryCount,
                    "Authored entry count must not be negative.");
            }

            if (disabledEntryCount < 0 ||
                disabledEntryCount >
                authoredEntryCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(disabledEntryCount),
                    disabledEntryCount,
                    "Disabled entry count must remain within the authored entry count.");
            }

            if (unvisitedEntryCount < 0 ||
                unvisitedEntryCount >
                authoredEntryCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(unvisitedEntryCount),
                    unvisitedEntryCount,
                    "Unvisited entry count must remain within the authored entry count.");
            }

            if (completedStepReports == null)
            {
                throw new ArgumentNullException(
                    nameof(completedStepReports));
            }

            if (finalResult == null)
            {
                throw new ArgumentNullException(
                    nameof(finalResult));
            }

            string normalizedDestinationId =
                NormalizeText(
                    destinationId);

            string normalizedDestinationDisplayName =
                NormalizeText(
                    destinationDisplayName);

            ValidateDestination(
                finalStatus,
                normalizedDestinationId,
                normalizedDestinationDisplayName);

            if (finalStatus ==
                    LaunchStatus.Completed &&
                (!finalResult.IsSuccessful ||
                 wasCancelled))
            {
                throw new ArgumentException(
                    "A completed launch report requires a successful non-cancelled final result.",
                    nameof(finalResult));
            }

            if (finalStatus ==
                    LaunchStatus.Interrupted &&
                !wasCancelled)
            {
                throw new ArgumentException(
                    "An interrupted launch report must record cancellation.",
                    nameof(wasCancelled));
            }

            if (finalStatus ==
                    LaunchStatus.Failed &&
                wasCancelled)
            {
                throw new ArgumentException(
                    "A failed launch report must not be marked cancelled.",
                    nameof(wasCancelled));
            }

            stepReports =
                new LaunchStepReport[
                    completedStepReports.Count];

            int warningCount = 0;
            int failureCount = 0;
            int blockingFailureCount = 0;
            bool finalResultAlreadyCounted = false;
            int previousStepIndex = -1;

            for (int index = 0;
                 index < completedStepReports.Count;
                 index++)
            {
                LaunchStepReport stepReport =
                    completedStepReports[index];

                if (stepReport == null)
                {
                    throw new ArgumentException(
                        "Launch step reports must not contain null entries.",
                        nameof(completedStepReports));
                }

                if (stepReport.StepCount !=
                    authoredEntryCount)
                {
                    throw new ArgumentException(
                        "Each launch step report must retain the complete authored entry count.",
                        nameof(completedStepReports));
                }

                if (stepReport.StepIndex <=
                        previousStepIndex ||
                    stepReport.StepIndex < 0 ||
                    stepReport.StepIndex >=
                        authoredEntryCount)
                {
                    throw new ArgumentException(
                        "Launch step reports must preserve unique authored order within sequence bounds.",
                        nameof(completedStepReports));
                }

                previousStepIndex =
                    stepReport.StepIndex;

                stepReports[index] =
                    stepReport;

                AccumulateResult(
                    stepReport.Result,
                    ref warningCount,
                    ref failureCount,
                    ref blockingFailureCount);

                if (ReferenceEquals(
                        stepReport.Result,
                        finalResult))
                {
                    finalResultAlreadyCounted =
                        true;
                }
            }

            int attemptedStepCount =
                stepReports.Length;

            if (attemptedStepCount +
                    disabledEntryCount +
                    unvisitedEntryCount !=
                authoredEntryCount)
            {
                throw new ArgumentException(
                    "Attempted, disabled, and unvisited entries must exactly balance the authored entry count.",
                    nameof(completedStepReports));
            }

            if (!finalResultAlreadyCounted)
            {
                AccumulateResult(
                    finalResult,
                    ref warningCount,
                    ref failureCount,
                    ref blockingFailureCount);
            }

            ReportSchemaVersion =
                CurrentSchemaVersion;

            PackageVersion =
                CurrentPackageVersion;

            LaunchMode =
                launchMode;

            ConfigurationId =
                NormalizeText(
                    configurationId);

            SequenceId =
                NormalizeText(
                    sequenceId);

            DestinationId =
                normalizedDestinationId;

            DestinationDisplayName =
                normalizedDestinationDisplayName;

            FinalStatus =
                finalStatus;

            StartSeconds =
                startSeconds;

            FinalizationSeconds =
                finalizationSeconds;

            ElapsedSeconds =
                finalizationSeconds -
                startSeconds;

            AuthoredEntryCount =
                authoredEntryCount;

            AttemptedStepCount =
                attemptedStepCount;

            DisabledEntryCount =
                disabledEntryCount;

            UnvisitedEntryCount =
                unvisitedEntryCount;

            WarningCount =
                warningCount;

            FailureCount =
                failureCount;

            BlockingFailureCount =
                blockingFailureCount;

            WasCancelled =
                wasCancelled;

            FinalResult =
                finalResult;
        }

        /// <summary>
        /// Gets the report schema version.
        /// </summary>
        public int ReportSchemaVersion
        {
            get;
        }

        /// <summary>
        /// Gets the EchoLaunch package version that produced the report.
        /// </summary>
        public string PackageVersion
        {
            get;
        }

        /// <summary>
        /// Gets the launch entry mode.
        /// </summary>
        public LaunchMode LaunchMode
        {
            get;
        }

        /// <summary>
        /// Gets the copied configuration identity when available.
        /// </summary>
        public string ConfigurationId
        {
            get;
        }

        /// <summary>
        /// Gets the copied startup-sequence identity when available.
        /// </summary>
        public string SequenceId
        {
            get;
        }

        /// <summary>
        /// Gets the copied initial destination identity when available.
        /// </summary>
        public string DestinationId
        {
            get;
        }

        /// <summary>
        /// Gets the copied initial destination display name when available.
        /// </summary>
        public string DestinationDisplayName
        {
            get;
        }

        /// <summary>
        /// Gets the finalized launch lifecycle status.
        /// </summary>
        public LaunchStatus FinalStatus
        {
            get;
        }

        /// <summary>
        /// Gets the monotonic unscaled launch start time.
        /// </summary>
        public double StartSeconds
        {
            get;
        }

        /// <summary>
        /// Gets the monotonic unscaled report finalization time.
        /// </summary>
        public double FinalizationSeconds
        {
            get;
        }

        /// <summary>
        /// Gets the total launch duration before report finalization.
        /// </summary>
        public double ElapsedSeconds
        {
            get;
        }

        /// <summary>
        /// Gets the total authored sequence entry count.
        /// </summary>
        public int AuthoredEntryCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of attempted enabled steps.
        /// </summary>
        public int AttemptedStepCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of disabled authored entries.
        /// </summary>
        public int DisabledEntryCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of authored entries not visited after an early stop.
        /// </summary>
        public int UnvisitedEntryCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of warning results represented by the report.
        /// </summary>
        public int WarningCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of recoverable or blocking failures represented by
        /// the report.
        /// </summary>
        public int FailureCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of explicit blocking failures represented by the
        /// report.
        /// </summary>
        public int BlockingFailureCount
        {
            get;
        }

        /// <summary>
        /// Gets whether cancellation ended the launch attempt.
        /// </summary>
        public bool WasCancelled
        {
            get;
        }

        /// <summary>
        /// Gets the stable final diagnostic or result for the launch attempt.
        /// </summary>
        public StartupStepResult FinalResult
        {
            get;
        }

        /// <summary>
        /// Gets the number of immutable completed step reports.
        /// </summary>
        public int StepReportCount =>
            stepReports.Length;

        /// <summary>
        /// Gets whether the report contains any warning result.
        /// </summary>
        public bool HasWarnings =>
            WarningCount > 0;

        /// <summary>
        /// Gets whether the report contains any recoverable or blocking
        /// failure.
        /// </summary>
        public bool HasFailures =>
            FailureCount > 0;

        /// <summary>
        /// Gets whether the report contains an explicit blocking failure.
        /// </summary>
        public bool HasBlockingFailures =>
            BlockingFailureCount > 0;

        /// <summary>
        /// Gets one immutable step report in authored traversal order.
        /// </summary>
        public LaunchStepReport GetStepReport(
            int index)
        {
            if (index < 0 ||
                index >= stepReports.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    index,
                    "The launch step report index is outside the finalized report bounds.");
            }

            return stepReports[index];
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
                    "A finalized launch report requires a defined non-unknown launch mode.");
            }
        }

        private static void ValidateFinalStatus(
            LaunchStatus finalStatus)
        {
            if (finalStatus !=
                    LaunchStatus.Completed &&
                finalStatus !=
                    LaunchStatus.Failed &&
                finalStatus !=
                    LaunchStatus.Interrupted)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(finalStatus),
                    finalStatus,
                    "A finalized launch report requires Completed, Failed, or Interrupted status.");
            }
        }

        private static void ValidateDestination(
            LaunchStatus finalStatus,
            string destinationId,
            string destinationDisplayName)
        {
            if (string.IsNullOrEmpty(
                    destinationId))
            {
                if (finalStatus ==
                    LaunchStatus.Completed)
                {
                    throw new ArgumentException(
                        "A completed launch report requires a destination identity.",
                        nameof(destinationId));
                }

                if (!string.IsNullOrEmpty(
                        destinationDisplayName))
                {
                    throw new ArgumentException(
                        "Destination display metadata requires a destination identity.",
                        nameof(destinationDisplayName));
                }

                return;
            }

            if (!LaunchDestination
                    .IsCanonicalDestinationId(
                        destinationId))
            {
                throw new ArgumentException(
                    "The report destination identity must use lowercase 32-character hexadecimal format.",
                    nameof(destinationId));
            }

            if (string.IsNullOrEmpty(
                    destinationDisplayName))
            {
                throw new ArgumentException(
                    "Destination display metadata is required when a destination identity is present.",
                    nameof(destinationDisplayName));
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

        private static void AccumulateResult(
            StartupStepResult result,
            ref int warningCount,
            ref int failureCount,
            ref int blockingFailureCount)
        {
            if (result == null)
            {
                throw new ArgumentNullException(
                    nameof(result));
            }

            if (result.Status ==
                StartupStepStatus.Warning)
            {
                warningCount++;
            }

            if (result.IsFailure)
            {
                failureCount++;
            }

            if (result.IsBlocking)
            {
                blockingFailureCount++;
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

//----- LaunchReport.cs END -----
