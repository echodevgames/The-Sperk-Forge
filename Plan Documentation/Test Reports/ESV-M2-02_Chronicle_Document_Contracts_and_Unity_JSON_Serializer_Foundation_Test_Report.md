---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M2-02 — Chronicle Document Contracts and Unity JSON Serializer Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-02
**Implementation commit:** `6404037`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 57 / 57**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 57 |
| Passed | 57 |
| Failed | 0 |
| Ignored | 0 reported |

## Covered proof

The final focused suite includes:
- all M1 authority/duplicate/lifecycle regressions;
- all M2-01 path safety and local backend regressions;
- serializer provider registration and lookup;
- duplicate serializer-provider rejection;
- default Unity JSON provider resolution;
- plain DTO round trip;
- package envelope round trip;
- null/empty serialize/deserialize rejection;
- obvious malformed JSON rejection;
- unsupported package-document version rejection;
- unsupported document-kind rejection;
- zero serializer-layer filesystem I/O.

## Evidence boundary

This report qualifies package-owned in-memory document/serializer behavior only. It does not qualify physical generation publication, head replacement, slot management, participants, migration/integrity/recovery, autosave, or release readiness.
