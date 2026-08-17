# The Looking Glass — Developer Current Notes

**Package:** `com.echodevgames.echo-ui`
**Package version:** 0.1.0
**Authority:** SFGSS-PKG-ECHOUI-001 v1.9.0
**Current checkpoint:** EUI-M4-03 Runtime Motif Foundation — ACTIVE / AUTHORIZED; root test correction published next, automated rerun pending
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0

## Current boundary

EUI-M1-01 and EUI-M1-02 are complete. The retained foundation provides package-local root authority, stable surface registration, scoped Screen history/Back, independent Window coexistence, externally supplied context response, designer-ordered per-dimension visibility/interactability/selection policy, transient overrides, and input-aware selection.

EUI-M2-01 is the completed first Runtime Core slice. It implements project-defined ordered layer topology, explicit Screen lifecycle/ownership, designer-controlled suspension visibility with scope-enforced noninteraction, and bounded strict-FIFO structural Screen operations.

EUI-M2-02, EUI-M3-01, EUI-M3-02, EUI-M4-01, and EUI-M4-02 are complete. EUI-M4-02 supplies project-defined bounded notification channels, priority/FIFO ordering, opt-in coalescing, pending-overflow policies, unscaled/manual lifetime, generation-safe handles, owner/presentation-loss cleanup, status/events, tests, and a replaceable project presenter seam.

EUI-M4-03 is active from clean closeout `2f59251`. Runtime is implemented through root integration: contracts `d67550d`, catalog/fallback `172d230`, session service `43da17a`, targets `efbc503`, reusable bindings `e17d816`, and root integration `ab5906c`. The reusable-binding gate is accepted at full Foundry **1433 / 1433**, EchoUI **327 / 327**, and all Motif fixtures **50 / 50**. The first root-integration run compiled and executed **1445** tests but produced **1444 passed / 1 failed** because the EditMode destruction test did not explicitly invoke the root's private `OnDestroy()` method. The bounded test-only correction is the current resume point; Runtime is unchanged. Full accessibility policy, settings persistence, Motif capture/apply/preview tooling, final Primitive Warehouse-facing schema, Primitive Warehouse/templates/catalog/utilities, Builder, prompts/tooltips, safe area, richer Window management, peer bridges, and project-wide lifetime composition remain separately gated future work.

## EUI-M4-03 declaration

- **Planning baseline:** `2f59251` — clean synchronized EUI-M4-02 final closeout.
- **Authority:** SFGSS-PKG-ECHOUI-001 v1.9.0 under unchanged SFGSS-000 v0.27.0.
- **Incoming floor to re-establish before Runtime edits:** full Foundry EditMode **1383 / 1383** and EchoUI Editor **277 / 277**, zero failed.
- Motifs own reusable appearance recipes only: initial typed tokens are colors, uGUI Selectable state-color recipes, sprites, and small target-understood decorative/numeric values.
- Definitions remain immutable; runtime consumes detached snapshots and never writes effective selection or target state back into ScriptableObjects.
- Root default applies at initialization. Explicitly registered targets receive current effective truth immediately; there is no hierarchy scan or per-frame reapply loop.
- Target bindings preserve an explicit local value when authored to do so.
- Unknown requested IDs resolve to the valid authored fallback without rewriting external preference input; absent fallback preserves last known-good appearance.
- Valid Motif truth commits before project-target application; failed targets/listeners are isolated and reported without fabricated rollback.
- Registration handles are fresh, generation-safe, and idempotent; destroyed owner/target cleanup cannot remove a newer registration.
- No TextMeshPro or peer Echo dependency is added.
- Final warehouse-facing typography/provider schema and Editor capture/apply/preview tooling remain deferred until real primitive authoring exposes the smallest useful contract.
- **Exact resume gate:** run repaired `RootDestructionShutsDownMotifRegistrations`, then complete EditMode. Expected: root fixture **12 / 12**, all Motifs **62 / 62**, EchoUI **339 / 339**, full Foundry **1445 / 1445**, zero failed/skipped/inconclusive. Do not begin Laboratory work before acceptance.

## Retained completion evidence

