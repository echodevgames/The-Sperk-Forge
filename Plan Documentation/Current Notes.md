# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 15, 2026
**Current focus:** The Looking Glass (`EchoUI`) EUI-M2-02 — COMPLETE; no follow-on Looking Glass checkpoint activated
**Current checkpoint:** EUI-M2-02 — Blocking Modal Lifecycle, Exact-Once Results, and UI-Scoped Interaction Blocking — COMPLETE

> Durable First Light decisions live in SFGSS-PKG-ECHOLAUNCH-001 v1.17.0 and the committed checkpoint/amendment records. Git history preserves the longer working trail.

---

## Looking Glass EUI-M2-02 Final Closeout — August 15, 2026

- Activation commit: `e2145ab` (`e2145ab992542d5b3600429bdfe8a7ef419ce0a9`).
- Modal/Window clarification commit: `b6fc160` (`b6fc1601fb357f7ad09782a2b655154eb438ed56`).
- Implementation commit: `5ab34b3` (`5ab34b3c49a84cc9157b9bfdcef4b9defa1a16b6`), **41 files / 7760 insertions / 401 deletions**.
- Incoming full EditMode floor before Runtime edits: **1153 / 1153 passed, 0 failed**.
- Focused EUI-M2-02 Modal lifecycle: **28 / 28 passed, 0 failed**.
- Final EchoUI EditMode assembly: **75 / 75 passed, 0 failed**.
- Final Foundry EditMode regression: **1181 / 1181 passed, 0 failed, 0 skipped, 0 inconclusive**.
- Manual Laboratory: **12 / 12 PASS**.
- Retained M2-01 Screens tab: **PASS**.
- Retained M1 tab: **PASS**.
- EUI-M2-02 implements deterministic stacked blocking Modals, project-defined stable result IDs, fresh exact-once result generations, structural `Aborted`, RootOwned/SceneOwned/ExternalOwned lifetime, designer-authored Back policy, lower-UI blocking without gameplay authority, and explicit Screen `Reject` / bounded FIFO `DeferUntilModalStackClears`.
- Blocking Modal behavior remains distinct from independent Window coexistence. Future most-recent-eligible Window Back/Escape, pin/lock, dragging/resizing, and layout persistence remain separately gated.
- Package/imported Laboratory parity is synchronized to the manually accepted scene. Sample visual/readability polish remains sample-owned.
- The compile-only out-parameter correction and the EditMode root-shutdown test-harness correction changed no authorized modal contract.
- Runtime retains zero hard peer Echo package dependency and no persistence/project-wide lifetime authority.
- Package authority remains **SFGSS-PKG-ECHOUI-001 v1.4.1**; no new suite ADR is required.
- **EUI-M2-02 is COMPLETE. No follow-on Looking Glass checkpoint is activated by this closeout.**

## Looking Glass EUI-M2-02 JIT Reconciliation and Activation — August 14, 2026
- Incoming repository baseline: `d5b9a73` — EUI-M2-01 final closeout, clean and synchronized.
- Retained EUI-M2-01 proof: final full EditMode **1153 / 1153**, focused EchoUI **47 / 47**, M2-01 focused **23 / 23**, manual Laboratory **10 / 10**.
- Package authority advances to **SFGSS-PKG-ECHOUI-001 v1.4.1** after the adjacent post-activation modal/window clarification.
- Blocking modals may stack with top-only Looking Glass interaction and safe lower-entry handle cleanup.
- Semantic modal completions use project-defined stable result IDs.
- Each modal opening owns a fresh awaiter/handle generation; first valid terminal result wins exactly once.
- Unexpected admitted owner/view loss or shutdown produces structural `Aborted` rather than semantic Cancel.
- Modal ownership reuses RootOwned / SceneOwned / ExternalOwned lifetime rules.
- Modal Back behavior is designer-authored: disabled or mapped to a configured stable result ID.
- Looking Glass blocks lower Looking Glass UI interaction but does not own gameplay input, WASD/action maps, pause/time scale, or cursor policy.
- Blocking Modal semantics are role-specific: independent `Window` surfaces remain non-blocking/coexistent by default and may stay open while peer Windows and gameplay remain usable.
- Future independent-window Back/Escape behavior is explicitly separate from operation FIFO: it may use a most-recent-eligible **LIFO** dismissal history with authored/runtime pin/lock exclusions. That Window-manager behavior remains later work.
- While blocking modals are active, Screen mutation uses safe default `Reject` or explicit bounded `DeferUntilModalStackClears`; deferred work preserves FIFO.
- Modal visuals/backdrops remain project-authored. Blur/transitions, full focus/EventSystem policy, HUD/transients, Motifs, Builder, primitive expansion, persistence, peer bridges, and project-wide lifetime composition remain outside this checkpoint.
- **EUI-M2-02 is ACTIVE / AUTHORIZED. Runtime implementation has not started.**
- Next gate: re-establish **1153 / 1153** full EditMode on the activation commit before Runtime edits.

