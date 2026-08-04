# The Atlas - EchoWorld Feasibility Foundation Specification

**Document ID:** SFGSS-PKG-ECHOWORLD  
**Specification version:** 1.0.0  
**Status:** Approved feasibility foundation; EchoWorld remains an Advanced candidate and implementation remains locked  
**Technical package name:** EchoWorld  
**Public title:** The Atlas - World Identity, Topology, and Travel Metadata  
**Package ID:** `com.echodevgames.echo-world`  
**Runtime namespace:** `EchoDevGames.EchoWorld`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Repository:** Planned `EchoDevGames/EchoWorld`  
**Current Notes:** `../Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Required feasibility record:** `../Research Records/SUITE-DOC-22_EchoWorld_Feasibility_and_Boundary_Record.md`  
**Last updated:** August 4, 2026

> “The Atlas names the places, remembers the roads, and marks the doors. The world itself still belongs to the game.”

> **Approval rule:** This document approves the Level 2 provider-neutral foundation for EchoWorld boundaries, identities, topology definitions, active context, travel planning, scene-binding metadata, discovery, visitation, fast-travel policy, entry and spawn marker contracts, map snapshots, versioned world-state seams, diagnostics, Laboratories, and optional bridges. It does not approve implementation, scene loading, procedural generation, level streaming, map rendering, navigation/pathfinding, world simulation, quest state, character spawning, multiplayer transport, cloud services, a universal open-world framework, or empirical performance and compatibility claims. Those remain blocked until SUITE-DOC-33 and later implementation evidence.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial feasibility foundation | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved provider-neutral world identity, topology, travel planning, scene-binding, marker, discovery, map-snapshot, persistence, diagnostics, and bridge foundation | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Atlas - World Identity, Topology, and Travel Metadata  
**Technical identifier:** EchoWorld  
**Flavor line:** The Atlas names the places, remembers the roads, and marks the doors.  
**Plain-language subtitle:** A provider-neutral Unity package foundation for stable world, zone, and location identity; authored topology; travel planning; scene-binding metadata; entry and spawn markers; discovery; visitation; fast-travel policy; map snapshots; versioned world-state contracts; diagnostics; and optional integration seams.

**One-sentence ownership contract:**

> EchoWorld owns stable world, zone, location, connection, binding, marker, and provider identities; immutable world topology definitions; current semantic world context; travel availability and route-plan contracts; scene-to-location binding metadata; entry and spawn marker registries; discovery and visitation truth; fast-travel eligibility; provider-neutral map snapshots; versioned world-state participant contracts; diagnostics, validation, and optional bridges; it does not own scene-loading execution, level art or procedural generation, world simulation, character spawning, movement, pathfinding, navigation meshes, objectives, inventory, dialogue, camera movement, production maps, save files, multiplayer transport, or one game's world rules and content.

### 1.1 Elevator summary

The Atlas gives a project stable names and relationships for the places that exist without forcing those places to be scenes, levels, zones, map tiles, streaming cells, or one particular open-world architecture. A game can define a world, group it into zones, define semantic locations, connect those locations with authored travel links, map locations to opaque scene-binding tokens, register runtime arrival markers, record discovery and visitation, prepare a travel plan, and expose map-ready snapshots.

The package stops before execution authority. The Passage still loads and unloads scenes. The Fellowship or project code still spawns characters. The Eye still moves cameras. The Path still owns objectives. The Chronicle still writes save files. The Convergence still decides shared-world network authority. EchoWorld tells those systems *where*, *which*, and *how the semantic topology connects*; it does not perform their work.

Definitions remain immutable. Current location, discovery, visit records, marker registrations, travel-plan histories, and provider state remain runtime-owned. Durable world snapshots contain only stable IDs, versions, approved core state, and opaque participant records. Scene objects, transforms, provider handles, pending scene operations, and transient marker leases never enter durable data.

### 1.2 Why this belongs in The Sperk's Forge

Existing projects repeatedly scatter location truth across build indexes, scene-name strings, spawn-point tags, checkpoint scripts, quest flags, minimap code, and save payloads. A renamed scene can break a save. A direct-scene test can start with no idea where the player is. A map screen invents a second location database. Fast travel, respawning, objectives, and multiplayer each form their own incompatible interpretation of “where.”

The Atlas extracts the shared semantic layer. It provides stable IDs and topology once, then lets Passage, Fellowship, Objectives, Camera, Save, UI, AI, and Multiplayer integrate without becoming circular authorities.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Always paired with “World Identity, Topology, and Travel Metadata.” |
| Setup guidance/tooltips | Yes | Must explain worlds, zones, locations, links, bindings, and markers plainly. |
| Samples | Optional | Verse-flavored places may appear but remain replaceable. |
| Runtime API/type names | No lore-only names | Use `WorldId`, `LocationDefinition`, `WorldTravelPlan`, and `WorldMarkerSnapshot`. |
| Project data | No required Verse content | Games own geography, lore, scenes, names, art, travel rules, and world-state semantics. |

---

## 2. Problem Statement

### 2.1 Current problem

A Unity project often begins with scene names as location identity. Later, objectives need stable locations, save files need to remember where the player was, character spawners need entry points, maps need node metadata, fast travel needs availability checks, and multiplayer needs one authoritative world context. Each system adds a partial location table. The tables drift, raw strings leak into runtime APIs, and scenes become accidental databases.

A reusable world package must provide stable semantic identity without taking over scenes or level design. It must distinguish a world from a zone, a location from a scene, a scene binding from a travel connection, a spawn marker from a character spawner, discovery from progression access, and world-state contracts from save transport.

### 2.2 Evidence from existing work

| Source | Existing pattern or need | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Boot, menu, level, password progression, spawn points, rescue locations, and direct-scene testing | Clear small-project destinations and role-based entry points | Replace scene strings and disconnected spawn logic with stable location/binding contracts |
| Echo Systems Lab | Hub, mission terminals, trials, scene loader, mission IDs, and unlock records | Stable IDs and explicit scene travel | Separate mission identity, location identity, and scene execution |
| Hackulos | Outdoor zones, quest NPCs, vendors, enemies, corpse recovery, and future fast travel | Data-driven RPG geography and authored spawn points | Keep RPG content outside the general package while sharing stable topology |
| The Passage | Validated asynchronous scene travel and route execution | One scene-transition authority | Let Atlas describe semantic destinations without duplicating scene loading |
| The Fellowship | Character spawning, replacement, respawn, and control handoff | Stable character identity and provider-based spawning | Select world entry markers without making World own characters |
| The Chronicle | Versioned save participants and recovery | Durable detached state | Keep files/slots outside Atlas while preserving world snapshots |
| The Path | Location-based goals, discovery, and travel objectives | Objective truth and reward ledgers | Observe world events instead of storing objective state inside Atlas |
| The Convergence | Shared sessions, synchronized travel seams, spawn ownership, and authority | Provider-neutral authority | Keep network IDs and transport outside world identity |

### 2.3 Consequences of doing nothing

- Scene paths and build indexes become accidental durable identifiers.
- Save migrations become fragile when scenes or folders are renamed.
- Maps, objectives, travel menus, AI, and spawners maintain duplicate location databases.
- Direct-scene testing starts with inconsistent world context.
- Fast travel mixes discovery, unlocks, scene loading, and character spawning in one manager.
- Multiplayer clients can disagree about semantic location even when the same scene is loaded.
- World-state persistence becomes an untyped bag of flags with no owner or migration path.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide stable, migration-safe world, zone, location, connection, binding, and marker identities.
- Keep semantic location separate from Unity scenes and scene paths.
- Describe immutable world topology and deterministic travel plans.
- Record active context, discovery, visitation, and fast-travel eligibility without absorbing progression or objectives.
- Provide scene-local entry and spawn marker registration/selection contracts.
- Expose provider-neutral map and world-state snapshots.
- Remain independently useful without Passage, Fellowship, Chronicle, UI, Camera, Objectives, or Multiplayer.
- Make invalid topology, missing providers, unavailable destinations, and stale revisions diagnosable.

### 3.2 Non-goals

- Scene loading, unloading, additive streaming, or transition presentation.
- Level generation, procedural worlds, terrain, art, lighting, navigation meshes, or streaming-cell management.
- Character spawning, movement, camera movement, map rendering, or waypoint UI.
- Quest/objective state, dialogue, inventory, combat, abilities, or AI behavior.
- Save files, slots, cloud synchronization, or multiplayer transport.
- A universal world simulation, time-of-day system, weather system, or global flag database.
- A universal pathfinder for movement through physical space.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Needs stable locations across a few scenes | Define one world, zones, locations, bindings, and markers in the Laboratory |
| Gameplay programmer | Needs travel, respawn, or discovery | Query stable locations, prepare plans, select markers, and observe committed events |
| Designer | Authors geography and travel links | Edit validated topology assets without touching runtime code |
| UI/map developer | Builds a map or travel screen | Consume immutable topology/discovery/availability snapshots without owning world truth |
| Save developer | Persists world state | Store detached versioned snapshots through Chronicle or project transport |
| Multiplayer developer | Synchronizes world context | Replicate stable IDs and revisions through an authority bridge |
| Tester | Reproduces topology and travel edge cases | Simulate broken links, missing bindings, stale markers, and failed imports in isolation |

### 3.4 Measurable success criteria

- Installs into a clean supported Unity project with zero compile errors.
- Core world topology, discovery, marker, and planning workflows run without another Echo runtime package.
- Definitions remain unchanged after Play Mode and tests.
- Location identity survives display-name, asset-path, and scene-path changes through stable IDs and aliases.
- A direct, multi-leg, denied, unavailable, and no-route travel case returns deterministic structured results.
- Marker selection rejects stale handles and cleans scene-owned registrations.
- World-state import is atomic and preserves unknown records.
- Optional bridges may be removed without breaking the core or deleting project-owned state.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Unity gameplay and systems programmers.
- Level, world, quest, and technical designers.
- UI/map, save, multiplayer, AI, and build-tool developers.
- Testers validating world topology and travel metadata.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| EWRLD-UC-001 | Query current semantic location | Project code | Authority initialized | Immutable world context snapshot | MVP |
| EWRLD-UC-002 | Plan direct travel | Project/Passage bridge | Authored valid connection | Allowed plan with destination binding and marker criteria | MVP |
| EWRLD-UC-003 | Plan multi-leg route | Project/UI | Connected topology | Deterministic ordered legs or NoRoute result | MVP |
| EWRLD-UC-004 | Register arrival marker | Scene component/provider | Known location and marker ID | Generational marker handle | MVP |
| EWRLD-UC-005 | Discover and visit location | Project/gameplay code | Known identity and authority | Idempotent discovery plus committed visit metadata | MVP |
| EWRLD-UC-006 | Build map snapshot | UI/project presenter | Valid catalog | Bounded topology and discovery snapshot | MVP |
| EWRLD-UC-007 | Save world state | Chronicle bridge/project | Safe detached state | Versioned snapshot with unknown-record preservation | MVP |
| EWRLD-UC-008 | Synchronize shared world context | Convergence bridge | Host/server authority | Replicated stable IDs and revisioned snapshot | Later bridge |

### 4.3 Explicitly unsupported use cases

- Treating scene names, asset GUIDs, display names, hierarchy paths, or build indexes as domain World IDs.
- Calling `SceneManager` directly from the neutral core.
- Spawning characters or moving cameras from a travel-plan result.
- Storing scene objects, Transforms, marker handles, or pending travel operations in durable snapshots.
- Using EchoWorld as a universal quest flag or arbitrary global variable database.
- Allowing clients to commit shared-world context without authoritative validation.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- World, zone, location, travel-connection, scene-binding, marker, discovery, and provider identity contracts.
- Immutable topology and map-layout definitions.
- Current semantic world context and revision.
- Travel availability, route planning, plan fingerprints, and structured results.
- Runtime entry/spawn marker registration and deterministic selection.
- Discovery and visitation records.
- Fast-travel semantic eligibility.
- Versioned core world-state snapshots and participant routing contracts.
- Semantic events, diagnostics, setup, validation, and standalone Laboratories.

### 5.2 The package does not own

- Scene-transition execution, loading screens, additive-scene lifetime, or build settings.
- Character identity, spawn/despawn execution, controller ownership, or respawn rules.
- Camera control, map rendering, navigation, or minimap presentation.
- Objectives, quests, dialogue, inventory, combat, abilities, crafting, or progression unlock truth.
- Save files, settings files, cloud providers, or multiplayer transport.
- Level content, geometry, procedural generation, streaming cells, weather, day/night, ecology, or world simulation.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | Atlas interaction |
|---|---|---|
| Scene transitions | The Passage | Optional bridge executes prepared semantic travel plan and reports result |
| Character roster/spawn | The Fellowship/project | Optional bridge selects marker and requests spawn/relocation |
| Save files and slots | The Chronicle | Stores detached Atlas snapshot through participant bridge |
| Objectives and quests | The Path | Observes discovery, visit, and travel events |
| Progression access | The Ascent/project | Read-only condition provider may allow or deny destination access |
| Map and travel UI | The Looking Glass/project | Consumes snapshots and submits commands |
| Camera and map framing | The Eye/project | Consumes layout/location metadata only |
| Localization | Many Tongues | Resolves project-owned display references |
| AI semantic location | Instinct/project | Reads location/zone context; does not alter topology directly |
| Multiplayer authority | The Convergence | Validates and replicates stable IDs, revisions, and shared state |
| Build scene validation | The Foundry | Validates scene bindings and referenced destinations |

### 5.4 Boundary tests

1. Does the feature describe *where* and *how places connect*, or does it execute another system's behavior?
2. Could the same location exist across several scenes or one scene contain several semantic locations?
3. Does the proposal rely on raw scene paths, GameObjects, or build indexes as durable truth?
4. Would the core still compile and prove itself without Passage, Characters, UI, Save, or Multiplayer?
5. Is a map presentation concern being mistaken for world authority?
6. Is an arbitrary project flag trying to hide inside the world-state contract?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

The package must:

- Compile with only declared Unity/platform dependencies.
- Initialize without First Light.
- Function without Passage, Fellowship, Chronicle, UI, Camera, Objectives, Progression, AI, Localization, Foundry, or Multiplayer.
- Use simulated scene, condition, marker, state, and authority providers in its Laboratory.
- Avoid direct references to project assemblies.
- Avoid a mandatory input asset, EventSystem, map prefab, navigation package, Addressables dependency, or networking SDK.
- Keep configured project world data outside immutable package source.
- Expose service injection and explicit provider registration.
- Fail visibly and safely when an optional collaborator is missing.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Planned evidence |
|---|---|---|
| Installed alone | Core catalog, service, marker registry, discovery, planner, snapshots, and Laboratory compile | EWRLD-T-PKG range |
| Enter Laboratory directly | Development initializer creates only Atlas authority | EWRLD-LAB-001 onward |
| Passage absent | Travel plans remain queryable but no scene execution occurs | Travel/scene tests |
| Fellowship absent | Marker selection works with simulated consumers | Marker tests |
| Chronicle absent | Export/import API remains callable by project transport | Persistence tests |
| UI absent | No production presentation is required | Map snapshot tests |
| Convergence absent | Local/single-player authority works | Multiplayer boundary tests |
| Samples deleted | Runtime and Editor assemblies compile | Packaging tests |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| UnityEngine.CoreModule | Platform | Yes | Unity baseline | Runtime lifecycle, ScriptableObjects, vectors, poses, and serialization | Package cannot compile without Unity |
| Unity Test Framework | Test-only | Yes for package tests | Evidence-pending | Automated EditMode/PlayMode tests | Runtime unaffected when test assembly absent |

### 6.4 Forbidden dependencies

- Direct references to another Sperk's Forge runtime package from the neutral core.
- Direct `SceneManager` travel execution in the core.
- Required navigation, map, Addressables, localization, networking, or UI provider.
- Project assemblies, scene names, tags, layers, or build indexes hidden in runtime assumptions.
- Samples or Editor assemblies at runtime.
- Reflection-based open provider discovery.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
|---|---|---|---|---:|---|
| EWRLD-CAP-001 | Duplicate-safe world authority | Claim one application-session world authority before catalog, provider, marker, or state registration | Approved | Yes | Runtime |
| EWRLD-CAP-002 | Stable world identities | World, zone, location, connection, binding, marker, discovery, and provider IDs | Approved | Yes | Runtime/Data |
| EWRLD-CAP-003 | World topology | Immutable catalogs describing hierarchy, locations, directed/bidirectional links, tags, and map metadata | Approved | Yes | Runtime/Data |
| EWRLD-CAP-004 | Active world context | Committed current world, zone, and location with revisioned snapshots | Approved | Yes | Runtime |
| EWRLD-CAP-005 | Travel availability | Structured Allowed, Denied, and Unavailable evaluation | Approved | Yes | Runtime |
| EWRLD-CAP-006 | Travel planning | Deterministic direct and multi-leg route planning without scene execution | Approved | Yes | Runtime |
| EWRLD-CAP-007 | Scene binding tokens | Stable location-to-scene binding records independent from Unity asset GUIDs | Approved | Yes | Runtime/Data |
| EWRLD-CAP-008 | Entry-marker registry | Scene-local arrival marker registrations and deterministic selection | Approved | Yes | Runtime |
| EWRLD-CAP-009 | Spawn-marker registry | Scene-local spawn marker registrations and deterministic selection | Approved | Yes | Runtime |
| EWRLD-CAP-010 | Discovery state | Idempotent world, zone, location, and connection discovery records | Approved | Yes | Runtime/Data |
| EWRLD-CAP-011 | Visit records | First visit, last visit, and bounded visit-count metadata | Approved | Yes | Runtime/Data |
| EWRLD-CAP-012 | Fast-travel policy | Discovery, eligibility, condition, and arrival-marker evaluation | Approved | Yes | Runtime |
| EWRLD-CAP-013 | World-state participants | Versioned provider records for project-owned world state without owning their semantics | Approved | Yes | Runtime |
| EWRLD-CAP-014 | State snapshots | Detached export/import, migrations, aliases, and unknown-record preservation | Approved | Yes | Runtime/Data |
| EWRLD-CAP-015 | Map snapshots | Provider-neutral topology, discovery, availability, and layout data for presenters | Approved | Yes | Runtime |
| EWRLD-CAP-016 | Condition providers | Read-only travel, discovery, and fast-travel condition registrations | Approved | Yes | Runtime |
| EWRLD-CAP-017 | Semantic events | Context, discovery, visit, travel-plan, marker, and state-import events | Approved | Yes | Runtime |
| EWRLD-CAP-018 | Diagnostics | Structured health, topology, provider, marker, plan, and state information | Approved | Yes | Runtime/Editor |
| EWRLD-CAP-019 | Authoring tools | Catalog, topology, binding, marker, reachability, and ID validation | Approved | Yes | Editor |
| EWRLD-CAP-020 | Standalone Laboratories | Simulated scene, travel, marker, discovery, persistence, and map providers | Approved | Yes | Sample/Test |
| EWRLD-CAP-021 | Passage bridge seam | Optional travel execution and scene-activation handoff | Approved | No | Bridge |
| EWRLD-CAP-022 | Character bridge seam | Optional spawn/entry ownership and active-character location updates | Approved | No | Bridge |
| EWRLD-CAP-023 | Chronicle bridge seam | Optional world-state persistence transport | Approved | No | Bridge |
| EWRLD-CAP-024 | Multiplayer authority seam | Optional shared-world authority, replication, and reconciliation | Approved | No | Bridge |

### 7.2 MVP capability set

The MVP is one stable world catalog, one semantic current context, deterministic travel planning, scene-binding tokens, runtime marker registration/selection, discovery and visits, fast-travel eligibility, map snapshots, versioned state export/import, diagnostics, Editor validation, and one standalone Laboratory.

### 7.3 Later capability set

- Streaming-cell and region-provider adapters.
- Addressables or asset-bundle scene-reference providers.
- Hierarchical or weighted travel cost algorithms beyond the MVP planner.
- Runtime world-content registration for generated worlds.
- Multi-world portals with provider-specific transition semantics.
- Minimap/map layout authoring adapters.
- Platform achievement or analytics observers.
- Large-world spatial databases and server sharding.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Full procedural generation | Rejected from core | Level/content generation is a separate authority | Dedicated generation package proposal |
| Scene streaming manager | Rejected from core | Passage or provider-specific streaming owns execution | Explicit streaming adapter design |
| Universal global flag database | Rejected | Untyped world god-object and persistence overlap | Never without a typed dedicated standard |
| Physical navigation/pathfinding | Rejected from core | Instinct/navigation providers own movement planning | Optional semantic-travel bridge only |
| Weather and time of day | Deferred to project/package | Different lifecycle and simulation authority | Dedicated specification |
| Production map UI | Rejected from core | Looking Glass/project presentation authority | Separate UI bridge/sample |
| Cloud world state | Deferred | Provider, privacy, conflict, and cost concerns | Approved provider adapter |
| Client-authoritative shared world | Rejected | Security and consistency risk | None for shared authoritative state |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Catalogs, worlds, zones, locations, links, scene bindings, marker criteria, map layout, policies | Live context, marker instances, scene objects, discovery mutation |
| Runtime state/behavior | Authority, context, topology index, planner, discovery, visits, marker registries, participants, histories | Editor APIs, production UI, scene-loading execution |
| Presentation/feedback | Optional map/travel presenters and sample readouts | Authoritative world, discovery, travel, or save truth |

### 8.2 Component topology

```text
EchoWorldRoot
├── WorldCatalogIndex
├── WorldContextAuthority
├── WorldTravelPlanner
├── WorldDiscoveryService
├── WorldMarkerRegistry
├── WorldStateParticipantRegistry
├── WorldMapSnapshotBuilder
├── WorldDiagnosticsState
└── explicit optional providers/bridges
```

No child becomes an independent persistent singleton.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | Yes for the default application-session service; service injection remains supported |
| Root type | `EchoWorldRoot` |
| Duplicate behavior | Reject duplicate before catalog indexing, provider registration, marker registration, or state mutation |
| Initialization trigger | Explicit `Initialize` or standalone `Awake` path with project configuration |
| Shutdown behavior | Cancel prepared work, unregister providers/markers, clear transient state, publish shutdown diagnostics |
| Direct-scene behavior | Development initializer may create only the configured Atlas authority |
| Test injection seam | `IEchoWorldService`, clock, route planner, scene resolver, state participants, and authority provider |

### 8.4 Identity model

The package distinguishes:

- `WorldId`: one authored world or large semantic world-space.
- `ZoneId`: one authored grouping within a world.
- `LocationId`: one stable semantic place.
- `TravelConnectionId`: one authored topology edge.
- `SceneBindingId`: one semantic binding record between a location and opaque scene reference.
- `EntryMarkerId`: one authored arrival point identity.
- `SpawnMarkerId`: one authored spawn-point identity.
- `WorldStateProviderId`: one versioned project/provider world-state payload owner.
- Runtime registration IDs and provider handles: session-only.

Unity asset GUIDs and scene paths are Editor/source identities. They never substitute for these domain IDs.

### 8.5 Topology model

A world contains zones; a zone contains locations. Travel connections form a separate graph whose endpoints are locations. A connection may be directed or bidirectional and may contain tags, travel mode IDs, authored cost metadata, condition references, visibility policy, and destination entry-marker criteria.

Hierarchy is for organization and semantic context. Travel reachability comes from the connection graph, not implied parent-child adjacency.

### 8.6 Active context lifecycle

The authority publishes one semantic context snapshot containing current world, zone, location, optional scene binding, context source, and revision. External travel execution prepares first, executes elsewhere, then requests a context commit with the expected previous revision and travel-plan fingerprint.

A context commit is atomic. Failed validation retains the prior context. Direct-scene development may establish context through an explicit binding registration but never guesses from a raw active-scene name.

### 8.7 Travel planning

1. Validate origin and destination identities.
2. Evaluate request authority and expected revisions.
3. Filter connections by direction, travel mode, discovery policy, and condition providers.
4. Search the bounded topology graph with deterministic ordering.
5. Resolve destination scene-binding token and marker criteria when required.
6. Produce an immutable `WorldTravelPlan` with ordered legs and fingerprint.
7. Publish plan-prepared diagnostics.
8. Do **not** load a scene or move a character.

The MVP planner prefers authored cost then stable connection ID for ties. More advanced cost models remain provider extensions.

### 8.8 Scene-location binding

A scene binding maps a stable `SceneBindingId` and `LocationId` to an opaque `WorldSceneReferenceToken`. The token is resolved only by an explicit project resolver or Passage bridge. Editor tooling may store a Unity source GUID and derive runtime metadata, but runtime APIs do not expose AssetDatabase GUIDs as domain identity.

One scene may contain several semantic locations. One location may have several bindings for variants, interiors, platforms, or development scenes.

### 8.9 Marker registry

Scene-local components or providers register entry and spawn markers with stable authored marker IDs, location IDs, poses, tags, priority, capacity, and owner scene/runtime lifetime. Registrations return generational handles. Selection is deterministic and rejects stale registrations.

The registry chooses a marker snapshot. It does not instantiate, teleport, possess, or animate a character.

### 8.10 Discovery, visitation, and fast travel

Discovery records semantic knowledge. Visitation records committed presence. Neither automatically means progression access, objective completion, or fast-travel permission.

Fast travel combines:

- Known destination identity.
- Authored fast-travel policy.
- Discovery/visit requirements.
- Read-only project condition providers.
- Valid destination binding and arrival marker criteria.
- Optional authority policy.

The result is a structured semantic plan, not scene execution.

### 8.11 World-state participant model

Atlas owns core context, discovery, and visitation records. Project systems may register one stable provider ID and export an opaque versioned detached payload. The package routes, preserves, migrates through the provider, and applies only after validation. It does not understand a door's open state, a harvested tree, a defeated boss, or a moved crate.

Unknown provider records are preserved. They are never silently pruned because an optional package is absent.

### 8.12 Failure model

| Code | Severity | Failure | Detection point | Runtime fallback |
|---|---|---|---|---|
| EWRLD-001 | Blocker | Duplicate authoritative root | Claim runtime before registrations | Duplicate rejects itself before side effects |
| EWRLD-002 | Blocker | Missing or invalid world catalog | Initialization preflight | Authority remains unavailable with structured report |
| EWRLD-003 | Error | Duplicate domain ID | Editor/runtime validation | Conflicting record is rejected |
| EWRLD-004 | Error | Broken hierarchy parent | Catalog validation | Invalid world, zone, or location is excluded |
| EWRLD-005 | Error | No route exists | Travel planner | Request returns Denied/NoRoute without mutation |
| EWRLD-006 | Warning | Required condition provider unavailable | Availability evaluation | Request returns Unavailable |
| EWRLD-007 | Error | Scene binding unresolved | Travel handoff | Plan remains descriptive and execution is refused |
| EWRLD-008 | Warning | Arrival marker unavailable | Marker selection | Request returns Unavailable or uses approved fallback policy |
| EWRLD-009 | Warning | Stale context or marker revision | Command validation | Request is rejected without mutation |
| EWRLD-010 | Error | World-state migration failed | Prepared import | Source snapshot remains untouched |
| EWRLD-011 | Warning | Unknown participant record | Import | Record is preserved opaquely |
| EWRLD-012 | Error | Provider throws or times out | Provider boundary | Provider fails in isolation; authority continues |
| EWRLD-013 | Warning | Client lacks shared-world authority | Authority gate | Request rejected and authoritative snapshot requested |
| EWRLD-014 | Warning | Topology capacity exceeded | Registration/planning | New work is rejected with bounded diagnostics |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoWorldConfiguration` | Catalog, limits, defaults, policies, diagnostics | Configuration identity | No | Yes |
| `WorldCatalog` | Worlds, zones, locations, links, bindings, aliases | Yes | No | Yes |
| `WorldDefinition` | World metadata and tags | `WorldId` | No | Yes |
| `ZoneDefinition` | Zone metadata and parent | `ZoneId` | No | Yes |
| `LocationDefinition` | Location metadata, policies, and layout | `LocationId` | No | Yes |
| `TravelConnectionDefinition` | Topology edge and travel policy | `TravelConnectionId` | No | Yes |
| `SceneLocationBindingDefinition` | Location-to-scene semantic binding | `SceneBindingId` | No | Yes |
| `WorldMapLayoutDefinition` | Optional map coordinates and presentation metadata | Location/connection IDs | No | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `WorldCatalogIndex` | Root | Initialization/session | Rebuilt on approved catalog reload | Derived, not saved |
| `WorldContextState` | Root | Session | Explicit clear/shutdown | Optional stable IDs only |
| `WorldDiscoveryState` | Root | Profile/slot or session | Explicit reset/import | Versioned durable DTO |
| `WorldVisitState` | Root | Profile/slot or session | Explicit reset/import | Versioned durable DTO |
| `WorldMarkerRegistryState` | Root | Scene/session | Handle disposal/scene unload | Never saved |
| `PreparedTravelPlanState` | Root | Bounded request lifetime | Completion/cancel/timeout | Never saved |
| `WorldStateParticipantRecordSet` | Root | Session/durable snapshot | Import/reset | Versioned opaque records |
| `WorldDiagnosticHistory` | Root | Bounded session | Reset/shutdown | Support export only |

