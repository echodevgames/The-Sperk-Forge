---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M3-03 — Chronicle Participant-Backed Generation Publication and Head-Last Integration Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-03
**Implementation commit:** `6970127`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 197 / 197**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 197 |
| Passed | 197 |
| Failed | 0 |
| Ignored | 0 reported |

## Added M3-03 coverage

The focused suite added **26** passing tests over the prior **171**-test regression floor:

- `SaveParticipantGenerationPublicationTests`: **12**;
- `SaveParticipantPublicationBatchValidatorTests`: **14**.

Coverage includes:
- participant-backed first/second generation publication;
- deterministic participant order through disk round-trip;
- payload/manifest inventory agreement;
- strict publication-boundary ID/schema/serializer/flags checks;
- exact per-entry UTF-8 byte-length/checksum revalidation;
- invalid batch zero-storage-mutation behavior;
- candidate payload/manifest failure preservation;
- candidate corruption preservation;
- immutable generation publication failure preservation;
- published-generation corruption preservation;
- head serialization failure orphan behavior;
- head publication failure orphan behavior;
- preservation of the existing M2 empty/transport publication path.

## Compile interruption

Before the final gate, a public parameterized NUnit test used a nested `internal FaultPoint` parameter type and produced CS0051 inconsistent accessibility. The nested enum was changed to `public`. This was test-only and changed no Chronicle runtime behavior.

## Evidence boundary

This report qualifies participant-backed immutable-generation/head-last publication as a bounded technical integration seam. It does not qualify unknown-payload carry-forward, production `SaveAsync`, load/apply, migrations, slot policy, recovery, retention, autosave, or release readiness.
