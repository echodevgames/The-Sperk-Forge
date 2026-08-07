using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    [CreateAssetMenu(
        fileName = "LaboratoryBlockingFailureStep",
        menuName =
            "EchoDevGames/First Light/Laboratory/Blocking Failure Step")]
    public sealed class LaboratoryBlockingFailureStep :
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
                return Completed(
                    StartupStepResult.BlockingFailure(
                        LaboratoryDiagnosticCodes.BlockingFailure,
                        "The Laboratory emitted its authored blocking failure."));
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
