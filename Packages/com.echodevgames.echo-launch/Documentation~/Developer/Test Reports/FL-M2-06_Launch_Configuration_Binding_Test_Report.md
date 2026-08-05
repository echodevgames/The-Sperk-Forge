# FL-M2-06 Launch Configuration Binding Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode
- Implementation commit: `3280472`

## Result

FL-M2-06 configuration-binding tests:

- Passed: `15`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `117`
- Failed: `0`
- Ignored: `0`

## Verified Areas

- Canonical lowercase hexadecimal identity
- Thirty-two-character identity length
- Unique identity generation
- Stable repeated identity reads
- Current schema version initialization
- Valid generated identity
- Supported generated schema
- Malformed identity detection without repair
- Unsupported schema detection without rewrite
- Authoritative root configuration exposure
- Null behavior without configuration assignment
- Duplicate-root configuration hiding
- Duplicate creation preserving authority configuration
- Former-authority hiding after reset
- Fresh-root configuration after reset
- Configuration immutability through root creation and destruction

## Manual Verification

Unity created a launch configuration through:

    Assets
        -> Create
            -> EchoDevGames
                -> First Light
                    -> Launch Configuration

The temporary asset was created under `Assets/Settings`.

Observed:

- Zero compiler errors
- Nearly empty default Inspector
- No scene object creation
- No lifecycle transition
- No startup execution
- No unexpected warning

The temporary asset was removed before Git staging.

## Diagnostic Evidence

Duplicate-root tests intentionally generated:

    [ELAUNCH-ROOT-001] Duplicate EchoLaunchRoot rejected. The first valid root remains authoritative.

Retained notification tests intentionally generated:

    [ELAUNCH-EVENT-001] Listener failure while dispatching the lifecycle notification.

The expected warnings were registered by automated tests and did not count as failures.

## Scope Limit

This report proves only FL-M2-06 configuration identity, passive root binding, retained runtime behavior, and the manual Create menu path.

It does not prove configuration preflight, startup sequence execution, launch reports, presentation, scene loading, Player-build compatibility, clean-project installation, migration tooling, or performance budgets.
