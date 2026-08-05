# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M4-01`
- Title: Automatic Root Start Gate and Plain Status Presenter Contract
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Implementation status: Complete and pushed
- Implementation commit: `46481b1`
- Previous documentation commit: `727b502`
- Documentation closeout: Pending adjacent commit
- Runtime Play Mode result: 396 passed, 0 failed, 0 ignored
- Compilation result: 0 errors, 0 compiler warnings

## Completed Result

Implemented:

- Automatic root startup from Unity `Start`
- Serialized automatic-start setting enabled by default
- Existing `StartLaunchAsync` one-run gate reuse
- Manual-before-automatic re-entry prevention
- Public neutral `ILaunchStatusPresenter`
- Silent `NullLaunchStatusPresenter`
- Safe presenter resolver and dispatcher
- Serialized neutral `MonoBehaviour` presenter seam
- Binding before validation
- Accepted snapshot presentation before public progress events
- Finalized report presentation after `LastReport` assignment
- Presenter unbinding during destruction
- Duplicate-root automatic-start and presenter silence
- `ELAUNCH-VIEW-001`
- `ELAUNCH-VIEW-002`
- Per-callback presenter exception containment
- Deterministic automatic-start and presenter test seams
- Sixteen new Runtime Play Mode tests

## Evidence Summary

### Final Pass

- Runtime Play Mode: 396 passed, 0 failed, 0 ignored
- New automatic-start/presenter fixture: 16 passed
- Compilation: 0 errors, 0 compiler warnings
- Implementation commit `46481b1` pushed to `main` and `origin/main`
- Working tree clean after implementation push

### Compile Corrections

- Replaced `AudioSource` with a dedicated invalid `MonoBehaviour` presenter component in the new fixture.
- Replaced unsupported NUnit `Is.AnyOf` with a direct terminal-state assertion.
- No production runtime behavior changed.

### Expected Diagnostics

Retained and new tests intentionally generate:

    ELAUNCH-ROOT-001
    ELAUNCH-EVENT-001
    ELAUNCH-VIEW-001
    ELAUNCH-VIEW-002

These are expected runtime diagnostics, not compiler warnings or failures.

### Not Run

- Default uGUI status view
- Presentation assembly
- Canvas, prefab, text, or progress-bar rendering
- Splash playback
- Test Lab scene presentation
- Real Boot-to-destination Standalone Laboratory activation
- Direct-scene initialization
- Persistent-root policy
- Editor setup and repair
- Player builds
- Separate clean-project installation
- External project adoption
- Performance measurements

## Changed Files

Modified runtime:

- `Runtime/Core/EchoLaunchRoot.cs`

New runtime:

- `Runtime/Presentation.meta`
- `Runtime/Presentation/ILaunchStatusPresenter.cs`
- `Runtime/Presentation/LaunchStatusPresenterDispatcher.cs`
- `Runtime/Presentation/NullLaunchStatusPresenter.cs`
- Unity-generated script `.meta` files

Automated tests:

- `Tests/Runtime/PlayMode/EchoLaunchAutomaticStartAndPresenterTests.cs`
- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs`
- Unity-generated `.meta`

## Handoff Snapshot

FL-M4-01 implementation is complete and pushed in commit `46481b1`.

The authoritative root can now begin automatically from Unity `Start`, and a neutral presenter can observe accepted progress and finalized reports without owning launch truth or pulling uGUI into Runtime.

The adjacent FL-M4-01 documentation closeout is the only active repository work.

Tentative next checkpoint: FL-M4-02 - Default uGUI Plain Status View and Presentation Assembly.
