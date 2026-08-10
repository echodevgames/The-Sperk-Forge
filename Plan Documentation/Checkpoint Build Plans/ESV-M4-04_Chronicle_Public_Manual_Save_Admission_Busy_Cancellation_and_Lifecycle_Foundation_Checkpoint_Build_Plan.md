---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-10
---
# ESV-M4-04 — Chronicle Public Manual Save Admission, Busy, Cancellation, and Lifecycle Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-04
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.21.0
**Prior checkpoint:** ESV-M4-03 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **439 / 439**
**Exact implementation baseline:** `3a84187`

## 1. Intent

Expose the first real public manual-save facade over the already-proven M4-03 transaction and establish the root-local admission/cancellation/lifecycle boundary that future mutating Chronicle operations can reuse.

M4-04 answers one bounded question:

> Can a project call public `SaveAsync` for the selected active slot and receive deterministic success/failure/Busy/cancel/Too-Late truth while Chronicle admits only one mutating operation, preserves M4-03 durable commit semantics, and prevents shutdown from admitting new work?

```text
public SaveRequest
        ↓
service/lifecycle preflight
        ↓
root-local mutating admission
        ├── occupied → Busy
        └── admitted
              ↓
        cancellation checkpoint
              ↓
        M4-03 manual-save transaction
              ↓
        durable publication boundary
              ↓
        cancellation becomes Too Late
              ↓
        public SaveOperationResult
              ↓
        release admission
```

This is public manual-save admission, not autosave and not the final generic operation scheduler.

## 2. Carried-forward authority

Chronicle already proves:

- M1 lifecycle and duplicate-safe root authority;
- public lifecycle-only `IEchoSaveService`;
- M4-01 explicit session-only active-slot selection;
- M4-03 complete active-slot manual-save transaction composition;
- exact source-provenance refresh and opaque unknown carry-forward;
- deterministic participant capture;
- expected-current-generation stale-source rejection;
- immutable generation publication and `head.json` last;
- truthful generation/head/catalog reconciliation outcomes;
- focused Chronicle Editor **439 / 439**.

### ESV-D-026 — public manual save uses one bounded root-local admission authority

> Public manual save enters through one root-local mutating-operation admission authority. A second manual save received while that authority is occupied returns Busy immediately rather than queueing. Cancellation is honored only before the durable publication boundary. Shutdown closes new admission and allows an already-committing save to settle.

## 3. Authorized implementation scope

### Public request and result surface

Add the first public manual-save DTOs required by the approved runtime API.

The public request may include bounded:

- project ID;
- project version;
- build ID;
- save reason/intent metadata where already authorized;
- cancellation input appropriate to the Unity/.NET baseline.

M4-04 public save remains **active-slot only**.

Do not add caller-controlled filesystem paths or rename/display-name mutation.

The public result must preserve at least:

- invalid request;
- service not ready / admission closed;
- Busy;
- canceled before durable publication;
- cancellation Too Late;
- M4-03 terminal failure mapping;
- durable generation publication truth;
- head publication truth;
- catalog reconciliation truth;
- full success.

Exact enum/type names are implementation details. Public durable truth is not.

### IEchoSaveService expansion

Add only the bounded public save member:

`Awaitable<SaveOperationResult> SaveAsync(SaveRequest request)`

Rules:

- fresh awaitable per call;
- public completion occurs on the main thread;
- existing lifecycle members remain compatible;
- no autosave method is implemented in this checkpoint;
- no public duplicate/delete/recovery methods are added merely to fill out the future interface.

### Root-local mutating-operation admission

Add one package-local admission authority owned by the EchoSave service/root session.

Required semantics:

- at most one mutating operation lease/admission at a time;
- no static/global cross-root lock;
- duplicate rejected roots acquire no admission state;
- manual save acquires/releases admission exactly once;
- overlapping manual save returns Busy immediately;
- Busy does not queue;
- failed validation/cancellation releases admission;
- terminal M4-03 success/failure releases admission;
- admission object is reusable later by duplicate/delete/recovery/autosave checkpoints without those operations being implemented now.

### Cancellation

M4-04 adds bounded manual-save cancellation semantics.

Required boundaries:

- already-canceled request may fail before admission or immediately after safe admission;
- cancellation before participant capture prevents capture/publication;
- cancellation at other safe pre-publication boundaries prevents head advancement;
- once durable generation/head publication begins, cancellation is Too Late;
- Too-Late does not cancel or roll back the durable transaction;
- prior current generation is never deleted because of cancellation;
- public result distinguishes Canceled from Too Late.

The implementation may add narrow internal cancellation checkpoints to M4-03 composition, but it must not duplicate or bypass M4-03 publication authority.

### Lifecycle / shutdown admission closure

When shutdown begins:

- close new mutating admission before storage/provider shutdown;
- new `SaveAsync` requests reject deterministically;
- an admitted save that has not begun durable publication may cancel/settle according to the bounded policy;
- an operation already crossing durable publication must settle to known terminal state;
- storage/backend shutdown must not race ahead and invalidate an active durable publication;
- after shutdown completes, no new save may admit.

Do not turn EchoSave into project-wide lifetime/service composition.

### Public result mapping

Map the complete M4-03 transaction result into the public save result without losing:

