# Arcana - EchoAbilities Feasibility Foundation Specification

**Document ID:** SFGSS-PKG-ECHOABILITIES  
**Specification version:** 1.0.0  
**Status:** Approved feasibility foundation; EchoAbilities remains an Advanced candidate and implementation remains locked  
**Technical package name:** EchoAbilities  
**Public title:** Arcana - Ability Activation and Effect Orchestration  
**Package ID:** `com.echodevgames.echo-abilities`  
**Runtime namespace:** `EchoDevGames.EchoAbilities`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Repository:** Planned `EchoDevGames/EchoAbilities`  
**Current Notes:** `../Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Required feasibility record:** `../Research Records/SUITE-DOC-21_EchoAbilities_Feasibility_and_Boundary_Record.md`  
**Last updated:** August 4, 2026

> “Arcana carries intention through cost, time, target, and effect. The game still decides what the power means.”

> **Approval rule:** This document approves the Level 2 provider-neutral foundation for EchoAbilities boundaries, identities, ability definitions, owner state, loadouts, activation validation, conditions, costs, charges, cooldowns, targeting, casting, interruption, channels, effect execution, persistence seams, diagnostics, Laboratories, and optional bridges. It does not approve implementation, a universal resource/stat model, specific spells or attacks, class or talent systems, one targeting technology, one status-effect framework, one networking provider, provider-specific prediction, or empirical performance and compatibility claims. Those remain blocked until SUITE-DOC-33 and later implementation evidence.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial feasibility foundation | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved provider-neutral ability definitions, owner state, activation requests, costs, charges, cooldowns, casting, interruption, targeting, typed effects, persistence and multiplayer seams, diagnostics, Laboratory, and explicit boundaries | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** Arcana - Ability Activation and Effect Orchestration  
**Technical identifier:** EchoAbilities  
**Flavor line:** Arcana carries intention through cost, time, target, and effect.  
**Plain-language subtitle:** A provider-neutral Unity package foundation for reusable ability definitions, owner loadouts, activation validation, costs, charges, cooldowns, cast and channel timing, interruption, targeting contracts, typed effect execution, diagnostics, persistence, and multiplayer authority seams.

**One-sentence ownership contract:**

> EchoAbilities owns provider-neutral ability definitions and owner runtime state, grants and loadouts, activation requests and results, conditions, cost preparation and commit coordination, charges, recharge, cooldown groups, cast and channel lifecycle, interruption, targeting contracts, typed effect orchestration, ability-state snapshots, diagnostics, validation, and optional bridges; it does not own character identity, movement, input bindings, camera control, health or general resources, combat formulas, inventory storage, specific spells or attacks, class fantasy, status-effect semantics, animation, VFX, audio, UI, AI decisions, scene travel, save-file transport, multiplayer transport, or one game's ability balance.

### 1.1 Elevator summary

Arcana supplies the reusable lifecycle beneath an ability without turning the suite into one RPG, shooter, action game, or card battler. A project grants an ability definition to an owner, submits an activation request, and supplies a target snapshot or targeting provider. EchoAbilities validates grants, owner gates, conditions, cooldowns, charges, concurrency, targets, and resource costs; creates a stale-safe activation instance; advances through targeting, casting, commitment, effect execution, channeling, recovery, and completion; and returns structured results throughout the lifecycle.

The package coordinates but does not impersonate neighboring authorities. Resource providers own mana, stamina, ammunition, item charges, health costs, or project-defined currencies. Clash owns instantaneous combat resolution. The Vault owns items and equipment. The Fellowship owns durable character identity. The Will owns devices and action maps. Impact owns coordinated presentation feedback. Arcana invokes these systems only through explicit effect, cost, requirement, target, or bridge contracts.

Ability definitions remain immutable. Cooldowns, charges, cast timers, activation instances, queue entries, effect tickets, and provider handles live in owner-scoped runtime state. Specific spells, attacks, class skills, talents, monsters, animations, icons, and balance remain game-owned or belong to `EchoRPG.Foundation` content.

### 1.2 Why this belongs in The Sperk's Forge

Existing projects repeatedly implement the same infrastructure in different shapes: a jump attack checks input, stamina, cooldown, target, animation timing, damage, interruption, and UI; a spell checks mana, cast time, target range, line of sight, effect application, and save state; an interaction ability consumes an item and starts a timer. Without one shared lifecycle, every ability reimplements validation, cost spending, cooldown logic, cancellation, target resolution, and event publication.

Arcana extracts that lifecycle while preserving game-owned meaning. It gives project code one clear activation contract, Clash one effect bridge, The Vault one item-cost bridge, The Fellowship one owner identity seam, The Will one input adapter, The Looking Glass one loadout and cooldown snapshot, and The Convergence one server-authority boundary.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Always paired with “Ability Activation and Effect Orchestration.” |
| Setup guidance/tooltips | Yes | Must explain owners, costs, timing, targets, effects, and commit points plainly. |
| Samples | Optional | Verse-flavored spells may appear but remain replaceable. |
| Runtime API/type names | No lore-only names | Use `AbilityDefinition`, `AbilityActivationRequest`, and `IAbilityEffectExecutor`. |
| Project data | No required Verse content | Games own skills, spells, attacks, classes, icons, animations, and balance. |

---

## 2. Problem Statement

### 2.1 Current problem

Ability code often grows around one button and one coroutine. It then accumulates hidden assumptions about who owns the resource, when the cooldown starts, whether a cast can be interrupted, how targets are selected, which effects have committed, what happens if one provider is missing, and which side is authoritative in multiplayer. A second ability copies the first, changes a few values, and inherits every invisible coupling.

A reusable package must expose the lifecycle without imposing one combat formula or one class system. It must distinguish preview from commitment, source data from runtime state, targeting from input, costs from general resource ownership, effects from presentation, interruption from rollback, and client presentation from authoritative multiplayer mutation.

### 2.2 Evidence from existing work

| Source | Existing pattern or need | Preserve | Improve |
|---|---|---|---|
| Hackulos | Fighter attacks, Necromancer spells, pet summoning, healing, life drain, damage-over-time, direct damage, cooldowns, and interruption | Data-driven RPG content and clear class identity | Keep content outside the general package and give every ability one neutral lifecycle |
| Rescuers2D | Role-specific actions such as axe use, shield actions, ladder actions, crawl, swim, and jump gates | Explicit capability and state gates | Separate controller capabilities from reusable ability timing and cost orchestration |
| Don’t Get Vince’d | Combo attacks, air kicks, invincibility abilities, and meter-driven actions | Semantic gameplay events and focused controllers | Remove duplicated cooldown, resource, timing, and cancellation logic |
| Echo Systems Lab | Weapon definitions, ammo, firing requests, damage messages, and events | Definition/runtime/presentation separation | Provide a generic ability activation layer above weapons and project actions |
| Clash foundation | Provider-neutral instantaneous combat resolution | Clear request, target, commit, and result contracts | Keep ongoing activation timing and effect orchestration outside Clash |
| The Vault | Generic item and equipment ownership | Atomic item transactions and stable IDs | Use explicit cost/effect bridges rather than teaching Arcana inventory internals |

### 2.3 Consequences of doing nothing

- Every action invents a different cooldown, charge, and cast model.
- Input, animation, damage, resources, and UI become directly coupled.
- Interrupted casts may spend twice, refund incorrectly, or publish stale effects.
- Client-authored targets and costs become difficult to validate in multiplayer.
- Ability state leaks into ScriptableObjects or scene objects and contaminates tests.
- Save files cannot distinguish granted abilities from active transient activations.
- Designers cannot inspect why an ability is unavailable without stepping through project code.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide a genre-neutral ability definition and runtime activation model.
- Keep immutable definitions separate from owner-scoped mutable state.
- Make validation, targeting, cost preparation, commitment, effects, and completion explicit.
- Support instant abilities, timed casts, bounded channels, recovery, charges, and cooldown groups.
- Make interruption and post-commit cancellation behavior honest.
- Integrate through typed providers and bridges rather than reflection or string method names.
- Remain useful without Clash, The Vault, The Fellowship, The Will, The Looking Glass, or The Convergence.
- Expose diagnostics explaining exactly why activation succeeded, failed, or became unavailable.

### 3.2 Non-goals

- A universal health, mana, stamina, ammunition, inventory, or attribute system.
- Specific spells, attacks, talents, classes, combo trees, or game balance.
- A universal status-effect, aura, buff, debuff, or damage-over-time framework in the MVP.
- Input bindings, target reticles, production hotbars, animation graphs, VFX, audio, or camera movement.
- One physics targeting solution or one networking provider.
- Automatic rollback of arbitrary effects after their commit point.
- A visual ability graph in the MVP.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Needs one cooldown-based action | Configure an ability, owner, simulated resource, target, and effect in the Laboratory |
| Gameplay programmer | Needs attacks, spells, items, or actions | Submit requests and receive structured lifecycle results without copying cooldown coroutines |
| Designer | Authors ability content | Configure timing, costs, charges, cooldowns, targets, effects, interruption, and loadout metadata through validated assets |
| UI developer | Builds hotbars and loadout screens | Read immutable snapshots and submit commands without owning ability truth |
| Multiplayer developer | Validates client requests | Route activation through a provider-neutral authoritative seam |
| Tester | Reproduces edge cases | Simulate stale targets, missing providers, interrupts, timeouts, queues, and replayed requests in isolation |

### 3.4 Measurable success criteria

- Installs into a clean supported Unity project with zero compile errors.
- Core activation works without any other Sperk's Forge runtime package.
- Definition assets remain unchanged after play and tests.
- One instant, one cast, one interrupted cast, and one channel path pass in the standalone Laboratory.
- Costs, charges, cooldowns, and effects never commit twice for one request ID.
- A failed pre-commit activation leaves authoritative resource and ability state unchanged.
- Every unavailable, denied, interrupted, cancelled, failed, and completed outcome has a structured result and stable diagnostic code.
- Optional bridges may be removed without breaking the core.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Unity gameplay programmers.
- Technical designers authoring reusable actions.
- RPG, action, shooter, puzzle, and interaction-system developers.
- UI, save, networking, and tools programmers integrating ability state.
- Testers validating timing, costs, targets, and authority.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| EABL-UC-001 | Activate instant self ability | Project/gameplay code | Ability granted and available | Costs and state commit once, effects execute, completion result publishes | MVP |
| EABL-UC-002 | Cast targeted ability | Project/gameplay code | Valid owner, target, condition, and resource provider | Cast advances through configured phases and executes effects at commitment | MVP |
| EABL-UC-003 | Interrupt cast | Project/bridge | Active cast allows interrupt category | Cast stops according to pre/post-commit rules | MVP |
| EABL-UC-004 | Channel ability | Project/gameplay code | Cast completed and channel policy valid | Bounded ticks execute until completion or interruption | MVP |
| EABL-UC-005 | Spend charge and cooldown | Ability service | Charge and cooldown available | Charge decrements and cooldown/recharge begins exactly once | MVP |
| EABL-UC-006 | Equip loadout ability | Project/UI | Owner and loadout revision valid | Ability grant and slot update commit atomically in Arcana state | MVP |
| EABL-UC-007 | Save owner ability state | Chronicle bridge/project | No active unsafe activation snapshot requested | Detached grants/loadout and approved timer state exports | MVP |
| EABL-UC-008 | Authoritative network activation | Convergence bridge | Server/host validates request | Authoritative result and reconciliation data publish | Later bridge |

### 4.3 Explicitly unsupported use cases

- Treating ability display names as durable IDs.
- Writing mutable cooldowns or charges into shared definitions.
- Calling arbitrary methods by string or reflection from effect assets.
- Trusting a client-provided target, cost, cooldown, or effect result without authoritative validation.
- Saving active coroutines, Unity object references, targeting cursors, or effect executor handles.
- Assuming all interrupted abilities refund committed costs.
- Using EchoAbilities as a full combat, stats, inventory, animation, or class framework.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Ability definition and catalog contracts.
- Owner-scoped grants, loadouts, charges, cooldowns, active activations, queues, and revisions.
- Activation request validation and idempotency.
- Condition-provider orchestration.
- Targeting request and snapshot contracts.
- Cost preparation and commit coordination.
- Cast, channel, interruption, recovery, and completion lifecycle.
- Typed effect execution order, failure policy, cancellation, and causality.
- Semantic events, diagnostics, validation, setup, and standalone Laboratory evidence.
- Detached ability-state snapshots and migration seams.

### 5.2 The package does not own

- Durable character identity, rosters, spawning, or possession.
- Health, mana, stamina, ammunition, currencies, inventory, or project resource truth.
- Damage and healing formulas or target resource mutation.
- Input devices, action maps, reticles, cursor position, or production UI.
- Animation graphs, VFX, audio playback, camera movement, or feedback rendering.
- AI decision making or world-interaction outcomes.
- Save files, settings storage, scene travel, or multiplayer transport.
- Specific spells, attacks, classes, talents, status effects, or balance.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | Arcana interaction |
|---|---|---|
| Character identity and control | The Fellowship or project | Owner IDs and optional bridge; no roster ownership |
| Input and device context | The Will or project | Adapter submits semantic activation, cancel, and target commands |
| General resources and items | Project, The Vault, or RPG package | One mutation-capable cost provider transaction |
| Instant damage and healing | Clash | Typed effect executor submits `CombatRequest` after Arcana commitment |
| Feedback | Impact | Effect/presentation bridge requests semantic feedback |
| Camera and aim presentation | The Eye/project | Targeting and presentation bridge only |
| UI and hotbars | The Looking Glass/project | Reads snapshots and submits commands |
| Save transport | The Chronicle | Versioned participant bridge |
| Multiplayer authority | The Convergence | Authoritative activation and reconciliation bridge |
| Objectives | The Path | Observes committed ability outcomes through bridge/project adapter |
| AI decisions | Instinct/project | Requests abilities through the same public activation API |
| Specific RPG content | `EchoRPG.Foundation`/project | Definitions and data, not core dependencies |

### 5.4 Boundary tests

1. Would the capability still make sense for a dash, scanner pulse, healing station, firearm reload, spell, pet summon, or puzzle action?
2. Does it describe the ability lifecycle, or does it belong to the target/resource/effect authority?
3. Can the core compile and prove itself without the proposed neighboring package?
4. Is presentation being mistaken for authoritative activation state?
5. Does the proposal require hidden reflection, a particular input map, or one combat formula?
6. Would removal preserve project-owned resources, characters, items, and saved content?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

The package must:

- Compile with only declared Unity dependencies.
- Function without First Light, Clash, The Vault, The Fellowship, The Will, The Looking Glass, The Chronicle, Impact, Instinct, or The Convergence.
- Use simulated owners, targets, resources, conditions, effects, and clocks in its Laboratory.
- Avoid direct references to project assemblies.
- Avoid a mandatory input asset, EventSystem, camera backend, animation controller, or physics targeting system.
- Keep content and configured project assets outside immutable package source.
- Expose service injection and explicit provider registrations.
- Fail safely when optional providers are unavailable.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Planned evidence |
|---|---|---|
| Installed alone | Core definitions, service, simulated providers, and Laboratory compile | EABL-T-PKG range |
| Enter Laboratory directly | Development initializer creates only the required authority | EABL-LAB-001 onward |
| Clash absent | Non-combat simulated effects still execute | Effect tests |
| Vault absent | Simulated resource provider handles costs | Cost tests |
| Fellowship absent | Project-created owner IDs work | Owner tests |
| Will/UI absent | Scripted commands drive activation | Laboratory tests |
| Convergence absent | Local authority policy works | Authority tests |
| Duplicate root present | Duplicate rejects before provider registration or state mutation | Lifecycle tests |
| Sample removed | Runtime and Editor assemblies remain valid | Packaging tests |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity runtime modules | Platform | Yes | Supported Unity baseline | ScriptableObjects, GameObject lifecycle, time, and basic serialization | Package cannot run without Unity |
| Unity Test Framework | Test only | Tests only | Verified during implementation | Automated EditMode and PlayMode tests | Runtime remains unaffected |
| Other Sperk's Forge packages | Optional bridge | No | Defined per bridge | Integration only | Core compiles and operates alone |

### 6.4 Forbidden dependencies

- Project-specific assemblies.
- A mandatory Echo package dependency in the neutral core.
- Samples or tests referenced from runtime assemblies.
- Reflection-based effect or condition method dispatch.
- Hidden Resources paths, scene names, tags, layers, input maps, or save filenames.
- Vendor networking, targeting, animation, or RPG frameworks in the core.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
|---|---|---|---|---:|---|
| EABL-CAP-001 | Duplicate-safe authority | Claim one abilities authority before provider registration or state mutation | Approved | Yes | Runtime |
| EABL-CAP-002 | Ability definitions | Immutable stable-ID definitions for activation, timing, costs, targets, cooldowns, charges, and effects | Approved | Yes | Runtime/Data |
| EABL-CAP-003 | Owner state | Bounded actor/owner ability state with grants, loadouts, cooldowns, charges, and active instances | Approved | Yes | Runtime |
| EABL-CAP-004 | Activation requests | Immutable request/result model with request IDs, owner IDs, ability IDs, and target snapshots | Approved | Yes | Runtime |
| EABL-CAP-005 | Availability conditions | Read-only condition providers with Allowed, Denied, and Unavailable results | Approved | Yes | Runtime |
| EABL-CAP-006 | Targeting contracts | Provider-neutral self, entity, point, direction, and bounded collection targeting | Approved | Yes | Runtime |
| EABL-CAP-007 | Cost transaction | One mutation-capable cost provider transaction plus read-only requirement providers | Approved | Yes | Runtime |
| EABL-CAP-008 | Charges | Per-owner charges with sequential recharge and stable state revisions | Approved | Yes | Runtime |
| EABL-CAP-009 | Cooldowns | Ability-local and shared cooldown groups including project-defined global groups | Approved | Yes | Runtime |
| EABL-CAP-010 | Cast timing | Instant and timed casting with scaled or unscaled clocks | Approved | Yes | Runtime |
| EABL-CAP-011 | Commit policies | Commit at cast start or cast completion with explicit irreversible boundaries | Approved | Yes | Runtime |
| EABL-CAP-012 | Interruption | Typed interruption requests, phase policies, priorities, and stale-safe cancellation | Approved | Yes | Runtime |
| EABL-CAP-013 | Channels | Bounded channel duration and deterministic ticks after activation commitment | Approved | Yes | Runtime |
| EABL-CAP-014 | Recovery | Post-effect recovery windows that block configured concurrency groups | Approved | Yes | Runtime |
| EABL-CAP-015 | Typed effects | Explicit effect executor registrations with typed payloads and stable effect-type IDs | Approved | Yes | Runtime |
| EABL-CAP-016 | Effect sequence | Ordered blocking and non-blocking effect steps with explicit failure policy | Approved | Yes | Runtime |
| EABL-CAP-017 | Concurrency groups | Bounded per-owner groups with reject, replace-if-interruptible, and one-item queue policies | Approved | Yes | Runtime |
| EABL-CAP-018 | Loadouts | Project-defined slots, grants, removals, snapshots, and revisions | Approved | Yes | Runtime/Data |
| EABL-CAP-019 | Events | Semantic activation, cast, commit, effect, interruption, cooldown, charge, and completion events | Approved | Yes | Runtime |
| EABL-CAP-020 | Diagnostics | Bounded state, activation traces, provider health, and stable EABL codes | Approved | Yes | Runtime/Editor |
| EABL-CAP-021 | Persistence snapshot | Versioned grants, loadouts, optional cooldown/charge state, aliases, and unknown-record preservation | Approved | Yes | Runtime |
| EABL-CAP-022 | Multiplayer seam | Authority gates, predicted-presentation hooks, sequence IDs, and reconciliation results | Approved | Yes | Bridge |
| EABL-CAP-023 | Editor authoring | Definition, catalog, loadout, effect, target, cooldown, and dependency validation | Approved | Yes | Editor |
| EABL-CAP-024 | Standalone Laboratory | Simulated owners, targets, resources, conditions, effects, clocks, and network authority | Approved | Yes | Sample/Test |
| EABL-CAP-025 | Passive abilities | Always-on or event-reactive passive effect orchestration | Deferred | No | Later |
| EABL-CAP-026 | Per-tick channel costs | Transactional resource cost on every channel tick | Deferred | No | Later |
| EABL-CAP-027 | Status-effect framework | Durations, stacking, dispels, auras, and periodic effects | Deferred | No | Later |
| EABL-CAP-028 | Ability graph editor | Visual graph authoring and debugging | Deferred | No | Later |
| EABL-CAP-029 | Prediction engine | Provider-specific gameplay prediction and rollback simulation | Rejected core | No | Provider |
| EABL-CAP-030 | Specific spells/classes | RPG spell books, classes, talents, and game content | Rejected core | No | Project/RPG |

### 7.2 MVP capability set

The smallest complete release provides one authority, immutable definitions, owner grants/loadouts, activation requests/results, read-only conditions, one mutation-capable cost transaction, charges, sequential recharge, ability and shared cooldown groups, instant and timed cast paths, explicit start/completion commit policy, interruptions, bounded channels, recovery, typed ordered effects, owner concurrency groups, snapshots, diagnostics, tooling, and one standalone Laboratory.

### 7.3 Later capability set

Later releases may add passive/reactive abilities, per-tick channel costs, visual ability graphs, status-effect modules, richer target queries, talent trees, combo chains, provider-specific prediction, rollback simulation, and project or RPG content packages. Each must preserve the authority and transaction boundaries established here.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Universal status-effect framework | Deferred | Requires separate duration, stacking, aura, dispel, resistance, save, and multiplayer design | Dedicated effect-system workshop |
| Passive/reactive abilities | Deferred | Event-source ownership and sustained lifecycle require more design | Active ability MVP proven |
| Per-tick channel costs | Deferred | Needs repeated transactional resource semantics and failure policy | Cost provider protocol validated |
| Visual graph editor | Deferred | Tooling should not precede stable runtime contracts | Runtime and list authoring proven |
| Universal stats/resources | Rejected core | Belongs to project or genre package | Never in neutral core |
| Specific spells/classes | Rejected core | Game-owned content | `EchoRPG.Foundation` or project package |
| Built-in rollback networking | Rejected core | Provider-specific and topology-specific | Convergence provider adapter |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Ability definitions, catalogs, target profiles, cooldown groups, loadout schemas, effect records, policies | Active timers, charges, provider handles, Unity scene targets |
| Runtime state/behavior | Root, owner state, activation instances, queues, cooldowns, recharge, casts, channels, effect tickets | Editor tooling, production UI, game-specific effect meaning |
| Presentation/feedback | Optional presenters, hotbar adapters, target reticles, animation/audio/VFX bridges | Authoritative costs, cooldowns, or activation state |

### 8.2 Component topology

```text
EchoAbilitiesRoot
├── AbilityCatalogRegistry
├── AbilityOwnerRegistry
│   └── AbilityOwnerState
│       ├── Grants and loadout snapshot
│       ├── Charges and cooldowns
│       ├── Active activation groups
│       └── Bounded pending queues
├── ActivationCoordinator
│   ├── Condition registrations
│   ├── Targeting registrations
│   ├── Resource-cost registrations
│   └── Effect executor registrations
├── AbilityClockRegistry
├── Diagnostics and bounded history
└── Optional bridge registrations
```

The root coordinates application-session definitions and owner state. It does not become a character, resource, combat, input, or UI authority.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root required? | Yes for the default application-session service |
| Root type | `EchoAbilitiesRoot` |
| Duplicate behavior | Reject duplicate before provider registration, owner state creation, subscriptions, or timing side effects |
| Initialization trigger | Explicit setup or `Awake` claim followed by validation and initialization |
| Shutdown behavior | Reject new requests, cancel uncommitted activations, stop future channel ticks, unregister providers, clear bounded state |
| Direct-scene behavior | Development initializer creates the configured root only when absent |
| Test seam | `IEchoAbilitiesService`, injected clocks, simulated providers, and factories |

### 8.4 Activation lifecycle

1. Receive immutable request and validate request identity.
2. Resolve owner, grant, loadout, concurrency group, and ability definition.
3. Evaluate owner gates and read-only conditions.
4. Resolve or validate the target snapshot.
5. Verify charges, cooldowns, and queue policy.
6. Prepare the resource-cost transaction without mutation.
7. Create a generational activation instance.
8. Enter targeting or casting when configured.
9. Reach the definition's declared commit point.
10. Commit prepared resource costs exactly once.
11. Commit internal charge, cooldown, activation, and group state through a non-failing in-memory transition.
12. Execute typed effect steps with causality and cancellation policy.
13. Enter channeling or recovery when configured.
14. Complete, interrupt, cancel, or fail with structured results.
15. Release handles and retain only bounded diagnostic history.

### 8.5 Activation states

- Requested
- WaitingForTarget
- Preparing
- Casting
- Committing
- Executing
- Channeling
- Recovering
- Completed
- Interrupted
- Cancelled
- Failed

State transitions are validated. A stale callback cannot move a newer activation instance.

### 8.6 Commit-point model

Two MVP commit policies are approved:

| Policy | Commit point | Pre-commit interruption | Post-commit interruption |
|---|---|---|---|
| AtCastStart | Immediately after validation/preparation | Not applicable after start commit | Stops future work but does not silently refund costs, charges, or cooldowns |
| AtCastCompletion | After cast timer completes | Cancels without committing costs, charges, or cooldowns | Stops future effects/channel work after commitment |

Effect executors may have their own declared commit points. Cancellation never implies rollback of already committed external effects.

### 8.7 Cost transaction model

The MVP permits one mutation-capable cost provider per activation. It may atomically handle several cost lines under its own authority. Additional providers may contribute read-only requirements.

```text
Validate request
    -> prepare cost token with expected revisions
        -> cast/target lifecycle
            -> commit cost token exactly once
                -> commit Arcana internal state
                    -> publish activation committed
                        -> execute effects
