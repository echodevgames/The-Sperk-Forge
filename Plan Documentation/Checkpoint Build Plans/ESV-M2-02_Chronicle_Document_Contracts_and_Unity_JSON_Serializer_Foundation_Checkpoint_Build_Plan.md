---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-09
---

# ESV-M2-02 — Chronicle Document Contracts and Unity JSON Serializer Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-02
**Milestone:** M2 — Document / Storage Core
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.4.0
**Prior checkpoint:** ESV-M2-01 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor `40 / 40`

## Intent

Give Chronicle a package-owned in-memory document language and the approved default serializer without yet publishing a durable save generation.

The separation is:

```text
package-owned document DTO
        ↓
ISaveSerializer
        ↓
UnityJsonSaveSerializer
        ↓
serialized representation

NOT YET:
serialized representation
        ↓
immutable generation publication
```

## Authorized scope

- `SaveDocumentVersions` or equivalent explicit package document-format constants;
- package-owned DTO contracts required for the Chronicle envelope layer;
- package-owned document identity/version fields;
- `UnityJsonSaveSerializer` using Unity `JsonUtility`;
- serializer provider identity and registration/lookup;
- structured serialize/deserialize result contracts and stable diagnostics;
- deterministic round-trip tests for supported package-owned plain DTOs;
- malformed JSON rejection;
- unsupported older/newer package-document version rejection where that decision can be made at this boundary;
- deterministic behavior for null/empty input;
- preservation of storage-provider independence;
- all M1 + M2-01 regressions.

## Package-document boundary

Package-owned documents may describe Chronicle transport/envelope facts such as:

- document kind;
- package document schema/version;
- serializer identity;
- timestamps/technical metadata when already authorized by the specification;
- future references to stable slot/generation/participant identities as neutral serialized fields.

They do not define the meaning of Inventory, Objectives, Progression, Characters, World, or any other gameplay payload.

## Default serializer constraints

The default serializer:

1. is package-owned;
2. uses Unity `JsonUtility`;
3. operates on plain serializable DTOs;
4. has a stable `SaveSerializerId`;
5. reports malformed/unsupported input through structured results;
6. does not perform filesystem I/O;
7. does not publish generations;
8. remains replaceable through the serializer-provider seam.

JsonUtility limitations must remain explicit rather than being hidden by reflection-heavy magic.

## Out of scope

Do not implement in ESV-M2-02:

- slot creation/catalog/active-slot selection;
- physical generation directory creation;
- immutable generation publication;
- head pointer publication/update;
- checksums or integrity provider;
- participant registry/capture/apply;
- participant payload migration;
- unknown payload preservation;
- document migration chains beyond explicit unsupported-version detection required by this slice;
- recovery;
- autosave;
- prepared loads;
- UI/tooling/Laboratory;
- peer-package bridges;
- Chronicle-owned DDOL.

## Proposed focused proof

- serializer registry accepts unique provider identity;
- duplicate serializer identity rejected deterministically;
- default Unity JSON serializer can be resolved;
- supported package DTO round trip preserves all authored fields;
- empty/null serialize request returns structured failure;
- empty/null/malformed deserialize request returns structured failure;
- unsupported document version blocks without storage mutation;
- serializer operation performs zero filesystem I/O;
- storage backend remains independently replaceable;
- all prior 40 focused tests remain green.

Executed counts are recorded only from Unity.

## Stop point

Stop when Chronicle package documents can be deterministically serialized/deserialized and version-validated in memory.

Do not continue into immutable generation commit/publication, head mutation, slots, participants, integrity/recovery, or autosave without the next bounded checkpoint.
