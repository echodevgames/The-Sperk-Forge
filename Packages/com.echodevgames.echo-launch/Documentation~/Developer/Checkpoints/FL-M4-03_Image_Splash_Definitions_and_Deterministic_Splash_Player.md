# FL-M4-03 - Image Splash Definitions and Deterministic Splash Player

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Checkpoint: `FL-M4-03`
- Milestone: M4 - Startup Entry and Presentation
- Implementation status: Complete and pushed
- Implementation commit: `f997a9a`
- Previous documentation commit: `cbaee24`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Add project-owned image splash definitions and deterministic playback without
changing configuration schema, launch reports, or root-owned launch flow.

## Implemented Contract

### Splash Sequence

Added project-owned:

```csharp
SplashSequence : ScriptableObject
```

Properties:

- Independent schema version `1`
- Stable canonical sequence ID
- Ordered image-only entries
- Read-only runtime access
- Side-effect-free playback validation

### Splash Entry

Each immutable definition provides:

- Stable canonical entry ID
- Sprite
- Replaceable display label
- Fade-in duration
- Hold duration
- Fade-out duration
- Minimum display duration
- Skip policy

Runtime does not repair or rewrite invalid definitions.

### Deterministic Playback

`SplashSequencePlayer` uses `ILaunchClock`.

It provides:

- Ordered traversal
- Fade-in, hold, and fade-out phases
- Normalized alpha
- Minimum-display expansion
- Reduced-motion fade removal
- Latched early skips
- Disallowed-skip containment
- Cancellation cleanup
- Active-player re-entry protection
- Invalid and backward clock rejection
- Headless fallback
- Immutable final result

### Neutral Presentation

Added public:

```csharp
IImageSplashPresenter
```

The contract receives immutable frames and exposes a neutral skip-request event.

Added logging-free:

```csharp
NullImageSplashPresenter
```

### Default uGUI Projection

`EchoLaunchStatusView` now implements `IImageSplashPresenter`.

Added optional serialized references:

- Splash root
- Splash `Image`
- Splash label `Text`

The view renders:

- Sprite
- Label
- Accepted alpha
- `Splash N of M`
- Replaceable `Showing splash.` copy

Public:

```csharp
bool RequestSplashSkip()
```

This requires no EchoInput dependency.

### Deliberate Integration Stop

Not changed:

- `EchoLaunchConfiguration` schema remains `3`
- `LaunchReport` schema remains `2`
- `EchoLaunchRoot` does not own splash playback
- Splash results are not included in launch reports
- No project input binding was added

## Files

Modified:

- `Presentation.UGUI/EchoLaunchStatusView.cs`

Created neutral presentation:

- `Runtime/Presentation/IImageSplashPresenter.cs`
- `Runtime/Presentation/IImageSplashPresenter.cs.meta`
- `Runtime/Presentation/NullImageSplashPresenter.cs`
- `Runtime/Presentation/NullImageSplashPresenter.cs.meta`

Created Runtime splash folder:

- `Runtime/Splash.meta`
- `Runtime/Splash/SplashEntry.cs`
- `Runtime/Splash/SplashEntry.cs.meta`
- `Runtime/Splash/SplashPlaybackPhase.cs`
- `Runtime/Splash/SplashPlaybackPhase.cs.meta`
- `Runtime/Splash/SplashPlaybackResult.cs`
- `Runtime/Splash/SplashPlaybackResult.cs.meta`
- `Runtime/Splash/SplashPresentationFrame.cs`
- `Runtime/Splash/SplashPresentationFrame.cs.meta`
- `Runtime/Splash/SplashSequence.cs`
- `Runtime/Splash/SplashSequence.cs.meta`
- `Runtime/Splash/SplashSequencePlayer.cs`
- `Runtime/Splash/SplashSequencePlayer.cs.meta`
- `Runtime/Splash/SplashSkipPolicy.cs`
- `Runtime/Splash/SplashSkipPolicy.cs.meta`

Created tests:

- `Tests/Runtime/PlayMode/SplashSequencePlayerTests.cs`
- `Tests/Runtime/PlayMode/SplashSequencePlayerTests.cs.meta`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchSplashPresentationTests.cs`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchSplashPresentationTests.cs.meta`

Created plan:

- `Plan Documentation/Checkpoint Build Plans/FL-M4-03_Image_Splash_Definitions_and_Deterministic_Splash_Player_Checkpoint_Build_Plan.md`

## Test-Run Corrections

### Apparent freeze

The first full run appeared stuck at:

```text
SnapshotRejectsInvalidElapsedTime
```

That retained test was not responsible. It was only the last Test Runner row
painted before the main thread entered the new concurrent-playback fixture.

### Zero-advance clock loop

`ConcurrentPlaybackIsRejected` initially started deterministic playback with a
manual clock advancing by zero seconds. Because ticks completed synchronously,
the player remained in an infinite loop on the main thread.

Correction:

- Replaced the live nonadvancing playback setup with direct active-gate state
  proof.
- Preserved the intended re-entry assertion.

### Skip request timing

Three tests requested skip after synchronous deterministic playback had already
completed.

Correction:

- The recording presenter gained a frame callback.
- Skip requests now occur during accepted frame presentation.

### Final two fixture failures

The next full run completed in approximately 1.93 seconds:

- Discovered: `450`
- Passed: `448`
- Failed: `2`
- Ignored: `0`

Corrections:

1. The concurrent-playback assertion now consumes the faulted `Awaitable` so
   NUnit observes `InvalidOperationException`.
2. The sequence-identity test now compares untouched newly created assets rather
   than the fixed-ID deterministic helper.

No production Runtime or presentation behavior changed for these corrections.

## Compile Evidence

- Errors: `0`
- Compiler warnings: `0`

## Test Evidence

New neutral Runtime fixture:

- Tests: `26`
- Failed: `0`
- Ignored: `0`

New isolated uGUI fixture:

- Tests: `10`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `450`
- Failed: `0`
- Ignored: `0`

Verified:

- Stable enum vocabulary
- Sequence schema and identity
- Separate generated identities
- Entry identity
- Timing rejection
- Sequence validation
- Empty playback
- Ordered playback
- Deterministic phases and alpha
- Minimum display
- Skip timing and policy
- Reduced motion
- Cancellation
- Re-entry
- Backward clock
- Headless fallback
- Result accounting
- Asset immutability
- uGUI image projection
- Public skip event
- Clearing and unbinding
- Null-frame rejection
- Missing-reference safety

## Evidence Not Yet Run

- Configuration-bound splash sequence
- Root-owned splash playback
- Splash placement within the authoritative launch lifecycle
- Splash result inclusion in launch reports
- Project input binding
- Package-supplied prefab
- Canvas art/layout pass
- Test Lab visual scene
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Exclusions Preserved

- Configuration schema advancement
- Root integration
- Report schema change
- Prefab YAML
- Input binding or EchoInput bridge
- Legal-splash semantics
- Video playback
- Custom animation adapters
- Interactive retry/cancel UI
- Editor setup and repair
- Test Lab scenes
- Package version change

## Closure Result

FL-M4-03 implementation is complete in commit `f997a9a`.

The implementation compiles with 0 errors and 0 compiler warnings.

All 450 Runtime Play Mode tests pass with 0 failed and 0 ignored.

The checkpoint is ready for its adjacent documentation closeout.

Tentative next checkpoint: FL-M4-04 - Splash Configuration Schema and Root
Playback Integration. Because it changes serialized configuration shape, it
requires authority promotion before implementation.
