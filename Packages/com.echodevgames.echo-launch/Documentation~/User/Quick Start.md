# First Light Quick Start

## Current Capability

First Light version `0.1.0` currently provides:

- Launch-authority ownership
- Neutral launch-state vocabulary

It does not yet execute startup sequences.

## Adding a Root

Add `EchoLaunchRoot` to one GameObject in a scene.

The first valid component becomes:

    EchoLaunchRoot.Current

For that component:

    IsAuthoritative == true
    WasRejectedAsDuplicate == false
    enabled == true

## Duplicate Behavior

If another `EchoLaunchRoot` awakens while an authority already exists:

- The first root remains authoritative.
- The duplicate sets `WasRejectedAsDuplicate` to `true`.
- The duplicate disables itself.
- First Light logs diagnostic code `ELAUNCH-ROOT-001`.

## Describing Launch State

Use `LaunchMode` to describe how launch was entered:

    Unknown
    CanonicalBoot
    DirectSceneDevelopment

Use `LaunchStatus` to describe the overall launch phase:

    None
    AuthorityClaimed
    Validating
    Running
    Transitioning
    Completed
    Failed
    Interrupted

Use `StartupStepStatus` to describe one startup step's active or terminal state.

## Creating Step Results

Use named factories:

    StartupStepResult.Success(...)
    StartupStepResult.Warning(...)
    StartupStepResult.RecoverableFailure(...)
    StartupStepResult.BlockingFailure(...)
    StartupStepResult.Skipped(...)
    StartupStepResult.TimedOut(...)
    StartupStepResult.Cancelled(...)

Warning and diagnostic failure outcomes require a nonblank code and message.

The result is immutable after creation.

## Creating Progress Snapshots

`LaunchProgressSnapshot` records one immutable observation of launch progress.

It validates:

- Total step count
- Active step index
- Progress from `0` through `1`
- Finite, nonnegative elapsed time

Creating a new snapshot does not mutate an earlier snapshot.

## What This Does Not Do Yet

The current package does not:

- Run startup steps
- Publish live progress
- Build a final launch report
- Load settings or audio
- Display a splash
- Load an initial scene
- Persist between scenes
- Create project setup automatically

## Automated Evidence

The Runtime Play Mode suite reports:

- Passed: `46`
- Failed: `0`
- Ignored: `0`

See:

- [FL-M2-02 Runtime Test Report](../Developer/Test%20Reports/FL-M2-02_Launch-State_Vocabulary_Test_Report.md)
- [Developer Architecture](../Developer/Architecture.md)
