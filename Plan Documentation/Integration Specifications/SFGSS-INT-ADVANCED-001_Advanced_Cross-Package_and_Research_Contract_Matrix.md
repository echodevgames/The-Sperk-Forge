---
tags:
  - sfgss/integration
  - sfgss/wave/advanced
  - sfgss/status/approved
status: approved
updated: 2026-08-04
---

# SFGSS-INT-ADVANCED-001 — Advanced Cross-Package and Research Contract Matrix

**Version:** 1.0.0  
**Status:** Approved  
**Decision date:** August 4, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Scope:** The five Advanced package foundations in SFGSS-000 Section 7.3, their Foundation and Expansion seams, research gates, provider adapters, persistence, Laboratories, and removal behavior  
**Parent authorities:** SFGSS-000 v0.14.0, the five approved Advanced package foundations, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, SFGSS-005 v1.2.0, SFGSS-ADR-001 v1.2.0, SFGSS-INT-FOUNDATION-001, and SFGSS-INT-EXPANSION-001  
**Evidence state:** Documentation and research-record review only; all implementation, provider-prototype, performance, compatibility, and release evidence remains `Not run`

> The Advanced packages may coordinate powerful systems, but none receives permission to become the hidden game engine beneath the game.

---

## 1. Purpose

This integration specification records the result of SUITE-DOC-24. It compares The Convergence, Instinct, Clash, Arcana, and The Atlas against one another and against every approved Foundation and Expansion authority.

It answers:

- Which Advanced package owns each concern?
- Which identities must remain separate across network, character, combat, ability, AI, and world systems?
- Which authority commits each multi-package operation?
- How do semantic world travel, network coordination, scene travel, spawning, and camera handoff occur without circular ownership?
- How do AI target selection, ability activation, and combat resolution collaborate without duplicating decisions?
- Which state may persist, and which runtime state must remain session-only?
- What provider research is approved, what remains unknown, and what evidence must not be fabricated?
- Which Advanced setup facades are registered for future Workshop automation?
- How are Laboratories, providers, bridges, and package removal separated?

This document adds integration detail. It does not approve implementation, select a multiplayer provider, select an AI backend, or replace the five package foundations as the Level 2 authorities for their internal behavior.

---

## 2. Review result

**Result:** Passed after one documentation repair and several integration clarifications.

| Gate | Result | Notes |
|---|---|---|
| One authority per concern | Pass | Network, AI, combat, ability, world, scene, character, save, and progression authorities remain distinct |
| Core dependency direction | Pass | No Advanced core requires an optional Echo peer or provider SDK |
| Provider neutrality | Pass | Networking, navigation, behavior-graph, inference, hit-detection, and scene backends remain isolated adapters |
| Identity separation | Pass | Participant, network entity, character, runtime actor, ability owner, combat target, and world location IDs remain distinct |
| Transaction ownership | Pass | Every high-risk workflow names one commit owner and rejects fictional distributed rollback |
| Travel orchestration | Pass after clarification | Atlas plans semantics, Convergence coordinates authority/readiness, Passage executes scenes, Atlas commits context, Fellowship places actors |
| AI/combat/ability boundary | Pass | Instinct proposes, Arcana orchestrates abilities, Clash resolves instantaneous combat operations |
| Persistence | Pass | Chronicle transports durable payloads; each package owns its own records; live operations remain session-only |
| Workshop setup | Pass after repair | ADR-001 v1.2.0 registers all five Advanced setup facades and domains |
| Diagnostics | Pass | `EMUL-*`, `EAI-*`, `ECLASH-*`, `EABL-*`, and `EWRLD-*` remain globally unique |
| Test Lab isolation | Pass | Simulated providers prove each neutral core; provider Integration Laboratories remain separate |
| Removal | Pass | Provider/bridge-first teardown preserves neutral cores and project-owned durable data |
| Provider selection | Not run | No networking, AI, navigation, behavior, inference, or hosting provider is approved by documentation alone |

No Advanced package was merged, removed, converted into a hard dependency, or promoted from feasibility/provider-neutral foundation to implementation-ready release status.

---

## 3. Advanced authority matrix

