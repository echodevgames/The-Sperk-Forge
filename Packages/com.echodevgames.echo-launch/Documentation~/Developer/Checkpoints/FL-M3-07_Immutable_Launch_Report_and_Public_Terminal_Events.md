# FL-M3-07 - Immutable Launch Report and Public Terminal Events

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
- Checkpoint: `FL-M3-07`
- Milestone: M3 - Startup Sequence
- Implementation status: Complete and pushed
- Implementation commit: `a6f6544`
- Previous documentation commit: `d728602`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Finalize truthful immutable diagnostic reports for failed and interrupted root-owned launches, then publish matching public terminal events only after lifecycle state and report storage are authoritative.

## Implemented Contract

### Immutable Step Reports

`LaunchStepReport` copies terminal step data:

- Entry and step identity
- Step display name
- Authored index and count
- Authored policy
- Final status, result, and progress
- Start, settlement, elapsed, and timeout timing
- Timeout and timeout-cancellation flags

No internal execution or executor reference escapes.

### Immutable Launch Reports

`LaunchReport` records:

- Report schema version `1`
- Producing package version `0.1.0`
- Launch mode
- Configuration and sequence identity
- Final failed or interrupted lifecycle status
- Launch start, finalization, and elapsed timing
- Authored, attempted, disabled, and unvisited counts
- Warning, failure, and blocking-failure counts
- Cancellation state
- Final diagnostic result
- Ordered immutable step reports

Public report state has no public setters or mutable collection exposure.

### Single-Use Builder

`LaunchReportBuilder`:

- Is internal and root-owned
- Captures completed steps exactly once
- Preserves authored order
- Reconciles run accounting
- Rejects second finalization
- Finalizes only `Failed` or `Interrupted`
- Retains transition-pending success without producing a report

### Root Report Surface

`EchoLaunchRoot` now exposes:

```csharp
public LaunchReport LastReport { get; }

public event Action<LaunchReport> LaunchFailed;

public event Action<LaunchReport> LaunchInterrupted;
```

Duplicate roots expose no report.

### Terminal Event Ordering

For failure and interruption:

1. Terminal lifecycle snapshot is accepted.
2. Root state is already terminal.
3. Immutable report is finalized.
4. `LastReport` stores that exact report.
5. Matching terminal event is dispatched.

Listener failures remain isolated through `ELAUNCH-EVENT-001`.

### Truthful Success Boundary

Successful or warning-only startup remains at `Transitioning`.

At that boundary:

- `LastReport` is `null`.
- No failed or interrupted event is raised.
- `LaunchCompleted` does not exist.
- `Completed` is not published.
- Successful builder data remains available for later destination handoff.

### Destruction Safety

Destroying an active root suppresses unsafe late terminal-event publication.

Finalized reports remain readable after active runtime and authored asset references are released.

## Files

Modified:

- `Runtime/Core/EchoLaunchRoot.cs`

Created:

- `Runtime/Reports.meta`
- `Runtime/Reports/LaunchStepReport.cs`
- `Runtime/Reports/LaunchStepReport.cs.meta`
- `Runtime/Reports/LaunchReport.cs`
- `Runtime/Reports/LaunchReport.cs.meta`
- `Runtime/Reports/LaunchReportBuilder.cs`
- `Runtime/Reports/LaunchReportBuilder.cs.meta`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs`
- `Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs.meta`
- `Plan Documentation/Checkpoint Build Plans/FL-M3-07_Immutable_Launch_Report_and_Public_Terminal_Events_Checkpoint_Build_Plan.md`

## Compile Correction

The first FL-M3-07 compile reported two errors because the new fixture referenced a nonexistent `EchoLaunchRuntimeReset.ResetStatics()` helper.

The bounded correction replaced both calls with the established:

```csharp
LaunchAuthorityClaim.Reset();
```

No runtime or report implementation change was required.

Final compilation:

- Errors: `0`
- Compiler warnings: `0`

## Test Evidence

New fixture:

- Passed: `25`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `336`
- Failed: `0`
- Ignored: `0`

## Expected Diagnostics

Tests intentionally produce listener and duplicate-root warnings including:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These are runtime diagnostic proof, not compiler warnings or test failures.

## Exclusions Preserved

Not implemented:

- `LaunchCompleted`
- Successful report finalization
- Destination definition or validation
- Initial destination loading or activation
- `Transitioning -> Completed`
- Public step lifecycle events
- Automatic Unity-callback startup
- Splash or status presentation
- Direct-scene initialization
- Persistent-root policy
- Report export
- Editor setup or repair
- Standalone Laboratory
- Player builds
- Performance claims

## Closure Result

FL-M3-07 implementation is complete in commit `a6f6544`.

The implementation compiles with 0 errors and 0 compiler warnings.

All 336 Runtime Play Mode tests pass with 0 failed and 0 ignored.

The checkpoint is ready for its adjacent documentation closeout.

Tentative next checkpoint: FL-M3-08 - Initial Destination Contract, Load Result, and Completed Handoff.
