---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M2-01 — Chronicle Storage Root, Path Safety, and Local Backend Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-01
**Implementation commit:** `e4ef76c`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 40 / 40**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 40 |
| Passed | 40 |
| Failed | 0 |
| Ignored | 0 reported |

## Covered proof

The final focused suite includes:

- M1 authority and duplicate-safety regressions;
- missing/invalid configuration behavior;
- shutdown/re-claim behavior;
- stable provider IDs;
- safe storage-key normalization;
- rooted/traversal/mixed-separator rejection;
- safe physical containment under a sandbox root;
- production-root resolution contract;
- local backend initialization;
- exact-byte write/read round trip;
- structured missing-read result;
- create-only conflict preserving original bytes;
- operation-before-initialize rejection;
- backend initialization failure;
- duplicate-before-storage-root creation.

## Development failure and correction

Initial focused run:

`29 / 40 passed; 11 failed`.

Failure concentration:

- `EchoSaveRootAuthorityTests`: 8 failures;
- `EchoSaveStorageLifecycleTests`: 3 failures;
- storage backend/key/provider tests: green.

Root cause was test lifecycle activation, not storage behavior. Direct EditMode `AddComponent` construction did not reliably execute the root's `Awake()` authority path before authority-dependent test operations.

A narrow internal test activation seam was added and the exact production `Awake()` path remained authoritative.

Final rerun:

`40 / 40 passed; 0 failed`.

## Evidence boundary

This report qualifies the ESV-M2-01 storage/path/backend foundation only. It does not qualify package save documents, immutable generation commits, slots, participants, migrations, integrity/recovery, autosave, or release readiness.
