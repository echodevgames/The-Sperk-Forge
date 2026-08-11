---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-10
---
# ESV-M4-07 — Chronicle Recovery Candidate Discovery, Immutable Recovery Plan Truth, and Deterministic Fallback Selection Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-07
**Unity:** 6000.3.8f1
**Planning/activation commit:** `7b00503`
**Implementation commit:** `9f68555`
**Final effective runtime baseline:** `9f68555`

## Final Result

`EchoDevGames.EchoSave.Tests.Editor`

**524 / 524 passed, 0 failed**

Prior focused regression floor: **497 / 497**

Net new focused tests: **27**

## Evidence Summary

The final green gate covers:
- healthy current -> recovery not required;
- missing head -> newest verified committed candidate;
- corrupt current -> prior valid candidate;
- current missing;
- malformed/unsupported head;
- deterministic candidate order and generation-ID tie-break;
- no valid candidate -> source preserved;
- checksum/integrity mismatch exclusion;
- manifest/payload inventory mismatch exclusion;
- unsupported newer generation preservation/exclusion;
- uncommitted generation exclusion;
- noncanonical child preservation/exclusion;
- bounded discovery failure;
- provider discovery/read failure;
- zero mutation audit;
- read-only public service surface;
- no active-slot requirement for explicit-slot planning;
- no mutating admission lease for planning;
- deterministic provenance and provenance change detection;
- absence of recovery execution/path/catalog surface from the plan model.

## Intermediate Corrections

Compile correction:
- `SaveRecoveryTestSupport.cs` referenced nonexistent `SaveDocumentVersions.HeadMajor`;
- corrected to authoritative `SaveDocumentVersions.HeadPointerMajor`;
- test support only.

First focused run:
- discovered: **524**
- passed: **522**
- failed: **2**

Failures:
- `UnsupportedGenerationIsPreservedAndExcluded`
- `UnsupportedHeadStillAllowsVerifiedCandidatePlan`

Root cause:
- the production serializer correctly validates package-document versions before serialization;
- intentionally unsupported test DTOs were rejected before durable fixture JSON could be produced;
- the recovery planner therefore never received the evidence under test.

Correction:
- ordinary fixtures remain on Chronicle's serializer;
- intentionally unsupported future-version fixtures are serialized directly with Unity `JsonUtility` after mutation.

Final rerun:
**524 / 524 passed, 0 failed**

## Scope Integrity

Final committed implementation/test scope:
- **22 files**
- **2912 insertions**
- **6 deletions**

Still outside the checkpoint:
- recovery execution/head rewrite/catalog reconciliation;
- automatic fallback;
- recovery mutation admission/cancellation;
- quarantine;
- destructive slot operations/trash;
- persistent catalog cache;
- generic queues;
- automatic autosave timers;
- full recovery/configuration/Setup expansion;
- scene travel;
- peer bridges;
- project-wide DDOL.
