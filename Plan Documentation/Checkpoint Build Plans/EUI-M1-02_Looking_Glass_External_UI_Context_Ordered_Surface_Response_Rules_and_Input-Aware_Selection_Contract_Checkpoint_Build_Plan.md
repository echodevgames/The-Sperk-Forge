---
tags:
  - sfgss/checkpoint
  - sfgss/wave/foundation
  - sfgss/ui
status: active
checkpoint: EUI-M1-02
updated: 2026-08-13
---

# EUI-M1-02 — The Looking Glass External UI Context, Ordered Surface Response Rules, and Input-Aware Selection Contract — Checkpoint Build Plan

**Checkpoint:** `EUI-M1-02`
**Status:** **ACTIVE / AUTHORIZED**
**Package:** The Looking Glass (`EchoUI`)
**Package ID:** `com.echodevgames.echo-ui`
**Package authority:** `SFGSS-PKG-ECHOUI-001` v1.2.0 Approved
**Workflow authority:** SFGSS-005 v1.6.0 + SFGSS-ADR-007 Green Path
**Learning gate:** PKG-LEARN-008 Complete, including bounded EUI-M1-02 revisit
**Starting repository baseline:** `57a4fa4` — EUI-M1-01 final recovery
**Incoming retained evidence:** full EditMode **1113 / 1113 passed, 0 failed**; manual Laboratory **5 / 5**
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0
**Activation authority:** Jesse “Echo” Adams explicitly authorized EUI-M1-02 on August 13, 2026 after Learn → Declare → Authorize. The Git commit containing this ACTIVE / AUTHORIZED transition is the durable activation commit and should be recorded by short hash at implementation closeout.

> This checkpoint turns externally supplied UI conditions into designer-controlled surface behavior without turning Looking Glass into game-state, input, persistence, or project-lifetime authority.

## 1. Observable outcome

Starting from the proven EUI-M1-01 surface/navigation foundation, a registered `UISurface` can consume externally supplied active/inactive context IDs and externally supplied input modality, resolve designer-authored ordered response rules, and apply only the UI dimensions those rules explicitly control.

The checkpoint must prove all of the following together:

- project-defined stable UI context IDs;
- multiple simultaneous active contexts;
- per-surface opt-in/opt-out from automatic external-context response;
- designer-controlled ordered rule precedence;
- independent response dimensions for visibility, interaction, and selection/focus intent;
- no intervention for dimensions not supplied by an applicable rule;
- reusable authored defaults with local/instance and transient runtime overrides;
- runtime overrides that do not mutate authored definitions and do not claim persistence;
- externally supplied input modality driving per-surface selection-on-open behavior;
- controller/keyboard default selection when explicitly configured;
- pointer/unselected behavior when configured;
- designer-configurable controller-unselected behavior;
- temporary-surface close defaulting to no selected control rather than implicit focus-history restoration;
- retained M1-01 scoped-navigation and independent-window behavior;
- zero hard runtime dependency on another Echo package.

## 2. Authorized scope

### 2.1 External UI context

EUI-M1-02 may add a package-local UI context identity/state surface with these semantics:

- context IDs are stable, normalized, nonempty, project-authored values;
- examples such as `pause`, `cinematic`, and `loading` are conventions only;
- a context is active or inactive;
- any number of contexts may be active simultaneously;
- Looking Glass receives context truth but does not determine why that truth became active;
- context IDs do not carry arbitrary domain payloads in this checkpoint.

The standalone Laboratory may use a tiny sample-owned context driver to toggle example context IDs. The helper is proof infrastructure only.

### 2.2 Ordered per-surface response rules

A registered surface may define an ordered list of context rules. The designer owns list order and may configure different ordering for different surfaces, scene instances, prefab instances, or project cases.

Rule evaluation is **per response dimension**:

1. inspect applicable active-context rules in authored priority order;
2. for each response dimension, the first applicable rule that explicitly supplies a value wins that dimension;
3. continue resolving other dimensions that remain unspecified;
4. if no rule supplies a dimension, context evaluation leaves the current value alone.

At minimum the checkpoint recognizes:

- visibility intent;
- interaction intent;
- selection/focus intent.

There is no package-global “Hide wins,” “Pause wins,” or similar precedence rule.

### 2.3 External-context participation

A surface may disable automatic external-context handling. Disabled participation means context evaluation does not automatically change that surface. Registration, direct open/close/navigation operations, and stable ID access remain available.

