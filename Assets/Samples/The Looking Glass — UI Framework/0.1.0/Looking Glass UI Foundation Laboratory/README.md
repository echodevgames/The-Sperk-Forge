# Looking Glass UI Foundation Laboratory

This remains the deliberately plain engineering proof sample for **The Looking Glass (`EchoUI`)**. EUI-M1-01 proved scoped Screen navigation plus an independent Window. EUI-M1-02 added externally supplied active/inactive UI context, designer-ordered surface response, independent visibility/interaction/selection dimensions, and externally supplied input-aware selection.

**EUI-M2-01** extends the same Laboratory only enough to prove:

- project-defined, stable-ID, variable-count ordered UI layers;
- authoritative Screen lifecycle and history;
- `SceneOwned`, `RootOwned`, and `ExternalOwned` ownership semantics;
- designer-controlled suspension visibility while suspended Screens remain non-interactable;
- deterministic Push / Replace / Reset / Back / Close behavior;
- structured strict-FIFO Screen operation settlement;
- retained M1 Window/context/selection behavior.

This sample is intentionally not Motifs, Builder, a polished menu kit, a primitive-prefab warehouse, a transition system, or a Reference Showcase.

## Existing UI hierarchy

```text
CanvasMasterCanvas                 [EchoUIRoot + Laboratory proof console]
├─ Panel_MenuRoot                 [UILayerHost: primary-ui, order 20]
│  ├─ Panel_MainMenu              [Screen: main-menu, scope: frontend]
│  │  ├─ Button_Settings
│  │  └─ Button_ToggleDefaultWindow
│  └─ Panel_SettingsMenu          [Screen: settings, scope: frontend]
│     └─ Button_Back
└─ Panel_WindowRoot               [UILayerHost: floating-lab, order 80]
   └─ Panel_DefaultWindow         [Window: default-window]
      └─ Button_DefaultClose

Template_M2_RootOwnedScreen       [inactive sample-owned RootOwned source template]
External_M2_ProjectOwnedScreen    [inactive project-supplied ExternalOwned view]
```

The two M2 proof layer IDs are intentionally **not** package-reserved names. `primary-ui` and `floating-lab` prove that projects can author their own stable layer IDs and order. Looking Glass snapshots/validates them at lifecycle initialization; it does not rewrite the authoring data.

The top-right of the Game view remains the Foundry Laboratory proof/debug safe zone. The single proof console occupies that zone and grows downward.

## EUI-M2-01 proof definitions

The Laboratory-owned driver creates bounded runtime `UIScreenDefinition` snapshots from the scene-authored proof pieces after the normal M1 surface foundation initializes:

| Screen | Ownership | Layer | Suspension visibility |
|---|---|---|---|
| `main-menu` | SceneOwned | `primary-ui` | Visible |
| `settings` | SceneOwned | `primary-ui` | Hidden |
| `lab-root-owned` | RootOwned | `floating-lab` | Visible |
| `lab-external-owned` | ExternalOwned | `floating-lab` | Hidden |

`main-menu` and `settings` both carry a `CanvasGroup` so the Laboratory can visibly prove the M2 rule that a suspended Screen is non-interactable regardless of whether it remains visible.

### RootOwned proof source

`Template_M2_RootOwnedScreen` is an intentionally inactive sample source object outside the authoritative root hierarchy. It is supplied through the RootOwned definition's prefab/source slot only for this engineering proof. Looking Glass creates a runtime clone under `floating-lab`, registers it, and releases only that owned clone when its entry leaves history. The source template remains untouched.

This is intentionally not the beginning of the future package prefab/template warehouse.

### ExternalOwned proof source

`External_M2_ProjectOwnedScreen` also lives outside the authoritative root hierarchy. The Laboratory explicitly supplies/registers it after Screen lifecycle initialization. Looking Glass may activate/hide it while it participates in Screen history, but closing the Screen must leave the supplied GameObject alive because object lifetime remains external.

## Laboratory-owned proof console

Enter Play Mode. The top-right console now has two tabs:

- **M2 Screen Lifecycle** for this checkpoint;
- **M1 Retained Proof** for the previous context/selection/window contract.

The M2 tab displays:

- resolved authored layer order;
- current `frontend` Screen;
- history depth;
- queue depth;
- current ownership mode;
- Main Menu / Settings visibility and interactability;
- RootOwned source/runtime instance state;
- ExternalOwned supplied-object state;
- recent operation sequence IDs and terminal results;
- observed rapid-operation settlement order.

The helper owns no pause/cinematic truth, no input-device detection, no persistence, and no peer Echo package integration.

# EUI-M2-01 manual acceptance

Record each item **PASS / FAIL**. Any failure stops checkpoint closeout.

## 1. Project-defined ordered layers

Enter Play Mode and open **M2 Screen Lifecycle**.

Expected console state includes:

```text
Resolved layers: #0 primary-ui (order 20) -> #1 floating-lab (order 80)
M2 lifecycle initialized: True
Proof readiness: READY
```

