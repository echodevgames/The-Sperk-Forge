# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.34.0
**Completed checkpoint:** ESV-M4-10 — Destructive Slot Deletion Planning, Confirmed Trash, and Bounded Trash Retention Foundation
**Completed milestone:** M3 — Participants and Loading
**Current checkpoint:** M4 milestone reconciliation — pending / no implementation activated
**Status:** M3 complete; ESV-M4-01 through ESV-M4-10 complete; M4 remains active pending milestone reconciliation

**Authority reconciliation:** Specification v1.34.0 records ESV-M4-10 complete at implementation `01e4cdd` with focused gate `587 / 587`. No M5 work is active; a dedicated M4 milestone reconciliation is the next gate.

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

## ESV-M4-07 closeout

**Planning baseline:** `9695450`.

**Planning/activation commit:** `7b00503`.

**Implementation commit:** `9f68555`.

**Final effective runtime baseline:** `9f68555`.

**Focused Chronicle Editor gate:** **524 / 524 passed, 0 failed**.

**Prior regression floor:** **497 / 497**.

**Net new focused tests:** **27**.

**Committed implementation/test scope:** **22 files**, `2912` insertions, `6` deletions.

Completed behavior:
- public read-only `BuildRecoveryPlanAsync(SaveSlotId)`;
- explicit source head/current diagnosis;
- bounded provider-neutral generation discovery;
- full candidate manifest/payload/integrity/identity/commit-state verification;
- preservation/exclusion of unsupported, corrupt, incomplete, mismatched, uncommitted, and noncanonical evidence;
- deterministic newest-valid fallback ordering;
- preferred candidate only when recovery is required;
- immutable payload-free recovery plan/candidate summaries;
- deterministic technical source-provenance fingerprint;
- zero durable mutation during planning;
- no participant callbacks or migration side effects;
- no mutating-operation admission lease for read-only planning.

Test-fixture corrections:
- one compile-only test-support constant was corrected from `HeadMajor` to `HeadPointerMajor`;
- initial focused run was **522 / 524** because two future-version fixtures tried to serialize through Chronicle's production serializer, which correctly rejected unsupported package documents;
- those intentionally unsupported fixtures now use Unity `JsonUtility` directly while ordinary supported fixtures remain on Chronicle's serializer;
- runtime/API/architecture/authority/recovery behavior/test intent/test count remained unchanged;
- final rerun passed **524 / 524**.

Still deferred:
- recovery execution/head publication;
- catalog reconciliation after recovery;
- automatic/configured fallback execution;
- recovery mutation admission/Busy/cancellation;
- stale-plan execution rejection beyond captured provenance;
- quarantine and incomplete-generation cleanup;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- generic queues/capacity/overflow;
- automatic autosave timers;
- permission-provider production facade wiring;
- full recovery/configuration/Setup expansion;
- document migration;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is active. The next implementation requires a bounded authorized Checkpoint Build Plan and must preserve the **524 / 524** focused regression floor.

## ESV-M4-08 closeout

**Planning baseline:** `0396adb`.

**Planning/activation commit:** `c324aa4`.

**Implementation commit:** `1985fb0`.

**Final effective runtime baseline:** `1985fb0`.

**Final focused gate:** **540 / 540 passed, 0 failed**.

**Prior focused regression floor:** **524 / 524**.

**Net new focused tests:** **16**.

**Committed implementation/test scope:** **18 files**, `1846` insertions, `10` deletions.

Completed behavior:
- public explicit `ExecuteRecoveryAsync(SaveRecoveryPlan, SaveRecoveryCandidate)`;
- root-local mutating-operation admission reuse;
- immediate Busy rejection with no recovery queue;
- fresh M4-07 recovery-plan rebuild after admission;
- exact source-provenance stale-plan rejection;
- exact selected-candidate membership and fresh verification;
- zero durable mutation before freshness/candidate proof;
- selected generation contents remain immutable;
- `head.json` repointing only;
- recovery-created `previousGenerationId` remains empty rather than blessing damaged source history;
- post-head catalog refresh/reconciliation;
- truthful `HeadPublished` versus `CatalogReconciled` result separation;
- committed-head truth is preserved if catalog reconciliation later fails;
- active-slot selection is preserved;
- no participant capture/apply/default callbacks.

Compile-only test maintenance:
- two new test references used nonexistent `FakeManualSaveTransactionExecutor.CallCount`;
- the established fake exposes `Calls`;
- the two references were corrected test-only;
- runtime/API/architecture/ESV-D-030 authority/recovery behavior/test intent/discovery shape were unchanged;
- final rerun passed **540 / 540**.

Still deferred:
- automatic/configured fallback;
- automatic recovery during load;
- quarantine/incomplete-generation cleanup;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- generic queues/capacity/overflow;
- recovery cancellation overload;
- automatic autosave timers;
- permission-provider production facade wiring;
- full recovery/configuration/Setup expansion;
- document/participant migration during recovery;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is active. The next implementation requires a bounded authorized Checkpoint Build Plan and must preserve the **540 / 540** focused regression floor.


## ESV-M4-09 closeout

**Planning baseline:** `07bbd2b`.

**Planning/activation commit:** `7d2d987`.

**Implementation commit:** `459023f`.

**Final effective runtime baseline:** `459023f`.

**Final focused gate:** **562 / 562 passed, 0 failed**.

**Prior focused regression floor:** **540 / 540**.

