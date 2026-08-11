# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 11, 2026
**Current focus:** Chronicle M5 — M5-01 Editor tooling assembly, Setup preview, and Validator foundation active
**Current checkpoint:** ESV-M5-01 — Editor Tooling Assembly, Setup Preview, and Validator Foundation — **ACTIVE / AUTHORIZED**

> Durable First Light decisions live in SFGSS-PKG-ECHOLAUNCH-001 v1.16.0 and the committed checkpoint/amendment records. Git history preserves the longer working trail.

---

## Current State

- First Light package version remains `0.1.0`.
- First Light package specification remains **SFGSS-PKG-ECHOLAUNCH-001 v1.16.0**.
- FL-M5-07 Standalone Test Laboratory is complete at `710aec3` with retained automated evidence `809 / 809` and manual Laboratory evidence `12 / 12`.
- FL-M6-01 Production Reference Showcase implementation and in-repository consumer proof are complete.
- A1 splash presentation/authoring is committed across `1b7ab84`, `d36b5cc`, `90e038c`, `9b24121`, and `4bdc264`.
- Slice E Setup creation-time splash authoring is committed at `9e6df00`.
- A1-E1 project-owned foundation resolution is authorized at `a70e478` and implemented at `e66b9fd`.
- The permanent First Light Gallery is committed at `ccb1d59`; obsolete pre-gallery folder metadata is removed at `ad12b27`.
- The final `EchoLaunchSetup` filtered EditMode gate passed **224 / 224**.
- No post-A1 full-suite aggregate is claimed by this closeout. The retained FL-M5-07 full-suite baseline remains historical evidence and should be rerun at the next release-qualification gate.
- SUITE-DIST-001 is complete at `c18eff6`; First Light `0.1.0` has a repository-owned Distribution Kit and Complete User Handout.
- First Light remains frozen for this pass.
- Jesse selected The Chronicle (`EchoSave`) as the next active package learning review.
- The next multi-package initiative is the **Game Shell / Front Door**: Chronicle → Accord → Resonance → Looking Glass, composed later with First Light while preserving standalone package ownership.

## Active Suite Architecture — Persistence and Lifetime Separation

Jesse approved SFGSS-ADR-006 and the suite-wide rule:

> Durable persistence, runtime state, and Unity object lifetime are separate concerns. Packages may expose persistence-capable state without depending directly on EchoSave. Cross-package persistence integration belongs in optional bridges/adapters. Long-lived Unity service composition is project-owned and must not turn First Light, The Chronicle, or another package into a universal service locator.

Immediate consequences:

- Chronicle owns durable **game-save transport**, slots/generations, migration/recovery, and save orchestration.
- Accord remains the authority for global preferences; Chronicle does not absorb graphics/audio settings merely because those values persist.
- Inventory, Progression, Objectives, Characters, World, and future peers retain their own live runtime truth and durable payload meaning.
- A Chronicle load restores participant-owned live state; Chronicle does not become that state after load.
- `DontDestroyOnLoad` / scene-surviving object lifetime is not durable persistence.
- Consumer projects may compose long-lived package services beneath a project-owned runtime root. That root does not become a new authority domain or service locator.
- First Light may initialize/discover long-lived services, then hands off.
- Chronicle may have a duplicate-safe package-local root, but never becomes the parent/locator for unrelated services.

SFGSS-000 v0.26.0, SFGSS-001 v1.5.0, SFGSS-ADR-006, SFGSS-INT-SUITE-001 v1.1.0, and Chronicle specification v1.2.0 carry this rule.

## Chronicle Learning Gate

- Review: `PKG-LEARN-009_EchoSave_Learning_Review.md`
- Status: **Complete**
- Implementation authorization: **ESV-M1-01 active / authorized**
- First implementation checkpoint scaffold: `ESV-M1-01 — Installable Skeleton and Duplicate-Safe Authority Claim`
- Jesse completed the Chronicle teach-back and explicitly activated implementation on 2026-08-09.
- The completed teach-back covered runtime truth versus durable snapshots, scopes/policies, participant ownership, migrations, transactional candidate safety, required/optional participation, known-good generations, duplicate-safe authority, shutdown, operation gating, coherent capture, coordinated restore/rollback, dirty state versus save policy, and separation of save model/serialization/storage.
- Serializer/file-format implementation choices remain intentionally deferred because ESV-M1-01 proves lifecycle and duplicate safety without real durable storage.

## Chronicle ESV-M1-01 Closeout

- Learning/activation commit: `5b05d9d`.
- Chronicle package implementation commit: `ecfa922`.
- Embedded Package Manager resolution commit: `2c70b1d`.
- Package ID/version: `com.echodevgames.echo-save` `0.1.0`.
- Unity compile/import: **Pass / green**.
- Focused `EchoDevGames.EchoSave.Tests.Editor` gate: **all green**; exact numeric count not captured, so no count is claimed.
- Apply-time M1 guards passed: no `System.IO` implementation, no `Application.persistentDataPath` resolution, no Chronicle-owned `DontDestroyOnLoad`, and no peer Echo runtime reference.
- ESV-M1-01 proved package-local authority, duplicate rejection before service construction/initialization, explicit initialize/shutdown, re-claim after shutdown, configuration blocking, stable provider IDs, and zero durable-storage side effects.

## Chronicle ESV-M2-01 Closeout

- Implementation commit: `e4ef76c`.
- Unity compile/import: **Pass / green**.
- Focused `EchoDevGames.EchoSave.Tests.Editor`: **40 / 40 passed, 0 failed**.
- First focused run was `29 / 40`; all 11 failures were isolated to EditMode `EchoSaveRoot` activation assumptions. The storage/path/backend tests were already green.
- A narrow internal `EnsureAuthorityClaimedForTesting()` seam was added so direct EditMode component construction deterministically exercises the exact production `Awake()` authority path when Unity has not invoked it.
- Final gate proves safe storage keys, traversal/root rejection, sandbox root resolution, default local backend initialization, exact-byte round trips, create-only conflict preservation, structured not-found/failure behavior, duplicate-before-storage behavior, and retained M1 lifecycle rules.
- No save document, serializer payload, slot, immutable generation publication, participant, recovery/autosave, peer bridge, or Chronicle-owned DDOL behavior was introduced.

