# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.26.0
**Completed checkpoint:** ESV-M4-06 — Generation Retention Policy, Recovery-History Protection, and Post-Publication Cleanup Foundation
**Completed milestone:** M3 — Participants and Loading
**Current checkpoint:** None activated — ESV-M4-06 complete
**Status:** M3 complete; ESV-M4-01 complete; ESV-M4-02 complete; ESV-M4-03 complete; ESV-M4-04 complete; ESV-M4-05 complete; ESV-M4-06 complete; M4 remains active

**Authority reconciliation:** Specification v1.26.0 records ESV-M4-06 complete at implementation commit `e714a90` with final focused Chronicle Editor evidence `497 / 497`; no follow-on M4 checkpoint is activated.

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

## ESV-M4-04 closeout

**Planning/activation commit:** `91dcb62`.

**Implementation commit:** `2732aaa`.

**Lifecycle-status hotfix:** `09ae8f1`.

**Final effective runtime baseline:** `09ae8f1`.

**Focused Chronicle Editor gate:** **456 / 456 passed, 0 failed**.

**Prior regression floor:** **439 / 439**.

**Net new focused tests:** **17**.

Completed behavior:
- public active-slot `SaveRequest` / `SaveOperationResult`;
- additive `IEchoSaveService.SaveAsync(...)`;
- one root-local mutating-operation admission authority;
- immediate Busy rejection with no hidden manual-save queue;
- safe pre-publication cancellation;
- Too-Late cancellation truth after durable publication begins;
- shutdown admission closure before backend shutdown;
- faithful M4-03 generation/head/catalog result mapping;
- main-thread public completion;
- pre-Ready lifecycle state reports `ServiceNotReady`; shutdown/closed admission reports `AdmissionClosed`.

Implementation-history note:
- one initial public-save lifecycle test failed because the admission coordinator begins closed before initialization;
- v1/v2 patch helpers refused without changing the repository;
- v3 applied the bounded one-file runtime correction;
- final **456 / 456** evidence supersedes the intermediate run.

Still deferred:
- autosave/coalescing;
- generic queued multi-operation scheduling, queue capacity, or overflow policy;
- permission-provider production facade wiring;
- retention/recovery;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- full slot-policy/configuration expansion;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is active. The next implementation requires a bounded authorized Checkpoint Build Plan and must preserve the **456 / 456** focused regression floor.

## ESV-M4-05 closeout

**Planning baseline:** `9a2ad29`.

**Planning/activation commit:** `8504ed4`.

**Implementation commit:** `9917f1b`.

**Final effective runtime baseline:** `9917f1b`.

**Focused Chronicle Editor gate:** **473 / 473 passed, 0 failed**.

**Prior regression floor:** **456 / 456**.

**Net new focused tests:** **17**.

**Committed implementation/test scope:** **22 files**.

Completed behavior:
- public caller-triggered `AutosaveRequest`;
- additive `IEchoSaveService.RequestAutosave(...)`;
- bounded autosave submission/result/ticket truth;
- exactly one pending latest autosave slot;
- latest-wins coalescing and explicit supersession;
- reuse of the M4-04 root-local mutating admission authority;
- reuse of the M4-03/M4-04 durable active-slot save transaction;
- manual save remains Busy rather than queued;
- pending autosave drains at most once after admission becomes available;
- shutdown rejects new submission and discards/prevents pending execution after closure;
- Chronicle owns no automatic gameplay autosave timer.

Regression maintenance:
- M4-04 had intentionally asserted `RequestAutosave` was absent while autosave remained deferred;
- M4-05 authorizes that surface, so the stale absence assertion was updated to assert the bounded `RequestAutosave(AutosaveRequest) -> AutosaveSubmissionResult` API instead;
- no runtime/API architecture change was made by that test maintenance.

Implementation-helper history:
- v1 failed safely on missing destination-directory creation;
- v2 applied all implementation files but counted status rows rather than files;
- v3 counted actual tracked/new files and verifies rollback state;
- final Unity evidence is **473 / 473**.

Still deferred:
- automatic timer/checkpoint autosave triggers;
- generation retention cleanup or `SaveRetentionPolicy`;
- generic multi-operation queueing, configured queue capacity, or overflow policy;
- permission-provider production facade wiring;
- recovery;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- full slot-policy/configuration expansion;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is active. The next implementation requires a bounded authorized Checkpoint Build Plan and must preserve the **473 / 473** focused regression floor.

## ESV-M4-06 closeout

**Planning baseline:** `3cdad0f`.

**Planning/activation commit:** `3d8e0b8`.

**Implementation commit:** `e714a90`.

**Final effective runtime baseline:** `e714a90`.

**Focused Chronicle Editor gate:** **497 / 497 passed, 0 failed**.

**Prior regression floor:** **473 / 473**.

**Net new focused tests:** **24**.

**Committed implementation/test scope:** **33 files**, `2136` insertions, `12` deletions.

Completed behavior:
- project-owned bounded `SaveRetentionPolicy`;
- minimum safe retained history bound of current + immediate predecessor;
- provider-neutral bounded generation discovery;
- additive optional `ISaveStorageTreeDeletionBackend`;
- unchanged base `ISaveStorageBackend`;
- deterministic fail-closed canonical committed-generation classification;
- current and immediate predecessor protection;
- oldest-first excess verified-history deletion;
- post-publication retention only;
- shared retention path for manual save and autosave;
- public retention-maintenance truth through `SaveOperationResult.RetentionResult`;
- cleanup failure never rewrites committed generation/head truth.

Integration-test correction:
- initial run discovered **497** tests with **495 passed / 2 failed**;
- the new manual-retention integration fixture had no registered participant, so two success-path tests failed before publication and one injected-failure test passed too early;
- one test-only correction registered a normal participant in all three integration cases;
- runtime/API/architecture/authority/test-count remained unchanged;
- final rerun passed **497 / 497**.

Still deferred:
- recovery-plan generation/execution and corruption fallback;
- quarantine movement;
- rename/duplicate/delete/trash and trash-history retention;
- persistent `catalog.cache.json`;
- generic queue policy/capacity/overflow;
- automatic timer/checkpoint autosave triggers;
- permission-provider production facade wiring;
- full `EchoSaveConfiguration` / Setup expansion;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is active. The next implementation requires a bounded authorized Checkpoint Build Plan and must preserve the **497 / 497** focused regression floor.
