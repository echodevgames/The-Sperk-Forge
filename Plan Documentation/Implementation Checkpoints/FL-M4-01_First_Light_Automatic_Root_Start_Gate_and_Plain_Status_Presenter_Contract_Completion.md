# FL-M4-01 - First Light Automatic Root Start and Presenter Contract Completion

## Status

- Checkpoint: `FL-M4-01`
- Milestone: M4 - Startup Entry and Presentation
- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Implementation result: Complete and pushed
- Implementation commit: `46481b1`
- Previous documentation commit: `727b502`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Implemented Scope

- Automatic Unity `Start` launch
- Serialized automatic-start setting enabled by default
- Existing `StartLaunchAsync` gate reuse
- Manual-before-automatic re-entry prevention
- Public neutral `ILaunchStatusPresenter`
- Logging-free `NullLaunchStatusPresenter`
- Safe presenter resolver and dispatcher
- Serialized neutral `MonoBehaviour` presenter seam
- Bind-before-validation ordering
- Accepted snapshot presentation before public progress events
- Finalized report presentation after `LastReport` assignment
- Exactly-once presenter unbind during destruction
- Duplicate-root automatic-start and presenter silence
- Stable `ELAUNCH-VIEW-001`
- Stable `ELAUNCH-VIEW-002`
- Presenter callback exception containment
- Deterministic internal test seams
- Sixteen new Runtime Play Mode tests

## Evidence

- Compilation errors: `0`
- Compiler warnings: `0`
- Final Runtime Play Mode tests passed: `396`
- Final Runtime Play Mode tests failed: `0`
- Final Runtime Play Mode tests ignored: `0`
- Automatic-start/presenter fixture passed: `16`
- Automatic first launch: Pass
- Disabled automatic startup: Pass
- Manual-before-automatic one-run protection: Pass
- Presenter bind ordering: Pass
- Accepted snapshot ordering: Pass
- Exact finalized report identity: Pass
- Headless fallback: Pass
- Invalid-component containment: Pass
- Callback-failure containment: Pass
- Completion continuity: Pass
- Exactly-once unbind: Pass
- Duplicate-root silence: Pass
- Runtime uGUI independence: Preserved
- Package independence: Preserved

## Bounded Compile Corrections

- Replaced `AudioSource` with a dedicated invalid `MonoBehaviour` presenter component in the new test fixture.
- Replaced unsupported NUnit `Is.AnyOf` with a direct terminal-state boolean assertion.
- No production runtime behavior changed.

## Expected Runtime Diagnostics

Tests intentionally emitted:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`
- `ELAUNCH-VIEW-001`
- `ELAUNCH-VIEW-002`

These diagnostics are expected and do not represent compiler warnings or test failures.

## Files

Created runtime:

- `Runtime/Presentation.meta`
- `Runtime/Presentation/ILaunchStatusPresenter.cs`
- `Runtime/Presentation/ILaunchStatusPresenter.cs.meta`
- `Runtime/Presentation/LaunchStatusPresenterDispatcher.cs`
- `Runtime/Presentation/LaunchStatusPresenterDispatcher.cs.meta`
- `Runtime/Presentation/NullLaunchStatusPresenter.cs`
- `Runtime/Presentation/NullLaunchStatusPresenter.cs.meta`

Modified runtime:

- `Runtime/Core/EchoLaunchRoot.cs`

Created tests:

- `Tests/Runtime/PlayMode/EchoLaunchAutomaticStartAndPresenterTests.cs`
- Unity-generated `.meta`

Modified tests:

- `Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs`
- `Tests/Runtime/PlayMode/LaunchDestinationAndCompletedHandoffTests.cs`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs`

Created plan:

- `Plan Documentation/Checkpoint Build Plans/FL-M4-01_Automatic_Root_Start_Gate_and_Plain_Status_Presenter_Contract_Checkpoint_Build_Plan.md`

## Evidence Not Yet Run

- Default uGUI status view
- Canvas, text, progress bar, or prefab rendering
- Splash playback
- Test Lab visual proof
- Real Boot-to-destination Standalone Laboratory activation
- Player builds
- Separate clean-project installation
- External project adoption
- Performance measurements

## Exclusions Preserved

- uGUI presentation assembly
- `EchoLaunchStatusView`
- Default status prefab
- Splash definition and playback
- Fade, hold, skip, and reduced-motion policy
- Direct-scene initializer
- Persistent-root lifetime policy
- Editor setup, validation, and repair
- Normal mid-game scene travel
- EchoUI bridge
- Report export
- Public step lifecycle events
- Package version change

## Completion Decision

FL-M4-01 implementation is complete in `46481b1`.

The repository was clean and synchronized after the implementation push.

The checkpoint is ready for the adjacent documentation closeout commit.

Tentative next checkpoint: FL-M4-02 - Default uGUI Plain Status View and Presentation Assembly.
