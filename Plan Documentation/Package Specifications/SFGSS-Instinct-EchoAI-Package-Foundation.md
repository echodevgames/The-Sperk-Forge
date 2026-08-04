# Instinct - EchoAI Feasibility Foundation Specification

**Document ID:** SFGSS-PKG-ECHOAI  
**Specification version:** 1.0.0  
**Status:** Approved feasibility foundation; EchoAI remains an Advanced candidate and implementation remains locked  
**Technical package name:** EchoAI  
**Public title:** Instinct - AI Perception, Decisions, and Behavior  
**Package ID:** `com.echodevgames.echo-ai`  
**Runtime namespace:** `EchoDevGames.EchoAI`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Repository:** Planned `EchoDevGames/EchoAI`  
**Current Notes:** `../Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1; optional backend package versions are research observations, not approved implementation pins  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Required feasibility record:** `../Research Records/SUITE-DOC-19_EchoAI_Feasibility_and_Provider_Record.md`  
**Last updated:** August 4, 2026

> “Instinct notices, remembers, weighs, and acts. The game still decides what kind of creature is thinking.”

> **Approval rule:** This document approves the Level 2 provider-neutral foundation for EchoAI boundaries, data contracts, lifecycle, Laboratory design, adapter seams, and pre-code evidence. It does not approve implementation, one universal enemy brain, a mandatory navigation backend, a Behavior Graph dependency, neural-network inference, or production performance claims. Those remain blocked until SUITE-DOC-33 and later implementation evidence.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial feasibility foundation | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved provider-neutral sensing, memory, scoring, blackboard, scheduling, behavior, navigation, diagnostics, adapter, Laboratory, and explicit non-goal contracts | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** Instinct - AI Perception, Decisions, and Behavior  
**Technical identifier:** EchoAI  
**Flavor line:** Instinct notices, remembers, weighs, and acts.  
**Plain-language subtitle:** A provider-neutral Unity package foundation for actor-local AI sensing, perception memory, target scoring, typed context, decision scheduling, lightweight behavior execution, navigation requests, debugging, and optional backend adapters.

**One-sentence ownership contract:**

> EchoAI owns actor-local AI runtime coordination, semantic stimulus and observation contracts, bounded perception memory, target scoring, typed blackboard/context, decision cadence and budgets, lightweight behavior/state-machine contracts, navigation request/status abstractions, debug traces, validation, and adapter compliance; it does not own a game’s enemy personality, combat rules, abilities, movement physics, animation, navigation technology, world state, dialogue, objectives, UI, audio, save transport, multiplayer authority, machine-learning training, or one universal behavior architecture.

### 1.1 Elevator summary

Instinct supplies the reusable plumbing beneath game-authored AI. A sensor reports that something was seen, heard, damaged, touched, smelled, or otherwise observed. The agent remembers the observation according to a bounded policy, scores possible targets, evaluates typed context, selects an available behavior, asks explicit executors or navigation providers to act, and publishes traceable results.

The package deliberately stops before enemy design. “Patrol,” “investigate,” “chase,” “flee,” and “return” may appear in samples, but they are examples built from neutral contracts. A rat, guard, companion, vehicle, puzzle actor, or boss may share the infrastructure without inheriting one genre’s threat model or one mandatory behavior tree.

The neutral core has no hard dependency on Unity AI Navigation, Unity Behavior, Inference Engine, The Vessel, Clash, Arcana, The Atlas, or The Convergence. Those systems connect through separately versioned adapters or project code.

### 1.2 Why this belongs in The Sperk's Forge

Existing and planned games repeatedly need sensory detection, memory, target selection, state changes, pursuit, fleeing, and debugging. Rebuilding these as one-off enemy scripts creates duplicated physics queries, hidden target references, unbounded update work, and behavior logic welded directly to movement, combat, and animation.

Instinct captures the reusable infrastructure while preserving project creativity. It also creates one clean seam for future advanced packages: Clash can publish damage/threat context, Arcana can expose ability availability, The Atlas can provide world/tactical context, The Vessel can execute movement intent, and The Convergence can declare server authority.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Always paired with “AI Perception, Decisions, and Behavior.” |
| Setup guidance/tooltips | Yes | Must explain sensors, memory, scores, blackboards, states, actions, and providers plainly. |
| Samples | Optional | Verse-flavored creatures may appear but remain replaceable. |
| Runtime API/type names | No lore-only names | Use names such as `AIAgentHost`, `AIObservation`, and `IAINavigationProvider`. |
| Project data | No required Verse content | Games own creatures, factions, tactics, animations, and content. |

---

## 2. Problem Statement

### 2.1 Current problem

AI code often begins as one MonoBehaviour containing detection, target selection, movement, combat, animation, timers, and state transitions. As features grow, every behavior reads and writes shared fields directly. Physics queries run every frame, stale target references survive despawns, multiple systems compete to move the actor, and debugging becomes a pile of temporary logs.

A reusable package must not replace this with an equally monolithic “universal AI manager.” It needs explicit agent-local authority, bounded memory, deterministic scoring, typed context, controlled scheduling, provider seams, and traceable decisions. It must also remain useful in 2D, 3D, grid, navigation-mesh, custom pathfinding, and network-authoritative projects.

### 2.2 Evidence from current architecture and provider research

| Source | Need/finding | Preserve | Improve |
|---|---|---|---|
| SFGSS-000 | Sensor, memory, scoring, state/behavior, navigation abstraction, debug samples | Package-neutral scope | Turn candidate bullets into explicit contracts |
| The Fellowship | Durable character identity and actor spawn lifecycle | Separate identity layers | AI agent identity must not become character or GameObject identity |
| The Vessel | Actor-local movement authority and normalized intent | Motor owns physics | AI requests movement; it does not mutate controller internals |
| The Hand | Offer discovery and execution requests | Request/result boundaries | AI may choose interactions without owning their outcomes |
| The Eye | Provider-neutral view requests | One authority per concern | AI debugging may observe camera but never control it implicitly |
| The Convergence | Authoritative host/server validation | Explicit capability/authority | AI decisions are authoritative on the selected server/host model |
| Unity AI Navigation research | Optional NavMesh package for navigation meshes, agents, links, and obstacles | Mature 3D navigation candidate | Keep it in a separate adapter and do not impose it on 2D/custom projects |
| Unity Behavior research | Visual modular behavior graphs and blackboards | Optional visual authoring path | Do not make graph assets the neutral runtime contract |
| Inference Engine research | Runtime neural-network inference | Future experimental extension | Do not confuse inference with AI authority, behavior design, or model training |

### 2.3 Consequences of doing nothing

- AI logic remains welded to controllers, combat, and animation.
- Every agent scans the world every frame.
- Perception memory grows without limits or explanation.
- Target switching jitters between nearly equal candidates.
- State transitions race with asynchronous actions.
- Navigation APIs leak into game logic.
- 2D games inherit 3D NavMesh assumptions.
- Multiplayer clients may run untrusted authoritative AI.
- Debugging cannot explain why an agent chose one action.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide actor-local, testable AI authority.
- Separate observation, memory, scoring, decisions, behavior execution, and navigation.
- Keep sensors and providers explicit and removable.
- Bound memory, scheduling, traces, candidate counts, and asynchronous work.
- Make target and behavior decisions deterministic under the same inputs, clock, and random seed.
- Provide typed blackboard/context data instead of arbitrary string/object bags.
- Provide one lightweight state-machine path without claiming to solve every behavior architecture.
- Support custom and vendor navigation adapters.
- Expose clear diagnostics and score/transition explanations.
- Remain independent of combat, abilities, controllers, characters, world, multiplayer, and UI packages.

### 3.2 Non-goals

- Ship one universal enemy brain or genre-specific threat model.
- Own movement physics, pathfinding implementation, combat, abilities, animation, or world simulation.
- Require Unity AI Navigation, Unity Behavior, Inference Engine, or one third-party AI asset.
- Train neural networks or author model datasets.
- Promise deterministic cross-platform simulation when providers are nondeterministic.
- Save every live memory, path, sensor, or action.
- Perform automatic scene-wide discovery each frame.
- Provide cheating-resistant multiplayer clients.
- Replace project-authored behavior, tactics, or content.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project | Import the neutral core and run a deterministic AI Laboratory without another Echo package |
| Programmer | Existing enemy code | Move reusable sensing, memory, scoring, scheduling, and traces behind stable contracts |
| Designer | Authoring a creature | Configure profiles, states, transitions, memory, and scoring without editing core code |
| AI programmer | Custom planner/navigation stack | Implement provider contracts without forking the neutral core |
| Tester | Reproducing odd decisions | Inspect observation, score, blackboard, transition, and navigation traces |
| Multiplayer developer | Server-authoritative game | Run decisions on the authoritative peer and publish presentation snapshots to clients |

### 3.4 Measurable success criteria

- Neutral core compiles with no optional AI/navigation/behavior package installed.
- One scripted Laboratory agent perceives, remembers, scores, changes state, and completes simulated navigation.
- Agent decisions are reproducible with the same deterministic fixtures.
- Removing optional adapters does not break the core.
- No agent requires a global persistent AI singleton.
- Every runtime collection and trace has a configured bound.
- A score or transition can explain why it occurred.
- No production compatibility or performance claim is marked Supported before evidence exists.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Unity gameplay programmers.
- Designers authoring small to medium behavior sets.
- Package bridge and provider-adapter authors.
- QA testers and maintainers.
- Future network, combat, ability, and world-system developers.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| EAI-UC-001 | Register an AI agent | Project/scene | Valid profile and world | Agent becomes Ready with bounded runtime state | MVP |
| EAI-UC-002 | Submit an observation | Sensor/provider | Valid source and stimulus type | Memory records or updates the observation | MVP |
| EAI-UC-003 | Decay and expire memory | Scheduler | Time advances | Confidence and retention follow policy | MVP |
| EAI-UC-004 | Select a target | Agent | Candidate observations exist | Deterministic winner or no-target result | MVP |
| EAI-UC-005 | Read/write typed context | Agent/provider | Valid schema | Blackboard revision updates safely | MVP |
| EAI-UC-006 | Run a state transition | Behavior runner | Guard satisfied | Exit, transition, and enter occur atomically | MVP |
| EAI-UC-007 | Execute an asynchronous action | Agent | Executor registered | Result, timeout, cancellation, and stale completion are explicit | MVP |
| EAI-UC-008 | Request navigation | Behavior | Provider available | Ticket and status are returned without backend leakage | MVP |
| EAI-UC-009 | Explain a decision | Tester | Trace enabled | Memory, score, state, and action reasons are inspectable | MVP |
| EAI-UC-010 | Use a NavMesh provider | 3D project | Adapter installed | Neutral navigation maps to Unity AI Navigation | Later adapter |
| EAI-UC-011 | Use a visual graph | Designer | Unity Behavior adapter installed | Graph execution maps to neutral context/providers | Later adapter |
| EAI-UC-012 | Run authoritative AI | Multiplayer bridge | Server/host authority active | Clients cannot authoritatively choose AI outcomes | Later bridge |

### 4.3 Explicitly unsupported use cases

- Asking EchoAI to calculate damage or activate an ability directly.
- Treating a Transform or GameObject name as durable AI identity.
- Requiring all agents to use the same state machine, behavior tree, or navigation backend.
- Running uncontrolled scene-wide physics searches every frame.
- Persisting live path-provider handles or async action tickets.
- Trusting client-side AI as shared-world authority by default.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Agent-local runtime coordination.
- World/scene registration boundaries for AI participants and stimuli.
- Semantic stimulus and observation validation.
- Bounded perception memory and confidence/expiry policy.
- Target filtering, scoring, hysteresis, and explanations.
- Typed blackboard schemas and authority-owned runtime values.
- Sensor and decision scheduling, budgets, and deterministic fixtures.
- Lightweight state-machine and utility-selection foundations.
- Behavior executor lifecycle, cancellation, timeout, and stale completion protection.
- Navigation request/status contracts and provider capability negotiation.
- AI diagnostics, traces, gizmos, validation, and Laboratories.

### 5.2 The package does not own

- Character identity, roster, or spawning.
- Movement physics or controller state.
- Combat, damage, teams, factions, abilities, inventory, crafting, objectives, or interactions.
- World/zone durable truth or scene travel.
- Animation, audio, VFX, camera, or production UI.
- Save files or settings persistence.
- Multiplayer session or network authority implementation.
- Navigation meshes, pathfinding algorithms, behavior graph vendors, or neural-model training.
- Game-specific enemy personality, tactics, or content.

### 5.3 Neighboring authorities

| Other authority/provider | Connection type | Owner | Data/events exchanged | Required? |
| --- | --- | --- | --- | --- |
| The Fellowship / EchoCharacters | Separate bridge | EchoAI bridge package | Character/actor identity, spawn/despawn, control-owner changes, availability snapshots | No |
| The Vessel / EchoControllers | Separate bridge or project adapter | Bridge/project | Navigation/locomotion intent requests and motor status; no direct Rigidbody mutation by core | No |
| Clash / EchoCombat | Future separate bridge | Bridge | Damage, threat, teams, targetability, defeat events, combat requests | No |
| Arcana / EchoAbilities | Future separate bridge | Bridge | Ability availability, targeting requests, activation results, cast interruption | No |
| The Atlas / EchoWorld | Future separate bridge | Bridge | Zone/location context, patrol/tactical points, durable world state | No |
| The Hand / EchoInteraction | Separate bridge | Bridge/project | Interaction offers, execution requests, and results | No |
| The Eye / EchoCamera | Project adapter | Project | Debug/spectator focus only; AI does not own camera | No |
| Impact / EchoFeedback | Project adapter | Project | Semantic feedback request after AI/gameplay events | No |
| Resonance / Jukebot | Project adapter | Project | Semantic audio requests and sound-stimulus generation | No |
| The Path / EchoObjectives | Project adapter | Project | Objective events may affect AI context; AI does not mutate objective truth | No |
| The Chronicle / EchoSave | Separate bridge/project participant | Bridge/project | Detached project-approved durable AI snapshot at safe points | No |
| The Convergence / EchoMultiplayer | Separate bridge | Bridge | Authoritative host/server decisions, replication snapshots, client presentation | No |
| Unity AI Navigation | Provider adapter package | EchoAI adapter | NavMesh destination, path status, stop, warp, and capabilities | No |
| Unity Behavior | Provider adapter package | EchoAI adapter | Visual behavior graph execution mapped to AI context/providers | No |
| Unity Inference Engine | Experimental provider adapter | EchoAI adapter | Model inference only; training and game authority remain external | No |

### 5.4 Boundary tests

1. Does the feature explain reusable perception, memory, selection, decision, or behavior lifecycle?
2. Would it remain useful without Clash, Arcana, The Atlas, The Vessel, or The Convergence?
3. Is the feature an AI request or the neighboring authority’s final mutation?
4. Does it force one navigation, behavior-tree, physics, or inference technology?
5. Is changing game-specific personality possible without editing package source?
6. Can the package be removed without deleting project-owned creatures or world data?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

EchoAI must:

- Compile with only declared Unity dependencies.
- Run its Laboratory with simulated sensors, scorers, actions, and navigation.
- Avoid a mandatory persistent root or First Light dependency.
- Keep agent hosts actor-local and world/scheduler services explicit.
- Avoid direct project-assembly references.
- Keep optional providers in separate assemblies/packages.
- Expose injected clock, random, sensor, scorer, behavior, and navigation seams.
- Fail visibly when optional providers are absent.
- Remove samples without breaking runtime code.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Evidence |
|---|---|---|
| Installed alone | Core compiles and deterministic Laboratory imports | EAI-T-001 through EAI-T-032 |
| No navigation adapter | Simulated provider or explicit Unavailable results | EAI-T-345 through EAI-T-392 |
| No Behavior package | Core state machine and interfaces remain usable | EAI-T-289 through EAI-T-344 |
| No combat/world/controller packages | Neutral observations and scripted executors still work | EAI-LAB-001 through EAI-LAB-080 |
| Duplicate actor host | Duplicate rejects before side effects | EAI-LAB-002 |
| Missing configuration | Agent blocks safely with diagnostics | EAI-LAB-075 |
| Samples deleted | Runtime and Editor assemblies compile | EAI-T-020 |
| Adapter removed | Core and unrelated bridges remain compile-safe | EAI-T-470 through EAI-T-480 |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Planned version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine | Platform | Yes | Unity 6000.0 floor; 6000.3.8f1 primary baseline | Components, vectors, serialization, Editor integration | Package cannot operate without Unity |
| Unity Test Framework | Test | Tests only | Resolve at implementation | EditMode/PlayMode evidence | Runtime unaffected |
| AI Navigation | Provider adapter | No | Research observed 2.0.14 | Optional NavMesh provider | Adapter removed first |
| Unity Behavior | Provider adapter | No | Research observed 1.0.16 | Optional visual behavior authoring/execution | Adapter removed first |
| Inference Engine | Experimental provider | No | Research observed 2.6.1 | Optional runtime model inference | Adapter removed first |

### 6.4 Forbidden dependencies

- Project assemblies.
- Mandatory Echo package peers.
- A mandatory behavior-tree or navigation vendor.
- Runtime references to Editor assemblies.
- Samples as runtime dependencies.
- Reflection-based discovery of arbitrary actions or conditions.
- Hidden scene names, layers, tags, input maps, or Resources paths.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
| --- | --- | --- | --- | --- | --- |
| EAI-CAP-001 | Actor-local agent host | Owns one agent runtime, provider handles, memory, blackboard, decisions, behavior, and diagnostics | Approved | Yes | Runtime |
| EAI-CAP-002 | Scene-local world registry | Explicit world boundary for agents, stimuli, and queries; multiple worlds may coexist | Approved | Yes | Runtime |
| EAI-CAP-003 | Semantic stimulus contracts | Stable stimulus types, sources, revisions, locations, confidence, tags, and provider payload seams | Approved | Yes | Runtime |
| EAI-CAP-004 | Sensor provider contracts | Actor-local or world sensors submit validated observations without owning memory | Approved | Yes | Runtime |
| EAI-CAP-005 | Perception memory | Bounded records, confidence, last-known values, decay, expiry, pinning, and explicit forgetting | Approved | Yes | Runtime |
| EAI-CAP-006 | Target candidate scoring | Deterministic filter/contribution pipeline with thresholds, hysteresis, ties, and explanations | Approved | Yes | Runtime |
| EAI-CAP-007 | Typed blackboard | Schema-driven agent/world context with typed keys, scopes, revisions, and provider-owned values | Approved | Yes | Runtime |
| EAI-CAP-008 | Decision scheduling | Separate sensor/decision cadence, manual tick, budgets, priority, fairness, and overrun reporting | Approved | Yes | Runtime |
| EAI-CAP-009 | Lightweight state machine runner | Flat authored states, guards, transitions, action executors, timeout, and cancellation | Approved | Yes | Runtime |
| EAI-CAP-010 | Utility behavior selector | Scores available behavior candidates without becoming a genre-specific brain | Approved | Yes | Runtime |
| EAI-CAP-011 | Behavior executor contracts | Typed synchronous/asynchronous actions with cancellation, timeout, and stale completion protection | Approved | Yes | Runtime |
| EAI-CAP-012 | Navigation abstraction | Destination requests, tickets, status, stop, warp, capabilities, and provider diagnostics | Approved | Yes | Runtime |
| EAI-CAP-013 | Deterministic test services | Injected clock, random source, sensors, scorers, behavior executors, and navigation simulation | Approved | Yes | Runtime/Test |
| EAI-CAP-014 | Debug traces and gizmos | Memory, scores, transitions, scheduling, navigation, and provider health | Approved | Yes | Runtime/Editor |
| EAI-CAP-015 | Validation and setup | Create assets, validate IDs/schemas/transitions/providers, and open the Laboratory | Approved | Yes | Editor |
| EAI-CAP-016 | Unity AI Navigation adapter | Optional NavMesh-backed navigation provider package | Proposed | No | Adapter |
| EAI-CAP-017 | Unity Behavior adapter | Optional visual behavior-graph adapter package | Proposed | No | Adapter |
| EAI-CAP-018 | 2D grid/path adapter | Project or future provider for 2D/grid navigation | Proposed | No | Adapter |
| EAI-CAP-019 | Inference Engine adapter | Experimental decision/inference provider; no training ownership | Deferred | No | Adapter |
| EAI-CAP-020 | Group tactics and shared knowledge | Squad coordination, reservations, formations, and group blackboards | Deferred | No | Later |
| EAI-CAP-021 | Advanced planning | Behavior trees, GOAP, planners, and long-running plans | Deferred | No | Later |
| EAI-CAP-022 | Cover and tactical-point systems | World-authored cover, tactical points, and reservation providers | Deferred | No | Later |
| EAI-CAP-023 | Durable AI state snapshots | Project-selected, safe-point durable keys and state aliases | Proposed | No | Bridge/Project |
| EAI-CAP-024 | Authoritative multiplayer AI bridge | Server/host decision authority and client presentation snapshots | Proposed | No | Bridge |

### 7.2 MVP capability set

The smallest credible release contains:

- Actor-local `AIAgentHost`.
- Explicit scene/world registry and scheduler.
- Semantic observations and sensor/provider registration.
- Bounded perception memory with decay/expiry.
- Deterministic target scoring with explanations and hysteresis.
- Typed blackboard schemas and snapshots.
- Separate sensor and decision cadence with budgets.
- Lightweight state-machine and utility-selection paths.
- Typed behavior executors with cancellation and timeout.
- Provider-neutral navigation tickets and simulated provider.
- Debug traces, validators, and the standalone Laboratory.

### 7.3 Later capability set

- Unity AI Navigation adapter.
- Unity Behavior adapter.
- 2D grid and project pathfinding providers.
- Shared group memory and tactics.
- Behavior trees, GOAP, planners, cover, formations, and tactical reservations.
- Durable project-approved AI snapshots.
- Authoritative multiplayer replication bridge.
- Experimental inference providers.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Universal enemy brain | Rejected | Hides game design and grows into a god manager | Never as core |
| Mandatory NavMesh dependency | Rejected | Excludes 2D, grid, custom, and non-navigation agents | Adapter only |
| Mandatory visual behavior graph | Rejected | Couples data/runtime to one authoring backend | Adapter after core proof |
| Save all live memory and paths | Rejected | Scene objects, timing, and provider handles are unsafe durable state | Project-selected safe snapshot only |
| Neural inference in MVP | Deferred | Needs model, performance, determinism, and authority research | Dedicated experimental adapter |
| Full GOAP planner | Deferred | Large architecture requiring independent proof | Dedicated module/specification |
| Shared squad tactics | Deferred | Needs group authority and reservation semantics | After single-agent MVP |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definitions/configuration | Profiles, stimulus types, memory/scoring policies, blackboard schemas, state machines, navigation policy | Live targets, current state, path handles, timers, scene objects |
| Runtime state/behavior | Agent host, world registry, scheduler, memory, blackboard, decision context, behavior tickets, navigation tickets | Editor logic, production UI, save-file transport |
| Presentation/debug | Gizmos, inspectors, trace panels, sample readouts | Authoritative decisions or gameplay mutations |

### 8.2 Component topology

```text
Scripted/project sensors ----> AIObservation
Physics/provider sensors ----> AIObservation
                                  |
                                  v
                         AIAgentHost (actor-local)
                         |  PerceptionMemory
                         |  TypedBlackboard
                         |  TargetScoring
                         |  DecisionContext
                         |  BehaviorRunner
                         |  NavigationTicket
                         v
                 explicit action/navigation providers

