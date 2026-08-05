# FL-M3-04 - Multi-Frame Async Proof and Runner Cancellation Outcome

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M3-04`
- Milestone: M3 - Startup Sequence
- Implementation status: Complete and pushed
- Implementation commit: `b51d722`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Prove production-shaped multi-frame Unity `Awaitable` execution and convert caller cancellation into one immutable structured runner outcome after the active executor settles.

## Authorized Files

Modified runtime files:

    Runtime/Execution/StartupSequenceRunResult.cs
    Runtime/Execution/StartupSequenceRunner.cs
    Runtime/Execution/StartupStepAwaitOutcome.cs
    Runtime/Execution/StartupStepTimeoutMonitor.cs

Modified retained test:

    Tests/Runtime/PlayMode/StartupSequenceRunnerTimeoutTests.cs

New test:

    Tests/Runtime/PlayMode/StartupSequenceRunnerMultiFrameAsyncTests.cs
    Required Unity .meta file

Plan:

    Plan Documentation/Checkpoint Build Plans/FL-M3-04_Multi-Frame_Async_Proof_and_Runner_Cancellation_Outcome_Checkpoint_Build_Plan.md

## Implemented Contract

### Multi-Frame Unity Async Proof

The new Play Mode fixture uses `Awaitable.NextFrameAsync` rather than an immediate completion source.

It proves:

- One executor remains active across multiple rendered frames.
- Progress is accepted while the attempt is active.
- Settlement records positive monotonic elapsed time.
- Authored traversal order is preserved after settlement.
- The runner does not treat production-shaped asynchronous work as an immediate test double.

### Caller-Cancellation Observation

`StartupStepAwaitOutcome` now records:

    CallerCancellationObserved

The value is immutable and is produced only after the monitor consumes the executor observation and captures final timing.

### Structured Cancellation Result

Caller cancellation produces:

    Status: Cancelled
    Code: ELAUNCH-STEP-005
    Message: Startup-sequence execution was cancelled by the caller.

The cancellation result is runner-owned.

Authored `ContinueWithWarning` policy cannot convert caller cancellation to warning or continue traversal.

### Settlement Safety

The runner still never abandons an active executor.

When caller cancellation is requested:

1. The linked attempt token is cancelled.
2. The executor settles.
3. The monitor captures the settled result or exception.
4. The runner completes the attempt as `Cancelled`.
5. The run stops before any later executor factory is called.

### Same-Tick Cancellation Race

The first complete run exposed a race where the executor settled with `OperationCanceledException` during the same tick that caller cancellation was requested.

The final monitor handles both observations:

- Caller cancellation latched before executor settlement.
- Executor `OperationCanceledException` observed while the caller token is already requested.

Both produce the same structured cancellation boundary after settlement.

### Run Result

`StartupSequenceRunResult` now exposes:

    WasCancelled

The value is derived from captured terminal execution results and remains immutable.

### Independence and Data Safety

FL-M3-04 adds:

- No root integration
- No lifecycle callback
- No scene or prefab
- No Editor dependency
- No peer-package dependency
- No public API change
- No serialized field or schema change

Authored configuration and ScriptableObject definition data remain unchanged.

## Test Evidence

New multi-frame async tests:

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

Verified multi-frame execution, progress, positive timing, authored order, structured cancellation, settlement before return, later-factory suppression, same-tick cancellation containment, stable `ELAUNCH-STEP-005`, `WasCancelled`, and authored asset immutability.

## Initial Failure and Correction

The first complete run reported:

- Passed: `264`
- Failed: `1`
- Ignored: `0`

`CallerCancellationReturnsStructuredOutcome` exposed the same-tick cancellation race.

The bounded correction was limited to `StartupStepTimeoutMonitor.cs`. The complete 265-test suite passed after the correction.

## Expected Diagnostics

Retained tests intentionally produced:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These yellow warnings are expected runtime diagnostic evidence.

They are not compiler warnings and did not count as test failures.

## Explicit Exclusions

Not implemented:

- Automatic retry
- Retry count or backoff
- Interactive retry
- Retry or skip UI
- Root-level cancellation command
- Shutdown or destruction cancellation orchestration
- `EchoLaunchRoot` runner integration
- Automatic startup
- Launch-session lifecycle advancement
- Public step events
- Launch reports
- Warning aggregation outside the run result
- Configuration or sequence preflight
- Duplicate-ID collision validation
- Dependency validation
- Runner re-entry protection
- Splash presentation
- Scene loading
- Persistent-root lifetime
- Direct-scene initialization
- Custom inspectors and setup windows
- Standalone Laboratory
- Peer-package bridges
- Player builds
- Performance claims

## Closure Result

Production-shaped multi-frame startup execution and structured caller cancellation compile with zero errors and zero compiler warnings.

All two hundred sixty-five Runtime Play Mode tests pass.

Implementation commit `b51d722` is present on `main` and `origin/main`.

FL-M3-04 is ready for its adjacent documentation commit.

The tentative next checkpoint is FL-M3-05 — Runner Re-entry Protection and Sequence Preflight Boundary. It is not authorized by this closeout.
