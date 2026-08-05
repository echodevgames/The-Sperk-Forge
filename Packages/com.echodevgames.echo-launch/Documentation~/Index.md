# First Light - Package Documentation

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

## Current Status

- Package version: `0.1.0`
- Development stage: Immutable failed/interrupted reports and public terminal events implemented; destination handoff and successful completion pending
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
- Current implemented boundary: authority, vocabulary, session state, guarded publication, isolated notifications, launch configuration, ordered definitions, authored policy, runtime attempt state, policy-aware timed traversal, structured exception containment, monotonic deadlines, cooperative timeout cancellation, multi-frame Unity async proof, structured caller cancellation, complete sequence preflight, runner re-entry protection, explicit root-owned startup, cooperative root cancellation, destruction-safe settlement, lifecycle projection through `Transitioning`, immutable failed/interrupted reports, authority-filtered `LastReport`, and exactly-once failed/interrupted terminal events
- Unity baseline: `6000.3.8f1`

## User Documentation

- [Installation](User/Installation.md)
- [Quick Start](User/Quick%20Start.md)

## Developer Documentation

- [Architecture](Developer/Architecture.md)
- [Current Notes](Developer/Current%20Notes.md)
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
- Public immutable failed/interrupted launch reports
- Report schema version `1`
- Internal single-use report builder
- Authority-filtered `LastReport`
- Exactly-once `LaunchFailed` and `LaunchInterrupted`
- Terminal state and report acceptance before event dispatch
- Defensive report copying and post-runtime readability
- Transition-pending success without false completion
- Three hundred thirty-six passing Runtime Play Mode tests

Retries, successful report finalization, `LaunchCompleted`, dependency-graph validation, automatic startup, presentation, and destination handoff remain outside the implemented scope.