## Looking Glass EUI-M2-01 Final Closeout — August 14, 2026
- Activation commit: `0c11262` (`0c112628fd5f7361bee0e4ea6ff92b4effd39c2e`).
- Implementation commit: `8dc9c71` (`8dc9c712884f0774d7f84720fb35f4b95f8152bc`), **46 files / 6956 insertions / 216 deletions**.
- Incoming post-activation full EditMode floor was re-established before Runtime edits: **1130 / 1130 passed, 0 failed**.
- Focused EchoUI proof: **47 / 47 passed, 0 failed**, comprising **23 EUI-M2-01 tests + 24 retained M1 tests**.
- Manual Looking Glass Laboratory acceptance: **10 / 10 PASS**.
- Final synchronized full EditMode regression: **1153 / 1153 passed, 0 failed, 0 skipped, 0 inconclusive**.
- EUI-M2-01 implements project-defined stable ordered variable-count layers; immutable Screen definitions/runtime entries; RootOwned, SceneOwned, and ExternalOwned lifecycle; designer-controlled suspension visibility with scope-enforced noninteraction; deterministic Push/Replace/Reset/Back/Close; and bounded strict-FIFO structural operations.
- RootOwned lifetime is package-controlled only for instances Looking Glass actually creates; SceneOwned and ExternalOwned views remain externally owned and are never destroyed by Looking Glass.
- The suspension/resume interaction hotfix fixed restoration of the underlying interaction baseline after forced suspension without changing the contract.
- Package/imported Laboratory parity was retained after correcting the sample scene component ownership wiring.
- M1 independent Window, external-context, pointer/controller selection, and Back behavior remain green.
- Runtime retains zero hard peer Echo package dependency and no persistence/project-wide lifetime authority.
- The implementation matches **SFGSS-PKG-ECHOUI-001 v1.3.0**; no package-spec revision or new suite ADR is required.
- **EUI-M2-01 is COMPLETE. EUI-M2-02 — Blocking Modal Lifecycle and Exact-Once Modal Results — remains named direction only and is NOT ACTIVATED.**

## Looking Glass EUI-M2-01 JIT Reconciliation and Activation — August 14, 2026
- Incoming repository baseline: `c114ba2` — EUI-M1-02 final closeout, clean and synchronized.
- Retained EUI-M1-02 proof: final full EditMode **1130 / 1130**, focused EchoUI **24 / 24**, manual Laboratory **10 / 10**.
- PKG-LEARN-008 bounded EUI-M2-01 revisit is **Complete**.
- Looking Glass package authority advances to **SFGSS-PKG-ECHOUI-001 v1.3.0**.
- Active plan: **EUI-M2-01 — Authoritative Screen Lifecycle, Project-Defined Layers, and Serialized Screen Operations**.
- **EUI-M2-01 is explicitly ACTIVE / AUTHORIZED. Runtime implementation has not started at this activation record.**
- Fixed seven-layer runtime law is superseded. Layer topology is stable-ID-addressed, project-authored, ordered, and variable-count; package starter layers are editable convenience only.
- `RootOwned`, `SceneOwned`, and `ExternalOwned` are first-class screen ownership modes.
- Designers choose suspended-screen visibility behavior, while Looking Glass guarantees suspended Screens are non-interactive within the active navigation scope.
- Accepted Push/Replace/Reset/Back/Close structural requests execute one at a time in bounded strict FIFO submission order; overflow/rejection is explicit and partial history mutation is forbidden.
- M2-01 is screen-only. Modal blocking/exact-once result lifecycle remains the intended EUI-M2-02 slice and is not activated here.
- First implementation gate: re-establish the incoming full EditMode **1130 / 1130** floor before changing Runtime code.

