---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active
updated: 2026-08-10
---
# ESV-M4-03 — Chronicle Manual Save Transaction Composition, Unknown Carry-Forward, and Catalog Reconciliation Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-03
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.19.0
**Prior checkpoint:** ESV-M4-02 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **425 / 425**
**Exact implementation baseline:** `a3eba25`

## 1. Intent

Compose the first complete **manual save transaction** from Chronicle primitives that are already independently proven, without prematurely turning the lifecycle-only service facade into the final production `SaveAsync`/queue/cancellation authority.

M4-03 answers one narrow question:

> Given one explicitly selected healthy slot, can Chronicle capture current known participant state, preserve valid opaque unknown payloads from the exact current generation, publish one new immutable generation only against that still-current source, and reconcile the slot catalog without lying about partial durable success?

```text
explicit healthy active slot
        ↓
current-generation validation
        ↓
source slot/generation provenance
        ↓
fresh known participant capture
        ↓
opaque unknown snapshot
        ↓
collision-safe known + unknown merge
        ↓
expected-current-generation publication
        ↓
candidate verification
        ↓
immutable generation publication
        ↓
published-generation revalidation
        ↓
head.json LAST
        ↓
catalog reconciliation
        ↓
truthful manual-save transaction result
```

This is deliberately **transaction composition**, not yet the production async service facade.

## 2. Carried-forward authority

Chronicle already proves:

- M4-01 provider-neutral payload-free catalog reconstruction;
- M4-01 healthy/degraded immutable catalog snapshots;
- M4-01 explicit session-only active-slot selection;
- M4-02 real technical slot creation as a committed initial empty generation;
- M3-02 detached deterministic participant capture;
- M3-04 validated current-generation reading and session unknown-payload preservation;
- M3-05 exact source provenance, collision-safe fresh+unknown merge, and expected-current-generation stale-source rejection;
- M2/M3 candidate verification, immutable publication, final revalidation, and `head.json` last;
- publication result truth distinguishes generation publication from head publication;
- ordinary save must not reinterpret unknown payloads.

### ESV-D-025 — compose manual-save truth before public operation admission

> The first manual-save checkpoint composes existing proven primitives internally before exposing production `SaveAsync` or generic operation admission.

This keeps lifecycle/service wiring, queue policy, Busy behavior, cancellation, autosave coalescing, and cross-operation concurrency out of the same checkpoint that first proves the save transaction itself.

## 3. Authorized implementation scope

### Manual save transaction request/result

Add one narrow internal technical request/result/status family sufficient to exercise the complete transaction.

The request may carry bounded current-build metadata such as:

- project ID;
- project version;
- build ID.

Rules:

- target slot is the already-selected M4-01 active slot;
- caller does not supply an arbitrary storage path;
- caller does not rename the slot through ordinary save;
- current display name is preserved from the trusted healthy catalog entry;
- request bounds are validated before participant callbacks.

The result must distinguish at least:

- invalid request / unavailable dependencies;
- no active slot;
- active slot not healthy/currently selectable;
- current-generation read/provenance failure;
- participant capture failure;
- unknown-payload merge/provenance/collision failure;
- stale expected-current-generation rejection;
- publication failure before head;
- durable generation published but head not published;
- head published / catalog reconciliation failed;
- full success.

Exact names are implementation details; durable truth is not.

### Active-slot and catalog preflight

Before participant capture:

- require a selected active slot;
- require the active ID to resolve to one healthy entry in the current catalog snapshot;
- snapshot the slot ID, current generation ID, display name, and lightweight current metadata needed by the transaction;
- fail before participant callbacks if the selected slot is missing/degraded.

M4-03 does not auto-select or repair selection.

### Current-generation provenance refresh

For the selected slot:

- read the current head and complete current generation through the existing M3-04 reader;
- validate package documents and payload agreement before replacing session unknown-payload state;
- require source provenance to identify the exact selected slot and current generation;
- do not recover a corrupt current generation in this checkpoint;
- do not silently fall back to another generation.

### Fresh known participant capture

Reuse `SaveParticipantCaptureCoordinator`.

Rules:

- capture occurs only after active-slot/current-generation preflight succeeds;
- participant ordering/identity/version/serializer rules remain unchanged;
- one participant failure stops the transaction before storage mutation;
- no participant apply/default callbacks occur.

### Opaque unknown-payload carry-forward

Reuse `SaveUnknownPayloadCarryForwardMerger`.

Rules:

- unknown entries remain opaque;
- unknown snapshot source slot/generation must match the validated source;
- a currently registered participant claiming an unknown ID remains an ownership collision and blocks;
- fresh known entries win only through explicit canonical ownership, never by silently deleting conflicting unknown data;
- empty unknown snapshots are valid when provenance is valid.

