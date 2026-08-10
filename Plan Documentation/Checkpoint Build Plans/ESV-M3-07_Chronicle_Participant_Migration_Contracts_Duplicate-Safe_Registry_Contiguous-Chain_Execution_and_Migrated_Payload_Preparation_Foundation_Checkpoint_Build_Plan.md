---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-09
---

# ESV-M3-07 — Chronicle Participant Migration Contracts, Duplicate-Safe Registry, Contiguous-Chain Execution, and Migrated Payload Preparation Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-07
**Milestone:** M3 — Participants and Loading
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.13.0
**Prior checkpoint:** ESV-M3-06 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **261 / 261**

## 1. Intent

Teach Chronicle how to move an **older known participant payload** through an explicit, contiguous, deterministic in-memory migration chain before the M3-06 current-version deserialization path runs.

This checkpoint is participant-payload migration only.

```text
fully validated known participant payload
        ↓
resolve current canonical owner
        ↓
stored schema < current schema?
        ↓ yes
plan exact contiguous migration chain
        ↓
v1 -> v2 -> v3 -> ... -> current
        ↓
validate every step/result
        ↓
current-version migrated serialized payload
        ↓
M3-06 trusted current DTO Type
        ↓
registered runtime-Type serializer
        ↓
prepared detached DTO
```

The source immutable generation is never rewritten.

No participant `Apply()` occurs.

No document migration occurs.

## 2. Authorized implementation scope

### Stable participant migration identity

Add a neutral stable migration-step identity value type, for example `SaveParticipantMigrationId`.

Rules:
- non-empty;
- bounded length;
- lowercase stable serialization-safe form;
- ordinal equality/hash;
- no display-name authority;
- never inferred from CLR type names.

### Participant migration-step contract

Add one explicit project/package registration contract, for example `ISaveParticipantMigrationStep`.

Each step declares:
- stable migration-step ID;
- current **canonical participant ID** it belongs to;
- `FromSchemaVersion`;
- `ToSchemaVersion`;
- deterministic in-memory transform behavior.

For M3-07, one registered step must represent exactly one contiguous edge:

`ToSchemaVersion == FromSchemaVersion + 1`

No version skipping is allowed.

The transform receives detached migration input containing only transport-safe participant data needed for migration, including:
- persisted participant ID provenance;
- current canonical participant owner ID;
- current source schema version;
- current serializer provider ID;
- current serialized payload text;
- bounded flags/metadata that Chronicle must preserve.

A successful step returns:
- the exact expected next schema version;
- a valid serializer provider ID for the migrated serialized form;
- migrated serialized payload text;
- structured stable diagnostic context.

Migration code may understand its own historical schema, but Chronicle does not infer historical CLR types from save data.

### Duplicate-safe migration registry

Add one runtime-memory-only `SaveParticipantMigrationRegistry`.

Registry key authority:

`current canonical participant ID + FromSchemaVersion`

Rules:
- only one active step may own one canonical participant/version edge;
- duplicate step IDs reject;
- duplicate participant/version edges reject;
- malformed/non-contiguous steps reject;
- registration exposes structured result/status;
- registration may use ownership leases/tokens so stale disposal cannot remove a newer registration;
- deterministic registry snapshot/order;
- no hardcoded Chronicle participant catalog;
- future participants/migrations register through the same public contract.

Persisted aliases are first resolved through the current participant registry to their current canonical owner. The migration registry itself is keyed by current canonical owner.

### Contiguous chain planning

Add deterministic chain planning from:

`stored participant schema version -> current registered participant schema version`

Rules:
- current version requires zero migration steps;
- newer stored schema remains unsupported and never enters migration planning;
- every integer schema edge must exist exactly once;
- missing edge fails before migration execution;
- chain order is strictly ascending;
- chain depth is bounded by an explicit positive execution limit supplied to the planner/executor;
- no discovery by reflection;
- no automatic downgrade path.

### In-memory migration execution

Execute the selected chain in memory only.

For every step:
1. verify the step still owns the expected canonical participant/version edge;
2. pass the current opaque serialized participant payload state;
3. catch and structure bounded migration exceptions;
4. require successful step result;
5. require output schema to equal exactly the planned next version;
6. require output serializer ID to be syntactically valid;
7. require output serialized payload text to be non-null and within the applicable participant payload bounds already enforced by the load path;
8. retain ordered migration provenance containing stable step ID and source/target versions;
9. advance only the in-memory working payload.

