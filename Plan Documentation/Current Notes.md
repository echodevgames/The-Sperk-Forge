
# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 5, 2026
**Current focus:** First Light FL-M4-04 authority promotion
**Current checkpoint:** FL-M4-04 — Splash Configuration Schema and Root Playback Integration

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current Focus

### Goal

Commit the authority required for configuration schema 4 and root-owned splash
playback before changing Runtime code.

### Starting State

- FL-M4-03 implementation is complete in `f997a9a`.
- FL-M4-03 documentation is complete in `b36e04d`.
- `main` and `origin/main` are synchronized at `b36e04d`.
- Working tree is clean.
- Runtime Play Mode baseline is 450 passed, 0 failed, 0 ignored.
- Unity compilation baseline is 0 errors and 0 compiler warnings.
- `SplashSequence` schema 1 and deterministic playback are implemented.
- `EchoLaunchConfiguration` remains schema 3 in Runtime.
- `LaunchReport` remains schema 2.
- `EchoLaunchRoot` does not yet own splash playback.
- FL-M4-04 changes serialized configuration and lifecycle order.
- Runtime implementation is locked until this authority update is committed.

---

## Approved FL-M4-04 Decisions

- `[AUTHORITY]` Package specification advances to v1.5.0.
- `[AUTHORITY]` Configuration advances to schema 4.
- `[AUTHORITY]` Schema 4 adds optional `SplashSequence`.
- `[AUTHORITY]` Schema 4 adds `UseReducedMotionForSplash`.
- `[AUTHORITY]` Null splash assignment is legal omission.
- `[AUTHORITY]` Empty valid sequence is legal no-op.
- `[AUTHORITY]` Assigned invalid sequence blocks preflight.
- `[AUTHORITY]` Root plays optional splash before startup steps.
- `[AUTHORITY]` Splash and steps do not run concurrently.
- `[AUTHORITY]` Root uses the launch clock and cancellation token.
- `[AUTHORITY]` Missing visuals warn and continue headless.
- `[AUTHORITY]` Cancellation during splash interrupts exactly once.
- `[AUTHORITY]` Playback failure blocks before steps and destination.
- `[AUTHORITY]` Report schema remains 2.
- `[AUTHORITY]` Runtime migration and silent repair remain prohibited.
- `[AUTHORITY]` Direct-scene mode uses the same configuration contract.

---

## Authority Files

- Package specification v1.5.0
- EchoLaunch ADR-002
- FL-M4-04 Checkpoint Build Plan
- Suite Current Notes
- Package Current Notes
- Package Documentation Index

---

## Latest Validation Snapshot

| Area | Result |
|---|---|
| Repository baseline | `b36e04d` |
| Last implementation | `f997a9a` |
| Runtime Play Mode | 450 passed, 0 failed, 0 ignored |
| Unity compilation | 0 errors, 0 compiler warnings |
| Implemented configuration schema | 3 |
| Approved FL-M4-04 schema | 4 |
| Splash schema | 1 |
| Report schema | 2, preserved |
| Root splash integration | Not implemented |
| Authority update | Prepared, not committed |
| Runtime implementation | Locked |

---

## Next Action

1. Apply the FL-M4-04 authority bundle.
2. Review the six-file scope.
3. Commit and push:

```text
echo-launch: approve FL-M4-04 splash schema 4 and root order
```

4. Confirm clean synchronized repository.
5. Begin the runtime implementation bundle from that authority commit.

---

## Handoff Snapshot

**Completed checkpoint:** FL-M4-03
**Active authority checkpoint:** FL-M4-04
**Baseline:** `b36e04d`
**Tests:** 450 passed, 0 failed, 0 ignored
**Compilation:** 0 errors, 0 compiler warnings
**Known blockers:** None
**Implementation lock:** Active until authority commit
**Next implementation boundary:** Schema-4 configuration binding and sequential root-owned splash playback
