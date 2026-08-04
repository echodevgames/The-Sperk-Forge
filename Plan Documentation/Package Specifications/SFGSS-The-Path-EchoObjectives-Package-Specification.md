# The Path - Objectives, Quests, and Tasks Package Specification

**Working document ID:** SFGSS-PKG-ECHOOBJECTIVES-001  
**Specification version:** 1.0.0  
**Status:** Approved  
**Technical package name:** EchoObjectives  
**Public title:** The Path - Objectives, Quests, and Tasks  
**Package ID:** `com.echodevgames.echo-objectives`  
**Runtime namespace:** `EchoDevGames.EchoObjectives`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoObjectives`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Last updated:** August 4, 2026

> “Mark the road clearly, measure every step honestly, and never confuse the destination with the hand that grants the reward.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoObjectives. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-005 and approved package authorities through Voices | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved objective identity, graph, progress, timer, repeatability, tracking, reward-ledger, persistence, diagnostics, authoring, Laboratory, integration, and release contracts | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Path - Objectives, Quests, and Tasks  
**Technical identifier:** EchoObjectives  
**Flavor line:** Mark the road clearly, measure every step honestly, and never confuse the destination with the hand that grants the reward.  
**Plain-language subtitle:** A standalone Unity package for project-authored objectives, quests, tasks, prerequisite graphs, deterministic runtime progress, timers, repeatable runs, tracked presentation snapshots, reward requests and ledgers, versioned state snapshots, diagnostics, authoring, and optional bridges.

**One-sentence ownership contract:**

> EchoObjectives owns project-authored objective definitions, stable objective/run/node identity, availability and prerequisite evaluation, one authoritative runtime registry of objective instances, sequential and parallel progress graphs, counter, flag, timer, manual, and provider-backed steps, optional and hidden behavior, repeatability, lifecycle transitions, tracked-objective selection, structured presentation snapshots, post-completion reward requests and delivery ledgers, versioned state export/import, diagnostics, authoring, validation, and optional bridge seams; it does not own the gameplay facts that create progress, dialogue rendering, inventory storage, progression grants, character state, scene travel, UI presentation, audio, save-file transport, multiplayer authority, or the implementation of rewards.

### 1.1 Elevator summary

The Path provides the reusable authority that answers four questions without borrowing another package's responsibilities: Which objectives are currently available? Which objective runs are active? What progress has each stable node made? Why did a run complete, fail, suspend, or become unavailable? Project adapters submit explicit, typed progress requests or expose read-only condition providers. EchoObjectives validates those requests, mutates one authoritative state model transactionally, recomputes group progress, publishes semantic events, and exposes presentation snapshots. It does not inspect arbitrary scene objects, subscribe to every gameplay event, or hide game rules in UI callbacks.

An objective definition is an immutable project-owned asset. Runtime state is a separate instance identified by an `ObjectiveRunId`, allowing repeatable objectives to create new runs without rewriting the definition or confusing old requests with new progress. Node records support ordered, all-required, any-required, and threshold groups. Leaf steps support manual completion, counters, flags, timers, and explicit provider snapshots. Optional and hidden are orthogonal policies: optional nodes do not block required completion, while hidden nodes affect presentation only and never alter underlying truth.

Completion and reward delivery are deliberately separated. The objective commits `Completed` once its authored requirements are satisfied. Each authored reward then produces a deterministic `RewardGrantId` and enters an independent delivery ledger. Inventory, progression, character, currency, or project reward executors may succeed, fail, become unavailable, or be retried without rolling back completed objective truth or granting the same reward twice. Chronicle may persist the versioned state through a bridge, but EchoObjectives remains usable with explicit in-memory export/import and no save package installed.

### 1.2 Why this belongs in The Sperk's Forge

Objective systems repeatedly become tangled with the concrete mechanics they observe and reward. Kill counters live in enemy scripts, quest completion lives in dialogue buttons, item turn-ins mutate inventory directly from UI, timers use whichever clock a developer happened to reach, and save files address steps by array index. A reusable package is justified because the repeated engineering need is stable identity, deterministic progress, lifecycle, validation, persistence seams, reward idempotency, and bridge boundaries, not one game's quest content.

| Source project or authority | Existing need or failure pattern | Preserve | Improve |
|---|---|---|---|
| Hackulos | Authored note delivery, rat-tail collection, bag-combine, placement, repeatable and follow-up quests | Concrete project goals and rewards | Stable objective graphs, explicit inventory/dialogue/crafting bridges, repeat-safe state |
| Rescuers2D | Rescue goals, survivor counts, role actions, timed hazards, and completion screens | Immediate readable progress | Move truth out of scene/UI scripts and expose neutral presentation snapshots |
| Echo Systems Lab | Mission definitions, target counts, completion records, and event-driven HUD | Definition/runtime/presentation separation | Generalize beyond one mission type and remove hard-coded scene/save assumptions |
| Voices | Dialogue may read objective state or request objective mutations | Explicit command/condition seams | Dialogue never becomes objective authority |
| The Ascent | Objective completion may grant unlocks or require progression | Atomic progression mutations | Separate objective truth from progression truth through reward/condition adapters |
| The Chronicle | Objective runs and reward ledgers require durable snapshots | Versioned participant model | Preserve unknown definitions/providers and safe prepared imports |
| SFGSS-003 | Stable IDs, aliases, migrations, and unknown-data retention | Durable identity discipline | Never use asset names, list indexes, or Unity asset GUIDs as runtime objective identity |
| SFGSS-004 | Planned tests must not masquerade as evidence | Complete pre-code registry | Keep implementation, compatibility, migration, and performance results `Not run` |

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title and documentation | Yes | “The Path” always appears beside the objectives, quests, and tasks responsibility |
| Setup headings and tooltips | Yes | Flavor may decorate route, milestone, and completion language while remaining immediately clear |
| Laboratory content | Optional | Sample objectives remain redistributable, removable, and clearly non-production |
| Runtime API/type names | No lore-only names | Use `ObjectiveDefinition`, `ObjectiveRunId`, `ObjectiveProgressRequest`, and direct technical names |
| Project quests and rewards | No required Verse content | Consumer projects own every title, description, condition, reward, signal, and narrative meaning |

---

## 2. Problem Statement

### 2.1 Current problem

Without one declared objective authority, projects commonly accumulate:

- objective truth split between enemies, inventory, dialogue, UI, and scene controllers;
- step progress addressed by array positions or display names;
- sequential and parallel requirements implemented as one-off booleans;
- optional and hidden steps that accidentally alter completion logic;
- repeated gameplay events that double-count progress;
- timers that use inconsistent clocks and cannot be restored safely;
- reward grants coupled directly to completion, causing either lost completion or duplicate rewards when another system fails;
- repeatable objectives that overwrite prior runs or allow stale requests to mutate a new run;
- UI widgets that become the only place tracked-objective state exists;
- save payloads that delete progress when an optional definition or provider is temporarily absent;
- no graph validation for cycles, unreachable nodes, impossible thresholds, or missing providers;
- no structured explanation of why an objective is locked, unavailable, failed, or awaiting reward delivery.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Hackulos vertical slice | Kill, collect, turn-in, combine, and place objectives span several systems | Exact authored quest structure | Neutral progress requests and reward/provider boundaries |
| Echo Systems Lab missions | Stable mission IDs and target completion events | Data-driven mission identity | Support groups, repeatability, timers, prerequisites, persistence, and optional steps |
| Rescuers2D | Rescue and role-specific tasks need HUD progress and level completion | Fast scene feedback | Keep scene and UI components as adapters rather than authorities |
| Foundation authorities | UI, dialogue, progression, save, scene, input, and diagnostics already have owners | One authority per concern | Define explicit bridge direction and teardown |
| SFGSS-002 to SFGSS-004 | Packages require visible dependencies, stable data, and evidence discipline | Suite-wide standards | Apply them to objective graphs, rewards, providers, and Laboratories |

### 2.3 Consequences of doing nothing

- Every game invents a new quest manager with incompatible semantics.
- Content authors cannot validate objective graphs before play.
- Save migration becomes fragile when steps are reordered or renamed.
- Reward failures either corrupt objective state or duplicate player gains.
- Dialogue, inventory, progression, and UI become circular dependencies.
- Repeatable objectives lose historical truth.
- The suite cannot offer reliable adventure, RPG, mission, or game-jam pathways.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one deterministic authority for objective availability, active runs, progress, lifecycle, tracking, and reward-delivery state.
- Keep objective definitions immutable and project-owned.
- Support sequential, parallel, threshold, optional, hidden, counter, flag, timer, manual, and provider-backed objective structures.
- Use stable package/domain IDs for objectives, nodes, rewards, providers, requests, and runs.
- Make all state mutations explicit, validated, transactional, and idempotency-aware.
- Separate completion truth from reward execution while preserving retry safety.
- Support non-repeatable and sequentially repeatable objective runs with bounded history.
- Expose structured presentation snapshots without owning production UI or localization.
- Export/import versioned state independently and integrate optionally with Chronicle.
- Provide complete authoring validation, diagnostics, a Standalone Laboratory, and a planned evidence registry.

### 3.2 Non-goals

- Observe arbitrary gameplay automatically or become a universal event bus.
- Render quest journals, HUD trackers, markers, dialogue, or reward screens.
- Store or transfer inventory items, currency, characters, or progression unlocks.
- Execute crafting, combat, interaction, camera, scene, or dialogue behavior.
- Own save files, slots, cloud sync, or platform storage.
- Provide online authoritative objective replication in the MVP.
- Provide a full visual scripting language or cinematic timeline.
- Guarantee real-world/offline timers before a dedicated policy and platform study.
- Replace project-specific narrative, encounter, mission, or reward design.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project | Create a catalog, import the Laboratory, and complete a sample objective without another Echo package |
| Gameplay programmer | Project events already exist | Translate semantic game events into typed progress requests without rewriting objective state logic |
| Designer/content author | Needs quests or mission goals | Author and validate structured objectives without editing runtime code |
| UI developer | Needs journal/HUD data | Consume immutable snapshots and issue tracking commands without owning truth |
| Save/integration developer | Needs durable progress | Export/import versioned state or register a Chronicle participant bridge |
| Tester | Needs reproducible failures | Simulate conditions, progress, timers, provider outages, reward failures, repeats, migrations, and limits in isolation |

### 3.4 Measurable success criteria

- Installs in a clean supported Unity project with zero compile errors.
- Compiles and runs without any peer Echo package.
- The Standalone Laboratory proves activation, progress, groups, timers, repeatability, tracking, rewards, failure, reset, and export/import.
- Duplicate roots are rejected before side effects.
- No runtime assembly references project code, Editor code, UI packages, or peer Echo packages.
- Every durable record uses stable IDs, versions, aliases, and unknown-data policy.
- Reward retries cannot duplicate a successful grant.
- Samples are removable without breaking runtime or Editor assemblies.
- All 268 planned tests remain traceable and honestly `Not run` until executed.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo developers and small teams building mission, quest, task, challenge, tutorial, adventure, RPG, puzzle, rescue, or game-jam systems.
- Content designers authoring objective structures and rewards.
- Programmers integrating gameplay signals, UI, dialogue, inventory, progression, saves, interaction, world, or multiplayer.
- QA testers validating deterministic objective state and recovery.
- Maintainers migrating definitions and durable objective data across releases.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| EOBJ-UC-001 | Evaluate availability | Project/service | Definition registered | Structured Available, Locked, or Unavailable result | MVP |
| EOBJ-UC-002 | Activate objective | Project/service | Availability permits activation | New run enters Active with stable run ID | MVP |
| EOBJ-UC-003 | Increment counter | Gameplay adapter | Active counter step | Transactional progress and semantic event | MVP |
| EOBJ-UC-004 | Set flag | Gameplay adapter | Active flag step | Target state updates and step may complete | MVP |
| EOBJ-UC-005 | Complete manual step | Project command | Active manual step | Step completes exactly once | MVP |
| EOBJ-UC-006 | Advance timer | Clock | Active timer step | Elapsed progress updates using authored clock policy | MVP |
| EOBJ-UC-007 | Evaluate provider step | Provider | Provider registered | Read-only structured progress snapshot | MVP |
| EOBJ-UC-008 | Run ordered objective | Runtime | Ordered group active | One child is active at a time | MVP |
| EOBJ-UC-009 | Run parallel objective | Runtime | All/Any/Threshold group active | Children progress independently under group policy | MVP |
| EOBJ-UC-010 | Use optional step | Designer/runtime | Optional child authored | Parent completion is not blocked | MVP |
| EOBJ-UC-011 | Hide/reveal step | Runtime/project | Visibility rule changes | Presentation visibility changes without altering truth | MVP |
| EOBJ-UC-012 | Suspend/resume objective | Project bridge | Active run | Lease-based suspension applies and releases safely | MVP |
| EOBJ-UC-013 | Fail/abandon objective | Project/service | Policy permits | Terminal state and reason are recorded | MVP |
| EOBJ-UC-014 | Repeat objective | Player/project | Repeat policy permits | New run ID created; prior history retained | MVP |
| EOBJ-UC-015 | Track objective | Player/UI adapter | Objective known | Primary/pinned tracking state changes | MVP |
| EOBJ-UC-016 | Complete objective | Runtime | Required graph satisfied | Completion commits once | MVP |
| EOBJ-UC-017 | Deliver reward | Reward executor | Completed run and pending grant | Grant ledger reaches success/failure state | MVP |
| EOBJ-UC-018 | Retry failed reward | Project/UI | Failed grant | Same grant ID retries safely | MVP |
| EOBJ-UC-019 | Export/import state | Project/save adapter | Service Ready | Versioned state round-trip | MVP |
| EOBJ-UC-020 | Author/validate objective | Designer | Editor installed | Stable definition and actionable report | MVP |
| EOBJ-UC-021 | Present journal/HUD | Looking Glass bridge | Both packages installed | UI consumes snapshots and sends commands | Later bridge |
| EOBJ-UC-022 | Use localized text | Many Tongues bridge | Both packages installed | Text references resolve without core dependency | Later bridge |
| EOBJ-UC-023 | Read/set through dialogue | Voices bridge | Both packages installed | Conditions read; explicit commands request mutations | Later bridge |
| EOBJ-UC-024 | Persist through Chronicle | Chronicle bridge | Both packages installed | Versioned participant payload persists | Later bridge |
| EOBJ-UC-025 | Deliver item reward | Inventory bridge | Executor registered | Inventory authority applies one idempotent grant | Later bridge |
| EOBJ-UC-026 | Deliver unlock reward | Ascent bridge | Executor registered | Progression authority applies one mutation batch | Later bridge |
| EOBJ-UC-027 | Receive interaction progress | Hand adapter | Semantic interaction occurs | Adapter submits explicit progress request | Later bridge |
| EOBJ-UC-028 | Validate before build | Foundry bridge | Both packages installed | Catalog and migration validation joins preflight | Later bridge |
| EOBJ-UC-029 | Compose through Workshop | Workshop | Setup facade available | Dry-run plan creates project-owned configuration/sample choices | Later bridge |
| EOBJ-UC-030 | Replicate authoritative objectives | Multiplayer adapter | Advanced authority approved | Provider validates and replicates semantic mutations | Advanced |

### 4.3 Explicitly unsupported use cases

- Treating UI checkbox state as objective truth.
- Discovering progress by scanning every scene object or subscribing to arbitrary reflection-based events.
- Using objective titles, asset names, or list indexes as save identity.
- Granting inventory, progression, character, or currency rewards inside the neutral core.
- Rolling back objective completion because an external reward provider failed.
- Concurrent runs of the same repeatable objective in the MVP.
- Real-money, entitlement, security, or anti-cheat reward authority.
- Offline real-time timers without a separately approved policy/provider.
- Cross-player or server-authoritative state without EchoMultiplayer research and adapters.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Objective catalogs, definitions, node/group/step records, prerequisites, repeat and tracking policies.
- Objective availability evaluation and structured lock/unavailable reasons.
- Runtime objective-run registry and lifecycle state.
- Node progress, group aggregation, timers, suspension, and bounded histories.
- Explicit progress mutation validation, batching, idempotency window, and events.
- Primary tracked objective and bounded pinned objective state.
- Objective completion truth.
- Provider-neutral reward definitions, deterministic grant IDs, dispatch state, retries, and ledgers.
- Versioned objective-state export/import, migrations, aliases, and orphan preservation.
- Package diagnostics, authoring, validation, setup, repair, and Standalone Laboratory.

### 5.2 The package does not own

- Gameplay events, combat kills, item pickups, interactions, crafting results, scene entry, or dialogue choices that earn progress.
- Production quest journal, HUD, map markers, notifications, or menu navigation.
- Translation tables, localized content, fonts, or text formatting.
- Inventory, currency, progression, character, ability, or crafting state.
- Dialogue traversal or conversation UI.
- Scene loading, camera motion, game-state policy, input maps, audio playback, or feedback effects.
- General save files, slots, cloud storage, or platform synchronization.
- Multiplayer validation or replication in the MVP.
- Analytics, achievements, entitlements, commerce, or anti-cheat.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoObjectives interacts |
|---|---|---|
| Objective progress/lifecycle | EchoObjectives | Direct public API |
| Production objective UI | EchoUI/project | Immutable snapshots and command results through bridge |
| Text localization | EchoLocalization | Provider-neutral text references through bridge |
| Dialogue flow | EchoDialogue | Read-only condition provider and explicit command handlers |
| Item/container truth | EchoInventory | Condition and reward adapters; no storage access in core |
| Unlock/checkpoint progression | EchoProgression | Condition and reward adapters |
| Save files/slots | EchoSave | Versioned participant bridge |
| World interactions | EchoInteraction/project | Semantic progress adapters |
| Scene travel | EchoSceneFlow/project | Optional location/route adapters; core never loads |
| Runtime state/time policy | EchoGameState/project | Optional suspension/clock adapters |
| Diagnostics dashboard | EchoDiagnostics | Redacted status provider bridge |
| Build validation | EchoBuildTools | Editor validator integration |
| Starter generation | EchoGameStarter | ADR-001 setup facade |
| Multiplayer authority | EchoMultiplayer/provider | Advanced authoritative adapter |
| Concrete game rules | Project code | Explicit conditions, progress requests, and reward executors |

### 5.4 Boundary tests

For every proposed feature:

1. Does it change objective truth or only observe/present/request it?
2. Can the core remain useful if the neighboring package is removed?
3. Does the feature require project-specific gameplay knowledge?
4. Is a condition provider, progress adapter, reward executor, or bridge sufficient?
5. Would adding it cause the core to store another authority's mutable state?
6. Can failure be reported without pretending another system succeeded?
7. Does the data require stable identity or migration under SFGSS-003?
8. Can the behavior be proven in the Standalone Laboratory without unrelated packages?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

EchoObjectives must:

- compile with only declared Unity dependencies;
- initialize without First Light, Observatory, UI, Localization, Dialogue, Inventory, Progression, Save, Interaction, or Multiplayer;
- use source labels and fake providers in its Laboratory;
- keep project definitions outside immutable package source;
- expose `IEchoObjectivesService` for injection and testing;
- reject duplicates before subscriptions, clocks, providers, or state mutations;
- fail visibly and safely when optional providers are absent;
- preserve unknown durable records instead of deleting them;
- permit sample removal without runtime breakage;
- provide development direct-scene initialization without creating a second production bootstrap.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Planned evidence |
|---|---|---|
| Installed alone | Runtime, Editor, and tests compile | Clean-project install tests |
| Laboratory entered directly | Development initializer creates one authority | EOBJ-LAB-003 |
| Optional bridge absent | Core uses source labels/fake providers or returns Unavailable | Provider absence tests |
| Duplicate root present | Duplicate rejected before side effects | EOBJ-LAB-002 |
| Missing configuration | Initialization fails with actionable result | Lifecycle tests |
| Empty catalog | Ready with advisory and no active objectives | EOBJ-LAB-005 |
| Sample deleted | Runtime and Editor remain intact | Installation tests |
| Definition/provider temporarily missing | Durable records remain orphaned/pending | Persistence tests |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Planned minimum | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity engine core | Platform | Yes | Unity 6000.0 | MonoBehaviour, ScriptableObject, serialization, Time, scene lifecycle | Package cannot run without Unity |
| Unity Test Framework | Test only | Yes for package tests | Verify at implementation | EditMode and PlayMode tests | Runtime unaffected if tests excluded |
| UI Toolkit Editor APIs | Editor only | Yes | Unity baseline | Authoring/setup/validation windows | Runtime unaffected |

No peer Echo package is a core dependency.

### 6.4 Forbidden dependencies

- Project assemblies or project-specific databases.
- Any peer Echo runtime package in the neutral core.
- Runtime references to `UnityEditor`.
- Samples or test assemblies as runtime dependencies.
- Hidden scene names, build indexes, tags, layers, input maps, or Resources paths.
- Reflection-discovered condition, reward, or progress handlers.
- Mutable runtime state inside shared ScriptableObjects.
- Unlicensed or non-redistributable sample content.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
|---|---|---|---|---:|---|
| EOBJ-CAP-001 | Duplicate-safe authority | One runtime root and injectable service | Approved | Yes | Runtime |
| EOBJ-CAP-002 | Catalog/definitions | Stable project-owned objective data | Approved | Yes | Runtime/Editor |
| EOBJ-CAP-003 | Availability | Built-in and provider prerequisites with reasons | Approved | Yes | Runtime |
| EOBJ-CAP-004 | Run lifecycle | Activate, suspend, resume, complete, fail, abandon, archive | Approved | Yes | Runtime |
| EOBJ-CAP-005 | Graph groups | Ordered, All, Any, Threshold | Approved | Yes | Runtime/Editor |
| EOBJ-CAP-006 | Leaf steps | Manual, Counter, Flag, Timer, Provider | Approved | Yes | Runtime/Editor |
| EOBJ-CAP-007 | Optional/hidden | Independent completion and presentation policies | Approved | Yes | Runtime |
| EOBJ-CAP-008 | Transactional progress | Typed requests, batches, dedupe, results | Approved | Yes | Runtime |
| EOBJ-CAP-009 | Repeatability | Sequential repeated runs and bounded history | Approved | Yes | Runtime |
| EOBJ-CAP-010 | Tracking | One primary plus bounded pinned objectives | Approved | Yes | Runtime |
| EOBJ-CAP-011 | Reward ledger | Stable grants, independent delivery, retry | Approved | Yes | Runtime |
| EOBJ-CAP-012 | State documents | Versioned export/import, aliases, orphan retention | Approved | Yes | Runtime |
| EOBJ-CAP-013 | Diagnostics | Structured status, codes, redacted snapshots | Approved | Yes | Runtime/Editor |
| EOBJ-CAP-014 | Authoring/validation | Setup, definition editor, reports, repair | Approved | Yes | Editor |
| EOBJ-CAP-015 | Laboratory | Standalone simulated objective workflow | Approved | Yes | Sample |
| EOBJ-CAP-016 | Chronicle bridge | Save participant integration | Approved | No | Bridge |
| EOBJ-CAP-017 | UI/localization bridges | Presentation and text resolution | Approved | No | Bridge |
| EOBJ-CAP-018 | Dialogue bridge | Conditions and explicit mutations | Approved | No | Bridge |
| EOBJ-CAP-019 | Inventory/progression bridges | Conditions and reward executors | Approved | No | Bridge |
| EOBJ-CAP-020 | Multiplayer adapter | Authoritative replication/validation | Deferred | No | Advanced adapter |

### 7.2 MVP capability set

The smallest complete release includes:

- one duplicate-safe root and injectable service;
- project-owned configuration, catalog, and definitions;
- stable objective, node, reward, provider, request, and run IDs;
- availability with built-in prerequisites and explicit providers;
- one active run per definition;
- ordered, all, any, and threshold groups;
- manual, counter, flag, timer, and provider steps;
- optional and hidden policies;
- transactional progress requests and bounded dedupe;
- suspension leases;
- non-repeatable and sequential repeatable runs;
- one primary tracked objective plus bounded pins;
- completion-first reward ledgers with stable grant IDs;
- versioned export/import and orphan preservation;
- diagnostics, authoring, validation, setup/repair, tests, and Laboratory.

### 7.3 Later capability set

- Concurrent runs of one definition.
- Real-world/offline timer providers.
- Objective templates/macros and reusable subgraphs.
- Shared/community objectives.
- Dynamic objective generation providers.
- Map-marker and world-location presentation adapters.
- Rich editor graph visualization beyond the MVP structured authoring window.
- Multiplayer authority and replication.
- Platform achievement and analytics adapters.
- Cross-objective transactional reward bundles after real integration evidence.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Automatic scene-wide event discovery | Rejected | Hidden coupling and nondeterministic ownership | Never in neutral core |
| Inventory storage inside objectives | Rejected | Owned by The Vault | Inventory bridge |
| Dialogue rendering inside objectives | Rejected | Owned by Voices/Looking Glass | Dialogue/UI bridge |
| Roll back completion when reward fails | Rejected for MVP | Cross-authority rollback is unsafe | Approved transactional provider protocol |
| Concurrent repeat runs | Deferred | Complicates identity, tracking, UI, and rewards | Concrete project need |
| Offline timers | Deferred/provider | Clock, cheating, platform, and persistence policy | Dedicated provider design |
| Dynamic procedural quests | Deferred/provider | Project/generator concern | Repeated cross-project evidence |
| Full visual scripting | Rejected | Scope explosion and maintenance burden | Separate product decision |
| Online shared quests | Deferred | Multiplayer authority required | EchoMultiplayer provider evidence |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Catalogs, objective/node/reward records, prerequisite trees, policies, text references, aliases | Active progress, timers, run IDs, reward results, scene objects |
| Runtime state/behavior | Root, service, run registry, node state, mutation engine, clocks, tracking, reward ledger, migrations | Editor APIs, production UI, inventory/progression state |
| Presentation/feedback | Immutable snapshots, sample presenter, optional bridges | Authoritative objective mutation or reward execution |

### 8.2 Component topology

```text
EchoObjectivesRoot
├── ObjectiveCatalogIndex
├── ObjectiveAvailabilityEvaluator
├── ObjectiveRunRegistry
│   └── ObjectiveRunState
│       ├── NodeStateRegistry
│       ├── SuspensionLeases
│       ├── MutationDedupeWindow
│       └── RewardDeliveryLedger
├── ObjectiveMutationEngine
├── ObjectiveClockRegistry
├── ObjectiveConditionProviderRegistry
├── ObjectiveProgressProviderRegistry
├── ObjectiveRewardExecutorRegistry
├── ObjectiveTrackingState
├── ObjectiveStateSerializer/Migrator
├── ObjectiveEventHub
└── ObjectiveDiagnosticsState

