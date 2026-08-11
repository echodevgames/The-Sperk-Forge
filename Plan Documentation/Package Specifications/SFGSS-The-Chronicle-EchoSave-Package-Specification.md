# The Chronicle – Save Infrastructure Package Specification

**Working document ID:** SFGSS-PKG-ECHOSAVE-001
**Specification version:** 1.36.0
**Status:** Approved
**Technical package name:** EchoSave
**Public title:** The Chronicle – Save Infrastructure
**Package ID:** `com.echodevgames.echo-save`
**Runtime namespace:** `EchoDevGames.EchoSave`
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Project boundary:** Independent solo project; not an Isekai Studios product
**Planned repository:** `EchoDevGames/EchoSave`
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`
**Unity baseline:** Unity 6000.3.8f1
**Minimum supported Unity version:** Unity 6000.0
**Default local-storage root:** A configured child directory beneath `Application.persistentDataPath`
**Default serializer:** Package-owned `UnityJsonSaveSerializer` using Unity `JsonUtility` for package envelopes and plain serializable DTOs, with explicit documented limitations and replaceable serializer providers
**Parent authority:** SFGSS-000 and SFGSS-001
**Last updated:** August 11, 2026

> “Let what must endure be recorded without chaining the game to the record.”

> **Approval rule:** This specification is approved as package authority. ESV-M4-R1 is complete at `ab18361` with focused Chronicle Editor evidence **618 / 618 passed, 0 failed**, preserving the prior **587 / 587** floor. R1 closes M4 audit gaps A-01/A-02 by composing participant registration, catalog snapshot/refresh, slot create/select, prepared-load creation/apply, and same-scene convenience load through `IEchoSaveService` over the existing M3/M4 authorities. R2 slot-policy runtime configuration is the next reconciliation gate but is not active yet. R3 package-document migration and final registry/document reconciliation remain mandatory before M4 can close. M5 remains locked.
>
> **v1.2.0 lifetime reconciliation:** SFGSS-ADR-006 clarifies that EchoSave durable transport, participant runtime truth, and Unity scene-surviving object lifetime are separate concerns. EchoSave may own a duplicate-safe package-local application-session root, but it does not own project-wide service composition or become a universal service locator.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-03 | Proposed | Initial complete specification based on SFGSS-000 v0.6.0, SFGSS-001 v1.1.0, and eight approved Foundation specifications | Pending |
| 1.0.0 | 2026-08-03 | Approved | Approved slot models, immutable save generations, participant payloads, two-phase load, metadata, serialization, migration, recovery, autosave, diagnostics, tooling, and the isolated Save Laboratory | Jesse “Echo” Adams |
| 1.1.0 | 2026-08-04 | Approved | Clarified Unity asset GUID versus optional runtime/export save-configuration identity. Also normalized registry metadata and evidence interpretation. | Jesse “Echo” Adams |
| 1.2.0 | 2026-08-09 | Approved | Reconciled SFGSS-ADR-006: durable save transport, participant runtime truth, and Unity object lifetime remain separate; Chronicle root authority is package-local; cross-package persistence stays optional; PKG-LEARN-009 becomes the active implementation gate. | Jesse “Echo” Adams |
| 1.3.0 | 2026-08-09 | Approved | Reconciled completed PKG-LEARN-009 and ESV-M1-01 evidence; recorded commits `ecfa922` / `2c70b1d`; activated bounded ESV-M2-01 storage-root, path-safety, and local-backend work without changing ownership boundaries. | Jesse “Echo” Adams |
| 1.4.0 | 2026-08-09 | Approved | Closed ESV-M2-01 at `e4ef76c` with focused Chronicle Editor evidence `40 / 40`; activated bounded ESV-M2-02 package-document and default Unity JSON serializer work while deferring physical generation publication, slots, participants, and recovery. | Jesse “Echo” Adams |
| 1.5.0 | 2026-08-09 | Approved | Closed ESV-M2-02 at `6404037` with focused Chronicle Editor evidence `57 / 57`; activated bounded ESV-M2-03 generation/slot technical IDs, package commit-document contracts, and SHA-256 integrity prerequisites while keeping physical generation/head publication separate. | Jesse “Echo” Adams |
| 1.6.0 | 2026-08-09 | Approved | Closed ESV-M2-03 at `ad3b646` with focused Chronicle Editor evidence `87 / 87`; activated bounded ESV-M2-04 physical immutable-generation publication and head-last commit behavior for the package-owned transport path while keeping slots, participants, recovery, retention, autosave, and prepared loads separate. | Jesse “Echo” Adams |
| 1.7.0 | 2026-08-09 | Approved | Closed ESV-M2-04 at `01b7ad3` with focused Chronicle Editor evidence `102 / 102`; marked bounded M2 document/storage core complete and activated ESV-M3-01 participant IDs, descriptors, registration lifetime, and duplicate-safe deterministic registry behavior without capture/save/load/apply orchestration. | Jesse “Echo” Adams |
| 1.8.0 | 2026-08-09 | Approved | Closed ESV-M3-01 at `b3b5f9f` with focused Chronicle Editor evidence `147 / 147`; locked the open-ended no-central-participant-catalog invariant and activated ESV-M3-02 deterministic detached capture, live-registration DTO type routing, serializer resolution, and in-memory payload-entry construction without physical generation publication. | Jesse “Echo” Adams |
| 1.9.0 | 2026-08-09 | Approved | Closed ESV-M3-02 at `e34d6d7` with focused Chronicle Editor evidence `171 / 171`; activated ESV-M3-03 publication-boundary revalidation and participant-bearing immutable-generation/head-last integration while keeping production SaveAsync, unknown-payload preservation, loading/apply, migration, slots, recovery, retention, and autosave separate. | Jesse “Echo” Adams |
| 1.10.0 | 2026-08-09 | Approved | Closed ESV-M3-03 at `6970127` with focused Chronicle Editor evidence `197 / 197`; activated ESV-M3-04 read-only current-generation validation and opaque unknown-payload session preservation while keeping carry-forward publication, prepared loads/apply, production SaveAsync admission, migrations, slots, recovery, retention, and autosave separate. | Jesse “Echo” Adams |
| 1.11.0 | 2026-08-09 | Approved | Closed ESV-M3-04 at `aa78e07` with focused Chronicle Editor evidence `218 / 218`; activated ESV-M3-05 source-provenance/freshness, collision-safe fresh+unknown merge, and opaque unknown-payload carry-forward through immutable generation/head-last publication while keeping prune policy, participant apply, production save admission, migrations, slots, recovery, retention, and autosave separate. | Jesse “Echo” Adams |
| 1.12.0 | 2026-08-09 | Approved | Closed ESV-M3-05 at `af28c96` with focused Chronicle Editor evidence `243 / 243`; activated ESV-M3-06 current-version known participant payload preparation, trusted live runtime-Type deserialization, and all-or-nothing prepared participant batching while keeping migrations, prepared-load lifecycle, participant apply, production operation admission, slots, recovery, retention, and autosave separate. | Jesse “Echo” Adams |
| 1.13.0 | 2026-08-09 | Approved | Closed ESV-M3-06 at `050bfa0` with focused Chronicle Editor evidence `261 / 261`; activated ESV-M3-07 explicit participant migration-step contracts, duplicate-safe migration registration, deterministic contiguous in-memory chain execution, migration provenance, and older-known-payload integration into current-version preparation while keeping document migration, prepared-load lifecycle, participant apply, production operation admission, slots, recovery, retention, and autosave separate. | Jesse “Echo” Adams |
| 1.14.0 | 2026-08-09 | Approved | Closed ESV-M3-07 at `d96936f` with focused Chronicle Editor evidence `294 / 294`; activated ESV-M3-08 bounded public disposable prepared-load handle lifecycle, exact source-provenance binding, package/session ownership, expiry/disposal/invalidation, opaque unknown snapshot binding, and live-handle resource admission while keeping participant apply, document migration, production operation admission, scene travel, slots, recovery, retention, and autosave separate. | Jesse “Echo” Adams |
| 1.15.0 | 2026-08-10 | Approved | Closed ESV-M3-08 at `798d38d` with focused Chronicle Editor evidence `332 / 332`; activated ESV-M3-09 deterministic participant apply/missing-payload policy and approved additive optional `ISaveDefaultableParticipant.InitializeDefault()` while keeping base `ISaveParticipant` unchanged, rejecting `Apply(null)` default semantics, and deferring rollback/compensation, production operation admission, scene travel, document migration, slots, recovery, retention, and autosave. | Jesse “Echo” Adams |
| 1.16.0 | 2026-08-10 | Approved | Closed ESV-M3-09 at `568fa3a` with focused Chronicle Editor evidence `366 / 366`, completing M3 Participants and Loading; activated ESV-M4-01 provider-neutral slot discovery, payload-free head/current-manifest metadata rebuild, deterministic catalog snapshots, and session-only active-slot selection; added ESV-D-023 keeping base `ISaveStorageBackend` unchanged while catalog discovery uses an additive optional provider capability. | Jesse “Echo” Adams |
| 1.17.0 | 2026-08-10 | Approved | Closed ESV-M4-01 at `62e8a54` with focused Chronicle Editor evidence `403 / 403`; activated ESV-M4-02 bounded technical slot creation, capacity enforcement, initial empty immutable-generation/head-last bootstrap, and post-publication catalog reconciliation; added ESV-D-024 defining successful slot creation as a committed generation rather than directory existence and counting degraded canonical slots toward capacity. | Jesse “Echo” Adams |
| 1.18.0 | 2026-08-10 | Approved | Closed ESV-M4-02 at implementation commit `d8d5c18` with focused Chronicle Editor evidence `425 / 425`; recorded bounded technical slot creation, capacity enforcement, initial empty committed generation, head-last publication, and truthful catalog reconciliation as complete; left the next M4 checkpoint intentionally unactivated pending a bounded Checkpoint Build Plan. | Jesse “Echo” Adams |
| 1.19.0 | 2026-08-10 | Approved | Activated ESV-M4-03 bounded manual-save transaction composition at clean baseline `a3eba25`: active-slot target resolution, healthy catalog preflight, current-generation/unknown-payload provenance refresh, fresh participant capture, collision-safe known+unknown merge, expected-current-generation head-last publication, and truthful catalog reconciliation; public `SaveAsync`, production operation admission/Busy semantics, autosave, retention, recovery, and unrelated slot operations remain deferred. | Jesse “Echo” Adams |
| 1.20.0 | 2026-08-10 | Approved | Closed ESV-M4-03 at implementation commit `c8ea742` with focused Chronicle Editor evidence `439 / 439`; recorded active-slot manual-save transaction composition, exact source provenance, fresh known capture, opaque unknown carry-forward, stale-source rejection, head-last publication, display-name preservation, catalog reconciliation, and truthful partial durable/head/catalog outcomes as complete; left public `SaveAsync`, operation admission/Busy/cancellation, autosave, retention, recovery, and the next M4 checkpoint deferred. | Jesse “Echo” Adams |
| 1.21.0 | 2026-08-10 | Approved | Activated ESV-M4-04 public manual-save admission foundation at clean baseline `3a84187`: public active-slot `SaveAsync`, public request/result truth, one root-local mutating-operation admission authority, immediate Busy rejection for overlapping manual saves, bounded pre-publication cancellation/Too-Late semantics, and shutdown admission closure; autosave/coalescing, generic queued multi-operation policy, permission-provider facade wiring, retention, recovery, rename/duplicate/delete, persistent catalog cache, and full slot-policy expansion remain deferred. | Jesse “Echo” Adams |
| 1.22.0 | 2026-08-10 | Approved | Closed ESV-M4-04: public active-slot `SaveAsync`, public request/result truth, one root-local mutating-operation admission authority, immediate Busy rejection with no queue, safe pre-publication cancellation, Too-Late truth after durable publication begins, and shutdown admission closure. Implementation committed at `2732aaa`; pre-Ready lifecycle-status hotfix committed at `09ae8f1`; final focused Chronicle Editor gate `456 / 456`, preserving the prior `439 / 439` floor. No follow-on M4 checkpoint activated. | Jesse “Echo” Adams |
| 1.23.0 | 2026-08-10 | Approved | Activated ESV-M4-05 bounded autosave request/coalescing foundation at clean baseline `9a2ad29`: explicit caller-triggered `RequestAutosave`, at-most-one pending latest request, deterministic coalesced/superseded/rejected submission truth, reuse of M4-04 root-local admission and M4-03 durable save transaction, and shutdown-safe pending-request settlement; generic queued multi-operation scheduling, retention, recovery, rename/duplicate/delete, persistent catalog cache, and full slot-policy/configuration expansion remain deferred. | Jesse “Echo” Adams |
| 1.24.0 | 2026-08-10 | Approved | Closed ESV-M4-05 at implementation commit `9917f1b` with focused Chronicle Editor evidence `473 / 473`; recorded explicit caller-triggered `RequestAutosave`, at-most-one latest-wins pending request, deterministic coalescing/supersession, reuse of the M4-04 root-local admission authority and M4-03/M4-04 durable active-slot save path, manual-save Busy preservation, pending drain after admission release, and shutdown-safe pending settlement as complete. Updated the stale M4-04 public-surface regression assertion to the newly authorized autosave API. Retention, generic queued multi-operation scheduling, recovery, rename/duplicate/delete, persistent catalog cache, and full slot-policy/configuration expansion remain deferred; no follow-on M4 checkpoint activated. | Jesse “Echo” Adams |
| 1.25.0 | 2026-08-10 | Approved | Activated ESV-M4-06 bounded generation-retention and post-publication cleanup foundation at clean baseline `3cdad0f`: project-owned `SaveRetentionPolicy`, bounded provider-neutral generation discovery, additive optional tree-deletion capability without changing base `ISaveStorageBackend`, deterministic safe cleanup of excess verified committed generations only after a new head is authoritative, protection of current and immediate recovery generations, and truthful cleanup-failure diagnostics that never fabricate save rollback; recovery execution, destructive slot operations/trash, persistent catalog cache, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred. | Jesse “Echo” Adams |
| 1.26.0 | 2026-08-10 | Approved | Closed ESV-M4-06 at implementation commit `e714a90` with focused Chronicle Editor evidence `497 / 497`; recorded bounded project-owned `SaveRetentionPolicy`, provider-neutral generation discovery, additive optional `ISaveStorageTreeDeletionBackend`, current/immediate-predecessor protection, deterministic oldest-first deletion of excess verified committed history, shared manual/autosave post-publication retention, and truthful maintenance failure without fabricated save rollback. The first focused run exposed two new integration-test setup failures because no participant was registered; a test-only correction made all three integration cases reach their intended publication/retention boundary. Recovery execution/quarantine, destructive slot operations, persistent catalog cache, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred; no follow-on M4 checkpoint activated. | Jesse “Echo” Adams |
| 1.27.0 | 2026-08-10 | Approved | Activated ESV-M4-07 bounded read-only recovery planning at clean baseline `9695450`: provider-neutral bounded generation discovery, full manifest/payload/integrity verification for recovery candidates, deterministic newest-valid ordering, missing/invalid-head and corrupt-current plan truth, immutable plan provenance for later stale-plan rejection, and zero durable mutation during planning. Recovery execution/head rewrite/catalog reconciliation, automatic fallback, quarantine, destructive slot operations, persistent catalog cache, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred. | Jesse “Echo” Adams |
| 1.28.0 | 2026-08-10 | Approved | Closed ESV-M4-07 at implementation commit `9f68555` with focused Chronicle Editor evidence `524 / 524`; recorded public read-only `BuildRecoveryPlanAsync(SaveSlotId)`, explicit head/current diagnosis, bounded provider-neutral generation discovery, full manifest/payload/integrity candidate verification, deterministic newest-valid ordering, immutable payload-free recovery-plan/candidate truth, and technical source-provenance fingerprinting with zero durable mutation. Two test-only fixture corrections were required: the head version constant was corrected to `HeadPointerMajor`, and intentionally unsupported future-version fixture JSON was authored outside the production serializer so that the runtime could inspect and preserve/exclude it. Recovery execution/head rewrite/catalog reconciliation, automatic fallback, quarantine, destructive slot operations, persistent catalog cache, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred; no follow-on M4 checkpoint activated. | Jesse “Echo” Adams |
| 1.29.0 | 2026-08-10 | Approved | Activated ESV-M4-08 bounded explicit recovery execution at clean baseline `0396adb`: public `ExecuteRecoveryAsync` over M4-07 plans, reuse of the root-local mutating-operation admission authority, exact source-provenance and candidate revalidation before any head mutation, head repointing to one already verified committed generation without rewriting generation contents, stale-plan rejection, and truthful post-head catalog reconciliation. Automatic/configured fallback, quarantine, destructive slot operations, persistent catalog cache, generic queues, automatic autosave timers, and broader recovery/configuration tooling remain deferred. | Jesse “Echo” Adams |
| 1.30.0 | 2026-08-10 | Approved | Closed ESV-M4-08 at implementation commit `1985fb0` with focused Chronicle Editor evidence `540 / 540`; recorded explicit public recovery execution, root-local Busy/no-queue admission reuse, fresh M4-07 provenance/candidate revalidation, stale-plan rejection before mutation, `head.json`-only repointing to an already verified immutable generation, selected-generation byte immutability, and truthful post-head catalog reconciliation. One test-only compile correction changed two new fake-executor counter references from `CallCount` to the existing `Calls` property without changing runtime/API/architecture/authority/test intent/discovery shape. Automatic/configured fallback, recovery-on-load, quarantine/cleanup, destructive slot operations, persistent catalog cache, generic queues, automatic autosave timers, and broader recovery/configuration tooling remain deferred; no follow-on M4 checkpoint activated. | Jesse “Echo” Adams |
| 1.31.0 | 2026-08-10 | Approved | Activated ESV-M4-09 bounded slot rename and full-state duplication at clean baseline `07bbd2b`: rename preserves `SaveSlotId` and physical slot path while publishing a new immutable metadata-updated generation; duplication requires capacity, creates a new package-generated slot/generation identity, copies the fully verified source payload/state without participant callbacks, revalidates source provenance before destination publication, publishes head last, and reconciles catalog truth. Added ESV-D-031. Delete planning/trash, persistent catalog cache, automatic/configured fallback, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred. | Jesse “Echo” Adams |
| 1.32.0 | 2026-08-10 | Approved | Closed ESV-M4-09 at implementation commit `459023f` with focused Chronicle Editor evidence `562 / 562`; recorded public bounded rename/duplicate operations, stable rename slot identity/path, immutable metadata-only rename publication, exact source revalidation, M4-06 retention reuse, canonical duplicate capacity and bounded ID collision handling, fully verified source-state copy into new slot/generation identities, source immutability, destination head-last publication, duplicate no-auto-select, truthful post-publication reconciliation state, and zero participant callbacks. Final implementation/test scope is 26 files with 22 net new focused tests. Prepare-delete/confirm-delete, trash, persistent catalog cache, automatic/configured fallback, generic queues, automatic autosave timers, and broader configuration/tooling remain deferred; no follow-on M4 checkpoint activated. | Jesse “Echo” Adams |

---
| 1.33.0 | 2026-08-11 | Approved | Activated ESV-M4-10 bounded destructive-slot deletion/trash foundation at clean baseline `4d2f2ac`: read-only immutable deletion plans, package/session/source-provenance binding, bounded expiry and one-use confirmation, root-local Busy admission reuse, confirmed recoverable trash move as the durable delete boundary, active-slot clear only after durable removal, bounded post-commit trash retention, and truthful catalog reconciliation. Added ESV-D-032. Permanent erase, restore-from-trash API, quarantine cleanup, persistent catalog cache, automatic recovery fallback, generic queues, automatic autosave timers, and M5 tooling remain deferred. | Jesse “Echo” Adams |
| 1.34.0 | 2026-08-11 | Approved | Closed ESV-M4-10 at implementation commit `01e4cdd` with focused Chronicle Editor evidence `587 / 587`; recorded zero-mutation two-step deletion planning, expiring one-use package/session/source-bound plans, root-local Busy/no-queue confirmation, exact source revalidation, recoverable complete-tree trash publication as durable delete truth, active-slot/catalog reconciliation, bounded fail-closed trash retention, unchanged base storage contract, and ESV-T-021 through ESV-T-023 completion. M4 remains pending a dedicated milestone reconciliation; M5 is not activated. | Jesse “Echo” Adams |
| 1.35.0 | 2026-08-11 | Approved | Recorded the M4 milestone reconciliation audit at clean baseline `48454ea` and activated `ESV-M4-R1` public runtime composition. Added ESV-D-033: preserve the approved MVP public-service promises and repair them in bounded reconciliation passes rather than weakening the specification. R1 exposes participant registration, catalog snapshot/refresh, slot creation/selection, prepared-load/apply, and same-scene convenience load over existing M3/M4 machinery; R2 owns slot-policy configuration; R3 owns package-document migration while CAP-014 remains intact. M5 remains locked until reconciliation closeout. | Jesse “Echo” Adams |
| 1.36.0 | 2026-08-11 | Approved | Closed ESV-M4-R1 at implementation commit `ab18361` with focused Chronicle Editor evidence `618 / 618` and prior `587 / 587` floor preserved. Recorded public participant registration, catalog snapshot/refresh, slot create/select, prepared load/apply, and same-scene convenience load composition over existing M3/M4 authorities; retained configuration schema 1 and technical capacity 64 for R2; no automatic recovery fallback, generic queue, scene/DDOL, or M5 tooling. R2 is next but not activated; R3 and final registry reconciliation remain required before M4 close. | Jesse “Echo” Adams |
## 1. Package Identity and One-Sentence Contract

**Public title:** The Chronicle – Save Infrastructure
**Technical identifier:** EchoSave
**Flavor line:** Record the state that must endure, then prove the record can be trusted.
**Plain-language subtitle:** Reliable local game-save files, slots, metadata, participants, migrations, backups, recovery, and save-operation diagnostics.

**One-sentence ownership contract:**

> EchoSave owns durable local game-save documents, slot and generation management, save/load orchestration, participant payload contracts, serialization-provider seams, migration, integrity checking, backup retention, corruption recovery, and save-specific diagnostics; it does not own global preferences, project-specific gameplay schemas, automatic scene-object serialization, game rules deciding when saving is allowed, production save UI, cloud synchronization, platform accounts, or the mutable runtime state contributed by the game’s systems.

### 1.1 Elevator summary

The Chronicle gives a project one reliable authority for writing and reading durable game progress without forcing the package to understand the game’s inventory, quests, characters, combat, world, or progression rules. Independent game systems register narrow, versioned save participants. EchoSave captures detached data-transfer objects, serializes them through an explicit provider, writes a complete immutable save generation, validates it, and only then publishes that generation as the slot’s current record.

The MVP uses **generation-based commits** rather than repeatedly overwriting one save file. Each slot contains immutable generations. A small head pointer selects the active generation. Slot metadata lives in a manifest separate from the full payload, so menus can list saves without deserializing the entire game state. Incomplete generations never become current. Previous valid generations form bounded recovery history. If the head pointer is damaged, EchoSave can scan complete generations and select the newest valid candidate according to documented policy.

Loading is deliberately two-phase. `PrepareLoadAsync` reads, validates, recovers, deserializes, and migrates a save into a disposable prepared-load handle. The project may then travel to the correct scene through its own scene-flow authority. `ApplyPreparedLoadAsync` applies participant state only after the required runtime participants are present. A novice convenience path, `LoadAndApplyAsync`, is available when the current scene already contains every required participant.

EchoSave works without First Light, The Observatory, The Accord, The Passage, The Pulse, Resonance, The Will, The Looking Glass, or The Workshop. Optional bridges may initialize it, display slot metadata, coordinate scene travel, store a last-selected-slot preference, publish diagnostics, or generate configuration, but no peer becomes a hidden runtime requirement.

### 1.2 Why this belongs in The Sperk’s Forge

Save/load infrastructure has appeared repeatedly in Rescuers2D, Echo Systems Lab, future Hackulos planning, password-progress discussions, and project bootstrap work. The repeated problem is not calling `File.WriteAllText`. The difficult work is stable participant identity, slot catalogs, crash-safe publication, old-version migration, unknown optional payload preservation, recovery from partial writes, asynchronous file work, main-thread capture/apply boundaries, direct-scene testing, and keeping UI and gameplay rules outside the file authority.

Echo Systems Lab demonstrated the usefulness of centralized save services and stable mission IDs. Rescuers2D exposed the risks of fixed filenames and bootstrap coupling. Hackulos will require many independently evolving payloads. The Chronicle preserves the useful service boundary while replacing project-specific databases, static stores, hard-coded filenames, and one-shot JSON writes with explicit contracts and recoverable generations.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Pair “The Chronicle” with “Save Infrastructure.” |
| Setup guidance/tooltips | Yes | Every flavored heading must include direct file, slot, migration, or recovery meaning. |
| Samples | Optional | Verse visuals and sample names must be removable. |
| Runtime API/type names | No lore-only names | Types describe slots, generations, manifests, participants, migrations, results, and recovery. |
| Project data | No required Hackulos content | The game owns player data, worlds, characters, items, quests, and display copy. |

## 2. Problem Statement

### 2.1 Current problem

A naïve Unity save system often serializes one large project object directly into one filename. That approach becomes fragile when:

1. Two systems write at the same time.
2. The application quits or loses power during replacement.
3. A participant fails after other state has already been captured.
4. A save schema changes between releases.
5. An optional package is removed and its unknown payload is silently discarded.
6. A display name is used as a filename and introduces invalid or colliding paths.
7. A save menu must parse full game payloads merely to show timestamps and locations.
8. A scene change is required before the loaded state can be applied.
9. Unity object references, scene hierarchy paths, or asset names are treated as durable identity.
10. Corrupt files are overwritten before the player has a recovery choice.
11. Autosaves grow without retention bounds.
12. Save operations run file I/O on the main thread and create visible stalls.
13. Global settings and slot-specific progress are mixed into one document.
14. Production UI becomes the only place where save restrictions and errors exist.
15. A cloud or platform provider is embedded into the local save authority.
16. Test data writes into real player directories.
17. Support exports expose full save contents, file paths, names, or sensitive project data.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Central `SaveService`, fixed JSON filename, bootstrap ownership, scene-name progress | One readable save authority and explicit continue checks | Replace fixed filenames, scene strings, direct project-data knowledge, and bootstrap coupling |
| Echo Systems Lab | Save manager persists mission progress, player progress, unlocks, and stable IDs | Stable identifiers and modular state contributors | Formalize participants, versions, recovery, tests, and package isolation |
| Don’t Get Vince’d | Player progression and future run state require persistence | Event-driven runtime systems | Keep combat and character schemas project-owned |
| Hackulos | Characters, inventory, quests, vendors, spells, world state, and corpse recovery will evolve independently | Data-driven definitions and system separation | Use versioned participant payloads rather than one RPG database dump |
| DeverQuest | Chronicle/timecards, backups, repair, readiness, and migration show product-grade file concerns | Explicit reports, backups, repair, and documentation | Keep Editor-product files outside runtime EchoSave authority |
| First Light | Startup may initialize save access and select continue destinations | Ordered optional initialization | EchoSave remains independently initializable and does not select scenes |
| Observatory | Save paths, health, versions, and operation state need privacy-safe diagnostics | Structured provider snapshots | Separate bridge, no mandatory dependency |
| Accord | Global preferences have separate persistence and migration | Clear settings/save boundary | EchoSave must never become the global-preference store |
| Passage | A load may require scene travel before applying state | Explicit transition lifecycle | Two-phase prepared load, project-owned coordination bridge |
| Pulse | Game state may permit or deny manual save and represent loading | Neutral policy requests | Pulse/project validators advise EchoSave; EchoSave does not own state rules |
| Resonance | Save UI may request feedback cues | Semantic events | No audio dependency in core |
| Will | Save UI may need navigation/input locks | Context and lock leases | No input dependency in core |
| Looking Glass | Save-slot screens need metadata and operation results | Presenter/view separation | UI displays results but never owns files or payloads |

### 2.3 Consequences of doing nothing

- Every project rebuilds slots, backups, metadata, and recovery.
- One broken participant can corrupt or block the entire save without useful evidence.
- Old saves become disposable whenever schemas change.
- Optional package removal destroys data unexpectedly.
- Scene travel and load application become tangled.
- Save menus become slow or tightly coupled to game data.
- Autosave and manual-save races produce nondeterministic results.
- A package update can accidentally change filenames, IDs, or serialized shapes.
- Players lose trust after one unrecoverable partial write.

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Maintain one duplicate-safe EchoSave authority per application session.
- Reject duplicates before creating directories, registering application callbacks, scanning slots, registering participants, or beginning file operations.
- Support single, fixed multi-slot, configurable multi-slot, and bounded/unlimited-profile policies.
- Represent every slot with a stable package ID independent from its display name.
- Publish saves as immutable validated generations selected by a small current-generation pointer.
- Keep slot metadata independently readable from full participant payloads.
- Capture and restore game-owned state through narrow, versioned participant contracts.
- Preserve unknown and temporarily unclaimed payload entries across load-save round trips.
- Separate main-thread participant capture/apply from background-safe serialization, checksums, and file I/O.
- Provide two-phase prepare/apply loading and one-step convenience loading.
- Detect incomplete, corrupt, older, newer, and internally inconsistent records.
- Migrate package documents and participant payloads through explicit contiguous migration chains.
- Retain bounded prior valid generations for backup and autosave recovery.
- Coalesce redundant autosave requests while serializing destructive and mutating operations.
- Provide structured results and stable diagnostics without requiring The Observatory.
- Supply safe Editor inspection, simulation, test-data, validation, repair-preview, and recovery tools.
- Prove the MVP in an isolated Save Laboratory using a sandbox storage root.

### 3.2 Non-goals

- No automatic serialization of arbitrary GameObjects, MonoBehaviours, scenes, static fields, or the whole object graph.
- No invention of player stats, inventory, objectives, characters, checkpoints, or world-state schemas.
- No global preference persistence; that belongs to The Accord.
- No production save-slot, confirmation, loading, error, or recovery UI.
- No ownership of scene travel, game state, pause, input contexts, audio, or startup sequencing.
- No mandatory encryption, compression, cloud saves, platform accounts, achievements, or anti-cheat guarantees in the MVP.
- No promise of cross-device synchronization without an approved provider adapter.
- No use of project display names as physical file paths.
- No silent deletion of unknown payloads or unsupported saves.
- No downgrade writing to older schemas.
- No hidden save during application quit as the only protection against lost progress.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project with one simple data model | Create configuration, register one participant, save, reload, and recover a prior generation through documented steps |
| Programmer | Multiple independent runtime systems | Register versioned participants and coordinate prepare, scene travel, and apply without package source edits |
| Designer/content author | Needs save slots and summary metadata | Configure slot policy, labels, retention, and project-owned metadata fields without touching file code |
| Tester | Needs repeatable failure coverage | Simulate truncation, locked files, old/new versions, missing participants, interrupted commits, and recovery in a sandbox |
| Maintainer | Needs safe upgrades | Inspect manifests, migration paths, diagnostic codes, generation history, and release compatibility |
| Support/debug user | Receives a player report | Export a redacted manifest/health snapshot without exposing full payload contents by default |

### 3.4 Measurable success criteria

- Installs into a clean supported Unity project with zero compile errors.
- Runs with no other Sperk’s Forge runtime package installed.
- Duplicate roots create no directories, subscriptions, participants, or file operations.
- A committed generation survives simulated interruption before, during, and after publication according to documented recovery rules.
- Slot listing reads manifests/catalog data without deserializing participant payloads.
- Unknown payloads survive a load-save cycle unchanged unless an explicit prune request is approved.
- Missing required migrations block safely without modifying source files.
- Corrupt current generations fall back only to verified retained generations and report the exact recovery path.
- Autosave requests remain bounded and do not create parallel writes.
- Samples can be removed without breaking runtime code.
- Editor tooling writes only to an explicit sandbox unless a user confirms a real save root.
- Documentation and diagnostic codes match the shipped API.

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers.
- Gameplay programmers separating runtime systems into save participants.
- Designers configuring save models and metadata.
- QA testers validating corruption, migration, and recovery.
- Maintainers integrating existing projects incrementally.
- Future provider authors adding cloud or platform storage behind the approved backend contract.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| UC-001 | Initialize local save service | Project/root | Valid configuration and writable root | Slot catalog becomes ready with structured status | MVP |
| UC-002 | Register participant | Game system | Unique stable participant ID | Participant appears in registry; duplicate policy is enforced | MVP |
| UC-003 | Create slot | Player/project | Slot policy permits another slot | New stable slot and initial metadata are created | MVP |
| UC-004 | Save active slot | Player/project | Active slot, required participants ready, permission granted | New validated generation becomes current | MVP |
| UC-005 | Request autosave | Game system | Active slot and autosave enabled | Request saves or coalesces into one bounded pending autosave | MVP |
| UC-006 | List slots | UI/project | Service ready | Lightweight metadata snapshots return without full payload application | MVP |
| UC-007 | Prepare load | Project | Valid slot exists | Validated/migrated prepared-load handle is returned | MVP |
| UC-008 | Apply prepared load | Project | Required participants registered | Participant state applies deterministically or rolls back/fails by policy | MVP |
| UC-009 | Load and apply in current scene | Project | All required participants ready | Convenience operation completes in one call | MVP |
| UC-010 | Recover corrupt current generation | Player/support | Retained valid generation exists | Recovery plan identifies safe candidate; explicit or configured fallback succeeds | MVP |
| UC-011 | Rename slot | Player/project | Display-name policy accepts value | Metadata changes; physical slot ID/path remains stable | MVP |
| UC-012 | Duplicate slot | Player/project | Capacity available | New slot ID receives a complete copy as a new generation | MVP |
| UC-013 | Delete slot | Player/project | Prepared destructive plan confirmed | Slot moves to recoverable trash/quarantine or is deleted per policy | MVP |
| UC-014 | Migrate old participant payload | Runtime | Contiguous migration chain exists | Payload migrates in memory and may be recommitted later | MVP |
| UC-015 | Inspect save health | Developer/tester | Editor or development build | Manifests, versions, sizes, checksums, and recovery status are visible | MVP |
| UC-016 | Export redacted support snapshot | Developer/player support | Explicit request | Payload-free diagnostic package is produced | MVP |
| UC-017 | Use cloud backend | Provider author | Approved adapter installed | Core contracts operate on provider storage | Later |
| UC-018 | Encrypt/compress payloads | Project/provider | Approved provider installed | Transformation is explicit and versioned | Later |

### 4.3 Explicitly unsupported use cases

- Reflecting over all scene objects and serializing them automatically.
- Loading a save from an unknown newer major document format by guessing.
- Writing directly into package source or `Assets` at runtime.
- Treating checksums as anti-cheat, authentication, or encryption.
- Accepting unbounded payload size, slot count, history, or migration depth.
- Saving Unity object instance IDs as durable cross-session references.
- Using cloud synchronization as a hidden side effect of a local save request.
- Making an application-quit save the only way progress is preserved.

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Save-root initialization and health.
- Slot identity, slot policy, slot catalog, and active-session slot selection.
- Immutable generation creation, validation, publication, retention, quarantine, and recovery.
- Package-owned manifest, payload-envelope, head-pointer, catalog-cache, and migration formats.
- Participant registration and deterministic capture/apply orchestration.
- Unknown payload preservation and explicit prune policy.
- Default local filesystem backend beneath the configured persistent-data root.
- Default JSON serializer and serializer-provider registry.
- Package/document and participant migration registration/execution.
- Checksums, size bounds, file/path validation, and operation diagnostics.
- Save operation admission, locking, autosave coalescing, and cancellation boundaries.
- Safe development inspection, simulation, and repair-preview tools.

### 5.2 The package does not own

- The meaning or shape of project gameplay data.
- Runtime state stored by a participant before capture or after restore.
- Global preferences, including audio, display, accessibility, and input preferences.
- When the game should autosave or whether a particular gameplay moment permits manual saving.
- Scene destination selection or transition execution.
- Main Menu, pause, save-slot, confirmation, loading, recovery, or error presentation.
- Music, SFX, haptics, input locks, cursor state, pause, or time scale.
- Asset catalogs and stable IDs owned by gameplay packages.
- Cloud accounts, cross-device merge rules, platform storage APIs, or provider billing.
- Encryption key management, anti-tamper authority, analytics, telemetry, or crash reporting.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoSave interacts |
|---|---|---|
| Global preferences | The Accord (`EchoSettings`) | Optional bridge may store last-selected-slot preference; save payloads remain separate |
| Initial startup | First Light (`EchoLaunch`) | Optional startup step initializes catalog and reports health |
| Diagnostics dashboard | The Observatory (`EchoDiagnostics`) | Separate provider bridge publishes health and operation snapshots |
| Scene travel for a loaded save | The Passage (`EchoSceneFlow`) | Project/bridge coordinates prepared load, transition, then apply |
| Save permission/high-level loading state | The Pulse (`EchoGameState`) or project | Optional validator/scope bridge; EchoSave does not own game-state rules |
| Save-slot and recovery UI | The Looking Glass (`EchoUI`) or project | Presenter bridge consumes metadata/results and submits requests |
| Input during save UI | The Will (`EchoInput`) | UI/project may acquire locks; core has no dependency |
| Audio feedback | Resonance (`Jukebot`) | UI/project requests cues after semantic results |
| Starter generation | The Workshop (`EchoGameStarter`) | Generates configuration, directories, sample participants, and reports |
| Progression/characters/inventory/objectives | Their package or project | Each registers its own versioned participant through an adapter |
| Cloud/platform storage | EchoSave provider adapter | Separate package implements storage/synchronization without replacing local authority contracts |

### 5.4 Boundary tests

For every proposed feature:

1. Does it protect or operate durable game-save records?
2. Can it function without knowing project gameplay types?
3. Is it a package-owned document concern or participant-owned payload concern?
4. Does it require another package merely to present or trigger the operation?
5. Would a storage/provider adapter be cleaner than core code?
6. Does it risk mixing global preferences with slot progress?
7. Can the package preserve unknown data instead of deleting it?
8. Can failure be reported without UI, audio, or scene assumptions?

Features that fail these tests move to project code, another package, a bridge, a provider adapter, or Deferred scope.

## 6. Independence Contract

### 6.1 Standalone guarantees

EchoSave must:

- Compile with only declared Unity/.NET dependencies.
- Initialize without First Light.
- Function without every other Sperk’s Forge package.
- Use a project-assigned configuration and local backend by default.
- Keep all project payload types outside package source.
- Use no runtime `UnityEditor` references.
- Permit participant injection, serializer replacement, storage replacement, clock injection, and sandbox testing.
- Fail visibly when configuration, storage, participants, or migrations are missing.
- Preserve unknown payloads without requiring the original package to be installed.
- Use a package-owned Standalone Laboratory storage root that cannot collide with production saves.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | Root initializes local storage and empty catalog | Clean-project PlayMode test |
| Enter Save Laboratory directly | Development initializer creates only missing authority | Direct-scene lab test |
| Optional bridge absent | Core APIs and lab remain complete | Assembly/reference audit |
| EchoUI absent | Metadata/results remain available through API and lab presenter | Standalone lab |
| Passage absent | Prepare/apply works; project coordinates scenes manually | Two-phase load test |
| Duplicate root present | Duplicate rejects before storage or subscriptions | Duplicate lifecycle test |
| Configuration missing | Root enters blocked state with `ESV-CFG-001` | Failure test |
| Storage unavailable | No slot mutation; structured failure returned | Backend simulation |
| Sample deleted | Runtime package and tests compile | Sample-removal test |
| Participant package removed | Unknown payload is preserved on round trip | Preservation test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine | Platform | Yes | 6000.0 | Runtime lifecycle, ScriptableObjects, `Application.persistentDataPath`, Awaitable | Package cannot run without Unity |
| .NET `System.IO` | Platform | Yes | Unity-supported profile | Local files, streams, directories | Required for default backend |
| .NET cryptography hashing | Platform | Yes for default integrity provider | Unity-supported profile | SHA-256 checksums for corruption detection | Replaceable integrity provider may be supplied |
| Unity Test Framework | Test | Tests only | Compatible with Unity floor | EditMode/PlayMode tests | Runtime unaffected |
| uGUI/TMP | Sample only | No | Compatible released versions | Laboratory controls/readout | Deleting sample does not affect runtime |

### 6.4 Forbidden dependencies

- Project assemblies or concrete gameplay types.
- Another Sperk’s Forge runtime package in core assemblies.
- `Resources` folder discovery as the only configuration path.
- Scene names, build indices, tags, layers, or input maps.
- Samples, test utilities, or Editor code at runtime.
- Unlicensed third-party serializers or cloud SDKs.
- Static mutable gameplay stores as the required participant API.
- Display names, scene hierarchy paths, or Unity instance IDs as durable save identity.

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| CAP-001 | Duplicate-safe root | One application-session save authority | Approved | Yes | Runtime | Claim before storage side effects |
| CAP-002 | Slot policies | Single, fixed, configurable, and bounded/unlimited profiles | Approved | Yes | Runtime/Editor | Physical capacity still bounded by configuration/platform |
| CAP-003 | Stable slot IDs | Display-name-independent identity | Approved | Yes | Runtime | Package-generated value type |
| CAP-004 | Immutable generations | Write complete new records; never edit committed payload in place | Approved | Yes | Runtime | Core reliability model |
| CAP-005 | Head pointer | Small record selects current valid generation | Approved | Yes | Runtime | Recovery can scan if damaged |
| CAP-006 | Independent manifests | Slot metadata readable without payload application | Approved | Yes | Runtime | Catalog cache is derived/rebuildable |
| CAP-007 | Participant registry | Versioned project payload contributors | Approved | Yes | Runtime | Deterministic ID order |
| CAP-008 | Unknown payload preservation | Round-trip unclaimed entries | Approved | Yes | Runtime | Explicit prune required |
| CAP-009 | Default JSON serializer | Unity JSON path for plain DTOs | Approved | Yes | Runtime | Replaceable and limitation-aware |
| CAP-010 | Serializer providers | Explicit serializer IDs and adapters | Approved | Yes | Runtime | Custom serializers project/provider-owned |
| CAP-011 | Checksums/size bounds | Detect truncation/corruption and reject oversized input | Approved | Yes | Runtime | Integrity, not security |
| CAP-012 | Two-phase loading | Prepare then apply | Approved | Yes | Runtime | Scene flow remains external |
| CAP-013 | Convenience load | Read/migrate/apply in current scene | Approved | Yes | Runtime | Requires all participants ready |
| CAP-014 | Migration chains | Document and participant migration | Approved | Yes | Runtime/Editor | Contiguous upgrade only |
| CAP-015 | Backup retention | Keep bounded prior valid generations | Approved | Yes | Runtime | Cleanup after successful publication |
| CAP-016 | Autosave admission | Coalesce and rotate bounded autosaves | Approved | Yes | Runtime | Game decides when to request |
| CAP-017 | Slot operations | Create, rename, duplicate, prepare-delete, confirm-delete | Approved | Yes | Runtime | Destructive operations are explicit |
| CAP-018 | Recovery planning | Inspect and select verified candidates | Approved | Yes | Runtime/Editor | No silent overwrite of corrupt source |
| CAP-019 | Sandbox Laboratory | Isolated save/recovery proof | Approved | Yes | Sample | No production path collision |
| CAP-020 | Validation/inspection | Setup, health, manifest, migration, and repair-preview tools | Approved | Yes | Editor | Non-destructive by default |
| CAP-021 | Redacted support snapshot | Export health without payload contents | Approved | Yes | Runtime/Editor | Explicit user action |
| CAP-022 | Optional thumbnails | Project provider contributes image/descriptor | Approved | No | Runtime/Bridge | Deferred from core capture path |
| CAP-023 | Compression | Provider transformation | Deferred | No | Provider | Requires format/security review |
| CAP-024 | Encryption/authentication | Provider transformation and key policy | Deferred | No | Provider | Not an MVP promise |
| CAP-025 | Cloud synchronization | Provider-neutral sync seams and adapter | Deferred | No | Provider | Separate package/research |
| CAP-026 | Cross-device merge | Conflict-resolution model | Deferred | No | Provider/Project | Game-specific policy |

### 7.2 MVP capability set

The smallest complete release includes:

- one protected root;
- project configuration;
- default local filesystem backend;
- stable slots and slot policies;
- immutable generations, manifests, payload documents, checksums, and head pointers;
- participant capture/apply and unknown payload preservation;
- Unity JSON serializer plus provider abstraction;
- two-phase and convenience loading;
- document and participant migrations;
- bounded prior-generation recovery and autosave retention;
- create, list, select, rename, duplicate, preview-delete, confirm-delete;
- structured results, diagnostics, and redacted snapshots;
- Editor setup/validation/inspection/simulation;
- one isolated Save Laboratory.

### 7.3 Later capability set

- Optional thumbnail provider and screenshot tooling.
- Compression providers.
- Encryption/authentication providers and key-management guidance.
- Cloud/platform storage adapters.
- Cross-device conflict and merge contracts.
- Console certification/provider integrations.
- Streaming/chunked payloads for very large worlds.
- Differential/incremental saves only after profiling proves need.
- Multi-user/shared-world save authority with EchoMultiplayer research.
- Mod/plugin namespaces and explicit payload ownership transfer tools.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Serialize every MonoBehaviour automatically | Rejected | Hidden coupling, unstable identity, unsafe object graphs | Never in core |
| Use `PlayerPrefs` for save payloads | Rejected | Wrong authority and poor large-document/recovery model | Never |
| One fixed `save.json` | Rejected | No slots, generations, recovery, or concurrency safety | Never |
| Save on quit as primary strategy | Rejected | Quit callbacks are not a reliability guarantee | Never as sole strategy |
| BinaryFormatter | Rejected | Unsafe/obsolete and poor compatibility | Never |
| Display name as filename | Rejected | Invalid characters, collisions, rename breakage | Never |
| Silent load of newer unknown format | Rejected | Can misinterpret or destroy data | Never |
| Default cloud SDK | Deferred | Provider, cost, auth, and platform decisions unresolved | Approved provider research |
| Whole-file encryption in core | Deferred | Key management and platform policy are not neutral | Provider specification |
| Incremental/delta saves | Deferred | Complexity without measured need | Large-world profiling evidence |

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | `EchoSaveConfiguration`, slot templates, retention, serializer/backend descriptors, limits, migration policy | Active slots, current operations, registered participant instances, file handles |
| Runtime state/behavior | Root, coordinator, participant registry, prepared loads, generation builder, catalog, backend, migrations, results, diagnostics | Editor APIs, production UI, gameplay schemas, scene travel |
| Presentation/feedback | Sample Laboratory, optional project/EchoUI presenters, Editor inspectors | Save truth, permission rules, participant state |

### 8.2 Component topology

```text
EchoSaveRoot
├── EchoSaveService
│   ├── SaveOperationCoordinator
│   ├── SaveSlotCatalog
│   ├── SaveParticipantRegistry
│   ├── SaveMigrationRegistry
│   ├── SaveSerializerRegistry
│   ├── ISaveStorageBackend
│   ├── IIntegrityProvider
│   ├── ISaveClock
│   └── SaveDiagnosticsBuffer
├── active session slot selection
└── application/session lifecycle

