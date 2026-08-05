# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M3-08`
- Title: Initial Destination Contract, Load Result, and Completed Handoff
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Package ADR: EchoLaunch-ADR-001 v1.0.0
- Authority commit: `eb9cc49`
- Implementation status: Complete and pushed
- Implementation commit: `114ac91`
- Previous documentation commit: `f76b9df`
- Documentation closeout: Pending adjacent commit
- Runtime Play Mode result: 380 passed, 0 failed, 0 ignored
- Compilation result: 0 errors, 0 compiler warnings

## Completed Result

Implemented:

- Project-owned immutable `LaunchDestination`
- Destination schema version 1
- Configuration schema version 3
- Serialized initial destination binding
- Historical schema 2 rejection without runtime rewrite
- Immutable load status and result contract
- Injectable `IInitialDestinationLoader`
- Standalone Unity asynchronous scene loader
- Destination preflight before startup-step side effects
- Stable `ELAUNCH-DEST-001` and `ELAUNCH-DEST-002`
- Destination progress while `Transitioning`
- Successful `Transitioning -> Completed` handoff
- Report schema version 2
- Destination metadata in completed reports
- Exactly-once `LaunchCompleted`
- Completed state and report acceptance before event dispatch
- Completion-listener isolation
- Failure, cancellation, duplicate-root, and destruction containment
- Startup warning preservation in completed reports
- Thirty-seven new destination/handoff tests
- Seven additional configuration/destination-binding tests

## Evidence Summary

### Final Pass

- Runtime Play Mode: 380 passed, 0 failed, 0 ignored
- Destination and completed-handoff fixture: 37 passed
- Configuration and destination binding fixture: 22 passed
- Compilation: 0 errors, 0 compiler warnings
- Implementation commit `114ac91` pushed to `main` and `origin/main`
- Working tree clean after implementation push

### Corrections

- Replaced three test references to `IsIndeterminate` with `IsProgressIndeterminate`.
- Updated one retained warning-completion expectation to distinguish successful destination activation from preserved startup warning data.
- Replaced one test reference to `FinalStatus` with `Status`.
- No production runtime behavior changed for these fixes.

### Expected Diagnostics

Retained and new tests intentionally generate:

    ELAUNCH-ROOT-001
    ELAUNCH-EVENT-001

These are expected runtime diagnostics, not compiler warnings or failures.

### Not Run

- Real Boot-to-destination Standalone Laboratory activation
- Automatic Unity-callback startup
- Splash or status presentation
- Direct-scene initialization
- Persistent-root policy
- Normal mid-game scene travel
- Editor schema migration
- Setup and repair tooling
- Report export
- Player builds
- Separate clean-project installation
- External project adoption
- Performance measurements

## Changed Files

Modified runtime:

- `Runtime/Configuration/EchoLaunchConfiguration.cs`
- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/Reports/LaunchReport.cs`
- `Runtime/Reports/LaunchReportBuilder.cs`

New runtime:

- `Runtime/SceneLoading.meta`
- `Runtime/SceneLoading/IInitialDestinationLoader.cs`
- `Runtime/SceneLoading/InitialDestinationLoadResult.cs`
- `Runtime/SceneLoading/InitialDestinationLoadStatus.cs`
- `Runtime/SceneLoading/InitialDestinationProgressRelay.cs`
- `Runtime/SceneLoading/LaunchDestination.cs`
- `Runtime/SceneLoading/UnityInitialDestinationLoader.cs`
- Unity-generated script `.meta` files

Automated tests:

- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs`
- `Tests/Runtime/PlayMode/LaunchConfigurationBindingTests.cs`
- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs`
- Unity-generated `.meta`

## Handoff Snapshot

FL-M3-08 implementation is complete and pushed in commit `114ac91`.

One validated project-owned initial destination can now complete after startup-sequence success, finalize one immutable completed report, and publish `LaunchCompleted` exactly once.

The adjacent FL-M3-08 documentation closeout is the only active repository work.

Tentative next checkpoint: FL-M4-01 - Automatic Root Start Gate and Plain Status Presenter Contract.
