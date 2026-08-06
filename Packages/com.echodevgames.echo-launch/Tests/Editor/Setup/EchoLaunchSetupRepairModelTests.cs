using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupRepairModelTests
    {
        [Test]
        public void RepairOperationCarriesBeforeAfterAndProof()
        {
            EchoLaunchSetupOperation operation =
                new EchoLaunchSetupOperation(
                    "repair.configuration",
                    20,
                    EchoLaunchSetupOperationKind.ResolveConfiguration,
                    EchoLaunchSetupOperationDisposition.Repair,
                    "Assets/Configuration.asset",
                    "Rebind references.",
                    EchoLaunchSetupDiagnosticCodes.RepairApprovalRequired,
                    false,
                    "Before",
                    "After",
                    "Proof");

            Assert.That(operation.ExistingState, Is.EqualTo("Before"));
            Assert.That(operation.ProposedState, Is.EqualTo("After"));
            Assert.That(operation.ProofSummary, Is.EqualTo("Proof"));
        }

        [Test]
        public void PlanReportsRepairAndCreatePresence()
        {
            EchoLaunchSetupPlan plan =
                new EchoLaunchSetupPlan(
                    EchoLaunchSetupTestFactory.CreateRequest(),
                    EchoLaunchSetupPathSet.CreateDefault(),
                    "Evidence",
                    EchoLaunchSetupPlanStatus.Ready,
                    new[]
                    {
                        new EchoLaunchSetupOperation(
                            "repair",
                            20,
                            EchoLaunchSetupOperationKind.ResolveConfiguration,
                            EchoLaunchSetupOperationDisposition.Repair,
                            "Assets/Configuration.asset",
                            "Repair"),
                        new EchoLaunchSetupOperation(
                            "create",
                            20,
                            EchoLaunchSetupOperationKind.ResolveStartupSequence,
                            EchoLaunchSetupOperationDisposition.Create,
                            "Assets/Sequence.asset",
                            "Create")
                    },
                    null);

            Assert.That(plan.HasRepairs, Is.True);
            Assert.That(plan.HasCreates, Is.True);
        }

        [Test]
        public void RepairResultCopiesPathCollections()
        {
            string[] repaired = { "Assets/B.asset", "Assets/A.asset" };
            EchoLaunchSetupRepairResult result =
                new EchoLaunchSetupRepairResult(
                    EchoLaunchSetupRepairStatus.Succeeded,
                    string.Empty,
                    "Done",
                    null,
                    null,
                    repaired,
                    null,
                    "Library/Backup",
                    "Before",
                    "After",
                    false,
                    null,
                    EchoLaunchSetupPlanStatus.Ready,
                    "fingerprint");

            repaired[0] = "Assets/Changed.asset";
            Assert.That(result.RepairedPaths[0], Is.EqualTo("Assets/A.asset"));
            Assert.That(result.RepairedPaths[1], Is.EqualTo("Assets/B.asset"));
        }
    }
}
