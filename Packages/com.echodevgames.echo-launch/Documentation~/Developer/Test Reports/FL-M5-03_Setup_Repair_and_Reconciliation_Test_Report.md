# FL-M5-03 — Setup Repair and Reconciliation Test Report

**Package:** First Light (`EchoLaunch`)
**Checkpoint:** FL-M5-03
**Unity:** 6000.3.8f1
**Implementation commit:** `dd15768`
**Date:** August 6, 2026
**Result:** Passed

## Automated Results

### EditMode

| Group | Passed | Failed | Ignored |
|---|---:|---:|---:|
| Setup, apply, and repair | 209 | 0 | 0 |
| Presentation prefab | 27 | 0 | 0 |
| **Total** | **236** | **0** | **0** |

### Runtime Play Mode

| Passed | Failed | Ignored |
|---:|---:|---:|
| 479 | 0 | 0 |

### Compilation and Console Gate

| Errors | Warnings |
|---:|---:|
| 0 | 0 |

Expected diagnostic logs produced inside negative-path tests were contained and
did not count as final Test Runner failures or final Console warnings.

## Focused Test Iteration

The first focused EditMode run discovered `236` tests and completed with:

```text
Passed: 234
Failed: 2
Ignored: 0
Errors: 0
Warnings: 0
```

The failures identified:

- Project path ownership validation occurred after filesystem lookup in the
  backup store.
- Build Settings repair result reporting reused the Boot scene path, causing
  deduplication of one repaired surface.

Both defects were corrected. The complete EditMode suite then passed all `236`
tests, and the retained Runtime Play Mode suite passed all `479` tests.

## Automated Coverage

- Immutable repair approval, candidate, change, backup, status, and result values
- Defensive copying and deterministic result formatting
- Read-only repair evidence collection
- Current-schema and exact-type eligibility
- Stable-ID validation
- Unique canonical dependency resolution
- Root-prefab variant lineage and root-count proof
- Canonical Boot-scene shape and safe-open-state proof
- Unique Build Settings identity and enabled-state proof
- Deterministic repair fingerprints and enumeration-order independence
- Stale-plan rejection before backup or writes
- Separate Apply and Repair authorization
- Shared single-active mutation gate
- Project-owned path rejection before filesystem lookup
- Exact asset and `.meta` backup
- Backup hash verification and successful cleanup
- Exact restore and retained-backup reporting
- Narrow configuration and destination writes
- Narrow root-prefab configuration binding
- Zero-root Boot-scene repair
- Unrelated scene-object preservation
- Missing and uniquely disabled Build Settings repair
- Build Settings last
- Complete rollback
- Incomplete rollback with retained backup
- Mixed create-plus-repair partial foundation
- Second and third repair no-op settlement
- Package template and selected destination preservation
- Retained complete Runtime Play Mode behavior

## Manual Acceptance

### Repair 1

- Status: `Succeeded`
- Rollback completed: `No`
- Backup directory retained: none
- Manual recovery paths: none
- Created paths: none
- Repaired paths: five
- Existing destination reused: `Assets/OutdoorsScene.unity`
- Boot Build Settings entry restored at index `1`
- Unrelated Boot marker object preserved

### Repair 2

- Status: `NoChanges`
- Created paths: none
- Repaired paths: none
- Build Settings unchanged
- Manual recovery paths: none

### Repair 3

- Status: `NoChanges`
- Created paths: none
- Repaired paths: none
- Build Settings unchanged
- Manual recovery paths: none

All three results used the same canonical fingerprint:

```text
56526ade68938e38bb6e87fde77d17b6f89329731a813fdf5a36c1a1c57bf77f
```

## Identity and Preservation Checks

Byte or metadata comparison confirmed:

- Configuration assets, stable IDs, GUIDs, and unrelated values unchanged after
  convergence
- Project root-prefab identity and content restored exactly
- Boot-scene GUID unchanged
- `OutdoorsScene` and its GUID untouched
- Package root template and its GUID untouched
- Build Settings matched the canonical accepted baseline

The successful repair left no `Library/EchoDevGames/FirstLight/RepairBackups`
content. Generated acceptance assets and temporary Build Settings changes were
removed before staging.

## Acceptance Conclusion

FL-M5-03 satisfies its automated and manual gates. The separate repair
transaction modifies only the approved current-schema surfaces after proof,
approval, and backup; preserves unrelated project intent and identities; and
settles deterministically to `NoChanges` on repeat. Migration and broader repair
remain outside this report.
