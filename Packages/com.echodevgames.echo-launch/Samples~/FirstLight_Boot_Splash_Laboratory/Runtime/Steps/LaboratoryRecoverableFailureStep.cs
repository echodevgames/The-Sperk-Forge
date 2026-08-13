using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    /// <summary>
    /// Emits the Laboratory's authored recoverable failure for policy-conversion testing.
    /// </summary>
    [CreateAssetMenu(
        menuName = "EchoDevGames/First Light Samples/Laboratory Recoverable Failure Step",
        fileName = "LaboratoryRecoverableFailureStep")]
    public sealed class LaboratoryRecoverableFailureStep : StartupStepDefinition
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

                return StartupStepResult.RecoverableFailure(
                    "ELAUNCH-LAB-STEP-002",
                    "The Laboratory emitted its authored recoverable failure.");
            }
        }
    }
}