### 2.4 Authored, local, and runtime overrides

The effective response configuration may resolve from:

1. designer-authored reusable/base defaults;
2. local scene/prefab/instance authoring overrides;
3. transient project runtime overrides.

Implementation should prefer normal Unity serialized/prefab instance override behavior where it cleanly provides the local case rather than inventing a global scene-policy database.

Runtime overrides:

- are session/runtime state;
- may supersede the effective authored value for explicitly overridden dimensions;
- must not mutate ScriptableObject/prefab/authored definitions;
- are not persisted by Looking Glass;
- do not activate Chronicle, Accord, or any peer-package bridge.

### 2.5 Input-aware selection

Input modality is externally supplied truth. EUI-M1-02 may define a small neutral modality vocabulary sufficient for selection presentation, such as pointer versus navigation/controller, without claiming action-map/device ownership.

Each surface may configure opening behavior independently, including:

- select a configured default control when navigation/controller modality is active;
- open unselected for pointer modality;
- open unselected for navigation/controller modality when the designer chooses.

Closing a temporary surface defaults to clearing package-applied selection. Automatic restoration of the prior selected control is not authorized by this checkpoint.

## 3. Explicit exclusions

EUI-M1-02 does **not** authorize:

- pause, cinematic, loading, saving, gameplay-state, or controller truth ownership;
- arbitrary key/value/object payloads attached to UI contexts;
- loading-progress, dialogue, save, inventory, audio, or gameplay data transport through context IDs;
- automatic input-device or control-scheme detection;
- hard runtime dependency on The Will, Vessel/Controllers, Pulse, Chronicle, Accord, Resonance, First Light, or another Echo package;
- selection-history restoration;
- general focused-window arbitration;
- z-order/window-manager policy beyond behavior already required by the existing surface foundation;
- draggable or resizable windows;
- persisted window positions/layouts/focus histories;
- Chronicle/Accord persistence of runtime overrides;
- Motif schema/capture/apply/local-override implementation;
- Looking Glass Builder;
- preset/template authoring tools;
- broad Lego primitive prefab library expansion;
- modal, notification, tooltip, prompt, safe-area, transition, or full-HUD service implementation;
- automatic `DontDestroyOnLoad`;
- project-wide service composition;
- polished Reference Showcase or production menu art.

Future preset/template work is allowed only as a design direction: presets copy useful starting configuration into freely editable project-owned rules rather than becoming mandatory live centralized policy.

## 4. Exact implementation file families

No runtime implementation file is changed by the authority/activation commit. Once that documentation commit is clean and synchronized, implementation is authorized only within the following bounded file families.

### 4.1 Existing runtime files allowed to change

- `Packages/com.echodevgames.echo-ui/Runtime/Core/EchoUIRoot.cs`
- `Packages/com.echodevgames.echo-ui/Runtime/Surfaces/UISurface.cs`
- `Packages/com.echodevgames.echo-ui/Runtime/EchoDevGames.EchoUI.Runtime.asmdef` only if required for existing Unity/uGUI references; no peer Echo reference may be added.

### 4.2 New runtime context files allowed

Under `Packages/com.echodevgames.echo-ui/Runtime/Context/`:

- `UIContextId.cs`
- `UIContextState.cs`
- `UISurfaceContextRule.cs`
- `UISurfaceContextResponse.cs`
- `UISurfaceContextResolver.cs`
- `UISurfaceRuntimeOverride.cs`
- Unity `.meta` files required for those assets/folders.

Exact member names and internal representation remain routine Level-4 implementation details, but these types may not exceed the behavioral contract in Sections 2 and 5.

### 4.3 New runtime selection files allowed

Under `Packages/com.echodevgames.echo-ui/Runtime/Selection/`:

- `UIInputModality.cs`
- `UISelectionOpenBehavior.cs`
- `UISurfaceSelectionPolicy.cs`
- `UISelectionCoordinator.cs`
- Unity `.meta` files required for those assets/folders.

The selection layer may use the existing Unity EventSystem/uGUI surface. It must not become an input-device detector.

### 4.4 Tests allowed

- `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIContextAndSelectionTests.cs`
- `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIRootFoundationTests.cs` only for retained-foundation regression setup/expectation adjustments caused directly by this checkpoint.
- related `.meta` files.

### 4.5 Laboratory proof files allowed

