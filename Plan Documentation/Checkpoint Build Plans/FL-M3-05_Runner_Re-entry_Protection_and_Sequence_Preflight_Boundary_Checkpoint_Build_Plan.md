# FL-M3-05 — Runner Re-entry Protection and Sequence Preflight Boundary

**Document ID:** FL-M3-05
**Version:** 1.0.0
**Status:** Active and authorized
**Package:** First Light (`EchoLaunch`)
**Package version:** `0.1.0`
**Package specification:** SFGSS-PKG-ECHOLAUNCH-001 v1.3.0
**Milestone:** M3 — Startup Sequence Runtime
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Repository/workspace:** The Sperk’s Forge
**Unity baseline:** Unity 6000.3.8f1
**Implementation baseline:** `ce2e23b`
**Previous implementation commit:** `b51d722`
**Previous documentation commit:** `ce2e23b`
**Workflow authority:** SFGSS-005 v1.4.0
**Authorized:** August 5, 2026

> Before the runner lights the first furnace, it checks the blueprint and locks the door against a second foreman.

---

## 1. Purpose and Observable Outcome

FL-M3-05 creates one complete **startup-sequence execution gate**.

When the checkpoint is complete:

1. The runner performs a complete, side-effect-free preflight before creating any executor.
2. Invalid configuration, sequence, entry, definition, policy, identity, schema, and duplicate-ID data is rejected before an executor factory is called.
3. One `StartupSequenceRunner` instance permits only one active run at a time.
4. A concurrent second run is rejected before preflight traversal or executor creation.
5. The runner becomes reusable after the active run settles successfully, fails, cancels, or throws during preflight.
6. Existing immediate, policy, timeout, and multi-frame behavior remains unchanged.
7. Authored ScriptableObject data remains immutable.

The observable proof is a Runtime Play Mode fixture showing that malformed authored data and concurrent re-entry cannot cross the runner’s side-effect boundary.

---

## 2. Starting Conditions

- `main` and `origin/main` are synchronized at `ce2e23b`.
- Working tree is clean.
- FL-M3-04 implementation is complete in `b51d722`.
- FL-M3-04 documentation is complete in `ce2e23b`.
- Unity compilation reports 0 errors and 0 compiler warnings.
- Runtime Play Mode reports 265 passed, 0 failed, 0 ignored.
- `StartupSequenceRunner` is still invoked explicitly and is not connected to `EchoLaunchRoot`.
- No launch report, destination loader, presenter, splash sequence, or automatic startup behavior exists.
- No unresolved architecture decision blocks the execution gate.

If any starting condition is false, stop and reconcile the repository before applying this checkpoint.

---

## 3. Authority and Constraints

First Light owns initial launch authority, ordered startup execution, startup-only presentation, structured reporting, and final startup handoff.

This checkpoint is limited to the runner’s **pre-execution safety boundary**.

The checkpoint must preserve:

- Package independence.
- No peer Sperk’s Forge runtime dependency.
- No project-assembly references.
- Immutable authored ScriptableObjects.
- Fresh single-use executors.
- Cooperative cancellation and executor settlement.
- Existing timeout and caller-cancellation behavior.
- Existing empty-sequence behavior until a later explicit empty-sequence policy exists.
- Disabled entries producing no executor side effects.
- Sequential reuse of one runner after a prior run reaches a terminal boundary.

The checkpoint must not silently establish root lifecycle, public reporting, scene policy, presentation, or Editor repair behavior.

---

## 4. Scope

### 4.1 Sequence preflight

Add one internal preflight authority that validates the complete authored sequence before executor creation.

Validation covers:

- Defined active `LaunchMode`.
- Non-null `EchoLaunchConfiguration`.
- Canonical configuration identity.
- Supported configuration schema.
- Non-null startup sequence.
- Canonical sequence identity.
- Supported sequence schema.
- Non-null entry collection elements.
- Canonical entry identities.
- Defined entry activation values.
- Unique entry identities.
- Enabled entry step-definition presence.
- Canonical referenced step identities.
- Supported referenced step schemas.
- Unique referenced step identities.
- Referenced definitions are validated globally; enabled policy values are checked by the runner immediately before factory creation so existing structured invalid-policy behavior is preserved.

### 4.2 Preserved compatibility rules

- An empty sequence remains a valid empty traversal in this checkpoint.
- A disabled entry may remain without a step definition because it cannot create an executor or perform a startup side effect.
- Existing exception types for null configuration, invalid launch mode, missing sequence, null entry, and enabled missing definition remain compatible where practical.
- Preflight does not repair, clamp, regenerate, or rewrite authored data.
- Preflight does not call `CreateExecutor()`.

