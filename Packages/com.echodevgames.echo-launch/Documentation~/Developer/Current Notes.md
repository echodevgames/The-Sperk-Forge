
# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M4-04`
- Title: Splash Configuration Schema and Root Playback Integration
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.5.0
- ADR: EchoLaunch-ADR-002
- Status: Authority approved; runtime implementation locked until authority commit
- Repository baseline: `b36e04d`
- Last implementation commit: `f997a9a`
- Runtime Play Mode baseline: 450 passed, 0 failed, 0 ignored
- Compilation baseline: 0 errors, 0 compiler warnings

## Approved Contract

### Configuration

- `EchoLaunchConfiguration.CurrentSchemaVersion` becomes `4`.
- Schema 4 adds optional `SplashSequence`.
- Schema 4 adds `UseReducedMotionForSplash`.
- Null reference means no splash.
- Empty valid sequence is a legal no-op.
- Runtime accepts only the current schema.
- Runtime never migrates or repairs assets.

### Root Order

```text
preflight
    -> bind presentation
    -> optional splash
    -> startup steps
    -> destination
    -> completed handoff
```

Splash and startup steps are sequential.

### Failure and Cancellation

- Invalid assigned sequence: `ELAUNCH-SPLASH-001`.
- Unexpected playback failure: `ELAUNCH-SPLASH-002`.
- Missing visual presenter: `ELAUNCH-SPLASH-003` and headless continuation.
- Root cancellation during splash: existing `ELAUNCH-LIFE-001`.
- Splash failure/interruption prevents steps and destination loading.
- Terminal report and events remain exactly once.

### Reporting

- Report schema remains `2`.
- Splash duration contributes to total launch elapsed time.
- Existing final-result code/message carries splash failure.
- No splash metrics are added to `LaunchReport`.

## Implementation Lock

Do not change Runtime code until the authority commit is pushed.

Required authority commit:

```text
echo-launch: approve FL-M4-04 splash schema 4 and root order
```

## Expected Runtime Span

- Configuration schema-4 binding
- Optional splash preflight
- Visual/headless presenter resolution
- Root-owned deterministic splash playback
- Reduced-motion forwarding
- Splash-before-steps ordering
- Cancellation/failure settlement
- Duplicate-root and automatic-start proof
- Direct-scene contract proof
- Configuration/splash immutability proof

## Explicit Exclusions

- Editor migration
- Runtime migration
- Report schema change
- Concurrent splash and startup steps
- Prefab art
- Input binding
- EchoInput/EchoSettings bridge
- Direct-scene initializer implementation
- Test Lab scenes
- Player builds

## Handoff Snapshot

FL-M4-03 is fully closed at `b36e04d`.

FL-M4-04 authority is prepared through specification v1.5.0, ADR-002, and the
approved Checkpoint Build Plan.

Runtime implementation begins only after the authority commit is confirmed.