| Package | Sole Advanced authority | Explicitly remains outside the package | Runtime topology | Durable package state |
|---|---|---|---|---|
| The Convergence (`EchoMultiplayer`) | Provider-neutral sessions, participants, roles, readiness, connection lifecycle, capabilities, authority queries, synchronized-travel/spawn/ownership seams, adapter compliance, security gates | Transport, RPC/replication implementation, provider cloud, credentials, game rules, character truth, scene execution, save files | One application-session root when multiplayer is active; exactly one production provider per session | Live session state is session-only; sensitive reconnect material follows provider security policy; shared saves remain Chronicle-owned |
| Instinct (`EchoAI`) | Actor-local sensing, perception memory, scoring, typed context, decision scheduling, lightweight behavior contracts, navigation requests, debug traces | Enemy personality, factions, combat rules, abilities, movement physics, animation, world truth, navigation technology, multiplayer authority | Actor-local agent hosts plus optional scene/world registry and scheduler; no universal enemy singleton | Live observations, paths, tickets, and decisions are session-only; only project-approved safe-point snapshots may persist |
| Clash (`EchoCombat`) | Combat requests/results, targetability, relation evaluation seams, deterministic modifier pipeline, transactional receiver application, combat outcomes and defeat events | Universal health/stats, attack timing, abilities, equipment rules, AI decisions, physics detection, respawn/loot, network transport | One application-session resolver/service with registered receivers/providers; injected service supported | Combat requests, hit traces, and logs are session-only; durable health/resources belong to their owning systems |
| Arcana (`EchoAbilities`) | Ability definitions, owner grants/loadouts, activation lifecycle, conditions, costs, charges, cooldowns, cast/channel/interruption, targeting contracts, typed effect orchestration | Character identity, input, general resources, combat formulas, status-effect framework, specific spells/classes, network transport | One application-session service coordinating owner-scoped ability state | Grants/loadouts and approved cooldown/charge state may persist; active casts, targets, effect tickets, and provider handles never persist |
| The Atlas (`EchoWorld`) | World/zone/location identity, topology, semantic travel plans, scene-binding metadata, marker registries, discovery/visitation, fast-travel eligibility, map snapshots, world-state participant routing | Scene execution, character spawning, movement, pathfinding, objectives, camera movement, map rendering, save files, network transport, world simulation | One application-session world authority with runtime marker/provider registries | Context, discovery, visitation, and typed participant records may persist through Chronicle; live markers and prepared plans are session-only |

---

## 4. Identity crosswalk

The following identities must not be collapsed into a generic `PlayerId`, `ActorId`, `TargetId`, or `Location` without qualification.

| Identity | Owner | Meaning | Never substituted with |
|---|---|---|---|
| `AccountId` or provider account reference | Authentication/provider layer | External account identity when a provider exposes one | Participant, character, profile, or save-slot ID |
| `ParticipantId` | Convergence | One participant in the current multiplayer session | Account, character, network entity, or input-user ID |
| `NetworkEntityId` | Networking provider adapter | Provider-owned replicated entity identity | Character definition, durable character, runtime actor, or GameObject instance ID |
| `CharacterId` | Fellowship | One durable roster member | Participant, network entity, actor GameObject, or ability owner token |
| `CharacterRuntimeInstanceId` | Fellowship | One spawned character actor in the current session | Durable CharacterId or provider network identity |
| `ControlOwnerId` | Fellowship | Durable control assignment identity used for possession handoff | Input user, participant, or network entity ID |
| Input user/device identity | Will/project adapter | Local device and input-user assignment | Participant or ControlOwnerId |
| AI agent identity | Instinct/project | One actor-local thinking authority | CharacterId unless a bridge explicitly maps them |
| Ability owner identity | Arcana/project provider | Owner-scoped ability state and activation authority | CharacterId, participant, or combat target unless bridged |
| Combat target identity | Clash receiver registry | One registered combat receiver target | Character, network entity, AI memory record, or world marker ID |
| World/zone/location IDs | Atlas | Durable semantic place identity | Scene path, build index, NavMesh node, spawn marker, or network scene ID |
| Scene-binding token | Atlas/project resolver | Opaque mapping from semantic location to scene-transition data | LocationId or raw scene string |
| Entry/spawn marker ID | Atlas | Semantic arrival or spawn candidate | Character spawn request, scene object path, or world location ID |

Bridges store explicit mappings and lifetimes. They do not redefine or alias one authority’s identifier into another authority’s domain.

---

