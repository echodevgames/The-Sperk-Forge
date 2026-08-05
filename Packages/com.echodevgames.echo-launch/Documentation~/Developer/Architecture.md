# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
- Unity baseline: `6000.3.8f1`

## Package Responsibility

First Light coordinates application startup.

The current implementation establishes:

1. A safe single-authority foundation
2. Neutral immutable vocabulary for launch state, step results, and progress snapshots

It does not yet execute startup behavior.

## Implemented Runtime Files

    Runtime/
    ├── Core/
    │   ├── LaunchAuthorityClaim.cs
    │   └── EchoLaunchRoot.cs
    ├── Properties/
    │   └── AssemblyInfo.cs
    ├── State/
    │   ├── LaunchMode.cs
    │   ├── LaunchStatus.cs
    │   └── LaunchProgressSnapshot.cs
    └── Steps/
        ├── StartupStepStatus.cs
        └── StartupStepResult.cs

    Tests/Runtime/
    └── PlayMode/
        ├── EchoLaunchRootAuthorityTests.cs
        └── LaunchStateVocabularyTests.cs

## Authority Core

`LaunchAuthorityClaim` owns claim, release, and subsystem-registration reset.

`EchoLaunchRoot` exposes the public scene-facing authority surface.

Duplicate roots are disabled and emit `ELAUNCH-ROOT-001`.

## Launch Modes

`LaunchMode` identifies how the launch attempt was entered:

- `Unknown`
- `CanonicalBoot`
- `DirectSceneDevelopment`

The default value is intentionally unresolved.

## Overall Launch Status

`LaunchStatus` describes the overall launch lifecycle vocabulary:

- `None`
- `AuthorityClaimed`
- `Validating`
- `Running`
- `Transitioning`
- `Completed`
- `Failed`
- `Interrupted`

These values do not cause transitions by themselves.

## Step Status

`StartupStepStatus` distinguishes active state from terminal outcomes:

Active:

- `NotStarted`
- `Running`

Terminal:

- `Succeeded`
- `Warning`
- `RecoverableFailure`
- `BlockingFailure`
- `Skipped`
- `TimedOut`
- `Cancelled`

## Structured Step Results

`StartupStepResult` is an immutable sealed class.

It contains:

- Status
- Stable diagnostic code
- Human-readable message
- Optional details
- Success classification
- Failure classification
- Blocking classification

Named factories prevent loose public construction.

Active statuses cannot become completed results.

Warning, recoverable failure, blocking failure, timeout, and cancellation require nonblank diagnostic codes and messages.

`Skipped`, `TimedOut`, and `Cancelled` remain policy-neutral. A future runner decides how authored policy affects continuation.

## Progress Snapshots

`LaunchProgressSnapshot` is an immutable readonly struct.

It records:

- Launch mode
- Overall status
- Active step identity
- Active step index
- Total step count
- Normalized progress
- Indeterminate-progress state
- Human-readable message
- Elapsed seconds
- Last completed step result

Validation prevents:

- Negative total counts
- Active indices below `-1`
- Active indices outside the total count
- NaN or infinite progress
- Progress outside `0` through `1`
- Negative, NaN, or infinite elapsed time

## Definition and Runtime-State Boundary

The architecture continues to distinguish:

    Definition = what should happen
    Runtime snapshot = what is happening now
    Report = what happened

FL-M2-02 implements runtime vocabulary only.

It does not introduce authored definitions or report aggregation.

## Test Evidence

Runtime Play Mode totals:

- Passed: `46`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Vocabulary tests: `39`

## Current Exclusions

Not implemented:

- Startup configuration assets
- Startup sequences
- Step definitions or executors
- Launch-session mutation
- Report aggregation
- Progress publication
- Splash presentation
- Scene loading
- Persistent-root lifetime policy
- Direct-scene initialization
- Editor setup tools
- Standalone Laboratory
- Peer-package bridges

## Checkpoint Stop Point

FL-M2-02 stops after the five vocabulary types and their validation suite.

The next runtime slice requires separate approval.
