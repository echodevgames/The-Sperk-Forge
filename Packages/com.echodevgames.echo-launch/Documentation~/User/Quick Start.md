# First Light Quick Start

## Current Capability

First Light version `0.1.0` currently provides:

- Launch-authority ownership
- Neutral launch-state vocabulary
- One live read-only launch session

It does not yet execute startup sequences.

## Adding a Root

Add `EchoLaunchRoot` to one GameObject in a scene.

The first valid root becomes authoritative and creates a fresh session.

Initial public state:

    State == LaunchStatus.AuthorityClaimed

Initial public progress:

    Progress.Mode == LaunchMode.CanonicalBoot
    Progress.Status == LaunchStatus.AuthorityClaimed
    Progress.ActiveStepIndex == -1
    Progress.TotalStepCount == 0
    Progress.Progress01 == 0
    Progress.IsProgressIndeterminate == true
    Progress.Message == "Launch authority claimed."

## Duplicate Behavior

A duplicate root:

- Is rejected
- Is disabled
- Logs `ELAUNCH-ROOT-001`
- Exposes `State == LaunchStatus.None`
- Exposes `Progress == LaunchProgressSnapshot.Empty`

## Empty Progress

`LaunchProgressSnapshot.Empty` is the safe canonical value for no active launch.

Its strings are normalized to empty strings rather than null.

## Read-Only Progress

Consumers may read:

    EchoLaunchRoot.State
    EchoLaunchRoot.Progress

Project code cannot directly replace session progress because publication remains internal.

## What This Does Not Do Yet

The package does not yet:

- Run startup steps
- Publish public progress events
- Enforce lifecycle transition rules
- Build a final launch report
- Display a splash
- Load an initial scene
- Persist between scenes
- Create project setup automatically

## Automated Evidence

The Runtime Play Mode suite reports:

- Passed: `60`
- Failed: `0`
- Ignored: `0`

See:

- [FL-M2-03 Runtime Test Report](../Developer/Test%20Reports/FL-M2-03_Launch_Session_and_Progress_Test_Report.md)
- [Developer Architecture](../Developer/Architecture.md)