## Chronicle ESV-M3-09 Closeout

- Implementation commit: `568fa3a`.
- Unity compile/import: **Pass / green**.
- Focused `EchoDevGames.EchoSave.Tests.Editor`: **366 / 366 passed, 0 failed**.
- The complete prior **332 / 332** Chronicle regression floor remained green.
- M3-09 added 34 focused participant-apply tests.
- Optional `ISaveDefaultableParticipant.InitializeDefault()` is additive; base `ISaveParticipant` remains unchanged.
- Complete apply planning invokes zero participant/default callbacks.
- Prepared owner/schema/runtime-type and duplicate prepared identity checks fail before mutation.
- Missing-payload `Fail` blocks before mutation.
- Missing-payload `Ignore` invokes no callback.
- Missing-payload `InitializeDefault` requires the optional capability.
- Default initialization never uses `Apply(null)`.
- Prepared state invokes exactly `Apply(detachedState)`.
- Registration ownership tokens are revalidated before callbacks.
- Pure preflight failure leaves the handle live for repair/retry.
- Once execution begins, terminal success/failure consumes the handle and replay rejects.
- Participant-returned failure/exception produces bounded structured partial reporting.
- Chronicle does not pretend arbitrary participant mutation is transactionally rollback-capable.
- Source save generation/head/payload remain unchanged.
- Scene/DDOL authority remains absent.

## Chronicle M3 Milestone Closeout

**M3 — Participants and Loading is complete.**

Completed M3 capabilities now include:
- open-ended participant contracts and duplicate-safe registry;
- detached capture and participant-backed immutable publication;
- opaque unknown-payload preservation and source-fresh carry-forward;
- trusted current-version payload preparation;
- contiguous participant migration;
- bounded prepared-load handle lifetime/session ownership;
- deterministic ownership-revalidated participant apply and missing-payload policy.

## Chronicle M4 Progress

`ESV-M4-01 — Chronicle Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation` is **complete** at `62e8a54`.

Evidence:
- Unity compile/import green;
- focused Chronicle Editor gate **403 / 403**, `0` failed;
- prior **366 / 366** regression floor preserved;
- 37 net new M4-01 tests passed;
- additive provider-neutral storage discovery with unchanged base `ISaveStorageBackend`;
- payload-free head/current-manifest catalog rebuild;
- healthy/degraded immutable snapshots;
- prior-snapshot preservation on untrustworthy refresh failure;
- explicit session-only active selection with no automatic selection.

`ESV-M4-02 — Chronicle Technical Slot Creation, Capacity Enforcement, Initial Empty Generation, and Catalog Reconciliation Foundation` is **complete** at `d8d5c18`.

Evidence:
- Unity compile/import green;
- focused Chronicle Editor gate **425 / 425**, `0` failed;
- prior **403 / 403** regression floor preserved;
- 22 net new M4-02 tests passed;
- bounded technical create request/result/status and coordinator;
- trustworthy catalog preflight before durable mutation;
- healthy and degraded canonical slots both count against positive capacity;
- package-generated canonical `SaveSlotId` with bounded collision retry;
- display/project/build metadata remains path-independent;
- real empty immutable generation publication with candidate verification, immutable publish, final verification, then `head.json` last;
- existing current head rejected inside the create-publication transaction;
- successful post-publication catalog reconciliation without auto-select;
- published-but-reconciliation-failed truth preserved without deleting committed durable state;
- zero participant callbacks;
- deferred persistent cache / rename / duplicate / delete / autosave / retention / recovery / scene / bridge / DDOL scope remains absent.

Implementation-history note:
- helper validation was narrowed after deferred-scope words inside architecture comments produced a false positive;
- NUnit parameterized-test accessibility/discovery was repaired test-only through public primitive parameters and internal enum casts;
- one final-verification expected value was corrected to preserve `generationPublished = true` after immutable publication;
- final **425 / 425** evidence supersedes all intermediate runs.

`ESV-M4-03 — Chronicle Manual Save Transaction Composition, Unknown Carry-Forward, and Catalog Reconciliation Foundation` is **complete** at implementation commit `c8ea742`.

Evidence:
- Unity compile/import green;
- focused Chronicle Editor gate **439 / 439**, `0` failed;
- prior **425 / 425** regression floor preserved;
- **14** net new focused M4-03 tests passed;
- manual save targets only the explicitly selected healthy active slot;
- current-generation validation refreshes exact source provenance before participant capture;
- fresh known participant capture reuses the proven deterministic capture authority;
- valid opaque unknown payloads carry forward through the collision-safe merger;
- ownership collisions and provenance mismatch fail before publication;
- expected-current-generation publication rejects stale source;
- immutable generation publication and `head.json` last remain unchanged;
- ordinary save preserves current display-name metadata;
- successful head publication is followed by catalog reconciliation;
- catalog reconciliation failure after durable head success reports partial durable truth without deleting committed state;
- participant Apply/default callbacks remain absent;
- public `SaveAsync`, production admission/Busy/cancellation, autosave, retention, recovery, persistent catalog cache, rename/duplicate/delete, full slot-policy assets, scene travel, bridges, and DDOL remain deferred.

No follow-on M4 checkpoint is activated by this closeout.

## Chronicle ESV-M4-06 Closeout

`ESV-M4-06 — Chronicle Generation Retention Policy, Recovery-History Protection, and Post-Publication Cleanup Foundation` is **complete**.

