# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.20.0
**Completed checkpoint:** ESV-M4-02 — Technical Slot Creation, Capacity Enforcement, Initial Empty Generation, and Catalog Reconciliation Foundation
**Completed milestone:** M3 — Participants and Loading
**Current checkpoint:** None activated — ESV-M4-03 complete
**Status:** M3 complete; ESV-M4-01 complete; ESV-M4-02 complete; ESV-M4-03 complete; M4 remains active

**Authority reconciliation:** Specification v1.20.0 records ESV-M4-03 complete at implementation commit `c8ea742` with focused Chronicle Editor evidence `439 / 439`; no follow-on M4 checkpoint is activated.

## ESV-M4-02 closeout

Implementation commit: `d8d5c18`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **425 / 425 passed, 0 failed**;
- prior **403 / 403** Chronicle regression floor remains green;
- 22 net new focused M4-02 tests passed;
- technical creation refreshes a trustworthy catalog before durable mutation;
- healthy and degraded canonical technical slots both count against capacity;
- invalid non-slot children remain excluded;
- canonical `SaveSlotId` is package-generated with bounded collision retry;
- display/project/build metadata never becomes physical path identity;
- initial creation publishes an empty immutable generation through candidate verification, immutable publication, final verification, and `head.json` last;
- create-specific publication rejects an existing current head inside the transaction;
- successful publication reconciles the M4-01 catalog without auto-selecting;
- post-publication refresh failure reports durable publication truth rather than deleting or pretending rollback;
- zero participant capture/apply/default callbacks enter technical creation;
- persistent cache, rename/duplicate/delete, full slot-policy assets, production operation admission, autosave, retention, recovery, scene travel, peer bridges, and DDOL ownership remain absent.

Implementation-history note:
- the first apply validator was narrowed after it matched deferred-scope words inside architecture comments;
- NUnit parameterized-test accessibility/discovery was repaired test-only by using public primitive parameters and internal casts;
- one final-verification expectation was corrected to preserve `generationPublished = true` after immutable publication;
- final Unity evidence is the authoritative **425 / 425** gate.

## M4 milestone state

**M4 — Slots / Autosave / Recovery remains active.**

Chronicle now has:
- provider-neutral payload-free catalog reconstruction;
- healthy/degraded immutable catalog snapshots;
- explicit session-only active-slot selection;
- bounded technical slot creation;
- positive capacity enforcement;
- package-generated technical identity with collision retry;
- real empty immutable first-generation publication with `head.json` last;
- truthful post-publication catalog reconciliation.

## ESV-M4-03 closeout

**Planning baseline:** `a3eba25`.
**Planning/activation commit:** `2c325e9`.
**Implementation commit:** `c8ea742`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **439 / 439 passed, 0 failed**;
- prior **425 / 425** regression floor preserved;
- **14** net new focused M4-03 tests passed;
- selected active slot must be healthy before capture;
- current generation is validated and bound as exact source provenance;
- fresh deterministic known participant capture remains all-or-nothing;
- valid opaque unknown payloads survive carry-forward;
- unknown ownership/provenance collisions block before publication;
- expected-current-generation stale-source rejection remains enforced;
- participant-backed immutable publication still commits `head.json` last;
- ordinary save preserves current display name;
- catalog reconciliation follows durable head publication;
- post-publication catalog-refresh failure reports partial durable truth without rollback fiction;
- participant Apply/default callbacks remain absent.

Still deferred:
- public `SaveAsync` / `IEchoSaveService` production save facade;
- generic operation admission, Busy queue semantics, cancellation, or shutdown settlement;
- permission-provider facade wiring;
- autosave/coalescing;
- retention/recovery;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- full slot-policy/configuration expansion;
- document migration;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

## Next bounded planning boundary

No follow-on M4 checkpoint is activated by this closeout.

Further Chronicle runtime implementation requires a new bounded Checkpoint Build Plan and must preserve the focused **439 / 439** regression floor.
