# FL-M3-04 Multi-Frame Async and Runner Cancellation Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode
- Implementation commit: `b51d722`

## Final Result

New FL-M3-04 multi-frame async tests:

- Passed: `2`
- Failed: `0`
- Ignored: `0`

Updated timeout and cancellation fixture:

- Passed: `18`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `265`
- Failed: `0`
- Ignored: `0`

Compilation:

- Errors: `0`
- Compiler warnings: `0`

## Verified Multi-Frame Behavior

- Executor work spans multiple Unity frames.
- The proof uses `Awaitable.NextFrameAsync`.
- Progress is accepted while the attempt remains active.
- The final progress belongs to the completed execution.
- Elapsed timing is positive.
- Timing remains monotonic and runtime-owned.
- A later authored entry begins only after the multi-frame executor settles.
- Authored traversal order is preserved.

## Verified Caller-Cancellation Behavior

- Caller cancellation reaches the linked executor token.
- The active executor settles before the runner returns.
- The attempted execution completes with `StartupStepStatus.Cancelled`.
- The result uses stable code `ELAUNCH-STEP-005`.
- The run reports `WasCancelled == true`.
- Cancellation stops traversal.
- The later entry remains unvisited.
- The later executor factory is not called.
- Authored `ContinueWithWarning` cannot downgrade caller cancellation.

## Verified Cancellation-Race Behavior

The first complete run reported:

- Passed: `264`
- Failed: `1`
- Ignored: `0`

Failure:

    CallerCancellationReturnsStructuredOutcome

Observed exception:

    System.OperationCanceledException

The caller token and executor settlement occurred during the same clock tick. The monitor consumed the cancelled executor before its next loop could latch caller cancellation.

The bounded runtime fix accepts either of these equivalent observations:

1. Caller cancellation was latched before settlement.
2. The executor settled with `OperationCanceledException` while the caller token was already requested.

After the fix, the complete suite passed.

## Verified Run-Result Behavior

- `StartupSequenceRunResult.WasCancelled` is true for the structured caller-cancellation run.
- Attempted, disabled, and unvisited accounting remains balanced.
- The stopping authored index identifies the cancelled entry.
- Completed execution storage remains immutable.
- Existing warning, failure, and blocking-failure summaries remain intact.

## Verified Retained Behavior

- Timeout zero remains disabled.
- Completion observable at the deadline still wins.
- Deadline crossing still creates `ELAUNCH-STEP-003`.
- Timeout cancellation remains distinct from caller cancellation.
- Late success, failure, and progress remain contained.
- Timeout policy conversion remains unchanged.
- Backward-clock behavior still becomes blocking `ELAUNCH-STEP-004`.
- Authored assets remain unchanged.

## Retained Diagnostic Evidence

Expected duplicate-root tests produced `ELAUNCH-ROOT-001`.

Expected listener-containment tests produced `ELAUNCH-EVENT-001`.

These yellow warnings were expected runtime diagnostics and did not count as failures or compiler warnings.

## Scope Limit

This report proves production-shaped multi-frame Unity async execution and structured caller cancellation at the internal sequence-runner boundary.

It does not prove automatic retry, interactive retry, root-level cancellation commands, shutdown cancellation orchestration, runner re-entry protection, sequence preflight, root integration, lifecycle advancement, reports, presentation, scene loading, Player builds, clean-project installation, or performance budgets.
