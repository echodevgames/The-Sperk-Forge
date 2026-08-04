# First Light Quick Start

## Current Package Stage

First Light version `0.1.0` contains the package skeleton only.

Unity can recognize, resolve, and compile the package, but First Light does not yet provide runtime startup behavior.

There is currently no component to add to a scene and no startup sequence to configure.

## Verify the Package

Open Unity Package Manager and select:

    First Light - Startup and Launch

Confirm:

- Package ID: `com.echodevgames.echo-launch`
- Version: `0.1.0`
- Source: Custom or Embedded
- Unity requirement: `6000.0`
- uGUI dependency: `2.0.0`

The Unity Console should contain zero First Light errors.

## Current Assembly Boundaries

The package currently defines four assemblies:

    EchoDevGames.EchoLaunch.Runtime
    EchoDevGames.EchoLaunch.Editor
    EchoDevGames.EchoLaunch.Tests.Runtime
    EchoDevGames.EchoLaunch.Tests.Editor

Their intended dependency direction is:

    Editor -> Runtime
    Runtime Tests -> Runtime
    Editor Tests -> Editor + Runtime

No C# scripts currently exist inside these assemblies.

## What You Cannot Do Yet

Version `0.1.0` cannot yet:

- Create an `EchoLaunchRoot`
- Author a startup sequence
- Run startup steps
- Display a startup splash
- Produce a runtime launch report
- Initialize a directly opened development scene
- Load an initial destination
- Validate First Light configuration
- Run a Standalone Laboratory

These capabilities belong to later implementation checkpoints.

## Current Developer Workflow

At this stage, the useful workflow is:

1. Confirm Unity recognizes the package.
2. Confirm all assembly definitions compile.
3. Review the package documentation.
4. Preserve all generated `.meta` files.
5. Commit the package skeleton with `Packages/packages-lock.json`.
6. Stop before adding runtime C# behavior.

## Next Development Milestone

The next implementation milestone will be authorized only after FL-M1-01 closes successfully.

That later milestone will introduce a small, explicitly approved portion of the First Light runtime architecture.

Do not create startup scripts, components, assets, scenes, prefabs, or setup tools during the package-skeleton checkpoint.

## More Information

- [Installation](Installation.md)
- [Package Documentation Index](../Index.md)
- [Package README](../../README.md)
- [Changelog](../../CHANGELOG.md)