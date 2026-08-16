---
id: EUI-M3-02
package: EchoUI
status: complete
authority: SFGSS-PKG-ECHOUI-001 v1.6.0
suite_authority: SFGSS-000 v0.27.0
activation_baseline: 0c582405280d19caed4045d0072d8cf29d138e1e
closeout_commit: 0affb7de757f8acdd35175457f70d00c657b85c3
date: 2026-08-15
---

# EUI-M3-02 — Looking Glass View Lifecycle, Replaceable Transition Drivers, and Deterministic Transition Recovery — Checkpoint Build Plan

**Status:** **COMPLETE / CLOSED**
**Package:** The Looking Glass (`EchoUI`)
**Technical package:** `com.echodevgames.echo-ui`
**Package authority:** **SFGSS-PKG-ECHOUI-001 v1.6.0**
**Suite authority:** **SFGSS-000 v0.27.0 unchanged**
**Incoming clean baseline:** `0c582405280d19caed4045d0072d8cf29d138e1e` (`0c58240`)
**Incoming full EditMode evidence:** **1205 / 1205 passed, 0 failed, 0 skipped, 0 inconclusive**
**Incoming EchoUI evidence:** **99 / 99 passed**
**Incoming focused EUI-M3-01:** **24 / 24 passed**
**Incoming manual Laboratory:** **12 / 12 PASS**
**Incoming bounded focus performance evidence:** **PASS**
**Runtime implementation at activation:** **Not started**

> This checkpoint is the second bounded slice of Milestone M3 — Focus and Presentation. It adds authoritative view-transition settlement without reopening the completed M3-01 focus/EventSystem contract or pulling Motifs/HUD/Builder work into the same implementation.

---

## 1. Outcome

Implement the smallest useful, independently provable transition/view-lifecycle layer that lets existing Screens, blocking Modals, and independent Windows enter and exit through replaceable transition drivers while structural lifecycle authority remains deterministic.

The checkpoint must prove fresh async terminal results, best-effort cancellation with mandatory safety, stale completion rejection, deterministic enter/exit recovery, package reference Immediate/CanvasGroup Fade drivers, professional custom-driver extensibility, reduced-motion substitution seams, and retained M3-01 focus/EventSystem behavior.

M3-02 does **not** implement the Motif/accessibility service, HUD/transients, the future Window manager, persistence, peer bridges, or the future Assembly Library/Builder authoring systems.

---

## 2. Locked declaration

### 2.1 Transition settlement belongs to the structural operation

An accepted transition-aware operation remains one authoritative operation. Structural mutation does not report terminal success until its required transition settles. Existing bounded FIFO/serialization law remains in force.

### 2.2 Driver authority boundary

Drivers may affect view presentation such as CanvasGroup alpha, transform scale/position, Animator state, project materials/shaders, or project-owned tween behavior. Drivers may not own screen history, Modal result meaning, gameplay rules, pause/time scale, cursor authority, input-map state, scene travel, save/settings truth, audio authority, or project lifetime composition.

### 2.3 Fresh async execution

Every transition execution must own fresh operation state and a fresh `Awaitable<UITransitionResult>`. Cached/reused awaitables are forbidden. Operation/generation identity prevents late stale callbacks from rewinding newer lifecycle truth.

### 2.4 Cancellation and hard bounds

Cancellation is best-effort. Safety is mandatory: unscaled time, a configured or package hard upper bound, structured timeout/exception/cancel settlement, stale-generation rejection, and cleanup on shutdown/view destruction.

### 2.5 Enter failure

Enter/open failure never commits the incoming entry as authoritative Active. RootOwned partial instances release; SceneOwned/ExternalOwned objects are not destroyed by Looking Glass; structural history returns to or retains the prior stable state; an admitted blocking Modal open failure settles structurally as `Aborted`.

### 2.6 Exit failure

Exit failure/timeout/exception forces the departing entry into deterministic closed/released settlement so a broken animation cannot hold the UI hostage. Existing Modal exact-once semantic result remains exact once.

