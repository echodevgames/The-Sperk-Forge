# First Light - Package Documentation

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

## Current Status

- Package version: `0.1.0`
- Development stage: FL-M5-06 complete; FL-M5-07 Standalone Test Laboratory authority approved, implementation not yet started
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
  - `FL-M3-04`
  - `FL-M3-05`
  - `FL-M3-06`
  - `FL-M3-07`
  - `FL-M3-08`
  - `FL-M4-01`
  - `FL-M4-02`
  - `FL-M4-03`
  - `FL-M4-04`
  - `FL-M4-05`
  - `FL-M5-01`
  - `FL-M5-02`
  - `FL-M5-03`
  - `FL-M5-04`
  - `FL-M5-05`
  - `FL-M5-06`
- Active authority checkpoint: `FL-M5-07` — Standalone Test Laboratory and Importable UPM Sample
- Current implemented boundary: runtime launch authority through completed destination handoff, removable plain uGUI and image-splash presentation, read-only setup planning, create-only repeat-safe Apply, explicit current-schema Repair, deterministic read-only project-health validation, release-gated Direct Scene development entry without scene reload, and an Editor-only deterministic Launch Simulator. FL-M5-07 authorizes only the importable Standalone Laboratory sample and evidence-gated sample-isolation correction described by specification v1.13.0.
- Unity baseline: `6000.3.8f1`

## User Documentation

- [Installation](User/Installation.md)
- [Quick Start](User/Quick%20Start.md)

## Developer Documentation

