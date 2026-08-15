# Looking Glass UI Foundation Laboratory

This is the engineering proof sample for **The Looking Glass**. It remains deliberately plain.

- **EUI-M1-01** proved scoped Screen navigation and an independent Window.
- **EUI-M1-02** proved external UI context, ordered per-surface response, and input-aware selection.
- **EUI-M2-01** proved project-defined layers, authoritative Screen history/lifecycle, all three Screen ownership modes, suspension policy, and strict FIFO structural execution.
- **EUI-M2-02** adds only the blocking Modal lifecycle proof: stacked top-only interaction, exact-once results, structural aborts, all three ownership modes, Back policy, UI-scoped interaction blocking, Screen Reject/Defer behavior, and explicit proof that gameplay/project behavior remains external.

The sample is not a polished Reference Showcase, Motif system, Builder, general focus manager, transition system, HUD framework, or MMO window-layout manager.

## Proof hierarchy

```text
CanvasMasterCanvas                 [EchoUIRoot + Laboratory proof console]
├─ Panel_MenuRoot                 [UILayerHost: primary-ui, order 20]
│  ├─ Panel_MainMenu              [Screen: main-menu, scope: frontend]
│  └─ Panel_SettingsMenu          [Screen: settings, scope: frontend]
└─ Panel_WindowRoot               [UILayerHost: floating-lab, order 80]
   ├─ Panel_DefaultWindow         [independent Window: default-window]
   └─ Panel_M2_02_SceneConfirmModal
                                  [SceneOwned Modal: lab-modal-confirm]

Template_M2_RootOwnedScreen       [retained M2-01 Screen source]
External_M2_ProjectOwnedScreen    [retained M2-01 Screen external view]
Template_M2_02_RootOwnedModal     [inactive RootOwned Modal source]
External_M2_02_ProjectOwnedModal  [inactive ExternalOwned Modal view]
```

The top-right of the Game view remains the Laboratory/debug safe zone. The proof console grows downward from there.

## M2-02 Modal definitions

The sample initializes three project-neutral stable-ID Modal definitions:

| Modal ID | Ownership | Back policy | Purpose |
|---|---|---|---|
| `lab-modal-confirm` | SceneOwned | completes with `cancel` | normal confirmation / stable result / Back proof |
| `lab-modal-root` | RootOwned | disabled | nested top-only / RootOwned lifetime proof |
| `lab-modal-external` | ExternalOwned | completes with `cancel` | external owner-loss / lifetime proof |

Semantic completion uses project-defined IDs such as:

```text
confirm
cancel
```

`Aborted` remains a structural outcome and is never rewritten into semantic `cancel`.

## Choosing the Screen-mutation policy

EUI-M2-02 exposes two initialization policies. A Modal lifecycle is initialized once per root, so the Laboratory deliberately asks you to choose one when Play Mode starts.

For checks **1-9, 11, and 12**, choose:

```text
Initialize: Reject
```

For check **10**, leave Play Mode, re-enter Play Mode, and choose:

```text
Initialize: Defer
```

This is intentional proof of two project initialization choices, not a production runtime policy toggle.

# EUI-M2-02 manual acceptance

Record every item **PASS / FAIL**. Any failed item stops closeout.

## 1. Open blocking Modal

Enter Play Mode.

On **M2-02 Modals**:

```text
Initialize: Reject
Reset M2-02 Proof State
Open Scene Confirm
```

Expected:

```text
Active Modal count: 1
Top Modal: lab-modal-confirm
SceneOwned Modal: visible=True, interactable=True, blocksRaycasts=True
main-menu: visible=True, interactable=False, blocksRaycasts=False
```

Try clicking the ordinary Settings button underneath. The current `frontend` Screen must remain `main-menu`.

The proof console itself is sample-owned IMGUI evidence tooling and intentionally remains usable.

## 2. Stable result ID

With `lab-modal-confirm` open, click:

```text
Complete: confirm
```

Expected terminal summary includes:

```text
outcome=Completed
resultId=confirm
```