Project adapters/bridges
├── submit explicit progress requests
├── expose read-only conditions/progress snapshots
├── execute typed reward grants
└── consume immutable presentation snapshots/events
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root required? | Yes for the normal application-session authority |
| Root type | `EchoObjectivesRoot` |
| Convenience access | Documented optional access plus injectable `IEchoObjectivesService` |
| Duplicate behavior | First valid configured root wins; duplicates are rejected before side effects |
| Initialization trigger | Explicit `Initialize` from Awake/First Light integration or documented standalone path |
| Default lifetime | Application session |
| Shutdown | Cancel pending reward work where safe, dispose providers/leases, clear runtime state, unsubscribe |
| Direct-scene behavior | Development initializer creates only the missing configured root and marks the session |
| Test injection seam | Service, clocks, condition/progress/reward providers, ID generator, and state backend interfaces |

### 8.4 Lifecycle sequence

1. Claim authority and reject duplicates.
2. Validate configuration, limits, catalogs, IDs, definitions, aliases, and schemas.
3. Build immutable indexes.
4. Initialize clocks, provider registries, tracking, diagnostics, and empty run state.
5. Enter Ready and expose availability results.
6. Accept activation, lifecycle, tracking, progress, provider, and reward operations.
7. Export/import versioned snapshots when requested.
8. On shutdown, stop accepting work, cancel safe asynchronous reward operations, dispose registrations and leases, and release authority.

