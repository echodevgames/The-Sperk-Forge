# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.14.0
**Completed checkpoint:** ESV-M3-07 — Participant Migration Contracts, Duplicate-Safe Registry, Contiguous-Chain Execution, and Migrated Payload Preparation Foundation
**Current checkpoint:** ESV-M3-08 — Prepared-Load Handle Lifecycle and Session Ownership Foundation
**Status:** ESV-M3-07 complete; ESV-M3-08 active / authorized

## ESV-M3-07 closeout

Implementation commit: `d96936f`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **294 / 294 passed, 0 failed**;
- prior **261 / 261** Chronicle regression floor remains green;
- 33 new focused migration tests passed;
- migration steps own exact contiguous canonical participant/version edges;
- complete chains are proven before execution;
- migration is bounded and in memory only;
- every step validates exact next schema, serializer ID, and payload output;
- persisted aliases route through the current canonical owner;
- migration provenance records stable IDs/version edges without payload contents;
- successful old-schema payloads rejoin the M3-06 trusted DTO preparation path;
- unknown payloads never enter migration planning;
- participant `Capture` and `Apply` remain unused;
- source immutable generations remain untouched.

## Active ESV-M3-08 boundary

Authorized:
- public disposable `PreparedSaveLoad` handle;
- exact source slot/generation binding;
- package/session owner token or epoch;
- owner-isolated package-internal prepared-state access;
- opaque unknown-payload snapshot binding;
- idempotent disposal;
- deterministic expiry through an injected time seam;
- session/owner invalidate-all behavior;
- bounded live-handle count;
- bounded aggregate source transport-byte estimate;
- capacity release on dispose/expiry/invalidation;
- public safe metadata only;
- no public detached DTO/raw unknown payload exposure.

Still absent:
- participant `Apply`;
- missing-payload default execution;
- apply rollback orchestration;
- document migration;
- production `PrepareLoadAsync` admission/cancellation;
- scene travel/Passage integration;
- slots/recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.