```

If cost commit fails, Arcana does not consume charges, start cooldowns, or publish a committed activation. After a successful cost commit, Arcana's prepared internal transition is required to be deterministic and non-failing.

### 8.8 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Authority claim | Duplicate rejected | Existing root remains authoritative | EABL-001 |
| Missing definition | Request validation | Unavailable | No mutation | EABL-101 |
| Ability not granted | Owner validation | Denied | No mutation | EABL-102 |
| Condition denied | Condition evaluation | Denied with reasons | No mutation | EABL-201 |
| Provider missing | Provider resolution | Unavailable | No mutation | EABL-202 |
| Invalid/stale target | Target validation | Denied or unavailable | Return to caller or cancel targeting | EABL-301 |
| Insufficient resource | Cost preparation | Denied | No mutation | EABL-401 |
| Cost commit failed | Commit | Failed | Arcana state remains uncommitted | EABL-402 |
| Interrupted before commit | Cast lifecycle | Interrupted | No cost/charge/cooldown commit | EABL-501 |
| Interrupted after commit | Execution/channel | Interrupted after commit | Stop future work; preserve committed state | EABL-502 |
| Effect executor failure | Effect execution | Failed or partial by policy | Abort or continue configured steps | EABL-601 |
| Timeout | Async provider/effect | Failed | Cancel future uncommitted work | EABL-602 |
| Replay request | Idempotency | Duplicate/replayed result | Do not execute again | EABL-701 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `AbilityDefinition` | Central immutable ability lifecycle and content references | Yes | No | Yes |
| `AbilityCatalog` | Bounded list of definitions and aliases | Yes | No | Yes |
| `AbilityLoadoutSchema` | Project-defined slots and restrictions | Yes | No | Yes |
| `AbilityTargetProfile` | Target kind, limits, provider IDs, and validation policy | Yes | No | Yes |
| `AbilityCooldownGroupDefinition` | Shared cooldown identity and duration policy | Yes | No | Yes |
| `AbilityEffectRecord` | Typed effect executor ID, payload, target scope, timing, and failure policy | Yes within definition | No | Yes |
| `AbilityCostRecord` | Cost provider ID, resource ID, amount, and requirement metadata | Yes within definition | No | Yes |
| `AbilityInterruptionPolicy` | Phase and interrupt-category rules | Yes or embedded | No | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `AbilityOwnerState` | EchoAbilities root | Owner registration/session | Owner removal or root shutdown | Detached snapshot may include grants/loadout and approved timers |
| `AbilityGrantState` | Owner state | Until removed/imported | Explicit mutation | Durable when project chooses |
| `AbilityChargeState` | Owner state | Session or imported snapshot | Definition/migration policy | Optional durable state |
| `AbilityCooldownState` | Owner state | Session or imported snapshot | Explicit reset/expiry | Optional durable state |
| `AbilityActivationInstance` | Owner state | One activation | Complete/interrupt/cancel/fail | Never durable |
| `AbilityTargetSnapshot` | Activation | Request/targeting lifecycle | Replaced or activation ends | Never durable by default |
| `PreparedCostToken` | Cost provider | Preparation to commit/cancel | Commit, cancel, timeout | Never durable |
| `AbilityEffectTicket` | Effect executor | Effect lifetime | Complete/cancel/timeout | Never durable |

### 9.3 Stable identifiers

The package defines stable domain IDs for abilities, catalogs, owners, loadout schemas, slots, cooldown groups, concurrency groups, condition providers, target providers, resource providers, effect types, interrupt categories, requests, activations, effects, and snapshots.

- Unity asset GUIDs remain Editor asset identities and are not Player/runtime contracts.
- `AbilityId` survives asset renames and moves.
- `AbilityOwnerId` references a provider/project-owned durable actor identity but does not replace The Fellowship's `CharacterId`.
- `AbilityActivationId` is session/runtime identity and is never reused during the active root lifetime.
- Aliases and tombstones support definition renames/removals under SFGSS-003.

### 9.4 ScriptableObject safety

Definitions may contain immutable tuning and references. They must not contain current charges, cooldown timestamps, active owners, target GameObjects, resource values, cast progress, queue entries, or effect tickets. Runtime tests must verify definition immutability across repeated activations and scene transitions.

### 9.5 Serialization and migration

`AbilityStateDocument` contains:

- Document and package schema versions.
- Owner stable ID.
- Granted ability records.
- Loadout slot records.
- Optional charge records.
- Optional cooldown/recharge records according to project policy.
- Opaque unknown provider extension records.
- Aliases, migration provenance, and source metadata.

Active targeting, casts, channels, effect tickets, prepared costs, transient queues, and provider handles are never serialized. Cooldown restoration may use remaining duration or a project-provided elapsed-time policy; no wall-clock behavior is assumed silently.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `IEchoAbilitiesService` | Interface | Main activation, owner, loadout, query, interrupt, and snapshot facade | Root or injected implementation |
| `AbilityId` | Value type | Stable ability identity | Definition/project tooling |
| `AbilityOwnerId` | Value type | Stable owner identity | Project/Fellowship adapter |
| `AbilityActivationRequest` | Immutable struct/class | Owner, ability, request ID, target data, authority context, and options | Caller |
| `AbilityActivationResult` | Immutable result | Accepted, queued, denied, unavailable, interrupted, failed, or completed information | Service |
| `AbilityActivationHandle` | Generational handle | Query/cancel/interrupt one activation | Service |
| `AbilityOwnerSnapshot` | Immutable DTO | Grants, loadout, charges, cooldowns, active groups, and revision | Service |
| `AbilityDefinition` | ScriptableObject | Immutable ability configuration | Project |
| `IAbilityConditionProvider` | Interface | Side-effect-free availability conditions | Project/bridge |
| `IAbilityTargetingProvider` | Interface | Target acquisition and validation | Project/adapter |
| `IAbilityResourceProvider` | Interface | Prepare, commit, and cancel one cost transaction | Project/bridge |
| `IAbilityEffectExecutor` | Interface | Execute one typed effect record | Project/bridge |
| `IAbilityClock` | Interface | Scaled/unscaled/simulated timing | Core/project/test |
| `AbilityStateDocument` | DTO | Detached durable owner state | Service/Chronicle bridge |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Main-loop rule |
|---|---|---|---|---|
| `ActivateAsync(request, cancellation)` | Begin or queue one ability activation | Valid root/request/owner | Structured activation result and handle | Main-thread entry; async providers obey documented thread rules |
| `TryInterrupt(request)` | Request interruption of an active activation | Valid owner/activation and policy | Accepted, denied, stale, too late, or unavailable | Main thread |
| `Cancel(handle)` | Cancel caller-owned uncommitted work when permitted | Valid generational handle | Cancelled, too late, stale, or completed | Main thread |
| `GetOwnerSnapshot(ownerId)` | Read immutable owner ability state | Owner registered | Snapshot or unavailable | Main thread unless copied DTO documented thread-safe |
| `GrantAbility(request)` | Add one ability grant | Valid owner/definition/revision | Atomic Arcana-state result | Main thread |
| `RemoveAbility(request)` | Remove grant under active policy | Valid owner/grant/revision | Atomic result or denied while active | Main thread |
| `AssignLoadoutSlot(request)` | Change loadout assignment | Valid schema/slot/revision | Atomic snapshot revision | Main thread |
| `ExportState(ownerId)` | Create detached versioned state | Owner registered and safe export point | Document or structured failure | Main thread capture; detached processing may continue elsewhere |
| `PrepareImport(document)` | Validate and migrate without mutation | Supported document | Prepared import or failure | Detached validation allowed |
| `CommitImport(prepared)` | Replace approved owner state | Valid prepared token/revision | Atomic Arcana-state commit | Main thread |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `ActivationRequested` | Service | After basic request acceptance | Request/activation IDs | Observational only |
| `ActivationStateChanged` | Activation | After authoritative state transition | Old/new state and reason | May not mutate internal state directly |
| `AbilityCommitted` | Service | After costs and Arcana internal state commit | Owner, ability, activation, causality | Safe point for presentation/effects observers |
| `EffectStepCompleted` | Coordinator | After one executor result | Step ID/result | Bounded and semantic |
| `ActivationInterrupted` | Service | After interruption state commit | Phase, category, pre/post-commit | No rollback assumption |
| `CooldownChanged` | Owner state | After cooldown commit/expiry/reset | Group, remaining, revision | Snapshot-friendly |
| `ChargesChanged` | Owner state | After charge/recharge commit | Ability, current/max, revision | Snapshot-friendly |
| `LoadoutChanged` | Owner state | After atomic loadout commit | Old/new revision | UI may refresh snapshot |
| `ActivationCompleted` | Service | After terminal state | Final result and committed-effect summary | Presentation listeners not required for completion |

### 10.4 Async and cancellation policy

- Public async operations use fresh awaitables/tasks according to the Unity baseline and final implementation decision.
- Cancellation is cooperative before the relevant commit point.
- Cost-provider and effect-executor operations must declare timeout and cancellation behavior.
- Stale async completions carry activation/effect generations and are ignored with diagnostics.
- Scene unload or owner removal cancels uncommitted work and stops future channel ticks.
- Already committed costs, cooldowns, charges, and external effects are not silently reversed.

### 10.5 API ergonomics

The novice path uses one configured root, one owner, one definition, one simulated target, one simulated resource, and one simulated effect. The advanced path injects the service, clocks, owner IDs, conditions, targeting, resources, effects, authority gates, and persistence providers.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install EchoAbilities.
2. Open **Tools > EchoDevGames > Arcana > Setup**.
3. Create or select an `EchoAbilitiesConfiguration`.
4. Create a root prefab or scene object.
5. Create an ability catalog and sample definition.
6. Choose simulated Laboratory providers or project integration adapters.
7. Preview all created/modified assets.
8. Apply setup and open the Arcana Laboratory.
9. Run validation.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create configuration | Project-owned configuration asset | Nothing existing by default | Yes | Undo | Setup receipt |
| Create root prefab | Project-owned prefab | Optional selected scene only after confirmation | Yes | Undo/backup | Setup receipt |
| Create catalog/definition | Project-owned assets | Catalog only | Yes | Undo | Asset report |
| Create Laboratory fixture | Imported sample assets | No package source | Yes | Sample removal | Sample report |
| Repair missing references | Only previewed safe references | Selected assets | Yes | Undo/backup | Repair receipt |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Arcana Setup | Installer | Create configuration, root, catalog, and Laboratory assets | No |
| Ability Definition Inspector | Designer | Edit timing, costs, charges, cooldowns, targets, interruption, and effects | No |
| Catalog Validator | Maintainer | Find duplicate IDs, missing aliases, invalid providers, and cycles | No |
| Owner Runtime Monitor | Tester | Inspect grants, loadouts, active activations, queues, cooldowns, and charges | Runtime only in Play Mode |
| Activation Simulator | Designer/tester | Simulate conditions, targets, costs, interrupts, timeouts, and effects | No production dependency |
| Snapshot Inspector | Maintainer | Inspect detached ability-state documents without applying them | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| EABL-VAL-001 | Duplicate `AbilityId` | Blocker | Generate new ID with explicit migration choice | No |
| EABL-VAL-002 | Missing definition ID | Blocker | Generate ID | Yes before release use |
| EABL-VAL-003 | Missing cost provider ID | Error | Select provider/remove cost | No |
| EABL-VAL-004 | Multiple mutation cost providers in one MVP ability | Blocker | Consolidate provider | No |
| EABL-VAL-005 | Missing effect executor ID | Error | Select executor/remove step | No |
| EABL-VAL-006 | Invalid cast/channel/recovery duration | Error | Clamp preview | Yes with approval |
| EABL-VAL-007 | Cooldown group missing | Error | Create/select group | No |
| EABL-VAL-008 | Charge max/recharge contradiction | Error | Correct values | No |
| EABL-VAL-009 | Target profile incompatible with effect target scope | Error | Edit profile/step | No |
| EABL-VAL-010 | Runtime assembly references Editor | Blocker | Manual code correction | No |
| EABL-VAL-011 | Public asset GUID instability | Release blocker | Restore meta/migrate intentionally | No |
| EABL-VAL-012 | Laboratory contains unrelated Echo dependency | Release blocker | Remove dependency | No |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

- Git URL.
- Local package path.
- Embedded package for development.
- Tarball after distribution begins.
- Workshop selection when the setup facade exists.

Every claimed route remains `Not run` until executed under SFGSS-004.

### 12.2 Minimal scene setup

- One `EchoAbilitiesRoot`.
- One configuration and ability catalog.
- One registered owner or simulated owner.
- One granted ability definition.
- One condition/target/resource/effect path, simulated or project-owned.

### 12.3 Boot-scene setup

The production root may live in a Boot scene, preload scene, or project-managed persistent scene. First Light may initialize it through a separate startup-step bridge. EchoAbilities does not require First Light.

### 12.4 Direct-scene setup

A development-only initializer checks for an existing authority, creates the configured root only when absent, labels the session as development-initialized, and uses the same duplicate-safety rules as production.

### 12.5 Scene isolation rule

The standalone Laboratory contains no Clash, Vault, Fellowship, Will, UI, Chronicle, or Convergence code. Simulated providers prove the core. Separate Integration Laboratories prove each bridge.

---

## 13. Standalone Test Lab and Samples

### 13.1 Laboratory purpose

The **Arcana Ability Lifecycle Laboratory** proves definitions, grants, loadouts, validation, targeting, costs, charges, cooldowns, casting, interruption, channels, effects, concurrency, snapshots, diagnostics, and teardown with simulated providers and deterministic clocks.

### 13.2 Required Laboratory contents

- One root and configuration.
- Several ability definitions: instant, cast, channel, charge-based, shared-cooldown, and intentionally invalid.
- Simulated owner/loadout registry.
- Simulated condition provider.
- Simulated target provider.
- Transactional simulated resource provider.
- Simulated blocking and non-blocking effect executors.
- Scaled, unscaled, and manual clocks.
- Controls for activation, targeting, interrupt, cancel, reset, snapshot, import, timeout, and provider failure.
- Readouts for owner revision, active state, commit point, charges, cooldowns, queue, targets, costs, effects, and diagnostics.

### 13.3 Laboratory acceptance checklist

| Test ID | Category | Action | Automation type | Status |
|---|---|---|---|---|
| EABL-LAB-001 | Authority and lifecycle | Create one abilities root and initialize a simulated owner registry | Manual/automated | Not run |
| EABL-LAB-002 | Authority and lifecycle | Introduce a duplicate root before initialization | Manual/automated | Not run |
| EABL-LAB-003 | Authority and lifecycle | Introduce a duplicate root after initialization | Manual/automated | Not run |
| EABL-LAB-004 | Authority and lifecycle | Disable and re-enable the authoritative root | Manual/automated | Not run |
| EABL-LAB-005 | Authority and lifecycle | Unload the scene that supplied a registered owner | Manual/automated | Not run |
| EABL-LAB-006 | Authority and lifecycle | Dispose provider registrations out of order | Manual/automated | Not run |
| EABL-LAB-007 | Authority and lifecycle | Reset the Laboratory and confirm bounded state is cleared | Manual/automated | Not run |
| EABL-LAB-008 | Authority and lifecycle | Shut down while an ability activation is pending | Manual/automated | Not run |
| EABL-LAB-009 | Definitions and loadouts | Register a valid ability catalog with unique stable IDs | Manual/automated | Not run |
| EABL-LAB-010 | Definitions and loadouts | Reject duplicate ability IDs | Manual/automated | Not run |
| EABL-LAB-011 | Definitions and loadouts | Grant an ability to one owner loadout | Manual/automated | Not run |
| EABL-LAB-012 | Definitions and loadouts | Remove an inactive granted ability | Manual/automated | Not run |
| EABL-LAB-013 | Definitions and loadouts | Reject removal of an active ability under deny-active policy | Manual/automated | Not run |
| EABL-LAB-014 | Definitions and loadouts | Move an ability between loadout slots | Manual/automated | Not run |
| EABL-LAB-015 | Definitions and loadouts | Reject a stale loadout revision | Manual/automated | Not run |
| EABL-LAB-016 | Definitions and loadouts | Preserve unknown ability records during import | Manual/automated | Not run |
| EABL-LAB-017 | Activation validation | Activate an instant self-targeted ability | Manual/automated | Not run |
| EABL-LAB-018 | Activation validation | Reject an ability not granted to the owner | Manual/automated | Not run |
| EABL-LAB-019 | Activation validation | Reject activation while disabled by an owner gate | Manual/automated | Not run |
| EABL-LAB-020 | Activation validation | Reject activation while on cooldown | Manual/automated | Not run |
| EABL-LAB-021 | Activation validation | Reject activation with no available charge | Manual/automated | Not run |
| EABL-LAB-022 | Activation validation | Reject activation when a condition provider denies it | Manual/automated | Not run |
| EABL-LAB-023 | Activation validation | Return Unavailable when a required provider is absent | Manual/automated | Not run |
| EABL-LAB-024 | Activation validation | Reject a replayed activation request ID | Manual/automated | Not run |
| EABL-LAB-025 | Targeting | Resolve a self target | Manual/automated | Not run |
| EABL-LAB-026 | Targeting | Resolve one entity target | Manual/automated | Not run |
| EABL-LAB-027 | Targeting | Resolve one world-point target | Manual/automated | Not run |
| EABL-LAB-028 | Targeting | Resolve one direction target | Manual/automated | Not run |
| EABL-LAB-029 | Targeting | Resolve a bounded target collection | Manual/automated | Not run |
| EABL-LAB-030 | Targeting | Reject a stale target snapshot | Manual/automated | Not run |
| EABL-LAB-031 | Targeting | Reject a target that fails line-of-sight validation through a provider | Manual/automated | Not run |
| EABL-LAB-032 | Targeting | Cancel a targeting session before commitment | Manual/automated | Not run |
| EABL-LAB-033 | Costs and resources | Prepare and commit one resource cost | Manual/automated | Not run |
| EABL-LAB-034 | Costs and resources | Reject activation with insufficient resources | Manual/automated | Not run |
| EABL-LAB-035 | Costs and resources | Reject a stale prepared cost token | Manual/automated | Not run |
| EABL-LAB-036 | Costs and resources | Fail resource commit and preserve ability state | Manual/automated | Not run |
| EABL-LAB-037 | Costs and resources | Commit several cost lines through one resource provider transaction | Manual/automated | Not run |
| EABL-LAB-038 | Costs and resources | Use a read-only requirement provider beside the mutation provider | Manual/automated | Not run |
| EABL-LAB-039 | Costs and resources | Reject multiple mutation-capable cost providers in the MVP | Manual/automated | Not run |
| EABL-LAB-040 | Costs and resources | Confirm post-commit cancellation does not silently refund costs | Manual/automated | Not run |
| EABL-LAB-041 | Charges and cooldowns | Consume one charge and begin recharge | Manual/automated | Not run |
| EABL-LAB-042 | Charges and cooldowns | Restore one charge after sequential recharge duration | Manual/automated | Not run |
| EABL-LAB-043 | Charges and cooldowns | Apply an ability-local cooldown | Manual/automated | Not run |
| EABL-LAB-044 | Charges and cooldowns | Apply a shared cooldown group | Manual/automated | Not run |
| EABL-LAB-045 | Charges and cooldowns | Apply a project-defined global cooldown group | Manual/automated | Not run |
| EABL-LAB-046 | Charges and cooldowns | Reject stale cooldown mutation | Manual/automated | Not run |
| EABL-LAB-047 | Charges and cooldowns | Reset cooldowns through an explicit development command | Manual/automated | Not run |
| EABL-LAB-048 | Charges and cooldowns | Export and import optional cooldown and charge state | Manual/automated | Not run |
| EABL-LAB-049 | Casting and interruption | Complete a cast that commits at cast completion | Manual/automated | Not run |
| EABL-LAB-050 | Casting and interruption | Interrupt a pre-commit cast without spending resources | Manual/automated | Not run |
| EABL-LAB-051 | Casting and interruption | Start a cast that commits at cast start | Manual/automated | Not run |
| EABL-LAB-052 | Casting and interruption | Interrupt a post-commit cast without rolling back committed state | Manual/automated | Not run |
| EABL-LAB-053 | Casting and interruption | Reject an interrupt that does not match the ability policy | Manual/automated | Not run |
| EABL-LAB-054 | Casting and interruption | Accept a stronger interrupt category | Manual/automated | Not run |
| EABL-LAB-055 | Casting and interruption | Cancel a cast through owner teardown | Manual/automated | Not run |
| EABL-LAB-056 | Casting and interruption | Reject a stale cast completion callback | Manual/automated | Not run |
| EABL-LAB-057 | Channels and recovery | Enter a bounded channel after cast completion | Manual/automated | Not run |
| EABL-LAB-058 | Channels and recovery | Execute deterministic channel ticks | Manual/automated | Not run |
| EABL-LAB-059 | Channels and recovery | Interrupt a channel and stop future ticks | Manual/automated | Not run |
| EABL-LAB-060 | Channels and recovery | Complete a channel and enter recovery | Manual/automated | Not run |
| EABL-LAB-061 | Channels and recovery | Reject reactivation during recovery | Manual/automated | Not run |
| EABL-LAB-062 | Channels and recovery | Use unscaled timing when configured | Manual/automated | Not run |
| EABL-LAB-063 | Channels and recovery | Use scaled timing when configured | Manual/automated | Not run |
| EABL-LAB-064 | Channels and recovery | Confirm per-tick resource costs are deferred from the MVP | Manual/automated | Not run |
| EABL-LAB-065 | Effect execution | Execute one blocking typed effect | Manual/automated | Not run |
| EABL-LAB-066 | Effect execution | Execute several ordered blocking effects | Manual/automated | Not run |
| EABL-LAB-067 | Effect execution | Execute one non-blocking presentation effect | Manual/automated | Not run |
| EABL-LAB-068 | Effect execution | Abort remaining effects under stop-on-failure policy | Manual/automated | Not run |
| EABL-LAB-069 | Effect execution | Continue remaining effects under continue-on-failure policy | Manual/automated | Not run |
| EABL-LAB-070 | Effect execution | Reject an unavailable effect executor | Manual/automated | Not run |
| EABL-LAB-071 | Effect execution | Reject a stale asynchronous effect completion | Manual/automated | Not run |
| EABL-LAB-072 | Effect execution | Preserve effect causality and activation IDs | Manual/automated | Not run |
| EABL-LAB-073 | Concurrency and ownership | Reject a second activation in the same concurrency group | Manual/automated | Not run |
| EABL-LAB-074 | Concurrency and ownership | Allow activations in separate concurrency groups | Manual/automated | Not run |
| EABL-LAB-075 | Concurrency and ownership | Replace an interruptible active activation | Manual/automated | Not run |
| EABL-LAB-076 | Concurrency and ownership | Reject replacement of an uninterruptible activation | Manual/automated | Not run |
| EABL-LAB-077 | Concurrency and ownership | Queue one bounded pending activation when configured | Manual/automated | Not run |
| EABL-LAB-078 | Concurrency and ownership | Reject activation beyond the bounded queue | Manual/automated | Not run |
| EABL-LAB-079 | Concurrency and ownership | Transfer control ownership without transferring ability authority | Manual/automated | Not run |
| EABL-LAB-080 | Concurrency and ownership | Remove an owner and cancel all uncommitted activations | Manual/automated | Not run |
| EABL-LAB-081 | Persistence and multiplayer | Export granted abilities and loadout slots | Manual/automated | Not run |
| EABL-LAB-082 | Persistence and multiplayer | Import a versioned ability-state snapshot | Manual/automated | Not run |
| EABL-LAB-083 | Persistence and multiplayer | Reject an active cast or channel in a durable snapshot | Manual/automated | Not run |
| EABL-LAB-084 | Persistence and multiplayer | Preserve unknown ability and provider records | Manual/automated | Not run |
| EABL-LAB-085 | Persistence and multiplayer | Reject a client-authored authoritative activation under server policy | Manual/automated | Not run |
| EABL-LAB-086 | Persistence and multiplayer | Accept an authoritative server activation | Manual/automated | Not run |
| EABL-LAB-087 | Persistence and multiplayer | Reconcile predicted presentation with an authoritative result | Manual/automated | Not run |
| EABL-LAB-088 | Persistence and multiplayer | Confirm no networking SDK is required by the core | Manual/automated | Not run |
| EABL-LAB-089 | Diagnostics, stress, and removal | Bound activation history under sustained use | Manual/automated | Not run |
| EABL-LAB-090 | Diagnostics, stress, and removal | Bound cooldown and charge diagnostics | Manual/automated | Not run |
| EABL-LAB-091 | Diagnostics, stress, and removal | Recover after one condition provider throws | Manual/automated | Not run |
| EABL-LAB-092 | Diagnostics, stress, and removal | Recover after one effect executor times out | Manual/automated | Not run |
| EABL-LAB-093 | Diagnostics, stress, and removal | Detect leaked registrations at Laboratory reset | Manual/automated | Not run |
| EABL-LAB-094 | Diagnostics, stress, and removal | Remove an optional bridge without breaking the core | Manual/automated | Not run |
| EABL-LAB-095 | Diagnostics, stress, and removal | Remove and reinstall EchoAbilities while preserving project-owned data | Manual/automated | Not run |
| EABL-LAB-096 | Diagnostics, stress, and removal | Export a privacy-safe final diagnostic snapshot | Manual/automated | Not run |

### 13.4 Optional integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| Arcana + Clash | EchoAbilities and EchoCombat | Ability effect submits damage/healing request | Depends on two authorities |
| Arcana + Vault | EchoAbilities and EchoInventory | Item/ammunition cost provider | Depends on inventory transactions |
| Arcana + Fellowship | EchoAbilities and EchoCharacters | Durable character-owner mapping and loadouts | Depends on character authority |
| Arcana + Will + Looking Glass | EchoAbilities, EchoInput, EchoUI | Hotbar activation, targeting, and cooldown presentation | Depends on input and UI |
| Arcana + Convergence | EchoAbilities and EchoMultiplayer | Server-authoritative activation and reconciliation | Depends on selected provider adapter |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The core is nonvisual. It exposes semantic definitions, snapshots, availability reasons, targeting requirements, timing state, cooldowns, charges, and activation events. The Looking Glass or project UI owns hotbars, tooltips, targeting reticles, cast bars, cooldown fills, charge indicators, error messages, and loadout screens.

### 14.2 Required presentation states

- Available.
- Denied with reason.
- Unavailable because provider/content is missing.
- Targeting.
- Casting.
- Committed/executing.
- Channeling.
- Recovering.
- On cooldown.
- Out of charges.
- Interrupted.
- Failed.

### 14.3 Accessibility requirements

- Ability availability must not rely on color alone.
- Cast/channel progress must expose text or numeric alternatives.
- Timing presentation must support reduced motion and configurable feedback.
- Hold/toggle behavior belongs to input/UI adapters and user preferences, not hard-coded core assumptions.
- Audio cues require visible alternatives through project presentation.
- Rapid flashes, shake, and rumble are requested through Impact and respect accessibility scaling.

### 14.4 Visual customization

Icons, names, descriptions, fonts, colors, animations, VFX, reticles, cast bars, and layout are project-owned or localized references. Runtime code never assumes a specific visual style.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Authority status | API/Inspector | Editor, development, release-safe summary | Constant |
| Provider registry health | API/Inspector | Editor/development | Bounded |
| Owner state snapshot | API/Inspector | Development; redacted release option | Bounded per owner |
| Activation trace | Ring buffer/export | Development | Configurable bounded history |
| Cooldown/charge state | API/Inspector | Development | Bounded |
| Validation report | Editor | Editor | Manual/pre-Play/pre-build |
| Support snapshot | Export | Explicit user action | Bounded and redacted |

### 15.2 Structured status

The package exposes:

- Package version and root identity.
- Configuration/catalog identity.
- Registered owner count and bounded capacity.
- Provider IDs, capabilities, and health.
- Active activation count by owner/group/state.
- Pending queue counts.
- Cooldown/charge summaries.
- Recent results and stable failure codes.
- Timeout, stale callback, replay, and leak counters.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| EABL-001 | Error | Duplicate authority rejected | Remove duplicate root/prefab |
| EABL-101 | Error | Ability definition unavailable | Repair catalog or migration alias |
| EABL-102 | Info/Warning | Ability not granted | Grant ability or correct owner/loadout |
| EABL-201 | Info | Condition denied | Inspect structured condition reasons |
| EABL-202 | Warning | Required provider unavailable | Install/register provider or remove dependency |
| EABL-301 | Info/Warning | Target invalid or stale | Reacquire target/repair provider |
| EABL-401 | Info | Cost unavailable | Restore resources or adjust ability |
| EABL-402 | Error | Cost commit failed | Inspect provider transaction and revisions |
| EABL-501 | Info | Interrupted before commit | Expected interruption path |
| EABL-502 | Info/Warning | Interrupted after commit | Do not expect automatic rollback |
| EABL-601 | Error | Effect executor failed | Inspect effect provider and failure policy |
| EABL-602 | Error | Async operation timed out | Repair provider or adjust bounded timeout |
| EABL-701 | Warning | Duplicate/replayed request rejected | Inspect caller/network replay |
| EABL-801 | Warning | Bounded capacity reached | Adjust limits after measurement |
| EABL-901 | Release blocker | Public asset/serialization incompatibility | Restore GUID/schema or provide migration |

### 15.4 Observatory bridge

A separate bridge may publish root health, provider status, active states, request rates, interruption counts, cooldown/charge summaries, stale callbacks, replay rejections, and bounded traces. The core never depends on The Observatory.

### 15.5 Logging policy

Logs are stable-code-first, categorized, bounded, and actionable. They exclude localized ability text, private account IDs, raw input, provider credentials, complete target metadata, and arbitrary effect payloads. No per-frame log spam is allowed.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Ability definitions/catalogs | Project content | Project | As assets | Unity assets |
| Grants/loadout | Owner/profile/save | EchoAbilities state | Optional/usually yes | Chronicle or project provider |
| Charges/cooldowns | Session or save | EchoAbilities state | Optional by project policy | Chronicle/project |
| Active targeting/cast/channel | Session | EchoAbilities | No | None |
| Effect tickets/provider handles | Session | Providers | No | None |
| Unknown extension records | Durable | Source provider/project | Preserved | Document payload |

### 16.2 Standalone behavior

Without The Chronicle, the package runs entirely in memory and may export/import detached `AbilityStateDocument` values through project code. It does not silently choose a path or filename.

### 16.3 Optional participant/provider contract

The Chronicle bridge registers a versioned participant for each configured owner/profile boundary. Capture occurs at a safe point. Import validates definitions, aliases, provider records, cooldown policy, charge bounds, and loadout schema before one Arcana-state commit.

### 16.4 Failure and recovery

- Missing document: initialize project defaults.
- Older document: run contiguous migrations against a preserved source copy.
- Newer unsupported document: return `Unavailable`; do not mutate state.
- Unknown abilities/providers: preserve opaque records and expose unresolved status.
- Invalid loadout slot: move to configured unassigned collection or fail by project policy.
- Active transient state in document: reject or ignore only through an explicit migration rule.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Every connection is explicit, removable, versioned, and owned by a bridge or project adapter. Installing a peer package does not silently alter Arcana behavior.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| Clash | Separate bridge | Arcana/Clash bridge package | Arcana -> Clash | Committed effect submits combat requests and receives results | No |
| The Vault | Separate bridge | Arcana/Vault bridge | Bidirectional | Cost preparation/commit, item charge or ammunition state | No |
| The Fellowship | Separate bridge | Arcana/Fellowship bridge | Fellowship -> Arcana | Character ID to owner ID, spawn/despawn/loadout lifecycle | No |
| The Will | Separate bridge | Arcana/Will bridge | Will -> Arcana | Semantic activate/cancel/target commands | No |
| The Looking Glass | Separate bridge | UI/Arcana bridge | Bidirectional | Snapshots, availability, cast/cooldown/charge events, commands | No |
| Impact | Separate bridge | Arcana/Impact bridge | Arcana -> Impact | Semantic cast, commit, interrupt, fail, and complete feedback requests | No |
| The Chronicle | Separate bridge | Chronicle/Arcana bridge | Bidirectional | Versioned owner-state participant | No |
| The Convergence | Provider bridge | Arcana/Multiplayer bridge | Bidirectional | Authoritative requests/results, sequence, prediction presentation, reconciliation | No |
| The Path | Project or bridge | Objectives/Arcana bridge | Arcana -> Objectives | Committed activation/effect/completion signals | No |
| Instinct | Project or bridge | AI/Arcana adapter | Instinct -> Arcana | AI activation requests and availability queries | No |
| EchoRPG.Foundation | Genre bridge/content | RPG family | Bidirectional | Resource, stat, class, spell, talent, and effect content | No |

### 17.3 Bridge placement decision

Two-package bridges ship separately when they reference both runtime assemblies. Provider-specific networking and RPG integrations ship as separate provider/genre packages. Tiny sample-only adapters remain inside Integration Laboratories, not core runtime code.

### 17.4 Integration failure behavior

- Missing peer: bridge does not compile/install; core remains unaffected.
- Disabled provider: returns `Unavailable` and never mutates core state.
- Version mismatch: validator blocks unsupported bridge version.
- Initialization order: bridge registers when both authorities are ready and disposes before either shuts down.
- Teardown: active bridge-owned effect or cost operations cancel according to commit status.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Planned target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Idle root allocations | Zero steady-state managed allocations per frame | Profiler in empty Laboratory | Measured before beta claim |
| Activation validation | Bounded by configured conditions/providers/effects | Stress Laboratory | Measured and documented |
| Active activations | Configured bounded maximum per root and owner | Stress Laboratory | Graceful rejection at limit |
| Cooldown/charge updates | No full-catalog scan per owner per frame | Profiler/custom counters | Measured before beta |
| Diagnostic history | Fixed ring-buffer capacity | Runtime diagnostics | Never unbounded |

### 18.2 Allocation policy

- No per-frame reflection or asset discovery.
- Stable arrays/lists or pooled runtime records where measurement justifies them.
- Snapshots are immutable copies created on demand, not every frame.
- Cooldowns/recharge use scheduled/bucketed updates rather than scanning every definition blindly.
- Effect payloads and target collections are bounded by configuration.

### 18.3 Scene and domain reload behavior

Provider registrations unsubscribe cleanly. Static caches reset under supported Enter Play Mode configurations. Owner and activation generations reject stale callbacks after reload or teardown. Direct-scene development initialization follows production duplicate rules.

### 18.4 Scalability limits

Advertised limits cover owners, granted abilities per owner, active groups, pending requests, targets per request, conditions, costs, effect steps, channels, providers, and diagnostic history. Numeric defaults remain provisional until measured.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

The core may process owner IDs, target IDs, resource IDs, ability IDs, and network authority context. It must not log account credentials, raw authentication data, private session tokens, typed input, localized text, or arbitrary effect payload contents.

### 19.2 Trust boundaries

- Client activation requests, target snapshots, costs, and effect claims are untrusted in authoritative multiplayer.
- Imported state documents are validated for size, versions, IDs, bounds, aliases, and unknown records.
- Provider responses are checked against request/activation generations.
- Effect and cost executors cannot be discovered from arbitrary untrusted strings.
- Server/host validates owner, grant, cooldown, charges, conditions, target, and resources before authoritative commit.

### 19.3 Platform behavior

| Platform | Supported? | Special behavior | Validation required |
|---|---:|---|---|
| Windows | Planned | Standard Player path | Clean build/runtime tests |
| macOS | Planned | Standard Player path | Clean build/runtime tests |
| Linux | Planned | Standard Player path | Clean build/runtime tests |
| WebGL | Planned/conditional | Threading and timing constraints | Dedicated build tests |
| Mobile | Planned | Suspend/resume and touch targeting adapters | Device tests |
| Console | Unknown | Platform certification and provider restrictions | Later approved environment |

No platform is marked Supported until SFGSS-004 evidence exists.

---

## 20. Package and Repository Structure

### 20.1 Proposed package anatomy

```text
Packages/com.echodevgames.echo-abilities/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   └── Developer/
├── Runtime/
│   ├── Core/
│   ├── Definitions/
│   ├── Owners/
│   ├── Activation/
│   ├── Targeting/
│   ├── Costs/
│   ├── Timing/
│   ├── Effects/
│   ├── Persistence/
│   ├── Diagnostics/
│   └── EchoDevGames.EchoAbilities.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Authoring/
│   ├── Validation/
│   ├── Simulation/
│   └── EchoDevGames.EchoAbilities.Editor.asmdef
├── Samples~/
│   └── Arcana Ability Lifecycle Laboratory/
└── Tests/
    ├── Editor/
    └── Runtime/
