//----- StartupStepExceptionPhase.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Identifies the bounded startup-step operation that produced a
    /// contained runtime exception.
    /// </summary>
    internal enum StartupStepExceptionPhase
    {
        /// <summary>
        /// The step definition failed while creating a fresh executor.
        /// </summary>
        ExecutorFactory = 0,

        /// <summary>
        /// The fresh executor failed while running the startup step.
        /// </summary>
        ExecutorExecution = 1
    }
}

//----- StartupStepExceptionPhase.cs END -----