AIWorldRegistry (scene/world boundary)
  -> agent/stimulus registration
  -> world query providers
  -> optional shared scheduler

AIScheduler
  -> sensor cadence
  -> decision cadence
  -> budgets/fairness/manual tick
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent application root required? | No |
| Actor authority | One `AIAgentHost` per AI actor |
| Shared services | Explicit scene/world registry and optional scheduler |
| Duplicate behavior | Duplicate actor host rejects before provider registration or scheduling |
| Initialization trigger | Explicit configuration/registration during component lifecycle or project setup |
| Shutdown | Cancel work, invalidate generations, unregister providers, clear runtime-only state |
| Direct-scene behavior | Scene-local world and scheduler may initialize without First Light |
| Test injection | Clock, random source, sensors, scorers, actions, world queries, scheduler, navigation |

Multiple worlds may exist in additive scenes. Cross-world observations are rejected unless an explicit project bridge translates them.

### 8.4 Lifecycle sequence

1. Validate immutable profile, stable IDs, and schemas.
2. Claim actor-local host authority.
3. Resolve or receive world registry and scheduler.
4. Create detached runtime memory, blackboard, traces, and generations.
5. Register sensors, scorers, conditions, actions, and navigation provider explicitly.
6. Enter configured initial behavior state.
7. Tick sensors and decisions under separate cadence/budget policies.
8. Publish semantic snapshots/events after authoritative changes.
9. Cancel work and invalidate handles during disable, transfer, reset, or shutdown.
10. Release scene/world registrations.

