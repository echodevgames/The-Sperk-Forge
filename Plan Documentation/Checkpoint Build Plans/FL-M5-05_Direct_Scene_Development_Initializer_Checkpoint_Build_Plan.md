# FL-M5-05 — Direct Scene Development Initializer

## 1. Document Control

| Field | Value |
|---|---|
| Document ID | FL-M5-05 |
| Version | 1.0.0 |
| Status | Approved |
| Package | First Light (`EchoLaunch`) |
| Package specification | SFGSS-PKG-ECHOLAUNCH-001 v1.11.0 |
| ADR | EchoLaunch-ADR-008 |
| Milestone | M5 — Tooling and Direct Scene |
| Repository | The-Sperk-Forge |
| Branch | `main` |
| Required baseline | `4e3bf34` |
| Unity baseline | `6000.3.8f1` |
| Owner | Jesse “Echo” Adams / EchoDevGames |
| Last updated | August 6, 2026 |
| Approved by | Jesse “Echo” Adams / EchoDevGames |

## 2. Purpose

A developer may open an explicitly configured gameplay or Test Lab scene and press Play.

```text
Existing authority -> reuse, create nothing
No authority + valid Editor policy -> instantiate one direct root
Direct destination already active -> complete without reload
Non-development release player -> creation impossible
```

The helper does not become a second startup system.

## 3. Starting Conditions

- HEAD: `4e3bf34`
- Working tree: clean
- `main` equals `origin/main`
- Package version: `0.1.0`
- FL-M5-04 authority: `c2397c9`
- FL-M5-04 implementation: `26732ea`
- FL-M5-04 documentation: `4e3bf34`
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `261` passed
- Runtime PlayMode baseline: `479` passed
- Total automated baseline: `740` passed
- `LaunchMode.DirectSceneDevelopment` and report schema `2` already exist

## 4. Learning Review

The checkpoint question is:

> How can the already-open scene enter the same startup architecture without duplicate authority or scene reload?

Rules:

1. Scene roots claim in `Awake`; helper waits until `Start`.
2. Existing authority is reused.
3. Helper uses a pre-authored project-owned direct configuration and prefab.
4. Destination must match the containing scene.
5. Active destination completes without reload.
6. Runtime code, not author memory, prohibits release creation.

## 5. Scope

- `DirectSceneConfiguration` schema `1`
- `DirectSceneEntryPolicy`
- `DirectSceneInitializationStatus`
- Immutable settlement result
- Stable `ELAUNCH-DIRECT-001` through `003`
- `EchoDirectSceneInitializer`
- Runtime environment seam
- Start-time reuse/create logic
- Active-destination no-reload path
- Direct report mode
- Optional custom Inspector
- Validator direct-helper evidence
- Activated `ELAUNCH-VAL-009`
- Focused EditMode and PlayMode tests
- Full regression
- Manual acceptance and documentation closeout

## 6. Exclusions

- Second startup runner
- Hidden discovery
- Runtime authored-asset mutation
- Automatic destination override
- Release/Always policy
- Build hooks or build blocking
- Automatic helper installation
- Setup mutation
- Duplicate deletion
- Simulator or Laboratory
- Migration, receipt, uninstall/reset, recovery, distribution, or adoption claims

## 7. Runtime contracts

### Policy

```text
EditorOnly
EditorAndDevelopmentBuilds
BootRequired
```

Default: `EditorOnly`. Unknown values block. Non-development release creation is impossible for all values.

### Status

```text
NotStarted
ReusedExistingAuthority
CreatedDevelopmentAuthority
BlockedByPolicy
BlockedByEnvironment
InvalidConfiguration
InstantiationFailed
```

### Configuration

Project-owned immutable ScriptableObject:

- Schema version `1`
- Stable direct configuration ID
- Project-owned direct root prefab
- Entry policy

### Initializer

Serialized:

- `DirectSceneConfiguration`
- Optional one-shot informational logging, default enabled

Read-only:

- Configured direct configuration
- HasSettled
- LastResult

`Start` calls idempotent `EnsureDevelopmentLaunch()` once, then the component disables itself.

### Root validation

Before instantiate:

- Configuration and prefab exist.
- Exactly one active `EchoLaunchRoot`.
- Authored mode is `DirectSceneDevelopment`.
- Launch configuration and destination are supported.
- Destination path equals containing scene path.

Editor Validator provides stronger project-path and prefab-lineage evidence.

### Active destination

Before `LoadSceneAsync`, the loader checks whether the destination scene is already loaded and active. When true, it reports progress `1` and returns success without loading.