- `Packages/com.echodevgames.echo-ui/Samples~/LookingGlass_UI_Foundation_Laboratory/README.md`
- `Packages/com.echodevgames.echo-ui/Samples~/LookingGlass_UI_Foundation_Laboratory/Scenes/The Looking Glass_UI_Laboratory.unity`
- `Packages/com.echodevgames.echo-ui/Samples~/LookingGlass_UI_Foundation_Laboratory/Runtime/LaboratoryUIContextDriver.cs`
- related `.meta` files.
- the synchronized imported project-sample counterparts beneath `Assets/Samples/The Looking Glass — UI Framework/0.1.0/Looking Glass UI Foundation Laboratory/` when the checkpoint intentionally uses the imported-authoring workflow.

### 4.6 Closeout documentation allowed

At closeout, reconcile only documentation required by SFGSS-005/SFGSS-ADR-007, including this plan, package/suite Current Notes, package README/CHANGELOG as applicable, Suite Health, test evidence, and the package specification only if implementation exposes a genuine contract correction.

A Green Path scope preflight must stop if implementation requires unrelated Runtime/Editor/sample families or a peer-package reference.

## 5. Implementation rules

1. **Incoming regression first.** Before editing Runtime code, rerun the full EditMode suite. The expected retained floor from `57a4fa4` is **1113 / 1113 passed, 0 failed**. A mismatch is a stop condition, not permission to update the expected count casually.
2. **Package independence remains absolute.** Runtime assembly references no peer Echo runtime package.
3. **Context IDs are presentation-facing addresses, not domain authority.** Looking Glass never decides that gameplay is paused/cinematic/loading.
4. **No arbitrary context payload bus.** Active/inactive context state is sufficient for this checkpoint.
5. **Rule order belongs to the designer.** Do not sort by hardcoded context type or hardcoded hide/show priority.
6. **Resolve per dimension.** Visibility, interaction, and selection/focus intent are evaluated independently. An unspecified dimension is `No Change`, not `false`.
7. **No-rule means no intervention.** Activating a context with no applicable rule must not mutate that surface merely because the context exists.
8. **Visibility is not interaction and interaction is not focus.** The implementation must permit visible/non-interactable/unselected combinations where valid.
9. **External participation is local.** Opting out prevents context-driven mutation only; it must not silently unregister/close the surface.
10. **Authored definitions stay immutable at runtime.** Runtime overrides resolve effective state separately and never write back into serialized authoring data.
11. **Runtime overrides are transient.** No persistence API/file/profile/Chronicle/Accord behavior enters this checkpoint.
12. **Selection is event-driven.** Do not poll/reselect every frame.
13. **Pointer behavior may remain unselected.** Opening a surface must not visually highlight a default merely because one exists when the configured modality/policy says unselected.
14. **Controller/navigation default selection is opt-in per surface.** If configured and a valid target exists, selection should be established deterministically.
15. **Invalid/missing selection targets fail safely.** They must not corrupt surface registration/navigation state.
16. **Close is neutral.** Package-applied selection clears on temporary-surface close by default; prior selection is not implicitly restored.
17. **M1-01 behavior stays green.** Scoped Screen history and independent Window coexistence are not rewritten into a global UI state machine.
18. **Laboratory helpers remain sample-owned.** No sample context/modality helper leaks into the Runtime assembly.
19. **No new suite authority.** If implementation appears to require one, stop and return to Declare/Authorize rather than stretching the checkpoint.

## 6. Focused automated tests

At minimum add focused EditMode coverage equivalent to these behaviors:

1. `ContextIdsAreProjectDefinedAndStable`
2. `MultipleContextsMayBeActiveTogether`
3. `DesignerOrderedRulesResolvePriorityPerDimension`
4. `UnspecifiedRuleDimensionLeavesCurrentStateUnchanged`
5. `SurfaceCanOptOutOfAutomaticExternalContextHandling`
6. `LocalAuthoredOverrideSupersedesReusableBaseWithoutChangingOtherDimensions`
7. `RuntimeOverrideSupersedesEffectiveAuthoredValueWithoutMutatingAuthoredConfiguration`
8. `PointerOpenPolicyMayRemainUnselected`
9. `NavigationModalityCanSelectConfiguredDefault`
10. `NavigationModalityMayBeConfiguredToRemainUnselected`
11. `TemporarySurfaceCloseDoesNotRestoreHistoricalSelectionByDefault`
12. `ContextAndSelectionChangesPreserveScopedScreenHistoryAndIndependentWindowCoexistence`
13. `RuntimeAssemblyHasNoPeerEchoPackageDependency`

