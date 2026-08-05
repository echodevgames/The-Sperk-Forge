# FL-M3-06 - First Light Root-Owned Startup Run and Lifecycle Advancement Completion

## Status

- Checkpoint: `FL-M3-06`
- Milestone: M3 - Startup Sequence
- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
- Implementation result: Complete and pushed
- Implementation commit: `e0e9645`
- Previous documentation commit: `485a09f`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Implemented Scope

- Explicit root-owned startup-sequence run
- Root-local active-launch gate
- Public cooperative cancellation command
- Stable `ELAUNCH-LIFE-001`
- Stable `ELAUNCH-LIFE-002`
- Internal runner observation interface
- Progress relay from execution state to root
- Structured preflight diagnostic exception
- Validation, running, failure, interruption, and transition-pending lifecycle mapping
- Step-start, progress, and completion snapshot projection
- Destruction cancellation and late-publication suppression
- Duplicate-root control rejection
- Internal last-run result retention
- Legacy direct-runner exact exception compatibility
- Twenty-three new Runtime Play Mode tests

## Evidence

- Compilation errors: `0`
- Compilation warnings: `0`
- New root lifecycle tests passed: `23`
- Final Runtime Play Mode tests passed: `311`
- Final Runtime Play Mode tests failed: `0`
- Final Runtime Play Mode tests ignored: `0`
- Initial compatibility run: 296 passed, 15 failed, 0 ignored
- Approved state order: Pass
- Success stopping at `Transitioning`: Pass
- Preflight and blocking failure mapping: Pass
- Cancellation settlement and interruption: Pass
- Destruction safety: Pass
- Duplicate-root and re-entry guards: Pass
- Authored asset immutability: Pass
- Package independence: Preserved

## Compatibility Correction

The structured preflight exception is an `InvalidOperationException` subtype.

Fifteen retained tests required the exact base type through NUnit `Assert.Throws<T>`.

The legacy runner overload now rethrows exact `InvalidOperationException`, while the root observer overload retains structured diagnostic data.

The final complete rerun passed 311/0/0.

## Expected Runtime Diagnostics

Retained tests intentionally emitted:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These yellow diagnostics are expected and do not represent compiler warnings or test failures.

## Files

Created:

- `Runtime/Execution/IStartupSequenceObserver.cs`
- `Runtime/Execution/IStartupSequenceObserver.cs.meta`
- `Runtime/Execution/StartupSequencePreflightException.cs`
- `Runtime/Execution/StartupSequencePreflightException.cs.meta`
- `Runtime/Execution/StartupStepProgressRelay.cs`
- `Runtime/Execution/StartupStepProgressRelay.cs.meta`
- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs.meta`
- `Plan Documentation/Checkpoint Build Plans/FL-M3-06_Root-Owned_Startup_Run_and_Lifecycle_Advancement_Checkpoint_Build_Plan.md`

Modified:

- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/Execution/StartupSequencePreflight.cs`
- `Runtime/Execution/StartupSequenceRunner.cs`

## Exclusions Preserved

- Automatic Unity-callback startup
- Immutable launch report
- Public terminal launch events
- Public step lifecycle events
- Initial destination selection or scene loading
- `Transitioning -> Completed` handoff
- Splash and status presentation
- Direct-scene initialization
- Persistent-root policy
- Editor setup and repair
- Standalone Laboratory
- Player builds
- Performance claims

## Completion Decision

FL-M3-06 implementation is complete in `e0e9645`.

The repository was clean and synchronized after the implementation push.

The checkpoint is ready for the adjacent documentation closeout commit.

Tentative next checkpoint: FL-M3-07 - Immutable Launch Report and Public Terminal Events.
