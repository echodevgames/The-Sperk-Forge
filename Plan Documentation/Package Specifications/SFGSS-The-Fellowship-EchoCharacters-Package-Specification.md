# The Fellowship – Character Identity and Roster Package Specification

**Working document ID:** SFGSS-PKG-ECHOCHARACTERS-001  
**Specification version:** 1.0.1
**Status:** Approved  
**Technical package name:** EchoCharacters  
**Public title:** The Fellowship – Character Identity and Roster
**Package ID:** `com.echodevgames.echo-characters`  
**Runtime namespace:** `EchoDevGames.EchoCharacters`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoCharacters`
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Last updated:** August 4, 2026

> “Gather the cast, preserve who they are, and pass the reins without losing the story.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoCharacters. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-005 and approved package authorities through The Eye | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved character identity, roster, availability, selection, grouping, spawning, switching, control ownership, snapshot, diagnostics, tooling, Laboratory, bridge, and release contracts | Jesse “Echo” Adams |
| 1.0.1 | 2026-08-04 | Approved | Normalized registry metadata and formal title; added the SUITE-DOC-30 governing-authority, evidence, test-registry, and compatibility clarification without authorizing implementation. | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Fellowship – Character Identity and Roster
**Technical identifier:** EchoCharacters  
**Flavor line:** Gather the cast, preserve who they are, and pass the reins without losing the story.  
**Plain-language subtitle:** A standalone Unity package for durable character identity, roster membership, availability, selection contexts, groups, spawning, replacement, respawn, control ownership, possession handoff, snapshots, diagnostics, authoring, validation, and explicit integration seams.

**One-sentence ownership contract:**

> EchoCharacters owns stable character definitions and durable character identities, runtime roster truth, availability and status records, selection contexts, ordered groups, spawn/despawn coordination, runtime actor registration, replacement and respawn requests, exclusive control-owner assignment, validated character switching, detached roster snapshots, diagnostics, authoring, validation, an isolated Fellowship Laboratory, and explicit bridge seams; it does not own locomotion, combat, abilities, AI, animation graphs, camera movement, input devices or bindings, inventory contents, health or statistics, dialogue, objectives, scene loading, save-file transport, network provider authority, or project-specific character behavior.

### 1.1 Elevator summary

The Fellowship provides one neutral authority for **who the characters are, which characters belong to a roster, who is selectable, which character is currently selected, which spawned actor represents each durable character, and who currently controls that actor**. It supports a single hero, a switchable rescue team, a party, local multiplayer selection contexts, fighting-game character assignment, or future network ownership without forcing one movement controller, input map, combat model, RPG schema, camera backend, or save system.

The package separates three identities that projects commonly blur together. A `CharacterDefinitionId` identifies a reusable archetype or authored character definition. A durable `CharacterId` identifies one roster member and survives saves, respawns, and prefab replacement. A session-only `CharacterRuntimeInstanceId` identifies one spawned actor. Multiple durable characters may use one definition, and one durable character may receive many runtime instances across its lifetime. No GameObject, prefab name, asset GUID, display name, or hierarchy path becomes the character's permanent identity.

Selection, spawning, and control are independent truths. A character may be selected but not spawned, spawned but not controlled, controlled without being the only roster member, or unavailable while remaining in history. Switching is a validated orchestration that may prepare a target spawn and control handoff before committing selection and ownership. The package publishes semantic events and structured snapshots so EchoControllers, The Will, The Eye, The Looking Glass, The Vault, The Hand, The Chronicle, The Ascent, and future multiplayer adapters can connect without becoming core dependencies.

### 1.2 Why this belongs in The Sperk's Forge

Rescuers2D already demonstrates the need to cycle Firefighter, Riot Officer, Rescue Specialist, and future roles while preserving shared team truth and handing input, animation, audio, camera, and role-specific behavior to the correct actor. Hackulos needs one player character, companions, pets, party members, respawn, roster persistence, and later multiplayer ownership. Fighting games and local multiplayer prototypes need independent selection contexts. Existing projects repeatedly place selection, spawning, input targets, camera targets, and controller enablement in one project-specific manager.

A reusable package is justified because identity and control ownership recur across genres while movement, combat, stats, animation, and game rules vary dramatically. The Fellowship extracts the stable authority and leaves the changing behavior in controllers, project components, and explicit bridges.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Always paired with “Character Identity and Roster.” |
| Setup guidance/tooltips | Yes | Must remain technically clear. |
| Samples | Optional | Fellowship imagery may decorate the Laboratory but is removable. |
| Runtime API/type names | No lore-only names | Types use `CharacterId`, `CharacterRoster`, `CharacterControlAssignment`, and similar technical names. |
| Project data | No required Verse content | Games own names, portraits, prefabs, tags, roles, biographies, animations, and character rules. |

---

## 2. Problem Statement

### 2.1 Current problem

Character systems often combine definition data, spawned GameObjects, selected UI state, player input, controller enablement, camera targets, party state, save data, and multiplayer ownership in one manager. A prefab name becomes an identity. Destroying and respawning a GameObject accidentally creates a “new character.” Switching characters directly enables one controller and disables another without a validated ownership transaction. Locked or injured characters disappear from the roster because availability is treated as membership. Multiple local players cannot select independently. Save code serializes scene objects or static dictionaries.

A reusable package must preserve durable identity while remaining neutral about genre behavior. It must work alone, spawn project-owned prefabs, allow custom/network providers, coordinate control handoff without owning controllers, and preserve unresolved definitions or extension records rather than deleting durable data when an optional package is absent.

### 2.2 Evidence from existing work

| Source project/system | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Shared input cycles role-specific actors | Clear active-character concept | Separate durable identity, spawn state, selection, and controller ownership |
| Hackulos | Player, Necromancer pet, companions, death/respawn, future party | Data-driven character definitions | Keep RPG stats and abilities outside neutral roster authority |
| Don't Get Vince'd | Player and enemy actors need stable identity and replacement seams | Focused actor components | Avoid importing combat or beat-'em-up rules into roster code |
| The Will | Local users/devices need a character target | Centralized input translation | Map users to `ControlOwnerId` through a bridge, not a core dependency |
| The Eye | Camera targets change with active character | Stable target handles | Update camera only after authoritative control/selection commit |
| The Vault | Character-owned containers/equipment need durable owner IDs | Stable item/container IDs | Map `CharacterId` to containers without moving inventory authority |
| The Ascent | Characters may be locked or unlocked | Progression nodes and stable IDs | Map progression state to availability through a bridge |
| The Chronicle | Roster state must survive sessions | Versioned participant snapshots | Export detached state, never live GameObjects or leases |

### 2.3 Consequences of doing nothing

- Character identity depends on prefab names, scene objects, or display labels.
- Respawn duplicates or loses save identity.
- Locked, injured, missing, or defeated characters are confused with removed members.
- Switching enables/disables controllers without rollback or stale-request protection.
- Input, camera, UI, inventory, and save systems reference one another directly.
- Local multiplayer selection requires rewriting a single global selected-character field.
- Scene unloads leave stale actor references and possession state.
- Optional package removal deletes or corrupts durable character data.
- Network ownership becomes entangled with one provider before research is complete.
- Diagnostics cannot explain who is selected, spawned, controlled, or unavailable.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one duplicate-safe application-session character authority.
- Separate definitions, durable characters, and spawned runtime instances.
- Support multiple rosters and bounded independent selection contexts.
- Keep roster membership separate from availability and narrative status.
- Support ordered party/squad/group membership without owning party combat rules.
- Coordinate spawning, despawning, replacement, and respawn through explicit providers.
- Own exclusive control assignment and stale-safe possession leases.
- Validate switch requests before selection or control changes commit.
- Publish immutable snapshots and semantic events after authoritative mutations.
- Export/import detached versioned roster state without save transport ownership.
- Remain useful with custom controllers and no other Sperk's Forge package installed.
- Provide actionable diagnostics, safe authoring tools, and an isolated Laboratory.

### 3.2 Non-goals

- Implement locomotion, jumping, climbing, swimming, vehicles, or navigation.
- Calculate damage, health, stats, classes, skills, equipment effects, or abilities.
- Author animation state machines, Animator parameters, or animation events.
- Poll input devices or own action maps.
- Move cameras or choose camera framing.
- Store inventory, quests, dialogue, or progression truth.
- Load scenes or select world destinations.
- Serialize save files or cloud records.
- Become a networking stack, server authority, or prediction system.
- Require every game to support switching, parties, multiple rosters, or multiple players.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project and several character prefabs | Create definitions, a roster, a root, and a working switchable sample through guided setup. |
| Programmer | Custom controllers and spawning rules | Use interfaces, requests, handles, snapshots, and events without editing package code. |
| Designer | Character roster and availability needs | Author characters, order, groups, status, and selection defaults through validated assets. |
| UI developer | Character-select or party screen | Read immutable roster snapshots and submit revision-aware requests. |
| Tester | Suspected switch/spawn/control defect | Reproduce the lifecycle in the Fellowship Laboratory and inspect structured diagnostics. |
| Maintainer | Package upgrade or project migration | Preserve IDs, aliases, snapshot versions, and project-owned assets. |

### 3.4 Measurable success criteria

- Installs into a clean supported Unity project with zero compile errors.
- Core runtime works with no other Sperk's Forge package installed.
- One definition may produce multiple durable characters without identity collision.
- One durable character may respawn into multiple runtime instances without losing identity.
- Selection, spawn, and control state remain independently queryable.
- Failed spawn or handoff preparation does not change authoritative selection/control truth.
- Stale runtime and control handles cannot affect newer state.
- Detached snapshots contain no GameObject, Transform, scene-object, or live lease references.
- Removing samples or optional bridges does not break core assemblies.
- The Standalone Laboratory proves the MVP with all evidence initially marked `Not run`.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers.
- Gameplay programmers building switchable characters, parties, companions, or local multiplayer.
- Designers authoring rosters and character metadata.
- UI developers building selection, party, or respawn screens.
- Testers validating lifecycle, identity, and handoff behavior.
- Future provider authors connecting controllers, input, camera, inventory, saves, or multiplayer.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| ECHR-UC-001 | Create a character catalog | Designer | Package installed | Stable validated character definitions exist | MVP |
| ECHR-UC-002 | Create a roster | Designer/programmer | Valid definitions | Ordered durable members are created | MVP |
| ECHR-UC-003 | Select a character | UI/gameplay | Available member and selection context | Selection changes atomically | MVP |
| ECHR-UC-004 | Cycle selection | Gameplay | Mixed availability roster | Next/previous selectable member is chosen | MVP |
| ECHR-UC-005 | Spawn a character | Gameplay | Valid definition and provider | Runtime actor and handle are returned | MVP |
| ECHR-UC-006 | Assign control | Gameplay | Spawned available character | Exclusive owner assignment commits | MVP |
| ECHR-UC-007 | Switch controlled character | Gameplay | Valid target and policy | Target prepares, then selection/control commits | MVP |
| ECHR-UC-008 | Despawn or respawn | Gameplay | Runtime actor exists or durable member is eligible | Runtime identity changes while CharacterId persists | MVP |
| ECHR-UC-009 | Mark availability/status | Project/bridge | Durable member exists | Selectability and status update without removing membership | MVP |
| ECHR-UC-010 | Maintain party groups | Gameplay/UI | Members exist | Ordered group snapshots update atomically | MVP |
| ECHR-UC-011 | Export/import roster state | Save bridge/project | Valid detached state | Durable roster truth round-trips | MVP |
| ECHR-UC-012 | Local player assignment | Input bridge | Registered ControlOwnerId | Player/user maps to controlled character | Later bridge |
| ECHR-UC-013 | Network ownership | Multiplayer adapter | Provider approved later | Server/provider validates assignment | Advanced bridge |

### 4.3 Explicitly unsupported use cases

- Serializing arbitrary MonoBehaviours as character state.
- Using display names or prefab names as durable IDs.
- Letting a selection screen mutate roster collections directly.
- Treating a live GameObject as the durable character record.
- Automatically granting control merely because an actor spawned.
- Network authority without an approved EchoMultiplayer/provider adapter.
- Shared multi-owner possession in the MVP.
- Saving live possession leases or scene object references.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Character definition and durable identity contracts.
- Runtime roster creation, membership, ordering, revisions, and snapshots.
- Selection contexts and selected-character truth.
- Availability dispositions, reason/status records, and standard status IDs.
- Ordered group membership.
- Spawn/despawn provider coordination and runtime actor registry.
- Replacement and respawn requests.
- Exclusive control-owner assignment and stale-safe leases.
- Switch orchestration and control-handoff participant sequencing.
- Detached roster-state export/import and package migrations.
- Standalone diagnostics, Editor authoring/validation, and Laboratory evidence definitions.

### 5.2 The package does not own

- Movement, physics, navigation, combat, AI, abilities, animation, camera, input, UI, inventory, dialogue, objectives, scene flow, save files, world state, or networking provider authority.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How The Fellowship interacts |
|---|---|---|
| Movement/controller behavior | The Vessel or project | Optional control-handoff bridge targets the selected runtime actor. |
| Input contexts/devices | The Will or project | Bridge maps local users to `ControlOwnerId` and submits semantic requests. |
| Camera view | The Eye or project | Bridge registers actor targets and updates after control commit. |
| Character selection UI | The Looking Glass or project | Presenter consumes snapshots and submits revision-aware commands. |
| Unlocks | The Ascent | Bridge maps progression nodes to availability disposition/status. |
| Inventory/equipment ownership | The Vault | Bridge maps `CharacterId` to project-owned container IDs. |
| World interactions | The Hand | Bridge associates controlled runtime actors with interactors. |
| Save transport | The Chronicle | Bridge registers detached versioned roster snapshot participant. |
| Scene/world spawn destinations | The Passage, future Atlas, or project | Project supplies explicit spawn context/point; Fellowship never loads a scene. |
| Multiplayer authority | The Convergence/provider adapter | Future bridge validates owners, spawn, and control assignment. |
| Character stats/RPG data | Project or `EchoRPG.Foundation` | Stable CharacterId is the integration key; Fellowship stores no RPG rules. |

### 5.4 Boundary tests

A proposed feature belongs in EchoCharacters only when it answers at least one of these questions: who is this durable character, which roster/group contains them, are they selectable, which runtime actor represents them, who controls them, or how does that authority survive a lifecycle transition? If it answers how they move, fight, animate, render, speak, carry items, gain XP, travel, or replicate, it belongs elsewhere or behind a bridge.

---

## 6. Independence Contract

### 6.1 Standalone guarantees

The package must compile and function with only declared Unity dependencies. It must provide a built-in prefab spawn provider, direct setup path, standalone snapshots, diagnostics, and a Laboratory without First Light, The Observatory, The Will, The Looking Glass, The Chronicle, The Eye, The Vault, The Hand, The Ascent, The Vessel, or project assemblies.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Planned evidence |
|---|---|---|
| Installed alone | Definitions, roster, selection, spawn, control, switch, snapshot, and diagnostics work | Clean-project suite and Laboratory |
| Enter Laboratory directly | Development initializer creates one authority only when absent | ECHR-LAB-061/062 |
| Optional bridge absent | Core reports no provider and remains usable | Integration-absence tests |
| Duplicate root present | Duplicate rejected before side effects | ECHR-LAB-002/003 |
| Required configuration missing | Structured blocker; no partial roster/spawn state | ECHR-LAB-004 |
| Sample content deleted | Runtime/Editor assemblies still compile | ECHR-LAB-064 |
| Unknown definition in snapshot | Opaque unresolved member preserved | ECHR-LAB-057 |
| Custom controller used | Control ownership works through project adapter | Controller bridge tests |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Planned minimum | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine core modules | Platform | Yes | Unity 6000.0 | GameObject, Transform, ScriptableObject, scenes, serialization | Package cannot function without Unity |
| Unity Test Framework | Test only | Yes for tests | Verified at implementation | EditMode/PlayMode evidence | Runtime unaffected when tests absent |
| Other Sperk's Forge packages | Optional bridge/sample only | No | Per bridge | Explicit integrations | Core continues without them |

### 6.4 Forbidden dependencies

- Project gameplay assemblies.
- EchoControllers, EchoInput, EchoCamera, EchoSave, EchoInventory, or EchoMultiplayer in core manifests/asmdefs.
- Editor assemblies from runtime.
- Reflection-based discovery of arbitrary project character managers.
- Hidden scene names, tags, layers, input maps, Resources paths, or singleton assumptions.
- Sample prefabs or scenes as runtime requirements.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
|---|---|---|---|---:|---|
| ECHR-CAP-001 | Duplicate-safe authority | One application-session root | Approved | Yes | Runtime |
| ECHR-CAP-002 | Character definitions | Stable reusable authored definitions | Approved | Yes | Runtime/Editor |
| ECHR-CAP-003 | Durable characters | CharacterId distinct from definitions and actors | Approved | Yes | Runtime |
| ECHR-CAP-004 | Rosters | Ordered membership, revisions, queries, snapshots | Approved | Yes | Runtime |
| ECHR-CAP-005 | Availability/status | Selectability plus stable status/reason records | Approved | Yes | Runtime |
| ECHR-CAP-006 | Selection contexts | Independent selected-character truth | Approved | Yes | Runtime |
| ECHR-CAP-007 | Groups | Ordered party/squad/group membership | Approved | Yes | Runtime |
| ECHR-CAP-008 | Spawn providers | Built-in prefab provider and custom seam | Approved | Yes | Runtime |
| ECHR-CAP-009 | Runtime registry | Spawned actor binding and stale-safe handles | Approved | Yes | Runtime |
| ECHR-CAP-010 | Control ownership | Exclusive ControlOwnerId assignment | Approved | Yes | Runtime |
| ECHR-CAP-011 | Handoff participants | Prepare/commit integration seam | Approved | Yes | Runtime |
| ECHR-CAP-012 | Switching | Validated selection/spawn/control orchestration | Approved | Yes | Runtime |
| ECHR-CAP-013 | Replacement/respawn | New actor, same durable identity where appropriate | Approved | Yes | Runtime |
| ECHR-CAP-014 | Snapshot export/import | Detached versioned state | Approved | Yes | Runtime |
| ECHR-CAP-015 | Diagnostics | Structured status and ECHR codes | Approved | Yes | Runtime/Editor |
| ECHR-CAP-016 | Authoring/validation | Setup, catalogs, rosters, ID/prefab checks | Approved | Yes | Editor |
| ECHR-CAP-017 | Fellowship Laboratory | Isolated identity/roster/spawn/control proof | Approved | Yes | Sample/Test |
| ECHR-CAP-018 | Multiple simultaneous local owners | Bounded owner assignments | Approved | Yes | Runtime |
| ECHR-CAP-019 | Shared possession | More than one controlling owner per actor | Deferred | No | Runtime |
| ECHR-CAP-020 | Network provider ownership | Authoritative remote ownership | Deferred to Convergence bridge | No | Adapter |
| ECHR-CAP-021 | Editor graph roster view | Visual relationship editor | Deferred | No | Editor |
| ECHR-CAP-022 | Dynamic downloadable characters | Addressables/provider-backed definitions | Deferred | No | Provider |

### 7.2 MVP capability set

The MVP includes one root, definitions/catalog, runtime rosters, durable CharacterIds, selection contexts, availability/status, groups, built-in prefab spawning, custom provider seams, runtime actor handles, exclusive control owners, handoff participants, validated switching, replacement/respawn, detached snapshots, setup/validation, diagnostics, and one standalone Laboratory.

### 7.3 Later capability set

Later work may add shared possession, network-aware spawn/ownership providers, Addressables definitions/prefabs, editor graph visualization, squad formations, richer selection policies, character pooling adapters, and provider-backed remote catalogs. Each remains optional and must preserve core authority.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| One universal character MonoBehaviour | Rejected | Would absorb movement, combat, stats, animation, and game rules | Never without a new authority decision |
| Static global roster dictionaries | Rejected | Fragile lifecycle and test isolation | Never |
| Display name as save identity | Rejected | Rename/localization unsafe | Never |
| Automatic scene travel during respawn | Rejected | Passage/World authority | Bridge specification |
| Shared multi-owner possession | Deferred | Needs multiplayer/local-coop policy evidence | Convergence research and use case |
| Pooled character actors | Deferred | Complex state reset and ownership concerns | Proven EchoPool adapter design |
| Character customization/avatar editor | Rejected from core | Project/presentation/content responsibility | Separate package proposal |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Character definitions, catalogs, roster definitions, standard status IDs, spawn/provider policies, limits | Mutable selection, runtime actors, control leases, scene objects |
| Runtime authority/state | Rosters, durable members, availability, selection contexts, groups, runtime registry, requests, handles, control ownership, snapshots | Editor APIs, production UI, controller behavior, save transport |
| Presentation/integration | Sample readouts, bridge presenters, controller/input/camera adapters | Authoritative roster mutation outside service APIs |

### 8.2 Component topology

```text
CharacterDefinition / CharacterCatalog / RosterDefinition
                         |
                         v
                EchoCharactersRoot
                         |
       +-----------------+------------------+
       |                 |                  |
 CharacterRosterStore  SpawnCoordinator  ControlCoordinator
       |                 |                  |
 Selection/Groups   ICharacterSpawnProvider  Handoff participants
       |                 |                  |
       +----------- CharacterRuntimeRegistry+
                         |
               Snapshots / Events / Diagnostics
                         |
       Optional bridges and project adapters
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root required? | Yes, application-session by default |
| Root type | `EchoCharactersRoot` |
| Duplicate behavior | Reject duplicate before subscriptions, roster creation, provider registration, spawning, or events |
| Initialization trigger | Explicit initialize invoked by setup/prefab, First Light step, or development initializer |
| Shutdown behavior | Cancel pending operations, release control, despawn owned runtime actors by policy, clear session registries, unsubscribe |
| Direct-scene behavior | Development-only initializer creates minimum configured authority only when absent |
| Test seam | `IEchoCharactersService`, clocks/ID generators/providers injected through configuration/factory |

