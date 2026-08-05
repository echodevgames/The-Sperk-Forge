# Changelog

All notable changes to First Light - Startup and Launch will be documented in this file.

The package follows Semantic Versioning once public compatibility commitments begin.

## [Unreleased]

### Added

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

- Corrected Unity Inspector defaults for newly added embedded sequence entries. Unity can create list elements from zeroed serialized data without applying C# field initializers; the serialized model now makes that zero state intentionally safe without adding automatic repair callbacks.

### Tested

Runtime Play Mode totals:

- Passed: `199`
- Failed: `0`
- Ignored: `0`

FL-M3-01 coverage:

- Runtime execution metadata copying
- Initial `NotStarted` state
- Single legal transition to `Running`
- Progress rejection before begin and after completion
- Progress capture while running
- Null and repeated completion rejection
- Terminal status and result capture
- Invalid execution construction rejection
- Null configuration rejection
- Missing sequence rejection
- Empty-sequence result
- Disabled-entry skipping before factory creation
- One executor and one invocation per enabled entry
- Fresh executors across repeated runs
- Context configuration, sequence, entry, and step identities
- Authored index and complete authored entry count
- Cancellation-token pass-through
- Immediate progress capture
- Success, warning, recoverable failure, and blocking failure preservation
- Authored execution order
- Continued traversal after a blocking result
- Null executor rejection
- Definition, entry, policy, sequence, and configuration immutability
- Expected retained diagnostics `ELAUNCH-ROOT-001` and `ELAUNCH-EVENT-001`

### Not Included

- `EchoLaunchRoot` runner integration
- Automatic startup from Unity scene callbacks
- Launch-session lifecycle advancement
- Step lifecycle public events
- Exception-to-result conversion
- Result-to-policy interpretation
- Blocking-result traversal stop
- Warning aggregation into a launch report
- Timeout measurement
- Clock abstraction
- Timeout cancellation
- Retry loops
- Interactive retry
- Configuration or sequence preflight
- Duplicate-ID collision scans
- Runner re-entry protection
- Asynchronous multi-frame proof
- Launch reports
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
