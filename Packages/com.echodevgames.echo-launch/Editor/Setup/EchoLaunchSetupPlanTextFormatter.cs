
using System.Text;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupPlanTextFormatter
    {
        internal string Format(EchoLaunchSetupPlan plan)
        {
            if (plan == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("First Light Setup Plan");
            builder.AppendLine("Status: " + plan.Status);
            builder.AppendLine(
                "Preview only: no project changes have been applied.");
            builder.AppendLine("Evidence: " + plan.SnapshotEvidenceSummary);
            builder.AppendLine("Request fingerprint: " + plan.RequestFingerprint);
            builder.AppendLine("Evidence fingerprint: " + plan.EvidenceFingerprint);
            builder.AppendLine("Plan fingerprint: " + plan.PlanFingerprint);

            if (plan.Paths != null)
            {
                builder.AppendLine(
                    "Project root: " + plan.Paths.ProjectRootPath);
                builder.AppendLine(
                    "Boot scene: " + plan.Paths.BootScenePath);
            }

            builder.AppendLine(
                "Foundation asset resolution: " +
                FormatFoundationResolution(
                    plan.Request.FoundationResolutionPolicy));

            if (plan.Request.SplashAuthoring != null)
            {
                builder.AppendLine(
                    "Splash creation authoring: " +
                    plan.Request.SplashAuthoring.Summary);
            }

            builder.AppendLine();
            builder.AppendLine("Operations:");

            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];

                builder.Append(index + 1);
                builder.Append(". [");
                builder.Append(operation.Disposition);
                builder.Append("] ");
                builder.Append(operation.Kind);
                builder.Append(" | ");
                builder.Append(operation.TargetPath);
                builder.Append(" | ");
                builder.Append(operation.Reason);

                if (!string.IsNullOrEmpty(operation.DiagnosticCode))
                {
                    builder.Append(" | ");
                    builder.Append(operation.DiagnosticCode);
                }

                if (operation.RequiresExplicitApproval)
                {
                    builder.Append(" | APPROVAL REQUIRED");
                }

                builder.AppendLine();

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Repair)
                {
                    builder.AppendLine("   Before: " + operation.ExistingState);
                    builder.AppendLine("   After: " + operation.ProposedState);
                    builder.AppendLine("   Proof: " + operation.ProofSummary);
                }
            }

            builder.AppendLine();
            builder.AppendLine("Diagnostics:");

            if (plan.Diagnostics.Count == 0)
            {
                builder.AppendLine("None.");
            }
            else
            {
                for (int index = 0; index < plan.Diagnostics.Count; index++)
                {
                    EchoLaunchSetupDiagnostic diagnostic =
                        plan.Diagnostics[index];

                    builder.Append("- [");
                    builder.Append(diagnostic.Severity);
                    builder.Append("] ");
                    builder.Append(diagnostic.Code);
                    builder.Append(": ");
                    builder.Append(diagnostic.Message);

                    if (!string.IsNullOrEmpty(diagnostic.TargetPath))
                    {
                        builder.Append(" (");
                        builder.Append(diagnostic.TargetPath);
                        builder.Append(")");
                    }

                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        private static string FormatFoundationResolution(
            EchoLaunchSetupFoundationResolutionPolicy policy)
        {
            switch (policy)
            {
                case EchoLaunchSetupFoundationResolutionPolicy
                    .CreateProjectOwnedSetup:
                    return "Create Project-Owned Setup";

                default:
                    return "Reuse Compatible Assets";
            }
        }
    }
}
