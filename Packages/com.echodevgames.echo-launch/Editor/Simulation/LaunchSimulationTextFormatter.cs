using System.Globalization;
using System.Text;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal static class LaunchSimulationTextFormatter
    {
        internal static string Format(
            LaunchSimulationReport report)
        {
            if (report == null)
            {
                return "First Light Launch Simulation Report\n" +
                       "Status: NotRun\n";
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                "First Light Launch Simulation Report");
            builder.AppendLine(
                "Schema: " +
                report.SchemaVersion.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Status: " + report.Status);
            builder.AppendLine(
                "Preset: " + report.Preset);
            builder.AppendLine(
                "Logical duration: " +
                report.LogicalDurationSeconds.ToString(
                    "0.###",
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Requested progress samples: " +
                report.ProgressSampleRequestCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Timeout: " +
                report.TimeoutSeconds.ToString(
                    "0.###",
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Authored: " +
                report.AuthoredEntryCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Disabled: " +
                report.DisabledEntryCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Attempted: " +
                report.AttemptedEntryCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Unvisited: " +
                report.UnvisitedEntryCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Cancelled: " + report.WasCancelled);
            builder.AppendLine(
                "Request fingerprint: " +
                report.RequestFingerprint);
            builder.AppendLine(
                "Plan fingerprint: " +
                report.PlanFingerprint);
            builder.AppendLine(
                "Report fingerprint: " +
                report.ReportFingerprint);

            if (!string.IsNullOrEmpty(report.DiagnosticCode))
            {
                builder.AppendLine();
                builder.AppendLine(
                    "Diagnostic: " +
                    report.DiagnosticCode);
                builder.AppendLine(
                    "Message: " +
                    report.DiagnosticMessage);

                if (!string.IsNullOrEmpty(
                        report.DiagnosticDetails))
                {
                    builder.AppendLine(
                        "Details: " +
                        report.DiagnosticDetails);
                }
            }

            builder.AppendLine();
            builder.AppendLine("Steps:");

            if (report.StepCount == 0)
            {
                builder.AppendLine("None.");
            }
            else
            {
                for (int index = 0;
                     index < report.StepCount;
                     index++)
                {
                    LaunchSimulationStepReport step =
                        report.GetStep(index);

                    builder.Append(index + 1)
                           .Append(". [")
                           .Append(step.Status)
                           .Append("] ")
                           .AppendLine(step.DisplayName);

                    builder.Append("   Entry: ")
                           .AppendLine(step.EntryId);
                    builder.Append("   Step: ")
                           .AppendLine(step.StepId);
                    builder.Append("   Policy: ")
                           .Append(step.IsRequired
                               ? "Required"
                               : "Optional")
                           .Append(" / ")
                           .AppendLine(
                               step.FailureAction.ToString());
                    builder.Append("   Logical elapsed: ")
                           .Append(
                               step.LogicalElapsedSeconds.ToString(
                                   "0.###",
                                   CultureInfo.InvariantCulture))
                           .AppendLine();

                    if (!string.IsNullOrEmpty(step.Code))
                    {
                        builder.Append("   Code: ")
                               .AppendLine(step.Code);
                    }

                    if (!string.IsNullOrEmpty(step.Message))
                    {
                        builder.Append("   Message: ")
                               .AppendLine(step.Message);
                    }

                    if (!string.IsNullOrEmpty(step.Details))
                    {
                        builder.Append("   Details: ")
                               .AppendLine(step.Details);
                    }
                }
            }

            builder.AppendLine();
            builder.AppendLine("Progress:");

            if (report.ProgressSampleCount == 0)
            {
                builder.AppendLine("None.");
            }
            else
            {
                for (int index = 0;
                     index < report.ProgressSampleCount;
                     index++)
                {
                    LaunchSimulationProgressSample sample =
                        report.GetProgressSample(index);

                    builder.Append(index + 1)
                           .Append(". Step ")
                           .Append(sample.AuthoredStepIndex + 1)
                           .Append(" @ ")
                           .Append(
                               sample.LogicalSeconds.ToString(
                                   "0.###",
                                   CultureInfo.InvariantCulture))
                           .Append("s: ");

                    if (sample.IsIndeterminate)
                    {
                        builder.Append("Indeterminate");
                    }
                    else
                    {
                        builder.Append(
                            (sample.Progress01 * 100f)
                            .ToString(
                                "0.###",
                                CultureInfo.InvariantCulture))
                               .Append('%');
                    }

                    if (!string.IsNullOrEmpty(sample.Message))
                    {
                        builder.Append(" | ")
                               .Append(sample.Message);
                    }

                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }
    }
}
