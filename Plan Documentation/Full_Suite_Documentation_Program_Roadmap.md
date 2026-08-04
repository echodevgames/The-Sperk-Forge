# The Sperk’s Forge - Full Suite Documentation Program Roadmap

**Document role:** Level 4 planning and checkpoint record  
**Status:** Active; package implementation locked  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, SFGSS-ADR-002  
**Workflow authority:** SFGSS-005 v1.1.0  
**Unity baseline:** Unity 6000.3.8f1  
**Last updated:** August 4, 2026

> Finish the package blueprints first. Use the standards as rails beneath them, not as a tunnel before them.

---

## 1. Purpose

This roadmap completes the package foundations named in SFGSS-000 Sections 7.2 and 7.3 before implementation begins.

The owner’s intended meaning of “continue until all documentation is ready” is now explicit:

1. Complete and approve every Expansion package specification.
2. Complete the responsible pre-code foundation for every Advanced package.
3. Run Expansion, Advanced, and full-suite collision reviews.
4. Finish the remaining suite standards and handoff documents using the completed package set as evidence.
5. Pass one final readiness gate before any package implementation begins.

SFGSS-002, SFGSS-003, and SFGSS-004 remain approved and immediately useful. They guide dependencies, assemblies, data, IDs, serialization, migration, testing, Laboratories, and release evidence for every package specification below.

## 2. Gate rule

No package manifest, assembly definition, C# script, Unity scene, prefab, ScriptableObject, setup tool, sample, bridge, provider adapter, or runtime implementation may begin until the final full-suite documentation readiness gate passes.

FL-M1-01 remains approved but dormant.

## 3. Honest completeness boundary

### 3.1 Required before code

- All Foundation, Expansion, and Advanced package foundations named in SFGSS-000 Sections 7.1–7.3.
- Ownership, non-goals, public API design, data model, lifecycle, failure behavior, diagnostics, setup design, Test Lab, integration seams, removal behavior, and release gates for each package.
- Crafting design-workshop conclusions before its package contract is approved.
- Multiplayer provider-neutral contracts, research plan, dated source matrix, prototype protocol, and explicit unknowns.
- Cross-package collision reviews and final suite matrices.
- Remaining standards and workflow documents needed to keep the complete package library coherent.

### 3.2 Evidence that remains pending

- Compile and automated-test output.
- Manual test results and screenshots.
- Measured performance and allocations.
- Verified platform and Unity/package compatibility.
- Migration evidence from shipped versions.
- Multiplayer prototype results and final provider approval.

These remain visibly `Not run` or conditional until implementation or research execution produces evidence.

## 4. Completed baseline

| Area | Status |
|---|---|
| SFGSS-000 | Approved v0.12.0 |
| SFGSS-001 | Approved v1.1.0 |
| SFGSS-002 | Approved v1.0.0 |
| SFGSS-003 | Approved v1.0.0 |
| SFGSS-004 | Approved v1.0.0 |
| SFGSS-005 | Approved v1.1.0 |
| Foundation package specifications | 10 of 10 approved |
| Expansion package specifications | 13 of 13 approved; Impact, The Wellspring, The Ascent, The Foundry, Many Tongues, Voices, The Path, The Vault, The Hand, The Eye, The Fellowship, The Vessel, and The Crucible v1.0.0 |
| Foundation cross-package matrix | Approved |
| Foundation readiness gate | Passed historically; implementation re-locked by ADR-002 |
| Package implementation | Not started |

## 5. Phase B - Expansion package specifications

Each package receives a complete SFGSS-001 specification. The order follows SFGSS-000 Section 7.2 and the owner’s clarified priority.

