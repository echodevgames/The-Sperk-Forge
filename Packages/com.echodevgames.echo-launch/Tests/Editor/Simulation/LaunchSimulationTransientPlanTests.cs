using NUnit.Framework;
using EchoDevGames.EchoLaunch.Editor.Simulation;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Simulation
{
    public sealed class LaunchSimulationTransientPlanTests
    {
        [Test]
        public void BuildCreatesValidSequenceAndDisposesAllObjects()
        {
            int before =
                LaunchSimulationTransientPlanBuilder.LiveObjectCount;

            LaunchSimulationPlan plan =
                LaunchSimulationPlan.Create(
                    new LaunchSimulationRequest(
                        LaunchSimulationPreset.WarningContinues,
                        0d,
                        0,
                        0d,
                        string.Empty));

            LaunchSimulationTransientPlan transient =
                LaunchSimulationTransientPlanBuilder.Build(plan);

            Assert.That(
                transient.Configuration.StartupSequence,
                Is.SameAs(transient.Sequence));

            Assert.That(
                transient.Sequence.EntryCount,
                Is.EqualTo(2));

            Assert.That(
                LaunchSimulationTransientPlanBuilder.LiveObjectCount,
                Is.GreaterThan(before));

            transient.Dispose();
            transient.Dispose();

            Assert.That(
                LaunchSimulationTransientPlanBuilder.LiveObjectCount,
                Is.EqualTo(before));
        }

        [Test]
        public void TransientObjectsUseDeterministicIds()
        {
            LaunchSimulationRequest request =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset.ImmediateSuccess,
                    0d,
                    0,
                    0d,
                    string.Empty);

            LaunchSimulationPlan plan =
                LaunchSimulationPlan.Create(request);

            using (
                LaunchSimulationTransientPlan transient =
                    LaunchSimulationTransientPlanBuilder.Build(
                        plan))
            {
                Assert.That(
                    transient.Configuration.ConfigurationId,
                    Is.EqualTo(
                        LaunchSimulationFingerprint.StableId(
                            request.RequestFingerprint +
                            "|Configuration")));

                Assert.That(
                    transient.Sequence.SequenceId,
                    Is.EqualTo(
                        LaunchSimulationFingerprint.StableId(
                            request.RequestFingerprint +
                            "|Sequence")));
            }
        }
    }
}
