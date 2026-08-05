# FL-M3-06 — Root-Owned Startup Run and Lifecycle Advancement

**Document ID:** FL-M3-06
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
**Milestone:** M3 — Startup Sequence Runtime
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Repository/workspace:** The Sperk’s Forge
**Unity baseline:** Unity 6000.3.8f1
**Implementation baseline:** `485a09f`
**Previous implementation commit:** `b70a100`
**Previous documentation commit:** `485a09f`
**Workflow authority:** SFGSS-005 v1.4.0
**Authorized:** August 5, 2026

> The sequence runner has proven its machinery. FL-M3-06 places the ignition key in the authoritative root without yet leaving the Boot scene.

---

## 1. Purpose and Observable Outcome

FL-M3-06 connects the proven startup-sequence runner to the authoritative `EchoLaunchRoot` through one explicit, root-owned launch attempt.

When complete:

1. `EchoLaunchRoot` owns one explicit startup-sequence invocation.
2. `Awake` still claims authority only; it does not automatically start launch work.
3. The root advances its existing `LaunchSession` from `AuthorityClaimed` to `Validating`, then to `Running` after successful sequence preflight.
4. Accepted step start, progress, and completion observations become immutable root progress snapshots.
5. A successful sequence advances to `Transitioning`, not `Completed`, because destination activation and handoff remain later work.
6. A blocking run advances to `Failed`.
7. Caller cancellation advances to `Interrupted` only after the active executor settles.
8. `CancelLaunch(reason)` becomes the root-owned cooperative cancellation command approved by the package specification.
9. A root cannot begin a second launch after its session advances or while a run is active.
10. Destroying an active root requests cancellation, releases authority, suppresses late publications, and allows the executor to settle safely.
11. Existing direct runner calls and all 288 retained Runtime Play Mode tests remain valid.

The visible proof is a root whose public `State` and `Progress` now reflect actual startup-sequence execution instead of manually injected test snapshots.

---

## 2. Starting Conditions

- `main` and `origin/main` are synchronized at `485a09f`.
- Working tree is clean.
- FL-M3-05 implementation is complete in `b70a100`.
- FL-M3-05 documentation is complete in `485a09f`.
- Unity compilation reports 0 errors and 0 compiler warnings.
- Runtime Play Mode reports 288 passed, 0 failed, 0 ignored.
- The runner performs complete preflight before executor creation.
- The runner rejects concurrent re-entry and releases its gate after settlement.
- Caller cancellation already returns a structured `StartupSequenceRunResult` after executor settlement.
- `EchoLaunchRoot` already owns one `LaunchSession` and publishes validated state/progress notifications.
- No report, destination loader, splash presenter, or automatic launch callback exists.

If any starting condition is false, stop and reconcile the repository before applying this checkpoint.

---

## 3. Authority and Constraints

The package specification assigns launch-session ownership to `EchoLaunchRoot`, startup-step attempts to the sequence runner, and mutable progress to fresh runtime objects. The root claims in `Awake`, but execution begins through an explicit internal start gate after authority claim and serialized-reference validation.

FL-M3-06 must preserve:

- One authoritative root.
- Duplicate rejection before launch side effects.
- One `LaunchSession` per root authority.
- `LaunchStateTransitionRules` as the lifecycle legality authority.
- Immutable `LaunchProgressSnapshot` publication.
- Listener-failure isolation through `ELAUNCH-EVENT-001`.
- Complete sequence preflight before executor factories.
- Runner-local re-entry protection through `ELAUNCH-RUN-001`.
- Monotonic timeout behavior.
- Cooperative cancellation and settlement before traversal return.
- Existing direct runner test and extension seams.
- Package independence and no peer-package dependency.

A successful sequence cannot publish `Completed` in this checkpoint. The package specification defines completion after destination activation and handoff, which are not yet implemented.

---

## 4. Scope

### 4.1 Root-owned explicit start

Add one internal explicit start method:

```text
EchoLaunchRoot.StartLaunchAsync()
```

It must:

- Require a live authoritative root.
- Require the session to remain at `AuthorityClaimed`.
- Atomically reject concurrent or repeated root starts.
- Create one root-owned cancellation source.
- Publish `Validating` before runner preflight.
- Invoke the root-owned runner exactly once.
- Retain the settled internal `StartupSequenceRunResult` for later reporting work.
- Dispose cancellation ownership through `finally`.
- Release the active-run gate after every terminal path.

The method remains internal. Automatic production invocation is deferred.