## Looking Glass EUI-M1-02 Final Closeout — August 14, 2026
- Activation commit: `f0b97ff` (`f0b97ffa3e4c0aa5ddd82fe028176da4e2fefa20`).
- Implementation commit: `1c0a46a` (`1c0a46aad8fe23c1a241bed04d4f864042fdd577`).
- Incoming full EditMode floor was re-established before Runtime edits: **1113 / 1113 passed, 0 failed**.
- Final focused EchoUI assembly proof: **24 / 24 passed, 0 failed**, comprising **17 EUI-M1-02 focused tests + 7 retained EUI-M1-01 foundation tests**.
- Manual Looking Glass Laboratory acceptance: **10 / 10 PASS**.
- Final synchronized full EditMode regression: **1130 / 1130 passed, 0 failed, 0 skipped, 0 inconclusive**.
- Package/imported Laboratory parity was reconciled before the final regression; the package sample preserves the tested `Button_DefaultClose` default-selection target and top-right proof-console safe zone.
- EUI-M1-02 proves project-defined active/inactive context IDs, simultaneous contexts, designer-authored ordered per-surface rules, per-dimension resolution, opt-out, transient runtime overrides, external input modality, pointer-unselected behavior, controller/default selection, neutral close selection, and retained M1-01 navigation/window behavior.
- Pause/cinematic/loading and input modality remain external truth. A surface with no applicable pause rule remains unchanged; designers explicitly author hide/non-interactable behavior when desired.
- Runtime retains **zero hard peer Echo package dependency** and introduces no persistence or project-wide lifetime authority.
- The implementation matched **SFGSS-PKG-ECHOUI-001 v1.2.0**; no package-authority revision or new suite ADR was required at closeout.
- EUI-M1-02 is **COMPLETE**. Motifs, Builder, reusable preset/template tooling, broad primitive-library expansion, richer window/focus systems, MMO layout persistence, arbitrary context payloads, peer bridges, and richer modal/HUD/transient systems remain future work.
- **No follow-on Looking Glass checkpoint is activated by this closeout.**

## Looking Glass EUI-M1-02 JIT Reconciliation and Activation — August 13, 2026

- Incoming repository baseline: `57a4fa4` — EUI-M1-01 final recovery, working tree clean/synchronized at retained closeout.
- Retained EUI-M1-01 proof: full EditMode **1113 / 1113 passed, 0 failed**; manual Laboratory **5 / 5**.
- PKG-LEARN-008 bounded EUI-M1-02 revisit is **Complete**.
- Looking Glass package authority advances to **SFGSS-PKG-ECHOUI-001 v1.2.0**.
- Active plan: **EUI-M1-02 — External UI Context, Ordered Surface Response Rules, and Input-Aware Selection Contract**.
- **EUI-M1-02 is explicitly ACTIVE / AUTHORIZED.** Runtime implementation has not started at this activation record.
- Context IDs are stable/project-defined and active/inactive only in this slice; Looking Glass consumes the truth without owning pause/cinematic/loading/gameplay state.
- Multiple contexts may coexist. Designers own ordered per-surface precedence; resolution is per visibility/interactability/selection dimension, and unspecified values cause no intervention.
- Surfaces may opt out of automatic external-context handling.
- Effective behavior may resolve from authored defaults, local/instance overrides, and transient project runtime overrides. Runtime overrides are not persistence and must not mutate authored definitions.
- Input modality remains external. Per-surface selection policy may choose controller default-selection or unselected behavior; pointer behavior may remain unselected.
- Closing a temporary surface defaults to no selected control; implicit selection-history restoration is outside this slice.
- Future presets/templates are approved as editable copy-in conveniences, but preset tooling, Motifs, Builder, broad primitive expansion, MMO layout persistence, arbitrary context payloads, peer bridges, and richer modal/HUD/transient systems remain excluded.
- First implementation gate: re-establish the incoming full EditMode **1113 / 1113** floor before changing runtime code. Any mismatch, unexpected repository state, or authority-changing discovery is a Green Path stop.


