# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Monotonic timeout execution and cooperative cancellation implemented; retries and lifecycle integration pending
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

First Light now executes, times, and evaluates startup steps only through explicit internal runner calls. It is not connected to `EchoLaunchRoot`, Unity scene callbacks, launch lifecycle, presentation, or destination loading.

## Implemented Runtime Files

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
    │   ├── StartupSequenceRunResult.cs
    │   ├── StartupSequenceRunner.cs
    │   ├── StartupStepAwaitOutcome.cs
    │   ├── StartupStepExceptionConverter.cs
    │   ├── StartupStepExceptionPhase.cs
    │   ├── StartupStepExecution.cs
    │   ├── StartupStepPolicyDecision.cs
    │   ├── StartupStepPolicyEvaluator.cs
    │   ├── StartupStepProgressGate.cs
    │   ├── StartupStepTimeoutMonitor.cs
    │   ├── StartupStepTiming.cs
    │   └── UnityLaunchClock.cs
    ├── Properties/
    │   └── AssemblyInfo.cs
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

    Tests/Runtime/PlayMode/
    ├── EchoLaunchRootAuthorityTests.cs
    ├── LaunchClockTimingAndGateTests.cs
    ├── LaunchConfigurationBindingTests.cs
    ├── LaunchLifecycleTransitionTests.cs
    ├── LaunchNotificationTests.cs
    ├── LaunchSessionProgressTests.cs
    ├── LaunchStateVocabularyTests.cs
    ├── StartupSequenceDefinitionTests.cs
    ├── StartupSequenceRunnerImmediateTests.cs
    ├── StartupSequenceRunnerPolicyAndExceptionTests.cs
    ├── StartupSequenceRunnerTimeoutTests.cs
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
- Immutable timing snapshot

A timed-out outcome may contain a later success, failure, or cancellation exception, but timeout remains the runner's source result.

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

The monitor also waits for settlement before allowing caller cancellation or a clock-contract failure to escape.

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

FL-M3-03 allows caller cancellation to escape as `OperationCanceledException` only after the active executor has settled. Converting caller cancellation into a structured run result remains later work.

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
18. Converts non-timeout executor exceptions through `ELAUNCH-STEP-004`.
19. Converts null results to blocking contract failures.
20. Applies authored failure policy.
21. Completes the execution with effective result and timing.
22. Appends the execution in authored order.
23. Continues or stops according to the policy decision.
24. Disposes per-attempt cancellation sources.
25. Returns immutable traversal accounting.

No later executor factory is called while a timed-out executor remains active.

FL-M3-03 deliberately does not:

- Retry
- Produce a structured caller-cancellation run result
- Publish root events
- Update `LaunchSession`
- Build a public report
- Start automatically

## Compile Evidence

The deterministic manual-clock helpers and immediate executors intentionally complete synchronously.

Local `CS1998` suppression keeps those bounded test helpers warning-free.

One test helper was adapted to the Unity `6000.3.8f1` by-value `AwaitableCompletionSource<T>.SetResult` signature.

The retained immediate fixture was realigned to preserve FL-M3-02 policy-aware assertions plus the FL-M3-03 linked-token assertion.

Final FL-M3-03 compile result:

- Errors: `0`
- Warnings: `0`

## Retained Lifecycle Architecture

`LaunchStateTransitionRules` remains the single internal authority for lifecycle legality.

`EchoLaunchRoot` continues to dispatch accepted state and progress notifications after authoritative state changes.

Listener failures remain isolated through `ELAUNCH-EVENT-001`.

No FL-M3-03 timing, timeout, or cancellation path calls or mutates these systems.

## Test Evidence

Runtime Play Mode totals:

- Passed: `263`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Clock, timing, and progress-gate tests: `14`
- Configuration binding tests: `15`
- Vocabulary tests: `39`
- Session and progress tests: `14`
- Lifecycle transition tests: `22`
- Lifecycle notification tests: `20`
- Startup sequence definition tests: `24`
- Startup step policy and executor-contract tests: `28`
- Startup step execution tests: `12`
- Immediate startup sequence runner tests: `18`
- Policy-application tests: `16`
- Runner policy and exception tests: `16`
- Timeout runner and cancellation tests: `18`

Verified FL-M3-03 behavior:

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
- Caller cancellation boundary
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

No production asset, scene, prefab, root, or automatic startup setup was required.

## Current Exclusions

Not implemented:

- Automatic retry
- Retry count or backoff
- Interactive retry
- Retry or skip UI
- Structured caller-cancellation run result
- Root-level cancellation command
- Shutdown or destruction cancellation orchestration
- `EchoLaunchRoot` runner integration
- Automatic startup from Unity callbacks
- Launch-session lifecycle advancement
- Public step lifecycle events
- Launch reports
- Warning aggregation outside the run result
- Configuration or sequence preflight
- Duplicate-ID collision validation
- Dependency validation
- Runner re-entry protection
- Production-shaped multi-frame asynchronous proof
- Splash presentation
- Scene loading
- Persistent-root lifetime
- Direct-scene initialization behavior
- Custom inspectors and setup windows
- Standalone Laboratory
- Peer-package bridges

## Stop Point

FL-M3-03 stops after monotonic unscaled timeout measurement, deterministic completion-versus-deadline ordering, stable timeout results, cooperative timeout cancellation, late progress containment, and executor settlement are proven.

The next runtime slice requires separate approval.
