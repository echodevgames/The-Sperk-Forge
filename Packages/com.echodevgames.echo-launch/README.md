# First Light - Startup and Launch

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

It coordinates ordered application initialization and final handoff without owning the internal behavior of peer packages.

## Package Status

- Package version: `0.1.0`
- Development stage: FL-M6-01 Production Reference Showcase complete; Setup/Repair/Validator/Direct Scene/Simulator/Laboratory plus A1 splash authoring and A1-E1 project-owned foundation resolution are implemented; in-repository First Light Gallery is complete/frozen; versioned `0.1.0` Distribution Kit is prepared as the handoff artifact while external tarball qualification remains pending
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
  - `FL-M3-06` Root-Owned Startup Run and Lifecycle Advancement
  - `FL-M3-07` Immutable Launch Report and Public Terminal Events
  - `FL-M3-08` Initial Destination Contract, Load Result, and Completed Handoff
  - `FL-M4-01` Automatic Root Start Gate and Plain Status Presenter Contract
  - `FL-M4-02` Default uGUI Plain Status View and Presentation Assembly
  - `FL-M4-03` Image Splash Definitions and Deterministic Splash Player
  - `FL-M4-04` Splash Configuration Schema and Root Playback Integration
  - `FL-M4-05` Startup Presentation Prefab and Canvas Assembly
  - `FL-M5-01` Editor Setup Foundation and Non-Destructive Project Plan
  - `FL-M5-02` Approved Setup Apply Engine and Repeat-Safe Asset Creation
  - `FL-M5-03` Explicit Setup Repair and Existing-Asset Reconciliation
  - `FL-M5-04` Read-Only Validator and Project Health Report
  - `FL-M5-05` Direct Scene Development Initializer
  - `FL-M5-06` Launch Simulator and Deterministic Failure Injection
  - `FL-M5-07` Standalone Test Laboratory and Importable UPM Sample
  - `FL-M6-01` Production Reference Showcase, Splash Presentation/Authoring, and Project-Owned Foundation Resolution
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
- Configuration schema version `4`
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

### Root-Owned Explicit Startup

- Internal explicit `EchoLaunchRoot.StartLaunchAsync`
- No automatic call from `Awake`, `Start`, or scene callbacks
- One root-local active-launch gate
- Stable start-gate diagnostic `ELAUNCH-LIFE-002`
- Latest settled sequence result retained internally
- Duplicate and previously advanced roots rejected

### Root Lifecycle Projection

- Configuration validation publishes `Validating`
- Accepted sequence validation publishes `Running`
- Step start, progress, and completion update existing root snapshots
- Blocking or unexpected outcomes publish `Failed`
- Cancellation publishes `Interrupted`
- Successful and warning-only runs publish `Transitioning`
- Successful destination activation publishes `Completed`

### Root Cancellation and Destruction Safety

- Public cooperative `CancelLaunch(reason)`
- Blank reason normalization
- Repeated request rejection
- Executor settlement before interruption completes
- Stable interruption diagnostic `ELAUNCH-LIFE-001`
- Destruction-driven cancellation
- Late-publication suppression
- Event cleanup and authority release

### Structured Preflight and Legacy Compatibility

- Internal `StartupSequencePreflightException`
- Stable diagnostic code and failure message retained for root publication
- Internal `IStartupSequenceObserver`
- Internal `StartupStepProgressRelay`
- Legacy direct-runner calls preserve exact `InvalidOperationException`

### Immutable Launch Reports

- Public immutable `LaunchStepReport`
- Public immutable `LaunchReport`
- Report schema version `2`
- Producing package version `0.1.0`
- Copied identity, policy, progress, result, and timing values
- Attempted, disabled, and unvisited accounting
- Warning, failure, blocking-failure, and cancellation summaries
- Indexed read-only step access
- Defensive collection copying
- No authored asset mutation
- No durable-save integration

### Report Builder

- Internal root-owned `LaunchReportBuilder`
- Completed-step capture exactly once
- Authored-order preservation
- Single finalization guard
- Completed, failed, and interrupted report finalization
- Transition-pending successful data retention during destination loading

