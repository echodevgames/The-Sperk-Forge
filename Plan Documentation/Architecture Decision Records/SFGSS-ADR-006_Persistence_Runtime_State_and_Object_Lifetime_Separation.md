---
tags:
  - sfgss/adr
  - sfgss/persistence
  - sfgss/lifecycle
  - sfgss/architecture
status: accepted
updated: 2026-08-09
---

# SFGSS-ADR-006 — Persistence, Runtime State, and Object Lifetime Separation

**Document ID:** SFGSS-ADR-006
**ADR version:** 1.0.0
**Status:** Accepted
**Decision date:** 2026-08-09
**Last reviewed:** 2026-08-09
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Decision scope:** Suite
**Evidence maturity:** Design approved; Chronicle learning/implementation pending
**Parent authorities:** SFGSS-000 v0.26.0; SFGSS-001 v1.5.0; SFGSS-002; SFGSS-003
**Affected documents:** SFGSS-000; SFGSS-001; SFGSS-INT-SUITE-001; The Chronicle specification; Package Learning Review Catalog; Suite Graph Roadmap; Current Notes
**Supersedes:** None
**Superseded by:** None
**Review triggers:** Chronicle implementation; Workshop long-lived service composition; Game Shell integration; proposal for a universal service locator or persistent-root package
**Related evidence:** Architectural decision only; runtime evidence `Not run`

> Durable persistence, mutable runtime truth, and Unity object lifetime are separate concerns, and no one package becomes the universal owner merely because several systems need to survive scenes or be saved.

---

## 1. Context and problem

The Game Shell / Front Door initiative places The Chronicle, The Accord, Resonance, and The Looking Glass immediately after First Light. Settings create the first unavoidable persistence question: audio and graphics values should survive process restart, while audio/UI services may also need to survive scene transitions.

Those requirements can easily be conflated into one oversized persistent `GameManager`, one mandatory `DontDestroyOnLoad` root, or one save service that becomes the live owner of every package's state.

The suite already assigns distinct authorities:

- The Chronicle owns durable **game-save transport** such as slots, generations, manifests, migration, integrity, backup, recovery, and save/load orchestration.
- The Accord owns global preferences.
- The Pulse owns high-level runtime state policy.
- Gameplay packages own their own mutable domain truth.
- First Light owns startup coordination and handoff.

This ADR makes the lifetime boundary explicit before Chronicle implementation begins.

### 1.1 Known facts

- Core packages are required to remain independently useful unless explicitly classified as bridges/providers/composers.
- Optional package connections belong in bridges or project adapters.
- Chronicle's approved contract already refuses global preferences, project gameplay schemas, scene-flow ownership, and production UI.
- Many package authorities are expected to live for an application session, but application-session lifetime is not durable persistence.
- Future packages such as Inventory, Objectives, Progression, Characters, and World will contain state that may need to be saved without surrendering ownership of that state to Chronicle.

### 1.2 Assumptions and evidence gaps

- Exact Chronicle root implementation and `DontDestroyOnLoad` mechanics remain implementation work.
- Exact serializer/file-format/backend choices are not decided by this ADR.
- No universal project composition root has been implemented or empirically proven.
- Cross-package persistence bridges are not implemented.

---

## 2. Decision drivers and constraints

- Preserve package independence.
- Keep one authoritative owner per concern.
- Prevent First Light from becoming a permanent application `GameManager`.
- Prevent Chronicle from becoming a global service locator or runtime database for unrelated systems.
- Allow save-capable packages to work when Chronicle is absent.
- Allow consumer projects to compose long-lived services without transferring package authority.
- Keep persistence integration optional, removable, and testable.
- Preserve future flexibility for Accord preferences, Chronicle save slots, and package-owned gameplay payloads.

---

## 3. Options considered

### Option A — Chronicle owns the universal persistent service root

**Description:** All long-lived services are children or registrations of EchoSave and use it as their shared lifetime/service access point.

**Advantages:**

- One obvious object appears to survive scenes.
- Initial scene setup may look simple.

**Costs/risks:**

