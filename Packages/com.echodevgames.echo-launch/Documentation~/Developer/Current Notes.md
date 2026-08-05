# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M3-06`
- Title: Root-Owned Startup Run and Lifecycle Advancement
- Package version: `0.1.0`
- Implementation status: Complete and pushed
- Implementation commit: `e0e9645`
- Previous documentation commit: `485a09f`
- Documentation closeout: Pending adjacent commit
- Runtime Play Mode result: 311 passed, 0 failed, 0 ignored
- Compilation result: 0 errors, 0 compiler warnings

## Completed Result

Implemented:

- Explicit internal `EchoLaunchRoot.StartLaunchAsync`
- Public cooperative `EchoLaunchRoot.CancelLaunch`
- Root-local atomic active-launch gate
- Stable `ELAUNCH-LIFE-001` and `ELAUNCH-LIFE-002`
- Internal `IStartupSequenceObserver`
- Internal `StartupStepProgressRelay`
- Structured `StartupSequencePreflightException`
- Root publication of validation, running, step progress, failure, interruption, and transition-pending snapshots
- Successful and warning-only runs stopping at `Transitioning`
- Blocking and preflight failures reaching `Failed`
- Cancellation reaching `Interrupted` after executor settlement
- Destruction-driven cancellation and late-publication suppression
- Duplicate-root start and cancellation rejection
- Root run-result retention
- Legacy direct-runner exact exception compatibility
- Twenty-three new Runtime Play Mode tests

## Evidence Summary

### Final Pass

- Runtime Play Mode: 311 passed, 0 failed, 0 ignored
- New root lifecycle fixture: 23 passed
- Compilation: 0 errors, 0 compiler warnings
- Implementation commit `e0e9645` pushed to `main` and `origin/main`
- Working tree clean after implementation push

### Compatibility Detour

Initial full-suite run:

- 296 passed
- 15 failed
- 0 ignored

Cause:

- Retained direct-runner tests required exact `InvalidOperationException`.
- Structured preflight exceptions inherited from that type but did not satisfy NUnit exact-type assertions.

Correction:

- Legacy three-argument runner overload restores exact historical exception type.
- Root-owned observer overload retains structured diagnostic data.

### Expected Diagnostics

Retained tests intentionally generate:

    ELAUNCH-ROOT-001
    ELAUNCH-EVENT-001

These warnings are expected runtime evidence, not compiler warnings or failures.

### Not Run

- Automatic startup from Unity callbacks
- Immutable launch reports
- Public terminal launch events
- Public step lifecycle events
- Initial destination selection or loading
- `Completed` publication after handoff
- Splash or status presentation
- Direct-scene initialization
- Editor setup and repair
- Standalone Laboratory
- Player builds
- Performance measurements

## Changed Files

Modified runtime:

- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/Execution/StartupSequencePreflight.cs`
- `Runtime/Execution/StartupSequenceRunner.cs`

New runtime:

- `Runtime/Execution/IStartupSequenceObserver.cs`
- `Runtime/Execution/StartupSequencePreflightException.cs`
- `Runtime/Execution/StartupStepProgressRelay.cs`
- Unity-generated `.meta` files

Automated tests:

- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- Unity-generated `.meta`

Checkpoint plan:

- `Plan Documentation/Checkpoint Build Plans/FL-M3-06_Root-Owned_Startup_Run_and_Lifecycle_Advancement_Checkpoint_Build_Plan.md`

## Handoff Snapshot

FL-M3-06 implementation is complete and pushed in commit `e0e9645`.

The authoritative root can explicitly own, observe, cancel, settle, and project one startup-sequence run through the approved lifecycle.

Success stops at `Transitioning`; automatic startup, immutable reporting, public terminal events, and destination handoff remain pending.

The adjacent FL-M3-06 documentation closeout is the only active repository work.

Tentative next checkpoint: FL-M3-07 - Immutable Launch Report and Public Terminal Events.
