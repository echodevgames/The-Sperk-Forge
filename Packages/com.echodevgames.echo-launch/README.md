# First Light - Startup and Launch

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

It coordinates ordered application initialization and final handoff without owning the internal behavior of peer packages.

## Package Status

- Package version: `0.1.0`
- Development stage: Policy-aware immediate execution implemented; timeout and lifecycle integration pending
- Completed runtime slices:
  - `FL-M2-01` Authority Claim and Static Reset Core
  - `FL-M2-02` Neutral Launch-State Vocabulary
  - `FL-M2-03` Launch Session and Read-Only Progress Surface
  - `FL-M2-04` Launch Lifecycle Transition Guard
  - `FL-M2-05` Lifecycle Notifications
  - `FL-M2-06` Launch Configuration Identity and Root Binding
  - `FL-M2-07` Startup Sequence Definition and Ordered Entry Model
  - `FL-M2-08` Startup Step Policy and Executor Contract
  - `FL-M3-01` Startup Sequence Runner Skeleton and Immediate Step Execution
  - `FL-M3-02` Step Result Policy Application and Exception Conversion
- Unity baseline: `6000.3.8f1`
- Minimum declared Unity version: `6000.0`
- uGUI dependency: `2.0.0`

## Implemented Runtime Scope

First Light now provides:

### Authority Core

- One process-wide launch-authority claim
- Immediate duplicate rejection
- Stable duplicate diagnostic code `ELAUNCH-ROOT-001`
- Owner-only authority release
- Static reset through subsystem registration

### Launch-State Vocabulary

- `LaunchMode`
- `LaunchStatus`
- `StartupStepStatus`
- Immutable `StartupStepResult`
- Immutable `LaunchProgressSnapshot`

### Launch Session and Progress

- One fresh `LaunchSession` per authoritative root
- Initial `AuthorityClaimed` state
- Read-only root state and progress
- Controlled internal progress publication
- Duplicate and stale-root state hiding

### Lifecycle Transition Guard

- Centralized transition rules
- Approved forward lifecycle path
- Same-state progress publication for active states
- Failure and interruption from active states
- Rejection of backward and skipped transitions
- Permanent terminal-state freezing
- Transactional publication

### Lifecycle Notifications

- Public state and progress observer events
- Previous/current payloads
- State-before-progress order
- Accepted state visible during callbacks
- Per-listener exception containment
- Stable listener diagnostic `ELAUNCH-EVENT-001`
- Delegate cleanup on root destruction

### Launch Configuration

- Project-owned `EchoLaunchConfiguration`
- Stable configuration ID
- Configuration schema version `2`
- Passive startup-sequence reference
- Authority-filtered root binding
- Invalid identity and schema detection without runtime repair

### Startup Definitions and Sequence

- Abstract immutable `StartupStepDefinition`
- Stable step identity and schema
- Display label separate from identity
- Serializable `StartupSequenceEntry`
- Stable entry identity
- Safe activation metadata
- Project-owned `StartupSequence`
- Sequence schema version `2`
- Ordered private entry list
- Read-only count and indexed access
- Passive configuration binding

### Startup Step Policy

- Exact MVP failure actions:
  - `BlockLaunch`
  - `ContinueWithWarning`
- Required and optional intent
- Timeout metadata
- Cancellation capability metadata
- Safe presets:
  - `RequiredBlocking`
  - `OptionalWarning`
- Invalid policy detection without clamping or repair
- Safe zero-state Unity serialization defaults

### Startup Step Progress

- Immutable determinate progress
- Immutable indeterminate progress
- Inclusive `0` through `1` range
- Invalid range rejection
- Normalized messages

### Startup Step Context

- Immutable launch mode and stable identities
- Step index and count
- Cooperative `CancellationToken`
- Package-owned progress reporter
- Constructor validation
- No launch authority

### Executor Contract

- Public `IStartupStepExecutor`
- Unity `Awaitable<StartupStepResult>`
- Fresh executor factory on every step definition
- Single-use executor intent
- Active state kept outside ScriptableObject definitions

### Runtime Step Execution

- Internal runtime-only `StartupStepExecution`
- Metadata creation before factory success
- One fresh executor attachment
- `NotStarted -> Running -> terminal` normal attempt path
- `NotStarted -> BlockingFailure` factory-contract path
- Progress accepted only while running
- Single terminal-result capture
- Copied authored identity, position, policy, and label metadata
- No authored asset mutation

### Policy Application