Evidence:
- planning baseline: `3cdad0f`;
- planning/activation commit: `3d8e0b8`;
- implementation commit: `e714a90`;
- final effective runtime baseline: `e714a90`;
- Unity compile/import: **green**;
- final focused `EchoDevGames.EchoSave.Tests.Editor`: **497 / 497 passed, 0 failed**;
- prior **473 / 473** Chronicle regression floor preserved;
- **24** net new focused tests;
- committed implementation/test scope: **33 files**, `2136` insertions, `12` deletions;
- bounded `SaveRetentionPolicy` is present;
- base `ISaveStorageBackend` remains unchanged;
- additive optional `ISaveStorageTreeDeletionBackend` owns provider tree deletion;
- current and immediate predecessor generations are protected;
- untrustworthy history fails closed without deletion;
- only excess verified committed history is eligible;
- deletion order is deterministic oldest-first;
- retention begins only after committed generation/head publication;
- manual save and autosave share the same retention path;
- retention failure is maintenance truth and never fabricates save rollback.

Integration-test correction:
- first focused run: **495 / 497**, `2` failed;
- both failures came from new integration tests that registered no participant and therefore stopped during capture before publication/retention;
- the third failure-injection integration test also passed for that wrong early-stop reason;
- a test-only setup correction registered one ordinary participant in all three cases;
- runtime implementation, API, architecture, authority, and discovery count were unchanged;
- final rerun: **497 / 497**.

Still deferred:
- recovery planning/execution and fallback selection;
- quarantine movement;
- rename/duplicate/delete/trash and trash retention;
- persistent catalog cache;
- generic operation queues/capacity/overflow;
- automatic autosave timers;
- permission-provider production wiring;
- full configuration/Setup expansion;
- scene travel, bridges, service locator, DDOL.

No follow-on M4 checkpoint is activated by this closeout.


## Chronicle ESV-M4-05 Closeout

`ESV-M4-05 — Chronicle Autosave Request Coalescing and Latest-Wins Pending Admission Foundation` is **complete**.

Evidence:
- planning/activation commit: `8504ed4`;
- implementation commit: `9917f1b`;
- final effective runtime baseline: `9917f1b`;
- Unity compile/import: **green**;
- final focused `EchoDevGames.EchoSave.Tests.Editor`: **473 / 473 passed, 0 failed**;
- prior **456 / 456** Chronicle regression floor preserved;
- **17** net new focused tests over the prior floor;
- implementation/test commit scope: **22 files**, including one stale M4-04 public-surface regression-test update;
- public caller-triggered `RequestAutosave(...)` is present;
- exactly one latest-wins pending autosave may exist while root-local admission is occupied;
- newer pending autosave metadata supersedes older pending metadata instead of growing a queue;
- M4-04 manual-save Busy behavior remains unchanged;
- pending autosave drains at most once after admission becomes available;
- shutdown rejects new autosave submissions and prevents pending work from starting after closure;
- autosave continues through the same M4-03/M4-04 durable active-slot save transaction;
- Chronicle still owns no gameplay timer or rule deciding when autosave should occur;
- retention, generic queued multi-operation policy, recovery, rename/duplicate/delete, persistent catalog cache, full slot-policy assets, scene travel, peer bridges, and project-wide DDOL remain deferred.

Implementation-history note:
- the first helper failed safely because a new nested destination directory was not created;
- the second helper applied the payload but incorrectly counted `git status --porcelain` rows instead of actual files;
- the corrected helper counted **4 tracked + 17 new = 21** M4-05 payload files and added verified rollback behavior;
- the first M4-05 Unity run exposed one stale M4-04 API absence assertion, not a runtime defect;
- the test-only maintenance update changed that assertion to the newly authorized bounded autosave surface;
- final **473 / 473** evidence supersedes the intermediate failing run.

No follow-on M4 checkpoint is activated by this closeout.


`ESV-M4-04 — Chronicle Public Manual Save Admission, Busy, Cancellation, and Lifecycle Foundation` is **active / authorized** at clean planning baseline `3a84187`.

Authorized boundary:
- expose public active-slot `SaveAsync` through `IEchoSaveService`;
- map public request/result truth onto the complete M4-03 manual-save transaction;
- establish one root-local mutating-operation admission authority for later reuse;
- overlapping manual saves return Busy immediately and do not queue;
- cancellation is honored before admission and at safe pre-publication boundaries;
- cancellation becomes Too Late once durable publication begins;
- shutdown closes new admission and does not abandon an in-progress commit boundary;
- public completion returns on the main thread;
- autosave/coalescing, generic queued multi-operation policy, permission-provider facade wiring, retention, recovery, rename/duplicate/delete, persistent catalog cache, full slot-policy assets, scene travel, bridges, and DDOL remain outside M4-04.


## Chronicle ESV-M4-04 Closeout

`ESV-M4-04 — Chronicle Public Manual Save Admission, Busy, Cancellation, and Lifecycle Foundation` is **complete**.

Evidence:
- planning/activation commit: `91dcb62`;
- implementation commit: `2732aaa`;
- bounded lifecycle-status hotfix: `09ae8f1`;
- final effective runtime baseline: `09ae8f1`;
- Unity compile/import: **green**;
- final focused `EchoDevGames.EchoSave.Tests.Editor`: **456 / 456 passed, 0 failed**;
- prior **439 / 439** Chronicle regression floor preserved;
- **17** net new focused M4-04 tests are present over the prior floor;
- public active-slot `SaveRequest` / `SaveOperationResult` and additive `IEchoSaveService.SaveAsync(...)` are present;
- one root-local mutating-operation admission authority admits one mutation at a time;
- overlapping manual save rejects Busy immediately and does not queue;
- pre-publication cancellation remains bounded and cannot advance the head;
- cancellation after durable publication begins reports Too Late without pretending rollback;
- shutdown closes new admission before backend shutdown and does not abandon a commit already crossing the durable boundary;
- M4-03 remains the durable capture/unknown-carry-forward/publication/catalog transaction engine;
- the pre-Ready lifecycle hotfix preserves `ServiceNotReady` before Ready and reserves `AdmissionClosed` for shutdown/actual closed admission;
- autosave/coalescing, generic queued multi-operation policy, retention, recovery, rename/duplicate/delete, persistent catalog cache, full slot-policy assets, scene travel, peer bridges, and project-wide DDOL remain deferred.

