# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 6, 2026
**Current focus:** First Light FL-M5-06 authority
**Current checkpoint:** FL-M5-06 — Launch Simulator and Deterministic Failure Injection

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Starting State

- Branch: `main`
- HEAD: `b6df92d`
- `main` equals `origin/main`
- Working tree: clean
- FL-M5-05 authority: `d538b5a`
- FL-M5-05 implementation: `4aa6ce7`
- FL-M5-05 documentation: `b6df92d`
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `266` passed
- Runtime Play Mode baseline: `503` passed
- Total automated baseline: `769` passed
- Specification: v1.11.0 before this authority update
- FL-M5-06 implementation locked until authority commit

## Learning Review Decisions

- `[DECISION]` Simulator proves startup-step semantics, not a complete launch.
- `[DECISION]` Simulator code is Editor-only.
- `[DECISION]` Use the real `StartupSequenceRunner` and policy contracts.
- `[DECISION]` Grant intentional friend access to the package Editor assembly.
- `[DECISION]` Build transient `HideAndDontSave` configuration/sequence objects.
- `[DECISION]` Never edit authored assets, scenes, or Build Settings.
- `[DECISION]` Use deterministic logical time rather than wall-clock evidence.
- `[DECISION]` Emit separate schema-1 `LaunchSimulationReport`, not
  `LaunchReport`.
- `[DECISION]` Support success, timed progress, warning, recoverable failure,
  blocking failure, timeout, exception, and cancellation.
- `[DECISION]` One active run; re-entry and cancellation are structured.
- `[DECISION]` Expected simulated failures remain report evidence, not Console
  warnings/errors.
- `[DECISION]` Standalone Laboratory remains a later separate checkpoint.

## Stable Simulator Diagnostics

- `ELAUNCH-SIM-001` invalid request.
- `ELAUNCH-SIM-002` active-run re-entry.
- `ELAUNCH-SIM-003` user cancellation.
- `ELAUNCH-SIM-004` infrastructure failure.
- `ELAUNCH-SIM-STEP-001` simulated warning.
- `ELAUNCH-SIM-STEP-002` simulated recoverable failure.
- `ELAUNCH-SIM-STEP-003` simulated blocking failure.

## Next Action

Commit and push:

```text
Approve FL-M5-06 launch simulator authority
```

Implementation may begin only after that commit.

## Handoff

**Checkpoint:** FL-M5-06
**Baseline:** `b6df92d`
**Specification target:** v1.12.0
**ADR:** EchoLaunch-ADR-009
**Implementation:** Locked until authority commit
**Blockers:** None recorded
