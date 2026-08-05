# FL-M3-03 - Monotonic Timeout Clock and Cooperative Cancellation Checkpoint Build Plan

**Document ID:** FL-M3-03
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
**Milestone:** M3 - Startup Sequence
**Owner:** Jesse "Echo" Adams / EchoDevGames
**Repository/workspace:** The Sperk Forge
**Unity baseline:** Unity 6000.3.8f1
**Public Unity floor:** Unity 6000.0
**Workflow authority:** SFGSS-005 v1.4.0
**Implementation baseline:** `78db46e`
**Starting Runtime Play Mode result:** 231 passed, 0 failed, 0 ignored
**Starting compilation result:** 0 errors, 0 warnings
**Last updated:** August 5, 2026

> FL-M3-02 taught the runner how to judge failures. FL-M3-03 gives it an unscaled clock, a deterministic deadline, and a cooperative cancellation request without allowing timed-out work to become an orphaned ghost process.

---

## 1. Purpose and Observable Outcome

Introduce an injectable monotonic launch clock, deterministic per-step timeout monitoring, stable timeout results, and cooperative timeout cancellation.

When this checkpoint is complete:

- Runtime code has one explicit `ILaunchClock` seam.
- The default Unity clock uses unscaled real time.
- Tests can supply a deterministic manual clock without waiting on wall-clock seconds.
- Timeout measurement begins immediately before executor invocation.
- Timeout zero remains disabled.
- Positive timeout metadata establishes one absolute deadline.
- Completion observed before the deadline wins.
- The first observed deadline crossing wins over any later executor result.
- A timed-out step produces `ELAUNCH-STEP-003`.
- The timeout result includes configured timeout, measured elapsed time, and whether cancellation was requested.
- A step declaring cancellation support receives a timeout cancellation request.
- A step declaring cancellation unsupported receives no timeout cancellation request.
- The runner does not continue to another entry while the timed-out executor is still active.
- The runner consumes the executor's eventual result or exception after timeout so no hidden operation is abandoned.
- Late progress is ignored after timeout determination.
- Timeout results pass through the existing `ContinueWithWarning` or `BlockLaunch` policy evaluator.
- Caller-provided cancellation remains distinct from timeout cancellation.
- `OperationCanceledException` caused by the caller still escapes the generic timeout path.
- Definitions, entries, policies, sequences, and configurations remain immutable.
- Thirty-two new Runtime Play Mode tests pass.
- Full expected Runtime Play Mode total is 263 passed, 0 failed, 0 ignored.
- Unity compilation remains 0 errors and 0 warnings.

---

## 2. Authority Set Used

This plan is derived from:

1. SFGSS-000 - suite authority and package independence.
2. SFGSS-PKG-ECHOLAUNCH-001 v1.3.0 - approved First Light package contract.
3. SFGSS-003 v1.1.0 - immutable authored definitions and runtime-only active state.
4. SFGSS-005 v1.4.0 - checkpoint planning, visible code, testing, evidence, and stop rules.
5. PKG-LEARN-001 - First Light learning review.
6. FL-M2-08 policy and executor contracts.
7. FL-M3-01 immediate runner.
8. FL-M3-02 policy and exception conversion.
9. Current package runtime files and 231-test baseline.
10. Unity 6 `Awaitable`, `Awaitable.NextFrameAsync`, and unscaled real-time API contracts.

Approved package decisions governing this checkpoint:

- `ILaunchClock` is the explicit clock seam.
- Timeout uses monotonic unscaled time.
- Pause and `Time.timeScale` must not freeze launch timeout policy.
- Tests must be able to supply a fake clock.
- Every active step receives a `CancellationToken`.
- Cancellation is cooperative.
- A step that cannot safely cancel must declare that fact.
- `ELAUNCH-STEP-003` identifies step timeout.
- Timeout results follow the already-approved continue-or-block failure action.
- Automatic retry and interactive retry remain deferred.
- Active timing and cancellation state remain outside ScriptableObject definitions.

No authority conflict was found.

---

## 3. Starting Conditions

Before implementation:

