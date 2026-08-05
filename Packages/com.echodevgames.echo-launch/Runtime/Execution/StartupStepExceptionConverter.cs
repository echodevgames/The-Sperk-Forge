//----- StartupStepExceptionConverter.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Converts bounded startup-step contract and execution failures into
    /// stable immutable runtime results.
    ///
    /// The converter never copies stack traces or recursive exception
    /// graphs into results. Cooperative cancellation remains outside the
    /// generic conversion path.
    /// </summary>
    internal static class StartupStepExceptionConverter
    {
        internal const string DiagnosticCode =
            "ELAUNCH-STEP-004";

        /// <summary>
        /// Converts one non-cancellation exception from the specified
        /// startup-step phase.
        /// </summary>
        internal static StartupStepResult Convert(
            StartupStepExceptionPhase phase,
            Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(
                    nameof(exception));
            }

            if (exception is OperationCanceledException)
            {
                throw exception;
            }

            string details =
                BuildSanitizedDetails(exception);

            switch (phase)
            {
                case StartupStepExceptionPhase
                    .ExecutorFactory:
                    return StartupStepResult
                        .BlockingFailure(
                            DiagnosticCode,
                            "The startup-step executor could not be created.",
                            details);

                case StartupStepExceptionPhase
                    .ExecutorExecution:
                    return StartupStepResult
                        .RecoverableFailure(
                            DiagnosticCode,
                            "The startup-step executor threw an exception.",
                            details);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(phase),
                        phase,
                        "The startup-step exception phase is not supported.");
            }
        }

        /// <summary>
        /// Creates a blocking contract-failure result when a definition
        /// returns no executor.
        /// </summary>
        internal static StartupStepResult
            CreateNullExecutorResult()
        {
            return StartupStepResult.BlockingFailure(
                DiagnosticCode,
                "The startup-step definition returned a null executor.",
                "ContractFailure: NullExecutor");
        }

        /// <summary>
        /// Creates a blocking contract-failure result when an executor
        /// returns no terminal result.
        /// </summary>
        internal static StartupStepResult
            CreateNullResult()
        {
            return StartupStepResult.BlockingFailure(
                DiagnosticCode,
                "The startup-step executor returned a null result.",
                "ContractFailure: NullResult");
        }

        private static string BuildSanitizedDetails(
            Exception exception)
        {
            string exceptionType =
                exception.GetType().FullName;

            if (string.IsNullOrWhiteSpace(
                    exceptionType))
            {
                exceptionType =
                    exception.GetType().Name;
            }

            string exceptionMessage =
                string.IsNullOrWhiteSpace(
                    exception.Message)
                    ? string.Empty
                    : exception.Message.Trim();

            if (string.IsNullOrEmpty(
                    exceptionMessage))
            {
                return
                    $"ExceptionType: {exceptionType}";
            }

            return
                $"ExceptionType: {exceptionType}\n" +
                $"ExceptionMessage: {exceptionMessage}";
        }
    }
}

//----- StartupStepExceptionConverter.cs END -----