### 8.5 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Code family |
|---|---|---|---|---|
| Missing configuration | Initialize | Blocking report | Do not publish Ready | `EOBJ-CFG-*` |
| Duplicate root | Claim | Warning/error | Destroy or disable duplicate before side effects | `EOBJ-AUTH-*` |
| Invalid definition graph | Validation/index | Definition blocked | Other valid definitions remain usable | `EOBJ-DEF-*` |
| Missing condition/progress provider | Evaluation | Unavailable result | No implicit pass | `EOBJ-PROV-*` |
| Invalid/stale progress request | Mutation | Structured rejection | No state change | `EOBJ-MUT-*` |
| Capacity exceeded | Activation/tracking/history | Structured rejection | Existing state preserved | `EOBJ-CAP-*` |
| Clock failure | Timer update | Step unavailable or suspended per policy | Other steps continue | `EOBJ-TIME-*` |
| Reward executor missing/fails | Delivery | Grant remains unavailable/failed | Objective stays Completed | `EOBJ-RWD-*` |
| Import/migration failure | Prepare/apply | Import rejected | Live state and source remain intact | `EOBJ-DATA-*` |
| Event/diagnostics listener throws | Publication | Logged diagnostic | Objective truth remains committed | `EOBJ-OBS-*` |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoObjectivesConfiguration` | Limits, policies, clocks, catalogs, diagnostics, tracking, history | Configuration ID | No | Yes |
| `ObjectiveCatalog` | Ordered set of objective definitions | Catalog ID | No | Yes |
| `ObjectiveDefinition` | Objective identity, metadata, graph, prerequisites, repeat, reward, tracking policies | `ObjectiveId` | No | Yes |
| `ObjectiveNodeRecord` | Stable group or leaf record | `ObjectiveNodeId` | No | Inside definition |
| `ObjectiveGroupRecord` | Child ordering and All/Any/Threshold policy | Node ID | No | Inside definition |
| `ObjectiveStepRecord` | Manual/counter/flag/timer/provider progress policy | Node ID | No | Inside definition |
| `ObjectivePrerequisiteSet` | Built-in and provider condition tree | Condition IDs | No | Inside definition |
| `ObjectiveRewardDefinition` | Stable reward type and opaque payload | `ObjectiveRewardId` | No | Inside definition |
| `ObjectiveTextReferences` | Provider-neutral title, description, and step references plus source fallback | Definition/node IDs | No | Inside definition |
| `ObjectiveAliasRegistry` | Old-to-current objective/node/reward/provider IDs | Alias IDs | No | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization |
|---|---|---|---|---|
| `ObjectiveRunState` | Service | Activation to terminal/prune | New run ID per repeat | Yes |
| `ObjectiveNodeState` | Run | Run lifetime | Definition default per new run | Yes |
| `ObjectiveTimerState` | Run/node | Active run | Reset per run | Yes |
| `ObjectiveSuspensionLeaseState` | Service | Lease lifetime | Cleared on shutdown; normally not persisted | No by default |
| `ObjectiveMutationDedupeState` | Run | Bounded request window | Per run, bounded | Yes for active runs |
| `ObjectiveTrackingState` | Service | Session/profile | Explicit clear/import | Yes |
| `ObjectiveRewardGrantState` | Run | Completion through prune | Stable grant ID; never reset after success | Yes |
| `ObjectiveProviderRegistration` | Service | Registration lifetime | Dispose/unregister | No |
| `ObjectiveEventHistory` | Diagnostics | Bounded session history | Clear/reset | No by default |
| `ObjectiveOrphanRecord` | State document | Until definition returns or explicit prune | Preserve | Yes |

### 9.3 Stable identifiers

Required domain IDs:

- `ObjectiveId`
- `ObjectiveNodeId`
- `ObjectiveRewardId`
- `ObjectiveProviderTypeId`
- `ObjectiveConditionId`
- `ObjectiveRunId`
- `ObjectiveProgressRequestId`
- `ObjectiveRewardGrantId`
- `ObjectiveClockId`

Rules:

- IDs are opaque, serialization-safe strings or strongly typed wrappers.
- Editor creation generates IDs; runtime never derives them from names, paths, or indexes.
- Unity asset GUIDs remain Editor source identity only.
- Run IDs are runtime/durable instance IDs and never replace definition IDs.
- Reward grant IDs are deterministic from run identity plus reward identity.
- Aliases support renamed/replaced durable IDs; collisions and chains are validated.
- Deleted definitions may use tombstones or preserved orphan records rather than silent deletion.

### 9.4 Objective graph and progress model

Each definition contains one acyclic rooted graph. Nodes are explicit records, not reflection-created subclasses discovered at runtime.

**Group policies:**

- `Ordered`: one incomplete eligible child active at a time.
- `AllRequired`: all required children must complete.
- `AnyRequired`: any eligible required child completes the group; remaining children use an authored closure policy.
- `Threshold`: a configured number of eligible required children must complete.

**Leaf progress policies:**

- `Manual`: explicit complete/fail request.
- `Counter`: current numeric value versus target, with authored set/increment/regression/clamp rules.
- `Flag`: current Boolean/enum-like target value using a bounded objective value type.
- `Timer`: elapsed duration using an approved clock policy.
- `Provider`: read-only structured snapshot from one explicitly registered provider type.

`Optional` affects whether a child blocks group/objective completion. `Hidden` affects standard presentation snapshots only. Neither silently mutates the other.

### 9.5 Objective lifecycle model

Availability and run lifecycle are distinct:

**Availability:** `Available`, `Locked`, `Unavailable`, `AlreadyActive`, `CompletedNonRepeatable`, `Cooldown`, `RepeatLimitReached`.

**Run lifecycle:** `Active`, `Suspended`, `Completing`, `Completed`, `Failed`, `Abandoned`, `Archived`, `Orphaned`.

- Availability evaluation is read-only.
- Activation creates a new run and commits it atomically.
- `Completing` is a short internal publication boundary while final graph state and reward ledger are created; external presentation normally observes Completed plus reward states.
- Reward failure never changes Completed back to Active or Failed.
- Concurrent runs for one definition are not supported in the MVP.

### 9.6 Reward ledger model

Completion creates one `ObjectiveRewardGrantState` per authored reward:

- `Pending`
- `InProgress`
- `Succeeded`
- `Failed`
- `Unavailable`
- `Skipped`

Every request includes a deterministic grant ID, reward type ID, opaque payload, objective/run context, and attempt number. Executors must report whether an irreversible commit point was crossed. A succeeded grant cannot be dispatched again. Automatic retries are bounded; manual/project retries preserve the same grant ID.

### 9.7 ScriptableObject safety

Definition and configuration assets must never store:

- current progress;
- active run IDs;
- timers;
- tracked selection;
- provider registrations;
- reward results;
- dedupe history;
- scene-object references;
- save-loaded mutable state.

Editor previews use temporary models or clones and must not dirty package/default definition assets accidentally.

### 9.8 Serialization and migration

The state document includes:

- document schema version;
- package/state revision;
- objective run records;
- node progress and timer records;
- tracking state;
- bounded dedupe state for active runs;
- reward ledgers;
- terminal history retained by policy;
- orphan/unknown records;
- content aliases/migration provenance.

Migrations are contiguous, deterministic, test-fixture-driven, and non-destructive. Prepared imports validate completely before application. A stale prepared import cannot overwrite newer live state. Newer unsupported documents are rejected without mutation.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Ownership |
|---|---|---|---|
| `EchoObjectivesRoot` | MonoBehaviour | Duplicate-safe package root | Scene/prefab/project |
| `IEchoObjectivesService` | Interface | Main objective authority API | Root exposes; injectable |
| `ObjectiveDefinition` | ScriptableObject | Immutable objective design | Project |
| `ObjectiveCatalog` | ScriptableObject | Definition registry | Project |
| `ObjectiveId`, `ObjectiveRunId`, `ObjectiveNodeId` | Value types | Stable identity | Package contracts |
| `ObjectiveAvailabilityResult` | Struct | Availability plus reasons | Service result |
| `ObjectiveActivationRequest/Result` | Structs | Validated run creation | Caller/service |
| `ObjectiveProgressRequest` | Struct | Typed node mutation | Caller |
| `ObjectiveMutationBatch/Result` | Structs | Atomic multi-operation mutation | Caller/service |
| `ObjectiveRunSnapshot` | Immutable DTO | Full semantic run snapshot | Service |
| `ObjectivePresentationSnapshot` | Immutable DTO | UI/localization-friendly view | Service |
| `ObjectiveTrackingSnapshot` | Immutable DTO | Primary/pinned tracking state | Service |
| `ObjectiveSuspensionHandle` | Struct/handle | Reason-based suspension lease | Service |
| `ObjectiveRewardRequest/Result` | DTOs | Typed external reward delivery | Service/executor |
| `ObjectiveStateDocument` | DTO | Versioned durable state | Service/project |
| `ObjectivePreparedImport` | Handle/model | Validated unapplied state | Service |
| `IObjectiveConditionProvider` | Interface | Read-only prerequisite evaluation | Project/bridge |
| `IObjectiveProgressProvider` | Interface | Read-only provider-step snapshot | Project/bridge |
| `IObjectiveRewardExecutor` | Interface | Typed idempotent reward delivery | Project/bridge |
| `IObjectiveClockProvider` | Interface | Project-selected timer clock | Project/bridge |
| `ObjectiveProviderRegistration` | Disposable handle | Explicit provider lifetime | Service |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure | Main-loop rule |
|---|---|---|---|---|
| `Initialize(configuration)` | Validate and enter Ready | Authority claimed | Structured init result | Main thread |
| `GetAvailability(objectiveId, context)` | Read availability | Ready | Available/Locked/Unavailable with reasons | Main thread, synchronous |
| `Activate(request)` | Create a run | Available and capacity | Activation result/run ID | Main thread, transactional |
| `SubmitProgress(request)` | Mutate one node | Active run and valid request | Applied/rejected/deduplicated | Main thread, transactional |
| `ApplyBatch(batch)` | Mutate several nodes/lifecycle values atomically | Valid revision | Full success or no change | Main thread |
| `Suspend(runId, reason)` | Acquire suspension lease | Active run | Handle or failure | Main thread |
| `Complete/Fail/Abandon` | Explicit lifecycle request | Policy permits | Structured result | Main thread |
| `Track(objectiveId)` | Set primary tracked objective | Known objective/run | Tracking result | Main thread |
| `Pin/Unpin(objectiveId)` | Manage bounded pins | Known objective/run | Tracking result | Main thread |
| `RetryReward(grantId)` | Retry eligible grant | Completed run and grant retryable | Awaitable result | Main thread entry; executor async |
| `GetRunSnapshot(runId)` | Read immutable state | Run known | Snapshot/not found | Main thread |
| `GetPresentationSnapshot(objectiveId/runId)` | Read display-neutral state | Definition/run known | Snapshot/not found | Main thread |
| `ExportState()` | Create detached document | Ready | Versioned snapshot | Main thread capture |
| `PrepareImport(document)` | Validate/migrate without mutation | Ready | Prepared import/failure | Main thread; detached pure work may be off-thread later |
| `ApplyImport(prepared)` | Replace state atomically | Prepared revision current | Success/failure | Main thread |
| `RegisterCondition/Progress/Reward/ClockProvider(provider)` | Explicit integration | Unique type ID | Disposable registration | Main thread |
| `Shutdown()` | Stop service safely | Any initialized state | Completion report | Main thread |

### 10.3 Events and callbacks

| Event | Timing | Payload | Listener assumptions |
|---|---|---|---|
| `AvailabilityChanged` | After cached result changes | Objective ID, old/new results | Listener failure isolated |
| `ObjectiveActivated` | After run commit | Run snapshot | Snapshot immutable |
| `ObjectiveProgressed` | After mutation commit | Run/node delta and snapshots | No presentation listener required |
| `ObjectiveLifecycleChanged` | After state commit | Old/new lifecycle and reason | Exactly once per transition |
| `ObjectiveTrackingChanged` | After tracking commit | Tracking snapshot | UI may rebuild safely |
| `ObjectiveCompleted` | After completion and reward-ledger creation | Completed run snapshot | Rewards may still be pending |
| `RewardDeliveryChanged` | After grant state changes | Grant snapshot/result | Does not alter completion truth |
| `StateImported` | After atomic import | New service revision/summary | Listeners query fresh snapshots |
| `ProviderAvailabilityChanged` | After registration changes | Provider type/status | Affected definitions reevaluate lazily/bounded |

Events are raised after authoritative state changes. No listener is required for completion.

### 10.4 Async and cancellation policy

- Availability, activation, progress, tracking, lifecycle, snapshot, and export operations are synchronous main-thread transactions in the MVP.
- Reward executors may be asynchronous and use fresh Unity `Awaitable` instances or a documented abstraction approved during implementation.
- Cancellation is honored before an executor-declared irreversible commit point.
- After commit, cancellation changes waiting/presentation behavior but cannot pretend the external reward did not occur.
- Timeouts produce Failed or Unavailable grant states according to executor result.
- Shutdown stops new work and cancels safe pending work; committed reward operations finish/reconcile through their executor contract.
- No public API promises thread safety in the MVP.

### 10.5 API ergonomics

**Novice path:** setup tool creates configuration/catalog/root; import Laboratory; use sample controller buttons to activate and progress a sample objective.

**Programmer path:** inject `IEchoObjectivesService`, submit typed progress requests, register explicit providers/executors, consume immutable snapshots/events, and integrate persistence through export/import or a Chronicle bridge.

Convenience access is never the only testable path.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install the package.
2. Open **Tools > EchoDevGames > The Path > Setup**.
3. Select or create a project-owned output folder.
4. Preview creation of configuration, catalog, root prefab, optional Boot placement, and Laboratory import guidance.
5. Apply create-only-safe operations.
6. Open the Objective Authoring window.
7. Create or add objective definitions.
8. Validate catalogs, graphs, providers, aliases, migrations, and build readiness.
9. Open the Standalone Laboratory and run the acceptance checklist.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeat safe | Undo/backup | Report |
|---|---|---|---:|---|---|
| Create configuration/catalog | Project assets | Nothing existing | Yes | Undo/create receipt | Setup report |
| Create root prefab | Project prefab | Nothing existing | Yes | Undo/create receipt | Setup report |
| Add root to Boot scene | Scene object | Selected scene | Yes, duplicate-aware | Scene undo | Setup report |
| Repair references | Missing assignments only | Explicit selected assets | Yes | Preview and undo | Repair report |
| Register definition | Catalog entry | Selected catalog | Yes, ID-aware | Undo | Validation report |
| Generate/repair IDs | Empty/colliding IDs | Selected project assets | Only with preview | Backup/undo | ID report |
| Generate aliases/migration map | Project-owned mapping | New/selected alias registry | Yes | Backup/undo | Migration report |
| Workshop apply | Project assets/facade receipt | Approved plan targets only | Yes under ADR-001 | Operation receipts | Setup facade receipt |

### 11.3 Objective authoring window

The MVP authoring window uses a structured UI Toolkit list/tree editor rather than depending on an experimental graph package. It provides:

- catalog and definition browser;
- stable ID display and regenerate-with-confirmation tools;
- root node and child hierarchy editing;
- Ordered, All, Any, and Threshold group settings;
- Manual, Counter, Flag, Timer, and Provider step forms;
- optional, hidden, failure, closure, repeat, tracking, and reward policies;
- prerequisite tree authoring;
- source fallback and provider-neutral text references;
- reward type/payload editing through registered Editor drawers;
- validation badges and report navigation;
- read-only runtime state inspection in Play Mode;
- no mutable runtime values written into definition assets.

A richer graph canvas may be added later without changing runtime records.

### 11.4 Validation and repair

| Check family | Examples | Severity | Auto-fix policy |
|---|---|---|---|
| `EOBJ-VAL-ID-*` | Empty/duplicate IDs, alias loops/collisions | Error/Blocker | Generate only with explicit preview |
| `EOBJ-VAL-GRAPH-*` | Cycles, missing children, unreachable nodes, impossible thresholds | Error/Blocker | No silent graph rewrite |
| `EOBJ-VAL-PROV-*` | Missing provider/reward types, duplicate registrations | Warning/Error | Registration guidance only |
| `EOBJ-VAL-TIME-*` | Invalid duration/clock policy | Error | Safe value suggestion, explicit apply |
| `EOBJ-VAL-RWD-*` | Duplicate rewards, missing executor drawer, unsafe payload | Warning/Error | No payload destruction |
| `EOBJ-VAL-DATA-*` | Schema gap, alias gap, missing migration | Error/Blocker | Generate migration stub only later during implementation |
| `EOBJ-VAL-SETUP-*` | Missing configuration/root/catalog | Warning/Error | Create-only-safe repair |
| `EOBJ-VAL-BUILD-*` | Development initializer or invalid content in release | Blocker | Explicit fix guidance |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Planned routes:

- Git URL.
- Local package path.
- Tarball.
- Embedded package development.
- Workshop selection after its setup facade is implemented.

Every claimed route requires its own SFGSS-004 evidence.

### 12.2 Minimal scene setup

Production minimum:

- one `EchoObjectivesRoot`;
- one project-owned `EchoObjectivesConfiguration`;
- at least one `ObjectiveCatalog` or a documented empty-catalog state;
- optional project adapters/providers.

No EventSystem, Canvas, input asset, save file, scene name, tag, or layer is required by the core.

### 12.3 Boot-scene setup

The normal production setup places one configured root in the canonical Boot scene or initializes it through a First Light integration. First Light is optional. Duplicate safety applies regardless of creation path.

### 12.4 Direct-scene setup

`EchoObjectivesDirectSceneInitializer` is development-only and:

- checks for an existing authority;
- creates only the configured missing root;
- marks diagnostics as direct-scene development initialization;
- rejects duplicates through the same claim path;
- can be disabled or excluded from release builds;
- never creates UI, save, dialogue, inventory, or other package authorities.

### 12.5 Scene isolation rule

The Standalone Laboratory contains only EchoObjectives, declared Unity dependencies, project-owned sample definitions, fake providers/executors/clocks, and redistributable sample presentation. Integration Labs are separate bridge evidence.

---

## 13. Standalone Test Lab and Samples

### 13.1 Laboratory purpose

The **Path Objectives Laboratory** proves the full core loop in isolation: availability, activation, progress, group aggregation, optional/hidden behavior, timers, suspension, repeatability, tracking, completion, reward delivery/failure/retry, export/import, orphan handling, diagnostics, reset, and capacity limits.

### 13.2 Required contents

- One Laboratory scene and README.
- Configuration and catalog with several sample objectives.
- Fake condition, progress, reward, and clock providers.
- Buttons/keyboard controls that call the public API directly.
- Source fallback labels with no Localization dependency.
- Visual state readout with IDs, lifecycle, nodes, timers, tracking, and reward ledger.
- Controls to simulate missing providers, duplicate requests, stale run IDs, reward failure, timeout, and retries.
- Controls to export, clear, import, migrate, orphan, restore, and reset state.
- Capacity/stress controls.
- No project-owned or restricted content.

### 13.3 Laboratory acceptance checklist

| Test | Action | Expected result | Type | Status |
|---|---|---|---|---|
| EOBJ-LAB-001 | Initialize one configured root | Root becomes Ready and exposes an empty objective registry | Manual/automatable | Not run |
| EOBJ-LAB-002 | Introduce a duplicate root before initialization | Duplicate is rejected before subscriptions, clocks, or providers start | Manual/automatable | Not run |
| EOBJ-LAB-003 | Start the Laboratory scene directly | Development initializer creates only the missing configured authority | Manual/automatable | Not run |
| EOBJ-LAB-004 | Delete the sample presenter | Runtime objective flow remains operational through API and diagnostics | Manual/automatable | Not run |
| EOBJ-LAB-005 | Load an empty catalog | Root reports Ready with an actionable empty-catalog advisory | Manual/automatable | Not run |
| EOBJ-LAB-006 | Load a definition with duplicate ObjectiveId | Validation blocks activation and identifies both assets | Manual/automatable | Not run |
| EOBJ-LAB-007 | Load a definition with duplicate node IDs | Validation blocks the definition and reports each collision | Manual/automatable | Not run |
| EOBJ-LAB-008 | Load a definition with a cyclic prerequisite graph | Validation reports the cycle and prevents unsafe availability evaluation | Manual/automatable | Not run |
| EOBJ-LAB-009 | Evaluate an objective whose prerequisites are met | Availability becomes Available without creating an active instance | Manual/automatable | Not run |
| EOBJ-LAB-010 | Evaluate an objective with an unmet prerequisite | Availability remains Locked with structured reasons | Manual/automatable | Not run |
| EOBJ-LAB-011 | Evaluate a missing external condition provider | Availability becomes Unavailable rather than silently granted | Manual/automatable | Not run |
| EOBJ-LAB-012 | Activate an available objective | One new run instance enters Active and publishes a snapshot | Manual/automatable | Not run |
| EOBJ-LAB-013 | Activate a non-repeatable objective twice | Second activation is rejected with a stable result code | Manual/automatable | Not run |
| EOBJ-LAB-014 | Activate a repeatable objective after completion | A new run ID is created while prior completion history remains intact | Manual/automatable | Not run |
| EOBJ-LAB-015 | Submit a counter increment | Only the addressed step changes and parent progress recomputes transactionally | Manual/automatable | Not run |
| EOBJ-LAB-016 | Submit the same mutation request ID twice | Second request is deduplicated within the configured idempotency window | Manual/automatable | Not run |
| EOBJ-LAB-017 | Set a flag step true | Step completes and publishes one semantic progress event | Manual/automatable | Not run |
| EOBJ-LAB-018 | Manually complete a step | Step completes only when the request targets the active instance and correct generation | Manual/automatable | Not run |
| EOBJ-LAB-019 | Submit progress to an inactive step in a sequence | Request is rejected without mutating later steps | Manual/automatable | Not run |
| EOBJ-LAB-020 | Complete the first child of an ordered group | Next child activates and previous child stays completed | Manual/automatable | Not run |
| EOBJ-LAB-021 | Complete all required children of an All group | Parent group completes exactly once | Manual/automatable | Not run |
| EOBJ-LAB-022 | Complete one child of an Any group | Parent completes and remaining children follow authored closure policy | Manual/automatable | Not run |
| EOBJ-LAB-023 | Reach the threshold in a Threshold group | Parent completes when the authored count is met | Manual/automatable | Not run |
| EOBJ-LAB-024 | Leave an optional child incomplete | Required parent completion is not blocked | Manual/automatable | Not run |
| EOBJ-LAB-025 | Reveal a hidden step through an authored rule | Visibility changes without altering objective truth | Manual/automatable | Not run |
| EOBJ-LAB-026 | Advance a scaled timer | Timer progresses only while its objective and clock policy permit | Manual/automatable | Not run |
| EOBJ-LAB-027 | Advance an unscaled timer while time scale is zero | Unscaled timer progresses and scaled timer remains paused | Manual/automatable | Not run |
| EOBJ-LAB-028 | Suspend an objective with a lease | Progress and authored timers obey suspension policy until the lease is released | Manual/automatable | Not run |
| EOBJ-LAB-029 | Release suspension leases out of order | Objective resumes only after the last applicable lease is gone | Manual/automatable | Not run |
| EOBJ-LAB-030 | Allow a fail-on-expiry timer to expire | Objective follows its authored failure route and records the reason | Manual/automatable | Not run |
| EOBJ-LAB-031 | Complete the last required node | Objective commits Completed before reward delivery begins | Manual/automatable | Not run |
| EOBJ-LAB-032 | Deliver two rewards successfully | Both deterministic grant IDs become Succeeded without duplicate dispatch | Manual/automatable | Not run |
| EOBJ-LAB-033 | Fail one reward executor | Objective remains Completed and the failed grant remains visible and retryable | Manual/automatable | Not run |
| EOBJ-LAB-034 | Retry a failed reward grant | Only the failed grant is retried with the same grant ID | Manual/automatable | Not run |
| EOBJ-LAB-035 | Remove a reward executor and reload | Pending grant remains preserved but unavailable until the executor returns | Manual/automatable | Not run |
| EOBJ-LAB-036 | Track one objective and pin two others | Presentation snapshot reflects deterministic primary and pin ordering | Manual/automatable | Not run |
| EOBJ-LAB-037 | Complete the tracked objective | Tracking follows authored auto-untrack policy without UI authority | Manual/automatable | Not run |
| EOBJ-LAB-038 | Export active and completed state | Versioned state document contains instances, nodes, tracking, and reward ledger | Manual/automatable | Not run |
| EOBJ-LAB-039 | Import a prepared compatible state | State applies atomically and events publish after the commit | Manual/automatable | Not run |
| EOBJ-LAB-040 | Import state with a missing definition | Record becomes preserved orphan data rather than being deleted | Manual/automatable | Not run |
| EOBJ-LAB-041 | Restore a previously missing definition | Orphan record rehydrates after validation and alias resolution | Manual/automatable | Not run |
| EOBJ-LAB-042 | Migrate an older state fixture | Contiguous migrations produce the current schema and preserve the source fixture | Manual/automatable | Not run |
| EOBJ-LAB-043 | Load a newer unsupported state document | Import refuses safely and reports the unsupported version | Manual/automatable | Not run |
| EOBJ-LAB-044 | Generate a redacted diagnostic snapshot | Snapshot contains IDs, states, counts, and failures but no resolved production text | Manual/automatable | Not run |
| EOBJ-LAB-045 | Stress the configured active-objective limit | New activation is rejected gracefully without corrupting existing runs | Manual/automatable | Not run |
| EOBJ-LAB-046 | Stress bounded progress history and event queues | Old diagnostic entries trim deterministically without changing objective state | Manual/automatable | Not run |
| EOBJ-LAB-047 | Disable every optional bridge | Standalone Laboratory continues through fake providers and source labels | Manual/automatable | Not run |
| EOBJ-LAB-048 | Reset the Laboratory repeatedly | Every run returns to a clean deterministic baseline without mutating definition assets | Manual/automatable | Not run |

### 13.4 Optional integration samples

| Sample | Packages | Purpose | Why not standalone proof |
|---|---|---|---|
| Quest Journal Lab | EchoObjectives + EchoUI + EchoLocalization | Present tracked objectives and localized descriptions | Requires presentation/localization authorities |
| Dialogue Objective Lab | EchoObjectives + EchoDialogue | Read conditions and request mutations from conversation commands | Requires dialogue authority |
| Inventory Reward Lab | EchoObjectives + EchoInventory | Collect item conditions and deliver item rewards | Requires inventory authority |
| Progression Reward Lab | EchoObjectives + EchoProgression | Unlock content on objective completion | Requires progression authority |
| Save-Based Quest Lab | EchoObjectives + EchoSave | Persist active runs and ledgers | Requires Chronicle transport |
| Interaction Progress Lab | EchoObjectives + EchoInteraction | Convert semantic interactions into progress | Requires interaction authority |
| Multiplayer Objective Lab | EchoObjectives + EchoMultiplayer | Validate authoritative objective mutations | Advanced adapter evidence only |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The core is nonvisual. It exposes immutable snapshots and semantic events. The Laboratory presenter is sample-only. Production journal, HUD, tracker, marker, completion, failure, reward, and notification views belong to EchoUI or project code.

### 14.2 Required presentation states

Structured snapshots must make these states distinguishable without requiring production strings:

- Available
- Locked
- Unavailable
- Active
- Suspended
- Completed
- Failed
- Abandoned
- Optional
- Hidden/revealed
- Reward pending/succeeded/failed/unavailable
- Tracked/pinned/untracked
- Orphaned or definition unavailable

### 14.3 Progress presentation contract

Snapshots provide:

- stable IDs and provider-neutral text references;
- lifecycle and visibility;
- current/target numeric values;
- normalized progress where meaningful;
- timer elapsed/remaining and clock metadata;
- group policy and child summaries;
- required/optional state;
- tracked/pinned ordering;
- reward delivery summary;
- structured failure/unavailable reasons.

They do not provide formatted production text, animated values, color choices, audio cues, map positions, or UI focus behavior.

### 14.4 Accessibility requirements

- Important state must not rely on color or audio alone.
- Numeric progress and timer values remain available to text and assistive presentation.
- Presenters can choose timer precision and transient duration.
- Hidden content must not leak through default accessibility labels before reveal.
- Completion and failure events support persistent history, not only transient toasts.
- Reduced motion, screen shake, flash, and audio policy remain external presentation/feedback settings.

### 14.5 Visual customization

All production visuals, icons, typography, layout, animations, map markers, and journal styles are project-owned and replaceable without editing the runtime package.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Initialization/configuration report | API/Inspector/Console | Editor/Development | Startup only |
| Definition validation report | Editor window | Editor | On demand/preflight |
| Runtime status snapshot | API/Inspector/Lab | Development; redacted release option | Bounded snapshot |
| Mutation/lifecycle history | API/Lab | Development | Bounded ring buffer |
| Reward ledger/failure summary | API/Lab | Development; redacted release option | Bounded |
| Exported support snapshot | Explicit command | Configurable | On demand |

### 15.2 Structured status

- authority/root ID and initialization state;
- package/configuration versions;
- catalog/definition counts and invalid definitions;
- active, suspended, terminal, tracked, pinned, orphaned, and capacity counts;
- provider registrations and unavailable provider types;
- pending/failed reward counts;
- current service revision;
- bounded recent errors and mutation summaries;
- direct-scene development flag.

### 15.3 Diagnostic codes

Reserved namespace: `EOBJ-*`.

| Family | Meaning |
|---|---|
| `EOBJ-AUTH-*` | Authority, duplicate, lifecycle |
| `EOBJ-CFG-*` | Configuration/setup |
| `EOBJ-DEF-*` | Definition/graph/ID validation |
| `EOBJ-PROV-*` | Condition/progress/clock provider |
| `EOBJ-MUT-*` | Progress/lifecycle mutation |
| `EOBJ-TIME-*` | Timer/clock |
| `EOBJ-TRACK-*` | Tracking/pinning |
| `EOBJ-RWD-*` | Reward dispatch/ledger |
| `EOBJ-DATA-*` | Export/import/migration/orphan |
| `EOBJ-CAP-*` | Capacity/limits |
| `EOBJ-OBS-*` | Diagnostics/listeners |
| `EOBJ-VAL-*` | Editor validation/build preflight |

### 15.4 Observatory bridge

A separate provider bridge may publish redacted counts, health, failures, provider availability, reward backlog, and recent semantic events. EchoObjectives never depends on Observatory.

### 15.5 Logging policy

- Searchable diagnostic codes and objective/run/node/reward IDs.
- No per-frame timer or counter spam.
- No resolved production titles/descriptions or arbitrary provider/reward payloads by default.
- Listener exceptions are contained and reported.
- Development verbosity is configurable and distinct from release-safe reporting.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Definitions/configuration | Project content | Project/EchoObjectives types | Asset data | Unity assets |
| Active/terminal runs | Profile/slot/project choice | EchoObjectives | Yes when project requests | Export/import or Chronicle bridge |
| Node/timer progress | Run | EchoObjectives | Yes | State document |
| Tracking/pins | Profile/slot/project choice | EchoObjectives | Yes when configured | State document |
| Reward ledgers | Run | EchoObjectives | Yes | State document |
| Provider registrations | Session | Integrating code | No | Re-register |
| Suspension leases | Session | Integrating code | No by default | Reacquire from authoritative context |
| Diagnostic history | Session | EchoObjectives | No by default | Bounded memory/export |

### 16.2 Standalone behavior

Without EchoSave, the project may:

- use session-only state;
- call `ExportState` and `PrepareImport/ApplyImport` explicitly;
- implement a project-owned backend;
- omit persistence entirely.

EchoObjectives never silently chooses a filename, slot, or `PlayerPrefs` key.

### 16.3 Optional participant/provider contract

The Chronicle bridge registers one versioned objective participant. Capture returns a detached `ObjectiveStateDocument`. Prepare-load validates/migrates without live mutation. Apply-load commits atomically after the project reaches an approved scene/state. Unknown objective/provider/reward records are preserved under SFGSS-003.

### 16.4 Failure and recovery

- Missing state: initialize empty.
- Corrupt/incompatible state: reject import and preserve live/source state.
- Older state: migrate through contiguous steps.
- Newer unsupported state: refuse safely.
- Missing definitions: preserve orphan records.
- Missing providers/executors: preserve state as unavailable/pending.
- Stale prepared import: reject after revision mismatch.
- Partial external reward commit: reconcile using grant ID and executor result, never blindly grant again.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Optional connections are explicit, removable, versioned, and fail visibly. Core behavior never changes merely because another package was installed. Bridge packages or project adapters translate semantic contracts without recreating either authority.

### 17.2 Planned integrations

| Authority | Connection | Bridge owner | Data/events | Required? |
|---|---|---|---|---:|
| EchoLocalization | Text-reference resolver | Separate bridge | Objective/step text refs, locale refresh | No |
| EchoUI | Journal/HUD presenter | Separate bridge | Presentation snapshots, tracking commands | No |
| EchoDialogue | Condition/command adapter | Separate bridge | Read state, explicit mutations | No |
| EchoInventory | Condition/reward adapters | Separate bridge | Item queries and idempotent grants | No |
| EchoProgression | Condition/reward adapters | Separate bridge | Access/completion queries and mutation batches | No |
| EchoSave | Save participant | Separate bridge | Versioned state document | No |
| EchoInteraction | Progress adapter | Separate bridge/project adapter | Semantic interaction results | No |
| EchoSceneFlow/EchoWorld | Location/route adapters | Project/separate bridge | Stable location/route conditions | No |
| EchoGameState | Suspension/clock policy | Separate bridge | State scopes and clock metadata | No |
| EchoDiagnostics | Status provider | Separate bridge | Redacted health/metrics | No |
| EchoBuildTools | Preflight validator | Separate Editor bridge | Catalog/graph/migration validation | No |
| EchoGameStarter | ADR-001 setup facade | EchoObjectives Editor | Plan/apply/receipt | No runtime dependency |
| EchoMultiplayer | Authority adapter | Advanced package | Validate/replicate semantic mutations | No |

### 17.3 Provider and adapter registration rules

- Condition, progress, clock, and reward provider type IDs are stable and unique.
- Registration is explicit and returns a disposable generational handle.
- Duplicate type registration is rejected unless an approved replacement policy is requested before work begins.
- Provider removal invalidates only dependent evaluations/work.
- Providers cannot mutate objective state during read-only evaluation.
- Reward executors receive stable grant IDs and report commit semantics.
- Reflection scanning is prohibited in the runtime core.

### 17.4 Integration failure behavior

- Missing peer: bridge does not install/compile or core remains standalone.
- Version mismatch: bridge blocks with actionable compatibility result.
- Provider absent: affected availability/steps/rewards become Unavailable, never implicitly successful.
- Bridge teardown: dispose registrations before removing peer/core packages.
- In-flight reward teardown: wait, cancel before commit, or reconcile through executor contract.
- Save bridge absent: state remains session-only unless project provides storage.
- Multiplayer adapter absent: core remains local authority only.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

All targets are planned and `Not run` until measured.

| Metric | Planned target | Measurement | Release threshold |
|---|---|---|---|
| Idle service overhead | No meaningful per-frame work with no active timers/providers | Profiler in Laboratory | Evidence required |
| Progress mutation | Bounded by affected node ancestors, not full catalog scan | PlayMode/performance tests | Evidence required |
| Availability evaluation | Cached/bounded; invalidates affected definitions | Synthetic catalog fixture | Evidence required |
| Timer updates | Configurable active-timer limit and update cadence | Stress Laboratory | Evidence required |
| Snapshot creation | No unbounded history or reflection | Profiler allocation tests | Evidence required |
| Reward scanning | Only pending/retry-eligible grants | Stress fixture | Evidence required |
| State export/import | Bounded documented limits | Large fixture | Evidence required |

### 18.2 Allocation policy

- No LINQ or reflection in hot mutation/timer paths unless measured and approved.
- Reuse buffers where safe without exposing mutable shared collections.
- Events carry immutable structs/DTOs or stable references.
- Provider evaluation is bounded and may use configured cadence.
- Diagnostic histories are ring buffers.
- Snapshot allocations are explicit caller operations, not hidden every frame.

### 18.3 Scene and domain reload behavior

- Dispose scene/project adapters cleanly.
- Application-session root persists only when configured.
- Reset statics for domain-reload-disabled Play Mode.
- Clear/rebuild provider registrations after reload.
- Direct-scene development initialization follows normal duplicate safety.
- No ScriptableObject runtime contamination across sessions.

### 18.4 Scalability limits

Configuration declares and validation enforces:

- maximum definitions per catalog;
- maximum nodes/prerequisite depth per definition;
- maximum active runs;
- maximum active timers/provider evaluations;
- maximum tracked/pinned objectives;
- maximum terminal history per definition/profile;
- maximum dedupe entries per run;
- maximum rewards and retry attempts;
- maximum diagnostic/event history.

Exceeding limits returns structured failure instead of unbounded growth.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Core state is normally gameplay data, not credentials or personal data. Project content may contain spoilers, names, or sensitive narrative text; default diagnostics therefore use IDs and structured states rather than resolved production strings or arbitrary payloads.

### 19.2 Trust boundaries

- Validate imported documents and provider/reward payload schemas.
- Never execute method names from serialized content.
- Providers and executors are registered code, not arbitrary asset-driven reflection.
- Reward grants are not purchases, entitlements, security proofs, or anti-cheat.
- Multiplayer clients are not authoritative without an approved server/provider adapter.
- Diagnostic exports require explicit user action and redact content by default.

### 19.3 Platform behavior

| Platform | Status | Special behavior | Required evidence |
|---|---|---|---|
| Windows | Planned | Standard Unity runtime | Clean install, Laboratory, persistence, build |
| macOS | Planned | Standard Unity runtime | Same |
| Linux | Planned | Standard Unity runtime | Same |
| WebGL | Planned | Main-thread/file-storage constraints affect bridges, not core state model | Player/Laboratory tests |
| Mobile | Planned | Pause/resume and timer policy require lifecycle tests | Device tests |
| Console | Unknown/planned | Platform certification and storage/provider rules unavailable | Provider/platform approval |

No platform is Supported until SFGSS-004 evidence exists.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-objectives/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
├── Runtime/
├── Editor/
├── Samples~/
└── Tests/
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoObjectivesRoot.cs
│   ├── IEchoObjectivesService.cs
│   ├── EchoObjectivesService.cs
│   └── ObjectiveServiceState.cs
├── Definitions/
│   ├── EchoObjectivesConfiguration.cs
│   ├── ObjectiveCatalog.cs
│   ├── ObjectiveDefinition.cs
│   ├── ObjectiveNodeRecord.cs
│   ├── ObjectiveGroupRecord.cs
│   ├── ObjectiveStepRecord.cs
│   ├── ObjectivePrerequisiteSet.cs
│   ├── ObjectiveRewardDefinition.cs
│   └── ObjectiveAliasRegistry.cs
├── Identity/
├── Availability/
├── RuntimeState/
├── Mutations/
├── Groups/
├── Timers/
├── Tracking/
├── Rewards/
├── Providers/
├── Persistence/
├── Diagnostics/
├── Development/
└── EchoDevGames.EchoObjectives.Runtime.asmdef

Editor/
├── Setup/
├── Authoring/
├── Validation/
├── Inspectors/
├── Workshop/
└── EchoDevGames.EchoObjectives.Editor.asmdef

Samples~/
└── The Path Objectives Laboratory/

Tests/
├── Editor/
│   └── EchoDevGames.EchoObjectives.Tests.Editor.asmdef
└── Runtime/
    └── EchoDevGames.EchoObjectives.Tests.Runtime.asmdef
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoObjectives.Runtime` | Runtime | Unity engine modules only | Yes | Neutral objective authority |
| `EchoDevGames.EchoObjectives.Editor` | Editor | Runtime + UnityEditor/UI Toolkit | No | Setup, authoring, validation, Workshop facade |
| `EchoDevGames.EchoObjectives.Tests.Runtime` | Test | Runtime + Test Framework | No | EditMode/PlayMode tests |
| `EchoDevGames.EchoObjectives.Tests.Editor` | Editor test | Runtime + Editor + Test Framework | No | Editor/setup/validation tests |

