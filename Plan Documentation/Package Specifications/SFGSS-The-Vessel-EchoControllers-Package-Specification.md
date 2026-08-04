# The Vessel - Player Controller Foundations Package Specification

**Working document ID:** SFGSS-PKG-ECHOCONTROLLERS-001  
**Specification version:** 1.0.0  
**Status:** Approved  
**Technical package name:** EchoControllers  
**Public title:** The Vessel - Player Controller Foundations  
**Package ID:** `com.echodevgames.echo-controllers`  
**Runtime namespace:** `EchoDevGames.EchoControllers`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoControllers`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Last updated:** August 4, 2026

> “Give intent a body, give motion a language, and let every game choose its own destination.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoControllers. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-005 and approved package authorities through The Fellowship | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved rootless actor-bound controller architecture, normalized family intents, control/source leases, Side-View 2D and Top-Down 2D MVP presets, physics boundaries, capabilities, diagnostics, tooling, Laboratories, bridges, and release contracts | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Vessel - Player Controller Foundations  
**Technical identifier:** EchoControllers  
**Flavor line:** Give intent a body, give motion a language, and let every game choose its own destination.  
**Plain-language subtitle:** A standalone Unity package for actor-bound controller hosts, normalized locomotion intent, physics-backed motors, focused capability modules, semantic movement state, Side-View 2D and Top-Down 2D presets, diagnostics, authoring, validation, and explicit integration seams.

**One-sentence ownership contract:**

> EchoControllers owns reusable actor-bound movement controller hosts, normalized family-specific locomotion intent contracts, stale-safe intent-source and control leases, local motor execution, grounding and movement probes, focused locomotion capabilities, semantic controller state and events, warp/external-motion seams, Side-View 2D and Top-Down 2D MVP presets, diagnostics, setup, validation, independent preset Laboratories, and explicit adapter/bridge seams; it does not own character identity or rosters, gameplay input devices or action maps, camera authority, combat, abilities, AI decisions, animation graphs, VFX, audio playback, UI, interaction outcomes, scene loading, save-file transport, networking authority, or one universal movement formula.

### 1.1 Elevator summary

The Vessel provides reusable **movement execution**, not a universal player object. Each controlled actor carries its own `ControllerHost` and one declared preset motor. Intent arrives through a normalized, preset-specific source contract. The motor consumes current intent on its documented update path, owns only its local locomotion state, and publishes immutable snapshots and semantic events after authoritative movement state changes.

The package is deliberately rootless. There is no application-session controller singleton and no persistent global manager. Several actors, local players, scenes, or independent Laboratories may run simultaneously without competing for one static authority. A controller may use the easy `AlwaysControlled` path in a single-pawn prototype or require a stale-safe control lease when connected to The Fellowship, local multiplayer, AI possession, cutscenes, or project-owned control arbitration.

The MVP ships two independently selectable controller families: a physics-backed Side-View 2D preset and a physics-backed Top-Down 2D preset. They share contracts, diagnostics, facing and constraint concepts, but they do not pretend identical physics or one giant enum fits every genre. Climbing, crawling, swimming, ladders, click-to-move, first-person, third-person, flying, vehicles, navigation, and network prediction remain later modules or families that must earn their own Laboratories and release evidence.

### 1.2 Why this belongs in The Sperk's Forge

Rescuers2D already needs walking, jumping, crawling, swimming, climbing, ladders, role-specific motion, character switching, and clear control handoff. Hackulos needs top-down movement now and optional click-to-move later. Echo Systems Lab, Don't Get Vince'd, game-jam prototypes, and future portfolio systems repeatedly rebuild grounding, movement input translation, facing, velocity limits, jump buffering, coyote time, diagnostics, and direct-scene proof.

Those recurring mechanics justify reusable foundations, but the existing projects also show why one universal controller becomes brittle. A side-view Rigidbody2D motor, a top-down motor, a CharacterController-based first-person preset, and a navigation-based click-to-move preset do not share identical physics truth. The Vessel preserves common contracts and focused utilities while allowing each family to own its honest backend and acceptance evidence.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Always paired with “Player Controller Foundations.” |
| Setup guidance/tooltips | Yes | Flavor must never obscure physics, intent, or component requirements. |
| Samples | Optional | Vessel imagery may decorate Labs but is removable. |
| Runtime API/type names | No lore-only names | Types use `ControllerHost`, `SideView2DIntentFrame`, `GroundProbe2D`, and similar technical names. |
| Project data | No required Verse content | Games own actors, sprites, models, animation graphs, input assets, movement tuning, and gameplay capabilities. |

---

## 2. Problem Statement

### 2.1 Current problem

Player movement code commonly mixes device polling, action-map assumptions, Rigidbody writes, animation parameters, camera updates, combat actions, audio, VFX, character switching, and project-specific states in one MonoBehaviour. Reusing it means copying the entire knot. A second locomotion mode adds booleans until crawling, climbing, ladders, swimming, knockback, cutscenes, and vehicles all compete inside one update loop.

Input is often read in both `Update` and `FixedUpdate` without an explicit buffer. Grounding depends on one scene layer or transform name. A character switch enables and disables components directly without a stale-safe ownership transfer. Animation graphs become the only record of movement state. Teleports leave damping and ground state stale. Direct-scene testing works only because another project bootstrap or input asset is present.

A reusable package must separate intent, motor execution, local state, and presentation. It must work without The Will, The Fellowship, The Eye, Impact, Jukebot, The Looking Glass, or project code. It must also remain honest that different physics backends and controller families require distinct motors, capabilities, limitations, and Laboratories.

### 2.2 Evidence from existing work

| Source project/system | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Firefighter, Riot Officer, and Rescue Specialist controllers share Rigidbody2D movement but grow role-specific flags | Fast role iteration and shared input translation | Separate base locomotion, focused traversal capabilities, and character control handoff |
| Hackulos | Top-down 2D movement needs four/eight-direction facing and future click-to-move | Data-driven character content | Keep controller movement neutral and click-to-move outside MVP |
| Don't Get Vince'd | Beat-'em-up locomotion, attacks, hit reactions, and camera zones often meet in one actor script | Responsive action movement | Keep combat and camera authority outside controller motor |
| Echo Systems Lab | Focused components and event-driven systems already work well | Definition/runtime/presentation separation | Formalize normalized intent, motor state, diagnostics, and Labs |
| The Will | Devices, action maps, contexts, and rebinding need a movement consumer | Centralized input authority | Translate through a separate Input System/Will adapter instead of core polling |
| The Fellowship | Control ownership and character switching need a movement target | Durable character/control truth | Bridge control assignment into controller leases without roster dependency |
| The Eye | Camera needs position, facing, velocity, and warp information | Stable target snapshots | Publish controller semantics; never move the camera from a controller |
| Impact/Clash | Knockback and feedback may influence movement | Semantic external requests | Let the motor apply bounded motion requests without owning cause or damage |

### 2.3 Consequences of doing nothing

- Every project rebuilds movement and grounding differently.
- Input assets and controller code become inseparable.
- Character switching leaves stale input or two enabled controllers.
- Animation parameters become hidden gameplay state.
- Crawling, swimming, climbing, and ladders inflate one all-purpose script.
- Physics writes occur from inconsistent update phases.
- Scene reloads retain static state or event subscriptions.
- Controller samples work only when unrelated package code is installed.
- Network, AI, and cutscene control cannot reuse the same normalized movement seam.
- Debugging cannot explain which source, control lease, capability, or constraint currently owns movement.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide actor-bound, rootless controller foundations.
- Separate normalized intent, motor execution, state/events, and presentation.
- Support stale-safe control and intent-source lifetimes.
- Execute physics-backed movement on an explicit fixed-step path.
- Provide focused utilities and capabilities rather than one universal controller.
- Ship independent Side-View 2D and Top-Down 2D MVP presets.
- Support easy standalone control and lease-required possession workflows.
- Publish immutable semantic state for animation, camera, audio, VFX, UI, diagnostics, and project logic.
- Expose warp and external-motion seams without absorbing combat or feedback authority.
- Remain usable without any other Sperk's Forge package.
- Provide repeat-safe setup, validation, diagnostics, and independent Laboratories.

### 3.2 Non-goals

- Own character identity, rosters, spawning, or control-owner truth.
- Poll one mandatory input system or require one action-map schema.
- Move cameras or define camera modes.
- Implement combat, attacks, damage, abilities, AI, interaction outcomes, or animation graphs.
- Provide identical behavior across Rigidbody2D, Rigidbody, CharacterController, navigation, and vehicle backends.
- Include every traversal capability in the MVP.
- Provide network prediction, reconciliation, rollback, or deterministic lockstep.
- Persist live velocity, intent buffers, ground contacts, or control leases.
- Become a global `PlayerControllerManager` singleton.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean 2D project with one actor | Add a supported preset, assign configuration, run its Lab, and understand every required component |
| Programmer | Project-owned input, AI, or possession system | Feed normalized intent and receive semantic state without editing package source |
| Designer | Needs movement tuning | Author project-owned configuration assets and preview safe ranges without changing code |
| Animator/technical artist | Needs reliable movement semantics | Consume facing, velocity, grounded, jump, landing, and locomotion events without owning rules |
| Tester | Movement feels wrong or stops | Inspect source/control generations, intent age, motor state, contacts, constraints, and diagnostics |

### 3.4 Measurable success criteria

- Installs into a clean supported Unity project with zero compile errors.
- Core and each MVP preset work with no other Sperk's Forge package installed.
- Side-View 2D and Top-Down 2D pass separate Laboratories.
- Removing optional bridges/adapters leaves core and peer packages compile-safe.
- Configuration assets remain immutable during play.
- No motor hot path allocates after initialization under the approved implementation target.
- Stale source/control handles cannot alter current authority.
- One actor cannot have two authoritative motors for the same preset.
- Documentation, setup, and diagnostics explain supported physics assumptions.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo developers and small Unity teams.
- Gameplay programmers building reusable movement.
- Designers tuning common locomotion.
- Technical artists connecting animation and feedback.
- QA testers validating controller lifecycle and physics behavior.
- Projects using single pawns, switchable characters, local multiplayer, AI possession, or future network adapters.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| ECTR-UC-001 | Configure a Side-View 2D actor | Novice installer | Rigidbody2D, Collider2D, package installed | Actor walks, falls, jumps, faces, and reports state | MVP |
| ECTR-UC-002 | Configure a Top-Down 2D actor | Novice installer | Rigidbody2D, Collider2D, package installed | Actor moves and faces under selected 4/8-direction policy | MVP |
| ECTR-UC-003 | Feed scripted intent | Tester | Controller ready | Deterministic Lab driver exercises the preset without input dependencies | MVP |
| ECTR-UC-004 | Feed project-owned input | Programmer | Adapter implements family intent source | Motor consumes normalized intent | MVP |
| ECTR-UC-005 | Require possession lease | Programmer | Host configured LeaseRequired | Only current control generation drives movement | MVP |
| ECTR-UC-006 | Switch Fellowship character | Characters bridge | Target actor ready | Bridge transfers control lease after Fellowship commit | Integration |
| ECTR-UC-007 | Publish camera target semantics | Eye bridge/project | Controller active | Position, velocity, facing, and warp revisions are translated | Integration |
| ECTR-UC-008 | Drive animation | Presenter/project | State/event listener installed | Presentation updates without owning locomotion rules | MVP sample |
| ECTR-UC-009 | Apply knockback/external motion | Combat/project | Motor supports request | Motion request applies under explicit combine policy | MVP seam |
| ECTR-UC-010 | Add traversal capability | Programmer | Compatible family/capability contract | Module participates without editing universal controller source | Later |
| ECTR-UC-011 | Diagnose lost movement | Tester | Play Mode active | Readout explains control, source, intent, motor, contact, and constraint state | MVP |
| ECTR-UC-012 | Use multiple controllers | Project | Several actors configured | Each actor remains independent; no global root exists | MVP |

### 4.3 Explicitly unsupported use cases

- Using the MVP controller as a network-predicted authoritative motor.
- Expecting Rigidbody2D tuning to match CharacterController or navigation behavior.
- Treating controller state as character identity or save authority.
- Requiring the package to generate an Animator Controller or game input asset.
- Adding unsupported capability booleans to the core instead of a focused module/specification.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Actor-local controller host lifecycle.
- One authoritative preset motor per host.
- Family-specific normalized intent contracts.
- Intent-source registration, arbitration, packet freshness, and edge buffering.
- Controller-local control admission and stale-safe leases.
- Motor-owned position/velocity writes within the supported backend contract.
- Grounding, facing, slope, jump, movement constraints, and other approved local capabilities.
- Semantic locomotion state, snapshots, events, and diagnostics.
- Validated warp and external-motion requests.
- Controller preset setup, validation, authoring, and Labs.

### 5.2 The package does not own

- Character identity, selection, roster, spawning, or durable control-owner assignment.
- Devices, bindings, action maps, rebinding, or global input contexts.
- Game state, pause authority, or time-scale policy.
- Camera output.
- Combat, abilities, interaction outcomes, AI, dialogue, objectives, inventory, or crafting.
- Animator graphs, animation clips, VFX, audio, UI, or localization.
- Scene flow, save transport, world persistence, or multiplayer provider authority.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How The Vessel interacts |
|---|---|---|
| Character identity/control ownership | The Fellowship or project | Separate bridge maps committed ownership to controller control leases |
| Devices/actions/contexts | The Will or project | Separate adapter emits normalized family intent |
| High-level pause/state | The Pulse or project | Bridge/project enables or suspends control; motor does not set time scale |
| Camera view | The Eye | Bridge publishes target/facing/velocity/warp semantics |
| Feedback recipes | Impact | Project/bridge requests external movement or listens to semantic events |
| Combat/knockback cause | Future Clash/project | Caller submits a motion influence; motor owns only execution |
| Animation | Project/sample presenter | Listens to semantic state/events |
| Audio | Resonance/project | Listens to semantic events through bridge/project adapter |
| Interaction | The Hand | May consume facing/origin and may request movement locks through project integration |
| Save/world position | Chronicle/project/future Atlas | Project saves semantic position/state; live controller internals are not save truth |
| Networking | Future Convergence | Provider adapter owns prediction/authority translation after research |

### 5.4 Boundary tests

A proposed feature belongs in The Vessel only when it directly executes or reports reusable locomotion, remains actor-local, works without a peer, and does not require game-specific meaning. Features fail the boundary when they choose the character, read a device, decide damage, move a camera, author an animation graph, advance a quest, load a scene, or persist world truth.

---

## 6. Independence Contract

### 6.1 Standalone guarantees

The package must:

- Compile with only declared Unity dependencies.
- Require no persistent root or First Light integration.
- Run each MVP preset without any peer Echo package.
- Accept a deterministic scripted intent source in every Standalone Lab.
- Keep input, character, camera, animation, and diagnostics integrations optional.
- Avoid project tags, layers, scene names, input maps, animator parameters, and folder assumptions.
- Keep project-owned tuning assets outside immutable package source.
- Fail visibly when required actor components or configuration are missing.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | Core and preset assemblies compile | Planned clean-install tests; Not run |
| Side-View Lab direct entry | Preset runs through scripted driver | ECTR-LAB-001–034; Not run |
| Top-Down Lab direct entry | Preset runs through scripted driver | ECTR-LAB-035–068; Not run |
| Input System adapter absent | Scripted/project source path remains valid | Planned tests; Not run |
| Fellowship bridge absent | AlwaysControlled/lease API remains available | Planned tests; Not run |
| Eye/Impact/Observatory absent | Core movement and local diagnostics remain available | Planned tests; Not run |
| Sample removed | Runtime and Editor assemblies compile | Planned sample-removal tests; Not run |
| Several actors present | Actor-local authorities remain independent | ECTR-LAB-062; Not run |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine core | Platform | Yes | Unity 6000.0 | MonoBehaviour, transforms, math, lifecycle | Package cannot function |
| Unity Physics 2D module | Platform | Yes for MVP package | Unity 6000.0 | Rigidbody2D, Collider2D, casts/contacts | MVP preset package cannot function |
| Unity Test Framework | Test only | No at runtime | Resolve at implementation | Automated tests | Runtime unaffected |

The core package does not hard-depend on Input System, Cinemachine, another Echo package, navigation, Timeline, or a networking SDK.

### 6.4 Forbidden dependencies

- Project assemblies or generated input wrappers.
- The Fellowship, The Will, The Eye, Pulse, Impact, Resonance, Observatory, or any other peer core.
- Sample assemblies from runtime code.
- Reflection-based discovery of peer packages, controllers, sources, or capabilities.
- Hidden tags, layers, animator parameters, scene names, Resources paths, or singleton objects.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| ECTR-CAP-001 | Actor-local host | Owns one controller actor lifecycle and diagnostics | Approved | Yes | Runtime | No global root |
| ECTR-CAP-002 | Intent-source leases | Bounded prioritized source registration and stale rejection | Approved | Yes | Runtime | Family-specific payloads |
| ECTR-CAP-003 | Control policies | AlwaysControlled and LeaseRequired modes | Approved | Yes | Runtime | Project/Fellowship may drive lease |
| ECTR-CAP-004 | Semantic snapshots/events | State after authoritative motor changes | Approved | Yes | Runtime | Presentation-independent |
| ECTR-CAP-005 | Side-View 2D motor | Horizontal motion, gravity/fall, ground/air state | Approved | Yes | Runtime preset | Dynamic Rigidbody2D |
| ECTR-CAP-006 | Ground probe 2D | Walkable surface and slope evaluation | Approved | Yes | Runtime capability | Configurable cast/filter |
| ECTR-CAP-007 | Jump capability 2D | Coyote, buffer, variable release, ceiling/landing semantics | Approved | Yes | Runtime capability | No air jump in MVP |
| ECTR-CAP-008 | Side-View facing | Left/right facing with thresholds | Approved | Yes | Runtime capability | No Animator ownership |
| ECTR-CAP-009 | Top-Down 2D motor | Planar motion, acceleration/deceleration, collision response | Approved | Yes | Runtime preset | Dynamic Rigidbody2D |
| ECTR-CAP-010 | Top-Down facing | 4/8 direction, look or move source, last-facing policy | Approved | Yes | Runtime capability | Sprite-friendly semantics |
| ECTR-CAP-011 | Movement constraints | Bounded actor-local motion restrictions | Approved | Yes | Runtime capability | Simple bounds MVP |
| ECTR-CAP-012 | Warp request | Atomic position/velocity/facing reset and revision | Approved | Yes | Runtime | Informs camera/presentation |
| ECTR-CAP-013 | External motion request | Combine/replace bounded velocity change | Approved | Yes | Runtime | Cause owned elsewhere |
| ECTR-CAP-014 | Controller monitor | State/source/control/contact diagnostics | Approved | Yes | Editor/Lab | Observatory bridge later |
| ECTR-CAP-015 | Deterministic Lab driver | Scripted intent with no input dependency | Approved | Yes | Sample | Standalone proof |
| ECTR-CAP-016 | Input System adapter | Converts actions into family intent | Deferred | No | Separate adapter | Version tested later |
| ECTR-CAP-017 | Fellowship bridge | Converts control assignment into leases | Deferred | No | Separate bridge | Integration Lab required |
| ECTR-CAP-018 | Eye bridge | Publishes target/facing/velocity/warp | Deferred | No | Separate bridge | Eye remains authority |
| ECTR-CAP-019 | Animator presenter | Maps semantic state to project parameters | Deferred | No | Sample/optional presentation | No generated graph |
| ECTR-CAP-020 | Crawl capability | Side-view crawl posture/motor module | Deferred | No | Later capability | Dedicated Lab required |
| ECTR-CAP-021 | Climb/ladder capability | Climb and authored ladder traversal | Deferred | No | Later capability | Dedicated design/Lab |
| ECTR-CAP-022 | Swim capability | Buoyancy/planar swim module | Deferred | No | Later capability | Separate backend truth |
| ECTR-CAP-023 | Click-to-move | Navigation target intent/controller | Deferred | No | Later family | Not top-down MVP |
| ECTR-CAP-024 | 3D/first-person/third-person | Additional controller families | Deferred | No | Later modules/packages | Each needs own Lab |
| ECTR-CAP-025 | Network prediction | Provider-specific authority/reconciliation | Rejected from core | No | Future Convergence adapter | Research required |

### 7.2 MVP capability set

One rootless controller host contract, stale-safe intent sources and control leases, actor-local semantic state, Side-View 2D and Top-Down 2D Dynamic Rigidbody2D presets, grounding/jump/facing/constraints, warp/external motion, diagnostics, setup/validation, deterministic scripted drivers, and independent Laboratories.

### 7.3 Later capability set

Optional Input System and peer bridges; animation presenters; crawl, climb, ladders, swim, click-to-move, top-down 3D, twin-stick, first-person, third-person, grid, flight, zero-gravity, and vehicle/pawn families. Each independently selectable family/capability must receive its own specification additions, assembly boundary, configuration, Laboratory, and evidence.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| One universal controller class | Rejected | Boolean/state explosion and incompatible backends | Never without suite ADR |
| Mandatory EchoInput | Rejected | Violates standalone-first rule | Use adapter |
| Mandatory Animator graph | Rejected | Presentation is project-owned | Optional presenter only |
| Save live controller internals | Rejected | Scene/session state and stale handles | Save semantic project state |
| One global controller singleton | Rejected | Actor-local authority and multiplayer conflict | Never |
| Split into many packages immediately | Deferred | Two MVP presets can share release cadence | Revisit after third backend/family proves independent cadence or dependency need |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Preset configuration, ground/slope/jump/facing/constraint policies, stable IDs | Live velocity, contacts, intent edges, scene objects |
| Runtime state/behavior | Host, source/control registrations, buffers, motors, probes, capabilities, state snapshots | Editor code, peer package rules, presentation assumptions |
| Presentation/feedback | Debug readouts, gizmos, optional animator sample, semantic listeners | Authoritative movement truth |

### 8.2 Component topology

```text
Project / scripted / optional input adapter
        -> family intent source lease
            -> ControllerHost
                -> control admission
                -> intent freshness/buffer
                -> one preset motor
                    -> focused probes/capabilities
                    -> Rigidbody2D backend
                -> immutable state snapshot + semantic events
                    -> project animation/audio/VFX/UI
                    -> optional Eye/Fellowship/Observatory bridges
