# Looking Glass UI Foundation Laboratory

This sample deliberately contains **instructions rather than a finished pretty menu**. EUI-M1-01 is the first hand-authored engineering proof of Looking Glass behavior.

## Build the hierarchy

Create a new scene and use the suite naming convention:

```text
Canvas_MasterCanvas
├─ Panel_MenuRoot
│  ├─ Panel_MainMenu
│  │  ├─ Button_Settings
│  │  └─ Button_ToggleDefaultWindow
│  └─ Panel_SettingsMenu
│     └─ Button_Back
└─ Panel_WindowRoot
   └─ Panel_DefaultWindow
```

Create an EventSystem using the project's normal Unity UI path.

## Components

1. Add `EchoUIRoot` to `Canvas_MasterCanvas`.
2. Add `UISurface` to `Panel_MainMenu`:
   - Surface Id: `main-menu`
   - Role: `Screen`
   - Navigation Scope Id: `frontend`
   - Start Visible: enabled
3. Add `UISurface` to `Panel_SettingsMenu`:
   - Surface Id: `settings`
   - Role: `Screen`
   - Navigation Scope Id: `frontend`
   - Start Visible: disabled
4. Add `UISurface` to `Panel_DefaultWindow`:
   - Surface Id: `default-window`
   - Role: `Window`
   - Navigation Scope Id: leave blank
   - Start Visible: disabled

## Buttons

Each button keeps the normal Unity `Button` and adds `UINavigationButton`.

- `Button_Settings`: Action `Navigate To`, Target Surface Id `settings`
- `Button_Back`: Action `Back`, Target Scope Id `frontend`
- `Button_ToggleDefaultWindow`: Action `Toggle Surface`, Target Surface Id `default-window`

## Manual acceptance

In Play Mode:

1. Main Menu is visible and Settings is hidden.
2. Settings button opens Settings and hides Main Menu.
3. Back restores Main Menu.
4. Toggle Default Window opens the window without changing the current `frontend` screen.
5. Toggle it again and the window closes without changing the current screen.

Plain default Unity controls are correct. Do not polish this into the future showcase. Save the proof scene inside this imported sample folder; the Green Path closeout synchronizes the completed sample back to package-owned `Samples~`.

## Project prerequisites and authoring notes

- The Laboratory uses the project's normal modern Unity UI workflow, including TextMesh Pro labels when Unity creates TMP-backed controls.
- If TMP-backed labels report missing resources, import **TMP Essential Resources** for the consuming project. **TMP Examples & Extras are not required.**
- Organizational roots such as `Panel_WindowRoot` should remain active structural containers and should not block pointer input. A RectTransform-only root is ideal; if an Image is present, disable its Raycast Target unless the root is intentionally interactive.
- Actual visible surfaces such as `Panel_MainMenu`, `Panel_SettingsMenu`, and `Panel_DefaultWindow` may use Images sized for their intended presentation/input area.
- `UINavigationButton.Root Override` stays empty for this single-root Laboratory. `Navigate To` uses Target Surface Id, `Back` uses Target Scope Id, and `Toggle Surface` uses Target Surface Id.
