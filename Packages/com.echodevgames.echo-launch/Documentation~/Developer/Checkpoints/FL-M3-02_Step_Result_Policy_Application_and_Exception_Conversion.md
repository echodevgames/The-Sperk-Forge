# FL-M3-02 - Step Result Policy Application and Exception Conversion

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M3-02`
- Milestone: M3 - Startup Sequence
- Implementation status: Complete and pushed
- Implementation commit: `6f2ab12`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Apply authored startup-step failure policy, convert bounded factory and executor failures into stable structured results, and stop traversal when the effective result requires it.

## Authorized Files

New runtime files:

    Runtime/Execution/StartupStepPolicyDecision.cs
    Runtime/Execution/StartupStepPolicyEvaluator.cs
    Runtime/Execution/StartupStepExceptionPhase.cs
    Runtime/Execution/StartupStepExceptionConverter.cs
    Required Unity .meta files

Modified runtime files:

    Runtime/Execution/StartupStepExecution.cs
    Runtime/Execution/StartupSequenceRunResult.cs
    Runtime/Execution/StartupSequenceRunner.cs

New tests:

    Tests/Runtime/PlayMode/StartupStepPolicyApplicationTests.cs
    Tests/Runtime/PlayMode/StartupSequenceRunnerPolicyAndExceptionTests.cs
    Required Unity .meta files

Modified retained test:

    Tests/Runtime/PlayMode/StartupSequenceRunnerImmediateTests.cs

Plan:

    Plan Documentation/Checkpoint Build Plans/FL-M3-02_Step_Result_Policy_Application_and_Exception_Conversion_Checkpoint_Build_Plan.md

## Implemented Contract

### Policy Decision

`StartupStepPolicyDecision` preserves the original result, exposes the effective result, and records whether traversal continues.

It exposes conversion by immutable result-instance identity.

### Policy Application

Preserve and continue:

- Success
- Warning
- Skipped

Preserve and stop:

- Cancelled

Failure-like results:

- Recoverable failure
- Blocking failure
- Timed out

`ContinueWithWarning` converts failure-like results to warnings and continues.

`BlockLaunch` converts failure-like results to blocking failures and stops.

Code, message, and details are preserved during policy conversion.

Explicit `FailureAction` is the runtime authority. Required/optional intent does not silently override it.

### Stable Exception Conversion

Stable diagnostic:

    ELAUNCH-STEP-004

Factory exception:

- Blocking failure
- Stops traversal
- No executor invocation

Null executor:

- Blocking contract failure
- Stops traversal

Executor exception:

- Recoverable source result
- Then authored policy applies

Null executor result:

- Blocking contract failure
- Stops traversal

Exception details contain only sanitized type and trimmed message.

`OperationCanceledException` remains outside generic conversion.

### Runtime Execution State

`StartupStepExecution` now supports:

- Metadata creation before factory success
- One executor attachment before begin
- Begin rejection without an executor
- One blocking pre-start completion
- Existing normal running completion

No authored asset is mutated.

### Traversal Accounting

`StartupSequenceRunResult` now exposes:

- Unvisited entry count
- Early-stop flag
- Stopping authored entry index

Invariant:

    attempted + disabled + unvisited = authored

No later authored entry is inspected after a stop.

### Runner Behavior

The runner now:

- Contains factory failures
- Contains executor failures
- Applies authored failure policy
- Captures effective results
- Stops before later factory creation when required
- Preserves complete authored-index accounting

## Test Evidence

FL-M3-02 tests:

- Policy application tests: `16`
- Runner policy and exception tests: `16`
- FL-M3-02 subtotal: `32`

Full Runtime Play Mode suite:

- Passed: `231`
- Failed: `0`
- Ignored: `0`

Compilation:

- Errors: `0`
- Warnings: `0`

Verified:

- Policy decision null guards
- Preserved result identity
- Converted result identity
- Success, warning, and skipped continuation
- Recoverable and blocking conversion
- Timed-out policy application
- Cancelled preservation
- Diagnostic text preservation
- Explicit failure-action authority
- Factory exception containment
- Null executor containment
- Later-factory suppression
- Executor exception continuation and stop
- Null result containment
- Stack-trace exclusion
- Cancellation exception escape
- Returned failure policy application
- Early-stop accounting
- Stopping authored index
- Complete traversal metadata
- Authored asset immutability

## Expected Diagnostics

Retained tests intentionally produced:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These yellow warnings remain expected test evidence.

No `ELAUNCH-STEP-004` Console warning is emitted by the runner. It is captured as structured runtime result data.

## Explicit Exclusions

Not implemented:

- Timeout clock
- Timeout race
- Timeout cancellation
- Retry loops
- Retry backoff
- Interactive retry
- Cancellation orchestration
- Root integration
- Automatic startup
- Lifecycle advancement
- Public step events
- Reports
- Preflight
- Duplicate-ID scans
- Re-entry protection
- Multi-frame asynchronous proof
- Presentation
- Scene loading
- Persistent lifetime
- Direct-scene behavior
- Custom inspectors
- Setup windows
- Test Lab scenes
- Peer-package bridges

## Closure Result

Policy-aware immediate startup execution compiles with zero errors and zero warnings.

All two hundred thirty-one Runtime Play Mode tests pass.

Implementation commit `6f2ab12` is present on `main` and `origin/main`.

FL-M3-02 is ready for its adjacent documentation commit.

The next runtime checkpoint requires separate approval.
