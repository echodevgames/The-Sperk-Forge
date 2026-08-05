# FL-M3-01 Startup Sequence Runner Immediate Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode
- Implementation commit: `0864b9c`

## Result

FL-M3-01 execution-state tests:

- Passed: `12`
- Failed: `0`
- Ignored: `0`

FL-M3-01 immediate-runner tests:

- Passed: `18`
- Failed: `0`
- Ignored: `0`

FL-M3-01 subtotal:

- Passed: `30`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `199`
- Failed: `0`
- Ignored: `0`

## Verified Execution State

- Authored metadata copying
- Initial `NotStarted` state
- `Begin()` transition to `Running`
- Repeated begin rejection
- Progress rejection before begin
- Progress capture while running
- Completion rejection before begin
- Null terminal result rejection
- Terminal status and result capture
- Repeated completion rejection
- Progress rejection after completion
- Invalid construction rejection without asset mutation

## Verified Immediate Runner Behavior

- Null configuration rejection
- Missing sequence rejection
- Empty sequence result
- Disabled-entry count
- Disabled-entry factory suppression
- One factory call per enabled entry
- One invocation per enabled entry
- Fresh executors across repeated runs
- Configuration, sequence, entry, and step identity delivery
- Authored index delivery
- Complete authored entry count
- Cancellation-token pass-through
- Immediate progress capture
- Success-result preservation
- Warning-result preservation
- Recoverable-failure preservation
- Blocking-failure preservation
- Authored execution order
- Continued traversal after blocking result
- Null executor rejection
- Configuration, sequence, entry, policy, and definition immutability

## Immediate-Only Boundary

The runner tests use test executors that complete synchronously through Unity `Awaitable<StartupStepResult>`.

This checkpoint does not prove:

- Multi-frame asynchronous completion
- Timeout racing
- Cancellation orchestration
- Exception conversion
- Policy application
- Root or lifecycle integration

## Compile Correction

The first Phase C build failed because `StartupSequenceRunner` referenced `LaunchMode.None`.

The package enum defines `LaunchMode.Unknown`.

The runner guard was corrected to reject `Unknown` and undefined enum values.

Unity then compiled with zero errors.

## Diagnostic Evidence

Retained duplicate-root tests intentionally generated:

    [ELAUNCH-ROOT-001] Duplicate EchoLaunchRoot rejected. The first valid root remains authoritative.

Retained notification tests intentionally generated:

    [ELAUNCH-EVENT-001] Listener failure while dispatching the lifecycle notification.

These warnings were expected and did not count as failures.

## Manual Verification

No production asset, scene, prefab, or root setup was required.

Observed:

- Zero compiler errors after the bounded enum correction
- No automatic root creation
- No automatic sequence execution
- No lifecycle transition outside retained tests
- No unexpected warning

## Scope Limit

This report proves immediate internal traversal and runtime attempt-state behavior only.

It does not prove production launch integration, policy enforcement, exception conversion, timeouts, retries, reports, presentation, scene loading, Player builds, clean-project installation, or performance budgets.
