using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    /// <summary>
    /// Provides the Laboratory's deterministic successful startup-step proof.
    /// </summary>
    [CreateAssetMenu(
        menuName = "EchoDevGames/First Light Samples/Laboratory Immediate Success Step",
        fileName = "LaboratoryImmediateSuccessStep")]
    public sealed class LaboratoryImmediateSuccessStep : StartupStepDefinition
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

                await Awaitable.NextFrameAsync(
                    context.CancellationToken);

                context.ProgressReporter.Report(
                    StartupStepProgress.Determinate(
                        1f,
                        "Laboratory immediate-success step completed."));

                return StartupStepResult.Success(
                    "Laboratory immediate-success step completed.");
            }
        }
    }
}
