# First Light - Startup and Launch

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

It coordinates ordered application initialization, launch reporting, launch-only presentation, direct-scene development initialization, and final handoff without owning the internal behavior of peer packages.

## Package Status

- Package version: `0.1.0`
- Development stage: Package skeleton complete
- Runtime behavior: Not implemented
- Unity baseline: `6000.3.8f1`
- Minimum declared Unity version: `6000.0`
- uGUI dependency: `2.0.0`

## Current Scope

This checkpoint establishes only the package boundary:

- Unity Package Manager manifest
- Runtime assembly
- Editor assembly
- Runtime test assembly
- Editor test assembly
- Package documentation shell

No C# runtime behavior, scenes, prefabs, ScriptableObjects, startup steps, presenters, setup tools, or bridges are included.

## Responsibility

First Light will eventually own:

- Claiming one startup authority
- Validating startup configuration
- Running startup steps in an intentional order
- Applying required and optional failure policies
- Producing structured launch reports
- Supporting direct-scene development initialization
- Handing control to the initial destination

First Light will not own:

- Audio playback
- Global preferences
- Save files or slots
- Production menus
- Normal scene travel after startup
- Input bindings
- Pause state
- Gameplay rules
- Peer package internals

## Assembly Layout

    Runtime/
        EchoDevGames.EchoLaunch.Runtime

    Editor/
        EchoDevGames.EchoLaunch.Editor

    Tests/Runtime/
        EchoDevGames.EchoLaunch.Tests.Runtime

    Tests/Editor/
        EchoDevGames.EchoLaunch.Tests.Editor

The dependency direction is:

    Editor -> Runtime
    Runtime Tests -> Runtime
    Editor Tests -> Editor + Runtime

The Runtime assembly must never reference the Editor or test assemblies.

## Documentation

Package documentation lives under `Documentation~`.

The suite-wide architectural authority and approved First Light specification live in the repository's `Plan Documentation` vault.

## Installation

This package is currently embedded directly in the Unity project at:

    Packages/com.echodevgames.echo-launch

External Git, tarball, registry, and public Package Manager installation evidence has not yet been collected.

## Evidence Status

The following evidence is available:

- Unity recognizes the embedded package.
- `package.json` parses successfully.
- Unity resolves uGUI `2.0.0`.
- All four assembly definitions parse and compile successfully.
- Unity restart verification passed.
- Embedded-package removal and reinstallation passed.
- Runtime and Editor asmdef GUIDs survived reinstallation unchanged.
- Package-local Markdown links resolve.
- No C# implementation files exist.

All runtime behavior, migration, performance, broad compatibility, public distribution, and behavioral test evidence remains `Not run`.

## License

See [LICENSE.md](LICENSE.md).

## Third-Party Notices

See [Third Party Notices.md](Third%20Party%20Notices.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).
