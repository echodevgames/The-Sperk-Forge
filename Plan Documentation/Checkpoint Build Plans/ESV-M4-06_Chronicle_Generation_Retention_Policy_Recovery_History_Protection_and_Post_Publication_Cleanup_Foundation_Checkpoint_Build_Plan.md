---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active
updated: 2026-08-10
---
# ESV-M4-06 — Chronicle Generation Retention Policy, Recovery-History Protection, and Post-Publication Cleanup Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-06
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.25.0
**Decision:** ESV-D-028
**Prior checkpoint:** ESV-M4-05 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **473 / 473**
**Exact implementation baseline:** `3cdad0f`

## 1. Intent

Bound committed generation growth without weakening Chronicle's durable-truth model or accidentally deleting recovery evidence.

M4-06 answers one bounded question:

> After a new generation and `head.json` have committed successfully, can Chronicle deterministically remove only excess verified committed history while always protecting the current generation and its immediate predecessor, failing closed on untrustworthy evidence, and reporting cleanup failure without pretending the save rolled back?

```text
save publication
      ↓
verified generation
      ↓
head.json LAST
      ↓
committed save truth
      ↓
retention preflight
      ↓
bounded generation discovery
      ↓
classify verified committed history
      ↓
protect current + immediate predecessor
      ↓
delete oldest excess eligible generation trees
      ↓
retention result
      ├── clean
      └── maintenance warning/failure
           committed save remains committed
```

Retention is maintenance after commit. It is not part of the commit transaction.

## 2. Carried-forward authority

Chronicle already proves:

- immutable committed generation publication;
- `head.json` last;
- current head + `previousGenerationId`;
- provider-neutral child-directory discovery through an additive capability;
- one root-local mutating admission authority;
- public manual save;
- explicit caller-triggered autosave with one pending latest request;
- truthful partial durable/head/catalog outcomes;
- base `ISaveStorageBackend.Delete(SaveStorageKey)` for single opaque objects;
- focused Chronicle Editor **473 / 473**.

The package specification already requires:
- bounded generation history;
- retention cleanup after successful publication, never before;
- recovery history from retained prior valid generations;
- project-owned `SaveRetentionPolicy`;
- corrupt/incomplete/unsupported material to be preserved or quarantined by policy rather than silently treated as ordinary retention garbage.

### ESV-D-028 — retention is post-commit maintenance, never commit authority

> Generation retention may remove only verified committed non-current generations after the new generation and `head.json` are already authoritative. It must protect the current generation and the head's immediate recovery predecessor. Failure to complete retention is maintenance failure, not fictional rollback of a committed save.

## 3. Authorized implementation scope

### SaveRetentionPolicy

Add a neutral project-owned policy definition for total committed-generation retention.

Required semantics:
- positive bounded total generation count;
- minimum configured value of `2`;
- the current generation counts toward the bound;
- the immediate predecessor counts toward the bound;
- an initial slot with only one generation remains valid;
- policy validation is explicit and testable;
- no trash-history or autosave-specific sub-bound yet.

Do not widen `EchoSaveConfiguration` schema or Setup authoring in this checkpoint unless required by already-implemented runtime construction. M4-06 may use an internal/default/test-injected policy seam while preserving the project-owned policy type for later configuration wiring.

### Provider-neutral generation discovery

Use the existing additive discovery capability to enumerate immediate children beneath:

`slots/<slot-id>/generations`

Rules:
- discovery is bounded;
- canonical `SaveGenerationId` directory names only become retention candidates;
- noncanonical children are ignored/preserved, not deleted;
- a result exceeding the discovery bound fails retention preflight and deletes nothing.

### Additive tree deletion capability

The base `ISaveStorageBackend` contract remains unchanged.

Add one optional provider capability for deleting a complete storage subtree by validated `SaveStorageKey`.

The default local backend may implement this capability.

Rules:
- the capability is provider-neutral;
- caller supplies only a validated key under the backend root;
- missing tree is a truthful not-found/no-change result according to the final contract;
- no retention coordinator may use `System.IO` directly;
- no arbitrary absolute path enters the API.

### Candidate classification

Before deletion, retention must establish trustworthy candidate truth.