### 8.5 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
| --- | --- | --- | --- | --- |
| Duplicate host for actor | Host claim | Duplicate rejected before registrations or scheduling | Existing host remains authoritative | EAI-001 |
| Missing profile | Initialization | Agent reports Blocked and does not tick | Development status surface remains available | EAI-002 |
| Invalid stable ID | Validation/registration | Asset or provider rejected | No partial registration | EAI-003 |
| Sensor throws | Observation tick | Warning/error with provider identity | Provider disabled or observation skipped per policy | EAI-010 |
| Memory capacity exceeded | Observation commit | Advisory and eviction trace | Configured deterministic eviction | EAI-020 |
| Required scorer unavailable | Target evaluation | Candidate unavailable with reason | Current target retained or cleared by policy | EAI-030 |
| Blackboard type mismatch | Write | Structured failure | Existing value unchanged | EAI-040 |
| Decision budget exceeded | Scheduler | Overrun metric/advisory | Remaining work deferred | EAI-050 |
| No valid behavior | Decision commit | Explicit idle/unavailable state | Agent remains safe and observable | EAI-060 |
| Action timeout | Behavior execution | TimedOut result | Cancellation and failure route | EAI-061 |
| Navigation provider unavailable | Navigation request | Unavailable result | Behavior follows failure route | EAI-070 |
| Stale behavior completion | Async completion | Completion ignored | Current state remains authoritative | EAI-071 |
| Foreign world observation | Observation validation | Rejected with warning | No memory change | EAI-080 |
| Provider version mismatch | Registration | Provider rejected | Core remains usable | EAI-090 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned? |
|---|---|---:|---:|---:|
| `AIProfile` | Agent defaults and referenced policies | Yes | No | Yes |
| `StimulusTypeDefinition` | Semantic observation category and default memory policy | Yes | No | Yes |
| `AIMemoryPolicy` | Confidence, decay, expiry, capacity, and eviction | Yes | No | Yes |
| `AITargetScoringProfile` | Filter/scorer ordering, weights, threshold, hysteresis, tie policy | Yes | No | Yes |
| `AIBlackboardSchema` | Typed keys, scopes, defaults, ownership, and version | Yes | No | Yes |
| `AIStateMachineDefinition` | States, transitions, guards, and action references | Yes | No | Yes |
| `AINavigationPolicy` | Capabilities, tolerances, timeout, partial-path policy | Yes | No | Yes |
| `AIWorldConfiguration` | Scheduler budgets, world bounds, provider policy, debug defaults | Yes | No | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `AIAgentRuntime` | Agent host | Actor session | Rebuilt on reset/respawn | Not saved directly |
| `PerceptionMemory` | Agent runtime | Session | Cleared or restored from project-approved snapshot | Detached optional snapshot only |
| `AIBlackboard` | Agent runtime/world | Session | Schema defaults plus provider values | Only whitelisted durable keys |
| `AITargetSelectionState` | Agent runtime | Session | Cleared on target invalidation/reset | Not saved by default |
| `AIBehaviorSession` | Agent runtime | Active behavior | Invalidated on transition/reset | Not saved |
| `AINavigationState` | Provider/agent | Active ticket | Invalidated on replacement/warp/reset | Not saved |
| `AITraceBuffer` | Agent/runtime diagnostics | Bounded session history | Cleared by development reset | Optional diagnostic export only |

### 9.3 Stable identifiers

Durable domain IDs include `AIProfileId`, `StimulusTypeId`, `BlackboardKeyId`, `AIStateId`, `AIActionId`, `AIConditionId`, `ScorerId`, and provider IDs. Runtime-only identities include `AIAgentRuntimeId`, observation sequence, behavior generation, and navigation ticket generation.

A Unity asset GUID may help Editor repair but is not the Player-runtime identity. Display names and GameObject hierarchy paths are never durable IDs.

### 9.4 ScriptableObject safety

Definitions hold authored configuration only. They never store current target, memory confidence, active state, cooldown, behavior ticket, path, Transform, provider, or mutable blackboard values.

### 9.5 Serialization and migration

The core does not promise to serialize every live agent. A project may opt into a detached `AIDurableStateSnapshot` containing stable agent/definition identity, schema version, authored durable blackboard keys, selected high-level state aliases, and opaque extension records. Import occurs at declared safe points after actor/world resolution. Unknown newer records are preserved where safe and never applied blindly.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
| --- | --- | --- | --- |
| AIAgentHost | Component/service boundary | Actor-local AI authority and lifecycle | One per controlled AI actor |
| AIWorldRegistry | Runtime service | Scene/world boundary for agents, stimuli, and query providers | Explicit scene-local instance |
| AIScheduler | Runtime service | Cadence, priority, budgets, and manual ticking | World-owned or injected |
| AIProfile | ScriptableObject | Agent configuration and references | Project-owned immutable asset |
| StimulusTypeDefinition | ScriptableObject | Semantic stimulus identity and policy defaults | Project-owned immutable asset |
| AIObservation | Readonly struct | One validated sensor observation | Created by sensor/provider |
| AIMemoryRecord | Readonly struct | Authority-owned remembered stimulus state | Owned by agent runtime |
| AIMemorySnapshot | Readonly struct | Immutable published memory view | Returned by service |
| AITargetCandidate | Readonly struct | Candidate identity, memory reference, and context | Created during evaluation |
| AITargetScore | Readonly struct | Total score, availability, contributions, and reason | Created by scorer pipeline |
| AIBlackboardSchema | ScriptableObject | Typed key declarations and scope rules | Project-owned immutable asset |
| AIBlackboardSnapshot | Readonly struct | Immutable values and revision | Owned by agent runtime |
| AIStateMachineDefinition | ScriptableObject | States, transitions, guards, and action references | Project-owned immutable asset |
| AIBehaviorTicket | Readonly handle | Generational active behavior request | Issued by host |
| AINavigationTicket | Readonly handle | Generational provider navigation request | Issued by navigation provider |
| IAISensor | Interface | Produces observations for one agent/world | Provider or project implementation |
| IAITargetScorer | Interface | Produces one deterministic score contribution | Provider or project implementation |
| IAIConditionEvaluator | Interface | Side-effect-free condition evaluation | Provider or project implementation |
| IAIActionExecutor | Interface | Executes one typed behavior action | Provider or project implementation |
| IAINavigationProvider | Interface | Provider-neutral movement/path request surface | Adapter or project implementation |
| IAIClock | Interface | Scaled, unscaled, or deterministic time source | Injected |
| IAIRandomSource | Interface | Seeded deterministic random source | Injected |
| IAITraceSink | Interface | Bounded semantic tracing output | Optional provider |

