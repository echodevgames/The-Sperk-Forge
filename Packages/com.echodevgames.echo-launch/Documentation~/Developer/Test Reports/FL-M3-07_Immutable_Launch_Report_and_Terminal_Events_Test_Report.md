# FL-M3-07 - Immutable Launch Report and Public Terminal Events Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Checkpoint: `FL-M3-07`
- Unity baseline: `6000.3.8f1`
- Implementation commit: `a6f6544`
- Test layer: Runtime Play Mode
- Final result: Pass

## Final Totals

- Passed: `336`
- Failed: `0`
- Ignored: `0`
- Compilation errors: `0`
- Compiler warnings: `0`

## New Fixture

`LaunchReportAndTerminalEventTests`

- Passed: `25`
- Failed: `0`
- Ignored: `0`

## Report Surface Coverage

Verified:

1. `LastReport` is null before launch.
2. Missing configuration finalizes a failed report.
3. Invalid preflight finalizes before executor creation.
4. Blocking step report copies terminal execution values.
5. Warning, disabled, failure, and unvisited accounting is preserved.
6. Failed event observes accepted state and stored report.
7. Failed event fires exactly once and interrupted does not fire.
8. Failed-listener failure does not prevent a later listener.
9. Interrupted report finalizes after executor settlement.
10. Interrupted event observes accepted state and stored report.
11. Interrupted event fires exactly once and failed does not fire.
12. Blank cancellation reason is normalized into the report.
13. Interrupted-listener failure does not prevent a later listener.
14. Transition-pending success has no final report or terminal event.
15. Duplicate root exposes no report and publishes no terminal event.
16. Destroyed root publishes no unsafe late terminal report event.
17. Report schema and producing package version remain stable.
18. Public report properties expose no public setters.
19. Invalid step-report index throws.
20. Builder rejects second finalization.
21. Launch report rejects nonterminal success status.
22. Launch report defensively copies the supplied step list.
23. Finalized report remains readable after root and assets are destroyed.
24. Failed report does not mutate authored assets.
25. Launch report rejects inconsistent accounting and timing.

## Compile Investigation

The first compile produced two `CS0103` errors in the new test fixture.

Cause:

```csharp
EchoLaunchRuntimeReset.ResetStatics();
```

The package does not define that helper.

Correction:

```csharp
LaunchAuthorityClaim.Reset();
```

Final compile result:

- Errors: `0`
- Compiler warnings: `0`

## Event Ordering Result

For failed and interrupted launches, tests confirm:

1. Terminal lifecycle snapshot is accepted.
2. Root state is already terminal.
3. Immutable report is finalized.
4. `LastReport` stores the exact report.
5. Matching event receives that exact report instance.

## Immutability Result

Pass:

- No public report setters
- No mutable collection exposure
- Step data copied from terminal execution
- Step list defensively copied
- Finalized reports remain readable after runtime references release
- Authored ScriptableObjects remain unchanged

## Success Boundary Result

Pass:

- Successful sequence reaches `Transitioning`
- `LastReport` remains null
- No failed event
- No interrupted event
- No `LaunchCompleted`
- No `Completed` publication

## Expected Runtime Diagnostics

Expected yellow diagnostics include:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These are intentional proof of duplicate-root and broken-listener containment.

## Regression Result

All retained coverage remained green for:

- Authority and lifecycle
- Notifications
- Configuration and sequence definitions
- Policy and executor contracts
- Immediate and multi-frame runner behavior
- Timeout and cooperative cancellation
- Preflight and runner re-entry
- Root-owned lifecycle
- Legacy direct-runner exception compatibility

## Final Decision

FL-M3-07 automated evidence passes.

The implementation may be documented and closed in an adjacent commit.
