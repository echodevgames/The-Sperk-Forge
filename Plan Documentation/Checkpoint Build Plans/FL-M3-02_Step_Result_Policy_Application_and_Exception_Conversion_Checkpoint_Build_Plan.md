# FL-M3-02 - Step Result Policy Application and Exception Conversion Checkpoint Build Plan

**Document ID:** FL-M3-02
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
**Implementation baseline:** `967173e`
**Starting Runtime Play Mode result:** 199 passed, 0 failed, 0 ignored
**Last updated:** August 5, 2026

> FL-M3-01 taught the runner to execute and record. FL-M3-02 teaches it when a failure becomes a warning, when launch must stop, and how thrown exceptions become stable structured results instead of loose shrapnel.

---

## 1. Purpose and Observable Outcome

Apply authored startup-step failure policy to completed results, convert contained factory and executor failures into stable structured results, and stop sequence traversal when policy requires it.

When this checkpoint is complete:

- Successful results remain unchanged and traversal continues.
- Existing warning results remain unchanged and traversal continues.
- Skipped results remain unchanged and traversal continues.
- `ContinueWithWarning` converts failure-like results into warnings and continues.
- `BlockLaunch` converts failure-like results into blocking failures and stops traversal.
- `Cancelled` remains cancelled and stops traversal.
- Factory exceptions and null factory returns become blocking `ELAUNCH-STEP-004` results.
- Executor exceptions become `ELAUNCH-STEP-004` results and then follow authored failure policy.
- Null executor results become blocking `ELAUNCH-STEP-004` contract-failure results.
- Exception results contain sanitized type and message details without stack traces.
- The run result reports early-stop and unvisited-entry metadata.
- No later entry creates an executor after a blocking decision.
- Definitions, entries, policies, sequences, and configurations remain immutable.
- Thirty-two new Runtime Play Mode tests pass.
- Full expected Runtime Play Mode total is 231 passed, 0 failed, 0 ignored.

---

## 2. Authority Set Used

This plan is derived from:

1. SFGSS-000 - suite authority and package independence.
2. SFGSS-PKG-ECHOLAUNCH-001 v1.3.0 - approved First Light package contract.
3. SFGSS-003 v1.1.0 - immutable authored definitions and runtime-only active state.
4. SFGSS-005 v1.4.0 - checkpoint planning, visible code, testing, evidence, and stop rules.
5. PKG-LEARN-001 - First Light learning review.
6. FL-M2-08 policy and executor contracts.
7. FL-M3-01 immediate runner implementation and documentation closeout.
8. Current package runtime files and 199-test baseline.

Approved package decisions governing this checkpoint:

- Exceptions from step execution are converted by the runner.
- Step exceptions use stable diagnostic `ELAUNCH-STEP-004`.
- MVP failure actions are exactly:
  - `ContinueWithWarning`;
  - `BlockLaunch`.
- Automatic retry and interactive retry are deferred.
- Definitions and configuration assets remain immutable.
- Active exceptions and results remain runtime-only.
- A null or throwing executor factory is a blocking contract failure.
- Timeout uses policy later, but timeout measurement is not part of this checkpoint.

No authority conflict was found.

---

## 3. Starting Conditions

Before implementation:

- `main` and `origin/main` are synchronized at `967173e`.
- The working tree is clean.
- Unity compiles with zero errors.
- FL-M2-01 through FL-M2-08 and FL-M3-01 are complete.
- Runtime Play Mode baseline is:
  - Passed: `199`
  - Failed: `0`
  - Ignored: `0`
- `StartupSequenceRunner` executes enabled entries in authored order.
- Disabled entries create no executor.
- Immediate executor results are captured.
- Blocking results are currently recorded without stopping traversal.
- Factory exceptions currently escape.
- Executor exceptions currently escape.
- Null executor returns currently throw.
- `StartupSequenceRunResult` currently requires every authored entry to be disabled or attempted.
- No timeout clock, retry behavior, preflight, report builder, root integration, or lifecycle automation exists.

If any starting condition is false, stop and reconcile it before implementation.

---

## 4. Architectural Constraints

