# Changelog

All notable changes to First Light - Startup and Launch will be documented in this file.

The package follows Semantic Versioning once public compatibility commitments begin.

## [Unreleased]

### Added
#### FL-M5-07 - Standalone Test Laboratory and Importable Package Sample
- Exactly one Package Manager sample named `First Light Standalone Test Lab`
- Explicit user-initiated import with no automatic Build Settings, ProjectSettings, Setup, Repair, Validator, scripting-define, scene-open, or Play Mode mutation
- Package-owned Boot and Destination Laboratory scenes with visible status, destination, report, and warning evidence
- Public-API immediate-success, timed-progress, warning, recoverable-failure, and blocking-failure sample steps
- Pre-authored success, warning, recoverable, blocking, and invalid-destination configurations
- Duplicate-root, missing-configuration, invalid-destination, Direct Scene creation/reuse, and minimum-duration splash-skip fixtures
- Imported sample content that is project-owned, removable, and reimportable
- Explicit optional Laboratory authoring command with persistent Direct Scene reference verification
- Sample/core assembly isolation, no friend access, no hidden discovery, and no peer-package or project-specific runtime dependency
- Setup/Repair candidate isolation for imported First Light sample definitions and root prefabs
- Stable package acceptance registry `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012`
- Seven focused Laboratory package EditMode tests
- Two hundred ninety-nine total passing EditMode tests
- Five hundred three retained Runtime Play Mode tests
- Eight hundred two total passing automated tests
- Manual acceptance of all twelve Laboratory cases, sample removal, clean reimport, three-run Setup/Repair repeatability, and final repository cleanup
#### FL-M5-06 - Launch Simulator and Deterministic Failure Injection
- Explicit Editor-only `Tools > Sperk's Forge > First Light > Simulator` window
- No automatic simulation on window open, repaint, import, reload, or Play Mode entry
- Stable presets for immediate success, timed progress, warning continuation, recoverable-failure continuation, blocking failure, timeout, executor exception, and cancellation
- Immutable normalized `LaunchSimulationRequest`, deterministic plan values, progress samples, step reports, and schema-1 `LaunchSimulationReport`
- Stable simulator diagnostics `ELAUNCH-SIM-001` through `ELAUNCH-SIM-004`
- Stable simulated step diagnostics `ELAUNCH-SIM-STEP-001` through `ELAUNCH-SIM-STEP-003`
- Transient `HideAndDontSave` configuration, sequence, entry, and step-definition authoring
- Real `StartupSequenceRunner`, policy, progress, timeout, exception, and cancellation execution
- Deterministic logical clock and copyable text evidence
- Single-active-run protection and cooperative cancellation
- Cancellation report normalization that removes human-click-dependent elapsed evidence while preserving canonical `ELAUNCH-STEP-005`
- No Runtime/player Simulator implementation, persistent scenario asset, scene mutation, Build Settings mutation, or project-authored asset rewrite
- Twenty-four focused Simulator EditMode tests
- Two hundred ninety total passing EditMode tests
- Five hundred three retained Runtime Play Mode tests
- Seven hundred ninety-three total passing automated tests
- Manual acceptance of all eight presets, clean Console behavior, repeatable cancellation fingerprints, and zero transient/project residue
#### FL-M5-05 - Direct Scene Development Initializer
- Project-owned immutable `DirectSceneConfiguration` schema version `1`
- Stable `DirectSceneEntryPolicy` values: `EditorOnly`, `EditorAndDevelopmentBuilds`, and `BootRequired`
- Stable direct-scene settlement states and immutable result evidence
- Stable runtime diagnostics `ELAUNCH-DIRECT-001` through `ELAUNCH-DIRECT-003`
- One-shot `EchoDirectSceneInitializer` settlement from `Start`
- Existing-authority reuse before creation
- Exactly-one approved `DirectSceneDevelopment` root creation
- Multiple-initializer convergence on one accepted authority
- Editor-only default policy and explicit Development-Build opt-in
- Unconditional non-development release-player creation prohibition
- Active-destination success without `LoadSceneAsync` or scene reload
- Existing launch-report schema version `2` with truthful Direct Scene mode
- Activated read-only `ELAUNCH-VAL-009`
- Five focused Direct Scene EditMode tests
- Twenty-four focused Direct Scene Runtime Play Mode tests
- Two hundred sixty-six total passing EditMode tests
- Five hundred three total passing Runtime Play Mode tests
- Seven hundred sixty-nine total passing automated tests
- Manual creation, reuse, convergence, warning, and exact healthy-restoration acceptance
#### FL-M5-04 - Read-Only Validator and Project Health Report
- Dedicated `Tools > Sperk's Forge > First Light > Validator` Editor window
- Explicit `Validate Project` action with no automatic validation on window open, repaint, import, reload, or Play Mode entry
- Immutable validation request, evidence, finding, and schema-1 report contracts
- Stable project-health states: `Healthy`, `NeedsAttention`, `Invalid`, and `Blocked`
- Stable validation severities: Information, Warning, Error, and Blocker
- Stable `ELAUNCH-VAL-001` through `ELAUNCH-VAL-015`
- Reserved `ELAUNCH-VAL-009` for later Direct Scene release-safety authority
- Read-only canonical configuration, startup-sequence, destination, splash, root-prefab, Boot-scene, and Build Settings inspection
- Enabled-build-scene duplicate-root inspection
- Scene-safe additive inspection with active/open/dirty scene-state preservation
- Deterministic request, evidence, and report fingerprints
- Deterministic plain-text `Copy Report` evidence
- Project-relative finding paths with absolute machine-path rejection
- Single-active validation-run protection and `ELAUNCH-VAL-015`
- Sanitized evidence-failure containment through `ELAUNCH-VAL-014`
- Twenty-five focused Validator EditMode tests
- Two hundred sixty-one total passing EditMode tests
- Retained four-hundred-seventy-nine Runtime Play Mode tests
- Manual healthy-to-blocked-to-healthy validation acceptance
- Exact restored healthy request, evidence, and report fingerprints
#### FL-M5-03 - Explicit Setup Repair and Existing-Asset Reconciliation
- Separate explicit `Repair Plan...` transaction that does not weaken create-only Apply
- Read-only repair evidence for configuration, destination, prefab lineage, Boot-scene root shape, and Build Settings identity
- Deterministic repair fingerprints and fresh-plan rejection before backup or writes
- Shared single-active mutation gate across Apply and Repair
- Immutable repair approval, candidate, change, backup, status, and result models
- Exact current-schema configuration-reference reconciliation
- Destination-scene-path reconciliation with authored-label preservation
- Verified project root-prefab configuration binding repair
- Zero-root canonical Boot-scene repair that preserves unrelated scene objects
- Missing or uniquely disabled canonical Boot Build Settings reconciliation
- Byte-for-byte asset and matching `.meta` backup beneath `Library/EchoDevGames/FirstLight/RepairBackups`
- Hash-verified backup, restore, cleanup, and retained-backup reporting
- Build Settings mutation performed only after asset, prefab, and scene repair succeeds
- Complete and incomplete repair rollback diagnostics
- Plain-text repair-result formatter and Copy Result action
- Stable `ELAUNCH-SETUP-013` through `ELAUNCH-SETUP-017`
- Repair path validation before filesystem lookup
- Distinct `ProjectSettings/EditorBuildSettings.asset` repair-result identity
- Repeat-safe second and third Repair returning `NoChanges`
- Two hundred nine Editor setup/apply/repair tests
- Retained twenty-seven prefab asset tests
- Retained four-hundred-seventy-nine Runtime Play Mode tests
#### FL-M5-02 - Approved Setup Apply Engine and Repeat-Safe Asset Creation
- Fresh-plan-gated create-only Setup Apply service
- Deterministic plan fingerprints and stale-plan rejection
- Single-active-Apply protection
- Immutable apply request, status, change record, and result models
- Deterministic project folder and ScriptableObject creation
- Configuration reference binding
- Project-owned root prefab variant generation
- Boot scene generation without opening or modifying the destination scene
- Explicit Build Settings writer with append and approved place-first policies
- In-memory compensating rollback journal
- Rollback and manual-recovery reporting
- Plain-text apply-result formatter and Copy Result command
- Stable `ELAUNCH-SETUP-008` through `ELAUNCH-SETUP-012`
- Repeat-safe second and third Apply returning `NoChanges`
- Readonly `Scene` lease restoration through a mutable local copy
- One hundred seventy focused EditMode setup-and-apply tests
- Retained twenty-seven prefab asset tests
- Retained four-hundred-seventy-nine Runtime Play Mode tests
#### FL-M5-01 - Editor Setup Foundation and Non-Destructive Project Plan
- Preview-only First Light Setup window
- Stable menu path at `Tools/Sperk's Forge/First Light/Setup`
- Read-only project snapshot collector
- Immutable in-memory setup request
- Immutable project asset and Build Settings facts
- Immutable setup operations, diagnostics, and plan
- Deterministic side-effect-free setup planner
- Approved project-owned default path set
- Project path normalization and safety validation
- Existing compatible asset reuse planning
- Incompatible target conflict planning
- Ambiguous candidate manual-decision planning
- Unsupported schema migration blocking
- Package root-template prerequisite validation
- Default append-if-missing Build Settings policy
- Explicit-approval place-first Build Settings policy
- Preservation of unrelated Build Settings scene order
- Stable `ELAUNCH-SETUP-001` through `ELAUNCH-SETUP-007`
- Deterministic plain-text setup-plan formatter
- Copy Plan command
- Preview-only warning and plan/diagnostic presentation
- No Apply, Repair, Migrate, Create, or Build Settings mutation action
- Editor-only friend access for setup tests
- Sixty-six focused Editor setup tests
- Retained twenty-seven prefab asset tests
- Retained four-hundred-seventy-nine Runtime Play Mode tests
#### FL-M4-05 - Startup Presentation Prefab and Canvas Assembly
- Stable package-owned `EchoLaunchStatusView.prefab`
- Stable package-owned `EchoLaunchRoot.prefab`
- Committed prefab and folder `.meta` identities
- Self-contained Screen Space Overlay startup Canvas
- `CanvasScaler` using 1920 x 1080 reference resolution and 0.5 match
- Hidden, non-interactive default `CanvasGroup`
- Neutral high-contrast backdrop and legacy uGUI text
- Complete splash and status presentation hierarchy
- Distinct determinate and indeterminate progress surfaces
- Every `EchoLaunchStatusView` serialized reference assigned
- Splash root inactive by default
- Non-interactable progress slider
- All graphics configured as non-raycast targets
- No `EventSystem`, input module, `GraphicRaycaster`, `Button`, or package skip binding
- No TextMeshPro or project-owned asset dependency
- Root prefab containing one authoritative `EchoLaunchRoot`
- Nested instance of `EchoLaunchStatusView.prefab`
- Root presenter reference wired to the nested view
- Root configuration intentionally unassigned
- Canonical Boot mode with automatic start enabled
- Editor-only prefab asset-test assembly
- Twenty-seven focused EditMode prefab asset tests
- Temporary Unity authoring helper removed after generation
#### FL-M4-04 - Splash Configuration Schema and Root Playback Integration
- `EchoLaunchConfiguration` schema version `4`
- Optional serialized project-owned `SplashSequence`
- Serialized project-authored `UseReducedMotionForSplash`
- Historical schema `3` rejection without runtime rewrite
- Public read-only splash and reduced-motion configuration surface
- Internal side-effect-free `SplashSequencePreflight`
- Null splash reference as a legal omission
- Empty valid splash sequence as a legal no-op
- Invalid assigned splash rejection before splash, startup-step, or destination side effects
- Stable `ELAUNCH-SPLASH-001` preflight diagnostic
- Stable `ELAUNCH-SPLASH-002` unexpected-playback diagnostic
- Stable `ELAUNCH-SPLASH-003` missing-visual-presenter warning
- Root-owned deterministic splash playback
- Shared injectable monotonic launch clock for splash and report timing
- Sequential root order of optional splash, startup steps, and destination transition
- Reduced-motion forwarding from schema-4 configuration
- Visual `IImageSplashPresenter` resolution through the active status presenter
- Logging-free headless fallback with authored timing preserved
- Splash clear before startup-step presentation
- Root cancellation during splash mapped to the existing interrupted-launch settlement
- Splash presenter or playback failure blocking startup steps and destination loading
- Successful splash timing included in existing total launch elapsed time
- Internal retention of the latest successful `SplashPlaybackResult`
- Duplicate-root splash silence
- Automatic-start splash-path proof
- Direct-scene mode splash-contract proof
- Configuration and splash asset immutability proof
- Report schema version `2` preserved
- Twenty-eight focused root splash lifecycle tests
- One additional retained schema-history configuration test
#### FL-M4-03 - Image Splash Definitions and Deterministic Splash Player
- Project-owned `SplashSequence` ScriptableObject with schema version `1`
- Immutable `SplashEntry` definitions with stable canonical identities
- Image-only splash entries with replaceable display labels
- Authored fade-in, hold, fade-out, and minimum-display timing
- Stable `SplashSkipPolicy` vocabulary
- Stable `SplashPlaybackPhase` vocabulary
- Immutable `SplashPresentationFrame`
- Immutable `SplashPlaybackResult`
- Neutral public `IImageSplashPresenter`
- Logging-free `NullImageSplashPresenter`
- Deterministic `SplashSequencePlayer` driven by `ILaunchClock`
- Sequence and entry validation without runtime asset repair
- Duplicate entry-ID detection
- Missing-image and malformed-timing rejection
- Ordered multi-entry traversal
- Normalized deterministic alpha projection
- Minimum-display hold expansion
- Latched early skip requests
- Disallowed skip containment
- Reduced-motion playback with fade phases removed
- Cancellation cleanup and presenter clearing
- Player-local active-playback re-entry protection
- Invalid and backward clock rejection
- Headless playback fallback
- Definition immutability proof
- `EchoLaunchStatusView` image-splash presentation
- Serialized splash root, image, and label references
- Public `RequestSplashSkip()` without an EchoInput dependency
- Splash image, label, alpha, and sequence-position rendering
- Splash clear and unbind cleanup
- Twenty-six Runtime splash tests
- Ten isolated uGUI splash-presentation tests
#### FL-M4-02 - Default uGUI Plain Status View and Presentation Assembly
- Separate `EchoDevGames.EchoLaunch.Presentation.UGUI` runtime assembly
- Separate `EchoDevGames.EchoLaunch.Tests.Presentation.UGUI` test assembly
- Public `EchoLaunchStatusView`
- Neutral `ILaunchStatusPresenter` implementation without widening Runtime
- Serialized `CanvasGroup`, `Text`, `Slider`, and progress-surface references
- Readable authority, validation, running, warning, transitioning, completed, failed, and interrupted copy
- Determinate normalized slider progress with percentage text
- Distinct indeterminate-progress surface and copy
- Active-step position and stable step-ID display
- Elapsed-time display
- Finalized report diagnostic and destination metadata display
- Exact terminal report retention
- Completed-report progress forced to 100 percent
- Failed/interrupted reports preserving the latest accepted progress mode
- Configurable show-on-bind, hide-on-unbind, and clear-on-unbind behavior
- Missing optional visual references remaining safe
- Serialized replaceable state copy
- Runtime friend access limited to the isolated presentation test assembly
- Presentation friend access limited to the isolated presentation test assembly
- Eighteen Runtime Play Mode presentation tests
#### FL-M4-01 - Automatic Root Start Gate and Plain Status Presenter Contract
- Serialized automatic-start gate enabled by default
- Unity `Start` callback routed through the existing `StartLaunchAsync` one-run boundary
- Manual-start-before-`Start` re-entry prevention
- Public neutral `ILaunchStatusPresenter`
- Internal silent `NullLaunchStatusPresenter`
- Internal safe presenter resolver and dispatcher
- Serialized `MonoBehaviour` presenter seam without a Runtime uGUI dependency
- Presenter binding before `Validating`
- Accepted snapshot presentation before public progress events
- Finalized report presentation after `LastReport` assignment
- Presenter unbinding during authoritative root destruction
- Duplicate-root presenter and automatic-start silence
- Stable invalid-presenter diagnostic `ELAUNCH-VIEW-001`
- Stable presenter-callback diagnostic `ELAUNCH-VIEW-002`
- Per-callback presenter exception containment
- Internal deterministic automatic-start test seam
- Internal deterministic presenter-injection test seam
- Sixteen Runtime Play Mode automatic-start and presenter tests
#### FL-M3-08 - Initial Destination Contract, Load Result, and Completed Handoff
- Project-owned immutable `LaunchDestination` ScriptableObject
- Destination schema version `1`
- Stable destination identity, display label, and runtime-safe scene path
- Configuration schema version `3`
- Serialized initial-destination reference on `EchoLaunchConfiguration`
- Historical configuration schema `2` rejection without runtime rewrite
- Public `InitialDestinationLoadStatus`
- Public immutable `InitialDestinationLoadResult`
- Public injectable `IInitialDestinationLoader`
- Internal optional destination preflight-validator seam
- Internal normalized destination-progress relay
- Standalone `UnityInitialDestinationLoader`
- Root-owned destination validation before startup-step side effects
- Stable `ELAUNCH-DEST-001` destination-preflight diagnostic
- Stable `ELAUNCH-DEST-002` destination-load diagnostic
- Destination progress publication while `Transitioning`
- Successful `Transitioning -> Completed` lifecycle handoff
- Report schema version `2`
- Completed report destination identity and display metadata
- Public exactly-once `LaunchCompleted`
- State and completed report acceptance before event dispatch
- Completion-listener isolation through `ELAUNCH-EVENT-001`
- Destruction-driven late-completion suppression
- Thirty-seven destination and completed-handoff tests
- Seven additional configuration and destination-binding tests
#### FL-M3-07 - Immutable Launch Report and Public Terminal Events
- Public immutable `LaunchStepReport`
- Public immutable `LaunchReport`
- Report schema version `1`
- Producing package version `0.1.0`
- Internal single-use `LaunchReportBuilder`
- Authority-filtered `EchoLaunchRoot.LastReport`
- Public `LaunchFailed`
- Public `LaunchInterrupted`
- Defensive copying of ordered terminal step summaries
- Launch timing and authored traversal accounting
- Warning, failure, blocking-failure, and cancellation summaries
- Failed preflight and blocking-run report finalization
- Interrupted report finalization after executor settlement
- State and `LastReport` acceptance before terminal event dispatch
- Exactly-once terminal event publication
- Terminal listener isolation through `ELAUNCH-EVENT-001`
- Duplicate-root report and terminal-event silence
- Destruction-driven late terminal-event suppression
- Transition-pending success retaining no finalized report
- Twenty-five Runtime Play Mode report and terminal-event tests
#### FL-M3-06 - Root-Owned Startup Run and Lifecycle Advancement
- Explicit root-owned `StartLaunchAsync` execution boundary
- Public cooperative `CancelLaunch` command for the active authoritative root
- Root-local active-launch gate with stable `ELAUNCH-LIFE-002`
- Structured preflight diagnostic preservation through `StartupSequencePreflightException`
- Internal `IStartupSequenceObserver` runner-to-root observation seam
- Internal `StartupStepProgressRelay`
- Authoritative lifecycle publication through `Validating`, `Running`, `Failed`, `Interrupted`, and `Transitioning`
- Step-start, progress, and completion translation into existing root progress snapshots
- Stable lifecycle interruption diagnostic `ELAUNCH-LIFE-001`
- Destruction-driven cooperative cancellation and late-publication suppression
- Duplicate-root start and cancellation rejection
- Root-owned run-result retention
- Success stopping at `Transitioning` until destination handoff exists
- Legacy three-argument runner exception compatibility
- Twenty-three Runtime Play Mode root lifecycle tests
#### FL-M3-05 - Runner Re-entry Protection and Sequence Preflight Boundary
- Internal side-effect-free `StartupSequencePreflight`
- Complete configuration and startup-sequence validation before executor creation
- Configuration, sequence, entry, and step identity validation
- Configuration, sequence, and step schema validation
- Null-entry and enabled-missing-definition rejection
- Undefined entry-activation rejection
- Duplicate entry-ID and duplicate step-ID detection
- Preserved empty-sequence behavior
- Preserved disabled-entry-without-definition behavior
- Runner-local atomic active-run gate
- Stable concurrent re-entry diagnostic `ELAUNCH-RUN-001`
- `try/finally` gate release across success, preflight rejection, blocking traversal, and structured cancellation
- Sequential reuse of one runner instance after settlement
- Twenty-three Runtime Play Mode preflight and re-entry tests

