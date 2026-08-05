# FL-M3-01 - Startup Sequence Runner Skeleton and Immediate Step Execution Checkpoint Build Plan

**Document ID:** FL-M3-01
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
**Implementation baseline:** `eedf759`
**Starting Runtime Play Mode result:** 169 passed, 0 failed, 0 ignored
**Last updated:** August 4, 2026

> The launch plan, rules, and executor plug now exist. FL-M3-01 creates the first runtime conductor and lets immediate step results cross the wire, but it does not yet decide what those results mean for launch policy.

---

## 1. Purpose and Observable Outcome

Create the first internal startup-sequence runner skeleton and prove deterministic execution of immediate startup-step executors.

When this checkpoint is complete:

- Every enabled sequence entry creates one fresh runtime executor.
- Enabled entries begin in authored order.
- Disabled entries create no executor and perform no work.
- Every attempted step owns one runtime-only `StartupStepExecution`.
- Each executor receives:
  - launch mode;
  - configuration ID;
  - sequence ID;
  - entry ID;
  - step ID;
  - authored index;
  - authored sequence count;
  - cancellation token;
  - package-owned progress reporter.
- Immediate executor results are awaited and captured.
- Success, warning, recoverable failure, and blocking failure results are preserved exactly.
- Step progress reported during execution is captured by the active execution object.
- The runner returns an immutable sequence-run summary.
- The runner does not interpret failure policy yet.
- The runner does not stop after a blocking result yet.
- No root, launch session, lifecycle state, event, report, presenter, destination, splash, timeout, retry, or preflight integration is added.
- Thirty new Runtime Play Mode tests pass.
- Full expected Runtime Play Mode total is 199 passed, 0 failed, 0 ignored.

---

## 2. Authority Set Used

This plan is derived from:

1. SFGSS-000 - suite authority and package independence.
2. SFGSS-PKG-ECHOLAUNCH-001 v1.3.0 - approved First Light package contract.
3. SFGSS-003 v1.1.0 - immutable authored definitions and runtime-only active state.
4. SFGSS-005 v1.4.0 - checkpoint planning, visible code, testing, evidence, and stop rules.
5. PKG-LEARN-001 - First Light learning review.
6. FL-M2-08 implementation and documentation closeout.
7. Current package runtime contracts and 169-test baseline.

Approved package decisions governing this checkpoint:

- Startup entries execute deterministically in authored order.
- Immediate work uses the same Unity `Awaitable<StartupStepResult>` contract as asynchronous work.
- Every attempt receives a fresh single-use executor.
- Every active attempt receives immutable context, cancellation, and package-owned progress reporting.
- Active index, progress, result, and executor state remain outside ScriptableObject definitions.
- Exception conversion belongs to the runner architecture but is deferred from this bounded first execution slice.
- MVP policy actions are already modeled, but result-to-policy interpretation remains a later checkpoint.

No authority conflict was found.

---

## 3. Starting Conditions

Before implementation:

- `main` and `origin/main` are synchronized at `eedf759`.
- The working tree is clean.
- Unity compiles with zero errors.
- FL-M2-01 through FL-M2-08 are complete.
- Runtime Play Mode baseline is:
  - Passed: `169`
  - Failed: `0`
  - Ignored: `0`
- `StartupSequence` schema is `2`.
- `StartupSequenceEntry` contains:
  - stable entry identity;
  - safe enabled/disabled activation;
  - one step-definition reference;
  - one authored policy.
- `StartupStepDefinition.CreateExecutor()` returns a fresh runtime executor.
- `IStartupStepExecutor.ExecuteAsync(context)` returns `Awaitable<StartupStepResult>`.
- `StartupStepContext`, progress reporting, cancellation metadata, and terminal step results already exist.
- No sequence runner exists.
- No `StartupStepExecution` runtime state object exists.
- No executor has been invoked by package runtime code.

If any starting condition is false, stop and reconcile it before implementation.

---

## 4. Architectural Constraints

The checkpoint must preserve these boundaries:

- `StartupStepExecution` is runtime-only state.
- Definitions, entries, sequences, configuration assets, policies, and stable IDs remain immutable.
- One execution object represents one enabled entry attempt.
- One executor instance represents one execution attempt.
- The runner creates the executor immediately before the attempt.
- Disabled entries create no executor.
- Authored list order controls attempt order.
- Authored list index remains visible in the execution context.
- The context step count uses the complete authored sequence entry count.
- The context does not grant root or session authority.
- The execution object is the progress reporter for its own attempt.
- Progress is accepted only while the execution is running.
- A terminal result can be captured exactly once.
- The first runner does not interpret `StartupStepPolicy`.
- A blocking result is captured but does not yet stop traversal.
- Exceptions are not converted to results in this checkpoint.
- Timeout metadata is not measured.
- Cancellation token is passed through but cancellation outcomes are not orchestrated.
- No public mutable collection is introduced.
- No `UnityEditor` API enters Runtime.
- No peer Sperk's Forge package becomes a dependency.
- Existing lifecycle, configuration, notification, sequence, and policy tests remain green.

---

## 5. Authorized Scope

FL-M3-01 authorizes:

1. `StartupStepExecution`.
2. `StartupSequenceRunResult`.
3. `StartupSequenceRunner`.
4. Runtime-only step-state transitions:
   - `NotStarted`;
   - `Running`;
   - one terminal `StartupStepResult`.
5. Active progress capture.
6. Enabled-entry traversal in authored order.
7. Disabled-entry counting without executor creation.
8. Fresh executor creation for each enabled attempt.
9. Immutable `StartupStepContext` creation.
10. Pass-through cancellation token.
11. Immediate result awaiting and capture.
12. Immutable sequence-run summary.
13. Runtime Play Mode tests for execution state and immediate runner behavior.
14. Unity-generated `.meta` files.
15. The established checkpoint documentation family at closeout.

---

## 6. Explicit Exclusions and Stop Point

Do not create or implement:

- `EchoLaunchRoot` runner integration
- automatic startup from `Awake`, `Start`, or another scene callback
- launch-session lifecycle advancement
- `StepStarted`, `StepProgressChanged`, or `StepCompleted` public events
- `LaunchReportBuilder`
- `LaunchReport`
- preflight validation
- stable runtime diagnostic codes for runner failures
- exception-to-result conversion
- null-definition recovery
- null-executor recovery
- result-to-policy interpretation
- blocking-result short circuit
- warning aggregation
- timeout clock
- timeout race
- timeout cancellation
- retry loops
- interactive retry
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

Stop after valid enabled entries can execute immediate test executors in order and return captured runtime results.

---

## 7. Exact File Manifest

### Create

```text
Packages/com.echodevgames.echo-launch/
├── Runtime/
│   ├── Execution.meta
│   └── Execution/
│       ├── StartupStepExecution.cs
│       ├── StartupStepExecution.cs.meta
│       ├── StartupSequenceRunResult.cs
│       ├── StartupSequenceRunResult.cs.meta
│       ├── StartupSequenceRunner.cs
│       └── StartupSequenceRunner.cs.meta
└── Tests/
    └── Runtime/
        └── PlayMode/
            ├── StartupStepExecutionTests.cs
            ├── StartupStepExecutionTests.cs.meta
            ├── StartupSequenceRunnerImmediateTests.cs
            └── StartupSequenceRunnerImmediateTests.cs.meta
```

### Checkpoint Plan

```text
Plan Documentation/
└── Checkpoint Build Plans/
    └── FL-M3-01_Startup_Sequence_Runner_Skeleton_and_Immediate_Step_Execution_Checkpoint_Build_Plan.md
```

### Modify

No existing runtime source file is modified in the planned implementation slice.

A test-only definition may be created inside the new runner test file. Existing tests should not require modification.

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
        │   └── FL-M3-01_Startup_Sequence_Runner_Skeleton_and_Immediate_Step_Execution.md
        └── Test Reports/
            └── FL-M3-01_Startup_Sequence_Runner_Immediate_Test_Report.md

Plan Documentation/
├── Current Notes.md
└── Implementation Checkpoints/
    └── FL-M3-01_First_Light_Startup_Sequence_Runner_Immediate_Execution_Completion.md