```

A host owns exactly one authoritative motor. Focused components collaborate through explicit interfaces and the host’s step context. Capability order is declared and validated; it is not discovered through arbitrary reflection or hierarchy order.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | No |
| Authority unit | One actor-bound `ControllerHost` plus one preset motor |
| Duplicate behavior | Reject multiple authoritative motors/hosts on one actor according to validation rules |
| Initialization trigger | Actor lifecycle (`Awake` validation, explicit/OnEnable readiness according to implementation plan) |
| Shutdown behavior | Stop accepting intent, release local registrations, clear buffers, publish disabled state, unsubscribe |
| Direct-scene behavior | Natural; actor and Lab open directly with no bootstrap |
| Test injection seam | Intent sources, clocks, probes, physics/motion adapters where approved, scripted driver |

### 8.4 Lifecycle sequence

1. Validate immutable configuration and required actor components.
2. Claim the actor-local host/motor slot.
3. Initialize probes, capabilities, state, and diagnostics without movement side effects.
4. Register or adopt intent sources and determine control admission.
5. Sample/publish intent on the source path.
6. Consume fresh intent and execute motor/capabilities on the declared fixed step.
7. Reconcile contacts, facing, state, constraints, and external requests.
8. Publish immutable snapshot and semantic events after authoritative changes.
9. On disable/destroy, stop intake, invalidate generations, clear buffers, unsubscribe, and release actor-local authority.

### 8.5 Failure model

| Failure | Detection point | Result | Fallback | Diagnostic |
|---|---|---|---|---|
| Missing configuration | Validation/init | Controller Blocked | No movement writes | ECTR-DIAG-001 |
| Missing Rigidbody2D/Collider2D | Validation/init | Controller Blocked | Actor remains project-owned | ECTR-DIAG-002 |
| Unsupported body type | Validation/init | Controller Blocked | No automatic conversion | ECTR-DIAG-003 |
| Duplicate motor/host | Claim | New/ambiguous authority rejected | Existing valid motor remains | ECTR-DIAG-004 |
| Stale source/control handle | Request/release | Request ignored | Current generation remains | ECTR-DIAG-005 |
| Stale intent sequence | Intake | Packet ignored | Latest accepted intent remains | ECTR-DIAG-006 |
| No valid intent source | Step | Neutral intent | Motor decelerates/holds per policy | ECTR-DIAG-007 |
| Ground probe unavailable | Step | Ground-dependent actions unavailable | Safe airborne/blocked policy | ECTR-DIAG-008 |
| Capability exception | Step boundary | Capability disabled/host faulted per severity | No silent continued corruption | ECTR-DIAG-009 |
| External actor destruction | Lifecycle | Host terminates | Registrations invalidated | ECTR-DIAG-010 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `ControllerPresetConfiguration` | Shared preset metadata and safety limits | Yes | No | Yes |
| `SideView2DControllerConfiguration` | Ground/air speed, acceleration, jump, slope, facing, constraints | Yes | No | Yes |
| `TopDown2DControllerConfiguration` | Speed, acceleration, diagonal, facing, constraints | Yes | No | Yes |
| `GroundProbe2DConfiguration` | Cast geometry, walkable filters, slope thresholds | Optional nested/stable config | No | Yes |
| `ControllerSafetyConfiguration` | Intent age, source counts, velocity/warp limits, diagnostics bounds | Yes or nested | No | Yes |

Unity asset GUIDs identify assets in the Editor; serialized domain IDs identify controller configurations across runtime records and diagnostics. They are not interchangeable.

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `ControllerHostState` | Host | Actor session | Rebuilt on enable/init | Not saved |
| `ControllerIntentSourceState` | Host | Registration lease | Removed on release/source loss | Not saved |
| `ControllerControlState` | Host | Control generation | Recomputed from active policy/lease | Not saved |
| `SideView2DMotorState` | Motor | Actor session | Reset/warp/disable policy | Not saved |
| `TopDown2DMotorState` | Motor | Actor session | Reset/warp/disable policy | Not saved |
| `GroundProbe2DState` | Probe | Physics step | Recomputed each step | Not saved |
| `ControllerStateSnapshot` | Host | Immutable publication | Replaced after state change/step | Diagnostic only |
| `PendingMotionRequest` | Motor | Bounded request lifetime | Consume/cancel/timeout | Not saved |

### 9.3 Stable identifiers

- `ControllerConfigurationId` identifies an authored project configuration.
- `ControllerRuntimeId` identifies one controller host during the current session.
- `ControllerIntentSourceId` identifies a registered source logically; the lease generation prevents stale use.
- `ControllerControlTokenId` identifies one current control grant without replacing Fellowship `ControlOwnerId`.
- `ControllerCapabilityId` identifies a declared capability type/configuration.
- Display names remain editable and non-authoritative.
- IDs are validated for emptiness, collisions, aliases, and migration under SFGSS-003.

### 9.4 ScriptableObject safety

Configuration assets remain immutable during play. Current velocity, facing, contacts, coyote timestamps, jump buffers, intent sequences, source priorities, control generations, capability state, diagnostic history, and scene references live in runtime objects. Editor preview uses detached copies or explicit preview state.

### 9.5 Serialization and migration

The MVP has no game-save payload. Authored configuration assets include schema versions and migrate through previewable, backup-preserving Editor tooling. Unknown future fields are preserved where the authoring format allows; otherwise migration stops rather than silently dropping data. Public preset/diagnostic IDs and serialized enums follow SFGSS-003 compatibility rules.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Ownership |
|---|---|---|---|
| `ControllerHost` | MonoBehaviour | Actor-local lifecycle, source/control arbitration, state publication | Actor/project |
| `IEchoController` | Interface | Readiness, control, snapshot, warp, external-motion surface | Host/motor implementation |
| `ControllerRuntimeId` | Struct | Session identity for one host | Host-generated |
| `ControllerControlMode` | Enum | `AlwaysControlled` or `LeaseRequired` | Configuration |
| `ControllerControlRequest` | Struct | Requests actor-local intent admission | Caller-created |
| `ControllerControlLease` | Struct/IDisposable | Generational control grant | Host-issued |
| `IControllerIntentSource` | Interface | Common source identity/readiness/lifecycle | Project/adapter/sample |
| `ISideView2DIntentSource` | Interface | Produces Side-View intent frames | Project/adapter/sample |
| `ITopDown2DIntentSource` | Interface | Produces Top-Down intent frames | Project/adapter/sample |
| `SideView2DIntentFrame` | Immutable struct | Horizontal intent and jump phases with sequence/time | Source-produced |
| `TopDown2DIntentFrame` | Immutable struct | Move/look vectors and sequence/time | Source-produced |
| `ControllerIntentSourceLease` | Struct/IDisposable | Stale-safe source registration | Host-issued |
| `SideView2DController` | MonoBehaviour/service | Supported side-view motor façade | Actor-owned |
| `TopDown2DController` | MonoBehaviour/service | Supported top-down motor façade | Actor-owned |
| `ControllerStateSnapshot` | Immutable struct | Common control/source/health/motion state | Host-produced |
| `SideView2DStateSnapshot` | Immutable struct | Ground, slope, jump, facing, velocity state | Motor-produced |
| `TopDown2DStateSnapshot` | Immutable struct | Move/facing/velocity/constraint state | Motor-produced |
| `ControllerWarpRequest/Result` | Structs | Validated actor warp | Caller/host |
| `ControllerMotionRequest/Result` | Structs | Bounded external velocity influence | Caller/motor |
| `IControllerCapability` | Interface | Focused compatible module lifecycle | Package/project capability |
| `IGroundProbe2D` | Interface | Ground and slope query contract | Package/project provider |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Loop rule |
|---|---|---|---|---|
| `RegisterIntentSource(...)` | Add bounded family-compatible source | Host Ready; compatible family | Lease or structured rejection | Main thread |
| `AcquireControl(...)` | Grant control in LeaseRequired mode | Host Ready; policy allows | Lease or denial | Main thread |
| `GetSnapshot()` | Read immutable current state | Host exists | Current/Unavailable snapshot | Main thread |
| `RequestWarp(...)` | Move actor through explicit safe seam | Valid request; host Ready | Atomic result and warp revision | Applied on declared motor boundary |
| `RequestMotion(...)` | Apply external velocity influence | Supported policy/capacity | Accepted/rejected/coalesced result | Consumed on motor step |
| `ResetController(...)` | Return runtime state to configured baseline | Development/project policy allows | Structured reset result | Safe boundary only |
| `SetBaseControlEnabled(...)` | Project-level non-lease gate where approved | Host Ready | State change with reason | Main thread |

Public APIs return structured results rather than logging-only failure. No convenience static singleton is the only access path.

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `ControllerReadyChanged` | Host | After readiness commits | Old/new readiness and reason | Listener optional |
| `ControlChanged` | Host | After effective control changes | Generation, enabled state, source | No ownership authority implied |
| `IntentSourceChanged` | Host | After winning source changes | Old/new source IDs | Listener optional |
| `ControllerStateChanged` | Host | After snapshot publication | Old/new common state | No per-frame requirement when unchanged |
| `MovementStarted/Stopped` | Motor | After velocity/state threshold commit | Snapshot | Presentation optional |
| `FacingChanged` | Capability/motor | After facing commit | Old/new direction | Presentation optional |
| `GroundedChanged` | Ground/motor | After contact truth commits | Ground snapshot | Presentation optional |
| `Jumped` | Jump capability | After vertical jump velocity commits | Jump context | Not a request event |
| `Landed` | Motor | After grounded landing commits | Pre-contact fall speed/context | Feedback optional |
| `Warped` | Host/motor | After warp commits | Warp revision/result | Camera bridge may consume |
| `ControllerFaulted` | Host | After fault state commits | Diagnostic code/context | No listener required for safe failure |

### 10.4 Async and cancellation policy

MVP motor execution is synchronous and main-thread/fixed-step based. Setup/migration may use Editor async operations, but normal movement does not return long-lived Tasks. Warp and motion requests are queued only to the next approved motor boundary and expose cancellation only before commit when a request handle exists. No API promises cancellation after Rigidbody2D state has been written.

### 10.5 API ergonomics

The novice path uses a preset setup command, one project-owned configuration, one actor prefab, and a scripted Lab driver. The advanced path implements family intent sources, probes, focused capabilities, or separate bridges. Public state is readable without reflection, hierarchy searches, or Animator parameters.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install the package.
2. Open **Sperk's Forge > The Vessel > Setup**.
3. Select Side-View 2D or Top-Down 2D.
4. Select an actor or request a project-owned starter actor.
5. Preview required components, configuration assets, and proposed changes.
6. Create-only-safe or explicitly repair the selected actor.
7. Open the matching Standalone Laboratory.
8. Run validation and export the setup report.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report |
|---|---|---|---:|---|---|
| Create preset configuration | Project-owned asset | Nothing existing | Yes | Unity Undo/create receipt | Asset path/ID |
| Add Side-View preset | Host/motor/probe/capability components | Selected actor after preview | Yes | Unity Undo | Component changes |
| Add Top-Down preset | Host/motor/facing/constraint components | Selected actor after preview | Yes | Unity Undo | Component changes |
| Repair missing references | Approved missing refs only | Selected actor/config | Yes | Preview/Undo | Before/after |
| Generate scripted Lab copy | Project-owned sample copy | New files only | Yes | Create-only-safe | Created assets |
| Validate all controllers | Reports only | None | Yes | N/A | Structured validation |
| Remove preset components | Removal plan | Selected actor only after confirmation | Conditional | Backup/Undo | Exact removals |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Vessel Setup Window | Novice/programmer | Create/repair preset actors and configurations | No |
| Controller Configuration Inspector | Designer | Validate ranges, policies, IDs, and capability compatibility | No |
| Controller Monitor | Tester | Inspect control/source/intent/motor/contact/state in Play Mode | No |
| Ground Probe Gizmos | Programmer/designer | Visualize cast geometry, normals, slope classification | No |
| Intent Script Editor | Tester | Author deterministic Laboratory sequences | No |
| Migration/Repair Tool | Maintainer | Preview and apply schema/component migration | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix? | Safe auto-fix? |
|---|---|---|---:|---:|
| ECTR-VAL-001 | Missing controller configuration | Blocker | Yes | No |
| ECTR-VAL-002 | Missing Rigidbody2D | Blocker | Yes | Only with explicit preview |
| ECTR-VAL-003 | Missing compatible Collider2D | Blocker | Yes | No default shape silently |
| ECTR-VAL-004 | Unsupported body type | Blocker | Yes | No |
| ECTR-VAL-005 | Duplicate host/motor | Blocker | Yes | No |
| ECTR-VAL-006 | Configuration ID empty/colliding | Blocker | Yes | Only unreleased/new asset with confirmation |
| ECTR-VAL-007 | Invalid speed/acceleration/jump range | Error | Yes | Safe clamp only when approved |
| ECTR-VAL-008 | Ground mask empty or self-only | Error | Yes | No |
| ECTR-VAL-009 | Slope threshold invalid | Error | Yes | Safe clamp with preview |
| ECTR-VAL-010 | Capability incompatible with family | Error | Yes | No |
| ECTR-VAL-011 | Intent source family mismatch | Error | Guidance | No |
| ECTR-VAL-012 | LeaseRequired with no setup path | Warning | Guidance | No |
| ECTR-VAL-013 | Shared config mutated in Play Mode | Error | Repair/reset | No silent overwrite |
| ECTR-VAL-014 | Sample/peer dependency leaked into core | Blocker | Guidance | No |
| ECTR-VAL-015 | Unsupported layer/tag assumption | Error | Guidance | No |
| ECTR-VAL-016 | Diagnostic/history bound invalid | Error | Yes | Safe clamp with preview |

The package implements the SFGSS-ADR-001 Editor setup facade protocol for The Workshop without adding a runtime Workshop dependency.

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

- Git URL after repository release.
- Local path or embedded package during development.
- Tarball after distribution evidence exists.
- Workshop selection after the package setup facade is implemented.
- Registry only after publication policy is approved.

All route evidence remains Not run.

### 12.2 Minimal Side-View 2D setup

- One actor GameObject.
- One supported Dynamic Rigidbody2D.
- One compatible Collider2D.
- `ControllerHost`.
- `SideView2DController`/motor.
- Ground probe and jump/facing capabilities required by the chosen preset.
- Project-owned configuration asset.
- One scripted, project, or optional adapter intent source.

### 12.3 Minimal Top-Down 2D setup

- One actor GameObject.
- One supported Dynamic Rigidbody2D with approved top-down gravity policy.
- One compatible Collider2D.
- `ControllerHost`.
- `TopDown2DController`/motor.
- Facing/constraint components required by the chosen preset.
- Project-owned configuration asset.
- One scripted, project, or optional adapter intent source.

### 12.4 Direct-scene setup

No production bootstrap is needed. A controller lives with its actor. Opening either Lab directly creates only its local sample actors, scripted drivers, environment, and readout. No First Light, persistent root, EventSystem, input asset, or peer package is required.

### 12.5 Scene isolation rule

Each preset has its own scene and sample folder. Side-View assets do not become required by Top-Down and vice versa. Integration scenes belong to their bridge/provider artifacts and do not count as standalone preset proof.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Laboratory purpose

The package ships two independent Laboratories:

1. **Side-View 2D Controller Laboratory** proves horizontal movement, ground/air transitions, jumping, slopes, facing, constraints, warps, external motion, leases, diagnostics, reset, and lifecycle.
2. **Top-Down 2D Controller Laboratory** proves planar movement, diagonal policy, facing modes, collision sliding, constraints, warps, external motion, leases, multi-actor independence, diagnostics, reset, and lifecycle.

Both include deterministic scripted intent drivers so no input package is required. Optional interactive Input System samples belong to the separate adapter artifact.

### 13.2 Required Laboratory contents

- Visible controls/instructions and current evidence status.
- Minimal project-owned sample configurations.
- Scripted intent sequences and reset controls.
- State/source/control/velocity/facing/contact readouts.
- Ground/slope/constraint gizmos where applicable.
- Success, invalid, stale, missing, and lifecycle cases.
- No unrelated Echo package code or restricted content.

### 13.3 Laboratory acceptance registry

| Test | Laboratory | Scenario | Action | Expected result | Type | Status |
|---|---|---|---|---|---|---|
| ECTR-LAB-001 | Side-View 2D | Initialize Side-View 2D controller | Open the Side-View 2D Laboratory with the approved sample actor. | The host, motor, ground probe, jump capability, facing capability, scripted intent driver, and readout become Ready. | Manual | Not run |
| ECTR-LAB-002 | Side-View 2D | Missing Side-View configuration | Remove the SideView2DControllerConfiguration reference. | Initialization is blocked with an actionable ECTR diagnostic and no movement side effects. | Manual | Not run |
| ECTR-LAB-003 | Side-View 2D | Missing Rigidbody2D | Remove the actor Rigidbody2D. | Validation blocks the controller and identifies the missing required component. | Manual | Not run |
| ECTR-LAB-004 | Side-View 2D | Missing Collider2D | Remove the actor body collider. | Validation blocks ground and movement execution without adding hidden project content. | Manual | Not run |
| ECTR-LAB-005 | Side-View 2D | Invalid Rigidbody2D body type | Change the actor body type away from the supported MVP Dynamic policy. | The controller reports UnsupportedConfiguration and does not write motion. | Manual | Not run |
| ECTR-LAB-006 | Side-View 2D | Duplicate Side-View motor | Add a second SideView2DMotor to the same host. | The actor refuses ambiguous motor ownership before FixedUpdate side effects. | Manual | Not run |
| ECTR-LAB-007 | Side-View 2D | Always-controlled easy path | Use the standalone AlwaysControlled control policy. | The scripted intent source can drive the motor without Fellowship or project possession code. | Manual | Not run |
| ECTR-LAB-008 | Side-View 2D | Acquire and release control lease | Use LeaseRequired mode and acquire then release one control lease. | Intent is accepted only while the current lease is valid. | Manual | Not run |
| ECTR-LAB-009 | Side-View 2D | Stale control lease | Acquire a newer control lease, then release the previous lease. | The stale lease cannot disable the current controller authority. | Manual | Not run |
| ECTR-LAB-010 | Side-View 2D | Out-of-order source release | Register two intent sources with different priorities and release them out of order. | The effective source is recomputed from active registrations rather than restored from stale state. | Manual | Not run |
| ECTR-LAB-011 | Side-View 2D | Reject stale intent sequence | Submit an older packet after a newer packet from the same source. | The stale packet is ignored and diagnosed without rewinding movement. | Manual | Not run |
| ECTR-LAB-012 | Side-View 2D | Expire buffered command | Buffer Jump, wait beyond the configured window, then land. | The expired command does not trigger a delayed jump. | Manual | Not run |
| ECTR-LAB-013 | Side-View 2D | Idle state | Provide zero horizontal intent on level ground. | Velocity settles under configured deceleration and the state reports GroundedIdle. | Manual | Not run |
| ECTR-LAB-014 | Side-View 2D | Ground acceleration | Ramp horizontal intent from zero to full. | Velocity approaches the configured ground-speed target without exceeding it. | Manual | Not run |
| ECTR-LAB-015 | Side-View 2D | Ground deceleration | Release horizontal intent while moving. | Velocity approaches zero under the configured deceleration policy. | Manual | Not run |
| ECTR-LAB-016 | Side-View 2D | Ground speed clamp | Hold full intent beyond the time required to reach maximum speed. | Horizontal motor-owned speed remains within the configured limit. | Manual | Not run |
| ECTR-LAB-017 | Side-View 2D | Facing changes | Reverse horizontal intent while grounded. | Facing changes once after the configured threshold and emits a semantic event. | Manual | Not run |
| ECTR-LAB-018 | Side-View 2D | Ground acquisition | Drop the actor onto a valid walkable surface. | Grounded truth commits after the configured probe policy and emits GroundedChanged/Landed. | Manual | Not run |
| ECTR-LAB-019 | Side-View 2D | Walk off ledge | Move from a platform into open air. | The state transitions to Falling without inventing a jump event. | Manual | Not run |
| ECTR-LAB-020 | Side-View 2D | Coyote-time jump | Press Jump shortly after leaving a ledge. | The jump succeeds only inside the configured coyote window. | Manual | Not run |
| ECTR-LAB-021 | Side-View 2D | Buffered landing jump | Press Jump shortly before landing. | The jump commits on the first valid grounded step inside the buffer window. | Manual | Not run |
| ECTR-LAB-022 | Side-View 2D | Reject airborne jump | Press Jump outside coyote time with no air-jump capability installed. | The request is rejected without changing vertical velocity. | Manual | Not run |
| ECTR-LAB-023 | Side-View 2D | Variable jump release | Release Jump before the configured full-jump hold window ends. | The upward trajectory is shortened through the approved variable-jump policy. | Manual | Not run |
| ECTR-LAB-024 | Side-View 2D | Ceiling interruption | Jump into a low ceiling. | Upward motor velocity is reconciled without leaving the controller in Rising. | Manual | Not run |
| ECTR-LAB-025 | Side-View 2D | Landing event | Fall from a known height onto level ground. | One landing event reports the pre-contact fall speed and final grounded state. | Manual | Not run |
| ECTR-LAB-026 | Side-View 2D | Ascend walkable slope | Move up a slope within the configured limit. | The motor preserves grounded movement and follows the walkable surface policy. | Manual | Not run |
| ECTR-LAB-027 | Side-View 2D | Descend walkable slope | Move down a slope within the configured limit. | The ground probe maintains stable contact without oscillating between grounded and falling. | Manual | Not run |
| ECTR-LAB-028 | Side-View 2D | Reject steep slope | Move into a slope beyond the supported angle. | The motor blocks or slides according to configuration and reports the reason. | Manual | Not run |
| ECTR-LAB-029 | Side-View 2D | Apply movement constraint | Enable a bounded horizontal movement constraint and move against it. | The actor remains inside the approved range and the constraint reports contact. | Manual | Not run |
| ECTR-LAB-030 | Side-View 2D | Apply external velocity change | Submit a project-owned external velocity request. | The motor applies the request through its public motion seam without claiming combat or feedback authority. | Manual | Not run |
| ECTR-LAB-031 | Side-View 2D | Warp actor | Issue a validated controller warp request. | Position and velocity policies apply atomically and a warp revision/event is published. | Manual | Not run |
| ECTR-LAB-032 | Side-View 2D | Pause and resume | Suspend control through the project control gate, then resume. | No movement command leaks during suspension and fresh intent is required after resume. | Manual | Not run |
| ECTR-LAB-033 | Side-View 2D | Change fixed timestep | Repeat the scripted run under another supported fixed timestep. | Behavior remains bounded by configuration; exact cross-step identity is not falsely promised. | Manual | Not run |
| ECTR-LAB-034 | Side-View 2D | Reset and reload Side-View Lab | Run, reset, and reload the scene repeatedly. | State, subscriptions, intent buffers, and diagnostics return to a known baseline without static contamination. | Manual | Not run |
| ECTR-LAB-035 | Top-Down 2D | Initialize Top-Down 2D controller | Open the Top-Down 2D Laboratory with the approved sample actor. | The host, motor, facing policy, scripted intent driver, and readout become Ready. | Manual | Not run |
| ECTR-LAB-036 | Top-Down 2D | Missing Top-Down configuration | Remove the TopDown2DControllerConfiguration reference. | Initialization is blocked with an actionable ECTR diagnostic and no motion side effects. | Manual | Not run |
| ECTR-LAB-037 | Top-Down 2D | Missing top-down Rigidbody2D | Remove the actor Rigidbody2D. | Validation blocks movement and identifies the required component. | Manual | Not run |
| ECTR-LAB-038 | Top-Down 2D | Missing top-down Collider2D | Remove the actor collider. | Validation blocks the controller without silently generating project content. | Manual | Not run |
| ECTR-LAB-039 | Top-Down 2D | Unexpected gravity scale | Set a nonzero gravity scale while using the MVP top-down preset. | Validation warns or blocks according to the configuration safety policy. | Manual | Not run |
| ECTR-LAB-040 | Top-Down 2D | Invalid top-down body type | Use an unsupported Rigidbody2D body type. | The motor refuses to execute and reports the supported MVP boundary. | Manual | Not run |
| ECTR-LAB-041 | Top-Down 2D | Duplicate Top-Down motor | Add a second TopDown2DMotor to the same host. | Ambiguous motor ownership is rejected before physics writes. | Manual | Not run |
| ECTR-LAB-042 | Top-Down 2D | Top-down control lease | Acquire and release control in LeaseRequired mode. | Only the current control generation may feed movement. | Manual | Not run |
| ECTR-LAB-043 | Top-Down 2D | Top-down stale lease | Transfer control, then dispose the prior lease. | The stale lease cannot revoke the new control generation. | Manual | Not run |
| ECTR-LAB-044 | Top-Down 2D | Top-down stale intent | Submit packets out of sequence. | Older packets are ignored without reversing facing or velocity. | Manual | Not run |
| ECTR-LAB-045 | Top-Down 2D | Top-down idle | Provide zero movement intent. | Velocity settles to zero and the state reports Idle. | Manual | Not run |
| ECTR-LAB-046 | Top-Down 2D | Cardinal movement | Drive north, south, east, and west. | Motion follows the normalized intent and configured acceleration policy. | Manual | Not run |
| ECTR-LAB-047 | Top-Down 2D | Normalized diagonal movement | Move diagonally with NormalizeDiagonal enabled. | Resulting speed does not exceed cardinal maximum speed. | Manual | Not run |
| ECTR-LAB-048 | Top-Down 2D | Preserve diagonal magnitude | Move diagonally with PreserveInputMagnitude enabled. | The selected policy is applied explicitly and reported in diagnostics. | Manual | Not run |
| ECTR-LAB-049 | Top-Down 2D | Top-down acceleration | Ramp from zero to full movement. | Velocity approaches the configured target without overshoot beyond tolerance. | Manual | Not run |
| ECTR-LAB-050 | Top-Down 2D | Top-down deceleration | Release input from maximum speed. | Velocity approaches zero under the configured deceleration policy. | Manual | Not run |
| ECTR-LAB-051 | Top-Down 2D | Top-down speed clamp | Hold full movement beyond acceleration time. | Motor-owned speed remains within the configured maximum. | Manual | Not run |
| ECTR-LAB-052 | Top-Down 2D | Four-direction facing | Move through diagonal vectors under FourDirection facing. | Facing resolves deterministically to one cardinal direction. | Manual | Not run |
| ECTR-LAB-053 | Top-Down 2D | Eight-direction facing | Move through all octants under EightDirection facing. | Facing resolves to the expected octant without jitter around thresholds. | Manual | Not run |
| ECTR-LAB-054 | Top-Down 2D | Independent look facing | Provide movement and look vectors in different directions. | Facing follows the configured look source while movement remains independent. | Manual | Not run |
| ECTR-LAB-055 | Top-Down 2D | Preserve last facing | Return movement and look to zero. | The last valid facing remains available for animation and interaction seams. | Manual | Not run |
| ECTR-LAB-056 | Top-Down 2D | Collision slide | Move diagonally into a wall. | Rigidbody2D collision response and motor policy produce stable tangent movement without transform tunneling. | Manual | Not run |
| ECTR-LAB-057 | Top-Down 2D | Top-down movement constraint | Enable rectangular bounds and move against each edge. | The actor remains inside the constraint and state remains coherent. | Manual | Not run |
| ECTR-LAB-058 | Top-Down 2D | Top-down external velocity | Apply an external velocity change while moving. | The motor combines or replaces velocity according to the explicit request policy. | Manual | Not run |
| ECTR-LAB-059 | Top-Down 2D | Top-down warp | Warp to a new valid position. | Position, velocity reset policy, facing policy, and warp revision commit atomically. | Manual | Not run |
| ECTR-LAB-060 | Top-Down 2D | Top-down pause and resume | Disable control, submit movement, then re-enable. | Buffered stale movement does not fire after resume. | Manual | Not run |
| ECTR-LAB-061 | Top-Down 2D | Disable and re-enable host | Disable the controller GameObject and enable it again. | Subscriptions, registrations, and state reset according to documented lifecycle. | Manual | Not run |
| ECTR-LAB-062 | Top-Down 2D | Multiple independent actors | Run several top-down controllers in one scene. | Each actor owns only its local state and no global singleton collision occurs. | Manual | Not run |
| ECTR-LAB-063 | Top-Down 2D | Scripted square path | Run the deterministic sample intent script. | The actor completes the expected bounded route and publishes state transitions. | Manual | Not run |
| ECTR-LAB-064 | Top-Down 2D | Reset Top-Down Lab | Mutate settings, run, then invoke Reset. | Configuration assets remain immutable and runtime state returns to baseline. | Manual | Not run |
| ECTR-LAB-065 | Top-Down 2D | Reload Top-Down scene | Reload the Laboratory repeatedly. | No stale source, control, or event registration survives the actor lifecycle. | Manual | Not run |
| ECTR-LAB-066 | Top-Down 2D | No animation presenter | Remove the optional sample animator presenter. | The controller continues to move and report semantic state. | Manual | Not run |
| ECTR-LAB-067 | Top-Down 2D | No peer packages installed | Run the Laboratory without Fellowship, Will, Eye, Pulse, Impact, or Observatory. | The core preset remains fully usable and diagnosable. | Manual | Not run |
| ECTR-LAB-068 | Top-Down 2D | Inspect top-down diagnostics | Open the runtime readout while moving and colliding. | The readout explains effective source, control state, intent age, velocity, facing, constraints, and health. | Manual | Not run |

### 13.4 Optional integration samples

| Sample | Packages | Purpose | Why not standalone proof |
|---|---|---|---|
| Fellowship + Vessel Control Handoff | EchoCharacters bridge artifact | Switch control between actors | Depends on both peers and bridge |
| Will + Vessel Input | EchoInput adapter/bridge | Map action contexts into family intent | Depends on The Will/Input System |
| Eye + Vessel Target Handoff | EchoCamera bridge | Publish facing, velocity, warp | Depends on The Eye |
| Animator Presentation | Sample-only/project presenter | Demonstrate semantic animation mapping | Presentation is not controller truth |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The runtime core is nonvisual. It publishes semantic state and events. Project code or optional sample presenters control sprites, models, Animator parameters, VFX, SFX, trails, dust, HUDs, and camera effects. The Editor and Labs provide diagnostics only.

### 14.2 Required states

- Unconfigured
- Initializing
- Ready/controlled
- Ready/uncontrolled
- Moving
- Grounded/airborne where applicable
- Suspended/disabled
- Blocked/faulted
- Source unavailable/stale
- Constraint/contact state

### 14.3 Accessibility requirements

- Controllers must accept remapped/alternative input through adapters rather than require specific buttons.
- Hold/toggle behavior belongs to the input adapter or capability policy and must be explicit.
- Reduced motion may scale camera/feedback through their authorities; core locomotion must not silently change without a gameplay accessibility decision.
- Debug readouts must not rely on color alone.
- Timing windows are configurable and documented.
- Labs provide scripted controls for users who cannot use the optional interactive adapter.

### 14.4 Visual customization

Samples use replaceable visuals. No runtime rule depends on sprite pivot, Animator Controller, model hierarchy, or art direction except where a project adapter explicitly supplies required transforms.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Controller snapshot | Public API/Inspector | Runtime/dev | Low |
| Source/control generations | Monitor/readout | Development | Low |
| Intent age/sequence | Monitor/readout | Development | Low |
| Velocity/facing/state | API/readout | Runtime/dev | Low |
| Ground/slope/contact | Gizmo/readout | Development | Configurable |
| Constraint/external requests | Monitor/history | Development | Bounded |
| Faults/validation | Logs/report | Editor/runtime | Event-driven |

### 15.2 Structured status

Every host exposes configuration ID, runtime ID, preset family, readiness, control mode/generation, winning source ID/generation, last accepted intent sequence/age, effective motor state, velocity, facing, capability health, constraint/contact state, warp revision, bounded recent events, and version metadata.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| ECTR-DIAG-001 | Blocker | Missing/invalid configuration | Assign or repair project configuration |
| ECTR-DIAG-002 | Blocker | Missing required physics component | Add supported component through setup preview |
| ECTR-DIAG-003 | Blocker | Unsupported physics body/configuration | Choose supported MVP policy or later backend |
| ECTR-DIAG-004 | Blocker | Duplicate actor authority | Remove/resolve duplicate host or motor |
| ECTR-DIAG-005 | Advisory/Warning | Stale source/control handle | Release current lease path correctly |
| ECTR-DIAG-006 | Advisory | Stale/out-of-order intent packet | Inspect adapter sequence generation |
| ECTR-DIAG-007 | Info/Warning | No valid intent source/control | Connect source or acquire control |
| ECTR-DIAG-008 | Error | Ground probe unavailable/invalid | Repair probe/filter/collider setup |
| ECTR-DIAG-009 | Error/Critical | Capability failed | Inspect capability and rollback/fault policy |
| ECTR-DIAG-010 | Warning | Actor/component destroyed unexpectedly | Inspect scene lifecycle/project destruction |
| ECTR-DIAG-011 | Warning | Intent too old | Inspect pause, adapter, and buffer policy |
| ECTR-DIAG-012 | Warning | Motion request rejected/clamped | Inspect request limits/combine mode |
| ECTR-DIAG-013 | Warning | Warp rejected | Inspect target, bounds, and request policy |
| ECTR-DIAG-014 | Error | Shared configuration mutation detected | Restore asset and move state to runtime |
| ECTR-DIAG-015 | Warning | Capability order/dependency invalid | Repair declared composition |

### 15.4 Observatory bridge

A separate bridge registers bounded controller providers with The Observatory. It translates health, source/control state, intent age, velocity, facing, contacts, capabilities, and diagnostic counts. The core never references EchoDiagnostics.

### 15.5 Logging policy

No per-frame log spam. Repeated conditions aggregate under bounded counters and transition logs. Release-safe diagnostics omit raw key/button values, project hierarchy paths, private filesystem paths, and unrestricted position histories. Exact position may be included only in explicitly approved development snapshots.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Controller configurations | Project asset | Project/The Vessel types | Asset | Unity asset serialization |
| Live velocity/contacts/intents | Session | Controller actor | No | N/A |
| Control/source leases | Session | Controller actor/bridge | No | N/A |
| Character/world position | Game/world truth | Project/Fellowship/future Atlas | Project decision | Chronicle/project |
| Player control preferences | Global | The Accord/Will/project | Optional | Settings backend |
| Diagnostic history | Bounded session | Controller/Observatory bridge | No by default | Optional report |

### 16.2 Standalone behavior

The package requires no save or settings package. Configuration assets provide defaults. Live movement reconstructs from current scene/actor state. A project may apply preferences through adapters without transferring persistence authority.

### 16.3 Optional participant/provider contract

No EchoControllers save participant ships in the MVP. Projects may save semantic actor pose or custom traversal state through their own Chronicle participant. A future family may define a narrow versioned payload only after proving which state is durable and safe to restore.

### 16.4 Failure and recovery

Missing or old configuration assets are handled by Editor migration/validation. Live controller state is never trusted from arbitrary save data. Unknown future configuration fields follow SFGSS-003 preservation rules. Removing the package does not delete project-owned assets automatically; removal guidance identifies components/configurations that the project must migrate or remove.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Every peer connection is explicit, removable, and dependency-visible. Core movement remains available without any integration. Bridges translate committed semantic truth and own every registration/lease they create.

### 17.2 Planned integrations

| Other authority | Connection | Bridge owner | Direction | Data/events | Required? |
|---|---|---|---|---|---:|
| The Will | Separate adapter/bridge | Adapter artifact | Will -> Vessel | Action values/context/control -> family intent | No |
| The Fellowship | Separate bridge | Bridge artifact | Fellowship -> Vessel | Control assignment/switch -> controller lease | No |
| The Eye | Separate bridge | Bridge artifact | Vessel -> Eye | Target pose, velocity, facing, warp revision | No |
| The Pulse | Separate bridge/project | Bridge/project | Pulse -> Vessel | Pause/cutscene policy -> control suspension | No |
| Impact | Project/bridge | Bridge/project | Impact/Project -> Vessel | External motion requests; controller events outward | No |
| Resonance | Project/bridge | Bridge/project | Vessel -> Resonance | Semantic footstep/jump/land cues through project mapping | No |
| The Hand | Project adapter | Project | Bidirectional semantic seams | Facing/origin, temporary movement lock | No |
| The Observatory | Separate bridge | Bridge artifact | Vessel -> Observatory | Bounded diagnostics provider | No |
| The Workshop | Editor setup facade | Package Editor | Workshop -> Vessel Editor | Plan/apply/receipt | No runtime dependency |
| Future Convergence | Provider adapter | Multiplayer family | Bidirectional | Authority, input, correction, prediction seams | No |

### 17.3 Bridge placement decisions

- Input System adapter: separate provider/adapter package because it adds a package dependency.
- The Will/Fellowship/Eye/Observatory integrations: separate bridges because they reference independent Echo packages.
- Animator mapping: sample or project adapter unless a neutral reusable presenter is later proven.
- Game-specific role capabilities remain project code until generalized by at least two independent uses and a dedicated capability specification.

### 17.4 Integration failure behavior

Missing bridges leave the standalone path intact. A bridge attaches only when both peers are Ready, invalidates stale generations on teardown, and never destroys a valid peer authority it does not own. Failed Fellowship handoff leaves its old authoritative owner intact according to the bridge transaction. Failed Eye/Observatory publication does not stop movement. Version mismatch reports through bridge-owned diagnostics.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Planned target | Measurement | Release threshold |
|---|---|---|---|
| Motor allocations after warmup | 0 B per FixedUpdate for MVP hot path | Profiler/GC recorder | Must pass before stable claim |
| Intent intake allocations | 0 B per accepted packet after warmup | Profiler | Must pass before stable claim |
| Ground probe queries | Fixed configured maximum per step | Lab/profiler | No unbounded query growth |
| Event/diagnostic histories | Bounded configurable capacity | Stress Lab | No unbounded growth |
| 100 actor stress fixture | Measured, not preclaimed | Stress scene | Budget approved during implementation |

### 18.2 Allocation policy

No LINQ, reflection, per-step boxing, unbounded lists, hierarchy searches, or string formatting in motor hot paths. Contact/cast buffers are preallocated or safely reused. Snapshot/event publication avoids allocations when unchanged where practical. Diagnostics sampling is bounded and configurable.

### 18.3 Scene and domain reload behavior

Actor components unsubscribe and invalidate registrations on disable/destroy. Static caches, if any are approved later, must reset under supported Enter Play Mode settings. The package must pass repeated scene load/unload and domain-reload configuration tests without stale controllers or intent sources.

### 18.4 Scalability limits

Supported actor counts, source counts, capability counts, query budgets, and stress thresholds remain Not run until measured. Defaults are bounded. The package rejects source/capability capacity overflow rather than growing indefinitely.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

The package handles movement intent, actor pose/velocity, and diagnostics. It does not handle credentials, analytics, purchases, platform identities, or personal profiles. Raw typed text, full input histories, device serials, and account identifiers do not belong in controller diagnostics.

### 19.2 Trust boundaries

- Validate adapter packets for finite values, age, sequence, family, generation, and configured magnitude.
- Clamp/reject NaN, Infinity, impossible warp, and unsafe external-motion requests.
- Network-originated intent is untrusted until a future Convergence adapter validates authority.
- ScriptableObjects and scene references are configuration, not proof of gameplay permission.

### 19.3 Platform behavior

| Platform | Status | Special behavior | Evidence |
|---|---|---|---|
| Windows Editor/Player | Planned | Primary development target | Not run |
| macOS/Linux | Planned | Unity Physics2D behavior must be tested | Not run |
| WebGL | Planned/conditional | Performance/input adapter differences | Not run |
| Mobile | Planned/conditional | Touch adapter and fixed-step budgets | Not run |
| Console | Unknown | SDK/device/provider evidence required | Not run |
| Dedicated server/headless | Experimental future | Controller/physics applicability project-specific | Not run |

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-controllers/
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
│   ├── ControllerHost.cs
│   ├── IEchoController.cs
│   ├── Control/
│   ├── Intent/
│   ├── State/
│   ├── Motion/
│   └── Diagnostics/
├── Physics2D/
│   ├── Grounding/
│   ├── Facing/
│   ├── Constraints/
│   └── Capabilities/
├── SideView2D/
│   ├── SideView2DController.cs
│   ├── SideView2DMotor.cs
│   ├── SideView2DIntentFrame.cs
│   └── SideView2DStateSnapshot.cs
├── TopDown2D/
│   ├── TopDown2DController.cs
│   ├── TopDown2DMotor.cs
│   ├── TopDown2DIntentFrame.cs
│   └── TopDown2DStateSnapshot.cs
└── Configuration/

Editor/
├── Setup/
├── Validation/
├── Inspectors/
├── Monitoring/
├── Migration/
└── WorkshopFacade/

Samples~/
├── Standalone Labs/
│   ├── Side-View 2D Controller Lab/
│   └── Top-Down 2D Controller Lab/
└── Scripted Intent Drivers/

Tests/
├── Editor/
└── Runtime/
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoControllers.Runtime` | Runtime | Unity Engine core | Yes | Shared host, intent, control, state, diagnostics contracts |
| `EchoDevGames.EchoControllers.Physics2D.Runtime` | Runtime | Core, Unity Physics2D | No | Shared 2D probes/capabilities/motion utilities |
| `EchoDevGames.EchoControllers.SideView2D.Runtime` | Runtime | Core, Physics2D | No | Side-view preset |
| `EchoDevGames.EchoControllers.TopDown2D.Runtime` | Runtime | Core, Physics2D | No | Top-down preset |
| `EchoDevGames.EchoControllers.Editor` | Editor | Runtime preset assemblies, UnityEditor | No | Setup, validation, monitoring, migration, facade |
| `EchoDevGames.EchoControllers.Tests.Editor` | Editor tests | Runtime/Editor, Test Framework | No | EditMode tests |
| `EchoDevGames.EchoControllers.Tests.Runtime` | Runtime tests | Runtime preset assemblies, Test Framework | No | PlayMode tests |
| Sample assemblies | Sample | Declared runtime preset only | No | Labs and scripted drivers |

