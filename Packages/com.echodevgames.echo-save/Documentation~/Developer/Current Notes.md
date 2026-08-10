# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.11.0
**Completed checkpoint:** ESV-M3-04 — Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation
**Current checkpoint:** ESV-M3-05 — Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation
**Status:** ESV-M3-04 complete; ESV-M3-05 active / authorized

## ESV-M3-04 closeout

Implementation commit: `aa78e07`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **218 / 218 passed, 0 failed**;
- all prior 197 Chronicle regressions remain green;
- current head/current immutable generation can be read and fully validated without storage mutation;
- whole-document and per-entry integrity validation remain mandatory;
- canonical and alias participant identities are recognized;
- unclaimed entries are preserved as opaque `SaveUnknownPayloadStore` session data;
- unknown serialized payload text and transport metadata are preserved without interpretation;
- unknown store snapshots are deterministic defensive copies;
- bounded unknown entry count/aggregate bytes are enforced;
- failed reads/classification preserve the prior valid store atomically.

## Active ESV-M3-05 boundary

Authorized:
- add source slot/generation provenance to the unknown store/snapshot;
- preserve entries and provenance atomically;
- preflight target current head against the preserved source generation before publication mutation;
- reject stale snapshots;
- merge one successful fresh known capture batch with one valid opaque unknown snapshot;
- re-resolve preserved IDs against current canonical/alias registry ownership;
- fail closed on changed ownership or identity collisions;
- preserve unknown participant serialized payload UTF-8 bytes and transport metadata exactly;
- keep unknown payloads opaque and inert;
- create deterministic merged payload/inventory records;
- publish the merged batch through the M3-03 generation-first/head-last transaction;
- leave the prior unknown snapshot stale after successful head advance until a fresh current read refreshes it.

Still absent:
- silent drop/prune behavior;
- automatic collision winner;
- participant deserialization/migration/apply;
- prepared/convenience loads;
- production save admission/permission/busy/coalescing/cancellation;
- concurrent save-operation ownership;
- slot catalog/policy;
- recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

Carry-forward is fail-closed. Chronicle preserves data it does not understand and refuses to guess when ownership becomes ambiguous.
