# The Sperk’s Forge - Current Notes

**Document role:** Living development capture page  
**Authority:** Working context only; SFGSS-000, approved package specifications, accepted ADRs, integration specifications, and approved Checkpoint Build Plans remain authoritative  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Last reconciled:** August 4, 2026  
**Current focus:** SFGSS-007 ADR Template and Decision Log
**Current checkpoint:** SUITE-DOC-26 - Define the suite ADR template, lifecycle, index, supersession rules, and current decision register

> Capture quickly here. Promote deliberately at checkpoint closeout.

---

## How to Use This Page

Use this page for information discovered while designing, implementing, testing, or reviewing the suite:

- `[NOTE]` - useful observation or context.
- `[QUESTION]` - unresolved question requiring research or approval.
- `[PROPOSAL]` - suggested change that is not yet authoritative.
- `[DECISION]` - approved decision awaiting or confirming documentation promotion.
- `[TEST]` - test result, reproduction, or validation evidence.
- `[BUG]` - defect or regression awaiting issue-log placement.
- `[RISK]` - dependency, compatibility, schedule, or architecture concern.
- `[HANDOFF]` - context the next work session must see.

Keep entries dated. Link to the affected specification, ADR, checkpoint, test, issue, guide, or source file whenever possible.

Do not leave durable decisions only on this page. At checkpoint closeout, promote each material entry into the document that owns it and record the destination below.

---

## Current Focus

### Goal

Complete every Expansion and Advanced package foundation in SFGSS-000 Sections 7.2 and 7.3 before package implementation begins, then finish the remaining standards and final reconciliation. Preserve honest `Not run` states for evidence that requires implementation.

### Active source documents

- `Echo_Game_Systems_Suite_Bible.md` - SFGSS-000 v0.15.0.
- `SFGSS-002_Dependency_Bridge_and_Assembly_Standard.md` - v1.0.0 Approved.
- `SFGSS-003_Data_IDs_Serialization_and_Migration_Standard.md` - v1.0.0 Approved.
- `SFGSS-004_Testing_Validation_Test_Labs_and_Release_Standard.md` - v1.0.0 Approved.
- `SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules.md` - v1.2.0 Approved.
- `SFGSS-006_New-Project_Guided_Pathways.md` - v1.0.0 Approved.
- `Architecture Decision Records/SFGSS-ADR-002_Full_Suite_Documentation_Gate_and_Learning_Implementation.md` - Accepted.
- `Full_Suite_Documentation_Program_Roadmap.md` - active roadmap.
- `Integration Specifications/SFGSS-INT-ADVANCED-001_Advanced_Cross-Package_and_Research_Contract_Matrix.md` - v1.0.0 Approved.
- `Architecture Decision Records/SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol.md` - v1.2.0 Accepted.
- `Package Specifications/SFGSS-The-Convergence-EchoMultiplayer-Package-Foundation.md` - v1.0.0 Approved provider-neutral foundation; provider selection pending.
- `Package Specifications/SFGSS-Instinct-EchoAI-Package-Foundation.md` - v1.0.0 Approved feasibility foundation; implementation and adapters pending.
- `Research Records/SUITE-DOC-19_EchoAI_Feasibility_and_Provider_Record.md` - approved dated feasibility/provider record.
- `Test Reports/SUITE-DOC-19_EchoAI_Feasibility_Foundation_Audit_Report.md` - documentation gate passed.
- `Research Records/SUITE-DOC-18_EchoMultiplayer_Provider_Research_Plan_and_Matrix.md` - approved dated research foundation.
- `Research Records/SUITE-DOC-18_EchoMultiplayer_Disposable_Prototype_Protocol.md` - approved comparison protocol; all executions Not run.
- `Test Reports/SUITE-DOC-18_EchoMultiplayer_Foundation_Audit_Report.md` - provider-neutral documentation gate passed.
- `Test Reports/Full_Suite_Documentation_Rebaseline_Report.md` - SUITE-DOC-01 Passed.
- Foundation package specifications, ADR-001, and the Foundation cross-package matrix - approved baseline.
- `Checkpoint Build Plans/First_Light_M1_Package_Skeleton_Checkpoint_Build_Plan.md` - approved but dormant.

### Next action

Run **SUITE-DOC-26 - SFGSS-007 ADR Template and Decision Log**. Define the canonical ADR structure, lifecycle, indexing, status transitions, supersession rules, review/approval expectations, Graph View links, and a current decision register covering ADR-001 through ADR-003 and future decisions.

---

## Open Questions

- Licensing remains a later suite-wide release decision.
- Final Multiplayer provider approval requires disposable prototype evidence and cannot be truthfully completed during the pre-code documentation gate.
- Empirical compatibility, performance, migration, screenshot, and release evidence remains `Not run` until implementation.
- Final provider selection intentionally blocks any production-provider approval in SUITE-DOC-18 until disposable comparison prototypes are executed and reviewed.

---

## Active Notes

### August 4, 2026 - SFGSS-006 New-Project Guided Pathways

- `[DECISION]` SFGSS-006 v1.0.0 is approved as the canonical package-selection and staged-composition standard.
- `[DECISION]` A pathway is visible guidance, not a hidden bundle. It lists minimum, recommended, optional, and explicitly excluded selections.
- `[DECISION]` Package selection begins from the authority a project needs rather than from a fixed all-suite template.
- `[DECISION]` Every pathway records its first vertical slice, persistence choice, scene/world choice, bridges/providers, project-owned work, evidence path, and removal story.
- `[DECISION]` The Workshop may implement a pathway as a versioned preset only through an immutable dry-run plan and exact package-owned setup facades. Manual composition remains supported.
- `[DECISION]` Advanced/provider-backed pathways remain research or experimental until executed evidence and an ADR support stronger claims.
- `[DECISION]` Existing-project adoption preserves working systems until standalone proof, project parity, migration, rollback, and removal evidence passes.
- `[DECISION]` Twelve approved guidance pathways now cover Blank Modular, package Laboratories, minimal audiovisual prototypes, Game Jam Quickstart, puzzle/tabletop, password platformers, save adventures, narrative games, action combat, RPG staging, local multiplayer, online multiplayer research, and incremental adoption.
- `[TEST]` Documentation audit confirms all 28 package authorities appear in the selection map, all three cross-package matrices are respected, and no implementation or compatibility result was promoted.
- `[HANDOFF]` SUITE-DOC-26 drafts SFGSS-007 ADR Template and Decision Log next.

**Promoted to:** SFGSS-006 v1.0.0, SFGSS-000 v0.15.0 decisions 91-96, SUITE-DOC-25 audit report, README, graph roadmap, health check, program roadmap, and artifact manifest.




### August 4, 2026 — Advanced cross-package and research review

- `[DECISION]` SFGSS-INT-ADVANCED-001 v1.0.0 is approved after reviewing The Convergence, Instinct, Clash, Arcana, and The Atlas against all Foundation and Expansion authorities.
- `[DECISION]` Multiplayer world travel follows the ordered Atlas plan -> Convergence authority/readiness -> Passage scene execution -> Atlas context commit -> Fellowship/project placement workflow.
- `[DECISION]` Participant, network entity, character, runtime actor, control owner, input user, AI agent, ability owner, combat target, world location, scene binding, and marker identities remain separate and explicitly mapped.
- `[DECISION]` Instinct proposes semantic actions, Arcana owns ability activation, and Clash owns instantaneous combat resolution and target-receiver transaction coordination.
- `[DECISION]` Clash relation and targetability are read-only inputs to AI and ability targeting; neither Instinct nor Arcana creates competing combat truth.
- `[DECISION]` Arcana permits one mutation-capable cost provider per MVP activation and rejects fictional cross-authority atomicity.
- `[DECISION]` Atlas semantic routes, Instinct navigation paths, Passage scene transitions, and Vessel movement commands are distinct contracts.
- `[DECISION]` Chronicle publishes shared multiplayer saves on the authoritative host/server; Advanced packages contribute only their versioned payloads.
- `[DECISION]` ADR-001 advances to v1.2.0 and registers exact Workshop setup facades and minimum planning domains for all five Advanced package foundations.
- `[DECISION]` No provider, topology, backend, or performance claim was promoted. Multiplayer prototypes remain 0 executed and all empirical evidence remains `Not run`.
- `[TEST]` SUITE-DOC-24 passes authority, lifecycle, dependency, identity, transaction, persistence, diagnostics, Laboratory, research-honesty, and removal review.
- `[HANDOFF]` SUITE-DOC-25 drafts SFGSS-006 New-Project Guided Pathways using all 28 package foundations and the three approved cross-package matrices.

**Promoted to:** SFGSS-INT-ADVANCED-001 v1.0.0, SFGSS-ADR-001 v1.2.0, SFGSS-000 v0.14.0, SUITE-DOC-24 review report, README, graph roadmap, health check, program roadmap, and artifact manifest.

### August 4, 2026 - The Atlas (`EchoWorld`) feasibility foundation

- `[DECISION]` The Atlas Feasibility Foundation v1.0.0 is approved as the Level 2 pre-code authority for stable world, zone, location, connection, scene-binding, marker, and provider identities; immutable topology; current semantic context; travel planning; discovery; visitation; fast-travel policy; map snapshots; state participants; diagnostics; validation; Laboratories; and optional bridges.
- `[DECISION]` Semantic `LocationId` values are independent from Unity scenes, scene paths, build indexes, display names, and Unity asset GUIDs.
- `[DECISION]` Atlas prepares semantic travel plans. The Passage or project provider executes scene travel and reports success before Atlas commits context.
- `[DECISION]` Atlas selects entry/spawn marker snapshots but never spawns, teleports, possesses, or controls characters.
- `[DECISION]` Discovery and visitation are separate from progression access, objective completion, and reward truth.
- `[DECISION]` Atlas owns core context/discovery/visit snapshots and routes versioned provider records; Chronicle remains save-file and slot authority.
- `[DECISION]` Map snapshots are provider-neutral data. Looking Glass or project code owns rendering and navigation.
- `[DECISION]` Shared-world context and state default to host/server authority through a future Convergence bridge; no networking SDK enters the core.
- `[DECISION]` Procedural generation, world simulation, scene streaming, navigation/pathfinding, weather/time systems, and large-world partitioning remain outside the MVP.
- `[TEST]` The foundation contains all 30 SFGSS-001 sections, 104 Laboratory scenarios, and 624 individually registered planned tests. All empirical results remain `Not run`.
- `[HANDOFF]` SUITE-DOC-23 performs the Expansion Cross-Package Collision Review next.

**Promoted to:** `Package Specifications/SFGSS-The-Atlas-EchoWorld-Package-Foundation.md`, `Research Records/SUITE-DOC-22_EchoWorld_Feasibility_and_Boundary_Record.md`, `Test Reports/SUITE-DOC-22_EchoWorld_Feasibility_Foundation_Audit_Report.md`, README, roadmap, and artifact manifest.

---

## Checkpoint Closeout Checklist - SUITE-DOC-22

- [x] Reconcile `Current Notes.md`.
- [x] Approve EchoWorld identity, topology, context, travel, scene-binding, marker, discovery, fast-travel, map, persistence, diagnostics, Laboratory, bridge, removal, and release contracts.
- [x] Keep Passage, Fellowship, Chronicle, UI, Camera, Objectives, AI, and Multiplayer authorities outside the neutral core.
- [x] Register 104 Laboratory scenarios and 624 package-qualified planned tests.
- [x] Keep every unexecuted runtime, provider, scene, marker, map, migration, multiplayer, platform, performance, compatibility, integration, and release result `Not run`.
- [x] Update README, Current Notes, roadmap, feasibility/boundary record, audit report, and artifact manifest.
- [x] Confirm no package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider, or gameplay implementation was created.
- [x] Record SUITE-DOC-21 as committed/pushed by the owner.
- [ ] Commit and push SUITE-DOC-22.
- [x] Stop before collision review conclusions or implementation work.

---

## Handoff Snapshot - SUITE-DOC-22

**Completed checkpoint:** SUITE-DOC-22 - The Atlas (`EchoWorld`) Feasibility Foundation  
**Result:** Approved v1.0.0 feasibility foundation  
**Current focus:** Advanced Cross-Package and Research Review  
**Active checkpoint:** SUITE-DOC-23  
**Foundation specifications:** 10 of 10 approved  
**Expansion specifications:** 13 of 13 approved  
**Advanced foundations:** 5 of 5 approved  
**Package implementation:** Not started  
**First queued implementation:** FL-M1-01 - First Light Package Skeleton  
**Runtime authorization:** None  
**Known blockers:** None for documentation; all implementation and provider evidence remains Not run  
**Prior checkpoint:** SUITE-DOC-21 confirmed committed/pushed by owner  
**Commit/push:** SUITE-DOC-22 pending user confirmation  
**Stop point:** Before any collision-review promotion, package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider adapter, or runtime implementation



### August 4, 2026 - Arcana (`EchoAbilities`) feasibility foundation

- `[DECISION]` Arcana Feasibility Foundation v1.0.0 is approved as the Level 2 authority for provider-neutral ability definitions, owner state, grants, loadouts, activation requests/results, conditions, targeting contracts, costs, charges, cooldowns, casting, interruption, channels, effect execution, persistence seams, diagnostics, validation, and optional bridges.
- `[DECISION]` One application-session root coordinates owner-scoped state. Definitions remain immutable; cooldowns, charges, activations, queues, targets, cost tokens, and effect tickets remain runtime state.
- `[DECISION]` The MVP permits one mutation-capable cost provider per activation. Additional providers may contribute read-only requirements. A multi-provider transaction protocol is deferred.
- `[DECISION]` Commit at cast start and commit at cast completion are both supported. Pre-commit interruption may avoid cost/state commitment; post-commit interruption never implies automatic rollback.
- `[DECISION]` Effects use explicit stable executor IDs and typed payloads. Reflection, string method dispatch, and open assembly scanning are prohibited.
- `[DECISION]` Clash remains the authority for instantaneous combat resolution. Arcana may submit Clash requests through a separate effect bridge after activation commitment.
- `[DECISION]` Active targeting, casts, channels, queues, prepared costs, and effect tickets are never durable save state.
- `[DECISION]` Shared-world activation defaults to authoritative host/server validation through a future Convergence bridge. Prediction remains presentation/provider-specific.
- `[DECISION]` Status effects, passive/reactive abilities, per-tick channel costs, visual graphs, and provider-specific rollback prediction are deferred.
- `[TEST]` The foundation contains all 30 SFGSS-001 sections, 96 Laboratory scenarios, and 600 individually registered planned tests. All executions remain `Not run`.
- `[HANDOFF]` SUITE-DOC-22 drafts The Atlas (`EchoWorld`) next.

**Promoted to:** `Package Specifications/SFGSS-Arcana-EchoAbilities-Package-Foundation.md`, `Research Records/SUITE-DOC-21_EchoAbilities_Feasibility_and_Boundary_Record.md`, `Test Reports/SUITE-DOC-21_EchoAbilities_Feasibility_Foundation_Audit_Report.md`, README, roadmap, and artifact manifest.



