# First Light - Current Notes

## Latest Completed Checkpoint

- Checkpoint: `FL-M5-07`
- Title: Standalone Test Laboratory and Importable Package Sample
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- ADR: EchoLaunch-ADR-010
- Authority commit: `741b77d`
- Implementation commit: `583b91a`
- Documentation closeout: pending
- Status: Complete pending documentation commit and push
- Compilation: `0` errors, `0` warnings
- Focused Laboratory EditMode: `7` passed
- Complete EditMode: `299` passed
- Runtime Play Mode: `503` passed
- Total automated: `802` passed, `0` failed, `0` ignored

## Implemented Laboratory

The package declares exactly one explicit UPM sample:

```text
First Light Standalone Test Lab
```

The distribution sample provides:

- `FirstLight_Boot_Lab.unity`
- `FirstLight_Destination_Lab.unity`
- visible startup and destination readout
- immediate, timed-progress, warning, recoverable-failure, and blocking steps
- success, warning, recoverable, blocking, and invalid-destination
  configurations
- duplicate-root and existing-authority fixtures
- a GUID-backed Direct Scene configuration reference
- neutral status and splash presentation
- explicit Build Settings, scenario, reset, removal, and reimport instructions

## Isolation Boundary

- Import is explicit and user initiated through Package Manager.
- Imported content is project-owned, editable, removable, and reimportable.
- Import does not automatically mutate Build Settings or ProjectSettings.
- Sample Runtime code uses public First Light APIs only.
- No friend access, reflection, hidden discovery, peer package, or
  project-specific runtime dependency is present.
- Core Runtime, Presentation, and Editor assemblies do not reference the
  sample assembly or content.
- Setup and Repair ignore imported sample definitions and root prefabs during
  automatic candidate discovery.
- The optional Laboratory authoring command is explicit and does not run on
  import, reload, repaint, or Play Mode entry.

## Accepted Laboratory Registry

| ID | Accepted result |
|---|---|
| `ELAUNCH-LAB-001` | Canonical Boot launch completed and activated Destination. |
| `ELAUNCH-LAB-002` | Timed progress remained visible, ordered, and responsive. |
| `ELAUNCH-LAB-003` | Warning and recoverable-failure fixtures continued successfully with warning evidence. |
| `ELAUNCH-LAB-004` | Missing configuration blocked before steps with `ELAUNCH-CFG-001`. |
| `ELAUNCH-LAB-005` | Blocking failure stopped later work and prevented handoff. |
| `ELAUNCH-LAB-006` | Duplicate root emitted `ELAUNCH-ROOT-001` with one authority and no duplicate side effects. |
| `ELAUNCH-LAB-007` | Invalid destination blocked with `ELAUNCH-DEST-001`. |
| `ELAUNCH-LAB-008` | Destination direct play created one development authority without scene reload. |
| `ELAUNCH-LAB-009` | Destination direct play reused an existing authority without duplication. |
| `ELAUNCH-LAB-010` | Early splash skip respected minimum display timing and advanced once when permitted. |
| `ELAUNCH-LAB-011` | Removing the imported sample preserved package compilation and Editor tools. |
| `ELAUNCH-LAB-012` | Reimport and three-run Setup/Repair checks preserved separation, identities, and repeatability. |

## Narrow Corrections

FL-M5-07 exposed and corrected two existing-contract defects:

1. Imported sample assets could be considered automatic Setup/Repair
   candidates. The snapshot collector now excludes First Light Package
   Manager sample roots.
2. A newly created Direct Scene configuration could be assigned before Unity
   reloaded its persistent asset identity, leaving the generated scene with a
   null serialized reference. The authoring tool now saves/imports/reloads and
   verifies the asset before scene save.

## Repository State

Implementation commit `583b91a` contains only the approved sample,
`package.json`, package-level tests, the two narrow corrections, and matching
Unity metadata. Imported acceptance content, temporary authoring content,
Build Settings, ProjectSettings, and generated solution drift were removed
before commit.

## Deferred

- M6 project adoption and optional bridges
- automatic sample installation
- historical schema migration
- receipts, uninstall/reset implementation, and crash-persistent recovery
- automatic Direct Scene installation and build hooks
- player builds, clean external install, performance, and adoption claims
- persistent-root lifetime policy

## Next Action

Commit the FL-M5-07 documentation closeout and push `main` so authority,
implementation, and closeout are synchronized with `origin/main`.
