# The Sperk’s Forge – Suite Health Check and Remaining Documentation

**Updated:** August 10, 2026
**Completed package checkpoint:** ESV-M4-04 – Chronicle Public Manual Save Admission, Busy, Cancellation, and Lifecycle Foundation
**Current implementation state:** First Light complete/frozen; Chronicle M2 complete; Chronicle M3 complete; Chronicle M4-01 through M4-04 complete; ESV-M4-04 final effective runtime baseline `09ae8f1` with `456 / 456`; next M4 checkpoint not yet activated

## Current health

| Area | Status |
|---|---|
| Suite Bible | Approved; no FL-M6-01 closeout authority change required |
| Standards | SFGSS-001 through SFGSS-010 complete |
| Package authorities | 28 of 28 approved |
| Learning workflow | Just-in-time package-local gate remains authoritative |
| First Light learning gate | Passed |
| First Light package version | `0.1.0` |
| First Light specification | SFGSS-PKG-ECHOLAUNCH-001 v1.16.0 |
| First Light Standalone Test Lab | Complete; retained `809 / 809` automated and `12 / 12` manual evidence |
| First Light Package Reference Showcase | **Complete** |
| Latest First Light Setup-focused gate | **224 / 224** |
| Permanent First Light Gallery | **Complete** — First Light Example + UMBRA Example |
| Clean-project reproduction | Not run in FL-M6-01 closeout |
| Release qualification/private beta | Not run |
| Next package learning | **PKG-LEARN-009 – The Chronicle (`EchoSave`) complete** |
| Chronicle implementation | **M3 complete; ESV-M4-01 through ESV-M4-04 complete; M4-04 effective runtime baseline `09ae8f1` with 456 / 456; next M4 checkpoint not yet activated** |
| Game Shell initiative | Chronicle → Accord → Resonance → Looking Glass; sequence is planning, not a hard dependency chain |
| Other package implementations | Not activated |
| Release-blocking architecture conflicts | None recorded by FL-M6-01 |

## First Light graduation state

```text
Learning / authority                 PASS
Implementation                       PASS for current approved scope
Standalone Test Lab                  PASS
Package Reference Showcase           PASS
Clean-project reproduction           NOT RUN
Release qualification                NOT RUN
Private beta / external adoption     NOT RUN
```

First Light is therefore **complete for its current in-repository implementation and Reference Showcase pass**, but this document does not call the package release-qualified.

## Permanent consumer proof

The project now retains:

```text
Assets/EchoDevGames/SuiteShowcase/First Light Gallery/
├── First Light Example/
└── UMBRA Example/
```

The Gallery demonstrates both the canonical EchoDevGames happy path and an independently-authored UMBRA consumer foundation. Additional future Gallery examples may be project-owned content exercises; they do not automatically authorize package changes.

## Evidence carried forward

- FL-M5-07 retained full automated baseline: `809 / 809`.
- FL-M5-07 manual Laboratory matrix: `12 / 12`.
- FL-M6-01-H1 focused identity gate: `5 / 5`.
- FL-M6-01-H2 focused destination-Build-Settings gate: `35 / 35`.
- Final FL-M6-01 `EchoLaunchSetup` filtered EditMode gate: `224 / 224`.
- UMBRA fresh-root Create Project-Owned Setup proof created the requested foundation and serialized three authored splashes.
- UMBRA runtime Boot presentation succeeded.
- Identical second Apply returned `NoChanges`, created no paths, and preserved Build Settings.

No post-A1 complete EditMode or Runtime Play Mode aggregate is invented here. Full-suite regression must be collected again when release qualification is activated.

## Active next-package work

The next package is **The Chronicle (`EchoSave`)**.

Chronicle M1, M2, and **M3 — Participants and Loading** are complete.

`ESV-M4-01` is complete at `62e8a54`; the focused Chronicle Editor gate passed **403 / 403**. Chronicle can now discover existing technical slots through an additive provider-neutral capability, rebuild payload-free lightweight metadata from authoritative heads/current manifests, preserve degraded technical slots honestly, and maintain explicit session-only active-slot selection.

`ESV-M4-02` is complete at `d8d5c18`; the focused Chronicle Editor gate passed **425 / 425**. Chronicle can create a bounded technical slot as one real empty immutable generation, enforce capacity across healthy and degraded canonical slots, reject generated-ID collisions within a positive bound, publish `head.json` last, reconcile the catalog without auto-selecting, and report published-but-reconciliation-failed truth without fictional rollback.

`ESV-M4-03` is complete at `c8ea742`; the focused Chronicle Editor gate passed **439 / 439**. Chronicle can perform one bounded internal manual-save transaction against the explicitly selected healthy slot, validate exact source provenance, capture fresh known participant state, preserve valid opaque unknown payloads, reject stale source/ownership collisions, publish one participant-backed immutable generation with `head.json` last, preserve ordinary display-name metadata, and reconcile the catalog with truthful partial durable/head/catalog outcomes.

`ESV-M4-04` is complete at implementation commit `2732aaa` with bounded lifecycle-status hotfix `09ae8f1`; the final focused Chronicle Editor gate passed **456 / 456**. Chronicle now exposes public active-slot `SaveAsync`, admits one root-local mutating operation at a time, rejects overlapping manual save as Busy without queueing, honors safe pre-publication cancellation, reports Too Late after durable publication begins, and closes new admission during shutdown while preserving truthful durable settlement.

No follow-on Chronicle checkpoint is activated by this closeout. Persistent catalog-cache optimization, rename/duplicate/delete, full slot-policy configuration assets, autosave/coalescing, generic queued multi-operation scheduling, retention, recovery, document migration, scene travel, peer bridges, and project-wide DDOL composition remain later bounded work. SFGSS-ADR-006 and the Laboratory → Reference Showcase → clean-project → Distribution Kit/release-evidence loop remain authoritative.

The planned follow-on Game Shell sequence is Accord, Resonance, then Looking Glass. That order is a development plan, not a hard dependency graph.

## First Light future return gates

If First Light returns to active development, the next release-oriented work remains separate from FL-M6-01:

- clean-project reproduction of the proven happy path;
- full current regression totals;
- supported installation-route evidence;
- player-build qualification where required;
- release checklist/version/tag/catalog decisions;
- private beta/external adoption.

No such work is active merely because FL-M6-01 closed.

## Current stop point

`ESV-M4-04` is **complete** at final effective runtime baseline `09ae8f1` with **456 / 456** focused Chronicle Editor tests.

**M3 — Participants and Loading is complete. M4 — Slots / Autosave / Recovery remains active.**

No next Chronicle checkpoint is currently activated. Further M4 implementation requires a bounded authorized Checkpoint Build Plan and must preserve the **456 / 456** regression floor.

Persistent catalog cache, rename/duplicate/delete, full slot-policy assets, autosave/coalescing, generic queued multi-operation scheduling, retention, recovery, document migration, scene travel, peer persistence bridges, and project-wide DDOL composition remain locked for later checkpoints.