### 4.2 Runner observation seam

Add one internal optional observer seam so the runner can publish accepted runtime facts without depending on scene-facing root code.

The observer receives:

- Sequence validated.
- Step started.
- Accepted step progress changed.
- Step completed.

The existing three-argument `RunAsync` overload must remain unchanged for existing tests and direct runner use. It delegates to the observer-aware overload with no observer.

### 4.3 Progress relay

Add one internal progress relay that:

1. Records progress in the authoritative `StartupStepExecution`.
2. Forwards the same immutable progress value to the optional observer.
3. Remains behind the existing late-progress gate.

Late progress after timeout or cancellation must remain suppressed before it reaches either execution state or the root.

### 4.4 Structured preflight failure identity

Preserve existing `InvalidOperationException` compatibility while introducing an internal derived preflight exception that carries:

- Stable diagnostic code.
- Human-readable failure message.

This lets the root publish the correct failure result without parsing exception text.

### 4.5 Lifecycle mapping

The root maps execution to lifecycle as follows:

| Runner/root condition | Root lifecycle result |
|---|---|
| Explicit start accepted | `Validating` |
| Sequence preflight accepted | `Running` |
| Step start/progress/completion | `Running` with updated snapshot |
| Sequence succeeds or completes with warnings | `Transitioning` |
| Blocking or effective failure | `Failed` |
| Root/caller cancellation after settlement | `Interrupted` |
| Preflight rejection | `Failed` with matching diagnostic |
| Unexpected runner contract failure | `Failed` with sanitized details |

### 4.6 Root cancellation command

Add the approved public command:

```text
bool CancelLaunch(string reason)
```

It must:

- Return `false` when the root is not authoritative.
- Return `false` when no launch is active.
- Return `false` after cancellation was already requested.
- Normalize a blank reason to a stable default message.
- Request cancellation once through the root-owned token source.
- Never abandon the active executor.
- Publish `Interrupted` only after runner settlement.

### 4.7 Root start diagnostic

Use stable root lifecycle diagnostics:

```text
ELAUNCH-LIFE-001
ELAUNCH-LIFE-002
```

- `ELAUNCH-LIFE-001` identifies root lifecycle interruption or destruction-related cancellation boundaries.
- `ELAUNCH-LIFE-002` identifies invalid root start or runner-replacement timing.

### 4.8 Destruction behavior

When an authoritative root is destroyed during launch:

- Mark the root as destroying before cancellation.
- Request cancellation once.
- Clear event delegates.
- Release static authority.
- Suppress all late root progress and lifecycle publication.
- Allow the runner and executor to settle through retained local references.
- Dispose the cancellation source when the asynchronous method reaches `finally`.

---

## 5. Explicit Exclusions

FL-M3-06 does not authorize:

- Automatic startup from `Awake`, `Start`, scene callbacks, or application callbacks.
- `LaunchCompleted`, `LaunchFailed`, `LaunchInterrupted`, `StepStarted`, `StepProgressChanged`, or `StepCompleted` public events.
- `LaunchReport`, `LaunchReportBuilder`, report export, or `LastReport`.
- Destination definitions, destination validation, scene loading, or handoff completion.
- Publishing `LaunchStatus.Completed`.
- Splash sequence definitions or presentation.
- `ILaunchStatusPresenter` or uGUI work.
- Direct-scene initializer behavior.
- Boot scene, prefab, Test Lab, or sample work.
- Editor setup, repair, migration, or validation windows.
- Retry, backoff, interactive retry, or skip UI.
- Dependency-graph validation.
- Public runner APIs.
- Peer-package bridges.
- Package version change.
- Serialized schema change.

---

## 6. Files and Assets

| Path | Action | Ownership | Purpose |
|---|---|---|---|
| `Packages/com.echodevgames.echo-launch/Runtime/Core/EchoLaunchRoot.cs` | Modify | Runtime authority | Own explicit launch run, cancellation, lifecycle mapping, and destruction boundary |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/IStartupSequenceObserver.cs` | Create | Runtime internal | Decouple runner observations from root implementation |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/IStartupSequenceObserver.cs.meta` | Create by Unity | Unity identity | Preserve script GUID |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupStepProgressRelay.cs` | Create | Runtime internal | Record and forward accepted progress behind the existing gate |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupStepProgressRelay.cs.meta` | Create by Unity | Unity identity | Preserve script GUID |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflightException.cs` | Create | Runtime internal | Carry stable preflight code/message while retaining exception compatibility |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflightException.cs.meta` | Create by Unity | Unity identity | Preserve script GUID |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflight.cs` | Modify | Runtime internal | Throw structured preflight exception for authored-data rejection |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequenceRunner.cs` | Modify | Runtime internal | Add optional observer overload and accepted lifecycle notifications |
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs` | Create | Runtime tests | Prove explicit root ownership, lifecycle, cancellation, re-entry, and destruction |
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs.meta` | Create by Unity | Unity identity | Preserve test script GUID |
| `Plan Documentation/Checkpoint Build Plans/FL-M3-06_Root-Owned_Startup_Run_and_Lifecycle_Advancement_Checkpoint_Build_Plan.md` | Create | Planning | Authoritative bounded checkpoint plan |

