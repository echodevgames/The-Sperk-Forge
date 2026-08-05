# FL-M4-03 - Deterministic Image Splash Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Checkpoint: `FL-M4-03`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Unity baseline: `6000.3.8f1`
- Implementation commit: `f997a9a`
- Test layer: Runtime Play Mode
- Neutral fixture: `SplashSequencePlayerTests`
- Presentation fixture: `EchoLaunchSplashPresentationTests`
- Final result: Pass

## Final Totals

- Passed: `450`
- Failed: `0`
- Ignored: `0`
- Compilation errors: `0`
- Compiler warnings: `0`

## New Runtime Fixture

`SplashSequencePlayerTests`

- Discovered: `26`
- Passed: `26`
- Failed: `0`
- Ignored: `0`

Verified:

1. Stable skip-policy values.
2. Stable playback-phase values.
3. Sequence schema 1 and canonical identity.
4. Separate generated sequence identities.
5. Canonical entry identity.
6. Negative timing rejection.
7. Nonfinite timing rejection.
8. Null-entry rejection.
9. Duplicate entry-ID rejection.
10. Missing-image rejection.
11. Empty-sequence completion.
12. Single-entry authored timeline.
13. Ordered two-entry traversal.
14. Fade phase publication.
15. Normalized alpha.
16. Minimum-display expansion.
17. Permitted skip after minimum.
18. Early skip latching.
19. Disallowed skip containment.
20. Reduced-motion fade removal.
21. Cancellation cleanup.
22. Concurrent playback rejection.
23. Backward clock rejection.
24. Headless fallback.
25. Skipped-entry result accounting.
26. Authored asset immutability.

## New Presentation Fixture

`EchoLaunchSplashPresentationTests`

- Discovered: `10`
- Passed: `10`
- Failed: `0`
- Ignored: `0`

Verified:

1. View implements `IImageSplashPresenter`.
2. Pre-bind presentation is a no-op.
3. Image, label, state, and position rendering.
4. Accepted alpha application.
5. Skip request without subscriber returns false.
6. Public skip request raises the neutral event.
7. Clear hides and clears the splash surface.
8. Unbind clears splash and handlers.
9. Null frame rejection.
10. Missing optional splash references remain safe.

## Initial Hang Diagnosis

The first complete run appeared frozen at:

```text
SnapshotRejectsInvalidElapsedTime
```

The retained snapshot test was not the source. It was the last visible Test
Runner row before the next new test entered an infinite synchronous loop.

Root cause:

- `ConcurrentPlaybackIsRejected` used a manual clock with `0` seconds per tick.
- `NextTickAsync` completed synchronously.
- Elapsed time never advanced.
- The main thread never returned to NUnit or the Test Runner UI.

Correction:

- The test now proves the private active-playback gate directly.
- No production player change was required.

## Skip Fixture Timing Correction

The deterministic manual clock allows playback to complete synchronously.

Three initial tests called `RequestSkip()` after `PlayAsync()` had already
returned.

Correction:

- The recording presenter gained a frame callback.
- Tests issue skip requests from accepted presentation frames.
- Early, allowed, and disallowed policies are now tested during active playback.

## Intermediate Full Run

After the hang correction:

- Discovered: `450`
- Passed: `448`
- Failed: `2`
- Ignored: `0`
- Duration: approximately `1.93` seconds

Failures:

1. Concurrent playback returned a faulted `Awaitable`, but the NUnit assertion
   did not consume it.
2. Sequence uniqueness used a helper that deliberately assigned the same fixed
   ID to both sequences.

Corrections:

- Consume the faulted `Awaitable` through the existing completion helper.
- Compare untouched newly created `SplashSequence` assets.

No production Runtime or presentation code changed.

## Timing and Skip Result

Pass:

- Fade-in, hold, fade-out
- Zero-duration fades
- Normalized alpha
- Minimum-display expansion
- Early skip latching
- Skip after minimum
- Disallowed skip
- Reduced-motion fade removal
- Monotonic-clock enforcement

## Validation Result

Pass:

- Sequence identity
- Sequence schema
- Entry collection
- Null entry
- Entry identity
- Image requirement
- Nonnegative finite timing
- Defined skip policy
- Duplicate entry IDs

Runtime performed no asset repair or mutation.

## Lifecycle Safety Result

Pass:

- Cancellation cleanup
- Presenter clear in `finally`
- Skip-event unsubscribe
- Active-playback gate release
- Concurrent-player rejection
- Backward-clock rejection
- Headless fallback

## uGUI Result

Pass:

- Image sprite
- Label
- Alpha
- Sequence position
- Replaceable state copy
- Public neutral skip request
- Clear behavior
- Unbind cleanup
- Null-frame rejection
- Missing-reference safety

## Schema Boundary Result

Pass:

- Configuration schema remains `3`
- Report schema remains `2`
- No root integration
- No splash result in `LaunchReport`
- No EchoInput dependency

## Evidence Not Run

- Configuration-bound sequence
- Root-owned playback
- Lifecycle placement
- Report integration
- Project input binding
- Package prefab/art
- Test Lab scene
- Player builds
- Clean-project installation
- External-project adoption
- Performance measurements

## Final Decision

FL-M4-03 automated evidence passes.

The implementation may be documented and closed in an adjacent commit.
