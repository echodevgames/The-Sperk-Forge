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

            Progress = new LaunchProgressSnapshot(
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
        }

        /// <summary>
        /// Replaces the current immutable progress snapshot.
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

            if (snapshot.Status == LaunchStatus.None)
            {
                throw new ArgumentException(
                    "An active launch session cannot publish the None status.",
                    nameof(snapshot));
            }

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