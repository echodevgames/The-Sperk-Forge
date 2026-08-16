# The Sperk’s Forge — Foundation Wave Specification Roadmap

**Document role:** Level 4 planning and checkpoint record  
**Status:** Foundation documentation pass complete; implementation activation superseded by SFGSS-ADR-002  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Parent authority:** SFGSS-000 v0.9.0, SFGSS-001 v1.1.0, and SFGSS-ADR-002  
**Unity baseline:** Unity 6000.3.8f1  
**Public Unity floor:** Unity 6000.0  
**Last updated:** August 3, 2026

> Draw every load-bearing line before the Forge gets hot.

> **Historical record:** This roadmap preserves the August 3 Foundation documentation/initial-activation state. Its “active” checkpoint statements are historical and must not be used for current rehydration. Use `Suite_Graph_Roadmap.md`, `Suite_Health_Check_and_Remaining_Documentation.md`, root/package Current Notes, and live Git history for current implementation state.

---

## 1. Purpose

This roadmap governs the documentation-first pass for the ten Foundation Wave packages in SFGSS-000 Section 7.1. Its purpose is to establish complete package authorities, MVP boundaries, public contracts, data ownership, lifecycle rules, Test Labs, diagnostics, optional bridges, and release gates before runtime implementation begins.

This roadmap is not a substitute for an individual package specification. Each package receives its own full SFGSS-001 document and becomes a Level 2 authority only after approval.

---

## 2. Documentation Gate

No Foundation Wave runtime package may enter M1 package-skeleton implementation until all of the following are true:

1. All ten package specifications are approved.
2. Every specification identifies its owned concern and explicit non-goals.
3. Cross-package authority and dependency contradictions are resolved.
4. Optional bridges are named and their dependency direction is visible.
5. Standalone Test Labs are designed for every scene-visible package.
6. The final consistency review FW-DOC-12 passes.
7. `Current Notes.md`, SFGSS-000, and affected package specifications are reconciled and committed.

This gate does not require every later expansion idea to be solved. Non-MVP questions may remain Deferred when they do not alter package authority, public MVP contracts, persistence ownership, or neighboring package design.

---

## 3. Specification Order

The order follows the Foundation Wave ownership layers: origin and observability first, coordination authorities second, user-facing services third, and the composer last.

| Checkpoint | Package | Public title | Why this position matters | Status |
|---|---|---|---|---|
| FW-DOC-01 | `EchoLaunch` | First Light — Startup and Launch | Defines startup ordering, launch reporting, handoff, and optional initialization bridge seams | Approved v1.0.0 |
| FW-DOC-02 | `EchoDiagnostics` | The Observatory — Diagnostics | Defines the neutral status-provider model and diagnostic vocabulary used by later packages without becoming mandatory | Approved v1.0.0 |
| FW-DOC-03 | `EchoSettings` | The Accord — Global Preferences | Establishes global preference ownership and change/application contracts used by audio, input, UI, localization, and feedback | Approved v1.0.0 |
| FW-DOC-04 | `EchoSceneFlow` | The Passage — Scene Flow | Defines normal transition authority after First Light handoff, loading phases, progress, locking, and failure behavior | Approved v1.0.0 |
| FW-DOC-05 | `EchoGameState` | The Pulse — Runtime State | Defines high-level modes, pause authority, time/cursor/input coordination requests, and nested state reasons | Approved v1.1.0 |
| FW-DOC-06 | `Jukebot` | Resonance — Audio Runtime | Defines music, SFX, ambience, mixer routing, handles, runtime cue state, and settings/state bridges | Approved v1.0.0 |
| FW-DOC-07 | `EchoInput` | The Will — Input Infrastructure | Defines input contexts, active-device state, rebinding, glyphs, lock reasons, and controller-independent intent support | Approved v1.0.0 |
| FW-DOC-08 | `EchoUI` | The Looking Glass — UI Framework | Defines screen/HUD/modal ownership and presenter boundaries after settings, state, input, scene, and audio contracts are known | Approved v1.0.0 |
| FW-DOC-09 | `EchoSave` | The Chronicle — Save Infrastructure | Defines files, slots, serializer/migration/recovery, and participant contracts with the neighboring package surfaces visible | Approved v1.0.0 |
| FW-DOC-10 | `EchoGameStarter` | The Workshop — Project Starter | Composer is specified last so its selectable outputs, reports, generated assets, and dependencies reflect approved package contracts | Approved v1.1.0 |
| FW-DOC-11 | Foundation contract matrix | Cross-spec reconciliation | Builds one package-to-package ownership, lifecycle, data, bridge, setup-facade, and removal matrix | Approved |
| FW-DOC-12 | Documentation readiness gate | Implementation authorization review | Confirms the reconciled set is ready and selects the first implementation checkpoint | Approved |

