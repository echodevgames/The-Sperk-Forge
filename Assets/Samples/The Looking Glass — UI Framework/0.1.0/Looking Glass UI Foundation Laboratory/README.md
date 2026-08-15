# Looking Glass UI Foundation Laboratory

This is the engineering proof sample for **The Looking Glass**. It remains deliberately plain.

- **EUI-M1-01** proved scoped Screen navigation and an independent Window.
- **EUI-M1-02** proved external UI context, ordered per-surface response, and input-aware selection.
- **EUI-M2-01** proved project-defined layers, authoritative Screen history/lifecycle, all three Screen ownership modes, suspension policy, and strict FIFO structural execution.
- **EUI-M2-02** proves the blocking Modal lifecycle: stacked top-only interaction, exact-once results, structural aborts, all three ownership modes, Back policy, UI-scoped interaction blocking, Screen Reject/Defer behavior, and explicit proof that gameplay/project behavior remains external.
- **EUI-M3-01** adds explicit/non-destructive EventSystem coordination, live/session focus memory, deterministic restoration/fallback, blocking-Modal focus containment, explicit revalidation, and event-driven idle behavior.

The sample is not a polished Reference Showcase, Motif system, Builder, transition system, HUD framework, gameplay-input owner, or MMO window-layout manager.

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

# EUI-M3-01 manual acceptance

The **M3-01 Focus** tab is Laboratory-owned proof infrastructure. Its buttons deliberately simulate project/designer calls into Looking Glass. They are not production input ownership, device detection, gameplay action-map switching, or an endorsement of polling.

Before a fresh acceptance run:

1. open `The Looking Glass_UI_Laboratory`;
2. enter Play Mode;
3. select **M3-01 Focus**;
4. click `Prepare M3 Baseline`;
5. verify the console reports:
   - `Focus coordination: Ready`;
   - `Active EventSystems: 1`;
   - `Current frontend Screen: main-menu`;
   - `default-window reopen policy: RememberThisSession`.

Run the checks below in order. `Prepare M3 Baseline` is safe to use between checks unless a check explicitly asks you to observe a degraded state first.

## M3-01 Check 1 — AdoptAssigned

Click:

`Run Check 1: Adopt Assigned Scene EventSystem`

PASS when the observation reports `Ready`, the assigned scene EventSystem remains alive, and Looking Glass reports that it adopted the explicitly assigned EventSystem.

[CHECK 1] expected Ready + assigned scene EventSystem. Observed: status=Ready, assignedAlive=YES, EventSystems=1, message=Looking Glass adopted the explicitly assigned EventSystem

## M3-01 Check 2 — distinct EventSystem coordination modes

Run all three buttons separately:

- `2A AdoptExisting`
- `2B CreateIfMissing`
- `2C RequireExternal`

Expected:

- **AdoptExisting:** one active scene EventSystem is adopted and status is `Ready`.
- **CreateIfMissing:** the scene EventSystem is temporarily disabled; Looking Glass creates one `EchoUI EventSystem` because creation was explicitly requested.
- **RequireExternal:** with the scene system temporarily disabled, Looking Glass creates nothing and reports the missing/degraded condition.

Click `Prepare M3 Baseline` after 2B/2C to restore the authored scene EventSystem.

[Check 2]
CHECK 2C expected Missing/degraded and zero created EventSystems. Observed: status=Missing, activeEventSystems=0, created=EchoUI EventSystem, operationSucceeded=False, message=RequireExternal found no active eligible EventSystem and will not create one.. Click Prepare M3 Baseline before the next check


[Check 2]2
CHECK 2A expected Ready by adopting the one existing scene EventSystem. Observed: status=Ready, EventSystems=1, message=Looking Glass adopted the unambiguous existing EventSystem.
CHECK 2B expected Ready + one Looking Glass-created EventSystem because none existed. Observed: status=Ready, created=EchoUI EventSystem, activeEventSystems=1, sceneEventSystemActive=NO, message=Looking Glass created a root-owned EventSystem because CreateIfMissing was explicitly configured.. Click Prepare M3 Baseline before the next check.
CHECK 2C expected Missing/degraded and zero created EventSystems. Observed: status=Missing, activeEventSystems=0, created=<none>, operationSucceeded=False, message=RequireExternal found no active eligible EventSystem and will not create one.. Click Prepare M3 Baseline before the next check.

## M3-01 Check 3 — ambiguous multiple EventSystems

Click:

`Run Check 3: Create Ambiguity`

PASS when:

- two active EventSystems are visibly reported;
- focus coordination reports `Ambiguous`;
- both the scene EventSystem and the Laboratory-created extra EventSystem remain alive;
- Looking Glass does not silently choose or delete one.

