# First Light Quick Start

This path creates one project-owned First Light Boot scene and launches one
existing destination scene. Complete [Installation](Installation.md) first.

## 1. Create the destination

Create or choose the scene that should open after startup, then save it beneath
`Assets/`.

Open **File > Build Profiles** and add the destination exactly once, enabled,
to the active Scene List. If the profile does not override Scene List, Unity
uses the global scene list.

## 2. Open First Light Setup

Use:

```text
Tools > Sperk's Forge > First Light > Setup
```

For a new project, keep the default paths:

```text
Project Root: Assets/EchoDevGames/FirstLight
Boot Scene:   Assets/EchoDevGames/FirstLight/Scenes/Boot.unity
```

Assign the saved destination scene.

Set **Build Settings Policy** to `PlaceFirstAfterApproval`, then check the
separate approval box. This makes the generated Boot scene the first player
scene while preserving the unrelated scene order.

Leave **Create Splash Sequence** off for the smallest setup. It may be enabled
when you want an empty project-owned splash asset to author later.

## 3. Review and apply

1. Press **Refresh Plan**.
2. Read the complete plan and diagnostics.
3. Press **Apply Plan...**.
4. Confirm the displayed changes.
5. Wait for Unity to finish importing/saving.

Apply creates missing project-owned assets and reuses compatible content. It
does not edit the destination scene.

Press **Refresh Plan** again. A correctly converged project should contain only
reuse/no-change results; a repeat Apply should settle as `NoChanges`.

## 4. Validate

Open:

```text
Tools > Sperk's Forge > First Light > Validator
```

Press **Validate Project**.

The expected new-project result is `Healthy`. If it is not, copy the report and
follow [Troubleshooting and Known Limitations](Troubleshooting%20and%20Known%20Limitations.md).

## 5. Run canonical Boot

1. Open `Assets/EchoDevGames/FirstLight/Scenes/Boot.unity`.
2. Enter Play Mode.
3. Confirm First Light reaches the chosen destination.
4. Confirm the final state is `Completed` and no unexpected Console error
   appears.

The initial `StartupSequence` is intentionally empty. Empty startup still
validates, reports, and hands off to the destination.

## Success checklist

- One First Light root has authority.
- Boot is first in the Scene List.
- Destination appears exactly once and enabled.
- Validator reports `Healthy`.
- Canonical Boot reaches the destination.
- Repeat Setup returns `NoChanges`.
- Project-owned content lives beneath `Assets/EchoDevGames/FirstLight`.

## Next steps

- Add project startup operations with [Startup Step Authoring](Startup%20Step%20Authoring.md).
- Configure splash/status presentation from project-owned assets and prefab
  overrides.
- Import the Standalone Test Lab for visible warning/failure/direct-scene proof.
- Read [Setup and Validation](Setup%20and%20Validation.md) before using Repair or
  changing Build Settings policy in an existing project.