### 10.2 Representative methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread rule |
|---|---|---|---|---|
| `RegisterAgent(AIAgentRegistration)` | Register actor-local host with a world | Valid IDs/profile/world | Handle or structured rejection | Main thread |
| `SubmitObservation(AIObservation)` | Validate and commit one sensor observation | Valid source/type/world/revision | Applied, deduplicated, stale, rejected, or failed | Main thread by default |
| `GetMemorySnapshot()` | Read immutable perception state | Agent Ready | Snapshot plus revision | Main thread; detached consumers may copy |
| `EvaluateTargets(AITargetQuery)` | Filter and score candidates | Scorers/providers available | Winner/no-target plus breakdown | Main thread/deterministic fixture |
| `TrySetBlackboard<T>(BlackboardKeyId,T)` | Write typed context | Schema permits caller/type | Success or structured rejection | Main thread |
| `RequestDecisionTick()` | Run one manual decision evaluation | Manual tick or test policy | Completed/deferred/blocked | Main thread |
| `RequestBehavior(AIBehaviorRequest)` | Begin/replace behavior | Behavior available | Generational ticket/result | Main thread |
| `RequestNavigation(AINavigationRequest)` | Ask provider to move/plan | Provider/capability available | Ticket or unavailable/invalid result | Main thread |
| `ResetAgent(AIResetPolicy)` | Clear runtime-only state | Development or approved runtime policy | Deterministic reset result | Main thread |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `AgentStateChanged` | Host | After authoritative lifecycle change | Old/new status and reason | Presentation only |
| `ObservationCommitted` | Memory | After record commit | Source/type/revision summary | No raw provider object required |
| `MemoryChanged` | Memory | After add/update/forget/expiry | Revision and change summary | Snapshot must be queried separately |
| `TargetChanged` | Scoring | After selection commit | Old/new target and explanation ID | Game rules remain external |
| `BlackboardChanged` | Blackboard | After value commit | Key, old/new summary, revision | Sensitive payloads may be redacted |
| `BehaviorTransitioned` | Runner | After exit/enter commit | Old/new state and reason | Listeners cannot veto after commit |
| `BehaviorCompleted` | Runner | After action/behavior result | Ticket and result | Stale tickets already filtered |
| `NavigationStatusChanged` | Provider bridge | After provider status update | Ticket, status, reason | Provider remains movement authority |
| `BudgetOverrun` | Scheduler | After bounded tick | Deferred counts and elapsed metric | Development diagnostic |

### 10.4 Async and cancellation policy

Behavior actions and navigation requests may be asynchronous. Every operation receives a generational ticket, cancellation signal, timeout policy, and explicit publication/commit boundary. Completion after replacement, state exit, agent reset, provider removal, or world shutdown is ignored as stale. Cancellation may be Too Late after a neighboring authority’s irreversible commit.

### 10.5 API ergonomics

The novice path uses one profile, state-machine definition, simulated sensors/navigation, and Laboratory prefab. The advanced path injects providers and custom behaviors. Convenience component access is not the only API; services and test seams remain explicit.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install EchoAI.
2. Open **Sperk's Forge > Instinct > Setup and Validation**.
3. Create an AI profile, stimulus catalog, blackboard schema, memory policy, scoring profile, and state-machine definition.
4. Preview created assets and paths.
5. Add an actor-local host and explicit world/scheduler references.
6. Open the standalone Instinct Laboratory.
7. Run validation before Play Mode.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeat-safe? | Undo/backup | Report |
|---|---|---|---:|---|---|
| Create minimal AI assets | Project-owned empty assets | Nothing existing | Yes | Unity Undo | Setup receipt |
| Add host/world components | Components on selected objects | Selected scene only | Yes | Unity Undo | Change list |
| Repair IDs | Missing/duplicate domain IDs | Selected assets after preview | Yes | Backup/Undo | ID report |
| Generate Laboratory fixtures | Sample-owned fixtures | Sample area only | Yes | Regenerate sample | Fixture report |
| Validate project | Nothing | Nothing | Yes | N/A | Validation results |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Instinct Setup and Validation | Installer | Create/repair/validate minimum assets and scene references | No |
| Agent Runtime Inspector | Programmer/tester | Memory, blackboard, target, state, tickets, budgets | Development only |
| Score Breakdown Panel | Designer/tester | Explain candidate contributions and switches | Development only |
| Behavior Trace Viewer | Programmer | Review transitions, actions, timeouts, and failures | Development only |
| Provider Inventory | Maintainer | List registered sensors/scorers/actions/navigation and versions | Development only |

### 11.4 Validation and repair

Checks include empty/duplicate IDs, invalid schema types, missing required keys, invalid initial state, missing states/actions/conditions, unreachable states, ambiguous transition priorities, missing scorer/provider contracts, invalid memory bounds, impossible scheduling budgets, unbounded traces, unavailable navigation requirements, and adapter-version incompatibility. Auto-fix is limited to safe ID generation, missing defaults, and explicitly previewed component creation.

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

- Git URL when repository exists.
- Local/embedded path during development.
- Tarball after release packaging.
- Workshop selection after the package and setup facade exist.
- Optional adapters installed separately.

### 12.2 Minimal scene setup

- One `AIWorldRegistry` and optional `AIScheduler` for the scene/world.
- One actor with `AIAgentHost`.
- One valid `AIProfile` and referenced configuration assets.
- At least one explicit observation source or scripted sensor.
- At least one behavior path.
- Simulated navigation or an installed provider only when movement is required.

### 12.3 Boot-scene setup

Not required. Project composition may register world services through First Light, but standalone scenes may initialize their own scene-local world without creating a second production bootstrap.

### 12.4 Direct-scene setup

Direct entry creates only the explicitly configured scene-local world/scheduler when absent. It never creates combat, characters, controllers, navigation vendors, or project data automatically.

### 12.5 Scene isolation rule

The standalone Laboratory uses simulated providers and no unrelated Echo runtime package. Navigation-vendor demonstrations belong in adapter Integration Laboratories.

---

## 13. Standalone Test Lab and Samples

### 13.1 Laboratory purpose

The **Instinct Perception and Decision Laboratory** proves an actor can receive scripted observations, remember and forget them, score targets, update typed context, transition behavior, complete simulated navigation, expose diagnostics, reset, and shut down without combat, controllers, characters, world, multiplayer, or vendor AI packages.

### 13.2 Required Laboratory contents

- Deterministic clock and seeded random source.
- Scripted stimulus emitters.
- Simulated sensor and navigation providers.
- Two or more target candidates.
- Editable memory/scoring/state-machine fixtures.
- Runtime memory, score, blackboard, state, and navigation readouts.
- Failure injection controls.
- Reset and trace export controls.
- No restricted or project-owned content.

### 13.3 Laboratory acceptance checklist

