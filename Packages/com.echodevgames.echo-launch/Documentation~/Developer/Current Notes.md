
# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M4-05`
- Title: Startup Presentation Prefab and Canvas Assembly
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.6.0
- ADR: EchoLaunch-ADR-003
- Status: Authority approved; prefab generation locked until authority commit
- Repository baseline: `9d6d469`
- Last implementation commit: `858808b`
- Runtime Play Mode baseline: 479 passed, 0 failed, 0 ignored
- Compilation baseline: 0 errors, 0 compiler warnings

## Approved Contract

### Package Template Assets

```text
Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab
Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

Both assets preserve committed stable GUIDs.

### Status Canvas

- Screen Space Overlay
- Sorting order 1000
- Scale With Screen Size
- 1920x1080 reference resolution
- Match 0.5
- Hidden through `CanvasGroup` by default
- Neutral dark backdrop and high-contrast text
- Built-in legacy uGUI font
- Splash and status surfaces
- Distinct determinate and indeterminate progress roots
- Every existing view reference assigned

### Input Boundary

No EventSystem, input module, GraphicRaycaster, Button, or skip binding.

All graphics are non-raycast targets.

The slider is non-interactable.

Projects route input to `RequestSplashSkip()` through project code.

### Root Template

- One `EchoLaunchRoot`
- Nested status-view prefab
- Presenter reference wired
- Configuration null
- CanonicalBoot mode
- Automatic start enabled
- No project-owned definition asset

### Runtime Boundary

No hidden discovery, `Resources` load, Addressables lookup, or automatic prefab
instantiation.

Projects or later setup tooling explicitly place or copy the template.

## Implementation Lock

Do not generate or stage prefab assets until the authority commit is pushed.

Required authority commit:

```text
echo-launch: approve FL-M4-05 neutral presentation prefabs
```

## Expected Final Scope

- Prefabs folder and `.meta`
- Two prefab assets and stable `.meta` files
- Editor-only prefab asset-test assembly
- Focused prefab asset tests
- Optional dependency-clean instantiated contract tests
- No production Runtime script changes

## Explicit Exclusions

- Setup/repair tooling
- Editor migration
- Direct-scene initializer
- Boot scene generation
- Test Lab scenes
- Project branding
- Custom fonts
- TextMeshPro
- Input bindings
- Safe-area scripts
- Animation
- Player builds

## Handoff Snapshot

FL-M4-04 is fully closed at `9d6d469`.

FL-M4-05 authority is prepared through specification v1.6.0, ADR-003, and the
approved Checkpoint Build Plan.

Prefab generation begins only after the authority commit is confirmed.
