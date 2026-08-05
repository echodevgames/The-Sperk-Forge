# FL-M2-05 - Lifecycle Notifications

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M2-05`
- Implementation status: Complete and pushed
- Implementation commit: `877761f`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Expose accepted launch lifecycle state to observers without granting listeners authority or allowing listener failures to damage the launch session.

## Authorized Files

New:

    Runtime/Events/LaunchNotificationDispatcher.cs
    Runtime/Events/LaunchProgressChangedEvent.cs
    Runtime/Events/LaunchStateChangedEvent.cs
    Tests/Runtime/PlayMode/LaunchNotificationTests.cs

Modified:

    Runtime/Core/EchoLaunchRoot.cs

Unity-generated `.meta` files are part of the authorized asset scope.

## Implemented Contract

- `LaunchStateChanged` for accepted lifecycle changes
- `LaunchProgressChanged` for every accepted progress publication
- Previous/current state and progress payloads
- State notification before progress notification
- Accepted state visible during callbacks
- No notification from rejected publication
- Per-listener exception containment
- Stable warning diagnostic `ELAUNCH-EVENT-001`
- Delegate cleanup during root destruction

## Notification Order

    validate publication
        -> accept snapshot
            -> state event when state changed
                -> progress event

A listener observes the same accepted state exposed by the root properties.

## Failure Containment

- One failing listener cannot block later listeners.
- A failing state listener cannot block the progress event.
- Listener failure cannot roll back accepted state.
- Duplicate roots cannot publish notifications.
- Invalid transitions, mode mismatches, and terminal rewrites emit no notifications.
- Destroyed-root subscriptions do not transfer to a replacement root.

## Test Evidence

| Area | Result |
|---|---|
| State notification | Pass |
| Progress notification | Pass |
| Same-state publication | Pass |
| Dispatch order | Pass |
| Payload accuracy | Pass |
| Rejected-publication silence | Pass |
| Unsubscription | Pass |
| Listener isolation | Pass |
| Stable diagnostics | Pass |
| Duplicate-root protection | Pass |
| Destruction cleanup | Pass |

FL-M2-05 totals:

- Passed: `20`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `102`
- Failed: `0`
- Ignored: `0`

## Expected Diagnostics

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These yellow warnings are intentional test evidence.

## Explicit Exclusions

Not implemented:

- Automatic lifecycle advancement
- Startup configuration
- Startup steps or executors
- Launch reports
- Presentation
- Scene loading
- Persistent lifetime
- Direct-scene initializer behavior
- Editor tools
- Test Lab scenes
- Peer-package bridges

## Closure Result

The approved lifecycle notification surface compiles and all one hundred two Runtime Play Mode tests pass.

Implementation commit `877761f` is present on `main` and `origin/main`.

FL-M2-05 is ready for its adjacent documentation commit.

The next runtime checkpoint requires separate approval.
