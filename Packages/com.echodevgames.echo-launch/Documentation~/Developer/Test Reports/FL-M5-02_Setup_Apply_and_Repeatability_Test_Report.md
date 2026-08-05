# FL-M5-02 — Setup Apply and Repeatability Test Report

**Package:** First Light (`EchoLaunch`)
**Checkpoint:** FL-M5-02
**Unity:** 6000.3.8f1
**Implementation commit:** `f05b95c`
**Date:** August 5, 2026
**Result:** Passed

## Automated Results

### EditMode

| Group | Passed | Failed | Ignored |
|---|---:|---:|---:|
| Setup and apply | 170 | 0 | 0 |
| Presentation prefab | 27 | 0 | 0 |
| **Total** | **197** | **0** | **0** |

### Runtime Play Mode

| Passed | Failed | Ignored |
|---:|---:|---:|
| 479 | 0 | 0 |

### Compilation and Console Gate

| Errors | Warnings |
|---:|---:|
| 0 | 0 |

Expected diagnostic warnings produced inside negative-path tests were contained
and did not count as Test Runner failures or final Console warnings.

## Automated Coverage

- Apply models and result formatting
- Deterministic plan fingerprints
- Freshness recollection and stale-plan rejection
- Single-active-Apply protection
- Authorized disposition enforcement
- Folder and definition creation
- Configuration binding
- Project root prefab variant creation
- Boot scene creation and scene-state preservation
- Build Settings append and approved place-first behavior
- Build Settings order/enabled-state preservation
- Failure injection and compensating rollback
- Rollback-complete and manual-recovery result reporting
- Repeat-safe no-op reruns
- Retained startup presentation prefab identity and wiring
- Retained complete Runtime Play Mode behavior

## Manual Acceptance

### Apply 1

- Status: `Succeeded`
- Rollback completed: `No`
- Manual recovery paths: none
- Existing destination reused: `Assets/OutdoorsScene.unity`
- Project foundation created beneath `Assets/EchoDevGames/FirstLight`
- Boot appended once at Build Settings index `1`

### Apply 2

- Status: `NoChanges`
- Created paths: none
- Build Settings unchanged
- Manual recovery paths: none

### Apply 3

- Status: `NoChanges`
- Created paths: none
- Build Settings unchanged
- Manual recovery paths: none

All three results used the same fingerprint:

```text
7e669d66eaab2c04a0dfbc4445458fcd976808c83f62db82c3d91a16494fc0c1
```

## Acceptance Conclusion

FL-M5-02 satisfies its automated and manual gates. The approved create-only
mutation boundary is repeat-safe and preserves project-owned intent. Repair and
migration evidence remain deliberately outside this report.
