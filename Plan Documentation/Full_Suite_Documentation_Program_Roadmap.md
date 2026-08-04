# The Sperk’s Forge — Full Suite Documentation Program Roadmap

**Document role:** Level 4 planning and checkpoint record  
**Status:** Active; implementation locked  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Parent authority:** SFGSS-000 v0.9.0, SFGSS-001 v1.1.0, SFGSS-ADR-002  
**Workflow authority:** SFGSS-005 v1.1.0  
**Unity baseline:** Unity 6000.3.8f1  
**Last updated:** August 3, 2026

> Complete the blueprint library before the first crate is opened.

---

## 1. Purpose

This roadmap extends the successful Foundation documentation pass across the complete planned documentation program in SFGSS-000 Section 18.

The goal is not to predict every implementation detail perfectly. The goal is to approve every decision that can be responsibly made before code, reveal cross-package collisions while they are inexpensive, and prepare acceptance contracts that teach and guide implementation later.

## 2. Gate rule

No package manifest, assembly definition, C# script, Unity scene, prefab, ScriptableObject, setup tool, sample, bridge, provider adapter, or runtime implementation may begin until **SUITE-DOC-36 — Full Suite Documentation Readiness Gate** passes.

FL-M1-01 remains approved but dormant.

## 3. Honest completeness boundary

### 3.1 Required before code

- Suite architecture/workflow standards.
- Package ownership, non-goals, public API designs, data models, lifecycle, failure behavior, diagnostics, setup designs, Test Labs, and release gates.
- Stable-ID, serialization, migration, bridge, assembly, repository, versioning, and documentation rules.
- Expansion specifications and Advanced feasibility/design records.
- Source-based research and provider approval criteria.
- Cross-package matrices and final collision review.
- Templates for future implementation evidence.

### 3.2 Evidence that remains pending

- Compile and automated-test output.
- Manual test results and screenshots.
- Measured performance, allocations, and platform behavior.
- Final verified Unity/package compatibility tables.
- Migration results from actual shipped versions.
- Release notes for implemented behavior.
- Multiplayer prototype findings and final provider approval.

These items may have prepared templates and explicit `Not run` states, but they are not marked complete without evidence.

## 4. Completed baseline

| Area | Status |
|---|---|
| SFGSS-000 | Approved v0.9.0 after rebaseline |
| SFGSS-001 | Approved v1.1.0 |
| SFGSS-005 | Approved v1.1.0 |
| Foundation package specifications | 10 of 10 Approved |
| Foundation cross-package matrix | Approved |
| Foundation readiness gate | Passed historically |
| Implementation | Not started; re-locked by ADR-002 |

## 5. Phase A — Remaining architecture and workflow standards

| Checkpoint | Deliverable | Outcome |
|---|---|---|
| SUITE-DOC-01 | Full Suite Documentation Rebaseline | Gate, roadmap, learning workflow, and dormant FL-M1 status approved |
| **SUITE-DOC-02** | **SFGSS-002 — Dependency, Bridge, and Assembly Standard** | Canonical package/asmdef dependency direction, optional bridges, provider adapters, compile guards, removal, and test assembly rules |
| SUITE-DOC-03 | SFGSS-003 — Data, IDs, Serialization, and Migration Standard | Stable IDs, aliases, DTOs, schema versions, migrations, unknown-data preservation, transactions, and asset-state safety |
| SUITE-DOC-04 | SFGSS-004 — Testing, Validation, Test Labs, and Release Standard | Test taxonomy, evidence states, Lab rules, clean install, tarballs, repeatability, compatibility, and release gates |
| SUITE-DOC-05 | SFGSS-006 — New-Project Guided Pathways | Package-selection pathways, starter combinations, decision trees, and Workshop-facing guidance |
| SUITE-DOC-06 | SFGSS-007 — ADR Template and Decision Log | Reusable ADR format, status lifecycle, indexing, supersession, and current decision register |
| SUITE-DOC-07 | SFGSS-008 — Suite Glossary and Naming Registry | Public titles, technical IDs, namespaces, diagnostic prefixes, stable terminology, and reserved names |
| SUITE-DOC-08 | SFGSS-009 — Repository, Versioning, and Integration Workspace Standard | Multi-repo workflow, tags, releases, package versions, compatibility catalog, local development, and distribution |
| SUITE-DOC-09 | SFGSS-010 — Living Documentation, Current Notes, and Obsidian Workflow Standard | Vault structure, links, note promotion, archive policy, handoff, and documentation commit rules |
| SUITE-DOC-10 | Standards Consistency Review | Reconcile SFGSS-002–010 with Foundation specs, ADRs, matrix, and template |

## 6. Phase B — Expansion package specifications

Each specification follows all 30 SFGSS-001 sections and remains independent unless explicitly classified as a bridge or Editor composer.