Optional bridges/providers are separate packages/assemblies under SFGSS-002.

### 20.4 Repository files

- README and documentation index.
- Current Notes link.
- User and developer guides.
- Architecture, lifecycle, provider, reward, persistence, diagnostics, Laboratory, migration, and troubleshooting docs.
- ADRs, checkpoints, test reports, issue records, and release checklist.
- Changelog, license, notices, contribution/security guidance.
- Stable `.meta` files and GUIDs.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | Planned 6000.0 | None yet | Primary development baseline 6000.3.8f1 |
| Unity Test Framework | Verify at implementation | None yet | Tests only |
| Peer Echo packages | None in core | None yet | Bridges pin exact compatible versions |

### 21.2 Semantic versioning policy

- **Patch:** bug fixes, diagnostics, docs, and behavior corrections that preserve APIs, IDs, serialization, and assets.
- **Minor:** additive capabilities, node/reward/provider types, APIs, and migrations that preserve compatibility.
- **Major:** breaking public API, definition schema, runtime state, provider, reward, assembly, package, or removal behavior.
- Serialization-compatible behavior changes still require migration/compatibility notes.

### 21.3 Deprecation policy

- Mark public APIs, IDs, node/reward/provider types, and fields deprecated before removal.
- Provide aliases/migrations and compile/runtime warnings where practical.
- Do not recycle stable IDs or diagnostic/test identifiers.
- Remove only in a major release unless a critical security/data-loss issue requires faster action.
- Document exact replacement and deadline.

