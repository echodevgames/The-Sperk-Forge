---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-11
---
# ESV-M4-10 — Chronicle Destructive Slot Deletion Planning, Confirmed Trash, and Bounded Trash Retention Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-10
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.34.0
**Decision:** ESV-D-032
**Planning baseline:** `4d2f2ac`
**Planning/activation commit:** `2244e3c`
**Implementation commit:** `01e4cdd`
**Unity baseline:** 6000.3.8f1
**Prior focused regression floor:** **562 / 562**
**Final focused Chronicle Editor gate:** **587 / 587**, `0` failed
**Net new focused tests:** **25**
**Committed implementation/test scope:** **28 files**, `2863` insertions, `6` deletions

## Outcome

ESV-M4-10 completes the bounded destructive CAP-017 runtime slice authorized by ESV-D-032.

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
DELETE COMMITTED
        ↓
active-slot/catalog reconciliation
        ↓
bounded trash maintenance
```

## Prepare-delete truth

Preparation:
- targets one explicit canonical live slot;
- performs zero durable mutation;
- does not acquire mutating admission merely to inspect;
- does not clear active selection;
- does not invoke participant callbacks;
- binds package/session identity;
- binds exact slot/head/current-generation provenance;
- records issue/expiry truth;
- produces one immutable one-use confirmation plan;
- opens only lightweight source evidence required for provenance.

A slot ID alone is never sufficient destructive confirmation.

## Confirm-delete truth

Confirmation:
- requires Ready state;
- reuses the existing root-local mutating-operation admission authority;
- returns Busy immediately when admission is occupied;
- creates no delete queue;
- rejects null, malformed, foreign-session, expired, consumed, or stale plans;
- freshly rebuilds/revalidates source truth before destructive mutation;
- consumes the plan at the destructive execution boundary;
- invokes no participant capture/apply/default/migration callbacks.

## Recoverable-trash durability

Safe deletion means the complete canonical live slot tree is moved from `slots/` into package-owned recoverable `trash/`.

Durable delete truth begins only after that complete-tree move succeeds. Pre-commit failure leaves the live slot authoritative.

After the durable boundary:
- the deleted live slot is no longer canonical;
- a deleted active slot is cleared from session selection;
- a different active slot remains selected;
- live catalog truth is refreshed;
- trash does not consume live slot capacity;
- later catalog or trash-maintenance failure cannot fabricate rollback.

## Trash retention

M4-10 applies bounded post-commit trash maintenance:
- canonical package-owned trash records only;
- fail-closed classification;
- deterministic oldest-first excess cleanup;
- maintenance only after durable delete truth and live catalog reconciliation;
- maintenance failure reports committed delete truth separately.

Permanent erase and public restore-from-trash remain separately deferred.

## Registry proofs

M4-10 completes:
- **ESV-T-021 — Delete without plan:** no mutation;
- **ESV-T-022 — Expired delete plan:** rejected;
- **ESV-T-023 — Confirm delete:** recoverable trash/delete policy applied.

## Boundary preservation

M4-10 did not add:
- one-step delete by slot ID;
- permanent erase API;
- public restore-from-trash API;
- direct filesystem authority to the deletion core;
- participant persistence callbacks;
- generic operation queues;
- automatic recovery fallback;
- automatic autosave timers;
- persistent catalog cache;
- M5 Editor tooling;
- scene travel;
- peer bridges;
- service-locator behavior;
- Chronicle-owned/project-wide DDOL.

## Closeout

ESV-M4-10 is **complete** at implementation commit `01e4cdd`.

Final focused evidence is **587 / 587 passed, 0 failed**, preserving the prior **562 / 562** Chronicle regression floor.

No follow-on implementation checkpoint is activated by this closeout.

The next action is the dedicated **M4 milestone reconciliation**. M4 must not be declared complete and M5 must not be activated until that audit reconciles CAP-002 through CAP-018, applicable test-registry truth, documentation, and implemented runtime truth.
