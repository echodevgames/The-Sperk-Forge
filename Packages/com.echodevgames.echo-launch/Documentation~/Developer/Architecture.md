# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
  - `FL-M2-03`
- Unity baseline: `6000.3.8f1`

## Current Architecture

First Light currently establishes:

1. Single launch authority
2. Neutral launch-state vocabulary
3. One live session owned by the authoritative root
4. Read-only state and progress exposure

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
    │   └── LaunchSession.cs
    └── Steps/
        ├── StartupStepStatus.cs
        └── StartupStepResult.cs

    Tests/Runtime/PlayMode/
    ├── EchoLaunchRootAuthorityTests.cs
    ├── LaunchStateVocabularyTests.cs
    └── LaunchSessionProgressTests.cs

## Launch Session

`LaunchSession` is an internal sealed class representing one launch attempt.

It owns:

- Launch mode
- Current launch state
- Latest immutable progress snapshot

A new session begins with:

- Configured mode
- `AuthorityClaimed`
- No active step
- Zero total steps
- Zero progress
- Indeterminate progress
- Message `Launch authority claimed.`
- Zero elapsed time
- No last result

## Progress Publication

`LaunchSession.Publish` replaces the stored snapshot.

It rejects:

- A snapshot with a different launch mode
- A snapshot using `LaunchStatus.None`

`EchoLaunchRoot.PublishProgress` remains internal and rejects publication when the root is not authoritative or has no session.

## Read-Only Root Surface

`EchoLaunchRoot` publicly exposes:

    public LaunchStatus State { get; }
    public LaunchProgressSnapshot Progress { get; }

Only the current authority may expose live session state.

Duplicate roots, stale roots after static reset, and roots without a session expose:

    LaunchStatus.None
    LaunchProgressSnapshot.Empty

## Empty Snapshot

`LaunchProgressSnapshot.Empty` is a normalized constructed value.

It avoids `default(LaunchProgressSnapshot)`, whose string properties could otherwise be null.

## Session Lifetime

The authoritative root creates one session immediately after claiming authority.

Destroying the root discards that session and releases authority.

A replacement root receives a completely fresh session.

Static reset hides stale private session data because non-authoritative roots expose only `None` and `Empty`.

## Test Evidence

Runtime Play Mode totals:

- Passed: `60`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Vocabulary tests: `39`
- Session and progress tests: `14`

## Current Exclusions

Not implemented:

- Startup configuration assets
- Startup sequences
- Step definitions or executors
- Lifecycle transition validation
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

FL-M2-03 stops after one authority owns one fresh session and exposes read-only state and progress.

The next runtime slice requires separate approval.