For each canonical discovered generation:
- read `manifest.json`;
- deserialize and validate it;
- require matching slot ID;
- require matching generation ID;
- require `Committed` state;
- parse a deterministic timestamp/order field;
- do not open participant payload merely to decide age;
- malformed, unsupported, corrupt, or mismatched material is preserved and makes that item ineligible for ordinary retention deletion.

If required classification cannot establish a trustworthy bounded set, prefer fail-closed behavior over broad deletion.

### Protected generations

Retention must never delete:
- `head.currentGenerationId`;
- valid non-empty `head.previousGenerationId`.

If the configured total bound would require deleting either protected generation, the protected generation wins and the effective retained count may temporarily exceed the nominal bound.

M4-06 does not rewrite `head.json` merely to make retention easier.

### Deletion ordering

Eligible excess verified committed generations are deleted oldest first.

Ordering:
1. validated manifest technical timestamp;
2. canonical generation ID ordinal tie-break.

Deletion is deterministic.

### Post-publication integration

Retention occurs only after:
- generation publication succeeded;
- final generation verification succeeded;
- `head.json` publication succeeded.

It must never run:
- against a candidate generation;
- before head publication;
- after a failed save that did not commit a new head.

Manual save and autosave share the same retention path because M4-05 deliberately reuses the same durable save transaction.

### Result truth

Retention reports:
- not required;
- completed;
- partial/failed maintenance;
- unsupported provider capability;
- invalid policy;
- untrustworthy discovery/classification.

A retention failure after commit:
- does **not** revert the head;
- does **not** delete the committed current generation;
- does **not** turn `GenerationPublished` or `HeadPublished` false;
- may surface a bounded diagnostic/maintenance warning in the encompassing save result.

## 4. Explicit non-scope

Do not add:
- recovery-plan generation;
- automatic fallback to older generation;
- head repair;
- quarantine movement;
- corrupt-generation deletion;
- rename slot;
- duplicate slot;
- delete slot;
- trash/quarantine retention;
- persistent `catalog.cache.json`;
- generic mutating-operation queues;
- queue capacity/overflow configuration;
- automatic autosave timers or gameplay triggers;
- permission-provider production wiring;
- full configuration/Setup authoring;
- document migration;
- scene travel;
- peer bridges;
- service locator;
- Chronicle-owned/project-wide DDOL.

## 5. Safety invariants

Tests must prove:
- invalid retention bounds reject before deletion;
- current generation is never deleted;
- immediate predecessor is never deleted;
- retention never runs before successful head publication;
- failed publication performs zero retention deletion;
- two-generation history is preserved even at the minimum bound;
- excess verified committed history deletes oldest first;
- noncanonical generation child is preserved;
- unreadable/mismatched/uncommitted manifest is preserved;
- discovery limit failure deletes nothing;
- missing optional tree-deletion capability deletes nothing and reports unsupported maintenance;
- tree-deletion failure does not fabricate save rollback;
- manual save and autosave both reach the same retention path after committed publication;
- retention coordinator owns no direct filesystem authority;
- base `ISaveStorageBackend` remains unchanged;
- recovery/quarantine/slot-delete scope remains absent;
- all prior **473 / 473** Chronicle tests remain green.

## 6. Proposed focused proof

- policy bounds and minimum;
- bounded generation discovery;
- canonical candidate filtering;
- manifest-only classification;
- protected current generation;
- protected immediate predecessor;
- deterministic oldest-first plan;
- exact tree deletion;
- missing tree-delete capability;
- delete failure / partial cleanup truth;
- zero cleanup before commit;
- manual-save post-publication cleanup;
- autosave post-publication cleanup;
- deferred-scope audit;
- prior **473 / 473** regression floor.

Executed totals are recorded from Unity, never predicted.

## 7. Stop point

Stop when Chronicle can bound ordinary committed generation history after a successful save while:

1. protecting current and immediate predecessor;
2. deleting only verified committed excess generations;
3. failing closed on untrustworthy discovery/classification;
4. using provider-neutral storage capabilities;
5. preserving committed-save truth if maintenance fails.

Do **not** implement recovery execution yet.

Do **not** add slot deletion/trash or generic operation queues.
