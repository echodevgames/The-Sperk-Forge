# FL-M3-05 - Runner Re-entry Protection and Sequence Preflight Boundary

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M3-05`
- Milestone: M3 - Startup Sequence
- Implementation status: Complete and pushed
- Implementation commit: `b70a100`
- Previous documentation commit: `ce2e23b`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Create one complete startup-sequence execution gate: validate the authored launch configuration and sequence before executor creation, then prevent one runner instance from owning concurrent traversals.

## Authorized Files

New runtime file:

    Runtime/Execution/StartupSequencePreflight.cs
    Required Unity .meta file

Modified runtime file:

    Runtime/Execution/StartupSequenceRunner.cs

New test fixture:

    Tests/Runtime/PlayMode/StartupSequenceRunnerPreflightAndReentryTests.cs
    Required Unity .meta file

Plan:

    Plan Documentation/Checkpoint Build Plans/FL-M3-05_Runner_Re-entry_Protection_and_Sequence_Preflight_Boundary_Checkpoint_Build_Plan.md

## Implemented Contract

### Side-Effect-Free Preflight

`StartupSequencePreflight.Validate` completes before the runner begins traversal.

It validates:

- Defined active launch mode
- Non-null configuration
- Configuration identity and schema
- Assigned startup sequence
- Sequence identity and schema
- Non-null entries
- Entry identity and activation
- Unique entry IDs
- Enabled step-definition presence
- Referenced step identity and schema
- Unique referenced step IDs

No executor factory is called while preflight is incomplete.

### Stable Diagnostics

Preflight uses:

- `ELAUNCH-CFG-001`
- `ELAUNCH-SEQ-001`
- `ELAUNCH-STEP-001`
- `ELAUNCH-STEP-002`

Concurrent runner re-entry uses:

- `ELAUNCH-RUN-001`

### Compatibility Rules

FL-M3-05 intentionally preserves:

- Empty sequence as a valid empty traversal
- Disabled entry without a definition as valid
- Invalid enabled policy as a structured pre-start blocking result
- Immutable authored ScriptableObject data

### Runner Re-entry Gate

One `StartupSequenceRunner` instance owns one integer active-run state.

Acquisition uses `Interlocked.CompareExchange`.

A second overlapping `RunAsync` call:

1. Is rejected immediately.
2. Contains `ELAUNCH-RUN-001`.
3. Does not begin preflight traversal.
4. Does not create a second executor.

### Gate Release

The entire active run is wrapped in `try/finally`.

The gate releases after:

- Normal success
- Preflight rejection
- Structured caller cancellation
- Blocking traversal
- Unexpected exceptions

After release, the same runner instance may be used for a later sequential run.

### Independence and Data Safety

FL-M3-05 adds:

- No root integration
- No lifecycle callback
- No scene or prefab
- No Editor dependency
- No peer-package dependency
- No public API change
- No serialized field or schema change

The preflight does not repair, clamp, migrate, or mutate authored data.

## Test Evidence

New preflight and re-entry fixture:

- Passed: `23`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `288`
- Failed: `0`
- Ignored: `0`

Compilation:

- Errors: `0`
- Compiler warnings: `0`

Verified complete preflight ordering, identity/schema validation, null-entry handling, activation validation, missing-definition handling, duplicate entry and step IDs, compatibility cases, authored-data immutability, concurrent re-entry rejection, no second factory, gate release, and sequential reuse.

## Expected Diagnostics

Retained tests intentionally produced:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These yellow warnings are expected runtime diagnostic evidence.

They are not compiler warnings and did not count as test failures.

## Explicit Exclusions

Not implemented:

- Dependency-graph validation
- Public preflight result or launch report
- Root-owned runner execution
- Root cancellation command
- Destruction-driven cancellation orchestration
- Automatic startup
- Launch-session lifecycle advancement
- Public step events
- Splash presentation
- Destination selection or scene loading
- Persistent-root lifetime policy
- Direct-scene initialization
- Custom inspectors and setup windows
- Standalone Laboratory
- Peer-package bridges
- Player builds
- Performance claims

## Closure Result

Complete authored preflight and runner re-entry protection compile with zero errors and zero compiler warnings.

All two hundred eighty-eight Runtime Play Mode tests pass.

Implementation commit `b70a100` is present on `main` and `origin/main`.

FL-M3-05 is ready for its adjacent documentation commit.

The tentative next checkpoint is FL-M3-06 — Root-Owned Startup Run and Lifecycle Advancement. It is not authorized by this closeout.
