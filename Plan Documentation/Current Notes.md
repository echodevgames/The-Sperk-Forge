# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 6, 2026
**Current focus:** First Light FL-M5-04 authority
**Current checkpoint:** FL-M5-04 — Read-Only Validator and Project Health Report

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current Focus

Approve the first dedicated read-only project-health surface for First Light.

FL-M5-04 adds one explicit Validator action, stable `ELAUNCH-VAL-*` findings,
scene-safe inspection, immutable health reporting, and deterministic copyable
evidence. It does not add a fix button or weaken Setup Apply/Repair boundaries.

## Starting State

- Branch: `main`
- HEAD: `638e676`
- `main` equals `origin/main`
- Working tree: clean
- FL-M5-03 authority: `6615c8f`
- FL-M5-03 implementation: `dd15768`
- FL-M5-03 documentation: `638e676`
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `236` passed
- Runtime Play Mode baseline: `479` passed
- Total automated baseline: `715` passed
- Current specification: v1.9.0 before this authority update
- FL-M5-04 implementation locked until authority commit

## Checkpoint Learning Review

- `[DECISION]` Validation is observation, not repair.
- `[DECISION]` Use a dedicated Validator window and explicit `Validate Project`.
- `[DECISION]` Do not validate on import, reload, Play Mode entry, window open, or repaint.
- `[DECISION]` Default to `Assets/EchoDevGames/FirstLight`, with an editable project root.
- `[DECISION]` Inspect Boot and enabled build scenes without saving them.
- `[DECISION]` Preserve open scenes, active scene, dirty states, assets, prefabs, and Build Settings.
- `[DECISION]` Return immutable schema-1 findings/report values.
- `[DECISION]` Derive `Healthy`, `NeedsAttention`, `Invalid`, or `Blocked` from stable severities.
- `[DECISION]` Produce deterministic request, evidence, and report fingerprints.
- `[DECISION]` Copy plain-text project-relative evidence with no machine paths.
- `[DECISION]` Reserve `ELAUNCH-VAL-009` for FL-M5-05 direct-scene release safety.
- `[DECISION]` Keep Apply, Repair, migration, direct scene, build hooks, Simulator, and Laboratory outside FL-M5-04.

## Stable Validation Codes

FL-M5-04 authorizes `ELAUNCH-VAL-001` through `ELAUNCH-VAL-015`.

`ELAUNCH-VAL-009` is reserved and must not be emitted until the direct-scene
helper exists under later authority.

## Next Action

Apply, review, commit, and push the six-file authority update:

```text
Approve FL-M5-04 read-only validator authority
```

After that commit, implementation may begin only within ADR-007 and the
FL-M5-04 Checkpoint Build Plan.

## Handoff

**Checkpoint:** FL-M5-04
**Baseline:** `638e676`
**Specification target:** v1.10.0
**ADR:** EchoLaunch-ADR-007
**Implementation:** Locked until authority commit
**Blockers:** None recorded
**Next:** Implement read-only Validator after authority commit