Implementation-history note:
- the first focused M4-04 run exposed one lifecycle-ordering failure: pre-Ready save returned `AdmissionClosed` instead of `ServiceNotReady`;
- the first two hotfix patch helpers refused safely and changed nothing;
- v3 used exact committed-file identity and a full corrected-file replacement;
- final **456 / 456** evidence supersedes the intermediate failing run.

No follow-on M4 checkpoint is activated by this closeout.

## Chronicle ESV-M4-07 Closeout

`ESV-M4-07 — Chronicle Recovery Candidate Discovery, Immutable Recovery Plan Truth, and Deterministic Fallback Selection Foundation` is **complete**.

Evidence:
- planning baseline: `9695450`;
- planning/activation commit: `7b00503`;
- implementation commit: `9f68555`;
- final effective runtime baseline: `9f68555`;
- Unity compile/import: **green**;
- final focused `EchoDevGames.EchoSave.Tests.Editor`: **524 / 524 passed, 0 failed**;
- prior **497 / 497** Chronicle regression floor preserved;
- **27** net new focused tests;
- committed implementation/test scope: **22 files**, `2912` insertions, `6` deletions;
- public read-only `BuildRecoveryPlanAsync(SaveSlotId)` is present;
- healthy/missing/invalid/broken-current source states are reported explicitly;
- generation discovery remains bounded and provider-neutral;
- candidate eligibility requires full document/integrity/committed-state verification;
- bad/unsupported/noncanonical evidence is preserved and excluded;
- candidate ordering is deterministic newest-valid first;
- recovery plans are immutable and payload-free;
- exact technical source provenance is fingerprinted for later stale-plan rejection;
- recovery planning performs zero durable mutation and no participant callbacks.

Test-fixture corrections:
- compile correction: `SaveDocumentVersions.HeadMajor` -> authoritative `SaveDocumentVersions.HeadPointerMajor` in new recovery test support only;
- first focused run: **522 / 524**, `2` failed;
- both failures came from intentionally unsupported-version fixtures trying to pass through Chronicle's production serializer, which correctly rejects unsupported package-document versions before serialization;
- the fixture was corrected so supported documents continue through Chronicle's serializer while intentionally future-version test JSON is authored directly with Unity `JsonUtility` for runtime inspection;
- runtime implementation, API, architecture, ESV-D-029 authority, recovery behavior, test intent, and discovery count were unchanged;
- final rerun: **524 / 524**.

Still deferred:
- `ExecuteRecoveryAsync` and head rewrite/publication;
- catalog reconciliation after recovery;
- automatic/configured fallback execution;
- recovery mutation admission/Busy/cancellation;
- stale-plan execution rejection beyond captured provenance;
- quarantine movement;
- incomplete-generation cleanup;
- rename/duplicate/delete/trash;
- persistent catalog cache;
- generic queues/capacity/overflow;
- automatic autosave timers;
- permission-provider production wiring;
- full recovery/configuration/Setup expansion;
- scene travel, bridges, service locator, DDOL.

No follow-on M4 checkpoint is activated by this closeout.

## Chronicle ESV-M4-08 Closeout

`ESV-M4-08 — Chronicle Explicit Recovery Execution, Stale-Plan Revalidation, Head Repointing, and Catalog Reconciliation Foundation` is **complete**.

Authority decision: **ESV-D-030**.

Evidence:
- planning baseline: `0396adb`;
- planning/activation commit: `c324aa4`;
- implementation commit: `1985fb0`;
- final effective runtime baseline: `1985fb0`;
- Unity compile/import: **green**;
- final focused `EchoDevGames.EchoSave.Tests.Editor`: **540 / 540 passed, 0 failed**;
- prior **524 / 524** Chronicle regression floor preserved;
- **16** net new focused tests;
- committed implementation/test scope: **18 files**, `1846` insertions, `10` deletions.

Completed boundary:
- public explicit `ExecuteRecoveryAsync(SaveRecoveryPlan, SaveRecoveryCandidate)`;
- one root-local mutation admission lease;
- immediate Busy rejection with no recovery queue;
- fresh M4-07 plan rebuild after admission;
- exact source-provenance stale-plan comparison;
- candidate must still exist and verify in the fresh plan;
- zero mutation before freshness/candidate proof;
- selected committed generation stays immutable;
- only `head.json` is repointed to the selected candidate;
- broken/untrusted source generation is not copied into `previousGenerationId`;
- post-head catalog reconciliation;
- truthful committed-head versus catalog-reconciled result;
- no participant apply/capture/default callbacks;
- no active-slot selection side effect.

Implementation-history note:
- first compilation exposed two new test references to nonexistent `FakeManualSaveTransactionExecutor.CallCount`;
- the established fake exposes `Calls`;
- the references were corrected test-only;
- runtime/API/architecture/ESV-D-030 authority/recovery behavior/test intent/discovery shape were unchanged;
- final focused gate passed **540 / 540**.

Still deferred:
- automatic/configured fallback;
- recovery-on-load;
- quarantine or corrupt-generation cleanup;
- rename/duplicate/delete/trash;
- persistent catalog cache;
- generic queues/capacity/overflow;
- recovery cancellation overload;
- automatic autosave timers;
- permission-provider production wiring;
- full recovery/configuration/Setup authoring;
- scene travel, bridges, service locator, DDOL.

No follow-on M4 checkpoint is activated by this closeout. Further Chronicle implementation requires a new bounded Checkpoint Build Plan and must preserve the **540 / 540** focused regression floor.


## Chronicle ESV-M4-09 Closeout

`ESV-M4-09 — Chronicle Slot Rename, Full-State Duplication, Stable Identity, and Catalog Reconciliation Foundation` is **complete**.

