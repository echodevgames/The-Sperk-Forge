# EUI-M4-03 — Looking Glass Runtime Motif Definitions, Registered Targets, Fallback, and Immutable Application

**Package:** The Looking Glass (`EchoUI`)
**Milestone:** M4 — Complete MVP Surfaces
**Status:** ACTIVE / AUTHORIZED
**Activation baseline:** `2f592513b6215b019ca9550fb302ab0cee6b65cc`
**Suite authority:** SFGSS-000 v0.27.0
**Package authority:** SFGSS-PKG-ECHOUI-001 v1.9.0
**Workflow authority:** SFGSS-005 v1.7.0
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0
**Activated:** August 17, 2026

## 1. Purpose and observable outcome

Implement the smallest independently useful runtime Motif slice: immutable project-owned Motif definitions with stable IDs and typed appearance tokens; one root-local effective Motif; explicit registered targets; deterministic initial application, switching, fallback, and last-known-good behavior; minimal local-value preservation; structured reports/events; and generation-safe target cleanup.

The Laboratory must visibly prove that two neutral project Motifs can restyle participating uGUI presentation without changing layout, navigation, content, or domain behavior; individual bindings may preserve authored local values; unknown Motif IDs and missing tokens fail safely through explicit fallback/reporting; one broken or destroyed target cannot roll back committed Motif truth or prevent healthy targets from updating; authored Motif assets remain unchanged; and settled Motif state performs no recurring scan or application work.

## 2. Starting conditions

- Repository and origin `main` are exactly at EUI-M4-02 documentation closeout `2f59251`.
- EUI-M4-02 Runtime/root/presenter implementation is accepted through `d93d0bd`; mirrored Laboratory implementation is `bde34f2`.
- Final accepted automated evidence is full Foundry EditMode **1383 / 1383**, EchoUI Editor **277 / 277**, aggregate notification fixtures **125 / 125**, and presenter fixture **17 / 17**, with zero failed/skipped/inconclusive.
- EUI-M4-02 manual Laboratory is **6 / 6 PASS**, retained M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke is user-confirmed green, and package/imported parity is verified.
- PKG-LEARN-008 is complete through the bounded EUI-M4-03 JIT revisit.
- Runtime package remains `0.1.0`; Unity dependency remains only `com.unity.ugui` `2.0.0`.
- No peer Echo package or TextMeshPro package is required.

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
- An unknown requested Motif ID applies the authored fallback and reports `FallbackApplied` without rewriting the caller's external preference/source ID.
- If neither the requested Motif nor fallback is valid, the service retains the last known-good effective appearance and reports unavailable without partial definition commit.
- Once a valid effective Motif is committed, one target exception/failure is isolated. Healthy targets continue, the committed effective Motif remains authoritative, and the report identifies partial failure rather than pretending arbitrary project views can roll back atomically.
- Reapplying unchanged truth is bounded and does not create a per-frame update loop.
- Runtime selection and reports are session state only; authored definitions never become mutable runtime stores.

### 3.3 Authorized slice

EUI-M4-03 owns only Motif/token identity and immutable definitions, deterministic resolution snapshots, default/effective/fallback state, explicit target registration, minimal inherit-versus-local binding policy, application/failure reporting, root integration, focused tests, and Laboratory proof.

## 4. Authority and invariants

