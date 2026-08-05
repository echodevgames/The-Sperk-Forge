# First Light - Package Documentation

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

## Current Status

- Package version: `0.1.0`
- Development stage: Policy-aware immediate execution implemented; timeout and lifecycle integration pending
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
- Current implemented boundary: authority, vocabulary, session state, guarded publication, isolated notifications, launch configuration, ordered definitions, authored policy, runtime attempt state, policy-aware immediate traversal, and structured exception containment
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
- Two hundred thirty-one passing Runtime Play Mode tests

Timeout handling, retries, cancellation orchestration, reports, preflight, root integration, and lifecycle automation remain outside the implemented scope.
