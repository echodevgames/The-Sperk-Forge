
# The Wellspring - Runtime Object Pooling Package Specification

**Working document ID:** SFGSS-PKG-ECHOPOOL-001  
**Specification version:** 1.0.0  
**Status:** Approved  
**Technical package name:** EchoPool  
**Public title:** The Wellspring - Runtime Object Pooling  
**Package ID:** `com.echodevgames.echo-pool`  
**Runtime namespace:** `EchoDevGames.EchoPool`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoPool`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Last updated:** August 4, 2026

> “Draw what is needed, return what is finished, and let the spring remember the vessel.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoPool. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-005 and all approved Foundation and Impact authorities | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved pool identity, lifecycle, generational leases, capacity and exhaustion policy, scene/application scopes, automatic return, diagnostics, tooling, Laboratories, bridge contracts, and release gates | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Wellspring - Runtime Object Pooling  
**Technical identifier:** EchoPool  
**Flavor line:** Reuse the vessel without inheriting the purpose that filled it.  
**Plain-language subtitle:** General-purpose GameObject and Component reuse with explicit capacities, leases, return safety, scene ownership, and diagnostics.

**One-sentence ownership contract:**

> EchoPool owns reusable GameObject/Component pool definitions, runtime pool instances, prewarming, acquisition, generational leases, return validation, capacity and exhaustion policy, scene/application ownership, lifecycle callbacks, automatic-return coordination, and pool diagnostics; it does not own gameplay spawn decisions, enemy waves, projectile behavior, audio voices, UI virtualization, network spawning authority, save-state truth, or the unique reset rules of project content.

### 1.1 Elevator summary

The Wellspring provides one reliable place to borrow and return reusable Unity objects. A caller asks for a pool by stable ID, supplies a spawn context, and receives a concrete instance plus a generational lease. EchoPool decides whether an inactive instance can be reused, whether growth is permitted, whether the request must be rejected, and how the instance is tracked until it returns. The caller remains responsible for why the object exists and what its gameplay behavior means.

The package is deliberately more disciplined than a loose stack of inactive prefabs. It defines lifecycle ordering, scene and application scopes, bounded growth, overflow behavior, double-return protection, stale-handle rejection, external-destruction reconciliation, automatic return by time or completion signal, and structured statistics. Project scripts reset their own mutable content through a narrow callback rather than relying on reflection or a magical universal reset.

The public contract remains independent of Unity's internal storage implementation. Unity provides `ObjectPool<T>` and `IObjectPool<T>` APIs for reuse, but EchoPool adds stable pool identity, scene ownership, project-facing leases, diagnostics, setup validation, and cross-package integration. The implementation may use Unity pool primitives internally when they satisfy the approved behavior, but consumers never depend on that internal choice.

### 1.2 Why this belongs in The Sperk’s Forge

Pooling is repeatedly recreated for projectiles, VFX, pickups, interaction markers, decals, temporary UI-world objects, destructibles, enemy bodies, and environmental effects. The first version usually saves a few instantiations. The fifth version is handling scene unloads, objects destroyed behind its back, forgotten reset state, runaway growth, and a return call from a stale coroutine. Those are package-shaped problems rather than one-game rules.

| Source project or system | Existing need or failure pattern | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Explosions, debris, interaction effects, temporary hazards, and repeated level objects benefit from reuse | Focused gameplay components and scene-based development | Remove repeated local pool implementations and expose scene-safe cleanup |
| Don’t Get Vince’d | Hit effects, pickups, projectiles, combo feedback, and enemies can create rapid transient allocations | Event-driven combat and strong feedback | Keep combat/spawn rules outside the pool while standardizing reuse and reset |
| Echo Systems Lab | Projectile and mission systems demonstrate definition/runtime separation | Clear data and runtime ownership | Add stable IDs, leases, capacity policy, and diagnostics |
| Impact | Feedback providers may need reusable VFX or UI-world objects | Provider authority and semantic requests | Let a provider use EchoPool without making either core depend on the other |
| The Passage | Scene transitions require predictable cleanup | Explicit scene lifecycle | Add optional pre-unload coordination while remaining standalone |
| The Observatory | Pool counts, leaks, exhaustion, and stale operations need visibility | Structured diagnostics | Publish bounded snapshots through an optional bridge |
| Jukebot | Audio voices are already internally pooled | Separate audio authority | Explicitly refuse to replace or expose Jukebot's voice pool |

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title and documentation | Yes | “The Wellspring” may lead only when paired with the pooling responsibility |
| Setup guidance and tooltips | Yes | Terms such as draw, vessel, and return may decorate but not replace technical labels |
| Standalone Laboratory | Optional | Sample labels remain removable and project-neutral |
| Runtime API/type names | No lore-only names | Use `PoolDefinition`, `PoolHandle`, `PoolSpawnRequest`, and related technical names |
| Project data | No required Verse content | Games own prefabs, effects, enemies, projectiles, pickups, names, and presentation |


## 2. Problem Statement

### 2.1 Current problem

Uncoordinated pooling commonly fails in ways that are subtle during a prototype and expensive during integration:

- inactive objects are stored without a stable pool identity;
- returned objects keep old velocity, targets, timers, subscriptions, parents, or visual state;
- the same instance is returned twice or by a coroutine from its previous use;
- a stale reference controls an object that has already been borrowed for another purpose;
- pools grow without a hard limit and quietly become memory leaks;
- full pools destroy new requests, recycle active gameplay objects, or instantiate overflow without a declared policy;
- scene changes destroy active or inactive instances without reconciling pool records;
- persistent pools accidentally retain scene references and prevent cleanup;
- project code assumes every pool is global or every pool is scene-local;
- external `Destroy` calls corrupt counts and later produce null entries;
- one pool manager absorbs enemy spawning, projectile configuration, VFX rules, and network authority;
- samples work only because unrelated systems happen to be installed.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| SFGSS-000 | EchoPool owns general-purpose reuse and diagnostics | One authority per concern | Make lifecycle, ownership, and removal explicit |
| SFGSS-002 | Optional integrations must be visible and removable | Bridge-first dependency direction | No direct dependency on Passage, Observatory, Impact, networking, or project code |
| SFGSS-003 | Definitions and runtime state must remain separate | Immutable project-owned definitions | Keep generations, active leases, timing, counts, and scene records in runtime state |
| SFGSS-004 | Planned tests are not executed evidence | Complete pre-code registry | Keep every Laboratory, performance, and compatibility result `Not run` |
| Unity object lifecycle | Deactivation changes callback/update behavior while scene unload may destroy scene objects | Use Unity lifecycle deliberately | Define callback order and reconcile external destruction rather than assuming it cannot occur |

### 2.3 Consequences of doing nothing

- Every game implements another incompatible pool API.
- Gameplay scripts become coupled to specific prefab stacks or singleton managers.
- Object state leaks between uses and produces non-reproducible bugs.
- Scene unloads and direct-scene testing corrupt pool counts.
- Memory usage grows without clear limits or evidence.
- Audio, networking, UI, and spawning systems lose their own authority to a giant pooling manager.
- Optimization claims cannot be measured because there is no shared diagnostic surface.


## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one duplicate-safe pooling authority with an injectable service interface.
- Support reusable GameObject and Component prefabs without imposing gameplay meaning.
- Use stable pool IDs independent from display names and Unity asset GUIDs.
- Prewarm pools deterministically and allow bounded on-demand growth.
- Return a generational handle with every successful spawn.
- Reject stale, foreign, invalid, and already-returned handles safely.
- Define exact spawn, activation, return, deactivation, destruction, and shutdown callback order.
- Support application-session, scene, and owner-lease pool scopes.
- Provide safe default exhaustion behavior and explicit opt-in alternatives.
- Support manual, timed, and completion-signal returns.
- Reconcile externally destroyed instances and scene unloads.
- Expose bounded statistics, health, warnings, and diagnostic codes.
- Offer repeatable Editor setup, validation, repair, and stress tools.
- Prove the MVP in an isolated Wellspring Laboratory.

### 3.2 Non-goals

- Decide when or where enemies, projectiles, pickups, VFX, or world objects should spawn.
- Replace game-specific spawn directors, wave systems, encounter managers, or factories.
- Own projectile damage, enemy AI, animation, physics rules, inventory state, or objective progress.
- Replace Jukebot's internal audio voice pooling.
- Replace EchoUI list virtualization or general UI element recycling.
- Become a network object-spawn authority or silently pool provider-owned network identities.
- Pool Entities/DOTS data in the MVP.
- Provide a universal managed-object/collection pool API in the MVP.
- Serialize active pooled instances into save files.
- Reset arbitrary project fields through reflection.
- Guarantee that pooling improves every workload; performance claims require evidence.
- Load Addressable prefabs or remote content in the MVP.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project and one repeated prefab | Create a definition, prewarm a pool, spawn/return safely, and see counts without writing a manager |
| Gameplay programmer | Existing projectile, pickup, or VFX code | Request reuse through `IEchoPoolService` while preserving gameplay authority |
| Systems programmer | Multiple scenes and strict capacities | Configure scope, growth, exhaustion, callbacks, and diagnostics explicitly |
| Designer | Project-owned prefabs and tuning | Adjust prewarm, limits, policy, and labels through validated assets |
| Tester | Suspected leak or stale object state | Reproduce exhaustion, double return, scene unload, and reset behavior in the Laboratory |
| Integrator | Passage, Observatory, Impact, BuildTools, or future Multiplayer installed | Add only the bridge/provider needed and remove it without breaking either core |

### 3.4 Measurable success criteria

- The package installs into a clean supported Unity project with zero compile errors.
- The runtime core works with no other Sperk's Forge runtime package installed.
- A prewarmed instance can be spawned, returned, and spawned again with a new generation.
- A stale handle cannot return or mutate the reused instance.
- Double return and foreign-handle operations return structured failures without corrupting counts.
- Fixed-capacity exhaustion rejects safely by default.
- Optional temporary overflow is bounded and destroyed rather than retained on return.
- Scene-scoped pools reconcile after scene unload without leaving active records.
- External object destruction is detected and counted.
- Repeating setup and repair does not duplicate roots, assets, or catalog entries.
- Removing samples or optional bridges leaves the core compiling and functional.
- Every advertised capability has a planned SFGSS-004 test and remains `Not run` until executed.


## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo Unity developers building prototypes, jams, portfolio systems, or full games.
- Gameplay and systems programmers who need bounded object reuse.
- Designers configuring project-owned pools without editing package source.
- Testers investigating lifecycle, scene, reset, capacity, and allocation behavior.
- Package integrators connecting pooling to explicit providers or bridge packages.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| EPOOL-UC-001 | Create project pool configuration | Installer | Package installed | Project-owned configuration and catalog exist | MVP |
| EPOOL-UC-002 | Register a pool definition | Designer | Valid prefab and stable PoolId | Definition validates and becomes available | MVP |
| EPOOL-UC-003 | Prewarm a fixed pool | Programmer | Definition with initial capacity | Inactive instances are created and counted | MVP |
| EPOOL-UC-004 | Spawn an inactive instance | Gameplay code | Pool ready | Instance is configured, activated, and returned with a lease | MVP |
| EPOOL-UC-005 | Return a leased instance | Gameplay code | Active valid lease | Instance callbacks run and object returns inactive | MVP |
| EPOOL-UC-006 | Reject a stale lease | Gameplay code | Instance was returned and reused | Operation fails without touching current use | MVP |
| EPOOL-UC-007 | Reject a double return | Gameplay code | Lease already returned | Structured failure and diagnostic are produced | MVP |
| EPOOL-UC-008 | Grow on demand | Programmer | Growth allowed below hard limit | New instance is created and tracked | MVP |
| EPOOL-UC-009 | Reject on exhaustion | Programmer | No inactive slot and hard limit reached | Spawn fails safely with reason | MVP |
| EPOOL-UC-010 | Create bounded temporary overflow | Programmer | Overflow enabled and below overflow cap | Temporary instance spawns and is destroyed on return | MVP |
| EPOOL-UC-011 | Return after duration | Designer/programmer | Timed return requested | Instance returns once using selected time mode | MVP |
| EPOOL-UC-012 | Return on completion signal | Project adapter | Completion relay bound | One completion signal returns the active lease | MVP |
| EPOOL-UC-013 | Spawn into a destination scene | Gameplay code | Valid loaded scene | Instance belongs to requested scene scope | MVP |
| EPOOL-UC-014 | Unload a scene-scoped pool | Scene system | Scene closes | Records reconcile and pool scope closes | MVP |
| EPOOL-UC-015 | Detect external destruction | Project code destroys instance | Marker remains until destruction callback | Pool marks instance lost and repairs counts | MVP |
| EPOOL-UC-016 | Close an owner scope | Feature owner | Valid scope lease | Active/inactive members follow configured close policy | MVP |
| EPOOL-UC-017 | Inspect pool statistics | Tester | Runtime active | Snapshot reports bounded counts and failures | MVP |
| EPOOL-UC-018 | Reset all Laboratory pools | Tester | Laboratory running | All instances and counters return to baseline | MVP |
| EPOOL-UC-019 | Adopt existing root | First Light bridge | Valid root exists | Bridge initializes existing authority without duplication | Later bridge |
| EPOOL-UC-020 | Prepare for Passage unload | Passage bridge | Transition requests pre-unload cleanup | Scene scopes validate/flush before unload | Later bridge |
| EPOOL-UC-021 | Display Observatory panel | Diagnostics bridge | Both packages installed | Pool health is visible without core dependency | Later bridge |
| EPOOL-UC-022 | Pool Impact provider objects | Project/bridge provider | Impact provider and pool installed | Provider borrows reusable VFX without moving authority | Later bridge |

### 4.3 Explicitly unsupported use cases

- Using EchoPool as an enemy wave scheduler or encounter director.
- Returning any random GameObject that was not created and leased by the target pool.
- Sharing one lease across several owners without project-side coordination.
- Reclaiming an arbitrary active instance by default when capacity is exhausted.
- Mutating pool definition assets during play to carry active counts or sequence state.
- Saving an active `PoolHandle` and expecting it to remain valid after reload.
- Pooling network-spawned objects without a provider-specific authority contract.
- Treating simulation providers or Laboratory results as proof of real-project performance.


## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Pool definitions, catalogs, and configuration contracts.
- Runtime pool registry and pool instance lifecycle.
- Prewarming, inactive storage, bounded growth, and retention limits.
- Spawn validation and lease creation.
- Return validation, stale-handle rejection, and double-return protection.
- Pool scope creation, ownership, closeout, and scene reconciliation.
- Core lifecycle callback ordering.
- Timed return scheduling and generic completion relays.
- External-destruction detection and count repair.
- Pool statistics, health, histories, and diagnostics.
- Editor setup, validation, repair, preview, and stress tooling.
- Standalone Laboratory assets and sample-only controls.

### 5.2 The package does not own

- Why an object should spawn, who requested it, or what gameplay event produced it.
- Enemy encounters, waves, spawn points, loot tables, projectile firing, damage, or AI.
- Audio voice allocation inside Jukebot.
- UI screen/widget hierarchy or list virtualization inside EchoUI.
- Network ownership, replication, prediction, or provider spawn identities.
- Save/load reconstruction of gameplay objects.
- Project-specific reset rules beyond the explicit lifecycle interface.
- Camera, feedback, inventory, objectives, characters, abilities, world, or combat truth.
- General scene travel or unload authorization.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoPool interacts |
|---|---|---|
| Initial startup/prewarm order | First Light or project boot code | Optional startup bridge or direct API |
| Normal scene travel | The Passage | Optional pre-unload/transition bridge |
| Runtime diagnostics dashboard | The Observatory | Snapshot provider bridge |
| Gameplay spawn decisions | Project gameplay, EchoCharacters, AI, abilities, combat, world systems | Call `IEchoPoolService`; pool never selects intent |
| Transient feedback recipes | Impact | Optional provider/project adapter may borrow pooled objects |
| Audio voices | Jukebot | No integration; Jukebot retains internal authority |
| Global preferences | The Accord | Normally none; diagnostics verbosity may be project-configured, not persisted by pool |
| Save files and reconstruction | The Chronicle and project participants | Save semantic state, then recreate through gameplay systems |
| Scene loading | The Passage | Pool observes scenes standalone and optionally participates before unload |
| Project generation | The Workshop | Editor setup facade only |
| Build validation | The Foundry | Future Editor validator integration |
| Network spawning | EchoMultiplayer provider adapter | Provider-specific bridge; no core network dependency |

### 5.4 Boundary tests

A proposed EchoPool feature belongs only when all answers remain safe:

1. Is the feature fundamentally about reusing an instance rather than deciding gameplay intent?
2. Can the core remain useful without another Echo package?
3. Does the feature avoid owning project-specific state reset?
4. Can a missing optional collaborator degrade visibly without compile failure?
5. Does removal leave project data and neighboring authorities intact?
6. Would a provider, bridge, sample utility, or project adapter be cleaner?
7. Is the behavior deterministic enough to validate with pool-local tests?

Features that fail these tests move to project code, another package, a provider adapter, or a later explicit module.


## 6. Independence Contract

Independence is a release gate.

### 6.1 Standalone guarantees

EchoPool must:

- compile with only declared Unity dependencies;
- function without First Light, Passage, Observatory, Impact, Jukebot, EchoUI, Chronicle, or Workshop;
- never reference project assemblies from the core;
- accept direct service injection through `IEchoPoolService`;
- keep project-owned prefabs and configuration outside immutable package source;
- detect missing configuration and invalid definitions before spawning;
- expose useful diagnostics without The Observatory;
- create only its own missing authority during direct-scene development;
- allow samples and optional modules to be removed safely;
- keep all Unity object operations on the main thread;
- avoid Resources-path, tag, layer, scene-name, or build-index assumptions.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Planned evidence |
|---|---|---|
| Installed alone | Core compiles, root initializes, Laboratory operates | Clean-project installation and Laboratory run |
| Enter Laboratory directly | Development initializer creates one root only when absent | PlayMode/direct-scene test |
| Optional bridge absent | Core exposes direct API and no missing-type errors | Compile/removal test |
| Passage absent | Scene events reconcile scopes using standalone behavior | Multi-scene Laboratory test |
| Observatory absent | Local snapshot/log surfaces remain available | Diagnostics test |
| Impact absent | No feedback/VFX provider assumption | Compile and sample-removal test |
| Duplicate root present | Duplicate is rejected before prewarm or subscriptions | Lifecycle test |
| Required configuration missing | Initialization fails safely with blocker diagnostic | Configuration test |
| Sample content deleted | Runtime and Editor assemblies still compile | Sample-removal test |
| External instance destroyed | Count is repaired and warning is emitted | Runtime lifecycle test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine Core | Platform | Yes | Unity 6000.0 | GameObject, Component, Transform, ScriptableObject, scenes, timing | Package cannot function without Unity |
| Unity Test Framework | Test-only | Yes for tests | Version selected and verified at implementation | EditMode and PlayMode test assemblies | Runtime remains unaffected |
| uGUI/TMP | Sample-only if selected | No | Evidence pending | Optional Laboratory dashboard | Sample can be omitted or removed |
| Other Echo packages | Optional bridge only | No | Per bridge specification | Explicit integrations | Remove bridge first; cores remain functional |

### 6.4 Forbidden dependencies

- Another Sperk's Forge runtime package in the core manifest or runtime assembly.
- UnityEditor types in runtime assemblies.
- Sample, Laboratory, test, or project assemblies in production runtime.
- Reflection-based discovery of arbitrary poolable components or peer packages.
- Hidden Resources folders or package-owned project prefab copies.
- Provider SDKs, networking packages, Addressables, Cinemachine, Input System, or physics modules in the neutral MVP core.
- Unlicensed or non-redistributable sample content.


## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| EPOOL-CAP-001 | Duplicate-safe authority | Claim one root before side effects | Approved | Yes | Runtime | Injectable interface remains available |
| EPOOL-CAP-002 | Stable pool definitions | Project-owned prefab, ID, capacity, scope, and policy assets | Approved | Yes | Runtime/Data | Immutable during play |
| EPOOL-CAP-003 | Pool catalog | Explicit collection of definitions | Approved | Yes | Runtime/Data | No Resources scan |
| EPOOL-CAP-004 | Prewarm | Create inactive instances before use | Approved | Yes | Runtime | Sync and incremental paths |
| EPOOL-CAP-005 | Spawn request/result | Validate and activate an instance | Approved | Yes | Runtime | Returns generational lease |
| EPOOL-CAP-006 | Safe return | Validate and deactivate/release | Approved | Yes | Runtime | Structured result |
| EPOOL-CAP-007 | Generational handles | Reject stale and recycled leases | Approved | Yes | Runtime | Not serializable durability |
| EPOOL-CAP-008 | Lifecycle callbacks | Created, spawned, returning, returned, destroyed | Approved | Yes | Runtime | Project resets its own state |
| EPOOL-CAP-009 | Fixed capacity | Never grow beyond prewarm/limit | Approved | Yes | Runtime | Deterministic memory |
| EPOOL-CAP-010 | On-demand growth | Grow below hard limit | Approved | Yes | Runtime | Main-thread only |
| EPOOL-CAP-011 | Reject exhaustion | Fail safely when unavailable | Approved | Yes | Runtime | Default policy |
| EPOOL-CAP-012 | Temporary overflow | Bounded non-retained overflow | Approved | Yes | Runtime | Opt-in and diagnosed |
| EPOOL-CAP-013 | Maximum retained count | Destroy excess returns beyond retained limit | Approved | Yes | Runtime | Bounded idle memory |
| EPOOL-CAP-014 | Application scope | Pool survives normal scene changes | Approved | Yes | Runtime | Root-owned inactive container |
| EPOOL-CAP-015 | Scene scope | Pool and instances tied to one loaded scene | Approved | Yes | Runtime | Reconciled on unload |
| EPOOL-CAP-016 | Owner-lease scope | Feature owns a closable pool scope | Approved | Yes | Runtime | Close policy explicit |
| EPOOL-CAP-017 | Timed return | Return after scaled or unscaled duration | Approved | Yes | Runtime | Uses root clock |
| EPOOL-CAP-018 | Completion relay | Return when project/provider signals completion | Approved | Yes | Runtime | Signal exactly once |
| EPOOL-CAP-019 | External-destruction detection | Reconcile objects destroyed outside pool | Approved | Yes | Runtime | Warn, repair, optionally replenish later |
| EPOOL-CAP-020 | Scene reconciliation | Handle scene unload and lost instances | Approved | Yes | Runtime | Passage bridge improves pre-unload behavior |
| EPOOL-CAP-021 | Statistics | Active, inactive, created, destroyed, failures, overflow | Approved | Yes | Runtime/Diagnostics | Bounded snapshots |
| EPOOL-CAP-022 | Setup/repair | Create assets/root and repair safe omissions | Approved | Yes | Editor | Non-destructive |
| EPOOL-CAP-023 | Validator | IDs, prefabs, capacity, callback, scene, duplicate checks | Approved | Yes | Editor | Stable diagnostic codes |
| EPOOL-CAP-024 | Stress Laboratory | Burst, churn, exhaustion, scene, stale-handle tests | Approved | Yes | Sample/Test | Isolated proof |
| EPOOL-CAP-025 | Passage bridge | Pre-unload flush and transition health | Approved | No | Bridge | Separate artifact |
| EPOOL-CAP-026 | Observatory bridge | Pool health panel/provider | Approved | No | Bridge | Separate artifact |
| EPOOL-CAP-027 | First Light bridge | Ordered prewarm startup step | Approved | No | Bridge | Separate artifact |
| EPOOL-CAP-028 | Impact provider integration | Reusable feedback objects | Proposed | No | Provider/Project | Does not change core authority |
| EPOOL-CAP-029 | Physics reset modules | Rigidbody/Rigidbody2D reset helpers | Deferred | No | Optional module | Avoid core physics dependency |
| EPOOL-CAP-030 | Particle completion adapter | Return when ParticleSystem stops | Deferred | No | Optional module | Separate feature assembly/sample |
| EPOOL-CAP-031 | Reclaim oldest active | Force-return active object on exhaustion | Experimental/Deferred | No | Runtime option | Dangerous; requires evidence and explicit semantics |
| EPOOL-CAP-032 | Addressable prefab provider | Async load/release of remote prefab assets | Deferred | No | Provider | Separate package/provider |
| EPOOL-CAP-033 | Managed-object pooling | Non-Unity object pooling facade | Deferred | No | Runtime | Not required for GameObject MVP |
| EPOOL-CAP-034 | DOTS/Entities pooling | Entity reuse | Rejected for this package | No | N/A | Different lifecycle/architecture |

### 7.2 MVP capability set

The smallest complete release includes:

- one duplicate-safe `EchoPoolRoot` and `IEchoPoolService`;
- project-owned configuration, catalog, and prefab-based `PoolDefinition` assets;
- stable pool IDs;
- deterministic prewarm and bounded on-demand growth;
- generational spawn leases and validated returns;
- safe fixed-capacity rejection and bounded temporary overflow;
- application, scene, and owner-lease scopes;
- exact lifecycle callbacks;
- timed and completion-signal returns;
- external-destruction and scene-unload reconciliation;
- statistics, local diagnostics, setup, validation, repair, and one isolated Laboratory.

### 7.3 Later capability set

- Separate First Light, Passage, Observatory, Foundry, Impact/provider, and Multiplayer integrations.
- Physics2D, Physics3D, ParticleSystem, animation-event, and VFX Graph reset/return adapters.
- Addressable prefab providers and asynchronous asset lifetime.
- Advanced replenishment and memory-pressure policies.
- Per-platform diagnostic sampling profiles.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Force-reclaim oldest active instance | Experimental/deferred | Can remove a live gameplay object unexpectedly | Proven genre-specific need and explicit cancellation contract |
| Unlimited growth | Rejected | Hides leaks and defeats bounded memory goals | Never for stable core |
| Reflection-based universal reset | Rejected | Fragile, expensive, and semantically unsafe | Never |
| Automatic enemy/projectile configuration | Rejected | Belongs to gameplay factories/spawn systems | Never in core |
| Save active pool leases | Rejected | Handles are session-local and generational | Save semantic owner state instead |
| Network pooling in core | Rejected | Provider authority and identity rules differ | Separate provider adapter |
| Addressables in MVP | Deferred | Adds provider lifetime and async failure concerns | Dedicated provider specification |
| General collection pooling API | Deferred | Unity already exposes collection/pool primitives; package promise is GameObject lifecycle | Repeated suite-wide need proven |


## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | `EchoPoolConfiguration`, `PoolCatalog`, `PoolDefinition`, capacity, scope defaults, policies, diagnostic settings | Active counts, instance references, generations, timers, scene handles, owner leases |
| Runtime state/behavior | Root, registry, pool runtimes, inactive storage, instance records, handles, scope records, return schedules, statistics | Editor logic, game-specific spawn decisions, saved gameplay truth |
| Presentation/feedback | Optional Laboratory dashboard, inspectors, reports, Observatory bridge | Pool authority, return rules, gameplay behavior |

### 8.2 Component topology

```mermaid
flowchart TD
    Caller[Project gameplay or provider] --> Service[IEchoPoolService]
    Root[EchoPoolRoot] --> Service
    Root --> Registry[PoolRegistry]
    Registry --> RuntimeA[PoolRuntime A]
    Registry --> RuntimeB[PoolRuntime B]
    RuntimeA --> Records[Instance records + generations]
    RuntimeA --> Inactive[Inactive storage]
    RuntimeA --> Scope[Scope ownership]
    RuntimeA --> Clock[Return schedules]
    RuntimeA --> Stats[Pool statistics]
    Definition[PoolDefinition] --> RuntimeA
    Catalog[PoolCatalog] --> Registry
    Instance[Pooled GameObject + marker] --> Records
    Instance --> Callbacks[IPoolable callbacks]
    Bridge[Optional bridge/provider] --> Service
    Diagnostics[Standalone diagnostics / Observatory bridge] <-- Stats
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | Yes for the default session service; scene-local pools remain owned by that root |
| Root type | `EchoPoolRoot` implementing `IEchoPoolService` |
| Duplicate behavior | Reject duplicate before catalogs, scene subscriptions, prewarm, or containers are created |
| Initialization trigger | Explicit initialization from configuration; optional `Awake` convenience must claim first and initialize once |
| Persistence | Configurable; default application-session `DontDestroyOnLoad` authority |
| Shutdown behavior | Stop accepting spawns, close scopes, return/destroy according to shutdown policy, dispose schedules, unsubscribe, emit final report |
| Direct-scene behavior | Development initializer creates only a missing EchoPool authority and identifies development mode |
| Test injection seam | `IEchoPoolService`, clock, instance factory, and diagnostic sink interfaces |

