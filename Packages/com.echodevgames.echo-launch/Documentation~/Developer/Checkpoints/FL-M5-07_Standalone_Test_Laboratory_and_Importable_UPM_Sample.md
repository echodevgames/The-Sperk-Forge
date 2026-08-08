# FL-M5-07 — Standalone Test Laboratory and Importable UPM Sample

## Status

- Checkpoint: `FL-M5-07`
- Status: Complete
- Package: First Light (`EchoLaunch`)
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- Unity baseline: `6000.3.8f1`
- Authority commit: `8ff4109`
- Sample shell implementation: `ff0feff`
- Authored Laboratory assets: `a51c054`
- Imported-sample isolation correction: `02429fb`
- Final acceptance fixes: `f1665f7`
- Completion date: August 8, 2026

## Purpose

Deliver the approved First Light Standalone Test Laboratory as exactly one fully authored, separately importable Unity Package Manager sample and prove the complete MVP launch loop without creating a second launch pipeline or setup system.

## Implemented Surface

- One package sample declaration: **First Light Standalone Test Lab**
- Fully authored `Samples~/First Light Standalone Test Lab/` distribution
- Sample-only runtime assembly
- Boot and destination Laboratory scenes
- Success, timed-progress, warning, recoverable, blocking, and invalid-destination configurations
- Laboratory startup step definitions and sequences
- Laboratory Direct Scene configuration
- Laboratory root prefab and disabled duplicate-root fixture
- Redistributable placeholder splash art
- Manual Laboratory readout
- Sample-only LAB-010 skip-request evidence surface
- Five-second Laboratory minimum splash for observable manual proof
- Static package/distribution tests
- Serialized asset/reference tests
- Narrow automatic `Assets/Samples/**` Setup-candidate exclusion with explicit selection preserved

## Acceptance Corrections

Normal Package Manager import exposed three bounded defects:

1. Imported `Assets/Samples/**` content entered automatic Setup candidate discovery. The accepted correction excludes standard imported sample roots from automatic discovery while preserving explicit selection.
2. The canonical Boot Laboratory serialized a null `configuration` prefab-instance override even though the Laboratory root prefab held the correct `SuccessConfiguration`. The null override was removed and regression-tested.
3. LAB-010 lacked a practical manual skip-request surface. The Laboratory readout now routes a sample-only request through `EchoLaunchStatusView.RequestSplashSkip()`, and the Laboratory-only minimum display is five seconds for observable evidence.

No production launch authority, Setup mutation authority, schema, diagnostic ownership, or project-wide input authority changed.

## Final Evidence

```text
Focused package tests: 6 / 6
Focused asset tests:   8 / 8
Complete EditMode:     306 / 306
Runtime Play Mode:     503 / 503
Total automated:       809 / 809
Manual LAB matrix:      12 / 12

Failed:   0
Ignored:  0
Errors:   0
Warnings: 0
```

## Manual Acceptance Matrix

| ID | Result |
|---|---|
| LAB-001 | Canonical Boot success completed and activated the destination |
| LAB-002 | Timed progress completed with responsive presentation |
| LAB-003 | Warning retained; traversal continued; destination activated |
| LAB-004 | Missing configuration blocked before execution with `ELAUNCH-CFG-001` |
| LAB-005 | Blocking failure stopped traversal and prevented destination activation |
| LAB-006 | Duplicate emitted `ELAUNCH-ROOT-001`; first authority remained authoritative |
| LAB-007 | Invalid destination blocked preflight with `ELAUNCH-DEST-001` |
| LAB-008 | Direct scene created exactly one development authority |
| LAB-009 | Direct scene reused an existing authority without duplication |
| LAB-010 | Early skip request remained blocked until the five-second minimum |
| LAB-011 | Removing imported sample left package compilation/tooling healthy |
| LAB-012 | Setup and Repair remained repeat-safe across three runs |

## LAB-012 Repeatability

```text
Setup:
Run 1: Succeeded
Run 2: NoChanges
Run 3: NoChanges

Repair:
Run 1: Succeeded — one deliberately cleared StartupSequence reference repaired
Run 2: NoChanges
Run 3: NoChanges

Healthy fingerprint:
7eca14d6390a883417bb0b68cb54a0e2711a93803798d08e099d4cc21750516c
```

No duplicate generated asset, root, Boot scene, or Build Settings entry was produced.

## Unity Editor Session-Restore Observation

Unity `6000.3.8f1` repeatedly hung during editor startup while attempting to restore/open the generated Boot asset path additively. Compilation, AssetDatabase refresh, and `OutdoorsScene` load completed first. The Editor opened when the Boot path was absent, and the hang returned when that path/GUID was restored even with empty or known-good replacement scene contents.

The evidence therefore does not support attributing the hang to First Light runtime, the generated root prefab, or the tested Boot scene contents. The observation is retained for tooling/environment follow-up.

## Completion

FL-M5-07 is implemented, import-tested, manually accepted `12 / 12`, automated-tested `809 / 809`, cleaned, and pushed. Documentation closeout is the final action before deliberately selecting the next checkpoint.