- Makes save infrastructure a mandatory dependency for unrelated systems.
- Confuses durable persistence with scene-surviving lifetime.
- Turns Chronicle into a service locator and hidden shared core.
- Makes package removal and standalone testing harder.
- Gives Chronicle authority it does not semantically own.

### Option B — First Light remains the permanent service root

**Description:** EchoLaunchRoot survives handoff and owns/locates all long-lived services.

**Advantages:**

- Startup already knows initialization order.
- One object can appear to bootstrap everything.

**Costs/risks:**

- Violates First Light's narrow startup/handoff authority.
- Reopens a package that has deliberately graduated with a bounded contract.
- Couples unrelated package lifetimes to launch implementation.

### Option C — Create a mandatory generic persistent-root package now

**Description:** Introduce a new suite package solely to host/locate all long-lived services.

**Advantages:**

- Avoids assigning the role specifically to Chronicle or First Light.

**Costs/risks:**

- Creates a mandatory shared core without executed evidence.
- Risks becoming the same service-locator/God-object problem under a neutral name.
- Adds another package before a real repeated contract proves it is needed.

### Option D — Separate concerns and keep composition project-owned

**Description:** Each package owns its own runtime truth and package-local lifecycle. Durable transport remains with the proper persistence authority. Optional adapters connect save-capable packages to Chronicle. The consumer project may compose scene-surviving services beneath a project-owned root when useful.

**Advantages:**

- Preserves authority boundaries and standalone packages.
- Allows `DontDestroyOnLoad` composition without equating it with saving.
- Makes Chronicle optional for packages that can run in memory or use another persistence backend.
- Supports future replacement/removal and isolated Laboratories.
- Keeps First Light free to hand off cleanly.

**Costs/risks:**

- Requires explicit adapters and project composition.
- More than one long-lived service/root may exist.
- Projects must understand which package owns each live truth.

---

## 4. Decision

**Option D is accepted.**

The suite adopts the following rule:

> **Durable persistence, runtime state, and Unity object lifetime are separate concerns. Packages may expose persistence-capable state without depending directly on EchoSave. Cross-package persistence integration belongs in optional bridges/adapters. Long-lived Unity service composition is project-owned and must not turn First Light, The Chronicle, or another package into a universal service locator.**

More specifically:

1. **Durable game-save transport:** Chronicle owns save files/slots/generations, manifests, migration/recovery, and save/load orchestration.
2. **Global preferences:** Accord remains authoritative for preferences such as audio, graphics, accessibility, controls, and locale.
3. **Runtime truth:** A participant/package remains authoritative for its live state before and after Chronicle capture/load.
4. **Persistence-capable packages:** Core packages may expose detached/versioned snapshot or import/export contracts without referencing EchoSave.
5. **Cross-package save integration:** A bridge/participant adapter/project adapter may reference both Chronicle and the participant package.
6. **Unity object lifetime:** A package may define a duplicate-safe scene-surviving root for its own authority. That lifetime does not make it the owner of unrelated services.
7. **Project composition:** The consumer project may compose long-lived package services beneath a project-owned `DontDestroyOnLoad` object when appropriate. Parentage does not transfer authority.
8. **First Light:** First Light may initialize or discover services during startup and then hands off. It is not the permanent application root.
9. **Chronicle:** Chronicle may own `EchoSaveRoot` for Chronicle operations. It is not a global service locator, generic `GameManager`, peer registry, or parent authority.
10. **No new mandatory root package:** The suite will not add one without later executed evidence and a new/superseding ADR.

---

## 5. Rationale

This choice keeps the suite modular while still allowing a real game to have long-lived systems and durable data.

The distinction also prevents a common architectural trap: an object can survive a scene change without being saved, and data can be saved without the save system becoming its live runtime owner. Treating these as separate axes gives future systems a clean way to participate in persistence while preserving their own domain boundaries.

---

## 6. Consequences

### 6.1 Positive

- Accord can persist preferences without becoming a Chronicle client.
- Resonance can apply audio preferences while Accord remains the preference authority.
- Inventory/Progression/Objectives/Characters/World can expose save snapshots without core EchoSave dependencies.
- Chronicle can evolve its transport/storage implementation independently of participant runtime models.
- First Light remains a startup/handoff package.
- A project's scene-surviving service composition remains explicit and replaceable.

