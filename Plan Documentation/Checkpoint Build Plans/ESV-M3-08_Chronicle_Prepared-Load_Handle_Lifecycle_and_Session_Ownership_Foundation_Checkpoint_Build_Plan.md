---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-09
---

# ESV-M3-08 — Chronicle Prepared-Load Handle Lifecycle and Session Ownership Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-08
**Milestone:** M3 — Participants and Loading
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.14.0
**Prior checkpoint:** ESV-M3-07 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **294 / 294**

## 1. Intent

Wrap one completely validated, classified, migrated, and prepared in-memory load in the public disposable handle promised by the Chronicle specification **without applying participant gameplay state yet**.

This checkpoint turns the existing read + migration + preparation proof into a bounded caller/service-owned lifetime object.

```text
validated current generation
        ↓
known participant preparation
        ↓
supported old schemas migrate in memory
        ↓
prepared detached participant batch
        +
opaque unknown-payload source snapshot
        +
exact source slot/generation identity
        ↓
PreparedSaveLoad
        ↓
caller may retain across external scene coordination
        ↓
apply remains LOCKED
```

The handle is a lifetime/ownership object, not a save operation and not a scene-flow authority.

## 2. Authorized implementation scope

### Public disposable prepared-load handle

Add public sealed/disposable `PreparedSaveLoad`.

The public surface may expose safe immutable metadata such as:
- source `SaveSlotId`;
- source `SaveGenerationId`;
- prepared known-participant count;
- opaque unknown-payload count;
- original source transport-byte estimate;
- creation time / expiry time or equivalent lifetime metadata;
- validity state (`IsValid`, disposed/expired reason, or structured equivalent).

The public surface must **not** expose mutable detached participant DTO objects, raw serialized unknown payload bodies, or package-internal migration machinery.

Package-internal code may retrieve the prepared participant batch only after proving the handle is currently owned and valid.

### Package/session ownership

Add one runtime-memory-only prepared-load owner/store, for example `SavePreparedLoadStore`.

Rules:
- a handle belongs to exactly one owner/store instance;
- ownership is represented by opaque runtime token/epoch state, never durable save identity;
- a handle from another owner/store rejects;
- stale token/release operations cannot affect a replacement handle;
- no static global service locator;
- no central project-wide lifetime registry;
- no hardcoded scene authority;
- no `DontDestroyOnLoad` behavior is added in this checkpoint.

The store is the package-internal lifetime authority that a later `EchoSaveService`/`EchoSaveRoot` may own.

### Source-provenance binding

Creation succeeds only when every contributing in-memory artifact refers to the **same exact** source slot and generation.

At minimum validate:
- successful current-generation read result;
- successful prepared participant batch;
- opaque unknown-payload snapshot provenance when unknown entries exist;
- slot/generation identity equality across those artifacts.

Any mismatch fails closed and exposes no handle.

The handle preserves this exact source identity for later apply-time validation.

### Opaque unknown-payload binding

A prepared handle retains a defensive opaque snapshot of unknown payload state associated with its exact source slot/generation.

Rules:
- unknown payload contents remain package-internal;
- the handle does not interpret or deserialize them;
- handle creation/disposal does not publish/save/rewrite them;
- creating one handle does not silently mutate another handle's snapshot;
- later apply/session adoption may use this snapshot, but that behavior is **not** authorized yet.

### Prepared-state encapsulation

The prepared participant batch remains detached and in memory.

Rules:
- no participant `Capture`;
- no participant `Apply`;
- no public mutation of prepared entries;
- handle creation performs no serializer, migration, or participant callback work if the supplied preparation already succeeded;
- disposal/expiry releases package references to detached prepared objects so they can be collected;
- disposing a handle does not alter disk.

### Expiry and disposal

Implement explicit lifecycle states sufficient to prove:
- live;
- disposed;
- expired;
- session-invalidated/owner-invalidated.

Requirements:
- `Dispose()` is idempotent;
- disposed handle cannot later become valid;
- expired handle cannot later become valid;
- owner/session invalidation invalidates all owned live handles;
- invalidation clears package references to prepared participant DTOs and opaque snapshots;
- validity checks are deterministic and testable through an injected clock/time seam;
- no Unity frame/update loop is required;
- expiry may be evaluated lazily on access/admission plus explicit store sweep.