```

No other file is authorized.

---

## 8. `StartupStepExecution` Contract

`StartupStepExecution` is an internal runtime-owned object.

It stores copied execution metadata:

- entry ID;
- step ID;
- step display name;
- authored step index;
- authored sequence count;
- authored policy;
- fresh executor reference;
- current `StartupStepStatus`;
- latest `StartupStepProgress`;
- terminal `StartupStepResult`.

Construction rules:

- Entry is non-null.
- Step definition is non-null.
- Executor is non-null.
- Step count is greater than zero.
- Step index is within count.
- Copied IDs are nonblank.

Initial state:

    Status = NotStarted
    Result = null
    LatestProgress = Indeterminate

Allowed state path:

    NotStarted
        -> Running
            -> terminal result status

Rules:

- `Begin()` is legal exactly once.
- `Report(progress)` is legal only while running.
- `Complete(result)` is legal only while running.
- Completion requires a non-null terminal result.
- Completion copies the result status.
- The terminal result is stored exactly once.
- Reporting after completion is rejected.
- Repeated completion is rejected.
- No ScriptableObject is modified.

The executor reference is internal and used only by the runner.

---

## 9. `StartupSequenceRunResult` Contract

`StartupSequenceRunResult` is an internal immutable sequence summary.

It exposes:

- authored sequence entry count;
- disabled entry count;
- attempted execution count;
- read-only indexed access to attempted executions;
- whether any captured result is a warning;
- whether any captured result is a failure;
- whether any captured result is blocking.

The result:

- preserves attempted execution order;
- does not expose a mutable backing list;
- does not interpret policy;
- does not claim launch success or failure;
- does not become the public `LaunchReport`.

An empty sequence returns a valid empty result.

---

## 10. `StartupSequenceRunner` Contract

`StartupSequenceRunner` is internal runtime behavior.

Planned method:

```csharp
Awaitable<StartupSequenceRunResult> RunAsync(
    LaunchMode launchMode,
    EchoLaunchConfiguration configuration,
    CancellationToken cancellationToken)
