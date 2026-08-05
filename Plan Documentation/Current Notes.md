
# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 5, 2026
**Current focus:** First Light FL-M4-05 authority promotion
**Current checkpoint:** FL-M4-05 — Startup Presentation Prefab and Canvas Assembly

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current Focus

### Goal

Commit the prefab-template and Canvas hierarchy authority before generating
public package assets.

### Starting State

- FL-M4-04 authority is complete in `90aabd1`.
- FL-M4-04 implementation is complete in `858808b`.
- FL-M4-04 documentation is complete in `9d6d469`.
- `main` and `origin/main` are synchronized at `9d6d469`.
- Working tree is clean.
- Runtime Play Mode baseline is 479 passed, 0 failed, 0 ignored.
- Unity compilation baseline is 0 errors and 0 compiler warnings.
- Configuration schema is 4.
- Report schema is 2.
- The plain uGUI presenter and root-owned splash path are implemented.
- No package prefab or Canvas hierarchy exists yet.
- FL-M4-05 creates stable public package assets.
- Prefab generation is locked until this authority update is committed.

---

## Approved FL-M4-05 Decisions

- `[AUTHORITY]` Package specification advances to v1.6.0.
- `[AUTHORITY]` Package ships `EchoLaunchStatusView.prefab`.
- `[AUTHORITY]` Package ships `EchoLaunchRoot.prefab`.
- `[AUTHORITY]` Prefabs live under `Presentation.UGUI/Prefabs`.
- `[AUTHORITY]` Status prefab is a Screen Space Overlay Canvas.
- `[AUTHORITY]` Canvas scales from 1920x1080 with 0.5 match.
- `[AUTHORITY]` Root prefab nests the status prefab.
- `[AUTHORITY]` Root configuration remains null.
- `[AUTHORITY]` Root mode is CanonicalBoot with automatic start enabled.
- `[AUTHORITY]` No project logo, branded art, project font, or project asset.
- `[AUTHORITY]` No TextMeshPro dependency.
- `[AUTHORITY]` No EventSystem, input module, GraphicRaycaster, Button, or skip binding.
- `[AUTHORITY]` All graphics are non-raycast targets.
- `[AUTHORITY]` Runtime performs no prefab discovery or automatic instantiation.
- `[AUTHORITY]` Projects customize through copies, variants, or replacement presenters.
- `[AUTHORITY]` Prefab and folder `.meta` identities become stable package evidence.
- `[AUTHORITY]` A temporary uncommitted Unity authoring helper may generate YAML but cannot enter final scope.

---

## Authority Files

- Package specification v1.6.0
- EchoLaunch ADR-003
- FL-M4-05 Checkpoint Build Plan
- Suite Current Notes
- Package Current Notes
- Package Documentation Index

---

## Latest Validation Snapshot

| Area | Result |
|---|---|
| Repository baseline | `9d6d469` |
| Last implementation | `858808b` |
| Runtime Play Mode | 479 passed, 0 failed, 0 ignored |
| Unity compilation | 0 errors, 0 compiler warnings |
| Configuration schema | 4 |
| Report schema | 2 |
| Status view code | Implemented |
| Package status prefab | Not implemented |
| Package root prefab | Not implemented |
| Authority update | Prepared, not committed |
| Prefab generation | Locked |

---

## Next Action

1. Apply the FL-M4-05 authority bundle.
2. Review the six-file scope.
3. Commit and push:

```text
echo-launch: approve FL-M4-05 neutral presentation prefabs
```

4. Confirm clean synchronized repository.
5. Generate the prefab assets from that authority commit.

---

## Handoff Snapshot

**Completed checkpoint:** FL-M4-04
**Active authority checkpoint:** FL-M4-05
**Baseline:** `9d6d469`
**Tests:** 479 passed, 0 failed, 0 ignored
**Compilation:** 0 errors, 0 compiler warnings
**Known blockers:** None
**Implementation lock:** Active until authority commit
**Next implementation boundary:** Two stable neutral package prefab templates and serialized Canvas/root wiring