### 8.4 Pool identity and runtime model

A `PoolDefinition` identifies one reusable prefab contract. It contains a stable `PoolId`, display name, prefab reference, initial capacity, maximum active/retained counts, growth and exhaustion policy, default scope, reset options owned by the core, and diagnostic labels.

A `PoolRuntime` is created from a definition and a scope context. Multiple runtime pools may use the same definition only when their scope keys differ explicitly. Example: one application pool for global impact VFX and one scene pool for temporary room debris. Runtime pools never write counts back into the definition asset.

Each instantiated object receives an internal `PooledInstanceMarker` and one `InstanceRecord` containing:

- pool runtime identity;
- stable slot/record index;
- current generation;
- inactive, spawning, active, returning, lost, or destroyed state;
- overflow status;
- scope and scene ownership;
- current return schedule;
- creation and last-use timestamps for diagnostics only.

### 8.5 Lease model

`PoolHandle` is a small session-local value containing root identity, runtime pool identity, record index, and generation. A handle is valid only while its exact generation is active.

When an instance returns, its generation is closed. When it is borrowed again, the generation increments. Calls made through an older handle fail as `StaleHandle` even if they still contain a reference to the same GameObject. This prevents a delayed coroutine, animation event, or prior owner from returning the instance during a later use.

The spawn result may expose the GameObject and requested Component for ordinary Unity work, but the handle remains the authoritative return token.

