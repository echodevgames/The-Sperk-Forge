# First Light - Current Notes

## Latest Completed Checkpoint

- Checkpoint: `FL-M5-06`
- Title: Launch Simulator and Deterministic Failure Injection
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.12.0
- ADR: EchoLaunch-ADR-009
- Authority commit: `a159349`
- Implementation commit: `956c381`
- Documentation closeout: `e28ff09`
- Status: Complete, documented, pushed, and clean
- Compilation: `0` errors, `0` warnings
- Focused Simulator EditMode: `24` passed
- Complete EditMode: `290` passed
- Runtime Play Mode: `503` passed
- Total automated: `793` passed

## Active Checkpoint

- Checkpoint: `FL-M5-07`
- Title: Standalone Test Laboratory and Importable Package Sample
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- ADR: EchoLaunch-ADR-010
- Authority baseline: `e28ff09`
- Status: Authority prepared; implementation locked
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `290` passed
- Runtime Play Mode baseline: `503` passed
- Total automated baseline: `793` passed

## Approved Outcome

One importable, package-owned **First Light Standalone Test Lab** proves the
complete First Light MVP launch loop without another Sperk's Forge package or
a project-specific runtime assembly.

The sample provides:

- `FirstLight_Boot_Lab.unity`
- `FirstLight_Destination_Lab.unity`
- visible startup status and destination evidence
- project-owned sample configurations and sequences after import
- immediate, timed-progress, warning, recoverable-failure, and blocking
  sample steps
- direct-scene development proof in the destination scene
- duplicate-root, missing-configuration, invalid-destination, splash-skip,
  sample-removal, and setup/repair-repeatability instructions
- package-qualified `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012` evidence

## Approved Boundary

- The Laboratory is one explicit UPM sample declared by `package.json`.
- Import is user initiated through Package Manager.
- Import does not automatically edit Build Settings, ProjectSettings, scenes,
  canonical setup assets, or scripting defines.
- Imported content is project-owned and removable.
- Core Runtime and Editor assemblies never depend on the sample.
- Sample code uses only public First Light APIs.
- No peer Sperk's Forge runtime package is referenced.
- The Destination scene doubles as the Direct Scene Lab unless implementation
  proves a third scene is necessary.
- Scenario selection uses visible pre-authored configurations; no hidden
  runtime rewriting of ScriptableObject assets.
- A temporary authoring workspace may be used to create and validate
  serialized content but must be removed before staging.
- M6 adoption, bridges, player-build claims, migration, receipts, uninstall,
  recovery, report export, and build hooks remain unauthorized.

## Next Action

Commit and push:

```text
Approve FL-M5-07 standalone laboratory authority
```

Implementation may begin only after that authority commit.
