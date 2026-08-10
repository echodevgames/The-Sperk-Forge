# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.21.0
**Completed checkpoint:** ESV-M4-02 — Technical Slot Creation, Capacity Enforcement, Initial Empty Generation, and Catalog Reconciliation Foundation
**Completed milestone:** M3 — Participants and Loading
**Current checkpoint:** ESV-M4-04 — Public Manual Save Admission, Busy, Cancellation, and Lifecycle Foundation — active / authorized
**Status:** M3 complete; ESV-M4-01 complete; ESV-M4-02 complete; ESV-M4-03 complete; ESV-M4-04 active; M4 remains active

**Authority reconciliation:** Specification v1.21.0 records ESV-M4-03 complete and activates bounded ESV-M4-04 at clean baseline `3a84187` under ESV-D-026.

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

## ESV-M4-04 active boundary

**Exact planning baseline:** `3a84187`.

M4-04 owns:
- public active-slot `SaveRequest` / `SaveOperationResult` surface;
- additive `IEchoSaveService.SaveAsync(...)`;
- one root-local mutating-operation admission authority intended for later reuse;
- immediate Busy result for overlapping manual saves;
- cancellation before admission and at safe pre-publication checkpoints;
- Too-Late cancellation truth after durable publication begins;
- shutdown closure of new manual-save admission without abandoning an active commit boundary;
- mapping M4-03 durable generation/head/catalog truth into the public result;
- main-thread public completion.

M4-04 does **not** own:
- autosave request/coalescing;
- generic queued multi-operation scheduling, queue capacity, or overflow policy;
- permission-provider production facade wiring;
- retention/recovery;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- full slot-policy/configuration expansion;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

The carried focused regression floor is **439 / 439**. Executed totals are recorded from Unity rather than predicted.