- EUI-M1-01: activation `83d3f9e`; implementation `e6b651f`; final recovery `57a4fa4`; full EditMode **1113 / 1113**; manual Laboratory **5 / 5**.
- EUI-M1-02: activation `f0b97ff`; implementation `1c0a46a`; closeout `c114ba2`.
- EUI-M1-02 final full EditMode: **1130 / 1130 passed, 0 failed, 0 skipped, 0 inconclusive**.
- EUI-M1-02 focused EchoUI: **24 / 24** (**17 M1-02 + 7 retained M1-01**).
- EUI-M1-02 manual Laboratory: **10 / 10 PASS**.
- Package/imported Laboratory parity includes `Button_DefaultClose` and the top-right proof/debug safe zone.

## EUI-M2-01 declaration

- **Planning baseline:** `c114ba2` — clean synchronized EUI-M1-02 closeout.
- **Authority:** SFGSS-PKG-ECHOUI-001 v1.3.0.
- **Incoming full EditMode floor:** **1130 / 1130 passed, 0 failed**.
- Fixed seven-layer runtime law is superseded by stable-ID project-defined ordered layer definitions; any starter arrangement is editable convenience.
- `RootOwned`, `SceneOwned`, and `ExternalOwned` are first-class screen ownership modes.
- Suspended screen visibility is designer-controlled; suspended Screens remain non-interactive inside their scope.
- Accepted structural screen mutations use bounded strict FIFO ordering. No silent M2-01 reorder/coalesce/drop policy.
- Push/Navigate, Replace, Reset/Return-to-root, Back, and Close are the bounded lifecycle operations.
- Operation/factory/lost-view/queue rejection must not partially mutate authoritative history or ownership state.
- Modal exact-once results are explicitly deferred to EUI-M2-02.
- **Runtime implementation has not started at activation.**

## EUI-M2-01 closeout

- Activation: `0c11262` (`0c112628fd5f7361bee0e4ea6ff92b4effd39c2e`).
- Implementation: `8dc9c71` (`8dc9c712884f0774d7f84720fb35f4b95f8152bc`).
- Incoming post-activation full EditMode floor: **1130 / 1130 passed, 0 failed** before Runtime edits.
- Focused EchoUI proof: **47 / 47 passed, 0 failed**, comprising **23 EUI-M2-01 tests + 24 retained M1 tests**.
- Final synchronized full EditMode regression: **1153 / 1153 passed, 0 failed, 0 skipped, 0 inconclusive**.
- Manual Laboratory acceptance: **10 / 10 PASS**.
- Package/imported Laboratory proof remains synchronized and retains the top-right proof/debug safe zone.
- The suspension/resume interaction correction restores the pre-suspension interaction baseline without weakening the rule that suspended Screens are non-interactive.
- RootOwned views are created/released by Looking Glass; SceneOwned and ExternalOwned object lifetime remains outside Looking Glass authority.
- Runtime retains zero hard dependency on another Echo package.
- Implementation matched **SFGSS-PKG-ECHOUI-001 v1.3.0**; no package-authority revision or new suite ADR is required at closeout.
- **EUI-M2-01 is COMPLETE. EUI-M2-02 is named follow-on direction only and is not activated.**

## EUI-M2-02 declaration

