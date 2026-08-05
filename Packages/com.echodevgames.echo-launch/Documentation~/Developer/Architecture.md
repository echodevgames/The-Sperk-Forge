# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Create-only repeat-safe Editor setup Apply implemented; explicit repair and migration pending
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
  - `FL-M2-03`
  - `FL-M2-04`
  - `FL-M2-05`
  - `FL-M2-06`
  - `FL-M2-07`
  - `FL-M2-08`
  - `FL-M3-01`
  - `FL-M3-02`
  - `FL-M3-03`
  - `FL-M3-04`
  - `FL-M3-05`
  - `FL-M3-06`
  - `FL-M3-07`
  - `FL-M3-08`
  - `FL-M4-01`
  - `FL-M4-02`
  - `FL-M4-03`
  - `FL-M4-04`
  - `FL-M4-05`
  - `FL-M5-01`
  - `FL-M5-02`
- Unity baseline: `6000.3.8f1`

## Current Architecture

First Light currently establishes:

1. Single launch authority
2. Neutral launch-state vocabulary
3. One live session owned by the authoritative root
4. Read-only state and progress exposure
5. Central lifecycle transition validation
6. Isolated lifecycle notifications
7. Project-owned launch configuration identity
8. Authority-filtered root configuration binding
9. Immutable startup-step definitions
10. Ordered startup-sequence entry modeling
11. Passive configuration-to-sequence binding
12. Authored step policy
13. Immutable step progress and runtime context
14. Fresh single-use executor contract
15. Runtime-only step-attempt state
16. Immutable completed traversal summaries
17. Ordered enabled-entry execution
18. Immediate executor result capture
19. Authored failure-policy application
20. Stable factory and executor exception conversion
21. Blocking traversal stops
22. Attempted, disabled, and unvisited entry accounting
23. Injectable monotonic launch clock
24. Immutable per-attempt timing
25. Deterministic completion-versus-timeout monitoring
26. Stable timeout result conversion
27. Cooperative timeout cancellation
28. Timed-out executor settlement before traversal
29. Late progress and late result containment
30. Production-shaped multi-frame Unity `Awaitable` proof
31. Structured caller-cancellation outcome after executor settlement
32. Stable caller-cancellation diagnostic `ELAUNCH-STEP-005`
33. Immutable run-level `WasCancelled` summary
34. Same-tick caller-cancellation race containment
35. Complete side-effect-free startup-sequence preflight
36. Configuration, sequence, entry, and step identity/schema validation
37. Duplicate entry-ID and step-ID detection before executor creation
38. Runner-local atomic active-run gate
39. Stable concurrent re-entry diagnostic `ELAUNCH-RUN-001`
40. Internal runner-to-root observation seam
41. Structured preflight diagnostic exception
42. Explicit root-owned startup execution
43. Root lifecycle advancement through validation and execution
44. Root cancellation command and stable lifecycle diagnostics
45. Destruction-driven cancellation and late-publication suppression
46. Success boundary at `Transitioning`
47. Exact legacy runner exception compatibility
48. Immutable public per-step report values
49. Immutable public failed/interrupted launch reports
50. Internal single-use report builder
51. Authority-filtered `LastReport`
52. Exactly-once `LaunchFailed` and `LaunchInterrupted`
53. Terminal state and report acceptance before event dispatch
54. Defensive report copying and post-runtime readability
55. Transition-pending success without false report completion
56. Project-owned `LaunchDestination` schema 1
57. Configuration schema 4 with startup sequence, initial destination, optional splash sequence, and reduced-motion binding
58. Immutable initial destination load result
59. Injectable package-local initial destination loader
60. Destination validation before startup-step side effects
61. Standalone Unity asynchronous destination loader
62. Destination progress while `Transitioning`
63. Successful `Transitioning -> Completed` handoff
64. Completed report schema 2 with destination metadata
65. Exactly-once `LaunchCompleted`
66. Startup warning preservation across successful destination activation
67. Unity `Start` automatic launch through the existing one-run gate
68. Serialized automatic-start opt-out
69. Public neutral `ILaunchStatusPresenter`
70. Logging-free headless presenter fallback
71. Presenter resolution without a Runtime uGUI dependency
72. Accepted snapshot presentation before public progress events
73. Finalized report presentation before public terminal events
74. Stable `ELAUNCH-VIEW-001` and `ELAUNCH-VIEW-002`
75. Presenter callback containment and destruction unbinding
76. Duplicate-root automatic-start and presenter silence
77. Separate replaceable uGUI presentation assembly
78. Public plain `EchoLaunchStatusView`
79. Text-complete lifecycle state presentation
80. Determinate slider progress and percentage copy
81. Distinct indeterminate progress surface
82. Active-step and elapsed-time presentation
83. Finalized terminal diagnostic and destination presentation
84. Configurable bind/unbind visibility and clearing
85. Missing-reference-safe visual degradation
86. Separate presentation test assembly and bounded friend access
87. Runtime remaining uGUI-free
88. Project-owned `SplashSequence` schema 1
89. Immutable image-only `SplashEntry` definitions
90. Stable splash skip-policy and playback-phase vocabulary
91. Deterministic `ILaunchClock`-driven splash playback
92. Ordered multi-entry traversal
93. Minimum-display timing expansion
94. Latched skip requests that cannot bypass minimum display
95. Reduced-motion fade removal
96. Cancellation, re-entry, and invalid-clock containment
97. Neutral `IImageSplashPresenter`
98. Logging-free headless splash presenter fallback
99. Immutable splash frames and playback results
100. uGUI image, label, alpha, and sequence-position projection
101. Public project-routed splash skip request
102. Missing-reference-safe splash presentation
103. Configuration schema 4 splash binding
104. Side-effect-free optional splash preflight
105. Stable `ELAUNCH-SPLASH-001`, `ELAUNCH-SPLASH-002`, and `ELAUNCH-SPLASH-003`
106. Sequential root order of splash, startup steps, and destination
107. Shared launch clock across splash, startup execution, and report timing
108. Root cancellation during splash using existing interrupted settlement
109. Headless splash timing when visuals are unavailable
110. Successful splash result retention
111. Duplicate-root and automatic-start splash containment
112. Direct-scene mode using the same splash contract
113. Configuration and splash asset immutability
114. Report schema 2 preserved
115. Stable package-owned status-view prefab identity
116. Stable package-owned root-composition prefab identity
117. Screen Space Overlay Canvas template
118. Complete serialized status and splash reference wiring
119. Neutral non-branded presentation defaults
120. Project-owned branding and input boundaries
121. Non-interactive graphics and progress surface
122. Root prefab with nested status-view prefab
123. Intentionally unassigned project configuration
124. Canonical Boot and automatic-start root defaults
125. No hidden prefab discovery or runtime instantiation
126. Editor-only serialized asset proof
127. Prefab dependency and missing-script containment
128. Read-only project snapshot collection
129. Immutable setup request and project evidence values
130. Immutable setup operation, diagnostic, and plan values
131. Deterministic pure setup planning
132. Stable project-owned default path set
133. Path traversal and external-path rejection
134. Compatible project asset reuse planning
135. Incompatible target conflict planning
136. Unsupported schema migration blocking
137. Ambiguous candidate manual-decision planning
138. Package-template prerequisite planning
139. Append-safe Build Settings planning
140. Explicit-approval place-first Build Settings planning
141. Preview-only Setup window
142. Deterministic plain-text setup report
143. Hard no-write Editor boundary
144. Editor-only focused setup proof

First Light now validates, executes, times, evaluates, projects, plays an optional configured splash, runs startup steps, loads one initial destination, finalizes one immutable terminal report, and starts automatically from Unity `Start`. The package also ships neutral presentation prefabs and a preview-only Editor setup-planning layer that can inspect and explain project adoption without modifying project content. Apply/repair, migration, direct-scene initialization, and standalone scene proof remain separate boundaries.

