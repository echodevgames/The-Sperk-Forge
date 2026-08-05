//----- LaunchProgressChangedEvent.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable notification raised after an authoritative progress snapshot
    /// is accepted.
    /// </summary>
    public readonly struct LaunchProgressChangedEvent
    {
        /// <summary>
        /// Gets the snapshot that was active before publication.
        /// </summary>
        public LaunchProgressSnapshot Previous { get; }

        /// <summary>
        /// Gets the newly accepted authoritative snapshot.
        /// </summary>
        public LaunchProgressSnapshot Current { get; }

        internal LaunchProgressChangedEvent(
            LaunchProgressSnapshot previous,
            LaunchProgressSnapshot current)
        {
            Previous = previous;
            Current = current;
        }
    }
}

//----- LaunchProgressChangedEvent.cs END -----