- **Planning baseline:** `d5b9a73` — clean synchronized EUI-M2-01 final closeout.
- **Authority:** SFGSS-PKG-ECHOUI-001 v1.4.1.
- **Incoming full EditMode floor to re-establish before Runtime edits:** **1153 / 1153 passed, 0 failed**.
- Blocking modals may stack; only the top eligible modal receives normal Looking Glass interaction.
- Normal completion uses project-defined stable result IDs; no package-reserved yes/no/cancel vocabulary.
- Every admitted opening receives a fresh awaiter/handle generation; first valid terminal completion wins exactly once.
- Unexpected post-admission owner/view loss or shutdown settles as structural `Aborted`, distinct from semantic Cancel.
- `RootOwned`, `SceneOwned`, and `ExternalOwned` are first-class modal ownership modes with the same lifetime boundary as Screens.
- Back is designer-authored per modal: disabled or complete with one configured stable result ID.
- Blocking modals gate lower Looking Glass pointer/navigation/submit/Back interaction only. Gameplay input, WASD/action maps, pause/time scale, and cursor authority remain external.
- **Clarification:** blocking semantics apply only to the blocking `Modal` lifecycle. Independent `Window` surfaces remain non-blocking/coexistent by default and may stay open while peer Windows and gameplay remain usable according to project-authored input/raycast policy.
- **Clarification:** M2-01 FIFO describes accepted operation execution order, not Back/Escape close order. Future independent-window Back/Escape behavior is reserved for a separate most-recent-eligible **LIFO** dismissal history, with authored/runtime pin/lock exclusions. That Window-manager capability is not implemented by EUI-M2-02.
- Screen structural mutation policy while a blocking modal stack is active is designer/project controlled between safe default `Reject` and bounded FIFO `DeferUntilModalStackClears`.
- Modal visual backdrop/style remains project-owned; generalized dim/blur/transitions remain later work.
- EUI-M2-02 excludes full focus-history restoration/EventSystem adoption, transitions, HUD/transients, Motifs, Builder, primitive expansion, arbitrary modal domain payload transport, persistence, peer bridges, and project-wide lifetime composition.
- **Runtime implementation was not started at activation; it is now complete at `5ab34b3`.**

## EUI-M2-02 closeout

- Activation: `e2145ab` (`e2145ab992542d5b3600429bdfe8a7ef419ce0a9`).
- Modal/Window clarification: `b6fc160` (`b6fc1601fb357f7ad09782a2b655154eb438ed56`).
- Implementation: `5ab34b3` (`5ab34b3c49a84cc9157b9bfdcef4b9defa1a16b6`), **41 files / 7760 insertions / 401 deletions**.
- Incoming full EditMode floor before Runtime edits: **1153 / 1153 passed, 0 failed**.
- Focused EUI-M2-02 Modal lifecycle: **28 / 28 passed, 0 failed**.
- Final EchoUI EditMode assembly: **75 / 75 passed, 0 failed**.
- Final Foundry EditMode regression: **1181 / 1181 passed, 0 failed, 0 skipped, 0 inconclusive**.
- Manual Laboratory acceptance: **12 / 12 PASS**.
- Retained M2-01 Screens tab: **PASS**.
- Retained M1 tab: **PASS**.
- The compile-only `TryGetDefinition` out-parameter correction and EditMode root-shutdown test-harness correction changed no authorized modal contract.
- Package/imported Laboratory parity is synchronized to the accepted scene/readability polish; those visual choices remain sample-owned rather than Runtime defaults.
- Blocking Modal semantics remain distinct from independent Window coexistence; future Window LIFO/pin/layout work remains separately gated.
- Runtime retains zero hard peer Echo dependency and claims no gameplay-input, pause/time-scale, cursor, persistence, or project-wide lifetime authority.
- Implementation matched **SFGSS-PKG-ECHOUI-001 v1.4.1**; no package-authority revision or new suite ADR is required at closeout.
- **EUI-M2-02 is COMPLETE. No follow-on Looking Glass checkpoint is activated by this closeout.**


## EUI-M3-01 declaration