- `main` and `origin/main` are synchronized at `78db46e`.
- The working tree is clean.
- Unity compiles with 0 errors and 0 warnings.
- FL-M2-01 through FL-M2-08 and FL-M3-01 through FL-M3-02 are complete.
- Runtime Play Mode baseline is:
  - Passed: `231`
  - Failed: `0`
  - Ignored: `0`
- `StartupStepPolicy` already exposes:
  - `TimeoutSeconds`;
  - `HasTimeout`;
  - `SupportsCancellation`.
- Zero timeout means disabled.
- Positive finite timeout enables timeout policy.
- `StartupStepContext` already carries a `CancellationToken`.
- `StartupStepPolicyEvaluator` already handles `TimedOut`.
- `StartupStepResult.TimedOut(...)` already exists.
- `StartupSequenceRunner` already:
  - traverses enabled entries in authored order;
  - creates fresh executors;
  - contains exceptions;
  - applies failure policy;
  - stops when the effective result requires it.
- `OperationCanceledException` currently escapes generic exception conversion.
- No clock seam exists.
- No timeout monitor exists.
- No timeout-triggered cancellation source exists.
- No execution timing snapshot exists.
- No retry behavior exists.

If any starting condition is false, stop and reconcile it before implementation.

---

## 4. Architectural Constraints

The checkpoint must preserve these boundaries:

- Authored timeout metadata remains immutable.
- Active deadlines, elapsed time, cancellation sources, timeout state, and late executor outcomes are runtime-only.
- The default clock is unscaled.
- The runner depends on `ILaunchClock`, not directly on `Time` after clock construction.
- The clock must never move backward during one attempt.
- Non-finite or negative clock values are invalid.
- Tests must not sleep or wait for real seconds.
- Timeout zero means no deadline and no timeout cancellation source.
- The timeout deadline is:
  - `startSeconds + TimeoutSeconds`.
- Executor completion observed before deadline evaluation wins.
- Once timeout is observed, the effective source result is timed out even if the executor later returns success, warning, or another failure.
- Timeout cancellation is requested only when `SupportsCancellation` is true.
- Caller cancellation and timeout cancellation use a linked per-attempt token.
- Caller cancellation remains distinguishable from timeout cancellation.
- The runner does not detach, abandon, or ignore an active executor.
- After timeout, the runner waits for the executor to settle before moving to another entry.
- A cancellation-supporting executor is expected to observe the token and settle promptly.
- A non-cancellable executor must finish naturally; the timeout result still applies after it settles.
- Late progress after timeout determination must not mutate the completed execution or throw into an abandoned operation.
- Timeout result code is exactly `ELAUNCH-STEP-003`.
- Timeout result then follows existing failure policy.
- No authored asset is repaired or rewritten.
- No public mutable collection is introduced.
- No `UnityEditor` API enters Runtime.
- No peer Sperk Forge package becomes a dependency.
- Existing authority, lifecycle, notification, configuration, sequence, policy, exception, and execution tests remain green.

---

## 5. Authorized Scope

FL-M3-03 authorizes:

1. Public `ILaunchClock`.
2. Internal `UnityLaunchClock`.
3. Immutable runtime `StartupStepTiming`.
4. Internal `StartupStepProgressGate`.
5. Immutable internal `StartupStepAwaitOutcome`.
6. Internal `StartupStepTimeoutMonitor`.
7. Stable runtime use of `ELAUNCH-STEP-003`.
8. Injected clock construction on `StartupSequenceRunner`.
9. Per-attempt linked cancellation source.
10. Timeout-triggered cooperative cancellation.
11. Timeout result creation and details.
12. Timing capture on `StartupStepExecution`.
13. Existing policy application to timeout results.
14. Focused Runtime Play Mode clock, timing, gate, timeout, and runner tests.
15. Unity-generated `.meta` files.
16. The established checkpoint documentation family at closeout.

---

## 6. Explicit Exclusions and Stop Point

Do not create or implement:

- automatic retry
- retry count
- retry backoff
- interactive retry
- retry or skip UI
- root-level `CancelLaunch` implementation
- shutdown/destruction cancellation orchestration
- root runner integration
- automatic startup from `Awake`, `Start`, or scene callbacks
- launch-session lifecycle advancement
- public step lifecycle events
- `LaunchReport`
- `LaunchReportBuilder`
- report timing aggregation
- preflight validation
- duplicate-ID scans
- dependency validation
- configuration migration or repair
- runner re-entry protection
- production splash execution
- destination loading
- presentation
- direct-scene behavior
- custom inspectors
- setup windows
- Test Lab scenes
- peer-package bridges

