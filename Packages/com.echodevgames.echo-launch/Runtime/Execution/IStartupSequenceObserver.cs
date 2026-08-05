//----- IStartupSequenceObserver.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Internal lifecycle sink used by the sequence runner to publish
    /// authoritative runtime observations without coupling execution to the
    /// scene-facing root.
    /// </summary>
    internal interface IStartupSequenceObserver
    {
        void SequenceValidated(
            StartupSequence sequence);

        void StepStarted(
            StartupStepExecution execution);

        void StepProgressChanged(
            StartupStepExecution execution,
            StartupStepProgress progress);

        void StepCompleted(
            StartupStepExecution execution);
    }
}

//----- IStartupSequenceObserver.cs END -----
