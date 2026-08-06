# First Light - Current Notes

## Latest Completed Checkpoint

- Checkpoint: `FL-M5-06`
- Title: Launch Simulator and Deterministic Failure Injection
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.12.0
- ADR: EchoLaunch-ADR-009
- Authority commit: `a159349`
- Implementation commit: `956c381`
- Documentation closeout: pending
- Status: Implemented, automated-tested, manually accepted, and pushed
- Compilation: `0` errors, `0` warnings
- Focused Simulator EditMode: `24` passed
- Complete EditMode: `290` passed
- Runtime Play Mode: `503` passed
- Total automated: `793` passed

## Implemented Outcome

The explicit Editor-only Simulator runs transient deterministic startup-step
scenarios through the real sequence runner and produces an immutable copyable
schema-1 simulation report.

Accepted presets:

- Immediate success
- Timed progress success
- Warning continuation
- Recoverable-failure continuation
- Blocking failure
- Timeout
- Executor exception
- Cancellation

## Retained Boundary

- No automatic run.
- No Play Mode requirement.
- No root, splash, presentation, or destination claim.
- Real runner, policy, progress, timeout, exception, and cancellation behavior.
- Transient `HideAndDontSave` authored shape.
- Deterministic logical timing and fingerprints.
- Separate truthful simulation report.
- One active run and cooperative cancellation.
- No authored asset, scene, Build Settings, or ProjectSettings mutation.
- No Simulator implementation in Runtime/player assemblies.
- Standalone Laboratory remains separate.

## Accepted Determinism Correction

Manual cancellation initially exposed human-click-dependent elapsed evidence.
The final implementation normalizes Simulator cancellation elapsed to zero,
preserves canonical `ELAUNCH-STEP-005`, removes `ElapsedSeconds:` from copied
cancellation evidence, and produces repeatable report fingerprints.

Accepted cancellation report fingerprint:

```text
e92b028d7798ec597894213539e3ae19b113931e714ef29bae6d8d11bb92362b
```

## Next Action

Apply, review, commit, and push the FL-M5-06 documentation closeout.

No later checkpoint is authorized.
