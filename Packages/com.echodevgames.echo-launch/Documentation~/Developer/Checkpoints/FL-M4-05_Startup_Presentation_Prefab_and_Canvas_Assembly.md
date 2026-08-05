# FL-M4-05 - Startup Presentation Prefab and Canvas Assembly

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.6.0
- ADR: EchoLaunch-ADR-003
- Checkpoint: `FL-M4-05`
- Milestone: M4 - Startup Entry and Presentation
- Authority commit: `311a9d2`
- Implementation commit: `8d3c6a7`
- Previous documentation commit: `9d6d469`
- Implementation status: Complete and pushed
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Create stable, neutral, package-owned uGUI prefab templates that assemble the
already-proven root and status/splash presenter without claiming project
branding, project configuration, or input authority.

## Implemented Assets

### Status-View Template

Created:

```text
Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab
```

The prefab contains:

- Screen Space Overlay `Canvas`
- `CanvasScaler`
- Hidden non-interactive `CanvasGroup`
- `EchoLaunchStatusView`
- Neutral backdrop
- Splash image and label surface
- Status, message, and step text
- Determinate slider and progress text
- Indeterminate progress surface
- Elapsed text

Every existing serialized presenter reference is assigned.

### Root Template

Created:

```text
Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

The prefab contains one `EchoLaunchRoot` and one nested instance of
`EchoLaunchStatusView.prefab`.

Defaults:

```text
Configuration: null
Launch Mode: CanonicalBoot
Start Automatically: true
Status Presenter: nested EchoLaunchStatusView
```

### Input and Branding Boundary

The final prefabs contain no:

- EventSystem
- Input module
- GraphicRaycaster
- Button
- Toggle
- Package skip binding
- TextMeshPro component
- Project-owned font
- Project logo or branded art
- Dependency beneath project `Assets/`

All graphics are non-raycast targets.

The progress slider is non-interactable.

### Stable Asset Identity

Committed:

- `Prefabs.meta`
- `EchoLaunchStatusView.prefab.meta`
- `EchoLaunchRoot.prefab.meta`
- EditMode test folder and assembly metadata

The prefab GUIDs are nonempty and distinct.

### Authoring Method

A temporary Unity Editor authoring helper generated the prefabs through Unity
APIs.

It created the hierarchy, assigned private serialized references, saved the
nested-prefab relationship, verified import, and deleted
`Assets/FLM405Temp`.

No temporary authoring file entered the implementation commit.

Unity-generated trailing whitespace in prefab YAML and metadata was trimmed
before staging without changing asset identity or behavior.

## Files

Created:

- `Presentation.UGUI/Prefabs.meta`
- `Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab`
- `Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab.meta`
- `Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab`
- `Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab.meta`
- `Tests/Presentation.UGUI/EditMode.meta`
- `Tests/Presentation.UGUI/EditMode/EchoDevGames.EchoLaunch.Tests.Presentation.UGUI.EditMode.asmdef`
- matching asmdef `.meta`
- `Tests/Presentation.UGUI/EditMode/EchoLaunchPresentationPrefabAssetTests.cs`
- matching test `.meta`

No production C# file was modified.

## Compile Evidence

- Errors: `0`
- Compiler warnings: `0`
- Temporary authoring folder removed: Yes

## EditMode Test Evidence

- Passed: `27`
- Failed: `0`
- Ignored: `0`

Verified both prefab paths and GUIDs, approved components and Canvas defaults,
complete hierarchy and serialized wiring, initial active states,
input-independence, font/dependency containment, nested-prefab identity, root
defaults, missing-script absence, and successful prefab instantiation.

## Retained Runtime Evidence

Runtime Play Mode:

- Passed: `479`
- Failed: `0`
- Ignored: `0`

No retained Runtime behavior regressed.

## Manual Evidence

Confirmed in Unity:

- Both prefab assets appear under `Presentation.UGUI/Prefabs`.
- Unity compilation settled with zero errors and zero warnings.
- The temporary `Assets/FLM405Temp` folder disappeared.

A full final-art or multi-aspect prefab-mode review was not claimed.

## Evidence Not Yet Run

- Final branded project variant
- Manual 16:9 and tall-aspect layout review
- Editor setup/copy tooling
- Direct-scene initializer
- Boot scene generation
- Standalone Laboratory activation
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Exclusions Preserved

- Runtime prefab discovery or spawning
- Runtime production script changes
- Editor migration
- Setup/repair windows
- Direct-scene tooling
- Project configuration assets
- Project branding and fonts
- TextMeshPro integration
- Input bindings and EventSystem
- Safe-area runtime behavior
- Animation and video splash support
- Test Lab scenes
- Package version change

## Closure Result

FL-M4-05 implementation is complete in commit `8d3c6a7`.

All 27 focused EditMode tests pass.

All 479 retained Runtime Play Mode tests pass.

Compilation has 0 errors and 0 warnings.

The checkpoint is ready for its adjacent documentation closeout.

Tentative next checkpoint: FL-M5-01 - Editor Setup Foundation and
Non-Destructive Project Plan.
