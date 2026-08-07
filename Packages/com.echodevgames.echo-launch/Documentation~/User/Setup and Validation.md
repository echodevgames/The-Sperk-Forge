# First Light Setup and Validation

This guide explains the implemented project-owned setup path for First Light
version `0.1.0`. It applies to the Unity `6000.3.8f1` development baseline.

## Before opening Setup

1. Save the gameplay/menu scene that First Light should activate after startup.
2. Add that destination scene exactly once and enabled in **File > Build
   Profiles > Scene List**. If the active profile has no Scene List override,
   Unity uses the global scene list.
3. Confirm the Console has no compile errors.
4. Keep unrelated dirty scenes saved or clearly identified.

First Light Setup validates an existing destination scene. It does not create,
open, edit, or add that destination to the Scene List.

## Open Setup

Use:

```text
Tools > Sperk's Forge > First Light > Setup
```

The default project root is:

```text
Assets/EchoDevGames/FirstLight
```

The default Boot scene path is:

```text
Assets/EchoDevGames/FirstLight/Scenes/Boot.unity
```

Assign the saved destination scene. Enable **Create Splash Sequence** only when
you want an initially empty project-owned splash asset.

## Build Settings Policy

| Policy | Behavior |
|---|---|
| `DoNotChange` | Creates/reuses project content without changing the Scene List. |
| `AddIfMissingAtEnd` | Adds the Boot scene enabled at the end when absent. This is the safe default for an existing project. |
| `PlaceFirstAfterApproval` | Places Boot first while preserving unrelated scene order. Requires the separate approval checkbox. Use this for a new project or a player build whose startup must begin at First Light. |

The destination must still be present exactly once and enabled.

## Refresh and review

1. Press **Refresh Plan**.
2. Read every operation and diagnostic.
3. Use **Copy Plan** when retaining setup evidence.
4. Resolve blockers or manual decisions before applying.

Refreshing is read-only. It does not create, repair, save, delete, move, rename,
or modify Build Settings.

## Apply Plan

Use **Apply Plan...** only for a fresh executable create/reuse plan.

Apply may create:

- project-owned First Light folders;
- `EchoLaunchConfiguration.asset`;
- `StartupSequence.asset`;
- `LaunchDestination.asset`;
- optional `SplashSequence.asset`;
- project-owned `EchoLaunchRoot.prefab` variant;
- canonical `Boot.unity`; and
- the approved Boot Scene List entry.

Apply is create-only. It never overwrites an incompatible target or edits the
selected destination scene. It replans immediately before writing and rejects
stale evidence. Run it again after success; the accepted repeat result is
`NoChanges`.

## Repair Plan

Use **Repair Plan...** only when Setup displays an approved, proof-backed
current-schema repair. Repair is a separate confirmation and may reconcile
only the narrow surfaces listed in the plan.

Repair does not migrate old schemas, replace an unknown type, regenerate an ID,
delete duplicate roots, restructure arbitrary prefabs, or clean unrelated scene
content. It secures exact asset and `.meta` backups before mutation and reports
any retained recovery path.

## Validate

Open:

```text
Tools > Sperk's Forge > First Light > Validator
```

1. Keep **Project Root** aligned with the root used in Setup.
2. Press **Validate Project**.
3. Review every finding.
4. Use **Copy Report** to retain deterministic project-relative evidence.

The Validator is read-only. It never invokes Apply or Repair.

Project-health meaning:

| Highest finding | Project health | Meaning |
|---|---|---|
| Information only | `Healthy` | No warning, error, or blocker was found. |
| Warning | `NeedsAttention` | Launch may work, but the warning must be understood. |
| Error | `Invalid` | Authored setup is not valid. |
| Blocker | `Blocked` | Launch-ready proof failed and must be corrected. |

An intentionally enabled Development-Build Direct Scene policy produces a
visible warning. `EditorOnly` is the release-safer default.

## First canonical run

1. Confirm Boot is first and the destination is enabled in the Scene List.
2. Open the generated Boot scene.
3. Enter Play Mode.
4. Confirm First Light reaches the destination and publishes a completed
   report.
5. Exit Play Mode and run the Validator again.

The generated startup sequence is intentionally empty. Add project-specific
steps only after the empty canonical launch succeeds.