The public core runtime remains easy to reference; optional preset assemblies are explicit. GUID-based asmdef references and stable `.meta` files follow SFGSS-002.

### 20.4 Repository files

README, quick start, architecture, preset guides, intent adapter guide, capability authoring guide, diagnostics reference, Lab guides, migration/removal guide, Current Notes, ADRs, test records, changelog, license, notices, release checklist, and stable asset metadata.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | 6000.0 planned | 6000.3.8f1 development baseline | Empirical evidence Not run |
| Unity Physics2D | Matching Unity module | Not run | MVP preset dependency |
| Input System adapter | To be selected/tested | Not run | Separate package |

### 21.2 Semantic versioning policy

- Patch: fixes preserving public APIs, behavior contracts, stable IDs, and serialized assets.
- Minor: additive presets, capabilities, APIs, diagnostics, or configuration fields with migration.
- Major: breaking public API, intent payload, motor contract, serialized schema, stable ID, or supported-physics change.
- A new controller family may be a minor addition inside the package only if dependencies/release cadence remain compatible; otherwise it triggers package-family review.

### 21.3 Deprecation policy

Deprecated APIs/configuration fields remain documented with analyzer/validator guidance, migration path, replacement, and removal version. Serialized intent/config IDs are not recycled. Supported migration fixtures remain immutable.