Evidence:
- planning baseline: `07bbd2b`;
- planning/activation commit: `7d2d987`;
- implementation commit: `459023f`;
- final effective runtime baseline: `459023f`;
- Unity compile/import: **green**;
- final focused `EchoDevGames.EchoSave.Tests.Editor`: **562 / 562 passed, 0 failed**;
- prior **540 / 540** Chronicle regression floor preserved;
- **22** net new focused tests;
- committed implementation/test scope: **26 files**, `3100` insertions, `8` deletions;
- base `ISaveStorageBackend` unchanged.

Completed boundary:
- public bounded rename and duplicate operations;
- root-local admission reuse with Busy/no-queue behavior;
- rename stable `SaveSlotId` and stable physical path;
- rename immutable metadata-only generation publication;
- source provenance verification/revalidation;
- expected-current stale-source protection;
- post-rename retention and catalog reconciliation;
- active-slot preservation;
- duplicate canonical capacity enforcement;
- bounded duplicate ID collision retry;
- new duplicate slot/generation identities;
- fully verified source-state copy without participant callbacks;
- source bytes preserved;
- destination head-last durable publication;
- duplicate no-auto-select;
- truthful committed-but-unreconciled catalog failure state;
- ESV-T-019 and ESV-T-020 complete.

Still deferred:
- prepare-delete/confirm-delete;
- trash/trash retention;
- quarantine/incomplete-generation cleanup;
- persistent `catalog.cache.json`;
- automatic/configured recovery fallback;
- recovery-on-load;
- generic operation queues/capacity/overflow;
- recovery cancellation overload;
- automatic autosave timers;
- permission-provider production wiring;
- full configuration/Setup expansion;
- document migration;
- scene travel, peer bridges, service locator, DDOL.

No follow-on M4 checkpoint is activated by this closeout. Any next Chronicle implementation requires a separately bounded Checkpoint Build Plan and must preserve the **562 / 562** focused regression floor.


## Historical — Chronicle ESV-M4-10 Activation

`ESV-M4-10 — Destructive Slot Deletion Planning, Confirmed Trash, and Bounded Trash Retention Foundation` is **active / authorized**.

Authority:
- clean planning baseline: `4d2f2ac`;
- Chronicle specification: **v1.33.0**;
- decision: **ESV-D-032**;
- carried focused regression floor: **562 / 562**.

This is intended as the final bounded runtime-capability slice needed before an M4 milestone reconciliation, subject to implementation evidence and a post-checkpoint audit.

M4-10 owns:
- read-only `PrepareDeleteSlotAsync`;
- immutable package/session/source-provenance-bound `SaveDeletionPlan`;
- bounded expiry and one-use confirmation truth;
- zero mutation during prepare-delete;
- root-local admitted `ConfirmDeleteSlotAsync`;
- immediate Busy rejection and no delete queue;
- fresh exact-source revalidation before destructive mutation;
- recoverable package-owned trash as the safe durable delete boundary;
- active-slot clearing only after durable removal;
- post-delete catalog reconciliation;
- bounded post-commit trash retention;
- ESV-T-021 / ESV-T-022 / ESV-T-023;
- zero participant callbacks;
- unchanged base `ISaveStorageBackend`.

Still deferred:
- permanent erase API;
- public restore-from-trash API;
- quarantine/incomplete cleanup;
- persistent catalog cache;
- automatic/configured recovery fallback;
- recovery-on-load;
- generic operation queues;
- recovery cancellation overload;
- automatic autosave timers/gameplay triggers;
- permission-provider production wiring;
- full configuration/Setup authoring;
- M5 Editor tools and Save Laboratory;
- scene travel, peer bridges, service locator, DDOL.

M4 is **not** marked complete merely by activating this checkpoint. After M4-10 implementation/closeout, perform a dedicated M4 milestone reconciliation against CAP-002 through CAP-018 and the applicable registry before advancing to M5.

## Chronicle ESV-M4-10 Closeout

`ESV-M4-10 — Destructive Slot Deletion Planning, Confirmed Trash, and Bounded Trash Retention Foundation` is **complete**.

Evidence:
- planning/activation commit: `2244e3c`;
- implementation commit: `01e4cdd`;
- final focused `EchoDevGames.EchoSave.Tests.Editor`: **587 / 587 passed, 0 failed**;
- prior focused floor: **562 / 562**;
- net new focused tests: **25**;
- committed implementation/test scope: **28 files**, `2863` insertions, `6` deletions.

Completed runtime truth:
- read-only zero-mutation `PrepareDeleteSlotAsync`;
- immutable expiring one-use package/session/source-bound plan truth;
- root-local admitted `ConfirmDeleteSlotAsync`;
- Busy/no-queue behavior;
- fresh exact-source revalidation before destructive mutation;
- recoverable trash complete-tree publication as durable delete truth;
- active-slot clearing only after durable removal;
- truthful live-catalog reconciliation;
- bounded fail-closed post-commit trash retention;
- ESV-T-021 through ESV-T-023 completed;
- zero participant callbacks;
- unchanged base `ISaveStorageBackend`.

M4 is **not** declared complete by this closeout.

No M5 implementation is active.

The next required work is the dedicated **M4 milestone reconciliation**.


## Chronicle M4 Milestone Reconciliation Audit + R1 Activation

The M4 audit against clean repository baseline `48454ea` found four material completion gaps:

1. **A-01 — public load facade incomplete:** M3 prepared-load/apply machinery exists, but `IEchoSaveService` does not yet expose the promised prepare/apply/convenience load flow.
2. **A-02 — public catalog/create/select facade incomplete:** M4 catalog, technical creation, and active-selection machinery exist, but the primary service does not expose the promised consumer operations.
3. **A-03 — CAP-002 slot-policy configuration incomplete:** runtime configuration is still schema 1 and does not own the approved single/fixed/configurable/bounded-unlimited policy model.
4. **A-04 — CAP-014 package-document migration incomplete:** participant migration exists; package-document migration remains deferred.

