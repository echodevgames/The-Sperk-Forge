# FL-M3-07 — Immutable Launch Report and Public Terminal Events

**Document ID:** FL-M3-07
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
**Milestone:** M3 — Startup Sequence Runtime
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Repository/workspace:** The Sperk’s Forge
**Unity baseline:** Unity 6000.3.8f1
**Implementation baseline:** `d728602`
**Previous implementation commit:** `e0e9645`
**Previous documentation commit:** `d728602`
**Workflow authority:** SFGSS-005 v1.4.0
**Authorized:** August 5, 2026

> The launch has spoken. This checkpoint seals its testimony in an immutable report and rings the correct terminal bell exactly once.

---

## 1. Purpose and Observable Outcome

FL-M3-07 adds the first complete structured reporting boundary to First Light.

When the checkpoint is complete:

1. Every failed or interrupted root-owned launch finalizes exactly one immutable `LaunchReport`.
2. The report records launch identity, mode, lifecycle outcome, timing, authored traversal accounting, warnings, failures, cancellation, and immutable per-step summaries.
3. `EchoLaunchRoot.LastReport` exposes the latest finalized report only from the authoritative root.
4. `LaunchFailed` is raised exactly once after a failed report is finalized and the authoritative state is already `Failed`.
5. `LaunchInterrupted` is raised exactly once after an interrupted report is finalized and the authoritative state is already `Interrupted`.
6. Listener failures are isolated through the existing notification dispatcher.
7. Successful sequence execution at `Transitioning` retains a report builder but does not finalize `LaunchReport` and does not publish `LaunchCompleted`.
8. Destination handoff can later finalize the successful report without replacing an already-published failed or interrupted report.
9. Existing 311 Runtime Play Mode tests remain green.
10. Authored ScriptableObject data remains unchanged.

The observable proof is a Runtime Play Mode fixture showing report immutability, complete step copying, exact terminal-event ordering, exactly-once publication, listener isolation, destruction behavior, and the deliberate absence of a completed report before destination activation.

---

## 2. Starting Conditions

- `main` and `origin/main` are synchronized at `d728602`.
- Working tree is clean.
- FL-M3-06 implementation is complete in `e0e9645`.
- FL-M3-06 documentation is complete in `d728602`.
- Unity compilation reports 0 errors and 0 compiler warnings.
- Runtime Play Mode reports 311 passed, 0 failed, 0 ignored.
- The authoritative root explicitly owns one startup-sequence run.
- Success currently stops at `LaunchStatus.Transitioning`.
- Failed and interrupted states currently publish snapshots but no immutable report.
- No destination loader or completed handoff exists.
- No unresolved authority decision blocks report construction or terminal failure/interruption events.

If any starting condition is false, stop and reconcile the repository before applying this checkpoint.

---

## 3. Authority and Constraints

The approved package specification requires:

- One structured `LaunchReport`.
- One immutable per-step report representation.
- `EchoLaunchRoot.LastReport`.
- `LaunchFailed` after the failure report is finalized.
- `LaunchInterrupted` after the interruption report is finalized.
- `LaunchCompleted` only after destination activation and report completion.
- A report schema version for future export compatibility.
- Reports that are diagnostic session artifacts, not EchoSave data.
- Sanitized and bounded diagnostic details.
- No mutation of authored configuration or sequence assets.

FL-M3-07 must preserve:

- Package independence.
- Neutral technical runtime APIs.
- No peer Sperk’s Forge dependency.
- No Editor runtime reference.
- No public mutable collections.
- No report replacement after finalization.
- No false successful terminal event at `Transitioning`.
- Existing root lifecycle and cancellation settlement behavior.
- Existing runner compatibility behavior.
- Existing listener-isolation diagnostic `ELAUNCH-EVENT-001`.
- Existing destruction suppression of unsafe late publication.

---

## 4. Report Model

### 4.1 `LaunchStepReport`

Add one public immutable per-step summary.

Required data:

- Entry ID.
- Step ID.
- Step display name.
- Authored zero-based index.
- Authored entry count.
- Authored `StartupStepPolicy`.
- Final `StartupStepStatus`.
- Immutable `StartupStepResult`.
- Final accepted `StartupStepProgress`.
- Start time in monotonic seconds.
- Settlement time in monotonic seconds.
- Elapsed seconds.
- Timeout seconds.
- Whether timeout was configured.
- Whether timeout was reached.

Rules:

- Copy values from `StartupStepExecution`.
- Never expose the mutable/internal execution object.
- Normalize text through existing immutable source values.
- Validate finite nonnegative timing.
- Preserve authored order.
- Provide no setters.
- Provide no mutable collection.
- Remain valid after the runner and root release their active references.

### 4.2 `LaunchReport`

Add one public immutable launch summary.

Required data:

- Report schema version.
- Package version.
- Launch mode.
- Configuration ID.
- Sequence ID.
- Finalized lifecycle status.
- Launch start time in monotonic seconds.
- Finalization time in monotonic seconds.
- Total elapsed seconds.
- Authored entry count.
- Attempted step count.
- Disabled entry count.
- Unvisited entry count.
- Warning count.
- Failure count.
- Blocking failure count.
- Whether cancellation occurred.
- Stable final diagnostic/result when available.
- Immutable ordered step reports.

Rules:

- Current report schema version begins at `1`.
- Finalized status in FL-M3-07 may be only `Failed` or `Interrupted`.
- `Transitioning` is not a finalized report status in this checkpoint.
- `Completed` remains unavailable until destination handoff.
- The report copies all step data into a private array.
- Public step access is read-only by index and count.
- Constructor validation rejects inconsistent accounting or invalid timing.
- Report data is session-only and is not serialized into authored assets.
- Export is not implemented.

### 4.3 `LaunchReportBuilder`

Add one internal single-use builder owned by the authoritative root.

The builder records:

- Launch mode.
- Configuration and sequence identity when known.
- Launch monotonic start.
- Validated authored count.
- Completed step summaries.
- Run-level accounting.
- Preflight or unexpected terminal result.
- Cancellation result and reason.
- Final lifecycle status.

Builder rules:

- Created once when an accepted root launch starts.
- Receives step completion observations after the execution is terminal.
- Prevents duplicate step capture.
- Preserves authored step order.
- May retain a successful transition-pending state without finalizing.
- Finalizes exactly once for `Failed` or `Interrupted`.
- Rejects a second finalization attempt.
- Does not publish events itself.
- Does not depend on scenes, presenters, destinations, or Editor code.

---

## 5. Root Integration

### 5.1 `LastReport`

Add the approved public property:

```csharp
public LaunchReport LastReport { get; }
```

Behavior:

- Returns `null` before any report is finalized.
- Returns `null` while the launch is active.
- Returns the immutable failed report after `Failed`.
- Returns the immutable interrupted report after `Interrupted`.
- Returns `null` at `Transitioning` in FL-M3-07 because successful handoff is not finished.
- Returns `null` from a duplicate root.
- Does not expose the internal builder.

### 5.2 Public terminal events

Add:

```csharp
public event Action<LaunchReport> LaunchFailed;
public event Action<LaunchReport> LaunchInterrupted;
```

Event ordering:

1. Final terminal progress snapshot is accepted.
2. Root state is already `Failed` or `Interrupted`.
3. Immutable report is finalized.
4. `LastReport` points to that exact report instance.
5. Matching terminal event is dispatched.
6. Listener failures are isolated.
7. No other terminal event fires.

Rules:

- One matching event per accepted launch.
- No event from duplicate roots.
- No event before report finalization.
- No event after root destruction if destruction suppression makes publication unsafe.
- Event listeners cannot mutate the report.
- `LaunchFailed` never fires for interruption.
- `LaunchInterrupted` never fires for failure.
- `LaunchCompleted` remains absent.

### 5.3 Root lifecycle paths

