# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Runtime contracts established; execution not yet implemented
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
  - `FL-M2-03`
  - `FL-M2-04`
  - `FL-M2-05`
  - `FL-M2-06`
  - `FL-M2-07`
  - `FL-M2-08`
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

It does not yet execute startup behavior.

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
    └── StartupStepPolicyAndExecutorContractTests.cs

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

## Retained Lifecycle Architecture

`LaunchStateTransitionRules` remains the single internal authority for lifecycle legality.

`EchoLaunchRoot` continues to dispatch accepted state and progress notifications after authoritative state changes.

Listener failures remain isolated through `ELAUNCH-EVENT-001`.

No FL-M2-08 contract calls or mutates these systems.

## Test Evidence

Runtime Play Mode totals:

- Passed: `169`
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

Manual verification:

- Unity created a temporary `StartupSequence`.
- A newly added list entry initially exposed unsafe false boolean defaults.
- The serialized model was corrected to safe zero-valued enums.
- Recreated entries displayed:
  - Enabled
  - Required
  - Block Launch
  - Timeout `0`
  - Cancellation Supported
- No executor, root, lifecycle transition, timeout, retry, preflight, or warning occurred.
- The temporary verification asset was removed before Git review.

## Current Exclusions

Not implemented:

- Startup sequence runner
- Active step execution object
- Executor invocation
- Timeout clock
- Timeout cancellation
- Retry loops
- Interactive retry
- Exception conversion
- Result-to-policy application
- Configuration or sequence preflight
- Duplicate-ID collision validation
- Automatic lifecycle advancement
- Step lifecycle events
- Launch reports
- Splash presentation
- Scene loading
- Persistent-root lifetime
- Direct-scene initialization behavior
- Custom inspectors and setup windows
- Standalone Laboratory
- Peer-package bridges

## Stop Point

FL-M2-08 stops after authored policy, immutable progress/context values, and the fresh executor API are proven without invoking an executor.

The next runtime slice requires separate approval.