### 2.7 Effective policy

`project/root default -> per-definition profile -> transient operation override`

Profiles may independently provide enter/exit drivers, timing/duration, optional curve/easing data, hard timeout, and reduced-motion substitution. Runtime overrides never mutate authored assets.

### 2.8 Role wiring

The seam is surface-general but EUI-M3-02 wires only Screen, blocking Modal, and independent Window lifecycle. HUD/notification/tooltip/prompt services remain M4 work.

### 2.9 Reference drivers

Built-ins:
- Immediate/no-animation;
- unscaled CanvasGroup Fade.

Professional project drivers may use Animator, tween libraries, shader/dissolve, slide, scale/pop, 3D transform, or other project animation systems without replacing Looking Glass lifecycle authority or adding a mandatory tween dependency.

### 2.10 Reduced-motion seam

Transition policy can later substitute Immediate or another approved reduced-motion path. The broader Motif/accessibility service is not activated here.

---

## 3. Durable future authoring promise

The following capabilities are declared package direction but explicitly outside M3-02 implementation.

### 3.1 Primitive Warehouse
Package-owned focused prefab families may include default/close buttons, sliders, toggles, tabs, text/input fields, dropdowns, scroll pieces, separators, progress indicators, panel/window surfaces, scalable 9-sliced borders/backgrounds, and parallel square/round/other visual families sharing behavior.

### 3.2 Panel/Menu Template Library
Ordinary editable prefab compositions assembled from primitives. Main menu, settings, pause, confirmation, inventory-style Window, character sheet, journal/quest, crafting, tabbed, and generic list/detail layouts are examples, not mandatory genre law.

### 3.3 Stable-ID Template Catalog
Package starter definitions plus project-extensible add/remove/replace/regroup behavior. Catalogs do not require editing package Runtime.

### 3.4 Assembly Utilities
Lightweight Editor commands may create from template, add button/slider/toggle groups, name/parent/validate, replace primitive families, and later prepare/apply Motif targets. These utilities remain useful without the full Builder.

### 3.5 Builder / Composer
The later Builder consumes the same catalog and creates normal editable project objects. It is never the only way to understand or edit generated UI.

---

## 4. Expected Runtime/Data additions

Likely bounded types:

```text
Runtime/Transitions/
  UITransitionOperationId
  UITransitionResult
  UITransitionStatus
  UITransitionDirection
  UITransitionRequest
  UITransitionProfile
  UITransitionResolvedPolicy
  IUITransitionDriver
  UITransitionCoordinator
  ImmediateUITransitionDriver
  CanvasGroupFadeTransitionDriver
```

Likely integration points: EchoUIRoot, Screen lifecycle settlement, Modal lifecycle settlement, independent Window open/close path, existing focus restoration hooks, and existing structural-operation queues/admission.

---

## 5. Serialization and compatibility

- Existing Screens/Modals/Windows without a transition profile remain valid.
- Missing profile resolves to project/root default or Immediate fallback.
- New serialized fields use safe defaults.
- Runtime overrides never rewrite ScriptableObjects.
- Transition progress is not persisted.
- No Chronicle dependency is introduced.
- Existing M3-01 focus contracts remain source-compatible unless a contract-preserving compile correction is required.

---

## 6. Lifecycle sequencing

### 6.1 Screen Push/Navigate
`preflight -> create/adopt -> bind -> resolve transition policy -> gate/suspend prior entry -> enter target -> commit Active -> establish/restore focus -> terminal result`

On enter failure, restore/retain prior stable state and release the incoming entry according to ownership.

### 6.2 Back/Close/Replace/Reset
Exit and enter work remain part of one serialized structural mutation where applicable. No overlapping transition may produce an out-of-order final state.

### 6.3 Blocking Modal open
An admitted Modal must enter successfully before normal top interaction. An admitted open failure settles structural `Aborted`.

### 6.4 Blocking Modal complete/close
Semantic result remains exact-once first-terminal-wins. Exit failure cannot fabricate a second result and force release still occurs.

