# The Eye – Camera Direction Package Specification

**Working document ID:** SFGSS-PKG-ECHOCAMERA-001  
**Specification version:** 1.0.1
**Status:** Approved  
**Technical package name:** EchoCamera  
**Public title:** The Eye – Camera Direction
**Package ID:** `com.echodevgames.echo-camera`  
**Runtime namespace:** `EchoDevGames.EchoCamera`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoCamera`
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Last updated:** August 4, 2026

> “Choose what the player sees, then let every lens move with purpose.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoCamera. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-005 and approved package authorities through The Hand | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved channel, target, group, mode, modifier, bounds, zone, blend, backend, direct-scene, diagnostics, tooling, Laboratory, bridge, and release contracts | Jesse “Echo” Adams |
| 1.0.1 | 2026-08-04 | Approved | Normalized registry metadata and formal title; added the SUITE-DOC-30 governing-authority, evidence, test-registry, and compatibility clarification without authorizing implementation. | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Eye – Camera Direction
**Technical identifier:** EchoCamera  
**Flavor line:** Choose what the player sees, then let every lens move with purpose.  
**Plain-language subtitle:** A standalone Unity package for camera channels, target registration and grouping, priority-based modes, blends, modifiers, bounds, zones, viewports, provider backends, diagnostics, authoring, validation, and explicit integration seams.

**One-sentence ownership contract:**

> EchoCamera owns camera-channel authority, target and target-group registration, provider-neutral camera modes, priority leases, blends, offsets, look-ahead, dead zones, zoom and lens intents, bounded impulse requests, bounds and zone arbitration, backend capability negotiation, built-in Unity Camera execution, diagnostics, authoring, validation, isolated 2D/3D Laboratories, and explicit bridge seams; it does not own player movement, character identity, level layout, rendering-pipeline configuration, post-processing, gameplay events, feedback recipes, dialogue truth, cutscene sequencing, input devices, global preference storage, scene loading, production UI, multiplayer player assignment, or one mandatory camera backend.

### 1.1 Elevator summary

The Eye provides one readable authority for **camera intent** while allowing the project to select the technology that performs it. Gameplay and neighboring packages request a mode, target, group, bounds profile, lens change, modifier, viewport, or transient impulse. EchoCamera validates those requests, resolves priority and lifetime through generational leases, produces one effective channel state, and hands that state to the selected backend. The backend moves the actual Unity Camera or maps the intent into another camera system.

The package is built around **camera channels**. A channel represents one independently evaluated camera output such as Main, PlayerOne, PlayerTwo, Spectator, or an authored secondary view. Each channel has one baseline mode, zero or more temporary mode contenders, a selected backend, target/group state, modifiers, bounds, zones, lens intent, viewport metadata, blend state, and diagnostics. The MVP supports one Main channel directly and keeps multiple bounded channels in the public model so split-screen and multiplayer adapters do not require a breaking redesign.

The neutral core does not require Cinemachine. A built-in Unity Camera backend proves the package can operate alone. A separate `com.echodevgames.echo-camera.cinemachine` provider adapter may map the same modes, targets, groups, blends, and bounds into Cinemachine 3 without placing `Unity.Cinemachine` types in core assemblies. The project may also supply a custom backend. EchoCamera owns the winning intent and lifecycle; the backend owns the technical execution details it explicitly advertises.

### 1.2 Why this belongs in The Sperk's Forge

Camera logic appears in nearly every project and quickly accumulates direct references to players, dialogue, combat, rooms, confiners, UI, inputs, and scene scripts. Temporary modes are commonly implemented by enabling one camera, disabling another, changing priorities, or editing transform offsets from several unrelated systems. When a target is destroyed, a scene unloads, a pause occurs, or two zones overlap, the camera becomes one of the first systems to expose hidden ownership problems.

Rescuers2D needs reliable character switching, room bounds, direct-scene testing, camera feedback, and future multi-role framing. Don't Get Vince'd needs beat-'em-up framing, boss and hit-response shots, and zone behavior. Hackulos needs top-down follow, click-to-move framing, dialogue shots, group/companion targets, and future world transitions. Echo Systems Lab already demonstrates target registration and system separation. The Eye captures the reusable camera infrastructure while leaving each game's movement, art, level design, cinematography, and presentation choices in the project or explicit adapters.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Always paired with “Camera Direction.” |
| Setup guidance/tooltips | Yes | Must remain immediately understandable. |
| Samples | Optional | Eye/observatory imagery may decorate Laboratories but is removable. |
| Runtime API/type names | No lore-only names | Types use `CameraChannel`, `CameraModeRequest`, `CameraTargetHandle`, and similar technical names. |
| Project data | No required Verse content | Games own mode names, shot language, targets, bounds, zones, camera art, and backend mappings. |

---

## 2. Problem Statement

### 2.1 Current problem

Projects repeatedly need target following, group framing, temporary modes, dialogue shots, aiming offsets, room bounds, camera zones, zoom, smooth blends, target-loss handling, split-screen hooks, and feedback impulses. Without one authority, these behaviors are scattered across player controllers, trigger volumes, dialogue commands, combat scripts, cutscenes, and scene loaders. Several systems write to the same Camera transform or Cinemachine priority, and no one can explain which request currently owns the view.

A reusable package must coordinate camera intent without becoming a universal cinematography engine. It must support simple 2D and 3D games, work without Cinemachine, allow a Cinemachine adapter, remain independent from Characters and Controllers, preserve The Eye as the final camera authority when Impact requests shake, and expose enough diagnostics to explain every active mode, target, modifier, zone, blend, and backend decision.

### 2.2 Evidence from existing work

| Source project/system | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Character switching, room framing, direct-scene testing, explosions, and role actions need one view authority | Clear active-character handoff | Replace direct Camera edits with target/mode/zone leases and camera-safe feedback |
| Don't Get Vince'd | Beat-'em-up framing, boss phases, hit response, and combat zones need layered camera behavior | Strong game feel and authored encounters | Keep combat and camera truth separate; define blend/target-loss/zone policies |
| Echo Systems Lab | Camera targets and mission systems already use focused components | Explicit references and event-driven updates | Package the repeated authority and diagnostics without importing project code |
| Impact | Feedback recipes request camera response | Semantic, accessibility-aware feedback | Impact maps a signal into The Eye; it never moves a camera directly |
| Voices | Dialogue needs speaker shots and camera cues | Explicit commands and provider seams | Dialogue command handlers acquire camera leases rather than owning a camera backend |
| The Hand | Interactors may need camera/aim origin information | Neutral pose snapshots | Project adapters expose origins without transferring camera authority |
| Future Fellowship/Vessel | Character selection and movement produce targets, facing, velocity, and ownership | Clear roster/control ownership | Camera consumes target snapshots through bridges or project adapters |

### 2.3 Consequences of doing nothing

- Several systems edit one Camera transform or Cinemachine priority.
- Temporary camera states restore the wrong previous state.
- Camera zones remain active after objects disable or scenes unload.
- Character switching produces long damping sweeps or stale targets.
- UI, dialogue, combat, and interaction code gain backend-specific references.
- Cinemachine becomes an accidental hard dependency of every game.
- Split-screen requires replacing a single-camera singleton.
- Camera shake bypasses accessibility and final camera authority.
- Missing bounds or target mappings fail as scene-specific null references.
- Performance problems remain invisible until large groups or rapid mode churn appear.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one duplicate-safe application-session camera authority.
- Represent independently evaluated outputs as bounded camera channels.
- Separate stable definitions from mutable channel, lease, blend, target, and backend state.
- Support targets, weighted groups, baseline modes, temporary priority modes, blends, and target-loss policies.
- Support offsets, look-ahead, dead-zone, zoom, lens, manual-look, and impulse requests through deterministic modifiers.
- Support bounds, camera zones, viewport metadata, and backend capability negotiation.
- Ship a built-in Unity Camera backend with no Cinemachine requirement.
- Define a separately versioned Cinemachine adapter contract.
- Expose structured diagnostics and isolated 2D/3D Laboratories.
- Preserve clean bridges to Characters, Controllers, Input, Settings, Pulse, Impact, Voices, Scene Flow, Interaction, World, and Multiplayer.

### 3.2 Non-goals

- Implement player movement or character selection.
- Author level geometry or room boundaries for the game.
- Replace Unity Camera rendering, URP/HDRP camera settings, camera stacking, or post-processing.
- Replace Cinemachine or copy its complete feature set.
- Own gameplay events, feedback recipes, dialogue sequence truth, Timeline, or cutscene direction.
- Poll one mandatory input map or choose player devices.
- Persist global preferences or live camera leases in save files.
- Assign multiplayer players to split-screen outputs.
- Promise collision avoidance, XR head tracking, or every genre camera in the MVP.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project with one Camera and one target | Create a root/configuration, bind the built-in backend, select a baseline follow mode, and pass a Laboratory test without Cinemachine |
| Programmer | Project with custom movement, camera, or Cinemachine setup | Request modes/targets/modifiers through stable APIs or write an explicit backend/bridge without editing core source |
| Designer | Needs rooms, dialogue shots, zooms, and authored transitions | Author modes, blend profiles, bounds, and zones with validation and predictable priority |
| Tester | Camera occasionally snaps, drifts, or follows the wrong target | Inspect channel contenders, target-loss reason, blend state, zones, modifiers, backend capabilities, and diagnostics |
| Maintainer | Package must be removed or upgraded | Preserve project-owned profiles and mappings; remove adapters/bridges before the core without breaking unrelated packages |

### 3.4 Measurable success criteria

- The package installs into a clean supported Unity project with zero compile errors.
- Core runtime and both built-in Laboratories run with no other Sperk's Forge package and no Cinemachine installation.
- One Main channel follows a target, switches modes, blends, loses/restores a target, applies bounds and modifiers, and reports diagnostics.
- A second channel can be created without changing the authority model.
- Removing the Cinemachine adapter leaves core and built-in backend functional.
- Duplicate roots and duplicate channels are rejected before camera side effects.
- Setup and repair are repeatable and non-destructive.
- Every implementation-dependent claim remains `Not run` until executed under SFGSS-004.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers.
- Gameplay programmers integrating project-owned controllers and characters.
- Designers authoring camera modes, zones, bounds, and blends.
- Technical artists using built-in Camera or Cinemachine through explicit mappings.
- QA testers diagnosing camera ownership and lifecycle defects.
- Package maintainers writing backend adapters and bridges.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| ECAM-UC-001 | Create a standalone Main channel | Novice installer | Package installed; one Camera exists | Root, channel, built-in backend, baseline mode, and target become Ready | MVP |
| ECAM-UC-002 | Register a follow target | Project code | Root/channel ready | Target handle and snapshot become available | MVP |
| ECAM-UC-003 | Switch controlled character target | Characters bridge/project | Old/new targets registered | Target lease transfers and warp policy prevents long damping sweep | MVP |
| ECAM-UC-004 | Acquire temporary dialogue mode | Voices bridge/project | Conversation active | Higher-priority mode controls the channel until lease release | MVP |
| ECAM-UC-005 | Acquire cutscene mode | Pulse/project | Cutscene begins | Camera intent applies without cutscene authority moving the backend directly | MVP |
| ECAM-UC-006 | Follow a weighted group | Project code | Several targets registered | Backend receives deterministic group snapshot and framing intent | MVP |
| ECAM-UC-007 | Apply room bounds | Designer/project | Bounds provider registered | Effective bounds constrain output according to backend capability | MVP |
| ECAM-UC-008 | Enter overlapping camera zones | Runtime subject | Zone adapters active | Zone-owned leases resolve by priority and release on exit | MVP |
| ECAM-UC-009 | Apply aiming look-ahead and offset | Controller/project | Target velocity/facing available | Modifiers adjust composition without controller moving camera | MVP |
| ECAM-UC-010 | Apply zoom request | Gameplay/UI project code | Channel ready | Resolved lens intent applies and clamps safely | MVP |
| ECAM-UC-011 | Request feedback impulse | Impact bridge | Impact recipe reaches camera channel | The Eye accepts bounded impulse and remains final camera authority | Integration |
| ECAM-UC-012 | Use manual look input | Input bridge/project | Semantic look data available | Camera consumes deltas without polling devices | Integration |
| ECAM-UC-013 | Configure a second viewport channel | Project/multiplayer adapter | Second Camera/backend available | Independent channel uses normalized viewport metadata | MVP architecture |
| ECAM-UC-014 | Use built-in Unity Camera backend | Developer | No Cinemachine installed | All core Laboratories remain usable | MVP |
| ECAM-UC-015 | Use Cinemachine backend adapter | Developer | Adapter and declared version installed | Modes/targets/groups/blends/bounds map through adapter | Later adapter |
| ECAM-UC-016 | Lose active target | Runtime | Target disables/destroys | Grace/fallback/baseline/block policy executes | MVP |
| ECAM-UC-017 | Notify target warp | Controller/Characters bridge | Target teleports | Backend invalidates damping history and refreshes composition | MVP |
| ECAM-UC-018 | Inspect runtime state | Tester | Play Mode active | Monitor explains channel/backend/mode/target/zone state | MVP |
| ECAM-UC-019 | Validate project setup | Designer/maintainer | Assets/scenes authored | Validator identifies duplicates, missing mappings, invalid limits, unsupported capabilities | MVP |
| ECAM-UC-020 | Remove adapter or package | Maintainer | Project-owned profiles/mappings exist | Removal preserves project content and unrelated compilation | MVP |

### 4.3 Explicitly unsupported use cases

- One class implementing every camera genre and backend.
- Gameplay code directly editing the final Camera while EchoCamera is active.
- Treating Cinemachine priority as the suite public API.
- Persisting live targets, blends, impulses, zones, or leases across saves.
- Using EchoCamera as level design, cutscene sequencing, rendering, post-processing, or player assignment authority.
- Guaranteeing arbitrary 3D collision avoidance, physical-camera simulation, or XR behavior in MVP.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Application-session root and channel registry.
- Channel readiness, backend binding, evaluation ownership, and final effective camera intent.
- Stable mode, channel, blend, bounds, zone, backend, and mapping identities.
- Target and weighted-group registration and snapshots.
- Baseline and temporary mode arbitration.
- Mode, modifier, bounds, zone, and impulse generational leases.
- Blend lifecycle, interruption, target loss, warp, and reduced-motion input.
- Provider-neutral lens, viewport, composition, look-ahead, dead-zone, offset, zoom, and manual-look intent.
- Bounds/zone arbitration, diagnostics, setup, validation, monitoring, and Laboratories.

### 5.2 The package does not own

- Character identity, player movement, input devices, gameplay events, dialogue flow, scene travel, level geometry, UI, audio, save files, global settings, post-processing, rendering pipelines, multiplayer player assignment, or a required vendor backend.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoCamera interacts |
|---|---|---|
| Character identity/selection | The Fellowship or project | Bridge registers/transfers targets and warp/control handoff |
| Movement/facing/velocity | The Vessel or project controller | Adapter provides snapshots and semantic camera intent |
| Input/rebinding/devices | The Will or project input | Bridge translates actions into manual-look/zoom requests |
| Global camera preferences | The Accord | Bridge applies sensitivity, inversion, FOV/zoom limits, reduced motion, shake scale |
| Runtime state/pause | The Pulse | Bridge may acquire modes; clock policy remains explicit |
| Feedback recipes | Impact | Bridge maps semantic signals to bounded impulses |
| Dialogue flow | Voices | Command handler acquires modes/targets; conversation owns sequence |
| Scene travel | The Passage | Lifecycle coordination only; no hard dependency |
| World interaction | The Hand | Project adapter may expose camera/aim pose |
| World/zone metadata | Future Atlas/project | Supplies stable zone/bounds mappings later |
| Multiplayer/split-screen ownership | Future Convergence/provider | Assigns players/subjects to channels; Eye controls channels |
| Production UI | Looking Glass/project | Presents settings/diagnostics only |
| Diagnostics aggregation | Observatory | Optional redacted provider |
| Save files | Chronicle | No live camera persistence |

### 5.4 Boundary tests

1. Does the feature decide what should be viewed, or merely generate the gameplay event requesting it?
2. Is it provider-neutral intent or backend/render-pipeline detail?
3. Can core remain useful without Characters, Controllers, Input, Impact, Voices, or Cinemachine?
4. Does another authority own persistence, scene travel, UI, or player assignment?
5. Would a bridge, backend adapter, or project adapter preserve ownership?
6. Does the feature create another writer to the final Camera?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

EchoCamera must compile and run alone, include a built-in Unity Camera backend, isolate Cinemachine and Physics adapters, avoid project assumptions, expose registration/injection seams, fail safely for missing capabilities, reject duplicates before side effects, and preserve project-owned profiles/mappings on removal.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Planned evidence |
|---|---|---|
| Installed alone | Core, built-in backend, Editor tools, and Laboratories operate | ECAM-T-001 through ECAM-T-030 |
| Direct 2D/3D Laboratory | Development authority created only when absent | ECAM-LAB registry |
| Cinemachine absent | No compile/runtime failure | Backend/removal tests |
| Optional bridge absent | Core behavior unchanged | Integration tests |
| Duplicate root | Rejected before Camera/backend mutation | Lifecycle tests |
| Missing configuration | Blocking diagnostic; no partial readiness | Validation tests |
| Samples deleted | Runtime remains intact | Removal tests |
| Second channel unused | Main-channel path stays simple | Channel tests |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum/planned version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity core | Platform | Yes | 6000.0 | Runtime/data/lifecycle | Required |
| Unity Camera APIs | Built-in backend | Yes for backend assembly | 6000.0 | Transform, lens, viewport | Core contracts remain separate |
| Physics2D | Optional adapter | No | Unity baseline | 2D zones | Adapter removable |
| Physics | Optional adapter | No | Unity baseline | 3D zones | Adapter removable |
| Cinemachine | Provider adapter | No | Planned 3.1.7; Not run | Optional Cinemachine 3 mapping | Adapter removed first |
| Test Framework | Test | Tests only | Verify later | Automated evidence | No Player dependency |

### 6.4 Forbidden dependencies

- Core references to Cinemachine, peer Echo packages, project assemblies, UnityEditor, tests, samples, or Workshop.
- Reflection discovery of backends/targets/mappings.
- Hidden scene, tag, layer, input-map, Resources, or Build Settings assumptions.
- Provider-specific assets inside neutral definitions.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
|---|---|---|---|---:|---|
| ECAM-CAP-001 | Duplicate-safe authority | One root claims before side effects | Approved | Yes | Runtime |
| ECAM-CAP-002 | Camera channels | Bounded independent outputs | Approved | Yes | Runtime/Data |
| ECAM-CAP-003 | Built-in backend | Standalone Unity Camera execution | Approved | Yes | Backend |
| ECAM-CAP-004 | Backend capabilities | Supported/unsupported/fallback results | Approved | Yes | Runtime/Backend |
| ECAM-CAP-005 | Targets | Generational target sources/snapshots | Approved | Yes | Runtime |
| ECAM-CAP-006 | Weighted groups | Dynamic bounded framing groups | Approved | Yes | Runtime |
| ECAM-CAP-007 | Baseline mode | Required fallback per channel | Approved | Yes | Data/Runtime |
| ECAM-CAP-008 | Mode leases | Priority/acquisition arbitration | Approved | Yes | Runtime |
| ECAM-CAP-009 | Blends | Cut/timed/easing/interruption | Approved | Yes | Data/Runtime |
| ECAM-CAP-010 | Target loss/warp | Grace, fallback, reset smoothing | Approved | Yes | Runtime |
| ECAM-CAP-011 | Modifiers | Offsets, look-ahead, dead zones, zoom, manual look | Approved | Yes | Runtime |
| ECAM-CAP-012 | Lens/viewport | Projection, common lens values, normalized viewport | Approved | Yes | Runtime/Backend |
| ECAM-CAP-013 | Impulses | Bounded transient camera response | Approved | Yes | Runtime/Backend |
| ECAM-CAP-014 | Bounds | One deterministic effective confinement request | Approved | Yes | Runtime |
| ECAM-CAP-015 | Zones | Zone-owned mode/bounds/modifier leases | Approved | Yes | Runtime/Adapters |
| ECAM-CAP-016 | Physics adapters | 2D/3D zone translation | Approved | Yes | Adapter/Sample |
| ECAM-CAP-017 | Multiple channels | Public multi-output model | Approved | Yes | Runtime |
| ECAM-CAP-018 | Cinemachine adapter | Separate Cinemachine 3 package | Approved later | No | Provider package |
| ECAM-CAP-019 | Peer bridges | Characters/Input/Settings/Pulse/Impact/Voices | Deferred | No | Bridge |
| ECAM-CAP-020 | Diagnostics/tooling | Monitor, validator, setup, support snapshot | Approved | Yes | Runtime/Editor |
| ECAM-CAP-021 | 2D/3D Laboratories | Independent standalone proof | Approved | Yes | Samples |
| ECAM-CAP-022 | Collision/occlusion | General obstacle solver | Deferred | No | Extension |
| ECAM-CAP-023 | Timeline | Cinematic clips | Deferred | No | Integration |
| ECAM-CAP-024 | Physical camera/post FX | Provider-specific art direction | Deferred | No | Provider |
| ECAM-CAP-025 | XR head tracking | XR ownership | Rejected for core | No | Provider |
| ECAM-CAP-026 | Automatic split-screen assignment | Player ownership/layout | Deferred | No | Convergence bridge |

### 7.2 MVP capability set

One root, Main channel plus bounded registry, built-in backend, targets/groups, baseline/temporary modes, blends, loss/warp, modifiers, lens/viewport, impulses, bounds/zones, diagnostics, setup/validation, and independent 2D/3D Laboratories.

### 7.3 Later capability set

Cinemachine adapter, peer bridges, collision/occlusion extensions, Timeline, advanced rails/orbits/presets, physical-camera/post-processing providers, and multiplayer/XR adapters after dedicated design.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Universal camera controller | Rejected | Monolithic and genre-conflicted | New package decision only |
| Cinemachine types in core | Rejected | Hidden hard dependency | Provider adapter |
| External Camera transform writers | Rejected | Breaks authority | Request intent instead |
| Persist live camera state | Rejected | Handles cannot restore safely | Save semantic project state only |
| Arbitrary bounds intersection | Deferred | Geometry/backend complexity | Proven requirement |
| General collision solver | Deferred | Large provider-specific problem | Extension design |
| Timeline core dependency | Rejected | Optional cinematics | Integration package |
| XR core ownership | Rejected | XR runtime owns head pose | Provider research |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Channel, mode, blend, bounds, zone, safety, backend mapping identities | Active targets, leases, scene Cameras, current blends |
| Runtime authority | Root, channels, arbitration, snapshots, leases, blends, diagnostics | Editor APIs, gameplay rules, vendor types |
| Backend execution | Built-in Camera or optional adapter | Camera authority or peer truth |
| Bridges/adapters | Target, input, preference, dialogue, feedback translation | Competing root |
| Tooling/samples | Setup, monitor, gizmos, Laboratories | Production authority |

### 8.2 Component topology

```text
Gameplay / bridge / project adapter
    -> IEchoCameraService
        -> EchoCameraRoot
            -> CameraChannelRuntime
                -> targets/groups
                -> baseline + mode contenders
                -> modifiers + impulses
                -> bounds + zones
                -> blend resolver
                -> CameraStateSnapshot
                    -> ICameraBackend
                        -> BuiltInUnityCameraBackend
                        -> optional Cinemachine adapter
                        -> project backend
                            -> Unity Camera output
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root? | Yes by default; injected service allowed |
| Root type | `EchoCameraRoot` |
| Duplicate behavior | Reject before backend subscriptions or Camera mutation |
| Initialization | Awake claim; explicit Initialize validates/binds |
| Shutdown | Cancel transient work, release leases/targets, unbind backend, release authority |
| Direct scene | Development initializer creates configured root/rig only when absent |
| Injection | `IEchoCameraService`, `ICameraBackend`, clocks, target/bounds providers |