No scene, prefab, configuration asset, assembly definition, manifest, package lock, or project setting is authorized.

---

## 7. Implementation Sequence

### Phase 1 — Neutral observation seam

1. Create `IStartupSequenceObserver`.
2. Create `StartupStepProgressRelay`.
3. Preserve the existing runner overload.
4. Add an internal observer-aware overload.
5. Notify only after authoritative sequence/execution state changes.
6. Keep observer calls optional.
7. Keep late progress behind `StartupStepProgressGate`.

### Phase 2 — Structured preflight identity

1. Create `StartupSequencePreflightException : InvalidOperationException`.
2. Preserve current exception-message diagnostic formatting.
3. Expose diagnostic code and normalized failure message internally.
4. Update preflight rejection construction only.
5. Preserve null configuration and invalid launch-mode exception types.

### Phase 3 — Root-owned lifecycle

1. Extend `EchoLaunchRoot` with a runner, cancellation source, active-run gate, and last sequence result.
2. Keep `Awake` limited to authority/session/runner creation.
3. Add internal `StartLaunchAsync`.
4. Publish `Validating` before runner invocation.
5. Translate observer callbacks into immutable `Running` snapshots.
6. Map settled runner outcomes to `Transitioning`, `Failed`, or `Interrupted`.
7. Add public `CancelLaunch(reason)`.
8. Cancel active work during destruction and suppress late publication.
9. Keep `PublishProgress` and notification isolation unchanged.

### Phase 4 — Automated proof

1. Add one self-contained root lifecycle fixture.
2. Prove no automatic launch in `Awake`.
3. Prove empty, success, warning, failure, and preflight paths.
4. Prove state-event ordering.
5. Prove step start/progress/completion snapshots.
6. Prove cancellation waits for executor settlement.
7. Prove normalized and preserved cancellation reasons.
8. Prove repeated cancellation rejection.
9. Prove concurrent and repeated root-start rejection.
10. Prove duplicate roots cannot start or cancel.
11. Prove destruction cancellation and late-publication suppression.
12. Prove success stops at `Transitioning`.
13. Prove direct-scene mode is preserved.
14. Prove authored assets remain immutable.
15. Prove the active root gate clears after preflight failure.

---

## 8. Visible Code and Learning Rule

Before application, provide the complete compile-ready contents of:

- `EchoLaunchRoot.cs`
- `IStartupSequenceObserver.cs`
- `StartupStepProgressRelay.cs`
- `StartupSequencePreflightException.cs`
- `StartupSequencePreflight.cs`
- `StartupSequenceRunner.cs`
- `EchoLaunchRootStartupLifecycleTests.cs`

Explain:

- Why the root owns the session while the runner owns step attempts.
- Why the observer is internal rather than public events.
- Why progress first reaches `StartupStepExecution` and only then the root.
- Why the root uses a second atomic gate in addition to the runner gate.
- Why cancellation returns only after settlement.
- Why destruction suppresses late publication instead of abandoning execution.
- Why success stops at `Transitioning`.
- Why automatic startup, reports, and destination loading remain excluded.

Provide one CMD-first apply bundle and stop first at the Unity compile gate.

---

## 9. Unity Editor Setup

No manual scene or Inspector setup is required.

Unity will:

- Recompile the runtime and test assemblies.
- Generate four new `.meta` files.
- Discover the new Runtime Play Mode fixture.

Expected Inspector-visible change:

- None. `EchoLaunchRoot` retains its existing serialized configuration and launch-mode fields.

`StartLaunchAsync` is internal and exercised by the package Runtime test assembly only during this checkpoint.

---

## 10. Validation and Tests

### 10.1 Compile gate

