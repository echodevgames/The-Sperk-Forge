
# FL-M4-05 — Startup Presentation Prefab and Canvas Assembly

**Document ID:** FL-M4-05
**Version:** 1.0.0
**Status:** Approved; implementation locked until authority commit
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.6.0
**ADR:** EchoLaunch-ADR-003
**Milestone:** M4 — Startup Entry and Presentation
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Unity baseline:** Unity 6000.3.8f1
**Starting implementation commit:** `858808b`
**Starting documentation commit:** `9d6d469`
**Starting Runtime Play Mode:** 479 passed, 0 failed, 0 ignored
**Starting compilation:** 0 errors, 0 compiler warnings
**Authorized:** August 5, 2026

> The machinery already speaks. This checkpoint gives it a plain, inspectable
> stage without painting the project's heraldry onto the curtains.

---

## 1. Purpose

FL-M4-05 creates the stable package-owned uGUI template prefabs that assemble
the already-proven root and presenter into a scene-ready default composition.

The checkpoint proves serialized references, Canvas defaults, neutral visual
structure, input independence, nested-prefab composition, and stable package
asset identity.

---

## 2. Observable outcome

When complete:

1. `EchoLaunchStatusView.prefab` exists in the presentation assembly folder.
2. `EchoLaunchRoot.prefab` exists beside it.
3. Both prefab `.meta` files are committed.
4. The status prefab is a Screen Space Overlay Canvas.
5. The Canvas uses scalable 1920x1080 reference resolution.
6. The view starts hidden.
7. Every required `EchoLaunchStatusView` serialized reference is assigned.
8. Splash root starts inactive.
9. Determinate and indeterminate progress surfaces are distinct.
10. Text meaning remains independent of color.
11. All graphics are non-raycast targets.
12. The progress slider is non-interactable.
13. No EventSystem, input module, GraphicRaycaster, Button, or skip binding is
    included.
14. No TextMeshPro dependency is added.
15. The root prefab contains a nested status-view prefab instance.
16. Root presenter wiring targets that nested view.
17. Root configuration is intentionally null.
18. Root launch mode is CanonicalBoot.
19. Root automatic start is enabled.
20. No project-owned asset is referenced.
21. No Runtime code discovers or instantiates package prefabs.
22. Editor asset tests pass.
23. Existing 479 Runtime Play Mode tests remain green.

---

## 3. Authority and constraints

Authority set:

- SFGSS-000
- SFGSS-PKG-ECHOLAUNCH-001 v1.6.0
- EchoLaunch-ADR-003
- SFGSS-005
- FL-M4-04 completion evidence
- Current Notes at baseline `9d6d469`

Constraints:

- Runtime authority remains in `EchoLaunchRoot`.
- Presentation remains replaceable.
- Project branding and final layout remain project-owned.
- Input remains project-owned.
- The neutral Runtime assembly must not gain a uGUI or prefab dependency.
- Package source assets are immutable after distribution.
- Stable public prefab GUIDs must be preserved after commit.

---

## 4. Exact prefab paths

```text
Packages/com.echodevgames.echo-launch/
    Presentation.UGUI/Prefabs.meta

Packages/com.echodevgames.echo-launch/
    Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab

Packages/com.echodevgames.echo-launch/
    Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab.meta

Packages/com.echodevgames.echo-launch/
    Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab

Packages/com.echodevgames.echo-launch/
    Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab.meta
```

---

## 5. Status prefab hierarchy

```text
EchoLaunch Status Canvas
├── Backdrop
├── Splash Root
│   ├── Splash Image
│   └── Splash Label
└── Status Root
    ├── State Text
    ├── Message Text
    ├── Step Text
    ├── Determinate Progress Root
    │   ├── Progress Slider
    │   │   ├── Background
    │   │   └── Fill Area
    │   │       └── Fill
    │   └── Progress Text
    ├── Indeterminate Progress Root
    │   └── Indeterminate Text
    └── Elapsed Text
```

