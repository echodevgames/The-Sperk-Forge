---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M3-04 — Chronicle Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-04
**Milestone:** M3 — Participants and Loading
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.10.0
**Prior checkpoint:** ESV-M3-03 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **197 / 197**

## 1. Intent

Protect data owned by systems that are absent from the current runtime.

M3-04 introduces a **read-only** current-generation inspection path plus a session-scoped opaque unknown-payload store.

```text
explicit SaveSlotId
      ↓
read head.json
      ↓
resolve current immutable generation
      ↓
read manifest.json + payload.json
      ↓
validate everything
      ↓
classify participant IDs against live registry
      ├── known → recognized only
      └── unknown → opaque UnknownPayloadStore
```

No participant payload is applied in this checkpoint.

No storage mutation occurs.

## 2. Authorized implementation scope

### Current committed generation reader

Add one bounded package-owned reader that:
- accepts an explicit technical `SaveSlotId`;
- uses the active `ISaveStorageBackend`, `ISaveSerializer`, and `IIntegrityProvider`;
- reads the slot `head.json`;
- validates the head document and slot identity;
- resolves the current generation ID;
- reads the immutable current generation `payload.json` and `manifest.json`;
- validates package document contracts and current package document versions;
- validates slot/generation identity agreement;
- requires committed generation state;
- validates whole serialized payload byte length/checksum;
- validates payload/manifest participant inventory agreement;
- validates every inline participant entry's own UTF-8 byte length/checksum;
- returns structured read status/diagnostics;
- never writes storage.

Do not add recovery fallback yet. A missing/corrupt current generation fails visibly in M3-04.

### Known/unknown classification

After one generation is fully validated:
- compare each participant entry against the active `SaveParticipantRegistry`;
- resolve both canonical participant IDs and registered prior-ID aliases;
- an entry that resolves to an active participant is **known**;
- an entry that resolves to no active participant is **unknown**;
- aliases are recognition only in M3-04; do not rewrite the persisted participant ID.

### Opaque UnknownPayloadStore

Add package-owned session state that:
- stores unknown payload records only;
- preserves every stored field:
  - participant ID;
  - participant schema version;
  - serializer ID;
  - required flag;
  - serialized payload;
  - byte-provider reference;
  - byte length;
  - checksum;
  - flags;
- preserves serialized payload text exactly;
- exposes deterministic canonical participant-ID ordering;
- exposes immutable/defensive-copy snapshots;
- supports clear/reset;
- does not deserialize or interpret payload contents;
- does not require the original package/participant implementation to be installed.

### Bounded safeguards

Add explicit bounded limits for this checkpoint:
- maximum unknown entry count;
- maximum aggregate unknown payload bytes;
- reject duplicates;
- reject invalid/noncanonical participant IDs;
- reject invalid metadata;
- reject unsupported byte-provider references while the package transport remains inline-only.

The exact bounded constants may be package-owned internal defaults in M3-04. Project configuration exposure may remain later if not already authorized.

### Atomic session replacement

Classification builds a new candidate unknown-payload snapshot in memory.

Only after the entire current-generation read, validation, bounds check, and classification succeeds may the active session `UnknownPayloadStore` be replaced.

If any step fails:
- return failure;
- preserve the previously valid store unchanged;
- do not expose a half-classified store.

## 3. Opaque-data invariant

Unknown payloads are durable inert records.

M3-04 must never:
- deserialize an unknown payload;
- resolve or invoke its serializer provider;
- use save data to activate a CLR type;
- run participant migration;
- invoke participant capture/apply;
- rewrite unknown serialized payload text;
- silently discard an unknown payload;
- execute code because an unknown participant ID exists.

Validation may inspect package-owned envelope fields and checksums only.

## 4. Explicitly out of scope

Do not implement:
- unknown-payload merge into fresh captured save batches;
- carry-forward generation publication;
- explicit prune plans;
- participant payload deserialization;
- participant migrations;
- participant apply;
- `PreparedSaveLoad`;
- `LoadAndApplyAsync`;
- production `SaveAsync`;
- save admission/permission/busy/coalescing/cancellation;
- autosave;
- slot catalog/policy or active-slot selection;
- recovery fallback;
- retention cleanup;
- peer-package bridges;
- Chronicle-owned/project-wide DDOL.

## 5. Failure invariants

M3-04 tests must prove:
- missing head → read failure / old unknown store unchanged;
- malformed head → failure / old store unchanged;
- missing current generation file → failure / old store unchanged;
- corrupt whole payload checksum → failure / old store unchanged;
- invalid per-entry checksum/length → failure / old store unchanged;
- payload/inventory mismatch → failure / old store unchanged;
- duplicate participant ID → failure / old store unchanged;
- unknown count/byte limit exceeded → failure / old store unchanged;
- successful read with all participants known → empty unknown store;
- successful read with absent participant → exact opaque unknown entry preserved;
- canonical participant registration recognizes canonical persisted ID;
- participant alias recognizes prior persisted ID as known;
- unknown payload text survives store snapshot exactly;
- unknown store snapshot mutation cannot mutate authoritative session state;
- classification invokes no participant capture/apply;
- classification resolves no serializer for unknown payloads;
- storage mutation calls remain zero.

## 6. Executed focused proof

- successful current head/generation read — **Pass**;
- known participant classification — **Pass**;
- unknown participant classification — **Pass**;
- alias-based known recognition — **Pass**;
- multiple unknowns remain deterministic — **Pass**;
- exact unknown payload field preservation — **Pass**;
- exact serialized payload text preservation — **Pass**;
- defensive-copy snapshot behavior — **Pass**;
- unknown-store clear/reset behavior — **Pass**;
- failed current-generation read preserves the previous valid unknown store — **Pass**;
- malformed/missing current head fails without store replacement — **Pass**;
- missing current generation file fails without store replacement — **Pass**;
- corrupt whole payload fails without store replacement — **Pass**;
- corrupt per-entry payload fails without store replacement — **Pass**;
- payload/inventory mismatch fails without store replacement — **Pass**;
- duplicate participant ID rejection — **Pass**;
- unknown count/aggregate-byte bounds rejection — **Pass**;
- unknown payload classification resolves no serializer provider — **Pass**;
- unknown payload classification invokes no participant capture/apply — **Pass**;
- current-generation inspection performs zero storage mutation — **Pass**;
- all prior 197 Chronicle tests remain green — **Pass**.

Final focused Unity gate: **218 / 218 passed, 0 failed**.

## 7. Stop point

**Reached.** Chronicle can safely read the current committed generation and remember every unclaimed participant entry as opaque session data without modifying storage or participant runtime state.

Implementation commit: `aa78e07`.

Final focused Chronicle Editor gate: **218 / 218 passed, 0 failed**.

Unknown entries are still not merged into a new generation by ESV-M3-04.

Next bounded checkpoint:

`ESV-M3-05 — Chronicle Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation`

M3-05 may join freshly captured known participant entries with an opaque unknown-payload snapshot and prove safe carry-forward into the next immutable generation.

Because the M3-04 store currently holds only entries/bytes and not source-generation provenance, M3-05 must first bind the preserved snapshot to the exact slot/generation from which it was classified. Carry-forward must fail closed if that source is stale, if current registry ownership has changed, or if a preserved identity collides with a fresh known capture.

M3-05 must not silently overwrite, drop, canonicalize, deserialize, migrate, or reinterpret an unknown entry. Production save/load admission, prune policy, participant apply, prepared loads, migrations, recovery/retention/autosave, slot catalog/policy, peer bridges, and project-wide DDOL remain later bounded work.