Cancellation ordering and canonical non-active loading remain unchanged.

## 8. Validator `ELAUNCH-VAL-009`

Blocker:

- Helper in canonical Boot.
- Missing/non-project direct configuration or prefab.
- Wrong root count or inactive root.
- Wrong launch mode.
- Invalid/package-owned configuration or destination.
- Destination differs from containing scene.
- Unknown policy.

Warning:

- `EditorAndDevelopmentBuilds` in an enabled build scene.

No finding:

- Valid `EditorOnly` helper outside Boot.

Report schema remains `1`; validation remains read-only and deterministic.

## 9. Files

### Create

- `Runtime/DirectScene/DirectSceneEntryPolicy.cs`
- `Runtime/DirectScene/DirectSceneInitializationStatus.cs`
- `Runtime/DirectScene/DirectSceneInitializationResult.cs`
- `Runtime/DirectScene/DirectSceneConfiguration.cs`
- `Runtime/DirectScene/IDirectSceneRuntimeEnvironment.cs`
- `Runtime/DirectScene/UnityDirectSceneRuntimeEnvironment.cs`
- `Runtime/DirectScene/EchoDirectSceneInitializer.cs`
- `Editor/DirectScene/EchoDirectSceneInitializerEditor.cs`
- Focused Editor and PlayMode DirectScene tests and matching metadata

### Modify only as required

- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/SceneLoading/UnityInitialDestinationLoader.cs`
- Validator evidence, collector, fingerprint, rule catalog, and related tests
- Existing destination/root tests for regression

Do not modify report schema, presentation, Setup Apply/Repair, package prefabs, ProjectSettings, or project assets for implementation.

## 10. Implementation sequence

1. Add immutable configuration, policy, status, result, and environment seam.
2. Prove release prohibition and default policy.
3. Expose only internal pre-Awake root evidence required for validation.
4. Implement one-shot Start settlement.
5. Reuse existing authority first.
6. Validate configuration/prefab/mode/destination before instantiate.
7. Instantiate exactly one root and capture result.
8. Add active-destination success/no-reload loader path.
9. Collect direct-helper Validator evidence and activate `VAL-009`.
10. Add optional Inspector with no mutation buttons.
11. Run focused and complete automated gates.
12. Complete manual direct-scene acceptance.
13. Remove temporary assets and restore repository drift.
14. Commit implementation and adjacent documentation.

## 11. Test matrix

- Default `EditorOnly`
- Unknown policy blocks
- Existing authority reused
- Valid Editor creation
- Multiple helpers converge to one authority
- `BootRequired` blocks with `DIRECT-001`
- `EditorOnly` blocks player
- Development opt-in permits debug build
- Every policy blocks non-development release creation
- Missing/wrong prefab, mode, configuration, or destination gives `DIRECT-002`
- Instantiate exception gives `DIRECT-003`
- Settlement is idempotent
- Active destination succeeds without `LoadSceneAsync`
- Direct report mode
- Canonical destination loading unchanged
- Valid EditorOnly helper produces no `VAL-009`
- Development opt-in produces Warning
- Invalid/Boot helper produces Blocker
- Validator remains deterministic and read-only
- Authored assets remain unchanged
- Complete EditMode and PlayMode regression

## 12. Manual acceptance

1. Generate a temporary canonical foundation.
2. Create a project-owned direct configuration and direct root prefab variant.
3. Author root mode `DirectSceneDevelopment`.
4. Match destination to the directly opened scene.
5. Add one `EditorOnly` initializer to that scene.
6. Enter Play directly.
7. Prove one root, one launch, direct report mode, and no scene reload.
8. Prove existing-root reuse.
9. Prove two initializers still yield one authority.
10. Validate Healthy with valid EditorOnly helper.
11. Change to Development-Build opt-in and validate `NeedsAttention` with `VAL-009`.
12. Restore EditorOnly and exact Healthy result.
13. Remove temporary project content.
14. Restore Build Settings and solution drift.
15. Stage only approved package code/tests/metadata.

## 13. Completion criteria

- Specification v1.11.0 and ADR-008 satisfied
- One startup architecture
- Exactly one authority
- Active scene not reloaded
- Report mode truthful
- Release creation impossible
- `VAL-009` active and read-only
- Focused and complete tests green
- Manual acceptance green
- No acceptance residue
- Documentation reconciled and pushed

## 14. Stop point

Stop after Direct Scene initializer, no-reload handoff, release gate, Validator rule, tests, acceptance, and documentation closeout.

Do not continue into build hooks, automatic installation, Simulator, Laboratory, migration, distribution, or project adoption.