### 9.3 Stable identifiers

IDs use the package-qualified stable ID rules from SFGSS-003. Display names, scene names, folder paths, hierarchy paths, and Unity asset GUIDs remain separate metadata. Duplicate IDs block publication. Approved aliases support rename/migration; tombstones preserve removed identities for old saves and diagnostics.

### 9.4 ScriptableObject safety

Definitions remain immutable during runtime. Discovery, visits, current context, plan histories, marker registrations, provider records, and revisions never write back into ScriptableObjects.

### 9.5 Serialization and migration

`WorldStateSnapshot` includes:

- Document format version.
- Catalog compatibility/fingerprint information.
- Optional current world/zone/location stable IDs.
- Discovery and visitation records.
- Provider records keyed by stable provider ID and schema version.
- Unknown records preserved opaquely.
- Integrity and migration diagnostics.

Migrations are contiguous and forward-only. Import validates before publication and preserves the source on failure.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| EchoWorldRoot | MonoBehaviour authority | Owns service lifecycle, configuration, providers, registries, and shutdown | Scene/prefab or injected host |
| IEchoWorldService | Interface | Queries topology, context, discovery, plans, markers, and snapshots | Root/injected implementation |
| WorldCatalog | ScriptableObject | Project-owned immutable collection of worlds, zones, locations, connections, and bindings | Project asset |
| WorldDefinition | Serializable definition | World metadata, tags, source display references, and map metadata | Catalog-owned |
| ZoneDefinition | Serializable definition | Zone parent, metadata, tags, and map grouping | Catalog-owned |
| LocationDefinition | Serializable definition | Location parent, travel/discovery policy, tags, and layout metadata | Catalog-owned |
| TravelConnectionDefinition | Serializable definition | Directed or bidirectional topology edge with policy and costs metadata | Catalog-owned |
| SceneLocationBindingDefinition | Serializable definition | Stable binding ID joining a location to an opaque scene reference token | Catalog-owned |
| WorldContextSnapshot | Immutable DTO | Current world, zone, location, binding, and revision | Runtime-owned |
| WorldTravelRequest | Immutable DTO | Origin, destination, travel mode, preferences, and expected revisions | Caller-owned |
| WorldTravelPlan | Immutable DTO | Validated ordered legs, destination binding, arrival marker criteria, and plan fingerprint | Runtime result |
| WorldTravelResult | Struct/record | Allowed, Denied, Unavailable, stale, and failure outcomes | Runtime result |
| WorldMarkerRegistration | Generational handle | Lifetime of one runtime entry or spawn marker | Scene/provider-owned |
| WorldMarkerSnapshot | Immutable DTO | Marker identity, location, pose, tags, priority, capacity, and revision | Runtime-owned |
| WorldDiscoverySnapshot | Immutable DTO | Discovered and visited world topology records | Runtime-owned |
| WorldStateSnapshot | Versioned DTO | Core discovery/context records and opaque participant payloads | Detached durable data |
| IWorldConditionProvider | Interface | Read-only travel, discovery, or fast-travel evaluation | Explicit registration |
| IWorldStateParticipant | Interface | Exports, migrates, validates, and applies one versioned world-state payload | Explicit registration |
| IWorldSceneResolver | Interface | Resolves opaque scene reference tokens for project or Passage handoff | Optional provider |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/main-loop rule |
|---|---|---|---|---|
| `WorldInitializeResult Initialize(WorldCatalog catalog)` | Initialize and validate one world catalog | No active initialized authority | Structured success/failure; no partial publication | Main thread |
| `WorldContextSnapshot CurrentContext` | Read current world context | Initialized | Immutable snapshot | Any caller; snapshot creation main thread |
| `WorldQueryResult<WorldDefinitionSnapshot> GetWorld(WorldId id)` | Query one world definition | Initialized | Found/Unknown/Unavailable | Main thread |
| `WorldTravelResult EvaluateTravel(WorldTravelRequest request)` | Evaluate travel without mutation | Valid request | Allowed/Denied/Unavailable with reasons | Main thread, pure provider calls |
| `Awaitable<WorldTravelPlanResult> PrepareTravelAsync(WorldTravelRequest request, CancellationToken token)` | Build a route and destination handoff plan | Initialized and not shutting down | Plan or structured failure | Main thread entry; detached search optional |
| `WorldContextChangeResult CommitContext(WorldContextChangeRequest request)` | Commit active world/zone/location after external travel execution | Validated handoff and expected revision | Atomic context revision or rejection | Main thread |
| `WorldDiscoveryResult Discover(WorldDiscoveryRequest request)` | Discover one topology identity | Known identity and authority | Idempotent committed result | Main thread |
| `WorldMarkerRegistration RegisterMarker(WorldMarkerRegistrationRequest request)` | Register scene-local entry or spawn marker | Known location and valid ID | Generational handle or failure | Main thread |
| `WorldMarkerSelectionResult SelectMarker(WorldMarkerSelectionRequest request)` | Choose deterministic marker | Valid criteria | Selected/Unavailable/Denied | Main thread |
| `WorldStateSnapshot ExportState()` | Export detached durable state | No unsafe import in progress | Versioned snapshot | Main thread capture |
| `Awaitable<WorldImportResult> ImportStateAsync(WorldStateSnapshot snapshot, CancellationToken token)` | Prepare, migrate, validate, and publish world state | Initialized and no conflicting import | Atomic result or source-preserving failure | Main thread plus detached work |
| `WorldMapSnapshot CreateMapSnapshot(WorldMapSnapshotRequest request)` | Create provider-neutral map data | Initialized | Bounded immutable snapshot | Main thread |

