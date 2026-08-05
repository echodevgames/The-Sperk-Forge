# First Light Quick Start

## Current Capability

First Light version `0.1.0` currently provides:

- Launch-authority ownership
- Neutral launch-state vocabulary
- One live read-only launch session
- Guarded lifecycle publication

It does not yet execute startup sequences automatically.

## Approved Lifecycle

The active lifecycle is:

    AuthorityClaimed
        -> Validating
            -> Running
                -> Transitioning
                    -> Completed

Any active phase may also enter:

    Failed
    Interrupted

## Same-State Progress

An active state may publish another snapshot with the same status.

Example:

    Running at 25%
        -> Running at 50%
            -> Running at 80%

This changes progress without changing lifecycle phase.

## Illegal Transitions

First Light rejects:

- Backward transitions
- Skipped required phases
- `LaunchStatus.None` in an active session
- Publication after a terminal state
- Undefined status values

A rejected publication leaves the previous snapshot unchanged.

## Terminal States

These states permanently end the current session:

    Completed
    Failed
    Interrupted

A new launch requires a new `LaunchSession`.

## What This Does Not Do Yet

The package does not yet:

- Advance the lifecycle automatically
- Run startup steps
- Publish public lifecycle events
- Build a final launch report
- Display a splash
- Load an initial scene
- Persist between scenes
- Create project setup automatically

## Automated Evidence

The Runtime Play Mode suite reports:

- Passed: `82`
- Failed: `0`
- Ignored: `0`

See:

- [FL-M2-04 Runtime Test Report](../Developer/Test%20Reports/FL-M2-04_Launch_Lifecycle_Transition_Test_Report.md)
- [Developer Architecture](../Developer/Architecture.md)