Finalize failed report for:

- Missing configuration.
- Invalid launch mode.
- Structured preflight rejection.
- Unexpected runner exception.
- Null runner result.
- Blocking or failed sequence result.

Finalize interrupted report for:

- Structured runner cancellation.
- Root cancellation.
- Cancellation exception associated with the active root request.
- Destruction only when finalization can be safely completed before public publication is suppressed.

At successful `Transitioning`:

- Record the settled sequence result in the builder.
- Keep `LastReport == null`.
- Raise no terminal report event.
- Preserve the builder for the later destination checkpoint.

---

## 6. Exact Files and Assets

| Path | Action | Ownership | Purpose |
|---|---|---|---|
| `Packages/com.echodevgames.echo-launch/Runtime/Reports/LaunchStepReport.cs` | Create | Public runtime | Immutable copy of one terminal step execution |
| `Packages/com.echodevgames.echo-launch/Runtime/Reports/LaunchStepReport.cs.meta` | Create by Unity | Unity identity | Stable script GUID |
| `Packages/com.echodevgames.echo-launch/Runtime/Reports/LaunchReport.cs` | Create | Public runtime | Immutable finalized launch summary |
| `Packages/com.echodevgames.echo-launch/Runtime/Reports/LaunchReport.cs.meta` | Create by Unity | Unity identity | Stable script GUID |
| `Packages/com.echodevgames.echo-launch/Runtime/Reports/LaunchReportBuilder.cs` | Create | Internal runtime | Single-use report assembly and finalization |
| `Packages/com.echodevgames.echo-launch/Runtime/Reports/LaunchReportBuilder.cs.meta` | Create by Unity | Unity identity | Stable script GUID |
| `Packages/com.echodevgames.echo-launch/Runtime/Core/EchoLaunchRoot.cs` | Modify | Runtime authority | Own builder, expose `LastReport`, finalize reports, publish terminal events |
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs` | Create | Runtime tests | Prove report contract and event behavior |
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs.meta` | Create by Unity | Unity identity | Stable test-script GUID |
| `Plan Documentation/Checkpoint Build Plans/FL-M3-07_Immutable_Launch_Report_and_Public_Terminal_Events_Checkpoint_Build_Plan.md` | Create | Planning | Authoritative bounded implementation plan |

No other file is authorized in the implementation commit unless compilation reveals a directly checkpoint-owned defect.

---

## 7. Explicit Exclusions

FL-M3-07 does not authorize:

- `LaunchCompleted`.
- Publishing `LaunchStatus.Completed`.
- Destination definitions.
- Destination validation.
- Scene loading.
- Destination activation proof.
- Success-report finalization.
- Report export to JSON, text, disk, clipboard, email, or bug bundle.
- Public `StepStarted`, `StepProgressChanged`, or `StepCompleted` events.
- `AuthorityClaimed` public event.
- Automatic startup from `Awake`, `Start`, or scene callbacks.
- Splash definitions or presentation.
- `ILaunchStatusPresenter`.
- Direct-scene initializer behavior.
- Persistent-root lifetime changes.
- Boot scene, prefab, Test Lab, or samples.
- Editor setup, repair, migration, or validation tooling.
- Retry, backoff, interactive retry, or skip UI.
- Dependency graph validation.
- Peer-package bridges.
- Package version change.
- Serialized configuration schema change.
- Durable save integration.

---

## 8. Implementation Sequence

### Phase 1 — Immutable report values

1. Create `LaunchStepReport`.
2. Create `LaunchReport`.
3. Validate constructor invariants.
4. Copy arrays defensively.
5. Expose indexed reads and counts only.
6. Keep all fields read-only after construction.

### Phase 2 — Internal builder

1. Create `LaunchReportBuilder`.
2. Initialize from accepted root start.
3. Record configuration and sequence identity when known.
4. Capture each completed execution exactly once.
5. Reconcile the final run result.
6. Support preflight and unexpected failure without a run result.
7. Support interruption with or without a settled execution.
8. Finalize once for `Failed` or `Interrupted`.
9. Preserve transition-pending success without finalizing.