### August 4, 2026 - The Convergence (`EchoMultiplayer`) provider-neutral foundation

- `[DECISION]` The Convergence provider-neutral foundation v1.0.0 is approved as the Level 2 authority for neutral multiplayer sessions, participants, readiness, roles, lifecycle requests/results, authority gates, synchronized-travel and spawn/ownership seams, diagnostics, security rules, adapter packaging, and research gates.
- `[DECISION]` No production provider, topology, transport, cloud/session service, provider adapter, package version, hosting model, platform claim, or implementation is approved by SUITE-DOC-18.
- `[DECISION]` The neutral core has no networking SDK dependency. Each provider or service integration ships separately under SFGSS-002.
- `[DECISION]` One production provider may be registered per root/session. Provider capabilities are explicit; unsupported features return structured `Unavailable` results.
- `[DECISION]` Session, participant, account/profile, character, network entity, and Unity object identities remain separate.
- `[DECISION]` Important gameplay requests are untrusted until validated at the authoritative peer/server through provider-backed gates.
- `[DECISION]` The Passage retains scene-transition authority; EchoMultiplayer coordinates synchronized travel through a bridge.
- `[DECISION]` Shared-world save authority defaults to host/server. Clients do not submit complete trusted save payloads.
- `[DECISION]` NGO plus Multiplayer Services is mandatory Prototype A. Prototype B uses FishNet only if license review clears; otherwise Mirror. Photon Fusion is a conditional third candidate.
- `[RISK]` FishNet uses a custom license with exclusions for competing networking solutions; public adapter distribution requires explicit review.
- `[TEST]` The provider-neutral specification contains all 30 SFGSS-001 sections, 84 Laboratory scenarios, and 486 individually registered planned tests. All executions remain `Not run`.
- `[TEST]` Disposable Prototypes A, B, and conditional C remain `Not run`; no provider winner is selected.
- `[HANDOFF]` SUITE-DOC-19 drafts Instinct (`EchoAI`) next.

**Promoted to:** `Package Specifications/SFGSS-The-Convergence-EchoMultiplayer-Package-Foundation.md`, both SUITE-DOC-18 Research Records, SUITE-DOC-18 audit report, README, roadmap, and artifact manifest.

### August 4, 2026 - The Vessel (`EchoControllers`) package specification

- `[DECISION]` EchoControllers owns actor-bound controller hosts, family-specific normalized intent, stale-safe source/control leases, local motor execution, approved probes/capabilities, semantic locomotion state/events, warp/external-motion seams, diagnostics, setup, validation, and independent preset Laboratories.
- `[DECISION]` The package is rootless. One controller host and one authoritative preset motor live with each actor; no persistent or global controller singleton exists.
- `[DECISION]` Side-View 2D and Top-Down 2D are the two MVP controller families. Each has its own assembly boundary, configuration, scene, scripted intent driver, readout, and acceptance evidence.
- `[DECISION]` Intent lifecycle is shared, but payloads are family-specific so the package does not create one universal locomotion action struct or mandatory input map.
- `[DECISION]` The MVP physics path supports Dynamic Rigidbody2D and executes motor-owned motion on a declared fixed-step boundary. Other physics/navigation backends require later family design.
- `[DECISION]` AlwaysControlled provides the simple single-pawn path; LeaseRequired provides stale-safe possession and local multiplayer integration.
- `[DECISION]` Configuration assets remain immutable. Velocity, contacts, coyote/jump buffers, intent sequences, source/control generations, capability state, and diagnostics are runtime-only.
- `[DECISION]` The package publishes semantic snapshots/events. Animation, camera, audio, VFX, UI, characters, input, combat, save, scene, and network systems retain their own authorities.
- `[DECISION]` Scripted intent drivers are the mandatory standalone Lab controls so the core has no Input System dependency. Interactive input belongs to a separate adapter.
- `[DECISION]` One modular UPM package remains approved for the MVP. A split into controller-family packages is reconsidered after a third distinct backend/family proves separate dependencies or release cadence.
- `[TEST]` The specification registers 68 package-qualified Laboratory scenarios and 408 individually registered planned tests. Every implementation-dependent result remains `Not run`.
- `[HANDOFF]` SUITE-DOC-16 is complete. Continue with SUITE-DOC-17: The Crucible (`EchoCrafting`) design workshop and package specification.

**Promoted to:** `Package Specifications/SFGSS-The-Vessel-EchoControllers-Package-Specification.md`, `Test Reports/SUITE-DOC-16_EchoControllers_Package_Specification_Audit_Report.md`, README, and roadmap.

### August 4, 2026 - The Eye (`EchoCamera`) package specification

- `[DECISION]` EchoCamera owns camera channels, target/group registration, provider-neutral modes, priority leases, blends, modifiers, bounds, zones, viewport metadata, impulses, backend capability negotiation, diagnostics, authoring, validation, and isolated 2D/3D Laboratories.
- `[DECISION]` Camera channels are the unit of independent output authority. Main is the MVP path, while bounded secondary channels prevent a future split-screen breaking redesign.
- `[DECISION]` The neutral core has no Cinemachine dependency and ships a built-in Unity Camera backend for true standalone operation. Cinemachine remains a separate provider adapter.
- `[DECISION]` Modes, modifiers, bounds, targets, groups, and impulses use generational handles or leases. Out-of-order release recomputes from active truth rather than restoring stale snapshots.
- `[DECISION]` Mode arbitration resolves higher priority, then later acquisition. Blend interruption begins from the current evaluated output.
- `[DECISION]` Backends declare either root-driven or backend-driven tick ownership. Two systems may never write one Camera during the same channel tick.
- `[DECISION]` Targets and groups supply provider-neutral snapshots and warp revisions so destruction, teleport, switching, and target loss remain explicit.
- `[DECISION]` One effective bounds request per channel is the bounded MVP. 2D and 3D zones are optional adapter assemblies that own only their occupancy-derived leases.
- `[DECISION]` Impact owns feedback recipes; The Eye owns final camera impulse application through an explicit bridge.
- `[DECISION]` Active channels, targets, groups, modes, blends, modifiers, bounds, zones, impulses, and backend state are session-only and are not durable save truth.
- `[DECISION]` EchoCamera does not own characters, controllers, input devices, dialogue, scene travel, UI, rendering pipelines, post-processing, level layout, multiplayer player assignment, or project cinematography.
- `[TEST]` The specification registers 60 package-qualified Laboratory scenarios and 360 package-qualified planned tests. Every implementation-dependent result remains `Not run`.
- `[HANDOFF]` SUITE-DOC-14 is complete. Continue with SUITE-DOC-15: The Fellowship (`EchoCharacters`) package specification.

**Promoted to:** `Package Specifications/SFGSS-The-Eye-EchoCamera-Package-Specification.md`, `Test Reports/SUITE-DOC-14_EchoCamera_Package_Specification_Audit_Report.md`, README, and roadmap.

### August 4, 2026 - The Hand (`EchoInteraction`) package specification

- `[DECISION]` EchoInteraction owns interaction action definitions, interactor/detector/endpoint registration, normalized offers, candidate freshness, deterministic focus, semantic prompt snapshots, interaction sessions, cancellation/commit policy, bounded local concurrency, diagnostics, authoring, validation, and isolated 2D/3D Laboratories.
- `[DECISION]` One endpoint may expose several independent offers. Detection discovers endpoints; endpoints author current offers; project executors own the unique result.
- `[DECISION]` The neutral core does not reference Physics2D or Physics3D. Separate adapter assemblies translate physics queries into package-neutral candidate observations.
- `[DECISION]` Focus uses deterministic ranking with configurable hysteresis. Small physics jitter must not make prompt focus oscillate.
- `[DECISION]` Blocked offers may remain visible with structured availability and denial reasons when the project chooses that presentation policy.
- `[DECISION]` Input remains external. EchoInteraction consumes semantic Start, Continue, Release, Cancel, and Repeat commands rather than polling devices or bindings.
- `[DECISION]` Tap, Hold, Timed, Toggle, and Repeated modes share one request/session model while retaining distinct lifetime and cancellation semantics.
- `[DECISION]` Executors explicitly declare the irreversible commit point. Cancellation before commit may succeed; cancellation after commit returns Too Late and preserves the committed result.
- `[DECISION]` One active session per interactor is the safe default. Endpoints may be Shared, Exclusive, or bounded-concurrency and use generational reservation/session identities.
- `[DECISION]` Focus, active sessions, reservations, block leases, and candidate caches are session-only state and are not durable save truth.
- `[DECISION]` EchoInteraction does not own input bindings, production UI, audio, feedback, objectives, inventory, dialogue, camera, scene loading, save transport, character movement, world-state persistence, multiplayer authority, or project-specific interaction outcomes.
- `[TEST]` The specification registers 56 package-qualified Laboratory scenarios and 336 package-qualified planned tests. Every implementation-dependent result remains `Not run`.
- `[HANDOFF]` SUITE-DOC-13 is complete. Continue with SUITE-DOC-14: The Eye (`EchoCamera`) package specification.

**Promoted to:** `Package Specifications/SFGSS-The-Hand-EchoInteraction-Package-Specification.md`, `Test Reports/SUITE-DOC-13_EchoInteraction_Package_Specification_Audit_Report.md`, README, and roadmap.

---

### August 4, 2026 - The Vault (`EchoInventory`) package specification

- `[DECISION]` The Vault (`EchoInventory`) Package Specification v1.0.0 is approved as the Level 2 authority for immutable item definitions, stable catalogs/tags, fungible stacks, unique mutable item instances, list/fixed/equipment containers, quantities, deterministic weight, filters, queries, atomic transactions, revisions, generic equipment occupancy, state export/import, unknown-data preservation, diagnostics, authoring, validation, and optional bridge seams; implementation remains locked until SUITE-DOC-33.
- `[DECISION]` EchoInventory does not own crafting transformations, vendor economics, combat or RPG effects, item-use gameplay, world spawning, production UI, save-file transport, objective/dialogue/character truth, or multiplayer authority.
- `[DECISION]` One duplicate-safe application-session `EchoInventoryRoot` exposes injectable `IEchoInventoryService`; duplicate rejection occurs before catalogs, providers, containers, subscriptions, or mutable state.
- `[DECISION]` Fungible stacks and unique item instances are separate entry species. Fungible units have no individual IDs; unique mutable items keep durable `ItemInstanceId` values and default to quantity one.
- `[DECISION]` Fixed-slot and bounded-list containers are the MVP. Quantity, stack, entry-count, slot, filter, and checked integer weight-unit rules are evaluated before commit.
- `[DECISION]` Add, remove, move, split, merge, swap, transfer, equip, unequip, and batch operations touching local containers are atomic. Expected container revisions reject stale requests.
- `[DECISION]` Generic equipment owns named slots and occupancy only. Combat effects, class restrictions, attributes, set bonuses, and item abilities remain external.
- `[DECISION]` State export/import is provider-neutral. Chronicle persistence is optional. Missing item definitions and unknown item-state component providers preserve opaque bounded records until restored or explicitly pruned.
- `[DECISION]` The diagnostic namespace `EINV-*` is reserved for The Vault.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 52 unique Laboratory scenarios, and 302 unique package-qualified planned tests.
- `[TEST]` Every runtime, transaction, equipment, provider, persistence, migration, compatibility, performance, platform, bridge, removal, and release result remains `Not run` under SFGSS-004.
- `[NOTE]` SFGSS-000 remains v0.12.0 because the specification refines the already-approved EchoInventory authority and equipment boundary without changing suite-wide ownership.
- `[HANDOFF]` SUITE-DOC-13 drafts EchoInteraction (`The Hand`) next. Preserve interaction discovery/selection/execution-request authority without deciding project-specific interaction results.

**Promoted to:** The Vault (`EchoInventory`) Package Specification v1.0.0, SUITE-DOC-12 audit report, README, roadmap, artifact manifest, and Current Notes handoff.

### August 4, 2026 - The Path (`EchoObjectives`) package specification

- `[DECISION]` The Path (`EchoObjectives`) Package Specification v1.0.0 is approved as the Level 2 authority for objective definitions, availability, prerequisite graphs, objective runs, sequential/parallel/threshold groups, manual/counter/flag/timer/provider steps, optional and hidden policies, repeatability, tracking, transactional progress, completion, reward ledgers, state export/import, migrations, diagnostics, authoring, validation, and optional bridge seams; implementation remains locked until SUITE-DOC-33.
- `[DECISION]` EchoObjectives does not own gameplay facts, production UI, localization, dialogue, inventory, progression, characters, crafting, scene travel, camera, audio, save-file transport, multiplayer authority, or concrete reward execution.
- `[DECISION]` One duplicate-safe application-session `EchoObjectivesRoot` exposes injectable `IEchoObjectivesService`; duplicate rejection occurs before subscriptions, clocks, providers, or state mutation.
- `[DECISION]` Objective definitions are immutable project-owned assets. Runtime runs use stable `ObjectiveRunId` identities, allowing sequential repeat runs without stale requests mutating a new run.
- `[DECISION]` The MVP graph vocabulary is Ordered, AllRequired, AnyRequired, and Threshold groups plus Manual, Counter, Flag, Timer, and Provider leaf steps. Optional and hidden are independent policies.
- `[DECISION]` Availability evaluation is read-only and distinguishes Available, Locked, and Unavailable. Missing or failed providers never imply success.
- `[DECISION]` Progress uses typed requests, atomic batches, run generations, and a bounded request-id dedupe window. Events publish only after state commits.
- `[DECISION]` Completion commits before reward delivery. Each authored reward receives a deterministic grant ID and an independent Pending/InProgress/Succeeded/Failed/Unavailable/Skipped ledger. Reward failure never rolls completed objective truth backward.
- `[DECISION]` The MVP supports one active run per definition, sequential repeatability, bounded terminal history, one primary tracked objective, and bounded pins. Concurrent repeated runs remain deferred.
- `[DECISION]` State export/import is core and provider-neutral. Chronicle persistence is optional. Missing definitions/providers/reward executors preserve orphaned or pending records until they return or an explicit backed-up prune occurs.
- `[DECISION]` The diagnostic namespace `EOBJ-*` is reserved for The Path.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 48 unique Laboratory scenarios, and 268 unique package-qualified planned tests.
- `[TEST]` Every runtime, provider, reward, persistence, migration, compatibility, performance, platform, bridge, removal, and release result remains `Not run` under SFGSS-004.
- `[NOTE]` SFGSS-000 remains v0.12.0 because the specification refines the already-approved EchoObjectives authority without changing a suite-wide ownership boundary.
- `[HANDOFF]` SUITE-DOC-12 drafts EchoInventory (`The Vault`) next. Preserve item/container authority without absorbing crafting transformations, RPG statistics, combat effects, vendor economics, or save-file transport.

**Promoted to:** The Path (`EchoObjectives`) Package Specification v1.0.0, SUITE-DOC-11 audit report, README, roadmap, artifact manifest, and Current Notes handoff.