## Looking Glass Kickoff — PKG-LEARN-008 / EUI-M1-01

- Starting repository baseline: `f57880a` — First Light FL-M5-R1 sealed and working tree clean.
- PKG-LEARN-008 is **Complete** through the new Learn → Declare → Authorize JIT model.
- Looking Glass package authority advances to **SFGSS-PKG-ECHOUI-001 v1.1.0**.
- SFGSS-005 advances to **v1.6.0** and SFGSS-ADR-007 accepts the Green Path self-validating checkpoint protocol.
- Exact observed Unity baseline: **6000.3.8f1**; project manifest contains **uGUI 2.0.0** and Input System **1.18.0**.
- EUI-M1-01 is the first Green Path package checkpoint and is explicitly active/authorized.
- Looking Glass uses stable project-authored surface IDs and supports both exclusive `Screen` navigation scopes and independent `Window`/HUD/Overlay surfaces.
- UI context is layered. Pause/Cinematic/Loading/Input-modality truth is supplied externally; the Laboratory may use a tiny sample-owned controller until real project adapters exist.
- Default Back is history-based with explicit navigation/root/resume-style actions available to designers.
- Future visibility policy cascades per surface instead of globally hardcoding “Pause hides HUD.”
- Default selection will become input-modality aware without making Looking Glass depend on The Will or Controller packages.
- Sample/editor hierarchy naming convention: **`Type_DescriptiveName`**, e.g. `Canvas_MasterCanvas`, `Panel_MenuRoot`, `Button_DefaultButton`.
- Future package pillars are Runtime Context/Navigation, Lego Primitive Library, Motif System, and Editor Builder.
- **Motif** is the approved front-facing term for reusable appearance recipes; Motifs own appearance only and later support capture/apply plus explicit local overrides.
- A queryable Surface Registry is an approved future-facing contract so project UI can build selectors/window managers and, eventually, the inevitable `Panel_MenuForMenusMenu`.

### EUI-M1-01 stop point

The first implementation proves only: package-local root authority, stable surface registration, `main-menu -> settings -> Back -> main-menu` inside `frontend`, and an independent `default-window` that can coexist without changing the active screen. Context rules, input-aware selection, Motifs, primitive prefabs, Builder, modal/HUD/transient systems, and cross-package bridges remain outside this slice.

## Current State

- First Light package version remains `0.1.0`.
- First Light package specification advances to **SFGSS-PKG-ECHOLAUNCH-001 v1.17.0** for FL-M5-R1 closeout.
- FL-M5-07 Standalone Test Laboratory is complete at `710aec3` with retained automated evidence `809 / 809` and manual Laboratory evidence `12 / 12`.
- FL-M6-01 Production Reference Showcase implementation and in-repository consumer proof are complete.
- A1 splash presentation/authoring is committed across `1b7ab84`, `d36b5cc`, `90e038c`, `9b24121`, and `4bdc264`.
- Slice E Setup creation-time splash authoring is committed at `9e6df00`.
- A1-E1 project-owned foundation resolution is authorized at `a70e478` and implemented at `e66b9fd`.
- The earlier First Light Gallery structure was reorganized at `b43e6bf`: UMBRA remains project-owned First Light showcase content, while imported package samples live separately beneath `Assets/Samples`. FL-M5-R1 then restored the concrete First Light package sample as `FirstLight_Boot_Splash_Laboratory`.
- The final `EchoLaunchSetup` filtered EditMode gate passed **224 / 224**.
- FL-M5-R1 produced a fresh full EditMode rerun of **1106 / 1106 passed, 0 failed** after reconciling fourteen stale sample-identity tests. No new Runtime Play Mode aggregate is claimed by R1; FL-M5-07 runtime/full-suite evidence remains retained history.
- SUITE-DIST-001 is complete at `c18eff6`; First Light `0.1.0` has a repository-owned Distribution Kit and Complete User Handout.
- FL-M5-R1 is complete at implementation commit `cea876e`; First Light remains frozen for this pass.
- Jesse selected The Chronicle (`EchoSave`) as the next active package learning review.
- The next multi-package initiative is the **Game Shell / Front Door**: Chronicle → Accord → Resonance → Looking Glass, composed later with First Light while preserving standalone package ownership.

