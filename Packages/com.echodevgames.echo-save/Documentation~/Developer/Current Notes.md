# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.16.0
**Completed checkpoint:** ESV-M3-09 — Deterministic Participant Apply and Missing-Payload Policy Foundation
**Completed milestone:** M3 — Participants and Loading
**Current checkpoint:** ESV-M4-01 — Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation
**Status:** M3 complete; ESV-M4-01 active / authorized

## ESV-M3-09 closeout

Implementation commit: `568fa3a`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **366 / 366 passed, 0 failed**;
- prior **332 / 332** Chronicle regression floor remains green;
- 34 new M3-09 tests passed;
- optional `ISaveDefaultableParticipant.InitializeDefault()` is additive;
- base `ISaveParticipant` remains unchanged;
- complete apply preflight invokes zero participant callbacks;
- missing-payload `Fail` blocks before mutation;
- `Ignore` performs no callback;
- explicit default initialization never uses `Apply(null)`;
- prepared state applies deterministically through `Apply(detachedState)`;
- registration ownership is revalidated before mutation;
- zero-callback preflight rejection leaves the prepared handle live;
- execution-time terminal results consume the prepared handle;
- partial failure reports completed/failed/not-attempted truth without automatic rollback fiction;
- source save files remain unchanged;
- scene/DDOL authority remains absent.

## M3 milestone state

**M3 — Participants and Loading is complete.**

Chronicle now has participant registration, detached capture, participant-backed generation publication, opaque unknown preservation/carry-forward, current/migrated payload preparation, prepared-load lifetime/session ownership, and deterministic participant apply.

## Active ESV-M4-01 boundary

Authorized:
- additive provider-neutral storage discovery capability while leaving base `ISaveStorageBackend` unchanged;
- default local backend implementation of bounded child discovery;
- technical slot-root discovery under `slots`;
- payload-free head/current-manifest metadata reconstruction;
- healthy/degraded slot metadata classification;
- deterministic immutable catalog snapshots;
- prior-snapshot preservation on untrustworthy refresh failure;
- session-only active-slot selection/clear/reconciliation;
- zero payload reads for normal catalog refresh;
- zero participant callbacks;
- zero durable writes for active selection.

Still absent:
- persistent `catalog.cache.json` optimization;
- physical slot create/rename/duplicate/delete;
- slot-policy asset expansion;
- production save/load operation admission;
- autosave;
- retention;
- recovery;
- document migration;
- scene travel;
- peer bridges;
- Chronicle-owned/project-wide DDOL.
