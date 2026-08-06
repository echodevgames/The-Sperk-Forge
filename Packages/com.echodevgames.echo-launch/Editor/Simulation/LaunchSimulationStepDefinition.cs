using System;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationStepDefinition :
        StartupStepDefinition
    {
        [NonSerialized]
        private LaunchSimulationStepPlan plan;

        internal void Configure(
            LaunchSimulationStepPlan configuredPlan)
        {
            plan =
                configuredPlan ??
                throw new ArgumentNullException(
                    nameof(configuredPlan));
        }

        public override IStartupStepExecutor CreateExecutor()
        {
            if (plan == null)
            {
                throw new InvalidOperationException(
                    "The transient Launch Simulator step definition was not configured.");
            }

            return new LaunchSimulationStepExecutor(plan);
        }
    }
}
