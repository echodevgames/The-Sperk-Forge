# FL-M2-02 - Neutral Launch-State Vocabulary

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M2-02`
- Status: Complete, pending commit and push
- Unity baseline: `6000.3.8f1`

## Goal

Create the neutral immutable vocabulary First Light will later use to describe:

- Launch entry mode
- Overall launch state
- Startup-step state
- Terminal step results
- Progress snapshots

## Authorized Runtime Files

    Runtime/State/LaunchMode.cs
    Runtime/State/LaunchStatus.cs
    Runtime/State/LaunchProgressSnapshot.cs
    Runtime/Steps/StartupStepStatus.cs
    Runtime/Steps/StartupStepResult.cs

Test file:

    Tests/Runtime/PlayMode/LaunchStateVocabularyTests.cs

## Implemented Vocabulary

### Launch Mode

- `Unknown`
- `CanonicalBoot`
- `DirectSceneDevelopment`

### Launch Status

- `None`
- `AuthorityClaimed`
- `Validating`
- `Running`
- `Transitioning`
- `Completed`
- `Failed`
- `Interrupted`

### Startup Step Status

- `NotStarted`
- `Running`
- `Succeeded`
- `Warning`
- `RecoverableFailure`
- `BlockingFailure`
- `Skipped`
- `TimedOut`
- `Cancelled`

## Structured Result Contract

`StartupStepResult` is immutable.

It exposes named factories and rejects active or undefined statuses.

Diagnostic outcomes require nonblank codes and messages.

Its convenience properties remain policy-neutral.

## Progress Snapshot Contract

`LaunchProgressSnapshot` is an immutable readonly struct.

It validates counts, indices, progress, and elapsed time.

Null strings normalize to empty strings.

A previous snapshot remains unchanged when a later snapshot is created.

## Test Evidence

| Area | Result |
|---|---|
| Stable enum values | Pass |
| Result factories | Pass |
| Result classification | Pass |
| Diagnostic validation | Pass |
| Text normalization | Pass |
| Valid snapshots | Pass |
| Invalid counts and indices | Pass |
| Invalid progress | Pass |
| Invalid elapsed time | Pass |
| Snapshot immutability | Pass |

Totals for FL-M2-02:

- Passed: `39`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `46`
- Failed: `0`
- Ignored: `0`

## Explicit Exclusions

Not implemented:

- `EchoLaunchRoot` lifecycle changes
- Startup definitions
- Startup executors
- ScriptableObjects
- Events
- Cancellation behavior
- Timeout behavior
- Report aggregation
- Presentation
- Scene loading
- Editor tooling
- Test Lab scenes
- Peer-package bridges

## Closure Result

The exact approved vocabulary slice compiles and all thirty-nine vocabulary tests pass.

FL-M2-02 is ready for commit and push.

The next runtime checkpoint requires separate approval.