### 21.4 GUID and asset compatibility

Public scripts, asmdefs, configuration templates, samples, and prefabs preserve committed `.meta` files. Moves/renames retain GUIDs when identity survives. Domain IDs remain separate from Unity asset GUIDs.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview and boundaries.
- Five-minute Side-View 2D quick start.
- Five-minute Top-Down 2D quick start.
- Component and configuration reference.
- Control and intent-source guide.
- Ground/jump/slope/facing guide.
- Warp/external-motion guide.
- Independent Lab guides.
- Diagnostics and troubleshooting.
- Input System adapter and peer bridge index.
- Migration/removal guide.
- Known limitations and unsupported families.

### 22.2 Required developer documentation

- Rootless actor-bound architecture.
- Update/fixed-step and buffering lifecycle.
- Public intent/motor/state APIs.
- Capability compatibility and ordering.
- Physics assumptions and failure policies.
- Test strategy and evidence registry.
- Assembly/dependency map.
- Workshop setup facade.
- Current Notes and checkpoint status.

### 22.3 Documentation truth rule

Every code example must compile against the released API. Screenshots, performance claims, supported counts, adapter versions, and platform claims remain absent or `Not run` until evidence exists. Side-View and Top-Down docs never imply parity with deferred families.

### 22.4 Living repository and Obsidian workflow

The package repository opens directly as an Obsidian-compatible Markdown vault/folder. Current Notes captures observations and proposals. Durable changes promote to the specification, ADR, issue, test record, guide, changelog, or release record at checkpoint closeout.

### 22.5 Repository scan and handoff order

README -> SFGSS-000 -> SFGSS-002/003/004/005 -> The Vessel specification -> applicable ADRs/bridges -> Current Notes -> checkpoint/test/issue/changelog -> relevant implementation/tests.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | MVP required? |
|---|---|---|---:|
| EditMode unit | IDs, configuration, intent freshness, policies, state calculation | Source arbitration, slope classification, diagonal/facing policies | Yes |
| PlayMode unit/integration | Actor lifecycle, motors, physics, leases, events | Jump/landing, collisions, warps, resets | Yes |
| Standalone Laboratories | Separate user-visible preset proof | Side-View 2D and Top-Down 2D | Yes |
| Bridge Integration Labs | Will/Fellowship/Eye/etc. | Owned by bridge artifact | When advertised |
| Clean-project install | Packaging and dependency proof | Git/local/tarball | Yes |
| Adoption parity | Existing project replacement | Rescuers2D/Hackulos later | Before integration claim |

### 23.2 Required test categories

- Configuration and stable IDs.
- Actor component and physics validation.
- Source/control generations and stale requests.
- Intent buffering, edge semantics, and pause/resume.
- Fixed-step motor execution and no hidden transform writes.
- Grounding, slopes, jump, facing, collisions, constraints, warp, and external motion.
- Multiple actors, lifecycle, scene reload, Enter Play Mode options, and static reset.
- Missing peers, bridge removal, sample removal, clean installation, and package removal.
- Allocation/performance/capacity evidence.
- Documentation usability and Laboratory reset.

### 23.3 Test case registry

