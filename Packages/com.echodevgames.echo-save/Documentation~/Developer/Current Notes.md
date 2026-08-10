# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.12.0
**Completed checkpoint:** ESV-M3-05 — Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation
**Current checkpoint:** ESV-M3-06 — Current-Version Participant Payload Preparation, Trusted Runtime-Type Deserialization, and Prepared-Participant Batch Foundation
**Status:** ESV-M3-05 complete; ESV-M3-06 active / authorized

## ESV-M3-05 closeout

Implementation commit: `af28c96`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **243 / 243 passed, 0 failed**;
- all prior **218 / 218** Chronicle regressions remain green;
- source provenance, stale-source rejection, canonical/alias collision fail-closed behavior, exact opaque carry-forward, deterministic merge, and head-last publication are proven;
- unknown payloads remain opaque and invoke no serializer or participant code.

## Active ESV-M3-06 boundary

Authorized:
- fully validated current-generation participant snapshot for preparation;
- live canonical/alias owner resolution;
- trusted `ISaveTypedParticipant.DetachedStateType`;
- current-schema-only preparation;
- older schema → migration-required;
- newer schema → unsupported-newer;
- already-registered runtime-Type serializer resolution;
- deterministic all-or-nothing prepared participant batch;
- no participant `Capture`;
- no participant `Apply`;
- zero storage mutation;
- unknown payloads remain opaque.

Still absent:
- participant migration chains;
- `PreparedSaveLoad` lifecycle;
- participant apply and missing-payload default execution;
- production operation admission;
- slots/recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.