#### FL-M3-04 - Multi-Frame Async Proof and Runner Cancellation Outcome
- Structured caller-cancellation observation after executor settlement
- Stable caller-cancellation diagnostic `ELAUNCH-STEP-005`
- Terminal `StartupStepStatus.Cancelled` runner outcome
- Immutable `StartupSequenceRunResult.WasCancelled`
- Cancellation-driven traversal stop before later factory creation
- Same-tick caller-cancellation race containment
- Production-shaped multi-frame executor using `Awaitable.NextFrameAsync`
- Multi-frame progress, positive timing, and authored-order proof
- Two Runtime Play Mode multi-frame async tests


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
- The Setup window now exposes approved Apply and Copy Result actions while preserving the preview-first workflow.
- The package specification current status records FL-M5-02 as implemented, tested, manually accepted, and pushed.
- Setup mutation remains create-only: compatible content is reused; incompatible, ambiguous, unsupported, repair, and migration cases remain non-executable.
- Build Settings mutation occurs last and preserves unrelated scene order and enabled states.
- Repeated Apply is an explicit no-op rather than a duplicate-generation path.
- First Light now exposes a real Editor setup-planning surface while preserving a hard no-write boundary.
- Setup observation, planning, and future mutation are separated into distinct architectural stages.
- The package specification current status now records FL-M5-01 as implemented and tested.
- Editor setup planning now uses project-owned default paths beneath `Assets/EchoDevGames/FirstLight`.
- Build Settings changes remain proposals only; default planning appends Boot and place-first planning requires explicit approval.
- First Light now ships inspectable scene-ready presentation templates without adding hidden runtime prefab discovery or instantiation.
- Package branding remains neutral; production art, fonts, layout variants, and input bindings remain project-owned.
- The package README now consistently reports configuration schema version `4`.
- Public prefab identity is preserved through committed `.meta` files while internal decorative child values remain non-contractual.
- Root preflight now validates configuration identity/schema, the optional splash sequence, the startup sequence, and the initial destination before launch side effects.
- The authoritative launch order is now optional splash, startup steps, then initial destination.
- Splash playback and startup-step execution remain sequential rather than concurrent.
- The active launch clock is shared by root splash playback, startup execution, and report timing.
- A configured splash without a visual splash presenter now preserves timing through `NullImageSplashPresenter` and emits one warning.
- `EchoLaunchConfiguration.CurrentSchemaVersion` is now `4`.
- `LaunchReport.CurrentSchemaVersion` remains `2`.
- First Light now includes standalone project-owned image splash definitions and deterministic playback without binding them to configuration or root launch flow.
- The default uGUI view now implements both `ILaunchStatusPresenter` and `IImageSplashPresenter`.
- Splash timing uses the established monotonic unscaled `ILaunchClock` seam.
- Reduced-motion playback removes fade phases while preserving authored hold and minimum-display time.
- Skip requests are neutral events supplied by project-owned input and cannot bypass minimum display time.
- Runtime splash playback remains valid with no visual presenter.
- `EchoLaunchConfiguration` remains at schema version `3`; no serialized splash reference was added in this checkpoint.
- Launch reports remain at schema version `2`; splash results are not yet included.
- First Light now provides a removable default plain uGUI implementation while preserving the neutral `ILaunchStatusPresenter` contract.
- The neutral Runtime asmdef remains free of uGUI and TextMeshPro references.
- Presentation code now lives under `Presentation.UGUI` and may be replaced without changing launch truth.
- Runtime `AssemblyInfo.cs` now grants internal report-constructor access only to the dedicated presentation test assembly.
- Completed terminal presentation now renders destination display metadata and full progress.
- Failed and interrupted terminal presentation retains the latest accepted progress mode.
- State meaning remains readable through text rather than requiring color.
- Serialized state copy may be replaced without subclassing the view.
- Authoritative roots now begin the existing launch gate from Unity `Start` when automatic startup is enabled.
- Automatic and manual startup share the same authority, lifecycle, and active-run protections.
- Retained manual Runtime Play Mode fixtures explicitly disable automatic startup before invoking `StartLaunchAsync`.
- Accepted launch snapshots are presented before public state/progress notifications.
- Finalized terminal reports are assigned to `LastReport`, presented, and then dispatched through the matching public terminal event.
- Missing presentation remains logging-free through the headless fallback.
- `EchoLaunchConfiguration.CurrentSchemaVersion` advanced from `2` to `3`; schema 3 adds the project-owned initial destination reference.
- Configuration schema 2 remains historical and is rejected through `ELAUNCH-CFG-002` without runtime migration or mutation.
- Successful and warning-only startup-sequence settlement now proceeds through initial destination loading and reaches `Completed`.
- The final lifecycle result after a successful handoff describes destination activation, while startup warnings remain preserved in the immutable completed report.
- `LaunchReport.CurrentSchemaVersion` advanced from `1` to `2` because completed reports now include destination identity and display metadata.
- `LaunchReportBuilder` now finalizes completed, failed, or interrupted reports exactly once.
- `EchoLaunchRoot.LastReport` now exposes completed reports in addition to failed and interrupted reports.
- `LaunchCompleted` now dispatches after completed state and report storage are authoritative.
- Root-owned destination validation occurs before any startup-step factory or destination-loader side effect.
- The startup-sequence runner remains destination-neutral.
- Failed and interrupted root-owned launches now finalize one immutable report after the terminal lifecycle snapshot is accepted.
- `EchoLaunchRoot.LastReport` now exposes only the authoritative finalized failed or interrupted report.
- `LaunchFailed` and `LaunchInterrupted` now dispatch after root state and report storage are authoritative.
- Successful sequence settlement still stops at `Transitioning` and intentionally produces no finalized report or completed event.
- Completed step executions are copied into immutable report values rather than exposed as live runtime objects.
- Report schema versioning is independent from package version and authored configuration schemas.
- `EchoLaunchRoot` now owns one explicit startup-sequence run after authority claim.
- Root lifecycle publication now advances from `AuthorityClaimed` through `Validating` and `Running`.
- Successful startup-sequence settlement now advances to `Transitioning`, not `Completed`, because destination handoff remains pending.
- Blocking or unexpected execution outcomes now advance the authoritative session to `Failed`.
- Caller or destruction cancellation now settles active work before the authoritative session advances to `Interrupted`.
- Existing root state and progress events now reflect runner validation, step start, step progress, step completion, and terminal mapping.
- The observer-aware runner overload preserves structured preflight diagnostics for the root.
- The legacy three-argument runner overload preserves exact historical `InvalidOperationException` behavior.
- `StartupSequenceRunner` now performs complete authored preflight before the first executor factory can run.
- One runner instance now rejects concurrent traversal with `ELAUNCH-RUN-001`.
- The runner's active-run gate is released through `finally`, preserving later sequential reuse after every terminal path.
- Invalid policy retains its structured pre-start blocking-result behavior while broader authored-data faults fail before factory creation.
- Empty sequences and disabled entries without definitions remain valid compatibility cases.
- Caller cancellation now returns a structured cancelled run result after the active executor settles.
- Authored `ContinueWithWarning` policy cannot downgrade caller cancellation or continue traversal.
- `StartupSequenceRunResult` now reports `WasCancelled`.
- The timeout monitor now preserves caller-cancellation ownership across same-tick executor settlement.
- Retained caller-cancellation coverage now asserts `ELAUNCH-STEP-005`, settlement, stopping index, and unvisited later entries.

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
- Trimmed Unity-generated trailing whitespace from the new Editor setup folder metadata before commit.
- Restored generated solution-file noise before validation and staging.
- Trimmed Unity-generated trailing whitespace from the committed prefab YAML and generated `.meta` files without changing asset GUIDs or serialized behavior.
- Removed the temporary `Assets/FLM405Temp` authoring folder and its generated metadata before staging.
- Replaced a zero-advance synchronous manual clock in `ConcurrentPlaybackIsRejected` that caused an infinite main-thread test loop.
- Moved three skip requests into deterministic frame presentation so they occur during playback rather than after synchronous completion.
- Updated the concurrent-playback assertion to consume the faulted `Awaitable`, allowing NUnit to observe `InvalidOperationException`.
- Updated the sequence-identity uniqueness test to compare untouched newly created assets rather than the fixed-ID helper.
- Confirmed the apparently stuck `SnapshotRejectsInvalidElapsedTime` test was only the last painted Test Runner row, not the source of the hang.
- Added the missing `EchoDevGames.EchoLaunch.Presentation.UGUI` namespace import to the isolated presentation test fixture.
- Replaced thirteen unsupported NUnit `Assert.Multiple` blocks with sequential `Assert.That` calls compatible with the installed Unity test framework.
- Restored generated `.slnx` noise before repository review.
- Removed Unity-generated trailing whitespace from the new presentation metadata files before commit.
- Replaced an invalid `AudioSource` presenter test component with a dedicated `MonoBehaviour` that intentionally does not implement `ILaunchStatusPresenter`.
- Replaced unsupported NUnit `Is.AnyOf` usage with a direct terminal-state assertion compatible with the installed Unity test framework.
- Corrected three new FL-M3-08 test references from nonexistent `LaunchProgressSnapshot.IsIndeterminate` to the established `IsProgressIndeterminate` property.
- Updated the retained warning-completion test to expect a successful destination-activation lifecycle result while verifying that the completed report preserves the startup warning.
- Corrected the retained report assertion from nonexistent `LaunchStepReport.FinalStatus` to the established `Status` property.
- Replaced two nonexistent `EchoLaunchRuntimeReset.ResetStatics()` calls in the new FL-M3-07 test fixture with the established `LaunchAuthorityClaim.Reset()` test reset, restoring clean compilation.
- Restored exact legacy `InvalidOperationException` behavior for direct three-argument runner calls after the first FL-M3-06 full-suite run exposed fifteen retained exact-type assertions.
- Preserved structured `StartupSequencePreflightException` behavior for root-owned observer runs.
- Contained the same-tick caller-cancellation race where the executor settled with `OperationCanceledException` before the monitor's next loop.
- Kept the final FL-M3-04 Unity compilation result at zero errors and zero compiler warnings.