---

## 4. Required Package-Specification Depth

Every package page must complete all 30 SFGSS-001 sections. “Thorough” means the page makes the following explicit rather than merely listing features:

- One-sentence ownership contract.
- Problem evidence from existing EchoDevGames projects.
- Goals, non-goals, users, and measurable outcomes.
- Standalone guarantees and forbidden dependencies.
- MVP, later, deferred, and rejected capabilities.
- Definition/configuration, runtime state, and presentation separation.
- Persistent-root and duplicate behavior where applicable.
- Lifecycle, failure model, and stable diagnostic codes.
- Public types, methods, results, events, async/cancellation rules, and test seams.
- Editor setup, preview, repair, validation, and repeat-run behavior.
- Minimal production scene setup and direct-scene behavior.
- Standalone Test Lab contents and acceptance tests.
- Accessibility and customization rules.
- Persistence classification and optional save/settings contracts.
- Bridge placement and missing-peer behavior.
- Performance, allocation, domain reload, and scalability rules.
- Platform, privacy, package anatomy, assembly direction, versioning, and GUID policy.
- Documentation deliverables, test registry, release gates, migration plan, risks, decisions, milestones, and handoff state.

A specification may say **Not applicable**, but it must explain why.

---

## 5. Cross-Package Questions to Resolve During the Pass

These questions are distributed to the package that owns the truth, then checked from the consumer side in neighboring specifications.

| Contract | Primary owning specification | Specifications that must consume/check it |
|---|---|---|
| Initial launch phases and report | EchoLaunch | EchoDiagnostics, EchoSceneFlow, EchoGameState, EchoUI, EchoGameStarter |
| Structured diagnostic provider and code conventions | EchoDiagnostics | All remaining Foundation packages |
| Global preference storage and apply/change workflow | EchoSettings | Jukebot, EchoInput, EchoUI, EchoGameStarter |
| Initial handoff versus normal scene travel | EchoSceneFlow | EchoLaunch, EchoGameState, EchoUI, EchoSave, EchoGameStarter |
| Pause/time/cursor/input coordination | EchoGameState | EchoInput, EchoUI, Jukebot, EchoSceneFlow |
| Audio requests, handles, profile state, mixer application | Jukebot | EchoSettings, EchoUI, EchoGameState, EchoDiagnostics |
| Input contexts, locks, devices, rebinding, glyph data | EchoInput | EchoUI, EchoGameState, EchoSettings, EchoGameStarter |
| Screen/HUD/modal/navigation/presenter contracts | EchoUI | Settings, Input, Save, SceneFlow, GameState, Jukebot |
| Save slots, atomic writes, participants, migration, recovery | EchoSave | EchoUI, EchoGameStarter and later gameplay packages |
| Generated composition, package selection, dry run, repair | EchoGameStarter | Every Foundation package as an optional selectable output |

---

## 6. Per-Specification Checkpoint Loop

For each package:

1. Re-read SFGSS-000 sections relevant to that package.
2. Re-read all previously approved Foundation package specifications that touch it.
3. Draft the complete SFGSS-001 page.
4. Label unresolved points as Proposed, Deferred, or release-blocking.
5. Prefer the smallest durable architecture that preserves standalone operation.
6. Review the ownership boundary and cross-package contracts with Jesse.
7. Resolve implementation-shaping choices.
8. Mark the package specification Approved.
9. Reconcile `Current Notes.md` and the roadmap status.
10. Promote any suite-wide change into SFGSS-000 or an ADR.
11. Commit and push the documentation checkpoint.

No C# scripts, Unity scenes, prefabs, package manifests, or generated assets are created during this documentation pass except documentation-support files explicitly approved as part of the repository workflow.

---

## 7. FW-DOC-11 Cross-Spec Consistency Review

The consistency review produces one matrix and one findings record. It must check:

### 7.1 Authority

- Exactly one owner for startup, diagnostics, settings, scene travel, high-level state/pause, audio, input context/rebinding, UI presentation infrastructure, save files/slots, and starter composition.
- No package silently owns a peer’s state because it presents or requests that state.

### 7.2 Dependencies and bridges

- Every core package compiles without unrelated Echo packages.
- Every two-package connection identifies bridge ownership and direction.
- Removing an optional peer or bridge has a documented non-breaking result.
- No reflection-based package discovery is required for normal operation.

### 7.3 Lifecycle

- First Light handoff, scene flow, game state, UI, input, settings, save, diagnostics, and audio startup expectations do not form a circular initialization requirement.
- Duplicate persistent-root rules are compatible and happen before side effects.
- Direct-scene helpers create only the minimum absent authorities and remain development-only by default.

### 7.4 Data and persistence

