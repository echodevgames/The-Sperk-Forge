# FL-M5-07 Standalone Test Laboratory and Importable UPM Sample Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Checkpoint: `FL-M5-07`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- Authority commit: `8ff4109`
- Final implementation commit: `f1665f7`
- Unity: `6000.3.8f1`
- Date: August 8, 2026
- Result: Passed

## Compilation

```text
Errors:   0
Warnings: 0
```

## Automated Tests

```text
Focused package tests: 6 / 6
Focused asset tests:   8 / 8
Project snapshot collector fixture: 14 / 14

Complete EditMode: 306 / 306
Runtime Play Mode: 503 / 503
Total automated:   809 / 809

Failed:   0
Ignored:  0
Errors:   0
Warnings: 0
```

## Imported Sample Regression Finding

Immediately after the first normal Package Manager import, the complete EditMode run fell to:

```text
Passed: 275
Failed:  29
```

The failures belonged to existing Setup integration behavior whose plans were blocked after imported Laboratory assets entered automatic candidate discovery.

The authorized correction excludes standard `Assets/Samples/**` roots from automatic discovery while retaining explicit imported-asset selection.

With the sample still imported after correction:

```text
EditMode: 306
PlayMode: 503
Total:    809
Failures:   0
Errors:     0
Warnings:   0
```

## Serialized Reference Finding

The Laboratory prefab correctly referenced `SuccessConfiguration`, and normal sample import preserved its GUID. The canonical Boot scene nevertheless contained a null prefab-instance `configuration` override, causing the first LAB-001 attempt to block with `ELAUNCH-CFG-001`.

The correction removed the null override and added an asset-test assertion. After fresh reimport, LAB-001 passed.

## LAB-010 Manual Timing Evidence

The initial one-second Laboratory minimum was too short for practical human observation.

The final sample uses a five-second Laboratory-only minimum and a sample-only skip-request control through the existing public uGUI presenter API.

Accepted evidence:

```text
Skip request routed at approximately 3.08s
Minimum display: 5.00s
CanSkipNow at request: False
Splash remained visible after request
Final run state: Completed
Failures: 0
```

## Manual Acceptance

All approved cases `LAB-001` through `LAB-012` passed.

Key evidence:

- canonical Boot success and destination handoff;
- timed progress;
- warning continuation;
- `ELAUNCH-CFG-001` missing-configuration block;
- authored blocking failure;
- `ELAUNCH-ROOT-001` duplicate rejection;
- `ELAUNCH-DEST-001` invalid-destination block;
- Direct Scene development-authority creation;
- Direct Scene existing-authority reuse;
- early-skip minimum-duration enforcement;
- sample removal with package/tooling health;
- repeat-safe Setup and Repair.

## LAB-012 Setup Repeatability

```text
Run 1: Succeeded
Run 2: NoChanges
Run 3: NoChanges

Healthy fingerprint:
7eca14d6390a883417bb0b68cb54a0e2711a93803798d08e099d4cc21750516c
```

Build Settings remained:

```text
0:On:Assets/OutdoorsScene.unity
1:On:Assets/EchoDevGames/FirstLight/Scenes/Boot.unity
```

across no-op repeat runs.

## LAB-012 Repair Repeatability

A controlled defect was introduced by clearing and saving the generated `EchoLaunchConfiguration.StartupSequence` reference.

```text
Run 1: Succeeded
Repaired:
Assets/EchoDevGames/FirstLight/Configuration/EchoLaunchConfiguration.asset

Run 2: NoChanges
Run 3: NoChanges
```

The successful repair restored the existing `StartupSequence.asset` reference, created no duplicate assets, and preserved Build Settings.

## Unity Editor Session-Restore Observation

During LAB-012, Unity repeatedly hung after successful compilation, AssetDatabase refresh, and `OutdoorsScene` load while attempting to restore/open the generated Boot path additively.

Isolation results:

```text
Boot path absent:
Editor opens

Boot path/GUID restored:
Editor hangs

Empty scene contents at same path/GUID:
Editor hangs

Known-good OutdoorsScene contents at same path/GUID:
Editor hangs
```

The observation followed the persisted Boot path/GUID rather than the tested First Light scene contents. It is retained as a separate Unity editor-session/tooling observation.

## Cleanup

- Imported `Assets/Samples` acceptance content removed.
- Generated LAB-012 `Assets/EchoDevGames/FirstLight` content removed.
- Build Settings restored.
- ProjectSettings clean.
- Solution-file drift restored.
- Final implementation staging contained package/test files only.
- `git diff --check` passed before final staging.
- Final implementation repository state was clean after push.

## Conclusion

FL-M5-07 satisfies the approved Standalone Laboratory checkpoint. The UPM sample is importable, removable, reference-complete after the accepted fixes, manually proven across all twelve LAB cases, and regression-clean at `809 / 809`.
