# FL-M4-02 — Default uGUI Plain Status View and Presentation Assembly

**Document ID:** FL-M4-02
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
**Milestone:** M4 — Startup Entry and Presentation
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Unity baseline:** Unity 6000.3.8f1
**Starting implementation commit:** `46481b1`
**Starting documentation commit:** `e4367bf`
**Starting Runtime Play Mode:** 396 passed, 0 failed, 0 ignored
**Starting compilation:** 0 errors, 0 compiler warnings
**Authorized:** August 5, 2026

> First Light already speaks through a neutral contract. This checkpoint gives that voice a plain, removable uGUI face.

---

## 1. Purpose and observable outcome

FL-M4-02 creates the first default visual implementation of
`ILaunchStatusPresenter` without adding uGUI references to the neutral Runtime
assembly.

When complete:

1. `EchoDevGames.EchoLaunch.Presentation.UGUI` compiles as a separate runtime
   assembly.
2. `EchoLaunchStatusView` implements `ILaunchStatusPresenter`.
3. The view shows readable state and detail text.
4. Determinate progress shows a normalized slider and percentage.
5. Indeterminate progress shows a distinct configurable surface.
6. Active step position and stable step ID are readable.
7. Elapsed launch time is readable.
8. Warning, failed, interrupted, transitioning, and completed states have
   distinct text.
9. Finalized report diagnostics and destination display metadata are shown.
10. Missing optional visual references remain safe.
11. Text copy remains replaceable through serialized fields.
12. Unbinding can hide and optionally clear startup-only presentation.
13. The neutral Runtime assembly remains free of uGUI references.
14. Presentation proof runs in its own test assembly.

---

## 2. Approved architecture

### 2.1 Assembly isolation

Create:

```text
EchoDevGames.EchoLaunch.Presentation.UGUI
```

References:

- `EchoDevGames.EchoLaunch.Runtime`
- `Unity.ugui`

The assembly is auto-referenced so a project-owned root can serialize an
`EchoLaunchStatusView` component through the neutral `MonoBehaviour` seam.

The existing Runtime asmdef remains unchanged and uGUI-free.

### 2.2 Test isolation

Create:

```text
EchoDevGames.EchoLaunch.Tests.Presentation.UGUI
```

References:

- Neutral Runtime
- Presentation.UGUI
- Unity UI
- Unity Test Framework

Runtime `AssemblyInfo.cs` grants internal constructor access only to this
dedicated test assembly so finalized reports can be tested without widening
the public report API.

### 2.3 View ownership

`EchoLaunchStatusView` owns only:

- Startup state text.
- Startup detail/diagnostic text.
- Active-step readout.
- Elapsed-time readout.
- Determinate progress display.
- Indeterminate progress display.
- Startup-only visibility.

It does not own:

- Root authority.
- Lifecycle transitions.
- Startup work.
- Destination loading.
- Report finalization.
- General menus or HUD navigation.
- Splash timing or skip policy.

### 2.4 Plain copy

State copy is serialized and replaceable:

- Preparing launch.
- Validating launch.
- Starting systems.
- Continuing with a warning.
- Loading destination.
- Launch complete.
- Launch blocked.
- Launch interrupted.

This is localization-ready authoring data, not a localization integration.

---

## 3. Exact files

### Modified

- `Runtime/Properties/AssemblyInfo.cs`

### Created presentation runtime

- `Presentation.UGUI/EchoDevGames.EchoLaunch.Presentation.UGUI.asmdef`
- `Presentation.UGUI/Properties/AssemblyInfo.cs`
- `Presentation.UGUI/EchoLaunchStatusView.cs`

### Created presentation tests