| ID | Group | Scenario | Action | Expected result | Evidence type | Status |
| --- | --- | --- | --- | --- | --- | --- |
| EAI-LAB-001 | Authority and lifecycle | Create one agent host | Add a host with a valid profile | One actor-local authority initializes and publishes Ready | Manual/Automated candidate | Not run |
| EAI-LAB-002 | Authority and lifecycle | Reject duplicate host | Add a second host for the same actor identity | Duplicate rejects before sensors, traces, or schedules start | Manual/Automated candidate | Not run |
| EAI-LAB-003 | Authority and lifecycle | Direct-scene entry | Open the Laboratory scene directly | Scene-local world and scheduler initialize without First Light | Manual/Automated candidate | Not run |
| EAI-LAB-004 | Authority and lifecycle | Multiple world contexts | Run two isolated world registries | Agents and stimuli never leak between worlds | Manual/Automated candidate | Not run |
| EAI-LAB-005 | Authority and lifecycle | Agent disable and enable | Disable then re-enable an agent host | Registrations and schedules release and rebuild cleanly | Manual/Automated candidate | Not run |
| EAI-LAB-006 | Authority and lifecycle | Agent destruction | Destroy a registered agent | World, scheduler, and diagnostics remove stale registrations | Manual/Automated candidate | Not run |
| EAI-LAB-007 | Authority and lifecycle | Scene unload | Unload the scene containing one world | Scene-local AI state is disposed without touching another scene | Manual/Automated candidate | Not run |
| EAI-LAB-008 | Authority and lifecycle | Domain reload reset | Enter Play Mode under supported reload settings | Static and registry state resets according to policy | Manual/Automated candidate | Not run |
| EAI-LAB-009 | Stimuli and observation | Submit visual observation | Script a visible target stimulus | Observation enters memory with source, type, position, confidence, and revision | Manual/Automated candidate | Not run |
| EAI-LAB-010 | Stimuli and observation | Submit audio observation | Script a sound stimulus without a visible target | Hearing-style observation is recorded independently | Manual/Automated candidate | Not run |
| EAI-LAB-011 | Stimuli and observation | Reject invalid source | Submit an empty or malformed source identity | Observation is rejected with an EAI diagnostic | Manual/Automated candidate | Not run |
| EAI-LAB-012 | Stimuli and observation | Deduplicate same revision | Submit the same source revision twice | Memory updates once and reports deduplication | Manual/Automated candidate | Not run |
| EAI-LAB-013 | Stimuli and observation | Accept newer revision | Submit a higher source revision | Memory replaces the older observation deterministically | Manual/Automated candidate | Not run |
| EAI-LAB-014 | Stimuli and observation | Out-of-order observation | Submit an older timestamp after a newer one | Policy rejects or records it without rewinding current truth | Manual/Automated candidate | Not run |
| EAI-LAB-015 | Stimuli and observation | Multiple stimulus types | Submit sight, sound, damage, and project tags | Type-specific memory remains distinct | Manual/Automated candidate | Not run |
| EAI-LAB-016 | Stimuli and observation | Sensor removal | Unregister an active sensor | No further observations arrive and stale callbacks are ignored | Manual/Automated candidate | Not run |
| EAI-LAB-017 | Perception memory | Confidence decay | Advance the injected clock | Confidence decays by the configured policy | Manual/Automated candidate | Not run |
| EAI-LAB-018 | Perception memory | Memory expiry | Advance beyond retention duration | Expired record leaves active memory and trace records why | Manual/Automated candidate | Not run |
| EAI-LAB-019 | Perception memory | Last known position | Remove current visibility | Last known position remains until expiry | Manual/Automated candidate | Not run |
| EAI-LAB-020 | Perception memory | Memory capacity | Exceed configured memory count | Deterministic eviction policy applies | Manual/Automated candidate | Not run |
| EAI-LAB-021 | Perception memory | Pinned memory | Pin one critical record then exceed capacity | Pinned record survives ordinary eviction | Manual/Automated candidate | Not run |
| EAI-LAB-022 | Perception memory | Forget source | Request explicit source removal | Matching records are removed without affecting others | Manual/Automated candidate | Not run |
| EAI-LAB-023 | Perception memory | World transfer rejection | Submit a record from another world | Foreign-world memory is rejected | Manual/Automated candidate | Not run |
| EAI-LAB-024 | Perception memory | Snapshot immutability | Attempt to mutate a published memory snapshot | Consumer cannot modify authority-owned state | Manual/Automated candidate | Not run |
| EAI-LAB-025 | Target scoring | Distance score | Evaluate two targets at different distances | Configured distance contribution ranks them deterministically | Manual/Automated candidate | Not run |
| EAI-LAB-026 | Target scoring | Confidence score | Compare fresh and stale observations | Confidence contribution affects total score | Manual/Automated candidate | Not run |
| EAI-LAB-027 | Target scoring | Threat provider | Register a project threat provider | Provider contribution appears with a reason record | Manual/Automated candidate | Not run |
| EAI-LAB-028 | Target scoring | Unavailable scorer | Remove a required scorer provider | Candidate becomes unavailable or contribution is skipped per policy | Manual/Automated candidate | Not run |
| EAI-LAB-029 | Target scoring | Tie breaker | Create equal candidate totals | Stable identity and configured tie policy choose predictably | Manual/Automated candidate | Not run |
| EAI-LAB-030 | Target scoring | Score threshold | Evaluate candidates below minimum score | No target is selected | Manual/Automated candidate | Not run |
| EAI-LAB-031 | Target scoring | Hysteresis | Oscillate two scores near each other | Current target remains until switch margin is exceeded | Manual/Automated candidate | Not run |
| EAI-LAB-032 | Target scoring | Score explanation | Inspect the winner | Per-contribution breakdown is available for diagnostics | Manual/Automated candidate | Not run |
| EAI-LAB-033 | Blackboard and context | Set typed value | Write a value matching the key schema | Value commits and revision increments | Manual/Automated candidate | Not run |
| EAI-LAB-034 | Blackboard and context | Reject wrong type | Write a mismatched type | Write fails without changing existing value | Manual/Automated candidate | Not run |
| EAI-LAB-035 | Blackboard and context | Required key validation | Initialize without a required key | Agent blocks or warns according to severity | Manual/Automated candidate | Not run |
| EAI-LAB-036 | Blackboard and context | Read-only key | Attempt to write a provider-owned key | Write is rejected | Manual/Automated candidate | Not run |
| EAI-LAB-037 | Blackboard and context | Local and shared scopes | Resolve keys from agent and world scopes | Scope precedence follows configuration | Manual/Automated candidate | Not run |
| EAI-LAB-038 | Blackboard and context | Value expiration | Advance a temporary key beyond its lifetime | Key expires deterministically | Manual/Automated candidate | Not run |
| EAI-LAB-039 | Blackboard and context | Snapshot revision | Read after multiple writes | Snapshot reports the current monotonic revision | Manual/Automated candidate | Not run |
| EAI-LAB-040 | Blackboard and context | Opaque extension value | Store a registered extension record | Unknown provider payload remains isolated and versioned | Manual/Automated candidate | Not run |
| EAI-LAB-041 | Scheduling and budgets | Decision cadence | Run agent ticks below the decision interval | Decision evaluation occurs only at configured cadence | Manual/Automated candidate | Not run |
| EAI-LAB-042 | Scheduling and budgets | Sensor cadence | Use a different sensor interval | Sensor and decision schedules remain independent | Manual/Automated candidate | Not run |
| EAI-LAB-043 | Scheduling and budgets | Round-robin budget | Run many agents under a fixed per-frame budget | Work distributes without starvation | Manual/Automated candidate | Not run |
| EAI-LAB-044 | Scheduling and budgets | Priority scheduling | Raise one agent priority | Higher-priority work runs first within bounded fairness | Manual/Automated candidate | Not run |
| EAI-LAB-045 | Scheduling and budgets | Pause policy | Pause scaled game time | Scaled AI stops while diagnostic/test clocks behave as configured | Manual/Automated candidate | Not run |
| EAI-LAB-046 | Scheduling and budgets | Manual tick mode | Disable automatic scheduling | Tests advance the agent only through explicit ticks | Manual/Automated candidate | Not run |
| EAI-LAB-047 | Scheduling and budgets | Overrun report | Force work beyond the budget | Overrun is reported and remaining work defers safely | Manual/Automated candidate | Not run |
| EAI-LAB-048 | Scheduling and budgets | Unregister during tick | Remove an agent while scheduled | Generation checks prevent stale continuation | Manual/Automated candidate | Not run |
| EAI-LAB-049 | Behavior and state machine | Enter initial state | Start a valid state machine | Configured initial state enters once | Manual/Automated candidate | Not run |
| EAI-LAB-050 | Behavior and state machine | Guarded transition | Satisfy a transition condition | Transition exits old state and enters new state atomically | Manual/Automated candidate | Not run |
| EAI-LAB-051 | Behavior and state machine | Failed guard | Leave transition condition false | Current state remains active | Manual/Automated candidate | Not run |
| EAI-LAB-052 | Behavior and state machine | Action failure route | Return Failure from an action executor | Configured failure transition or safe idle runs | Manual/Automated candidate | Not run |
| EAI-LAB-053 | Behavior and state machine | Action timeout | Hold an asynchronous action past timeout | Action cancels and follows timeout policy | Manual/Automated candidate | Not run |
| EAI-LAB-054 | Behavior and state machine | Stale completion | Complete an action after its state exited | Completion is ignored by generation | Manual/Automated candidate | Not run |
| EAI-LAB-055 | Behavior and state machine | Utility choice | Score multiple behavior candidates | Highest valid candidate wins with a score explanation | Manual/Automated candidate | Not run |
| EAI-LAB-056 | Behavior and state machine | No valid behavior | Make every candidate unavailable | Agent enters explicit idle/unavailable behavior | Manual/Automated candidate | Not run |
| EAI-LAB-057 | Navigation abstraction | Set destination | Request a valid destination from the simulated provider | Navigation ticket enters calculating then moving | Manual/Automated candidate | Not run |
| EAI-LAB-058 | Navigation abstraction | Reject invalid destination | Request outside provider bounds | Structured invalid result is returned | Manual/Automated candidate | Not run |
| EAI-LAB-059 | Navigation abstraction | Stop navigation | Cancel an active move before commit | Provider stops and ticket completes Cancelled | Manual/Automated candidate | Not run |
| EAI-LAB-060 | Navigation abstraction | Stale navigation result | Complete an old request after replacement | Old completion cannot overwrite current status | Manual/Automated candidate | Not run |
| EAI-LAB-061 | Navigation abstraction | Arrive at destination | Advance simulated movement | Status becomes Arrived within tolerance | Manual/Automated candidate | Not run |
| EAI-LAB-062 | Navigation abstraction | Partial path | Provider reports partial reachability | Policy chooses accept, retry, or fail explicitly | Manual/Automated candidate | Not run |
| EAI-LAB-063 | Navigation abstraction | Provider unavailable | Remove navigation provider | Behavior receives Unavailable rather than a null failure | Manual/Automated candidate | Not run |
| EAI-LAB-064 | Navigation abstraction | Warp revision | Warp the actor while moving | Provider and host invalidate stale path state | Manual/Automated candidate | Not run |
| EAI-LAB-065 | Failure and recovery | Sensor exception | Throw from one sensor | Failure is isolated and agent continues per policy | Manual/Automated candidate | Not run |
| EAI-LAB-066 | Failure and recovery | Scorer exception | Throw from one scorer | Candidate evaluation records failure without corrupting memory | Manual/Automated candidate | Not run |
| EAI-LAB-067 | Failure and recovery | Behavior exception | Throw from an action executor | Agent enters configured safe failure state | Manual/Automated candidate | Not run |
| EAI-LAB-068 | Failure and recovery | Navigation exception | Throw from provider | Navigation ticket fails and behavior receives a structured result | Manual/Automated candidate | Not run |
| EAI-LAB-069 | Failure and recovery | Missing definition | Remove a referenced state or key | Validator blocks before Play Mode or runtime fails safely | Manual/Automated candidate | Not run |
| EAI-LAB-070 | Failure and recovery | Provider version mismatch | Register incompatible provider metadata | Registration is rejected with an actionable code | Manual/Automated candidate | Not run |
| EAI-LAB-071 | Failure and recovery | Trace overflow | Exceed bounded trace capacity | Old records are pruned deterministically | Manual/Automated candidate | Not run |
| EAI-LAB-072 | Failure and recovery | Reset agent | Invoke development reset | Memory, blackboard, behavior, and navigation return to configured baseline | Manual/Automated candidate | Not run |
| EAI-LAB-073 | Debugging, lifecycle, and scale | Memory inspector | Select one agent in the Laboratory | Current observations and expiry are visible | Manual/Automated candidate | Not run |
| EAI-LAB-074 | Debugging, lifecycle, and scale | Score inspector | Evaluate a target set | Contribution breakdown and winner are visible | Manual/Automated candidate | Not run |
| EAI-LAB-075 | Debugging, lifecycle, and scale | Behavior trace | Run several transitions | Bounded transition history is visible | Manual/Automated candidate | Not run |
| EAI-LAB-076 | Debugging, lifecycle, and scale | Navigation trace | Run one move request | Ticket history and provider status are visible | Manual/Automated candidate | Not run |
| EAI-LAB-077 | Debugging, lifecycle, and scale | Gizmo toggle | Disable debug drawing | No debug geometry remains and runtime logic is unchanged | Manual/Automated candidate | Not run |
| EAI-LAB-078 | Debugging, lifecycle, and scale | Support snapshot redaction | Export diagnostics | Project-private labels and exact positions follow redaction policy | Manual/Automated candidate | Not run |
| EAI-LAB-079 | Debugging, lifecycle, and scale | Many-agent stress | Run the configured stress fixture | Budget, queue, and memory metrics are captured as Not run evidence until executed | Manual/Automated candidate | Not run |
| EAI-LAB-080 | Debugging, lifecycle, and scale | Clean removal | Remove samples and optional adapters | Neutral core remains compile-safe or is fully removable | Manual/Automated candidate | Not run |

### 13.4 Optional integration samples

| Sample | Packages/providers | Purpose | Why not standalone proof |
|---|---|---|---|
| Unity NavMesh Pursuit | EchoAI + AI Navigation adapter | Demonstrate 3D destination/path status | Depends on optional provider |
| Fellowship-controlled NPC | EchoAI + Fellowship | Actor identity/spawn/control lifecycle | Tests bridge, not core |
| Vessel locomotion | EchoAI + Vessel | Translate navigation/behavior intent into motor requests | Movement authority is external |
| Server-authoritative AI | EchoAI + Convergence provider | Host/server decisions and client snapshots | Requires networking provider evidence |
| Clash threat response | EchoAI + Clash | Damage/threat context and combat requests | Combat authority is external |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

EchoAI is nonvisual. Runtime presentation is optional debug tooling and sample readouts. Production UI belongs to The Looking Glass or project code. Animation, VFX, camera, and audio consume semantic events through project adapters.

### 14.2 Required diagnostic states

- Uninitialized
- Ready
- Paused/suspended
- Provider unavailable
- No target
- Acting
- Navigating
- Blocked
- Warning
- Failure
- Shutting down

