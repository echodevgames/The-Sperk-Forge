//----- StartupStepProgressRelay.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Captures accepted progress in the authoritative execution and forwards
    /// the same immutable value to an optional sequence observer.
    /// </summary>
    internal sealed class StartupStepProgressRelay :
        IStartupStepProgressReporter
    {
        private readonly StartupStepExecution
            execution;

        private readonly IStartupSequenceObserver
            observer;

        internal StartupStepProgressRelay(
            StartupStepExecution execution,
            IStartupSequenceObserver observer)
        {
            this.execution =
                execution ??
                throw new ArgumentNullException(
                    nameof(execution));

            this.observer = observer;
        }

        public void Report(
            StartupStepProgress progress)
        {
            execution.Report(progress);

            observer?.StepProgressChanged(
                execution,
                progress);
        }
    }
}

//----- StartupStepProgressRelay.cs END -----
