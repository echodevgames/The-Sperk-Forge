# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Policy-aware immediate execution implemented; timeout and lifecycle integration pending
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

First Light now executes and evaluates startup steps only through explicit internal runner calls. It is not connected to `EchoLaunchRoot`, Unity scene callbacks, launch lifecycle, presentation, or destination loading.

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
    │   ├── StartupSequenceRunResult.cs
    │   ├── StartupSequenceRunner.cs
    │   ├── StartupStepExceptionConverter.cs
    │   ├── StartupStepExceptionPhase.cs
    │   ├── StartupStepExecution.cs
    │   ├── StartupStepPolicyDecision.cs
    │   └── StartupStepPolicyEvaluator.cs
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
    ├── LaunchConfigurationBindingTests.cs
    ├── LaunchLifecycleTransitionTests.cs
    ├── LaunchNotificationTests.cs
    ├── LaunchSessionProgressTests.cs
    ├── LaunchStateVocabularyTests.cs
    ├── StartupSequenceDefinitionTests.cs
    ├── StartupSequenceRunnerImmediateTests.cs
    ├── StartupSequenceRunnerPolicyAndExceptionTests.cs
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
- FL-M2-08 does not measure time.

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
- `CancellationToken`
- `IStartupStepProgressReporter`

Constructor validation rejects:

- Blank identities
- Step count less than one
- Step index outside the current count
- Null progress reporter

The context owns no launch authority and exposes no setters.

## Executor Contract

`IStartupStepExecutor` exposes:

    Awaitable<StartupStepResult> ExecuteAsync(
        StartupStepContext context)

This contract uses Unity `Awaitable<T>`.

Rules established by FL-M2-08:

- One executor instance represents one execution attempt.
- A definition creates a fresh executor for every attempt.
- The definition does not store the executor.
- The executor owns its own active state.
- Cancellation is cooperative through the context token.
- Progress is reported through the package-owned reporter.
- No executor is invoked by FL-M2-08.

Exception conversion, timeout handling, result interpretation, and lifecycle advancement belong to the future runner.

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
- Latest `StartupStepProgress`
- One terminal `StartupStepResult`

Normal execution path:

    NotStarted
        -> Running
            -> terminal result status

Factory or pre-execution contract failure path:

    NotStarted
        -> BlockingFailure

Guards:

- An executor may be attached exactly once before begin.
- Begin requires an attached executor.
- Progress is legal only while running.
- Normal completion is legal only while running.
- Pre-start completion accepts one blocking result only.
- Completion is legal exactly once.
- Progress after completion is rejected.

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

## Policy-Aware Immediate Sequence Runner

`StartupSequenceRunner.RunAsync` accepts:

- Launch mode
- Launch configuration
- Cancellation token

It then:

1. Validates the configuration and active launch mode.
2. Reads the configured startup sequence.
3. Iterates authored indices directly.
4. Skips and counts disabled entries.
5. Creates runtime execution metadata.
6. Calls `CreateExecutor()`.
7. Converts factory exceptions or null executors to blocking `ELAUNCH-STEP-004`.
8. Attaches the fresh executor.
9. Creates immutable `StartupStepContext`.
10. Begins execution.
11. Awaits `ExecuteAsync(context)`.
12. Converts non-cancellation executor exceptions.
13. Converts null results to blocking contract failures.
14. Applies authored failure policy.
15. Completes the execution with the effective result.
16. Appends the execution in authored order.
17. Continues or stops according to the policy decision.
18. Returns immutable traversal accounting.

No later executor factory is called after a stop.

FL-M3-02 deliberately does not:

- Measure timeout
- Orchestrate cancellation
- Retry
- Publish root events
- Update `LaunchSession`
- Build a public report
- Start automatically

## Compile Evidence

The retained immediate test executor intentionally completes synchronously.

Its local `CS1998` suppression keeps Unity compilation clean without changing immediate execution semantics.

Final FL-M3-02 compile result:

- Errors: `0`
- Warnings: `0`

## Retained Lifecycle Architecture

`LaunchStateTransitionRules` remains the single internal authority for lifecycle legality.

`EchoLaunchRoot` continues to dispatch accepted state and progress notifications after authoritative state changes.

Listener failures remain isolated through `ELAUNCH-EVENT-001`.

No FL-M3-02 execution or policy path calls or mutates these systems.

## Test Evidence

Runtime Play Mode totals:

- Passed: `231`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
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

Verified FL-M3-02 behavior:

- Immutable policy decisions
- Preserved and converted results
- Explicit failure-action authority
- Continue-with-warning conversion
- Block-launch conversion and stop
- Cancelled-result preservation
- Factory exception containment
- Null executor containment
- Executor exception conversion
- Null result containment
- Stable `ELAUNCH-STEP-004`
- Sanitized exception details
- Cancellation exception escape
- No later factory creation after stop
- Attempted, disabled, and unvisited accounting
- Stopping authored-index capture
- Complete traversal metadata
- Authored asset immutability
- Zero compiler errors
- Zero compiler warnings

Expected retained diagnostics:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

No production asset, scene, prefab, root, or automatic startup setup was required.

## Current Exclusions

Not implemented:

- Timeout measurement
- `ILaunchClock`
- Timeout race
- Timeout cancellation
- Retry loops
- Retry backoff
- Interactive retry
- Cancellation orchestration
- `EchoLaunchRoot` runner integration
- Automatic startup from Unity callbacks
- Launch-session lifecycle advancement
- Public step lifecycle events
- Launch reports
- Warning aggregation outside the run result
- Configuration or sequence preflight
- Duplicate-ID collision validation
- Runner re-entry protection
- Asynchronous multi-frame proof
- Splash presentation
- Scene loading
- Persistent-root lifetime
- Direct-scene initialization behavior
- Custom inspectors and setup windows
- Standalone Laboratory
- Peer-package bridges

## Stop Point

FL-M3-02 stops after failure policy and bounded exception conversion produce deterministic effective results and blocking decisions stop traversal.

The next runtime slice requires separate approval.
