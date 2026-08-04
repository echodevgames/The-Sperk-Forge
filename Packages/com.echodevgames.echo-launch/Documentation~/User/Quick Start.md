# First Light Quick Start

## Current Capability

First Light version `0.1.0` contains the package skeleton and its first runtime slice.

The implemented runtime behavior is limited to launch-authority ownership.

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

The duplicate does not perform startup behavior.

## Authority Release

Destroying the authoritative root releases the claim.

Destroying a rejected duplicate does not affect the authoritative root.

Unity subsystem registration also clears stale static authority before runtime startup.

## What This Does Not Do Yet

The current component does not:

- Run startup steps
- Load settings
- Initialize audio
- Display a splash
- Produce a launch report
- Load an initial scene
- Persist between scenes
- Create project setup automatically

Those features belong to later approved checkpoints.

## Automated Evidence

The Runtime Play Mode suite contains seven passing tests with zero failures.

See:

- [FL-M2-01 Runtime Test Report](../Developer/Test%20Reports/FL-M2-01_Authority_Runtime_Test_Report.md)
- [Developer Architecture](../Developer/Architecture.md)
