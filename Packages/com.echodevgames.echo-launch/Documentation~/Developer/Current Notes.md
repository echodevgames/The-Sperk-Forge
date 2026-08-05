# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M2-05`
- Title: Lifecycle Notifications
- Package version: `0.1.0`
- Implementation status: Complete and pushed
- Implementation commit: `877761f`
- Documentation closeout: Pending adjacent commit
- Runtime Play Mode result: 102 passed, 0 failed, 0 ignored

## Completed Result

Implemented:

- Public `LaunchStateChanged` notification
- Public `LaunchProgressChanged` notification
- Previous/current notification payloads
- Accepted-state visibility before callbacks
- State notification before progress notification
- Same-state progress notification without a false state event
- No notification from rejected publications
- Per-listener exception containment
- Stable diagnostic `ELAUNCH-EVENT-001`
- Delegate cleanup when the root is destroyed
- Twenty notification tests

## Evidence Summary

### Passed

- Accepted lifecycle state event
- Accepted progress event
- Same-state progress-only publication
- State-before-progress order
- Previous/current state payload
- Previous/current progress payload
- Invalid-transition silence
- Launch-mode mismatch silence
- Terminal-rewrite silence
- Unsubscription
- State-listener isolation
- Progress-listener isolation
- Cross-event isolation
- Stable listener-failure diagnostics
- Duplicate-root publication rejection
- No-listener publication
- Destroyed-root subscription isolation
- Delegate-field cleanup
- One hundred two total Runtime Play Mode tests

### Expected Diagnostics

Tests intentionally generated:

    ELAUNCH-ROOT-001
    ELAUNCH-EVENT-001

These warnings were expected and matched by the automated test suite.

### Not Run

- Automatic lifecycle advancement
- Startup configuration
- Startup execution
- Launch reports
- Splash presentation
- Scene loading
- Player builds
- Performance measurements

## Changed Files

Runtime implementation:

- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/Events/LaunchNotificationDispatcher.cs`
- `Runtime/Events/LaunchProgressChangedEvent.cs`
- `Runtime/Events/LaunchStateChangedEvent.cs`
- Unity-generated `.meta` files

Automated tests:

- `Tests/Runtime/PlayMode/LaunchNotificationTests.cs`
- Unity-generated `.meta` file

Adjacent documentation:

- Package checkpoint
- Package test report
- Root completion record
- Changelog, architecture, index, README, and suite Current Notes

## Handoff Snapshot

FL-M2-05 implementation is complete and pushed in commit `877761f`.

The adjacent documentation closeout is ready for final Git review, commit, and push.

No additional runtime behavior is authorized until the next checkpoint is approved.
