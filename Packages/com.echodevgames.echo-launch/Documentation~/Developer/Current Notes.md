# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M4-04`
- Title: Splash Configuration Schema and Root Playback Integration
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.5.0
- ADR: EchoLaunch-ADR-002
- Implementation status: Complete and pushed
- Authority commit: `90aabd1`
- Implementation commit: `858808b`
- Previous documentation commit: `b36e04d`
- Documentation closeout: Pending adjacent commit
- Runtime Play Mode result: 479 passed, 0 failed, 0 ignored
- Compilation result: 0 errors, 0 compiler warnings

## Completed Result

Implemented:

- Configuration schema 4
- Optional `SplashSequence`
- `UseReducedMotionForSplash`
- Historical schema 3 rejection
- Side-effect-free `SplashSequencePreflight`
- Null and empty splash no-op paths
- Splash and startup validation before side effects
- Sequential root order: splash, startup steps, destination
- Shared launch clock
- Reduced-motion forwarding
- Visual/headless presenter resolution
- `ELAUNCH-SPLASH-001`
- `ELAUNCH-SPLASH-002`
- `ELAUNCH-SPLASH-003`
- Splash clear before step presentation
- Failure and cancellation settlement
- Internal successful splash-result retention
- Automatic-start route
- Duplicate-root silence
- Direct-scene consistency
- Configuration and splash immutability
- Report schema 2 preservation
- Twenty-eight focused tests
- One additional schema-history test

## Evidence Summary

- Runtime Play Mode: 479 passed, 0 failed, 0 ignored
- Compilation: 0 errors, 0 compiler warnings
- Implementation commit `858808b` pushed to `main` and `origin/main`
- Working tree clean after implementation push
- No compile or test correction bundle was required

## Schema Boundary

- Configuration schema is `4`.
- Splash sequence schema remains `1`.
- Destination schema remains `1`.
- Report schema remains `2`.
- Runtime migration remains prohibited.
- Editor migration remains unimplemented.

## Not Run

- Editor migration
- Startup presentation prefab
- Canvas art/layout
- Project input binding
- Direct-scene initializer tooling
- Standalone Laboratory scene
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Handoff Snapshot

FL-M4-04 implementation is complete and pushed in commit `858808b`.

First Light now validates and plays an optional configured splash before startup
steps, then loads the initial destination. The path is deterministic,
cancellation-aware, headless-safe, and proven through 479 passing tests.

The adjacent FL-M4-04 documentation closeout is the only active repository work.

Tentative next checkpoint: FL-M4-05 - Startup Presentation Prefab and Canvas
Assembly.
