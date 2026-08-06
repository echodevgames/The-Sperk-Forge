# FL-M5-03 — Explicit Setup Repair and Existing-Asset Reconciliation

**Package:** First Light (`EchoLaunch`)
**Specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.9.0
**ADR:** EchoLaunch-ADR-006
**Status:** Complete
**Authority commit:** `6615c8f`
**Implementation commit:** `dd15768`
**Date:** August 6, 2026

## Outcome

First Light now provides a separate explicit Setup Repair transaction for a
narrow set of provable current-schema canonical drift. Create-only Apply remains
create-only. Repair recollects evidence, verifies a fresh equivalent plan,
requires explicit confirmation, secures exact existing-file backups, changes
only approved fields or root presence, writes Build Settings last, and settles
to `NoChanges` when repeated.

## Implemented Scope

- Read-only repair evidence for configuration references and supported schemas
- Destination scene-path and authored-label evidence
- Root-prefab variant lineage, root count, and configuration-binding evidence
- Canonical Boot-scene root-shape and safe-open-state evidence
- Unique Build Settings identity and enabled-state evidence
- `Repair` plan disposition and deterministic repair fingerprints
- Separate immutable repair approval, candidate, change, backup, status, and
  result contracts
- Shared single-active mutation gate across Apply and Repair
- Fresh recollection/replanning before backup or writes
- Exact asset and matching `.meta` backup beneath `Library`
- Hash-verified backup, restore, cleanup, and retained-backup reporting
- Narrow configuration-reference repair
- Narrow destination-scene-path repair with authored-label preservation
- Verified root-prefab configuration-binding repair
- Zero-root canonical Boot-scene repair with unrelated-object preservation
- Missing and uniquely disabled canonical Boot Build Settings repair
- Build Settings mutation after all asset, prefab, and scene repair
- Complete and incomplete rollback result paths
- Deterministic plain-text Repair result and Copy Result
- Stable `ELAUNCH-SETUP-013` through `ELAUNCH-SETUP-017`
- Repeat-safe `NoChanges` result

## Test-Discovered Corrections

The first focused EditMode run found two bounded defects:

1. The backup store checked filesystem availability before validating that the
   requested path belonged beneath project `Assets/`. Validation now occurs
   first, producing the approved ownership rejection.
2. Build Settings repair recorded the Boot scene path as the repaired path.
   The result now records `ProjectSettings/EditorBuildSettings.asset` as its own
   repaired surface.

Both corrections were applied before the final complete EditMode and Runtime
Play Mode gates.

## Manual Acceptance

The accepted scenario generated the canonical foundation beneath:

```text
Assets/EchoDevGames/FirstLight
```

and reused:

```text
Assets/OutdoorsScene.unity
```

Authorized drift was introduced by:

- Clearing the configuration sequence, destination, and splash references.
- Assigning a stale destination scene path while preserving the authored label.
- Clearing the project root-prefab configuration binding.
- Removing the sole Boot root while preserving an unrelated
  `FL_M5_03_UnrelatedMarker` object.
- Removing the canonical Boot Build Settings entry.

The first Repair returned `Succeeded` and repaired exactly five surfaces:

```text
Assets/EchoDevGames/FirstLight/Configuration/EchoLaunchConfiguration.asset
Assets/EchoDevGames/FirstLight/Configuration/LaunchDestination.asset
Assets/EchoDevGames/FirstLight/Prefabs/EchoLaunchRoot.prefab
Assets/EchoDevGames/FirstLight/Scenes/Boot.unity
ProjectSettings/EditorBuildSettings.asset
```

The second and third Repair returned `NoChanges`.

Stable fingerprint:

```text
56526ade68938e38bb6e87fde77d17b6f89329731a813fdf5a36c1a1c57bf77f
```

No rollback or manual recovery path was required.

## Preservation Proof

The accepted repair preserved:

- Configuration asset bytes after convergence, stable IDs, GUIDs, and unrelated
  values
- Project root-prefab identity and canonical content
- Boot-scene GUID
- The unrelated Boot marker object
- Destination scene bytes and GUID
- Package root-template bytes and GUID
- Unrelated Build Settings order and enabled state

Successful temporary repair backups were removed. No generated acceptance asset,
Build Settings drift, solution-file noise, or `Library` backup residue entered
the implementation commit.

## Validation

| Gate | Result |
|---|---|
| Compilation | 0 errors, 0 warnings |
| EditMode | 236 passed, 0 failed, 0 ignored |
| Setup/apply/repair EditMode | 209 passed |
| Prefab EditMode | 27 passed |
| Runtime Play Mode | 479 passed, 0 failed, 0 ignored |
| Total automated | 715 passed |
| First manual Repair | Succeeded |
| Second manual Repair | NoChanges |
| Third manual Repair | NoChanges |
| Retained backup after success | None |
| Manual recovery paths | None |

## Deferred

- Historical schema migration or downgrade
- Stable-ID regeneration
- Type replacement
- Sequence-entry or splash-entry content repair
- Duplicate-root deletion or consolidation
- Arbitrary scene cleanup
- Prefab replacement, rebase, unpack, or structural rewrite
- Move, rename, delete, or relocation tools
- Persistent setup receipts
- Uninstall/reset
- Crash-persistent recovery
- Direct Scene initialization
- Validator and Standalone Laboratory activation
- Player builds, external clean installation, adoption, and performance evidence

## Stop Point

Do not extend FL-M5-03 into migration, cleanup, structural rewriting, receipts,
uninstall, crash recovery, Direct Scene, Validator, Laboratory, or normal scene
travel ownership.

## Next Checkpoint

No next First Light checkpoint is authorized by this closeout. Select and
approve the next bounded M5 outcome through its specification/ADR review and
Checkpoint Build Plan before implementation.
