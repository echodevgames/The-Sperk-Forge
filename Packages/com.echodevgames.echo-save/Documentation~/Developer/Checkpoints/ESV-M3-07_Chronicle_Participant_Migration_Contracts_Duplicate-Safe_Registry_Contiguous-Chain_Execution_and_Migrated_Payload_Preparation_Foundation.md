---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
  - sfgss/closeout
status: complete
updated: 2026-08-09
---

# ESV-M3-07 — Chronicle Participant Migration Contracts, Duplicate-Safe Registry, Contiguous-Chain Execution, and Migrated Payload Preparation Foundation

**Package:** The Chronicle (`EchoSave`)
**Implementation commit:** `d96936f`
**Unity:** 6000.3.8f1
**Result:** **Complete**

## Outcome

ESV-M3-07 proves Chronicle can migrate explicitly supported older known participant payloads through one complete deterministic contiguous in-memory schema chain and then reuse the M3-06 trusted current-version DTO preparation path.

Delivered:
- stable `SaveParticipantMigrationId`;
- public explicit participant migration-step contract;
- structured migration input/output contracts;
- duplicate-safe runtime migration registration and ownership leases;
- canonical-participant/from-version edge authority;
- deterministic migration registry snapshots;
- exact one-version-at-a-time contiguous chain planning;
- positive migration-depth bound;
- missing-edge failure before migration execution;
- in-memory migration execution with registry ownership recheck;
- exact target-version, serializer-ID, and payload-output validation;
- ordered stable migration provenance without payload contents;
- persisted alias → current canonical migration routing;
- migrated-payload integration into M3-06 trusted DTO preparation;
- all-or-nothing preparation after migration;
- unknown payload migration non-use;
- zero participant `Capture`;
- zero participant `Apply`;
- zero source-generation rewrite.

## Evidence

Final focused gate:

`EchoDevGames.EchoSave.Tests.Editor — 294 / 294 passed, 0 failed`

The complete prior **261 / 261** Chronicle regression floor remained green.

M3-07 added **33** focused tests:
- migrated preparation: 7;
- migration executor: 7;
- migration ID: 8;
- migration registry: 11.

## Boundary preserved

ESV-M3-07 does not activate:
- `PreparedSaveLoad`;
- participant apply/default/rollback;
- document migration;
- automatic migration recommit;
- production operation admission/coalescing/cancellation;
- slots;
- recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

## Closeout decision

**ESV-M3-07 is complete.**

Next:

`ESV-M3-08 — Chronicle Prepared-Load Handle Lifecycle and Session Ownership Foundation`
