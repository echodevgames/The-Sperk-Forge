# FL-M3-06 - Root-Owned Startup Lifecycle Runtime Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Checkpoint: `FL-M3-06`
- Unity baseline: `6000.3.8f1`
- Implementation commit: `e0e9645`
- Test layer: Runtime Play Mode
- Final result: Pass

## Final Totals

- Passed: `311`
- Failed: `0`
- Ignored: `0`
- Compilation errors: `0`
- Compiler warnings: `0`

## New Fixture

`EchoLaunchRootStartupLifecycleTests`

- Passed: `23`
- Failed: `0`
- Ignored: `0`

## Root Ownership Coverage

Verified:

1. `Awake` claims authority without starting launch automatically.
2. Empty sequence reaches `Transitioning`.
3. Successful work publishes the approved state order.
4. Step start, progress, and completion reach root snapshots.
5. Warning-only traversal reaches `Transitioning`.
6. Blocking traversal reaches `Failed`.
7. Preflight rejection reaches `Failed` before factory creation.
8. Missing configuration reaches `Failed`.
9. Cancellation before launch returns false.
10. Active cancellation waits for executor settlement.
11. Blank cancellation reason uses the stable default.
12. Repeated cancellation request is rejected.
13. Concurrent root start is rejected before a second factory.
14. Settled root session cannot restart.
15. Failed root session cannot restart.
16. Duplicate root cannot start or cancel.
17. Destroying the active root requests cancellation and suppresses late publication.
18. Success does not publish `Completed` before destination handoff.
19. Direct-scene launch mode remains stable.
20. Root-owned execution does not mutate authored assets.
21. Runner replacement is rejected after lifecycle advancement.
22. Preflight failure clears the active root gate.
23. Cancellation publishes `Interrupted` exactly once.

## Lifecycle and Diagnostic Coverage

Verified lifecycle states:

- `AuthorityClaimed`
- `Validating`
- `Running`
- `Transitioning`
- `Failed`
- `Interrupted`

Verified stable diagnostics:

- `ELAUNCH-LIFE-001`
- `ELAUNCH-LIFE-002`
- Existing preflight diagnostic codes
- Retained `ELAUNCH-ROOT-001`
- Retained `ELAUNCH-EVENT-001`

## Compatibility Investigation

Initial full-suite result:

- Passed: `296`
- Failed: `15`
- Ignored: `0`

Cause:

- The structured preflight exception was a subclass of `InvalidOperationException`.
- Fifteen retained tests used NUnit exact-type `Assert.Throws<InvalidOperationException>` assertions.

Correction:

- The legacy three-argument runner overload now catches the structured exception and throws exact `InvalidOperationException`.
- The root-owned observer overload still receives the structured exception.

Final rerun:

- Passed: `311`
- Failed: `0`
- Ignored: `0`

## Regression Coverage

The final suite retained all prior proof for:

- Authority and duplicate rejection
- Launch vocabulary and immutable snapshots
- Session state and transition legality
- Notification ordering and listener isolation
- Configuration and sequence definitions
- Step policy and executor contracts
- Immediate and multi-frame execution
- Failure policy and exception containment
- Monotonic timeout and cooperative cancellation
- Structured caller cancellation
- Complete preflight
- Runner re-entry protection

## Data and Independence Result

Pass:

- No authored ScriptableObject mutation
- No scene or prefab dependency
- No automatic startup callback
- No Editor runtime reference
- No peer-package dependency
- No serialized schema change
- No package version change

## Final Decision

FL-M3-06 automated evidence passes.

The implementation may be documented and closed in an adjacent commit.
