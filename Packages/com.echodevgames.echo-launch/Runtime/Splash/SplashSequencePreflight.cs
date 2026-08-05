//----- SplashSequencePreflight.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Validates the optional project-owned image splash sequence before any
    /// splash frame, startup-step executor, or destination side effect occurs.
    ///
    /// A null reference is a legal omission. Runtime reads but never repairs or
    /// rewrites the assigned sequence.
    /// </summary>
    internal static class SplashSequencePreflight
    {
        internal const string DiagnosticCode =
            "ELAUNCH-SPLASH-001";

        internal static SplashSequence Validate(
            EchoLaunchConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(
                    nameof(configuration));
            }

            SplashSequence sequence =
                configuration.SplashSequence;

            if (sequence == null)
            {
                return null;
            }

            try
            {
                sequence.ValidateForPlayback();
                return sequence;
            }
            catch (InvalidOperationException exception)
            {
                throw new StartupSequencePreflightException(
                    DiagnosticCode,
                    string.IsNullOrWhiteSpace(
                        exception.Message)
                        ? "The assigned splash sequence is invalid."
                        : exception.Message.Trim());
            }
            catch (ArgumentOutOfRangeException exception)
            {
                throw new StartupSequencePreflightException(
                    DiagnosticCode,
                    string.IsNullOrWhiteSpace(
                        exception.Message)
                        ? "The assigned splash sequence contains invalid authored data."
                        : exception.Message.Trim());
            }
        }
    }
}

//----- SplashSequencePreflight.cs END -----
