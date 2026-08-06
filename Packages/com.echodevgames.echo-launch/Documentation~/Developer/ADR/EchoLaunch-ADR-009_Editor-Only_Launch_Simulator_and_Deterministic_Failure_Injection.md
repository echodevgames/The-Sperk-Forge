# EchoLaunch ADR-009 — Editor-Only Launch Simulator and Deterministic Failure Injection

## Metadata

- ADR: `EchoLaunch-ADR-009`
- Status: Approved
- Date: August 6, 2026
- Package: First Light (`EchoLaunch`)
- Package specification: SFGSS-PKG-ECHOLAUNCH-001 v1.12.0
- Checkpoint: FL-M5-06
- Decision owner: Jesse “Echo” Adams / EchoDevGames
- Baseline commit: `b6df92d`

## Context

First Light already owns and validates real startup authority, ordered sequence
execution, step policies, progress, timeout, exception conversion, immutable
launch reporting, destination handoff, Setup, Repair, Validator, and Direct
Scene development entry.

The specification still requires delay and failure simulation before the
Standalone Laboratory can provide visible MVP evidence.

A careless Simulator could:

- Edit the project's real startup sequence.
- Save disposable test assets into `Assets/`.
- Add production runtime types or release behavior.
- Invent a second step runner.
- Produce a `LaunchReport` that falsely claims root or destination completion.
- Depend on future Laboratory samples.
- Produce machine-speed-dependent evidence.
- Leave transient Unity objects behind after cancellation or window closure.

## Decision

### Explicit Editor-only tool

Add:

```text
Tools > Sperk's Forge > First Light > Simulator
```

Opening the window does not run anything. The user explicitly chooses a scenario
and presses `Run Simulation`.

All Simulator orchestration, transient definitions/executors, logical timing,
formatting, and window code lives in the Editor assembly.

### Real step contracts, not a second runner

The Simulator executes the existing internal `StartupSequenceRunner` with the
existing:

- `StartupStepPolicy`
- `StartupStepResult`
- Progress gate and relay
- Timeout monitor
- Exception converter
- Traversal and stopping behavior

The Runtime assembly may add
`InternalsVisibleTo("EchoDevGames.EchoLaunch.Editor")` so the package-owned
Editor assembly can use these established seams.

No alternate runner or policy evaluator is introduced.

### Transient in-memory authored shape

Each run creates transient `HideAndDontSave` instances representing a valid
configuration, startup sequence, entries, and simulator step definitions.

The tool never writes, edits, imports, renames, moves, or deletes project assets.
It never modifies a user's configuration or sequence.

All transient Unity objects are disposed after completion, cancellation,
invalid-request rejection after creation, infrastructure failure, and window
closure.

### Separate simulation report

The Simulator returns `LaunchSimulationReport` schema version `1`.

It does not return `LaunchReport`, because no root claims authority, no
presentation is activated, and no destination handoff completes.

The simulation report copies semantic sequence evidence without retaining Unity
objects:

- Request, plan, and report fingerprints.
- Scenario and normalized parameters.
- Authored, disabled, attempted, and unvisited counts.
- Ordered step outcomes.
- Ordered progress samples.
- Final effective result.
- Cancellation state.
- Stable simulator diagnostic.
- Deterministic text.

### Built-in scenarios

Version 1 includes:

- Immediate success.
- Timed progress success.
- Warning followed by success.
- Recoverable failure followed by success.
- Blocking failure with an unvisited later step.
- Timeout with an unvisited later step.
- Executor exception with an unvisited later step.
- User cancellation.

The built-in warning/recoverable/blocking results use stable
`ELAUNCH-SIM-STEP-*` codes. Timeout and exception scenarios use the existing
canonical timeout/exception semantics.

### Deterministic logical time

The Simulator uses a package-owned Editor logical clock and deterministic
scheduler.