[Check 3]
CHECK 3 expected Ambiguous/degraded, both EventSystems still alive, and no arbitrary adoption. Observed: status=Ambiguous, activeEventSystems=2, sceneAlive=YES, extraAlive=YES, operationSucceeded=False, message=Multiple active eligible EventSystems were found. Looking Glass will not choose an arbitrary winner

Then click:

`Cleanup / Restore One EventSystem`
[Check 3 Cont]
Baseline: status=Ready, EventSystems=1, selected=<none>

## M3-01 Check 4 — Modal close restores lower remembered focus

Click:

`Run Check 4: Modal Restore`

The helper primes `M3_MainMenu_RememberedTarget`, opens the SceneOwned blocking Modal, explicitly focuses the Modal, then completes it.

PASS when the observation shows:

- `before=M3_MainMenu_RememberedTarget`;
- `during=Panel_M2_02_SceneConfirmModal`;
- `after=M3_MainMenu_RememberedTarget`;
- the Modal completion succeeded.

CHECK 4 expected lower focus remembered,
Modal owns focus while open,
then lower focus restored.
Observed before=M3_MainMenu_RememberedTarget,
during=Panel_M2_02_SceneConfirmModal,
after=M3_MainMenu_RememberedTarget,
prime=Succeeded, completion=Succeeded,



## M3-01 Check 5 — Screen Back restoration

Click:

`Run Check 5: Screen Back Restore`

PASS when Settings is pushed over Main Menu, Back returns to `main-menu`, and the final selected object is `M3_MainMenu_RememberedTarget`.

[Check 5]
CHECK 5 expected Back to expose main-menu and restore its remembered target. Observed before=M3_MainMenu_RememberedTarget, settings=<none>, currentScreen=main-menu, after=M3_MainMenu_RememberedTarget, expectedAfter=M3_MainMenu_RememberedTarget



## M3-01 Check 6 — Fresh reopen ignores old memory

Click:

`Run Check 6: Fresh Reopen`

Settings is deliberately authored `Fresh`.

PASS when the first focus is `M3_Settings_FreshTarget`, then reopening Settings does **not** restore that old alternate target. The reopened state follows Settings' ordinary authored opening policy instead.
[Check 6]
CHECK 5 expected Back to expose main-menu and restore its remembered target. Observed before=M3_MainMenu_RememberedTarget, settings=<none>, currentScreen=main-menu, after=M3_MainMenu_RememberedTarget, expectedAfter=M3_MainMenu_RememberedTarget

[Check 6]2
CHECK 6 expected Fresh reopen to ignore the old alternate target and use Settings' authored opening policy. Observed primed=M3_Settings_FreshTarget, reopened=<none>, oldAlternate=M3_Settings_FreshTarget, ignoredOldMemory=YES

## M3-01 Check 7 — RememberThisSession reopen

The Laboratory authors `default-window` as:

`RememberThisSession`

Click:

`Run Check 7: Session Reopen`

PASS when the helper focuses `M3_DefaultWindow_SessionTarget`, closes the Window, reopens it, and the same target is restored.

[Check 7]
CHECK 7 expected RememberThisSession to restore the alternate stable-surface target. Observed primed=M3_DefaultWindow_SessionTarget, reopened=Button_DefaultClose, expected=M3_DefaultWindow_SessionTarget


[Check 7]2
CHECK 7 PASS expected RememberThisSession to restore the alternate stable-surface target. Observed policy=RememberThisSession, primed=M3_DefaultWindow_SessionTarget, reopened=M3_DefaultWindow_SessionTarget, expected=M3_DefaultWindow_SessionTarget
## M3-01 Check 8 — invalid remembered target fallback

Click:

`Run Check 8: Invalidate Remembered Target`

The helper stores the session target, closes the Window, disables that remembered target, then reopens the Window.

PASS when the invalid target is skipped and focus falls through to `Button_DefaultClose`. A legal `<none>` remains structurally allowed in policies that author no fallback, but the current Laboratory authors `Button_DefaultClose`.
[Check 8]
CHECK 8 expected invalid remembered target to fall through to Button_DefaultClose or legal <none>. Observed reopened=Button_DefaultClose, authoredDefault=Button_DefaultClose


[Check 8]2
CHECK 8 expected invalid remembered target to fall through to Button_DefaultClose or legal <none>. Observed reopened=Button_DefaultClose, authoredDefault=Button_DefaultClose
## M3-01 Check 9 — pointer no-focus / no jitter

Click:

`Run Check 9: Pointer <none> / 60-Frame Stability`

PASS when:

- pointer-opened `default-window` begins at `<none>`;
- it remains `<none>` across the 60-frame idle observation;
- focus generation remains unchanged for that idle period.

This proves the sample does not invent focus merely because time passes.
[Check 9]
CHECK 9 expected pointer-opened default-window to remain <none> without idle focus jitter. Observed initial=<none>, after60Frames=<none>, generation=144 -> 144, stable=YES

