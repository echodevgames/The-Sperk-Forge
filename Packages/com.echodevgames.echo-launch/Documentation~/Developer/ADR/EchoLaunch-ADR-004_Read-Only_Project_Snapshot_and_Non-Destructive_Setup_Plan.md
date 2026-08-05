
# EchoLaunch ADR-004 — Read-Only Project Snapshot and Non-Destructive Setup Plan

## Metadata

- ADR: `EchoLaunch-ADR-004`
- Status: Approved
- Date: August 5, 2026
- Package: First Light (`EchoLaunch`)
- Package specification: SFGSS-PKG-ECHOLAUNCH-001 v1.7.0
- Checkpoint: FL-M5-01
- Decision owner: Jesse “Echo” Adams / EchoDevGames
- Baseline commit: `8bd2a57`

## Context

FL-M4 completed the standalone runtime loop and neutral package presentation
templates.

M5 begins Editor tooling. The specification requires setup and repair to be:

- Previewable.
- Repeat-safe.
- Non-destructive by default.
- Explicit about project-owned paths.
- Incapable of silently reordering unrelated Build Settings scenes.
- Incapable of overwriting project-owned assets.

Combining observation, planning, and mutation in one Editor action would make
those guarantees difficult to prove. The first tooling checkpoint therefore
needs a hard architectural seam before any create/repair action exists.

## Decision

### Separate observation, planning, and mutation

The approved setup flow is:

```text
read-only collector
    -> immutable project snapshot
    -> deterministic pure planner
    -> immutable setup plan
    -> later separately approved apply service
```

FL-M5-01 implements no apply service.

### Read-only project snapshot

`EchoLaunchProjectSnapshotCollector` may inspect:

- Asset existence and main asset type.
- Asset GUIDs.
- Package template availability.
- EditorBuildSettings scene paths, enabled states, and order.
- Selected destination scene validity.
- Configuration schema when a compatible configuration asset already exists.

It may not:

- Create/import/move/delete assets.
- Open or save scenes.
- Instantiate prefabs.
- Change Build Settings.
- Change serialized objects.
- Run migrations.
- Mark assets dirty.

The snapshot is an immutable value object detached from later Unity state.

### Immutable setup request

`EchoLaunchSetupRequest` captures user intent in memory:

- Project root path.
- Boot scene path.
- Destination scene path.
- Whether to create an optional splash sequence.
- Build Settings policy.
- Explicit selected existing assets when ambiguity exists.

The request is not a ScriptableObject and is not stored in project assets.

FL-M5-01 does not store project identity in EditorPrefs.

### Default project-owned paths

The default project root is:

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

The splash target is included only when requested.

The destination scene must already exist.

### Deterministic plan

`EchoLaunchSetupPlanner` is pure after receiving the request and snapshot.

The same request and equivalent snapshot must produce value-equivalent ordered
plans.

Plan statuses:

```text
Ready
ReadyWithWarnings
Blocked
```

Operation dispositions:

```text
Create
Reuse
NoChange
ManualDecision
Conflict
Unsupported
```

Each operation includes:

- Stable operation key.
- Operation kind.
- Disposition.
- Target path.
- Plain-language reason.
- Stable diagnostic code when relevant.
- Explicit-approval requirement.
- Phase/order information.

### Operation phases

The plan uses this deterministic phase order:

1. Validate request and package prerequisites.
2. Ensure project folders.
3. Resolve project-owned definition assets.
4. Resolve project-owned root prefab variant.
5. Resolve Boot scene.
6. Resolve Build Settings entry/order.
7. Summarize blockers, warnings, and manual decisions.

FL-M5-01 previews these operations only.

### Build Settings policy

Approved policies:

```text
DoNotChange
AddIfMissingAtEnd
PlaceFirstAfterApproval
```

Default:

```text
AddIfMissingAtEnd
```

`PlaceFirstAfterApproval` is always marked as requiring explicit approval and
must preserve the relative order of all unrelated scenes.

No Build Settings mutation occurs in FL-M5-01.

### Existing asset handling

- Exact compatible asset at target path: `Reuse` or `NoChange`.
- Wrong asset type at target path: `Conflict`.
- Multiple compatible candidates without explicit selection:
  `ManualDecision`.
- Missing asset: proposed `Create`.
- Package template unavailable: blocked `Unsupported`.
- Unsupported configuration schema: blocked migration diagnostic.
- Existing project asset is never proposed for overwrite.

### Root template adoption

A later apply checkpoint creates a project-owned prefab variant from the stable
package `EchoLaunchRoot.prefab`.

The package template itself is never modified.

The future variant receives the project-owned configuration reference.

### Preview-only Setup window

FL-M5-01 creates:

```text
Tools > Sperk's Forge > First Light > Setup
```

The window may:

- Edit the in-memory request.
- Refresh observation.
- Display status, operations, diagnostics, and paths.
- Copy a plain-text dry-run report.
- Ping existing assets.

The window has no Apply, Repair, Migrate, Create, or Change Build Settings
button.

### Stable diagnostics

Approved setup diagnostics:

- `ELAUNCH-SETUP-001` invalid project path/request.
- `ELAUNCH-SETUP-002` incompatible asset at target path.
- `ELAUNCH-SETUP-003` unsupported schema requires migration.
- `ELAUNCH-SETUP-004` Build Settings reorder requires approval.
- `ELAUNCH-SETUP-005` ambiguous compatible candidates.
- `ELAUNCH-SETUP-006` required package template/script unavailable.
- `ELAUNCH-SETUP-007` compatible existing asset will be reused.

## Rejected alternatives

### One-click setup before dry-run proof

Rejected.

It mixes planning and mutation before repeatability and conflict behavior are
proven.

### Mutable plan objects backed by Unity assets

Rejected.

Plans are transient evidence, not project configuration.

### Automatic search-and-adopt of arbitrary existing boot managers

Rejected.

The planner may report candidates but cannot infer replacement authority.

### Silent Boot scene promotion to build index zero

Rejected.

Reordering existing scenes requires explicit approval.

### Overwrite compatible-looking project assets

Rejected.

Project ownership wins. Existing assets are reused or surfaced for manual
decision.

### Store selected project identity in EditorPrefs

Rejected for this checkpoint.

It risks machine-local hidden behavior and cross-project confusion.

### Open scenes to inspect root contents during planning

Rejected for FL-M5-01.

Opening scenes changes Editor state. Deeper scene validation belongs to a later
validator/apply checkpoint with explicit restoration rules.

## Consequences

### Positive

- The mutation boundary becomes testable before writes exist.
- Dry-run output can be reviewed and copied.
- Existing projects receive conflict evidence instead of surprise edits.
- Build Settings order remains protected.
- Future apply/repair services can consume one approved immutable plan.
- Repeatability can be tested as value equality.

### Costs

- FL-M5-01 cannot complete setup by itself.
- Scene-content inspection is intentionally shallow.
- Ambiguous existing assets require user selection.
- Apply, Undo, backup, receipt, and recovery behavior remain later work.

## Implementation boundary

FL-M5-01 may add:

- Editor setup contracts and enums.
- Read-only snapshot collector.
- Pure setup planner.
- Preview-only Setup window.
- Plain-text plan formatter.
- Editor tests for purity, ordering, paths, conflicts, Build Settings policy,
  and window availability.

FL-M5-01 may not add:

- Asset or folder creation.
- Prefab copy/variant creation.
- Scene creation/open/save.
- Build Settings mutation.
- Configuration migration.
- Setup receipt or project manifest.
- Direct-scene initializer.
- Runtime code changes.
- Test Lab scenes.
- Player-build claims.

## Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
