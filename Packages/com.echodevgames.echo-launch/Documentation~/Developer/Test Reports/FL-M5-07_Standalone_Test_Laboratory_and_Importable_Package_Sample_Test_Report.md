# FL-M5-07 Standalone Test Laboratory and Importable Package Sample Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Checkpoint: `FL-M5-07`
- Authority commit: `741b77d`
- Implementation commit: `583b91a`
- Unity: `6000.3.8f1`
- Date: August 7, 2026
- Result: Passed

## Compilation

```text
Errors:   0
Warnings: 0
```

Compilation passed after initial sample bootstrap, imported-sample candidate
isolation, and the final Direct Scene serialization correction.

## Automated Tests

### Focused Standalone Laboratory EditMode

```text
Passed:   7
Failed:   0
Ignored:  0
```

Accepted focused contracts:

1. Manifest declares exactly one approved sample.
2. Required sample inventory is present.
3. Sample Runtime references only First Light Runtime and approved Unity
   modules.
4. Core assemblies do not reference the sample assembly.
5. Sample source uses no forbidden discovery or friend access.
6. The explicit authoring command is not an automatic callback.
7. The generated Destination scene retains a GUID-backed Direct Scene
   configuration reference.

### Complete EditMode

```text
Passed:   299
Failed:     0
Ignored:    0
```

The increase from the FL-M5-06 baseline is:

- `7` Standalone Laboratory package tests
- `2` imported-sample Setup/Repair candidate-isolation tests

### Complete Runtime PlayMode

```text
Passed:   503
Failed:     0
Ignored:    0
```

### Combined

```text
Total automated: 802
Failed:            0
Ignored:           0
```

## Package Import Evidence

- Package Manager displayed exactly one sample.
- Explicit import succeeded beneath:

  ```text
  Assets/Samples/First Light — Startup and Launch/0.1.0/First Light Standalone Test Lab
  ```

- Import alone did not change Build Settings or ProjectSettings.
- Import alone did not create canonical First Light setup content.
- Generated package and imported payload evidence matched.
- The sample compiled without a peer Sperk's Forge runtime package.

## Manual Acceptance Matrix

| Test | Setup/action | Accepted evidence | Result |
|---|---|---|---|
| `ELAUNCH-LAB-001` | Success configuration; Boot first, Destination second | Completed `2/2`, Destination active, successful handoff/readout | Passed |
| `ELAUNCH-LAB-002` | Timed progress configuration | Visible ordered progress remained responsive through completion | Passed |
| `ELAUNCH-LAB-003` | Warning and recoverable configurations | Completed and handed off with warning evidence retained | Passed |
| `ELAUNCH-LAB-004` | Cleared required configuration | Failed before steps with `ELAUNCH-CFG-001`; reference restored | Passed |
| `ELAUNCH-LAB-005` | Blocking configuration | Failed at step `1/2`; later work stopped; no handoff | Passed |
| `ELAUNCH-LAB-006` | Enabled duplicate Boot root fixture | One accepted authority; `ELAUNCH-ROOT-001`; zero duplicate side effects | Passed |
| `ELAUNCH-LAB-007` | Invalid destination configuration | Preflight failed `0/2` with `ELAUNCH-DEST-001`; no load | Passed |
| `ELAUNCH-LAB-008` | Opened Destination without an existing root | One Direct Scene development authority; no scene reload | Passed |
| `ELAUNCH-LAB-009` | Enabled Destination existing-root fixture | Existing authority reused; no duplicate created | Passed |
| `ELAUNCH-LAB-010` | Requested skip before positive minimum | Minimum remained enforced; exactly one permitted advance | Passed |
| `ELAUNCH-LAB-011` | Deleted imported sample and restored settings | Runtime/Editor package and Setup/Validator/Simulator remained available | Passed |
| `ELAUNCH-LAB-012` | Reimported and repeated Setup/Repair three times | One clean copy, no sample/core coupling, no overwrite or duplicate | Passed |

## Direct Scene Defect and Correction

### Initial failure

The generated and imported Destination scenes initially contained:

```text
directSceneConfiguration: {fileID: 0}
```

Direct play therefore blocked with `ELAUNCH-DIRECT-002` even though the asset
and `.meta` GUID existed.

### Corrected behavior

The authoring command now reloads the persistent configuration asset before
assignment and verifies the expected GUID-backed YAML reference after saving.
The focused regression expanded from `6` to `7` tests and passed.

Final Inspector evidence showed the imported initializer bound to:

```text
LaboratoryDirectSceneConfiguration
```

## Setup/Repair Isolation Evidence

Imported sample definitions and root prefabs are no longer automatic canonical
candidate discoveries. Focused tests cover both asset families.

Manual repeatability evidence retained:

- canonical configuration identities and GUIDs
- canonical root-prefab identity
- canonical Boot-scene identity
- destination scene identity
- package-template identity
- unrelated content and Build Settings baseline
- no retained repair-backup residue

## Removal and Cleanup Evidence

- Imported `Assets/Samples` content was removed before staging.
- Temporary Laboratory authoring content was absent from staging.
- Original Build Settings and ProjectSettings were restored.
- Generated `.slnx` drift was restored.
- Repair-backup residue was removed.
- Package tests passed without an imported sample copy.
- Final implementation status was clean immediately after commit.

## Conclusion

FL-M5-07 passed its focused, full-regression, import, manual Laboratory,
removal, reimport, isolation, and repository-cleanliness gates. The retained
evidence supports package-local standalone MVP completion without claiming
player-build, clean external-install, performance, or adoption results.
