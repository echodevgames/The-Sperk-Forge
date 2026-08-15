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


## EUI-M2-01 screen lifecycle runtime core

The first Runtime Core slice adds:

- stable project-defined layer IDs and variable ordered layer topology with no fixed runtime layer count;
- immutable authored Screen definitions separated from mutable runtime entries/history;
- explicit `RootOwned`, `SceneOwned`, and `ExternalOwned` Screen view ownership;
- designer-controlled suspended-Screen visibility with scope-enforced noninteraction while another Screen is top;
- deterministic Push/Navigate, Replace, Reset/Return-to-root, Back, and Close operations;
- bounded strict-FIFO structural Screen mutation with explicit overflow/invalid rejection;
- structured terminal operation results and preflight behavior that avoids partial history/ownership mutation;
- safe RootOwned release without taking lifetime authority over SceneOwned or ExternalOwned views.

The Laboratory proves custom layer order, both suspension visibility styles, all three ownership modes, Screen history operations, visible FIFO settlement order, and retained M1 context/selection/independent-window behavior.

EUI-M2-01 does not implement modal exact-once results, transitions, general focus-history restoration, full EventSystem policy, HUD/transient services, Motifs, Builder tooling, primitive-library expansion, persistence, peer-package bridges, or project-wide lifetime composition. Those remain separately gated future work.

## EUI-M2-02 blocking modal runtime core

The second Runtime Core slice adds:

- deterministic stacked blocking Modals with top-only Looking Glass interaction;
- project-defined stable Modal and result IDs;
- fresh per-opening handles with first-terminal-wins exact-once settlement;
- structural `Aborted` outcomes distinct from semantic project cancellation;
- `RootOwned`, `SceneOwned`, and `ExternalOwned` Modal lifetime behavior;
- designer-authored Back dismissal policy;
- lower Looking Glass UI interaction blocking without claiming gameplay input, pause/time-scale, cursor, or simulation authority;
- explicit Screen mutation `Reject` and bounded `DeferUntilModalStackClears` policies;
- retained FIFO execution for accepted/deferred Screen structural work.

Blocking Modal semantics do not redefine ordinary independent Windows. Coexistent inventory, character, crafting, quest, tool-palette, or similar Window surfaces remain a separate non-blocking design space. Future most-recent-eligible Window Back/Escape dismissal, pin/lock behavior, dragging/resizing, and layout persistence remain separately gated.

The Laboratory proves the full Modal lifecycle in 12 manual checks while retaining the M2-01 Screen and M1 Window/context/selection contracts.

EUI-M2-02 does not implement transitions, generalized dim/blur/backdrop services, full focus/EventSystem policy, HUD/transients, Motifs, Builder tooling, primitive-library expansion, arbitrary modal domain payload transport, persistence, peer-package bridges, or project-wide lifetime composition.

## EUI-M3-01 EventSystem and focus lifecycle

The first Focus and Presentation slice adds:
- explicit non-destructive EventSystem coordination through `AdoptAssigned`, deterministic `AdoptExisting`, `CreateIfMissing`, and `RequireExternal`;
- structured degraded behavior when multiple eligible EventSystems make adoption ambiguous;
- per-live-entry focus memory plus optional transient root-session stable-surface memory;
- designer-selectable fresh versus `RememberThisSession` reopening;
- deterministic focus resolution through explicit target, remembered target, authored default, entry resolver, global fallback, or legal no-focus;
- Screen Back/resume and Modal completion restoration where policy allows;
- structural focus containment inside the top blocking Modal while lower-entry memory survives;
- distinct independent Window focus memory without introducing z-order/LIFO/pin/layout management;
- event-driven focus maintenance, explicit revalidation, and stale-generation protection.

Input modality remains project-owned and externally supplied. Pointer policy may intentionally leave selection at `<none>`, while navigation/controller policy may establish an authored target. Looking Glass does not own gameplay action maps, device detection, pause/time-scale, cursor policy, or the generated `InputSystem_Actions` wrapper.

The Laboratory proves the EventSystem/focus lifecycle in 12 manual checks, retains the M2/M1 proof tabs, and records bounded idle evidence that focus coordination does not require a universal per-frame scan.

EUI-M3-01 does not implement transitions, Motifs/accessibility presentation, HUD/transients, the full independent-Window LIFO/pinning/drag/layout manager, persistence, Builder tooling, primitive/9-slice warehouse work, peer bridges, or polished Reference Showcase art. Those remain separately gated future work.

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
