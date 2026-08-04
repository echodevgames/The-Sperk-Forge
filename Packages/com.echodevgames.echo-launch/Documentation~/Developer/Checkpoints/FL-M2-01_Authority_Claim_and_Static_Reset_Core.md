# FL-M2-01 - Authority Claim and Static Reset Core

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M2-01`
- Status: Complete, pending commit and push
- Unity baseline: `6000.3.8f1`

## Goal

Establish the smallest safe runtime foundation for First Light:

- One launch authority
- Immediate duplicate rejection
- Owner-only release
- Static reset
- Automated evidence

## Authorized Files

    Runtime/Core/LaunchAuthorityClaim.cs
    Runtime/Core/EchoLaunchRoot.cs
    Runtime/Properties/AssemblyInfo.cs
    Tests/Runtime/PlayMode/EchoLaunchRootAuthorityTests.cs

Unity-generated `.meta` files and the Unity solution update are adjacent implementation artifacts.

## Implemented Public Surface

    public static EchoLaunchRoot Current { get; }
    public bool IsAuthoritative { get; }
    public bool WasRejectedAsDuplicate { get; }

## Duplicate Policy

The first valid root claims authority.

A later root:

- Is marked rejected
- Is disabled
- Logs `ELAUNCH-ROOT-001`
- Cannot replace the real authority
- Cannot release the real authority when destroyed

## Reset Policy

Static authority resets using Unity subsystem registration.

This protects Play Mode workflows where domain reload may be disabled.

## Test Evidence

| Test | Result |
|---|---|
| First root claims authority | Pass |
| Second root rejected without replacement | Pass |
| Duplicate destruction preserves authority | Pass |
| Authority destruction releases claim | Pass |
| Reset clears authority | Pass |
| Fresh root claims after reset | Pass |
| Deferred destruction permits fresh claim | Pass |

Totals:

- Passed: `7`
- Failed: `0`
- Ignored: `0`

Two duplicate warnings were expected and validated through `LogAssert.Expect`.

## Explicit Exclusions

Not implemented:

- Configuration
- Startup steps
- Executors
- Sessions
- Reports
- Progress
- Presentation
- Scene loading
- Persistent lifetime
- Direct-scene mode
- Editor setup
- Standalone Laboratory
- Bridges

## Closure Result

The exact approved runtime slice compiles and all seven tests pass.

FL-M2-01 is ready for commit and push.

The next runtime checkpoint requires separate approval.