### 6.5 Independent Window
Window transitions do not activate focused-window arbitration, z-order doctrine, LIFO Back/Escape, pins, drag/resize, or persisted layout.

---

## 7. Transition result semantics

Structured results should distinguish at least Completed, Cancelled, TimedOut, Failed, Stale/Superseded, and Unavailable/InvalidDefinition where applicable. Diagnostics may carry bounded operation ID/generation, surface ID, direction, driver/profile ID, elapsed time, and terminal code. No arbitrary domain payload transport.

---

## 8. Failure matrix

| Failure | Required authoritative outcome |
|---|---|
| Missing driver/profile | Resolve default/Immediate or structured unavailable path; no half-state |
| Driver throws | Structured failure and deterministic recovery |
| Driver never completes | Hard timeout and deterministic recovery |
| Driver ignores cancellation | Stale generation cannot commit; hard bound cleans coordinator state |
| View destroyed during enter | Incoming entry fails/aborts; prior stable state retained/restored |
| View destroyed during exit | Force terminal release/closed state |
| Root shutdown | Root shutdown wins; transition becomes stale/cancelled |
| Duplicate root | No transition side effects |
| Stale completion | No authoritative mutation |
| Modal enter failure | Structural Aborted, never semantic Cancel |
| Modal exit failure | Existing exact-once semantic result remains; force release |
| Focus invalid after transition | Existing M3-01 fallback/revalidation applies |

---

## 9. Automated proof minimum

At minimum prove:
1. Immediate enter.
2. Immediate exit.
3. CanvasGroup fade unscaled policy.
4. fresh operation/awaiter per execution.
5. structural success waits for transition settlement.
6. queued structural work does not race the same transition.
7. stale enter cannot activate superseded entry.
8. stale exit cannot reopen released entry.
9. cancellable success path.
10. noncancellable stale path harmless.
11. never-completing driver hard timeout.
12. enter exception restores prior stable Screen.
13. enter timeout restores prior stable Screen.
14. exit exception force-closes Screen.
15. exit timeout force-closes Screen.
16. Modal enter failure structural Aborted.
17. Modal exact-once retained when exit fails.
18. RootOwned partial release on failed enter.
19. SceneOwned/ExternalOwned object not destroyed.
20. root/default profile resolution.
21. definition override.
22. transient operation override without asset mutation.
23. reduced-motion Immediate substitution.
24. focus after enter remains M3-01 compliant.
25. focus restoration after exit remains M3-01 compliant.
26. Window transition does not activate full Window manager.
27. no hard peer Echo dependency.
28. no mandatory tween dependency.
29. no generated `InputSystem_Actions` dependency.
30. no universal per-frame transition scene scan.
31. retained M3-01 focus suite green.

Exact test count may exceed this minimum.

---

## 10. Laboratory proof minimum

Dedicated M3-02 proof should demonstrate Immediate Screen enter/exit, visible CanvasGroup fade, Modal fade with exact-once retained, independent Window fade, default-vs-definition profile, transient override, enter failure recovery, exit failure force-close, timeout recovery, stale completion rejection, reduced-motion substitution, retained focus behavior, and retained M3-01/M2-02/M2-01/M1 tabs. Sample-only fake drivers may generate deterministic failure/timeout/stale cases.

---

## 11. Performance proof

Prove no per-frame global transition scan, idle coordinator quiescence, bounded operation history/queues, cleanup after timeout/cancel, and unscaled timing while `Time.timeScale = 0` in a sample-owned proof without transferring pause authority.

---

## 12. Documentation and closeout

At seal/closeout reconcile package README/CHANGELOG, package/suite Current Notes, this plan, Roadmap, Health, activation/implementation hashes, focused/full automated evidence, Laboratory evidence, and performance evidence. Keep Motif/accessibility service and M4 HUD/transient activation closed unless separately Learn/Declare/Authorize reviewed.

---

## 13. Explicitly not in EUI-M3-02

