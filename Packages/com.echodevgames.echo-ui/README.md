# The Looking Glass — UI Framework

The Looking Glass is EchoDevGames' reusable uGUI presentation framework and UI construction toolkit.

## EUI-M1-01 foundation

The first implementation proves:

- one package-local `EchoUIRoot` authority;
- stable project-authored surface IDs;
- `Screen`, `Window`, `HUD`, and `Overlay` roles;
- one active Screen per navigation scope;
- history-based Back;
- independent Window open/close/toggle behavior;
- a thin uGUI `UINavigationButton` adapter.

It intentionally does **not** yet implement context visibility rules, input-aware default focus, Motifs, primitive prefabs, the Looking Glass Builder, modals, notifications, or peer-package bridges.

## Hierarchy convention

Looking Glass samples and authoring tools use `Type_DescriptiveName`:

```text
Canvas_MasterCanvas
├─ Panel_MenuRoot
│  ├─ Panel_MainMenu
│  └─ Panel_SettingsMenu
└─ Panel_WindowRoot
   └─ Panel_DefaultWindow
```

Runtime stable IDs remain separate from GameObject names.

## Package boundary

Looking Glass coordinates UI presentation. It does not decide that the game is paused/cinematic/loading, own input maps, write saves/preferences, load scenes, play audio, or create project-wide `DontDestroyOnLoad` composition.
