using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupRepairResultFormatterTests
    {
        [Test]
        public void FormatIncludesStatusBackupAndRepairPaths()
        {
            EchoLaunchSetupRepairResult result =
                new EchoLaunchSetupRepairResult(
                    EchoLaunchSetupRepairStatus.Succeeded,
                    string.Empty,
                    "Done",
                    null,
                    null,
                    new[] { "Assets/Repaired.asset" },
                    null,
                    "Library/EchoDevGames/FirstLight/RepairBackups/id",
                    "Before",
                    "After",
                    false,
                    null,
                    EchoLaunchSetupPlanStatus.Ready,
                    "fingerprint",
                    new[] { "Assets/Unchanged.asset" },
                    new[]
                    {
                        new EchoLaunchSetupOperation(
                            "repair",
                            20,
                            EchoLaunchSetupOperationKind.ResolveConfiguration,
                            EchoLaunchSetupOperationDisposition.Repair,
                            "Assets/Repaired.asset",
                            "Repair",
                            EchoLaunchSetupDiagnosticCodes.RepairApprovalRequired,
                            false,
                            "Old",
                            "New",
                            "Verified")
                    });

            string report =
                new EchoLaunchSetupRepairResultFormatter().Format(result);

            Assert.That(report, Does.Contain("Status: Succeeded"));
            Assert.That(report, Does.Contain("Assets/Repaired.asset"));
            Assert.That(report, Does.Contain("RepairBackups/id"));
            Assert.That(report, Does.Contain("Build Settings before: Before"));
            Assert.That(report, Does.Contain("Assets/Unchanged.asset"));
            Assert.That(report, Does.Contain("Before: Old"));
            Assert.That(report, Does.Contain("After: New"));
            Assert.That(report, Does.Contain("Proof: Verified"));
        }

        [Test]
        public void NullResultFormatsAsEmpty()
        {
            Assert.That(
                new EchoLaunchSetupRepairResultFormatter().Format(null),
                Is.Empty);
        }
    }
}
