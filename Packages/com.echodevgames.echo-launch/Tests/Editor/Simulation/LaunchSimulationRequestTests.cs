using NUnit.Framework;
using EchoDevGames.EchoLaunch.Editor.Simulation;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Simulation
{
    public sealed class LaunchSimulationRequestTests
    {
        [Test]
        public void DefaultImmediateRequestIsValid()
        {
            LaunchSimulationRequest request =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.ImmediateSuccess,
                    0d,
                    0,
                    0d,
                    "  hello  ");

            string message;

            Assert.That(
                request.TryValidate(out message),
                Is.True);

            Assert.That(
                request.Message,
                Is.EqualTo("hello"));

            Assert.That(
                request.RequestFingerprint.Length,
                Is.EqualTo(64));
        }

        [Test]
        public void TimedProgressRequiresDurationAndSamples()
        {
            LaunchSimulationRequest request =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.TimedProgressSuccess,
                    0d,
                    0,
                    0d,
                    string.Empty);

            string message;

            Assert.That(
                request.TryValidate(out message),
                Is.False);

            Assert.That(message, Does.Contain("requires"));
        }

        [Test]
        public void TimeoutPresetRequiresPositiveTimeout()
        {
            LaunchSimulationRequest request =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.TimeoutStops,
                    0d,
                    0,
                    0d,
                    string.Empty);

            string message;

            Assert.That(
                request.TryValidate(out message),
                Is.False);

            Assert.That(
                message,
                Does.Contain("positive timeout"));
        }

        [Test]
        public void SameRequestProducesSameFingerprint()
        {
            LaunchSimulationRequest first =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.WarningContinues,
                    0d,
                    0,
                    0d,
                    "warning");

            LaunchSimulationRequest second =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.WarningContinues,
                    0d,
                    0,
                    0d,
                    "warning");

            Assert.That(
                second.RequestFingerprint,
                Is.EqualTo(first.RequestFingerprint));
        }

        [Test]
        public void ChangedParameterChangesFingerprint()
        {
            LaunchSimulationRequest first =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.TimedProgressSuccess,
                    1d,
                    4,
                    0d,
                    string.Empty);

            LaunchSimulationRequest second =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.TimedProgressSuccess,
                    2d,
                    4,
                    0d,
                    string.Empty);

            Assert.That(
                second.RequestFingerprint,
                Is.Not.EqualTo(first.RequestFingerprint));
        }
    }
}
