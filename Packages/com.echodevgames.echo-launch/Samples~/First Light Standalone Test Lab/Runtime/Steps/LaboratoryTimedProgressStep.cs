using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    [CreateAssetMenu(
        fileName = "LaboratoryTimedProgressStep",
        menuName =
            "EchoDevGames/First Light/Laboratory/Timed Progress Step")]
    public sealed class LaboratoryTimedProgressStep :
        StartupStepDefinition
    {
        [SerializeField]
        [Min(1)]
        private int frameCount = 4;

        public int FrameCount =>
            Math.Max(1, frameCount);

        public override IStartupStepExecutor CreateExecutor()
        {
            return new Executor(FrameCount);
        }

        private sealed class Executor :
            IStartupStepExecutor
        {
            private readonly int frameCount;

            internal Executor(int frameCount)
            {
                this.frameCount = Math.Max(1, frameCount);
            }

            public async Awaitable<StartupStepResult> ExecuteAsync(
                StartupStepContext context)
            {
                for (int index = 0;
                     index < frameCount;
                     index++)
                {
                    context.CancellationToken
                        .ThrowIfCancellationRequested();

                    await Awaitable.NextFrameAsync(
                        context.CancellationToken);

                    float progress =
                        (index + 1f) / frameCount;

                    context.ProgressReporter.Report(
                        StartupStepProgress.Determinate(
                            progress,
                            $"Laboratory progress {index + 1} of {frameCount}."));
                }

                return StartupStepResult.Success(
                    "Timed Laboratory progress completed.");
            }
        }
    }
}