### August 4, 2026 - Voices (`EchoDialogue`) package specification

- `[DECISION]` Voices (`EchoDialogue`) Package Specification v1.0.0 is approved as the Level 2 authority for stable speaker and conversation definitions, one deterministic foreground conversation session, node traversal, lines, choices, read-only conditions, explicit project commands, local variables, interruption, suspension, cancellation, semantic history, safe active-session snapshots, diagnostics, authoring, validation, and optional bridge seams; implementation remains locked until SUITE-DOC-33.
- `[DECISION]` EchoDialogue does not own translation tables, production UI, player input, audio playback, quest/objective truth, inventory/character state, camera movement, global game state/pause, scene travel, save-file transport, cinematic direction, or the game's complete narrative database.
- `[DECISION]` The runtime uses one duplicate-safe application-session `EchoDialogueRoot` exposing injectable `IDialogueService`; the MVP supports one foreground session with RejectNew, bounded QueueLatest, and opt-in ReplaceActive for interruptible conversations.
- `[DECISION]` Conversation definitions use stable explicit records for Line, Choice, Branch, Command, LocalMutation, Wait, and End rather than reflection-discovered method/node types.
- `[DECISION]` Conditions are read-only and synchronous in the MVP. Commands are explicit typed asynchronous handlers with timeout, cancellation, authored failure policy, and an honest commit point.
- `[DECISION]` Source fallback text permits standalone use. Many Tongues, Looking Glass, Resonance, Pulse, Will, Path, Eye, Chronicle, First Light, Observatory, and Workshop integrations remain explicit optional bridges/providers.
- `[DECISION]` Session, presentation, and choice generations reject stale or foreign requests. Semantic history stores IDs/references rather than resolved production text by default.
- `[DECISION]` Active-session state may be exported/imported only at declared safe points; committed commands are never replayed merely to reconstruct presentation.
- `[DECISION]` The diagnostic namespace `EDLG-*` is reserved for Voices.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 44 unique Laboratory scenarios, and 217 unique package-qualified planned test cases.
- `[TEST]` Every runtime, installation, provider, presenter, command, persistence, compatibility, performance, platform, bridge, removal, and release result remains `Not run` under SFGSS-004.
- `[NOTE]` SFGSS-000 remains v0.12.0 because the specification refines the already-approved EchoDialogue authority without changing a suite-wide ownership boundary.
- `[HANDOFF]` SUITE-DOC-11 drafts EchoObjectives (`The Path`) next. Preserve objective/quest progress authority without absorbing dialogue rendering, inventory storage, reward execution, or save-file transport.

**Promoted to:** Voices (`EchoDialogue`) Package Specification v1.0.0, SUITE-DOC-10 audit report, README, roadmap, artifact manifest, and Current Notes handoff.

### August 4, 2026 - The Ascent (`EchoProgression`) package specification

- `[DECISION]` The Ascent (`EchoProgression`) Package Specification v1.0.0 is approved as the Level 2 authority for neutral progression definitions, unlocks, access evaluation, checkpoints, completion records, local rank snapshots, authored password grants, atomic progression mutations, versioned state documents, migration, and diagnostics; implementation remains locked until SUITE-DOC-33.
- `[DECISION]` EchoProgression does not own save-file transport, scene loading, production UI, inventory, character statistics/XP, objective logic, platform achievements, online leaderboards, multiplayer authority, or the gameplay events that earn progression.
- `[DECISION]` The runtime uses one duplicate-safe application-session `EchoProgressionRoot` exposing an injectable `IEchoProgressionService`.
- `[DECISION]` Project-defined categories use stable domain IDs rather than a fixed public enum. Unity asset GUIDs and display names never become durable progression identity.
- `[DECISION]` Access evaluation uses bounded built-in condition trees plus explicit provider registration. Missing/failed providers return `Unavailable` and never imply access granted.
- `[DECISION]` Mutation batches validate completely and publish atomically. Events occur after state publication.
- `[DECISION]` Completion records support counts, latest/best bounded numeric metrics, and project-authored local rank tables without becoming analytics or online leaderboard authority.
- `[DECISION]` Checkpoints store stable identity and opaque resume tokens. Passage/project adapters map them to travel; the core never loads a scene.
- `[DECISION]` The MVP password system uses authored normalized entries, redacted diagnostics, exact-state generation, preview, freshness validation, and atomic grant application. Passwords are convenience codes, not security, DRM, credentials, or entitlements.
- `[DECISION]` Core persistence is explicit versioned export/import. Chronicle participation and a small progression-only local backend remain optional separate integrations/providers.
- `[DECISION]` Unknown/orphan durable records are preserved but inactive until their definitions return or an explicit backed-up prune operation removes them.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 40 unique Laboratory scenarios, and 144 unique package-qualified planned test cases.
- `[TEST]` Every runtime, installation, migration, compatibility, performance, provider, platform, bridge, removal, and release result remains `Not run` under SFGSS-004.
- `[NOTE]` SFGSS-000 remains v0.12.0 because this package specification refines the already-approved EchoProgression authority without changing a suite-wide ownership boundary.
- `[HANDOFF]` SUITE-DOC-08 drafts EchoBuildTools (`The Foundry`) next. Preserve build preparation/validation authority without absorbing runtime game flow, source control, or automatic external deployment.

**Promoted to:** The Ascent (`EchoProgression`) Package Specification v1.0.0, SUITE-DOC-07 audit report, README, roadmap, artifact manifest, and Current Notes handoff.

### August 4, 2026 - The Wellspring (`EchoPool`) package specification

- `[DECISION]` The Wellspring (`EchoPool`) Package Specification v1.0.0 is approved as the Level 2 authority for general-purpose GameObject and Component reuse; implementation remains locked until SUITE-DOC-33.
- `[DECISION]` EchoPool owns pool definitions, runtime pool instances, prewarming, acquisition, generational leases, validated return, capacity/growth/exhaustion policy, scope ownership, lifecycle callbacks, automatic return, external-destruction reconciliation, and diagnostics.
- `[DECISION]` EchoPool does not own spawn intent, encounters, projectile behavior, audio voices, UI virtualization, network spawning authority, save truth, or project-specific reset semantics.
- `[DECISION]` The default runtime uses one duplicate-safe application-session `EchoPoolRoot` implementing an injectable `IEchoPoolService`; scene and owner pools remain explicit child scopes.
- `[DECISION]` Every successful spawn returns a session-local generational handle. Stale, foreign, double-returned, lost, and destroyed handles fail without mutating the current instance use.
- `[DECISION]` `PoolDefinition` and catalog assets are immutable project-owned definitions with stable domain IDs. Active counts, records, generations, schedules, scenes, scopes, and statistics remain runtime state.
- `[DECISION]` Exhaustion defaults to safe rejection. Bounded temporary overflow is opt-in and is destroyed rather than retained on return. Forced reclamation of active instances is deferred.
- `[DECISION]` The core resets only generic parent, transform, active state, scene, and lease metadata. Project-specific state resets through `IPoolable` or explicit optional adapters; reflection-based universal reset is rejected.
- `[DECISION]` Application, scene, and owner-lease scopes are approved. Standalone scene-unload reconciliation remains available, while a separate Passage bridge may coordinate pre-unload cleanup.
- `[DECISION]` Manual, scaled-duration, unscaled-duration, and generic completion-signal returns are approved. Every schedule/signal binds to the current generation.
- `[DECISION]` Active pool handles and runtime instances are never saved. Gameplay authorities save semantic state and reconstruct objects through their own factories.
- `[DECISION]` Jukebot retains its internal audio voice pool, and network object reuse requires a provider-specific adapter.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 36 unique Laboratory scenarios, and 118 unique planned package test IDs.
- `[TEST]` Every runtime, installation, scene, performance, compatibility, provider, removal, and release test remains `Not run` under SFGSS-004.
- `[NOTE]` SFGSS-000 remains v0.12.0 because the specification refines the already-approved EchoPool authority without changing a suite-wide ownership boundary.
- `[HANDOFF]` SUITE-DOC-07 drafts EchoProgression (`The Ascent`) next. Preserve unlock/checkpoint/progression authority without absorbing save-file transport, inventory, character statistics, or platform achievements.

**Promoted to:** The Wellspring (`EchoPool`) Package Specification v1.0.0, SUITE-DOC-06 audit report, README, roadmap, artifact manifest, and Current Notes handoff.

### August 4, 2026 - Impact (`EchoFeedback`) package specification

- `[DECISION]` Impact (`EchoFeedback`) Package Specification v1.0.0 is approved as the Level 2 authority for coordinated transient feedback; implementation remains locked until SUITE-DOC-33.
- `[DECISION]` EchoFeedback owns immutable feedback recipes, transient instance execution, unscaled scheduling, channel scaling, arbitration, cancellation, provider coordination, and bounded diagnostics.
- `[DECISION]` EchoFeedback does not own gameplay results, camera movement, audio playback, UI state, input-device assignment, settings persistence, save data, or final pause/time-scale authority.
- `[DECISION]` The MVP uses a flat semantic timeline. Parallel and sequential behavior are expressed through start offsets rather than a branching graph or general visual-scripting language.
- `[DECISION]` Production effects execute only through explicitly registered channel providers. The core remains independent of EchoCamera, Jukebot, EchoUI, EchoInput, The Accord, and The Pulse.
- `[DECISION]` Recipe and signal ScriptableObjects are immutable definitions with stable domain IDs. Active instances, handles, clocks, providers, scales, and histories are runtime-owned state.
- `[DECISION]` Public feedback handles are generational so stale handles cannot cancel recycled instances.
- `[DECISION]` Scheduling, cancellation, restoration, and diagnostics use an unscaled clock so feedback remains controllable while scaled game time is zero.
- `[DECISION]` A standalone Unity time provider is opt-in and exclusive. When The Pulse is installed, a separate bridge preserves The Pulse as the final time authority.
- `[DECISION]` Input System haptics belong to a separate provider artifact rather than the core package.
- `[DECISION]` Project safety caps, accessibility scales, and channel suppression apply before provider execution. Providers receive already-resolved effective values.
- `[DECISION]` The isolated Impact Laboratory uses simulated providers. Simulation proves the core package but does not count as support evidence for optional bridges or hardware providers.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 32 unique Laboratory scenarios, and 92 unique planned test IDs.
- `[TEST]` Every runtime, provider, compatibility, performance, platform, removal, and release test remains `Not run` under SFGSS-004.
- `[NOTE]` SFGSS-000 remains v0.12.0 because the Impact specification refines the already-approved EchoFeedback authority without changing suite-wide ownership.
- `[HANDOFF]` SUITE-DOC-06 drafts EchoPool (`The Wellspring`) next. Preserve general-purpose object reuse authority without absorbing enemy spawning, projectile rules, audio voice pooling, or other package-owned behavior.

**Promoted to:** Impact (`EchoFeedback`) Package Specification v1.0.0, SUITE-DOC-05 audit report, README, roadmap, and Current Notes handoff.

### August 4, 2026 - Package specification priority clarification

- `[DECISION]` The owner clarified that “continue until all documentation is ready instead of just 7.1” primarily means completing the package foundations in SFGSS-000 Sections 7.2 Expansion and 7.3 Advanced before implementation.
- `[DECISION]` Remaining general standards no longer block the start of Expansion package specifications. They move after the package foundations, except where a standard is directly required to resolve an active package decision.
- `[DECISION]` SFGSS-002, SFGSS-003, and SFGSS-004 remain approved and become the dependency, data, and evidence guardrails for every remaining package specification.
- `[DECISION]` Expansion specifications follow the owner’s listed order beginning with EchoFeedback, then EchoPool, EchoProgression, EchoBuildTools, EchoLocalization, EchoDialogue, EchoObjectives, EchoInventory, EchoInteraction, EchoCamera, EchoCharacters, EchoControllers, and EchoCrafting.
- `[DECISION]` Advanced package foundations follow with EchoMultiplayer, EchoAI, EchoCombat, EchoAbilities, and EchoWorld.
- `[DECISION]` EchoCrafting’s checkpoint must include its required design-workshop record before approving the package contract. EchoMultiplayer remains evidence-honest and may approve research, neutral contracts, and prototype criteria without claiming unperformed provider prototypes.
- `[DECISION]` The final documentation unlock gate is renumbered to SUITE-DOC-33 after the roadmap is condensed around package-first work.

**Promoted to:** Full Suite Documentation Program Roadmap, Package Specification Priority Rebaseline Report, README, and Current Notes handoff.

### August 3, 2026 - Living repository documentation

- `[DECISION]` Suite and package documentation will live in the Git repository beside development work.
- `[DECISION]` The repository documentation folder will be opened directly in Obsidian rather than copied into a separate vault.
- `[DECISION]` Every active repository will expose a linked `Current Notes.md` page for ongoing observations, proposals, tests, questions, and handoff context.
- `[DECISION]` At meaningful checkpoints, durable notes will be promoted into the bible, package specification, ADR, issue/test record, guide, changelog, or checkpoint status that owns them.
- `[DECISION]` Major documentation changes will be committed with the related code when practical, or in an immediately adjacent documentation commit.

**Promoted to:** SFGSS-000 v0.5.0 decision 31 and SFGSS-001 v1.1.0 documentation requirements.

### August 3, 2026 - Foundation Specification Pass

- `[DECISION]` Complete and approve all ten Foundation Wave package specifications before beginning Foundation Wave runtime implementation.
- `[DECISION]` Run a cross-package consistency review after the tenth specification and before opening any M1 package skeleton checkpoint.
- `[DECISION]` First Light specification v1.0.0 is approved as the Level 2 package authority, but its implementation remains deferred by the suite documentation gate.
- `[DECISION]` First Light uses Unity `Awaitable<T>` for startup execution.
- `[DECISION]` First Light startup authoring uses immutable `StartupStepDefinition` ScriptableObjects that create separate single-use runtime executors.
- `[DECISION]` First Light ships a default uGUI status/image presenter isolated from its launch core.
- `[DECISION]` First Light root lifetime is configurable and defaults to `UntilHandoff`.
- `[DECISION]` The initial public Foundation package floor is Unity 6000.0, with Unity 6000.3.8f1 as the primary development baseline.

**Promoted to:** SFGSS-000 v0.6.0 decisions 32–33, First Light specification v1.0.0, and the Foundation Wave Specification Roadmap.

### August 3, 2026 - The Observatory specification