### 8.4 Lifecycle sequence

1. Claim root authority.
2. Validate configuration, limits, catalogs, and IDs.
3. Register built-in providers and project-supplied explicit providers.
4. Create configured runtime rosters or remain ready with none.
5. Publish Ready snapshot.
6. Accept roster, selection, spawn, control, switch, and snapshot requests.
7. Reconcile actor destruction, scene unload, provider removal, and stale handles.
8. On shutdown, stop admission, cancel pre-commit operations, release assignments, clean runtime actors, and publish final status.

### 8.5 Identity model

| Identity | Lifetime | Meaning | Durable? |
|---|---|---|---:|
| `CharacterDefinitionId` | Asset/project | Reusable definition/archetype | Yes |
| `CharacterId` | Roster/save | One durable character/member | Yes |
| `CharacterRuntimeInstanceId` | Spawned session actor | One concrete runtime representation | No |
| `RosterId` | Project/save | One roster authority set | Yes |
| `SelectionContextId` | Project/session/save by policy | Independent selection owner/context | Conditional |
| `CharacterGroupId` | Project/save | Ordered group/party/squad | Yes |
| `ControlOwnerId` | Session/provider | Entity permitted to control one actor | No by default |
| `SpawnPointId` | Project/world provider | Requested spawn location | Durable reference, provider-resolved |

### 8.6 Selection, spawn, and control separation

- **Selection** means a context points at a durable `CharacterId`.
- **Spawned** means a runtime actor currently represents that `CharacterId`.
- **Controlled** means a `ControlOwnerId` has the current exclusive assignment lease for that runtime actor.
- None implies the others unless a request explicitly chooses an orchestration policy.
- UI may change selection without spawning.
- A party may keep several actors spawned while one owner controls one.
- Respawn creates a new runtime identity while retaining durable character identity.

### 8.7 Switching transaction

A switch request has explicit policies for target selection context, control owner, target spawn preparation, and old actor disposition. The sequence is:

1. Validate request, revisions, availability, owner, target, and policy.
2. Resolve or prepare the target runtime actor.
3. Ask registered handoff participants to prepare relinquish/acquire.
4. If any required preparation fails, cancel prepared target work and preserve old truth.
5. Commit selected CharacterId and control assignment atomically.
6. Publish immutable snapshots and semantic control/selection events.
7. Notify participants of committed handoff.
8. Apply old actor disposition such as KeepSpawned or DespawnAfterCommit.
9. Report post-commit integration failures without silently rewriting authoritative ownership.

### 8.8 Failure model

