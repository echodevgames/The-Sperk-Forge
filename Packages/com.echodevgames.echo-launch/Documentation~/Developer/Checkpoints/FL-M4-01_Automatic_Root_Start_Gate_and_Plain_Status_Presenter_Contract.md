# FL-M4-01 - Automatic Root Start Gate and Plain Status Presenter Contract

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Checkpoint: `FL-M4-01`
- Milestone: M4 - Startup Entry and Presentation
- Implementation status: Complete and pushed
- Implementation commit: `46481b1`
- Previous documentation commit: `727b502`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Allow the authoritative root to begin automatically from Unity `Start` through the existing one-run gate, and establish a neutral startup-status presentation contract without implementing the default uGUI view.

## Implemented Contract

### Automatic Root Start

The root now has a serialized automatic-start setting enabled by default.

Unity entry:

```csharp
private async Awaitable Start()
```

The callback exits without work when automatic startup is disabled or when the launch session has already advanced.

When enabled and still idle, it calls the same `StartLaunchAsync` boundary used by manual callers.

### One-Run Protection

Automatic start does not bypass:

- Authoritative-root validation
- `AuthorityClaimed` start-state requirement
- Atomic active-run gate
- Existing lifecycle diagnostics
- Existing report and terminal-event ordering

A manual launch started before Unity `Start` prevents a later automatic re-entry.

### Neutral Presenter Contract

Added public:

```csharp
public interface ILaunchStatusPresenter
{
    void Bind(LaunchProgressSnapshot initialSnapshot);
    void Present(LaunchProgressSnapshot snapshot);
    void PresentTerminal(LaunchReport report);
    void Unbind();
}
```

The presenter observes immutable accepted state and finalized reports. It does not own launch authority, lifecycle, work execution, loading, report construction, or UI navigation.

### Serialized Presenter Seam

The neutral Runtime root stores:

```csharp
[SerializeField]
private MonoBehaviour statusPresenterComponent;
```

The component is resolved to `ILaunchStatusPresenter`.

This preserves a neutral Runtime assembly while permitting a later isolated uGUI presenter assembly.

### Headless Fallback

When no presenter component is assigned, the root uses a logging-free `NullLaunchStatusPresenter`.

An explicitly assigned component that does not implement the presenter contract:

- Emits `ELAUNCH-VIEW-001`
- Falls back to the headless presenter
- Does not block launch

### Safe Presenter Dispatch

`LaunchStatusPresenterDispatcher` contains every callback.

Presenter exceptions:

- Emit `ELAUNCH-VIEW-002`
- Are sanitized
- Do not alter launch state
- Do not block later lifecycle or public event work

### Presentation Ordering

Binding:

1. Authority already exists.
2. Presenter binds once with the `AuthorityClaimed` snapshot.
3. Root begins validation.

Progress:

1. Session accepts the snapshot.
2. Presenter receives the accepted snapshot.
3. Public state/progress events dispatch if the root remains live.

Terminal:

1. Terminal state is accepted.
2. Immutable report is finalized.
3. `LastReport` is assigned.
4. Presenter receives that exact report.
5. Matching public terminal event dispatches if the root remains live.

### Destruction and Duplicate Roots

- A successfully bound presenter unbinds exactly once when the root is destroyed.
- Duplicate roots never automatically start.
- Duplicate roots never bind or present status.
- Presenter callbacks cannot resurrect or replace launch truth.

## Files

Modified runtime:

- `Runtime/Core/EchoLaunchRoot.cs`

Created runtime:

- `Runtime/Presentation.meta`
- `Runtime/Presentation/ILaunchStatusPresenter.cs`
- `Runtime/Presentation/ILaunchStatusPresenter.cs.meta`
- `Runtime/Presentation/LaunchStatusPresenterDispatcher.cs`
- `Runtime/Presentation/LaunchStatusPresenterDispatcher.cs.meta`
- `Runtime/Presentation/NullLaunchStatusPresenter.cs`
- `Runtime/Presentation/NullLaunchStatusPresenter.cs.meta`

Created tests:

- `Tests/Runtime/PlayMode/EchoLaunchAutomaticStartAndPresenterTests.cs`
- `Tests/Runtime/PlayMode/EchoLaunchAutomaticStartAndPresenterTests.cs.meta`

Modified retained tests:

- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs`

Checkpoint plan:

- `Plan Documentation/Checkpoint Build Plans/FL-M4-01_Automatic_Root_Start_Gate_and_Plain_Status_Presenter_Contract_Checkpoint_Build_Plan.md`

## Compile Corrections

Two bounded new-test corrections were required:

1. `AudioSource` was replaced with a dedicated invalid `MonoBehaviour` presenter component because the serialized root field accepts `MonoBehaviour`.
2. Unsupported NUnit `Is.AnyOf` was replaced with a direct terminal-state boolean assertion.

No production runtime code changed for these corrections.

Final compilation:

- Errors: `0`
- Compiler warnings: `0`

## Test Evidence

New automatic-start and presenter fixture:

- Passed: `16`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `396`
- Failed: `0`
- Ignored: `0`

Verified:

- Automatic first launch
- Disabled automatic startup
- Manual-before-automatic one-run behavior
- Bind-before-validation ordering
- Accepted lifecycle presentation order
- Exact finalized report identity
- Headless fallback
- Serialized presenter resolution
- Invalid-component fallback
- Bind/progress/terminal callback containment
- Completion event continuity
- Presenter replacement guard
- Null injection guard
- Exactly-once unbind
- Duplicate-root silence

## Expected Diagnostics

Tests intentionally emit:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`
- `ELAUNCH-VIEW-001`
- `ELAUNCH-VIEW-002`

These are expected runtime diagnostics, not compiler warnings or test failures.

## Evidence Not Yet Run

- Default uGUI status view
- Canvas, prefab, text, or progress-bar rendering
- Splash playback
- Test Lab scene presentation
- Real Boot-to-destination Standalone Laboratory activation
- Player builds
- Separate clean-project installation
- External project adoption
- Performance measurements

## Exclusions Preserved

- uGUI presentation assembly
- `EchoLaunchStatusView`
- Default presentation prefab
- Splash definition/playback
- Fade, hold, skip, and reduced-motion policy
- Direct-scene initializer
- Persistent-root lifetime policy
- Editor setup, validation, and repair
- Configuration migration
- Normal mid-game scene travel
- EchoUI bridge
- Report export
- Public step lifecycle events
- Package version change

## Closure Result

FL-M4-01 implementation is complete in commit `46481b1`.

The implementation compiles with 0 errors and 0 compiler warnings.

All 396 Runtime Play Mode tests pass with 0 failed and 0 ignored.

The checkpoint is ready for its adjacent documentation closeout.

Tentative next checkpoint: FL-M4-02 - Default uGUI Plain Status View and Presentation Assembly.
