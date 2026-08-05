//----- StartupStepAwaitOutcome.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores the immutable monitored outcome of one settled startup-step
    /// executor.
    ///
    /// The outcome preserves whether the executor returned normally or
    /// threw, plus the timing facts that determine whether timeout remains
    /// authoritative. It does not apply failure policy.
    /// </summary>
    internal sealed class StartupStepAwaitOutcome
    {
        private StartupStepAwaitOutcome(
            bool completedWithoutException,
            StartupStepResult executorResult,
            Exception executorException,
            StartupStepTiming timing,
            bool callerCancellationObserved)
        {
            if (completedWithoutException)
            {
                if (executorException != null)
                {
                    throw new ArgumentException(
                        "A normally completed startup-step outcome cannot also contain an executor exception.",
                        nameof(executorException));
                }
            }
            else if (executorException == null)
            {
                throw new ArgumentNullException(
                    nameof(executorException));
            }

            CompletedWithoutException =
                completedWithoutException;

            ExecutorResult = executorResult;
            ExecutorException = executorException;
            Timing = timing;
            CallerCancellationObserved =
                callerCancellationObserved;
        }

        /// <summary>
        /// Gets whether the executor settled by returning instead of
        /// throwing.
        ///
        /// A normally returned null result remains distinguishable from an
        /// exception and is handled later as a contract failure.
        /// </summary>
        internal bool CompletedWithoutException
        {
            get;
        }

        /// <summary>
        /// Gets the executor's returned result.
        ///
        /// This can be null only when the executor returned null.
        /// </summary>
        internal StartupStepResult ExecutorResult
        {
            get;
        }

        /// <summary>
        /// Gets the executor exception when it did not return normally.
        /// </summary>
        internal Exception ExecutorException
        {
            get;
        }

        /// <summary>
        /// Gets whether the executor settled by throwing.
        /// </summary>
        internal bool HasExecutorException =>
            !CompletedWithoutException;

        /// <summary>
        /// Gets the immutable attempt timing snapshot.
        /// </summary>
        internal StartupStepTiming Timing
        {
            get;
        }

        /// <summary>
        /// Gets whether the runner's caller requested cancellation before
        /// the executor became observable as settled.
        ///
        /// The monitor still consumes the executor before publishing this
        /// fact so later traversal never overlaps active startup work.
        /// </summary>
        internal bool CallerCancellationObserved
        {
            get;
        }

        /// <summary>
        /// Gets whether the monitored deadline won before settlement.
        /// </summary>
        internal bool TimedOut =>
            Timing.TimedOut;

        /// <summary>
        /// Gets whether timeout handling requested cooperative
        /// cancellation.
        /// </summary>
        internal bool CancellationRequested =>
            Timing.CancellationRequested;

        /// <summary>
        /// Creates an outcome for an executor that returned normally.
        ///
        /// The returned result may be null so the runner can convert that
        /// contract failure explicitly.
        /// </summary>
        internal static StartupStepAwaitOutcome
            FromResult(
                StartupStepResult executorResult,
                StartupStepTiming timing,
                bool callerCancellationObserved = false)
        {
            return new StartupStepAwaitOutcome(
                true,
                executorResult,
                null,
                timing,
                callerCancellationObserved);
        }

        /// <summary>
        /// Creates an outcome for an executor that settled by throwing.
        /// </summary>
        internal static StartupStepAwaitOutcome
            FromException(
                Exception executorException,
                StartupStepTiming timing,
                bool callerCancellationObserved = false)
        {
            return new StartupStepAwaitOutcome(
                false,
                null,
                executorException,
                timing,
                callerCancellationObserved);
        }
    }
}

//----- StartupStepAwaitOutcome.cs END -----
