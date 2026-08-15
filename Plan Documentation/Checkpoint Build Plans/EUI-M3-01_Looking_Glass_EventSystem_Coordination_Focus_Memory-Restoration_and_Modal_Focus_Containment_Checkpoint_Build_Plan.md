---
tags:
  - sfgss/checkpoint
  - sfgss/wave/foundation
  - sfgss/ui
status: complete
checkpoint: EUI-M3-01
updated: 2026-08-15
---

# EUI-M3-01 — The Looking Glass EventSystem Coordination, Focus Memory/Restoration, and Modal Focus Containment — Checkpoint Build Plan

**Checkpoint:** `EUI-M3-01`
**Status:** **COMPLETE**
**Package:** The Looking Glass (`EchoUI`)
**Package ID:** `com.echodevgames.echo-ui`
**Package authority:** `SFGSS-PKG-ECHOUI-001` v1.5.0 Approved
**Suite authority:** SFGSS-000 v0.27.0
**Workflow authority:** SFGSS-005 v1.6.0 + SFGSS-ADR-007 Green Path
**Learning gate:** PKG-LEARN-008 Complete, including bounded EUI-M3-01 revisit
**Starting repository baseline:** `0b7622cd0c61803eb53921b2b859898c814f1510` — repository hygiene after EUI-M2-02 closeout
**Incoming retained evidence:** full EditMode **1181 / 1181 passed, 0 failed**; EchoUI **75 / 75**; EUI-M2-02 focused **28 / 28**; M2-02 Laboratory **12 / 12 PASS**
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0
**Runtime implementation at activation:** **Not started**
**Activation commit:** `292cb66f216ecc130de67e977befccc10e104297`
**Implementation commit:** `f08c926478b47e11ab810c9898558ca1f8d0a930`

> This checkpoint turns Looking Glass selection into a real, policy-aware focus lifecycle without making Looking Glass the owner of project input, gameplay state, persistence, or the future MMO Window Manager.

## 1. Observable outcome

Starting from the completed M1/M2 foundation, Looking Glass can coordinate an explicitly configured Unity EventSystem, remember eligible focus per live UI entry, optionally remember focus across reopenings during the current UI session, restore/fallback deterministically when Screens or Modals expose prior UI, contain focus inside the top blocking Modal, and revalidate dynamic focus only when requested/relevant events occur.

The checkpoint must prove all of the following together:

- explicit/non-destructive EventSystem coordination;
- deterministic behavior for assigned/existing/create-if-missing/require-external modes;
- actionable degraded/blocking behavior for ambiguous multiple active EventSystems;
- per-live-entry focus memory;
- optional transient stable-surface root-session focus memory;
- designer-selectable fresh versus remember-this-session reopening;
- deterministic restoration/fallback/no-focus resolution;
- pointer and navigation modality remaining designer-controlled;
- blocking Modal focus containment;
- preserved lower-entry focus memory during Modal containment;
- Screen Back/resume focus restoration where policy allows;
- distinct independent Window focus memory without the full Window manager;
- event-driven focus maintenance plus explicit revalidation;
- stale operation/generation focus requests rejected;
- no hard dependency on another Echo package or generated `InputSystem_Actions` wrapper;
- retained M1/M2 Screen, Window, context, Modal, exact-once, and FIFO behavior.

## 2. Authorized scope

### 2.1 EventSystem coordination

Configuration may expose exactly these coordination modes:

- `AdoptAssigned`
- deterministic `AdoptExisting`
- `CreateIfMissing`
- `RequireExternal`

Rules:

- `AdoptAssigned` uses the project/designer-assigned EventSystem only when valid.
- `AdoptExisting` may adopt one unambiguous eligible existing EventSystem.
- `CreateIfMissing` may create only when explicitly configured and no eligible EventSystem exists.
- `RequireExternal` never creates one.
- Looking Glass does not destroy, disable, rename, or silently replace externally owned EventSystems.
- Multiple eligible active EventSystems do not produce an arbitrary winner. Focus coordination enters a structured degraded/blocking state with actionable diagnostics.