### 4.3 Pre-start policy boundary

After global authored-data preflight and before `CreateExecutor()`, the runner validates the enabled entry policy. Invalid policy produces the retained structured `ELAUNCH-STEP-004` blocking result without creating an executor.

### 4.4 Runner re-entry protection

Add one runner-instance active-run gate.

The gate must:

- Acquire before preflight begins.
- Reject a second concurrent `RunAsync` call immediately.
- Reject re-entry before configuration traversal or executor creation.
- Release through `finally`.
- Release after success.
- Release after structured caller cancellation.
- Release after timeout or blocking traversal.
- Release after preflight rejection.
- Release after an unexpected exception.
- Permit a later sequential run after release.

### 4.4 Diagnostic vocabulary

Use a stable runner re-entry code:

```text
ELAUNCH-RUN-001
```

The diagnostic appears in the rejection exception message for searchability. A public report representation remains later work.

Preflight exceptions use approved existing diagnostic vocabulary where suitable:

- `ELAUNCH-CFG-001`
- `ELAUNCH-SEQ-001`
- `ELAUNCH-STEP-001`
- `ELAUNCH-STEP-002`

A new duplicate-entry diagnostic may be introduced only if the implementation cannot accurately express the failure with existing sequence vocabulary. Any new stable code must be recorded in the checkpoint and closeout documentation.

---

## 5. Explicit Exclusions

FL-M3-05 does not authorize:

- `EchoLaunchRoot` runner integration.
- Automatic execution from `Awake`, `Start`, scene callbacks, or application callbacks.
- Root-level `CancelLaunch`.
- Launch-session state advancement.
- Public step-started or step-completed events.
- `LaunchReport` or `LaunchReportBuilder`.
- Public preflight result models.
- Presenter or status-view integration.
- Splash definitions or playback.
- Destination validation or scene loading.
- Direct-scene initialization.
- Dependency graph validation.
- Automatic retry or retry metadata expansion.
- Interactive retry, cancel, or skip UI.
- Editor setup, repair, validation windows, or asset migration.
- Test Lab scenes or prefabs.
- Package version change.
- Public API or serialized schema change.
- Peer-package bridge work.

---

## 6. Files and Assets

| Path | Action | Ownership | Purpose |
|---|---|---|---|
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflight.cs` | Create | Runtime internal | Complete side-effect-free configuration and sequence validation |
| `Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequenceRunner.cs` | Modify | Runtime internal | Acquire/release re-entry gate and invoke preflight before traversal |
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/StartupSequenceRunnerPreflightAndReentryTests.cs` | Create | Runtime tests | Prove preflight ordering, duplicate detection, re-entry rejection, and runner reuse |
| `Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/StartupSequenceRunnerPreflightAndReentryTests.cs.meta` | Create by Unity | Unity asset identity | Preserve stable Unity test-script GUID |
| `Plan Documentation/Checkpoint Build Plans/FL-M3-05_Runner_Re-entry_Protection_and_Sequence_Preflight_Boundary_Checkpoint_Build_Plan.md` | Create | Planning | Authoritative bounded implementation plan |

No other file is authorized during the implementation commit unless compilation exposes a directly checkpoint-owned defect.

---

## 7. Implementation Sequence

### Phase 1 — Preflight authority

1. Create `StartupSequencePreflight`.
2. Keep it internal and stateless.
3. Validate the launch mode, configuration, sequence, entries, definitions, policies, schemas, and identities.
4. Use hash sets for duplicate entry and step identity detection.
5. Perform no executor creation and no asset mutation.
6. Preserve empty-sequence and disabled-entry compatibility.
7. Leave enabled invalid-policy conversion to the runner's pre-start structured-result boundary.

### Phase 2 — Runner gate

1. Add an integer active-run state to `StartupSequenceRunner`.
2. Acquire with `Interlocked.CompareExchange`.
3. Reject concurrent re-entry with `ELAUNCH-RUN-001`.
4. Wrap the complete run body in `try/finally`.
5. Invoke preflight after the gate is acquired and before traversal.
6. Convert enabled invalid policy to the retained structured blocking result before factory creation.
7. Release the gate with `Volatile.Write` or equivalent in `finally`.
8. Preserve all FL-M3-04 timeout and cancellation behavior.

### Phase 3 — Automated proof