The checkpoint must preserve these boundaries:

- Authored `StartupStepPolicy.FailureAction` is the runtime continuation authority.
- `IsRequired` and `IsOptional` remain descriptive authoring intent and do not silently override the explicit failure action.
- Policy application occurs after a terminal step result exists.
- Success, warning, and skipped results are policy-neutral and continue.
- Recoverable failure, blocking failure, and already-returned timeout results are failure-like for policy application.
- `ContinueWithWarning` produces a warning result and continues.
- `BlockLaunch` produces a blocking failure and stops.
- Converted results preserve the original code, message, and details.
- Cancelled results remain cancelled and stop.
- Factory creation failure blocks regardless of entry policy because no valid executor exists.
- Executor exceptions are converted before policy application.
- `OperationCanceledException` is not converted by the generic exception path; cancellation orchestration remains a later checkpoint.
- Null executor results are invalid contract failures and block.
- Exception details include only sanitized exception type and message.
- Stack traces and recursive inner-exception dumps are not placed in step results.
- No exception is logged repeatedly by the runner.
- No authored asset is repaired or rewritten.
- No public mutable collection is introduced.
- No `UnityEditor` API enters Runtime.
- No peer Sperk Forge package becomes a dependency.
- Existing authority, lifecycle, notification, configuration, sequence, policy, and execution behavior remains green except the intentionally changed blocking-traversal expectation.

---

## 5. Authorized Scope

FL-M3-02 authorizes:

1. `StartupStepPolicyDecision`.
2. `StartupStepPolicyEvaluator`.
3. `StartupStepExceptionPhase`.
4. `StartupStepExceptionConverter`.
5. Stable runtime use of `ELAUNCH-STEP-004`.
6. Pre-executor failure capture on `StartupStepExecution`.
7. Early-stop and unvisited-entry metadata on `StartupSequenceRunResult`.
8. Policy and exception handling inside `StartupSequenceRunner`.
9. Factory exception containment.
10. Null executor containment.
11. Executor exception containment.
12. Null result containment.
13. Blocking traversal stop.
14. Updates to the FL-M3-01 blocking-result test expectation.
15. Focused Runtime Play Mode policy and exception suites.
16. Unity-generated `.meta` files.
17. The established checkpoint documentation family at closeout.

---

## 6. Explicit Exclusions and Stop Point

Do not create or implement:

- timeout measurement
- `ILaunchClock`
- timeout race
- timeout cancellation
- automatic retry
- retry count
- retry backoff
- interactive retry
- user skip or retry UI
- root runner integration
- automatic startup from `Awake`, `Start`, or scene callbacks
- launch-session lifecycle advancement
- public step lifecycle events
- `LaunchReport`
- `LaunchReportBuilder`
- warning aggregation outside the run result
- preflight validation
- duplicate-ID scans
- dependency validation
- configuration migration or repair
- runner re-entry protection
- asynchronous multi-frame proof
- splash execution
- destination loading
- presentation
- direct-scene behavior
- custom inspectors
- setup windows
- Test Lab scenes
- peer-package bridges

Stop after policy and exception conversion produce deterministic structured run results and blocking decisions stop traversal.

---

## 7. Exact File Manifest

### Create

```text
Packages/com.echodevgames.echo-launch/
├── Runtime/
│   └── Execution/
│       ├── StartupStepPolicyDecision.cs
│       ├── StartupStepPolicyDecision.cs.meta
│       ├── StartupStepPolicyEvaluator.cs
│       ├── StartupStepPolicyEvaluator.cs.meta
│       ├── StartupStepExceptionPhase.cs
│       ├── StartupStepExceptionPhase.cs.meta
│       ├── StartupStepExceptionConverter.cs
│       └── StartupStepExceptionConverter.cs.meta
└── Tests/
    └── Runtime/
        └── PlayMode/
            ├── StartupStepPolicyApplicationTests.cs
            ├── StartupStepPolicyApplicationTests.cs.meta
            ├── StartupSequenceRunnerPolicyAndExceptionTests.cs
            └── StartupSequenceRunnerPolicyAndExceptionTests.cs.meta
```

