# FL-M5-07 — Standalone Test Laboratory and Importable Package Sample

## Status

- Checkpoint: `FL-M5-07`
- Status: Complete pending documentation commit and push
- Package: First Light (`EchoLaunch`)
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- ADR: EchoLaunch-ADR-010
- Authority commit: `741b77d`
- Implementation commit: `583b91a`
- Unity baseline: `6000.3.8f1`
- Completion date: August 7, 2026

## Purpose

Provide one explicit importable First Light sample that proves the visible MVP
launch loop in isolation, teaches supported public consumer usage, and remains
removable without becoming a dependency of production package assemblies or
Editor tools.

## Delivered Surface

- One `package.json` sample declaration
- Display name `First Light Standalone Test Lab`
- `FirstLight_Boot_Lab.unity`
- `FirstLight_Destination_Lab.unity`
- Neutral splash and plain status presentation
- Visible destination/report readout
- Immediate-success step
- Timed-progress step
- Warning step
- Recoverable-failure step
- Blocking-failure step
- Pre-authored scenario configurations and sequences
- Missing-configuration and invalid-destination proof
- Duplicate-root fixture
- Direct Scene creation and existing-authority-reuse fixtures
- Explicit optional Laboratory authoring command
- Reset, removal, and reimport guide

## Isolation Result

The Laboratory is a UPM sample, not production Runtime content.

- Import is explicit and user initiated.
- Imported content is project-owned, editable, removable, and reimportable.
- Import does not change Build Settings, ProjectSettings, canonical setup
  assets, scripting defines, open scenes, or Play Mode.
- Sample Runtime code uses only public First Light APIs.
- Sample assemblies receive no friend access.
- Sample code uses no reflection or hidden discovery.
- No peer Sperk's Forge or project-specific runtime assembly is referenced.
- Core Runtime, Presentation, and Editor assemblies do not reference sample
  code or content.
- Package tests do not require an imported copy beneath `Assets/Samples`.

## Narrow Existing-Contract Corrections

### Imported sample candidate isolation

The Laboratory exposed that Setup/Repair snapshot discovery could consider an
imported First Light sample definition or root prefab as an automatic canonical
candidate. The collector now excludes First Light Package Manager sample roots.

Two focused tests retain this boundary:

- imported sample definitions are not automatic candidates
- imported sample root prefabs are not automatic candidates

### Direct Scene serialization

The first generated Destination scene serialized its Direct Scene configuration
as `{fileID: 0}` because the newly created asset was assigned before Unity
reloaded its persistent identity.

The accepted correction:

- saves and synchronously imports the new asset
- reloads the persistent `DirectSceneConfiguration`
- assigns that persistent object to the initializer
- marks and saves the scene
- verifies the expected GUID-backed YAML reference
- aborts authoring if persistence cannot be proven

The package and imported generated payloads matched after the correction, and
the final imported Destination initializer showed
`LaboratoryDirectSceneConfiguration` rather than `None`.

## Automated Evidence

```text
Compilation:             0 errors, 0 warnings
Focused Laboratory:      7 passed
Complete EditMode:     299 passed
Runtime PlayMode:      503 passed
Total automated:       802 passed
Failed:                  0
Ignored:                 0
```

## Manual Acceptance

All package-qualified Laboratory cases passed:

| ID | Accepted outcome |
|---|---|
| `ELAUNCH-LAB-001` | Canonical Boot run completed, reported success, and activated Destination. |
| `ELAUNCH-LAB-002` | Timed progress remained visible and ordered. |
| `ELAUNCH-LAB-003` | Warning and recoverable-failure scenarios continued with warning evidence. |
| `ELAUNCH-LAB-004` | Missing configuration blocked before steps with `ELAUNCH-CFG-001`. |
| `ELAUNCH-LAB-005` | Blocking failure stopped later work and prevented destination handoff. |
| `ELAUNCH-LAB-006` | Duplicate root emitted `ELAUNCH-ROOT-001` with one authority and no duplicate side effects. |
| `ELAUNCH-LAB-007` | Invalid destination blocked with `ELAUNCH-DEST-001`. |
| `ELAUNCH-LAB-008` | Destination direct play created one development authority without reloading the active scene. |
| `ELAUNCH-LAB-009` | Destination direct play reused an existing authority without creating a duplicate. |
| `ELAUNCH-LAB-010` | Early skip respected positive minimum splash duration and advanced once when permitted. |
| `ELAUNCH-LAB-011` | Removing the imported sample preserved Runtime/Editor compilation and package tools. |
| `ELAUNCH-LAB-012` | Clean reimport plus three-run Setup/Repair checks preserved identities, separation, and repeatability. |

## Repository Evidence

Implementation commit `583b91a` contains `86` changed files with `3997`
insertions and `4` deletions. Its scope is limited to:

- `package.json`
- one package sample and Unity metadata
- package-level Laboratory tests
- imported-sample Setup/Repair candidate isolation and tests
- Direct Scene sample serialization correction and regression test

Before commit, the imported acceptance copy, temporary authoring content,
repair-backup residue, Build Settings changes, ProjectSettings changes, and
generated solution drift were removed or restored. The working tree was clean
immediately after implementation commit.

## Deferred

- M6 adoption and optional bridges
- automatic sample installation
- automatic Direct Scene installation and build hooks
- migration, receipts, uninstall/reset implementation, and recovery
- persistent-root lifetime policy
- player builds and automatic production-startup evidence
- Git URL, tarball, and separate clean-project installation
- external adoption and performance evidence

## Completion

FL-M5-07 satisfies EchoLaunch-ADR-010 and its approved SFGSS-005 Checkpoint
Build Plan. The package-local MVP now has isolated visible Laboratory evidence.
The checkpoint becomes fully closed when this documentation is committed and
`main` is pushed and synchronized with `origin/main`.