Stop after deterministic timeout measurement, timeout result creation, safe cooperative cancellation request, executor settlement, and existing failure policy application are proven.

---

## 7. Exact File Manifest

### Create

```text
Packages/com.echodevgames.echo-launch/
├── Runtime/
│   └── Execution/
│       ├── ILaunchClock.cs
│       ├── ILaunchClock.cs.meta
│       ├── UnityLaunchClock.cs
│       ├── UnityLaunchClock.cs.meta
│       ├── StartupStepTiming.cs
│       ├── StartupStepTiming.cs.meta
│       ├── StartupStepProgressGate.cs
│       ├── StartupStepProgressGate.cs.meta
│       ├── StartupStepAwaitOutcome.cs
│       ├── StartupStepAwaitOutcome.cs.meta
│       ├── StartupStepTimeoutMonitor.cs
│       └── StartupStepTimeoutMonitor.cs.meta
└── Tests/
    └── Runtime/
        └── PlayMode/
            ├── LaunchClockTimingAndGateTests.cs
            ├── LaunchClockTimingAndGateTests.cs.meta
            ├── StartupSequenceRunnerTimeoutTests.cs
            └── StartupSequenceRunnerTimeoutTests.cs.meta
```

### Modify

```text
Packages/com.echodevgames.echo-launch/
└── Runtime/
    └── Execution/
        ├── StartupStepExecution.cs
        └── StartupSequenceRunner.cs
```

Existing retained tests may receive only bounded assertion maintenance if the new timing surface requires it. No retained test behavior is expected to change except where timeout state is newly visible.

### Checkpoint Plan

```text
Plan Documentation/
└── Checkpoint Build Plans/
    └── FL-M3-03_Monotonic_Timeout_Clock_and_Cooperative_Cancellation_Checkpoint_Build_Plan.md
```

### Closeout Documentation

Use the established nine-file pattern:

```text
Packages/com.echodevgames.echo-launch/
├── CHANGELOG.md
├── README.md
└── Documentation~/
    ├── Index.md
    └── Developer/
        ├── Architecture.md
        ├── Current Notes.md
        ├── Checkpoints/
        │   └── FL-M3-03_Monotonic_Timeout_Clock_and_Cooperative_Cancellation.md
        └── Test Reports/
            └── FL-M3-03_Timeout_Clock_and_Cancellation_Test_Report.md

Plan Documentation/
├── Current Notes.md
└── Implementation Checkpoints/
    └── FL-M3-03_First_Light_Timeout_Clock_and_Cancellation_Completion.md
```

No other file is authorized.

---

## 8. `ILaunchClock` Contract

`ILaunchClock` is the explicit runtime and test clock seam.

It exposes:

```csharp
double NowSeconds { get; }

Awaitable NextTickAsync(
    CancellationToken cancellationToken);
```

Rules:

- `NowSeconds` is monotonic and nondecreasing.
- Values are finite and nonnegative.
- Values are expressed in seconds.
- Values are unscaled.
- `NextTickAsync` yields without blocking the Unity player loop.
- The default implementation resumes on a later Unity frame.
- A test implementation may advance deterministically when ticked.
- Clock implementations own no launch authority.
- Clock implementations do not interpret timeout policy.

The interface is public because custom integration tests and project-defined runner composition may need an explicit deterministic clock seam.

---

## 9. `UnityLaunchClock` Contract

`UnityLaunchClock` is the default internal implementation.

It uses:

```text
Time.realtimeSinceStartupAsDouble
Awaitable.NextFrameAsync(cancellationToken)
```

Rules:

- Time scale does not affect the clock.
- The implementation contains no mutable launch state.
- The runner may use a shared stateless instance.
- Unity API access occurs on the Unity main thread.
- No Editor API is used.

---

## 10. `StartupStepTiming` Contract

`StartupStepTiming` is an immutable runtime value.

It records:

