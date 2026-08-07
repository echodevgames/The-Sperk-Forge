using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    /// <summary>
    /// Emits the Laboratory's authored blocking failure.
    /// </summary>
    [CreateAssetMenu(
        menuName = "EchoDevGames/First Light Samples/Laboratory Blocking Failure Step",
        fileName = "LaboratoryBlockingFailureStep")]
    public sealed class LaboratoryBlockingFailureStep : StartupStepDefinition
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

                return StartupStepResult.BlockingFailure(
                    "ELAUNCH-LAB-STEP-003",
                    "The Laboratory emitted its authored blocking failure.");
            }
        }
    }
}