```

Run order:

1. Validate the configuration argument.
2. Read the assigned `StartupSequence`.
3. Capture authored entry count.
4. Iterate indices from zero through count minus one.
5. Count disabled entries and continue without creating an executor.
6. For each enabled entry:
   - require a non-null definition;
   - call `CreateExecutor()` once;
   - require a non-null executor;
   - create `StartupStepExecution`;
   - create `StartupStepContext`;
   - begin the execution;
   - invoke and await `ExecuteAsync(context)`;
   - require a non-null result;
   - complete the execution;
   - append the terminal execution to the run result.
7. Return the immutable run summary.

The runner does not:

- mutate the sequence or configuration;
- reorder entries;
- evaluate policy;
- stop on blocking failure;
- catch executor exceptions;
- measure timeout;
- retry;
- publish root events;
- update `LaunchSession`;
- build a report.

These omissions are deliberate stop boundaries.

---

## 11. Immediate Executor Test Contract

The new tests use test-only step definitions and executors.

An immediate test executor may:

- synchronously report progress;
- return an already completed Unity `Awaitable<StartupStepResult>`;
- record the context it received;
- record invocation order;
- expose factory-call and execution-call counts.

The test executors must not:

- touch scenes;
- create roots;
- wait frames;
- use background threads;
- mutate definition assets;
- invoke Editor APIs.

Asynchronous multi-frame behavior belongs to a later checkpoint.

---

## 12. Implementation Phases

### Phase A - Runtime Step Execution State

Create:

- `Runtime/Execution/StartupStepExecution.cs`

Verify:

- Unity compiles with zero errors.
- Existing 169 tests remain available.
- No executor is invoked.

### Phase B - Immutable Sequence Run Result

Create:

- `Runtime/Execution/StartupSequenceRunResult.cs`

Verify:

- Unity compiles.
- The result exposes no mutable collection.

### Phase C - Internal Sequence Runner

Create:

- `Runtime/Execution/StartupSequenceRunner.cs`

Verify:

- Unity compiles.
- Nothing runs automatically.
- No root or scene changes.

### Phase D - Execution State Tests

Create:

- `Tests/Runtime/PlayMode/StartupStepExecutionTests.cs`

Run the focused fixture.

### Phase E - Immediate Runner Tests

Create:

- `Tests/Runtime/PlayMode/StartupSequenceRunnerImmediateTests.cs`

Run all Runtime Play Mode tests.

Expected:

- Passed: `199`
- Failed: `0`
- Ignored: `0`

### Phase F - Git and Documentation Closeout

1. Review only checkpoint-owned files.
2. Clean Unity-generated folder metadata automatically if needed.
3. Commit and push implementation.
4. Generate one-command nine-file documentation closeout.
5. Commit and push documentation.
6. Confirm clean synchronized repository.
7. Stop before policy interpretation, exception conversion, timeout, or root integration.

---

## 13. Planned Automated Test Registry

| ID | Test | Expected |
|---|---|---|
| FL-M3-01-T-001 | Execution construction | Copies entry, step, display, index, count, and policy metadata |
| FL-M3-01-T-002 | New execution state | `NotStarted`, null result, indeterminate progress |
| FL-M3-01-T-003 | Begin | State becomes `Running` |
| FL-M3-01-T-004 | Begin twice | Rejected |
| FL-M3-01-T-005 | Progress before begin | Rejected |
| FL-M3-01-T-006 | Progress while running | Latest progress captured |
| FL-M3-01-T-007 | Complete before begin | Rejected |
| FL-M3-01-T-008 | Null completion result | Rejected |
| FL-M3-01-T-009 | Terminal completion | Status and result captured |
| FL-M3-01-T-010 | Complete twice | Rejected |
| FL-M3-01-T-011 | Progress after completion | Rejected |
| FL-M3-01-T-012 | Invalid construction data | Rejected without asset mutation |
| FL-M3-01-T-013 | Null configuration | Runner rejects |
| FL-M3-01-T-014 | Missing sequence | Runner rejects |
| FL-M3-01-T-015 | Empty sequence | Valid empty run result |
| FL-M3-01-T-016 | Disabled entry | Counted; no executor factory call |
| FL-M3-01-T-017 | Enabled entry | One factory call and one execution |
| FL-M3-01-T-018 | Repeated runner calls | Fresh executors created |
| FL-M3-01-T-019 | Context identities | Configuration, sequence, entry, and step IDs preserved |
| FL-M3-01-T-020 | Context position | Authored index and full entry count preserved |
| FL-M3-01-T-021 | Context cancellation | Token preserved |
| FL-M3-01-T-022 | Progress report | Active execution captures immediate progress |
| FL-M3-01-T-023 | Success result | Preserved |
| FL-M3-01-T-024 | Warning result | Preserved |
| FL-M3-01-T-025 | Recoverable result | Preserved |
| FL-M3-01-T-026 | Blocking result | Preserved |
| FL-M3-01-T-027 | Authored order | Execution order preserved |
| FL-M3-01-T-028 | No policy interpretation | Runner continues after blocking result |
| FL-M3-01-T-029 | Null executor factory result | Rejected |
| FL-M3-01-T-030 | Definition immutability | Sequence, entries, policies, and definitions unchanged |
| FL-M3-01-T-031 | Full Runtime Play Mode suite | 199 / 0 / 0 |
| FL-M3-01-T-032 | Git scope | Only authorized files |

The automated count is 30. The final two rows cover full-suite and Git evidence.

---

## 14. Manual Unity Verification

No production asset, scene, prefab, or root setup is required.

Manual verification is limited to:

1. Return to Unity after each phase.
2. Confirm zero compiler errors.
3. Confirm no root or GameObject appears automatically.
4. Confirm no sequence executes merely because the new runtime types exist.
5. Confirm no unexpected Console warning appears.
6. Run all Play Mode tests after Phase E.

Do not create temporary project assets for this checkpoint unless a failing automated test requires a bounded reproduction.

---

## 15. Common Failure Symptoms and Bounded Fixes

| Symptom | Likely Cause | Allowed Fix |
|---|---|---|
| Executor runs on compilation or Play entry | Runner connected to scene callback | Remove automatic integration |
| Disabled entry creates executor | Filter occurs after factory call | Check activation before `CreateExecutor()` |
| Attempt order differs from list order | Sorting or filtered index used | Iterate authored indices directly |
| Context count uses enabled-only count | Runner compacted the sequence | Use complete authored entry count |
| Progress accepted before running | Execution guard missing | Reject outside `Running` |
| Result captured twice | Terminal guard missing | Reject repeated completion |
| Blocking result stops runner | Policy leaked into checkpoint | Capture result and continue |
| Exception becomes a result | Conversion leaked into checkpoint | Let exception escape for now |
| Timeout behavior appears | Policy metadata was interpreted | Remove timeout logic |
| Definition asset changes | Runtime state stored in asset | Move state into execution object |
| Public mutable list appears | Run result exposes backing list | Use count and indexed getter |
| Existing 169 tests fail | Prior contract changed unnecessarily | Restore prior behavior |

---

## 16. Rollback

To return to `eedf759` behavior:

1. Remove:
   - `Runtime/Execution/`
   - `Runtime/Execution.meta`
   - `StartupStepExecutionTests.cs`
   - `StartupStepExecutionTests.cs.meta`
   - `StartupSequenceRunnerImmediateTests.cs`
   - `StartupSequenceRunnerImmediateTests.cs.meta`
2. Remove the FL-M3-01 Checkpoint Build Plan.
3. Refresh Unity.
4. Confirm 169 Runtime Play Mode tests pass.
5. Revert only FL-M3-01 documentation if abandoned.

No configuration asset, scene, prefab, build setting, or project setting requires recovery.

---

## 17. Commit Plan

Preferred implementation commit:

```text
Add EchoLaunch immediate startup sequence runner
```

Preferred adjacent documentation commit:

```text
Close FL-M3-01 immediate runner documentation
```

No commit or remote state is claimed until CMD evidence exists.

---

## 18. Completion Criteria

- [ ] Clean starting repository at `eedf759`.
- [ ] `StartupStepExecution` exists.
- [ ] Execution state is runtime-only.
- [ ] Progress is accepted only while running.
- [ ] Terminal result is captured exactly once.
- [ ] `StartupSequenceRunResult` is immutable.
- [ ] `StartupSequenceRunner` exists.
- [ ] Disabled entries create no executor.
- [ ] Enabled entries create fresh executors.
- [ ] Enabled entries execute in authored order.
- [ ] Immediate terminal results are awaited and preserved.
- [ ] Context identities, index, count, cancellation, and progress are correct.
- [ ] Runner does not interpret policy.
- [ ] Runner continues after a blocking result.
- [ ] Runner does not catch executor exceptions.
- [ ] No timeout, retry, preflight, report, event, root, or lifecycle behavior exists.
- [ ] Thirty new tests pass.
- [ ] Full suite is 199 passed, 0 failed, 0 ignored.
- [ ] Implementation commit and push are confirmed.
- [ ] Nine-file documentation closeout is committed and pushed.
- [ ] Working tree is clean.
- [ ] Work stops before policy interpretation or root integration.

---

## 19. Next Recommended Checkpoint

**FL-M3-02 - Step Result Policy Application and Exception Conversion**

Expected future scope:

- convert executor/factory exceptions into stable step results;
- interpret `ContinueWithWarning` versus `BlockLaunch`;
- stop traversal on blocking policy;
- preserve warning and blocking summaries;
- no timeout clock yet;
- no root lifecycle integration yet.

FL-M3-01 does not authorize FL-M3-02.

---

## 20. Approval

**Decision:** Active and authorized
**Approved by:** Jesse "Echo" Adams / EchoDevGames
**Date:** August 4, 2026
**Conditions:** Keep execution state runtime-only, create fresh executors, execute only enabled entries in authored order, capture immediate results without interpreting policy, prove 199 Runtime Play Mode tests, and stop before exception conversion, timeout, reports, events, root integration, or lifecycle automation.
