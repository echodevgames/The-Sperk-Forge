---
tags:
  - sfgss/checkpoint
  - sfgss/package/looking-glass
  - sfgss/implementation
status: complete
updated: 2026-08-13
---

# EUI-M1-01 — Looking Glass Installable Surface Foundation, Scoped Navigation, and Independent Window Proof

**Package:** The Looking Glass (`EchoUI`)
**Checkpoint:** EUI-M1-01
**Milestone:** M1 — Surface Foundation
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOUI-001 v1.1.0
**Workflow:** SFGSS-005 v1.6.0 + SFGSS-ADR-007 Green Path
**Learning prerequisite:** PKG-LEARN-008 — **Complete**
**Starting Git baseline:** `f57880a`
**Unity baseline:** 6000.3.8f1
**Verified Unity dependency:** `com.unity.ugui` 2.0.0

> Give the mirror one frame, two doors, and one window. Do not build the palace yet.

## 1. Observable outcome

A clean embedded `com.echodevgames.echo-ui` package installs and proves the smallest useful Looking Glass behavior without any peer Echo package:

```text
Canvas_MasterCanvas
├─ Panel_MenuRoot
│  ├─ Panel_MainMenu      [Screen: main-menu, scope: frontend]
│  └─ Panel_SettingsMenu  [Screen: settings, scope: frontend]
└─ Panel_WindowRoot
   └─ Panel_DefaultWindow [Window: default-window]
```

Manual behavior:

```text
main-menu
    -> settings
    -> Back
    -> main-menu

while default-window may open/close independently
without replacing the active frontend screen.
```

## 2. Authorized scope

- embedded package manifest and documentation shell;
- runtime/test asmdefs;
- package-local `EchoUIRoot` authority claim and orderly release;
- stable surface ID registry;
- initial surface roles: `Screen`, `Window`, `HUD`, `Overlay`;
- optional navigation scope identity for screens;
- one-current-screen-per-scope behavior;
- history-based Back;
- independent open/close/toggle operations;
- structured operation results;
- minimal uGUI `Button` navigation adapter;
- focused Editor tests for authority, validation, navigation, Back, failure non-mutation, and independent-window coexistence;
- minimal UPM sample instructions for the hand-authored Laboratory hierarchy;
- if the sample is imported and the proof scene is saved inside that imported sample, the Green Path closeout mirrors the finished sample back into package-owned `Samples~` before implementation commit.

## 3. Explicit exclusions

EUI-M1-01 does **not** authorize:

- pause/cinematic/loading context-provider APIs;
- visibility-rule evaluation;
- automatic input-modality detection or default-selection application;
- Motif schema/application/capture/local overrides;
- prefab primitive library beyond the runtime components required for this proof;
- Looking Glass Builder;
- modal, notification, tooltip, prompt, safe-area, transition, or full HUD service implementation;
- movable/resizable/persisted MMO window layouts;
- Chronicle/Accord/Resonance/First Light/Will/Controller bridges;
- automatic `DontDestroyOnLoad` or project-wide service composition;
- production menu art or a polished showcase.

## 4. Exact implementation file families

```text
Packages/com.echodevgames.echo-ui/
├─ package.json
├─ README.md
├─ CHANGELOG.md
├─ Documentation~/
│  ├─ Index.md
│  └─ Developer/Current Notes.md
├─ Runtime/
│  ├─ Core/EchoUIRoot.cs
│  ├─ Surfaces/UISurface.cs
│  ├─ Surfaces/UISurfaceRole.cs
│  ├─ Surfaces/UISurfaceOperationResult.cs
│  ├─ Surfaces/UISurfaceOperationStatus.cs
│  ├─ Navigation/UINavigationAction.cs
│  ├─ Navigation/UINavigationButton.cs
│  └─ EchoDevGames.EchoUI.Runtime.asmdef
├─ Tests/Editor/
│  ├─ EchoUIRootFoundationTests.cs
│  └─ EchoDevGames.EchoUI.Tests.Editor.asmdef
└─ Samples~/LookingGlass_UI_Foundation_Laboratory/README.md
```

Unity may create corresponding `.meta` files; they are checkpoint-owned and staged with the package. If the Package Manager sample is imported for manual proof, its project-owned `Assets/Samples/.../Looking Glass UI Foundation Laboratory/` copy is also checkpoint-owned for EUI-M1-01 and is synchronized back to `Samples~` before commit.

## 5. Implementation rules