- Logical time starts at zero.
- Progress points are generated from normalized request values.
- Timeout uses the real timeout monitor against the logical clock.
- Identical accepted requests produce identical outcomes, ordering,
  fingerprints, progress samples, and copied text.
- Wall-clock time, system date, frame rate, repaint frequency, and machine speed
  are excluded from accepted evidence.

The UI may animate accepted logical progress without changing report data.

### Single active run and cancellation

Only one simulation may be active.

Re-entry returns `Busy` and `ELAUNCH-SIM-002`.

Cancellation is cooperative and travels through the real runner cancellation
path. Closing the window requests cancellation and waits for cleanup.

### Stable statuses and diagnostics

Statuses:

- `NotRun`
- `Completed`
- `Cancelled`
- `InvalidRequest`
- `Busy`
- `InfrastructureFailure`

Diagnostics:

- `ELAUNCH-SIM-001` invalid request.
- `ELAUNCH-SIM-002` active-run re-entry.
- `ELAUNCH-SIM-003` user cancellation.
- `ELAUNCH-SIM-004` transient-plan or infrastructure failure.
- `ELAUNCH-SIM-STEP-001` simulated warning.
- `ELAUNCH-SIM-STEP-002` simulated recoverable failure.
- `ELAUNCH-SIM-STEP-003` simulated blocking failure.

### No production dependency

FL-M5-06 adds no Simulator type to the Runtime assembly and no player behavior.

It adds no:

- Runtime simulation component.
- Scripting define.
- Build hook.
- Scene object.
- Persistent asset.
- Samples dependency.
- Peer-package dependency.
- Reflection discovery.
- Automatic Play Mode entry.

## Rejected

- Modifying an authored startup sequence for simulation.
- Persistent project-owned simulator scenario assets in FL-M5-06.
- A second sequence runner.
- A fake successful `LaunchReport`.
- Loading or reloading a destination.
- Creating a root or presentation object.
- Running automatically when the window opens.
- Requiring Play Mode.
- Depending on Standalone Laboratory content.
- Logging simulated warnings/failures as Unity Console warnings/errors.
- Using wall-clock timestamps in deterministic evidence.
- Build hooks or release-player simulation code.
- Portable JSON/support-bundle export in this checkpoint.

## Consequences

### Positive

- Testers can prove real policy, progress, timeout, exception, and cancellation
  behavior without damaging authored content.
- Failure evidence is deterministic and copyable.
- Production runtime behavior remains unchanged.
- Laboratory work can reuse proven outcome expectations later without depending
  on Simulator implementation.
- The distinction between simulated step behavior and a full launch remains
  truthful.

### Costs

- The Editor assembly receives intentional friend access to Runtime internals.
- Transient serialized-object construction requires strict cleanup tests.
- Logical timing needs a deterministic scheduler rather than ordinary
  wall-clock delays.
- Full visible Boot/presentation/destination evidence remains deferred to the
  Standalone Laboratory.

## Validation Obligations

FL-M5-06 must prove:

- Window creation/repaint does not run simulation.
- Every built-in preset uses the real runner.
- Warning and recoverable failure continue according to policy.
- Blocking failure, timeout, and exception stop traversal.
- Unvisited counts are truthful.
- Progress values are ordered and clamped.
- Identical requests produce identical reports and text.
- Re-entry returns `ELAUNCH-SIM-002`.
- Cancellation settles through canonical cancellation behavior.
- Invalid requests create no transient objects.
- Every terminal path destroys transient objects.
- Authored assets, scenes, Build Settings, and ProjectSettings remain unchanged.
- Simulated warnings/failures do not pollute the Unity Console.
- Runtime player assemblies contain no Simulator implementation.
- Existing `266` EditMode and `503` Runtime Play Mode tests remain green.
- Manual runs cover all presets, determinism, cancellation, and clean Git state.

## Supersession

This ADR extends CAP-014 and the Editor tooling boundary. It does not supersede
the startup runner, launch-report contract, Validator, Direct Scene, or
Standalone Laboratory authorities.
