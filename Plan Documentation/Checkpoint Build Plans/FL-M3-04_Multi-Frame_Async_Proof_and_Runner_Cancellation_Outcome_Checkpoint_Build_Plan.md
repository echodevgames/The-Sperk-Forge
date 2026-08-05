# FL-M3-04 — Multi-Frame Async Proof and Runner Cancellation Outcome

## 1. Document Control

| Field | Value |
|---|---|
| Document ID | `FL-M3-04` |
| Version | `1.0.0` |
| Status | Approved for execution / In Development |
| Package | First Light — Startup and Launch (`EchoLaunch`) |
| Package version | `0.1.0` |
| Package specification | `SFGSS-PKG-ECHOLAUNCH-001` v`1.3.0` |
| Milestone | M3 — Startup Sequence |
| Unity baseline | `6000.3.8f1` |
| Repository | `EchoDevGames/The-Sperk-Forge` |
| Starting branch | `main` |
| Starting commit | `a40789c` |
| Previous implementation commit | `92c97ae` |
| Previous documentation commit | `bae786f` |
| Owner | Jesse “Echo” Adams / EchoDevGames |
| Opened | August 5, 2026 |

### Authorities

1. SFGSS-000 v0.23.0
2. First Light Package Specification v1.3.0
3. SFGSS-005 v1.4.0
4. `Plan Documentation/ChatGPT_Handoff.md` v1.0.0
5. First Light Developer Architecture through FL-M3-03
6. FL-M3-03 checkpoint and test report
7. Root and package Current Notes
8. Live GitHub `main` history and source at `a40789c`

### Approval

**Decision:** Approve
**Approver:** Jesse “Echo” Adams
**Approval basis:** Jesse authorized continuation from the live Git repository and designated FL-M3-04 as the next action. Normal bounded checkpoint work is pre-approved by the handoff protocol.

---

## 2. Purpose and Observable Outcome

FL-M3-04 closes two deliberately open proof gaps from FL-M3-03:

1. A real Unity executor must remain active across multiple rendered frames, publish accepted progress during those frames, settle through `UnityEngine.Awaitable`, and preserve authored traversal order.
2. Caller cancellation must stop traversal through an immutable structured runner outcome instead of escaping as an unstructured `OperationCanceledException`.

When complete, a tester can observe that:

- A production-shaped custom executor spans at least three Unity frames.
- The runner waits for that executor rather than treating it as an immediate fixture.
- Multi-frame progress is retained while the attempt is active.
- Monotonic timing records a positive elapsed duration.
- Caller cancellation reaches the linked executor token.
- The active executor settles before the runner returns.
- The attempted execution completes with `StartupStepStatus.Cancelled`.
- The run result exposes `WasCancelled == true`.
- The cancellation uses stable diagnostic code `ELAUNCH-STEP-005`.
- Authored failure policy cannot downgrade caller cancellation into a warning or continue traversal.
- No later executor factory is called after cancellation.
- Existing timeout authority and all retained runtime behavior remain unchanged.

---

## 3. Starting Conditions

- Repository `main` points to `a40789c`.
- `a40789c` follows:
  - `bae786f` — FL-M3-03 documentation closeout
  - `92c97ae` — FL-M3-03 implementation
- Working tree is expected to be clean before applying this checkpoint.
- Unity compilation baseline: 0 errors, 0 warnings.
- Runtime Play Mode baseline: 263 passed, 0 failed, 0 ignored.
- First Light package version remains `0.1.0`.
- FL-M3-03 timing, timeout, policy, settlement, and progress-containment behavior is retained.
- No architecture blocker or peer-package dependency is present.

---

## 4. Authority and Constraints

### Package boundary

EchoLaunch owns ordered startup execution, step timing, startup cancellation handling, and structured startup results. It does not own:

- Root-level shutdown orchestration
- Scene loading
- Splash presentation
- Save access
- Audio
- General UI
- Gameplay state
- Retry UI
- Arbitrary service location

### Independence

This checkpoint must:

- Use only declared Unity/runtime dependencies.
- Add no peer-package reference.
- Add no project-specific runtime reference.
- Require no scene, prefab, Resources path, tag, layer, or build-setting change.
- Keep ScriptableObject definitions immutable.
- Keep active cancellation and timing facts in runtime-owned objects.
- Preserve the isolated Play Mode proof surface.

### API and compatibility

- No public API is changed.
- No serialized field is added, removed, renamed, or reordered.
- No ScriptableObject schema version changes.
- No dependency or assembly-definition changes.
- New diagnostic code `ELAUNCH-STEP-005` becomes stable once checkpoint evidence passes.
- Runtime changes remain internal to the EchoLaunch assembly.

---

## 5. Authorized Scope

### Runtime outcome changes

