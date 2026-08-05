# FL-M2-07 - Startup Sequence Definition and Ordered Entry Model

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M2-07`
- Implementation status: Complete and pushed
- Implementation commit: `38b03b1`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Create the immutable authored model for one ordered startup sequence without implementing policy, executors, a runner, preflight, or lifecycle advancement.

## Authorized Files

New:

    Runtime/Steps/StartupStepDefinition.cs
    Runtime/Steps/StartupSequenceEntry.cs
    Runtime/Steps/StartupSequence.cs
    Tests/Runtime/PlayMode/StartupSequenceDefinitionTests.cs
    Plan Documentation/Checkpoint Build Plans/FL-M2-07_Startup_Sequence_Definition_and_Ordered_Entry_Model_Checkpoint_Build_Plan.md

Modified:

    Runtime/Configuration/EchoLaunchConfiguration.cs

Unity-generated `.meta` files are part of the authorized asset scope.

## Implemented Contract

### Startup Step Definition

- Abstract immutable ScriptableObject base
- Stable canonical step ID
- Step-definition schema version `1`
- Authored display label separate from identity
- Read-only public identity, schema, and label
- Internal identity and schema support checks
- No execution behavior or mutable runtime state

### Startup Sequence Entry

- Embedded serializable record
- Stable canonical entry ID
- Authored enabled state
- One `StartupStepDefinition` reference
- Read-only public access
- Entry identity independent from list index

### Startup Sequence

- Project-owned ScriptableObject
- Stable canonical sequence ID
- Sequence schema version `1`
- Ordered embedded entry list
- Read-only `EntryCount`
- Read-only indexed `GetEntry`
- Clear range rejection
- No public mutable backing list

### Configuration Binding

- `EchoLaunchConfiguration` schema advanced from `1` to `2`
- One passive serialized `StartupSequence` reference
- Read-only `StartupSequence` property
- No validation, execution, migration, repair, or warning behavior

## Identity Contract

Step, entry, and sequence IDs are:

- 32 characters
- Lowercase hexadecimal
- Free of separators and whitespace
- Independent from asset names, display labels, paths, and list positions

Malformed IDs are detected without automatic repair.

## Ordered Entry Contract

List position controls authored order.

List index is not durable identity.

Reordering an entry changes its position but does not change its serialized `EntryId`.

The backing list remains private.

## Schema Contract

- Startup-step definition schema: `1`
- Startup-sequence schema: `1`
- Launch-configuration schema: `2`

Unsupported schema values are detected but not rewritten.

Runtime migration remains outside this checkpoint.

## Test Evidence

FL-M2-07 totals:

- Passed: `24`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `141`
- Failed: `0`
- Ignored: `0`

Verified:

- Canonical step, entry, and sequence IDs
- Unique IDs across separate instances
- Stable repeated reads
- Current schema initialization
- Display label separation from step identity
- Malformed identity preservation
- Unsupported schema preservation
- Entry enabled default
- Entry step-reference preservation
- Empty-sequence behavior
- Authored-order preservation
- Invalid-index rejection
- Configuration-to-sequence binding
- Definition immutability

## Manual Evidence

Unity successfully created:

    Assets/Settings/FL-M2-07_TestStartupSequence.asset
    Assets/Settings/FL-M2-07_TestLaunchConfiguration.asset

The sequence Inspector showed an empty `Entries` list.

The configuration Inspector showed one `Startup Sequence` field and accepted the temporary sequence reference.

Creating and assigning the assets produced:

- No compile error
- No root or GameObject
- No lifecycle transition
- No startup execution
- No unexpected warning

Both temporary assets were deleted before Git review.

## Expected Diagnostics

Retained tests intentionally produced:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These yellow warnings are expected evidence and are not failures.

## Explicit Exclusions

Not implemented:

- Startup-step policy
- `IStartupStepExecutor`
- Runtime step context
- Sequence runner
- Automatic lifecycle advancement
- Configuration or sequence preflight
- Duplicate-ID collision scans
- Runtime migration or repair
- Launch reports
- Presentation
- Scene loading
- Persistent lifetime
- Direct-scene initializer behavior
- Custom inspectors
- Setup windows
- Test Lab scenes
- Peer-package bridges

## Closure Result

The startup-sequence definition surface compiles and all one hundred forty-one Runtime Play Mode tests pass.

Implementation commit `38b03b1` is present on `main` and `origin/main`.

FL-M2-07 is ready for its adjacent documentation commit.

The next runtime checkpoint requires separate approval.
