# FL-M2-07 Startup Sequence Definition Test Report

## Environment

- Unity: `6000.3.8f1`
- Package: `com.echodevgames.echo-launch`
- Package version: `0.1.0`
- Assembly: `EchoDevGames.EchoLaunch.Tests.Runtime`
- Mode: Play Mode
- Implementation commit: `38b03b1`

## Result

FL-M2-07 startup-sequence definition tests:

- Passed: `24`
- Failed: `0`
- Ignored: `0`

Full Runtime Play Mode suite:

- Passed: `141`
- Failed: `0`
- Ignored: `0`

## Verified Areas

- Canonical step ID format
- Unique step IDs
- Stable step ID reads
- Current step schema
- Display label preservation
- Display label independence from step identity
- Malformed step ID detection without repair
- Unsupported step schema detection without rewrite
- Canonical entry ID format
- Unique entry IDs
- Default enabled entry state
- Preserved step-definition reference
- Malformed entry ID detection without repair
- Canonical sequence ID format
- Unique sequence IDs
- Stable sequence ID reads
- Current sequence schema
- Valid generated sequence identity and schema
- Malformed sequence ID detection without repair
- Unsupported sequence schema detection without rewrite
- Empty sequence behavior
- Authored-order preservation
- Invalid-index range rejection
- Configuration-to-sequence binding
- Definition immutability

## Manual Verification

Unity created a startup sequence through:

    Assets
        -> Create
            -> EchoDevGames
                -> First Light
                    -> Startup Sequence

The temporary sequence asset showed an empty `Entries` list.

Unity also created a temporary launch configuration, and its `Startup Sequence` field accepted the sequence reference.

Observed:

- Zero compiler errors
- No scene object creation
- No root creation
- No lifecycle transition
- No startup execution
- No unexpected warning

Both temporary assets were removed before Git staging.

## Diagnostic Evidence

Retained duplicate-root tests intentionally generated:

    [ELAUNCH-ROOT-001] Duplicate EchoLaunchRoot rejected. The first valid root remains authoritative.

Retained notification tests intentionally generated:

    [ELAUNCH-EVENT-001] Listener failure while dispatching the lifecycle notification.

The expected warnings were registered by automated tests and did not count as failures.

## Scope Limit

This report proves only FL-M2-07 step-definition identity, sequence-entry modeling, sequence ordering, passive configuration binding, retained runtime behavior, and the manual authoring path.

It does not prove step policies, executor creation, sequence execution, preflight, duplicate-ID collision scanning, launch reports, presentation, scene loading, Player-build compatibility, clean-project installation, migration tooling, or performance budgets.
