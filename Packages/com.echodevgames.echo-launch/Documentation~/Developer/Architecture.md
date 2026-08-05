# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
  - `FL-M2-03`
  - `FL-M2-04`
  - `FL-M2-05`
  - `FL-M2-06`
- Unity baseline: `6000.3.8f1`

## Current Architecture

First Light currently establishes:

1. Single launch authority
2. Neutral launch-state vocabulary
3. One live session owned by the authoritative root
4. Read-only state and progress exposure
5. Central lifecycle transition validation
6. Isolated lifecycle notifications
7. Project-owned launch configuration identity
8. Authority-filtered root configuration binding

It does not yet execute startup behavior.

## Implemented Runtime Files

    Runtime/
    ├── Configuration/
    │   └── EchoLaunchConfiguration.cs
    ├── Core/
    │   ├── LaunchAuthorityClaim.cs
    │   └── EchoLaunchRoot.cs
    ├── Events/
    │   ├── LaunchNotificationDispatcher.cs
    │   ├── LaunchProgressChangedEvent.cs
    │   └── LaunchStateChangedEvent.cs
    ├── Properties/
    │   └── AssemblyInfo.cs
    ├── State/
    │   ├── LaunchMode.cs
    │   ├── LaunchStatus.cs
    │   ├── LaunchProgressSnapshot.cs
    │   ├── LaunchSession.cs
    │   └── LaunchStateTransitionRules.cs
    └── Steps/
        ├── StartupStepStatus.cs
        └── StartupStepResult.cs

    Tests/Runtime/PlayMode/
    ├── EchoLaunchRootAuthorityTests.cs
    ├── LaunchConfigurationBindingTests.cs
    ├── LaunchLifecycleTransitionTests.cs
    ├── LaunchNotificationTests.cs
    ├── LaunchSessionProgressTests.cs
    └── LaunchStateVocabularyTests.cs

## Launch Configuration Definition

`EchoLaunchConfiguration` is a project-owned `ScriptableObject`.

It contains authored definition data only:

    configurationId
    schemaVersion

It does not contain current launch state, progress, timings, retries, active scene references, or execution results.

Active mutable state remains owned by `LaunchSession`.

### Stable Configuration Identity

Every newly created configuration receives:

    Guid.NewGuid().ToString("N")

The canonical format is:

- Exactly 32 characters
- Lowercase hexadecimal
- Characters `0-9` and `a-f`
- No spaces, punctuation, or separators

The runtime-safe configuration ID is distinct from Unity's asset GUID, asset path, filename, display name, and runtime instance ID.

Runtime code detects malformed identity but does not silently regenerate or repair it.

### Configuration Schema

`EchoLaunchConfiguration.CurrentSchemaVersion` is `1`.

The serialized `schemaVersion` describes the structure of the configuration asset. It is independent from the package version.

Runtime code detects unsupported schema values but does not rewrite or migrate them.

Migration and repair remain future Editor-tooling responsibilities.

## Root Configuration Binding

`EchoLaunchRoot` contains one passive serialized configuration reference.

The public property:

    EchoLaunchRoot.Configuration

returns:

- The assigned configuration when the root is authoritative
- `null` when no configuration is assigned
- `null` when the root is a rejected duplicate
- `null` when a former authority becomes stale after reset

Binding a configuration does not:

- Validate or repair the asset
- Advance lifecycle state
- Begin startup execution
- Create a default configuration
- Clone the asset
- Write runtime values into the asset
- Emit a missing-configuration warning

Preflight behavior remains outside FL-M2-06.

## Lifecycle Transition Authority

`LaunchStateTransitionRules` is the single internal authority for lifecycle legality.

It exposes:

    IsTerminal
    CanTransition
    EnsureCanPublish

It validates that status values are defined before interpreting them.

## Approved Transition Graph

    None
        -> AuthorityClaimed

    AuthorityClaimed
        -> AuthorityClaimed
        -> Validating
        -> Failed
        -> Interrupted

    Validating
        -> Validating
        -> Running
        -> Failed
        -> Interrupted

    Running
        -> Running
        -> Transitioning
        -> Failed
        -> Interrupted

    Transitioning
        -> Transitioning
        -> Completed
        -> Failed
        -> Interrupted

    Terminal:
        Completed
        Failed
        Interrupted

## Same-State Publication

Same-state publication is legal only for active states.

It supports progress updates within one lifecycle phase without inventing a false transition.

## Terminal Freezing

Once a session reaches `Completed`, `Failed`, or `Interrupted`, no additional snapshot may be published.

This includes publication of the same terminal state.

A new launch attempt requires a new session.

## Transactional Publication

`LaunchSession.Publish` validates in this order:

    Validate launch mode
        -> validate lifecycle transition
            -> replace current snapshot

If validation fails, the stored progress snapshot remains unchanged.

## Root Integration

`EchoLaunchRoot.PublishProgress` delegates to `LaunchSession.Publish`.

The root therefore inherits the lifecycle guard automatically without duplicating transition logic.

## Lifecycle Notifications

`EchoLaunchRoot` exposes two public observer events:

    LaunchStateChanged
    LaunchProgressChanged

`LaunchStateChanged` is raised only when an accepted publication changes the lifecycle state.

`LaunchProgressChanged` is raised after every accepted publication, including a same-state progress update.

Publication order is:

    validate publication
        -> accept the new snapshot
            -> dispatch state change when required
                -> dispatch progress change

Listeners therefore observe the accepted authoritative state during their callbacks.

`LaunchNotificationDispatcher` snapshots the invocation list and invokes each listener independently. One listener exception cannot stop later listeners, block the progress event, or roll back accepted launch state.

Listener failures produce:

    ELAUNCH-EVENT-001

Rejected duplicate-root, mode-mismatched, invalid-transition, and terminal-rewrite publications emit no notifications.

Root destruction clears both event delegate fields so subscriptions cannot transfer to a later root.

## Test Evidence

Runtime Play Mode totals:

- Passed: `117`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Configuration binding tests: `15`
- Vocabulary tests: `39`
- Session and progress tests: `14`
- Lifecycle transition tests: `22`
- Lifecycle notification tests: `20`

Manual verification:

- Unity Create menu generated a project-owned launch configuration asset.
- The default Inspector exposed no mutable session state.
- Asset creation produced no root, GameObject, lifecycle transition, startup behavior, or warning.
- The temporary verification asset was removed before Git review.

## Current Exclusions

Not implemented:

- Automatic lifecycle advancement
- Startup sequences
- Step definitions or executors
- Configuration preflight
- Configuration migration or repair
- Launch reports
- Splash presentation
- Scene loading
- Persistent-root lifetime
- Direct-scene initialization behavior
- Editor setup tools beyond `CreateAssetMenu`
- Standalone Laboratory
- Peer-package bridges

## Stop Point

FL-M2-06 stops after project-owned configuration identity and passive authoritative root binding are proven.

The next runtime slice requires separate approval.
