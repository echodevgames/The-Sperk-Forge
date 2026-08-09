---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M2-03 — Chronicle Generation Identity, Integrity, and Commit-Document Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-03
**Implementation commit:** `ad3b646`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 87 / 87**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 87 |
| Passed | 87 |
| Failed | 0 |
| Ignored | 0 reported |

## Covered proof

The final focused suite includes:
- all M1 authority/duplicate/lifecycle regressions;
- all M2-01 storage/path/local-backend regressions;
- all M2-02 serializer/document regressions;
- slot technical-ID generation/validation/equality;
- generation technical-ID uniqueness/canonical/sortable behavior;
- unsafe/noncanonical technical-ID rejection;
- SHA-256 known-vector calculation;
- checksum success/mismatch/invalid-input behavior;
- manifest/payload identity agreement;
- payload length/checksum agreement;
- integrity-provider identity agreement;
- transport inventory agreement;
- manifest/payload/head serializer round trips;
- head identity/version validation;
- empty transport payload round trip;
- zero storage mutation from pre-publication document/integrity validation.

## Evidence boundary

This report qualifies the pre-publication identity/document/integrity foundation only. It does not qualify physical immutable-generation publication, head replacement, slot management, participants, recovery, retention, autosave, prepared loads, migration, or release readiness.