## First Light FL-M5-R1 Closeout

- Activation: `93182c5`.
- Implementation: `cea876e` (`174 files / 3703 insertions / 47 deletions`).
- Canonical package sample: `Samples~/FirstLight_Boot_Splash_Laboratory/`.
- Package Manager display name: **First Light Boot Splash Laboratory**.
- Imported project proof: `Assets/Samples/First Light — Startup and Launch/0.1.0/First Light Boot Splash Laboratory/`.
- Package/imported Laboratory parity synchronized across `78` files.
- Serialized destination paths and sample-test expectations use the new concrete identity.
- Final shipped sample retains EchoDevGames splash, revised First Light art, basic camera plumbing, and authored Boot/Destination proof.
- Optional Splash Shake presets are `None`, `Subtle`, `Medium`, `Nightmare`; `None = 0`; schema remains `1`; Reduced Motion disables the effect.
- Shake is local to the startup splash presentation surface and never claims gameplay-camera/global feedback authority.
- Post-reconciliation full EditMode gate: **1106 / 1106 passed, 0 failed**.
- Final manual `Nightmare` playback was visible and successful.
- UMBRA remains a separate project-owned First Light showcase and does not replace the package sample.
- No additional First Light work is active.

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

## Historical FL-M6-01 Gallery Proof

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

This block records the historical FL-M6-01 Gallery proof before the later `b43e6bf`/FL-M5-R1 organization pass. Current organization separates the package-owned First Light Boot Splash Laboratory from UMBRA, which remains project-owned First Light showcase content. Showcase content is not package content, a UPM sample, a required dependency, or evidence that The Sperk’s Forge is an Isekai Studios product.

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


## ESV-M5-01 closeout

**Activation commit:** `affe3ae`
**Implementation commit:** `69721af`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.45.0 / ESV-D-037
**Focused Chronicle Editor:** **697 / 697 passed, 0 failed**
**Incoming floor:** **660 / 660**
**Net-new focused tests:** **37**
**Implementation/test scope:** **21 files**, `2404` insertions, `1` deletion
**Runtime C# changes:** **0**

M5-01 establishes the bounded Chronicle Editor tooling foundation:
- Setup preview-before-mutation;
- create-only current schema-2 configuration authoring;
- safe no-clobber occupied-target rejection;
- Runtime-consistent slot-policy capacity preview;
- read-only deterministic Validator coverage for `ESV-VAL-001`, `002`, `003`, `009`, and `015`.

Manual proof confirmed successful Preview, explicit Apply, `ESV-SETUP-002` no-overwrite behavior on the occupied target, Validator **Issues: 0**, and cleanup back to a clean repository.

A small Setup path-field clarity advisory was observed during the manual pass when an entry produced `Assets/Assets/...`; the tool rejected safely with zero mutation. It is not an M5-01 blocker.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**. **M5-01 is Complete. M5 remains open. No M5-02 slice is active until a separate authority/activation checkpoint is committed.**


## ESV-M5-02 activation

**Planning baseline:** `8774dd2`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.46.0 / ESV-D-038
**Incoming focused Chronicle floor:** **697 / 697 passed, 0 failed**

ESV-M5-02 is the separately bounded continuation of M5 Setup/configuration work.

Authority locks:
- schema-3 authoring/upgrade is explicit and compatibility-preserving;
- runtime never silently rewrites schema-1/schema-2 assets;
- every existing-asset/root/reference repair has a deterministic Preview before mutation;
- Apply is explicit and Undo/backup-safe where Unity permits;
- ambiguous/destructive repairs fail closed;
- authored provider/retention/limit/recovery/template values are exposed only when they have real runtime/tooling semantics;
- Browser/Simulator/Laboratory and production save-data repair remain later checkpoints.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**. M5 remains open.


## ESV-M5-02 closeout

