# FL-M5-01 - Editor Setup Foundation and Non-Destructive Project Plan

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.7.0
- ADR: EchoLaunch-ADR-004
- Checkpoint: `FL-M5-01`
- Milestone: M5 - Tooling and Direct Scene
- Authority commit: `b6a4f27`
- Implementation commit: `453bc14`
- Previous documentation commit: `8bd2a57`
- Implementation status: Complete and pushed
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Build the Editor-only observation and planning layer that explains exactly what
First Light setup would do without modifying project assets, scenes, prefabs,
EditorPrefs, or Build Settings.

## Implemented Architecture

```text
EchoLaunchSetupWindow
    -> EchoLaunchSetupRequest
    -> EchoLaunchProjectSnapshotCollector
    -> EchoLaunchProjectSnapshot
    -> EchoLaunchSetupPlanner
    -> EchoLaunchSetupPlan
    -> EchoLaunchSetupPlanTextFormatter
```

Observation and planning are implemented. Mutation remains absent.

## Implemented Contracts

- Build Settings, status, disposition, operation-kind, severity, and asset-role enums
- Immutable path set and setup request
- Immutable asset and Build Settings facts
- Immutable project snapshot
- Immutable diagnostics and operations
- Immutable setup plan with defensive collections
- Deterministic plain-text formatter

## Project Ownership and Path Policy

Default root:

```text
Assets/EchoDevGames/FirstLight
```

Default targets:

```text
Configuration/EchoLaunchConfiguration.asset
Configuration/StartupSequence.asset
Configuration/LaunchDestination.asset
Configuration/SplashSequence.asset
Prefabs/EchoLaunchRoot.prefab
Scenes/Boot.unity
```

The path layer rejects absolute paths, non-`Assets/` paths, traversal,
incorrect extensions, `Assets` as the root, and file-like project roots.

## Read-Only Snapshot

The collector reads asset existence, folder state, main type, GUID,
configuration schema, package-template availability, Build Settings order, and
compatible candidates.

It does not create folders/scenes, open scenes, dirty the package template, or
change Build Settings.

## Deterministic Plan

Statuses:

```text
Ready
ReadyWithWarnings
Blocked
```

Dispositions:

```text
Create
Reuse
NoChange
ManualDecision
Conflict
Unsupported
```

Missing targets become create proposals only. Compatible targets are reused.
Wrong types conflict. Unsupported schemas block. Multiple candidates require a
manual decision.

## Build Settings Planning

Policies:

```text
DoNotChange
AddIfMissingAtEnd
PlaceFirstAfterApproval
```

The default append-if-missing policy preserves unrelated scene order.

Place-first requires explicit approval.

No Build Settings mutation occurs.

## Setup Window

Menu:

```text
Tools/Sperk's Forge/First Light/Setup
```

The window can edit an in-memory request, select an existing destination scene,
refresh observation, display operations/diagnostics, and copy a text report.

It displays:

```text
Preview only. This checkpoint changes nothing in the project.
```

No Apply, Repair, Migrate, Create Assets, or Change Build Settings method is
exposed.

## Stable Diagnostics

- `ELAUNCH-SETUP-001`
- `ELAUNCH-SETUP-002`
- `ELAUNCH-SETUP-003`
- `ELAUNCH-SETUP-004`
- `ELAUNCH-SETUP-005`
- `ELAUNCH-SETUP-006`
- `ELAUNCH-SETUP-007`

## Implementation Scope

- 37 files
- 3,784 insertions
- Editor setup source and metadata
- Editor setup tests and metadata
- No Runtime production change
- No project `Assets/` content
- No scene, prefab, or `ProjectSettings` change

## Validation

Compilation:

- Errors: `0`
- Warnings: `0`

EditMode:

- Passed: `93`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Focused FL-M5-01 Editor tests: `66`
- Retained prefab asset tests: `27`

Runtime Play Mode:

- Passed: `479`
- Failed: `0`
- Ignored: `0`

Total:

- Passed: `572`
- Failed: `0`
- Ignored: `0`

## Git Integrity

- Generated `.slnx` noise restored.
- Three generated folder `.meta` files repaired.
- `git diff --cached --check` passed.
- Implementation commit `453bc14` pushed.
- Working tree clean and synchronized.

## Evidence Not Yet Run

- Setup apply/repair
- Asset, prefab-variant, and scene creation
- Build Settings mutation
- Undo/backup/recovery
- Configuration migration
- Direct-scene initializer
- Standalone Laboratory
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Closure Result

FL-M5-01 implementation is complete in `453bc14`.

The checkpoint is ready for its adjacent documentation closeout.

Tentative next checkpoint: FL-M5-02 - Approved Setup Apply Engine and
Repeat-Safe Asset Creation.
