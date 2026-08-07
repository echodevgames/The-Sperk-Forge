using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    [CreateAssetMenu(
        fileName = "LaboratoryImmediateSuccessStep",
        menuName =
            "EchoDevGames/First Light/Laboratory/Immediate Success Step")]
    public sealed class LaboratoryImmediateSuccessStep :
        StartupStepDefinition
    {
        public override IStartupStepExecutor CreateExecutor()
        {
            return new Executor();
        }

        private sealed class Executor :
            IStartupStepExecutor
        {
            public Awaitable<StartupStepResult> ExecuteAsync(
                StartupStepContext context)
            {
                context.ProgressReporter.Report(
                    StartupStepProgress.Determinate(
                        1f,
                        "Immediate Laboratory step completed."));

                return Completed(
                    StartupStepResult.Success(
                        "Immediate Laboratory step completed."));
            }
        }

        private static Awaitable<StartupStepResult> Completed(
            StartupStepResult result)
        {
            AwaitableCompletionSource<StartupStepResult> source =
                new AwaitableCompletionSource<StartupStepResult>();

            source.SetResult(result);
            return source.Awaitable;
        }
    }
}