1. Add a self-contained Runtime Play Mode fixture.
2. Corrupt authored values only inside tests through bounded reflection helpers.
3. Verify each preflight rejection happens before any factory call.
4. Verify duplicate entry and step identities are detected.
5. Verify disabled null-definition compatibility.
6. Verify empty-sequence compatibility.
7. Hold one multi-frame run active and attempt concurrent re-entry.
8. Verify the second call rejects before factory creation.
9. Settle the first run and verify later sequential reuse.
10. Verify gate release after preflight rejection and cancellation.
11. Verify authored data remains unchanged.

---

## 8. Visible Code and Learning Rule

Before applying the implementation:

- Show or provide the complete contents of every created or replaced C# file.
- State the exact repository path.
- Explain responsibility and authority boundary.
- Explain the preflight ordering.
- Explain why the gate uses atomic acquisition rather than a plain Boolean.
- Explain the `finally` release guarantee.
- Explain why empty sequence and disabled null definitions remain compatible.
- Explain why public report conversion remains excluded.
- Provide one apply bundle with a CMD-first script.
- Stop first at the Unity compile gate.
- Run the complete Runtime Play Mode suite only after clean compilation.

---

## 9. Unity Editor Setup

No scene, prefab, component, Build Settings, Inspector, or project-setting change is required.

Unity will:

- Recompile the runtime and test assemblies.
- Generate the new test file’s `.meta` file.
- Discover the new Runtime Play Mode fixture.

Expected Inspector-visible changes: none.

---

## 10. Validation and Tests

### 10.1 Compile gate

| Test | Expected result |
|---|---|
| Unity script compilation | 0 errors |
| Unity compiler warnings | 0 |
| Runtime assembly boundary | No `UnityEditor` reference |
| Package dependencies | Unchanged |

### 10.2 Preflight tests

| ID | Action | Expected result |
|---|---|---|
| FL-M3-05-T01 | Run with undefined/unknown launch mode | Reject before executor factory |
| FL-M3-05-T02 | Run with null configuration | Reject before traversal |
| FL-M3-05-T03 | Corrupt configuration identity | Reject before factory |
| FL-M3-05-T04 | Corrupt configuration schema | Reject before factory |
| FL-M3-05-T05 | Remove startup sequence | Reject before factory |
| FL-M3-05-T06 | Corrupt sequence identity | Reject before factory |
| FL-M3-05-T07 | Corrupt sequence schema | Reject before factory |
| FL-M3-05-T08 | Insert null entry | Reject before any factory |
| FL-M3-05-T09 | Corrupt entry identity | Reject before any factory |
| FL-M3-05-T10 | Corrupt activation enum | Reject before any factory |
| FL-M3-05-T11 | Duplicate entry identity | Reject before any factory |
| FL-M3-05-T12 | Enabled entry lacks definition | Reject before any factory |
| FL-M3-05-T13 | Corrupt definition identity | Reject before any factory |
| FL-M3-05-T14 | Corrupt definition schema | Reject before any factory |
| FL-M3-05-T15 | Duplicate step identity | Reject before any factory |
| FL-M3-05-T16 | Corrupt enabled policy | Structured blocking result before any factory |
| FL-M3-05-T17 | Disabled entry lacks definition | Valid traversal; no factory |
| FL-M3-05-T18 | Empty sequence | Valid empty traversal |
| FL-M3-05-T19 | Preflight run | Authored assets unchanged |

### 10.3 Re-entry tests

| ID | Action | Expected result |
|---|---|---|
| FL-M3-05-T20 | Start a multi-frame run, then call same runner again | Second call rejects with `ELAUNCH-RUN-001` |
| FL-M3-05-T21 | Observe second call | No second executor factory |
| FL-M3-05-T22 | Settle first run, then run again | Sequential reuse succeeds |
| FL-M3-05-T23 | Trigger preflight rejection, then run valid configuration | Gate was released |
| FL-M3-05-T24 | Cancel active run, then run again | Gate was released after settlement |
| FL-M3-05-T25 | Complete blocking/timeout path, then run again | Gate was released |

### 10.4 Full regression gate

Expected retained baseline before additions:

```text
265 passed
0 failed
0 ignored
```

The new fixture contains 23 test cases. The expected complete Runtime Play Mode total is:

```text
288 passed
0 failed
0 ignored
```

The checkpoint closeout records the observed total and does not claim success until Unity evidence is supplied.

---

## 11. Failure Symptoms and Fixes

### Re-entry test hangs

Cause: active executor was not deterministically settled.

Fix: use one controlled completion source and always settle it in `finally` inside the test.

### Existing empty-sequence test fails

Cause: preflight incorrectly introduced an unapproved empty-sequence policy.

Fix: restore empty sequence as valid for FL-M3-05.

### Disabled-entry test fails

Cause: preflight requires a definition for disabled entries.