- **Planning baseline:** `0b7622c` — clean synchronized repository-hygiene boundary after EUI-M2-02 closeout `7f5ad40`.
- **Authority:** SFGSS-PKG-ECHOUI-001 v1.5.0 under SFGSS-000 v0.27.0.
- **Incoming full EditMode floor:** **1181 / 1181 passed, 0 failed**.
- Retained EchoUI: **75 / 75**; focused M2-02: **28 / 28**; M2-02 Laboratory: **12 / 12 PASS**.
- EventSystem modes are `AdoptAssigned`, deterministic `AdoptExisting`, `CreateIfMissing`, and `RequireExternal`; external systems are never silently destroyed or disabled.
- Multiple eligible active EventSystems produce degraded/blocking focus status rather than arbitrary adoption.
- Focus memory is per live entry, with optional transient stable-surface root-session memory.
- Reopen policy remains designer-controlled between fresh/default behavior and remember-this-session.
- Restoration resolves explicit -> remembered -> authored default -> entry resolver -> global fallback -> legal no-focus.
- Pointer/navigation focus behavior remains designer-controlled.
- Blocking Modal focus is structurally contained to the top eligible Modal while lower-entry focus memory survives.
- Independent Windows may keep distinct focus memory without activating the later Window manager.
- Focus maintenance is event-driven by default with explicit project-callable revalidation; no universal per-frame scan is required.
- Stale operation/generation focus requests cannot overwrite newer UI state.
- Optional input adapters may default to SFGSS-000's Unity-default action-name compatibility profile. EchoUI core has no hard dependency on the generated `InputSystem_Actions` wrapper, exact bindings/GUIDs, The Will, or project action-map ownership.
- **Runtime implementation has not started at activation.**

## EUI-M3-01 closeout
- Activation: `292cb66` (`292cb66f216ecc130de67e977befccc10e104297`).
- Implementation: `f08c926` (`f08c926478b47e11ab810c9898558ca1f8d0a930`), **31 files / 5956 insertions / 58 deletions**.
- Incoming full EditMode floor was re-established before Runtime edits: **1181 / 1181 passed, 0 failed**.
- Focused EUI-M3-01 EventSystem/focus lifecycle: **24 / 24 passed, 0 failed**.
- Final EchoUI EditMode assembly: **99 / 99 passed, 0 failed**.
- Final Foundry EditMode regression: **1205 / 1205 passed, 0 failed, 0 skipped, 0 inconclusive**.
- Manual Laboratory acceptance: **12 / 12 PASS**.
- Bounded event-driven performance evidence: **PASS**; the idle focus probe remained stable and explicit revalidation stayed bounded.
- Retained M2-02 Modal tab, M2-01 Screen tab, and M1 foundation tab: **PASS**.
- Package/imported Laboratory README, driver, and scene parity: **VERIFIED** at implementation seal.
- The session-memory hotfix corrected implementation to the already-authorized remember-this-session contract and required no authority revision.
- Runtime retains zero hard peer Echo dependency, no generated `InputSystem_Actions` wrapper dependency, and no gameplay-input/action-map ownership.
- Implementation matched **SFGSS-PKG-ECHOUI-001 v1.5.0** under **SFGSS-000 v0.27.0**; no package-authority revision or new suite ADR is required at closeout.
- **EUI-M3-01 is COMPLETE. No follow-on Looking Glass checkpoint is activated by this closeout.**

## EUI-M3-02 declaration

- **Planning baseline:** `0c58240` — clean synchronized EUI-M3-01 final closeout.
- **Authority:** SFGSS-PKG-ECHOUI-001 v1.6.0 under unchanged SFGSS-000 v0.27.0.
- **Incoming full EditMode floor to re-establish before Runtime edits:** **1205 / 1205 passed, 0 failed**.
- Retained EchoUI: **99 / 99**; focused M3-01: **24 / 24**; M3-01 Laboratory: **12 / 12 PASS**; bounded focus performance PASS.
- Transition execution becomes part of admitted Screen/Modal/Window structural lifecycle settlement rather than an unrelated animation race.
- Transition drivers are replaceable presentation-only collaborators and may not own navigation history, Modal semantic meaning, input maps, pause/time scale, scene travel, persistence, audio, gameplay, or project lifetime.
- Every execution owns a fresh awaitable/result plus operation/generation identity. Stale completion cannot overwrite newer lifecycle truth.
- Transition time is unscaled. Cancellation is best-effort, but stale-generation rejection and a hard safety bound are mandatory.
- Enter failure cleans/aborts the incoming entry and restores the prior stable UI; an admitted blocking Modal open failure settles structurally as `Aborted`.
- Exit failure forces deterministic closed/released settlement so a broken animation cannot strand the root.
- Effective transition policy resolves project/root default -> per-definition profile -> transient operation override without mutating authored assets.
- Profiles may independently describe enter/exit driver/timing, optional curve/easing data, timeout/safety bounds, and reduced-motion substitution.
- Package reference drivers are Immediate and CanvasGroup Fade. Professional custom Animator/tween/shader/slide/scale/3D drivers remain first-class without a mandatory tween dependency.
- The transition seam is surface-general, but M3-02 wires only Screen, blocking Modal, and independent Window lifecycle. M4 HUD/transient services are not activated.
- Reduced-motion substitution is architecturally supported; broader Motif/accessibility implementation remains separately gated.
- **Durable authoring promise:** Primitive Warehouse, editable Panel/Menu Template Library, stable-ID Template Catalog, Assembly Utilities, and later Builder/Composer are separate package capabilities. Templates remain ordinary editable prefab compositions, and the lightweight utilities do not depend on the full Builder.
- Primitive/9-slice/template/catalog/utility/Builder implementation is **not** part of M3-02.
- **Runtime implementation has not started at activation.**