### Public Terminal Report Events

- Authority-filtered `LastReport`
- Public `LaunchCompleted`
- Public `LaunchFailed`
- Public `LaunchInterrupted`
- Root state accepted before report finalization
- `LastReport` assigned before event dispatch
- Exact event-payload identity
- Exactly-once matching event
- Per-listener exception isolation
- Duplicate-root silence
- Destruction-driven late-event suppression
- No terminal event before its matching lifecycle outcome

### Project-Owned Initial Destination

- Public immutable `LaunchDestination` ScriptableObject
- Stable canonical destination identity
- Destination schema version `1`
- User-facing display label
- Runtime-safe scene path
- Configuration schema version `3`
- Historical configuration schema `2` rejection without runtime rewrite
- Read-only authority-filtered initial destination exposure

### Initial Destination Loading

- Public `IInitialDestinationLoader`
- Immutable `InitialDestinationLoadResult`
- `Succeeded`, `Failed`, and `Cancelled` status vocabulary
- Internal normalized progress relay
- Standalone `UnityInitialDestinationLoader`
- Build-loadability validation
- Unity asynchronous single-scene loading
- Destination activation confirmation
- No ownership of normal mid-game scene travel

### Completed Handoff

- Destination validation before startup-step side effects
- Stable `ELAUNCH-DEST-001` and `ELAUNCH-DEST-002`
- Destination progress while state remains `Transitioning`
- Successful `Transitioning -> Completed` lifecycle
- Completed report schema version `2`
- Destination identity and display metadata in completed reports
- Public `LaunchCompleted`
- Completed state and report accepted before event dispatch
- Exact `LastReport` event payload identity
- Exactly-once completion publication
- Listener failure isolation
- Cancellation and destruction containment
- Startup warning preservation in immutable reports

### Automatic Root Startup

- Serialized automatic-start setting enabled by default
- Unity `Start` entry point
- Existing `StartLaunchAsync` gate reuse
- Manual-before-automatic one-run protection
- Duplicate-root automatic-start silence
- Internal deterministic opt-out for manual tests

### Neutral Status Presentation

- Public `ILaunchStatusPresenter`
- Bind, accepted-snapshot, terminal-report, and unbind callbacks
- Serialized neutral `MonoBehaviour` presenter seam
- Logging-free `NullLaunchStatusPresenter`
- Safe `LaunchStatusPresenterDispatcher`
- `ELAUNCH-VIEW-001` invalid-component fallback
- `ELAUNCH-VIEW-002` callback-failure containment
- Accepted snapshots presented before public progress events
- Finalized report presented after `LastReport` assignment
- Exactly-once unbind during destruction
- No Runtime dependency on uGUI or TextMeshPro

### Default Plain uGUI Status View

- Separate `EchoDevGames.EchoLaunch.Presentation.UGUI` assembly
- Public `EchoLaunchStatusView`
- Neutral presenter implementation
- Serialized `CanvasGroup`, `Text`, `Slider`, and progress surfaces
- Text-complete lifecycle state copy
- Determinate slider progress and percentage
- Separate indeterminate progress surface
- Active-step position and stable step ID
- Elapsed-time readout
- Warning diagnostic rendering
- Completed destination and full progress
- Failed and interrupted diagnostic rendering
- Show-on-bind, hide-on-unbind, and clear-on-unbind
- Missing-reference-safe behavior
- Serialized replaceable copy
- No TextMeshPro dependency
- Neutral Runtime assembly remains uGUI-free

### Deterministic Image Splash Playback

