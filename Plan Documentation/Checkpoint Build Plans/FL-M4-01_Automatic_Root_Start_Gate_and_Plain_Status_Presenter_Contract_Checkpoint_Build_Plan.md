# FL-M4-01 — Automatic Root Start Gate and Plain Status Presenter Contract

**Document ID:** FL-M4-01
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
**Milestone:** M4 — Startup Presentation Boundary
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Unity baseline:** Unity 6000.3.8f1
**Starting implementation commit:** `114ac91`
**Starting documentation commit:** `727b502`
**Starting Runtime Play Mode:** 380 passed, 0 failed, 0 ignored
**Starting compilation:** 0 errors, 0 compiler warnings
**Authorized:** August 5, 2026

> The launch machinery is awake. This checkpoint lets Unity open the gate automatically and gives presentation one window into the truth without handing it the keys.

---

## 1. Purpose and observable outcome

FL-M4-01 introduces the automatic root start boundary and the neutral startup-status presenter contract.

When complete:

1. The authoritative root still claims in `Awake`.
2. Unity `Start` begins launch automatically by default.
3. Automatic and explicit starts use the same one-run `StartLaunchAsync` gate.
4. A manual start completed before Unity `Start` cannot trigger a second run.
5. Duplicate roots remain disabled before automatic execution or presenter callbacks.
6. A neutral `ILaunchStatusPresenter` observes immutable accepted progress.
7. The presenter receives the initial `AuthorityClaimed` snapshot before validation.
8. The presenter receives accepted lifecycle/progress snapshots in authority order.
9. The presenter receives the finalized immutable terminal report after `LastReport` assignment.
10. Presenter failures are isolated and cannot change launch truth.
11. Missing presentation uses a silent headless fallback.
12. An assigned component that does not implement the presenter contract emits `ELAUNCH-VIEW-001`.
13. Presenter callback failures emit `ELAUNCH-VIEW-002`.
14. The presenter unbinds during root destruction.
15. Existing manual test fixtures explicitly disable automatic startup.
16. The neutral Runtime assembly remains free of uGUI references.

---

## 2. Approved architecture

### 2.1 Root startup sequence

```text
Unity loads root
    -> Awake claims authority
        -> duplicate exits before presentation or launch
        -> authoritative root creates session and resolves presenter
            -> Unity Start checks automatic-start gate
                -> StartLaunchAsync owns the one-run gate
```

`Awake` does not execute startup steps.

`Start` does not introduce a second execution path. It calls the existing internal `StartLaunchAsync` gate only when:

- automatic startup is enabled,
- the root is live and authoritative,
- state is still `AuthorityClaimed`,
- no launch is active.

### 2.2 Automatic-start policy

New serialized root field:

```csharp
[SerializeField]
private bool startAutomatically = true;
```

Production default is enabled.

An internal test seam may disable it only while the root is idle and still `AuthorityClaimed`.

This field is scene/prefab state, not a project-owned ScriptableObject schema change.

### 2.3 Neutral presenter contract

Public Runtime interface:

```csharp
public interface ILaunchStatusPresenter
{
    void Bind(LaunchProgressSnapshot initialSnapshot);
    void Present(LaunchProgressSnapshot snapshot);
    void PresentTerminal(LaunchReport report);
    void Unbind();
}
```

Contract rules:

- Callbacks occur on Unity’s main thread.
- Inputs are immutable accepted state/report values.
- The presenter never publishes lifecycle state.
- The presenter never executes startup steps.
- The presenter never loads scenes.
- The presenter never finalizes reports.
- The presenter never owns general UI navigation.
- Callback exceptions are contained.

### 2.4 Headless fallback

When no presenter component is assigned, First Light uses an internal no-op presenter.

No warning is emitted for the intentionally headless path.

When a component is assigned but does not implement `ILaunchStatusPresenter`, First Light emits:

```text
ELAUNCH-VIEW-001
```

and continues through the headless fallback.

### 2.5 Callback-failure containment

Any `Bind`, `Present`, `PresentTerminal`, or `Unbind` exception emits:

```text
ELAUNCH-VIEW-002
```

The root continues through the authoritative lifecycle whenever it remains alive.

### 2.6 Ordering

Presenter ordering:

```text
StartLaunchAsync enters
    -> presenter Bind(AuthorityClaimed)
        -> accepted progress snapshot
            -> presenter Present(snapshot)
                -> public progress/state events
                    -> terminal state accepted
                        -> report finalized
                            -> LastReport assigned
                                -> presenter PresentTerminal(report)
                                    -> public terminal event
```

The presenter observes truth. It does not create truth.

---

## 3. Exact implementation scope

### Runtime files created

