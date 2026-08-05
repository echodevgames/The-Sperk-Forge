# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 5, 2026
**Current focus:** First Light FL-M3-04 documentation closeout
**Current checkpoint:** FL-M3-04 — Multi-Frame Async Proof and Runner Cancellation Outcome

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current Focus

### Goal

Close FL-M3-04 after real multi-frame Unity `Awaitable` execution and structured runner cancellation passed the complete Runtime Play Mode suite and were pushed to `origin/main`.

### Starting State

- FL-M3-04 implementation is complete in commit `b51d722`.
- `main` and `origin/main` are synchronized at `b51d722`.
- The working tree was clean after the implementation push.
- Runtime Play Mode result is 265 passed, 0 failed, 0 ignored.
- Unity compiler result is 0 errors and 0 compiler warnings.
- Expected yellow runtime diagnostics remain `ELAUNCH-ROOT-001` and `ELAUNCH-EVENT-001`.
- The adjacent FL-M3-04 documentation closeout is the only active repository work.
- Every later runtime checkpoint remains locked until this documentation closeout is committed and pushed.

---

## Active Notes

### August 5, 2026 — FL-M3-04 multi-frame async and structured cancellation

- `[TEST]` A production-shaped executor used `Awaitable.NextFrameAsync` across multiple Unity frames.
- `[TEST]` Multi-frame progress, positive elapsed timing, and authored traversal order passed.
- `[TEST]` Caller cancellation reached the linked executor token and waited for executor settlement.
- `[TEST]` Caller cancellation returned an immutable `StartupStepStatus.Cancelled` result with `ELAUNCH-STEP-005`.
- `[TEST]` `StartupSequenceRunResult.WasCancelled` reported the cancellation outcome.
- `[TEST]` Cancellation stopped traversal before any later executor factory was called.
- `[TEST]` Authored configuration, sequence, entry, policy, and definition data remained unchanged.
- `[TEST]` The first complete run reported 264 passed and 1 failed, exposing a same-tick cancellation race.
- `[FIX]` `StartupStepTimeoutMonitor` now recognizes an executor `OperationCanceledException` as caller cancellation when the caller token is already requested.
- `[TEST]` After the bounded race fix, all 265 Runtime Play Mode tests passed with 0 failed and 0 ignored.
- `[TEST]` Unity compiled the checkpoint with 0 errors and 0 compiler warnings.
- `[DECISION]` Caller cancellation is runner-owned and cannot be downgraded by authored `ContinueWithWarning` policy.
- `[DECISION]` The runner still never abandons an active executor.
- `[DECISION]` `ELAUNCH-STEP-005` is the stable structured diagnostic for caller-cancelled startup-sequence execution.
- `[HANDOFF]` Implementation commit `b51d722` is synchronized on `main` and `origin/main`.

**Promoted to:** package checkpoint, package test report, package architecture, package changelog, package README, package documentation index, and root implementation completion record.

---

## Promotion Queue

| Entry | Destination | State |
|---|---|---|
| Multi-frame Unity `Awaitable` proof | Package checkpoint, architecture, README, and test report | Promoted |
| Structured caller-cancellation outcome | Package checkpoint, architecture, changelog, and test report | Promoted |
| Stable `ELAUNCH-STEP-005` | Architecture, changelog, checkpoint, and test report | Promoted |
| `StartupSequenceRunResult.WasCancelled` | Architecture and checkpoint | Promoted |
| Same-tick cancellation race correction | Test report and changelog | Promoted |
| 265-test evidence | Package test report and root completion record | Promoted |
| FL-M3-04 documentation closeout commit | Git history | Pending |

---

## Latest Validation Snapshot

| Area | Result |
|---|---|
| FL-M3-04 implementation | Closed at `b51d722` |
| Unity compilation | 0 errors, 0 compiler warnings |
| Runtime Play Mode | 265 passed, 0 failed, 0 ignored |
| Multi-frame async tests | 2 passed |
| Updated timeout/cancellation fixture | 18 passed |
| Expected runtime diagnostics | `ELAUNCH-ROOT-001`, `ELAUNCH-EVENT-001` |
| Repository synchronization | `main` equals `origin/main` |
| Working tree after implementation push | Clean |
| Documentation closeout | Pending adjacent commit |
| Later runtime work | Locked |

---

## Checkpoint Closeout Checklist

- [x] Confirm implementation commit `b51d722`.
- [x] Record the multi-frame Unity `Awaitable` proof.
- [x] Record structured caller cancellation and `ELAUNCH-STEP-005`.
- [x] Record `StartupSequenceRunResult.WasCancelled`.
- [x] Record the same-tick cancellation race and bounded fix.
- [x] Record 265 passed, 0 failed, 0 ignored.
- [x] Reconcile package and suite Current Notes.
- [x] Update architecture, changelog, README, and documentation index.
- [x] Create package checkpoint, package test report, and root completion record.
- [ ] Review the staged documentation diff.
- [ ] Commit and push the adjacent FL-M3-04 documentation closeout.
- [ ] Confirm clean synchronized repository.
- [ ] Open the next approved First Light checkpoint.

---

## Handoff Snapshot

**Completed implementation checkpoint:** FL-M3-04 — Multi-Frame Async Proof and Runner Cancellation Outcome
**Implementation commit:** `b51d722`
**Previous documentation commit:** `a40789c`
**Runtime Play Mode:** 265 passed, 0 failed, 0 ignored
**Compilation:** 0 errors, 0 compiler warnings
**Active work:** Adjacent FL-M3-04 documentation closeout
**Known blockers:** None
**Next action:** Apply, review, commit, and push the FL-M3-04 documentation set
**Tentative later checkpoint:** FL-M3-05 — Runner Re-entry Protection and Sequence Preflight Boundary
