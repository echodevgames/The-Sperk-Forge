---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-10
---

# ESV-M3-09 — Chronicle Deterministic Participant Apply and Missing-Payload Policy Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M3-09
**Milestone:** M3 — Participants and Loading
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.15.0
**Prior checkpoint:** ESV-M3-08 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **332 / 332**

## 1. Intent

Activate deterministic application of one live `PreparedSaveLoad` to the **currently registered** participant set without smuggling scene authority, disk mutation, hidden default semantics, or imaginary rollback guarantees into Chronicle.

M3-09 is the first checkpoint allowed to call participant gameplay mutation APIs.

```text
live PreparedSaveLoad
        ↓
complete apply preflight
        ↓
deterministic current-registration apply plan
        ↓
prepared payload exists?
   yes ───────────────→ participant.Apply(detachedState)
   no
        ↓
 MissingPayloadPolicy
   InitializeDefault ─→ ISaveDefaultableParticipant.InitializeDefault()
   Ignore ────────────→ record skipped/ignored
   Fail ──────────────→ preflight failure before mutation
        ↓
structured participant apply report
        ↓
consume handle once execution begins
```

The source immutable generation and opaque unknown payload bytes are never rewritten or interpreted by this checkpoint.

## 2. Approved contract decision

Jesse approved an **additive optional capability** instead of changing `ISaveParticipant`:

```csharp
public interface ISaveDefaultableParticipant
{
    SaveParticipantApplyResult InitializeDefault();
}
```

Rules:
- the base `ISaveParticipant` contract remains unchanged;
- `Apply(null)` is **not** a default-initialization signal;
- `SaveMissingPayloadPolicy.InitializeDefault` requires the current participant to implement `ISaveDefaultableParticipant`;
- missing capability is a preflight failure before any participant mutation;
- `SaveMissingPayloadPolicy.Ignore` records a deterministic skip;
- `SaveMissingPayloadPolicy.Fail` blocks the whole apply during preflight.

This preserves existing participant implementations that never use default initialization.

## 3. Authorized implementation scope

### Additive optional default capability

Add public `ISaveDefaultableParticipant` under the participant contracts area.

It exposes only:

`SaveParticipantApplyResult InitializeDefault();`

Chronicle owns no participant-specific default DTO construction.

### Complete apply preflight

Before the first participant mutation callback, validate the entire apply request.

At minimum:
- handle is non-null, live, unexpired, and owned by the exact `SavePreparedLoadStore`;
- package-internal prepared participant batch can still be retrieved from the handle owner;
- current participant registry snapshot is valid and deterministic;
- every prepared participant entry resolves to exactly one current canonical registration;
- a prepared participant that no longer has a current owner blocks apply;
- current canonical ID agrees with the prepared entry;
- current participant schema/runtime detached-state authority remains compatible with the prepared entry;
- no duplicate prepared canonical participant exists;
- every currently registered participant that has no prepared payload is classified by its declared `SaveMissingPayloadPolicy`;
- `Fail` blocks before mutation;
- `InitializeDefault` requires `ISaveDefaultableParticipant` before mutation;
- `Ignore` is represented explicitly in the plan;
- unknown opaque payloads are not apply actions;
- all apply actions are fully planned before execution.

No participant `Apply` or `InitializeDefault` callback may occur during planning.

### Deterministic apply plan

Build one immutable in-memory apply plan in deterministic canonical participant order.

Each action is exactly one of:
- `ApplyPreparedState`;
- `InitializeDefault`;
- `Ignore`.

The plan may retain package-internal participant registration ownership tokens/references needed for stale-owner revalidation, but no payload body or detached DTO is exposed through public result/report surfaces.

### Registration ownership revalidation

Participant registration can legitimately change across the prepare → scene coordination → apply interval.

Therefore:
- preflight resolves against the **current** registry;
- before executing the first callback, all planned participant registrations are revalidated;
- immediately before each mutating callback, that participant's registration ownership is revalidated;
- replacement/unregistration fails closed;
- if ownership changes before any callback, the handle remains live for retry;
- if ownership changes after mutation has begun, execution stops and the handle is consumed.

Chronicle must never call a stale participant owner merely because it existed during preparation.

### Missing-payload execution

When no prepared payload exists for a currently registered participant:

`Ignore`
- invoke no participant callback;
- report deterministic ignored/skipped outcome.

`Fail`
- block during complete preflight;
- invoke no participant callback anywhere in the apply operation.

`InitializeDefault`
- require `ISaveDefaultableParticipant` during preflight;
- invoke `InitializeDefault()` during deterministic execution;
- use the returned `SaveParticipantApplyResult` as the authoritative participant outcome.

### Prepared payload execution

For a participant with prepared detached state:
- invoke `ISaveParticipant.Apply(detachedState)`;
- use `SaveParticipantApplyResult` for structured success/failure;
- convert thrown participant exceptions to a bounded structured failure;
- do not expose the detached object in reports.

### Handle consumption