Tests must include simultaneous-context cases where one higher-priority rule controls visibility while a lower-priority rule controls interaction, proving the per-dimension cascade rather than first-rule-takes-all behavior.

## 7. Manual Laboratory proof

Extend the existing Looking Glass UI Foundation Laboratory only enough to show the checkpoint behavior directly.

Required manual evidence:

1. Toggle an example `pause` context and observe one configured surface respond while another surface with no rule remains unchanged.
2. Enable `pause` + `cinematic` together and show designer-authored order determining the effective visibility rule.
3. Show per-dimension cascade: one active high-priority rule controls visibility while another controls interaction.
4. Show a surface with external participation disabled ignoring the same context toggles.
5. Show a visible but non-interactable surface to prove visibility and interaction are separable.
6. Set pointer modality, open the configured surface, and confirm no forced selection.
7. Set navigation/controller modality, open a surface configured for default selection, and confirm the configured control is selected.
8. Configure navigation/controller opening as unselected and confirm the designer choice is honored.
9. Close the temporary surface and confirm the package does not implicitly restore prior selection.
10. Repeat M1-01 `main-menu -> settings -> Back -> main-menu` and independent `default-window` coexistence proof to show the foundation remains intact.

The sample helper may expose simple buttons/toggles for context and modality simulation. It must be clearly labeled as Laboratory-owned simulation.

## 8. Green Path execution

EUI-M1-02 may use Green Path automation after this authority/activation commit lands.

Required sequence:

1. Verify branch/repository state is expected and clean.
2. Verify the authority/activation documentation is committed and synchronized.
3. Run the incoming full EditMode regression and require the retained **1113 / 1113, 0 failed** floor before Runtime edits.
4. Apply only the exact EUI-M1-02 implementation scope.
5. Allow Unity compilation/import.
6. Run focused EUI-M1-02 tests.
7. Run the full EditMode suite and require `0` failures; record the actual discovered/passed count rather than assuming it.
8. Perform the ten-item manual Laboratory proof and retain evidence.
9. Synchronize package/imported Laboratory content only where intentionally authored.
10. Run whitespace/scope validation without rewriting stock/vendor assets.
11. Commit the implementation as a distinct implementation boundary.
12. Reconcile package/suite documentation and retained evidence in a distinct closeout boundary.
13. Push and verify clean synchronized repository state.

Green Path must stop on compile/test/manual-proof failure, unexpected path scope, unexpected Git state, baseline mismatch, peer dependency, authored-definition mutation, rollback failure, or authority-changing discovery.

## 9. Stop point

EUI-M1-02 is complete only when:

- the full incoming regression floor was re-established before Runtime implementation;
- focused context/selection tests are green;
- the final full EditMode suite has zero failures;
- the ten manual Laboratory checks pass;
- project-defined active/inactive context IDs, multi-context coexistence, designer ordering, per-dimension no-change behavior, opt-out, authored/local/runtime overrides, and input-aware selection are all proven;
- Runtime has no peer Echo package dependency;
- M1-01 scoped-navigation and independent-window behavior remains green;
- package/imported Laboratory parity is reconciled where applicable;
- documentation matches the committed implementation;
- HEAD/origin and working-tree state are verified clean at closeout.

**Then stop.** Do not begin Motifs, Builder, presets tooling, broad primitive expansion, MMO layout persistence, peer bridges, arbitrary context payloads, focus-history restoration, or richer modal/HUD/transient systems in the same checkpoint.

## 10. Follow-on direction, not authorization

Potential later slices may address reusable copy-in presets/templates, richer focus/window behavior, Motifs, primitive libraries, Builder workflows, modal/HUD/transient services, MMO layout customization/persistence, and optional package bridges.

Naming a follow-on slice does not activate it. The next checkpoint must pass its own bounded Learn → Declare → Authorize gate when required.

---

## Activation record

- JIT learning/intake: **Complete**
- Package declaration: **Complete**
- Package authority reconciliation: **SFGSS-PKG-ECHOUI-001 v1.2.0**
- Checkpoint authorization: **EUI-M1-02 ACTIVE / AUTHORIZED**
- Starting implementation baseline: `57a4fa4`
- Runtime implementation at activation: **Not started**
- Activation Git commit: **the commit containing this ACTIVE / AUTHORIZED transition; record exact short hash at closeout**
