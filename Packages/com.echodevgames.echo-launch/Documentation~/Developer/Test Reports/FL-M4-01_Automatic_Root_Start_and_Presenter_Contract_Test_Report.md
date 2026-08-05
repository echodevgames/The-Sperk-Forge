# FL-M4-01 - Automatic Root Start and Presenter Contract Test Report

## Report Metadata

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- Checkpoint: `FL-M4-01`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.4.0
- Unity baseline: `6000.3.8f1`
- Implementation commit: `46481b1`
- Test layer: Runtime Play Mode
- Final result: Pass

## Final Totals

- Passed: `396`
- Failed: `0`
- Ignored: `0`
- Compilation errors: `0`
- Compiler warnings: `0`

## New Fixture

`EchoLaunchAutomaticStartAndPresenterTests`

- Passed: `16`
- Failed: `0`
- Ignored: `0`

Verified:

1. Automatic start completes the first enabled launch.
2. Disabled automatic start remains `AuthorityClaimed`.
3. Manual start before Unity `Start` does not re-enter.
4. Injected presenter binds before validation.
5. Presenter receives accepted lifecycle order.
6. Presenter receives the exact finalized report.
7. Missing presenter uses the silent headless fallback.
8. Serialized presenter component is resolved.
9. Invalid assigned presenter warns and launch continues.
10. Presenter bind failure is contained.
11. Presenter progress failure is contained.
12. Presenter terminal failure does not block `LaunchCompleted`.
13. Presenter replacement is rejected after launch advancement.
14. Null presenter injection is rejected.
15. Presenter unbinds once when the root is destroyed.
16. Duplicate root never starts or binds presentation.

## Compile Corrections

Initial compile produced two test-only errors.

### Invalid component type

The test used `AudioSource`, which is a `Behaviour` but not a `MonoBehaviour`. The root’s serialized presenter field is intentionally typed as `MonoBehaviour`.

Correction:

- Added a dedicated `InvalidLaunchStatusPresenterComponent : MonoBehaviour`.
- The component intentionally does not implement `ILaunchStatusPresenter`.

### Unsupported NUnit constraint

The installed NUnit version does not expose `Is.AnyOf`.

Correction:

- Replaced the constraint with a direct boolean assertion covering `Completed`, `Failed`, or `Interrupted`.

No production runtime code changed.

Final compile:

- Errors: `0`
- Compiler warnings: `0`

## Automatic Start Result

Pass:

- Unity `Start` opens the existing `StartLaunchAsync` gate.
- Automatic startup is enabled by default.
- Disabled automatic startup performs no launch work.
- Manual startup before Unity `Start` prevents a second run.
- Duplicate roots do not automatically start.
- Retained manual fixtures remain deterministic through explicit opt-out.

## Presenter Contract Result

Pass:

- Presenter binds once with the authoritative initial snapshot.
- First presented lifecycle snapshot is `Validating`.
- Presenter observes accepted snapshots, not speculative values.
- Presenter receives the exact `LastReport` terminal object.
- Missing presentation is silent and headless-safe.
- Invalid serialized presentation falls back safely.
- Presenter exceptions do not fail or reclassify launch.
- Completion event still dispatches after terminal presenter failure.
- Presenter unbinds once during destruction.
- Duplicate roots receive no presentation callbacks.

## Diagnostic Result

Verified:

- `ELAUNCH-VIEW-001` for an explicitly assigned invalid presenter component.
- `ELAUNCH-VIEW-002` for bind, progress, or terminal callback exceptions.
- Diagnostic exception messages are sanitized.
- Presenter diagnostics are expected runtime warnings, not compiler warnings.

## Assembly Boundary Result

Pass:

- `ILaunchStatusPresenter` lives in the neutral Runtime namespace.
- Runtime stores only a `MonoBehaviour` seam.
- No uGUI, TextMeshPro, Canvas, or presentation-assembly reference was added.
- The headless fallback requires no visual package.

## Regression Result

All retained coverage remained green for:

- Authority and lifecycle
- Configuration and startup sequence
- Runner policy, timeout, cancellation, preflight, and re-entry
- Root-owned execution
- Destination handoff
- Immutable reports
- Failed, interrupted, and completed terminal events
- Asset immutability
- Duplicate-root and destruction containment

## Expected Runtime Diagnostics

Tests intentionally emit:

- `ELAUNCH-ROOT-001`
- `ELAUNCH-EVENT-001`
- `ELAUNCH-VIEW-001`
- `ELAUNCH-VIEW-002`

These are intentional proof, not compiler warnings or test failures.

## Evidence Not Run

- Default uGUI status view
- Canvas/prefab rendering
- Splash presentation
- Test Lab visual proof
- Real Boot-to-destination Standalone Laboratory activation
- Player builds
- Clean-project installation
- External project adoption
- Performance measurements

## Final Decision

FL-M4-01 automated evidence passes.

The implementation may be documented and closed in an adjacent commit.
