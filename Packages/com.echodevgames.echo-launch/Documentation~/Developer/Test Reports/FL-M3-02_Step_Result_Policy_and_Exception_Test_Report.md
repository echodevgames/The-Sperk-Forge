# FL-M3-02 Step Result Policy and Exception Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode
- Implementation commit: `6f2ab12`

## Result

FL-M3-02 policy-application tests:

- Passed: `16`
- Failed: `0`
- Ignored: `0`

FL-M3-02 runner policy and exception tests:

- Passed: `16`
- Failed: `0`
- Ignored: `0`

FL-M3-02 subtotal:

- Passed: `32`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `231`
- Failed: `0`
- Ignored: `0`

Compilation:

- Errors: `0`
- Warnings: `0`

## Verified Policy Behavior

- Null decision inputs rejected
- Preserved decisions retain the original result instance
- Converted decisions use a new immutable result
- Success preserves and continues
- Warning preserves and continues
- Skipped preserves and continues
- Recoverable failure plus continue becomes warning
- Blocking failure plus continue becomes warning
- Recoverable failure plus block becomes blocking
- Blocking failure plus block remains blocking
- Timed out plus continue becomes warning
- Timed out plus block becomes blocking
- Cancelled remains cancelled and stops
- Converted code, message, and details are preserved
- Explicit failure action overrides unusual required/optional pairing

## Verified Exception and Runner Behavior

- Factory exception becomes blocking `ELAUNCH-STEP-004`
- Null executor becomes blocking `ELAUNCH-STEP-004`
- Factory failure prevents later factory creation
- Executor exception plus continue becomes warning
- Executor exception plus block becomes blocking
- Null executor result becomes blocking `ELAUNCH-STEP-004`
- Exception details contain type and trimmed message
- Exception details exclude stack traces
- `OperationCanceledException` escapes generic conversion
- Returned recoverable result follows policy
- Returned blocking result follows policy
- Early-stop accounting balances authored entries
- Stopping authored index is correct
- Complete traversal has no unvisited entries
- Authored configuration, sequence, entry, policy, and definitions remain unchanged

## Stable Diagnostic

Step factory, executor, and contract failures use:

    ELAUNCH-STEP-004

The diagnostic is captured in `StartupStepResult`.

The runner does not emit it as a Console warning during normal structured containment.

## Cancellation Boundary

`OperationCanceledException` is explicitly excluded from generic conversion.

FL-M3-02 proves only that it escapes the generic failure path.

Cancellation request, linked tokens, timeout-triggered cancellation, and cancelled run-result orchestration remain outside this checkpoint.

## Compile Warning Cleanup

The immediate test executor intentionally returns an already-completed Unity `Awaitable<StartupStepResult>` without awaiting another operation.

The test helper now suppresses `CS1998` locally.

Final Unity compilation reported:

- Zero errors
- Zero warnings

## Retained Diagnostic Evidence

Expected duplicate-root tests produced:

    [ELAUNCH-ROOT-001] Duplicate EchoLaunchRoot rejected. The first valid root remains authoritative.

Expected listener-containment tests produced:

    [ELAUNCH-EVENT-001] Listener failure while dispatching the lifecycle notification.

These warnings were expected and did not count as failures.

## Scope Limit

This report proves policy application, bounded exception conversion, early traversal stops, accounting metadata, and retained immutability.

It does not prove timeout measurement, cancellation orchestration, retries, reports, preflight, production root integration, lifecycle advancement, presentation, scene loading, Player builds, clean-project installation, or performance budgets.
