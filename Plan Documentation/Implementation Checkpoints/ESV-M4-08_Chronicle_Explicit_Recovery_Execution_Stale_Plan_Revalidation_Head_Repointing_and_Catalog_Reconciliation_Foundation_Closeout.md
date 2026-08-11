---
tags:
  - sfgss/checkpoint-closeout
  - sfgss/package/chronicle
status: complete
updated: 2026-08-10
---
# ESV-M4-08 — Chronicle Explicit Recovery Execution, Stale-Plan Revalidation, Head Repointing, and Catalog Reconciliation Foundation — Closeout

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-08
**Decision:** ESV-D-030
**Planning baseline:** `0396adb`
**Planning/activation commit:** `c324aa4`
**Implementation commit:** `1985fb0`
**Unity baseline:** 6000.3.8f1
**Final focused gate:** **540 / 540 passed, 0 failed**
**Prior floor:** **524 / 524**
**Net new focused tests:** **16**
**Implementation/test scope:** **18 files**, `1846` insertions, `10` deletions

## Closeout decision

ESV-M4-08 is **complete**.

The Chronicle now owns one explicit, bounded recovery-execution path over the read-only M4-07 recovery-plan seam.

The operation:
1. enters the existing root-local mutation admission authority;
2. rejects Busy rather than queueing;
3. rebuilds current M4-07 recovery evidence after admission;
4. rejects stale plan provenance before mutation;
5. requires the selected candidate to remain fully verified;
6. republishes only `head.json`;
7. never rewrites immutable generation contents;
8. treats successful head publication as durable recovery commit;
9. reconciles the derived slot catalog afterward;
10. preserves truthful committed-head/catalog-reconciliation partial state.

## ESV-D-030 confirmation

ESV-D-030 is satisfied.

A supplied recovery plan is not mutation authority by itself. Execution revalidates the source snapshot and selected candidate after acquiring admission and before publishing the new head.

If that source snapshot changed, the operation returns stale-plan truth and performs no head mutation.

## Durable commit semantics

The selected recovery generation already exists as one fully verified immutable committed generation.

M4-08 does not manufacture a new recovery copy. It changes only the small current pointer.

`head.json` publication is the recovery commit boundary.

A later catalog-refresh failure does not undo or disguise the committed head.

## Test history

The implementation payload initially hit one compile-only test defect: the new public-recovery test referenced `FakeManualSaveTransactionExecutor.CallCount`, while the existing fake exposes `Calls`.

The two references were corrected test-only.

No runtime implementation, public API, architecture, ESV-D-030 authority, recovery behavior, test intent, or discovery shape changed.

Final focused result:

```text
540 / 540 passed
0 failed
```

## Deferred boundary

Still not activated:
- automatic/configured recovery fallback;
- recovery-on-load;
- quarantine/incomplete-generation cleanup;
- slot rename/duplicate/delete/trash;
- persistent catalog cache;
- generic operation queues/capacity/overflow;
- recovery cancellation overload;
- automatic autosave timers;
- production permission-provider wiring;
- full recovery/configuration/Setup authoring;
- recovery-time document/participant migration;
- scene travel;
- peer bridges;
- service-locator behavior;
- Chronicle/project-wide DDOL ownership.

## Next checkpoint

No follow-on M4 checkpoint is automatically activated.

Any further Chronicle runtime implementation requires a separately bounded Checkpoint Build Plan and must preserve the **540 / 540** focused regression floor.
