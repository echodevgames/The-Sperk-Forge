---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M3-01 — Chronicle Participant Contracts, Descriptor Validation, and Duplicate-Safe Registry Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-01
**Milestone:** M3 — Participants and Loading
**Status:** **COMPLETE**
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

## 6. Executed focused proof

- canonical participant-ID acceptance — **Pass**;
- malformed/noncanonical/path-like ID rejection — **Pass**;
- descriptor positive schema-version validation — **Pass**;
- criticality and missing-payload-policy preservation — **Pass**;
- bounded alias validation — **Pass**;
- unique registration succeeds — **Pass**;
- duplicate canonical registration rejected — **Pass**;
- canonical/alias collision matrix rejected — **Pass**;
- deterministic registry ordering — **Pass**;
- canonical and alias lookup resolve the same active participant — **Pass**;
- registration dispose/unregister — **Pass**;
- idempotent disposal — **Pass**;
- stale-handle ownership-token safety — **Pass**;
- registry snapshot immutability — **Pass**;
- registry never invokes participant capture/apply — **Pass**;
- registry performs zero durable storage/publication work — **Pass**;
- arbitrary future participant registration requires no Chronicle core edit or predefined catalog — **Pass**;
- all prior 102 Chronicle tests remain green — **Pass**.

Final focused Unity gate: **147 / 147 passed, 0 failed**.

## 7. Stop point

**Reached.** Chronicle can safely identify, validate, register, enumerate, resolve, and unregister open-ended participant authorities in runtime memory.

Implementation commit: `b3b5f9f`.

Final focused Chronicle Editor gate: **147 / 147 passed, 0 failed**.

The open-ended seat invariant is now proven: Chronicle contains no compile-time catalog of known participants. A future system can implement the same public participant contract and register without editing Chronicle core.

Next bounded checkpoint:

`ESV-M3-02 — Chronicle Detached Participant Capture, Runtime Type Routing, and Payload-Entry Construction Foundation`

M3-02 may invoke participant capture and convert detached DTOs into validated in-memory package transport entries. It must still stop before physical generation publication, production `SaveAsync`, loading/apply, unknown-payload carry-forward, or migration.
