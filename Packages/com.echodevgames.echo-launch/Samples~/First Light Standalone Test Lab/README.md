# First Light Standalone Test Lab

This is the isolated First Light package sample for FL-M5-07.

The first import contains the public-API sample steps and an explicit Unity
authoring command. It does not create scenes, assets, Build Settings entries,
or project settings automatically.

## Bootstrap authoring flow

1. Import this sample from Package Manager.
2. Wait for compilation to finish.
3. Run:

   `Tools > Sperk's Forge > First Light > Laboratory > Build Imported Laboratory`

4. The command creates the serialized Laboratory under this imported sample and
   exports the generated distribution content back into the embedded package's
   `Samples~` folder.
5. Remove this imported sample copy.
6. Reimport the sample from Package Manager to validate the final distribution
   payload.

The authoring command is temporary checkpoint tooling. It will be removed from
the final distribution before FL-M5-07 implementation staging.

## Safety boundary

Importing this sample does not:

- Change Build Settings.
- Change ProjectSettings.
- Run First Light Setup, Apply, Repair, or Validator.
- Create canonical project setup assets.
- Enter Play Mode.
- Claim a launch root.
- Install another Sperk's Forge package.

No other Sperk's Forge package is required.

## Planned final scenes

- `Generated/Scenes/FirstLight_Boot_Lab.unity`
- `Generated/Scenes/FirstLight_Destination_Lab.unity`

The generated sample assets are examples, not canonical project setup.