### 10.3 Events and callbacks

| Event | Timing | Payload | Listener assumptions |
|---|---|---|---|
| `ContextChanged` | After atomic context commit | Previous/current snapshots and causality | Listener failure cannot undo context |
| `LocationDiscovered` | After discovery commit | Identity, source, and new snapshot revision | Idempotent repeats do not republish unless configured |
| `LocationVisited` | After visit commit | Visit metadata and revision | No objective or reward implication |
| `TravelPlanPrepared` | After plan creation | Immutable plan summary | Does not mean execution succeeded |
| `MarkerRegistryChanged` | After marker add/remove | Location, marker type, and revision | Scene objects are not exposed outside validated snapshot |
| `WorldStateImported` | After successful publication | Migration and import summary | Failed imports publish a separate failure result |

Events occur after authoritative state changes. Presentation listeners are optional.

### 10.4 Async and cancellation policy

Travel preparation and state import may use `Awaitable`/provider-neutral async seams. Cancellation is cooperative before publication. Once context or state has committed, cancellation cannot silently roll it back. Scene loading belongs to Passage and follows Passage's own cancellation policy.

### 10.5 API ergonomics

The novice path configures one catalog and uses the Laboratory. The advanced path injects service, planner, resolver, condition, state, authority, and bridge providers. Convenience access never becomes the only testable API.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install package.
2. Open **Sperk's Forge > The Atlas > Setup**.
3. Create project-owned configuration and empty catalog.
4. Author one world, zone, and location.
5. Add scene binding and marker sample definitions.
6. Preview changes and validation results.
7. Open the standalone Atlas Laboratory.
8. Run topology and binding validation.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create configuration | Project asset | Nothing existing | Yes | Unity Undo | Setup receipt |
| Create empty catalog | Project asset | Configuration reference after approval | Yes | Unity Undo | Setup receipt |
| Create Laboratory sample copy | Sample/project content | Nothing outside selected path | Yes | Delete generated folder | Generation receipt |
| Repair missing root/config reference | Scene/prefab assignment | Selected object only | Yes | Unity Undo | Repair report |
| Regenerate scene-binding metadata | Binding source metadata | Selected binding records | Yes with preview | Backup/Undo | Diff report |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Atlas Setup Window | Novice/programmer | Create configuration, catalog, root, and Laboratory | No |
| World Catalog Editor | Designer | Edit hierarchy, connections, bindings, aliases, and layout metadata | No |
| Topology Graph View | Designer/tester | Inspect reachability, direction, orphan nodes, and routes | No |
| Marker Registry Monitor | Tester | Inspect runtime markers, owners, stale handles, and capacity | Development runtime only |
| World State Inspector | Programmer/tester | Inspect detached snapshots and provider records without project secrets | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| EWRLD-VAL-001 | Missing configuration or catalog | Blocker | Yes | Yes when creating new assets |
| EWRLD-VAL-002 | Duplicate domain ID | Blocker | Guided | No after durable release |
| EWRLD-VAL-003 | Broken world/zone/location parent | Error | Guided | No |
| EWRLD-VAL-004 | Unreachable required location | Warning/Error by policy | Report | No |
| EWRLD-VAL-005 | Broken connection endpoint | Error | Guided | No |
| EWRLD-VAL-006 | Missing or duplicate scene binding | Warning/Error | Guided | No |
| EWRLD-VAL-007 | Asset GUID used as domain ID | Error | Guided | No |
| EWRLD-VAL-008 | Marker references unknown location | Error | Guided | No |
| EWRLD-VAL-009 | Provider ID/schema collision | Blocker | Report | No |
| EWRLD-VAL-010 | Catalog exceeds configured bound | Warning/Blocker | Report | No |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

- Embedded package during development.
- Local path or tarball UPM installation.
- Git URL after repository release.
- Workshop selection after an Atlas setup facade is implemented.

All routes remain `Not run` until executed.

### 12.2 Minimal scene setup

- One `EchoWorldRoot` or injected host.
- One `EchoWorldConfiguration` referencing a valid project-owned catalog.
- Optional scene-local marker registration components.
- No required UI, input, camera, scene-flow, save, or networking package.

### 12.3 Boot-scene setup

A production project may place the root in its Boot scene or have First Light initialize it through an optional startup step. First Light is not required.

### 12.4 Direct-scene setup

A development initializer may create the configured authority and establish context from one explicit development binding. It must not infer context from a scene name and must reject duplicates before side effects.

### 12.5 Scene isolation rule

The Atlas Laboratory uses simulated scene tokens and marker owners. It proves semantic world behavior without unrelated Echo package code.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Test Lab purpose

The **Atlas World Topology Laboratory** proves one world catalog, hierarchy, route planning, bindings, markers, discovery, fast travel, map snapshots, state import/export, diagnostics, and failure recovery without Passage, Characters, UI, Save, or Multiplayer.

### 13.2 Required Laboratory contents

- Minimal world with two zones and several locations.
- Directed, bidirectional, blocked, and alternate travel links.
- Simulated scene-binding resolver.
- Simulated entry/spawn marker registrations.
- Simulated condition, state, and authority providers.
- Topology and context readout.
- Discovery, visit, travel-plan, marker, map, import, and reset controls.
- Duplicate, stale, unavailable, no-route, failed-migration, and provider-exception cases.
- No restricted or project-owned content.

### 13.3 Laboratory acceptance checklist

