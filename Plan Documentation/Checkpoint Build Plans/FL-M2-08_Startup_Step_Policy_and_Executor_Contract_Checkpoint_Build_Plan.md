# FL-M2-08 — Startup Step Policy and Executor Contract Checkpoint Build Plan

**Document ID:** FL-M2-08
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
**Milestone:** M3 — Startup Sequence
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Repository/workspace:** The Sperk Forge
**Unity baseline:** Unity 6000.3.8f1
**Public Unity floor:** Unity 6000.0
**Workflow authority:** SFGSS-005 v1.4.0
**Implementation baseline:** `e576aa6`
**Starting Runtime Play Mode result:** 141 passed, 0 failed, 0 ignored
**Last updated:** August 4, 2026

> FL-M2-07 authored the startup playlist. FL-M2-08 defines the failure rules on each track and the plug every fresh runtime performer must use, but the conductor still does not press Play.

---

## 1. Purpose and Observable Outcome

Define the immutable authored policy for each startup-sequence entry and the public runtime contract used by a fresh single-use step executor.

When this checkpoint is complete:

- Every sequence entry carries an explicit `StartupStepPolicy`.
- MVP failure behavior is limited to:
  - `BlockLaunch`;
  - `ContinueWithWarning`.
- Policy records:
  - required or optional intent;
  - failure action;
  - timeout seconds;
  - cancellation support.
- Zero timeout means no timeout is configured.
- Negative, NaN, infinite, or undefined policy values are detected without runtime repair.
- `StartupStepProgress` carries determinate or indeterminate progress safely.
- `IStartupStepProgressReporter` provides the package-owned progress seam.
- `StartupStepContext` provides immutable execution metadata, cancellation, and progress reporting.
- `IStartupStepExecutor` exposes Unity `Awaitable<StartupStepResult> ExecuteAsync(context)`.
- `StartupStepDefinition.CreateExecutor()` becomes the public factory for one fresh runtime executor.
- Existing test-only step definitions are updated to satisfy the new factory contract.
- `StartupSequence` schema advances from `1` to `2` because embedded entries gain serialized policy data.
- No step is executed.
- Twenty-eight new Runtime Play Mode tests pass.
- Full expected Runtime Play Mode total is 169 passed, 0 failed, 0 ignored.

---

## 2. Authority Set Used

This plan is derived from:

1. SFGSS-000 — suite authority and package independence.
2. SFGSS-PKG-ECHOLAUNCH-001 v1.3.0 — approved First Light package contract.
3. SFGSS-003 v1.1.0 — immutable authored definitions and runtime-only active state.
4. SFGSS-005 v1.4.0 — checkpoint planning, visible code, testing, evidence, and stop rules.
5. PKG-LEARN-001 — First Light learning review.
6. FL-M2-07 implementation and documentation closeout.
7. Current package runtime files and 141-test baseline.

Approved package decisions governing this checkpoint:

- Unity `Awaitable<T>` is the public async primitive.
- Every step definition creates a fresh single-use executor.
- Every active step receives cancellation and package-owned progress reporting.
- MVP failure actions are `ContinueWithWarning` and `BlockLaunch`.
- Automatic retry and interactive retry remain deferred.
- Shared ScriptableObject assets never store active execution state.

No authority conflict was found.

---

## 3. Starting Conditions

Before implementation:

- `main` and `origin/main` are synchronized at `e576aa6`.
- The working tree is clean.
- Unity compiles with zero errors.
- FL-M2-01 through FL-M2-07 are complete.
- Runtime Play Mode baseline is:
  - Passed: `141`
  - Failed: `0`
  - Ignored: `0`
- `StartupStepDefinition` is abstract and contains identity, schema, and display label only.
- `StartupSequenceEntry` contains identity, enabled state, and one definition reference.
- `StartupSequence.CurrentSchemaVersion` is `1`.
- No step policy exists.
- No executor interface exists.
- No step context or progress reporter exists.
- No sequence runner or preflight exists.

If any starting condition is false, stop and reconcile it before implementation.

---

## 4. Architectural Constraints

The checkpoint must preserve these boundaries:

- Policy is authored definition data.
- Active elapsed time, timeout countdown, retry count, cancellation state, current progress, and result remain runtime-only.
- MVP failure actions are exactly:
  - `BlockLaunch`;
  - `ContinueWithWarning`.
