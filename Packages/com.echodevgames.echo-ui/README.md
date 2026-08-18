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

## EUI-M3-02 transition lifecycle

The completed M3-02 slice adds authoritative transition settlement for Screens, blocking Modals, and independent Windows through replaceable presentation-only drivers. It includes fresh generation-bound operations, unscaled hard-bounded timing, cancellation/stale protection, deterministic enter rollback and exit force-close recovery, root/default + definition + transient policy layers, Immediate/CanvasGroup Fade references, reduced-motion substitution seams, and retained focus/exact-once behavior.

Final retained evidence is **1246 / 1246** full EditMode, **140 / 140** EchoUI Editor, and **14 / 14** Laboratory at closeout `0affb7d`.

## EUI-M4-01 named HUD regions — complete

The completed M4-01 slice adds project-defined named HUD regions, bounded widget registration, fresh generation-safe widget and visibility leases, deterministic effective visibility, owner-loss/shutdown cleanup, status/events, and focused tests without entering Screen history or owning gameplay/domain truth.

Activation is `ce30ac6`; Runtime/tests are `df9e2be`; bounded corrections run through `e47d43b`; Laboratory implementation is `29573ef`; closeout is `5e7ad92`. Manual HUD Laboratory is **5 / 5 PASS**, retained smoke is green, and package/imported parity is verified.

## EUI-M4-02 bounded notifications — complete

The completed M4-02 slice adds project-defined independently bounded notification channels; priority/FIFO pending promotion without visible preemption; fresh-generation coalescing; deterministic pending-overflow policies; unscaled automatic and manual lifetime; generation-safe dismissal; owner/presentation loss, reset, and shutdown settlement; structured status/events; and a replaceable project presenter seam.

Activation is `fd8256f`; Runtime/root/presenter implementation is accepted through `d93d0bd`. The activation baseline was full Foundry EditMode **1258 / 1258** and EchoUI Editor **152 / 152**. Final automated evidence is full Foundry EditMode **1383 / 1383**, EchoUI Editor **277 / 277**, aggregate notification fixtures **125 / 125**, and presenter fixture **17 / 17**, with zero failed/skipped/inconclusive.

Mirrored Laboratory implementation is `bde34f2`. Manual Laboratory is **6 / 6 PASS**, the 180-frame idle probe is stable, retained M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke is green, and package/imported parity is verified. EUI-M4-02 is complete.

## EUI-M4-03 runtime Motifs — complete

The completed M4-03 slice adds a bounded Runtime Motif foundation:

- normalized stable `UIMotifId` and `UIMotifTokenId` values;
- immutable project-owned Motif definitions and detached snapshots;
- color, uGUI `Selectable` color-state, sprite, and small numeric/decorative token families;
- authored default/fallback resolution and one root-local effective session Motif;
- explicit bounded target registration with immediate application, generation-safe release, stale-handle safety, and destroyed-owner/target cleanup;
- reusable `UseMotif` / `KeepLocal` bindings;
- safe prior/local presentation for missing tokens;
- deterministic switch/reset/shutdown plus structured result truth;
- target/listener failure isolation after committed Motif truth;
- no scene-wide/per-frame target scan or recurring application loop.

Activation is `435fc66`. Runtime implementation proceeds through `d67550d`, `172d230`, `43da17a`, `efbc503`, `e17d816`, and root integration `ab5906c`. The test-only root teardown correction is `d291885` and changes no Runtime source.

The mirrored Laboratory proof landed at `b48eae68`. `7f9272bd` corrected the missing-token proof's registration expectation; `8188b91c` corrected unknown-fallback proof sequencing by resetting to the authored default before requesting the unknown ID. Neither correction changes Runtime authority or behavior.

Final accepted automated evidence from `TestResults_20260818_060619.xml` is full Foundry EditMode **1445 / 1445**, EchoUI Editor **339 / 339**, aggregate Motif fixtures **62 / 62**, root integration **12 / 12**, and zero failed/skipped/inconclusive.

Manual Motif Laboratory is **6 / 6 PASS**, including authored-asset immutability and **180-frame idle quiescence**. Retained M4-02/M4-01/M3-02/M3-01/M2-02/M2-01/M1 representative smoke is user-confirmed green. Package/imported Motif proof parity is verified.

Check 5 deliberately injects one broken target and therefore intentionally logs two caught target exceptions: one during immediate registration application and one during the following switch. These are isolation evidence, not failures.

EUI-M4-03 adds no hard peer Echo dependency, no mandatory TextMeshPro package dependency, no persistence authority, and no global accessibility/settings ownership.

**EUI-M4-03 is COMPLETE / CLOSED. No successor Looking Glass checkpoint is active.** The **Primitive Warehouse** is the named next direction only and requires separate activation.

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
