# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 5, 2026
**Current focus:** First Light post-FL-M5-02 reconciliation
**Current checkpoint:** FL-M5-02 — Approved Setup Apply Engine and Repeat-Safe Asset Creation — complete

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Completed Checkpoint

FL-M5-02 implemented the first approved First Light project-mutation boundary.
The Setup window now applies one fresh executable plan, creates only missing
project-owned foundation content, reuses compatible existing assets, preserves
scene state, mutates Build Settings only through the selected policy, rolls
back active-attempt changes on failure, and becomes a no-op on repeat Apply.

## Repository Evidence

- Authority commit: `208ee71` — `echo-launch: approve FL-M5-02 repeat-safe setup apply`
- Implementation commit: `f05b95c` — `Implement repeat-safe First Light setup apply engine`
- Branch: `main`
- Remote: `origin/main`
- Implementation push: complete
- Working tree after implementation push: clean

## Validation Evidence

- EditMode: `197` passed, `0` failed, `0` ignored
  - Setup and apply: `170`
  - Presentation prefab: `27`
- Runtime Play Mode: `479` passed, `0` failed, `0` ignored
- Total automated tests: `676`
- Compilation: `0` errors, `0` warnings
- First manual Apply: `Succeeded`
- Second manual Apply: `NoChanges`
- Third manual Apply: `NoChanges`
- Stable plan fingerprint: `7e669d66eaab2c04a0dfbc4445458fcd976808c83f62db82c3d91a16494fc0c1`
- Build Settings proof: existing `OutdoorsScene` retained at index `0`; one enabled Boot scene appended at index `1`
- Rollback completed: not required
- Manual recovery paths: none

## Promoted Decisions

- Freshness is recollected and replanned immediately before writes.
- Only `Create`, `Reuse`, and `NoChange` operations are executable.
- Existing compatible content is reused and never silently repaired.
- Incompatible or unsupported content remains blocking.
- Project-owned folders, definitions, configuration, root prefab variant, and Boot scene are created deterministically.
- Build Settings is written last and only through the approved policy.
- One active Apply is allowed.
- Active-attempt failure uses an in-memory compensating rollback journal.
- Repeat Apply returns `NoChanges` without duplicate assets or Build Settings entries.

## Deferred Boundary

Repair, migration, persistent receipts, uninstall/reset, crash-persistent
recovery, Direct Scene initialization, Validator, real Standalone Laboratory
activation, player builds, clean external installation, and performance evidence
remain outside FL-M5-02.

## Next Action

Apply and review the FL-M5-02 documentation closeout, then commit and push:

```text
echo-launch: document FL-M5-02 completion
```

## Handoff

**Completed:** FL-M5-02
**Implementation:** `f05b95c`
**Blockers:** None
**Tentative next:** FL-M5-03 — Explicit Setup Repair and Existing-Asset Reconciliation
**Authorization state:** Not yet approved for implementation