Save operation
  request
    -> admission and permission validation
    -> main-thread participant capture
    -> detached payload snapshot
    -> background serialization/checksum/write
    -> generation verification
    -> head publication
    -> derived catalog update
    -> retention cleanup
    -> structured result/event

Load operation
  prepare request
    -> locate/recover generation
    -> validate manifest/payload/checksums/limits
    -> document migration
    -> participant payload migration/deserialization
    -> PreparedSaveLoad handle
  project may change scene
  apply request
    -> validate participants
    -> deterministic participant apply
    -> active slot/session update
    -> structured result/event
```

### 8.3 Package-local long-lived authority

| Question | Decision |
|---|---|
| Scene-surviving package root required? | Yes for normal runtime use |
| Root type | `EchoSaveRoot` |
| Lifetime | Application session by default |
| Authority scope | EchoSave operations/catalog/prepared-load lifecycle only; never peer runtime truth or project-wide service location |
| Project composition | A consumer project may parent/compose EchoSaveRoot beneath a project-owned long-lived runtime composition root; parentage does not transfer authority |
| Duplicate behavior | Reject duplicate before path creation, callbacks, catalog scans, registration, or operations |
| Initialization trigger | Explicit `InitializeAsync`; optional auto-initialize flag for prefab path |
| Shutdown | Stop admission, finish or settle commit-critical work, dispose prepared handles, unsubscribe, clear authority |
| Direct-scene behavior | Development initializer creates configured root only when absent |
| Test seams | Backend, serializer, integrity provider, clock, participant registry, and path policy are injectable |

`DontDestroyOnLoad` or equivalent scene-surviving lifetime is an object-lifetime decision, not durable persistence. EchoSave does not own a universal persistent root, generic `GameManager`, peer discovery registry, or service locator. First Light may initialize/discover EchoSave during startup but does not own the root after launch handoff.

### 8.4 Storage topology and generation commit model

Default local layout:

```text
<Application.persistentDataPath>/<configured-root>/
├── catalog.cache.json                 # derived, rebuildable, never sole authority
├── slots/
│   └── <slot-id>/
│       ├── head.json                  # current committed generation pointer
│       ├── generations/
│       │   └── <generation-id>/
│       │       ├── manifest.json      # metadata, versions, entry inventory, hashes
│       │       ├── payload.json       # participant payload entries
│       │       └── thumbnail.bin      # optional provider-owned format, later/MVP optional
│       ├── incomplete/                # interrupted/uncommitted generations
│       └── quarantine/                # corrupt/unsupported records preserved for inspection
├── trash/                             # bounded recoverable deletion when enabled
└── diagnostics/                       # optional redacted exports only
```

Rules:

1. A committed generation is immutable.
2. A generation becomes eligible only after all required files are written, flushed where supported, checksummed, and verified.
3. `head.json` is updated only after generation verification.
4. If atomic file replacement is supported, the backend uses it for the small head pointer and may create a backup pointer.
5. If atomic replacement is unavailable, the backend uses a documented temp/rename strategy and recovery scans complete generations; it never claims a guarantee the platform cannot provide.
6. A catalog cache accelerates listing but is derived. It can always be rebuilt from heads and manifests.
7. Retention cleanup occurs after successful publication, never before.
8. Incomplete or corrupt generations are ignored for normal load and preserved/quarantined according to policy.

### 8.5 Lifecycle sequence

1. Root claims authority.
2. Configuration and providers validate.
3. Storage root resolves and path safety validates.
4. Package/document migrations validate.
5. Slot catalog cache loads or rebuilds.
6. Service enters Ready or Degraded/Blocked state.
7. Participants register and operations execute.
8. Scene changes do not destroy the root.
9. On shutdown, new operations stop; commit-critical publication settles or reports failure.
10. Authority and handles clear.

### 8.6 Failure model

| Failure | Detection | User-visible/API result | Runtime fallback | Code |
|---|---|---|---|---|
| Missing configuration | Initialization | Blocked status | No storage mutation | ESV-CFG-001 |
| Unsafe root path | Initialization/request | Blocker | Refuse operation | ESV-PATH-001 |
| Duplicate root | Awake/claim | Warning/result | Destroy/disable duplicate before side effects | ESV-LIFE-001 |
| Storage unavailable/locked | Init/write/read | Failure | Preserve current generation; retry allowed | ESV-IO-001 |
| Slot capacity reached | Create/duplicate | Rejected | No mutation | ESV-SLOT-004 |
| Required participant missing | Apply | Blocking failure | Prepared handle remains valid if policy allows retry | ESV-PART-004 |
| Participant capture fails | Save capture | Save fails | No generation published | ESV-PART-006 |
| Participant apply fails | Apply | Load failure | Apply rollback policy/report; source unchanged | ESV-PART-007 |
| Missing migration step | Prepare | Unsupported-old failure | Source quarantined only by explicit action | ESV-MIG-002 |
| Newer format | Prepare/list | Unsupported-newer status | Preserve file; no guessing | ESV-MIG-004 |
| Checksum mismatch | Verify/read | Corrupt status | Recovery candidate search | ESV-DATA-003 |
| Head pointer damaged | Catalog/read | Degraded/recovery | Scan verified generations | ESV-REC-001 |
| No valid recovery generation | Load | Failure | Preserve evidence, do not overwrite | ESV-REC-004 |
| Oversized file/payload | Read | Rejected | Quarantine/report | ESV-SEC-002 |
| Cancellation after publication starts | Operation | Too-late result | Finish to known state | ESV-OP-008 |

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoSaveConfiguration` | Root path, slot mode, limits, retention, policies, providers | Unity asset GUID for Editor identity; optional `SaveConfigurationId` only when exported/runtime-addressed | No | Yes |
| `SaveSlotTemplate` | Fixed-slot labels/order/default IDs | Yes | No | Yes |
| `SaveRetentionPolicy` | Prior generation/autosave/trash bounds | No | No | Yes |
| `SaveLimitPolicy` | File, payload, participant, slot, migration limits | No | No | Yes |
| `SaveSerializerDescriptor` | Selects serializer provider ID/options | Yes | No | Yes |
| `SaveStorageDescriptor` | Selects storage backend ID/options | Yes | No | Yes |
| `SaveRecoveryPolicy` | Automatic/manual recovery behavior | No | No | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `EchoSaveServiceState` | Root | Application session | Reinitialize/shutdown | Diagnostics only |
| `SaveSlotCatalogSnapshot` | Catalog | Until refresh | Rebuilt from manifests | Derived cache, not gameplay truth |
| `SaveParticipantRegistration` | Registry | Registration/root lifetime | Dispose/unregister | Not persisted |
| `SaveOperationRecord` | Coordinator | Bounded history | Clear/reset | Diagnostics only |
| `PreparedSaveLoad` | Caller/service | Until apply/dispose/expiry | Dispose/timeout/shutdown | In-memory only |
| `UnknownPayloadStore` | Active loaded session | Until slot change/reset | Rebuilt on load | Preserved into next generation |
| `ActiveSlotSelection` | Service | Session | Select/clear/shutdown | Session-only by default |
| `SaveGenerationBuilder` | Operation | One save | Dispose on terminal result | Never persisted until commit files |

