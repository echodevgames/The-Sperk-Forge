# Changelog

All notable changes to First Light - Startup and Launch will be documented in this file.

The package follows Semantic Versioning once public compatibility commitments begin.

## [Unreleased]

### Added

#### FL-M3-03 - Monotonic Timeout Clock and Cooperative Cancellation

- Public `ILaunchClock` runtime and test seam
- Internal shared `UnityLaunchClock`
- Double-precision unscaled real-time clock source
- Non-blocking Unity frame tick source
- Immutable `StartupStepTiming`
- Internal `StartupStepProgressGate`
- Internal immutable `StartupStepAwaitOutcome`
- Internal `StartupStepTimeoutMonitor`
- Deterministic completion-before-deadline race ordering
- Absolute per-attempt timeout deadlines
- Stable timeout diagnostic `ELAUNCH-STEP-003`
- Timeout details containing configured timeout, measured elapsed time, and cancellation-request state
- Linked caller and timeout cancellation tokens
- Cooperative timeout cancellation only for supporting steps
- Timed-out executor settlement before traversal continues
- Late executor-result containment
- Late progress containment
- Clock-contract validation and backward-clock blocking
- Fourteen Runtime Play Mode clock, timing, and progress-gate tests
- Eighteen Runtime Play Mode timeout-runner and cancellation tests

#### FL-M3-02 - Step Result Policy Application and Exception Conversion

- Immutable `StartupStepPolicyDecision`
- Internal `StartupStepPolicyEvaluator`
- Explicit `ContinueWithWarning` result conversion
- Explicit `BlockLaunch` result conversion and traversal stop
- Cancelled-result preservation and traversal stop
- Internal `StartupStepExceptionPhase`
- Internal `StartupStepExceptionConverter`
- Stable step failure diagnostic `ELAUNCH-STEP-004`
- Blocking factory-exception containment
- Blocking null-executor contract containment
- Policy-aware executor-exception containment
- Blocking null-result contract containment
- Sanitized exception type and message details
- Pre-executor failure capture on `StartupStepExecution`
- Unvisited-entry and stopping-index accounting on `StartupSequenceRunResult`
- Sixteen Runtime Play Mode policy-application tests
- Sixteen Runtime Play Mode runner policy and exception tests

#### FL-M3-01 - Startup Sequence Runner Skeleton and Immediate Step Execution

- Internal runtime-only `StartupStepExecution`
- `NotStarted -> Running -> terminal` attempt-state path
- Active progress capture through `IStartupStepProgressReporter`
- Single terminal-result capture
- Immutable `StartupSequenceRunResult`
- Authored entry, disabled entry, and attempted execution counts
- Ordered indexed access to completed attempts
- Warning, failure, and blocking-failure summary flags
- Internal `StartupSequenceRunner`
- Enabled-entry traversal in authored order
- Disabled-entry skipping before executor creation
- Fresh executor creation for every enabled attempt
- Immutable `StartupStepContext` delivery
- Cooperative cancellation-token pass-through
- Immediate `Awaitable<StartupStepResult>` execution and result capture
- Twelve Runtime Play Mode execution-state tests
- Eighteen Runtime Play Mode immediate-runner tests

#### FL-M2-08 - Startup Step Policy and Executor Contract

- MVP `StartupStepFailureAction` vocabulary
  - `BlockLaunch`
  - `ContinueWithWarning`
- Immutable authored `StartupStepPolicy`
- Safe `RequiredBlocking` and `OptionalWarning` policy presets
- Required/optional intent
- Failure-action metadata
- Optional timeout metadata
- Cooperative-cancellation capability metadata
- Invalid policy detection without runtime repair
- Immutable determinate and indeterminate `StartupStepProgress`
- Package-owned `IStartupStepProgressReporter`
- Validated immutable `StartupStepContext`
- Public `IStartupStepExecutor`
- Unity `Awaitable<StartupStepResult>` executor method contract
- Fresh-executor factory on `StartupStepDefinition`
- Authored policy on every `StartupSequenceEntry`
- Twenty-eight Runtime Play Mode policy and executor-contract tests

