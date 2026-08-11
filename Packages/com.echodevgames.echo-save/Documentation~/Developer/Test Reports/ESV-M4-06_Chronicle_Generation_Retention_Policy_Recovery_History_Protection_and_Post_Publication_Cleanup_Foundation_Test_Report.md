---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-10
---
# ESV-M4-06 — Chronicle Generation Retention Policy, Recovery-History Protection, and Post-Publication Cleanup Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-06
**Unity:** 6000.3.8f1
**Planning/activation commit:** `3d8e0b8`
**Implementation commit:** `e714a90`
**Final effective runtime baseline:** `e714a90`

## Final Result

`EchoDevGames.EchoSave.Tests.Editor`

**497 / 497 passed, 0 failed**

Prior focused regression floor: **473 / 473**

Net new focused tests: **24**

## Evidence Summary

The final green gate covers:
- retention policy bounds and safe minimum;
- provider-neutral bounded generation discovery;
- optional tree-deletion capability;
- unchanged base `ISaveStorageBackend`;
- canonical/committed candidate filtering;
- current-generation protection;
- immediate-predecessor protection;
- deterministic oldest-first cleanup;
- preservation of noncanonical/untrustworthy history;
- discovery-limit fail-closed behavior;
- unsupported deletion capability;
- deletion failure truth;
- no retention before successful head publication;
- shared manual/autosave maintenance mapping;
- public retention result exposure;
- deferred recovery/slot/scene/DDOL boundaries.

## Intermediate Failure

First run:
- discovered: **497**
- passed: **495**
- failed: **2**

Failures:
- `RetentionRunsOnlyAfterSuccessfulHeadPublication`
- `RetentionFailureNeverFabricatesSaveRollback`

Root cause:
- integration setup registered no participant;
- manual-save capture failed before publication/retention;
- the failure-injection integration test also passed at the wrong earlier boundary.

Correction:
- test-only registration of one ordinary participant in all three integration cases.

Final rerun:
**497 / 497 passed, 0 failed**

## Scope Integrity

Final committed implementation/test scope:
- **33 files**
- **2136 insertions**
- **12 deletions**

Still outside the checkpoint:
- recovery execution/fallback;
- quarantine;
- destructive slot operations/trash;
- persistent catalog cache;
- generic operation queues;
- automatic autosave timers;
- full configuration/Setup expansion;
- scene travel;
- peer bridges;
- project-wide DDOL.
