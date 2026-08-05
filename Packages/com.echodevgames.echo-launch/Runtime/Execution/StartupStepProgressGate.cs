//----- StartupStepProgressGate.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Forwards active startup-step progress and silently contains reports
    /// that arrive after timeout or another runtime boundary closes the
    /// gate.
    /// </summary>
    internal sealed class StartupStepProgressGate :
        IStartupStepProgressReporter
    {
        private readonly IStartupStepProgressReporter
            reporter;

        private bool isOpen = true;

        /// <summary>
        /// Creates one open progress gate.
        /// </summary>
        internal StartupStepProgressGate(
            IStartupStepProgressReporter reporter)
        {
            this.reporter =
                reporter ??
                throw new ArgumentNullException(
                    nameof(reporter));
        }

        /// <summary>
        /// Gets whether progress is still forwarded.
        /// </summary>
        internal bool IsOpen =>
            isOpen;

        /// <summary>
        /// Permanently closes the gate.
        ///
        /// Repeated close calls are safe.
        /// </summary>
        internal void Close()
        {
            isOpen = false;
        }

        /// <summary>
        /// Forwards progress while open and ignores late progress after
        /// closure.
        /// </summary>
        public void Report(
            StartupStepProgress progress)
        {
            if (!isOpen)
            {
                return;
            }

            reporter.Report(progress);
        }
    }
}

//----- StartupStepProgressGate.cs END -----