- Adapted the timeout test helper to Unity `6000.3.8f1`, where `AwaitableCompletionSource<T>.SetResult` accepts the result by value.
- Realigned the retained immediate-runner fixture after a stale test artifact temporarily restored three pre-FL-M3-02 expectations.
- Preserved FL-M3-02 policy-aware retained behavior while adding the FL-M3-03 linked-token expectation.
- Kept the full Unity compilation result at zero errors and zero warnings.

### Tested

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

FL-M5-02 coverage:

- Fresh-plan fingerprinting and stale-plan rejection
- Single-active-Apply containment
- Create, reuse, and no-change execution
- Folder, asset, configuration, prefab-variant, scene, and Build Settings writers
- Destination/open/active/dirty scene preservation
- Build Settings append and approved place-first policy
- Failure injection, compensating rollback, and recovery reporting
- Immutable result formatting
- Successful first Apply and `NoChanges` second and third Apply
- Manual acceptance fingerprint `7e669d66eaab2c04a0dfbc4445458fcd976808c83f62db82c3d91a16494fc0c1`

Retained FL-M5-01 coverage:
- Approved default project paths
- Path normalization and invalid-path rejection
- Immutable request, snapshot, operation, diagnostic, and plan values
- Defensive collection copying
- Stable deterministic operation ordering
- Equivalent plan output for equivalent evidence
- Existing compatible asset reuse
- Wrong target type conflicts
- Unsupported configuration schema blocking
- Ambiguous candidate manual decisions
- Package-template prerequisite blocking
- Optional splash planning
- Default append-if-missing Build Settings planning
- Do-not-change Build Settings planning
- Explicit-approval place-first planning
- Read-only Build Settings observation
- Open-scene state preservation
- Package-template dirty-state preservation
- Missing-destination handling
- Deterministic plain-text reports
- Preview-only Setup window availability
- Stable menu path and warning copy
- No Apply, Repair, Migrate, asset-create, or Build Settings mutation method
- No project folder or Boot scene creation during collection/window refresh
- Zero compiler errors and zero compiler warnings

