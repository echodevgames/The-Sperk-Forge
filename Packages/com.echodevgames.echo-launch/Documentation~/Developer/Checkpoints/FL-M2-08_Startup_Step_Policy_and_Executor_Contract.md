# FL-M2-08 - Startup Step Policy and Executor Contract

## Checkpoint Metadata

- Package: First Light (`EchoLaunch`)
- Package ID: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Checkpoint: `FL-M2-08`
- Implementation status: Complete and pushed
- Implementation commit: `8a02bd8`
- Documentation closeout: Pending adjacent commit
- Unity baseline: `6000.3.8f1`

## Goal

Define authored startup-step policy and the fresh single-use executor contract without invoking a step, running a sequence, measuring timeout, applying policy, or advancing lifecycle.

## Authorized Files

New:

    Runtime/Steps/StartupStepFailureAction.cs
    Runtime/Steps/StartupStepPolicy.cs
    Runtime/Steps/StartupStepProgress.cs
    Runtime/Steps/IStartupStepProgressReporter.cs
    Runtime/Steps/StartupStepContext.cs
    Runtime/Steps/IStartupStepExecutor.cs
    Tests/Runtime/PlayMode/StartupStepPolicyAndExecutorContractTests.cs
    Plan Documentation/Checkpoint Build Plans/FL-M2-08_Startup_Step_Policy_and_Executor_Contract_Checkpoint_Build_Plan.md

Modified:

    Runtime/Steps/StartupStepDefinition.cs
    Runtime/Steps/StartupSequenceEntry.cs
    Runtime/Steps/StartupSequence.cs
    Tests/Runtime/PlayMode/StartupSequenceDefinitionTests.cs

Unity-generated `.meta` files are part of the authorized asset scope.

## Implemented Contract

### Failure Vocabulary

Exactly two MVP values:

    BlockLaunch
    ContinueWithWarning

Retry is not represented.

### Authored Policy

`StartupStepPolicy` contains:

- Required or optional intent
- Failure action
- Timeout seconds
- Cancellation support

Safe presets:

- `RequiredBlocking`
- `OptionalWarning`

Invalid values are detected without runtime repair.

### Progress

`StartupStepProgress` supports:

- Determinate progress from zero through one
- Indeterminate progress
- Normalized messages
- Immutable reads

### Progress Reporting

`IStartupStepProgressReporter` provides one package-owned `Report` seam.

### Execution Context

`StartupStepContext` provides immutable validated:

- Launch mode
- Configuration ID
- Sequence ID
- Entry ID
- Step ID
- Step index
- Step count
- Cancellation token
- Progress reporter

### Executor Contract

`IStartupStepExecutor` exposes:

    Awaitable<StartupStepResult> ExecuteAsync(
        StartupStepContext context)

`StartupStepDefinition` exposes:

    CreateExecutor()

Every valid factory call must create a fresh executor.

No executor is invoked by this checkpoint.

### Entry Policy

Every sequence entry contains one authored policy.

The returned policy is a value copy.

### Sequence Schema

`StartupSequence.CurrentSchemaVersion` advanced:

    1 -> 2

Schema `2` adds policy data to embedded entries.

## Unity Serialized Default Correction

Manual Inspector verification discovered that Unity-created embedded list elements can arrive as zeroed serialized data without applying field initializers.

The serialized model now deliberately maps zero to:

- Activation: Enabled
- Requirement: Required
- Failure action: Block Launch
- Timeout: Disabled
- Cancellation: Supported

No repair callback, `OnValidate`, migration, or runtime rewrite was introduced.

## Test Evidence

FL-M2-08 totals:

- Passed: `28`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `169`
- Failed: `0`
- Ignored: `0`

Verified:

- Exact failure-action enum
- Required policy preset
- Optional policy preset
- Blocking and warning failure actions
- Zero and positive timeout metadata
- Invalid negative and non-finite timeout values
- Undefined failure-action preservation
- Determinate progress
- Inclusive progress boundaries
- Indeterminate progress
- Progress range rejection
- Message normalization
- Context identity data
- Step index and count
- Cancellation token
- Progress reporter
- Null reporter rejection
- Executor return type
- Executor creation
- Fresh executor instances
- Safe entry defaults
- Sequence schema `2`

## Manual Evidence

Unity successfully created:

    Assets/Settings/FL-M2-08_TestStartupSequence.asset

Initial observation:

- Unity added a zeroed embedded entry.
- Boolean field initializers were not applied.
- Enabled, Required, and Supports Cancellation appeared false.

Bounded correction:

- Replaced unsafe booleans with zero-valued serialized enums.
- Recreated the temporary sequence.
- Verified safe defaults:
  - Enabled
  - Required
  - Block Launch
  - Timeout `0`
  - Cancellation Supported

No executor, runner, timeout, retry, lifecycle transition, preflight, or warning occurred.

The temporary asset was deleted before Git review.

## Expected Diagnostics

Retained tests intentionally produced:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`

These yellow warnings are expected evidence and are not failures.

## Explicit Exclusions

Not implemented:

- Sequence runner
- Active execution tracking
- Executor invocation
- Timeout measurement
- Clock abstraction
- Timeout cancellation
- Retry loop
- Interactive retry
- Exception conversion
- Policy application to step results
- Configuration or sequence preflight
- Duplicate-ID validation
- Automatic lifecycle advancement
- Step events
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

The startup-step policy and executor contracts compile and all one hundred sixty-nine Runtime Play Mode tests pass.

Implementation commit `8a02bd8` is present on `main` and `origin/main`.

FL-M2-08 is ready for its adjacent documentation commit.

The next runtime checkpoint requires separate approval.
