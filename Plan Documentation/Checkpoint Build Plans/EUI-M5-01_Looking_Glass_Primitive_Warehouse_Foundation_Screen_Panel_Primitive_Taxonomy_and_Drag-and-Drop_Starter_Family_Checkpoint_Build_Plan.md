# EUI-M5-01 — Looking Glass Primitive Warehouse Foundation, Screen/Panel/Primitive Taxonomy, and Drag-and-Drop Starter Family

**Document ID:** EUI-M5-01
**Version:** 1.0.0
**Status:** ACTIVE / AUTHORIZED
**Package:** The Looking Glass (`EchoUI`)
**Milestone:** M5 — Tooling and Laboratory
**Activation baseline:** `a8f70f855715bcf48be1e96fb694c41867282125`
**Suite authority:** SFGSS-000 v0.27.0
**Package authority:** SFGSS-PKG-ECHOUI-001 v1.9.0 — unchanged
**Workflow authority:** SFGSS-005 v1.7.0
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0
**Activated:** August 18, 2026
**Owner:** Jesse “Echo” Adams / EchoDevGames

## 1. Purpose and observable outcome

Establish the smallest useful implementation of the Looking Glass **Primitive Warehouse** as a package-owned, directly browsable collection of ordinary editable Unity prefabs.

The completed slice must prove a simple primary workflow: a developer browses the EchoUI package in Unity, drags a Warehouse prefab into a project Canvas, and receives a useful normal uGUI starting point that can be moved, duplicated, overridden, unpacked, nested, or otherwise edited with standard Unity prefab workflows. No proprietary Builder, catalog window, generation format, or peer Echo package is required.

This checkpoint also makes the durable Assembly Library promise concrete through one three-tier taxonomy:

```text
Screens / Screen Templates
    built from Panels and Primitives

Panels / Panel Modules
    built from Primitives and optionally other Panels

Primitives
    focused reusable UI pieces with one obvious presentation job
```

**Motifs are orthogonal to the three construction tiers.** Participating Warehouse pieces may expose the existing Runtime Motif bindings, but a Motif is an appearance recipe rather than a fourth composition tier.

The Warehouse is intentionally designed to grow organically over the lifetime of the package. The first checkpoint proves the shelf system, not the eventual inventory.

## 2. Starting conditions

- Live `main` at activation is exactly EUI-M4-03 final closeout `a8f70f855715bcf48be1e96fb694c41867282125`.
- Final post-closeout Unity evidence on that parent is full Foundry EditMode **1445 / 1445**, EchoUI Editor **339 / 339**, Motif fixtures **62 / 62**, root fixture **12 / 12**, failed/skipped/inconclusive **0 / 0 / 0**.
- EUI-M4-03 manual Motif Laboratory is **6 / 6 PASS**; 180-frame quiescence PASS; retained M4-02 through M1 representative smoke is user-confirmed green; package/imported Motif proof parity is verified.
- Runtime package remains `0.1.0`.
- Required Unity package remains only `com.unity.ugui` `2.0.0` for this slice.
- PKG-LEARN-008 remains complete. This checkpoint records a bounded JIT revisit of its already-approved Assembly Library direction rather than reopening the package learning review.
- SFGSS-PKG-ECHOUI-001 v1.9.0 already contains EUI-D-028, EUI-D-029, EUI-D-031, EUI-D-041, and EUI-D-078, which authorize Lego-like primitives, editable templates, project-owned composition, Motifs as appearance, and the separation of Warehouse/Template/Catalog/Utilities from the later Builder.

## 3. Learn → Declare → Authorize reconciliation

### 3.1 Learned boundary

The live authority already promises the Primitive Warehouse and ordinary editable template compositions. EUI-M4-03 intentionally stopped before fixing a final Warehouse-facing authoring schema so that real primitive authoring could expose the smallest useful contract.

The first Warehouse checkpoint therefore does not need a new Runtime architecture or package-authority version merely to create normal uGUI prefab assets. The correct first proof is authoring-oriented: stable package assets, clear composition tiers, standard Unity prefab behavior, dependency safety, Motif compatibility where the existing Runtime contract is sufficient, and a small test/manual proof surface.

A Warehouse asset is presentation composition. It must not silently acquire domain, persistence, scene-flow, input, pause, audio, or project-lifetime authority merely because a future whole-game UI may present those systems.

### 3.2 Jesse's declared product intent

The approved designer intent for this checkpoint is:

- The **primary Warehouse workflow is direct package-folder browsing and drag-and-drop prefab use**. Tooling may accelerate this later, but it cannot become mandatory.
- The Warehouse should eventually become very large. Abundance is a goal; discoverability and organization solve scale rather than artificially limiting the inventory.
- Growth should be organic. Real games may produce reusable pieces that are later deliberately cleaned of game-specific dependencies, validated, and promoted into the package through a future bounded workflow.
- Construction uses three tiers:
  - **Primitives:** smallest focused reusable UI pieces.
  - **Panels:** reusable functional groups composed from Primitives and, where useful, other Panels.
  - **Screens / Screen Templates:** whole starting compositions built from Panels and Primitives.
