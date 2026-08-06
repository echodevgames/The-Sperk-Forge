using System;
using NUnit.Framework;
using EchoDevGames.EchoLaunch.Editor.Simulation;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Simulation
{
    public sealed class LaunchSimulationReportTests
    {
        [Test]
        public void EmptyInvalidReportHasDeterministicText()
        {
            LaunchSimulationRequest request =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.ImmediateSuccess,
                    0d,
                    0,
                    0d,
                    string.Empty);

            LaunchSimulationReport first =
                CreateInvalidReport(request);

            LaunchSimulationReport second =
                CreateInvalidReport(request);

            Assert.That(
                second.ReportFingerprint,
                Is.EqualTo(first.ReportFingerprint));

            Assert.That(
                second.Text,
                Is.EqualTo(first.Text));

            Assert.That(
                first.Text,
                Does.Contain("ELAUNCH-SIM-001"));
        }

        [Test]
        public void OutOfRangeEvidenceAccessIsRejected()
        {
            LaunchSimulationRequest request =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.ImmediateSuccess,
                    0d,
                    0,
                    0d,
                    string.Empty);

            LaunchSimulationReport report =
                CreateInvalidReport(request);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => report.GetStep(0));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => report.GetProgressSample(0));
        }

        private static LaunchSimulationReport CreateInvalidReport(
            LaunchSimulationRequest request)
        {
            return new LaunchSimulationReport(
                LaunchSimulationStatus.InvalidRequest,
                request,
                string.Empty,
                0,
                0,
                0,
                false,
                Array.Empty<LaunchSimulationStepReport>(),
                Array.Empty<LaunchSimulationProgressSample>(),
                LaunchSimulationDiagnosticCodes.InvalidRequest,
                "Invalid.",
                string.Empty);
        }
    }
}
