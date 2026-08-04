# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoint: `FL-M2-01`
- Implemented slice: Authority Claim and Static Reset Core
- Unity baseline: `6000.3.8f1`

## Package Responsibility

First Light coordinates application startup.

The current implementation establishes only the single-authority foundation required before startup behavior can safely exist.

## Implemented Runtime Files

    Runtime/
    ├── Core/
    │   ├── LaunchAuthorityClaim.cs
    │   └── EchoLaunchRoot.cs
    └── Properties/
        └── AssemblyInfo.cs

    Tests/Runtime/
    └── PlayMode/
        └── EchoLaunchRootAuthorityTests.cs

## `LaunchAuthorityClaim`

`LaunchAuthorityClaim` is an internal static kernel responsible for:

- Holding the current Unity authority object
- Claiming authority
- Rejecting different candidates
- Allowing the same owner to re-confirm its claim
- Releasing only when the caller is the actual owner
- Resetting static state during subsystem registration

It deliberately knows nothing about:

- Startup configuration
- Startup sequencing
- UI
- Audio
- Saving
- Scene destinations
- Other Echo packages

## `EchoLaunchRoot`

`EchoLaunchRoot` is the public scene-facing component.

Public surface:

    public static EchoLaunchRoot Current { get; }
    public bool IsAuthoritative { get; }
    public bool WasRejectedAsDuplicate { get; }

Duplicate behavior occurs in `Awake`:

1. Attempt the authority claim.
2. If successful, remain enabled.
3. If rejected, record duplicate state.
4. Disable the component.
5. Emit `ELAUNCH-ROOT-001`.

The duplicate is disabled before any future startup behavior could execute.

## Authority Release

`OnDestroy` asks the claim kernel to release authority.

The kernel releases only when the destroyed object is the current owner.

Therefore:

- Destroying the authority clears `Current`.
- Destroying a duplicate leaves the authority untouched.

## Static Reset

The claim kernel uses:

    RuntimeInitializeLoadType.SubsystemRegistration

This clears stale static authority when Unity registers runtime subsystems, including Play Mode configurations where domain reload is disabled.

## Unity Object Null Handling

The kernel stores a `UnityEngine.Object`.

Its `Current` property normalizes Unity's destroyed-object null behavior so callers receive `null` when the stored Unity object is no longer alive.

## Test Access Boundary

`AssemblyInfo.cs` grants internal access only to:

    EchoDevGames.EchoLaunch.Tests.Runtime

This allows tests to verify claim, release, and reset behavior without exposing test-control methods to game projects.

## Verified Runtime Tests

Seven Runtime Play Mode tests pass:

- `FirstRootClaimsAuthority`
- `SecondRootIsRejectedWithoutReplacingAuthority`
- `DestroyingDuplicateDoesNotReleaseAuthority`
- `DestroyingAuthorityReleasesClaim`
- `ResetClearsCurrentAuthority`
- `FreshRootCanClaimAfterReset`
- `DestroyedAuthorityAllowsFreshRootToClaim`

Result:

- Passed: `7`
- Failed: `0`
- Ignored: `0`

## Current Exclusions

Not implemented:

- Startup configuration assets
- Startup sequences
- Step definitions or executors
- Launch sessions or reports
- Progress snapshots
- Splash presentation
- Scene loading
- Persistent-root lifetime policy
- Direct-scene initialization
- Editor setup tools
- Standalone Laboratory
- Peer-package bridges

## Checkpoint Stop Point

FL-M2-01 stops after authority claiming, duplicate rejection, release, static reset, and automated tests.

The next runtime slice requires separate approval.
