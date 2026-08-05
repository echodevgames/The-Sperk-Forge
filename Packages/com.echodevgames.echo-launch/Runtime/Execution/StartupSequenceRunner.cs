//----- StartupSequenceRunner.cs START -----

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Executes enabled startup-sequence entries in authored order.
    ///
    /// FL-M3-06 preserves complete authored preflight and runner re-entry
    /// protection while forwarding accepted sequence, step, progress, and
    /// completion observations to an optional internal lifecycle sink.
    ///
    /// Public step events, reports, presentation, and destination loading
    /// remain later checkpoints.
    /// </summary>
    internal sealed class StartupSequenceRunner
    {
        private const int NoStoppingIndex = -1;

        private const string TimeoutDiagnosticCode =
            "ELAUNCH-STEP-003";

        private const string CallerCancellationDiagnosticCode =
            "ELAUNCH-STEP-005";

        private const string ReentryDiagnosticCode =
            "ELAUNCH-RUN-001";

        private readonly ILaunchClock clock;
        private int activeRunState;

        /// <summary>
        /// Creates a runner using Unity's unscaled real-time clock.
        /// </summary>
        internal StartupSequenceRunner()
            : this(
                UnityLaunchClock.Shared)
        {
        }

        /// <summary>
        /// Creates a runner using an explicit monotonic clock.
        /// </summary>
        internal StartupSequenceRunner(
            ILaunchClock clock)
        {
            this.clock =
                clock ??
                throw new ArgumentNullException(
                    nameof(clock));
        }

        /// <summary>
        /// Traverses one configured startup sequence and awaits each enabled
        /// entry's fresh executor.
        ///
        /// One runner instance permits only one active traversal. The gate is
        /// released after every terminal path so the instance may be reused
        /// sequentially.
        /// </summary>
        internal async Awaitable<StartupSequenceRunResult>
            RunAsync(
                LaunchMode launchMode,
                EchoLaunchConfiguration configuration,
                CancellationToken cancellationToken)
        {
            try
            {
                return await RunAsync(
                    launchMode,
                    configuration,
                    cancellationToken,
                    null);
            }
            catch (StartupSequencePreflightException exception)
            {
                throw new InvalidOperationException(
                    exception.Message,
                    exception);
            }
        }

        /// <summary>
        /// Traverses one configured startup sequence and forwards accepted
        /// lifecycle observations to an optional internal sink.
        /// </summary>
        internal async Awaitable<StartupSequenceRunResult>
            RunAsync(
                LaunchMode launchMode,
                EchoLaunchConfiguration configuration,
                CancellationToken cancellationToken,
                IStartupSequenceObserver observer)
        {
            if (Interlocked.CompareExchange(
                    ref activeRunState,
                    1,
                    0) != 0)
            {
                throw new InvalidOperationException(
                    $"[{ReentryDiagnosticCode}] " +
                    "The startup-sequence runner already owns an active run.");
            }

            try
            {
                return await RunCoreAsync(
                    launchMode,
                    configuration,
                    cancellationToken,
                    observer);
            }
            finally
            {
                Volatile.Write(
                    ref activeRunState,
                    0);
            }
        }

        private async Awaitable<StartupSequenceRunResult>
            RunCoreAsync(
                LaunchMode launchMode,
                EchoLaunchConfiguration configuration,
                CancellationToken cancellationToken,
                IStartupSequenceObserver observer)
        {
            StartupSequence sequence =
                StartupSequencePreflight.Validate(
                    launchMode,
                    configuration);

            observer?.SequenceValidated(
                sequence);

            int authoredEntryCount =
                sequence.EntryCount;

            int disabledEntryCount = 0;

            int stoppingAuthoredEntryIndex =
                NoStoppingIndex;

            List<StartupStepExecution>
                completedExecutions =
                    new List<StartupStepExecution>();

            for (int index = 0;
                 index < authoredEntryCount;
                 index++)
            {
                StartupSequenceEntry entry =
                    sequence.GetEntry(index);

                if (entry == null)
                {
                    throw new InvalidOperationException(
                        $"Startup-sequence entry {index} is null.");
                }

                if (!entry.IsEnabled)
                {
                    disabledEntryCount++;
                    continue;
                }

                StartupStepDefinition definition =
                    entry.StepDefinition;

                if (definition == null)
                {
                    throw new InvalidOperationException(
                        $"Enabled startup-sequence entry {index} does not reference a step definition.");
                }

                StartupStepExecution execution =
                    new StartupStepExecution(
                        entry,
                        index,
                        authoredEntryCount);

                if (!execution.Policy.IsValid)
                {
                    execution.CompleteBeforeStart(
                        StartupStepResult.BlockingFailure(
                            StartupStepExceptionConverter
                                .DiagnosticCode,
                            "The startup-step policy contains unsupported authored values.",
                            "ContractFailure: InvalidPolicy"));

                    completedExecutions.Add(
                        execution);

                    observer?.StepCompleted(
                        execution);

                    stoppingAuthoredEntryIndex =
                        index;

                    break;
                }

                IStartupStepExecutor executor;

                try
                {
                    executor =
                        definition.CreateExecutor();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    StartupStepResult factoryFailure =
                        StartupStepExceptionConverter
                            .Convert(
                                StartupStepExceptionPhase
                                    .ExecutorFactory,
                                exception);

                    execution.CompleteBeforeStart(
                        factoryFailure);

                    completedExecutions.Add(
                        execution);

                    observer?.StepCompleted(
                        execution);

                    stoppingAuthoredEntryIndex =
                        index;

                    break;
                }

                if (executor == null)
                {
                    execution.CompleteBeforeStart(
                        StartupStepExceptionConverter
                            .CreateNullExecutorResult());

                    completedExecutions.Add(
                        execution);

                    observer?.StepCompleted(
                        execution);

                    stoppingAuthoredEntryIndex =
                        index;

                    break;
                }

                execution.AttachExecutor(
                    executor);

                double startSeconds;

                try
                {
                    startSeconds =
                        ReadStartTime();
                }
                catch (Exception exception)
                {
                    execution.CompleteBeforeStart(
                        CreateClockContractFailure(
                            exception));

                    completedExecutions.Add(
                        execution);

                    observer?.StepCompleted(
                        execution);

                    stoppingAuthoredEntryIndex =
                        index;

                    break;
                }

                using (
                    CancellationTokenSource
                        timeoutCancellationSource =
                            new CancellationTokenSource())
                using (
                    CancellationTokenSource
                        linkedCancellationSource =
                            CancellationTokenSource
                                .CreateLinkedTokenSource(
                                    cancellationToken,
                                    timeoutCancellationSource
                                        .Token))
                {
                    StartupStepProgressRelay progressRelay =
                        new StartupStepProgressRelay(
                            execution,
                            observer);

                    StartupStepProgressGate progressGate =
                        new StartupStepProgressGate(
                            progressRelay);

                    StartupStepContext context =
                        new StartupStepContext(
                            launchMode,
                            configuration.ConfigurationId,
                            sequence.SequenceId,
                            execution.EntryId,
                            execution.StepId,
                            index,
                            authoredEntryCount,
                            linkedCancellationSource.Token,
                            progressGate);

                    execution.Begin();

                    observer?.StepStarted(
                        execution);

                    Awaitable<StartupStepResult>
                        executorAwaitable =
                            InvokeExecutorAsync(
                                executor,
                                context);

                    StartupStepAwaitOutcome outcome;

                    try
                    {
                        outcome =
                            await StartupStepTimeoutMonitor
                                .MonitorAsync(
                                    executorAwaitable,
                                    execution.Policy,
                                    clock,
                                    startSeconds,
                                    timeoutCancellationSource,
                                    progressGate,
                                    cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        execution.Complete(
                            CreateClockContractFailure(
                                exception),
                            StartupStepTiming
                                .NotMeasured);

                        completedExecutions.Add(
                            execution);

                        observer?.StepCompleted(
                            execution);

                        stoppingAuthoredEntryIndex =
                            index;

                        break;
                    }

                    if (outcome
                        .CallerCancellationObserved)
                    {
                        execution.Complete(
                            CreateCallerCancellationResult(
                                outcome),
                            outcome.Timing);

                        completedExecutions.Add(
                            execution);

                        observer?.StepCompleted(
                            execution);

                        stoppingAuthoredEntryIndex =
                            index;

                        break;
                    }

                    StartupStepResult originalResult;

                    if (outcome.TimedOut)
                    {
                        originalResult =
                            CreateTimeoutResult(
                                outcome.Timing);
                    }
                    else if (outcome
                        .HasExecutorException)
                    {
                        if (outcome.ExecutorException is
                            OperationCanceledException)
                        {
                            throw outcome
                                .ExecutorException;
                        }

                        originalResult =
                            StartupStepExceptionConverter
                                .Convert(
                                    StartupStepExceptionPhase
                                        .ExecutorExecution,
                                    outcome
                                        .ExecutorException);
                    }
                    else
                    {
                        originalResult =
                            outcome.ExecutorResult;
                    }

                    if (originalResult == null)
                    {
                        execution.Complete(
                            StartupStepExceptionConverter
                                .CreateNullResult(),
                            outcome.Timing);

                        completedExecutions.Add(
                            execution);

                        observer?.StepCompleted(
                            execution);

                        stoppingAuthoredEntryIndex =
                            index;

                        break;
                    }

                    StartupStepPolicyDecision decision =
                        ApplyPolicy(
                            execution.Policy,
                            originalResult);

                    execution.Complete(
                        decision.EffectiveResult,
                        outcome.Timing);

                    completedExecutions.Add(
                        execution);

                    observer?.StepCompleted(
                        execution);

                    if (decision.StopsTraversal)
                    {
                        stoppingAuthoredEntryIndex =
                            index;

                        break;
                    }
                }
            }

            return new StartupSequenceRunResult(
                authoredEntryCount,
                disabledEntryCount,
                completedExecutions,
                stoppingAuthoredEntryIndex);
        }

        private static async Awaitable<
            StartupStepResult>
            InvokeExecutorAsync(
                IStartupStepExecutor executor,
                StartupStepContext context)
        {
            return await executor.ExecuteAsync(
                context);
        }

        private double ReadStartTime()
        {
            double startSeconds =
                clock.NowSeconds;

            if (double.IsNaN(startSeconds) ||
                double.IsInfinity(startSeconds) ||
                startSeconds < 0d)
            {
                throw new InvalidOperationException(
                    "The launch clock returned an invalid startup-step start time.");
            }

            return startSeconds;
        }

        private static StartupStepResult
            CreateTimeoutResult(
                StartupStepTiming timing)
        {
            string details =
                string.Format(
                    CultureInfo.InvariantCulture,
                    "TimeoutSeconds: {0:0.###}\n" +
                    "ElapsedSeconds: {1:0.###}\n" +
                    "CancellationRequested: {2}",
                    timing.TimeoutSeconds,
                    timing.ElapsedSeconds,
                    timing.CancellationRequested);

            return StartupStepResult.TimedOut(
                TimeoutDiagnosticCode,
                "The startup step exceeded its configured timeout.",
                details);
        }

        private static StartupStepResult
            CreateCallerCancellationResult(
                StartupStepAwaitOutcome outcome)
        {
            if (outcome == null)
            {
                throw new ArgumentNullException(
                    nameof(outcome));
            }

            string details =
                string.Format(
                    CultureInfo.InvariantCulture,
                    "ElapsedSeconds: {0:0.###}\n" +
                    "ExecutorCompletedWithoutException: {1}",
                    outcome.Timing.ElapsedSeconds,
                    outcome.CompletedWithoutException);

            return StartupStepResult.Cancelled(
                CallerCancellationDiagnosticCode,
                "Startup-sequence execution was cancelled by the caller.",
                details);
        }

        private static StartupStepResult
            CreateClockContractFailure(
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

            string message =
                string.IsNullOrWhiteSpace(
                    exception.Message)
                    ? string.Empty
                    : exception.Message.Trim();

            string details =
                string.IsNullOrEmpty(message)
                    ? $"ExceptionType: {exceptionType}"
                    : $"ExceptionType: {exceptionType}\n" +
                      $"ExceptionMessage: {message}";

            return StartupStepResult
                .BlockingFailure(
                    StartupStepExceptionConverter
                        .DiagnosticCode,
                    "The startup-step timing system violated its runtime contract.",
                    details);
        }

        private static StartupStepPolicyDecision
            ApplyPolicy(
                StartupStepPolicy policy,
                StartupStepResult originalResult)
        {
            if (policy.IsValid)
            {
                return StartupStepPolicyEvaluator
                    .Evaluate(
                        policy,
                        originalResult);
            }

            StartupStepResult invalidPolicyResult =
                StartupStepResult.BlockingFailure(
                    StartupStepExceptionConverter
                        .DiagnosticCode,
                    "The startup-step policy contains unsupported authored values.",
                    "ContractFailure: InvalidPolicy");

            return new StartupStepPolicyDecision(
                originalResult,
                invalidPolicyResult,
                false);
        }
    }
}

//----- StartupSequenceRunner.cs END -----