No participant state is automatically applied on expiry/disposal.

### Bounded live-handle admission

Prepared loads are in-memory resource objects, so M3-08 must include explicit bounds.

Authorize:
- positive maximum live-handle count;
- positive maximum aggregate prepared-load source transport-byte estimate;
- fail-closed creation when either bound would be exceeded;
- release of owned capacity on dispose/expiry/invalidation;
- no unbounded prepared-load accumulation.

Transport-byte estimate may be derived from fully validated known participant entry byte lengths plus opaque unknown snapshot byte totals. It is a safety estimate, not a promise of exact managed-heap size.

### Handle creation result

Add structured creation/admission result/status with stable diagnostics for:
- invalid request;
- mismatched source provenance;
- unknown snapshot provenance mismatch;
- count limit exceeded;
- byte estimate limit exceeded;
- owner/session unavailable;
- success.

No exceptions should be required for normal invalid handle-creation requests.

## 3. Explicitly out of scope

Do not implement:
- `ApplyPreparedLoadAsync`;
- participant `Apply`;
- missing-payload default execution;
- participant apply rollback/compensation;
- convenience `LoadAndApplyAsync`;
- scene travel or Passage integration;
- automatic scene selection;
- document migration registry/execution;
- recovery candidate selection;
- source-generation rewrite or automatic migration recommit;
- production `PrepareLoadAsync` operation admission/cancellation;
- production `SaveAsync`;
- operation busy/coalescing rules;
- slot catalog/policy;
- active-slot selection;
- retention cleanup;
- autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL/service locator.

## 4. Failure and lifecycle invariants

M3-08 tests must prove:
- valid matching read/prepared/unknown artifacts create one live handle;
- source slot mismatch rejects;
- source generation mismatch rejects;
- unknown snapshot source mismatch rejects;
- unknown entries without valid source provenance reject;
- handle public metadata is immutable/read-only;
- prepared DTO objects are not exposed publicly;
- raw unknown payload bodies are not exposed publicly;
- handle from owner A rejects owner B internal access;
- stale ownership token cannot release/revalidate replacement state;
- `Dispose()` is idempotent;
- disposal invalidates internal prepared-batch access;
- disposal releases live-handle count and byte budget;
- disposal performs zero storage mutation;
- expiry invalidates internal prepared-batch access;
- expired handle releases live-handle count and byte budget;
- session/owner invalidation invalidates every live owned handle;
- invalidation cannot resurrect after a later store/session reset;
- count limit rejects without evicting a valid existing handle;
- byte-estimate limit rejects without evicting a valid existing handle;
- failed admission exposes no partially-owned handle;
- unknown snapshot is defensively isolated per handle;
- participant `Capture` invocation count remains zero;
- participant `Apply` invocation count remains zero;
- no serializer/migration callbacks run during handle wrapping;
- no Unity scene/lifetime API is required;
- all prior **294 / 294** Chronicle tests remain green.

## 5. Proposed focused proof

- prepared-load public metadata and opacity;
- matching source artifacts create live handle;
- slot mismatch rejection;
- generation mismatch rejection;
- unknown-provenance mismatch rejection;
- owner A vs owner B rejection;
- idempotent disposal;
- expired-handle rejection;
- deterministic injected-time expiry;
- invalidate-all/session-epoch behavior;
- stale token safety;
- count-cap admission/release;
- source-byte-budget admission/release;
- failed admission has no capacity leak;
- defensive unknown snapshot ownership;
- prepared batch internal access only while live;
- zero `Capture`;
- zero `Apply`;
- zero storage mutation;
- zero scene/DDOL APIs;
- prior **294 / 294** regression floor remains green.

Executed totals are recorded from Unity, not predicted.

## 6. Stop point

Stop when Chronicle can encapsulate one exact-source, fully validated, migrated, prepared in-memory load in a bounded public disposable handle that remains valid until apply/dispose/expiry/session invalidation and can safely survive caller-controlled time between prepare and a future apply.

Do not apply participant state yet.

Do not add scene travel.

Do not add document migration.

The next bounded checkpoint should activate deterministic prepared-participant apply validation and execution, including missing-payload policy and structured apply reporting, while preserving the prepared-handle ownership boundary.
