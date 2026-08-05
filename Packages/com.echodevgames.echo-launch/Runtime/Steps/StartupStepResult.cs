//----- StartupStepResult.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable terminal result produced by one startup-step execution.
    /// </summary>
    public sealed class StartupStepResult
    {
        /// <summary>
        /// Gets the terminal status reported by the startup step.
        /// </summary>
        public StartupStepStatus Status { get; }

        /// <summary>
        /// Gets the stable diagnostic code associated with the result.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the human-readable summary of the result.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets optional diagnostic details.
        /// </summary>
        public string Details { get; }

        /// <summary>
        /// Gets whether the step completed successfully, including success
        /// with a warning.
        /// </summary>
        public bool IsSuccessful =>
            Status == StartupStepStatus.Succeeded ||
            Status == StartupStepStatus.Warning;

        /// <summary>
        /// Gets whether the result explicitly represents a recoverable or
        /// blocking failure.
        /// </summary>
        public bool IsFailure =>
            Status == StartupStepStatus.RecoverableFailure ||
            Status == StartupStepStatus.BlockingFailure;

        /// <summary>
        /// Gets whether the result explicitly blocks launch continuation.
        /// </summary>
        public bool IsBlocking =>
            Status == StartupStepStatus.BlockingFailure;

        /// <summary>
        /// Creates a validated terminal result.
        ///
        /// Internal access allows the Runtime test assembly to verify invalid
        /// construction without exposing a loose constructor to game code.
        /// </summary>
        internal StartupStepResult(
            StartupStepStatus status,
            string code,
            string message,
            string details)
        {
            ValidateTerminalStatus(status);

            if (RequiresDiagnostic(status))
            {
                Code = RequireText(code, nameof(code));
                Message = RequireText(message, nameof(message));
            }
            else
            {
                Code = NormalizeOptionalText(code);
                Message = NormalizeOptionalText(message);
            }

            Status = status;
            Details = NormalizeOptionalText(details);
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static StartupStepResult Success(
            string message = "",
            string details = "")
        {
            return new StartupStepResult(
                StartupStepStatus.Succeeded,
                string.Empty,
                message,
                details);
        }

        /// <summary>
        /// Creates a successful result that carries a warning.
        /// </summary>
        public static StartupStepResult Warning(
            string code,
            string message,
            string details = "")
        {
            return new StartupStepResult(
                StartupStepStatus.Warning,
                code,
                message,
                details);
        }

        /// <summary>
        /// Creates a non-blocking failure result.
        /// </summary>
        public static StartupStepResult RecoverableFailure(
            string code,
            string message,
            string details = "")
        {
            return new StartupStepResult(
                StartupStepStatus.RecoverableFailure,
                code,
                message,
                details);
        }

        /// <summary>
        /// Creates a blocking failure result.
        /// </summary>
        public static StartupStepResult BlockingFailure(
            string code,
            string message,
            string details = "")
        {
            return new StartupStepResult(
                StartupStepStatus.BlockingFailure,
                code,
                message,
                details);
        }

        /// <summary>
        /// Creates a skipped result.
        /// </summary>
        public static StartupStepResult Skipped(
            string message = "",
            string details = "")
        {
            return new StartupStepResult(
                StartupStepStatus.Skipped,
                string.Empty,
                message,
                details);
        }

        /// <summary>
        /// Creates a timed-out result.
        /// </summary>
        public static StartupStepResult TimedOut(
            string code,
            string message,
            string details = "")
        {
            return new StartupStepResult(
                StartupStepStatus.TimedOut,
                code,
                message,
                details);
        }

        /// <summary>
        /// Creates a cancelled result.
        /// </summary>
        public static StartupStepResult Cancelled(
            string code,
            string message,
            string details = "")
        {
            return new StartupStepResult(
                StartupStepStatus.Cancelled,
                code,
                message,
                details);
        }

        private static void ValidateTerminalStatus(
            StartupStepStatus status)
        {
            if (status == StartupStepStatus.NotStarted ||
                status == StartupStepStatus.Running)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    status,
                    "A completed startup-step result requires a terminal status.");
            }

            if (!Enum.IsDefined(typeof(StartupStepStatus), status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    status,
                    "The startup-step status is not defined.");
            }
        }

        private static bool RequiresDiagnostic(
            StartupStepStatus status)
        {
            return
                status == StartupStepStatus.Warning ||
                status == StartupStepStatus.RecoverableFailure ||
                status == StartupStepStatus.BlockingFailure ||
                status == StartupStepStatus.TimedOut ||
                status == StartupStepStatus.Cancelled;
        }

        private static string RequireText(
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

//----- StartupStepResult.cs END -----
