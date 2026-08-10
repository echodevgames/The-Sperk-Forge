# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.17.0
**Completed checkpoint:** ESV-M4-01 — Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation
**Completed milestone:** M3 — Participants and Loading
**Current checkpoint:** ESV-M4-02 — Technical Slot Creation, Capacity Enforcement, Initial Empty Generation, and Catalog Reconciliation Foundation
**Status:** M3 complete; ESV-M4-01 complete; ESV-M4-02 active / authorized

## ESV-M4-01 closeout

Implementation commit: `62e8a54`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **403 / 403 passed, 0 failed**;
- prior **366 / 366** Chronicle regression floor remains green;
- 37 new focused M4-01 tests passed;
- base `ISaveStorageBackend` remains unchanged;
- provider-neutral slot discovery is additive through `ISaveStorageDiscoveryBackend`;
- default local backend performs bounded immediate-child discovery;
- technical slot discovery accepts canonical `SaveSlotId` children only;
- normal catalog rebuild reads `head.json` + current `manifest.json`, never `payload.json`;
- healthy/degraded catalog metadata is deterministic and immutable;
- untrustworthy overall refresh failure preserves the prior complete snapshot;
- active slot selection is explicit, session-only, non-durable, and never auto-selects;
- refresh removal/unhealthiness clears stale active selection;
- no participant callback, persistent catalog cache, physical slot mutation, autosave, retention, recovery, scene authority, or DDOL ownership was introduced.

Implementation-history note:
- the first archive's tracked-file patch rejected before repository mutation;
- corrected v2 applied against the exact baseline;
- one NUnit `System.Type` constraint compile hotfix changed tests only;
- three failing catalog fixtures were corrected without runtime changes;
- final Unity evidence is the authoritative **403 / 403** gate.

## M4 milestone state

**M4 — Slots / Autosave / Recovery is active.**

Chronicle now has a proven provider-neutral, payload-free slot catalog and safe session-only active-slot selection. Slot mutation has not yet begun.

## Active ESV-M4-02 boundary

### ESV-D-024 — a created slot is a committed generation, not a directory

M4-02 records:

> Chronicle must not call a slot successfully created merely because a technical directory exists. Successful slot creation publishes one verified empty immutable generation and then publishes `head.json` last. Capacity counts every discovered canonical technical slot, including degraded entries, so corrupt/incomplete slots cannot be ignored to bypass limits.

Authorized:
- bounded technical slot-capacity input;
- a technical create request with safe display/project/build metadata;
- package-generated canonical `SaveSlotId`;
- bounded generated-ID collision retry against the current trustworthy catalog;
- collision rejection for every already-discovered canonical technical slot, healthy or degraded;
- initial empty generation publication through the existing immutable generation/head-last transaction;
- an initial-publication entry point that requires no existing current head;
- post-publication catalog refresh/reconciliation;
- truthful structured result when publication succeeds but catalog refresh later fails;
- no automatic active-slot selection after creation;
- zero participant capture/apply/default callbacks.

Still absent:
- persistent `catalog.cache.json` optimization;
- rename / duplicate / delete;
- full single/fixed/configurable/unlimited-profile configuration asset expansion;
- production async operation admission/coalescing/cancellation;
- concurrent public create-operation ownership;
- autosave;
- retention;
- recovery;
- document migration;
- scene travel;
- peer bridges;
- Chronicle-owned/project-wide DDOL.