The value is the exact project-authored stable ID.

## 3. Exact once

Immediately click:

```text
Complete Again: cancel
```

Expected attempt status:

```text
AlreadyCompleted
```

The terminal summary must still report:

```text
resultId=confirm
```

No second terminal result replaces the first.

## 4. Nested top-only interaction

Reset, then:

```text
Open Scene Confirm
Open Root Modal (Back Disabled)
```

Expected:

```text
Active Modal count: 2
Top Modal: lab-modal-root
SceneOwned Modal: interactable=False, blocksRaycasts=False
RootOwned Modal runtime: visible=True, interactable=True, blocksRaycasts=True
```

Only the top Modal owns normal Looking Glass UI interaction.

## 5. Out-of-order lower cleanup

Leave the two-Modals state from Check 4 and click:

```text
Abort Lower Scene Handle
```

Expected:

```text
Active Modal count: 1
Top Modal: lab-modal-root
RootOwned Modal runtime: ... interactable=True ...
```

The lower generation may settle out of order without stealing or corrupting the top interaction frontier.

Then click `Complete Root: confirm` to clean up.

## 6. Back policy

### Dismissible Modal

Reset, click:

```text
Open Dismissible Scene
Back on Top Modal
```

Expected terminal result:

```text
outcome=Completed
resultId=cancel
```

### Non-dismissible Modal

Reset, click:

```text
Open Non-dismissible Root
Back on Top Modal
```

Expected attempt status:

```text
BackDisabled
```

The Modal remains active until you click `Complete Root: confirm` or Reset.

## 7. Structural abort

Reset, then:

```text
Open External Modal
Simulate External Owner Loss
```

Expected terminal summary:

```text
outcome=Aborted
abortReason=OwnerLost
```

It must **not** report semantic `cancel`.

The external GameObject remains alive because Looking Glass does not own its lifetime.

## 8. Ownership lifetime

Use Reset between subchecks as needed.

### SceneOwned

```text
Open Scene Confirm
Complete: confirm
```

Expected afterward:

```text
SceneOwned Modal: <object still present / inactive>
```

The scene object is not destroyed.

### RootOwned

```text
Open Root Modal (Back Disabled)
Complete Root: confirm
```

On the following frame expected:

```text
RootOwned Modal template alive: YES
RootOwned Modal runtime: <released>
```

Only the Looking-Glass-owned runtime clone is released.

### ExternalOwned

If Check 7 unregistered the external view, Reset will re-register it.

```text
Open External Modal
Complete External: confirm
```

Expected afterward:

```text
ExternalOwned Modal object alive: YES
ExternalOwned Modal active: NO
```

The supplied object survives settlement.

## 9. Screen Reject policy

This check requires the Play Mode session initialized with:

```text
Initialize: Reject
```

Reset, open `Scene Confirm`, then click:

```text
Request Push Settings (expect BlockedByModal)
```

Expected:

```text
Screen mutation observed: ... BlockedByModal
Current frontend Screen: main-menu
History depth: 1
```

No Screen-history mutation occurs under the active Modal.

## 10. Screen Defer policy / FIFO

Exit Play Mode and re-enter.

Choose:

```text
Initialize: Defer
```

Then:

```text
Reset M2-02 Proof State
Open Scene Confirm
Queue Deferred: Settings -> RootOwned Screen
```

Before settling the Modal, expected:

```text
Deferred Screen queue depth: 2
... Push Pending -> ... Push Pending
Current frontend Screen: main-menu
```

Now click:

```text
Complete: confirm
```

Expected after the Modal stack clears:

```text
Deferred Screen queue depth: 0
first request:  Push Succeeded
second request: Push Succeeded
Current frontend Screen: lab-root-owned
History depth: 3
```

The two accepted Screen requests execute only after the blocking stack is empty and preserve their original FIFO submission order.

## 11. Gameplay / project-input separation

In either policy session:

```text
Reset M2-02 Proof State
Open Scene Confirm
Trigger External Project Action (+1)
```

Expected:

```text
main-menu: interactable=False, blocksRaycasts=False
External project action count: 1
```

This sample button stands in for external project/gameplay behavior. It proves Looking Glass blocks lower Looking Glass UI without seizing pause, simulation, WASD/action-map, or project gameplay authority.

## 12. Retained behavior

### M2-01 Screens tab

Reconfirm a compact subset:

1. `Reset Complete Laboratory Proof State`
2. `Push Settings`
3. verify `main-menu` is visible but non-interactable while Settings is current;
4. `Back: frontend` returns to Main Menu;
5. `Push RootOwned` and `Close RootOwned` still create/release the runtime Screen instance.

### M1 Retained tab

Reconfirm:

1. Pointer-opened `default-window` is unselected.
2. Navigation/controller-opened `default-window` selects `Button_DefaultClose`.
3. `pause` can leave the Window visible while making it non-interactable.
4. Settings / Back still performs `main-menu -> settings -> main-menu`.
5. The independent `default-window` coexists without replacing the current `frontend` Screen.

PASS only if the Modal lifecycle has not absorbed or regressed the prior Screen/Window/context/selection contracts.

## Notes

- `lab-modal-*` IDs are sample conventions only.
- The Laboratory does not detect devices or own gameplay input.
- The Laboratory does not set pause/time scale or cursor policy.
- The project-defined `primary-ui` / `floating-lab` layer IDs remain ordinary sample-authored IDs.
- Blocking Modal semantics do **not** redefine independent Window behavior.
- Future independent Window most-recent-eligible LIFO Back/Escape, pin/lock behavior, dragging/resizing, and layout persistence remain later work.
- Visual polish, generalized dim/blur, transitions, Motifs, Builder, HUD/transients, persistence, peer-package bridges, and production showcase content remain outside EUI-M2-02.

## EUI-M2-02 manual acceptance record — 2026-08-14

The complete EUI-M2-02 Laboratory acceptance run passed:

- Check 1 — Open blocking Modal: **PASS**
- Check 2 — Stable result ID: **PASS**
- Check 3 — Exact once: **PASS**
- Check 4 — Nested top-only interaction: **PASS**
- Check 5 — Out-of-order lower cleanup: **PASS**
- Check 6 — Back policy: **PASS**
- Check 7 — Structural abort: **PASS**
- Check 8 — Ownership lifetime: **PASS**
- Check 9 — Screen Reject policy: **PASS**
- Check 10 — Screen Defer policy / FIFO: **PASS**
- Check 11 — Gameplay / project-input separation: **PASS**
- Check 12 — Retained behavior: **PASS**
- Retained M2-01 Screens tab: **PASS**
- Retained M1 tab: **PASS**

Automated evidence immediately preceding the manual run:

- focused EUI-M2-02 Modal lifecycle: **28 / 28 passed**
- EchoUI EditMode assembly: **75 / 75 passed**
- full Foundry EditMode regression: **1181 / 1181 passed**

## Laboratory readability palette

The Laboratory may use a small project/sample-owned visual palette to improve readability. These colors are **not Looking Glass Runtime defaults** and do not constrain consumer projects, future Motifs, or Builder tooling.

Current sample palette direction:

```text
Glass / panel fill:       #0C0015, alpha 100
Primary fluorescent text: #00CF87, alpha 255

Button Normal:            #003D97, alpha 100
Button Highlighted:       #0091CF, alpha 100
Button Pressed:           #002A67, alpha 100
Button Selected:          #0091CF, alpha 100
Button Disabled:          #3A0005, alpha 100
Button text:              #00CF87, alpha 255
```

Authoring cautions:

- labels and decorative border graphics should have **Raycast Target disabled**;
- structural roots such as `Panel_MenuRoot` must not become accidental pointer blockers;
- translucent parent and child fills visually accumulate, so author the desired glass opacity intentionally;
- button target graphics should avoid unintended alpha multiplication between their base Graphic color and Color Tint state colors;
- fluorescent borders should be authored as separate border graphics rather than relying on Unity's `Outline` effect over a translucent fill.