- Immutable `StartupStepPolicyDecision`
- Internal `StartupStepPolicyEvaluator`
- Success, warning, and skipped preserve and continue
- Cancelled preserves and stops
- `ContinueWithWarning` converts failure-like results to warnings
- `BlockLaunch` converts failure-like results to blocking failures
- Code, message, and details preservation
- Explicit failure action remains authoritative

### Exception Conversion

- Stable `ELAUNCH-STEP-004`
- Factory exception containment
- Null executor containment
- Executor exception conversion before policy
- Null result containment
- Sanitized exception type and message
- No stack trace copying
- `OperationCanceledException` excluded from generic conversion

### Policy-Aware Immediate Sequence Runner

- Internal `StartupSequenceRunner`
- Explicit invocation only
- Disabled entries skipped before factory creation
- Fresh executor for every enabled attempt
- Authored-order traversal
- Immutable context delivery
- Cancellation-token pass-through
- Immediate progress capture
- Effective terminal-result capture
- Blocking traversal stops before later factory creation
- Immutable `StartupSequenceRunResult`
- Attempted, disabled, and unvisited accounting
- Stopping authored-index capture

## Safe Serialized Entry Defaults

Unity can create new embedded list elements from zeroed serialized data.

First Light maps zero to safe authored defaults:

```text
Activation: Enabled
Requirement: Required
Failure Action: Block Launch
Timeout Seconds: 0
Cancellation: Supported
```

No automatic repair or migration callback is used.

## Approved Lifecycle

    None
        -> AuthorityClaimed
            -> Validating
                -> Running
                    -> Transitioning
                        -> Completed

Active states may also enter:

    Failed
    Interrupted

`Completed`, `Failed`, and `Interrupted` are terminal.

## Verified Behavior

The Runtime Play Mode suite reports:

- Passed: `231`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Launch configuration binding tests: `15`
- Launch-state vocabulary tests: `39`
- Launch session and progress tests: `14`
- Lifecycle transition tests: `22`
- Lifecycle notification tests: `20`
- Startup sequence definition tests: `24`
- Startup step policy and executor-contract tests: `28`
- Startup step execution tests: `12`
- Immediate startup sequence runner tests: `18`
- Policy-application tests: `16`
- Runner policy and exception tests: `16`

Compilation:

- Errors: `0`
- Warnings: `0`

Expected yellow diagnostic evidence:

- `ELAUNCH-ROOT-001` from duplicate-root tests
- `ELAUNCH-EVENT-001` from broken-listener containment tests

Policy and exception evidence:

- Continue-with-warning converts failure-like results and continues.
- Block-launch converts failure-like results and stops.
- Cancelled results remain cancelled and stop.
- Factory and executor failures become structured `ELAUNCH-STEP-004` results.
- Null executors and null terminal results become blocking contract results.
- Later factories are never called after a stop.
- Attempted, disabled, and unvisited counts balance against the authored count.
- Stopping authored index is preserved.
- Definitions, entries, policies, sequences, and configurations remain unchanged.

No production asset, scene, prefab, root, or automatic startup setup was required.

## Not Implemented Yet

First Light does not yet provide:

- Timeout measurement
- `ILaunchClock`
- Timeout race
- Timeout cancellation
- Retry loops
- Retry backoff
- Interactive retry
- Cancellation orchestration
- `EchoLaunchRoot` runner integration
- Automatic startup from Unity scene callbacks
- Launch-session lifecycle advancement
- Public step lifecycle events
- Launch reports
- Warning aggregation outside the run result
- Configuration or sequence preflight
- Duplicate-ID collision validation
- Runner re-entry protection
- Multi-frame asynchronous proof
- Splash presentation
- Scene loading
- Persistent-root lifetime policy
- Direct-scene initialization behavior
- Custom inspectors or setup windows
- Standalone Laboratory
- Peer-package bridges

## Documentation

Package documentation lives under `Documentation~`.

The suite-wide architecture and approved First Light specification live in the repository's `Plan Documentation` vault.

## Evidence Status

Available evidence:

- Embedded package recognition
- Clean Unity compilation
- Unity restart
- Embedded-package removal and reinstallation
- Stable assembly-definition GUIDs
- Two hundred thirty-one passing Runtime Play Mode tests
- Safe policy authoring verification
- Fresh executor factory contract
- Policy-aware immediate startup execution with no root or lifecycle integration

Still `Not run`:

- Git URL installation
- Tarball installation
- Separate clean-project installation
- Player builds
- Production startup integration
- Multi-frame asynchronous execution
- Timeout behavior
- Cancellation orchestration
- Performance measurements

## License

See [LICENSE.md](LICENSE.md).

## Third-Party Notices

See [Third Party Notices.md](Third%20Party%20Notices.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).