### 9.3 Stable identifiers

- `SaveSlotId`: package-generated lowercase canonical GUID string, never a display name.
- `SaveGenerationId`: unique sortable identifier containing monotonic session sequence plus randomness; uniqueness does not rely on clock alone.
- `SaveParticipantId`: developer-authored reverse-domain or namespace-like stable string, such as `com.mygame.player-progress`.
- `SaveSerializerId`, `SaveStorageBackendId`, and migration IDs: stable lowercase identifiers.
- IDs reject empty values, path separators, traversal segments, control characters, reserved names, and collisions.
- Renaming a slot changes metadata only.
- Participant ID changes require alias/migration mapping.
- Project definition IDs inside payloads are owned and migrated by the participant/project.

### 9.4 Package-owned document model

`SaveHeadPointer`

- slot ID;
- current generation ID;
- previous head generation ID when available;
- pointer format version;
- update sequence;
- checksum.

`SaveManifest`

- package document format version;
- slot ID and generation ID;
- creation/update UTC timestamps;
- save reason/kind;
- project/build identity and version;
- display metadata snapshot;
- payload file name, byte length, checksum, integrity algorithm;
- participant entry inventory with ID, schema version, serializer ID, required/optional flag, byte length, checksum;
- generation commit state;
- optional thumbnail descriptor;
- migration provenance and recovery provenance;
- no arbitrary full gameplay payload.

`SavePayloadDocument`

- package payload-document version;
- bounded list of `SavePayloadEntry` records;
- each entry contains participant ID, participant schema version, serializer ID, serialized payload string or byte-provider reference, checksum, and flags;
- unknown entries remain opaque and unchanged when preserved.

### 9.5 Participant model

Each `ISaveParticipant` declares:

- stable participant ID;
- current participant schema version;
- criticality: Required or Optional;
- missing-payload policy: InitializeDefault, Ignore, or Fail;
- capture method producing a detached DTO/result;
- apply method consuming a migrated/deserialized DTO;
- serializer ID or default serializer;
- optional aliases for prior participant IDs;
- optional migration chain ownership.

Rules:

- Capture/apply executes on the Unity main thread by default.
- Participants must not return live Unity object graphs, scene objects, or mutable shared ScriptableObjects.
- Payload DTOs reference project definitions by stable project-owned IDs.
- Participant order is deterministic by canonical participant ID.
- Duplicate participant IDs block the later registration.
- Chronicle contains no compile-time catalog of known persistence participants.
- A participant introduced after Chronicle ships registers through the same public contract; adding that participant must not require a Chronicle core source edit.
- Runtime detached DTO type authority comes from the active registered participant, never from a CLR type name stored in save data.
- Required participant capture failure aborts save.
- Optional failure policy is explicit; default is still to abort rather than silently create a partial save.
- Apply events fire only after authoritative participant state changes.
- EchoSave cannot guarantee transactional rollback inside arbitrary participant logic; participants that mutate during apply should support snapshot/rollback or stage-then-commit. The apply report identifies every participant that completed or failed.

### 9.6 Unknown payload preservation

When a loaded generation contains payloads with no active participant:

1. EchoSave validates their manifest/checksum/size.
2. Entries remain opaque in an `UnknownPayloadStore` for the active loaded slot.
3. The next save carries them forward byte-for-byte unless an explicit prune plan names them.
4. A new game/empty slot begins with no unknown entries.
5. Unknown entries cannot execute code.
6. Size and count limits still apply.
7. Diagnostics report IDs and sizes, not payload contents.

### 9.7 ScriptableObject safety

Configuration assets and slot templates remain immutable during play. Runtime selected slot, operation state, generation counters, unknown payloads, catalog entries, and prepared load handles live in runtime objects. Project participants must not use shared ScriptableObjects as the mutable save state.

### 9.8 Serialization and migration

- Package document versions and participant schema versions are separate.
- Document migration updates package-owned head/manifest/payload envelope shapes.
- Participant migration updates one participant’s opaque payload.
- Chains are contiguous and deterministic from stored version to current version.
- Missing steps fail before apply.
- Migration occurs in memory first; source generations remain untouched.
- Successful load does not automatically rewrite the source unless configured and explicitly permitted; the next save writes a current-format generation.
- Newer unsupported major versions are not loaded or downgraded.
- Every migration step records source/target versions and stable diagnostic context.

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Ownership |
|---|---|---|---|
| `EchoSaveRoot` | MonoBehaviour | Claims authority and owns service lifecycle | Scene/prefab, one active |
| `EchoSaveConfiguration` | ScriptableObject | Project configuration | Project asset |
| `IEchoSaveService` | Interface | Primary save API | Root service |
| `ISaveParticipant` | Interface | Captures/applies one project state domain | Project/package adapter |
| `SaveParticipantDescriptor` | Struct | Stable ID, version, criticality, serializer, policies | Participant |
| `SaveParticipantRegistration` | Disposable struct/class | Owns registry membership | Caller |
| `ISaveStorageBackend` | Interface | File/object storage operations and capabilities | Provider/root |
| `ISaveSerializer` | Interface | Serializes detached DTOs by type/provider ID | Provider/root |
| `IIntegrityProvider` | Interface | Calculates/verifies content hashes | Provider/root |
| `ISaveClock` | Interface | UTC and monotonic timing | Provider/root |
| `ISavePermissionProvider` | Interface | Project-specific allow/deny advice | Project/bridge |
| `SaveSlotId` | Value type | Stable slot identity | Package |
| `SaveGenerationId` | Value type | Stable generation identity | Package |
| `SaveSlotCreateRequest` | Struct/class | Public project metadata for one new technical slot | Caller |
| `SaveSlotCreateResult` | Immutable result | Durable slot-publication and catalog-reconciliation truth | Service |
| `SaveSlotCatalogEntry` | Immutable model | Lightweight slot display/health data | Service result |
| `SaveSlotCatalogSnapshot` | Immutable model | Bounded slot list and catalog health | Service result |
| `SaveRequest` | Struct | Save target, reason, metadata, cancellation, options | Caller |
| `SaveOperationResult` | Struct | Structured save terminal result | Service |
| `SaveLoadRequest` | Struct | Slot/generation/recovery options | Caller |
| `PreparedSaveLoad` | Disposable handle | Validated migrated in-memory load | Service/caller |
| `SaveLoadResult` | Struct | Prepare/apply terminal result | Service |
| `SaveRecoveryPlan` | Immutable model | Candidate generations and recommended action | Service |
| `SaveDeletionPlan` | Immutable model | Two-step destructive-operation token | Service |
| `SaveDiagnosticSnapshot` | Immutable model | Redacted service/operation health | Service |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure | Thread rule |
|---|---|---|---|---|
| `InitializeAsync()` | Validate providers and build catalog | Authority claimed | Ready/degraded/blocked result | Completes on main thread |
| `RegisterParticipant(ISaveParticipant)` | Add participant | Ready or initializing; unique ID | Registration or structured failure | Main thread |
| `GetCatalogSnapshot()` | Read current immutable catalog | Ready | `SaveSlotCatalogSnapshot`, never mutable internals | Main thread, no I/O |
| `RefreshCatalogAsync()` | Re-scan heads/manifests | Ready and root-local operation admission available | `SaveSlotCatalogRefreshResult` or Busy/lifecycle failure | Async/read, complete main |
| `CreateSlotAsync(SaveSlotCreateRequest)` | Create stable slot through the proven M4-02 technical creation path | Ready; current R1 technical capacity permits | `SaveSlotCreateResult`; never auto-select | Async/exclusive |
| `SelectSlot(SaveSlotId)` | Set session active slot | Ready; slot exists and is selectable in current catalog | `SaveActiveSlotSelectionResult` | Main thread, memory-only |
| `RenameSlotAsync(SaveSlotRenameRequest)` | Change display metadata only | Valid slot/name | New generation/manifest or metadata commit | Async |
| `DuplicateSlotAsync(SaveSlotDuplicateRequest)` | Copy current verified generation into new slot ID | Capacity and source healthy | New slot or failure | Async |
| `PrepareDeleteSlotAsync(SaveSlotId)` | Build destructive plan/token | Slot exists | Plan, no mutation | Async/read |
| `ConfirmDeleteSlotAsync(SaveDeletionPlan)` | Move/delete slot per policy | Valid unexpired plan | Result and catalog update | Async/exclusive |
| `SaveAsync(SaveRequest)` | Capture and commit a generation | Active/target slot, participants ready, permission allowed | `SaveOperationResult` | Capture main; serialize/I/O background; complete main |
| `RequestAutosave(AutosaveRequest)` | Submit coalescible autosave | Autosave enabled | Accepted/coalesced/rejected ticket | Main thread |
| `PrepareLoadAsync(SaveLoadRequest)` | Read/validate/prepare current canonical generation without applying participants | Ready; explicit slot exists; no automatic recovery fallback in R1 | `PreparedLoadCreationResult` | Async/read; complete main |
| `ApplyPreparedLoadAsync(PreparedSaveLoad)` | Apply one already prepared handle using participant-owned missing-payload policy | Ready; valid handle and participants | `SavePreparedLoadApplyResult` | Main thread/exclusive |
| `LoadAndApplyAsync(SaveLoadRequest)` | Convenience prepare then apply in the current scene | Ready; all required participants ready; no scene travel | `SaveLoadResult` | Async; apply on main thread |
| `BuildRecoveryPlanAsync(SaveSlotId)` | Inspect generations | Slot path accessible | Candidate plan | Background read |
| `ExecuteRecoveryAsync(SaveRecoveryPlan, candidate)` | Publish selected verified generation | Explicit/allowed policy | Recovery result | Async/exclusive |
| `ExportRedactedSnapshotAsync(path/options)` | Support report | Explicit request | Payload-free export | Async |
| `ResetSessionState()` | Clear selection, unknown payload cache, histories as allowed | No active operation | Result | Main thread |
| `ShutdownAsync()` | Stop admission and settle lifecycle | Authority active | Terminal result | Main thread completion |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `InitializationChanged` | Root/service | After state change | Old/new status and result | Optional listeners |
| `CatalogChanged` | Catalog | After authoritative catalog snapshot replaces old | Immutable snapshot/delta | UI not required |
| `ActiveSlotChanged` | Service | After selection changes | Old/new slot IDs | Session-only truth |
| `OperationStarted` | Coordinator | After admission | Operation summary | Diagnostics/presentation optional |
| `OperationProgressed` | Coordinator | Bounded meaningful phase changes | Phase/progress | No per-frame requirement |
| `SaveCompleted` | Service | After generation publication or terminal failure | Save result | Audio/UI listeners optional |
| `LoadPrepared` | Service | After valid handle created | Prepared summary | Scene coordination optional |
| `LoadApplied` | Service | After participant apply terminal result | Detailed apply result | Game state already changed for successful participants |
| `RecoveryCompleted` | Service | After head/catalog change | Recovery result | Optional |
| `ParticipantRegistryChanged` | Registry | After register/unregister | Bounded summary | Diagnostics only |

Events occur after authoritative package state changes. A listener is never required for commit, load, or recovery to complete.

### 10.4 Async and cancellation policy

- Public asynchronous operations return a fresh `Awaitable<T>` and are not reused.
- Unity object access, participant capture, participant apply, and event dispatch occur on the main thread.
- Detached serialization, hashing, manifest parsing, and local file I/O may run on a background thread when provider capability allows.
- Operations return to the main thread before public completion unless a provider contract explicitly documents otherwise.
- Cancellation is honored while queued, during validation, before/during capture when safe, and during background work before commit publication.
- Once head publication begins, cancellation is reported as Too Late and the operation completes to a known committed or failed state.
- Cancellation never deletes the prior current generation.
- Prepared loads are disposable, bounded, and expire according to configuration; disposal releases memory only and never mutates disk.
- Shutdown stops new admission. It does not abandon a head publication halfway through.

### 10.5 Operation admission and concurrency

MVP policy:

- One mutating operation globally per EchoSave root.
- Catalog snapshot reads are memory-only and may occur during operations.
- Explicit catalog refresh, prepare-load, save, duplicate, delete, and recovery are serialized by the coordinator.
- Manual save requests received while busy return Busy by default rather than form an unbounded queue.
- Autosaves coalesce into at most one pending latest request.
- Repeated identical catalog refreshes coalesce.
- Load/recovery/delete requests never coalesce with save requests.
- Queue capacity and overflow behavior are configured and diagnosed.

### 10.6 API ergonomics

Novice path:

1. Create configuration and root through Setup.
2. Implement one participant.
3. Register it.
4. Call `CreateSlotAsync`, `SelectSlot`, and `SaveAsync`.
5. Call `LoadAndApplyAsync` in the same scene.

Advanced path:

- inject storage/serializer/integrity/clock providers;
- use prepare/scene-travel/apply;
- register participant migrations;
- preserve or explicitly prune unknown payloads;
- supply project permission and metadata providers;
- build provider adapters without editing core.

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install package.
2. Open **Tools > Sperk’s Forge > The Chronicle > Setup**.
3. Choose slot model, capacity, root subdirectory, retention, serializer, and recovery policy.
4. Preview generated assets and scene changes.
5. Create project-owned configuration and optional root prefab.
6. Validate path, limits, providers, fixed slot IDs, and duplicate roots.
7. Import/open the Save Laboratory.
8. Run the Laboratory sandbox checklist.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeat safe? | Undo/backup | Report |
|---|---|---|---:|---|---|
| Create configuration | Project asset | Nothing existing by default | Yes | Unity undo/create-only | Setup report |
| Create root prefab | Project prefab | Optional assigned scene instance | Yes | Preview/Undo | Setup report |
| Create fixed slot templates | Project assets/subassets | Configuration only after approval | Yes | Undo | ID report |
| Repair missing references | Only selected safe references | Selected project asset | Yes | Undo | Repair report |
| Create test participant | Project/sample script template | New file only | Yes with unique name | VCS/preview | Generation report |
| Create sandbox profile | Editor prefs/project test config | No production root | Yes | Delete-safe | Sandbox path report |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Chronicle Setup | Installer | Configure root, slot model, paths, retention, providers | No |
| Chronicle Validator | Developer/QA | Run configuration, ID, path, migration, root, and assembly checks | No |
| Save Browser | Developer/support | List slots/generations/manifests/health without applying payloads | No |
| Generation Inspector | Developer | Show versions, participants, sizes, hashes, provenance | No |
| Migration Graph | Maintainer | Visualize contiguous document/participant migration chains | No |
| Failure Simulator | QA | Truncate, lock, corrupt, orphan, age, or version sandbox records | No |
| Recovery Planner | QA/support | Preview candidate recovery actions | No |
| Test Data Generator | QA | Create bounded synthetic slots and payloads in sandbox | No |
| Redacted Snapshot Exporter | Support | Export health/manifest data only | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix? | Safe auto-fix? |
|---|---|---|---:|---:|
| ESV-VAL-001 | Missing configuration | Blocker | Yes | Yes, create new asset only |
| ESV-VAL-002 | Unsafe/empty root subpath | Blocker | Yes | No |
| ESV-VAL-003 | Duplicate root in scene/prefab setup | Error | Yes | No, user chooses authority |
| ESV-VAL-004 | Duplicate fixed slot IDs | Blocker | Yes | Only before any release/save exists |
| ESV-VAL-005 | Invalid retention bounds | Error | Yes | Yes to safe defaults |
| ESV-VAL-006 | Missing serializer/backend provider | Blocker | Yes | Yes if default selected |
| ESV-VAL-007 | Missing migration step | Blocker for affected version | No automatic | No |
| ESV-VAL-008 | Participant ID collision in analyzable registrations | Error | Guidance | No |
| ESV-VAL-009 | Runtime assembly references `UnityEditor` | Blocker | Guidance | No |
| ESV-VAL-010 | Production root equals Laboratory sandbox | Blocker | Yes | No |
| ESV-VAL-011 | Catalog cache inconsistent | Warning | Rebuild | Yes, cache only |
| ESV-VAL-012 | Orphan/incomplete generations | Info/Warning | Quarantine/clean plan | Only sandbox; production requires confirmation |
| ESV-VAL-013 | Corrupt current generation with valid fallback | Error | Recovery plan | No silent publication by Editor tool |
| ESV-VAL-014 | File/payload exceeds configured limits | Error | Inspect/quarantine | No |
| ESV-VAL-015 | Display name used in physical path | Blocker | Migration plan | No |