| Checkpoint | Package specification | Core design focus |
|---|---|---|
| SUITE-DOC-05 | EchoFeedback - Impact - **Approved v1.0.0** | Coordinated camera shake, hit stop, rumble, flashes, response recipes, cancellation, accessibility scaling, and bridge boundaries |
| SUITE-DOC-06 | EchoPool - The Wellspring - **Approved v1.0.0** | General-purpose pooling, definitions, ownership, lifecycle, capacity, exhaustion, return safety, scene transitions, and diagnostics |
| SUITE-DOC-07 | EchoProgression - The Ascent - **Approved v1.0.0** | Unlocks, passwords, checkpoints, level access, completion records, storage boundary, and UI/save bridges |
| SUITE-DOC-08 | EchoBuildTools - The Foundry - **Approved v1.0.0** | Build Profiles, recipes, versioning, preflight, scene validation, safe output, reports, checksums, and release preparation |
| SUITE-DOC-09 | EchoLocalization - Many Tongues - **Approved v1.0.0** | Locale tables, localized references, fallbacks, fonts, formatting, pseudolocalization, and UI/dialogue boundaries |
| SUITE-DOC-10 | EchoDialogue - Voices - **Approved v1.0.0** | Speakers, lines, sequences, branching, choices, conditions, commands, history, localization, and save boundary |
| SUITE-DOC-11 | EchoObjectives - The Path - **Approved v1.0.0** | Objectives, quests, tasks, progress graphs, conditions, rewards-as-requests, persistence, and tracked presentation data |
| SUITE-DOC-12 | EchoInventory - The Vault - **Approved v1.0.0** | Item definitions/instances, containers, stacks, transactions, capacity, transfer, equipment-storage boundary, and persistence |
| SUITE-DOC-13 | EchoInteraction - The Hand - **Approved v1.0.0** | Detection, focus, range, priority, prompt data, availability, execution requests, cancellation, and 2D/3D adapters |
| SUITE-DOC-14 | EchoCamera - The Eye - **Approved v1.0.0** | Targets, modes, requests, bounds, blends, zones, backend adapters, and camera/feedback/dialogue boundaries |
| SUITE-DOC-15 | EchoCharacters - The Fellowship - **Approved v1.0.0** | Identity, roster, selection, spawning, switching, control ownership, possession, save snapshots, and multiplayer seams |
| SUITE-DOC-16 | EchoControllers - The Vessel - **Approved v1.0.0** | Intent/motor contracts, controller families, capability modules, physics boundaries, adapters, and independent preset Laboratories |
| SUITE-DOC-17 | EchoCrafting - The Crucible - **Approved v1.0.0** | Dedicated design-workshop record plus recipes, requirements, stations, queues, transactions, repair/salvage boundaries, UI, persistence, and multiplayer seams |

## 6. Phase C - Advanced and adapter package foundations

Advanced documents remain honest about research and provider uncertainty. “Foundation complete” means the package boundary and pre-code contracts are prepared, not that unsupported empirical claims are invented.

| Checkpoint | Package foundation | Responsible pre-code outcome |
|---|---|---|
| SUITE-DOC-18 | EchoMultiplayer - The Convergence - **Approved provider-neutral foundation v1.0.0; provider selection pending** | Research plan, dated provider matrix, disposable prototype protocol, provider-neutral session/player/authority contracts, security rules, adapter packaging, and explicit unknowns |
| SUITE-DOC-19 | EchoAI - Instinct - **Approved feasibility foundation v1.0.0; implementation/adapters pending** | Sensing, stimuli, perception memory, scoring, typed context, scheduling, behavior seams, navigation adapters, debugging, and explicit non-goals |
| SUITE-DOC-20 | EchoCombat - Clash - **Approved feasibility foundation v1.0.0; implementation/adapters pending** | Damage/healing messages, teams, targets, resolution contracts, hit adapters, combat events, and genre-neutral boundaries |
| SUITE-DOC-21 | EchoAbilities - Arcana - **Approved feasibility foundation v1.0.0; implementation/adapters pending** | Ability definitions/instances, activation, costs, charges, cooldowns, casting, interruption, targeting, effects, save, and multiplayer seams |
| SUITE-DOC-22 | EchoWorld - The Atlas - **Approved feasibility foundation v1.0.0; implementation/adapters pending** | World/zone/location IDs, travel metadata, spawn markers, scene mapping, world-state contracts, discovery, and map/provider seams |

## 7. Phase D - Package collision reviews

| Checkpoint | Deliverable | Exit condition |
|---|---|---|
| SUITE-DOC-23 | Expansion Cross-Package Collision Review — **Approved** | SFGSS-INT-EXPANSION-001 approved; Ascent v1.1.0 and ADR-001 v1.1.0 repairs complete |
| SUITE-DOC-24 | Advanced Cross-Package and Research Review | Crafting, Multiplayer, AI, Combat, Abilities, and World fit Foundation/Expansion authorities without fabricated provider evidence |

## 8. Phase E - Remaining suite standards and guided documentation

These standards now follow the package foundations so they can describe the actual complete suite instead of guessing ahead of it.

