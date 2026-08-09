---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-09
---

# ESV-M3-01 — Chronicle Participant Contracts, Descriptor Validation, and Duplicate-Safe Registry Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-01
**Milestone:** M3 — Participants and Loading
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.7.0
**Prior checkpoint:** ESV-M2-04 — **Complete**
**Prior milestone:** M2 — **Complete for bounded document/storage-core path**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **102 / 102**

## 1. Intent

Begin Chronicle's participant/loading milestone at the smallest authority surface: define who can participate in persistence and prove registration safety before any participant is captured, serialized, written, loaded, migrated, or applied.

```text
project / package adapter
        ↓
ISaveParticipant
        ↓
validated descriptor
        ↓
SaveParticipantRegistry
        ↓
deterministic runtime registration snapshot

NO FILE I/O
NO SAVE CAPTURE
NO LOAD APPLY
```

## 2. Authorized implementation scope

### Stable participant identity

Implement:
- `SaveParticipantId`;
- developer-authored lowercase reverse-domain / namespace-like stable IDs;
- stable validation that rejects empty values, traversal/path separators, control characters, reserved/invalid forms, and noncanonical casing;
- equality/string behavior independent from Unity object identity.

### Participant descriptor

Implement:
- `SaveParticipantCriticality` with `Required` and `Optional`;
- `SaveMissingPayloadPolicy` with `InitializeDefault`, `Ignore`, and `Fail`;
- `SaveParticipantDescriptor`;
- current participant schema version;
- serializer provider ID;
- bounded prior-ID aliases;
- descriptor validation.

Rules:
- schema version must be positive;
- canonical ID and aliases must each be valid;
- aliases cannot contain the canonical ID;
- aliases are unique within a descriptor;
- project gameplay DTO type/schema meaning remains outside the descriptor.

### Participant contract

Define `ISaveParticipant` according to the approved participant model:
- descriptor/identity surface;
- detached capture-facing method/result contract;
- apply-facing method/result contract;
- no requirement that Chronicle know the participant's gameplay type;
- no reflection/type-name activation from save files;
- no implicit Unity object graph serialization.

M3-01 defines these method contracts only. Production capture/apply orchestration is later.

### Registry

Implement:
- `SaveParticipantRegistry`;
- `SaveParticipantRegistration`;
- structured registration result/status;
- stable participant diagnostics;
- duplicate canonical-ID rejection;
- alias/canonical collision rejection across active registrations;
- alias/alias collision rejection across active registrations;
- deterministic ordering by canonical participant ID;
- registration lookup by canonical ID and accepted alias where appropriate;
- bounded immutable registry snapshot/descriptor list;
- idempotent registration disposal;
- unregister only the registration that owns the active registry claim.

## 3. Registration lifetime rules

1. Registry membership is application-session runtime state.
2. Registration itself performs no durable I/O.
3. Disposing a registration releases only that participant's registry membership.
4. Re-disposal is a no-op.
5. A stale registration handle cannot unregister a later participant that reused the same ID after the original was removed.
6. Duplicate/collision rejection occurs before registry mutation.
7. Registry order is canonical ID order, not registration order.
8. Aliases resolve identity but do not create second participant entries.
9. Registry snapshot callers cannot mutate registry internals.
10. Registry behavior remains independent from scenes and DDOL composition.

## 4. Explicitly out of scope

Do not implement participant capture orchestration, DTO serialization routing, `SaveAsync`, physical participant payload publication, participant apply, prepared loads, convenience loads, unknown-payload preservation, migrations, slot catalog/policy, recovery/retention, autosave, peer bridges, or Chronicle-owned DDOL.

## 5. Failure invariants

M3-01 tests must prove:
- invalid participant ID → no registry mutation;
- invalid descriptor/schema/policy → no registry mutation;
- duplicate canonical ID → later registration rejected;
- canonical ID colliding with an active alias → rejected;
- alias colliding with an active canonical ID → rejected;
- alias colliding with another active alias → rejected;
- disposing registration removes exactly its own claim;
- disposing twice is safe;
- stale handle cannot remove a replacement registration;
- deterministic descriptor order is independent from registration order;
- aliases never create duplicate ordered entries;
- no registry operation touches storage or publishes a generation.

## 6. Proposed focused proof

- canonical participant-ID acceptance;
- malformed/noncanonical/path-like ID rejection;
- descriptor positive schema-version validation;
- criticality/payload-policy preservation;
- bounded alias validation;
- unique registration succeeds;
- duplicate canonical registration rejected;
- canonical/alias collision matrix rejected;
- deterministic ordering;
- canonical and alias lookup resolve the same active participant;
- registration dispose/unregister;
- idempotent disposal;
- stale-handle generation/token safety;
- registry snapshot immutability;
- zero storage/publication side effects;
- all prior **102 / 102** Chronicle tests remain green.

Executed totals are recorded from Unity, not predicted.

## 7. Stop point

Stop when Chronicle can safely identify, validate, register, enumerate, resolve, and unregister participant authorities in runtime memory.

Do not wire those participants into physical save publication or loading during ESV-M3-01.

The next M3 checkpoint may then begin detached participant capture and package-owned payload-entry construction while preserving the already-proven generation/head transaction.
