# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-06`
- Title: Launch Simulator and Deterministic Failure Injection
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.12.0
- ADR: EchoLaunch-ADR-009
- Authority baseline: `b6df92d`
- Previous authority: `d538b5a`
- Previous implementation: `4aa6ce7`
- Previous documentation: `b6df92d`
- Status: Authority prepared; implementation locked
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `266` passed
- Runtime Play Mode baseline: `503` passed
- Total automated baseline: `769` passed

## Approved Outcome

An explicit Editor-only Simulator runs transient deterministic startup-step
scenarios through the real sequence runner and produces one immutable copyable
schema-1 simulation report.

## Approved Boundary

- No automatic run.
- No Play Mode requirement.
- No root, splash, presentation, or destination work.
- Real runner, policy, progress, timeout, exception, and cancellation behavior.
- Transient `HideAndDontSave` authored shape.
- Deterministic logical timing.
- Separate truthful simulation report.
- One active run and cooperative cancellation.
- No authored asset, scene, Build Settings, or ProjectSettings mutation.
- No Simulator implementation in Runtime/player assemblies.
- Standalone Laboratory remains separate.

## Next Action

Commit and push:

```text
Approve FL-M5-06 launch simulator authority
```

Then implement only ADR-009 and the FL-M5-06 Checkpoint Build Plan.