```

Bridges and provider adapters that reference optional packages live in separate UPM packages under SFGSS-002.

### 20.2 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoAbilities.Runtime` | Runtime | Unity runtime only | Yes | Neutral ability service, data, lifecycle, providers, state, diagnostics |
| `EchoDevGames.EchoAbilities.Editor` | Editor | Runtime plus UnityEditor | No | Setup, inspectors, validation, simulation |
| `EchoDevGames.EchoAbilities.Tests.Editor` | Editor tests | Runtime/Editor/Test Framework | No | EditMode tests |
| `EchoDevGames.EchoAbilities.Tests.Runtime` | Runtime tests | Runtime/Test Framework | No | PlayMode tests |

### 20.3 Repository files

- Concise README and documentation routes.
- Public API and provider guides.
- Definition and loadout authoring guide.
- Targeting, cost, effect, timing, and interruption guides.
- Persistence and migration guide.
- Diagnostics code reference.
- Integration guide index.
- Laboratory guide and evidence registry.
- Changelog, license, notices, contribution guidance, release checklist, and stable `.meta` files.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | Planned Unity 6000.0 floor | Not run | Exact compatibility claim requires implementation evidence |
| Unity Test Framework | Planned | Not run | Test-only |
| Optional Echo bridges | Per bridge specification | Not run | Never required by core |

