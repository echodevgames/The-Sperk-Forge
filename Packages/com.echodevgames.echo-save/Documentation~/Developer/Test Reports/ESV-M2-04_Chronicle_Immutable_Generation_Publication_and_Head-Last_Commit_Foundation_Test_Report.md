---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: complete
updated: 2026-08-09
---

# ESV-M2-04 — Chronicle Immutable Generation Publication and Head-Last Commit Foundation — Test Report

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-04
**Implementation commit:** `01b7ad3`
**Unity:** 6000.3.8f1
**Overall result:** **PASS — 102 / 102**

## Final focused gate

| Metric | Result |
|---|---:|
| Total | 102 |
| Passed | 102 |
| Failed | 0 |
| Ignored | 0 reported |

## Covered proof

The final focused suite includes all prior Chronicle lifecycle/storage/document/serializer/integrity regressions plus:
- explicit local publication capabilities;
- first current-head publication;
- subsequent current-head replacement;
- same-root candidate-to-final publication;
- generation-first/head-last ordering;
- previous-generation preservation;
- head sequence and previous-generation identity advancement;
- candidate payload write-failure preservation;
- candidate manifest write-failure preservation;
- candidate verification-failure preservation;
- generation publication-failure preservation;
- head serialization-failure preservation;
- head publication-failure preservation;
- orphan verified generation remains non-current;
- duplicate generation publication rejection;
- create-only committed-generation immutability.

## Evidence boundary

This report qualifies the bounded package-owned empty/transport publication transaction. It does not qualify participant-backed production `SaveAsync`, slot catalogs, prepared loads, recovery, retention, autosave, migrations, or release readiness.