### Modify

```text
Packages/com.echodevgames.echo-launch/
├── Runtime/
│   └── Execution/
│       ├── StartupStepExecution.cs
│       ├── StartupSequenceRunResult.cs
│       └── StartupSequenceRunner.cs
└── Tests/
    └── Runtime/
        └── PlayMode/
            └── StartupSequenceRunnerImmediateTests.cs
```

### Checkpoint Plan

```text
Plan Documentation/
└── Checkpoint Build Plans/
    └── FL-M3-02_Step_Result_Policy_Application_and_Exception_Conversion_Checkpoint_Build_Plan.md
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
        │   └── FL-M3-02_Step_Result_Policy_Application_and_Exception_Conversion.md
        └── Test Reports/
            └── FL-M3-02_Step_Result_Policy_and_Exception_Test_Report.md

Plan Documentation/
├── Current Notes.md
└── Implementation Checkpoints/
    └── FL-M3-02_First_Light_Step_Result_Policy_and_Exception_Completion.md
```

No other file is authorized.

---

## 8. `StartupStepPolicyDecision` Contract

`StartupStepPolicyDecision` is an internal immutable runtime value.

It stores:

- original terminal result;
- effective terminal result after policy;
- whether traversal continues.

It exposes:

- `OriginalResult`
- `EffectiveResult`
- `ShouldContinue`
- `StopsTraversal`
- `WasConverted`

Rules:

- Original result is non-null.
- Effective result is non-null.
- Conversion is determined by reference identity because preserved results remain the original immutable instance.
- The decision does not modify either result.
- The decision does not know about root lifecycle, reports, scenes, retries, or timeouts.

---

## 9. Policy Evaluation Contract

`StartupStepPolicyEvaluator` evaluates one authored policy and one terminal result.

### 9.1 Continue without conversion

These statuses preserve the original result and continue:

- `Succeeded`
- `Warning`
- `Skipped`

### 9.2 Failure-like results

These statuses follow `FailureAction`:

- `RecoverableFailure`
- `BlockingFailure`
- `TimedOut`

For `ContinueWithWarning`:

- Convert to `Warning`.
- Preserve code, message, and details.
- Continue traversal.

For `BlockLaunch`:

- Convert to `BlockingFailure` when needed.
- Preserve code, message, and details.
- Stop traversal.

### 9.3 Cancellation

`Cancelled`:

- Remains `Cancelled`.
- Stops traversal.
- Is never converted to warning.

### 9.4 Authored intent versus action

`IsRequired` and `IsOptional` remain visible authored intent.

`FailureAction` is the explicit runtime decision authority.

Therefore unusual but valid combinations behave according to `FailureAction`:

- Required plus `ContinueWithWarning` continues.
- Optional plus `BlockLaunch` blocks.

The Editor validator may later warn about unusual combinations. Runtime does not invent a hidden override.

### 9.5 Invalid policy

An invalid policy is rejected as a blocking contract failure before result application.

Runtime does not repair the policy.

---

## 10. Exception Conversion Contract

`StartupStepExceptionConverter` uses stable code:

```text
ELAUNCH-STEP-004
```

### 10.1 Factory phase

Factory failures include:

- `CreateExecutor()` throws.
- `CreateExecutor()` returns null.

Factory failures:

- Produce a blocking result.
- Stop traversal.
- Do not invoke an executor.
- Do not follow `ContinueWithWarning`, because no valid execution contract exists.

### 10.2 Execution phase

Executor failures include:

- `ExecuteAsync(context)` throws a non-cancellation exception.
- An executor returns null instead of `StartupStepResult`.

Executor exceptions:

1. Become a recoverable `ELAUNCH-STEP-004` source result.
2. Follow the authored failure action.
3. Continue as warning or stop as blocking.

A null result is a blocking contract failure and does not continue.

### 10.3 Sanitized content

The converted result includes:

- code `ELAUNCH-STEP-004`;
- a plain operation-specific message;
- exception type;
- trimmed exception message when present.

It excludes:

- stack trace;
- recursive inner-exception graph;
- Unity object dumps;
- repeated Console logging.

### 10.4 Cancellation exception

`OperationCanceledException` is not converted by the generic exception converter.

Cancellation orchestration remains outside FL-M3-02.

---

## 11. `StartupStepExecution` Changes

The execution object gains a bounded pre-executor failure path.

It must support:

- creation from valid entry metadata before executor creation;
- attaching one executor exactly once while `NotStarted`;
- normal `Begin()` only when an executor is attached;
- blocking completion before begin for factory-contract failure;
- one terminal result only.

Planned internal additions:

- `HasExecutor`
- `AttachExecutor(executor)`
- `CompleteBeforeStart(blockingResult)`

Rules:

- Existing constructor with executor remains available for retained tests.
- `AttachExecutor` rejects null, repeated attachment, or attachment after start.
- `Begin` rejects missing executor.
- `CompleteBeforeStart` accepts only blocking terminal results.
- Pre-start completion does not fake executor invocation.
- Definitions and entries remain unchanged.

---

## 12. `StartupSequenceRunResult` Changes

A policy stop means some authored entries are neither disabled nor attempted.

The immutable run result gains:

- `UnvisitedEntryCount`
- `WasStoppedEarly`
- `StoppingAuthoredEntryIndex`

Accounting invariant:

```text
attempted + disabled + unvisited = authored
```

Rules:

- A complete traversal has:
  - unvisited `0`;
  - stopped early `false`;
  - stopping index `-1`.
- A policy stop records the authored index that stopped traversal.
- Later entries are counted as unvisited.
- Disabled entries before the stop remain disabled.
- Entries after the stop are unvisited even if authored disabled, because traversal never inspected them.
- The backing execution array remains private.

---

## 13. Runner Application Order

For each authored entry:

1. Stop immediately when a prior decision ended traversal.
2. Read the entry.
3. Skip and count disabled entries.
4. Create `StartupStepExecution` metadata before factory creation.
5. Call `CreateExecutor()`.
6. Convert factory exceptions or null returns to blocking failure.
7. Attach the fresh executor.
8. Build immutable context.
9. Begin execution.
10. Await `ExecuteAsync(context)`.
11. Convert non-cancellation executor exceptions.
12. Reject null results through the contract-failure path.
13. Evaluate policy.
14. Complete the execution with the effective result.
15. Append the execution.
16. Continue or stop according to the decision.
17. Return an immutable run result with accounting metadata.

No later factory is called after a stop decision.

---

## 14. Implementation Phases

### Phase A - Policy Decision Value

Create:

- `StartupStepPolicyDecision.cs`

Verify:

- Unity compiles with zero errors.
- Existing 199 tests remain available.
- No runner behavior changes yet.

### Phase B - Policy Evaluator

Create:

- `StartupStepPolicyEvaluator.cs`

Verify:

- Unity compiles.
- No runner behavior changes yet.

### Phase C - Exception Vocabulary and Converter

Create:

- `StartupStepExceptionPhase.cs`
- `StartupStepExceptionConverter.cs`

Verify:

- Stable `ELAUNCH-STEP-004` conversion compiles.
- No runner behavior changes yet.

### Phase D - Execution and Run-Result Support

Modify:

- `StartupStepExecution.cs`
- `StartupSequenceRunResult.cs`

Verify:

- Existing tests compile.
- New pre-executor failure and early-stop metadata are available.

### Phase E - Runner Policy and Exception Application

Modify:

- `StartupSequenceRunner.cs`
- `StartupSequenceRunnerImmediateTests.cs`

Verify:

- Blocking results stop later factory creation.
- Existing immediate behavior remains green where policy allows continuation.

### Phase F - Automated Tests

Create:

- `StartupStepPolicyApplicationTests.cs`
- `StartupSequenceRunnerPolicyAndExceptionTests.cs`

Run all Runtime Play Mode tests.

Expected:

- Passed: `231`
- Failed: `0`
- Ignored: `0`

### Phase G - Git and Documentation Closeout

