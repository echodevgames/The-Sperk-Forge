# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 7, 2026
**Current focus:** First Light post-rewind baseline reconciliation
**Current checkpoint:** None — FL-M5-06 is complete; no later checkpoint is authorized

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Canonical First Light Baseline

- Branch: `main`
- FL-M5-06 authority: `a159349`
- FL-M5-06 implementation: `956c381`
- FL-M5-06 documentation closeout: `e28ff09`
- FL-M5-05 documentation: `b6df92d`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.12.0
- Compilation revalidated August 7: `0` errors, `0` warnings
- Complete EditMode revalidated August 7: `290` passed, `0` failed, `0` ignored
- Runtime Play Mode revalidated August 7: `503` passed, `0` failed, `0` ignored
- Total automated baseline: `793` passed
- FL-M5-06 manual scenarios: `8` accepted
- No later checkpoint is authorized by this reconciliation.

## Post-Rewind Drift Reconciliation

- `[DECISION]` Commit `e28ff09` is the canonical FL-M5-06 closeout baseline for resumed development.
- `[DECISION]` Work created after `e28ff09` was intentionally removed from `main` and is not implementation or architecture authority unless deliberately reintroduced through a new approved checkpoint.
- `[TEST]` The restored `e28ff09` project compiled with `0` errors and `0` warnings and freshly passed `290` EditMode plus `503` Runtime Play Mode tests.
- `[NOTE]` The FL-M5-06 closeout commit correctly updated the package README, package documentation index, architecture documentation, changelog, specification, checkpoint record, test report, and completion record, but both `Current Notes.md` pages retained stale pre-closeout wording that still said the documentation closeout was pending.
- `[DECISION]` This reconciliation corrects only those two living-note pages. It changes no Runtime code, Editor code, package manifest, asset, scene, Build Settings, ProjectSettings, public API, schema, diagnostic, or test contract.

## FL-M5-06 Durable Decisions Retained

- `[DECISION]` Simulator proves startup-step semantics, not a complete launch.
- `[DECISION]` Simulator code is Editor-only.
- `[DECISION]` Use the real `StartupSequenceRunner` and policy contracts.
- `[DECISION]` Grant intentional friend access to the package Editor assembly.
- `[DECISION]` Build transient `HideAndDontSave` configuration/sequence objects.
- `[DECISION]` Never edit authored assets, scenes, or Build Settings through Simulator execution.
- `[DECISION]` Use deterministic logical time rather than wall-clock evidence.
- `[DECISION]` Emit separate schema-1 `LaunchSimulationReport`, not `LaunchReport`.
- `[DECISION]` Support success, timed progress, warning, recoverable failure, blocking failure, timeout, exception, and cancellation.
- `[DECISION]` One active Simulator run; re-entry and cancellation are structured.
- `[DECISION]` Expected simulated failures remain report evidence, not Console warnings/errors.
- `[DECISION]` Cancellation evidence filters human-click-dependent elapsed time in the Simulator report-copy layer while preserving runtime runner truth.
- `[DECISION]` Standalone Laboratory remains a separate checkpoint requiring fresh authorization.

## Accepted FL-M5-06 Evidence

- Immediate success: 1 attempted / 1 authored.
- Timed progress: 25%, 50%, 75%, 100% at deterministic logical times.
- Warning: continued to the proof step.
- Recoverable failure: policy-converted to Warning and continued.
- Blocking failure: later step unvisited.
- Timeout: canonical `ELAUNCH-STEP-003`, later step unvisited.
- Executor exception: canonical `ELAUNCH-STEP-004`, later step unvisited.
- Cancellation: canonical `ELAUNCH-STEP-005` plus `ELAUNCH-SIM-003`.
- Accepted cancellation report fingerprint: `e92b028d7798ec597894213539e3ae19b113931e714ef29bae6d8d11bb92362b`.
- Final Console: `0` errors, `0` warnings.

## Next Action

Draft and review a fresh FL-M5-07 authority package from the reconciled `e28ff09` baseline.

Do not implement Standalone Laboratory content until that checkpoint is explicitly authorized and committed.
