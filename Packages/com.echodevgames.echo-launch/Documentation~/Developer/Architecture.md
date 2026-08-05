# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
  - `FL-M2-03`
  - `FL-M2-04`
- Unity baseline: `6000.3.8f1`

## Current Architecture

First Light currently establishes:

1. Single launch authority
2. Neutral launch-state vocabulary
3. One live session owned by the authoritative root
4. Read-only state and progress exposure
5. Central lifecycle transition validation

It does not yet execute startup behavior.

## Implemented Runtime Files

    Runtime/
    ├── Core/
    │   ├── LaunchAuthorityClaim.cs
    │   └── EchoLaunchRoot.cs
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
    └── LaunchLifecycleTransitionTests.cs

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

## Test Evidence

Runtime Play Mode totals:

- Passed: `82`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Vocabulary tests: `39`
- Session and progress tests: `14`
- Lifecycle transition tests: `22`

## Current Exclusions

Not implemented:

- Automatic lifecycle advancement
- Startup configuration assets
- Startup sequences
- Step definitions or executors
- Public state or progress events
- Launch reports
- Splash presentation
- Scene loading
- Persistent-root lifetime
- Direct-scene initialization behavior
- Editor setup tools
- Standalone Laboratory
- Peer-package bridges

## Stop Point

FL-M2-04 stops after illegal lifecycle publication is rejected without mutating the session.

The next runtime slice requires separate approval.
