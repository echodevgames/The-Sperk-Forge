using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    /// <summary>
    /// Provides a multi-frame determinate-progress proof without storing mutable state in the shared definition.
    /// </summary>
    [CreateAssetMenu(
        menuName = "EchoDevGames/First Light Samples/Laboratory Timed Progress Step",
        fileName = "LaboratoryTimedProgressStep")]
    public sealed class LaboratoryTimedProgressStep : StartupStepDefinition
    {
        public override IStartupStepExecutor CreateExecutor()
        {
            return new Executor();
        }

        private sealed class Executor : IStartupStepExecutor
        {
            public async Awaitable<StartupStepResult> ExecuteAsync(
                StartupStepContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                const int sampleCount = 4;

                for (int sampleIndex = 1;
                    sampleIndex <= sampleCount;
                    sampleIndex++)
                {
                    await Awaitable.NextFrameAsync(
                        context.CancellationToken);

                    context.ProgressReporter.Report(
                        StartupStepProgress.Determinate(
                            sampleIndex / (float)sampleCount,
                            $"Laboratory progress sample {sampleIndex} of {sampleCount}."));
                }

                return StartupStepResult.Success(
                    "Laboratory timed-progress step completed.");
            }
        }
    }
}