### 2.2 Focus memory

Every eligible live UI entry may retain its last valid selected target.

A surface may additionally opt into transient root-session memory keyed by stable surface ID. Designers choose whether a reopened surface:

- starts fresh through its normal authored policy; or
- attempts to reuse valid session memory before falling through the rest of the resolution chain.

Session memory is runtime-only. It is not Chronicle/Accord persistence and does not mutate authored assets.

### 2.3 Focus resolution and restoration

The deterministic resolution chain is:

`explicit target -> valid remembered target -> authored default -> entry resolver -> global fallback -> legal no-focus`

A target is skipped when destroyed, disabled, non-interactable, outside the eligible surface/focus scope, blocked by a higher Modal, or otherwise invalid.

Restoration may occur when:

- a suspended Screen resumes;
- Back exposes a previous Screen entry;
- a blocking Modal completes/aborts and exposes lower UI;
- a Window/surface becomes the explicit focus target;
- project code requests focus or revalidation.

Earlier M1-02 neutral-close behavior is retained as historical checkpoint behavior, but M3-01 explicitly authorizes restoration. Designers may still choose fresh/no-focus behavior.

### 2.4 Pointer/navigation policy

Input modality remains externally supplied/project-owned.

- Pointer interaction/opening may intentionally leave EventSystem selection at `<none>`.
- Looking Glass must not clear focus merely because a pointer moved trivially.
- Navigation/controller modality may establish a configured eligible target when policy requests one.
- No automatic gameplay action-map ownership, switching, or disabling is authorized.

### 2.5 Blocking Modal focus containment

While a blocking Modal stack is active:

- only the top eligible Modal may own legal Looking Glass focus;
- selection attempts into lower Looking Glass UI are rejected/recovered according to the current focus policy;
- lower entries retain their own focus memory;
- completing/aborting the top Modal may restore focus to the newly exposed entry according to policy;
- gameplay input remains outside Looking Glass authority.

### 2.6 Independent Windows

Independent Windows may each retain focus memory and may become the explicitly focused UI surface through project/user interaction.

This checkpoint does not authorize:

- automatic z-order raising doctrine;
- most-recent-eligible Back/Escape LIFO dismissal;
- pin/lock state;
- dragging/resizing;
- persisted layouts;
- generalized focused-window manager behavior.

### 2.7 Event-driven revalidation

Focus maintenance is event-driven by default.

Allowed triggers include:

- entry open/close/suspend/resume;
- Modal stack changes;
- selection target invalidation;
- externally supplied modality change;
- explicit focus request;
- explicit project-callable focus revalidation.

No universal per-frame full-scene selectable/EventSystem scan is required or authorized by default.

A future opt-in tick/revalidation driver is not prohibited, but requires later profiling/evidence and separate authorization.

### 2.8 Stale request protection

Focus work carries operation/generation identity sufficient to prevent older asynchronous/lifecycle work from overriding newer authoritative UI state.

Rejected stale work must fail harmlessly and remain diagnosable.

### 2.9 Optional suite input compatibility profile

SFGSS-000 v0.27.0 defines the suite Unity-default Input Actions compatibility profile.

EUI-M3-01 may use the conventional `UI/Navigate`, `UI/Submit`, `UI/Cancel`, `UI/Point`, `UI/Click`, and related names through optional project/sample adapter seams when useful.

EchoUI Runtime must not hard-depend on:

- generated class `InputSystem_Actions`;
- a specific `.inputactions` asset path;
- generated action/binding GUIDs;
- exact bindings;
- `PlayerInput`;
- The Will or another Echo package.

Explicit project overrides remain supported. Looking Glass does not own enabling/disabling project action maps.

## 3. Explicit exclusions

EUI-M3-01 does **not** authorize:

- transition drivers, transition sequencing, or generalized animation;
- generalized dim/blur/backdrop presentation;
- Motif schema/capture/apply or accessibility presentation implementation;
- HUD region service;
- notifications, prompts, tooltips, or transient messaging services;
- full focused-window/z-order manager;
- Window Back/Escape LIFO history, pin/lock state, dragging/resizing, or persisted layouts;
- durable focus persistence;
- Chronicle/Accord persistence integration;
- arbitrary gameplay/input action-map ownership or automatic switching;
- hard runtime dependency on The Will, Controllers/Vessel, or another Echo package;
- Builder tooling;
- broad primitive or 9-sliced sprite/prefab warehouse work;
- project-wide lifetime composition;
- polished Reference Showcase art.

## 4. Exact implementation file families

No Runtime file changes in the authority/activation commit.

### 4.1 Existing Runtime files allowed to change

Only where directly required for focus/EventSystem hooks:

- `Packages/com.echodevgames.echo-ui/Runtime/Core/EchoUIRoot.cs`
- `Packages/com.echodevgames.echo-ui/Runtime/Surfaces/UISurface.cs`
- existing files under `Packages/com.echodevgames.echo-ui/Runtime/Selection/`
- existing files under `Packages/com.echodevgames.echo-ui/Runtime/Screens/`
- existing files under `Packages/com.echodevgames.echo-ui/Runtime/Modals/`
- `Packages/com.echodevgames.echo-ui/Runtime/EchoDevGames.EchoUI.Runtime.asmdef` only if a Unity/uGUI reference already compatible with package independence is genuinely required.

No peer Echo assembly reference may be added.

### 4.2 New Runtime focus/EventSystem files allowed

New package-local implementation may live beneath:

- `Packages/com.echodevgames.echo-ui/Runtime/Focus/`
- and/or `Packages/com.echodevgames.echo-ui/Runtime/EventSystem/`

Expected responsibilities may include:

- EventSystem coordination mode/status;
- focus memory/cache;
- focus resolution request/result;
- focus coordinator;
- explicit revalidation surface;
- structured focus/EventSystem diagnostics.

Exact type/member names remain routine implementation details so long as they stay within this declared behavior.

### 4.3 Tests allowed

Primary focused coverage:

- `Packages/com.echodevgames.echo-ui/Tests/Editor/EchoUIFocusAndEventSystemTests.cs`
- related `.meta`.

Existing EchoUI test files may change only for direct retained-regression setup/expectation adjustments caused by this checkpoint.

### 4.4 Laboratory proof files allowed

- package `Samples~/LookingGlass_UI_Foundation_Laboratory/README.md`
- package Laboratory scene
- package Laboratory driver/runtime proof helper
- synchronized imported `Assets/Samples/.../Looking Glass UI Foundation Laboratory/` counterparts where intentionally authored.

Sample-only optional input/modality helpers may reference the project Input System if already available, but no such dependency leaks into EchoUI Runtime.

### 4.5 Closeout documentation allowed

At closeout reconcile only SFGSS-005/ADR-007 required package/suite notes, checkpoint evidence, package README/CHANGELOG, Suite Health/Roadmap, and package authority only if implementation exposes a real contract correction.

## 5. Implementation rules

1. **Incoming regression first.** Re-establish full EditMode **1181 / 1181, 0 failed** before any Runtime edit.
2. **Package independence remains absolute.** No hard peer Echo Runtime dependency.
3. **EventSystem ownership is explicit.** Never guess by destroying/disabling an external system.
4. **Ambiguity degrades safely.** Multiple eligible active EventSystems block/degrade focus coordination with diagnostics.
5. **Focus memory is transient.** No disk/Chronicle/Accord/authored-asset mutation.
6. **Fresh reopen remains available.** Session memory is opt-in per authored/effective policy.
7. **Legal no-focus is first-class.** Do not force selection solely because a fallback exists.
8. **Pointer policy is stable.** Do not create focus jitter from trivial pointer movement.
9. **Modal containment is structural.** Lower Looking Glass UI cannot retain legal selection while covered by a blocking top Modal.
10. **Independent Windows stay independent.** Do not smuggle in z-order/LIFO/pins/layout.
11. **Event-driven by default.** No universal per-frame scan.
12. **Explicit revalidation is supported.** Dynamic projects may tell the coordinator when their UI state changes.
13. **Stale requests are harmless.** Older operation/generation work cannot overwrite newer authoritative focus.
14. **Input compatibility is a convenience.** Do not bind core behavior to `InputSystem_Actions` or exact Unity-generated metadata.
15. **Retained M1/M2 behavior stays green.**
16. **Laboratory helpers remain sample-owned.**
17. **Stop on authority change.** If implementation needs excluded capabilities or new suite ownership, return to Declare/Authorize.

