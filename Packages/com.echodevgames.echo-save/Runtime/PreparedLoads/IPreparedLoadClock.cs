
using System;

namespace EchoDevGames.EchoSave
{
    internal interface IPreparedLoadClock
    {
        DateTimeOffset UtcNow { get; }
    }
}
