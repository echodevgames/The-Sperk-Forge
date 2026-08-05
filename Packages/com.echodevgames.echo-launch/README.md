# First Light - Startup and Launch

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

It coordinates ordered application initialization and final handoff without owning the internal behavior of peer packages.

## Package Status

- Package version: `0.1.0`
- Development stage: Immediate execution skeleton implemented; policy and lifecycle integration pending
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
- `NotStarted -> Running -> terminal` attempt path
- Progress accepted only while running
- Single terminal-result capture
- Copied authored identity, position, policy, and label metadata
- No authored asset mutation

### Immediate Sequence Runner

- Internal `StartupSequenceRunner`
- Explicit invocation only
- Disabled entries skipped before factory creation
- Fresh executor for every enabled attempt
- Authored-order traversal
- Immutable context delivery
- Cancellation-token pass-through
- Immediate progress capture
- Immediate terminal-result capture
- Immutable `StartupSequenceRunResult`
- Blocking results recorded without stopping traversal

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

- Passed: `199`
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

Expected yellow diagnostic evidence:

- `ELAUNCH-ROOT-001` from duplicate-root tests
- `ELAUNCH-EVENT-001` from broken-listener containment tests

Immediate execution evidence:

- Disabled entries create no executor.
- Enabled entries create fresh executors.
- Enabled entries execute in authored order.
- Context identities, authored index, complete count, cancellation, and progress are preserved.
- Success, warning, recoverable failure, and blocking failure results are captured exactly.
- Traversal continues after blocking results because policy application is not implemented yet.
- Definitions, entries, policies, sequence assets, and configuration assets remain unchanged.

No production asset, scene, prefab, root, or automatic startup setup was required.

## Not Implemented Yet

First Light does not yet provide:

- `EchoLaunchRoot` runner integration
- Automatic startup from Unity scene callbacks
- Launch-session lifecycle advancement
- Public step lifecycle events
- Exception-to-result conversion
- Result-to-policy interpretation
- Blocking-result traversal stop
- Warning aggregation
- Timeout measurement
- Clock abstraction
- Timeout cancellation
- Retry loops
- Interactive retry
- Configuration or sequence preflight
- Duplicate-ID collision validation
- Runner re-entry protection
- Multi-frame asynchronous proof
- Launch reports
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
- One hundred ninety-nine passing Runtime Play Mode tests
- Safe policy authoring verification
- Fresh executor factory contract
- Explicit immediate startup execution with no root or lifecycle integration

Still `Not run`:

- Git URL installation
- Tarball installation
- Separate clean-project installation
- Player builds
- Production startup integration
- Multi-frame asynchronous execution
- Timeout behavior
- Performance measurements

## License

See [LICENSE.md](LICENSE.md).

## Third-Party Notices

See [Third Party Notices.md](Third%20Party%20Notices.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).
