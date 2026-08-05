using System.Text;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupApplyResultFormatter
    {
        internal string Format(EchoLaunchSetupApplyResult result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("First Light Setup Apply Result");
            builder.AppendLine("Status: " + result.Status);

            if (!string.IsNullOrEmpty(result.DiagnosticCode))
            {
                builder.AppendLine(
                    "Diagnostic: " + result.DiagnosticCode);
            }

            builder.AppendLine("Message: " + result.Message);
            builder.AppendLine(
                "Rollback completed: " +
                (result.RollbackCompleted ? "Yes" : "No"));

            builder.AppendLine(
                "Final plan status: " +
                (result.FinalPlanStatus.HasValue
                    ? result.FinalPlanStatus.Value.ToString()
                    : "Unavailable"));

            builder.AppendLine(
                "Final plan fingerprint: " +
                result.FinalPlanFingerprint);

            builder.AppendLine();
            builder.AppendLine("Created paths:");

            if (result.CreatedPaths.Count == 0)
            {
                builder.AppendLine("None.");
            }
            else
            {
                for (int index = 0;
                     index < result.CreatedPaths.Count;
                     index++)
                {
                    builder.AppendLine("- " + result.CreatedPaths[index]);
                }
            }

            builder.AppendLine();
            builder.AppendLine("Reused paths:");

            if (result.ReusedPaths.Count == 0)
            {
                builder.AppendLine("None.");
            }
            else
            {
                for (int index = 0;
                     index < result.ReusedPaths.Count;
                     index++)
                {
                    builder.AppendLine("- " + result.ReusedPaths[index]);
                }
            }

            builder.AppendLine();
            builder.AppendLine(
                "Build Settings before: " +
                result.BuildSettingsBefore);

            builder.AppendLine(
                "Build Settings after: " +
                result.BuildSettingsAfter);

            builder.AppendLine();
            builder.AppendLine("Manual recovery paths:");

            if (result.ManualRecoveryPaths.Count == 0)
            {
                builder.AppendLine("None.");
            }
            else
            {
                for (int index = 0;
                     index < result.ManualRecoveryPaths.Count;
                     index++)
                {
                    builder.AppendLine(
                        "- " +
                        result.ManualRecoveryPaths[index]);
                }
            }

            return builder.ToString();
        }
    }
}