FL-M4-05 coverage:
- Status-view prefab path and stable GUID
- Root prefab path and distinct stable GUID
- Approved status root components
- Canvas render-mode, sorting, and scaler defaults
- Hidden and non-interactive initial Canvas state
- Required named presentation hierarchy
- Complete serialized presenter references
- Initial splash and progress-root states
- Non-interactive slider configuration
- Non-raycast graphics
- Absence of EventSystem, input modules, GraphicRaycaster, Button, and Toggle
- Absence of TextMeshPro components
- Built-in non-project font references
- Absence of project `Assets/` dependencies
- One root and one nested presenter
- Nested status-prefab identity
- Presenter reference targeting the nested view
- Intentionally null root configuration
- Canonical Boot mode and automatic start
- No missing scripts
- Successful prefab instantiation
- Zero compiler errors and zero compiler warnings

FL-M4-04 coverage:
- Stable splash diagnostic codes
- Schema-4 splash-sequence binding
- Schema-4 reduced-motion binding
- Historical schema-3 rejection without rewrite
- Root splash configuration exposure
- Null and empty splash no-op paths
- Startup-sequence preflight before splash side effects
- Invalid splash identity, schema, entry, image, and duplicate-ID blocking
- Splash presentation before the first startup step
- Splash clear before startup-step presentation
- Startup-step completion before destination loading
- Reduced-motion frame behavior through the root
- Missing visual presenter warning and headless continuation
- Project-routed skip shortening the configured splash
- Total report elapsed time including splash time
- Root retention of successful splash playback result
- Presenter/playback failure blocking step and destination work
- Root cancellation during splash with exactly-once interrupted settlement
- Duplicate-root splash silence
- Direct-scene mode using the same splash contract
- Configuration and splash definition immutability
- Launch report schema remaining 2
- Automatic-start route using the splash path
- Zero compiler errors and zero compiler warnings

