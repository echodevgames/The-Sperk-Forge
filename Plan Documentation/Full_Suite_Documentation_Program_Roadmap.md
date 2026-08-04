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
| Expansion package specifications | 5 of 13 approved; Impact, The Wellspring, The Ascent, The Foundry, and Many Tongues v1.0.0 |
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
| SUITE-DOC-11 | EchoObjectives - The Path | Objectives, quests, tasks, progress graphs, conditions, rewards-as-requests, persistence, and tracked presentation data |
| SUITE-DOC-12 | EchoInventory - The Vault | Item definitions/instances, containers, stacks, transactions, capacity, transfer, equipment-storage boundary, and persistence |
| SUITE-DOC-13 | EchoInteraction - The Hand | Detection, focus, range, priority, prompt data, availability, execution requests, cancellation, and 2D/3D adapters |
| SUITE-DOC-14 | EchoCamera - The Eye | Targets, modes, requests, bounds, blends, zones, backend adapters, and camera/feedback/dialogue boundaries |
| SUITE-DOC-15 | EchoCharacters - The Fellowship | Identity, roster, selection, spawning, switching, control ownership, possession, save snapshots, and multiplayer seams |
| SUITE-DOC-16 | EchoControllers - The Vessel | Intent/motor contracts, controller families, capability modules, physics boundaries, adapters, and independent preset Laboratories |
| SUITE-DOC-17 | EchoCrafting - The Crucible | Dedicated design-workshop record plus recipes, requirements, stations, queues, transactions, repair/salvage boundaries, UI, persistence, and multiplayer seams |

## 6. Phase C - Advanced and adapter package foundations

Advanced documents remain honest about research and provider uncertainty. “Foundation complete” means the package boundary and pre-code contracts are prepared, not that unsupported empirical claims are invented.

| Checkpoint | Package foundation | Responsible pre-code outcome |
|---|---|---|
| SUITE-DOC-18 | EchoMultiplayer - The Convergence | Research plan, dated provider matrix, disposable prototype protocol, provider-neutral session/player/authority contracts, security rules, adapter packaging, and explicit unknowns |
| SUITE-DOC-19 | EchoAI - Instinct | Sensing, stimuli, perception memory, scoring, behavior seams, navigation adapters, debugging, and explicit non-goals |
| SUITE-DOC-20 | EchoCombat - Clash | Damage/healing messages, teams, targets, resolution contracts, hit adapters, combat events, and genre-neutral boundaries |
| SUITE-DOC-21 | EchoAbilities - Arcana | Ability definitions/instances, activation, costs, charges, cooldowns, casting, interruption, targeting, effects, save, and multiplayer seams |
| SUITE-DOC-22 | EchoWorld - The Atlas | World/zone/location IDs, travel metadata, spawn markers, scene mapping, world-state contracts, discovery, and map/provider seams |

## 7. Phase D - Package collision reviews

| Checkpoint | Deliverable | Exit condition |
|---|---|---|
| SUITE-DOC-23 | Expansion Cross-Package Collision Review | One authority per concern; IDs, persistence, UI/input/audio, Laboratories, bridges, and removal agree across 7.2 |
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
| Completed checkpoint | SUITE-DOC-10 - Voices (`EchoDialogue`) Package Specification |
| Clarification checkpoint | Package Specification Priority Rebaseline - approved August 4, 2026 |
| Active checkpoint | **SUITE-DOC-11 - The Path (`EchoObjectives`) Package Specification** |
| Foundation specifications | 10 of 10 approved |
| Expansion specifications | 6 of 13 approved |
| Advanced package foundations | 0 of 5 approved |
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
