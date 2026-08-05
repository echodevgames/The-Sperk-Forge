//----- LaunchStateTransitionRules.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Defines the legal lifecycle transitions for one launch session.
    /// </summary>
    internal static class LaunchStateTransitionRules
    {
        /// <summary>
        /// Returns whether the supplied status permanently ends a session.
        /// </summary>
        internal static bool IsTerminal(
            LaunchStatus status)
        {
            ValidateDefined(
                status,
                nameof(status));

            return
                status == LaunchStatus.Completed ||
                status == LaunchStatus.Failed ||
                status == LaunchStatus.Interrupted;
        }

        /// <summary>
        /// Returns whether a session may move from the current status
        /// to the requested next status.
        /// </summary>
        internal static bool CanTransition(
            LaunchStatus current,
            LaunchStatus next)
        {
            ValidateDefined(
                current,
                nameof(current));

            ValidateDefined(
                next,
                nameof(next));

            if (IsTerminal(current))
            {
                return false;
            }

            switch (current)
            {
                case LaunchStatus.None:
                    return
                        next == LaunchStatus.AuthorityClaimed;

                case LaunchStatus.AuthorityClaimed:
                    return
                        next == LaunchStatus.AuthorityClaimed ||
                        next == LaunchStatus.Validating ||
                        next == LaunchStatus.Failed ||
                        next == LaunchStatus.Interrupted;

                case LaunchStatus.Validating:
                    return
                        next == LaunchStatus.Validating ||
                        next == LaunchStatus.Running ||
                        next == LaunchStatus.Failed ||
                        next == LaunchStatus.Interrupted;

                case LaunchStatus.Running:
                    return
                        next == LaunchStatus.Running ||
                        next == LaunchStatus.Transitioning ||
                        next == LaunchStatus.Failed ||
                        next == LaunchStatus.Interrupted;

                case LaunchStatus.Transitioning:
                    return
                        next == LaunchStatus.Transitioning ||
                        next == LaunchStatus.Completed ||
                        next == LaunchStatus.Failed ||
                        next == LaunchStatus.Interrupted;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Throws when a requested publication would violate the
        /// approved launch lifecycle.
        /// </summary>
        internal static void EnsureCanPublish(
            LaunchStatus current,
            LaunchStatus next)
        {
            ValidateDefined(
                current,
                nameof(current));

            ValidateDefined(
                next,
                nameof(next));

            if (IsTerminal(current))
            {
                throw new InvalidOperationException(
                    $"The launch session is already terminal in state " +
                    $"'{current}' and cannot publish another snapshot.");
            }

            if (!CanTransition(current, next))
            {
                throw new InvalidOperationException(
                    $"The launch lifecycle cannot transition from " +
                    $"'{current}' to '{next}'.");
            }
        }

        private static void ValidateDefined(
            LaunchStatus status,
            string parameterName)
        {
            if (!Enum.IsDefined(
                    typeof(LaunchStatus),
                    status))
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    status,
                    "The launch status is not defined.");
            }
        }
    }
}

//----- LaunchStateTransitionRules.cs END -----