### 8.6 Spawn lifecycle sequence

1. Validate root state, caller request, PoolId, scope, scene, parent, and requested component.
2. Resolve or create the matching `PoolRuntime` if policy permits.
3. Acquire one inactive record, grow below the hard limit, create bounded overflow, or return an exhaustion failure.
4. Mark the record `Spawning`, increment/assign its active generation, and cancel any old schedule.
5. Keep the GameObject inactive while assigning destination scene, parent, local/world transform, and core reset policy.
6. Invoke `IPoolable.OnPoolSpawnPreparing` on cached callback receivers.
7. Activate the GameObject when the request requires activation.
8. Invoke `IPoolable.OnPoolSpawned` after activation and after the lease is valid.
9. Mark the record `Active`, register automatic return if requested, update statistics, and raise the semantic spawn event.
10. Return `PoolSpawnResult` containing the instance, requested component if any, and the generational handle.

A callback failure is isolated and reported. The default behavior aborts the spawn, safely returns/destroys the instance, and reports `CallbackFailed`; a project cannot receive a half-initialized success.

### 8.7 Return lifecycle sequence

1. Validate root identity, runtime pool identity, record index, generation, and active state.
2. Reject foreign, stale, already-returned, lost, or destroyed handles without modifying the current record.
3. Mark the record `Returning` before user callbacks to block reentrant duplicate returns.
4. Cancel timed/completion schedules and detach generic completion relays.
5. Invoke `IPoolable.OnPoolReturning` while the object is still active unless shutdown policy explicitly skips callbacks.
6. Deactivate the GameObject.
7. Apply core-owned parent/scene/transform reset and clear lease metadata.
8. Invoke `IPoolable.OnPoolReturned` after deactivation.
9. Retain the instance if the pool has inactive capacity; otherwise destroy it and mark the record destroyed.
10. Close the generation, update statistics, and raise the semantic return event.

Returning a temporary overflow instance always destroys it after lifecycle callbacks. Returning above `MaximumRetained` destroys the excess rather than growing idle memory.

### 8.8 Lifecycle callback contract

```csharp
public interface IPoolable
{
    void OnPoolCreated(in PoolCreationContext context);
    void OnPoolSpawnPreparing(in PoolSpawnContext context);
    void OnPoolSpawned(in PoolSpawnContext context);
    void OnPoolReturning(in PoolReturnContext context);
    void OnPoolReturned(in PoolReturnContext context);
    void OnPoolDestroyed(in PoolDestructionContext context);
}
```

The exact signatures remain subject to implementation naming review, but the semantic order is approved. Callback receiver lists are cached at instance creation. The core does not search the hierarchy on every spawn or return.

Callbacks must be idempotent where practical and must not return the same active lease recursively. Project code owns resets such as health, targets, damage state, animation triggers, velocity, particle state, subscriptions, and timers. Optional reset modules may provide common adapters later.

### 8.9 Capacity and exhaustion model

Each runtime pool distinguishes:

- `InitialCapacity`: target prewarm count;
- `MaximumActive`: hard count of retained plus active non-overflow instances;
- `MaximumRetained`: maximum inactive objects kept after return;
- `GrowthBatchSize`: optional batch count for incremental growth;
- `MaximumTemporaryOverflow`: additional bounded instances that are never retained.

Approved growth policies:

- `Fixed`: prewarm/registered instances only; no retained growth.
- `GrowOnDemand`: create below `MaximumActive` when no inactive instance exists.

Approved MVP exhaustion policies:

- `Reject`: default; return a structured failure.
- `TemporaryOverflow`: create only below `MaximumTemporaryOverflow`; destroy on return.

`ReclaimOldestActive` is deferred because it changes gameplay state and requires an explicit cancellation/ownership contract.

### 8.10 Scope and scene ownership

Approved scope kinds:

- `Application`: runtime pool and inactive container survive normal scene changes under the persistent root.
- `Scene`: runtime pool belongs to one loaded Unity scene. Active and inactive objects are assigned to that scene and are expected to be destroyed when it unloads.
- `OwnerLease`: runtime pool belongs to a closable `PoolScopeHandle` created by a feature or project system.

A spawn request may specify a loaded destination scene or a parent whose scene becomes the destination. Ambiguous or unloaded destinations fail validation.

Standalone scene behavior subscribes to Unity scene-unload notifications. After unload, the root marks destroyed scene objects lost, closes the scope, repairs counts, and produces a report. Because unload notification is not a substitute for a pre-unload transaction, an optional Passage bridge may request validation, return, or cancellation before the scene operation begins.

### 8.11 Automatic return

The MVP supports:

- Manual return through the handle.
- Return after a positive duration using scaled or unscaled time selected by the request.
- Return after a generic `PoolCompletionRelay.SignalCompletion()` call.

Automatic-return state is runtime-only and bound to the current generation. A timer or completion signal from an older generation is ignored as stale. Project-specific adapters, such as ParticleSystem completion or animation events, call the generic relay or return API rather than entering the core package.

### 8.12 External destruction and reconciliation

Every pooled instance carries an internal marker that notifies the root when Unity destroys the object. If destruction was not initiated by EchoPool:

1. the record becomes `Lost`/`ExternallyDestroyed`;
2. its active generation closes;
3. active/inactive counts are repaired;
4. a diagnostic and statistic increment are emitted;
5. automatic replenishment is deferred until a safe explicit prewarm/growth operation rather than instantiating from `OnDestroy`.

Destroyed Unity-object null semantics are handled centrally. Null slots are never returned as successful spawns.

### 8.13 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Authority claim | Error/blocker | Destroy/disable duplicate before side effects | EPOOL-001 |
| Missing configuration | Initialization | Blocking report | Root remains unavailable | EPOOL-002 |
| Duplicate/empty PoolId | Validation | Blocker | Definition not registered | EPOOL-003 |
| Missing prefab | Validation/prewarm | Blocker | Pool unavailable | EPOOL-004 |
| Invalid capacity relationship | Validation | Error | Pool unavailable until fixed | EPOOL-005 |
| Pool not found | Spawn | Structured failure | No object created | EPOOL-010 |
| Exhausted | Spawn | Structured failure/advisory | Reject or bounded overflow | EPOOL-011 |
| Invalid destination scene | Spawn | Structured failure | No object activated | EPOOL-012 |
| Callback failure during spawn | Spawn callback | Failure report | Abort and return/destroy instance | EPOOL-013 |
| Foreign handle | Return | Structured failure | Ignore operation | EPOOL-020 |
| Stale handle | Return/schedule | Structured failure | Ignore operation | EPOOL-021 |
| Double return | Return | Structured failure | Ignore operation | EPOOL-022 |
| Externally destroyed instance | Marker/reconciliation | Warning | Repair counts; close lease | EPOOL-023 |
| Scene unloaded | Scene event | Scope report | Close scene pools and repair records | EPOOL-024 |
| Owner scope closed with active leases | Scope close | Policy result | Reject, return, or destroy per policy | EPOOL-025 |
| Timed return clock invalid | Schedule | Error | Cancel schedule; keep manual return | EPOOL-030 |
| Shutdown with active instances | Shutdown | Final report | Return/destroy according to policy | EPOOL-031 |


## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoPoolConfiguration` | Root defaults, diagnostics, clocks, shutdown and development policy | Optional configuration ID | No | Yes |
| `PoolCatalog` | Explicit ordered set of pool definitions | Yes if referenced durably | No | Yes |
| `PoolDefinition` | Prefab, PoolId, capacity, growth, exhaustion, scope, reset defaults | Yes, required | No | Yes |
| `PoolDiagnosticProfile` | History and sampling limits | Optional | No | Yes |
| `PoolLaboratoryProfile` | Sample-only stress presets | Sample ID only | No | Sample-owned |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `PoolRegistry` | Root | Root session | Rebuilt at initialization | Never saved |
| `PoolRuntime` | Registry/scope | Scope lifetime | Destroyed on scope close | Never saved |
| `InstanceRecord` | PoolRuntime | Instance lifetime | State machine controlled by pool | Never saved |
| `PoolHandle` | Caller/runtime | One active generation | Invalid after return/loss/shutdown | Never durable |
| `PoolScopeRecord` | Root | Application, scene, or owner lease | Closed explicitly or on scene/shutdown | Never saved |
| `ReturnSchedule` | Root clock service | One active generation | Cancel on return/loss | Never saved |
| `PoolStatistics` | Runtime | Root session/bounded history | Reset by explicit diagnostics action | Optional diagnostic export only |

### 9.3 Stable identifiers

- `PoolId` is a package/domain stable ID, not a Unity asset GUID or prefab name.
- Recommended format is a lowercase namespaced token such as `project.projectiles.player-basic` or a generated normalized UUID string, selected consistently by project policy.
- IDs are generated in the Editor, validated for emptiness and collisions, and never changed silently after release.
- Display names and prefab names may change without changing `PoolId`.
- A released ID change requires an alias/migration map if project-authored references depend on it.
- Runtime pool instance identity combines `PoolId` with scope identity; it is not a durable save ID.
- `PoolHandle` record indexes and generations are session-local runtime identifiers only.

