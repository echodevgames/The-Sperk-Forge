using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Validation;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Validation
{
    public sealed class EchoLaunchValidationContractTests
    {
        [Test]
        public void ReportDefensivelyCopiesFindings()
        {
            List<EchoLaunchValidationFinding> source =
                new List<EchoLaunchValidationFinding>
                {
                    Finding(
                        EchoLaunchValidationSeverity.Warning,
                        "ELAUNCH-VAL-010")
                };

            EchoLaunchValidationReport report =
                new EchoLaunchValidationReport(
                    EchoLaunchValidationRequest.CreateDefault(),
                    "request",
                    "evidence",
                    source);

            source.Clear();

            Assert.That(report.FindingCount, Is.EqualTo(1));
            Assert.That(report.WarningCount, Is.EqualTo(1));
        }

        [Test]
        public void HighestSeverityDerivesHealth()
        {
            AssertHealth(
                EchoLaunchValidationSeverity.Information,
                EchoLaunchProjectHealth.Healthy);

            AssertHealth(
                EchoLaunchValidationSeverity.Warning,
                EchoLaunchProjectHealth.NeedsAttention);

            AssertHealth(
                EchoLaunchValidationSeverity.Error,
                EchoLaunchProjectHealth.Invalid);

            AssertHealth(
                EchoLaunchValidationSeverity.Blocker,
                EchoLaunchProjectHealth.Blocked);
        }

        [Test]
        public void FindingRejectsAbsolutePath()
        {
            EchoLaunchValidationFinding finding =
                new EchoLaunchValidationFinding(
                    "ELAUNCH-VAL-014",
                    EchoLaunchValidationSeverity.Blocker,
                    "Title",
                    "Message",
                    @"C:\Secret\Project",
                    "Evidence",
                    "Action");

            Assert.That(finding.ProjectPath, Is.Empty);
        }

        private static void AssertHealth(
            EchoLaunchValidationSeverity severity,
            EchoLaunchProjectHealth expected)
        {
            EchoLaunchValidationReport report =
                new EchoLaunchValidationReport(
                    EchoLaunchValidationRequest.CreateDefault(),
                    "request",
                    "evidence",
                    new[] { Finding(severity, "ELAUNCH-VAL-001") });

            Assert.That(report.Health, Is.EqualTo(expected));
        }

        private static EchoLaunchValidationFinding Finding(
            EchoLaunchValidationSeverity severity,
            string code)
        {
            return new EchoLaunchValidationFinding(
                code,
                severity,
                "Title",
                "Message",
                "Assets/FirstLight",
                "Evidence",
                "Action");
        }
    }
}