1. Motif and token identity are normalized stable IDs and collision checked.
2. Motif definitions and their token payloads remain immutable during play, reset, repeated application, and shutdown.
3. The service resolves a detached runtime snapshot before applying project targets.
4. The root owns only effective session presentation state; external project/Accord authority owns durable preference selection.
5. Registration is explicit and bounded; no scene-wide or per-frame hierarchy scan is permitted.
6. Newly registered targets receive current effective truth immediately when available.
7. Local-preserve bindings remain unchanged across default application, switching, fallback, and reapplication.
8. Unknown requested Motif IDs do not rewrite external data and resolve through the authored fallback when valid.
9. Missing tokens never cause a null crash; targets receive explicit unavailable/fallback truth and preserve their safe prior/local presentation.
10. A valid effective Motif commits before project-target notification. Target failure cannot roll back committed service truth or block healthy targets.
11. Listener failure is isolated after committed truth and cannot disable the service.
12. Registration handles are generation-safe and idempotent; a stale handle cannot remove a newer live registration.
13. Destroyed owner/target cleanup affects only the matching live registration.
14. Reset restores the authored default/fallback contract with a fresh application generation; shutdown releases registrations and rejects new work.
15. Status does not retain hierarchy paths, visible text, typed input, arbitrary project payloads, or production asset names beyond approved stable IDs.
16. Motif mutation never changes Screen history, Modal order/results, Window state, HUD/notification truth, focus, transitions, gameplay, input, pause, cursor, persistence, or project lifetime authority.
17. Capacity, status, and diagnostic history remain bounded.
18. Settled Motif state performs no recurring package-owned application, polling, or allocation work.
19. Registered-target capacity is explicitly configured and positive; duplicate/full registration rejects without mutation.
20. Missing Motif configuration leaves only the Motif capability unavailable/degraded and cannot prevent retained root, Screen, Modal, Window, HUD, notification, focus, or transition initialization.

## 5. Runtime scope

Exact public names may receive compile-safe refinement without changing authority:

- `UIMotifId` and `UIMotifTokenId` stable value types.
- Immutable project-owned ScriptableObject `UIMotifDefinition` with stable Motif ID and bounded typed tokens.
- Color, uGUI `Selectable` color-state, sprite, and numeric/decorative token values.
- Detached read-only `UIMotifSnapshot` / resolved token lookup contract.
- Minimal `UIMotifBindingMode` or equivalent `UseMotif` / `KeepLocal` policy consumed by targets.
- `IUIMotifTarget` application seam plus fresh generation-safe registration handle/lease.
- Structured resolution, registration, application, fallback, partial-failure, stale, reset, and shutdown results.
- Root-owned `UIMotifService` with authored definition catalog, default Motif, fallback Motif, configured registered-target capacity, effective state, and monotonic application generation.
- Explicit registration with optional Unity owner for destroyed-owner/target cleanup.
- Apply-by-stable-ID, deterministic fallback, no-change/reapply behavior, reset, shutdown, post-commit events, and side-effect-free snapshots.
- Root initialization and public facade integration without changing package lifetime composition.
- Sample-owned uGUI reference targets; production projects may implement custom targets without replacing Motif authority.

## 6. Automated proof

At minimum prove valid stable Motif/token construction and normalization; invalid/empty/duplicate Motif and token rejection without partial state; detached immutable snapshots; deterministic typed token lookup; default initialization; missing configuration degrades only Motif capability; explicit switching; immediate application to late registrations; duplicate/capacity rejection without mutation; inherit-versus-local preservation; unknown Motif fallback without external-ID mutation; missing-token safety; no-valid-fallback last-known-good preservation; healthy-target continuation after target exception/failure; listener isolation; fresh generation and stale registration-handle rejection; idempotent unregister; destroyed owner/target cleanup; reset/shutdown; root facade/state integration; repeated application without ScriptableObject mutation; no Screen/Modal/Window/HUD/notification/focus/transition mutation; bounded idle state; and full retained EchoUI regression.

## 7. Laboratory proof

Add an **M4-03 Motifs** tab before retained tabs. Use two neutral sample-owned Motifs and plain uGUI target adapters to demonstrate:

1. authored default Motif application across multiple registered targets and token families;
2. live switching to a second Motif while one explicitly local binding remains unchanged;
3. missing-token behavior with safe prior/local presentation and actionable result truth;
4. unknown requested Motif ID resolving to the authored fallback without rewriting caller input;
5. one failing/destroyed target, stale registration release, reset, fresh generation, and final default baseline;
6. authored asset immutability plus 180-frame idle quiescence and retained M4-02/M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke.

The sample controls only sample appearance. It does not establish production art, global settings, accessibility preference, or persistence authority.

## 8. Explicit exclusions