PASS only if the non-default custom IDs resolve in the authored order. No fixed seven-layer topology is required.

## 2. SceneOwned Push and Back

Click:

```text
Reset Complete Laboratory Proof State
Push Settings
```

Expected:

```text
Current frontend Screen: settings
History depth: 2
Current ownership: SceneOwned
```

Then click `Back: frontend`.

Expected:

```text
Current frontend Screen: main-menu
History depth: 1
```

Neither scene-authored Screen is destroyed.

## 3. Suspension policy: prior Screen remains visible but non-interactable

Reset, then click `Push Settings`.

`main-menu` is authored with `SuspensionVisibility.Visible`.

Expected while Settings is current:

```text
main-menu: visible=True, interactable=False
settings: visible=True, interactable=True
```

Main Menu may remain visible, but it must not accept interaction while Settings owns the `frontend` scope.

## 4. Suspension policy: prior Screen hides

Leave Settings current from Check 3, then click `Push RootOwned`.

`settings` is authored with `SuspensionVisibility.Hidden`.

Expected:

```text
Current frontend Screen: lab-root-owned
settings: visible=False, interactable=False
RootOwned runtime instance: visible=True, ...
```

Click `Back: frontend` afterward to return to Settings, then Reset if desired.

## 5. Replace does not grow history

Reset, click `Push Settings` so history depth is 2, then click:

```text
Replace Top -> ExternalOwned
```

The operation log includes the measured depth transition.

Expected:

```text
Replace ExternalOwned [depth 2 -> 2]
Current frontend Screen: lab-external-owned
History depth: 2
```

Replace changes the top entry without appending another history entry.

## 6. Reset / Return-to-root clears prior history

From any non-root state with history depth greater than 1, click:

```text
Reset -> Main Menu
```

Expected:

```text
Current frontend Screen: main-menu
History depth: 1
main-menu: visible=True, interactable=True
```

Prior Screen history is gone rather than merely hidden underneath.

## 7. RootOwned create / close / release

Reset, then click:

```text
Push RootOwned
```

Expected:

```text
Current ownership: RootOwned
RootOwned template alive: YES
RootOwned runtime instance: visible=True, ...
```

Then click:

```text
Close RootOwned
```

On the following frame expected:

```text
RootOwned template alive: YES
RootOwned runtime instance: <released>
Current frontend Screen: main-menu
```

Only the runtime instance owned by Looking Glass is released. SceneOwned Main Menu remains intact.

## 8. ExternalOwned close preserves the supplied object

Reset, then click:

```text
Push ExternalOwned
```

Expected:

```text
Current ownership: ExternalOwned
ExternalOwned supplied object alive: YES
ExternalOwned active: YES
```

Then click:

```text
Close ExternalOwned
```

Expected:

```text
ExternalOwned supplied object alive: YES
ExternalOwned active: NO
```

Looking Glass may coordinate the view but must not destroy the externally owned object.

## 9. Rapid structural operations settle in FIFO submission order

Reset, then click:

```text
Run Rapid FIFO: Settings -> RootOwned -> Back
```

Expected `Observed` line shows three increasing operation sequence IDs in exactly this semantic order:

```text
Push Succeeded -> Push Succeeded -> Back Succeeded
```

The exact numeric sequence values depend on earlier operations, but they must increase left-to-right. Final state should be:

```text
Current frontend Screen: settings
History depth: 2
Queue depth: 0
```

The focused automated suite carries the delayed queue seam that proves accepted pending requests also settle FIFO across delayed processing. This manual check makes the public settlement sequence visible in the Laboratory.

## 10. Retained M1 behavior still works

Reset and switch to **M1 Retained Proof**.

Perform this compact retained proof:

1. Choose `Pointer`, open `default-window`: EventSystem selection is `<none>`.
2. Close it, choose `Navigation / Controller`, open it: selection becomes `Button_DefaultClose`.
3. With the window open, toggle `pause` ON: `default-window` remains visible and reports `interactable=False`; `main-menu` remains available because it has no pause interaction rule.
4. Toggle pause OFF.
5. Navigate Settings and Back: `main-menu -> settings -> Back -> main-menu` still behaves normally.
6. Toggle/open the independent `default-window` and verify it does not replace the current `frontend` Screen.

PASS only if the M2 lifecycle has not absorbed or regressed the M1 Window/context/selection behavior.

## Project prerequisites and authoring notes

- Unity baseline: 6000.3.8f1.
- The Laboratory uses the project's normal modern Unity UI workflow.
- If TMP-backed labels report missing resources, import **TMP Essential Resources**. TMP Examples & Extras are not required.
- Organizational roots stay active structural containers; actual `UISurface` children own surface visibility.
- `pause` and `cinematic` remain sample conventions for externally supplied context truth only.
- `primary-ui` and `floating-lab` are sample-authored layer IDs only, not reserved Looking Glass vocabulary.
- Do not expand this Laboratory into blocking modals, transition choreography, focus-history restoration, HUD/transient systems, Motifs, Builder, primitive-library content, persistence, or peer-package bridges during EUI-M2-01.
