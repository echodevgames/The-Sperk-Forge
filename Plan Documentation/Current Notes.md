# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 7, 2026
**Current focus:** First Light FL-M5-07 Standalone Test Laboratory
**Current checkpoint:** FL-M5-07 — Standalone Test Laboratory and Importable UPM Sample

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Canonical Starting Baseline

- Package: First Light (`EchoLaunch`)
- Package version: `0.1.0`
- FL-M5-06 closeout baseline: `e28ff09`
- Post-rewind reconciliation: two living `Current Notes.md` pages only; no implementation change
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.13.0
- Compilation baseline: `0` errors, `0` warnings
- Complete EditMode baseline: `290` passed
- Runtime Play Mode baseline: `503` passed
- Total automated baseline: `793` passed
- Working tree required clean before FL-M5-07 implementation begins

## Active FL-M5-07 Authority

- `[DECISION]` FL-M5-07 implements the already-approved First Light Standalone Test Laboratory as exactly one UPM sample named **First Light Standalone Test Lab**.
- `[DECISION]` The shipped sample is fully authored content under `Samples~/First Light Standalone Test Lab/`; it does not ship a second setup/generation engine.
- `[DECISION]` Importing the sample copies authored content into Unity's normal `Assets/Samples/...` destination and performs no automatic generation, scene mutation, Build Settings mutation, Setup/Repair invocation, validation run, Simulator run, or Play Mode action.
- `[DECISION]` The sample contains its own narrow sample assembly for sample-only step/readout code. No project-specific runtime assembly and no unrelated Sperk's Forge package is permitted.
- `[DECISION]` Sample scenes/configurations/prefabs are serialized with their required references before distribution. Imported reference integrity is an acceptance gate; users must not hand-repair missing references to make the sample pass.
- `[DECISION]` Package Runtime, Direct Scene, Setup/Repair, Validator, Simulator, presentation, schema, and diagnostic contracts remain unchanged unless the Laboratory exposes a reproducible defect in an existing contract.
- `[DECISION]` If imported `Assets/Samples/**` content is found to pollute automatic Setup candidate discovery, only a narrow standard imported-sample exclusion may be added, with explicit user selection remaining available and dedicated regression coverage required. No speculative production Editor change is authorized without that evidence.
- `[DECISION]` FL-M5-07 does not resurrect or copy implementation from discarded post-`e28ff09` history. All implementation is reviewed against this fresh authority and current source.

## Required Laboratory Proof

FL-M5-07 must exercise the twelve previously approved package acceptance cases:

1. `LAB-001` canonical Boot success.
2. `LAB-002` timed progress.
3. `LAB-003` warning continues.
4. `LAB-004` missing configuration blocks with `ELAUNCH-CFG-001`.
5. `LAB-005` blocking failure stops before destination handoff.
6. `LAB-006` duplicate authority is rejected with `ELAUNCH-ROOT-001` and zero duplicate side effects.
7. `LAB-007` invalid destination blocks with `ELAUNCH-DEST-001`.
8. `LAB-008` direct-scene entry creates one development authority when absent.
9. `LAB-009` direct-scene entry reuses an existing authority without duplication.
10. `LAB-010` minimum splash duration and skip policy remain enforced.
11. `LAB-011` removing the imported sample leaves package compilation/tooling healthy.
12. `LAB-012` Setup and Repair remain repeat-safe across three runs.

## Explicit Exclusions

FL-M5-07 does not authorize:

- a sample authoring/generation window or shipped generation service;
- automatic sample scene installation;
- automatic Build Settings edits on sample import;
- runtime discovery of package/sample content;
- another launch pipeline or sample-specific launch authority;
- report export formats;
- schema migration;
- receipts, uninstall, or crash-persistent recovery;
- build hooks;
- external package integrations;
- unrelated Runtime or Editor refactors.

## Next Action

Commit and push this FL-M5-07 authority package. Then implement only the files and conditional defect corrections named by the checkpoint plan.
