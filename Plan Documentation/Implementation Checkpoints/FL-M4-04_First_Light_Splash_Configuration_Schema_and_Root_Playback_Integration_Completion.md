# FL-M4-04 - First Light Splash Configuration and Root Playback Completion

## Status

- Checkpoint: `FL-M4-04`
- Milestone: M4 - Startup Entry and Presentation
- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.5.0
- ADR: EchoLaunch-ADR-002
- Authority commit: `90aabd1`
- Implementation result: Complete and pushed
- Implementation commit: `858808b`
- Previous documentation commit: `b36e04d`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Implemented Scope

- Configuration schema 4
- Optional splash-sequence binding
- Reduced-motion configuration binding
- Historical schema 3 rejection
- Splash preflight
- Startup preflight before splash side effects
- Sequential splash, startup-step, and destination order
- Shared launch clock
- Visual/headless splash presenter resolution
- Stable splash diagnostics
- Splash clear before startup-step presentation
- Failure blocking later phases
- Root cancellation during splash
- Successful splash-result retention
- Total elapsed time including splash
- Duplicate-root splash silence
- Automatic-start splash path
- Direct-scene contract consistency
- Configuration and splash immutability
- Report schema 2 preservation
- Twenty-eight focused tests
- One retained schema-history test

## Evidence

- Compilation errors: `0`
- Compiler warnings: `0`
- Final Runtime Play Mode tests passed: `479`
- Final Runtime Play Mode tests failed: `0`
- Final Runtime Play Mode tests ignored: `0`
- Focused FL-M4-04 tests passed: `28`
- Additional schema-history test passed: `1`
- Configuration schema 4: Pass
- Splash preflight: Pass
- Startup-before-splash preflight protection: Pass
- Splash-before-step ordering: Pass
- Step-before-destination ordering: Pass
- Reduced-motion forwarding: Pass
- Headless fallback: Pass
- Project-routed skip: Pass
- Failure blocking: Pass
- Cancellation settlement: Pass
- Duplicate-root silence: Pass
- Automatic-start routing: Pass
- Direct-scene routing: Pass
- Configuration immutability: Pass
- Splash immutability: Pass
- Report schema 2 preservation: Pass

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

## Evidence Not Yet Run

- Editor migration from schema 3 to 4
- Startup presentation prefab
- Canvas art/layout pass
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
- Report schema expansion
- Concurrent splash and startup steps
- Prefab YAML
- EchoInput/EchoSettings integration
- Legal-splash semantics
- Video/custom animation adapters
- Editor setup/repair
- Test Lab scenes
- Package version change

## Completion Decision

FL-M4-04 implementation is complete in `858808b`.

The repository was clean and synchronized after the implementation push.

The checkpoint is ready for the adjacent documentation closeout commit.

Tentative next checkpoint: FL-M4-05 - Startup Presentation Prefab and Canvas
Assembly.
