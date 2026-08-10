# The Sperk’s Forge – Suite Health Check and Remaining Documentation

**Updated:** August 9, 2026
**Completed package checkpoint:** ESV-M3-08 – Chronicle Prepared-Load Handle Lifecycle and Session Ownership Foundation
**Current implementation state:** First Light complete/frozen; Chronicle M2 complete; ESV-M3-01 through ESV-M3-08 complete; ESV-M3-09 active / authorized

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
| Chronicle implementation | **ESV-M3-08 complete at `798d38d` with 332 / 332; ESV-M3-09 active / authorized** |
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

Chronicle M1 and M2 are complete. M3-01 through M3-08 are now complete.

`ESV-M3-08` is complete at `798d38d`; the focused Chronicle Editor gate passed **332 / 332**. Chronicle can now retain one exact-source validated/migrated prepared load behind a bounded opaque disposable handle with package/session ownership, deterministic expiry/disposal/invalidation, defensive unknown snapshot retention, and live-handle resource bounds.

`ESV-M3-09` is active / authorized for deterministic participant apply preflight/execution and missing-payload policy. Jesse approved an additive optional `ISaveDefaultableParticipant.InitializeDefault()` capability; the base `ISaveParticipant` contract remains unchanged and `Apply(null)` is not default protocol.

Rollback/compensation, production async operation admission, convenience loading, scene travel, document migration, slot catalogs, recovery, retention, autosave, peer bridges, and project-wide DDOL composition remain later bounded work. SFGSS-ADR-006 and the Laboratory → Reference Showcase → clean-project → Distribution Kit/release-evidence loop remain authoritative.

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

`ESV-M3-08` is **complete** with **332 / 332** focused Chronicle Editor tests.

`ESV-M3-09` is **active / authorized** for deterministic participant apply/missing-payload policy only.

The approved default-init seam is additive `ISaveDefaultableParticipant.InitializeDefault()`. The base participant contract remains unchanged.

Rollback/compensation, production async operation admission, convenience loading, scene travel, document migration, slot catalog behavior, recovery/retention/autosave, peer persistence bridges, and project-wide DDOL composition remain locked for later checkpoints.
