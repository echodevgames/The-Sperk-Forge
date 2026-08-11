# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 10, 2026
**Current focus:** Chronicle M4 — ESV-M4-06 complete; next bounded checkpoint planning
**Current checkpoint:** None activated — ESV-M4-06 is **complete**

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

## Next Action
1. Rehydrate exact `e714a90`.
2. Preserve the focused **497 / 497** Chronicle regression floor.
3. Treat ESV-M4-06 bounded retention and post-publication cleanup as complete.
4. Define and activate the next bounded M4 Checkpoint Build Plan before writing further runtime code.
5. Keep recovery execution/quarantine, rename/duplicate/delete/trash, persistent catalog cache, generic queues, automatic autosave timers, permission-provider production wiring, full configuration/Setup expansion, scene travel, peer bridges, service locator, and DDOL locked until separately authorized.