**Net new focused tests:** **22**.

**Committed implementation/test scope:** **26 files**, `3100` insertions, `8` deletions.

Completed behavior:
- public bounded `RenameSlotAsync(SaveSlotRenameRequest)`;
- public bounded `DuplicateSlotAsync(SaveSlotDuplicateRequest)`;
- shared root-local mutation admission reuse;
- immediate Busy rejection with no rename/duplicate queues;
- pre-Ready `ServiceNotReady` and post-shutdown `AdmissionClosed` truth;
- stable `SaveSlotId` and stable physical path across rename;
- rename as a new immutable metadata-updated generation rather than in-place committed-file mutation;
- source-current-generation verification and exact provenance revalidation;
- expected-current-generation protection before rename publication;
- M4-06 retention after committed rename;
- M4-01 catalog reconciliation after publication;
- active-slot identity preservation;
- duplicate M4-02 canonical-slot capacity accounting;
- bounded package-generated duplicate slot-ID collision retry;
- new destination slot/generation identities;
- fully verified source-state copy without participant callbacks;
- source committed bytes preserved;
- destination generation/head-last publication;
- duplicate no-auto-select behavior;
- truthful committed-but-unreconciled result after post-head catalog failure;
- unchanged base `ISaveStorageBackend`.

Registry proofs completed:
- ESV-T-019 — rename changes display metadata while ID/path stay stable;
- ESV-T-020 — duplicate creates a new slot identity with equivalent verified state.

Still deferred:
- prepare-delete / confirm-delete;
- trash and trash retention;
- quarantine / incomplete-generation cleanup;
- persistent `catalog.cache.json`;
- automatic/configured recovery fallback;
- recovery-on-load;
- generic operation queues/capacity/overflow;
- recovery cancellation overload;
- automatic autosave timers/gameplay triggers;
- permission-provider production wiring;
- full configuration/Setup expansion;
- document migration;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is active. Any further Chronicle runtime implementation requires a separately bounded authorized Checkpoint Build Plan and must preserve the **562 / 562** focused regression floor.

## ESV-M4-10 activation

**Clean planning baseline:** `4d2f2ac`.

**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.33.0 / ESV-D-032.

**Carried focused regression floor:** **562 / 562**.

Authorized behavior:
- read-only two-step deletion planning;
- immutable package/session/source-provenance-bound deletion plans;
- bounded expiry and one-use confirmation;
- no mutation during prepare-delete;
- root-local Busy/no-queue confirm-delete admission;
- fresh source revalidation before destructive mutation;
- recoverable trash move as durable delete truth;
- active-slot clear only after durable delete;
- catalog reconciliation after durable removal;
- bounded post-commit trash retention;
- ESV-T-021 through ESV-T-023;
- no participant callbacks;
- unchanged base `ISaveStorageBackend`.

Still deferred:
- permanent erase;
- public restore-from-trash;
- quarantine/incomplete cleanup;
- persistent catalog cache;
- automatic/configured recovery fallback;
- generic queues;
- automatic autosave timers;
- permission-provider production wiring;
- full configuration/Setup authoring;
- M5 Editor tooling/Laboratory;
- scene travel, bridges, service locator, DDOL.

Do not mark M4 complete automatically after implementation. ESV-M4-10 closeout must be followed by a dedicated M4 milestone reconciliation.


## ESV-M4-10 closeout

**Planning baseline:** `4d2f2ac`.
**Planning/activation commit:** `2244e3c`.
**Implementation commit:** `01e4cdd`.
**Final focused gate:** **587 / 587 passed, 0 failed**.
**Prior focused regression floor:** **562 / 562**.
**Net new focused tests:** **25**.
**Committed implementation/test scope:** **28 files**, `2863` insertions, `6` deletions.

Completed behavior:
- public read-only `PrepareDeleteSlotAsync(SaveSlotId)`;
- immutable package/session/source-bound `SaveDeletionPlan`;
- bounded expiry and one-use confirmation truth;
- zero durable mutation during prepare-delete;
- public admitted `ConfirmDeleteSlotAsync(SaveDeletionPlan)`;
- immediate Busy rejection with no delete queue;
- fresh exact-source/catalog revalidation before destructive mutation;
- complete live-slot move into recoverable package-owned trash;
- active-slot clear only after durable removal;
- non-active delete preserves current active selection;
- post-delete live catalog reconciliation;
- bounded fail-closed oldest-first trash retention;
- truthful committed-but-unreconciled and committed-but-maintenance-failed states;
- ESV-T-021 / ESV-T-022 / ESV-T-023 complete;
- zero participant callbacks;
- unchanged base `ISaveStorageBackend`.

Still deferred:
- permanent erase;
- public restore-from-trash;
- quarantine/incomplete cleanup;
- persistent catalog cache;
- automatic/configured recovery fallback;
- recovery-on-load;
- generic operation queues/capacity/overflow;
- recovery cancellation overload;
- automatic autosave timers/gameplay triggers;
- permission-provider production wiring;
- full configuration/Setup authoring;
- M5 Editor tooling/Laboratory;
- document migration;
- scene travel, bridges, service locator, DDOL.

No follow-on runtime checkpoint is active.

**Next gate:** dedicated M4 milestone reconciliation. Do not declare M4 complete or activate M5 until the milestone audit reconciles authority, implemented capability truth, the applicable test registry, and closeout documentation.