Required object names are testable asset contract for setup tooling.

Internal slider children may follow Unity's standard hierarchy.

---

## 6. Canvas and layout defaults

### Canvas

```text
Render mode: Screen Space Overlay
Sorting order: 1000
Pixel perfect: false
```

### CanvasScaler

```text
UI Scale Mode: Scale With Screen Size
Reference Resolution: 1920 x 1080
Screen Match Mode: Match Width Or Height
Match: 0.5
Reference Pixels Per Unit: 100
```

### CanvasGroup

```text
Alpha: 0
Interactable: false
Blocks Raycasts: false
Ignore Parent Groups: false
```

### Anchors

- Canvas and backdrop: full stretch.
- Splash root: full stretch.
- Splash image: centered with proportional margins and `preserveAspect`.
- Splash label: bottom-center within the splash surface.
- Status root: lower-third stretch with safe edge margins.
- Text: horizontal stretch where practical.
- Progress roots: lower-third width, separate sibling surfaces.
- Elapsed text: bottom-right within the status root.

No safe-area runtime behavior is authorized.

---

## 7. Neutral visual defaults

- Dark neutral backdrop with readable opacity.
- Light high-contrast text.
- Built-in Unity runtime font.
- Legacy uGUI `Text`.
- No logo.
- No package splash sprite.
- No branded background.
- No external sample art.
- No animation.
- No color-only status meaning.

Serialized view copy remains the authority for state wording.

---

## 8. Root prefab defaults

```text
EchoLaunch Root
└── EchoLaunch Status Canvas
```

`EchoLaunch Status Canvas` is a nested instance of
`EchoLaunchStatusView.prefab`.

Root serialized defaults:

```text
Configuration: null
Launch Mode: CanonicalBoot
Start Automatically: true
Status Presenter Component: nested EchoLaunchStatusView
```

No project sequence, destination, splash, or other package reference is stored.

---

## 9. Input-independence rules

The final prefab asset must contain no:

- `EventSystem`
- `BaseInputModule`
- `StandaloneInputModule`
- Input System UI module
- `GraphicRaycaster`
- `Button`
- `Toggle`
- selectable navigation
- package-owned skip event binding

All `Graphic.raycastTarget` values are false.

`Slider.interactable` is false.

Projects may add controls to a project-owned variant and route them to
`RequestSplashSkip()`.

---

## 10. Files and assets

### Final authorized package scope

| Path | Action | Purpose |
|---|---|---|
| `Presentation.UGUI/Prefabs.meta` | Create | Stable folder identity |
| `Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab` | Create | Neutral self-contained status/splash Canvas |
| `Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab.meta` | Create | Stable public prefab GUID |
| `Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab` | Create | Root plus nested default presenter |
| `Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab.meta` | Create | Stable public prefab GUID |
| `Tests/Presentation.UGUI/EditMode.meta` | Create when missing | Editor asset-test folder identity |
| `Tests/Presentation.UGUI/EditMode/EchoDevGames.EchoLaunch.Tests.Presentation.UGUI.EditMode.asmdef` | Create | Editor-only prefab asset tests |
| matching asmdef `.meta` | Create | Stable test assembly identity |
| `Tests/Presentation.UGUI/EditMode/EchoLaunchPresentationPrefabAssetTests.cs` | Create | Serialized hierarchy and dependency proof |
| matching test `.meta` | Create | Unity identity |
| `Tests/Presentation.UGUI/PlayMode/EchoLaunchPresentationPrefabContractTests.cs` | Create only when asset loading can remain dependency-clean | Optional instantiated behavior proof |
| matching test `.meta` | Create when applicable | Unity identity |
| active FL-M4-05 Checkpoint Build Plan | Already authorized | Scope authority |

No production script modification is expected.

### Temporary implementation aid

A temporary file may be created outside final package scope:

```text
Assets/Editor/FLM405PresentationPrefabAuthoring.cs
```

It may use Unity Editor APIs to generate the serialized prefab assets.

It and its `.meta` must be deleted before staging. It is not release content.

---

## 11. Implementation sequence

### Phase 1 — Inspect current package identities

1. Record GUIDs of `EchoLaunchRoot.cs` and `EchoLaunchStatusView.cs`.
2. Confirm the presentation assembly compiles.
3. Confirm no existing prefab path conflicts.
4. Confirm the working tree is clean.

### Phase 2 — Generate status prefab

1. Create the exact hierarchy.
2. Assign Canvas defaults.
3. Assign built-in font and readable neutral styling.
4. Configure non-interactive graphics.
5. Wire every `EchoLaunchStatusView` serialized reference.
6. Save through Unity's prefab APIs.
7. Confirm the prefab imports without missing scripts or references.

### Phase 3 — Generate root prefab

1. Create one `EchoLaunchRoot`.
2. Nest the status-view prefab.
3. Wire the neutral presenter reference.
4. Leave configuration null.
5. Preserve CanonicalBoot and automatic-start defaults.
6. Save through Unity's prefab APIs.

### Phase 4 — Remove temporary authoring aid

1. Delete the temporary script and `.meta`.
2. Refresh AssetDatabase.
3. Confirm no `Assets/Editor` residue remains.
4. Confirm final Git scope contains package assets/tests only.

### Phase 5 — Asset tests

Prove:

- Paths and GUIDs.
- Required components.
- Canvas/CanvasScaler/CanvasGroup defaults.
- Required named hierarchy.
- Serialized view references.
- Initial active states.
- Non-interactive graphics.
- No input components.
- No TextMeshPro components or dependency.
- Root nested-prefab composition.
- Root configuration and presenter wiring.
- No dependencies under project `Assets/`.
- No Runtime code modification.

### Phase 6 — Retained runtime proof

1. Compile with 0 errors and 0 warnings.
2. Run Editor asset tests.
3. Run complete Runtime Play Mode suite.
4. Confirm all retained 479 tests pass.
5. Record final discovered Editor-test count.

### Phase 7 — Closeout

1. Review exact Git scope.
2. Commit and push prefab implementation.
3. Generate adjacent documentation.
4. Commit and push documentation.
5. Confirm clean synchronized repository.

---

## 12. Minimum test matrix

At least 18 focused Editor tests should verify:

1. Status prefab exists.
2. Root prefab exists.
3. Both prefab GUIDs resolve.
4. Status prefab root has `Canvas`.
5. Status prefab root has `CanvasScaler`.
6. Status prefab root has `CanvasGroup`.
7. Status prefab root has `EchoLaunchStatusView`.
8. Canvas defaults match authority.
9. CanvasGroup begins hidden/non-interactive.
10. Required hierarchy names exist.
11. Every view serialized reference is assigned.
12. Splash root begins inactive.
13. Slider is non-interactable.
14. All graphics are non-raycast targets.
15. No EventSystem/input module/GraphicRaycaster/Button exists.
16. Text uses a non-null built-in font.
17. Root prefab has one `EchoLaunchRoot`.
18. Root prefab contains nested status view.
19. Root presenter reference targets nested view.
20. Root configuration is null.
21. Root mode is CanonicalBoot and automatic start is true.
22. Prefab dependencies do not include project `Assets/`.
23. No TextMeshPro component or assembly dependency exists.
24. Package prefabs instantiate without missing scripts.

Planning floor:

```text
Editor tests: 24 or greater
Runtime Play Mode retained: 479
```

Final discovered totals are evidence.

---

## 13. Manual inspection gate

In Unity Prefab Mode:

- Open `EchoLaunchStatusView.prefab`.
- Confirm the Canvas preview is readable at 16:9 and a tall aspect ratio.
- Confirm status and splash roots do not overlap incorrectly.
- Confirm no yellow missing-reference warnings.
- Confirm the hierarchy names match the plan.
- Confirm the prefab begins visually hidden.
- Open `EchoLaunchRoot.prefab`.
- Confirm configuration is `None`.
- Confirm presenter points to the nested view.
- Confirm automatic start is enabled.
- Confirm no EventSystem exists.

This is structural inspection, not a final art review.

---

## 14. Failure symptoms

| Symptom | Likely cause | In-scope response |
|---|---|---|
| Missing script on prefab | Wrong script GUID or generated before compile | Regenerate through Unity APIs after clean compile |
| Presenter field is null | Nested reference not serialized | Rewire with `SerializedObject` and resave |
| Text invisible | Built-in font not assigned or CanvasGroup hidden during Prefab preview | Assign built-in font; inspect child graphics directly |
| UI blocks project input | Raycast target or GraphicRaycaster included | Remove interactive components and disable raycast targets |
| Prefab references `Assets/` content | Temporary project asset leaked into template | Replace with built-in/package-only dependency |
| Root starts and fails in a test scene | Configuration intentionally null | Do not enter Play Mode with the template root until a project configuration is assigned |
| Test assembly cannot use AssetDatabase | Wrong assembly platform | Use the authorized Editor-only test asmdef |
| Generated authoring script appears in Git scope | Temporary aid not removed | Delete script and `.meta` before staging |

---

## 15. Explicit exclusions

FL-M4-05 does not authorize:

- Setup or repair windows.
- Editor migration.
- Direct-scene initializer tooling.
- Automatic prefab discovery or spawning.
- Runtime code changes unless a proven prefab blocker requires authority review.
- Project configuration assets.
- Boot scene generation.
- Test Lab scenes.
- Project logo or branded art.
- Custom font assets.
- TextMeshPro integration.
- Input bindings or EventSystem.
- Safe-area runtime scripts.
- Animation systems.
- Video splash support.
- Player builds.
- Package version change.

---

## 16. Rollback

Before commit:

```cmd
git restore --staged .
git clean -fd -- "Packages/com.echodevgames.echo-launch/Presentation.UGUI/Prefabs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Presentation.UGUI/Prefabs.meta"
git clean -fd -- "Packages/com.echodevgames.echo-launch/Tests/Presentation.UGUI/EditMode"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Presentation.UGUI/EditMode.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Presentation.UGUI/PlayMode/EchoLaunchPresentationPrefabContractTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Presentation.UGUI/PlayMode/EchoLaunchPresentationPrefabContractTests.cs.meta"
git clean -fd -- "Assets/Editor/FLM405PresentationPrefabAuthoring.cs"
git clean -f -- "Assets/Editor/FLM405PresentationPrefabAuthoring.cs.meta"
```

After a pushed implementation commit, use `git revert`.

---

## 17. Commit plan

Authority:

```text
echo-launch: approve FL-M4-05 neutral presentation prefabs
```

Implementation:

```text
echo-launch: complete FL-M4-05 startup presentation prefabs
```

Documentation:

```text
echo-launch: document FL-M4-05 completion
```

---

## 18. Stop point

Stop after the two package template prefabs, stable GUIDs, serialized Canvas and
root wiring, focused Editor asset proof, manual structural inspection, and
retained Runtime Play Mode proof.

Do not begin setup tooling, migration, direct-scene initialization, Boot scene
generation, or Standalone Laboratory work.

---

## 19. Tentative next checkpoint

**FL-M5-01 — Editor Setup Foundation and Non-Destructive Project Plan**

Tentative only. It requires a separate authority and Checkpoint Build Plan.

---

## 20. Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Commit specification v1.6.0 and ADR-003 before prefab
generation. Preserve project ownership of branding and input, stable prefab
GUIDs, neutral Runtime independence, and the no-hidden-instantiation rule.
