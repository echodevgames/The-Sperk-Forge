# FL-M3-03 - Monotonic Timeout Clock and Cooperative Cancellation

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M3-03`
- Milestone: M3 - Startup Sequence
- Implementation status: Complete and pushed
- Implementation commit: `92c97ae`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Introduce an injectable monotonic unscaled clock, deterministic per-step timeout monitoring, stable timeout results, and cooperative timeout cancellation without abandoning active executor work.

## Authorized Files

New runtime files:

    Runtime/Execution/ILaunchClock.cs
    Runtime/Execution/UnityLaunchClock.cs
    Runtime/Execution/StartupStepTiming.cs
    Runtime/Execution/StartupStepProgressGate.cs
    Runtime/Execution/StartupStepAwaitOutcome.cs
    Runtime/Execution/StartupStepTimeoutMonitor.cs
    Required Unity .meta files

Modified runtime files:

    Runtime/Execution/StartupStepExecution.cs
    Runtime/Execution/StartupSequenceRunner.cs

New tests:

    Tests/Runtime/PlayMode/LaunchClockTimingAndGateTests.cs
    Tests/Runtime/PlayMode/StartupSequenceRunnerTimeoutTests.cs
    Required Unity .meta files

Modified retained test:

    Tests/Runtime/PlayMode/StartupSequenceRunnerImmediateTests.cs

Plan:

    Plan Documentation/Checkpoint Build Plans/FL-M3-03_Monotonic_Timeout_Clock_and_Cooperative_Cancellation_Checkpoint_Build_Plan.md

## Implemented Contract

### Clock Seam

`ILaunchClock` exposes:

    double NowSeconds

    Awaitable NextTickAsync(
        CancellationToken cancellationToken)

Clock values must remain finite, nonnegative, monotonic, unscaled, and measured in seconds.

`UnityLaunchClock.Shared` uses:

    Time.realtimeSinceStartupAsDouble
    Awaitable.NextFrameAsync(cancellationToken)

### Runtime Timing

`StartupStepTiming` captures start time, settlement time, derived elapsed time, configured timeout, timeout state, and cancellation-request state.

Timing remains runtime-only and immutable.

### Progress Containment

`StartupStepProgressGate` forwards progress while open and ignores late progress after idempotent closure.

### Deterministic Timeout Race

The monitor observes executor completion before evaluating the deadline.

Therefore completion already observable at the deadline wins. Otherwise the first observed deadline crossing wins, and a late result cannot replace timeout.

### Stable Timeout Result

Stable diagnostic:

    ELAUNCH-STEP-003

Message:

    The startup step exceeded its configured timeout.

Details include configured timeout seconds, measured elapsed seconds, and whether timeout cancellation was requested.

### Cooperative Cancellation

Every enabled attempt receives a token linked from caller cancellation and attempt-local timeout cancellation.

At timeout, supporting steps receive one cancellation request. Unsupported steps receive no timeout cancellation request. The runner waits for executor settlement in both cases.

### Settlement Safety

The runner does not continue to another entry while the timed-out executor remains active.

It also waits for executor settlement before allowing caller cancellation or a clock-contract failure to escape.

### Policy Integration

`ContinueWithWarning` converts timeout to warning and continues after settlement.

`BlockLaunch` converts timeout to blocking failure and leaves later entries unvisited.

### Clock Contract Failure

Backward, non-finite, or negative clock values become a blocking `ELAUNCH-STEP-004` timing-system contract result after active work settles.

## Test Evidence

FL-M3-03 tests:

- Clock, timing, and progress-gate tests: `14`
- Timeout runner and cancellation tests: `18`
- FL-M3-03 subtotal: `32`

Full Runtime Play Mode suite:

- Passed: `263`
- Failed: `0`
- Ignored: `0`

Compilation:

- Errors: `0`
- Warnings: `0`

Verified zero timeout, deadline ordering, stable timeout diagnostics, supported and unsupported cancellation, late result and progress containment, caller cancellation boundaries, policy application, backward-clock blocking, authored asset immutability, and executor settlement before later factory creation.

## Bounded Test Fixture Corrections

1. `AwaitableCompletionSource<T>.SetResult` was changed from the older `ref` call shape to the Unity `6000.3.8f1` by-value signature.
2. The retained immediate fixture was restored from the correct FL-M3-02 baseline after a stale artifact temporarily reintroduced three obsolete expectations.

The final retained immediate suite preserves policy-aware stops, null-executor contract conversion, and the distinct linked-token expectation.

## Expected Diagnostics

Retained tests intentionally produced `ELAUNCH-ROOT-001` and `ELAUNCH-EVENT-001`.

These yellow warnings remain expected test evidence.

`ELAUNCH-STEP-003` and timing-system `ELAUNCH-STEP-004` values are structured runtime result data rather than normal Console warnings.

## Explicit Exclusions

Not implemented:

- Automatic retry
- Retry count or backoff
- Interactive retry
- Retry or skip UI
- Structured caller-cancellation run result
- Root-level cancellation command
- Shutdown or destruction cancellation orchestration
- Root integration
- Automatic startup
- Lifecycle advancement
- Public step events
- Reports
- Preflight
- Duplicate-ID scans
- Dependency validation
- Re-entry protection
- Production-shaped multi-frame proof
- Presentation
- Scene loading
- Persistent lifetime
- Direct-scene behavior
- Custom inspectors
- Setup windows
- Test Lab scenes
- Peer-package bridges

## Closure Result

Monotonic timeout execution compiles with zero errors and zero warnings.

All two hundred sixty-three Runtime Play Mode tests pass.

Implementation commit `92c97ae` is present on `main` and `origin/main`.

FL-M3-03 is ready for its adjacent documentation commit.

The next runtime checkpoint requires separate approval.
