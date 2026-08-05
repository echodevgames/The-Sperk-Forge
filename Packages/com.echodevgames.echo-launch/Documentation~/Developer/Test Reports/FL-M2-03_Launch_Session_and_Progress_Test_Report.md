# FL-M2-03 Launch Session and Progress Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode

## Result

FL-M2-03 tests:

- Passed: `14`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `60`
- Failed: `0`
- Ignored: `0`

## Verified Areas

- Fresh authority session
- Canonical initial progress
- Direct-scene launch mode
- Safe empty snapshot
- Duplicate state hiding
- Snapshot replacement
- Same-state publication
- Previous snapshot immutability
- Mode mismatch rejection
- `None` status rejection
- Undefined mode rejection
- Duplicate publication rejection
- Static-reset stale-state hiding
- Fresh session after authority destruction

## Diagnostic Evidence

Four tests intentionally created duplicate roots.

Expected warning:

    [ELAUNCH-ROOT-001] Duplicate EchoLaunchRoot rejected. The first valid root remains authoritative.

The warnings were registered with `LogAssert.Expect` and did not count as failures.

## Scope Limit

This report proves only FL-M2-03 launch-session ownership and read-only progress behavior.

It does not prove startup execution, lifecycle transition rules, public events, launch reports, scene loading, presentation, or Player-build compatibility.
