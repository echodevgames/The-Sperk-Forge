# EUI-M4-03 — Looking Glass Runtime Motif Definitions, Registered Targets, Fallback, and Immutable Application

**Package:** The Looking Glass (`EchoUI`)
**Milestone:** M4 — Complete MVP Surfaces
**Status:** COMPLETE / CLOSED
**Activation baseline:** `2f592513b6215b019ca9550fb302ab0cee6b65cc`
**Suite authority:** SFGSS-000 v0.27.0
**Package authority:** SFGSS-PKG-ECHOUI-001 v1.9.0
**Workflow authority:** SFGSS-005 v1.7.0
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0
**Activated:** August 17, 2026
**Closed:** August 18, 2026

## 1. Purpose and observable outcome

Implement the smallest independently useful runtime Motif slice: immutable project-owned Motif definitions with stable IDs and typed appearance tokens; one root-local effective Motif; explicit registered targets; deterministic initial application, switching, fallback, and last-known-good behavior; minimal local-value preservation; structured reports/events; and generation-safe target cleanup.

The Laboratory must visibly prove that two neutral project Motifs can restyle participating uGUI presentation without changing layout, navigation, content, or domain behavior; individual bindings may preserve authored local values; unknown Motif IDs and missing tokens fail safely through explicit fallback/reporting; one broken or destroyed target cannot roll back committed Motif truth or prevent healthy targets from updating; authored Motif assets remain unchanged; and settled Motif state performs no recurring scan or application work.

## 2. Starting conditions

- Repository and origin `main` began this checkpoint at EUI-M4-02 documentation closeout `2f59251`.
- EUI-M4-02 Runtime/root/presenter implementation was accepted through `d93d0bd`; mirrored Laboratory implementation was `bde34f2`.
- Incoming automated evidence was full Foundry EditMode **1383 / 1383**, EchoUI Editor **277 / 277**, aggregate notification fixtures **125 / 125**, and presenter fixture **17 / 17**, with zero failed/skipped/inconclusive.
- EUI-M4-02 manual Laboratory was **6 / 6 PASS**, retained M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke was user-confirmed green, and package/imported parity was verified.
- PKG-LEARN-008 was complete through the bounded EUI-M4-03 JIT revisit.
- Runtime package remained `0.1.0`; Unity dependency remained only `com.unity.ugui` `2.0.0`.
- No peer Echo package or TextMeshPro package was required.

## 3. Learn → Declare → Authorize reconciliation

### 3.1 Learned boundary

A Motif is a reusable appearance recipe, not a prefab factory, layout system, navigation graph, domain presenter, settings store, accessibility authority, or production art mandate. Motif definitions are project-owned authored assets. Looking Glass may resolve and apply their appearance tokens, retain one effective session selection, and report application truth, but it does not persist preference IDs or mutate shared authored assets during play.

### 3.2 Declared intent

- Motifs are project-defined, stable-ID-addressed, immutable definitions.
- The first typed token families are colors, uGUI `Selectable` color-state recipes, sprites, and small numeric/decorative values understood by a registered target.
- Typography-provider integration, final warehouse-facing token schema, and full capture/preview tooling remain additive later work; M4-03 adds no mandatory TextMeshPro dependency.
- The root owns one effective Motif for its current session and applies an authored default during initialization.
- Targets register explicitly; Looking Glass does not scan scenes or hierarchies automatically.
- A newly registered target immediately receives the current resolved Motif snapshot.
- Target bindings may choose Motif inheritance or preserve an authored local value. Motif reapplication cannot silently overwrite a binding marked local.
- An unknown requested Motif ID applies the authored fallback and reports fallback truth without rewriting the caller's external preference/source ID.
- If neither the requested Motif nor fallback is valid, the service retains the last known-good effective appearance and reports unavailable without partial definition commit.
- Once a valid effective Motif is committed, one target exception/failure is isolated. Healthy targets continue, the committed effective Motif remains authoritative, and the report identifies partial failure rather than pretending arbitrary project views can roll back atomically.
- Reapplying unchanged truth is bounded and does not create a per-frame update loop.
- Runtime selection and reports are session state only; authored definitions never become mutable runtime stores.

