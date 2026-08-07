# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-07`
- Title: Standalone Test Laboratory and Importable UPM Sample
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- Starting implementation baseline: FL-M5-06 closeout commit `e28ff09`, followed only by bounded living-note drift reconciliation
- Status: Authorized; implementation not yet started
- Compilation baseline: `0` errors, `0` warnings
- Complete EditMode baseline: `290` passed
- Runtime Play Mode baseline: `503` passed
- Total automated baseline: `793` passed

## Latest Completed Checkpoint

- Checkpoint: `FL-M5-06`
- Title: Launch Simulator and Deterministic Failure Injection
- Authority commit: `a159349`
- Implementation commit: `956c381`
- Documentation closeout: `e28ff09`
- ADR: EchoLaunch-ADR-009
- Status: Complete

## FL-M5-07 Authorized Outcome

First Light will gain exactly one separately importable UPM sample named
**First Light Standalone Test Lab**. The sample proves the complete MVP launch
loop using existing production contracts rather than adding another launch or
setup pipeline.

The shipped sample is fully authored beneath:

```text
Samples~/First Light Standalone Test Lab/
```

Unity Package Manager import is the only normal installation action. Importing
the sample must not run a generator, mutate Build Settings, invoke Setup or
Repair, launch the Simulator, enter Play Mode, or edit unrelated project data.

## Sample Isolation Boundary

- No other Sperk's Forge runtime package.
- No project-specific runtime assembly.
- Sample-only executable helpers live in a narrow sample assembly.
- Sample definitions/configurations/scenes/prefabs are already serialized and reference-complete before distribution.
- The sample may depend only on EchoLaunch and its declared Unity dependency surface.
- The imported sample becomes removable project content under Unity's normal `Assets/Samples/...` location.
- Removing that imported sample must leave EchoLaunch Runtime and Editor assemblies healthy.
- Existing production contracts remain unchanged unless the Laboratory proves a reproducible defect.

## Required Acceptance Cases

`LAB-001` through `LAB-012` remain exactly the approved cases in the package
specification. Manual evidence is paired with existing or new automated proof
where practical.

## Conditional Setup Isolation Correction

If and only if evidence shows imported `Assets/Samples/**` scenes are being
included as automatic Setup candidates, FL-M5-07 may add a narrow exclusion for
standard imported-sample roots. Explicit project selection must remain possible,
and the change requires regression tests. No broader Setup behavior change is
authorized.

## Explicitly Not Authorized

- Shipped sample generator/authoring engine.
- Automatic scene or configuration generation after import.
- Import-time Build Settings mutation.
- Automatic Simulator/Validator/Setup execution.
- New runtime authority or launch pipeline.
- Schema migration, receipts, uninstall, recovery, or build hooks.
- Peer-package integration.

## Next Action

Commit and push the FL-M5-07 authority documents, then begin implementation with
the package manifest/sample declaration and fully-authored sample source only.
