# First Light - Startup and Launch

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

It coordinates ordered application initialization and final handoff without owning the internal behavior of peer packages.

## Package Status

- Package version: `0.1.0`
- Development stage: Startup-sequence preflight and runner re-entry protection implemented; root-owned lifecycle integration pending
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
  - `FL-M3-03` Monotonic Timeout Clock and Cooperative Cancellation
  - `FL-M3-04` Multi-Frame Async Proof and Runner Cancellation Outcome
  - `FL-M3-05` Runner Re-entry Protection and Sequence Preflight Boundary
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
- Single immutable timing capture
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

### Launch Clock and Timing

- Public `ILaunchClock`
- Internal shared `UnityLaunchClock`
- `Time.realtimeSinceStartupAsDouble`
- `Awaitable.NextFrameAsync`
- Injected deterministic test clocks
- Immutable `StartupStepTiming`
- Finite, nonnegative, monotonic clock validation
- Derived elapsed duration
- Runtime-only timing state

### Timeout and Cooperative Cancellation

- Absolute per-attempt deadlines
- Timeout zero disabled
- Deterministic completion-before-deadline race
- Stable `ELAUNCH-STEP-003`
- Timeout detail capture
- Linked caller and timeout cancellation tokens
- Cancellation requests only for supporting steps
- Timed-out executor settlement before traversal
- Late executor-result containment
- Late progress containment
- Backward-clock blocking through `ELAUNCH-STEP-004`

### Policy-Aware Timed Sequence Runner

- Internal `StartupSequenceRunner`
- Default or injected monotonic clock
- Explicit invocation only
- Disabled entries skipped before factory creation
- Fresh executor for every enabled attempt
- Authored-order traversal
- Immutable context delivery
- Linked per-attempt cancellation token
- Immediate and multi-tick progress capture
- Effective terminal-result and timing capture
- Blocking traversal stops before later factory creation
- Timed-out executor settles before later traversal
- Immutable `StartupSequenceRunResult`
- Attempted, disabled, and unvisited accounting
- Stopping authored-index capture
- Structured caller cancellation after executor settlement
- Stable `ELAUNCH-STEP-005`
- Run-level `WasCancelled`
- Same-tick cancellation-race containment

### Startup-Sequence Preflight

- Complete authored-data validation before executor creation
- Configuration and sequence identity/schema checks
- Null-entry and enabled-missing-definition rejection
- Entry identity, activation, and duplicate-ID checks
- Referenced step identity, schema, and duplicate-ID checks
- Stable preflight diagnostics:
  - `ELAUNCH-CFG-001`
  - `ELAUNCH-SEQ-001`
  - `ELAUNCH-STEP-001`
  - `ELAUNCH-STEP-002`
- No executor factory calls during preflight
- No asset repair, migration, or mutation
- Empty-sequence compatibility
- Disabled-entry-without-definition compatibility

### Runner Re-entry Protection

- One active traversal per runner instance
- Atomic acquisition through `Interlocked.CompareExchange`
- Stable concurrent re-entry diagnostic `ELAUNCH-RUN-001`
- Rejection before a second factory can run
- Gate release through `finally`
- Sequential runner reuse after success, cancellation, blocking traversal, or preflight rejection

### Multi-Frame Async Proof

- Production-shaped executor using `Awaitable.NextFrameAsync`
- Execution across multiple rendered Unity frames
- Progress accepted while the attempt is active
- Positive monotonic elapsed timing
- Authored traversal order preserved after settlement
- No scene, prefab, root, or automatic startup dependency

### Structured Caller Cancellation

- Caller cancellation reaches the linked executor token
- Active executor settles before the runner returns
- Attempt completes with `StartupStepStatus.Cancelled`
- Stable diagnostic `ELAUNCH-STEP-005`
- `StartupSequenceRunResult.WasCancelled`
- Authored warning policy cannot downgrade cancellation
- Later entries remain unvisited
- Later executor factories are not called

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

- Passed: `288`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Clock, timing, and progress-gate tests: `14`
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
- Timeout runner and cancellation tests: `18`
- Multi-frame async runner tests: `2`
- Preflight and re-entry tests: `23`

Compilation:

- Errors: `0`
- Warnings: `0`

Expected yellow diagnostic evidence:

- `ELAUNCH-ROOT-001` from duplicate-root tests
- `ELAUNCH-EVENT-001` from broken-listener containment tests

Timeout and cancellation evidence:

- Zero timeout remains disabled.
- Completion observable at the deadline wins.
- First observed deadline crossing creates `ELAUNCH-STEP-003`.
- Timeout details preserve configured timeout, elapsed time, and cancellation request.
- Supporting steps receive one timeout cancellation request.
- Unsupported steps are allowed to settle naturally.
- Timed-out executors settle before later factories are created.
- Late success, failure, and progress cannot replace the timeout boundary.
- Continue-with-warning and block-launch policies apply after settlement.
- Caller cancellation remains distinct and returns `ELAUNCH-STEP-005` only after active work settles.
- A same-tick executor cancellation exception is contained when the caller token is already requested.
- Multi-frame execution preserves progress, positive timing, and authored order.
- Backward clocks become blocking timing-contract results.
- Definitions, entries, policies, sequences, and configurations remain unchanged.

No production asset, scene, prefab, root, or automatic startup setup was required.

## Not Implemented Yet

First Light does not yet provide:

- Automatic retry
- Retry count or backoff
- Interactive retry
- Retry or skip UI
- Root-level cancellation command
- Shutdown or destruction cancellation orchestration
- `EchoLaunchRoot` runner integration
- Automatic startup from Unity scene callbacks
- Launch-session lifecycle advancement
- Public step lifecycle events
- Launch reports
- Warning aggregation outside the run result
- Dependency validation
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
- Two hundred eighty-eight passing Runtime Play Mode tests
- Safe policy authoring verification
- Fresh executor factory contract
- Policy-aware timed startup execution with complete preflight and runner re-entry protection, but no root or lifecycle integration

Still `Not run`:

- Git URL installation
- Tarball installation
- Separate clean-project installation
- Player builds
- Production startup integration
- Root cancellation orchestration
- Performance measurements

## License

See [LICENSE.md](LICENSE.md).

## Third-Party Notices

See [Third Party Notices.md](Third%20Party%20Notices.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).