### 21.2 Semantic versioning policy

- Patch: diagnostics, documentation, compatible validation, and bug fixes.
- Minor: additive ability fields with backward-compatible defaults, new optional providers/effect types, and additive APIs.
- Major: breaking public APIs, identity formats, state schemas, commit semantics, provider contracts, or definition serialization.

### 21.3 Deprecation policy

Deprecated members receive documentation, warnings, migration paths, and at least one supported minor release when practical before removal. Durable data aliases and migrations outlive source API deprecations when saves depend on them.

### 21.4 GUID and asset compatibility

Public scripts, definitions, configuration templates, prefabs, samples, and migration assets preserve committed `.meta` files. Moves and renames retain GUIDs when identity is intended to survive.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview and authority boundaries.
- Installation and five-minute simulated ability quick start.
- Ability definition and catalog authoring.
- Grants and loadouts.
- Conditions, targeting, costs, charges, cooldowns, casting, channels, interruption, and recovery.
- Typed effect executors.
- Standalone Laboratory guide.
- Diagnostics and troubleshooting.
- Persistence and migration.
- Integration guide index.
- Known limitations and deferred capabilities.

### 22.2 Required developer documentation

- Architecture and activation lifecycle.
- Commit-point and cost transaction semantics.
- Provider registration and teardown.
- Threading, timing, cancellation, and stale-callback rules.
- State-document schema and migration.
- Testing strategy, release workflow, ADRs, Current Notes, and checkpoint status.

