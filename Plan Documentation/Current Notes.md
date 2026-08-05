# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 5, 2026
**Current focus:** First Light FL-M4-03 documentation closeout
**Current checkpoint:** FL-M4-03 — Image Splash Definitions and Deterministic Splash Player

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current Focus

### Goal

Close FL-M4-03 after standalone project-owned image splash definitions,
deterministic playback, neutral skip requests, reduced-motion behavior, and
default uGUI projection passed complete automated proof.

### Starting State

- FL-M4-03 implementation is committed and pushed in `f997a9a`.
- FL-M4-02 documentation is complete in `cbaee24`.
- `main` and `origin/main` are synchronized at `f997a9a`.
- Working tree was clean after the implementation push.
- Runtime Play Mode result is 450 passed, 0 failed, 0 ignored.
- Unity compiler result is 0 errors and 0 compiler warnings.
- The first full test attempt hung due to a test-only zero-advance clock loop.
- A later full run completed with 448 passed and 2 fixture failures.
- Both final fixture failures were corrected without production code changes.
- Configuration remains schema 3.
- Reports remain schema 2.
- Root-owned splash integration is not implemented.
- The adjacent FL-M4-03 documentation closeout is the only active repository work.
- Later implementation remains locked until this documentation set is committed and pushed.

---

## Active Notes

### August 5, 2026 — FL-M4-03 deterministic image splashes

- `[IMPLEMENTATION]` Added `SplashSequence` schema 1.
- `[IMPLEMENTATION]` Added immutable `SplashEntry`.
- `[IMPLEMENTATION]` Added deterministic clock-driven splash player.
- `[IMPLEMENTATION]` Added minimum-display and skip-policy enforcement.
- `[IMPLEMENTATION]` Added reduced-motion fade removal.
- `[IMPLEMENTATION]` Added cancellation, re-entry, and clock containment.
- `[IMPLEMENTATION]` Added neutral and headless splash presenters.
- `[IMPLEMENTATION]` Added immutable frames and result.
- `[IMPLEMENTATION]` Added uGUI image, label, alpha, and position.
- `[IMPLEMENTATION]` Added public `RequestSplashSkip()`.
- `[DECISION]` Project input remains outside EchoLaunch.
- `[DECISION]` Configuration remains schema 3.
- `[DECISION]` Reports remain schema 2.
- `[DECISION]` Root-owned splash playback remains deferred.
- `[TEST]` All 26 Runtime splash tests passed.
- `[TEST]` All 10 uGUI splash tests passed.
- `[TEST]` All 450 Runtime Play Mode tests passed.
- `[TEST]` Unity compiled with 0 errors and 0 compiler warnings.
- `[FIX]` Removed a test-only zero-advance clock infinite loop.
- `[FIX]` Moved skip requests into active deterministic frame presentation.
- `[FIX]` Consumed a faulted Awaitable in the NUnit re-entry assertion.
- `[FIX]` Corrected the generated sequence-identity uniqueness fixture.
- `[EVIDENCE GAP]` No configuration/root integration, prefab, input binding, or visual Test Lab proof.
- `[HANDOFF]` Implementation commit `f997a9a` is synchronized on `main` and `origin/main`.

**Promoted to:** package checkpoint, package test report, architecture, changelog,
README, documentation index, specification status, root completion record, and
Current Notes.

---

## Promotion Queue

| Entry | Destination | State |
|---|---|---|
| Splash definition schema | Architecture and checkpoint | Promoted |
| Deterministic player | Architecture and checkpoint | Promoted |
| Skip/minimum timing | Architecture and test report | Promoted |
| Reduced motion | Architecture and test report | Promoted |
| Neutral/headless presenters | Architecture and README | Promoted |
| uGUI splash projection | Checkpoint and README | Promoted |
| Test hang and fixture corrections | Changelog and test report | Promoted |
| 450-test evidence | Test report and completion record | Promoted |
| Schema/root integration gap | Checkpoint and Current Notes | Promoted |
| FL-M4-03 documentation closeout commit | Git history | Pending |

---

## Latest Validation Snapshot

| Area | Result |
|---|---|
| FL-M4-03 implementation | `f997a9a` |
| Previous documentation | `cbaee24` |
| Unity compilation | 0 errors, 0 compiler warnings |
| Runtime Play Mode | 450 passed, 0 failed, 0 ignored |
| New Runtime splash fixture | 26 passed |
| New uGUI splash fixture | 10 passed |
| Configuration schema | 3, unchanged |
| Report schema | 2, unchanged |
| Root splash integration | Not implemented |
| Repository synchronization | `main` equals `origin/main` |
| Working tree after implementation push | Clean |
| Documentation closeout | Pending adjacent commit |
| Later implementation | Locked |

---

## Checkpoint Closeout Checklist

- [x] Confirm implementation commit `f997a9a`.
- [x] Record splash sequence and entry definitions.
- [x] Record deterministic timing and alpha.
- [x] Record minimum-display and skip policy.
- [x] Record reduced-motion behavior.
- [x] Record cancellation, re-entry, and clock containment.
- [x] Record neutral/headless presenters.
- [x] Record uGUI projection and public skip request.
- [x] Record test hang diagnosis and fixture corrections.
- [x] Record 450 passed, 0 failed, 0 ignored.
- [x] Record 0 compiler errors and 0 compiler warnings.
- [x] Record unchanged configuration/report schemas.
- [x] Record root-integration evidence gap.
- [x] Reconcile package and suite Current Notes.
- [x] Update architecture, specification status, changelog, README, and index.
- [x] Create package checkpoint, package test report, and root completion record.
- [ ] Review the staged documentation diff.
- [ ] Commit and push the adjacent FL-M4-03 documentation closeout.
- [ ] Confirm clean synchronized repository.
- [ ] Open the next authority-first First Light checkpoint.

---

## Handoff Snapshot

**Completed implementation checkpoint:** FL-M4-03 — Image Splash Definitions and Deterministic Splash Player
**Implementation commit:** `f997a9a`
**Previous documentation commit:** `cbaee24`
**Runtime Play Mode:** 450 passed, 0 failed, 0 ignored
**Compilation:** 0 errors, 0 compiler warnings
**Active work:** Adjacent FL-M4-03 documentation closeout
**Known blockers:** None
**Evidence gap:** Configuration/root integration, prefab, input binding, and visual Test Lab proof not run
**Next action:** Apply, review, commit, and push the FL-M4-03 documentation set
**Tentative later checkpoint:** FL-M4-04 — Splash Configuration Schema and Root Playback Integration