| Test | Expected result |
|---|---|
| Unity script compilation | 0 errors |
| Unity compiler warnings | 0 |
| Runtime assembly boundary | No `UnityEditor` reference |
| Package dependencies | Unchanged |
| Serialized root fields | Existing fields preserved |

### 10.2 Root lifecycle fixture

| ID | Proof | Expected result |
|---|---|---|
| FL-M3-06-T01 | Root `Awake` | Authority claimed; no automatic sequence run |
| FL-M3-06-T02 | Empty explicit run | `Validating` → `Running` → `Transitioning` |
| FL-M3-06-T03 | Successful step | Approved state-event order |
| FL-M3-06-T04 | Step lifecycle | Start, accepted progress, and completion snapshots published |
| FL-M3-06-T05 | Warning run | Warning retained; root reaches `Transitioning` |
| FL-M3-06-T06 | Blocking run | Root reaches `Failed` with blocking result |
| FL-M3-06-T07 | Invalid configuration identity | Root reaches `Failed`; no executor factory |
| FL-M3-06-T08 | Missing configuration | Root reaches `Failed` with `ELAUNCH-CFG-001` |
| FL-M3-06-T09 | Cancel while idle | Returns `false`; state unchanged |
| FL-M3-06-T10 | Active cancellation | Waits for settlement, then reaches `Interrupted` |
| FL-M3-06-T11 | Blank cancellation reason | Stable default message used |
| FL-M3-06-T12 | Repeated cancellation | First accepted, second rejected |
| FL-M3-06-T13 | Concurrent root start | Rejected with `ELAUNCH-LIFE-002`; no second factory |
| FL-M3-06-T14 | Restart after success | Rejected with `ELAUNCH-LIFE-002` |
| FL-M3-06-T15 | Restart after failure | Rejected with `ELAUNCH-LIFE-002` |
| FL-M3-06-T16 | Duplicate root | Cannot start or cancel; authority remains unchanged |
| FL-M3-06-T17 | Destroy active root | Cancellation requested; executor settles; no late publication |
| FL-M3-06-T18 | Successful sequence | Does not publish `Completed` before destination work |
| FL-M3-06-T19 | Direct-scene mode | Preserved through root-owned run |
| FL-M3-06-T20 | Authored data | No configuration/sequence/entry/definition mutation |
| FL-M3-06-T21 | Runner replacement timing | Rejected after lifecycle advancement |
| FL-M3-06-T22 | Preflight terminal path | Root active gate clears |
| FL-M3-06-T23 | Interrupted state event | Published exactly once |

### 10.3 Full regression gate

Starting total:

```text
288 passed
0 failed
0 ignored
```

Expected final total after 23 new tests:

```text
311 passed
0 failed
0 ignored
```

Expected yellow runtime diagnostics remain:

```text
ELAUNCH-ROOT-001
ELAUNCH-EVENT-001
```

These are intentional Runtime test diagnostics, not compiler warnings or test failures.

---

## 11. Failure Symptoms and Fixes

### Root reaches `Completed`

Cause: sequence success was mapped beyond the authorized destination boundary.

Fix: publish `Transitioning` only. Destination activation owns the later move to `Completed`.

### Root remains `Validating`

Cause: observer was not notified after successful preflight.

Fix: call `SequenceValidated` after preflight and before traversal.

### Progress appears in the execution but not on the root

Cause: the context reporter still points directly at `StartupStepExecution`.

Fix: route accepted progress through `StartupStepProgressRelay` behind the existing gate.

### Late progress appears after cancellation or timeout

Cause: observer forwarding bypasses `StartupStepProgressGate`.

Fix: keep relay inside the gate, never outside it.

### Cancellation returns before executor settlement

Cause: root returned on token request rather than awaiting runner completion.

Fix: request cancellation only; await the structured runner result.

### Destroyed root publishes terminal events

Cause: observer does not check root destruction/authority state.

Fix: reject late publication through the root’s runtime-publication guard.

### Existing runner tests fail

Cause: the original `RunAsync` contract changed.

Fix: preserve the three-argument overload and delegate to the optional observer overload.

### Existing root tests fail

Cause: `Awake` started work or changed authority/session defaults.

Fix: keep `Awake` claim-only and retain initial `AuthorityClaimed` state.

---

## 12. Rollback and Recovery

Before commit:

```cmd
git restore --staged .
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Core/EchoLaunchRoot.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflight.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequenceRunner.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/IStartupSequenceObserver.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/IStartupSequenceObserver.cs.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupStepProgressRelay.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupStepProgressRelay.cs.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflightException.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflightException.cs.meta"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/EchoLaunchRootStartupLifecycleTests.cs.meta"
git clean -f -- "Plan Documentation/Checkpoint Build Plans/FL-M3-06_Root-Owned_Startup_Run_and_Lifecycle_Advancement_Checkpoint_Build_Plan.md"
```

