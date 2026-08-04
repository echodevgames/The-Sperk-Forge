# FL-M2-01 Authority Runtime Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode

## Result

- Passed: `7`
- Failed: `0`
- Ignored: `0`

## Tests

1. `FirstRootClaimsAuthority`
2. `SecondRootIsRejectedWithoutReplacingAuthority`
3. `DestroyingDuplicateDoesNotReleaseAuthority`
4. `DestroyingAuthorityReleasesClaim`
5. `ResetClearsCurrentAuthority`
6. `FreshRootCanClaimAfterReset`
7. `DestroyedAuthorityAllowsFreshRootToClaim`

## Diagnostic Evidence

Two tests intentionally created a duplicate root.

Expected warning:

    [ELAUNCH-ROOT-001] Duplicate EchoLaunchRoot rejected. The first valid root remains authoritative.

The warnings were registered with `LogAssert.Expect` and did not count as unexpected test failures.

## Verified Contracts

- First valid root owns authority.
- Duplicate root cannot replace authority.
- Duplicate root disables itself.
- Duplicate destruction cannot release authority.
- Authority destruction releases the claim.
- Static reset clears stale authority.
- A fresh authority can claim after reset.
- Deferred Unity destruction follows the same release contract.

## Scope Limit

This report proves only FL-M2-01 authority behavior.

It does not prove startup sequencing, presentation, scene loading, persistence, direct-scene initialization, or Player-build compatibility.