1. Review only checkpoint-owned files.
2. Clean Unity-generated metadata automatically if needed.
3. Commit and push implementation.
4. Generate one-command nine-file documentation closeout.
5. Commit and push documentation.
6. Confirm clean synchronized repository.
7. Stop before timeout, retries, reports, root integration, or lifecycle automation.

---

## 15. Planned Automated Test Registry

### Policy decision and evaluator

| ID | Test | Expected |
|---|---|---|
| FL-M3-02-T-001 | Decision rejects null original | Clear exception |
| FL-M3-02-T-002 | Decision rejects null effective | Clear exception |
| FL-M3-02-T-003 | Preserved decision | Same result, not converted |
| FL-M3-02-T-004 | Converted decision | Different result, converted |
| FL-M3-02-T-005 | Success policy | Preserved and continue |
| FL-M3-02-T-006 | Warning policy | Preserved and continue |
| FL-M3-02-T-007 | Skipped policy | Preserved and continue |
| FL-M3-02-T-008 | Recoverable plus continue | Warning and continue |
| FL-M3-02-T-009 | Blocking plus continue | Warning and continue |
| FL-M3-02-T-010 | Recoverable plus block | Blocking and stop |
| FL-M3-02-T-011 | Blocking plus block | Preserved and stop |
| FL-M3-02-T-012 | Timed out plus continue | Warning and continue |
| FL-M3-02-T-013 | Timed out plus block | Blocking and stop |
| FL-M3-02-T-014 | Cancelled | Preserved and stop |
| FL-M3-02-T-015 | Converted text | Code, message, details preserved |
| FL-M3-02-T-016 | Unusual required/action pair | Failure action remains authoritative |

### Runner and exception conversion

| ID | Test | Expected |
|---|---|---|
| FL-M3-02-T-017 | Factory exception | Blocking `ELAUNCH-STEP-004` |
| FL-M3-02-T-018 | Null executor | Blocking `ELAUNCH-STEP-004` |
| FL-M3-02-T-019 | Factory failure later step | Later factory not called |
| FL-M3-02-T-020 | Executor exception plus continue | Warning and later step runs |
| FL-M3-02-T-021 | Executor exception plus block | Blocking and later step does not run |
| FL-M3-02-T-022 | Null executor result | Blocking contract failure |
| FL-M3-02-T-023 | Exception details | Type and trimmed message only |
| FL-M3-02-T-024 | Cancellation exception | Not converted by generic path |
| FL-M3-02-T-025 | Returned recoverable plus continue | Warning and continue |
| FL-M3-02-T-026 | Returned blocking plus continue | Warning and continue |
| FL-M3-02-T-027 | Returned recoverable plus block | Blocking and stop |
| FL-M3-02-T-028 | Returned blocking plus block | Blocking and stop |
| FL-M3-02-T-029 | Early-stop accounting | Attempted, disabled, unvisited sum to authored |
| FL-M3-02-T-030 | Stopping authored index | Correct index recorded |
| FL-M3-02-T-031 | Complete traversal metadata | No early stop and no unvisited entries |
| FL-M3-02-T-032 | Definition immutability | Assets and policies unchanged |
| FL-M3-02-T-033 | Full Runtime Play Mode suite | 231 / 0 / 0 |
| FL-M3-02-T-034 | Git scope | Only authorized files |

The automated count is 32. The final two rows cover full-suite and Git evidence.

---

## 16. Manual Unity Verification

No production asset, scene, prefab, or root setup is required.

Manual verification is limited to:

1. Return to Unity after each phase.
2. Confirm zero compiler errors.
3. Confirm no root or GameObject appears automatically.
4. Confirm no sequence runs outside explicit tests.
5. Confirm no unexpected Console warning appears.
6. Run all Play Mode tests after Phase F.

Do not create temporary project assets unless a failing test requires a bounded reproduction.

---

## 17. Common Failure Symptoms and Bounded Fixes