### Phase 3 — Root terminal publication

1. Add `LastReport`.
2. Add `LaunchFailed`.
3. Add `LaunchInterrupted`.
4. Create the builder at launch start.
5. Forward completed-step observations into the builder.
6. Finalize after terminal lifecycle snapshot acceptance.
7. Set `LastReport` before event dispatch.
8. Dispatch through `LaunchNotificationDispatcher`.
9. Clear new event delegates on destruction.
10. Preserve success at `Transitioning` with no terminal report event.

### Phase 4 — Automated proof

1. Add one self-contained Runtime Play Mode fixture.
2. Prove failed preflight report.
3. Prove failed blocking-step report.
4. Prove interrupted report after settlement.
5. Prove report step order and immutable copied values.
6. Prove warning and failure accounting.
7. Prove disabled and unvisited accounting.
8. Prove timing copy.
9. Prove `LastReport` identity.
10. Prove event sees the already-updated root state and report.
11. Prove exactly-once publication.
12. Prove listener failure isolation.
13. Prove duplicate-root silence.
14. Prove transition-pending success produces no finalized report.
15. Prove authored assets remain unchanged.

---

## 9. Visible Code and Learning Rule

Before applying implementation:

- Provide the complete contents of every created or replaced C# file.
- State each exact repository path.
- Explain why the public report contains copies rather than internal execution references.
- Explain why the report schema version is independent from package and configuration schema versions.
- Explain the builder’s single-use finalization guard.
- Explain the event ordering relative to lifecycle state and `LastReport`.
- Explain why success at `Transitioning` does not produce `LaunchCompleted` or a finalized report.
- Explain why report export remains separate.
- Provide one CMD-first apply bundle.
- Stop first at the Unity compile gate.
- Run the complete Runtime Play Mode suite only after clean compilation.

---

## 10. Unity Editor Setup

No scene, prefab, Inspector, Build Settings, package manifest, or project-setting change is required.

Unity will:

- Create the `Runtime/Reports` folder if absent.
- Import three new runtime scripts.
- Generate three runtime `.meta` files.
- Import one new test fixture.
- Generate its `.meta` file.
- Recompile runtime and Runtime Play Mode test assemblies.

Expected Inspector-visible changes: none.

---

## 11. Validation and Tests

### 11.1 Compile gate

| Test | Expected result |
|---|---|
| Unity script compilation | 0 errors |
| Unity compiler warnings | 0 |
| Runtime assembly boundary | No `UnityEditor` reference |
| Package dependencies | Unchanged |
| Serialized configuration schema | Unchanged |

### 11.2 Report model tests

| ID | Action | Expected result |
|---|---|---|
| FL-M3-07-T01 | Build one step report | All copied values match terminal execution |
| FL-M3-07-T02 | Mutate source list after report creation | Report remains unchanged |
| FL-M3-07-T03 | Read step reports by index | Authored order preserved |
| FL-M3-07-T04 | Use invalid index | Range exception |
| FL-M3-07-T05 | Supply invalid timing/accounting | Constructor rejects |
| FL-M3-07-T06 | Inspect schema | Current report schema is `1` |
| FL-M3-07-T07 | Inspect public surface | No mutable collection or setter |

### 11.3 Failed report tests

| ID | Action | Expected result |
|---|---|---|
| FL-M3-07-T08 | Missing configuration | Failed report finalized with `ELAUNCH-CFG-001` |
| FL-M3-07-T09 | Invalid preflight | Failed report finalized before any executor |
| FL-M3-07-T10 | Blocking step | Failed report contains terminal step |
| FL-M3-07-T11 | Warning then blocking step | Warning and failure counts are correct |
| FL-M3-07-T12 | Disabled and unvisited entries | Accounting balances authored count |
| FL-M3-07-T13 | Observe root | `LastReport` is exact event payload instance |
| FL-M3-07-T14 | Observe event timing | Root state is already `Failed` |
| FL-M3-07-T15 | Add failing first listener | Later listener still receives report |
| FL-M3-07-T16 | Complete one failed launch | `LaunchFailed` fires exactly once |
| FL-M3-07-T17 | Complete one failed launch | `LaunchInterrupted` does not fire |

