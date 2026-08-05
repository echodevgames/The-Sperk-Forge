# FL-M2-05 Lifecycle Notification Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode
- Implementation commit: `877761f`

## Result

FL-M2-05 lifecycle notification tests:

- Passed: `20`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `102`
- Failed: `0`
- Ignored: `0`

## Verified Areas

- Accepted state-event emission
- Accepted progress-event emission
- Same-state progress-only publication
- State-before-progress order
- Previous/current state payload
- Previous/current progress payload
- Accepted root state visible during callbacks
- No notifications after invalid transition
- No notifications after launch-mode mismatch
- No notifications after terminal rewrite
- Listener unsubscription
- State-listener isolation
- Progress-listener isolation
- Progress notification after failed state listener
- Stable listener-failure diagnostics
- Duplicate-root publication rejection
- Publication with no listeners
- Destroyed-root subscription isolation
- State delegate cleanup
- Progress delegate cleanup

## Diagnostic Evidence

Duplicate-root tests intentionally generated:

    [ELAUNCH-ROOT-001] Duplicate EchoLaunchRoot rejected. The first valid root remains authoritative.

Broken-listener tests intentionally generated:

    [ELAUNCH-EVENT-001] Listener failure while dispatching the lifecycle notification.

The expected warnings were registered by the automated tests and did not count as failures.

## Scope Limit

This report proves only FL-M2-05 lifecycle notification behavior and the retained Runtime Play Mode suite.

It does not prove automatic lifecycle advancement, startup execution, launch reports, presentation, scene loading, Player-build compatibility, clean-project installation, or performance budgets.