### 3.3 Authorized slice

EUI-M4-03 owns only Motif/token identity and immutable definitions, deterministic resolution snapshots, default/effective/fallback state, explicit target registration, minimal inherit-versus-local binding policy, application/failure reporting, root integration, focused tests, and Laboratory proof.

## 4. Authority and invariants

1. Motif and token identity are normalized stable IDs and collision checked.
2. Motif definitions and token payloads remain immutable during play, reset, repeated application, and shutdown.
3. The service resolves a detached runtime snapshot before applying project targets.
4. The root owns only effective session presentation state; external project/Accord authority owns durable preference selection.
5. Registration is explicit and bounded; no scene-wide or per-frame hierarchy scan is permitted.
6. Newly registered targets receive current effective truth immediately when available.
7. Local-preserve bindings remain unchanged across default application, switching, fallback, and reapplication.
8. Unknown requested Motif IDs do not rewrite external data and resolve through the authored fallback when valid.
9. Missing tokens never cause a null crash; targets receive explicit result truth and preserve safe prior/local presentation.
10. A valid effective Motif commits before project-target notification. Target failure cannot roll back committed service truth or block healthy targets.
11. Listener failure is isolated after committed truth and cannot disable the service.
12. Registration handles are generation-safe and idempotent; a stale handle cannot remove a newer live registration.
13. Destroyed owner/target cleanup affects only the matching live registration.
14. Reset restores the authored default/fallback contract; shutdown releases registrations and rejects new work.
15. Status does not retain hierarchy paths, visible text, typed input, arbitrary project payloads, or production asset names beyond approved stable IDs.
16. Motif mutation never changes Screen history, Modal order/results, Window state, HUD/notification truth, focus, transitions, gameplay, input, pause, cursor, persistence, or project lifetime authority.
17. Capacity, status, and diagnostic history remain bounded.
18. Settled Motif state performs no recurring package-owned application, polling, or allocation work.
19. Registered-target capacity is explicitly configured and positive; duplicate/full registration rejects without mutation.
20. Missing Motif configuration leaves only the Motif capability unavailable/degraded and cannot prevent retained root, Screen, Modal, Window, HUD, notification, focus, or transition initialization.

## 5. Implemented Runtime scope

The completed Runtime slice includes normalized `UIMotifId` / `UIMotifTokenId`; typed color, Selectable-state, sprite, and numeric tokens; immutable transient/authored definitions and detached snapshots; catalog/default/fallback resolution; `IUIMotifTarget`; explicit generation-safe registrations; reusable binding helpers with `UseMotif` / `KeepLocal`; root-local `UIMotifService`; deterministic switch/reset/shutdown; target/listener isolation; destroyed-target pruning; and `EchoUIRoot` facade integration.

Runtime implementation chain:

- activation `435fc66`;
- contracts `d67550d`;
- catalog/fallback `172d230`;
- session service `43da17a`;
- registered targets `efbc503`;
- reusable bindings `e17d816`;
- root integration `ab5906c`;
- root EditMode teardown fixture correction `d291885` with no Runtime source change.

## 6. Automated proof

The automated suite proves stable-ID construction/normalization; invalid/duplicate rejection; detached immutable snapshots; typed lookup; default initialization; missing configuration degradation; explicit switching; late registration; bounded/duplicate registration behavior; inherit-versus-local preservation; fallback; missing-token safety; last-known-good behavior; target/listener isolation; fresh generation/stale handle safety; idempotent release; destroyed owner/target cleanup; reset/shutdown; root facade/state integration; authored ScriptableObject immutability; retained Screen/Modal/Window/HUD/notification/focus/transition truth; and bounded idle state.

### Final accepted automated evidence — August 18, 2026

`TestResults_20260818_060619.xml` records:

- full Foundry EditMode **1445 / 1445 passed**;
- EchoUI Editor **339 / 339 passed**;
- aggregate Motif fixtures **62 / 62 passed**;
- `EchoUIMotifRootIntegrationTests` **12 / 12 passed**;
- failed / skipped / inconclusive **0 / 0 / 0**.

The earlier first root-integration run was **1444 / 1445** with only `RootDestructionShutsDownMotifRegistrations` failing because the EditMode fixture destroyed the GameObject without explicitly invoking the private root teardown path used by established root tests. `d291885` corrected only that fixture by invoking `OnDestroy()` before destruction. Runtime source did not change. The repaired test and complete suite are green in the final evidence above.

Intentional `motif-observer`, `motif-target`, and `root-motif-observer` exception logs remain passing isolation proofs.

## 7. Laboratory proof

The mirrored Laboratory adds a dedicated M4-03 Motif proof console while retaining M4-02 through M1 proof surfaces. Two neutral transient sample Motifs and plain uGUI target adapters prove:

1. authored default application across multiple targets and all initial token families;
2. live switching while an explicit `KeepLocal` binding remains unchanged;
3. missing-token partial application with safe prior presentation;
4. unknown requested ID fallback without caller-ID rewriting;
5. failing/destroyed target isolation, stale release, fresh generation, reset, and final default baseline;
6. authored-asset immutability plus **180-frame idle quiescence**.

Laboratory implementation and correction chain:

- `b48eae68` — initial mirrored Motif Laboratory proof;
- `7f9272bd` — Check 3 assertion correction: a `Partial` target application is successful registration truth and therefore reports `Registered` while retaining explicit partial/failure counts;
- `8188b91c` — Check 4 sequencing correction: Reset to authored default before unknown-ID request so the proof exercises an actual `FallbackApplied` transition rather than a legitimate same-effective `Unchanged` result.

Check 5 intentionally injects one broken target. The failing target is applied once during immediate registration and again during the subsequent switch, so the proof intentionally emits **two** target exception logs. Both are caught and isolated by Runtime and are accepted evidence, not failures.

### Final manual acceptance — August 18, 2026

- Check 1 Default + Typed Tokens: **PASS**.
- Check 2 Switch + Keep Local: **PASS**.
- Check 3 Missing Token Safety: **PASS**.
- Check 4 Unknown ID Fallback: **PASS**.
- Check 5 Failure + Stale + Reset: **PASS** with the two intentional target-isolation exceptions described above.
- Check 6 180-Frame Idle: **PASS**.
- Retained M4-02/M4-01/M3-02/M3-01/M2-02/M2-01/M1 representative smoke: **user-confirmed green**; exact per-tab observation strings were not separately supplied.
- Package/imported Motif proof source parity: **VERIFIED** at closeout.

The sample controls only sample appearance. It does not establish production art, global settings, accessibility preference, persistence authority, or final authoring schema.

## 8. Explicit exclusions retained at closeout

This checkpoint does not authorize full accessibility policy; text scaling; contrast-selection policy; focus-indicator policy; automatic connection of reduced-motion transition behavior; transient timing accessibility policy; safe-area adaptation; Accord/settings persistence; Editor Motif create/capture/apply/preview workflows; final warehouse-facing typography/provider schema; mandatory TextMeshPro dependency; Primitive Warehouse; 9-slice prefab families; Panel/Menu templates; Template Catalog; Assembly Utilities; Builder/Composer; automatic hierarchy scanning; edit-time destructive prefab restyling; prompts; tooltips; richer Window management; generalized dim/blur; new transition drivers; durable history; localization/audio/analytics; domain authority; peer bridges; project-wide lifetime composition; pooling; showcase art; integration; clean-project reproduction; or release qualification.

## 9. Dependency and packaging result

- Runtime package remains `0.1.0`.
- Core dependency remains only `com.unity.ugui` `2.0.0` on the recorded Unity 6000.3.8f1 baseline.
- No hard peer Echo dependency was added.
- No mandatory TextMeshPro package dependency was added by Motifs.
- New Laboratory C# assets retain committed `.meta` identity; closeout changes no GUIDs.