### 11.4 Interrupted report tests

| ID | Action | Expected result |
|---|---|---|
| FL-M3-07-T18 | Cancel active multi-frame step | Report finalizes after settlement |
| FL-M3-07-T19 | Inspect interrupted report | `WasCancelled` true and final status `Interrupted` |
| FL-M3-07-T20 | Observe event timing | Root state is already `Interrupted` |
| FL-M3-07-T21 | Observe report identity | `LastReport` is exact event payload instance |
| FL-M3-07-T22 | Complete cancellation | `LaunchInterrupted` fires exactly once |
| FL-M3-07-T23 | Complete cancellation | `LaunchFailed` does not fire |
| FL-M3-07-T24 | Blank cancellation reason | Stable normalized message recorded |
| FL-M3-07-T25 | Failing listener | Later interrupted listener still runs |

### 11.5 Transition-pending success tests

| ID | Action | Expected result |
|---|---|---|
| FL-M3-07-T26 | Complete successful sequence | Root reaches `Transitioning` |
| FL-M3-07-T27 | Inspect root | `LastReport` remains null |
| FL-M3-07-T28 | Observe events | No failed or interrupted event |
| FL-M3-07-T29 | Inspect internal builder through test seam | Success data retained for later destination finalization |
| FL-M3-07-T30 | Inspect lifecycle | `Completed` is never published |

### 11.6 Safety tests

| ID | Action | Expected result |
|---|---|---|
| FL-M3-07-T31 | Duplicate root attempts launch/report access | No terminal event and no report |
| FL-M3-07-T32 | Destroy root before safe public finalization | No unsafe late event |
| FL-M3-07-T33 | Complete report | Authored assets unchanged |
| FL-M3-07-T34 | Attempt second builder finalization | Rejected |
| FL-M3-07-T35 | Retain report after active runtime references release | Report remains readable |

### 11.7 Full regression gate

Starting retained baseline:

```text
311 passed
0 failed
0 ignored
```

The closeout records the exact discovered final total. It must not claim a predicted number as evidence.

---

## 12. Failure Symptoms and Fixes

### Report changes after source mutation

Cause: public report retained an internal array, list, or execution reference.

Fix: copy all report values and arrays during construction.

### Terminal event sees stale root state

Cause: event dispatched before the final progress snapshot is accepted.

Fix: publish terminal snapshot first, then finalize/store report, then dispatch event.

### Event fires twice

Cause: both a lifecycle path and `finally` dispatch the same event.

Fix: centralize finalization and event dispatch behind one guarded method.

### Success publishes a report too early

Cause: sequence settlement is treated as full launch completion.

Fix: retain the builder at `Transitioning`; finalize only after later destination activation.

### Destruction emits noisy event

Cause: report finalization ignores `isDestroying` and publication suppression.

Fix: allow internal settlement bookkeeping but suppress unsafe public dispatch after destruction begins.

### Retained tests fail

Cause: report integration altered runner, timeout, lifecycle, or compatibility contracts.

Fix: restore those contracts and limit changes to report capture and post-terminal dispatch.

---

## 13. Rollback and Recovery

Before commit:

```cmd
git restore --staged .
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Core/EchoLaunchRoot.cs"
git clean -fd -- "Packages/com.echodevgames.echo-launch/Runtime/Reports"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/LaunchReportAndTerminalEventTests.cs.meta"
git clean -f -- "Plan Documentation/Checkpoint Build Plans/FL-M3-07_Immutable_Launch_Report_and_Public_Terminal_Events_Checkpoint_Build_Plan.md"
```

