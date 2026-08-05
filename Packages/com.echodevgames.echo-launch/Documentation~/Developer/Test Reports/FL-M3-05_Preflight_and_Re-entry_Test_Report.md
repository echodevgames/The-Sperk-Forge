# FL-M3-05 - Preflight and Re-entry Runtime Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Checkpoint: `FL-M3-05`
- Unity baseline: `6000.3.8f1`
- Implementation commit: `b70a100`
- Test layer: Runtime Play Mode
- Result: Pass

## Final Totals

- Passed: `288`
- Failed: `0`
- Ignored: `0`
- Compilation errors: `0`
- Compiler warnings: `0`

## New Fixture

`StartupSequenceRunnerPreflightAndReentryTests`

- Passed: `23`
- Failed: `0`
- Ignored: `0`

## Preflight Coverage

The fixture verified:

1. Unknown launch mode is rejected before a factory.
2. Null configuration is rejected.
3. Invalid configuration identity is rejected before a factory.
4. Unsupported configuration schema is rejected before a factory.
5. Missing startup sequence is rejected before a factory.
6. Invalid sequence identity is rejected before a factory.
7. Unsupported sequence schema is rejected before a factory.
8. Null entry is rejected before every factory.
9. Invalid entry identity is rejected before every factory.
10. Undefined activation is rejected before every factory.
11. Duplicate entry identity is rejected before every factory.
12. Enabled missing definition is rejected before every factory.
13. Invalid step identity is rejected before a factory.
14. Unsupported step schema is rejected before a factory.
15. Duplicate step identity is rejected before every factory.
16. Invalid policy becomes a pre-start blocking result without a factory.
17. Disabled entry without a definition remains valid.
18. Empty sequence remains valid.
19. Successful preflight does not mutate authored assets.

## Re-entry Coverage

The fixture verified:

1. A concurrent second run is rejected with `ELAUNCH-RUN-001`.
2. Re-entry rejection occurs before a second executor factory.
3. The first active run remains authoritative.
4. The same runner can be reused after the first run settles.
5. The gate releases after preflight rejection.
6. The gate releases after structured caller cancellation.
7. The gate releases after blocking traversal.

## Diagnostic Coverage

Preflight failures used:

- `ELAUNCH-CFG-001`
- `ELAUNCH-SEQ-001`
- `ELAUNCH-STEP-001`
- `ELAUNCH-STEP-002`

Concurrent re-entry used:

- `ELAUNCH-RUN-001`

Retained expected warnings:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

The retained warnings are intentional test evidence and are not compiler warnings.

## Regression Coverage

The complete suite retained all earlier proof for:

- Launch authority
- State vocabulary
- Session progress
- Lifecycle transitions
- Listener isolation
- Configuration binding
- Sequence definitions
- Step policy and executor contract
- Runtime execution state
- Immediate traversal
- Failure policy and exception conversion
- Timeout and cooperative cancellation
- Multi-frame Unity async execution
- Structured caller cancellation

## Data and Independence Result

Pass:

- No ScriptableObject mutation
- No scene or prefab dependency
- No Editor runtime reference
- No peer-package dependency
- No public API change
- No serialized schema change
- No root or lifecycle integration

## Final Decision

FL-M3-05 automated evidence passes.

The implementation may be documented and closed in an adjacent commit.