### 22.3 Documentation truth rule

Examples must compile against the documented release. Provider and bridge examples must state optional dependencies. Performance, platform, compatibility, and multiplayer claims remain `Not run` until executed.

### 22.4 Repository/Obsidian workflow

Documentation lives in Git with the package. Current Notes captures provisional discoveries, which are promoted into the package specification, ADRs, research, tests, guides, changelog, or release records at each checkpoint.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, definitions, validators, policies, migrations | Duplicate IDs, timing validation, effect ordering | Yes |
| PlayMode unit/integration | Root, owners, activation, clocks, providers | Cast/interrupt/commit/channel paths | Yes |
| Standalone Laboratory | User-visible isolated lifecycle | Simulated owner/resource/target/effects | Yes |
| Bridge Integration Laboratory | Optional peer connection | Clash, Vault, Fellowship, UI/Input, Convergence | When bridge ships |
| Clean-project install | Packaging and independence | Git/local/tarball/embedded routes | Yes before release claim |
| Existing-project adoption | Migration/parity | Hackulos or another real project | Before adoption claim |

### 23.2 Required test categories

- Authority and duplicate protection.
- Definition and identity validation.
- Grants and loadout revisions.
- Conditions and provider availability.
- Targets and stale snapshots.
- Cost prepare/commit/cancel/failure.
- Charges, recharge, cooldown groups, and clock policies.
- Cast start/completion commit policies.
- Interruption and cancellation before/after commit.
- Channels, ticks, recovery, and timeouts.
- Effect ordering, failures, cancellations, and stale completions.
- Queues, concurrency, idempotency, and bounds.
- Persistence, migration, unknown data, and removal/reinstall.
- Multiplayer authority and prediction-presentation seams.
- Performance, platforms, accessibility, privacy, diagnostics, and distribution.

### 23.3 Test case registry