After commit, use `git revert <FL-M3-07-IMPLEMENTATION-COMMIT>` rather than rewriting shared history.

No project-owned content, scene, prefab, configuration asset, Build Settings entry, or dependency is modified.

---

## 14. Documentation Reconciliation

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
- FL-M3-06 pending documentation-commit references with confirmed `d728602` evidence.

The package specification changes only if implementation reveals a genuine contract change. The immutable report, `LastReport`, `LaunchFailed`, and `LaunchInterrupted` concepts are already approved.

---

## 15. Commit and Push Plan

### Implementation commit

```text
echo-launch: complete FL-M3-07 immutable reports and terminal events
```

### Adjacent documentation commit

```text
echo-launch: document FL-M3-07 completion
```

The assistant must not claim either commit or push without Jesse’s CMD evidence.

---

## 16. Completion Criteria

- [ ] Public immutable `LaunchStepReport` exists.
- [ ] Public immutable `LaunchReport` exists.
- [ ] Report schema version is `1`.
- [ ] Internal single-use `LaunchReportBuilder` exists.
- [ ] Failed launches finalize one report.
- [ ] Interrupted launches finalize one report.
- [ ] `LastReport` exposes only the finalized authoritative report.
- [ ] Failed event fires after failed state and report finalization.
- [ ] Interrupted event fires after interrupted state and report finalization.
- [ ] Terminal listener failures remain isolated.
- [ ] Transition-pending success finalizes no report.
- [ ] No `LaunchCompleted` event exists.
- [ ] No `Completed` state is published.
- [ ] Step reports preserve authored order and immutable copied values.
- [ ] Accounting balances authored entries.
- [ ] Timing remains finite and nonnegative.
- [ ] Duplicate roots expose no report and publish no event.
- [ ] Destruction produces no unsafe late event.
- [ ] Authored assets remain immutable.
- [ ] Unity compiles with 0 errors and 0 compiler warnings.
- [ ] Complete Runtime Play Mode suite passes.
- [ ] Implementation commit is pushed.
- [ ] Documentation closeout is pushed.
- [ ] Working tree is clean and synchronized.

---

## 17. Stop Point

Stop after failed and interrupted root-owned launch attempts produce immutable reports and matching exactly-once public terminal events.

Do not finalize a successful report.

Do not publish `LaunchCompleted`.

Do not load or validate a destination.

Do not publish `LaunchStatus.Completed`.

Do not add public step events, presentation, splashes, direct-scene behavior, scenes, prefabs, Editor tooling, report export, or peer-package bridges.

---

## 18. Next Recommended Checkpoint

**FL-M3-08 — Initial Destination Contract, Load Result, and Completed Handoff**

Tentative combined outcome:

- Add the approved destination definition and validation boundary.
- Add an injectable standalone initial destination loader.
- Load one validated destination asynchronously.
- Finalize the successful report after destination activation.
- Publish `LaunchCompleted` exactly once.
- Advance `Transitioning -> Completed`.
- Preserve failed/interrupted reports without replacement.

This checkpoint is not authorized by FL-M3-07.

---

## 19. Handoff Record

| Field | Value |
|---|---|
| Package | First Light (`EchoLaunch`) |
| Package version | `0.1.0` |
| Specification version | `1.3.0` |
| Checkpoint | FL-M3-07 — Immutable Launch Report and Public Terminal Events |
| Starting implementation commit | `e0e9645` |
| Starting documentation commit | `d728602` |
| Starting compilation | 0 errors, 0 compiler warnings |
| Starting Runtime Play Mode | 311 passed, 0 failed, 0 ignored |
| Known blockers | None |
| Implementation outcome | Pending |
| Documentation outcome | Pending |
| Next checkpoint | FL-M3-08, tentative only |

---

## 20. Approval

**Decision:** Approve
**Approver:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Preserve immutable copied report data, finalize only failed/interrupted reports, dispatch terminal events after state and report acceptance, retain transition-pending success for destination handoff, and stop before destination or completed-launch behavior.
