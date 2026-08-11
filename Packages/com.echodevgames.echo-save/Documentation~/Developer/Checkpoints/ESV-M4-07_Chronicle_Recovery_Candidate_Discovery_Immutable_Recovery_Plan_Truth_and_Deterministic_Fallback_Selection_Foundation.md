---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/closeout
status: complete
updated: 2026-08-10
---
# ESV-M4-07 — Recovery Candidate Discovery, Immutable Recovery Plan Truth, and Deterministic Fallback Selection Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-07
**Status:** **COMPLETE**
**Planning baseline:** `9695450`
**Planning/activation commit:** `7b00503`
**Implementation commit:** `9f68555`
**Final effective runtime baseline:** `9f68555`
**Authority after closeout:** SFGSS-PKG-ECHOSAVE-001 v1.28.0
**Unity:** 6000.3.8f1
**Focused regression floor entering checkpoint:** **497 / 497**
**Final focused gate:** **524 / 524**, `0` failed
**Net new focused tests:** **27**

## Completed Capability

ESV-M4-07 establishes Chronicle's first trustworthy recovery-planning boundary without performing recovery mutation.

Completed behavior:
- public read-only `BuildRecoveryPlanAsync(SaveSlotId)`;
- explicit head/current diagnosis;
- bounded provider-neutral generation discovery;
- full candidate document/integrity/identity/commit-state verification;
- preservation/exclusion of bad, unsupported, incomplete, uncommitted, mismatched, corrupt, and noncanonical evidence;
- deterministic newest-valid ordering with generation-ID tie-break;
- preferred candidate only when recovery is required;
- immutable payload-free recovery plan and candidate summaries;
- technical source-provenance fingerprint for later stale-plan rejection;
- zero durable mutation;
- no participant capture/apply/default or migration side effects;
- no mutating-operation admission lease for read-only planning.

## Verification

Final focused Unity Test Runner evidence:
- `EchoDevGames.EchoSave.Tests.Editor`
- **524 / 524 passed**
- **0 failed**
- prior **497 / 497** regression floor preserved
- **27** net new focused tests

Committed implementation/test scope:
- **22 files**
- **2912 insertions**
- **6 deletions**

## Test-Fixture Corrections

### Head version constant

Compilation first exposed one new test-support reference to nonexistent `SaveDocumentVersions.HeadMajor`. Chronicle's authoritative head document constant is `SaveDocumentVersions.HeadPointerMajor`.

The correction changed test support only. Runtime implementation, public API, architecture, authority, test intent, and discovery shape were unchanged.

### Intentionally unsupported package documents

The first focused run discovered **524** tests with **522 passed / 2 failed**:
- `UnsupportedGenerationIsPreservedAndExcluded`;
- `UnsupportedHeadStillAllowsVerifiedCandidatePlan`.

Chronicle's production serializer correctly refuses to serialize unsupported package-document versions. The tests had mutated supported DTOs to future major versions and then attempted to serialize them through that guarded serializer, so the fixture failed before the recovery planner ever received the unsupported evidence.

The correction kept ordinary supported fixtures on Chronicle's production serializer but authored intentionally unsupported future-version fixture JSON directly with Unity `JsonUtility`.

Runtime implementation, public API, architecture, ESV-D-029 authority, recovery behavior, test intent, and NUnit discovery count remained unchanged.

Final rerun: **524 / 524 passed, 0 failed**.

## Deferred Scope

Still not owned by M4-07:
- recovery execution/head rewrite/publication;
- catalog reconciliation after recovery;
- automatic/configured fallback execution;
- recovery mutation admission/Busy/cancellation;
- stale-plan execution rejection beyond captured provenance;
- quarantine movement;
- incomplete-generation cleanup;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- generic operation queues/capacity/overflow;
- automatic autosave timers/gameplay triggers;
- permission-provider production wiring;
- full recovery/configuration/Setup expansion;
- document migration;
- scene travel;
- peer bridges;
- project-wide DDOL/service-locator composition.

## Stop Point

ESV-M4-07 is complete.

No follow-on M4 checkpoint is activated by this closeout. Further runtime implementation requires a new bounded Checkpoint Build Plan and must preserve the **524 / 524** focused Chronicle regression floor.
