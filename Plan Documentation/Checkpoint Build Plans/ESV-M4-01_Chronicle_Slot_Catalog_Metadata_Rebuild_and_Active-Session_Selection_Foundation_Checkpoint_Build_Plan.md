---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-10
---

# ESV-M4-01 — Chronicle Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-01
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.16.0
**Prior checkpoint:** ESV-M3-09 — **Complete**
**Prior milestone:** M3 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **366 / 366**

## 1. Intent

Establish Chronicle's first real slot-catalog/session layer without coupling the catalog to participant payload interpretation, local-filesystem internals, destructive slot operations, autosave, retention, or recovery execution.

M4-01 proves that Chronicle can discover existing technical slot roots through a provider-neutral capability, reconstruct lightweight metadata from authoritative `head.json` + current `manifest.json` documents, publish one deterministic immutable in-memory catalog snapshot, and maintain session-only active-slot selection.

```text
storage backend
      ↓
provider-neutral slot discovery
      ↓
valid technical SaveSlotId roots
      ↓
read head.json
      ↓
read current manifest.json
      ↓
NO payload.json read
      ↓
slot metadata / health classification
      ↓
atomic deterministic catalog snapshot
      ↓
optional session active-slot selection
```

The catalog is **derived state**. It is never durable gameplay truth and never becomes sole authority for a slot.

## 2. Carried-forward authority

The Chronicle specification already establishes:
- `SaveSlotId` is package-generated technical identity and is never a display name;
- slot metadata must be readable without applying full participant payloads;
- catalog state is derived/rebuildable;
- authoritative slot truth comes from slot heads and immutable generation manifests;
- active slot selection is session state and is not persisted by Chronicle by default;
- display names do not define physical paths;
- recovery, retention, autosave, destructive slot operations, and production save/load admission are separate concerns.

### ESV-D-023 — provider-neutral catalog discovery

M4-01 records one bounded architecture decision:

> Slot discovery must not make Chronicle core reach through `LocalFileSaveStorageBackend.RootPath` or call `System.IO` directly. The existing base `ISaveStorageBackend` byte-object contract remains unchanged. Catalog-capable providers expose slot-root enumeration through an additive optional storage capability; the default local backend implements it.

Consequences:
- existing backends/test doubles remain source-compatible;
- a backend that does not expose the optional discovery capability cannot perform M4 catalog refresh and returns a structured unsupported/unavailable result;
- catalog logic consumes provider-neutral discovered child names/keys only;
- no local-filesystem path leaks into public catalog models.

## 3. Authorized implementation scope

### Additive storage discovery capability

Add a narrow optional storage capability for bounded child discovery beneath a validated relative storage key.

Requirements:
- base `ISaveStorageBackend` remains unchanged;
- the default local backend implements the capability;
- discovery is read-only;
- returned names are immediate children only;
- results are deterministic after Chronicle sorts them;
- unsafe names/path traversal never escape the configured root;
- provider exceptions become structured storage/catalog failures;
- catalog code does not use `System.IO` directly.

### Slot-root discovery

Refresh begins at the technical `slots` root.

Rules:
- absent `slots` root means a valid empty catalog;
- only immediate child names that parse as canonical `SaveSlotId` may become catalog slot candidates;
- invalid/unrecognized child names never become selectable slots;
- discovery is bounded by a positive scan-entry limit;
- exceeding the bound fails refresh before snapshot replacement;
- deterministic order is canonical `SaveSlotId` ordinal order.

### Lightweight authoritative metadata reconstruction

For each valid discovered slot ID:
1. read `slots/<slot-id>/head.json`;
2. validate package document kind/version and technical slot identity;
3. resolve the current generation ID;
4. read only that generation's `manifest.json`;
5. validate manifest kind/version, slot/generation identity, current commit state, and bounded metadata fields;
6. construct a lightweight immutable slot metadata entry.

M4-01 **must not read `payload.json`** merely to build or refresh the catalog.

Metadata may expose only safe lightweight fields already carried by the manifest/technical documents, such as:
- technical `SaveSlotId`;
- current `SaveGenerationId` when trustworthy;
- creation/update UTC values when valid;
- display name as display metadata only;
- save kind;
- project/build/version metadata;
- current generation participant-count/transport-size summaries only where available without payload reads;
- health/selectability state and stable diagnostics.

No participant payload body, unknown payload body, detached DTO, serializer object, or physical root path enters public catalog metadata.

### Slot health classification

A valid technical slot directory should remain discoverable even when its current head or manifest is unhealthy enough that it cannot be selected normally.

At minimum distinguish enough structured state to represent:
- healthy/selectable;
- missing head;
- invalid/unsupported head;
- missing current manifest;
- invalid/unsupported current manifest;
- head/manifest identity disagreement;
- backend read failure.

Per-slot corruption should produce degraded metadata rather than silently deleting the slot from the catalog.

A failure of the overall enumeration transport, an exceeded catalog bound, or another condition that prevents a trustworthy complete scan fails the refresh and preserves the prior in-memory snapshot.

### Deterministic immutable catalog snapshot

