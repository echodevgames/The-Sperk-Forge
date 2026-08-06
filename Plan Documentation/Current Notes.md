# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 6, 2026
**Current focus:** First Light FL-M5-05 authority
**Current checkpoint:** FL-M5-05 — Direct Scene Development Initializer

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Starting State

- Branch: `main`
- HEAD: `4e3bf34`
- `main` equals `origin/main`
- Working tree: clean
- FL-M5-04 authority: `c2397c9`
- FL-M5-04 implementation: `26732ea`
- FL-M5-04 documentation: `4e3bf34`
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `261` passed
- Runtime Play Mode baseline: `479` passed
- Total automated baseline: `740` passed
- Specification: v1.10.0 before this authority update
- FL-M5-05 implementation locked until authority commit

## Approved Decisions

- Direct Scene enters the existing launch architecture.
- Scene roots claim in `Awake`; helper waits until `Start`.
- Existing authority is reused before creation.
- Helper references one project-owned immutable `DirectSceneConfiguration`.
- The direct root prefab is pre-authored as `DirectSceneDevelopment`.
- Destination must match the containing scene.
- Active destination completes without reload.
- Default policy is `EditorOnly`.
- Development Builds require explicit opt-in.
- Non-development release creation is impossible.
- `BootRequired` blocks.
- Activate `ELAUNCH-VAL-009`.
- No build hook or automatic helper installation.
- Preserve LaunchReport schema version `2`.

## Next Action

Commit and push:

```text
Approve FL-M5-05 direct scene initializer authority
```

Implementation may begin only after that commit.

## Handoff

**Checkpoint:** FL-M5-05
**Baseline:** `4e3bf34`
**Specification target:** v1.11.0
**ADR:** EchoLaunch-ADR-008
**Implementation:** Locked until authority commit
**Blockers:** None recorded