- `[DECISION]` The Observatory (`EchoDiagnostics`) specification v1.0.0 is approved as the Level 2 authority for diagnostics and validation; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoDiagnostics observes and reports package/runtime health but never becomes the source of truth for the behavior it observes and never silently repairs production state.
- `[DECISION]` Runtime integrations use explicit provider registration with stable provider IDs and disposable registration handles. Reflection-based discovery is not required for normal operation.
- `[DECISION]` Providers capture bounded, synchronous snapshots. Systems with asynchronous work cache their latest safe status rather than making the diagnostic sampler await or block gameplay.
- `[DECISION]` Diagnostic snapshots use normalized availability, health, severity, and privacy classifications so unsupported information is reported as unavailable rather than as a misleading zero or success.
- `[DECISION]` The runtime root is duplicate-safe, persists for the application session when enabled, and owns its sampler, histories, registry, event buffer, and overlay services. Editor validation can run without a runtime root.
- `[DECISION]` Runtime metric/event histories use bounded buffers and configurable update rates; diagnostic failure must degrade diagnostics rather than gameplay.
- `[DECISION]` The initial overlay uses uGUI and TextMeshPro but remains an isolated presenter over neutral diagnostic state. It does not own general UI navigation, the EventSystem, input contexts, game pause, or gameplay time scale.
- `[DECISION]` Local support-snapshot export is explicit, versioned, privacy-filtered, and never transmitted automatically.
- `[DECISION]` Editor validation supports manual, pre-Play, and pre-build execution. Repairs remain explicit and non-destructive; validation itself does not mutate production configuration.
- `[DECISION]` First Light remains independent. A separate First Light–Observatory bridge maps concrete launch status and reports into the Observatory’s neutral launch model.
- `[DECISION]` Package inventory is an Editor capability in the MVP. A Player-build package manifest is deferred until a safe build-time generation design is approved.
- `[DECISION]` The Observatory does not replace Unity’s Console or Profiler, does not promise hardware sensor support, and does not globally intercept all logs in its MVP.