Fix: validate definition presence only for enabled entries while still validating any non-null referenced definition.

### Runner remains busy after failure

Cause: gate release is not in a complete `finally` boundary.

Fix: place the whole post-acquisition body inside `try/finally`.

### Existing timeout or cancellation tests fail

Cause: runner integration altered FL-M3-04 monitoring behavior.

Fix: restore the timeout/cancellation body unchanged and limit edits to preflight invocation and outer gate ownership.

### Duplicate-ID test calls a factory

Cause: preflight occurs inside traversal or after the first factory.

Fix: complete all preflight validation before the execution loop begins.

---

## 12. Rollback and Recovery

Before commit:

```cmd
git restore --staged .
git restore -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequenceRunner.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Runtime/Execution/StartupSequencePreflight.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/StartupSequenceRunnerPreflightAndReentryTests.cs"
git clean -f -- "Packages/com.echodevgames.echo-launch/Tests/Runtime/PlayMode/StartupSequenceRunnerPreflightAndReentryTests.cs.meta"
git clean -f -- "Plan Documentation/Checkpoint Build Plans/FL-M3-05_Runner_Re-entry_Protection_and_Sequence_Preflight_Boundary_Checkpoint_Build_Plan.md"
```

After commit, use `git revert <FL-M3-05-IMPLEMENTATION-COMMIT>` rather than rewriting shared history.

No project-owned content, scenes, prefabs, configuration assets, or Build Settings are modified.

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

The package specification changes only if implementation reveals a genuine contract change. No such change is anticipated.

---

## 14. Commit and Push Plan

### Implementation commit

Suggested message:

```text
echo-launch: complete FL-M3-05 preflight and re-entry gate
```

### Adjacent documentation commit

Suggested message:

```text
echo-launch: document FL-M3-05 completion
```

The assistant must not claim either commit or push without Jesse’s CMD evidence.

---

## 15. Completion Criteria

- [ ] Preflight completes before all executor factories.
- [ ] Configuration identity and schema are validated.
- [ ] Sequence identity and schema are validated.
- [ ] Entry identity, activation, and uniqueness are validated.
- [ ] Enabled definition presence is validated.
- [ ] Definition identity, schema, and uniqueness are validated.
- [ ] Enabled invalid policy produces a structured pre-start blocker without executor creation.
- [ ] Empty sequence remains compatible.
- [ ] Disabled null-definition entry remains compatible.
- [ ] Concurrent runner re-entry is rejected.
- [ ] Re-entry rejection occurs before a second factory.
- [ ] Runner gate releases after every terminal path.
- [ ] Sequential runner reuse remains valid.
- [ ] Authored assets remain unchanged.
- [ ] Unity compiles with 0 errors and 0 compiler warnings.
- [ ] Complete Runtime Play Mode suite passes.
- [ ] Implementation commit is pushed.
- [ ] Documentation closeout is pushed.
- [ ] Working tree is clean and synchronized.

---

## 16. Stop Point

Stop after the runner has one proven pre-execution safety gate and the documentation is closed.

Do not connect the runner to `EchoLaunchRoot`.

Do not create reports, destination loading, presentation, splash playback, direct-scene behavior, setup tooling, or a Test Lab under this checkpoint.

---

## 17. Next Recommended Checkpoint

**FL-M3-06 — Root-Owned Startup Run and Lifecycle Advancement**

Tentative outcome:

- The authoritative root owns one runner invocation.
- Explicit start advances the launch session through approved lifecycle states.
- Runner progress is translated into root progress snapshots.
- Runner completion maps to terminal launch state.
- Root cancellation and destruction ownership are defined.

This is not authorized by FL-M3-05.

---

## 18. Handoff Record

| Field | Value |
|---|---|
| Package | First Light (`EchoLaunch`) |
| Package version | `0.1.0` |
| Specification version | `1.3.0` |
| Checkpoint | FL-M3-05 — Runner Re-entry Protection and Sequence Preflight Boundary |
| Starting implementation commit | `b51d722` |
| Starting documentation commit | `ce2e23b` |
| Starting compilation | 0 errors, 0 compiler warnings |
| Starting Runtime Play Mode | 265 passed, 0 failed, 0 ignored |
| Known blockers | None |
| Implementation outcome | Pending |
| Documentation outcome | Pending |
| Next checkpoint | FL-M3-06, tentative only |

---

## 19. Approval

**Decision:** Approve
**Approver:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 5, 2026
**Conditions:** Preserve package independence, complete preflight before executor creation, keep re-entry protection runner-local, preserve FL-M3-04 behavior, and stop before root integration.
