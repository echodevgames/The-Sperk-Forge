using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    [CreateAssetMenu(
        fileName = "LaboratoryRecoverableFailureStep",
        menuName =
            "EchoDevGames/First Light/Laboratory/Recoverable Failure Step")]
    public sealed class LaboratoryRecoverableFailureStep :
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
                    StartupStepResult.RecoverableFailure(
                        LaboratoryDiagnosticCodes
                            .RecoverableFailure,
                        "The Laboratory emitted its authored recoverable failure."));
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
