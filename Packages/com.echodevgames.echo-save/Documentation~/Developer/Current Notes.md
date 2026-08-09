# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.9.0
**Completed checkpoint:** ESV-M3-02 — Detached Participant Capture, Runtime Type Routing, and Payload-Entry Construction Foundation
**Current checkpoint:** ESV-M3-03 — Participant-Backed Generation Publication and Head-Last Integration Foundation
**Status:** ESV-M3-02 complete; ESV-M3-03 active / authorized

## ESV-M3-02 closeout

Implementation commit: `e34d6d7`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **171 / 171 passed, 0 failed**;
- all prior 147 Chronicle regressions remain green;
- runtime DTO type authority comes only from live participant registration code;
- no CLR/assembly-qualified type name is persisted or activated from save data;
- participant capture order is deterministic;
- explicit/default serializer routing is proven;
- exact UTF-8 participant payload byte lengths are recorded;
- per-entry SHA-256 integrity checksums are recorded;
- payload and manifest inventory records agree in memory;
- Required/Optional projects into transport metadata;
- capture/type/serializer/integrity failures abort the entire batch;
- future participants use the same capture pipeline;
- capture performs no durable storage/publication mutation.

## Active ESV-M3-03 boundary

Authorized:
- successful capture-batch publication only;
- publication-boundary revalidation of participant entries/inventory;
- inline participant byte-length/checksum verification;
- participant-bearing `SavePayloadDocument`;
- matching participant inventory in `SaveManifest`;
- whole-payload generation checksum/byte-length calculation;
- candidate write/read-back verification;
- immutable participant-bearing generation publication;
- published-generation revalidation;
- `head.json` update last;
- prior-known-good preservation across injected failures;
- orphan participant generation remains non-current after failed head update;
- focused participant-backed publication tests.

Still absent:
- production `SaveAsync`;
- request admission/permission/coalescing/cancellation;
- autosave;
- unknown-payload carry-forward;
- participant apply and prepared loads;
- migrations;
- slot catalog/policy;
- recovery/retention;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

M3-03 is a bounded integration seam, not the final public save operation API.