| Failure | Detection | User-visible result | Fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Claim | Duplicate rejected | Existing root remains | ECHR-001 |
| Missing configuration | Initialize | Blocker report | No partial authority | ECHR-002 |
| Stable ID collision | Validation/import | Blocker | Explicit repair required | ECHR-003 |
| Unknown definition | Lookup/import | Unavailable/unresolved | Preserve durable record | ECHR-004 |
| Invalid roster mutation | Request validation | Rejected result | State unchanged | ECHR-005 |
| Spawn provider absent | Spawn prepare | Unavailable | No actor created | ECHR-006 |
| Spawn failure | Provider result | Failed request | State unchanged | ECHR-007 |
| Stale runtime handle | API call | Rejected | Current actor unaffected | ECHR-008 |
| Control conflict | Assignment validation | Denied | Existing owner retained | ECHR-009 |
| Handoff prepare failure | Switch prepare | Failed switch | Old selection/control retained | ECHR-010 |
| Post-commit participant failure | Commit callback | Warning/failure detail | Core truth retained; explicit recovery | ECHR-011 |
| Snapshot migration failure | Import prepare | Import rejected | Existing state retained; source preserved | ECHR-012 |
| External actor destruction | Reconciliation | Warning | Runtime registry repairs | ECHR-013 |
| Capacity exceeded | Admission | Rejected/Unavailable | No unbounded growth | ECHR-014 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID | Runtime mutable? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoCharactersConfiguration` | Limits, default providers, direct-scene policy, diagnostics | Configuration ID | No | Yes |
| `CharacterDefinition` | Display metadata, prefab reference, tags, spawn/provider hints | `CharacterDefinitionId` | No | Yes |
| `CharacterCatalog` | Validated definition registry | Catalog ID | No | Yes |
| `CharacterRosterDefinition` | Initial ordered roster and policies | `RosterId` | No | Yes |
| `CharacterGroupDefinition` | Optional initial group membership/order | `CharacterGroupId` | No | Yes |
| `CharacterStatusDefinition` | Project-authored status/reason metadata | Status ID | No | Yes |
| `CharacterSpawnPolicy` | Provider, single/multi-instance, scene ownership, defaults | Policy ID | No | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `CharacterRosterState` | Root | Application/session | Clear or import | Export detached version |
| `CharacterRecord` | Roster | Durable within roster | Remove explicitly | CharacterId, definition ID, availability, extensions |
| `SelectionContextState` | Roster | Session/durable by policy | Clear/import | Optional selected CharacterId |
| `CharacterGroupState` | Roster | Session/durable | Clear/import | Ordered CharacterIds |
| `CharacterRuntimeRecord` | Runtime registry | Spawn lifetime | Despawn/destroy/reconcile | Never directly serialized |
| `ControlAssignmentState` | Control coordinator | Assignment lifetime | Release/transfer/shutdown | Never serialize live lease |
| `PendingCharacterOperation` | Coordinator | Request lifetime | Complete/cancel/timeout | Never serialized |
| `CharacterHistoryEntry` | Diagnostics | Bounded session | Ring-buffer prune | Diagnostic only |

### 9.3 Stable identifiers

IDs follow SFGSS-003. Display names, prefab names, asset paths, and Unity GUIDs are never domain identities. CharacterId generation uses an injected collision-checked generator. A definition rename does not alter `CharacterDefinitionId`; respawn does not alter `CharacterId`; each spawn produces a new `CharacterRuntimeInstanceId`. Released IDs are not silently recycled. Aliases and tombstones preserve migrations.

### 9.4 Availability and status model

`CharacterAvailabilitySnapshot` separates selection disposition from narrative/project status:

- `Selectable` - eligible for selection/control according to core policy.
- `Locked` - known member but gated, commonly by progression.
- `Unavailable` - known member temporarily or permanently unavailable.
- `Unknown` - durable record exists but required definition/provider data is unresolved.

A stable optional `StatusId`, reason code, and project extension records may express Injured, Missing, Defeated, Reserved, Recovering, or game-specific meanings without extending a public enum. Availability does not remove roster membership.

### 9.5 ScriptableObject safety

Definitions and configurations remain immutable at runtime. Current selection, groups, availability, spawn state, control ownership, history, and provider state live in authority-owned runtime objects. Sample tools must restore assets after preview and may not write Play Mode state into shared ScriptableObjects.

### 9.6 Serialization and migration

`CharacterRosterSnapshotDocument` contains a document envelope, schema version, roster records, durable characters, selection contexts, groups, availability/status records, aliases applied, and opaque extension records. It excludes GameObjects, scene handles, runtime instance IDs unless included only as non-authoritative diagnostic metadata, provider objects, cancellation tokens, and live control leases. Import uses prepare/validate/migrate/apply phases and preserves unknown records.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `IEchoCharactersService` | Interface | Main query and request surface | Root/injected implementation |
| `EchoCharactersRoot` | MonoBehaviour | Unity lifecycle and authority host | Setup prefab/project |
| `CharacterDefinition` | ScriptableObject | Immutable authored character definition | Project |
| `CharacterCatalog` | ScriptableObject | Definition registry | Project |
| `CharacterRosterDefinition` | ScriptableObject | Initial roster template | Project |
| `CharacterId` / `CharacterDefinitionId` | Struct | Durable identities | Service/authoring |
| `CharacterRuntimeInstanceId` | Struct | Session actor identity | Spawn coordinator |
| `RosterId`, `SelectionContextId`, `CharacterGroupId` | Structs | Stable roster concepts | Project/service |
| `ControlOwnerId` | Struct | Session control principal | Project/bridge/provider |
| `CharacterRosterSnapshot` | Immutable model | Read-only roster truth | Service |
| `CharacterSelectionResult` | Result struct | Selection success/failure | Service |
| `CharacterSpawnRequest/Result` | Request/result | Spawn operation | Caller/service |
| `CharacterRuntimeHandle` | Struct/lease | Stale-safe actor access | Service |
| `CharacterControlLease` | Disposable struct/class | Exclusive control assignment | Service |
| `CharacterSwitchRequest/Result` | Request/result | Orchestrated switch | Caller/service |
| `ICharacterSpawnProvider` | Interface | Creates/destroys runtime actors | Built-in/project/adapter |
| `ICharacterSpawnPointProvider` | Interface | Resolves SpawnPointId to pose/context | Project/adapter |
| `ICharacterControlHandoffParticipant` | Interface | Prepares and observes control transfer | Project/bridge |
| `CharacterRosterSnapshotDocument` | DTO | Versioned detached persistence | Service/Chronicle bridge |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure | Main-loop rule |
|---|---|---|---|---|
| `InitializationState` | Current authority state | None | Immutable status | Main thread read |
| `TryGetRoster(RosterId, out snapshot)` | Query roster | Valid ID | False if absent | Main thread |
| `CreateRoster(request)` | Create runtime roster | Valid definition/ID | Structured result | Main thread |
| `AddCharacter(request)` | Add durable member | Expected revision valid | Atomic result | Main thread |
| `RemoveCharacter(request)` | Remove durable member | Policies satisfied | Atomic result | Main thread |
| `SetAvailability(request)` | Change disposition/status | Member exists | Atomic result | Main thread |
| `SelectCharacter(request)` | Change context selection | Selectable target | Structured result | Main thread |
| `SelectNext/Previous(request)` | Cycle selection | Context/roster valid | Structured result | Main thread |
| `CreateOrUpdateGroup(request)` | Mutate ordered group | Members valid | Atomic result | Main thread |
| `SpawnAsync(request)` | Prepare and spawn actor | Provider available | Awaitable result | Unity work main thread |
| `DespawnAsync(handle, reason)` | Despawn current actor | Handle current | Awaitable result | Main thread/provider |
| `AssignControlAsync(request)` | Assign/transfer owner | Spawned target | Awaitable result | Main thread |
| `ReleaseControl(lease)` | Release matching assignment | Lease current | Idempotent/stale-safe | Main thread |
| `SwitchCharacterAsync(request)` | Select/spawn/handoff | Valid policies | Awaitable result | Main thread |
| `RespawnAsync(request)` | Spawn new actor for CharacterId | Durable member valid | Awaitable result | Main thread |
| `ExportSnapshot(request)` | Produce detached DTO | Authority ready | Structured result | Main thread capture |
| `PrepareImport(document)` | Validate/migrate detached state | Supported version | Prepared import/result | Detached work where safe |
| `ApplyImport(prepared)` | Atomically replace/merge roster truth | Prepared current | Structured result | Main thread |

### 10.3 Events and callbacks

| Event | Timing | Payload | Listener assumptions |
|---|---|---|---|
| `RosterCreated` | After commit | Roster snapshot | Informational |
| `RosterChanged` | After atomic mutation | Old/new revisions and delta | Listeners do not mutate collection directly |
| `AvailabilityChanged` | After commit | CharacterId, old/new availability | UI/bridges may react |
| `SelectionChanged` | After commit | Context, old/new CharacterId | Camera/input/controller bridges react after truth changes |
| `CharacterSpawned` | After runtime registry commit | CharacterId, runtime handle | Actor is queryable |
| `CharacterDespawned` | After registry removal | IDs and reason | Handle is stale afterward |
| `ControlChanging` | Before authority commit | Read-only planned handoff | Observational; required preparation uses participant interface |
| `ControlChanged` | After commit | Owner, old/new CharacterId/runtime IDs | Listeners must tolerate provider failure |
| `CharacterSwitchCompleted` | After orchestration | Full result | Exactly once |
| `SnapshotImported` | After apply | Version and affected rosters | No live control restored implicitly |

### 10.4 Async and cancellation policy

Spawn, despawn, control preparation, switch, respawn, and import preparation may be asynchronous. Public operations use fresh Unity `Awaitable<T>` instances at implementation. Cancellation is cooperative before each documented commit point. After a spawn provider or control ownership commit becomes irreversible, cancellation returns Too Late and cleanup follows explicit policy. Timeouts use injected unscaled time. Pending operations are bounded and canceled safely on shutdown or relevant scene/provider loss.

### 10.5 API ergonomics

The novice path uses setup-created assets, a configured root prefab, built-in prefab spawning, one roster, one default selection context, and one sample control owner. The advanced path injects the service, providers, ID generators, handoff participants, status definitions, custom snapshot extensions, and bridge adapters. Static convenience access may exist but cannot be the only API.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install package.
2. Open **Tools > EchoDevGames > The Fellowship > Setup**.
3. Create or select configuration, catalog, roster definition, and root prefab.
4. Add character definitions and project prefabs.
5. Preview planned assets/scenes/settings.
6. Apply create-only-safe operations.
7. Open the Fellowship Laboratory.
8. Run validation and export setup report.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeat-safe? | Undo/backup | Report |
|---|---|---|---:|---|---|
| Create configuration | Configuration asset | None by default | Yes | Undo | Created/adopted path |
| Create catalog | Catalog asset | Optional explicit additions | Yes | Undo | IDs and entries |
| Create roster definition | Roster asset | Optional membership | Yes | Undo | Ordered membership |
| Create root prefab | Root prefab | Explicit scene insertion only | Yes | Undo | Components/references |
| Create character definition | Definition asset | None | Yes | Undo | Stable ID/prefab |
| Repair IDs | Alias/new IDs after preview | Selected assets | Conditional | Backup/Undo | Exact before/after |
| Validate prefabs | Nothing | Nothing | Yes | N/A | Binding/provider issues |
| Generate Laboratory sample | Imported Samples~ content | Sample only | Yes | Removable | Import report |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Fellowship Setup | Installer | Guided root/config/catalog/roster creation | No |
| Character Catalog Window | Designer | Search definitions, IDs, tags, prefab readiness | No |
| Roster Authoring Inspector | Designer | Ordered members, groups, defaults, policies | No |
| Runtime Roster Monitor | Tester | Inspect immutable snapshots, spawned actors, owners | Editor only |
| Snapshot Inspector | Maintainer | View detached snapshot/migration details safely | No |
| Validation Window | Maintainer | Run ECHR-VAL checks and safe repairs | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix? | Safe auto-fix? |
|---|---|---|---:|---:|
| ECHR-VAL-001 | Missing configuration | Blocker | Yes | Yes, create asset |
| ECHR-VAL-002 | Duplicate CharacterDefinitionId | Blocker | Yes | No, explicit identity choice |
| ECHR-VAL-003 | Duplicate RosterId/GroupId | Blocker | Yes | No |
| ECHR-VAL-004 | Missing prefab for built-in provider | Error | Yes | No |
| ECHR-VAL-005 | Invalid/missing actor binding | Error | Yes | No |
| ECHR-VAL-006 | Unknown initial member definition | Error | Yes | No |
| ECHR-VAL-007 | Duplicate roster member policy violation | Error | Yes | No |
| ECHR-VAL-008 | Invalid default selection | Error | Yes | Possibly select first valid |
| ECHR-VAL-009 | Unknown group member | Error | Yes | No |
| ECHR-VAL-010 | Alias cycle/collision | Blocker | Yes | No |
| ECHR-VAL-011 | Unsafe limits | Error | Yes | Yes, clamp after preview |
| ECHR-VAL-012 | Optional bridge absent | Advisory | No | No |

The package-owned Editor setup facade follows ADR-001 and SFGSS-002. It exposes deterministic Plan, Apply, Validate, Repair, and Receipt operations without giving The Workshop direct access to package internals.

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Planned routes are Git URL, local path, embedded development package, tarball, and Workshop selection. Registry distribution remains future. Every claimed route requires SFGSS-004 evidence.

### 12.2 Minimal scene setup

- One `EchoCharactersRoot` or explicit injected service host.
- One configuration asset.
- One character catalog.
- At least one roster definition or runtime roster request.
- Character definitions with project prefabs when using built-in spawning.
- Optional actor binding component on spawned prefabs.

### 12.3 Boot-scene setup

The root may live in a canonical Boot scene and persist for the application session. First Light integration is optional and only invokes package setup/initialization. EchoCharacters remains the authority for its own root.

### 12.4 Direct-scene setup

A development-only `EchoCharactersDirectSceneInitializer` creates the configured minimum root and sample roster only when no authority exists. It identifies the session as development initialization and can be excluded from release builds. It never creates input, camera, UI, save, or controller peers.

### 12.5 Scene isolation rule

The Fellowship Laboratory contains only EchoCharacters, declared Unity dependencies, and redistributable sample actors/providers. Controller, input, camera, save, inventory, and multiplayer examples belong in separate Integration Laboratories.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Laboratory purpose

The **Fellowship Roster and Possession Laboratory** proves durable identity, roster membership, availability, selection contexts, groups, spawning, runtime handles, exclusive control ownership, switching, respawn, snapshot export/import, failure behavior, reset, and duplicate protection without unrelated Echo packages.

### 13.2 Required Laboratory contents

- Four simple project-neutral character prefabs sharing and differing definitions.
- One catalog and roster with selectable, locked, and unavailable members.
- Two selection contexts and two simulated control owners.
- Built-in prefab provider and deterministic fake failure provider.
- Explicit spawn points.
- Runtime readout for IDs, roster revision, selection, spawn state, owners, pending operations, and diagnostics.
- Buttons/commands for every listed scenario.
- Reset to a deterministic initial state.
- Visible instructions and sample README.

### 13.3 Laboratory acceptance checklist

| Test | Scenario | Action | Expected result | Type | Status |
|---|---|---|---|---|---|
| ECHR-LAB-001 | Root claims authority | Enter the Laboratory with one configured root. | One Fellowship authority initializes and reports Ready. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-002 | Duplicate root in scene | Enable a second root before Play Mode. | The duplicate is rejected before roster, spawn, or event side effects. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-003 | Duplicate root introduced later | Instantiate a second root after initialization. | The new root is rejected and existing state remains authoritative. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-004 | Missing configuration | Remove the active configuration. | Initialization fails visibly with a structured blocker. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-005 | Empty catalog | Use a valid empty character catalog. | The service initializes Ready with an empty roster and clear diagnostics. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-006 | Definition ID collision | Create two definitions with the same CharacterDefinitionId. | Validation blocks approval and identifies both assets. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-007 | Roster ID collision | Create two roster definitions with the same RosterId. | Validation blocks setup until IDs are repaired explicitly. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-008 | Create runtime roster | Create a roster from a valid roster definition. | A roster snapshot is published with stable ordered membership. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-009 | Add durable character | Add a character definition to a mutable roster. | A new durable CharacterId is generated and the roster revision advances once. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-010 | Add duplicate definition twice | Add the same definition twice where duplicates are allowed. | Two distinct CharacterIds reference one definition without identity collision. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-011 | Reject duplicate definition | Add a repeated definition to a roster that forbids duplicates. | The request is rejected without mutation. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-012 | Remove unselected character | Remove an ordinary roster member. | Membership, groups, and snapshots update atomically. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-013 | Remove selected character | Remove the selected character with fallback enabled. | Selection advances deterministically to the next selectable member. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-014 | Remove controlled character | Request removal of a possessed spawned character. | The request follows configured release/despawn policy or is rejected safely. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-015 | Availability becomes locked | Set a selectable character to Locked. | Selection requests are denied and existing selection follows policy. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-016 | Availability becomes unavailable | Apply a temporary unavailable status and reason. | The structured snapshot separates selectability from narrative status. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-017 | Project-defined status | Apply a custom stable status ID. | The status is preserved without changing core enum contracts. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-018 | Select by CharacterId | Select an available member in the default selection context. | Selection changes once and publishes old/new IDs. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-019 | Select locked character | Request selection of a locked member. | The result is Denied with an actionable reason and no state change. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-020 | Cycle next | Cycle forward across mixed selectable and unavailable members. | The next selectable member is chosen deterministically. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-021 | Cycle previous | Cycle backward with wrap enabled. | The previous selectable member is chosen deterministically. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-022 | No selectable members | Make every member unavailable and request cycle. | The request returns Unavailable without clearing state silently. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-023 | Independent selection contexts | Select different characters for PlayerOne and PlayerTwo contexts. | Each context retains its own selected CharacterId. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-024 | Delete selection context | Remove a temporary selection context. | Its selection state is removed without affecting other contexts. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-025 | Create party group | Create an ordered group from existing members. | Group membership and order are published in the roster snapshot. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-026 | Character in multiple groups | Add one CharacterId to Party and RescueTeam groups. | Both memberships remain valid and independently ordered. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-027 | Invalid group member | Add an unknown CharacterId to a group. | The operation is rejected atomically. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-028 | Spawn selected character | Spawn the selected member at an explicit pose. | A RuntimeInstanceId and stale-safe spawn handle are returned. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-029 | Spawn by point ID | Spawn using an explicit spawn-point provider and SpawnPointId. | The provider resolves a pose and the character spawns there. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-030 | Unknown spawn point | Request a missing SpawnPointId. | The request fails before prefab creation. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-031 | Missing prefab | Spawn a definition without a prefab reference. | The built-in provider returns a structured configuration failure. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-032 | Provider failure | Use a simulated spawn provider that fails. | No runtime record, control assignment, or selection mutation is committed. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-033 | Concurrent spawn duplicate | Request two spawns for one single-instance character. | The configured coalescing or rejection policy is deterministic. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-034 | Multiple runtime instances allowed | Spawn a definition configured for multiple simultaneous instances. | Distinct RuntimeInstanceIds and handles are produced. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-035 | Despawn ordinary instance | Despawn a valid runtime instance. | Provider cleanup completes and runtime registries update once. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-036 | Stale despawn handle | Despawn, respawn, then reuse the old handle. | The stale handle is rejected without affecting the new instance. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-037 | Externally destroyed actor | Destroy the spawned GameObject outside the service. | The next reconciliation removes stale runtime state and reports the violation. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-038 | Scene unload reconciliation | Unload a scene containing spawned actors. | Scene-scoped instances are reconciled without corrupting durable roster state. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-039 | Assign exclusive control | Assign ControlOwnerId PlayerOne to a spawned character. | The assignment commits and a control lease is returned. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-040 | Control unspawned character | Assign control to an unspawned member. | The request is rejected unless an explicit auto-spawn policy was requested. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-041 | Character already controlled | Assign a second owner to an exclusively controlled character. | The request is denied or follows explicit transfer policy. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-042 | Owner already controls another | Assign one owner to a second character. | An atomic transfer occurs or the request is rejected by policy. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-043 | Out-of-order control release | Release older and newer control leases out of order. | Only the current matching lease can change ownership. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-044 | Stale control lease | Transfer control, then release the previous lease. | The stale lease is ignored and diagnosed. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-045 | Control handoff participant prepare failure | A controller bridge refuses acquisition during prepare. | No core ownership change commits. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-046 | Control handoff participant commit failure | A bridge fails after ownership publication. | Core truth remains committed, failure is diagnosed, and recovery is explicit. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-047 | Switch to spawned character | Switch one owner from Character A to already spawned Character B. | Selection and ownership commit atomically with one completion result. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-048 | Switch requiring spawn | Switch to an unspawned character with SpawnThenTransfer policy. | The target prepares before ownership changes. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-049 | Switch spawn failure | Fail target spawn during a switch. | Old selection and control remain unchanged. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-050 | Switch keep old spawned | Switch with old disposition KeepSpawned. | Old actor remains spawned but uncontrolled. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-051 | Switch despawn old | Switch with old disposition DespawnAfterCommit. | Old actor despawns only after the new control assignment commits. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-052 | Rapid switch requests | Submit A->B and A->C requests rapidly. | Admission policy rejects, queues, or replaces deterministically. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-053 | Replace defeated character | Request replacement from a defeated member to a valid substitute. | The durable old character remains in roster history while active assignment changes. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-054 | Respawn character | Respawn a despawned durable character at a valid spawn point. | A new RuntimeInstanceId is created for the same CharacterId. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-055 | Export snapshot | Export rosters, members, availability, groups, and selection. | A detached versioned snapshot contains no GameObject or live lease references. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-056 | Import snapshot | Import a valid snapshot into an empty authority. | Durable roster truth is restored while runtime actors remain unspawned. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-057 | Unknown definition on import | Import a CharacterId whose definition is absent. | The unresolved record is preserved and surfaced diagnostically. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-058 | Unknown extension record | Import a project extension payload with no provider installed. | The opaque record survives round trip unchanged. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-059 | Alias migration | Import a snapshot containing an old CharacterDefinitionId alias. | Migration resolves the canonical ID without changing CharacterId. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-060 | Control not restored from save | Import a snapshot that records last active selection. | Selection may restore, but live control leases and GameObjects do not. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-061 | Direct-scene initializer | Enter the Laboratory scene without a preexisting root. | A development-only root and sample roster are created once. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-062 | Direct-scene duplicate protection | Enter with a canonical root already present. | The initializer adopts the existing authority and creates nothing. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-063 | Observatory absent | Run all core workflows without EchoDiagnostics. | Structured status remains available through standalone APIs and inspectors. | Manual/automated as specified at implementation | Not run |
| ECHR-LAB-064 | Sample removal | Delete imported Laboratory samples. | Runtime and Editor package assemblies still compile independently. | Manual/automated as specified at implementation | Not run |

### 13.4 Optional showcase and integration samples

| Sample | Packages | Purpose | Why not standalone proof |
|---|---|---|---|
| Fellowship + Vessel Handoff | EchoCharacters + EchoControllers | Switch controller ownership | Requires controller package |
| Fellowship + Will Local Players | EchoCharacters + EchoInput | Map users/devices to owners | Requires input authority |
| Fellowship + Eye Camera Target | EchoCharacters + EchoCamera | Follow active controlled character | Requires camera authority |
| Fellowship + Chronicle Save | EchoCharacters + EchoSave | Persist roster snapshot | Requires save transport |
| Fellowship + Ascent Unlocks | EchoCharacters + EchoProgression | Map unlocks to availability | Requires progression authority |
| Fellowship + Vault Ownership | EchoCharacters + EchoInventory | Map characters to containers/equipment | Requires inventory authority |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

Core runtime is nonvisual. It exposes snapshots, semantic status, display references, and request/result APIs. Production roster, party, selection, respawn, and character-switch screens belong to The Looking Glass or project UI through a bridge. Sample UI is Laboratory-only.

### 14.2 Required presentation states

- Ready with roster.
- Empty roster.
- Selected.
- Spawned/unspawned.
- Controlled/uncontrolled.
- Locked.
- Unavailable with reason.
- Unresolved definition.
- Busy switching/spawning.
- Warning/failure.

### 14.3 Accessibility requirements

- Status must not rely on color alone.
- Selection order and labels must be screen-reader/assistive friendly where the UI backend supports it.
- Character portraits/icons require text alternatives supplied by project content.
- Switching feedback may respect reduced motion and timing through UI/camera/feedback bridges.
- Input method is not hard-coded.
- Locked and unavailable reasons must be available as semantic data rather than icon-only presentation.

### 14.4 Visual customization

Names, portraits, icons, colors, roles, biographies, status art, selection frames, animations, and transitions are project-owned and replaceable without editing runtime code.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Initialization/root status | API/Inspector | Editor/dev/release-safe summary | Low |
| Roster/member/group counts | API/Inspector | Configurable | Low |
| Selection contexts | API/Inspector | Configurable | Low |
| Spawn/runtime registry | API/Inspector | Development by default | Low/bounded |
| Control assignments | API/Inspector | Development, redacted | Low |
| Pending operations | API/Inspector | Development | Bounded |
| Recent failures/history | Ring buffer/export | Development/support | Bounded |
| Validation report | Editor window/file | Editor | On demand |

### 15.2 Structured status

Status includes package version, initialization state, root identity, configuration source ID, roster IDs/revisions, member counts by disposition, group counts, selection contexts, spawned actor counts, provider health, control-owner counts, pending operations, capacity use, latest result, and recent diagnostic codes. Display names and arbitrary project metadata are omitted by default.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| ECHR-001 | Error | Duplicate root rejected | Remove duplicate setup/source scene |
| ECHR-002 | Blocker | Missing/invalid configuration | Run setup/validation |
| ECHR-003 | Blocker | Stable ID collision | Resolve explicitly with aliases/migration |
| ECHR-004 | Warning | Definition unresolved | Restore definition or migration alias |
| ECHR-005 | Warning | Roster mutation rejected | Inspect result/revision/policy |
| ECHR-006 | Warning | Spawn provider unavailable | Register provider or correct definition policy |
| ECHR-007 | Error | Spawn/despawn provider failed | Inspect provider receipt/exception |
| ECHR-008 | Advisory | Stale runtime handle rejected | Refresh caller snapshot/handle |
| ECHR-009 | Warning | Control conflict | Inspect owner and assignment policy |
| ECHR-010 | Error | Handoff preparation failed | Inspect participant result |
| ECHR-011 | Error | Post-commit participant failed | Run explicit recovery; core truth remains |
| ECHR-012 | Error | Snapshot import/migration failed | Preserve source and correct version/data |
| ECHR-013 | Warning | Runtime actor destroyed externally | Fix lifecycle ownership |
| ECHR-014 | Warning | Capacity/exhaustion limit reached | Adjust safe configuration or request rate |

### 15.4 Observatory bridge

A separate bridge publishes redacted roster, spawn, selection, owner, pending-operation, provider-health, and recent-result snapshots to The Observatory. EchoCharacters never requires EchoDiagnostics.

### 15.5 Logging policy

Logs are categorized and code-prefixed, avoid per-frame spam, redact player-entered/display names and arbitrary extension payloads by default, and distinguish development detail from release-safe summaries. Exceptions from providers are bounded and isolated.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Definitions/catalogs | Project asset | Project | Asset | Unity assets |
| Durable roster membership/CharacterId | Profile/slot/project | EchoCharacters state | Yes when project chooses | Snapshot provider |
| Availability/status | Profile/slot/project | EchoCharacters state | Usually | Snapshot provider |
| Selection contexts | Session or durable by policy | EchoCharacters state | Optional | Snapshot provider |
| Groups/order | Profile/slot/project | EchoCharacters state | Optional/usually | Snapshot provider |
| RuntimeInstanceId/GameObject | Session | Runtime registry | No | None |
| Live ControlOwnerId/lease | Session/provider | Control coordinator | No by default | None |
| Last selected/active CharacterId | Profile/slot | EchoCharacters state | Optional | Snapshot provider |
| Extension records | Provider-defined | Provider/project | Preserve | Opaque snapshot records |

### 16.2 Standalone behavior

Without Chronicle, the project may export/import snapshots through its own backend or keep state session-only. EchoCharacters never chooses a filename, slot model, cloud provider, or autosave policy.

### 16.3 Optional participant/provider contract

The Chronicle bridge registers a versioned participant that captures detached roster documents, prepares/migrates imports, and applies them after any project-required scene preparation. Missing character definitions and unknown extension records remain preserved. Import never silently spawns actors or restores live possession. The project explicitly decides post-load spawning and control assignment.

### 16.4 Failure and recovery

- Missing state creates configured defaults.
- Corrupt/unsupported state is rejected before mutation.
- Newer unknown versions are preserved and reported.
- Older supported versions migrate through contiguous steps.
- Unknown definitions become unresolved durable records.
- Unknown extension records round-trip unchanged.
- Failed apply retains current state and prepared source evidence.
- Partial imports are not published.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Connections are explicit, removable, versioned, and follow SFGSS-002. The package publishes stable IDs, immutable snapshots, requests, events, and provider interfaces. A peer package cannot silently become required because it is installed.

### 17.2 Planned integrations

| Other authority | Connection | Owner of bridge | Direction | Data/events | Required? |
|---|---|---|---|---|---:|
| First Light | Startup step | EchoCharacters/bridge | Launch -> Characters | Initialize configuration/root | No |
| Observatory | Diagnostics provider | Separate bridge | Characters -> Diagnostics | Redacted health/snapshots | No |
| Chronicle | Save participant | Separate bridge | Bidirectional | Detached roster document | No |
| The Ascent | Availability mapper | Separate bridge | Progression -> Characters | Unlock/lock state and stable mappings | No |
| The Will | Local owner mapper | Separate bridge | Bidirectional | User/ControlOwnerId and semantic commands | No |
| The Looking Glass | Presenter/commands | Separate bridge | Bidirectional | Roster snapshots and requests | No |
| The Eye | Camera target mapper | Separate bridge | Characters -> Camera | Active runtime target/warp metadata | No |
| The Hand | Interactor owner mapper | Separate bridge | Characters -> Interaction | Character/control ownership metadata | No |
| The Vault | Container ownership mapper | Separate bridge | Bidirectional | CharacterId/container IDs | No |
| The Vessel | Control-handoff adapter | Separate bridge | Bidirectional | Controller target enable/disable/prepare | No |
| The Convergence | Network ownership/spawn adapter | Provider bridge | Bidirectional | Player authority, spawn, ownership | No |
| Atlas/Passage/project | Spawn context adapter | Project/bridge | World -> Characters | SpawnPointId/pose/context | No |

### 17.3 Bridge placement decision

Two-package Echo integrations ship separately when direct references to both are required. Project-specific controller, animation, pet, party, or world translation remains project adapter code. Provider SDK/network integrations remain separate provider adapters. Tiny compile-safe owner integrations may live in the owner package only when SFGSS-002's removal test is satisfied.

### 17.4 Integration failure behavior

Missing peers produce Unavailable or advisory results without changing core behavior. Version mismatch blocks only the bridge. Initialization order is explicit and registration handles are disposable. On teardown, the bridge stops new work, unregisters participants, releases leases/mappings, then peer/core packages shut down. Post-commit bridge failure cannot silently rewrite roster truth.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Planned target | Measurement | Release threshold |
|---|---|---|---|
| Selection query/mutation | No frame-visible stall at advertised roster size | Profiler + stress Laboratory | Empirical target set before beta |
| Snapshot query | Bounded allocations and immutable reuse where practical | Allocation profiler | Measured threshold before beta |
| Control transfer | No unbounded scanning/reflection | PlayMode stress | Pass functional/performance budget |
| Spawn coordination overhead | Small relative to prefab/provider cost | Simulated provider tests | Measured before beta |
| Diagnostics | No per-frame full roster serialization | Profiler | Sampling bounded/configurable |

All values remain `Not run` until implementation.

### 18.2 Allocation policy

- No LINQ or reflection in hot paths unless profiling proves acceptable and specification is revised.
- Roster/member maps are keyed by stable IDs.
- Snapshots may cache immutable arrays by revision.
- Events carry compact semantic payloads.
- Histories, pending operations, owners, groups, and runtime records are bounded.
- Provider exceptions and strings are not generated per frame.

### 18.3 Scene and domain reload behavior

Static convenience access resets deterministically. Events unsubscribe. Scene unload reconciles scene-owned actors and points. Durable roster state remains in the persistent authority. Enter Play Mode options, domain reload, disabled reload, shutdown, and direct-scene helper behavior require explicit tests.

### 18.4 Scalability limits

Initial configurable safe defaults are planned around 256 durable members per roster, 32 rosters, 64 groups per roster, 16 selection contexts per roster, 16 control owners, 128 simultaneous spawned actors, 16 pending async operations, and bounded 256-entry histories. These are design limits, not performance claims, and remain subject to measured implementation evidence.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Core data may include project-authored character names, portraits, tags, and optional player-created display metadata. Support snapshots omit display names, biographies, raw extension payloads, profile names, private paths, and provider account/network identifiers by default. Stable IDs are not treated as secrets.

### 19.2 Trust boundaries

Imported snapshots, provider results, network ownership claims, extension records, prefab references, and user-authored metadata are untrusted until validated. Sizes/counts are bounded. Unknown payloads remain opaque. Network authority is never inferred from a local control assignment. No credentials or platform account data belong in core snapshots.

### 19.3 Platform behavior

| Platform | Planned status | Special behavior | Validation required |
|---|---:|---|---|
| Windows | Planned | Standard GameObject spawning | Clean build/Laboratory |
| macOS | Planned | Standard GameObject spawning | Clean build/Laboratory |
| Linux | Planned | Standard GameObject spawning | Clean build/Laboratory |
| WebGL | Planned | Async/provider constraints may differ | Player build tests |
| Mobile | Planned | Memory limits and app suspend require evidence | Device tests |
| Console | Unknown | SDK/provider requirements | Platform approval/tests |

No platform is marked Supported until SFGSS-004 evidence exists.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-characters/
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
│   ├── EchoCharactersRoot.cs
│   ├── IEchoCharactersService.cs
│   └── Initialization/
├── Identity/
├── Definitions/
├── Rosters/
├── Availability/
├── Selection/
├── Groups/
├── Spawning/
│   ├── Providers/
│   ├── SpawnPoints/
│   └── RuntimeRegistry/
├── Control/
│   ├── Owners/
│   ├── Handoff/
│   └── Switching/
├── Persistence/
├── Diagnostics/
└── EchoDevGames.EchoCharacters.Runtime.asmdef

Editor/
├── Setup/
├── Authoring/
├── Validation/
├── Inspectors/
├── Monitoring/
└── EchoDevGames.EchoCharacters.Editor.asmdef

Samples~/
└── Standalone Labs/
    └── Fellowship Roster and Possession Laboratory/

Tests/
├── Editor/
│   └── EchoDevGames.EchoCharacters.Tests.Editor.asmdef
└── Runtime/
    └── EchoDevGames.EchoCharacters.Tests.Runtime.asmdef
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoCharacters.Runtime` | Runtime | Unity core only | Yes | Neutral runtime authority/API |
| `EchoDevGames.EchoCharacters.Editor` | Editor | Runtime + UnityEditor | No | Setup, authoring, validation, monitoring |
| `EchoDevGames.EchoCharacters.Tests.Runtime` | Test | Runtime + Test Framework | No | EditMode/PlayMode tests |
| `EchoDevGames.EchoCharacters.Tests.Editor` | Editor test | Runtime + Editor + Test Framework | No | Editor tooling tests |