- Retry metadata and interactive retry are not added.
- Zero timeout means no timeout is configured.
- Timeout values are stored in seconds.
- Runtime code detects invalid policy values but does not clamp, rewrite, or repair them.
- Cancellation support is a declaration of executor capability, not active cancellation state.
- An executor instance is single-use and must be newly created for each attempt.
- A step definition never stores its executor.
- `StartupStepContext` is immutable after construction.
- The context carries references and values needed by an executor, but owns no launch authority.
- Progress reports are immutable values.
- No public mutable collections are introduced.
- No `UnityEditor` API enters Runtime.
- No peer Sperk’s Forge package becomes a dependency.
- Existing lifecycle, configuration, notification, and sequence-definition behavior remains unchanged.

---

## 5. Authorized Scope

FL-M2-08 authorizes:

1. `StartupStepFailureAction`.
2. `StartupStepPolicy`.
3. `StartupStepProgress`.
4. `IStartupStepProgressReporter`.
5. `StartupStepContext`.
6. `IStartupStepExecutor`.
7. Public abstract `StartupStepDefinition.CreateExecutor()`.
8. Serialized `StartupStepPolicy` on `StartupSequenceEntry`.
9. Read-only `StartupSequenceEntry.Policy`.
10. `StartupSequence` schema advancement from `1` to `2`.
11. Required updates to test-only step-definition subclasses.
12. One focused policy/executor-contract Runtime Play Mode test suite.
13. Unity-generated `.meta` files.
14. The established checkpoint documentation family at closeout.

---

## 6. Explicit Exclusions and Stop Point

Do not create or implement:

- `StartupSequenceRunner`
- `StartupStepExecution`
- executor invocation
- executor reuse tracking
- timeout measurement
- `ILaunchClock`
- timeout cancellation
- retry count or retry loops
- interactive retry
- automatic skip UI
- exception conversion
- result-to-policy application
- configuration or sequence preflight
- duplicate-ID scanning
- automatic lifecycle advancement
- step started/progress/completed root events
- launch reports
- splash presentation
- scene loading
- root lifetime changes
- direct-scene initialization
- custom inspectors
- setup windows
- Test Lab scenes
- peer-package bridges

Stop after authored policy and the fresh executor API can be created, read, validated, and tested without running a step.

---

## 7. Exact File Manifest

### Create

```text
Packages/com.echodevgames.echo-launch/
├── Runtime/
│   └── Steps/
│       ├── StartupStepFailureAction.cs
│       ├── StartupStepFailureAction.cs.meta
│       ├── StartupStepPolicy.cs
│       ├── StartupStepPolicy.cs.meta
│       ├── StartupStepProgress.cs
│       ├── StartupStepProgress.cs.meta
│       ├── IStartupStepProgressReporter.cs
│       ├── IStartupStepProgressReporter.cs.meta
│       ├── StartupStepContext.cs
│       ├── StartupStepContext.cs.meta
│       ├── IStartupStepExecutor.cs
│       └── IStartupStepExecutor.cs.meta
└── Tests/
    └── Runtime/
        └── PlayMode/
            ├── StartupStepPolicyAndExecutorContractTests.cs
            └── StartupStepPolicyAndExecutorContractTests.cs.meta
```

### Modify

```text
Packages/com.echodevgames.echo-launch/
├── Runtime/
│   └── Steps/
│       ├── StartupStepDefinition.cs
│       ├── StartupSequenceEntry.cs
│       └── StartupSequence.cs
└── Tests/
    └── Runtime/
        └── PlayMode/
            └── StartupSequenceDefinitionTests.cs
```

### Checkpoint Plan

```text
Plan Documentation/
└── Checkpoint Build Plans/
    └── FL-M2-08_Startup_Step_Policy_and_Executor_Contract_Checkpoint_Build_Plan.md
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
        │   └── FL-M2-08_Startup_Step_Policy_and_Executor_Contract.md
        └── Test Reports/
            └── FL-M2-08_Startup_Step_Policy_and_Executor_Contract_Test_Report.md

Plan Documentation/
├── Current Notes.md
└── Implementation Checkpoints/
    └── FL-M2-08_First_Light_Startup_Step_Policy_and_Executor_Contract_Completion.md
```