**Activation commit:** `3456489`
**Implementation commit:** `d2e9252`
**Repository hygiene before documentation closeout:** `423fac1` repaired an unrelated pre-existing empty First Light Example folder `.meta`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.47.0 / ESV-D-038
**Focused Chronicle Editor:** **724 / 724 passed, 0 failed**
**Incoming floor:** **697 / 697**
**Net-new focused tests:** **27**
**Implementation scope:** **23 files**, `3268` insertions, `281` deletions

M5-02 closes the full Setup/configuration-authoring and safe selected-reference repair slice:
- current schema-3 authoring;
- deterministic schema-1/schema-2 compatibility without runtime asset rewriting;
- immutable runtime-policy snapshot;
- configured retention/provider/limit/manual-recovery truth;
- optional fixed-slot-template authoring metadata;
- Preview-first Create/Edit/schema-upgrade Apply;
- selected `EchoSaveRoot` configuration-reference Preview/Apply with Undo;
- expanded read-only Validator checks.

Manual proof confirmed schema-2 → schema-3 Preview/Apply with read-only pre-Apply state, stable schema-3 `NoChanges` afterward, exact root-reference repair Preview, explicit Apply, successful Undo back to `None`, and Validator **Issues: 0**.

The unrelated metadata repair in `423fac1` changed no Chronicle implementation file and is not part of ESV-M5-02 scope.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**. **ESV-M5-02 is Complete. M5 remains open. No M5-03 slice is active until a separate authority/activation checkpoint is committed.**


## ESV-M5-03 activation

**Planning baseline:** `b4d4d0b`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.48.0 / ESV-D-039
**Incoming focused Chronicle floor:** **724 / 724 passed, 0 failed**

ESV-M5-03 is the separately bounded read-only inspection slice.

Authority locks:
- inspection never mutates production save data or project configuration;
- Browser state comes from actual Chronicle catalog/slot/generation truth rather than inferred filenames;
- Generation Inspector treats committed generation bytes and package documents as immutable evidence;
- Migration Graph reflects registered migration edges and reachable paths only;
- unsupported-newer or missing-migration states remain visible and fail closed rather than being “fixed” by the inspector;
- refresh/selection is deterministic and does not execute recovery;
- any required new runtime surface must be additive and read-only.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**. M5 remains open.


## ESV-M5-03 closeout

**Activation:** `e805ae3`
**Implementation:** `9c3771c`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.49.0 / ESV-D-039
**Focused Chronicle Editor:** **735 / 735 passed, 0 failed**
**Incoming floor:** **724 / 724**
**Net-new focused tests:** **11**
**Implementation scope:** **26 files**, `2419` insertions

Manual inspection proof is complete:
- Browser empty-root/no-create: `SucceededEmpty`, `Slots (0)`, no production root created;
- Migration Graph: valid production registry, zero registered edges, all four current production document kinds at `1.0.0`;
- Generation Inspector: disposable real Chronicle slot/generation reported `CURRENT`, `Healthy`, `Committed`, manifest `1.0.0 -> 1.0.0`, no in-memory migration, `0 participants / 238 bytes`;
- disposable generation/root, temporary proof seeder, and disposable configuration were removed;
- unrelated First Light scene and generated solution noise were restored;
- final repository verification was silent/clean at `9c3771c`.

The inspection boundary remained zero-write except for the deliberately separate temporary seed/cleanup flow used solely to create proof evidence. Browser refresh, selection, Generation Inspector, and Migration Graph did not mutate save/project state.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.

**ESV-M5-03 is Complete. M5 remains open. M5-04 requires separate activation.**


## ESV-M5-04 activation

**Planning baseline:** `ffff18f`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.50.0 / ESV-D-040
**Incoming focused Chronicle floor:** **735 / 735 passed, 0 failed**

Authority locks:
- Failure Simulator and Test Data Generator are sandbox-only.
- Sandbox resolution must fail closed if it collides with or nests into the configured production Chronicle root.
- Recovery Planner is descriptive/preview-only; it never executes recovery or changes a head.
- Redacted Snapshot Exporter is explicit-action and payload-free.
- Full local paths are omitted/redacted from support output by default.
- Technical slot identity is redacted/hashed in support mode.
- Tooling remains bounded by explicit record/count/byte limits.
- No direct-scene Laboratory is activated by M5-04.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**. M5 remains open.