- start time;
- completion/settlement time;
- elapsed seconds;
- configured timeout seconds;
- whether timeout was configured;
- whether timeout was reached;
- whether timeout cancellation was requested.

Validation:

- Start and completion times are finite and nonnegative.
- Completion is not earlier than start.
- Timeout seconds are finite and nonnegative.
- Timeout cannot be marked reached when timeout is disabled.
- Cancellation-requested cannot be true when timeout was not reached.
- Elapsed time is derived, not independently authored.

It does not contain:

- executor result;
- stack trace;
- policy decision;
- retry state;
- root lifecycle state.

---

## 11. `StartupStepProgressGate` Contract

The progress gate prevents late timed-out work from mutating a completed attempt.

It wraps one `IStartupStepProgressReporter`.

Open state:

- Reports forward to the wrapped reporter.

Closed state:

- Reports are ignored.
- Reports do not throw.
- The gate does not reopen.

Rules:

- Wrapped reporter is non-null.
- Closing is idempotent.
- The gate contains no clock and no policy.
- Normal non-timeout progress behavior remains unchanged.

---

## 12. `StartupStepAwaitOutcome` Contract

`StartupStepAwaitOutcome` is an immutable internal result from one monitored executor.

It records:

- executor result when completion wins;
- whether timeout was reached;
- whether timeout cancellation was requested;
- timing snapshot.

Rules:

- Normal completion requires a non-null result.
- Timed-out outcome may discard the executor's late result from effective policy.
- The outcome does not apply `StartupStepFailureAction`.
- The outcome does not mutate the execution object.
- Caller cancellation may still escape instead of producing an outcome.

---

## 13. `StartupStepTimeoutMonitor` Contract

The monitor owns the deadline loop for one executor invocation.

Inputs:

- one executor awaitable;
- one valid policy;
- one clock;
- one timeout cancellation source;
- one progress gate.

Algorithm:

1. Capture and validate start time.
2. Get the executor awaiter.
3. If no timeout is configured, await normal completion.
4. For timed execution:
   - check executor completion first;
   - validate current clock time;
   - if current time reaches/exceeds deadline:
     - mark timeout exactly once;
     - close the progress gate;
     - request cancellation when supported;
   - await the next clock tick;
   - repeat until the executor settles.
5. Consume the executor result or exception.
6. Capture settlement time.
7. Return normal or timed-out outcome.

Deterministic race rule:

- If executor completion is already observable before deadline evaluation, completion wins.
- Otherwise the first observed deadline crossing wins.
- Once timeout wins, later executor success/failure does not replace timeout.

Safety rule:

- The monitor never returns while the executor remains active.
- This prevents detached work from continuing into later startup steps.

---

## 14. Timeout Result Contract

Stable code:

```text
ELAUNCH-STEP-003
```

Message:

```text
The startup step exceeded its configured timeout.
```

Details include normalized invariant values:

```text
TimeoutSeconds: <configured>
ElapsedSeconds: <measured>
CancellationRequested: <True|False>
```

The timeout source result uses `StartupStepResult.TimedOut(...)`.

Existing policy then applies:

- `ContinueWithWarning`
  - effective warning;
  - next entry may run after the timed-out executor has settled.
- `BlockLaunch`
  - effective blocking failure;
  - later entries remain unvisited.

Timeout results are structured data. The runner does not emit per-frame or repeated timeout logs.

---

## 15. Cooperative Cancellation Contract

For one timed attempt:

1. Create a timeout `CancellationTokenSource`.
2. Link it with the caller-provided token.
3. Supply the linked token through `StartupStepContext`.
4. At timeout:
   - when `SupportsCancellation` is true, cancel the timeout source;
   - when false, do not request timeout cancellation.
5. Wait for the executor to settle.
6. Dispose per-attempt cancellation sources.

Rules:

- Timeout cancellation is requested exactly once.
- Caller cancellation is not mislabeled as timeout.
- A timeout-triggered `OperationCanceledException` produces timeout outcome.
- A caller-triggered `OperationCanceledException` still escapes this checkpoint's generic path.
- No cancellation source is stored in a ScriptableObject.
- No executor is reused.

---

## 16. `StartupStepExecution` Changes

