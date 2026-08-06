using EchoDevGames.EchoLaunch.Editor.Validation;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Validation
{
    public sealed class EchoLaunchValidationFingerprintTests
    {
        [Test]
        public void SameRequestProducesSameFingerprint()
        {
            EchoLaunchValidationRequest first =
                EchoLaunchValidationRequest.CreateDefault();

            EchoLaunchValidationRequest second =
                EchoLaunchValidationRequest.CreateDefault();

            Assert.That(
                EchoLaunchValidationFingerprint.ForRequest(first),
                Is.EqualTo(
                    EchoLaunchValidationFingerprint.ForRequest(second)));
        }

        [Test]
        public void RequestOptionChangesFingerprint()
        {
            EchoLaunchValidationRequest first =
                new EchoLaunchValidationRequest(
                    "Assets/EchoDevGames/FirstLight",
                    true);

            EchoLaunchValidationRequest second =
                new EchoLaunchValidationRequest(
                    "Assets/EchoDevGames/FirstLight",
                    false);

            Assert.That(
                EchoLaunchValidationFingerprint.ForRequest(first),
                Is.Not.EqualTo(
                    EchoLaunchValidationFingerprint.ForRequest(second)));
        }

        [Test]
        public void SameEvidenceProducesSameFingerprint()
        {
            string first =
                EchoLaunchValidationFingerprint.ForEvidence(
                    EchoLaunchValidationTestFactory.CreateHealthyEvidence());

            string second =
                EchoLaunchValidationFingerprint.ForEvidence(
                    EchoLaunchValidationTestFactory.CreateHealthyEvidence());

            Assert.That(first, Is.EqualTo(second));
        }

        [Test]
        public void SameReportProducesSameFingerprint()
        {
            EchoLaunchValidationEvidence evidence =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationReport first =
                new EchoLaunchValidationReport(
                    evidence.Request,
                    EchoLaunchValidationFingerprint.ForRequest(
                        evidence.Request),
                    evidence.EvidenceFingerprint,
                    EchoLaunchValidationRuleCatalog.Evaluate(evidence));

            EchoLaunchValidationReport second =
                new EchoLaunchValidationReport(
                    evidence.Request,
                    EchoLaunchValidationFingerprint.ForRequest(
                        evidence.Request),
                    evidence.EvidenceFingerprint,
                    EchoLaunchValidationRuleCatalog.Evaluate(evidence));

            Assert.That(
                first.ReportFingerprint,
                Is.EqualTo(second.ReportFingerprint));
        }
    }
}
