# FL-M5-02 — Approved Setup Apply Engine and Repeat-Safe Asset Creation

**Package:** First Light (`EchoLaunch`)
**Specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.8.0
**ADR:** EchoLaunch-ADR-005
**Status:** Complete
**Authority commit:** `208ee71`
**Implementation commit:** `f05b95c`
**Date:** August 5, 2026

## Outcome

First Light now turns a fresh executable setup plan into a bounded create-only
project mutation. It creates the missing canonical project-owned foundation,
reuses compatible content, preserves existing scene intent, changes Build
Settings only through the approved policy, compensates active-attempt failures,
and becomes a no-op when repeated.

## Implemented Scope

- Immutable apply request, status, change, and result contracts
- Deterministic plan fingerprinting
- Recollection and replanning immediately before writes
- Stale-plan rejection
- Single-active-Apply gate
- Executable `Create`, `Reuse`, and `NoChange` dispositions only
- Folder and ScriptableObject writers
- Configuration reference binding
- Project-owned root prefab variant writer
- Boot scene writer and isolated untitled-scene lease
- Explicit Build Settings writer
- Reverse-order in-memory rollback journal
- Failure injection seams and rollback integration tests
- Plain-text apply-result formatter and Copy Result action
- Repeat-safe `NoChanges` result

## Manual Acceptance

The accepted plan targeted:

```text
Assets/EchoDevGames/FirstLight
```

and reused:

```text
Assets/OutdoorsScene.unity
```

The first Apply returned `Succeeded` and created the configuration assets, root
prefab variant, and Boot scene. Build Settings changed from one enabled
`OutdoorsScene` entry to the same entry plus one enabled Boot scene appended at
index `1`.

The second and third Apply both returned `NoChanges`. No assets or Build
Settings entries were duplicated.

Stable fingerprint:

```text
7e669d66eaab2c04a0dfbc4445458fcd976808c83f62db82c3d91a16494fc0c1
```

Rollback was not required and no manual recovery paths were produced.

## Validation

| Gate | Result |
|---|---|
| Compilation | 0 errors, 0 warnings |
| EditMode | 197 passed, 0 failed, 0 ignored |
| Setup/apply EditMode | 170 passed |
| Prefab EditMode | 27 passed |
| Runtime Play Mode | 479 passed, 0 failed, 0 ignored |
| Total automated | 676 passed |
| First manual Apply | Succeeded |
| Second manual Apply | NoChanges |
| Third manual Apply | NoChanges |

## Commit Hygiene

Generated acceptance assets, the temporary Build Settings mutation, and Unity
solution-file noise were removed or restored before staging. The implementation
commit contains only package Editor setup code, tests, and their Unity metadata.

## Deferred

- Repair and existing-asset reconciliation
- Historical schema migration
- Persistent setup receipts
- Uninstall/reset
- Crash-persistent recovery
- Direct Scene initialization
- Validator and real Standalone Laboratory
- Player builds, external clean install, and performance evidence

## Stop Point

Do not extend this checkpoint into repair, migration, receipts, uninstall,
Direct Scene, Validator, Laboratory, or normal scene-flow ownership.

## Next Candidate

`FL-M5-03 — Explicit Setup Repair and Existing-Asset Reconciliation` is the
tentative next checkpoint and requires its own approved authority before
implementation.