---

## EUI-M3-02 FINAL CLOSEOUT COMPLETE

EUI-M3-02 is implemented, automated-green, manually accepted, and sealed at implementation commit `c919238` from activation `ee9d3ffa9c3b2ad4fc8136a70943122f852cca49`.

Final proof is **1246 / 1246** full EditMode, **140 / 140** EchoUI Editor, **21 / 21** transition core, **10 / 10** Screen/Window lifecycle integration, **10 / 10** Modal transition integration, and **14 / 14** Laboratory acceptance.

The Laboratory-discovered synchronous cancellation race is corrected by guarding fallback direct-awaitable cancellation after token-first settlement. No further Runtime feature slice is authorized by this closeout, and no next Looking Glass checkpoint is active.


## EUI-M4-01 declaration

- Baseline `0affb7d`; authority SFGSS-PKG-ECHOUI-001 v1.7.0 under unchanged SFGSS-000 v0.27.0.
- Incoming proof: full EditMode **1246 / 1246**, EchoUI Editor **140 / 140**, Laboratory **14 / 14**.
- Named HUD regions use stable project-defined IDs and bounded registration.
- Widget and visibility ownership use fresh generation-safe idempotent leases.
- Effective visibility resolves deterministically without one caller erasing another caller's live reason.
- HUD remains presentation-only and independent of Screen history, Modal stack, Window state, gameplay input, pause/time scale, cursor, persistence, and domain truth.
- Owner loss and shutdown clean only matching live generations.
- Notifications, prompts, tooltips, Motifs/accessibility, full Window management, authoring libraries/Builder, bridges, integration, and release remain excluded.
- **EUI-M4-01 is COMPLETE / CLOSED. No successor checkpoint is active.**

## EUI-M4-01 FINAL CLOSEOUT COMPLETE

- Activation: `ce30ac6`.
- Retained-floor timing stabilization: `dbdf6bd`.
- Runtime and focused tests: `df9e2be`.
- Bounded compile/test corrections: `81f9625`, `3992bbc`, `e47d43b`.
- Laboratory implementation seal: `29573ef`.
- The requested focused/full automated gate is user-confirmed green. Exact post-M4 NUnit totals were not captured; `1246 / 1246` remains the retained pre-M4 floor.
- Manual HUD Laboratory: **5 / 5 PASS**, including two named SceneOwned regions and three widgets, overlapping out-of-order visibility releases, owner-loss/stale-generation safety, duplicate/capacity rejection without mutation, and 180-frame idle quiescence.
- Jesse's final `green` confirms retained M3-02/M3-01/M2-02/M2-01/M1 smoke; exact per-tab observation strings were not separately supplied.
- Package/imported Laboratory README, driver, and scene parity: **VERIFIED**.
- Runtime retains zero hard peer Echo dependency and claims no gameplay-input, pause/time-scale, cursor, persistence, domain-truth, or project-lifetime authority.
- Package authority remains SFGSS-PKG-ECHOUI-001 v1.7.0 under SFGSS-000 v0.27.0; no new ADR is required.
- **Historical EUI-M4-01 stop point:** EUI-M4-02 was inactive at closeout; the later EUI-M4-02 declaration below supersedes that status.