Optional bridges/providers use separate packages/assemblies and declare both dependencies explicitly.

### 20.4 Repository files

README, five-minute quick start, setup guide, API guide, architecture/lifecycle guide, roster/identity guide, spawning/control guide, snapshot/migration guide, diagnostics reference, Laboratory guide, bridge index, known limitations, license, notices, contribution/support/security guidance, release checklist, Current Notes, ADRs, test evidence, and stable `.meta` files.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | 6000.0 planned | 6000.3.8f1 development baseline | Empirical support remains Not run |
| Unity Test Framework | Resolve at implementation | Not run | Test only |
| Optional Echo bridges | Per bridge specification | Not run | Never core dependencies |

### 21.2 Semantic versioning policy

- Patch: compatible fixes, diagnostics, validation, documentation.
- Minor: additive definitions, requests, result fields with safe defaults, providers, Editor tools, samples.
- Major: breaking public API, stable ID semantics, snapshot schema with unsupported migration, assembly/package ID changes, control/switch commit semantics.

### 21.3 Deprecation policy

Deprecated public APIs remain documented with migration paths for at least one compatible release line unless security/data-loss risk requires faster removal. Stable diagnostic and ID meanings are not repurposed. Removed data fields require migration or explicit tombstones.

### 21.4 GUID and asset compatibility