FL-M4-03 coverage:
- Stable splash skip-policy and playback-phase vocabulary
- Splash sequence schema 1 and canonical identities
- Separate generated sequence identities
- Canonical entry identity
- Negative and nonfinite timing rejection
- Null-entry, duplicate-ID, and missing-image rejection
- Empty-sequence completion
- Authored single-entry timeline completion
- Ordered multi-entry traversal
- Fade-in, hold, and fade-out phases
- Normalized alpha values
- Minimum-display hold expansion
- Permitted skip after minimum display
- Early skip latching until minimum display
- Disallowed skip containment
- Reduced-motion fade removal
- Cancellation cleanup
- Active-player re-entry rejection
- Backward-clock rejection
- Headless fallback
- Playback-result skipped-entry accounting
- Authored asset immutability
- Default uGUI splash presenter contract
- Pre-bind splash no-op
- Image, label, state, position, and alpha projection
- Public skip-request event
- Splash clearing and unbind cleanup
- Null-frame rejection
- Missing splash-reference safety
- Zero compiler errors and zero compiler warnings

FL-M4-02 coverage:
- Presentation view implementing the neutral presenter contract
- Bind visibility and authority copy
- Determinate slider progress and percentage
- Indeterminate progress surface and copy
- Active-step position and stable step ID
- Elapsed-time formatting
- Warning copy and diagnostic rendering
- Transitioning copy
- Completed report destination, message, and 100-percent progress
- Failed report diagnostic rendering
- Interrupted report cancellation rendering
- Pre-bind snapshot no-op
- Pre-bind terminal-report no-op
- Null terminal-report rejection
- Hide-on-unbind behavior
- Clear-on-unbind behavior
- Rebind clearing the previous terminal report
- Missing optional visual references remaining safe
- Serialized state-copy replacement
- Separate presentation runtime and test assemblies
- Neutral Runtime asmdef remaining uGUI-free
- Zero compiler errors and zero compiler warnings

