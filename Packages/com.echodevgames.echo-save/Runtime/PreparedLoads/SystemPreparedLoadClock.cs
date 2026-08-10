
using System;

namespace EchoDevGames.EchoSave
{
    internal sealed class SystemPreparedLoadClock : IPreparedLoadClock
    {
        internal static readonly SystemPreparedLoadClock Instance =
            new SystemPreparedLoadClock();

        private SystemPreparedLoadClock()
        {
        }

        public DateTimeOffset UtcNow =>
            DateTimeOffset.UtcNow;
    }
}
