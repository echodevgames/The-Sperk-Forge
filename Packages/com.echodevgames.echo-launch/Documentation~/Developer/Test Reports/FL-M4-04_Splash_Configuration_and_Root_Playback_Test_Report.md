# FL-M4-04 - Splash Configuration and Root Playback Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Checkpoint: `FL-M4-04`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.5.0
- ADR: EchoLaunch-ADR-002
- Unity baseline: `6000.3.8f1`
- Authority commit: `90aabd1`
- Implementation commit: `858808b`
- Test layer: Runtime Play Mode
- Focused fixture: `EchoLaunchRootSplashLifecycleTests`
- Final result: Pass

## Final Totals

- Passed: `479`
- Failed: `0`
- Ignored: `0`
- Compilation errors: `0`
- Compiler warnings: `0`

## Focused Root Fixture

`EchoLaunchRootSplashLifecycleTests`

- Discovered: `28`
- Passed: `28`
- Failed: `0`
- Ignored: `0`

Verified:

1. Stable splash diagnostic codes.
2. Assigned splash sequence configuration.
3. Reduced-motion configuration.
4. Root splash exposure.
5. Null splash no-op.
6. Empty splash no-op.
7. Invalid startup sequence blocks before splash.
8. Invalid splash identity blocks.
9. Unsupported splash schema blocks.
10. Null splash entry blocks.
11. Missing image blocks.
12. Duplicate entry ID blocks.
13. Splash presents before the first startup step.
14. Splash clears before startup-step presentation.
15. Startup step completes before destination loading.
16. Reduced motion removes fade frames.
17. Missing visual presenter warns and continues headless.
18. Presenter skip request shortens the splash.
19. Successful report elapsed time includes splash time.
20. Root retains the successful splash result.
21. Presenter failure blocks step and destination.
22. Cancellation during splash interrupts exactly once.
23. Duplicate root produces no second splash.
24. Direct-scene mode uses the same contract.
25. Configuration remains immutable.
26. Splash sequence remains immutable.
27. Launch report schema remains 2.
28. Automatic Unity `Start` uses the splash path.

## Retained Configuration Fixture

`LaunchConfigurationBindingTests`

Added and passed:

- Historical schema 3 is unsupported without runtime rewrite.

## Configuration and Preflight Result

Pass:

- Current schema is 4.
- Optional splash reference and reduced-motion default bind.
- Historical schema 3 blocks without rewrite.
- Null and empty splash paths are valid.
- Invalid splash definitions block before presentation, steps, and destination.
- Invalid startup sequence blocks before splash presentation.
- Runtime performs no asset migration or repair.

## Ordering Result

Pass:

```text
optional splash
    -> startup steps
    -> destination loading
```

Splash clears before startup-step presentation.

Destination loading begins only after the startup sequence settles.

Automatic-start and direct-scene routes use the same contract.

## Presentation Result

Pass:

- Active status presenter is reused when it implements
  `IImageSplashPresenter`.
- Missing visual presenter emits `ELAUNCH-SPLASH-003`.
- Headless fallback preserves authored timing.
- Project-routed skip reaches active splash playback.

## Failure and Cancellation Result

Pass:

- Invalid sequence produces `ELAUNCH-SPLASH-001`.
- Unexpected playback/presenter failure produces `ELAUNCH-SPLASH-002`.
- Root cancellation uses the existing interrupted settlement.
- Failure and interruption prevent startup-step and destination side effects.
- Cancellation publishes exactly one interrupted result/event.
- Duplicate roots produce no additional splash side effect.

## Clock and Reporting Result

Pass:

- Splash playback uses the injected launch clock.
- Startup execution uses the same clock seam.
- Total elapsed launch time includes splash time.
- Root retains the successful `SplashPlaybackResult`.
- `LaunchReport.CurrentSchemaVersion` remains `2`.
- No splash-specific report fields were added.

## Asset Immutability Result

Pass:

- Configuration asset remains unchanged.
- Splash sequence and entries remain unchanged.
- Runtime performs no migration or repair.

## Evidence Not Run

- Editor schema migration
- Startup presentation prefab
- Canvas art/layout
- Project input binding
- Direct-scene initializer tooling
- Standalone Laboratory scene
- Player builds
- Clean-project installation
- External-project adoption
- Performance measurements

## Final Decision

FL-M4-04 automated evidence passes.

The implementation may be documented and closed in an adjacent commit.