Do not opportunistically implement Motif capture/apply/local overrides; accessibility service beyond transition substitution seam; generalized dim/blur; HUD regions; notifications; tooltip/prompt services; full Window LIFO/pin/drag/resize/layout; UI persistence; peer bridges; gameplay-input map ownership; project-wide lifetime composition; Primitive Warehouse; 9-slice warehouse; Panel/Menu Template Library; Template Catalog; Assembly Utilities; full Builder/Composer; or polished Reference Showcase art.

---

## 14. Green Path

Routine bounded compile/test corrections that do not change the declared contract are pre-approved. Stop and return to authority for package ownership, peer dependency, structural-operation law, Modal exact-once semantics, input/game-state authority, durable serialization compatibility, public transition contract beyond this extension seam, Primitive/Template/Builder ownership boundaries, or suite authority.

Generated `.slnx` and recognized package-import metadata churn should be handled by repository hygiene without conversational approval prompts.

---

## 15. Activation gate

This plan is authorized only after its authority/activation commit lands on clean baseline `0c582405280d19caed4045d0072d8cf29d138e1e`. Immediately after activation: verify clean synchronized Git state, run full EditMode, require **1205 / 1205 passed, 0 failed**, and only then begin Runtime transition work. No follow-on checkpoint is activated by this plan.


---

## EUI-M3-02 FINAL CLOSEOUT COMPLETE

**Status:** Complete and sealed.

**Activation commit:** `ee9d3ffa9c3b2ad4fc8136a70943122f852cca49`

**Implementation commit:** `c919238` (`Implement EUI-M3-02 transition lifecycle and Laboratory proof`)

**Closeout commit:** `0affb7d` (`Close out EUI-M3-02 transition lifecycle`)

**Unity baseline:** `6000.3.8f1`

EUI-M3-02 delivered authoritative enter/exit settlement through replaceable transition drivers across Screens, independent Windows, and blocking Modals. The completed slice includes root/default, definition-profile, and transient policy layering; deterministic failure, timeout, cancellation, and stale recovery; reduced-motion substitution; unscaled presentation timing; exact-once Modal terminal claims through exit settlement; deferred Screen wait-through-exit behavior; and retained M3-01 focus authority.

### Final automated evidence

- Full Foundry EditMode: **1246 / 1246 passed**, 0 failed, 0 inconclusive, 0 skipped.
- `EchoDevGames.EchoUI.Tests.Editor.dll`: **140 / 140 passed**.
- `EchoUITransitionCoreTests`: **21 / 21 passed**.
- `EchoUITransitionLifecycleIntegrationTests`: **10 / 10 passed**.
- `EchoUIModalTransitionIntegrationTests`: **10 / 10 passed**.
- Final evidence XML: `TestResults_20260816_144932.xml`.

### Final Laboratory evidence

The Looking Glass UI Foundation Laboratory passed checks **1-14**, covering Immediate Screen lifecycle, visible ExternalOwned Screen fades, transient independent Window fades, Modal exact-once completion through asynchronous exit, policy layering, enter rollback, failed-exit force-close, hard timeout recovery, stale supersession rejection, reduced-motion substitution, unscaled timing at `Time.timeScale = 0`, retained M3-01 focus, 180-frame idle quiescence, and retained M3-01/M2-02/M2-01/M1 smoke.

Final formerly-red observation:

`CHECK 9 PASS stale completion rejection. first=Stale, second=Completed, finalAlpha=1.00, activeTransitions=0.`

### Laboratory-discovered Runtime correction

Check 9 exposed a synchronous Unity `Awaitable` cancellation race. Token cancellation could settle and release an operation before fallback direct-awaitable cancellation executed. `UITransitionCoordinator` now records terminal settlement and performs direct `Awaitable.Cancel()` only when token-first cancellation did not already settle the operation. The retained regression `SupersedingTokenCancelledFadeDoesNotRecancelReleasedAwaitable` proves the exact slow-fade-to-Immediate supersession path.

This closeout did not itself activate a follow-on checkpoint. EUI-M4-01 was activated later by its own separate authority commit `ce30ac6`.