Any failure:
- exposes no migrated/prepared participant entry;
- exposes no partial prepared batch;
- leaves the source generation unchanged.

### Migrated participant preparation integration

Extend the M3-06 preparation coordinator:

- current-schema known payloads keep the existing no-migration path;
- older known payloads resolve a complete migration chain;
- execute the chain to current schema in memory;
- use the final migrated serializer provider ID and serialized payload;
- then run the existing M3-06 trusted live `DetachedStateType` + registered `IRuntimeTypeSaveSerializer` current-version deserialization;
- preserve original persisted ID;
- preserve current canonical owner ID;
- record original stored schema version, final current schema version, migration step count, and ordered stable migration provenance in package-internal prepared-entry metadata;
- one participant migration failure fails the complete preparation batch.

### Unknown payload boundary

Unknown payloads remain opaque.

M3-07 must not:
- look up migration steps for an unknown participant;
- deserialize unknown payloads;
- invoke unknown participant code;
- reinterpret unknown schema numbers;
- delete/rewrite unknown payloads.

M3-04/M3-05 unknown preservation/carry-forward remains authoritative.

## 3. Explicitly out of scope

Do not implement:
- package/document migration registry or execution;
- downgrade migrations;
- automatic migration discovery/reflection scanning;
- source-generation rewrite after successful migration;
- automatic recommit of migrated data;
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

M3-07 tests must prove:
- invalid migration ID rejects;
- non-contiguous step (`1 -> 3`) rejects;
- duplicate migration step ID rejects;
- duplicate canonical participant/from-version edge rejects;
- stale registration token cannot unregister replacement step;
- deterministic registry snapshot/order;
- current-version payload plans zero steps;
- newer payload never enters migration execution;
- one-step old payload plans/executes exactly one step;
- multi-step old payload executes every contiguous edge in ascending order;
- missing middle edge returns migration-chain-missing before executing any step;
- execution depth limit rejects an overlong chain;
- step throws → structured migration failure;
- step returns failure → structured migration failure;
- step returns wrong target version → fail closed;
- step returns invalid serializer ID → fail closed;
- step returns invalid/null serialized payload → fail closed;
- successful migration preserves source immutable generation;
- migration invokes neither participant `Capture` nor participant `Apply`;
- unknown participant triggers zero migration registry lookup/execution;
- persisted alias resolves current canonical owner before migration planning;
- final migrated current-version payload deserializes using the current live DTO `Type`;
- migration provenance records ordered stable step IDs/version edges without payload contents;
- any participant migration/preparation failure exposes no partial prepared batch;
- all prior **261 / 261** Chronicle tests remain green.

## 5. Proposed focused proof

- migration ID validation/equality;
- valid `v1 -> v2` registration;
- duplicate ID and duplicate edge rejection;
- stale registration lease protection;
- deterministic registry snapshot;
- zero-step current-version plan;
- one-step plan and execution;
- two-step contiguous plan and execution;
- missing-step failure with zero execution;
- bounded maximum-step failure;
- throwing/failing/malformed-output migration step failures;
- serializer-ID change across a valid migration step;
- persisted alias → canonical owner migration route;
- current-version deserialization after migration;
- ordered migration provenance;
- all-or-nothing preparation batch across mixed current/migrated participants;
- unknown payload migration non-use;
- zero `Capture`;
- zero `Apply`;
- zero storage mutation;
- prior **261 / 261** regression floor remains green.

Executed totals are recorded from Unity, not predicted.

## 6. Stop point

Stop when Chronicle can take a fully validated **older supported known participant payload**, resolve an explicit complete contiguous migration chain to the current registered schema, execute that chain deterministically in memory, and feed the resulting current-version serialized payload into the already-proven M3-06 trusted DTO preparation path.

Do not produce a public `PreparedSaveLoad` handle yet.

Do not apply participant state yet.

Do not add document migrations yet.

The next bounded checkpoint should establish the disposable prepared-load handle/session-lifetime contract around one fully validated, migrated, prepared in-memory load before coordinated participant apply is activated.
