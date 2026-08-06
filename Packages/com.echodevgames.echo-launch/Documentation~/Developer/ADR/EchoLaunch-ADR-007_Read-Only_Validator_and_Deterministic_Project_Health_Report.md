# EchoLaunch ADR-007 — Read-Only Validator and Deterministic Project Health Report

## Metadata

- ADR: `EchoLaunch-ADR-007`
- Status: Approved
- Date: August 6, 2026
- Package: First Light (`EchoLaunch`)
- Package specification: SFGSS-PKG-ECHOLAUNCH-001 v1.10.0
- Checkpoint: FL-M5-04
- Decision owner: Jesse “Echo” Adams / EchoDevGames
- Baseline commit: `638e676`

## Context

FL-M5-01 established side-effect-free project evidence and deterministic setup
planning. FL-M5-02 added a fresh-plan-gated create-only Apply transaction.
FL-M5-03 added a separate proof-backed Repair transaction for narrowly approved
current-schema drift.

First Light can now create and reconcile its canonical project foundation, but a
maintainer still lacks one authoritative read-only answer to a simpler question:

> Is this installed First Light foundation healthy, and if not, what exactly is
> wrong?

The Setup plan explains intended creation and repair operations. It is not a
general project-health report, must remain centered on one setup request, and
must not silently grow into a validator with unrelated rule ownership.

The Validator therefore needs separate Editor authority, stable diagnostic
rules, an immutable report, deterministic repeatability, scene-safe inspection,
and an explicit prohibition against mutation.

## Decision

### The Validator is a distinct read-only Editor surface

FL-M5-04 adds:

```text
Tools > Sperk's Forge > First Light > Validator
```

Opening the window, changing its project-root field, importing the package,
refreshing Unity, entering Play Mode, or drawing the Inspector must not run
validation automatically.

The user explicitly presses:

```text
Validate Project
```

The Validator does not expose Apply, Repair, migration, delete, move, rename,
disable, or fix buttons.

The Setup window remains the authority for previewed creation and repair.
Validation findings may tell the user to open Setup, but the Validator never
invokes Setup mutation on the user's behalf.

### Validation target

The validation request contains:

- Project-owned First Light root path.
- Whether informational findings are included in the copied report.

The default root is:

```text
Assets/EchoDevGames/FirstLight
```

The canonical asset, prefab, and Boot-scene paths are derived from that root.
The configured destination is derived from the existing
`EchoLaunchConfiguration` and `LaunchDestination`; the Validator does not ask
the user to choose a different destination merely to make the report pass.

### Read-only scene inspection

The Validator may inspect:

- The canonical Boot scene.
- Every enabled scene in Editor Build Settings.
- Already-open scenes relevant to the validation target.

A closed scene may be opened additively for inspection only when the Validator
can preserve the user's existing scene set, active scene, and dirty states.

Validation must:

1. Capture the existing open-scene set, active scene, and dirty flags.
2. Inspect without saving.
3. Close only scenes opened by the Validator.
4. Restore the active scene.
5. Preserve every pre-existing dirty state.
6. Leave scene files and `.meta` files byte-identical.

If safe inspection cannot be completed, validation emits a blocking evidence
finding rather than guessing.

### No project mutation

A validation run must not:

- Create or delete files or folders.
- Save or import project assets as a mutation step.
- Rewrite serialized objects.
- Change prefab overrides.
- Save scenes.
- Change Editor Build Settings.
- Change package assets.
- Change stable IDs or schemas.
- Dirty any asset, prefab, scene, or project setting.
- Invoke Apply or Repair.
- Create setup receipts or backups.

Ordinary read-only `AssetDatabase`, `SerializedObject`, prefab-source, and scene
inspection APIs are permitted. A final `AssetDatabase.Refresh` is not part of
successful validation because it is unnecessary and can obscure mutation
evidence.

### Immutable validation contracts

FL-M5-04 owns immutable Editor contracts for:

- `EchoLaunchValidationRequest`
- `EchoLaunchValidationSeverity`
- `EchoLaunchProjectHealth`
- `EchoLaunchValidationFinding`
- `EchoLaunchValidationReport`

The report uses schema version `1`.

A finding contains only immutable, sanitized values:

- Stable diagnostic code.
- Severity.
- Short title.
- Actionable message.
- Project-relative path when applicable.
- Evidence summary.
- Suggested user action.

It must not retain mutable Unity objects, scene handles, `SerializedObject`
instances, or exception objects.

All collections are defensively copied.

### Health model

The report health is derived from its findings:

| Highest finding severity | Project health |
|---|---|
| None or Information | `Healthy` |
| Warning | `NeedsAttention` |
| Error | `Invalid` |
| Blocker | `Blocked` |

Informational findings never reduce health.

Warnings identify a configuration that may intentionally function but deserves
review. Errors identify an invalid advertised setup. Blockers identify a
condition that prevents trustworthy validation or canonical launch readiness.

### Deterministic report and fingerprints

Validation produces:

- Request fingerprint.
- Evidence fingerprint.
- Report fingerprint.

The report body excludes wall-clock timestamps, random IDs, absolute machine
paths, scene handles, object instance IDs, and nondeterministic collection
ordering.

Unchanged project evidence and the same request must produce the same:

- Health.
- Finding sequence.
- Counts.
- Evidence fingerprint.
- Report fingerprint.
- Copyable text, except for an optional UI-only elapsed-duration display that is
  excluded from report equality and copied evidence.

Findings are ordered by rule order, then project-relative path, then stable
message key.

### Stable validation rules

FL-M5-04 implements and stabilizes these rule IDs:

| Code | Condition | Default severity |
|---|---|---|
| `ELAUNCH-VAL-001` | Canonical Boot scene is missing or not a valid scene asset | Blocker |
| `ELAUNCH-VAL-002` | More than one effective `EchoLaunchRoot` exists across the canonical Boot scene and enabled build scenes, or Boot itself contains multiple roots | Blocker |
| `ELAUNCH-VAL-003` | Canonical root prefab/scene instance is missing its expected configuration or points to a mismatched configuration | Blocker |
| `ELAUNCH-VAL-004` | `EchoLaunchConfiguration` is missing, wrong type, has invalid identity, or uses an unsupported schema | Blocker |
| `ELAUNCH-VAL-005` | Startup sequence is missing, wrong type, contains a null entry/definition, or has invalid identity/schema | Error |
| `ELAUNCH-VAL-006` | Startup sequence or referenced definitions contain duplicate stable IDs | Blocker |
| `ELAUNCH-VAL-007` | Configured final destination is missing, invalid, or not uniquely enabled in Build Settings | Blocker |
| `ELAUNCH-VAL-008` | Canonical Boot scene is missing, disabled, or duplicated in Build Settings | Blocker |
| `ELAUNCH-VAL-009` | Direct-scene helper is unsafe for release | Reserved for FL-M5-05; not emitted by FL-M5-04 |
| `ELAUNCH-VAL-010` | Configured visual splash/status presentation cannot be provided by the verified project root prefab | Warning |
| `ELAUNCH-VAL-011` | Configured splash sequence contains invalid identity, schema, references, or timing | Error |
| `ELAUNCH-VAL-012` | Required startup step uses an unsafe or contradictory failure/timeout policy | Error |
| `ELAUNCH-VAL-013` | Project-owned configuration content resolves inside immutable package source | Error |
| `ELAUNCH-VAL-014` | Required scene, prefab-lineage, serialized, or Build Settings evidence could not be inspected safely | Blocker |
| `ELAUNCH-VAL-015` | A validation run is already active | Warning |

`ELAUNCH-VAL-009` remains reserved because FL-M5-04 must not invent or implement
the direct-scene helper before FL-M5-05 authority exists.

Rules may produce more than one finding with the same code when different paths
or entries fail. Every finding must identify the affected path or authored
entry when possible.

### Rule scope

FL-M5-04 validates:

- Package root-template availability.
- Canonical project folder and asset presence.
- Exact expected Unity asset types.
- Current supported schemas and nonempty stable IDs.
- Launch configuration references.
- Startup sequence entries, referenced definitions, IDs, and approved policy.
- Optional splash sequence identity, references, and timing.
- Project root-prefab lineage, root count, configuration binding, and required
  presentation capability.
