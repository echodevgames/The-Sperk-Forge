# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.15.0
**Completed checkpoint:** ESV-M3-08 — Prepared-Load Handle Lifecycle and Session Ownership Foundation
**Current checkpoint:** ESV-M3-09 — Deterministic Participant Apply and Missing-Payload Policy Foundation
**Status:** ESV-M3-08 complete; ESV-M3-09 active / authorized

## ESV-M3-08 closeout

Implementation commit: `798d38d`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **332 / 332 passed, 0 failed**;
- prior **294 / 294** Chronicle regression floor remains green;
- 38 new prepared-load tests passed;
- `PreparedSaveLoad` is public, opaque, sealed, and disposable;
- exact source slot/generation provenance is bound before admission;
- prepared DTOs remain package-internal;
- opaque unknown payloads remain package-internal and defensively isolated;
- owner token/session epoch prevents cross-owner and stale-token access;
- expiry/disposal/session invalidation deterministically release capacity;
- count and aggregate source-byte limits are bounded;
- participant `Capture` and `Apply` remain unused;
- storage/publication and scene/DDOL authority remain absent.

## Approved M3-09 contract decision

Add optional:

`ISaveDefaultableParticipant.InitializeDefault()`

The base `ISaveParticipant` contract remains unchanged.

Missing-payload behavior:
- `InitializeDefault` requires the optional capability;
- `Ignore` skips and reports;
- `Fail` blocks during complete preflight;
- `Apply(null)` is never used as a hidden default-initialization signal.

## Active ESV-M3-09 boundary

Authorized:
- complete apply preflight before mutation;
- deterministic current-registration apply planning;
- prepared-state `Apply(detachedState)`;
- optional explicit `InitializeDefault()`;
- missing-payload `Ignore` / `Fail` semantics;
- current registration ownership revalidation;
- structured ordered apply report;
- partial failure truth without rollback fiction;
- preflight failure leaves handle live;
- execution consumes handle;
- zero source-save mutation;
- unknown opaque payload non-use.

Still absent:
- rollback/compensation contract;
- production `ApplyPreparedLoadAsync` admission/cancellation;
- production `PrepareLoadAsync`;
- convenience `LoadAndApplyAsync`;
- scene travel/Passage integration;
- document migration;
- slots/recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.
