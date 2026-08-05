
# FL-M4-04 — Splash Configuration Schema and Root Playback Integration

**Document ID:** FL-M4-04
**Version:** 1.0.0
**Status:** Approved; runtime implementation locked until authority commit
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.5.0
**ADR:** EchoLaunch-ADR-002
**Milestone:** M4 — Startup Entry and Presentation
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Unity baseline:** Unity 6000.3.8f1
**Starting implementation commit:** `f997a9a`
**Starting documentation commit:** `b36e04d`
**Starting Runtime Play Mode:** 450 passed, 0 failed, 0 ignored
**Starting compilation:** 0 errors, 0 compiler warnings
**Authorized:** August 5, 2026

> The splash system already knows how to perform. This checkpoint gives the
> authoritative root the sheet music without letting two orchestras play at
> once.

---

## 1. Purpose

FL-M4-04 binds the completed standalone splash system into the project-owned
launch configuration and authoritative root lifecycle.

The checkpoint advances configuration schema 3 to 4, validates the optional
sequence during root preflight, plays it before startup steps, preserves
cancellation and exactly-once terminal settlement, and leaves report schema 2
unchanged.

---

## 2. Approved observable outcome

When complete:

1. `EchoLaunchConfiguration.CurrentSchemaVersion` is `4`.
2. Configuration exposes optional `SplashSequence`.
3. Configuration exposes `UseReducedMotionForSplash`.
4. Schema 3 assets are unsupported at runtime.
5. Null splash assignment is a legal omission.
6. Empty valid sequence is a legal no-op.
7. Assigned invalid sequence blocks before splash, steps, and destination.
8. Root resolves visual or headless splash presentation.
9. Root plays splash before startup steps.
10. Startup steps do not overlap splash playback.
11. Successful splash clears before step presentation.
12. Splash cancellation interrupts the launch exactly once.
13. Splash playback failure finalizes one failed report.
14. Missing splash visuals warn and continue headless.
15. Duplicate roots perform no splash side effect.
16. Automatic start uses the same guarded root path.
17. Direct-scene mode follows the same configuration contract.
18. Destination loading cannot begin before splash and steps settle.
19. Total report elapsed time includes splash time.
20. Report schema remains `2`.
21. All retained tests remain green.
22. Configuration and splash assets remain immutable.

---

## 3. Authorized runtime contract

### Configuration schema 4

Add serialized:

```csharp
SplashSequence splashSequence;
bool useReducedMotionForSplash;
```

Expose read-only:

```csharp
SplashSequence SplashSequence { get; }
bool UseReducedMotionForSplash { get; }
```

Runtime does not support or mutate schema 3 assets.

### Root phase order

```text
preflight
    -> bind presentation
    -> optional splash
    -> startup sequence
    -> destination transition
    -> completed handoff
```

No splash/step concurrency.

### Presenter resolution

```text
configured presenter implements IImageSplashPresenter
    -> use it

otherwise
    -> record ELAUNCH-SPLASH-003
    -> use NullImageSplashPresenter
```

### Failure conversion

```text
invalid assigned sequence
    -> ELAUNCH-SPLASH-001
    -> Failed

unexpected playback/presenter/clock exception
    -> ELAUNCH-SPLASH-002
    -> Failed

root cancellation
    -> ELAUNCH-LIFE-001
    -> Interrupted
```

No startup step or destination side effect follows a splash failure or
interruption.

### Reporting

Report schema remains `2`.

Existing fields carry:

- Total elapsed launch time.
- Final status.
- Final diagnostic code/message.
- Existing startup step reports.

No splash-specific report fields are authorized.

---

## 4. Expected implementation files

### Modified Runtime

- `Runtime/Configuration/EchoLaunchConfiguration.cs`
- `Runtime/Core/EchoLaunchRoot.cs`
- `Runtime/Execution/StartupSequencePreflight.cs` or a focused splash-preflight
  collaborator when separation is clearer
- Existing presentation-dispatch code only when needed to resolve
  `IImageSplashPresenter`

### Possible created Runtime

A focused internal helper may be created when it reduces root complexity:

- `Runtime/Splash/SplashSequencePreflight.cs`
- `Runtime/Splash/SplashSequencePreflightException.cs`

Do not create speculative abstractions beyond the integration need.

### Modified tests

- `Tests/Runtime/PlayMode/LaunchConfigurationBindingTests.cs`
- Relevant root lifecycle fixtures

### Preferred new focused fixture

- `Tests/Runtime/PlayMode/EchoLaunchRootSplashLifecycleTests.cs`

### Plan

- `Plan Documentation/Checkpoint Build Plans/FL-M4-04_Splash_Configuration_Schema_and_Root_Playback_Integration_Checkpoint_Build_Plan.md`

Unity generates matching `.meta` files.

---

## 5. Implementation sequence

### Phase 1 — Schema binding

1. Advance configuration schema to 4.
2. Add optional splash reference.
3. Add reduced-motion default.
4. Preserve canonical configuration identity.
5. Update test-only binding helpers.
6. Prove schema 3 rejection.
7. Prove runtime immutability.
8. Compile.

### Phase 2 — Splash preflight

1. Treat null as legal omission.
2. Treat empty valid sequence as legal no-op.
3. Validate assigned sequence before launch work.
4. Convert invalid assignment to `ELAUNCH-SPLASH-001`.
5. Prove steps and destination remain untouched.
6. Compile.

### Phase 3 — Root playback