- `Runtime/Presentation.meta`
- `Runtime/Presentation/ILaunchStatusPresenter.cs`
- `Runtime/Presentation/ILaunchStatusPresenter.cs.meta`
- `Runtime/Presentation/NullLaunchStatusPresenter.cs`
- `Runtime/Presentation/NullLaunchStatusPresenter.cs.meta`
- `Runtime/Presentation/LaunchStatusPresenterDispatcher.cs`
- `Runtime/Presentation/LaunchStatusPresenterDispatcher.cs.meta`

### Runtime files modified

- `Runtime/Core/EchoLaunchRoot.cs`

### Automated tests created

- `Tests/Runtime/PlayMode/EchoLaunchAutomaticStartAndPresenterTests.cs`
- `Tests/Runtime/PlayMode/EchoLaunchAutomaticStartAndPresenterTests.cs.meta`

### Retained tests modified

Only manual root helpers are updated to disable automatic startup before directly invoking `StartLaunchAsync`:

- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs`

The stale retained test name is updated from “without automatic startup” to “before automatic start.”

---

## 4. Runtime requirements

### 4.1 Unity Start callback

Use Unity `Awaitable`:

```csharp
private async Awaitable Start()
```

The callback returns without work when automatic startup is disabled or the session has already advanced.

### 4.2 One-run protection

Automatic start must never bypass:

- authoritative-root check,
- `AuthorityClaimed` state requirement,
- active-run interlocked gate,
- existing start-gate diagnostics.

### 4.3 Serialized presenter reference

The neutral Runtime root stores:

```csharp
[SerializeField]
private MonoBehaviour statusPresenterComponent;
```

The reference is resolved to `ILaunchStatusPresenter`.

This permits a later isolated uGUI presentation assembly without making the Runtime assembly reference uGUI.

### 4.4 Test injection

Internal seams:

```csharp
SetAutomaticStartForTesting(bool enabled)
SetStatusPresenterForTesting(ILaunchStatusPresenter presenter)
```

Both are allowed only on an idle authoritative root before launch begins.

### 4.5 Binding and unbinding

- Bind exactly once per launch.
- Bind before `Validating`.
- A callback failure does not cause a second bind attempt.
- Unbind exactly once after a successful bind when the root is destroyed.
- Duplicate roots never bind.
- Headless fallback participates without side effects.

### 4.6 Progress observation

After `LaunchSession.Publish` accepts a snapshot:

1. Presenter receives the accepted snapshot.
2. If the root remains live, public state/progress events dispatch.

This preserves accepted-state visibility for presentation while retaining destruction safety.

### 4.7 Terminal observation

After terminal report finalization:

1. `LastReport` is assigned.
2. Presenter receives that exact report.
3. If the root remains live, matching public terminal event dispatches.

---

## 5. Stable diagnostics

| Code | Meaning |
|---|---|
| `ELAUNCH-VIEW-001` | An explicitly assigned presenter component is unavailable or does not implement the presenter contract |
| `ELAUNCH-VIEW-002` | A presenter callback threw an exception |

Existing diagnostic meanings remain unchanged.

---

## 6. Explicit exclusions

FL-M4-01 does not authorize:

- `EchoDevGames.EchoLaunch.Presentation.UGUI` assembly
- `EchoLaunchStatusView`
- Canvas, CanvasGroup, Image, Text, or TextMeshPro implementation
- Default status prefab
- Splash data or playback
- Fade, hold, minimum display, skip, or reduced-motion policy
- Test Lab scenes or prefabs
- Direct-scene initializer
- Persistent-root lifetime policy
- Editor setup, validation, or repair
- Configuration migration
- Report export
- Normal mid-game scene travel
- EchoUI bridge
- Player builds
- Performance claims
- Package version change

---

## 7. Implementation sequence

### Phase 1 — Neutral presenter boundary

1. Add `ILaunchStatusPresenter`.
2. Add headless fallback.
3. Add safe resolver/dispatcher.
4. Modify root serialized and runtime presenter state.
5. Compile gate.

### Phase 2 — Automatic root start

1. Add serialized automatic-start field.
2. Add Unity `Start` callback.
3. Route automatic execution through `StartLaunchAsync`.
4. Add test seams.
5. Compile gate.

### Phase 3 — Presentation ordering

1. Bind before validation.
2. Present accepted snapshots before public progress events.
3. Present finalized report after `LastReport` assignment.
4. Unbind on destruction.
5. Compile gate.

### Phase 4 — Automated proof

1. Add focused automatic-start/presenter fixture.
2. Disable auto start in retained manual helpers.
3. Run complete Runtime Play Mode suite.
4. Record observed totals.

### Phase 5 — Closeout

1. Commit and push implementation.
2. Batch-generate checkpoint, test, architecture, changelog, README, index, and Current Notes.
3. Commit and push adjacent documentation.
4. Confirm clean synchronized repository.

---

## 8. Automated test matrix

New fixture proves:

1. Automatic start completes the first enabled launch.
2. Disabled automatic start remains `AuthorityClaimed`.
3. Manual start before Unity `Start` does not re-enter.
4. Presenter binds exactly once.
5. Bind receives `AuthorityClaimed`.
6. First presented snapshot is `Validating`.
7. Accepted lifecycle order reaches `Completed`.
8. Presenter receives the exact finalized report.
9. Missing presenter uses silent headless fallback.
10. Serialized presenter component resolves.
11. Invalid assigned component emits `ELAUNCH-VIEW-001` and launch continues.
12. Bind failure emits `ELAUNCH-VIEW-002` and launch continues.
13. Progress failure emits `ELAUNCH-VIEW-002` and launch continues.
14. Terminal failure does not block `LaunchCompleted`.
15. Presenter replacement after launch advancement is rejected.
16. Null presenter injection is rejected.
17. Presenter unbinds once on destruction.
18. Duplicate root never starts or binds presentation.

The implemented fixture may combine related assertions into fewer test methods. Final totals are recorded only after Unity discovery.

Starting baseline:

```text
380 passed
0 failed
0 ignored
```

Current generated fixture target:

```text
16 additional tests
396 predicted total
```

The predicted total is not evidence.

---

## 9. Compile and evidence gates

### Compile gate

- Unity errors: 0.
- Unity compiler warnings: 0.
- Neutral Runtime has no uGUI reference.
- No new peer-package dependency.
- No project-assembly reference.

### Runtime gate

- All retained and new Runtime Play Mode tests pass.
- Automatic-start tests run through actual Unity `Start`.
- Manual fixtures remain deterministic.
- Presenter errors are expected diagnostics, not test failures.
- Duplicate roots produce no presenter callback.

### Git gate

- `git diff --check` passes.
- Only authorized files are staged.
- Implementation and documentation commits remain adjacent and separately evidenced.
- `main` equals `origin/main`.
- Working tree is clean.

---

## 10. Failure symptoms and fixes

### Retained manual tests begin automatically

Cause: helper activates a root but does not disable automatic startup before manually calling the launch gate.

Fix: call `SetAutomaticStartForTesting(false)` immediately after authoritative `Awake` and before any frame yield.

### Automatic launch runs twice

Cause: `Start` bypasses state or active-run checks.

Fix: use the existing `StartLaunchAsync` gate and return when state is no longer `AuthorityClaimed`.

### Every headless test logs `ELAUNCH-VIEW-001`

Cause: an intentionally absent presenter is treated as invalid.

Fix: reserve `ELAUNCH-VIEW-001` for explicitly assigned invalid components; use the no-op fallback when no component is assigned.

### Presenter exception fails launch

Cause: root invokes callbacks without the safe dispatcher.

Fix: isolate every callback through `LaunchStatusPresenterDispatcher`.

### Presenter sees unaccepted state

Cause: callback occurs before `LaunchSession.Publish`.

Fix: present only the accepted `session.Progress` snapshot.

### Completion event observes no report

Cause: presenter/event ordering occurs before `LastReport` assignment.

Fix: finalize, assign, present terminal, then dispatch public terminal event.

---

## 11. Rollback

Before implementation commit:

```cmd
git restore --staged .
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Core/EchoLaunchRoot.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs"
git clean -fd -- "Packages/com.echodevgames.echo-launch/Runtime/Presentation"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Presentation.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchAutomaticStartAndPresenterTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchAutomaticStartAndPresenterTests.cs.meta"
```

After a pushed implementation commit, use `git revert`.

---

## 12. Commit plan

### Implementation

```text
echo-launch: complete FL-M4-01 automatic start and presenter contract
```

### Documentation closeout

```text
echo-launch: document FL-M4-01 completion
```

No commit or push is claimed without CMD evidence.

---

## 13. Stop point

Stop after Unity `Start` safely opens the existing root launch gate and the neutral presenter contract observes accepted progress and finalized reports.

Do not implement the default uGUI view, splashes, direct-scene helpers, Editor tooling, or Laboratory scenes.

---

## 14. Next tentative checkpoint

**FL-M4-02 — Default uGUI Plain Status View and Presentation Assembly**

Tentative only. Not authorized by FL-M4-01.

---

## 15. Approval

**Decision:** Approved through user selection
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Preserve one-run authority, isolate presentation from Runtime truth, keep missing presentation headless-safe, and defer the uGUI visual implementation.