After commit, use `git revert <FL-M3-06-IMPLEMENTATION-COMMIT>` rather than rewriting shared history.

No project-owned content, scenes, prefabs, Build Settings, or configuration assets are modified.

---

## 13. Documentation Reconciliation

At closeout, batch-update:

- Package checkpoint record.
- Package Runtime Play Mode test report.
- Root implementation completion record.
- Package `Documentation~/Developer/Architecture.md`.
- Package `Documentation~/Developer/Current Notes.md`.
- Package `CHANGELOG.md`.
- Package `README.md`.
- Package documentation index.
- Root `Plan Documentation/Current Notes.md`.
- FL-M3-05 pending documentation-commit references with confirmed `485a09f` evidence.

The package specification changes only if implementation reveals a genuine contract change. The public `CancelLaunch(reason)` concept and root-owned lifecycle are already approved.

---

## 14. Commit and Push Plan

### Implementation commit

```text
echo-launch: complete FL-M3-06 root-owned startup lifecycle
```

### Adjacent documentation commit

```text
echo-launch: document FL-M3-06 completion
```

The assistant must not claim either commit or push without Jesse’s CMD evidence.

---

## 15. Completion Criteria

- [ ] Root `Awake` remains claim-only.
- [ ] Explicit internal root start exists.
- [ ] Root owns one cancellation source and active-run gate.
- [ ] Concurrent and repeated root starts reject with `ELAUNCH-LIFE-002`.
- [ ] Runner retains its existing direct-call overload.
- [ ] Optional observer receives accepted lifecycle observations.
- [ ] Progress relay remains behind late-progress containment.
- [ ] Preflight failures retain stable diagnostic identity.
- [ ] Root publishes `Validating` before preflight.
- [ ] Root publishes `Running` after successful preflight.
- [ ] Root publishes step start, progress, and completion snapshots.
- [ ] Success and warnings reach `Transitioning`.
- [ ] Blocking outcomes reach `Failed`.
- [ ] Cancellation reaches `Interrupted` after settlement.
- [ ] Destruction requests cancellation and suppresses late publication.
- [ ] Root does not publish `Completed`.
- [ ] Authored assets remain immutable.
- [ ] Unity compiles with 0 errors and 0 compiler warnings.
- [ ] Runtime Play Mode reports 311 passed, 0 failed, 0 ignored.
- [ ] Implementation commit is pushed.
- [ ] Documentation closeout is pushed.
- [ ] Working tree is clean and synchronized.

---

## 16. Stop Point

Stop after one explicit root-owned startup sequence reaches `Transitioning`, `Failed`, or `Interrupted`, and after documentation is reconciled.

Do not add automatic startup.

Do not add destination loading or mark launch `Completed`.

Do not add reports, public lifecycle events, presentation, splashes, direct-scene initialization, scenes, prefabs, or Editor tooling.

---

## 17. Next Recommended Checkpoint

**FL-M3-07 — Immutable Launch Report and Public Terminal Events**

Tentative combined outcome:

- Build one immutable report from preflight, completed step executions, warnings, failure, cancellation, and timing.
- Expose `LastReport`.
- Publish `LaunchFailed` and `LaunchInterrupted` after report finalization.
- Prepare the success report for later destination handoff without falsely publishing `LaunchCompleted`.

This checkpoint is not authorized by FL-M3-06.

---

## 18. Handoff Record

| Field | Value |
|---|---|
| Package | First Light (`EchoLaunch`) |
| Package version | `0.1.0` |
| Specification version | `1.3.0` |
| Checkpoint | FL-M3-06 — Root-Owned Startup Run and Lifecycle Advancement |
| Starting implementation commit | `b70a100` |
| Starting documentation commit | `485a09f` |
| Starting compilation | 0 errors, 0 compiler warnings |
| Starting Runtime Play Mode | 288 passed, 0 failed, 0 ignored |
| Known blockers | None |
| Implementation outcome | Pending |
| Documentation outcome | Pending |
| Next checkpoint | FL-M3-07, tentative only |

---

## 19. Approval

**Decision:** Approve
**Approver:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Preserve explicit start, package independence, lifecycle legality, runner compatibility, settlement-safe cancellation, and the `Transitioning` stop boundary before destination work.
