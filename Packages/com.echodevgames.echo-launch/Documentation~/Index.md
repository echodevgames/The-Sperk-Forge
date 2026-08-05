# First Light - Package Documentation

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

## Current Status

- Package version: `0.1.0`
- Development stage: Runtime contracts established; execution not yet implemented
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
  - `FL-M2-03`
  - `FL-M2-04`
  - `FL-M2-05`
  - `FL-M2-06`
  - `FL-M2-07`
  - `FL-M2-08`
- Current implemented boundary: authority, vocabulary, session state, guarded publication, isolated notifications, launch configuration, ordered startup definitions, authored policy, immutable progress/context, and fresh executor contracts
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
- One hundred sixty-nine passing Runtime Play Mode tests

The sequence runner, executor invocation, timeout handling, retries, preflight, and lifecycle automation remain outside the implemented scope.
