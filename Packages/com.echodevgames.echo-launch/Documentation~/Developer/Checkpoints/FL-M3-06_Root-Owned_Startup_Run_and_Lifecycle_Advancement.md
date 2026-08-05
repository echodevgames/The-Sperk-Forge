# FL-M3-06 - Root-Owned Startup Run and Lifecycle Advancement

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
- Checkpoint: `FL-M3-06`
- Milestone: M3 - Startup Sequence
- Implementation status: Complete and pushed
- Implementation commit: `e0e9645`
- Previous documentation commit: `485a09f`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Connect the proven sequence runner to the authoritative root through one explicit launch command, then project validated runner observations into the approved launch lifecycle without introducing automatic startup, reports, presentation, or destination loading.

## Implemented Contract

### Explicit Root Ownership

`EchoLaunchRoot.StartLaunchAsync()` now owns one startup-sequence run.

The method is internal and explicit. `Awake`, `Start`, and scene callbacks do not invoke it.

### Approved Lifecycle Projection

One successful root-owned run publishes:

    AuthorityClaimed
        -> Validating
            -> Running
                -> Transitioning

Success deliberately stops at `Transitioning`. `Completed` remains reserved for the later initial-destination handoff.

Blocking and unexpected failures publish `Failed`.

Root or caller cancellation publishes `Interrupted`.

### Observation Seam

`IStartupSequenceObserver` keeps the runner neutral while forwarding:

- Sequence validation
- Step start
- Accepted step progress
- Step completion

`StartupStepProgressRelay` records progress before forwarding it.

### Structured Preflight Diagnostics

`StartupSequencePreflightException` preserves stable diagnostic code and failure message for root-owned failure publication.

The retained three-argument runner overload converts that structured exception back to exact `InvalidOperationException`, preserving the historical direct-runner contract.

### Root Start Gate

One atomic root-local active-launch gate prevents overlapping root starts.

Rejected start attempts use:

    ELAUNCH-LIFE-002

Settled and terminally advanced sessions cannot restart.

Duplicate roots cannot start or cancel the authoritative run.

### Cooperative Cancellation

`CancelLaunch(reason)`:

- Requires the active authoritative root
- Accepts only the first request
- Normalizes blank reasons
- Requests cooperative cancellation
- Waits for executor settlement
- Publishes `Interrupted` once

Interruption uses:

    ELAUNCH-LIFE-001

### Destruction Safety

Destroying an active root requests cancellation, allows settlement, suppresses late publication, clears events, and releases authority.

### Data and Dependency Safety

FL-M3-06 adds:

- No public serialized field or schema change
- No peer-package dependency
- No Editor runtime dependency
- No scene or prefab requirement
- No authored ScriptableObject mutation

## Authorized Runtime Files

Modified:

- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/Execution/StartupSequencePreflight.cs`
- `Runtime/Execution/StartupSequenceRunner.cs`

Created:

- `Runtime/Execution/IStartupSequenceObserver.cs`
- `Runtime/Execution/StartupSequencePreflightException.cs`
- `Runtime/Execution/StartupStepProgressRelay.cs`
- Required Unity `.meta` files

Tests:

- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- Required Unity `.meta`

Plan:

- `Plan Documentation/Checkpoint Build Plans/FL-M3-06_Root-Owned_Startup_Run_and_Lifecycle_Advancement_Checkpoint_Build_Plan.md`

## Test Evidence

New root lifecycle fixture:

- Passed: `23`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `311`
- Failed: `0`
- Ignored: `0`

Compilation:

- Errors: `0`
- Compiler warnings: `0`

## Compatibility Correction

The first full FL-M3-06 run produced:

- Passed: `296`
- Failed: `15`
- Ignored: `0`

All fifteen failures were retained exact-type assertions. `StartupSequencePreflightException` derives from `InvalidOperationException`, but NUnit `Assert.Throws<T>` required the exact historical type.

The bounded correction changed only the legacy three-argument runner overload:

- Direct legacy calls receive exact `InvalidOperationException`
- Root-owned observer calls retain structured `StartupSequencePreflightException`

The complete suite then passed 311/0/0.

## Expected Diagnostics

Retained tests intentionally produce:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These are expected warning diagnostics and do not count as compiler warnings or test failures.

## Explicit Exclusions

Not implemented:

- Automatic startup from Unity callbacks
- Immutable `LaunchReport`
- Public `LaunchFailed`, `LaunchInterrupted`, or `LaunchCompleted`
- Public step lifecycle events
- Initial destination selection or scene loading
- `Transitioning -> Completed` handoff
- Splash or status presentation
- Direct-scene initialization
- Persistent-root lifetime policy
- Editor setup or repair tools
- Standalone Laboratory
- Player builds
- Performance claims

## Closure Result

The implementation compiles cleanly and all 311 Runtime Play Mode tests pass.

Implementation commit `e0e9645` is synchronized on `main` and `origin/main`.

FL-M3-06 is ready for its adjacent documentation closeout.

The tentative next checkpoint is FL-M3-07 - Immutable Launch Report and Public Terminal Events.