#### FL-M2-07 - Startup Sequence Definition and Ordered Entry Model

- Abstract immutable `StartupStepDefinition`
- Stable step identity and step-definition schema version `1`
- Authored step display label separated from stable identity
- Serializable `StartupSequenceEntry`
- Stable entry identity
- Enabled/disabled authored entry state
- One immutable step-definition reference per entry
- Project-owned `StartupSequence` ScriptableObject
- Stable sequence identity and sequence schema version `1`
- Ordered embedded sequence-entry list
- Read-only entry count and indexed entry access
- Passive `StartupSequence` binding on `EchoLaunchConfiguration`
- Configuration schema advancement from `1` to `2`
- Twenty-four Runtime Play Mode startup-sequence definition tests
- Create menu path under `EchoDevGames/First Light/Startup Sequence`

#### FL-M2-06 - Launch Configuration Identity and Root Binding

- Project-owned `EchoLaunchConfiguration` ScriptableObject
- Canonical lowercase 32-character hexadecimal configuration identity
- Serialized configuration schema version `1`
- Internal identity and schema support checks
- Passive serialized configuration binding on `EchoLaunchRoot`
- Read-only authority-filtered `EchoLaunchRoot.Configuration`
- Fifteen Runtime Play Mode configuration-binding tests
- Create menu path under `EchoDevGames/First Light/Launch Configuration`

#### FL-M2-05 - Lifecycle Notifications

- Public `LaunchStateChanged` observer event
- Public `LaunchProgressChanged` observer event
- Previous/current state and progress payloads
- State notification before progress notification
- Per-listener exception containment
- Stable listener-failure diagnostic `ELAUNCH-EVENT-001`
- Notification cleanup when the authoritative root is destroyed
- Twenty Runtime Play Mode notification tests

#### FL-M2-04 - Launch Lifecycle Transition Guard

- Internal `LaunchStateTransitionRules`
- Approved lifecycle transition matrix
- Same-state progress publication for active states
- Failure and interruption paths from active states
- Rejection of backward transitions
- Rejection of skipped lifecycle phases
- Permanent freezing of `Completed`, `Failed`, and `Interrupted`
- Transactional `LaunchSession.Publish` behavior
- Twenty-two Runtime Play Mode lifecycle transition cases
- Lifecycle-aligned maintenance of the existing session test suite

#### FL-M2-03 - Launch Session and Read-Only Progress Surface

- Internal `LaunchSession`
- One fresh session per authoritative root
- `LaunchProgressSnapshot.Empty`
- Public read-only root state and progress
- Fourteen Runtime Play Mode session and progress tests

#### FL-M2-02 - Neutral Launch-State Vocabulary

- Launch-mode, lifecycle, step-status, result, and snapshot vocabulary
- Thirty-nine Runtime Play Mode vocabulary tests

#### FL-M2-01 - Authority Claim and Static Reset Core

- Single launch authority
- Duplicate rejection
- Stable diagnostic code `ELAUNCH-ROOT-001`
- Seven Runtime Play Mode authority tests

### Changed

- `StartupSequenceRunner` now supports default and injected `ILaunchClock` construction.
- Every enabled attempt receives a linked per-attempt cancellation token.
- Positive timeout metadata now establishes one monotonic unscaled deadline.
- Timeout zero remains disabled.
- Executor completion observable before deadline evaluation wins the boundary race.
- The first observed deadline crossing remains authoritative over later success or failure.
- Timed-out executors settle before the runner evaluates continuation or creates a later executor.
- `ContinueWithWarning` converts timed-out source results to warnings after executor settlement.
- `BlockLaunch` converts timed-out source results to blocking failures and leaves later entries unvisited.
- `StartupStepExecution` now captures one immutable timing snapshot with terminal completion.
- Retained immediate-runner cancellation tests now assert a distinct linked token rather than caller-token identity.

