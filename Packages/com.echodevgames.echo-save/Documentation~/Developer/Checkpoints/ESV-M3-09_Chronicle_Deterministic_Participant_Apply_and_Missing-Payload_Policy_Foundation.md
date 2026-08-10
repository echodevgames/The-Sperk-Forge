---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
  - sfgss/closeout
status: complete
updated: 2026-08-10
---

# ESV-M3-09 — Chronicle Deterministic Participant Apply and Missing-Payload Policy Foundation

**Package:** The Chronicle (`EchoSave`)
**Milestone:** M3 — Participants and Loading
**Implementation commit:** `568fa3a`
**Unity:** 6000.3.8f1
**Result:** **Complete**

## Outcome

ESV-M3-09 is the first Chronicle checkpoint that mutates participant-owned gameplay state, and it does so through a complete preflight → deterministic plan → ownership-revalidated execution boundary.

Delivered:
- additive optional `ISaveDefaultableParticipant.InitializeDefault()` capability;
- unchanged base `ISaveParticipant` contract;
- explicit prepared/default/ignore action kinds;
- payload-free structured ordered apply reports;
- zero-callback complete apply preflight;
- deterministic current-registration apply planning;
- prepared schema/runtime-type compatibility validation;
- canonical participant ownership token capture/revalidation;
- explicit `InitializeDefault` / `Ignore` / `Fail` missing-payload semantics;
- prepared `Apply(detachedState)` execution;
- explicit `InitializeDefault()` execution;
- bounded participant-returned failure and exception conversion;
- accurate completed/failed/not-attempted reporting;
- live-handle retry after zero-callback preflight rejection;
- terminal prepared-load `Consumed` state after execution begins;
- replay rejection after consumption;
- zero source-save mutation;
- zero scene/DDOL authority;
- no automatic rollback/compensation fiction.

## Evidence

Final focused gate:

`EchoDevGames.EchoSave.Tests.Editor — 366 / 366 passed, 0 failed`

The complete prior **332 / 332** Chronicle regression floor remained green.

M3-09 added **34** focused tests:
- `ISaveDefaultableParticipantTests` — 3;
- `SaveParticipantApplyExecutorTests` — 10;
- `SaveParticipantApplyPlannerTests` — 12;
- `SaveParticipantRegistryOwnedResolutionTests` — 2;
- `SavePreparedLoadApplyCoordinatorTests` — 7.

## Delivery notes

Two helper defects occurred before the final Unity proof:
1. the first implementation archive contained a malformed generated Git patch and stopped before repository mutation;
2. the corrected archive applied all 42 implementation files, then its post-copy CMD validator stopped because the literal `Apply(null)` text was parsed inside a parenthesized batch block.

Neither incident was a Chronicle runtime defect. The second run left the complete intended implementation applied, Unity compiled it, and the focused gate passed **366 / 366**.

## Boundary preserved

ESV-M3-09 does not activate:
- production async operation admission/cancellation;
- convenience load-and-apply;
- scene travel;
- rollback/compensation contracts;
- document migration;
- slot operations/catalog/session selection;
- recovery;
- retention;
- autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

## Milestone closeout

**M3 — Participants and Loading is complete.**

Chronicle now has the bounded participant/loading foundation required by the approved milestone: open-ended participant registration, capture/publication, unknown preservation/carry-forward, current/migrated preparation, prepared-load lifetime, and deterministic participant apply.

## Next

`ESV-M4-01 — Chronicle Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation`

M4-01 begins M4 with provider-neutral technical slot discovery, payload-free metadata reconstruction from head/current-manifest authority, deterministic immutable catalog snapshots, and session-only active-slot selection.