| Test ID | Category | Requirement | Setup | Action | Expected result | Status |
|---|---|---|---|---|---|---|
| EABL-T-001 | Authority and lifecycle | Authority and lifecycle accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-002 | Authority and lifecycle | Authority and lifecycle rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-003 | Authority and lifecycle | Authority and lifecycle rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-004 | Authority and lifecycle | Authority and lifecycle preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-005 | Authority and lifecycle | Authority and lifecycle reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-006 | Authority and lifecycle | Authority and lifecycle does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-007 | Authority and lifecycle | Authority and lifecycle remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-008 | Authority and lifecycle | Authority and lifecycle cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-009 | Authority and lifecycle | Authority and lifecycle survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-010 | Authority and lifecycle | Authority and lifecycle isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-011 | Authority and lifecycle | Authority and lifecycle preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-012 | Authority and lifecycle | Authority and lifecycle avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-013 | Authority and lifecycle | Authority and lifecycle uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-014 | Authority and lifecycle | Authority and lifecycle works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-015 | Authority and lifecycle | Authority and lifecycle records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-016 | Authority and lifecycle | Authority and lifecycle supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-017 | Authority and lifecycle | Authority and lifecycle handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-018 | Authority and lifecycle | Authority and lifecycle handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-019 | Authority and lifecycle | Authority and lifecycle rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-020 | Authority and lifecycle | Authority and lifecycle produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-021 | Authority and lifecycle | Authority and lifecycle preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-022 | Authority and lifecycle | Authority and lifecycle avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-023 | Authority and lifecycle | Authority and lifecycle validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-024 | Authority and lifecycle | Authority and lifecycle supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-025 | Authority and lifecycle | Authority and lifecycle keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-026 | Authority and lifecycle | Authority and lifecycle keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-027 | Authority and lifecycle | Authority and lifecycle preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-028 | Authority and lifecycle | Authority and lifecycle documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-029 | Authority and lifecycle | Authority and lifecycle documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-030 | Authority and lifecycle | Authority and lifecycle passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-031 | Stable identity and handles | Stable identity and handles accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-032 | Stable identity and handles | Stable identity and handles rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-033 | Stable identity and handles | Stable identity and handles rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-034 | Stable identity and handles | Stable identity and handles preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-035 | Stable identity and handles | Stable identity and handles reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-036 | Stable identity and handles | Stable identity and handles does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-037 | Stable identity and handles | Stable identity and handles remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-038 | Stable identity and handles | Stable identity and handles cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-039 | Stable identity and handles | Stable identity and handles survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-040 | Stable identity and handles | Stable identity and handles isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-041 | Stable identity and handles | Stable identity and handles preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-042 | Stable identity and handles | Stable identity and handles avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-043 | Stable identity and handles | Stable identity and handles uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-044 | Stable identity and handles | Stable identity and handles works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-045 | Stable identity and handles | Stable identity and handles records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-046 | Stable identity and handles | Stable identity and handles supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-047 | Stable identity and handles | Stable identity and handles handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-048 | Stable identity and handles | Stable identity and handles handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-049 | Stable identity and handles | Stable identity and handles rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-050 | Stable identity and handles | Stable identity and handles produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-051 | Stable identity and handles | Stable identity and handles preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-052 | Stable identity and handles | Stable identity and handles avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-053 | Stable identity and handles | Stable identity and handles validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-054 | Stable identity and handles | Stable identity and handles supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-055 | Stable identity and handles | Stable identity and handles keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-056 | Stable identity and handles | Stable identity and handles keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-057 | Stable identity and handles | Stable identity and handles preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-058 | Stable identity and handles | Stable identity and handles documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-059 | Stable identity and handles | Stable identity and handles documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-060 | Stable identity and handles | Stable identity and handles passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-061 | Definitions and catalogs | Definitions and catalogs accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-062 | Definitions and catalogs | Definitions and catalogs rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-063 | Definitions and catalogs | Definitions and catalogs rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-064 | Definitions and catalogs | Definitions and catalogs preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-065 | Definitions and catalogs | Definitions and catalogs reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-066 | Definitions and catalogs | Definitions and catalogs does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-067 | Definitions and catalogs | Definitions and catalogs remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-068 | Definitions and catalogs | Definitions and catalogs cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-069 | Definitions and catalogs | Definitions and catalogs survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-070 | Definitions and catalogs | Definitions and catalogs isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-071 | Definitions and catalogs | Definitions and catalogs preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-072 | Definitions and catalogs | Definitions and catalogs avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-073 | Definitions and catalogs | Definitions and catalogs uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-074 | Definitions and catalogs | Definitions and catalogs works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-075 | Definitions and catalogs | Definitions and catalogs records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-076 | Definitions and catalogs | Definitions and catalogs supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-077 | Definitions and catalogs | Definitions and catalogs handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-078 | Definitions and catalogs | Definitions and catalogs handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-079 | Definitions and catalogs | Definitions and catalogs rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-080 | Definitions and catalogs | Definitions and catalogs produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-081 | Definitions and catalogs | Definitions and catalogs preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-082 | Definitions and catalogs | Definitions and catalogs avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-083 | Definitions and catalogs | Definitions and catalogs validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-084 | Definitions and catalogs | Definitions and catalogs supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-085 | Definitions and catalogs | Definitions and catalogs keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-086 | Definitions and catalogs | Definitions and catalogs keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-087 | Definitions and catalogs | Definitions and catalogs preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-088 | Definitions and catalogs | Definitions and catalogs documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-089 | Definitions and catalogs | Definitions and catalogs documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-090 | Definitions and catalogs | Definitions and catalogs passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-091 | Loadouts and grants | Loadouts and grants accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-092 | Loadouts and grants | Loadouts and grants rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-093 | Loadouts and grants | Loadouts and grants rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-094 | Loadouts and grants | Loadouts and grants preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-095 | Loadouts and grants | Loadouts and grants reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-096 | Loadouts and grants | Loadouts and grants does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-097 | Loadouts and grants | Loadouts and grants remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-098 | Loadouts and grants | Loadouts and grants cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-099 | Loadouts and grants | Loadouts and grants survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-100 | Loadouts and grants | Loadouts and grants isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-101 | Loadouts and grants | Loadouts and grants preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-102 | Loadouts and grants | Loadouts and grants avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-103 | Loadouts and grants | Loadouts and grants uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-104 | Loadouts and grants | Loadouts and grants works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-105 | Loadouts and grants | Loadouts and grants records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-106 | Loadouts and grants | Loadouts and grants supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-107 | Loadouts and grants | Loadouts and grants handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-108 | Loadouts and grants | Loadouts and grants handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-109 | Loadouts and grants | Loadouts and grants rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-110 | Loadouts and grants | Loadouts and grants produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-111 | Loadouts and grants | Loadouts and grants preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-112 | Loadouts and grants | Loadouts and grants avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-113 | Loadouts and grants | Loadouts and grants validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-114 | Loadouts and grants | Loadouts and grants supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-115 | Loadouts and grants | Loadouts and grants keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-116 | Loadouts and grants | Loadouts and grants keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-117 | Loadouts and grants | Loadouts and grants preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-118 | Loadouts and grants | Loadouts and grants documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-119 | Loadouts and grants | Loadouts and grants documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-120 | Loadouts and grants | Loadouts and grants passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-121 | Activation validation | Activation validation accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-122 | Activation validation | Activation validation rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-123 | Activation validation | Activation validation rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-124 | Activation validation | Activation validation preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-125 | Activation validation | Activation validation reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-126 | Activation validation | Activation validation does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-127 | Activation validation | Activation validation remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-128 | Activation validation | Activation validation cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-129 | Activation validation | Activation validation survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-130 | Activation validation | Activation validation isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-131 | Activation validation | Activation validation preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-132 | Activation validation | Activation validation avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-133 | Activation validation | Activation validation uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-134 | Activation validation | Activation validation works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-135 | Activation validation | Activation validation records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-136 | Activation validation | Activation validation supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-137 | Activation validation | Activation validation handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-138 | Activation validation | Activation validation handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-139 | Activation validation | Activation validation rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-140 | Activation validation | Activation validation produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-141 | Activation validation | Activation validation preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-142 | Activation validation | Activation validation avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-143 | Activation validation | Activation validation validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-144 | Activation validation | Activation validation supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-145 | Activation validation | Activation validation keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-146 | Activation validation | Activation validation keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-147 | Activation validation | Activation validation preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-148 | Activation validation | Activation validation documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-149 | Activation validation | Activation validation documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-150 | Activation validation | Activation validation passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-151 | Conditions and availability | Conditions and availability accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-152 | Conditions and availability | Conditions and availability rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-153 | Conditions and availability | Conditions and availability rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-154 | Conditions and availability | Conditions and availability preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-155 | Conditions and availability | Conditions and availability reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-156 | Conditions and availability | Conditions and availability does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-157 | Conditions and availability | Conditions and availability remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-158 | Conditions and availability | Conditions and availability cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-159 | Conditions and availability | Conditions and availability survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-160 | Conditions and availability | Conditions and availability isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-161 | Conditions and availability | Conditions and availability preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-162 | Conditions and availability | Conditions and availability avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-163 | Conditions and availability | Conditions and availability uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-164 | Conditions and availability | Conditions and availability works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-165 | Conditions and availability | Conditions and availability records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-166 | Conditions and availability | Conditions and availability supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-167 | Conditions and availability | Conditions and availability handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-168 | Conditions and availability | Conditions and availability handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-169 | Conditions and availability | Conditions and availability rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-170 | Conditions and availability | Conditions and availability produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-171 | Conditions and availability | Conditions and availability preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-172 | Conditions and availability | Conditions and availability avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-173 | Conditions and availability | Conditions and availability validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-174 | Conditions and availability | Conditions and availability supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-175 | Conditions and availability | Conditions and availability keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-176 | Conditions and availability | Conditions and availability keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-177 | Conditions and availability | Conditions and availability preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-178 | Conditions and availability | Conditions and availability documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-179 | Conditions and availability | Conditions and availability documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-180 | Conditions and availability | Conditions and availability passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-181 | Targeting and target snapshots | Targeting and target snapshots accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-182 | Targeting and target snapshots | Targeting and target snapshots rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-183 | Targeting and target snapshots | Targeting and target snapshots rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-184 | Targeting and target snapshots | Targeting and target snapshots preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-185 | Targeting and target snapshots | Targeting and target snapshots reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-186 | Targeting and target snapshots | Targeting and target snapshots does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-187 | Targeting and target snapshots | Targeting and target snapshots remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-188 | Targeting and target snapshots | Targeting and target snapshots cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-189 | Targeting and target snapshots | Targeting and target snapshots survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-190 | Targeting and target snapshots | Targeting and target snapshots isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-191 | Targeting and target snapshots | Targeting and target snapshots preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-192 | Targeting and target snapshots | Targeting and target snapshots avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-193 | Targeting and target snapshots | Targeting and target snapshots uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-194 | Targeting and target snapshots | Targeting and target snapshots works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-195 | Targeting and target snapshots | Targeting and target snapshots records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-196 | Targeting and target snapshots | Targeting and target snapshots supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-197 | Targeting and target snapshots | Targeting and target snapshots handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-198 | Targeting and target snapshots | Targeting and target snapshots handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-199 | Targeting and target snapshots | Targeting and target snapshots rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-200 | Targeting and target snapshots | Targeting and target snapshots produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-201 | Targeting and target snapshots | Targeting and target snapshots preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-202 | Targeting and target snapshots | Targeting and target snapshots avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-203 | Targeting and target snapshots | Targeting and target snapshots validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-204 | Targeting and target snapshots | Targeting and target snapshots supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-205 | Targeting and target snapshots | Targeting and target snapshots keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-206 | Targeting and target snapshots | Targeting and target snapshots keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-207 | Targeting and target snapshots | Targeting and target snapshots preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-208 | Targeting and target snapshots | Targeting and target snapshots documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-209 | Targeting and target snapshots | Targeting and target snapshots documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-210 | Targeting and target snapshots | Targeting and target snapshots passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-211 | Costs and resource transactions | Costs and resource transactions accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-212 | Costs and resource transactions | Costs and resource transactions rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-213 | Costs and resource transactions | Costs and resource transactions rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-214 | Costs and resource transactions | Costs and resource transactions preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-215 | Costs and resource transactions | Costs and resource transactions reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-216 | Costs and resource transactions | Costs and resource transactions does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-217 | Costs and resource transactions | Costs and resource transactions remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-218 | Costs and resource transactions | Costs and resource transactions cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-219 | Costs and resource transactions | Costs and resource transactions survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-220 | Costs and resource transactions | Costs and resource transactions isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-221 | Costs and resource transactions | Costs and resource transactions preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-222 | Costs and resource transactions | Costs and resource transactions avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-223 | Costs and resource transactions | Costs and resource transactions uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-224 | Costs and resource transactions | Costs and resource transactions works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-225 | Costs and resource transactions | Costs and resource transactions records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-226 | Costs and resource transactions | Costs and resource transactions supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-227 | Costs and resource transactions | Costs and resource transactions handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-228 | Costs and resource transactions | Costs and resource transactions handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-229 | Costs and resource transactions | Costs and resource transactions rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-230 | Costs and resource transactions | Costs and resource transactions produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-231 | Costs and resource transactions | Costs and resource transactions preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-232 | Costs and resource transactions | Costs and resource transactions avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-233 | Costs and resource transactions | Costs and resource transactions validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-234 | Costs and resource transactions | Costs and resource transactions supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-235 | Costs and resource transactions | Costs and resource transactions keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-236 | Costs and resource transactions | Costs and resource transactions keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-237 | Costs and resource transactions | Costs and resource transactions preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-238 | Costs and resource transactions | Costs and resource transactions documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-239 | Costs and resource transactions | Costs and resource transactions documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-240 | Costs and resource transactions | Costs and resource transactions passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-241 | Charges and recharge | Charges and recharge accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-242 | Charges and recharge | Charges and recharge rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-243 | Charges and recharge | Charges and recharge rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-244 | Charges and recharge | Charges and recharge preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-245 | Charges and recharge | Charges and recharge reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-246 | Charges and recharge | Charges and recharge does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-247 | Charges and recharge | Charges and recharge remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-248 | Charges and recharge | Charges and recharge cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-249 | Charges and recharge | Charges and recharge survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-250 | Charges and recharge | Charges and recharge isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-251 | Charges and recharge | Charges and recharge preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-252 | Charges and recharge | Charges and recharge avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-253 | Charges and recharge | Charges and recharge uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-254 | Charges and recharge | Charges and recharge works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-255 | Charges and recharge | Charges and recharge records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-256 | Charges and recharge | Charges and recharge supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-257 | Charges and recharge | Charges and recharge handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-258 | Charges and recharge | Charges and recharge handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-259 | Charges and recharge | Charges and recharge rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-260 | Charges and recharge | Charges and recharge produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-261 | Charges and recharge | Charges and recharge preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-262 | Charges and recharge | Charges and recharge avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-263 | Charges and recharge | Charges and recharge validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-264 | Charges and recharge | Charges and recharge supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-265 | Charges and recharge | Charges and recharge keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-266 | Charges and recharge | Charges and recharge keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-267 | Charges and recharge | Charges and recharge preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-268 | Charges and recharge | Charges and recharge documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-269 | Charges and recharge | Charges and recharge documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-270 | Charges and recharge | Charges and recharge passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-271 | Cooldown groups | Cooldown groups accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-272 | Cooldown groups | Cooldown groups rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-273 | Cooldown groups | Cooldown groups rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-274 | Cooldown groups | Cooldown groups preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-275 | Cooldown groups | Cooldown groups reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-276 | Cooldown groups | Cooldown groups does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-277 | Cooldown groups | Cooldown groups remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-278 | Cooldown groups | Cooldown groups cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-279 | Cooldown groups | Cooldown groups survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-280 | Cooldown groups | Cooldown groups isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-281 | Cooldown groups | Cooldown groups preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-282 | Cooldown groups | Cooldown groups avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-283 | Cooldown groups | Cooldown groups uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-284 | Cooldown groups | Cooldown groups works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-285 | Cooldown groups | Cooldown groups records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-286 | Cooldown groups | Cooldown groups supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-287 | Cooldown groups | Cooldown groups handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-288 | Cooldown groups | Cooldown groups handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-289 | Cooldown groups | Cooldown groups rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-290 | Cooldown groups | Cooldown groups produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-291 | Cooldown groups | Cooldown groups preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-292 | Cooldown groups | Cooldown groups avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-293 | Cooldown groups | Cooldown groups validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-294 | Cooldown groups | Cooldown groups supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-295 | Cooldown groups | Cooldown groups keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-296 | Cooldown groups | Cooldown groups keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-297 | Cooldown groups | Cooldown groups preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-298 | Cooldown groups | Cooldown groups documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-299 | Cooldown groups | Cooldown groups documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-300 | Cooldown groups | Cooldown groups passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-301 | Casting and commit points | Casting and commit points accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-302 | Casting and commit points | Casting and commit points rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-303 | Casting and commit points | Casting and commit points rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-304 | Casting and commit points | Casting and commit points preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-305 | Casting and commit points | Casting and commit points reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-306 | Casting and commit points | Casting and commit points does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-307 | Casting and commit points | Casting and commit points remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-308 | Casting and commit points | Casting and commit points cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-309 | Casting and commit points | Casting and commit points survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-310 | Casting and commit points | Casting and commit points isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-311 | Casting and commit points | Casting and commit points preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-312 | Casting and commit points | Casting and commit points avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-313 | Casting and commit points | Casting and commit points uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-314 | Casting and commit points | Casting and commit points works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-315 | Casting and commit points | Casting and commit points records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-316 | Casting and commit points | Casting and commit points supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-317 | Casting and commit points | Casting and commit points handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-318 | Casting and commit points | Casting and commit points handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-319 | Casting and commit points | Casting and commit points rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-320 | Casting and commit points | Casting and commit points produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-321 | Casting and commit points | Casting and commit points preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-322 | Casting and commit points | Casting and commit points avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-323 | Casting and commit points | Casting and commit points validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-324 | Casting and commit points | Casting and commit points supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-325 | Casting and commit points | Casting and commit points keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-326 | Casting and commit points | Casting and commit points keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-327 | Casting and commit points | Casting and commit points preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-328 | Casting and commit points | Casting and commit points documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-329 | Casting and commit points | Casting and commit points documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-330 | Casting and commit points | Casting and commit points passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-331 | Interruption and cancellation | Interruption and cancellation accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-332 | Interruption and cancellation | Interruption and cancellation rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-333 | Interruption and cancellation | Interruption and cancellation rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-334 | Interruption and cancellation | Interruption and cancellation preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-335 | Interruption and cancellation | Interruption and cancellation reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-336 | Interruption and cancellation | Interruption and cancellation does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-337 | Interruption and cancellation | Interruption and cancellation remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-338 | Interruption and cancellation | Interruption and cancellation cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-339 | Interruption and cancellation | Interruption and cancellation survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-340 | Interruption and cancellation | Interruption and cancellation isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-341 | Interruption and cancellation | Interruption and cancellation preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-342 | Interruption and cancellation | Interruption and cancellation avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-343 | Interruption and cancellation | Interruption and cancellation uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-344 | Interruption and cancellation | Interruption and cancellation works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-345 | Interruption and cancellation | Interruption and cancellation records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-346 | Interruption and cancellation | Interruption and cancellation supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-347 | Interruption and cancellation | Interruption and cancellation handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-348 | Interruption and cancellation | Interruption and cancellation handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-349 | Interruption and cancellation | Interruption and cancellation rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-350 | Interruption and cancellation | Interruption and cancellation produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-351 | Interruption and cancellation | Interruption and cancellation preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-352 | Interruption and cancellation | Interruption and cancellation avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-353 | Interruption and cancellation | Interruption and cancellation validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-354 | Interruption and cancellation | Interruption and cancellation supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-355 | Interruption and cancellation | Interruption and cancellation keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-356 | Interruption and cancellation | Interruption and cancellation keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-357 | Interruption and cancellation | Interruption and cancellation preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-358 | Interruption and cancellation | Interruption and cancellation documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-359 | Interruption and cancellation | Interruption and cancellation documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-360 | Interruption and cancellation | Interruption and cancellation passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-361 | Channels and recovery | Channels and recovery accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-362 | Channels and recovery | Channels and recovery rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-363 | Channels and recovery | Channels and recovery rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-364 | Channels and recovery | Channels and recovery preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-365 | Channels and recovery | Channels and recovery reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-366 | Channels and recovery | Channels and recovery does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-367 | Channels and recovery | Channels and recovery remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-368 | Channels and recovery | Channels and recovery cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-369 | Channels and recovery | Channels and recovery survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-370 | Channels and recovery | Channels and recovery isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-371 | Channels and recovery | Channels and recovery preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-372 | Channels and recovery | Channels and recovery avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-373 | Channels and recovery | Channels and recovery uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-374 | Channels and recovery | Channels and recovery works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-375 | Channels and recovery | Channels and recovery records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-376 | Channels and recovery | Channels and recovery supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-377 | Channels and recovery | Channels and recovery handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-378 | Channels and recovery | Channels and recovery handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-379 | Channels and recovery | Channels and recovery rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-380 | Channels and recovery | Channels and recovery produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-381 | Channels and recovery | Channels and recovery preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-382 | Channels and recovery | Channels and recovery avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-383 | Channels and recovery | Channels and recovery validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-384 | Channels and recovery | Channels and recovery supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-385 | Channels and recovery | Channels and recovery keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-386 | Channels and recovery | Channels and recovery keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-387 | Channels and recovery | Channels and recovery preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-388 | Channels and recovery | Channels and recovery documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-389 | Channels and recovery | Channels and recovery documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-390 | Channels and recovery | Channels and recovery passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-391 | Effect execution | Effect execution accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-392 | Effect execution | Effect execution rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-393 | Effect execution | Effect execution rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-394 | Effect execution | Effect execution preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-395 | Effect execution | Effect execution reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-396 | Effect execution | Effect execution does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-397 | Effect execution | Effect execution remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-398 | Effect execution | Effect execution cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-399 | Effect execution | Effect execution survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-400 | Effect execution | Effect execution isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-401 | Effect execution | Effect execution preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-402 | Effect execution | Effect execution avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-403 | Effect execution | Effect execution uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-404 | Effect execution | Effect execution works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-405 | Effect execution | Effect execution records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-406 | Effect execution | Effect execution supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-407 | Effect execution | Effect execution handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-408 | Effect execution | Effect execution handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-409 | Effect execution | Effect execution rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-410 | Effect execution | Effect execution produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-411 | Effect execution | Effect execution preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-412 | Effect execution | Effect execution avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-413 | Effect execution | Effect execution validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-414 | Effect execution | Effect execution supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-415 | Effect execution | Effect execution keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-416 | Effect execution | Effect execution keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-417 | Effect execution | Effect execution preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-418 | Effect execution | Effect execution documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-419 | Effect execution | Effect execution documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-420 | Effect execution | Effect execution passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-421 | Concurrency and queues | Concurrency and queues accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-422 | Concurrency and queues | Concurrency and queues rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-423 | Concurrency and queues | Concurrency and queues rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-424 | Concurrency and queues | Concurrency and queues preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-425 | Concurrency and queues | Concurrency and queues reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-426 | Concurrency and queues | Concurrency and queues does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-427 | Concurrency and queues | Concurrency and queues remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-428 | Concurrency and queues | Concurrency and queues cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-429 | Concurrency and queues | Concurrency and queues survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-430 | Concurrency and queues | Concurrency and queues isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-431 | Concurrency and queues | Concurrency and queues preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-432 | Concurrency and queues | Concurrency and queues avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-433 | Concurrency and queues | Concurrency and queues uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-434 | Concurrency and queues | Concurrency and queues works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-435 | Concurrency and queues | Concurrency and queues records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-436 | Concurrency and queues | Concurrency and queues supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-437 | Concurrency and queues | Concurrency and queues handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-438 | Concurrency and queues | Concurrency and queues handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-439 | Concurrency and queues | Concurrency and queues rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-440 | Concurrency and queues | Concurrency and queues produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-441 | Concurrency and queues | Concurrency and queues preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-442 | Concurrency and queues | Concurrency and queues avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-443 | Concurrency and queues | Concurrency and queues validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-444 | Concurrency and queues | Concurrency and queues supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-445 | Concurrency and queues | Concurrency and queues keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-446 | Concurrency and queues | Concurrency and queues keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-447 | Concurrency and queues | Concurrency and queues preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-448 | Concurrency and queues | Concurrency and queues documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-449 | Concurrency and queues | Concurrency and queues documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-450 | Concurrency and queues | Concurrency and queues passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-451 | Events and diagnostics | Events and diagnostics accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-452 | Events and diagnostics | Events and diagnostics rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-453 | Events and diagnostics | Events and diagnostics rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-454 | Events and diagnostics | Events and diagnostics preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-455 | Events and diagnostics | Events and diagnostics reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-456 | Events and diagnostics | Events and diagnostics does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-457 | Events and diagnostics | Events and diagnostics remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-458 | Events and diagnostics | Events and diagnostics cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-459 | Events and diagnostics | Events and diagnostics survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-460 | Events and diagnostics | Events and diagnostics isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-461 | Events and diagnostics | Events and diagnostics preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-462 | Events and diagnostics | Events and diagnostics avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-463 | Events and diagnostics | Events and diagnostics uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-464 | Events and diagnostics | Events and diagnostics works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-465 | Events and diagnostics | Events and diagnostics records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-466 | Events and diagnostics | Events and diagnostics supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-467 | Events and diagnostics | Events and diagnostics handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-468 | Events and diagnostics | Events and diagnostics handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-469 | Events and diagnostics | Events and diagnostics rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-470 | Events and diagnostics | Events and diagnostics produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-471 | Events and diagnostics | Events and diagnostics preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-472 | Events and diagnostics | Events and diagnostics avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-473 | Events and diagnostics | Events and diagnostics validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-474 | Events and diagnostics | Events and diagnostics supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-475 | Events and diagnostics | Events and diagnostics keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-476 | Events and diagnostics | Events and diagnostics keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-477 | Events and diagnostics | Events and diagnostics preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-478 | Events and diagnostics | Events and diagnostics documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-479 | Events and diagnostics | Events and diagnostics documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-480 | Events and diagnostics | Events and diagnostics passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-481 | Persistence and migration | Persistence and migration accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-482 | Persistence and migration | Persistence and migration rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-483 | Persistence and migration | Persistence and migration rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-484 | Persistence and migration | Persistence and migration preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-485 | Persistence and migration | Persistence and migration reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-486 | Persistence and migration | Persistence and migration does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-487 | Persistence and migration | Persistence and migration remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-488 | Persistence and migration | Persistence and migration cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-489 | Persistence and migration | Persistence and migration survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-490 | Persistence and migration | Persistence and migration isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-491 | Persistence and migration | Persistence and migration preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-492 | Persistence and migration | Persistence and migration avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-493 | Persistence and migration | Persistence and migration uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-494 | Persistence and migration | Persistence and migration works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-495 | Persistence and migration | Persistence and migration records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-496 | Persistence and migration | Persistence and migration supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-497 | Persistence and migration | Persistence and migration handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-498 | Persistence and migration | Persistence and migration handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-499 | Persistence and migration | Persistence and migration rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-500 | Persistence and migration | Persistence and migration produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-501 | Persistence and migration | Persistence and migration preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-502 | Persistence and migration | Persistence and migration avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-503 | Persistence and migration | Persistence and migration validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-504 | Persistence and migration | Persistence and migration supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-505 | Persistence and migration | Persistence and migration keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-506 | Persistence and migration | Persistence and migration keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-507 | Persistence and migration | Persistence and migration preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-508 | Persistence and migration | Persistence and migration documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-509 | Persistence and migration | Persistence and migration documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-510 | Persistence and migration | Persistence and migration passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-511 | Multiplayer authority seams | Multiplayer authority seams accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-512 | Multiplayer authority seams | Multiplayer authority seams rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-513 | Multiplayer authority seams | Multiplayer authority seams rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-514 | Multiplayer authority seams | Multiplayer authority seams preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-515 | Multiplayer authority seams | Multiplayer authority seams reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-516 | Multiplayer authority seams | Multiplayer authority seams does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-517 | Multiplayer authority seams | Multiplayer authority seams remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-518 | Multiplayer authority seams | Multiplayer authority seams cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-519 | Multiplayer authority seams | Multiplayer authority seams survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-520 | Multiplayer authority seams | Multiplayer authority seams isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-521 | Multiplayer authority seams | Multiplayer authority seams preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-522 | Multiplayer authority seams | Multiplayer authority seams avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-523 | Multiplayer authority seams | Multiplayer authority seams uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-524 | Multiplayer authority seams | Multiplayer authority seams works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-525 | Multiplayer authority seams | Multiplayer authority seams records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-526 | Multiplayer authority seams | Multiplayer authority seams supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-527 | Multiplayer authority seams | Multiplayer authority seams handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-528 | Multiplayer authority seams | Multiplayer authority seams handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-529 | Multiplayer authority seams | Multiplayer authority seams rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-530 | Multiplayer authority seams | Multiplayer authority seams produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-531 | Multiplayer authority seams | Multiplayer authority seams preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-532 | Multiplayer authority seams | Multiplayer authority seams avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-533 | Multiplayer authority seams | Multiplayer authority seams validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-534 | Multiplayer authority seams | Multiplayer authority seams supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-535 | Multiplayer authority seams | Multiplayer authority seams keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-536 | Multiplayer authority seams | Multiplayer authority seams keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-537 | Multiplayer authority seams | Multiplayer authority seams preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-538 | Multiplayer authority seams | Multiplayer authority seams documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-539 | Multiplayer authority seams | Multiplayer authority seams documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-540 | Multiplayer authority seams | Multiplayer authority seams passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-541 | Performance and bounded work | Performance and bounded work accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-542 | Performance and bounded work | Performance and bounded work rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-543 | Performance and bounded work | Performance and bounded work rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-544 | Performance and bounded work | Performance and bounded work preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-545 | Performance and bounded work | Performance and bounded work reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-546 | Performance and bounded work | Performance and bounded work does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-547 | Performance and bounded work | Performance and bounded work remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-548 | Performance and bounded work | Performance and bounded work cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-549 | Performance and bounded work | Performance and bounded work survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-550 | Performance and bounded work | Performance and bounded work isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-551 | Performance and bounded work | Performance and bounded work preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-552 | Performance and bounded work | Performance and bounded work avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-553 | Performance and bounded work | Performance and bounded work uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-554 | Performance and bounded work | Performance and bounded work works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-555 | Performance and bounded work | Performance and bounded work records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-556 | Performance and bounded work | Performance and bounded work supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-557 | Performance and bounded work | Performance and bounded work handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-558 | Performance and bounded work | Performance and bounded work handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-559 | Performance and bounded work | Performance and bounded work rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-560 | Performance and bounded work | Performance and bounded work produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-561 | Performance and bounded work | Performance and bounded work preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-562 | Performance and bounded work | Performance and bounded work avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-563 | Performance and bounded work | Performance and bounded work validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-564 | Performance and bounded work | Performance and bounded work supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-565 | Performance and bounded work | Performance and bounded work keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-566 | Performance and bounded work | Performance and bounded work keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-567 | Performance and bounded work | Performance and bounded work preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-568 | Performance and bounded work | Performance and bounded work documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-569 | Performance and bounded work | Performance and bounded work documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-570 | Performance and bounded work | Performance and bounded work passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-571 | Packaging, removal, and release | Packaging, removal, and release accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-572 | Packaging, removal, and release | Packaging, removal, and release rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-573 | Packaging, removal, and release | Packaging, removal, and release rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-574 | Packaging, removal, and release | Packaging, removal, and release preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-575 | Packaging, removal, and release | Packaging, removal, and release reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-576 | Packaging, removal, and release | Packaging, removal, and release does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-577 | Packaging, removal, and release | Packaging, removal, and release remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-578 | Packaging, removal, and release | Packaging, removal, and release cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-579 | Packaging, removal, and release | Packaging, removal, and release survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-580 | Packaging, removal, and release | Packaging, removal, and release isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-581 | Packaging, removal, and release | Packaging, removal, and release preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-582 | Packaging, removal, and release | Packaging, removal, and release avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-583 | Packaging, removal, and release | Packaging, removal, and release uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-584 | Packaging, removal, and release | Packaging, removal, and release works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-585 | Packaging, removal, and release | Packaging, removal, and release records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-586 | Packaging, removal, and release | Packaging, removal, and release supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-587 | Packaging, removal, and release | Packaging, removal, and release handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-588 | Packaging, removal, and release | Packaging, removal, and release handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-589 | Packaging, removal, and release | Packaging, removal, and release rejects rollback after the declared commit point. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-590 | Packaging, removal, and release | Packaging, removal, and release produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-591 | Packaging, removal, and release | Packaging, removal, and release preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-592 | Packaging, removal, and release | Packaging, removal, and release avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-593 | Packaging, removal, and release | Packaging, removal, and release validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-594 | Packaging, removal, and release | Packaging, removal, and release supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-595 | Packaging, removal, and release | Packaging, removal, and release keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-596 | Packaging, removal, and release | Packaging, removal, and release keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-597 | Packaging, removal, and release | Packaging, removal, and release preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-598 | Packaging, removal, and release | Packaging, removal, and release documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-599 | Packaging, removal, and release | Packaging, removal, and release documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |
| EABL-T-600 | Packaging, removal, and release | Packaging, removal, and release passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Arcana contract. | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Authority and non-authority boundaries approved.
- [x] MVP and deferred scope separated.
- [x] Commit points and cost transaction semantics defined.
- [x] Provider and bridge seams explicit.
- [x] Standalone Laboratory designed.
- [x] All implementation evidence remains `Not run`.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared dependencies only.
- [ ] Duplicate root rejects before side effects.
- [ ] Definitions remain immutable.
- [ ] Costs, charges, cooldowns, and effects commit exactly once.
- [ ] Cancellation/interruption follows commit rules.
- [ ] Provider failures and stale callbacks are isolated.
- [ ] Editor tooling is separate and repeat-safe.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Laboratory passes without unrelated Echo packages.
- [ ] Samples remove safely.
- [ ] Direct-scene development initialization behaves as documented.
- [ ] Simulated providers prove all MVP lifecycle paths.