## 10. Completion gates

- [x] Activation-baseline EchoUI **277 / 277** and full Foundry **1383 / 1383** re-established before Runtime edits.
- [x] Focused EUI-M4-03 tests pass.
- [x] EchoUI Editor **339 / 339** passes.
- [x] Full Foundry EditMode **1445 / 1445** passes with zero failed/skipped/inconclusive.
- [x] Motif aggregate **62 / 62** and root **12 / 12** pass.
- [x] Runtime dependency boundary remains unchanged.
- [x] Motif assets remain unchanged across runtime application/reset.
- [x] Package/imported Laboratory Motif proof parity verified.
- [x] Manual Motif Laboratory **6 / 6 PASS**.
- [x] Retained M4-02/M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke user-confirmed green.
- [x] 180-frame settled Motif quiescence passes.
- [x] Documentation reconciled to accepted evidence.

## 11. Failure and bounded-fix record

The checkpoint preserved the declared Runtime contract. The two Laboratory-owned corrections changed proof expectations/sequencing only:

- Check 3 originally expected `RegisteredWithApplyFailure` even though the authoritative target result was `Partial`, whose `Succeeded` truth correctly yields `Registered`.
- Check 4 originally requested the unknown ID while the authored fallback was already effective, so Runtime correctly returned `Unchanged`; the proof now resets first and forces a real fallback transition.

Neither correction changed Runtime behavior or authority.

A publication-tooling mistake created a transient root file `__nope__` in intermediate commit `2b1e6cf`; the immediately following accepted `8188b91c` tree removed it through a non-forced fast-forward. The current tree contains no such file. The harmless intermediate commit remains in history because `main` was not rewritten.

## 12. Documentation reconciliation

Closeout reconciles the active plan, package specification status, PKG-LEARN-008/tracker, suite/package Current Notes, Suite Graph Roadmap, Suite Health, package README/changelog, and Laboratory wording/evidence. Exact final automated totals, manual evidence, dependency state, correction history, and evidence qualifications are retained.

## 13. Commit plan

- Activation: `Activate EUI-M4-03 runtime Motif foundation`.
- Runtime/tests: bounded implementation commits listed in Section 5.
- Laboratory: `Add EUI-M4-03 Motif Laboratory proof` plus bounded proof corrections listed in Section 7.
- Closeout: `Close out EUI-M4-03 runtime Motif foundation`.

## 14. Completion and stop point

**EUI-M4-03 is COMPLETE / CLOSED.**

No successor Looking Glass checkpoint is activated by this closeout. Do not begin accessibility policy, Motif authoring tools, Primitive Warehouse, templates/catalog/utilities, Builder, prompts/tooltips, safe area, Window management, persistence, bridges, integration, or release work without separate Learn → Declare → Authorize activation.

## 15. Named next direction

The **Primitive Warehouse** remains the named next Looking Glass program direction after the Runtime Motif foundation. This closeout does not assign or authorize a successor checkpoint ID. The final warehouse-facing Motif capture/preview/local-override authoring format may be revisited only when real primitive authoring exposes the smallest useful contract.

## 16. Handoff

EUI-M4-03 is closed under SFGSS-PKG-ECHOUI-001 v1.9.0. Final accepted evidence is full Foundry **1445 / 1445**, EchoUI **339 / 339**, Motifs **62 / 62**, root **12 / 12**, zero failed/skipped/inconclusive; manual Motif Laboratory **6 / 6 PASS**; 180-frame idle quiescence PASS; retained M4-02 through M1 smoke user-confirmed green; package/imported Motif proof parity verified.

**Exact resume phase:** no Looking Glass implementation checkpoint is active. Rehydrate from live `main`, confirm this closeout, then perform a new bounded JIT review/activation before any successor work. Primitive Warehouse is the named direction only.
