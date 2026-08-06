# EchoLaunch ADR-008 — Direct Scene Development Initializer and Release-Safe Runtime Gate

## Metadata

- ADR: `EchoLaunch-ADR-008`
- Status: Approved
- Date: August 6, 2026
- Package: First Light (`EchoLaunch`)
- Package specification: SFGSS-PKG-ECHOLAUNCH-001 v1.11.0
- Checkpoint: FL-M5-05
- Decision owner: Jesse “Echo” Adams / EchoDevGames
- Baseline commit: `4e3bf34`

## Context

First Light already owns one protected runtime authority, one startup pipeline, one launch report, one destination handoff, non-destructive setup/repair, and a read-only Validator.

Developers still need to open a gameplay or Test Lab scene directly and press Play without entering the canonical Boot scene first. A careless helper could create a second bootstrap architecture, instantiate duplicate roots before scene-authored roots claim, reload the scene being tested, or accidentally run development bootstrap behavior in a release player.

FL-M5-05 must add convenience without weakening authority, duplicate rejection, immutable authored assets, report truthfulness, release safety, package independence, or Validator purity.

## Decision

### One helper, one launch architecture

Add `EchoDirectSceneInitializer : MonoBehaviour`.

It does not execute startup work. It waits until `Start`, reuses `EchoLaunchRoot.Current` when authority already exists, or instantiates one explicitly assigned project-owned direct root prefab through a project-owned immutable `DirectSceneConfiguration`.

The instantiated root performs the normal authority claim, splash, startup sequence, reporting, destination handoff, duplicate protection, and lifetime policy.

### Start, not Awake

Scene-authored roots claim in `Awake`. The helper settles in `Start`, ensuring existing roots win first.

Multiple initializers remain safe because the first accepted prefab claims in its own `Awake`; later initializers reuse it.

### Project-owned immutable configuration

`DirectSceneConfiguration` has schema version `1`, a stable ID, one project-owned direct root prefab, and one `DirectSceneEntryPolicy`.

The prefab must contain exactly one active `EchoLaunchRoot`, be authored with `LaunchMode.DirectSceneDevelopment`, reference a supported project-owned launch configuration, and target the scene containing the helper.

No `Resources`, labels, filenames, reflection, or global discovery are permitted. Runtime does not rewrite authored assets.

### Environment gate

Supported policies:

```text
EditorOnly
EditorAndDevelopmentBuilds
BootRequired
```

`EditorOnly` is the default.

`EditorAndDevelopmentBuilds` additionally permits `Debug.isDebugBuild`. `BootRequired` never creates a root.

A non-development player build is prohibited unconditionally for every serialized policy. No Release or Always policy exists.

Existing authority reuse remains allowed because it creates no development root.

No build hook is authorized in FL-M5-05.

### Active destination completes without reload

When the configured destination scene is already loaded and active, `UnityInitialDestinationLoader` returns normal success, reports progress `1`, and does not call `LoadSceneAsync`.

The final `LaunchReport` remains schema version `2` and records `LaunchMode.DirectSceneDevelopment`.

### Stable settlement

Statuses:

- `NotStarted`
- `ReusedExistingAuthority`
- `CreatedDevelopmentAuthority`
- `BlockedByPolicy`
- `BlockedByEnvironment`
- `InvalidConfiguration`
- `InstantiationFailed`

Diagnostics:

- `ELAUNCH-DIRECT-001` policy or environment prohibited.
- `ELAUNCH-DIRECT-002` invalid direct configuration/prefab/mode/configuration/destination.
- `ELAUNCH-DIRECT-003` instantiation failed.

The helper settles once, exposes one immutable/read-only result, logs at most one sanitized message, and disables further behavior.

### Validator activation

FL-M5-05 activates reserved `ELAUNCH-VAL-009`.

Blockers cover helper presence in canonical Boot, missing or non-project direct configuration/prefab, wrong root count or launch mode, unsupported/package-owned configuration or destination, and destination mismatch.

A valid `EditorOnly` helper outside Boot produces no finding. `EditorAndDevelopmentBuilds` in an enabled build scene produces a Warning.

Validation remains explicit and read-only.

### Authoring boundary

A custom Inspector may clarify policy and references. FL-M5-05 does not add automatic scene installation, Setup mutation, build hooks, or auto-fix.

## Rejected

- A second startup runner.
- Helper startup in `Awake`.
- Hidden discovery.
- Runtime asset mutation.
- Automatic destination override to the open scene.
- Reloading the active direct scene.
- Release/Always policy.
- Serialized bypass of the release gate.
- Build hooks.
- Automatic helper installation.
- Duplicate-root deletion.
- Report schema changes.
- Simulator, Laboratory, migration, receipt, uninstall, or recovery work.

## Consequences

Direct-scene testing uses the production root and pipeline, respects scene-authored authority, does not reload the tested scene, reports truthful direct mode, and cannot create a development root in release.

Projects must explicitly author one direct configuration and matching direct root prefab per destination shape they want to test.

## Validation obligations

FL-M5-05 must prove existing-authority reuse, one-root creation, multiple-helper convergence, direct report mode, no-reload active destination, unchanged canonical loading, policy/environment gates, unconditional release prohibition, invalid-input blocking before instantiate, one-shot settlement, Validator `ELAUNCH-VAL-009`, authored-asset immutability, full regression, and manual direct-scene acceptance.

## Supersession

This ADR extends EchoLaunch-ADR-007 and decision `ELAUNCH-D-007`. It does not weaken canonical Boot, runtime authority, setup/repair, Validator purity, package independence, or release-evidence boundaries.