Public scripts, prefabs, templates, definitions, configuration assets, and samples preserve committed `.meta` GUIDs. Moves/renames retain identity. Unity asset GUIDs remain Editor asset identities and are never substituted for CharacterDefinitionId or CharacterId.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview and authority boundaries.
- Installation and five-minute quick start.
- Character definition/catalog authoring.
- Roster, availability, selection-context, and group guide.
- Built-in spawning and custom provider guide.
- Control ownership and switching guide.
- Snapshot/export/import guide.
- Laboratory guide.
- Diagnostic-code reference.
- Integration guide index.
- Migration, removal, and known limitations.

### 22.2 Required developer documentation

- Identity model and stable IDs.
- Root lifecycle and duplicate protection.
- Atomic roster mutation and revision rules.
- Spawn/runtime registry lifecycle.
- Control handoff and switch commit points.
- Provider and bridge extension contracts.
- Persistence schema/migrations.
- Testing and release workflow.
- ADRs, Current Notes, and checkpoint status.

### 22.3 Documentation truth rule

Examples must compile against the documented release. Screenshots/menu paths match the tested Unity baseline. Planned compatibility, performance, platform, provider, and migration claims remain explicitly `Not run` until evidence exists.

### 22.4 Living repository and Obsidian workflow

Documentation lives in Git and opens directly in Obsidian. Discoveries begin in Current Notes and are promoted at checkpoints. Architecture/behavior changes update the specification or an ADR before code. Defects and evidence move to permanent reports. Git history is the archive.

### 22.5 Repository scan and handoff order

README, SFGSS-000, this specification, applicable ADRs/bridge specs, SFGSS-002 through SFGSS-005, Current Notes, active checkpoint, test reports, changelog, implementation, and automated tests.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, rosters, availability, selection, groups, policies, migrations | Pure authority tests | Yes |
| PlayMode unit/integration | Root lifecycle, spawning, runtime registry, control, switching | Simulated/built-in providers | Yes |
| Standalone Laboratory | User-visible isolated lifecycle | 64 scenarios | Yes |
| Bridge Integration Lab | Optional peer connection | Controller/Input/Camera/Save/etc. | When bridge ships |
| Showcase | Combined party/switching presentation | Portfolio/sample | No |
| Clean-project install | Packaging and independence | Git/local/tarball | Yes |
| Existing-project migration | Rescuers2D/Hackulos adoption | Preserve-until-parity | Before claim |

### 23.2 Required test categories

Installation, assemblies, root lifecycle, configuration, IDs, rosters, availability, selection contexts, groups, spawning, runtime actors, control owners, handoff, switching, replacement, respawn, snapshots, export/import, migrations, diagnostics, Editor setup, validation, direct-scene entry, Laboratory, performance, scalability, scene/domain reload, privacy/security, platform, removal, bridges, and staged release gates.

### 23.3 Test case registry

