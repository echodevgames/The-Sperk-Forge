---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/reconciliation
status: complete
updated: 2026-08-11
---
# ESV-M4-R1 — Chronicle Public Runtime Composition and Consumer Facade Reconciliation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-R1
**Milestone:** M4 — Slots / Autosave / Recovery Reconciliation
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.36.0
**Decision:** ESV-D-033
**Planning baseline:** `48454ea`
**Planning/activation commit:** `bdb0c00`
**Implementation commit:** `ab18361`
**Unity baseline:** 6000.3.8f1
**Prior focused regression floor:** **587 / 587**
**Final focused Chronicle Editor gate:** **618 / 618**, `0` failed
**Net new focused tests:** **31**
**Committed implementation/test scope:** **29 files**, `2995` insertions, `18` deletions

## Outcome

R1 closes the two public-composition audit gaps A-01 and A-02 without rewriting Chronicle's established persistence machinery.

Chronicle's primary consumer service now exposes:

```text
RegisterParticipant(...)
GetCatalogSnapshot()
RefreshCatalogAsync()

CreateSlotAsync(...)
SelectSlot(...)

PrepareLoadAsync(...)
ApplyPreparedLoadAsync(...)
LoadAndApplyAsync(...)
```

Existing public save/autosave/recovery/rename/duplicate/delete operations remain intact.

## Composition truth

R1 reuses existing authorities:
- `SaveParticipantRegistry` for registration ownership and collision truth;
- `SaveSlotCatalog` for immutable snapshot/refresh and active-selection reconciliation;
- M4-02 technical slot creation for durable empty-slot publication;
- current-generation reading and validation for load preparation;
- participant preparation and participant migration machinery;
- prepared-load bounded lifetime/store truth;
- M3-09 apply planning/execution.

No parallel participant registry, slot catalog, slot publication path, or load-apply engine was introduced.

## Catalog/create/select truth

- `GetCatalogSnapshot()` is memory-only.
- `RefreshCatalogAsync()` reuses payload-free catalog reconstruction.
- Public create adapts the internal technical creation coordinator.
- Public create does not auto-select.
- Selection remains session-only and performs no durable write.
- R1 retains the technical capacity value `64`.
- R2 owns replacing that hard runtime default with project-owned slot-policy configuration.

## Load truth

- `PrepareLoadAsync` performs no participant mutation.
- Prepare targets the explicit slot's current canonical generation.
- Prepare does not silently execute recovery fallback.
- Known participant payloads reuse existing preparation/migration authority.
- Unknown payload handling remains opaque/preservation-safe.
- `ApplyPreparedLoadAsync` reuses all-preflight-before-mutation semantics.
- Missing-payload behavior remains participant-descriptor owned.
- `LoadAndApplyAsync` is same-scene convenience composition only.
- No scene travel, DDOL, or durable-generation mutation is introduced.
- Convenience composition does not fabricate rollback after participant mutation begins.

## Lifecycle and architecture preservation

R1 preserved:
- base `ISaveStorageBackend`;
- base `ISaveParticipant`;
- additive `ISaveDefaultableParticipant`;
- `EchoSaveConfiguration` schema 1;
- immutable generation durability;
- explicit recovery APIs;
- no generic operation queue;
- no direct filesystem authority in the public facade;
- no runtime `UnityEditor` dependency;
- no Chronicle-owned scene travel;
- no project-wide service locator/DDOL ownership.

## Evidence

```text
EchoDevGames.EchoSave.Tests.Editor
618 / 618 passed
0 failed
```

The prior **587 / 587** floor remained green.

R1 authored **31** focused tests and the discovered Chronicle total increased from `587` to `618`, exactly matching the authored increment.

No post-apply hotfix was required.

## Remaining M4 reconciliation

R1 completion does **not** complete M4.

Still required:
1. R2 — slot-policy runtime configuration;
2. R3 — package-document migration while preserving CAP-014;
3. final 100-case registry/document evidence reconciliation;
4. final M4 closeout from actual evidence.

M5 remains locked.

## Closeout

ESV-M4-R1 is complete at implementation commit `ab18361`.

No follow-on implementation checkpoint is automatically activated by this closeout.
