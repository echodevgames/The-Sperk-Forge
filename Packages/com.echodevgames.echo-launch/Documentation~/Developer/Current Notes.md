# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-07`
- Title: Standalone Test Laboratory and Importable UPM Sample
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- Unity baseline: `6000.3.8f1`
- Status: Implementation and acceptance complete; documentation closeout prepared
- Authority commit: `8ff4109`
- Sample shell implementation: `ff0feff`
- Authored Laboratory assets: `a51c054`
- Imported-sample isolation correction: `02429fb`
- Final acceptance fixes: `f1665f7`

## Final Evidence

```text
Focused package tests: 6 / 6
Focused asset tests:   8 / 8
Complete EditMode:     306 / 306
Runtime Play Mode:     503 / 503
Total automated:       809 / 809
Manual LAB matrix:      12 / 12

Failed:   0
Ignored:  0
Errors:   0
Warnings: 0
```

## Implemented Boundary

First Light now ships exactly one fully authored, removable Unity Package Manager sample named **First Light Standalone Test Lab**.

The sample imports normally, contains authored Boot/destination scenes and scenario assets, and does not automatically mutate Build Settings or invoke Setup/Repair.

Standard imported `Assets/Samples/**` content is excluded from automatic Setup candidate discovery while explicit user selection remains supported.

The canonical Boot scene inherits `SuccessConfiguration` from its Laboratory root prefab. The Laboratory provides a sample-only LAB-010 skip request through the existing uGUI presenter API and a five-second Laboratory-only minimum display for observable manual evidence.

## Repeatability

```text
Setup:  Succeeded, NoChanges, NoChanges
Repair: Succeeded, NoChanges, NoChanges

Healthy fingerprint:
7eca14d6390a883417bb0b68cb54a0e2711a93803798d08e099d4cc21750516c
```

## Tooling Observation

Unity `6000.3.8f1` exhibited a persistent editor-session restore hang while reopening the generated Boot asset path after LAB-012. The hang followed the persisted path/GUID through scene-content substitutions, so the collected evidence does not attribute it to First Light runtime or Laboratory scene contents.

## Cleanup

- Imported sample acceptance copy removed.
- Generated Setup/Repair acceptance content removed.
- Build Settings restored.
- Solution-file drift restored.
- Final package-only regression green.
- Working tree clean after `f1665f7`.

## Next Action

Commit and push the FL-M5-07 documentation closeout. Then select the next First Light checkpoint deliberately.
