---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-09
---

# ESV-M3-03 — Chronicle Participant-Backed Generation Publication and Head-Last Integration Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-03
**Milestone:** M3 — Participants and Loading
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.9.0
**Prior checkpoint:** ESV-M3-02 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **171 / 171**

## 1. Intent

Join the already-proven participant capture batch to the already-proven durable generation transaction without yet creating the final production save-operation API.

```text
SaveParticipantRegistry
        ↓
M3-02 detached capture
        ↓
verified all-or-nothing participant batch
        ↓
M3-03 publication-boundary validation
        ↓
participant-bearing SavePayloadDocument
+ matching SaveManifest inventory
        ↓
M2 candidate write
        ↓
candidate read-back verification
        ↓
immutable generation publication
        ↓
published-generation revalidation
        ↓
HEAD LAST
```

## 2. Authorized implementation scope

### Preserve M2 proof path

Keep the existing empty/transport generation publication path working for regression and bounded technical proof.

M3-03 may refactor shared publication internals only when that reduces duplication without weakening the established generation-first/head-last invariants.

### Participant-batch publication entry point

Add one bounded technical publication entry point that accepts:
- explicit technical `SaveSlotId`;
- project/build/display metadata already used by M2 publication;
- one successful `SaveParticipantCaptureBatchResult`.

Do not make this the final public `SaveAsync` API.

### Publication-boundary validation

Before the first storage write, revalidate the capture batch defensively.

Reject:
- null or unsuccessful capture batch;
- null payload or inventory entries;
- payload/inventory count mismatch;
- duplicate canonical participant IDs;
- invalid participant IDs;
- non-positive participant schema versions;
- empty/invalid serializer IDs;
- unsupported flags;
- non-empty byte-provider references while M3-03 remains inline-payload-only;
- payload/inventory metadata disagreement;
- negative byte lengths;
- exact UTF-8 byte-length mismatch;
- exact per-entry checksum mismatch.

Validation failure must leave the storage backend untouched.

### Participant-bearing package documents

Construct:
- `SavePayloadDocument.entries` from the validated participant entries;
- `SaveManifest.payloadEntries` from the matching inventory entries.

Then:
- serialize the complete package payload document;
- compute generation-level payload byte length over the complete serialized payload document;
- compute generation-level checksum over those exact serialized bytes;
- preserve the active integrity-provider ID in the manifest;
- preserve deterministic participant order.

### Durable transaction integration

Use the established M2 sequence:
1. read/validate existing head;
2. write complete candidate payload;
3. write complete candidate manifest;
4. read back and verify candidate;
5. publish candidate tree as immutable generation;
6. read back and revalidate published generation;
7. construct next head with previous generation and incremented sequence;
8. publish `head.json` **last**.

No committed participant-bearing generation file is edited in place.

## 3. Reliability invariants

M3-03 must prove:
- invalid capture batch → zero storage mutation;
- invalid per-entry metadata/checksum → zero storage mutation;
- candidate payload failure → previous head unchanged;
- candidate manifest failure → previous head unchanged;
- candidate verification failure → previous head unchanged;
- generation publication failure → previous head unchanged;
- published-generation verification failure → previous head unchanged;
- head serialization failure → previous head unchanged;
- head publication failure → previous head unchanged and new generation non-current/orphaned;
- successful first participant-backed generation becomes current;
- successful second participant-backed generation becomes current while the first remains preserved;
- published payload entries survive disk round trip in canonical order;
- manifest inventory still agrees with payload entries after read-back;
- the existing empty/transport generation proof remains green.

## 4. Explicitly out of scope

Do not implement:
- final public `SaveAsync`;
- save request admission/permission checks;
- busy/reject/coalescing policy;
- cancellation semantics;
- autosave requests;
- unknown-payload preservation or merging;
- participant apply;
- prepared/convenience load;
- migrations;
- slot catalog/policy or active-slot state;
- recovery planning/execution;
- retention cleanup;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

## 5. Data-preservation warning

Because unknown-payload carry-forward is not yet implemented, M3-03 must remain a bounded technical integration path and test surface.

It must not be presented as the final production save API for arbitrary historical saves. The later production save operation must preserve unclaimed durable participant payloads according to Chronicle policy before it can safely replace a current generation in real consumer workflows.

## 6. Proposed focused proof

- successful one-participant generation publication;
- successful multi-participant generation publication;
- deterministic participant order survives serialization/read-back;
- manifest inventory matches participant payload entries;
- exact inline participant byte-length/checksum revalidation before storage;
- duplicate participant ID rejected before storage;
- invalid participant entry rejected before storage;
- mismatched inventory rejected before storage;
- unsupported byte-provider reference rejected before storage;
- first participant-backed head publication;
- second participant-backed head advances sequence and preserves prior generation;
- candidate payload write failure preserves head;
- candidate manifest write failure preserves head;
- candidate verification failure preserves head;
- generation publication failure preserves head;
- published-generation revalidation failure preserves head;
- head serialization failure preserves head;
- head publication failure leaves orphan non-current;
- empty/transport M2 publication remains green;
- all prior **171 / 171** Chronicle tests remain green.

Executed totals are recorded from Unity, not predicted.

## 7. Stop point

Stop when Chronicle can durably publish one fully captured participant snapshot through the existing immutable-generation/head-last transaction while preserving the previous known-good generation across injected failures.

Do not expose the path as production `SaveAsync` during ESV-M3-03.

The next bounded work must address the remaining production-operation concerns, especially unknown-payload preservation and explicit save-operation admission semantics, before Chronicle can safely present a consumer-facing save operation.
