# First Light Standalone Test Lab

This sample is the isolated visible First Light launch proof. It uses only
public First Light Runtime APIs and requires no other Sperk's Forge package.

The imported sample is project-owned example content. It is not canonical
project setup and may be edited, deleted, and reimported.

## Import

1. Open **Window > Package Management > Package Manager**.
2. Select **First Light - Startup and Launch**.
3. Import **First Light Standalone Test Lab**.
4. Wait for compilation to finish.

The imported path follows this pattern:

```text
Assets/Samples/First Light — Startup and Launch/0.1.0/First Light Standalone Test Lab
```

Import does not change the Build Profiles Scene List, ProjectSettings, canonical First Light
setup assets, scripting defines, open scenes, or Play Mode.

## Scenes and Build Profiles

The final distribution scenes are:

```text
Generated/Scenes/FirstLight_Boot_Lab.unity
Generated/Scenes/FirstLight_Destination_Lab.unity
```

For Boot testing, add them explicitly to **File > Build Profiles > Scene List**
in this order:

1. `FirstLight_Boot_Lab`
2. `FirstLight_Destination_Lab`

Do not replace unrelated project scene entries permanently. Restore the
project's original scene list after Laboratory testing.

## Canonical Boot Run

1. Open `FirstLight_Boot_Lab`.
2. Assign `Generated/Configuration/SuccessConfiguration.asset` to the active
   `EchoLaunchRoot` configuration field.
3. Enter Play Mode.
4. Observe the splash/status presentation, ordered steps, completed report,
   and activation of `FirstLight_Destination_Lab`.

The destination readout exposes accepted state, progress, warnings,
diagnostics, launch mode, and destination evidence.

## Scenario Selection

Select one pre-authored configuration before entering Play Mode:

| Configuration | Expected result |
|---|---|
| `SuccessConfiguration.asset` | Immediate plus timed progress completes and hands off. |
| `WarningConfiguration.asset` | Warning evidence is retained and launch continues. |
| `RecoverableConfiguration.asset` | Optional failure converts to a warning and launch continues. |
| `BlockingConfiguration.asset` | Blocking failure stops later work and prevents handoff. |
| `InvalidDestinationConfiguration.asset` | Preflight blocks with `ELAUNCH-DEST-001`. |

For missing-configuration proof, clear the root's Configuration field before
Play. The launch blocks with `ELAUNCH-CFG-001`. Restore the reference afterward.

For duplicate-root proof, enable the inactive duplicate fixture in the Boot
scene. Exactly one authority remains active and the duplicate emits
`ELAUNCH-ROOT-001`. Disable the fixture after the run.

## Direct Scene Tests

Open `FirstLight_Destination_Lab` directly.

For creation proof:

1. Leave the inactive existing-root fixture disabled.
2. Confirm the `EchoDirectSceneInitializer` references
   `LaboratoryDirectSceneConfiguration`.
3. Enter Play Mode.
4. Confirm one `DirectSceneDevelopment` authority is created, the active scene
   is not reloaded, and the launch completes.

For reuse proof:

1. Enable the inactive existing-root fixture.
2. Enter Play Mode.
3. Confirm the initializer reuses the existing authority and does not create a
   duplicate.
4. Disable the fixture after the run.

## Splash Minimum-Duration Test

Use the success configuration and request a skip during the positive minimum
display interval. The splash remains visible until the minimum is satisfied,
then advances exactly once when skipping is permitted.

## Optional Rebuild Command

The sample includes an explicit Editor authoring command:

```text
Tools > Sperk's Forge > First Light > Laboratory > Build Imported Laboratory
```

Use it only when intentionally rebuilding the generated Laboratory payload.
It replaces the imported sample's `Generated` folder, validates serialized
   references, and exports that generated folder back to the embedded package
sample. It does not run automatically on import, reload, repaint, or Play Mode
entry, and it does not change the Build Profiles Scene List or ProjectSettings.

## Reset, Removal, and Reimport

To reset a scenario:

1. Exit Play Mode.
2. Restore `SuccessConfiguration.asset` on the Boot root.
3. Disable the duplicate fixture in Boot.
4. Disable the existing-root fixture in Destination.
5. Restore the project's original Build Profiles Scene List.

To remove the sample, delete only its imported folder beneath `Assets/Samples`.
First Light Runtime, Editor tools, Setup, Validator, and Simulator do not
depend on the imported copy.

To restore a clean sample, return to Package Manager and import **First Light
Standalone Test Lab** again. Reimport creates one clean project-owned copy; it
does not synchronize earlier edits or alter canonical setup assets.

## Safety Boundary

Importing or using the sample does not:

- install or require a peer Sperk's Forge runtime package
- grant sample code access to First Light internals
- make sample assets canonical Setup/Repair candidates
- automatically run Setup, Apply, Repair, Validator, or Simulator
- automatically edit the Build Profiles Scene List or ProjectSettings
- install a production startup or Direct Scene build hook

Generated Laboratory assets are examples. Copy and adapt them deliberately;
do not treat their IDs, paths, fixtures, or branding as production authority.
