---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-09
---

# ESV-M3-06 — Chronicle Current-Version Participant Payload Preparation, Trusted Runtime-Type Deserialization, and Prepared-Participant Batch Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-06
**Milestone:** M3 — Participants and Loading
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.12.0
**Prior checkpoint:** ESV-M3-05 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **243 / 243**

## 1. Intent

Move Chronicle from validated durable participant records toward safe two-phase loading **without applying gameplay state yet**.

M3-06 prepares only participant payloads whose durable schema already equals the currently registered participant schema.

```text
fully validated current generation
        ↓
known participant entry
        ↓
resolve live participant / canonical owner
        ↓
trusted live DetachedStateType
        ↓
resolve already-registered serializer provider
        ↓
current schema exactly matches?
        ↓ yes
runtime-type deserialize
        ↓
prepared detached participant entry
        ↓
deterministic all-or-nothing prepared batch
```

No participant `Apply()` call occurs in this checkpoint.

No migration occurs in this checkpoint.

No storage mutation occurs in this checkpoint.

## 2. Authorized implementation scope

### Validated generation exposure

Extend the existing read/validation path with one package-internal immutable/defensive-copy snapshot suitable for preparation.

The snapshot may expose:
- source `SaveSlotId`;
- source `SaveGenerationId`;
- fully validated participant payload entries;
- matching validated inventory metadata when needed;
- deterministic persisted participant order.

The snapshot is valid only after head, current generation, manifest/payload, identity, commit state, whole-payload integrity, inventory agreement, and per-entry integrity have all passed.

No partially validated participant payload set may be exposed.

### Trusted participant ownership and runtime type

For each persisted participant entry selected as known:
- resolve its active participant through the current `SaveParticipantRegistry`;
- canonical ID and approved alias resolution use the existing live registry authority;
- preserve the persisted participant ID for provenance;
- record the current canonical owner ID separately;
- require the live participant to provide `ISaveTypedParticipant.DetachedStateType`;
- runtime DTO `Type` authority comes only from live registration code;
- save data never contains or requests a CLR/assembly-qualified type name.

### Current-version-only schema gate

M3-06 prepares only entries where:

`persisted participant schema version == current registered participant schema version`

If persisted version is lower:
- return structured migration-required / older-participant-schema failure;
- expose no prepared batch;
- do not rewrite source;
- do not invoke participant code.

If persisted version is higher:
- return structured unsupported-newer-participant-schema failure;
- expose no prepared batch;
- preserve source unchanged;
- do not guess forward compatibility.

Participant migration chains are a later checkpoint.

### Serializer-provider resolution

For each known current-version entry:
- parse/validate its persisted `SaveSerializerId`;
- resolve only an **already registered** provider from `SaveSerializerRegistry`;
- require the provider to implement `IRuntimeTypeSaveSerializer`;
- deserialize using the trusted live `DetachedStateType`;
- do not instantiate providers from save data;
- do not use reflection/type names from save data;
- provider-not-found or missing runtime-type capability fails visibly.

### Prepared participant entry and batch

Add package-internal prepared participant entry/batch contracts that may contain:
- persisted participant ID;
- current canonical owner ID;
- participant schema version;
- serializer ID;
- trusted detached-state `Type`;
- freshly deserialized detached state object.

Rules:
- no live scene/GameObject/Component authority;
- not durable;
- no participant `Apply`;
- deterministic order by current canonical owner ID;
- duplicate canonical owners rejected;
- no batch exposed if any selected known participant fails preparation;
- result reports structured failure identity and prepared count only on success.

### Unknown payload boundary

Unknown payloads remain opaque.

M3-06 must not:
- deserialize unknown payloads;
- resolve serializer providers for unknown payloads;
- activate CLR types for unknown payloads;
- migrate unknown payloads;
- invoke participant code for unknown payloads;
- delete or rewrite unknown payloads.

The existing `UnknownPayloadStore` remains authoritative for opaque preservation/carry-forward.

## 3. Explicitly out of scope

Do not implement:
- document migrations;
- participant migration registries/chains;
- migration execution;
- `PreparedSaveLoad` public/disposable handle lifecycle;
- participant `Apply`;
- missing-payload default application;
- apply rollback orchestration;
- scene travel;
- convenience load;
- production `SaveAsync`;
- operation admission/busy/coalescing/cancellation;
- slot catalog/policy or active-slot service;
- recovery fallback;
- retention cleanup;
- autosave;
- peer-package bridges;
- Chronicle-owned/project-wide DDOL.

## 4. Failure invariants

M3-06 tests must prove:
- invalid validated-generation snapshot → no prepared batch;
- unknown participant entries are never deserialized;
- known canonical persisted ID resolves current owner;
- known alias persisted ID resolves current canonical owner without rewriting persisted ID;
- participant missing `ISaveTypedParticipant` capability → fail closed;
- null/invalid trusted runtime DTO type → fail closed;
- older participant schema → migration-required failure;
- newer participant schema → unsupported-newer failure;
- serializer provider missing → fail closed;
- serializer lacks runtime-type capability → fail closed;
- malformed participant serialized payload → fail closed;
- deserialized object type mismatch/null → fail closed;
- duplicate canonical prepared owner → fail closed;
- one participant preparation failure exposes no partial batch;
- preparation invokes no participant `Capture`;
- preparation invokes no participant `Apply`;
- preparation performs zero storage mutation;
- source generation remains unchanged;
- all prior **243 / 243** Chronicle tests remain green.

## 5. Proposed focused proof

- one current-version canonical participant prepares successfully;
- multiple current-version participants prepare in deterministic canonical order;
- persisted alias prepares under current canonical owner while retaining persisted ID provenance;
- trusted runtime `Type` comes only from live registration;
- default Unity JSON runtime-type deserialization succeeds for a plain DTO;
- alternate already-registered runtime-type serializer provider resolves;
- missing provider failure;
- provider without runtime-type capability failure;
- older schema migration-required result;
- newer schema unsupported result;
- malformed JSON failure;
- null/type mismatch failure;
- unknown payload never reaches serializer registry;
- participant `Capture` invocation count remains zero;
- participant `Apply` invocation count remains zero;
- storage mutation count remains zero;
- all-or-nothing batch behavior;
- prior **243 / 243** regression floor remains green.

Executed totals are recorded from Unity, not predicted.

## 6. Stop point

Stop when Chronicle can transform a fully validated **current-version** known participant payload set into one deterministic all-or-nothing prepared detached-state batch using only live participant identity/type authority, already-registered serializer providers, and validated durable participant payload text.

Do not apply the batch to runtime gameplay state.

Do not migrate older participant payloads yet.

The next bounded checkpoint should add explicit participant migration registration/contiguous-chain execution before `PreparedSaveLoad` and coordinated participant apply are activated.