| Checkpoint | Package specification | Primary design focus |
|---|---|---|
| SUITE-DOC-11 | EchoProgression — The Ascent | Unlocks, passwords, checkpoints, completion records, storage boundary |
| SUITE-DOC-12 | EchoBuildTools — The Foundry | Build profiles, preflight, version stamping, reports, safe output |
| SUITE-DOC-13 | EchoFeedback — Impact | Feedback recipes, channel requests, accessibility scaling, cancellation |
| SUITE-DOC-14 | EchoPool — The Wellspring | Pool definitions, lifecycle, exhaustion, scene ownership, diagnostics |
| SUITE-DOC-15 | EchoInteraction — The Hand | Detection, focus, prompt data, availability, execution contracts |
| SUITE-DOC-16 | EchoCharacters — The Fellowship | Identity, roster, selection, spawn, switching, possession |
| SUITE-DOC-17 | EchoControllers — The Vessel and Controller Preset Template | Intent/motor contracts, capability modules, physics boundaries, independent Labs |
| SUITE-DOC-18 | EchoInventory — The Vault | Definitions, instances, containers, transactions, equipment boundary |
| SUITE-DOC-19 | EchoObjectives — The Path | Objective graphs, progress, rewards-as-requests, persistence |
| SUITE-DOC-20 | EchoDialogue — Voices | Speakers, sequences, choices, conditions, commands, save boundary |
| SUITE-DOC-21 | EchoCamera — The Eye | Targets, modes, requests, bounds, backend adapters |
| SUITE-DOC-22 | EchoLocalization — Many Tongues | Localized references, locales, fallback, fonts, pseudolocalization |
| SUITE-DOC-23 | Expansion Cross-Package Collision Review | Authority, IDs, persistence, UI/input/audio bridges, Labs, removal |

## 7. Phase C — Advanced design and research

| Checkpoint | Deliverable | Honest pre-code completion |
|---|---|---|
| SUITE-DOC-24 | EchoCrafting Design Workshop Record | Resolve recipes, skills, discovery, quality, stations, queues, failure, repair, salvage, UI, persistence, multiplayer seams |
| SUITE-DOC-25 | EchoCrafting — The Crucible Specification | Approved package contract and smallest complete vertical slice |
| SUITE-DOC-26 | EchoMultiplayer Research Plan | Provider-neutral questions, prototype protocol, authority/security criteria, cost/licensing research plan |
| SUITE-DOC-27 | EchoMultiplayer Source-Based Provider Matrix | Dated source research, shortlist, risks, and explicit unknowns; no fabricated prototype evidence |
| SUITE-DOC-28 | EchoMultiplayer Neutral Contracts and Adapter Strategy | Session/player/authority contracts, bridge boundaries, adapter packaging; final provider remains conditional until prototypes |
| SUITE-DOC-29 | EchoAI — Instinct Feasibility Specification | Sensors, memory, scoring, behavior seams, navigation adapters |
| SUITE-DOC-30 | EchoCombat — Clash Feasibility Specification | Damage/healing messages, teams, targeting, resolution seams |
| SUITE-DOC-31 | EchoAbilities — Arcana Feasibility Specification | Activation, costs, cooldowns, targeting, effect execution |
| SUITE-DOC-32 | EchoWorld — The Atlas Feasibility Specification | World/zone IDs, travel metadata, spawn markers, world-state seams |
| SUITE-DOC-33 | Advanced Collision and Research Review | Cross-check Crafting, Multiplayer, AI, Combat, Abilities, World with the suite |

## 8. Phase D — Final suite reconciliation

| Checkpoint | Deliverable | Exit condition |
|---|---|---|
| SUITE-DOC-34 | Full Suite Authority, Dependency, and Persistence Matrix | One owner per concern; no circular core dependency; bridge/removal rules explicit |
| SUITE-DOC-35 | Full Suite Documentation and Learning Handoff Audit | README, Current Notes, standards, specs, ADRs, research records, templates, and educational workflow agree |
| **SUITE-DOC-36** | **Full Suite Documentation Readiness Gate** | Explicitly authorize or revise the first implementation checkpoint |

## 9. Out-of-scope additions

The roadmap does not automatically add packages or standards beyond SFGSS-000 Section 18. `EchoRPG.Foundation`, provider-specific multiplayer adapters, cloud services, hardware-sensor providers, and controller-family splits require their own promotion decision before joining this gate.

## 10. Current status

| Field | Value |
|---|---|
| Completed checkpoint | SUITE-DOC-01 — Full Suite Documentation Rebaseline |
| Active checkpoint | **SUITE-DOC-02 — SFGSS-002 Dependency, Bridge, and Assembly Standard** |
| Foundation specifications | 10 of 10 Approved |
| Remaining standards checkpoints | 9 including consistency review |
| Expansion specifications/review | 13 checkpoints |
| Advanced design/research/review | 10 checkpoints |
| Final reconciliation/readiness | 3 checkpoints |
| Package implementation | Not started |
| First queued implementation | FL-M1-01 — First Light Package Skeleton |
| Runtime authorization | None |

## 11. Checkpoint closeout rule

Every SUITE-DOC checkpoint must:

1. Reconcile Current Notes.
2. Promote durable decisions into SFGSS-000, a standard, a package specification, an ADR, or an integration record.
3. Update this roadmap and README.
4. Validate links, document IDs, statuses, versions, and authority order.
5. Commit and push the documentation checkpoint before beginning the next one when practical.