No other file is authorized.

---

## 8. Policy Contract

### 8.1 `StartupStepFailureAction`

The enum contains exactly:

```text
BlockLaunch
ContinueWithWarning
```

`BlockLaunch` remains numeric default zero so an uninitialized serialized enum fails closed.

Automatic retry is not represented.

### 8.2 `StartupStepPolicy`

The serializable value type stores:

- `required`
- `failureAction`
- `timeoutSeconds`
- `supportsCancellation`

It exposes:

- `IsRequired`
- `IsOptional`
- `FailureAction`
- `TimeoutSeconds`
- `HasTimeout`
- `SupportsCancellation`
- `RequiredBlocking`
- `OptionalWarning`
- internal `HasValidFailureAction`
- internal `HasValidTimeout`
- internal `IsValid`

Timeout rules:

- `0` means no timeout configured.
- Values greater than `0` enable timeout metadata.
- Negative values are invalid.
- NaN and infinity are invalid.
- Invalid values remain unchanged for diagnostics and later repair.
- This checkpoint does not measure time.

Preset rules:

- `RequiredBlocking`:
  - required;
  - block launch;
  - no timeout;
  - cancellation supported.
- `OptionalWarning`:
  - optional;
  - continue with warning;
  - no timeout;
  - cancellation supported.

These presets provide safe semantic starting points without pretending one universal timeout is approved.

---

## 9. Progress Contract

### 9.1 `StartupStepProgress`

The immutable value stores:

- normalized progress;
- indeterminate state;
- message.

Factories:

- `Determinate(progress01, message)`
- `Indeterminate(message)`

Determinate values must be within `0` through `1`.

Indeterminate progress does not invent a numeric completion percentage.

Messages are normalized without modifying authored assets.

### 9.2 `IStartupStepProgressReporter`

The package-owned interface exposes:

```csharp
void Report(StartupStepProgress progress);
```

The executor receives the interface but does not own the runner, root, presenter, or report builder.

This checkpoint only defines and tests the seam.

---

## 10. Context Contract

`StartupStepContext` is a read-only runtime object created later by the runner.

It carries:

- launch mode;
- configuration ID;
- sequence ID;
- entry ID;
- step ID;
- step index;
- step count;
- cancellation token;
- progress reporter.

Validation rules:

- IDs are nonblank.
- Step count is greater than zero.
- Step index is within count.
- Progress reporter is non-null.
- Values are copied and exposed read-only.

The context cannot:

- mutate launch state;
- publish root notifications;
- access arbitrary services;
- change sequence assets;
- execute another step;
- finalize a report.

---

## 11. Executor Contract

`IStartupStepExecutor` exposes:

```csharp
Awaitable<StartupStepResult> ExecuteAsync(
    StartupStepContext context);
```

Rules:

- Execution begins through the runner in a later checkpoint.
- The runner begins calls on the Unity main thread.
- Immediate work may return an already-completed `Awaitable`.
- Asynchronous work must not block the player loop.
- Cancellation is cooperative.
- Unity API use must occur on the Unity main thread.
- Exceptions are converted by the future runner, not by this interface.
- Each executor instance is single-use.
- This checkpoint does not enforce reuse at runtime because no runner exists.

`StartupStepDefinition` adds:

```csharp
public abstract IStartupStepExecutor CreateExecutor();
```

Every call must produce one fresh executor when the definition is valid.

A null return or thrown factory exception will become a preflight blocker later. FL-M2-08 only exposes the contract and test seam.

---

## 12. Sequence Entry and Schema Contract

`StartupSequenceEntry` gains:

```csharp
[SerializeField]
private StartupStepPolicy policy =
    StartupStepPolicy.RequiredBlocking;
```

and:

```csharp
public StartupStepPolicy Policy =>
    policy;
```

The returned struct is a copy and cannot mutate the serialized entry.

Because the embedded entry shape changes, `StartupSequence.CurrentSchemaVersion` advances:

```text
1 -> 2
```

Runtime migration and repair remain outside this checkpoint.

---

## 13. Implementation Phases

### Phase A — Policy Vocabulary

Create:

- `StartupStepFailureAction.cs`
- `StartupStepPolicy.cs`

