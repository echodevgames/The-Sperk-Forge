---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M2-03 — Chronicle Generation Identity, Integrity, and Commit-Document Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-03
**Milestone:** M2 — Document / Storage Core
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.5.0
**Prior checkpoint:** ESV-M2-02 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **57 / 57**

## 1. Intent

Prepare the exact package-owned identities, commit-document contracts, and integrity primitive required by the approved immutable-generation commit model **before** allowing Chronicle to publish a physical generation.

```text
safe storage backend                 COMPLETE
        ↓
package JSON serializer              COMPLETE
        ↓
slot / generation technical IDs
        ↓
manifest / payload / head contracts
        ↓
SHA-256 calculation + verification
        ↓
document agreement proof
        ↓
NEXT CHECKPOINT:
candidate write → verify → publish generation → head last
```

## 2. Authorized implementation scope

### Technical identities

- `SaveSlotId` as a package-generated stable lowercase canonical GUID-style value.
- `SaveGenerationId` as a unique sortable technical ID whose uniqueness does not rely on wall clock alone.
- Validation against empty values, path/traversal characters, control characters, and collisions where applicable.
- Deterministic equality/string behavior.

This does not activate slot catalog, capacity, display naming, active-slot selection, rename, duplicate, or delete behavior.

### Package commit documents

- `SaveHeadPointer`.
- `SaveManifest`.
- `SavePayloadDocument`.
- `SavePayloadEntry`.
- Explicit independent format-version constants for those package document kinds.
- Package-owned generation commit-state enum/value where needed.
- Neutral fields needed for slot/generation identity, serializer identity, technical timestamps/metadata, payload descriptor, checksums, and transport entry inventory.

`SavePayloadEntry` is an opaque transport record in M2-03. It may carry neutral participant-like ID/version/serializer/checksum fields because the package document format owns those fields, but no participant registry, capture/apply behavior, project DTO type binding, or gameplay schema meaning is activated.

### Integrity

- `IIntegrityProvider`.
- Default `Sha256IntegrityProvider`.
- Stable integrity algorithm identity.
- SHA-256 calculation over detached byte arrays.
- Deterministic checksum verification.
- Structured integrity result/diagnostic contracts.
- Explicit documentation that checksum integrity is corruption detection, not authentication or anti-cheat.

### Agreement / in-memory validation

- Manifest/payload slot/generation agreement checks.
- Payload byte-length/checksum agreement when detached bytes are supplied.
- Head slot/current-generation identity/version validation.
- Serialization/deserialization of all M2-03 package documents through the approved serializer.
- No storage mutation during document/integrity validation.

## 3. Explicitly out of scope

Do not implement:
- generation candidate directories;
- moving/renaming a candidate into `generations/<generation-id>`;
- writing or replacing `head.json`;
- atomic/fallback head replacement or backup heads;
- slot catalog/cache;
- active-slot selection;
- slot policy/capacity;
- user-visible slot metadata operations;
- participant registry/capture/apply;
- project gameplay payload schemas;
- migrations;
- recovery candidate selection;
- retention cleanup;
- autosave;
- prepared loads;
- peer-package bridges;
- Chronicle-owned DDOL.

## 4. Integrity rules

1. Default algorithm is SHA-256.
2. Hashes detect accidental corruption; they are not authentication.
3. Hash input is detached bytes, not a live Unity object.
4. Verification is deterministic and side-effect free.
5. Invalid/empty checksum text fails structurally.
6. Package documents name the integrity algorithm explicitly where required.
7. Unsupported integrity algorithms are never guessed.

## 5. Commit-document rules

1. Slot/generation IDs are technical identity, never display names.
2. Manifest and payload agree on slot/generation identity.
3. Head points only by stable generation ID.
4. Head version/identity can be validated without gameplay payload meaning.
5. Manifest contains no arbitrary full gameplay payload.
6. Payload entries remain opaque transport records until participant checkpoints activate.
7. An empty payload document may be used for proof without pretending participant capture exists.
8. No M2-03 operation publishes a generation.

## 6. Executed focused proof

- `SaveSlotId` generation/validation/equality — **Pass**;
- `SaveGenerationId` uniqueness and canonical/sortable behavior — **Pass**;
- unsafe technical IDs rejected — **Pass**;
- default SHA-256 provider known-vector proof — **Pass**;
- altered bytes fail verification — **Pass**;
- invalid checksum text fails structurally — **Pass**;
- manifest/payload same identities pass agreement — **Pass**;
- manifest/payload mismatched slot rejected — **Pass**;
- manifest/payload mismatched generation rejected — **Pass**;
- payload byte-length/checksum mismatch rejected — **Pass**;
- unsupported head version rejected — **Pass**;
- head current-generation identity validation — **Pass**;
- manifest/payload/head round trips through `UnityJsonSaveSerializer` — **Pass**;
- empty transport payload document round trip — **Pass**;
- integrity/document validation performs zero storage mutation — **Pass**;
- all prior 57 Chronicle tests remain green — **Pass**.

Final focused Unity gate: **87 / 87 passed, 0 failed**.

## 7. Stop point

**Reached.** Chronicle can create and validate package commit documents, generate stable slot/generation identities, and prove detached bytes with SHA-256 entirely in memory.

Implementation commit: `ad3b646`.

Final focused Chronicle Editor gate: **87 / 87 passed, 0 failed**.

Next bounded checkpoint:

`ESV-M2-04 — Chronicle Immutable Generation Publication and Head-Last Commit Foundation`

M2-04 may finally cross into the physical publication protocol, but only for the already-proven package-owned empty/transport payload path. It must preserve the previous known-good generation/head on every failure before successful head publication.
