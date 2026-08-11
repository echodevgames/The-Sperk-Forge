---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-10
---
# ESV-M4-08 — Chronicle Explicit Recovery Execution, Stale-Plan Revalidation, Head Repointing, and Catalog Reconciliation Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-08
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.30.0
**Decision:** ESV-D-030
**Planning baseline:** `0396adb`
**Planning/activation commit:** `c324aa4`
**Implementation commit:** `1985fb0`
**Unity baseline:** 6000.3.8f1
**Prior focused regression floor:** **524 / 524**
**Final focused Chronicle Editor gate:** **540 / 540**, `0` failed
**Net new focused tests:** **16**
**Committed implementation/test scope:** **18 files**, `1846` insertions, `10` deletions

## Outcome

ESV-M4-08 completes the first explicit durable recovery-execution path built on the immutable M4-07 recovery-plan seam.

Chronicle now exposes:

`ExecuteRecoveryAsync(SaveRecoveryPlan plan, SaveRecoveryCandidate candidate)`

as one explicit root-local mutating operation.

The completed execution path:
- requires the Chronicle service to be Ready;
- reuses the existing root-local mutating-operation admission authority;
- returns Busy immediately while another mutation owns admission;
- creates no recovery queue;
- validates the supplied plan and explicitly selected candidate;
- rebuilds a fresh M4-07 recovery plan after admission;
- compares exact source-provenance fingerprints;
- rejects stale source evidence before any durable mutation;
- requires the selected candidate to remain one exact fully verified candidate;
- keeps the selected committed generation immutable;
- republishes only `head.json`;
- points `head.currentGenerationId` at the explicitly selected verified generation;
- leaves recovery-created `previousGenerationId` empty rather than blessing damaged/untrusted source state;
- refreshes the slot catalog after successful head publication;
- preserves any pre-existing active-slot selection;
- never invokes participant capture/apply/default callbacks;
- reports committed-head truth separately from catalog-reconciliation truth.

## Durable truth boundary

The M4-08 durable recovery boundary is:

```text
fresh plan/candidate proof
        ↓
validated head construction
        ↓
PublishCurrentObject(head.json)
        ↓
RECOVERY COMMITTED
        ↓
catalog reconciliation
```

Before successful head publication, recovery is not committed.

After successful head publication, the selected verified generation is current durable truth. A later catalog-reconciliation failure returns `HeadPublished = true` / `CatalogReconciled = false`; Chronicle does not rewrite the old damaged head merely to make the operation result look tidy.

## Stale-plan safety

ESV-T-079 is now implemented at the execution boundary.

After acquiring mutation admission, Chronicle rebuilds the M4-07 plan and compares:
- target slot;
- source-provenance fingerprint;
- recovery-required state;
- exact selected candidate identity/metadata.

A changed head, changed generation set, corrupted candidate, or otherwise changed source snapshot rejects before head publication.

## Generation immutability

Recovery execution does not:
- create a replacement generation;
- rewrite the selected generation;
- copy selected payload/manifest bytes;
- migrate participant data;
- delete damaged evidence;
- run retention merely because recovery executed.

The selected generation's manifest and payload remain byte-identical across successful recovery execution.

## Catalog reconciliation

After head publication Chronicle refreshes the existing session catalog.

Successful reconciliation requires the recovered slot to report the selected generation as one healthy/selectable current entry.

If the catalog refresh fails after the head commit:
- recovery remains committed;
- the failure is reported as reconciliation maintenance truth;
- no fabricated rollback is attempted.

## Test maintenance

The first compile after the M4-08 payload exposed one test-only mistake:

```text
FakeManualSaveTransactionExecutor.CallCount
```

The existing Chronicle test double exposes:

```text
FakeManualSaveTransactionExecutor.Calls
```

The two new test references were corrected from `CallCount` to `Calls`.

This correction changed:
- runtime implementation: **NO**;
- public API: **NO**;
- architecture: **NO**;
- ESV-D-030 authority: **NO**;
- test intent: **NO**;
- NUnit discovery shape: **NO**.

Compilation then succeeded and the focused gate passed **540 / 540**.

## Explicitly still deferred

ESV-M4-08 does not activate:
- automatic/configured fallback execution;
- implicit recovery during load/prepare;
- quarantine movement;
- incomplete/corrupt generation cleanup;
- slot rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- generic operation queue/capacity/overflow;
- a caller-facing recovery cancellation overload;
- automatic autosave timers/gameplay triggers;
- permission-provider production facade wiring;
- full recovery/configuration/Setup authoring;
- participant/document migration during recovery;
- scene travel;
- peer bridges;
- service locator behavior;
- Chronicle-owned/project-wide DDOL.

## Closeout

ESV-M4-08 is **complete** at implementation commit `1985fb0`.

Final focused evidence is **540 / 540 passed, 0 failed**, preserving the prior **524 / 524** Chronicle regression floor.

No follow-on M4 checkpoint is activated by this closeout.