- slot ID;
- source generation ID;
- published generation ID;
- failing participant/current-owner identity where appropriate;
- fresh/unknown counts where appropriate;
- total payload bytes where appropriate;
- generation-published flag;
- head-published flag;
- catalog-reconciled flag;
- reconciled catalog entry/summary where safe.

Public result mapping must not claim rollback after durable publication.

## 4. Explicit non-scope

Do not add:

- `RequestAutosave`;
- autosave coalescing;
- generic queued multi-operation scheduler;
- configurable queue capacity or overflow policy;
- coalesced catalog refresh;
- duplicate/delete/recovery admission wiring;
- permission-provider production facade wiring;
- save-operation progress/event system beyond minimal existing lifecycle plumbing;
- retention cleanup;
- recovery planning/execution;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- full single/fixed/configurable/unlimited-profile configuration assets;
- document migration;
- scene travel;
- peer bridges;
- service locator;
- Chronicle-owned/project-wide DDOL.

## 5. Failure and safety invariants

Tests must prove:

- public save before Ready rejects without participant callbacks;
- public save after shutdown begins rejects without participant callbacks;
- one admitted save blocks a second manual save with Busy;
- Busy request does not queue and does not later execute;
- admission releases after success;
- admission releases after preflight/capture/publication failure;
- pre-canceled request advances no head;
- cancellation before participant capture invokes no participant capture;
- cancellation at a safe pre-publication point advances no head;
- cancellation after durable publication begins reports Too Late and lets the transaction settle;
- Too-Late never fabricates rollback;
- shutdown closes new admission;
- shutdown does not abandon an already-committing publication;
- M4-03 success maps faithfully to public success;
- M4-03 partial durable/catalog failure maps faithfully;
- active-slot targeting and display-name preservation remain unchanged;
- no autosave/coalescing, retention/recovery, rename/duplicate/delete, cache, scene, bridge, or DDOL scope enters runtime;
- all prior **439 / 439** Chronicle tests remain green.

## 6. Proposed focused proof

- service-not-ready rejection;
- shutdown-admission rejection;
- first manual-save admission success;
- overlapping manual-save Busy;
- Busy non-queue proof;
- admission release after success;
- admission release after failure;
- already-canceled request;
- cancel before capture;
- cancel immediately before durable publication;
- Too-Late cancellation after publication boundary;
- shutdown while no operation active;
- shutdown while admitted pre-publication save exists;
- shutdown while durable publication is already settling;
- public success result mapping;
- public failure result mapping;
- public partial durable/catalog truth mapping;
- no participant callbacks for rejected/Busy/canceled-before-capture cases;
- prior **439 / 439** regression floor.

Executed totals are recorded from Unity, never predicted.

## 7. Stop point

Stop when one ready EchoSave root exposes a public active-slot `SaveAsync` that:

1. uses the proven M4-03 transaction;
2. admits only one root-local mutating operation;
3. returns Busy immediately for overlapping manual save;
4. honors safe pre-publication cancellation;
5. reports Too Late once durable publication begins;
6. closes new admission during shutdown;
7. preserves truthful durable/head/catalog outcomes;
8. completes publicly on the main thread.

Do **not** implement autosave yet.

Do **not** implement generic queued multi-operation scheduling, permission-provider facade wiring, retention, recovery, persistent cache, or other slot operations yet.


## 8. Completion Evidence

**Planning/activation commit:** `91dcb62`

**Implementation commit:** `2732aaa`

**Lifecycle-status hotfix:** `09ae8f1`

**Final effective runtime baseline:** `09ae8f1`

**Unity compile/import:** Green

**Focused Chronicle Editor gate:** **456 / 456 passed, 0 failed**

**Prior regression floor:** **439 / 439**

**Net new focused tests:** **17**

Observed completion:
- public active-slot `SaveAsync` is available through `IEchoSaveService`;
- one root-local mutating-operation admission authority rejects overlapping manual save as Busy without queueing;
- cancellation before durable publication remains safe;
- cancellation after durable publication begins reports Too Late without fictional rollback;
- shutdown closes new admission before backend shutdown while allowing an already-committing operation to settle;
- public results preserve M4-03 durable generation/head/catalog truth;
- pre-Ready lifecycle rejection is `ServiceNotReady`;
- shutdown/closed-admission rejection is `AdmissionClosed`;
- autosave/coalescing, generic queued multi-operation scheduling, retention, recovery, rename/duplicate/delete, persistent catalog cache, full slot-policy configuration, scene/bridge/DDOL scope remain absent.

### Implementation history

The first focused M4-04 run discovered **456** tests with **455 passed / 1 failed**. The lone failure was `SaveBeforeReadyRejectsWithoutTransactionExecution`: expected `ServiceNotReady`, observed `AdmissionClosed`.

The defect was lifecycle ordering, not test intent. `SaveOperationAdmissionCoordinator` intentionally begins closed before initialization, and the public service preflight incorrectly allowed that internal admission state to override the public pre-Ready lifecycle result.

Two patch helpers refused safely without changing the repository. The final bounded v3 hotfix replaced only `EchoSaveService.cs` after exact committed-file identity validation. The final rerun passed **456 / 456**.

No follow-on M4 checkpoint is activated by this closeout.
