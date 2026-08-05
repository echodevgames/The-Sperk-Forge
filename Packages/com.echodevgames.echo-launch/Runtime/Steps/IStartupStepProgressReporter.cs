//----- IStartupStepProgressReporter.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Receives immutable progress values from one active startup-step
    /// executor.
    /// </summary>
    public interface IStartupStepProgressReporter
    {
        /// <summary>
        /// Reports the latest progress value for the active step.
        /// </summary>
        void Report(
            StartupStepProgress progress);
    }
}

//----- IStartupStepProgressReporter.cs END -----
