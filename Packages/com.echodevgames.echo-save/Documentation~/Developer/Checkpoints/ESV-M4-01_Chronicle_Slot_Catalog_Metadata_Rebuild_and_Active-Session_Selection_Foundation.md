---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-10
---

# ESV-M4-01 — Chronicle Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-01
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Implementation commit:** `62e8a54`
**Unity baseline:** 6000.3.8f1
**Focused Chronicle gate:** **403 / 403 passed, 0 failed**
**Prior regression floor:** **366 / 366**

## Outcome

M4-01 established Chronicle's first real slot-catalog/session layer.

Completed:
- additive optional `ISaveStorageDiscoveryBackend`;
- unchanged base `ISaveStorageBackend`;
- bounded path-redacted immediate-child discovery;
- default local provider implementation;
- canonical technical `SaveSlotId` filtering;
- deterministic slot ordering;
- successful empty catalog when `slots` is absent;
- payload-free `head.json` + current `manifest.json` reconstruction;
- zero normal catalog `payload.json` reads;
- healthy/degraded slot health classification;
- immutable deterministic catalog snapshots;
- prior-snapshot preservation when an overall refresh cannot be trusted;
- explicit session-only active-slot select/no-change/reject/clear;
- stale selection reconciliation after successful catalog replacement;
- zero automatic selection;
- zero durable writes for active selection;
- zero participant callbacks;
- no scene/DDOL authority.

## Evidence

Final Unity Test Runner evidence:
- `EchoDevGames.EchoSave.Tests.Editor`: **403 / 403**;
- failures: **0**;
- prior Chronicle floor: **366 / 366** preserved;
- net new focused tests: **37**.

Repository implementation:
- commit `62e8a54`;
- 43 files changed;
- 3298 insertions;
- 1 deletion;
- post-commit working tree clean and `origin/main` aligned.

## Repair trail

The final committed implementation includes three narrow pre-commit repairs:
1. the first distribution archive's tracked-file patch rejected before it changed the repository;
2. a test-only NUnit type-constraint compile hotfix replaced six incompatible `Does.Not.Contain(typeof(...))` assertions without runtime changes;
3. three catalog test fixtures were corrected so they exercised invalid canonical IDs and unsupported package-document versions accurately.

No runtime architecture was changed by those test repairs.

## Boundary preserved

Still absent:
- persistent `catalog.cache.json`;
- physical slot create/rename/duplicate/delete;
- production operation admission/coalescing/cancellation;
- autosave;
- retention;
- recovery;
- document migration;
- scene travel;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

## Stop point

Chronicle can now discover and describe existing technical slots without opening participant payloads, retain degraded technical slots honestly, preserve the last trustworthy catalog snapshot when refresh itself is untrustworthy, and maintain explicit non-durable active-slot state.

ESV-M4-02 is authorized to begin bounded technical slot creation.