- [Architecture](Developer/Architecture.md)
- [Current Notes](Developer/Current%20Notes.md)
- [EchoLaunch-ADR-001 Project-Owned Launch Destination and Configuration Schema 3](Developer/ADR/EchoLaunch-ADR-001_Project-Owned_Launch_Destination_and_Configuration_Schema_3.md)
- [EchoLaunch-ADR-002 Splash Configuration Schema 4 and Root Playback Order](Developer/ADR/EchoLaunch-ADR-002_Splash_Configuration_Schema_4_and_Root_Playback_Order.md)
- [EchoLaunch-ADR-003 Neutral Startup Prefab Templates and Canvas Assembly](Developer/ADR/EchoLaunch-ADR-003_Neutral_Startup_Prefab_Templates_and_Canvas_Assembly.md)
- [EchoLaunch-ADR-004 Read-Only Project Snapshot and Non-Destructive Setup Plan](Developer/ADR/EchoLaunch-ADR-004_Read-Only_Project_Snapshot_and_Non-Destructive_Setup_Plan.md)
- [EchoLaunch-ADR-005 Approved Setup Apply Engine and Repeat-Safe Asset Creation](Developer/ADR/EchoLaunch-ADR-005_Approved_Setup_Apply_Engine_and_Repeat-Safe_Asset_Creation.md)
- [EchoLaunch-ADR-006 Explicit Setup Repair and Existing-Asset Reconciliation](Developer/ADR/EchoLaunch-ADR-006_Explicit_Setup_Repair_and_Existing-Asset_Reconciliation.md)
- [EchoLaunch-ADR-007 Read-Only Validator and Deterministic Project Health Report](Developer/ADR/EchoLaunch-ADR-007_Read-Only_Validator_and_Deterministic_Project_Health_Report.md)
- [EchoLaunch-ADR-008 Direct Scene Development Initializer and Release-Safe Runtime Gate](Developer/ADR/EchoLaunch-ADR-008_Direct_Scene_Development_Initializer_and_Release-Safe_Runtime_Gate.md)
- [EchoLaunch-ADR-009 Editor-Only Launch Simulator and Deterministic Failure Injection](Developer/ADR/EchoLaunch-ADR-009_Editor-Only_Launch_Simulator_and_Deterministic_Failure_Injection.md)
- [FL-M2-04 Launch Lifecycle Transition Guard](Developer/Checkpoints/FL-M2-04_Launch_Lifecycle_Transition_Guard.md)
- [FL-M2-04 Runtime Test Report](Developer/Test%20Reports/FL-M2-04_Launch_Lifecycle_Transition_Test_Report.md)
- [FL-M2-05 Lifecycle Notifications](Developer/Checkpoints/FL-M2-05_Lifecycle_Notifications.md)
- [FL-M2-05 Runtime Test Report](Developer/Test%20Reports/FL-M2-05_Lifecycle_Notification_Test_Report.md)
- [FL-M2-06 Launch Configuration Identity and Root Binding](Developer/Checkpoints/FL-M2-06_Launch_Configuration_Identity_and_Root_Binding.md)
- [FL-M2-06 Runtime Test Report](Developer/Test%20Reports/FL-M2-06_Launch_Configuration_Binding_Test_Report.md)
- [FL-M2-07 Startup Sequence Definition and Ordered Entry Model](Developer/Checkpoints/FL-M2-07_Startup_Sequence_Definition_and_Ordered_Entry_Model.md)
- [FL-M2-07 Runtime Test Report](Developer/Test%20Reports/FL-M2-07_Startup_Sequence_Definition_Test_Report.md)
- [FL-M2-08 Startup Step Policy and Executor Contract](Developer/Checkpoints/FL-M2-08_Startup_Step_Policy_and_Executor_Contract.md)
- [FL-M2-08 Runtime Test Report](Developer/Test%20Reports/FL-M2-08_Startup_Step_Policy_and_Executor_Contract_Test_Report.md)
- [FL-M3-01 Startup Sequence Runner Skeleton and Immediate Step Execution](Developer/Checkpoints/FL-M3-01_Startup_Sequence_Runner_Skeleton_and_Immediate_Step_Execution.md)
- [FL-M3-01 Runtime Test Report](Developer/Test%20Reports/FL-M3-01_Startup_Sequence_Runner_Immediate_Test_Report.md)
- [FL-M3-02 Step Result Policy Application and Exception Conversion](Developer/Checkpoints/FL-M3-02_Step_Result_Policy_Application_and_Exception_Conversion.md)
- [FL-M3-02 Runtime Test Report](Developer/Test%20Reports/FL-M3-02_Step_Result_Policy_and_Exception_Test_Report.md)
- [FL-M3-03 Monotonic Timeout Clock and Cooperative Cancellation](Developer/Checkpoints/FL-M3-03_Monotonic_Timeout_Clock_and_Cooperative_Cancellation.md)
- [FL-M3-03 Runtime Test Report](Developer/Test%20Reports/FL-M3-03_Timeout_Clock_and_Cancellation_Test_Report.md)
- [FL-M3-04 Multi-Frame Async Proof and Runner Cancellation Outcome](Developer/Checkpoints/FL-M3-04_Multi-Frame_Async_Proof_and_Runner_Cancellation_Outcome.md)
- [FL-M3-04 Runtime Test Report](Developer/Test%20Reports/FL-M3-04_Multi-Frame_Async_and_Runner_Cancellation_Test_Report.md)
- [FL-M3-05 Runner Re-entry Protection and Sequence Preflight Boundary](Developer/Checkpoints/FL-M3-05_Runner_Re-entry_Protection_and_Sequence_Preflight_Boundary.md)
- [FL-M3-05 Runtime Test Report](Developer/Test%20Reports/FL-M3-05_Preflight_and_Re-entry_Test_Report.md)
- [FL-M3-06 Root-Owned Startup Run and Lifecycle Advancement](Developer/Checkpoints/FL-M3-06_Root-Owned_Startup_Run_and_Lifecycle_Advancement.md)
- [FL-M3-06 Runtime Test Report](Developer/Test%20Reports/FL-M3-06_Root-Owned_Startup_Lifecycle_Test_Report.md)
- [FL-M3-07 Immutable Launch Report and Public Terminal Events](Developer/Checkpoints/FL-M3-07_Immutable_Launch_Report_and_Public_Terminal_Events.md)
- [FL-M3-07 Runtime Test Report](Developer/Test%20Reports/FL-M3-07_Immutable_Launch_Report_and_Terminal_Events_Test_Report.md)
- [FL-M3-08 Initial Destination Contract, Load Result, and Completed Handoff](Developer/Checkpoints/FL-M3-08_Initial_Destination_Contract_Load_Result_and_Completed_Handoff.md)
- [FL-M3-08 Runtime Test Report](Developer/Test%20Reports/FL-M3-08_Initial_Destination_and_Completed_Handoff_Test_Report.md)
- [FL-M4-01 Automatic Root Start Gate and Plain Status Presenter Contract](Developer/Checkpoints/FL-M4-01_Automatic_Root_Start_Gate_and_Plain_Status_Presenter_Contract.md)
- [FL-M4-01 Runtime Test Report](Developer/Test%20Reports/FL-M4-01_Automatic_Root_Start_and_Presenter_Contract_Test_Report.md)
- [FL-M4-02 Default uGUI Plain Status View and Presentation Assembly](Developer/Checkpoints/FL-M4-02_Default_uGUI_Plain_Status_View_and_Presentation_Assembly.md)
- [FL-M4-02 Runtime Test Report](Developer/Test%20Reports/FL-M4-02_Default_uGUI_Plain_Status_View_Test_Report.md)
- [FL-M4-03 Image Splash Definitions and Deterministic Splash Player](Developer/Checkpoints/FL-M4-03_Image_Splash_Definitions_and_Deterministic_Splash_Player.md)
- [FL-M4-03 Runtime Test Report](Developer/Test%20Reports/FL-M4-03_Deterministic_Image_Splash_Test_Report.md)
- [FL-M4-04 Splash Configuration Schema and Root Playback Integration](Developer/Checkpoints/FL-M4-04_Splash_Configuration_Schema_and_Root_Playback_Integration.md)
- [FL-M4-04 Runtime Test Report](Developer/Test%20Reports/FL-M4-04_Splash_Configuration_and_Root_Playback_Test_Report.md)
- [FL-M4-05 Startup Presentation Prefab and Canvas Assembly](Developer/Checkpoints/FL-M4-05_Startup_Presentation_Prefab_and_Canvas_Assembly.md)
- [FL-M4-05 Prefab Asset Test Report](Developer/Test%20Reports/FL-M4-05_Startup_Presentation_Prefab_Asset_Test_Report.md)
- [FL-M5-01 Editor Setup Foundation and Non-Destructive Project Plan](Developer/Checkpoints/FL-M5-01_Editor_Setup_Foundation_and_Non-Destructive_Project_Plan.md)
- [FL-M5-01 Editor Setup Planning Test Report](Developer/Test%20Reports/FL-M5-01_Editor_Setup_Planning_Test_Report.md)
- [FL-M5-02 Approved Setup Apply Engine and Repeat-Safe Asset Creation](Developer/Checkpoints/FL-M5-02_Approved_Setup_Apply_Engine_and_Repeat-Safe_Asset_Creation.md)
- [FL-M5-02 Setup Apply and Repeatability Test Report](Developer/Test%20Reports/FL-M5-02_Setup_Apply_and_Repeatability_Test_Report.md)
- [FL-M5-03 Explicit Setup Repair and Existing-Asset Reconciliation](Developer/Checkpoints/FL-M5-03_Explicit_Setup_Repair_and_Existing-Asset_Reconciliation.md)
- [FL-M5-03 Setup Repair and Reconciliation Test Report](Developer/Test%20Reports/FL-M5-03_Setup_Repair_and_Reconciliation_Test_Report.md)
- [FL-M5-04 Read-Only Validator and Project Health Report](Developer/Checkpoints/FL-M5-04_Read-Only_Validator_and_Project_Health_Report.md)
- [FL-M5-04 Validator and Project Health Test Report](Developer/Test%20Reports/FL-M5-04_Validator_and_Project_Health_Test_Report.md)
- [FL-M5-05 Direct Scene Development Initializer](Developer/Checkpoints/FL-M5-05_Direct_Scene_Development_Initializer.md)
- [FL-M5-05 Direct Scene Development Initializer Test Report](Developer/Test%20Reports/FL-M5-05_Direct_Scene_Development_Initializer_Test_Report.md)
- [FL-M5-06 Launch Simulator and Deterministic Failure Injection](Developer/Checkpoints/FL-M5-06_Launch_Simulator_and_Deterministic_Failure_Injection.md)
- [FL-M5-06 Launch Simulator Test Report](Developer/Test%20Reports/FL-M5-06_Launch_Simulator_and_Deterministic_Failure_Injection_Test_Report.md)

