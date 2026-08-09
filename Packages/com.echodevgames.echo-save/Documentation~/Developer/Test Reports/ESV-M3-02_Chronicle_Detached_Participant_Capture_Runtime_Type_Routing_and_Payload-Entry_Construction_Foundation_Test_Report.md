---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M3-02 — Chronicle Detached Participant Capture, Runtime Type Routing, and Payload-Entry Construction Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-02
**Implementation commit:** `e34d6d7`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 171 / 171**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 171 |
| Passed | 171 |
| Failed | 0 |
| Ignored | 0 reported |

## Added M3-02 coverage

The focused suite added **24** passing tests over the prior **147**-test regression floor.

Coverage includes:
- trusted runtime-Type Unity JSON serialization/deserialization;
- no CLR type metadata injected into serialized DTO payload;
- deterministic participant capture order;
- default and explicit serializer-provider routing;
- exact UTF-8 payload byte length;
- exact per-entry SHA-256 checksum;
- payload/inventory metadata agreement;
- Required/Optional transport projection;
- capture failure whole-batch abort;
- null successful capture abort;
- detached-state type mismatch abort;
- untyped participant capture rejection;
- missing serializer rejection;
- serializer-without-runtime-Type-capability rejection;
- serializer failure abort;
- integrity failure abort;
- future participant same-pipeline capture;
- defensive-copy capture-batch access;
- zero filesystem mutation by the capture coordinator.

## Evidence boundary

This report qualifies deterministic detached participant capture and verified in-memory transport-entry construction only. It does not qualify participant-backed physical generation publication, production `SaveAsync`, load/apply, unknown-payload carry-forward, migration, slot policy, recovery, retention, autosave, or release readiness.