### 24.4 Quality gate

- [ ] Automated tests pass.
- [ ] Manual Laboratory checklist passes.
- [ ] No Blocker or Critical defect remains.
- [ ] Performance and bounded-capacity targets are measured.
- [ ] Diagnostics are actionable and privacy-safe.
- [ ] Documentation matches implementation.
- [ ] Current Notes is reconciled.

### 24.5 Distribution gate

- [ ] Manifest and asmdefs are valid.
- [ ] Version/changelog updated.
- [ ] Stable `.meta` files included.
- [ ] Git/local/tarball installation tested as claimed.
- [ ] Removal/reinstall tested.
- [ ] Beta, release-candidate, and stable evidence gates satisfied separately under SFGSS-004.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Hackulos | Planned Fighter/Necromancer abilities and cooldowns | Introduce definitions and simulated providers, then bridge one ability family at a time | Fighter basic attack plus two Necromancer spells pass in isolation and project | Keep project implementation until parity |
| Don't Get Vince'd | Combo/air-kick/invincibility actions | Extract one cooldown/meter action without replacing controller combat timing wholesale | One action retains behavior and feedback | Restore original action path |
| Echo Systems Lab | Weapon firing/ammo/cooldown patterns | Use as evidence for one ability/resource integration | Existing target-range behavior remains stable | Keep original weapon controller |
| Rescuers2D | Role-specific actions | Evaluate only actions that truly benefit from Arcana | No controller capability regression | Preserve existing controller scripts |