All repair tools preview targets, preserve backups where practical, and report exact changes. Production save mutation requires explicit confirmation.

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Supported for the first release:

- Embedded package development.
- Local UPM path.
- Git URL/tag.
- Tarball installation.
- Workshop selection when The Workshop exists.

Registry distribution may be added later. Exact manifest dependency versions are verified at M1.

### 12.2 Minimal scene setup

Production minimum:

1. One `EchoSaveRoot` with assigned `EchoSaveConfiguration`, or project-created root through explicit API.
2. No participant is required merely to initialize.
3. Game systems register participants when their runtime instances become authoritative.
4. Project calls `InitializeAsync` or enables approved auto-initialize.
5. Project creates/selects slots and requests operations.

No required scene name, build index, tag, layer, EventSystem, Canvas, input asset, or other Echo package exists.

### 12.3 Boot-scene setup

Recommended but optional:

- Place root in canonical Boot/preload scene.
- First Light bridge initializes and reports catalog health.
- Root persists through destination scene.
- Participants register as their owning systems initialize.
- Continue flow prepares load, chooses destination through project/Passage, then applies.

### 12.4 Direct-scene setup

`EchoSaveDirectSceneInitializer` is development-only:

- checks for an existing authority;
- creates only the configured development root when absent;
- identifies direct-scene mode in diagnostics;
- uses the project’s development configuration or an explicit sandbox;
- never creates a second root;
- may be excluded from release builds;
- can require canonical Boot for sensitive integration tests.

### 12.5 Path safety

- Default root is a child directory beneath `Application.persistentDataPath`.
- Configuration accepts a normalized relative subpath, not an arbitrary unchecked player path.
- Absolute/custom paths are advanced Editor/development options behind explicit policy.
- Slot IDs, generation IDs, and fixed filenames are package-generated/validated.
- Display names never become directory names.
- All backend operations verify that resolved paths remain inside the allowed root.

### 12.6 Scene isolation rule

The Save Laboratory contains no peer Echo runtime package. It uses a sandbox storage backend/root, sample participants, simple project-owned DTOs, and sample presentation only.

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Test Lab purpose

The **Chronicle Save Laboratory** proves the complete isolated loop:

1. initialize a sandbox root;
2. register multiple versioned participants;
3. create/select a slot;
4. save a validated generation;
5. mutate runtime state;
6. prepare and apply load;
7. inspect metadata/generations;
8. simulate interruption/corruption/version mismatch;
9. recover a prior generation;
10. reset and repeat without touching production data.

### 13.2 Required Laboratory contents

- One scene and sample README.
- Explicit sandbox path displayed prominently.
- Sample participants: Player, World, Optional Package.
- Plain controls for slot create/select/rename/duplicate/delete-plan/confirm.
- Save, autosave, prepare, apply, one-step load, cancel, and reset controls.
- Runtime values visibly editable so restoration is obvious.
- Catalog, active slot, operation phase, participant registry, unknown payload count, generation list, and diagnostic-code readouts.
- Failure simulation: capture failure, apply failure, missing participant, locked backend, truncated payload, bad checksum, missing head, old version, newer version, oversized payload, incomplete generation.
- Recovery planner and explicit candidate selection.
- No copyrighted or project-owned production content.

### 13.3 Laboratory acceptance checklist

| Test | Action | Expected result | Type |
|---|---|---|---|
| LAB-001 | Enter scene directly | One sandbox authority initializes | Manual/automated |
| LAB-002 | Add duplicate root | Duplicate exits before storage side effects | Automated |
| LAB-003 | Create single slot | Stable ID and empty metadata appear | Manual/automated |
| LAB-004 | Save three participants | Verified generation becomes current | Manual/automated |
| LAB-005 | Mutate then load-and-apply | Values restore exactly | Manual/automated |
| LAB-006 | Prepare load, wait, apply | Handle remains valid and applies | Manual |
| LAB-007 | Dispose prepared load | Memory releases; disk unchanged | Automated |
| LAB-008 | Rename slot | Display name changes; path/ID remains | Manual/automated |
| LAB-009 | Duplicate slot | New ID with equivalent state | Manual/automated |
| LAB-010 | Delete without confirmation | No mutation | Automated |
| LAB-011 | Confirm deletion plan | Slot moves/deletes per sandbox policy | Manual/automated |
| LAB-012 | Request rapid autosaves | At most one pending request remains | Automated |
| LAB-013 | Fail required capture | No generation publishes | Automated |
| LAB-014 | Fail optional capture under default policy | Save fails visibly | Automated |
| LAB-015 | Remove optional participant after load | Unknown payload is preserved on next save | Automated |
| LAB-016 | Explicitly prune unknown entry | Only named entry is removed after confirmation | Manual/automated |
| LAB-017 | Truncate payload | Checksum/parse failure reports corruption | Manual/automated |
| LAB-018 | Damage head pointer | Scan finds newest complete generation | Manual/automated |
| LAB-019 | Interrupt before head update | Prior head remains current | Automated backend |
| LAB-020 | Interrupt after head update | New verified generation is current | Automated backend |
| LAB-021 | Missing migration step | Prepare blocks; source unchanged | Automated |
| LAB-022 | Complete migration chain | Prepared payload reaches current version | Automated |
| LAB-023 | Newer document version | Load refuses without rewrite | Automated |
| LAB-024 | Oversized payload | Read rejects before allocation beyond cap | Automated |
| LAB-025 | Locked backend | Operation fails; current generation survives | Automated |
| LAB-026 | Cancel while queued | Request cancels cleanly | Automated |
| LAB-027 | Cancel before publication | No current-head change | Automated |
| LAB-028 | Cancel after publication begins | Operation finishes and reports too late | Automated |
| LAB-029 | Rebuild catalog cache | Slot list matches manifests | Manual/automated |
| LAB-030 | Delete sample UI | Runtime package still compiles | Packaging |
| LAB-031 | Reset laboratory | Sandbox returns to known empty state | Manual |
| LAB-032 | Shutdown/re-enter | Authority and callbacks clean up | Automated |

### 13.4 Optional integration samples

| Sample | Packages | Purpose | Not standalone proof because |
|---|---|---|---|
| Chronicle + Looking Glass | EchoSave, EchoUI | Slot list, confirmation, progress, recovery presenter | Depends on UI bridge |
| Chronicle + Passage | EchoSave, EchoSceneFlow | Prepare, transition, apply | Depends on two authorities |
| Chronicle + First Light | EchoSave, EchoLaunch | Startup initialization and continue candidate | Depends on launch bridge |
| Chronicle + Accord | EchoSave, EchoSettings | Optional last-selected-slot preference | Depends on settings bridge |
| Chronicle + Observatory | EchoSave, EchoDiagnostics | Save-health panel | Depends on diagnostics bridge |

Samples are separately importable and removable.

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

EchoSave runtime core is nonvisual. It exposes immutable metadata, operation progress phases, structured results, recovery plans, and diagnostic snapshots. Production presentation belongs to project code or The Looking Glass through a bridge.

The Laboratory may use uGUI/TMP as sample-only presentation. Removing it cannot affect runtime assemblies.

### 14.2 Required presentation states

Consumers must be able to represent:

- Initializing/catalog scan.
- Ready with no slots.
- Ready with slots.
- Slot selected/unselected.
- Busy saving/loading/duplicating/deleting/recovering.
- Save/load success.
- Permission denied.
- Storage unavailable.
- Corrupt current generation with/without recovery candidate.
- Unsupported older version/missing migration.
- Unsupported newer version.
- Required participant missing.
- Cancellation accepted/too late.
- Recovery preview and explicit confirmation.
- Degraded catalog/cache status.

### 14.3 Accessibility requirements

- Results and health cannot rely on color alone.
- Progress must include textual phase/state.
- Destructive actions require explicit readable confirmation through the presenter/project.
- Save completion must not rely only on sound.
- Timed confirmations, if any, are presenter-owned and must use accessibility-aware timing.
- Slot metadata uses localization-friendly display fields and project-owned formatting.
- File paths and technical codes may be copyable in development/support surfaces.

### 14.4 Visual customization

The project owns slot layouts, fonts, icons, thumbnails, labels, date formatting, save-kind names, and all player-facing copy. EchoSave supplies data contracts, not a mandatory visual identity.

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Initialization/root state | API/Inspector/log | All builds as configured | Constant |
| Storage backend/capabilities | Snapshot | Development/release-safe summary | Constant |
| Active slot and catalog health | API/snapshot | Development; redacted release | Bounded |
| Current operation phase/progress | API/events | All builds | Event-driven |
| Participant registry summary | API/snapshot | Development | Bounded by configured cap |
| Slot/generation manifest health | Editor/API | Development/support | I/O on demand |
| Migration chain status | Editor/validation | Editor | On demand |
| Recent operation history | Bounded buffer | Development | Configurable |
| Redacted support export | File export | Explicit request | On demand |

### 15.2 Structured status

Snapshots include:

- package version;
- root identity and duplicate/direct-scene mode;
- initialization state;
- normalized/redacted storage root token, not full path in public mode;
- backend/serializer/integrity provider IDs and capabilities;
- catalog slot counts and health counts;
- active slot ID in hashed/redacted form for support mode;
- current operation ID/type/phase/duration/progress;
- queue/coalesced autosave counts;
- participant IDs, versions, criticality, and registration health;
- prepared-load count and memory estimate;
- unknown payload count/bytes by ID only;
- latest stable diagnostic codes;
- no payload contents.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| ESV-CFG-001 | Blocker | Configuration missing | Assign/create configuration |
| ESV-CFG-002 | Error | Invalid limit/retention policy | Correct configuration |
| ESV-LIFE-001 | Warning | Duplicate root rejected | Remove duplicate setup |
| ESV-LIFE-002 | Warning | Direct-scene development initializer used | Verify canonical Boot for release tests |
| ESV-PATH-001 | Blocker | Resolved path escaped allowed root | Correct root/subpath |
| ESV-IO-001 | Error | Storage unavailable or operation failed | Check platform/path/permissions |
| ESV-IO-002 | Error | Flush/replace capability unavailable or failed | Use fallback/retry; inspect backend |
| ESV-SLOT-001 | Error | Slot not found | Refresh/select valid slot |
| ESV-SLOT-002 | Error | Invalid slot/display request | Correct request |
| ESV-SLOT-003 | Warning | Slot unhealthy/degraded | Inspect recovery plan |
| ESV-SLOT-004 | Warning | Slot capacity reached | Delete/increase configured capacity |
| ESV-OP-001 | Info | Operation rejected because busy | Retry later |
| ESV-OP-002 | Info | Autosave coalesced | No action |
| ESV-OP-003 | Warning | Operation queue full | Reduce request spam/configure policy |
| ESV-OP-008 | Info | Cancellation arrived after publication began | Operation will settle normally |
| ESV-PART-001 | Error | Duplicate participant ID | Fix IDs/registration |
| ESV-PART-002 | Error | Invalid participant descriptor | Correct ID/version/policy |
| ESV-PART-004 | Error | Required participant missing | Initialize/register before apply |
| ESV-PART-006 | Error | Participant capture failed | Inspect participant result |
| ESV-PART-007 | Error | Participant apply failed | Inspect apply report/rollback |
| ESV-SER-001 | Error | Serializer provider missing | Install/select provider |
| ESV-SER-002 | Error | Payload serialization failed | Fix DTO/provider |
| ESV-DATA-001 | Error | Manifest/payload parse failed | Build recovery plan |
| ESV-DATA-002 | Error | Manifest/payload mismatch | Recover/quarantine |
| ESV-DATA-003 | Error | Checksum mismatch | Recover/quarantine |
| ESV-DATA-004 | Warning | Unknown payload preserved | Reinstall owner or explicitly prune |
| ESV-MIG-001 | Info | Migration applied in memory | Save later to current format |
| ESV-MIG-002 | Error | Migration chain missing | Supply migration or keep old build |
| ESV-MIG-003 | Error | Migration failed | Inspect migration/test source |
| ESV-MIG-004 | Error | Save format newer than supported | Do not overwrite; use newer build |
| ESV-REC-001 | Warning | Head invalid; generation scan used | Inspect storage health |
| ESV-REC-002 | Warning | Prior generation selected | Inform player/support |
| ESV-REC-004 | Error | No valid recovery candidate | Preserve files; restore external backup if available |
| ESV-SEC-001 | Error | Invalid ID/path component | Reject external record |
| ESV-SEC-002 | Error | File/payload exceeds safety limit | Quarantine/reject |
| ESV-SEC-003 | Warning | Integrity hash is not authentication | Do not treat as anti-tamper |

### 15.4 Observatory bridge

Separate bridge publishes provider snapshots and bounded events. EchoSave core never references The Observatory. The bridge must honor privacy modes and omit payload content, full paths, display names, thumbnails, and arbitrary project metadata unless explicitly approved.

### 15.5 Logging policy

- Stable code in every actionable log.
- No per-frame logging or polling.
- One start and one terminal record per operation at normal development verbosity.
- Payload contents and player-facing names excluded by default.
- Full local paths Editor-only unless explicit support mode.
- Repeated identical health warnings rate-limited.

## 16. Persistence and Save Integration

### 16.0 Separation of transport, live truth, and object lifetime

EchoSave owns **durable game-save transport**, not all persistent-looking concerns.

| Concern | Chronicle authority |
|---|---|
| Save files, slots, generations, manifests, integrity, migration, backup/recovery, orchestration | Owns |
| Global preferences such as graphics/audio/accessibility/control choices | Does not own; The Accord owns |
| Inventory/objective/progression/character/world live state | Does not own; participant/project authority owns |
| Participant payload schema and semantic meaning | Does not own; participant/project authority owns |
| Scene-surviving lifetime of `EchoSaveRoot` | Owns only its package-local lifecycle/duplicate claim |
| Project-wide `DontDestroyOnLoad` hierarchy/service composition | Does not own; consumer project composes |
| First Light startup sequencing | Does not own; First Light may initialize/discover EchoSave, then hands off |

A peer package can be persistence-capable without referencing EchoSave. The peer exposes a detached/versioned snapshot/import-export contract under its own authority; a separate bridge/participant adapter translates that contract to EchoSave when both are installed.

After a successful load, EchoSave applies/restores detached data into the participant's runtime authority. EchoSave does not remain the authoritative source of that participant's live state.

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Slot/generation documents | Durable slot | EchoSave | Yes | Selected save backend |
| Participant payload | Durable slot | Participant/project; transported by EchoSave | Yes | Selected save backend |
| Global preferences | Global/profile | The Accord | No in EchoSave by default | Settings backend |
| Active slot selection | Session | EchoSave | No by default | Optional Accord/project bridge |
| Catalog cache | Derived local cache | EchoSave | Yes but rebuildable | Save backend |
| Operation history | Development session | EchoSave | No by default | Memory/redacted export |
| Prepared load | Temporary session | EchoSave/caller | No | Memory |
| Unknown payload store | Loaded session/next save | EchoSave transport | Preserved into generation | Save backend |

### 16.2 Standalone behavior

Without The Accord, EchoSave still stores game saves. It does not persist last-selected-slot preference unless project code explicitly chooses to do so. Without EchoUI, slot metadata and operations remain available through API. Without Passage, the project may prepare and apply in the same scene or coordinate scene changes manually.

### 16.3 Participant contribution contract

A participant contributes one independent versioned payload. EchoSave does not read project fields, databases, static stores, or ScriptableObject definitions directly. Registration and payload ID remain stable across releases. Participant documentation must define:

- data captured;
- mutable runtime owner;
- schema version;
- stable IDs referenced inside payload;
- migration chain;
- missing/optional behavior;
- capture/apply failure behavior;
- test cases.

### 16.4 Failure and recovery

- Missing root directory: create if policy permits.
- Missing slot: return Not Found.
- Missing head with valid generations: produce recovery plan/automatic safe recovery only if configured.
- Corrupt current generation: never overwrite before a verified candidate is chosen.
- Older supported format: migrate in memory.
- Newer format: preserve and refuse.
- Locked file/backend: current generation remains unchanged.
- Partially written generation: ignore/quarantine; prior head remains.
- Partially updated cache: rebuild from manifests.
- Deleted slot: recover from bounded trash only if policy enabled and retention not expired.

### 16.5 Save permission and restrictions

EchoSave exposes `ISavePermissionProvider` and request validation. The default standalone provider allows save/load. A game or Pulse bridge may deny manual saves during combat, loading, cutscenes, death, or other states and return a localized/presentation-ready reason key. EchoSave does not hardcode those rules.

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Peer connections are explicit, removable, versioned, and independently tested. Installing a peer does not silently change EchoSave behavior. The local core remains complete.

### 17.2 Planned integrations

| Other authority | Connection | Bridge owner | Direction | Data/events | Required? |
|---|---|---|---|---|---:|
| First Light | Startup step | Separate bridge | Launch -> Save | Initialize, catalog health, continue candidates | No |
| Observatory | Diagnostics provider | Separate bridge | Save -> Diagnostics | Health, operation, catalog summary | No |
| Accord | Last-selected-slot/global save preference | Separate/project adapter | Both | Slot ID preference only, never game payload | No |
| Passage | Prepared-load scene coordination | Separate/project adapter | Both | Prepared handle, destination metadata, apply command | No |
| Pulse | Permission/loading scope | Separate bridge | Both | Save permission, Loading scope requests | No |
| Resonance | Semantic result feedback | Project/UI bridge | Save event -> Audio request | Success/failure cue intent | No |
| Will | UI input lock/context | Project/UI bridge | UI operation -> Input request | Lock/context lease | No |
| Looking Glass | Slot/recovery presenters | Separate bridge | Both | Metadata, progress, results, commands | No |
| Workshop | Editor composition | Workshop | Workshop -> project | Configuration/root/sample/report | No |
| Progression/Characters/Inventory/Objectives | Save participant adapters | Owning package/project | Both | Versioned payloads | No |
| Cloud/platform provider | Storage adapter | Separate EchoSave-family package | Core <-> provider | Files/objects/sync state | No |

### 17.3 Bridge placement decision

- Two-package Echo bridges ship separately by default.
- Tiny project-specific permission, metadata, and scene coordination remain project-local adapters.
- Cloud/platform SDK integrations are separate provider packages.
- Participant adapters normally live with the package that owns the mutable state.

### 17.4 Integration failure behavior

- Missing bridge: core continues.
- Missing peer: bridge disables itself with one actionable diagnostic.
- Version mismatch: bridge reports incompatible state and does not register partial behavior.
- Peer initialized late: explicit registration/retry, no broad reflection scan.
- Peer removed after save: unknown participant payload remains preserved.
- Scene coordination failure: prepared load may remain for retry until disposal/expiry; no participant apply occurs automatically.
- Shutdown order: registrations/leases dispose idempotently; core does not call destroyed peers.

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Target | Measurement | Release threshold |
|---|---|---|---|
| Idle runtime overhead | No required `Update`; zero package allocations per idle frame | Profiler in empty scene | Pass |
| Catalog snapshot | Memory-only after initialization | 32-slot Laboratory | No file I/O on caller path |
| Catalog refresh | Manifest/head files only, payloads unopened | 32/100-slot sandbox | Completes asynchronously without long main-thread stall |
| Save main-thread package overhead | Participant capture scheduling and detached snapshot assembly only | 50 participants/5 MB synthetic save | Package overhead profiled separately; participant work identified |
| Save I/O | Background where backend permits | 5 MB/25 MB sandbox | No synchronous full-file write on main thread |
| Operation queues | Bounded | Stress test | Never exceed configured capacity |
| Histories/generations | Bounded retention | 100 saves | Stable disk/memory growth |
| Prepared loads | Bounded count and bytes | Stress test | Reject beyond configured cap |
| Unknown payloads | Bounded count/bytes | Preservation test | Reject/quarantine oversized input |

Exact millisecond budgets are recorded after M2 profiling on the supported reference hardware. The specification refuses to hide participant-owned capture/apply cost inside package claims.

### 18.2 Allocation policy

- No LINQ in hot operation loops unless profiling approves it.
- Pool small internal buffers only when ownership is unambiguous.
- Serialize from detached DTOs, not live scene graphs.
- Stream/file APIs should avoid unnecessary duplicate copies where serializer/backend permits.
- A complete payload may require source DTO, serialized bytes/string, and verification buffer; memory estimates are exposed and capped.
- Large-world streaming/chunking is deferred rather than pretending the MVP handles arbitrary size.

### 18.3 Scene and domain reload behavior

- Static authority reset is explicit for supported Enter Play Mode configurations.
- All callbacks and participant registrations dispose cleanly.
- Runtime file streams are scoped and disposed with `using`/equivalent.
- Prepared handles become invalid on domain reload/shutdown.
- Scene unload unregisters scene-owned participants through owned registrations.
- Root persistence across scenes does not preserve destroyed participant instances.

### 18.4 Scalability limits

Configuration must bound:

- slots;
- generations per slot;
- autosave history;
- trash history;
- participant count;
- payload entry bytes and total bytes;
- manifest size;
- migration steps;
- prepared-load count/bytes;
- operation queue length;
- diagnostic history.

“Unlimited profiles” means no design-fixed slot count, not infinite resources. Platform and configured safety limits still apply.

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Game saves may contain player-created names, play history, progress, choices, and project-defined personal content. EchoSave does not knowingly store credentials, authentication tokens, payment data, or platform secrets. Provider adapters must define their own sensitivity and compliance rules.

### 19.2 Trust boundaries

- Treat files as untrusted input.
- Validate IDs, normalized paths, counts, versions, lengths, checksums, and manifest/payload agreement before deserialization/application.
- Enforce maximum file and payload sizes before allocating large buffers.
- Reject path traversal and absolute paths outside approved root.
- Never instantiate arbitrary types named by a save file. Serializer/type binding comes from registered participants/providers.
- Unknown payloads are opaque data, never executable content.
- SHA-256 or equivalent integrity hashes detect accidental corruption but do not authenticate or prevent deliberate tampering.
- Encryption/authentication require a separate approved provider and key policy.
- Redacted exports omit payload contents, full paths, display names, thumbnails, and arbitrary metadata by default.

