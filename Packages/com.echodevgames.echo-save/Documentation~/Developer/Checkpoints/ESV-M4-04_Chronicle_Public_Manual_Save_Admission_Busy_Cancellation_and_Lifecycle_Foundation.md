---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/closeout
status: complete
updated: 2026-08-10
---
# ESV-M4-04 — Public Manual Save Admission, Busy, Cancellation, and Lifecycle Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-04
**Status:** **COMPLETE**
**Planning/activation commit:** `91dcb62`
**Implementation commit:** `2732aaa`
**Lifecycle-status hotfix:** `09ae8f1`
**Final effective runtime baseline:** `09ae8f1`
**Authority after closeout:** SFGSS-PKG-ECHOSAVE-001 v1.22.0
**Unity:** 6000.3.8f1
**Focused regression floor entering checkpoint:** **439 / 439**
**Final focused gate:** **456 / 456**, `0` failed

## Completed Capability

ESV-M4-04 exposes the first production-facing manual-save facade without replacing the M4-03 durable transaction engine.

Completed behavior:
- public active-slot `SaveRequest` / `SaveOperationResult`;
- additive `IEchoSaveService.SaveAsync(...)`;
- one root-local mutating-operation admission authority;
- immediate Busy rejection for overlapping manual saves with no hidden queue;
- bounded safe pre-publication cancellation;
- Too-Late cancellation truth after durable publication begins;
- shutdown closure of new admission before backend shutdown;
- active durable commit settlement rather than shutdown abandonment;
- public generation/head/catalog truth mapped from M4-03;
- main-thread public completion;
- active-slot and ordinary display-name semantics preserved.

## Verification

Final focused Unity Test Runner evidence:
- `EchoDevGames.EchoSave.Tests.Editor`
- **456 / 456 passed**
- **0 failed**
- prior **439 / 439** regression floor preserved
- **17** net new focused tests

## Defect Resolved During Checkpoint

The first full focused run contained one failure:

`SaveBeforeReadyRejectsWithoutTransactionExecution`

Observed:
- expected `ServiceNotReady`;
- received `AdmissionClosed`.

Cause:
- the root-local operation admission coordinator intentionally begins closed before successful initialization;
- pre-Ready public service preflight consulted that internal closed state and allowed it to override lifecycle truth.

Correction:
- AuthorityClaimed / Initializing / Blocked → `ServiceNotReady`;
- ShuttingDown / Shutdown → `AdmissionClosed`;
- Ready → actual operation admission determines Closed / Busy.

Two patch-format helpers refused safely and made no repository changes. The final v3 hotfix validated the exact committed source identity, replaced only `EchoSaveService.cs`, and produced the final **456 / 456** gate.

## Deferred Scope

Still not owned by M4-04:
- autosave/coalescing;
- generic queued multi-operation scheduler;
- queue capacity / overflow policy;
- permission-provider production facade wiring;
- retention/recovery;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- full slot-policy/configuration expansion;
- document migration;
- scene travel;
- peer bridges;
- project-wide DDOL/service-locator composition.

## Stop Point

ESV-M4-04 is complete.

No follow-on M4 checkpoint is activated by this closeout. Further runtime implementation requires a new bounded Checkpoint Build Plan and must preserve the **456 / 456** focused Chronicle regression floor.