| Test ID | Requirements | Layer | Setup | Action | Expected result | Status |
|---|---|---|---|---|---|---|
| ECTR-T-001 | ECTR-LAB-001; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Initialize Side-View 2D controller | Validate configuration, stable IDs, required references, and supported preset boundaries for: Initialize Side-View 2D controller. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-002 | ECTR-LAB-001; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Initialize Side-View 2D controller | Exercise pure policy and state calculations for: Initialize Side-View 2D controller. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-003 | ECTR-LAB-001; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Initialize Side-View 2D controller | Run the normal runtime workflow for: Initialize Side-View 2D controller. | The host, motor, ground probe, jump capability, facing capability, scripted intent driver, and readout become Ready. | Not run |
| ECTR-T-004 | ECTR-LAB-001; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Initialize Side-View 2D controller | Inject the closest approved invalid, stale, missing, or interrupted condition for: Initialize Side-View 2D controller. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-005 | ECTR-LAB-001; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Initialize Side-View 2D controller | Repeat, disable/enable, reset, or reload around: Initialize Side-View 2D controller. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-006 | ECTR-LAB-001; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Initialize Side-View 2D controller | Follow the documented Laboratory action for: Initialize Side-View 2D controller. | The host, motor, ground probe, jump capability, facing capability, scripted intent driver, and readout become Ready. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-007 | ECTR-LAB-002; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Missing Side-View configuration | Validate configuration, stable IDs, required references, and supported preset boundaries for: Missing Side-View configuration. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-008 | ECTR-LAB-002; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Missing Side-View configuration | Exercise pure policy and state calculations for: Missing Side-View configuration. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-009 | ECTR-LAB-002; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Side-View configuration | Run the normal runtime workflow for: Missing Side-View configuration. | Initialization is blocked with an actionable ECTR diagnostic and no movement side effects. | Not run |
| ECTR-T-010 | ECTR-LAB-002; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Side-View configuration | Inject the closest approved invalid, stale, missing, or interrupted condition for: Missing Side-View configuration. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-011 | ECTR-LAB-002; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Side-View configuration | Repeat, disable/enable, reset, or reload around: Missing Side-View configuration. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-012 | ECTR-LAB-002; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Missing Side-View configuration | Follow the documented Laboratory action for: Missing Side-View configuration. | Initialization is blocked with an actionable ECTR diagnostic and no movement side effects. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-013 | ECTR-LAB-003; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Missing Rigidbody2D | Validate configuration, stable IDs, required references, and supported preset boundaries for: Missing Rigidbody2D. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-014 | ECTR-LAB-003; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Missing Rigidbody2D | Exercise pure policy and state calculations for: Missing Rigidbody2D. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-015 | ECTR-LAB-003; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Rigidbody2D | Run the normal runtime workflow for: Missing Rigidbody2D. | Validation blocks the controller and identifies the missing required component. | Not run |
| ECTR-T-016 | ECTR-LAB-003; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Rigidbody2D | Inject the closest approved invalid, stale, missing, or interrupted condition for: Missing Rigidbody2D. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-017 | ECTR-LAB-003; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Rigidbody2D | Repeat, disable/enable, reset, or reload around: Missing Rigidbody2D. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-018 | ECTR-LAB-003; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Missing Rigidbody2D | Follow the documented Laboratory action for: Missing Rigidbody2D. | Validation blocks the controller and identifies the missing required component. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-019 | ECTR-LAB-004; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Missing Collider2D | Validate configuration, stable IDs, required references, and supported preset boundaries for: Missing Collider2D. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-020 | ECTR-LAB-004; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Missing Collider2D | Exercise pure policy and state calculations for: Missing Collider2D. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-021 | ECTR-LAB-004; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Collider2D | Run the normal runtime workflow for: Missing Collider2D. | Validation blocks ground and movement execution without adding hidden project content. | Not run |
| ECTR-T-022 | ECTR-LAB-004; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Collider2D | Inject the closest approved invalid, stale, missing, or interrupted condition for: Missing Collider2D. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-023 | ECTR-LAB-004; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Missing Collider2D | Repeat, disable/enable, reset, or reload around: Missing Collider2D. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-024 | ECTR-LAB-004; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Missing Collider2D | Follow the documented Laboratory action for: Missing Collider2D. | Validation blocks ground and movement execution without adding hidden project content. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-025 | ECTR-LAB-005; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Invalid Rigidbody2D body type | Validate configuration, stable IDs, required references, and supported preset boundaries for: Invalid Rigidbody2D body type. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-026 | ECTR-LAB-005; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Invalid Rigidbody2D body type | Exercise pure policy and state calculations for: Invalid Rigidbody2D body type. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-027 | ECTR-LAB-005; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Invalid Rigidbody2D body type | Run the normal runtime workflow for: Invalid Rigidbody2D body type. | The controller reports UnsupportedConfiguration and does not write motion. | Not run |
| ECTR-T-028 | ECTR-LAB-005; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Invalid Rigidbody2D body type | Inject the closest approved invalid, stale, missing, or interrupted condition for: Invalid Rigidbody2D body type. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-029 | ECTR-LAB-005; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Invalid Rigidbody2D body type | Repeat, disable/enable, reset, or reload around: Invalid Rigidbody2D body type. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-030 | ECTR-LAB-005; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Invalid Rigidbody2D body type | Follow the documented Laboratory action for: Invalid Rigidbody2D body type. | The controller reports UnsupportedConfiguration and does not write motion. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-031 | ECTR-LAB-006; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Duplicate Side-View motor | Validate configuration, stable IDs, required references, and supported preset boundaries for: Duplicate Side-View motor. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-032 | ECTR-LAB-006; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Duplicate Side-View motor | Exercise pure policy and state calculations for: Duplicate Side-View motor. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-033 | ECTR-LAB-006; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Duplicate Side-View motor | Run the normal runtime workflow for: Duplicate Side-View motor. | The actor refuses ambiguous motor ownership before FixedUpdate side effects. | Not run |
| ECTR-T-034 | ECTR-LAB-006; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Duplicate Side-View motor | Inject the closest approved invalid, stale, missing, or interrupted condition for: Duplicate Side-View motor. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-035 | ECTR-LAB-006; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Duplicate Side-View motor | Repeat, disable/enable, reset, or reload around: Duplicate Side-View motor. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-036 | ECTR-LAB-006; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Duplicate Side-View motor | Follow the documented Laboratory action for: Duplicate Side-View motor. | The actor refuses ambiguous motor ownership before FixedUpdate side effects. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-037 | ECTR-LAB-007; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Always-controlled easy path | Validate configuration, stable IDs, required references, and supported preset boundaries for: Always-controlled easy path. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-038 | ECTR-LAB-007; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Always-controlled easy path | Exercise pure policy and state calculations for: Always-controlled easy path. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-039 | ECTR-LAB-007; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Always-controlled easy path | Run the normal runtime workflow for: Always-controlled easy path. | The scripted intent source can drive the motor without Fellowship or project possession code. | Not run |
| ECTR-T-040 | ECTR-LAB-007; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Always-controlled easy path | Inject the closest approved invalid, stale, missing, or interrupted condition for: Always-controlled easy path. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-041 | ECTR-LAB-007; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Always-controlled easy path | Repeat, disable/enable, reset, or reload around: Always-controlled easy path. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-042 | ECTR-LAB-007; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Always-controlled easy path | Follow the documented Laboratory action for: Always-controlled easy path. | The scripted intent source can drive the motor without Fellowship or project possession code. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-043 | ECTR-LAB-008; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Acquire and release control lease | Validate configuration, stable IDs, required references, and supported preset boundaries for: Acquire and release control lease. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-044 | ECTR-LAB-008; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Acquire and release control lease | Exercise pure policy and state calculations for: Acquire and release control lease. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-045 | ECTR-LAB-008; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Acquire and release control lease | Run the normal runtime workflow for: Acquire and release control lease. | Intent is accepted only while the current lease is valid. | Not run |
| ECTR-T-046 | ECTR-LAB-008; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Acquire and release control lease | Inject the closest approved invalid, stale, missing, or interrupted condition for: Acquire and release control lease. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-047 | ECTR-LAB-008; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Acquire and release control lease | Repeat, disable/enable, reset, or reload around: Acquire and release control lease. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-048 | ECTR-LAB-008; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Acquire and release control lease | Follow the documented Laboratory action for: Acquire and release control lease. | Intent is accepted only while the current lease is valid. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-049 | ECTR-LAB-009; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Stale control lease | Validate configuration, stable IDs, required references, and supported preset boundaries for: Stale control lease. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-050 | ECTR-LAB-009; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Stale control lease | Exercise pure policy and state calculations for: Stale control lease. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-051 | ECTR-LAB-009; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Stale control lease | Run the normal runtime workflow for: Stale control lease. | The stale lease cannot disable the current controller authority. | Not run |
| ECTR-T-052 | ECTR-LAB-009; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Stale control lease | Inject the closest approved invalid, stale, missing, or interrupted condition for: Stale control lease. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-053 | ECTR-LAB-009; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Stale control lease | Repeat, disable/enable, reset, or reload around: Stale control lease. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-054 | ECTR-LAB-009; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Stale control lease | Follow the documented Laboratory action for: Stale control lease. | The stale lease cannot disable the current controller authority. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-055 | ECTR-LAB-010; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Out-of-order source release | Validate configuration, stable IDs, required references, and supported preset boundaries for: Out-of-order source release. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-056 | ECTR-LAB-010; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Out-of-order source release | Exercise pure policy and state calculations for: Out-of-order source release. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-057 | ECTR-LAB-010; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Out-of-order source release | Run the normal runtime workflow for: Out-of-order source release. | The effective source is recomputed from active registrations rather than restored from stale state. | Not run |
| ECTR-T-058 | ECTR-LAB-010; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Out-of-order source release | Inject the closest approved invalid, stale, missing, or interrupted condition for: Out-of-order source release. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-059 | ECTR-LAB-010; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Out-of-order source release | Repeat, disable/enable, reset, or reload around: Out-of-order source release. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-060 | ECTR-LAB-010; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Out-of-order source release | Follow the documented Laboratory action for: Out-of-order source release. | The effective source is recomputed from active registrations rather than restored from stale state. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-061 | ECTR-LAB-011; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Reject stale intent sequence | Validate configuration, stable IDs, required references, and supported preset boundaries for: Reject stale intent sequence. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-062 | ECTR-LAB-011; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Reject stale intent sequence | Exercise pure policy and state calculations for: Reject stale intent sequence. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-063 | ECTR-LAB-011; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject stale intent sequence | Run the normal runtime workflow for: Reject stale intent sequence. | The stale packet is ignored and diagnosed without rewinding movement. | Not run |
| ECTR-T-064 | ECTR-LAB-011; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject stale intent sequence | Inject the closest approved invalid, stale, missing, or interrupted condition for: Reject stale intent sequence. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-065 | ECTR-LAB-011; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject stale intent sequence | Repeat, disable/enable, reset, or reload around: Reject stale intent sequence. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-066 | ECTR-LAB-011; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Reject stale intent sequence | Follow the documented Laboratory action for: Reject stale intent sequence. | The stale packet is ignored and diagnosed without rewinding movement. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-067 | ECTR-LAB-012; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Expire buffered command | Validate configuration, stable IDs, required references, and supported preset boundaries for: Expire buffered command. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-068 | ECTR-LAB-012; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Expire buffered command | Exercise pure policy and state calculations for: Expire buffered command. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-069 | ECTR-LAB-012; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Expire buffered command | Run the normal runtime workflow for: Expire buffered command. | The expired command does not trigger a delayed jump. | Not run |
| ECTR-T-070 | ECTR-LAB-012; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Expire buffered command | Inject the closest approved invalid, stale, missing, or interrupted condition for: Expire buffered command. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-071 | ECTR-LAB-012; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Expire buffered command | Repeat, disable/enable, reset, or reload around: Expire buffered command. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-072 | ECTR-LAB-012; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Expire buffered command | Follow the documented Laboratory action for: Expire buffered command. | The expired command does not trigger a delayed jump. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-073 | ECTR-LAB-013; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Idle state | Validate configuration, stable IDs, required references, and supported preset boundaries for: Idle state. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-074 | ECTR-LAB-013; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Idle state | Exercise pure policy and state calculations for: Idle state. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-075 | ECTR-LAB-013; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Idle state | Run the normal runtime workflow for: Idle state. | Velocity settles under configured deceleration and the state reports GroundedIdle. | Not run |
| ECTR-T-076 | ECTR-LAB-013; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Idle state | Inject the closest approved invalid, stale, missing, or interrupted condition for: Idle state. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-077 | ECTR-LAB-013; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Idle state | Repeat, disable/enable, reset, or reload around: Idle state. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-078 | ECTR-LAB-013; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Idle state | Follow the documented Laboratory action for: Idle state. | Velocity settles under configured deceleration and the state reports GroundedIdle. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-079 | ECTR-LAB-014; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ground acceleration | Validate configuration, stable IDs, required references, and supported preset boundaries for: Ground acceleration. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-080 | ECTR-LAB-014; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ground acceleration | Exercise pure policy and state calculations for: Ground acceleration. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-081 | ECTR-LAB-014; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground acceleration | Run the normal runtime workflow for: Ground acceleration. | Velocity approaches the configured ground-speed target without exceeding it. | Not run |
| ECTR-T-082 | ECTR-LAB-014; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground acceleration | Inject the closest approved invalid, stale, missing, or interrupted condition for: Ground acceleration. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-083 | ECTR-LAB-014; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground acceleration | Repeat, disable/enable, reset, or reload around: Ground acceleration. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-084 | ECTR-LAB-014; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Ground acceleration | Follow the documented Laboratory action for: Ground acceleration. | Velocity approaches the configured ground-speed target without exceeding it. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-085 | ECTR-LAB-015; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ground deceleration | Validate configuration, stable IDs, required references, and supported preset boundaries for: Ground deceleration. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-086 | ECTR-LAB-015; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ground deceleration | Exercise pure policy and state calculations for: Ground deceleration. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-087 | ECTR-LAB-015; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground deceleration | Run the normal runtime workflow for: Ground deceleration. | Velocity approaches zero under the configured deceleration policy. | Not run |
| ECTR-T-088 | ECTR-LAB-015; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground deceleration | Inject the closest approved invalid, stale, missing, or interrupted condition for: Ground deceleration. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-089 | ECTR-LAB-015; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground deceleration | Repeat, disable/enable, reset, or reload around: Ground deceleration. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-090 | ECTR-LAB-015; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Ground deceleration | Follow the documented Laboratory action for: Ground deceleration. | Velocity approaches zero under the configured deceleration policy. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-091 | ECTR-LAB-016; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ground speed clamp | Validate configuration, stable IDs, required references, and supported preset boundaries for: Ground speed clamp. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-092 | ECTR-LAB-016; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ground speed clamp | Exercise pure policy and state calculations for: Ground speed clamp. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-093 | ECTR-LAB-016; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground speed clamp | Run the normal runtime workflow for: Ground speed clamp. | Horizontal motor-owned speed remains within the configured limit. | Not run |
| ECTR-T-094 | ECTR-LAB-016; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground speed clamp | Inject the closest approved invalid, stale, missing, or interrupted condition for: Ground speed clamp. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-095 | ECTR-LAB-016; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground speed clamp | Repeat, disable/enable, reset, or reload around: Ground speed clamp. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-096 | ECTR-LAB-016; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Ground speed clamp | Follow the documented Laboratory action for: Ground speed clamp. | Horizontal motor-owned speed remains within the configured limit. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-097 | ECTR-LAB-017; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Facing changes | Validate configuration, stable IDs, required references, and supported preset boundaries for: Facing changes. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-098 | ECTR-LAB-017; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Facing changes | Exercise pure policy and state calculations for: Facing changes. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-099 | ECTR-LAB-017; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Facing changes | Run the normal runtime workflow for: Facing changes. | Facing changes once after the configured threshold and emits a semantic event. | Not run |
| ECTR-T-100 | ECTR-LAB-017; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Facing changes | Inject the closest approved invalid, stale, missing, or interrupted condition for: Facing changes. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-101 | ECTR-LAB-017; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Facing changes | Repeat, disable/enable, reset, or reload around: Facing changes. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-102 | ECTR-LAB-017; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Facing changes | Follow the documented Laboratory action for: Facing changes. | Facing changes once after the configured threshold and emits a semantic event. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-103 | ECTR-LAB-018; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ground acquisition | Validate configuration, stable IDs, required references, and supported preset boundaries for: Ground acquisition. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-104 | ECTR-LAB-018; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ground acquisition | Exercise pure policy and state calculations for: Ground acquisition. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-105 | ECTR-LAB-018; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground acquisition | Run the normal runtime workflow for: Ground acquisition. | Grounded truth commits after the configured probe policy and emits GroundedChanged/Landed. | Not run |
| ECTR-T-106 | ECTR-LAB-018; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground acquisition | Inject the closest approved invalid, stale, missing, or interrupted condition for: Ground acquisition. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-107 | ECTR-LAB-018; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ground acquisition | Repeat, disable/enable, reset, or reload around: Ground acquisition. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-108 | ECTR-LAB-018; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Ground acquisition | Follow the documented Laboratory action for: Ground acquisition. | Grounded truth commits after the configured probe policy and emits GroundedChanged/Landed. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-109 | ECTR-LAB-019; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Walk off ledge | Validate configuration, stable IDs, required references, and supported preset boundaries for: Walk off ledge. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-110 | ECTR-LAB-019; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Walk off ledge | Exercise pure policy and state calculations for: Walk off ledge. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-111 | ECTR-LAB-019; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Walk off ledge | Run the normal runtime workflow for: Walk off ledge. | The state transitions to Falling without inventing a jump event. | Not run |
| ECTR-T-112 | ECTR-LAB-019; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Walk off ledge | Inject the closest approved invalid, stale, missing, or interrupted condition for: Walk off ledge. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-113 | ECTR-LAB-019; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Walk off ledge | Repeat, disable/enable, reset, or reload around: Walk off ledge. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-114 | ECTR-LAB-019; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Walk off ledge | Follow the documented Laboratory action for: Walk off ledge. | The state transitions to Falling without inventing a jump event. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-115 | ECTR-LAB-020; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Coyote-time jump | Validate configuration, stable IDs, required references, and supported preset boundaries for: Coyote-time jump. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-116 | ECTR-LAB-020; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Coyote-time jump | Exercise pure policy and state calculations for: Coyote-time jump. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-117 | ECTR-LAB-020; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Coyote-time jump | Run the normal runtime workflow for: Coyote-time jump. | The jump succeeds only inside the configured coyote window. | Not run |
| ECTR-T-118 | ECTR-LAB-020; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Coyote-time jump | Inject the closest approved invalid, stale, missing, or interrupted condition for: Coyote-time jump. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-119 | ECTR-LAB-020; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Coyote-time jump | Repeat, disable/enable, reset, or reload around: Coyote-time jump. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-120 | ECTR-LAB-020; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Coyote-time jump | Follow the documented Laboratory action for: Coyote-time jump. | The jump succeeds only inside the configured coyote window. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-121 | ECTR-LAB-021; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Buffered landing jump | Validate configuration, stable IDs, required references, and supported preset boundaries for: Buffered landing jump. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-122 | ECTR-LAB-021; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Buffered landing jump | Exercise pure policy and state calculations for: Buffered landing jump. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-123 | ECTR-LAB-021; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Buffered landing jump | Run the normal runtime workflow for: Buffered landing jump. | The jump commits on the first valid grounded step inside the buffer window. | Not run |
| ECTR-T-124 | ECTR-LAB-021; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Buffered landing jump | Inject the closest approved invalid, stale, missing, or interrupted condition for: Buffered landing jump. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-125 | ECTR-LAB-021; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Buffered landing jump | Repeat, disable/enable, reset, or reload around: Buffered landing jump. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-126 | ECTR-LAB-021; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Buffered landing jump | Follow the documented Laboratory action for: Buffered landing jump. | The jump commits on the first valid grounded step inside the buffer window. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-127 | ECTR-LAB-022; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Reject airborne jump | Validate configuration, stable IDs, required references, and supported preset boundaries for: Reject airborne jump. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-128 | ECTR-LAB-022; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Reject airborne jump | Exercise pure policy and state calculations for: Reject airborne jump. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-129 | ECTR-LAB-022; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject airborne jump | Run the normal runtime workflow for: Reject airborne jump. | The request is rejected without changing vertical velocity. | Not run |
| ECTR-T-130 | ECTR-LAB-022; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject airborne jump | Inject the closest approved invalid, stale, missing, or interrupted condition for: Reject airborne jump. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-131 | ECTR-LAB-022; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject airborne jump | Repeat, disable/enable, reset, or reload around: Reject airborne jump. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-132 | ECTR-LAB-022; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Reject airborne jump | Follow the documented Laboratory action for: Reject airborne jump. | The request is rejected without changing vertical velocity. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-133 | ECTR-LAB-023; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Variable jump release | Validate configuration, stable IDs, required references, and supported preset boundaries for: Variable jump release. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-134 | ECTR-LAB-023; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Variable jump release | Exercise pure policy and state calculations for: Variable jump release. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-135 | ECTR-LAB-023; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Variable jump release | Run the normal runtime workflow for: Variable jump release. | The upward trajectory is shortened through the approved variable-jump policy. | Not run |
| ECTR-T-136 | ECTR-LAB-023; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Variable jump release | Inject the closest approved invalid, stale, missing, or interrupted condition for: Variable jump release. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-137 | ECTR-LAB-023; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Variable jump release | Repeat, disable/enable, reset, or reload around: Variable jump release. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-138 | ECTR-LAB-023; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Variable jump release | Follow the documented Laboratory action for: Variable jump release. | The upward trajectory is shortened through the approved variable-jump policy. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-139 | ECTR-LAB-024; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ceiling interruption | Validate configuration, stable IDs, required references, and supported preset boundaries for: Ceiling interruption. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-140 | ECTR-LAB-024; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ceiling interruption | Exercise pure policy and state calculations for: Ceiling interruption. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-141 | ECTR-LAB-024; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ceiling interruption | Run the normal runtime workflow for: Ceiling interruption. | Upward motor velocity is reconciled without leaving the controller in Rising. | Not run |
| ECTR-T-142 | ECTR-LAB-024; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ceiling interruption | Inject the closest approved invalid, stale, missing, or interrupted condition for: Ceiling interruption. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-143 | ECTR-LAB-024; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ceiling interruption | Repeat, disable/enable, reset, or reload around: Ceiling interruption. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-144 | ECTR-LAB-024; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Ceiling interruption | Follow the documented Laboratory action for: Ceiling interruption. | Upward motor velocity is reconciled without leaving the controller in Rising. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-145 | ECTR-LAB-025; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Landing event | Validate configuration, stable IDs, required references, and supported preset boundaries for: Landing event. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-146 | ECTR-LAB-025; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Landing event | Exercise pure policy and state calculations for: Landing event. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-147 | ECTR-LAB-025; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Landing event | Run the normal runtime workflow for: Landing event. | One landing event reports the pre-contact fall speed and final grounded state. | Not run |
| ECTR-T-148 | ECTR-LAB-025; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Landing event | Inject the closest approved invalid, stale, missing, or interrupted condition for: Landing event. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-149 | ECTR-LAB-025; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Landing event | Repeat, disable/enable, reset, or reload around: Landing event. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-150 | ECTR-LAB-025; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Landing event | Follow the documented Laboratory action for: Landing event. | One landing event reports the pre-contact fall speed and final grounded state. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-151 | ECTR-LAB-026; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ascend walkable slope | Validate configuration, stable IDs, required references, and supported preset boundaries for: Ascend walkable slope. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-152 | ECTR-LAB-026; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Ascend walkable slope | Exercise pure policy and state calculations for: Ascend walkable slope. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-153 | ECTR-LAB-026; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ascend walkable slope | Run the normal runtime workflow for: Ascend walkable slope. | The motor preserves grounded movement and follows the walkable surface policy. | Not run |
| ECTR-T-154 | ECTR-LAB-026; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ascend walkable slope | Inject the closest approved invalid, stale, missing, or interrupted condition for: Ascend walkable slope. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-155 | ECTR-LAB-026; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Ascend walkable slope | Repeat, disable/enable, reset, or reload around: Ascend walkable slope. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-156 | ECTR-LAB-026; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Ascend walkable slope | Follow the documented Laboratory action for: Ascend walkable slope. | The motor preserves grounded movement and follows the walkable surface policy. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-157 | ECTR-LAB-027; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Descend walkable slope | Validate configuration, stable IDs, required references, and supported preset boundaries for: Descend walkable slope. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-158 | ECTR-LAB-027; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Descend walkable slope | Exercise pure policy and state calculations for: Descend walkable slope. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-159 | ECTR-LAB-027; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Descend walkable slope | Run the normal runtime workflow for: Descend walkable slope. | The ground probe maintains stable contact without oscillating between grounded and falling. | Not run |
| ECTR-T-160 | ECTR-LAB-027; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Descend walkable slope | Inject the closest approved invalid, stale, missing, or interrupted condition for: Descend walkable slope. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-161 | ECTR-LAB-027; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Descend walkable slope | Repeat, disable/enable, reset, or reload around: Descend walkable slope. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-162 | ECTR-LAB-027; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Descend walkable slope | Follow the documented Laboratory action for: Descend walkable slope. | The ground probe maintains stable contact without oscillating between grounded and falling. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-163 | ECTR-LAB-028; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Reject steep slope | Validate configuration, stable IDs, required references, and supported preset boundaries for: Reject steep slope. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-164 | ECTR-LAB-028; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Reject steep slope | Exercise pure policy and state calculations for: Reject steep slope. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-165 | ECTR-LAB-028; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject steep slope | Run the normal runtime workflow for: Reject steep slope. | The motor blocks or slides according to configuration and reports the reason. | Not run |
| ECTR-T-166 | ECTR-LAB-028; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject steep slope | Inject the closest approved invalid, stale, missing, or interrupted condition for: Reject steep slope. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-167 | ECTR-LAB-028; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reject steep slope | Repeat, disable/enable, reset, or reload around: Reject steep slope. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-168 | ECTR-LAB-028; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Reject steep slope | Follow the documented Laboratory action for: Reject steep slope. | The motor blocks or slides according to configuration and reports the reason. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-169 | ECTR-LAB-029; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Apply movement constraint | Validate configuration, stable IDs, required references, and supported preset boundaries for: Apply movement constraint. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-170 | ECTR-LAB-029; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Apply movement constraint | Exercise pure policy and state calculations for: Apply movement constraint. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-171 | ECTR-LAB-029; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Apply movement constraint | Run the normal runtime workflow for: Apply movement constraint. | The actor remains inside the approved range and the constraint reports contact. | Not run |
| ECTR-T-172 | ECTR-LAB-029; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Apply movement constraint | Inject the closest approved invalid, stale, missing, or interrupted condition for: Apply movement constraint. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-173 | ECTR-LAB-029; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Apply movement constraint | Repeat, disable/enable, reset, or reload around: Apply movement constraint. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-174 | ECTR-LAB-029; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Apply movement constraint | Follow the documented Laboratory action for: Apply movement constraint. | The actor remains inside the approved range and the constraint reports contact. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-175 | ECTR-LAB-030; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Apply external velocity change | Validate configuration, stable IDs, required references, and supported preset boundaries for: Apply external velocity change. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-176 | ECTR-LAB-030; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Apply external velocity change | Exercise pure policy and state calculations for: Apply external velocity change. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-177 | ECTR-LAB-030; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Apply external velocity change | Run the normal runtime workflow for: Apply external velocity change. | The motor applies the request through its public motion seam without claiming combat or feedback authority. | Not run |
| ECTR-T-178 | ECTR-LAB-030; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Apply external velocity change | Inject the closest approved invalid, stale, missing, or interrupted condition for: Apply external velocity change. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-179 | ECTR-LAB-030; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Apply external velocity change | Repeat, disable/enable, reset, or reload around: Apply external velocity change. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-180 | ECTR-LAB-030; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Apply external velocity change | Follow the documented Laboratory action for: Apply external velocity change. | The motor applies the request through its public motion seam without claiming combat or feedback authority. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-181 | ECTR-LAB-031; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Warp actor | Validate configuration, stable IDs, required references, and supported preset boundaries for: Warp actor. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-182 | ECTR-LAB-031; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Warp actor | Exercise pure policy and state calculations for: Warp actor. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-183 | ECTR-LAB-031; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Warp actor | Run the normal runtime workflow for: Warp actor. | Position and velocity policies apply atomically and a warp revision/event is published. | Not run |
| ECTR-T-184 | ECTR-LAB-031; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Warp actor | Inject the closest approved invalid, stale, missing, or interrupted condition for: Warp actor. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-185 | ECTR-LAB-031; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Warp actor | Repeat, disable/enable, reset, or reload around: Warp actor. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-186 | ECTR-LAB-031; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Warp actor | Follow the documented Laboratory action for: Warp actor. | Position and velocity policies apply atomically and a warp revision/event is published. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-187 | ECTR-LAB-032; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Pause and resume | Validate configuration, stable IDs, required references, and supported preset boundaries for: Pause and resume. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-188 | ECTR-LAB-032; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Pause and resume | Exercise pure policy and state calculations for: Pause and resume. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-189 | ECTR-LAB-032; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Pause and resume | Run the normal runtime workflow for: Pause and resume. | No movement command leaks during suspension and fresh intent is required after resume. | Not run |
| ECTR-T-190 | ECTR-LAB-032; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Pause and resume | Inject the closest approved invalid, stale, missing, or interrupted condition for: Pause and resume. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-191 | ECTR-LAB-032; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Pause and resume | Repeat, disable/enable, reset, or reload around: Pause and resume. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-192 | ECTR-LAB-032; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Pause and resume | Follow the documented Laboratory action for: Pause and resume. | No movement command leaks during suspension and fresh intent is required after resume. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-193 | ECTR-LAB-033; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Change fixed timestep | Validate configuration, stable IDs, required references, and supported preset boundaries for: Change fixed timestep. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-194 | ECTR-LAB-033; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Change fixed timestep | Exercise pure policy and state calculations for: Change fixed timestep. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-195 | ECTR-LAB-033; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Change fixed timestep | Run the normal runtime workflow for: Change fixed timestep. | Behavior remains bounded by configuration; exact cross-step identity is not falsely promised. | Not run |
| ECTR-T-196 | ECTR-LAB-033; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Change fixed timestep | Inject the closest approved invalid, stale, missing, or interrupted condition for: Change fixed timestep. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-197 | ECTR-LAB-033; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Change fixed timestep | Repeat, disable/enable, reset, or reload around: Change fixed timestep. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-198 | ECTR-LAB-033; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Change fixed timestep | Follow the documented Laboratory action for: Change fixed timestep. | Behavior remains bounded by configuration; exact cross-step identity is not falsely promised. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-199 | ECTR-LAB-034; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Reset and reload Side-View Lab | Validate configuration, stable IDs, required references, and supported preset boundaries for: Reset and reload Side-View Lab. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-200 | ECTR-LAB-034; controller independence/lifecycle contract | EditMode | Side-View 2D fixture for Reset and reload Side-View Lab | Exercise pure policy and state calculations for: Reset and reload Side-View Lab. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-201 | ECTR-LAB-034; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reset and reload Side-View Lab | Run the normal runtime workflow for: Reset and reload Side-View Lab. | State, subscriptions, intent buffers, and diagnostics return to a known baseline without static contamination. | Not run |
| ECTR-T-202 | ECTR-LAB-034; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reset and reload Side-View Lab | Inject the closest approved invalid, stale, missing, or interrupted condition for: Reset and reload Side-View Lab. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-203 | ECTR-LAB-034; controller independence/lifecycle contract | PlayMode | Side-View 2D fixture for Reset and reload Side-View Lab | Repeat, disable/enable, reset, or reload around: Reset and reload Side-View Lab. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-204 | ECTR-LAB-034; controller independence/lifecycle contract | Manual Laboratory | Side-View 2D fixture for Reset and reload Side-View Lab | Follow the documented Laboratory action for: Reset and reload Side-View Lab. | State, subscriptions, intent buffers, and diagnostics return to a known baseline without static contamination. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-205 | ECTR-LAB-035; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Initialize Top-Down 2D controller | Validate configuration, stable IDs, required references, and supported preset boundaries for: Initialize Top-Down 2D controller. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-206 | ECTR-LAB-035; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Initialize Top-Down 2D controller | Exercise pure policy and state calculations for: Initialize Top-Down 2D controller. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-207 | ECTR-LAB-035; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Initialize Top-Down 2D controller | Run the normal runtime workflow for: Initialize Top-Down 2D controller. | The host, motor, facing policy, scripted intent driver, and readout become Ready. | Not run |
| ECTR-T-208 | ECTR-LAB-035; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Initialize Top-Down 2D controller | Inject the closest approved invalid, stale, missing, or interrupted condition for: Initialize Top-Down 2D controller. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-209 | ECTR-LAB-035; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Initialize Top-Down 2D controller | Repeat, disable/enable, reset, or reload around: Initialize Top-Down 2D controller. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-210 | ECTR-LAB-035; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Initialize Top-Down 2D controller | Follow the documented Laboratory action for: Initialize Top-Down 2D controller. | The host, motor, facing policy, scripted intent driver, and readout become Ready. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-211 | ECTR-LAB-036; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Missing Top-Down configuration | Validate configuration, stable IDs, required references, and supported preset boundaries for: Missing Top-Down configuration. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-212 | ECTR-LAB-036; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Missing Top-Down configuration | Exercise pure policy and state calculations for: Missing Top-Down configuration. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-213 | ECTR-LAB-036; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing Top-Down configuration | Run the normal runtime workflow for: Missing Top-Down configuration. | Initialization is blocked with an actionable ECTR diagnostic and no motion side effects. | Not run |
| ECTR-T-214 | ECTR-LAB-036; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing Top-Down configuration | Inject the closest approved invalid, stale, missing, or interrupted condition for: Missing Top-Down configuration. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-215 | ECTR-LAB-036; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing Top-Down configuration | Repeat, disable/enable, reset, or reload around: Missing Top-Down configuration. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-216 | ECTR-LAB-036; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Missing Top-Down configuration | Follow the documented Laboratory action for: Missing Top-Down configuration. | Initialization is blocked with an actionable ECTR diagnostic and no motion side effects. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-217 | ECTR-LAB-037; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Missing top-down Rigidbody2D | Validate configuration, stable IDs, required references, and supported preset boundaries for: Missing top-down Rigidbody2D. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-218 | ECTR-LAB-037; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Missing top-down Rigidbody2D | Exercise pure policy and state calculations for: Missing top-down Rigidbody2D. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-219 | ECTR-LAB-037; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing top-down Rigidbody2D | Run the normal runtime workflow for: Missing top-down Rigidbody2D. | Validation blocks movement and identifies the required component. | Not run |
| ECTR-T-220 | ECTR-LAB-037; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing top-down Rigidbody2D | Inject the closest approved invalid, stale, missing, or interrupted condition for: Missing top-down Rigidbody2D. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-221 | ECTR-LAB-037; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing top-down Rigidbody2D | Repeat, disable/enable, reset, or reload around: Missing top-down Rigidbody2D. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-222 | ECTR-LAB-037; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Missing top-down Rigidbody2D | Follow the documented Laboratory action for: Missing top-down Rigidbody2D. | Validation blocks movement and identifies the required component. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-223 | ECTR-LAB-038; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Missing top-down Collider2D | Validate configuration, stable IDs, required references, and supported preset boundaries for: Missing top-down Collider2D. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-224 | ECTR-LAB-038; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Missing top-down Collider2D | Exercise pure policy and state calculations for: Missing top-down Collider2D. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-225 | ECTR-LAB-038; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing top-down Collider2D | Run the normal runtime workflow for: Missing top-down Collider2D. | Validation blocks the controller without silently generating project content. | Not run |
| ECTR-T-226 | ECTR-LAB-038; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing top-down Collider2D | Inject the closest approved invalid, stale, missing, or interrupted condition for: Missing top-down Collider2D. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-227 | ECTR-LAB-038; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Missing top-down Collider2D | Repeat, disable/enable, reset, or reload around: Missing top-down Collider2D. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-228 | ECTR-LAB-038; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Missing top-down Collider2D | Follow the documented Laboratory action for: Missing top-down Collider2D. | Validation blocks the controller without silently generating project content. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-229 | ECTR-LAB-039; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Unexpected gravity scale | Validate configuration, stable IDs, required references, and supported preset boundaries for: Unexpected gravity scale. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-230 | ECTR-LAB-039; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Unexpected gravity scale | Exercise pure policy and state calculations for: Unexpected gravity scale. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-231 | ECTR-LAB-039; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Unexpected gravity scale | Run the normal runtime workflow for: Unexpected gravity scale. | Validation warns or blocks according to the configuration safety policy. | Not run |
| ECTR-T-232 | ECTR-LAB-039; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Unexpected gravity scale | Inject the closest approved invalid, stale, missing, or interrupted condition for: Unexpected gravity scale. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-233 | ECTR-LAB-039; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Unexpected gravity scale | Repeat, disable/enable, reset, or reload around: Unexpected gravity scale. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-234 | ECTR-LAB-039; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Unexpected gravity scale | Follow the documented Laboratory action for: Unexpected gravity scale. | Validation warns or blocks according to the configuration safety policy. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-235 | ECTR-LAB-040; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Invalid top-down body type | Validate configuration, stable IDs, required references, and supported preset boundaries for: Invalid top-down body type. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-236 | ECTR-LAB-040; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Invalid top-down body type | Exercise pure policy and state calculations for: Invalid top-down body type. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-237 | ECTR-LAB-040; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Invalid top-down body type | Run the normal runtime workflow for: Invalid top-down body type. | The motor refuses to execute and reports the supported MVP boundary. | Not run |
| ECTR-T-238 | ECTR-LAB-040; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Invalid top-down body type | Inject the closest approved invalid, stale, missing, or interrupted condition for: Invalid top-down body type. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-239 | ECTR-LAB-040; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Invalid top-down body type | Repeat, disable/enable, reset, or reload around: Invalid top-down body type. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-240 | ECTR-LAB-040; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Invalid top-down body type | Follow the documented Laboratory action for: Invalid top-down body type. | The motor refuses to execute and reports the supported MVP boundary. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-241 | ECTR-LAB-041; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Duplicate Top-Down motor | Validate configuration, stable IDs, required references, and supported preset boundaries for: Duplicate Top-Down motor. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-242 | ECTR-LAB-041; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Duplicate Top-Down motor | Exercise pure policy and state calculations for: Duplicate Top-Down motor. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-243 | ECTR-LAB-041; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Duplicate Top-Down motor | Run the normal runtime workflow for: Duplicate Top-Down motor. | Ambiguous motor ownership is rejected before physics writes. | Not run |
| ECTR-T-244 | ECTR-LAB-041; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Duplicate Top-Down motor | Inject the closest approved invalid, stale, missing, or interrupted condition for: Duplicate Top-Down motor. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-245 | ECTR-LAB-041; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Duplicate Top-Down motor | Repeat, disable/enable, reset, or reload around: Duplicate Top-Down motor. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-246 | ECTR-LAB-041; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Duplicate Top-Down motor | Follow the documented Laboratory action for: Duplicate Top-Down motor. | Ambiguous motor ownership is rejected before physics writes. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-247 | ECTR-LAB-042; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down control lease | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down control lease. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-248 | ECTR-LAB-042; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down control lease | Exercise pure policy and state calculations for: Top-down control lease. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-249 | ECTR-LAB-042; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down control lease | Run the normal runtime workflow for: Top-down control lease. | Only the current control generation may feed movement. | Not run |
| ECTR-T-250 | ECTR-LAB-042; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down control lease | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down control lease. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-251 | ECTR-LAB-042; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down control lease | Repeat, disable/enable, reset, or reload around: Top-down control lease. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-252 | ECTR-LAB-042; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down control lease | Follow the documented Laboratory action for: Top-down control lease. | Only the current control generation may feed movement. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-253 | ECTR-LAB-043; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down stale lease | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down stale lease. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-254 | ECTR-LAB-043; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down stale lease | Exercise pure policy and state calculations for: Top-down stale lease. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-255 | ECTR-LAB-043; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down stale lease | Run the normal runtime workflow for: Top-down stale lease. | The stale lease cannot revoke the new control generation. | Not run |
| ECTR-T-256 | ECTR-LAB-043; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down stale lease | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down stale lease. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-257 | ECTR-LAB-043; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down stale lease | Repeat, disable/enable, reset, or reload around: Top-down stale lease. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-258 | ECTR-LAB-043; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down stale lease | Follow the documented Laboratory action for: Top-down stale lease. | The stale lease cannot revoke the new control generation. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-259 | ECTR-LAB-044; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down stale intent | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down stale intent. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-260 | ECTR-LAB-044; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down stale intent | Exercise pure policy and state calculations for: Top-down stale intent. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-261 | ECTR-LAB-044; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down stale intent | Run the normal runtime workflow for: Top-down stale intent. | Older packets are ignored without reversing facing or velocity. | Not run |
| ECTR-T-262 | ECTR-LAB-044; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down stale intent | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down stale intent. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-263 | ECTR-LAB-044; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down stale intent | Repeat, disable/enable, reset, or reload around: Top-down stale intent. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-264 | ECTR-LAB-044; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down stale intent | Follow the documented Laboratory action for: Top-down stale intent. | Older packets are ignored without reversing facing or velocity. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-265 | ECTR-LAB-045; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down idle | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down idle. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-266 | ECTR-LAB-045; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down idle | Exercise pure policy and state calculations for: Top-down idle. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-267 | ECTR-LAB-045; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down idle | Run the normal runtime workflow for: Top-down idle. | Velocity settles to zero and the state reports Idle. | Not run |
| ECTR-T-268 | ECTR-LAB-045; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down idle | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down idle. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-269 | ECTR-LAB-045; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down idle | Repeat, disable/enable, reset, or reload around: Top-down idle. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-270 | ECTR-LAB-045; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down idle | Follow the documented Laboratory action for: Top-down idle. | Velocity settles to zero and the state reports Idle. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-271 | ECTR-LAB-046; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Cardinal movement | Validate configuration, stable IDs, required references, and supported preset boundaries for: Cardinal movement. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-272 | ECTR-LAB-046; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Cardinal movement | Exercise pure policy and state calculations for: Cardinal movement. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-273 | ECTR-LAB-046; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Cardinal movement | Run the normal runtime workflow for: Cardinal movement. | Motion follows the normalized intent and configured acceleration policy. | Not run |
| ECTR-T-274 | ECTR-LAB-046; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Cardinal movement | Inject the closest approved invalid, stale, missing, or interrupted condition for: Cardinal movement. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-275 | ECTR-LAB-046; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Cardinal movement | Repeat, disable/enable, reset, or reload around: Cardinal movement. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-276 | ECTR-LAB-046; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Cardinal movement | Follow the documented Laboratory action for: Cardinal movement. | Motion follows the normalized intent and configured acceleration policy. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-277 | ECTR-LAB-047; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Normalized diagonal movement | Validate configuration, stable IDs, required references, and supported preset boundaries for: Normalized diagonal movement. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-278 | ECTR-LAB-047; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Normalized diagonal movement | Exercise pure policy and state calculations for: Normalized diagonal movement. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-279 | ECTR-LAB-047; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Normalized diagonal movement | Run the normal runtime workflow for: Normalized diagonal movement. | Resulting speed does not exceed cardinal maximum speed. | Not run |
| ECTR-T-280 | ECTR-LAB-047; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Normalized diagonal movement | Inject the closest approved invalid, stale, missing, or interrupted condition for: Normalized diagonal movement. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-281 | ECTR-LAB-047; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Normalized diagonal movement | Repeat, disable/enable, reset, or reload around: Normalized diagonal movement. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-282 | ECTR-LAB-047; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Normalized diagonal movement | Follow the documented Laboratory action for: Normalized diagonal movement. | Resulting speed does not exceed cardinal maximum speed. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-283 | ECTR-LAB-048; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Preserve diagonal magnitude | Validate configuration, stable IDs, required references, and supported preset boundaries for: Preserve diagonal magnitude. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-284 | ECTR-LAB-048; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Preserve diagonal magnitude | Exercise pure policy and state calculations for: Preserve diagonal magnitude. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-285 | ECTR-LAB-048; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Preserve diagonal magnitude | Run the normal runtime workflow for: Preserve diagonal magnitude. | The selected policy is applied explicitly and reported in diagnostics. | Not run |
| ECTR-T-286 | ECTR-LAB-048; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Preserve diagonal magnitude | Inject the closest approved invalid, stale, missing, or interrupted condition for: Preserve diagonal magnitude. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-287 | ECTR-LAB-048; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Preserve diagonal magnitude | Repeat, disable/enable, reset, or reload around: Preserve diagonal magnitude. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-288 | ECTR-LAB-048; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Preserve diagonal magnitude | Follow the documented Laboratory action for: Preserve diagonal magnitude. | The selected policy is applied explicitly and reported in diagnostics. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-289 | ECTR-LAB-049; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down acceleration | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down acceleration. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-290 | ECTR-LAB-049; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down acceleration | Exercise pure policy and state calculations for: Top-down acceleration. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-291 | ECTR-LAB-049; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down acceleration | Run the normal runtime workflow for: Top-down acceleration. | Velocity approaches the configured target without overshoot beyond tolerance. | Not run |
| ECTR-T-292 | ECTR-LAB-049; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down acceleration | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down acceleration. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-293 | ECTR-LAB-049; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down acceleration | Repeat, disable/enable, reset, or reload around: Top-down acceleration. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-294 | ECTR-LAB-049; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down acceleration | Follow the documented Laboratory action for: Top-down acceleration. | Velocity approaches the configured target without overshoot beyond tolerance. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-295 | ECTR-LAB-050; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down deceleration | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down deceleration. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-296 | ECTR-LAB-050; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down deceleration | Exercise pure policy and state calculations for: Top-down deceleration. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-297 | ECTR-LAB-050; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down deceleration | Run the normal runtime workflow for: Top-down deceleration. | Velocity approaches zero under the configured deceleration policy. | Not run |
| ECTR-T-298 | ECTR-LAB-050; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down deceleration | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down deceleration. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-299 | ECTR-LAB-050; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down deceleration | Repeat, disable/enable, reset, or reload around: Top-down deceleration. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-300 | ECTR-LAB-050; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down deceleration | Follow the documented Laboratory action for: Top-down deceleration. | Velocity approaches zero under the configured deceleration policy. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-301 | ECTR-LAB-051; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down speed clamp | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down speed clamp. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-302 | ECTR-LAB-051; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down speed clamp | Exercise pure policy and state calculations for: Top-down speed clamp. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-303 | ECTR-LAB-051; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down speed clamp | Run the normal runtime workflow for: Top-down speed clamp. | Motor-owned speed remains within the configured maximum. | Not run |
| ECTR-T-304 | ECTR-LAB-051; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down speed clamp | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down speed clamp. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-305 | ECTR-LAB-051; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down speed clamp | Repeat, disable/enable, reset, or reload around: Top-down speed clamp. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-306 | ECTR-LAB-051; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down speed clamp | Follow the documented Laboratory action for: Top-down speed clamp. | Motor-owned speed remains within the configured maximum. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-307 | ECTR-LAB-052; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Four-direction facing | Validate configuration, stable IDs, required references, and supported preset boundaries for: Four-direction facing. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-308 | ECTR-LAB-052; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Four-direction facing | Exercise pure policy and state calculations for: Four-direction facing. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-309 | ECTR-LAB-052; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Four-direction facing | Run the normal runtime workflow for: Four-direction facing. | Facing resolves deterministically to one cardinal direction. | Not run |
| ECTR-T-310 | ECTR-LAB-052; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Four-direction facing | Inject the closest approved invalid, stale, missing, or interrupted condition for: Four-direction facing. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-311 | ECTR-LAB-052; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Four-direction facing | Repeat, disable/enable, reset, or reload around: Four-direction facing. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-312 | ECTR-LAB-052; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Four-direction facing | Follow the documented Laboratory action for: Four-direction facing. | Facing resolves deterministically to one cardinal direction. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-313 | ECTR-LAB-053; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Eight-direction facing | Validate configuration, stable IDs, required references, and supported preset boundaries for: Eight-direction facing. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-314 | ECTR-LAB-053; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Eight-direction facing | Exercise pure policy and state calculations for: Eight-direction facing. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-315 | ECTR-LAB-053; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Eight-direction facing | Run the normal runtime workflow for: Eight-direction facing. | Facing resolves to the expected octant without jitter around thresholds. | Not run |
| ECTR-T-316 | ECTR-LAB-053; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Eight-direction facing | Inject the closest approved invalid, stale, missing, or interrupted condition for: Eight-direction facing. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-317 | ECTR-LAB-053; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Eight-direction facing | Repeat, disable/enable, reset, or reload around: Eight-direction facing. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-318 | ECTR-LAB-053; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Eight-direction facing | Follow the documented Laboratory action for: Eight-direction facing. | Facing resolves to the expected octant without jitter around thresholds. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-319 | ECTR-LAB-054; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Independent look facing | Validate configuration, stable IDs, required references, and supported preset boundaries for: Independent look facing. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-320 | ECTR-LAB-054; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Independent look facing | Exercise pure policy and state calculations for: Independent look facing. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-321 | ECTR-LAB-054; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Independent look facing | Run the normal runtime workflow for: Independent look facing. | Facing follows the configured look source while movement remains independent. | Not run |
| ECTR-T-322 | ECTR-LAB-054; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Independent look facing | Inject the closest approved invalid, stale, missing, or interrupted condition for: Independent look facing. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-323 | ECTR-LAB-054; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Independent look facing | Repeat, disable/enable, reset, or reload around: Independent look facing. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-324 | ECTR-LAB-054; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Independent look facing | Follow the documented Laboratory action for: Independent look facing. | Facing follows the configured look source while movement remains independent. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-325 | ECTR-LAB-055; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Preserve last facing | Validate configuration, stable IDs, required references, and supported preset boundaries for: Preserve last facing. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-326 | ECTR-LAB-055; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Preserve last facing | Exercise pure policy and state calculations for: Preserve last facing. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-327 | ECTR-LAB-055; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Preserve last facing | Run the normal runtime workflow for: Preserve last facing. | The last valid facing remains available for animation and interaction seams. | Not run |
| ECTR-T-328 | ECTR-LAB-055; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Preserve last facing | Inject the closest approved invalid, stale, missing, or interrupted condition for: Preserve last facing. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-329 | ECTR-LAB-055; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Preserve last facing | Repeat, disable/enable, reset, or reload around: Preserve last facing. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-330 | ECTR-LAB-055; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Preserve last facing | Follow the documented Laboratory action for: Preserve last facing. | The last valid facing remains available for animation and interaction seams. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-331 | ECTR-LAB-056; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Collision slide | Validate configuration, stable IDs, required references, and supported preset boundaries for: Collision slide. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-332 | ECTR-LAB-056; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Collision slide | Exercise pure policy and state calculations for: Collision slide. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-333 | ECTR-LAB-056; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Collision slide | Run the normal runtime workflow for: Collision slide. | Rigidbody2D collision response and motor policy produce stable tangent movement without transform tunneling. | Not run |
| ECTR-T-334 | ECTR-LAB-056; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Collision slide | Inject the closest approved invalid, stale, missing, or interrupted condition for: Collision slide. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-335 | ECTR-LAB-056; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Collision slide | Repeat, disable/enable, reset, or reload around: Collision slide. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-336 | ECTR-LAB-056; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Collision slide | Follow the documented Laboratory action for: Collision slide. | Rigidbody2D collision response and motor policy produce stable tangent movement without transform tunneling. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-337 | ECTR-LAB-057; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down movement constraint | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down movement constraint. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-338 | ECTR-LAB-057; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down movement constraint | Exercise pure policy and state calculations for: Top-down movement constraint. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-339 | ECTR-LAB-057; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down movement constraint | Run the normal runtime workflow for: Top-down movement constraint. | The actor remains inside the constraint and state remains coherent. | Not run |
| ECTR-T-340 | ECTR-LAB-057; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down movement constraint | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down movement constraint. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-341 | ECTR-LAB-057; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down movement constraint | Repeat, disable/enable, reset, or reload around: Top-down movement constraint. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-342 | ECTR-LAB-057; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down movement constraint | Follow the documented Laboratory action for: Top-down movement constraint. | The actor remains inside the constraint and state remains coherent. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-343 | ECTR-LAB-058; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down external velocity | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down external velocity. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-344 | ECTR-LAB-058; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down external velocity | Exercise pure policy and state calculations for: Top-down external velocity. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-345 | ECTR-LAB-058; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down external velocity | Run the normal runtime workflow for: Top-down external velocity. | The motor combines or replaces velocity according to the explicit request policy. | Not run |
| ECTR-T-346 | ECTR-LAB-058; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down external velocity | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down external velocity. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-347 | ECTR-LAB-058; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down external velocity | Repeat, disable/enable, reset, or reload around: Top-down external velocity. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-348 | ECTR-LAB-058; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down external velocity | Follow the documented Laboratory action for: Top-down external velocity. | The motor combines or replaces velocity according to the explicit request policy. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-349 | ECTR-LAB-059; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down warp | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down warp. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-350 | ECTR-LAB-059; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down warp | Exercise pure policy and state calculations for: Top-down warp. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-351 | ECTR-LAB-059; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down warp | Run the normal runtime workflow for: Top-down warp. | Position, velocity reset policy, facing policy, and warp revision commit atomically. | Not run |
| ECTR-T-352 | ECTR-LAB-059; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down warp | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down warp. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-353 | ECTR-LAB-059; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down warp | Repeat, disable/enable, reset, or reload around: Top-down warp. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-354 | ECTR-LAB-059; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down warp | Follow the documented Laboratory action for: Top-down warp. | Position, velocity reset policy, facing policy, and warp revision commit atomically. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-355 | ECTR-LAB-060; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down pause and resume | Validate configuration, stable IDs, required references, and supported preset boundaries for: Top-down pause and resume. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-356 | ECTR-LAB-060; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Top-down pause and resume | Exercise pure policy and state calculations for: Top-down pause and resume. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-357 | ECTR-LAB-060; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down pause and resume | Run the normal runtime workflow for: Top-down pause and resume. | Buffered stale movement does not fire after resume. | Not run |
| ECTR-T-358 | ECTR-LAB-060; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down pause and resume | Inject the closest approved invalid, stale, missing, or interrupted condition for: Top-down pause and resume. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-359 | ECTR-LAB-060; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Top-down pause and resume | Repeat, disable/enable, reset, or reload around: Top-down pause and resume. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-360 | ECTR-LAB-060; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Top-down pause and resume | Follow the documented Laboratory action for: Top-down pause and resume. | Buffered stale movement does not fire after resume. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-361 | ECTR-LAB-061; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Disable and re-enable host | Validate configuration, stable IDs, required references, and supported preset boundaries for: Disable and re-enable host. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-362 | ECTR-LAB-061; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Disable and re-enable host | Exercise pure policy and state calculations for: Disable and re-enable host. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-363 | ECTR-LAB-061; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Disable and re-enable host | Run the normal runtime workflow for: Disable and re-enable host. | Subscriptions, registrations, and state reset according to documented lifecycle. | Not run |
| ECTR-T-364 | ECTR-LAB-061; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Disable and re-enable host | Inject the closest approved invalid, stale, missing, or interrupted condition for: Disable and re-enable host. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-365 | ECTR-LAB-061; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Disable and re-enable host | Repeat, disable/enable, reset, or reload around: Disable and re-enable host. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-366 | ECTR-LAB-061; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Disable and re-enable host | Follow the documented Laboratory action for: Disable and re-enable host. | Subscriptions, registrations, and state reset according to documented lifecycle. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-367 | ECTR-LAB-062; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Multiple independent actors | Validate configuration, stable IDs, required references, and supported preset boundaries for: Multiple independent actors. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-368 | ECTR-LAB-062; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Multiple independent actors | Exercise pure policy and state calculations for: Multiple independent actors. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-369 | ECTR-LAB-062; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Multiple independent actors | Run the normal runtime workflow for: Multiple independent actors. | Each actor owns only its local state and no global singleton collision occurs. | Not run |
| ECTR-T-370 | ECTR-LAB-062; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Multiple independent actors | Inject the closest approved invalid, stale, missing, or interrupted condition for: Multiple independent actors. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-371 | ECTR-LAB-062; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Multiple independent actors | Repeat, disable/enable, reset, or reload around: Multiple independent actors. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-372 | ECTR-LAB-062; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Multiple independent actors | Follow the documented Laboratory action for: Multiple independent actors. | Each actor owns only its local state and no global singleton collision occurs. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-373 | ECTR-LAB-063; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Scripted square path | Validate configuration, stable IDs, required references, and supported preset boundaries for: Scripted square path. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-374 | ECTR-LAB-063; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Scripted square path | Exercise pure policy and state calculations for: Scripted square path. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-375 | ECTR-LAB-063; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Scripted square path | Run the normal runtime workflow for: Scripted square path. | The actor completes the expected bounded route and publishes state transitions. | Not run |
| ECTR-T-376 | ECTR-LAB-063; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Scripted square path | Inject the closest approved invalid, stale, missing, or interrupted condition for: Scripted square path. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-377 | ECTR-LAB-063; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Scripted square path | Repeat, disable/enable, reset, or reload around: Scripted square path. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-378 | ECTR-LAB-063; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Scripted square path | Follow the documented Laboratory action for: Scripted square path. | The actor completes the expected bounded route and publishes state transitions. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-379 | ECTR-LAB-064; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Reset Top-Down Lab | Validate configuration, stable IDs, required references, and supported preset boundaries for: Reset Top-Down Lab. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-380 | ECTR-LAB-064; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Reset Top-Down Lab | Exercise pure policy and state calculations for: Reset Top-Down Lab. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-381 | ECTR-LAB-064; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Reset Top-Down Lab | Run the normal runtime workflow for: Reset Top-Down Lab. | Configuration assets remain immutable and runtime state returns to baseline. | Not run |
| ECTR-T-382 | ECTR-LAB-064; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Reset Top-Down Lab | Inject the closest approved invalid, stale, missing, or interrupted condition for: Reset Top-Down Lab. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-383 | ECTR-LAB-064; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Reset Top-Down Lab | Repeat, disable/enable, reset, or reload around: Reset Top-Down Lab. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-384 | ECTR-LAB-064; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Reset Top-Down Lab | Follow the documented Laboratory action for: Reset Top-Down Lab. | Configuration assets remain immutable and runtime state returns to baseline. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-385 | ECTR-LAB-065; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Reload Top-Down scene | Validate configuration, stable IDs, required references, and supported preset boundaries for: Reload Top-Down scene. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-386 | ECTR-LAB-065; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Reload Top-Down scene | Exercise pure policy and state calculations for: Reload Top-Down scene. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-387 | ECTR-LAB-065; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Reload Top-Down scene | Run the normal runtime workflow for: Reload Top-Down scene. | No stale source, control, or event registration survives the actor lifecycle. | Not run |
| ECTR-T-388 | ECTR-LAB-065; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Reload Top-Down scene | Inject the closest approved invalid, stale, missing, or interrupted condition for: Reload Top-Down scene. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-389 | ECTR-LAB-065; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Reload Top-Down scene | Repeat, disable/enable, reset, or reload around: Reload Top-Down scene. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-390 | ECTR-LAB-065; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Reload Top-Down scene | Follow the documented Laboratory action for: Reload Top-Down scene. | No stale source, control, or event registration survives the actor lifecycle. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-391 | ECTR-LAB-066; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for No animation presenter | Validate configuration, stable IDs, required references, and supported preset boundaries for: No animation presenter. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-392 | ECTR-LAB-066; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for No animation presenter | Exercise pure policy and state calculations for: No animation presenter. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-393 | ECTR-LAB-066; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for No animation presenter | Run the normal runtime workflow for: No animation presenter. | The controller continues to move and report semantic state. | Not run |
| ECTR-T-394 | ECTR-LAB-066; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for No animation presenter | Inject the closest approved invalid, stale, missing, or interrupted condition for: No animation presenter. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-395 | ECTR-LAB-066; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for No animation presenter | Repeat, disable/enable, reset, or reload around: No animation presenter. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-396 | ECTR-LAB-066; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for No animation presenter | Follow the documented Laboratory action for: No animation presenter. | The controller continues to move and report semantic state. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-397 | ECTR-LAB-067; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for No peer packages installed | Validate configuration, stable IDs, required references, and supported preset boundaries for: No peer packages installed. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-398 | ECTR-LAB-067; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for No peer packages installed | Exercise pure policy and state calculations for: No peer packages installed. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-399 | ECTR-LAB-067; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for No peer packages installed | Run the normal runtime workflow for: No peer packages installed. | The core preset remains fully usable and diagnosable. | Not run |
| ECTR-T-400 | ECTR-LAB-067; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for No peer packages installed | Inject the closest approved invalid, stale, missing, or interrupted condition for: No peer packages installed. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-401 | ECTR-LAB-067; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for No peer packages installed | Repeat, disable/enable, reset, or reload around: No peer packages installed. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-402 | ECTR-LAB-067; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for No peer packages installed | Follow the documented Laboratory action for: No peer packages installed. | The core preset remains fully usable and diagnosable. Evidence is captured separately; status remains Not run until execution. | Not run |
| ECTR-T-403 | ECTR-LAB-068; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Inspect top-down diagnostics | Validate configuration, stable IDs, required references, and supported preset boundaries for: Inspect top-down diagnostics. | The validator returns the documented severity and never mutates project-owned assets silently. | Not run |
| ECTR-T-404 | ECTR-LAB-068; controller independence/lifecycle contract | EditMode | Top-Down 2D fixture for Inspect top-down diagnostics | Exercise pure policy and state calculations for: Inspect top-down diagnostics. | The result is deterministic for the supplied inputs and matches the approved contract. | Not run |
| ECTR-T-405 | ECTR-LAB-068; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Inspect top-down diagnostics | Run the normal runtime workflow for: Inspect top-down diagnostics. | The readout explains effective source, control state, intent age, velocity, facing, constraints, and health. | Not run |
| ECTR-T-406 | ECTR-LAB-068; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Inspect top-down diagnostics | Inject the closest approved invalid, stale, missing, or interrupted condition for: Inspect top-down diagnostics. | The controller fails safely, preserves authoritative state, and emits an actionable ECTR result. | Not run |
| ECTR-T-407 | ECTR-LAB-068; controller independence/lifecycle contract | PlayMode | Top-Down 2D fixture for Inspect top-down diagnostics | Repeat, disable/enable, reset, or reload around: Inspect top-down diagnostics. | No stale registrations, intent edges, control generations, events, or static state survive improperly. | Not run |
| ECTR-T-408 | ECTR-LAB-068; controller independence/lifecycle contract | Manual Laboratory | Top-Down 2D fixture for Inspect top-down diagnostics | Follow the documented Laboratory action for: Inspect top-down diagnostics. | The readout explains effective source, control state, intent age, velocity, facing, constraints, and health. Evidence is captured separately; status remains Not run until execution. | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and non-goals approved.
- [x] Rootless actor authority approved.
- [x] MVP presets and deferred families separated.
- [x] Physics assumptions and intent/control seams defined.
- [x] Independent Laboratories designed.
- [x] Test registry approved with all evidence Not run.