## 5. Runtime topology and lifecycle

### 5.1 Topology classes

| Class | Packages | Rule |
|---|---|---|
| Application-session authority | Convergence when multiplayer is enabled, Clash, Arcana, Atlas | First valid root/service claims before side effects; injection remains available |
| Actor-local authority | Instinct | Each AI agent owns its local memory, context, and behavior state; shared registries/schedulers coordinate but do not become the agent brain |
| Provider adapter | Multiplayer, navigation, behavior, inference, hit, hosting, platform adapters | Separate package/assembly with real hard dependency on its provider and neutral core |
| Project adapter/bridge | Cross-package workflows | Registers only after all required peers are Ready and owns every lease/subscription it creates |

### 5.2 Recommended composition order

When a project installs all relevant authorities, the recommended order is:

1. Foundation preferences, diagnostics, save, game-state, scene, input, UI, and audio authorities selected by the project.
2. Expansion definition/state authorities such as Fellowship, Vault, Path, Ascent, and Crucible.
3. Atlas definitions/topology and current semantic context.
4. Clash resolver and Arcana ability service.
5. Convergence neutral service and the explicitly selected provider adapter, when multiplayer is active.
6. Spawned actor-local Fellowship instances and Vessel motors.
7. Instinct AI agents, sensors, navigation adapters, and behavior executors.
8. Optional pairwise bridges and project orchestration register after both peers report Ready.

No core waits indefinitely for an optional bridge or provider. Late registration reconciles current snapshots and revisions rather than replaying unsafe historical commands.

### 5.3 Shutdown order

1. Stop new project-level commands.
2. Cancel or settle pre-commit Arcana activations, AI behavior tickets, Atlas plans, and network requests.
3. Remove project orchestration and pairwise bridges.
4. Stop provider adapters and provider-owned callbacks.
5. Unregister actor-local agents, targets, receivers, markers, and ability owners.
6. Shut down Advanced roots/services.
7. Preserve project-owned durable records and configuration.

Unexpected peer loss invalidates its handles and returns structured `Unavailable`, `Disconnected`, or `Stale` results. Stale references are never retained as silent authority.

---

## 6. Dependency and adapter topology

### 6.1 Core rule

Each Advanced core compiles with only its declared Unity/platform dependencies. It does not directly reference another Echo runtime core or an optional provider SDK.

### 6.2 Adapter classes

| Adapter class | Examples | Dependency rule |
|---|---|---|
| Multiplayer provider | NGO, FishNet, Mirror, Photon Fusion, future provider | Depends on Convergence core and exact provider SDK/service packages |
| AI navigation provider | Unity AI Navigation, 2D/grid/custom navigation | Depends on Instinct and exact navigation backend |
| Behavior-authoring provider | Unity Behavior or future graph backend | Depends on Instinct and exact authoring/runtime package |
| Inference provider | Unity Inference Engine or future inference backend | Experimental separate adapter; never required by Instinct core |
| Combat hit provider | Physics2D, Physics3D, projectile/project adapters | Depends on Clash plus declared physics/project seams |
| Ability effect bridge | Arcana + Clash, Arcana + Vault, Arcana + Impact, project effects | Separate bridge or project adapter; Arcana core does not reference peers |
| World/scene bridge | Atlas + Passage | Separate bridge mapping semantic plans to scene transitions |
| World/network bridge | Atlas + Convergence | Separate bridge mapping shared context and authority snapshots |

Provider and bridge removal occurs before either neutral core. Removing one adapter never deletes project-owned data or makes unrelated cores fail to compile.

### 6.3 Multi-package orchestration

Reusable bridges should remain pairwise. A workflow requiring three or more authorities is composed by project-owned orchestration or a separately approved integration specification. The suite does not create a giant “Advanced Everything Bridge.”

---

## 7. Commit ownership matrix

Exactly one authority owns each commit. Other packages may prepare, validate, authorize, execute a provider operation, ledger, replicate, or observe.