## Implemented Package Files

    Presentation.UGUI/
    ├── EchoDevGames.EchoLaunch.Presentation.UGUI.asmdef
    ├── EchoLaunchStatusView.cs
    ├── Prefabs/
    │   ├── EchoLaunchRoot.prefab
    │   └── EchoLaunchStatusView.prefab
    └── Properties/
        └── AssemblyInfo.cs

    Editor/
    ├── Properties/
    │   └── AssemblyInfo.cs
    └── Setup/
        ├── EchoLaunchProjectSnapshot.cs
        ├── EchoLaunchProjectSnapshotCollector.cs
        ├── EchoLaunchSetupDiagnosticCodes.cs
        ├── EchoLaunchSetupEnums.cs
        ├── EchoLaunchSetupPaths.cs
        ├── EchoLaunchSetupPlanModels.cs
        ├── EchoLaunchSetupPlanTextFormatter.cs
        ├── EchoLaunchSetupPlanner.cs
        ├── EchoLaunchSetupRequest.cs
        └── EchoLaunchSetupWindow.cs

    Runtime/
    ├── Configuration/
    │   └── EchoLaunchConfiguration.cs
    ├── Core/
    │   ├── LaunchAuthorityClaim.cs
    │   └── EchoLaunchRoot.cs
    ├── Events/
    │   ├── LaunchNotificationDispatcher.cs
    │   ├── LaunchProgressChangedEvent.cs
    │   └── LaunchStateChangedEvent.cs
    ├── Execution/
    │   ├── ILaunchClock.cs
    │   ├── IStartupSequenceObserver.cs
    │   ├── StartupSequencePreflight.cs
    │   ├── StartupSequencePreflightException.cs
    │   ├── StartupSequenceRunResult.cs
    │   ├── StartupSequenceRunner.cs
    │   ├── StartupStepAwaitOutcome.cs
    │   ├── StartupStepExceptionConverter.cs
    │   ├── StartupStepExceptionPhase.cs
    │   ├── StartupStepExecution.cs
    │   ├── StartupStepPolicyDecision.cs
    │   ├── StartupStepPolicyEvaluator.cs
    │   ├── StartupStepProgressGate.cs
    │   ├── StartupStepProgressRelay.cs
    │   ├── StartupStepTimeoutMonitor.cs
    │   ├── StartupStepTiming.cs
    │   └── UnityLaunchClock.cs
    ├── Presentation/
    │   ├── IImageSplashPresenter.cs
    │   ├── ILaunchStatusPresenter.cs
    │   ├── LaunchStatusPresenterDispatcher.cs
    │   ├── NullImageSplashPresenter.cs
    │   └── NullLaunchStatusPresenter.cs
    ├── Properties/
    │   └── AssemblyInfo.cs
    ├── Reports/
    │   ├── LaunchReport.cs
    │   ├── LaunchReportBuilder.cs
    │   └── LaunchStepReport.cs
    ├── SceneLoading/
    │   ├── IInitialDestinationLoader.cs
    │   ├── InitialDestinationLoadResult.cs
    │   ├── InitialDestinationLoadStatus.cs
    │   ├── InitialDestinationProgressRelay.cs
    │   ├── LaunchDestination.cs
    │   └── UnityInitialDestinationLoader.cs
    ├── Splash/
    │   ├── SplashEntry.cs
    │   ├── SplashPlaybackPhase.cs
    │   ├── SplashPlaybackResult.cs
    │   ├── SplashPresentationFrame.cs
    │   ├── SplashSequence.cs
    │   ├── SplashSequencePlayer.cs
    │   ├── SplashSequencePreflight.cs
    │   └── SplashSkipPolicy.cs
    ├── State/
    │   ├── LaunchMode.cs
    │   ├── LaunchStatus.cs
    │   ├── LaunchProgressSnapshot.cs
    │   ├── LaunchSession.cs
    │   └── LaunchStateTransitionRules.cs
    └── Steps/
        ├── IStartupStepExecutor.cs
        ├── IStartupStepProgressReporter.cs
        ├── StartupSequence.cs
        ├── StartupSequenceEntry.cs
        ├── StartupStepContext.cs
        ├── StartupStepDefinition.cs
        ├── StartupStepFailureAction.cs
        ├── StartupStepPolicy.cs
        ├── StartupStepProgress.cs
        ├── StartupStepResult.cs
        └── StartupStepStatus.cs

    Tests/Editor/
    ├── EchoDevGames.EchoLaunch.Tests.Editor.asmdef
    └── Setup/
        ├── EchoLaunchProjectSnapshotCollectorTests.cs
        ├── EchoLaunchSetupPathUtilityTests.cs
        ├── EchoLaunchSetupPlanTextFormatterTests.cs
        ├── EchoLaunchSetupPlannerTests.cs
        ├── EchoLaunchSetupTestFactory.cs
        └── EchoLaunchSetupWindowTests.cs

    Tests/Presentation.UGUI/
    ├── EchoDevGames.EchoLaunch.Tests.Presentation.UGUI.asmdef
    ├── EditMode/
    │   ├── EchoDevGames.EchoLaunch.Tests.Presentation.UGUI.EditMode.asmdef
    │   └── EchoLaunchPresentationPrefabAssetTests.cs
    └── PlayMode/
        ├── EchoLaunchSplashPresentationTests.cs
        └── EchoLaunchStatusViewTests.cs

    Tests/Runtime/PlayMode/
    ├── EchoLaunchAutomaticStartAndPresenterTests.cs
    ├── EchoLaunchRootAuthorityTests.cs
    ├── EchoLaunchRootSplashLifecycleTests.cs
    ├── EchoLaunchRootStartupLifecycleTests.cs
    ├── LaunchClockTimingAndGateTests.cs
    ├── LaunchConfigurationBindingTests.cs
    ├── LaunchLifecycleTransitionTests.cs
    ├── LaunchDestinationAndCompletedHandoffTests.cs
    ├── LaunchNotificationTests.cs
    ├── LaunchReportAndTerminalEventTests.cs
    ├── LaunchSessionProgressTests.cs
    ├── LaunchStateVocabularyTests.cs
    ├── SplashSequencePlayerTests.cs
    ├── StartupSequenceDefinitionTests.cs
    ├── StartupSequenceRunnerImmediateTests.cs
    ├── StartupSequenceRunnerPolicyAndExceptionTests.cs
    ├── StartupSequenceRunnerTimeoutTests.cs
    ├── StartupSequenceRunnerMultiFrameAsyncTests.cs
    ├── StartupSequenceRunnerPreflightAndReentryTests.cs
    ├── StartupStepExecutionTests.cs
    ├── StartupStepPolicyAndExecutorContractTests.cs
    └── StartupStepPolicyApplicationTests.cs

## Authored Definition Boundary

Shared ScriptableObject assets contain authored definition data only.

They may contain:

- Stable domain identity
- Schema version
- Display metadata
- Ordered references
- Enabled or disabled intent
- Failure policy
- Timeout metadata
- Cancellation capability metadata

They must not contain:

- Current execution status
- Elapsed time
- Remaining timeout
- Retry count
- Cancellation state
- Current progress
- Exceptions
- Results
- Runner ownership
- Scene-transition state

Active data must live in fresh runtime-owned objects introduced by later checkpoints.

## Startup Step Failure Vocabulary

`StartupStepFailureAction` contains exactly:

    BlockLaunch = 0
    ContinueWithWarning = 1

`BlockLaunch` is numeric zero so an uninitialized serialized enum fails closed.

Automatic retry and interactive retry are intentionally absent.

## Startup Step Policy

`StartupStepPolicy` is a serializable authored value type.

It exposes:

    IsRequired
    IsOptional
    FailureAction
    TimeoutSeconds
    HasTimeout
    SupportsCancellation

Safe presets:

    RequiredBlocking
    OptionalWarning

### Required Blocking

- Required
- Block launch on failure
- No timeout configured
- Cooperative cancellation supported

### Optional Warning

- Optional
- Continue with warning on failure
- No timeout configured
- Cooperative cancellation supported

### Timeout Metadata

Timeout is stored in seconds.

- `0` means no timeout configured.
- A finite value greater than `0` enables timeout metadata.
- Negative, NaN, and infinite values are invalid.
- Invalid values remain unchanged for diagnostics and future explicit repair.
- FL-M3-03 measures positive timeout metadata through an injected monotonic unscaled clock.

### Policy Validation

Internal validation detects:

- Undefined requirement mode
- Undefined failure action
- Negative or non-finite timeout
- Undefined cancellation mode

Runtime code does not clamp, repair, or rewrite authored policy.

## Safe Unity Serialized Defaults

Unity can create a new embedded list element from zeroed serialized data without applying C# field initializers.

To make that path safe by construction:

- `EntryActivation.Enabled` is zero.
- `RequirementMode.Required` is zero.
- `StartupStepFailureAction.BlockLaunch` is zero.
- Timeout zero means disabled.
- `CancellationMode.Supported` is zero.

Therefore a zeroed new entry becomes:

    Activation: Enabled
    Requirement: Required
    Failure Action: Block Launch
    Timeout Seconds: 0
    Cancellation: Supported

No `OnValidate`, serialization callback, migration, or automatic repair path was added.