### 24.2 Implementation gate

- [ ] Core/preset assemblies compile with declared dependencies only.
- [ ] No runtime UnityEditor or peer references.
- [ ] Configuration remains immutable during play.
- [ ] Stale handles/packets cannot alter current state.
- [ ] Setup/repair repeats safely.
- [ ] Public API matches specification or authority updated first.

### 24.3 Standalone gate

- [ ] Clean-project installation passes.
- [ ] Both preset Labs pass independently.
- [ ] Samples remove safely.
- [ ] Multiple actors run without global authority conflict.
- [ ] Missing optional adapters/bridges leave standalone path intact.

### 24.4 Quality gate

- [ ] Required automated/manual tests pass.
- [ ] No Blocker/Critical defect remains.
- [ ] Hot-path allocation and performance targets pass.
- [ ] Diagnostics are actionable and bounded.
- [ ] Documentation matches artifact.
- [ ] Current Notes reconciled.
- [ ] Licenses/notices complete.

### 24.5 Distribution gate

- [ ] Manifest/asmdefs valid.
- [ ] Version/changelog updated.
- [ ] Stable `.meta` files included.
- [ ] Git/local/tarball routes tested.
- [ ] Beta, release-candidate, and stable evidence gates pass under SFGSS-004.
- [ ] Compatibility catalog updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Rescuers2D | Role-specific Rigidbody2D controllers and shared input reader | Introduce Side-View base on one role; keep role actions/project capabilities outside; migrate incrementally | Walk/jump/facing and required role movement parity in isolated and project tests | Retain original controllers/prefabs/branch until parity |
| Hackulos | Planned top-down controller | Prove Top-Down Lab, then use project/Input adapter and character bridge later | Four/eight direction movement, facing, collisions, animation events | Keep project prototype controller until parity |
| Echo Systems Lab | Project-specific movement/controller experiments | Adopt only where case study benefits | No regression in existing scene | Separate branch/prefab |

