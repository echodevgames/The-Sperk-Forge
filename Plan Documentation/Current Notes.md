# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 6, 2026
**Current focus:** First Light FL-M5-07 authority
**Current checkpoint:** FL-M5-07 — Standalone Test Laboratory and Importable Package Sample

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Starting State

- Branch: `main`
- HEAD: `e28ff09`
- `main` equals `origin/main`
- Working tree: clean
- FL-M5-06 authority: `a159349`
- FL-M5-06 implementation: `956c381`
- FL-M5-06 documentation: `e28ff09`
- Compilation baseline: `0` errors, `0` warnings
- EditMode baseline: `290` passed
- Runtime Play Mode baseline: `503` passed
- Total automated baseline: `793` passed
- Specification: v1.12.0 before this authority update
- FL-M5-07 implementation locked until authority commit

## Learning Review Decisions

- `[DECISION]` The next bounded checkpoint is `FL-M5-07`, not M6 adoption.
- `[DECISION]` FL-M5-07 closes deferred standalone MVP evidence without
  renumbering completed checkpoints.
- `[DECISION]` Ship one importable UPM sample named
  `First Light Standalone Test Lab`.
- `[DECISION]` Declare the sample in `package.json`; import remains explicit
  and user initiated.
- `[DECISION]` Imported sample content becomes project-owned and removable.
- `[DECISION]` Core Runtime and Editor assemblies may not depend on sample
  code, scenes, assets, or utilities.
- `[DECISION]` Sample code uses only public First Light Runtime APIs.
- `[DECISION]` The sample contains no peer Sperk's Forge or project-specific
  runtime assembly reference.
- `[DECISION]` Use Boot and Destination scenes; Destination also carries the
  Direct Scene proof unless evidence requires a third scene.
- `[DECISION]` Use visible pre-authored scenario configurations rather than
  mutating authored assets during Play.
- `[DECISION]` Include immediate, timed progress, warning, recoverable
  failure, and blocking failure sample steps.
- `[DECISION]` Use package-qualified Laboratory IDs
  `ELAUNCH-LAB-001` through `ELAUNCH-LAB-012`.
- `[DECISION]` Build Settings remain explicit and are not changed by import.
- `[DECISION]` Sample removal and reimport are acceptance gates.
- `[DECISION]` A temporary Unity authoring workspace is permitted only for
  sample creation/validation and must be removed before commit.
- `[DECISION]` Existing package defects exposed by the Laboratory may receive
  only narrow checkpoint-owned fixes with complete regression evidence.
- `[DECISION]` M6 adoption, bridges, release claims, migration, receipts,
  uninstall, recovery, report export, and build hooks remain separate.

## Planned Acceptance Registry

- `ELAUNCH-LAB-001` successful canonical Boot launch.
- `ELAUNCH-LAB-002` deterministic timed progress.
- `ELAUNCH-LAB-003` warning continuation.
- `ELAUNCH-LAB-004` missing required configuration blocks.
- `ELAUNCH-LAB-005` blocking failure prevents destination handoff.
- `ELAUNCH-LAB-006` duplicate root produces zero duplicate side effects.
- `ELAUNCH-LAB-007` invalid destination blocks preflight.
- `ELAUNCH-LAB-008` direct scene creates one development authority.
- `ELAUNCH-LAB-009` direct scene reuses existing authority.
- `ELAUNCH-LAB-010` splash skip respects minimum timing.
- `ELAUNCH-LAB-011` sample removal preserves package compilation.
- `ELAUNCH-LAB-012` reimport/setup/repair repeat without duplicates.

## Next Action

Commit and push:

```text
Approve FL-M5-07 standalone laboratory authority
```

Implementation may begin only after that authority commit.
