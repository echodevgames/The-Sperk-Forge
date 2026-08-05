# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M3-08`
- Title: Initial Destination Contract, Load Result, and Completed Handoff
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Package ADR: EchoLaunch-ADR-001 v1.0.0
- Authority status: Approved; commit pending
- Starting implementation commit: `a6f6544`
- Starting documentation commit: `f76b9df`
- Runtime Play Mode baseline: 336 passed, 0 failed, 0 ignored
- Compilation baseline: 0 errors, 0 compiler warnings

## Approved Decision

- `LaunchDestination` is a standalone project-owned ScriptableObject.
- Destination schema begins at version 1.
- `EchoLaunchConfiguration` advances from schema 2 to schema 3.
- Schema 2 remains the historical startup-sequence-only shape.
- Schema 3 adds one serialized initial destination reference.
- Runtime blocks older/unknown schema.
- Runtime does not silently migrate or rewrite project assets.
- Editor migration from schema 2 to 3 is later work.
- Normal mid-game scene travel remains outside EchoLaunch.

## FL-M3-08 Intended Runtime Outcome

- Validate the destination before startup-step side effects.
- Execute the startup sequence.
- Transition through one injected initial destination loader.
- Publish meaningful transition progress.
- Confirm destination activation.
- Advance `Transitioning -> Completed`.
- Finalize one successful immutable `LaunchReport`.
- Store it in `LastReport`.
- Dispatch `LaunchCompleted` exactly once.
- Preserve failure, interruption, duplicate-root, destruction, and immutability behavior.

## Explicitly Not Authorized

- Automatic startup.
- Presenter/splash implementation.
- Direct-scene initializer.
- Editor migration or setup.
- Test Lab scenes.
- EchoSceneFlow bridge.
- Normal scene travel.
- Report export.
- Package version bump.

## Evidence State

- Authority decision: Approved.
- Source conflict discovery: Complete.
- Runtime implementation: Not started.
- Compilation: Not run for FL-M3-08.
- Runtime tests: Not run for FL-M3-08.
- Standalone scene activation: Not run.
- Real-project adoption: Not run.

## Handoff Snapshot

FL-M3-07 is fully closed in implementation commit `a6f6544` and documentation commit `f76b9df`.

FL-M3-08 may begin only after the specification v1.4.0, EchoLaunch-ADR-001, and Checkpoint Build Plan are committed.

Next action: apply and push the authority bundle.
