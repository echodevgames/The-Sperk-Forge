//----- StartupStepTimeoutMonitor.cs START -----

using System;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Monitors one startup-step executor against an optional monotonic
    /// deadline and waits for the executor to settle.
    ///
    /// The monitor never abandons active work. Once timeout is observed it
    /// closes progress, requests cooperative cancellation when supported,
    /// and continues ticking until the executor returns or throws.
    /// </summary>
    internal static class StartupStepTimeoutMonitor
    {
        /// <summary>
        /// Monitors one already-created executor awaitable.
        ///
        /// Caller cancellation remains distinct. Once observed, the monitor
        /// waits for the linked executor to settle and returns that fact as
        /// part of the immutable await outcome.
        /// </summary>
        internal static async Awaitable<
            StartupStepAwaitOutcome>
            MonitorAsync(
                Awaitable<StartupStepResult>
                    executorAwaitable,
                StartupStepPolicy policy,
                ILaunchClock clock,
                double startSeconds,
                CancellationTokenSource
                    timeoutCancellationSource,
                StartupStepProgressGate progressGate,
                CancellationToken callerCancellationToken)
        {
            if (!policy.IsValid)
            {
                throw new ArgumentException(
                    "Startup-step timeout monitoring requires a valid authored policy.",
                    nameof(policy));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(
                    nameof(clock));
            }

            ValidateClockValue(
                startSeconds,
                nameof(startSeconds));

            if (progressGate == null)
            {
                throw new ArgumentNullException(
                    nameof(progressGate));
            }

            if (timeoutCancellationSource == null)
            {
                throw new ArgumentNullException(
                    nameof(timeoutCancellationSource));
            }

            double previousSeconds =
                startSeconds;

            double timeoutSeconds =
                policy.HasTimeout
                    ? policy.TimeoutSeconds
                    : 0d;

            double deadlineSeconds =
                policy.HasTimeout
                    ? startSeconds +
                        timeoutSeconds
                    : 0d;

            if (policy.HasTimeout &&
                (double.IsNaN(deadlineSeconds) ||
                 double.IsInfinity(deadlineSeconds) ||
                 deadlineSeconds < startSeconds))
            {
                throw new InvalidOperationException(
                    "The startup-step timeout deadline is not a finite monotonic value.");
            }

            Awaitable<StartupStepResult>.Awaiter
                executorAwaiter =
                    executorAwaitable.GetAwaiter();

            bool timedOut = false;

            bool cancellationRequested = false;

            bool callerCancellationObserved = false;

            Exception monitorFailure = null;

            while (!executorAwaiter.IsCompleted)
            {
                if (!callerCancellationObserved &&
                    callerCancellationToken
                        .IsCancellationRequested)
                {
                    callerCancellationObserved = true;
                    progressGate.Close();
                }

                if (executorAwaiter.IsCompleted)
                {
                    break;
                }

                if (monitorFailure == null &&
                    !callerCancellationObserved)
                {
                    try
                    {
                        double currentSeconds =
                            ReadClock(
                                clock,
                                previousSeconds);

                        previousSeconds =
                            currentSeconds;

                        if (executorAwaiter.IsCompleted)
                        {
                            break;
                        }

                        if (!timedOut &&
                            policy.HasTimeout &&
                            currentSeconds >=
                            deadlineSeconds)
                        {
                            timedOut = true;
                            progressGate.Close();

                            if (policy
                                .SupportsCancellation)
                            {
                                RequestCancellation(
                                    timeoutCancellationSource);

                                cancellationRequested =
                                    true;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        monitorFailure = exception;
                        progressGate.Close();

                        if (policy
                            .SupportsCancellation)
                        {
                            RequestCancellation(
                                timeoutCancellationSource);
                        }
                    }
                }

                if (executorAwaiter.IsCompleted)
                {
                    break;
                }

                try
                {
                    if (monitorFailure == null)
                    {
                        await clock.NextTickAsync(
                            callerCancellationObserved
                                ? CancellationToken.None
                                : callerCancellationToken);
                    }
                    else
                    {
                        await Awaitable.NextFrameAsync(
                            CancellationToken.None);
                    }
                }
                catch (OperationCanceledException)
                    when (callerCancellationToken
                        .IsCancellationRequested)
                {
                    callerCancellationObserved = true;
                    progressGate.Close();
                }
                catch (Exception exception)
                {
                    if (monitorFailure == null)
                    {
                        monitorFailure = exception;
                        progressGate.Close();

                        if (policy
                            .SupportsCancellation)
                        {
                            RequestCancellation(
                                timeoutCancellationSource);
                        }
                    }
                }
            }

            progressGate.Close();

            double settlementSeconds =
                previousSeconds;

            if (monitorFailure == null)
            {
                try
                {
                    settlementSeconds =
                        ReadClock(
                            clock,
                            previousSeconds);
                }
                catch (Exception exception)
                {
                    monitorFailure = exception;
                }
            }

            bool completedWithoutException = false;

            StartupStepResult executorResult = null;

            Exception executorException = null;

            try
            {
                executorResult =
                    executorAwaiter.GetResult();

                completedWithoutException = true;
            }
            catch (Exception exception)
            {
                executorException = exception;
            }

            if (!callerCancellationObserved &&
                executorException is
                    OperationCanceledException &&
                callerCancellationToken
                    .IsCancellationRequested)
            {
                callerCancellationObserved = true;
            }

            StartupStepTiming timing =
                new StartupStepTiming(
                    startSeconds,
                    settlementSeconds,
                    timeoutSeconds,
                    timedOut,
                    cancellationRequested);

            if (callerCancellationObserved)
            {
                if (completedWithoutException)
                {
                    return StartupStepAwaitOutcome
                        .FromResult(
                            executorResult,
                            timing,
                            true);
                }

                return StartupStepAwaitOutcome
                    .FromException(
                        executorException,
                        timing,
                        true);
            }

            if (monitorFailure != null)
            {
                throw monitorFailure;
            }

            if (completedWithoutException)
            {
                return StartupStepAwaitOutcome
                    .FromResult(
                        executorResult,
                        timing);
            }

            if (executorException is
                    OperationCanceledException &&
                !timedOut)
            {
                throw executorException;
            }

            return StartupStepAwaitOutcome
                .FromException(
                    executorException,
                    timing);
        }

        private static double ReadClock(
            ILaunchClock clock,
            double previousSeconds)
        {
            double currentSeconds =
                clock.NowSeconds;

            ValidateClockValue(
                currentSeconds,
                "clock.NowSeconds");

            if (currentSeconds < previousSeconds)
            {
                throw new InvalidOperationException(
                    "The launch clock moved backward during startup-step execution.");
            }

            return currentSeconds;
        }

        private static void ValidateClockValue(
            double value,
            string parameterName)
        {
            if (double.IsNaN(value) ||
                double.IsInfinity(value) ||
                value < 0d)
            {
                throw new InvalidOperationException(
                    $"The launch clock returned an invalid time value for '{parameterName}'.");
            }
        }

        private static void RequestCancellation(
            CancellationTokenSource
                cancellationSource)
        {
            if (cancellationSource
                .IsCancellationRequested)
            {
                return;
            }

            try
            {
                cancellationSource.Cancel();
            }
            catch (AggregateException)
            {
                // Timeout remains authoritative even when a consumer-owned
                // cancellation callback throws. The active executor is still
                // consumed before this monitor returns.
            }
        }
    }
}

//----- StartupStepTimeoutMonitor.cs END -----