- Extend `StartupStepAwaitOutcome` with immutable caller-cancellation observation.
- Change `StartupStepTimeoutMonitor` so caller cancellation is returned after executor settlement rather than thrown.
- Convert caller cancellation inside `StartupSequenceRunner` to a terminal `StartupStepResult.Cancelled`.
- Add stable caller-cancellation diagnostic `ELAUNCH-STEP-005`.
- Add `WasCancelled` to `StartupSequenceRunResult`.
- Preserve timeout-first and completion-first race behavior already proven by FL-M3-03.
- Preserve non-caller executor cancellation behavior unless the executor returns an explicit cancelled result.

### Automated proof

- Replace the retained exception-based caller-cancellation assertion with structured-outcome assertions.
- Add a production-shaped multi-frame test executor using `Awaitable.NextFrameAsync`.
- Prove frame spanning, progress, timing, order, cancellation settlement, later-factory suppression, immutable authored assets, and retained totals.

### Documentation

- Create this Checkpoint Build Plan now.
- Generate checkpoint, test-report, architecture, Current Notes, README, index, and changelog closeout as one adjacent documentation bundle after implementation evidence exists.

---

## 6. Explicit Exclusions

FL-M3-04 does not authorize:

- Automatic retry
- Retry count, delay, or backoff
- Interactive retry, skip, or cancel UI
- Root-level `CancelLaunch`
- Shutdown or `OnDestroy` cancellation orchestration
- `EchoLaunchRoot` runner integration
- Automatic startup from Unity callbacks
- `LaunchSession` lifecycle advancement
- Public step lifecycle events
- Public launch reports
- Preflight or duplicate-ID validation
- Dependency validation
- Runner re-entry protection
- Splash presentation
- Destination loading
- Persistent-root lifetime changes
- Direct-scene initializer behavior
- Editor windows, custom inspectors, or setup tools
- Standalone Laboratory scene construction
- Peer-package bridges
- Player builds or performance claims
- Public API, serialization, assembly, or dependency changes

---

## 7. Files and Assets

### Phase 1 — Runtime cancellation outcome

| Path | Action | Ownership | Purpose |
|---|---|---|---|
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupStepAwaitOutcome.cs` | Modify | Runtime execution | Carry immutable caller-cancellation observation after settlement |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupStepTimeoutMonitor.cs` | Modify | Runtime execution | Return caller cancellation as an await outcome after consuming the executor |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequenceRunner.cs` | Modify | Runtime execution | Complete the attempt with `Cancelled`, stop traversal, and emit `ELAUNCH-STEP-005` |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequenceRunResult.cs` | Modify | Runtime execution | Expose whether the run captured cancellation |
| `Plan Documentation/Checkpoint Build Plans/FL-M3-04_Multi-Frame_Async_Proof_and_Runner_Cancellation_Outcome_Checkpoint_Build_Plan.md` | Create | Planning | Authorize and bound FL-M3-04 |

### Phase 2 — Automated proof

