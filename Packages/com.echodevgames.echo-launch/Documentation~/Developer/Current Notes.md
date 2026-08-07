# First Light - Current Notes

## Latest Completed Checkpoint

- Checkpoint: `FL-M5-06`
- Title: Launch Simulator and Deterministic Failure Injection
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.12.0
- ADR: EchoLaunch-ADR-009
- Authority commit: `a159349`
- Implementation commit: `956c381`
- Documentation closeout: `e28ff09`
- Status: Complete, documented, automated-tested, manually accepted, and pushed
- Compilation revalidated August 7: `0` errors, `0` warnings
- Focused Simulator EditMode: `24` passed
- Complete EditMode revalidated August 7: `290` passed
- Runtime Play Mode revalidated August 7: `503` passed
- Total automated baseline: `793` passed

## Post-Rewind Reconciliation

The active `main` history was intentionally returned to FL-M5-06 closeout commit
`e28ff09`. Later work is not authoritative unless a new approved checkpoint
reintroduces it.

The closeout commit left this living page with stale wording that still said
FL-M5-06 documentation was pending. This reconciliation corrects that status
only. It changes no package implementation, public API, assets, schemas,
diagnostics, manifest, or test behavior.

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

- No automatic Simulator run.
- No Play Mode requirement for Simulator use.
- No Simulator root, splash, presentation, or destination claim.
- Real runner, policy, progress, timeout, exception, and cancellation behavior.
- Transient `HideAndDontSave` authored shape.
- Deterministic logical timing and fingerprints.
- Separate truthful simulation report.
- One active run and cooperative cancellation.
- No authored asset, scene, Build Settings, or ProjectSettings mutation.
- No Simulator implementation in Runtime/player assemblies.
- Standalone Laboratory remains separate and unauthorized until the next authority checkpoint is approved.

## Accepted Determinism Correction

Manual cancellation initially exposed human-click-dependent elapsed evidence.
The final implementation normalizes Simulator cancellation elapsed to zero,
preserves canonical `ELAUNCH-STEP-005`, removes `ElapsedSeconds:` from copied
cancellation evidence, and produces repeatable report fingerprints.

Accepted cancellation report fingerprint:

```text
e92b028d7798ec597894213539e3ae19b113931e714ef29bae6d8d11bb92362b
```

## Fresh Baseline Evidence — August 7, 2026

```text
Compilation: 0 errors / 0 warnings
EditMode:    290 passed / 0 failed / 0 ignored
PlayMode:    503 passed / 0 failed / 0 ignored
Total:       793 passed
```

## Next Action

Draft and approve a fresh FL-M5-07 Standalone Test Laboratory checkpoint from
the reconciled FL-M5-06 baseline. No Laboratory implementation is authorized by
this reconciliation alone.
