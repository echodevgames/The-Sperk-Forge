# FL-M5-06 — Launch Simulator and Deterministic Failure Injection

## 1. Document Control

| Field | Value |
|---|---|
| Document ID | FL-M5-06 |
| Version | 1.0.0 |
| Status | Approved |
| Package | First Light (`EchoLaunch`) |
| Package specification | SFGSS-PKG-ECHOLAUNCH-001 v1.12.0 |
| ADR | EchoLaunch-ADR-009 |
| Milestone | M5 — Tooling and Direct Scene |
| Repository | The-Sperk-Forge |
| Branch | `main` |
| Required baseline | `b6df92d` |
| Unity baseline | `6000.3.8f1` |
| Owner | Jesse “Echo” Adams / EchoDevGames |
| Last updated | August 6, 2026 |
| Approved by | Jesse “Echo” Adams / EchoDevGames |

## 2. Purpose

Provide an explicit Editor-only Simulator that exercises real startup-step
execution, progress, policy, timeout, exception, and cancellation behavior
without editing authored project content or pretending to complete a full
launch.

```text
Simulation request
    -> validate
    -> build transient in-memory sequence
    -> run real StartupSequenceRunner
    -> copy immutable simulation report
    -> destroy transient objects
```

## 3. Starting Conditions

- HEAD: `b6df92d`
- Working tree: clean
- `main` equals `origin/main`
- Package version: `0.1.0`
- Specification: v1.11.0 before this authority update
- FL-M5-05 authority: `d538b5a`
- FL-M5-05 implementation: `4aa6ce7`
- FL-M5-05 documentation: `b6df92d`
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `266` passed
- Runtime PlayMode baseline: `503` passed
- Total automated baseline: `769` passed
- Existing runner, policy, progress, timeout, exception, and report contracts
  are stable
- No unresolved architecture blocker

## 4. Learning Review

The Simulator must not answer the wrong question.

It is responsible for:

> What does the real startup-step runner do when controlled steps report
> progress, warnings, failures, timeout, exception, or cancellation?

It is not responsible for:

> Did a real root claim, show presentation, and reach a destination?

That second question belongs to the Standalone Laboratory.

The review therefore selects:

1. Editor-only implementation.
2. Real internal runner and policy contracts.
3. Transient in-memory authored shape.
4. Deterministic logical timing.
5. Separate schema-1 simulation report.
6. Explicit run and cancellation.
7. No Runtime/player Simulator implementation.
8. No authored asset or scene mutation.

## 5. Scope

- Simulator menu/window
- Immutable `LaunchSimulationRequest`
- Stable `LaunchSimulationPreset`
- Immutable normalized plan/step values
- Immutable `LaunchSimulationProgressSample`
- Immutable `LaunchSimulationStepReport`
- Immutable schema-1 `LaunchSimulationReport`
- Stable Simulator status and diagnostics
- Deterministic request/plan/report fingerprints
- Deterministic text formatter
- Single-active-run gate
- Cooperative cancellation
- Logical clock and deterministic scheduler
- Transient `HideAndDontSave` configuration/sequence/entry/definition objects
- Built-in simulator step definitions/executors in the Editor assembly
- Real `StartupSequenceRunner` use
- Friend access from Runtime to package Editor assembly
- Focused Editor tests
- Complete EditMode and Runtime PlayMode regression
- Manual preset, determinism, cancellation, cleanup, and Git acceptance
- Documentation closeout after implementation

## 6. Explicit Exclusions

- Runtime/player Simulator types
- Root authority claim
- Splash or status presentation
- Destination load or handoff
- `LaunchReport` creation
- Standalone Laboratory scenes/assets
- Runtime sample step definitions
- Persistent scenario assets
- Editing selected project configurations/sequences
- Scene, Build Settings, ProjectSettings, or package-manifest mutation
- Automatic Play Mode entry
- Build hooks
- JSON/support-bundle export
- Simulator-to-Validator integration
- Migration, receipt, uninstall/reset, or recovery

## 7. Contracts

### 7.1 Presets

```text
ImmediateSuccess
TimedProgressSuccess
WarningContinues
RecoverableFailureContinues
BlockingFailureStops
TimeoutStops
ExecutorExceptionStops
Cancellation
```

Unknown values are invalid.

### 7.2 Request

Immutable request fields:

- Schema version `1`
- Preset
- Logical duration
- Progress sample count
- Timeout
- Optional normalized message
- Request fingerprint

Bounds:

- Logical duration: finite, `0` through `60` seconds
- Progress samples: `0` through `120`
- Timeout: finite, `0` through `60` seconds
- Message: maximum `256` normalized characters
- Preset-specific constraints applied before any transient object creation

