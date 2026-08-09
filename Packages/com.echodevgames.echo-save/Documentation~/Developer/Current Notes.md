# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.10.0
**Completed checkpoint:** ESV-M3-03 — Participant-Backed Generation Publication and Head-Last Integration Foundation
**Current checkpoint:** ESV-M3-04 — Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation
**Status:** ESV-M3-03 complete; ESV-M3-04 active / authorized

## ESV-M3-03 closeout

Implementation commit: `6970127`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **197 / 197 passed, 0 failed**;
- all prior 171 Chronicle regressions remain green;
- invalid participant batches cause zero storage mutation;
- participant entry/inventory structure and inline integrity are revalidated at the publication boundary;
- participant-bearing generations use the established candidate/read-back/immutable-generation/published-reverify/head-last transaction;
- every pre-head failure preserves the previous known-good head;
- head serialization/publication failure leaves the new generation non-current/orphaned;
- second participant-backed save advances head sequence and preserves the prior immutable generation;
- M2 empty/transport publication remains green.

Test-rig note:
- one public parameterized NUnit test initially used a less-accessible nested enum, producing CS0051;
- the enum was made public inside the Editor test class;
- no Chronicle runtime behavior or package API changed.

## Active ESV-M3-04 boundary

Authorized:
- read-only current-head resolution for one explicit technical slot;
- current manifest/payload read and complete structural/integrity validation;
- per-participant entry integrity validation;
- canonical/alias recognition against the active participant registry;
- package-owned opaque `UnknownPayloadStore`;
- byte-for-byte / field-for-field preservation of unclaimed participant entries;
- immutable/defensive-copy store snapshots;
- deterministic ordering and bounded count/byte safeguards;
- atomic session-store replacement only after a complete successful read/classification;
- failed read/classification preserves the prior valid session store.

Still absent:
- merge/carry-forward of unknown entries into a new generation;
- explicit prune plans;
- participant deserialization/migration/apply;
- prepared/convenience loads;
- production save admission/permission/busy/coalescing/cancellation;
- slot catalog/policy;
- recovery/retention/autosave;
- peer bridges;
- Chronicle-owned/project-wide DDOL.

Unknown payloads are inert durable data in M3-04. Chronicle may validate and remember them, but it may not interpret them.