### 25.2 Preserve-until-parity rule

Existing controllers remain intact until the package passes its standalone Lab and one feature category at a time in the target project. Input, combat, animation, camera, and role-specific actions migrate independently. Removal happens only after reversible parity evidence.

### 25.3 Migration tooling

Future tools detect supported project components only through explicit adapters, preview proposed component/config creation, preserve prefab/scene backups where practical, never rewrite project code automatically, validate after conversion, and generate a rollback report.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| ECTR-R-001 | Universal-controller scope inflation | High | High | Typed preset families, capability boundaries, per-family Labs | Any unrelated feature request |
| ECTR-R-002 | Physics behavior differs by timestep/platform | Medium | High | Explicit backend contract and measured compatibility | Implementation testing |
| ECTR-R-003 | Input edges lost between Update/FixedUpdate | Medium | High | Sequence-aware bounded buffering and tests | Adapter implementation |
| ECTR-R-004 | Character/control authority duplicated | Medium | High | Fellowship bridge owns translation; actor-local lease only | Bridge design |
| ECTR-R-005 | Config assets mutated at runtime | Medium | High | Detached runtime state and validator | PlayMode tests |
| ECTR-R-006 | Capability ordering becomes hidden | Medium | Medium | Declared order/dependencies and validation | New capability |
| ECTR-R-007 | Sample input dependency leaks into core | Medium | High | Scripted drivers; separate Input System adapter | Packaging audit |
| ECTR-R-008 | Animator becomes hidden rule authority | Medium | Medium | Semantic state/events; optional presenter | Adoption review |
| ECTR-R-009 | External motion conflicts with motor | Medium | High | Explicit combine/replace policies and bounded queue | Combat/feedback integration |
| ECTR-R-010 | Controller package becomes oversized | Medium | Medium | Review package split after third distinct backend/family | Roadmap expansion |
| ECTR-R-011 | Network expectations exceed local motor | Medium | High | Explicit non-goal; Convergence research/adapter | Multiplayer design |
| ECTR-R-012 | Existing-project migration regresses role behavior | High | High | Preserve-until-parity and incremental project adapters | Rescuers2D adoption |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR? |
|---|---|---|---|---|---:|
| ECTR-D-001 | Controller authority is actor-bound; no persistent/global root | Approved | Movement truth is local and concurrent | Multiple actors/scenes work independently | No |
| ECTR-D-002 | MVP remains one modular UPM package with explicit preset assemblies | Approved | Shared contracts and release cadence are still coherent | Revisit after third backend/family | No |
| ECTR-D-003 | Intent payloads are family-specific behind shared source lifecycle contracts | Approved | Avoid universal action-struct bloat | Adapters target a declared family | No |
| ECTR-D-004 | Physics-backed MVP motors execute on a declared fixed-step path | Approved | Coherent Rigidbody2D writes | Sources require buffering/sequence semantics | No |
| ECTR-D-005 | MVP supports Dynamic Rigidbody2D only | Approved | Bounded honest scope | Kinematic/other backends require later design | No |
| ECTR-D-006 | Side-View 2D and Top-Down 2D are separate preset authorities/Labs | Approved | Different state and physics needs | No omnibus Lab proof |
| ECTR-D-007 | Deterministic scripted intent is the mandatory standalone Lab driver | Approved | No input-package dependency | Interactive adapter remains optional |
| ECTR-D-008 | AlwaysControlled and LeaseRequired are the two MVP control modes | Approved | Easy path plus possession-safe path | Bridge maps external owner truth |
| ECTR-D-009 | Live controller state is not persisted | Approved | Session/scene truth and stale handles | Project saves semantic pose/state |
| ECTR-D-010 | Animation, camera, audio, VFX, UI, and peer behavior consume semantics | Approved | Preserve authority separation | Optional bridges/presenters only |

