---
tags:
  - sfgss/checkpoint-build-plan
  - sfgss/package/chronicle
  - sfgss/reconciliation
status: active
updated: 2026-08-11
---
# ESV-M4-R1 — Chronicle Public Runtime Composition and Consumer Facade Reconciliation — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-R1
**Milestone:** M4 — Slots / Autosave / Recovery Reconciliation
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.35.0
**Authority decision:** ESV-D-033
**Planning baseline:** `48454ea`
**Unity baseline:** 6000.3.8f1
**Carried focused Chronicle Editor floor:** **587 / 587**
**M5 state:** **LOCKED**

## 1. Why this checkpoint exists

The post-M4-10 milestone audit found that Chronicle's internal M3/M4 machinery is substantially implemented and tested, but the primary consumer service does not yet compose several approved MVP capabilities.

The gap is not a request to redesign persistence.

It is a bounded composition repair.

R1 makes the already-proven participant registry, catalog, technical slot creation, prepared-load, and apply foundations usable through the package's intended public service.

## 2. Audit findings owned by R1

R1 owns:

### A-01 — Public load facade

Current state:
- `PreparedSaveLoad` exists publicly;
- prepared-load lifetime/store machinery exists;
- participant preparation and participant migration foundations exist;
- deterministic prepared-load apply exists;
- `IEchoSaveService` does not expose prepare/apply/convenience load.

R1 outcome:
- public prepare;
- public apply;
- public same-scene convenience composition.

### A-02 — Public catalog/create/select facade

Current state:
- `SaveSlotCatalog`, `SaveSlotCatalogSnapshot`, `SaveSlotCatalogRefreshResult`, and active selection exist;
- M4-02 technical slot creation exists;
- `IEchoSaveService` does not expose catalog/create/select;
- participant registry exists internally but is not exposed through the primary service.

R1 outcome:
- public participant registration;
- public catalog snapshot;
- public refresh;
- public create;
- public select.

## 3. Explicitly not owned by R1

R1 does not resolve:

### A-03 — CAP-002 slot policy

The current hard technical capacity remains `64`.

R2 will:
- define the runtime slot-policy model;
- advance `EchoSaveConfiguration`;
- support approved single/fixed/configurable/bounded-unlimited semantics;
- make M5 Setup author that runtime truth.

### A-04 — CAP-014 package-document migration

R3 will implement package-document migration.

CAP-014 remains intact as **document and participant migration**.

R1 does not weaken the capability.

## 4. Public API target

R1 extends `IEchoSaveService` with:

```csharp
SaveParticipantRegistrationResult RegisterParticipant(
    ISaveParticipant participant);

SaveSlotCatalogSnapshot GetCatalogSnapshot();

Awaitable<SaveSlotCatalogRefreshResult>
    RefreshCatalogAsync();

Awaitable<SaveSlotCreateResult>
    CreateSlotAsync(
        SaveSlotCreateRequest request);

SaveActiveSlotSelectionResult SelectSlot(
    SaveSlotId slotId);

Awaitable<PreparedLoadCreationResult>
    PrepareLoadAsync(
        SaveLoadRequest request);

Awaitable<SavePreparedLoadApplyResult>
    ApplyPreparedLoadAsync(
        PreparedSaveLoad handle);

Awaitable<SaveLoadResult>
    LoadAndApplyAsync(
        SaveLoadRequest request);
```

Existing methods remain source-compatible.

## 5. Public type reconciliation

### 5.1 Catalog

Use existing public types:
- `SaveSlotCatalogEntry`;
- `SaveSlotCatalogSnapshot`;
- `SaveSlotCatalogRefreshResult`;
- `SaveActiveSlotSelectionResult`.

Do not create duplicate `SaveCatalogSnapshot` / `SaveSlotMetadata` models merely to mirror older specification wording.

### 5.2 Slot creation

Internal `SaveTechnicalSlotCreateRequest`, `SaveTechnicalSlotCreateResult`, and `SaveTechnicalSlotCreateStatus` remain implementation details.

Add consumer-facing:
- `SaveSlotCreateRequest`;
- `SaveSlotCreateResult`;
- `SaveSlotCreateStatus`.

The facade maps faithfully to the M4-02 technical coordinator and preserves durable publication versus catalog-reconciliation truth.

### 5.3 Load request/result

Add one bounded public `SaveLoadRequest`.