### 9.4 ScriptableObject safety

Definitions remain immutable during Play Mode, Laboratory simulation, and runtime preview. The following must never be written into a definition asset:

- active/inactive counts;
- instance lists or scene objects;
- current generation or last handle;
- timers or completion subscriptions;
- statistics or leak histories;
- scene handles, owner leases, or root identity;
- external-destruction state;
- current overflow count.

Editor preview tools use detached models or temporary hidden objects and clean them deterministically.

### 9.5 Serialization and migration

EchoPool has no MVP save payload. Durable package data consists of Unity project assets and future setup receipts.

- Configuration and definitions follow SFGSS-003 stable-ID and schema rules.
- Public serialized enums must append values rather than reorder released numeric meanings.
- Removed policies remain readable long enough to migrate assets.
- Setup/migration tools preview changes, preserve source assets, and write explicit receipts.
- Unknown future fields must not be silently destroyed by a migration tool that claims round-trip preservation.
- Active handles, schedules, scene scopes, and instance records are never serialized for gameplay restoration.


## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `IEchoPoolService` | Interface | Injectable spawn, return, prewarm, scope, and query contract | Implemented by root or test double |
| `EchoPoolRoot` | MonoBehaviour | Default authority and service implementation | Scene/setup created; duplicate-safe |
| `EchoPoolConfiguration` | ScriptableObject | Root defaults and diagnostic limits | Project-owned |
| `PoolCatalog` | ScriptableObject | Explicit definition registry | Project-owned |
| `PoolDefinition` | ScriptableObject | One prefab pool contract | Project-owned |
| `PoolId` | Value type | Stable pool definition identity | Editor generated/project selected |
| `PoolHandle` | Readonly struct | Generational active lease token | Returned by successful spawn |
| `PoolScopeHandle` | Readonly struct/IDisposable pattern | Owner-lease scope token | Created by service |
| `PoolSpawnRequest` | Readonly struct | Position, rotation, parent, scene, activation, return policy, requested type | Caller-created |
| `PoolSpawnResult` | Readonly struct | Success/failure, object/component, handle, overflow, diagnostic | Service-created |
| `PoolReturnRequest` | Readonly struct | Handle, reason, optional policy metadata | Caller-created |
| `PoolReturnResult` | Readonly struct | Success/failure and final disposition | Service-created |
| `PoolPrewarmRequest` | Readonly struct | Definition/scope/count/budget | Caller-created |
| `PoolPrewarmResult` | Readonly struct | Created/reused/cancelled/failure counts | Service-created |
| `PoolScopeRequest` | Readonly struct | Application/scene/owner scope configuration | Caller-created |
| `PoolStatisticsSnapshot` | Readonly struct | Bounded point-in-time health and counts | Service-created |
| `PoolFailure` | Enum/value | Stable failure reason | Service-created |
| `PoolGrowthPolicy` | Enum | Fixed or GrowOnDemand | Definition |
| `PoolExhaustionPolicy` | Enum | Reject or TemporaryOverflow in MVP | Definition |
| `PoolScopeKind` | Enum | Application, Scene, OwnerLease | Definition/request |
| `PoolTimeMode` | Enum | Scaled or Unscaled | Automatic return |
| `IPoolable` | Interface | Project-owned lifecycle reset callbacks | Implemented by pooled components |
| `IPoolClock` | Interface | Testable scaled/unscaled scheduling | Root dependency |
| `IPoolInstanceFactory` | Interface | Main-thread instantiate/destroy seam | Root dependency/test double |
| `PoolCompletionRelay` | Component | Generic completion signal bound to current generation | Optional on pooled prefab |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread rule |
|---|---|---|---|---|
| `PoolInitializationState State` | Inspect root lifecycle | Root exists | Read-only state | Main thread |
| `bool TrySpawn(PoolId id, in PoolSpawnRequest request, out PoolSpawnResult result)` | Synchronous spawn/reuse | Ready root, valid request | Never throws for expected capacity/config failures | Main thread only |
| `bool TryReturn(in PoolReturnRequest request, out PoolReturnResult result)` | Return an active generation | Valid root and handle | Foreign/stale/double returns are structured failures | Main thread only |
| `Awaitable<PoolPrewarmResult> PrewarmAsync(in PoolPrewarmRequest request, CancellationToken token)` | Incremental main-thread prewarm | Ready root | Cancellation stops before next creation; existing created instances remain valid | Main thread continuation |
| `bool TryCreateScope(in PoolScopeRequest request, out PoolScopeHandle handle)` | Create owner or scene scope | Valid request | Failure leaves no partial scope | Main thread only |
| `bool TryCloseScope(PoolScopeHandle handle, PoolScopeClosePolicy policy, out PoolScopeCloseResult result)` | Close a scope deterministically | Valid current scope | Policy reports active lease conflicts | Main thread only |
| `bool TryGetStatistics(PoolId id, in PoolScopeQuery scope, out PoolStatisticsSnapshot snapshot)` | Read pool health | Pool exists | False if absent | Main thread only |
| `IReadOnlyList<PoolStatisticsSnapshot> GetAllStatisticsSnapshot()` | Read bounded snapshot list | Root ready | Detached read-only data | Main thread only |
| `bool IsHandleActive(PoolHandle handle)` | Validate current generation | Root ready | False for foreign/stale/lost | Main thread only |
| `void RequestDiagnosticsReset()` | Reset counters/history, not pool contents | Development permission | Emits reset event | Main thread only |
| `Awaitable<PoolShutdownReport> ShutdownAsync(...)` | Stop and dispose root | Root not already destroyed | Idempotent, bounded, reports active leases | Main thread orchestration |

Static convenience access may expose `EchoPoolRoot.Current`, but project code must be able to receive `IEchoPoolService` explicitly. Static access is never the only supported API.

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `Initialized` | Root | After catalog validation and required prewarm | Initialization report | No listener required |
| `PoolCreated` | Registry | After runtime pool becomes valid | Pool identity/scope snapshot | Diagnostic/presentation only |
| `InstanceSpawned` | PoolRuntime | After activation and callbacks succeed | PoolId, handle, scope, overflow flag | State already authoritative |
| `InstanceReturned` | PoolRuntime | After generation closes | PoolId, reason, retained/destroyed | State already authoritative |
| `SpawnRejected` | PoolRuntime | After safe failure | Failure/capacity snapshot | No retry implied |
| `InstanceLost` | PoolRuntime | After external destruction/reconciliation | Old handle identity and reason | Object may already be gone |
| `PoolExhausted` | PoolRuntime | On bounded exhaustion | Counts and policy | Rate-limited diagnostics recommended |
| `ScopeClosed` | Root | After close policy completes | Scope report | No active scope remains |
| `ShuttingDown` | Root | Before final disposal | Shutdown context | New spawns already rejected |
| `ShutdownCompleted` | Root | After cleanup | Final report | Root no longer usable |

Events occur after authoritative state changes. A listener failure cannot roll back pool truth.

### 10.4 Async and cancellation policy

- Ordinary spawn and return are synchronous main-thread operations.
- Incremental prewarm and shutdown may use Unity `Awaitable` because they can distribute instantiation/destruction across frames.
- Cancellation is cooperative between creations or cleanup batches.
- Objects already created by a cancelled prewarm remain valid inactive pool members unless the request explicitly uses an isolated rollback scope.
- No background thread may access UnityEngine.Object, scenes, transforms, or components.
- A destroyed root or application quit completes pending operations with structured cancellation/shutdown results.
- Addressable asynchronous prefab loading is outside the MVP and requires a provider specification.

### 10.5 API ergonomics

Novice path:

1. Create `EchoPoolConfiguration`, `PoolCatalog`, and `PoolDefinition` through the setup window.
2. Add/repair the root prefab or scene object.
3. Call the simple spawn method using the definition reference or `PoolId`.
4. Return through the `PoolHandle` or `PooledObject` convenience component.

Programmer path:

- inject `IEchoPoolService`;
- create explicit scopes;
- provide custom clock/factory test doubles;
- consume structured results and events;
- author optional adapters or bridges without modifying the core.


## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install EchoPool through a supported UPM route.
2. Open **Tools > Sperk's Forge > The Wellspring > Setup**.
3. Choose the project configuration folder and runtime-root strategy.
4. Preview the exact configuration, catalog, prefab/root, and documentation links to be created.
5. Apply create-only-safe operations.
6. Create one `PoolDefinition` from a selected project prefab.
7. Validate IDs, capacity, scope, callbacks, and prefab references.
8. Import/open the Standalone Wellspring Laboratory.
9. Run prewarm, spawn, return, exhaustion, stale-handle, and scene-scope tests.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create project configuration | Config/catalog assets | Nothing existing by default | Yes | Unity Undo/project backup | Setup receipt |
| Create runtime root prefab | Project-owned prefab | Optional selected Boot scene only with approval | Yes | Preview + Undo | Setup receipt |
| Create definition from prefab | PoolDefinition asset | Catalog only after confirmation | Yes | Undo | Definition report |
| Add definition to catalog | Catalog entry | Project catalog | Yes; duplicate-safe | Undo | Catalog diff |
| Repair missing marker/root refs | Missing safe fields/components | Selected project asset | Yes | Preview + Undo | Repair receipt |
| Regenerate duplicate PoolId | Selected unreleased definition | Definition and known project references only when mapped | Conditional | Backup + explicit confirmation | ID migration report |
| Prewarm preview | Temporary hidden objects or calculation | No durable asset | Yes | Automatic cleanup | Estimate report |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Wellspring Setup Window | Installer | Create/repair configuration, root, and catalog | No |
| Pool Definition Inspector | Designer | Edit capacity/policies and validate prefab/callbacks | No |
| Pool Catalog Inspector | Designer | Detect duplicate/missing definitions and IDs | No |
| Pool Capacity Preview | Programmer | Estimate retained/active/overflow configuration | No |
| Runtime Pool Monitor | Tester | View active/inactive/overflow/leak counts in Play Mode | Editor reads runtime API |
| Wellspring Stress Console | Tester | Drive Laboratory presets and export results | Sample/Editor only |
| Setup Facade | Workshop | Produce deterministic plans and apply approved operations | Editor only, ADR-001 |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| EPOOL-VAL-001 | Missing configuration | Blocker | Yes | Create-only safe |
| EPOOL-VAL-002 | Duplicate root in canonical scene/prefab | Blocker | Guided | No silent deletion |
| EPOOL-VAL-003 | Empty PoolId | Blocker | Yes | Only before release/with confirmation |
| EPOOL-VAL-004 | Duplicate PoolId | Blocker | Guided | No automatic reference rewrite |
| EPOOL-VAL-005 | Missing prefab | Blocker | No | No |
| EPOOL-VAL-006 | Prefab is scene instance instead of asset | Error | Guided | No |
| EPOOL-VAL-007 | Negative capacity or limit | Error | Yes | Clamp only with confirmation |
| EPOOL-VAL-008 | Initial capacity exceeds maximum active | Error | Yes | No silent policy change |
| EPOOL-VAL-009 | Maximum retained exceeds maximum active | Warning/Error by policy | Yes | No |
| EPOOL-VAL-010 | Overflow enabled with zero cap | Warning | Yes | No |
| EPOOL-VAL-011 | Scene scope configured for persistent-only use | Warning | Guided | No |
| EPOOL-VAL-012 | Completion relay missing for completion-return sample | Warning | Yes | Add with confirmation |
| EPOOL-VAL-013 | IPoolable callback type throws in preview | Error | No | No |
| EPOOL-VAL-014 | Package/sample asset placed in project-owned config slot | Warning | Guided copy | No overwrite |
| EPOOL-VAL-015 | Unbounded diagnostic history | Error | Yes | Set approved default with confirmation |


## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Supported/planned routes:

- Embedded package development.
- Local path reference.
- Git URL/tag installation.
- Tarball installation.
- Registry installation when a registry is approved.
- The Workshop selection through the ADR-001 setup facade.

Each route receives separate evidence under SFGSS-004. A successful local-path import does not prove tarball or Git support.

### 12.2 Minimal scene setup

Minimum production setup:

- one valid project-owned `EchoPoolConfiguration`;
- one explicit `PoolCatalog` containing at least one valid definition;
- one `EchoPoolRoot` referencing configuration/catalog;
- no required UI, EventSystem, input asset, scene name, layer, tag, or peer package.

The root may create its inactive application-scope container at runtime. Scene-scope containers live in their target scene and are clearly named for debugging.

### 12.3 Boot-scene setup

Normal production path:

1. Place the canonical root in the project Boot/preload scene or create it through a First Light integration step.
2. Claim duplicate safety before reading catalogs or prewarming.
3. Validate required definitions.
4. Prewarm only pools marked startup-required or requested by the project's startup plan.
5. Report readiness without blocking unrelated optional pools.

First Light remains optional. Direct project code may initialize the root explicitly.

