---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M2-02 — Chronicle Document Contracts and Unity JSON Serializer Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-02
**Milestone:** M2 — Document / Storage Core
**Status:** **COMPLETE**
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

## Executed focused proof

- serializer registry accepts unique provider identity — **Pass**;
- duplicate serializer identity rejected deterministically — **Pass**;
- default Unity JSON serializer resolves — **Pass**;
- supported package DTO round trip preserves authored fields — **Pass**;
- empty/null serialize requests fail structurally — **Pass**;
- empty/null/malformed deserialize requests fail structurally — **Pass**;
- unsupported package-document versions block without storage mutation — **Pass**;
- serializer/document layer performs zero filesystem I/O — **Pass**;
- storage backend remains independently replaceable — **Pass**;
- all M1/M2-01 regressions remain green — **Pass**.

Final focused Unity gate: **57 / 57 passed, 0 failed**.

## Stop point

**Reached.** Chronicle package documents serialize/deserialize deterministically in memory, validate explicit package-document versions, and preserve the prior storage/lifecycle regression floor.

Implementation commit: `6404037`.

Next bounded checkpoint:

`ESV-M2-03 — Chronicle Generation Identity, Integrity, and Commit-Document Foundation`

M2-03 prepares the exact package-owned IDs, commit documents, and integrity primitive required before any physical immutable generation may be published. It still does not create generation directories or mutate `head.json`.