## M3-01 Check 10 — navigation establishes default

Click:

`Run Check 10: Navigation Default`

PASS when navigation/controller modality opens `default-window` with `Button_DefaultClose` selected.


[Check 10]
CHECK 10 expected Navigation/controller policy to select Button_DefaultClose. Observed selected=Button_DefaultClose, expected=Button_DefaultClose

## M3-01 Check 11 — blocking Modal containment

Click:

`Run Check 11: Modal Containment`

The helper first establishes legal Modal focus, then deliberately forces EventSystem selection into lower Main Menu UI and calls explicit focus revalidation.

PASS when the lower target does not remain selected after repair. The repaired result may restore the Modal's remembered target or resolve to legal `<none>`.

[Check 11]
CHECK 11 expected forced lower-UI focus to be repaired back inside the top Modal or to legal <none>. Observed legalModalFocus=Panel_M2_02_SceneConfirmModal, forcedLower=M3_MainMenu_RememberedTarget, repaired=Panel_M2_02_SceneConfirmModal, revalidation=Succeeded, escapedLowerAfterRepair=NO

## M3-01 Check 12 — explicit dynamic revalidation + retained smoke

Click:

`Run Check 12: Revalidation + Smoke`

The helper:

1. focuses a dynamic alternate target in `default-window`;
2. disables that target;
3. explicitly calls Looking Glass focus revalidation;
4. verifies fallback to the authored Window target;
5. runs a compact retained Screen push/back, pause-context, and Modal exact-once smoke sequence.

PASS when:

- repaired focus is `Button_DefaultClose`;
- `retainedSmoke=PASS`;
- final frontend Screen is `main-menu`.

Afterward, briefly visit **M2-02 Modals**, **M2-01 Screens**, and **M1 Retained** and confirm their existing proof controls still look/behave normal.

[Check12]
CHECK 12 expected explicit revalidation to repair disabled dynamic focus, then retained Screen/context/Modal smoke to remain healthy. Observed repairedFocus=Button_DefaultClose, expectedFallback=Button_DefaultClose, revalidation=Succeeded, pushAccepted=True, backAccepted=True, pauseObserved=True, firstModalCompletion=Succeeded

[Check12]2
CHECK 12 retainedSmoke=PASS, repairedFocus=Button_DefaultClose, expectedFallback=Button_DefaultClose, revalidation=Succeeded, finalScreen=main-menu. Details: pushAccepted=True, backAccepted=True, pauseObserved=True, firstModalCompletion=Succeeded, secondModalCompletion=AlreadyCompleted


# EUI-M3-01 bounded performance evidence

Still on **M3-01 Focus**, click:

`Run 180-Frame Idle Focus Probe`

Do not interact with the UI during the probe.

PASS when the performance line reports:

- approximately 180 idle frames observed;
- focus generation remained `STABLE`;
- explicit revalidation completed synchronously afterward.

This Laboratory evidence is paired with the focused automated proof that `UISelectionCoordinator` has no `Update` or `LateUpdate` loop. Together they demonstrate event-driven idle focus behavior without adding a hidden universal polling path.

# EUI-M3-01 acceptance record

Leave this record unmodified until the manual run is actually complete.

- Check 1 — AdoptAssigned: **PASS**
- Check 2 — AdoptExisting / CreateIfMissing / RequireExternal: **PASS**
- Check 3 — Multiple EventSystems degrade safely: **PASS**
- Check 4 — Modal restore: **PASS**
- Check 5 — Screen Back restore: **PASS**
- Check 6 — Fresh reopen: **PASS**
- Check 7 — RememberThisSession reopen: **PASS**
- Check 8 — Invalid remembered fallback: **PASS**
- Check 9 — Pointer no-focus/no jitter: **PASS**
- Check 10 — Navigation default: **PASS**
- Check 11 — Modal focus containment: **PASS**
- Check 12 — Explicit revalidation + retained smoke: **PASS**
- Bounded performance evidence: **PASS**

Automated evidence immediately preceding this Laboratory phase:

- focused EUI-M3-01: **24 / 24 passed**
- EchoUI EditMode assembly: **99 / 99 passed**
- full Foundry EditMode regression: **1205 / 1205 passed**

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
- EchoUI EditMode assembly: **99 / 99 passed**
- full Foundry EditMode regression: **1205 / 1205 passed**

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

## EUI-M3-01 implementation-seal evidence
- EUI-M3-01 manual acceptance: **12 / 12 PASS**
- Retained M2-02 Modal tab smoke: **PASS**
- Retained M2-01 Screen tab smoke: **PASS**
- Retained M1 foundation tab smoke: **PASS**
- Post-hotfix automated floor: **24 / 24 focused, 99 / 99 EchoUI, 1205 / 1205 full EditMode**