## Startup Step Progress

`StartupStepProgress` is an immutable runtime value.

Factories:

    Determinate(progress01, message)
    Indeterminate(message)

Determinate progress:

- Accepts finite values from `0` through `1`, inclusive.
- Rejects negative, greater-than-one, NaN, and infinite values.

Indeterminate progress:

- Does not invent a percentage.
- Exposes `Progress01` as zero while `IsIndeterminate` is true.

Messages are trimmed. Null, empty, and whitespace-only messages normalize to an empty string.

## Progress Reporting Seam

`IStartupStepProgressReporter` exposes:

    void Report(StartupStepProgress progress)

The interface is package-owned and intentionally narrow.

An executor does not receive the root, presenter, report builder, or mutable sequence through this seam.

## Startup Step Context

`StartupStepContext` is a validated immutable runtime object.

It carries:

- Launch mode
- Configuration ID
- Sequence ID
- Entry ID
- Step ID
- Zero-based step index
- Step count
- Linked per-attempt `CancellationToken`
- `IStartupStepProgressReporter`

Constructor validation rejects:

- Blank identities
- Step count less than one
- Step index outside the current count
- Null progress reporter

The runner links the caller token with one attempt-local timeout token.

The context owns no launch authority and exposes no setters.

## Executor Contract

`IStartupStepExecutor` exposes:

    Awaitable<StartupStepResult> ExecuteAsync(
        StartupStepContext context)

This contract uses Unity `Awaitable<T>`.

Rules:

- One executor instance represents one execution attempt.
- A definition creates a fresh executor for every attempt.
- The definition does not store the executor.
- The executor owns its own active state.
- Cancellation is cooperative through the linked context token.
- Progress is reported through the package-owned reporter.
- A cancellable executor must settle promptly after cancellation.
- A non-cancellable executor may finish naturally.
- The runner never abandons an active executor.
- A late result cannot replace an already-observed timeout.
- A late progress report is ignored after the progress gate closes.

Exception conversion, timeout monitoring, and failure-policy application remain runner-owned.

## Definition Factory

`StartupStepDefinition` now requires:

    public abstract IStartupStepExecutor CreateExecutor();

Repeated factory calls must return distinct executor instances.

A null return or factory exception will become a preflight or runtime blocker in a later checkpoint. FL-M2-08 only establishes and tests the contract.

## Sequence Entry Policy

`StartupSequenceEntry` now contains:

    entryId
    activation
    stepDefinition
    policy

It exposes a copy of `StartupStepPolicy`.

Because the policy is a struct, callers cannot mutate the serialized entry through the returned value.

## Sequence Schema

`StartupSequence.CurrentSchemaVersion` is now `2`.

Schema `2` adds policy data to each embedded sequence entry.

Runtime migration remains unimplemented.

## Runtime Step Execution

`StartupStepExecution` is an internal runtime-owned object representing one enabled entry attempt.

It copies:

- Entry ID
- Step ID
- Step display label
- Authored index
- Complete authored entry count
- Authored policy

It owns:

- An optional fresh executor
- Current `StartupStepStatus`
- Latest accepted `StartupStepProgress`
- One terminal `StartupStepResult`
- One immutable `StartupStepTiming`

Normal execution path:

    NotStarted
        -> Running
            -> terminal result status

Factory or pre-execution contract failure path:

    NotStarted
        -> BlockingFailure

Timing is assigned exactly once with terminal completion.

Retained callers that do not measure executor time use `StartupStepTiming.NotMeasured`.

Guards:

- An executor may be attached exactly once before begin.
- Begin requires an attached executor.
- Progress is legal only while running.
- Normal completion is legal only while running.
- Pre-start completion accepts one blocking result only.
- Result and timing completion are legal exactly once.
- Direct progress after completion is rejected.
- Timeout-driven late progress is filtered before reaching the execution object.

No ScriptableObject is mutated.

## Policy Decision

`StartupStepPolicyDecision` stores:

- Original terminal result
- Effective terminal result
- Whether traversal continues

Derived reads expose:

- Whether traversal stops
- Whether policy replaced the immutable result instance

Preserved results retain their original instance.

Converted results are new immutable result instances.

## Policy Evaluation

`StartupStepPolicyEvaluator` applies the explicit authored `FailureAction`.

Preserve and continue:

- `Succeeded`
- `Warning`
- `Skipped`

Preserve and stop:

- `Cancelled`

Failure-like statuses:

- `RecoverableFailure`
- `BlockingFailure`
- `TimedOut`

`ContinueWithWarning`:

- Converts the effective status to `Warning`.
- Preserves code, message, and details.
- Continues traversal.

`BlockLaunch`:

- Converts the effective status to `BlockingFailure` when necessary.
- Preserves code, message, and details.
- Stops traversal.

`IsRequired` and `IsOptional` remain authoring intent. They do not secretly override the explicit failure action.

Invalid policy is converted by the runner into a blocking `ELAUNCH-STEP-004` contract result without rewriting the authored asset.

## Exception Conversion

`StartupStepExceptionConverter` uses stable diagnostic code:

    ELAUNCH-STEP-004

Factory exception:

- Converts to `BlockingFailure`.
- Stops traversal.
- Does not invoke an executor.
- Does not follow continue-with-warning policy because no valid executor exists.

Null executor:

- Converts to a blocking contract result.
- Stops traversal.

Executor exception:

- Converts to a recoverable source result.
- Then follows authored failure policy.

Null executor result:

- Converts to a blocking contract result.
- Stops traversal.

Sanitized details contain:

- Exception type
- Trimmed exception message

They exclude:

- Stack trace
- Recursive inner-exception graph
- Unity object dumps

`OperationCanceledException` is not converted by the generic path. Cancellation orchestration remains a later checkpoint.

## Monotonic Launch Clock

`ILaunchClock` is the public runtime and testing seam.

It exposes:

    double NowSeconds

    Awaitable NextTickAsync(
        CancellationToken cancellationToken)

Clock requirements:

- Finite values
- Nonnegative values
- Monotonic nondecreasing values
- Seconds as the unit
- Unscaled time
- Non-blocking ticks

`UnityLaunchClock` is the internal shared default.

It uses:

    Time.realtimeSinceStartupAsDouble
    Awaitable.NextFrameAsync(cancellationToken)

The runner also accepts an injected `ILaunchClock` for deterministic tests.

## Attempt Timing

`StartupStepTiming` is an immutable runtime value.

It records:

- Start time
- Settlement time
- Derived elapsed seconds
- Configured timeout seconds
- Whether timeout was configured
- Whether timeout was reached
- Whether timeout cancellation was requested

Validation rejects:

- NaN
- Infinity
- Negative time
- Settlement before start
- Timeout-reached state without positive timeout
- Cancellation-request state without timeout

Timing remains outside ScriptableObject definitions.

## Late Progress Gate

`StartupStepProgressGate` wraps one progress reporter.

While open, progress is forwarded.

After close:

- Progress is ignored.
- Repeated close calls are safe.
- The gate never reopens.

The timeout monitor closes the gate when timeout, caller cancellation, monitor failure, or executor settlement is observed.

## Await Outcome

`StartupStepAwaitOutcome` preserves the settled executor observation before the runner applies result policy.

It distinguishes:

- Normal returned result
- Normal returned null result
- Thrown exception
- Timed-out state
- Timeout cancellation-request state
- Caller-cancellation observation
- Immutable timing snapshot

A timed-out outcome may contain a later success, failure, or cancellation exception, but timeout remains the runner's source result.

A caller-cancelled outcome is returned only after the active executor settles. The outcome preserves the settled executor observation while recording that the caller cancellation boundary owns the effective runner result.

## Timeout Monitor

`StartupStepTimeoutMonitor` owns one attempt deadline.

Deterministic race rule:

1. Observe executor completion first.
2. Read and validate the current clock.
3. Recheck completion.
4. Compare the clock with the absolute deadline.
5. Latch the first observed timeout.

Therefore an executor already observable as complete at the deadline boundary wins.

Once timeout is latched:

- The progress gate closes.
- Timeout cancellation is requested only when the authored policy supports cancellation.
- The monitor continues until the executor settles.
- A late result or exception cannot replace timeout.

The monitor also waits for settlement before returning caller cancellation or allowing a clock-contract failure to escape.

Caller cancellation is recognized when it was latched during monitoring or when the executor settles with `OperationCanceledException` while the caller token is already requested. This closes the same-tick cancellation race without treating unrelated executor cancellation as caller cancellation.

