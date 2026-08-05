
# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-01`
- Title: Editor Setup Foundation and Non-Destructive Project Plan
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.7.0
- ADR: EchoLaunch-ADR-004
- Status: Authority approved; Editor implementation locked until authority commit
- Repository baseline: `8bd2a57`
- Last implementation commit: `8d3c6a7`
- EditMode baseline: 27 passed, 0 failed, 0 ignored
- Runtime Play Mode baseline: 479 passed, 0 failed, 0 ignored
- Compilation baseline: 0 errors, 0 compiler warnings

## Approved Contract

### Architecture

```text
read-only snapshot collector
    -> immutable project snapshot
    -> deterministic setup planner
    -> immutable setup plan
    -> preview-only Setup window
```

Mutation is not implemented in FL-M5-01.

### Setup Window

Menu:

```text
Tools > Sperk's Forge > First Light > Setup
```

The window can edit an in-memory request, refresh observation, display
operations/diagnostics, and copy a plain-text report.

The window has no Apply, Repair, Migrate, Create, or Build Settings action.

### Default Paths

```text
Assets/EchoDevGames/FirstLight
```

with Configuration, Prefabs, and Scenes children.

The destination scene must already exist.

### Existing Project Safety

- Compatible existing assets: reuse.
- Incompatible target path: block.
- Multiple candidates: manual decision.
- Unsupported schema: block migration.
- Default Build Settings plan: append if missing.
- Place-first plan: explicit approval.
- Unrelated scene order: preserved.
- Package template: never modified.
- Project-owned root: future prefab variant.
- No EditorPrefs project identity.
- No scene opening during collection.

### Diagnostics

- `ELAUNCH-SETUP-001`
- `ELAUNCH-SETUP-002`
- `ELAUNCH-SETUP-003`
- `ELAUNCH-SETUP-004`
- `ELAUNCH-SETUP-005`
- `ELAUNCH-SETUP-006`
- `ELAUNCH-SETUP-007`

## Implementation Lock

Do not add Editor setup code until the authority commit is pushed.

Required authority commit:

```text
echo-launch: approve FL-M5-01 non-destructive setup planning
```

## Expected Final Implementation Scope

- Internal Editor setup value contracts
- Read-only project snapshot collector
- Pure deterministic planner
- Path validation/defaults
- Plain-text plan formatter
- Preview-only Setup window
- Editor test assembly and focused tests
- No Runtime production change
- No Assets, scene, prefab, or ProjectSettings change

## Explicit Exclusions

- Plan apply
- Asset/folder creation
- Prefab variant creation
- Boot scene generation
- Build Settings mutation
- Undo/backup/receipt
- Schema migration
- Direct-scene initializer
- Validator/simulator/report viewer
- Test Lab
- Player builds

## Handoff Snapshot

FL-M4-05 is fully closed at `8bd2a57`.

FL-M5-01 authority is prepared through specification v1.7.0, ADR-004, and the
approved Checkpoint Build Plan.

Editor implementation begins only after the authority commit is confirmed.
