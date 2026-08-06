using System;
using EchoDevGames.EchoLaunch.Editor.Validation;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Validation
{
    public sealed class EchoLaunchValidationServiceTests
    {
        [TearDown]
        public void TearDown()
        {
            EchoLaunchValidationService.SetValidationActiveForTests(false);
        }

        [Test]
        public void ReentryReturns015WithoutScanning()
        {
            RecordingSource source = new RecordingSource();

            EchoLaunchValidationService service =
                new EchoLaunchValidationService(source);

            EchoLaunchValidationService.SetValidationActiveForTests(true);

            EchoLaunchValidationReport report =
                service.Validate(
                    EchoLaunchValidationRequest.CreateDefault());

            Assert.That(source.CallCount, Is.Zero);

            Assert.That(
                EchoLaunchValidationTestFactory.HasCode(
                    report.Findings,
                    EchoLaunchValidationDiagnosticCodes.AlreadyRunning),
                Is.True);
        }

        [Test]
        public void EvidenceExceptionBecomes014()
        {
            EchoLaunchValidationService service =
                new EchoLaunchValidationService(
                    new ThrowingSource());

            EchoLaunchValidationReport report =
                service.Validate(
                    EchoLaunchValidationRequest.CreateDefault());

            Assert.That(
                report.Health,
                Is.EqualTo(EchoLaunchProjectHealth.Blocked));

            Assert.That(
                EchoLaunchValidationTestFactory.HasCode(
                    report.Findings,
                    EchoLaunchValidationDiagnosticCodes.EvidenceUnavailable),
                Is.True);
        }

        [Test]
        public void HealthyEvidenceReturnsHealthy()
        {
            EchoLaunchValidationService service =
                new EchoLaunchValidationService(
                    new FixedSource(
                        EchoLaunchValidationTestFactory
                            .CreateHealthyEvidence()));

            EchoLaunchValidationReport report =
                service.Validate(
                    EchoLaunchValidationRequest.CreateDefault());

            Assert.That(
                report.Health,
                Is.EqualTo(EchoLaunchProjectHealth.Healthy));

            Assert.That(report.FindingCount, Is.Zero);
        }

        private sealed class RecordingSource :
            IEchoLaunchValidationEvidenceSource
        {
            internal int CallCount { get; private set; }

            public EchoLaunchValidationEvidence Collect(
                EchoLaunchValidationRequest request)
            {
                CallCount++;
                return EchoLaunchValidationTestFactory
                    .CreateHealthyEvidence();
            }
        }

        private sealed class ThrowingSource :
            IEchoLaunchValidationEvidenceSource
        {
            public EchoLaunchValidationEvidence Collect(
                EchoLaunchValidationRequest request)
            {
                throw new InvalidOperationException(
                    @"Do not leak C:\Secret\Project.");
            }
        }

        private sealed class FixedSource :
            IEchoLaunchValidationEvidenceSource
        {
            private readonly EchoLaunchValidationEvidence evidence;

            internal FixedSource(
                EchoLaunchValidationEvidence evidence)
            {
                this.evidence = evidence;
            }

            public EchoLaunchValidationEvidence Collect(
                EchoLaunchValidationRequest request)
            {
                return evidence;
            }
        }
    }
}