Backward, non-finite, or negative clock behavior becomes a blocking `ELAUNCH-STEP-004` timing-contract failure after the active executor settles.

## Timeout Result

Timed-out attempts use stable code:

    ELAUNCH-STEP-003

Message:

    The startup step exceeded its configured timeout.

Details contain invariant values for:

- `TimeoutSeconds`
- `ElapsedSeconds`
- `CancellationRequested`

The source result uses `StartupStepStatus.TimedOut`.

Existing failure policy then applies:

- `ContinueWithWarning` converts the timeout to `Warning` and continues after settlement.
- `BlockLaunch` converts the timeout to `BlockingFailure` and stops before a later factory.

## Cooperative Cancellation

Each enabled attempt owns:

- One timeout `CancellationTokenSource`
- One token source linked with the caller token

The linked token is delivered through `StartupStepContext`.

At timeout:

- Supporting steps receive one timeout cancellation request.
- Unsupported steps receive no timeout cancellation request.
- The runner still waits for natural settlement.

Caller cancellation remains distinct from timeout cancellation.

FL-M3-04 converts caller cancellation into a structured terminal result only after the active executor settles.

Stable code:

    ELAUNCH-STEP-005

Message:

    Startup-sequence execution was cancelled by the caller.

Caller cancellation stops traversal regardless of authored failure policy. `ContinueWithWarning` cannot downgrade it, and no later executor factory is called.

## Completed Sequence Run Result

`StartupSequenceRunResult` is an internal immutable summary.

It exposes:

- Authored entry count
- Disabled entry count
- Attempted execution count
- Unvisited entry count
- Whether traversal stopped early
- Stopping authored entry index
- Indexed completed execution access
- Warning presence
- Failure presence
- Blocking-failure presence
- Cancellation presence through `WasCancelled`

Accounting invariant:

    attempted + disabled + unvisited = authored

A complete traversal has:

- Unvisited count `0`
- `WasStoppedEarly == false`
- Stopping index `-1`

After a stop, every later authored entry is unvisited because the runner never inspects it.

The backing execution array remains private.

## Policy-Aware Timed Sequence Runner

`StartupSequenceRunner` supports:

    StartupSequenceRunner()

    StartupSequenceRunner(
        ILaunchClock clock)

The default constructor uses `UnityLaunchClock.Shared`.

The injected constructor rejects a null clock.

`RunAsync` then:

1. Validates the configuration and active launch mode.
2. Reads the configured startup sequence.
3. Iterates authored indices directly.
4. Skips and counts disabled entries.
5. Creates runtime execution metadata.
6. Calls `CreateExecutor()`.
7. Converts factory exceptions or null executors to blocking `ELAUNCH-STEP-004`.
8. Attaches the fresh executor.
9. Captures and validates the monotonic start time.
10. Creates timeout and linked cancellation sources.
11. Creates a progress gate.
12. Creates immutable `StartupStepContext` with the linked token.
13. Begins execution.
14. Invokes the executor exactly once.
15. Monitors completion, timeout, caller cancellation, and clock validity.
16. Waits for executor settlement.
17. Creates `ELAUNCH-STEP-003` when timeout won.
18. Creates structured `ELAUNCH-STEP-005` when caller cancellation won after settlement.
19. Converts non-timeout, non-caller-cancellation executor exceptions through `ELAUNCH-STEP-004`.
20. Converts null results to blocking contract failures.
21. Applies authored failure policy only when the outcome is not caller cancellation.
22. Completes the execution with effective result and timing.
23. Appends the execution in authored order.
24. Stops immediately after caller cancellation or follows the normal policy decision.
25. Disposes per-attempt cancellation sources.
26. Returns immutable traversal accounting, including `WasCancelled`.

No later executor factory is called while a timed-out executor remains active.

FL-M3-04 deliberately does not:

- Retry
- Add a root-level cancellation command
- Publish root events
- Update `LaunchSession`
- Build a public report
- Start automatically

## Startup Sequence Preflight

`StartupSequencePreflight` is an internal stateless read-only gate.

Before the runner creates any executor, it validates:

- A defined active launch mode
- Non-null launch configuration
- Valid configuration identity
- Supported configuration schema
- Assigned startup sequence
- Valid sequence identity
- Supported sequence schema
- Non-null entries
- Valid and unique entry identities
- Defined entry activation values
- Step-definition presence for enabled entries
- Valid referenced step identities
- Supported referenced step schemas
- Unique referenced step identities

The preflight uses stable diagnostics:

- `ELAUNCH-CFG-001`
- `ELAUNCH-SEQ-001`
- `ELAUNCH-STEP-001`
- `ELAUNCH-STEP-002`

The preflight does not:

- Call `CreateExecutor()`
- Mutate or repair authored assets
- Clamp invalid values
- Migrate schema data
- Build a public report
- Validate dependency graphs

Compatibility preserved in FL-M3-05:

- An empty sequence remains a valid empty traversal.
- A disabled entry may omit its step definition.
- Invalid enabled-entry policy remains a runner-created pre-start blocking result so the existing structured contract is preserved.

## Runner Re-entry Gate

Each `StartupSequenceRunner` instance owns one integer active-run state.

`RunAsync` acquires the gate through `Interlocked.CompareExchange`.

A concurrent call is rejected with:

    ELAUNCH-RUN-001

The rejection happens before preflight traversal and before any second executor factory can run.

The complete post-acquisition run body is wrapped in `try/finally`. The `finally` block releases the gate after:

- Successful traversal
- Structured caller cancellation
- Timeout or blocking traversal
- Preflight rejection
- Unexpected exception

The same runner instance may therefore be reused sequentially after the prior run settles, but it cannot own two overlapping traversals.

## Structured Preflight Diagnostic Boundary

`StartupSequencePreflightException` derives from `InvalidOperationException` and preserves:

- Stable diagnostic code
- Human-readable failure message
- Searchable formatted exception text

The observer-aware runner overload allows `EchoLaunchRoot` to map preflight rejection into an authoritative `Failed` snapshot without parsing exception text.

The retained three-argument runner overload catches the structured exception and rethrows an exact `InvalidOperationException`. This compatibility adapter preserves the historical direct-runner contract verified by retained NUnit exact-type assertions.

## Runner Observation Boundary

`IStartupSequenceObserver` is an internal neutral sink.

The runner may report:

- Sequence validation
- Step start
- Accepted step progress
- Step completion

`StartupStepProgressRelay` first records progress on the runtime execution, then forwards the accepted value to the observer. The runner remains independent of `EchoLaunchRoot`, scenes, presentation, and destination policy.

## Root-Owned Startup Lifecycle

The authoritative `EchoLaunchRoot` now exposes one internal explicit start boundary:

    StartLaunchAsync()

The method is not called by `Awake`, `Start`, or a scene callback.

One accepted run publishes:

    AuthorityClaimed
        -> Validating
            -> Running
                -> Transitioning

The root translates runner observations into existing immutable `LaunchProgressSnapshot` values.

Terminal mapping:

- Preflight or unexpected failure -> `Failed`
- Blocking or failed run result -> `Failed`
- Root or caller cancellation -> `Interrupted`
- Successful or warning-only run -> `Transitioning`

Success does not publish `Completed`. That state remains reserved for the later initial-destination handoff.

The root retains the latest settled `StartupSequenceRunResult` internally.

## Root Start Gate, Cancellation, and Destruction

One root-local atomic active-launch state prevents overlapping root starts.

Rejected starts use:

    ELAUNCH-LIFE-002

`CancelLaunch(reason)`:

- Requires the authoritative root
- Requires an active launch
- Accepts only the first cancellation request
- Normalizes blank reasons
- Requests cooperative cancellation
- Waits for executor settlement through the runner
- Publishes `Interrupted` exactly once

Lifecycle interruption uses:

    ELAUNCH-LIFE-001

When an active authoritative root is destroyed:

1. The root marks itself as destroying.
2. It requests cooperative cancellation.
3. The active run is allowed to settle.
4. Late state and progress publication is suppressed.
5. Event delegates are cleared.
6. Authority is released.

Duplicate roots cannot start or cancel the authoritative launch.

## Immutable Step Report Boundary

`LaunchStepReport` is a public immutable copy of one terminal `StartupStepExecution`.

It contains:

- Entry and step identity
- Display label
- Authored index and count
- Authored policy
- Final status and immutable result
- Final accepted progress
- Monotonic start, settlement, elapsed, and timeout timing
- Timeout and timeout-cancellation flags

It never exposes the internal execution object, executor, progress gate, or cancellation source.

## Immutable Launch Report Boundary

