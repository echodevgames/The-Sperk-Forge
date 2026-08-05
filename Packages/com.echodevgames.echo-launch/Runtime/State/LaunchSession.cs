//----- LaunchSession.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Owns the live state for one First Light launch attempt.
    /// </summary>
    internal sealed class LaunchSession
    {
        /// <summary>
        /// Gets the mode selected for this launch attempt.
        /// </summary>
        internal LaunchMode Mode { get; }

        /// <summary>
        /// Gets the current overall launch state.
        /// </summary>
        internal LaunchStatus State =>
            Progress.Status;

        /// <summary>
        /// Gets the latest immutable progress snapshot.
        /// </summary>
        internal LaunchProgressSnapshot Progress
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates one fresh launch session.
        /// </summary>
        internal LaunchSession(
            LaunchMode mode)
        {
            ValidateMode(mode);

            Mode = mode;

            LaunchProgressSnapshot initialSnapshot =
                new LaunchProgressSnapshot(
                    mode,
                    LaunchStatus.AuthorityClaimed,
                    string.Empty,
                    -1,
                    0,
                    0f,
                    true,
                    "Launch authority claimed.",
                    0d,
                    null);

            LaunchStateTransitionRules.EnsureCanPublish(
                LaunchStatus.None,
                initialSnapshot.Status);

            Progress = initialSnapshot;
        }

        /// <summary>
        /// Replaces the current immutable progress snapshot after
        /// validating its mode and lifecycle transition.
        /// </summary>
        internal void Publish(
            LaunchProgressSnapshot snapshot)
        {
            if (snapshot.Mode != Mode)
            {
                throw new ArgumentException(
                    "The snapshot mode must match the launch session mode.",
                    nameof(snapshot));
            }

            LaunchStateTransitionRules.EnsureCanPublish(
                State,
                snapshot.Status);

            Progress = snapshot;
        }

        private static void ValidateMode(
            LaunchMode mode)
        {
            if (!Enum.IsDefined(
                    typeof(LaunchMode),
                    mode))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(mode),
                    mode,
                    "The launch mode is not defined.");
            }
        }
    }
}

//----- LaunchSession.cs END -----