| Checkpoint | Deliverable | Outcome |
|---|---|---|
| SUITE-DOC-25 | SFGSS-006 - New-Project Guided Pathways | Package-selection pathways and Workshop guidance built from the full approved package catalog |
| SUITE-DOC-26 | SFGSS-007 - ADR Template and Decision Log | ADR format, lifecycle, indexing, supersession, and current decision register |
| SUITE-DOC-27 | SFGSS-008 - Suite Glossary and Naming Registry | Public titles, technical IDs, namespaces, diagnostics, stable terminology, and reserved names |
| SUITE-DOC-28 | SFGSS-009 - Repository, Versioning, and Integration Workspace Standard | Multi-repo workflow, tags, releases, compatibility catalog, local development, and distribution |
| SUITE-DOC-29 | SFGSS-010 - Living Documentation, Current Notes, and Obsidian Workflow Standard | Vault structure, links, note promotion, archives, handoff, and documentation commits |
| SUITE-DOC-30 | Standards and Package Consistency Review | Reconcile SFGSS-002–010 with all Foundation, Expansion, and Advanced package authorities |

## 9. Phase F - Final suite reconciliation

| Checkpoint | Deliverable | Exit condition |
|---|---|---|
| SUITE-DOC-31 | Full Suite Authority, Dependency, Bridge, and Persistence Matrix | One owner per concern; no circular core dependency; installation/removal behavior explicit |
| SUITE-DOC-32 | Full Suite Documentation and Learning Handoff Audit | README, Current Notes, standards, package specs, ADRs, research, tests, and learning workflow agree |
| **SUITE-DOC-33** | **Full Suite Documentation Readiness Gate** | Explicitly authorize or revise the first implementation checkpoint |

## 10. Current status

| Field | Value |
|---|---|
| Completed checkpoint | SUITE-DOC-23 - Expansion Cross-Package Collision Review |
| Clarification checkpoint | Package Specification Priority Rebaseline - approved August 4, 2026 |
| Active checkpoint | **SUITE-DOC-24 - Advanced Cross-Package and Research Review** |
| Foundation specifications | 10 of 10 approved |
| Expansion specifications | 13 of 13 approved |
| Advanced package foundations | 5 of 5 approved foundations |
| Package implementation | Not started |
| First queued implementation | FL-M1-01 - First Light Package Skeleton |
| Runtime authorization | None |
| Final unlock gate | SUITE-DOC-33 |

## 11. Checkpoint closeout rule

Every checkpoint must:

1. Reconcile `Current Notes.md`.
2. Promote durable decisions into SFGSS-000, a standard, package specification, ADR, integration record, or research record.
3. Update this roadmap and README.
4. Validate links, IDs, statuses, versions, authorities, and evidence states.
5. Commit and push the documentation checkpoint before advancing when practical.


## SUITE-DOC-20 Closeout - Clash Approved

Clash (`EchoCombat`) Feasibility Foundation v1.0.0 is approved. The foundation defines provider-neutral requests, targetability, relations, deterministic pure modifiers, target-owned receiver transactions, outcomes, events, logs, hit-adapter seams, save/multiplayer boundaries, 84 Laboratory scenarios, and 540 planned tests. All empirical evidence remains `Not run`.

**Next checkpoint:** SUITE-DOC-21 - Arcana (`EchoAbilities`) Feasibility Foundation.


## SUITE-DOC-21 Closeout - Arcana Approved

Arcana (`EchoAbilities`) Feasibility Foundation v1.0.0 is approved. The foundation defines provider-neutral ability definitions, owner state, grants, loadouts, activation validation, one mutation-capable cost transaction, charges, cooldowns, cast/channel timing, interruption, targeting contracts, typed effect execution, persistence and multiplayer seams, 96 Laboratory scenarios, and 600 planned tests. All empirical evidence remains `Not run`.

**Next checkpoint:** SUITE-DOC-22 - The Atlas (`EchoWorld`) Feasibility Foundation.


## SUITE-DOC-22 Closeout - The Atlas Approved

The Atlas (`EchoWorld`) Feasibility Foundation v1.0.0 is approved. The foundation defines stable semantic world/zone/location identity, topology, current context, travel planning, scene-binding tokens, entry/spawn marker registries, discovery, visitation, fast-travel eligibility, map snapshots, world-state participant contracts, diagnostics, 104 Laboratory scenarios, and 624 planned tests. All empirical evidence remains `Not run`.

**Next checkpoint:** SUITE-DOC-23 - Expansion Cross-Package Collision Review.


## SUITE-DOC-23 Closeout - Expansion Collision Review Approved

SFGSS-INT-EXPANSION-001 v1.0.0 passes the thirteen-package authority, lifecycle, dependency, persistence, transaction, diagnostics, Laboratory, setup-facade, and removal review. The Ascent advances to v1.1.0 to distinguish progression-node completion from objective-run completion. ADR-001 advances to v1.1.0 with all thirteen Expansion setup facades. No implementation evidence was produced.

**Next checkpoint:** SUITE-DOC-24 - Advanced Cross-Package and Research Review.