## EUI-M4-02 declaration

- Baseline `5e7ad92`; authority SFGSS-PKG-ECHOUI-001 v1.8.0 under unchanged SFGSS-000 v0.27.0.
- Retained EUI-M4-01 proof: automated focused/full gate user-confirmed green; manual HUD Laboratory **5 / 5 PASS**; retained prior-tab smoke green; package/imported parity verified.
- Exact post-M4 NUnit totals were not captured. The first EUI-M4-02 gate records the current EchoUI/full EditMode baseline before Runtime edits; retained `1246 / 1246` remains pre-M4 history.
- Channels are project-defined, variable-count, stable-ID-addressed, and independently bounded.
- Higher priority promotes first; equal priority is FIFO; visible entries do not preempt.
- Coalescing is opt-in and channel-scoped. Replacement uses a fresh generation, restarts lifetime by default, and makes the old handle stale.
- Pending overflow defaults to `RejectNewest`, with authored `DropOldestPending` and strict-outrank `ReplaceLowestPriorityPending` alternatives.
- Automatic lifetime uses unscaled monotonic time; manual lifetime requires explicit/structural dismissal.
- Notifications remain transient presentation and cannot own durable history, localization/audio/analytics, gameplay/domain truth, input, pause/time scale, cursor, persistence, or project lifetime.
- Prompts, tooltips, Motifs/accessibility implementation, safe area, full Window management, authoring libraries/Builder, bridges, integration, and release remain excluded.
- **Activation baseline:** full Foundry EditMode **1258 / 1258**; EchoUI Editor **152 / 152**; zero failed before Runtime edits.
- **Implemented through `d93d0bd`:** contracts, bounded channel state, admission/promotion, coalescing, overflow, unscaled/manual lifetime, cleanup, status/events, root integration, and the replaceable presenter seam.
- **Final accepted automated evidence:** full Foundry EditMode **1383 / 1383**; EchoUI Editor **277 / 277**; aggregate notification fixtures **125 / 125**; presenter fixture **17 / 17**; zero failed/skipped/inconclusive.
- **Laboratory implementation:** `bde34f2` adds mirrored three-channel scene configuration, sample-owned plain presenter, checks 1-6, retained tabs, and package/imported parity.

## EUI-M4-02 FINAL CLOSEOUT COMPLETE

- Activation: `fd8256f`; Runtime/root/presenter implementation accepted through `d93d0bd`; Laboratory implementation `bde34f2`.
- Manual notification Laboratory: **6 / 6 PASS** with baseline ready and unchanged Screen/Modal/Window/HUD/transition truth.
- Check 1 proved independent channels, bounded visible state, no visible preemption, priority/FIFO promotion, and secondary-channel independence.
- Check 2 proved visible/pending coalescing, fresh generations, stale old handles, pending promotion, visible lifetime restart, and expiry.
- Check 3 proved all three overflow policies with exact bounds and no unrelated mutation.
- Check 4 proved unscaled automatic expiry at `Time.timeScale == 0`, manual retention/dismissal, exact time-scale restoration, and unchanged structural truth.
- Check 5 proved owner/presentation loss, deterministic promotion, stale-generation safety, exact three-entry reset, fresh post-reset generation, and final empty baseline.
- Check 6 proved 180-frame quiescence: channels `3 -> 3`, visible `0 -> 0`, pending `0 -> 0`, presenter-visible `0 -> 0`, presenter apply count `50 -> 50`, stable snapshots, and unchanged structural truth.
- Retained M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke: user-confirmed green; exact per-tab strings were not separately supplied.
- Submitted Unity screenshots show the sample-owned presenter, Check 1 PASS, zero Console errors/warnings, and the retained **1383-test** green runner.
- Package/imported Laboratory parity: **VERIFIED**.
- Runtime retains zero hard peer Echo dependency and claims no gameplay-input, pause/time-scale, cursor, persistence, domain-truth, localization/audio/analytics, or project-lifetime authority.
- **EUI-M4-02 is COMPLETE / CLOSED.** Its no-successor stop point was satisfied before the separate EUI-M4-03 activation recorded above.
