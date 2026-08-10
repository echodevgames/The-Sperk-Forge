---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M3-04 — Chronicle Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-04
**Implementation commit:** `aa78e07`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 218 / 218**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 218 |
| Passed | 218 |
| Failed | 0 |
| Ignored | 0 reported |

## Regression delta

Prior focused Chronicle gate: **197 / 197**.

M3-04 added **21** passing focused tests while preserving the complete prior regression floor.

Coverage includes:
- successful current committed generation inspection;
- canonical participant recognition;
- alias participant recognition;
- unknown participant classification;
- exact unknown field and serialized payload preservation;
- deterministic defensive-copy snapshots;
- clear/reset behavior;
- missing/malformed head failure;
- missing current-generation file failure;
- whole payload corruption failure;
- per-entry corruption failure;
- inventory mismatch failure;
- duplicate identity rejection;
- unknown count/aggregate-byte bounds;
- previous valid store preservation across failure;
- no unknown serializer activation;
- no participant capture/apply invocation;
- zero storage mutation by current-generation inspection.

## Evidence boundary

This report qualifies read-only current-generation validation plus opaque session preservation of unclaimed participant entries. It does not qualify unknown carry-forward publication, prune policy, participant deserialization/apply, migration, production save operation admission, slots, recovery, retention, autosave, or release readiness.