| Path | Action | Ownership | Purpose |
|---|---|---|---|
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/StartupSequenceRunnerTimeoutTests.cs` | Modify | Runtime Play Mode tests | Replace escaping-cancellation expectation with structured outcome and settlement assertions |
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/StartupSequenceRunnerMultiFrameAsyncTests.cs` | Create | Runtime Play Mode tests | Prove real multi-frame execution, progress, timing, traversal, and cancellation |
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/StartupSequenceRunnerMultiFrameAsyncTests.cs.meta` | Create by Unity | Unity asset identity | Preserve the new test source GUID |

### Phase 3 — Adjacent closeout documentation

Generated only after actual compilation, test totals, and Git evidence exist:

- Package checkpoint completion record
- Package FL-M3-04 test report
- First Light Developer Architecture
- First Light Developer Current Notes
- First Light documentation index
- First Light README
- First Light CHANGELOG
- Root Current Notes
- Root completion/status record when applicable

No other file is authorized.

---

## 8. Implementation Sequence

### Phase 1 — Runtime model and conversion

1. Extend `StartupStepAwaitOutcome`.
2. Update `StartupStepTimeoutMonitor`.
3. Update `StartupSequenceRunner`.
4. Update `StartupSequenceRunResult`.
5. Return to Unity and wait for compilation.
6. Stop on the first error or warning.
7. Do not run the retained test suite yet because the old cancellation assertion intentionally describes the previous behavior.

### Phase 2 — Tests

1. Update the retained caller-cancellation test.
2. Add the new multi-frame Play Mode fixture.
3. Let Unity create the new `.meta` file.
4. Compile with 0 errors and 0 warnings.
5. Run the FL-M3-04-focused tests.
6. Run the complete Runtime Play Mode suite.
7. Diagnose the first failure only.
8. Re-run the full suite after every correction.

### Phase 3 — Git scope

1. Run `git status --short`.
2. Confirm only authorized files and the required `.meta` file changed.
3. Run `git diff --check`.
4. Review the staged stat and diff.
5. Commit implementation and tests.
6. Push and verify synchronization.

### Phase 4 — Documentation closeout

1. Reconcile root and package Current Notes.
2. Update architecture and checkpoint/test records.
3. Update README, index, and changelog only where the validated behavior changes their truth.
4. Commit and push the adjacent documentation set.
5. Confirm clean synchronized `main`.

---

## 9. Visible Code and Learning Rule

Each changed runtime file is delivered completely and explained before application.

Learning order:

1. What fact the file owns.
2. Why the prior FL-M3-03 behavior is insufficient.
3. The complete replacement file.
4. Cancellation and settlement flow.
5. Why authored policy does not convert caller cancellation.
6. Expected compiler result.
7. Focused and retained proof.

No runtime change is hidden inside the documentation apply script.

---

## 10. Unity Editor Setup

No scene or Inspector setup is required.

### Phase 1

- Apply the four runtime replacements.
- Open Unity `6000.3.8f1`.
- Allow script compilation to finish.
- Record exact errors and warnings.
- Do not enter Play Mode before the test phase is installed.

### Phase 2

- Open `Window > General > Test Runner`.
- Select PlayMode.
- Run the FL-M3-04-focused fixtures.
- Then run the complete Runtime Play Mode suite.

Expected new source asset:

`Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/StartupSequenceRunnerMultiFrameAsyncTests.cs`

Unity must generate and retain its matching `.meta` file.

---

## 11. Validation and Tests

| ID | Setup | Action | Expected result | Type |
|---|---|---|---|---|
| FL-M3-04-T01 | Multi-frame executor | Run with default Unity clock | Executor remains active across at least three frames | Automated Play Mode |
| FL-M3-04-T02 | Multi-frame executor reports progress | Await traversal | Accepted progress advances while attempt is active | Automated Play Mode |
| FL-M3-04-T03 | Multi-frame executor | Inspect timing | Positive unscaled elapsed duration; no timeout | Automated Play Mode |
| FL-M3-04-T04 | Multi-frame first step plus immediate second step | Run sequence | Second factory and executor run only after first settles | Automated Play Mode |
| FL-M3-04-T05 | Active cancellable executor | Cancel caller token | Runner returns normally with a cancelled execution | Automated Play Mode |
| FL-M3-04-T06 | Caller cancellation | Inspect result | Status `Cancelled`; code `ELAUNCH-STEP-005`; `WasCancelled == true` | Automated Play Mode |
| FL-M3-04-T07 | Optional warning policy | Cancel caller token | Cancellation remains cancelled and stops; policy does not convert it | Automated Play Mode |
| FL-M3-04-T08 | Executor observes cancellation | Cancel caller token | Executor settles before runner returns | Automated Play Mode |
| FL-M3-04-T09 | Later authored step | Cancel first step | Later factory and executor are not called; later entry is unvisited | Automated Play Mode |
| FL-M3-04-T10 | Progress after cancellation | Report after gate closure | Late progress is ignored | Automated Play Mode |
| FL-M3-04-T11 | Project-owned assets | Run success and cancellation cases | Definitions, sequence, entries, policy, and configuration remain unchanged | Automated Play Mode |
| FL-M3-04-T12 | Full retained suite | Run Runtime Play Mode | All retained and new tests pass with zero failures and ignored tests | Automated Play Mode |
| FL-M3-04-T13 | Unity compiler | Import all checkpoint files | 0 errors, 0 warnings | Compiler |
| FL-M3-04-T14 | Git scope | Review status/diff | Authorized files only; no whitespace errors | Git |

No numerical final test total is predicted. Actual Unity output is the only accepted result.

---

## 12. Failure Symptoms and Fixes

| Symptom | Likely owner | Bounded response |
|---|---|---|
| `OperationCanceledException` still escapes caller cancellation | Runner/monitor conversion | Inspect `CallerCancellationObserved` path before timeout/exception conversion |
| Cancellation becomes `Warning` | Policy path incorrectly applied | Bypass authored failure conversion for caller cancellation |
| Later factory runs after cancellation | Stopping index or loop break missing | Complete execution, append it, set stopping index, then break |
| Run returns before executor settles | Monitor exits on token alone | Continue ticking with a non-cancelled monitor token until executor awaiter settles |
| Timeout test changes unexpectedly | Cancellation precedence changed | Preserve completion and timeout race rules; caller cancellation is a separate post-settlement outcome |
| Elapsed timing is zero in the multi-frame proof | Default Unity clock not used or frame proof completed immediately | Use `UnityLaunchClock.Shared` and real `Awaitable.NextFrameAsync` frames |
| Existing exception test fails | Expected during Phase 1 only | Install the Phase 2 test update before running the full suite |
| New `.meta` missing | Unity has not imported new test file | Return to Unity and allow import; do not fabricate GUID by hand |
| Warning introduced by async test helper | Helper completes synchronously or unused async | Use genuine frame awaits; do not suppress a real production-shaped proof warning |

---

## 13. Rollback and Recovery

Before application:

```cmd
git status --short
git rev-parse --short HEAD
```

Expected baseline:

```text
a40789c
```

To discard only uncommitted FL-M3-04 runtime replacements before any other work:

```cmd
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupStepAwaitOutcome.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupStepTimeoutMonitor.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequenceRunner.cs"
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequenceRunResult.cs"
git clean -f -- "Plan Documentation/Checkpoint Build Plans/FL-M3-04_Multi-Frame_Async_Proof_and_Runner_Cancellation_Outcome_Checkpoint_Build_Plan.md"
```

After Phase 2, restore only the authorized test files or delete the new test and its Unity-generated `.meta` file.

Do not delete or rewrite project-owned ScriptableObject assets. This checkpoint does not modify them.

---

## 14. Documentation Reconciliation

At closeout:

- Record `ELAUNCH-STEP-005`.
- Replace “caller cancellation escapes after settlement” with “caller cancellation returns a structured cancelled run outcome after settlement.”
- Add `WasCancelled` to the internal run-result architecture.
- Record the production-shaped multi-frame proof and actual frame/timing evidence.
- Move completed exclusions out of the package Current Notes “Not Run” list.
- Retain root integration, root cancellation commands, retries, reports, preflight, presentation, scene loading, and re-entry protection as not implemented.
- Record actual compiler and test totals only.
- Record actual implementation and documentation commits only after Git evidence.

---

## 15. Commit and Push Plan

Suggested implementation commit:

```text
echo-launch: complete FL-M3-04 async proof and cancellation outcome
```

Suggested adjacent documentation commit:

```text
echo-launch: document FL-M3-04 async and cancellation proof
```

Required pre-commit evidence:

```cmd
git status --short
git diff --check
git diff --stat
```

Required post-push evidence:

```cmd
git status
git log -3 --oneline
```

No script in this checkpoint stages, commits, or pushes.

---

## 16. Completion Criteria

- [ ] All four runtime files match the approved FL-M3-04 implementation.
- [ ] Caller cancellation no longer escapes the monitor as the normal runner outcome.
- [ ] Active cancellation completes the attempted execution with `Cancelled`.
- [ ] Stable diagnostic `ELAUNCH-STEP-005` is verified.
- [ ] `StartupSequenceRunResult.WasCancelled` is verified.
- [ ] Cancellation stops regardless of authored warning policy.
- [ ] The active executor settles before runner return.
- [ ] No later factory is called after cancellation.
- [ ] Real multi-frame execution spans at least three Unity frames.
- [ ] Multi-frame progress and timing are verified.
- [ ] Authored assets remain immutable.
- [ ] Compilation is 0 errors, 0 warnings.
- [ ] Complete Runtime Play Mode suite passes.
- [ ] Git scope contains authorized files only.
- [ ] Implementation commit and push are evidenced.
- [ ] Documentation closeout is committed and pushed.
- [ ] Repository is clean and synchronized.

---

## 17. Stop Point

Stop immediately after FL-M3-04 implementation, tests, Git evidence, and adjacent documentation closeout.

Do not connect the runner to `EchoLaunchRoot`, Unity lifecycle callbacks, launch-session state, reports, presentation, scene loading, or direct-scene behavior.

---

## 18. Next Recommended Checkpoint

**Tentative only, not authorized:**
`FL-M3-05 — Runner Re-entry Protection and Sequence Preflight Boundary`

The exact FL-M3-05 title and scope must be selected from the remaining package architecture after FL-M3-04 evidence is reconciled.

---

## 19. Handoff Record

| Field | Value |
|---|---|
| Package | First Light (`EchoLaunch`) |
| Package version | `0.1.0` |
| Specification | v`1.3.0` |
| Checkpoint | `FL-M3-04 — Multi-Frame Async Proof and Runner Cancellation Outcome` |
| Starting commit | `a40789c` |
| Starting compiler evidence | 0 errors, 0 warnings |
| Starting automated evidence | 263 passed, 0 failed, 0 ignored |
| Current phase | Phase 1 — Runtime cancellation outcome |
| Known blockers | None |
| Runtime files authorized | 4 modified |
| Test files authorized | 1 modified, 1 new, 1 Unity-generated `.meta` |
| Root integration | Not authorized |
| Next immediate evidence | Unity compilation after Phase 1 |