## 6. Focused automated tests

At minimum add focused EditMode coverage equivalent to:

1. `AdoptAssignedUsesExplicitEventSystem`
2. `AdoptExistingRequiresUnambiguousEligibleSystem`
3. `CreateIfMissingCreatesOnlyWhenConfiguredAndMissing`
4. `RequireExternalNeverCreatesEventSystem`
5. `MultipleActiveEventSystemsEnterDegradedBlockedFocusStateWithoutDeletion`
6. `LiveEntryRemembersLastValidFocus`
7. `SurfaceMayOptIntoSessionLevelReopenMemory`
8. `FreshReopenPolicyIgnoresSessionMemory`
9. `ModalCloseRestoresUnderlyingRememberedFocusWhenPolicyAllows`
10. `ScreenBackRestoresPreviousEntryFocusWhenPolicyAllows`
11. `InvalidRememberedTargetFallsThroughFallbackChain`
12. `PointerPolicyMayResolveToNoFocus`
13. `NavigationPolicyCanResolveConfiguredDefault`
14. `BlockingModalContainsFocusToTopModal`
15. `LowerEntryFocusMemorySurvivesModalContainment`
16. `ExplicitRevalidationRepairsDynamicInvalidFocus`
17. `StaleFocusRequestCannotOverrideNewerState`
18. `IndependentWindowsRetainDistinctFocusMemoryWithoutWindowManager`
19. `RuntimeAssemblyHasNoPeerEchoPackageDependency`
20. `CoreHasNoHardInputSystemGeneratedWrapperDependency`

Retain and rerun the existing M1/M2 focus/selection/Modal/Screen lifecycle tests.

## 7. Manual Laboratory proof

Extend the existing Looking Glass Laboratory only enough to prove:

1. `AdoptAssigned` coordinates the assigned EventSystem.
2. `AdoptExisting`, `CreateIfMissing`, and `RequireExternal` demonstrate their distinct creation/adoption rules.
3. Multiple eligible active EventSystems produce a visible degraded/blocked focus state without deleting either system.
4. A lower surface remembers focus while a blocking Modal opens and restores it when the Modal completes.
5. Screen Back exposes the prior Screen and restores its remembered eligible target when policy allows.
6. Fresh reopen policy ignores old session memory.
7. Remember-this-session reopen policy reuses valid stable-surface memory.
8. An invalid remembered target falls through to the next legal target or `<none>`.
9. Pointer policy may remain `<none>` without jitter.
10. Navigation/controller policy may establish the configured default.
11. Blocking Modal focus cannot escape into lower Looking Glass UI.
12. Explicit revalidation repairs deliberately invalidated dynamic focus, then retained M2/M1 Screen/Window/context/Modal behavior still passes a smoke check.

The Laboratory may use sample-owned simulation controls. It must label them as proof infrastructure rather than project input authority.

## 8. Performance evidence

Record bounded evidence that idle focus coordination does not perform a full-scene EventSystem/selectable search every frame.

At minimum:

- demonstrate no recurring full-scene focus search during a stable idle UI state;
- show explicit lifecycle/revalidation triggers perform bounded work;
- record any unavoidable allocations/search work discovered during focused profiling.

A failure here is a stop condition, not permission to add a hidden polling loop.

## 9. Green Path execution

