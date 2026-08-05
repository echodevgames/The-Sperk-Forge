//----- StartupStepPolicyEvaluator.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Applies one immutable authored startup-step policy to one terminal
    /// runtime result.
    ///
    /// The evaluator creates effective runtime results only. It never
    /// rewrites the authored policy, entry, definition, or sequence.
    /// </summary>
    internal static class StartupStepPolicyEvaluator
    {
        /// <summary>
        /// Evaluates one terminal result and determines whether traversal
        /// may continue.
        /// </summary>
        internal static StartupStepPolicyDecision Evaluate(
            StartupStepPolicy policy,
            StartupStepResult terminalResult)
        {
            if (terminalResult == null)
            {
                throw new ArgumentNullException(
                    nameof(terminalResult));
            }

            if (!policy.IsValid)
            {
                throw new ArgumentException(
                    "Startup-step policy contains unsupported authored values.",
                    nameof(policy));
            }

            switch (terminalResult.Status)
            {
                case StartupStepStatus.Succeeded:
                case StartupStepStatus.Warning:
                case StartupStepStatus.Skipped:
                    return Preserve(
                        terminalResult,
                        true);

                case StartupStepStatus.Cancelled:
                    return Preserve(
                        terminalResult,
                        false);

                case StartupStepStatus.RecoverableFailure:
                case StartupStepStatus.BlockingFailure:
                case StartupStepStatus.TimedOut:
                    return ApplyFailureAction(
                        policy.FailureAction,
                        terminalResult);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(terminalResult),
                        terminalResult.Status,
                        "The startup-step result does not contain a supported terminal status.");
            }
        }

        private static StartupStepPolicyDecision
            ApplyFailureAction(
                StartupStepFailureAction failureAction,
                StartupStepResult originalResult)
        {
            switch (failureAction)
            {
                case StartupStepFailureAction
                    .ContinueWithWarning:
                    return new StartupStepPolicyDecision(
                        originalResult,
                        StartupStepResult.Warning(
                            originalResult.Code,
                            originalResult.Message,
                            originalResult.Details),
                        true);

                case StartupStepFailureAction.BlockLaunch:
                    StartupStepResult blockingResult =
                        originalResult.Status ==
                        StartupStepStatus.BlockingFailure
                            ? originalResult
                            : StartupStepResult
                                .BlockingFailure(
                                    originalResult.Code,
                                    originalResult.Message,
                                    originalResult.Details);

                    return new StartupStepPolicyDecision(
                        originalResult,
                        blockingResult,
                        false);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(failureAction),
                        failureAction,
                        "The startup-step failure action is not supported.");
            }
        }

        private static StartupStepPolicyDecision Preserve(
            StartupStepResult result,
            bool shouldContinue)
        {
            return new StartupStepPolicyDecision(
                result,
                result,
                shouldContinue);
        }
    }
}

//----- StartupStepPolicyEvaluator.cs END -----
