# First Light - Startup and Launch

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

It coordinates ordered application initialization and final handoff without owning the internal behavior of peer packages.

## Package Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed runtime slice: Authority Claim and Static Reset Core
- Unity baseline: `6000.3.8f1`
- Minimum declared Unity version: `6000.0`
- uGUI dependency: `2.0.0`

## Implemented Runtime Scope

First Light now provides:

- One process-wide launch-authority claim
- `EchoLaunchRoot.Current`
- `EchoLaunchRoot.IsAuthoritative`
- `EchoLaunchRoot.WasRejectedAsDuplicate`
- Immediate duplicate rejection in `Awake`
- Stable duplicate diagnostic code `ELAUNCH-ROOT-001`
- Duplicate-component disabling before future startup behavior
- Owner-only authority release
- Static reset through Unity subsystem registration
- Runtime test access through `InternalsVisibleTo`

## Verified Behavior

The Runtime Play Mode suite proves:

- The first root claims authority.
- A second root is rejected without replacing the first.
- Destroying a duplicate does not release the real authority.
- Destroying the authoritative root releases the claim.
- Static reset clears the current authority.
- A fresh root can claim after reset.
- Deferred Unity destruction allows a fresh root to claim.

Test result:

- Passed: `7`
- Failed: `0`
- Ignored: `0`

The two `ELAUNCH-ROOT-001` warnings are expected diagnostic evidence from duplicate-root tests.

## Not Implemented Yet

First Light does not yet provide:

- Startup configuration assets
- Startup sequences or step definitions
- Startup executors
- Launch sessions or reports
- Progress snapshots
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
- Seven passing Runtime Play Mode tests
- Expected duplicate diagnostic warnings
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