- Examples of future Screen inventory include Main Menu, Settings, Pause, Win, Lose, Save/Load, Character, Inventory, Quest Log, Crafting, Journal, Level Select, Credits, and whatever proves generically useful later.
- Examples of future Panel inventory include button groups, slider groups, audio settings, graphics settings, scores, player information, stats, inventory groups, save slots, quests, tabs, headers, and footers.
- Examples of future Primitive inventory include button families, sliders, toggles, dropdowns, fields, tabs, progress indicators, portraits, item slots, borders, panel surfaces, separators, scroll pieces, and icons.
- **Motifs cross the tiers rather than sitting above them as a construction tier.** A future Motif Palette may preview/apply appearances to selected Warehouse pieces, but that Editor tool is not part of EUI-M5-01.
- Warehouse assets remain normal editable Unity prefabs. Projects may use prefab overrides, variants, unpacking, copying, reparenting, and local customization. The package must not create an opaque generated-object format.
- A later Builder/Composer consumes the Warehouse; the Warehouse never depends on the Builder.
- A much-later community/Creator Lab may allow users to validate/share Warehouse-compatible content. That idea is deliberately outside current scope.

### 3.3 Authority reconciliation

These declarations refine the implementation shape already allowed by SFGSS-PKG-ECHOUI-001 v1.9.0. They do **not** change package ownership, hard dependencies, serialized Runtime authority, or public Runtime lifecycle contracts. Package authority therefore remains **v1.9.0**.

If implementation reveals that the first Warehouse family requires a new Runtime registration model, new mandatory dependency, new serialized public contract, or a change to EUI-D-078, the Green Path stops and returns to package authority before that change is made.

## 4. Authorized taxonomy and invariants

1. The package Warehouse has three compositional tiers: `Screens`, `Panels`, and `Primitives`.
2. `Screen Template` is the precise authoring term when distinction from the Runtime `Screen` lifecycle role matters.
3. A Primitive has one focused presentation/interaction job. If an asset contains several independently meaningful UI concepts, it normally belongs in Panels rather than Primitives.
4. A Panel is a reusable functional composition of Primitives and may nest other Panels when doing so remains understandable and useful.
5. A Screen Template is a whole editable starting composition. It is not a mandatory canonical implementation of Settings, Pause, Win, or any other project screen.
6. Motifs are an orthogonal appearance system. Motif application cannot become layout, navigation, content, persistence, or domain authority.
7. Direct drag-and-drop from package folders is a first-class supported workflow.
8. Warehouse use does not require the Template Catalog, Assembly Utilities, Motif Palette, or Builder.
9. Package assets retain stable committed `.meta` GUIDs.
10. Package assets must not reference project-only assemblies/assets or peer Echo packages.
11. Package assets remain ordinary Unity/uGUI objects and can be customized with normal prefab workflows.
12. The first family must not lock the final typography/provider schema. Where useful, content/label anchors may remain project-populated rather than forcing a new text dependency.
13. Starter assets may expose existing `UIMotifBindingTarget` / `UseMotif` / `KeepLocal` behavior, but EUI-M5-01 does not change the explicit Motif-registration authority established in EUI-M4-03.
14. A visual `Close` button primitive does not gain universal Screen/Window/Modal/domain-close authority. Its click meaning remains project/surface wiring unless an already-authorized generic Looking Glass adapter is explicitly configured.
15. The Warehouse may grow without a fixed catalog-size limit because it is an authored asset collection, not an unbounded Runtime queue/state store.
16. Project-born content may be promoted into the Warehouse only through a later deliberate validation/cleaning workflow; no automatic promotion is authorized now.

## 5. First starter family

EUI-M5-01 implements exactly one small composition chain across all three tiers.

### 5.1 Primitive tier

1. **Default square button** — a neutral uGUI Button starting point with focused visual hierarchy and optional existing Motif bindings.
2. **Square close button** — a compact close-shaped visual Button starting point whose actual close command remains project/surface wiring.
3. **Default panel surface** — a neutral reusable background/surface Image primitive suitable for nesting beneath Panels/Screens. This checkpoint does not require a production 9-slice art family.

### 5.2 Panel tier

4. **Button group panel** — a reusable editable arrangement composed from the starter button primitives.

### 5.3 Screen tier

5. **Basic menu Screen Template** — a whole neutral starting composition assembled from the starter panel/surface/primitives. It may carry example/default Looking Glass surface metadata where safe, but all project-specific stable IDs and command meanings remain overrideable/project-owned.

