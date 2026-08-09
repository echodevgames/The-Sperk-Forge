---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M3-02 — Chronicle Detached Participant Capture, Runtime Type Routing, and Payload-Entry Construction Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-02
**Milestone:** M3 — Participants and Loading
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.8.0
**Prior checkpoint:** ESV-M3-01 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **147 / 147**

## 1. Intent

Turn the open-ended participant registry into a deterministic, verified **in-memory transport batch** without yet touching Chronicle's durable generation transaction.

```text
SaveParticipantRegistrySnapshot
        ↓ canonical ID order
participant.Capture()
        ↓
detached participant DTO
        ↓ live-registration type authority
serializer registry
        ↓
serialized payload text
        ↓ UTF-8 bytes
integrity provider
        ↓
SavePayloadEntry
+
SavePayloadInventoryEntry

NO GENERATION WRITE
NO HEAD UPDATE
```

## 2. Authorized implementation scope

### Runtime detached DTO type authority

Chronicle may extend the participant/serializer contracts as needed so that:
- the active participant declares the runtime detached DTO type it owns;
- serializers can operate on a trusted runtime `System.Type` supplied by active code;
- existing generic serializer APIs remain available where practical;
- a save document stores **no CLR assembly-qualified type name**;
- save-file text can never request arbitrary type activation;
- later deserialization will derive type authority from the active registered participant, not from stored type metadata.

### Deterministic capture orchestration

Implement a package-owned in-memory capture coordinator that:
- consumes an immutable participant registry snapshot / active registrations;
- visits participants in canonical participant-ID order;
- invokes `Capture()` only during the explicit capture operation;
- validates successful capture status;
- rejects null detached state;
- validates detached state against the participant-declared runtime DTO type;
- records structured participant-specific diagnostics.

Capture/apply remain Unity-main-thread operations by authority. M3-02 does not introduce background Unity-object access.

### Serializer resolution

For each participant:
- use the descriptor's explicit serializer ID when present;
- otherwise resolve the Chronicle default serializer provider;
- fail visibly when a requested serializer provider is unavailable;
- serialize only the detached DTO returned by that participant.

### Package transport entry construction

For each successful participant capture:
- serialize the detached DTO in memory;
- calculate UTF-8 byte length;
- calculate integrity checksum over the exact serialized UTF-8 bytes;
- construct one `SavePayloadEntry`;
- construct one matching `SavePayloadInventoryEntry`;
- preserve participant ID;
- preserve participant schema version;
- preserve serializer provider ID;
- project Required/Optional into the package transport required flag;
- leave byte-provider reference empty for inline JSON/text payload;
- keep flags at the bounded default unless explicitly required by existing authority.

### Batch result

Implement a bounded immutable result containing:
- ordered participant entries;
- ordered matching inventory entries;
- structured terminal status;
- failing participant identity/diagnostic context when applicable;
- total participant payload byte count if useful and bounded.

M3-02 uses **all-or-nothing batch construction**. Any capture/type/serializer/integrity failure aborts the batch. No quietly partial save candidate is returned.

## 3. Open-ended participant rule

Chronicle must not introduce a central participant catalog to perform capture.

A participant added years later must flow through:

```text
ISaveParticipant
→ registry
→ same capture coordinator
→ same serializer/integrity seams
→ package transport entry
```

without a Chronicle core source edit.

## 4. Explicitly out of scope

Do not implement:
- writing participant `SavePayloadEntry` records into candidate generation files;
- joining the capture batch to `SaveGenerationPublicationCoordinator`;
- production `SaveAsync`;
- save admission/permission/coalescing;
- autosave;
- participant apply;
- `PreparedSaveLoad`;
- `LoadAndApplyAsync`;
- unknown-payload storage/carry-forward/prune;
- participant migrations;
- package-document migrations;
- slot catalog/policy;
- recovery/retention;
- peer-package bridges;
- Chronicle-owned/project-wide DDOL.

## 5. Failure invariants

M3-02 tests must prove:
- participant capture failure → batch failure, no partial successful batch exposed;
- successful capture returning null → batch failure;
- detached state incompatible with declared runtime DTO type → batch failure;
- missing serializer provider → batch failure;
- serializer failure → batch failure;
- integrity failure → batch failure;
- later participant failure does not convert earlier captures into a publishable partial batch;
- deterministic entry order matches canonical participant-ID order;
- payload entry and inventory entry metadata agree;
- checksums cover exact serialized UTF-8 payload bytes;
- registry registration remains open-ended;
- no generation/head/storage mutation occurs.

## 6. Executed focused proof

- trusted participant runtime DTO type declaration — **Pass**;
- runtime-type Unity JSON DTO round trip — **Pass**;
- save transport contains no CLR/assembly-qualified type metadata — **Pass**;
- deterministic canonical participant capture order — **Pass**;
- explicit serializer resolution — **Pass**;
- default Unity JSON serializer resolution — **Pass**;
- exact UTF-8 participant payload byte-length proof — **Pass**;
- exact SHA-256 participant payload checksum proof — **Pass**;
- payload/inventory metadata agreement — **Pass**;
- Required/Optional transport projection — **Pass**;
- participant capture failure aborts whole batch — **Pass**;
- null successful capture aborts whole batch — **Pass**;
- detached-state type mismatch aborts whole batch — **Pass**;
- untyped participant aborts detached capture — **Pass**;
- missing serializer provider aborts whole batch — **Pass**;
- serializer without runtime-Type capability aborts whole batch — **Pass**;
- serializer failure aborts whole batch — **Pass**;
- integrity failure aborts whole batch — **Pass**;
- future/unanticipated participant uses the same capture pipeline — **Pass**;
- batch entry access is defensively copied — **Pass**;
- capture coordinator performs no filesystem/publication mutation — **Pass**;
- all prior 147 Chronicle tests remain green — **Pass**.

Final focused Unity gate: **171 / 171 passed, 0 failed**.

## 7. Stop point

**Reached.** Chronicle can transform the active participant registry into one fully validated, all-or-nothing in-memory participant transport batch.

Implementation commit: `e34d6d7`.

Final focused Chronicle Editor gate: **171 / 171 passed, 0 failed**.

The capture batch still performs no storage mutation and no generation/head publication.

Next bounded checkpoint:

`ESV-M3-03 — Chronicle Participant-Backed Generation Publication and Head-Last Integration Foundation`

M3-03 may feed a successful captured participant batch into the already-proven M2 immutable-generation transaction, publish the participant-bearing payload/manifest candidate, verify it, publish the generation, and update `head.json` last.

M3-03 must remain a bounded technical integration seam. It does not yet authorize production `SaveAsync`, save admission/coalescing/cancellation policy, unknown-payload carry-forward, load/apply, migrations, slot catalog/policy, recovery, retention, or autosave.
