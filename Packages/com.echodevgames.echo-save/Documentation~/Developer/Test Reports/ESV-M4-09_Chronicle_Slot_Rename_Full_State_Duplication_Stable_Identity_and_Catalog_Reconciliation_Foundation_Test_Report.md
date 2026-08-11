
---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: passed
updated: 2026-08-10
---
# ESV-M4-09 Chronicle Test Report

**Checkpoint:** ESV-M4-09
**Unity:** 6000.3.8f1
**Planning/activation commit:** `7d2d987`
**Implementation commit:** `459023f`
**Test assembly:** `EchoDevGames.EchoSave.Tests.Editor`
**Prior focused floor:** **540 / 540**
**Final discovered total:** **562**
**Passed:** **562**
**Failed:** **0**
**Ignored/errors:** none reported in the final focused gate
**Net new focused tests:** **22**

## Final result

```text
EchoDevGames.EchoSave.Tests.Editor
562 / 562 passed
0 failed
```

The entire previously green **540 / 540** Chronicle floor remained green.

## Scope under test

The M4-09 focused proof covers:
- additive public rename/duplicate service operations;
- root-local admission reuse;
- Busy/no-queue overlap behavior;
- pre-Ready and shutdown lifecycle truth;
- stable rename slot identity/path;
- immutable rename generation publication;
- source payload/state preservation;
- source provenance/freshness revalidation;
- expected-current-generation protection;
- rename retention maintenance;
- active-slot preservation;
- duplicate capacity enforcement;
- bounded duplicate ID collision handling;
- new duplicate slot/generation identity;
- fully verified duplicate source-state copy;
- source committed-byte immutability;
- destination head-last publication;
- duplicate no-auto-select;
- post-publication catalog reconciliation;
- committed truth after reconciliation failure;
- absence of participant callbacks;
- absence of delete/trash and generic-queue scope.

## Evidence interpretation

The first reported M4-09 implementation run compiled and the focused gate passed **562 / 562**. No M4-09 runtime/test hotfix was required between implementation apply and the committed green result.

The closeout does not claim:
- prepare-delete / confirm-delete;
- trash/trash retention;
- quarantine or cleanup;
- persistent catalog cache;
- automatic/configured recovery fallback;
- generic queued operation policy;
- automatic autosave scheduling;
- clean-project reproduction;
- player-build qualification;
- release/private-beta qualification.

Those remain separate future evidence gates.
