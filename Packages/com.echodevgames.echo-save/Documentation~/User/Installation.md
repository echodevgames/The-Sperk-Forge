# Installation

The Chronicle is currently embedded in The Sperk's Forge repository as:

`Packages/com.echodevgames.echo-save`

## Requirements

- Unity `6000.0` or newer.
- No other Sperk's Forge runtime package is required.

## ESV-M1-01 setup

1. Create an `EchoSaveConfiguration` asset through:
   `Assets > Create > EchoDevGames > The Chronicle > Echo Save Configuration`
2. Add `EchoSaveRoot` to a GameObject chosen by the consumer project.
3. Assign the configuration asset.
4. Decide whether the project calls `InitializeAsync()` itself or enables `Auto Initialize`.

The consumer project remains responsible for scene-surviving lifetime. The Chronicle does not automatically call `DontDestroyOnLoad`.

## Current limitation

ESV-M1-01 does not write save data. Seeing no `.sav`, `.json`, slot, or generation output is the expected result.
