//----- LaunchStateChangedEvent.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable notification raised after the authoritative launch state changes.
    /// </summary>
    public readonly struct LaunchStateChangedEvent
    {
        /// <summary>
        /// Gets the state that was active before publication.
        /// </summary>
        public LaunchStatus PreviousState { get; }

        /// <summary>
        /// Gets the newly accepted authoritative state.
        /// </summary>
        public LaunchStatus CurrentState { get; }

        /// <summary>
        /// Gets the newly accepted progress snapshot.
        /// </summary>
        public LaunchProgressSnapshot Progress { get; }

        internal LaunchStateChangedEvent(
            LaunchStatus previousState,
            LaunchStatus currentState,
            LaunchProgressSnapshot progress)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            Progress = progress;
        }
    }
}

//----- LaunchStateChangedEvent.cs END -----