---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M2-04 — Chronicle Immutable Generation Publication and Head-Last Commit Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-04
**Milestone:** M2 — Document / Storage Core
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.6.0
**Prior checkpoint:** ESV-M2-03 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **87 / 87**

## 1. Intent

Cross Chronicle's first physical durable-publication boundary without activating participant gameplay state or slot-management policy.

```text
build package-owned transport documents
        ↓
write candidate under incomplete/<generation-id>
        ↓
complete provider write / flush where supported
        ↓
read/verify candidate bytes + package documents
        ↓
publish immutable generations/<generation-id>
        ↓
publish/update head.json LAST
        ↓
return success
```

If any step before successful head publication fails, the previously authoritative head/generation remains authoritative and unchanged.

## 2. Authorized implementation scope

### Provider-neutral publication seam

Add only generic storage operations/capability advertisement required for safe publication:
- publish/move a new storage subtree or equivalent provider operation;
- publish a new small object;
- replace a small current object where supported;
- explicit capability information for atomic replace/move semantics.

The storage backend must remain ignorant of Chronicle's slot/generation/document meaning.

### Default local backend

Authorized local behavior:
- candidate storage beneath the validated Chronicle root;
- local file/directory move/publication primitives;
- small-head temp + publish/replace behavior;
- `File.Replace` or equivalent only when actually supported/applicable;
- documented fallback behavior when atomic replacement is unavailable;
- no claim of stronger guarantees than the runtime/platform primitive provides.

### Package publication coordinator

Authorized:
- construct candidate/final storage keys from validated `SaveSlotId` and `SaveGenerationId`;
- serialize an empty/transport-only `SavePayloadDocument`;
- compute payload bytes/checksum;
- build final `SaveManifest` bytes;
- write candidate payload and manifest completely;
- read back and verify candidate bytes/documents;
- publish the verified candidate as a new immutable generation;
- build/serialize/validate a `SaveHeadPointer`;
- publish/update the head **only after** generation publication succeeds;
- preserve the previous head/current generation until new head publication succeeds;
- leave failed/uncommitted candidate state non-current for later recovery/quarantine work;
- return structured publication results/diagnostics.

## 3. First-publication and update rules

1. First save may publish a new `head.json` when no head exists.
2. Later saves must not destroy/overwrite the current head before replacement is ready.
3. New final generation path must not already exist.
4. A committed generation is immutable after publication.
5. Head points only to an already-published and revalidated generation.
6. `previousGenerationId` records the prior valid current generation where available.
7. `updateSequence` advances monotonically from the previously read head.
8. Failed head publication returns failure even if the new generation was published; the old head remains authoritative.
9. An orphaned verified generation is not silently made current.
10. Retention cleanup never runs in this checkpoint.

## 4. Explicitly out of scope

Do not implement:
- slot catalog/cache;
- slot creation UX/policy/capacity;
- active-slot selection service;
- participant registry/capture/apply;
- project gameplay payload schemas;
- recovery candidate selection or automatic orphan adoption;
- quarantine policy;
- retention deletion;
- autosave/coalescing;
- prepared loads;
- migration chains;
- production save API orchestration;
- peer-package bridges;
- Chronicle-owned DDOL.

## 5. Failure invariants

M2-04 tests must prove:
- candidate payload write failure → no final generation, old head unchanged;
- candidate manifest write failure → no final generation, old head unchanged;
- candidate verification failure → no final generation, old head unchanged;
- final generation publication failure → old head unchanged;
- head serialization/validation failure → published generation may remain orphaned, old head unchanged;
- head publication/replacement failure → published generation may remain orphaned, old head unchanged;
- successful head publication → new generation becomes current;
- a published generation is never modified after final publication;
- an incomplete candidate is never treated as current;
- a duplicate generation ID is rejected without mutating the current head.

## 6. Executed focused proof

- provider capability advertisement is explicit and accurate — **Pass**;
- local first-head publication succeeds in sandbox — **Pass**;
- local second-generation publication updates head last — **Pass**;
- second head records previous generation and increments sequence — **Pass**;
- generated payload/manifest read back valid — **Pass**;
- candidate-to-final publication produces immutable final files — **Pass**;
- interruption/failure injection at each pre-head boundary preserves old head — **Pass**;
- failed head update preserves old head — **Pass**;
- newly published orphan generation is not current after failed head update — **Pass**;
- duplicate final generation rejected — **Pass**;
- no slot catalog/participant/recovery behavior was introduced — **Pass**;
- all prior 87 Chronicle tests remain green — **Pass**.

Final focused Unity gate: **102 / 102 passed, 0 failed**.

## 7. Stop point

**Reached.** The default local Chronicle backend now proves one real package-owned transport-generation transaction with generation-first/head-last publication and previous-known-good preservation.

Implementation commit: `01b7ad3`.

Final focused Chronicle Editor gate: **102 / 102 passed, 0 failed**.

Milestone consequence:

**M2 — Document / Storage Core is complete for the approved bounded implementation path.**

Next bounded checkpoint:

`ESV-M3-01 — Chronicle Participant Contracts, Descriptor Validation, and Duplicate-Safe Registry Foundation`

M3-01 begins the approved M3 participant/loading milestone. It establishes participant identity, descriptors, registration lifetime, deterministic ordering, and duplicate-safe registry behavior without yet wiring participant capture into `SaveAsync`, applying loaded state, preparing loads, migrations, or unknown-payload carry-forward.