Verify:

- Unity compiles with zero errors.
- Existing 141 tests remain available.
- No asset or scene setup is required.

### Phase B — Progress and Context Contracts

Create:

- `StartupStepProgress.cs`
- `IStartupStepProgressReporter.cs`
- `StartupStepContext.cs`

Verify:

- Unity compiles.
- No execution occurs.

### Phase C — Executor Factory Contract

Create:

- `IStartupStepExecutor.cs`

Modify:

- `StartupStepDefinition.cs`
- `StartupSequenceDefinitionTests.cs`

Verify:

- All concrete/test definitions satisfy `CreateExecutor()`.
- Unity compiles.
- No executor is invoked.

### Phase D — Entry Policy and Sequence Schema

Modify:

- `StartupSequenceEntry.cs`
- `StartupSequence.cs`

Verify manually with a temporary sequence:

- Added entry shows policy fields.
- Default policy is required and blocking.
- Sequence asset creation causes no execution.
- Delete the temporary asset before Git review.

### Phase E — Automated Tests

Create:

- `StartupStepPolicyAndExecutorContractTests.cs`

Run all Runtime Play Mode tests.

Expected:

- Passed: `169`
- Failed: `0`
- Ignored: `0`

### Phase F — Git and Documentation Closeout

1. Review only checkpoint-owned files.
2. Clean Unity-generated metadata automatically if needed.
3. Commit and push implementation.
4. Generate one-command nine-file documentation closeout.
5. Commit and push documentation.
6. Confirm clean synchronized repository.
7. Stop before runner or preflight implementation.

---

## 14. Planned Automated Test Registry

| ID | Test | Expected |
|---|---|---|
| FL-M2-08-T-001 | Failure enum values | Exactly block and continue-warning |
| FL-M2-08-T-002 | Required preset | Required |
| FL-M2-08-T-003 | Required preset action | Block launch |
| FL-M2-08-T-004 | Required preset timeout | Disabled |
| FL-M2-08-T-005 | Required preset cancellation | Supported |
| FL-M2-08-T-006 | Optional preset | Optional |
| FL-M2-08-T-007 | Optional preset action | Continue with warning |
| FL-M2-08-T-008 | Positive timeout | Enabled and preserved |
| FL-M2-08-T-009 | Zero timeout | Disabled |
| FL-M2-08-T-010 | Negative timeout | Invalid without repair |
| FL-M2-08-T-011 | Non-finite timeout | Invalid without repair |
| FL-M2-08-T-012 | Undefined failure action | Invalid without rewrite |
| FL-M2-08-T-013 | Determinate progress | Preserved |
| FL-M2-08-T-014 | Progress boundaries | Zero and one accepted |
| FL-M2-08-T-015 | Indeterminate progress | Marked indeterminate |
| FL-M2-08-T-016 | Progress below range | Rejected |
| FL-M2-08-T-017 | Progress above range | Rejected |
| FL-M2-08-T-018 | Progress message | Normalized |
| FL-M2-08-T-019 | Context identity data | Preserved |
| FL-M2-08-T-020 | Context index/count | Preserved |
| FL-M2-08-T-021 | Context cancellation | Preserved |
| FL-M2-08-T-022 | Context progress seam | Reporter receives value |
| FL-M2-08-T-023 | Null reporter | Rejected |
| FL-M2-08-T-024 | Executor method contract | Returns `Awaitable<StartupStepResult>` |
| FL-M2-08-T-025 | Definition factory | Produces executor |
| FL-M2-08-T-026 | Repeated factory calls | Distinct executors |
| FL-M2-08-T-027 | Entry default policy | Required and blocking |
| FL-M2-08-T-028 | Sequence schema | Version `2`; older value unsupported |
| FL-M2-08-T-029 | Full Runtime Play Mode suite | 169 / 0 / 0 |
| FL-M2-08-T-030 | Manual policy authoring | Inspector fields visible |
| FL-M2-08-T-031 | Git scope | Only authorized files |

The automated count is 28. The last three rows cover full-suite, manual, and Git evidence.

---

## 15. Manual Unity Verification

After Phase D:

1. Create a temporary `StartupSequence` in `Assets/Settings`.
2. Expand `Entries`.
3. Add one entry.
4. Confirm the entry shows:
   - Enabled;
   - Step Definition;
   - Policy;
   - Required;
   - Failure Action;
   - Timeout Seconds;
   - Supports Cancellation.
5. Confirm default policy:
   - Required: true;
   - Failure Action: Block Launch;
   - Timeout Seconds: 0;
   - Supports Cancellation: true.
6. Do not assign a step definition.
7. Confirm no root, execution, lifecycle change, or warning occurs.
8. Delete the temporary sequence before Git review.

---

## 16. Common Failure Symptoms and Bounded Fixes

| Symptom | Likely Cause | Allowed Fix |
|---|---|---|
| Policy defaults optional | Struct field lacks explicit preset | Initialize entry policy to `RequiredBlocking` |
| Enum contains retry | Deferred behavior leaked in | Remove retry values |
| Negative timeout becomes zero | Runtime clamp hides invalid data | Preserve raw value and report invalid |
| Definition stores executor | Definition/runtime boundary violated | Return a fresh instance only |
| Existing test step fails compile | Abstract factory added | Add test-only executor and override |
| Context exposes setters | Runtime metadata is mutable | Use constructor and read-only properties |
| Progress permits values over one | Validation missing | Reject invalid determinate values |
| Sequence schema stays one | Embedded serialized shape changed | Advance to schema `2` |
| Adding entry starts work | Runner behavior leaked in | Remove execution |
| Temporary asset appears in Git | Manual evidence retained | Delete the temporary asset |

---

## 17. Rollback

To return to `e576aa6` behavior:

1. Restore:
   - `StartupStepDefinition.cs`
   - `StartupSequenceEntry.cs`
   - `StartupSequence.cs`
   - `StartupSequenceDefinitionTests.cs`
2. Remove the six new policy/context/executor files and their `.meta` files.
3. Remove the new contract test and its `.meta`.
4. Remove temporary assets.
5. Refresh Unity.
6. Confirm 141 Runtime Play Mode tests pass.
7. Revert only FL-M2-08 documentation if abandoned.

No save data, scenes, prefabs, build settings, or project settings require recovery.

---

## 18. Commit Plan

Preferred implementation commit:

```text
Add EchoLaunch step policy and executor contracts
```

Preferred adjacent documentation commit:

```text
Close FL-M2-08 step contract documentation
```

No commit or remote state is claimed until CMD evidence exists.

---

## 19. Completion Criteria

- [ ] Clean starting repository at `e576aa6`.
- [ ] MVP failure enum contains only two approved actions.
- [ ] Policy models required/optional intent.
- [ ] Policy models timeout and cancellation capability.
- [ ] Invalid policy data is detected without repair.
- [ ] Progress value type supports determinate and indeterminate reporting.
- [ ] Package-owned progress reporter exists.
- [ ] Context is immutable and validated.
- [ ] Executor uses Unity `Awaitable<StartupStepResult>`.
- [ ] Definition creates a fresh executor.
- [ ] Entry stores policy as authored data.
- [ ] Sequence schema advances to `2`.
- [ ] No runner or preflight exists.
- [ ] No executor is invoked.
- [ ] Twenty-eight new tests pass.
- [ ] Full suite is 169 passed, 0 failed, 0 ignored.
- [ ] Temporary assets are removed.
- [ ] Implementation commit and push are confirmed.
- [ ] Nine-file documentation closeout is committed and pushed.
- [ ] Working tree is clean.
- [ ] Work stops before runner or preflight implementation.

---

## 20. Next Recommended Checkpoint

**FL-M3-01 — Startup Sequence Runner Skeleton and Immediate Step Execution**

Expected future scope:

- runtime-owned `StartupStepExecution`;
- ordered enabled-entry traversal;
- fresh executor creation;
- immediate completed `Awaitable` support;
- result capture;
- no timeout clock or asynchronous timeout race yet.

FL-M2-08 does not authorize FL-M3-01.

---

## 21. Approval

**Decision:** Active and authorized
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 4, 2026
**Conditions:** Keep policy authored and runtime state separate, use only the two approved MVP failure actions, create fresh executors, preserve invalid values for diagnostics, prove 169 Runtime Play Mode tests, and stop before runner or preflight behavior.