### 7.3 Status

```text
NotRun
Completed
Cancelled
InvalidRequest
Busy
InfrastructureFailure
```

### 7.4 Report

Immutable report fields:

- Schema version `1`
- Status
- Preset
- Normalized request values
- Request, plan, and report fingerprints
- Authored, disabled, attempted, and unvisited counts
- Ordered step reports
- Ordered progress samples
- Final effective result
- Cancellation flag
- Simulator diagnostic code/message/details

No Unity object reference survives report construction.

### 7.5 Determinism

Fingerprints and copied text include only normalized semantic evidence.

Excluded:

- Current date/time
- Wall-clock duration
- Unity instance IDs
- Object names generated by Unity
- Memory addresses
- Stack traces
- Machine/project absolute paths
- Editor repaint/frame frequency

### 7.6 Transient plan

The plan uses transient ScriptableObjects with `HideFlags.HideAndDontSave`.

The transient builder:

- Creates valid canonical IDs deterministically from the request/step index.
- Creates a supported transient configuration and sequence.
- Creates ordered enabled entries and simulator definitions.
- Assigns preset-specific policy.
- Tracks every created object.
- Destroys all objects in reverse ownership order.
- Is idempotent on disposal.
- Never calls `AssetDatabase.CreateAsset`, `SaveAssets`, scene save, or Build
  Settings APIs.

### 7.7 Logical clock and scheduler

- Clock starts at `0`.
- Scheduler advances only through accepted logical samples.
- Progress is monotonic and clamped.
- Timed success reaches `1`.
- Timeout passes through the existing timeout monitor.
- Cancellation uses the runner's caller cancellation path.
- No actual waiting is required for automated tests.
- The Editor UI may pace display separately without changing report evidence.

### 7.8 Console policy

Expected simulated warning/failure outcomes appear in the report and window.
They are not logged as Unity Console warnings/errors.

Only an unexpected Simulator infrastructure failure may log one sanitized
Error after an immutable `ELAUNCH-SIM-004` report is accepted.

## 8. Files

### Create

- `Editor/Simulation/LaunchSimulationPreset.cs`
- `Editor/Simulation/LaunchSimulationStatus.cs`
- `Editor/Simulation/LaunchSimulationRequest.cs`
- `Editor/Simulation/LaunchSimulationPlan.cs`
- `Editor/Simulation/LaunchSimulationProgressSample.cs`
- `Editor/Simulation/LaunchSimulationStepReport.cs`
- `Editor/Simulation/LaunchSimulationReport.cs`
- `Editor/Simulation/LaunchSimulationFingerprint.cs`
- `Editor/Simulation/LaunchSimulationTextFormatter.cs`
- `Editor/Simulation/LaunchSimulationDiagnosticCodes.cs`
- `Editor/Simulation/LaunchSimulationLogicalClock.cs`
- `Editor/Simulation/LaunchSimulationScheduler.cs`
- `Editor/Simulation/LaunchSimulationTransientPlanBuilder.cs`
- `Editor/Simulation/LaunchSimulationStepDefinition.cs`
- `Editor/Simulation/LaunchSimulationStepExecutor.cs`
- `Editor/Simulation/LaunchSimulationService.cs`
- `Editor/Simulation/EchoLaunchSimulatorWindow.cs`
- Focused `Tests/Editor/Simulation/*`
- Matching folder/source metadata

### Modify only as required

- `Runtime/Properties/AssemblyInfo.cs`
  - Grant package Editor assembly intentional internal access
- Existing Editor test assembly definition only if a missing framework reference
  is proven
- Documentation only in the later closeout

Do not modify runtime runner, policy, result, root, destination, presentation,
Setup, Repair, Validator, Direct Scene, ProjectSettings, or project assets
unless a test-proven defect requires a separate authority pause.

## 9. Implementation Sequence

### Phase 1 — Immutable contracts

1. Add preset/status/diagnostic vocabulary.
2. Add normalized request and report contracts.
3. Add deterministic fingerprints and text formatter.
4. Prove defensive copies and invalid-value rejection.

### Phase 2 — Transient planning

1. Add deterministic ID generation.
2. Build transient configuration/sequence/entries/definitions.
3. Assign scenario-specific policy and later-step visibility.
4. Track and dispose every Unity object.
5. Prove no AssetDatabase/scene/Build Settings mutation.

### Phase 3 — Logical execution

1. Add logical clock and scheduler.
2. Add simulator definitions/executors.
3. Run through the real `StartupSequenceRunner`.
4. Copy executions and progress into immutable report values.
5. Prove continuation and stop behavior.
6. Prove timeout, exception conversion, and cancellation.

