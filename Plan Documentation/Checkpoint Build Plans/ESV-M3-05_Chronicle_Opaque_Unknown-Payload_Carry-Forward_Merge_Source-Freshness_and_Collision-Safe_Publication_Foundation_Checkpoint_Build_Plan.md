---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M3-05 — Chronicle Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-05
**Milestone:** M3 — Participants and Loading
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.11.0
**Prior checkpoint:** ESV-M3-04 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **218 / 218**

## 1. Intent

Prove that Chronicle can create the next immutable participant-bearing generation **without deleting durable payloads owned by systems that are absent from the current runtime**.

```text
current committed generation
      ↓
M3-04 read/classify
      ↓
opaque unknown snapshot
      +
fresh M3-02 known participant capture
      ↓
M3-05 source-freshness + identity ownership checks
      ↓
collision-safe deterministic merge
      ↓
M3-03 immutable generation transaction
      ↓
HEAD LAST
```

Unknown payload contents remain opaque and inert throughout the operation.

## 2. Source-provenance requirement

M3-04 proved safe unknown entry storage, but the current `SaveUnknownPayloadStore` / `SaveUnknownPayloadSnapshot` does not retain the source slot/generation that produced the preserved entries.

M3-05 must add bounded provenance so a carry-forward snapshot is tied to:
- one valid `SaveSlotId`;
- one valid source `SaveGenerationId`;
- one completed successful M3-04 classification.

A successful current-generation read must atomically replace:
- unknown entries;
- aggregate unknown bytes;
- source slot ID;
- source generation ID.

A failed read/classification must preserve the entire previous store **including its provenance**.

A manual clear/reset must clear both entries and provenance.

An empty unknown set produced by a successful current-generation read still carries source provenance. Zero unknown entries does not mean “no source generation.”

## 3. Source-freshness preflight

Before any candidate storage mutation, carry-forward publication must prove:
- the preserved snapshot has valid source provenance;
- the target slot equals the preserved source slot;
- the target slot still has a valid current head;
- the current head still selects the preserved source generation.

If current head no longer equals the preserved source generation:
- fail with a structured stale-source result;
- perform **zero** candidate/generation/head mutation;
- do not guess which generation's unknown payloads should survive;
- require current-generation read/classification to refresh the store before another carry-forward attempt.

This is a bounded freshness preflight, not authorization for concurrent save writers. Production operation admission/serialization remains later work.

## 4. Merge inputs

The merge foundation accepts:
- one successful fresh `SaveParticipantCaptureBatchResult`;
- one valid `SaveUnknownPayloadSnapshot` with source provenance;
- the active `SaveParticipantRegistry`;
- the target technical slot.

Fresh known captures remain authoritative only for identities the live registry currently owns.

Unknown entries remain authoritative only while no active participant currently claims their persisted identity.

## 5. Collision and ownership rules

Before merge, re-resolve every preserved unknown participant ID against the **current** participant registry.

### Stale ownership claim

If a preserved unknown ID now resolves through:
- a participant canonical ID; or
- one of that participant's approved aliases;

then the snapshot is stale with respect to current participant ownership.

M3-05 must:
- fail closed;
- report the colliding persisted ID and current canonical owner when available;
- perform zero publication mutation;
- keep the unknown store unchanged;
- require a fresh M3-04 re-read/classification or a later explicit migration/prune policy.

Do **not** silently drop the unknown entry just because a package has appeared.

### Fresh capture collision

If a preserved unknown persisted ID conflicts with any fresh captured participant identity, directly or through current canonical/alias ownership, fail closed.

There is no implicit “fresh wins” or “old wins” rule.

### Unknown-to-unknown collision

Duplicate preserved unknown participant IDs remain invalid and fail before publication.

### No canonical rewrite

An unclaimed unknown entry keeps its original persisted participant ID.

M3-05 does not rewrite an unknown alias or historical ID to some guessed canonical identity.

## 6. Carry-forward preservation rule

For every non-colliding unknown entry, carry forward exactly:
- `participantId`;
- `participantSchemaVersion`;
- `serializerId`;
- `required`;
- `serializedPayload`;
- `byteProviderReference`;
- `byteLength`;
- `checksum`;
- `flags`.

The UTF-8 bytes of `serializedPayload` must be identical before and after merge, and the entry checksum/byte-length metadata must remain unchanged.

The surrounding package `SavePayloadDocument` is a **new generation document** and will be serialized again with a new generation ID and a new whole-document checksum. “Byte-for-byte carry-forward” therefore applies to the preserved participant payload body and its transport metadata, not to the outer generation JSON file as a whole.

Unknown payloads must not:
- resolve a serializer;
- deserialize;
- activate CLR types;
- invoke participant code;
- migrate;
- be recomputed from gameplay state.

## 7. Deterministic merged batch

M3-05 may add a package-internal merged transport result that:
- contains payload entries plus matching inventory entries;
- defensively copies mutable transport records;
- validates each fresh and preserved entry before exposure;
- reconstructs unknown inventory metadata only from the preserved entry's existing transport fields;
- sorts the complete merged set by persisted participant ID using ordinal comparison;
- rejects duplicates and noncanonical technical IDs;
- reports fresh-known count, preserved-unknown count, and aggregate payload bytes.

No merged batch may be exposed on failure.

## 8. Publication integration