| Workflow | Commit owner | Other participants |
|---|---|---|
| Session create/join/leave/readiness | Convergence provider adapter | UI submits requests; Pulse observes session phase |
| Participant-to-character selection | Fellowship | Convergence validates session authority; UI submits request |
| Provider network ownership | Networking provider adapter | Convergence exposes neutral result; Fellowship stores no provider-native ownership object |
| Actor control handoff | Fellowship | Will maps local input; Vessel acquires actor-local lease; Convergence validates remote authority |
| AI target choice | Instinct | Clash relation/targetability and Arcana availability may be read-only inputs |
| Ability activation | Arcana | Conditions, cost provider, targeting provider, AI/input requester, Convergence authority gate |
| Ability combat effect | Clash receiver transaction | Arcana commits activation/effect dispatch first; Clash resolves and target receiver commits resource mutation |
| Direct combat request | Clash receiver transaction | AI, interaction, projectile, ability, or project submits request |
| Target health/resource mutation | Registered target receiver’s owning system | Clash validates/resolves and publishes result after receiver commit |
| Character defeat availability | Fellowship/project character-state authority | Clash reports receiver outcome; Path/Ascent/AI observe through bridges |
| Semantic world travel plan | Atlas | Conditions and topology providers contribute read-only data |
| Network travel readiness/authorization | Convergence/provider adapter | Atlas plan and Passage request remain unchanged until authority succeeds |
| Scene transition | Passage | Convergence coordinates peers; UI/Audio/Pulse present; Atlas does not load scenes |
| World-context commit | Atlas | Occurs only after successful approved transition/handoff |
| Arrival marker selection | Atlas | Fellowship/project executes character placement or spawn |
| Shared-world save publication | Chronicle on authoritative host/server | Atlas, Fellowship, Vault, Path, Arcana, AI/project participants contribute their own payloads |
| Objective or progression mutation | Path or Ascent respectively | AI/combat/ability/world events submit idempotent semantic requests |

No bridge claims distributed rollback after a foreign authority reports commitment.

---

## 8. Canonical high-risk workflows

### 8.1 Multiplayer semantic world travel

1. Atlas validates the requested destination and prepares a semantic `WorldTravelPlan` containing stable IDs, revisions, scene-binding token, and marker criteria.
2. The Atlas/Convergence bridge submits the plan identity and required participant policy to Convergence.
3. Convergence/provider validates authority, readiness, capability, late-join policy, and synchronized-travel admission.
4. The project orchestration or approved integration invokes Passage with the opaque scene-binding result.
5. Passage owns fade, loading, activation, failure recovery, and scene-transition completion.
6. Convergence/provider confirms synchronized completion or identifies failed/disconnected participants.
7. Atlas commits the new semantic world context only after the approved transition result.
8. Atlas selects arrival markers; Fellowship/project owns spawn or relocation.
9. Eye, Hand, AI, UI, audio, and objectives reconcile from committed snapshots.

Failure before Atlas context commit leaves the previous semantic context authoritative. A partially connected provider cannot make a failed scene transition appear successful.

### 8.2 Multiplayer character spawn and control

1. Convergence identifies the session `ParticipantId` and validates the request.
2. The Fellowship bridge maps that participant to a durable `CharacterId` under project policy.
3. Fellowship validates roster availability and requests/commits spawn through its provider.
4. The provider adapter assigns a separate `NetworkEntityId` when applicable.
5. Fellowship commits `ControlOwnerId`; the Vessel bridge acquires the actor-local control lease.
6. The Will maps only the local participant/input user to normalized intent.
7. The Eye and Hand receive actor mappings after the control commit.

Participant identity, network ownership, durable character identity, runtime actor identity, and input ownership remain separate records.

### 8.3 AI ability and combat decision

1. Instinct gathers observations and bounded memory.
2. Instinct scores targets and behavior choices using read-only providers.
3. Clash may provide relation and targetability snapshots; Arcana may provide ability availability snapshots.
4. Instinct chooses a semantic action. It does not mutate health, spend resources, or start a cooldown itself.
5. For an ability, Instinct submits an Arcana activation request.
6. Arcana validates owner, conditions, target, costs, charges, cooldowns, cast/channel policy, and authority.
7. At the declared Arcana commit point, costs/charges/cooldowns settle according to policy.
8. A typed effect may submit a Clash combat request.
9. Clash resolves deterministic modifiers and asks the target receiver to commit the resource mutation.
10. Clash publishes the combat outcome; Instinct updates memory only from committed semantic events.

The AI decision is not the ability commit, and the ability commit is not the combat-resource commit.

### 8.4 Ability costs spanning foreign systems

