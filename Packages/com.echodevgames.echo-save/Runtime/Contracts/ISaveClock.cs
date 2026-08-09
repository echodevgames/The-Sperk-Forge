using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Replaceable time seam for later Chronicle operations.
    /// </summary>
    public interface ISaveClock
    {
        DateTime UtcNow { get; }

        double MonotonicSeconds { get; }
    }
}