This checkpoint does not authorize full accessibility policy; text scaling; contrast-selection policy; focus-indicator policy; automatic connection of reduced-motion transition behavior; transient timing accessibility policy; safe-area adaptation; Accord/settings persistence; Editor Motif create/capture/apply/preview workflows; final warehouse-facing typography/provider schema; mandatory TextMeshPro dependency; Primitive Warehouse; 9-slice prefab families; Panel/Menu templates; Template Catalog; Assembly Utilities; Builder/Composer; automatic hierarchy scanning; edit-time destructive prefab restyling; prompts; tooltips; richer Window management; generalized dim/blur; new transition drivers; durable history; localization/audio/analytics; domain authority; peer bridges; project-wide lifetime composition; pooling; showcase art; integration; clean-project reproduction; or release qualification.

## 9. Expected files

The implementation is expected to create or modify only this manifest, subject to compile-safe consolidation inside the named folders:

- `Packages/com.echodevgames.echo-ui/Runtime/Motifs.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/UIMotifId.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/UIMotifTokenId.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/UIMotifTokens.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/UIMotifDefinition.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/UIMotifSnapshot.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/IUIMotifTarget.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/UIMotifRegistration.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/UIMotifResults.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Motifs/UIMotifService.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Runtime/Core/EchoUIRoot.cs`
- `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIMotifContractTests.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIMotifServiceTests.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIMotifRootIntegrationTests.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Samples~/LookingGlass_UI_Foundation_Laboratory/Runtime/LaboratoryMotifTarget.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Samples~/LookingGlass_UI_Foundation_Laboratory/Runtime/LaboratoryMotifProof.cs` and `.meta`
- `Packages/com.echodevgames.echo-ui/Samples~/LookingGlass_UI_Foundation_Laboratory/Scenes/The Looking Glass_UI_Laboratory.unity`
- `Packages/com.echodevgames.echo-ui/Samples~/LookingGlass_UI_Foundation_Laboratory/README.md`
- `Assets/Samples/The Looking Glass — UI Framework/0.1.0/Looking Glass UI Foundation Laboratory/Runtime/LaboratoryMotifTarget.cs` and `.meta`
- `Assets/Samples/The Looking Glass — UI Framework/0.1.0/Looking Glass UI Foundation Laboratory/Runtime/LaboratoryMotifProof.cs` and `.meta`
- `Assets/Samples/The Looking Glass — UI Framework/0.1.0/Looking Glass UI Foundation Laboratory/Scenes/The Looking Glass_UI_Laboratory.unity`
- `Assets/Samples/The Looking Glass — UI Framework/0.1.0/Looking Glass UI Foundation Laboratory/README.md`
- `Plan Documentation/Checkpoint Build Plans/EUI-M4-03_Looking_Glass_Runtime_Motif_Definitions_Registered_Targets_Fallback_and_Immutable_Application_Checkpoint_Build_Plan.md`
- `Plan Documentation/Current Notes.md`
- `Plan Documentation/Learning Reviews/PKG-LEARN-008_EchoUI_Learning_Review.md`
- `Plan Documentation/Learning Reviews/PKG-LEARN-TRACKER.json`
- `Plan Documentation/Package Specifications/SFGSS-The-Looking-Glass-EchoUI-Package-Specification.md`
- `Plan Documentation/Suite_Graph_Roadmap.md`
- `Plan Documentation/Suite_Health_Check_and_Remaining_Documentation.md`
- `Packages/com.echodevgames.echo-ui/Documentation~/Developer/Current Notes.md`
- `Packages/com.echodevgames.echo-ui/README.md`
- `Packages/com.echodevgames.echo-ui/CHANGELOG.md`

Every new Unity asset requires its committed `.meta`. New Runtime dependencies, asmdefs, packages, or unlisted production assets are not authorized.

## 10. Implementation sequence