### Expected-current-generation publication

Reuse `PublishMergedParticipantTransportGeneration`.

The publication must:

- carry the merged known+unknown transport batch;
- require the previously validated source generation to still be current;
- reject stale source if the head changed after provenance was read;
- preserve candidate write/read-back verification;
- publish immutable generation before head;
- revalidate the published generation;
- publish `head.json` last;
- never delete the prior current generation on failure.

### Metadata behavior

Ordinary save is not rename.

Therefore:

- preserve current `displayName` from the trusted catalog entry;
- allow bounded project/build metadata from the save transaction request;
- keep slot ID and physical path unchanged;
- do not add rename policy or display-name uniqueness policy.

### Catalog reconciliation

After successful head publication:

- refresh the M4-01 catalog;
- require the new current generation to appear as the healthy current entry on reconciliation success;
- preserve the existing active selection when the same healthy slot remains present;
- if refresh fails after head publication, report **head published / catalog reconciliation failed**;
- never roll back or delete a valid committed generation merely because the derived catalog failed to refresh.

## 4. Explicit non-scope

Do not add:

- public `SaveAsync`;
- `IEchoSaveService` save-operation expansion;
- generic production `SaveOperationCoordinator`;
- Busy/reject queue policy;
- queue capacity;
- cancellation or Too-Late cancellation behavior;
- shutdown settlement of active operations;
- permission-provider production facade wiring;
- autosave request/coalescing;
- retention cleanup;
- recovery/fallback generation selection;
- persistent `catalog.cache.json`;
- rename;
- duplicate;
- delete/deletion plans;
- trash/quarantine;
- full single/fixed/configurable/unlimited-profile configuration assets;
- document migration;
- scene travel;
- peer bridges;
- Chronicle-owned/project-wide DDOL/service locator.

## 5. Failure and safety invariants

Tests must prove:

- no active slot fails before participant capture;
- selected degraded/missing slot fails before participant capture;
- invalid request metadata fails before participant capture;
- current head/current generation must validate before capture;
- current-generation read failure does not mutate unknown session state with untrusted data;
- fresh participant capture failure publishes nothing;
- unknown-payload provenance mismatch publishes nothing;
- unknown ownership collision publishes nothing;
- valid empty unknown snapshot can merge with fresh captures;
- valid unknown opaque payload survives byte-for-byte through save;
- expected-current-generation stale rejection publishes no new head;
- candidate/publication/final-verification/head failures preserve truthful generation/head flags;
- current display name is preserved by ordinary save;
- successful save advances head to exactly one new participant-backed generation;
- successful save keeps prior current generation as previous-generation history;
- successful catalog reconciliation exposes the new generation and keeps the active slot selected;
- catalog reconciliation failure after head success does not delete the committed generation;
- zero participant apply/default callbacks occur;
- public `SaveAsync`, Busy admission, autosave, retention, recovery, rename/duplicate/delete, scene, bridge, and DDOL scope remain absent;
- all prior **425 / 425** Chronicle tests remain green.

## 6. Proposed focused proof

- request/provider validation;
- no-active-slot rejection;
- degraded/missing-active-slot rejection;
- source current-generation read failure;
- exact source-provenance binding;
- successful fresh known capture;
- participant capture failure;
- empty unknown carry-forward;
- opaque unknown byte-for-byte carry-forward;
- unknown ownership collision;
- unknown provenance mismatch;
- stale source generation at publication boundary;
- candidate write failure;
- candidate verification failure;
- immutable generation publication failure;
- final published-generation verification failure;
- head publication failure;
- display-name preservation;
- successful participant-backed save;
- previous-generation pointer preservation;
- catalog reconciliation success;
- active-slot preservation after reconciliation;
- catalog reconciliation failure after durable head success;
- zero participant apply/default callbacks;
- prior **425 / 425** regression floor.

Executed totals are recorded from Unity, never predicted.

## 7. Stop point

Stop when Chronicle can take one already-selected healthy slot and perform one deterministic manual-save transaction that:

1. validates the exact current source generation;
2. captures fresh known participants;
3. preserves valid opaque unknown payloads;
4. rejects stale source before head replacement;
5. publishes one verified immutable participant-backed generation with `head.json` last;
6. preserves ordinary display metadata;
7. reconciles the catalog;
8. reports durable/head/catalog truth accurately.

Do **not** expose production `SaveAsync` yet.

Do **not** add generic operation admission, Busy/cancellation, autosave, retention, recovery, persistent cache, or other slot operations yet.