FL-M4-01 coverage:
- Automatic launch on the first enabled authoritative root
- Disabled automatic startup remaining at `AuthorityClaimed`
- Manual launch before Unity `Start` without re-entry
- Presenter binding exactly once before validation
- Accepted lifecycle snapshot ordering
- Exact finalized report presentation
- Silent headless fallback
- Serialized presenter component resolution
- Invalid assigned presenter containment through `ELAUNCH-VIEW-001`
- Bind, progress, and terminal callback containment through `ELAUNCH-VIEW-002`
- Completion event continuity after presenter failure
- Presenter replacement rejection after launch advancement
- Null presenter-injection rejection
- Exactly-once presenter unbind on destruction
- Duplicate-root automatic-start and presenter silence
- Zero compiler errors and zero compiler warnings

FL-M3-08 coverage:
- Destination schema 1 and canonical identity
- Configuration schema 3 and initial destination binding
- Historical schema 2 rejection without runtime rewrite
- Destination display-name and scene-path validation
- Missing and invalid destination rejection before factories and loading
- Loader-specific preflight rejection
- Missing loader rejection
- Immutable success, failure, and cancellation load results
- Normalized destination progress and late-progress containment
- Successful destination handoff and exactly-once loader invocation
- `Transitioning` destination progress
- `Transitioning -> Completed` state order
- Completed report destination and sequence accounting
- Exact `LastReport` and `LaunchCompleted` payload identity
- Exactly-once completion without failed or interrupted events
- Completion-listener isolation
- Destination-load failure through `ELAUNCH-DEST-002`
- Null and mismatched loader result containment
- Cancellation before load start
- Cancellation during injected load after settlement
- Destruction-driven late-completion suppression
- Default-loader build-settings rejection
- Default-loader pre-start cancellation
- Completed-report single finalization
- Completed-report destination requirements
- Authored configuration and destination immutability
- Startup warning preservation in the completed report
- Zero compiler errors and zero compiler warnings

