# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Package skeleton
- Runtime behavior: Not implemented
- Active checkpoint: `FL-M1-01`
- Architectural authority: Suite Bible, suite standards, and the approved First Light package specification

This document describes the package boundaries established by the current checkpoint. It does not claim that the planned startup runtime has been implemented.

## Package Responsibility

First Light is responsible for coordinating application startup.

Its future responsibilities include:

- Claiming one startup authority
- Validating startup configuration
- Running startup steps in an intentional order
- Applying required and optional failure policies
- Producing a structured launch report
- Supporting direct-scene development initialization
- Handing control to the first approved destination

First Light does not own the internal behavior of other packages.

For example:

    First Light requests that audio initialize.
    Jukebot owns the actual audio initialization.

## Current Package Layout

    com.echodevgames.echo-launch/
    ├── package.json
    ├── README.md
    ├── CHANGELOG.md
    ├── LICENSE.md
    ├── Third Party Notices.md
    ├── Runtime/
    │   └── EchoDevGames.EchoLaunch.Runtime.asmdef
    ├── Editor/
    │   └── EchoDevGames.EchoLaunch.Editor.asmdef
    ├── Tests/
    │   ├── Runtime/
    │   │   └── EchoDevGames.EchoLaunch.Tests.Runtime.asmdef
    │   └── Editor/
    │       └── EchoDevGames.EchoLaunch.Tests.Editor.asmdef
    └── Documentation~/
        ├── Index.md
        ├── User/
        └── Developer/

No C# implementation files currently exist.

## Assembly Responsibilities

### Runtime Assembly

Assembly:

    EchoDevGames.EchoLaunch.Runtime

Root namespace:

    EchoDevGames.EchoLaunch

This assembly will eventually contain Player-safe startup contracts and runtime behavior.

It may use UnityEngine APIs.

It must not reference:

- The Editor assembly
- Either test assembly
- Optional peer Echo packages
- Project-owned gameplay assemblies

Optional integrations belong in separate bridges or project adapters.

### Editor Assembly

Assembly:

    EchoDevGames.EchoLaunch.Editor

Root namespace:

    EchoDevGames.EchoLaunch.Editor

This assembly is restricted to the Unity Editor.

It may reference the Runtime assembly.

It will eventually contain package setup, validation, repair, inspection, and development tooling.

It must never be required by a Player build.

### Runtime Test Assembly

Assembly:

    EchoDevGames.EchoLaunch.Tests.Runtime

This assembly references the Runtime assembly and Unity Test Framework support.

It will eventually test Player-safe startup contracts and runtime behavior.

It must not become a dependency of production code.

### Editor Test Assembly

Assembly:

    EchoDevGames.EchoLaunch.Tests.Editor

This assembly references both the Runtime and Editor assemblies.

It will eventually test setup tools, validators, inspectors, migrations, and other Editor-only behavior.

It is restricted to the Unity Editor.

## Dependency Direction

The approved assembly dependency direction is:

    Editor -> Runtime

    Runtime Tests -> Runtime

    Editor Tests -> Editor + Runtime

The following directions are prohibited:

    Runtime -X-> Editor

    Runtime -X-> Tests

    Editor -X-> Tests

    Production Project Code -X-> Test Assemblies

The Runtime assembly is the lowest reusable First Light code boundary.

## Stable Assembly References

The Editor and test assemblies use Unity asset GUID references to identify their assembly dependencies.

Current assembly-definition GUIDs:

- Runtime: `6370d00c0cfa8144795d367cb689f221`
- Editor: `994a9bf984e48cc4a9c5139c901e11f6`

These GUIDs come from Unity-generated `.meta` files.

The `.meta` files must be preserved in Git because changing or deleting them can break assembly references.

## Definition and Runtime-State Boundary

The future startup architecture will distinguish:

    Definition = what should happen

    Runtime state = what is happening now

    Launch report = what happened

Authored configuration must not store temporary launch progress.

Runtime executors must be fresh and single-use for each launch attempt.

The package skeleton does not implement these concepts yet.

## Planned Runtime Concepts

Later checkpoints may introduce approved concepts such as:

- `EchoLaunchRoot`
- Startup sequence definitions
- Startup step definitions
- Startup step executors
- Launch context
- Launch-step results
- Launch reports
- Normal Boot mode
- Direct-scene development mode
- Launch-only presentation state

Their exact implementation remains controlled by later checkpoint plans.

## Presentation Boundary

The neutral Runtime assembly must remain independent from a specific UI implementation.

The approved default uGUI presentation path will live in a separate presentation assembly when authorized.

Presentation may display:

- Logos
- Startup status
- Progress
- Warnings
- Blocking failures

Presentation must not decide whether startup succeeds, fails, or continues.

## Package Dependency Boundary

The package manifest currently declares:

    com.unity.ugui 2.0.0

No other Sperk's Forge package is a hard dependency.

First Light must remain independently installable.

Optional integrations with settings, diagnostics, audio, saves, UI, or scene flow will use explicit bridges or startup-step adapters rather than hidden peer-package references.

## Persistence Boundary

First Light does not own save files or global preferences.

Temporary startup state is session-only.

A launch report may be exposed to diagnostics or project code, but First Light does not silently persist it as game-save data.

## Current Evidence

The following evidence has been collected:

- Unity recognizes the embedded package.
- `package.json` parses successfully.
- Unity resolves uGUI `2.0.0`.
- All four assembly definitions parse successfully.
- Unity generated stable `.meta` files.
- The package compiles with zero Console errors.
- No C# implementation files exist.

The following evidence remains `Not run`:

- Runtime behavior
- Automated tests
- Standalone Laboratory
- Removal and reinstallation
- Git installation
- Tarball installation
- Clean-project installation
- Player builds
- Platform compatibility
- Performance measurements

## Checkpoint Stop Point

`FL-M1-01` must stop before the first C# implementation file.

Completing the package skeleton does not authorize runtime startup behavior.