`LaunchReport` is a public immutable finalized launch artifact.

Current report schema:

    LaunchReport.CurrentSchemaVersion = 2

Producing package version:

    LaunchReport.CurrentPackageVersion = "0.1.0"

FL-M3-08 permits finalized statuses for:

- `Completed`
- `Failed`
- `Interrupted`

Completed reports require canonical destination identity and destination display metadata. Failed and interrupted reports may retain destination metadata when the handoff had already begun.

The report copies step reports into a private ordered array and exposes only count plus indexed reads. Constructor validation rejects nonterminal states, invalid timing, inconsistent authored traversal accounting, invalid cancellation combinations, and invalid destination metadata.

Reports are session diagnostics. They are not authored assets and are not EchoSave data.

## Project-Owned Destination Boundary

`LaunchDestination` is a project-owned immutable `ScriptableObject`.

It contains:

- Canonical stable destination identity
- Destination schema version `1`
- Trimmed nonblank display label
- Runtime-safe `Assets/.../*.unity` scene path

`EchoLaunchConfiguration` schema version `3` stores one initial destination reference. Historical schema 2 remains unsupported until later Editor migration.

Runtime never repairs, migrates, or rewrites configuration or destination assets.

## Initial Destination Loader Boundary

`IInitialDestinationLoader` is the public package-local seam for the one startup handoff.

It receives:

- The validated destination asset
- A normalized progress receiver
- The active launch cancellation token

It returns one immutable `InitialDestinationLoadResult`.

`UnityInitialDestinationLoader` is the standalone default. It validates build-loadability, starts `SceneManager.LoadSceneAsync` in single mode, reports normalized progress, waits for settlement, and confirms the requested destination scene is active before reporting success.

The loader does not own root lifecycle, reports, public events, normal mid-game scene travel, or presentation.

## Report Builder Boundary

`LaunchReportBuilder` is internal, root-owned, and single-use.

It:

- Captures validated configuration, sequence, and destination identity
- Captures completed step reports exactly once
- Preserves authored order
- Reconciles attempted, disabled, and unvisited accounting
- Records the settled sequence result
- Retains transition-pending successful data during loading
- Rejects duplicate capture and second finalization
- Finalizes completed, failed, or interrupted reports

The builder does not publish events and does not own lifecycle authority.

## Terminal Report Publication

`EchoLaunchRoot.LastReport` exposes the latest finalized report only from the current authoritative root.

Completed ordering:

    Destination activation confirmed
        -> Completed snapshot accepted
            -> immutable report finalized
                -> LastReport assigned
                    -> LaunchCompleted dispatched

Failed ordering:

    Failed snapshot accepted
        -> immutable report finalized
            -> LastReport assigned
                -> LaunchFailed dispatched

Interrupted ordering:

    Interrupted snapshot accepted
        -> immutable report finalized
            -> LastReport assigned
                -> LaunchInterrupted dispatched

Terminal listeners observe the already-authoritative root state and the exact `LastReport` instance supplied as the event payload.

Per-listener failures remain isolated by `LaunchNotificationDispatcher` through `ELAUNCH-EVENT-001`.

Duplicate roots expose no report and publish no terminal report event.

Root destruction suppresses unsafe late terminal-event publication.

## Completed Destination Handoff

After a successful or warning-only startup sequence:

1. The root publishes `Transitioning`.
2. The validated initial destination loader is invoked exactly once.
3. Accepted destination progress replaces progress while state remains `Transitioning`.
4. Loader failure maps to `ELAUNCH-DEST-002` and `Failed`.
5. Loader cancellation maps to interruption after settlement.
6. Loader success must match the authored destination identity.
7. Destination activation success publishes `Completed`.
8. The completed report is finalized and assigned.
9. `LaunchCompleted` dispatches exactly once.

The final lifecycle snapshot describes successful destination activation. Any startup warnings remain preserved through `WarningCount` and immutable per-step reports.

## Automatic Root Start Boundary

The authoritative root has a serialized automatic-start setting that defaults to enabled.

Unity entry:

```csharp
private async Awaitable Start()
```

The callback does not create a second execution path. It returns when automatic startup is disabled or when the session has already advanced, otherwise it awaits the existing `StartLaunchAsync` gate.

Therefore automatic startup retains:

- Authority filtering
- `AuthorityClaimed` start-state requirement
- Atomic active-run protection
- Existing lifecycle and cancellation behavior
- Existing terminal report and event ordering
- Duplicate-root rejection

Manual startup before Unity `Start` advances the same gate, so the later Unity callback performs no second run.

## Neutral Presenter Boundary

`ILaunchStatusPresenter` is a public Runtime contract with four callbacks:

```csharp
void Bind(LaunchProgressSnapshot initialSnapshot);
void Present(LaunchProgressSnapshot snapshot);
void PresentTerminal(LaunchReport report);
void Unbind();
```

Presenters observe immutable accepted truth. They do not own:

- Launch authority
- Lifecycle transitions
- Startup-step execution
- Destination loading
- Report finalization
- General UI navigation
- Scene travel

The root stores an optional serialized `MonoBehaviour` and resolves it to `ILaunchStatusPresenter`. This permits a later isolated uGUI assembly without making the neutral Runtime assembly reference uGUI.

## Headless Presentation

When no presenter component is assigned, First Light uses `NullLaunchStatusPresenter`.

The fallback:

- Produces no logs
- Allocates no visual resources
- Does not affect launch state
- Preserves headless and test execution

An explicitly assigned component that does not implement the contract emits `ELAUNCH-VIEW-001` and falls back to the headless presenter.

## Presenter Ordering and Containment

Binding occurs once before the root publishes `Validating`.

For accepted progress:

1. `LaunchSession.Publish` accepts the snapshot.
2. The presenter receives the accepted snapshot.
3. If the root remains live, public state/progress events dispatch.

For terminal reports:

1. Terminal lifecycle state is accepted.
2. The immutable report is finalized.
3. `LastReport` is assigned.
4. The presenter receives that exact report.
5. If the root remains live, the matching public terminal event dispatches.

Every presenter callback is isolated. Exceptions produce `ELAUNCH-VIEW-002` and do not alter lifecycle truth or block later public notifications.

A successfully bound presenter is unbound once during root destruction. Duplicate roots never bind or present.

## Default Plain uGUI View

`EchoLaunchStatusView` is the first package-supplied visual implementation of
`ILaunchStatusPresenter`.

Assembly:

```text
EchoDevGames.EchoLaunch.Presentation.UGUI
```

References:

- Neutral First Light Runtime.
- Unity uGUI.

The neutral Runtime assembly does not reference the presentation assembly,
uGUI, TextMeshPro, Canvas, `Text`, or `Slider`.

The root continues to serialize only a neutral `MonoBehaviour` seam and resolves
the component through `ILaunchStatusPresenter`.

## Plain Status Surfaces

The view can render:

- Lifecycle state copy.
- Diagnostic or detail copy.
- Active step position and stable step ID.
- Elapsed launch time.
- Determinate progress through a normalized `Slider`.
- Determinate percentage text.
- A distinct indeterminate-progress surface.
- Final destination display metadata.
- Completed, failed, and interrupted terminal reports.

The view uses serialized legacy uGUI `Text` references and introduces no
TextMeshPro dependency.

All state copy is serialized and replaceable. This creates a localization-ready
authoring seam but does not claim localization integration.

## Text-Complete Meaning

State meaning does not require color.

Default copy includes:

```text
Preparing launch.
Validating launch.
Starting systems.
Continuing with a warning.
Loading destination.
Launch complete.
Launch blocked.
Launch interrupted.
```

Warnings and diagnostic results may show stable code plus sanitized message.

Determinate and indeterminate progress use separate surfaces and separate text.

## Terminal Presentation

Completed report behavior:

- Retains the exact finalized report.
- Shows completed copy.
- Shows destination display name.
- Shows final result message.
- Forces determinate progress to 100 percent.

Failed and interrupted report behavior:

- Shows matching terminal copy.
- Shows diagnostic code and message.
- Preserves the most recently accepted progress mode.

A null terminal report is rejected. A valid terminal report before binding is
ignored without changing visible state.

## Visibility and Replacement

The view supports serialized:

- Show on bind.
- Hide on unbind.
- Clear on unbind.

A `CanvasGroup` controls startup-only visibility without requiring the root to
destroy the view.

Missing optional text, slider, or progress-surface references remain safe. The
view still tracks accepted snapshots and reports.

The checkpoint does not create a package prefab, Canvas hierarchy, background,
font asset, logo, animation, or splash player.