FL-M3-07 coverage:
- `LastReport` null before finalization
- Missing-configuration failed report
- Invalid-preflight report before executor creation
- Blocking-step immutable terminal copy
- Warning, disabled, failure, and unvisited accounting
- Failed event after accepted failed state and stored report
- Exactly-once failed event without interrupted event
- Failed-listener isolation
- Interrupted report after executor settlement
- Interrupted event after accepted interrupted state and stored report
- Exactly-once interrupted event without failed event
- Normalized blank cancellation reason
- Interrupted-listener isolation
- Transition-pending success without finalized report or terminal event
- Duplicate-root report and terminal-event silence
- Destruction-driven late terminal-event suppression
- Stable report schema and package version
- Public report properties without public setters
- Indexed report bounds checking
- Builder second-finalization rejection
- Rejection of nonterminal successful report status
- Defensive step-list copying
- Report readability after root and authored assets are destroyed
- Authored asset immutability
- Accounting and timing invariant rejection
- Zero compiler errors and zero compiler warnings

FL-M3-06 coverage:
- Authority claim without automatic startup
- Empty-sequence success to `Transitioning`
- Approved `AuthorityClaimed -> Validating -> Running -> Transitioning` order
- Root publication of step start, progress, and completion
- Warning traversal success to `Transitioning`
- Blocking traversal failure to `Failed`
- Structured preflight failure before executor creation
- Missing-configuration failure mapping
- Cancellation rejection before launch
- Cooperative cancellation waiting for executor settlement
- Stable blank cancellation reason
- Repeated cancellation request rejection
- Concurrent root start rejection before a second factory
- Settled and failed session restart rejection
- Duplicate-root start and cancellation rejection
- Destruction cancellation and late-publication suppression
- No premature `Completed` publication
- Direct-scene launch-mode preservation
- Authored asset immutability
- Runner replacement restriction after lifecycle advancement
- Active-gate release after preflight failure
- Exactly one `Interrupted` lifecycle publication
- Zero compiler errors and zero compiler warnings

