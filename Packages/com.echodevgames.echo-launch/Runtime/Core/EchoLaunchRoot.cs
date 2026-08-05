//----- EchoLaunchRoot.cs START -----

using System;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Scene-facing authority root for First Light.
    ///
    /// The authoritative root owns one explicit startup-sequence run,
    /// lifecycle publication, cooperative cancellation, immutable terminal
    /// reporting, and destruction-safe settlement. Automatic startup,
    /// successful destination handoff, presentation, and scene loading remain
    /// later checkpoints.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EchoLaunchRoot : MonoBehaviour
    {
        internal const string DuplicateDiagnosticCode =
            "ELAUNCH-ROOT-001";

        internal const string LifecycleDiagnosticCode =
            "ELAUNCH-LIFE-001";

        internal const string StartGateDiagnosticCode =
            "ELAUNCH-LIFE-002";

        [Header("Launch")]
        [SerializeField]
        private EchoLaunchConfiguration configuration;

        [SerializeField]
        private LaunchMode launchMode =
            LaunchMode.CanonicalBoot;

        private LaunchSession session;

        private StartupSequenceRunner sequenceRunner;

        private CancellationTokenSource
            launchCancellationSource;

        private StartupSequenceRunResult
            lastSequenceRunResult;

        private LaunchReportBuilder
            launchReportBuilder;

        private LaunchReport lastReport;

        private string cancellationReason =
            string.Empty;

        private double launchStartSeconds;

        private int activeLaunchState;

        private bool isDestroying;

        /// <summary>
        /// Raised after an accepted snapshot changes the launch lifecycle state.
        /// </summary>
        public event Action<LaunchStateChangedEvent>
            LaunchStateChanged;

        /// <summary>
        /// Raised after every accepted authoritative progress snapshot.
        /// </summary>
        public event Action<LaunchProgressChangedEvent>
            LaunchProgressChanged;

        /// <summary>
        /// Raised after a failed report is finalized and authoritative state is
        /// already <see cref="LaunchStatus.Failed"/>.
        /// </summary>
        public event Action<LaunchReport>
            LaunchFailed;

        /// <summary>
        /// Raised after an interrupted report is finalized and authoritative
        /// state is already <see cref="LaunchStatus.Interrupted"/>.
        /// </summary>
        public event Action<LaunchReport>
            LaunchInterrupted;

        /// <summary>
        /// Returns the currently authoritative First Light root.
        /// </summary>
        public static EchoLaunchRoot Current =>
            LaunchAuthorityClaim.Current
                as EchoLaunchRoot;

        /// <summary>
        /// Returns true when this component currently owns launch authority.
        /// </summary>
        public bool IsAuthoritative =>
            ReferenceEquals(Current, this);

        /// <summary>
        /// Returns true when this component was rejected as a duplicate.
        /// </summary>
        public bool WasRejectedAsDuplicate
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the project-owned launch configuration assigned to the
        /// authoritative root.
        /// </summary>
        public EchoLaunchConfiguration Configuration =>
            IsAuthoritative
                ? configuration
                : null;

        /// <summary>
        /// Gets the current authoritative launch state.
        /// </summary>
        public LaunchStatus State =>
            IsAuthoritative &&
            session != null
                ? session.State
                : LaunchStatus.None;

        /// <summary>
        /// Gets the latest authoritative launch progress snapshot.
        /// </summary>
        public LaunchProgressSnapshot Progress =>
            IsAuthoritative &&
            session != null
                ? session.Progress
                : LaunchProgressSnapshot.Empty;

        /// <summary>
        /// Gets the latest immutable finalized launch report.
        ///
        /// FL-M3-07 finalizes failed and interrupted attempts only. A successful
        /// sequence remains report-pending while the root is Transitioning.
        /// </summary>
        public LaunchReport LastReport =>
            IsAuthoritative
                ? lastReport
                : null;

        /// <summary>
        /// Requests cooperative cancellation of the active root-owned launch.
        ///
        /// Returns false when this root is not authoritative, no launch is
        /// active, or cancellation was already requested.
        /// </summary>
        public bool CancelLaunch(
            string reason)
        {
            if (!IsAuthoritative ||
                Volatile.Read(
                    ref activeLaunchState) == 0)
            {
                return false;
            }

            CancellationTokenSource source =
                launchCancellationSource;

            if (source == null ||
                source.IsCancellationRequested)
            {
                return false;
            }

            cancellationReason =
                NormalizeCancellationReason(
                    reason);

            source.Cancel();
            return true;
        }

        /// <summary>
        /// Gets whether this root currently owns an unsettled startup run.
        /// </summary>
        internal bool IsLaunchActive =>
            Volatile.Read(
                ref activeLaunchState) != 0;

        /// <summary>
        /// Gets the latest settled sequence result owned by this root.
        /// </summary>
        internal StartupSequenceRunResult
            LastSequenceRunResult =>
                lastSequenceRunResult;

        /// <summary>
        /// Gets whether successful sequence data is retained for the later
        /// destination handoff without a finalized public report.
        /// </summary>
        internal bool HasPendingLaunchReport =>
            launchReportBuilder != null &&
            launchReportBuilder
                .HasPendingSuccessfulRun;

        private void Awake()
        {
            if (LaunchAuthorityClaim.TryClaim(this))
            {
                WasRejectedAsDuplicate = false;

                try
                {
                    session =
                        new LaunchSession(
                            launchMode);

                    sequenceRunner =
                        new StartupSequenceRunner();
                }
                catch
                {
                    LaunchAuthorityClaim.Release(
                        this);

                    throw;
                }

                return;
            }

            WasRejectedAsDuplicate = true;

            // Disable before performing any future startup behavior.
            enabled = false;

            Debug.LogWarning(
                $"[{DuplicateDiagnosticCode}] " +
                "Duplicate EchoLaunchRoot rejected. " +
                "The first valid root remains authoritative.",
                this);
        }

        private void OnDestroy()
        {
            isDestroying = true;

            if (Volatile.Read(
                    ref activeLaunchState) != 0)
            {
                cancellationReason =
                    "The launch root was destroyed.";

                CancellationTokenSource source =
                    launchCancellationSource;

                if (source != null &&
                    !source.IsCancellationRequested)
                {
                    source.Cancel();
                }
            }
            else
            {
                DisposeLaunchCancellationSource();
            }

            LaunchStateChanged = null;
            LaunchProgressChanged = null;
            LaunchFailed = null;
            LaunchInterrupted = null;

            session = null;
            sequenceRunner = null;
            lastSequenceRunResult = null;
            launchReportBuilder = null;
            lastReport = null;

            LaunchAuthorityClaim.Release(this);
        }

        /// <summary>
        /// Explicitly begins the one startup-sequence run owned by this root.
        ///
        /// First Light deliberately does not call this method from Awake, Start,
        /// or a scene callback. Automatic startup remains a later checkpoint.
        /// </summary>
        internal async Awaitable<StartupSequenceRunResult>
            StartLaunchAsync()
        {
            EnsureMayStart();

            if (Interlocked.CompareExchange(
                    ref activeLaunchState,
                    1,
                    0) != 0)
            {
                throw CreateStartGateException();
            }

            try
            {
                launchCancellationSource =
                    new CancellationTokenSource();

                cancellationReason =
                    string.Empty;

                lastSequenceRunResult = null;
                lastReport = null;

                launchStartSeconds =
                    GetMonotonicNow();

                launchReportBuilder =
                    new LaunchReportBuilder(
                        launchMode,
                        configuration,
                        launchStartSeconds);

                PublishValidationStarted();

                StartupSequenceRunner runner =
                    sequenceRunner ??
                    new StartupSequenceRunner();

                sequenceRunner = runner;

                RootSequenceObserver observer =
                    new RootSequenceObserver(
                        this);

                try
                {
                    lastSequenceRunResult =
                        await runner.RunAsync(
                            launchMode,
                            configuration,
                            launchCancellationSource
                                .Token,
                            observer);
                }
                catch (
                    StartupSequencePreflightException
                        exception)
                {
                    PublishPreflightFailure(
                        exception);

                    return null;
                }
                catch (
                    ArgumentNullException exception)
                {
                    PublishUnexpectedFailure(
                        StartupSequencePreflight
                            .ConfigurationDiagnosticCode,
                        "The launch configuration is missing.",
                        exception);

                    return null;
                }
                catch (
                    ArgumentOutOfRangeException
                        exception)
                {
                    PublishUnexpectedFailure(
                        StartupSequencePreflight
                            .ConfigurationDiagnosticCode,
                        "The active launch mode is invalid.",
                        exception);

                    return null;
                }
                catch (
                    OperationCanceledException
                        exception)
                {
                    if (IsCancellationRequested)
                    {
                        PublishInterrupted(
                            null,
                            CreateLifecycleCancellationResult(
                                exception));

                        return null;
                    }

                    PublishUnexpectedFailure(
                        StartupStepExceptionConverter
                            .DiagnosticCode,
                        "Startup-sequence execution was cancelled without an active root cancellation request.",
                        exception);

                    return null;
                }
                catch (Exception exception)
                {
                    PublishUnexpectedFailure(
                        StartupStepExceptionConverter
                            .DiagnosticCode,
                        "Startup-sequence execution failed unexpectedly.",
                        exception);

                    return null;
                }

                if (!CanPublishRuntimeProgress)
                {
                    return lastSequenceRunResult;
                }

                if (lastSequenceRunResult == null)
                {
                    PublishUnexpectedFailure(
                        StartupStepExceptionConverter
                            .DiagnosticCode,
                        "Startup-sequence execution returned no run result.",
                        null);

                    return null;
                }

                if (lastSequenceRunResult
                    .WasCancelled ||
                    IsCancellationRequested)
                {
                    PublishInterrupted(
                        lastSequenceRunResult,
                        GetFinalResult(
                            lastSequenceRunResult));

                    return lastSequenceRunResult;
                }

                if (lastSequenceRunResult
                        .HasBlockingFailures ||
                    lastSequenceRunResult
                        .HasFailures)
                {
                    PublishFailedRun(
                        lastSequenceRunResult);

                    return lastSequenceRunResult;
                }

                PublishTransitionPending(
                    lastSequenceRunResult);

                return lastSequenceRunResult;
            }
            finally
            {
                DisposeLaunchCancellationSource();

                Volatile.Write(
                    ref activeLaunchState,
                    0);
            }
        }

        /// <summary>
        /// Replaces the runner before launch begins for deterministic runtime
        /// tests. Production uses the default Unity-clock runner.
        /// </summary>
        internal void SetSequenceRunnerForTesting(
            StartupSequenceRunner runner)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(
                    nameof(runner));
            }

            if (!IsAuthoritative ||
                session == null ||
                session.State !=
                    LaunchStatus.AuthorityClaimed ||
                IsLaunchActive)
            {
                throw new InvalidOperationException(
                    $"[{StartGateDiagnosticCode}] " +
                    "The sequence runner may be replaced only on an idle authoritative root before launch begins.");
            }

            sequenceRunner = runner;
        }

        /// <summary>
        /// Replaces the authoritative root's current progress snapshot and
        /// safely notifies observers after the snapshot is accepted.
        /// </summary>
        internal void PublishProgress(
            LaunchProgressSnapshot snapshot)
        {
            if (!IsAuthoritative)
            {
                throw new InvalidOperationException(
                    "Only the authoritative EchoLaunchRoot may publish launch progress.");
            }

            if (session == null)
            {
                throw new InvalidOperationException(
                    "The authoritative EchoLaunchRoot does not have an active launch session.");
            }

            LaunchProgressSnapshot previous =
                session.Progress;

            session.Publish(snapshot);

            LaunchProgressSnapshot current =
                session.Progress;

            if (previous.Status !=
                current.Status)
            {
                LaunchStateChangedEvent stateEvent =
                    new LaunchStateChangedEvent(
                        previous.Status,
                        current.Status,
                        current);

                LaunchNotificationDispatcher.Dispatch(
                    LaunchStateChanged,
                    stateEvent,
                    nameof(LaunchStateChanged),
                    this);
            }

            LaunchProgressChangedEvent progressEvent =
                new LaunchProgressChangedEvent(
                    previous,
                    current);

            LaunchNotificationDispatcher.Dispatch(
                LaunchProgressChanged,
                progressEvent,
                nameof(LaunchProgressChanged),
                this);
        }

        private bool CanPublishRuntimeProgress =>
            !isDestroying &&
            IsAuthoritative &&
            session != null;

        private bool IsCancellationRequested =>
            launchCancellationSource != null &&
            launchCancellationSource
                .IsCancellationRequested;

        private void EnsureMayStart()
        {
            if (isDestroying ||
                !IsAuthoritative ||
                session == null)
            {
                throw new InvalidOperationException(
                    $"[{StartGateDiagnosticCode}] " +
                    "Only a live authoritative EchoLaunchRoot may start launch execution.");
            }

            if (session.State !=
                LaunchStatus.AuthorityClaimed ||
                IsLaunchActive)
            {
                throw CreateStartGateException();
            }
        }

        private static InvalidOperationException
            CreateStartGateException()
        {
            return new InvalidOperationException(
                $"[{StartGateDiagnosticCode}] " +
                "The launch root already owns an active or previously advanced launch session.");
        }

        private void PublishValidationStarted()
        {
            if (!CanPublishRuntimeProgress)
            {
                return;
            }

            PublishProgress(
                new LaunchProgressSnapshot(
                    launchMode,
                    LaunchStatus.Validating,
                    string.Empty,
                    -1,
                    GetAuthoredEntryCount(),
                    0f,
                    true,
                    "Validating launch configuration.",
                    GetElapsedSeconds(),
                    null));
        }

        private void OnSequenceValidated(
            StartupSequence sequence)
        {
            if (!CanPublishRuntimeProgress)
            {
                return;
            }

            launchReportBuilder?
                .RecordSequenceValidated(
                    sequence);

            PublishProgress(
                new LaunchProgressSnapshot(
                    launchMode,
                    LaunchStatus.Running,
                    string.Empty,
                    -1,
                    sequence == null
                        ? 0
                        : sequence.EntryCount,
                    0f,
                    true,
                    "Startup sequence validated.",
                    GetElapsedSeconds(),
                    null));
        }

        private void OnStepStarted(
            StartupStepExecution execution)
        {
            if (!CanPublishRuntimeProgress ||
                execution == null)
            {
                return;
            }

            StartupStepProgress progress =
                execution.LatestProgress;

            PublishProgress(
                CreateExecutionSnapshot(
                    execution,
                    progress.Progress01,
                    progress.IsIndeterminate,
                    string.IsNullOrEmpty(
                        progress.Message)
                        ? $"Starting {execution.StepDisplayName}."
                        : progress.Message,
                    null));
        }

        private void OnStepProgressChanged(
            StartupStepExecution execution,
            StartupStepProgress progress)
        {
            if (!CanPublishRuntimeProgress ||
                execution == null)
            {
                return;
            }

            PublishProgress(
                CreateExecutionSnapshot(
                    execution,
                    progress.Progress01,
                    progress.IsIndeterminate,
                    string.IsNullOrEmpty(
                        progress.Message)
                        ? execution.StepDisplayName
                        : progress.Message,
                    null));
        }

        private void OnStepCompleted(
            StartupStepExecution execution)
        {
            if (!CanPublishRuntimeProgress ||
                execution == null ||
                !execution.IsComplete)
            {
                return;
            }

            launchReportBuilder?
                .RecordStepCompleted(
                    execution);

            StartupStepResult result =
                execution.Result;

            PublishProgress(
                CreateExecutionSnapshot(
                    execution,
                    1f,
                    false,
                    string.IsNullOrEmpty(
                        result.Message)
                        ? execution.StepDisplayName
                        : result.Message,
                    result));
        }

        private LaunchProgressSnapshot
            CreateExecutionSnapshot(
                StartupStepExecution execution,
                float progress01,
                bool isIndeterminate,
                string message,
                StartupStepResult lastResult)
        {
            return new LaunchProgressSnapshot(
                launchMode,
                LaunchStatus.Running,
                execution.StepId,
                execution.StepIndex,
                execution.StepCount,
                progress01,
                isIndeterminate,
                message,
                GetElapsedSeconds(),
                lastResult);
        }

        private void PublishPreflightFailure(
            StartupSequencePreflightException
                exception)
        {
            StartupStepResult result =
                StartupStepResult.BlockingFailure(
                    exception.DiagnosticCode,
                    exception.FailureMessage,
                    string.Empty);

            PublishFailureAndReport(
                null,
                string.Empty,
                -1,
                GetAuthoredEntryCount(),
                0f,
                true,
                result);
        }

        private void PublishUnexpectedFailure(
            string diagnosticCode,
            string message,
            Exception exception)
        {
            StartupStepResult result =
                StartupStepResult.BlockingFailure(
                    diagnosticCode,
                    message,
                    CreateExceptionDetails(
                        exception));

            PublishFailureAndReport(
                null,
                string.Empty,
                -1,
                GetAuthoredEntryCount(),
                0f,
                true,
                result);
        }

        private void PublishFailedRun(
            StartupSequenceRunResult result)
        {
            StartupStepExecution finalExecution =
                GetFinalExecution(result);

            StartupStepResult finalResult =
                finalExecution == null
                    ? StartupStepResult
                        .BlockingFailure(
                            StartupStepExceptionConverter
                                .DiagnosticCode,
                            "Startup-sequence execution failed.",
                            string.Empty)
                    : finalExecution.Result;

            PublishFailureAndReport(
                result,
                finalExecution == null
                    ? string.Empty
                    : finalExecution.StepId,
                finalExecution == null
                    ? -1
                    : finalExecution.StepIndex,
                result.AuthoredEntryCount,
                finalExecution == null
                    ? 0f
                    : 1f,
                finalExecution == null,
                finalResult);
        }

        private void PublishFailureAndReport(
            StartupSequenceRunResult runResult,
            string stepId,
            int stepIndex,
            int totalStepCount,
            float progress01,
            bool isIndeterminate,
            StartupStepResult finalResult)
        {
            if (!PublishTerminalSnapshot(
                    LaunchStatus.Failed,
                    stepId,
                    stepIndex,
                    totalStepCount,
                    progress01,
                    isIndeterminate,
                    finalResult.Message,
                    finalResult))
            {
                return;
            }

            FinalizeTerminalReport(
                LaunchStatus.Failed,
                runResult,
                finalResult);
        }

        private void PublishInterrupted(
            StartupSequenceRunResult runResult,
            StartupStepResult cancellationResult)
        {
            StartupStepExecution finalExecution =
                GetFinalExecution(
                    runResult);

            StartupStepResult sourceResult =
                cancellationResult ??
                StartupStepResult.Cancelled(
                    LifecycleDiagnosticCode,
                    "Launch execution was interrupted.",
                    string.Empty);

            string message =
                string.IsNullOrEmpty(
                    cancellationReason)
                    ? sourceResult.Message
                    : cancellationReason;

            string diagnosticCode =
                string.IsNullOrWhiteSpace(
                    sourceResult.Code)
                    ? LifecycleDiagnosticCode
                    : sourceResult.Code;

            StartupStepResult reportResult =
                StartupStepResult.Cancelled(
                    diagnosticCode,
                    message,
                    sourceResult.Details);

            if (!PublishTerminalSnapshot(
                    LaunchStatus.Interrupted,
                    finalExecution == null
                        ? string.Empty
                        : finalExecution.StepId,
                    finalExecution == null
                        ? -1
                        : finalExecution.StepIndex,
                    runResult == null
                        ? GetAuthoredEntryCount()
                        : runResult.AuthoredEntryCount,
                    finalExecution == null
                        ? 0f
                        : 1f,
                    finalExecution == null,
                    reportResult.Message,
                    reportResult))
            {
                return;
            }

            FinalizeTerminalReport(
                LaunchStatus.Interrupted,
                runResult,
                reportResult);
        }

        private void PublishTransitionPending(
            StartupSequenceRunResult result)
        {
            StartupStepResult finalResult =
                GetFinalResult(
                    result);

            if (!PublishTerminalSnapshot(
                    LaunchStatus.Transitioning,
                    string.Empty,
                    -1,
                    result.AuthoredEntryCount,
                    1f,
                    false,
                    "Startup sequence completed. Initial destination transition is pending.",
                    finalResult))
            {
                return;
            }

            launchReportBuilder?
                .MarkTransitionPending(
                    result);
        }

        private bool PublishTerminalSnapshot(
            LaunchStatus status,
            string stepId,
            int stepIndex,
            int totalStepCount,
            float progress01,
            bool isIndeterminate,
            string message,
            StartupStepResult lastResult)
        {
            if (!CanPublishRuntimeProgress)
            {
                return false;
            }

            PublishProgress(
                new LaunchProgressSnapshot(
                    launchMode,
                    status,
                    stepId,
                    stepIndex,
                    totalStepCount,
                    progress01,
                    isIndeterminate,
                    message,
                    GetElapsedSeconds(),
                    lastResult));

            return true;
        }

        private void FinalizeTerminalReport(
            LaunchStatus finalStatus,
            StartupSequenceRunResult runResult,
            StartupStepResult finalResult)
        {
            if (!CanPublishRuntimeProgress ||
                launchReportBuilder == null ||
                lastReport != null)
            {
                return;
            }

            LaunchReport report =
                launchReportBuilder
                    .FinalizeReport(
                        finalStatus,
                        runResult,
                        finalResult,
                        GetMonotonicNow());

            lastReport = report;

            if (finalStatus ==
                LaunchStatus.Failed)
            {
                LaunchNotificationDispatcher.Dispatch(
                    LaunchFailed,
                    report,
                    nameof(LaunchFailed),
                    this);

                return;
            }

            LaunchNotificationDispatcher.Dispatch(
                LaunchInterrupted,
                report,
                nameof(LaunchInterrupted),
                this);
        }

        private int GetAuthoredEntryCount()
        {
            StartupSequence sequence =
                configuration == null
                    ? null
                    : configuration
                        .StartupSequence;

            return sequence == null
                ? 0
                : sequence.EntryCount;
        }

        private double GetElapsedSeconds()
        {
            double now =
                GetMonotonicNow();

            if (now < launchStartSeconds)
            {
                return 0d;
            }

            return now -
                   launchStartSeconds;
        }

        private static double GetMonotonicNow()
        {
            double now =
                Time.realtimeSinceStartupAsDouble;

            return double.IsNaN(now) ||
                   double.IsInfinity(now) ||
                   now < 0d
                ? 0d
                : now;
        }

        private static StartupStepExecution
            GetFinalExecution(
                StartupSequenceRunResult result)
        {
            if (result == null ||
                result.AttemptedExecutionCount == 0)
            {
                return null;
            }

            return result.GetExecution(
                result.AttemptedExecutionCount -
                1);
        }

        private static StartupStepResult
            GetFinalResult(
                StartupSequenceRunResult result)
        {
            StartupStepExecution execution =
                GetFinalExecution(
                    result);

            return execution == null
                ? null
                : execution.Result;
        }

        private static StartupStepResult
            CreateLifecycleCancellationResult(
                Exception exception)
        {
            return StartupStepResult.Cancelled(
                LifecycleDiagnosticCode,
                "Launch execution was interrupted.",
                CreateExceptionDetails(
                    exception));
        }

        private static string
            CreateExceptionDetails(
                Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            string typeName =
                exception.GetType().FullName;

            if (string.IsNullOrWhiteSpace(
                    typeName))
            {
                typeName =
                    exception.GetType().Name;
            }

            string message =
                string.IsNullOrWhiteSpace(
                    exception.Message)
                    ? string.Empty
                    : exception.Message.Trim();

            return string.IsNullOrEmpty(
                    message)
                ? $"ExceptionType: {typeName}"
                : $"ExceptionType: {typeName}\n" +
                  $"ExceptionMessage: {message}";
        }

        private void DisposeLaunchCancellationSource()
        {
            CancellationTokenSource source =
                launchCancellationSource;

            launchCancellationSource = null;

            if (source != null)
            {
                source.Dispose();
            }
        }

        private static string
            NormalizeCancellationReason(
                string reason)
        {
            return string.IsNullOrWhiteSpace(
                    reason)
                ? "Launch cancellation requested."
                : reason.Trim();
        }

        private sealed class RootSequenceObserver :
            IStartupSequenceObserver
        {
            private readonly EchoLaunchRoot root;

            internal RootSequenceObserver(
                EchoLaunchRoot root)
            {
                this.root =
                    root ??
                    throw new ArgumentNullException(
                        nameof(root));
            }

            public void SequenceValidated(
                StartupSequence sequence)
            {
                root.OnSequenceValidated(
                    sequence);
            }

            public void StepStarted(
                StartupStepExecution execution)
            {
                root.OnStepStarted(
                    execution);
            }

            public void StepProgressChanged(
                StartupStepExecution execution,
                StartupStepProgress progress)
            {
                root.OnStepProgressChanged(
                    execution,
                    progress);
            }

            public void StepCompleted(
                StartupStepExecution execution)
            {
                root.OnStepCompleted(
                    execution);
            }
        }
    }
}

//----- EchoLaunchRoot.cs END -----
