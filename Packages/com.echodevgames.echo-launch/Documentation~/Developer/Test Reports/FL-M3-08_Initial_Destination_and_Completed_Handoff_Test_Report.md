# FL-M3-08 - Initial Destination and Completed Handoff Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Checkpoint: `FL-M3-08`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Unity baseline: `6000.3.8f1`
- Authority commit: `eb9cc49`
- Implementation commit: `114ac91`
- Test layer: Runtime Play Mode
- Final result: Pass

## Final Totals

- Passed: `380`
- Failed: `0`
- Ignored: `0`
- Compilation errors: `0`
- Compiler warnings: `0`

## Fixture Breakdown

- Authority tests: `7`
- Root-owned startup lifecycle tests: `23`
- Clock, timing, and progress-gate tests: `14`
- Configuration and destination binding tests: `22`
- Destination and completed-handoff tests: `37`
- Launch-state vocabulary tests: `39`
- Launch session and progress tests: `14`
- Lifecycle transition tests: `22`
- Lifecycle notification tests: `20`
- Launch report and terminal-event tests: `25`
- Startup sequence definition tests: `24`
- Startup step policy and executor-contract tests: `28`
- Startup step execution tests: `12`
- Immediate startup sequence runner tests: `18`
- Policy-application tests: `16`
- Runner policy and exception tests: `16`
- Timeout runner and cancellation tests: `18`
- Multi-frame async runner tests: `2`
- Preflight and re-entry tests: `23`

## New Destination Fixture

`LaunchDestinationAndCompletedHandoffTests`

- Passed: `37`
- Failed: `0`
- Ignored: `0`

Verified:

1. New destination schema is 1.
2. Destination IDs use canonical format.
3. Valid destination metadata is accepted.
4. Malformed destination identity is rejected without repair.
5. Unsupported destination schema is rejected without rewrite.
6. Blank destination display name is invalid.
7. Blank destination scene path is invalid.
8. Configuration schema is 3.
9. Historical schema 2 fails before factory and loader.
10. Missing destination fails before factory and loader.
11. Invalid destination fails before factory and loader.
12. Loader-validator rejection fails before factory and load.
13. Missing loader fails before factory.
14. Successful load result is immutable and normalized.
15. Failed load result requires a diagnostic code.
16. Cancelled load result requires a message.
17. Undefined load status is rejected.
18. Progress relay accepts normalized values and ignores late reports.
19. Progress relay rejects nonfinite values.
20. Successful handoff completes and invokes the loader once.
21. Successful state order includes `Transitioning` then `Completed`.
22. Destination progress is published while transitioning.
23. Completed report contains destination and sequence accounting.
24. `LastReport` is the exact completed-event payload.
25. Completed event fires exactly once without failure events.
26. Completion-listener failure does not block later listeners.
27. Destination-load failure produces a failed report and no completion.
28. Null load result produces destination failure.
29. Mismatched success destination produces failure.
30. Cancellation before destination loading prevents invocation.
31. Cancellation during loading waits for settlement and interrupts.
32. Destroyed root publishes no late completion event.
33. Default loader rejects scenes outside Build Settings.
34. Default loader honors cancellation before start.
35. Completed report requires destination metadata.
36. Builder finalizes a completed report only once.
37. Completed launch does not mutate authored assets.

## Expanded Configuration Fixture

`LaunchConfigurationBindingTests`

- Passed: `22`
- Failed: `0`
- Ignored: `0`

New coverage includes:

- Current configuration schema 3
- Historical schema 2 unsupported without rewrite
- Destination schema 1 and canonical identity
- Configuration destination binding
- Authority destination exposure
- Duplicate-root destination hiding
- Destination immutability through lifecycle work

## Retained Lifecycle Correction

The intermediate full run reported:

- Passed: `379`
- Failed: `1`
- Ignored: `0`

Failure:

```text
WarningRunAdvancesToCompleted
Expected: Warning
But was: Succeeded
```

FL-M3-08 correctly uses successful destination activation as the final lifecycle result. The earlier startup warning remains preserved in:

- `LaunchReport.WarningCount`
- `LaunchReport.GetStepReport(0).Status`

The retained test was corrected to verify both parts of the contract.

## Compile Corrections

The first FL-M3-08 compile reported three test-only missing-property errors:

```text
LaunchProgressSnapshot.IsIndeterminate
```

The established property is:

```text
LaunchProgressSnapshot.IsProgressIndeterminate
```

After correcting the retained warning test, one additional test-only compile error exposed:

```text
LaunchStepReport.FinalStatus
```

The established property is:

```text
LaunchStepReport.Status
```

No production runtime code changed for these corrections.

Final compilation:

- Errors: `0`
- Compiler warnings: `0`

## Event Ordering Result

For successful destination handoff, tests confirm:

1. Destination activation succeeds.
2. Completed lifecycle state is accepted.
3. Immutable completed report is finalized.
4. `LastReport` stores that exact report.
5. `LaunchCompleted` receives that exact report instance.
6. The event is published exactly once.

## Failure and Cancellation Result

Pass:

- Destination preflight blocks before startup-step side effects.
- Destination-load failure uses `ELAUNCH-DEST-002`.
- Null and mismatched success results cannot create false completion.
- Cancellation before load start prevents invocation.
- Cancellation during an injected load waits for settlement.
- Cancelled transition reaches `Interrupted`.
- Destroyed roots cannot publish late completion.

## Immutability Result

Pass:

- Configuration schema remains unchanged at runtime.
- Destination schema and authored metadata remain unchanged.
- Completed reports are immutable copies.
- Startup warning information survives destination success.
- No runtime migration or repair occurs.

## Default Loader Evidence Boundary

Automated evidence covers:

- Build-settings rejection before loading.
- Cancellation before loading starts.
- Progress and loader contract behavior through controlled seams.

Not run:

- Real Boot-to-destination scene activation in a Standalone Laboratory.
- Player build scene activation.

## Expected Runtime Diagnostics

Expected yellow diagnostics include:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These are intentional proof, not compiler warnings or test failures.

## Final Decision

FL-M3-08 automated evidence passes.

The implementation may be documented and closed in an adjacent commit.