### 19.3 Platform behavior

| Platform | Initial status | Special behavior | Validation |
|---|---:|---|---|
| Windows | Primary supported | Local filesystem beneath persistent data path; replace capability tested | Clean install, lock, interruption, recovery |
| macOS | Supported target | Path/case/permission behavior verified | External install and recovery tests |
| Linux | Supported target | Case-sensitive paths and permissions | External install and recovery tests |
| WebGL | Conditional/Deferred public claim | Browser-backed persistence/synchronization semantics differ | Dedicated backend/platform tests required before claim |
| Android/iOS | Planned supported | Lifecycle/storage quotas and background suspension | Device interruption and restore tests |
| Consoles | Unknown/provider-specific | Certification, storage APIs, user accounts | Approved provider required |

### 19.4 Technical basis

- Unity documents `Application.persistentDataPath` as the persistent per-application data directory and ties mobile/update continuity to the bundle identifier.
- Unity `JsonUtility` serializes fields on plain serializable classes/structs under Unity serialization rules; the default serializer therefore documents unsupported dictionaries, polymorphic graphs, interfaces, and durable Unity object references.
- Unity `Awaitable` supports switching to background and main threads; EchoSave keeps Unity API and participant application on the main thread and uses background work only for detached data/provider operations.
- .NET `File.Replace` can replace a destination while creating a backup on supported filesystems. The backend advertises actual capability and uses recovery-safe fallback when unavailable rather than claiming universal atomicity.

Reference URLs are maintained in `Documentation~/Developer/Platform-And-Serialization-Basis.md` at implementation time.

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-save/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
├── Runtime/
├── Editor/
├── Samples~/
└── Tests/
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoSaveRoot.cs
│   ├── EchoSaveService.cs
│   ├── EchoSaveServiceState.cs
│   ├── SaveOperationCoordinator.cs
│   └── EchoDevGames.EchoSave.Runtime.asmdef
├── Configuration/
│   ├── EchoSaveConfiguration.cs
│   ├── SaveSlotTemplate.cs
│   ├── SaveRetentionPolicy.cs
│   ├── SaveLimitPolicy.cs
│   └── SaveRecoveryPolicy.cs
├── Slots/
│   ├── SaveSlotId.cs
│   ├── SaveGenerationId.cs
│   ├── SaveSlotCatalog.cs
│   ├── SaveSlotMetadata.cs
│   ├── SaveCatalogSnapshot.cs
│   └── SaveDeletionPlan.cs
├── Documents/
│   ├── SaveHeadPointer.cs
│   ├── SaveManifest.cs
│   ├── SavePayloadDocument.cs
│   ├── SavePayloadEntry.cs
│   └── SaveDocumentVersions.cs
├── Participants/
│   ├── ISaveParticipant.cs
│   ├── SaveParticipantDescriptor.cs
│   ├── SaveParticipantRegistry.cs
│   ├── SaveParticipantRegistration.cs
│   └── UnknownPayloadStore.cs
├── Operations/
│   ├── SaveRequest.cs
│   ├── SaveOperationResult.cs
│   ├── SaveLoadRequest.cs
│   ├── PreparedSaveLoad.cs
│   ├── SaveLoadResult.cs
│   ├── SaveRecoveryPlan.cs
│   └── SaveOperationPhase.cs
├── Serialization/
│   ├── ISaveSerializer.cs
│   ├── SaveSerializerRegistry.cs
│   └── UnityJsonSaveSerializer.cs
├── Storage/
│   ├── ISaveStorageBackend.cs
│   ├── LocalFileSaveStorageBackend.cs
│   ├── SaveStorageCapabilities.cs
│   └── SavePathPolicy.cs
├── Integrity/
│   ├── IIntegrityProvider.cs
│   └── Sha256IntegrityProvider.cs
├── Migration/
│   ├── ISaveDocumentMigration.cs
│   ├── ISaveParticipantMigration.cs
│   └── SaveMigrationRegistry.cs
├── Diagnostics/
│   ├── SaveDiagnosticCode.cs
│   ├── SaveDiagnosticSnapshot.cs
│   └── SaveDiagnosticsBuffer.cs
└── Development/
    └── EchoSaveDirectSceneInitializer.cs

Editor/
├── Setup/
│   ├── EchoSaveSetupWindow.cs
│   └── EchoSaveSetupService.cs
├── Validation/
│   ├── EchoSaveValidatorWindow.cs
│   └── EchoSaveValidationRule.cs
├── Inspection/
│   ├── SaveBrowserWindow.cs
│   ├── SaveGenerationInspector.cs
│   └── SaveMigrationGraphWindow.cs
├── Simulation/
│   ├── SaveFailureSimulatorWindow.cs
│   ├── SaveRecoveryPlannerWindow.cs
│   └── SaveTestDataGenerator.cs
└── EchoDevGames.EchoSave.Editor.asmdef

Samples~/
└── Standalone Labs/
    └── Chronicle Save Laboratory/

