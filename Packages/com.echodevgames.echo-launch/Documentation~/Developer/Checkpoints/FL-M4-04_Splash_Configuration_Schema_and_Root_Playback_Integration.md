# FL-M4-04 - Splash Configuration Schema and Root Playback Integration

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.5.0
- ADR: EchoLaunch-ADR-002
- Checkpoint: `FL-M4-04`
- Milestone: M4 - Startup Entry and Presentation
- Implementation status: Complete and pushed
- Authority commit: `90aabd1`
- Implementation commit: `858808b`
- Previous documentation commit: `b36e04d`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Bind the standalone deterministic splash system into configuration schema 4 and
the authoritative root lifecycle without changing launch report schema 2.

## Implemented Contract

### Configuration Schema 4

`EchoLaunchConfiguration.CurrentSchemaVersion` is now `4`.

Schema 4 adds:

```csharp
SplashSequence SplashSequence
bool UseReducedMotionForSplash
```

A null splash reference intentionally omits the splash phase.

Historical schema 3 assets remain unsupported at runtime and are not rewritten.

### Splash Preflight

Added internal:

```csharp
SplashSequencePreflight
```

It accepts null as legal omission, accepts empty valid sequences, validates
assigned sequence identity/schema/entries/images/timing/duplicate IDs, and
converts invalid authored data to `ELAUNCH-SPLASH-001`.

It runs before splash frames, startup executors, and destination work.

The root also validates the startup sequence before splash playback.

### Root Phase Order

```text
validation
    -> optional splash
    -> startup steps
    -> initial destination
    -> completed handoff
```

Splash and startup steps are sequential.

The splash surface clears before startup-step presentation.

### Shared Clock

The root injects one `ILaunchClock` into splash playback, startup execution, and
launch report timing.

Successful splash time contributes to total launch elapsed time.

### Presenter Resolution

When the active status presenter implements `IImageSplashPresenter`, it receives
the configured splash.

When a nonempty splash is configured without a visual splash presenter:

- The root emits `ELAUNCH-SPLASH-003`.
- Playback continues through `NullImageSplashPresenter`.
- Authored timing remains intact.

### Failure and Cancellation

Invalid splash preflight:

```text
ELAUNCH-SPLASH-001
```

Unexpected playback/presenter/clock failure:

```text
ELAUNCH-SPLASH-002
```

Root cancellation during splash:

```text
ELAUNCH-LIFE-001
```

Failure or interruption prevents startup steps and destination loading.

Terminal report and event settlement remain exactly once.

### Report Boundary

`LaunchReport.CurrentSchemaVersion` remains `2`.

No splash-specific report fields were added.

The root retains the latest successful `SplashPlaybackResult` internally for
focused lifecycle evidence.

### Automatic, Duplicate, and Direct-Scene Paths

Verified:

- Automatic Unity `Start` uses the same splash route.
- Duplicate roots present no additional splash.
- Direct-scene launch mode uses the same schema-4 splash contract.

## Files

Modified:

- `Runtime/Configuration/EchoLaunchConfiguration.cs`
- `Runtime/Core/EchoLaunchRoot.cs`
- `Tests/Runtime/PlayMode/LaunchConfigurationBindingTests.cs`
- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs`

Created:

- `Runtime/Splash/SplashSequencePreflight.cs`
- `Runtime/Splash/SplashSequencePreflight.cs.meta`
- `Tests/Runtime/PlayMode/EchoLaunchRootSplashLifecycleTests.cs`
- `Tests/Runtime/PlayMode/EchoLaunchRootSplashLifecycleTests.cs.meta`

## Compile Evidence

- Errors: `0`
- Compiler warnings: `0`

## Test Evidence

Focused FL-M4-04 root fixture:

- Passed: `28`
- Failed: `0`
- Ignored: `0`

Additional retained schema-history test:

- Passed: `1`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `479`
- Failed: `0`
- Ignored: `0`

Verified configuration binding, preflight, phase ordering, reduced motion,
headless fallback, project-routed skip, elapsed-time inclusion, result retention,
failure blocking, cancellation, duplicate-root silence, automatic start,
direct-scene mode, asset immutability, and report-schema preservation.

## Evidence Not Yet Run

- Editor migration from schema 3 to 4
- Package-supplied startup presentation prefab
- Canvas hierarchy and art pass
- Project input binding
- Direct-scene initializer tooling
- Standalone Laboratory scene
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Exclusions Preserved

- Runtime migration
- Silent asset repair
- Report schema change
- Splash-specific report metrics
- Concurrent splash and startup steps
- Prefab YAML
- EchoInput or EchoSettings bridge
- Legal-splash semantics
- Video or custom animation adapters
- Editor setup and repair
- Test Lab scenes
- Package version change

## Closure Result

FL-M4-04 implementation is complete in commit `858808b`.

The implementation compiles with 0 errors and 0 compiler warnings.

All 479 Runtime Play Mode tests pass with 0 failed and 0 ignored.

The checkpoint is ready for its adjacent documentation closeout.

Tentative next checkpoint: FL-M4-05 - Startup Presentation Prefab and Canvas
Assembly.