`StartupStepExecution` gains one immutable completed timing snapshot.

Planned additions:

- `StartupStepTiming Timing`
- `bool HasTiming`
- completion overloads that accept timing

Rules:

- Timing is assigned exactly once with terminal completion.
- Pre-start factory failures may use zero elapsed timing only if no executor invocation began.
- Normal completion and timeout completion preserve current status/result guards.
- Existing construction and metadata behavior remains.
- Progress after timeout is blocked by the progress gate before execution completion.

---

## 17. `StartupSequenceRunner` Changes

The runner gains clock injection.

Constructors:

```csharp
StartupSequenceRunner()

StartupSequenceRunner(
    ILaunchClock clock)
```

Default constructor:

- uses `UnityLaunchClock`.

Injected constructor:

- rejects null clock.

Per enabled entry:

1. Create runtime execution metadata.
2. Create executor.
3. Create timeout and linked cancellation sources.
4. Create progress gate.
5. Build context with linked token and gate.
6. Begin execution.
7. Invoke executor.
8. Monitor completion and timeout.
9. Convert non-timeout executor exceptions through existing `ELAUNCH-STEP-004`.
10. Build timeout result when deadline wins.
11. Apply existing failure policy.
12. Complete execution with effective result and timing.
13. Continue or stop.
14. Dispose per-attempt cancellation sources.

No root or lifecycle integration is added.

---

## 18. Implementation Phases

### Phase A - Clock Interface

Create:

- `Runtime/Execution/ILaunchClock.cs`

Verify:

- Unity compiles with 0 errors and 0 warnings.
- Existing 231 tests remain available.
- No runner behavior changes.

### Phase B - Default Unity Clock

Create:

- `Runtime/Execution/UnityLaunchClock.cs`

Verify:

- Unity compiles.
- No runner behavior changes.
- Clock is unscaled.

### Phase C - Timing and Progress Gate

Create:

- `Runtime/Execution/StartupStepTiming.cs`
- `Runtime/Execution/StartupStepProgressGate.cs`

Modify:

- `Runtime/Execution/StartupStepExecution.cs`

Verify:

- Existing tests compile.
- Timing and gate behavior remain disconnected from the runner.

### Phase D - Await Outcome and Timeout Monitor

Create:

- `Runtime/Execution/StartupStepAwaitOutcome.cs`
- `Runtime/Execution/StartupStepTimeoutMonitor.cs`

Verify:

- Clock-driven monitoring compiles.
- No runner behavior changes.

### Phase E - Runner Timeout Integration

Modify:

- `Runtime/Execution/StartupSequenceRunner.cs`

Verify:

- Clock injection works.
- Timeout uses linked cancellation.
- Timeout result flows through existing policy.
- No timeout logs, retries, reports, root, or lifecycle behavior appear.

### Phase F - Automated Tests

Create:

- `Tests/Runtime/PlayMode/LaunchClockTimingAndGateTests.cs`
- `Tests/Runtime/PlayMode/StartupSequenceRunnerTimeoutTests.cs`

Run all Runtime Play Mode tests.

Expected:

- Passed: `263`
- Failed: `0`
- Ignored: `0`

Expected compilation:

- Errors: `0`
- Warnings: `0`

### Phase G - Git and Documentation Closeout

1. Review only checkpoint-owned files.
2. Clean Unity-generated metadata automatically if needed.
3. Commit and push implementation.
4. Generate one-command nine-file documentation closeout.
5. Commit and push documentation.
6. Confirm clean synchronized repository.
7. Stop before retries, reports, root integration, and lifecycle automation.

---

## 19. Planned Automated Test Registry

### Clock, timing, and progress gate