### 21.4 GUID and asset compatibility

Public scripts, definitions, templates, prefabs, samples, and configuration assets preserve committed `.meta` GUIDs. Renames/moves retain GUIDs when identity survives. Runtime domain IDs remain separate and require alias/migration when changed.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview, boundaries, and five-minute quick start.
- Installation and direct setup.
- Objective authoring guide.
- Groups, steps, optional/hidden, timers, repeat, tracking, and rewards.
- Provider/adapter quick starts.
- Standalone Laboratory guide.
- Diagnostics and error-code reference.
- State export/import and Chronicle integration.
- Migration/upgrade/removal guide.
- Known limitations, license, credits, and notices.

### 22.2 Required developer documentation

- Authority and lifecycle model.
- Graph and progress semantics.
- Stable IDs, aliases, state schema, and migrations.
- Mutation, dedupe, suspension, timer, tracking, and reward contracts.
- Provider/executor registration and teardown.
- Integration/bridge index.
- Testing/evidence strategy.
- Release workflow, ADRs, current checkpoint, and Current Notes.

### 22.3 Documentation truth rule

Examples must compile against the documented release. Screenshots and menu paths must match the tested Unity version. Planned tests remain `Not run` until executed. No compatibility, performance, migration, or platform claim may exceed evidence.

### 22.4 Living repository and Obsidian workflow

Documentation lives in Git and opens directly in Obsidian. Current Notes capture provisional work. Durable decisions are promoted into this specification, ADRs, guides, tests, issues, changelog, or release records at checkpoint closeout. Git history is the archive.

### 22.5 Repository scan and handoff order

1. README/index.
2. SFGSS-000.
3. SFGSS-002 through SFGSS-005.
4. This specification.
5. Applicable ADRs/bridge specs.
6. Current Notes.
7. Current checkpoint, test reports, issues, changelog.
8. Relevant implementation/tests after coding begins.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | MVP required |
|---|---|---|---:|
| EditMode unit | IDs, graphs, prerequisites, mutations, groups, migrations, reward ledger | Pure deterministic policies | Yes |
| PlayMode | Root lifecycle, timers, providers, events, direct-scene behavior | Runtime authority | Yes |
| Editor tooling | Setup, authoring, validation, repair, Workshop facade | Repeatability/non-destruction | Yes |
| Standalone Laboratory | Complete isolated user workflow | 48 scenarios | Yes |
| Bridge Integration Labs | UI, Dialogue, Inventory, Progression, Save, Interaction | Separate bridge evidence | When bridge ships |
| Clean-project install | Git/local/tarball/embedded | Compile and function | Yes |
| Existing-project migration | Adoption/replacement | Preserve-until-parity | Before claim |
| Performance/platform | Capacity and supported environments | Measured evidence | Yes for Supported claims |

### 23.2 Required test categories

