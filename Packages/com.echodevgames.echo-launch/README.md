# First Light - Startup and Launch

First Light is the startup coordination package for The Sperk's Forge - EchoDevGames Game Systems Suite.

It coordinates ordered application initialization and final handoff without owning the internal behavior of peer packages.

## Package Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed runtime slices:
  - `FL-M2-01` Authority Claim and Static Reset Core
  - `FL-M2-02` Neutral Launch-State Vocabulary
  - `FL-M2-03` Launch Session and Read-Only Progress Surface
  - `FL-M2-04` Launch Lifecycle Transition Guard
- Unity baseline: `6000.3.8f1`
- Minimum declared Unity version: `6000.0`
- uGUI dependency: `2.0.0`

## Implemented Runtime Scope

First Light now provides:

### Authority Core

- One process-wide launch-authority claim
- Immediate duplicate rejection
- Stable duplicate diagnostic code `ELAUNCH-ROOT-001`
- Owner-only authority release
- Static reset through subsystem registration

### Launch-State Vocabulary

- `LaunchMode`
- `LaunchStatus`
- `StartupStepStatus`
- Immutable `StartupStepResult`
- Immutable `LaunchProgressSnapshot`

### Launch Session and Progress

- One fresh `LaunchSession` per authoritative root
- Initial `AuthorityClaimed` state
- `LaunchProgressSnapshot.Empty`
- Read-only `EchoLaunchRoot.State`
- Read-only `EchoLaunchRoot.Progress`
- Controlled internal progress publication
- Duplicate and stale-root state hiding

### Lifecycle Transition Guard

- Centralized `LaunchStateTransitionRules`
- Approved forward lifecycle path
- Same-state progress publication for active states
- Failure and interruption from active states
- Rejection of backward and skipped-phase transitions
- Permanent terminal-state freezing
- Transactional publication that preserves the prior snapshot when validation fails

## Approved Lifecycle

    None
        -> AuthorityClaimed
            -> Validating
                -> Running
                    -> Transitioning
                        -> Completed

Active states may also enter:

    Failed
    Interrupted

`Completed`, `Failed`, and `Interrupted` are terminal.

## Verified Behavior

The Runtime Play Mode suite reports:

- Passed: `82`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Launch-state vocabulary tests: `39`
- Launch session and progress tests: `14`
- Lifecycle transition tests: `22`

Four `ELAUNCH-ROOT-001` warnings are expected diagnostic evidence from duplicate-root tests.

## Not Implemented Yet

First Light does not yet provide:

- Automatic lifecycle advancement
- Startup configuration assets
- Startup sequences or step definitions
- Startup executors
- Public state or progress events
- Launch reports
- Splash presentation
- Scene loading
- Persistent-root lifetime policy
- Direct-scene initialization behavior
- Editor setup tools
- Standalone Laboratory
- Peer-package bridges

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
- Eighty-two passing Runtime Play Mode tests
- No out-of-scope startup execution

Still `Not run`:

- Git URL installation
- Tarball installation
- Separate clean-project installation
- Player builds
- Startup execution
- Performance measurements

## License

See [LICENSE.md](LICENSE.md).

## Third-Party Notices

See [Third Party Notices.md](Third%20Party%20Notices.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).
