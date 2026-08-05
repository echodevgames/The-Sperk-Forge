# FL-M2-03 - Launch Session and Read-Only Progress Surface

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M2-03`
- Status: Complete, pending commit and push
- Unity baseline: `6000.3.8f1`

## Goal

Turn the FL-M2-02 vocabulary into live runtime state owned by the authoritative root without executing startup steps.

## Authorized Files

New:

    Runtime/State/LaunchSession.cs
    Tests/Runtime/PlayMode/LaunchSessionProgressTests.cs

Modified:

    Runtime/Core/EchoLaunchRoot.cs
    Runtime/State/LaunchProgressSnapshot.cs

## Implemented Contract

- One fresh session per authoritative root
- Initial state `AuthorityClaimed`
- Initial immutable progress snapshot
- Read-only public `State`
- Read-only public `Progress`
- Internal controlled snapshot replacement
- Canonical `LaunchProgressSnapshot.Empty`
- Duplicate and stale-root state hiding
- Fresh session after authority replacement

## Validation

`LaunchSession` rejects:

- Undefined launch mode
- Snapshot mode mismatch
- `LaunchStatus.None`

`EchoLaunchRoot.PublishProgress` rejects:

- Non-authoritative roots
- Missing sessions

## Test Evidence

| Test Area | Result |
|---|---|
| Authority creates session | Pass |
| Initial progress canonical | Pass |
| Supplied launch mode preserved | Pass |
| Empty snapshot safe | Pass |
| Duplicate exposes no state | Pass |
| Publication replaces progress | Pass |
| Same-state publication works | Pass |
| Previous snapshot immutable | Pass |
| Mode mismatch rejected | Pass |
| None status rejected | Pass |
| Undefined mode rejected | Pass |
| Duplicate cannot publish | Pass |
| Static reset hides stale session | Pass |
| Fresh authority gets fresh session | Pass |

FL-M2-03 totals:

- Passed: `14`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `60`
- Failed: `0`
- Ignored: `0`

## Explicit Exclusions

Not implemented:

- Public state or progress events
- Startup configuration
- Startup steps
- Executors
- Lifecycle transition rules
- Reports
- Presentation
- Scene loading
- Persistent lifetime
- Direct-scene initializer behavior
- Editor tools
- Test Lab scenes
- Peer-package bridges

## Closure Result

The exact approved session and read-only progress slice compiles and all fourteen new tests pass.

FL-M2-03 is ready for commit and push.

The next runtime checkpoint requires separate approval.