FL-M3-05 coverage:
- Unknown launch-mode rejection before factory creation
- Null configuration rejection
- Invalid configuration identity and unsupported schema rejection
- Missing sequence rejection
- Invalid sequence identity and unsupported schema rejection
- Null-entry rejection before any factory
- Invalid entry identity and undefined activation rejection
- Duplicate entry-ID detection
- Enabled missing-definition rejection
- Invalid step identity and unsupported step schema rejection
- Duplicate step-ID detection
- Invalid policy conversion before factory creation
- Empty-sequence compatibility
- Disabled-entry-without-definition compatibility
- Authored asset immutability
- Concurrent re-entry rejection through `ELAUNCH-RUN-001`
- No second factory during rejected re-entry
- Runner reuse after success
- Gate release after preflight rejection
- Gate release after structured caller cancellation
- Gate release after blocking traversal
- Zero compiler errors and zero compiler warnings

FL-M3-04 coverage:
- Production-shaped multi-frame `Awaitable.NextFrameAsync` execution
- Progress retention while an attempt remains active
- Positive monotonic elapsed timing
- Authored-order traversal after multi-frame settlement
- Linked caller cancellation reaching the executor
- Executor settlement before runner return
- Structured `Cancelled` result with `ELAUNCH-STEP-005`
- Run-level `WasCancelled`
- Later-entry and later-factory suppression
- Same-tick caller-cancellation race containment
- Authored asset immutability
- Zero compiler errors and zero compiler warnings

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
- Public step lifecycle events
- Warning aggregation outside the run result
- Dependency validation
- Explicit setup repair and existing-asset reconciliation
- Editor migration from historical configuration schemas
- Direct-scene initializer tooling
- Real Boot-to-destination Laboratory activation proof
- Persistent root lifetime policy
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