R1 request semantics:
- one explicit `SaveSlotId`;
- optional cancellation token if required by existing operation conventions;
- current canonical generation only;
- no hidden automatic recovery;
- no scene identity;
- no project gameplay type information.

Add one public `SaveLoadResult` for same-scene convenience composition.

The result must distinguish:
- preparation failure;
- apply preflight failure;
- partial/failed participant apply after mutation began;
- success;
- lifecycle/Busy/cancellation where applicable.

Do not fabricate rollback after participant mutation.

### 5.4 Prepared apply

Use the existing public `SavePreparedLoadApplyResult`.

Do not add `ApplyLoadOptions` in R1.

Missing-payload behavior remains participant-descriptor policy established by M3-09.

## 6. Participant registration composition

`EchoSaveService` must own/use exactly the existing `SaveParticipantRegistry`.

Public registration:
- requires package authority;
- is main-thread;
- is memory-only;
- returns existing `SaveParticipantRegistrationResult`;
- exposes the disposable `SaveParticipantRegistration`;
- preserves duplicate canonical ID and alias collision rules;
- does not capture;
- does not apply;
- does not touch storage.

No central package participant catalog may be introduced.

## 7. Catalog composition

### 7.1 Snapshot

`GetCatalogSnapshot()`:
- returns the current immutable `SaveSlotCatalogSnapshot`;
- performs no storage I/O;
- exposes no mutable catalog internals;
- is valid only when Chronicle has initialized enough to own a catalog;
- may be read while other operations are active.

### 7.2 Refresh

`RefreshCatalogAsync()`:
- reuses `SaveSlotCatalog.Refresh()`;
- remains payload-free;
- never opens participant payloads merely to list slots;
- must surface lifecycle/Busy truth without inventing an unbounded queue;
- must preserve active-selection reconciliation behavior.

## 8. Slot create/select composition

### 8.1 Create

`CreateSlotAsync()`:
- validates the public request;
- adapts to M4-02 technical creation;
- uses current R1 capacity `64`;
- reuses package-generated `SaveSlotId`;
- publishes one real empty immutable generation with head last;
- reconciles the catalog;
- never auto-selects;
- reports published-but-unreconciled truth if catalog refresh fails.

No new slot publication path may be invented.

### 8.2 Select

`SelectSlot()`:
- is main-thread and memory-only;
- uses the current authoritative catalog snapshot;
- requires a selectable entry;
- never writes storage;
- never changes display metadata;
- never selects merely because create succeeded.

## 9. Prepared load composition

R1 must compose, not replace, the M3 foundations.

Required existing concepts include:
- current-generation validation;
- unknown-payload preservation;
- serializer registry/provider resolution;
- participant preparation;
- participant migration;
- prepared-load store/lifetime;
- `PreparedSaveLoad`;
- deterministic apply planner;
- deterministic apply executor.

### 9.1 Prepare

`PrepareLoadAsync()`:
- targets one explicit live slot;
- loads the current canonical generation;
- validates source/integrity;
- preserves unknown payloads;
- prepares/migrates known participant payloads;
- creates one bounded disposable prepared handle;
- performs zero participant mutation;
- performs zero durable mutation;
- does not silently execute recovery fallback.

If the source requires recovery, return structured failure and leave recovery to the existing explicit recovery APIs.

### 9.2 Apply

`ApplyPreparedLoadAsync()`:
- requires Ready state;
- validates the handle;
- performs complete apply preflight before the first participant mutation;
- uses participant-owned missing-payload policy;
- consumes/disposes the handle according to existing prepared-load lifecycle truth;
- mutates participant runtime state only;
- does not write save storage;
- does not perform scene travel.

### 9.3 Convenience load

`LoadAndApplyAsync()`:
1. prepare;
2. if prepare fails, do not apply;
3. if prepare succeeds, apply in the current scene;
4. return one truthful terminal `SaveLoadResult`;
5. ensure the convenience path does not leak an abandoned handle;
6. never pretend participant state rolled back when an apply failure occurred after mutation began.

## 10. Operation admission

R1 may reuse the existing root-local operation admission authority to prevent incompatible overlap.

It may not introduce:
- a generic queue;
- load queues;
- catalog-refresh queues;
- priority scheduling;
- configurable queue capacity.

Expected bounded behavior:
- memory-only catalog snapshot read may proceed;
- registration remains memory-only/main-thread;
- storage-reading refresh/prepare and participant-mutating apply may reject Busy when exclusive authority is occupied;
- convenience load owns whatever bounded admission is required to avoid save/apply races.

