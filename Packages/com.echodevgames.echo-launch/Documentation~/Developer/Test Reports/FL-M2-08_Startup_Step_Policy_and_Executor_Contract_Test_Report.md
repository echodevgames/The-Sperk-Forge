# FL-M2-08 Startup Step Policy and Executor Contract Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode
- Implementation commit: `8a02bd8`

## Result

FL-M2-08 policy and executor-contract tests:

- Passed: `28`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `169`
- Failed: `0`
- Ignored: `0`

## Verified Areas

- Failure enum contains only approved MVP actions
- Required preset is required
- Required preset blocks launch
- Required preset has no timeout
- Required preset supports cancellation
- Optional preset is optional
- Optional preset continues with warning
- Positive timeout is enabled and preserved
- Zero timeout is disabled
- Negative timeout is invalid without repair
- NaN timeout is invalid without repair
- Infinite timeout is invalid without repair
- Undefined failure action is invalid without rewrite
- Determinate progress preserves values
- Progress accepts zero and one
- Indeterminate progress avoids invented percentage
- Progress below zero is rejected
- Progress above one is rejected
- Progress messages are normalized
- Context identity data is preserved
- Context index and count are preserved
- Context cancellation token is preserved
- Context reporter receives progress
- Null reporter is rejected
- Executor method returns `Awaitable<StartupStepResult>`
- Definition factory produces an executor
- Repeated factory calls produce distinct executors
- New entries use safe default policy
- Sequence schema is `2`
- Older sequence schema remains unsupported without rewrite

## Executor Invocation Status

No startup executor was invoked by the new suite.

The test-only executor exists only to satisfy and inspect the factory contract.

Execution behavior remains outside FL-M2-08.

## Manual Verification

Unity created a temporary startup sequence and one embedded entry.

Initial result:

- Enabled: false
- Required: false
- Supports Cancellation: false

This showed that Unity created the embedded list element from zeroed serialized data rather than applying C# field initializers.

The serialized model was changed so zero maps to safe defaults.

After deleting and recreating the temporary asset:

- Activation: Enabled
- Step Definition: None
- Requirement: Required
- Failure Action: Block Launch
- Timeout Seconds: `0`
- Cancellation: Supported

Observed throughout:

- Zero compiler errors
- No root creation
- No executor invocation
- No sequence execution
- No lifecycle transition
- No timeout or retry behavior
- No unexpected warning

The temporary asset was removed before Git staging.

## Diagnostic Evidence

Retained duplicate-root tests intentionally generated:

    [ELAUNCH-ROOT-001] Duplicate EchoLaunchRoot rejected. The first valid root remains authoritative.

Retained notification tests intentionally generated:

    [ELAUNCH-EVENT-001] Listener failure while dispatching the lifecycle notification.

The expected warnings were registered by automated tests and did not count as failures.

## Scope Limit

This report proves only FL-M2-08 authored policy, progress/context value contracts, executor API shape, fresh factory behavior, safe serialized entry defaults, retained runtime behavior, and manual Inspector authoring.

It does not prove sequence execution, timeout measurement, cancellation handling by a runner, retries, exception conversion, policy application, preflight, launch reports, presentation, scene loading, Player-build compatibility, clean-project installation, migration tooling, or performance budgets.