Add package-owned runtime catalog state that:
- exposes immutable/defensive snapshots;
- sorts entries deterministically by canonical technical slot ID;
- records total and health counts without unbounded history;
- replaces the live snapshot atomically only after a complete trustworthy scan/classification pass;
- preserves the prior snapshot on overall refresh failure;
- begins with a valid empty snapshot;
- performs no participant callbacks.

The in-memory catalog is derived runtime state. M4-01 proves **authoritative rebuild from heads/manifests first**.

Persistent `catalog.cache.json` read/write optimization remains deferred. When introduced later it must remain disposable/rebuildable and never supersede authoritative head/manifest reconstruction.

### Active-session slot selection

Add session-only active-slot state independent from the immutable catalog snapshot.

Rules:
- no slot is selected by default;
- selection accepts only a currently catalog-known, selectable slot;
- selecting an unhealthy/unknown slot fails without changing the prior selection;
- selecting the already-active slot is deterministic `NoChange` behavior;
- explicit clear/unselect is supported;
- successful catalog replacement reconciles the active selection;
- if the active slot disappears or becomes non-selectable after refresh, Chronicle clears the session selection rather than retaining a stale technical ID;
- refresh never auto-selects a slot;
- active selection is not written to disk in this checkpoint.

### Result and diagnostic contracts

Add stable structured results/statuses sufficient for:
- discovery capability unavailable;
- empty catalog success;
- successful healthy/degraded rebuild;
- overall refresh failure with prior-snapshot preservation;
- active-slot selected/no-change/cleared/rejected;
- per-slot lightweight health diagnostics.

Public diagnostics must remain payload-free and path-redacted.

## 4. Explicitly out of scope

Do not implement:
- physical slot creation;
- slot rename;
- slot duplication;
- deletion plans/confirmation/trash;
- persistent `catalog.cache.json` optimization;
- slot templates or full configurable slot-policy asset expansion;
- production `SaveAsync` operation admission/coalescing/cancellation;
- production `PrepareLoadAsync` / `ApplyPreparedLoadAsync` service orchestration;
- convenience `LoadAndApplyAsync`;
- autosave;
- retention cleanup;
- recovery candidate selection or recovery publication;
- document migration;
- scene travel / Passage integration;
- peer bridges;
- Chronicle-owned/project-wide DDOL/service locator.

## 5. Failure and safety invariants

M4-01 tests must prove:
- base `ISaveStorageBackend` remains unchanged;
- local backend exposes only the additive discovery capability needed by catalog refresh;
- discovery cannot escape the configured storage root;
- missing `slots` root yields a successful empty catalog;
- invalid technical child name does not become a slot;
- deterministic child ordering is independent of provider enumeration order;
- scan-entry bound is enforced before live snapshot replacement;
- healthy head + manifest produces healthy selectable metadata;
- catalog refresh reads head + current manifest and never reads payload;
- display name never becomes a storage key or technical ID;
- missing/corrupt/unsupported head produces degraded non-selectable metadata;
- missing/corrupt/unsupported manifest produces degraded non-selectable metadata;
- head/manifest identity mismatch produces degraded non-selectable metadata;
- one unhealthy slot does not erase unrelated healthy slots;
- overall enumeration/read transport failure preserves the previous complete snapshot when the scan cannot be trusted;
- public snapshots are immutable/defensive;
- active slot begins unset;
- healthy known slot selects successfully;
- unknown/unhealthy slot selection rejects without changing prior selection;
- selecting the same slot returns no-change;
- explicit clear removes session selection;
- refresh removal/unhealthiness clears stale active selection;
- refresh never auto-selects;
- active selection performs zero durable writes;
- catalog refresh invokes zero participant capture/apply/default callbacks;
- no payload body is exposed in public metadata/diagnostics;
- no scene/DDOL authority is introduced;
- all prior **366 / 366** Chronicle tests remain green.

## 6. Proposed focused proof

- additive provider discovery capability;
- unchanged base storage interface;
- local discovery root/path safety;
- empty `slots` root;
- invalid child technical ID;
- deterministic enumeration ordering;
- discovery bound;
- healthy head/manifest metadata reconstruction;
- payload-read spy proves zero payload access;
- display-name/path separation;
- missing/invalid head health;
- missing/invalid manifest health;
- identity disagreement health;
- mixed healthy/degraded catalog;
- failed-refresh prior-snapshot preservation;
- immutable snapshot defensive copy;
- active-slot select/no-change/reject/clear;
- active-slot invalidation after catalog replacement;
- zero durable writes for session selection;
- zero participant callbacks;
- zero scene/DDOL authority;
- prior **366 / 366** regression floor remains green.

Executed totals are recorded from Unity, not predicted.

## 7. Stop point

Stop when Chronicle can reconstruct a trustworthy deterministic payload-free slot catalog from provider-neutral discovery plus authoritative head/current-manifest documents, represent unhealthy slots without pretending they are selectable, preserve the last complete snapshot across untrustworthy refresh failure, and maintain a safe session-only active-slot selection.

Do not create, rename, duplicate, or delete slots yet.

Do not add autosave, retention, or recovery yet.

Do not write a persistent catalog cache yet.

The next bounded M4 checkpoint should build physical slot creation and slot-policy/capacity behavior on top of this proven catalog/session foundation, or activate production operation admission if repository evidence shows that service orchestration must precede slot mutation.
