# The Sperk’s Forge – Suite Health Check and Remaining Documentation

**Updated:** August 11, 2026
**Completed package checkpoint:** ESV-M4-R2 – Chronicle Slot Policy Runtime Configuration and CAP-002 Reconciliation
**Current implementation state:** First Light complete/frozen; Chronicle ESV-M4-R1 and ESV-M4-R2 complete; R2 implementation `8a8e7e7` with focused `636 / 636`; R3/final reconciliation remain; M4 open; M5 locked

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
| Chronicle implementation | **ESV-M4-R1 and ESV-M4-R2 complete; R2 at `8a8e7e7` with 636 / 636; ESV-M4-R3 package-document migration is next / not activated; final reconciliation remains before M4 close; M5 locked** |
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

`ESV-M4-05` is complete at `9917f1b`; the focused Chronicle Editor gate passed **473 / 473**. Chronicle now accepts explicit caller-triggered autosave requests, retains at most one latest pending request, coalesces/supersedes without growing a queue, reuses the same M4-04 root-local admission and M4-03/M4-04 durable active-slot save path, preserves manual-save Busy behavior, drains pending autosave at most once after admission release, and prevents pending work from starting after shutdown admission closure.

`ESV-M4-06` is complete at `e714a90`; the focused Chronicle Editor gate passed **497 / 497**. Chronicle now bounds ordinary committed-generation history after successful publication, protects current and immediate predecessor generations, fails closed on untrustworthy history, deletes only oldest excess verified committed generations through optional provider-neutral tree deletion, and reports retention maintenance separately from committed save truth.

`ESV-M4-07` is complete at `9f68555`; the focused Chronicle Editor gate passed **524 / 524**. Chronicle can now build immutable read-only recovery plans that diagnose source state, discover retained generations through bounded provider-neutral reads, admit only fully verified committed candidates, order them deterministically newest-valid first, preserve/exclude bad evidence, and carry technical source provenance for later stale-plan rejection without mutating durable storage.

`ESV-M4-08` is complete at `1985fb0`; the focused Chronicle Editor gate passed **540 / 540**. Chronicle can now explicitly execute one M4-07 recovery choice through shared mutation admission, reject stale plan/candidate evidence before mutation, repoint only `head.json` to an already verified immutable generation, preserve generation bytes, and reconcile catalog truth after the durable head commit.

`ESV-M4-09` is complete at `459023f`; the focused Chronicle Editor gate passed **562 / 562**. Chronicle can now rename one canonical healthy slot while preserving technical slot identity/path and publishing display metadata through a new immutable generation, and can duplicate one fully verified current source state into a new package-generated slot/generation identity. Both operations reuse root-local admission, source revalidation, head-last publication, and truthful post-publication catalog semantics; rename reuses retention, duplicate honors canonical capacity, participant callbacks remain absent, and the new duplicate is not auto-selected.


`ESV-M4-10` is complete at `01e4cdd`; the focused Chronicle Editor gate passed **587 / 587**. Chronicle now provides read-only zero-mutation deletion planning, immutable expiring one-use source-bound plans, root-local Busy/no-queue confirmed deletion, fresh source revalidation, complete-tree recoverable trash publication, active-slot/catalog reconciliation, bounded fail-closed trash retention, and truthful post-commit failure states.

`ESV-M4-R1` is complete at `ab18361`; the focused Chronicle Editor gate passed **618 / 618** and closed audit gaps A-01/A-02 through bounded public runtime composition.

`ESV-M4-R2` is complete at `8a8e7e7`; the focused Chronicle Editor gate passed **636 / 636**, adding **18** focused policy tests. Chronicle now resolves schema-2 project-owned `SingleSlot`, `FixedMultiSlot`, `ConfigurableMultiSlot`, and `BoundedProfiles` policy into one immutable finite service-session capacity shared by create and duplicate, while schema-1 remains non-mutating compatible at historical capacity 64. ESV-T-015 through ESV-T-018 are complete and audit gap A-03 is closed.

No follow-on runtime checkpoint is activated by this closeout. ESV-M4-R3 package-document migration is next but still requires bounded activation; final 100-case registry/document evidence reconciliation remains mandatory after R3. Permanent erase, restore-from-trash public API, quarantine/cleanup, persistent catalog-cache optimization, automatic autosave timers, generic queued multi-operation scheduling, scene travel, peer bridges, and project-wide DDOL composition remain later bounded work.

SFGSS-ADR-006 and the Laboratory → Reference Showcase → clean-project → Distribution Kit/release-evidence loop remain authoritative.

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

`ESV-M4-R2` is **complete** at implementation commit `8a8e7e7`.

Final focused Chronicle Editor evidence: **636 / 636 passed, 0 failed**, preserving the incoming **618 / 618** floor.

CAP-002 schema-2 slot-policy runtime configuration is reconciled. Schema-1 capacity-64 compatibility remains read-only/non-mutating, and create/duplicate share one resolved immutable session capacity.

ESV-M4-R3 package-document migration is **next / not activated**.

Still mandatory before M4 close:
- R3 package-document migration preserving CAP-014;
- final 100-case registry/document evidence reconciliation;
- final focused Chronicle regression evidence at the actual closing total.

**M5 remains locked.**