Tests/
├── Editor/
│   └── EchoDevGames.EchoSave.Tests.Editor.asmdef
└── Runtime/
    └── EchoDevGames.EchoSave.Tests.Runtime.asmdef
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoSave.Runtime` | Any | UnityEngine modules only | Yes | Core runtime |
| `EchoDevGames.EchoSave.Editor` | Editor | Runtime, UnityEditor | No | Setup/validation/inspection |
| `EchoDevGames.EchoSave.Tests.Editor` | Editor tests | Runtime, Editor, Test Framework | No | Validation/tool tests |
| `EchoDevGames.EchoSave.Tests.Runtime` | PlayMode | Runtime, Test Framework | No | Lifecycle/operation tests |

### 20.4 Repository files

- README and five-minute quick start.
- Architecture, generation-commit, participant, serializer, storage, migration, recovery, security, and Test Laboratory guides.
- Current Notes and checkpoint records.
- Diagnostic-code reference.
- Sample README.
- Changelog, license, notices, contribution/support/security guidance.
- Stable `.meta` files and GUIDs.

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Primary tested | Notes |
|---|---|---|---|
| Unity | 6000.0 | 6000.3.8f1 | Exact API/package compatibility reverified at M1 and release |
| .NET profile | Unity-supported | Unity baseline default | File/crypto capability tested per platform |
| uGUI/TMP | Sample only | Compatible release | Not a runtime-core dependency |

### 21.2 Semantic versioning policy

Patch:

- fixes preserving public API, IDs, file formats, and behavior;
- validator/tool fixes;
- new diagnostic detail without breaking parsers.

Minor:

- additive APIs, providers, optional manifest fields, migration steps, diagnostics, or sample features;
- new backward-compatible slot/retention options.

Major:

- incompatible public API;
- document or participant contract changes requiring consumer migration;
- changed default destructive/recovery behavior;
- removal/rename of stable IDs without aliases;
- format changes not readable by prior supported releases.

### 21.3 Deprecation policy

- Mark APIs/formats deprecated in code and docs.
- Provide replacement and migration path.
- Preserve read compatibility for the documented support window.
- Never silently delete old migration steps while supported saves may exist.
- Removal requires a major version and release notes.

### 21.4 GUID and asset compatibility

Public scripts, configuration assets, templates, sample prefabs/scenes, and migration assets preserve `.meta` GUIDs. Moves retain identity. Slot/participant/serializer/storage IDs are serialization contracts independent from Unity asset names.

### 21.5 Save-format compatibility

- Package document format uses explicit major/minor/version fields.
- Readers reject unsupported newer major formats.
- Additive optional fields may be tolerated when version policy permits.
- Participant versions are independent.
- Downgrade is not promised.
- Unknown participant payloads remain preserved.

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview and authority boundaries.
- Installation and five-minute quick start.
- Slot model selection guide.
- Configuration reference.
- Participant authoring guide.
- Save/load API examples.
- Two-phase load and scene coordination guide.
- Save Laboratory guide.
- Recovery and backup guide.
- Troubleshooting and diagnostic codes.
- Upgrade/migration guide.
- Known limitations and platform notes.
- License/notices.

### 22.2 Required developer documentation

- Architecture and root lifecycle.
- Immutable generation/head publication design.
- Local storage layout and path policy.
- Participant contract and unknown payload preservation.
- Serializer/provider contract and Unity JSON limitations.
- Document and participant migrations.
- Async/main-thread/background-thread rules.
- Operation admission/cancellation.
- Integrity/security/privacy limits.
- Testing strategy and failure simulator.
- Release workflow and Current Notes.

### 22.3 Documentation truth rule

Examples compile against the release. File layouts, menu paths, versions, diagnostic codes, and migration behavior match shipped code. A save format is not release-ready when the documentation cannot explain how old, new, corrupt, and interrupted files behave.

### 22.4 Living repository and Obsidian workflow

At each checkpoint:

1. Reconcile Current Notes.
2. Promote behavior/API/format changes into this specification or an ADR.
3. Update migration, recovery, test, setup, and changelog records.
4. Verify documentation against committed tests and generated files.
5. Commit documentation with or adjacent to implementation.

### 22.5 Repository scan and handoff order

1. README/index.
2. SFGSS-000.
3. This specification.
4. Save-format/migration ADRs.
5. Current Notes.
6. Current checkpoint, tests, issue log, changelog.
7. Relevant code and generated test records.

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | MVP? |
|---|---|---|---:|
| EditMode unit | IDs, paths, manifests, policies, serializers, migrations, recovery selection | Pure deterministic tests | Yes |
| PlayMode lifecycle | Root, participants, async coordinator, scene persistence, shutdown | Duplicate/direct-scene tests | Yes |
| Backend fault tests | Partial writes, locks, unavailable replace, out-of-space simulation | Fake/in-memory/faulting backend | Yes |
| Standalone Laboratory | Visible save/load/recovery loop | 32 acceptance scenarios | Yes |
| Bridge Integration Lab | Optional peer connection | UI/Passage/First Light examples | When bridge ships |
| Clean-project install | Packaging and missing-peer proof | Git/tarball/external project | Yes |
| Existing-project migration | Adoption parity and rollback | Rescuers2D/Echo Systems Lab target | Before replacement claim |
| Platform tests | Filesystem/lifecycle behavior | Windows/macOS/Linux and later mobile | Per support claim |

### 23.2 Required categories

- Installation/removal.
- Root lifecycle and duplicates.
- Configuration/path/provider failure.
- Slot policies and destructive plans.
- Catalog/cache rebuild.
- Participant capture/apply and unknown payload preservation.
- Save admission, autosave coalescing, cancellation.
- Commit interruption at every boundary.
- Prepare/apply load.
- Integrity, size, and path security.
- Document/participant migration.
- Recovery candidate selection and publication.
- Performance/resource bounds.
- Direct-scene entry.
- Optional integrations absent/present.
- Existing-project parity/rollback.

### 23.3 Test case registry

| Test ID | Category | Action | Expected result | Status |
|---|---|---|---|---|

| ESV-T-001 | Install | Clean project install | Package compiles and setup opens | Planned |
| ESV-T-002 | Install | Tarball install | Package compiles with stable GUIDs | Planned |
| ESV-T-003 | Install | Remove sample | Runtime still compiles | Planned |
| ESV-T-004 | Install | No UnityEditor reference in runtime | Assembly audit passes | Planned |
| ESV-T-005 | Lifecycle | One root claims | Service initializes once | Planned |
| ESV-T-006 | Lifecycle | Duplicate before play | Duplicate has zero side effects | Planned |
| ESV-T-007 | Lifecycle | Duplicate during scene load | Original remains authority | Planned |
| ESV-T-008 | Lifecycle | Shutdown and recreate | Authority clears cleanly | Planned |
| ESV-T-009 | Lifecycle | Domain reload disabled | Static state resets correctly | Planned |
| ESV-T-010 | Config | Missing configuration | Blocked result ESV-CFG-001 | Planned |
| ESV-T-011 | Config | Unsafe path | Operation refuses ESV-PATH-001 | Planned |
| ESV-T-012 | Config | Invalid limits | Validator blocks | Planned |
| ESV-T-013 | Config | Missing serializer | Initialization blocks | Planned |
| ESV-T-014 | Config | Missing backend | Initialization blocks | Planned |
| ESV-T-015 | Slots | Create single slot | Stable ID and metadata created | Planned |
| ESV-T-016 | Slots | Fixed slot capacity | Extra slot rejected | Planned |
| ESV-T-017 | Slots | Configurable capacity | Configured limit enforced | Planned |
| ESV-T-018 | Slots | Unlimited policy safety cap | Platform/config cap enforced | Planned |
| ESV-T-019 | Slots | Rename slot | ID/path unchanged | Planned |
| ESV-T-020 | Slots | Duplicate slot | New ID and equivalent payload | Planned |
| ESV-T-021 | Slots | Delete without plan | No mutation | Complete |
| ESV-T-022 | Slots | Expired delete plan | Rejected | Complete |
| ESV-T-023 | Slots | Confirm delete | Trash/delete policy applied | Complete |
| ESV-T-024 | Catalog | List slots | Payload files unopened | Planned |
| ESV-T-025 | Catalog | Corrupt cache | Rebuild succeeds | Planned |
| ESV-T-026 | Catalog | Missing cache | Rebuild succeeds | Planned |
| ESV-T-027 | Participants | Register unique participants | Deterministic registry | Planned |
| ESV-T-028 | Participants | Duplicate participant ID | Later registration rejected | Planned |
| ESV-T-029 | Participants | Required capture success | Payload entry written | Planned |
| ESV-T-030 | Participants | Required capture failure | No generation published | Planned |
| ESV-T-031 | Participants | Optional capture default failure | Save fails visibly | Planned |
| ESV-T-032 | Participants | Missing required apply participant | Prepared load remains/reports | Planned |
| ESV-T-033 | Participants | Missing payload initialize default | Participant default policy runs | Planned |
| ESV-T-034 | Participants | Apply failure | Detailed partial/rollback report | Planned |
| ESV-T-035 | Participants | Out-of-order unregister | Registry remains correct | Planned |
| ESV-T-036 | Unknown | Removed optional participant | Opaque payload preserved | Planned |
| ESV-T-037 | Unknown | Reinstalled participant | Preserved payload applies | Planned |
| ESV-T-038 | Unknown | Explicit prune plan | Only selected payload removed | Planned |
| ESV-T-039 | Unknown | Oversized unknown payload | Rejected/quarantined | Planned |
| ESV-T-040 | Save | Basic save | Generation verifies and head advances | Planned |
| ESV-T-041 | Save | Second save | Prior generation retained | Planned |
| ESV-T-042 | Save | Rapid manual saves | Busy/reject policy enforced | Planned |
| ESV-T-043 | Autosave | Rapid autosaves | One pending latest request | Planned |
| ESV-T-044 | Autosave | Retention rotation | Bounds enforced after commit | Planned |
| ESV-T-045 | Save | Permission denied | No capture/write | Planned |
| ESV-T-046 | Save | Cancel queued | No side effects | Planned |
| ESV-T-047 | Save | Cancel pre-publication | Head unchanged | Planned |
| ESV-T-048 | Save | Cancel during head publication | Operation settles/TooLate | Planned |
| ESV-T-049 | Save | File lock | Current generation survives | Planned |
| ESV-T-050 | Save | Out of space simulation | Current generation survives | Planned |
| ESV-T-051 | Commit | Crash before generation complete | Incomplete ignored | Planned |
| ESV-T-052 | Commit | Crash after generation verify before head | Old head remains; orphan recoverable | Planned |
| ESV-T-053 | Commit | Crash during head fallback update | Recovery scan chooses valid generation | Planned |
| ESV-T-054 | Commit | Crash after head publish before cache | Head authoritative; cache rebuilds | Planned |
| ESV-T-055 | Load | Prepare valid generation | Handle created | Planned |
| ESV-T-056 | Load | Dispose prepared handle | No apply/disk mutation | Planned |
| ESV-T-057 | Load | Prepared handle expiry | Apply rejected | Planned |
| ESV-T-058 | Load | Load and apply same scene | State restored | Planned |
| ESV-T-059 | Load | Prepare then simulated scene change | Apply after participant registration | Planned |
| ESV-T-060 | Load | Wrong slot/generation identity | Validation rejects | Planned |
| ESV-T-061 | Integrity | Truncated manifest | Corrupt status | Planned |
| ESV-T-062 | Integrity | Truncated payload | Corrupt status | Planned |
| ESV-T-063 | Integrity | Checksum mismatch | Recovery plan | Planned |
| ESV-T-064 | Integrity | Manifest/payload entry mismatch | Rejected | Planned |
| ESV-T-065 | Integrity | Oversized manifest | Rejected before large allocation | Planned |
| ESV-T-066 | Integrity | Oversized payload | Rejected before apply | Planned |
| ESV-T-067 | Migration | Current version | No migration | Planned |
| ESV-T-068 | Migration | Contiguous document chain | Migrates in memory | Planned |
| ESV-T-069 | Migration | Missing document step | Blocks source unchanged | Planned |
| ESV-T-070 | Migration | Participant chain | Payload reaches current version | Planned |
| ESV-T-071 | Migration | Participant alias ID | Old ID maps safely | Planned |
| ESV-T-072 | Migration | Migration throws/fails | Prepare fails source unchanged | Planned |
| ESV-T-073 | Migration | Newer major format | Refused preserved | Planned |
| ESV-T-074 | Recovery | Missing head valid generations | Plan selects newest valid | Planned |
| ESV-T-075 | Recovery | Current corrupt prior valid | Prior candidate offered | Planned |
| ESV-T-076 | Recovery | Multiple valid candidates | Deterministic order | Planned |
| ESV-T-077 | Recovery | No candidate | Files preserved | Planned |
| ESV-T-078 | Recovery | Execute plan | Head/catalog update atomically/fallback safely | Planned |
| ESV-T-079 | Recovery | Stale recovery plan | Rejected | Planned |
| ESV-T-080 | Serializer | Unity JSON plain DTO | Round trip | Planned |
| ESV-T-081 | Serializer | Unsupported DTO shape | Actionable failure | Planned |
| ESV-T-082 | Serializer | Custom provider | Provider selected by ID | Planned |
| ESV-T-083 | Serializer | Provider missing on load | Structured failure | Planned |
| ESV-T-084 | Security | Path traversal ID | Rejected | Planned |
| ESV-T-085 | Security | Absolute external path | Rejected by default | Planned |
| ESV-T-086 | Security | Unknown type name in file | No arbitrary type activation | Planned |
| ESV-T-087 | Privacy | Redacted snapshot | No payload/full path/display name | Planned |
| ESV-T-088 | Performance | Idle root | No Update/allocations | Planned |
| ESV-T-089 | Performance | 32-slot catalog | Manifest-only async refresh | Planned |
| ESV-T-090 | Performance | 50 participants 5MB | Budgets measured/reportable | Planned |
| ESV-T-091 | Stress | 100 sequential saves | Retention/disk bounded | Planned |
| ESV-T-092 | Stress | Queue flood | Capacity enforced | Planned |
| ESV-T-093 | Stress | Prepared-load flood | Count/bytes cap enforced | Planned |
| ESV-T-094 | Direct scene | Development initializer | One sandbox root | Planned |
| ESV-T-095 | Direct scene | Production root already exists | No duplicate | Planned |
| ESV-T-096 | Integration | First Light absent/present | Both paths work | Planned |
| ESV-T-097 | Integration | Looking Glass bridge removed | Core compiles/operates | Planned |
| ESV-T-098 | Integration | Passage coordination failure | Prepared handle retry/dispose | Planned |
| ESV-T-099 | Migration adoption | Existing project parallel run | Old system remains rollback | Planned |
| ESV-T-100 | Release | External clean install and sample checklist | Pass | Planned |

### 23.4 Test data and isolation

- Automated tests use in-memory/fault-injecting backends where possible.
- File integration tests use unique temporary sandbox roots and delete only verified children.
- No test reads or deletes the normal production save root.
- Golden old-version fixtures are committed with licenses and checksums.
- Corruption fixtures include truncated, mismatched, oversized, missing-head, and newer-version records.
- Test clocks and deterministic generation IDs are injectable.
- Every bug affecting persistence receives a permanent regression fixture/test.

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Authority and exclusions approved.
- [x] MVP/deferred scope separated.
- [x] Slot/generation/participant models defined.
- [x] Two-phase load and migration defined.
- [x] Recovery and security boundaries defined.
- [x] Laboratory and test registry designed.
- [x] No release-blocking design question remains.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared dependencies only.
- [ ] Duplicate root produces zero storage side effects.
- [ ] Immutable generation publication is implemented and fault-tested.
- [ ] Participant and unknown-payload contracts match spec.
- [ ] Default serializer limitations are validated/documented.
- [ ] Main/background thread boundaries are tested.
- [ ] Setup/repair tools are repeatable and non-destructive.
- [ ] Public API changes update spec/ADR first.

### 24.3 Standalone gate

- [ ] Clean install succeeds.
- [ ] Package functions without peers.
- [ ] 32 Laboratory scenarios pass.
- [ ] Samples remove safely.
- [ ] Direct-scene behavior matches docs.
- [ ] Sandbox cannot collide with production root.

### 24.4 Quality gate

- [ ] Applicable 100-case registry passes.
- [ ] Fault injection covers every commit boundary.
- [ ] Old/new/corrupt/oversized records behave as documented.
- [ ] Unknown payload preservation passes.
- [ ] Performance/resource targets pass.
- [ ] Privacy/security review passes.
- [ ] Docs and diagnostic codes match build.
- [ ] Current Notes reconciled.
- [ ] License/notices complete.

### 24.5 Distribution gate

- [ ] Manifest/version/changelog valid.
- [ ] Stable `.meta` files included.
- [ ] Tarball/Git install tested externally.
- [ ] Platform support claims backed by tests.
- [ ] Migration fixture pack included.
- [ ] Release tag prepared.
- [ ] Compatibility catalog updated.
- [ ] Repository documentation committed/pushed.

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Echo Systems Lab | SaveManager/player/mission progress stores | Wrap existing state in participants, parallel-write to sandbox, compare, then switch reads | Save, quit, reload, mission unlock parity | Restore old manager and files |
| Rescuers2D | `SaveService`, fixed filename, bootstrap continue logic | Keep current service, add adapter/participant, migrate one copied save, coordinate continue through project code | New/continue, scene destination, password/progress parity | Re-enable old service/bootstrap path |
| Hackulos | Not yet implemented | Begin directly with participant boundaries when package is stable | Vertical-slice save/load/recovery tests | Project checkpoint/branch rollback |

### 25.2 Preserve-until-parity rule

1. Back up existing saves.
2. Keep old service active.
3. Install EchoSave and pass standalone Laboratory.
4. Add project participants without deleting old models.
5. Build explicit importer/migration from copied old data.
6. Compare old/new snapshots and user flows.
7. Switch one read/write path at a time.
8. Preserve rollback for at least one release/test cycle.
9. Remove old code only after parity and recovery tests pass.

### 25.3 Migration tooling

Project-specific migration tooling must:

- detect source format/version;
- preview target slot/generation;
- operate on copies;
- preserve source backup;
- validate imported payloads;
- produce report and rollback instructions;
- never claim the package understands project data without an adapter.

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| R-001 | Scope expands into cloud, encryption, and whole-world streaming | High | High | Keep providers/deferred scope explicit | Any MVP addition request |
| R-002 | “Atomic” claim exceeds platform capability | Medium | Critical | Capability-advertising backend, generation recovery, precise docs | Backend implementation |
| R-003 | Participant applies partial state then fails | Medium | High | Stage/commit guidance, detailed apply report, rollback-capable participants | Participant design review |
| R-004 | Unknown payload preservation grows stale data forever | Medium | Medium | Bounds, diagnostics, explicit prune plans | Size/slot health thresholds |
| R-005 | JsonUtility limitations surprise users | High | Medium | Clear DTO rules, validator, provider seam, tests | Quick-start/API docs |
| R-006 | File I/O stalls main thread | Medium | High | Detached DTOs, background provider work, profiling | M2 performance tests |
| R-007 | Root duplicates create parallel writers | Medium | Critical | Claim before all storage side effects | Lifecycle tests |
| R-008 | Display names leak into paths | Low | High | Stable IDs and path validator | Setup/runtime validation |
| R-009 | Migration chain removed or untested | Medium | Critical | Fixture pack, contiguous-chain validation, release gate | Every schema change |
| R-010 | Catalog cache becomes mistaken authority | Medium | Medium | Derived/rebuildable design, tests | Catalog implementation |
| R-011 | Autosave request spam | High | Medium | One pending coalesced autosave, bounded queue | Stress test |
| R-012 | Corrupt file overwritten before recovery | Low | Critical | Immutable generations, no in-place repair, explicit plan | Recovery implementation |
| R-013 | Security hash mistaken for anti-cheat | Medium | High | Diagnostics/docs explicitly distinguish integrity/authentication | Security review |
| R-014 | Tests touch real player saves | Low | Critical | Mandatory sandbox/path guards | Test/tool review |
| R-015 | Large save exceeds memory | Medium | High | Size limits, memory estimates, defer streaming | Profiling/limit hit |
| R-016 | Optional package removal loses data | Medium | High | Opaque unknown payload preservation | Round-trip regression |
| R-017 | Existing-project replacement regresses continue flow | Medium | High | Parallel integration and preserve-until-parity | Adoption checkpoint |
| R-018 | Platform lifecycle differs, especially WebGL/mobile | Medium | High | Conditional support claims and device tests | Platform release |
| R-019 | Destructive delete/repair invoked accidentally | Low | High | Two-step plans, preview, trash retention | Tool/API review |
| R-020 | Save root reveals private data in support exports | Medium | High | Redaction defaults and explicit opt-in | Privacy review |

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR? |
|---|---|---|---|---|---:|
| ESV-D-001 | EchoSave owns game-save transport/files/slots, not gameplay schemas or global preferences | Approved | Preserves authority boundaries | Participants/project own data meaning | No |
| ESV-D-002 | One duplicate-safe application-session root | Approved | Prevents parallel writers | Claim must precede storage side effects | No |
| ESV-D-003 | Committed saves are immutable generations | Approved | Recovery and crash safety | More files/retention management | Yes when implemented in detail |
| ESV-D-004 | A small head pointer publishes the current generation | Approved | Avoids in-place payload overwrite | Backend capability/fallback tests required | Yes |
| ESV-D-005 | Slot metadata lives in a manifest separate from payload | Approved | Fast/independent listing | Generation contains multiple files |
| ESV-D-006 | Catalog cache is derived and rebuildable | Approved | Prevents cache corruption becoming data loss | Refresh cost must be profiled |
| ESV-D-007 | Participants are narrow, stable-ID, versioned contributors | Approved | Decouples gameplay systems | Participant docs/tests required |
| ESV-D-008 | Unknown payloads are preserved opaque by default | Approved | Clean optional package removal | Bounds/prune tooling required |
| ESV-D-009 | Loading is two-phase with a convenience one-step path | Approved | Scene flow stays external | Prepared handles/lifecycle required |
| ESV-D-010 | Default serializer uses Unity JsonUtility for plain DTOs | Approved | No third-party dependency | Limitations explicit; provider seam required |
| ESV-D-011 | Package and participant migrations are separate contiguous chains | Approved | Independent evolution | Fixture pack and version discipline |
| ESV-D-012 | No unsupported newer-format guessing or downgrade | Approved | Prevents destructive misread | Newer saves remain untouched |
| ESV-D-013 | Main-thread capture/apply, background detached serialization/I/O | Approved | Unity thread safety and responsiveness | DTO detachment required |
| ESV-D-014 | One mutating operation globally in MVP | Approved | Simpler deterministic safety | Throughput lower but predictable |
| ESV-D-015 | Autosaves coalesce to one pending latest request | Approved | Bounded request pressure | Intermediate requests may be superseded |
| ESV-D-016 | Cancellation stops before publication; after publication starts it is Too Late | Approved | Keeps storage in known state | UI must explain terminal behavior |
| ESV-D-017 | Destructive slot operations use two-step plans/tokens | Approved | Prevents accidental deletion | Slightly more API work |
| ESV-D-018 | Checksums detect corruption but are not authentication | Approved | Honest security boundary | Anti-tamper remains provider/project concern |
| ESV-D-019 | Cloud, encryption, compression, streaming, and merge are deferred providers | Approved | Protects neutral MVP | Later specs/adapters required |
| ESV-D-020 | Foundation implementation remains locked after this approval | Approved | Documentation-first gate | Workshop and consistency review come first |
| ESV-D-021 | Durable persistence, runtime truth, and Unity object lifetime remain separate; Chronicle root authority is package-local and peer persistence uses optional adapters | Approved | SFGSS-ADR-006 prevents Chronicle/First Light/global-root coupling before implementation | ESV-M1-01 proves only package-local lifecycle; future peer persistence requires bridges/adapters |
| ESV-D-022 | Missing-payload default initialization is an additive optional `ISaveDefaultableParticipant.InitializeDefault()` capability; base `ISaveParticipant` remains unchanged and `Apply(null)` is not a default protocol signal | Approved | Keeps existing participant implementations source-compatible and makes default initialization explicit rather than encoding hidden null semantics | `InitializeDefault` policy requires the optional capability during apply preflight; missing capability fails before mutation |
| ESV-D-023 | Slot-catalog discovery is provider-neutral through an additive optional storage enumeration capability; base `ISaveStorageBackend` remains unchanged and catalog core does not reach through local `RootPath`/`System.IO` | Approved | Preserves storage-provider neutrality and source compatibility while enabling authoritative slot discovery | Local backend implements bounded immediate-child discovery; providers lacking the capability report catalog refresh unavailable/unsupported |
| ESV-D-024 | Successful slot creation means one verified immutable generation plus `head.json` last; directory existence alone is not success; all discovered canonical technical slots including degraded entries count toward capacity; creation does not auto-select | Approved | Prevents fake/half-created slots, capacity bypass through degraded entries, display-name/path coupling, and hidden selection side effects | M4-02 adds bounded technical create/capacity/catalog reconciliation while rename/duplicate/delete and production operation admission remain later work |
| ESV-D-025 | The first manual-save checkpoint composes existing proven primitives internally before exposing production `SaveAsync` or generic operation admission | Approved | Keeps M4-03 bounded and proves durable save semantics without forcing lifecycle/service wiring, queue policy, cancellation, autosave, and cross-operation concurrency into one checkpoint | M4-03 targets the explicitly selected healthy slot, refreshes source provenance, captures fresh known participants, preserves valid unknown payloads, publishes only against the expected current generation, reconciles the catalog, and reports partial durable/catalog truth; M4-04 or later owns public admission/Busy/cancellation |
| ESV-D-026 | Public manual save enters through one root-local mutating-operation admission authority; an overlapping manual save returns Busy immediately rather than queueing, cancellation is honored only before durable publication begins, and shutdown closes new admission without abandoning a publication already crossing the commit boundary | Approved | Establishes the production-facing manual-save seam without prematurely implementing autosave coalescing or every later mutating operation | M4-04 expands `IEchoSaveService` with active-slot `SaveAsync`, maps public results to M4-03 durable truth, proves Busy/cancel/Too-Late/shutdown behavior, and leaves generic multi-operation queue policy plus autosave for later checkpoints |
| ESV-D-027 | Autosave is an explicit caller-triggered request source, not a Chronicle-owned gameplay timer; Chronicle coalesces autosaves into at most one pending latest request and reuses the same root-local mutating admission/durable save path as manual save | Approved | Preserves project ownership of when autosave should occur, keeps request spam bounded, and avoids inventing a generic queue before other operation types exist | M4-05 adds `RequestAutosave` submission/coalescing truth and one latest-wins pending autosave slot; manual save remains Busy while occupied, pending autosave never becomes an unbounded queue, and retention remains a later bounded checkpoint |
| ESV-D-028 | Generation retention is post-publication maintenance over verified committed non-current generations; it never participates in head commit, never deletes the current head generation or its immediate recovery predecessor, and retention failure never fabricates rollback of an already committed save | Approved | Preserves durable-truth semantics while bounding disk growth and protecting one immediate recovery generation | M4-06 introduces bounded retention policy/discovery/tree-deletion seams and post-publication cleanup; corrupt/unsupported/unrecognized material remains for later recovery/quarantine authority rather than being silently deleted |
| ESV-D-029 | Recovery planning is read-only, bounded, and deterministic; only fully verified committed generations may become candidates, the preferred fallback is the newest valid candidate by validated technical timestamp with generation ID as tie-break, and every immutable plan binds exact observed source provenance for later stale-plan rejection before any execution | Approved | Prevents damaged current state from being overwritten during diagnosis and makes future recovery execution auditable/revalidatable | M4-07 builds plans only; head publication, catalog mutation, automatic fallback, quarantine, and recovery execution remain later bounded work |
| ESV-D-030 | Explicit recovery execution is one root-local mutating operation that must rebuild/revalidate the M4-07 source snapshot and selected candidate before publishing a head; a stale/mismatched plan or candidate fails before mutation; successful head publication is durable recovery truth and later catalog reconciliation failure is reported separately rather than rolling back the head | Approved | Makes recovery auditable and race-resistant while preserving the same durable-truth model used by save publication | M4-08 republishes only the small head pointer to an already verified immutable committed generation; it does not rewrite generation payloads, auto-select fallback, quarantine evidence, or invent a generic queue |
| ESV-D-031 | Slot rename and duplication are non-destructive admitted slot mutations over the immutable-generation model: rename never changes `SaveSlotId` or physical slot path and commits display-metadata change through a new generation/head update; duplicate never mutates the source, requires capacity, creates a new package-generated slot/generation identity, copies the verified source state without participant callbacks, and revalidates the bound source before publishing the destination | Approved | Preserves immutable history, stable slot identity, and race-resistant copy semantics without smuggling destructive delete/trash or a generic operation queue into the same checkpoint | M4-09 covers ESV-T-019 and ESV-T-020 only; delete-plan/confirm, trash, persistent cache, and automatic recovery remain later bounded work |
| ESV-D-032 | Destructive slot deletion is a two-step plan-bound mutation: prepare-delete is read-only and produces an immutable package/session/source-provenance-bound plan; confirm-delete requires one valid unexpired unused plan, revalidates the bound canonical source before mutation, reuses root-local mutation admission, and commits deletion only when the slot tree has been moved to package-owned recoverable trash. Active-slot clearing and catalog reconciliation occur only after durable removal; bounded trash retention is post-commit maintenance and cannot fabricate rollback | Approved | Prevents accidental/stale/replayed destructive actions while making the safe default deletion recoverable and preserving the same durable-truth discipline used by save/recovery | M4-10 covers ESV-T-021 through ESV-T-023; permanent erase, restore-from-trash public API, quarantine cleanup, persistent cache, and generic queues remain separately bounded work |
| ESV-D-033 | M4 reconciliation preserves the already-approved MVP public runtime contract instead of weakening it: R1 must compose existing proven M3/M4 internals behind a coherent `IEchoSaveService` facade for participant registration, catalog snapshot/refresh, slot creation/selection, two-phase loading, and same-scene convenience load; R2 owns runtime slot-policy configuration; R3 implements package-document migration so CAP-014 remains intact | Approved | The `48454ea` milestone audit proved the internal foundations exist but the consumer-facing composition and two remaining MVP capabilities are incomplete | M4 cannot close and M5 cannot activate until R1/R2/R3 plus registry/document reconciliation are complete; R1 may add public facade DTOs/results and service wiring but may not change save-generation durability, participant contracts, base storage contracts, scene ownership, or M5 tooling | No |

### 27.2 Release-blocking questions

None. Exact filesystem replacement/flush behavior, package dependency versions, performance budgets, and supported-platform matrix are implementation verification tasks, not unresolved authority decisions.

### 27.3 Non-blocking later questions

- Whether binary/stream serializers become first-party providers.
- Whether thumbnails belong in the core MVP or first minor release.
- Whether provider-neutral cloud synchronization contracts belong in EchoSave core or a tiny companion contracts package.
- Whether very large worlds require chunk manifests or a separate world-stream persistence package.
- Whether multi-user saves require participant namespaces and authority tokens.
- Which consoles/platform services receive first adapters.
- How cross-device merge policy should expose project-defined conflicts.

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Evidence |
|---|---|---|---|
| M0 - Specification | Approved authority and contracts | All 30 sections | This document |
| M1 - Skeleton | Installable package, configuration, root claim, provider interfaces | Manifest, asmdefs, docs shell, lifecycle | Clean compile/duplicate tests |
| M2 - Document/storage core | IDs, paths, manifests, payloads, local backend, immutable generations | Commit fault tests | Automated tests |
| M3 - Participants and loading | Registry, capture/apply, prepared loads, unknown preservation | Participant/migration tests | Automated tests |
| M4 - Slots/autosave/recovery | Catalog, slot ops, retention, autosave, recovery | Fault/recovery matrix | Automated tests |
| M5 - Tooling and Laboratory | Setup, validator, browser, simulator, sample | 32 lab scenarios | Repeatability report |
| M6 - First integration | Existing project adapter and parity | Echo Systems Lab or Rescuers2D | Parity/rollback report |
| M7 - Release | Distribution-ready candidate | Docs, fixtures, platform tests, artifact | External install/release gates |

### 28.2 Foundation documentation gate

This approval completes the ninth of ten Foundation package specifications. No EchoSave implementation begins until:

1. The Workshop (`EchoGameStarter`) specification is approved.
2. FW-DOC-11 cross-spec contract matrix is complete.
3. FW-DOC-12 documentation readiness gate passes.
4. Conflicts are reconciled into specs, SFGSS-000, or ADRs.
5. Current Notes explicitly authorizes M1 work.

### 28.3 Completed first implementation checkpoint

**ESV-M1-01 - Installable skeleton and duplicate-safe authority claim**

**Status:** Complete.

Outcome achieved:
- package manifest, asmdefs, documentation shell, configuration, and lifecycle shell;
- provider interfaces/value IDs/results without file writes;
- duplicate rejection before storage/path/callback side effects;
- initial lifecycle/EditMode tests;
- Unity compile/import and focused Chronicle Editor gate all green;
- implementation commit `ecfa922`;
- embedded Package Manager resolution commit `2c70b1d`.

Stop point preserved: M1 writes no real save file.

### 28.4 Completed storage-foundation checkpoint

**ESV-M2-01 - Storage root, path safety, and local backend foundation**

**Status:** Complete.

Outcome achieved:
- configured default local root beneath `Application.persistentDataPath`;
- strict relative storage-key/path containment;
- replaceable `LocalFileSaveStorageBackend`;
- injected sandbox roots for automated tests;
- exact-byte read/write and structured failure primitives;
- duplicate rejection before storage-root/backend side effects;
- implementation commit `e4ef76c`;
- focused Chronicle Editor gate **40 / 40**.

Stop point preserved: storage-provider I/O exists and is path-safe, while save documents, physical immutable generation publication, slots, participants, recovery, and peer bridges remain unimplemented.

### 28.5 Completed document/serializer checkpoint

**ESV-M2-02 - Document contracts and Unity JSON serializer foundation**

**Status:** Complete.

Outcome achieved:
- package-owned Chronicle document identity/version contracts;
- default `UnityJsonSaveSerializer` using `JsonUtility`;
- serializer provider registration/lookup;
- structured serialize/deserialize results and diagnostics;
- deterministic DTO/envelope round trips;
- malformed/null/empty input rejection;
- unsupported package-document version/kind rejection;
- serializer/document layer with zero filesystem I/O;
- implementation commit `6404037`;
- focused Chronicle Editor gate **57 / 57**.

Stop point preserved: Chronicle understands and serializes its own package documents in memory, but no physical generation or head pointer is published.

### 28.6 Completed generation-integrity checkpoint

**ESV-M2-03 - Generation identity, integrity, and commit-document foundation**

**Status:** Complete.

Outcome achieved:
- canonical package-generated `SaveSlotId`;
- unique sortable `SaveGenerationId`;
- package manifest/payload/head commit-document contracts and explicit versions;
- replaceable `IIntegrityProvider` and default `Sha256IntegrityProvider`;
- detached-byte SHA-256 calculation and verification;
- manifest/payload/head identity/version/checksum/inventory agreement proof;
- serializer compatibility for the new package documents;
- implementation commit `ad3b646`;
- focused Chronicle Editor gate **87 / 87**.

Stop point preserved: Chronicle can describe and verify a generation entirely in memory, but M2-03 publishes no generation and mutates no head pointer.

### 28.7 Completed immutable-generation publication checkpoint

**ESV-M2-04 - Immutable generation publication and head-last commit foundation**

**Status:** Complete.

Outcome achieved:
- provider-neutral publication capability seam;
- local same-root candidate-to-final publication;
- local small-current-object move/replace publication with capability-accurate claims;
- verified package transport candidate writes;
- immutable generation publication;
- published-generation revalidation;
- `head.json` publication/update last;
- previous-known-good preservation across injected pre-head failure;
- orphan generation remains non-current when head publication fails;
- implementation commit `01b7ad3`;
- focused Chronicle Editor gate **102 / 102**.

Milestone consequence: **M2 - Document / Storage Core is complete for the approved bounded implementation path.**

### 28.8 Completed participant-registry checkpoint

**ESV-M3-01 - Participant contracts, descriptor validation, and duplicate-safe registry foundation**

**Status:** Complete.

Outcome achieved:
- stable canonical `SaveParticipantId`;
- Required/Optional criticality;
- InitializeDefault/Ignore/Fail missing-payload policy;
- validated descriptors with bounded aliases;
- open-ended public `ISaveParticipant`;
- structured registration results;
- disposable/idempotent ownership leases;
- duplicate-safe deterministic registry;
- canonical/alias collision rejection;
- stale-registration ownership-token protection;
- immutable deterministic registry snapshots;
- no registry durable I/O;
- no registry capture/apply invocation;
- implementation commit `b3b5f9f`;
- focused Chronicle Editor gate **147 / 147**.

Architectural invariant proven: Chronicle has no predefined participant catalog. A future system can register through the same public participant contract without editing Chronicle core.

### 28.9 Completed detached-capture checkpoint

**ESV-M3-02 - Detached participant capture, runtime type routing, and payload-entry construction foundation**

**Status:** Complete.

Outcome achieved:
- trusted live-registration runtime DTO type authority;
- optional runtime-Type serializer capability;
- no CLR/assembly-qualified type metadata in save transport;
- deterministic participant capture;
- default/explicit serializer resolution;
- detached DTO null/type/live-Unity-object validation;
- exact UTF-8 participant payload lengths;
- per-entry SHA-256 checksums;
- package payload/inventory entry construction;
- all-or-nothing capture-batch failure;
- defensive-copy batch results;
- future participant same-pipeline proof;
- zero storage/publication mutation;
- implementation commit `e34d6d7`;
- focused Chronicle Editor gate **171 / 171**.

### 28.10 Completed participant-generation publication checkpoint

**ESV-M3-03 - Participant-backed generation publication and head-last integration foundation**

**Status:** Complete.

Outcome achieved:
- participant-batch publication-boundary validation;
- duplicate/order/schema/serializer/flags validation;
- exact per-entry UTF-8 byte-length and checksum revalidation;
- payload/inventory agreement revalidation;
- participant-bearing payload/manifest document construction;
- shared M2/M3 generation-first/head-last transaction core;
- candidate and published-generation participant-entry revalidation;
- zero-storage mutation for invalid batches;
- previous-known-good head preservation across injected pre-head failures;
- orphan/non-current behavior on head serialization/publication failure;
- first/second participant-bearing generations with prior-generation preservation;
- existing M2 empty/transport path preserved;
- implementation commit `6970127`;
- focused Chronicle Editor gate **197 / 197**.

### 28.11 Completed current-generation read / unknown-payload checkpoint

**ESV-M3-04 - Current-generation read, opaque unknown-payload preservation, and session store foundation**

**Status:** Complete.

Outcome achieved:
- read-only current head/generation resolution;
- complete current immutable payload/manifest validation;
- whole-document and per-entry integrity validation;
- canonical/alias participant recognition;
- package-owned opaque unknown-payload session store;
- exact unknown serialized payload text and transport metadata preservation;
- defensive-copy deterministic snapshots;
- bounded unknown count/aggregate bytes;
- atomic successful store replacement;
- prior-store preservation across failed reads/classification;
- zero storage mutation;
- implementation commit `aa78e07`;
- focused Chronicle Editor gate **218 / 218**.

### 28.12 Completed source-fresh unknown carry-forward checkpoint

**ESV-M3-05 - Opaque unknown-payload carry-forward merge, source-freshness, and collision-safe publication foundation**

**Status:** Complete.

Outcome achieved:
- source slot/generation provenance;
- atomic provenance refresh/preservation/reset;
- stale-source and slot mismatch rejection;
- canonical/alias ownership collision fail-closed behavior;
- exact opaque payload bytes/metadata preservation;
- deterministic fresh-known + unknown merge;
- unknown opacity with no serializer/participant invocation;
- expected-source recheck inside publication;
- immutable merged generation publication and `head.json` last;
- previous known-good head preservation;
- old snapshot staleness after successful head advance;
- implementation commit `af28c96`;
- focused Chronicle Editor gate **243 / 243**.

### 28.13 Completed current-version participant payload preparation checkpoint

**ESV-M3-06 - Current-version participant payload preparation, trusted runtime-Type deserialization, and prepared-participant batch foundation**

**Status:** Complete.

Outcome achieved:
- fully validated current-generation participant snapshot;
- defensive-copy entries with source slot/generation provenance;
- live canonical/alias ownership resolution;
- persisted-ID/current-canonical-owner provenance separation;
- trusted live `DetachedStateType` authority;
- current-schema-only preparation;
- older-schema migration-required result;
- newer-schema unsupported-newer result;
- already-registered runtime-Type serializer resolution;
- unknown-payload serializer bypass;
- deterministic all-or-nothing prepared participant batching;
- zero participant `Capture`/`Apply`;
- implementation commit `050bfa0`;
- focused Chronicle Editor gate **261 / 261**.

### 28.14 Completed participant migration checkpoint

**ESV-M3-07 - Participant migration contracts, duplicate-safe registry, contiguous-chain execution, and migrated payload preparation foundation**

**Status:** Complete.

Outcome achieved:
- stable participant migration-step IDs;
- explicit canonical participant/version-edge migration-step contracts;
- one exact `vN -> vN+1` edge per step;
- duplicate-safe runtime-memory migration registry;
- deterministic registration leases/snapshots;
- exact contiguous old-schema-to-current chain planning;
- bounded in-memory chain execution;
- missing-edge failure before execution;
- per-step output version/serializer/payload validation;
- ordered migration provenance without payload contents;
- persisted aliases resolve current canonical ownership before migration lookup;
- supported older known payloads migrate to current schema, then reuse M3-06 current DTO preparation;
- unknown payloads never enter migration planning;
- source immutable generations remain untouched;
- no participant `Capture` or `Apply`;
- implementation commit `d96936f`;
- focused Chronicle Editor gate **294 / 294**.

### 28.15 Completed prepared-load handle lifecycle checkpoint

**ESV-M3-08 - Prepared-load handle lifecycle and session ownership foundation**

**Status:** Complete.

Outcome achieved:
- public opaque sealed/disposable `PreparedSaveLoad`;
- safe immutable source/lifetime/count metadata;
- exact source slot/generation provenance binding;
- package/session owner token + epoch isolation;
- package-internal prepared DTO ownership;
- defensive package-internal opaque unknown snapshot ownership;
- idempotent disposal;
- deterministic injected-time expiry;
- owner/session invalidate-all;
- cross-owner and stale-token rejection;
- positive live-handle count bound;
- positive aggregate source transport-byte bound;
- deterministic capacity release;
- no participant `Capture` or `Apply`;
- no storage mutation;
- no scene/DDOL authority;
- implementation commit `798d38d`;
- focused Chronicle Editor gate **332 / 332**.

### 28.16 Completed deterministic participant apply checkpoint

**ESV-M3-09 - Deterministic participant apply and missing-payload policy foundation**

**Status:** Complete.

Outcome achieved:
- additive optional public `ISaveDefaultableParticipant.InitializeDefault()`;
- unchanged base `ISaveParticipant` contract;
- complete zero-callback apply preflight;
- deterministic current-registration apply plan;
- prepared owner/schema/runtime-type compatibility checks;
- current registration ownership-token capture and revalidation;
- explicit `InitializeDefault` / `Ignore` / `Fail` missing-payload behavior;
- no `Apply(null)` default protocol;
- prepared `Apply(detachedState)` execution;
- explicit `InitializeDefault()` execution;
- payload-free ordered partial reporting;
- structured participant-returned failure/exception handling;
- pure preflight rejection leaves handle live;
- execution consumes handle and replay rejects;
- source immutable generation remains unchanged;
- no scene/DDOL authority;
- implementation commit `568fa3a`;
- focused Chronicle Editor gate **366 / 366**.

**M3 — Participants and Loading is complete.**

### 28.17 Completed slot catalog and session-selection checkpoint

**ESV-M4-01 - Slot catalog, metadata rebuild, and active-session selection foundation**

**Status:** Complete.

Outcome achieved:
- additive provider-neutral `ISaveStorageDiscoveryBackend`;
- unchanged base `ISaveStorageBackend`;
- default local bounded immediate-child discovery;
- canonical technical `SaveSlotId` filtering and deterministic ordering;
- absent `slots` root as successful empty catalog;
- payload-free `head.json` + current `manifest.json` reconstruction;
- healthy/degraded non-selectable slot health;
- immutable deterministic catalog snapshots;
- prior-snapshot preservation on untrustworthy overall refresh failure;
- session-only explicit active-slot select/no-change/reject/clear;
- stale active-selection reconciliation;
- no automatic selection;
- zero durable active-selection writes;
- zero participant callbacks;
- implementation commit `62e8a54`;
- focused Chronicle Editor gate **403 / 403**.

### 28.18 Completed technical slot-creation checkpoint

**ESV-M4-02 - Technical slot creation, capacity enforcement, initial empty generation, and catalog reconciliation foundation**

**Status:** Complete.

Outcome achieved:
- a created slot is a durable committed record, not merely a directory;
- successful creation publishes one verified empty immutable generation and `head.json` last;
- technical identity remains package-generated canonical `SaveSlotId`;
- display name/project/build fields remain manifest metadata only;
- every discovered canonical technical slot, including degraded entries, counts toward the bounded creation capacity;
- generated-ID collision retry is positive and bounded;
- create semantics reject an existing current head rather than silently becoming an update/save;
- successful durable publication is followed by M4-01 catalog reconciliation;
- creation never auto-selects the new slot;
- publication success followed by catalog-refresh failure reports partial truth and does not fictionalize rollback;
- participant callbacks remain absent;
- implementation commit `d8d5c18`;
- focused Chronicle Editor gate **425 / 425**, preserving the prior **403 / 403** regression floor.

Stop point: Chronicle can create one bounded technical slot as one real initial committed empty generation, enforce capacity without ignoring degraded technical slots, reject technical collisions, reconcile the catalog after publication, and report durable-publication/catalog-refresh outcomes accurately without auto-selecting.

### 28.19 ESV-M4-03 manual-save transaction composition closeout

**Status:** Complete.

**Planning baseline:** `a3eba25`.
**Planning/activation commit:** `2c325e9`.
**Implementation commit:** `c8ea742`.
**Final focused Chronicle Editor evidence:** `439 / 439`, `0` failed.
**Prior regression floor:** `425 / 425` preserved.
**Net new focused M4-03 tests:** `14`.

ESV-M4-03 completed the bounded M4 slice:

- resolve one explicitly selected active slot from the M4-01 session/catalog authority;
- require that slot to be present and healthy in the current trusted catalog snapshot before capture begins;
- read and fully validate the current generation for that exact slot;
- refresh the session unknown-payload snapshot with exact source slot/generation provenance;
- capture all currently registered known participants through the existing deterministic M3-02 capture coordinator;
- merge fresh known entries with preserved opaque unknown entries through the existing ownership/collision rules;
- publish one new participant-backed immutable generation only if the previously validated source generation is still the current head;
- preserve `head.json`-last commit semantics and stale-source rejection already proven by M3-05;
- refresh the slot catalog after successful head publication;
- return truthful terminal state when durable publication succeeds but catalog reconciliation fails;
- preserve the slot display name from current catalog metadata so ordinary save does not secretly become rename.

### ESV-D-025 — compose manual-save truth before public operation admission

> The first manual-save checkpoint is an internal deterministic transaction composition checkpoint. It proves the complete active-slot save transaction using already-proven Chronicle primitives, but it does not yet expose production `SaveAsync`, generic operation admission, queueing, Busy policy, autosave coalescing, or cancellation.

Consequences:

- M4-03 can prove save correctness without simultaneously turning the lifecycle-only `IEchoSaveService` into the final production facade;
- a failed capture/merge/pre-publication step cannot advance the head;
- expected-current-generation publication rejects a source that changed after provenance was read;
- unknown payloads remain opaque and source-bound;
- a normal save preserves current display-name metadata rather than mutating slot identity/presentation;
- M4-04 or another separately authorized checkpoint can add public async admission/Busy/cancellation on top of a proven manual-save transaction.

**Explicitly still deferred:** persistent `catalog.cache.json`; rename/duplicate/delete and trash/quarantine; full slot-policy/configuration assets; public `SaveAsync`; generic production operation admission and concurrent mutation ownership; permission-provider facade wiring; queue/coalescing/cancellation policy; autosave; retention; recovery; document migration; scene travel; peer bridges; service-locator behavior; Chronicle-owned/project-wide DDOL.

The final ESV-M4-03 focused Chronicle Editor gate is **439 / 439**, with the prior **425 / 425** regression floor preserved. No follow-on M4 checkpoint is activated by this closeout.
### 28.20 ESV-M4-04 public manual-save admission closeout

**Status:** Complete.

**Planning baseline:** `3a84187`.

**Planning/activation commit:** `91dcb62`.

**Implementation commit:** `2732aaa`.

**Lifecycle-status hotfix commit:** `09ae8f1`.

**Final effective runtime baseline:** `09ae8f1`.

**Focused Chronicle Editor evidence:** **456 / 456**, `0` failed.

**Prior focused regression floor:** **439 / 439**.

**Net new focused tests:** **17**.

ESV-M4-04 completed the first production-facing active-slot manual-save seam over the proven M4-03 transaction:

- public `SaveRequest` / `SaveOperationResult`;
- additive `IEchoSaveService.SaveAsync(...)`;
- one root-local mutating-operation admission authority;
- immediate Busy rejection for overlapping manual saves with no hidden queue;
- bounded pre-publication cancellation;
- Too-Late cancellation truth after durable publication begins;
- shutdown closure of new admission without abandoning a commit already crossing the durable boundary;
- faithful M4-03 generation/head/catalog result mapping;
- no caller-controlled save path or display-name mutation.

The first focused M4-04 run exposed one lifecycle-ordering defect: a service that had not reached Ready could report `AdmissionClosed` because the root-local admission coordinator intentionally starts closed before initialization. The bounded runtime hotfix at `09ae8f1` restored the intended distinction:

- AuthorityClaimed / Initializing / Blocked → `ServiceNotReady`;
- ShuttingDown / Shutdown → `AdmissionClosed`;
- Ready → consult actual operation admission for Closed / Busy semantics.

The final rerun passed **456 / 456**.

### ESV-D-026 — public manual save uses one bounded root-local admission authority

> Public manual save enters through one root-local mutating-operation admission authority. A second manual save received while that authority is occupied returns Busy immediately rather than queueing. Cancellation is honored only before the durable publication boundary. Shutdown closes new admission and allows an already-committing save to settle.

ESV-D-026 remains the durable M4-04 authority.

**Explicitly still deferred:** autosave/coalescing; generic queued multi-operation policy and configured queue capacity/overflow; permission-provider facade wiring; operation progress/events beyond the bounded public result plumbing already required by M4-04; retention; recovery; rename/duplicate/delete and trash/quarantine; persistent `catalog.cache.json`; full slot-policy/configuration expansion; document migration; scene travel; peer bridges; service-locator behavior; Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is activated by this closeout.

### 28.21 ESV-M4-05 autosave request/coalescing closeout

**Status:** Complete.

**Planning baseline:** `9a2ad29`.

**Planning/activation commit:** `8504ed4`.

**Implementation commit:** `9917f1b`.

**Final effective runtime baseline:** `9917f1b`.

**Focused Chronicle Editor evidence:** **473 / 473**, `0` failed.

**Prior focused regression floor:** **456 / 456**.

**Net new focused tests:** **17**.

ESV-M4-05 completed the first bounded public autosave-request/coalescing path:

- public caller-triggered `AutosaveRequest`;
- additive `IEchoSaveService.RequestAutosave(...)`;
- bounded `AutosaveSubmissionResult` and observable ticket state;
- exactly zero-or-one pending latest autosave request;
- latest-wins coalescing/supersession while admission is occupied;
- reuse of the M4-04 root-local mutating-operation admission authority;
- reuse of the M4-03/M4-04 durable active-slot save transaction;
- preservation of M4-04 manual-save Busy behavior;
- at-most-once pending drain after admission becomes available;
- deterministic rejection/discard during shutdown admission closure;
- no Chronicle-owned gameplay timer or generic autosave queue.

The final implementation commit includes one necessary regression-test maintenance change. The M4-04 public-service surface test had correctly required `RequestAutosave` to be absent while autosave was deferred. Once ESV-M4-05 explicitly authorized that public API, the absence assertion became stale. The test was updated to continue proving bounded `SaveAsync(SaveRequest)` while also proving bounded `RequestAutosave(AutosaveRequest) -> AutosaveSubmissionResult`.

Implementation-helper history:
- the first helper failed safely because the new `Runtime/Autosave` parent directory had not been created;
- the second helper applied all payload files but incorrectly counted `git status --porcelain` rows rather than actual files;
- the corrected helper counted exact tracked/new files and verified rollback state;
- final implementation/test scope committed as **22 files**, including the regression-test maintenance.

### ESV-D-027 — autosave is caller-triggered and latest-wins bounded

> Chronicle does not decide when gameplay should autosave. A project/system explicitly submits an autosave request. Chronicle bounds request pressure by retaining at most one pending latest autosave while another mutating operation owns admission. The pending request is not a general-purpose queue.

ESV-D-027 remains the durable M4-05 authority.

**Explicitly still deferred:** generation retention cleanup and `SaveRetentionPolicy`; generic queued multi-operation scheduling/capacity/overflow; permission-provider production facade wiring; recovery; rename/duplicate/delete and trash/quarantine; persistent `catalog.cache.json`; full slot-policy/configuration expansion; automatic timer/checkpoint autosave triggers; document migration; scene travel; peer bridges; service-locator behavior; Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is activated by this closeout.

### 28.22 ESV-M4-06 generation-retention closeout

**Status:** Complete.

**Planning baseline:** `3cdad0f`.

**Planning/activation commit:** `3d8e0b8`.

**Implementation commit:** `e714a90`.

**Final effective runtime baseline:** `e714a90`.

**Focused Chronicle Editor evidence:** **497 / 497**, `0` failed.

**Prior focused regression floor:** **473 / 473**.

**Net new focused tests:** **24**.

**Committed implementation/test scope:** **33 files**, `2136` insertions, `12` deletions.

ESV-M4-06 completes the first bounded ordinary committed-generation retention path:

- project-owned bounded `SaveRetentionPolicy`;
- minimum safe total retained-generation bound of **2**;
- provider-neutral bounded generation-directory discovery;
- additive optional `ISaveStorageTreeDeletionBackend`;
- unchanged base `ISaveStorageBackend`;
- fail-closed canonical/readable/committed candidate classification;
- protection of `head.currentGenerationId`;
- protection of valid `head.previousGenerationId`;
- deterministic oldest-first excess-history deletion with generation-ID tie-break;
- retention only after successful generation publication, final verification, and `head.json` publication;
- shared retention path for public manual save and caller-triggered autosave;
- public `SaveOperationResult.RetentionResult` maintenance truth;
- retention maintenance failure never fabricates rollback of an already committed save.

### Integration-test correction

The first focused M4-06 run discovered **497** tests with **495 passed / 2 failed**. Both failures were in the newly added manual-retention integration proof:

- `RetentionRunsOnlyAfterSuccessfulHeadPublication`;
- `RetentionFailureNeverFabricatesSaveRollback`.

The test fixture created an empty slot but registered no participant, so the transaction failed during participant capture before reaching publication/retention. The third integration test, `FailedPublicationNeverInvokesRetention`, also stopped early and therefore passed for the wrong pre-publication reason.

The test-only correction registered one ordinary participant in all three integration cases. Runtime implementation, public API, architecture, authority, and test discovery count did not change.

Final rerun: **497 / 497 passed, 0 failed**.

### ESV-D-028 — retention remains post-commit maintenance

> Generation retention may remove only verified committed non-current generations after the new generation and `head.json` are already authoritative. It protects current and immediate recovery history, fails closed when history cannot be trusted, and never rewrites committed-save truth merely because cleanup maintenance fails.

ESV-D-028 remains the durable M4-06 authority.

**Explicitly still deferred:** recovery-plan generation/execution; fallback selection; quarantine movement; slot rename/duplicate/delete/trash; trash-history retention; persistent `catalog.cache.json`; generic queued multi-operation scheduling/capacity/overflow; automatic timer/checkpoint autosave triggers; permission-provider production facade wiring; full `EchoSaveConfiguration`/Setup expansion; document migration; scene travel; peer bridges; service locator; Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is activated by this closeout.

### 28.23 ESV-M4-07 recovery-candidate planning closeout

**Status:** Complete.

**Planning baseline:** `9695450`.

**Planning/activation commit:** `7b00503`.

**Implementation commit:** `9f68555`.

**Final effective runtime baseline:** `9f68555`.

**Focused Chronicle Editor evidence:** **524 / 524**, `0` failed.

**Prior focused regression floor:** **497 / 497**.

**Net new focused tests:** **27**.

**Committed implementation/test scope:** **22 files**, `2912` insertions, `6` deletions.

ESV-M4-07 completes the first read-only Chronicle recovery-planning path:

- public `BuildRecoveryPlanAsync(SaveSlotId)`;
- explicit healthy, missing, unreadable/invalid, missing-current, and invalid-current source-state truth;
- bounded provider-neutral generation discovery;
- full manifest/payload/package-version/slot-generation/integrity/committed-state candidate verification;
- preservation and exclusion of noncanonical, malformed, unsupported, incomplete, mismatched, uncommitted, or corrupt evidence;
- deterministic newest-valid candidate ordering with generation-ID tie-break;
- preferred candidate only when recovery is actually required;
- immutable payload-free `SaveRecoveryPlan` and candidate summaries;
- deterministic technical source-provenance fingerprint for later stale-plan execution rejection;
- zero durable mutation during planning;
- no participant capture/apply/default or migration side effects;
- no mutating-operation admission lease merely to inspect recovery state.

### Test-fixture corrections

The first M4-07 compile exposed one new test-support reference to nonexistent `SaveDocumentVersions.HeadMajor`. Chronicle's actual head document major-version authority is `SaveDocumentVersions.HeadPointerMajor`. The test fixture was corrected only; runtime/API/architecture/authority/test intent did not change.

The first focused test run then discovered **524** tests with **522 passed / 2 failed**:

- `UnsupportedGenerationIsPreservedAndExcluded`;
- `UnsupportedHeadStillAllowsVerifiedCandidatePlan`.

Both failures occurred before the planner ran because the production `UnityJsonSaveSerializer` correctly refuses to serialize intentionally unsupported package-document versions. The test fixture was corrected so supported fixtures continue using Chronicle's production serializer, while the two intentionally future-version fixtures use Unity `JsonUtility` directly to author unsupported JSON evidence for the runtime to inspect and preserve/exclude.

Runtime implementation, public API, architecture, ESV-D-029 authority, recovery behavior, test intent, and NUnit discovery count remained unchanged.

Final rerun: **524 / 524 passed, 0 failed**.

### ESV-D-029 — recovery planning remains read-only and provenance-bound

> Chronicle recovery planning may inspect damaged head/current-generation state and retained generations, but it performs zero durable mutation. Only fully verified committed generations may become candidates. Candidate ordering is deterministic newest-valid first. The immutable plan records exact observed source provenance so a later execution checkpoint can reject stale evidence before publication.

ESV-D-029 remains the durable M4-07 authority.

**Explicitly still deferred:** `ExecuteRecoveryAsync`; head rewrite/publication to a selected candidate; catalog reconciliation after recovery; automatic/configured fallback execution; recovery mutation admission/Busy/cancellation; stale-plan execution rejection beyond the provenance already captured; quarantine movement; incomplete-generation cleanup; rename/duplicate/delete/trash; persistent `catalog.cache.json`; generic operation queues/capacity/overflow; automatic timer/checkpoint autosave triggers; permission-provider production facade wiring; full recovery/configuration/Setup expansion; document migration; scene travel; peer bridges; service locator; Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is activated by this closeout.

### 28.24 ESV-M4-08 explicit recovery-execution activation

**Status:** Complete.

**Planning baseline:** `0396adb`.

**Planning/activation commit:** `c324aa4`.

**Implementation commit:** `1985fb0`.

**Final effective runtime baseline:** `1985fb0`.

**Carried focused regression floor:** **524 / 524**.

**Final focused Chronicle Editor evidence:** **540 / 540**, `0` failed.

**Net new focused tests:** **16**.

**Committed implementation/test scope:** **18 files**, `1846` insertions, `10` deletions.

ESV-M4-08 completes the first explicit durable recovery mutation over the M4-07 planning seam.

### ESV-D-030 — recovery execution must revalidate before head mutation

> Explicit recovery execution is one root-local mutating operation. Chronicle must first prove that the supplied immutable plan still describes the exact current source state and that the selected candidate is still a fully verified candidate in a freshly rebuilt plan. Any stale plan, mismatched slot, or no-longer-valid candidate fails before head mutation. When head publication succeeds, recovery is durably committed even if later catalog reconciliation fails.

Completed M4-08 behavior:
- additive public `ExecuteRecoveryAsync(SaveRecoveryPlan, SaveRecoveryCandidate)` surface;
- service Ready preflight;
- root-local mutating-operation admission reuse;
- immediate Busy rejection when another mutation already owns admission;
- no hidden recovery queue;
- explicit caller-selected candidate only;
- supplied plan/candidate validation;
- fresh M4-07 recovery-plan rebuild after admission;
- exact source-provenance fingerprint comparison;
- stale-plan rejection before any storage mutation;
- selected-candidate revalidation against the freshly rebuilt plan;
- candidate generation remains immutable and is never copied, rewritten, migrated, or recaptured;
- head publication only after plan/candidate revalidation succeeds;
- `head.currentGenerationId` becomes the explicitly selected verified candidate;
- recovery-created `head.previousGenerationId` is empty rather than preserving an unverified broken source head/current generation;
- provider-neutral small-current-object publication through existing publication capability;
- post-publication catalog refresh/reconciliation;
- truthful result separation between uncommitted failure, committed-but-unreconciled partial success, and fully reconciled success;
- recovery completion never applies participants or changes active-slot selection.

Primary registry proofs:
- ESV-T-078 — explicit recovery execution publishes the selected verified generation through a new head and reconciles the catalog;
- ESV-T-079 — stale recovery plan rejects before mutation.

Focused completion additionally proves Busy admission, invalid candidate membership, selected candidate becoming invalid after planning, head publication failure, catalog-reconciliation failure after successful head publication, active-slot nonmutation, no participant callbacks, and selected-generation byte immutability.

### M4-08 test maintenance

The first M4-08 compile exposed two new service-test references to nonexistent `FakeManualSaveTransactionExecutor.CallCount`. The established test double exposes `Calls`. The references were corrected test-only.

Runtime implementation, public API, architecture, ESV-D-030 authority, recovery behavior, test intent, and NUnit discovery shape remained unchanged.

Final rerun: **540 / 540 passed, 0 failed**.

Still deferred:
- automatic/configured fallback execution;
- recovery-on-load without explicit caller action;
- quarantine movement;
- incomplete/corrupt generation cleanup;
- rename / duplicate / delete / trash;
- persistent `catalog.cache.json`;
- generic queued multi-operation scheduling/capacity/overflow;
- a new caller-cancellation overload for recovery execution;
- automatic autosave timers/gameplay triggers;
- permission-provider production facade wiring;
- full `SaveRecoveryPolicy`, `EchoSaveConfiguration`, or Setup authoring expansion;
- document or participant migration during recovery;
- scene travel, peer bridges, service locator, or Chronicle-owned/project-wide DDOL.

No follow-on M4 checkpoint is activated by this closeout.


### 28.25 ESV-M4-09 slot rename and full-state duplication closeout

**Status:** Complete.

**Planning baseline:** `07bbd2b`.

**Planning/activation commit:** `7d2d987`.

**Implementation commit:** `459023f`.

**Final focused gate:** **562 / 562 passed, 0 failed**.

**Prior focused regression floor:** **540 / 540**.

**Net new focused tests:** **22**.

**Carried focused regression floor:** **540 / 540**.

**Authority decision:** ESV-D-031.

ESV-M4-09 owns the remaining non-destructive public slot mutations from CAP-017 before Chronicle enters destructive delete/trash work.

#### Rename boundary

Rename:
- targets one explicit canonical existing slot;
- requires the slot/current generation to be healthy and fully validated;
- changes display metadata only from the consumer's point of view;
- never changes `SaveSlotId`;
- never changes the physical slot directory/path;
- never rewrites an already committed manifest or payload in place;
- creates a new immutable generation whose payload bytes and participant inventory remain equivalent to the validated source while the display metadata changes;
- publishes `head.json` last using expected-current-generation stale-source protection;
- applies existing generation retention only after durable publication;
- refreshes/reconciles the catalog afterward;
- preserves active-slot identity if the renamed slot is active;
- invokes no participant capture/apply/default/migration callbacks.

#### Duplicate boundary

Duplicate:
- targets one explicit canonical existing source slot;
- requires the source current generation to be healthy and fully verified;
- refreshes catalog/capacity truth before durable destination mutation;
- counts all canonical technical slots, including degraded entries, toward the M4-02 capacity bound;
- generates a new canonical `SaveSlotId` with bounded collision retry;
- generates a new `SaveGenerationId`;
- copies the source's current fully verified payload/state into the new slot without participant capture;
- rewrites only destination package identity/manifest metadata required for the new slot/generation;
- preserves the source slot and source generation byte-for-byte;
- revalidates the bound source current generation before destination publication;
- publishes the new destination generation and then destination `head.json` last;
- reconciles the catalog after successful publication;
- does not auto-select the new duplicate slot;
- does not modify the session unknown-payload store merely because a duplicate was created.

#### Public/admission boundary

M4-09 may add bounded public rename/duplicate request/result surfaces to `IEchoSaveService`.

Both operations:
- reuse the existing root-local mutating-operation admission authority;
- return Busy immediately when another mutation owns admission;
- create no rename queue and no duplicate queue;
- reject pre-Ready and post-shutdown states consistently with existing public mutating operations;
- do not introduce a generic operation scheduler;
- do not add a new caller cancellation overload in this checkpoint.

#### Durable-truth boundary

For rename, successful new-head publication is durable rename truth. A later retention or catalog-reconciliation failure is maintenance/derived-state truth and must not fabricate rollback.

For duplicate, successful destination-head publication is durable duplicate truth. A later catalog-reconciliation failure must report that the new slot is durably published but catalog reconciliation is incomplete.

No operation may report success merely because a directory or candidate generation exists.

#### Registry proofs

M4-09 maps directly to:
- ESV-T-019 — Rename slot: display metadata changes while slot ID/path remain stable.
- ESV-T-020 — Duplicate slot: a new slot ID receives equivalent fully verified state.

Focused tests must additionally prove:
- rename rejects invalid/missing/degraded source before mutation;
- rename stale-source race rejects before head advancement;
- rename leaves payload bytes unchanged;
- repeated rename obeys existing retention bounds;
- duplicate capacity failure performs no mutation;
- duplicate source becoming stale rejects before destination publication;
- duplicate creates new slot/generation identities;
- duplicate source bytes remain unchanged;
- duplicate destination payload is state-equivalent to the validated source;
- duplicate does not auto-select;
- Busy/AdmissionClosed/ServiceNotReady truth remains consistent;
- post-publication catalog failure never fabricates rollback;
- no participant callbacks occur.

**Explicitly still deferred:** prepare-delete/confirm-delete; trash and trash retention; quarantine/incomplete-generation cleanup; persistent `catalog.cache.json`; automatic/configured recovery fallback; recovery-on-load; generic queued multi-operation scheduling/capacity/overflow; recovery cancellation overload; automatic timer/checkpoint autosave triggers; permission-provider production wiring; full `EchoSaveConfiguration`/Setup expansion; scene travel; peer bridges; service locator; Chronicle-owned/project-wide DDOL.

**M4-09 completion evidence:** implementation commit `459023f`; focused Chronicle Editor **562 / 562 passed, 0 failed**; prior **540 / 540** floor preserved; **22** net new focused tests; committed implementation/test scope **26 files**, `3100` insertions, `8` deletions; ESV-T-019 and ESV-T-020 complete; no follow-on M4 checkpoint activated.


### 28.26 ESV-M4-10 destructive slot deletion planning, confirmed trash, and bounded trash retention closeout

**Status:** Complete.

**Clean planning baseline:** `4d2f2ac`.

**Carried focused regression floor:** **562 / 562**.

**Planning/activation commit:** `2244e3c`.

**Implementation commit:** `01e4cdd`.

**Final focused gate:** **587 / 587 passed, 0 failed**.

**Net new focused tests:** **25**.

**Implementation/test scope:** **28 files**, `2863` insertions, `6` deletions.

**Authority decision:** ESV-D-032.

ESV-M4-10 completes the remaining bounded destructive CAP-017 runtime slice authorized by ESV-D-032. M4 itself remains pending a dedicated milestone reconciliation.

#### Prepare-delete boundary

`PrepareDeleteSlotAsync(SaveSlotId)` is read-only.

It:
- targets one explicit canonical existing slot;
- requires no mutation-admission lease merely to inspect/build a plan;
- performs zero durable mutation;
- returns one immutable `SaveDeletionPlan`;
- binds the plan to package/session authority;
- binds the exact source slot/head/current-generation provenance observed during preparation;
- records bounded issue/expiry truth using injected package clock authority;
- contains a one-use confirmation identity/token;
- exposes enough technical/display metadata for a presenter to ask for confirmation without opening participant payloads;
- does not invoke participant capture/apply/default/migration callbacks.

Preparing a deletion plan never deletes, trashes, renames, rewrites, or clears the active slot.

#### Confirm-delete boundary

`ConfirmDeleteSlotAsync(SaveDeletionPlan)` is one admitted destructive mutation.

It:
- requires Chronicle Ready state;
- reuses the existing root-local mutating-operation admission authority;
- returns Busy immediately if another mutation owns admission;
- creates no delete queue;
- rejects null, foreign-session, expired, already-consumed, malformed, or stale plans before mutation;
- freshly revalidates the exact bound canonical slot/head/current-generation provenance before destructive storage mutation;
- performs no participant callbacks;
- must never accept "slot ID only" as destructive confirmation.

#### Durable trash boundary

The M4-10 safe default deletion is recoverable trash rather than immediate permanent erase.

Confirmed deletion:
- moves the complete canonical slot tree out of `slots/<SaveSlotId>/`;
- publishes it under package-owned bounded `trash/` storage with a package-generated deletion identity;
- must not leave the original canonical slot simultaneously discoverable as a live slot after successful durable trash publication;
- treats successful removal from canonical `slots/` into recoverable trash as durable delete truth;
- clears session active-slot selection only if that exact slot was active and only after durable removal succeeds;
- refreshes/reconciles the live catalog after durable removal;
- reports committed-but-catalog-unreconciled truth if the catalog refresh later fails;
- never fabricates rollback after durable trash publication.

The base `ISaveStorageBackend` contract remains unchanged. Provider-neutral destructive move capability, if required, is additive/optional in the same style as prior discovery/tree-deletion capabilities.

#### Trash retention boundary

Trash retention is bounded post-commit maintenance.

M4-10 may:
- apply one deterministic project/package default trash-history bound sufficient for runtime proof;
- discover only canonical package-owned trash records;
- remove excess oldest eligible trash records only after the newly trashed slot is durably outside the live slot catalog;
- fail closed on ambiguous/untrusted trash evidence;
- report maintenance failure without pretending the live slot was restored.

Full project-owned retention/configuration authoring remains M5 tooling/configuration work.

#### Active-slot and capacity truth

After successful confirmed deletion:
- a deleted active slot becomes unselected;
- deleting a non-active slot preserves the current active selection;
- the removed live slot no longer counts toward canonical live-slot capacity after catalog reconciliation;
- trash records do not count as live canonical slots.

#### Registry proofs

M4-10 maps directly to:
- ESV-T-021 — delete without plan: no mutation;
- ESV-T-022 — expired delete plan: rejected;
- ESV-T-023 — confirm delete: recoverable trash/delete policy applied.

Focused tests additionally prove:
- prepare-delete performs zero durable mutation;
- plan/session/source provenance is immutable and bounded;
- stale source after preparation rejects before destructive mutation;
- replayed/consumed plans reject;
- Busy/ServiceNotReady/AdmissionClosed truth matches other mutations;
- successful delete removes the live slot and publishes recoverable trash;
- active-slot clearing occurs only after durable delete truth;
- non-active deletion preserves active selection;
- catalog reconciliation failure never fabricates rollback;
- trash retention is bounded and post-commit;
- participant callbacks never occur;
- base `ISaveStorageBackend` remains unchanged.

**Explicitly still deferred:** permanent erase API; public restore-from-trash API; quarantine/incomplete-generation cleanup; persistent `catalog.cache.json`; automatic/configured recovery fallback; recovery-on-load; generic queued multi-operation scheduling/capacity/overflow; recovery cancellation overload; automatic timer/checkpoint autosave triggers; permission-provider production wiring; full `EchoSaveConfiguration`/Setup authoring; Editor browser/simulator/Laboratory work; scene travel; peer bridges; service locator; Chronicle-owned/project-wide DDOL.

**M4-10 completion evidence:** implementation commit `01e4cdd`; focused Chronicle Editor **587 / 587 passed, 0 failed**; prior **562 / 562** floor preserved; **25** net new focused tests; committed implementation/test scope **28 files**, `2863` insertions, `6` deletions; ESV-T-021 through ESV-T-023 complete.

No M5 tooling implementation is authorized by this closeout. M4 milestone closeout requires the dedicated reconciliation next.


### 28.27 ESV-M4-R1 public runtime composition and consumer facade reconciliation activation

**Status:** Complete.

**Milestone:** M4 — Slots / Autosave / Recovery reconciliation.

**Clean planning baseline:** `48454ea`.

**Carried focused regression floor:** **587 / 587**.

**Authority decision:** ESV-D-033.

#### Audit trigger

The dedicated M4 milestone reconciliation found that M3/M4 built substantial internal runtime machinery but did not finish the public consumer facade promised by Sections 7 and 10.

R1 addresses the composition gaps only.

#### R1 owns

1. Public participant registration through `IEchoSaveService`.
2. Public immutable catalog snapshot access.
3. Public explicit catalog refresh.
4. Public slot creation over the proven M4-02 technical creation coordinator.
5. Public active-slot selection over the proven M4-01 session catalog authority.
6. Public two-phase load preparation over the proven M3 read/validate/participant-preparation/migration/prepared-handle foundations.
7. Public prepared-handle apply over the proven M3-09 apply planner/executor.
8. Public same-scene convenience load as prepare-then-apply composition.
9. Public consumer DTO/result/status types needed to avoid exposing internal `Technical*` implementation names.
10. Root-local lifecycle/admission truth for the new public operations.
11. Focused public-surface and composition tests proving the facade uses the existing authorities rather than duplicating them.

#### Public surface target

R1 targets the following consumer-facing shape:

```text
SaveParticipantRegistrationResult RegisterParticipant(ISaveParticipant participant)

