# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 8, 2026
**Current focus:** First Light FL-M5-07 documentation closeout
**Current checkpoint:** FL-M5-07 — Standalone Test Laboratory and Importable UPM Sample

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current State

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- Unity baseline: `6000.3.8f1`
- Authority commit: `8ff4109`
- Sample shell implementation: `ff0feff`
- Authored Laboratory assets: `a51c054`
- Imported-sample candidate isolation: `02429fb`
- Final acceptance fixes: `f1665f7`
- Working tree was clean after the final implementation push.

## FL-M5-07 Implemented Outcome

- `[DECISION]` First Light ships exactly one separately importable UPM sample named **First Light Standalone Test Lab**.
- `[DECISION]` The Laboratory is fully authored under `Samples~/First Light Standalone Test Lab/`; no second setup or launch engine is shipped.
- `[DECISION]` Standard imported `Assets/Samples/**` content is excluded from automatic Setup candidate discovery while explicit user selection remains supported.
- `[DECISION]` The canonical Boot Laboratory inherits `SuccessConfiguration` from the Laboratory root prefab and must not serialize a null scene override for that configuration.
- `[DECISION]` LAB-010 uses a sample-only manual skip-request surface routed through the existing uGUI splash presenter API. Production input ownership remains unchanged.
- `[DECISION]` The Laboratory splash minimum is five seconds so the manual minimum-duration/early-skip proof is human-observable.
- `[DECISION]` No package Runtime authority, Setup Apply/Repair write authority, Validator authority, Simulator authority, schema, or diagnostic ownership was broadened.

## Final Automated Validation

```text
Focused package tests: 6 / 6
Focused asset tests:   8 / 8

EditMode: 306 / 306
PlayMode: 503 / 503
Total:    809 / 809

Failed:   0
Ignored:  0
Errors:   0
Warnings: 0
```

## Manual Laboratory Acceptance

All twelve approved cases passed: `LAB-001` through `LAB-012`.

Setup repeatability:

```text
Run 1: Succeeded
Run 2: NoChanges
Run 3: NoChanges
```

Repair repeatability:

```text
Run 1: Succeeded — one deliberately cleared StartupSequence reference repaired
Run 2: NoChanges
Run 3: NoChanges
```

Healthy settled plan fingerprint:

```text
7eca14d6390a883417bb0b68cb54a0e2711a93803798d08e099d4cc21750516c
```

## Acceptance Findings Resolved During FL-M5-07

- `[BUG][RESOLVED]` Imported sample content initially polluted automatic Setup candidate discovery. The bounded `Assets/Samples/**` automatic-discovery exclusion restored the full suite while preserving explicit selection.
- `[BUG][RESOLVED]` The authored Boot Laboratory scene serialized a null `configuration` prefab-instance override. The override was removed and regression-tested.
- `[BUG][RESOLVED]` LAB-010 had no practical manual skip-request surface. A sample-only readout control and live elapsed/minimum/`CanSkipNow` evidence were added.
- `[NOTE]` The Laboratory-only minimum splash duration was increased from one second to five seconds for reliable manual observation.

## Unity Editor Session-Restore Observation

During LAB-012, Unity `6000.3.8f1` repeatedly hung during startup while restoring/opening the generated Boot asset path. Isolation showed that the hang followed the path/GUID even when scene contents were replaced with empty or known-good contents. The evidence does not support attributing this observation to First Light runtime or Laboratory scene contents.

## Repository Cleanup

- Imported `Assets/Samples` acceptance content removed.
- LAB-012 generated `Assets/EchoDevGames/FirstLight` content removed.
- Temporary Build Settings changes restored.
- Generated solution-file drift restored.
- Final implementation staging contained package/test files only.
- Final implementation commit pushed as `f1665f7`.

## Next Action

Commit and push the FL-M5-07 documentation closeout. After that commit is synchronized, select the next First Light checkpoint deliberately.
