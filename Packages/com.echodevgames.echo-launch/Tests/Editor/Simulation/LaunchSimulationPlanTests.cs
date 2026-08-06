using NUnit.Framework;
using EchoDevGames.EchoLaunch.Editor.Simulation;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Simulation
{
    public sealed class LaunchSimulationPlanTests
    {
        [Test]
        public void PresetsBuildExpectedAuthoredStepCounts()
        {
            LaunchSimulationPreset[] presets =
            {
                LaunchSimulationPreset.ImmediateSuccess,
                LaunchSimulationPreset.TimedProgressSuccess,
                LaunchSimulationPreset.WarningContinues,
                LaunchSimulationPreset.RecoverableFailureContinues,
                LaunchSimulationPreset.BlockingFailureStops,
                LaunchSimulationPreset.TimeoutStops,
                LaunchSimulationPreset.ExecutorExceptionStops,
                LaunchSimulationPreset.Cancellation
            };

            int[] expectedCounts =
            {
                1,
                1,
                2,
                2,
                2,
                2,
                2,
                2
            };

            for (int index = 0;
                 index < presets.Length;
                 index++)
            {
                LaunchSimulationPlan plan =
                    LaunchSimulationPlan.Create(
                        CreateRequest(presets[index]));

                Assert.That(
                    plan.StepCount,
                    Is.EqualTo(expectedCounts[index]),
                    presets[index].ToString());

                Assert.That(
                    plan.PlanFingerprint.Length,
                    Is.EqualTo(64),
                    presets[index].ToString());
            }
        }

        [Test]
        public void RecoverableFailureUsesContinuationPolicy()
        {
            LaunchSimulationPlan plan =
                LaunchSimulationPlan.Create(
                    CreateRequest(
                        LaunchSimulationPreset
                            .RecoverableFailureContinues));

            LaunchSimulationStepPlan first =
                plan.GetStep(0);

            Assert.That(first.IsRequired, Is.False);

            Assert.That(
                first.FailureAction,
                Is.EqualTo(
                    StartupStepFailureAction
                        .ContinueWithWarning));
        }

        [Test]
        public void BlockingFailureHasUnvisitedProofStep()
        {
            LaunchSimulationPlan plan =
                LaunchSimulationPlan.Create(
                    CreateRequest(
                        LaunchSimulationPreset
                            .BlockingFailureStops));

            Assert.That(
                plan.GetStep(0).Behavior,
                Is.EqualTo(
                    LaunchSimulationStepBehavior
                        .BlockingFailure));

            Assert.That(
                plan.GetStep(1).DisplayName,
                Does.Contain("Unvisited"));
        }

        private static LaunchSimulationRequest CreateRequest(
            LaunchSimulationPreset preset)
        {
            return new LaunchSimulationRequest(
                preset,
                preset ==
                    LaunchSimulationPreset
                        .TimedProgressSuccess
                    ? 1d
                    : 0d,
                preset ==
                    LaunchSimulationPreset
                        .TimedProgressSuccess
                    ? 4
                    : 0,
                preset ==
                    LaunchSimulationPreset.TimeoutStops
                    ? 0.5d
                    : 0d,
                string.Empty);
        }
    }
}