### 6.2 Costs and risks

- Bridges/adapters add visible integration artifacts.
- Projects may need to compose multiple package-local roots.
- Documentation and Laboratories must test ownership and removal carefully.
- A future repeated composition pattern might eventually justify a shared project-composition artifact, but not yet.

### 6.3 Deferred consequences

- Exact Chronicle storage/serializer implementation.
- Exact Accord preference storage format.
- Concrete Game Shell integration bridges.
- Whether Workshop later generates an optional project-owned runtime composition root.
- Whether a repeated neutral composition contract eventually justifies a new package.

---

## 7. Authority and document impact

| Document/artifact | Required action | Status |
|---|---|---|
| SFGSS-000 | Add suite law and persistence-scope clarification | Updated to v0.26.0 |
| SFGSS-001 | Require three-lifetime analysis in package specs | Updated to v1.5.0 |
| Chronicle specification | Clarify package-local root and participant/runtime separation | Updated to v1.2.0 |
| Full Suite Matrix | Clarify application-session root terminology | Updated to v1.1.0 |
| Current Notes | Activate Chronicle learning and record Game Shell initiative | Updated |
| Learning catalog/tracker | Activate PKG-LEARN-009 | Updated |
| Tests/research | Implementation evidence deferred | Not run |

---

## 8. Implementation and migration impact

- **Implementation state:** Not started for Chronicle.
- **Public API impact:** None yet; future APIs must comply.
- **Serialized data impact:** None.
- **Migration/upgrade impact:** None.
- **Removal/reinstall impact:** Improves future removal by keeping integrations optional.
- **Workshop/setup impact:** Workshop may later compose project-owned long-lived roots, but cannot introduce hidden ownership or mandatory peer dependencies.

---

## 9. Evidence and validation plan

| Evidence | Required result | Current status |
|---|---|---|
| Learning review | Jesse can explain the three separate lifetime/persistence concerns and ownership | In progress |
| Chronicle skeleton tests | Duplicate-safe EchoSave authority with zero peer-service ownership | Not run |
| Standalone Laboratory | Chronicle works without Accord/Resonance/UI/First Light | Not run |
| Integration bridge tests | Peer snapshot transport works without authority transfer | Not run |
| Game Shell showcase | Settings/audio/UI/save composition survives scene transition and process restart under correct owners | Not run |

---

## 10. Security, privacy, licensing, cost, and provider impact

This ADR chooses no storage vendor, serializer, cloud service, encryption system, or third-party dependency. Security/privacy rules remain owned by Chronicle/package/provider authorities. Project-owned service composition introduces no new licensing cost.

---

## 11. Removal, reversal, and supersession plan

Removing a bridge must leave both core packages independently usable. Removing Chronicle must not prevent a peer from running in memory or using another project-owned persistence path where its package contract allows one. Project-owned composition objects may be replaced without changing package ownership.

A future proposal for a universal service root, mandatory shared composition package, or Chronicle-owned peer registry requires a new ADR that explicitly supersedes this decision.

---

## 12. Review triggers

- ESV-M1-01 implementation reveals a real need for project-wide service discovery.
- Workshop begins generating long-lived service composition.
- The Game Shell showcase composes Chronicle, Accord, Resonance, and Looking Glass.
- Three or more packages repeat an identical composition contract that cannot remain project-owned.
- A proposal introduces a universal singleton/service locator/persistent-root package.
- A peer package would require a hard EchoSave dependency solely to persist state.

---

## 13. Approval record

**Decision:** ACCEPTED
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** 2026-08-09
**Conditions:** Chronicle implementation remains locked behind PKG-LEARN-009 and explicit ESV-M1-01 activation.

---

## 14. Graph Navigation

- [[../Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[SFGSS-ADR-LOG_Suite_Decision_Log|Suite Decision Log]]
- [[../Current Notes|Current Notes]]
- [[../Package Specifications/SFGSS-The-Chronicle-EchoSave-Package-Specification|The Chronicle (`EchoSave`)]]
- [[../Integration Specifications/SFGSS-INT-SUITE-001_Full_Suite_Authority_Dependency_Bridge_and_Persistence_Matrix|Full Suite Matrix]]
