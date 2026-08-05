# FL-M2-06 - Launch Configuration Identity and Root Binding

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M2-06`
- Implementation status: Complete and pushed
- Implementation commit: `3280472`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Create the smallest project-owned launch configuration definition and bind it passively to the accepted `EchoLaunchRoot` without beginning validation or execution.

## Authorized Files

New:

    Runtime/Configuration/EchoLaunchConfiguration.cs
    Tests/Runtime/PlayMode/LaunchConfigurationBindingTests.cs
    Plan Documentation/Checkpoint Build Plans/FL-M2-06_Launch_Configuration_Identity_and_Root_Binding_Checkpoint_Build_Plan.md

Modified:

    Runtime/Core/EchoLaunchRoot.cs

Unity-generated `.meta` files are part of the authorized asset scope.

## Implemented Contract

### Configuration Definition

- Project-owned `EchoLaunchConfiguration` ScriptableObject
- `CreateAssetMenu` entry under First Light
- Stable canonical configuration identity
- Serialized schema version `1`
- Read-only `ConfigurationId`
- Read-only `SchemaVersion`
- Internal identity validity check
- Internal schema support check
- No mutable launch-session state

### Root Binding

- One serialized configuration reference
- Read-only `EchoLaunchRoot.Configuration`
- Assigned asset visible only through the accepted authority
- Duplicate roots expose `null`
- Former authorities expose `null` after reset
- Missing assignment returns `null`
- Root creation and destruction do not mutate the asset

## Stable Identity Contract

Canonical configuration IDs are:

- 32 characters
- Lowercase hexadecimal
- Free of separators and whitespace

Malformed IDs are detected without automatic repair.

The configuration ID is not the Unity asset GUID, path, filename, display name, or runtime instance ID.

## Schema Contract

The current configuration schema is:

    1

Unsupported schema values are detected but not rewritten.

Runtime migration and repair are outside this checkpoint.

## Test Evidence

FL-M2-06 totals:

- Passed: `15`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `117`
- Failed: `0`
- Ignored: `0`

Verified:

- Canonical generated identity
- Unique identities
- Stable repeated reads
- Current schema initialization
- Valid identity and schema recognition
- Malformed identity preservation
- Unsupported schema preservation
- Authoritative configuration exposure
- Null behavior without assignment
- Duplicate-root hiding
- Authority preservation
- Reset behavior
- Fresh-root rebinding
- Configuration immutability

## Manual Evidence

Unity successfully created:

    Assets/Settings/FL-M2-06_TestLaunchConfiguration.asset

The asset's default Inspector showed only its script reference because identity and schema are hidden.

Creating the asset produced:

- No compile error
- No root or GameObject
- No lifecycle transition
- No startup execution
- No warning

The temporary verification asset was deleted before Git review.

## Expected Diagnostics

Duplicate-root tests intentionally produced:

- `ELAUNCH-ROOT-001`

Retained FL-M2-05 notification tests intentionally produced:

- `ELAUNCH-EVENT-001`

These yellow warnings are expected evidence and are not failures.

## Explicit Exclusions

Not implemented:

- Startup sequence
- Startup steps or executors
- Automatic lifecycle advancement
- Configuration preflight
- Missing-configuration diagnostic
- Runtime migration or repair
- Launch reports
- Presentation
- Scene loading
- Persistent lifetime
- Direct-scene initializer behavior
- Editor tools beyond `CreateAssetMenu`
- Test Lab scenes
- Peer-package bridges

## Closure Result

The configuration identity and root-binding surface compiles and all one hundred seventeen Runtime Play Mode tests pass.

Implementation commit `3280472` is present on `main` and `origin/main`.

FL-M2-06 is ready for its adjacent documentation commit.

The next runtime checkpoint requires separate approval.
