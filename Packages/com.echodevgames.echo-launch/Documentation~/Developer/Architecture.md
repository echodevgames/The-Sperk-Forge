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
- Unity baseline: `6000.3.8f1`

## Current Architecture

First Light currently establishes:

1. Single launch authority
2. Neutral launch-state vocabulary
3. One live session owned by the authoritative root
4. Read-only state and progress exposure
5. Central lifecycle transition validation
6. Isolated lifecycle notifications

It does not yet execute startup behavior.

## Implemented Runtime Files

    Runtime/
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
    ├── LaunchStateVocabularyTests.cs
    ├── LaunchSessionProgressTests.cs
    ├── LaunchLifecycleTransitionTests.cs
    └── LaunchNotificationTests.cs

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

- Passed: `102`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Vocabulary tests: `39`
- Session and progress tests: `14`
- Lifecycle transition tests: `22`
- Lifecycle notification tests: `20`

## Current Exclusions

Not implemented:

- Automatic lifecycle advancement
- Startup configuration assets
- Startup sequences
- Step definitions or executors
- Launch reports
- Splash presentation
- Scene loading
- Persistent-root lifetime
- Direct-scene initialization behavior
- Editor setup tools
- Standalone Laboratory
- Peer-package bridges

## Stop Point

FL-M2-05 stops after accepted lifecycle state can be observed safely without giving listeners launch authority.

The next runtime slice requires separate approval.
