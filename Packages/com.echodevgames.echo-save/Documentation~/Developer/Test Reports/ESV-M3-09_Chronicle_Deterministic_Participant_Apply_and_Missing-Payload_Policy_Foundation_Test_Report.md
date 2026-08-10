---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-10
---

# ESV-M3-09 — Chronicle Deterministic Participant Apply and Missing-Payload Policy Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-09
**Implementation commit:** `568fa3a`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 366 / 366**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 366 |
| Passed | 366 |
| Failed | 0 |
| Prior regression floor | 332 / 332 preserved |
| New M3-09 focused tests | 34 |

## New M3-09 test groups

| Test group | Passed |
|---|---:|
| `ISaveDefaultableParticipantTests` | 3 |
| `SaveParticipantApplyExecutorTests` | 10 |
| `SaveParticipantApplyPlannerTests` | 12 |
| `SaveParticipantRegistryOwnedResolutionTests` | 2 |
| `SavePreparedLoadApplyCoordinatorTests` | 7 |

## Coverage

The final gate proves:
- additive default capability surface;
- unchanged base participant contract;
- deterministic zero-callback preflight;
- missing owner/type/schema/duplicate rejection;
- explicit missing-payload `Fail`, `Ignore`, and `InitializeDefault` behavior;
- no `Apply(null)` default protocol;
- prepared detached-state apply;
- explicit default initialization;
- registration ownership-token revalidation;
- structured participant-returned failures;
- bounded callback exception conversion;
- deterministic report ordering;
- not-attempted tail reporting after terminal failure;
- preflight retry leaves the handle live;
- execution consumes the handle;
- consumed-handle replay rejection;
- unknown-only load non-use of participant callbacks;
- zero source-generation/head/payload mutation;
- zero scene/DDOL authority.

## Delivery-helper evidence note

The first archive failed before applying because its generated Git patch was malformed. The corrected archive applied the intended 42 files; its post-copy validation script then hit a CMD parser defect around the literal `Apply(null)` text. The Unity gate is the authoritative implementation proof and passed **366 / 366**.

## Evidence boundary

This report qualifies deterministic participant apply/missing-payload policy only.

Production async operation admission, convenience loading, scene travel, rollback/compensation contracts, document migration, slots/catalog operations, recovery, retention, autosave, and release readiness remain later work.
