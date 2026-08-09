# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.8.0
**Completed checkpoint:** ESV-M3-01 — Participant Contracts, Descriptor Validation, and Duplicate-Safe Registry Foundation
**Current checkpoint:** ESV-M3-02 — Detached Participant Capture, Runtime Type Routing, and Payload-Entry Construction Foundation
**Status:** ESV-M3-01 complete; ESV-M3-02 active / authorized

## ESV-M3-01 closeout

Implementation commit: `b3b5f9f`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **147 / 147 passed, 0 failed**;
- all prior 102 Chronicle regressions remain green;
- stable canonical participant IDs;
- Required / Optional criticality;
- InitializeDefault / Ignore / Fail missing-payload policy;
- validated participant descriptors with bounded aliases;
- open-ended public `ISaveParticipant`;
- duplicate-safe deterministic registry;
- canonical/alias collision rejection;
- idempotent registration disposal;
- stale-handle ownership-token protection;
- immutable registry snapshots;
- no registry storage/publication behavior;
- no registry capture/apply invocation;
- future participant registration proven without Chronicle core edits.

## Open-ended seat invariant

Chronicle does **not** contain a hardcoded participant catalog.

A new system introduced after Chronicle ships may provide an `ISaveParticipant` implementation and register through the same public contract as every existing participant. Chronicle core must not require source edits merely because a new persistence participant exists.

## Active ESV-M3-02 boundary

Authorized:
- participant-declared runtime detached DTO type;
- type-aware serializer routing using the live registered participant, never save-file CLR type names;
- deterministic registry-snapshot capture order;
- explicit participant `Capture()` orchestration;
- detached-state type/null validation;
- serializer resolution by stable provider ID;
- in-memory serialized participant payload;
- `SavePayloadEntry` construction;
- `SavePayloadInventoryEntry` construction;
- UTF-8 byte length and per-entry integrity checksum;
- all-or-nothing capture-batch result;
- focused capture/type/serialization/integrity tests.

Still absent:
- candidate/final generation writes containing participant payloads;
- production `SaveAsync`;
- participant apply;
- prepared loads;
- unknown-payload carry-forward;
- migrations;
- slot catalog/policy;
- recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

M3-02 turns registered participants into verified in-memory transport entries only. The M2 durable transaction remains physically untouched until a later checkpoint deliberately joins the two halves.