### 14.3 Accessibility requirements

Debug views must support readable text scaling, non-color-only status, reduced or disabled animated traces, keyboard navigation when interactive, and the ability to hide sensory/debug geometry. Gameplay accessibility remains project-owned, though AI policies may consume project-provided difficulty/accessibility context through typed providers.

### 14.4 Visual customization

Gizmos, colors, icons, panels, and sample visuals are replaceable and excluded from runtime authority.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost policy |
|---|---|---|---|
| Agent lifecycle/status | API/Inspector | Editor/Development | Constant bounded snapshot |
| Provider inventory | API/Window | Editor/Development | Registration-time |
| Memory records | API/Inspector | Development | Bounded by policy |
| Score breakdown | API/Inspector | Development | On evaluation/selected sampling |
| Blackboard snapshot | API/Inspector | Development | Bounded typed values |
| Behavior trace | API/Window | Development | Ring buffer |
| Scheduler metrics | API/Overlay | Development | Configurable sample interval |
| Navigation trace | API/Inspector | Development | Bounded ticket history |
| Support snapshot | Export | Explicit development action | Redacted and bounded |

### 15.2 Structured status

The status model exposes agent/world identity, configuration IDs, current lifecycle, memory count/revision, selected target, blackboard revision, state/action ticket, navigation ticket/status, provider health, sensor/decision cadence, deferred work, warnings, and package version.

### 15.3 Diagnostic codes

Stable `EAI-*` codes cover initialization, IDs, providers, observation validation, memory, scoring, blackboard, scheduling, behavior, navigation, migration, security, and removal. Codes never include raw production dialogue, secret provider data, or unrestricted scene content.

### 15.4 Observatory bridge

A separate bridge publishes provider-neutral agent health, aggregate counts, budget overruns, current high-level state, and redacted trace summaries. EchoAI never requires The Observatory.

### 15.5 Logging policy

No per-frame spam. Repeated warnings are deduplicated/rate-limited. Score and observation details belong in structured traces rather than Console floods. Exact positions and project-private labels follow export redaction policy.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Profiles/definitions | Project design | Project | As Unity assets | Unity asset serialization |
| Live observations/memory | Session | Agent host | No by default | N/A |
| Blackboard runtime values | Session/project-selected | Agent/world | Whitelist only | Detached optional snapshot |
| Current action/navigation ticket | Session | Agent/provider | No | N/A |
| High-level durable AI state | Slot/world when explicitly designed | Project bridge | Optional | Chronicle participant/project backend |
| Debug traces | Development session | EchoAI diagnostics | Optional export only | Redacted report |

### 16.2 Standalone behavior

Without Chronicle, EchoAI starts from authored configuration and session context. It does not select a hidden filename or persist live AI state.

### 16.3 Optional participant/provider contract

A project may export versioned, detached durable records at safe points. Records use stable agent/definition/state/key IDs, schema versions, aliases, and opaque extension records. Import validates the actor/world and never restores provider handles, Transforms, paths, timers, or asynchronous tickets.

### 16.4 Failure and recovery

Missing snapshots start from configuration. Older supported versions migrate through contiguous steps. Newer or unknown records are preserved where safe and reported. Corrupt records fail without partially mutating live state. Applied snapshots commit atomically per agent/project-defined batch.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

AI observes and requests; neighboring authorities decide final gameplay mutations. Optional connections remain explicit, removable, versioned, and capability-aware.

### 17.2 Planned integrations

| Other authority/provider | Connection type | Owner | Data/events exchanged | Required? |
| --- | --- | --- | --- | --- |
| The Fellowship / EchoCharacters | Separate bridge | EchoAI bridge package | Character/actor identity, spawn/despawn, control-owner changes, availability snapshots | No |
| The Vessel / EchoControllers | Separate bridge or project adapter | Bridge/project | Navigation/locomotion intent requests and motor status; no direct Rigidbody mutation by core | No |
| Clash / EchoCombat | Future separate bridge | Bridge | Damage, threat, teams, targetability, defeat events, combat requests | No |
| Arcana / EchoAbilities | Future separate bridge | Bridge | Ability availability, targeting requests, activation results, cast interruption | No |
| The Atlas / EchoWorld | Future separate bridge | Bridge | Zone/location context, patrol/tactical points, durable world state | No |
| The Hand / EchoInteraction | Separate bridge | Bridge/project | Interaction offers, execution requests, and results | No |
| The Eye / EchoCamera | Project adapter | Project | Debug/spectator focus only; AI does not own camera | No |
| Impact / EchoFeedback | Project adapter | Project | Semantic feedback request after AI/gameplay events | No |
| Resonance / Jukebot | Project adapter | Project | Semantic audio requests and sound-stimulus generation | No |
| The Path / EchoObjectives | Project adapter | Project | Objective events may affect AI context; AI does not mutate objective truth | No |
| The Chronicle / EchoSave | Separate bridge/project participant | Bridge/project | Detached project-approved durable AI snapshot at safe points | No |
| The Convergence / EchoMultiplayer | Separate bridge | Bridge | Authoritative host/server decisions, replication snapshots, client presentation | No |
| Unity AI Navigation | Provider adapter package | EchoAI adapter | NavMesh destination, path status, stop, warp, and capabilities | No |
| Unity Behavior | Provider adapter package | EchoAI adapter | Visual behavior graph execution mapped to AI context/providers | No |
| Unity Inference Engine | Experimental provider adapter | EchoAI adapter | Model inference only; training and game authority remain external | No |

### 17.3 Bridge placement decision

- Vendor navigation/behavior/inference integrations are separate provider adapter packages.
- Echo-to-Echo integrations are separate bridge packages when both peers are optional.
- Game-specific personality, threat, tactics, and movement translation remain project adapters.
- Tiny compile-safe debug integrations may live in a presentation/debug assembly only when they introduce no optional hard dependency.

### 17.4 Integration failure behavior

Missing peers yield `Unavailable` or omit optional contributions. Provider removal invalidates handles, cancels active work, and leaves the agent in a configured safe state. Version mismatch blocks registration before behavior begins. Teardown removes bridges before core services.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

All targets remain Proposed/Not run until implementation. The design must measure decision time, sensor time, observations processed, memory count, candidates scored, deferred work, action count, navigation requests, and allocations under declared fixtures.

| Metric | Initial policy | Measurement | Release threshold |
|---|---|---|---|
| Per-agent memory entries | Project-configured hard bound | Laboratory stress fixture | No unbounded growth |
| Candidate evaluations | Bounded per decision | Score stress fixture | Defer/reject beyond bound |
| Sensor/decision cadence | Independent configurable intervals | Scheduler trace | No hidden every-frame requirement |
| Per-frame AI budget | World-configured | Aggregate stress fixture | Excess work defers predictably |
| Trace history | Ring buffer | Diagnostic stress | Fixed maximum |
| Allocations | No avoidable steady-state allocations in hot paths | Profiler evidence | Threshold set after prototype |

### 18.2 Allocation policy

Avoid LINQ, reflection, per-tick strings, scene-wide object searches, and unbounded closures in hot paths. Reuse working buffers where ownership is clear. Public snapshots are immutable and may be pooled/copied only with explicit lifetime rules.

### 18.3 Scene and domain reload behavior

Registrations, static caches, generations, schedules, and debug state reset according to supported Enter Play Mode options. Scene unload cancels world-owned work. Event/provider registrations use disposable handles and generation checks.

### 18.4 Scalability limits

Advertised agent counts are not claimed before measurement. Projects configure hard limits for agents per world, sensors per agent, observations per tick, memory entries, candidate counts, actions, navigation tickets, traces, and deferred work. Exceeding limits degrades through rejection, eviction, or deferral, never silent allocation growth.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

EchoAI normally handles game-world positions, identifiers, tags, scores, and behavior traces. It must not collect credentials, account tokens, private chat, typed text, or device identifiers. Support exports may redact exact positions, project-private names, and custom provider payloads.

### 19.2 Trust boundaries

- Sensor/provider data is validated before commit.
- Serialized snapshots are untrusted until schema/ID validation.
- Reflection-discovered actions are forbidden.
- Multiplayer clients do not authoritatively commit shared AI decisions by default.
- Inference output, if later supported, is advisory until validated by game authority.
- Provider exceptions are isolated and cannot corrupt other agents.

### 19.3 Platform behavior

| Platform | Status | Special behavior | Evidence required |
|---|---|---|---|
| Windows | Planned | Primary development baseline | Installation, Lab, performance, adapters |
| macOS | Planned | Editor/runtime verification | Clean-project and Lab |
| Linux | Planned | Runtime/headless relevance | Player/headless evidence |
| WebGL | Unknown | Threading/provider limitations possible | Core and adapter-specific evidence |
| Mobile | Planned/Unknown | Tight budgets and provider support | Device performance and suspend/resume |
| Console | Unknown | Platform approval/provider limits | Platform-holder evidence |

---

## 20. Package and Repository Structure

### 20.1 Proposed package anatomy

```text
Packages/com.echodevgames.echo-ai/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
├── Runtime/
│   ├── Core/
│   ├── Agents/
│   ├── Stimuli/
│   ├── Memory/
│   ├── Scoring/
│   ├── Blackboard/
│   ├── Scheduling/
│   ├── Behavior/
│   ├── Navigation/
│   ├── Diagnostics/
│   └── EchoDevGames.EchoAI.Runtime.asmdef
├── RuntimeAdapters/
│   ├── Physics2D/
│   └── Physics3D/
├── Editor/
├── Samples~/
│   └── Instinct Perception and Decision Laboratory/
└── Tests/
    ├── Editor/
    └── Runtime/
```

Optional provider repositories/packages:

```text
com.echodevgames.echo-ai.navigation.unity-navmesh
com.echodevgames.echo-ai.behavior.unity-behavior
com.echodevgames.echo-ai.inference.unity-inference
```

### 20.2 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoAI.Runtime` | Runtime | UnityEngine only | Yes | Neutral contracts and agent runtime |
| `EchoDevGames.EchoAI.Physics2D` | Runtime | Runtime, Physics2D | No | Optional 2D sensor/query adapters |
| `EchoDevGames.EchoAI.Physics3D` | Runtime | Runtime, Physics | No | Optional 3D sensor/query adapters |
| `EchoDevGames.EchoAI.Editor` | Editor | Runtime, UnityEditor | No | Setup, validation, inspectors, gizmos |
| `EchoDevGames.EchoAI.Tests.Editor` | Editor tests | Runtime, Editor, Test Framework | No | Pure/editor tests |
| `EchoDevGames.EchoAI.Tests.Runtime` | Runtime tests | Runtime, Test Framework | No | PlayMode/runtime tests |

### 20.3 Repository files