### Phase 4 — Service and window

1. Add single-active-run service.
2. Add explicit `Run Simulation`.
3. Add `Cancel Simulation`.
4. Add preset and bounded parameter controls.
5. Show ordered step/progress evidence.
6. Add `Copy Report`.
7. Ensure open/repaint performs no run.
8. Cancel/cleanup safely on window close.

### Phase 5 — Automated gates

1. Focused Simulation contract tests.
2. Focused transient cleanup tests.
3. Focused runner/policy scenario tests.
4. Focused determinism/formatter tests.
5. Focused window no-auto-run/reentry tests.
6. Complete EditMode suite.
7. Complete Runtime PlayMode regression.
8. Compile and Console gate.
9. Git scope and residue inspection.

### Phase 6 — Manual acceptance

1. Open Simulator and confirm no automatic run.
2. Run Immediate Success.
3. Run Timed Progress Success.
4. Run Warning Continues.
5. Run Recoverable Failure Continues.
6. Run Blocking Failure Stops.
7. Run Timeout Stops.
8. Run Executor Exception Stops.
9. Start Cancellation and cancel it.
10. Repeat unchanged scenarios and compare exact copied text/fingerprints.
11. Confirm simulated warnings/failures create no Console warnings/errors.
12. Confirm authored configuration and sequence hashes are unchanged.
13. Confirm no new assets, scene changes, Build Settings changes, or
    ProjectSettings changes.
14. Close/reopen window and confirm clean idle state.
15. Restore generated solution drift.
16. Stage only approved package Editor/test/metadata and AssemblyInfo change.

## 10. Test Matrix

| ID | Proof | Expected |
|---|---|---|
| FL-M5-06-T01 | Window opens/repaints | No run |
| FL-M5-06-T02 | Default request | Valid ImmediateSuccess |
| FL-M5-06-T03 | Unknown preset | SIM-001; no transient objects |
| FL-M5-06-T04 | Bounds validation | SIM-001 |
| FL-M5-06-T05 | Immediate success | One successful step |
| FL-M5-06-T06 | Timed progress | Ordered samples, final progress 1 |
| FL-M5-06-T07 | Warning | Warning then later success |
| FL-M5-06-T08 | Recoverable failure | Effective continuation then success |
| FL-M5-06-T09 | Blocking failure | Stops; later step unvisited |
| FL-M5-06-T10 | Timeout | Existing STEP-003; later step unvisited |
| FL-M5-06-T11 | Executor exception | Existing conversion; later step unvisited |
| FL-M5-06-T12 | Cancellation | Existing cancellation path; SIM-003 report |
| FL-M5-06-T13 | Re-entry | Busy/SIM-002 |
| FL-M5-06-T14 | Identical request | Identical report fingerprint and text |
| FL-M5-06-T15 | Changed parameter | Changed request/plan/report fingerprints |
| FL-M5-06-T16 | Defensive copy | Immutable reports |
| FL-M5-06-T17 | Every terminal path | Zero transient residue |
| FL-M5-06-T18 | Authored assets | Unchanged/not dirty |
| FL-M5-06-T19 | Scenes/Build Settings | Unchanged |
| FL-M5-06-T20 | Console policy | Expected failures not logged as warnings/errors |
| FL-M5-06-T21 | Player boundary | No Simulator type in Runtime/player assembly |
| FL-M5-06-T22 | Complete regression | All existing tests green |

## 11. Expected Baseline

```text
Compilation:       0 errors, 0 warnings
EditMode:          266 passed
Runtime PlayMode:  503 passed
Total automated:   769 passed
```

New discovery totals are not predetermined. Record Unity's actual counts.

## 12. Completion Criteria

FL-M5-06 is complete only when:

- Specification v1.12.0 and ADR-009 remain satisfied.
- Simulator runs only after explicit user action.
- Real sequence runner and policy contracts are used.
- No alternate runner exists.
- Reports are immutable, deterministic, and truthful.
- All presets settle with approved outcomes.
- Cancellation and re-entry are bounded.
- Transient cleanup is proven.
- Authored project content remains untouched.
- Runtime/player behavior remains unchanged.
- Focused and complete automated gates pass.
- Manual acceptance passes.
- Documentation closeout is committed and pushed.

## 13. Stop Point

Stop after the Editor-only Simulator, deterministic failure injection, tests,
manual acceptance, cleanup, and documentation closeout.

Do not continue into:

- Standalone Laboratory scenes/assets.
- Runtime sample steps.
- Automatic scenario installation.
- Portable report export.
- Build hooks.
- Migration.
- Distribution.
- First project adoption.
