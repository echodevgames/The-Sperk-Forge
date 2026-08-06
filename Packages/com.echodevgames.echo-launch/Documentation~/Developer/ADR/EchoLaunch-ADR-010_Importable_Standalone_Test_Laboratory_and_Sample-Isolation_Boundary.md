# EchoLaunch-ADR-010 — Importable Standalone Test Laboratory and Sample-Isolation Boundary

## Status

Accepted

## Date

August 6, 2026

## Decision Owners

Jesse “Echo” Adams / EchoDevGames

## Related Authority

- SFGSS-000 — Echo Game Systems Suite Bible
- SFGSS-001 — Package Specification Template
- SFGSS-002 — Assembly and Dependency Rules
- SFGSS-004 — Evidence and Compatibility Rules
- SFGSS-005 — Checkpoint Build Plan
- SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- FL-M5-07 Standalone Test Laboratory and Importable Package Sample Plan

## Context

First Light's Runtime, presentation, setup, validation, Direct Scene, and
Simulator boundaries are implemented and closed through FL-M5-06.

The package still lacks the standalone visible proof required by its MVP
specification:

- canonical Boot-to-destination launch
- visible splash and status presentation
- immediate and timed startup steps
- warning and recoverable-failure continuation
- blocking failure
- duplicate root protection
- invalid configuration/destination behavior
- Direct Scene creation and reuse
- splash minimum-duration skip behavior
- sample removal
- setup/repair repeatability

M6 is adoption/integration work. Beginning an external adapter or bridge
before package-local standalone proof would blur whether success belongs to
First Light itself or to an integration.

Unity Package Manager supports package samples as explicitly imported
content. That model gives First Light a teaching and proof environment
without making examples part of production Runtime assemblies.

## Problem

The Laboratory must be complete enough to prove the MVP, isolated from peer
packages, removable, importable without hidden mutation, representative of
public consumer usage, and truthful about its evidence.

A scene embedded as production content would blur example and runtime
authority. An automatic installer would repeat the hidden mutation risks
First Light's setup architecture rejects. An internal-only sample would prove
less than a real consumer-facing sample using public APIs.

## Decision

### 1. Checkpoint identity

The next bounded checkpoint is:

```text
FL-M5-07
Standalone Test Laboratory and Importable Package Sample
```

It closes deferred standalone MVP evidence before M6. It does not renumber
completed checkpoints.

### 2. One explicit UPM sample

The package ships exactly one sample:

```text
Samples~/First Light Standalone Test Lab
```

`package.json` declares one stable sample entry with display name:

```text
First Light Standalone Test Lab
```

Import is an explicit user action through Package Manager.

### 3. Imported content is project-owned

After import, the sample copy lives in the project Assets tree and is:

- editable
- removable
- reimportable
- outside core package authority
- allowed to contain project-owned configuration assets and fixtures

The package does not silently synchronize imported sample content later.

### 4. Import performs no hidden setup

Import does not automatically:

- modify Build Settings
- run Setup, Apply, Repair, or Validator
- create canonical setup folders/assets
- modify ProjectSettings
- add scripting defines
- open or save scenes
- claim a root
- enter Play Mode
- install a bridge
- rewrite unrelated project content

Required setup is documented and explicit.

### 5. Two-scene default

The default scene set is:

```text
FirstLight_Boot_Lab.unity
FirstLight_Destination_Lab.unity
```

Destination also demonstrates Direct Scene development through an explicit
`EchoDirectSceneInitializer`.

A third Direct Scene scene may be added only if implementation evidence proves
the two-scene design insufficient.

### 6. Visible pre-authored scenarios

Scenario choice is represented by visible pre-authored configurations or
scene fixtures selected before Play.

The Laboratory does not mutate ScriptableObject assets during Play merely to
turn one scenario into another.

Approved sample steps:

- immediate success
- timed progress
- warning
- recoverable failure
- blocking failure

