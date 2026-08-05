//----- InitialDestinationProgressRelay.cs START -----

using System;
using System.Threading;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Accepts finite normalized destination progress while one transition is
    /// active and ignores reports after settlement.
    /// </summary>
    internal sealed class
        InitialDestinationProgressRelay :
            IProgress<float>
    {
        private readonly Action<float>
            acceptedProgress;

        private int isClosed;

        internal InitialDestinationProgressRelay(
            Action<float> acceptedProgress)
        {
            this.acceptedProgress =
                acceptedProgress ??
                throw new ArgumentNullException(
                    nameof(acceptedProgress));
        }

        internal bool IsClosed =>
            Volatile.Read(
                ref isClosed) != 0;

        public void Report(
            float value)
        {
            if (float.IsNaN(value) ||
                float.IsInfinity(value) ||
                value < 0f ||
                value > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Initial destination progress must be finite and normalized.");
            }

            if (IsClosed)
            {
                return;
            }

            acceptedProgress(value);
        }

        internal void Close()
        {
            Interlocked.Exchange(
                ref isClosed,
                1);
        }
    }
}

//----- InitialDestinationProgressRelay.cs END -----