- Project-owned `SplashSequence` schema 1
- Immutable image-only `SplashEntry`
- Stable sequence and entry identities
- Authored fade-in, hold, fade-out, and minimum-display time
- Allowed and disallowed skip policy
- Latched early skip requests
- Reduced-motion fade removal
- Deterministic `ILaunchClock` traversal
- Ordered multi-entry playback
- Normalized alpha
- Cancellation and re-entry containment
- Invalid/backward clock rejection
- Immutable frames and playback result
- Neutral `IImageSplashPresenter`
- Logging-free headless fallback
- Default uGUI sprite, label, alpha, and position
- Public `RequestSplashSkip()` with no EchoInput dependency
- Splash definition schema remains `1`
- Root integration is supplied by schema-4 configuration
- Report schema remains `2`

### Schema-4 Root Splash Integration

- Configuration schema version `4`
- Optional serialized `SplashSequence`
- Serialized reduced-motion default
- Historical schema 3 rejection without rewrite
- Null and empty splash no-op behavior
- Side-effect-free splash and startup preflight
- Sequential root order: splash, startup steps, destination
- Shared monotonic launch clock
- Visual or headless splash presenter resolution
- `ELAUNCH-SPLASH-001`
- `ELAUNCH-SPLASH-002`
- `ELAUNCH-SPLASH-003`
- Splash clear before startup-step presentation
- Root cancellation during splash
- Failure blocking later phases
- Successful splash-result retention
- Total report elapsed time including splash
- Duplicate-root splash silence
- Automatic-start and direct-scene splash routing
- Configuration and splash immutability
- Report schema version `2` preserved

### Neutral Startup Presentation Prefabs

The package ships two stable template assets:

```text
Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab
Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

The status prefab provides:

- Screen Space Overlay Canvas
- 1920 x 1080 scalable reference resolution
- Hidden non-interactive CanvasGroup
- Neutral backdrop and readable legacy uGUI text
- Splash image and label
- State, message, step, progress, and elapsed surfaces
- Complete serialized `EchoLaunchStatusView` references
- No input authority or project asset dependency

The root prefab provides:

- One `EchoLaunchRoot`
- Nested `EchoLaunchStatusView.prefab`
- Wired presenter reference
- Null project configuration
- Canonical Boot mode
- Automatic start enabled

Projects explicitly place, copy, variant, or replace these templates.

Runtime does not locate or instantiate them automatically.

## First Light Setup Apply

Open:

```text
Tools > Sperk's Forge > First Light > Setup
```

The window can inspect the project, select an existing destination scene,
generate a deterministic ordered plan, display diagnostics, copy the plan, and
apply one fresh executable plan.

FL-M5-02 Apply is deliberately create-only and reuse-only:

- Recollects project evidence and replans immediately before writes.
- Rejects stale or non-executable plans.
- Executes only `Create`, `Reuse`, and `NoChange` operations.
- Creates missing project-owned folders, definition assets, configuration,
  root prefab variant, and Boot scene in deterministic order.
- Never opens or modifies the selected destination scene.
- Preserves existing open, active, and dirty scene state.
- Applies the selected Build Settings policy last.
- Allows only one active Apply.
- Uses compensating rollback for changes made by the active attempt.
- Returns an immutable result with created, reused, Build Settings, rollback,
  and recovery evidence.
- Returns `NoChanges` on repeat Apply when the project already matches.

Default proposed root:

```text
Assets/EchoDevGames/FirstLight
```

Default Build Settings policy:

```text
AddIfMissingAtEnd
```

Moving Boot first requires explicit approval.

Apply does not repair, migrate, overwrite, move, rename, or delete existing
project-authored content.

### FL-M6-01 Setup Presentation and Foundation Authoring

Setup additionally supports creation-time authoring for a **new** SplashSequence:

- Splash Only / Splash + Status;
- project-owned background;
- Allow Advancement;
- per-entry image, optional audio intent, label, timing, advancement, and None/Pulse motion.

Setup never re-authors a reused SplashSequence.

Foundation resolution is explicit:

```text
Reuse Compatible Assets
Create Project-Owned Setup
```

`Reuse Compatible Assets` preserves the backward-compatible candidate-reuse behavior. `Create Project-Owned Setup` creates missing canonical Configuration, LaunchDestination, StartupSequence, optional SplashSequence, and RootPrefab targets beneath the requested Project Root instead of substituting compatible off-root candidates. Existing compatible requested targets remain authoritative; incompatible targets still block; the explicitly selected destination scene may be reused. The selected mode participates in request/plan fingerprints and stale-plan protection.

The permanent suite repository examples live under `Assets/EchoDevGames/SuiteShowcase/First Light Gallery/**`. They are project-owned examples, not package dependencies or defaults.

## First Light Setup Repair

`Repair Plan...` is a separate explicit transaction. It is available only when
the refreshed plan contains narrowly approved current-schema repair operations
and every required ownership and shape proof succeeds.

FL-M5-03 Repair:

- Keeps `Apply Plan...` create-only.
- Recollects project evidence, replans, and compares deterministic fingerprints
  before backup or writes.
- Shares one active-mutation gate with Apply.
- Requires explicit Repair confirmation.
- Reconciles only the approved configuration references, destination scene path,
  verified root-prefab configuration binding, zero-root canonical Boot scene,
  and canonical Boot Build Settings entry.
- Blocks unsupported schemas, invalid identities, ambiguous references, unsafe
  prefab lineage/root counts, and unsafe scene shapes.
- Backs up each existing project asset and matching `.meta` bytes beneath
  `Library/EchoDevGames/FirstLight/RepairBackups/<repair-id>` before mutation.
- Hash-verifies backup and restoration.
- Writes Build Settings last.
- Deletes successful temporary backups.
- Retains and reports backup paths when rollback cannot complete.
- Preserves stable IDs, Unity GUIDs, unrelated configuration values, prefab
  content, unrelated Boot-scene objects, the destination scene, and package
  templates.
- Returns `NoChanges` on repeat Repair after the project converges.

Repair does not migrate schemas, regenerate IDs, replace types, edit sequence or
splash contents, delete duplicate roots, restructure prefabs, clean arbitrary
scene content, move/rename/delete assets, or modify the selected destination
scene.

## First Light Validator

Open:

```text
Tools > Sperk's Forge > First Light > Validator
```

The Validator runs only after the user presses `Validate Project`. Opening,
repainting, importing, reloading, or entering Play Mode does not run it
automatically.

FL-M5-04 Validator:

- Inspects the canonical project-owned First Light root.
- Reads configuration, startup sequence, destination, optional splash, root
  prefab, Boot scene, enabled Build Settings scenes, and Build Settings entries.
- Opens closed scenes only for additive read-only inspection.
- Preserves the user's active, open, and dirty scene state.
- Never applies, repairs, migrates, saves, deletes, moves, renames, or changes
  Build Settings.
- Returns immutable schema-1 findings and one derived project-health result.
- Uses stable `ELAUNCH-VAL-001` through `ELAUNCH-VAL-015`.
- Activates `ELAUNCH-VAL-009` for Direct Scene structure, destination, and Development-Build policy safety.
- Produces deterministic request, evidence, and report fingerprints.
- Copies a deterministic project-relative plain-text report.
- Rejects absolute machine paths from finding output.
- Contains evidence failures through `ELAUNCH-VAL-014`.
- Rejects a second concurrent run through `ELAUNCH-VAL-015`.

Project health is derived from the highest finding severity:

```text
Blocker -> Blocked
Error   -> Invalid
Warning -> NeedsAttention
Info    -> Healthy
```

The Validator may recommend opening Setup, but it never invokes Apply or Repair.

## Direct Scene Development Entry

Add `EchoDirectSceneInitializer` to a gameplay or Test Lab scene and assign one
project-owned `DirectSceneConfiguration`.

Supported policies:

```text
EditorOnly
EditorAndDevelopmentBuilds
BootRequired
```

`EditorOnly` is the default. A non-development release player can never create a
direct root.

At Play, scene-authored roots claim first in `Awake`. The helper settles once in
`Start`, reuses existing authority, or creates one approved
`DirectSceneDevelopment` root. If the configured destination is already active,
startup completes without reloading the scene.

The helper uses the normal First Light root, splash, sequence, report,
destination, duplicate-authority, and lifetime behavior. It is not a second
bootstrap pipeline.

The Validator reports `ELAUNCH-VAL-009` for unsafe Direct Scene authoring.
A valid `EditorOnly` helper remains Healthy. Explicit Development-Build opt-in
returns `NeedsAttention` as a visible warning.

## Launch Simulator

Open:

```text
Tools > Sperk's Forge > First Light > Simulator
```

The Simulator is an explicit Editor-only diagnostic tool. Opening the window
does not start a run.

Built-in presets:

```text
ImmediateSuccess
TimedProgressSuccess
WarningContinues
RecoverableFailureContinues
BlockingFailureStops
TimeoutStops
ExecutorExceptionStops
Cancellation
```

Each accepted request builds transient `HideAndDontSave` configuration and
startup-sequence objects, executes the real First Light sequence runner, copies
one immutable schema-1 simulation report, and destroys the transient objects.

The Simulator does not:

- edit project-authored configurations or sequences
- create persistent assets
- add scene objects
- change Build Settings
- claim a launch root
- play splash/status presentation
- load a destination
- run in player builds

Use **Copy Report** to capture deterministic request, plan, report, step, and
progress evidence.

Expected simulated warning and failure results appear only inside the report.
They do not create Unity Console warnings or errors.

## Standalone Test Laboratory

First Light ships exactly one separately importable Unity Package Manager sample:

```text
First Light Standalone Test Lab
```

Import it through the package's **Samples** section in Unity Package Manager.

The sample contains:

- a canonical Boot Laboratory scene;
- a destination Laboratory scene;
- authored success, timed-progress, warning, recoverable-failure, blocking-failure,
  and invalid-destination scenarios;
- a Laboratory root prefab and duplicate-root fixture;
- a Direct Scene configuration;
- redistributable placeholder splash art;
- sample-only runtime step/readout helpers;
- a sample-only LAB-010 splash skip-request control routed through the existing
  `EchoLaunchStatusView.RequestSplashSkip()` API.

The sample does not automatically run Setup, Repair, Validator, Simulator, Play
Mode, or modify Build Settings when imported.

Standard imported `Assets/Samples/**` content is excluded from automatic First
Light Setup candidate discovery. Explicit user selection of imported sample
assets remains supported.

The Laboratory splash minimum is five seconds so the manual early-skip
acceptance proof is human-observable. This changes Laboratory evidence timing
only and does not change production splash or input ownership.

Removing the imported sample leaves the package Runtime and Editor tooling
healthy.

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

Retained FL-M5-07 complete regression evidence:

```text
Complete EditMode:     306 / 306
Runtime Play Mode:     503 / 503
Total automated:       809 / 809
Manual Laboratory:      12 / 12
```

FL-M6-01 additive evidence:

```text
H1 focused identity gate:              5 / 5
H2 focused destination gate:          35 / 35
Final EchoLaunchSetup filtered gate: 224 / 224
```

Focused A1 data/player/uGUI/Inspector/Setup slices were green during implementation. This closeout does **not** claim a new complete post-A1 EditMode or Runtime Play Mode aggregate. A fresh complete regression belongs to future release qualification.

Reference Gallery acceptance additionally proved:

- canonical First Light Boot → ordered splashes → MainMenu handoff;
- independent UMBRA foundation creation through Create Project-Owned Setup;
- three generated stable-ID splash entries with project-owned art/audio intent/timing/motion/advancement;
- successful UMBRA runtime presentation;
- identical second Apply `NoChanges`, no created paths, and unchanged Build Settings.

Manual FL-M5-03 acceptance created the project-owned First Light foundation,
introduced only authorized current-schema drift, repaired five approved
surfaces, preserved unrelated content and identities, proved two no-op Repair
reruns, and removed generated acceptance and backup residue.

Manual FL-M5-04 acceptance then proved deterministic `Healthy` reporting,
introduced root-binding, duplicate-root, and Boot Build Settings faults, observed
`Blocked` with `ELAUNCH-VAL-002`, path-specific `ELAUNCH-VAL-003`, and
`ELAUNCH-VAL-008`, restored the project explicitly, and reproduced the exact
original healthy request, evidence, and report fingerprints.

Manual FL-M5-05 acceptance proved direct-play root creation without scene
reload, existing-authority reuse, two-initializer convergence on one authority,
zero-error startup completion, `ELAUNCH-VAL-009` warning for explicit
Development-Build opt-in, and exact restored `EditorOnly` healthy fingerprints.

Manual FL-M5-06 acceptance proved all eight Simulator presets, ordered logical
progress, warning and recoverable-failure continuation, blocking/timeout/exception
traversal stops, cooperative cancellation, clean Console behavior, and exact
repeatable cancellation fingerprints after deterministic report normalization.
Manual FL-M5-07 acceptance proved all twelve approved Laboratory cases,
including canonical Boot handoff, timed progress, warning continuation,
configuration and destination preflight blocking, authored blocking failure,
duplicate-root rejection, Direct Scene creation/reuse, minimum-duration splash
skip gating, sample removal, and three-run Setup/Repair repeatability. Imported
sample acceptance also exposed and resolved the bounded automatic
`Assets/Samples/**` candidate-isolation defect and a null Boot-scene
configuration override.

## Not Implemented Yet

First Light does not yet provide:

- Automatic retry
- Retry count or backoff
- Interactive retry
- Retry or skip UI
- Public step lifecycle events
- Warning aggregation outside the run result
- Dependency validation
- Editor migration from historical configuration schemas
- Automatic Direct Scene helper installation
- Direct Scene build hooks or automatic build blocking
- Persistent-root lifetime policy
- Peer-package bridges

## Official Distribution Kit

The repository-owned First Light distribution snapshot lives at:

```text
Distributions/First Light/0.1.0/
```

It contains `com.echodevgames.echo-launch-0.1.0.tgz`, a complete user handout, distribution manifest, SHA-256 integrity record, and build record.

The tarball is an official handoff/evaluation artifact. It is **not yet a release-qualified tarball route**; external clean-project install, removal/reinstall, player-build, performance, tag/catalog, and private-beta evidence remain future explicit work.

The First Light Gallery remains project-owned repository content and is not included in the UPM tarball.

## Documentation

Package documentation lives under `Documentation~`.

Start with:

- [Installation](Documentation~/User/Installation.md)
- [Quick Start](Documentation~/User/Quick%20Start.md)
- [Complete User Handout](Documentation~/User/Complete%20User%20Handout.md)

The suite-wide architecture and approved First Light specification live in the repository's `Plan Documentation` vault.

## Evidence Status

Available evidence includes:

- embedded package recognition and clean Unity compilation;
- Unity restart and embedded-package removal/restoration evidence;
- stable assembly-definition GUIDs;
- retained FL-M5-07 `809 / 809` automated and `12 / 12` manual Laboratory evidence;
- FL-M6-01 H1 `5 / 5`, H2 `35 / 35`, and final `EchoLaunchSetup` `224 / 224` focused evidence;
- public Setup creation/reuse/no-op behavior and explicit project-owned foundation resolution;
- read-only Validator, Direct Scene, Launch Simulator, and Standalone Laboratory evidence;
- permanent First Light Gallery with canonical First Light and independent UMBRA examples;
- UMBRA fresh Create → serialized three-entry sequence → runtime presentation → repeated `NoChanges` proof.

Still `Not run` / not claimed by FL-M6-01:

- Git URL installation
- Git tag installation
- Clean-project tarball installation qualification / public registry installation (the versioned tarball artifact is prepared, but route proof is still Not run)
- Separate clean-project reproduction of the final A1/A1-E1 happy path
- Fresh complete post-A1 EditMode and Runtime Play Mode aggregate
- Player-build qualification
- Performance measurements
- Release tag/catalog/private-beta qualification

## License

See [LICENSE.md](LICENSE.md).

## Third-Party Notices

See [Third Party Notices.md](Third%20Party%20Notices.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).
