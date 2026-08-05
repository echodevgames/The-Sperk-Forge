//----- StartupStepPolicyDecision.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores the immutable result of applying one authored startup-step
    /// policy to one terminal runtime result.
    ///
    /// The original result remains available for diagnostics. The effective
    /// result is the value captured by the execution and future report.
    /// </summary>
    internal sealed class StartupStepPolicyDecision
    {
        /// <summary>
        /// Creates one immutable policy decision.
        /// </summary>
        internal StartupStepPolicyDecision(
            StartupStepResult originalResult,
            StartupStepResult effectiveResult,
            bool shouldContinue)
        {
            OriginalResult =
                originalResult ??
                throw new ArgumentNullException(
                    nameof(originalResult));

            EffectiveResult =
                effectiveResult ??
                throw new ArgumentNullException(
                    nameof(effectiveResult));

            ShouldContinue = shouldContinue;
        }

        /// <summary>
        /// Gets the terminal result produced before policy application.
        /// </summary>
        internal StartupStepResult OriginalResult
        {
            get;
        }

        /// <summary>
        /// Gets the terminal result that the runner must capture.
        /// </summary>
        internal StartupStepResult EffectiveResult
        {
            get;
        }

        /// <summary>
        /// Gets whether sequence traversal may continue to the next authored
        /// entry.
        /// </summary>
        internal bool ShouldContinue
        {
            get;
        }

        /// <summary>
        /// Gets whether this decision stops sequence traversal.
        /// </summary>
        internal bool StopsTraversal =>
            !ShouldContinue;

        /// <summary>
        /// Gets whether policy replaced the original immutable result with a
        /// different result instance.
        /// </summary>
        internal bool WasConverted =>
            !ReferenceEquals(
                OriginalResult,
                EffectiveResult);
    }
}

//----- StartupStepPolicyDecision.cs END -----
