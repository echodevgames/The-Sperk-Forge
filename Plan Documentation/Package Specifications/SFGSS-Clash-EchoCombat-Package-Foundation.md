# Clash - EchoCombat Feasibility Foundation Specification

**Document ID:** SFGSS-PKG-ECHOCOMBAT  
**Specification version:** 1.0.0  
**Status:** Approved feasibility foundation; EchoCombat remains an Advanced candidate and implementation remains locked  
**Technical package name:** EchoCombat  
**Public title:** Clash - Combat Messages, Targets, and Resolution  
**Package ID:** `com.echodevgames.echo-combat`  
**Runtime namespace:** `EchoDevGames.EchoCombat`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Repository:** Planned `EchoDevGames/EchoCombat`  
**Current Notes:** `../Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Required feasibility record:** `../Research Records/SUITE-DOC-20_EchoCombat_Feasibility_and_Boundary_Record.md`  
**Last updated:** August 4, 2026

> “Clash carries the truth of a hit. The game still decides what the blow means.”

> **Approval rule:** This document approves the Level 2 provider-neutral foundation for EchoCombat boundaries, identities, request and result contracts, relation and targetability seams, resolution stages, transactional receiver application, events, diagnostics, Laboratories, and optional adapters. It does not approve implementation, a universal health/stat component, one damage formula, one faction system, one hit-detection technology, one networking provider, or empirical performance and compatibility claims. Those remain blocked until SUITE-DOC-33 and later implementation evidence.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial feasibility foundation | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved provider-neutral combat requests, targetability, relations, pure modifier resolution, transactional receiver application, hit adapters, events, diagnostics, Laboratory, and explicit genre-neutral boundaries | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** Clash - Combat Messages, Targets, and Resolution  
**Technical identifier:** EchoCombat  
**Flavor line:** Clash carries the truth of a hit.  
**Plain-language subtitle:** A provider-neutral Unity package foundation for damage and healing requests, targetability, combat relations, deterministic resolution, transactional target application, hit results, combat events, diagnostics, and optional 2D/3D or multiplayer adapters.

**One-sentence ownership contract:**

> EchoCombat owns provider-neutral combat requests and results, combat target identity and targetability contracts, team/relation evaluation seams, deterministic modifier and resolution pipelines, transactional receiver-application contracts, combat outcome and defeat events, bounded combat-log data, diagnostics, validation, and optional hit-provider adapters; it does not own a universal health/stat model, character progression, equipment rules, abilities, attack timing, movement, animation, VFX, audio, UI, AI, faction reputation, physics detection, scene loading, save transport, multiplayer transport, respawning, or one genre's damage formula.

### 1.1 Elevator summary

Clash provides the shared language and resolution pipeline beneath combat without turning every game into the same combat game. A source or project system submits an immutable `CombatRequest` describing an intended damage or healing operation. EchoCombat validates source, target, targetability, relation policy, request identity, and magnitude; gathers ordered pure modifiers; produces a proposed resolution; asks the target-owned receiver to prepare and commit the authoritative state change; and only then publishes results, defeat outcomes, and bounded combat events.

The target remains the authority over its mutable resources. A health component, destructible prop, shield, puzzle object, vehicle, boss phase, or RPG stat block may implement the receiver contract. EchoCombat never assumes that “zero health” always means death, that all damage uses floating-point subtraction, or that healing is merely negative damage.

Physics2D, Physics3D, Arcana, Instinct, the Fellowship, the Vault, the Convergence, and project combat code connect through explicit adapters or bridges. The neutral core remains useful for a platformer hazard, shooter projectile, beat-'em-up strike, RPG spell, healing station, destructible object, or tactical simulation.

### 1.2 Why this belongs in The Sperk's Forge

Existing projects repeatedly need damage messages, hit results, target interfaces, team checks, death signals, combat feedback, and debugging. When each project invents these independently, controllers, weapons, enemies, UI, audio, and save code become directly coupled. Damage often mutates a field before validation, friendly-fire rules are scattered across callers, duplicate hits occur through multiple colliders, and multiplayer authority cannot be added without rewriting every attack path.

Clash extracts the neutral operation and evidence flow while leaving combat design in the game. It gives Arcana one way to request an effect, Instinct one way to observe harm, Impact one way to react after commitment, The Path one way to count outcomes, and The Convergence one seam for authoritative validation.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Always paired with “Combat Messages, Targets, and Resolution.” |
| Setup guidance/tooltips | Yes | Must explain requests, targets, receivers, modifiers, and outcomes plainly. |
| Samples | Optional | Verse-flavored combatants may appear but remain replaceable. |
| Runtime API/type names | No lore-only names | Use `CombatRequest`, `ICombatReceiver`, and `CombatResolutionResult`. |
| Project data | No required Verse content | Games own weapons, damage types, factions, stats, visuals, and balance. |

---

## 2. Problem Statement

### 2.1 Current problem

Combat code commonly begins as `target.Health -= damage`. That shortcut hides source identity, relation policy, resistances, invulnerability, duplicate-hit protection, healing, defeat outcomes, feedback, logging, and authority. As systems grow, each weapon or ability reimplements target lookup, friendly-fire checks, critical calculations, resource mutation, death callbacks, and UI/audio effects.

A reusable combat package must not replace that shortcut with a universal RPG formula. It needs a neutral request/result language, explicit target-owned state mutation, deterministic modifier ordering, idempotency, stale-safe handles, and clear seams for physics, abilities, AI, equipment, saves, and networking.

### 2.2 Evidence from current architecture

| Source | Need/finding | Preserve | Improve |
|---|---|---|---|
| SFGSS-000 | Damage/healing messages, damageable/targetable contracts, teams, hit results, modifiers, defeat, combat logs, 2D/3D adapters | Genre-neutral scope | Turn candidate bullets into explicit lifecycle and authority contracts |
| The Fellowship | Durable character identity is separate from runtime actor identity | Stable character and actor identities | Combat targets must not become character definitions or GameObject names |
| The Vault | Equipment storage does not calculate combat statistics | Generic storage boundary | Combat modifiers arrive through bridges/providers, not inventory internals |
| The Hand | Discovery and execution requests are separate from outcomes | Request/result discipline | Interaction hits do not silently become combat damage |
| Impact | Feedback reacts to semantic committed events | Accessibility-aware response | Combat publishes events but never performs camera/audio/VFX itself |
| Instinct | AI observes semantic stimuli and uses server authority in multiplayer | Actor-local decision authority | Combat events may become stimuli without AI becoming the combat resolver |
| The Convergence | Important shared state is authoritative on host/server | Explicit authority checks | Client hit reports are evidence, not automatic authoritative damage |
| The Crucible | Transactions validate before irreversible commit | Transaction and idempotency discipline | Target state mutation occurs through one explicit receiver authority |

### 2.3 Consequences of doing nothing

- Damage and healing formulas remain duplicated across weapons, abilities, enemies, and hazards.
- Friendly-fire and targetability policies disagree between callers.
- Multiple colliders create duplicate damage.
- Modifiers execute in hidden or nondeterministic order.
- Death and defeat are inferred from one field rather than target-owned outcomes.
- Feedback listeners fire before state commits or become required for state changes.
- Save and networking code become entangled with combat presentation.
- Tests cannot explain why one request was accepted, changed, rejected, or replayed.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide immutable, stable-ID combat requests and structured results.
- Keep targetability, relation evaluation, resolution, application, and presentation separate.
- Let the target-owned receiver prepare and commit mutable resource changes.
- Support damage, healing, and project-defined combat operation kinds without treating healing as negative damage.
- Provide deterministic, pure modifier stages and complete resolution explanations.
- Reject stale targets, duplicate requests, replayed requests, and invalid magnitudes safely.
- Publish semantic events only after authoritative state changes.
- Support 2D and 3D hit adapters without placing physics in the core.
- Provide server/host authority seams without a networking SDK dependency.
- Bound histories, queues, idempotency records, modifier counts, targets per batch, and diagnostics.

### 3.2 Non-goals

- Define one health, shield, stamina, posture, durability, or RPG-stat component.
- Dictate one action, shooter, RPG, fighting-game, tactics, or MMO damage formula.
- Own attack animation timing, hitboxes, projectiles, weapons, abilities, status effects, knockback, or hit stop.
- Own faction reputation, social relationships, party membership, or character identity.
- Perform respawning, corpse handling, loot, XP, objective rewards, or save writes.
- Promise cross-target atomicity for area attacks.
- Accept untrusted client hit reports as authoritative multiplayer truth.
- Store arbitrary project metadata in logs or network messages.
- Use reflection-based modifier or receiver discovery.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project | Run the Combat Laboratory with simulated targets and understand every stage |
| Gameplay programmer | Existing weapon or hazard | Submit a request and receive a structured result without rewriting target mutation |
| Systems programmer | RPG/shooter/fighter rules | Add pure modifiers and receiver providers without forking the core |
| Designer | Project-owned definitions | Tune relation, clamp, and resolution profiles without mutable runtime assets |
| Tester | Reproduction case | Inspect request, modifier, commit, outcome, and event evidence by stable IDs |
| Multiplayer engineer | Authoritative host/server model | Validate client evidence and resolve combat on the authority without SDK leakage |

### 3.4 Measurable success criteria

- Clean supported project installation produces zero compile errors.
- Core compiles without Arcana, Instinct, the Fellowship, the Vault, Impact, or the Convergence.
- A simulated receiver proves damage, healing, rejection, modifiers, defeat, replay protection, and logs.
- Equal inputs and provider order produce equal resolution explanations.
- Listener failure never rolls back already committed target state.
- Physics2D and Physics3D adapters remain separately removable.
- Removing EchoCombat leaves project-owned target data intact.
- Every executed claim is represented by SFGSS-004 evidence; all current empirical claims remain `Not run`.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers.
- Gameplay and systems programmers.
- Designers authoring relation and resolution policies.
- QA testers and maintainers.
- Future bridge/provider authors.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| ECLASH-UC-001 | Apply direct damage | Project combat code | Valid source, target, receiver, request ID | Target commits accepted damage and result is published | MVP |
| ECLASH-UC-002 | Apply healing | Project code | Receiver permits healing | Target commits healing without treating it as negative damage | MVP |
| ECLASH-UC-003 | Reject friendly fire | Resolver | Relation policy denies request | No target mutation; structured denial result | MVP |
| ECLASH-UC-004 | Apply resistance | Modifier provider | Target context exposes resistance | Pure modifier changes proposed magnitude deterministically | MVP |
| ECLASH-UC-005 | Publish defeat | Receiver | Commit reports defeated outcome | Defeat event publishes after commit | MVP |
| ECLASH-UC-006 | Resolve 2D hit | Physics2D adapter | Collider maps to target | Adapter emits neutral hit candidate; resolver remains separate | MVP adapter |
| ECLASH-UC-007 | Resolve server-authoritative hit | Convergence bridge | Server validates evidence | Authority submits request and replicates result | Later bridge |
| ECLASH-UC-008 | Feed AI threat | Instinct bridge | Committed hostile result | Semantic observation is published to AI adapter | Later bridge |
| ECLASH-UC-009 | Observe combat for objective | Path bridge | Committed result matches condition | Objective adapter records progress without owning combat | Later bridge |
| ECLASH-UC-010 | Present feedback | Impact/UI adapters | Result committed | Presentation reacts without being required for commit | Later bridge |

### 4.3 Explicitly unsupported use cases

- A universal `HealthComponent` that every project must use.
- Reflection-discovered damage methods or string-named modifier callbacks.
- One package-managed weapon/ability/effect system.
- Automatic client-authoritative damage.
- Network transport, lag compensation, or rollback simulation inside the neutral core.
- Arbitrary cross-target rollback for area damage.
- Saving live attacks, pending requests, collider contacts, or listener subscriptions.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Combat request/result structures and stable identities.
- Combat operation-kind and channel/tag references.
- Target and receiver registration contracts.
- Targetability evaluation and relation-policy seams.
- Resolution context, pure modifier collection, ordering, and explanation.
- Receiver prepare/commit application contract.
- Request replay/idempotency policy.
- Combat outcomes, defeat/recovery event data, and bounded logs.
- Hit-candidate contracts and optional adapter compliance.
- Validation, diagnostics, Laboratory, and package setup design.

### 5.2 The package does not own

- Mutable health, shield, armor, stats, posture, durability, or other resource stores.
- Characters, teams, party rosters, faction reputation, equipment, or abilities.
- Physics queries, projectiles, attack colliders, animation events, or targeting UI.
- Damage-type content, balance, critical-hit rules, elemental formulas, or status effects.
- Respawn, loot, XP, quests, saves, networking transport, camera, audio, VFX, or input.

### 5.3 Neighboring authorities

| Concern | Authority | EchoCombat interaction |
|---|---|---|
| Character identity/ownership | The Fellowship or project | Optional source/target identity bridge |
| Movement and knockback execution | The Vessel/project motor | Consumes committed result or project impulse request |
| Abilities, costs, cooldowns, effects | Arcana/project | Submits combat requests and consumes results |
| Equipment and item state | The Vault/project | Optional modifier/context provider; no direct mutation |
| AI decisions/threat | Instinct/project | Consumes committed combat observations |
| Feedback | Impact, The Eye, Resonance, UI | Reacts to semantic events through bridges |
| Objective progress | The Path | Observes committed results/outcomes |
| Save transport | The Chronicle | Saves target-owned durable state, not live combat requests |
| Multiplayer | The Convergence/provider | Determines authority and replicates evidence/results |
| Scene travel/world identity | The Passage/The Atlas | Provides context only; does not resolve combat |

### 5.4 Boundary tests

A feature belongs in Clash only when it describes a combat request, target eligibility, relation, resolution transformation, target application contract, committed outcome, or diagnostic evidence. If it determines how an attack is animated, moved, aimed, spawned, heard, seen, saved, rewarded, or network transported, it belongs elsewhere or in a bridge.

---

## 6. Independence Contract

### 6.1 Standalone guarantees

The neutral core must:

- Compile with declared Unity dependencies only.
- Operate with simulated targets and providers.
- Require no character, inventory, abilities, AI, feedback, save, UI, or multiplayer package.
- Avoid project assembly references.
- Keep physics adapters outside the core assembly.
- Fail visibly when required receiver/relation/modifier providers are missing.
- Permit explicit injection of service, clock, ID source, and provider registries.
- Avoid static mutable combat state as the only API.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Evidence |
|---|---|---|
| Installed alone | Simulated requests resolve through Laboratory receiver | Not run |
| No relation provider | Configured fallback relation policy applies | Not run |
| No modifier providers | Base request resolves unchanged except clamps | Not run |
| Physics adapters removed | Neutral request API still works | Not run |
| Optional bridge absent | No compile or runtime failure | Not run |
| Duplicate root | Duplicate rejects before subscriptions/registrations | Not run |
| Sample removed | Runtime and Editor assemblies compile | Not run |
| Package removed | Project-owned receiver/resource data remains | Not run |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine/CoreModule | Platform | Yes | Unity 6000.0 planned | MonoBehaviour, ScriptableObject, vectors, lifecycle | Package cannot compile without Unity |
| Unity Test Framework | Test-only | Yes for tests | Verify at implementation | Automated tests | Runtime unaffected when tests absent |
| Physics2D module | Adapter | No | Unity baseline | Optional 2D hit adapter | Core remains usable |
| Physics module | Adapter | No | Unity baseline | Optional 3D hit adapter | Core remains usable |

### 6.4 Forbidden dependencies

- Project assemblies.
- Optional Sperk's Forge packages in the neutral core.
- Networking SDKs, RPG frameworks, physics assets, or ability systems.
- Samples or Editor assemblies at runtime.
- Reflection-based discovery as the default integration mechanism.
- Mutable ScriptableObject state.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
|---|---|---|---|---:|---|
| ECLASH-CAP-001 | Combat requests | Immutable damage/healing/project-kind requests | Approved | Yes | Runtime |
| ECLASH-CAP-002 | Target identity | Stable target handles and snapshots | Approved | Yes | Runtime |
| ECLASH-CAP-003 | Targetability | Structured eligibility evaluation | Approved | Yes | Runtime |
| ECLASH-CAP-004 | Relations | Team/relation provider seam and policy | Approved | Yes | Runtime |
| ECLASH-CAP-005 | Resolution pipeline | Ordered validation, modifiers, clamps, explanation | Approved | Yes | Runtime |
| ECLASH-CAP-006 | Receiver transaction | Prepare/commit target-owned state change | Approved | Yes | Runtime |
| ECLASH-CAP-007 | Idempotency | Bounded replay and duplicate-request protection | Approved | Yes | Runtime |
| ECLASH-CAP-008 | Outcomes | Applied, rejected, defeated, destroyed, recovered, custom | Approved | Yes | Runtime |
| ECLASH-CAP-009 | Events/logs | Semantic committed events and bounded records | Approved | Yes | Runtime |
| ECLASH-CAP-010 | 2D hit adapter | Collider/raycast to neutral candidate | Approved adapter | Yes | Adapter |
| ECLASH-CAP-011 | 3D hit adapter | Collider/raycast to neutral candidate | Approved adapter | Yes | Adapter |
| ECLASH-CAP-012 | Batch causality | Related per-target requests and batch summary | Approved | Yes | Runtime |
| ECLASH-CAP-013 | Editor validation | IDs, profiles, phases, adapters, limits | Approved | Yes | Editor |
| ECLASH-CAP-014 | Combat Laboratory | Simulated receiver/modifiers/hit adapters | Approved | Yes | Sample/Test |
| ECLASH-CAP-015 | Network authority bridge | Server validation and replicated results | Deferred | No | Bridge |
| ECLASH-CAP-016 | Prediction/rollback | Provider-specific prediction reconciliation | Deferred | No | Provider/Bridge |
| ECLASH-CAP-017 | Ongoing status effects | Duration, stacking, ticks, dispels | Deferred to Arcana/project | No | Separate authority |
| ECLASH-CAP-018 | Combat stats framework | Health/armor/attributes/crit formulas | Rejected from core | No | Project/RPG |

### 7.2 MVP capability set

The smallest complete release includes requests, target registration, targetability, relations, deterministic fixed-point magnitude handling, pure modifier stages, receiver prepare/commit, idempotency, outcomes, events, bounded logs, simulated providers, setup/validation, and separate 2D/3D hit adapters with isolated Laboratories.

### 7.3 Later capability set

- Convergence authority and prediction adapters.
- Arcana effect integration.
- Instinct threat/observation bridge.
- Equipment/stat modifier bridges.
- Combat replay visualization from bounded semantic records.
- Provider-specific lag-compensation evidence.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Universal health component | Rejected from core | Conflicts with project/RPG authorities | Optional sample/reference module only |
| One critical-hit formula | Rejected | Genre and project specific | Project modifier provider |
| Status-effect engine | Deferred to Arcana/project | Different lifecycle and persistence authority | Arcana specification review |
| Cross-target atomic area damage | Deferred | Requires distributed transaction semantics | Proven game need and ADR |
| Client-authoritative damage | Rejected | Security risk | Never as default |
| Reflection-discovered damage methods | Rejected | Hidden dependencies and stripping risk | None |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Resolver profiles, operation kinds, channels, relation policies, limits | Runtime targets, health values, request history |
| Runtime state/behavior | Registries, requests, contexts, modifier pipeline, transaction orchestration, events | Editor logic, UI, game-specific stats |
| Presentation/feedback | Optional inspectors, Laboratory readouts, bridge presenters | Authoritative resolution or target mutation |

### 8.2 Component topology

```text
Project attack / Arcana / hazard / network authority
    -> CombatRequest
        -> EchoCombatRoot / ICombatService
            -> request validation and idempotency
            -> targetability and relation evaluation
            -> source/target/context snapshots
            -> ordered pure modifier contributors
            -> CombatResolutionDraft and explanation
            -> ICombatReceiver.Prepare(...)
                -> target-owned application token
                    -> Commit()
                        -> target-owned state changes
            -> CombatResolutionResult
            -> semantic events and bounded combat log

