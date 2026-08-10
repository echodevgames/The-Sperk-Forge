---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
  - sfgss/closeout
status: complete
updated: 2026-08-09
---

# ESV-M3-04 — Chronicle Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Runtime version:** `0.1.0`
**Implementation commit:** `aa78e07`
**Unity:** 6000.3.8f1
**Result:** **Complete**

## Outcome

ESV-M3-04 established Chronicle's read-only current-generation inspection path and package-owned opaque unknown-payload session preservation.

Delivered:
- current head and immutable-generation resolution;
- complete package document and slot/generation identity validation;
- whole serialized payload integrity validation;
- per-entry inline byte-length/checksum validation;
- canonical and alias participant recognition;
- opaque unknown entry classification;
- deterministic defensive-copy unknown snapshots;
- bounded unknown count and aggregate bytes;
- atomic successful unknown-store replacement;
- prior-store preservation across failed read/classification;
- zero storage mutation.

## Evidence

Final focused gate:

`EchoDevGames.EchoSave.Tests.Editor — 218 / 218 passed, 0 failed`

The complete prior **197 / 197** Chronicle regression floor remained green.

## Opaque-data evidence

Unknown entries:
- retain participant ID/schema/serializer/required/payload/reference/length/checksum/flags;
- preserve serialized payload text exactly;
- resolve no serializer provider;
- activate no CLR type;
- invoke no participant capture/apply;
- perform no migration;
- perform no storage mutation.

## Boundary preserved

ESV-M3-04 does not activate:
- unknown-payload merge/carry-forward publication;
- prune plans;
- participant deserialization/migration/apply;
- prepared/convenience loads;
- production save admission/coalescing/cancellation;
- slots;
- recovery/retention/autosave;
- peer bridges;
- Chronicle-owned DDOL.

## Closeout decision

**ESV-M3-04 is complete.**

Next:

`ESV-M3-05 — Chronicle Opaque Unknown-Payload Carry-Forward Merge, Source-Freshness, and Collision-Safe Publication Foundation`