| Symptom | Likely Cause | Allowed Fix |
|---|---|---|
| Optional warning failure still blocks | Evaluator ignores failure action | Make `FailureAction` authoritative |
| Required continue policy secretly blocks | `IsRequired` overrides action | Remove hidden override |
| Blocking result still runs later factory | Stop decision applied too late | Break before next loop iteration |
| Exception escapes | Catch boundary missing | Convert non-cancellation exception |
| Cancellation becomes warning | Generic exception catch is too broad | Exclude `OperationCanceledException` |
| Stack trace appears in result | Converter copies exception text wholesale | Store only type and message |
| Factory failure continues | Policy applied to invalid factory | Force blocking factory result |
| Null result crashes completion | Contract failure not converted | Create blocking result before completion |
| Run-result invariant fails | Unvisited count missing | Track attempted, disabled, and unvisited |
| Entry after stop counted disabled | Runner inspects it after stop | Count all later entries as unvisited |
| Asset changes after run | Policy applied to definition asset | Create runtime effective result only |
| Existing immediate test fails | Old test expected no policy stop | Update only the deliberate boundary test |

---

## 18. Rollback

To return to `967173e` behavior:

1. Restore:
   - `StartupStepExecution.cs`
   - `StartupSequenceRunResult.cs`
   - `StartupSequenceRunner.cs`
   - `StartupSequenceRunnerImmediateTests.cs`
2. Remove:
   - `StartupStepPolicyDecision.cs`
   - `StartupStepPolicyEvaluator.cs`
   - `StartupStepExceptionPhase.cs`
   - `StartupStepExceptionConverter.cs`
   - `StartupStepPolicyApplicationTests.cs`
   - `StartupSequenceRunnerPolicyAndExceptionTests.cs`
   - their `.meta` files.
3. Remove the FL-M3-02 Checkpoint Build Plan.
4. Refresh Unity.
5. Confirm 199 Runtime Play Mode tests pass.
6. Revert only FL-M3-02 documentation if abandoned.

No configuration asset, scene, prefab, build setting, or project setting requires recovery.

---

## 19. Commit Plan

Preferred implementation commit:

```text
Apply EchoLaunch step policy and exception conversion
```

Preferred adjacent documentation commit:

```text
Close FL-M3-02 policy and exception documentation
```

No commit or remote state is claimed until CMD evidence exists.

---

## 20. Completion Criteria

- [ ] Clean starting repository at `967173e`.
- [ ] Immutable policy decision value exists.
- [ ] Success, warning, and skipped results continue unchanged.
- [ ] Continue-with-warning converts failure-like results.
- [ ] Block-launch converts failure-like results and stops.
- [ ] Cancelled remains cancelled and stops.
- [ ] Factory exceptions become blocking `ELAUNCH-STEP-004`.
- [ ] Null factory results become blocking `ELAUNCH-STEP-004`.
- [ ] Executor exceptions follow policy.
- [ ] Null executor results block.
- [ ] Exception details are sanitized.
- [ ] `OperationCanceledException` is not generically converted.
- [ ] Later entries create no executor after a stop.
- [ ] Run-result early-stop metadata is correct.
- [ ] Authored assets remain immutable.
- [ ] Thirty-two new tests pass.
- [ ] Full suite is 231 passed, 0 failed, 0 ignored.
- [ ] Implementation commit and push are confirmed.
- [ ] Nine-file documentation closeout is committed and pushed.
- [ ] Working tree is clean.
- [ ] Work stops before timeout, retry, reports, root integration, and lifecycle automation.

---

## 21. Next Recommended Checkpoint

**FL-M3-03 - Monotonic Timeout Clock and Cooperative Cancellation**

Expected future scope:

- `ILaunchClock`;
- unscaled monotonic time;
- per-step timeout race;
- timeout result `ELAUNCH-STEP-003`;
- cooperative cancellation request;
- no retry UI;
- no root lifecycle integration yet.

FL-M3-02 does not authorize FL-M3-03.

---

## 22. Approval

**Decision:** Active and authorized
**Approved by:** Jesse "Echo" Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Keep `FailureAction` explicit, contain non-cancellation exceptions through `ELAUNCH-STEP-004`, make invalid factories block, preserve authored assets, prove 231 Runtime Play Mode tests, and stop before timeout, retry, reports, root integration, or lifecycle automation.