### 8.4 Channel lifecycle

1. Claim root authority.
2. Validate configuration and unique channels.
3. Bind one backend per channel.
4. Validate baseline mode/lens/bounds policy.
5. Publish Ready or Unavailable.
6. Accept targets, groups, modes, modifiers, bounds, zones, impulses.
7. Resolve effective state and blend.
8. Exactly one backend applies output.
9. Release scene/lifetime-owned state.
10. Teardown backend before invalidating generation.

### 8.5 Arbitration and blends

Modes resolve by higher priority, then later acquisition. Losing modes remain latent. Modifiers use documented additive, multiplicative, and override rules with clamps. Blends default to unscaled time. Interruption begins from the current evaluated output, never the stale original source. Reduced-motion policy is applied before publication.

### 8.6 Failure model

| Failure | Detection | Result | Fallback | Code |
|---|---|---|---|---|
| Duplicate root | Awake claim | Duplicate rejected | No side effects | ECAM-001 |
| Missing configuration | Initialize | Blocking report | No partial readiness | ECAM-002 |
| Duplicate channel ID | Validation | Blocking report | Reject config | ECAM-003 |
| Missing backend | Bind | Channel Unavailable | Other channels continue | ECAM-004 |
| Unsupported capability | Request validation | Unsupported | Explicit fallback only | ECAM-005 |
| Missing baseline | Channel validation | Blocking channel failure | Unavailable | ECAM-006 |
| Invalid profile | Validation | Error | Reject request | ECAM-007 |
| Target lost | Snapshot | Structured reason | Grace/hold/fallback/baseline/block | ECAM-008 |
| Stale handle | Generation check | Rejected | No state change | ECAM-009 |
| Blend/backend failure | Apply | Error | Cut/fallback/hold | ECAM-010 |
| Bounds unavailable | Resolve/apply | Warning/error | Required block or explicit fallback | ECAM-011 |
| Zone disabled | Lifecycle | Cleanup | Release once | ECAM-012 |
| Capacity exceeded | Admission | Rejected/replaced | Bounded state | ECAM-013 |
| Backend exception | Apply boundary | Channel isolated | Disable/fallback channel | ECAM-014 |
| Scene unload mid-operation | Lifecycle | Warning | Release scene state | ECAM-015 |
| Adapter mismatch | Initialization | Compatibility error | Core remains usable | ECAM-016 |

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoCameraConfiguration` | Root limits, channel catalog, fallback and diagnostics policy | Configuration ID | No | Yes |
| `CameraChannelDefinition` | Channel ID, baseline mode, viewport, backend key, limits | Yes | No | Yes |
| `CameraModeDefinition` | Follow/group/lens/blend/bounds/modifier intent | Yes | No | Yes |
| `CameraBlendProfile` | Duration, curve key, time domain, reduced-motion behavior | Yes | No | Yes |
| `CameraLensProfile` | Orthographic/perspective-neutral lens intent and clamps | Yes | No | Yes |
| `CameraBoundsProfile` | Provider-neutral bounds policy and fallback behavior | Yes | No | Yes |
| `CameraZoneProfile` | Mode, bounds, modifier and priority requests for a zone | Yes | No | Yes |
| `CameraImpulseProfile` | Bounded amplitude, duration, frequency and channel policy | Yes | No | Yes |
| `CameraBackendMapping` | Project-owned mapping from neutral intent to one backend | Mapping ID | No | Yes |

Unity asset GUIDs remain Editor identities. Durable runtime references use package/domain IDs under SFGSS-003. Display names may change without changing save, diagnostic, bridge, or migration identity.

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `CameraChannelRuntime` | Root | Application session/channel | Recreated when channel removed/root resets | Never saved |
| `CameraTargetRegistration` | Root/channel | Handle lifetime | Release, owner destruction, scene unload | Never saved |
| `CameraGroupRuntime` | Root/channel | Handle lifetime | Release/empty policy | Never saved |
| `CameraModeLeaseState` | Channel | Lease lifetime | Explicit release/owner teardown | Never saved |
| `CameraModifierLeaseState` | Channel | Lease lifetime | Explicit release/owner teardown | Never saved |
| `CameraBoundsLeaseState` | Channel | Lease lifetime | Explicit release/owner teardown | Never saved |
| `CameraBlendState` | Channel/backend | One transition | Completion, replacement, cut, shutdown | Never saved |
| `CameraImpulseState` | Channel | Bounded duration | Complete/cancel/replace/shutdown | Never saved |
| `CameraZoneOccupancy` | Zone adapter | Occupancy/zone lifetime | Exit, disable, scene unload, teleport reconciliation | Never saved |
| `CameraDiagnosticHistory` | Root | Bounded session history | Reset/shutdown | Exportable only as redacted diagnostics |

### 9.3 Stable identifiers

- `CameraChannelId` identifies a logical output such as Main or PlayerOne.
- `CameraModeId`, `CameraBlendProfileId`, `CameraBoundsProfileId`, `CameraZoneProfileId`, and `CameraImpulseProfileId` identify authored definitions.
- `CameraTargetHandle`, `CameraGroupHandle`, `CameraModeLease`, `CameraModifierLease`, `CameraBoundsLease`, and `CameraImpulseHandle` are runtime generation-qualified handles, not durable IDs.
- IDs use canonical lowercase namespaced strings or SFGSS-003-compatible generated IDs.
- Validators detect empty IDs, duplicates, aliases, collisions, and released-ID changes.
- A renamed display label never changes the durable ID.
- Released-ID changes require an alias/migration record.

### 9.4 ScriptableObject safety

Definitions remain immutable during play. Active target transforms, positions, velocities, blend progress, zone occupants, winning priorities, impulse elapsed time, viewport ownership, backend instances, and current camera poses live in runtime state. Editor previews must use cloned or detached preview models rather than mutating shared assets.

### 9.5 Serialization and migration

EchoCamera has no MVP game-save payload. Configuration assets carry schema versions for authored-data migration. Migrations must preserve source assets, preview changes, retain GUIDs, validate stable IDs, and produce receipts. Unknown future fields are preserved when the chosen authoring format supports them; otherwise migration must stop rather than silently discard them. Runtime camera state is reconstructed from current project state after load or scene entry.

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `EchoCameraRoot` | Component | Claims authority, owns channels/backends/state/diagnostics | Setup prefab or First Light integration |
| `IEchoCameraService` | Interface | Injectable service boundary | Implemented by root/runtime service |
| `CameraChannelId` | Value type | Stable logical output identity | Authored/project code |
| `CameraChannelSnapshot` | Immutable struct | Effective mode, targets, blend, lens, bounds, viewport, health | Service-produced |
| `ICameraTargetSource` | Interface | Supplies pose/velocity/validity/warp revision snapshots | Project or bridge |
| `CameraTargetHandle` | Struct | Generational target registration lease | Service-issued |
| `CameraGroupHandle` | Struct | Generational weighted-group lease | Service-issued |
| `CameraModeRequest` | Struct | Mode, priority, targets/group, blend and lifetime request | Caller-created |
| `CameraModeLease` | Struct | Generational temporary-mode lease | Service-issued |
| `CameraModifierRequest` | Struct | Offset/look-ahead/dead-zone/zoom/manual-look intent | Caller-created |
| `CameraModifierLease` | Struct | Generational modifier lease | Service-issued |
| `CameraBoundsRequest` | Struct | Bounds provider/profile/priority request | Caller-created |
| `CameraBoundsLease` | Struct | Generational bounds lease | Service-issued |
| `CameraImpulseRequest` | Struct | Bounded transient camera-response intent | Caller/Impact bridge |
| `CameraImpulseHandle` | Struct | Generational impulse handle | Service-issued |
| `ICameraBackend` | Interface | Applies one channel's effective camera state | Built-in or adapter |
| `CameraBackendCapabilities` | Struct | Declares backend feature support and tick ownership | Backend-produced |
| `CameraStateSnapshot` | Immutable struct | Provider-neutral evaluated output for backend | Channel-produced |
| `CameraResult` | Struct/enum payload | Success, rejection, unsupported, stale, unavailable and reasons | All operations |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/main-loop rule |
|---|---|---|---|---|
| `TryRegisterTarget(channel, source, options)` | Register a target source | Ready channel, valid source | Handle or structured rejection | Main thread |
| `TryCreateGroup(channel, members, options)` | Create weighted group | Valid member handles/limits | Group handle or rejection | Main thread |
| `RequestMode(request)` | Acquire temporary camera mode | Ready channel and supported intent | Lease or structured result | Main thread |
| `TryRelease(CameraModeLease)` | Release one mode contender | Matching root/channel/generation | Success, stale or foreign-handle result | Main thread |
| `RequestModifier(request)` | Add bounded lens/offset/look intent | Valid channel/profile/limits | Lease or result | Main thread |
| `RequestBounds(request)` | Acquire bounds authority | Valid provider/profile/channel | Lease or result | Main thread |
| `RequestImpulse(request)` | Start transient impulse | Channel supports impulse path and safety policy | Handle or result | Main thread |
| `TryCancel(CameraImpulseHandle)` | Cancel one active impulse | Matching generation | Success, stale or already-complete | Main thread |
| `NotifyTargetWarp(handle, pose)` | Prevent long damping after teleport/switch | Valid target handle | Updates warp revision or rejects stale | Main thread |
| `TryGetSnapshot(channel)` | Read immutable effective channel state | Known channel | Snapshot or unavailable result | Main thread; detached snapshot may be read later |
| `RegisterBackend(channel, backend)` | Bind explicit backend | Unbound/replace-safe channel | Registration handle/result | Main thread |
| `ResetDevelopmentState()` | Return Laboratories/dev sessions to baseline | Development-only | Deterministic reset report | Main thread |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `OnChannelStateChanged` | Channel | After effective state commit | Old/new immutable snapshots | Listeners do not drive authority |
| `OnModeWinnerChanged` | Channel | After arbitration | Channel, old/new mode IDs and reasons | Presentation/diagnostics only |
| `OnBlendStarted` | Channel | After blend state created | From/to, profile, duration | No backend mutation by listener |
| `OnBlendCompleted` | Channel | After final output commit | Channel, terminal result | Exactly once |
| `OnTargetAvailabilityChanged` | Root/channel | After registration validity changes | Target ID/handle and reason | No Transform retention required |
| `OnImpulseChanged` | Channel | After start/cancel/complete | Handle and state | Bounded listener cost |
| `OnBackendHealthChanged` | Root | After capability/health change | Channel/backend/health/reasons | Failure-isolated |
| `OnDiagnostic` | Root | After structured diagnostic creation | Redacted `ECAM-*` event | No per-frame spam |

Events are raised only after authoritative state commits. Listener exceptions are isolated and diagnosed; they never roll back channel truth.

### 10.4 Async and cancellation policy

Core evaluation and built-in backend application are synchronous on Unity's main thread. Backends may prepare asynchronous resources only through an explicit lifecycle contract, timeout, cancellation token/handle, and main-thread publication step. Mode, modifier, bounds, target, and group leases cancel by disposal/release. Blend interruption starts from the current evaluated output. Impulses can be cancelled until terminal completion. Scene unload and shutdown release scene-bound work and invalidate generations.

### 10.5 API ergonomics

The novice path uses one setup-created root, a Main channel, built-in Unity Camera backend, one target component, one follow mode, and one bounds profile. The advanced path injects `IEchoCameraService`, custom target sources, backends, clocks, bounds providers, zone adapters, and project bridges. Static convenience access may exist, but injection remains available for tests and custom composition.

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install the package.
2. Open **Tools > EchoDevGames > The Eye > Setup**.
3. Select Core plus Built-in Backend, and optionally 2D/3D zone adapters.
4. Choose or create the project-owned output folder.
5. Configure Main channel, baseline mode, Camera reference/prefab, bounds and safety limits.
6. Preview every created or modified asset/scene reference.
7. Apply create-only-safe operations.
8. Open the 2D or 3D Standalone Laboratory.
9. Run validation and save the setup receipt.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create configuration/catalogs | Project-owned assets | Nothing existing by default | Yes | Undo/create receipt | Setup report |
| Create root/rig prefab | Root, backend and Camera prefab | Nothing existing | Yes | Undo | Setup report |
| Add canonical Boot instance | Scene instance after preview | Selected scene only | Yes | Undo | Scene receipt |
| Add target source template | Sample/project component prefab | Nothing existing | Yes | Undo | Creation receipt |
| Add 2D/3D zone template | Adapter components/profile | Selected object only after preview | Yes | Undo | Scene receipt |
| Repair safe references | Missing assignments only | Selected assets/scenes | Yes | Undo and before/after report | Repair report |
| Generate diagnostic snapshot | None | None | Yes | N/A | Portable report |

Setup never silently adds unrelated packages, deletes cameras, replaces scenes, changes render-pipeline settings, or overwrites project-owned profiles.

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Eye Setup Window | Installer | Preview/apply package setup | No |
| Channel/Mode Inspectors | Designer | Author baseline, priority, blend, lens and fallback policy | No |
| Target/Group Inspector | Designer/developer | Weights, aim points, grace and warp policy | No |
| Bounds/Zone Inspectors | Level designer | Bounds providers, priorities, occupancy and debug drawing | No |
| Backend Mapping Inspector | Technical designer | Neutral capability mapping | No |
| Runtime Camera Monitor | Tester | Winners, leases, targets, blends, zones, impulses and backend health | Editor observer only |
| Camera Simulator | Tester | Force target loss, blend interruption, capability mismatch and bounds failure | No production dependency |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| ECAM-VAL-001 | Missing configuration/root | Blocker | Yes | Create-only |
| ECAM-VAL-002 | Duplicate roots/channels | Blocker | Guided | No destructive auto-fix |
| ECAM-VAL-003 | Empty/duplicate released stable ID | Blocker | Guided | Only unreleased empty IDs |
| ECAM-VAL-004 | Missing baseline mode/backend | Blocker | Yes | Assign/create with preview |
| ECAM-VAL-005 | Unsupported mode/backend capability | Error | Guided | No |
| ECAM-VAL-006 | Invalid blend/lens/modifier limits | Error | Guided | Safe clamp only with consent |
| ECAM-VAL-007 | Missing/invalid bounds provider | Warning/Error by requiredness | Guided | No |
| ECAM-VAL-008 | Zone lacks compatible adapter/collider | Error | Guided | Add component only with preview |
| ECAM-VAL-009 | Unbounded channels/leases/history/impulses | Blocker | Yes | Apply approved defaults with consent |
| ECAM-VAL-010 | Two authorities write one Camera | Blocker | Guided | No |
| ECAM-VAL-011 | Backend tick ownership ambiguous | Blocker | Guided | No |
| ECAM-VAL-012 | Cinemachine types leaked into core | Blocker | Move to adapter | No |
| ECAM-VAL-013 | Scene/sample dependency leaked into Runtime | Blocker | Guided | No |
| ECAM-VAL-014 | Released ID changed without alias | Blocker | Guided | No |
| ECAM-VAL-015 | Diagnostics expose hierarchy/production data | Warning | Guided | No |
| ECAM-VAL-016 | Workshop setup facade unavailable after integration ships | Error | Generate package-owned facade shell only when authorized | No |

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Planned routes are Git URL, local path, tarball, embedded development, and Workshop selection after the setup facade is implemented. Registry publication remains future evidence. Every route remains `Planned/Not run` until executed under SFGSS-004.

### 12.2 Minimal scene setup

1. One configured `EchoCameraRoot` or injected service.
2. One Main `CameraChannelDefinition` and baseline mode.
3. One Unity Camera bound through the built-in backend.
4. One registered `ICameraTargetSource`.
5. Optional bounds provider/profile.
6. Project code or a bridge that requests temporary modes/modifiers/impulses.

### 12.3 Boot-scene setup

Normal production setup places the canonical root in the Boot/preload scene or creates it through First Light's explicit integration. It claims before side effects, binds configured backends, persists according to configuration, and releases scene-owned targets/zones/bounds on unload.

### 12.4 Direct-scene setup

`EchoCameraDirectSceneInitializer` creates the configured development root and rig only when absent, clearly marks development initialization, uses the same duplicate rules as production, and is disabled/excluded from release builds unless explicitly approved.

### 12.5 Scene isolation rule

The 2D and 3D Standalone Laboratories use only EchoCamera core, built-in backend, the relevant zone adapter, declared Unity dependencies, and redistributable sample assets. Cinemachine, Characters, Controllers, Dialogue, Impact, Input, UI, Passage, and project code are absent. Their integration scenes are separate bridge evidence.

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Laboratory purpose

The package ships two independent Laboratories:

- **Eye Camera 2D Laboratory:** orthographic follow, target/group changes, look-ahead, offsets, zoom, 2D bounds, overlapping zones, impulses, multiple modes, direct-scene startup, and stress diagnostics.
- **Eye Camera 3D Laboratory:** perspective follow, lens intent, 3D bounds anchors/volumes, target loss, group framing, viewport/channel behavior, backend capability tests, and the same lease/blend contracts.

Both use the built-in backend and fake bridge providers. Cinemachine is not required for standalone proof.

### 13.2 Required Laboratory contents

- Visible instructions, package/version/channel/backend status.
- Manual controls independent from The Will and The Looking Glass.
- Plain diagnostic readouts for targets, groups, winning mode, modifiers, bounds, zones, blend, impulse and backend health.
- Target destruction, disable, teleport/warp and replacement controls.
- Overlapping zones and bounds providers.
- Cut, blend, interruption, reduced-motion and target-loss cases.
- One secondary channel/viewport demonstration.
- Duplicate-root, capability-mismatch, saturation, reset and shutdown controls.
- No restricted or project-owned production content.

### 13.3 Laboratory acceptance checklist

| Test | Action | Expected result | Automated/manual | Status |
|---|---|---|---|---|
| ECAM-LAB-001 | Initialize one configured Main channel | Root, channel and built-in backend become Ready with baseline state | Manual unless automated later | Not run |
| ECAM-LAB-002 | Introduce a duplicate root before initialization | Duplicate is rejected before backend binding or Camera mutation | Manual unless automated later | Not run |
| ECAM-LAB-003 | Introduce a duplicate root after readiness | Existing authority remains unchanged and duplicate performs no side effects | Manual unless automated later | Not run |
| ECAM-LAB-004 | Remove required configuration | Root reports a blocking configuration result without partial readiness | Manual unless automated later | Not run |
| ECAM-LAB-005 | Remove the configured backend | Affected channel becomes Unavailable while other channels remain valid | Manual unless automated later | Not run |
| ECAM-LAB-006 | Register one target and use baseline follow | Built-in backend follows the target using the authored mode | Manual unless automated later | Not run |
| ECAM-LAB-007 | Acquire a higher-priority mode | Temporary mode becomes the deterministic winner | Manual unless automated later | Not run |
| ECAM-LAB-008 | Release lower and higher mode leases out of order | Winner recomputes from active leases without stale restoration | Manual unless automated later | Not run |
| ECAM-LAB-009 | Reuse a stale mode lease after channel reset | Lease is rejected and cannot affect recycled state | Manual unless automated later | Not run |
| ECAM-LAB-010 | Request a cut transition | Output changes in one evaluation without a residual blend | Manual unless automated later | Not run |
| ECAM-LAB-011 | Request a timed blend | Evaluated output progresses according to the configured curve/time domain | Manual unless automated later | Not run |
| ECAM-LAB-012 | Interrupt an active blend | New blend begins from the current evaluated output | Manual unless automated later | Not run |
| ECAM-LAB-013 | Enable reduced-motion policy during a long blend | Resolved transition follows configured reduced-motion behavior | Manual unless automated later | Not run |
| ECAM-LAB-014 | Register, query and release a target | Target lifecycle and generation remain deterministic | Manual unless automated later | Not run |
| ECAM-LAB-015 | Disable the active target source | Target becomes unavailable with a structured reason | Manual unless automated later | Not run |
| ECAM-LAB-016 | Destroy the active target source | Grace/fallback policy runs without a missing-reference exception | Manual unless automated later | Not run |
| ECAM-LAB-017 | Notify a target warp or teleport | Backend resets damping history and avoids a long catch-up sweep | Manual unless automated later | Not run |
| ECAM-LAB-018 | Restore a target within its grace interval | Configured grace policy resumes without a false new target | Manual unless automated later | Not run |
| ECAM-LAB-019 | Lose a target beyond grace | Channel holds, falls back, returns to baseline, or blocks by policy | Manual unless automated later | Not run |
| ECAM-LAB-020 | Create a weighted target group | Group framing uses valid members and authored weights | Manual unless automated later | Not run |
| ECAM-LAB-021 | Add and remove group members rapidly | Group remains bounded and deterministic during churn | Manual unless automated later | Not run |
| ECAM-LAB-022 | Apply velocity-based look-ahead | Look intent follows the clamped target snapshot | Manual unless automated later | Not run |
| ECAM-LAB-023 | Stack two additive offsets | Effective offset combines in documented order | Manual unless automated later | Not run |
| ECAM-LAB-024 | Request a zoom/lens modifier | Lens intent clamps to mode and channel safety bounds | Manual unless automated later | Not run |
| ECAM-LAB-025 | Override dead-zone intent temporarily | Override wins and releases without stale restoration | Manual unless automated later | Not run |
| ECAM-LAB-026 | Apply manual-look input through a project adapter | Semantic look intent changes framing without input dependency | Manual unless automated later | Not run |
| ECAM-LAB-027 | Reuse a stale modifier lease | Lease is rejected and current modifier state remains unchanged | Manual unless automated later | Not run |
| ECAM-LAB-028 | Start one bounded impulse | Camera response is applied through The Eye authority | Manual unless automated later | Not run |
| ECAM-LAB-029 | Cancel an active impulse | Impulse ends once and cannot alter later recycled handles | Manual unless automated later | Not run |
| ECAM-LAB-030 | Acquire two bounds requests with different priorities | One effective bounds request wins deterministically | Manual unless automated later | Not run |
| ECAM-LAB-031 | Use a 2D bounds provider | Built-in backend constrains the view to the provider result | Manual unless automated later | Not run |
| ECAM-LAB-032 | Use a 3D anchor/volume bounds provider | Camera obeys provider-neutral 3D bounds intent | Manual unless automated later | Not run |
| ECAM-LAB-033 | Remove a required bounds provider | Channel follows explicit failure/fallback policy | Manual unless automated later | Not run |
| ECAM-LAB-034 | Enter one camera zone | Zone acquires its authored mode/bounds/modifier leases | Manual unless automated later | Not run |
| ECAM-LAB-035 | Overlap two zones | Priority and later-acquisition rules select the effective zone intent | Manual unless automated later | Not run |
| ECAM-LAB-036 | Exit overlapping zones out of order | Each zone releases only its own leases | Manual unless automated later | Not run |
| ECAM-LAB-037 | Disable a zone while occupied | Zone releases once without waiting for an exit callback | Manual unless automated later | Not run |
| ECAM-LAB-038 | Teleport across zones without normal exit events | Occupancy reconciliation produces correct active zones | Manual unless automated later | Not run |
| ECAM-LAB-039 | Assign a normalized viewport to a channel | Backend applies the configured viewport rectangle | Manual unless automated later | Not run |
| ECAM-LAB-040 | Run two bounded camera channels | Each channel evaluates independently with no output collision | Manual unless automated later | Not run |
| ECAM-LAB-041 | Remove one secondary channel | Its backend and leases teardown without disturbing Main | Manual unless automated later | Not run |
| ECAM-LAB-042 | Request a capability the backend does not support | Request returns Unsupported or uses only an explicit fallback | Manual unless automated later | Not run |
| ECAM-LAB-043 | Replace a backend at a safe boundary | Old backend tears down before new backend publishes output | Manual unless automated later | Not run |
| ECAM-LAB-044 | Use a backend-driven update policy | Root does not also write the Camera during that frame | Manual unless automated later | Not run |
| ECAM-LAB-045 | Use the root-driven LateUpdate backend | Exactly one root-owned write applies the final state | Manual unless automated later | Not run |
| ECAM-LAB-046 | Pause gameplay time | Unscaled blends/impulses continue or pause according to authored policy | Manual unless automated later | Not run |
| ECAM-LAB-047 | Lose and regain application focus | Channel state remains coherent and backend reapplies as documented | Manual unless automated later | Not run |
| ECAM-LAB-048 | Unload a scene containing targets/zones | Scene-owned state releases without leaked handles | Manual unless automated later | Not run |
| ECAM-LAB-049 | Enter a Laboratory scene directly | Development initializer creates one clearly marked authority when absent | Manual unless automated later | Not run |
| ECAM-LAB-050 | Validate a release build configuration | Development initializer and debug-only helpers are excluded/disabled | Manual unless automated later | Not run |
| ECAM-LAB-051 | Remove the Cinemachine adapter completely | Built-in backend and standalone Laboratories still compile and function | Manual unless automated later | Not run |
| ECAM-LAB-052 | Map neutral state through a fake Cinemachine adapter | Capability mapping preserves core authority and adapter isolation | Manual unless automated later | Not run |
| ECAM-LAB-053 | Acquire a dialogue-shot mode through a fake bridge | Voices requests a lease but does not own the backend | Manual unless automated later | Not run |
| ECAM-LAB-054 | Request an impulse through a fake Impact bridge | Impact signal maps into Eye impulse without direct Camera mutation | Manual unless automated later | Not run |
| ECAM-LAB-055 | Switch a character target through a fake Fellowship bridge | Target handoff and warp notification prevent stale following | Manual unless automated later | Not run |
| ECAM-LAB-056 | Export a redacted diagnostic snapshot | Snapshot includes IDs, modes, timings and health without private hierarchy data | Manual unless automated later | Not run |
| ECAM-LAB-057 | Exceed the bounded diagnostic history | Oldest records prune deterministically | Manual unless automated later | Not run |
| ECAM-LAB-058 | Load an invalid mode or bounds profile | Validation blocks the request with an ECAM code | Manual unless automated later | Not run |
| ECAM-LAB-059 | Run setup and repair twice | Second run is non-destructive and reports no duplicate output | Manual unless automated later | Not run |
| ECAM-LAB-060 | Remove samples and optional adapters | Core package remains compile-safe and removable by documented order | Manual unless automated later | Not run |

### 13.4 Optional showcase and integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| Eye + Fellowship + Vessel | Camera, Characters, Controllers | Switchable-character target and movement framing | Depends on roster/control bridges |
| Eye + Impact | Camera, Feedback | Accessibility-aware camera impulses | Impact owns recipe; Eye owns final view |
| Eye + Voices | Camera, Dialogue | Speaker/group shot leases | Dialogue owns conversation flow |
| Eye + Passage | Camera, Scene Flow | Scene-exit/entry camera handoff | Passage owns travel |
| Eye + Cinemachine | Camera plus provider adapter | Prove backend mapping | Adapter evidence, not core proof |

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The runtime core is nonvisual except for the Camera output it authorizes. Production camera indicators, split-screen frames, debug panels, photo overlays, letterboxing, reticles, and transition UI belong to project presentation or The Looking Glass. The package may ship plain Laboratory readouts and Editor gizmos.

### 14.2 Required states

Presentation and diagnostics must distinguish:

- Ready baseline.
- Blending.
- Cut.
- Target unavailable/grace/fallback.
- Bounds unavailable.
- Backend unavailable or capability mismatch.
- Reduced-motion substitution.
- Secondary channel disabled.
- Warning, degraded, and failure states.

### 14.3 Accessibility requirements

- Every blend profile declares a reduced-motion alternative: cut, shortened duration, reduced travel, or project-defined safe substitute.
- Camera impulses pass through project safety limits for amplitude, duration, frequency and concurrent count.
- The package exposes semantic intensity channels so accessibility policy can scale motion before backend publication.
- Important camera state is never communicated by motion alone; UI/prompt bridges may expose status text or icons.
- Manual-look and recenter behavior remain configurable through project adapters.
- Split-screen viewport metadata supports readable UI safe-area calculation without making EchoCamera the UI authority.
- Flash, rumble, audio and hit-stop remain outside The Eye and may be disabled independently.

### 14.4 Visual customization

Project-owned modes, curves, bounds, zones, rigs, viewport layouts, Camera settings, art, post-processing and backend mappings are replaceable without editing package Runtime.

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| Root/channel/backend health | API, Inspector, report | All, redacted | Low |
| Winning mode and contenders | API, Runtime Monitor | Development; optional release summary | Low |
| Targets/groups and validity | Monitor/snapshot | Development | Configurable |
| Blend/lens/modifier/bounds/zone state | Monitor/snapshot | Development | Configurable |
| Impulse counts and safety clamping | Counters/events | Development; limited release | Low |
| Evaluation/backend timings | Bounded counters | Development | Configurable |
| Redacted support snapshot | Explicit export | Development/support | Bounded |

### 15.2 Structured status

Every channel exposes:

- Initialization and availability state.
- Authority/root generation.
- Configuration/channel/backend IDs and versions.
- Backend capabilities and tick owner.
- Baseline and winning mode IDs.
- Target/group IDs and validity reasons.
- Effective lens, viewport, bounds and modifier summaries.
- Blend and impulse counts/timings.
- Active zone and lease counts.
- Saturation, clamp, fallback and last failure codes.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| ECAM-001 | Blocker | Duplicate root rejected | Remove/repair duplicate authority |
| ECAM-002 | Blocker | Configuration missing/invalid | Run setup/validator |
| ECAM-003 | Blocker | Duplicate channel ID | Repair project-owned IDs |
| ECAM-004 | Error | Backend unavailable | Assign/install compatible backend |
| ECAM-005 | Warning/Error | Capability unsupported | Change request or explicit fallback |
| ECAM-006 | Blocker | Baseline mode missing | Assign baseline |
| ECAM-007 | Error | Invalid profile/request | Repair authored data |
| ECAM-008 | Info/Warning | Target unavailable | Inspect target-loss policy |
| ECAM-009 | Warning | Stale/foreign handle rejected | Stop retaining expired handles |
| ECAM-010 | Error | Blend/backend apply failure | Inspect backend and fallback |
| ECAM-011 | Warning/Error | Bounds unavailable | Repair provider or fallback |
| ECAM-012 | Info | Zone lifecycle reconciliation | Verify disable/unload path if unexpected |
| ECAM-013 | Warning | Capacity/safety limit reached | Tune bounded limits |
| ECAM-014 | Error | Backend exception isolated | Repair/replace backend |
| ECAM-015 | Warning | Scene-owned state removed mid-operation | Verify lifecycle ownership |
| ECAM-016 | Error | Adapter/core version mismatch | Install compatible versions |

### 15.4 Observatory bridge

A separate bridge exposes channel health, backend capabilities, effective modes, targets/groups, blends, zones, bounds, impulses, timings, limits and recent `ECAM-*` events to The Observatory. EchoCamera never requires EchoDiagnostics.

### 15.5 Logging policy

Logs are categorized and rate-limited. Normal operation produces no per-frame spam. Release logs omit production hierarchy paths, scene-object names when sensitive, transform histories, camera screenshots, user input and arbitrary project payloads. High-frequency state belongs in bounded counters/snapshots, not Console floods.

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Mode/channel/bounds/zone definitions | Project configuration | Project | As assets | Unity assets |
| Active targets/groups/leases | Session | EchoCamera | No | Runtime only |
| Current blend/impulses | Session | EchoCamera | No | Runtime only |
| Backend instance/Camera reference | Session/scene | Project/backend | No | Runtime/scene |
| Optional player camera preference | Global preference | The Accord/project | Optional through bridge | Accord/project backend |
| Optional game-specific camera mode | Save-slot game state | Project | Project decision, not raw EchoCamera state | Chronicle/project payload |

### 16.2 Standalone behavior

Without Chronicle or The Accord, EchoCamera starts from project configuration and runtime requests. It chooses no hidden persistence backend and writes no save/settings file.

### 16.3 Optional participant/provider contract

A project may persist stable, game-specific camera choices such as preferred zoom sensitivity or an authored exploration camera preset. The bridge stores a small project-defined preference/state record and reapplies a fresh request after the relevant target, scene and channel exist. It must not serialize runtime handles, Camera references, blend progress, zone occupancy or temporary leases.

### 16.4 Failure and recovery

Missing/old/newer optional records fall back to configured defaults with structured diagnostics. Unknown records remain owned by the persistence authority. Failed reapplication does not corrupt camera state; the channel remains at baseline or an explicit project fallback.

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

All integrations are explicit, removable and versioned. The requester supplies semantic intent; EchoCamera remains final camera authority; the bridge never lets the peer write the Camera/backend directly.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| First Light | Separate bridge or tiny owner integration | Integration specification | Launch -> Eye | Initialize configured root/channel | No |
| Observatory | Separate bridge | Integration package | Eye -> Diagnostics | Health, counters, snapshots | No |
| Accord | Separate bridge/project adapter | Integration package | Settings -> Eye | Reduced motion, sensitivity, camera preferences | No |
| Passage | Separate bridge/project adapter | Project/bridge | Scene Flow <-> Eye | Entry/exit lifecycle and optional transition modes | No |
| Pulse | Separate bridge | Integration package | State -> Eye | Pause/time policy, cutscene/dialogue mode requests | No |
| Will | Project adapter/bridge | Project/bridge | Input -> Eye | Semantic manual-look/recenter intent | No |
| Looking Glass | Separate bridge | Integration package | Eye -> UI | Viewport/safe-area/status data | No |
| Impact | Separate bridge | Integration package | Feedback -> Eye | Semantic impulse request | No |
| Voices | Separate bridge/project adapter | Integration package | Dialogue -> Eye | Speaker/group shot leases | No |
| Hand | Project adapter | Project | Interaction -> Eye | Aim/focus camera requests | No |
| Fellowship | Separate bridge | Integration package | Characters -> Eye | Target ownership/switch/warp | No |
| Vessel | Project adapter | Project | Controller -> Eye | Velocity/facing/look-ahead snapshots | No |
| Cinemachine | Provider adapter package | EchoCamera provider repo | Eye -> backend | Neutral state mapped to Cinemachine | No |

### 17.3 Bridge placement decision

- Cross-package behavior ships in separate bridge packages when it references both packages.
- Cinemachine ships as a separate provider adapter with a real package dependency.
- Small game-specific shot logic remains project adapter code.
- The built-in Unity Camera backend ships with EchoCamera because it depends only on Unity and proves standalone behavior.

### 17.4 Integration failure behavior

Missing peers change nothing. Missing bridges expose no partial behavior. Version mismatch disables only the bridge/adapter and reports a compatibility error. Bridge teardown releases every lease/registration it owns before package removal or peer shutdown. Peer failure never transfers camera authority.

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Planned target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Main-channel evaluation | Stable, allocation-free steady state | Profiler/Camera Lab | Measured before release |
| Target/group evaluation | Bounded by configured member limits | Group stress Lab | Measured before release |
| Zone reconciliation | Bounded occupants/zones and cadence | 2D/3D zone stress Labs | Measured before release |
| Blend/modifier/impulse evaluation | No unbounded per-frame collection growth | Profiler | Measured before release |
| Backend application | One authoritative write path per channel/tick | Profiler/backend diagnostics | Measured before release |
| Diagnostic overhead | Configurable and bounded | Observatory/Profiler | Measured before release |

No numerical performance claim is approved before implementation evidence exists.

### 18.2 Allocation policy

- No LINQ, reflection scanning or avoidable allocations in per-frame evaluation.
- Reuse bounded buffers for targets, groups, modifiers, zones, bounds and impulses.
- Snapshots are immutable and intentionally copied only at documented publication points.
- Debug gizmos and verbose histories are Editor/development features.
- Provider exceptions are isolated without allocating unbounded error records.

### 18.3 Scene and domain reload behavior

Every registration and lease has explicit teardown. Static convenience state resets through subsystem-registration hooks. Enter Play Mode without domain reload must not retain authority generations or scene references. Backends unsubscribe and release before root invalidation. Scene unload removes scene-owned targets, zones, bounds and rigs.

### 18.4 Scalability limits

Configuration declares maximum channels, target registrations, group members, mode leases, modifiers, bounds requests, zones, impulses, histories and queued events. Exceeding a limit returns a structured rejection or documented replacement result; it never grows silently without bound.

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

EchoCamera does not require credentials, analytics, network identity or personal data. Diagnostic exports may reveal project IDs, scene IDs, backend versions and camera configuration summaries, so release/support exports are explicit and redacted.

### 19.2 Trust boundaries

Project-authored assets, custom backends, bridges and providers are untrusted extension points. The core validates capabilities, limits, stable IDs, null/invalid snapshots, viewport ranges, lens ranges and exception boundaries. Multiplayer adapters must validate camera ownership locally but cannot treat a local camera request as proof of gameplay authority.

### 19.3 Platform behavior

| Platform | Supported status | Special behavior | Validation required |
|---|---|---|---|
| Windows | Planned | Built-in backend and Editor tools | Player and Editor evidence |
| macOS | Planned | Built-in backend and Editor tools | Player and Editor evidence |
| Linux | Planned | Built-in backend and Editor tools | Player and Editor evidence |
| WebGL | Planned/conditional | Backend and performance constraints | Player evidence |
| Mobile | Planned/conditional | Aspect ratios, safe areas, orientation, performance | Device evidence |
| Console | Unknown/planned later | Platform certification and viewport rules | Authorized hardware evidence |
| XR | Unsupported MVP | Requires dedicated camera authority/provider research | Future specification |

Support claims remain `Planned` or `Unknown` until SFGSS-004 evidence exists.

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-camera/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
├── Runtime/
│   ├── Core/
│   ├── Data/
│   ├── Configuration/
│   ├── Backends/UnityCamera/
│   ├── Zones/Physics2D/
│   ├── Zones/Physics3D/
│   ├── Diagnostics/
│   └── Prefabs/
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Inspectors/
│   ├── Monitoring/
│   └── Migration/
├── Samples~/
│   ├── Standalone Labs/Eye Camera 2D Laboratory/
│   └── Standalone Labs/Eye Camera 3D Laboratory/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Proposed source tree

```text
Runtime/Core
├── EchoCameraRoot.cs
├── IEchoCameraService.cs
├── CameraChannelRuntime.cs
├── CameraArbitrator.cs
├── CameraBlendRuntime.cs
├── CameraTargetRegistry.cs
├── CameraGroupRegistry.cs
├── CameraModifierRegistry.cs
├── CameraBoundsRegistry.cs
└── CameraImpulseRuntime.cs
Runtime/Data
├── CameraChannelId.cs
├── CameraModeRequest.cs
├── CameraStateSnapshot.cs
├── CameraResult.cs
└── HandleTypes.cs
Runtime/Backends/UnityCamera
├── UnityCameraBackend.cs
├── UnityCameraBackendConfiguration.cs
└── UnityCameraPoseApplier.cs
Runtime/Zones
├── Physics2D/EchoCameraZone2D.cs
└── Physics3D/EchoCameraZone3D.cs
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoCamera.Runtime` | Runtime | Unity core modules only | Yes | Neutral channel/state/API core |
| `EchoDevGames.EchoCamera.Backend.UnityCamera` | Runtime | EchoCamera Runtime, Unity Camera modules | Yes | Built-in standalone backend |
| `EchoDevGames.EchoCamera.Zones.Physics2D` | Runtime | Runtime, Unity Physics2D | No | Optional 2D zone/bounds adapters |
| `EchoDevGames.EchoCamera.Zones.Physics3D` | Runtime | Runtime, Unity Physics | No | Optional 3D zone/bounds adapters |
| `EchoDevGames.EchoCamera.Editor` | Editor | Runtime/backend/zone assemblies, UnityEditor | No | Setup, validation, inspectors, migration |
| `EchoDevGames.EchoCamera.Tests.Editor` | Editor test | Editor + Runtime | No | EditMode/tool tests |
| `EchoDevGames.EchoCamera.Tests.Runtime` | Runtime test | Runtime/backend | No | PlayMode/runtime tests |

The Cinemachine adapter is a separate package and assembly, tentatively `EchoDevGames.EchoCamera.Cinemachine`, with real manifest/version dependencies under SFGSS-002.

### 20.4 Repository files

The repository includes a routed README, documentation index, API/lifecycle/backend guides, Current Notes, ADRs, checkpoints, test reports, changelog, license, third-party notices, security/support guidance, release checklist, stable `.meta` files and compatibility records.

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Planned/tested | Notes |
|---|---|---|---|
| Unity | 6000.0 | Primary baseline 6000.3.8f1; Not run | Reverify before implementation/release |
| Built-in Camera API | Unity 6000 modules | Not run | Required for built-in backend |
| Cinemachine adapter | Planned Cinemachine 3.1.7 | Not run | Separate optional package |

### 21.2 Semantic versioning policy

- Patch: fixes with no public API, ID, serialized schema or behavior-contract break.
- Minor: backward-compatible APIs, capabilities, profiles, validators or adapters.
- Major: breaking API/assembly/package changes, incompatible schema changes, stable-ID semantics, arbitration rules, tick ownership or backend contracts.

### 21.3 Deprecation policy

Deprecated APIs/assets receive warnings, migration guidance and at least one documented compatibility period unless safety requires faster removal. Removal requires changelog, migration notes, replacement examples and regression tests.

### 21.4 GUID and asset compatibility

Public scripts, prefabs, templates, profiles, Laboratories and samples preserve `.meta` GUIDs when identity is intended to survive. Moves/renames retain GUIDs. Domain IDs remain separate from Unity GUIDs.

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview, ownership and non-goals.
- Installation routes and five-minute built-in-backend quick start.
- Boot and direct-scene setup.
- Targets, groups, modes, blends, modifiers, bounds, zones and impulses.
- Built-in backend guide.
- 2D/3D Laboratory guides.
- Diagnostics and `ECAM-*` reference.
- Accessibility and reduced-motion policy.
- Migration, troubleshooting, known limitations and removal.
- Cinemachine adapter guide when released.

### 22.2 Required developer documentation

- Authority/channel lifecycle.
- Arbitration and blend mathematics/contracts.
- Target/group snapshot contract.
- Backend capability and tick ownership contract.
- Bounds/zone/impulse extension points.
- Bridge specifications.
- Testing/release workflow, ADRs, Current Notes and checkpoint status.

### 22.3 Documentation truth rule

Examples must compile against the documented release. Screenshots, menu paths, dependency versions, backend mappings, performance numbers, compatibility claims and test statuses must match executed evidence. Planned behavior is never described as implemented.

### 22.4 Living repository and Obsidian workflow

Current discoveries enter `Current Notes.md`, then durable decisions move into this specification, ADRs, bridge specs, issue/test records, guides or changelog at checkpoint closeout. Git is the archive; Obsidian opens the same files directly.

### 22.5 Repository scan and handoff order

1. README/index.
2. SFGSS-000.
3. SFGSS-002 through SFGSS-005.
4. This specification.
5. Applicable ADRs and bridge/backend specifications.
6. Current Notes, checkpoint, tests, issues and changelog.
7. Relevant implementation and automated tests after code exists.

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, definitions, arbitration, validation, migrations | Priority ties, aliases, clamps, capability negotiation | Yes |
| PlayMode unit/integration | Root, channels, targets, leases, blends, backends | Duplicate protection, teardown, target loss, impulses | Yes |
| Standalone Laboratories | User-visible isolated camera workflows | 2D and 3D Labs | Yes |
| Bridge Integration Laboratory | One explicit connection | Impact, Voices, Fellowship, Cinemachine | When bridge ships |
| Showcase | Combined presentation | Multi-system demo | No |
| Clean-project installation | Packaging and independence | Git/local/tarball/import/removal | Yes |
| Existing-project migration | Adoption without regressions | Rescuers2D/Systems Lab target | Before adoption claim |

### 23.2 Required test categories

Happy path, missing/invalid configuration, duplicate authority, channel lifecycle, target loss, blends, modifiers, bounds, zones, impulses, backend capability mismatch, direct-scene entry, scene unload, optional integration absent/present, no-domain-reload behavior, sample removal, package removal, migrations, performance, accessibility, platform, privacy and release evidence all follow SFGSS-004.

### 23.3 Planned test registry

Every record below is a planned definition. Execution status remains **Not run** until implementation exists and evidence is captured.

| Test ID | Category | Requirement/action | Expected result | Automated? | Status |
|---|---|---|---|---:|---|
| ECAM-T-001 | Installation and assembly | Validate clean Git installation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-002 | Installation and assembly | Validate local-path installation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-003 | Installation and assembly | Validate tarball installation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-004 | Installation and assembly | Validate embedded development installation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-005 | Installation and assembly | Validate package removal | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-006 | Installation and assembly | Validate package reinstall | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-007 | Installation and assembly | Validate runtime assembly isolation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-008 | Installation and assembly | Validate Editor assembly isolation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-009 | Installation and assembly | Validate test assembly isolation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-010 | Installation and assembly | Validate sample assembly isolation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-011 | Installation and assembly | Validate no UnityEditor reference in Runtime | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-012 | Installation and assembly | Validate no Cinemachine reference in core | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-013 | Installation and assembly | Validate Physics2D adapter optionality | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-014 | Installation and assembly | Validate Physics3D adapter optionality | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-015 | Installation and assembly | Validate built-in backend standalone compile | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-016 | Installation and assembly | Validate missing optional adapter compile | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-017 | Installation and assembly | Validate package manifest validation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-018 | Installation and assembly | Validate stable meta files | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-019 | Installation and assembly | Validate documentation route validation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-020 | Installation and assembly | Validate Workshop facade absence safety | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-021 | Installation and assembly | Validate Workshop facade plan generation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-022 | Installation and assembly | Validate duplicate package version rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-023 | Installation and assembly | Validate dependency version mismatch report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-024 | Installation and assembly | Validate sample import | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-025 | Installation and assembly | Validate sample removal | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-026 | Installation and assembly | Validate domain reload compilation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-027 | Installation and assembly | Validate no-domain-reload compilation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-028 | Installation and assembly | Validate build-player compilation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-029 | Installation and assembly | Validate package upgrade from prior fixture | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-030 | Installation and assembly | Validate uninstall bridge-first guidance | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-031 | Lifecycle and channels | Validate first root claim | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-032 | Lifecycle and channels | Validate duplicate root before readiness | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-033 | Lifecycle and channels | Validate duplicate root after readiness | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-034 | Lifecycle and channels | Validate root shutdown | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-035 | Lifecycle and channels | Validate root restart in development | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-036 | Lifecycle and channels | Validate Main channel creation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-037 | Lifecycle and channels | Validate secondary channel creation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-038 | Lifecycle and channels | Validate duplicate channel ID | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-039 | Lifecycle and channels | Validate channel removal | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-040 | Lifecycle and channels | Validate channel recreation generation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-041 | Lifecycle and channels | Validate backend bind | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-042 | Lifecycle and channels | Validate backend unbind | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-043 | Lifecycle and channels | Validate safe backend replacement | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-044 | Lifecycle and channels | Validate backend failure isolation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-045 | Lifecycle and channels | Validate root-driven tick | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-046 | Lifecycle and channels | Validate backend-driven tick | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-047 | Lifecycle and channels | Validate ambiguous tick rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-048 | Lifecycle and channels | Validate scene unload cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-049 | Lifecycle and channels | Validate application quit cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-050 | Lifecycle and channels | Validate focus loss and regain | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-051 | Lifecycle and channels | Validate direct-scene initializer absent root | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-052 | Lifecycle and channels | Validate direct-scene initializer existing root | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-053 | Lifecycle and channels | Validate release exclusion of initializer | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-054 | Lifecycle and channels | Validate static reset no-domain-reload | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-055 | Lifecycle and channels | Validate channel capacity rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-056 | Lifecycle and channels | Validate event listener exception isolation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-057 | Lifecycle and channels | Validate channel snapshot immutability | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-058 | Lifecycle and channels | Validate configuration missing | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-059 | Lifecycle and channels | Validate baseline missing | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-060 | Lifecycle and channels | Validate deterministic reset | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-061 | Modes and blends | Validate baseline mode resolution | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-062 | Modes and blends | Validate single temporary mode | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-063 | Modes and blends | Validate priority winner | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-064 | Modes and blends | Validate equal-priority later winner | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-065 | Modes and blends | Validate out-of-order release | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-066 | Modes and blends | Validate stale mode lease | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-067 | Modes and blends | Validate foreign mode lease | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-068 | Modes and blends | Validate mode capacity rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-069 | Modes and blends | Validate mode replacement policy | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-070 | Modes and blends | Validate unsupported mode intent | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-071 | Modes and blends | Validate cut transition | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-072 | Modes and blends | Validate timed blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-073 | Modes and blends | Validate unscaled blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-074 | Modes and blends | Validate scaled blend policy | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-075 | Modes and blends | Validate blend curve evaluation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-076 | Modes and blends | Validate blend interruption | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-077 | Modes and blends | Validate repeated interruption | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-078 | Modes and blends | Validate reduced-motion cut | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-079 | Modes and blends | Validate reduced-motion shortened blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-080 | Modes and blends | Validate zero-duration blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-081 | Modes and blends | Validate target-loss during blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-082 | Modes and blends | Validate backend failure during blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-083 | Modes and blends | Validate scene unload during blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-084 | Modes and blends | Validate mode owner destroyed | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-085 | Modes and blends | Validate mode lease double release | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-086 | Modes and blends | Validate baseline restored from active truth | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-087 | Modes and blends | Validate no stale-state restoration | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-088 | Modes and blends | Validate blend terminal event once | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-089 | Modes and blends | Validate blend history bounded | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-090 | Modes and blends | Validate mode diagnostics | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-091 | Targets and groups | Validate register target | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-092 | Targets and groups | Validate release target | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-093 | Targets and groups | Validate stale target handle | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-094 | Targets and groups | Validate foreign target handle | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-095 | Targets and groups | Validate disabled target | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-096 | Targets and groups | Validate destroyed target | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-097 | Targets and groups | Validate invalid target snapshot | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-098 | Targets and groups | Validate target warp notification | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-099 | Targets and groups | Validate repeated warp notification | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-100 | Targets and groups | Validate target grace success | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-101 | Targets and groups | Validate target grace expiry | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-102 | Targets and groups | Validate fallback target | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-103 | Targets and groups | Validate baseline on target loss | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-104 | Targets and groups | Validate block on target loss | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-105 | Targets and groups | Validate hold-last-safe pose | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-106 | Targets and groups | Validate target capacity rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-107 | Targets and groups | Validate target scene cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-108 | Targets and groups | Validate target owner cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-109 | Targets and groups | Validate weighted group creation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-110 | Targets and groups | Validate group member add | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-111 | Targets and groups | Validate group member remove | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-112 | Targets and groups | Validate group empty policy | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-113 | Targets and groups | Validate invalid group member | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-114 | Targets and groups | Validate duplicate group member | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-115 | Targets and groups | Validate group weight normalization | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-116 | Targets and groups | Validate group member capacity | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-117 | Targets and groups | Validate group stale handle | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-118 | Targets and groups | Validate group churn | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-119 | Targets and groups | Validate group framing bounds | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-120 | Targets and groups | Validate target diagnostic redaction | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-121 | Modifiers and impulses | Validate single offset modifier | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-122 | Modifiers and impulses | Validate stacked additive offsets | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-123 | Modifiers and impulses | Validate priority override modifier | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-124 | Modifiers and impulses | Validate multiplicative zoom modifier | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-125 | Modifiers and impulses | Validate lens clamp | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-126 | Modifiers and impulses | Validate look-ahead clamp | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-127 | Modifiers and impulses | Validate dead-zone override | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-128 | Modifiers and impulses | Validate manual-look adapter intent | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-129 | Modifiers and impulses | Validate modifier out-of-order release | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-130 | Modifiers and impulses | Validate stale modifier lease | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-131 | Modifiers and impulses | Validate modifier capacity rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-132 | Modifiers and impulses | Validate modifier owner cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-133 | Modifiers and impulses | Validate modifier scene cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-134 | Modifiers and impulses | Validate single impulse | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-135 | Modifiers and impulses | Validate multiple impulses bounded | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-136 | Modifiers and impulses | Validate impulse amplitude clamp | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-137 | Modifiers and impulses | Validate impulse duration clamp | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-138 | Modifiers and impulses | Validate impulse frequency clamp | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-139 | Modifiers and impulses | Validate impulse priority | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-140 | Modifiers and impulses | Validate impulse replacement | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-141 | Modifiers and impulses | Validate impulse cancellation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-142 | Modifiers and impulses | Validate stale impulse handle | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-143 | Modifiers and impulses | Validate impulse completion once | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-144 | Modifiers and impulses | Validate impulse during blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-145 | Modifiers and impulses | Validate impulse during pause | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-146 | Modifiers and impulses | Validate reduced-motion impulse scaling | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-147 | Modifiers and impulses | Validate Impact bridge mapping | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-148 | Modifiers and impulses | Validate provider failure during impulse | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-149 | Modifiers and impulses | Validate impulse history bounded | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-150 | Modifiers and impulses | Validate definition immutability | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-151 | Bounds and zones | Validate single bounds request | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-152 | Bounds and zones | Validate bounds priority winner | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-153 | Bounds and zones | Validate bounds equal-priority tie | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-154 | Bounds and zones | Validate bounds out-of-order release | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-155 | Bounds and zones | Validate stale bounds lease | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-156 | Bounds and zones | Validate required bounds unavailable | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-157 | Bounds and zones | Validate optional bounds fallback | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-158 | Bounds and zones | Validate 2D bounds provider | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-159 | Bounds and zones | Validate 3D bounds provider | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-160 | Bounds and zones | Validate invalid bounds geometry | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-161 | Bounds and zones | Validate bounds capacity rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-162 | Bounds and zones | Validate bounds scene cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-163 | Bounds and zones | Validate zone enter | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-164 | Bounds and zones | Validate zone exit | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-165 | Bounds and zones | Validate overlapping zones | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-166 | Bounds and zones | Validate out-of-order zone exit | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-167 | Bounds and zones | Validate zone disable while occupied | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-168 | Bounds and zones | Validate zone destroy while occupied | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-169 | Bounds and zones | Validate teleport occupancy reconciliation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-170 | Bounds and zones | Validate multi-collider zone dedupe | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-171 | Bounds and zones | Validate zone priority | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-172 | Bounds and zones | Validate zone mode lease | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-173 | Bounds and zones | Validate zone modifier lease | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-174 | Bounds and zones | Validate zone bounds lease | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-175 | Bounds and zones | Validate zone owner cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-176 | Bounds and zones | Validate zone capacity rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-177 | Bounds and zones | Validate Physics2D adapter removed | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-178 | Bounds and zones | Validate Physics3D adapter removed | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-179 | Bounds and zones | Validate zone debug disabled release | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-180 | Bounds and zones | Validate bounds diagnostic snapshot | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-181 | Backends and rendering | Validate built-in orthographic follow | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-182 | Backends and rendering | Validate built-in perspective follow | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-183 | Backends and rendering | Validate built-in viewport application | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-184 | Backends and rendering | Validate normalized viewport clamp | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-185 | Backends and rendering | Validate built-in lens application | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-186 | Backends and rendering | Validate built-in pose application | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-187 | Backends and rendering | Validate one write per tick | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-188 | Backends and rendering | Validate backend capability report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-189 | Backends and rendering | Validate unsupported capability result | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-190 | Backends and rendering | Validate explicit capability fallback | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-191 | Backends and rendering | Validate backend initialization failure | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-192 | Backends and rendering | Validate backend apply exception | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-193 | Backends and rendering | Validate backend teardown exception | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-194 | Backends and rendering | Validate backend version report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-195 | Backends and rendering | Validate custom backend registration | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-196 | Backends and rendering | Validate custom backend removal | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-197 | Backends and rendering | Validate fake Cinemachine mapping | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-198 | Backends and rendering | Validate Cinemachine adapter absent | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-199 | Backends and rendering | Validate adapter mismatch | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-200 | Backends and rendering | Validate adapter core isolation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-201 | Backends and rendering | Validate backend state snapshot | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-202 | Backends and rendering | Validate backend allocation steady state | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-203 | Backends and rendering | Validate Camera reference destroyed | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-204 | Backends and rendering | Validate Camera disabled | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-205 | Backends and rendering | Validate multiple Camera outputs | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-206 | Backends and rendering | Validate viewport overlap allowed policy | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-207 | Backends and rendering | Validate viewport invalid rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-208 | Backends and rendering | Validate render pipeline independence | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-209 | Backends and rendering | Validate post-processing non-ownership | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-210 | Backends and rendering | Validate backend diagnostics bounded | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-211 | Scenes and integrations | Validate Boot scene startup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-212 | Scenes and integrations | Validate direct scene startup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-213 | Scenes and integrations | Validate scene target registration | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-214 | Scenes and integrations | Validate scene zone registration | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-215 | Scenes and integrations | Validate scene unload target cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-216 | Scenes and integrations | Validate scene unload zone cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-217 | Scenes and integrations | Validate Passage transition handoff | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-218 | Scenes and integrations | Validate First Light initialization bridge | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-219 | Scenes and integrations | Validate Pulse pause bridge | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-220 | Scenes and integrations | Validate Accord reduced-motion bridge | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-221 | Scenes and integrations | Validate Will manual-look bridge | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-222 | Scenes and integrations | Validate Looking Glass viewport bridge | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-223 | Scenes and integrations | Validate Impact impulse bridge | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-224 | Scenes and integrations | Validate Voices dialogue-shot bridge | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-225 | Scenes and integrations | Validate Hand focus adapter | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-226 | Scenes and integrations | Validate Fellowship target handoff | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-227 | Scenes and integrations | Validate Vessel velocity adapter | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-228 | Scenes and integrations | Validate Observatory snapshot bridge | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-229 | Scenes and integrations | Validate bridge absent behavior | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-230 | Scenes and integrations | Validate bridge removal cleanup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-231 | Scenes and integrations | Validate bridge version mismatch | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-232 | Scenes and integrations | Validate bridge exception isolation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-233 | Scenes and integrations | Validate project adapter injection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-234 | Scenes and integrations | Validate multiple bridges same channel | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-235 | Scenes and integrations | Validate temporary shot release on dialogue end | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-236 | Scenes and integrations | Validate character switch warp | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-237 | Scenes and integrations | Validate scene change while mode active | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-238 | Scenes and integrations | Validate scene change while impulse active | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-239 | Scenes and integrations | Validate integration sample removal | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-240 | Scenes and integrations | Validate core after all bridges removed | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-241 | Diagnostics and validation | Validate root health report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-242 | Diagnostics and validation | Validate channel health report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-243 | Diagnostics and validation | Validate backend health report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-244 | Diagnostics and validation | Validate mode contender report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-245 | Diagnostics and validation | Validate target validity report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-246 | Diagnostics and validation | Validate group report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-247 | Diagnostics and validation | Validate modifier report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-248 | Diagnostics and validation | Validate bounds report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-249 | Diagnostics and validation | Validate zone report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-250 | Diagnostics and validation | Validate blend report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-251 | Diagnostics and validation | Validate impulse report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-252 | Diagnostics and validation | Validate saturation report | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-253 | Diagnostics and validation | Validate redacted export | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-254 | Diagnostics and validation | Validate no hierarchy path leak | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-255 | Diagnostics and validation | Validate no screenshot export | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-256 | Diagnostics and validation | Validate no per-frame log spam | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-257 | Diagnostics and validation | Validate diagnostic rate limit | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-258 | Diagnostics and validation | Validate history pruning | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-259 | Diagnostics and validation | Validate ECAM code uniqueness | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-260 | Diagnostics and validation | Validate validator missing root | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-261 | Diagnostics and validation | Validate validator duplicate root | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-262 | Diagnostics and validation | Validate validator duplicate ID | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-263 | Diagnostics and validation | Validate validator missing backend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-264 | Diagnostics and validation | Validate validator capability mismatch | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-265 | Diagnostics and validation | Validate validator unsafe limits | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-266 | Diagnostics and validation | Validate validator Cinemachine core leak | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-267 | Diagnostics and validation | Validate validator tick ambiguity | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-268 | Diagnostics and validation | Validate validator released ID change | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-269 | Diagnostics and validation | Validate repair repeat run | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-270 | Diagnostics and validation | Validate report deterministic ordering | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-271 | Performance and stress | Validate one-channel steady-state profile | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-272 | Performance and stress | Validate multi-channel bounded profile | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-273 | Performance and stress | Validate target capacity stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-274 | Performance and stress | Validate group member stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-275 | Performance and stress | Validate mode churn stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-276 | Performance and stress | Validate modifier churn stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-277 | Performance and stress | Validate bounds churn stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-278 | Performance and stress | Validate zone occupancy stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-279 | Performance and stress | Validate impulse concurrency stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-280 | Performance and stress | Validate blend interruption stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-281 | Performance and stress | Validate target destruction storm | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-282 | Performance and stress | Validate scene unload storm | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-283 | Performance and stress | Validate backend replacement stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-284 | Performance and stress | Validate diagnostic export stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-285 | Performance and stress | Validate no steady-state LINQ | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-286 | Performance and stress | Validate no reflection hot path | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-287 | Performance and stress | Validate no unbounded collection growth | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-288 | Performance and stress | Validate buffer reuse | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-289 | Performance and stress | Validate snapshot allocation budget | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-290 | Performance and stress | Validate event listener load | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-291 | Performance and stress | Validate fake provider exception storm | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-292 | Performance and stress | Validate low frame-rate blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-293 | Performance and stress | Validate high frame-rate blend | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-294 | Performance and stress | Validate time-scale zero behavior | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-295 | Performance and stress | Validate long session history bounds | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-296 | Performance and stress | Validate multiple viewport stress | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-297 | Performance and stress | Validate direct-scene repeated entry | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-298 | Performance and stress | Validate no-domain-reload repeated play | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-299 | Performance and stress | Validate shutdown under load | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-300 | Performance and stress | Validate performance evidence remains Not run pre-code | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-301 | Data migration and removal | Validate configuration schema current | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-302 | Data migration and removal | Validate configuration older migration | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-303 | Data migration and removal | Validate configuration newer rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-304 | Data migration and removal | Validate mode ID duplicate | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-305 | Data migration and removal | Validate mode ID alias | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-306 | Data migration and removal | Validate bounds ID alias | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-307 | Data migration and removal | Validate zone ID alias | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-308 | Data migration and removal | Validate channel ID alias | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-309 | Data migration and removal | Validate released ID change rejection | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-310 | Data migration and removal | Validate asset GUID preserved on move | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-311 | Data migration and removal | Validate domain ID independent from asset GUID | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-312 | Data migration and removal | Validate definition runtime immutability | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-313 | Data migration and removal | Validate preview does not mutate source | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-314 | Data migration and removal | Validate migration backup | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-315 | Data migration and removal | Validate migration dry run | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-316 | Data migration and removal | Validate migration interruption | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-317 | Data migration and removal | Validate migration rollback | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-318 | Data migration and removal | Validate unknown field preservation policy | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-319 | Data migration and removal | Validate invalid migration chain | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-320 | Data migration and removal | Validate removed backend mapping | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-321 | Data migration and removal | Validate removed target provider | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-322 | Data migration and removal | Validate removed bridge package | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-323 | Data migration and removal | Validate removed zone adapter | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-324 | Data migration and removal | Validate removed samples | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-325 | Data migration and removal | Validate removed package root | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-326 | Data migration and removal | Validate reinstall preserves project assets | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-327 | Data migration and removal | Validate clean removal compile | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-328 | Data migration and removal | Validate upgrade compatibility fixture | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-329 | Data migration and removal | Validate changelog migration entry | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-330 | Data migration and removal | Validate removal documentation | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-331 | Platform accessibility and release | Validate Windows planned matrix | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-332 | Platform accessibility and release | Validate macOS planned matrix | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-333 | Platform accessibility and release | Validate Linux planned matrix | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-334 | Platform accessibility and release | Validate WebGL conditional matrix | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-335 | Platform accessibility and release | Validate mobile portrait viewport | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-336 | Platform accessibility and release | Validate mobile landscape viewport | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-337 | Platform accessibility and release | Validate mobile safe-area handoff | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-338 | Platform accessibility and release | Validate console unknown claim | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-339 | Platform accessibility and release | Validate XR unsupported MVP claim | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-340 | Platform accessibility and release | Validate orthographic platform consistency | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-341 | Platform accessibility and release | Validate perspective platform consistency | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-342 | Platform accessibility and release | Validate reduced-motion blend policy | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-343 | Platform accessibility and release | Validate reduced-motion impulse policy | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-344 | Platform accessibility and release | Validate color-independent diagnostics | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-345 | Platform accessibility and release | Validate manual-look sensitivity seam | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-346 | Platform accessibility and release | Validate recenter seam | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-347 | Platform accessibility and release | Validate pause accessibility policy | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-348 | Platform accessibility and release | Validate split-screen readable metadata | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-349 | Platform accessibility and release | Validate beta specification gate | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-350 | Platform accessibility and release | Validate beta implementation gate | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-351 | Platform accessibility and release | Validate beta standalone gate | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-352 | Platform accessibility and release | Validate release-candidate clean install | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-353 | Platform accessibility and release | Validate release-candidate upgrade | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-354 | Platform accessibility and release | Validate release-candidate removal | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-355 | Platform accessibility and release | Validate stable docs truth | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-356 | Platform accessibility and release | Validate stable license notices | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-357 | Platform accessibility and release | Validate stable performance evidence | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-358 | Platform accessibility and release | Validate stable platform evidence | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-359 | Platform accessibility and release | Validate stable tarball evidence | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |
| ECAM-T-360 | Platform accessibility and release | Validate stable release receipt | Behavior matches this specification with structured failure and no hidden ownership | Planned mix | Not run |

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and exclusions align with SFGSS-000.
- [x] MVP and deferred scope are separated.
- [x] Core/backend/adapter boundaries follow SFGSS-002.
- [x] IDs and runtime state follow SFGSS-003.
- [x] Laboratories and planned evidence follow SFGSS-004.
- [x] Release-blocking architecture questions are resolved.

### 24.2 Implementation gate

- [ ] Runtime assemblies compile with declared dependencies only.
- [ ] Built-in backend functions without Cinemachine.
- [ ] Duplicate protection, channels, leases and lifecycle pass automated tests.
- [ ] Setup/repair/migration are repeatable and non-destructive.
- [ ] Public APIs match this specification or an approved revision/ADR.
- [ ] All implementation is delivered through the teaching-oriented SFGSS-005 workflow.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] 2D and 3D Laboratories pass without unrelated Echo packages.
- [ ] Direct-scene entry behaves as documented.
- [ ] Samples and optional adapters can be removed safely.
- [ ] Package remains useful with every bridge absent.

### 24.4 Quality gate

- [ ] Required automated and manual tests pass with evidence.
- [ ] No Blocker or Critical defect remains.
- [ ] Performance budgets are measured and pass approved thresholds.
- [ ] Reduced-motion and safety policies pass.
- [ ] Diagnostics are actionable and privacy-safe.
- [ ] Documentation matches the shipped API and Unity paths.
- [ ] Current Notes are reconciled and durable decisions promoted.
- [ ] License and notices are complete.

### 24.5 Distribution gate

- [ ] Manifest and semantic version are correct.
- [ ] Stable `.meta` files are included.
- [ ] Git/local/tarball installation claims have executed evidence.
- [ ] Upgrade and removal paths are tested.
- [ ] Repository release/tag and compatibility catalog are prepared.
- [ ] Optional Cinemachine adapter has its own tested compatibility record before being advertised.

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Echo Systems Lab | Project-owned target/follow camera behavior | Install Eye standalone, map one follow workflow, then replace one camera path | Original target/follow behavior plus diagnostics | Re-enable original scripts/prefabs |
| Rescuers2D | Shared active-character and room camera behavior | Bridge active character, add zones/bounds, then feedback impulses | Character switching, bounds and direct-scene parity | Keep original camera controller disabled but intact |
| Don't Get Vince'd | Beat-'em-up framing and combat feedback | Map zone/group framing and Impact bridge incrementally | Encounter framing and hit-response parity | Restore original controller/zone scripts |
| Hackulos | Future top-down follow/dialogue/group framing | Adopt after core package proves standalone | Approved vertical-slice camera checklist | Remove bridge/package and restore project adapter |

### 25.2 Preserve-until-parity rule

Existing camera systems remain available until The Eye passes standalone evidence and one feature category at a time passes in the real project. No bulk deletion, prefab overwrite, scene replacement or Cinemachine conversion occurs before parity and rollback evidence.

### 25.3 Migration tooling

Migration tooling may detect known project patterns, preview target/mode/bounds mappings, create project-owned assets, preserve backups, generate a receipt, validate duplicate writers and support rollback. It must never silently disable or delete existing camera scripts, rigs, virtual cameras, confiners, zones or project data.

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| ECAM-R-001 | Scope expands into universal cinematography | High | High | MVP capability list and deferred modules | Spec review/Jesse |
| ECAM-R-002 | Two systems write one Camera | Medium | Critical | One channel/backend tick owner and validator | Implementation owner |
| ECAM-R-003 | Cinemachine leaks into core | Medium | High | Separate provider package and assembly validation | Package maintainer |
| ECAM-R-004 | Temporary modes restore stale state | Medium | High | Lease arbitration recomputes from active truth | Runtime tests |
| ECAM-R-005 | Target/zone lifecycle leaks | High | High | Generational handles, owner/scene teardown, reconciliation | Runtime tests |
| ECAM-R-006 | Motion causes accessibility problems | Medium | High | Reduced-motion alternatives and impulse safety limits | Accessibility review |
| ECAM-R-007 | Group/zone/modifier growth hurts performance | Medium | Medium | Bounded capacities, buffers and stress tests | Performance gate |
| ECAM-R-008 | Backend capability assumptions diverge | Medium | High | Capability negotiation and explicit fallback | Adapter tests |
| ECAM-R-009 | Viewport/split-screen scope inflates MVP | Medium | Medium | Bounded channel model; advanced orchestration deferred | Milestone review |
| ECAM-R-010 | Existing-project migration breaks shots | Medium | High | Preserve-until-parity and reversible adoption | Integration owner |
| ECAM-R-011 | Asset or domain IDs break references | Low | High | SFGSS-003 validation, aliases and GUID preservation | Release gate |
| ECAM-R-012 | Samples become production dependencies | Low | High | Assembly/package isolation and removal tests | SFGSS-004 gate |

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| ECAM-D-001 | Camera channels are the independent output authority | Approved | Supports Main now and bounded multi-view later | No single-camera-only API | No |
| ECAM-D-002 | Core is provider-neutral and ships a built-in Unity Camera backend | Approved | Guarantees standalone usefulness | Backend contract must remain stable | No |
| ECAM-D-003 | Cinemachine is a separate provider adapter | Approved | Prevents optional dependency leakage | Separate package/version evidence | No |
| ECAM-D-004 | Modes, modifiers and bounds use generational leases | Approved | Safe nested/out-of-order lifetime | Callers retain/release their own handles | No |
| ECAM-D-005 | Losing modes remain latent and effective state is recomputed | Approved | Prevents stale restoration | Arbitration must be deterministic | No |
| ECAM-D-006 | Blend interruption starts from current evaluated output | Approved | Prevents snapping to stale source state | Backends expose/evaluate current state | No |
| ECAM-D-007 | Backends declare root-driven or backend-driven tick ownership | Approved | Prevents double Camera writes | Validators block ambiguity | No |
| ECAM-D-008 | Targets/groups use snapshots and warp revisions | Approved | Handles destruction, teleport and controller changes | Target providers stay lightweight | No |
| ECAM-D-009 | One effective bounds request per channel in MVP | Approved | Clear authority and bounded complexity | Composite bounds deferred |
| ECAM-D-010 | Impact requests impulses; The Eye owns final camera application | Approved | Preserves feedback and camera boundaries | Bridge required for composition | No |
| ECAM-D-011 | Runtime camera state is session-only | Approved | Handles/scene objects are not durable truth | Projects persist only stable preferences/intents | No |
| ECAM-D-012 | Two standalone Labs use the built-in backend | Approved | Proves package independence | Adapter Labs are separate evidence | No |

### 27.2 Release-blocking questions

None recorded. Exact Unity/Cinemachine version compatibility, performance budgets and platform support remain evidence questions for implementation and release, not unresolved architecture.

### 27.3 Non-blocking later questions

- Whether advanced split-screen layout orchestration belongs in The Eye or a multiplayer/UI bridge.
- Whether composite/intersecting bounds graduate beyond one winning request.
- Whether cutscene shot sequencing belongs in a future cinematic package.
- Which additional backends or render/XR adapters deserve supported provider packages.
- Whether photo mode becomes a separate package or an Eye/Pulse/UI composition.

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved package contract | Design only | This approved document |
| M1 - Skeleton | Installable package anatomy | Manifest, assemblies, docs shell | Clean compile/install |
| M2 - Core authority | Root, channels, IDs, snapshots, leases | Core API and tests | Automated lifecycle tests |
| M3 - Built-in backend | One standalone camera path | Follow, lens, viewport, blends | 2D/3D runtime tests |
| M4 - Targets and modes | Groups, modifiers, target loss/warp | Core useful workflow | Laboratory evidence |
| M5 - Bounds, zones and impulses | 2D/3D adapters and safety | Full MVP behavior | Stress/accessibility tests |
| M6 - Tooling and Labs | Setup, validation, monitor, samples | Authoring workflow | Repeatability/Lab evidence |
| M7 - Integrations | First bridges/provider adapter | Impact/Fellowship/Cinemachine candidates | Separate Integration Labs |
| M8 - Release | Distribution-ready version | Docs, migration, package | Clean external install and release gates |

### 28.2 Checkpoint rule

Each milestone is split into one-outcome Checkpoint Build Plans under SFGSS-005. When implementation eventually unlocks, every checkpoint supplies complete visible code, file paths, explanations, Unity setup, tests, expected results, failure fixes, rollback and a stop point so Jesse can implement and understand the system personally.

### 28.3 First recommended checkpoint

After SUITE-DOC-33 authorizes code: **ECAM-M1-01 - Package Skeleton**. Create only manifest, asmdefs, documentation shell and test assembly shells, then stop before runtime behavior.

## 29. New-Conversation Handoff

```text
We are continuing development of The Sperk's Forge - EchoDevGames Game Systems Suite.