| Test ID | Category | Planned test | Automation | Status |
|---|---|---|---|---|
| ECHR-T-001 | Package installation | Git install succeeds | Planned | Not run |
| ECHR-T-002 | Package installation | local path install succeeds | Planned | Not run |
| ECHR-T-003 | Package installation | embedded package compiles | Planned | Not run |
| ECHR-T-004 | Package installation | tarball install succeeds | Planned | Not run |
| ECHR-T-005 | Package installation | reinstall preserves project assets | Planned | Not run |
| ECHR-T-006 | Package installation | upgrade preserves GUIDs | Planned | Not run |
| ECHR-T-007 | Package installation | sample removal is safe | Planned | Not run |
| ECHR-T-008 | Package installation | package removal leaves project data | Planned | Not run |
| ECHR-T-009 | Assembly boundaries | runtime has no UnityEditor reference | Planned | Not run |
| ECHR-T-010 | Assembly boundaries | Editor assembly is isolated | Planned | Not run |
| ECHR-T-011 | Assembly boundaries | tests use dedicated assemblies | Planned | Not run |
| ECHR-T-012 | Assembly boundaries | optional peers are absent safely | Planned | Not run |
| ECHR-T-013 | Assembly boundaries | bridge types do not leak into core | Planned | Not run |
| ECHR-T-014 | Assembly boundaries | autoReferenced policy matches SFGSS-002 | Planned | Not run |
| ECHR-T-015 | Assembly boundaries | GUID references are stable | Planned | Not run |
| ECHR-T-016 | Assembly boundaries | circular references are absent | Planned | Not run |
| ECHR-T-017 | Root lifecycle | single root initializes | Planned | Not run |
| ECHR-T-018 | Root lifecycle | preexisting duplicate is rejected | Planned | Not run |
| ECHR-T-019 | Root lifecycle | late duplicate is rejected | Planned | Not run |
| ECHR-T-020 | Root lifecycle | root shutdown is idempotent | Planned | Not run |
| ECHR-T-021 | Root lifecycle | events unsubscribe on shutdown | Planned | Not run |
| ECHR-T-022 | Root lifecycle | domain reload reset is safe | Planned | Not run |
| ECHR-T-023 | Root lifecycle | direct scene initializer creates once | Planned | Not run |
| ECHR-T-024 | Root lifecycle | direct scene initializer adopts existing | Planned | Not run |
| ECHR-T-025 | Configuration | valid configuration initializes | Planned | Not run |
| ECHR-T-026 | Configuration | missing configuration blocks visibly | Planned | Not run |
| ECHR-T-027 | Configuration | invalid limits are rejected | Planned | Not run |
| ECHR-T-028 | Configuration | empty catalog is accepted | Planned | Not run |
| ECHR-T-029 | Configuration | missing catalog is diagnosed | Planned | Not run |
| ECHR-T-030 | Configuration | configuration remains immutable | Planned | Not run |
| ECHR-T-031 | Configuration | repair is repeatable | Planned | Not run |
| ECHR-T-032 | Configuration | setup report is deterministic | Planned | Not run |
| ECHR-T-033 | Definition identity | definition ID is generated | Planned | Not run |
| ECHR-T-034 | Definition identity | duplicate ID is detected | Planned | Not run |
| ECHR-T-035 | Definition identity | empty ID is detected | Planned | Not run |
| ECHR-T-036 | Definition identity | display rename preserves ID | Planned | Not run |
| ECHR-T-037 | Definition identity | asset move preserves identity | Planned | Not run |
| ECHR-T-038 | Definition identity | alias resolves canonical ID | Planned | Not run |
| ECHR-T-039 | Definition identity | tombstone is preserved | Planned | Not run |
| ECHR-T-040 | Definition identity | runtime never depends on AssetDatabase GUID | Planned | Not run |
| ECHR-T-041 | Character identity | CharacterId is durable | Planned | Not run |
| ECHR-T-042 | Character identity | duplicate definitions produce distinct CharacterIds | Planned | Not run |
| ECHR-T-043 | Character identity | RuntimeInstanceId is session-only | Planned | Not run |
| ECHR-T-044 | Character identity | respawn preserves CharacterId | Planned | Not run |
| ECHR-T-045 | Character identity | runtime instance IDs do not collide | Planned | Not run |
| ECHR-T-046 | Character identity | removed IDs are not silently reused | Planned | Not run |
| ECHR-T-047 | Character identity | import preserves CharacterId | Planned | Not run |
| ECHR-T-048 | Character identity | logs redact display name by default | Planned | Not run |
| ECHR-T-049 | Roster creation | roster creates from definition | Planned | Not run |
| ECHR-T-050 | Roster creation | empty roster creates | Planned | Not run |
| ECHR-T-051 | Roster creation | duplicate roster ID is rejected | Planned | Not run |
| ECHR-T-052 | Roster creation | roster revision starts deterministically | Planned | Not run |
| ECHR-T-053 | Roster creation | default ordering is preserved | Planned | Not run |
| ECHR-T-054 | Roster creation | initial selection follows policy | Planned | Not run |
| ECHR-T-055 | Roster creation | immutable source is not mutated | Planned | Not run |
| ECHR-T-056 | Roster creation | creation event follows commit | Planned | Not run |
| ECHR-T-057 | Roster mutation | add commits atomically | Planned | Not run |
| ECHR-T-058 | Roster mutation | remove commits atomically | Planned | Not run |
| ECHR-T-059 | Roster mutation | batch add is atomic | Planned | Not run |
| ECHR-T-060 | Roster mutation | batch failure rolls back | Planned | Not run |
| ECHR-T-061 | Roster mutation | duplicate policy is enforced | Planned | Not run |
| ECHR-T-062 | Roster mutation | unknown member removal is rejected | Planned | Not run |
| ECHR-T-063 | Roster mutation | revision increments once | Planned | Not run |
| ECHR-T-064 | Roster mutation | snapshot is immutable | Planned | Not run |
| ECHR-T-065 | Availability | selectable availability succeeds | Planned | Not run |
| ECHR-T-066 | Availability | locked availability denies selection | Planned | Not run |
| ECHR-T-067 | Availability | unavailable reason is exposed | Planned | Not run |
| ECHR-T-068 | Availability | custom status ID is preserved | Planned | Not run |
| ECHR-T-069 | Availability | availability change is atomic | Planned | Not run |
| ECHR-T-070 | Availability | current selection follows policy | Planned | Not run |
| ECHR-T-071 | Availability | controlled character policy is enforced | Planned | Not run |
| ECHR-T-072 | Availability | availability event follows commit | Planned | Not run |
| ECHR-T-073 | Selection contexts | default context selects | Planned | Not run |
| ECHR-T-074 | Selection contexts | multiple contexts remain independent | Planned | Not run |
| ECHR-T-075 | Selection contexts | locked target is denied | Planned | Not run |
| ECHR-T-076 | Selection contexts | unknown target is unavailable | Planned | Not run |
| ECHR-T-077 | Selection contexts | next cycle skips unavailable | Planned | Not run |
| ECHR-T-078 | Selection contexts | previous cycle wraps deterministically | Planned | Not run |
| ECHR-T-079 | Selection contexts | empty cycle returns unavailable | Planned | Not run |
| ECHR-T-080 | Selection contexts | context deletion is isolated | Planned | Not run |
| ECHR-T-081 | Group membership | group creates | Planned | Not run |
| ECHR-T-082 | Group membership | member adds | Planned | Not run |
| ECHR-T-083 | Group membership | member removes | Planned | Not run |
| ECHR-T-084 | Group membership | order changes atomically | Planned | Not run |
| ECHR-T-085 | Group membership | unknown member is rejected | Planned | Not run |
| ECHR-T-086 | Group membership | duplicate member policy is enforced | Planned | Not run |
| ECHR-T-087 | Group membership | multiple groups are supported | Planned | Not run |
| ECHR-T-088 | Group membership | group snapshot is immutable | Planned | Not run |
| ECHR-T-089 | Spawn provider registration | built-in provider registers | Planned | Not run |
| ECHR-T-090 | Spawn provider registration | custom provider registers | Planned | Not run |
| ECHR-T-091 | Spawn provider registration | duplicate provider ID is rejected | Planned | Not run |
| ECHR-T-092 | Spawn provider registration | provider removal is safe | Planned | Not run |
| ECHR-T-093 | Spawn provider registration | missing provider is unavailable | Planned | Not run |
| ECHR-T-094 | Spawn provider registration | provider capability is reported | Planned | Not run |
| ECHR-T-095 | Spawn provider registration | registration handle is stale-safe | Planned | Not run |
| ECHR-T-096 | Spawn provider registration | provider exception is isolated | Planned | Not run |
| ECHR-T-097 | Spawn requests | explicit pose spawn succeeds | Planned | Not run |
| ECHR-T-098 | Spawn requests | spawn point ID succeeds | Planned | Not run |
| ECHR-T-099 | Spawn requests | unknown spawn point fails | Planned | Not run |
| ECHR-T-100 | Spawn requests | missing prefab fails | Planned | Not run |
| ECHR-T-101 | Spawn requests | duplicate spawn coalesces or rejects | Planned | Not run |
| ECHR-T-102 | Spawn requests | multi-instance policy is enforced | Planned | Not run |
| ECHR-T-103 | Spawn requests | spawn cancellation before commit works | Planned | Not run |
| ECHR-T-104 | Spawn requests | spawn result includes stable handles | Planned | Not run |
| ECHR-T-105 | Spawn lifecycle | runtime actor binds after creation | Planned | Not run |
| ECHR-T-106 | Spawn lifecycle | spawn event follows registration | Planned | Not run |
| ECHR-T-107 | Spawn lifecycle | disable behavior follows policy | Planned | Not run |
| ECHR-T-108 | Spawn lifecycle | despawn invokes provider once | Planned | Not run |
| ECHR-T-109 | Spawn lifecycle | external destruction reconciles | Planned | Not run |
| ECHR-T-110 | Spawn lifecycle | scene unload reconciles | Planned | Not run |
| ECHR-T-111 | Spawn lifecycle | shutdown despawns owned actors | Planned | Not run |
| ECHR-T-112 | Spawn lifecycle | runtime registry has no leaks | Planned | Not run |
| ECHR-T-113 | Spawn points | point provider registers | Planned | Not run |
| ECHR-T-114 | Spawn points | stable SpawnPointId resolves | Planned | Not run |
| ECHR-T-115 | Spawn points | scene-scoped point expires | Planned | Not run |
| ECHR-T-116 | Spawn points | duplicate point ID is detected | Planned | Not run |
| ECHR-T-117 | Spawn points | pose validity is checked | Planned | Not run |
| ECHR-T-118 | Spawn points | provider priority is deterministic | Planned | Not run |
| ECHR-T-119 | Spawn points | fallback pose is explicit | Planned | Not run |
| ECHR-T-120 | Spawn points | point diagnostics are redacted | Planned | Not run |
| ECHR-T-121 | Control owners | ControlOwnerId registers | Planned | Not run |
| ECHR-T-122 | Control owners | owner metadata is optional | Planned | Not run |
| ECHR-T-123 | Control owners | duplicate owner registration is rejected | Planned | Not run |
| ECHR-T-124 | Control owners | owner removal releases assignments | Planned | Not run |
| ECHR-T-125 | Control owners | owner session identity is not durable by default | Planned | Not run |
| ECHR-T-126 | Control owners | owner lookup is bounded | Planned | Not run |
| ECHR-T-127 | Control owners | owner events follow commit | Planned | Not run |
| ECHR-T-128 | Control owners | unknown owner request is rejected | Planned | Not run |
| ECHR-T-129 | Control assignment | exclusive assignment succeeds | Planned | Not run |
| ECHR-T-130 | Control assignment | unspawned target is denied by default | Planned | Not run |
| ECHR-T-131 | Control assignment | already controlled target follows policy | Planned | Not run |
| ECHR-T-132 | Control assignment | owner transfer is atomic | Planned | Not run |
| ECHR-T-133 | Control assignment | character transfer is atomic | Planned | Not run |
| ECHR-T-134 | Control assignment | control lease is generational | Planned | Not run |
| ECHR-T-135 | Control assignment | stale lease is rejected | Planned | Not run |
| ECHR-T-136 | Control assignment | assignment snapshot is immutable | Planned | Not run |
| ECHR-T-137 | Handoff participants | participant registers | Planned | Not run |
| ECHR-T-138 | Handoff participants | prepare succeeds | Planned | Not run |
| ECHR-T-139 | Handoff participants | prepare denial prevents commit | Planned | Not run |
| ECHR-T-140 | Handoff participants | prepare timeout prevents commit | Planned | Not run |
| ECHR-T-141 | Handoff participants | commit callback follows authority change | Planned | Not run |
| ECHR-T-142 | Handoff participants | post-commit failure is diagnosed | Planned | Not run |
| ECHR-T-143 | Handoff participants | participant removal is safe | Planned | Not run |
| ECHR-T-144 | Handoff participants | callback order is deterministic | Planned | Not run |
| ECHR-T-145 | Switch orchestration | spawned target switch succeeds | Planned | Not run |
| ECHR-T-146 | Switch orchestration | spawn-then-transfer succeeds | Planned | Not run |
| ECHR-T-147 | Switch orchestration | spawn failure preserves old control | Planned | Not run |
| ECHR-T-148 | Switch orchestration | keep-old-spawned policy works | Planned | Not run |
| ECHR-T-149 | Switch orchestration | despawn-old-after-commit works | Planned | Not run |
| ECHR-T-150 | Switch orchestration | selection and control commit once | Planned | Not run |
| ECHR-T-151 | Switch orchestration | rapid requests follow admission policy | Planned | Not run |
| ECHR-T-152 | Switch orchestration | switch cancellation boundary is honored | Planned | Not run |
| ECHR-T-153 | Replacement and respawn | replacement preserves old history | Planned | Not run |
| ECHR-T-154 | Replacement and respawn | replacement validates substitute | Planned | Not run |
| ECHR-T-155 | Replacement and respawn | respawn creates new RuntimeInstanceId | Planned | Not run |
| ECHR-T-156 | Replacement and respawn | respawn uses same CharacterId | Planned | Not run |
| ECHR-T-157 | Replacement and respawn | respawn point failure is safe | Planned | Not run |
| ECHR-T-158 | Replacement and respawn | respawn concurrency is bounded | Planned | Not run |
| ECHR-T-159 | Replacement and respawn | replacement events are ordered | Planned | Not run |
| ECHR-T-160 | Replacement and respawn | defeated status remains project controlled | Planned | Not run |
| ECHR-T-161 | Runtime actor binding | actor reports CharacterId | Planned | Not run |
| ECHR-T-162 | Runtime actor binding | actor reports RuntimeInstanceId | Planned | Not run |
| ECHR-T-163 | Runtime actor binding | binding rejects mismatched identity | Planned | Not run |
| ECHR-T-164 | Runtime actor binding | binding exposes GameObject safely | Planned | Not run |
| ECHR-T-165 | Runtime actor binding | binding teardown is idempotent | Planned | Not run |
| ECHR-T-166 | Runtime actor binding | actor capability bridge is optional | Planned | Not run |
| ECHR-T-167 | Runtime actor binding | missing binding is diagnosed | Planned | Not run |
| ECHR-T-168 | Runtime actor binding | actor does not own roster truth | Planned | Not run |
| ECHR-T-169 | Snapshots | roster snapshot is detached | Planned | Not run |
| ECHR-T-170 | Snapshots | selection snapshot is detached | Planned | Not run |
| ECHR-T-171 | Snapshots | control snapshot excludes live handles | Planned | Not run |
| ECHR-T-172 | Snapshots | spawn snapshot excludes GameObjects | Planned | Not run |
| ECHR-T-173 | Snapshots | snapshot revisions are monotonic | Planned | Not run |
| ECHR-T-174 | Snapshots | snapshot collections are read-only | Planned | Not run |
| ECHR-T-175 | Snapshots | unknown records are preserved | Planned | Not run |
| ECHR-T-176 | Snapshots | snapshot hashing is deterministic | Planned | Not run |
| ECHR-T-177 | Export | valid export succeeds | Planned | Not run |
| ECHR-T-178 | Export | export uses schema version | Planned | Not run |
| ECHR-T-179 | Export | export excludes session runtime IDs where specified | Planned | Not run |
| ECHR-T-180 | Export | export preserves CharacterIds | Planned | Not run |
| ECHR-T-181 | Export | export preserves groups | Planned | Not run |
| ECHR-T-182 | Export | export preserves availability | Planned | Not run |
| ECHR-T-183 | Export | export preserves extension records | Planned | Not run |
| ECHR-T-184 | Export | export failure does not mutate state | Planned | Not run |
| ECHR-T-185 | Import | valid import prepares | Planned | Not run |
| ECHR-T-186 | Import | validation precedes apply | Planned | Not run |
| ECHR-T-187 | Import | apply is atomic | Planned | Not run |
| ECHR-T-188 | Import | unknown definitions are preserved | Planned | Not run |
| ECHR-T-189 | Import | unknown extensions are preserved | Planned | Not run |
| ECHR-T-190 | Import | newer unsupported version is rejected | Planned | Not run |
| ECHR-T-191 | Import | older version migrates | Planned | Not run |
| ECHR-T-192 | Import | failed import leaves current state | Planned | Not run |
| ECHR-T-193 | Migration | contiguous migration chain works | Planned | Not run |
| ECHR-T-194 | Migration | missing migration blocks | Planned | Not run |
| ECHR-T-195 | Migration | alias mapping works | Planned | Not run |
| ECHR-T-196 | Migration | CharacterId remains stable | Planned | Not run |
| ECHR-T-197 | Migration | selection context migration works | Planned | Not run |
| ECHR-T-198 | Migration | group migration works | Planned | Not run |
| ECHR-T-199 | Migration | extension migration is provider-owned | Planned | Not run |
| ECHR-T-200 | Migration | source snapshot is preserved on failure | Planned | Not run |
| ECHR-T-201 | Chronicle bridge | bridge registers participant | Planned | Not run |
| ECHR-T-202 | Chronicle bridge | capture uses detached snapshot | Planned | Not run |
| ECHR-T-203 | Chronicle bridge | load prepares before apply | Planned | Not run |
| ECHR-T-204 | Chronicle bridge | scene travel is not owned | Planned | Not run |
| ECHR-T-205 | Chronicle bridge | live control is not restored | Planned | Not run |
| ECHR-T-206 | Chronicle bridge | bridge removal preserves payload | Planned | Not run |
| ECHR-T-207 | Chronicle bridge | version mismatch is explicit | Planned | Not run |
| ECHR-T-208 | Chronicle bridge | Chronicle remains save authority | Planned | Not run |
| ECHR-T-209 | Progression bridge | unlock maps to availability | Planned | Not run |
| ECHR-T-210 | Progression bridge | locked maps without deleting member | Planned | Not run |
| ECHR-T-211 | Progression bridge | missing progression node is unavailable | Planned | Not run |
| ECHR-T-212 | Progression bridge | bridge removal preserves roster state | Planned | Not run |
| ECHR-T-213 | Progression bridge | progression event does not duplicate | Planned | Not run |
| ECHR-T-214 | Progression bridge | manual availability override policy is explicit | Planned | Not run |
| ECHR-T-215 | Progression bridge | identity mapping uses stable IDs | Planned | Not run |
| ECHR-T-216 | Progression bridge | Progression remains unlock authority | Planned | Not run |
| ECHR-T-217 | Controller bridge | control maps to controller target | Planned | Not run |
| ECHR-T-218 | Controller bridge | release disables prior controller safely | Planned | Not run |
| ECHR-T-219 | Controller bridge | prepare failure blocks transfer | Planned | Not run |
| ECHR-T-220 | Controller bridge | stale handoff is rejected | Planned | Not run |
| ECHR-T-221 | Controller bridge | bridge removal leaves roster usable | Planned | Not run |
| ECHR-T-222 | Controller bridge | custom controllers work without EchoControllers | Planned | Not run |
| ECHR-T-223 | Controller bridge | movement remains outside Fellowship | Planned | Not run |
| ECHR-T-224 | Controller bridge | integration lab declares both packages | Planned | Not run |
| ECHR-T-225 | Input bridge | local user maps to ControlOwnerId | Planned | Not run |
| ECHR-T-226 | Input bridge | device change does not alter roster automatically | Planned | Not run |
| ECHR-T-227 | Input bridge | unpaired user is unavailable | Planned | Not run |
| ECHR-T-228 | Input bridge | bridge removal releases mapping | Planned | Not run |
| ECHR-T-229 | Input bridge | input context remains The Will authority | Planned | Not run |
| ECHR-T-230 | Input bridge | multiple owners remain bounded | Planned | Not run |
| ECHR-T-231 | Input bridge | selection command is semantic | Planned | Not run |
| ECHR-T-232 | Input bridge | raw device data is not stored in roster | Planned | Not run |
| ECHR-T-233 | Camera bridge | active character registers camera target | Planned | Not run |
| ECHR-T-234 | Camera bridge | target changes after control commit | Planned | Not run |
| ECHR-T-235 | Camera bridge | warp revision is forwarded | Planned | Not run |
| ECHR-T-236 | Camera bridge | camera failure does not rollback roster truth | Planned | Not run |
| ECHR-T-237 | Camera bridge | bridge removal leaves control intact | Planned | Not run |
| ECHR-T-238 | Camera bridge | group target mapping is optional | Planned | Not run |
| ECHR-T-239 | Camera bridge | camera remains final view authority | Planned | Not run |
| ECHR-T-240 | Camera bridge | integration lab declares dependencies | Planned | Not run |
| ECHR-T-241 | UI bridge | roster snapshot presents | Planned | Not run |
| ECHR-T-242 | UI bridge | selection request routes to service | Planned | Not run |
| ECHR-T-243 | UI bridge | stale UI revision is rejected | Planned | Not run |
| ECHR-T-244 | UI bridge | display names remain project content | Planned | Not run |
| ECHR-T-245 | UI bridge | UI failure does not block mutation | Planned | Not run |
| ECHR-T-246 | UI bridge | modal ownership remains EchoUI | Planned | Not run |
| ECHR-T-247 | UI bridge | focus remains EchoUI authority | Planned | Not run |
| ECHR-T-248 | UI bridge | bridge removal leaves core functional | Planned | Not run |
| ECHR-T-249 | Inventory bridge | CharacterId maps to container ID | Planned | Not run |
| ECHR-T-250 | Inventory bridge | equipment container ownership maps | Planned | Not run |
| ECHR-T-251 | Inventory bridge | inventory failure does not alter roster identity | Planned | Not run |
| ECHR-T-252 | Inventory bridge | bridge removal preserves container payload | Planned | Not run |
| ECHR-T-253 | Inventory bridge | duplicate mapping is diagnosed | Planned | Not run |
| ECHR-T-254 | Inventory bridge | unique IDs remain separate | Planned | Not run |
| ECHR-T-255 | Inventory bridge | inventory remains item authority | Planned | Not run |
| ECHR-T-256 | Inventory bridge | integration lab declares dependencies | Planned | Not run |
| ECHR-T-257 | Interaction bridge | controlled character maps to interactor | Planned | Not run |
| ECHR-T-258 | Interaction bridge | owner change updates metadata | Planned | Not run |
| ECHR-T-259 | Interaction bridge | stale mapping is rejected | Planned | Not run |
| ECHR-T-260 | Interaction bridge | interaction remains request authority | Planned | Not run |
| ECHR-T-261 | Interaction bridge | bridge removal leaves actor spawned | Planned | Not run |
| ECHR-T-262 | Interaction bridge | multiple interactors policy is explicit | Planned | Not run |
| ECHR-T-263 | Interaction bridge | CharacterId is used instead of display name | Planned | Not run |
| ECHR-T-264 | Interaction bridge | integration failure is diagnosed | Planned | Not run |
| ECHR-T-265 | Multiplayer seams | authority provider is optional | Planned | Not run |
| ECHR-T-266 | Multiplayer seams | network owner maps to ControlOwnerId | Planned | Not run |
| ECHR-T-267 | Multiplayer seams | server rejection is structured | Planned | Not run |
| ECHR-T-268 | Multiplayer seams | client prediction is not implied | Planned | Not run |
| ECHR-T-269 | Multiplayer seams | spawn provider may be network-backed | Planned | Not run |
| ECHR-T-270 | Multiplayer seams | disconnect releases assignment by policy | Planned | Not run |
| ECHR-T-271 | Multiplayer seams | late join snapshot is provider-owned | Planned | Not run |
| ECHR-T-272 | Multiplayer seams | core remains network-agnostic | Planned | Not run |
| ECHR-T-273 | Diagnostics | initialization state is exposed | Planned | Not run |
| ECHR-T-274 | Diagnostics | root identity is exposed | Planned | Not run |
| ECHR-T-275 | Diagnostics | roster counts are exposed | Planned | Not run |
| ECHR-T-276 | Diagnostics | spawn counts are exposed | Planned | Not run |
| ECHR-T-277 | Diagnostics | control assignments are exposed | Planned | Not run |
| ECHR-T-278 | Diagnostics | recent failures are bounded | Planned | Not run |
| ECHR-T-279 | Diagnostics | display names are redacted by default | Planned | Not run |
| ECHR-T-280 | Diagnostics | no per-frame spam occurs | Planned | Not run |
| ECHR-T-281 | Diagnostic codes | duplicate root code is stable | Planned | Not run |
| ECHR-T-282 | Diagnostic codes | missing configuration code is stable | Planned | Not run |
| ECHR-T-283 | Diagnostic codes | ID collision code is stable | Planned | Not run |
| ECHR-T-284 | Diagnostic codes | spawn failure code is stable | Planned | Not run |
| ECHR-T-285 | Diagnostic codes | control failure code is stable | Planned | Not run |
| ECHR-T-286 | Diagnostic codes | stale handle code is stable | Planned | Not run |
| ECHR-T-287 | Diagnostic codes | import failure code is stable | Planned | Not run |
| ECHR-T-288 | Diagnostic codes | codes remain searchable | Planned | Not run |
| ECHR-T-289 | Editor setup | setup preview is deterministic | Planned | Not run |
| ECHR-T-290 | Editor setup | create assets is create-only safe | Planned | Not run |
| ECHR-T-291 | Editor setup | repeat setup is idempotent | Planned | Not run |
| ECHR-T-292 | Editor setup | existing assets are adopted explicitly | Planned | Not run |
| ECHR-T-293 | Editor setup | destructive repair requires confirmation | Planned | Not run |
| ECHR-T-294 | Editor setup | report lists exact changes | Planned | Not run |
| ECHR-T-295 | Editor setup | Undo or backup is provided where practical | Planned | Not run |
| ECHR-T-296 | Editor setup | Workshop facade matches ADR-001 | Planned | Not run |
| ECHR-T-297 | Validation | definition validation runs | Planned | Not run |
| ECHR-T-298 | Validation | roster validation runs | Planned | Not run |
| ECHR-T-299 | Validation | prefab validation runs | Planned | Not run |
| ECHR-T-300 | Validation | spawn point validation runs | Planned | Not run |
| ECHR-T-301 | Validation | alias validation runs | Planned | Not run |
| ECHR-T-302 | Validation | group validation runs | Planned | Not run |
| ECHR-T-303 | Validation | integration absence is advisory | Planned | Not run |
| ECHR-T-304 | Validation | blockers prevent release | Planned | Not run |
| ECHR-T-305 | Direct scene testing | development initializer is dev-only | Planned | Not run |
| ECHR-T-306 | Direct scene testing | canonical root is adopted | Planned | Not run |
| ECHR-T-307 | Direct scene testing | sample roster creates once | Planned | Not run |
| ECHR-T-308 | Direct scene testing | duplicate authority is avoided | Planned | Not run |
| ECHR-T-309 | Direct scene testing | release build exclusion is validated | Planned | Not run |
| ECHR-T-310 | Direct scene testing | reset returns deterministic state | Planned | Not run |
| ECHR-T-311 | Direct scene testing | no peer package is required | Planned | Not run |
| ECHR-T-312 | Direct scene testing | direct scene status is visible | Planned | Not run |
| ECHR-T-313 | Laboratory | Laboratory imports independently | Planned | Not run |
| ECHR-T-314 | Laboratory | instructions are visible | Planned | Not run |
| ECHR-T-315 | Laboratory | all cases reset | Planned | Not run |
| ECHR-T-316 | Laboratory | fake providers are deterministic | Planned | Not run |
| ECHR-T-317 | Laboratory | sample assets are redistributable | Planned | Not run |
| ECHR-T-318 | Laboratory | sample removal is safe | Planned | Not run |
| ECHR-T-319 | Laboratory | success and failure cases exist | Planned | Not run |
| ECHR-T-320 | Laboratory | Laboratory is not production dependency | Planned | Not run |
| ECHR-T-321 | Performance | roster query stays within target | Planned | Not run |
| ECHR-T-322 | Performance | selection stays allocation-bounded | Planned | Not run |
| ECHR-T-323 | Performance | snapshot creation is measured | Planned | Not run |
| ECHR-T-324 | Performance | spawn concurrency is bounded | Planned | Not run |
| ECHR-T-325 | Performance | events avoid unbounded history | Planned | Not run |
| ECHR-T-326 | Performance | no per-frame roster scanning is required | Planned | Not run |
| ECHR-T-327 | Performance | diagnostics sampling is bounded | Planned | Not run |
| ECHR-T-328 | Performance | stress test records empirical evidence | Planned | Not run |
| ECHR-T-329 | Scalability | default roster limit is enforced | Planned | Not run |
| ECHR-T-330 | Scalability | group limit is enforced | Planned | Not run |
| ECHR-T-331 | Scalability | owner limit is enforced | Planned | Not run |
| ECHR-T-332 | Scalability | spawn request limit is enforced | Planned | Not run |
| ECHR-T-333 | Scalability | history limit is enforced | Planned | Not run |
| ECHR-T-334 | Scalability | overflow fails gracefully | Planned | Not run |
| ECHR-T-335 | Scalability | limits are configurable within safe bounds | Planned | Not run |
| ECHR-T-336 | Scalability | advertised limits match tests | Planned | Not run |
| ECHR-T-337 | Scene and reload | scene unload preserves durable roster | Planned | Not run |
| ECHR-T-338 | Scene and reload | scene actors reconcile | Planned | Not run |
| ECHR-T-339 | Scene and reload | domain reload resets statics | Planned | Not run |
| ECHR-T-340 | Scene and reload | Enter Play Mode options are tested | Planned | Not run |
| ECHR-T-341 | Scene and reload | event subscriptions do not duplicate | Planned | Not run |
| ECHR-T-342 | Scene and reload | direct scene helper cleans up | Planned | Not run |
| ECHR-T-343 | Scene and reload | shutdown cancels pending work | Planned | Not run |
| ECHR-T-344 | Scene and reload | persistent root survives configured transitions | Planned | Not run |
| ECHR-T-345 | Security and privacy | untrusted snapshot sizes are bounded | Planned | Not run |
| ECHR-T-346 | Security and privacy | unknown payloads are opaque | Planned | Not run |
| ECHR-T-347 | Security and privacy | diagnostics omit private names | Planned | Not run |
| ECHR-T-348 | Security and privacy | paths are redacted | Planned | Not run |
| ECHR-T-349 | Security and privacy | provider exceptions are isolated | Planned | Not run |
| ECHR-T-350 | Security and privacy | malformed IDs are rejected | Planned | Not run |
| ECHR-T-351 | Security and privacy | external metadata is validated | Planned | Not run |
| ECHR-T-352 | Security and privacy | no credentials are stored | Planned | Not run |
| ECHR-T-353 | Platform | Windows plan is recorded | Planned | Not run |
| ECHR-T-354 | Platform | macOS plan is recorded | Planned | Not run |
| ECHR-T-355 | Platform | Linux plan is recorded | Planned | Not run |
| ECHR-T-356 | Platform | WebGL plan is recorded | Planned | Not run |
| ECHR-T-357 | Platform | mobile plan is recorded | Planned | Not run |
| ECHR-T-358 | Platform | console status is Unknown | Planned | Not run |
| ECHR-T-359 | Platform | platform claims remain Not run | Planned | Not run |
| ECHR-T-360 | Platform | unsupported behavior fails clearly | Planned | Not run |
| ECHR-T-361 | Removal | sample removal is safe | Planned | Not run |
| ECHR-T-362 | Removal | bridge-first removal is documented | Planned | Not run |
| ECHR-T-363 | Removal | core removal leaves project assets | Planned | Not run |
| ECHR-T-364 | Removal | reinstall resolves known IDs | Planned | Not run |
| ECHR-T-365 | Removal | unknown payloads survive peer removal | Planned | Not run |
| ECHR-T-366 | Removal | generated assets are not overwritten | Planned | Not run |
| ECHR-T-367 | Removal | diagnostics identify orphaned references | Planned | Not run |
| ECHR-T-368 | Removal | removal guide is complete | Planned | Not run |
| ECHR-T-369 | Release gates | specification gate passes | Planned | Not run |
| ECHR-T-370 | Release gates | implementation gate is defined | Planned | Not run |
| ECHR-T-371 | Release gates | standalone gate is defined | Planned | Not run |
| ECHR-T-372 | Release gates | quality gate is defined | Planned | Not run |
| ECHR-T-373 | Release gates | beta gate is defined | Planned | Not run |
| ECHR-T-374 | Release gates | release candidate gate is defined | Planned | Not run |
| ECHR-T-375 | Release gates | stable gate is defined | Planned | Not run |
| ECHR-T-376 | Release gates | distribution gate is defined | Planned | Not run |
| ECHR-T-377 | Documentation | README routes correctly | Planned | Not run |
| ECHR-T-378 | Documentation | quick start is planned | Planned | Not run |
| ECHR-T-379 | Documentation | API guide is planned | Planned | Not run |
| ECHR-T-380 | Documentation | architecture guide is planned | Planned | Not run |
| ECHR-T-381 | Documentation | diagnostic reference is planned | Planned | Not run |
| ECHR-T-382 | Documentation | migration guide is planned | Planned | Not run |
| ECHR-T-383 | Documentation | Current Notes link exists | Planned | Not run |
| ECHR-T-384 | Documentation | examples must compile before release | Planned | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Authority/non-ownership approved.
- [x] Identity model approved.
- [x] MVP/deferred scope separated.
- [x] Public API/data/lifecycle/failure contracts defined.
- [x] Laboratory designed.
- [x] Release-blocking questions resolved for pre-code foundation.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared dependencies only.
- [ ] Editor code isolated.
- [ ] Duplicate protection occurs before side effects.
- [ ] Rosters, IDs, handles, switching, providers, and snapshots match specification.
- [ ] Setup/repair repeat safely.
- [ ] Any architecture change updates specification/ADR first.