The Arcana MVP permits one mutation-capable cost provider per activation. That provider owns atomic cost preparation and commit.

- A project may supply one owner resource service covering mana, stamina, charges, or similar resources.
- A Vault bridge may implement an item/ammunition cost provider through one Vault transaction.
- A future aggregate provider must become a real authority with truthful rollback semantics before it can combine multiple foreign systems.
- Arcana must reject a configuration that promises atomic costs across several independent providers without a coordinating transaction owner.

### 8.5 Defeat, rewards, and world consequences

1. Clash reports the receiver-owned combat outcome, including defeat when the receiver declares it.
2. Fellowship or project character-state code decides availability, despawn, respawn, corpse, or replacement behavior.
3. The Path may submit objective progress using an idempotent request.
4. The Ascent may receive a separate progression mutation where designed.
5. The Vault or project loot authority owns item grants and containers.
6. Atlas or project world-state participants own durable boss/encounter/world consequences.
7. Impact, Resonance, Eye, and UI present feedback after authoritative commits.

Clash never awards loot, changes roster availability, marks objectives complete, or writes world flags directly.

### 8.6 AI navigation versus world travel

- Atlas route planning operates over semantic worlds, zones, locations, and authored travel connections.
- Instinct navigation requests operate over local movement/navigation providers inside the currently active world/scene context.
- Passage executes Unity scene transitions.
- Vessel executes actor movement physics.

A semantic route is not a NavMesh path. A NavMesh path is not a scene transition. A scene transition is not a controller movement command.

---

## 9. Persistence and save ownership

| Advanced state | Durable? | Owner and transport rule |
|---|---:|---|
| Convergence live sessions, participants, readiness, network entities | No | Provider/session runtime only |
| Reconnect credential/token | Conditional sensitive | Provider/project secure storage policy; never ordinary logs or Chronicle payload by default |
| Shared-world game save | Yes | Chronicle on authoritative host/server; each package contributes only its own payload |
| Instinct observations, paths, active behaviors, scheduler tickets | No | Session-only |
| Approved durable AI snapshot | Optional | Instinct/project participant at declared safe points; no live provider handles or scene objects |
| Clash requests, modifiers-in-flight, hit logs | No | Session-only diagnostics; target resources persist under their owners |
| Arcana grants and loadouts | Optional durable | Arcana participant through Chronicle/project transport |
| Arcana cooldown/charge state | Optional by project policy | Versioned Arcana payload; unknown definitions preserved |
| Arcana active casts/channels/effect tickets | No | Never serialized |
| Atlas current context | Optional durable | Stable world/zone/location IDs through Atlas participant |
| Atlas discovery and visitation | Optional durable | Atlas participant through Chronicle/project transport |
| Atlas runtime markers and prepared travel plans | No | Session-only |
| Atlas typed world-state participant records | Optional durable | Routed by Atlas, transported by Chronicle, owned by the registered provider |

Clients do not upload trusted shared-world truth by default. Provider-native connection/session objects never enter durable DTOs.

---

## 10. Provider research and approval gates

### 10.1 Multiplayer

- No production networking provider is approved.
- NGO plus current Multiplayer Services remains the first-party baseline prototype.
- FishNet or Mirror remains the second comparison slot according to license review.
- Photon Fusion remains conditional when prediction, lag compensation, or managed cloud is a leading requirement.
- At least two disposable prototypes must execute the same protocol before a provider-selection ADR.
- Topology, hosting, relay/lobby, prediction, reconnect, migration, cost, platform, and licensing claims remain `Not run` until exact evidence exists.

### 10.2 AI and navigation

- Instinct core has no mandatory navigation backend.
- Unity AI Navigation, Unity Behavior, and Unity Inference Engine remain optional candidate adapters recorded in the feasibility research.
- No candidate is approved as mandatory, Supported, or performance-qualified.
- 2D, grid, flying, custom-navigation, and simulated providers must remain possible.

### 10.3 Combat, abilities, and world

- Clash does not select one health/stat, hit-detection, or faction implementation.
- Arcana does not select one resource, targeting, status-effect, graph, or prediction provider.
- Atlas does not select one scene-loading, streaming, Addressables, pathfinding, map, or procedural-world backend.
- Provider-specific claims require separate adapters, Laboratories, compatibility evidence, and release gates.

### 10.4 Research honesty

