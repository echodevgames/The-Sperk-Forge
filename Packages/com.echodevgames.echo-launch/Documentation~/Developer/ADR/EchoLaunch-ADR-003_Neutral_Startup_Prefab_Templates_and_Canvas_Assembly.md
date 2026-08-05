
# EchoLaunch ADR-003 — Neutral Startup Prefab Templates and Canvas Assembly

## Metadata

- ADR: `EchoLaunch-ADR-003`
- Status: Approved
- Date: August 5, 2026
- Package: First Light (`EchoLaunch`)
- Package specification: SFGSS-PKG-ECHOLAUNCH-001 v1.6.0
- Checkpoint: FL-M4-05
- Decision owner: Jesse “Echo” Adams / EchoDevGames
- Baseline commit: `9d6d469`

## Context

FL-M4-02 implemented the removable plain uGUI presenter.

FL-M4-03 added deterministic image splash presentation.

FL-M4-04 bound splash playback to configuration schema 4 and the authoritative
root.

The remaining M4 presentation gap is a reusable package-owned prefab and Canvas
assembly. The specification also requires project ownership of branding,
production layout, fonts, colors, and input bindings.

The package therefore needs a neutral template without turning that template
into hidden runtime authority or project branding.

## Decision

### Two stable package template prefabs

The package ships:

```text
Packages/com.echodevgames.echo-launch/
    Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab

Packages/com.echodevgames.echo-launch/
    Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

These are immutable package templates with committed stable `.meta` files.

### Status-view template

`EchoLaunchStatusView.prefab` owns one self-contained startup-only Canvas.

Required root components:

- `RectTransform`
- `Canvas`
- `CanvasScaler`
- `CanvasGroup`
- `EchoLaunchStatusView`

Canvas defaults:

```text
Render mode: Screen Space Overlay
Sorting order: 1000
Pixel perfect: false
CanvasScaler mode: Scale With Screen Size
Reference resolution: 1920 x 1080
Screen match mode: Match Width Or Height
Match: 0.5
```

The Canvas begins hidden:

```text
CanvasGroup.alpha = 0
CanvasGroup.interactable = false
CanvasGroup.blocksRaycasts = false
```

### Required hierarchy roles

```text
EchoLaunch Status Canvas
├── Backdrop
├── Splash Root                       [inactive by default]
│   ├── Splash Image
│   └── Splash Label
└── Status Root
    ├── State Text
    ├── Message Text
    ├── Step Text
    ├── Determinate Progress Root
    │   ├── Progress Slider
    │   └── Progress Text
    ├── Indeterminate Progress Root
    │   └── Indeterminate Text
    └── Elapsed Text
```

The implementation may add purely structural children when Unity's `Slider`
requires them, but the documented role objects above remain discoverable.

### Neutral default visuals

The template uses:

- A plain dark neutral backdrop.
- High-contrast light text.
- Legacy uGUI `Text`.
- Unity's built-in runtime font.
- Full-stretch and lower-third anchors.
- No logo.
- No branded splash image.
- No external sample art.
- No animation component.

These values are a readable fallback, not project branding.

### Non-interactive package template

The package template contains no:

- `EventSystem`
- `StandaloneInputModule`
- Input System UI module
- `GraphicRaycaster`
- `Button`
- package-owned skip binding
- keyboard/controller binding
- pointer binding

All `Graphic.raycastTarget` values are false.

The progress `Slider` is non-interactable.

Projects route input to `EchoLaunchStatusView.RequestSplashSkip()` through their
own input code or add project-owned controls in a prefab variant.

### Root composition template

`EchoLaunchRoot.prefab` contains:

```text
EchoLaunch Root
└── EchoLaunch Status Canvas
```

The child is a nested instance of `EchoLaunchStatusView.prefab`.

Root defaults:

```text
Launch mode: CanonicalBoot
Start automatically: true
Configuration: null
Status presenter: nested EchoLaunchStatusView
```

The configuration is intentionally null because launch definitions are
project-owned.

The template contains no destination, sequence, splash, project service, or
other package dependency.

### No hidden runtime discovery

EchoLaunch Runtime does not:

- Load these prefabs from `Resources`.
- Use Addressables to locate them.
- Search scenes by name.
- Instantiate them automatically.
- Repair or replace project copies at runtime.

A scene, future setup tool, or direct-scene development helper explicitly uses a
template.

### Project ownership

A production project may:

- Place the package root prefab directly and assign configuration.
- Create a prefab variant.
- Copy the template into `Assets/`.
- Replace the presenter entirely.

Project-owned variants may change art, fonts, colors, copy, Canvas mode, layout,
input controls, and animation while preserving the neutral presenter contract.

Future setup tooling must not overwrite existing project variants merely
because the package template changes.

### Asset generation

The final committed artifacts are prefab YAML and stable `.meta` files.

A temporary uncommitted package-development Editor script may be used to
generate the prefab assets safely through Unity APIs. It must be removed,
together with its `.meta`, before final staging.

The temporary generator is not package API, setup tooling, or release content.

## Rejected alternatives

### Runtime-generated Canvas

Rejected.

It hides scene composition, is difficult for designers to customize, produces
unstable hierarchy ownership, and weakens prefab evidence.

### Auto-load from Resources

Rejected.

It creates hidden discovery rules, forces build inclusion, and couples Runtime
to a specific package asset path.

### Include a default skip button

Rejected.

Input ownership remains project-specific. The neutral public skip-request seam
already exists.

### Include an EventSystem or input module

Rejected.

The package must not choose the project's UI input stack.

### Use TextMeshPro

Rejected for the default template.

The current presentation assembly intentionally depends only on uGUI. Projects
may replace text components through a custom presenter or later approved
adapter.

### Put prefabs under neutral Runtime

Rejected.

The assets contain uGUI and `EchoLaunchStatusView`; they belong to the removable
presentation assembly.

### Store project configuration in the root template

Rejected.

Configuration, sequences, splashes, and destinations are project-owned
definitions.

## Consequences

### Positive

- A scene-ready default composition exists.
- Runtime remains neutral and does not auto-spawn visuals.
- Designers can inspect and customize a concrete hierarchy.
- Package removal and presenter replacement remain possible.
- Input ownership remains with the project.
- Stable prefab GUIDs support setup tooling and future upgrades.
- Project branding remains separate from package defaults.

### Costs

- The template uses legacy uGUI `Text` until a later optional presentation
  adapter is approved.
- Projects must explicitly place or copy the root template.
- Mobile safe-area adaptation remains project-owned or later tooling.
- Editor asset tests are required because prefab wiring is serialized evidence.

## Implementation boundary

FL-M4-05 may create the two prefab assets, stable `.meta` files, focused
Editor asset tests, and any presentation tests required to prove the committed
hierarchy.

It may not implement:

- Setup/repair windows.
- Editor schema migration.
- Direct-scene initializer tooling.
- Automatic prefab discovery or instantiation.
- Project branding or sample art.
- Input bindings.
- TextMeshPro integration.
- Safe-area runtime scripts.
- Test Lab scenes.
- Player-build claims.

## Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
