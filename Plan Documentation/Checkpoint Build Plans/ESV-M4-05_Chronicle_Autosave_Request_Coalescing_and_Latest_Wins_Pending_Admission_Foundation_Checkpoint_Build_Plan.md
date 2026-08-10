---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-10
---
# ESV-M4-05 — Chronicle Autosave Request Coalescing and Latest-Wins Pending Admission Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-05
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.23.0
**Prior checkpoint:** ESV-M4-04 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **456 / 456**
**Exact implementation baseline:** `9a2ad29`

## 1. Intent

Add the first public autosave-request path while preserving the boundaries already proven by M4-03 and M4-04.

M4-05 answers one bounded question:

> When project code explicitly requests autosave, can Chronicle keep request pressure bounded to one latest pending request, reuse the same root-local admission and durable active-slot save transaction as manual save, and settle pending work deterministically without inventing a generic operation queue or gameplay-owned autosave timer?

```text
project/system decides "autosave now"
        ↓
RequestAutosave(request)
        ↓
service / request preflight
        ↓
root-local mutating admission state
        ├── idle
        │     ↓
        │  autosave admitted
        │     ↓
        │  M4-03/M4-04 durable save path
        │
        └── occupied
              ↓
        pending autosave slot
              ↓
        latest request replaces older pending request
              ↓
        active mutation settles
              ↓
        execute at most the current pending autosave
```

This checkpoint bounds autosave **submission and coalescing**. It is not retention, recovery, a generic queue, or an automatic gameplay timer.

## 2. Carried-forward authority

Chronicle already proves:

- one root-local mutating-operation admission authority;
- manual save returns Busy immediately rather than queueing;
- active-slot public `SaveAsync`;
- M4-03 durable participant capture, unknown-payload carry-forward, immutable generation publication, `head.json` last, and catalog reconciliation;
- bounded pre-publication cancellation and Too-Late truth;
- shutdown admission closure;
- main-thread public completion;
- focused Chronicle Editor **456 / 456**.

The package specification already requires:
- `RequestAutosave(AutosaveRequest)` as the public coalescible autosave surface;
- autosaves to coalesce into **at most one pending latest request**;
- manual save to remain Busy rather than form an unbounded queue;
- one mutating operation per EchoSave root;
- project/game authority to decide when autosave should be requested.

### ESV-D-027 — autosave is caller-triggered and latest-wins bounded

> Chronicle does not decide when gameplay should autosave. A project/system explicitly submits an autosave request. Chronicle bounds request pressure by retaining at most one pending latest autosave while another mutating operation owns admission. The pending request is not a general-purpose queue.

## 3. Authorized implementation scope

### Public autosave request/submission surface

Add the narrow public types required for explicit autosave submission.

The implementation should provide:
- `AutosaveRequest`;
- a bounded submission/ticket/result type representing accepted, coalesced, superseded/rejected, and shutdown/closed outcomes as needed;
- additive `IEchoSaveService.RequestAutosave(AutosaveRequest)`.

Rules:
- request is active-slot only;
- request contains no filesystem path;
- request does not rename the slot;
- request metadata uses existing save metadata bounds;
- caller invocation is the autosave trigger; Chronicle adds no timer, checkpoint watcher, combat rule, or scene rule;
- invalid/not-ready/no-active-slot conditions reject without creating pending work.

### At-most-one pending latest autosave

Chronicle may retain **zero or one** pending autosave request.

Rules:
- first autosave submitted while another mutation is active becomes pending if otherwise valid;
- a newer autosave while one is pending replaces/coalesces the pending request;
- only the newest pending request metadata survives;
- pending count never exceeds one;
- there is no list/queue collection of autosaves;
- submission result makes coalescing observable without exposing mutable internal queue state.

### Idle autosave admission

If the service is Ready and root-local admission is available:
- autosave may acquire the same admission authority used by manual save;
- autosave executes through the same durable active-slot save transaction;
- no second save engine is permitted;
- ordinary slot display name/path/ID semantics remain unchanged;
- autosave preserves public generation/head/catalog truth.

### Pending autosave drain

When the active mutating operation releases:
- Chronicle checks for one current pending autosave;
- it may admit and execute that request only if service lifecycle, active slot, and request preconditions still permit;
- it executes at most once;
- if execution cannot start, terminal pending truth is recorded and the pending slot is cleared;
- a newer request arriving before drain owns the pending slot instead of the older request.

Implementation may add one narrow release/availability notification to the M4-04 admission authority. It must not introduce a generic multi-operation scheduler.

### Manual-save interaction

M4-04 manual-save semantics remain authoritative:
- overlapping manual save returns Busy;
- manual save is never silently queued behind autosave;
- autosave coalescing does not change manual cancellation or Too-Late semantics;
- one mutating operation remains authoritative per root.

### Shutdown

When shutdown begins:
- new autosave requests reject;
- pending autosave must not start after admission closes;
- pending request state is cleared/settled deterministically;
- an autosave already crossing durable publication follows M4-04 Too-Late/settlement semantics;
- shutdown never invents rollback for a committed generation.

### Diagnostics/result truth