A dated comparison or feasibility record is planning evidence, not runtime proof. “Candidate,” “planned,” “experimental,” and “not run” must remain visible until retained executions justify stronger language.

---

## 11. Workshop setup facade registry

SFGSS-ADR-001 v1.2.0 is the exact registry. Advanced packages remain usable without facades and may advertise only manual setup until facade and Workshop adapter evidence passes.

| Package | Exact facade type | Minimum planning domains |
|---|---|---|
| The Convergence | `EchoDevGames.EchoMultiplayer.Editor.Workshop.EchoMultiplayerWorkshopSetupFacade` | Neutral configuration, simulated provider, session/readiness/authority policies, metadata schemas, provider selection report, Laboratory; production adapter installation remains explicit |
| Instinct | `EchoDevGames.EchoAI.Editor.Workshop.EchoAIWorkshopSetupFacade` | Agent configuration, sensor/memory/scoring/blackboard/scheduler/behavior schemas, simulated providers, navigation-adapter choice, Laboratories |
| Clash | `EchoDevGames.EchoCombat.Editor.Workshop.EchoCombatWorkshopSetupFacade` | Resolver configuration, operation/channel/relation/modifier definitions, simulated receivers, hit-adapter choices, Laboratories |
| Arcana | `EchoDevGames.EchoAbilities.Editor.Workshop.EchoAbilitiesWorkshopSetupFacade` | Ability catalogs, owner/loadout defaults, condition/cost/target/effect schemas, clocks and policies, Laboratory |
| The Atlas | `EchoDevGames.EchoWorld.Editor.Workshop.EchoWorldWorkshopSetupFacade` | World catalogs, hierarchy/topology, scene bindings, marker schemas, discovery/fast-travel policy, map metadata, Laboratory |

A facade does not install a provider SDK or adapter unless the Workshop plan explicitly selected that package and displayed the dependency/license implications.

---

## 12. Standalone and Integration Laboratories

| Package | Standalone proof | Integration proof belongs elsewhere |
|---|---|---|
| Convergence | Deterministic simulated provider for sessions, participants, readiness, authority, reconnect, travel/spawn request contracts | One Integration Laboratory per selected networking provider and each advertised gameplay bridge |
| Instinct | Simulated senses, memory, scoring, scheduler, behavior, and navigation providers | Navigation/Behavior/Inference adapter Labs; Fellowship/Vessel/Clash/Arcana/Atlas/Convergence bridge Labs |
| Clash | Simulated receiver/resolver plus separate 2D and 3D hit Labs | Arcana, Fellowship, AI, Vault/equipment, Convergence, Impact bridge Labs |
| Arcana | Simulated owners, resources, conditions, targets, effects, clocks, and authority | Clash effect, Vault cost, Fellowship owner, AI requester, Convergence authority, UI/Input bridge Labs |
| Atlas | Simulated topology/context/planner/marker/state Laboratory | Passage travel, Fellowship spawn, Chronicle save, Convergence shared-world, UI map, AI context bridge Labs |

A provider or bridge sample cannot count as standalone proof for either peer. Showcase scenes come after both standalone and integration evidence.

---

## 13. Diagnostics and privacy

| Package | Prefix | Sensitive exclusions |
|---|---|---|
| Convergence | `EMUL-*` | Credentials, tickets, secrets, private session data, raw provider tokens |
| Instinct | `EAI-*` | Unbounded world observations, private player text, provider internals not approved for export |
| Clash | `ECLASH-*` | Unbounded combat payloads, private player/account data, provider-native secrets |
| Arcana | `EABL-*` | Private targeting payloads, sensitive owner data, arbitrary effect/provider state |
| Atlas | `EWRLD-*` | Private scene paths where redaction is required, arbitrary world-state payload contents, provider secrets |

All histories are bounded. Support exports prefer stable IDs, revisions, states, counts, timings, and diagnostic codes over raw content.

---

## 14. Removal and replacement

### 14.1 Provider or bridge removal

1. Disable the integration path.
2. Settle/cancel work before foreign commit points where possible.
3. Dispose registrations, leases, subscriptions, and callbacks.
4. Remove Integration Laboratory samples.
5. Remove provider/bridge package.
6. Recompile and run both peers’ standalone validation.

### 14.2 Advanced core removal