## ESV-M5-04 closeout

**Activation:** `df3c30b`
**Implementation:** `577dc01`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.51.0 / ESV-D-040
**Focused Chronicle Editor:** **746 / 746 passed, 0 failed**
**Incoming floor:** **735 / 735**
**Net-new focused tests:** **11**
**Implementation scope:** **31 files**, `3206` insertions

Manual QA/support proof is complete:
- bounded deterministic Test Data Preview/Generate/no-clobber/Cleanup;
- Failure Simulator exact-target `Truncate Manifest` Preview/Apply/verified Cleanup;
- Recovery Planner preview-only absent-root proof with no Apply/Recover control;
- Redacted Snapshot explicit export with hashed tokens and zero raw slot ID/full local path/participant payload markers.

A pre-commit compile correction replaced the mistaken `SaveSlotPolicy.EffectiveTechnicalSlotCapacity` member reference with the existing `SaveSlotPolicy.EffectiveCapacity` property. The corrected implementation is what was tested, committed, and pushed at `577dc01`.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.

**ESV-M5-04 is Complete. M5 remains open. M5-05 requires separate activation.**


## ESV-M5-05 activation

**Planning baseline:** `1111b46`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.52.0 / ESV-D-041
**Incoming focused Chronicle floor:** **746 / 746 passed, 0 failed**

M5-05 is a prerequisite-completion slice, not the Laboratory itself.

Authority locks:
- unknown payload pruning requires an explicit source-bound Preview/plan and exact named unknown IDs;
- Confirm must fail closed if source generation/provenance changes or a named ID becomes claimed/known;
- successful prune creates a new immutable generation and preserves every unnamed unknown entry byte-for-byte;
- committed historical generations are never edited;
- `catalog.cache.json` is derived acceleration only and never durable slot authority;
- missing/corrupt/stale/incompatible cache falls back to bounded head/manifest rebuild;
- cache rebuild may replace only the cache;
- cache write failure is reported as maintenance truth and cannot fabricate save/catalog rollback;
- the Editor may expose Preview + explicit Rebuild Cache for LAB-029 preparation;
- no production quarantine cleanup, trash restore, permanent erase, automatic recovery, or Laboratory scene is activated.

### M5 sequence correction

The earlier shorthand `M5-05 Save Laboratory` is superseded because the Laboratory acceptance matrix itself requires unimplemented LAB-016 unknown prune and LAB-029 catalog-cache rebuild.

The corrected sequence is:
- **M5-05:** prune/cache prerequisites;
- **M5-06:** Chronicle Save Laboratory + LAB-001 through LAB-032.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**.


## ESV-M5-05 closeout

**Activation:** `94c33a3`
**Implementation:** `ad715c3`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.53.0 / ESV-D-041
**Focused Chronicle Editor:** **753 / 753 passed, 0 failed**
**Implementation scope:** **33 files / 4118 insertions / 3 deletions**

ESV-M5-05 is **Complete**.

LAB-016 prerequisite evidence completed exact-ID unknown prune with zero-write Preview, source/provenance binding, one-entry prune, one remaining unknown, new immutable generation publication, historical-source byte immutability, unnamed-unknown byte preservation, known-payload preservation, and successful head publication.

LAB-029 prerequisite evidence completed the derived cache chain `Missing -> Rebuild -> Valid -> Stale -> Rebuild -> Valid`, including a deliberate durable-head advance without cache maintenance and final matching durable/cache fingerprints.

The ownership-marked production proof fixture was removed and post-cleanup absence was verified. Temporary proof tooling/disposable configuration/Unity solution noise were removed before implementation commit.

The R4 registry remains **61 Complete / 39 Deferred / 0 Blocked**. M5 remains open. **ESV-M5-06 Save Laboratory is not activated.**


## ESV-M5-06 activation

**Planning baseline:** `868b17f`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.54.0 / ESV-D-042
**Incoming focused Chronicle floor:** **753 / 753 passed, 0 failed**