## Presentation Test Assembly

Presentation proof lives in:

```text
EchoDevGames.EchoLaunch.Tests.Presentation.UGUI
```

Runtime internal report constructors are exposed only to this named test
assembly. Presentation internals are likewise exposed only to the same test
assembly.

This avoids widening the public report API merely to test visual projection.

## Project-Owned Image Splash Definitions

`SplashSequence` is a project-owned `ScriptableObject` with independent schema
version `1`.

Each `SplashEntry` stores:

- Stable canonical entry ID.
- Image sprite.
- Replaceable display label.
- Fade-in seconds.
- Hold seconds.
- Fade-out seconds.
- Minimum display seconds.
- Skip policy.

Runtime validates but does not repair or rewrite the authored asset.

Validation rejects:

- Malformed sequence identity.
- Unsupported sequence schema.
- Missing entry collection.
- Null entries.
- Malformed entry identity.
- Missing image.
- Negative or nonfinite timing.
- Undefined skip policy.
- Duplicate entry IDs.

## Deterministic Splash Player

`SplashSequencePlayer` owns only temporary traversal, timing, alpha, and latched
skip state.

The player uses `ILaunchClock`, preserving the same monotonic unscaled-time seam
used by startup execution.

Effective timing:

```text
fadeIn = reducedMotion ? 0 : authoredFadeIn
fadeOut = reducedMotion ? 0 : authoredFadeOut
hold = max(
    authoredHold,
    minimumDisplay - fadeIn - fadeOut,
    0)
total = fadeIn + hold + fadeOut
```

The player rejects:

- NaN, infinite, or negative clock values.
- Backward clock movement.
- Concurrent playback on the same player.

Cancellation always unsubscribes skip requests, clears presentation, and
releases the active-playback gate.

## Skip Semantics

Stable policies:

```text
Disallowed
AfterMinimumDisplay
```

A permitted skip request is latched.

If it arrives early, playback continues until the minimum-display boundary and
then ends the active entry.

A disallowed request has no effect.

The package does not bind input. Project-owned input calls the public
`RequestSplashSkip()` seam on the default view or raises the neutral presenter
event through another implementation.

## Reduced Motion

Reduced-motion playback removes fade-in and fade-out phases.

It preserves:

- Authored hold.
- Minimum display time.
- Entry ordering.
- Skip policy.
- Full-opacity image presentation.

The caller supplies the reduced-motion choice. Platform preference discovery is
outside this checkpoint.

## Immutable Splash Projection

`SplashPresentationFrame` contains accepted immutable presentation truth:

- Sequence and entry identity.
- Sprite and label.
- Entry index and count.
- Playback phase.
- Normalized alpha.
- Elapsed and minimum-display time.
- Whether skipping is currently permitted.
- Reduced-motion state.

`SplashPlaybackResult` stores:

- Sequence identity.
- Presented entry count.
- Skipped entry count.
- Total elapsed time.
- Reduced-motion state.

## Neutral Splash Presenter

`IImageSplashPresenter` receives frames and exposes one neutral `SkipRequested`
event.

`NullImageSplashPresenter` provides logging-free headless behavior.

A missing visual presenter therefore does not invalidate deterministic playback.

## Default uGUI Splash Projection

`EchoLaunchStatusView` now implements both:

```text
ILaunchStatusPresenter
IImageSplashPresenter
```

The view can render:

- Splash sprite.
- Replaceable label.
- Accepted frame alpha.
- `Splash N of M` position.
- Replaceable `Showing splash.` state copy.

`RequestSplashSkip()` returns `false` when the view is unbound, splash
presentation is inactive, or no playback subscriber exists.

Clearing or unbinding:

- Hides the splash root.
- Clears the image sprite.
- Resets image alpha.
- Clears the splash label.
- Releases stored splash frame state.
- Removes skip-request handlers on unbind.

Missing optional splash references remain safe.

## Integration Boundary

FL-M4-03 deliberately does not bind `SplashSequence` to
`EchoLaunchConfiguration`.

Configuration remains schema version `3`.

Launch reports remain schema version `2`.

Root-owned splash playback, configuration serialization, report integration, and
lifecycle placement require an authority-first follow-up checkpoint.

## Schema-4 Splash Configuration

`EchoLaunchConfiguration.CurrentSchemaVersion` is now:

```text
4
```

Schema 4 stores:

- `StartupSequence`
- `InitialDestination`
- Optional `SplashSequence`
- `UseReducedMotionForSplash`

Historical schemas remain unsupported at runtime.

A null splash reference means the splash phase is intentionally omitted.

An assigned empty but valid sequence is a legal no-op.

Runtime reads but never repairs, migrates, or rewrites configuration or splash
assets.

## Root Splash Preflight

`SplashSequencePreflight` validates the optional assigned sequence before any
splash frame, startup-step executor, or destination side effect.

The root preflight order is:

```text
configuration identity and schema
    -> optional splash sequence
    -> startup sequence
    -> initial destination
```

An invalid assigned sequence produces:

```text
ELAUNCH-SPLASH-001
```

Startup-sequence validation also occurs before splash playback. An invalid
startup sequence therefore cannot display a splash before blocking.

## Root-Owned Phase Order

The authoritative order is:

```text
bind presentation
    -> publish validation
    -> validate all assigned launch definitions
    -> play optional splash
    -> run startup sequence
    -> load initial destination
    -> publish completed handoff
```

Splash playback and startup steps do not overlap.

The splash presenter is cleared before startup-step presentation begins.

The destination loader cannot start until splash and startup execution settle.

## Shared Launch Clock

The root supplies the same injected `ILaunchClock` seam to:

- Splash playback.
- Startup-sequence execution.
- Root report timing.

This keeps ordering and elapsed-time proof deterministic.

Successful splash time contributes to the existing total launch elapsed time.

## Splash Presenter Resolution

When the active status presenter also implements `IImageSplashPresenter`, the
root uses it.

When a nonempty sequence is configured but the status presenter has no splash
surface, the root emits:

```text
ELAUNCH-SPLASH-003
```

Playback continues through `NullImageSplashPresenter`.

Headless fallback preserves authored timing, minimum display, skip policy, and
reduced-motion behavior.

Empty sequences use the silent headless path without warning.

## Splash Failure and Cancellation

Unexpected splash playback, clock, or presenter failure produces:

```text
ELAUNCH-SPLASH-002
```

The launch fails before startup steps and destination loading.

Root cancellation during splash uses the existing lifecycle interruption:

```text
ELAUNCH-LIFE-001
```

Cancellation:

- Clears splash presentation.
- Prevents startup-step execution.
- Prevents destination loading.
- Finalizes one interrupted report.
- Publishes one interrupted terminal event.

Duplicate roots cannot present or play another splash.

## Splash Execution Evidence

The root retains the latest successful `SplashPlaybackResult` internally for
focused runtime evidence.

The immutable public launch report remains schema version `2`.

No splash-specific report fields were added.

Existing report fields carry:

- Total elapsed launch time.
- Final status.
- Final diagnostic code and message.
- Existing startup-step reports.
- Existing destination metadata.

## Neutral Package Prefab Templates

The removable uGUI assembly now contains two stable public package assets:

```text
Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab
Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

Their committed `.meta` files establish package asset identity.

### Status-View Prefab

`EchoLaunchStatusView.prefab` is a self-contained startup-only Canvas with:

- `RectTransform`
- `Canvas`
- `CanvasScaler`
- `CanvasGroup`
- `EchoLaunchStatusView`

Approved Canvas defaults:

```text
Render Mode: Screen Space Overlay
Sorting Order: 1000
Scale Mode: Scale With Screen Size
Reference Resolution: 1920 x 1080
Match Width Or Height: 0.5
Reference Pixels Per Unit: 100
```

The root Canvas begins hidden and non-interactive:

```text
CanvasGroup Alpha: 0
Interactable: false
Blocks Raycasts: false
```

The committed hierarchy provides:

```text
EchoLaunch Status Canvas
├── Backdrop
├── Splash Root
│   ├── Splash Image
│   └── Splash Label
└── Status Root
    ├── State Text
    ├── Message Text
    ├── Step Text
    ├── Determinate Progress Root
    │   ├── Progress Slider
    │   └── Progress Text
    ├── Indeterminate Progress Root
    │   └── Indeterminate Text
    └── Elapsed Text
