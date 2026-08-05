# First Light - Startup and Launch

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

It coordinates ordered application initialization and final handoff without owning the internal behavior of peer packages.

## Package Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed runtime slices:
  - `FL-M2-01` Authority Claim and Static Reset Core
  - `FL-M2-02` Neutral Launch-State Vocabulary
- Unity baseline: `6000.3.8f1`
- Minimum declared Unity version: `6000.0`
- uGUI dependency: `2.0.0`

## Implemented Runtime Scope

First Light now provides:

### Authority Core

- One process-wide launch-authority claim
- `EchoLaunchRoot.Current`
- `EchoLaunchRoot.IsAuthoritative`
- `EchoLaunchRoot.WasRejectedAsDuplicate`
- Immediate duplicate rejection in `Awake`
- Stable duplicate diagnostic code `ELAUNCH-ROOT-001`
- Duplicate-component disabling before future startup behavior
- Owner-only authority release
- Static reset through Unity subsystem registration

### Launch-State Vocabulary

- `LaunchMode`
- `LaunchStatus`
- `StartupStepStatus`
- Immutable `StartupStepResult`
- Immutable `LaunchProgressSnapshot`
- Validated diagnostic result factories
- Policy-neutral result classification
- Snapshot range and index validation
- Safe string normalization

## Verified Behavior

The Runtime Play Mode suite now reports:

- Passed: `46`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Launch-state vocabulary tests: `39`

The two `ELAUNCH-ROOT-001` warnings remain expected diagnostic evidence from duplicate-root tests.

## Not Implemented Yet

First Light does not yet provide:

- Startup configuration assets
- Startup sequences or step definitions
- Startup executors
- Launch sessions or report aggregation
- Progress publication
- Splash presentation
- Scene loading
- `DontDestroyOnLoad` lifetime policy
- Direct-scene development initialization
- Editor setup tools
- Standalone Laboratory
- Bridges to peer packages

## Assembly Layout

    Runtime/
        EchoDevGames.EchoLaunch.Runtime

    Editor/
        EchoDevGames.EchoLaunch.Editor

    Tests/Runtime/
        EchoDevGames.EchoLaunch.Tests.Runtime

    Tests/Editor/
        EchoDevGames.EchoLaunch.Tests.Editor

The dependency direction remains:

    Editor -> Runtime
    Runtime Tests -> Runtime
    Editor Tests -> Editor + Runtime

The Runtime assembly must never reference the Editor or test assemblies.

## Documentation

Package documentation lives under `Documentation~`.

The suite-wide architecture and approved First Light specification live in the repository's `Plan Documentation` vault.

## Evidence Status

Available evidence:

- Embedded package recognition
- Clean Unity compilation
- Unity restart
- Embedded-package removal and reinstallation
- Stable assembly-definition GUIDs
- Seven passing authority tests
- Thirty-nine passing vocabulary tests
- Forty-six total passing Runtime Play Mode tests
- No out-of-scope First Light runtime features

Still `Not run`:

- Git URL installation
- Tarball installation
- Separate clean-project installation
- Player builds
- Startup sequencing
- Performance measurements

## License

See [LICENSE.md](LICENSE.md).

## Third-Party Notices

See [Third Party Notices.md](Third%20Party%20Notices.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).
