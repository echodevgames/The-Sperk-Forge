using System;
using System.Globalization;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationStepReport
    {
        internal LaunchSimulationStepReport(
            LaunchSimulationStepPlan plan,
            StartupStepResult result,
            bool hasProgress,
            StartupStepProgress latestProgress,
            double logicalElapsedSeconds)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(
                    nameof(plan));
            }

            if (result == null)
            {
                throw new ArgumentNullException(
                    nameof(result));
            }

            if (double.IsNaN(logicalElapsedSeconds) ||
                double.IsInfinity(logicalElapsedSeconds) ||
                logicalElapsedSeconds < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(logicalElapsedSeconds));
            }

            AuthoredIndex = plan.AuthoredIndex;
            EntryId = plan.EntryId;
            StepId = plan.StepId;
            DisplayName = plan.DisplayName;
            IsRequired = plan.IsRequired;
            FailureAction = plan.FailureAction;
            TimeoutSeconds = plan.TimeoutSeconds;
            SupportsCancellation = plan.SupportsCancellation;
            Status = result.Status;
            Code = result.Code ?? string.Empty;
            Message = result.Message ?? string.Empty;
            Details = result.Details ?? string.Empty;
            HasProgress = hasProgress;
            LatestProgress01 = hasProgress
                ? latestProgress.Progress01
                : 0f;
            LatestProgressIsIndeterminate =
                hasProgress &&
                latestProgress.IsIndeterminate;
            LatestProgressMessage = hasProgress
                ? latestProgress.Message
                : string.Empty;
            LogicalElapsedSeconds =
                logicalElapsedSeconds;
        }

        internal int AuthoredIndex { get; }
        internal string EntryId { get; }
        internal string StepId { get; }
        internal string DisplayName { get; }
        internal bool IsRequired { get; }
        internal StartupStepFailureAction FailureAction { get; }
        internal double TimeoutSeconds { get; }
        internal bool SupportsCancellation { get; }
        internal StartupStepStatus Status { get; }
        internal string Code { get; }
        internal string Message { get; }
        internal string Details { get; }
        internal bool HasProgress { get; }
        internal float LatestProgress01 { get; }
        internal bool LatestProgressIsIndeterminate { get; }
        internal string LatestProgressMessage { get; }
        internal double LogicalElapsedSeconds { get; }

        internal string ToCanonicalText()
        {
            return string.Join(
                "|",
                AuthoredIndex.ToString(
                    CultureInfo.InvariantCulture),
                EntryId,
                StepId,
                DisplayName,
                IsRequired.ToString(),
                ((int)FailureAction).ToString(
                    CultureInfo.InvariantCulture),
                TimeoutSeconds.ToString(
                    "R",
                    CultureInfo.InvariantCulture),
                SupportsCancellation.ToString(),
                ((int)Status).ToString(
                    CultureInfo.InvariantCulture),
                Code,
                Message,
                Details,
                HasProgress.ToString(),
                LatestProgress01.ToString(
                    "R",
                    CultureInfo.InvariantCulture),
                LatestProgressIsIndeterminate.ToString(),
                LatestProgressMessage,
                LogicalElapsedSeconds.ToString(
                    "R",
                    CultureInfo.InvariantCulture));
        }
    }
}