```

Every existing private serialized view reference is assigned.

The splash and progress roots begin inactive.

All graphics reject raycasts, and the progress slider is non-interactable.

### Root Prefab

`EchoLaunchRoot.prefab` contains:

```text
EchoLaunch Root
└── EchoLaunch Status Canvas
```

The child is a nested instance of `EchoLaunchStatusView.prefab`.

Serialized root defaults:

```text
Configuration: null
Launch Mode: CanonicalBoot
Start Automatically: true
Status Presenter: nested EchoLaunchStatusView
```

The null configuration is intentional because launch definitions are
project-owned.

### Dependency Boundary

The templates contain no:

- Project `Assets/` dependency.
- Project logo or branded art.
- Project font.
- TextMeshPro component.
- EventSystem or input module.
- GraphicRaycaster.
- Button or Toggle.
- Package-owned skip binding.

Runtime does not discover, load, repair, or instantiate these prefabs
automatically.

Projects explicitly place the root prefab, make a prefab variant, copy the
template into project assets, or replace the presenter.

### Asset Authoring

Unity Editor APIs generated the final prefab YAML so private serialized
references and nested-prefab identity were authored by Unity rather than by
manual YAML editing.

The temporary authoring helper was deleted before staging.

Generated trailing whitespace was trimmed from prefab YAML and metadata without
changing GUIDs or serialized behavior.

## Create-Only Editor Setup Apply

FL-M5-02 extends the FL-M5-01 observation and planning boundary with one
approved mutation service. Planning remains side-effect-free. Apply may execute
only a freshly recollected, equivalent, executable plan.

Approved flow:

```text
EchoLaunchSetupWindow
    -> EchoLaunchSetupRequest
    -> EchoLaunchProjectSnapshotCollector
    -> EchoLaunchSetupPlanner
    -> EchoLaunchSetupFingerprint
    -> EchoLaunchSetupApplyService
        -> EchoLaunchSetupAssetWriter
        -> EchoLaunchSetupPrefabWriter
        -> EchoLaunchSetupSceneWriter
        -> EchoLaunchSetupBuildSettingsWriter
        -> EchoLaunchSetupRollbackJournal
    -> EchoLaunchSetupApplyResult
    -> EchoLaunchSetupApplyResultFormatter
