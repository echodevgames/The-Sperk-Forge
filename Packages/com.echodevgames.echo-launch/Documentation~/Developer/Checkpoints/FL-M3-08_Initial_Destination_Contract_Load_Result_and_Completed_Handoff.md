# FL-M3-08 - Initial Destination Contract, Load Result, and Completed Handoff

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Package ADR: EchoLaunch-ADR-001 v1.0.0
- Checkpoint: `FL-M3-08`
- Milestone: M3 - Startup Sequence Runtime
- Authority commit: `eb9cc49`
- Implementation status: Complete and pushed
- Implementation commit: `114ac91`
- Previous documentation commit: `f76b9df`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Complete the first root-owned standalone handoff boundary after startup-sequence success.

## Implemented Contract

### Project-Owned Destination

`LaunchDestination` is a public project-owned immutable `ScriptableObject`.

It owns:

- Canonical lowercase 32-character hexadecimal destination ID
- Destination schema version `1`
- Trimmed nonblank display name
- Runtime-safe `Assets/.../*.unity` scene path
- Create menu path under `EchoDevGames/First Light/Launch Destination`

Runtime reads but never repairs, migrates, or rewrites the asset.

### Configuration Schema 3

`EchoLaunchConfiguration.CurrentSchemaVersion` is now `3`.

Schema 3 adds:

```csharp
[SerializeField]
private LaunchDestination initialDestination;
```

Schema 2 remains the historical startup-sequence-only shape and is rejected without runtime rewrite.

### Destination Loader Contract

Added:

- `InitialDestinationLoadStatus`
- Immutable `InitialDestinationLoadResult`
- Public `IInitialDestinationLoader`
- Internal optional `IInitialDestinationPreflightValidator`
- Internal `InitialDestinationProgressRelay`
- Standalone `UnityInitialDestinationLoader`

The default loader uses Unity asynchronous single-scene loading, validates build-loadability, reports normalized progress, waits for settlement, and confirms the requested destination is active before success.

### Root Destination Preflight

Before startup-step side effects, the root validates:

- Configuration schema 3
- Assigned destination
- Destination identity
- Destination schema 1
- Display label
- Scene path
- Assigned loader
- Loader-specific preflight

Destination preflight failure uses `ELAUNCH-DEST-001`.

The startup-sequence runner remains destination-neutral.

### Completed Handoff

Successful order:

1. Startup sequence settles successfully or with warnings.
2. Root publishes `Transitioning`.
3. Loader begins exactly once.
4. Destination progress publishes while state remains `Transitioning`.
5. Destination activation is confirmed.
6. Root publishes `Completed`.
7. Completed immutable report finalizes.
8. `LastReport` stores the exact report.
9. `LaunchCompleted` dispatches exactly once.

No completion is published before destination activation success.

### Completed Reports

`LaunchReport.CurrentSchemaVersion` is now `2`.

Completed reports include:

- Canonical destination ID
- Destination display name
- Sequence accounting
- Immutable per-step reports
- Warning/failure/cancellation summaries
- Final successful destination activation result

Startup warnings remain preserved in report warning counts and step reports even though the final lifecycle result is successful.

### Failure, Cancellation, and Destruction

- Destination-load failure maps to `ELAUNCH-DEST-002`.
- Null or mismatched loader results are contained as destination failures.
- Cancellation before loading prevents invocation.
- Cancellation during an injected load waits for settlement, then interrupts.
- Root destruction suppresses unsafe late completion events.
- Existing failed and interrupted report behavior remains intact.

## Files

Modified runtime:

- `Runtime/Configuration/EchoLaunchConfiguration.cs`
- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/Reports/LaunchReport.cs`
- `Runtime/Reports/LaunchReportBuilder.cs`

Created runtime:

- `Runtime/SceneLoading.meta`
- `Runtime/SceneLoading/IInitialDestinationLoader.cs`
- `Runtime/SceneLoading/IInitialDestinationLoader.cs.meta`
- `Runtime/SceneLoading/InitialDestinationLoadResult.cs`
- `Runtime/SceneLoading/InitialDestinationLoadResult.cs.meta`
- `Runtime/SceneLoading/InitialDestinationLoadStatus.cs`
- `Runtime/SceneLoading/InitialDestinationLoadStatus.cs.meta`
- `Runtime/SceneLoading/InitialDestinationProgressRelay.cs`
- `Runtime/SceneLoading/InitialDestinationProgressRelay.cs.meta`
- `Runtime/SceneLoading/LaunchDestination.cs`
- `Runtime/SceneLoading/LaunchDestination.cs.meta`
- `Runtime/SceneLoading/UnityInitialDestinationLoader.cs`
- `Runtime/SceneLoading/UnityInitialDestinationLoader.cs.meta`

Modified tests:

- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- `Tests/Runtime/PlayMode/LaunchConfigurationBindingTests.cs`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs`

Created tests:

- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs`
- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs.meta`

## Compile Corrections

Three bounded test-fixture corrections were required:

1. Three nonexistent `LaunchProgressSnapshot.IsIndeterminate` references were changed to `IsProgressIndeterminate`.
2. The retained warning-completion test was updated to expect the successful destination-activation lifecycle result while verifying preserved report warning data.
3. One nonexistent `LaunchStepReport.FinalStatus` assertion was changed to `LaunchStepReport.Status`.

No production runtime change was required by these corrections.

Final compilation:

- Errors: `0`
- Compiler warnings: `0`

## Test Evidence

Final Runtime Play Mode suite:

- Passed: `380`
- Failed: `0`
- Ignored: `0`

New destination and completed-handoff fixture:

- Passed: `37`
- Failed: `0`
- Ignored: `0`

Expanded configuration and destination binding fixture:

- Passed: `22`
- Failed: `0`
- Ignored: `0`

Intermediate run:

- Passed: `379`
- Failed: `1`
- Cause: retained warning-completion expectation
- Resolution: test contract corrected; final suite green

## Expected Diagnostics

Tests intentionally emit runtime diagnostics including:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These are expected proof and are not compiler warnings or test failures.

## Evidence Not Yet Run

- Real Boot-to-destination Standalone Laboratory scene activation
- Player builds
- Separate clean-project installation
- External project adoption
- Performance measurements
- Editor migration from configuration schema 2 to 3

## Exclusions Preserved

- Automatic startup from `Awake`, `Start`, or Unity scene callbacks
- Splash and status presentation
- Direct-scene initialization
- Persistent-root lifetime policy
- Normal mid-game scene travel
- Conditional destination providers
- Save-aware destination selection
- EchoSceneFlow bridge
- Additive loading
- Loading-screen ownership
- Editor setup, repair, and migration
- Report export
- Public step lifecycle events

## Closure Result

FL-M3-08 implementation is complete in commit `114ac91`.

The implementation compiles with 0 errors and 0 compiler warnings.

All 380 Runtime Play Mode tests pass with 0 failed and 0 ignored.

The checkpoint is ready for its adjacent documentation closeout.

Tentative next checkpoint: FL-M4-01 - Automatic Root Start Gate and Plain Status Presenter Contract.