1. On the activation commit, run EchoUI Editor and full Foundry EditMode suites before Runtime edits; re-establish **277 / 277** EchoUI and **1383 / 1383** full or stop on any divergence.
2. Add pure stable IDs, typed tokens, immutable definitions, detached snapshots, and structured results.
3. Add definition validation/catalog, authored default/fallback resolution, last-known-good behavior, and bounded status.
4. Add explicit target registration, immediate current-snapshot delivery, local-preserve policy, generation-safe handles, and destroyed-owner/target cleanup.
5. Add deterministic apply/switch/reapply/reset/shutdown, target/listener isolation, and post-commit events.
6. Integrate the root facade/default initialization without changing existing service authority.
7. Add focused tests; run EchoUI and full EditMode suites.
8. Extend the mirrored Laboratory while preserving retained tabs and package/imported parity.
9. Obtain manual acceptance.
10. Seal implementation, reconcile documentation, commit, and push.

## 11. Validation gates

- Activation-baseline EchoUI **277 / 277** and full Foundry EditMode **1383 / 1383** re-established with zero failures before Runtime edits
- Focused EUI-M4-03 tests all pass
- EchoUI Editor all pass
- Full Foundry EditMode all pass with zero failed/skipped/inconclusive
- `git diff --check` and cached equivalent clean
- Runtime remains dependent only on `com.unity.ugui` `2.0.0`
- Motif assets remain unchanged across runtime application/reset
- Package/imported Laboratory parity verified
- Manual Motif Laboratory all checks pass
- Retained M4-02/M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke passes
- 180-frame settled Motif quiescence passes
- Repository clean and origin synchronized at closeout

## 12. Failure and bounded fixes

Invalid/duplicate definitions or tokens reject before authoritative service state changes. Unknown requested IDs use the valid authored fallback. If no fallback exists, retain last known-good truth. Target and listener failures are isolated after valid Motif commit and reported without fabricated rollback. Missing tokens preserve safe target presentation. Stale handles preserve newer registrations. Compile/test corrections inside this contract are pre-approved; any need for a new dependency, public ownership change, serialized migration, or wider authoring schema stops the Green Path.

## 13. Rollback

Before implementation sealing, restore only the EUI-M4-03 manifest if a red gate cannot be resolved inside scope. Do not rewrite retained EUI-M1 through EUI-M4-02 history. No destructive project/prefab migration is authorized.

## 14. Documentation reconciliation

At closeout update this plan, package specification, PKG-LEARN-008/tracker, suite/package Current Notes, Suite Graph Roadmap, Suite Health, package README/changelog, and Laboratory evidence. Preserve exact hashes, test counts, dependency state, and missing-evidence qualifications.

## 15. Commit plan

- Activation: `Activate EUI-M4-03 runtime Motif foundation`
- Runtime/tests: `Implement EUI-M4-03 runtime Motif foundation`
- Laboratory: `Add EUI-M4-03 Motif Laboratory proof`
- Closeout: `Close out EUI-M4-03 runtime Motif foundation`

## 16. Completion criteria and stop point

EUI-M4-03 completes only after exact incoming/final automated evidence, manual proof, retained regression, truthful documentation, clean repository, and push. Stop there. Do not begin accessibility policy, Motif authoring tools, Primitive Warehouse, templates/catalog/utilities, Builder, prompts/tooltips, safe area, Window management, persistence, bridges, integration, or release work without separate activation.

## 17. Named next direction

The Primitive Warehouse remains the named next program direction after the runtime Motif foundation closes, but this activation does not assign or authorize a successor checkpoint ID. The final warehouse-facing Motif capture/preview/local-override authoring format may be revisited only when real primitive authoring exposes the smallest useful contract.

## 18. Handoff

EUI-M4-03 is **ACTIVE / AUTHORIZED** from clean EUI-M4-02 closeout `2f59251` under package authority v1.9.0. Runtime implementation has not started. The first gate is to re-establish EchoUI Editor **277 / 277** and full Foundry EditMode **1383 / 1383** on the activation commit before touching Runtime.

The slice is limited to immutable project-owned Motif definitions/tokens, detached snapshots, one root-local effective/default/fallback Motif, explicit registered targets, inherit-versus-local binding preservation, structured fallback/failure truth, target/listener isolation, generation-safe cleanup, tests, and Laboratory proof. Accessibility policy, persistence, Editor capture/apply tooling, final Primitive Warehouse-facing schema, authoring libraries/Builder, prompts/tooltips, safe area, richer Window management, bridges, integration, and release remain inactive.
