# FL-M5-06 — Launch Simulator and Deterministic Failure Injection

## Status

- Checkpoint: `FL-M5-06`
- Status: Complete
- Package: First Light (`EchoLaunch`)
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.12.0
- ADR: EchoLaunch-ADR-009
- Authority commit: `a159349`
- Implementation commit: `956c381`
- Unity baseline: `6000.3.8f1`
- Completion date: August 6, 2026

## Purpose

Provide an explicit Editor-only tool that proves real startup-step runner,
policy, progress, timeout, exception, and cancellation semantics without
modifying authored project content or falsely claiming a complete root,
presentation, or destination launch.

## Implemented Surface

- `Tools > Sperk's Forge > First Light > Simulator`
- Explicit `Run Simulation`
- Cooperative `Cancel Simulation`
- `Copy Report`
- Immutable normalized simulation request
- Deterministic scenario plan
- Stable request, plan, and report fingerprints
- Immutable schema-1 simulation report
- Ordered immutable step and progress evidence
- Single-active-run protection
- Transient `HideAndDontSave` configuration, sequence, entry, and definition data
- Real `StartupSequenceRunner` and existing policy contracts
- Deterministic Editor logical clock
- Stable `ELAUNCH-SIM-*` and `ELAUNCH-SIM-STEP-*` diagnostics
- Editor-only friend access to established Runtime internals

## Accepted Presets

1. Immediate Success
2. Timed Progress Success
3. Warning Continues
4. Recoverable Failure Continues
5. Blocking Failure Stops
6. Timeout Stops
7. Executor Exception Stops
8. Cancellation

## Architecture Result

The Simulator is not a second launch pipeline.

```text
Transient request
    -> transient authored shape
    -> real startup runner
    -> immutable simulation report
    -> complete cleanup
```

It creates no root, presentation, destination transition, persistent scenario
asset, scene mutation, Build Settings mutation, build hook, or player behavior.

## Cancellation Determinism Correction

Manual acceptance exposed one bounded evidence defect: caller cancellation
correctly used canonical `ELAUNCH-STEP-005`, but the copied Simulator report
included human-click-dependent elapsed time.

The accepted correction:

- retained the runtime runner unchanged
- retained canonical cancellation code and message
- normalized Simulator cancellation logical elapsed to `0`
- retained stable `ExecutorCompletedWithoutException: False`
- excluded `ElapsedSeconds:` from copied cancellation evidence
- added a regression assertion
- reproduced the same report fingerprint across three manual reruns

Accepted report fingerprint:

```text
e92b028d7798ec597894213539e3ae19b113931e714ef29bae6d8d11bb92362b
```

## Automated Evidence

```text
Compilation:        0 errors, 0 warnings
Focused Simulator: 24 passed
Complete EditMode: 290 passed
Runtime PlayMode:  503 passed
Total automated:   793 passed
Failed:               0
Ignored:              0
```

## Manual Evidence

- Window opened in `Not Run` state.
- No simulation started automatically.
- Immediate success completed.
- Timed progress produced 25%, 50%, 75%, and 100% at deterministic logical times.
- Warning continued to the proof step.
- Optional recoverable failure converted to Warning and continued.
- Blocking failure stopped with one unvisited step.
- Timeout used canonical `ELAUNCH-STEP-003`.
- Executor exception used canonical `ELAUNCH-STEP-004`.
- Cancellation used canonical `ELAUNCH-STEP-005` and `ELAUNCH-SIM-003`.
- Three repeated cancellation runs produced identical fingerprints.
- Expected simulated failures produced no Unity Console warnings or errors.
- Final Console reported `0` errors and `0` warnings.

## Repository Evidence

The implementation commit contains:

- 19 Editor Simulator source files
- 6 focused test source files
- matching Unity metadata
- one Runtime `AssemblyInfo.cs` friend-access modification

It contains no project assets, scenes, Build Settings, ProjectSettings,
documentation, package-manifest, or generated solution-file changes.

## Deferred

- Standalone Laboratory scenes and assets
- Runtime sample steps
- automatic scenario installation
- portable report export
- build hooks
- migration, receipts, uninstall, or recovery
- player-build and external-adoption evidence

## Completion

FL-M5-06 is implemented, automated-tested, manually accepted, pushed, and ready
for documentation closeout.