### 24.3 Standalone gate

- [ ] Clean install succeeds.
- [ ] Core works without peers.
- [ ] Fellowship Laboratory passes.
- [ ] Sample removal is safe.
- [ ] Direct-scene behavior matches documentation.

### 24.4 Quality gate

- [ ] Automated tests pass.
- [ ] Manual scenarios pass.
- [ ] No Blocker/Critical issue remains.
- [ ] Performance/scalability evidence passes.
- [ ] Diagnostics actionable and privacy-safe.
- [ ] Current Notes reconciled.
- [ ] Documentation matches build.
- [ ] Licenses/notices complete.

### 24.5 Distribution gate

- [ ] Manifest valid.
- [ ] Version/changelog updated.
- [ ] Stable `.meta` files included.
- [ ] Git/tarball installation tested externally.
- [ ] Beta gate passes before beta claim.
- [ ] Release-candidate gate passes before RC claim.
- [ ] Stable gate passes before stable claim.
- [ ] Compatibility catalog updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Rescuers2D | ActiveCharacter enum, PlayerInputReader routing, role controllers/prefabs | Define durable members, bridge control handoff one role at a time, preserve old switching until parity | Switch all roles, direct-scene tests, camera/input handoff, save semantics if used | Disable bridge and restore existing input routing |
| Hackulos | Planned player/party/pet character management | Introduce definitions/roster before gameplay systems depend on project-specific identity | Player spawn/select/respawn and companion/pet identity proof | Keep project-local registry until parity |
| Echo Systems Lab | Project-specific actors/mission references | Adopt only where reusable roster/control ownership adds value | Isolated package + one project integration | Remove adapter; project code remains |