Add stable bounded diagnostic/result truth for:
- autosave accepted for immediate/admitted execution;
- autosave retained pending;
- autosave coalesced/replaced by a newer latest request;
- autosave rejected because service/request/selection is invalid;
- autosave rejected/cleared because admission/lifecycle closed;
- autosave terminal save result where the public surface exposes it.

Do not add per-frame polling or unbounded operation history.

## 4. Explicit non-scope

Do not add:
- automatic time-based autosave timers;
- gameplay checkpoint triggers;
- save-permission/game-state policy wiring;
- `SaveRetentionPolicy` or generation cleanup;
- autosave-history generation bounds;
- generic queued multi-operation scheduler;
- general queue capacity/overflow configuration;
- duplicate/delete/recovery scheduling;
- repeated catalog-refresh coalescing;
- retention;
- recovery;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- full slot-policy/configuration expansion;
- document migration;
- scene travel;
- peer bridges;
- service locator;
- Chronicle-owned/project-wide DDOL.

## 5. Failure and safety invariants

Tests must prove:
- autosave before Ready rejects and creates no pending request;
- invalid autosave request rejects and creates no pending request;
- autosave with no valid active slot rejects;
- idle autosave uses the existing root-local admission authority;
- idle autosave executes the existing durable save transaction;
- one occupied root can retain exactly one pending autosave;
- a second pending autosave coalesces/replaces the first rather than adding another entry;
- latest metadata is the metadata that eventually executes;
- repeated autosave spam keeps pending count bounded to one;
- manual save while occupied still returns Busy and does not queue;
- pending autosave runs at most once after active admission releases;
- failed pending preflight clears pending state safely;
- shutdown clears/rejects pending autosave and does not start it after closure;
- an already-committing autosave settles under M4-04 Too-Late truth;
- no second admission lock or durable save engine appears;
- no timers, retention, recovery, rename/duplicate/delete, persistent cache, scene, bridge, or DDOL scope enters runtime;
- all prior **456 / 456** Chronicle tests remain green.

## 6. Proposed focused proof

- public API exposure;
- request bounds;
- not-Ready rejection;
- no-active-slot rejection;
- immediate idle acceptance/execution;
- occupied admission creates one pending autosave;
- latest-wins coalescing;
- repeated spam remains one pending;
- manual-save Busy contract unchanged;
- pending drain after release;
- pending executes once;
- pending preflight failure clears safely;
- shutdown rejects new autosave;
- shutdown discards pending autosave;
- committing autosave settlement;
- successful autosave maps durable generation/head/catalog truth;
- deferred-scope audits;
- prior **456 / 456** regression floor.

Executed totals are recorded from Unity, never predicted.

## 7. Stop point

Stop when project code can explicitly request autosave and Chronicle:

1. accepts or rejects the request deterministically;
2. uses the existing active-slot save transaction;
3. uses the existing root-local mutating admission authority;
4. retains no more than one pending latest autosave;
5. coalesces newer pending requests without growing a queue;
6. preserves manual-save Busy semantics;
7. drains at most the latest pending autosave after admission becomes available;
8. clears/rejects pending work safely during shutdown.

Do **not** implement retention yet.

Do **not** add a generic operation queue or automatic gameplay autosave trigger.


## 8. Completion Evidence

**Planning baseline:** `9a2ad29`

**Planning/activation commit:** `8504ed4`

**Implementation commit:** `9917f1b`

**Final effective runtime baseline:** `9917f1b`

**Unity compile/import:** Green

**Focused Chronicle Editor gate:** **473 / 473 passed, 0 failed**

**Prior focused regression floor:** **456 / 456**

**Net new focused tests:** **17**

**Committed implementation/test scope:** **22 files**

Observed completion:
- public explicit caller-triggered `RequestAutosave(...)` is available;
- autosave retains at most one latest pending request;
- newer pending requests supersede older pending metadata instead of growing a queue;
- autosave reuses the M4-04 root-local admission authority;
- autosave reuses the M4-03/M4-04 durable active-slot save transaction;
- manual-save Busy semantics remain unchanged;
- pending autosave drains at most once after admission becomes available;
- shutdown rejects new autosave submission and prevents pending work from starting after closure;
- no automatic gameplay timer, generic operation queue, retention, recovery, rename/duplicate/delete, persistent cache, scene, bridge, or DDOL scope entered runtime.

### Regression maintenance

The first M4-05 focused run exposed one stale M4-04 public-surface regression assertion. M4-04 correctly required `RequestAutosave` to be absent while autosave was deferred. ESV-M4-05 explicitly authorizes that public API, so the test was updated to prove both bounded manual `SaveAsync` and bounded `RequestAutosave(AutosaveRequest) -> AutosaveSubmissionResult`.

Final rerun: **473 / 473 passed, 0 failed**.

### Helper hardening

The implementation apply workflow exposed two helper defects before the final green run:
- missing creation of a new nested destination directory;
- counting `git status --porcelain` rows instead of actual files.

The final helper uses parent-directory creation, actual tracked/untracked file counting, and verified rollback state. These are workflow/tooling corrections, not Chronicle runtime architecture changes.

No follow-on M4 checkpoint is activated by this closeout.