| Test ID | Category | Action | Type | Status |
|---|---|---|---|---|
| EWRLD-LAB-001 | Authority and lifecycle | Create one world root and initialize a valid world catalog | Manual/automated | Not run |
| EWRLD-LAB-002 | Authority and lifecycle | Introduce a duplicate world root before initialization | Manual/automated | Not run |
| EWRLD-LAB-003 | Authority and lifecycle | Introduce a duplicate world root after initialization | Manual/automated | Not run |
| EWRLD-LAB-004 | Authority and lifecycle | Disable and re-enable the authoritative root | Manual/automated | Not run |
| EWRLD-LAB-005 | Authority and lifecycle | Reset the Laboratory and clear bounded runtime state | Manual/automated | Not run |
| EWRLD-LAB-006 | Authority and lifecycle | Shut down while a travel plan is being prepared | Manual/automated | Not run |
| EWRLD-LAB-007 | Authority and lifecycle | Dispose provider registrations out of order | Manual/automated | Not run |
| EWRLD-LAB-008 | Authority and lifecycle | Enter the Laboratory scene directly without First Light | Manual/automated | Not run |
| EWRLD-LAB-009 | World topology identities | Register one world with two zones and several locations | Manual/automated | Not run |
| EWRLD-LAB-010 | World topology identities | Reject duplicate world IDs | Manual/automated | Not run |
| EWRLD-LAB-011 | World topology identities | Reject duplicate zone IDs | Manual/automated | Not run |
| EWRLD-LAB-012 | World topology identities | Reject duplicate location IDs | Manual/automated | Not run |
| EWRLD-LAB-013 | World topology identities | Reject a zone whose parent world is missing | Manual/automated | Not run |
| EWRLD-LAB-014 | World topology identities | Reject a location whose parent zone is missing | Manual/automated | Not run |
| EWRLD-LAB-015 | World topology identities | Apply an approved ID alias during import | Manual/automated | Not run |
| EWRLD-LAB-016 | World topology identities | Preserve a tombstoned location record without reactivating it | Manual/automated | Not run |
| EWRLD-LAB-017 | Location hierarchy and active context | Set the active world, zone, and location through one context request | Manual/automated | Not run |
| EWRLD-LAB-018 | Location hierarchy and active context | Reject a location that does not belong to the requested zone | Manual/automated | Not run |
| EWRLD-LAB-019 | Location hierarchy and active context | Clear active context during project-defined unload | Manual/automated | Not run |
| EWRLD-LAB-020 | Location hierarchy and active context | Recover active context from a valid scene binding registration | Manual/automated | Not run |
| EWRLD-LAB-021 | Location hierarchy and active context | Reject a stale active-context revision | Manual/automated | Not run |
| EWRLD-LAB-022 | Location hierarchy and active context | Publish context change events after commitment | Manual/automated | Not run |
| EWRLD-LAB-023 | Location hierarchy and active context | Keep previous context when validation fails | Manual/automated | Not run |
| EWRLD-LAB-024 | Location hierarchy and active context | Handle direct-scene entry with an explicit development binding | Manual/automated | Not run |
| EWRLD-LAB-025 | Travel graph and planning | Plan one direct travel connection | Manual/automated | Not run |
| EWRLD-LAB-026 | Travel graph and planning | Plan a multi-leg route through the topology graph | Manual/automated | Not run |
| EWRLD-LAB-027 | Travel graph and planning | Respect directed travel connections | Manual/automated | Not run |
| EWRLD-LAB-028 | Travel graph and planning | Respect bidirectional travel connections | Manual/automated | Not run |
| EWRLD-LAB-029 | Travel graph and planning | Reject a route with no valid path | Manual/automated | Not run |
| EWRLD-LAB-030 | Travel graph and planning | Reject a connection denied by a condition provider | Manual/automated | Not run |
| EWRLD-LAB-031 | Travel graph and planning | Return Unavailable when a required condition provider is absent | Manual/automated | Not run |
| EWRLD-LAB-032 | Travel graph and planning | Preserve deterministic route choice when several routes tie | Manual/automated | Not run |
| EWRLD-LAB-033 | Scene and destination mapping | Resolve one location to one scene-binding token | Manual/automated | Not run |
| EWRLD-LAB-034 | Scene and destination mapping | Resolve one location with several valid scene bindings by priority | Manual/automated | Not run |
| EWRLD-LAB-035 | Scene and destination mapping | Reject duplicate binding IDs | Manual/automated | Not run |
| EWRLD-LAB-036 | Scene and destination mapping | Reject a binding that references a missing location | Manual/automated | Not run |
| EWRLD-LAB-037 | Scene and destination mapping | Keep Unity asset GUID separate from runtime scene-binding identity | Manual/automated | Not run |
| EWRLD-LAB-038 | Scene and destination mapping | Return a travel plan without loading a scene | Manual/automated | Not run |
| EWRLD-LAB-039 | Scene and destination mapping | Handle an unavailable scene resolver | Manual/automated | Not run |
| EWRLD-LAB-040 | Scene and destination mapping | Remove the Passage bridge without breaking world topology queries | Manual/automated | Not run |
| EWRLD-LAB-041 | Entry and spawn markers | Register one runtime entry marker | Manual/automated | Not run |
| EWRLD-LAB-042 | Entry and spawn markers | Register one runtime spawn marker | Manual/automated | Not run |
| EWRLD-LAB-043 | Entry and spawn markers | Select a marker by exact marker ID | Manual/automated | Not run |
| EWRLD-LAB-044 | Entry and spawn markers | Select a marker by required tags and priority | Manual/automated | Not run |
| EWRLD-LAB-045 | Entry and spawn markers | Reject a stale marker registration | Manual/automated | Not run |
| EWRLD-LAB-046 | Entry and spawn markers | Unregister markers when their scene unloads | Manual/automated | Not run |
| EWRLD-LAB-047 | Entry and spawn markers | Return Unavailable when no marker satisfies the request | Manual/automated | Not run |
| EWRLD-LAB-048 | Entry and spawn markers | Preserve deterministic marker selection across equal candidates | Manual/automated | Not run |
| EWRLD-LAB-049 | Discovery and visitation | Discover one world | Manual/automated | Not run |
| EWRLD-LAB-050 | Discovery and visitation | Discover one zone | Manual/automated | Not run |
| EWRLD-LAB-051 | Discovery and visitation | Discover one location | Manual/automated | Not run |
| EWRLD-LAB-052 | Discovery and visitation | Record a first visit and increment repeat visits | Manual/automated | Not run |
| EWRLD-LAB-053 | Discovery and visitation | Ignore an idempotent repeated discovery request | Manual/automated | Not run |
| EWRLD-LAB-054 | Discovery and visitation | Reject discovery of an unknown identity | Manual/automated | Not run |
| EWRLD-LAB-055 | Discovery and visitation | Preserve discovery when an optional package is removed | Manual/automated | Not run |
| EWRLD-LAB-056 | Discovery and visitation | Publish immutable discovery snapshots | Manual/automated | Not run |
| EWRLD-LAB-057 | Fast travel | Allow fast travel to a discovered eligible destination | Manual/automated | Not run |
| EWRLD-LAB-058 | Fast travel | Deny fast travel to an undiscovered destination | Manual/automated | Not run |
| EWRLD-LAB-059 | Fast travel | Deny fast travel while a condition provider blocks it | Manual/automated | Not run |
| EWRLD-LAB-060 | Fast travel | Return Unavailable when a fast-travel provider is missing | Manual/automated | Not run |
| EWRLD-LAB-061 | Fast travel | Require an authored entry marker for fast-travel arrival | Manual/automated | Not run |
| EWRLD-LAB-062 | Fast travel | Plan fast travel without executing scene loading | Manual/automated | Not run |
| EWRLD-LAB-063 | Fast travel | Reject a stale destination revision | Manual/automated | Not run |
| EWRLD-LAB-064 | Fast travel | Keep normal travel and fast travel policies distinct | Manual/automated | Not run |
| EWRLD-LAB-065 | World-state records | Export core discovery and visitation state | Manual/automated | Not run |
| EWRLD-LAB-066 | World-state records | Export one registered world-state participant record | Manual/automated | Not run |
| EWRLD-LAB-067 | World-state records | Import a versioned world-state snapshot | Manual/automated | Not run |
| EWRLD-LAB-068 | World-state records | Preserve an unknown participant record | Manual/automated | Not run |
| EWRLD-LAB-069 | World-state records | Reject duplicate participant provider IDs | Manual/automated | Not run |
| EWRLD-LAB-070 | World-state records | Reject active scene-object references in durable state | Manual/automated | Not run |
| EWRLD-LAB-071 | World-state records | Apply contiguous migrations before publication | Manual/automated | Not run |
| EWRLD-LAB-072 | World-state records | Keep the source snapshot when import fails | Manual/automated | Not run |
| EWRLD-LAB-073 | Map and presentation snapshots | Build a topology snapshot for a map presenter | Manual/automated | Not run |
| EWRLD-LAB-074 | Map and presentation snapshots | Include nodes, connections, discovery, and availability metadata | Manual/automated | Not run |
| EWRLD-LAB-075 | Map and presentation snapshots | Exclude production localized text from the core snapshot | Manual/automated | Not run |
| EWRLD-LAB-076 | Map and presentation snapshots | Preserve stable map-layout metadata from project assets | Manual/automated | Not run |
| EWRLD-LAB-077 | Map and presentation snapshots | Return hidden locations according to authored visibility policy | Manual/automated | Not run |
| EWRLD-LAB-078 | Map and presentation snapshots | Update a presenter snapshot after discovery | Manual/automated | Not run |
| EWRLD-LAB-079 | Map and presentation snapshots | Remove a map presenter without changing world truth | Manual/automated | Not run |
| EWRLD-LAB-080 | Map and presentation snapshots | Support a no-presentation standalone runtime | Manual/automated | Not run |
| EWRLD-LAB-081 | Provider and bridge boundaries | Register and remove a Passage travel executor bridge | Manual/automated | Not run |
| EWRLD-LAB-082 | Provider and bridge boundaries | Register and remove a Fellowship spawn-owner bridge | Manual/automated | Not run |
| EWRLD-LAB-083 | Provider and bridge boundaries | Register and remove a Chronicle persistence bridge | Manual/automated | Not run |
| EWRLD-LAB-084 | Provider and bridge boundaries | Register and remove an Objectives discovery observer | Manual/automated | Not run |
| EWRLD-LAB-085 | Provider and bridge boundaries | Register and remove an Eye map-camera adapter | Manual/automated | Not run |
| EWRLD-LAB-086 | Provider and bridge boundaries | Register and remove a Convergence authority bridge | Manual/automated | Not run |
| EWRLD-LAB-087 | Provider and bridge boundaries | Reject an incompatible provider version | Manual/automated | Not run |
| EWRLD-LAB-088 | Provider and bridge boundaries | Isolate one provider exception from the world authority | Manual/automated | Not run |
| EWRLD-LAB-089 | Multiplayer and authority | Reject a client-authored shared-world context change | Manual/automated | Not run |
| EWRLD-LAB-090 | Multiplayer and authority | Accept an authoritative host world-state update | Manual/automated | Not run |
| EWRLD-LAB-091 | Multiplayer and authority | Keep personal discovery separate from shared-world discovery | Manual/automated | Not run |
| EWRLD-LAB-092 | Multiplayer and authority | Reject a stale replicated world revision | Manual/automated | Not run |
| EWRLD-LAB-093 | Multiplayer and authority | Reconcile a client presentation snapshot after authority update | Manual/automated | Not run |
| EWRLD-LAB-094 | Multiplayer and authority | Keep provider network entity IDs outside durable World IDs | Manual/automated | Not run |
| EWRLD-LAB-095 | Multiplayer and authority | Confirm no networking SDK is required by the core | Manual/automated | Not run |
| EWRLD-LAB-096 | Multiplayer and authority | Remove the multiplayer bridge without corrupting local world data | Manual/automated | Not run |
| EWRLD-LAB-097 | Diagnostics, stress, and removal | Bound travel-plan history under sustained requests | Manual/automated | Not run |
| EWRLD-LAB-098 | Diagnostics, stress, and removal | Bound marker registries and report configured limits | Manual/automated | Not run |
| EWRLD-LAB-099 | Diagnostics, stress, and removal | Detect unreachable topology nodes | Manual/automated | Not run |
| EWRLD-LAB-100 | Diagnostics, stress, and removal | Detect circular references that violate authored policy | Manual/automated | Not run |
| EWRLD-LAB-101 | Diagnostics, stress, and removal | Detect leaked runtime marker registrations at reset | Manual/automated | Not run |
| EWRLD-LAB-102 | Diagnostics, stress, and removal | Remove and reinstall EchoWorld while preserving project-owned assets | Manual/automated | Not run |
| EWRLD-LAB-103 | Diagnostics, stress, and removal | Delete samples without breaking the runtime package | Manual/automated | Not run |
| EWRLD-LAB-104 | Diagnostics, stress, and removal | Export a privacy-safe final diagnostic snapshot | Manual/automated | Not run |

### 13.4 Optional integration samples

| Sample | Packages involved | Purpose | Why not standalone proof |
|---|---|---|---|
| Atlas + Passage Travel | Atlas, Passage | Execute a prepared semantic travel plan | Depends on two authorities and bridge |
| Atlas + Fellowship Entry | Atlas, Fellowship | Select marker and spawn/relocate a character | Character authority is external |
| Atlas + Looking Glass Map | Atlas, Looking Glass | Render topology and travel UI | Presentation dependency |
| Atlas + Chronicle Persistence | Atlas, Chronicle | Save/load detached world state | Save transport dependency |
| Atlas + Convergence Shared World | Atlas, Convergence, provider | Replicate shared context and discovery | Networking/provider dependency |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The runtime core is nonvisual. It exposes semantic snapshots and commands. Looking Glass or project presenters own world maps, travel screens, location labels, icons, route lines, legends, loading messages, and confirmation prompts.

### 14.2 Required presenter states

- Ready with current location.
- Unknown or no active context.
- Destination allowed.
- Destination denied with reason.
- Destination unavailable due to missing provider.
- Hidden/undiscovered location.
- No route.
- Missing scene binding or marker.
- Travel being prepared or externally executed.
- Import/migration warning.

### 14.3 Accessibility requirements

- Map and travel information must not rely on color alone.
- Locations and connections expose textual/assistive labels through localization references.
- Route, availability, and current-position states expose semantic descriptors.
- Motion/zoom of map presentations remains optional and respects reduced-motion policy through UI/Camera bridges.
- Icon-only markers require text equivalents.
- Hidden location policy must distinguish intentionally hidden from unavailable data.

### 14.4 Visual customization

Project visuals, icons, fonts, maps, line styles, geography, and layout remain replaceable without editing Atlas runtime code.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| Initialization and catalog health | API/Inspector/log | Development/release-safe summary | Low |
| Active context and revision | API/Inspector | Development/release-safe | Low |
| Topology counts and reachability | Validator/report | Editor/development | Bounded |
| Last travel plan/result | API/Inspector | Development | Bounded |
| Marker registry counts and leaks | API/overlay bridge | Development | Low/bounded |
| Discovery/visit counts | API | Development/release-safe summary | Low |
| Participant/provider health | API/report | Development | Low |
| State import/migration summary | Report | Development/support | Bounded |

### 15.2 Structured status

Status includes root identity, package version, catalog identity/fingerprint, initialized state, current context, topology counts, provider registrations, marker counts, prepared-plan count, discovery/visit counts, import state, warnings, errors, and configured limits.

### 15.3 Diagnostic codes

Use stable `EWRLD-*` codes. Logs remain categorized and avoid resolved production text, private file paths, player account identifiers, credentials, provider secrets, and arbitrary opaque payload contents.

### 15.4 Observatory bridge

An optional bridge publishes status, topology, current context, route timings, marker counts, discovery counts, provider health, import/migration summaries, and recent failures. Atlas never requires the Observatory.

### 15.5 Logging policy

- No per-frame spam.
- Route and provider failures are actionable and stable-coded.
- Opaque participant payloads are redacted.
- Development verbosity is separate from release-safe summaries.
- Support exports include IDs and schemas, not production narrative content.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Catalog/topology | Project definition | Project/Atlas | Asset, not save | ScriptableObject |
| Current context | Session/profile/slot by project policy | Atlas | Optional stable IDs | Chronicle/project transport |
| Discovery and visits | Profile/slot/session by policy | Atlas | Yes when configured | Chronicle/project transport |
| Runtime marker registry | Scene/session | Atlas | No | Runtime only |
| Prepared travel plans | Request/session | Atlas | No | Runtime only |
| Provider world-state records | Provider-defined | Provider routed by Atlas | Optional | Chronicle/project transport |
| Diagnostic history | Development session | Atlas | Support export only | Bounded record |

