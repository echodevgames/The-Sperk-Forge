using EchoDevGames.EchoLaunch.Editor.Validation;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Validation
{
    public sealed class EchoLaunchValidationTextFormatterTests
    {
        [Test]
        public void HealthyReportFormatsDeterministically()
        {
            EchoLaunchValidationEvidence evidence =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationReport report =
                new EchoLaunchValidationReport(
                    evidence.Request,
                    EchoLaunchValidationFingerprint.ForRequest(
                        evidence.Request),
                    evidence.EvidenceFingerprint,
                    EchoLaunchValidationRuleCatalog.Evaluate(evidence));

            string first =
                EchoLaunchValidationTextFormatter.Format(report);

            string second =
                EchoLaunchValidationTextFormatter.Format(report);

            Assert.That(first, Is.EqualTo(second));
            Assert.That(first, Does.Contain("Health: Healthy"));
            Assert.That(first, Does.Contain(report.ReportFingerprint));
        }

        [Test]
        public void ReportContainsNoAbsoluteMachinePath()
        {
            EchoLaunchValidationReport report =
                new EchoLaunchValidationReport(
                    EchoLaunchValidationRequest.CreateDefault(),
                    "request",
                    "evidence",
                    new[]
                    {
                        new EchoLaunchValidationFinding(
                            EchoLaunchValidationDiagnosticCodes
                                .EvidenceUnavailable,
                            EchoLaunchValidationSeverity.Blocker,
                            "Evidence failed",
                            "The read-only operation failed.",
                            @"C:\Users\Jesse\Secret",
                            "ExceptionType=IOException.",
                            "Try again.")
                    });

            string text =
                EchoLaunchValidationTextFormatter.Format(report);

            Assert.That(text, Does.Not.Contain(@"C:\Users"));
        }
    }
}