SaveSlotCatalogSnapshot GetCatalogSnapshot()
Awaitable<SaveSlotCatalogRefreshResult> RefreshCatalogAsync()

Awaitable<SaveSlotCreateResult> CreateSlotAsync(SaveSlotCreateRequest request)
SaveActiveSlotSelectionResult SelectSlot(SaveSlotId slotId)

Awaitable<PreparedLoadCreationResult> PrepareLoadAsync(SaveLoadRequest request)
Awaitable<SavePreparedLoadApplyResult> ApplyPreparedLoadAsync(PreparedSaveLoad handle)
Awaitable<SaveLoadResult> LoadAndApplyAsync(SaveLoadRequest request)
```

Existing M4 public save/autosave/recovery/rename/duplicate/delete methods remain source-compatible.

#### Load semantics locked for R1

- R1 loads the explicit slot's current canonical generation.
- R1 does not silently execute recovery fallback.
- Callers use `BuildRecoveryPlanAsync` / `ExecuteRecoveryAsync` when recovery is required.
- Prepare performs no participant mutation.
- Apply performs complete preflight before the first participant mutation.
- Participant missing-payload policy remains descriptor-owned.
- `ApplyPreparedLoadAsync` therefore does not invent a caller-owned `ApplyLoadOptions` policy override.
- Convenience load is same-scene only and performs no scene travel.
- Failed convenience composition must not leak an unbounded prepared handle.
- Source save generations remain immutable.

#### Catalog/create/select semantics locked for R1

- `GetCatalogSnapshot()` is memory-only and performs no storage I/O.
- `RefreshCatalogAsync()` reuses the proven payload-free catalog scan.
- R1 may serialize refresh/prepare/apply against the existing root-local operation authority, but it does not create a generic queue.
- `CreateSlotAsync` adapts the proven M4-02 technical creation coordinator; it does not rewrite slot publication.
- The current bounded technical capacity remains **64** during R1.
- R2, not R1, replaces that hard runtime default with project-owned slot-policy configuration.
- Create does not auto-select.
- `SelectSlot` is session-only and never writes durable storage.
- Degraded/unselectable entries remain non-selectable.

#### Participant registration semantics locked for R1

- The existing open-ended `SaveParticipantRegistry` remains the single runtime registry authority.
- R1 exposes it through `IEchoSaveService`; it does not add a central participant catalog.
- Registration stays main-thread and memory-only.
- Registration never captures, applies, or writes storage merely because a participant joined.
- Disposable registration ownership and duplicate/alias collision truth remain unchanged.

#### Explicitly out of R1

- `EchoSaveConfiguration` schema expansion;
- configurable slot policies/capacity;
- package-document migration;
- registry-wide test-status cleanup;
- persistent `catalog.cache.json`;
- automatic/configured recovery fallback or recovery-on-load;
- automatic autosave timers;
- generic operation queues/capacity/overflow;
- permission-provider production wiring;
- permanent erase or public restore-from-trash;
- quarantine/incomplete cleanup;
- redacted support export;
- broad event-surface expansion;
- M5 Setup/Validator/Browser/Simulator/Laboratory;
- scene travel;
- peer-package bridges;
- service-locator behavior;
- Chronicle-owned/project-wide DDOL.

#### R1 focused proof requirements

At minimum, focused tests must prove:

- public interface exposes the authorized facade and no unrelated new surface;
- registration success, duplicate-ID rejection, alias collision, disposal/unregister ownership;
- catalog snapshot access is memory-only;
- public refresh faithfully returns M4-01 catalog truth;
- public create faithfully maps M4-02 durable publication/catalog reconciliation and does not auto-select;
- public selection is session-only and rejects missing/degraded slots;
- prepare load returns a bounded `PreparedSaveLoad` without participant mutation;
- prepare load preserves source bytes and does not auto-execute recovery;
- apply rejects invalid/disposed/expired/foreign/stale handles before mutation;
- apply retains M3-09 all-preflight-before-mutation semantics;
- same-scene convenience load successfully prepares and applies;
- convenience-load preparation failure performs no apply;
- convenience-load apply failure reports truthful partial apply state and does not fabricate rollback;
- load/apply invokes no scene travel or DDOL behavior;
- existing save/autosave/recovery/slot-operation regressions remain green;
- focused Chronicle Editor floor is not allowed to drop below **587 / 587**.

#### R1 closeout rule

R1 completion does **not** complete M4.

After R1:
1. R2 must implement the approved CAP-002 runtime slot-policy/configuration model.
2. R3 must implement package-document migration while preserving CAP-014.
3. The final reconciliation pass must map the 100-case registry and documentation to retained evidence.
4. Only then may M4 be declared complete and M5 activated.



#### R1 completion evidence

**Planning/activation commit:** `bdb0c00`
**Implementation commit:** `ab18361`
**Final focused Chronicle Editor gate:** **618 / 618 passed, 0 failed**
**Prior floor:** **587 / 587**
**Net new focused tests:** **31**
**Implementation/test scope:** **29 files**, `2995` insertions, `18` deletions

R1 completed without a post-apply runtime/test hotfix.

R1 completion does not complete M4. R2 slot-policy runtime configuration is the next gate but is not activated by this closeout. R3 package-document migration and final registry/document reconciliation remain required. M5 remains locked.
## 29. New-Conversation Handoff

```text
We are continuing development of The Sperk’s Forge - EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the authority for suite-wide boundaries and architecture.
Treat The Chronicle (EchoSave) Package Specification v1.36.0 as the authority
for durable save files, slots, generations, manifests, participants, loading,
migrations, recovery, tooling, the Save Laboratory, and release gates.