- Installation and assembly isolation.
- Authority, duplicate protection, direct-scene, shutdown, reload.
- Definition, ID, graph, alias, and provider validation.
- Availability and prerequisites.
- Activation, lifecycle, suspension, failure, abandonment.
- Progress mutations, batches, dedupe, events.
- Groups, order, optional, hidden, closure, failure propagation.
- Timers and clocks.
- Repeatability and history.
- Tracking and presentation snapshots.
- Reward delivery, idempotency, failure, timeout, retry, reconciliation.
- Export/import, migrations, orphans, unknown data, Chronicle bridge.
- Diagnostics and privacy.
- Editor setup/authoring/repair.
- Performance, capacities, platform, accessibility, removal, upgrade, release.

### 23.3 Test case registry

| ID | Category | Planned test | Status |
|---|---|---|---|
| EOBJ-T-001 | Installation and assembly | Install through a Git URL in a clean supported Unity project. | Not run |
| EOBJ-T-002 | Installation and assembly | Install from a local package path. | Not run |
| EOBJ-T-003 | Installation and assembly | Install from a tarball. | Not run |
| EOBJ-T-004 | Installation and assembly | Embed the package for development. | Not run |
| EOBJ-T-005 | Installation and assembly | Compile Runtime with no other Echo package installed. | Not run |
| EOBJ-T-006 | Installation and assembly | Compile Editor with no other Echo package installed. | Not run |
| EOBJ-T-007 | Installation and assembly | Compile Runtime with no UnityEditor reference. | Not run |
| EOBJ-T-008 | Installation and assembly | Verify the neutral Runtime assembly has no uGUI, TextMeshPro, Dialogue, Inventory, Progression, Save, Interaction, or Multiplayer reference. | Not run |
| EOBJ-T-009 | Installation and assembly | Import the Standalone Laboratory sample without modifying package source. | Not run |
| EOBJ-T-010 | Installation and assembly | Remove the Standalone Laboratory sample without breaking Runtime or Editor assemblies. | Not run |
| EOBJ-T-011 | Installation and assembly | Remove EchoObjectives after removing bridges and confirm the project compiles. | Not run |
| EOBJ-T-012 | Installation and assembly | Reinstall EchoObjectives and reopen project-owned catalogs and definitions. | Not run |
| EOBJ-T-013 | Installation and assembly | Validate package.json, asmdefs, documentation routes, license, and notices. | Not run |
| EOBJ-T-014 | Installation and assembly | Run the package in an embedded-package workspace with Enter Play Mode options enabled. | Not run |
| EOBJ-T-015 | Authority and lifecycle | Create one configured root and initialize successfully. | Not run |
| EOBJ-T-016 | Authority and lifecycle | Reject a duplicate root before subscriptions, clocks, provider registration, or state mutation. | Not run |
| EOBJ-T-017 | Authority and lifecycle | Introduce a duplicate root during scene loading and keep the original authority. | Not run |
| EOBJ-T-018 | Authority and lifecycle | Shut down the authority and dispose every registration and suspension lease. | Not run |
| EOBJ-T-019 | Authority and lifecycle | Reinitialize after a clean domain reload. | Not run |
| EOBJ-T-020 | Authority and lifecycle | Reset static state when domain reload is disabled. | Not run |
| EOBJ-T-021 | Authority and lifecycle | Start the Standalone Laboratory scene directly with no existing authority. | Not run |
| EOBJ-T-022 | Authority and lifecycle | Start a normal scene directly when development initialization is disabled. | Not run |
| EOBJ-T-023 | Authority and lifecycle | Fail visibly when required configuration is missing. | Not run |
| EOBJ-T-024 | Authority and lifecycle | Initialize with an empty catalog and emit only an advisory. | Not run |
| EOBJ-T-025 | Authority and lifecycle | Reject public mutation calls before Ready. | Not run |
| EOBJ-T-026 | Authority and lifecycle | Reject mutation calls after shutdown. | Not run |
| EOBJ-T-027 | Authority and lifecycle | Keep initialization failure from partially publishing an authority. | Not run |
| EOBJ-T-028 | Authority and lifecycle | Expose root identity, version, configuration source, and initialization state. | Not run |
| EOBJ-T-029 | Authority and lifecycle | Unsubscribe from scene and application callbacks on shutdown. | Not run |
| EOBJ-T-030 | Authority and lifecycle | Complete application quit without asynchronous reward work leaking callbacks. | Not run |
| EOBJ-T-031 | Definitions, IDs, and validation | Accept unique stable ObjectiveId values. | Not run |
| EOBJ-T-032 | Definitions, IDs, and validation | Reject duplicate ObjectiveId values across the catalog. | Not run |
| EOBJ-T-033 | Definitions, IDs, and validation | Reject empty ObjectiveId values. | Not run |
| EOBJ-T-034 | Definitions, IDs, and validation | Keep display-name changes from changing ObjectiveId. | Not run |
| EOBJ-T-035 | Definitions, IDs, and validation | Accept unique stable NodeId values inside one definition. | Not run |
| EOBJ-T-036 | Definitions, IDs, and validation | Reject duplicate NodeId values inside one definition. | Not run |
| EOBJ-T-037 | Definitions, IDs, and validation | Reject a node that references a missing child. | Not run |
| EOBJ-T-038 | Definitions, IDs, and validation | Reject a node that references itself. | Not run |
| EOBJ-T-039 | Definitions, IDs, and validation | Detect a cycle in the objective node graph. | Not run |
| EOBJ-T-040 | Definitions, IDs, and validation | Detect a cycle in objective prerequisites. | Not run |
| EOBJ-T-041 | Definitions, IDs, and validation | Detect unreachable required nodes. | Not run |
| EOBJ-T-042 | Definitions, IDs, and validation | Detect an ordered group with duplicate child ordering. | Not run |
| EOBJ-T-043 | Definitions, IDs, and validation | Detect a threshold greater than eligible child count. | Not run |
| EOBJ-T-044 | Definitions, IDs, and validation | Detect a group with no children when its policy requires children. | Not run |
| EOBJ-T-045 | Definitions, IDs, and validation | Detect a reward with an empty RewardId. | Not run |
| EOBJ-T-046 | Definitions, IDs, and validation | Detect duplicate RewardId values inside one definition. | Not run |
| EOBJ-T-047 | Definitions, IDs, and validation | Detect a provider-backed step with no ProviderTypeId. | Not run |
| EOBJ-T-048 | Definitions, IDs, and validation | Detect a timer with an invalid duration. | Not run |
| EOBJ-T-049 | Definitions, IDs, and validation | Preserve Unity asset GUID separately from runtime domain identity. | Not run |
| EOBJ-T-050 | Definitions, IDs, and validation | Apply configured ID aliases without silently rewriting source assets at runtime. | Not run |
| EOBJ-T-051 | Availability and prerequisites | Return Available when all built-in prerequisites pass. | Not run |
| EOBJ-T-052 | Availability and prerequisites | Return Locked with structured reasons when a prerequisite fails. | Not run |
| EOBJ-T-053 | Availability and prerequisites | Return Unavailable when an external condition provider is missing. | Not run |
| EOBJ-T-054 | Availability and prerequisites | Return Unavailable when an external condition provider throws or returns failure. | Not run |
| EOBJ-T-055 | Availability and prerequisites | Combine All prerequisite groups deterministically. | Not run |
| EOBJ-T-056 | Availability and prerequisites | Combine Any prerequisite groups deterministically. | Not run |
| EOBJ-T-057 | Availability and prerequisites | Evaluate a threshold prerequisite group. | Not run |
| EOBJ-T-058 | Availability and prerequisites | Evaluate completion of another objective by stable ID. | Not run |
| EOBJ-T-059 | Availability and prerequisites | Evaluate active state of another objective by stable ID. | Not run |
| EOBJ-T-060 | Availability and prerequisites | Evaluate repeat-count prerequisites. | Not run |
| EOBJ-T-061 | Availability and prerequisites | Evaluate project conditions through explicit provider registration. | Not run |
| EOBJ-T-062 | Availability and prerequisites | Reject provider registration with a duplicate ProviderTypeId. | Not run |
| EOBJ-T-063 | Availability and prerequisites | Remove a provider and invalidate only affected availability caches. | Not run |
| EOBJ-T-064 | Availability and prerequisites | Publish availability change events only when the structured result changes. | Not run |
| EOBJ-T-065 | Availability and prerequisites | Bound prerequisite depth and node count. | Not run |
| EOBJ-T-066 | Availability and prerequisites | Prevent availability evaluation from mutating project or objective state. | Not run |
| EOBJ-T-067 | Activation, lifecycle, and suspension | Activate one available objective and create a unique ObjectiveRunId. | Not run |
| EOBJ-T-068 | Activation, lifecycle, and suspension | Reject activation of a locked objective. | Not run |
| EOBJ-T-069 | Activation, lifecycle, and suspension | Reject activation of an unavailable objective. | Not run |
| EOBJ-T-070 | Activation, lifecycle, and suspension | Reject a second concurrent run when the definition forbids it. | Not run |
| EOBJ-T-071 | Activation, lifecycle, and suspension | Create a new run after a repeatable objective completes. | Not run |
| EOBJ-T-072 | Activation, lifecycle, and suspension | Abandon an active objective when policy permits. | Not run |
| EOBJ-T-073 | Activation, lifecycle, and suspension | Reject abandonment when policy forbids it. | Not run |
| EOBJ-T-074 | Activation, lifecycle, and suspension | Fail an objective through an explicit authorized request. | Not run |
| EOBJ-T-075 | Activation, lifecycle, and suspension | Reject terminal-state mutation after completion. | Not run |
| EOBJ-T-076 | Activation, lifecycle, and suspension | Archive bounded terminal run history according to policy. | Not run |
| EOBJ-T-077 | Activation, lifecycle, and suspension | Acquire one objective suspension lease. | Not run |
| EOBJ-T-078 | Activation, lifecycle, and suspension | Acquire multiple suspension leases with independent reasons. | Not run |
| EOBJ-T-079 | Activation, lifecycle, and suspension | Release suspension leases out of order. | Not run |
| EOBJ-T-080 | Activation, lifecycle, and suspension | Dispose the same suspension lease twice safely. | Not run |
| EOBJ-T-081 | Activation, lifecycle, and suspension | Reject a foreign-root suspension lease. | Not run |
| EOBJ-T-082 | Activation, lifecycle, and suspension | Recompute presentation snapshots after lifecycle transitions. | Not run |
| EOBJ-T-083 | Progress requests and idempotency | Increment a counter step by a positive amount. | Not run |
| EOBJ-T-084 | Progress requests and idempotency | Apply a negative counter delta only when the definition permits regression. | Not run |
| EOBJ-T-085 | Progress requests and idempotency | Clamp counter progress at its authored bounds. | Not run |
| EOBJ-T-086 | Progress requests and idempotency | Set an absolute counter value. | Not run |
| EOBJ-T-087 | Progress requests and idempotency | Set a flag step to its target value. | Not run |
| EOBJ-T-088 | Progress requests and idempotency | Complete a manual step explicitly. | Not run |
| EOBJ-T-089 | Progress requests and idempotency | Fail a step explicitly when the definition permits failure. | Not run |
| EOBJ-T-090 | Progress requests and idempotency | Reject progress for a missing objective run. | Not run |
| EOBJ-T-091 | Progress requests and idempotency | Reject progress for a missing node. | Not run |
| EOBJ-T-092 | Progress requests and idempotency | Reject progress for a terminal run. | Not run |
| EOBJ-T-093 | Progress requests and idempotency | Reject progress for an inactive sequence child. | Not run |
| EOBJ-T-094 | Progress requests and idempotency | Reject progress from a stale run generation. | Not run |
| EOBJ-T-095 | Progress requests and idempotency | Deduplicate a repeated ProgressRequestId. | Not run |
| EOBJ-T-096 | Progress requests and idempotency | Accept a new request after the bounded dedupe window evicts an old ID. | Not run |
| EOBJ-T-097 | Progress requests and idempotency | Apply a multi-step mutation batch atomically. | Not run |
| EOBJ-T-098 | Progress requests and idempotency | Roll back a mutation batch when one operation fails validation. | Not run |
| EOBJ-T-099 | Progress requests and idempotency | Publish progress events after state commit. | Not run |
| EOBJ-T-100 | Progress requests and idempotency | Keep event listener failure from rolling back objective truth. | Not run |
| EOBJ-T-101 | Progress requests and idempotency | Handle a provider snapshot step returning complete. | Not run |
| EOBJ-T-102 | Progress requests and idempotency | Handle a provider snapshot step returning incomplete. | Not run |
| EOBJ-T-103 | Progress requests and idempotency | Handle a provider snapshot step returning unavailable. | Not run |
| EOBJ-T-104 | Progress requests and idempotency | Bound per-frame provider evaluation work. | Not run |
| EOBJ-T-105 | Groups, order, optional, and hidden behavior | Activate only the first incomplete child in an ordered group. | Not run |
| EOBJ-T-106 | Groups, order, optional, and hidden behavior | Advance an ordered group after the active child completes. | Not run |
| EOBJ-T-107 | Groups, order, optional, and hidden behavior | Retain completed earlier sequence children. | Not run |
| EOBJ-T-108 | Groups, order, optional, and hidden behavior | Complete an All group after all required children complete. | Not run |
| EOBJ-T-109 | Groups, order, optional, and hidden behavior | Complete an Any group after one eligible child completes. | Not run |
| EOBJ-T-110 | Groups, order, optional, and hidden behavior | Complete a Threshold group after the configured count completes. | Not run |
| EOBJ-T-111 | Groups, order, optional, and hidden behavior | Keep optional children from blocking parent completion. | Not run |
| EOBJ-T-112 | Groups, order, optional, and hidden behavior | Allow optional children to remain active after parent completion only when authored policy permits. | Not run |
| EOBJ-T-113 | Groups, order, optional, and hidden behavior | Close remaining Any-group children using Skip policy. | Not run |
| EOBJ-T-114 | Groups, order, optional, and hidden behavior | Close remaining Any-group children using Cancel policy. | Not run |
| EOBJ-T-115 | Groups, order, optional, and hidden behavior | Keep hidden nodes out of normal presentation snapshots. | Not run |
| EOBJ-T-116 | Groups, order, optional, and hidden behavior | Reveal a hidden node through an authored visibility mutation. | Not run |
| EOBJ-T-117 | Groups, order, optional, and hidden behavior | Complete a hidden node without exposing production text in diagnostics. | Not run |
| EOBJ-T-118 | Groups, order, optional, and hidden behavior | Reject a graph whose closure policy creates an impossible terminal state. | Not run |
| EOBJ-T-119 | Groups, order, optional, and hidden behavior | Recompute aggregate progress deterministically. | Not run |
| EOBJ-T-120 | Groups, order, optional, and hidden behavior | Produce stable presentation ordering independent from dictionary order. | Not run |
| EOBJ-T-121 | Groups, order, optional, and hidden behavior | Prevent a parent group from completing twice. | Not run |
| EOBJ-T-122 | Groups, order, optional, and hidden behavior | Propagate required child failure according to authored policy. | Not run |
| EOBJ-T-123 | Groups, order, optional, and hidden behavior | Ignore optional child failure for required completion unless authored otherwise. | Not run |
| EOBJ-T-124 | Groups, order, optional, and hidden behavior | Bound graph recursion and reject definitions exceeding the configured safety limit. | Not run |
| EOBJ-T-125 | Timers and clocks | Advance a scaled timer with the built-in scaled clock. | Not run |
| EOBJ-T-126 | Timers and clocks | Advance an unscaled timer with the built-in unscaled clock. | Not run |
| EOBJ-T-127 | Timers and clocks | Keep a scaled timer paused while time scale is zero. | Not run |
| EOBJ-T-128 | Timers and clocks | Advance an unscaled timer while time scale is zero. | Not run |
| EOBJ-T-129 | Timers and clocks | Pause a timer while its objective is suspended when policy requires. | Not run |
| EOBJ-T-130 | Timers and clocks | Continue a timer during objective suspension when policy permits. | Not run |
| EOBJ-T-131 | Timers and clocks | Complete a timer at its duration boundary exactly once. | Not run |
| EOBJ-T-132 | Timers and clocks | Fail a timer on expiry when fail-on-expiry is authored. | Not run |
| EOBJ-T-133 | Timers and clocks | Use an explicitly registered project clock provider. | Not run |
| EOBJ-T-134 | Timers and clocks | Return unavailable when a required project clock provider is missing. | Not run |
| EOBJ-T-135 | Timers and clocks | Snapshot elapsed timer state. | Not run |
| EOBJ-T-136 | Timers and clocks | Restore elapsed timer state without double-counting time. | Not run |
| EOBJ-T-137 | Timers and clocks | Reject negative or non-finite clock deltas. | Not run |
| EOBJ-T-138 | Timers and clocks | Keep offline/real-world elapsed progression disabled in the MVP. | Not run |
| EOBJ-T-139 | Repeatability and history | Keep a non-repeatable objective completed after its first run. | Not run |
| EOBJ-T-140 | Repeatability and history | Permit manual reset only when authored. | Not run |
| EOBJ-T-141 | Repeatability and history | Require repeat cooldown before creating a new run. | Not run |
| EOBJ-T-142 | Repeatability and history | Evaluate repeat limits. | Not run |
| EOBJ-T-143 | Repeatability and history | Create unique run IDs across repeated runs. | Not run |
| EOBJ-T-144 | Repeatability and history | Preserve bounded completion history across repeats. | Not run |
| EOBJ-T-145 | Repeatability and history | Compute repeat count from durable terminal records. | Not run |
| EOBJ-T-146 | Repeatability and history | Prevent an old run request from mutating a new run. | Not run |
| EOBJ-T-147 | Repeatability and history | Prune old run history only through explicit configured policy. | Not run |
| EOBJ-T-148 | Repeatability and history | Preserve reward ledgers for retained historical runs. | Not run |
| EOBJ-T-149 | Repeatability and history | Migrate repeat history from an older schema fixture. | Not run |
| EOBJ-T-150 | Repeatability and history | Reject concurrent repeat runs in the MVP. | Not run |
| EOBJ-T-151 | Tracking and presentation snapshots | Set one primary tracked objective. | Not run |
| EOBJ-T-152 | Tracking and presentation snapshots | Clear the primary tracked objective. | Not run |
| EOBJ-T-153 | Tracking and presentation snapshots | Pin objectives up to the configured limit. | Not run |
| EOBJ-T-154 | Tracking and presentation snapshots | Reject or replace pins according to configured policy when full. | Not run |
| EOBJ-T-155 | Tracking and presentation snapshots | Keep deterministic pin ordering. | Not run |
| EOBJ-T-156 | Tracking and presentation snapshots | Auto-untrack a completed objective when policy requires. | Not run |
| EOBJ-T-157 | Tracking and presentation snapshots | Keep a completed objective tracked when policy permits. | Not run |
| EOBJ-T-158 | Tracking and presentation snapshots | Exclude hidden nodes from standard snapshots. | Not run |
| EOBJ-T-159 | Tracking and presentation snapshots | Include structured progress values without resolved localized text. | Not run |
| EOBJ-T-160 | Tracking and presentation snapshots | Publish tracking events after tracking state commits. | Not run |
| EOBJ-T-161 | Reward dispatch and ledgers | Create deterministic reward grant IDs from run and reward identity. | Not run |
| EOBJ-T-162 | Reward dispatch and ledgers | Dispatch rewards only after objective completion commits. | Not run |
| EOBJ-T-163 | Reward dispatch and ledgers | Deliver one reward successfully. | Not run |
| EOBJ-T-164 | Reward dispatch and ledgers | Deliver multiple rewards independently. | Not run |
| EOBJ-T-165 | Reward dispatch and ledgers | Keep objective completion when a reward fails. | Not run |
| EOBJ-T-166 | Reward dispatch and ledgers | Record Pending, Succeeded, Failed, Skipped, and Unavailable delivery states. | Not run |
| EOBJ-T-167 | Reward dispatch and ledgers | Retry only failed or unavailable grants. | Not run |
| EOBJ-T-168 | Reward dispatch and ledgers | Reuse the same grant ID during retries. | Not run |
| EOBJ-T-169 | Reward dispatch and ledgers | Prevent a succeeded grant from executing twice. | Not run |
| EOBJ-T-170 | Reward dispatch and ledgers | Reject a duplicate reward executor type registration. | Not run |
| EOBJ-T-171 | Reward dispatch and ledgers | Handle a missing reward executor without deleting the grant. | Not run |
| EOBJ-T-172 | Reward dispatch and ledgers | Handle a reward executor timeout. | Not run |
| EOBJ-T-173 | Reward dispatch and ledgers | Handle reward cancellation before the executor commit point. | Not run |
| EOBJ-T-174 | Reward dispatch and ledgers | Honor an executor-declared irreversible commit point. | Not run |
| EOBJ-T-175 | Reward dispatch and ledgers | Resume pending grants after state import. | Not run |
| EOBJ-T-176 | Reward dispatch and ledgers | Preserve unknown reward payload records. | Not run |
| EOBJ-T-177 | Reward dispatch and ledgers | Redact reward payloads from ordinary diagnostics. | Not run |
| EOBJ-T-178 | Reward dispatch and ledgers | Bound automatic retry attempts and require explicit action after exhaustion. | Not run |
| EOBJ-T-179 | Persistence and migration | Export a versioned objective-state document. | Not run |
| EOBJ-T-180 | Persistence and migration | Export active, suspended, completed, failed, abandoned, and archived run state. | Not run |
| EOBJ-T-181 | Persistence and migration | Export node progress, timer state, tracking, dedupe window, and reward ledgers. | Not run |
| EOBJ-T-182 | Persistence and migration | Prepare an import without mutating live state. | Not run |
| EOBJ-T-183 | Persistence and migration | Apply a prepared import atomically. | Not run |
| EOBJ-T-184 | Persistence and migration | Reject a stale prepared import after live revision changes. | Not run |
| EOBJ-T-185 | Persistence and migration | Import a document with no active objectives. | Not run |
| EOBJ-T-186 | Persistence and migration | Import a document with missing objective definitions as orphan records. | Not run |
| EOBJ-T-187 | Persistence and migration | Rehydrate orphan records when definitions return. | Not run |
| EOBJ-T-188 | Persistence and migration | Preserve unknown provider payloads. | Not run |
| EOBJ-T-189 | Persistence and migration | Apply contiguous document migrations. | Not run |
| EOBJ-T-190 | Persistence and migration | Apply definition/node aliases during import. | Not run |
| EOBJ-T-191 | Persistence and migration | Reject a newer unsupported document version. | Not run |
| EOBJ-T-192 | Persistence and migration | Back up source data before destructive migration tooling. | Not run |
| EOBJ-T-193 | Persistence and migration | Keep Chronicle as optional save-file transport. | Not run |
| EOBJ-T-194 | Persistence and migration | Register and unregister a Chronicle participant through a separate bridge. | Not run |
| EOBJ-T-195 | Persistence and migration | Recover from a participant payload failure without corrupting other save participants. | Not run |
| EOBJ-T-196 | Persistence and migration | Preserve durable state when EchoObjectives is removed and later reinstalled. | Not run |
| EOBJ-T-197 | Diagnostics and observability | Expose root identity, package version, configuration source, and revision. | Not run |
| EOBJ-T-198 | Diagnostics and observability | Expose counts of definitions, active runs, terminal runs, tracked objectives, and pending rewards. | Not run |
| EOBJ-T-199 | Diagnostics and observability | Expose current provider registrations and unavailable provider types. | Not run |
| EOBJ-T-200 | Diagnostics and observability | Expose bounded recent mutation, lifecycle, reward, and validation histories. | Not run |
| EOBJ-T-201 | Diagnostics and observability | Use the EOBJ diagnostic namespace without collision. | Not run |
| EOBJ-T-202 | Diagnostics and observability | Generate a redacted support snapshot. | Not run |
| EOBJ-T-203 | Diagnostics and observability | Exclude resolved objective titles, descriptions, reward payloads, and project data from default diagnostics. | Not run |
| EOBJ-T-204 | Diagnostics and observability | Publish an optional Observatory provider through a separate bridge. | Not run |
| EOBJ-T-205 | Diagnostics and observability | Avoid per-frame log spam during normal timer and progress updates. | Not run |
| EOBJ-T-206 | Diagnostics and observability | Keep diagnostics failure from changing objective state. | Not run |
| EOBJ-T-207 | Editor tooling and authoring | Create a project-owned configuration and empty catalog through setup tooling. | Not run |
| EOBJ-T-208 | Editor tooling and authoring | Preview every setup change before apply. | Not run |
| EOBJ-T-209 | Editor tooling and authoring | Repeat setup without duplicating assets or IDs. | Not run |
| EOBJ-T-210 | Editor tooling and authoring | Repair a missing root prefab reference without overwriting project data. | Not run |
| EOBJ-T-211 | Editor tooling and authoring | Open the objective authoring window. | Not run |
| EOBJ-T-212 | Editor tooling and authoring | Create a definition with generated stable IDs. | Not run |
| EOBJ-T-213 | Editor tooling and authoring | Duplicate a definition while regenerating domain IDs. | Not run |
| EOBJ-T-214 | Editor tooling and authoring | Validate node and prerequisite cycles. | Not run |
| EOBJ-T-215 | Editor tooling and authoring | Validate missing providers and reward executors. | Not run |
| EOBJ-T-216 | Editor tooling and authoring | Validate impossible group thresholds and closure policies. | Not run |
| EOBJ-T-217 | Editor tooling and authoring | Validate aliases and deprecated IDs. | Not run |
| EOBJ-T-218 | Editor tooling and authoring | Generate a human-readable objective graph report. | Not run |
| EOBJ-T-219 | Editor tooling and authoring | Generate the ADR-001 Workshop plan and apply receipt. | Not run |
| EOBJ-T-220 | Editor tooling and authoring | Cancel an Editor operation without partial destructive changes. | Not run |
| EOBJ-T-221 | Performance and capacity | Profile idle overhead with no active objectives. | Not run |
| EOBJ-T-222 | Performance and capacity | Profile configured maximum active objective count. | Not run |
| EOBJ-T-223 | Performance and capacity | Profile a large but supported objective graph. | Not run |
| EOBJ-T-224 | Performance and capacity | Profile timer updates at configured capacity. | Not run |
| EOBJ-T-225 | Performance and capacity | Profile provider evaluation budget. | Not run |
| EOBJ-T-226 | Performance and capacity | Profile snapshot creation and event publication. | Not run |
| EOBJ-T-227 | Performance and capacity | Profile reward-ledger retry scanning. | Not run |
| EOBJ-T-228 | Performance and capacity | Reject capacities beyond configured hard limits gracefully. | Not run |
| EOBJ-T-229 | Integration bridges | Resolve objective text references through Many Tongues without a core dependency. | Not run |
| EOBJ-T-230 | Integration bridges | Present objective snapshots through Looking Glass without moving objective truth. | Not run |
| EOBJ-T-231 | Integration bridges | Read dialogue conditions through a Voices bridge. | Not run |
| EOBJ-T-232 | Integration bridges | Request objective mutations from explicit Voices command handlers. | Not run |
| EOBJ-T-233 | Integration bridges | Deliver item rewards through an Inventory reward executor. | Not run |
| EOBJ-T-234 | Integration bridges | Read item-count conditions through an Inventory condition provider. | Not run |
| EOBJ-T-235 | Integration bridges | Request progression grants through an Ascent reward executor. | Not run |
| EOBJ-T-236 | Integration bridges | Read progression prerequisites through an Ascent condition provider. | Not run |
| EOBJ-T-237 | Integration bridges | Persist objective state through Chronicle. | Not run |
| EOBJ-T-238 | Integration bridges | Receive semantic interaction progress through The Hand adapter. | Not run |
| EOBJ-T-239 | Integration bridges | Receive scene/location progress through a Passage or World adapter. | Not run |
| EOBJ-T-240 | Integration bridges | Acquire game-state-aware timer policy through a Pulse adapter without time authority duplication. | Not run |
| EOBJ-T-241 | Integration bridges | Expose diagnostics through Observatory. | Not run |
| EOBJ-T-242 | Integration bridges | Validate definitions before builds through Foundry. | Not run |
| EOBJ-T-243 | Integration bridges | Generate setup plans through Workshop. | Not run |
| EOBJ-T-244 | Integration bridges | Remove each optional bridge and retain core compilation. | Not run |
| EOBJ-T-245 | Integration bridges | Handle bridge version mismatch visibly. | Not run |
| EOBJ-T-246 | Integration bridges | Handle bridge teardown while no operation is in a commit phase. | Not run |
| EOBJ-T-247 | Integration bridges | Keep Multiplayer authority deferred behind an adapter contract. | Not run |
| EOBJ-T-248 | Integration bridges | Keep UI, audio, camera, input, and save authorities outside the core. | Not run |
| EOBJ-T-249 | Upgrade, removal, accessibility, platform, and release | Upgrade from the previous supported package version using migration fixtures. | Not run |
| EOBJ-T-250 | Upgrade, removal, accessibility, platform, and release | Preserve public asset GUIDs across a package update. | Not run |
| EOBJ-T-251 | Upgrade, removal, accessibility, platform, and release | Preserve project-owned definitions during package update. | Not run |
| EOBJ-T-252 | Upgrade, removal, accessibility, platform, and release | Remove samples without breaking runtime. | Not run |
| EOBJ-T-253 | Upgrade, removal, accessibility, platform, and release | Remove bridges before core package removal. | Not run |
| EOBJ-T-254 | Upgrade, removal, accessibility, platform, and release | Keep unresolved objective records in save data after package removal. | Not run |
| EOBJ-T-255 | Upgrade, removal, accessibility, platform, and release | Provide text-scale-friendly structured progress values to presenters. | Not run |
| EOBJ-T-256 | Upgrade, removal, accessibility, platform, and release | Provide color-independent lifecycle and failure states. | Not run |
| EOBJ-T-257 | Upgrade, removal, accessibility, platform, and release | Provide user-configurable timer-display precision through presentation policy. | Not run |
| EOBJ-T-258 | Upgrade, removal, accessibility, platform, and release | Keep critical completion/failure information available without audio alone. | Not run |
| EOBJ-T-259 | Upgrade, removal, accessibility, platform, and release | Validate Windows behavior. | Not run |
| EOBJ-T-260 | Upgrade, removal, accessibility, platform, and release | Validate macOS behavior. | Not run |
| EOBJ-T-261 | Upgrade, removal, accessibility, platform, and release | Validate Linux behavior. | Not run |
| EOBJ-T-262 | Upgrade, removal, accessibility, platform, and release | Validate WebGL behavior. | Not run |
| EOBJ-T-263 | Upgrade, removal, accessibility, platform, and release | Validate mobile behavior. | Not run |
| EOBJ-T-264 | Upgrade, removal, accessibility, platform, and release | Keep console support Planned until platform evidence exists. | Not run |
| EOBJ-T-265 | Upgrade, removal, accessibility, platform, and release | Pass Beta gate evidence requirements. | Not run |
| EOBJ-T-266 | Upgrade, removal, accessibility, platform, and release | Pass Release Candidate gate evidence requirements. | Not run |
| EOBJ-T-267 | Upgrade, removal, accessibility, platform, and release | Pass Stable gate evidence requirements. | Not run |
| EOBJ-T-268 | Upgrade, removal, accessibility, platform, and release | Install the distributable tarball in another clean project. | Not run |

