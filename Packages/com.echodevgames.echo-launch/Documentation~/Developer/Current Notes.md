# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M4-03`
- Title: Image Splash Definitions and Deterministic Splash Player
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Implementation status: Complete and pushed
- Implementation commit: `f997a9a`
- Previous documentation commit: `cbaee24`
- Documentation closeout: Pending adjacent commit
- Runtime Play Mode result: 450 passed, 0 failed, 0 ignored
- Compilation result: 0 errors, 0 compiler warnings

## Completed Result

Implemented:

- `SplashSequence` schema 1
- Immutable `SplashEntry`
- Stable splash skip and playback vocabulary
- Immutable presentation frames and results
- Neutral `IImageSplashPresenter`
- Logging-free headless fallback
- Deterministic `SplashSequencePlayer`
- Ordered traversal
- Fade, hold, and fade-out
- Minimum-display expansion
- Latched early skip
- Disallowed skip containment
- Reduced-motion fade removal
- Cancellation and re-entry containment
- Invalid/backward clock rejection
- Definition immutability
- uGUI image, label, alpha, and position
- Public `RequestSplashSkip()`
- Splash clear and unbind cleanup
- Twenty-six Runtime tests
- Ten isolated uGUI tests

## Evidence Summary

### Final Pass

- Runtime Play Mode: 450 passed, 0 failed, 0 ignored
- New splash tests: 36 passed
- Compilation: 0 errors, 0 compiler warnings
- Implementation commit `f997a9a` pushed to `main` and `origin/main`
- Working tree clean after implementation push

### Corrections

- Diagnosed a zero-advance synchronous manual-clock loop.
- Corrected skip requests that occurred after synchronous completion.
- Consumed the faulted concurrent-playback `Awaitable` in the NUnit assertion.
- Compared untouched generated sequence IDs in the uniqueness test.
- No production Runtime or presentation behavior changed.

### Schema Boundary

- Configuration schema remains `3`.
- Report schema remains `2`.
- No splash reference was added to configuration.
- Root-owned splash playback is not implemented.
- Splash results are not reported through `LaunchReport`.

### Not Run

- Configuration-bound splash sequence
- Root-owned splash playback
- Project input binding
- Package prefab
- Canvas art/layout
- Test Lab visual scene
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Changed Files

Modified:

- `Presentation.UGUI/EchoLaunchStatusView.cs`

New neutral presentation:

- `Runtime/Presentation/IImageSplashPresenter.cs`
- `Runtime/Presentation/NullImageSplashPresenter.cs`

New Runtime splash system:

- `Runtime/Splash/SplashEntry.cs`
- `Runtime/Splash/SplashPlaybackPhase.cs`
- `Runtime/Splash/SplashPlaybackResult.cs`
- `Runtime/Splash/SplashPresentationFrame.cs`
- `Runtime/Splash/SplashSequence.cs`
- `Runtime/Splash/SplashSequencePlayer.cs`
- `Runtime/Splash/SplashSkipPolicy.cs`
- Unity-generated folder and script `.meta` files

New tests:

- `Tests/Runtime/PlayMode/SplashSequencePlayerTests.cs`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchSplashPresentationTests.cs`
- Unity-generated `.meta` files

## Handoff Snapshot

FL-M4-03 implementation is complete and pushed in commit `f997a9a`.

First Light now owns standalone deterministic image-splash definitions and
playback with a neutral presentation contract, reduced-motion behavior, minimum
display protection, and project-routed skip requests.

The adjacent FL-M4-03 documentation closeout is the only active repository work.

Tentative next checkpoint: FL-M4-04 - Splash Configuration Schema and Root
Playback Integration. It requires authority-first schema approval.