README, documentation index, Current Notes link, architecture/lifecycle docs, provider-adapter guide, diagnostics reference, Laboratory guide, migration guide, changelog, license, third-party notices, contribution guidance, release checklist, and stable `.meta` files.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum/planned | Tested | Notes |
|---|---|---|---|
| Unity | Planned floor 6000.0 | 6000.3.8f1 planned | Not run |
| AI Navigation adapter | Research observed 2.0.14 | Not run | Optional provider only |
| Unity Behavior adapter | Research observed 1.0.16 | Not run | Optional provider only |
| Inference Engine adapter | Research observed 2.6.1 | Not run | Experimental/deferred |

### 21.2 Semantic versioning policy

Breaking public contracts, stable IDs, serialized schemas, provider compliance, behavior-state semantics, or package structure require major versions. Backward-compatible capabilities and adapters are minor. Fixes without contract changes are patch releases. Provider adapters version independently.

### 21.3 Deprecation policy

Deprecated contracts remain documented for at least one supported minor line when practical, include migration guidance, and emit development diagnostics. Removal requires a major version or explicit pre-1.0 policy.

### 21.4 GUID and asset compatibility

Public scripts, definitions, schemas, samples, and templates preserve `.meta` GUIDs. Domain stable IDs remain separate and require alias/migration records if changed.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview and authority boundaries.
- Installation and five-minute quick start.
- Agent/profile/stimulus/memory/scoring/blackboard/state-machine setup.
- Laboratory guide.
- Sensor, scorer, action, navigation, and world-provider authoring.
- Diagnostics and error-code reference.
- Optional adapter index.
- Migration, removal, and known limitations.

### 22.2 Required developer documentation

- Agent/world/scheduler architecture.
- Observation and memory lifecycle.
- Target-score determinism.
- Typed blackboard rules.
- Behavior cancellation and stale completion.
- Navigation provider compliance.
- Performance budgets and deterministic testing.
- Multiplayer authority guidance.
- Extension/provider examples.

### 22.3 Documentation truth rule

All compatibility, performance, platform, provider, migration, and release claims remain `Not run` until executed. Example code must compile against the documented version when implementation begins.

### 22.4 Living repository workflow

Current Notes captures provisional discoveries. Durable changes move into this foundation, provider records, ADRs, integration specs, tests, guides, or release records. Obsidian opens the repository Markdown directly.

### 22.5 Handoff scan order

README -> SFGSS-000 -> SFGSS-002/003/004/005 -> this foundation -> SUITE-DOC-19 feasibility/provider record -> relevant peer specs -> Current Notes -> roadmap -> implementation/tests when they exist.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required before release? |
|---|---|---|---:|
| EditMode unit | IDs, memory, scores, schemas, transitions, budgets | Deterministic pure policies | Yes |
| PlayMode runtime | Host/world lifecycle, async actions, navigation tickets | Actor-local runtime | Yes |
| Standalone Laboratory | Complete isolated perception-decision loop | 80 scenarios below | Yes |
| Provider Integration Lab | AI Navigation/Behavior/Inference adapters | Adapter-specific proof | When adapter ships |
| Clean-project install | Dependency and sample independence | Core with no optional provider | Yes |
| Existing-project adoption | Replace one project AI path incrementally | Hackulos/Don't Get Vince'd target | Before adoption claim |
| Multiplayer authority | Server/host decisions and client snapshots | Convergence bridge | When bridge ships |

### 23.2 Required categories

Happy path, missing/invalid configuration, duplicates, observation ordering, memory expiry/capacity, scoring ties/hysteresis, typed context, budgets, state transitions, action timeout/cancellation, navigation absence/failure, direct-scene entry, scene unload, adapter absence/removal, deterministic fixtures, malformed snapshots, platform behavior, performance, privacy, and release packaging.

### 23.3 Test case registry

| Test ID | Category | Requirement | Setup | Action | Expected result | Automation | Status |
| --- | --- | --- | --- | --- | --- | --- | --- |
| EAI-T-001 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-002 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-003 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-004 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-005 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-006 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-007 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-008 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-009 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-010 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-011 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-012 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-013 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-014 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-015 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-016 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-017 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-018 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-019 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-020 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-021 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-022 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-023 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-024 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-025 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-026 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-027 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-028 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-029 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-030 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-031 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-032 | Installation and independence | Install, compile, remove, reinstall, sample, and adapter independence case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-033 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-034 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-035 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-036 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-037 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-038 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-039 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-040 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-041 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-042 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-043 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-044 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-045 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-046 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-047 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-048 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-049 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-050 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-051 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-052 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-053 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-054 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-055 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-056 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-057 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-058 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-059 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-060 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-061 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-062 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-063 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-064 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-065 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 33 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-066 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 34 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-067 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 35 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-068 | Authority and lifecycle | Agent, world, scheduler, registration, reset, and teardown lifecycle case 36 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-069 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-070 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-071 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-072 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-073 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-074 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-075 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-076 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-077 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-078 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-079 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-080 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-081 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-082 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-083 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-084 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-085 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-086 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-087 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-088 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-089 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-090 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-091 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-092 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-093 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-094 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-095 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-096 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-097 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-098 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-099 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-100 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-101 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 33 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-102 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 34 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-103 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 35 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-104 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 36 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-105 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 37 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-106 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 38 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-107 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 39 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-108 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 40 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-109 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 41 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-110 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 42 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-111 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 43 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-112 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 44 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-113 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 45 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-114 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 46 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-115 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 47 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-116 | Stimulus and observation | Stimulus identity, observation validation, deduplication, revision, sensor registration, and ordering case 48 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-117 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-118 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-119 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-120 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-121 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-122 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-123 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-124 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-125 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-126 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-127 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-128 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-129 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-130 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-131 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-132 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-133 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-134 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-135 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-136 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-137 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-138 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-139 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-140 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-141 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-142 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-143 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-144 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-145 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-146 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-147 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-148 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-149 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 33 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-150 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 34 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-151 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 35 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-152 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 36 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-153 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 37 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-154 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 38 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-155 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 39 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-156 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 40 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-157 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 41 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-158 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 42 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-159 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 43 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-160 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 44 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-161 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 45 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-162 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 46 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-163 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 47 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-164 | Perception memory | Memory creation, confidence, decay, expiry, capacity, eviction, pinning, and snapshots case 48 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-165 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-166 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-167 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-168 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-169 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-170 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-171 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-172 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-173 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-174 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-175 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-176 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-177 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-178 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-179 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-180 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-181 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-182 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-183 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-184 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-185 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-186 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-187 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-188 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-189 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-190 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-191 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-192 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-193 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-194 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-195 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-196 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-197 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 33 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-198 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 34 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-199 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 35 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-200 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 36 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-201 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 37 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-202 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 38 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-203 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 39 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-204 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 40 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-205 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 41 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-206 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 42 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-207 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 43 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-208 | Target scoring | Candidate filtering, contributions, thresholds, hysteresis, ties, and explanation case 44 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-209 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-210 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-211 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-212 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-213 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-214 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-215 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-216 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-217 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-218 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-219 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-220 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-221 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-222 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-223 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-224 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-225 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-226 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-227 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-228 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-229 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-230 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-231 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-232 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-233 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-234 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-235 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-236 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-237 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-238 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-239 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-240 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-241 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 33 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-242 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 34 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-243 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 35 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-244 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 36 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-245 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 37 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-246 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 38 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-247 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 39 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-248 | Blackboard and context | Schemas, typed values, scopes, revisions, expiration, providers, and extension records case 40 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-249 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-250 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-251 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-252 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-253 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-254 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-255 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-256 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-257 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-258 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-259 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-260 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-261 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-262 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-263 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-264 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-265 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-266 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-267 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-268 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-269 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-270 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-271 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-272 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-273 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-274 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-275 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-276 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-277 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-278 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-279 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-280 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-281 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 33 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-282 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 34 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-283 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 35 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-284 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 36 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-285 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 37 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-286 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 38 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-287 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 39 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-288 | Scheduling and budgets | Cadence, priorities, fairness, time policies, manual tick, overruns, and stale work case 40 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-289 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-290 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-291 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-292 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-293 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-294 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-295 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-296 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-297 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-298 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-299 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-300 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-301 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-302 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-303 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-304 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-305 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-306 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-307 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-308 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-309 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-310 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-311 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-312 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-313 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-314 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-315 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-316 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-317 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-318 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-319 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-320 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-321 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 33 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-322 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 34 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-323 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 35 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-324 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 36 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-325 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 37 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-326 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 38 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-327 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 39 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-328 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 40 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-329 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 41 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-330 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 42 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-331 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 43 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-332 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 44 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-333 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 45 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-334 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 46 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-335 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 47 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-336 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 48 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-337 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 49 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-338 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 50 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-339 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 51 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-340 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 52 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-341 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 53 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-342 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 54 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-343 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 55 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-344 | Behavior and state machine | States, transitions, guards, utility selection, actions, cancellation, timeout, and stale completion case 56 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-345 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-346 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-347 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-348 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-349 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-350 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-351 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-352 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-353 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-354 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-355 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-356 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-357 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-358 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-359 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-360 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-361 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-362 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-363 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-364 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-365 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-366 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-367 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-368 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-369 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-370 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-371 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-372 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-373 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-374 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-375 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-376 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-377 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 33 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-378 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 34 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-379 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 35 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-380 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 36 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-381 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 37 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-382 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 38 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-383 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 39 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-384 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 40 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-385 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 41 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-386 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 42 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-387 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 43 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-388 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 44 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-389 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 45 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-390 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 46 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-391 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 47 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-392 | Navigation abstraction | Provider registration, destinations, tickets, statuses, replacement, cancellation, paths, and warp case 48 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-393 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-394 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-395 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-396 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-397 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-398 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-399 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-400 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-401 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-402 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-403 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-404 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-405 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-406 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-407 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-408 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-409 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-410 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-411 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-412 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-413 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-414 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-415 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-416 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-417 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-418 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-419 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-420 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-421 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-422 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-423 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-424 | Diagnostics and debugging | Codes, traces, inspectors, gizmos, snapshots, redaction, and bounded histories case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-425 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-426 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-427 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-428 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-429 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-430 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-431 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-432 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-433 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-434 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-435 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-436 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-437 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-438 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-439 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-440 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-441 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-442 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-443 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-444 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-445 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-446 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-447 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-448 | Persistence and migration | Detached durable state, aliases, migrations, unknown records, and safe-point import/export case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-449 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-450 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-451 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-452 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-453 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-454 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-455 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-456 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-457 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-458 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-459 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-460 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-461 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-462 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-463 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-464 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-465 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-466 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-467 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-468 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-469 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-470 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-471 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-472 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-473 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-474 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-475 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-476 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-477 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-478 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-479 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-480 | Integration and removal | Characters, Controllers, Combat, Abilities, World, Multiplayer, adapter removal, and bridge teardown case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-481 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 01 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-482 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 02 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-483 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 03 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-484 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 04 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-485 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 05 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-486 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 06 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-487 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 07 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-488 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 08 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-489 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 09 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-490 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 10 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-491 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 11 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-492 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 12 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-493 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 13 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-494 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 14 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-495 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 15 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-496 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 16 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-497 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 17 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-498 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 18 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-499 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 19 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-500 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 20 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-501 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 21 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-502 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 22 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-503 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 23 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-504 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 24 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-505 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 25 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-506 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 26 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-507 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 27 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-508 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 28 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-509 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 29 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-510 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 30 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-511 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 31 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |
| EAI-T-512 | Performance, platform, and security | Budgets, allocation, scale, platform capability, authoritative AI, privacy, and malformed input case 32 | Controlled fixture with declared providers and deterministic clock/random source | Execute the registered condition and capture result/evidence | Expected contract outcome occurs without unrelated authority changes | Planned | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Feasibility/specification gate