### 23.4 Evidence rules

Every case above is a **planned test definition**, not executed evidence. Executions require environment, version, commit, inputs, expected/actual result, evidence location, issue link, and reviewer under SFGSS-004. Retries and flaky results remain visible.

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership/non-ownership approved.
- [x] MVP/deferred scope separated.
- [x] Data, IDs, graph, lifecycle, providers, rewards, persistence, and failure behavior defined.
- [x] Laboratory and planned test registry complete.
- [x] No release-blocking design question remains.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared dependencies only.
- [ ] Editor isolated from runtime.
- [ ] Duplicate/lifecycle/direct-scene behavior validated.
- [ ] Definition assets remain immutable at runtime.
- [ ] Progress and reward idempotency validated.
- [ ] Setup/repair repeat safely.
- [ ] Public API matches this specification or specification/ADR changes first.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Package works with no peer Echo package.
- [ ] Laboratory passes all required scenarios.
- [ ] Samples remove safely.
- [ ] Direct-scene behavior matches docs.
- [ ] Fake providers prove missing/failure paths.

### 24.4 Quality gate

- [ ] Automated and manual tests pass for the target release.
- [ ] No Blocker/Critical defect remains.
- [ ] Performance/capacity evidence passes.
- [ ] Diagnostics and redaction are actionable.
- [ ] Migration/orphan/reward recovery tests pass.
- [ ] Documentation matches implementation.
- [ ] Current Notes reconciled.
- [ ] Licenses/notices complete.

