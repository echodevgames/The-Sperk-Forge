using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal static class LaunchSimulationFingerprint
    {
        internal static string ComputeRequest(
            LaunchSimulationRequest request)
        {
            return request == null
                ? Sha256("Request=None")
                : Sha256(request.ToCanonicalText());
        }

        internal static string ComputePlan(
            LaunchSimulationRequest request,
            LaunchSimulationStepPlan[] steps)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                "Request=" +
                (request?.RequestFingerprint ?? string.Empty));

            if (steps != null)
            {
                for (int index = 0;
                     index < steps.Length;
                     index++)
                {
                    builder.Append("Step[")
                           .Append(index.ToString(
                               CultureInfo.InvariantCulture))
                           .Append("]=")
                           .Append(steps[index]?.ToCanonicalText() ??
                                   "None")
                           .Append('\n');
                }
            }

            return Sha256(builder.ToString());
        }

        internal static string ComputeReport(
            LaunchSimulationReport report)
        {
            if (report == null)
            {
                return Sha256("Report=None");
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                "Schema=" +
                report.SchemaVersion.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Status=" +
                ((int)report.Status).ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Preset=" +
                ((int)report.Preset).ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Request=" + report.RequestFingerprint);
            builder.AppendLine(
                "Plan=" + report.PlanFingerprint);
            builder.AppendLine(
                "Authored=" +
                report.AuthoredEntryCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Disabled=" +
                report.DisabledEntryCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Attempted=" +
                report.AttemptedEntryCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Unvisited=" +
                report.UnvisitedEntryCount.ToString(
                    CultureInfo.InvariantCulture));
            builder.AppendLine(
                "Cancelled=" + report.WasCancelled);
            builder.AppendLine(
                "Diagnostic=" + report.DiagnosticCode);
            builder.AppendLine(
                "DiagnosticMessage=" + report.DiagnosticMessage);
            builder.AppendLine(
                "DiagnosticDetails=" + report.DiagnosticDetails);

            for (int index = 0;
                 index < report.StepCount;
                 index++)
            {
                builder.Append("Step[")
                       .Append(index.ToString(
                           CultureInfo.InvariantCulture))
                       .Append("]=")
                       .Append(report.GetStep(index)
                                     .ToCanonicalText())
                       .Append('\n');
            }

            for (int index = 0;
                 index < report.ProgressSampleCount;
                 index++)
            {
                builder.Append("Progress[")
                       .Append(index.ToString(
                           CultureInfo.InvariantCulture))
                       .Append("]=")
                       .Append(report.GetProgressSample(index)
                                     .ToCanonicalText())
                       .Append('\n');
            }

            return Sha256(builder.ToString());
        }

        internal static string StableId(
            string seed)
        {
            return Sha256(seed ?? string.Empty)
                .Substring(0, 32);
        }

        internal static string Sha256(
            string value)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(value ?? string.Empty);

            using (SHA256 algorithm = SHA256.Create())
            {
                byte[] hash = algorithm.ComputeHash(bytes);
                StringBuilder builder =
                    new StringBuilder(hash.Length * 2);

                for (int index = 0;
                     index < hash.Length;
                     index++)
                {
                    builder.Append(
                        hash[index].ToString(
                            "x2",
                            CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }
    }
}