### 27.2 Release-blocking questions

None remain for documentation approval. Exact implementation values, Input System adapter version, measured limits, and platform behavior remain evidence-pending and do not authorize unsupported claims.

### 27.3 Non-blocking later questions

- Whether the third distinct controller backend triggers separate UPM packages.
- Exact capability composition API after MVP motor prototypes.
- Kinematic Rigidbody2D and moving-platform support policy.
- Which crawl, climb, ladder, and swim modules graduate first.
- Network authority/prediction model after Convergence research.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included | Evidence |
|---|---|---|---|
| M0 | Approved specification | Design only | This document |
| M1 | Package skeleton | Manifest, assemblies, docs shell | Clean compile |
| M2 | Rootless core contracts | Host, source/control, snapshots, diagnostics | EditMode/PlayMode tests |
| M3 | Side-View 2D vertical slice | Motor, ground, jump, facing | Side-View Lab |
| M4 | Top-Down 2D vertical slice | Motor, diagonal/facing/constraints | Top-Down Lab |
| M5 | Tooling and validation | Setup, inspectors, monitor, repair | Repeatability tests |
| M6 | Adapters/first adoption | Input adapter, Fellowship bridge, project parity | Integration/project reports |
| M7 | Beta/release | Packaging, docs, full evidence | SFGSS-004 gates |

### 28.2 Checkpoint rule

Implementation remains locked until SUITE-DOC-33. When unlocked, each checkpoint follows SFGSS-005, shows complete code in conversation, explains each file and decision, provides exact Editor setup/tests, and stops at a proof boundary so Jesse can enter and understand the work.

### 28.3 First recommended checkpoint

`ECTR-M1-01 - The Vessel Package Skeleton`: manifest, core/Physics2D/preset/Editor/test asmdefs, documentation shell, and no runtime C# behavior beyond what the later approved checkpoint explicitly authorizes.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.
Treat SFGSS-000 and SFGSS-002 through SFGSS-005 as suite authorities.
Treat The Vessel (`EchoControllers`) Specification v1.0.0 as the Level 2 authority
for actor-bound controller hosts, normalized family intents, source/control leases,
physics-backed motor execution, Side-View 2D and Top-Down 2D presets, focused
capabilities, semantic state, tooling, diagnostics, Laboratories, and integration seams.

Current package: EchoControllers
Current specification: v1.0.0 Approved
Implementation status: locked until SUITE-DOC-33
Current documentation checkpoint: SUITE-DOC-17 - The Crucible (`EchoCrafting`)

Before changing this package:
1. Preserve rootless actor-local authority.
2. Keep character, input, camera, combat, animation, UI, save, scene, and network truth outside core.
3. Keep Side-View and Top-Down preset evidence independent.
4. Preserve immutable configurations and stale-safe source/control generations.
5. Keep all empirical evidence Not run until executed.
6. When coding is authorized, show complete files and explain every step for Jesse.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | 1.0.0 specification; no implementation version |
| Completed checkpoint | SUITE-DOC-16 specification approval |
| Files/assets created | Specification, audit report, roadmap/README/Current Notes updates |
| Tests passed | None; all planned evidence Not run |
| Tests failed | None executed |
| Known issues | Empirical performance/platform/adapter behavior pending |
| Decisions added | ECTR-D-001 through ECTR-D-010 |
| Next checkpoint | SUITE-DOC-17 - The Crucible (`EchoCrafting`) design workshop and specification |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and responsibility are clear.
- [x] Ownership and non-goals align with SFGSS-000.
- [x] Rootless independence proof is credible.
- [x] MVP is limited to two useful 2D presets.
- [x] Intent, control, motor, state, and physics boundaries are defined.
- [x] Setup and direct-scene workflows are understandable.
- [x] Each preset has an independent Laboratory.
- [x] Standalone diagnostics do not require Observatory.
- [x] Optional integrations are separated under SFGSS-002.
- [x] Data/identity/migration follow SFGSS-003.
- [x] Test/release evidence follows SFGSS-004.
- [x] No Isekai Studios identity or ownership is introduced.
- [x] Jesse has approved the specification for future implementation planning after the suite gate.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams  
**Date:** August 4, 2026  
**Conditions:** Package implementation remains locked until SUITE-DOC-33. Every empirical result remains Not run until executed. Future controller families and focused capabilities require their own scope, Laboratory, and evidence before support is advertised.

---

## Template Completion Rule

A new collaborator can identify The Vessel's exact authority, exclusions, MVP presets, standalone behavior, configuration/runtime separation, public seams, failure model, actor lifecycle, physics boundary, Laboratories, optional integrations, evidence state, and release gates without consulting an old chat. The specification is therefore complete and approved as a pre-code package foundation.


---

## Graph Navigation

#sfgss/package #sfgss/wave/expansion #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
