# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.13.0
**Completed checkpoint:** ESV-M3-06 — Current-Version Participant Payload Preparation, Trusted Runtime-Type Deserialization, and Prepared-Participant Batch Foundation
**Current checkpoint:** ESV-M3-07 — Participant Migration Contracts, Duplicate-Safe Registry, Contiguous-Chain Execution, and Migrated Payload Preparation Foundation
**Status:** ESV-M3-06 complete; ESV-M3-07 active / authorized

## ESV-M3-06 closeout

Implementation commit: `050bfa0`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **261 / 261 passed, 0 failed**;
- prior **243 / 243** Chronicle regression floor remains green;
- 18 new focused preparation tests passed;
- validated participant snapshots retain source provenance and defensive-copy semantics;
- known canonical/alias ownership resolves through the live registry;
- trusted detached DTO `Type` comes only from live registration;
- current-schema payloads deserialize through already-registered runtime-Type serializers;
- older schemas fail migration-required;
- newer schemas fail unsupported-newer;
- unknown payloads bypass serializer lookup;
- preparation invokes neither `Capture` nor `Apply`;
- preparation exposes no partial batch on failure.

## Active ESV-M3-07 boundary

Authorized:
- stable participant migration-step IDs;
- explicit participant migration-step contracts;
- current-canonical-participant + from-version migration edge authority;
- duplicate-safe migration registration/leases;
- deterministic registry snapshots;
- exact contiguous one-version-at-a-time chain planning;
- bounded in-memory migration execution;
- migration output serializer-ID/payload validation;
- ordered stable migration provenance without payload contents;
- older supported known participant payload integration into M3-06 preparation;
- final current-version deserialization through live trusted DTO `Type`;
- no source-generation rewrite;
- no participant `Capture`;
- no participant `Apply`;
- unknown payloads remain opaque.

Still absent:
- document migrations;
- `PreparedSaveLoad` lifecycle;
- participant apply/default/rollback orchestration;
- production operation admission;
- slots/recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.
