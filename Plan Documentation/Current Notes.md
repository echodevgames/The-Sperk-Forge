
# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 5, 2026
**Current focus:** First Light FL-M5-01 authority promotion
**Current checkpoint:** FL-M5-01 — Editor Setup Foundation and Non-Destructive Project Plan

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current Focus

### Goal

Commit the Editor setup-planning boundary before adding any package tool that
could modify project assets, scenes, or Build Settings.

### Starting State

- FL-M4-05 authority is complete in `311a9d2`.
- FL-M4-05 implementation is complete in `8d3c6a7`.
- FL-M4-05 documentation is complete in `8bd2a57`.
- `main` and `origin/main` are synchronized at `8bd2a57`.
- Working tree is clean.
- EditMode baseline is 27 passed, 0 failed, 0 ignored.
- Runtime Play Mode baseline is 479 passed, 0 failed, 0 ignored.
- Unity compilation baseline is 0 errors and 0 compiler warnings.
- Stable package root and status-view prefabs exist.
- The Editor assembly exists but setup behavior is not implemented.
- FL-M5-01 implementation remains locked until this authority update is pushed.

---

## Approved FL-M5-01 Decisions

- `[AUTHORITY]` Package specification advances to v1.7.0.
- `[AUTHORITY]` Setup architecture separates observation, planning, and mutation.
- `[AUTHORITY]` FL-M5-01 implements no mutation stage.
- `[AUTHORITY]` Project observation is captured in an immutable read-only snapshot.
- `[AUTHORITY]` Setup intent is an immutable in-memory request.
- `[AUTHORITY]` The planner is deterministic and pure after snapshot capture.
- `[AUTHORITY]` The Setup window is preview-only.
- `[AUTHORITY]` Default project root is `Assets/EchoDevGames/FirstLight`.
- `[AUTHORITY]` Destination scene must already exist.
- `[AUTHORITY]` Future setup uses a project-owned root prefab variant.
- `[AUTHORITY]` Existing compatible assets are reused.
- `[AUTHORITY]` Incompatible target assets block.
- `[AUTHORITY]` Ambiguous candidates require manual selection.
- `[AUTHORITY]` Default Build Settings policy is append-if-missing.
- `[AUTHORITY]` Moving Boot to index zero requires explicit approval.
- `[AUTHORITY]` Existing unrelated scene order is preserved.
- `[AUTHORITY]` Unsupported schema blocks and does not migrate.
- `[AUTHORITY]` Stable `ELAUNCH-SETUP-001` through `ELAUNCH-SETUP-007` diagnostics are approved.
- `[AUTHORITY]` No EditorPrefs project identity is stored.
- `[AUTHORITY]` No scene is opened during project snapshot collection.

---

## Authority Files

- Package specification v1.7.0
- EchoLaunch ADR-004
- FL-M5-01 Checkpoint Build Plan
- Suite Current Notes
- Package Current Notes
- Package Documentation Index

---

## Latest Validation Snapshot

| Area | Result |
|---|---|
| Repository baseline | `8bd2a57` |
| Last implementation | `8d3c6a7` |
| EditMode | 27 passed, 0 failed, 0 ignored |
| Runtime Play Mode | 479 passed, 0 failed, 0 ignored |
| Unity compilation | 0 errors, 0 compiler warnings |
| Editor setup implementation | Not implemented |
| Apply/repair engine | Explicitly excluded |
| Authority update | Prepared, not committed |
| Implementation lock | Active |

---

## Next Action

1. Apply the FL-M5-01 authority bundle.
2. Review the six-file scope.
3. Commit and push:

```text
echo-launch: approve FL-M5-01 non-destructive setup planning
```

4. Confirm clean synchronized repository.
5. Implement the read-only snapshot, pure planner, and preview-only window.

---

## Handoff Snapshot

**Completed checkpoint:** FL-M4-05
**Active authority checkpoint:** FL-M5-01
**Baseline:** `8bd2a57`
**EditMode:** 27 passed, 0 failed, 0 ignored
**Runtime Play Mode:** 479 passed, 0 failed, 0 ignored
**Compilation:** 0 errors, 0 compiler warnings
**Known blockers:** None
**Implementation lock:** Active until authority commit
**Next implementation boundary:** Preview-only Editor Setup window backed by a read-only snapshot and deterministic immutable plan
