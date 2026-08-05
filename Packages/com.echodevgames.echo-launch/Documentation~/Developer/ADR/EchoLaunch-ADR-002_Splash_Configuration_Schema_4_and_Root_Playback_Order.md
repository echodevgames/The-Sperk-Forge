
# EchoLaunch ADR-002 — Splash Configuration Schema 4 and Root Playback Order

## Metadata

- ADR: `EchoLaunch-ADR-002`
- Status: Approved
- Date: August 5, 2026
- Package: First Light (`EchoLaunch`)
- Package specification: SFGSS-PKG-ECHOLAUNCH-001 v1.5.0
- Checkpoint: FL-M4-04
- Decision owner: Jesse “Echo” Adams / EchoDevGames
- Baseline commit: `b36e04d`

## Context

FL-M4-03 completed standalone project-owned image splash definitions,
deterministic `ILaunchClock` playback, neutral/headless presentation, reduced
motion, minimum-display protection, skip policy, and uGUI projection.

The completed implementation intentionally did not:

- Add a splash reference to `EchoLaunchConfiguration`.
- Advance configuration schema 3.
- Let `EchoLaunchRoot` own splash playback.
- Add splash fields to report schema 2.
- Decide whether splashes overlap startup steps.

FL-M4-04 crosses serialized configuration and authoritative lifecycle
boundaries. Those decisions must be recorded before runtime implementation.

## Decision

### Configuration schema 4

`EchoLaunchConfiguration.CurrentSchemaVersion` advances from `3` to `4`.

Schema 4 adds:

```text
SplashSequence SplashSequence
bool UseReducedMotionForSplash
```

The exact private serialized field casing remains implementation detail. The
public read-only semantic surface uses the names above.

Historical shapes remain:

```text
Schema 2:
    StartupSequence

Schema 3:
    StartupSequence
    InitialDestination

Schema 4:
    StartupSequence
    InitialDestination
    optional SplashSequence
    UseReducedMotionForSplash
```

Runtime accepts only the current schema and never migrates or rewrites assets.

### Optional splash assignment

A null splash reference is intentional configuration:

```text
no assigned sequence
    -> no splash phase
    -> no warning
    -> startup steps begin
```

An assigned empty but otherwise valid sequence is a legal no-op.

An assigned invalid sequence blocks preflight.

### Root playback order

The canonical MVP order is:

```text
claim authority
    -> validate configuration and all assigned assets
    -> bind status/splash presentation
    -> play optional splash sequence
    -> run startup sequence
    -> load initial destination
    -> complete handoff
```

Splash playback and startup-step execution are sequential.

They do not run concurrently.

### Why splash precedes startup steps

This order:

- Matches the approved lifecycle's presentation-before-work boundary.
- Gives one deterministic presentation owner at a time.
- Avoids hidden startup side effects while the splash timeline is still active.
- Makes cancellation and failure attribution unambiguous.
- Keeps root tests reproducible with one monotonic clock.
- Avoids inventing concurrency policy before Standalone Laboratory evidence.

Concurrency may be reconsidered only through a later authority decision.

### Reduced motion

`UseReducedMotionForSplash` is the project-authored default stored in schema 4.

The root passes it to `SplashSequencePlayer`.

This is not a user-settings system. Runtime preference providers and
EchoSettings bridges remain deferred.

### Presentation resolution

When the configured status presenter also implements `IImageSplashPresenter`,
the root uses that same component.

When no visual splash presenter exists:

- The root records `ELAUNCH-SPLASH-003`.
- Playback continues through `NullImageSplashPresenter`.
- Authored timing and minimum display remain deterministic.
- Missing visuals do not transfer launch authority or crash the launch.

### Preflight and diagnostics

| Condition | Outcome | Code |
|---|---|---|
| Null splash reference | Legal omission | None |
| Empty valid sequence | Legal no-op | None |
| Invalid identity/schema/entry/image/timing/duplicate ID | Block before splash, steps, or destination | `ELAUNCH-SPLASH-001` |
| Unexpected playback, clock, or presenter exception | Failed launch before steps/destination | `ELAUNCH-SPLASH-002` |
| Configured splash without visual presenter | Warning, headless deterministic playback | `ELAUNCH-SPLASH-003` |
| Root cancellation/destruction during splash | Interrupted launch | Existing `ELAUNCH-LIFE-001` |
| User skip permitted by entry policy | Successful entry completion | None |

### Cancellation

The player receives the root launch cancellation token.

Cancellation during splash:

- Clears splash presentation.
- Prevents startup steps.
- Prevents destination loading.
- Finalizes one interrupted report.
- Emits the existing exactly-once interrupted terminal event.
- Uses the existing lifecycle diagnostic semantics.

### Reporting

`LaunchReport.CurrentSchemaVersion` remains `2`.

FL-M4-04 does not add:

- Splash sequence ID.
- Presented-entry count.
- Skipped-entry count.
- Reduced-motion flag.
- Splash-specific report collections.

Existing report behavior is sufficient for this checkpoint:

- Total launch elapsed time includes successful splash time.
- Splash failure codes/messages occupy the existing immutable final-result
  surface.
- Failed and interrupted launches still produce one finalized report.

A report schema change requires a separate authority decision.

### Direct-scene development

Direct-scene development uses the same schema-4 configuration and splash rules.

A project that does not want splashes in direct-scene development uses a
development configuration with no assigned splash sequence.

The package does not add hidden mode-specific bypass behavior.

## Rejected alternatives

### Run splash and startup steps concurrently

Rejected for FL-M4-04.

It introduces race conditions around cancellation, warnings, progress,
presentation ownership, and terminal settlement.

### Put splash entries directly in configuration

Rejected.

`SplashSequence` already has independent identity, schema, validation, reuse,
and designer ownership.

### Treat missing splash reference as an error

Rejected.

First Light must remain useful for projects that want startup coordination but
no splash images.

### Skip timing when headless

Rejected.

Headless presentation is a replacement surface, not authority to alter authored
timing.

### Advance report schema now

Rejected.

Root integration can be proven without adding report fields.

### Runtime-migrate schema 3 assets

Rejected.

Runtime remains side-effect-free toward project-owned assets.

## Consequences

### Positive

- One explicit serialized configuration shape.
- One deterministic root order.
- No hidden concurrency.
- Optional splash use.
- Reduced-motion configuration is visible and testable.
- Headless operation remains supported.
- Existing report consumers remain compatible.
- Root cancellation and terminal-event rules remain reusable.

### Costs

- Existing schema-3 assets become unsupported until future Editor migration.
- Splash time occurs before startup steps and therefore extends total launch
  duration.
- Direct-scene projects need a separate configuration when they want no splash.
- Report schema 2 does not expose successful splash metrics.

## Implementation boundary

FL-M4-04 may modify only the configuration binding, splash preflight,
root-owned playback path, lifecycle diagnostics, and focused automated proof
needed for this decision.

It may not implement:

- Editor migration.
- Prefab or Canvas art.
- Project input bindings.
- Report schema changes.
- Concurrent splash/step execution.
- Direct-scene initializer tooling.
- Standalone Laboratory scenes.
- Player-build evidence.

## Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