Sample-authored warnings/failures use Laboratory-specific diagnostics rather
than reusing production codes for new meanings.

### 7. Public API only

Sample runtime code lives in a sample-owned assembly and references only
public First Light Runtime APIs.

It receives no friend access to internals and uses no reflection or hidden
discovery.

Core Runtime, Presentation, and Editor assemblies do not reference sample
code or content.

### 8. No peer-package dependency

The Laboratory contains no runtime reference to:

- Jukebot / Resonance
- EchoUI / Looking Glass
- EchoSave
- EchoSettings
- EchoSceneFlow / Passage
- EchoGameState
- EchoInput
- EchoDiagnostics / Observatory
- project-specific assemblies

Optional integration samples remain M6-or-later work.

### 9. Package-qualified evidence IDs

The authoritative Laboratory registry is:

```text
ELAUNCH-LAB-001 through ELAUNCH-LAB-012
```

Historical `LAB-001` shorthand remains documentation history only.

### 10. Temporary authoring workspace

Serialized scenes/assets must be created and validated through Unity.

FL-M5-07 may use a temporary project-owned authoring workspace and an
imported acceptance copy. Both are test/build residue and must be removed
before implementation staging.

Final staged scope may include only:

- `package.json`
- approved package-level tests
- the distribution sample payload
- narrowly necessary checkpoint-owned fixes
- required Unity metadata for the chosen supported layout

### 11. Sample removal is a first-class gate

Deleting the imported sample must leave:

- Runtime compilation intact
- Editor compilation intact
- Setup available
- Validator available
- Simulator available
- package tests independent from imported sample paths

Reimport must restore one clean copy without duplicate package content.

### 12. Existing-contract defect rule

If Laboratory testing exposes a defect in an already approved First Light
contract, FL-M5-07 may include only the smallest correction needed to make
that approved behavior true.

It requires focused evidence, full regression, closeout documentation, and no
unrelated refactor.

## Consequences

### Positive

- Models real consumer usage.
- Proves package independence visibly.
- Proves Boot and Direct Scene behavior without integration.
- Provides an editable learning environment.
- Removal proves examples are not production dependencies.
- Gives M6 a known standalone baseline.

### Costs

- Serialized sample content requires disciplined Unity authoring.
- Import, scene-list, removal, and reimport evidence are required.
- The Laboratory needs sample-only code and documentation.
- Build Settings cannot be assumed after import.

### Risks

- Serialized references may break during packaging/import.
- Imported paths may vary by display name/version.
- Sample code may accidentally depend on internals or project assemblies.
- Test workspaces may leak into staging.
- Users may mistake sample assets for canonical setup.

### Mitigations

- Validate the exact Package Manager import route.
- Use public APIs and explicit references only.
- Add manifest/inventory/dependency tests.
- Document imported-path discovery rather than machine paths.
- Restore all test residue before staging.
- Label every sample asset/scene as Laboratory content.
- Run removal and reimport gates.

## Rejected Alternatives

### Automatic sample installer

Rejected because it would mutate the project before review and duplicate
setup authority.

### Laboratory under production Runtime/Editor folders

Rejected because it would blur removable examples with production content.

### First adoption before standalone Laboratory

Rejected because integration success cannot replace isolated proof.

### Internal-only sample

Rejected because consumers must reproduce it through supported public APIs.

### One Play-time-mutated universal configuration

Rejected because it would dirty/rewrite authored assets and obscure scenario
provenance.

### Separate Direct Scene by default

Rejected until Destination proves insufficient.

## Validation

This ADR is satisfied only when:

- exactly one sample appears in Package Manager
- explicit import succeeds
- required assets/scenes are present
- `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012` are executed
- removal preserves package compilation/tools
- reimport succeeds
- complete regressions pass
- no test residue enters the implementation commit

## Deferred

- M6 adoption and optional bridges
- player-build and external-adoption claims
- report export
- build hooks
- migration
- receipts
- uninstall/reset implementation
- crash-persistent recovery
- performance claims