- Canonical Boot-scene root count and prefab lineage.
- Canonical Boot Build Settings uniqueness/enabled state.
- Configured destination existence and Build Settings uniqueness/enabled state.
- Duplicate effective roots across enabled build scenes.
- Project-owned content improperly referencing immutable package-owned
  configuration content.

The Validator does not attempt historical migration or infer what a project
author “probably meant.”

### Copyable report

The Validator window shows:

- Project health.
- Information/warning/error/blocker counts.
- Target root.
- Stable fingerprints.
- Findings grouped visibly by severity while preserving deterministic rule
  order.
- A `Copy Report` action.

The copied report is plain text suitable for Git issues, test evidence, and
ChatGPT handoff. JSON export and support-bundle packaging remain later work.

### Re-entry and failure containment

One validation run may be active at a time.

Re-entry returns a structured report containing `ELAUNCH-VAL-015`; it does not
throw an unhandled exception or begin a second scene scan.

Unexpected rule or inspection exceptions are converted into sanitized
`ELAUNCH-VAL-014` findings. One rule failure must not erase findings already
accepted from earlier rules, but evidence whose trustworthiness depends on the
failed inspection must not be reported as healthy.

### Relationship to Setup and future tools

The Validator consumes existing read-only setup evidence where useful, but it
does not make the Setup planner a hidden dependency for every rule.

Setup operations may be suggested as remediation:

```text
Open First Light Setup and refresh the plan.
```

The user still chooses Apply or Repair separately.

FL-M5-05 may add the direct-scene release-safety rule under reserved code
`ELAUNCH-VAL-009` without changing the report schema.

A future build-preflight integration may run the same validation service before
a release build, but FL-M5-04 does not hook build callbacks or block builds.

## Explicitly rejected for FL-M5-04

- Any automatic or one-click fix.
- Applying or repairing from the Validator.
- Schema migration or downgrade.
- Stable-ID regeneration.
- Root deletion or scene cleanup.
- Prefab/scene/asset rewrite.
- Direct-scene initializer implementation.
- Build-preprocess hooks or automatic build blocking.
- Automatic validation on import, domain reload, Play Mode entry, or window
  open.
- Runtime overlay or Observatory bridge.
- JSON/support-bundle export.
- Validation of arbitrary project gameplay systems.
- Asset move, rename, delete, receipt, uninstall, or reset.
- Clean-project distribution or performance claims.

## Consequences

### Positive

- The project gains one truthful read-only health surface.
- Setup creation and repair remain separate, explicit mutation authorities.
- Validation evidence is deterministic and suitable for checkpoint records.
- Scene and Build Settings defects become visible before launch or later
  direct-scene work.
- FL-M5-05 can add direct-scene release safety to an existing rule/report model.

### Costs

- Enabled build scenes require careful additive inspection and state
  restoration.
- Deep sequence/splash/prefab rules add focused Editor tests.
- A dedicated report model duplicates some setup-plan presentation concepts by
  design, preserving ownership separation.
- Some projects will remain `Blocked` until ambiguity is resolved manually.

## Validation obligations

FL-M5-04 must prove:

- Opening or repainting the Validator performs no validation or mutation.
- Validation does not dirty assets, prefabs, scenes, or Build Settings.
- Closed-scene inspection preserves the user's open/active/dirty scene state.
- A canonical healthy foundation produces `Healthy`.
- The same unchanged evidence produces an identical report fingerprint and
  copied report.
- Each implemented code is emitted by at least one focused automated test.
- Multiple findings remain deterministic and defensively copied.
- Duplicate roots across enabled scenes are detected.
- Unsupported schemas and ambiguous/failed evidence block without writes.
- Report formatting contains no absolute machine path or nondeterministic ID.
- Re-entry is contained with `ELAUNCH-VAL-015`.
- Apply and Repair behavior remain unchanged.
- Complete EditMode and Runtime Play Mode suites remain green.
- Manual acceptance proves healthy → intentionally invalid → healthy reporting
  without validator-created Git drift.

## Supersession

This ADR extends EchoLaunch-ADR-004, EchoLaunch-ADR-005, and
EchoLaunch-ADR-006. It does not weaken setup planning, create-only Apply,
explicit Repair, backup/rollback, package independence, runtime immutability, or
migration boundaries.
