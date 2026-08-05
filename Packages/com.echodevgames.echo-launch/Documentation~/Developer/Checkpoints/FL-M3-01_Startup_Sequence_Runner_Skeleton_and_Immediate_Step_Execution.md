# FL-M3-01 - Startup Sequence Runner Skeleton and Immediate Step Execution

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M3-01`
- Milestone: M3 - Startup Sequence
- Implementation status: Complete and pushed
- Implementation commit: `0864b9c`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Create the first internal startup-sequence runner and prove deterministic execution of immediate startup-step executors without integrating the runner with roots, scenes, launch lifecycle, policy application, timeout, retries, reports, or presentation.

## Authorized Files

Runtime:

    Runtime/Execution.meta
    Runtime/Execution/StartupStepExecution.cs
    Runtime/Execution/StartupSequenceRunResult.cs
    Runtime/Execution/StartupSequenceRunner.cs
    Required Unity .meta files

Tests:

    Tests/Runtime/PlayMode/StartupStepExecutionTests.cs
    Tests/Runtime/PlayMode/StartupSequenceRunnerImmediateTests.cs
    Required Unity .meta files

Plan:

    Plan Documentation/Checkpoint Build Plans/FL-M3-01_Startup_Sequence_Runner_Skeleton_and_Immediate_Step_Execution_Checkpoint_Build_Plan.md

No existing runtime source file was modified.

## Implemented Contract

### Runtime Attempt State

`StartupStepExecution` stores runtime-only:

- Entry identity
- Step identity
- Display label
- Authored index and count
- Authored policy snapshot
- Fresh executor
- Current status
- Latest progress
- Terminal result

Legal path:

    NotStarted -> Running -> terminal result status

Progress is accepted only while running.

A terminal result is captured once.

Definitions, entries, policies, sequences, and configurations remain immutable.

### Completed Run Summary

`StartupSequenceRunResult` stores:

- Authored entry count
- Disabled entry count
- Attempted execution count
- Ordered completed execution references
- Warning presence
- Failure presence
- Blocking-failure presence

The summary exposes count plus indexed reads and no mutable collection.

It records classifications without interpreting policy or declaring final launch success.

### Immediate Runner

`StartupSequenceRunner`:

- Requires a valid active launch mode.
- Requires a configuration and startup sequence.
- Iterates authored indices.
- Skips disabled entries before factory creation.
- Creates one fresh executor per enabled entry.
- Creates immutable context.
- Passes cancellation through.
- Captures immediate progress.
- Awaits the terminal result.
- Preserves attempted order.
- Returns an immutable run summary.

### Deliberate Policy Boundary

A blocking result is captured, but traversal continues.

This is intentional evidence that FL-M3-01 does not yet interpret `StartupStepPolicy`.

### Compile Correction

The first runner draft referenced `LaunchMode.None`.

The package enum uses `LaunchMode.Unknown`.

Phase C was corrected to reject `Unknown` and undefined values. The correction changed only `StartupSequenceRunner.cs`.

## Test Evidence

FL-M3-01 tests:

- Startup-step execution tests: `12`
- Immediate runner tests: `18`
- FL-M3-01 subtotal: `30`

Full Runtime Play Mode suite:

- Passed: `199`
- Failed: `0`
- Ignored: `0`

Verified:

- Construction metadata copying
- Initial attempt state
- Single begin transition
- Progress guards
- Single terminal result
- Invalid construction rejection
- Null configuration rejection
- Missing sequence rejection
- Empty sequence traversal
- Disabled entry skipping
- Enabled entry execution
- Fresh executors across runs
- Stable context identities
- Authored index and full count
- Cancellation-token pass-through
- Immediate progress capture
- Success result preservation
- Warning result preservation
- Recoverable failure preservation
- Blocking failure preservation
- Authored order
- Continued traversal after blocking result
- Null executor rejection
- Definition immutability

## Expected Diagnostics

Retained tests intentionally produced:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These yellow warnings remain expected evidence and are not failures.

## Manual Evidence

No production asset, scene, prefab, root, or automatic startup setup was required.

Unity compiled each phase with zero errors after the bounded `LaunchMode.Unknown` correction.

No runtime object appeared automatically and no sequence executed outside explicit tests.

## Explicit Exclusions

Not implemented:

- Root integration
- Automatic scene-callback startup
- Lifecycle advancement
- Public step events
- Exception conversion
- Result-to-policy application
- Blocking traversal stop
- Warning aggregation
- Timeout handling
- Retry behavior
- Preflight
- Reports
- Splash presentation
- Scene loading
- Persistent lifetime
- Direct-scene initialization
- Custom inspectors
- Setup windows
- Test Lab scenes
- Peer-package bridges

## Closure Result

Immediate startup execution compiles and all one hundred ninety-nine Runtime Play Mode tests pass.

Implementation commit `0864b9c` is present on `main` and `origin/main`.

FL-M3-01 is ready for its adjacent documentation commit.

The next runtime checkpoint requires separate approval.
