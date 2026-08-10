---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M3-06 — Chronicle Current-Version Participant Payload Preparation, Trusted Runtime-Type Deserialization, and Prepared-Participant Batch Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-06
**Implementation commit:** `050bfa0`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 261 / 261**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 261 |
| Passed | 261 |
| Failed | 0 |
| Prior regression floor | 243 / 243 preserved |
| New M3-06 focused tests | 18 |

## M3-06 coverage

Coverage includes:
- validated participant snapshot exposure and defensive copying;
- source slot/generation provenance;
- current canonical participant preparation;
- deterministic canonical order;
- persisted alias/current canonical-owner provenance;
- trusted live runtime DTO `Type`;
- Unity JSON runtime-Type deserialization;
- alternate registered runtime-Type serializer resolution;
- unknown-payload serializer bypass;
- missing/unusable trusted type failure;
- older-schema migration-required failure;
- newer-schema unsupported-newer failure;
- missing/non-runtime serializer failure;
- malformed payload and invalid detached-state failure;
- duplicate canonical-owner failure;
- all-or-nothing batch behavior;
- zero participant `Capture`;
- zero participant `Apply`.

## Evidence boundary

This report qualifies current-version known-participant preparation only.

Participant migration chains, document migrations, `PreparedSaveLoad`, participant apply, production operation admission, slots, recovery, retention, autosave, and release readiness remain later work.