1. Root duplicate rejection occurs before registry/navigation initialization.
2. Root does not call `DontDestroyOnLoad`.
3. Surface identity is stable project-authored data and does not derive authority from GameObject names.
4. A `Screen` requires a non-empty navigation scope.
5. `Window`, `HUD`, and `Overlay` do not require a navigation scope.
6. Only opening a `Screen` replaces the current screen in that same scope.
7. Opening/toggling an independent surface does not mutate screen history/current-screen state.
8. Failed requests do not mutate unrelated visible state/history.
9. Button adapter only translates a uGUI click into a Looking Glass request; it does not own input maps or domain commands.
10. Runtime assembly must not reference UnityEditor or another Echo package.

## 6. Focused tests

| Intent | Required result |
|---|---|
| First root authority | First root claims package-local authority |
| Duplicate root | Duplicate is non-authoritative and does not initialize |
| Duplicate surface ID | Initialization fails without accepting ambiguous registry state |
| Scoped Navigate To | `main-menu -> settings` hides only prior screen in `frontend` |
| Back | Restores `main-menu` and updates current screen |
| Independent window | `default-window` opens/closes while `settings` or `main-menu` remains current |
| Unknown surface | Structured failure and no unrelated mutation |

All focused tests must pass. If the full EditMode suite is run instead, `failed = 0` is still mandatory and the seven named EUI-M1-01 tests must all be present/passed.

## 7. Manual Laboratory proof

Build the simple hierarchy by hand using the approved `Type_DescriptiveName` convention. Wire real uGUI Buttons through `UINavigationButton`.

Required manual proof:

1. Play starts on `main-menu`.
2. Settings button opens `settings`; Main Menu is inactive.
3. Back returns to `main-menu`.
4. Open/toggle `default-window`; the current frontend screen does not change.
5. Close/toggle `default-window`; frontend screen remains unchanged.

No art polish is required. A plain Canvas/Panel/Button engineering harness is correct for M1-01. Save the proof scene inside the imported `Looking Glass UI Foundation Laboratory` sample if you want it retained; Green Path Phase 3 will mirror that finished imported sample back to package-owned `Samples~`.

## 8. Green Path execution

The supplied execution kit may:

1. apply/commit/push the authority activation;
2. apply implementation payload and stop for Unity;
3. validate exported NUnit XML + manual proof confirmation;
4. stage/commit/push implementation;
5. reconcile bounded closeout docs;
6. commit/push closeout;
7. verify HEAD/origin, no staged changes, and clean working tree.

Any failed gate stops. No helper may continue through a red test/manual proof/repository state.

## 9. Stop point

After EUI-M1-01 is green and closed out, stop. Do not begin Motifs or the Builder opportunistically.

## 10. Next recommended checkpoint

**EUI-M1-02 — External UI Context, Cascading Visibility Rules, and Input-Aware Selection Contract** is the likely next slice, but it is not activated by EUI-M1-01.

## EUI-M1-01 Closeout Evidence

- Implementation commit: `e6b651f`.
- Exported EditMode result: **1113 / 1113 passed, 0 failed**.
- All seven required EUI-M1-01 focused tests were present and passed.
- Manual direct-scene proof confirmed `main-menu -> settings -> Back -> main-menu` and independent `default-window` coexistence.
- Implementation stat:  50 files changed, 5530 insertions(+), 14 deletions(-).
- EUI-M1-01 is Complete. EUI-M1-02 is not activated.

## EUI-M1-01 Final Recovery Reconciliation

- Authority activation: `83d3f9e`.
- Implementation: `e6b651f` (`50 files changed, 5530 insertions(+), 14 deletions(-)`).
- Initial closeout commit: `4f94c3b`; the Green Path closeout generator encountered a documentation anchor mismatch after the implementation was safely pushed, so final reconciliation is intentionally adjacent rather than rewriting history.
- Full EditMode regression: **1113 / 1113 passed, 0 failed**.
- All seven required EUI-M1-01 focused tests passed.
- Five-item manual Laboratory acceptance passed.
- The authored Laboratory proved active/non-raycast organizational roots, exclusive `frontend` screen navigation, history Back, and independent-window coexistence.
- The project Laboratory uses standard TMP UI labels. TMP Essential Resources are retained as project infrastructure; TMP Examples & Extras are not a Looking Glass sample requirement.
- Test-result XML is evidence, not distributable sample content, and remains outside package `Samples~`.
- Green Path lessons from this first execution are recorded for later workflow hardening; no new runtime capability is authorized by this reconciliation.

**EUI-M1-01 is Complete. EUI-M1-02 remains not activated.**