Current package: EchoSave
Current specification version: 1.36.0
Completed checkpoints: ESV-M1-01; ESV-M2-01; ESV-M2-02; ESV-M2-03; ESV-M2-04; ESV-M3-01; ESV-M3-02; ESV-M3-03; ESV-M3-04; ESV-M3-05; ESV-M3-06; ESV-M3-07; ESV-M3-08; ESV-M3-09; ESV-M4-01; ESV-M4-02; ESV-M4-03; ESV-M4-04; ESV-M4-05; ESV-M4-06; ESV-M4-07; ESV-M4-08; ESV-M4-09; ESV-M4-10
Current milestone/checkpoint: M4 Reconciliation R2 — Slot Policy Runtime Configuration — next gate / not activated
Current Unity version: 6000.3.8f1
Current implementation status: ESV-M4-R1 complete at `ab18361` with focused Chronicle Editor `618 / 618`; R2 next; M4 remains open; M5 locked
Known blockers: M4 audit gaps A-01 through A-04; R1 owns A-01/A-02 composition only
Current Notes reviewed through: August 11, 2026

Before writing Chronicle runtime code:
1. Rehydrate exact `48454ea` planning baseline.
2. Preserve the **587 / 587** focused Chronicle regression floor.
3. Implement only `ESV-M4-R1` public runtime composition authorized by ESV-D-033.
4. Reuse existing participant registry, catalog, technical slot creation, prepared-load, apply, save, recovery, and slot-operation authorities rather than duplicating them.
5. Do not change `EchoSaveConfiguration` schema or implement CAP-002 policy expansion in R1.
6. Do not implement package-document migration in R1; that is the approved R3 repair and CAP-014 remains intact.
7. Do not activate M5.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | Runtime package `0.1.0`; Specification v1.36.0 |
| Completed checkpoint | ESV-M4-10 - Destructive Slot Deletion Planning, Confirmed Trash, and Bounded Trash Retention Foundation |
| Implementation commit | `01e4cdd` |
| Tests passed | ESV-M4-10 focused Chronicle Editor gate `587 / 587`; prior `562 / 562` regression floor preserved; Unity compile/import green |
| Tests failed | Final ESV-M4-10 gate: `0` |
| Known issues | M4 audit gaps A-01 through A-04; R1 addresses public composition A-01/A-02 |
| Decisions added | ESV-D-001 through ESV-D-033; ESV-D-033 defines the approved R1/R2/R3 M4 reconciliation path and preserves CAP-014 document migration |
| Active learning checkpoint | PKG-LEARN-009 complete |
| Implementation permission | No reconciliation implementation checkpoint is active. R2 planning/authority activation is the next gate. |

## 30. Approval

### 30.1 Approval checklist

- [x] Package identity and responsibility are clear.
- [x] Ownership and exclusions align with SFGSS-000.
- [x] Independence proof is credible.
- [x] MVP is reliable without absorbing project schemas or cloud providers.
- [x] Slots, generations, manifests, payloads, head publication, and recovery are specified.
- [x] Participant capture/apply, unknown preservation, and migrations are specified.
- [x] Main/background thread and cancellation boundaries are explicit.
- [x] Setup, inspection, simulation, direct-scene, and Laboratory workflows are defined.
- [x] Diagnostics work without The Observatory.
- [x] Optional integrations are explicit and removable.
- [x] Performance, security, privacy, compatibility, adoption, and rollback are documented.
- [x] Test/release gates are measurable.
- [x] No Isekai Studios identity or ownership has been introduced.
- [x] Jesse’s standing approval to select the most effective long-term architecture has been applied.
- [x] Runtime implementation advances only through explicit bounded checkpoint gates.

### 30.2 Approval record

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams
**Date:** August 3, 2026
**Conditions:** This approval authorizes architecture and documentation only. Runtime implementation begins only after The Workshop specification and the Foundation cross-package consistency/readiness reviews are approved.

---

## Specification Completion Record

A new collaborator can answer from this document:

1. EchoSave owns durable local save transport, files, slots, generations, migrations, recovery, and operation diagnostics.
2. It refuses gameplay schema, global preference, scene-flow, UI, game-state, audio, input, and cloud-provider ownership.
3. Its MVP is one protected root with generation-based local saves, manifests, participant payloads, two-phase load, migration, backups, recovery, tooling, and an isolated Laboratory.
4. It installs and works alone.
5. Configuration assets remain immutable; slots, catalog snapshots, operation state, prepared loads, and participant registrations are runtime state.
6. Public requests, results, events, async boundaries, cancellation, and provider seams are specified.
7. Missing configuration, corruption, interruption, migration gaps, newer formats, oversized input, and participant failures have structured behavior.
8. The Save Laboratory proves normal, empty, invalid, interrupted, corrupt, migration, recovery, duplicate, stress, reset, and cleanup cases.
9. Other packages connect through bridges, project adapters, participant adapters, or provider packages.
10. Release requires the 32 Laboratory scenarios, applicable 100-case registry, fault injection, migration fixtures, performance/privacy/security evidence, documentation parity, and external installation tests.

The Chronicle specification is complete and **Approved v1.33.0**. PKG-LEARN-009, Chronicle M2, Chronicle M3, and ESV-M4-01 through ESV-M4-09 are complete. M4 remains active with ESV-M4-10 authorized as the bounded destructive delete/trash checkpoint.


---


## SUITE-DOC-30 Consistency Addendum

**Review status:** Passed
**Review date:** August 4, 2026
**Current governing authorities:** SFGSS-000 v0.26.0; SFGSS-001 v1.5.0; SFGSS-002 v1.1.0; SFGSS-003 v1.1.0; SFGSS-004 v1.4.0; SFGSS-005 v1.2.0; SFGSS-006 through SFGSS-010; SFGSS-ADR-001 through SFGSS-ADR-006; SFGSS-INT-SUITE-001 v1.1.0; and the approved Foundation, Expansion, and Advanced integration matrices.

The original parent-authority header remains approval provenance. This addendum records the standards that govern the specification after the full consistency review.

- The formal public title, technical identifier, package ID, namespace family, document ID, diagnostic/test prefix, setup facade, and planned repository were checked against SFGSS-008 and SFGSS-009.
- All implementation, compatibility, platform, performance, migration, Laboratory, provider, and release evidence remains `Not run` unless a retained execution record says otherwise.
- Package-qualified test and Laboratory IDs are authoritative. Pre-code range tables are planning shorthand only; implementation registries must expand them into individual definitions with separate automation class, execution status, evidence reference, and issue reference fields.
- A platform cell written as `Yes` in an older pre-code table means **planned design support**, not `Tested` or `Supported`, until SFGSS-004 evidence exists.
- Primary public Runtime assemblies may remain `autoReferenced: true`; Editor, test, sample, internal support, bridge, and provider assemblies default to `false` under SFGSS-002 unless this specification explicitly records a justified exception.
- Current Notes captures future discoveries, but durable changes return to this specification or an ADR before implementation advances.

**Package-specific repairs:**

- Clarified Unity asset GUID versus optional runtime/export save-configuration identity.

## Graph Navigation

#sfgss/package #sfgss/wave/foundation #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