- `StartupSequenceRunner` now applies authored `StartupStepFailureAction` to failure-like terminal results.
- `ContinueWithWarning` converts recoverable, blocking, and timed-out results to warnings and continues traversal.
- `BlockLaunch` converts failure-like results to blocking failures and stops before any later executor factory is called.
- `StartupStepExecution` can now capture a blocking factory or contract failure before execution begins.
- `StartupSequenceRunResult` now accounts for attempted, disabled, and unvisited entries and records the stopping authored index.
- Retained immediate-runner tests now assert blocking traversal stops rather than continuation.
- The intentional synchronous test executor suppresses compiler warning `CS1998` locally.

- `StartupStepDefinition` now requires `CreateExecutor()` to return a fresh single-use runtime executor.
- `StartupSequenceEntry` now serializes one `StartupStepPolicy`.
- `StartupSequence.CurrentSchemaVersion` advanced from `1` to `2` because the embedded entry shape now includes policy data.
- Entry activation, policy requirement, and cancellation support use safe zero-valued serialized enums so Unity-created list elements default to:
  - enabled;
  - required;
  - block launch;
  - no timeout;
  - cancellation supported.
- Existing startup-sequence definition tests now use a test-only executor factory without invoking an executor.

### Fixed

- Adapted the timeout test helper to Unity `6000.3.8f1`, where `AwaitableCompletionSource<T>.SetResult` accepts the result by value.
- Realigned the retained immediate-runner fixture after a stale test artifact temporarily restored three pre-FL-M3-02 expectations.
- Preserved FL-M3-02 policy-aware retained behavior while adding the FL-M3-03 linked-token expectation.
- Kept the full Unity compilation result at zero errors and zero warnings.

### Tested

Runtime Play Mode totals:

- Passed: `263`
- Failed: `0`
- Ignored: `0`

FL-M3-03 coverage:

- Approved `ILaunchClock` interface shape
- Default Unity clock interface implementation
- Finite nonnegative Unity clock values
- Deterministic manual clock advancement
- Timing validation for non-finite, negative, and backward values
- Derived elapsed-time calculation
- Disabled and reached timeout timing states
- Open progress forwarding
- Closed late-progress containment
- Idempotent progress-gate closure
- Single execution-timing assignment
- Zero-timeout delayed completion
- Completion before deadline
- Completion observable at the exact deadline
- Deadline crossing and timeout authority
- Stable `ELAUNCH-STEP-003`
- Timeout diagnostic details
- Supported cancellation request exactly once
- Unsupported timeout without cancellation request
- Late success containment
- Late failure containment
- Timeout-triggered cancellation exception containment
- Caller cancellation escape after executor settlement
- Continue-with-warning timeout traversal
- Block-launch timeout traversal stop
- Late-progress containment
- Backward-clock blocking contract result
- Authored asset immutability
- Later factory creation only after timed-out executor settlement
- Zero compiler errors and zero compiler warnings
- Expected retained diagnostics `ELAUNCH-ROOT-001` and `ELAUNCH-EVENT-001`

### Not Included

- Automatic retry
- Retry count or backoff
- Interactive retry
- Retry or skip presentation
- Structured caller-cancellation run result
- Root-level cancellation command
- Shutdown or destruction cancellation orchestration
- `EchoLaunchRoot` runner integration
- Automatic startup from Unity scene callbacks
- Launch-session lifecycle advancement
- Public step lifecycle events
- Launch reports
- Warning aggregation outside the run result
- Configuration or sequence preflight
- Duplicate-ID collision scans
- Dependency validation
- Runner re-entry protection
- Production-shaped multi-frame executor proof
- Splash presentation
- Scene loading
- Persistent root lifetime
- Direct-scene initializer behavior
- Custom inspectors or setup windows
- Standalone Laboratory
- Peer-package bridges

## [0.1.0] - 2026-08-04

### Added

- Initial Unity Package Manager manifest
- Embedded package registration
- Runtime, Editor, Runtime-test, and Editor-test assembly boundaries
- Initial package documentation shell
