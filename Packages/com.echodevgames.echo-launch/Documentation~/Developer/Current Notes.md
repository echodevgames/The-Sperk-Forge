# First Light - Current Notes

## Completed Checkpoint

- Checkpoint: `FL-M5-02`
- Title: Approved Setup Apply Engine and Repeat-Safe Asset Creation
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.8.0
- ADR: EchoLaunch-ADR-005
- Authority commit: `208ee71`
- Implementation commit: `f05b95c`
- Status: Implemented, tested, manually accepted, and pushed

## Implemented Boundary

The Setup window can now apply one fresh executable setup plan.

- Recollect and replan before writes
- Deterministic plan fingerprint
- Single active Apply
- Executable `Create`, `Reuse`, and `NoChange` dispositions only
- Create-only project folders and definition assets
- Configuration reference binding
- Project-owned root prefab variant
- Boot scene creation without changing the destination scene
- Explicit Build Settings policy
- Build Settings write last
- In-memory compensating rollback
- Immutable apply result and plain-text formatter
- Second and third Apply settle as `NoChanges`

## Validation

- EditMode: `197` passed, `0` failed, `0` ignored
  - Setup and apply: `170`
  - Presentation prefab: `27`
- Runtime Play Mode: `479` passed, `0` failed, `0` ignored
- Compilation: `0` errors, `0` warnings
- Manual Apply sequence: `Succeeded`, `NoChanges`, `NoChanges`
- Stable fingerprint: `7e669d66eaab2c04a0dfbc4445458fcd976808c83f62db82c3d91a16494fc0c1`
- Existing destination reused: `Assets/OutdoorsScene.unity`
- Boot appended once: `Assets/EchoDevGames/FirstLight/Scenes/Boot.unity`
- Rollback required: no
- Manual recovery paths: none

## Commit Scope

The implementation commit contains package Editor setup code, EditMode tests,
and Unity metadata only. Generated acceptance assets, Build Settings drift, and
solution-file noise were removed before staging.

## Deferred

Repair, migration, persistent receipts, uninstall/reset, crash-persistent
recovery, Direct Scene, Validator, Laboratory, builds, clean/external adoption,
and performance evidence.

## Next Action

Commit and push the reconciled documentation:

```text
echo-launch: document FL-M5-02 completion
```

Tentative next checkpoint: `FL-M5-03 — Explicit Setup Repair and Existing-Asset Reconciliation`.
It is not authorized until its specification/ADR/checkpoint authority is approved.
