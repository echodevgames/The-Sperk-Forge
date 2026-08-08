# FL-M5-07 — First Light Standalone Test Laboratory and Importable UPM Sample Completion

## Completion Summary

- Package: First Light (`EchoLaunch`)
- Checkpoint: `FL-M5-07`
- Status: Complete
- Specification authority: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- Unity baseline: `6000.3.8f1`
- Authority commit: `8ff4109`
- Implementation commits: `ff0feff`, `a51c054`, `02429fb`, `f1665f7`
- Completion date: August 8, 2026

## Delivered

FL-M5-07 delivers exactly one fully authored Unity Package Manager sample:

```text
First Light Standalone Test Lab
```

The sample proves the complete First Light MVP launch loop using the established runtime, presentation, Direct Scene, Setup/Repair, Validator, and Simulator contracts without creating a second authority or setup system.

## Conditional Production Correction Exercised

The checkpoint's pre-authorized imported-sample isolation correction was required.

Normal sample import caused standard `Assets/Samples/**` content to enter automatic Setup candidate discovery. The correction excludes standard imported sample roots from automatic discovery while preserving explicit selection and without widening Setup mutation authority.

## Acceptance Corrections

Manual acceptance also exposed and resolved:

1. a null Boot-scene root-configuration override;
2. the absence of a practical manual LAB-010 skip request;
3. a Laboratory-only one-second splash minimum that was too short for reliable manual timing evidence.

No package Runtime authority or production input ownership changed.

## Final Evidence

```text
Focused package tests: 6 / 6
Focused asset tests:   8 / 8
Complete EditMode:     306 / 306
Runtime Play Mode:     503 / 503
Total automated:       809 / 809
Manual LAB matrix:      12 / 12

Failures: 0
Ignored:  0
Errors:   0
Warnings: 0
```

## Repeatability

```text
Setup:
Succeeded
NoChanges
NoChanges

Repair:
Succeeded
NoChanges
NoChanges

Healthy fingerprint:
7eca14d6390a883417bb0b68cb54a0e2711a93803798d08e099d4cc21750516c
```

No duplicate root, configuration, Boot scene, generated asset, or Build Settings entry was produced by repeat runs.

## Tooling Observation

A Unity `6000.3.8f1` editor-session restore hang was isolated during LAB-012. The available evidence showed that the hang followed the persisted generated Boot asset path/GUID during editor startup even when scene contents were replaced with empty or known-good scene contents.

This observation did not block Setup/Repair repeatability and is not attributed to First Light runtime or Laboratory scene-content behavior by the evidence collected.

## Repository Cleanup

The checkpoint closed with:

- imported sample acceptance content removed;
- generated Setup/Repair acceptance content removed;
- Build Settings restored;
- ProjectSettings clean;
- solution-file drift restored;
- package-only regression green;
- implementation commit `f1665f7` pushed;
- working tree clean before documentation closeout.

## Result

FL-M5-07 is complete and ready for documentation closeout commit.

Per the approved checkpoint stop point, do not automatically continue into release packaging, beta versioning, external adoption, report export, migration, receipts, uninstall/recovery, or another package. Select the next checkpoint deliberately after this closeout is committed and synchronized.
