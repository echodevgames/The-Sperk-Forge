---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/closeout
status: complete
updated: 2026-08-10
---
# ESV-M4-06 — Chronicle Generation Retention Policy, Recovery-History Protection, and Post-Publication Cleanup Foundation — Closeout

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-06
**Status:** **COMPLETE**
**Planning baseline:** `3cdad0f`
**Planning/activation commit:** `3d8e0b8`
**Implementation commit:** `e714a90`
**Final effective runtime baseline:** `e714a90`
**Authority after closeout:** SFGSS-PKG-ECHOSAVE-001 v1.26.0
**Unity:** 6000.3.8f1
**Focused regression floor entering checkpoint:** **473 / 473**
**Final focused gate:** **497 / 497**, `0` failed
**Net new focused tests:** **24**

## Completed Capability

ESV-M4-06 bounds ordinary committed-generation history without weakening Chronicle's durable-truth model.

Completed behavior:
- project-owned bounded `SaveRetentionPolicy`;
- minimum safe total-generation bound of two;
- provider-neutral bounded generation discovery;
- additive optional `ISaveStorageTreeDeletionBackend`;
- base `ISaveStorageBackend` unchanged;
- fail-closed candidate classification;
- current and immediate predecessor protection;
- deterministic oldest-first deletion of excess verified committed history;
- retention only after successful generation/head publication;
- manual save and autosave reuse the same retention path;
- public retention-maintenance truth;
- cleanup failure never fabricates save rollback.

## Verification

Final focused Unity Test Runner evidence:
- `EchoDevGames.EchoSave.Tests.Editor`
- **497 / 497 passed**
- **0 failed**
- prior **473 / 473** regression floor preserved
- **24** net new focused tests

Committed implementation/test scope:
- **33 files**
- **2136 insertions**
- **12 deletions**

## Intermediate Integration-Test Failure

The first focused run discovered **497** tests with **495 passed / 2 failed**:
- `RetentionRunsOnlyAfterSuccessfulHeadPublication`
- `RetentionFailureNeverFabricatesSaveRollback`

The new integration test fixture created an empty slot but registered no participant. Manual save therefore failed during participant capture before reaching publication/retention.

`FailedPublicationNeverInvokesRetention` also stopped at that earlier boundary and therefore passed for the wrong reason.

A test-only correction registered one ordinary participant in all three integration cases.

Changed by that correction:
- test setup only.

Unchanged:
- runtime implementation;
- public API;
- architecture;
- ESV-D-028 authority;
- NUnit discovery count.

Final rerun: **497 / 497 passed, 0 failed**.

## Deferred Scope

Still not owned by M4-06:
- recovery-plan generation/execution;
- automatic fallback selection;
- quarantine movement;
- rename/duplicate/delete/trash;
- trash-history retention;
- persistent `catalog.cache.json`;
- generic operation queues/capacity/overflow;
- automatic autosave timers/gameplay triggers;
- permission-provider production wiring;
- full configuration/Setup expansion;
- document migration;
- scene travel;
- peer bridges;
- project-wide DDOL/service-locator composition.

## Stop Point

ESV-M4-06 is complete.

No follow-on M4 checkpoint is activated by this closeout. Further runtime implementation requires a new bounded Checkpoint Build Plan and must preserve the **497 / 497** focused Chronicle regression floor.
