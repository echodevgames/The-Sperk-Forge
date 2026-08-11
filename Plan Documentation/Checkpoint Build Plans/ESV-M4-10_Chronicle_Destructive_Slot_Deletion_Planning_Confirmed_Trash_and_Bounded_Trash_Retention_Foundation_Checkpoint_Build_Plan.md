
---
tags:
  - sfgss/checkpoint-build-plan
  - sfgss/package/chronicle
status: complete
updated: 2026-08-11
---
# ESV-M4-10 — Chronicle Destructive Slot Deletion Planning, Confirmed Trash, and Bounded Trash Retention Foundation — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-10
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.34.0
**Decision:** ESV-D-032
**Prior checkpoint:** ESV-M4-09 — **Complete**
**Clean planning baseline:** `4d2f2ac`
**Unity baseline:** 6000.3.8f1
**Carried focused regression floor:** **562 / 562**

## 1. Checkpoint purpose

Complete the remaining destructive CAP-017 runtime slice without broadening Chronicle into permanent erasure tooling, a generic operation scheduler, automatic recovery, or M5 Editor tooling.

The checkpoint provides one safe two-step deletion flow:

```text
PrepareDeleteSlotAsync(slot)
        ↓
immutable bounded deletion plan
ZERO durable mutation
        ↓
explicit caller confirmation
        ↓
ConfirmDeleteSlotAsync(plan)
        ↓
root-local admission
        ↓
fresh exact-source revalidation
        ↓
recoverable trash publication
        ↓
live slot removed
        ↓
active-slot/catalog truth reconciled
        ↓
bounded trash maintenance
```

## 2. Authority boundary

M4-10 may implement:
- `SaveDeletionPlan` and bounded public plan/result/status contracts;
- `PrepareDeleteSlotAsync(SaveSlotId)`;
- `ConfirmDeleteSlotAsync(SaveDeletionPlan)`;
- package/session ownership tokens;
- injected-clock issue/expiry truth;
- one-use/replay protection;
- exact source head/current-generation provenance;
- optional provider-neutral tree move/rename capability if needed;
- recoverable package-owned trash records;
- bounded trash retention cleanup after durable deletion;
- catalog refresh/reconciliation;
- active-slot clearing after durable removal;
- focused runtime/storage/admission tests.

M4-10 may not implement:
- one-step delete by slot ID;
- permanent erase as the default/public operation;
- public restore-from-trash;
- quarantine/incomplete-generation cleanup;
- persistent catalog cache;
- automatic recovery fallback;
- recovery-on-load;
- generic operation queues;
- automatic autosave scheduling;
- M5 Setup/Validator/Browser/Simulator/Laboratory UI;
- scene travel, peer bridges, service locator, DDOL.

## 3. Two-step plan contract

`PrepareDeleteSlotAsync` must:
1. validate one canonical existing slot;
2. collect lightweight source metadata without opening participant payloads unnecessarily;
3. bind exact current source provenance sufficient to reject stale confirmation;
4. create one immutable package/session-owned plan;
5. give the plan a bounded lifetime;
6. perform zero durable mutation;
7. not clear active selection;
8. not acquire destructive admission merely to inspect.

Plan data should be sufficient for a project/Laboratory presenter to display:
- technical slot ID;
- display name/health summary where safely available;
- current generation identity/provenance;
- issue/expiry time;
- one-use plan identity;
- whether the slot is currently selected.

## 4. Confirm-delete contract

`ConfirmDeleteSlotAsync` must:
1. reject pre-Ready with `ServiceNotReady`;
2. reject shutdown/closed admission with `AdmissionClosed`;
3. reject overlap with `Busy`;
4. reject null/malformed/foreign-session plans;
5. reject expired plans;
6. reject consumed/replayed plans;
7. freshly rebuild/revalidate source/catalog truth;
8. reject stale slot/head/current-generation provenance before mutation;
9. consume the plan deterministically at the destructive execution boundary;
10. invoke no participant callbacks.

No code path may accept only a `SaveSlotId` as sufficient destructive confirmation.

## 5. Recoverable trash durability

For this checkpoint, safe deletion means moving the complete canonical slot tree from live `slots/` into package-owned recoverable `trash/`.

Requirements:
- destination identity is package-generated and collision-safe;
- source slot data is not rewritten merely to delete it;
- successful trash publication must remove the live canonical slot from normal catalog discovery;
- partial pre-commit failure must leave the live slot authoritative;
- successful durable trash move is delete commit truth;
- post-commit catalog/maintenance failure cannot fabricate rollback;
- trash does not count toward live slot capacity.

The base `ISaveStorageBackend` contract remains unchanged. Any storage-specific tree move capability must be additive/optional and provider-neutral.

## 6. Active-slot truth

If the deleted slot is active:
- active selection remains unchanged before durable delete commit;
- it clears only after durable removal from live slots succeeds.

