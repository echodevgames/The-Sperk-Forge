using System.Text;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal static class EchoLaunchValidationTextFormatter
    {
        internal static string Format(
            EchoLaunchValidationReport report)
        {
            if (report == null)
            {
                return
                    "First Light Validation Report\n" +
                    "No validation report is available.";
            }

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("First Light Validation Report");
            builder.Append("Schema: ")
                .AppendLine(report.SchemaVersion.ToString());

            builder.Append("Health: ")
                .AppendLine(report.Health.ToString());

            builder.Append("Project root: ")
                .AppendLine(report.Request.ProjectRootPath);

            builder.Append("Information: ")
                .AppendLine(report.InformationCount.ToString());

            builder.Append("Warnings: ")
                .AppendLine(report.WarningCount.ToString());

            builder.Append("Errors: ")
                .AppendLine(report.ErrorCount.ToString());

            builder.Append("Blockers: ")
                .AppendLine(report.BlockerCount.ToString());

            builder.Append("Request fingerprint: ")
                .AppendLine(report.RequestFingerprint);

            builder.Append("Evidence fingerprint: ")
                .AppendLine(report.EvidenceFingerprint);

            builder.Append("Report fingerprint: ")
                .AppendLine(report.ReportFingerprint);

            builder.AppendLine();
            builder.AppendLine("Findings:");

            if (report.FindingCount == 0)
            {
                builder.AppendLine("None.");
                return builder.ToString().TrimEnd();
            }

            for (int index = 0;
                 index < report.Findings.Count;
                 index++)
            {
                EchoLaunchValidationFinding finding =
                    report.Findings[index];

                builder.Append(index + 1)
                    .Append(". [")
                    .Append(finding.Severity)
                    .Append("] ")
                    .Append(finding.Code)
                    .Append(": ")
                    .AppendLine(finding.Title);

                if (!string.IsNullOrEmpty(finding.ProjectPath))
                {
                    builder.Append("   Path: ")
                        .AppendLine(finding.ProjectPath);
                }

                if (!string.IsNullOrEmpty(finding.Message))
                {
                    builder.Append("   Message: ")
                        .AppendLine(finding.Message);
                }

                if (!string.IsNullOrEmpty(finding.Evidence))
                {
                    builder.Append("   Evidence: ")
                        .AppendLine(finding.Evidence);
                }

                if (!string.IsNullOrEmpty(finding.SuggestedAction))
                {
                    builder.Append("   Action: ")
                        .AppendLine(finding.SuggestedAction);
                }
            }

            return builder.ToString().TrimEnd();
        }
    }
}
