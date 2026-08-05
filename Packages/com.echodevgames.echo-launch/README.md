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
  - `FL-M2-05` Lifecycle Notifications
  - `FL-M2-06` Launch Configuration Identity and Root Binding
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

### Lifecycle Notifications

- Public `LaunchStateChanged` observer event
- Public `LaunchProgressChanged` observer event
- Previous/current notification payloads
- State notification before progress notification
- Accepted state visible during callbacks
- Per-listener exception containment
- Stable listener-failure diagnostic `ELAUNCH-EVENT-001`
- Delegate cleanup when the root is destroyed

### Launch Configuration

- Project-owned `EchoLaunchConfiguration` asset
- Create menu entry under First Light
- Canonical runtime-safe stable configuration ID
- Serialized schema version `1`
- Read-only configuration identity and schema
- Invalid identity detection without silent repair
- Unsupported schema detection without runtime rewrite
- Passive serialized root binding
- Read-only authority-filtered `EchoLaunchRoot.Configuration`
- Duplicate and stale-root configuration hiding
- No mutable launch-session state stored in the asset

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

- Passed: `117`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Launch configuration binding tests: `15`
- Launch-state vocabulary tests: `39`
- Launch session and progress tests: `14`
- Lifecycle transition tests: `22`
- Lifecycle notification tests: `20`

Expected yellow diagnostic evidence:

- `ELAUNCH-ROOT-001` from duplicate-root tests
- `ELAUNCH-EVENT-001` from broken-listener containment tests

Manual evidence:

- Unity created a project-owned launch configuration through the package Create menu.
- The default Inspector exposed no mutable launch-session state.
- Asset creation caused no scene object, lifecycle transition, startup execution, or warning.
- The temporary verification asset was removed before Git review.

## Not Implemented Yet

First Light does not yet provide:

- Automatic lifecycle advancement
- Startup sequences or step definitions
- Startup executors
- Configuration preflight
- Configuration migration or repair
- Launch reports
- Splash presentation
- Scene loading
- Persistent-root lifetime policy
- Direct-scene initialization behavior
- Editor setup tools beyond `CreateAssetMenu`
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
- One hundred seventeen passing Runtime Play Mode tests
- Launch configuration Create menu verification
- Configuration immutability and authority filtering
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