**Promoted to:** The Observatory (`EchoDiagnostics`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the already-approved diagnostics authority without changing suite ownership boundaries.


### August 3, 2026 - The Accord specification

- `[DECISION]` The Accord (`EchoSettings`) specification v1.0.0 is approved as the Level 2 authority for global preferences; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoSettings owns global preference truth, defaults, committed/effective values, drafts, validation, application coordination, versioned persistence, migration, and safe display confirmation. It does not own production settings UI, audio playback, input execution, localization content, save-slot progress, pause, or gameplay rules.
- `[DECISION]` The runtime model separates project defaults, committed settings, effective settings, editable drafts, and preserved unknown section records.
- `[DECISION]` The package uses explicit stable-ID typed section registration with independent document and section schema versions. Reflection-based settings discovery is not approved.
- `[DECISION]` Unknown optional-package section payloads are preserved when their definition or bridge is absent so clean package removal does not erase data.
- `[DECISION]` Edit sessions record the committed revision they started from. A stale draft returns a conflict rather than overwriting a newer commit silently.
- `[DECISION]` Settings application is transactional. Required appliers run provisionally in deterministic order and previously applied sections revert in reverse order when a required step fails.
- `[DECISION]` Risky display changes remain provisional until confirmed. Cancel, unscaled timeout, shutdown, application failure, or persistence failure restores the previous effective platform state.
- `[DECISION]` The default backend is a versioned structured JSON document stored beneath `Application.persistentDataPath`; `PlayerPrefs` is not the default backend.
- `[DECISION]` Corrupt, unsupported-old, and newer files are preserved for recovery. Recovery/default use does not silently overwrite evidence or a newer schema.
- `[DECISION]` The MVP built-in sections are Audio, Display, and basic Accessibility. EchoSettings stores audio/accessibility preference values; Jukebot, feedback, UI, input, and localization behavior remains in optional bridges or project adapters.
- `[DECISION]` The built-in display adapter is replaceable and capability-aware. Unsupported platform fields report unavailable rather than false success.
- `[DECISION]` Optional consumers may register appliers after settings initialization and receive the current effective values, avoiding circular startup requirements.
- `[DECISION]` The core is nonvisual. A sample or EchoUI presenter owns controls, silent binding, prompts, navigation, and display-confirmation presentation.
- `[DECISION]` Named profiles, import/export, monitor selection, HDR/dynamic-resolution options, cloud synchronization, and secure storage remain deferred or outside the MVP.
- `[DECISION]` Public asynchronous operations use fresh Unity `Awaitable<T>` instances, consistent with the Foundation Unity 6 baseline.

**Promoted to:** The Accord (`EchoSettings`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the existing global-preference authority and preserve the approved cross-package ownership matrix.

### August 3, 2026 - The Passage specification

- `[DECISION]` The Passage (`EchoSceneFlow`) specification v1.0.0 is approved as the Level 2 authority for normal scene travel after First Light handoff; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoSceneFlow owns destination validation, request admission, one serialized transition pipeline, progress, activation, route helpers, recovery results, and scene-flow diagnostics. It does not own startup orchestration, game-state rules, production UI, save policy, audio playback, gameplay completion, multiplayer authority, or scene content.
- `[DECISION]` Runtime destinations use project-owned `SceneDefinition` and `SceneRouteDefinition` assets with stable IDs. Scene asset paths are backend locators maintained by Editor tooling, not durable identity or a public raw-string API.
- `[DECISION]` One duplicate-safe application-session `EchoSceneFlowRoot` owns the service, backend, queue, runner, participants, presenter registration, status, and bounded history. Duplicate rejection occurs before subscriptions or scene-operation side effects.
- `[DECISION]` The MVP backend uses Unity `SceneManager.LoadSceneAsync` for asynchronous single-scene loading behind an `ISceneLoadBackend` seam. Additive loading, owned unload, persistent scene sets, Addressables, and multiplayer providers remain deferred.
- `[DECISION]` Public asynchronous operations use fresh Unity `Awaitable<T>` instances and execute Unity scene APIs on the main thread.
- `[DECISION]` Only one scene operation may be active. The default admission policy is `RejectNew`; optional FIFO queuing is bounded, pending requests may be replaced by policy, and the active load is never replaced.
- `[DECISION]` Equivalent active or queued requests coalesce to one operation. Explicit reload remains a distinct operation.
- `[DECISION]` Cancellation is cooperative while queued or before the backend begins loading. After Unity loading starts, cancellation is reported as unsupported in the current phase and the operation continues to a safe terminal state or recovery.
- `[DECISION]` Immediate scene activation is the default. Optional delayed activation is short, hard-bounded, and never permitted to stall Unity's async operation queue indefinitely.
- `[DECISION]` Transition presenters and lifecycle participants register explicitly through disposable handles. No presenter is a valid core path; reflection discovery is not required.
- `[DECISION]` The runtime core is nonvisual and does not depend on uGUI or TextMeshPro. The Standalone Test Lab may use a sample-only presenter, and EchoUI may provide a separate production presenter bridge.
- `[DECISION]` Recovery may attempt one configured fallback only, with validation and runtime loop protection.
- `[DECISION]` Reload, Main Menu, and Hub helpers resolve project-configured route assets rather than hidden scene names.
- `[DECISION]` The direct-scene initializer is development-only by default and creates the minimum root only when an authority is absent.

**Promoted to:** The Passage (`EchoSceneFlow`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the existing normal-scene-travel authority and preserve the approved cross-package ownership matrix.


### August 3, 2026 - The Pulse specification

- `[DECISION]` The Pulse (`EchoGameState`) specification v1.0.0 is approved as the Level 2 authority for high-level runtime state, validated primary transitions, temporary override scopes, nested pause reasons, and resulting global time/cursor policy; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoGameState owns exactly one primary application state plus zero or more leased override scopes. It does not own menu presentation, input bindings, audio playback, scene loading, character/enemy state machines, save transport, or project-specific victory/defeat rules.
- `[DECISION]` Override scopes are stored as a keyed set rather than a strict last-in-first-out stack so owners may release their own scopes safely and out of order.
- `[DECISION]` Override dominance is deterministic: higher explicit priority wins and acquisition sequence breaks equal-priority ties.
- `[DECISION]` Pause is derived from active states, policies, and leases. The package exposes no fragile global pause boolean and no caller-managed increment/decrement counter.
- `[DECISION]` Any active pause requirement wins. One running scope cannot cancel another owner’s pause requirement.
- `[DECISION]` Cursor, input-intent, and audio-intent channels select the highest-priority explicit policy while remaining neutral requests for peer packages to apply through bridges.
- `[DECISION]` Primary-state transitions are synchronous and atomic. Transition guards are explicit, synchronous, deterministic, and side-effect-free; asynchronous preparation remains outside the state mutation.
- `[DECISION]` Effective policy is recomputed from current primary state and active scopes rather than restored from a previous-value stack, preventing stale restoration after out-of-order releases.
- `[DECISION]` One duplicate-safe application-session `EchoGameStateRoot` owns the state service, policy composer, Unity time/cursor adapters, scope registry, bounded history, diagnostics, and cleanup. Duplicate rejection occurs before side effects.
- `[DECISION]` Unity time and cursor behavior are behind replaceable adapters. Fixed-step scaling is configurable and disabled by default because the correct physics policy is project-specific.
- `[DECISION]` State timing, history, timeout, and diagnostics use an injected unscaled clock so they remain observable while gameplay time is paused.
- `[DECISION]` Input and audio coordination remain semantic intents. EchoInput and Jukebot retain authority over input execution and audio behavior through optional bridges.
- `[DECISION]` The runtime core is nonvisual and has no uGUI, TextMeshPro, Input System, networking, or other Echo-package dependency. The Standalone Test Lab uses removable sample-only controls and readouts.
- `[DECISION]` Primary state, active scopes, and runtime history are session state and are not automatically saved. Future persistence may store validated project-defined hints, never live lease handles.
- `[DECISION]` Direct-scene initialization is development-only by default and creates the minimum authority only when absent.
- `[DECISION]` Active scopes and state history are bounded, diagnostic provider failures remain isolated, and drift reconciliation occurs at explicit lifecycle points rather than through hidden per-frame contention.
- `[DECISION]` Slow motion, hit stop, photo-mode time modifiers, focus-loss policy, and multiplayer authority remain deferred until their neighboring package and integration contracts are approved.

**Promoted to:** The Pulse (`EchoGameState`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the already-approved runtime-state and pause authority without changing the suite ownership matrix.

### August 3, 2026 - Resonance specification

- `[DECISION]` Resonance (`Jukebot`) specification v1.0.0 is approved as the Level 2 authority for runtime music, SFX, ambience, voice-pool, playback-handle, and mixer-routing execution; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` One duplicate-safe application-session `JukebotRoot` owns ordinary music, SFX, ambience, bus, diagnostics, and runtime-state children. `Awake` claims authority only; initialization performs side effects later.
- `[DECISION]` Music, SFX, and ambience remain independent services and transports so one channel cannot accidentally stop, pause, replace, or duplicate another.
- `[DECISION]` The MVP music player uses exactly two owned sources and a deterministic transport state machine for play, pause, resume, stop, playlist navigation, rapid replacement, and crossfade behavior.
- `[DECISION]` Music starts and handoffs use Unity DSP time where scheduling improves consistency, but the package makes no universal gapless-playback claim across all clips, import settings, and platforms.
- `[DECISION]` SFX playback uses a bounded owned voice pool rather than relying on one untracked `PlayOneShot` source for the production path.
- `[DECISION]` SFX playback handles are generational so stale handles cannot stop or modify a later sound that reused the same voice.
- `[DECISION]` Cue cooldown and concurrency are validated before allocation where possible. Cue and group limits use explicit reject-or-steal policies.
- `[DECISION]` Voice stealing is deterministic by configured priority, audibility estimate, age, and stable voice index as the final tie-break.
- `[DECISION]` `MusicTrack`, `MusicPlaylist`, `SfxCue`, variations, ambience profiles, routing, and audio-profile assets remain immutable. Playlist indexes, shuffle bags, cooldown timestamps, active counts, handles, queues, and transition state are runtime-owned.
- `[DECISION]` Audio profiles use a hybrid schema-and-instance model: package/project schemas define stable semantic slots, project profiles map those slots to cues, and profile sets compose only the groups a game needs.
- `[DECISION]` Project-owned mixer routing exposes stable bus bindings. Jukebot applies normalized values and mute state but never persists global preferences; The Accord retains persistence authority.
- `[DECISION]` Jukebot does not own the project AudioListener, scene-to-music mapping, pause truth, production settings UI, gameplay triggers, or save files.
- `[DECISION]` The runtime core is nonvisual and has no uGUI, TextMeshPro, or peer Echo-package dependency. Editor preview tools and the standalone Audio Laboratory may use removable presentation dependencies.
- `[DECISION]` Mixer snapshot/ducking graphs, random ambience one-shots, segmented tracks, custom loop regions, adaptive stems, Addressables/provider clips, and reverse playback remain deferred or experimental.
- `[DECISION]` First Light, Observatory, Accord, Pulse, Passage, Looking Glass, and later gameplay connections remain explicit bridges or project adapters with clean missing-peer and removal behavior.
- `[DECISION]` The standalone Resonance Audio Laboratory must prove music transport races, pooled voices, stale handles, concurrency, ambience independence, routing, domain pause, diagnostics, reset, shutdown, and definition immutability without unrelated Echo packages.

**Promoted to:** Resonance (`Jukebot`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the already-approved audio authority without changing the suite ownership matrix.


### August 3, 2026 - The Will specification

- `[DECISION]` The Will (`EchoInput`) specification v1.0.0 is approved as the Level 2 authority for input contexts, reason-based locks, active-device/control-scheme awareness, primary-user pairing, rebinding, binding-override models, prompt/glyph data, and input diagnostics; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoInput owns input infrastructure but does not own movement physics, gameplay action meaning, controller behavior, production UI screens, high-level game state, audio feedback, scene travel, or durable preference storage.
- `[DECISION]` One duplicate-safe application-session `EchoInputRoot` owns runtime services, and duplicate rejection occurs before action cloning, subscriptions, device pairing, override application, or map enablement.
- `[DECISION]` The project-owned `InputActionAsset` remains immutable authoring data. The default runtime mode clones it into an owned action collection; advanced injected collection mode is explicit and lower-isolation.
- `[DECISION]` Actions, maps, and bindings use Unity Input System GUIDs as persistence authority. Names and indexes are never stable save identifiers.
- `[DECISION]` Context state uses one primary context plus independently leased override contexts. Map directives are Enable, Disable, or Unchanged, with deterministic priority and acquisition-order resolution.
- `[DECISION]` Input locks are additive, reason-based leases that can target all input, maps, or actions. They resolve after context directives and release safely out of order.
- `[DECISION]` EchoInput owns enablement only for configured maps/actions. External drift is detected and reported rather than fought every frame.
- `[DECISION]` The MVP supports one primary `InputUser` with conservative pairing. Device/scheme changes require meaningful input and filter analog drift, pointer jitter, noisy/synthetic events, and unassigned devices.
- `[DECISION]` Device changes never automatically change gameplay context; prompt presentation and gameplay mode remain separate truths.
- `[DECISION]` Interactive rebinding is transactional: snapshot, internal lock/context, candidate validation, conflict analysis, atomic commit, or exact rollback. One session per user is allowed by default.
- `[DECISION]` Composite rebinding commits all required parts together or restores every part.
- `[DECISION]` Conflict analysis considers normalized path, control scheme/group, user, expected type, composite identity, context overlap, shareability metadata, and reserved controls. The safe default is Reject.
- `[DECISION]` Binding overrides use a versioned package-owned document keyed by stable action/binding GUIDs, with source identity/fingerprint, migration reporting, and preserved orphan/unknown entries. Unity’s opaque override JSON is interoperability input, not the long-term authority.
- `[DECISION]` The Accord or project integration owns durable storage. EchoInput core provides session import/export and never silently chooses `PlayerPrefs`, a filename, or a profile boundary.
- `[DECISION]` Glyph libraries and control displays are project-owned. Resolution falls back from exact glyph to family/generic/text, and the core ships no unlicensed branded controller art.
- `[DECISION]` The runtime core is nonvisual and has no required uGUI, TextMeshPro, EventSystem, `PlayerInput`, generated-wrapper, or peer Echo-package dependency.
- `[DECISION]` Built-in Input System interactions and processors are preferred before custom hold/tap/multi-tap/dead-zone helpers are introduced.
- `[DECISION]` Diagnostics expose semantic state only and never retain raw typed text, key sequences, continuous input histories, full device serials, or platform-account identifiers.
- `[DECISION]` Unity 6000.0 and Input System 1.17.0 are the planned public floors, with Unity 6000.3.8f1 as the primary development baseline; exact compatibility must be reverified before implementation and release.
- `[DECISION]` The standalone Will Input Laboratory must prove contexts, locks, device filtering, pairing, transactional rebinding, composites, conflict resolution, override migration, prompt fallback, duplicate safety, reset, shutdown, and source-asset immutability without unrelated Echo packages.
- `[DECISION]` The implementation test registry contains 70 planned cases.
- `[DECISION]` No SFGSS-000 revision is required because these choices refine the already-approved EchoInput authority without changing the suite ownership matrix.

**Promoted to:** The Will (`EchoInput`) Package Specification v1.0.0.

---


### August 3, 2026 - The Looking Glass specification

- `[DECISION]` The Looking Glass (`EchoUI`) specification v1.0.0 is approved as the Level 2 authority for reusable runtime UI presentation infrastructure; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoUI owns its persistent layer root, screen history, modal ordering/results, HUD region coordination, bounded notifications, prompts/tooltips, EventSystem/focus coordination, view lifecycles, theme application, accessibility-aware presentation, and UI diagnostics. It does not own settings truth, save files, input-context authority, scene travel, pause/time, audio playback, localization content, or gameplay rules.
- `[DECISION]` The runtime uses one duplicate-safe application-session `EchoUIRoot`. Authority is claimed before EventSystem adoption/creation, layer setup, registry mutation, focus work, subscriptions, or transitions.
- `[DECISION]` The root exposes seven explicit layers: Screen, HUD, Modal, Notification, Tooltip/Prompt, Transition, and Debug.
- `[DECISION]` Screen history supports Push, Replace, Reset, and Back. Structural operations are serialized with bounded admission, explicit coalescing/rejection/queue behavior, cancellation, stale-operation protection, and hard transition bounds.
- `[DECISION]` Modal entries use owned generational handles and exact-once terminal results. Out-of-order close, owner loss, repeated completion, capacity, queue overflow, and shutdown behavior are defined.
- `[DECISION]` EventSystem behavior is an explicit non-destructive policy: adopt assigned, adopt one valid existing system, create when missing, or require an external system. EchoUI reports conflicts and never silently deletes project EventSystems.
- `[DECISION]` Focus is event-driven and deterministic, with declared defaults, restoration, scoped containment, fallback, and a legal no-selection state. The package does not perform broad hierarchy searches or force reselection every frame.
- `[DECISION]` Project-owned view prefabs and presenters interpret domain data and commands. EchoUI owns lifecycle and presentation coordination, never the domain state displayed by a view.
- `[DECISION]` HUD regions, notification queues, prompts, tooltips, screen history, modal queues, and diagnostic histories are bounded. Overflow behavior is explicit and observable.
- `[DECISION]` Runtime themes/configuration remain immutable. Effective accessibility policy may scale text, extend/manualize transient timing, suppress/reduce motion, and select contrast/fallback presentation; The Accord remains the persistence authority.
- `[DECISION]` uGUI with TextMeshPro-compatible text is the first approved backend. Exact Unity 6 dependency versions are verified at M1 rather than guessed. UI Toolkit, native screen-reader providers, XR, and advanced virtualization remain deferred.
- `[DECISION]` EchoUI diagnostics are privacy-safe and do not retain or export rendered text, typed input, arbitrary view-model payloads, profile names, screenshots, or hierarchy/file paths by default.
- `[DECISION]` Peer integrations are explicit removable bridges or project adapters. EchoUI presents settings, saves, scene loading, pause, input, and audio state without absorbing their authority.
- `[DECISION]` The isolated Looking Glass UI Laboratory defines 42 manual scenarios and the specification registers 84 implementation tests across installation, lifecycle, screens, modals, focus, accessibility, diagnostics, stress, integration, migration, and removal.

**Promoted to:** The Looking Glass (`EchoUI`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the existing UI presentation authority and preserve the approved cross-package ownership matrix.


### August 3, 2026 - The Chronicle specification

- `[DECISION]` The Chronicle (`EchoSave`) specification v1.0.0 is approved as the Level 2 authority for durable local game-save documents, slots, generations, participant payload transport, migration, recovery, and save-operation diagnostics; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoSave owns save files, slot and generation management, save/load orchestration, participant contracts, serializer/storage seams, integrity checks, backup retention, corruption recovery, and save-specific tooling. It does not own global preferences, project gameplay schemas, automatic scene-object serialization, production save UI, game-state rules, scene travel, cloud synchronization, or platform accounts.
- `[DECISION]` One duplicate-safe application-session `EchoSaveRoot` claims authority before paths, callbacks, catalog scans, participant registration, or file operations.
- `[DECISION]` The MVP uses immutable save generations. A complete generation is written, flushed where supported, checksummed, re-read/verified, and only then published as current through a small head pointer.
- `[DECISION]` Slot metadata lives in a manifest separate from the participant payload so slot lists do not deserialize full game state. The catalog cache is derived and rebuildable, never the sole authority.
- `[DECISION]` Slots use stable package-generated IDs independent from display names. Display names never become physical directory names.
- `[DECISION]` Independent game systems register narrow, stable-ID, versioned participants. EchoSave transports detached DTOs without knowing the project’s inventory, character, quest, world, or progression models.
- `[DECISION]` Unknown or temporarily unclaimed participant payloads are preserved opaquely across a load-save round trip by default. Removal requires an explicit bounded prune plan.
- `[DECISION]` Loading is two-phase: `PrepareLoadAsync` validates, recovers, deserializes, and migrates into a disposable handle; `ApplyPreparedLoadAsync` applies only after required participants exist. A one-step convenience path remains available for same-scene loads.
- `[DECISION]` Package document migrations and participant payload migrations are separate contiguous upgrade chains. Missing steps block safely, source records remain unchanged, unsupported newer formats are preserved, and downgrade is not promised.
- `[DECISION]` The default serializer uses Unity `JsonUtility` for package envelopes and plain serializable DTOs. Unsupported dictionaries, polymorphic graphs, interfaces, and durable Unity object references are documented; custom serializers use explicit provider IDs.
- `[DECISION]` Participant capture and apply occur on the main thread by default. Detached serialization, hashing, and local file I/O may run in the background when provider capability allows. Public async operations return fresh `Awaitable<T>` instances and complete on the main thread.
- `[DECISION]` One mutating operation runs globally in the MVP. Manual requests reject while busy by default, while autosaves coalesce into at most one pending latest request.
- `[DECISION]` Cancellation is honored before publication. Once head publication begins, cancellation is Too Late and the operation settles to a known committed or failed state without abandoning the prior valid generation.
- `[DECISION]` Create, rename, duplicate, select, prepare-delete, confirm-delete, prepare-load, apply-load, recovery planning, and redacted support export have structured request/result contracts. Destructive actions require explicit two-step plans.
- `[DECISION]` Checksums detect accidental corruption but are not encryption, authentication, or anti-cheat. Payload sizes, counts, migration depth, histories, queues, and paths are bounded and validated.
- `[DECISION]` Cloud/platform storage, cross-device merge, compression, encryption, streaming/chunked worlds, and multiplayer save authority remain deferred provider or future-specification work.
- `[DECISION]` The isolated Chronicle Save Laboratory defines 32 acceptance scenarios and the implementation registry contains 100 planned cases, including fault injection at each generation-publication boundary.
- `[DECISION]` No SFGSS-000 revision is required because these choices refine the already-approved EchoSave authority and preserve the global settings/save boundary.

**Promoted to:** The Chronicle (`EchoSave`) Package Specification v1.0.0.


### August 3, 2026 - The Workshop specification

- `[DECISION]` The Workshop (`EchoGameStarter`) specification v1.0.0 is approved as the Level 2 authority for Editor-time package selection, composition planning, package-operation coordination, safe project generation, generation records, repair planning, removal guidance, and readiness reporting. Implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` The Workshop is Editor-only and ships no runtime assembly, persistent root, `GameManager`, service locator, or Player dependency. Generated projects must remain valid after The Workshop is removed.
- `[DECISION]` Every apply operation begins from an immutable dry-run plan that exposes package changes, bridges, scenes, folders, assets, project settings, risk, ownership, and reversibility. A materially changed resolved package graph invalidates approval and requires review.
- `[DECISION]` Package Manager operations and asset generation are separate resumable phases. A transient journal under `Library/EchoGameStarter/Transactions` records recovery state across domain reload or Editor restart, but never auto-resumes mutation after restart.
- `[DECISION]` Normal package changes use Unity Package Manager Client APIs rather than direct editing of `Packages/manifest.json`. Recommended sources use exact versions, tags, or commits; development branches remain visibly non-reproducible choices.
- `[DECISION]` Package-specific setup remains owned by the selected package. The Workshop uses exact allowlisted, versioned Editor setup-facade adapter descriptors and does not perform open-ended assembly discovery or copy package setup logic.
- `[RESOLVED]` FW-DOC-11 reconciled the setup-facade contract through SFGSS-ADR-001. No shared Editor-only contracts package was introduced.
- `[DECISION]` Generated output is project-owned. A durable manifest records logical IDs, GUIDs, paths, origins, versions, fingerprints, adoption, modification, and operation receipts without granting The Workshop perpetual control.
- `[DECISION]` Create-only-safe behavior is the default. Existing, adopted, or modified assets are preserved. Any fingerprint drift removes automatic overwrite eligibility and moves upgrades to manual or side-by-side handling.
- `[DECISION]` The MVP ships Blank Modular Starter and Game Jam Quickstart. Blank may select no peer packages. Game Jam shows every selected package and bridge; the Chronicle is an explicit save-model choice rather than a hidden requirement.
- `[DECISION]` The MVP provides repeat-run analysis, safe repair plans for eligible missing outputs, a basic upgrade diff, and a removal guide. Full automatic uninstall remains deferred.
- `[DECISION]` Unity 6 global scene lists and Build Profile overrides are handled through an explicit adapter with complete before/after reporting; ambiguity blocks modification.
- `[DECISION]` UI Toolkit is the approved Editor UI. The core standalone proof is an isolated Workshop Laboratory and disposable clean-project fixtures rather than a meaningless runtime scene.
- `[DECISION]` The Laboratory defines 40 acceptance scenarios and the specification registers 121 implementation tests spanning package resolution, reload recovery, planning, security, generation, setup facades, scenes, reports, migration, repeatability, removal, and performance.
- `[DECISION]` The Workshop never commits or pushes source control in the MVP. It writes commit-friendly reports and leaves Git actions to the user or a future explicit provider.
- `[RESOLVED]` The Workshop package decisions originally required no SFGSS-000 revision. FW-DOC-11 later promoted the suite-wide facade and collision rules into SFGSS-000 v0.7.0.

**Promoted to:** The Workshop (`EchoGameStarter`) Package Specification v1.0.0.


### August 3, 2026 - Foundation cross-package collision review

- `[DECISION]` SFGSS-INT-FOUNDATION-001 is approved as the Foundation authority/lifecycle/dependency/bridge/data/Test Lab/removal reconciliation record.
- `[TEST]` All ten specifications retain exactly one Foundation authority per concern and no peer runtime dependency in core assemblies.
- `[BUG]` The Pulse and The Workshop both used the `EGS-*` diagnostic namespace.
- `[DECISION]` The Pulse specification advances to v1.1.0 and uses the globally unique `EGSTATE-*` namespace. EchoGameStarter retains `EGS-*`.
- `[RISK]` The nine peer packages defined setup tools but no exact Editor endpoint for The Workshop.
- `[DECISION]` SFGSS-ADR-001 accepts a package-owned exact Editor setup facade protocol with allowlisted types, six static JSON methods, plan/apply hashes, receipts, bounded reflection, and manual fallback.
- `[DECISION]` The Workshop specification advances to v1.1.0 and records SFGSS-ADR-001 as the resolved facade contract.
- `[DECISION]` Separate bridge packages declare dependencies on both peers and are removed before either peer. Core packages remain independently functional.
- `[DECISION]` Direct-scene helpers create only their own minimum missing root; First Light bridges adopt existing valid peer authorities.
- `[DECISION]` Cross-package reports qualify locally repeated `UC-*`, `CAP-*`, and `LAB-*` identifiers with the package ID.
- `[TEST]` Settings/save boundaries, launch-to-Passage handoff, UI/input/state/audio boundaries, diagnostics bridges, standalone laboratories, and removal behavior pass the documentation collision review.
- `[HANDOFF]` No runtime implementation is authorized yet. FW-DOC-12 is the final documentation gate.

**Promoted to:** SFGSS-000 v0.7.0 decisions 34–38, The Pulse specification v1.1.0, The Workshop specification v1.1.0, SFGSS-ADR-001, and SFGSS-INT-FOUNDATION-001.



### August 3, 2026 - Foundation Documentation Readiness Gate

- `[TEST]` FW-DOC-12 verified ten Approved Foundation specifications with all thirty SFGSS-001 sections present.
- `[TEST]` SFGSS-ADR-001 and SFGSS-INT-FOUNDATION-001 are present and aligned with Pulse v1.1.0 and Workshop v1.1.0.
- `[BUG]` The repository referenced SFGSS-005 without containing the workflow document.
- `[DECISION]` SFGSS-005 v1.0.0 is approved as the Checkpoint Build Workflow and ChatGPT Collaboration authority.
- `[BUG]` First Light v1.0.0 still pointed to the completed documentation gate rather than the first implementation checkpoint.
- `[DECISION]` First Light advances to v1.1.0 for status/workflow reconciliation only; runtime behavior and API intent are unchanged.
- `[DECISION]` FW-DOC-12 passes. FL-M1-01 - First Light Package Skeleton is the first authorized implementation checkpoint.
- `[DECISION]` FL-M1-01 authorizes package files and validation only. It authorizes no C# script, authority root, ScriptableObject, prefab, scene, sample, setup tool, or bridge.
- `[HANDOFF]` Import this checkpoint, commit/push it, then execute `Checkpoint Build Plans/First_Light_M1_Package_Skeleton_Checkpoint_Build_Plan.md`.

**Promoted to:** SFGSS-000 v0.8.0 decisions 39–40, SFGSS-005 v1.0.0, First Light v1.1.0, the Foundation Documentation Readiness Report, and FL-M1-01 Checkpoint Build Plan.

---


### August 3, 2026 - Full Suite Documentation Rebaseline

- `[DECISION]` Extend the documentation-first gate from the Foundation Wave to the complete planned documentation program in SFGSS-000 Section 18.
- `[DECISION]` Preserve all Foundation approvals and readiness evidence; FL-M1-01 remains the first queued implementation checkpoint but is dormant until SUITE-DOC-36.
- `[DECISION]` Complete architecture standards, Expansion specifications, Advanced design/research records, and final full-suite collision/readiness reviews before code.
- `[DECISION]` Distinguish design-complete documentation from implementation evidence. Do not invent compile results, screenshots, performance measurements, compatibility validation, migration evidence, release notes, or prototype findings.
- `[DECISION]` When implementation begins, ChatGPT must show complete compile-ready code in the conversation, explain each file and important section, provide exact Unity Editor steps, and teach the architectural reason for the choice.
- `[DECISION]` Jesse enters the code himself by default. Generated source files or direct edits occur only when explicitly requested and do not replace visible code/explanations.
- `[HANDOFF]` The active checkpoint is SUITE-DOC-02 - SFGSS-002 Dependency, Bridge, and Assembly Standard.

**Promoted to:** SFGSS-000 v0.9.0 decisions 41–43, SFGSS-ADR-002, SFGSS-005 v1.1.0, Full Suite Documentation Program Roadmap, and the SUITE-DOC-01 Rebaseline Report.

---


### August 4, 2026 - Dependency, Bridge, and Assembly Standard

- `[DECISION]` SFGSS-002 v1.0.0 is approved as the canonical package-manifest, assembly-direction, bridge/provider, compile-guard, sample/test dependency, compatibility, and clean-removal standard.
- `[DECISION]` Core runtime packages do not reference optional peer Echo packages. A separate bridge declares dependencies on every peer it connects; peers never reference the bridge.
- `[DECISION]` UPM manifests record concrete required dependency versions. Broader compatible/tested ranges live in documentation and the suite compatibility catalog and remain pending until evidenced.
- `[DECISION]` Runtime assemblies cannot reference Editor, test, sample, Workshop, project, or optional-peer assemblies. Optional presentation/backend/provider technologies are isolated when they are not central hard dependencies.
- `[DECISION]` Primary public Runtime assemblies may remain Auto Referenced for novice usability. Editor, tests, samples, and optional bridge/provider assemblies default to non-auto-referenced unless a documented public use case requires otherwise.
- `[DECISION]` Compile symbols, version defines, `.asmref` files, and reflection cannot conceal dependency truth or replace a proper bridge/provider package.
- `[DECISION]` Exact allowlisted Editor reflection remains permitted for ADR-001 setup facades; broad assembly scans remain prohibited.
- `[DECISION]` Standalone Labs use only the package and hard dependencies. Integration Labs belong to the bridge/provider artifact.
- `[DECISION]` Optional artifacts follow bridge-first teardown/removal and own all registrations, leases, subscriptions, and adapter resources they create.
- `[TEST]` The standard was reconciled against SFGSS-000, SFGSS-001, ADR-001, ADR-002, the Foundation contract matrix, and all ten Foundation assembly/dependency tables.
- `[RISK]` First Light’s approved assembly table still places proposed uGUI in the neutral Runtime assembly; SFGSS-002 prefers a separate presentation assembly. Reconcile during SUITE-DOC-30 before code.
- `[RISK]` Several Foundation specifications list Editor assemblies as Auto Referenced or describe optional sample uGUI/TMP dependencies without a final compile-safe packaging decision. Reconcile during SUITE-DOC-30.
- `[HANDOFF]` SUITE-DOC-03 must align stable IDs, DTOs, unknown-data preservation, aliases, migrations, transactions, and provider/package removal with SFGSS-002.

**Promoted to:** SFGSS-000 v0.10.0 decisions 44–51, SFGSS-002 v1.0.0, the SUITE-DOC-02 audit report, README, and the full-suite roadmap.

---


### August 4, 2026 - Data, IDs, Serialization, and Migration Standard

- `[DECISION]` SFGSS-003 v1.0.0 is approved as the canonical data classification, identity, Unity GUID, serialization, migration, unknown-data, transaction, recovery, and durable-removal standard.
- `[DECISION]` Unity asset GUIDs, package/project domain stable IDs, and runtime instance IDs are separate contracts. AssetDatabase identity is Editor-only unless explicitly copied into a runtime-safe build record.
- `[DECISION]` Stable domain IDs use either approved opaque generated IDs or package/project-qualified semantic IDs. Names, paths, indexes, timestamps alone, runtime instance IDs, and CLR type names are not durable identity.
- `[DECISION]` Shared ScriptableObjects and configuration assets remain immutable runtime inputs. Mutable session state lives in authority-owned runtime records; durable state uses detached DTOs or opaque payloads.
- `[DECISION]` Durable documents declare a format ID and schema version independently from package SemVer. Serializer providers state supported shapes, bounds, unknown-field behavior, determinism, and failure behavior.
- `[DECISION]` Unity JsonUtility is approved for simple DTOs only. It does not by itself provide dictionary, general polymorphism, or unknown-field round-trip guarantees.
- `[DECISION]` Supported migrations are explicit contiguous forward steps on staged data. They preserve the source until verified publication, report changes, and do not promise downgrade.
- `[DECISION]` Released ID changes use aliases or tombstones. Alias cycles, ambiguous mappings, and reuse of retired IDs are prohibited.
- `[DECISION]` Unknown optional settings, save, provider, and generated records remain bounded, opaque, preserved, and non-executable through package absence/reinstallation.
- `[DECISION]` Data-changing operations validate and stage before one documented publication point. Each package states its real rollback class and never labels a partial apply as atomic.
- `[TEST]` SFGSS-003 was reconciled against SFGSS-000, SFGSS-001, SFGSS-002, ADR-001, ADR-002, the Foundation matrix, and all ten Foundation package data sections.
- `[RISK]` Accord and Chronicle use “Asset GUID” wording for configuration identity. Clarify Unity asset identity versus runtime domain identity during SUITE-DOC-30.
- `[RISK]` Accord and Will unknown-field preservation requires an explicit opaque-record or extension-capable serializer strategy before implementation.
- `[RISK]` Foundation public serialized enums and fingerprints require compatibility/canonicalization review during SUITE-DOC-30.
- `[HANDOFF]` SUITE-DOC-04 must turn existing package test lists into one canonical evidence, laboratory, compatibility, defect, and release standard without claiming tests have run.

**Promoted to:** SFGSS-000 v0.11.0 decisions 52–61, SFGSS-003 v1.0.0, the SUITE-DOC-03 audit report, README, and the full-suite roadmap.

---

### August 4, 2026 – SFGSS-004 Testing, Validation, Test Labs, and Release Standard

- `[DECISION]` SFGSS-004 v1.0.0 is approved as the suite test, evidence, Laboratory, compatibility, defect, and release-quality authority.
- `[DECISION]` Durable test results use Not run, Pass, Pass with advisory, Fail, Blocked, or Not applicable.
- `[DECISION]` Compatibility language uses Unknown, Planned, Tested, Supported, Experimental, or Unsupported and must name the exact environment covered.
- `[DECISION]` Stable test IDs are package/bridge/provider-qualified and are never recycled.
- `[DECISION]` Test definitions and executions are separate records. A planned registry is not passing evidence.
- `[DECISION]` Standalone Laboratories prove one package; Integration Laboratories belong to bridges/providers; Showcases do not replace either proof.
- `[DECISION]` Clean import/compile must be followed by the smallest functional workflow for each advertised installation route.
- `[DECISION]` Setup, repair, migration, removal, reinstall, failure recovery, performance, platform, accessibility, privacy, and security evidence are explicit release concerns when applicable.
- `[DECISION]` Defect severity is Blocker, Critical, Major, Minor, or Advisory and remains separate from priority.
- `[DECISION]` Flaky/quarantined required tests and retry-hidden failures cannot count as passing stable release evidence.
- `[DECISION]` Beta, release-candidate, and stable gates require progressively stronger evidence.
- `[TEST]` SFGSS-004 was reconciled against SFGSS-000, SFGSS-001, SFGSS-002, SFGSS-003, SFGSS-005, ADR-001, ADR-002, the Foundation matrix, and all ten Foundation package test/release sections.
- `[RISK]` Bare Laboratory IDs, mixed automation fields, compressed Will test ranges, broad platform wording, combined distribution gates, and missing evidence/issue columns require normalization during SUITE-DOC-30.
- `[HANDOFF]` SUITE-DOC-05 must turn package-selection guidance into explicit user pathways without creating hidden hard dependencies or pretending every project needs the full Foundation set.

**Promoted to:** SFGSS-000 v0.12.0 decisions 62–71, SFGSS-004 v1.0.0, the SUITE-DOC-04 audit report, README, and the full-suite roadmap.

---

## Promotion Queue

| Date | Entry | Destination | Status |
|---|---|---|---|
| 2026-08-04 | EchoCamera channels, targets/groups, modes, blends, modifiers, bounds, zones, impulses, backends, diagnostics, and Laboratories | EchoCamera Package Specification v1.0.0 | Promoted |
| 2026-08-04 | EchoInventory definitions, stacks, unique instances, containers, atomic transactions, equipment storage, persistence, diagnostics, and Laboratory | EchoInventory Package Specification v1.0.0 | Promoted |
| 2026-08-04 | EchoObjectives objective graphs, progress, repeatability, reward ledgers, persistence, diagnostics, and Laboratory | EchoObjectives Package Specification v1.0.0 | Promoted |
| 2026-08-04 | EchoDialogue speakers, conversation graph, lines, choices, conditions, commands, interruption, semantic history, safe snapshots, diagnostics, authoring, and Laboratory | EchoDialogue Package Specification v1.0.0 | Promoted |
| 2026-08-04 | EchoLocalization locale lifecycle, stable localized references, fallback/missing policy, formatting, asset leases, fonts/direction, pseudo-localization, diagnostics, and Laboratory | EchoLocalization Package Specification v1.0.0 | Promoted |
| 2026-08-04 | EchoBuildTools recipes, Build Profile boundary, planning, validation, stamping, output safety, receipts, manifests, checksums, providers, CLI, and Laboratory | EchoBuildTools Package Specification v1.0.0 | Promoted |
| 2026-08-04 | EchoPool definitions, generational leases, capacity, exhaustion, scopes, callbacks, automatic return, reconciliation, diagnostics, and Laboratory | EchoPool Package Specification v1.0.0 | Promoted |
| 2026-08-04 | Impact recipes, providers, timeline, cancellation, channel scales, time boundary, diagnostics, and Laboratory | EchoFeedback Package Specification v1.0.0 | Promoted |
| 2026-08-04 | Testing taxonomy, evidence states, Laboratories, compatibility, defects, performance, and release gates | SFGSS-000 v0.12.0 and SFGSS-004 v1.0.0 | Promoted |
| 2026-08-04 | Data classification, stable IDs, Unity GUIDs, DTOs, serializers, migrations, aliases, unknown data, transactions, and recovery | SFGSS-000 v0.11.0 and SFGSS-003 v1.0.0 | Promoted |
| 2026-08-04 | Dependency, bridge, provider, assembly, compile-guard, sample/test, and clean-removal rules | SFGSS-000 v0.10.0 and SFGSS-002 v1.0.0 | Promoted |
| 2026-08-03 | Full Suite Documentation Gate and learning-oriented implementation | SFGSS-000 v0.9.0, SFGSS-ADR-002, SFGSS-005 v1.1.0, full-suite roadmap | Promoted |
| 2026-08-03 | Foundation Documentation Readiness Gate | SFGSS-000 v0.8.0 and readiness report | Promoted |
| 2026-08-03 | Checkpoint Build Workflow and ChatGPT collaboration rules | SFGSS-005 v1.0.0 | Promoted |
| 2026-08-03 | First Light implementation handoff and FL-M1-01 selection | First Light v1.1.0 and FL-M1-01 plan | Promoted |
| 2026-08-03 | Foundation authority/lifecycle/dependency/data/Test Lab/removal collision review | SFGSS-INT-FOUNDATION-001 and SFGSS-000 v0.7.0 | Promoted |
| 2026-08-03 | Package-owned Editor setup facade protocol | SFGSS-ADR-001 and Workshop v1.1.0 | Promoted |
| 2026-08-03 | EchoGameState/EchoGameStarter diagnostic namespace collision | Pulse v1.1.0 and SFGSS-000 v0.7.0 | Promoted |
| 2026-08-03 | Foundation Specification Pass before implementation | SFGSS-000 v0.6.0 and roadmap | Promoted |
| 2026-08-03 | Repository/Obsidian living-documentation workflow | SFGSS-000 and SFGSS-001 | Promoted |

---

## Latest Validation Snapshot

| Area | Result | Evidence/notes |
|---|---|---|
| Foundation package specifications | Approved | 10 of 10 |
| Package implementation | Not started | No package files or code authorized |
| Dependency/bridge/assembly standard | Approved | SFGSS-002 v1.0.0 |
| Data/IDs/serialization/migration standard | Approved | SFGSS-003 v1.0.0 |
| Testing/validation/Laboratory/release standard | Approved | SFGSS-004 v1.0.0 |
| Impact package specification | Approved | v1.0.0; 30 sections; 32 Laboratory scenarios; 92 planned tests, all Not run |
| EchoPool package specification | Approved | v1.0.0; 30 sections; 36 Laboratory scenarios; 118 planned tests, all Not run |
| EchoProgression package specification | Approved | v1.0.0; 30 sections; 40 Laboratory scenarios; 144 planned tests, all Not run |
| EchoBuildTools package specification | Approved | v1.0.0; 30 sections; 40 Laboratory scenarios; 156 planned tests, all Not run |
| EchoLocalization package specification | Approved | v1.0.0; 30 sections; 44 Laboratory scenarios; 196 planned tests, all Not run |
| Expansion specifications | 10 of 13 approved | Impact, The Wellspring, The Ascent, The Foundry, Many Tongues, Voices, The Path, The Vault, The Hand, and The Eye |
| EchoDialogue package specification | Approved | v1.0.0; 30 sections; 44 Laboratory scenarios; 217 planned tests, all Not run |
| EchoObjectives package specification | Approved | v1.0.0; 30 sections; 48 Laboratory scenarios; 268 planned tests, all Not run |
| EchoInventory package specification | Approved | v1.0.0; 30 sections; 52 Laboratory scenarios; 302 planned tests, all Not run |
| EchoInteraction package specification | Approved | v1.0.0; 30 sections; 56 Laboratory scenarios; 336 planned tests, all Not run |
| EchoCamera package specification | Approved | v1.0.0; 30 sections; 60 Laboratory scenarios; 360 planned tests, all Not run |
| Current checkpoint | Active | SUITE-DOC-15 - EchoCharacters: The Fellowship Package Specification |
| Known blockers | None | Multiplayer empirical provider approval intentionally remains later |

---

### August 4, 2026 - The Foundry (`EchoBuildTools`) Package Specification

- `[DECISION]` The Foundry Package Specification v1.0.0 is approved as the Level 2 authority for EchoBuildTools.
- `[DECISION]` EchoBuildTools is Editor-only. It has no runtime root, runtime assembly, Player state, or gameplay authority.
- `[DECISION]` Unity Build Profile assets own target platform, effective scenes, profile scripting defines, and platform/Player overrides. Foundry recipes wrap one explicit profile with release identity, output, validation, build, and evidence policy.
- `[DECISION]` Every attempt resolves an immutable BuildPlan and SHA-256 plan fingerprint before validation and approval.
- `[DECISION]` Foundry never changes scripting defines during execution because compilation changes require a domain reload. Define mismatch blocks before BuildPipeline.
- `[DECISION]` Version and platform build stamps are temporary by default, captured before mutation, restored after every attempt, and backed by a recovery journal.
- `[DECISION]` The MVP does not automatically increment, commit, tag, or push version/source-control state.
- `[DECISION]` Output cleaning is allowed only for an exact empty or Foundry-owned leaf whose marker matches the project and recipe. Protected roots, ancestors, traversal, symlink escape, and unowned nonempty folders block.
- `[DECISION]` Unity BuildPipeline success is not final release publication. Required artifact processors, inventory, checksums, manifest, restoration, and receipt publication must complete.
- `[DECISION]` Git metadata, CI vendors, signing, notarization, itch/store deployment, and external network operations remain optional provider adapters.
- `[DECISION]` Peer-package validators connect through explicit bridges/providers and retain ownership of package-specific truth and repair.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 40 Laboratory scenarios, and 156 package-qualified planned tests. All implementation evidence remains Not run.
- `[HANDOFF]` SUITE-DOC-09 drafts Many Tongues (`EchoLocalization`) next.

**Promoted to:** EchoBuildTools Package Specification v1.0.0, the SUITE-DOC-08 audit report, README, and full-suite roadmap.

### August 4, 2026 - Many Tongues package specification

- `[DECISION]` Many Tongues (`EchoLocalization`) specification v1.0.0 is approved as the Level 2 authority for runtime locale lifecycle, suite-facing localized reference/result contracts, fallback and missing-content policy, regional formatting, font/script/direction profiles, localization diagnostics, setup, validation, pseudo-localization, and optional bridge seams.
- `[DECISION]` EchoLocalization uses Unity's official Localization package as a declared platform backend. It does not implement a competing table, Smart String, localized asset, pseudo-locale, import/export, or Addressables system.
- `[DECISION]` The planned implementation dependency is `com.unity.localization` 1.5.12. Compatibility with Unity 6000.3.8f1 remains Planned/Not run until a clean installation checkpoint verifies it.
- `[DECISION]` One duplicate-safe application-session `EchoLocalizationRoot` claims authority before backend initialization, subscriptions, cache creation, or locale changes.
- `[DECISION]` Initial locale selection follows explicit development override, persisted preference, system match, configured startup fallback, and source locale. Every rejected candidate and winning selection source is diagnosable.
- `[DECISION]` Runtime locale changes are serialized transactions with validation, critical preload, commit, publication, cache invalidation, semantic notifications, and separate preference persistence. Cancellation is Too Late after publication begins.
- `[DECISION]` The Accord or a project provider owns durable locale preference storage. EchoLocalization validates and applies the locale; preference failure leaves the successful session change intact with a warning.
- `[DECISION]` Stable localized references use Unity Localization provider table-collection and entry identities rather than mutable display names. Generic AssetDatabase GUIDs are not treated as locale or content domain IDs.
- `[DECISION]` Missing strings/assets, fallback use, formatting failures, and asset type mismatches produce structured results. Normal missing content does not throw by default.
- `[DECISION]` Smart Strings and locale culture provide plural, select, list, number, date, time, percentage, and currency formatting. Currency formatting does not perform conversion.
- `[DECISION]` Localized asset loading returns typed disposable generational leases so backend/Addressables ownership is explicit and bounded.
- `[DECISION]` Project-owned font profiles map locales/scripts to primary fonts, fallback chains, direction metadata, and glyph fixtures. The neutral core does not promise bidirectional shaping or automatic layout mirroring.
- `[DECISION]` Pseudo-localization is an MVP requirement and shipping profiles must exclude pseudo locales.
- `[DECISION]` Native Unity component localizers and Unity 6 UI Toolkit bindings remain valid presentation paths. EchoLocalization does not require every surface to use a custom component.
- `[DECISION]` UI, Dialogue, Audio, Settings, Startup, Diagnostics, Build, and Workshop integrations remain explicit bridges or package-owned Editor facades.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 44 package-qualified Laboratory scenarios, and 196 individually registered planned tests. All implementation evidence remains Not run.
- `[DECISION]` No SFGSS-000 revision is required because the specification refines the already-approved EchoLocalization authority and preserves the suite ownership matrix.
- `[HANDOFF]` SUITE-DOC-10 drafts Voices (`EchoDialogue`) next.

**Promoted to:** Many Tongues (`EchoLocalization`) Package Specification v1.0.0, SUITE-DOC-09 audit report, README, and full-suite roadmap.

---

## Checkpoint Closeout Checklist

- [x] Reconcile `Current Notes.md`.
- [x] Approve EchoCamera ownership, independence, data, lifecycle, failure, diagnostics, authoring, Laboratory, backend, bridge, removal, and release contracts.
- [x] Define channels, targets, groups, modes, priorities, blends, modifiers, bounds, zones, viewports, impulses, backend capability negotiation, tick ownership, and direct-scene behavior.
- [x] Register 60 Laboratory scenarios and 360 package-qualified planned tests.
- [x] Keep every unexecuted runtime, channel, target, group, mode, blend, modifier, bounds, zone, impulse, backend, performance, platform, compatibility, integration, and release result `Not run`.
- [x] Update README, Current Notes, roadmap, audit report, and artifact manifest.
- [x] Confirm no package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider adapter, camera rig, target, zone, bounds, or gameplay implementation was created.
- [x] Record SUITE-DOC-13 as committed/pushed by the owner.
- [ ] Commit and push SUITE-DOC-14.
- [x] Stop before EchoCharacters specification work.

---

## Handoff Snapshot

**Completed checkpoint:** SUITE-DOC-14 - The Eye (`EchoCamera`) Package Specification  
**Result:** Approved v1.0.0  
**Current focus:** EchoCharacters - The Fellowship  
**Active checkpoint:** SUITE-DOC-15 - EchoCharacters Package Specification  
**Expansion specifications:** 10 of 13 approved  
**Package implementation:** Not started  
**First queued implementation:** FL-M1-01 - First Light Package Skeleton  
**Runtime authorization:** None  
**Known blockers:** None  
**Prior checkpoint:** SUITE-DOC-13 confirmed committed/pushed by owner  
**Commit/push:** SUITE-DOC-14 pending user confirmation  
**Stop point:** Before any package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider adapter, character definition, roster, spawner, selector, possession system, or gameplay implementation

---

## August 4, 2026 - SUITE-DOC-15 The Fellowship approved

- `[DECISION]` The Fellowship (`EchoCharacters`) Package Specification v1.0.0 is approved as the Level 2 authority for character identity, durable rosters, availability, selection contexts, groups, spawning, runtime actor registration, exclusive control ownership, switching, detached snapshots, diagnostics, authoring, validation, Laboratories, and optional bridge seams.
- `[DECISION]` Character definition identity, durable CharacterId, and session RuntimeInstanceId are separate contracts.
- `[DECISION]` Roster membership, availability/status, selection, spawn state, and control ownership are independent truths.
- `[DECISION]` Availability uses a core disposition plus stable project-defined status IDs instead of a genre-locked public status enum.
- `[DECISION]` Selection contexts are independent from ControlOwnerId assignments, supporting menus and multiple local players without implying possession.
- `[DECISION]` MVP control ownership is exclusive, stale-safe, and lease-based. Shared possession is deferred.
- `[DECISION]` Spawn does not imply selection or control. Switching prepares target spawn and handoff participants before committing selection/control truth.
- `[DECISION]` Live GameObjects, RuntimeInstanceIds, pending operations, and control leases are not saved. Detached snapshots preserve durable roster truth, aliases, and opaque extension records.
- `[DECISION]` Built-in prefab spawning proves standalone independence. Controllers, input, camera, UI, progression, inventory, interaction, saves, world spawn context, and multiplayer remain bridges/adapters.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 64 package-qualified Laboratory scenarios, and 384 individually registered planned tests. Every empirical result remains Not run.
- `[DECISION]` No SFGSS-000 revision is required because this specification refines the already-approved EchoCharacters authority and preserves the ownership matrix.
- `[HANDOFF]` SUITE-DOC-16 drafts The Vessel (`EchoControllers`) next.

**Promoted to:** The Fellowship (`EchoCharacters`) Package Specification v1.0.0, SUITE-DOC-15 audit report, README, Current Notes, and full-suite roadmap.

---

## Checkpoint Closeout Checklist - SUITE-DOC-15

- [x] Reconcile `Current Notes.md`.
- [x] Approve EchoCharacters ownership, independence, identity, roster, availability, selection, group, spawn, actor, control, switching, persistence, diagnostics, authoring, Laboratory, bridge, removal, and release contracts.
- [x] Register 64 Laboratory scenarios and 384 package-qualified planned tests.
- [x] Keep every unexecuted runtime, provider, platform, performance, compatibility, integration, migration, and release result `Not run`.
- [x] Update README, Current Notes, roadmap, audit report, and artifact manifest.
- [x] Confirm no package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider adapter, character definition, roster, spawner, control system, or gameplay implementation was created.
- [x] Record SUITE-DOC-14 as committed/pushed by the owner.
- [x] Commit and push SUITE-DOC-15 - confirmed by owner.
- [x] Stop before EchoControllers specification work.

---

## Handoff Snapshot - SUITE-DOC-15

**Completed checkpoint:** SUITE-DOC-15 - The Fellowship (`EchoCharacters`) Package Specification  
**Result:** Approved v1.0.0  
**Current focus:** EchoControllers - The Vessel  
**Active checkpoint:** SUITE-DOC-16 - EchoControllers Package Specification  
**Expansion specifications:** 11 of 13 approved  
**Package implementation:** Not started  
**First queued implementation:** FL-M1-01 - First Light Package Skeleton  
**Runtime authorization:** None  
**Known blockers:** None  
**Prior checkpoint:** SUITE-DOC-14 confirmed committed/pushed by owner  
**Commit/push:** SUITE-DOC-15 confirmed committed/pushed by owner  
**Stop point:** Before any package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider adapter, input adapter, motor, controller preset, locomotion capability, or gameplay implementation

---

## Checkpoint Closeout Checklist - SUITE-DOC-16

- [x] Reconcile `Current Notes.md`.
- [x] Approve EchoControllers ownership, rootless independence, normalized family intent, control/source leases, fixed-step physics boundary, Side-View 2D and Top-Down 2D MVP presets, capabilities, diagnostics, authoring, Laboratories, bridges, removal, and release contracts.
- [x] Register 68 Laboratory scenarios and 408 package-qualified planned tests.
- [x] Keep every unexecuted runtime, physics, input-adapter, controller, capability, platform, performance, compatibility, integration, migration, and release result `Not run`.
- [x] Update README, Current Notes, roadmap, audit report, and artifact manifest.
- [x] Confirm no package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider adapter, intent source, motor, controller preset, or gameplay implementation was created.
- [x] Record SUITE-DOC-15 as committed/pushed by the owner.
- [ ] Commit and push SUITE-DOC-16.
- [x] Stop before The Crucible design workshop or implementation work.

---

## Handoff Snapshot - SUITE-DOC-16

**Completed checkpoint:** SUITE-DOC-16 - The Vessel (`EchoControllers`) Package Specification  
**Result:** Approved v1.0.0  
**Current focus:** EchoCrafting - The Crucible design workshop and package specification  
**Active checkpoint:** SUITE-DOC-17 - EchoCrafting Design Workshop and Package Specification  
**Expansion specifications:** 12 of 13 approved  
**Package implementation:** Not started  
**First queued implementation:** FL-M1-01 - First Light Package Skeleton  
**Runtime authorization:** None  
**Known blockers:** None; the crafting workshop exists to resolve design questions before approval  
**Prior checkpoint:** SUITE-DOC-15 confirmed committed/pushed by owner  
**Commit/push:** SUITE-DOC-16 pending user confirmation  
**Stop point:** Before any package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider adapter, crafting recipe, ingredient provider, station, queue, transaction system, repair/salvage module, or gameplay implementation



## August 4, 2026 - SUITE-DOC-17 The Crucible approved

- `[DECISION]` The required EchoCrafting design workshop is complete and promoted to `Research Records/SUITE-DOC-17_EchoCrafting_Design_Workshop_Record.md`.
- `[DECISION]` The Crucible (`EchoCrafting`) Package Specification v1.0.0 is approved as the Level 2 authority for recipe definitions, Simple Combine and standard immediate crafting, deterministic preview, provider-neutral requirements, one-provider atomic resource transactions, idempotency, recipe knowledge, stations, diagnostics, authoring, validation, Laboratories, and extension seams.
- `[DECISION]` Hackulos's exact combine bag is represented as a strict Simple Combine recipe profile over the common transaction engine. The bag UI and resource container remain project/Vault concerns.
- `[DECISION]` One mutation-capable resource provider owns both input consumption and output grants for an MVP request. Arbitrary distributed mutation across unrelated providers is not promised.
- `[DECISION]` Multiple read-only requirement providers may evaluate skills, professions, tools, context, or project conditions without mutating their authorities.
- `[DECISION]` Preview is side-effect-free, deterministic, revision-aware, and fingerprinted. Execution revalidates stale resource/provider revisions.
- `[DECISION]` CraftingRequestId and bounded idempotency records prevent duplicate output grants.
- `[DECISION]` Recipe knowledge is crafting-specific state; broader progression, quest, dialogue, item, and world systems may trigger discovery through bridges.
- `[DECISION]` Timed jobs, queues, quality, failure, salvage, repair, upgrades, offline production, multi-provider coordination, and multiplayer adapters are explicit later capabilities, not hidden MVP scope.
- `[DECISION]` Live reservations, providers, stations, scene objects, and active tasks are not saved. Chronicle owns transport for detached knowledge/idempotency state.
- `[TEST]` The specification registers 74 package-qualified Laboratory scenarios and 444 individually registered planned tests. Every empirical result remains `Not run`.
- `[DECISION]` Expansion package specifications are now 13 of 13 approved.
- `[HANDOFF]` SUITE-DOC-18 begins The Convergence (`EchoMultiplayer`) research and provider-neutral foundation. Final provider approval remains blocked on executed disposable prototypes.

**Promoted to:** `Package Specifications/SFGSS-The-Crucible-EchoCrafting-Package-Specification.md`, `Research Records/SUITE-DOC-17_EchoCrafting_Design_Workshop_Record.md`, `Test Reports/SUITE-DOC-17_EchoCrafting_Package_Specification_Audit_Report.md`, README, and roadmap.

---

## Checkpoint Closeout Checklist - SUITE-DOC-17

- [x] Reconcile `Current Notes.md`.
- [x] Complete the required crafting design workshop.
- [x] Approve EchoCrafting ownership, independence, recipe, provider, transaction, knowledge, station, persistence, diagnostics, authoring, Laboratory, bridge, removal, and release contracts.
- [x] Preserve the exact combine bag without reducing the package to that mechanic.
- [x] Register 74 Laboratory scenarios and 444 package-qualified planned tests.
- [x] Keep every unexecuted runtime, provider, transaction, timed, quality, failure, repair, salvage, platform, performance, compatibility, integration, and release result `Not run`.
- [x] Update README, Current Notes, roadmap, workshop record, audit report, and artifact manifest.
- [x] Confirm no package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, provider, bridge, adapter, recipe asset, station, queue, or gameplay implementation was created.
- [x] Record SUITE-DOC-16 as committed/pushed by the owner.
- [ ] Commit and push SUITE-DOC-17.
- [x] Stop before EchoMultiplayer provider research execution or implementation.

---

## Handoff Snapshot - SUITE-DOC-17

**Completed checkpoint:** SUITE-DOC-17 - The Crucible (`EchoCrafting`) Design Workshop and Package Specification  
**Result:** Approved v1.0.0  
**Current focus:** EchoMultiplayer - The Convergence  
**Active checkpoint:** SUITE-DOC-18 - Research and Provider-Neutral Foundation  
**Expansion specifications:** 13 of 13 approved  
**Advanced foundations:** 0 of 5 approved  
**Package implementation:** Not started  
**First queued implementation:** FL-M1-01 - First Light Package Skeleton  
**Runtime authorization:** None  
**Known blocker:** Final networking provider approval requires executed prototype evidence  
**Prior checkpoint:** SUITE-DOC-16 confirmed committed/pushed by owner  
**Commit/push:** SUITE-DOC-17 pending user confirmation  
**Stop point:** Before any provider selection claim, network SDK install, package manifest, asmdef, C# file, scene, prefab, sample, adapter, bridge, prototype, or runtime implementation


## August 4, 2026 - SUITE-DOC-19 Instinct feasibility foundation approved

- `[DECISION]` Instinct (`EchoAI`) Feasibility Foundation v1.0.0 is approved as the Level 2 pre-code authority for actor-local AI coordination, semantic observations, bounded perception memory, target scoring, typed blackboards, decision scheduling, lightweight behavior/state-machine contracts, navigation request/status abstractions, diagnostics, validation, Laboratories, and adapter boundaries.
- `[DECISION]` EchoAI remains an Advanced candidate. This checkpoint does not authorize implementation, a mandatory navigation backend, a Unity Behavior dependency, neural inference, measured performance, or production compatibility.
- `[DECISION]` Agent authority is actor-local. Shared world registries and schedulers are explicit scene/world services rather than one persistent global AI manager.
- `[DECISION]` Definition/configuration assets remain immutable. Memory, confidence, target selection, blackboard values, behavior state, tickets, paths, schedules, and traces are runtime-owned and bounded.
- `[DECISION]` The core includes a lightweight state-machine and utility-selection foundation but explicitly rejects one universal enemy brain or mandatory behavior-tree architecture.
- `[DECISION]` Navigation technology remains optional. Unity AI Navigation, 2D/custom pathfinding, Unity Behavior, and Inference Engine connect through separately versioned adapters.
- `[DECISION]` Target selection uses deterministic filters, score contributions, thresholds, hysteresis, stable ties, seeded random sources, and explainable results.
- `[DECISION]` Live observations, provider handles, behavior actions, navigation tickets, and paths are not saved. Projects may opt into detached, versioned, safe-point durable snapshots.
- `[DECISION]` Multiplayer AI defaults to authoritative host/server decisions through The Convergence bridge; clients do not authoritatively commit shared AI outcomes.
- `[TEST]` The foundation contains all 30 SFGSS-001 sections, 80 package-qualified Laboratory scenarios, and 512 individually registered planned tests. Every empirical result remains `Not run`.
- `[RESEARCH]` The dated provider record reviews Unity AI Navigation 2.0.14, Unity Behavior 1.0.16, and Unity Inference Engine 2.6.1 as optional adapter candidates, not implementation pins.
- `[HANDOFF]` SUITE-DOC-20 drafts Clash (`EchoCombat`) next.

**Promoted to:** `Package Specifications/SFGSS-Instinct-EchoAI-Package-Foundation.md`, `Research Records/SUITE-DOC-19_EchoAI_Feasibility_and_Provider_Record.md`, SUITE-DOC-19 audit report, README, roadmap, and artifact manifest.

---

## Checkpoint Closeout Checklist - SUITE-DOC-19

- [x] Reconcile `Current Notes.md`.
- [x] Approve EchoAI ownership, independence, data, lifecycle, observation, memory, scoring, blackboard, scheduling, behavior, navigation, diagnostics, Laboratory, provider, bridge, removal, and release contracts.
- [x] Keep AI Navigation, Unity Behavior, custom navigation, and inference optional.
- [x] Register 80 Laboratory scenarios and 512 package-qualified planned tests.
- [x] Keep every unexecuted runtime, provider, navigation, behavior, inference, platform, performance, compatibility, integration, migration, and release result `Not run`.
- [x] Update README, Current Notes, roadmap, feasibility/provider record, audit report, and artifact manifest.
- [x] Confirm no package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, sensor, scorer, behavior, navigation provider, bridge, or gameplay implementation was created.
- [x] Record SUITE-DOC-18 as committed/pushed by the owner.
- [ ] Commit and push SUITE-DOC-19.
- [x] Stop before EchoCombat specification or implementation work.

---

## Handoff Snapshot - SUITE-DOC-19

**Completed checkpoint:** SUITE-DOC-19 - Instinct (`EchoAI`) Feasibility Foundation  
**Result:** Approved v1.0.0 feasibility foundation  
**Current focus:** EchoCombat - Clash  
**Active checkpoint:** SUITE-DOC-20 - EchoCombat Feasibility Foundation  
**Foundation specifications:** 10 of 10 approved  
**Expansion specifications:** 13 of 13 approved  
**Advanced foundations:** 2 of 5 approved  
**Package implementation:** Not started  
**First queued implementation:** FL-M1-01 - First Light Package Skeleton  
**Runtime authorization:** None  
**Known blockers:** None for documentation; all implementation and provider evidence remains Not run  
**Prior checkpoint:** SUITE-DOC-18 confirmed committed/pushed by owner  
**Commit/push:** SUITE-DOC-19 pending user confirmation  
**Stop point:** Before any package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, bridge, provider adapter, combat message, resolver, hit adapter, or gameplay implementation


## August 4, 2026 - SUITE-DOC-20 Clash feasibility foundation approved

- `[DECISION]` Clash (`EchoCombat`) Feasibility Foundation v1.0.0 is approved as the Level 2 pre-code authority for combat requests/results, source and target identity, targetability, relation seams, fixed-point magnitudes, pure modifier ordering, receiver prepare/commit transactions, outcomes, defeat/recovery events, bounded combat records, diagnostics, validation, Laboratories, and hit-adapter boundaries.
- `[DECISION]` Mutable health, shields, stats, durability, posture, and defeated state remain target/project-owned. EchoCombat never reaches into those models directly.
- `[DECISION]` Damage and healing are separate semantic operation kinds. Healing is not negative damage.
- `[DECISION]` Resolution modifiers are pure ordered transforms; target mutation occurs only through one explicit receiver transaction.
- `[DECISION]` Successful combat events publish after receiver commit. Listener failure cannot undo a committed result.
- `[DECISION]` Physics2D, Physics3D, abilities, AI, characters, equipment/stat rules, feedback, objectives, saves, and networking connect through optional adapters or project code.
- `[DECISION]` Area effects use per-target transactions connected by causality and batch IDs. The core does not promise cross-target atomic rollback.
- `[DECISION]` Multiplayer clients do not authoritatively apply shared combat. The selected Convergence/provider bridge validates evidence and submits authoritative requests.
- `[TEST]` The foundation contains all 30 SFGSS-001 sections, 84 package-qualified Laboratory scenarios, and 540 individually registered planned tests. Every empirical result remains `Not run`.
- `[HANDOFF]` SUITE-DOC-21 drafts Arcana (`EchoAbilities`) next.

**Promoted to:** `Package Specifications/SFGSS-Clash-EchoCombat-Package-Foundation.md`, `Research Records/SUITE-DOC-20_EchoCombat_Feasibility_and_Boundary_Record.md`, SUITE-DOC-20 audit report, README, roadmap, and artifact manifest.

---

## Checkpoint Closeout Checklist - SUITE-DOC-20

- [x] Reconcile `Current Notes.md`.
- [x] Approve EchoCombat ownership, identities, requests, targetability, relations, fixed-point magnitude, pure modifiers, receiver transactions, outcomes, events, diagnostics, Laboratories, bridges, removal, and release contracts.
- [x] Keep health/stats, abilities, AI, hit detection, presentation, save, and multiplayer transport outside the neutral core.
- [x] Register 84 Laboratory scenarios and 540 package-qualified planned tests.
- [x] Keep every unexecuted runtime, adapter, multiplayer, platform, performance, compatibility, integration, migration, and release result `Not run`.
- [x] Update README, Current Notes, roadmap, feasibility/boundary record, audit report, and artifact manifest.
- [x] Confirm no package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, target, receiver, resolver, hit adapter, bridge, or gameplay implementation was created.
- [x] Record SUITE-DOC-19 as committed/pushed by the owner.
- [ ] Commit and push SUITE-DOC-20.
- [x] Stop before EchoAbilities specification or implementation work.

---

## Handoff Snapshot - SUITE-DOC-20

**Completed checkpoint:** SUITE-DOC-20 - Clash (`EchoCombat`) Feasibility Foundation  
**Result:** Approved v1.0.0 feasibility foundation  
**Current focus:** EchoAbilities - Arcana  
**Active checkpoint:** SUITE-DOC-21 - EchoAbilities Feasibility Foundation  
**Foundation specifications:** 10 of 10 approved  
**Expansion specifications:** 13 of 13 approved  
**Advanced foundations:** 3 of 5 approved  
**Package implementation:** Not started  
**First queued implementation:** FL-M1-01 - First Light Package Skeleton  
**Runtime authorization:** None  
**Known blockers:** None for documentation; all implementation and provider evidence remains Not run  
**Prior checkpoint:** SUITE-DOC-19 confirmed committed/pushed by owner  
**Commit/push:** SUITE-DOC-20 pending user confirmation  
**Stop point:** Before any EchoAbilities package manifest, asmdef, C# file, ScriptableObject, scene, prefab, setup tool, sample, ability, targeting provider, effect executor, bridge, or gameplay implementation


---

## August 4, 2026 — SUITE-DOC-23 Expansion Collision Review

- `[DECISION]` SFGSS-INT-EXPANSION-001 v1.0.0 passes the thirteen-package collision review.
- `[DECISION]` The Ascent v1.1.0 limits completion records to progression definitions; The Path retains objective-run and step completion authority.
- `[DECISION]` Cross-package workflows have one commit owner and may not claim distributed rollback across authorities.
- `[DECISION]` One reusable bridge artifact exists per peer pair and behavior; mirror bridges are prohibited.
- `[DECISION]` ADR-001 v1.1.0 registers exact Workshop setup facades and minimum domains for all thirteen Expansion packages.
- `[DECISION]` UI focus, interaction focus, camera targets, selected characters, tracked objectives, input users, control owners, controller leases, and network participants remain qualified, separate identities.
- `[TEST]` Static audit found 13 unique package IDs and diagnostic prefixes, no duplicate documented assembly names, all required SFGSS-001 sections, and no implementation artifacts.
- `[TEST]` All runtime, bridge, provider, Laboratory, platform, performance, compatibility, migration, removal, and release evidence remains `Not run`.

**Promoted to:** SFGSS-000 v0.13.0, SFGSS-INT-EXPANSION-001, SFGSS-ADR-001 v1.1.0, The Ascent v1.1.0, SUITE-DOC-23 audit report.

## Handoff Snapshot — SUITE-DOC-23

**Completed checkpoint:** SUITE-DOC-23 — Expansion Cross-Package Collision Review  
**Result:** Approved after documentation repairs  
**Current focus:** Advanced Cross-Package and Research Review  
**Active checkpoint:** SUITE-DOC-24  
**Foundation specifications:** 10 of 10 approved  
**Expansion specifications:** 13 of 13 approved; The Ascent v1.1.0, other Expansion specifications v1.0.0  
**Advanced foundations:** 5 of 5 approved foundations  
**Package implementation:** Not started  
**Runtime authorization:** None  
**Known blockers:** None for documentation; provider/prototype evidence remains Not run  
**Commit/push:** SUITE-DOC-23 pending owner confirmation  
**Stop point:** Before any Advanced integration implementation, provider prototype, package manifest, asmdef, C# file, asset, scene, prefab, setup facade, bridge, or test execution


---

## August 4, 2026 — Suite graph roadmap and package learning gate

- `[DECISION]` The vault now maintains `Suite_Graph_Roadmap.md` as the primary Obsidian navigation and Graph View hub.
- `[DECISION]` Every current package specification links back to the graph roadmap through a standardized Graph Navigation block.
- `[DECISION]` The graph roadmap is navigation only and does not override SFGSS-000, package specifications, ADRs, standards, or integration specifications.
- `[DECISION]` Every package in Sections 7.1 through 7.3 receives an individual plain-language learning review before implementation is authorized.
- `[DECISION]` Each learning review includes purpose, analogy, practical example, owns/does-not-own boundary, definitions versus runtime state, lifecycle, public concepts, bridges, Standalone Laboratory, and a teach-back check.
- `[DECISION]` SUITE-DOC-33 now requires both documentation completion and all 28 package learning reviews.
- `[DECISION]` SFGSS-005 advances to v1.2.0 and ADR-003 records the graph and learning-review workflow.
- `[TEST]` The health check confirms 28 of 28 package foundations exist, Foundation and Expansion collision reviews pass, no architecture blocker is recorded, and empirical implementation evidence remains `Not run`.
- `[HANDOFF]` The active numbered checkpoint remains SUITE-DOC-24 — Advanced Cross-Package and Research Review.

**Promoted to:** `Suite_Graph_Roadmap.md`, `Package_Learning_Review_Catalog.md`, `Suite_Health_Check_and_Remaining_Documentation.md`, SFGSS-005 v1.2.0, ADR-003, README, and Full Suite Documentation Program Roadmap.
