# FL-M4-03 - First Light Deterministic Image Splash Completion

## Status

- Checkpoint: `FL-M4-03`
- Milestone: M4 - Startup Entry and Presentation
- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Implementation result: Complete and pushed
- Implementation commit: `f997a9a`
- Previous documentation commit: `cbaee24`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Implemented Scope

- Splash sequence schema 1
- Immutable image splash entries
- Stable identities
- Stable skip-policy vocabulary
- Stable playback-phase vocabulary
- Immutable presentation frames
- Immutable playback results
- Neutral splash presenter contract
- Logging-free headless fallback
- Deterministic launch-clock player
- Ordered traversal
- Fade, hold, and fade-out
- Minimum-display expansion
- Early skip latching
- Disallowed skip containment
- Reduced-motion fade removal
- Cancellation cleanup
- Player-local re-entry protection
- Invalid/backward clock rejection
- Definition immutability
- Default uGUI image projection
- Public neutral skip request
- Clear and unbind cleanup
- Twenty-six Runtime tests
- Ten isolated uGUI tests

## Evidence

- Compilation errors: `0`
- Compiler warnings: `0`
- Final Runtime Play Mode tests passed: `450`
- Final Runtime Play Mode tests failed: `0`
- Final Runtime Play Mode tests ignored: `0`
- New splash tests passed: `36`
- Definition validation: Pass
- Deterministic timing: Pass
- Minimum display: Pass
- Skip policy: Pass
- Reduced motion: Pass
- Cancellation cleanup: Pass
- Re-entry protection: Pass
- Invalid clock containment: Pass
- Headless fallback: Pass
- Asset immutability: Pass
- uGUI projection: Pass
- Public skip event: Pass
- Missing-reference safety: Pass
- Package independence: Preserved

## Test-Harness Corrections

- Removed a zero-advance synchronous manual-clock infinite loop.
- Requested skips from active frame presentation.
- Consumed the faulted re-entry `Awaitable` for NUnit.
- Compared untouched generated sequence identities.
- Confirmed no production Runtime or presentation changes were required.

## Schema Boundary

- Configuration schema remains `3`.
- Report schema remains `2`.
- No configuration splash reference exists.
- Root-owned splash playback is not implemented.
- Splash results are not included in launch reports.
- Project input remains outside EchoLaunch.

## Files

Modified:

- `Presentation.UGUI/EchoLaunchStatusView.cs`

Created:

- `Runtime/Presentation/IImageSplashPresenter.cs`
- `Runtime/Presentation/NullImageSplashPresenter.cs`
- `Runtime/Splash/SplashEntry.cs`
- `Runtime/Splash/SplashPlaybackPhase.cs`
- `Runtime/Splash/SplashPlaybackResult.cs`
- `Runtime/Splash/SplashPresentationFrame.cs`
- `Runtime/Splash/SplashSequence.cs`
- `Runtime/Splash/SplashSequencePlayer.cs`
- `Runtime/Splash/SplashSkipPolicy.cs`
- `Tests/Runtime/PlayMode/SplashSequencePlayerTests.cs`
- `Tests/Presentation.UGUI/PlayMode/EchoLaunchSplashPresentationTests.cs`
- Unity-generated folder and script `.meta` files
- `Plan Documentation/Checkpoint Build Plans/FL-M4-03_Image_Splash_Definitions_and_Deterministic_Splash_Player_Checkpoint_Build_Plan.md`

## Evidence Not Yet Run

- Configuration-bound splash sequence
- Root-owned splash execution
- Lifecycle placement
- Report integration
- Project input binding
- Package prefab/art
- Test Lab visual scene
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Exclusions Preserved

- Configuration schema advancement
- Report schema change
- Root integration
- Prefab YAML
- Input binding or EchoInput bridge
- Legal-splash semantics
- Video playback
- Custom animation adapters
- Editor setup and repair
- Test Lab scenes
- Package version change

## Completion Decision

FL-M4-03 implementation is complete in `f997a9a`.

The repository was clean and synchronized after the implementation push.

The checkpoint is ready for the adjacent documentation closeout commit.

Tentative next checkpoint: FL-M4-04 - Splash Configuration Schema and Root
Playback Integration. Because it changes serialized configuration shape, it
requires explicit authority promotion before implementation.
