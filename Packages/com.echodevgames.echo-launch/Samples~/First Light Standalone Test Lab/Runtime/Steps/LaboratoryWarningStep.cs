using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    [CreateAssetMenu(
        fileName = "LaboratoryWarningStep",
        menuName =
            "EchoDevGames/First Light/Laboratory/Warning Step")]
    public sealed class LaboratoryWarningStep :
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
                        "Laboratory warning emitted."));

                return Completed(
                    StartupStepResult.Warning(
                        LaboratoryDiagnosticCodes.Warning,
                        "The Laboratory emitted its authored warning."));
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
