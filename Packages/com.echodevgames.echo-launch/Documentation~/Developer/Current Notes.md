# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M3-04`
- Title: Multi-Frame Async Proof and Runner Cancellation Outcome
- Package version: `0.1.0`
- Implementation status: Complete and pushed
- Implementation commit: `b51d722`
- Documentation closeout: Pending adjacent commit
- Runtime Play Mode result: 265 passed, 0 failed, 0 ignored
- Compilation result: 0 errors, 0 compiler warnings

## Completed Result

Implemented:

- Immutable caller-cancellation observation on `StartupStepAwaitOutcome`
- Structured caller-cancellation conversion inside `StartupSequenceRunner`
- Stable caller-cancellation diagnostic `ELAUNCH-STEP-005`
- Terminal `StartupStepStatus.Cancelled` execution outcome
- `StartupSequenceRunResult.WasCancelled`
- Cancellation-driven traversal stop before later factory creation
- Executor settlement before runner return
- Same-tick caller-cancellation race containment
- Production-shaped multi-frame executor proof
- Multi-frame progress and positive timing proof
- Authored-order proof across real Unity frames
- Two new Runtime Play Mode tests
- One retained caller-cancellation test updated to the structured outcome

## Evidence Summary

### Passed

- Real `Awaitable.NextFrameAsync` execution across multiple Unity frames
- Progress publication while the attempt remained active
- Positive monotonic elapsed timing
- Authored-order execution after multi-frame settlement
- Linked caller cancellation reaching the active executor
- Executor settlement before cancellation result return
- Structured `Cancelled` result
- Stable `ELAUNCH-STEP-005`
- `WasCancelled == true`
- Later entry remaining unvisited
- Later executor factory not being called
- Authored warning policy unable to downgrade caller cancellation
- Same-tick cancellation exception containment
- Authored asset immutability
- Full 265-test suite
- Clean compilation with zero compiler warnings

### Initial Failure and Bounded Fix

The first complete FL-M3-04 run reported:

- Passed: 264
- Failed: 1
- Ignored: 0

The failing retained test was:

    CallerCancellationReturnsStructuredOutcome

The executor settled with `OperationCanceledException` in the same tick that caller cancellation was requested. The monitor consumed the executor before its next loop could latch the caller token.

The bounded fix treats a settled `OperationCanceledException` as caller cancellation when the caller token is already requested. The complete suite then passed.

### Expected Diagnostics

Retained tests intentionally generated:

    ELAUNCH-ROOT-001
    ELAUNCH-EVENT-001

These yellow warnings were expected runtime diagnostic evidence, not compiler warnings or test failures.

### Not Run

- Automatic retry
- Retry backoff
- Interactive retry
- Retry or skip UI
- Root-level cancellation command
- Shutdown or destruction cancellation orchestration
- Root integration
- Automatic startup
- Lifecycle advancement
- Public step events
- Reports
- Warning aggregation outside the run result
- Configuration or sequence preflight
- Duplicate-ID collision validation
- Dependency validation
- Runner re-entry protection
- Splash presentation
- Scene loading
- Player builds
- Performance measurements

## Changed Files

Modified runtime implementation:

- `Runtime/Execution/StartupSequenceRunResult.cs`
- `Runtime/Execution/StartupSequenceRunner.cs`
- `Runtime/Execution/StartupStepAwaitOutcome.cs`
- `Runtime/Execution/StartupStepTimeoutMonitor.cs`

Automated tests:

- Modified `Tests/Runtime/PlayMode/StartupSequenceRunnerTimeoutTests.cs`
- Added `Tests/Runtime/PlayMode/StartupSequenceRunnerMultiFrameAsyncTests.cs`
- Added Unity-generated `.meta` file

Checkpoint plan:

- `Plan Documentation/Checkpoint Build Plans/FL-M3-04_Multi-Frame_Async_Proof_and_Runner_Cancellation_Outcome_Checkpoint_Build_Plan.md`

Adjacent documentation:

- Package checkpoint
- Package test report
- Root completion record
- Changelog
- Architecture
- Documentation index
- README
- Root and package Current Notes

## Handoff Snapshot

FL-M3-04 implementation is complete and pushed in commit `b51d722`.

The adjacent documentation closeout is ready for final Git review, commit, and push.

Real multi-frame Unity async execution and structured caller cancellation are proven without root, lifecycle, report, presentation, or scene integration.

Retries, preflight, runner re-entry protection, root cancellation commands, root integration, reports, presentation, and scene loading remain unauthorized until a later checkpoint is approved.