| ID | Test | Expected |
|---|---|---|
| FL-M3-03-T-001 | Clock interface shape | Double time plus cancellable next tick |
| FL-M3-03-T-002 | Unity clock implements seam | Default clock assignable to interface |
| FL-M3-03-T-003 | Unity clock value | Finite and nonnegative |
| FL-M3-03-T-004 | Manual clock advance | Deterministic monotonic ticks |
| FL-M3-03-T-005 | Timing rejects non-finite start | Clear exception |
| FL-M3-03-T-006 | Timing rejects negative start | Clear exception |
| FL-M3-03-T-007 | Timing rejects completion before start | Clear exception |
| FL-M3-03-T-008 | Timing elapsed | Derived correctly |
| FL-M3-03-T-009 | Timing timeout disabled | Valid non-timeout snapshot |
| FL-M3-03-T-010 | Timing timeout reached | Valid timeout snapshot |
| FL-M3-03-T-011 | Progress gate open | Forwards report |
| FL-M3-03-T-012 | Progress gate closed | Ignores late report |
| FL-M3-03-T-013 | Progress gate repeated close | Safe and idempotent |
| FL-M3-03-T-014 | Execution timing assignment | Captured exactly once |

### Timeout monitor and runner

| ID | Test | Expected |
|---|---|---|
| FL-M3-03-T-015 | Zero timeout | Disabled; immediate result preserved |
| FL-M3-03-T-016 | Positive timeout before deadline | Executor result wins |
| FL-M3-03-T-017 | Completion observable at boundary | Completion wins deterministic race |
| FL-M3-03-T-018 | Deadline crossing | Timeout wins |
| FL-M3-03-T-019 | Timeout code | `ELAUNCH-STEP-003` |
| FL-M3-03-T-020 | Timeout details | Timeout, elapsed, cancellation flag |
| FL-M3-03-T-021 | Supported cancellation | Token cancelled once |
| FL-M3-03-T-022 | Unsupported cancellation | Token not timeout-cancelled |
| FL-M3-03-T-023 | Timed-out late success | Timeout remains authoritative |
| FL-M3-03-T-024 | Timed-out late failure | Timeout remains authoritative |
| FL-M3-03-T-025 | Timeout cancellation exception | Consumed as timeout |
| FL-M3-03-T-026 | Caller cancellation exception | Escapes timeout path |
| FL-M3-03-T-027 | Continue-with-warning timeout | Warning after settlement; later step runs |
| FL-M3-03-T-028 | Block-launch timeout | Blocking after settlement; later step unvisited |
| FL-M3-03-T-029 | Late progress | Ignored after timeout determination |
| FL-M3-03-T-030 | Backward clock | Blocking contract failure |
| FL-M3-03-T-031 | Authored asset immutability | Definitions and policies unchanged |
| FL-M3-03-T-032 | Full Runtime Play Mode suite | 263 / 0 / 0 |
| FL-M3-03-T-033 | Compilation | 0 errors, 0 warnings |
| FL-M3-03-T-034 | Git scope | Only authorized files |

The automated count is 32. The final two rows cover compilation and Git evidence.

---

## 20. Manual Unity Verification

No production asset, scene, prefab, root, or automatic startup setup is required.

Manual verification is limited to:

1. Return to Unity after each phase.
2. Confirm 0 compiler errors.
3. Confirm 0 compiler warnings.
4. Confirm no root or GameObject appears automatically.
5. Confirm no sequence runs outside explicit tests.
6. Confirm no unexpected Console warning appears.
7. Run all Play Mode tests after Phase F.

Do not use real multi-second waits for automated tests.

Do not create temporary project assets unless a failing test requires a bounded reproduction.

---

## 21. Common Failure Symptoms and Bounded Fixes

| Symptom | Likely Cause | Allowed Fix |
|---|---|---|
| Timeout freezes when time scale is zero | Scaled clock used | Route through `ILaunchClock` defaulting to real time |
| Tests take real seconds | Wall-clock wait used | Use manual clock and deterministic ticks |
| Completion loses at exact deadline | Deadline checked before completed awaiter | Observe completion first |
| Later result replaces timeout | Timeout state not latched | Make first deadline crossing authoritative |
| Runner continues while timed-out executor runs | Awaitable abandoned | Wait for cooperative settlement |
| Late progress throws | Execution reporter used directly | Close progress gate at timeout |
| Unsupported step receives cancellation | Policy flag ignored | Cancel only when supported |
| Supported step never sees cancellation | Context token not linked | Use linked per-attempt token |
| Caller cancellation becomes timeout | Cancellation source not distinguished | Track timeout-triggered state separately |
| Timeout uses `ELAUNCH-STEP-004` | Wrong converter path | Create timed-out result with `ELAUNCH-STEP-003` |
| Timeout bypasses failure policy | Evaluator not called | Apply existing policy to timed-out result |
| Later factory runs after blocking timeout | Stop applied too late | Stop before next iteration |
| Clock moves backward silently | Clock contract not validated | Convert to blocking contract failure |
| ScriptableObject becomes dirty | Runtime timing stored in asset | Keep timing in execution object |
| Retry behavior appears | Scope leak | Remove retry count, loop, and UI |