1. Verify clean synchronized authority/activation commit.
2. Re-establish incoming full EditMode **1181 / 1181** before Runtime edits.
3. Apply only the declared Runtime/test scope.
4. Allow Unity compilation/import.
5. Run focused M3-01 tests.
6. Run the full EchoUI EditMode assembly.
7. Run final full Foundry EditMode with zero failures.
8. Perform the 12-item manual Laboratory proof.
9. Capture bounded performance evidence.
10. Synchronize package/imported Laboratory parity.
11. Run hygiene/scope validation.
12. Commit implementation as a distinct boundary.
13. Reconcile documentation/evidence as a distinct closeout boundary.
14. Push and verify clean synchronized repository.

Green Path stops on compile/test/manual/performance failure, EventSystem ambiguity mishandling, unexpected scope, peer dependency, persistence leak, input-ownership leak, stale-request race, baseline mismatch, or authority-changing discovery.

## 10. Stop point

EUI-M3-01 is complete only when:

- incoming **1181 / 1181** is re-established before Runtime edits;
- focused M3-01 tests are green;
- final EchoUI and full Foundry EditMode have zero failures;
- all 12 manual checks pass;
- bounded performance evidence confirms event-driven behavior;
- package/imported Laboratory parity is reconciled;
- Runtime has no peer Echo or generated InputActions-wrapper hard dependency;
- M1/M2 behavior remains green;
- documentation matches committed implementation;
- HEAD/origin and working tree are clean.

**Then stop.** Do not begin transitions, Motifs/accessibility presentation, HUD/transients, Window LIFO/pinning/layout, persistence, Builder, primitive/9-slice work, peer bridges, or polished showcase work in the same checkpoint.

## 11. Activation record

- JIT learning/intake: **Complete**
- Package declaration: **Complete**
- Suite authority reconciliation: **SFGSS-000 v0.27.0**
- Package authority reconciliation: **SFGSS-PKG-ECHOUI-001 v1.5.0**
- Checkpoint authorization: **EUI-M3-01 ACTIVE / AUTHORIZED**
- Starting implementation baseline: `0b7622cd0c61803eb53921b2b859898c814f1510`
- Incoming retained full EditMode: **1181 / 1181 passed, 0 failed**
- Runtime implementation at activation: **Not started**
- Activation Git commit: `292cb66f216ecc130de67e977befccc10e104297` (`292cb66`)

**Stop after activation. Do not begin Runtime implementation until the incoming 1181 / 1181 gate is re-established on the activation commit.**

## 12. Final closeout record

- Activation: `292cb66` (`292cb66f216ecc130de67e977befccc10e104297`).
- Implementation: `f08c926` (`f08c926478b47e11ab810c9898558ca1f8d0a930`).
- Incoming activation regression re-established before Runtime edits: **1181 / 1181 passed, 0 failed**.
- Focused EUI-M3-01 tests: **24 / 24 passed, 0 failed**.
- EchoUI EditMode assembly: **99 / 99 passed, 0 failed**.
- Full Foundry EditMode: **1205 / 1205 passed, 0 failed, 0 skipped, 0 inconclusive**.
- Manual Laboratory: **12 / 12 PASS**.
- Bounded performance evidence: **PASS**; stable idle focus behavior plus explicit bounded revalidation were observed.
- Retained M2-02, M2-01, and M1 Laboratory tabs: **PASS**.
- Package/imported Laboratory parity: **VERIFIED**.
- Runtime has no hard peer Echo dependency and no generated `InputSystem_Actions` wrapper dependency.
- The implementation remains inside the declared EventSystem/focus scope. The session-memory hotfix repaired the authorized transient-session-memory behavior rather than changing package authority.
- Package authority remains **SFGSS-PKG-ECHOUI-001 v1.5.0** under **SFGSS-000 v0.27.0**; no new suite ADR is required.
- **EUI-M3-01 is COMPLETE. No follow-on Looking Glass checkpoint is activated by this closeout.**
- Transitions, Motifs/accessibility presentation, HUD/transients, Window LIFO/pinning/layout, persistence, Builder, primitive/9-slice work, peer bridges, and polished showcase work remain separately gated.
