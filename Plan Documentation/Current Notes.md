# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 6, 2026
**Current focus:** First Light FL-M5-06 documentation closeout
**Current checkpoint:** FL-M5-06 — Launch Simulator and Deterministic Failure Injection

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Completed Implementation State

- Branch: `main`
- HEAD: `956c381`
- `main` equals `origin/main`
- Working tree: clean
- FL-M5-06 authority: `a159349`
- FL-M5-06 implementation: `956c381`
- FL-M5-05 documentation: `b6df92d`
- Compilation: `0` errors, `0` warnings
- Focused Simulator EditMode: `24` passed
- Complete EditMode: `290` passed
- Runtime Play Mode: `503` passed
- Total automated: `793` passed
- Manual scenarios: `8` accepted
- Specification: v1.12.0
- Implementation pushed; documentation closeout pending

## Durable Decisions Confirmed

- `[DECISION]` Simulator proves startup-step semantics, not a complete launch.
- `[DECISION]` Simulator code is Editor-only.
- `[DECISION]` Use the real `StartupSequenceRunner` and policy contracts.
- `[DECISION]` Grant intentional friend access to the package Editor assembly.
- `[DECISION]` Build transient `HideAndDontSave` configuration/sequence objects.
- `[DECISION]` Never edit authored assets, scenes, or Build Settings.
- `[DECISION]` Use deterministic logical time rather than wall-clock evidence.
- `[DECISION]` Emit separate schema-1 `LaunchSimulationReport`, not `LaunchReport`.
- `[DECISION]` Support success, timed progress, warning, recoverable failure,
  blocking failure, timeout, exception, and cancellation.
- `[DECISION]` One active run; re-entry and cancellation are structured.
- `[DECISION]` Expected simulated failures remain report evidence, not Console
  warnings/errors.
- `[DECISION]` Cancellation evidence filters human-click-dependent elapsed time
  in the Simulator report-copy layer while preserving runtime runner truth.
- `[DECISION]` Standalone Laboratory remains a later separate checkpoint.

## Accepted Evidence

- Immediate success: 1 attempted / 1 authored.
- Timed progress: 25%, 50%, 75%, 100% at deterministic logical times.
- Warning: continued to the proof step.
- Recoverable failure: policy-converted to Warning and continued.
- Blocking failure: later step unvisited.
- Timeout: canonical `ELAUNCH-STEP-003`, later step unvisited.
- Executor exception: canonical `ELAUNCH-STEP-004`, later step unvisited.
- Cancellation: canonical `ELAUNCH-STEP-005` plus `ELAUNCH-SIM-003`.
- Three cancellation reruns produced report fingerprint:
  `e92b028d7798ec597894213539e3ae19b113931e714ef29bae6d8d11bb92362b`.
- Final Console: `0` errors, `0` warnings.
- No project asset, scene, Build Settings, ProjectSettings, or solution residue
  entered the implementation commit.

## Next Action

Apply, review, commit, and push the FL-M5-06 documentation closeout.

No later checkpoint is authorized.
