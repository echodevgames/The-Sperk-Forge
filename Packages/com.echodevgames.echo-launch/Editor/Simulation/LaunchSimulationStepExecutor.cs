using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationStepExecutor :
        IStartupStepExecutor
    {
        private readonly LaunchSimulationStepPlan plan;

        internal LaunchSimulationStepExecutor(
            LaunchSimulationStepPlan plan)
        {
            this.plan =
                plan ??
                throw new ArgumentNullException(
                    nameof(plan));
        }

        public async Awaitable<StartupStepResult> ExecuteAsync(
            StartupStepContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            switch (plan.Behavior)
            {
                case LaunchSimulationStepBehavior.Success:
                    await LaunchSimulationEditorTick.NextAsync(
                        context.CancellationToken);

                    context.ProgressReporter.Report(
                        StartupStepProgress.Determinate(
                            1f,
                            "Simulation step completed."));

                    return StartupStepResult.Success(
                        plan.Message);

                case LaunchSimulationStepBehavior
                    .TimedProgressSuccess:
                    return await ExecuteTimedProgressAsync(context);

                case LaunchSimulationStepBehavior.Warning:
                    await LaunchSimulationEditorTick.NextAsync(
                        context.CancellationToken);

                    return StartupStepResult.Warning(
                        LaunchSimulationDiagnosticCodes
                            .SimulatedWarning,
                        plan.Message);

                case LaunchSimulationStepBehavior
                    .RecoverableFailure:
                    await LaunchSimulationEditorTick.NextAsync(
                        context.CancellationToken);

                    return StartupStepResult.RecoverableFailure(
                        LaunchSimulationDiagnosticCodes
                            .SimulatedRecoverableFailure,
                        plan.Message);

                case LaunchSimulationStepBehavior
                    .BlockingFailure:
                    await LaunchSimulationEditorTick.NextAsync(
                        context.CancellationToken);

                    return StartupStepResult.BlockingFailure(
                        LaunchSimulationDiagnosticCodes
                            .SimulatedBlockingFailure,
                        plan.Message);

                case LaunchSimulationStepBehavior.WaitForTimeout:
                case LaunchSimulationStepBehavior
                    .WaitForCancellation:
                    return await WaitForCancellationAsync(context);

                case LaunchSimulationStepBehavior.ThrowException:
                    await LaunchSimulationEditorTick.NextAsync(
                        context.CancellationToken);

                    throw new InvalidOperationException(
                        "Simulated Launch Simulator executor exception.");

                default:
                    throw new InvalidOperationException(
                        "The transient Launch Simulator step behavior is unsupported.");
            }
        }

        private async Awaitable<StartupStepResult>
            ExecuteTimedProgressAsync(
                StartupStepContext context)
        {
            for (int sampleIndex = 1;
                 sampleIndex <= plan.ProgressSampleCount;
                 sampleIndex++)
            {
                await LaunchSimulationEditorTick.NextAsync(
                    context.CancellationToken);

                float progress =
                    (float)sampleIndex /
                    plan.ProgressSampleCount;

                context.ProgressReporter.Report(
                    StartupStepProgress.Determinate(
                        progress,
                        "Logical progress sample " +
                        sampleIndex +
                        " of " +
                        plan.ProgressSampleCount +
                        "."));
            }

            return StartupStepResult.Success(
                plan.Message);
        }

        private static async Awaitable<StartupStepResult>
            WaitForCancellationAsync(
                StartupStepContext context)
        {
            context.ProgressReporter.Report(
                StartupStepProgress.Indeterminate(
                    "Awaiting cancellation."));

            while (true)
            {
                await LaunchSimulationEditorTick.NextAsync(
                    context.CancellationToken);
            }
        }
    }
}