Physics2D / Physics3D / project hit systems
    -> CombatHitCandidate
        -> project request factory
            -> CombatRequest
```

Hit detection intentionally does not apply damage automatically. The project or adapter decides how a hit candidate becomes a request, preserving attack-specific rules and authority.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root required? | One optional application-session `EchoCombatRoot`; direct service injection is also supported |
| Duplicate behavior | Reject before registrations, subscriptions, histories, or side effects |
| Initialization trigger | Explicit initialize or package setup prefab lifecycle |
| Shutdown | Stop accepting requests, cancel only uncommitted work, dispose providers, clear bounded session data |
| Direct-scene behavior | Development initializer may create the configured root only when absent |
| Test seam | `ICombatService`, explicit registries, simulated clock/ID source/receiver/providers |

### 8.4 Lifecycle sequence

1. Claim authority or accept injected service.
2. Validate resolver configuration and bounds.
3. Initialize target, relation, modifier, and diagnostic registries.
4. Register explicit providers and receivers.
5. Accept requests.
6. Validate identity, target, relation, magnitude, replay, and authority.
7. Build snapshots and resolution context.
8. Collect and order pure modifiers.
9. Produce proposed result and explanation.
10. Ask one target receiver to prepare application.
11. Commit exactly once.
12. Publish result, outcome, events, and bounded records.
13. Dispose registrations and session data at shutdown.

### 8.5 Failure model

| Failure | Detection | Result | Fallback | Diagnostic |
|---|---|---|---|---|
| Duplicate root | Claim | Duplicate destroyed/disabled | Existing root retained | ECLASH-001 |
| Missing target | Request validation | Rejected | No mutation | ECLASH-101 |
| Stale target handle | Registry validation | Rejected | Refresh target | ECLASH-102 |
| Relation unavailable | Relation stage | Unavailable/rejected by policy | Configured fallback | ECLASH-201 |
| Modifier exception | Resolution | Failed or provider skipped by declared policy | No commit unless policy permits | ECLASH-301 |
| Invalid magnitude | Validation | Rejected | No mutation | ECLASH-302 |
| Receiver prepare failure | Prepare | Rejected/failed | No commit | ECLASH-401 |
| Receiver commit failure | Commit | Failed | Receiver-owned recovery; no success event | ECLASH-402 |
| Duplicate request | Idempotency | Return prior result or reject | No duplicate commit | ECLASH-501 |
| Listener exception | Event delivery | Logged | Committed result remains valid | ECLASH-601 |
| Unauthorized network caller | Authority bridge | Rejected | Authority may re-evaluate evidence | ECLASH-701 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID | Mutable runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `CombatConfiguration` | Bounds, defaults, policies, history capacities | Yes | No | Yes |
| `CombatOperationKind` | Damage, healing, and project-defined semantic operation | Yes | No | Yes |
| `CombatChannelDefinition` | Semantic channel/tag such as physical, fire, poison | Yes | No | Yes |
| `CombatResolverProfile` | Phase IDs, ordering, clamps, relation policies | Yes | No | Yes |
| `CombatRelationPolicy` | Friendly/self/unknown relation behavior | Yes | No | Yes |
| `CombatModifierProfile` | Optional authored modifier configuration | Yes | No | Yes |

### 9.2 Runtime state

| State | Owner | Lifetime | Reset | Serialization |
|---|---|---|---|---|
| Target registry | Combat service | Session/scene registrations | Dispose/unload | Not saved |
| Provider registry | Combat service | Registration lease | Dispose | Not saved |
| Request context | Combat service | One request | Completion | Not saved |
| Resolution draft/explanation | Combat service | One request | Completion/history bound | Not saved by default |
| Idempotency record | Combat service/authority | Bounded session window | Expiry/reset | Network adapters may map their own durable/replay state |
| Combat event record | Combat service | Bounded session history | Ring-buffer eviction | Optional support export only |
| Mutable health/resources | Target/project authority | Project-defined | Project-defined | Saved by owning system through Chronicle |

### 9.3 Stable identifiers

- `CombatRequestId` identifies one intended authoritative application attempt.
- `CombatCausalityId` links requests from one attack, ability, explosion, or network command.
- `CombatBatchId` groups per-target requests without promising cross-target atomicity.
- `CombatTargetId` identifies a target registration in its declared identity scope.
- `CombatSourceId` is a provider-neutral source identity and must not be inferred from display names.
- `CombatOperationKindId`, `CombatChannelId`, `CombatModifierId`, `CombatProviderId`, and `CombatRelationPolicyId` are stable domain IDs.
- Runtime registration handles include owner and generation to reject stale usage.

### 9.4 Combat magnitude model

The neutral core uses a nonnegative signed-64-bit fixed-point `CombatMagnitude` in project-configured smallest units. Damage and healing remain separate operation kinds; callers do not encode healing as a negative damage number. Profiles define scale, legal range, rounding, and clamp behavior. The core rejects overflow and undefined conversion rather than silently wrapping.

### 9.5 ScriptableObject safety

Definitions hold authored policy only. They never store current health, runtime team membership, active requests, sequential modifier state, idempotency windows, hit contacts, defeat status, or listener references.

### 9.6 Serialization and migration

EchoCombat does not own a live combat save document. Stable authored definitions follow SFGSS-003. Target-owned durable resources and defeated state are serialized by their authority. Any future durable combat-history export requires a separate versioned DTO and privacy review. Unknown operation/channel/modifier records in project data must be preserved or quarantined by the owning serializer.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Ownership |
|---|---|---|---|
| `ICombatService` | Interface | Submit requests, query targets/status, subscribe to semantic events | Root/injected service |
| `CombatRequest` | Immutable struct/record | Source, target, operation, magnitude, IDs, tags, metadata references | Caller |
| `CombatRequestId` | Value type | Idempotency identity | Caller/authority |
| `CombatCausalityId` | Value type | Related-action identity | Caller |
| `CombatTargetHandle` | Generational value type | Stale-safe target reference | Registry |
| `ICombatTarget` | Interface | Identity, targetability, context snapshot, receiver access | Project/adapter |
| `ICombatReceiver` | Interface | Prepare target-owned state application | Target authority |
| `ICombatApplication` | Interface | Commit one prepared target mutation | Receiver |
| `CombatTargetabilityResult` | Struct | Allowed/denied/unavailable with reason | Target/provider |
| `ICombatRelationProvider` | Interface | Resolve source-target relation | Project/bridge |
| `CombatRelationResult` | Struct | Ally/neutral/hostile/unknown/unavailable | Provider |
| `ICombatModifierContributor` | Interface | Add pure ordered modifiers | Project/bridge |
| `CombatModifier` | Immutable struct | Phase, priority, stable ID, transform data | Provider |
| `CombatResolutionContext` | Read-only struct | Request and source/target/relation snapshots | Service |
| `CombatResolutionDraft` | Immutable transform model | Proposed operation and magnitude | Pipeline |
| `CombatResolutionExplanation` | DTO | Ordered stage inputs/outputs/reasons | Service |
| `CombatResolutionResult` | Struct | Final status, applied magnitude, outcome, IDs | Service |
| `CombatOutcome` | Enum/value | None, affected, defeated, destroyed, recovered, custom | Receiver result |
| `CombatHitCandidate` | Struct | Neutral contact evidence | Hit adapter/project |
| `CombatEventRecord` | DTO | Bounded semantic event/log data | Service |

### 10.2 Representative methods and properties

| Member | Purpose | Preconditions | Result/failure | Thread rule |
|---|---|---|---|---|
| `Resolve(in CombatRequest)` | Validate, resolve, prepare, commit, publish | Initialized service; valid IDs | Structured result | Main thread in MVP |
| `TryRegisterTarget(ICombatTarget, out CombatTargetHandle)` | Register explicit target | Unique ID and valid target | Lease/validation result | Main thread |
| `TryGetTarget(CombatTargetHandle, out CombatTargetSnapshot)` | Read target context | Fresh handle | Snapshot or stale failure | Main thread |
| `RegisterRelationProvider(...)` | Add explicit relation provider | Unique provider ID | Disposable registration | Main thread |
| `RegisterModifierContributor(...)` | Add modifier source | Valid phase/priority/ID | Disposable registration | Main thread |
| `GetRecentEvents(CombatEventQuery)` | Query bounded semantic records | Initialized | Immutable snapshot | Main thread |
| `ResetDevelopmentState()` | Clear Laboratory session state | Development only | Structured report | Main thread |

### 10.3 Events and callbacks

| Event | Timing | Payload | Rule |
|---|---|---|---|
| `RequestReceived` | After basic request parsing | Request summary | No target mutation yet |
| `RequestRejected` | After a terminal validation denial | Rejection result | No mutation |
| `ResolutionPrepared` | After pure modifier pipeline | Explanation summary | No mutation yet |
| `CombatCommitted` | After receiver commit succeeds | Final result | Authoritative event |
| `CombatFailed` | On non-committed failure | Failure result | No success event |
| `TargetDefeated` | After receiver reports defeat on commit | Source/target/outcome IDs | Does not respawn/loot |
| `TargetRecovered` | After receiver reports recovery on commit | Target/outcome IDs | Receiver-defined |
| `BatchCompleted` | After all related per-target requests finish | Summary only | No cross-target rollback claim |

Listener exceptions are isolated and logged. They cannot reverse a committed target state and cannot be required for the state change to complete.

### 10.4 Async and cancellation policy

The MVP resolution pipeline is synchronous and main-thread-bound to keep receiver mutation and event ordering deterministic. Callers may schedule or queue requests outside the package, but EchoCombat does not hide an asynchronous transaction behind the core API. Future provider-specific network or jobified extensions require explicit contracts and evidence.

Cancellation is valid only before receiver commit. Once commit succeeds, the result is authoritative and cancellation returns `TooLate`.

### 10.5 API ergonomics

The novice path uses a configured root, one simulated/simple project receiver, and `Resolve`. The advanced path injects `ICombatService`, relation providers, modifier contributors, target adapters, clocks, ID sources, and diagnostic sinks. Convenience access never becomes the only test seam.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install the package.
2. Open **Sperk's Forge > Clash > Setup and Validation**.
3. Create or select a project-owned Combat Configuration.
4. Select relation, magnitude, clamp, replay, and history policies.
5. Preview root, profile, folder, and adapter changes.
6. Apply create-only-safe operations.
7. Open the resolver Laboratory and optional 2D/3D hit Laboratories.
8. Run validation and export a setup report.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeat safe? | Undo/backup | Report |
|---|---|---|---:|---|---|
| Create configuration | Project asset | Nothing existing by default | Yes | Unity Undo | Setup receipt |
| Create root prefab | Project prefab | Selected scene only with approval | Yes | Undo | Scene receipt |
| Add resolver profile | Project asset | Configuration reference | Yes | Undo | Diff |
| Add optional adapter sample | Sample content | No core files | Yes | Package Manager sample import | Import report |
| Repair missing references | Only approved missing references | Selected project assets | Yes | Preview/Undo | Repair report |

### 11.3 Inspectors and windows

| Tool | Purpose | Runtime dependency? |
|---|---|---:|
| Combat Setup and Validation | Create/repair configuration and root | No |
| Resolver Profile Inspector | Phase/order/clamp/replay policy authoring | No |
| Target/Receiver Inspector | Development status and handle diagnostics | No |
| Modifier Trace Viewer | Explain ordered transforms | No |
| Combat Event Viewer | Query bounded semantic records | No |
| Hit Adapter Inspector | Validate 2D/3D mappings | Adapter only |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix? | Safe auto-fix? |
|---|---|---|---:|---:|
| ECLASH-VAL-001 | Missing configuration | Blocker | Yes | Yes, create-only |
| ECLASH-VAL-002 | Duplicate root | Blocker | Guided | No |
| ECLASH-VAL-003 | Duplicate stable ID | Blocker | Guided | No after release |
| ECLASH-VAL-004 | Invalid magnitude scale/range | Error | Yes | No |
| ECLASH-VAL-005 | Unknown modifier phase | Error | Guided | No |
| ECLASH-VAL-006 | Unbounded history/queue | Error | Yes | No |
| ECLASH-VAL-007 | Physics adapter missing module | Error | Remove/declare | No |
| ECLASH-VAL-008 | Runtime references Editor assembly | Blocker | Manual | No |
| ECLASH-VAL-009 | Project receiver mutates definition assets | Error | Guidance | No |
| ECLASH-VAL-010 | Diagnostic export includes forbidden metadata | Blocker | Manual | No |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

- Git URL after a release exists.
- Local package during development.
- Embedded package for package implementation.
- Tarball after distribution evidence.
- Workshop selection after its adapter and compatibility record exist.

All routes are currently planned and `Not run`.

### 12.2 Minimal scene setup

- One `EchoCombatRoot` or injected `ICombatService`.
- One project-owned `CombatConfiguration`.
- At least one explicitly registered target/receiver.
- One caller or Laboratory controller that submits requests.

### 12.3 Boot-scene setup

The root may be initialized by First Light through an explicit startup adapter. EchoCombat does not require First Light and must reject duplicates before registration or event subscriptions.

### 12.4 Direct-scene setup

A development initializer may create the configured root only when none exists. It must identify development initialization, use the same duplicate claim rules, and be removable/disabled in release builds.

### 12.5 Scene isolation rule

The neutral Resolver Laboratory depends only on EchoCombat and redistributable sample assets. Physics2D and Physics3D Laboratories depend only on the core plus their declared Unity modules. Bridge evidence lives in separate Integration Laboratories.

---

## 13. Standalone Test Lab and Samples

### 13.1 Laboratory purpose

The Clash Laboratories prove request validation, targetability, relations, modifiers, transactions, outcomes, hit adapters, logs, bounds, recovery, and package independence without abilities, characters, inventory, AI, feedback, saves, UI, or networking packages.

### 13.2 Required Laboratory contents

- Simulated source and target identities.
- Configurable simulated receiver with health-like integer resource for demonstration only.
- Relation provider controls.
- Pure modifier toggles and ordered trace.
- Damage/healing request controls.
- Duplicate, stale, replay, failure, defeat, and recovery controls.
- Bounded event/log readout.
- Separate 2D and 3D hit candidate fixtures.
- Reset and leak checks.
- Explicit statement that the simulated receiver is sample evidence, not a mandatory health model.

### 13.3 Laboratory acceptance checklist

| Test ID | Group | Action | Type | Status |
|---|---|---|---|---|
| ECLASH-LAB-001 | Authority and lifecycle | Create one combat root and initialize the simulated receiver registry | Manual/automated | Not run |
| ECLASH-LAB-002 | Authority and lifecycle | Introduce a duplicate root before initialization | Manual/automated | Not run |
| ECLASH-LAB-003 | Authority and lifecycle | Introduce a duplicate root after initialization | Manual/automated | Not run |
| ECLASH-LAB-004 | Authority and lifecycle | Disable and re-enable the authoritative root | Manual/automated | Not run |
| ECLASH-LAB-005 | Authority and lifecycle | Unload the scene that supplied a registered combat target | Manual/automated | Not run |
| ECLASH-LAB-006 | Authority and lifecycle | Dispose provider registrations out of order | Manual/automated | Not run |
| ECLASH-LAB-007 | Authority and lifecycle | Reset the Laboratory and confirm bounded state is cleared | Manual/automated | Not run |
| ECLASH-LAB-008 | Targetability and relations | Resolve a hostile target that is currently targetable | Manual/automated | Not run |
| ECLASH-LAB-009 | Targetability and relations | Reject a target that reports unavailable | Manual/automated | Not run |
| ECLASH-LAB-010 | Targetability and relations | Reject a stale target handle | Manual/automated | Not run |
| ECLASH-LAB-011 | Targetability and relations | Reject self-targeting under a deny-self policy | Manual/automated | Not run |
| ECLASH-LAB-012 | Targetability and relations | Allow self-healing under an allow-self policy | Manual/automated | Not run |
| ECLASH-LAB-013 | Targetability and relations | Reject friendly fire through a relation policy | Manual/automated | Not run |
| ECLASH-LAB-014 | Targetability and relations | Resolve an unknown relation using the configured fallback | Manual/automated | Not run |
| ECLASH-LAB-015 | Damage resolution | Resolve unmodified direct damage | Manual/automated | Not run |
| ECLASH-LAB-016 | Damage resolution | Apply one outgoing modifier | Manual/automated | Not run |
| ECLASH-LAB-017 | Damage resolution | Apply one incoming modifier | Manual/automated | Not run |
| ECLASH-LAB-018 | Damage resolution | Apply resistance and vulnerability contributors | Manual/automated | Not run |
| ECLASH-LAB-019 | Damage resolution | Apply deterministic modifier ordering with equal priorities | Manual/automated | Not run |
| ECLASH-LAB-020 | Damage resolution | Clamp a negative final damage value to zero | Manual/automated | Not run |
| ECLASH-LAB-021 | Damage resolution | Reject an overflowing or invalid fixed-point magnitude | Manual/automated | Not run |
| ECLASH-LAB-022 | Healing resolution | Resolve basic healing | Manual/automated | Not run |
| ECLASH-LAB-023 | Healing resolution | Clamp healing to the receiver maximum | Manual/automated | Not run |
| ECLASH-LAB-024 | Healing resolution | Reject healing on a receiver that forbids healing | Manual/automated | Not run |
| ECLASH-LAB-025 | Healing resolution | Allow a receiver-defined revive result | Manual/automated | Not run |
| ECLASH-LAB-026 | Healing resolution | Reject a revive when the receiver does not support it | Manual/automated | Not run |
| ECLASH-LAB-027 | Healing resolution | Apply outgoing and incoming healing modifiers | Manual/automated | Not run |
| ECLASH-LAB-028 | Healing resolution | Preserve the distinction between healing and negative damage | Manual/automated | Not run |
| ECLASH-LAB-029 | Transactions and idempotency | Prepare and commit a receiver transaction exactly once | Manual/automated | Not run |
| ECLASH-LAB-030 | Transactions and idempotency | Reject a duplicate request ID after a successful commit | Manual/automated | Not run |
| ECLASH-LAB-031 | Transactions and idempotency | Retry a previously rejected request after the rejection TTL expires | Manual/automated | Not run |
| ECLASH-LAB-032 | Transactions and idempotency | Fail before commit and confirm no combat event is published | Manual/automated | Not run |
| ECLASH-LAB-033 | Transactions and idempotency | Fail during receiver commit and confirm no success event is published | Manual/automated | Not run |
| ECLASH-LAB-034 | Transactions and idempotency | Reject a stale prepared application token | Manual/automated | Not run |
| ECLASH-LAB-035 | Transactions and idempotency | Bound the idempotency record history under sustained requests | Manual/automated | Not run |
| ECLASH-LAB-036 | Defeat and lifecycle outcomes | Publish a defeat event after the receiver commits defeat | Manual/automated | Not run |
| ECLASH-LAB-037 | Defeat and lifecycle outcomes | Publish a recovery event after receiver-defined recovery | Manual/automated | Not run |
| ECLASH-LAB-038 | Defeat and lifecycle outcomes | Do not infer death from a numeric value alone | Manual/automated | Not run |
| ECLASH-LAB-039 | Defeat and lifecycle outcomes | Preserve receiver-specific defeated versus destroyed outcomes | Manual/automated | Not run |
| ECLASH-LAB-040 | Defeat and lifecycle outcomes | Reject further damage when the receiver closes targetability | Manual/automated | Not run |
| ECLASH-LAB-041 | Defeat and lifecycle outcomes | Allow post-defeat healing only when the receiver permits it | Manual/automated | Not run |
| ECLASH-LAB-042 | Defeat and lifecycle outcomes | Confirm respawn is not performed by EchoCombat | Manual/automated | Not run |
| ECLASH-LAB-043 | Hit adapters | Convert a 2D collision candidate into a neutral combat hit | Manual/automated | Not run |
| ECLASH-LAB-044 | Hit adapters | Convert a 3D raycast candidate into a neutral combat hit | Manual/automated | Not run |
| ECLASH-LAB-045 | Hit adapters | Deduplicate multiple colliders belonging to one target | Manual/automated | Not run |
| ECLASH-LAB-046 | Hit adapters | Order multiple hit candidates deterministically | Manual/automated | Not run |
| ECLASH-LAB-047 | Hit adapters | Reject a collider without a target adapter | Manual/automated | Not run |
| ECLASH-LAB-048 | Hit adapters | Preserve hit point, normal, direction, and causal IDs | Manual/automated | Not run |
| ECLASH-LAB-049 | Hit adapters | Confirm hit detection does not apply damage by itself | Manual/automated | Not run |
| ECLASH-LAB-050 | Batches and area effects | Resolve several targets under one causal batch ID | Manual/automated | Not run |
| ECLASH-LAB-051 | Batches and area effects | Allow one target to fail without rolling back other targets | Manual/automated | Not run |
| ECLASH-LAB-052 | Batches and area effects | Deduplicate one target found by multiple overlap results | Manual/automated | Not run |
| ECLASH-LAB-053 | Batches and area effects | Apply a per-target relation policy in one batch | Manual/automated | Not run |
| ECLASH-LAB-054 | Batches and area effects | Publish one batch summary after all target requests finish | Manual/automated | Not run |
| ECLASH-LAB-055 | Batches and area effects | Cancel an uncommitted queued target request | Manual/automated | Not run |
| ECLASH-LAB-056 | Batches and area effects | Confirm cross-target atomicity is not promised | Manual/automated | Not run |
| ECLASH-LAB-057 | Events and logging | Publish request-received, committed, and completed events in order | Manual/automated | Not run |
| ECLASH-LAB-058 | Events and logging | Publish a rejected result with a stable diagnostic code | Manual/automated | Not run |
| ECLASH-LAB-059 | Events and logging | Redact project metadata from a support snapshot | Manual/automated | Not run |
| ECLASH-LAB-060 | Events and logging | Bound the combat-event ring buffer | Manual/automated | Not run |
| ECLASH-LAB-061 | Events and logging | Filter combat logs by source, target, kind, tag, and outcome | Manual/automated | Not run |
| ECLASH-LAB-062 | Events and logging | Reject a listener exception without undoing committed combat | Manual/automated | Not run |
| ECLASH-LAB-063 | Events and logging | Confirm display names and localized text are absent from core logs | Manual/automated | Not run |
| ECLASH-LAB-064 | Multiplayer authority | Reject a client-authored authoritative request under server-only policy | Manual/automated | Not run |
| ECLASH-LAB-065 | Multiplayer authority | Accept an authoritative server request | Manual/automated | Not run |
| ECLASH-LAB-066 | Multiplayer authority | Preserve request and causality IDs across a simulated network adapter | Manual/automated | Not run |
| ECLASH-LAB-067 | Multiplayer authority | Reject a replayed authoritative request | Manual/automated | Not run |
| ECLASH-LAB-068 | Multiplayer authority | Separate prediction presentation from authoritative commit | Manual/automated | Not run |
| ECLASH-LAB-069 | Multiplayer authority | Reconcile a predicted hit against an authoritative result | Manual/automated | Not run |
| ECLASH-LAB-070 | Multiplayer authority | Confirm no networking SDK is required by the core | Manual/automated | Not run |
| ECLASH-LAB-071 | Persistence and removal | Confirm live combat requests are not exported as save state | Manual/automated | Not run |
| ECLASH-LAB-072 | Persistence and removal | Remove a modifier provider and preserve unrelated target state | Manual/automated | Not run |
| ECLASH-LAB-073 | Persistence and removal | Remove EchoCombat and retain project-owned health data | Manual/automated | Not run |
| ECLASH-LAB-074 | Persistence and removal | Reinstall EchoCombat and re-register project receivers | Manual/automated | Not run |
| ECLASH-LAB-075 | Persistence and removal | Preserve unknown durable receiver data outside EchoCombat | Manual/automated | Not run |
| ECLASH-LAB-076 | Persistence and removal | Reject importing an unsupported combat snapshot because none is defined | Manual/automated | Not run |
| ECLASH-LAB-077 | Persistence and removal | Confirm Chronicle owns save transport | Manual/automated | Not run |
| ECLASH-LAB-078 | Stress and recovery | Resolve the configured maximum requests in one frame budget | Manual/automated | Not run |
| ECLASH-LAB-079 | Stress and recovery | Reject requests beyond the bounded queue limit | Manual/automated | Not run |
| ECLASH-LAB-080 | Stress and recovery | Recover after one modifier provider throws an exception | Manual/automated | Not run |
| ECLASH-LAB-081 | Stress and recovery | Recover after one relation provider becomes unavailable | Manual/automated | Not run |
| ECLASH-LAB-082 | Stress and recovery | Recover after one receiver unregisters during validation | Manual/automated | Not run |
| ECLASH-LAB-083 | Stress and recovery | Detect leaked registrations at Laboratory reset | Manual/automated | Not run |
| ECLASH-LAB-084 | Stress and recovery | Export a final bounded diagnostic snapshot | Manual/automated | Not run |

### 13.4 Optional integration samples

| Sample | Packages | Purpose | Why not standalone proof |
|---|---|---|---|
| Clash + Impact | EchoCombat, EchoFeedback | Turn committed result into feedback request | Requires both authorities |
| Clash + Instinct | EchoCombat, EchoAI | Convert harm into observation/threat context | Tests bridge semantics |
| Clash + Arcana | EchoCombat, EchoAbilities | Ability requests damage/healing | Arcana owns activation/effects |
| Clash + Convergence | EchoCombat, EchoMultiplayer provider | Server-authoritative request validation | Requires selected provider adapter |
| Clash + Fellowship/Vessel | Characters, Controllers, Combat | Actor identity and movement reaction | Does not prove combat core alone |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

EchoCombat is nonvisual. It exposes semantic results, explanations, event records, and diagnostic snapshots. Production HUD, damage numbers, health bars, combat text, hit flashes, camera response, rumble, sound, and animation belong to UI, Impact, The Eye, Resonance, animation/project code, or bridges.

### 14.2 Required diagnostic states

- Uninitialized
- Ready
- Resolving
- Rejected
- Failed before commit
- Committed
- Target unavailable
- Provider unavailable
- Duplicate/replayed
- Degraded diagnostics

### 14.3 Accessibility requirements

- Combat truth must not depend on color, shake, flash, rumble, or audio.
- Results expose semantic severity and operation kind for accessible presentation.
- Impact and UI bridges must honor reduced-motion, flash, haptic, and text settings.
- Logs use stable IDs and plain-language explanations.

### 14.4 Visual customization

All visuals are project-owned. Sample readouts remain plain, replaceable, and removable.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Root/registry state | API/Inspector | Editor/Development | Low |
| Request result | API/Event | Runtime | Low |
| Resolution explanation | Optional bounded trace | Development/configurable | Medium |
| Provider health | API/Inspector | Editor/Development | Low |
| Event ring buffer | API/Window | Development | Bounded |
| Support snapshot | Explicit export | Development | On demand |

### 15.2 Structured status

Status includes package version, root identity, configuration ID, target/provider counts, active resolution count, queue/idempotency bounds, recent rejection/failure counts, dropped diagnostic count, and last committed result summary. It excludes project-authored display names, arbitrary metadata, private network information, and complete gameplay histories by default.

### 15.3 Diagnostic codes

| Range | Meaning |
|---|---|
| `ECLASH-001–099` | Root/lifecycle/configuration |
| `ECLASH-100–199` | Target/identity/targetability |
| `ECLASH-200–299` | Relations/authority |
| `ECLASH-300–399` | Magnitude/modifier/resolution |
| `ECLASH-400–499` | Receiver transaction/application |
| `ECLASH-500–599` | Idempotency/batches/queues |
| `ECLASH-600–699` | Events/logging/diagnostics |
| `ECLASH-700–799` | Multiplayer/security adapters |
| `ECLASH-800–899` | Physics hit adapters |
| `ECLASH-900–999` | Setup/migration/removal |

### 15.4 Observatory bridge

A separate bridge exposes status, counts, provider health, recent outcomes, resolution timing, queue pressure, and redacted event summaries. EchoCombat never requires the Observatory.

### 15.5 Logging policy

- Stable package-qualified codes.
- No per-frame spam.
- No full arbitrary metadata dumps.
- No credentials, auth tokens, private session IDs, localized text, or account data.
- Development traces are bounded and opt-in.
- Committed results remain valid if logging fails.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Combat definitions/configuration | Project asset | Project | Asset | Unity assets |
| Target resources/defeat state | Project target authority | Project/save participant | Project decision | Chronicle or project backend |
| Active requests/resolution contexts | Session | EchoCombat | No | None |
| Target/provider registrations | Session/scene | EchoCombat | No | None |
| Idempotency records | Session/authority window | EchoCombat/network authority | No by core | Provider-specific if required |
| Event/log history | Bounded diagnostic session | EchoCombat | No by default | Explicit support export only |

### 16.2 Standalone behavior

Without Chronicle, EchoCombat functions fully for the current session. It never chooses a save filename or silently persists combat state.

### 16.3 Optional participant/provider contract

The target-owned resource/stat system contributes its own versioned state to Chronicle. EchoCombat may provide semantic source information or stable operation IDs, but it does not serialize the target's resource internals. A future integration specification may define aliases for renamed damage channels or operation kinds.

### 16.4 Failure and recovery

Loading project target state may produce unavailable definitions, old channels, or invalid values. The owning participant migrates or quarantines that data. EchoCombat validates current runtime registrations after load and does not restore stale target handles, pending attacks, physics contacts, or listener state.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Peers connect through explicit bridges, project adapters, or provider registrations. Installing another package never silently changes the core resolver.

### 17.2 Planned integrations

| Authority | Connection | Bridge owner | Direction | Data/events | Required? |
|---|---|---|---|---|---:|
| Arcana | Ability effect to combat request | Separate bridge/project | Arcana -> Clash | Source/target, operation, magnitude, causality | No |
| Instinct | Combat outcome to observation/threat | Separate bridge | Clash -> Instinct | Committed semantic event | No |
| Fellowship | Character/source identity adapter | Separate bridge/project | Fellowship -> Clash | Durable/session identities | No |
| Vessel | Reaction/knockback request | Project/bridge | Clash -> Vessel | Committed hit intent, not direct physics mutation | No |
| Vault/RPG Foundation | Modifier/context provider | Separate bridge | Equipment/stats -> Clash | Read-only snapshot/modifiers | No |
| Impact | Feedback request | Separate bridge | Clash -> Impact | Semantic outcome and intensity | No |
| Path | Progress observation | Separate bridge | Clash -> Path | Committed result/outcome | No |
| Chronicle | Target-owned save participant | Project/bridge | Target authority -> Chronicle | Durable resource state | No |
| Convergence | Authority, replay, replication | Separate bridge/provider | Bidirectional | Evidence, requests, results, causality | No |
| Looking Glass/Resonance/Eye | Presentation | Separate bridges | Clash -> presentation | Semantic committed event | No |

### 17.3 Bridge placement decision

Two-package integrations ship separately when they reference both packages. Physics2D and Physics3D adapters may ship as first-party optional assemblies/packages under EchoCombat because they depend only on Unity modules and the core. Game-specific formulas and target receivers remain project code or genre packages.

### 17.4 Integration failure behavior

- Missing bridge: core remains unchanged.
- Version mismatch: bridge fails explicitly and does not register.
- Provider unavailable: structured unavailable/rejected result.
- Teardown: bridge unregisters before either connected authority.
- Network disconnect: authority adapter determines retry/reconciliation; core does not invent transport behavior.
- Presentation failure: committed combat remains committed.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Planned target | Fixture | Evidence |
|---|---|---|---|
| Resolver allocation after warmup | Zero avoidable managed allocations on standard happy path | Resolver stress fixture | Not run |
| Request throughput | Declare only after measured target platform evidence | Simulated receivers/modifiers | Not run |
| Modifier count | Configured hard maximum per request | Modifier stress fixture | Not run |
| Targets per batch | Configured hard maximum | Area-request fixture | Not run |
| History/idempotency memory | Bounded by configuration | Long-session fixture | Not run |
| Physics adapter cost | Measured separately from resolver | 2D/3D hit fixtures | Not run |

### 18.2 Allocation policy

- No LINQ, reflection, or unbounded collection growth in hot paths.
- Reuse bounded buffers where evidence supports it.
- Immutable public results may use pooled internal builders but cannot expose mutable pooled storage.
- Explanation detail is configurable and may be reduced in release builds.
- Provider registration is explicit and cached.

### 18.3 Scene and domain reload behavior

Targets and providers unregister cleanly. Static caches reset according to supported Enter Play Mode settings. Duplicate roots reject before side effects. Domain reload and disabled-domain-reload behavior require separate evidence.

### 18.4 Scalability limits

Every capacity is explicit: targets, providers, modifiers per request, active resolutions, queued requests, batch targets, event records, explanation steps, idempotency records, and support-export size. Exceeding a limit produces a structured result rather than silent growth.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Combat data may reveal player/account or encounter behavior when bridged to multiplayer or analytics. The neutral core stores only stable IDs and bounded semantic records needed for debugging. Arbitrary project metadata is opt-in and must be redacted before support export.

### 19.2 Trust boundaries

- Client hit reports are untrusted evidence until the selected authority validates them.
- Request IDs and causality IDs do not authenticate a caller.
- Target receivers validate their own mutable-state invariants.
- Modifier providers cannot directly mutate target state during pure resolution.
- Imported definitions/configurations validate IDs, ranges, and phase references.
- Logs and snapshots exclude credentials and provider secrets.

### 19.3 Platform behavior

| Platform | Foundation status | Special concern | Evidence |
|---|---|---|---|
| Windows | Planned | Baseline development | Not run |
| macOS | Planned | IL2CPP/Editor differences | Not run |
| Linux | Planned | Headless/server use | Not run |
| WebGL | Conditional | Main-thread and memory constraints | Not run |
| Mobile | Conditional | Allocation and physics cost | Not run |
| Console | Unknown/planned | Platform certification and provider rules | Not run |
| Dedicated server | Conditional | No presentation dependencies; authority integration | Not run |

---

## 20. Package and Repository Structure

### 20.1 Proposed package anatomy

```text
Packages/com.echodevgames.echo-combat/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
├── Runtime/
│   ├── Core/
│   ├── Configuration/
│   ├── Identity/
│   ├── Targets/
│   ├── Relations/
│   ├── Resolution/
│   ├── Modifiers/
│   ├── Transactions/
│   ├── Events/
│   ├── Diagnostics/
│   └── EchoDevGames.EchoCombat.Runtime.asmdef
├── Adapters/
│   ├── Physics2D/
│   │   └── EchoDevGames.EchoCombat.Physics2D.asmdef
│   └── Physics3D/
│       └── EchoDevGames.EchoCombat.Physics3D.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Inspectors/
│   ├── Diagnostics/
│   └── EchoDevGames.EchoCombat.Editor.asmdef
├── Samples~/
│   ├── Clash Resolver Laboratory/
│   ├── Clash 2D Hit Laboratory/
│   └── Clash 3D Hit Laboratory/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Assembly definitions