### 16.2 Standalone behavior

Without Chronicle, project code may export/import `WorldStateSnapshot` through any approved local transport. Atlas never silently chooses a filename or storage backend.

### 16.3 Optional participant/provider contract

Each `IWorldStateParticipant` owns one stable provider ID, schema version, detached payload, migration path, validation, and application behavior. Atlas preserves unknown records and routes known records. Chronicle transports the aggregate document through a separate bridge.

### 16.4 Failure and recovery

- Missing snapshot: initialize approved defaults.
- Older snapshot: migrate contiguously before publication.
- Newer unsupported snapshot: return Unavailable/Unsupported and preserve source.
- Missing definition: retain orphan identity and diagnostics according to policy.
- Missing participant provider: preserve opaque record.
- Failed provider application: abort import before publication where possible; otherwise follow declared participant transaction policy.
- Corrupt snapshot: reject and defer recovery to Chronicle/project transport.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Atlas describes semantic world truth. Bridges translate that truth into scene travel, character placement, UI, persistence, objectives, camera, localization, build validation, AI context, or multiplayer authority. Installing a peer package never silently changes core behavior.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| The Passage (`EchoSceneFlow`) | Separate bridge | Passage/World bridge | World plan -> Passage request; Passage result -> context commit | Location, scene-binding token, route, marker criteria, travel result | No |
| The Fellowship (`EchoCharacters`) | Separate bridge/project adapter | Characters/World bridge | Character location and spawn/entry handoff | CharacterId, location, marker criteria, spawn result | No |
| The Chronicle (`EchoSave`) | Separate bridge | World/Chronicle bridge | World participant snapshot to save transport | Versioned WorldStateSnapshot | No |
| The Path (`EchoObjectives`) | Project adapter/bridge | Objectives/World bridge | Discovery, visit, and travel events | Stable IDs and committed events | No |
| The Eye (`EchoCamera`) | Project adapter | Camera/World adapter | Map camera and location framing metadata | Location/map layout snapshots | No |
| The Looking Glass (`EchoUI`) | Separate bridge/project presenter | UI/World bridge | World map and travel-selection presentation | Map snapshot and commands | No |
| Many Tongues (`EchoLocalization`) | Project adapter | Localization/World adapter | Resolve display references | Localized references only | No |
| The Convergence (`EchoMultiplayer`) | Separate bridge/provider adapter | World/Multiplayer bridge | Shared context, discovery, and travel authority | Stable IDs, revisions, snapshots | No |
| Instinct (`EchoAI`) | Project adapter | AI/World adapter | Semantic location and zone context | World/zone/location snapshots | No |
| The Foundry (`EchoBuildTools`) | Validator provider | Foundry integration | Validate referenced scenes/bindings before build | Validation results only | No |

### 17.3 Bridge placement decision

Two-package bridges ship separately when they reference both runtime packages. Small project-specific translations remain project adapters. Provider-specific scene, streaming, map, or network adapters remain separate provider packages.

### 17.4 Integration failure behavior

- Missing peer: core remains functional; related operation returns Unavailable.
- Version mismatch: bridge refuses registration with stable compatibility result.
- Peer teardown: disposable registration removes provider and invalidates stale handles.
- Passage failure: travel execution fails externally; Atlas context does not commit unless approved handoff succeeds.
- Character spawn failure: world context and marker truth remain separate from actor placement.
- Network disconnect: shared-state behavior follows Convergence/provider policy; local data remains intact.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Planning target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Context/discovery query | No steady-state allocation after warmup | Profiler + tests | Evidence-pending |
| Direct travel evaluation | Bounded by authored connection degree | Atlas Laboratory | Evidence-pending |
| Multi-leg route planning | Bounded catalog and measurable worst case | Stress fixture | Evidence-pending |
| Marker selection | Bounded candidates per location/tag index | Marker stress fixture | Evidence-pending |
| Map snapshot generation | Bounded nodes/edges and optional caching | Map fixture | Evidence-pending |
| State export/import | Detached payload and participant limits | Migration fixture | Evidence-pending |

### 18.2 Allocation policy

- No per-frame polling is required by the core.
- Catalog indexes build once per approved initialization/reload.
- Route search uses pooled or bounded working state after implementation proves the best strategy.
- No reflection in hot paths.
- Snapshots are immutable and created on demand or revision change.
- Marker/provider registries use explicit limits and indexed lookup.

### 18.3 Scene and domain reload behavior

Registrations unsubscribe through handles, static convenience state resets, direct-scene helpers obey duplicate safety, and marker registrations reconcile on scene teardown. Enter Play Mode configurations require automated coverage.

### 18.4 Scalability limits

Advertised limits remain `Not run`. Configuration must expose bounded worlds, zones, locations, connections, markers, providers, plan history, state records, aliases, and snapshot sizes. Huge-world databases and server sharding remain later adapters.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Atlas may process authored world IDs, player discovery/visit state, current semantic location, and provider records. It does not require credentials, account secrets, analytics identifiers, or precise real-world geolocation.

### 19.2 Trust boundaries

- Imported snapshots are untrusted until version, identity, size, migration, and participant validation pass.
- Client-provided shared-world changes are untrusted until authoritative Convergence/provider validation.
- Scene-binding tokens are configuration references, not filesystem access grants.
- Opaque participant payloads are size-bounded and never logged verbatim.
- Map snapshots contain game-world metadata only and must not include user secrets.

### 19.3 Platform behavior

| Platform | Status | Special behavior | Validation required |
|---|---:|---|---|
| Windows | Planned | Standard Unity scene lifecycle | Clean Player tests |
| macOS | Planned | Standard Unity scene lifecycle | Clean Player tests |
| Linux | Planned | Standard Unity scene lifecycle | Clean Player tests |
| WebGL | Planned | Memory and async/provider limitations | Browser Player tests |
| Mobile | Planned | Memory, pause/resume, and scene lifecycle | Device tests |
| Console | Unknown | Platform approval and provider constraints | Licensed platform testing |

