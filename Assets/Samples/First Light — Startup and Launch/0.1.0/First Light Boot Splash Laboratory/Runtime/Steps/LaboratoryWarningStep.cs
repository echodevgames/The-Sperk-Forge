using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    /// <summary>
    /// Emits the Laboratory's authored warning result.
    /// </summary>
    [CreateAssetMenu(
        menuName = "EchoDevGames/First Light Samples/Laboratory Warning Step",
        fileName = "LaboratoryWarningStep")]
    public sealed class LaboratoryWarningStep : StartupStepDefinition
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

                return StartupStepResult.Warning(
                    "ELAUNCH-LAB-STEP-001",
                    "The Laboratory emitted its authored warning.");
            }
        }
    }
}
