# FL-M4-02 - Default uGUI Plain Status View and Presentation Assembly

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Checkpoint: `FL-M4-02`
- Milestone: M4 - Startup Entry and Presentation
- Implementation status: Complete and pushed
- Implementation commit: `0e049ef`
- Previous documentation commit: `e4367bf`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Create the first default visual implementation of the neutral First Light
presenter contract without coupling the neutral Runtime assembly to uGUI.

## Implemented Contract

### Separate Presentation Assembly

Created:

```text
EchoDevGames.EchoLaunch.Presentation.UGUI
```

The assembly references:

- `EchoDevGames.EchoLaunch.Runtime`
- `Unity.ugui`

The neutral Runtime asmdef remains unchanged and uGUI-free.

### Plain Status View

Added public:

```csharp
EchoLaunchStatusView :
    MonoBehaviour,
    ILaunchStatusPresenter
```

The view renders accepted immutable snapshots and finalized reports. It does not
own authority, lifecycle transitions, startup execution, destination loading,
or report construction.

### Serialized Visual References

The view supports optional serialized references for:

- `CanvasGroup`
- State `Text`
- Message `Text`
- Step `Text`
- Progress `Text`
- Elapsed `Text`
- Determinate `Slider`
- Determinate progress root
- Indeterminate progress root

Missing optional references do not throw or block launch.

### Text-Complete State Copy

Default replaceable copy includes:

- Preparing launch.
- Validating launch.
- Starting systems.
- Continuing with a warning.
- Loading destination.
- Launch complete.
- Launch blocked.
- Launch interrupted.

Meaning does not require color.

### Determinate and Indeterminate Progress

Determinate behavior:

- Slider normalized to `0..1`
- Percentage text
- Determinate surface active
- Indeterminate surface inactive

Indeterminate behavior:

- Distinct progress surface
- Configurable `Working...` copy
- Determinate surface inactive

### Step and Time Readout

When a step is active, the view displays:

```text
Step N of M - stable-step-id
```

Elapsed launch time displays with one decimal place.

### Terminal Report Projection

Completed:

- Retains the exact report
- Shows completed copy
- Shows destination display name
- Shows final message
- Forces progress to 100 percent

Failed:

- Shows blocked copy
- Shows diagnostic code and message
- Preserves latest progress mode

Interrupted:

- Shows interrupted copy
- Shows cancellation code and message
- Preserves latest progress mode

### Bind and Unbind

The view supports:

- Show on bind
- Hide on unbind
- Optional clear on unbind
- Rebinding after unbind
- Previous terminal report reset on bind

### Test Isolation

Created:

```text
EchoDevGames.EchoLaunch.Tests.Presentation.UGUI
```

The Runtime and presentation assemblies grant bounded internal access only to
this dedicated test assembly.

## Files

Modified:

- `Runtime/Properties/AssemblyInfo.cs`

Created presentation runtime:

- `Presentation.UGUI.meta`
- `Presentation.UGUI/EchoDevGames.EchoLaunch.Presentation.UGUI.asmdef`
- `Presentation.UGUI/EchoDevGames.EchoLaunch.Presentation.UGUI.asmdef.meta`
- `Presentation.UGUI/EchoLaunchStatusView.cs`
- `Presentation.UGUI/EchoLaunchStatusView.cs.meta`
- `Presentation.UGUI/Properties.meta`
- `Presentation.UGUI/Properties/AssemblyInfo.cs`
- `Presentation.UGUI/Properties/AssemblyInfo.cs.meta`

Created presentation tests:

- `Tests/Presentation.UGUI.meta`
- `Tests/Presentation.UGUI/EchoDevGames.EchoLaunch.Tests.Presentation.UGUI.asmdef`
- `Tests/Presentation.UGUI/EchoDevGames.EchoLaunch.Tests.Presentation.UGUI.asmdef.meta`
- `Tests/Presentation.UGUI/PlayMode.meta`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchStatusViewTests.cs`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchStatusViewTests.cs.meta`

Created plan:

- `Plan Documentation/Checkpoint Build Plans/FL-M4-02_Default_uGUI_Plain_Status_View_and_Presentation_Assembly_Checkpoint_Build_Plan.md`

## Compile Corrections

Three bounded repository/test corrections were required:

1. Added the missing presentation namespace import to the test fixture.
2. Replaced thirteen unsupported NUnit `Assert.Multiple` blocks with sequential
   assertions.
3. Removed generated `.slnx` noise and Unity-generated trailing whitespace from
   the new `.meta` files before staging.

No production presentation behavior changed for these corrections.

Final compilation:

- Errors: `0`
- Compiler warnings: `0`

## Test Evidence

New presentation fixture:

- Passed: `18`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `414`
- Failed: `0`
- Ignored: `0`

Verified:

- Interface implementation
- Bind visibility and authority copy
- Determinate slider and percentage
- Indeterminate progress
- Step position and identity
- Elapsed-time formatting
- Warning diagnostic rendering
- Transitioning copy
- Completed destination and full progress
- Failed diagnostics
- Interrupted cancellation details
- Pre-bind snapshot no-op
- Pre-bind terminal no-op
- Null report rejection
- Hide-on-unbind
- Clear-on-unbind
- Rebind report reset
- Missing optional-reference safety
- Serialized copy replacement

## Expected Diagnostics

Retained tests intentionally emit:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`
- `ELAUNCH-VIEW-001`
- `ELAUNCH-VIEW-002`

These are expected runtime diagnostics, not compiler warnings or test failures.

## Evidence Not Yet Run

- Package-supplied presentation prefab
- Canvas art/layout pass
- Font and color asset selection
- Project logo or background
- Splash definition and playback
- Test Lab visual scene proof
- Real Boot-to-destination Standalone Laboratory activation
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Exclusions Preserved

- Prefab YAML
- Canvas hierarchy generation
- Art direction
- Logo/background assets
- Splash sequence definitions
- Splash playback
- Fade, hold, skip, and reduced-motion behavior
- Automatic prefab discovery
- Root hierarchy creation
- Direct-scene initializer
- Editor setup and repair
- Test Lab scenes
- Persistent-root lifetime policy
- EchoUI bridge
- Package version change

## Closure Result

FL-M4-02 implementation is complete in commit `0e049ef`.

The implementation compiles with 0 errors and 0 compiler warnings.

All 414 Runtime Play Mode tests pass with 0 failed and 0 ignored.

The checkpoint is ready for its adjacent documentation closeout.

Tentative next checkpoint: FL-M4-03 - Image Splash Definitions and
Deterministic Splash Player.