M5-06 is deliberately the smallest useful Chronicle direct-scene engineering Lab. One tiny Sperk-themed persisted participant, crude controls/readouts, deterministic fixtures, and a visible evidence console are enough. It must not establish production UI architecture or depend on Looking Glass/Resonance.

The later Chronicle Reference Showcase owns polished examples of recognizable save-system formats after the suite presentation packages are available.

## Chronicle ESV-M5-06 closeout and M5 completion

- Activation: `d6f079a`.
- Implementation: `4bcfbf1`.
- Adjacent project organization: `b43e6bf`.
- Final focused Chronicle Editor gate: **761 / 761 passed, 0 failed**.
- M5-06 adds **8** net-new focused tests over the incoming **753 / 753** floor.
- Final Chronicle package scope: **21 files / 2341 insertions / 0 deletions**.
- The package now declares/imports `Chronicle Save Laboratory` through `Samples~`.
- Initial sample import exposed one sample-only `CS4008 Cannot await 'void'` defect; `InitializeLaboratoryAsync()` was corrected from `async void` to `async Awaitable`, the imported sample was reimported, and the final 761-test gate remained green.
- A basic camera/background was added to the Laboratory scene and synchronized into the package source so human Game-view proof no longer carries Unity's no-camera overlay.
- Direct-scene evidence proved authoritative readiness, create/select, Save -> unsaved mutation -> Load exact restoration, prepared-load one-use behavior, rename, duplicate, delete Preview/Confirm, catalog reconciliation, and ownership-verified reset/cleanup.
- `RESULT: THE CHRONICLE REMEMBERS.` was observed after both normal Load & Apply and prepared-load apply.
- LAB-001 through LAB-032 are reconciled from retained M4/M5 automated evidence, M5-05 prerequisite/manual evidence, and the M5-06 direct-scene proof rather than 32 separate UI workflows.
- **Chronicle M5 — Tooling and Laboratory is Complete.**
- M6 First Integration is not activated.

### Sample / Showcase separation follow-up

The project keeps imported package sample content separate from polished project-owned showcase content. UMBRA remains a First Light showcase and does not satisfy First Light's package-sample requirement. FL-M5-R1 completed the bounded follow-up and restored/hardened the package sample as `FirstLight_Boot_Splash_Laboratory`; UMBRA was never promoted into that role.

For Chronicle, the current `Chronicle Save Laboratory` is intentionally the package sample/engineering harness. Rich player-facing Chronicle save-menu examples remain deferred to the later Reference Showcase after Looking Glass and preferably Resonance are available.

## EUI-M1-01 closeout

**Activation:** `83d3f9e`
**Implementation:** `e6b651f`
**Partial closeout:** `4f94c3b`
**Final recovery:** `57a4fa4`
**Full EditMode:** **1113 / 1113 passed, 0 failed**
**Manual Laboratory:** **5 / 5 passed**

Looking Glass now has its first installable surface/navigation foundation. The proof establishes package-local root authority, stable project-authored surface IDs, one exclusive `frontend` screen scope with history-based Back, and an independent `default-window` that can coexist without changing the active frontend screen.

The hand-authored Laboratory is retained both as the imported project sample and package-owned `Samples~/LookingGlass_UI_Foundation_Laboratory/` content. Structural organizational roots are intentionally active and non-raycast; registered `UISurface` children own visibility. The Laboratory uses normal modern Unity UI authoring with TextMesh Pro labels and the project's TMP Essential Resources; TMP Examples & Extras are not part of the sample requirement.

First Green Path execution exposed workflow hardening items: establish the incoming regression floor before implementation, prefer focused post-compile tests before the final full regression run, treat documentation-generator/anchor failures as fatal, sanitize Unity-authored YAML/meta whitespace before staging, keep exported test evidence outside distributable samples, and normalize evidence paths before CMD parsing. These are process lessons for subsequent Green Path kits; they do not reopen EUI-M1-01 runtime scope.

**Historical closeout state at `57a4fa4`: EUI-M1-01 was Complete and EUI-M1-02 was not yet activated. This status is superseded by the August 13, 2026 EUI-M1-02 activation record at the top of this document.**