Add one bounded technical carry-forward publication path that:
1. performs source-provenance/freshness checks;
2. validates the fresh capture batch;
3. validates the preserved unknown snapshot;
4. rechecks current ownership/collisions;
5. builds the deterministic merged batch;
6. passes that complete merged transport through the proven M3-03 participant-entry validation and generation transaction;
7. writes candidate payload/manifest;
8. verifies the candidate;
9. publishes the immutable generation;
10. revalidates the published generation;
11. publishes `head.json` **last**.

The existing M2 empty path and M3-03 fresh-participant-only path must remain green.

## 9. Session-store behavior after publication

ESV-M3-05 must favor safety over convenience.

A successful carry-forward publication does **not** silently mutate or rebase the authoritative unknown store to the new generation.

After `head.json` advances, the existing preserved snapshot is intentionally stale by provenance and another carry-forward attempt must fail freshness preflight until the caller performs a new M3-04 current-generation read/classification.

A later production save-orchestration checkpoint may authorize a tighter read/capture/publish/store-refresh operation once save admission and operation serialization are defined.

## 10. Failure invariants

M3-05 must prove:
- missing snapshot provenance → zero storage mutation;
- snapshot slot mismatch → zero storage mutation;
- stale source generation → zero storage mutation;
- unknown ID now claimed canonically → zero storage mutation;
- unknown ID now claimed through alias → zero storage mutation;
- fresh/unknown identity collision → zero storage mutation;
- malformed preserved unknown entry → zero storage mutation;
- preserved unknown checksum/byte-length mismatch → zero storage mutation;
- merge duplicate → zero storage mutation;
- merge failure exposes no partial merged batch;
- candidate/generation/final-verification/head failures retain M3-03 known-good-head behavior;
- successful publication carries unknown serialized payload bytes unchanged;
- successful publication carries unknown metadata unchanged;
- successful publication includes fresh known captures and preserved unknown entries in deterministic order;
- successful head advance makes the old unknown snapshot stale until re-read;
- unknown payloads resolve no serializer and invoke no participant code;
- all prior **218 / 218** Chronicle tests remain green.

## 11. Explicitly out of scope

Do not implement:
- silent unknown-payload dropping;
- automatic “fresh wins” collision resolution;
- explicit prune plans;
- participant deserialization;
- participant migration;
- participant apply;
- `PreparedSaveLoad`;
- convenience load;
- production `SaveAsync` admission/permission/busy/coalescing/cancellation;
- concurrent save-operation ownership;
- autosave;
- slot catalog/policy or active-slot selection;
- recovery fallback;
- retention cleanup;
- peer-package bridges;
- Chronicle-owned/project-wide DDOL.

## 12. Executed focused proof

- source slot/generation provenance stored with unknown snapshots — **Pass**;
- successful current-generation read atomically refreshes unknown entries plus provenance — **Pass**;
- failed read preserves prior unknown entries plus provenance — **Pass**;
- manual clear resets entries plus provenance — **Pass**;
- empty unknown snapshot still retains source provenance — **Pass**;
- stale-source generation rejected before publication mutation — **Pass**;
- target-slot/source-slot mismatch rejected before publication mutation — **Pass**;
- current canonical ownership collision rejected — **Pass**;
- current alias ownership collision rejected — **Pass**;
- fresh-known/unknown identity collision rejected — **Pass**;
- malformed preserved unknown entry rejected — **Pass**;
- preserved unknown byte-length/checksum mismatch rejected — **Pass**;
- merge duplicates rejected without partial batch exposure — **Pass**;
- unknown serialized payload UTF-8 bytes preserved exactly — **Pass**;
- unknown transport metadata preserved exactly — **Pass**;
- deterministic fresh-known + preserved-unknown ordering — **Pass**;
- unknown payloads resolve no serializer and invoke no participant code — **Pass**;
- candidate/generation/final-verification/head failure behavior preserves the previous known-good head — **Pass**;
- successful merged publication advances `head.json` last — **Pass**;
- old unknown snapshot becomes stale after successful head advance — **Pass**;
- M2 empty and M3-03 fresh-participant-only publication paths remain green — **Pass**;
- all prior **218 / 218** Chronicle regressions remain green — **Pass**.

Final focused Unity gate: **243 / 243 passed, 0 failed**.

Implementation commit: `af28c96`.

## 13. Stop point

**Reached.** Chronicle can carry one source-fresh opaque unknown snapshot beside fresh known captures into the next immutable generation with:
- source freshness proven before mutation;
- no ambiguous canonical/alias ownership;
- unknown participant payload bytes/metadata preserved exactly;
- candidate/final verification retained;
- `head.json` published last;
- the previous known-good generation preserved on failure.

The carry-forward path remains a bounded technical seam and is not production `SaveAsync`.

Next bounded checkpoint:

`ESV-M3-06 — Chronicle Current-Version Participant Payload Preparation, Trusted Runtime-Type Deserialization, and Prepared-Participant Batch Foundation`

M3-06 may prepare **current-schema** known participant payloads into detached runtime DTOs using live registration type authority and already-registered serializer providers. It must remain side-effect-free with respect to participant runtime state and storage.

Participant migrations, `PreparedSaveLoad` handle lifecycle, participant apply, production operation admission, slots, recovery, retention, and autosave remain later bounded work.