The exact visible names remain implementation-level refinements under the established `Type_DescriptiveName` convention. The initial expected prefab filenames are recorded in Section 8 and may be changed only before first publication if Unity authoring evidence shows a clearer non-breaking name.

## 6. Motif relationship

EUI-M5-01 consumes the existing Motif Runtime rather than extending it by default.

- Starter visual components should use current Motif token families where doing so is useful and dependency-safe.
- `KeepLocal` remains available for elements that should retain authored local presentation.
- No automatic scene scan is introduced.
- No new self-registering Motif target behavior is authorized by default.
- Manual/Laboratory proof may explicitly register placed starter targets through the already-public root API.
- The final typography/provider schema, Motif capture/apply/preview tooling, Motif Palette, and destructive edit-time restyling remain deferred.

## 7. Explicit exclusions

EUI-M5-01 does **not** authorize:

- Motif Palette Editor window;
- Motif capture/apply/preview authoring tools;
- Stable-ID Template Catalog implementation;
- Assembly Utilities implementation;
- Builder / Composer implementation;
- automatic project-to-package prefab promotion;
- community contribution ingestion or Creator Lab;
- broad Warehouse population beyond the five starter roles in Section 5;
- slider, toggle, dropdown, input, tab, progress, portrait, item-slot, inventory, save, audio, graphics, settings, pause, win, lose, character, quest, crafting, journal, level-select, or credits families unless they are separately activated later;
- production art, final visual family branding, or a full 9-slice asset library;
- final Warehouse-facing typography/provider contract or new mandatory text dependency;
- new mandatory TextMeshPro dependency;
- new hard peer Echo dependency;
- Chronicle, Passage, Accord, Resonance, Will, Pulse, First Light, or other peer-package wiring;
- persistence, scene-flow, gameplay, pause/time-scale, cursor, input-map, audio, localization, analytics, or domain authority;
- full accessibility policy;
- prompts/tooltips;
- safe-area adaptation;
- richer independent Window LIFO/pin/drag/resize/layout management;
- generalized dim/blur;
- new transition drivers;
- project-wide lifetime composition;
- polished Reference Showcase work;
- clean-project reproduction, Distribution Kit work, release qualification, or external adoption;
- material Motif Runtime or other Runtime architecture changes without a stop-and-return-to-authority decision.

## 8. Exact implementation files and assets

The first implementation slice is confined to the following package-owned Warehouse/test/documentation manifest. All Unity-imported assets and folders receive unique committed GUIDs. `Documentation~` is not imported as Unity asset content and therefore does not receive Unity `.meta` files.

| Path | Action | Purpose |
|---|---|---|
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs.meta` | Add | New package prefab-root folder identity |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse.meta` | Add | Warehouse folder identity |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives.meta` | Add | Primitive tier folder identity |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives/Buttons.meta` | Add | Button family folder identity |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives/Surfaces.meta` | Add | Surface family folder identity |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Panels.meta` | Add | Panel tier folder identity |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Screens.meta` | Add | Screen Template tier folder identity |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives/Buttons/Button_DefaultSquare.prefab` | Add | Default square button primitive |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives/Buttons/Button_DefaultSquare.prefab.meta` | Add | Stable prefab GUID |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives/Buttons/Button_CloseSquare.prefab` | Add | Square close button primitive |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives/Buttons/Button_CloseSquare.prefab.meta` | Add | Stable prefab GUID |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives/Surfaces/Image_DefaultPanelSurface.prefab` | Add | Basic panel-surface primitive |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Primitives/Surfaces/Image_DefaultPanelSurface.prefab.meta` | Add | Stable prefab GUID |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Panels/Panel_ButtonGroup.prefab` | Add | Panel module composed from button primitives |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Panels/Panel_ButtonGroup.prefab.meta` | Add | Stable prefab GUID |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Screens/Screen_BasicMenu.prefab` | Add | Whole basic Screen Template composition |
| `Packages/com.echodevgames.echo-ui/Runtime/Prefabs/Warehouse/Screens/Screen_BasicMenu.prefab.meta` | Add | Stable prefab GUID |
| `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIWarehousePrefabTests.cs` | Add | Focused authoring/asset contract proof |
| `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIWarehousePrefabTests.cs.meta` | Add | Stable test-script GUID |
| `Packages/com.echodevgames.echo-ui/Documentation~/User/Primitive Warehouse.md` | Add | Direct drag-and-drop usage, taxonomy, customization, limitations |

The package README/changelog, Current Notes, checkpoint plan, and suite status records are closeout/reconciliation files and may change later without expanding Runtime scope.

No existing Runtime C# source is part of the default implementation manifest.

## 9. Focused automated proof

