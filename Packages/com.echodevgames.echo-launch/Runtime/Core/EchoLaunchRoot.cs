//----- EchoLaunchRoot.cs START -----

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Scene-facing authority root for First Light.
    ///
    /// The authoritative root owns one explicit startup-sequence run,
    /// lifecycle publication, cooperative cancellation, immutable terminal
    /// reporting, initial destination handoff, automatic root startup, neutral
    /// status presentation, and destruction-safe settlement. Direct-scene
    /// helpers, default visual presentation, and normal mid-game scene travel
    /// remain later checkpoints.
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

        internal const string
            ConfigurationSchemaDiagnosticCode =
                "ELAUNCH-CFG-002";

        internal const string
            DestinationPreflightDiagnosticCode =
                "ELAUNCH-DEST-001";

        internal const string
            DestinationLoadDiagnosticCode =
                "ELAUNCH-DEST-002";

        internal const string
            PresenterUnavailableDiagnosticCode =
                "ELAUNCH-VIEW-001";

        internal const string
            PresenterFailureDiagnosticCode =
                "ELAUNCH-VIEW-002";

        internal const string
            SplashPreflightDiagnosticCode =
                "ELAUNCH-SPLASH-001";

        internal const string
            SplashPlaybackDiagnosticCode =
                "ELAUNCH-SPLASH-002";

        internal const string
            SplashPresenterUnavailableDiagnosticCode =
                "ELAUNCH-SPLASH-003";

        [Header("Launch")]
        [SerializeField]
        private EchoLaunchConfiguration configuration;

        [SerializeField]
        private LaunchMode launchMode =
            LaunchMode.CanonicalBoot;

        [SerializeField]
        private bool startAutomatically = true;

        [Header("Presentation")]
        [SerializeField]
        private MonoBehaviour statusPresenterComponent;

        private LaunchSession session;

        private ILaunchClock launchClock;

        private StartupSequenceRunner sequenceRunner;
        private bool sequenceRunnerWasInjected;

        private IInitialDestinationLoader
            initialDestinationLoader;

        private ILaunchStatusPresenter
            statusPresenter;

        private CancellationTokenSource
            launchCancellationSource;

        private StartupSequenceRunResult
            lastSequenceRunResult;

        private SplashPlaybackResult
            lastSplashPlaybackResult;

        private LaunchReportBuilder
            launchReportBuilder;

        private LaunchReport lastReport;

        private string cancellationReason =
            string.Empty;

        private double launchStartSeconds;

        private int activeLaunchState;

        private bool isDestroying;
        private bool isStatusPresenterBound;
        private bool statusPresenterWasInjected;
        private bool isTemporarilyPreservedForHandoff;
        private GameObject preservedHandoffObject;

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
        /// Raised after the configured initial destination is active, the
        /// authoritative state is already Completed, and the successful
        /// immutable report is stored in LastReport.
        /// </summary>
        public event Action<LaunchReport>
            LaunchCompleted;

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
        /// Gets the serialized configuration before authority filtering.
        /// Direct-scene validation uses this read-only evidence before a
        /// project-owned prefab is instantiated.
        /// </summary>
        internal EchoLaunchConfiguration AuthoredConfiguration =>
            configuration;

        /// <summary>
        /// Gets the serialized launch mode before authority filtering.
        /// </summary>
        internal LaunchMode AuthoredLaunchMode =>
            launchMode;

        /// <summary>
        /// Gets the project-owned initial destination assigned through the
        /// authoritative configuration.
        /// </summary>
        public LaunchDestination InitialDestination =>
            IsAuthoritative &&
            configuration != null
                ? configuration.InitialDestination
                : null;

        /// <summary>
        /// Gets the optional project-owned splash sequence assigned through
        /// the authoritative configuration.
        /// </summary>
        public SplashSequence SplashSequence =>
            IsAuthoritative &&
            configuration != null
                ? configuration.SplashSequence
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
        /// Failed, interrupted, and destination-activated completed attempts are
        /// finalized. No report is exposed before terminal finalization.
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
        /// Gets whether Unity's Start callback may begin the launch
        /// automatically.
        /// </summary>
        internal bool IsAutomaticStartEnabled =>
            startAutomatically;

        /// <summary>
        /// Gets whether the active presenter has accepted its initial binding.
        /// </summary>
        internal bool IsStatusPresenterBound =>
            isStatusPresenterBound;

        /// <summary>
        /// Gets the latest settled sequence result owned by this root.
        /// </summary>
        internal StartupSequenceRunResult
            LastSequenceRunResult =>
                lastSequenceRunResult;

        /// <summary>
        /// Gets the latest successfully completed root-owned splash result.
        /// </summary>
        internal SplashPlaybackResult
            LastSplashPlaybackResult =>
                lastSplashPlaybackResult;

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

                    launchClock =
                        UnityLaunchClock.Shared;

                    sequenceRunner =
                        new StartupSequenceRunner(
                            launchClock);

                    sequenceRunnerWasInjected =
                        false;

                    initialDestinationLoader =
                        UnityInitialDestinationLoader
                            .Shared;

                    ResolveStatusPresenter();
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

        private async Awaitable Start()
        {
            if (!startAutomatically ||
                !MayStartAutomatically)
            {
                return;
            }

            await StartLaunchAsync();
        }

        private void OnDestroy()
        {
            isDestroying = true;

            UnbindStatusPresenter();

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
            LaunchCompleted = null;

            session = null;
            launchClock = null;
            sequenceRunner = null;
            sequenceRunnerWasInjected = false;
            initialDestinationLoader = null;
            statusPresenter = null;
            statusPresenterComponent = null;
            statusPresenterWasInjected = false;
            lastSequenceRunResult = null;
            lastSplashPlaybackResult = null;
            launchReportBuilder = null;
            lastReport = null;
            preservedHandoffObject = null;
            isTemporarilyPreservedForHandoff = false;

            LaunchAuthorityClaim.Release(this);
        }

        /// <summary>
        /// Explicitly begins the one startup-sequence run owned by this root.
        ///
        /// Unity's Start callback uses this same gate when automatic startup is
        /// enabled. Tests and development helpers may still invoke it directly
        /// after disabling automatic startup through the internal test seam.
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
                lastSplashPlaybackResult = null;
                lastReport = null;

                launchStartSeconds =
                    GetMonotonicNow();

                launchReportBuilder =
                    new LaunchReportBuilder(
                        launchMode,
                        configuration,
                        launchStartSeconds);

                BindStatusPresenter();
                PublishValidationStarted();

                LaunchDestination initialDestination;
                SplashSequence splashSequence;

                try
                {
                    initialDestination =
                        ValidateInitialDestination(
                            out splashSequence);
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

                if (splashSequence != null)
                {
                    try
                    {
                        lastSplashPlaybackResult =
                            await PlayConfiguredSplashAsync(
                                splashSequence,
                                launchCancellationSource
                                    .Token);
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
                            SplashPlaybackDiagnosticCode,
                            "Splash playback was cancelled without an active root cancellation request.",
                            exception);

                        return null;
                    }
                    catch (Exception exception)
                    {
                        PublishUnexpectedFailure(
                            SplashPlaybackDiagnosticCode,
                            "Splash playback failed unexpectedly.",
                            exception);

                        return null;
                    }
                }

                if (!CanPublishRuntimeProgress)
                {
                    return null;
                }

                StartupSequenceRunner runner =
                    sequenceRunner ??
                    new StartupSequenceRunner(
                        launchClock ??
                        UnityLaunchClock.Shared);

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

                await CompleteInitialDestinationHandoffAsync(
                    lastSequenceRunResult,
                    initialDestination);

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
        /// Enables or disables Unity Start-driven launch before the session
        /// advances. This is an internal deterministic test seam.
        /// </summary>
        internal void SetAutomaticStartForTesting(
            bool enabled)
        {
            EnsureMayReplaceLaunchDependency(
                "Automatic startup");

            startAutomatically = enabled;
        }

        /// <summary>
        /// Replaces the neutral status presenter before launch begins for
        /// deterministic runtime tests.
        /// </summary>
        internal void SetStatusPresenterForTesting(
            ILaunchStatusPresenter presenter)
        {
            if (presenter == null)
            {
                throw new ArgumentNullException(
                    nameof(presenter));
            }

            EnsureMayReplaceLaunchDependency(
                "The status presenter");

            statusPresenter = presenter;
            statusPresenterWasInjected = true;
            statusPresenterComponent = null;
        }

        /// <summary>
        /// Replaces the root's monotonic launch clock before execution begins.
        /// The default startup runner is rebuilt against the same clock unless
        /// a runner was explicitly injected.
        /// </summary>
        internal void SetLaunchClockForTesting(
            ILaunchClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(
                    nameof(clock));
            }

            EnsureMayReplaceLaunchDependency(
                "The launch clock");

            launchClock = clock;

            if (!sequenceRunnerWasInjected)
            {
                sequenceRunner =
                    new StartupSequenceRunner(
                        launchClock);
            }

        }

        /// <summary>
        /// Replaces the runner before launch begins for deterministic runtime
        /// tests. Production uses the root launch clock.
        /// </summary>
        internal void SetSequenceRunnerForTesting(
            StartupSequenceRunner runner)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(
                    nameof(runner));
            }

            EnsureMayReplaceLaunchDependency(
                "The sequence runner");

            sequenceRunner = runner;
            sequenceRunnerWasInjected = true;
        }

        /// <summary>
        /// Replaces the initial destination loader before launch begins for
        /// deterministic runtime tests. Production uses the standalone Unity
        /// scene loader.
        /// </summary>
        internal void SetInitialDestinationLoaderForTesting(
            IInitialDestinationLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(
                    nameof(loader));
            }

            EnsureMayReplaceLaunchDependency(
                "The initial destination loader");

            initialDestinationLoader =
                loader;
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

            PresentStatusSnapshot(
                current);

            if (!CanPublishRuntimeProgress)
            {
                return;
            }

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

        private bool MayStartAutomatically =>
            !isDestroying &&
            IsAuthoritative &&
            session != null &&
            session.State ==
                LaunchStatus.AuthorityClaimed &&
            !IsLaunchActive;

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

        private void EnsureMayReplaceLaunchDependency(
            string dependencyName)
        {
            if (!IsAuthoritative ||
                session == null ||
                session.State !=
                    LaunchStatus.AuthorityClaimed ||
                IsLaunchActive)
            {
                throw new InvalidOperationException(
                    $"[{StartGateDiagnosticCode}] " +
                    dependencyName +
                    " may be changed only on an idle authoritative root before launch begins.");
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

        private bool PublishTransitionPending(
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
                    0f,
                    true,
                    "Startup sequence completed. Loading the initial destination.",
                    finalResult))
            {
                return false;
            }

            launchReportBuilder?
                .MarkTransitionPending(
                    result);

            return true;
        }


        private LaunchDestination
            ValidateInitialDestination(
                out SplashSequence splashSequence)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(
                    nameof(configuration));
            }

            if (!configuration.HasValidIdentity)
            {
                throw new StartupSequencePreflightException(
                    StartupSequencePreflight
                        .ConfigurationDiagnosticCode,
                    "The launch configuration identity is invalid.");
            }

            if (!configuration.HasSupportedSchema)
            {
                throw new StartupSequencePreflightException(
                    ConfigurationSchemaDiagnosticCode,
                    "The launch configuration schema version is unsupported. Schema 4 is required.");
            }

            splashSequence =
                SplashSequencePreflight.Validate(
                    configuration);

            StartupSequencePreflight.Validate(
                launchMode,
                configuration);

            LaunchDestination destination =
                configuration.InitialDestination;

            if (destination == null)
            {
                throw new StartupSequencePreflightException(
                    DestinationPreflightDiagnosticCode,
                    "The launch configuration does not reference an initial destination.");
            }

            if (!destination.HasValidIdentity)
            {
                throw new StartupSequencePreflightException(
                    DestinationPreflightDiagnosticCode,
                    "The initial destination identity is invalid.");
            }

            if (!destination.HasSupportedSchema)
            {
                throw new StartupSequencePreflightException(
                    DestinationPreflightDiagnosticCode,
                    "The initial destination schema version is unsupported.");
            }

            if (!destination.HasValidDisplayName)
            {
                throw new StartupSequencePreflightException(
                    DestinationPreflightDiagnosticCode,
                    "The initial destination display name is blank or not normalized.");
            }

            if (!destination.HasValidScenePath)
            {
                throw new StartupSequencePreflightException(
                    DestinationPreflightDiagnosticCode,
                    "The initial destination scene path is invalid.");
            }

            IInitialDestinationLoader loader =
                initialDestinationLoader;

            if (loader == null)
            {
                throw new StartupSequencePreflightException(
                    DestinationPreflightDiagnosticCode,
                    "The authoritative launch root does not have an initial destination loader.");
            }

            if (loader is
                    IInitialDestinationPreflightValidator
                        validator &&
                !validator.TryValidate(
                    destination,
                    out string failureMessage))
            {
                throw new StartupSequencePreflightException(
                    DestinationPreflightDiagnosticCode,
                    string.IsNullOrWhiteSpace(
                        failureMessage)
                        ? "The initial destination loader rejected the configured destination."
                        : failureMessage.Trim());
            }

            return destination;
        }

        private async Awaitable<SplashPlaybackResult>
            PlayConfiguredSplashAsync(
                SplashSequence sequence,
                CancellationToken cancellationToken)
        {
            if (sequence == null)
            {
                return null;
            }

            IImageSplashPresenter presenter =
                ResolveSplashPresenter(
                    sequence);

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    launchClock ??
                    UnityLaunchClock.Shared,
                    presenter);

            return await player.PlayAsync(
                sequence,
                configuration != null &&
                configuration
                    .UseReducedMotionForSplash,
                cancellationToken);
        }

        private IImageSplashPresenter
            ResolveSplashPresenter(
                SplashSequence sequence)
        {
            if (sequence == null ||
                sequence.EntryCount == 0)
            {
                return NullImageSplashPresenter.Shared;
            }

            if (statusPresenter is
                    IImageSplashPresenter
                        splashPresenter)
            {
                return splashPresenter;
            }

            Debug.LogWarning(
                $"[{SplashPresenterUnavailableDiagnosticCode}] " +
                "A splash sequence is configured, but the active status presenter does not implement IImageSplashPresenter. " +
                "First Light will preserve authored splash timing through the headless presenter.",
                this);

            return NullImageSplashPresenter.Shared;
        }

        private async Awaitable
            CompleteInitialDestinationHandoffAsync(
                StartupSequenceRunResult runResult,
                LaunchDestination destination)
        {
            if (!PublishTransitionPending(
                    runResult))
            {
                return;
            }

            if (IsCancellationRequested)
            {
                PublishInterrupted(
                    runResult,
                    StartupStepResult.Cancelled(
                        LifecycleDiagnosticCode,
                        NormalizeCancellationReason(
                            cancellationReason),
                        string.Empty));

                return;
            }

            IInitialDestinationLoader loader =
                initialDestinationLoader;

            if (loader == null)
            {
                PublishDestinationLoadFailure(
                    runResult,
                    "The initial destination loader became unavailable.",
                    string.Empty);

                return;
            }

            InitialDestinationProgressRelay progress =
                new InitialDestinationProgressRelay(
                    OnInitialDestinationProgressChanged);

            InitialDestinationLoadResult loadResult =
                null;

            PrepareRootForDestinationLoad(
                loader);

            try
            {
                loadResult =
                    await loader.LoadAsync(
                        destination,
                        progress,
                        launchCancellationSource
                            .Token);
            }
            catch (
                OperationCanceledException
                    exception)
            {
                if (IsCancellationRequested)
                {
                    PublishInterrupted(
                        runResult,
                        StartupStepResult.Cancelled(
                            LifecycleDiagnosticCode,
                            NormalizeCancellationReason(
                                cancellationReason),
                            CreateExceptionDetails(
                                exception)));

                    return;
                }

                PublishDestinationLoadFailure(
                    runResult,
                    "Initial destination loading was cancelled without an active root cancellation request.",
                    CreateExceptionDetails(
                        exception));

                return;
            }
            catch (Exception exception)
            {
                PublishDestinationLoadFailure(
                    runResult,
                    "Initial destination loading failed unexpectedly.",
                    CreateExceptionDetails(
                        exception));

                return;
            }
            finally
            {
                progress.Close();

                RestoreRootAfterDestinationLoad();
            }

            if (!CanPublishRuntimeProgress)
            {
                return;
            }

            if (loadResult == null)
            {
                PublishDestinationLoadFailure(
                    runResult,
                    "The initial destination loader returned no terminal result.",
                    string.Empty);

                return;
            }

            if (IsCancellationRequested ||
                loadResult.IsCancelled)
            {
                string code =
                    string.IsNullOrWhiteSpace(
                        loadResult.Code)
                        ? LifecycleDiagnosticCode
                        : loadResult.Code;

                string message =
                    string.IsNullOrWhiteSpace(
                        cancellationReason)
                        ? loadResult.Message
                        : cancellationReason;

                PublishInterrupted(
                    runResult,
                    StartupStepResult.Cancelled(
                        code,
                        NormalizeCancellationReason(
                            message),
                        loadResult.Details));

                return;
            }

            if (loadResult.IsFailed)
            {
                PublishDestinationLoadFailure(
                    runResult,
                    loadResult.Message,
                    loadResult.Details);

                return;
            }

            if (!loadResult.IsSucceeded ||
                !string.Equals(
                    loadResult.DestinationId,
                    destination.DestinationId,
                    StringComparison.Ordinal))
            {
                PublishDestinationLoadFailure(
                    runResult,
                    "The initial destination loader returned an invalid success result.",
                    $"ExpectedDestinationId: {destination.DestinationId}\n" +
                    $"ObservedDestinationId: {loadResult.DestinationId}");

                return;
            }

            PublishCompletedHandoff(
                runResult,
                destination,
                loadResult);
        }

        private void OnInitialDestinationProgressChanged(
            float progress01)
        {
            if (!CanPublishRuntimeProgress ||
                session.State !=
                    LaunchStatus.Transitioning)
            {
                return;
            }

            PublishProgress(
                new LaunchProgressSnapshot(
                    launchMode,
                    LaunchStatus.Transitioning,
                    string.Empty,
                    -1,
                    GetAuthoredEntryCount(),
                    progress01,
                    false,
                    "Loading the initial destination.",
                    GetElapsedSeconds(),
                    GetFinalResult(
                        lastSequenceRunResult)));
        }

        private void PublishDestinationLoadFailure(
            StartupSequenceRunResult runResult,
            string message,
            string details)
        {
            StartupStepResult result =
                StartupStepResult.BlockingFailure(
                    DestinationLoadDiagnosticCode,
                    string.IsNullOrWhiteSpace(
                        message)
                        ? "The initial destination failed to load."
                        : message.Trim(),
                    details);

            PublishFailureAndReport(
                runResult,
                string.Empty,
                -1,
                runResult == null
                    ? GetAuthoredEntryCount()
                    : runResult.AuthoredEntryCount,
                1f,
                false,
                result);
        }

        private void PublishCompletedHandoff(
            StartupSequenceRunResult runResult,
            LaunchDestination destination,
            InitialDestinationLoadResult loadResult)
        {
            string message =
                string.IsNullOrWhiteSpace(
                    loadResult.Message)
                    ? $"Initial destination '{destination.DisplayName}' activated."
                    : loadResult.Message;

            StartupStepResult finalResult =
                StartupStepResult.Success(
                    message,
                    loadResult.Details);

            if (!PublishTerminalSnapshot(
                    LaunchStatus.Completed,
                    string.Empty,
                    -1,
                    runResult.AuthoredEntryCount,
                    1f,
                    false,
                    finalResult.Message,
                    finalResult))
            {
                return;
            }

            FinalizeTerminalReport(
                LaunchStatus.Completed,
                runResult,
                finalResult,
                destination);
        }

        private void PrepareRootForDestinationLoad(
            IInitialDestinationLoader loader)
        {
            if (!ReferenceEquals(
                    loader,
                    UnityInitialDestinationLoader
                        .Shared) ||
                isDestroying)
            {
                return;
            }

            GameObject handoffRoot =
                transform.root.gameObject;

            preservedHandoffObject =
                handoffRoot;

            DontDestroyOnLoad(
                handoffRoot);

            isTemporarilyPreservedForHandoff =
                true;
        }

        private void RestoreRootAfterDestinationLoad()
        {
            if (!isTemporarilyPreservedForHandoff)
            {
                return;
            }

            isTemporarilyPreservedForHandoff =
                false;

            GameObject handoffObject =
                preservedHandoffObject;

            preservedHandoffObject = null;

            if (isDestroying ||
                handoffObject == null)
            {
                return;
            }

            Scene activeScene =
                SceneManager.GetActiveScene();

            if (activeScene.IsValid() &&
                activeScene.isLoaded &&
                handoffObject.scene !=
                    activeScene)
            {
                SceneManager.MoveGameObjectToScene(
                    handoffObject,
                    activeScene);
            }
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
            StartupStepResult finalResult,
            LaunchDestination destination = null)
        {
            if (!CanPublishRuntimeProgress ||
                launchReportBuilder == null ||
                lastReport != null)
            {
                return;
            }

            LaunchReport report =
                finalStatus ==
                    LaunchStatus.Completed
                    ? launchReportBuilder
                        .FinalizeCompletedReport(
                            destination,
                            finalResult,
                            GetMonotonicNow())
                    : launchReportBuilder
                        .FinalizeReport(
                            finalStatus,
                            runResult,
                            finalResult,
                            GetMonotonicNow());

            lastReport = report;

            PresentTerminalReport(
                report);

            if (!CanPublishRuntimeProgress)
            {
                return;
            }

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

            if (finalStatus ==
                LaunchStatus.Interrupted)
            {
                LaunchNotificationDispatcher.Dispatch(
                    LaunchInterrupted,
                    report,
                    nameof(LaunchInterrupted),
                    this);

                return;
            }

            LaunchNotificationDispatcher.Dispatch(
                LaunchCompleted,
                report,
                nameof(LaunchCompleted),
                this);
        }

        private void ResolveStatusPresenter()
        {
            if (statusPresenterWasInjected)
            {
                return;
            }

            statusPresenter =
                LaunchStatusPresenterDispatcher
                    .Resolve(
                        statusPresenterComponent,
                        this);
        }

        private void BindStatusPresenter()
        {
            if (isStatusPresenterBound)
            {
                return;
            }

            ResolveStatusPresenter();

            ILaunchStatusPresenter presenter =
                statusPresenter ??
                NullLaunchStatusPresenter.Shared;

            ConfigureSplashPresentation(
                presenter);

            LaunchStatusPresenterDispatcher
                .TryBind(
                    presenter,
                    session == null
                        ? LaunchProgressSnapshot.Empty
                        : session.Progress,
                    this);

            isStatusPresenterBound = true;
        }

        private void ConfigureSplashPresentation(
            ILaunchStatusPresenter presenter)
        {
            if (!(presenter is
                    ISplashPresentationSettingsReceiver
                        receiver))
            {
                return;
            }

            SplashPresentationSettings settings =
                SplashPresentationSettings
                    .LegacyDefaults;

            SplashSequence configuredSequence =
                configuration == null
                    ? null
                    : configuration
                        .SplashSequence;

            if (configuredSequence != null &&
                configuredSequence
                    .PresentationSettings
                    .HasValidDefinition)
            {
                settings =
                    configuredSequence
                        .PresentationSettings;
            }

            try
            {
                receiver
                    .ConfigureSplashPresentation(
                        settings);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[{PresenterFailureDiagnosticCode}] " +
                    "Splash presentation configuration failed. " +
                    $"{exception.GetType().Name}: " +
                    $"{exception.Message}",
                    this);
            }
        }

        private void PresentStatusSnapshot(
            LaunchProgressSnapshot snapshot)
        {
            if (!isStatusPresenterBound)
            {
                return;
            }

            LaunchStatusPresenterDispatcher
                .TryPresent(
                    statusPresenter ??
                    NullLaunchStatusPresenter.Shared,
                    snapshot,
                    this);
        }

        private void PresentTerminalReport(
            LaunchReport report)
        {
            if (!isStatusPresenterBound ||
                report == null)
            {
                return;
            }

            LaunchStatusPresenterDispatcher
                .TryPresentTerminal(
                    statusPresenter ??
                    NullLaunchStatusPresenter.Shared,
                    report,
                    this);
        }

        private void UnbindStatusPresenter()
        {
            if (!isStatusPresenterBound)
            {
                return;
            }

            isStatusPresenterBound = false;

            LaunchStatusPresenterDispatcher
                .TryUnbind(
                    statusPresenter ??
                    NullLaunchStatusPresenter.Shared,
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

        private double GetMonotonicNow()
        {
            ILaunchClock clock =
                launchClock ??
                UnityLaunchClock.Shared;

            double now =
                clock.NowSeconds;

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