- `Tests/Presentation.UGUI/EchoDevGames.EchoLaunch.Tests.Presentation.UGUI.asmdef`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchStatusViewTests.cs`

### Created plan

- `Plan Documentation/Checkpoint Build Plans/FL-M4-02_Default_uGUI_Plain_Status_View_and_Presentation_Assembly_Checkpoint_Build_Plan.md`

Unity generates folder, asmdef, and script `.meta` files.

---

## 4. View contract

### Bind

- Resolve the local `CanvasGroup`.
- Normalize the determinate slider to `0..1`.
- Mark the presenter bound.
- Clear the previous terminal report.
- Show the view when configured.
- Render the initial immutable snapshot.

### Present

- Ignore calls before binding.
- Preserve the exact accepted snapshot.
- Render state, message, active step, elapsed time, and progress mode.
- Show warning copy when a running snapshot carries a warning result.

### PresentTerminal

- Reject a null report.
- Ignore valid reports before binding.
- Preserve the exact report instance.
- Render completed, failed, or interrupted copy.
- Show diagnostic code and message when present.
- Show destination display metadata when present.
- Force completed progress to 100 percent.
- Preserve the last progress mode for failed/interrupted outcomes.

### Unbind

- Be idempotent.
- Mark the view unbound.
- Hide through `CanvasGroup` when configured.
- Optionally clear rendered startup-only state.

---

## 5. Accessibility and replacement rules

- Every state is understandable through text.
- Color is not required for meaning.
- Determinate and indeterminate progress use separate visible surfaces.
- All state copy is serialized and replaceable.
- The view does not require audio.
- The view uses anchored uGUI-compatible references but does not own a prefab
  in this checkpoint.
- Missing optional references do not break launch.
- Project-owned fonts, colors, hierarchy, and layout remain replaceable.

---

## 6. Automated proof

New test count: `18`.

Required coverage:

- Interface implementation.
- Bind visibility and initial copy.
- Determinate progress.
- Indeterminate progress.
- Step position and ID.
- Elapsed time.
- Warning diagnostic rendering.
- Transitioning copy.
- Completed report rendering.
- Failed report rendering.
- Interrupted report rendering.
- Pre-bind snapshot no-op.
- Pre-bind terminal no-op.
- Null report rejection.
- Unbind visibility.
- Clear-on-unbind behavior.
- Rebind reset.
- Missing optional references.
- Serialized copy replacement.

Predicted full Runtime Play Mode total:

```text
414
```

This is a target, not evidence.

---

## 7. Compile gates

- Unity errors: 0.
- Unity compiler warnings: 0.
- Presentation assembly resolves Unity UI.
- Neutral Runtime asmdef remains without Unity UI references.
- Test assembly resolves internal report constructors through the named
  friend assembly.
- No TextMeshPro dependency is introduced.

---

## 8. Explicit exclusions

FL-M4-02 does not authorize:

- Default prefab YAML.
- Canvas art pass.
- Project logo or background.
- Splash sequence definitions.
- Splash playback.
- Fade, hold, skip, or reduced-motion behavior.
- Automatic prefab discovery.
- Root hierarchy creation.
- Editor setup or repair.
- Test Lab scenes.
- Direct-scene initialization.
- Persistent-root policy.
- EchoUI bridge.
- Player build proof.
- Package version change.

---

## 9. Implementation sequence

### Phase 1 — Assembly boundary

1. Add the presentation asmdef.
2. Add presentation friend-assembly metadata.
3. Add the dedicated presentation test asmdef.
4. Add the Runtime friend test assembly declaration.
5. Compile.

### Phase 2 — Plain view

1. Add `EchoLaunchStatusView`.
2. Bind serialized text/progress references.
3. Implement determinate and indeterminate modes.
4. Implement terminal report presentation.
5. Implement visibility and unbind behavior.
6. Compile.

### Phase 3 — Isolated proof

1. Add `18` presentation tests.
2. Run the complete Runtime Play Mode suite.
3. Record observed totals.
4. Inspect expected versus unexpected warnings.

### Phase 4 — Closeout

1. Review exact Git scope.
2. Commit and push implementation.
3. Batch-generate documentation closeout.
4. Commit and push documentation.
5. Confirm clean synchronized repository.

---

## 10. Rollback

Before commit:

```cmd
git restore --staged .
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Properties/AssemblyInfo.cs"
git clean -fd -- "Packages/com.echodevgames.echo-launch/Presentation.UGUI"
git clean -fd -- "Packages/com.echodevgames.echo-launch/Tests/Presentation.UGUI"
git clean -f -- "Plan Documentation/Checkpoint Build Plans/FL-M4-02_Default_uGUI_Plain_Status_View_and_Presentation_Assembly_Checkpoint_Build_Plan.md"
```

After a pushed implementation commit, use `git revert`.

---

## 11. Commit plan

Implementation:

```text
echo-launch: complete FL-M4-02 plain uGUI status view
```

Documentation:

```text
echo-launch: document FL-M4-02 completion
```

---

## 12. Stop point

Stop after the removable uGUI assembly and plain presenter pass isolated
automated proof.

Do not begin splash playback, prefab creation, Editor setup, direct-scene
initialization, or Laboratory scenes.

---

## 13. Tentative next checkpoint

**FL-M4-03 — Image Splash Definitions and Deterministic Splash Player**

Tentative only. Not authorized by FL-M4-02.

---

## 14. Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Keep Runtime uGUI-free, keep text meaning independent from
color, preserve headless fallback, and stop before splash playback or prefab
tooling.