## Completed FL-M5-03 Boundary

FL-M5-03 implements a separate explicit Repair transaction for narrow,
proof-backed current-schema canonical drift. Create-only Apply remains
unchanged. Repair requires a fresh equivalent plan, explicit confirmation,
type/schema/identity/lineage/shape proof, exact asset plus `.meta` backup before
writes, Build Settings last, and deterministic result evidence. Manual
acceptance repaired five approved surfaces, preserved unrelated content and
identities, and returned `NoChanges` on the second and third Repair.

## Completed FL-M5-04 Boundary

FL-M5-04 implements a dedicated explicit read-only Validator, immutable
schema-1 project-health findings/report, stable `ELAUNCH-VAL-*` rules,
scene-safe enabled-build-scene inspection, deterministic fingerprints, and a
copyable project-relative text report.

Manual acceptance proved deterministic `Healthy`, deliberate `Blocked`, and
exact restored `Healthy` results. The Validator does not authorize or invoke
auto-fix, Apply, Repair, migration, Direct Scene implementation, build hooks,
Simulator, or Laboratory.

## Completed FL-M5-05 Boundary

FL-M5-05 implements the project-owned Direct Scene Development Initializer,
Start-time authority reuse, exactly-one approved direct root creation,
active-destination no-reload handoff, `EditorOnly` default with explicit
Development-Build opt-in, unconditional non-development release-player
prohibition, truthful `DirectSceneDevelopment` report mode, and activated
read-only `ELAUNCH-VAL-009`.

