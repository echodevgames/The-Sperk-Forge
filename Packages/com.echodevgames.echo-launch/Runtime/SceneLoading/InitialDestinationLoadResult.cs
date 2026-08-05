//----- InitialDestinationLoadResult.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable terminal observation returned by one initial destination
    /// loader invocation.
    /// </summary>
    public sealed class InitialDestinationLoadResult
    {
        internal InitialDestinationLoadResult(
            InitialDestinationLoadStatus status,
            string destinationId,
            string code,
            string message,
            string details)
        {
            if (!Enum.IsDefined(
                    typeof(
                        InitialDestinationLoadStatus),
                    status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    status,
                    "The initial destination load status is not defined.");
            }

            string normalizedDestinationId =
                NormalizeRequiredText(
                    destinationId,
                    nameof(destinationId));

            if (!LaunchDestination
                    .IsCanonicalDestinationId(
                        normalizedDestinationId))
            {
                throw new ArgumentException(
                    "The destination result identity must use lowercase 32-character hexadecimal format.",
                    nameof(destinationId));
            }

            if (status ==
                InitialDestinationLoadStatus.Succeeded)
            {
                Code =
                    NormalizeOptionalText(code);

                Message =
                    NormalizeOptionalText(message);
            }
            else
            {
                Code =
                    NormalizeRequiredText(
                        code,
                        nameof(code));

                Message =
                    NormalizeRequiredText(
                        message,
                        nameof(message));
            }

            Status =
                status;

            DestinationId =
                normalizedDestinationId;

            Details =
                NormalizeOptionalText(details);
        }

        public InitialDestinationLoadStatus Status
        {
            get;
        }

        public string DestinationId
        {
            get;
        }

        public string Code
        {
            get;
        }

        public string Message
        {
            get;
        }

        public string Details
        {
            get;
        }

        public bool IsSucceeded =>
            Status ==
            InitialDestinationLoadStatus.Succeeded;

        public bool IsFailed =>
            Status ==
            InitialDestinationLoadStatus.Failed;

        public bool IsCancelled =>
            Status ==
            InitialDestinationLoadStatus.Cancelled;

        public static InitialDestinationLoadResult
            Success(
                string destinationId,
                string message =
                    "Initial destination activated.",
                string details = "")
        {
            return new InitialDestinationLoadResult(
                InitialDestinationLoadStatus
                    .Succeeded,
                destinationId,
                string.Empty,
                message,
                details);
        }

        public static InitialDestinationLoadResult
            Failed(
                string destinationId,
                string code,
                string message,
                string details = "")
        {
            return new InitialDestinationLoadResult(
                InitialDestinationLoadStatus
                    .Failed,
                destinationId,
                code,
                message,
                details);
        }

        public static InitialDestinationLoadResult
            Cancelled(
                string destinationId,
                string code,
                string message,
                string details = "")
        {
            return new InitialDestinationLoadResult(
                InitialDestinationLoadStatus
                    .Cancelled,
                destinationId,
                code,
                message,
                details);
        }

        private static string NormalizeRequiredText(
            string value,
            string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "A nonblank value is required.",
                    parameterName);
            }

            return value.Trim();
        }

        private static string NormalizeOptionalText(
            string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }
    }
}

//----- InitialDestinationLoadResult.cs END -----