- Project-owned configuration is not overwritten by package updates.
- Mutable state is not stored in shared ScriptableObjects.
- EchoSettings owns global preferences; EchoSave owns game-save files/slots.
- Stable IDs, migration ownership, and report/schema versions are compatible.

### 7.5 Presentation and accessibility

- EchoUI is never required for a non-UI package to explain a blocking failure.
- Package-local minimal presenters do not become general UI authorities.
- Input, audio, motion, timing, and text accessibility hooks have clear owners.

### 7.6 Test Labs and release

- Every scene-visible package has a standalone lab independent of other Echo packages.
- Integration Labs declare both packages explicitly.
- Setup and repair are repeatable and non-destructive.
- Documentation, test, clean-install, removal, upgrade, and tarball gates do not contradict one another.

---

## 8. FW-DOC-12 Exit Criteria

The documentation gate passes only when:

- [x] Ten package specifications are Approved.
- [x] No release-blocking question remains that changes another Foundation package’s MVP or authority.
- [x] The cross-package ownership matrix has no duplicate authority.
- [x] All core dependencies and optional bridges are explicit.
- [x] Each package has an isolated Test Lab plan.
- [x] Direct-scene and duplicate-root policies are coherent.
- [x] Settings/save ownership and serialization boundaries are coherent.
- [x] The Workshop can describe every selectable package without inventing missing contracts; SFGSS-ADR-001 defines the facade boundary.
- [x] SFGSS-000, Current Notes, roadmap, and package statuses agree in the generated checkpoint.
- [x] Documentation checkpoint content is reconciled and ready for commit/push.
- [x] The first implementation checkpoint is selected and written as a Checkpoint Build Plan.
- [ ] FW-DOC-12 commit and push are confirmed by the user.

The expected first implementation checkpoint after this gate is **First Light M1 — Package Skeleton**, unless the consistency review records a reason to revise that order.

---

## 9. Historical Status at August 3, 2026

| Metric | Current value |
|---|---|
| Foundation specifications required | 10 |
| Approved | 10 |
| In drafting | 0 |
| Remaining | 0 |
| Completed documentation checkpoint | FW-DOC-12 — Documentation Readiness Gate |
| Active implementation checkpoint | FL-M1-01 — First Light Package Skeleton |
| Package implementation | First Light authorized; not started |
| Runtime behavior authorization | None; FL-M1-01 authorizes skeleton files only, while later packages require their own just-in-time learning review |
| Known blockers | None; live Unity, Git, package-path, and uGUI checks remain before file creation |
| Commit/push | SUITE-DOC-33 pending user confirmation |


---

## 10. FW-DOC-11 Result

**Decision:** Approved.

The Foundation Cross-Package Contract Matrix found two blocking documentation collisions and resolved both before implementation:

1. EchoGameState and EchoGameStarter shared `EGS-*`; Pulse v1.1.0 now uses `EGSTATE-*`.
2. The Workshop lacked exact peer setup endpoints; SFGSS-ADR-001 now defines the package-owned Editor setup facade protocol.

No duplicate runtime authority, circular core dependency, settings/save ownership conflict, Test Lab contradiction, or removal blocker remains. Proceed to FW-DOC-12. Runtime implementation is still locked.


---

## 11. FW-DOC-12 Result

**Decision:** Approved.

The Documentation Readiness Gate verified the ten package specifications, SFGSS-ADR-001, SFGSS-INT-FOUNDATION-001, repository status pages, identifier corrections, and implementation handoff. It found and resolved two documentation-readiness blockers:

1. SFGSS-005 was referenced but absent; v1.0.0 is now approved.
2. First Light still pointed to the completed documentation gate; v1.1.0 now records FL-M1-01.

The first authorized implementation checkpoint is:

```text
FL-M1-01 — First Light Package Skeleton
```

Authorization is limited to package manifest, asmdefs, documentation shell, compile/import validation, and closeout evidence. No C# implementation or launch behavior is authorized.


---

## 12. Subsequent full-suite rebaseline

SFGSS-ADR-002 preserves this roadmap as the completed Foundation documentation record but supersedes its immediate implementation activation.

- SUITE-DOC-33 has activated FL-M1-01.
- The active roadmap is `Full_Suite_Documentation_Program_Roadmap.md`.
- The active checkpoint is FL-M1-01.
- FL-M1-01 may create only its package skeleton. No C# or launch behavior is authorized. Later packages remain individually locked until their learning review passes.


---

## 13. Historical SUITE-DOC-33 Result

**Decision:** Passed with advisory.

PKG-LEARN-001 satisfies the First Light learning gate and FL-M1-01 v1.3.0 is active. The checkpoint must begin by verifying the live Unity compile, Git state, package path, and exact uGUI version. No C# or launch behavior is authorized.