A pure preflight rejection that invokes **zero** participant/default callbacks leaves the prepared handle live, subject to its normal expiry/owner lifecycle, so the project may repair registration and retry.

Once deterministic execution begins:
- the apply attempt consumes the prepared handle at terminal completion;
- successful apply consumes it;
- participant-returned failure consumes it;
- participant/default callback exception consumes it;
- registry ownership failure discovered after any prior mutating callback consumes it.

Reason: Chronicle cannot prove arbitrary participant mutation is reversible or idempotent.

M3-09 may add a terminal `Consumed` prepared-load state or equivalent package-owned terminal lifecycle representation.

### Structured apply report

Add stable structured result/report contracts sufficient to communicate:
- overall success/failure/preflight rejection;
- source slot/generation identity;
- whether participant mutation began;
- whether the handle was consumed;
- ordered participant outcomes;
- canonical participant ID;
- planned action kind;
- outcome such as applied/default-initialized/ignored/failed/not-attempted;
- participant diagnostic code/message where available;
- the first terminal failure.

Reports must not contain:
- raw serialized payload bodies;
- unknown opaque payload bodies;
- detached DTO object references;
- serializer/migration implementation objects.

### Rollback truth

M3-09 does **not** promise transactional rollback of arbitrary participant gameplay mutations.

Rules:
- complete preflight eliminates avoidable failures before mutation;
- execution stops at the first terminal participant failure;
- the report identifies all completed, ignored, failed, and not-attempted actions;
- already-completed participant mutations are not silently reversed by Chronicle;
- no automatic compensation callback is invented;
- source save data remains unchanged.

Participant-specific stage/commit or rollback capability may be designed later as a separate explicit contract if required.

## 4. Explicitly out of scope

Do not implement:
- production `ApplyPreparedLoadAsync` operation admission/cancellation;
- production `PrepareLoadAsync`;
- `LoadAndApplyAsync`;
- scene travel or Passage integration;
- automatic participant spawning/registration;
- document migration;
- automatic migration recommit;
- participant rollback/compensation contract;
- save operation coalescing/busy ownership;
- slot catalog/active-slot policy;
- recovery candidate selection;
- retention cleanup;
- autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL/service locator.

## 5. Failure and safety invariants

M3-09 tests must prove:
- valid live handle + matching current participants produces deterministic apply plan;
- planning invokes zero participant callbacks;
- missing prepared owner rejects before mutation;
- stale/replaced registration rejects before mutation when detected in preflight;
- prepared detached-state type mismatch rejects before mutation;
- duplicate prepared canonical ID rejects before mutation;
- missing payload + `Fail` rejects before any callback;
- missing payload + `InitializeDefault` without optional capability rejects before any callback;
- missing payload + `Ignore` invokes no callback and records ignored;
- missing payload + `InitializeDefault` invokes exactly one `InitializeDefault`;
- default initialization never calls `Apply(null)`;
- prepared payload invokes exactly one `Apply(detachedState)`;
- current registry ordering yields deterministic action order;
- unknown opaque payloads produce zero apply/default callbacks;
- thrown `Apply` exception becomes structured failure;
- thrown `InitializeDefault` exception becomes structured failure;
- participant-returned apply failure stops later mutating actions;
- participant-returned default failure stops later mutating actions;
- completed/failed/not-attempted actions are reported accurately;
- pure preflight failure leaves handle live;
- successful execution consumes handle;
- callback failure consumes handle;
- ownership failure after prior mutation consumes handle;
- consumed handle rejects replay;
- source generation/head/payload files are not mutated;
- no scene/DDOL API is required;
- all prior **332 / 332** Chronicle tests remain green.

## 6. Proposed focused proof

- optional default capability public contract;
- base `ISaveParticipant` remains unchanged;
- apply plan determinism;
- zero-callback preflight;
- prepared participant owner missing;
- stale registration owner;
- prepared type mismatch;
- duplicate prepared canonical ID;
- missing policy `Fail`;
- missing policy `Ignore`;
- missing policy `InitializeDefault` with/without capability;
- proof that defaulting never routes through `Apply(null)`;
- deterministic prepared-state apply;
- apply result failure;
- default result failure;
- apply/default exception conversion;
- partial report ordering;
- not-attempted tail reporting;
- preflight retry leaves handle live;
- execution consumes handle;
- replay rejection;
- unknown payload non-use;
- zero storage mutation;
- zero scene/DDOL authority;
- prior **332 / 332** regression floor remains green.

Executed totals are recorded from Unity, not predicted.

## 7. Stop point

Stop when Chronicle can take one live prepared handle, prove a complete deterministic current-registration apply plan, honor all three missing-payload policies without hidden null semantics, invoke prepared/default participant mutation in deterministic order, and return an accurate partial report while consuming the handle whenever mutation execution begins.

Do not add production async operation admission yet.

Do not add convenience loading or scene travel.

Do not add rollback fiction.

The next bounded checkpoint should wrap the proven prepare/apply foundations in production operation admission/main-thread orchestration and public service behavior while preserving project-owned scene flow and package-local lifetime authority.