Approved disposition:
- preserve the existing MVP promises;
- do not weaken CAP-002/CAP-012/CAP-013/CAP-014/CAP-017 merely to close M4;
- execute the repair in bounded reconciliation passes;
- **R1:** public runtime composition;
- **R2:** slot-policy runtime configuration;
- **R3:** package-document migration, preserving CAP-014 intact;
- final pass: test-registry/document evidence reconciliation;
- only then may M4 close and M5 activate.

**ESV-M4-R1 is now active / authorized.**

Authority:
- clean planning baseline: `48454ea`;
- Chronicle specification: **v1.35.0**;
- decision: **ESV-D-033**;
- carried focused regression floor: **587 / 587**.

R1 owns only:
- participant registration facade;
- catalog snapshot/refresh facade;
- public slot create/select facade;
- prepared-load/apply facade;
- same-scene convenience load;
- consumer-facing facade DTO/results needed for those operations;
- focused composition tests.

R1 explicitly does not own configuration schema expansion, slot-policy implementation, package-document migration, M5 tools, automatic recovery fallback, persistent catalog cache, generic queues, scene travel, peer bridges, or DDOL.

## Suite Distribution Kit Standard

Jesse approved one new suite-wide graduation rule after First Light FL-M6-01 closeout:

> Every independently distributed Sperk's Forge package gets a repository-owned, versioned Distribution Kit containing the exact package artifact plus a complete user handout, distribution manifest, SHA-256 integrity record, and build record.

The preferred repository layout is:

```text
Distributions/
├── README.md
├── _Template/
│   ├── COMPLETE_USER_HANDOUT_TEMPLATE.md
│   └── DISTRIBUTION_MANIFEST_TEMPLATE.md
└── <Public Title>/
    └── <Package Version>/
        ├── README.md
        ├── <package-artifact>.tgz
        ├── <COMPLETE_USER_HANDOUT>.md
        ├── DISTRIBUTION_MANIFEST.md
        ├── DISTRIBUTION_BUILD_RECORD.txt
        └── SHA256SUMS.txt
```

Authority is promoted into SFGSS-000 v0.25.0, SFGSS-001 v1.4.0, and SFGSS-004 v1.4.0.

**Honesty rule:** creating a tarball and kit does not prove the tarball route. The artifact is now available for distribution/evaluation, while external clean-project tarball installation, removal/reinstall, player builds, performance, tags/catalog, and release/private-beta qualification remain future evidence gates.

First Light is the first package to receive the standard kit at:

```text
Distributions/First Light/0.1.0/
```

The complete First Light handout is also included in package documentation at:

```text
Packages/com.echodevgames.echo-launch/Documentation~/User/Complete User Handout.md
```

## Permanent First Light Gallery

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
├── First Light Example/
│   └── First Light Splashs/
│       ├── Art/
│       ├── Audio/
│       ├── Configuration/
│       ├── Prefabs/
│       └── Scenes/
└── UMBRA Example/
    └── UMBRA Splashs/
        ├── Art/
        ├── Audio/
        ├── Configuration/
        ├── Prefabs/
        └── Scenes/