The implementation must document the exact lease boundaries it chooses.

## 11. Lifecycle truth

Every new facade operation must report current service lifecycle truth.

At minimum distinguish:
- before Ready;
- Ready;
- ShuttingDown / Shutdown;
- Busy when applicable;
- invalid request;
- operation-specific terminal state.

Shutdown must release/dispose prepared-load runtime resources without mutating durable save generations.

## 12. Architecture invariants

R1 must preserve:

- base `ISaveStorageBackend` unchanged;
- base `ISaveParticipant` unchanged;
- optional `ISaveDefaultableParticipant` remains additive;
- no `UnityEditor` runtime dependency;
- no direct scene authority;
- no project-wide service locator;
- no Chronicle-owned/project-wide DDOL composition;
- no gameplay schemas in Chronicle;
- no display-name-as-path identity;
- no in-place committed generation mutation;
- no hidden automatic recovery;
- no public exposure of internal `SaveTechnicalSlot*` creation types.

## 13. Expected implementation areas

Likely runtime areas:
- `Runtime/Contracts/IEchoSaveService.cs`;
- `Runtime/Core/EchoSaveService.cs`;
- public facade DTO/result/status files for create/load;
- existing catalog/participant/prepared-load/apply types only where public accessibility or bounded status expansion is required;
- existing runtime builder/reset composition.

Likely tests:
- one focused public service surface test class;
- participant-registration service tests;
- catalog/create/select service tests;
- public prepared-load service tests;
- convenience-load service tests;
- shared R1 service test support where necessary.

Do not modify unrelated M4 coordinator semantics merely to make public facade tests convenient.

## 14. Focused proof matrix

### Public surface
- R1 methods exist with exact bounded signatures.
- Existing M4 public methods remain present.
- No M5/editor/scene/public restore/permanent erase APIs appear.

### Registration
- unique registration succeeds;
- duplicate canonical ID rejects;
- alias collision rejects;
- dispose/unregister ownership remains correct;
- registration causes zero storage mutation and zero participant callbacks.

### Catalog
- snapshot is memory-only;
- refresh preserves payload-free M4-01 behavior;
- degraded canonical slots remain represented honestly;
- refresh failure preserves prior snapshot truth.

### Create/select
- create succeeds through M4-02 path;
- capacity rejection remains enforced at current R1 bound;
- create does not auto-select;
- published-but-unreconciled truth remains visible;
- select requires current selectable catalog entry;
- select is session-only and storage-free.

### Prepare
- healthy current source produces prepared handle;
- prepare performs zero participant apply;
- prepare performs zero durable mutation;
- unknown payloads remain opaque/preserved;
- participant preparation/migration failures block handle creation;
- corrupt source does not silently auto-recover;
- invalid/missing slot rejects;
- prepared-handle bounds/lifetime remain enforced.

### Apply
- successful preflight applies deterministic participants;
- missing-payload Ignore/Fail/InitializeDefault semantics remain intact;
- all preflight blockers occur before participant mutation;
- expired/disposed/foreign/stale handle rejects;
- apply performs no save-storage mutation;
- source generation remains immutable.

### Convenience
- same-scene prepare+apply succeeds;
- prepare failure performs no apply;
- apply preflight failure reports no mutation;
- post-mutation participant failure reports truthful partial apply state;
- convenience path leaves no abandoned prepared handle.

### Regression
- incoming focused floor: **587 / 587**;
- final actual discovered total must be recorded;
- zero regressions permitted.

## 15. Documentation closeout requirements

On R1 completion:
- package/root test report;
- package checkpoint closeout;
- root implementation closeout;
- CHANGELOG/README/index;
- package/root Current Notes;
- Suite Health;
- Chronicle specification authority update;
- actual implementation commit and actual Unity test total.

R1 closeout must leave R2 as the next reconciliation checkpoint and must not declare M4 complete.

## 16. Stop rule

Pause only for a truly authority-changing contradiction.

Ordinary bounded public-facade type naming, result-shape composition, test support, and lifecycle/status details are implementation decisions inside ESV-D-033 and do not require another approval.

## 17. Completion rule

`ESV-M4-R1` is complete only when:

1. all authorized consumer facade operations exist;
2. they reuse existing M3/M4 authorities;
3. focused R1 tests pass;
4. the entire Chronicle focused regression suite passes at or above **587 / 587**;
5. documentation records the actual evidence;
6. R2 remains the next gate;
7. M5 remains locked.
