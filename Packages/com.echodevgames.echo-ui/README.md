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

EUI-M1-01 intentionally stopped before external-context response and input-aware selection; those bounded capabilities are now supplied by EUI-M1-02. Motifs, broad primitive-prefab libraries, the Looking Glass Builder, richer modal/notification/HUD systems, persistence, and peer-package bridges remain future work.

## EUI-M1-02 context and selection

The second bounded implementation adds:

- project-defined stable active/inactive UI context IDs;
- multiple simultaneous contexts;
- designer-ordered per-surface response rules;
- independent visibility, interaction, and selection/focus response dimensions;
- no-intervention behavior when no applicable rule supplies a dimension;
- optional per-surface participation in external-context handling;
- transient runtime overrides that do not mutate authored definitions or claim persistence;
- externally supplied pointer versus navigation/controller modality;
- per-surface selection-on-open policy, including pointer-unselected and configured default-selection behavior;
- neutral temporary-surface close behavior without implicit historical selection restoration.

Pause, cinematic, loading, and input modality remain external project/system truth. Looking Glass only decides how each surface responds to supplied truth.

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