Treat SFGSS-000 as suite authority. Treat SFGSS-002 through SFGSS-005 as the
standards for dependencies, data/IDs, testing/release, and checkpoint teaching.
Treat The Eye (EchoCamera) Specification v1.0.0 as the authority for camera
channels, targets/groups, modes, blends, modifiers, bounds, zones, impulses,
backend contracts, Laboratories, diagnostics and release gates.

Current package: EchoCamera / The Eye
Current specification: v1.0.0 Approved
Current implementation: Locked until SUITE-DOC-33
Current milestone/checkpoint: <CHECKPOINT>
Unity baseline: Unity 6000.3.8f1
Known blockers: <BLOCKERS>

Before writing code:
1. Reconcile Current Notes and the active checkpoint.
2. Preserve one camera authority and one backend tick owner per channel.
3. Keep Cinemachine and peer packages behind explicit adapters/bridges.
4. Keep runtime state out of definition assets and save payloads.
5. Show every complete code file and explain what it does and why.
6. Stop at the checkpoint boundary and record evidence honestly.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | Specification v1.0.0; implementation not started |
| Completed checkpoint | SUITE-DOC-14 package specification |
| Files/assets created | Documentation only |
| Tests passed | None; all planned tests Not run |
| Tests failed | None; implementation absent |
| Known issues | Exact dependency/performance/platform evidence pending implementation |
| Decisions added | ECAM-D-001 through ECAM-D-012 |
| Next checkpoint | SUITE-DOC-15 - The Fellowship (`EchoCharacters`) specification |

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and plain responsibility are clear.
- [x] Ownership/non-ownership align with SFGSS-000.
- [x] Independence proof is credible.
- [x] MVP is bounded and useful.
- [x] Public API, data, lifecycle, arbitration and failure behavior are specified.
- [x] Built-in and optional backend boundaries are explicit.
- [x] Setup/direct-scene workflows and two Standalone Labs are defined.
- [x] Diagnostics do not require The Observatory.
- [x] Optional integrations are explicit and removable.
- [x] Test/release gates are measurable and remain Not run where appropriate.
- [x] No Isekai identity or ownership was introduced.
- [x] Jesse has approved the documentation-first package program; this specification is approved under that program.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** This approves the package architecture and documentation only. Implementation remains locked until SUITE-DOC-33. Exact Unity/Cinemachine compatibility, performance, platform, migration, installation and release evidence must be produced later under SFGSS-004.


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