No platform is claimed Supported until SFGSS-004 evidence exists.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-world/
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
│   ├── Authority/
│   ├── Context/
│   ├── Topology/
│   ├── Travel/
│   ├── Discovery/
│   ├── Markers/
│   ├── State/
│   ├── Maps/
│   └── Diagnostics/
├── Data/
│   ├── IDs/
│   ├── Definitions/
│   ├── Snapshots/
│   └── Migration/
├── Providers/
└── EchoDevGames.EchoWorld.Runtime.asmdef
Editor/
├── Setup/
├── Validation/
├── Topology/
├── Bindings/
└── EchoDevGames.EchoWorld.Editor.asmdef
Samples~/
└── Atlas World Topology Laboratory/
Tests/
├── Editor/
└── Runtime/
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoWorld.Runtime` | Runtime | Unity modules only | Yes | Neutral world contracts and service |
| `EchoDevGames.EchoWorld.Editor` | Editor | Runtime, UnityEditor | False | Setup, validation, topology and binding tools |
| `EchoDevGames.EchoWorld.Tests.Editor` | Editor test | Runtime, Editor, Test Framework | False | EditMode evidence |
| `EchoDevGames.EchoWorld.Tests.Runtime` | Runtime test | Runtime, Test Framework | False | PlayMode evidence |

Optional bridges/providers receive separate packages/assemblies under SFGSS-002.

### 20.4 Repository files

README, Documentation index, user/developer guides, Current Notes link, ADRs, changelog, license, notices, contribution guidance, release checklist, stable `.meta` files, and compatibility records are required.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | Planned 6000.0 | 6000.3.8f1 planning baseline | Runtime evidence Not run |
| Optional Echo bridges | Per bridge specification | Not run | No hard core dependency |

### 21.2 Semantic versioning policy

- Patch: diagnostics, validation, documentation, and compatible behavior fixes.
- Minor: additive APIs, definition fields with defaults, providers, and optional capabilities.
- Major: breaking IDs, serialization, topology semantics, route behavior, public APIs, or removal contracts.

### 21.3 Deprecation policy

Deprecated IDs, fields, and APIs receive warnings, aliases/migrations where durable state is affected, documented replacement, and at least one minor-version migration window before removal when practical.

### 21.4 GUID and asset compatibility

Public definitions, templates, prefabs, samples, and scripts preserve `.meta` GUIDs. Unity asset GUIDs remain source asset identity and do not replace Atlas domain IDs.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package boundaries and conceptual model.
- Five-minute single-world setup.
- Worlds, zones, locations, links, bindings, and markers guide.
- Discovery, visits, fast travel, and map snapshot guide.
- Direct-scene testing guide.
- Laboratory guide.
- Diagnostic-code reference.
- Optional integration index.
- Migration and known-limitations guide.

### 22.2 Required developer documentation

- Authority and lifecycle.
- Identity taxonomy.
- Topology and route-planning contracts.
- Scene-binding and Passage boundary.
- Marker registration/selection lifecycle.
- State-participant and persistence boundary.
- Map snapshot and presentation seam.
- Multiplayer authority seam.
- Testing, release, and removal strategy.

### 22.3 Documentation truth rule

Examples must compile against the documented release after implementation. No platform, scale, provider, performance, migration, or compatibility claim becomes Supported without executed evidence.

### 22.4 Living repository workflow

Current Notes captures discoveries; durable changes promote to this foundation, standards, ADRs, bridge specs, research, tests, or guides at checkpoint closeout.

### 22.5 Repository scan order

README -> SFGSS-000 -> SFGSS-002/003/004/005 -> this foundation -> feasibility record -> applicable bridge specs -> Current Notes -> active checkpoint/tests -> code when implementation begins.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, hierarchy, topology, planning, migration, validation | Duplicate IDs, reachability, aliases, deterministic routes | Yes |
| PlayMode unit/integration | Root lifecycle, context, marker registry, provider teardown | Direct-scene, scene unload, stale handles | Yes |
| Standalone Laboratory | User-visible isolated world loop | Discovery, plans, markers, snapshots, failures | Yes |
| Bridge Integration Laboratory | Passage, Characters, Save, UI, Multiplayer | Explicit bridge proof | When bridge ships |
| Showcase | Combined map/travel/world demo | Portfolio presentation | No |
| Clean-project install | Packaging and missing-dependency proof | Embedded/local/tarball/Git | Yes |
| Existing-project migration | Replace scene-string location logic | Rescuers2D/Echo Systems Lab/Hackulos later | Before adoption claim |

### 23.2 Required test categories

Authority, IDs, hierarchy, topology, connections, planning, binding metadata, markers, discovery, visits, fast travel, snapshots, migrations, unknown records, conditions, providers, events, direct-scene entry, scene teardown, optional bridges, performance, platform, packaging, removal, reinstall, and release gates.

### 23.3 Test case registry

| Test ID | Category | Requirement | Setup | Action | Expected result | Status |
|---|---|---|---|---|---|---|
| EWRLD-T-001 | Authority and lifecycle | Authority and lifecycle accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-002 | Authority and lifecycle | Authority and lifecycle rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-003 | Authority and lifecycle | Authority and lifecycle rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-004 | Authority and lifecycle | Authority and lifecycle preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-005 | Authority and lifecycle | Authority and lifecycle reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-006 | Authority and lifecycle | Authority and lifecycle does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-007 | Authority and lifecycle | Authority and lifecycle remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-008 | Authority and lifecycle | Authority and lifecycle cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-009 | Authority and lifecycle | Authority and lifecycle survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-010 | Authority and lifecycle | Authority and lifecycle isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-011 | Authority and lifecycle | Authority and lifecycle preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-012 | Authority and lifecycle | Authority and lifecycle avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-013 | Authority and lifecycle | Authority and lifecycle uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-014 | Authority and lifecycle | Authority and lifecycle works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-015 | Authority and lifecycle | Authority and lifecycle records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-016 | Authority and lifecycle | Authority and lifecycle supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-017 | Authority and lifecycle | Authority and lifecycle handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-018 | Authority and lifecycle | Authority and lifecycle keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-019 | Authority and lifecycle | Authority and lifecycle keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-020 | Authority and lifecycle | Authority and lifecycle produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-021 | Authority and lifecycle | Authority and lifecycle preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-022 | Authority and lifecycle | Authority and lifecycle avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-023 | Authority and lifecycle | Authority and lifecycle validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-024 | Authority and lifecycle | Authority and lifecycle supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-025 | Authority and lifecycle | Authority and lifecycle keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-026 | Authority and lifecycle | Authority and lifecycle keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-027 | Stable identity and aliases | Stable identity and aliases accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-028 | Stable identity and aliases | Stable identity and aliases rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-029 | Stable identity and aliases | Stable identity and aliases rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-030 | Stable identity and aliases | Stable identity and aliases preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-031 | Stable identity and aliases | Stable identity and aliases reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-032 | Stable identity and aliases | Stable identity and aliases does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-033 | Stable identity and aliases | Stable identity and aliases remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-034 | Stable identity and aliases | Stable identity and aliases cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-035 | Stable identity and aliases | Stable identity and aliases survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-036 | Stable identity and aliases | Stable identity and aliases isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-037 | Stable identity and aliases | Stable identity and aliases preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-038 | Stable identity and aliases | Stable identity and aliases avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-039 | Stable identity and aliases | Stable identity and aliases uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-040 | Stable identity and aliases | Stable identity and aliases works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-041 | Stable identity and aliases | Stable identity and aliases records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-042 | Stable identity and aliases | Stable identity and aliases supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-043 | Stable identity and aliases | Stable identity and aliases handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-044 | Stable identity and aliases | Stable identity and aliases keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-045 | Stable identity and aliases | Stable identity and aliases keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-046 | Stable identity and aliases | Stable identity and aliases produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-047 | Stable identity and aliases | Stable identity and aliases preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-048 | Stable identity and aliases | Stable identity and aliases avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-049 | Stable identity and aliases | Stable identity and aliases validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-050 | Stable identity and aliases | Stable identity and aliases supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-051 | Stable identity and aliases | Stable identity and aliases keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-052 | Stable identity and aliases | Stable identity and aliases keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-053 | Catalog and topology validation | Catalog and topology validation accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-054 | Catalog and topology validation | Catalog and topology validation rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-055 | Catalog and topology validation | Catalog and topology validation rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-056 | Catalog and topology validation | Catalog and topology validation preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-057 | Catalog and topology validation | Catalog and topology validation reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-058 | Catalog and topology validation | Catalog and topology validation does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-059 | Catalog and topology validation | Catalog and topology validation remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-060 | Catalog and topology validation | Catalog and topology validation cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-061 | Catalog and topology validation | Catalog and topology validation survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-062 | Catalog and topology validation | Catalog and topology validation isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-063 | Catalog and topology validation | Catalog and topology validation preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-064 | Catalog and topology validation | Catalog and topology validation avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-065 | Catalog and topology validation | Catalog and topology validation uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-066 | Catalog and topology validation | Catalog and topology validation works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-067 | Catalog and topology validation | Catalog and topology validation records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-068 | Catalog and topology validation | Catalog and topology validation supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-069 | Catalog and topology validation | Catalog and topology validation handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-070 | Catalog and topology validation | Catalog and topology validation keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-071 | Catalog and topology validation | Catalog and topology validation keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-072 | Catalog and topology validation | Catalog and topology validation produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-073 | Catalog and topology validation | Catalog and topology validation preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-074 | Catalog and topology validation | Catalog and topology validation avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-075 | Catalog and topology validation | Catalog and topology validation validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-076 | Catalog and topology validation | Catalog and topology validation supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-077 | Catalog and topology validation | Catalog and topology validation keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-078 | Catalog and topology validation | Catalog and topology validation keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-079 | Active world context | Active world context accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-080 | Active world context | Active world context rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-081 | Active world context | Active world context rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-082 | Active world context | Active world context preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-083 | Active world context | Active world context reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-084 | Active world context | Active world context does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-085 | Active world context | Active world context remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-086 | Active world context | Active world context cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-087 | Active world context | Active world context survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-088 | Active world context | Active world context isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-089 | Active world context | Active world context preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-090 | Active world context | Active world context avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-091 | Active world context | Active world context uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-092 | Active world context | Active world context works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-093 | Active world context | Active world context records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-094 | Active world context | Active world context supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-095 | Active world context | Active world context handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-096 | Active world context | Active world context keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-097 | Active world context | Active world context keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-098 | Active world context | Active world context produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-099 | Active world context | Active world context preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-100 | Active world context | Active world context avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-101 | Active world context | Active world context validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-102 | Active world context | Active world context supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-103 | Active world context | Active world context keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-104 | Active world context | Active world context keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-105 | World, zone, and location hierarchy | World, zone, and location hierarchy accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-106 | World, zone, and location hierarchy | World, zone, and location hierarchy rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-107 | World, zone, and location hierarchy | World, zone, and location hierarchy rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-108 | World, zone, and location hierarchy | World, zone, and location hierarchy preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-109 | World, zone, and location hierarchy | World, zone, and location hierarchy reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-110 | World, zone, and location hierarchy | World, zone, and location hierarchy does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-111 | World, zone, and location hierarchy | World, zone, and location hierarchy remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-112 | World, zone, and location hierarchy | World, zone, and location hierarchy cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-113 | World, zone, and location hierarchy | World, zone, and location hierarchy survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-114 | World, zone, and location hierarchy | World, zone, and location hierarchy isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-115 | World, zone, and location hierarchy | World, zone, and location hierarchy preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-116 | World, zone, and location hierarchy | World, zone, and location hierarchy avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-117 | World, zone, and location hierarchy | World, zone, and location hierarchy uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-118 | World, zone, and location hierarchy | World, zone, and location hierarchy works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-119 | World, zone, and location hierarchy | World, zone, and location hierarchy records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-120 | World, zone, and location hierarchy | World, zone, and location hierarchy supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-121 | World, zone, and location hierarchy | World, zone, and location hierarchy handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-122 | World, zone, and location hierarchy | World, zone, and location hierarchy keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-123 | World, zone, and location hierarchy | World, zone, and location hierarchy keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-124 | World, zone, and location hierarchy | World, zone, and location hierarchy produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-125 | World, zone, and location hierarchy | World, zone, and location hierarchy preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-126 | World, zone, and location hierarchy | World, zone, and location hierarchy avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-127 | World, zone, and location hierarchy | World, zone, and location hierarchy validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-128 | World, zone, and location hierarchy | World, zone, and location hierarchy supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-129 | World, zone, and location hierarchy | World, zone, and location hierarchy keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-130 | World, zone, and location hierarchy | World, zone, and location hierarchy keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-131 | Travel connections | Travel connections accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-132 | Travel connections | Travel connections rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-133 | Travel connections | Travel connections rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-134 | Travel connections | Travel connections preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-135 | Travel connections | Travel connections reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-136 | Travel connections | Travel connections does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-137 | Travel connections | Travel connections remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-138 | Travel connections | Travel connections cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-139 | Travel connections | Travel connections survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-140 | Travel connections | Travel connections isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-141 | Travel connections | Travel connections preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-142 | Travel connections | Travel connections avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-143 | Travel connections | Travel connections uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-144 | Travel connections | Travel connections works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-145 | Travel connections | Travel connections records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-146 | Travel connections | Travel connections supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-147 | Travel connections | Travel connections handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-148 | Travel connections | Travel connections keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-149 | Travel connections | Travel connections keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-150 | Travel connections | Travel connections produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-151 | Travel connections | Travel connections preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-152 | Travel connections | Travel connections avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-153 | Travel connections | Travel connections validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-154 | Travel connections | Travel connections supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-155 | Travel connections | Travel connections keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-156 | Travel connections | Travel connections keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-157 | Travel planning and route choice | Travel planning and route choice accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-158 | Travel planning and route choice | Travel planning and route choice rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-159 | Travel planning and route choice | Travel planning and route choice rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-160 | Travel planning and route choice | Travel planning and route choice preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-161 | Travel planning and route choice | Travel planning and route choice reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-162 | Travel planning and route choice | Travel planning and route choice does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-163 | Travel planning and route choice | Travel planning and route choice remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-164 | Travel planning and route choice | Travel planning and route choice cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-165 | Travel planning and route choice | Travel planning and route choice survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-166 | Travel planning and route choice | Travel planning and route choice isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-167 | Travel planning and route choice | Travel planning and route choice preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-168 | Travel planning and route choice | Travel planning and route choice avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-169 | Travel planning and route choice | Travel planning and route choice uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-170 | Travel planning and route choice | Travel planning and route choice works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-171 | Travel planning and route choice | Travel planning and route choice records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-172 | Travel planning and route choice | Travel planning and route choice supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-173 | Travel planning and route choice | Travel planning and route choice handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-174 | Travel planning and route choice | Travel planning and route choice keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-175 | Travel planning and route choice | Travel planning and route choice keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-176 | Travel planning and route choice | Travel planning and route choice produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-177 | Travel planning and route choice | Travel planning and route choice preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-178 | Travel planning and route choice | Travel planning and route choice avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-179 | Travel planning and route choice | Travel planning and route choice validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-180 | Travel planning and route choice | Travel planning and route choice supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-181 | Travel planning and route choice | Travel planning and route choice keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-182 | Travel planning and route choice | Travel planning and route choice keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-183 | Scene and location mapping | Scene and location mapping accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-184 | Scene and location mapping | Scene and location mapping rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-185 | Scene and location mapping | Scene and location mapping rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-186 | Scene and location mapping | Scene and location mapping preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-187 | Scene and location mapping | Scene and location mapping reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-188 | Scene and location mapping | Scene and location mapping does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-189 | Scene and location mapping | Scene and location mapping remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-190 | Scene and location mapping | Scene and location mapping cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-191 | Scene and location mapping | Scene and location mapping survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-192 | Scene and location mapping | Scene and location mapping isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-193 | Scene and location mapping | Scene and location mapping preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-194 | Scene and location mapping | Scene and location mapping avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-195 | Scene and location mapping | Scene and location mapping uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-196 | Scene and location mapping | Scene and location mapping works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-197 | Scene and location mapping | Scene and location mapping records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-198 | Scene and location mapping | Scene and location mapping supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-199 | Scene and location mapping | Scene and location mapping handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-200 | Scene and location mapping | Scene and location mapping keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-201 | Scene and location mapping | Scene and location mapping keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-202 | Scene and location mapping | Scene and location mapping produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-203 | Scene and location mapping | Scene and location mapping preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-204 | Scene and location mapping | Scene and location mapping avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-205 | Scene and location mapping | Scene and location mapping validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-206 | Scene and location mapping | Scene and location mapping supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-207 | Scene and location mapping | Scene and location mapping keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-208 | Scene and location mapping | Scene and location mapping keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-209 | Entry markers | Entry markers accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-210 | Entry markers | Entry markers rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-211 | Entry markers | Entry markers rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-212 | Entry markers | Entry markers preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-213 | Entry markers | Entry markers reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-214 | Entry markers | Entry markers does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-215 | Entry markers | Entry markers remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-216 | Entry markers | Entry markers cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-217 | Entry markers | Entry markers survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-218 | Entry markers | Entry markers isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-219 | Entry markers | Entry markers preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-220 | Entry markers | Entry markers avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-221 | Entry markers | Entry markers uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-222 | Entry markers | Entry markers works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-223 | Entry markers | Entry markers records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-224 | Entry markers | Entry markers supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-225 | Entry markers | Entry markers handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-226 | Entry markers | Entry markers keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-227 | Entry markers | Entry markers keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-228 | Entry markers | Entry markers produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-229 | Entry markers | Entry markers preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-230 | Entry markers | Entry markers avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-231 | Entry markers | Entry markers validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-232 | Entry markers | Entry markers supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-233 | Entry markers | Entry markers keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-234 | Entry markers | Entry markers keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-235 | Spawn markers | Spawn markers accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-236 | Spawn markers | Spawn markers rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-237 | Spawn markers | Spawn markers rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-238 | Spawn markers | Spawn markers preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-239 | Spawn markers | Spawn markers reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-240 | Spawn markers | Spawn markers does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-241 | Spawn markers | Spawn markers remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-242 | Spawn markers | Spawn markers cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-243 | Spawn markers | Spawn markers survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-244 | Spawn markers | Spawn markers isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-245 | Spawn markers | Spawn markers preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-246 | Spawn markers | Spawn markers avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-247 | Spawn markers | Spawn markers uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-248 | Spawn markers | Spawn markers works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-249 | Spawn markers | Spawn markers records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-250 | Spawn markers | Spawn markers supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-251 | Spawn markers | Spawn markers handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-252 | Spawn markers | Spawn markers keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-253 | Spawn markers | Spawn markers keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-254 | Spawn markers | Spawn markers produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-255 | Spawn markers | Spawn markers preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-256 | Spawn markers | Spawn markers avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-257 | Spawn markers | Spawn markers validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-258 | Spawn markers | Spawn markers supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-259 | Spawn markers | Spawn markers keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-260 | Spawn markers | Spawn markers keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-261 | Discovery and visitation | Discovery and visitation accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-262 | Discovery and visitation | Discovery and visitation rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-263 | Discovery and visitation | Discovery and visitation rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-264 | Discovery and visitation | Discovery and visitation preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-265 | Discovery and visitation | Discovery and visitation reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-266 | Discovery and visitation | Discovery and visitation does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-267 | Discovery and visitation | Discovery and visitation remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-268 | Discovery and visitation | Discovery and visitation cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-269 | Discovery and visitation | Discovery and visitation survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-270 | Discovery and visitation | Discovery and visitation isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-271 | Discovery and visitation | Discovery and visitation preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-272 | Discovery and visitation | Discovery and visitation avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-273 | Discovery and visitation | Discovery and visitation uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-274 | Discovery and visitation | Discovery and visitation works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-275 | Discovery and visitation | Discovery and visitation records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-276 | Discovery and visitation | Discovery and visitation supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-277 | Discovery and visitation | Discovery and visitation handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-278 | Discovery and visitation | Discovery and visitation keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-279 | Discovery and visitation | Discovery and visitation keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-280 | Discovery and visitation | Discovery and visitation produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-281 | Discovery and visitation | Discovery and visitation preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-282 | Discovery and visitation | Discovery and visitation avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-283 | Discovery and visitation | Discovery and visitation validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-284 | Discovery and visitation | Discovery and visitation supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-285 | Discovery and visitation | Discovery and visitation keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-286 | Discovery and visitation | Discovery and visitation keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-287 | Fast-travel policy | Fast-travel policy accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-288 | Fast-travel policy | Fast-travel policy rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-289 | Fast-travel policy | Fast-travel policy rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-290 | Fast-travel policy | Fast-travel policy preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-291 | Fast-travel policy | Fast-travel policy reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-292 | Fast-travel policy | Fast-travel policy does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-293 | Fast-travel policy | Fast-travel policy remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-294 | Fast-travel policy | Fast-travel policy cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-295 | Fast-travel policy | Fast-travel policy survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-296 | Fast-travel policy | Fast-travel policy isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-297 | Fast-travel policy | Fast-travel policy preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-298 | Fast-travel policy | Fast-travel policy avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-299 | Fast-travel policy | Fast-travel policy uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-300 | Fast-travel policy | Fast-travel policy works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-301 | Fast-travel policy | Fast-travel policy records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-302 | Fast-travel policy | Fast-travel policy supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-303 | Fast-travel policy | Fast-travel policy handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-304 | Fast-travel policy | Fast-travel policy keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-305 | Fast-travel policy | Fast-travel policy keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-306 | Fast-travel policy | Fast-travel policy produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-307 | Fast-travel policy | Fast-travel policy preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-308 | Fast-travel policy | Fast-travel policy avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-309 | Fast-travel policy | Fast-travel policy validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-310 | Fast-travel policy | Fast-travel policy supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-311 | Fast-travel policy | Fast-travel policy keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-312 | Fast-travel policy | Fast-travel policy keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-313 | World-state participants | World-state participants accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-314 | World-state participants | World-state participants rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-315 | World-state participants | World-state participants rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-316 | World-state participants | World-state participants preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-317 | World-state participants | World-state participants reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-318 | World-state participants | World-state participants does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-319 | World-state participants | World-state participants remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-320 | World-state participants | World-state participants cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-321 | World-state participants | World-state participants survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-322 | World-state participants | World-state participants isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-323 | World-state participants | World-state participants preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-324 | World-state participants | World-state participants avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-325 | World-state participants | World-state participants uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-326 | World-state participants | World-state participants works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-327 | World-state participants | World-state participants records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-328 | World-state participants | World-state participants supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-329 | World-state participants | World-state participants handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-330 | World-state participants | World-state participants keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-331 | World-state participants | World-state participants keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-332 | World-state participants | World-state participants produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-333 | World-state participants | World-state participants preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-334 | World-state participants | World-state participants avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-335 | World-state participants | World-state participants validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-336 | World-state participants | World-state participants supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-337 | World-state participants | World-state participants keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-338 | World-state participants | World-state participants keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-339 | Persistence and migration | Persistence and migration accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-340 | Persistence and migration | Persistence and migration rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-341 | Persistence and migration | Persistence and migration rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-342 | Persistence and migration | Persistence and migration preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-343 | Persistence and migration | Persistence and migration reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-344 | Persistence and migration | Persistence and migration does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-345 | Persistence and migration | Persistence and migration remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-346 | Persistence and migration | Persistence and migration cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-347 | Persistence and migration | Persistence and migration survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-348 | Persistence and migration | Persistence and migration isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-349 | Persistence and migration | Persistence and migration preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-350 | Persistence and migration | Persistence and migration avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-351 | Persistence and migration | Persistence and migration uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-352 | Persistence and migration | Persistence and migration works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-353 | Persistence and migration | Persistence and migration records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-354 | Persistence and migration | Persistence and migration supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-355 | Persistence and migration | Persistence and migration handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-356 | Persistence and migration | Persistence and migration keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-357 | Persistence and migration | Persistence and migration keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-358 | Persistence and migration | Persistence and migration produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-359 | Persistence and migration | Persistence and migration preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-360 | Persistence and migration | Persistence and migration avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-361 | Persistence and migration | Persistence and migration validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-362 | Persistence and migration | Persistence and migration supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-363 | Persistence and migration | Persistence and migration keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-364 | Persistence and migration | Persistence and migration keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-365 | Map snapshots and presentation seams | Map snapshots and presentation seams accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-366 | Map snapshots and presentation seams | Map snapshots and presentation seams rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-367 | Map snapshots and presentation seams | Map snapshots and presentation seams rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-368 | Map snapshots and presentation seams | Map snapshots and presentation seams preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-369 | Map snapshots and presentation seams | Map snapshots and presentation seams reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-370 | Map snapshots and presentation seams | Map snapshots and presentation seams does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-371 | Map snapshots and presentation seams | Map snapshots and presentation seams remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-372 | Map snapshots and presentation seams | Map snapshots and presentation seams cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-373 | Map snapshots and presentation seams | Map snapshots and presentation seams survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-374 | Map snapshots and presentation seams | Map snapshots and presentation seams isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-375 | Map snapshots and presentation seams | Map snapshots and presentation seams preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-376 | Map snapshots and presentation seams | Map snapshots and presentation seams avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-377 | Map snapshots and presentation seams | Map snapshots and presentation seams uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-378 | Map snapshots and presentation seams | Map snapshots and presentation seams works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-379 | Map snapshots and presentation seams | Map snapshots and presentation seams records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-380 | Map snapshots and presentation seams | Map snapshots and presentation seams supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-381 | Map snapshots and presentation seams | Map snapshots and presentation seams handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-382 | Map snapshots and presentation seams | Map snapshots and presentation seams keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-383 | Map snapshots and presentation seams | Map snapshots and presentation seams keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-384 | Map snapshots and presentation seams | Map snapshots and presentation seams produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-385 | Map snapshots and presentation seams | Map snapshots and presentation seams preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-386 | Map snapshots and presentation seams | Map snapshots and presentation seams avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-387 | Map snapshots and presentation seams | Map snapshots and presentation seams validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-388 | Map snapshots and presentation seams | Map snapshots and presentation seams supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-389 | Map snapshots and presentation seams | Map snapshots and presentation seams keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-390 | Map snapshots and presentation seams | Map snapshots and presentation seams keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-391 | Conditions and availability | Conditions and availability accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-392 | Conditions and availability | Conditions and availability rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-393 | Conditions and availability | Conditions and availability rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-394 | Conditions and availability | Conditions and availability preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-395 | Conditions and availability | Conditions and availability reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-396 | Conditions and availability | Conditions and availability does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-397 | Conditions and availability | Conditions and availability remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-398 | Conditions and availability | Conditions and availability cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-399 | Conditions and availability | Conditions and availability survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-400 | Conditions and availability | Conditions and availability isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-401 | Conditions and availability | Conditions and availability preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-402 | Conditions and availability | Conditions and availability avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-403 | Conditions and availability | Conditions and availability uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-404 | Conditions and availability | Conditions and availability works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-405 | Conditions and availability | Conditions and availability records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-406 | Conditions and availability | Conditions and availability supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-407 | Conditions and availability | Conditions and availability handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-408 | Conditions and availability | Conditions and availability keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-409 | Conditions and availability | Conditions and availability keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-410 | Conditions and availability | Conditions and availability produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-411 | Conditions and availability | Conditions and availability preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-412 | Conditions and availability | Conditions and availability avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-413 | Conditions and availability | Conditions and availability validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-414 | Conditions and availability | Conditions and availability supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-415 | Conditions and availability | Conditions and availability keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-416 | Conditions and availability | Conditions and availability keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-417 | Events and diagnostics | Events and diagnostics accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-418 | Events and diagnostics | Events and diagnostics rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-419 | Events and diagnostics | Events and diagnostics rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-420 | Events and diagnostics | Events and diagnostics preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-421 | Events and diagnostics | Events and diagnostics reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-422 | Events and diagnostics | Events and diagnostics does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-423 | Events and diagnostics | Events and diagnostics remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-424 | Events and diagnostics | Events and diagnostics cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-425 | Events and diagnostics | Events and diagnostics survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-426 | Events and diagnostics | Events and diagnostics isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-427 | Events and diagnostics | Events and diagnostics preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-428 | Events and diagnostics | Events and diagnostics avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-429 | Events and diagnostics | Events and diagnostics uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-430 | Events and diagnostics | Events and diagnostics works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-431 | Events and diagnostics | Events and diagnostics records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-432 | Events and diagnostics | Events and diagnostics supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-433 | Events and diagnostics | Events and diagnostics handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-434 | Events and diagnostics | Events and diagnostics keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-435 | Events and diagnostics | Events and diagnostics keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-436 | Events and diagnostics | Events and diagnostics produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-437 | Events and diagnostics | Events and diagnostics preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-438 | Events and diagnostics | Events and diagnostics avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-439 | Events and diagnostics | Events and diagnostics validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-440 | Events and diagnostics | Events and diagnostics supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-441 | Events and diagnostics | Events and diagnostics keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-442 | Events and diagnostics | Events and diagnostics keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-443 | Provider registration and isolation | Provider registration and isolation accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-444 | Provider registration and isolation | Provider registration and isolation rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-445 | Provider registration and isolation | Provider registration and isolation rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-446 | Provider registration and isolation | Provider registration and isolation preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-447 | Provider registration and isolation | Provider registration and isolation reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-448 | Provider registration and isolation | Provider registration and isolation does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-449 | Provider registration and isolation | Provider registration and isolation remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-450 | Provider registration and isolation | Provider registration and isolation cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-451 | Provider registration and isolation | Provider registration and isolation survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-452 | Provider registration and isolation | Provider registration and isolation isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-453 | Provider registration and isolation | Provider registration and isolation preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-454 | Provider registration and isolation | Provider registration and isolation avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-455 | Provider registration and isolation | Provider registration and isolation uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-456 | Provider registration and isolation | Provider registration and isolation works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-457 | Provider registration and isolation | Provider registration and isolation records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-458 | Provider registration and isolation | Provider registration and isolation supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-459 | Provider registration and isolation | Provider registration and isolation handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-460 | Provider registration and isolation | Provider registration and isolation keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-461 | Provider registration and isolation | Provider registration and isolation keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-462 | Provider registration and isolation | Provider registration and isolation produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-463 | Provider registration and isolation | Provider registration and isolation preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-464 | Provider registration and isolation | Provider registration and isolation avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-465 | Provider registration and isolation | Provider registration and isolation validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-466 | Provider registration and isolation | Provider registration and isolation supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-467 | Provider registration and isolation | Provider registration and isolation keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-468 | Provider registration and isolation | Provider registration and isolation keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-469 | Passage bridge boundary | Passage bridge boundary accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-470 | Passage bridge boundary | Passage bridge boundary rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-471 | Passage bridge boundary | Passage bridge boundary rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-472 | Passage bridge boundary | Passage bridge boundary preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-473 | Passage bridge boundary | Passage bridge boundary reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-474 | Passage bridge boundary | Passage bridge boundary does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-475 | Passage bridge boundary | Passage bridge boundary remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-476 | Passage bridge boundary | Passage bridge boundary cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-477 | Passage bridge boundary | Passage bridge boundary survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-478 | Passage bridge boundary | Passage bridge boundary isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-479 | Passage bridge boundary | Passage bridge boundary preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-480 | Passage bridge boundary | Passage bridge boundary avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-481 | Passage bridge boundary | Passage bridge boundary uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-482 | Passage bridge boundary | Passage bridge boundary works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-483 | Passage bridge boundary | Passage bridge boundary records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-484 | Passage bridge boundary | Passage bridge boundary supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-485 | Passage bridge boundary | Passage bridge boundary handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-486 | Passage bridge boundary | Passage bridge boundary keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-487 | Passage bridge boundary | Passage bridge boundary keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-488 | Passage bridge boundary | Passage bridge boundary produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-489 | Passage bridge boundary | Passage bridge boundary preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-490 | Passage bridge boundary | Passage bridge boundary avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-491 | Passage bridge boundary | Passage bridge boundary validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-492 | Passage bridge boundary | Passage bridge boundary supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-493 | Passage bridge boundary | Passage bridge boundary keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-494 | Passage bridge boundary | Passage bridge boundary keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-495 | Character and spawn boundaries | Character and spawn boundaries accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-496 | Character and spawn boundaries | Character and spawn boundaries rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-497 | Character and spawn boundaries | Character and spawn boundaries rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-498 | Character and spawn boundaries | Character and spawn boundaries preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-499 | Character and spawn boundaries | Character and spawn boundaries reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-500 | Character and spawn boundaries | Character and spawn boundaries does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-501 | Character and spawn boundaries | Character and spawn boundaries remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-502 | Character and spawn boundaries | Character and spawn boundaries cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-503 | Character and spawn boundaries | Character and spawn boundaries survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-504 | Character and spawn boundaries | Character and spawn boundaries isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-505 | Character and spawn boundaries | Character and spawn boundaries preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-506 | Character and spawn boundaries | Character and spawn boundaries avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-507 | Character and spawn boundaries | Character and spawn boundaries uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-508 | Character and spawn boundaries | Character and spawn boundaries works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-509 | Character and spawn boundaries | Character and spawn boundaries records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-510 | Character and spawn boundaries | Character and spawn boundaries supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-511 | Character and spawn boundaries | Character and spawn boundaries handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-512 | Character and spawn boundaries | Character and spawn boundaries keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-513 | Character and spawn boundaries | Character and spawn boundaries keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-514 | Character and spawn boundaries | Character and spawn boundaries produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-515 | Character and spawn boundaries | Character and spawn boundaries preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-516 | Character and spawn boundaries | Character and spawn boundaries avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-517 | Character and spawn boundaries | Character and spawn boundaries validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-518 | Character and spawn boundaries | Character and spawn boundaries supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-519 | Character and spawn boundaries | Character and spawn boundaries keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-520 | Character and spawn boundaries | Character and spawn boundaries keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-521 | Multiplayer authority seams | Multiplayer authority seams accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-522 | Multiplayer authority seams | Multiplayer authority seams rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-523 | Multiplayer authority seams | Multiplayer authority seams rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-524 | Multiplayer authority seams | Multiplayer authority seams preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-525 | Multiplayer authority seams | Multiplayer authority seams reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-526 | Multiplayer authority seams | Multiplayer authority seams does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-527 | Multiplayer authority seams | Multiplayer authority seams remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-528 | Multiplayer authority seams | Multiplayer authority seams cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-529 | Multiplayer authority seams | Multiplayer authority seams survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-530 | Multiplayer authority seams | Multiplayer authority seams isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-531 | Multiplayer authority seams | Multiplayer authority seams preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-532 | Multiplayer authority seams | Multiplayer authority seams avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-533 | Multiplayer authority seams | Multiplayer authority seams uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-534 | Multiplayer authority seams | Multiplayer authority seams works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-535 | Multiplayer authority seams | Multiplayer authority seams records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-536 | Multiplayer authority seams | Multiplayer authority seams supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-537 | Multiplayer authority seams | Multiplayer authority seams handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-538 | Multiplayer authority seams | Multiplayer authority seams keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-539 | Multiplayer authority seams | Multiplayer authority seams keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-540 | Multiplayer authority seams | Multiplayer authority seams produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-541 | Multiplayer authority seams | Multiplayer authority seams preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-542 | Multiplayer authority seams | Multiplayer authority seams avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-543 | Multiplayer authority seams | Multiplayer authority seams validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-544 | Multiplayer authority seams | Multiplayer authority seams supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-545 | Multiplayer authority seams | Multiplayer authority seams keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-546 | Multiplayer authority seams | Multiplayer authority seams keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-547 | Performance and bounded work | Performance and bounded work accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-548 | Performance and bounded work | Performance and bounded work rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-549 | Performance and bounded work | Performance and bounded work rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-550 | Performance and bounded work | Performance and bounded work preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-551 | Performance and bounded work | Performance and bounded work reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-552 | Performance and bounded work | Performance and bounded work does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-553 | Performance and bounded work | Performance and bounded work remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-554 | Performance and bounded work | Performance and bounded work cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-555 | Performance and bounded work | Performance and bounded work survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-556 | Performance and bounded work | Performance and bounded work isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-557 | Performance and bounded work | Performance and bounded work preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-558 | Performance and bounded work | Performance and bounded work avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-559 | Performance and bounded work | Performance and bounded work uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-560 | Performance and bounded work | Performance and bounded work works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-561 | Performance and bounded work | Performance and bounded work records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-562 | Performance and bounded work | Performance and bounded work supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-563 | Performance and bounded work | Performance and bounded work handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-564 | Performance and bounded work | Performance and bounded work keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-565 | Performance and bounded work | Performance and bounded work keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-566 | Performance and bounded work | Performance and bounded work produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-567 | Performance and bounded work | Performance and bounded work preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-568 | Performance and bounded work | Performance and bounded work avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-569 | Performance and bounded work | Performance and bounded work validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-570 | Performance and bounded work | Performance and bounded work supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-571 | Performance and bounded work | Performance and bounded work keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-572 | Performance and bounded work | Performance and bounded work keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-573 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-574 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-575 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-576 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-577 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-578 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-579 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-580 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-581 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-582 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-583 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-584 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-585 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-586 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-587 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-588 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-589 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-590 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-591 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-592 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-593 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-594 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-595 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-596 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-597 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-598 | Platform and scene-lifecycle behavior | Platform and scene-lifecycle behavior keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-599 | Packaging, removal, and release | Packaging, removal, and release accepts the valid happy path. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-600 | Packaging, removal, and release | Packaging, removal, and release rejects missing required identity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-601 | Packaging, removal, and release | Packaging, removal, and release rejects a stale revision or handle. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-602 | Packaging, removal, and release | Packaging, removal, and release preserves deterministic ordering. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-603 | Packaging, removal, and release | Packaging, removal, and release reports a stable failure code. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-604 | Packaging, removal, and release | Packaging, removal, and release does not mutate immutable definitions. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-605 | Packaging, removal, and release | Packaging, removal, and release remains bounded at configured capacity. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-606 | Packaging, removal, and release | Packaging, removal, and release cleans up after teardown. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-607 | Packaging, removal, and release | Packaging, removal, and release survives repeated initialization. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-608 | Packaging, removal, and release | Packaging, removal, and release isolates a provider exception. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-609 | Packaging, removal, and release | Packaging, removal, and release preserves request causality. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-610 | Packaging, removal, and release | Packaging, removal, and release avoids duplicate publication. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-611 | Packaging, removal, and release | Packaging, removal, and release uses project-owned configuration. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-612 | Packaging, removal, and release | Packaging, removal, and release works without optional packages. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-613 | Packaging, removal, and release | Packaging, removal, and release records Not run evidence honestly. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-614 | Packaging, removal, and release | Packaging, removal, and release supports direct Laboratory entry. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-615 | Packaging, removal, and release | Packaging, removal, and release handles an unavailable provider. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-616 | Packaging, removal, and release | Packaging, removal, and release keeps scene loading outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-617 | Packaging, removal, and release | Packaging, removal, and release keeps character spawning outside the core. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-618 | Packaging, removal, and release | Packaging, removal, and release produces a privacy-safe diagnostic snapshot. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-619 | Packaging, removal, and release | Packaging, removal, and release preserves main-thread Unity boundaries. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-620 | Packaging, removal, and release | Packaging, removal, and release avoids per-frame reflection. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-621 | Packaging, removal, and release | Packaging, removal, and release validates duplicate stable IDs. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-622 | Packaging, removal, and release | Packaging, removal, and release supports removal and reinstall. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-623 | Packaging, removal, and release | Packaging, removal, and release keeps samples removable. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |
| EWRLD-T-624 | Packaging, removal, and release | Packaging, removal, and release keeps Editor code out of Runtime. | Planned fixture | Execute the package-specific case. | Expected behavior matches the approved Atlas contract. | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and non-ownership approved.
- [x] Identity taxonomy approved.
- [x] Scene and Passage boundaries approved.
- [x] Marker, discovery, fast-travel, map, state, and multiplayer seams approved.
- [x] MVP and deferred scope separated.
- [x] Laboratory and planned test registries defined.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared dependencies only.
- [ ] Editor code isolated.
- [ ] Definitions remain immutable.
- [ ] Root, context, planner, marker, discovery, and state lifecycle validated.
- [ ] Public API matches approved authority.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Package works without unrelated Echo packages.
- [ ] Atlas Laboratory passes.
- [ ] Samples remove cleanly.
- [ ] Direct-scene entry behaves as documented.

### 24.4 Quality gate

- [ ] Automated and manual tests pass.
- [ ] No blocker/critical defects remain.
- [ ] Performance targets measured and passed.
- [ ] Diagnostics actionable.
- [ ] Accessibility and privacy checks pass.
- [ ] Current Notes reconciled.

### 24.5 Distribution gate

- [ ] Manifest valid.
- [ ] Version/changelog/notices complete.
- [ ] Stable `.meta` files included.
- [ ] Tarball/Git install tested externally.
- [ ] Beta, RC, and stable evidence gates satisfied separately.
- [ ] Compatibility catalog updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing pattern | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Echo Systems Lab | Hub/trial scene names and mission locations | Introduce stable world/location definitions and Passage bridge incrementally | Existing Hub/Trial loop preserved | Keep old scene loader/config until parity |
| Rescuers2D | Menu/level/password destinations and character entry points | Add world/location/marker metadata beside existing flow | Direct and password travel still work | Retain original scene/spawn scripts |
| Hackulos | Zones, NPCs, vendors, enemies, corpse recovery, quest locations | Use Atlas semantic locations with project/RPG content and Passage/Fellowship bridges | Vertical slice travel/respawn/quest references pass | Keep game-owned registries until parity |

### 25.2 Preserve-until-parity rule

Existing scene loaders, spawn systems, save data, and location registries remain intact until Atlas works alone and then in the target project. Migration is incremental and reversible.

### 25.3 Migration tooling

Future tooling should detect scene-string references, propose stable IDs/bindings, preview aliases, preserve backups, report unresolved references, and never rewrite project assets silently.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EWRLD-R-001 | World package becomes a god-object for scene, level, quest, map, and save logic | High | High | Keep semantic topology and contracts only; neighboring authorities execute behavior | Any proposed concrete gameplay rule |
| EWRLD-R-002 | Location identity collapses into scene paths | High | High | Use domain IDs and separate scene-binding tokens | Any runtime API requiring raw scene name |
| EWRLD-R-003 | Scene and travel authority overlaps Passage | Medium | High | Atlas prepares semantic plans; Passage executes scene transitions | Any scene load call in core |
| EWRLD-R-004 | World-state provider becomes duplicate save system | Medium | High | Atlas owns semantic snapshots; Chronicle owns transport/files | Any slot/file API in Atlas |
| EWRLD-R-005 | Map requirements inflate core presentation | Medium | Medium | Expose neutral snapshots; UI owns rendering | Any mandatory map prefab or UI framework |
| EWRLD-R-006 | Generic flags become untyped project database | Medium | High | Use versioned participant records and stable schemas, not open string/object bags | Any arbitrary mutable dictionary |
| EWRLD-R-007 | Marker registries leak scene objects | Medium | Medium | Generational handles, scene cleanup, leak diagnostics | Marker survives owner scene unexpectedly |
| EWRLD-R-008 | Fast travel duplicates progression or objective access | Medium | Medium | Use read-only providers and structured reasons | Atlas starts owning unlock rules |
| EWRLD-R-009 | Multiplayer clients author shared world state | Medium | High | Server/host authority gate through Convergence bridge | Client commits shared context |
| EWRLD-R-010 | Huge worlds cause unbounded graph work | Medium | Medium | Bound catalogs, cache validated topology, document measured limits | Planner exceeds budget |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EWRLD-D-001 | Semantic Location IDs are independent from scenes | Approved | One scene may hold many locations and one location may use many scenes | Requires explicit bindings | No |
| EWRLD-D-002 | Atlas prepares travel; Passage executes scene transitions | Approved | Preserves one authority per concern | Requires bridge for full travel workflow | No |
| EWRLD-D-003 | World hierarchy and travel graph are separate | Approved | Parent grouping does not imply reachability | Designers author connections explicitly | No |
| EWRLD-D-004 | Marker selection is owned; spawning is not | Approved | Reusable arrival metadata without character coupling | Fellowship/project executes placement | No |
| EWRLD-D-005 | Discovery is knowledge, not progression access | Approved | Avoids duplicate Ascent/Objective authority | Fast travel evaluates separate conditions | No |
| EWRLD-D-006 | World-state records are typed/versioned participant payloads | Approved | Avoids universal untyped flag database | Chronicle remains transport authority | No |
| EWRLD-D-007 | Shared-world state defaults to host/server authority | Approved | Prevents client-authored shared truth | Requires Convergence bridge | No |
| EWRLD-D-008 | No mandatory map, navigation, Addressables, or streaming backend | Approved | Keeps core portable and independently testable | Providers ship separately | No |

### 27.2 Release-blocking questions

None for the pre-code feasibility foundation. Implementation details such as planner data structures, exact numeric bounds, async primitive usage, and scene-token representation must follow these contracts and be measured later.

### 27.3 Non-blocking later questions

- Whether generated/runtime worlds require a separate registration module.
- Whether very large worlds require hierarchical path planning or server partitions.
- Whether travel costs need a shared currency/time contract or remain metadata/providers.
- Whether map layout authoring belongs in Atlas Editor or a dedicated map package.
- Whether personal, party, and shared discovery become separate first-class state scopes.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Foundation | Approved pre-code contract | This document and feasibility record | Approved docs |
| M1 - Skeleton | Installable package anatomy | Manifest, assemblies, docs shell | Clean compile |
| M2 - Identities/topology | Definitions and validation | IDs, catalog, hierarchy, links | EditMode tests |
| M3 - Runtime context/planner | Authority, context, availability, routes | Core workflow | PlayMode tests |
| M4 - Markers/discovery/state | Markers, discovery, visits, snapshots | Full MVP | Laboratory evidence |
| M5 - Tooling | Setup, graph, validation, diagnostics | Editor workflow | Repeatability tests |
| M6 - Bridges/adoption | Passage/Chronicle/Characters first integrations | Separate bridge evidence | Integration Labs |
| M7 - Release | Distribution-ready package | Docs, tests, migration, package | External install |

### 28.2 Checkpoint rule

Every implementation checkpoint follows SFGSS-005, displays complete code in chat, explains each file and decision, includes exact Editor steps, tests, stop point, documentation updates, and commit guidance. Implementation remains locked until SUITE-DOC-33.

### 28.3 First recommended checkpoint

After the final suite readiness gate: package skeleton only. No runtime world behavior is authorized by this document alone.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.
Treat SFGSS-000 and SFGSS-002 through SFGSS-005 as suite authorities.
Treat The Atlas EchoWorld Feasibility Foundation v1.0.0 as the Level 2
pre-code authority for semantic world identity, topology, current context,
travel planning, scene bindings, markers, discovery, fast travel, map snapshots,
world-state participants, diagnostics, and optional bridges.

Do not let EchoWorld load scenes, spawn characters, move cameras, own objectives,
write save files, render maps, perform pathfinding, or own multiplayer transport.
The Passage executes scene travel. The Fellowship/project spawns characters.
The Chronicle transports saves. The Convergence owns network authority.
All empirical evidence remains Not run. Package implementation is locked until
SUITE-DOC-33. When implementation begins, show complete code and explain every
step so Jesse can enter and understand it himself.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Foundation version | 1.0.0 |
| Completed checkpoint | SUITE-DOC-22 |
| Implementation | Not started |
| Laboratory scenarios | 104 planned; all Not run |
| Test cases | 624 planned; all Not run |
| Known issues | No documentation blocker; empirical limits unknown |
| Next checkpoint | SUITE-DOC-23 Expansion Cross-Package Collision Review |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and plain responsibility are clear.
- [x] Ownership aligns with SFGSS-000.
- [x] Location identity is independent from scenes and asset GUIDs.
- [x] Passage, Fellowship, Chronicle, UI, Camera, Objectives, and Multiplayer boundaries are explicit.
- [x] MVP is independent and testable.
- [x] Data, lifecycle, failure, diagnostics, persistence, and removal are specified.
- [x] Laboratory and planned tests are fully registered.
- [x] All implementation evidence remains Not run.
- [x] No Isekai Studios identity or ownership is introduced.

### 30.2 Approval record

**Decision:** Approved feasibility foundation  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Implementation remains locked until SUITE-DOC-33. Scene streaming, procedural generation, world simulation, large-world scaling, provider-specific networking, and all empirical claims require later design or evidence.

---

## Foundation Completion Rule

This feasibility foundation is complete when a new collaborator can explain:

1. Why a semantic location is not a Unity scene.
2. What worlds, zones, locations, connections, bindings, and markers each own.
3. Why Atlas prepares travel but Passage executes it.
4. Why marker selection does not equal character spawning.
5. Why discovery differs from progression access and objective completion.
6. What Atlas world state may persist and what remains session-only.
7. How maps, saves, characters, AI, UI, and multiplayer connect optionally.
8. How stable IDs, aliases, and unknown records preserve compatibility.
9. How the package proves itself in isolation.
10. What evidence remains Not run.


---

## Graph Navigation

#sfgss/package #sfgss/wave/advanced #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