The initial target is one focused Editor fixture with **8 tests**. Exact final test names may receive compile-safe refinement while preserving these eight assertions:

1. all five starter prefabs exist at their declared package paths;
2. default square button has the expected ordinary uGUI interaction/presentation structure;
3. square close button has the expected ordinary uGUI interaction/presentation structure and no universal domain-close behavior;
4. default panel surface has the expected ordinary uGUI surface structure;
5. button-group Panel composes the intended starter Primitive assets without project-only dependencies;
6. basic menu Screen Template composes the intended Panel/Primitive assets and remains an ordinary editable prefab;
7. package prefab dependency closure contains no project assemblies/assets or peer Echo packages;
8. Motif-ready bindings/local-preserve configuration, where authored, use only the existing EUI-M4-03 Runtime contract and do not mutate source prefab assets during proof.

If all eight tests are added without other test-count changes, the expected post-implementation aggregate becomes full Foundry **1453 / 1453** and EchoUI Editor **347 / 347**. Those totals are a planning expectation only; Unity XML establishes the authoritative final count.

## 10. Manual proof

After automated green, the user performs a small authoring proof in Unity:

1. Browse the package Warehouse directly in the Project window and drag the default square Button into a Canvas. Confirm it is a normal editable prefab instance and functions as an ordinary uGUI Button.
2. Drag the Button Group Panel into the Canvas. Confirm its children are understandable/editable and the group can be rearranged/customized using standard Unity prefab-instance workflows.
3. Drag the Basic Menu Screen Template into the UI hierarchy. Confirm it is visibly assembled from the starter Panel/Primitive pieces and does not require the future Builder/Catalog/Utilities.
4. Exercise existing Motif application on participating targets through the already-authorized registration path. Confirm appearance may change without changing layout/navigation/domain meaning and any authored `KeepLocal` binding remains local.
5. Make a scene-instance or project-variant customization and confirm the package source prefab remains unchanged.
6. Run representative retained Looking Glass smoke and confirm no EUI-M4-03 through M1 behavior regressed.

No claim of Chronicle/Passage/Accord integration is made by this proof.

## 11. Organic growth rule

The Warehouse is intentionally open-ended. Future checkpoints may add new Primitive families, Panels, and Screen Templates whenever real project work demonstrates a reusable generic pattern.

A project-created candidate does not enter package source merely because it exists. Future promotion should require at minimum:

- removal of game-specific/domain dependencies;
- neutral naming/content or clearly reusable semantics;
- package dependency validation;
- prefab/GUID hygiene;
- focused tests or validation appropriate to the asset;
- documentation/catalog placement appropriate to the then-current Warehouse tooling.

Automated promotion tooling and community ingestion are future ideas, not EUI-M5-01 requirements.

## 12. Stop-on-red rules

Stop before implementation expands if any of the following becomes necessary:

- a new hard package dependency;
- a new mandatory text provider/package;
- a new Runtime Motif registration authority or automatic scene scan;
- a change to public Screen/Window/Modal/HUD/notification lifecycle semantics;
- a serialized compatibility break;
- a package-specification decision that contradicts v1.9.0;
- project/domain behavior embedded in a generic Warehouse prefab;
- an unexpected file outside the manifest that cannot be explained as Unity metadata for an authorized asset.

Any such discovery returns to Learn → Declare → Authorize before continuing.

## 13. Commit boundaries

Planned durable boundaries:

1. `Activate EUI-M5-01 Primitive Warehouse foundation` — documentation-only activation.
2. `Implement EUI-M5-01 Primitive Warehouse starter family` — five prefab roles, exact required folder metadata, focused tests, and bounded user documentation.
3. Bounded correction commit(s) only for the first owned Unity/authoring failure if needed.
4. `Close out EUI-M5-01 Primitive Warehouse foundation` — accepted automated/manual evidence and status reconciliation.

All publication to `main` remains non-forced and exact-parent verified.

## 14. Activation gate

EUI-M5-01 is **ACTIVE / AUTHORIZED** only after the documentation activation commit is published from clean parent `a8f70f855715bcf48be1e96fb694c41867282125`.

The activation changes no Runtime, prefab, test, dependency, package-version, or GUID state. The final post-EUI-M4-03 **1445 / 1445** run on the activation parent is the incoming regression floor; a redundant Unity rerun is not required solely for a documentation-only activation.

## 15. Exact resume phase after activation

Present the first implementation slice again and wait for explicit `go` before creating Warehouse prefab/test assets.

The first implementation slice is **only the five starter roles plus their required folder metadata, one focused 8-test fixture, and the bounded Warehouse user guide**. Motif Palette, Template Catalog, Assembly Utilities, Builder/Composer, promotion tooling, community tooling, broad Warehouse population, bridges, and adjacent M5 work remain inactive.