```

The Gallery is project-owned consumer/showcase content. It is not package content, a UPM sample, a required dependency, or evidence that The Sperk’s Forge is an Isekai Studios product.

### First Light Example

The canonical in-house example demonstrates the normal First Light happy path:

```text
FirstLight_Showcase_Boot
→ EchoDevGames splash
→ First Light splash
→ startup settles
→ destination validates and loads
→ FirstLight_Showcase_MainMenu
```

It uses normal public Setup, project-owned configuration, normal Inspector authoring, stable splash identities, optional stored audio intent, and the public uGUI presentation path.

### UMBRA Example

The second Gallery example is the former Slice E proof promoted into a permanent consumer example. It proves the package is not secretly tied to the canonical First Light branding or foundation.

Observed proof:

- `Foundation > Asset Resolution = Create Project-Owned Setup`;
- fresh requested Configuration, LaunchDestination, SplashSequence, StartupSequence, RootPrefab, and Boot scene planned as `Create`;
- explicitly selected existing destination scene remained `Reuse`;
- first Apply created the requested project-owned foundation;
- generated SplashSequence retained **3** authored entries: `The Sperk`, `Isekai Studios`, and `UMBRA`;
- each generated entry received a non-empty stable ID;
- project-owned images, optional `PreferredAudioClip` intent, timings, motion, and advancement settings serialized correctly;
- presentation used Splash Only, black background, and advancement enabled;
- the Isekai entry retained Pulse (`1.05` maximum scale, `2.5` second cycle);
- the generated Boot experience played successfully and looked correct;
- identical second Preview reused the requested local targets;
- identical second Apply returned **`NoChanges`** with **Created paths: None** and unchanged Build Settings evidence.

Audio remains intent metadata only. First Light does not own audio playback.

## FL-M6-01 Defects Resolved by the Real Consumer Workflow

### H1 — Splash Entry Authoring Identity

Normal Inspector authoring originally left hidden blank `SplashEntry.entryId` values and runtime correctly blocked with `ELAUNCH-SPLASH-001`.

Resolved result:

- Editor-only blank-ID generation;
- existing non-empty IDs preserved;
- no Runtime rewrite;
- no schema bump;
- focused gate **5 / 5**;
- public Inspector workflow advanced beyond `ELAUNCH-SPLASH-001`.

### H2 — Destination Build Settings Conformance

Setup originally reported success while the configured destination was absent from Build Settings and runtime correctly blocked with `ELAUNCH-DEST-001`.

Resolved result:

- Setup ensures Boot and configured destination are enabled exactly once;
- unrelated Build Settings order remains preserved;
- repeat Apply settles `NoChanges`;
- focused H2 gate **35 / 35**;
- public Boot → splashes → destination path succeeded without manual Build Settings editing.

### A1 / A1-E1 — Presentation and Independent Setup Authoring

The Showcase then justified and proved:

- Splash Only / Splash + Status;
- project-owned background color;
- Allow Advancement;
- None / Pulse motion;
- Automatic / Skippable After Minimum / Wait For Input After Minimum;
- normal Inspector authoring;
- Setup creation-time authoring for newly-created sequences;
- backward-compatible `Reuse Compatible Assets`;
- explicit `Create Project-Owned Setup` for an independent requested foundation;
- request/plan freshness participation and repeat-safe convergence.

Schemas remain unchanged: SplashSequence schema `1`, EchoLaunchConfiguration schema `4`.

## Acceptance

FL-M6-01 acceptance is complete for the in-repository Package Reference Showcase stage.

```text
Learning / authority
→ implementation
→ Standalone Test Lab                PASS
→ Package Reference Showcase         PASS
→ clean-project reproduction         NOT RUN in this closeout
→ release qualification              NOT RUN
→ private beta / external adoption   NOT RUN
```

The permanent Gallery now supplies two consumer examples that can be extended later without widening First Light package authority.

## Evidence Boundary

This closeout does **not** claim:

- a post-A1 complete EditMode aggregate;
- a post-A1 complete Runtime Play Mode aggregate;
- clean-project reproduction;
- Git URL, tag, registry, or public-package installation support;
- clean-project **tarball installation support** (the versioned `0.1.0` tarball artifact is now prepared, but the route is not yet qualified);
- player-build qualification;
- performance qualification;
- release tag/catalog readiness;
- private beta or external adoption;
- First Light-owned audio playback.

Those remain future release-qualification work if/when First Light returns to the release queue.

## Stop Point

**First Light implementation and in-repository Gallery work are frozen for this pass.**

Do not begin FL-M6-02 automatically. Do not add more First Light features merely because the Gallery can host more examples.

## ESV-M4-R1 Closeout

ESV-M4-R1 is **complete**.

Evidence:
- planning/activation commit: `bdb0c00`;
- implementation commit: `ab18361`;
- focused Chronicle Editor: **618 / 618 passed, 0 failed**;
- prior focused floor: **587 / 587**;
- net new focused tests: **31**;
- implementation/test scope: **29 files**, `2995` insertions, `18` deletions.

R1 closed audit gaps A-01 and A-02 by composing the existing participant, catalog, slot-create/select, prepared-load, and apply authorities through the public Chronicle service.

No R1 hotfix was required.

## ESV-M4-R2 Closeout

ESV-M4-R2 is **complete**.

Evidence:
- planning baseline: `176b240`;
- planning/activation commit: `428369e`;
- implementation commit: `8a8e7e7`;
- Chronicle specification closeout: **v1.38.0**;
- focused Chronicle Editor: **636 / 636 passed, 0 failed**;
- incoming focused floor: **618 / 618**;
- net new focused R2 tests: **18**;
- implementation/test scope: **8 files**, `768` insertions, `13` deletions.

R2 closes audit blocker **A-03 / CAP-002**. `EchoSaveConfiguration` schema 2 now owns project-authored slot policy; `SingleSlot`, `FixedMultiSlot`, `ConfigurableMultiSlot`, and `BoundedProfiles` each resolve to one finite immutable service-session capacity. Create and duplicate consume that same resolved capacity. Schema-1 configurations remain readable at historical capacity 64 without runtime asset mutation.

One pre-commit compile compatibility correction restored `EchoSaveService.DefaultTechnicalSlotCapacity` as an internal alias to the legacy schema-1 value because an existing regression test still referenced the symbol. It does **not** drive schema-2 behavior; final Unity compile and **636 / 636** evidence are authoritative.

ESV-T-015 through ESV-T-018 are now complete. Degraded canonical live slots continue to count, trash does not count, and confirmed deletion frees capacity only through the existing durable/catalog truth.

R2 does not add fixed-slot auto-provisioning, runtime template identity, document migration, M5 tooling, runtime policy mutation, generic queues, scene travel, or DDOL.

## ESV-M4-R3 Activation

ESV-M4-R3 is **active / authorized**.

Authority:
- clean planning baseline: `0ebf1a1`;
- incoming focused Chronicle regression floor: **636 / 636 passed, 0 failed**;
- Chronicle specification: **v1.39.0**;
- decision: **ESV-D-035**;
- audit target: **A-04 / CAP-014 package-document migration**.

R3 preserves CAP-014 intact and adds the missing Chronicle-owned package-document half of migration. Package-document migration runs at read time on detached in-memory serialized content, follows deterministic contiguous exact-version chains for each document kind, reaches the exact current package-document version before current DTO validation, and never mutates the source generation.

Package-document migration remains separate from participant migration. Chronicle owns package-document migration steps because Chronicle owns the package document formats; consumers do not register arbitrary package-document migration steps. Existing participant migration contracts and registration remain unchanged.

Current production package-document versions remain `1.0.0` for envelope, manifest, payload, and head pointer. R3 does **not** create a meaningless package-format bump solely to prove the engine. Focused tests may use internal deterministic fixture steps while production built-in steps remain empty until a real package document shape change requires one.

R3 must fail closed for missing/ambiguous chains, step failures, invalid outputs, or unsupported newer package versions. Successful read-time migration does not rewrite/republish the source; the next separately authorized save writes current-format documents through the existing immutable-generation/head-last publication path.

R3 directly targets ESV-T-067, ESV-T-068, ESV-T-069, the document side of ESV-T-072, and ESV-T-073. Existing participant migration evidence for ESV-T-070/071 remains separate. Final 100-case registry/document evidence reconciliation remains mandatory after R3.

M4 remains open. M5 remains locked.

## ESV-M4-R3 Closeout

ESV-M4-R3 is **complete**.

Evidence:
- planning baseline: `0ebf1a1`;
- planning/activation commit: `2dcae91`;
- implementation commit: `c6ba1ad`;
- Chronicle specification closeout: **v1.40.0**;
- focused Chronicle Editor: **660 / 660 passed, 0 failed**;
- incoming focused floor: **636 / 636**;
- net new focused R3 tests: **24**;
- implementation/test scope: **17 files**, `3359` insertions, `31` deletions.

R3 closes audit blocker **A-04 / CAP-014** while preserving CAP-014 intact. Chronicle now owns an internal package-document migration engine that probes exact package document versions, resolves deterministic contiguous package-owned chains, migrates detached serialized text in memory, and reaches the exact current package version before existing strict DTO validation. Missing/ambiguous/failed/invalid/newer paths fail closed and source generations remain immutable.

Package-document migration remains separate from participant migration. The production envelope, manifest, payload, and head-pointer versions remain `1.0.0`; no fictional production format version was created for tests, and the production migration-step registry remains empty until a real package document change requires a step.

The migration-aware read seam is integrated into current-generation loading, catalog scanning, and recovery candidate verification. R3-owned ESV-T-067, ESV-T-068, ESV-T-069, ESV-T-072, and ESV-T-073 now have retained green evidence.

## ESV-M4-R4 Activation

ESV-M4-R4 — **Final 100-Case Registry, Documentation Evidence Reconciliation, and M4 Closeout** — is **active / authorized**.

Authority:
- clean planning baseline: `e3d7a2e`;
- R3 implementation baseline retained: `c6ba1ad`;
- incoming focused Chronicle regression floor: **660 / 660 passed, 0 failed**;
- Chronicle specification: **v1.41.0**;
- decision: **ESV-D-036**.

R4 is a documentation/evidence reconciliation checkpoint, not a runtime feature checkpoint.

It must review **ESV-T-001 through ESV-T-100 individually**. A row becomes Complete only from retained direct evidence. Rows belonging to later M5 tooling/Laboratory, clean-project, release qualification, performance/stress, integration/adoption, or other deferred work stay explicitly not complete and are not mass-promoted to make M4 look finished.

R4 authorizes no runtime or test-code changes. If an M4-applicable row lacks retained proof, M4 closeout stops and that gap receives a separate bounded repair checkpoint.

R4 must also synchronize stale current-state documentation, including the package README, CHANGELOG, package documentation index/navigation, Current Notes, Suite Health, milestone audit, package specification, checkpoint records where needed, and handoff truth.

## Next Action

1. Treat `e3d7a2e` as the clean R4 planning baseline.
2. Preserve **660 / 660** as the focused Chronicle regression floor.
3. Build the row-by-row ESV-T-001 through ESV-T-100 evidence/disposition map.
4. Repair stale registry statuses only where retained direct evidence supports the change.
5. Leave later M5/release/adoption rows explicitly not complete.
6. Reconcile README/CHANGELOG/index/current-state documentation against the final map.
7. If any M4-applicable row lacks proof, stop and open a bounded repair checkpoint rather than editing runtime/tests inside R4.
8. Rerun the focused Chronicle Editor suite and record the actual closing total.
9. Declare M4 complete only from a committed R4 closeout with zero unresolved M4-applicable gaps.
10. Keep M5 locked until that closeout commit.

## ESV-M4-R4 Evidence Reconciliation

**Activation commit:** `81c53dd`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.42.0 / ESV-D-036
**Incoming focused Chronicle floor:** **660 / 660 passed, 0 failed**
**Row-map result:** **61 Complete / 39 Deferred / 0 Blocked**

Every ESV-T-001 through ESV-T-100 row has now been classified individually. Complete rows have retained direct M1-M4/R1-R3 evidence. Deferred rows are owned by later M5 Laboratory/Setup, persistent-cache/fault-injection, clean-project/distribution, performance/stress, integration/adoption, or release gates. No M4-applicable evidence gap was found, so no R4 repair checkpoint is required.

Current-state documentation parity is reconciled across the package README, CHANGELOG, documentation index, root/package Current Notes, Suite Health, milestone audit, R4 plan, package specification, and the dedicated 100-case evidence matrix.

R4 has changed documentation/evidence only. No runtime or test-code change is authorized or included.

**Final R4 evidence:** fresh focused Chronicle Editor **660 / 660 passed, 0 failed**. The 100-case map remains **61 Complete / 39 Deferred / 0 Blocked**. **M4 is complete. M5 is eligible for separate activation and is not active yet.**

## ESV-M4-R4 / M4 Closeout

**Activation commit:** `81c53dd`
**Final authority:** SFGSS-PKG-ECHOSAVE-001 v1.43.0 / ESV-D-036
**Final registry:** **61 Complete / 39 Deferred / 0 Blocked**
**Fresh final focused Chronicle Editor:** **660 / 660 passed, 0 failed**
**Runtime/test-code changes in R4:** none
**M4 status:** **Complete**
**Next milestone:** M5 is eligible for a separate activation checkpoint; it is not automatically active.

The 39 Deferred registry rows remain explicit later-gate obligations. M4 completion does not waive M5 Laboratory/Setup, persistent-cache/fault-injection, clean-project/distribution, performance/stress, integration/adoption, or release qualification.

## ESV-M5-01 Activation

**Planning baseline:** `e63d83f` — `Close out ESV-M4-R4 and Chronicle M4`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.44.0 / ESV-D-037
**Incoming focused Chronicle floor:** **660 / 660 passed, 0 failed**
**M4:** Complete
**M5:** Active through bounded checkpoint M5-01 only

M5-01 establishes the Editor-only tooling boundary before broader authoring/inspection/simulation work. It owns the Editor asmdef, non-mutating Setup preview, create-only current-schema configuration authoring, and an initial non-destructive Validator for currently representable path/configuration/root/assembly truth.

It does not authorize runtime save-semantic changes, configuration schema expansion, provider/retention/recovery authoring, Browser/Inspector/Simulator tooling, sandbox/Laboratory content, persistent cache, cleanup/destructive expansion, scene travel, peer bridges, or DDOL.

The prior **61 Complete / 39 Deferred / 0 Blocked** registry map remains authoritative. Deferred rows stay Deferred until their exact later gate produces direct evidence.
