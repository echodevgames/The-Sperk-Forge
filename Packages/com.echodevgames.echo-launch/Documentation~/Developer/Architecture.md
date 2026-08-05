# First Light Developer Architecture

## Document Status

- Package version: `0.1.0`
- Development stage: Early runtime implementation
- Completed checkpoints:
  - `FL-M2-01`
  - `FL-M2-02`
  - `FL-M2-03`
  - `FL-M2-04`
  - `FL-M2-05`
  - `FL-M2-06`
  - `FL-M2-07`
- Unity baseline: `6000.3.8f1`

## Current Architecture

First Light currently establishes:

1. Single launch authority
2. Neutral launch-state vocabulary
3. One live session owned by the authoritative root
4. Read-only state and progress exposure
5. Central lifecycle transition validation
6. Isolated lifecycle notifications
7. Project-owned launch configuration identity
8. Authority-filtered root configuration binding
9. Immutable startup-step definitions
10. Ordered startup-sequence entry modeling
11. Passive configuration-to-sequence binding

It does not yet execute startup behavior.

## Implemented Runtime Files

    Runtime/
    ├── Configuration/
    │   └── EchoLaunchConfiguration.cs
    ├── Core/
    │   ├── LaunchAuthorityClaim.cs
    │   └── EchoLaunchRoot.cs
    ├── Events/
    │   ├── LaunchNotificationDispatcher.cs
    │   ├── LaunchProgressChangedEvent.cs
    │   └── LaunchStateChangedEvent.cs
    ├── Properties/
    │   └── AssemblyInfo.cs
    ├── State/
    │   ├── LaunchMode.cs
    │   ├── LaunchStatus.cs
    │   ├── LaunchProgressSnapshot.cs
    │   ├── LaunchSession.cs
    │   └── LaunchStateTransitionRules.cs
    └── Steps/
        ├── StartupSequence.cs
        ├── StartupSequenceEntry.cs
        ├── StartupStepDefinition.cs
        ├── StartupStepStatus.cs
        └── StartupStepResult.cs

    Tests/Runtime/PlayMode/
    ├── EchoLaunchRootAuthorityTests.cs
    ├── LaunchConfigurationBindingTests.cs
    ├── LaunchLifecycleTransitionTests.cs
    ├── LaunchNotificationTests.cs
    ├── LaunchSessionProgressTests.cs
    ├── LaunchStateVocabularyTests.cs
    └── StartupSequenceDefinitionTests.cs

## Launch Configuration Definition

`EchoLaunchConfiguration` is a project-owned `ScriptableObject`.

It contains authored definition data only:

    configurationId
    schemaVersion
    startupSequence

It does not contain current launch state, progress, timings, retries, active scene references, or execution results.

Active mutable state remains owned by `LaunchSession`.

### Configuration Schema

`EchoLaunchConfiguration.CurrentSchemaVersion` is now `2`.

Schema `2` adds the passive serialized `StartupSequence` reference.

The package does not migrate older configuration assets at runtime. Migration and repair remain future Editor-tooling responsibilities.

## Startup Step Definition

`StartupStepDefinition` is an abstract project or bridge-owned ScriptableObject base.

It contains:

    stepId
    schemaVersion
    displayName

It does not contain:

- Executor instances
- Current status
- Progress
- Retry counters
- Timeout state
- Cancellation state
- Runtime ownership
- Scene references
- Result history

### Step Identity

Every newly created step definition receives:

    Guid.NewGuid().ToString("N")

The canonical format is:

- Exactly 32 characters
- Lowercase hexadecimal
- Characters `0-9` and `a-f`
- No spaces, punctuation, or separators

The stable step ID is distinct from asset name, display label, path, list index, Unity asset GUID, and runtime instance ID.

Runtime code detects malformed identity but does not silently repair it.

### Step Display Label

`DisplayName` is presentation metadata, not identity.

Changing the label does not change `StepId`.

A blank authored label falls back to the Unity object name.

### Step Schema

`StartupStepDefinition.CurrentSchemaVersion` is `1`.

Unsupported schema values are detected but not rewritten.

## Startup Sequence Entry

`StartupSequenceEntry` is an embedded serializable authored record.

It contains:

    entryId
    enabled
    stepDefinition

The entry ID is durable identity and remains independent from list index.

Reordering entries changes authored order, not entry identity.

The enabled flag is authored definition data. It does not represent runtime skipped, running, failed, or completed state.

A null step reference is legal authored data at this checkpoint. Validation belongs to later preflight work.

## Startup Sequence

`StartupSequence` is a project-owned ScriptableObject.

It contains:

    sequenceId
    schemaVersion
    ordered List<StartupSequenceEntry>

It exposes:

    SequenceId
    SchemaVersion
    EntryCount
    GetEntry(index)

The mutable backing list is not publicly exposed.

`GetEntry(index)` preserves authored order and throws `ArgumentOutOfRangeException` for invalid positions.

An empty sequence is legal authored data at this checkpoint.

### Sequence Identity and Schema

Sequence IDs use the same canonical 32-character lowercase hexadecimal format.

`StartupSequence.CurrentSchemaVersion` is `1`.

Malformed IDs and unsupported schema values are detected without runtime repair.

## Configuration-to-Sequence Binding

`EchoLaunchConfiguration.StartupSequence` passively exposes the assigned project-owned sequence.

Binding does not:

- Validate the sequence
- Execute entries
- Create step executors
- Advance launch lifecycle
- Repair IDs
- Clone definitions
- Mutate the sequence
- Emit warnings

Preflight and execution remain later checkpoints.

## Definition Immutability

Startup configuration, sequence, entry, and step-definition objects are authored inputs.

Runtime inspection does not alter:

- Configuration ID or schema
- Sequence ID or schema
- Entry ID or enabled state
- Step ID, schema, or display label
- Authored references
- Authored order

Future active execution state must live in fresh runtime-owned objects.

## Lifecycle Transition Authority

`LaunchStateTransitionRules` remains the single internal authority for lifecycle legality.

It validates defined status values and approved transition paths before `LaunchSession` accepts a new snapshot.

## Lifecycle Notifications

`EchoLaunchRoot` continues to dispatch accepted state and progress notifications after authoritative state has changed.

Listener failures remain isolated through `ELAUNCH-EVENT-001`.

## Test Evidence

Runtime Play Mode totals:

- Passed: `141`
- Failed: `0`
- Ignored: `0`

Breakdown:

- Authority tests: `7`
- Configuration binding tests: `15`
- Vocabulary tests: `39`
- Session and progress tests: `14`
- Lifecycle transition tests: `22`
- Lifecycle notification tests: `20`
- Startup sequence definition tests: `24`

Manual verification:

- Unity created a project-owned `StartupSequence` asset.
- The sequence Inspector displayed an empty ordered `Entries` list.
- Unity created a temporary launch configuration and accepted the sequence reference.
- Asset creation and assignment produced no root, GameObject, lifecycle transition, startup behavior, or warning.
- Both temporary verification assets were removed before Git review.

## Current Exclusions

Not implemented:

- Startup-step policy
- Step executor contract
- Startup sequence runner
- Runtime step context
- Automatic lifecycle advancement
- Configuration or sequence preflight
- Duplicate-ID collision validation
- Runtime migration or repair
- Launch reports
- Splash presentation
- Scene loading
- Persistent-root lifetime
- Direct-scene initialization behavior
- Custom inspectors and setup windows
- Standalone Laboratory
- Peer-package bridges

## Stop Point

FL-M2-07 stops after startup-step definitions, ordered sequence entries, sequence identity, and passive configuration binding are proven.

The next runtime slice requires separate approval.
