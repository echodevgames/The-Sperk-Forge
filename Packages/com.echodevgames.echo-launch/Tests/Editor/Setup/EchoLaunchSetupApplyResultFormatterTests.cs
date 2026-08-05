using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupApplyResultFormatterTests
    {
        [Test]
        public void NullResultFormatsEmpty()
        {
            Assert.That(
                new EchoLaunchSetupApplyResultFormatter().Format(null),
                Is.Empty);
        }

        [Test]
        public void ReportIncludesStatus()
        {
            Assert.That(
                Format(CreateResult()),
                Does.Contain("Status: Succeeded"));
        }

        [Test]
        public void ReportIncludesDiagnostic()
        {
            EchoLaunchSetupApplyResult result =
                new EchoLaunchSetupApplyResult(
                    EchoLaunchSetupApplyStatus.StalePlan,
                    EchoLaunchSetupDiagnosticCodes.StalePlan,
                    "Refresh.",
                    null,
                    null,
                    null,
                    "Before",
                    "After",
                    false,
                    null,
                    EchoLaunchSetupPlanStatus.Ready,
                    "fingerprint");

            Assert.That(
                Format(result),
                Does.Contain(EchoLaunchSetupDiagnosticCodes.StalePlan));
        }

        [Test]
        public void ReportIncludesCreatedPaths()
        {
            Assert.That(
                Format(CreateResult()),
                Does.Contain("Assets/Created.asset"));
        }

        [Test]
        public void ReportIncludesReusedPaths()
        {
            Assert.That(
                Format(CreateResult()),
                Does.Contain("Assets/Reused.asset"));
        }

        [Test]
        public void ReportIncludesBuildSettingsSummaries()
        {
            string report = Format(CreateResult());

            Assert.That(report, Does.Contain("Before"));
            Assert.That(report, Does.Contain("After"));
        }

        [Test]
        public void ReportIncludesManualRecoveryPaths()
        {
            EchoLaunchSetupApplyResult result =
                new EchoLaunchSetupApplyResult(
                    EchoLaunchSetupApplyStatus.FailedRollbackIncomplete,
                    EchoLaunchSetupDiagnosticCodes.RollbackIncomplete,
                    "Recover.",
                    null,
                    null,
                    null,
                    "Before",
                    "After",
                    false,
                    new[] { "Assets/Recover.asset" },
                    null,
                    string.Empty);

            Assert.That(
                Format(result),
                Does.Contain("Assets/Recover.asset"));
        }

        [Test]
        public void ReportIsDeterministic()
        {
            EchoLaunchSetupApplyResult result = CreateResult();
            EchoLaunchSetupApplyResultFormatter formatter =
                new EchoLaunchSetupApplyResultFormatter();

            Assert.That(
                formatter.Format(result),
                Is.EqualTo(formatter.Format(result)));
        }

        private static string Format(
            EchoLaunchSetupApplyResult result)
        {
            return new EchoLaunchSetupApplyResultFormatter()
                .Format(result);
        }

        private static EchoLaunchSetupApplyResult CreateResult()
        {
            return new EchoLaunchSetupApplyResult(
                EchoLaunchSetupApplyStatus.Succeeded,
                string.Empty,
                "Done.",
                new[]
                {
                    new EchoLaunchSetupChange(
                        EchoLaunchSetupChangeKind.CreatedAsset,
                        "Assets/Created.asset",
                        "Created.")
                },
                new[] { "Assets/Created.asset" },
                new[] { "Assets/Reused.asset" },
                "Before",
                "After",
                false,
                null,
                EchoLaunchSetupPlanStatus.Ready,
                "fingerprint");
        }
    }
}