---

## 22. Rollback

To return to `78db46e` behavior:

1. Restore:
   - `StartupStepExecution.cs`
   - `StartupSequenceRunner.cs`
2. Remove:
   - `ILaunchClock.cs`
   - `UnityLaunchClock.cs`
   - `StartupStepTiming.cs`
   - `StartupStepProgressGate.cs`
   - `StartupStepAwaitOutcome.cs`
   - `StartupStepTimeoutMonitor.cs`
   - `LaunchClockTimingAndGateTests.cs`
   - `StartupSequenceRunnerTimeoutTests.cs`
   - their `.meta` files.
3. Remove the FL-M3-03 Checkpoint Build Plan.
4. Refresh Unity.
5. Confirm 231 Runtime Play Mode tests pass.
6. Confirm compilation is 0 errors and 0 warnings.
7. Revert only FL-M3-03 documentation if abandoned.

No configuration asset, scene, prefab, build setting, or project setting requires recovery.

---

## 23. Commit Plan

Preferred implementation commit:

```text
Add EchoLaunch timeout clock and cooperative cancellation
```

Preferred adjacent documentation commit:

```text
Close FL-M3-03 timeout and cancellation documentation
```

No commit or remote state is claimed until CMD evidence exists.

---

## 24. Completion Criteria

- [ ] Clean starting repository at `78db46e`.
- [ ] Public `ILaunchClock` exists.
- [ ] Default Unity clock is unscaled.
- [ ] Manual tests can advance time deterministically.
- [ ] Runtime timing is immutable.
- [ ] Progress gate ignores late reports.
- [ ] Timeout monitor observes completion before deadline checks.
- [ ] Timeout zero remains disabled.
- [ ] Positive timeout uses one absolute deadline.
- [ ] Timeout code is `ELAUNCH-STEP-003`.
- [ ] Timeout details include timeout, elapsed, and cancellation request.
- [ ] Supported cancellation requests the linked token exactly once.
- [ ] Unsupported cancellation does not request timeout cancellation.
- [ ] Timed-out executor settles before traversal proceeds.
- [ ] Late executor result does not replace timeout.
- [ ] Timeout-triggered cancellation exception becomes timeout.
- [ ] Caller cancellation remains distinct.
- [ ] Continue-with-warning timeout allows later traversal after settlement.
- [ ] Block-launch timeout leaves later entries unvisited.
- [ ] Authored assets remain immutable.
- [ ] Thirty-two new tests pass.
- [ ] Full suite is 263 passed, 0 failed, 0 ignored.
- [ ] Compilation is 0 errors and 0 warnings.
- [ ] Implementation commit and push are confirmed.
- [ ] Nine-file documentation closeout is committed and pushed.
- [ ] Working tree is clean.
- [ ] Work stops before retries, reports, root integration, and lifecycle automation.

---

## 25. Next Recommended Checkpoint

**FL-M3-04 - Multi-Frame Async Proof and Runner Cancellation Outcome**

Expected future scope:

- production-shaped multi-frame test executor;
- caller cancellation converted into structured cancelled/interrupted run outcome;
- cancellation reason metadata;
- linked-token teardown proof;
- no retry UI;
- no root lifecycle integration yet.

FL-M3-03 does not authorize FL-M3-04.

---

## 26. Approval

**Decision:** Active and authorized
**Approved by:** Jesse "Echo" Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Use an injectable monotonic unscaled clock, preserve deterministic completion-versus-timeout ordering, request cancellation only when supported, never abandon timed-out work, apply existing failure policy to `ELAUNCH-STEP-003`, prove 263 Runtime Play Mode tests with zero compiler warnings, and stop before retries, reports, root integration, and lifecycle automation.