### 24.5 Distribution gate

- [ ] Manifest/version/changelog valid.
- [ ] Stable `.meta` files included.
- [ ] Git and tarball installs tested externally.
- [ ] Beta, Release Candidate, or Stable evidence gate satisfied under SFGSS-004.
- [ ] Repository release/tag prepared.
- [ ] Compatibility catalog updated.
- [ ] Documentation/status committed and pushed.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Echo Systems Lab | Mission/target progress | Map one mission into objective definition and adapter | Existing mission loop/save/HUD parity | Keep original mission system |
| Hackulos | Authored starter quests | Build one non-repeatable and one repeatable objective flow | Dialogue, inventory, combine, rewards, save parity | Preserve project quest prototype |
| Rescuers2D | Rescue/level goals | Adapt one survivor-count or role-action objective | Win flow and HUD parity | Keep current level controller |
| New clean project | None | Install Laboratory and author one objective | Standalone checklist | Remove package/sample |

### 25.2 Preserve-until-parity rule

Existing systems remain intact while EchoObjectives proves:

1. standalone behavior;
2. content mapping;
3. progress parity;
4. reward and persistence parity;
5. UI/dialogue integration parity;
6. rollback/reinstall safety.

Removal occurs only after documented parity and backup.

### 25.3 Migration tooling

Later implementation should provide:

- inventory of existing mission/quest IDs and state;
- dry-run mapping to ObjectiveId/NodeId/Run records;
- alias generation;
- backup of source data;
- conversion into project-owned definitions/state fixtures;
- validation and parity report;
- rollback instructions;
- no direct destructive conversion without explicit approval.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EOBJ-R-001 | Scope grows into universal gameplay scripting | High | High | Explicit node/provider boundaries and deferred features | Any new gameplay-specific node proposal |
| EOBJ-R-002 | Objective truth leaks into UI/dialogue/inventory | Medium | High | Snapshots, commands, bridges, ownership tests | Cross-package review |
| EOBJ-R-003 | Graph cycles/impossible completion | Medium | High | Editor validation and blocked definitions | Authoring/preflight |
| EOBJ-R-004 | Stale requests mutate repeated run | Medium | High | Run IDs/generations and dedupe | Repeat tests |
| EOBJ-R-005 | Reward duplicates after failure/load | Medium | Critical | Deterministic grant IDs and persistent ledger | Reward integration |
| EOBJ-R-006 | Reward failure rolls back objective truth | Medium | High | Completion-first, independent delivery contract | Provider implementation |
| EOBJ-R-007 | Mutable ScriptableObject contamination | Medium | High | Separate runtime models and tests | Play/reload tests |
| EOBJ-R-008 | Missing provider silently grants progress | Medium | High | Unavailable result, never implicit success | Provider evaluation |
| EOBJ-R-009 | Timer semantics differ across games | High | Medium | Explicit clock/pause policy and provider seam | Timer integration |
| EOBJ-R-010 | Save migration loses renamed nodes | Medium | High | Stable IDs, aliases, orphan preservation | Version changes |
| EOBJ-R-011 | Unbounded histories/repeats consume memory | Medium | Medium | Configured capacities and explicit prune | Stress tests |
| EOBJ-R-012 | Diagnostics leak story/reward content | Low | Medium | ID-only redaction by default | Support export review |
| EOBJ-R-013 | Multiplayer clients become authoritative | Medium | Critical | Defer and require server/provider adapter | Multiplayer work |
| EOBJ-R-014 | Setup tool overwrites project assets | Low | High | Create-only-safe preview/receipts | Editor tests |
| EOBJ-R-015 | Sample becomes hidden dependency | Low | High | Assembly isolation and removal tests | Release gate |
| EOBJ-R-016 | Definition changes invalidate active state | Medium | High | Content versioning, migrations, blocked incompatibility | Upgrade/migration |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequence | ADR required |
|---|---|---|---|---|---:|
| EOBJ-D-001 | One duplicate-safe application-session objective authority | Approved | One truth owner | No independent quest-manager singletons | No |
| EOBJ-D-002 | Definitions are immutable project-owned assets; runs are separate mutable state | Approved | SFGSS-003 safety | Repeat/save/test behavior remains deterministic | No |
| EOBJ-D-003 | Explicit rooted acyclic node records with Ordered/All/Any/Threshold groups | Approved | Supports core structures without visual-scripting scope | Cycles blocked in validation | No |
| EOBJ-D-004 | Leaf steps are Manual, Counter, Flag, Timer, or Provider in MVP | Approved | Small complete vocabulary | Genre-specific steps use adapters/providers | No |
| EOBJ-D-005 | Optional and hidden are independent policies | Approved | Completion and presentation are separate | Hidden never changes truth |
| EOBJ-D-006 | Progress uses typed requests, atomic batches, and bounded idempotency | Approved | Prevent duplicate/inconsistent mutation | Callers supply request IDs when needed | No |
| EOBJ-D-007 | One active run per definition; sequential repeats only | Approved | Honest MVP | Concurrent runs deferred | No |
| EOBJ-D-008 | Completion commits before reward delivery | Approved | Avoid unsafe cross-authority rollback | Failed rewards remain ledgered/retryable | No |
| EOBJ-D-009 | Reward grant IDs are deterministic and persistent | Approved | Prevent duplicate grants | Executors must respect idempotency |
| EOBJ-D-010 | One primary tracked objective plus bounded pins | Approved | Neutral useful presentation state | UI remains external | No |
| EOBJ-D-011 | State export/import is core; Chronicle is optional | Approved | Standalone independence | Package chooses no file/slot |
| EOBJ-D-012 | Missing definitions/providers preserve inactive/orphan state | Approved | Clean removal/reinstall | Explicit prune required |
| EOBJ-D-013 | Runtime providers register explicitly; reflection discovery prohibited | Approved | Visible dependency and teardown | More setup, safer behavior | No |
| EOBJ-D-014 | Editor MVP uses structured UI Toolkit authoring, not an experimental graph dependency | Approved | Stable delivery scope | Rich graph canvas later |
| EOBJ-D-015 | Diagnostic namespace is `EOBJ-*` | Approved | Unique searchable identity | Codes cannot be recycled | No |

### 27.2 Release-blocking questions

None remain for specification approval. Implementation must still verify exact Unity/Test Framework versions, serialization shapes, allocation budgets, clock behavior, and bridge package IDs before release claims.

### 27.3 Non-blocking later questions

- Should a later version support concurrent runs of one definition?
- Which offline/real-time timer providers are safe and useful?
- Do reusable objective subgraphs deserve a separate definition type?
- How should world/map marker metadata connect to EchoWorld and EchoCamera?
- Which multiplayer authority model will validate shared objectives?
- Is a richer graph authoring canvas worth its maintenance/dependency cost?
- Should objective completion and reward grants support an optional provider-coordinated transaction protocol later?

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included | Evidence |
|---|---|---|---|
| M0 | Approved specification | Design only | This document |
| M1 | Package skeleton | Manifest, assemblies, docs shell | Clean compile/install |
| M2 | Definitions and validation core | IDs, catalog, graph records, pure validators | EditMode tests |
| M3 | Runtime authority | Root, availability, runs, lifecycle, groups, progress | PlayMode tests |
| M4 | Timers, repeat, tracking | Clock policies, history, snapshots | Tests/Lab |
| M5 | Reward ledger and state documents | Grants, retries, export/import/migrations | Tests/fixtures |
| M6 | Editor tooling/Laboratory | Setup, authoring, validation, sample | Manual/automated checklist |
| M7 | First bridges/adoption | UI/Dialogue/Save or project target | Integration Lab/parity report |
| M8 | Release | Distribution docs/evidence | External install/release gate |

### 28.2 Checkpoint rule

Every implementation milestone is split into small SFGSS-005 Checkpoint Build Plans. Each checkpoint must show complete code in the conversation, explain every file and important section, list exact Unity setup, define expected results/tests, stop at a safe boundary, and reconcile documentation before advancing.

### 28.3 First recommended implementation checkpoint

After SUITE-DOC-33 unlocks code:

> **EOBJ-M1-01 - EchoObjectives Package Skeleton**

Create only package manifest, assembly definitions, documentation shell, test assemblies, and clean compile/install evidence. Do not implement runtime behavior in the skeleton checkpoint.

---

## 29. New-Conversation Handoff

```text
We are continuing The Sperk's Forge - EchoObjectives (`The Path`).

Treat SFGSS-000 as suite authority, SFGSS-002 for dependencies/assemblies,
SFGSS-003 for IDs/data/migration, SFGSS-004 for evidence/release truth,
SFGSS-005 for implementation teaching workflow, and this approved specification
as the Level 2 package authority.

Current package: EchoObjectives
Specification: v1.0.0 Approved
Implementation: locked until SUITE-DOC-33
Current documentation checkpoint: SUITE-DOC-12 - EchoInventory (`The Vault`)

Before writing code:
1. Preserve one objective progress/lifecycle authority.
2. Keep gameplay facts, UI, dialogue, inventory, progression, save, and rewards
   behind explicit adapters/bridges.
3. Preserve stable IDs, run generations, completion-first reward ledgers,
   state migrations, and orphan records.
4. Keep every unexecuted test marked Not run.
5. Follow the learning-oriented checkpoint workflow and show complete code.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | Specification v1.0.0; implementation not started |
| Completed checkpoint | SUITE-DOC-11 - The Path specification |
| Files/assets created | Documentation only |
| Tests passed | Documentation structure/ID/archive audits only |
| Tests failed | None in documentation audit |
| Runtime tests | All Not run |
| Known issues | Implementation evidence, exact dependency versions, and bridge IDs pending |
| Next checkpoint | SUITE-DOC-12 - The Vault (`EchoInventory`) specification |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and plain responsibility are clear.
- [x] Ownership/non-ownership align with SFGSS-000.
- [x] UI, localization, dialogue, inventory, progression, save, interaction, game state, build, and multiplayer boundaries are explicit.
- [x] Independence proof is credible.
- [x] MVP is bounded and independently useful.
- [x] IDs, graph, lifecycle, progress, timers, repeat, tracking, rewards, persistence, and failure behavior are specified.
- [x] Completion and reward-delivery truth are separated safely.
- [x] Laboratory and planned evidence registry are complete.
- [x] Diagnostics work without Observatory.
- [x] Optional integrations remain removable.
- [x] No Isekai Studios identity or ownership was introduced.
- [x] Jesse approved the package-first documentation program that authorizes this specification work.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Package implementation remains locked until SUITE-DOC-33. Runtime, platform, performance, migration, bridge, and release evidence remains `Not run` until executed.

---

## Specification Completion Statement

A new collaborator can determine what EchoObjectives owns, what it refuses to own, how availability and objective graphs work, how progress is mutated safely, how repeat runs and timers behave, why completion does not roll back when rewards fail, how reward idempotency works, how state is persisted and migrated, how optional packages connect, how the Standalone Laboratory proves the core, and what evidence is required before release without consulting an old conversation.


---

## Graph Navigation

#sfgss/package #sfgss/wave/expansion #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
