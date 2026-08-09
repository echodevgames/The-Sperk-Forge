
using System;
using System.Diagnostics;

namespace EchoDevGames.EchoSave
{
    internal sealed class SystemSaveClock :
        ISaveClock
    {
        internal static readonly SystemSaveClock
            Instance =
                new SystemSaveClock();

        private SystemSaveClock()
        {
        }

        public DateTime UtcNow =>
            DateTime.UtcNow;

        public double MonotonicSeconds =>
            Stopwatch.GetTimestamp() /
            (double)Stopwatch.Frequency;
    }
}
