//----- StartupSequencePreflightException.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Internal structured exception used when side-effect-free sequence
    /// preflight rejects authored launch data.
    /// </summary>
    internal sealed class StartupSequencePreflightException :
        InvalidOperationException
    {
        internal StartupSequencePreflightException(
            string diagnosticCode,
            string failureMessage)
            : base(
                $"[{RequireText(diagnosticCode, nameof(diagnosticCode))}] " +
                RequireText(failureMessage, nameof(failureMessage)))
        {
            DiagnosticCode =
                diagnosticCode.Trim();

            FailureMessage =
                failureMessage.Trim();
        }

        internal string DiagnosticCode
        {
            get;
        }

        internal string FailureMessage
        {
            get;
        }

        private static string RequireText(
            string value,
            string parameterName)
        {
            if (string.IsNullOrWhiteSpace(
                    value))
            {
                throw new ArgumentException(
                    "A nonblank value is required.",
                    parameterName);
            }

            return value.Trim();
        }
    }
}

//----- StartupSequencePreflightException.cs END -----