```

### Freshness and Concurrency

Immediately before the first write, Apply recollects project evidence, rebuilds
the plan, and compares deterministic fingerprints. A changed project produces a
stale-plan result instead of applying outdated intent.

Only one Apply attempt may be active. Re-entry returns a stable diagnostic and
performs no mutation.

### Executable Operations

Only these dispositions may execute:

```text
Create
Reuse
NoChange
```

`ManualDecision`, `Conflict`, and `Unsupported` remain blocking. Apply does not
convert repair or migration cases into writes.

Creation order is deterministic:

1. Project-owned folders.
2. Definition assets.
3. `EchoLaunchConfiguration` and serialized references.
4. Project-owned `EchoLaunchRoot` prefab variant.
5. Boot scene.
6. Build Settings mutation, when approved.

### Asset and Scene Safety

- Existing compatible project assets are reused without modification.
- Existing incompatible targets block before writes.
- The selected destination scene is never opened or modified.
- Existing open, active, and dirty scene state is preserved.
- Boot creation uses an isolated scene lease and restores the leased Scene name
  through a mutable local struct copy.
- Package-owned templates remain immutable.

### Build Settings

Supported policies:

```text
DoNotChange
AddIfMissingAtEnd
PlaceFirstAfterApproval
```

Build Settings writes last. The writer preserves unrelated paths, order, and
enabled states. The default policy appends one enabled Boot scene. Place-first
requires explicit approval.

### Rollback and Result

Every mutation created by the active attempt is registered in an in-memory
compensating rollback journal. On failure, rollback proceeds in reverse order.
The immutable result records:

- Status and message.
- Final plan status and fingerprint.
- Created and reused paths.
- Build Settings before and after.
- Whether rollback completed.
- Manual recovery paths when compensation is incomplete.

The first accepted manual Apply succeeded. The second and third Apply returned
`NoChanges` with the same fingerprint and no duplicate Build Settings entry.

### Stable Apply Diagnostics

- `ELAUNCH-SETUP-008` stale plan.
- `ELAUNCH-SETUP-009` Apply already active.
- `ELAUNCH-SETUP-010` Apply failed and rollback completed.
- `ELAUNCH-SETUP-011` rollback incomplete; manual recovery required.
- `ELAUNCH-SETUP-012` unauthorized operation disposition.

## Compile Evidence

The deterministic manual-clock helpers and immediate executors intentionally complete synchronously.

Local `CS1998` suppression keeps those bounded test helpers warning-free.

One test helper was adapted to the Unity `6000.3.8f1` by-value `AwaitableCompletionSource<T>.SetResult` signature.

The retained immediate fixture was realigned to preserve FL-M3-02 policy-aware assertions plus the FL-M3-03 linked-token assertion.

Final FL-M5-02 compile result:

- Errors: `0`
- Warnings: `0`

## Retained Lifecycle Architecture

`LaunchStateTransitionRules` remains the single internal authority for lifecycle legality.

`EchoLaunchRoot` publishes runner-derived state and progress only through the existing transactional `LaunchSession.Publish` boundary.

Listener failures remain isolated through `ELAUNCH-EVENT-001`.

The runner remains neutral: it emits internal observations but does not own root authority, lifecycle transition rules, notification dispatch, presentation, or destination policy.

## Test Evidence

Full EditMode totals:

- Passed: `197`
- Failed: `0`
- Ignored: `0`

FL-M5-02 setup and apply tests:

- Passed: `170`
- Failed: `0`
- Ignored: `0`

Retained prefab asset tests:

- Passed: `27`
- Failed: `0`
- Ignored: `0`

Runtime Play Mode totals:

- Passed: `479`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Editor setup and apply tests: `170` EditMode
- Prefab asset tests: `27` EditMode
- Root splash integration tests: `28` Runtime Play Mode
- Additional schema-history test: `1`
- Splash playback tests: `26`
- Splash uGUI presentation tests: `10`
- Plain uGUI presentation tests: `18`
- Automatic-start and presenter tests: `16`
- Authority tests: `7`
- Root-owned startup lifecycle tests: `23`
- Clock, timing, and progress-gate tests: `14`
- Configuration and destination binding tests: `22`
- Vocabulary tests: `39`
- Session and progress tests: `14`
- Lifecycle transition tests: `22`
- Lifecycle notification tests: `20`
- Destination and completed-handoff tests: `37`
- Launch report and terminal-event tests: `25`
- Startup sequence definition tests: `24`
- Startup step policy and executor-contract tests: `28`
- Startup step execution tests: `12`
- Immediate startup sequence runner tests: `18`
- Policy-application tests: `16`
- Runner policy and exception tests: `16`
- Timeout runner and cancellation tests: `18`
- Multi-frame async runner tests: `2`
- Preflight and re-entry tests: `23`

Verified FL-M5-02 and retained behavior:

- Fresh-plan fingerprint equality and stale-plan rejection
- Single-active-Apply rejection
- Create/reuse/no-change execution boundary
- Deterministic folder and asset creation
- Configuration binding
- Project root prefab variant generation
- Boot scene creation with destination and open-scene preservation
- Build Settings append and approved place-first policies
- Reverse-order compensating rollback and manual recovery reporting
- Immutable apply-result formatting
- First manual Apply `Succeeded`
- Second and third manual Apply `NoChanges`
- Stable fingerprint `7e669d66eaab2c04a0dfbc4445458fcd976808c83f62db82c3d91a16494fc0c1`

Retained FL-M5-01 and earlier behavior:

- Stable preview-only Setup menu path
- Approved preview-only warning
- Approved default path generation
- Backslash and duplicate-separator normalization
- Absolute/external/traversal/wrong-extension rejection
- Immutable setup request
- Immutable asset and Build Settings facts
- Immutable operation, diagnostic, and plan values
- Defensive plan collection copies
- Deterministic plan equality and ordering
- Missing project assets represented as create proposals only
- Existing compatible asset reuse
- Incompatible target conflict blocking
- Unsupported configuration schema blocking
- Multiple candidate manual decisions
- Explicit selected candidate reuse
- Optional splash planning
- Package-template prerequisite validation
- Append-if-missing Build Settings proposal
- Do-not-change Build Settings proposal
- Place-first explicit approval
- Existing Boot entry no-change handling
- Read-only Build Settings observation
- Open-scene setup preservation
- Package-template dirty-state preservation
- Missing destination evidence
- Deterministic plain-text reports
- Window plan refresh and report generation
- No Apply/Repair/Migrate mutation methods
- No project folder or Boot-scene creation
- Retained 27 prefab asset tests
- Retained 479 Runtime Play Mode tests

Verified FL-M4-05 and retained behavior:

- Stable package prefab paths and distinct GUIDs
- Approved Canvas and CanvasScaler defaults
- Hidden/non-interactive initial Canvas state
- Required status and splash hierarchy
- Complete serialized view reference wiring
- Splash and progress roots inactive by default
- Non-interactive slider and non-raycast graphics
- No package input authority
- No TextMeshPro component
- Built-in non-project font use
- No project asset dependencies
- Nested status prefab within the root prefab
- Presenter reference to the nested view
- Intentionally null project configuration
- Canonical Boot and automatic start defaults
- No missing scripts
- Successful prefab instantiation
- Temporary authoring helper removal
- Retained 479-test Runtime Play Mode suite

Verified FL-M4-04 and retained behavior:

- Configuration schema 4
- Optional splash and reduced-motion configuration binding
- Historical schema 3 rejection without rewrite
- Null and empty splash no-op behavior
- Splash and startup preflight before side effects
- Splash presentation before startup steps
- Splash clear before step presentation
- Startup completion before destination load
- Reduced-motion forwarding
- Headless fallback warning
- Project-routed skip through the root path
- Total elapsed time including splash duration
- Successful splash result retention
- Presenter/playback failure blocking later phases
- Cancellation during splash with exactly-once interruption
- Duplicate-root splash silence
- Automatic-start splash routing
- Direct-scene contract consistency
- Configuration and splash immutability
- Report schema 2 preservation

Verified FL-M4-03 and retained behavior:

- Splash sequence schema 1
- Canonical and distinct generated identities
- Entry timing and image validation
- Duplicate entry-ID detection
- Empty and ordered multi-entry playback
- Deterministic fade phases and normalized alpha
- Minimum-display expansion
- Permitted, early, and disallowed skip behavior
- Reduced-motion fade removal
- Cancellation cleanup
- Player re-entry protection
- Invalid and backward clock rejection
- Headless fallback
- Immutable authored assets
- Immutable frames and result accounting
- uGUI splash image, label, alpha, and position
- Public skip-request event
- Clear and unbind cleanup
- Missing splash-reference safety
- Configuration and report schemas unchanged

Verified FL-M4-02 and retained behavior:

- Separate uGUI presentation runtime assembly
- Separate presentation test assembly
- Plain view implementing `ILaunchStatusPresenter`
- Bind visibility and initial accepted snapshot
- Determinate slider progress and percentage
- Indeterminate progress surface
- Step and elapsed-time copy
- Warning diagnostics
- Transitioning copy
- Completed destination and full progress
- Failed and interrupted diagnostic rendering
- Pre-bind no-op behavior
- Null terminal-report rejection
- Hide and clear unbind behavior
- Rebind report reset
- Missing optional-reference safety
- Serialized copy replacement
- Neutral Runtime remaining uGUI-free
- No TextMeshPro dependency

Verified FL-M4-01 and retained behavior:

- Automatic Unity `Start` launch
- Serialized automatic-start opt-out
- Manual-before-automatic one-run protection
- Public neutral presenter contract
- Logging-free headless fallback
- Serialized presenter component resolution
- `ELAUNCH-VIEW-001` invalid-component containment
- `ELAUNCH-VIEW-002` callback-failure containment
- Binding before validation
- Accepted snapshot presentation ordering
- Finalized report presentation ordering
- Completion-event continuity after presenter failure
- Presenter replacement and null-injection guards
- Exactly-once unbind on destruction
- Duplicate-root automatic-start and presenter silence
- Runtime assembly remaining uGUI-neutral

Verified FL-M3-08 and retained behavior:

- Project-owned immutable destination schema 1
- Configuration schema 3 destination binding
- Historical schema 2 rejection without rewrite
- Destination identity, label, path, and loader preflight
- Immutable load status and result contract
- Normalized transition progress
- Default Unity loader preflight and pre-start cancellation
- Exactly-once injected loader invocation
- Successful `Transitioning -> Completed` handoff
- Completed report schema 2
- Destination metadata in completed reports
- Exact `LastReport` and `LaunchCompleted` payload identity
- Exactly-once completion publication
- Completion-listener isolation
- Destination failure and null/mismatched result containment
- Cancellation before and during transition
- Destruction-driven late-completion suppression
- Startup warning preservation in completed reports
- Configuration and destination asset immutability

Verified FL-M3-07 and retained behavior:

- Immutable public `LaunchStepReport`
- Immutable public `LaunchReport`
- Report schema version `1`
- Internal single-use report builder
- Failed preflight and blocking reports
- Interrupted report after executor settlement
- `LastReport` identity with terminal event payload
- State-before-terminal-event ordering
- Exactly-once failed and interrupted event publication
- Listener isolation through `ELAUNCH-EVENT-001`
- Defensive collection copying
- Report readability after runtime object release
- Duplicate-root report and event silence
- Destruction-driven late-event suppression
- Transition-pending success without finalized report
- Report accounting and timing validation
- Authored asset immutability

Verified FL-M3-06 and retained behavior:

- Authority claim without automatic startup
- Root-owned explicit startup execution
- `Validating`, `Running`, `Failed`, `Interrupted`, and `Transitioning` lifecycle publication
- Step-start, progress, and completion snapshot translation
- Warning-only success to `Transitioning`
- Blocking and preflight failure mapping to `Failed`
- Cooperative root cancellation after executor settlement
- Stable `ELAUNCH-LIFE-001` and `ELAUNCH-LIFE-002`
- Repeated and duplicate-root cancellation rejection
- Destruction-driven cancellation and late-publication suppression
- No premature `Completed` publication
- Direct-scene launch-mode preservation
- Structured observer-path preflight diagnostics
- Exact legacy direct-runner exception compatibility
- Root active-gate release
- Root-owned authored asset immutability

Verified FL-M3-05 and retained behavior:

- Complete preflight before executor factory creation
- Configuration and sequence identity/schema validation
- Entry identity, activation, and uniqueness validation
- Enabled definition presence validation
- Step identity, schema, and uniqueness validation
- Invalid policy blocking before factory creation
- Empty-sequence compatibility
- Disabled-entry-without-definition compatibility
- Concurrent runner re-entry rejection
- Stable `ELAUNCH-RUN-001`
- No second factory during rejected re-entry
- Gate release after success, preflight rejection, structured cancellation, and blocking traversal
- Sequential runner reuse
- Clock interface and default implementation
- Deterministic manual clock
- Immutable timing validation
- Progress-gate forwarding and closure
- Single timing assignment
- Zero-timeout behavior
- Completion before deadline
- Completion at the observed deadline boundary
- Stable `ELAUNCH-STEP-003`
- Timeout diagnostic details
- Supported timeout cancellation
- Unsupported timeout behavior
- Late success containment
- Late failure containment
- Timeout cancellation-exception containment
- Structured caller cancellation after executor settlement
- Stable `ELAUNCH-STEP-005`
- Run-level `WasCancelled`
- Same-tick cancellation-race containment
- Production-shaped multi-frame `Awaitable.NextFrameAsync` execution
- Multi-frame progress, positive timing, and authored order
- Continue-with-warning timeout
- Block-launch timeout
- Late-progress containment
- Backward-clock containment
- Executor settlement before later factory creation
- Authored asset immutability
- Zero compiler errors
- Zero compiler warnings

Expected retained diagnostics:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

No production asset, scene, prefab, or automatic startup setup was required.

## Current Exclusions

Not implemented:

- Automatic retry
- Retry count or backoff
- Interactive retry
- Retry or skip UI
- Public step lifecycle events
- Warning aggregation outside the run result
- Dependency validation
- Explicit setup repair and existing-asset reconciliation
- Editor migration from historical configuration schemas
- Direct-scene initializer tooling
- Real Boot-to-destination Standalone Laboratory proof
- Persistent-root lifetime policy
- Direct-scene initialization behavior
- Standalone Laboratory
- Peer-package bridges

## Stop Point

FL-M5-02 stops after the fresh-plan-gated create-only Apply service,
deterministic project-owned foundation creation, explicit Build Settings
mutation, compensating rollback, immutable result reporting, one successful
manual Apply, two repeat-safe `NoChanges` Applies, one hundred ninety-seven
passing EditMode tests, and four hundred seventy-nine passing Runtime Play Mode
tests.

Repairing or migrating existing assets, persistent receipts, uninstall/reset,
crash-persistent recovery, direct-scene initialization, persistent-root policy,
player builds, external adoption, and real Standalone Laboratory activation
require later approved checkpoints.