1. Resolve `IImageSplashPresenter`.
2. Fall back to `NullImageSplashPresenter`.
3. Record `ELAUNCH-SPLASH-003` when configured visuals are unavailable.
4. Construct player with the root launch clock.
5. Pass configuration reduced-motion value.
6. Pass root cancellation token.
7. Await splash before starting the startup runner.
8. Clear splash before step presentation.
9. Compile.

### Phase 4 — Failure and cancellation

1. Convert cancellation to existing interrupted settlement.
2. Convert unexpected playback failure to `ELAUNCH-SPLASH-002`.
3. Prevent step and destination side effects.
4. Preserve exactly-once report/event behavior.
5. Prove destruction cancellation.
6. Compile.

### Phase 5 — Ordering and retained proof

1. Prove optional splash before first startup step.
2. Prove startup completion before destination load.
3. Prove duplicate root silence.
4. Prove automatic-start route.
5. Prove direct-scene mode uses the same contract.
6. Run complete Runtime Play Mode suite.
7. Record discovered totals.

### Phase 6 — Closeout

1. Review exact Git scope.
2. Commit and push implementation.
3. Batch-generate adjacent documentation.
4. Commit and push documentation.
5. Confirm clean synchronized repository.

---

## 6. Minimum automated proof

At least 24 focused tests should cover:

### Configuration

1. Current schema is 4.
2. Schema 3 is unsupported.
3. Splash reference binds.
4. Reduced-motion default binds.
5. Configuration remains immutable.

### Preflight

6. Null splash is accepted.
7. Empty valid splash is accepted.
8. Invalid sequence identity blocks.
9. Unsupported splash schema blocks.
10. Null entry blocks.
11. Missing image blocks.
12. Duplicate entry ID blocks.

### Root ordering

13. No splash starts steps directly.
14. Assigned splash precedes first step.
15. Splash completes before steps.
16. Steps complete before destination load.
17. Reduced-motion value reaches playback.
18. Visual presenter receives frames.
19. Missing visual presenter continues headless with warning.

### Settlement

20. Splash failure blocks steps and destination.
21. Splash cancellation interrupts once.
22. Destruction during splash interrupts once.
23. Duplicate root presents no splash.
24. Automatic start uses the same path.
25. Direct-scene mode uses the same contract.
26. Successful total report elapsed includes splash time.
27. Report schema remains 2.
28. Config and splash assets are not dirtied.

Predicted full-suite target:

```text
478 or greater
```

The final discovered count is evidence. This number is only a planning floor.

---

## 7. Diagnostics

### `ELAUNCH-SPLASH-001`

Blocking preflight result for invalid assigned splash definitions.

Details may identify:

- Sequence identity/schema.
- Entry index.
- Entry ID.
- Missing image.
- Invalid timing.
- Duplicate ID.

### `ELAUNCH-SPLASH-002`

Blocking runtime result for unexpected playback failure.

Development details may contain sanitized exception information.

### `ELAUNCH-SPLASH-003`

Warning when a splash is configured but no visual splash presenter exists.

Playback continues headless.

No per-frame logs.

---

## 8. Compile and test gates

- Unity errors: 0.
- Unity compiler warnings: 0.
- Complete Runtime Play Mode suite passes.
- Existing 450 tests remain green.
- Report schema remains 2.
- Configuration schema is 4.
- No runtime asset mutation.
- No peer-package dependency.
- No EchoInput dependency.
- No splash/step concurrency.
- `git diff --check` passes.

---

## 9. Explicit exclusions

FL-M4-04 does not authorize:

- Editor schema migration.
- Runtime schema migration.
- Report schema 3.
- Splash metrics inside `LaunchReport`.
- Concurrent splash and startup steps.
- Prefab or Canvas art.
- Project input bindings.
- EchoInput or EchoSettings bridges.
- Direct-scene initializer implementation.
- Legal-screen semantics.
- Video or custom animation adapters.
- Interactive retry/cancel UI.
- Test Lab scenes.
- Player builds.
- Package version change.

---

## 10. Rollback

Before commit:

```cmd
git restore --staged .
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Configuration/EchoLaunchConfiguration.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Core/EchoLaunchRoot.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflight.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchConfigurationBindingTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Splash/SplashSequencePreflight.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Splash/SplashSequencePreflight.cs.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Splash/SplashSequencePreflightException.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Splash/SplashSequencePreflightException.cs.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchRootSplashLifecycleTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchRootSplashLifecycleTests.cs.meta"
```

After a pushed commit, use `git revert`.

---

## 11. Commit plan

Authority:

```text
echo-launch: approve FL-M4-04 splash schema 4 and root order
```

Implementation:

```text
echo-launch: complete FL-M4-04 splash root integration
```

Documentation closeout:

```text
echo-launch: document FL-M4-04 completion
```

---

## 12. Stop point

Stop after schema-4 configuration binding, optional splash preflight, sequential
root playback before startup steps, cancellation/failure settlement, and focused
automated proof.

Do not begin Editor migration, report expansion, prefab art, direct-scene
tooling, or Laboratory scenes.

---

## 13. Tentative next checkpoint

**FL-M4-05 — Startup Presentation Prefab and Canvas Assembly**

Tentative only. It does not become active through FL-M4-04 authority.

---

## 14. Approval

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Commit specification v1.5.0 and ADR-002 before runtime
implementation. Preserve report schema 2, sequential phase order, headless
fallback, asset immutability, and exactly-once terminal settlement.
