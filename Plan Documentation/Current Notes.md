# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 5, 2026
**Current focus:** First Light FL-M5-02 authority
**Current checkpoint:** FL-M5-02 — Approved Setup Apply Engine and Repeat-Safe Asset Creation

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current Focus

Approve the first mutation boundary for First Light setup.

FL-M5-02 creates only missing project-owned foundation content, preserves
existing intent, detects stale plans, compensates for active-attempt failures,
and becomes a no-op on repeated Apply.

## Starting State

- Branch: `main`
- HEAD: `4c4d168`
- `main` equals `origin/main`
- Working tree: clean
- FL-M5-01 authority: `b6a4f27`
- FL-M5-01 implementation: `453bc14`
- FL-M5-01 documentation: `4c4d168`
- EditMode baseline: 93 passed
- Runtime Play Mode baseline: 479 passed
- Compilation baseline: 0 errors, 0 warnings
- Current specification: v1.7.0
- FL-M5-02 implementation locked until authority commit

## Approved Decisions

- `[DECISION]` Advance specification to v1.8.0.
- `[DECISION]` Recollect and replan before writes.
- `[DECISION]` Execute Create, Reuse, and NoChange only.
- `[DECISION]` Never overwrite, move, delete, repair, or migrate existing assets.
- `[DECISION]` Create folders, definitions, configuration, variant, Boot, then Build Settings.
- `[DECISION]` Root is a project-owned prefab variant.
- `[DECISION]` Preserve open/active/dirty scene state.
- `[DECISION]` Append Boot by default; place-first requires approval.
- `[DECISION]` Preserve unrelated Build Settings order/enabled states.
- `[DECISION]` Allow one active Apply.
- `[DECISION]` Use an in-memory compensating rollback journal.
- `[DECISION]` Return immutable apply results.
- `[DECISION]` Second and third Apply must be NoChanges.
- `[DECISION]` Defer repair, migration, receipts, uninstall, Direct Scene, Validator, and Laboratory.

## New Diagnostics

- `ELAUNCH-SETUP-008` stale plan
- `ELAUNCH-SETUP-009` apply active
- `ELAUNCH-SETUP-010` failed and rolled back
- `ELAUNCH-SETUP-011` rollback incomplete
- `ELAUNCH-SETUP-012` unauthorized apply operation

## Next Action

Apply, review, commit, and push the six-file authority update:

```text
echo-launch: approve FL-M5-02 repeat-safe setup apply
```

## Handoff

**Checkpoint:** FL-M5-02
**Baseline:** `4c4d168`
**Implementation:** Locked
**Blockers:** None
**Tentative next:** FL-M5-03 — Explicit Setup Repair and Existing-Asset Reconciliation