- Remove provider and bridge packages first.
- Preserve project-owned definitions, configuration, save payloads, aliases, research evidence, and migration records unless the user explicitly prunes them.
- Removing Convergence does not delete multiplayer profiles or provider project settings automatically.
- Removing Instinct does not delete character definitions or world content.
- Removing Clash does not delete project health/stats or hit assets.
- Removing Arcana does not delete game-owned spell/attack content or owner resource data.
- Removing Atlas does not delete scenes, levels, maps, or project world data.
- Reinstallation validates preserved records before reclaiming them.

### 14.3 Provider replacement

Provider replacement is migration work, not a dropdown toggle. The provider-neutral core preserves semantic contracts, but provider-native prefab identities, RPCs, prediction, transport, hosting, authentication, and service configuration require explicit migration, testing, and rollback plans.

---

## 15. Approved collision-review decisions

| Decision ID | Decision | Status |
|---|---|---|
| ADV-D-001 | Advanced cores remain provider-neutral and compile without optional Echo peers or provider SDKs | Approved |
| ADV-D-002 | Participant, network entity, character, runtime actor, control owner, input user, AI agent, ability owner, combat target, and world identities remain separate | Approved |
| ADV-D-003 | Atlas plans semantic travel, Convergence coordinates multiplayer authority/readiness, Passage executes scenes, Atlas commits world context, and Fellowship/project places actors | Approved |
| ADV-D-004 | Instinct proposes semantic decisions; Arcana owns ability activation; Clash owns instantaneous combat resolution and receiver transaction coordination | Approved |
| ADV-D-005 | Clash relation/targetability may inform AI and abilities read-only; Instinct and Arcana do not create competing combat-relation truth | Approved |
| ADV-D-006 | Arcana permits one mutation-capable cost provider per MVP activation and rejects fictional atomicity across independent authorities | Approved |
| ADV-D-007 | Clash defeat outcomes do not directly mutate Fellowship, Path, Ascent, Vault, or Atlas truth; bridges submit separate idempotent consequences | Approved |
| ADV-D-008 | Atlas semantic routes, Instinct local navigation paths, Passage scene transitions, and Vessel movement commands are different contracts | Approved |
| ADV-D-009 | Shared multiplayer saves are Chronicle publications owned by the authoritative host/server; Advanced packages contribute only their payloads | Approved |
| ADV-D-010 | Live sessions, AI observations, combat requests, active abilities, markers, and prepared travel operations remain session-only unless a package explicitly defines a safe detached snapshot | Approved |
| ADV-D-011 | ADR-001 v1.2.0 registers the five Advanced setup facades without approving automated provider installation | Approved |
| ADV-D-012 | No networking, AI, navigation, behavior, inference, hit, world, or hosting provider is approved by documentation alone | Approved |

---

## 16. Open non-blocking questions

These do not block the documentation review but must remain visible:

- Which networking provider, topology, hosting, relay/lobby, prediction, reconnect, and host-migration policies will pass the prototype program?
- Whether FishNet licensing permits the intended public adapter.
- Which navigation and behavior adapters are worth distributing after real implementation evidence.
- Whether Arcana later receives a separate status-effect module and visual authoring adapter.
- Whether large Atlas worlds require hierarchical planning, partitions, streaming, or Addressables adapters.
- Whether AI, Combat, Abilities, and World graduate from candidate foundations to committed implementation packages after prioritization and learning review.
- Exact package IDs and repositories for every pairwise bridge/provider adapter under SFGSS-009.

---

## 17. Gate result

SUITE-DOC-24 passes.

The five Advanced foundations fit the approved Foundation and Expansion authorities. No release-blocking ownership, lifecycle, persistence, or dependency collision remains in documentation.

Implementation remains locked. Provider selection and all empirical claims remain blocked by their research, prototype, compatibility, Laboratory, and release evidence gates.

The next checkpoint is:

```text
SUITE-DOC-25 — SFGSS-006 New-Project Guided Pathways
```

---

## Graph Navigation

#sfgss/integration #sfgss/wave/advanced #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[Integration Specifications/Foundation_Cross-Package_Contract_Matrix|Foundation Cross-Package Contract Matrix]]
- [[Integration Specifications/SFGSS-INT-EXPANSION-001_Expansion_Cross-Package_Contract_Matrix|Expansion Cross-Package Contract Matrix]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