### 25.2 Preserve-until-parity rule

Existing project code remains intact until Arcana proves the ability in its Laboratory and then through one reversible project adapter. Content and balance stay project-owned. Replacement proceeds one ability family at a time.

### 25.3 Migration tooling

Future tools may detect candidate cooldowns/charges and generate draft definitions, but they must preview changes and never rewrite project gameplay code automatically. State migration uses stable IDs, aliases, preserved source documents, and explicit rollback.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EABL-R-001 | Ability system becomes a universal gameplay framework | High | High | Enforce strict effect/provider boundaries and deferred modules | Specification review |
| EABL-R-002 | Cross-authority cost/state commit inconsistency | Medium | High | One mutation provider, prepare token, non-failing Arcana state commit after provider commit | Runtime design/tests |
| EABL-R-003 | Hidden reflection/string dispatch | Medium | High | Explicit typed executor/provider registration | API review |
| EABL-R-004 | Interrupted casts duplicate or refund incorrectly | Medium | High | Explicit pre/post-commit policy and idempotent request IDs | Lifecycle tests |
| EABL-R-005 | Targeting drags in camera/input/physics dependencies | High | Medium | Neutral snapshots and separate adapters | Dependency audit |
| EABL-R-006 | Status effects inflate MVP | High | High | Separate later workshop/module | Scope gate |
| EABL-R-007 | Save captures unsafe active state | Medium | High | Durable snapshots exclude activations/effect handles | Migration tests |
| EABL-R-008 | Multiplayer trusts clients | Medium | Critical | Server/host validation through Convergence bridge | Security review |
| EABL-R-009 | Cooldown updates scan all abilities per frame | Medium | Medium | Scheduled/bucketed runtime state and profiling | Performance tests |
| EABL-R-010 | Ability definitions become mutable runtime state | Medium | High | Runtime owner state and immutability tests | Automated tests |
| EABL-R-011 | Effect executor failures leave unclear partial outcomes | Medium | High | Per-step commit/failure policy and result ledger | Effect tests |
| EABL-R-012 | Package split becomes necessary | Low/Medium | Medium | Keep provider/bridge assemblies separate; reassess after implementation | Repository review |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EABL-D-001 | Use one application-session root with owner-scoped state | Approved | Central service and persistence without a global gameplay owner | Root stays narrow; owners remain explicit | No |
| EABL-D-002 | Definitions are immutable; all live state is owner-scoped | Approved | Prevent shared asset contamination | More runtime state models | No |
| EABL-D-003 | One mutation-capable cost provider per MVP activation | Approved | Makes commit semantics tractable | Multi-provider costs deferred | Yes if changed |
| EABL-D-004 | Support commit at cast start or cast completion | Approved | Covers common action and spell patterns | Cancellation semantics must remain explicit | No |
| EABL-D-005 | Post-commit interruption never implies automatic rollback | Approved | External effects may already be irreversible | Projects use compensating effects if desired | No |
| EABL-D-006 | Effects use explicit typed executor registrations | Approved | Avoid reflection and hidden dependencies | More provider boilerplate, safer contracts | No |
| EABL-D-007 | Clash owns instantaneous combat resolution | Approved | Prevent duplicate damage authority | Arcana effect bridge submits Clash requests | No |
| EABL-D-008 | Active activations are never saved | Approved | Coroutines/targets/provider handles are unsafe durable state | Loads resume from stable owner state only | No |
| EABL-D-009 | Server/host is authoritative by default in shared multiplayer | Approved | Prevent client-trusted ability outcomes | Prediction remains presentation/provider-specific | No |
| EABL-D-010 | Status effects are deferred | Approved | Requires its own coherent architecture | Arcana MVP remains achievable | No |

### 27.2 Release-blocking questions

None block the feasibility foundation. Implementation must later choose the exact async primitive and internal scheduler only within the approved contracts, and record any architecture change before code diverges.

### 27.3 Non-blocking later questions

- Whether status effects become an Arcana subpackage or a separate package family.
- Whether passive/reactive abilities share the same runtime or a specialized module.
- Whether more than one mutation provider can be coordinated through a later transaction protocol.
- Whether visual graph authoring is justified after list-based authoring is proven.
- Which Convergence provider adapter first proves authoritative prediction/reconciliation.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Feasibility foundation | Approved provider-neutral contract | This document and boundary record | Approved documentation |
| M1 - Skeleton | Installable package anatomy | Manifest, asmdefs, docs shell | Clean compile |
| M2 - Definitions and owner state | IDs, catalog, grants, loadouts, snapshots | Unit tests |
| M3 - Activation core | Validation, conditions, target snapshots, costs, commit | PlayMode tests |
| M4 - Timing and effects | Cast, interruption, channels, cooldowns, charges, effects | Laboratory paths |
| M5 - Tooling and Laboratory | Setup, validators, simulator, isolated sample | Repeatability and manual evidence |
| M6 - First bridge | One approved project/peer integration | Integration Laboratory |
| M7 - Release | Distribution-ready package | Full SFGSS-004 evidence |

### 28.2 Checkpoint rule

Every milestone is split into small, learning-oriented Checkpoint Build Plans under SFGSS-005. Complete code is shown in conversation with exact paths and explanations when implementation is eventually authorized.

### 28.3 First recommended checkpoint

After SUITE-DOC-33 unlocks implementation: **EABL-M1-01 - Arcana Package Skeleton**, limited to manifest, asmdefs, documentation shell, and clean compilation. It is not currently authorized.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.
Treat SFGSS-000 and SFGSS-002 through SFGSS-005 as suite authorities.
Treat Arcana - EchoAbilities Feasibility Foundation v1.0.0 as the Level 2 authority
for provider-neutral ability definitions, owner state, grants, loadouts, activation,
conditions, costs, charges, cooldowns, casting, interruption, targeting, effects,
persistence, diagnostics, and multiplayer seams.

EchoAbilities does not own resources, health, combat formulas, character identity,
input, UI, animation, VFX, audio, camera, save transport, networking transport,
specific spells/classes, or status-effect semantics. Clash owns instantaneous combat
resolution. One mutation-capable cost provider is allowed per MVP activation.
Post-commit interruption does not imply rollback.

Implementation is locked until SUITE-DOC-33. All empirical evidence is Not run.
Current next package: The Atlas (EchoWorld) feasibility foundation.
When implementation later begins, show complete code and explain every step so Jesse
can enter and understand it himself.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | 1.0.0 feasibility foundation |
| Completed checkpoint | SUITE-DOC-21 |
| Files created | Arcana foundation, feasibility record, audit report, roadmap/README/Current Notes updates, manifest |
| Tests passed | Documentation structure and artifact integrity only |
| Tests failed | None in documentation audit |
| Planned tests | 600, all Not run |
| Known issues | Implementation, measurements, compatibility, adapters, and status-effect design pending |
| Next checkpoint | SUITE-DOC-22 - The Atlas (`EchoWorld`) Feasibility Foundation |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and responsibility are clear.
- [x] Boundaries align with SFGSS-000 and Clash.
- [x] Independence proof is credible.
- [x] MVP is useful without becoming a universal RPG framework.
- [x] Costs, commit points, interruption, and effect boundaries are explicit.
- [x] Targeting and neighboring authorities remain optional.
- [x] Persistence excludes unsafe transient state.
- [x] Multiplayer authority is server/host-first and provider-neutral.
- [x] Laboratory and planned tests are fully registered.
- [x] All implementation evidence remains Not run.
- [x] No Isekai Studios identity or ownership is introduced.

### 30.2 Approval record

**Decision:** Approved feasibility foundation  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Implementation remains locked until SUITE-DOC-33. Status effects, passive abilities, provider-specific prediction, multi-provider resource transactions, and all empirical claims require later design or evidence.

---

## Foundation Completion Rule

This feasibility foundation is complete when a new collaborator can explain:

1. What Arcana owns and refuses to own.
2. How definitions, owners, activations, costs, charges, cooldowns, and effects differ.
3. Where the irreversible commit point occurs.
4. What interruption can and cannot roll back.
5. Why one mutation cost provider is allowed in the MVP.
6. How targeting works without input, camera, or physics dependencies.
7. How Clash, Vault, Fellowship, UI/Input, Chronicle, and Convergence connect optionally.
8. What state is durable and what is always session-only.
9. How the package proves itself in isolation.
10. What evidence remains Not run.
