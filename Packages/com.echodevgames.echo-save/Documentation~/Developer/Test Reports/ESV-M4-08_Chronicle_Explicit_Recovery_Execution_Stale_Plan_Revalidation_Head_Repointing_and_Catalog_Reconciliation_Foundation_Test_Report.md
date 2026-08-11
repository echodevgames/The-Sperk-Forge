---
tags:
  - sfgss/test-report
  - sfgss/package/chronicle
status: passed
updated: 2026-08-10
---
# ESV-M4-08 Chronicle Test Report

**Checkpoint:** ESV-M4-08
**Unity:** 6000.3.8f1
**Planning/activation commit:** `c324aa4`
**Implementation commit:** `1985fb0`
**Test assembly:** `EchoDevGames.EchoSave.Tests.Editor`
**Prior focused floor:** **524 / 524**
**Final discovered total:** **540**
**Passed:** **540**
**Failed:** **0**
**Ignored/errors:** none reported in the final focused gate
**Net new focused tests:** **16**

## Final result

```text
EchoDevGames.EchoSave.Tests.Editor
540 / 540 passed
0 failed
```

The entire previously green **524 / 524** Chronicle floor remained green.

## Scope under test

The M4-08 focused proof covers:
- additive public `ExecuteRecoveryAsync(SaveRecoveryPlan, SaveRecoveryCandidate)`;
- Ready/admission lifecycle truth;
- Busy rejection with no hidden recovery queue;
- shutdown/admission-closed rejection;
- supplied-plan validation;
- selected-candidate membership validation;
- fresh M4-07 plan rebuild after admission;
- exact source-provenance stale-plan rejection;
- candidate invalidation after planning;
- explicit selection of a non-preferred verified candidate;
- head-only recovery publication;
- successful catalog reconciliation;
- catalog failure after head publication without fabricated rollback;
- selected generation byte immutability;
- recovery-not-required rejection;
- no-valid-candidate rejection;
- active-slot selection preservation;
- absence of participant apply/capture effects.

## Compile correction

The first compilation exposed:

```text
'FakeManualSaveTransactionExecutor' does not contain a definition for 'CallCount'
```

The existing test double uses `Calls`.

The two new M4-08 test references were corrected:

```text
Executor.CallCount
→ Executor.Calls
```

This was a test-only compile correction. It changed no runtime code, API, architecture, authority, recovery behavior, test intent, or NUnit discovery shape.

## Evidence interpretation

The final **540 / 540** focused result supersedes the compile-blocked intermediate state.

The closeout does not claim:
- automatic recovery/fallback policy;
- quarantine or cleanup;
- destructive slot operations;
- generic queued operation policy;
- automatic autosave scheduling;
- clean-project reproduction;
- player-build qualification;
- release/private-beta qualification.

Those remain separate future evidence gates.
