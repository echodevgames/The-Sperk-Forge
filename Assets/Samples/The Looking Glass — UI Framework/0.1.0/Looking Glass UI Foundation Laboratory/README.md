# Looking Glass UI Foundation Laboratory

This is the engineering proof sample for The Looking Glass. EUI-M1-01 proved scoped Screen navigation plus an independent Window. EUI-M1-02 extends that same scene only enough to prove externally supplied active/inactive UI context, designer-ordered per-surface response, independent visibility/interaction/selection dimensions, and externally supplied input-aware selection.

The sample intentionally remains plain. It is not the future polished Reference Showcase.

## Existing hierarchy

```text
Canvas_MasterCanvas
├─ Panel_MenuRoot
│  ├─ Panel_MainMenu      [Screen: main-menu, scope: frontend]
│  │  ├─ Button_Settings
│  │  └─ Button_ToggleDefaultWindow
│  └─ Panel_SettingsMenu  [Screen: settings, scope: frontend]
│     └─ Button_Back
└─ Panel_WindowRoot
   └─ Panel_DefaultWindow [Window: default-window]
```

An EventSystem is present using the project's normal Unity UI path.

## EUI-M1-02 authored proof configuration

The scene deliberately uses ordinary `UISurface` Inspector data rather than a sample-only parallel policy system.

### `Panel_MainMenu`

- no external-context rules;
- pointer opening clears selection;
- navigation opening may select `Button_Settings`.

This surface is used to prove that an active context with no applicable rule causes no Looking Glass intervention.

### `Panel_SettingsMenu`

- contains a `pause` rule that would request Hidden / NonInteractable;
- **Allow External Context is disabled**, so that rule is intentionally ignored;
- pointer and navigation opening are configured to remain unselected.

This surface proves that external-context participation is a local designer choice.

### `Panel_DefaultWindow`

`Panel_DefaultWindow` has a `CanvasGroup` for interaction proof and the following designer-authored ordered rules:

1. `cinematic`
   - Visibility: Hidden
   - Interaction: No Change
   - Selection: No Change
2. `pause`
   - Visibility: Visible
   - Interaction: NonInteractable
   - Selection: No Change

Because resolution is per dimension, `pause + cinematic` resolves Hidden from the higher-priority cinematic rule while still resolving NonInteractable from the lower pause rule.

Selection policy:

- Pointer open: Clear Selection
- Navigation/controller open: Select Default
- Default target: `Panel_DefaultWindow` itself for this identity/selection proof; the panel intentionally carries no `Button` or navigation-button adapter component

## Laboratory-owned simulation console

`LaboratoryUIContextDriver` is attached to the root Canvas and draws a small IMGUI proof console in Play Mode. It is **sample-owned simulation only**. Foundry Laboratory proof/debug consoles reserve the **top-right safe zone** and stack downward in rows from there so authored sample UI can be designed around a predictable non-overlap region.

It can:

- toggle the example `pause` context;
- toggle the example `cinematic` context;
- supply Pointer or Navigation/Controller modality;
- open/close/toggle `default-window`;
- navigate to Settings and Back in the `frontend` scope;
- prime `Button_Settings` as prior selection for the neutral-close proof;
- display current context, visibility, interaction, screen, and EventSystem selection state.

The helper does not detect devices, own pause/cinematic truth, persist anything, or reference another Echo package.

## EUI-M1-02 manual acceptance

Enter Play Mode and use the proof console plus the existing uGUI controls.

1. **No-rule means no intervention.** With Main Menu current, toggle `pause` ON. `default-window` responds according to its authored rule, while `main-menu` remains visible because it has no pause rule.
2. **Designer order wins.** With `pause` ON, toggle `cinematic` ON. `default-window` becomes hidden because cinematic is authored above pause for Visibility.
3. **Per-dimension cascade.** While both are active, inspect the console: `default-window` is Hidden and NonInteractable. Cinematic supplied Visibility; pause supplied Interaction.
4. **External participation can be disabled.** Reset, navigate to Settings, then toggle `pause` ON. Settings remains visible even though it contains a pause Hide rule because Allow External Context is OFF.
5. **Visibility and interaction are separate.** Reset, open `default-window`, then toggle `pause` ON. The window remains visible but reports `interactable=False`.
6. **Pointer can open unselected.** Reset, choose Pointer, then Open Default Window. EventSystem selected should be `<none>`.
7. **Navigation/controller can select the configured default.** Close the window, choose Navigation/Controller, then Open Default Window. EventSystem selected should be `Panel_DefaultWindow`.
8. **Navigation/controller may also be designer-configured unselected.** Reset, choose Navigation/Controller, then Navigate Settings. EventSystem selected should remain `<none>`.
9. **Close is neutral; no historical restoration.** Reset. Use the existing Main Menu, click `Prime Prior Selection: Button_Settings`, choose Navigation/Controller, Open Default Window, then Close Default Window. The selected object becomes `<none>` rather than restoring `Button_Settings`.
10. **M1-01 remains intact.** Reset. Use Settings / Back to prove `main-menu -> settings -> Back -> main-menu`, then Toggle Default Window and confirm it coexists without replacing the current `frontend` screen.

Record each item Pass/Fail. Any failure stops EUI-M1-02 closeout.

## Project prerequisites and authoring notes

- The Laboratory uses the project's normal modern Unity UI workflow, including TextMesh Pro labels when Unity creates TMP-backed controls.
- If TMP-backed labels report missing resources, import **TMP Essential Resources**. **TMP Examples & Extras are not required.**
- Organizational roots such as `Panel_WindowRoot` remain active structural containers and should not block pointer input.
- `Panel_DefaultWindow` is a surface panel, not a button; do not bake a `Button` or `UINavigationButton` component onto that panel merely to give the Laboratory a selection target.
- Actual registered `UISurface` children own visibility.
- Context IDs `pause` and `cinematic` are sample conventions only. Looking Glass does not claim those domain truths.
- Do not turn this Laboratory into Motifs, Builder, a window-layout system, a peer-package bridge, or polished showcase content during EUI-M1-02.