### 12.4 Direct-scene setup

`EchoPoolDirectSceneInitializer` is a development-only helper:

- checks for an existing valid root;
- creates the configured development root only when absent;
- marks the runtime as direct-scene development;
- avoids prewarming unrelated project catalogs unless selected;
- rejects duplicates before side effects;
- is excluded/disabled from release builds by default.

### 12.5 Scene isolation rule

The Wellspring Laboratory contains only EchoPool, declared Unity dependencies, and redistributable sample assets. Passage, Observatory, Impact, Jukebot, EchoUI, and project gameplay code are absent from standalone proof. Any advertised bridge receives a separate Integration Laboratory.


## 13. Standalone Test Lab and Samples

### 13.1 Standalone Laboratory purpose

The **Wellspring Object Reuse Laboratory** proves that EchoPool can prewarm, spawn, return, grow, reject, overflow, schedule, reconcile scenes, detect stale/double returns, and report health without another Sperk's Forge package.

The Laboratory may contain a main scene plus additive scene fixtures. Together they remain one package-owned standalone sample.

### 13.2 Required Laboratory contents

- Main control scene with project-neutral primitive prefabs.
- At least one Component-return example.
- Fixed pool and grow-on-demand pool.
- Bounded temporary-overflow pool.
- Application-scope and scene-scope pools.
- Owner-lease scope controls.
- Manual, timed-scaled, timed-unscaled, and completion-relay return examples.
- Objects that deliberately reset visible state through `IPoolable`.
- An external-destroy test control.
- Stale-handle and double-return test controls.
- Burst/churn stress controls with bounded counts.
- Visual readout for active, inactive, created, destroyed, overflow, rejected, lost, and stale operations.
- Reset-to-baseline control.
- Sample README with exact controls and expected results.

### 13.3 Laboratory acceptance checklist

| Test ID | Action | Expected result | Automation | Status |
|---|---|---|---|---|
| EPOOL-LAB-001 | Enter Laboratory directly | Exactly one development root initializes | Manual + PlayMode | Not run |
| EPOOL-LAB-002 | Prewarm five instances | Five inactive objects exist, none active | Manual + PlayMode | Not run |
| EPOOL-LAB-003 | Spawn one instance | One active, four inactive, valid handle | Manual + PlayMode | Not run |
| EPOOL-LAB-004 | Return the handle | Object deactivates and counts restore | Manual + PlayMode | Not run |
| EPOOL-LAB-005 | Spawn returned instance | Same object may be reused with new generation | Manual + PlayMode | Not run |
| EPOOL-LAB-006 | Return old generation | Stale failure; current use remains active | Manual + PlayMode | Not run |
| EPOOL-LAB-007 | Return current handle twice | First succeeds, second is DoubleReturn | Manual + PlayMode | Not run |
| EPOOL-LAB-008 | Exhaust fixed pool | Additional request is rejected safely | Manual + PlayMode | Not run |
| EPOOL-LAB-009 | Grow below maximum | New retained object is created | Manual + PlayMode | Not run |
| EPOOL-LAB-010 | Reach grow hard limit | Further request follows exhaustion policy | Manual + PlayMode | Not run |
| EPOOL-LAB-011 | Use temporary overflow | Overflow spawns and is marked temporary | Manual + PlayMode | Not run |
| EPOOL-LAB-012 | Return temporary overflow | Overflow object is destroyed, not retained | Manual + PlayMode | Not run |
| EPOOL-LAB-013 | Exceed overflow cap | Request is rejected | Manual + PlayMode | Not run |
| EPOOL-LAB-014 | Timed scaled return | Object returns after scaled duration | Manual + PlayMode | Not run |
| EPOOL-LAB-015 | Pause scaled time | Scaled timer pauses | Manual | Not run |
| EPOOL-LAB-016 | Timed unscaled return while paused | Object still returns | Manual + PlayMode | Not run |
| EPOOL-LAB-017 | Signal completion | Bound generation returns exactly once | Manual + PlayMode | Not run |
| EPOOL-LAB-018 | Signal old completion after reuse | Stale signal is ignored | Manual + PlayMode | Not run |
| EPOOL-LAB-019 | Mutate visible state then return | IPoolable reset restores next spawn | Manual | Not run |
| EPOOL-LAB-020 | Destroy active object externally | Lost count increments and lease closes | Manual + PlayMode | Not run |
| EPOOL-LAB-021 | Destroy inactive object externally | Inactive count repairs | Manual + PlayMode | Not run |
| EPOOL-LAB-022 | Open additive scene pool | Scene pool initializes in target scene | Manual | Not run |
| EPOOL-LAB-023 | Unload additive scene | Scene records close without persistent leak | Manual + PlayMode | Not run |
| EPOOL-LAB-024 | Spawn application object then change scene | Object/pool survives as configured | Manual | Not run |
| EPOOL-LAB-025 | Close owner scope with no active leases | Scope closes cleanly | Manual + PlayMode | Not run |
| EPOOL-LAB-026 | Close owner scope with active lease using Reject | Close fails without destroying object | Manual + PlayMode | Not run |
| EPOOL-LAB-027 | Close owner scope using Return | Active members return/destroy by policy | Manual + PlayMode | Not run |
| EPOOL-LAB-028 | Burst spawn/return repeatedly | Counts remain coherent and bounded | Manual + Performance | Not run |
| EPOOL-LAB-029 | Throw from lifecycle callback fixture | Spawn/return fails safely and reports callback | Manual + PlayMode | Not run |
| EPOOL-LAB-030 | Duplicate root fixture | Duplicate rejected before prewarm | Manual + PlayMode | Not run |
| EPOOL-LAB-031 | Reset Laboratory | Pools/counters return to documented baseline | Manual | Not run |
| EPOOL-LAB-032 | Remove optional dashboard sample | Core still compiles and runs | Clean-project | Not run |
| EPOOL-LAB-033 | Quit with active objects | Shutdown report lists and cleans them | Manual + PlayMode | Not run |
| EPOOL-LAB-034 | Disable/re-enable root according to policy | No duplicate subscriptions or phantom records | Manual + PlayMode | Not run |
| EPOOL-LAB-035 | Request wrong Component type | Structured failure; no instance escapes | Manual + PlayMode | Not run |
| EPOOL-LAB-036 | Use foreign root handle | ForeignHandle failure | Manual + PlayMode | Not run |

### 13.4 Optional showcase and integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| Passage + Wellspring Scene Cleanup | EchoPool + Passage bridge | Pre-unload validation and scope cleanup | Depends on two authorities |
| Observatory Pool Panel | EchoPool + Observatory bridge | Display health and histories | Diagnostics peer required |
| Impact VFX Provider | EchoPool + Impact/provider | Reuse transient feedback objects | Provider semantics required |
| Network Provider Pool | EchoPool + selected EchoMultiplayer adapter | Provider-approved network object reuse | Networking authority/provider required |


## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

EchoPool is nonvisual. Runtime presentation is optional and diagnostic only.

- Core status is exposed through APIs, structured results, logs, and reports.
- The Laboratory may use sample-only controls and a dashboard.
- The Observatory bridge owns richer ongoing presentation when installed.
- EchoUI is not required and does not become a pool dependency.

### 14.2 Required states

Any setup window or Laboratory readout must distinguish:

- Uninitialized.
- Validating.
- Prewarming.
- Ready.
- Empty pool/catalog.
- Active use.
- Exhausted/rejected.
- Overflow active.
- Warning/lost instances.
- Shutting down.
- Failure/blocker.

### 14.3 Accessibility requirements

- Laboratory controls must support keyboard and mouse; controller support is required only if the sample declares it.
- Status cannot rely on color alone; labels and counts accompany visual states.
- Stress flashing or rapid object motion must have reduced-motion and pause controls.
- Text must remain readable at common scaling values.
- Timed demonstrations must allow slower/manual stepping.
- No unavoidable camera shake, screen flash, or haptic effect belongs in the pool Laboratory.

### 14.4 Visual customization

All project visuals, prefab meshes/sprites, labels, fonts, and dashboard styling are sample/project-owned and replaceable without editing runtime code.


## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| Root initialization state | API/Inspector | Development + optional release | Constant |
| Pool counts | Snapshot API | Development + optional release | O(number of queried pools) |
| Spawn/return failure | Structured result/log | All builds | Event-only |
| Bounded event history | API/Editor monitor | Development by default | Configurable bounded memory |
| Leak/lost-instance summary | Shutdown/scope report | Development + optional release | Scope-close cost |
| Configuration validation | Editor report | Editor | On demand |
| Full object references/hierarchy details | Editor-only | Editor | Development only |

### 15.2 Structured status

EchoPool exposes:

- package/version and root identity;
- initialization mode and configuration source;
- registered definition and runtime-pool counts;
- active, inactive, overflow, created, retained-destroyed, externally destroyed, rejected, stale, and double-return counts;
- scope ownership and scene identity using safe names/handles;
- prewarm progress and cancellation state;
- recent bounded failures and exhaustion events;
- shutdown and scope-close reports.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| EPOOL-001 | Blocker | Duplicate root rejected | Remove/repair duplicate setup |
| EPOOL-002 | Blocker | Missing configuration/catalog | Assign or create project assets |
| EPOOL-003 | Blocker | Empty or duplicate PoolId | Repair ID before runtime |
| EPOOL-004 | Blocker | Missing/invalid prefab | Assign project prefab |
| EPOOL-005 | Error | Capacity/policy relationship invalid | Correct definition values |
| EPOOL-010 | Warning/Error | Pool not registered | Register definition or correct ID |
| EPOOL-011 | Advisory/Warning | Pool exhausted | Raise capacity, change usage, or accept rejection |
| EPOOL-012 | Error | Invalid destination scene/parent | Use a valid loaded destination |
| EPOOL-013 | Error | Lifecycle callback failed | Fix project callback; inspect inner report |
| EPOOL-020 | Warning | Foreign handle | Return through originating authority |
| EPOOL-021 | Warning | Stale generation | Cancel delayed prior-owner operation |
| EPOOL-022 | Warning | Double return | Fix caller lifecycle |
| EPOOL-023 | Warning | Instance destroyed outside pool | Replace `Destroy` with return or accept loss policy |
| EPOOL-024 | Info/Warning | Scene unload reconciled lost instances | Review scene scope/pre-unload integration |
| EPOOL-025 | Warning/Error | Scope close blocked or forced | Resolve active leases/choose policy |
| EPOOL-030 | Error | Automatic-return clock/schedule failed | Use manual return and fix clock |
| EPOOL-031 | Warning | Shutdown with active leases | Review ownership cleanup |
| EPOOL-040 | Advisory | Temporary overflow used | Validate capacity under stress |
| EPOOL-041 | Advisory | Returned object destroyed above retention cap | Expected if configured; review memory policy |

### 15.4 Observatory bridge

A separate bridge may implement an Observatory provider exposing:

- pool inventory and health;
- active/inactive/overflow graphs;
- exhaustion/rejection rates;
- lost/double/stale counts;
- scope/scene summaries;
- prewarm duration and counts;
- redacted recent events.

EchoPool never depends on The Observatory.

### 15.5 Logging policy

- No per-frame or per-object spam in normal mode.
- Repeated exhaustion and stale operations are rate-limited/aggregated.
- Logs include package code, PoolId, scope, and safe counts.
- Release logs omit hierarchy paths and object names unless explicitly enabled.
- Object references are never serialized into portable support reports.
- Development verbosity is configurable separately from release diagnostics.


## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Pool definitions/configuration | Project asset | Project/EchoPool authoring | Yes as Unity assets | Unity asset database/source control |
| Active/inactive instance records | Session runtime | EchoPool | No | Memory only |
| PoolHandle/generation | Active lease | EchoPool/caller | No | Memory only |
| Statistics/history | Diagnostic session | EchoPool | No by default | Optional redacted export |
| Gameplay meaning of spawned objects | Game system | Project/owning package | As that system decides | Chronicle/project backend |

### 16.2 Standalone behavior

EchoPool requires neither The Chronicle nor The Accord. It initializes from project assets and keeps only transient runtime state.

### 16.3 Optional participant/provider contract

No EchoSave participant ships in the MVP. Systems that save semantic state do so through their own participants. On load they request the required gameplay objects again through their own spawn/factory logic, which may choose EchoPool as an optimization.

Examples:

- an objective saves that three hazards are active, not three `PoolHandle` values;
- a projectile is normally not saved at all;
- a world system saves a destroyed/placed object record, then reconstructs it through its authority;
- a network adapter restores provider identities through network authority, not an EchoPool generation.

### 16.4 Failure and recovery

Removing EchoPool must not delete project prefabs, definitions, or saved gameplay data. If a project removes the package, project systems must fall back to direct instantiation or another explicit factory before compilation can succeed. A migration guide documents replacement of API calls; there is no hidden save-file migration because pool runtime state is never durable.


## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Optional connections are explicit, removable, versioned, and separately tested. Installing a peer package does not silently change pooling behavior. The core exposes `IEchoPoolService`, statistics snapshots, scope APIs, and lifecycle events; bridges translate those contracts.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| First Light | Separate two-package bridge | EchoPool/bridge repo decision | Launch step -> Pool | Initialize, validate, prewarm, readiness report | No |
| The Passage | Separate two-package bridge | EchoPool/Passage bridge | Transition lifecycle <-> Pool | Pre-unload scope report, flush/close request | No |
| The Observatory | Separate two-package bridge | Diagnostics bridge | Pool -> Observatory | Health/status snapshots | No |
| The Workshop | Package-owned Editor setup facade | EchoPool Editor | Workshop -> EchoPool Editor | Plan/apply setup operations | No runtime dependency |
| The Foundry | Editor bridge or validator provider | Future integration | Pool validation -> Build preflight | Catalog/prefab/config readiness | No |
| Impact | Provider/project adapter | Provider owner | Impact provider -> Pool | Borrow/return VFX objects | No |
| EchoCharacters/AI/Combat/Abilities/World | Project adapter | Project/gameplay owner | Gameplay -> Pool | Spawn request and handle | No |
| EchoMultiplayer | Provider-specific adapter | Network adapter owner | Network authority <-> Pool | Provider-approved object reuse | No |

### 17.3 Bridge placement decision

- First Light, Passage, and Observatory integrations are separate two-package bridges because each references two optional authorities.
- Workshop setup remains in EchoPool's Editor assembly through ADR-001.
- Impact VFX and gameplay factories are project/provider adapters unless a repeated neutral contract justifies a bridge.
- Network pooling is always provider-specific and separate.
- Physics/particle reset utilities may live in optional feature assemblies within the EchoPool package only if they do not add undeclared project or peer dependencies.

### 17.4 Integration failure behavior

- Missing peer: bridge package is not installed; core remains unchanged.
- Version mismatch: bridge refuses registration and reports compatibility failure.
- Peer initializes later: bridge registers when both authorities are ready through explicit lifecycle hooks.
- Peer shuts down first: bridge unregisters and cancels only its own subscriptions/operations.
- Bridge removed: each core retains standalone behavior.
- Passage bridge failure: transition system decides whether cleanup warning blocks travel; EchoPool does not load scenes.
- Network adapter failure: provider authority owns despawn/recovery; EchoPool never guesses.


## 18. Performance and Resource Policy

### 18.1 Performance targets

Targets are architectural goals until measured under SFGSS-004:

| Metric | Target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Warm spawn/return allocations | Zero managed allocations on validated hot path after warmup | Wellspring stress Laboratory + Profiler | Evidence pending |
| Acquire/return complexity | O(1) average inactive storage operation | Unit/performance fixture | Evidence pending |
| Per-frame baseline | No scan of every pooled instance in normal operation | Profiler/Laboratory | Evidence pending |
| Timed-return scheduling | Bounded by active scheduled returns, no per-pool global search | Stress fixture | Evidence pending |
| Diagnostic history | Fixed configured maximum | Runtime monitor | Must never grow unbounded |
| Prewarm frame budget | Configurable incremental batch | Prewarm fixture | Evidence pending |
| Scene reconciliation | Proportional to affected scope, not all project objects | Multi-scene fixture | Evidence pending |

### 18.2 Allocation policy

- Cache callback receivers at instance creation.
- Avoid LINQ, reflection, string formatting, and hierarchy scans on spawn/return hot paths.
- Reuse result/event buffers where safe without exposing mutable shared collections.
- Keep debug stack traces and hierarchy paths Editor/development-only.
- Pool storage implementation remains internal. Unity's `ObjectPool<T>` or a custom stack/list may be used only after behavior and allocation tests prove compliance.
- The package itself does not recursively pool its internal small value objects unless evidence justifies it.

### 18.3 Scene and domain reload behavior

- Unsubscribe from scene events and clocks during shutdown/disable.
- Reset static convenience references under domain reload and Enter Play Mode configurations.
- Reject duplicate roots after scene/domain transitions.
- Rebuild runtime registries from project definitions after a true domain reload.
- Do not depend on static runtime state surviving reload.
- Tests cover supported Enter Play Mode option combinations before compatibility claims are made.

### 18.4 Scalability limits

Every configuration advertises and validates:

- maximum definitions registered;
- maximum runtime pools/scopes;
- per-pool maximum active, retained, and overflow counts;
- maximum scheduled automatic returns;
- maximum diagnostic history length;
- maximum incremental prewarm batch.

Defaults remain conservative. “Unlimited” is not an approved stable option. Tested limits are published only after measurements.


## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

EchoPool handles Unity object references, project prefab identities, scene ownership, counts, and diagnostic timings. It does not handle credentials, analytics identities, account data, chat, network payloads, or personal information.

Portable diagnostic exports exclude:

- absolute filesystem paths;
- full hierarchy paths by default;
- arbitrary component field values;
- user-entered text;
- screenshots;
- network/player identifiers.

### 19.2 Trust boundaries

- Project prefabs and callback implementations are untrusted extension code and may throw.
- Callback failures are isolated and do not corrupt pool state.
- Runtime requests validate PoolId, handle origin/generation, scene status, scope ownership, and capacity.
- Editor migration tools never load or execute arbitrary external code beyond normal Unity asset inspection.
- Provider/bridge inputs are validated at their boundary.
- Network requests are never trusted by the core; the network authority validates them first.

### 19.3 Platform behavior

| Platform | Supported? | Special behavior | Validation required |
|---|---:|---|---|
| Windows | Planned | Standard Unity object lifecycle | Clean build/Laboratory |
| macOS | Planned | Standard Unity object lifecycle | Clean build/Laboratory |
| Linux | Planned | Standard Unity object lifecycle | Clean build/Laboratory |
| WebGL | Planned/conditional | Main-thread only; memory pressure and timing require evidence | WebGL build/stress |
| Mobile | Planned/conditional | Conservative capacities; application pause/focus and memory pressure | Device tests |
| Console | Unknown/planned | Platform certification and memory budgets unavailable pre-implementation | Provider/platform evidence |
| Dedicated server | Conditional | Headless project may still reuse GameObjects; presentation samples excluded | Server build evidence |

No platform becomes “Supported” until SFGSS-004 evidence exists.


## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-pool/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   │   ├── Installation.md
│   │   ├── Quick Start.md
│   │   ├── Pool Definitions.md
│   │   ├── Spawning and Returning.md
│   │   ├── Scopes and Scenes.md
│   │   ├── Diagnostics.md
│   │   └── Troubleshooting.md
│   └── Developer/
│       ├── Architecture.md
│       ├── Lifecycle Contract.md
│       ├── API Reference.md
│       ├── Extension Points.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Runtime/
│   ├── Core/
│   ├── Configuration/
│   ├── Data/
│   ├── Lifecycle/
│   ├── Scopes/
│   ├── Diagnostics/
│   ├── Components/
│   └── EchoDevGames.EchoPool.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Inspectors/
│   ├── Monitoring/
│   └── EchoDevGames.EchoPool.Editor.asmdef
├── Samples~/
│   └── Wellspring Object Reuse Laboratory/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoPoolRoot.cs
│   ├── IEchoPoolService.cs
│   ├── PoolRegistry.cs
│   ├── PoolRuntime.cs
│   └── PoolInitializationState.cs
├── Configuration/
│   ├── EchoPoolConfiguration.cs
│   ├── PoolCatalog.cs
│   └── PoolDefinition.cs
├── Data/
│   ├── PoolId.cs
│   ├── PoolHandle.cs
│   ├── PoolSpawnRequest.cs
│   ├── PoolSpawnResult.cs
│   ├── PoolReturnRequest.cs
│   ├── PoolReturnResult.cs
│   ├── PoolScopeHandle.cs
│   └── PoolStatisticsSnapshot.cs
├── Lifecycle/
│   ├── IPoolable.cs
│   ├── PoolCreationContext.cs
│   ├── PoolSpawnContext.cs
│   ├── PoolReturnContext.cs
│   ├── InstanceRecord.cs
│   └── PoolCompletionRelay.cs
├── Scopes/
│   ├── PoolScopeKind.cs
│   ├── PoolScopeRecord.cs
│   ├── ScenePoolScope.cs
│   └── OwnerPoolScope.cs
├── Diagnostics/
│   ├── PoolDiagnosticCode.cs
│   ├── PoolDiagnosticEvent.cs
│   └── PoolShutdownReport.cs
└── Components/
    ├── PooledInstanceMarker.cs
    └── EchoPoolDirectSceneInitializer.cs
```

Exact file decomposition is finalized in implementation checkpoint plans without changing the public contract silently.

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoPool.Runtime` | Runtime | Unity Engine modules required by core only | Yes | Neutral pool runtime |
| `EchoDevGames.EchoPool.Editor` | Editor | Runtime + UnityEditor | No | Setup, validation, inspectors, monitor, facade |
| `EchoDevGames.EchoPool.Tests.Editor` | Editor tests | Runtime + Editor + Test Framework | No | EditMode tests |
| `EchoDevGames.EchoPool.Tests.Runtime` | PlayMode tests | Runtime + Test Framework | No | Runtime/lifecycle tests |
| Sample assembly if needed | Sample only | Runtime + declared sample UI deps | No | Laboratory controls |

Optional physics/particle modules receive separate assemblies and dependency declarations if approved later.

### 20.4 Repository files

- Concise README routing users to `Documentation~`.
- Visible link to `Current Notes.md`.
- Package specification link/reference.
- Changelog, license, and third-party notices.
- Contribution/development notes if collaboration is allowed.
- Release checklist and evidence index.
- Stable `.meta` files and GUIDs for public scripts/assets.
- No generated project content committed inside immutable package source except intentional samples.


## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | 6000.0 planned | 6000.3.8f1 development baseline; runtime evidence Not run | Exact supported patch list follows testing |
| Unity Test Framework | Implementation-selected concrete version | Not run | Test-only dependency |
| Optional bridges/providers | Per integration specification | Not run | Exact compatibility record required |

### 21.2 Semantic versioning policy

- **Patch:** diagnostics wording, documentation, internal optimization, bug fix with no public behavior/serialized meaning change.
- **Minor:** additive API, policy, diagnostic field, optional module, or definition field with safe defaults.
- **Major:** breaking public API, lifecycle callback order, handle semantics, stable ID rule, serialized enum meaning, capacity policy meaning, or removal behavior.
- Laboratory/sample additions follow package SemVer when they change support claims.

### 21.3 Deprecation policy

- Mark deprecated public members with documentation and compiler guidance where practical.
- Provide at least one documented migration path before removal in a major release.
- Keep released serialized enum values readable through migration.
- Do not reuse removed diagnostic, capability, test, or PoolId meanings.
- Update package spec, changelog, migration guide, tests, and ADR when architecture changes.

### 21.4 GUID and asset compatibility

Public scripts, configuration templates, samples, prefabs, and definitions preserve committed `.meta` identities when their conceptual identity survives. Moving or renaming an asset does not justify a new GUID. Project-owned generated assets are never overwritten by package updates.


## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview and authority boundaries.
- Installation routes and five-minute quick start.
- Definition/catalog authoring.
- Capacity, growth, exhaustion, retention, and overflow explanation.
- Spawn/return examples using handles and components.
- Lifecycle callback order and reset responsibilities.
- Application, scene, and owner scopes.
- Automatic return and completion relay.
- Laboratory guide.
- Diagnostics and code reference.
- Troubleshooting stale handles, double returns, external destruction, and scene unload.
- Optional integration index.
- Migration, removal, and known limitations.
- License, credits, and notices.

### 22.2 Required developer documentation

- Root/registry/runtime topology.
- Generational handle state machine.
- Spawn and return sequence diagrams.
- Callback reentrancy and exception rules.
- Scope/scene ownership model.
- Performance and allocation policy.
- Test strategy and evidence registry.
- Extension seams for clocks, factories, providers, bridges, and optional modules.
- Release workflow, ADRs, checkpoints, and Current Notes.

### 22.3 Documentation truth rule

Examples must compile against the documented release once implementation exists. Until then, signatures shown in the specification are contract proposals approved for implementation, not claimed code. Screenshots, measured results, and support statements remain absent or marked pending until evidence exists.

### 22.4 Living repository and Obsidian workflow

All package-development notes use the repository Markdown files directly. Current Notes captures provisional findings; durable decisions move into this specification, an ADR, bridge spec, issue/test record, guide, or changelog at checkpoint closeout.

### 22.5 Repository scan and handoff order

1. Repository README/index.
2. SFGSS-000.
3. SFGSS-002 through SFGSS-005 as applicable.
4. This EchoPool specification.
5. Relevant ADRs and bridge specifications.
6. Current Notes.
7. Current checkpoint, tests, issue log, and changelog.
8. Relevant implementation once it exists.


## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, definitions, capacity validation, handle equality, policies | Duplicate IDs, invalid capacity, generation comparisons | Yes |
| PlayMode unit/integration | Root, registry, lifecycle, callbacks, scopes, schedules | Spawn/return, stale/double, scene unload, shutdown | Yes |
| Standalone Laboratory | User-visible isolated workflow | Burst, exhaustion, overflow, reset, direct scene | Yes |
| Bridge Integration Laboratory | Optional package connection | Passage pre-unload, Observatory panel | When bridge ships |
| Showcase | Combined presentation | Game-jam VFX/projectile examples | No |
| Clean-project install | Packaging and independence | Git/local/tarball/sample removal | Yes |
| Existing-project migration | Adoption without regressions | Replace local projectile/VFX pool | Before integration claim |
| Performance | Allocations, frame cost, capacity behavior | Warm hot path and stress bursts | Yes for release claim |

### 23.2 Required test categories

- Happy-path prewarm, spawn, return, reuse.
- Missing/invalid configuration and prefab.
- Empty/duplicate PoolId.
- Duplicate authority.
- Fixed and growing capacities.
- Reject and temporary-overflow exhaustion.
- Maximum-retained destruction.
- Generational stale handle, foreign handle, and double return.
- Callback order, exception, reentrancy, and reset behavior.
- Timed scaled/unscaled and completion-signal return.
- External destruction active/inactive.
- Application, scene, and owner scopes.
- Scene unload and direct-scene entry.
- Shutdown, application quit, domain reload, and Enter Play Mode options.
- Sample removal and optional bridge absence/presence.
- Setup repeatability, repair, removal, and reinstall.
- Performance, allocations, memory limits, and diagnostic bounds.
- Platform/build validation.

### 23.3 Test case registry

All test cases below are planned and `Not run` until implementation evidence exists.


| Test ID | Requirement | Setup | Action | Expected result | Automation | Status |
|---|---|---|---|---|---:|---|
| EPOOL-T-CFG-001 | Configuration and ID validation | Package-specific fixture | Valid configuration initializes | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-002 | Configuration and ID validation | Package-specific fixture | Missing configuration blocks safely | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-003 | Configuration and ID validation | Package-specific fixture | Empty catalog is allowed with advisory | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-004 | Configuration and ID validation | Package-specific fixture | Empty poolid blocks registration | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-005 | Configuration and ID validation | Package-specific fixture | Duplicate poolid blocks registration | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-006 | Configuration and ID validation | Package-specific fixture | Missing prefab blocks pool | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-007 | Configuration and ID validation | Package-specific fixture | Scene instance reference rejected | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-008 | Configuration and ID validation | Package-specific fixture | Negative initial capacity rejected | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-009 | Configuration and ID validation | Package-specific fixture | Initial exceeds maximum active rejected | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-010 | Configuration and ID validation | Package-specific fixture | Retained exceeds allowed relation reported | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-011 | Configuration and ID validation | Package-specific fixture | Overflow cap zero with overflow policy reported | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-CFG-012 | Configuration and ID validation | Package-specific fixture | Definition asset remains immutable in play mode | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-013 | Authority and lifecycle | Package-specific fixture | First root claims authority | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-014 | Authority and lifecycle | Package-specific fixture | Duplicate root rejected before prewarm | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-015 | Authority and lifecycle | Package-specific fixture | Explicit initialization is idempotent | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-016 | Authority and lifecycle | Package-specific fixture | Disable/enable does not duplicate subscriptions | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-017 | Authority and lifecycle | Package-specific fixture | Shutdown rejects new spawns | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-018 | Authority and lifecycle | Package-specific fixture | Shutdown is idempotent | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-019 | Authority and lifecycle | Package-specific fixture | Application quit cleans schedules | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-020 | Authority and lifecycle | Package-specific fixture | Static convenience resets on domain reload | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-021 | Authority and lifecycle | Package-specific fixture | Direct-scene initializer creates only missing root | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-ROOT-022 | Authority and lifecycle | Package-specific fixture | Canonical root adopted by direct scene helper | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-023 | Prewarm and growth | Package-specific fixture | Fixed pool prewarms exact count | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-024 | Prewarm and growth | Package-specific fixture | Incremental prewarm respects frame budget | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-025 | Prewarm and growth | Package-specific fixture | Prewarm cancellation stops future creation | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-026 | Prewarm and growth | Package-specific fixture | Cancelled prewarm retains valid created objects | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-027 | Prewarm and growth | Package-specific fixture | Grow-on-demand creates below limit | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-028 | Prewarm and growth | Package-specific fixture | Growth stops at maximum active | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-029 | Prewarm and growth | Package-specific fixture | Growth batch does not exceed hard limit | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-030 | Prewarm and growth | Package-specific fixture | Prewarm callback failure is isolated | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-031 | Prewarm and growth | Package-specific fixture | Prewarm report counts created and failed | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-PRE-032 | Prewarm and growth | Package-specific fixture | Prewarm repeated request is idempotent by requested target | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-033 | Spawn behavior | Package-specific fixture | Inactive instance is reused | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-034 | Spawn behavior | Package-specific fixture | Spawn returns valid gameobject and handle | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-035 | Spawn behavior | Package-specific fixture | Component request returns expected component | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-036 | Spawn behavior | Package-specific fixture | Wrong component type fails without escape | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-037 | Spawn behavior | Package-specific fixture | Parent and world transform applied correctly | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-038 | Spawn behavior | Package-specific fixture | Destination scene assignment succeeds | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-039 | Spawn behavior | Package-specific fixture | Unloaded destination scene fails | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-040 | Spawn behavior | Package-specific fixture | Spawn preparing callback precedes activation | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-041 | Spawn behavior | Package-specific fixture | Spawned callback follows activation | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-042 | Spawn behavior | Package-specific fixture | Spawn callback exception aborts success | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-043 | Spawn behavior | Package-specific fixture | Spawn event raised after authoritative active state | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SPAWN-044 | Spawn behavior | Package-specific fixture | Spawn request does not mutate definition | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-045 | Return and lease safety | Package-specific fixture | Valid handle returns instance | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-046 | Return and lease safety | Package-specific fixture | Generation increments on reuse | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-047 | Return and lease safety | Package-specific fixture | Stale handle cannot return reused instance | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-048 | Return and lease safety | Package-specific fixture | Double return fails safely | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-049 | Return and lease safety | Package-specific fixture | Foreign root handle fails safely | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-050 | Return and lease safety | Package-specific fixture | Foreign pool handle fails safely | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-051 | Return and lease safety | Package-specific fixture | Returning callback runs before deactivation | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-052 | Return and lease safety | Package-specific fixture | Returned callback runs after deactivation | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-053 | Return and lease safety | Package-specific fixture | Return event raised after generation closes | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-054 | Return and lease safety | Package-specific fixture | Return above retained cap destroys object | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-055 | Return and lease safety | Package-specific fixture | Overflow return destroys object | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-056 | Return and lease safety | Package-specific fixture | Return callback exception preserves coherent state | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-057 | Return and lease safety | Package-specific fixture | Reentrant same-handle return rejected | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-RETURN-058 | Return and lease safety | Package-specific fixture | Manual return cancels automatic schedule | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-059 | Exhaustion and limits | Package-specific fixture | Fixed exhausted pool rejects by default | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-060 | Exhaustion and limits | Package-specific fixture | Reject result reports capacity snapshot | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-061 | Exhaustion and limits | Package-specific fixture | Temporary overflow creates below cap | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-062 | Exhaustion and limits | Package-specific fixture | Temporary overflow stops at cap | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-063 | Exhaustion and limits | Package-specific fixture | Overflow never enters inactive retained storage | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-064 | Exhaustion and limits | Package-specific fixture | Maximum retained bounds idle count | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-065 | Exhaustion and limits | Package-specific fixture | Exhaustion diagnostics rate limit | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-066 | Exhaustion and limits | Package-specific fixture | Active count never exceeds retained plus overflow caps | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-067 | Exhaustion and limits | Package-specific fixture | Unlimited sentinel is rejected | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-EXH-068 | Exhaustion and limits | Package-specific fixture | Reclaim-oldest policy unavailable in mvp | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-069 | Automatic return | Package-specific fixture | Scaled timer returns after scaled duration | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-070 | Automatic return | Package-specific fixture | Scaled timer pauses at zero scale | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-071 | Automatic return | Package-specific fixture | Unscaled timer continues at zero scale | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-072 | Automatic return | Package-specific fixture | Negative duration rejected | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-073 | Automatic return | Package-specific fixture | Zero duration follows documented next-safe-point policy | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-074 | Automatic return | Package-specific fixture | Completion relay returns current lease | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-075 | Automatic return | Package-specific fixture | Completion relay signals exactly once | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-076 | Automatic return | Package-specific fixture | Old relay signal ignored after reuse | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-077 | Automatic return | Package-specific fixture | Schedule cancelled on external destruction | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-AUTO-078 | Automatic return | Package-specific fixture | Schedule cancelled on shutdown | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-079 | Scopes and scenes | Package-specific fixture | Application pool survives scene load | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-080 | Scopes and scenes | Package-specific fixture | Scene pool objects assigned to target scene | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-081 | Scopes and scenes | Package-specific fixture | Scene unload closes scene pool | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-082 | Scopes and scenes | Package-specific fixture | Scene unload repairs active lost records | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-083 | Scopes and scenes | Package-specific fixture | Scene unload repairs inactive lost records | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-084 | Scopes and scenes | Package-specific fixture | Owner scope closes empty | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-085 | Scopes and scenes | Package-specific fixture | Owner scope reject policy blocks with active leases | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-086 | Scopes and scenes | Package-specific fixture | Owner scope return policy returns active leases | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-087 | Scopes and scenes | Package-specific fixture | Owner scope destroy policy reports destruction | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-088 | Scopes and scenes | Package-specific fixture | Foreign scope handle rejected | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-089 | Scopes and scenes | Package-specific fixture | Scope close is idempotent | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-SCOPE-090 | Scopes and scenes | Package-specific fixture | Spawn into closed scope rejected | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-LOSS-091 | External destruction and recovery | Package-specific fixture | External active destroy closes generation | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-LOSS-092 | External destruction and recovery | Package-specific fixture | External inactive destroy repairs count | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-LOSS-093 | External destruction and recovery | Package-specific fixture | Externally destroyed object never returned as success | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-LOSS-094 | External destruction and recovery | Package-specific fixture | Lost instance increments statistic | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-LOSS-095 | External destruction and recovery | Package-specific fixture | Lost instance does not instantiate inside ondestroy | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-LOSS-096 | External destruction and recovery | Package-specific fixture | Next growth may replenish below limit | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-LOSS-097 | External destruction and recovery | Package-specific fixture | Destroy initiated by pool not reported as external | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-LOSS-098 | External destruction and recovery | Package-specific fixture | Destroy during shutdown classified correctly | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-DIAG-099 | Diagnostics and bounds | Package-specific fixture | Statistics snapshot counts active/inactive | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-DIAG-100 | Diagnostics and bounds | Package-specific fixture | Snapshot is detached/read-only | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-DIAG-101 | Diagnostics and bounds | Package-specific fixture | History respects maximum length | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-DIAG-102 | Diagnostics and bounds | Package-specific fixture | Diagnostics reset preserves pool contents | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-DIAG-103 | Diagnostics and bounds | Package-specific fixture | Support export redacts object references | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-DIAG-104 | Diagnostics and bounds | Package-specific fixture | Repeated stale warnings aggregate | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-DIAG-105 | Diagnostics and bounds | Package-specific fixture | Scope report identifies safe scene identity | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-DIAG-106 | Diagnostics and bounds | Package-specific fixture | Shutdown report lists active leases without hierarchy data | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-107 | Installation, tooling, and removal | Package-specific fixture | Embedded install compiles | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-108 | Installation, tooling, and removal | Package-specific fixture | Local-path install compiles | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-109 | Installation, tooling, and removal | Package-specific fixture | Git install compiles | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-110 | Installation, tooling, and removal | Package-specific fixture | Tarball install compiles | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-111 | Installation, tooling, and removal | Package-specific fixture | Sample imports separately | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-112 | Installation, tooling, and removal | Package-specific fixture | Sample removal leaves core compiling | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-113 | Installation, tooling, and removal | Package-specific fixture | Setup repeated creates no duplicate assets | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-114 | Installation, tooling, and removal | Package-specific fixture | Repair preview reports exact changes | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-115 | Installation, tooling, and removal | Package-specific fixture | Remove bridge before core succeeds | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-116 | Installation, tooling, and removal | Package-specific fixture | Remove echopool after project adapter removal succeeds | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-117 | Installation, tooling, and removal | Package-specific fixture | Reinstall preserves project definitions | Expected approved behavior and no state corruption | Planned | Not run |
| EPOOL-T-INSTALL-118 | Installation, tooling, and removal | Package-specific fixture | No runtime assembly references unityeditor | Expected approved behavior and no state corruption | Planned | Not run |


### 23.4 Evidence requirements

Each execution record must include package version, Unity version, platform, installation route, test runner/manual operator, timestamp, commit/tag, result, evidence location, and linked issue when failed. Retry history and flaky behavior remain visible. Passing the definition registry does not imply passing the Laboratory or distribution gate.


## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and non-ownership approved.
- [x] MVP and deferred scope separated.
- [x] Dependencies and bridge direction explicit.
- [x] Definition/runtime identity model approved.
- [x] Generational handle and callback order approved.
- [x] Capacity, exhaustion, retention, scope, and scene policy approved.
- [x] Standalone Laboratory designed.
- [x] Release-blocking design questions resolved.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared dependencies only.
- [ ] Editor code is isolated.
- [ ] Root claims before side effects.
- [ ] Public API matches this specification or authority is revised first.
- [ ] Definitions remain immutable in Play Mode.
- [ ] Setup/repair is repeatable and non-destructive.
- [ ] Generational, callback, capacity, scene, and shutdown behavior passes tests.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Core works without unrelated Sperk's Forge packages.
- [ ] Wellspring Laboratory passes.
- [ ] Samples can be removed safely.
- [ ] Direct-scene entry behaves as documented.
- [ ] Optional bridges/providers can be absent.

### 24.4 Quality gate

- [ ] Automated tests pass with evidence.
- [ ] Manual Laboratory checklist passes.
- [ ] No blocker/critical defect remains.
- [ ] Allocation/performance targets are measured and documented.
- [ ] Diagnostic bounds and redaction pass.
- [ ] Documentation matches implementation.
- [ ] Current Notes reconciled.
- [ ] Licenses/notices complete.

### 24.5 Distribution gate

- [ ] Manifest valid with concrete dependencies.
- [ ] Version/changelog updated.
- [ ] Stable `.meta` files included.
- [ ] Git/local/tarball routes tested as claimed.
- [ ] Removal/reinstall tested.
- [ ] Repository tag/release prepared.
- [ ] Compatibility catalog updated.
- [ ] Beta, release-candidate, and stable evidence satisfy SFGSS-004 separately.


## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Echo Systems Lab | Projectile/VFX instantiation or local pool patterns | Introduce one non-authoritative pool for a repeated effect/projectile | Behavior identical, reset correct, diagnostics clean | Restore original instantiate/destroy path |
| Rescuers2D | Explosions, debris, temporary interaction effects | Migrate one prefab category at a time | Scene unload/direct-scene tests and visual parity | Keep old factory/prefab path |
| Don’t Get Vince’d | Hit effects, pickups, projectiles | Replace one transient category after Laboratory proof | Combo/boss stress does not leak state | Feature flag/original path |
| Future Impact provider | Feedback VFX reuse | Project/provider adapter calls `IEchoPoolService` | Impact and pool Integration Lab passes | Provider instantiates directly |

### 25.2 Preserve-until-parity rule

Existing working systems remain intact. EchoPool is installed and validated alone first. One prefab category is migrated at a time. Gameplay factories keep their public behavior while replacing only allocation/reuse internals. The old path is removed only after parity, scene, stress, and rollback tests pass.

### 25.3 Migration tooling

Planned tools may:

- detect common local pool prefabs/components only through explicit user selection, not project-wide destructive scans;
- preview candidate definitions and stable IDs;
- create project-owned definitions/catalog entries;
- generate adapter scaffolding only when requested;
- preserve original scripts/prefabs;
- produce a migration checklist and rollback receipt;
- never rewrite arbitrary gameplay code automatically in the MVP.


## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EPOOL-R-001 | Scope expands into spawn manager | Medium | High | Enforce authority boundary and project adapters | Specification review |
| EPOOL-R-002 | Mutable state leaks between uses | High | High | Exact callbacks, reset guide, Laboratory fixtures | Runtime/testing |
| EPOOL-R-003 | Stale coroutine returns reused object | High | High | Generational handles and schedule binding | Core runtime |
| EPOOL-R-004 | Unlimited growth hides memory leak | Medium | High | Hard limits, no unlimited stable option, diagnostics | Definition validation |
| EPOOL-R-005 | Double return corrupts inactive storage | Medium | High | Record state machine and structured rejection | Core runtime |
| EPOOL-R-006 | External Destroy corrupts counts | High | Medium | Internal marker and reconciliation | Core runtime |
| EPOOL-R-007 | Scene unload leaves persistent references | Medium | High | Scope ownership, scene reconciliation, Passage bridge | Lifecycle tests |
| EPOOL-R-008 | Reclaim policy removes live gameplay | Medium | High | Defer force-reclaim from MVP | Architecture owner |
| EPOOL-R-009 | Callback throws and leaves half-spawned object | Medium | High | Guarded sequence and abort cleanup | Core runtime |
| EPOOL-R-010 | Pooling worsens performance for low-use objects | Medium | Medium | Evidence-based adoption and capacity guidance | Performance tests |
| EPOOL-R-011 | Optional physics modules create hidden dependencies | Medium | Medium | Separate assemblies/modules and SFGSS-002 validation | Integration review |
| EPOOL-R-012 | Package update breaks serialized definitions | Low | High | SemVer, GUID preservation, enum append-only, migration | Release owner |
| EPOOL-R-013 | Laboratory UI becomes runtime requirement | Low | Medium | Sample-only assembly and removal test | Packaging tests |
| EPOOL-R-014 | Network provider bypasses authority/security | Medium | High | Provider-specific adapter; no core network semantics | Advanced review |
| EPOOL-R-015 | Diagnostic history becomes another leak | Low | Medium | Fixed bounds and sampling controls | Diagnostics tests |


## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EPOOL-D-001 | EchoPool owns reuse lifecycle, not spawn intent | Approved | Preserves neighboring gameplay authorities | Callers remain responsible for semantic factories | No |
| EPOOL-D-002 | Default runtime uses one duplicate-safe application authority | Approved | Consistent registry, handles, scopes, and diagnostics | Scene pools remain children of the authority | No |
| EPOOL-D-003 | Public returns use generational handles | Approved | Prevents stale prior-use operations | Handles are session-only and validated |
| EPOOL-D-004 | Definitions are immutable project-owned ScriptableObjects | Approved | Safe authoring and reuse | Runtime counts live elsewhere |
| EPOOL-D-005 | Main-thread Unity object operations only | Approved | Unity object APIs are not general background-thread APIs | Prewarm async distributes work across frames, not worker threads |
| EPOOL-D-006 | Exhaustion defaults to Reject | Approved | Safest predictable behavior | Callers must handle failure |
| EPOOL-D-007 | Temporary overflow is bounded and never retained | Approved | Allows explicit resilience without hidden permanent growth | Overflow is diagnosed and destroyed on return |
| EPOOL-D-008 | Force-reclaim active instances is not MVP | Approved | It changes gameplay ownership | Deferred/experimental only |
| EPOOL-D-009 | Core performs only generic parent/transform/active reset | Approved | Arbitrary state is project semantic | IPoolable/optional modules reset content |
| EPOOL-D-010 | Scene unload reconciliation works standalone; Passage adds pre-unload coordination | Approved | Preserves package independence | Post-unload cleanup cannot promise pre-unload callbacks |
| EPOOL-D-011 | Active handles and pool runtime state are never saved | Approved | Handles are ephemeral generations | Save semantic game state elsewhere |
| EPOOL-D-012 | Internal storage algorithm is not public API | Approved | Allows evidence-based implementation choice | Unity ObjectPool or custom storage may be tested internally |
| EPOOL-D-013 | Jukebot voice pooling remains outside EchoPool | Approved | Audio owns voice lifecycle | No audio bridge for internal voices |
| EPOOL-D-014 | Network pooling requires provider-specific adapter | Approved | Network identity/authority differs by provider | No core network package dependency |

### 27.2 Release-blocking questions

None remain for specification approval. Internal data-structure choice, exact concrete Unity package versions, and measured default capacities are implementation evidence questions governed by approved behavior and SFGSS-004.

### 27.3 Non-blocking later questions

- Which optional physics/particle reset modules are common enough to ship?
- Should incremental prewarm expose time-budget milliseconds, count-per-frame, or both after profiling?
- Does a later Addressables provider belong in the EchoPool repository or a separate provider repository?
- Are memory-pressure trim policies portable enough for a stable core expansion?
- Does any real project justify experimental active reclaim with explicit cancellation?


## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved package authority | Design only | This approved document |
| M1 - Package skeleton | Installable UPM structure | Manifest, asmdefs, docs shell | Clean compile/removal |
| M2 - Definitions and root | Configuration, catalog, definition validation, duplicate-safe root | EPOOL-CAP-001 to 003 | EditMode/PlayMode tests |
| M3 - Core leases | Prewarm, spawn, return, generational handles, callbacks | EPOOL-CAP-004 to 008 | Runtime tests |
| M4 - Capacity and scopes | Growth, exhaustion, overflow, retention, application/scene/owner scope | EPOOL-CAP-009 to 016 | Stress and scene tests |
| M5 - Automatic return/reconciliation | Timers, completion relay, external destruction, scene cleanup | EPOOL-CAP-017 to 020 | Lifecycle tests |
| M6 - Tooling and Laboratory | Setup, validation, monitor, isolated sample | EPOOL-CAP-021 to 024 | Laboratory and repeat-run evidence |
| M7 - First integration | One explicit bridge/project adoption | Selected later capability | Integration Lab/parity report |
| M8 - Beta/release | Distribution-ready package | Docs, tests, licenses, package | SFGSS-004 gates |

### 28.2 Checkpoint rule

Every milestone is split into SFGSS-005 Checkpoint Build Plans. A checkpoint authorizes only named files, behavior, Editor work, tests, and stop point. When coding eventually begins, complete compile-ready files are shown in the conversation with architecture and line-by-line concept explanations so Jesse can implement them himself.

### 28.3 First recommended checkpoint

After SUITE-DOC-33 unlocks implementation:

> **EPOOL-M1-01 - EchoPool Package Skeleton**

Create only the package manifest, Runtime/Editor/Test asmdefs, documentation shell, and installation/removal test plan. Stop before runtime C# behavior.


## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk’s Forge - EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the suite authority. Treat SFGSS-002 as the dependency/assembly authority, SFGSS-003 as the data/identity/migration authority, SFGSS-004 as the testing/evidence authority, SFGSS-005 as the checkpoint workflow authority, and this approved EchoPool specification as the Level 2 authority for The Wellspring.

Current package: EchoPool - The Wellspring
Specification version: 1.0.0 Approved
Current documentation checkpoint: SUITE-DOC-07 - EchoProgression (`The Ascent`) Package Specification
Implementation status: Locked until SUITE-DOC-33
Known blockers: None

Before changing EchoPool:
1. Summarize its ownership boundary: reuse lifecycle, not spawn intent.
2. Preserve generational handles, bounded capacity, safe exhaustion, callback order, and scope behavior.
3. Keep project reset rules in IPoolable or explicit adapters.
4. Keep optional peers behind bridges/providers.
5. Mark all unexecuted evidence Not run.
6. Do not create package code until the final documentation gate passes.
7. When implementation begins, show complete code and explain each file and step so Jesse can enter and understand it himself.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package specification | 1.0.0 Approved |
| Completed checkpoint | SUITE-DOC-06 - EchoPool Package Specification |
| Files/assets created | Documentation only |
| Tests passed | None; all planned evidence Not run |
| Tests failed | None executed |
| Known issues | None blocking; implementation advisories remain evidence-pending |
| Decisions added | EPOOL-D-001 through EPOOL-D-014 |
| Next checkpoint | SUITE-DOC-07 - EchoProgression: The Ascent Package Specification |


## 30. Approval

### 30.1 Approval checklist

- [x] Package identity and responsibility are clear.
- [x] Ownership and non-ownership align with SFGSS-000.
- [x] Independence proof is credible.
- [x] MVP is bounded and useful.
- [x] Public API, data, lifecycle, callbacks, capacity, scope, and failure behavior are specified.
- [x] Setup and direct-scene workflows are understandable.
- [x] Standalone Laboratory is fully defined.
- [x] Diagnostics exist without The Observatory.
- [x] Optional integrations are separated.
- [x] Test and release gates are measurable and remain Not run.
- [x] No Isekai Studios identity or ownership was introduced.
- [x] Jesse has approved continuing the package-first documentation workflow and delegated durable design choices to the most effective long-term architecture.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams  
**Date:** August 4, 2026  
**Conditions:** Package implementation remains locked until SUITE-DOC-33. Internal storage choice and measured capacities must be selected through implementation evidence without changing the approved public behavior silently.

---

## Template Completion Check

A new collaborator can determine from this specification:

1. EchoPool owns reusable instance lifecycle, not gameplay spawn intent.
2. The package explicitly refuses enemy waves, projectile rules, audio voices, UI virtualization, networking authority, and save truth.
3. The MVP is a bounded GameObject/Component pool with prewarm, generational leases, scopes, safe exhaustion, callbacks, automatic return, diagnostics, tooling, and a Laboratory.
4. It works without any other Sperk's Forge runtime package.
5. Definitions remain immutable while runtime pools, records, generations, schedules, and statistics remain session state.
6. Public API and lifecycle ordering are explicit.
7. Configuration, callback, capacity, scene, stale-handle, and shutdown failures have structured behavior.
8. The isolated Wellspring Laboratory proves the core without unrelated package code.
9. Optional packages connect through bridges/providers and can be removed independently.
10. No release claim exists until SFGSS-004 evidence is executed and recorded.