Manual acceptance proved direct creation, existing-authority reuse,
two-initializer convergence, one accepted authority, no scene reload,
Development-Build warning, and exact restored healthy fingerprints.

## Completed FL-M5-06 Boundary

FL-M5-06 implements the explicit Editor-only Launch Simulator, transient
in-memory scenario planning, real startup-sequence runner and policy
execution, deterministic logical timing, immutable schema-1 simulation
reports, stable diagnostics, copyable text evidence, cancellation, and
single-active-run protection.

Automated acceptance passed `24` focused Simulator tests, `290` complete
EditMode tests, and `503` Runtime Play Mode tests. Manual acceptance proved
every preset, clean Console behavior, transient cleanup, and repeatable
cancellation fingerprints after filtering human-click-dependent elapsed
evidence.

Runtime/player Simulator types, persistent scenario assets, authored
configuration mutation, root/presentation/destination simulation,
Standalone Laboratory scenes, build hooks, and report export remain outside
the implemented boundary.

## Package Root Documents

- [README](../README.md)
- [Changelog](../CHANGELOG.md)
- [License](../LICENSE.md)
- [Third-Party Notices](../Third%20Party%20Notices.md)

## Current Runtime Boundary

First Light currently proves:

- One valid launch authority
- Stable launch-state vocabulary
- One fresh launch session per authority
- Read-only state and progress
- Controlled snapshot replacement
- Approved lifecycle transition rules
- Frozen terminal states
- Transactional rejection of invalid publication
- State and progress lifecycle notifications
- Per-listener exception containment
- Project-owned launch configuration
- Immutable startup-step definitions
- Ordered startup-sequence modeling
- Authored required/optional policy
- Blocking and continue-with-warning failure metadata
- Safe timeout and cancellation metadata
- Immutable determinate and indeterminate progress
- Immutable validated execution context
- Package-owned progress-reporting seam
- Unity `Awaitable<StartupStepResult>` executor contract
- Fresh executor creation
- Safe Unity serialized entry defaults
- Runtime-only step-attempt state
- Immutable completed traversal summaries
- Disabled-entry skipping
- Fresh executor invocation for enabled entries
- Authored-order immediate traversal
- Immediate progress and terminal-result capture
- Explicit continue-with-warning conversion
- Explicit block-launch conversion and traversal stop
- Stable `ELAUNCH-STEP-004` exception and contract results
- Sanitized exception details
- Attempted, disabled, and unvisited entry accounting
- Stopping authored-index capture
- Public monotonic `ILaunchClock`
- Default unscaled Unity clock
- Immutable per-attempt timing
- Deterministic completion-versus-timeout ordering
- Stable `ELAUNCH-STEP-003`
- Cooperative timeout cancellation
- Timed-out executor settlement
- Late progress and result containment
- Production-shaped multi-frame Unity `Awaitable` execution
- Multi-frame progress, positive timing, and authored-order proof
- Structured caller cancellation after executor settlement
- Stable `ELAUNCH-STEP-005`
- Run-level `WasCancelled`
- Same-tick cancellation-race containment
- Complete configuration and sequence preflight before executor creation
- Configuration and sequence identity/schema validation
- Entry identity, activation, definition, and duplicate-ID validation
- Referenced step identity, schema, and duplicate-ID validation
- Empty-sequence and disabled-entry compatibility
- Runner-local atomic active-run gate
- Stable `ELAUNCH-RUN-001`
- Gate release across all terminal paths
- Sequential runner reuse after settlement
- Internal runner-to-root observation seam
- Explicit root-owned startup execution
- Root lifecycle advancement through validation and execution
- Root cancellation after executor settlement
- Destruction-driven cancellation and late-publication suppression
- Stable `ELAUNCH-LIFE-001` and `ELAUNCH-LIFE-002`
- Structured root preflight diagnostics with legacy direct-runner compatibility
- Successful launch stopping at `Transitioning`
- Public immutable per-step reports
- Public immutable completed, failed, and interrupted launch reports
- Report schema version `2`
- Internal single-use report builder
- Authority-filtered `LastReport`
- Exactly-once `LaunchFailed` and `LaunchInterrupted`
- Terminal state and report acceptance before event dispatch
- Defensive report copying and post-runtime readability
- Transition-pending success without false completion
- Project-owned immutable `LaunchDestination`
- Destination schema version `1`
- Configuration schema version `3`
- Historical schema 2 rejection without rewrite
- Immutable destination load status and result
- Public injectable initial destination loader
- Standalone Unity asynchronous destination loader
- Destination validation before startup-step side effects
- Stable `ELAUNCH-DEST-001` and `ELAUNCH-DEST-002`
- Destination progress while `Transitioning`
- Successful `Transitioning -> Completed` handoff
- Completed report schema version `2`
- Exactly-once `LaunchCompleted`
- Startup warning preservation in completed reports
- Automatic Unity `Start` launch
- Serialized automatic-start opt-out
- Public neutral `ILaunchStatusPresenter`
- Logging-free headless presentation fallback
- Serialized presenter-component resolution
- Accepted snapshot presentation before public progress events
- Finalized report presentation before public terminal events
- Stable `ELAUNCH-VIEW-001` and `ELAUNCH-VIEW-002`
- Presenter exception containment and destruction unbinding
- Duplicate-root automatic-start and presenter silence
- Separate uGUI presentation runtime assembly
- Separate presentation test assembly
- Public plain `EchoLaunchStatusView`
- Text-complete lifecycle state rendering
- Determinate slider and percentage
- Distinct indeterminate progress surface
- Step and elapsed-time display
- Terminal diagnostic and destination display
- Configurable visibility and clearing
- Missing-reference-safe projection
- Runtime remaining uGUI-free
- Project-owned splash sequence schema 1
- Immutable image splash entries
- Deterministic clock-driven splash player
- Minimum-display and skip-policy enforcement
- Reduced-motion fade removal
- Neutral and headless splash presenters
- Default uGUI image-splash projection
- Public project-routed skip request
- Implemented configuration schema 4
- Optional configured splash before startup steps
- Side-effect-free splash and startup preflight
- Stable splash preflight, playback, and headless diagnostics
- Shared root launch clock
- Splash clear before step presentation
- Startup completion before destination loading
- Cancellation during splash with exactly-once interruption
- Duplicate-root, automatic-start, and direct-scene splash containment
- Configuration and splash asset immutability
- Report schema 2 preserved
- Five hundred three passing Runtime Play Mode tests
- Twenty-seven passing EditMode prefab asset tests
- Stable neutral status-view and root prefab identities
- Complete serialized Canvas and presenter wiring
- Nested root-to-presenter prefab composition
- No project branding, input authority, TextMeshPro, or project asset dependency
- Temporary authoring helper removed before commit
- Read-only project snapshot and deterministic dry-run setup plan
- Fresh-plan-gated create-only Setup Apply
- Deterministic fingerprints and stale-plan rejection
- Shared single-active Apply/Repair protection
- Project-owned foundation creation and compatible reuse
- Build Settings mutation through explicit policy
- Compensating rollback and immutable result reporting
- Successful first Apply plus two `NoChanges` Apply reruns
- Separate proof-backed current-schema Setup Repair
- Exact asset and `.meta` backup, rollback, and retained-backup reporting
- Narrow configuration, destination, root-prefab, Boot-scene, and Build Settings reconciliation
- Successful first Repair plus two `NoChanges` Repair reruns
- Two hundred nine Editor setup, apply, and repair tests
- Twenty-five focused Validator tests
- Five focused Direct Scene Validator tests
- Twenty-four focused Direct Scene runtime tests
- Twenty-four focused Launch Simulator EditMode tests
- Two hundred ninety total passing EditMode tests
- Five hundred three passing Runtime Play Mode tests
- Seven hundred ninety-three total passing automated tests
- Stable setup diagnostics `ELAUNCH-SETUP-001` through `ELAUNCH-SETUP-017`
- Stable validation diagnostics `ELAUNCH-VAL-001` through `ELAUNCH-VAL-015`, with `009` active for Direct Scene safety
- Stable direct-entry diagnostics `ELAUNCH-DIRECT-001` through `ELAUNCH-DIRECT-003`

FL-M5-06 is implemented, automated-tested, manually accepted, documented, and pushed. FL-M5-07 is the active authority checkpoint and may add exactly one fully-authored importable Standalone Laboratory UPM sample plus a narrowly evidence-gated automatic Setup-candidate isolation correction if required. Schema migration, receipts, uninstall, crash-persistent recovery, automatic Direct Scene installation, build hooks, persistent-root policy, normal scene travel, player-build evidence, external adoption, and performance evidence remain outside the implemented boundary.
