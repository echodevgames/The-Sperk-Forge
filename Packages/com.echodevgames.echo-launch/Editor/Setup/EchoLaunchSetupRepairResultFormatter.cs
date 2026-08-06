using System.Text;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupRepairResultFormatter
    {
        internal string Format(EchoLaunchSetupRepairResult result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("First Light Setup Repair Result");
            builder.AppendLine("Status: " + result.Status);
            builder.AppendLine("Message: " + result.Message);
            if (!string.IsNullOrEmpty(result.DiagnosticCode))
            {
                builder.AppendLine("Diagnostic: " + result.DiagnosticCode);
            }
            builder.AppendLine(
                "Rollback completed: " +
                (result.RollbackCompleted ? "Yes" : "No"));
            builder.AppendLine(
                "Final plan status: " +
                (result.FinalPlanStatus.HasValue
                    ? result.FinalPlanStatus.Value.ToString()
                    : "Unavailable"));
            builder.AppendLine(
                "Final plan fingerprint: " + result.FinalPlanFingerprint);
            builder.AppendLine(
                "Backup directory: " +
                (string.IsNullOrEmpty(result.BackupDirectory)
                    ? "None"
                    : result.BackupDirectory));

            AppendPaths(builder, "Created paths", result.CreatedPaths);
            AppendPaths(builder, "Repaired paths", result.RepairedPaths);
            AppendPaths(builder, "Reused paths", result.ReusedPaths);
            AppendPaths(builder, "Unchanged paths", result.UnchangedPaths);
            AppendRepairSummaries(builder, result.RepairOperations);
            builder.AppendLine();
            builder.AppendLine(
                "Build Settings before: " + result.BuildSettingsBefore);
            builder.AppendLine(
                "Build Settings after: " + result.BuildSettingsAfter);
            AppendPaths(
                builder,
                "Manual recovery paths",
                result.ManualRecoveryPaths);
            return builder.ToString();
        }

        private static void AppendRepairSummaries(
            StringBuilder builder,
            System.Collections.Generic.IReadOnlyList<
                EchoLaunchSetupOperation> operations)
        {
            builder.AppendLine();
            builder.AppendLine("Approved repair summaries:");
            if (operations == null || operations.Count == 0)
            {
                builder.AppendLine("None.");
                return;
            }

            for (int index = 0; index < operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = operations[index];
                builder.AppendLine("- " + operation.TargetPath);
                builder.AppendLine("  Before: " + operation.ExistingState);
                builder.AppendLine("  After: " + operation.ProposedState);
                builder.AppendLine("  Proof: " + operation.ProofSummary);
            }
        }

        private static void AppendPaths(
            StringBuilder builder,
            string heading,
            System.Collections.Generic.IReadOnlyList<string> paths)
        {
            builder.AppendLine();
            builder.AppendLine(heading + ":");
            if (paths == null || paths.Count == 0)
            {
                builder.AppendLine("None.");
                return;
            }

            for (int index = 0; index < paths.Count; index++)
            {
                builder.AppendLine("- " + paths[index]);
            }
        }
    }
}