| Assembly | Platform | References | Auto referenced | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoCombat.Runtime` | Runtime | Unity Core only | Yes initially; reconcile at implementation | Neutral contracts/resolver |
| `EchoDevGames.EchoCombat.Physics2D` | Runtime optional | Core, Physics2D | No | 2D hit adapter |
| `EchoDevGames.EchoCombat.Physics3D` | Runtime optional | Core, Physics | No | 3D hit adapter |
| `EchoDevGames.EchoCombat.Editor` | Editor | Core, UnityEditor | No | Setup/validation/tools |
| `EchoDevGames.EchoCombat.Tests.Editor` | Editor tests | Core, Editor, Test Framework | No | Pure/editor tests |
| `EchoDevGames.EchoCombat.Tests.Runtime` | Runtime tests | Core, Test Framework | No | PlayMode/lifecycle tests |

### 20.3 Repository files

README, routed documentation, architecture, API guide, receiver/modifier guide, hit-adapter guide, diagnostics codes, Laboratory guide, migration/removal guide, Current Notes, ADRs, test registry, release checklist, license, notices, and stable `.meta` files.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | Planned 6000.0 | 6000.3.8f1 development baseline | Actual support evidence Not run |
| Physics2D | Unity baseline | Not run | Optional adapter |
| Physics3D | Unity baseline | Not run | Optional adapter |
| Sperk's Forge peers | None in core | Not run | Bridges version separately |

### 21.2 Semantic versioning policy

- Patch: compatible fixes, diagnostics, docs, validator improvements.
- Minor: additive request/result fields with safe defaults, new optional adapters/providers.
- Major: changed request semantics, modifier ordering, fixed-point scale behavior, receiver transaction contract, stable ID format, or event timing.

### 21.3 Deprecation policy

Public fields, IDs, phases, diagnostic codes, and adapter contracts receive documented replacement, migration guidance, warnings, and at least one supported release window before removal unless a security defect requires faster action.

### 21.4 GUID and asset compatibility

Public scripts, definitions, configuration templates, prefabs, and samples preserve `.meta` identities. Domain stable IDs follow SFGSS-003 aliases/tombstones and must not rely on Unity asset GUIDs at runtime.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview and authority boundaries.
- Installation and five-minute resolver setup.
- Project receiver guide.
- Requests, operation kinds, channels, and fixed-point magnitude guide.
- Relations and targetability guide.
- Modifier and explanation guide.
- Hit-adapter guide.
- Laboratory guide.
- Troubleshooting/diagnostic codes.
- Migration/removal/known limitations.

### 22.2 Required developer documentation

- Architecture and lifecycle.
- Receiver transaction contract.
- Idempotency and authority model.
- Modifier purity and ordering.
- Event timing and listener failure policy.
- Bridge/provider authoring.
- Testing and performance strategy.
- Security and multiplayer boundaries.
- Current Notes and checkpoint status.

### 22.3 Documentation truth rule

Examples must compile against the documented release. No performance, platform, provider, compatibility, or release claim may advance from `Not run` without SFGSS-004 evidence.

### 22.4 Living repository workflow

All documents live beside development in Git and open directly in Obsidian. Discoveries begin in Current Notes and are promoted into the specification, ADR, issue, test, guide, or release record at checkpoint closeout.

### 22.5 Handoff scan order

README, SFGSS-000, SFGSS-002 through SFGSS-005, this foundation, its feasibility/boundary record, Current Notes, roadmap, audit report, implementation/tests when they later exist.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Required before release? |
|---|---|---:|
| EditMode unit | Magnitudes, IDs, targetability, relations, modifiers, explanations, policies | Yes |
| PlayMode unit/integration | Root, registrations, receivers, events, scene lifecycle | Yes |
| Resolver Laboratory | Visible core request-to-commit loop | Yes |
| 2D/3D Laboratories | Hit candidate adapters | When adapter ships |
| Bridge Integration Laboratory | Optional peer/provider connection | When bridge ships |
| Clean-project install | Missing dependency/removal proof | Yes |
| Real-project adoption | One narrow combat integration | Before adoption claim |

### 23.2 Required categories

Happy path, missing/invalid configuration, duplicate roots, stale handles, targetability, relations, fixed-point range, modifiers, receiver transactions, idempotency, defeat/recovery, listeners, batches, hit adapters, scene unload, direct-scene entry, removal/reinstall, privacy/security, multiplayer authority, performance bounds, platform/build, migration, docs, and release gates.

### 23.3 Test case registry

| Test ID | Category | Requirement | Setup | Action | Expected result | Status |
|---|---|---|---|---|---|---|
| ECLASH-T-001 | Authority and lifecycle | Authority and lifecycle accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-002 | Authority and lifecycle | Authority and lifecycle rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-003 | Authority and lifecycle | Authority and lifecycle rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-004 | Authority and lifecycle | Authority and lifecycle preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-005 | Authority and lifecycle | Authority and lifecycle reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-006 | Authority and lifecycle | Authority and lifecycle does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-007 | Authority and lifecycle | Authority and lifecycle remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-008 | Authority and lifecycle | Authority and lifecycle cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-009 | Authority and lifecycle | Authority and lifecycle survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-010 | Authority and lifecycle | Authority and lifecycle isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-011 | Authority and lifecycle | Authority and lifecycle preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-012 | Authority and lifecycle | Authority and lifecycle avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-013 | Authority and lifecycle | Authority and lifecycle uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-014 | Authority and lifecycle | Authority and lifecycle works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-015 | Authority and lifecycle | Authority and lifecycle records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-016 | Authority and lifecycle | Authority and lifecycle supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-017 | Authority and lifecycle | Authority and lifecycle handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-018 | Authority and lifecycle | Authority and lifecycle handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-019 | Authority and lifecycle | Authority and lifecycle rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-020 | Authority and lifecycle | Authority and lifecycle produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-021 | Authority and lifecycle | Authority and lifecycle preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-022 | Authority and lifecycle | Authority and lifecycle avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-023 | Authority and lifecycle | Authority and lifecycle validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-024 | Authority and lifecycle | Authority and lifecycle supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-025 | Authority and lifecycle | Authority and lifecycle keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-026 | Authority and lifecycle | Authority and lifecycle keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-027 | Authority and lifecycle | Authority and lifecycle preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-028 | Authority and lifecycle | Authority and lifecycle documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-029 | Authority and lifecycle | Authority and lifecycle documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-030 | Authority and lifecycle | Authority and lifecycle passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-031 | Stable identity and handles | Stable identity and handles accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-032 | Stable identity and handles | Stable identity and handles rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-033 | Stable identity and handles | Stable identity and handles rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-034 | Stable identity and handles | Stable identity and handles preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-035 | Stable identity and handles | Stable identity and handles reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-036 | Stable identity and handles | Stable identity and handles does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-037 | Stable identity and handles | Stable identity and handles remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-038 | Stable identity and handles | Stable identity and handles cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-039 | Stable identity and handles | Stable identity and handles survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-040 | Stable identity and handles | Stable identity and handles isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-041 | Stable identity and handles | Stable identity and handles preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-042 | Stable identity and handles | Stable identity and handles avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-043 | Stable identity and handles | Stable identity and handles uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-044 | Stable identity and handles | Stable identity and handles works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-045 | Stable identity and handles | Stable identity and handles records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-046 | Stable identity and handles | Stable identity and handles supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-047 | Stable identity and handles | Stable identity and handles handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-048 | Stable identity and handles | Stable identity and handles handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-049 | Stable identity and handles | Stable identity and handles rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-050 | Stable identity and handles | Stable identity and handles produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-051 | Stable identity and handles | Stable identity and handles preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-052 | Stable identity and handles | Stable identity and handles avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-053 | Stable identity and handles | Stable identity and handles validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-054 | Stable identity and handles | Stable identity and handles supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-055 | Stable identity and handles | Stable identity and handles keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-056 | Stable identity and handles | Stable identity and handles keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-057 | Stable identity and handles | Stable identity and handles preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-058 | Stable identity and handles | Stable identity and handles documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-059 | Stable identity and handles | Stable identity and handles documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-060 | Stable identity and handles | Stable identity and handles passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-061 | Targetability | Targetability accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-062 | Targetability | Targetability rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-063 | Targetability | Targetability rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-064 | Targetability | Targetability preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-065 | Targetability | Targetability reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-066 | Targetability | Targetability does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-067 | Targetability | Targetability remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-068 | Targetability | Targetability cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-069 | Targetability | Targetability survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-070 | Targetability | Targetability isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-071 | Targetability | Targetability preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-072 | Targetability | Targetability avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-073 | Targetability | Targetability uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-074 | Targetability | Targetability works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-075 | Targetability | Targetability records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-076 | Targetability | Targetability supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-077 | Targetability | Targetability handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-078 | Targetability | Targetability handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-079 | Targetability | Targetability rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-080 | Targetability | Targetability produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-081 | Targetability | Targetability preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-082 | Targetability | Targetability avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-083 | Targetability | Targetability validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-084 | Targetability | Targetability supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-085 | Targetability | Targetability keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-086 | Targetability | Targetability keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-087 | Targetability | Targetability preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-088 | Targetability | Targetability documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-089 | Targetability | Targetability documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-090 | Targetability | Targetability passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-091 | Teams and relations | Teams and relations accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-092 | Teams and relations | Teams and relations rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-093 | Teams and relations | Teams and relations rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-094 | Teams and relations | Teams and relations preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-095 | Teams and relations | Teams and relations reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-096 | Teams and relations | Teams and relations does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-097 | Teams and relations | Teams and relations remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-098 | Teams and relations | Teams and relations cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-099 | Teams and relations | Teams and relations survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-100 | Teams and relations | Teams and relations isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-101 | Teams and relations | Teams and relations preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-102 | Teams and relations | Teams and relations avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-103 | Teams and relations | Teams and relations uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-104 | Teams and relations | Teams and relations works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-105 | Teams and relations | Teams and relations records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-106 | Teams and relations | Teams and relations supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-107 | Teams and relations | Teams and relations handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-108 | Teams and relations | Teams and relations handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-109 | Teams and relations | Teams and relations rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-110 | Teams and relations | Teams and relations produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-111 | Teams and relations | Teams and relations preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-112 | Teams and relations | Teams and relations avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-113 | Teams and relations | Teams and relations validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-114 | Teams and relations | Teams and relations supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-115 | Teams and relations | Teams and relations keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-116 | Teams and relations | Teams and relations keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-117 | Teams and relations | Teams and relations preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-118 | Teams and relations | Teams and relations documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-119 | Teams and relations | Teams and relations documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-120 | Teams and relations | Teams and relations passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-121 | Request validation | Request validation accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-122 | Request validation | Request validation rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-123 | Request validation | Request validation rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-124 | Request validation | Request validation preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-125 | Request validation | Request validation reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-126 | Request validation | Request validation does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-127 | Request validation | Request validation remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-128 | Request validation | Request validation cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-129 | Request validation | Request validation survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-130 | Request validation | Request validation isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-131 | Request validation | Request validation preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-132 | Request validation | Request validation avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-133 | Request validation | Request validation uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-134 | Request validation | Request validation works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-135 | Request validation | Request validation records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-136 | Request validation | Request validation supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-137 | Request validation | Request validation handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-138 | Request validation | Request validation handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-139 | Request validation | Request validation rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-140 | Request validation | Request validation produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-141 | Request validation | Request validation preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-142 | Request validation | Request validation avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-143 | Request validation | Request validation validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-144 | Request validation | Request validation supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-145 | Request validation | Request validation keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-146 | Request validation | Request validation keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-147 | Request validation | Request validation preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-148 | Request validation | Request validation documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-149 | Request validation | Request validation documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-150 | Request validation | Request validation passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-151 | Damage resolution | Damage resolution accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-152 | Damage resolution | Damage resolution rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-153 | Damage resolution | Damage resolution rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-154 | Damage resolution | Damage resolution preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-155 | Damage resolution | Damage resolution reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-156 | Damage resolution | Damage resolution does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-157 | Damage resolution | Damage resolution remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-158 | Damage resolution | Damage resolution cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-159 | Damage resolution | Damage resolution survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-160 | Damage resolution | Damage resolution isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-161 | Damage resolution | Damage resolution preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-162 | Damage resolution | Damage resolution avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-163 | Damage resolution | Damage resolution uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-164 | Damage resolution | Damage resolution works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-165 | Damage resolution | Damage resolution records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-166 | Damage resolution | Damage resolution supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-167 | Damage resolution | Damage resolution handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-168 | Damage resolution | Damage resolution handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-169 | Damage resolution | Damage resolution rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-170 | Damage resolution | Damage resolution produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-171 | Damage resolution | Damage resolution preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-172 | Damage resolution | Damage resolution avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-173 | Damage resolution | Damage resolution validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-174 | Damage resolution | Damage resolution supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-175 | Damage resolution | Damage resolution keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-176 | Damage resolution | Damage resolution keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-177 | Damage resolution | Damage resolution preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-178 | Damage resolution | Damage resolution documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-179 | Damage resolution | Damage resolution documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-180 | Damage resolution | Damage resolution passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-181 | Healing resolution | Healing resolution accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-182 | Healing resolution | Healing resolution rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-183 | Healing resolution | Healing resolution rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-184 | Healing resolution | Healing resolution preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-185 | Healing resolution | Healing resolution reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-186 | Healing resolution | Healing resolution does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-187 | Healing resolution | Healing resolution remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-188 | Healing resolution | Healing resolution cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-189 | Healing resolution | Healing resolution survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-190 | Healing resolution | Healing resolution isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-191 | Healing resolution | Healing resolution preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-192 | Healing resolution | Healing resolution avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-193 | Healing resolution | Healing resolution uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-194 | Healing resolution | Healing resolution works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-195 | Healing resolution | Healing resolution records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-196 | Healing resolution | Healing resolution supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-197 | Healing resolution | Healing resolution handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-198 | Healing resolution | Healing resolution handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-199 | Healing resolution | Healing resolution rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-200 | Healing resolution | Healing resolution produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-201 | Healing resolution | Healing resolution preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-202 | Healing resolution | Healing resolution avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-203 | Healing resolution | Healing resolution validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-204 | Healing resolution | Healing resolution supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-205 | Healing resolution | Healing resolution keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-206 | Healing resolution | Healing resolution keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-207 | Healing resolution | Healing resolution preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-208 | Healing resolution | Healing resolution documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-209 | Healing resolution | Healing resolution documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-210 | Healing resolution | Healing resolution passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-211 | Modifier ordering | Modifier ordering accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-212 | Modifier ordering | Modifier ordering rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-213 | Modifier ordering | Modifier ordering rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-214 | Modifier ordering | Modifier ordering preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-215 | Modifier ordering | Modifier ordering reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-216 | Modifier ordering | Modifier ordering does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-217 | Modifier ordering | Modifier ordering remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-218 | Modifier ordering | Modifier ordering cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-219 | Modifier ordering | Modifier ordering survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-220 | Modifier ordering | Modifier ordering isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-221 | Modifier ordering | Modifier ordering preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-222 | Modifier ordering | Modifier ordering avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-223 | Modifier ordering | Modifier ordering uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-224 | Modifier ordering | Modifier ordering works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-225 | Modifier ordering | Modifier ordering records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-226 | Modifier ordering | Modifier ordering supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-227 | Modifier ordering | Modifier ordering handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-228 | Modifier ordering | Modifier ordering handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-229 | Modifier ordering | Modifier ordering rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-230 | Modifier ordering | Modifier ordering produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-231 | Modifier ordering | Modifier ordering preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-232 | Modifier ordering | Modifier ordering avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-233 | Modifier ordering | Modifier ordering validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-234 | Modifier ordering | Modifier ordering supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-235 | Modifier ordering | Modifier ordering keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-236 | Modifier ordering | Modifier ordering keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-237 | Modifier ordering | Modifier ordering preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-238 | Modifier ordering | Modifier ordering documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-239 | Modifier ordering | Modifier ordering documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-240 | Modifier ordering | Modifier ordering passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-241 | Receiver transactions | Receiver transactions accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-242 | Receiver transactions | Receiver transactions rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-243 | Receiver transactions | Receiver transactions rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-244 | Receiver transactions | Receiver transactions preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-245 | Receiver transactions | Receiver transactions reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-246 | Receiver transactions | Receiver transactions does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-247 | Receiver transactions | Receiver transactions remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-248 | Receiver transactions | Receiver transactions cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-249 | Receiver transactions | Receiver transactions survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-250 | Receiver transactions | Receiver transactions isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-251 | Receiver transactions | Receiver transactions preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-252 | Receiver transactions | Receiver transactions avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-253 | Receiver transactions | Receiver transactions uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-254 | Receiver transactions | Receiver transactions works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-255 | Receiver transactions | Receiver transactions records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-256 | Receiver transactions | Receiver transactions supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-257 | Receiver transactions | Receiver transactions handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-258 | Receiver transactions | Receiver transactions handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-259 | Receiver transactions | Receiver transactions rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-260 | Receiver transactions | Receiver transactions produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-261 | Receiver transactions | Receiver transactions preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-262 | Receiver transactions | Receiver transactions avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-263 | Receiver transactions | Receiver transactions validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-264 | Receiver transactions | Receiver transactions supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-265 | Receiver transactions | Receiver transactions keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-266 | Receiver transactions | Receiver transactions keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-267 | Receiver transactions | Receiver transactions preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-268 | Receiver transactions | Receiver transactions documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-269 | Receiver transactions | Receiver transactions documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-270 | Receiver transactions | Receiver transactions passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-271 | Idempotency and replay | Idempotency and replay accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-272 | Idempotency and replay | Idempotency and replay rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-273 | Idempotency and replay | Idempotency and replay rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-274 | Idempotency and replay | Idempotency and replay preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-275 | Idempotency and replay | Idempotency and replay reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-276 | Idempotency and replay | Idempotency and replay does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-277 | Idempotency and replay | Idempotency and replay remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-278 | Idempotency and replay | Idempotency and replay cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-279 | Idempotency and replay | Idempotency and replay survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-280 | Idempotency and replay | Idempotency and replay isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-281 | Idempotency and replay | Idempotency and replay preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-282 | Idempotency and replay | Idempotency and replay avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-283 | Idempotency and replay | Idempotency and replay uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-284 | Idempotency and replay | Idempotency and replay works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-285 | Idempotency and replay | Idempotency and replay records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-286 | Idempotency and replay | Idempotency and replay supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-287 | Idempotency and replay | Idempotency and replay handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-288 | Idempotency and replay | Idempotency and replay handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-289 | Idempotency and replay | Idempotency and replay rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-290 | Idempotency and replay | Idempotency and replay produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-291 | Idempotency and replay | Idempotency and replay preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-292 | Idempotency and replay | Idempotency and replay avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-293 | Idempotency and replay | Idempotency and replay validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-294 | Idempotency and replay | Idempotency and replay supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-295 | Idempotency and replay | Idempotency and replay keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-296 | Idempotency and replay | Idempotency and replay keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-297 | Idempotency and replay | Idempotency and replay preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-298 | Idempotency and replay | Idempotency and replay documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-299 | Idempotency and replay | Idempotency and replay documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-300 | Idempotency and replay | Idempotency and replay passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-301 | Defeat and outcomes | Defeat and outcomes accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-302 | Defeat and outcomes | Defeat and outcomes rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-303 | Defeat and outcomes | Defeat and outcomes rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-304 | Defeat and outcomes | Defeat and outcomes preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-305 | Defeat and outcomes | Defeat and outcomes reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-306 | Defeat and outcomes | Defeat and outcomes does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-307 | Defeat and outcomes | Defeat and outcomes remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-308 | Defeat and outcomes | Defeat and outcomes cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-309 | Defeat and outcomes | Defeat and outcomes survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-310 | Defeat and outcomes | Defeat and outcomes isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-311 | Defeat and outcomes | Defeat and outcomes preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-312 | Defeat and outcomes | Defeat and outcomes avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-313 | Defeat and outcomes | Defeat and outcomes uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-314 | Defeat and outcomes | Defeat and outcomes works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-315 | Defeat and outcomes | Defeat and outcomes records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-316 | Defeat and outcomes | Defeat and outcomes supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-317 | Defeat and outcomes | Defeat and outcomes handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-318 | Defeat and outcomes | Defeat and outcomes handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-319 | Defeat and outcomes | Defeat and outcomes rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-320 | Defeat and outcomes | Defeat and outcomes produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-321 | Defeat and outcomes | Defeat and outcomes preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-322 | Defeat and outcomes | Defeat and outcomes avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-323 | Defeat and outcomes | Defeat and outcomes validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-324 | Defeat and outcomes | Defeat and outcomes supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-325 | Defeat and outcomes | Defeat and outcomes keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-326 | Defeat and outcomes | Defeat and outcomes keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-327 | Defeat and outcomes | Defeat and outcomes preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-328 | Defeat and outcomes | Defeat and outcomes documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-329 | Defeat and outcomes | Defeat and outcomes documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-330 | Defeat and outcomes | Defeat and outcomes passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-331 | Physics2D hit adapter | Physics2D hit adapter accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-332 | Physics2D hit adapter | Physics2D hit adapter rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-333 | Physics2D hit adapter | Physics2D hit adapter rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-334 | Physics2D hit adapter | Physics2D hit adapter preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-335 | Physics2D hit adapter | Physics2D hit adapter reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-336 | Physics2D hit adapter | Physics2D hit adapter does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-337 | Physics2D hit adapter | Physics2D hit adapter remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-338 | Physics2D hit adapter | Physics2D hit adapter cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-339 | Physics2D hit adapter | Physics2D hit adapter survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-340 | Physics2D hit adapter | Physics2D hit adapter isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-341 | Physics2D hit adapter | Physics2D hit adapter preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-342 | Physics2D hit adapter | Physics2D hit adapter avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-343 | Physics2D hit adapter | Physics2D hit adapter uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-344 | Physics2D hit adapter | Physics2D hit adapter works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-345 | Physics2D hit adapter | Physics2D hit adapter records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-346 | Physics2D hit adapter | Physics2D hit adapter supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-347 | Physics2D hit adapter | Physics2D hit adapter handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-348 | Physics2D hit adapter | Physics2D hit adapter handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-349 | Physics2D hit adapter | Physics2D hit adapter rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-350 | Physics2D hit adapter | Physics2D hit adapter produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-351 | Physics2D hit adapter | Physics2D hit adapter preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-352 | Physics2D hit adapter | Physics2D hit adapter avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-353 | Physics2D hit adapter | Physics2D hit adapter validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-354 | Physics2D hit adapter | Physics2D hit adapter supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-355 | Physics2D hit adapter | Physics2D hit adapter keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-356 | Physics2D hit adapter | Physics2D hit adapter keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-357 | Physics2D hit adapter | Physics2D hit adapter preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-358 | Physics2D hit adapter | Physics2D hit adapter documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-359 | Physics2D hit adapter | Physics2D hit adapter documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-360 | Physics2D hit adapter | Physics2D hit adapter passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-361 | Physics3D hit adapter | Physics3D hit adapter accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-362 | Physics3D hit adapter | Physics3D hit adapter rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-363 | Physics3D hit adapter | Physics3D hit adapter rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-364 | Physics3D hit adapter | Physics3D hit adapter preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-365 | Physics3D hit adapter | Physics3D hit adapter reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-366 | Physics3D hit adapter | Physics3D hit adapter does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-367 | Physics3D hit adapter | Physics3D hit adapter remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-368 | Physics3D hit adapter | Physics3D hit adapter cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-369 | Physics3D hit adapter | Physics3D hit adapter survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-370 | Physics3D hit adapter | Physics3D hit adapter isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-371 | Physics3D hit adapter | Physics3D hit adapter preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-372 | Physics3D hit adapter | Physics3D hit adapter avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-373 | Physics3D hit adapter | Physics3D hit adapter uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-374 | Physics3D hit adapter | Physics3D hit adapter works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-375 | Physics3D hit adapter | Physics3D hit adapter records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-376 | Physics3D hit adapter | Physics3D hit adapter supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-377 | Physics3D hit adapter | Physics3D hit adapter handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-378 | Physics3D hit adapter | Physics3D hit adapter handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-379 | Physics3D hit adapter | Physics3D hit adapter rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-380 | Physics3D hit adapter | Physics3D hit adapter produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-381 | Physics3D hit adapter | Physics3D hit adapter preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-382 | Physics3D hit adapter | Physics3D hit adapter avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-383 | Physics3D hit adapter | Physics3D hit adapter validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-384 | Physics3D hit adapter | Physics3D hit adapter supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-385 | Physics3D hit adapter | Physics3D hit adapter keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-386 | Physics3D hit adapter | Physics3D hit adapter keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-387 | Physics3D hit adapter | Physics3D hit adapter preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-388 | Physics3D hit adapter | Physics3D hit adapter documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-389 | Physics3D hit adapter | Physics3D hit adapter documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-390 | Physics3D hit adapter | Physics3D hit adapter passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-391 | Batches and area effects | Batches and area effects accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-392 | Batches and area effects | Batches and area effects rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-393 | Batches and area effects | Batches and area effects rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-394 | Batches and area effects | Batches and area effects preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-395 | Batches and area effects | Batches and area effects reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-396 | Batches and area effects | Batches and area effects does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-397 | Batches and area effects | Batches and area effects remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-398 | Batches and area effects | Batches and area effects cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-399 | Batches and area effects | Batches and area effects survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-400 | Batches and area effects | Batches and area effects isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-401 | Batches and area effects | Batches and area effects preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-402 | Batches and area effects | Batches and area effects avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-403 | Batches and area effects | Batches and area effects uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-404 | Batches and area effects | Batches and area effects works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-405 | Batches and area effects | Batches and area effects records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-406 | Batches and area effects | Batches and area effects supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-407 | Batches and area effects | Batches and area effects handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-408 | Batches and area effects | Batches and area effects handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-409 | Batches and area effects | Batches and area effects rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-410 | Batches and area effects | Batches and area effects produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-411 | Batches and area effects | Batches and area effects preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-412 | Batches and area effects | Batches and area effects avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-413 | Batches and area effects | Batches and area effects validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-414 | Batches and area effects | Batches and area effects supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-415 | Batches and area effects | Batches and area effects keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-416 | Batches and area effects | Batches and area effects keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-417 | Batches and area effects | Batches and area effects preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-418 | Batches and area effects | Batches and area effects documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-419 | Batches and area effects | Batches and area effects documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-420 | Batches and area effects | Batches and area effects passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-421 | Events and logs | Events and logs accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-422 | Events and logs | Events and logs rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-423 | Events and logs | Events and logs rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-424 | Events and logs | Events and logs preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-425 | Events and logs | Events and logs reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-426 | Events and logs | Events and logs does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-427 | Events and logs | Events and logs remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-428 | Events and logs | Events and logs cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-429 | Events and logs | Events and logs survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-430 | Events and logs | Events and logs isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-431 | Events and logs | Events and logs preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-432 | Events and logs | Events and logs avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-433 | Events and logs | Events and logs uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-434 | Events and logs | Events and logs works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-435 | Events and logs | Events and logs records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-436 | Events and logs | Events and logs supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-437 | Events and logs | Events and logs handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-438 | Events and logs | Events and logs handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-439 | Events and logs | Events and logs rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-440 | Events and logs | Events and logs produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-441 | Events and logs | Events and logs preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-442 | Events and logs | Events and logs avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-443 | Events and logs | Events and logs validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-444 | Events and logs | Events and logs supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-445 | Events and logs | Events and logs keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-446 | Events and logs | Events and logs keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-447 | Events and logs | Events and logs preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-448 | Events and logs | Events and logs documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-449 | Events and logs | Events and logs documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-450 | Events and logs | Events and logs passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-451 | Multiplayer authority seams | Multiplayer authority seams accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-452 | Multiplayer authority seams | Multiplayer authority seams rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-453 | Multiplayer authority seams | Multiplayer authority seams rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-454 | Multiplayer authority seams | Multiplayer authority seams preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-455 | Multiplayer authority seams | Multiplayer authority seams reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-456 | Multiplayer authority seams | Multiplayer authority seams does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-457 | Multiplayer authority seams | Multiplayer authority seams remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-458 | Multiplayer authority seams | Multiplayer authority seams cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-459 | Multiplayer authority seams | Multiplayer authority seams survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-460 | Multiplayer authority seams | Multiplayer authority seams isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-461 | Multiplayer authority seams | Multiplayer authority seams preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-462 | Multiplayer authority seams | Multiplayer authority seams avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-463 | Multiplayer authority seams | Multiplayer authority seams uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-464 | Multiplayer authority seams | Multiplayer authority seams works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-465 | Multiplayer authority seams | Multiplayer authority seams records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-466 | Multiplayer authority seams | Multiplayer authority seams supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-467 | Multiplayer authority seams | Multiplayer authority seams handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-468 | Multiplayer authority seams | Multiplayer authority seams handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-469 | Multiplayer authority seams | Multiplayer authority seams rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-470 | Multiplayer authority seams | Multiplayer authority seams produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-471 | Multiplayer authority seams | Multiplayer authority seams preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-472 | Multiplayer authority seams | Multiplayer authority seams avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-473 | Multiplayer authority seams | Multiplayer authority seams validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-474 | Multiplayer authority seams | Multiplayer authority seams supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-475 | Multiplayer authority seams | Multiplayer authority seams keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-476 | Multiplayer authority seams | Multiplayer authority seams keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-477 | Multiplayer authority seams | Multiplayer authority seams preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-478 | Multiplayer authority seams | Multiplayer authority seams documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-479 | Multiplayer authority seams | Multiplayer authority seams documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-480 | Multiplayer authority seams | Multiplayer authority seams passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-481 | Performance and bounds | Performance and bounds accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-482 | Performance and bounds | Performance and bounds rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-483 | Performance and bounds | Performance and bounds rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-484 | Performance and bounds | Performance and bounds preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-485 | Performance and bounds | Performance and bounds reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-486 | Performance and bounds | Performance and bounds does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-487 | Performance and bounds | Performance and bounds remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-488 | Performance and bounds | Performance and bounds cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-489 | Performance and bounds | Performance and bounds survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-490 | Performance and bounds | Performance and bounds isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-491 | Performance and bounds | Performance and bounds preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-492 | Performance and bounds | Performance and bounds avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-493 | Performance and bounds | Performance and bounds uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-494 | Performance and bounds | Performance and bounds works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-495 | Performance and bounds | Performance and bounds records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-496 | Performance and bounds | Performance and bounds supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-497 | Performance and bounds | Performance and bounds handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-498 | Performance and bounds | Performance and bounds handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-499 | Performance and bounds | Performance and bounds rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-500 | Performance and bounds | Performance and bounds produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-501 | Performance and bounds | Performance and bounds preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-502 | Performance and bounds | Performance and bounds avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-503 | Performance and bounds | Performance and bounds validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-504 | Performance and bounds | Performance and bounds supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-505 | Performance and bounds | Performance and bounds keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-506 | Performance and bounds | Performance and bounds keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-507 | Performance and bounds | Performance and bounds preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-508 | Performance and bounds | Performance and bounds documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-509 | Performance and bounds | Performance and bounds documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-510 | Performance and bounds | Performance and bounds passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-511 | Packaging, removal, and release | Packaging, removal, and release accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-512 | Packaging, removal, and release | Packaging, removal, and release rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-513 | Packaging, removal, and release | Packaging, removal, and release rejects a stale handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-514 | Packaging, removal, and release | Packaging, removal, and release preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-515 | Packaging, removal, and release | Packaging, removal, and release reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-516 | Packaging, removal, and release | Packaging, removal, and release does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-517 | Packaging, removal, and release | Packaging, removal, and release remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-518 | Packaging, removal, and release | Packaging, removal, and release cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-519 | Packaging, removal, and release | Packaging, removal, and release survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-520 | Packaging, removal, and release | Packaging, removal, and release isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-521 | Packaging, removal, and release | Packaging, removal, and release preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-522 | Packaging, removal, and release | Packaging, removal, and release avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-523 | Packaging, removal, and release | Packaging, removal, and release uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-524 | Packaging, removal, and release | Packaging, removal, and release works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-525 | Packaging, removal, and release | Packaging, removal, and release records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-526 | Packaging, removal, and release | Packaging, removal, and release supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-527 | Packaging, removal, and release | Packaging, removal, and release handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-528 | Packaging, removal, and release | Packaging, removal, and release handles cancellation before commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-529 | Packaging, removal, and release | Packaging, removal, and release rejects cancellation after commit. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-530 | Packaging, removal, and release | Packaging, removal, and release produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-531 | Packaging, removal, and release | Packaging, removal, and release preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-532 | Packaging, removal, and release | Packaging, removal, and release avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-533 | Packaging, removal, and release | Packaging, removal, and release validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-534 | Packaging, removal, and release | Packaging, removal, and release supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-535 | Packaging, removal, and release | Packaging, removal, and release keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-536 | Packaging, removal, and release | Packaging, removal, and release keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-537 | Packaging, removal, and release | Packaging, removal, and release preserves GUIDs for public assets. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-538 | Packaging, removal, and release | Packaging, removal, and release documents platform limitations. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-539 | Packaging, removal, and release | Packaging, removal, and release documents measured limits only after execution. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |
| ECLASH-T-540 | Packaging, removal, and release | Packaging, removal, and release passes the package-qualified regression case. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved contract. | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Feasibility/specification gate

- [x] Ownership and non-goals approved.
- [x] Receiver-owned mutable state boundary approved.
- [x] Requests, IDs, magnitudes, targetability, relations, modifiers, transactions, outcomes, events, hit adapters, diagnostics, and Laboratory specified.
- [x] Multiplayer/save/presentation boundaries explicit.
- [x] Every empirical claim remains `Not run`.

### 24.2 Implementation gate

- [ ] Package skeleton approved after SUITE-DOC-33.
- [ ] Core compiles with declared dependencies only.
- [ ] Editor and adapter assemblies remain isolated.
- [ ] Pure modifiers cannot mutate receiver state.
- [ ] Receiver commit/idempotency/event ordering passes tests.
- [ ] Public API matches this foundation or an approved revision/ADR.

### 24.3 Standalone gate

- [ ] Clean-project install succeeds.
- [ ] Resolver Laboratory passes all required scenarios.
- [ ] Simulated receiver proves core behavior without peer packages.
- [ ] Samples can be removed.
- [ ] Direct-scene entry and teardown match documentation.

### 24.4 Adapter/quality gate

- [ ] Each hit/provider/bridge adapter has its own Integration Laboratory.
- [ ] Authority and capability mismatches are explicit.
- [ ] Performance bounds pass declared fixtures.
- [ ] Diagnostics are actionable and redacted.
- [ ] No Blocker/Critical defects remain.

### 24.5 Distribution gate

- [ ] Manifest, version, changelog, license, notices, and `.meta` files complete.
- [ ] Git/tarball installation tested.
- [ ] Removal/reinstall tested with project-owned receiver data preserved.
- [ ] Documentation examples compile.
- [ ] Compatibility catalog updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing combat | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Don't Get Vince'd | Beat-'em-up attacks, damage, boss phases | Introduce request/result and receiver contracts one damage path at a time | Existing combos, reactions, defeat, and animation timing unchanged | Keep original damage path selectable |
| Rescuers2D | Axe, shield bash, hazards, destructibles | Add neutral target/receiver around one interaction/hazard | No controller or animation regression | Retain project scripts |
| Echo Systems Lab | Target Range damage framework | Build a clean portfolio integration | Target hits, scoring, HUD events preserved | Remove adapter and restore original |
| Hackulos | Planned melee, spells, DoTs, healing, enemies | Start with instantaneous damage/healing only | Fighter/Necromancer vertical-slice parity | Keep project combat layer until proven |

### 25.2 Preserve-until-parity rule

Working combat remains intact until Clash passes its standalone Laboratories and one narrow project integration. Migrate request creation, target receiving, modifiers, and presentation separately. Do not remove existing health or stats merely because the neutral contract exists.

### 25.3 Migration tooling

Future tools may detect damageable interfaces, duplicate IDs, direct field mutation patterns, repeated team checks, and event subscriptions; create project-owned receiver adapters; and produce a migration report. They must not rewrite gameplay formulas or health systems automatically.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| ECLASH-R-001 | Universal combat-framework scope inflation | High | High | Neutral request/receiver boundary and explicit non-goals | Every capability review |
| ECLASH-R-002 | Combat owns mutable health/stat state | Medium | High | Receiver-owned transaction contract | API review |
| ECLASH-R-003 | Nondeterministic modifier order | Medium | High | Phase, priority, stable ID ordering and explanations | Unit tests |
| ECLASH-R-004 | Duplicate hits/replays | High | High | Request IDs, deduplication, idempotency bounds | Runtime/network tests |
| ECLASH-R-005 | Friendly-fire inconsistency | Medium | High | One relation-policy stage | Integration tests |
| ECLASH-R-006 | Listener performs required gameplay state | Medium | High | Publish after commit; listener failure isolation | Code review |
| ECLASH-R-007 | Client-authoritative cheating | Medium | Critical | Convergence authority validation | Network review |
| ECLASH-R-008 | Physics adapter leaks into core | Medium | Medium | Separate assemblies/packages | Assembly audit |
| ECLASH-R-009 | Fixed-point scale misuse/overflow | Medium | High | Config validation, checked arithmetic, range tests | Resolver tests |
| ECLASH-R-010 | Cross-target atomicity assumed | Medium | High | Per-target transactions and explicit batch semantics | API/docs |
| ECLASH-R-011 | Logs leak private/project data | Low | Medium | Redacted bounded records | Security review |
| ECLASH-R-012 | Package removal deletes target state | Low | Critical | Project-owned receivers and bridge-first removal | Removal test |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Approved package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| ECLASH-D-001 | Mutable combat resources remain target/project-owned | Approved | Preserve genre neutrality and save ownership | Core needs receiver transaction seam | No |
| ECLASH-D-002 | Damage and healing are distinct operation kinds | Approved | Avoid negative-damage ambiguity | Separate policies/modifiers/results | No |
| ECLASH-D-003 | Magnitudes use checked 64-bit fixed-point units | Approved | Deterministic, fractional-capable representation | Project config defines scale/rounding | Review after prototype evidence |
| ECLASH-D-004 | Modifiers are pure ordered transforms | Approved | Explainability and rollback safety | Side effects happen only in receiver commit or listeners after commit | No |
| ECLASH-D-005 | Receiver prepare/commit is the one mutation boundary | Approved | Atomic target-owned application | One receiver authority per target request | No |
| ECLASH-D-006 | Events publish after commit | Approved | Presentation cannot become rule authority | Listeners cannot veto committed result | No |
| ECLASH-D-007 | Hit detection is adapter evidence, not resolution | Approved | Support 2D/3D/custom/network hits | Project maps candidate to request | No |
| ECLASH-D-008 | Area attacks are per-target transactions under one causality/batch ID | Approved | Avoid false distributed atomicity | Partial target outcomes are explicit | No |
| ECLASH-D-009 | Multiplayer authority belongs to Convergence/provider bridge | Approved | Security and provider neutrality | Core has no networking SDK | No |
| ECLASH-D-010 | Live combat state is not saved by the core | Approved | Prevent stale attacks/targets | Target state saved by owner | No |

### 27.2 Release-blocking questions

No question blocks approval of this feasibility foundation. Implementation later must resolve:

- Exact minimum Unity version and package pins.
- Measured fixed-point scale ergonomics and overflow limits.
- Exact first project receiver/reference sample without implying a universal health model.
- Whether the core exposes synchronous-only resolution permanently or adds a separately versioned async/provider module.
- Concrete throughput, modifier, target, and history limits.

### 27.3 Non-blocking later questions

- Armor/shield layer conventions for `EchoRPG.Foundation`.
- Lag compensation and predicted presentation per networking provider.
- Ongoing status-effect ownership with Arcana.
- Combat replay and deterministic simulation requirements.
- Shared combat-log schema with analytics or spectator systems.

---

## 28. Milestones and Checkpoint Path

| Milestone | Outcome | Included capabilities | Evidence |
|---|---|---|---|
| C0 - Feasibility foundation | Approved pre-code contract | This document and boundary record | Documentation audit |
| C1 - Skeleton | Installable package anatomy | Manifest, asmdefs, docs shell | Clean compile/removal |
| C2 - Pure data/policies | IDs, magnitudes, targetability, relations, modifiers | EditMode tests |
| C3 - Runtime service | Root, registries, resolver, receiver transactions | PlayMode tests |
| C4 - Events/diagnostics | Outcomes, logs, explanations, redaction | Diagnostic tests |
| C5 - Hit adapters | Physics2D and Physics3D candidates | Separate Laboratories |
| C6 - Editor tooling | Setup, validation, inspectors | Repeat-safe tooling tests |
| C7 - First project adoption | One narrow real-project path | Parity/rollback report |
| C8 - First bridges | Impact/Instinct/Arcana or Convergence | Integration Laboratories |
| C9 - Beta/release | Packaging, docs, licenses, evidence | SFGSS-004 gates |

### 28.1 First recommended implementation checkpoint

Dormant until SUITE-DOC-33: create only the package skeleton and documentation shell. No combat runtime code is authorized now.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.
Treat SFGSS-000 and SFGSS-002 through SFGSS-005 as suite authorities.
Treat Clash - EchoCombat Feasibility Foundation v1.0.0 as the Level 2 authority
for provider-neutral combat requests, targetability, relations, deterministic
resolution, pure modifiers, target-owned receiver transactions, outcomes,
events, hit adapters, diagnostics, and genre-neutral combat boundaries.
EchoCombat remains an Advanced candidate. No implementation, universal health
model, damage formula, physics adapter, networking provider, measured performance,
or production compatibility is approved. Package implementation remains locked
until SUITE-DOC-33. Current checkpoint after approval: SUITE-DOC-21 - Arcana
(EchoAbilities) Feasibility Foundation. Preserve receiver-owned mutable state,
explicit adapters, post-commit events, and honest Not run evidence. When code is
eventually authorized, show complete files and explain every step so Jesse can
enter and understand them himself.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package foundation | 1.0.0 Approved feasibility foundation |
| Implementation | Not started |
| Laboratory scenarios | 84 planned; Not run |
| Tests | 540 planned; Not run |
| Optional adapters | Physics2D, Physics3D, Arcana, Instinct, Fellowship, Impact, Convergence; none implemented |
| Known blockers | None for documentation; implementation locked by SUITE-DOC-33 |
| Next checkpoint | SUITE-DOC-21 - Arcana (`EchoAbilities`) Feasibility Foundation |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and responsibility are clear.
- [x] A universal health/stat model and universal formula are explicitly rejected.
- [x] Target/project ownership of mutable resources is preserved.
- [x] Requests, identities, magnitudes, targetability, relations, modifiers, receiver transactions, outcomes, events, and hit adapters are specified.
- [x] Save, multiplayer, AI, abilities, inventory, feedback, UI, and physics boundaries are explicit.
- [x] Standalone Laboratories and tests are planned.
- [x] Diagnostics, privacy, bounds, removal, and release gates are specified without false evidence.
- [x] No implementation is authorized.

### 30.2 Approval record

**Decision:** Approved feasibility foundation  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Implementation remains locked until SUITE-DOC-33. All empirical evidence remains `Not run` until executed and recorded.


---

## Graph Navigation

#sfgss/package #sfgss/wave/advanced #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