- [x] Ownership and non-ownership approved.
- [x] Rootless/actor-local authority approved.
- [x] MVP and deferred scope separated.
- [x] Observation, memory, scoring, context, behavior, and navigation contracts defined.
- [x] Optional provider strategy documented.
- [x] Laboratory and planned evidence registered.
- [x] All empirical evidence remains Not run.

### 24.2 Implementation gate

- [ ] SUITE-DOC-33 explicitly authorizes implementation.
- [ ] Runtime and Editor assemblies compile with declared dependencies only.
- [ ] Core has no optional provider dependency.
- [ ] Setup/validation are repeat-safe.
- [ ] Runtime collections and histories are bounded.
- [ ] Public API matches this foundation or an approved revision/ADR.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Standalone Laboratory passes all required scenarios.
- [ ] Simulated providers prove core behavior.
- [ ] Samples can be removed.
- [ ] Direct-scene entry and teardown match documentation.

### 24.4 Provider/quality gate

- [ ] Provider adapter has its own Integration Laboratory.
- [ ] Capability mismatches are explicit.
- [ ] Determinism limits are documented.
- [ ] Performance budgets pass declared fixtures.
- [ ] Diagnostics are actionable and redacted.
- [ ] No Blocker/Critical defects remain.

### 24.5 Distribution gate

- [ ] Manifest, version, changelog, license, notices, and `.meta` files are complete.
- [ ] Git/tarball install tested.
- [ ] Removal/reinstall tested.
- [ ] Documentation examples compile.
- [ ] Compatibility catalog updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing AI | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Hackulos | Planned rat and aggressive humanoid behaviors | Introduce neutral observation/memory/scoring and one actor at a time | Existing aggro, flee, attack-request, and debug behavior preserved | Keep project AI scripts until parity |
| Don't Get Vince'd | Existing enemy/boss logic | Extract reusable target/memory/decision seams only where useful | No combat or animation regression | Retain original components |
| Echo Systems Lab | Portfolio test actors | Build an isolated AI case study | Lab evidence and clear architecture page | Remove sample/adapter |
| Rescuers2D | Limited NPC/environment behaviors | Use only where a real AI use case exists | No forced package adoption | Do not migrate unnecessarily |

### 25.2 Preserve-until-parity rule

Existing AI remains active until the neutral package passes its standalone Laboratory and one narrow project integration. Migrate observation, memory, scoring, behavior, and navigation separately. Never remove working project behavior merely because a package contract exists.

### 25.3 Migration tooling

Future tools may detect duplicate IDs, create profiles from selected component settings, map state names to stable IDs, preview unsupported custom behavior, and generate a migration report. They must not rewrite project AI automatically without explicit approval and backup.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EAI-R-001 | Universal-brain scope inflation | High | High | Actor-local contracts, small MVP, explicit deferred modules | Every capability review |
| EAI-R-002 | Navigation backend leakage | Medium | High | Provider-neutral tickets and separate adapters | Provider spec/review |
| EAI-R-003 | Per-frame work explosion | High | High | Cadence, budgets, bounds, fairness, metrics | Stress evidence |
| EAI-R-004 | Nondeterministic target/behavior choices | Medium | High | Stable ordering, seeded random, explanations | Unit/Lab tests |
| EAI-R-005 | Stale async action/path completion | Medium | High | Generational tickets and cancellation | Runtime tests |
| EAI-R-006 | Blackboard becomes untyped dumping ground | High | Medium | Schemas, ownership, typed keys, limits | Authoring validation |
| EAI-R-007 | AI mutates neighboring authorities directly | Medium | High | Request/result bridges and boundary tests | Integration review |
| EAI-R-008 | Save restores invalid scene/provider references | Medium | High | Detached safe-point snapshots only | Migration tests |
| EAI-R-009 | Client-authoritative multiplayer AI | Medium | Critical | Convergence bridge/server authority rule | Network review |
| EAI-R-010 | Vendor package/API drift | Medium | Medium | Separate adapters and version evidence | Adapter release |
| EAI-R-011 | Debug traces leak project/private data | Low | Medium | Redaction and explicit export | Security review |
| EAI-R-012 | 2D projects receive 3D assumptions | Medium | High | No mandatory NavMesh; separate 2D adapters | Lab/adoption review |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Approved package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EAI-D-001 | Authority is actor-local; shared world/scheduler services are explicit and scene/world scoped | Approved | Avoid global AI singleton | Multiple worlds and independent agents | No |
| EAI-D-002 | Neutral core owns observations, memory, scoring, context, scheduling, behavior contracts, and navigation requests | Approved | Reusable infrastructure boundary | Game personality remains project-owned | No |
| EAI-D-003 | Core ships one lightweight state-machine/utility path but not a universal behavior tree | Approved | Useful standalone MVP without architecture lock-in | Visual/planner systems are adapters/modules | No |
| EAI-D-004 | Navigation technology is always optional | Approved | 2D/grid/custom compatibility | Separate provider packages | No |
| EAI-D-005 | Typed blackboard schemas replace arbitrary string/object dictionaries | Approved | Validation and migration safety | More explicit authoring | No |
| EAI-D-006 | Decisions use stable ordering, hysteresis, seeded random, and explanations | Approved | Reproducibility/debugging | Providers must declare nondeterminism | No |
| EAI-D-007 | Live memory, paths, and action tickets are session-only by default | Approved | Avoid invalid durable references | Projects opt into safe snapshots | No |
| EAI-D-008 | Multiplayer AI decisions default to authoritative host/server execution | Approved | Security and consistency | Separate Convergence bridge | No |

### 27.2 Release-blocking questions

No question blocks approval of this feasibility foundation. Implementation later must resolve:

- Exact minimum Unity version and package pins.
- Exact first navigation adapter and 2D pathfinding strategy.
- Whether the lightweight behavior runner remains in the core assembly or a first-party module after prototype evidence.
- Concrete measured budgets and supported agent counts.
- Durable snapshot use cases, if any.

### 27.3 Non-blocking later questions

- Shared squad memory and reservation ownership.
- Behavior tree/GOAP module boundaries.
- Cover/tactical point provider design.
- Inference determinism, platform, security, and performance.
- Client-side prediction/presentation for networked AI.

---

## 28. Milestones and Checkpoint Path

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| A0 - Feasibility foundation | Approved pre-code contract | This document and provider record | Documentation audit |
| A1 - Skeleton | Installable package anatomy | Manifest, asmdefs, docs shell | Clean compile/removal |
| A2 - Agent data/policies | IDs, profiles, memory/scoring/blackboard pure logic | EditMode tests |
| A3 - Runtime host | Agent/world/scheduler lifecycle | PlayMode lifecycle tests |
| A4 - Behavior foundation | State machine, utility selector, action lifecycle | Deterministic behavior tests |
| A5 - Navigation abstraction | Simulated provider/tickets/status | Standalone Lab |
| A6 - Editor/debug | Setup, validation, inspectors, traces | Repeat-safe tooling tests |
| A7 - First project adoption | One narrow Hackulos or Lab actor | Parity report and rollback |
| A8 - First provider adapter | AI Navigation or approved alternative | Separate Integration Lab |
| A9 - Beta/release | Docs, licenses, packaging, evidence | SFGSS-004 gates |

### 28.1 First recommended implementation checkpoint

Dormant until SUITE-DOC-33: create only the package skeleton and documentation shell. No AI runtime code is authorized now.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.
Treat SFGSS-000 and SFGSS-002 through SFGSS-005 as suite authorities.
Treat Instinct - EchoAI Feasibility Foundation v1.0.0 as the Level 2 authority
for provider-neutral sensing, memory, scoring, typed context, scheduling,
behavior contracts, navigation seams, diagnostics, and AI package boundaries.
EchoAI remains an Advanced candidate and no implementation, navigation backend,
Behavior Graph dependency, inference provider, performance claim, or production
compatibility is approved. Package implementation remains locked until SUITE-DOC-33.
Current checkpoint after approval: SUITE-DOC-20 - Clash (EchoCombat) Feasibility Foundation.
Preserve actor-local authority, explicit adapters, request/result boundaries,
and honest Not run evidence. When implementation is eventually authorized,
show complete code and explain every file and step so Jesse can enter it himself.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package foundation | 1.0.0 Approved feasibility foundation |
| Implementation | Not started |
| Laboratory scenarios | 80 planned; Not run |
| Tests | 512 planned; Not run |
| Optional adapters | AI Navigation, Unity Behavior, 2D/custom navigation, Inference; none implemented |
| Known blockers | None for documentation; implementation locked by SUITE-DOC-33 |
| Next checkpoint | SUITE-DOC-20 - Clash (`EchoCombat`) Feasibility Foundation |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and responsibility are clear.
- [x] One universal enemy brain is explicitly rejected.
- [x] Actor-local authority and scene/world services are separated.
- [x] Observation, memory, scoring, blackboard, scheduling, behavior, and navigation contracts are specified.
- [x] Optional provider/adapters remain removable.
- [x] Durable/live state boundaries are documented.
- [x] Standalone Laboratory and tests are planned.
- [x] Diagnostics and performance bounds are specified without false evidence.
- [x] Multiplayer authority and neighboring package boundaries are explicit.
- [x] No implementation is authorized.

### 30.2 Approval record

**Decision:** Approved feasibility foundation  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** EchoAI remains an Advanced candidate. Production implementation, adapters, measured budgets, platform support, and release claims require SUITE-DOC-33 and executed evidence.

---

## Foundation Completion Rule

This feasibility foundation is complete because a new collaborator can determine what Instinct owns, what remains project/peer/provider authority, how an agent observes and remembers, how targets and behaviors are chosen, how navigation is requested without backend lock-in, how state is bounded and diagnosed, how optional systems connect, and what evidence is still missing.


---

## Graph Navigation

#sfgss/package #sfgss/wave/advanced #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
