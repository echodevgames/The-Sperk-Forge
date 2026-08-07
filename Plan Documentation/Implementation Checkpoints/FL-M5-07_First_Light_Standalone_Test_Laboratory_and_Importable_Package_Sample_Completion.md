# FL-M5-07 — First Light Standalone Test Laboratory and Importable Package Sample Completion

## Completion Record

- Suite: The Sperk’s Forge — EchoDevGames Game Systems Suite
- Package: First Light (`EchoLaunch`)
- Checkpoint: `FL-M5-07`
- Milestone: M5 closure / standalone MVP evidence before M6
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- ADR: EchoLaunch-ADR-010
- Authority commit: `741b77d`
- Implementation commit: `583b91a`
- Documentation closeout commit: pending
- Date: August 7, 2026
- Status: Complete pending documentation commit and push

## Delivered

FL-M5-07 delivered exactly one explicit Package Manager sample named
`First Light Standalone Test Lab` with:

- isolated public-API sample code
- Boot and Destination scenes
- neutral splash/status presentation
- visible launch and destination evidence
- immediate, timed, warning, recoverable, and blocking steps
- visible pre-authored scenarios
- duplicate-root and Direct Scene fixtures
- explicit Build Settings and scenario instructions
- safe reset, removal, and reimport instructions

Import is user initiated and performs no hidden setup or project-setting
mutation. Imported content is project-owned and removable. No core assembly or
peer package depends on the sample.

## Evidence Summary

```text
Compilation:           0 errors, 0 warnings
Focused Laboratory:    7 passed
Complete EditMode:   299 passed
Runtime PlayMode:    503 passed
Total automated:     802 passed
Laboratory registry:  12 passed
Failed:                 0
Ignored:                0
```

## Acceptance Result

Every `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012` case passed. Accepted proof
includes canonical Boot-to-Destination launch, timed progress, warning and
recoverable continuation, missing configuration, blocking failure, duplicate
authority containment, invalid destination, Direct Scene creation/reuse,
splash minimum timing, sample removal, clean reimport, and repeatable
Setup/Repair separation.

## Defect Findings and Resolutions

The Laboratory exposed two bounded existing-contract defects:

1. Imported sample assets could enter automatic Setup/Repair candidate
   discovery. The collector now excludes First Light Package Manager sample
   roots, with two regression tests.
2. The generated Destination scene could serialize a null Direct Scene
   configuration reference. The authoring command now reloads and verifies the
   persistent GUID-backed asset before scene save, with a focused regression
   test and final imported Inspector proof.

No Runtime core behavior was expanded for either correction.

## Independence and Removal Proof

- Sample Runtime uses public First Light Runtime APIs only.
- No friend access, reflection, or hidden discovery is used.
- No peer Sperk's Forge or project-specific runtime assembly is referenced.
- Core Runtime, Presentation, and Editor assemblies do not reference the sample.
- Removing the imported sample preserved package compilation and tools.
- Reimport restored one clean editable project-owned copy.
- Setup/Repair remained canonical-content-only and repeat-safe.

## Repository Scope

Implementation commit `583b91a` contains the approved sample, manifest change,
package tests, two narrow checkpoint corrections, and matching Unity metadata:

```text
86 files changed
3997 insertions
4 deletions
```

Imported acceptance content, temporary authoring content, Build Settings,
ProjectSettings, repair backups, and generated solution drift were excluded.
The working tree was clean after the implementation commit.

## Deferred Work

- M6 adoption and optional bridges
- automatic sample and Direct Scene installation
- build hooks and player-build evidence
- Git URL, tarball, and separate clean-project installation
- historical schema migration
- receipts, uninstall/reset implementation, and crash-persistent recovery
- persistent-root lifetime policy
- performance and external-adoption evidence

## Next Authority

None.

The next bounded checkpoint requires a new just-in-time learning review and
committed authority. First, commit this closeout, push `main`, confirm
`main == origin/main`, and confirm a clean working tree.