If another slot is active:
- its selection remains unchanged.

If post-delete catalog refresh fails:
- the result must still report durable deletion truth separately from reconciliation truth.

## 7. Trash retention

Trash retention is maintenance after delete commit.

The checkpoint must prove:
- a positive bounded trash-history limit;
- deterministic oldest-first cleanup among trusted package-owned trash records;
- the newly committed trash record is never erased before delete truth exists;
- ambiguous/untrusted trash records fail closed;
- cleanup failure does not restore the live slot or report rollback.

Full project-owned retention-policy authoring remains deferred to later configuration/tooling.

## 8. Required tests

Registry-aligned:
- ESV-T-021 — delete without plan causes no mutation;
- ESV-T-022 — expired deletion plan is rejected;
- ESV-T-023 — confirmed deletion applies recoverable trash policy.

Additional focused proofs:
- prepare plan performs zero durable mutation;
- prepare plan does not acquire mutation admission;
- plan immutable/package/session-bound;
- malformed/foreign plan rejected;
- stale source rejects before mutation;
- replay rejects;
- Busy rejects without queue;
- pre-Ready / shutdown lifecycle truth;
- source remains live on pre-commit storage failure;
- confirmed delete removes live canonical slot;
- trash destination is discoverable only as trash, not live slot;
- active-slot clear timing is post-commit only;
- non-active deletion preserves active selection;
- post-commit catalog failure reports committed-but-unreconciled;
- trash retention stays bounded;
- trash retention failure does not fabricate rollback;
- zero participant callbacks;
- base storage contract unchanged.

## 9. Implementation strategy

Prefer the same architectural pattern already proven in M4:
- public service facade handles lifecycle/main-thread/root-local admission;
- deletion planner is read-only and provider-neutral;
- deletion executor owns bounded plan/source validation and durable trash mutation;
- storage-specific whole-tree move is an additive optional provider capability if required;
- catalog remains derived from live heads/manifests;
- cleanup occurs after durable mutation;
- structured result truth separates committed deletion from maintenance/reconciliation.

## 10. Stop conditions

Stop and do not broaden the checkpoint if implementation would require:
- changing the base `ISaveStorageBackend`;
- inventing a generic queue/scheduler;
- permanent erase semantics;
- a restore-from-trash public API;
- M5 Editor tooling;
- automatic recovery;
- project-owned scene/lifetime composition;
- participant callbacks during delete.

Any such need requires a new authority decision or later checkpoint.

## 11. Completion gate

ESV-M4-10 closes only when:
1. Unity compiles cleanly;
2. the complete focused Chronicle Editor assembly passes with the prior **562 / 562** floor preserved;
3. ESV-T-021 through ESV-T-023 are proven;
4. all additional stale/replay/admission/active-slot/trash-retention cases pass;
5. base `ISaveStorageBackend` remains unchanged;
6. no participant callback, scene, DDOL, permanent erase, or generic queue authority enters the runtime;
7. implementation scope is reviewed;
8. documentation records the actual discovered test total;
9. implementation and documentation commits are pushed cleanly.

## 12. Milestone follow-on

ESV-M4-10 is expected to be the final new runtime-capability checkpoint before an **M4 milestone reconciliation**, but M4 completion is not pre-declared.

After M4-10 closes:
1. audit CAP-002 through CAP-018 against committed runtime behavior;
2. audit the applicable ESV-T registry coverage and deferred/not-applicable cases;
3. reconcile configuration/policy claims versus what belongs to M5;
4. run the focused regression gate again if reconciliation changes code/tests;
5. formally close M4 only if the audit is clean.

If M4 closes cleanly, activate **M5 — Tooling and Laboratory** next.


## Closeout evidence

**Planning/activation commit:** `2244e3c`
**Implementation commit:** `01e4cdd`
**Final focused Chronicle Editor gate:** **587 / 587 passed, 0 failed**
**Prior regression floor preserved:** **562 / 562**
**Net new focused tests:** **25**
**Committed implementation/test scope:** **28 files**, `2863` insertions, `6` deletions

Observed closeout:
- implementation payload applied cleanly;
- generated payload and tracked diff whitespace checks passed;
- base `ISaveStorageBackend` remained unchanged;
- public two-step delete surface was present;
- one-step destructive public API remained absent;
- deletion runtime retained no direct filesystem authority;
- participant callbacks remained absent;
- scene/DDOL authority remained absent;
- payload-free planning and exact source revalidation guards passed;
- durable complete-tree trash publication and bounded post-commit trash maintenance guards passed;
- Unity focused gate passed **587 / 587** on the first reported run.

## Checkpoint disposition

ESV-M4-10 is **complete**.

This closeout does not declare M4 complete and does not activate M5.

The next gate is a dedicated M4 milestone reconciliation against CAP-002 through CAP-018, applicable test-registry truth, closeout records, and committed implementation.