### 25.2 Preserve-until-parity rule

Existing working code remains available. Install/validate in isolation, map IDs without deleting old data, bridge one selection/control path, compare behavior, migrate snapshots only after tests, and remove old authority only after reversible parity evidence.

### 25.3 Migration tooling

Planned tooling detects existing prefabs/role enums/registries, previews definitions and stable IDs, generates project-owned assets create-only, records mapping aliases, validates prefab bindings, exports a migration report, and never edits gameplay controllers automatically without explicit project-specific tooling and backup.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| ECHR-R-001 | Scope grows into universal character framework | High | High | Enforce authority boundary and bridges | Any stats/combat/animation proposal |
| ECHR-R-002 | Definition/Character/runtime IDs become confused | Medium | Critical | Three-layer identity model, validators, tests | Any serialization/API ambiguity |
| ECHR-R-003 | Switching commits before target/provider readiness | Medium | High | Prepare phase and explicit commit point | Switch implementation review |
| ECHR-R-004 | Post-commit controller bridge failure leaves no usable control | Medium | High | Participant diagnostics/recovery contract and Integration Lab | Bridge implementation |
| ECHR-R-005 | Scene unload leaves stale runtime actors/leases | Medium | High | Reconciliation and scene lifecycle tests | PlayMode stress |
| ECHR-R-006 | Save import restores transient ownership incorrectly | Medium | High | Snapshot excludes live leases/runtime actors | Chronicle bridge review |
| ECHR-R-007 | Availability becomes genre-locked enum | Medium | Medium | Core disposition plus stable project status IDs | New status proposal |
| ECHR-R-008 | Multiple local players expose single-selection assumptions | Medium | High | Selection contexts and ControlOwnerId separation | Local multiplayer Lab |
| ECHR-R-009 | Network provider leaks into core | Medium | High | SFGSS-002 adapter boundary | Convergence work |
| ECHR-R-010 | Character prefabs carry hidden controller/input/camera requirements | High | Medium | Prefab validation and standalone sample actors | Setup validation |
| ECHR-R-011 | Diagnostics expose player-created names/metadata | Low | High | Redaction by default | Support export review |
| ECHR-R-012 | Snapshot extension records are lost when providers absent | Medium | High | Opaque preservation per SFGSS-003 | Migration tests |
| ECHR-R-013 | Safe limits are either too restrictive or unbounded | Medium | Medium | Configurable bounded defaults and measured evidence | Stress testing |
| ECHR-R-014 | Existing projects regress during adoption | Medium | High | Preserve-until-parity and reversible adapters | Integration checkpoint |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequence | ADR? |
|---|---|---|---|---|---:|
| ECHR-D-001 | Definition, durable character, and runtime instance IDs are separate | Approved | Respawn/duplicates/save safety | Three stable identity types | No |
| ECHR-D-002 | Roster membership and availability are separate truths | Approved | Preserve history and project statuses | Unavailable members remain in roster | No |
| ECHR-D-003 | Availability uses core disposition plus stable status IDs | Approved | Avoid genre-locked enum | Project statuses extend safely | No |
| ECHR-D-004 | Selection contexts are independent from control owners | Approved | UI/local multiplayer flexibility | Multiple selections without possession | No |
| ECHR-D-005 | MVP control ownership is exclusive | Approved | Clear safe authority | Shared possession deferred | No |
| ECHR-D-006 | Spawn does not imply selection or control | Approved | Keep concerns explicit | Orchestration policy required |
| ECHR-D-007 | Switch prepares spawn/handoff before commit | Approved | Prevent half-switch state | Async staged transaction |
| ECHR-D-008 | Live control leases/runtime actors are not saved | Approved | Scene/provider safety | Project explicitly rebinds after load | No |
| ECHR-D-009 | Built-in prefab provider proves independence | Approved | Standalone usefulness | Network/pool providers remain optional |
| ECHR-D-010 | Post-commit bridge failures do not rewrite core truth silently | Approved | One authority per concern | Explicit recovery diagnostics | No |

### 27.2 Release-blocking questions

None for the pre-code package foundation. Empirical Unity compatibility, safe performance limits, exact async implementation details, and bridge behavior remain implementation evidence, not architecture blockers.

### 27.3 Non-blocking later questions

- Whether shared possession belongs in core or only multiplayer adapters.
- Whether Addressables definition/prefab providers should be first-party.
- Whether pooled character actors can meet strict reset contracts.
- Whether roster graph visualization provides enough value for a later Editor module.
- Which exact multiplayer provider semantics map into ControlOwnerId after research.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 | Approved specification | Design only | This document |
| M1 | Installable skeleton | Manifest, asmdefs, docs shell | Clean compile/install |
| M2 | Identity and roster core | IDs, definitions, rosters, availability, selection/groups | Unit tests |
| M3 | Spawn/runtime registry | Built-in provider, handles, lifecycle | PlayMode tests |
| M4 | Control/switch core | Owners, participants, switching, respawn | PlayMode tests |
| M5 | Laboratory/tooling | Setup, validation, 64 scenarios | Manual/automated evidence |
| M6 | First integration | Rescuers2D or Hackulos reversible adoption | Parity report |
| M7 | Distribution | Docs, licenses, package artifact | External install/release gates |

### 28.2 Checkpoint rule

Every milestone is divided using SFGSS-005. Code is shown in full with exact paths and teaching explanations after SUITE-DOC-33. Each checkpoint stops at a compile/test boundary and reconciles documentation.

### 28.3 First recommended checkpoint

After the final suite documentation gate authorizes implementation: **ECHR-M1-01 - Fellowship Package Skeleton**, limited to package anatomy, asmdefs, documentation shell, and installation evidence. No runtime C# is authorized by this specification alone.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.
Treat SFGSS-000 and SFGSS-002 through SFGSS-005 as suite authorities.
Treat The Fellowship (`EchoCharacters`) Package Specification v1.0.0 as the
Level 2 authority for character identity, rosters, availability, selection,
groups, spawning, runtime actor identity, control ownership, switching,
snapshots, diagnostics, tooling, Laboratories, and bridges.

Package implementation remains locked until SUITE-DOC-33.
Current next documentation checkpoint: SUITE-DOC-16 - The Vessel (`EchoControllers`).
Do not absorb movement, combat, animation, camera, input, inventory, save-file,
scene-flow, or multiplayer provider authority into EchoCharacters.
Keep all empirical evidence Not run.
When implementation eventually begins, show complete files and explain each step.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | 1.0.0 |
| Completed checkpoint | SUITE-DOC-15 - Fellowship specification |
| Files/assets created | Documentation only |
| Tests passed | Documentation structure/integrity only; runtime tests Not run |
| Tests failed | None executed |
| Known issues | Empirical compatibility/performance and bridge evidence pending |
| Decisions added | ECHR-D-001 through ECHR-D-010 |
| Next checkpoint | SUITE-DOC-16 - The Vessel (`EchoControllers`) specification |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and responsibility are clear.
- [x] Ownership aligns with SFGSS-000.
- [x] Independence proof is credible.
- [x] Definition/Character/runtime identities are distinct.
- [x] Membership, availability, selection, spawn, and control are distinct.
- [x] MVP is useful and bounded.
- [x] Public API, lifecycle, failure, snapshot, and migration contracts are defined.
- [x] Setup/direct-scene workflows are understandable.
- [x] Standalone Laboratory is fully defined.
- [x] Diagnostics do not require The Observatory.
- [x] Optional integrations are explicit and removable.
- [x] Tests and release gates are measurable and Not run where empirical.
- [x] No Isekai Studios identity or ownership was introduced.
- [x] Jesse has approved continuation of the package-first documentation program.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Package implementation remains locked until SUITE-DOC-33. All empirical evidence remains Not run until executed.


---


## SUITE-DOC-30 Consistency Addendum

**Review status:** Passed  
**Review date:** August 4, 2026  
**Current governing authorities:** SFGSS-000 v0.20.0; SFGSS-001 v1.2.0; SFGSS-002 v1.1.0; SFGSS-003 v1.1.0; SFGSS-004 v1.2.0; SFGSS-005 v1.2.0; SFGSS-006 through SFGSS-010; SFGSS-ADR-001 through SFGSS-ADR-003; and the approved Foundation, Expansion, and Advanced integration matrices.

The original parent-authority header remains approval provenance. This addendum records the standards that govern the specification after the full consistency review.

- The formal public title, technical identifier, package ID, namespace family, document ID, diagnostic/test prefix, setup facade, and planned repository were checked against SFGSS-008 and SFGSS-009.
- All implementation, compatibility, platform, performance, migration, Laboratory, provider, and release evidence remains `Not run` unless a retained execution record says otherwise.
- Package-qualified test and Laboratory IDs are authoritative. Pre-code range tables are planning shorthand only; implementation registries must expand them into individual definitions with separate automation class, execution status, evidence reference, and issue reference fields.
- A platform cell written as `Yes` in an older pre-code table means **planned design support**, not `Tested` or `Supported`, until SFGSS-004 evidence exists.
- Primary public Runtime assemblies may remain `autoReferenced: true`; Editor, test, sample, internal support, bridge, and provider assemblies default to `false` under SFGSS-002 unless this specification explicitly records a justified exception.
- Current Notes captures future discoveries, but durable changes return to this specification or an ADR before implementation advances.

## Graph Navigation

#sfgss/package #sfgss/wave/expansion #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
