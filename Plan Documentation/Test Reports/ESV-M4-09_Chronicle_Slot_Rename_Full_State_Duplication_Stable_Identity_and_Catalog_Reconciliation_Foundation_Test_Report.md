
---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: passed
updated: 2026-08-10
---
# ESV-M4-09 — Chronicle Slot Rename and Full-State Duplication — Test Report

**Implementation commit:** `459023f`
**Unity:** 6000.3.8f1
**Assembly:** `EchoDevGames.EchoSave.Tests.Editor`

## Result

| Measure | Result |
|---|---:|
| Prior focused floor | 540 / 540 |
| Final discovered | 562 |
| Passed | 562 |
| Failed | 0 |
| Net new focused tests | 22 |

## Verified behavior

The focused suite verifies the bounded M4-09 contract:
- public slot rename and duplicate operations;
- shared root-local mutation admission;
- Busy/no-queue behavior;
- ServiceNotReady and AdmissionClosed lifecycle truth;
- stable slot ID/path across rename;
- immutable metadata-updated rename generation;
- source state/byte preservation;
- exact source freshness revalidation;
- expected-current stale protection;
- rename retention;
- active-slot preservation;
- duplicate canonical capacity enforcement;
- bounded destination slot-ID collision retry;
- new destination slot/generation identities;
- fully verified source-state copy;
- destination head-last publication;
- duplicate no-auto-select;
- truthful catalog-reconciliation partial failure;
- zero participant callbacks;
- no delete/trash or generic queue surface.

## Evidence history

The implementation payload applied cleanly and the first reported focused run was green:

```text
562 / 562 passed
0 failed
```

No M4-09 runtime/test hotfix was required before implementation commit `459023f`.

## Evidence boundary

This report proves the focused Chronicle Editor gate only.

It does not claim:
- full repository aggregate testing;
- Play Mode slot-mutation coverage;
- clean-project reproduction;
- player-build qualification;
- performance/release qualification;
- destructive slot delete/trash behavior;
- automatic recovery policy;
- quarantine/cleanup.

Final authoritative focused evidence for ESV-M4-09 is **562 / 562 passed, 0 failed**.
