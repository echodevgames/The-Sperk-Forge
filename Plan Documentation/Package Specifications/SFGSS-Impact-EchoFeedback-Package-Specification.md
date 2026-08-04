# Impact - Coordinated Feedback Package Specification

**Working document ID:** SFGSS-PKG-ECHOFEEDBACK-001  
**Specification version:** 1.0.0  
**Status:** Approved  
**Technical package name:** EchoFeedback  
**Public title:** Impact - Coordinated Feedback  
**Package ID:** `com.echodevgames.echo-feedback`  
**Runtime namespace:** `EchoDevGames.EchoFeedback`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoFeedback`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, and SFGSS-004 v1.0.0  
**Last updated:** August 4, 2026

> “Let an action strike once and let every chosen sense answer together.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoFeedback. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-004 and all approved Foundation package authorities | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved semantic timeline recipes, explicit channel providers, unscaled coordination, transient time authority boundaries, channel scaling, cancellation, accessibility, diagnostics, tooling, Laboratories, bridge contracts, and release gates | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** Impact - Coordinated Feedback  
**Technical identifier:** EchoFeedback  
**Flavor line:** Make the moment land without letting presentation become the rule.  
**Plain-language subtitle:** Data-driven coordination of transient camera, timing, haptic, visual, UI, and audio feedback requests.

**One-sentence ownership contract:**

> EchoFeedback owns reusable feedback recipes, transient feedback-instance execution, channel scaling, scheduling, arbitration, cancellation, and provider coordination; it does not own damage, combat resolution, camera movement, audio playback, UI state, input-device assignment, global settings persistence, pause truth, or the gameplay events that request feedback.

### 1.1 Elevator summary

Impact turns one meaningful game event into a coordinated response across several optional feedback channels. A heavy hit may request camera impulse, a brief time pulse, controller rumble, a screen flash, UI punch, and an audio cue. The game reports only the semantic event and a runtime context. EchoFeedback selects and runs the configured recipe, applies project and accessibility scaling, coordinates timing, rejects unsafe or excessive work, and returns one cancellable handle.

The package remains provider-neutral. Its runtime core does not move a camera, play a Jukebot cue, animate an EchoUI screen, choose a controller, or permanently own `Time.timeScale`. Those actions belong to explicit providers or bridge packages. The core schedules semantic channel requests and guarantees that their lifecycle, cancellation, priority, diagnostics, and failure behavior are coherent.

The MVP supports a flat timeline recipe. Steps may begin sequentially or in parallel by using start offsets. This provides chained responses without introducing a general visual scripting graph, branching language, gameplay condition system, or hidden coroutine maze.

### 1.2 Why this belongs in The Sperk’s Forge

Feedback coordination is rebuilt in nearly every game. A hit sound is triggered in one class, camera shake in another, controller rumble in a third, UI flash in a fourth, and hit stop through a global time edit that no one remembers to restore. These effects begin as small flourishes and then grow into a knot of direct references, duplicated timing logic, inaccessible intensity values, and scene-specific assumptions.

The package is justified because the repeated problem is not any single effect. The repeated problem is coordinating several effects as one semantic response while preserving the authority of the systems that actually perform them.

| Source project or system | Existing need or failure pattern | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Character actions, destruction, explosions, rescues, denied interactions, and role switching need coordinated response | Event-driven gameplay and clear role feedback | Remove direct camera, audio, UI, and time coupling from gameplay scripts |
| Don’t Get Vince’d | Combo hits, boss phases, damage, pickups, invincibility, and victories need layered game feel | Semantic combat events and strong response | Keep combat resolution separate from presentation and add cancellation/accessibility |
| Echo Systems Lab | Weapons and mission events already separate authoritative state from listeners | Definition, runtime, event, presentation separation | Turn local listeners into reusable recipes and provider contracts |
| Jukebot | Audio cues need semantic requests rather than gameplay owning sources | Audio authority and structured requests | Use a bridge instead of embedding Jukebot types in recipes |
| The Pulse | Pause and high-level time policy need one authority | Base time and pause truth | Treat hit stop/time dilation as a transient multiplier requested through a time provider |
| The Accord | Reduced motion, flash, rumble, and shake preferences need durable storage | Global preference truth | EchoFeedback consumes effective scales without persisting them |
| The Looking Glass | Screen overlays and UI punch effects need a UI authority | UI hierarchy, focus, and presentation ownership | Request visual feedback through a bridge, never mutate UI internals directly |
| Future EchoCamera | Camera shake needs one camera authority | Camera target/mode/bounds ownership | Request impulses through an explicit provider rather than moving cameras in core |

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title and documentation | Yes | “Impact” may lead, followed by a clear technical subtitle |
| Setup guidance and tooltips | Yes | Flavor must never obscure channel, recipe, provider, or safety meaning |
| Standalone Laboratory | Optional | Verse-flavored labels must be replaceable sample content |
| Runtime API and serialized type names | No lore-only names | Use technical names such as `FeedbackRecipe`, `FeedbackHandle`, and `IFeedbackChannelProvider` |
| Project content | No required Verse data | Games own their events, signal IDs, visuals, audio cues, camera profiles, and terminology |

---

## 2. Problem Statement

### 2.1 Current problem

Game-feel effects are commonly authored as direct side effects inside gameplay code. That creates several recurring defects:

- one gameplay event knows about camera, audio, UI, input devices, and time;
- effects use inconsistent clocks, so hit stop prevents its own release or pauses leave rumble running;
- multiple hits stack without a declared policy and produce extreme shake, flash, or time scaling;
- stopping a character, scene, or ability does not reliably cancel its feedback;
- accessibility settings are applied unevenly or too late;
- controller rumble is sent to the wrong device or continues after focus/device loss;
- missing optional systems produce null-reference failures instead of structured degradation;
- project-specific effect assets leak into reusable package code;
- one combined “feedback manager” becomes a second camera, audio, UI, and game-state authority.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Foundation authority matrix | Camera, audio, UI, settings, input, diagnostics, and game state already have distinct owners | One authority per concern | EchoFeedback coordinates requests without bypassing those owners |
| SFGSS-002 | Optional package connections must be explicit and removable | Visible dependency direction | Separate bridge/provider packages, no hidden peer references |
| SFGSS-003 | Definitions and runtime state must remain separate | Immutable recipes and stable IDs | Keep active instances, clocks, handles, and provider state outside assets |
| SFGSS-004 | Planned tests are not executed evidence | Complete pre-code registry | Mark every laboratory and automated case `Not run` until implementation |
| Existing project feedback scripts | Local shake, time, UI, and audio listeners are easy to create but hard to coordinate | Small focused effects | One timeline, one lifecycle, one handle, bounded channels, clear diagnostics |

### 2.3 Consequences of doing nothing

- Each project invents a different feedback API and recipe format.
- Gameplay code accumulates presentation dependencies.
- Pauses, scene changes, destroyed targets, and application focus changes leave effects stuck.
- Strong effects become inaccessible or unsafe because there is no central scale and suppression boundary.
- Camera and audio systems are bypassed by convenience calls.
- Stress behavior remains undefined until a rapid-hit sequence turns the screen into a thunderstorm in a jar.
- Real-project migration becomes all-or-nothing instead of channel-by-channel.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one duplicate-safe application-session feedback authority when persistence is configured.
- Represent reusable responses as immutable, stable-ID feedback recipes.
- Coordinate semantic requests across time, camera, haptic, flash, UI, audio, and future custom channels.
- Keep channel execution behind explicitly registered providers.
- Use an unscaled clock for recipe scheduling, cancellation, release, and diagnostics.
- Support sequential and parallel effects through flat timeline offsets.
- Return structured play results and generational feedback handles.
- Support cancellation by handle, owner, group, channel, scene policy, and shutdown.
- Apply project safety caps, runtime channel scales, and accessibility suppression before provider execution.
- Define deterministic overlap, concurrency, priority, replacement, and rejection behavior.
- Support spatial and directional runtime context without storing scene-object references in definitions.
- Provide an opt-in standalone Unity time provider for projects without another time authority.
- Provide a Pulse bridge contract that preserves one final time-scale authority when The Pulse is installed.
- Define a separate Input System haptics provider artifact rather than making input-device ownership part of the core.
- Expose bounded diagnostics, snapshots, event history, and stable codes without requiring The Observatory.
- Provide safe setup, validation, recipe preview, provider simulation, stress controls, and one isolated Impact Laboratory.
- Remain useful with no other Sperk’s Forge package installed.

### 3.2 Non-goals

- EchoFeedback does not calculate damage, healing, stagger, victory, failure, or any gameplay result.
- It does not decide which semantic event occurred.
- It does not directly move production cameras or own camera modes, bounds, targets, or blends.
- It does not play production audio or own mixer routing.
- It does not own EchoUI screens, HUD state, focus, modal stacks, or production animation controllers.
- It does not own input actions, device pairing, rebinding, or player-to-device assignment.
- It does not persist accessibility preferences or game-save data.
- It does not become the pause authority or a second permanent time-scale authority.
- It does not provide a general tween engine, animation graph, Timeline replacement, VFX graph, post-processing framework, or cinematic sequencer.
- It does not provide branching, loops, gameplay conditions, random loot-like selection, or arbitrary code execution inside recipes.
- It does not guarantee haptic support on every controller or platform.
- It does not make Cinemachine, Input System, Jukebot, EchoUI, EchoCamera, EchoSettings, or EchoGameState mandatory core dependencies.
- It does not claim medical accessibility certification or that one default flash setting is safe for every player.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean project with one gameplay event | Create a recipe, use simulated or standalone providers, and see a coordinated response in the Laboratory |
| Gameplay programmer | Semantic event such as heavy hit or denied action | Request one recipe and receive a structured result and cancellable handle |
| Designer | Several effects need timing and tuning | Author a data-driven timeline without editing gameplay code |
| Accessibility integrator | Reduced motion, flash, rumble, or hit-stop preferences exist | Apply per-channel scales and suppression through an explicit bridge |
| Systems integrator | Camera, audio, UI, input, and game-state packages are installed | Add removable bridges while preserving each package’s authority |
| Tester | Effects stack, linger, or disappear | Inspect active recipes, steps, providers, scales, denials, cancellations, and bounded history |
| Maintainer | Package must ship independently | Validate clean install, removal, reinstall, migration, GUID stability, and Laboratory isolation |

### 3.4 Measurable success criteria

- The package installs in a clean supported Unity project with zero compile errors.
- Core runtime compiles and functions with no peer Sperk’s Forge package installed.
- A duplicate root cannot register providers, start recipes, edit time, subscribe, or create side effects.
- One semantic request can coordinate at least three simulated channels in the Standalone Laboratory.
- Missing optional providers follow each step’s declared requiredness without null-reference failures.
- Recipe scheduling and cancellation complete while scaled game time is zero.
- A stale handle cannot cancel a newer feedback instance that reused the same slot.
- Active recipe and step counts remain within configured hard limits.
- Project and accessibility scales clamp effect intensity before provider execution.
- Provider exceptions or cancellation failures do not leave the runner, time provider, or other channels permanently stuck.
- Removing a bridge or provider package leaves core EchoFeedback compilable and project-owned recipes preserved.
- ScriptableObject recipes and signal definitions remain unchanged after Play Mode and stress tests.
- Every planned test remains `Not run` until executed under SFGSS-004.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers.
- Gameplay, combat, UI, audio, camera, and systems programmers.
- Designers tuning game feel and accessibility-safe alternatives.
- QA testers reproducing overlap, cancellation, focus-loss, and scene-lifecycle defects.
- The Workshop when composing selected packages and setup plans.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| EFB-UC-001 | Initialize EchoFeedback | Root or setup | Valid configuration; no existing authority | Root claims, validates, registers built-in services, and becomes Ready | MVP |
| EFB-UC-002 | Reject duplicate | Scene or direct helper | Authority already exists | Duplicate exits before provider or effect side effects | MVP |
| EFB-UC-003 | Play recipe | Gameplay code | Root Ready; valid recipe | Structured result and live handle returned | MVP |
| EFB-UC-004 | Run parallel steps | Recipe runner | Multiple steps share start offsets | Providers begin in deterministic order within the same frame | MVP |
| EFB-UC-005 | Run chained steps | Recipe runner | Steps use increasing offsets | Steps begin according to unscaled timeline | MVP |
| EFB-UC-006 | Cancel by handle | Gameplay code | Live handle | Remaining work cancels according to policy and handle becomes terminal | MVP |
| EFB-UC-007 | Cancel by owner | Character or scene adapter | Several effects share owner token | Matching instances cancel; unrelated instances continue | MVP |
| EFB-UC-008 | Replace overlap group | Rapid gameplay event | Same group already active | Existing instance is replaced or request rejected by declared policy | MVP |
| EFB-UC-009 | Apply channel scales | Settings or project code | Root Ready | Effective intensity reflects project and accessibility scales | MVP |
| EFB-UC-010 | Execute hit stop standalone | Project without Pulse | Standalone time provider explicitly enabled | Transient multiplier applies and restores on unscaled time | MVP |
| EFB-UC-011 | Execute hit stop with Pulse | Pulse bridge installed | Pulse owns final time scale | Bridge combines feedback multiplier without a second time authority | Integration |
| EFB-UC-012 | Request camera impulse | EchoCamera or project provider registered | Spatial context available | Provider receives semantic signal and normalized parameters | Integration |
| EFB-UC-013 | Request audio response | Jukebot bridge registered | Signal mapping exists | Bridge requests Jukebot cue; EchoFeedback does not touch AudioSources | Integration |
| EFB-UC-014 | Request UI punch or flash | EchoUI or project provider registered | Target/presentation mapping exists | Provider executes within UI authority | Integration |
| EFB-UC-015 | Request controller rumble | Haptics provider registered | Resolved audience has supported device | Provider runs bounded rumble and resets safely | Provider |
| EFB-UC-016 | Missing provider | Recipe references unavailable channel | Step requiredness declared | Step skips, warns, or fails recipe exactly as configured | MVP |
| EFB-UC-017 | Provider throws or times out | Misbehaving provider | Recipe active | Failure is isolated, diagnosed, and cleanup continues | MVP |
| EFB-UC-018 | Lose application focus | Effect active | Focus policy configured | Haptics/time/visual providers follow safe focus-loss policy | MVP/Provider |
| EFB-UC-019 | Enter scene directly | Developer | Helper enabled; no authority | Minimal configured root initializes once | MVP |
| EFB-UC-020 | Stress rapid impacts | Tester | Laboratory imported | Limits, replacement, cancellation, scaling, and diagnostics remain bounded | MVP |
| EFB-UC-021 | Preview recipe | Designer | Editor preview tool open | Simulated providers display timeline without production dependencies | MVP |
| EFB-UC-022 | Export diagnostic snapshot | Tester | Root Ready | Bounded local status report created without project content leakage | MVP |

### 4.3 Explicitly unsupported use cases

- Using a recipe to calculate whether an attack hits.
- Storing arbitrary gameplay conditions or invoking arbitrary methods from assets.
- Treating feedback completion as proof that authoritative gameplay completed.
- Using EchoFeedback as a general camera controller, audio engine, UI animation system, input manager, save system, or pause manager.
- Persisting active recipes across application restarts.
- Assuming every device has dual-motor rumble.
- Networking feedback-instance state as authoritative gameplay state.
- Running unbounded nested recipes or recursive recipe references in the MVP.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Feedback recipe definitions and validation.
- Semantic channel and signal identifiers used by recipes.
- Runtime feedback requests, instances, handles, groups, priorities, and cancellation.
- Unscaled scheduling of recipe steps.
- Channel-provider registration and lifecycle contracts.
- Project safety caps and effective channel-scale composition.
- Overlap, concurrency, replacement, and rejection policy for feedback instances.
- Structured results, diagnostics, bounded history, and local status snapshots.
- The opt-in standalone feedback-time provider when explicitly selected in a project without another time authority.
- Editor setup, validation, preview simulation, and the Impact Laboratory.

### 5.2 The package does not own

- Damage, healing, combat, objectives, interactions, abilities, or event truth.
- Production camera transforms, modes, priorities, targets, bounds, or backend choice.
- Audio clips, playback, mixer buses, or persistent volume settings.
- UI screen state, navigation, focus, modals, or production view hierarchy.
- Input actions, active device detection, player pairing, or rebinding.
- Global preferences or save files.
- High-level game state, pause truth, or normal base time-scale policy.
- Character identity, spawn, controller movement, or animation graphs.
- Networking authority or replication.
- Project-specific effect assets and semantic event naming.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | EchoFeedback interaction |
|---|---|---|
| Gameplay event and result | Project code, EchoCombat, EchoObjectives, EchoInteraction, or another gameplay authority | Receives a semantic recipe request after or alongside authoritative change |
| Base time and pause | The Pulse or project time authority | Requests a transient multiplier through a provider; standalone provider is exclusive and opt-in |
| Camera movement and shake | EchoCamera or project camera system | Separate bridge/provider maps feedback signal into camera request |
| Audio playback | Jukebot | Separate bridge maps signal into cue request |
| UI hierarchy and animation | EchoUI or project UI | Separate provider executes UI punch, flash, or overlay inside UI authority |
| Global accessibility preferences | The Accord | Bridge applies effective per-channel settings; EchoFeedback does not persist them |
| Device assignment and input context | The Will or project input layer | Haptics provider resolves the intended device through an adapter |
| Runtime diagnostics dashboard | The Observatory | Optional status-provider bridge |
| Project composition | The Workshop | Calls EchoFeedback Editor setup facade through ADR-001 |
| Scene transitions | The Passage | Project/bridge may cancel scene-scoped feedback; Passage remains travel authority |
| Object pooling | The Wellspring or project pool | Providers may use their own declared pools; EchoFeedback core does not become a GameObject pool |

### 5.4 Boundary tests

A feature belongs in EchoFeedback only when all of these are true:

1. It coordinates a transient response to a semantic event.
2. It can be expressed without owning the underlying gameplay rule.
3. It can execute through a provider without bypassing another package authority.
4. It benefits from shared timing, scaling, priority, cancellation, or diagnostics.
5. It remains useful when optional providers are absent.
6. It does not require project-specific content in immutable package source.

If a feature calculates damage, chooses a camera target, plays an AudioSource, changes a UI stack, pairs a gamepad, stores preferences, or saves progress, it belongs to another authority or bridge.

---

## 6. Independence Contract

Independence is a release gate.

### 6.1 Standalone guarantees

EchoFeedback must:

- compile with only its declared Unity dependencies;
- initialize without First Light;
- run recipes without EchoCamera, Jukebot, EchoUI, EchoInput, EchoSettings, EchoGameState, EchoDiagnostics, or EchoSave;
- provide simulated Laboratory providers for visual proof without turning them into production dependencies;
- provide an explicit standalone time provider for projects that choose EchoFeedback as their only time-effect authority;
- avoid direct references to peer package assemblies in the core package;
- expose provider injection and test clocks;
- preserve project recipes and signal assets when bridges are removed;
- fail visibly and safely when a requested provider is absent;
- use no hidden scene name, tag, layer, input map, mixer, camera backend, or Resources path.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Planned evidence |
|---|---|---|
| Installed alone | Core compiles; setup and simulated/standalone providers work | `EFB-T-INST-001`, `EFB-LAB-001` |
| Enter Impact Laboratory directly | One development root initializes if absent | `EFB-T-LIFE-006`, `EFB-LAB-002` |
| First Light absent | Explicit/standalone initialization remains available | `EFB-T-LIFE-003` |
| Optional camera/audio/UI/input bridges absent | Corresponding steps obey requiredness and other channels continue | `EFB-T-PROV-007` through `010` |
| The Accord absent | Project defaults and runtime scale API remain available; no persistence claimed | `EFB-T-SCALE-001` |
| The Pulse absent | Standalone time provider may be selected explicitly | `EFB-T-TIME-001` |
| Duplicate root present | Duplicate exits before registrations, subscriptions, time edits, or recipe execution | `EFB-T-LIFE-004` |
| Required configuration missing | Root enters Failed with actionable diagnostics | `EFB-T-CFG-001` |
| Samples deleted | Runtime and Editor assemblies continue compiling | `EFB-T-INST-008` |
| Bridge removed after project use | Core recipes and stable signal IDs remain; mapped provider assets may become safely unavailable | `EFB-T-REM-004` |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Planned minimum | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity CoreModule | Platform | Yes | Unity 6000.0 | MonoBehaviour, ScriptableObject, time, vectors, transforms, serialization | Package cannot run without Unity |
| Unity Test Framework | Test only | Yes for tests | Verify at implementation | EditMode and PlayMode tests | Test assemblies only |
| Unity Input System | Provider package only | No in core | Verify at implementation | Production haptics provider | Removing provider leaves core intact |
| Peer Sperk’s Forge packages | Bridge only | No in core | Per bridge specification | Optional authority integration | Remove bridge first, then peer/core as allowed |

### 6.4 Forbidden dependencies

- Project assemblies in package runtime or Editor assemblies.
- Direct core references to EchoCamera, Jukebot, EchoUI, EchoInput, EchoSettings, EchoGameState, or EchoDiagnostics.
- Runtime references to `UnityEditor`.
- Hidden reflection-based provider discovery.
- Samples or Laboratory scripts as runtime requirements.
- Cinemachine, post-processing, Input System, TextMeshPro, or uGUI as hidden core dependencies.
- Hard-coded scene names, build indexes, tags, layers, mixer groups, action maps, camera objects, or Resources folders.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| EFB-CAP-001 | Duplicate-safe root | One application-session authority with owned runner/registry/history | Approved | Yes | Runtime | Claim before side effects |
| EFB-CAP-002 | Explicit initialization | Standalone or First Light-triggered idempotent initialization | Approved | Yes | Runtime | Fresh `Awaitable<T>` per call |
| EFB-CAP-003 | Feedback recipes | Immutable stable-ID flat timeline assets | Approved | Yes | Data | No branching or recursion |
| EFB-CAP-004 | Semantic signals | Stable provider-neutral signal definitions | Approved | Yes | Data | Providers map signal IDs |
| EFB-CAP-005 | Timeline runner | Unscaled scheduling of sequential/parallel steps | Approved | Yes | Runtime | Start offsets drive order |
| EFB-CAP-006 | Structured requests/results | Request context, priority, scales, owner/group, and result codes | Approved | Yes | Runtime | No raw provider leakage |
| EFB-CAP-007 | Generational handles | Query/cancel without stale-slot interference | Approved | Yes | Runtime | Bounded registry |
| EFB-CAP-008 | Provider registry | Explicit channel registration with disposable handles | Approved | Yes | Runtime | One active provider per channel ID in MVP |
| EFB-CAP-009 | Provider requiredness | Optional, warning, or blocking step behavior | Approved | Yes | Runtime/Data | Declared per step |
| EFB-CAP-010 | Cancellation | Handle, owner, group, channel, scene policy, shutdown | Approved | Yes | Runtime | Provider support reported honestly |
| EFB-CAP-011 | Overlap policy | Stack, replace group, ignore group, restart group | Approved | Yes | Runtime/Data | Deterministic priority rules |
| EFB-CAP-012 | Capacity limits | Active recipe/step/provider queue hard caps | Approved | Yes | Runtime/Config | No unbounded growth |
| EFB-CAP-013 | Channel scaling | Project, runtime, accessibility, and request multipliers | Approved | Yes | Runtime/Config | Clamp before provider call |
| EFB-CAP-014 | Channel suppression | Disable shake, haptics, flash, UI motion, time, or custom channels | Approved | Yes | Runtime | No persistence ownership |
| EFB-CAP-015 | Safety caps | Maximum intensity, duration, active count, flash/rumble policy seams | Approved | Yes | Config/Runtime | Provider validates channel-specific limits |
| EFB-CAP-016 | Spatial context | Optional position, direction, target, audience, and tags | Approved | Yes | Runtime | No scene refs in recipe assets |
| EFB-CAP-017 | Standalone time provider | Explicit exclusive provider for projects without Pulse | Approved | Yes | Runtime/Provider | Restores exact owned values |
| EFB-CAP-018 | Time-provider contract | Transient multiplier and hit-stop/time-dilation profile mapping | Approved | Yes | Runtime | Final time authority stays external when present |
| EFB-CAP-019 | Camera provider contract | Semantic camera impulse request | Approved | Yes | Runtime/Bridge | Production provider external |
| EFB-CAP-020 | Haptics provider contract | Audience-aware bounded low/high-frequency requests | Approved | Yes | Runtime/Provider | Input System artifact separate |
| EFB-CAP-021 | Visual flash provider contract | Full-screen or target visual response request | Approved | Yes | Runtime/Bridge | UI/render authority external |
| EFB-CAP-022 | UI impulse provider contract | UI punch/emphasis request | Approved | Yes | Runtime/Bridge | EchoUI/project provider |
| EFB-CAP-023 | Audio provider contract | Semantic audio response request | Approved | Yes | Runtime/Bridge | Jukebot bridge |
| EFB-CAP-024 | Custom channels | Stable channel IDs and provider registration | Approved | Yes | Runtime | Same lifecycle rules |
| EFB-CAP-025 | Editor setup facade | ADR-001 compatible plan/apply/validate/repair facade | Approved | Yes | Editor | Workshop callable |
| EFB-CAP-026 | Recipe inspector/preview | Timeline visualization and simulated-provider preview | Approved | Yes | Editor | No production authority required |
| EFB-CAP-027 | Validation/repair | IDs, offsets, limits, providers, mappings, cycles, assets | Approved | Yes | Editor | Non-destructive by default |
| EFB-CAP-028 | Impact Laboratory | Isolated channel, timing, accessibility, and stress proof | Approved | Yes | Sample | Simulated providers allowed |
| EFB-CAP-029 | Structured diagnostics | State, providers, scales, active instances, denials, history | Approved | Yes | Runtime/Editor | Observatory optional |
| EFB-CAP-030 | Recipe nesting | Recipes invoking recipes | Deferred | No | Runtime/Data | Avoid cycles and hidden expansion |
| EFB-CAP-031 | Conditional/branching tracks | Runtime conditions and branches | Rejected for MVP | No | Runtime/Data | Belongs in gameplay or later orchestration design |
| EFB-CAP-032 | Random recipe variants | Weighted recipe selection | Deferred | No | Runtime/Data | Project can select recipe before request |
| EFB-CAP-033 | Timeline/Playable integration | Timeline clips invoking recipes | Deferred | No | Adapter | Separate integration |
| EFB-CAP-034 | Post-processing provider | Bloom, vignette, chromatic effects | Deferred | No | Provider | Rendering-backend specific |
| EFB-CAP-035 | XR haptics | XR device-specific haptic providers | Deferred | No | Provider | Requires platform/provider research |
| EFB-CAP-036 | Network presentation adapter | Local presentation from replicated semantic events | Deferred | No | Bridge | Multiplayer authority remains external |

### 7.2 MVP capability set

The smallest complete release includes:

- one duplicate-safe root;
- immutable `FeedbackRecipe` and `FeedbackSignalDefinition` assets;
- one flat unscaled timeline runner;
- explicit provider registration;
- structured play results and generational handles;
- cancellation by handle, owner, group, and shutdown;
- deterministic overlap and capacity policy;
- per-channel project and accessibility scaling;
- provider contracts for time, camera, haptics, visual flash, UI impulse, audio, and custom channels;
- an opt-in standalone Unity time provider;
- simulated Laboratory providers for visible proof;
- Editor setup, validation, preview, and report tools;
- standalone diagnostics and one isolated Impact Laboratory;
- documented bridge examples and separate provider package plans.

### 7.3 Later capability set

Later approved candidates include:

- nested recipes after cycle, cancellation, and traceability design;
- rendering-pipeline post-processing providers;
- Timeline/Playable integration;
- XR haptic providers;
- network presentation adapters;
- authoring conveniences such as reusable step templates and recipe variants;
- per-channel budgeting informed by measured implementation evidence.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| General visual scripting graph | Rejected | Would turn feedback into a second gameplay logic system | Separate product decision |
| Arbitrary method invocation from recipes | Rejected | Unsafe, untraceable, and authority-breaking | Do not revisit without ADR |
| Recursive recipe nesting | Deferred | Requires cycle, depth, cancellation, and diagnostics design | Flat MVP proven |
| Direct Cinemachine implementation in core | Rejected | Camera backend and EchoCamera authority must remain optional | Separate provider/bridge |
| Direct Jukebot cue type in core recipe | Rejected | Creates hard peer dependency | Jukebot bridge |
| Direct Input System device selection in core | Rejected | Input/device authority belongs elsewhere | Separate haptics provider |
| Save active feedback | Rejected | Feedback is transient presentation state | No planned revisit |
| Unbounded stacking | Rejected | Performance and accessibility hazard | No planned revisit |

---
## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | `FeedbackConfiguration`, `FeedbackRecipe`, `FeedbackSignalDefinition`, channel policies, safety defaults | Active instances, current handles, provider objects, scene references, live accessibility values |
| Runtime state/behavior | Root, runner, provider registry, clocks, scale resolver, active instances, handles, histories | Editor APIs, project gameplay rules, camera/audio/UI implementation assumptions |
| Provider/bridge execution | Time, camera, haptic, flash, UI, audio, and custom provider implementations | Core package ownership of peer-package truth |
| Presentation/sample | Laboratory controls, simulated channel displays, optional inspectors | Authoritative gameplay, persistence, or production UI rules |

The runtime path is:

```text
Semantic gameplay event
    -> FeedbackRequest
        -> EchoFeedbackRoot
            -> recipe validation and admission
            -> scale/safety resolution
            -> FeedbackRunner
                -> unscaled timeline
                    -> channel request
                        -> explicit provider
                            -> provider-owned presentation effect
```

### 8.2 Component topology

```text
EchoFeedbackRoot
├── FeedbackProviderRegistry
├── FeedbackRunner
│   ├── active FeedbackInstance records
│   ├── FeedbackHandle registry
│   └── unscaled scheduling clock
├── FeedbackScaleResolver
├── FeedbackAdmissionPolicy
├── FeedbackDiagnosticBuffer
└── optional StandaloneUnityTimeFeedbackProvider

Project or bridge registrations
├── IFeedbackChannelProvider: camera
├── IFeedbackChannelProvider: haptics
├── IFeedbackChannelProvider: visual-flash
├── IFeedbackChannelProvider: ui-impulse
├── IFeedbackChannelProvider: audio
└── IFeedbackChannelProvider: custom channel IDs
```

The core permits one active provider per exact channel ID in the MVP. A provider may internally route by audience, player, target, or subchannel. Multiple providers requiring distinct ownership use distinct stable channel IDs or a later approved composite provider.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | Yes for the default runtime path; direct service injection remains possible in tests |
| Root type | `EchoFeedbackRoot` |
| Default lifetime | Application session when installed as a persistent service |
| Duplicate behavior | Reject duplicate before provider registration, subscriptions, time mutation, runner creation, or recipe work |
| Initialization trigger | Explicit `InitializeAsync`; standalone component may invoke once after claim |
| Shutdown behavior | Reject new work, cancel active instances, request provider cleanup, restore standalone time ownership, dispose registrations, clear bounded state |
| Direct-scene behavior | Development initializer creates only the configured root when no authority exists |
| Test seam | `IFeedbackService`, `IFeedbackClock`, provider interfaces, and in-memory diagnostics |

The documented convenience access point may expose the active root, but public APIs must also accept an injected `IFeedbackService`. Static access cannot be the only usable path.

### 8.4 Lifecycle sequence

1. **Claim authority** before any side effect.
2. **Preflight configuration** and stable IDs.
3. **Create runtime-owned services** including the runner, clock adapter, scale resolver, and bounded histories.
4. **Register approved built-in providers**, including standalone time only when explicitly selected.
5. **Become Ready** and publish one initialization result.
6. **Accept requests**, resolve recipe/context/scales, and apply admission rules.
7. **Create an active instance** and generational handle.
8. **Schedule steps on unscaled time** and invoke available providers.
9. **Settle steps** as completed, skipped, failed, timed out, or cancelled.
10. **Settle the recipe instance**, publish a terminal result, and release its slot.
11. **On focus, scene, or provider changes**, apply explicit policies rather than guessing.
12. **Shutdown**, restoring owned external state and invalidating all handles.

### 8.5 Feedback instance state machine

```text
Created
  -> Admitted
  -> Scheduled
  -> Running
      -> Completing
      -> Cancelling
      -> Failing
  -> Completed | Cancelled | Failed | Rejected
```

A rejected request never receives a live handle. A terminal handle remains queryable only for the configured bounded history window, then becomes `Expired` rather than ambiguously invalid.

### 8.6 Timeline and provider execution model

Each recipe contains ordered `FeedbackStepDefinition` records. A step declares:

- stable step ID within the recipe;
- channel ID;
- semantic signal ID;
- nonnegative unscaled start offset;
- optional duration override;
- normalized intensity;
- envelope;
- priority offset;
- requiredness;
- overlap group and policy;
- cancellation policy;
- context-consumption flags.

Steps with equal offsets begin in serialized order. Equal-time order is deterministic for diagnostics but providers must not depend on another equal-time step having completed.

The provider interface executes one channel request asynchronously and returns a fresh `Awaitable<FeedbackProviderResult>`. The runner supplies a cancellation token and a hard provider timeout derived from package safety limits. Immediate providers may return a completed awaitable. Providers must complete on the main thread unless their integration specification explicitly documents a safe detached phase and main-thread handoff.

### 8.7 Time authority model

EchoFeedback owns transient feedback-time requests, not final project time truth.

- The core expresses a normalized transient multiplier and timing envelope through the time channel.
- `StandaloneUnityTimeFeedbackProvider` is opt-in, exclusive, and intended only when no other time authority exists.
- The standalone provider records the exact values it owns, uses unscaled time for release, detects external drift, and restores according to explicit policy.
- A project using The Pulse must replace the standalone provider with the Pulse bridge. The bridge combines the feedback multiplier with Pulse base/pause policy, leaving Pulse as the final writer.
- The fixed-timestep response is an explicit provider policy: preserve baseline by default, or scale with effective time only when selected and tested.
- EchoFeedback never assumes that `timeScale == 0` means the game is paused, and never clears a pause owned by another authority.

### 8.8 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Claim | Warning/status | Duplicate destroys/disables with no side effects | `EFB-ROOT-001` |
| Missing configuration | Preflight | Blocking setup error | Root enters Failed | `EFB-CFG-001` |
| Invalid recipe | Request/validation | Rejected result | No instance created | `EFB-RCP-001` |
| Duplicate stable ID | Editor/runtime validation | Error | Conflicting asset/request rejected | `EFB-ID-001` |
| Active-instance limit reached | Admission | Rejected or replacement result | Existing work follows configured policy | `EFB-CAP-001` |
| Required provider missing | Step start | Recipe failure with channel details | Remaining steps cancel or continue by recipe policy | `EFB-PROV-001` |
| Optional provider missing | Step start | Advisory/skip result | Other channels continue | `EFB-PROV-002` |
| Provider registration collision | Registration | Rejected registration | Existing provider remains | `EFB-PROV-003` |
| Provider throws | Provider await | Step failure | Isolation, cancellation, cleanup, recipe policy | `EFB-PROV-004` |
| Provider timeout | Runner | Step timeout | Cancel provider token; continue/fail per policy | `EFB-PROV-005` |
| Stale handle used | API | `Expired` or `InvalidGeneration` | New instance unaffected | `EFB-HND-001` |
| Standalone time drift | Provider update | Warning/error by policy | Adopt, restore, or disable provider explicitly | `EFB-TIME-001` |
| Time provider fails restoration | Shutdown/cancel | Critical diagnostic | Best-effort baseline restore and disable | `EFB-TIME-002` |
| Haptic device lost | Provider | Step cancelled/advisory | Reset available haptics | `EFB-HAP-001` |
| Target destroyed | Provider/context validation | Step skip/cancel | Recipe continues by requiredness | `EFB-CTX-001` |
| Application loses focus | Focus event | Policy result | Reset haptics; cancel/continue other channels as configured | `EFB-FOCUS-001` |
| Diagnostic listener fails | Publish | Development warning | Feedback lifecycle continues | `EFB-DIAG-001` |
| Shutdown during request | API | Rejected/cancelled | Existing cleanup completes | `EFB-LIFE-001` |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable domain ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `FeedbackConfiguration` | Root limits, default policies, provider timeout, history sizes, standalone-time selection | Yes | No | Yes |
| `FeedbackRecipe` | Flat semantic timeline and recipe-level admission/cancellation defaults | Yes | No | Yes |
| `FeedbackSignalDefinition` | Stable semantic signal identity and authoring metadata | Yes | No | Yes |
| `FeedbackSafetyProfile` | Project maximum channel intensities, durations, counts, and focus rules | Yes | No | Yes |
| `FeedbackScaleDefaults` | Default effective channel enable/scale values before Accord/project overrides | Yes | No | Yes |
| Provider mapping asset | Maps signal IDs to provider-specific camera/audio/UI/haptic/time content | Yes, owned by provider/bridge | No | Yes |

Package source may ship empty templates and safe Laboratory samples. Production recipes, signals, mappings, colors, audio cues, shake profiles, and UI presets belong to the project.

### 9.2 Recipe definition

A `FeedbackRecipe` contains:

- `RecipeId` as a stable domain ID;
- display name and authoring description;
- tags;
- default priority;
- default group ID;
- recipe overlap policy;
- cancellation policy;
- missing-required-provider policy;
- maximum declared duration;
- ordered `FeedbackStepDefinition` list.

A step stores only provider-neutral values. It does not serialize Jukebot cues, EchoCamera profiles, UI view objects, Input System devices, or production scene references.

### 9.3 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `FeedbackRuntimeState` | Root | Application session | Recreated on initialization | Never saved |
| `FeedbackInstance` | Runner | One admitted recipe | Released after terminal history window | Never saved |
| `FeedbackStepExecution` | Instance | One step execution | Released on settle | Never saved |
| `FeedbackHandleSlot` | Handle registry | Active plus bounded terminal history | Generation increments on reuse | Never saved |
| `FeedbackProviderRegistration` | Provider registry | Registration lifetime | Disposed explicitly or shutdown | Never saved |
| `FeedbackScaleSnapshot` | Scale resolver | Until next effective-scale change | Recomputed atomically | May be supplied from settings, not saved here |
| `FeedbackDiagnosticEvent` | Diagnostic buffer | Bounded session history | Oldest-first eviction | Exported only by explicit local action |
| `FeedbackContext` | Request/instance | Request lifetime | Discarded on terminal state | Never saved |

### 9.4 Stable identifiers

EchoFeedback uses domain IDs, not Unity asset GUIDs, as runtime contracts.

| Identity | Format | Rule |
|---|---|---|
| Recipe ID | Project/package-qualified semantic ID or approved opaque ID | Unique in the project’s validated recipe set |
| Signal ID | Qualified semantic ID such as `game.feedback.hit.heavy` | Stable across mapping/provider changes |
| Channel ID | Reserved package ID such as `echo.feedback.camera` or project-qualified custom ID | Exact provider-registration key |
| Step ID | Unique within one recipe | Used for diagnostics and migration, not global lookup |
| Group/owner ID | Runtime stable token or project semantic group | Must not rely only on object name or instance index |
| Handle | Runtime slot plus generation | Never durable or network authoritative |

Released IDs use SFGSS-003 aliases or tombstones when renamed. Display names may change without changing IDs. Runtime instance IDs, object names, hierarchy paths, timestamps, and CLR type names are not durable identities.

### 9.5 ScriptableObject safety

Recipe, signal, safety, scale-default, and mapping assets are immutable runtime inputs. They must not store:

- active or last-played state;
- cooldown timestamps;
- active providers;
- current scale values;
- handle generations;
- target transforms;
- scene ownership;
- cancellation tokens;
- per-player device selection.

Editor preview creates detached runtime copies or simulation state and must not write preview progress back into assets.

### 9.6 Serialization and migration

The MVP does not persist active feedback. Durable data consists only of project-authored definition/configuration assets and optional external settings sections.

- Serialized asset schemas declare an independent schema version when migration becomes necessary.
- Recipe migrations operate in Editor tooling on staged data, preserve source assets, and report exact changes.
- Provider mapping assets are migrated by their owning provider/bridge.
- Unknown custom channel IDs remain valid strings even when the provider is absent.
- Removed provider packages must not delete project recipes or semantic signals.
- Any future runtime import/export format must follow SFGSS-003 envelopes, versioning, bounds, unknown-data, and publication rules.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `EchoFeedbackRoot` | sealed `MonoBehaviour` | Default Unity authority and lifecycle host | Scene/prefab/setup tool |
| `IFeedbackService` | interface | Request, cancel, query, scale, provider, and snapshot API | Implemented by root/runtime service |
| `FeedbackConfiguration` | ScriptableObject | Runtime limits and defaults | Project-owned asset |
| `FeedbackRecipe` | ScriptableObject | Immutable semantic timeline | Project-owned asset |
| `FeedbackSignalDefinition` | ScriptableObject | Stable semantic signal | Project-owned asset |
| `FeedbackStepDefinition` | serializable struct/class | One provider-neutral timeline step | Owned by recipe asset |
| `FeedbackRequest` | readonly struct | Recipe plus runtime context and overrides | Caller-created |
| `FeedbackContext` | readonly struct | Position, direction, target, audience, tags, source token | Caller-created; runtime only |
| `FeedbackHandle` | readonly struct | Generational instance reference | Returned by service |
| `FeedbackPlayResult` | readonly struct | Admission outcome, code, handle, diagnostics | Returned by `Play` |
| `FeedbackInstanceSnapshot` | readonly struct | Read-only current/terminal instance state | Service-created |
| `FeedbackChannelRequest` | readonly struct | Resolved provider request | Runner-created |
| `FeedbackProviderResult` | readonly struct | Provider completion status | Provider-created |
| `IFeedbackChannelProvider` | interface | Executes one exact channel ID | Bridge/provider/project implementation |
| `FeedbackProviderRegistration` | disposable struct/class | Owns one provider registration | Service-created |
| `IFeedbackClock` | interface | Unscaled time and async delay seam | Runtime/test implementation |
| `IFeedbackScaleSource` | interface | Supplies runtime/accessibility scales | Project/bridge implementation |
| `FeedbackStatusSnapshot` | readonly model | Root, limits, scales, providers, active counts, recent results | Service-created |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread rule |
|---|---|---|---|---|
| `Awaitable<FeedbackInitializationResult> InitializeAsync(CancellationToken)` | Initialize idempotently | Claimed root/service | Ready, AlreadyReady, Cancelled, or Failed | Main thread completion |
| `FeedbackPlayResult Play(in FeedbackRequest)` | Admit and schedule a recipe | Ready; valid recipe | Live handle or structured rejection | Main thread |
| `Awaitable<FeedbackCompletionResult> AwaitCompletionAsync(FeedbackHandle, CancellationToken)` | Await terminal state | Valid current/terminal handle | Completed, Cancelled, Failed, Expired | Main thread completion |
| `FeedbackCancelResult Cancel(FeedbackHandle, FeedbackCancelMode)` | Cancel one instance | Live handle | Accepted, unsupported remainder, terminal, stale, invalid | Main thread |
| `int CancelByOwner(FeedbackOwnerToken, FeedbackCancelMode)` | Cancel matching instances | Ready | Count accepted | Main thread |
| `int CancelGroup(FeedbackGroupId, FeedbackCancelMode)` | Cancel matching group | Ready | Count accepted | Main thread |
| `int CancelChannel(FeedbackChannelId, FeedbackCancelMode)` | Cancel matching channel executions | Ready | Count accepted | Main thread |
| `FeedbackProviderRegistration RegisterProvider(IFeedbackChannelProvider)` | Register exact channel provider | Ready or initializing registration phase | Registration or collision result | Main thread |
| `bool TryGetInstance(FeedbackHandle, out FeedbackInstanceSnapshot)` | Query state | Any initialized state | False if invalid/expired | Main thread |
| `FeedbackScaleApplyResult SetRuntimeScales(in FeedbackChannelScaleSet)` | Apply nonpersistent scale layer | Ready | Atomic validated snapshot or rejection | Main thread |
| `FeedbackStatusSnapshot CaptureStatus()` | Capture bounded diagnostics | Initialized | Synchronous detached snapshot | Main thread; nonblocking |
| `Awaitable<FeedbackShutdownResult> ShutdownAsync(CancellationToken)` | Cancel, restore, dispose | Initialized | Settled cleanup result | Main thread completion |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `InitializationChanged` | Root | After state change | Initialization snapshot | Listener failure isolated |
| `RecipeStarted` | Runner | After instance becomes Running | Handle, recipe ID, context summary | Not gameplay authority |
| `StepSettled` | Runner | After provider step terminal state | Recipe/step/channel/result | Development use; may be filtered |
| `RecipeCompleted` | Runner | After all steps settle successfully | Handle and completion summary | State already terminal |
| `RecipeCancelled` | Runner | After cancellation cleanup | Handle, reason, provider cleanup summary | State already terminal |
| `RecipeFailed` | Runner | After failure policy completes | Handle, code, failed steps | State already terminal |
| `RequestRejected` | Admission | After validation/admission decision | Recipe ID, code, capacity/policy detail | No handle created |
| `ProviderChanged` | Registry | After registration/disposal | Channel/provider status | Existing instances follow documented teardown policy |
| `EffectiveScalesChanged` | Scale resolver | After atomic snapshot swap | New detached scale snapshot | No persistence implied |

Events are raised after authoritative runtime state changes. No listener is required for recipe execution or cleanup to complete.

### 10.4 Async and cancellation policy

- Public asynchronous operations return a fresh Unity `Awaitable<T>` for every call.
- The core uses `CancellationToken` for initialization, provider execution, completion waits, and shutdown.
- Recipe scheduling uses an injected unscaled clock so hit stop cannot prevent its own release.
- Cancelling a caller’s `AwaitCompletionAsync` wait does not automatically cancel the feedback instance.
- Cancelling the feedback handle requests provider cancellation according to step policy.
- A provider reports whether cancellation was completed, deferred to release, unsupported, or failed.
- Shutdown uses a bounded cleanup timeout. Providers that fail to settle are diagnosed and force-disposed only through their documented contract.
- Application-exit cancellation is honored where practical, but time/haptics providers must also implement synchronous best-effort reset hooks.

### 10.5 API ergonomics

Novice path:

```text
Create recipe -> configure root -> call Play(recipe) -> optionally Cancel(handle)
```

Programmer path:

```text
Inject IFeedbackService
Register project/bridge providers
Build FeedbackRequest with context, owner, audience, priority, and scale
Inspect structured results and await/cancel by handle
```

The API returns result objects rather than relying only on logs. Raw provider internals, coroutines, camera objects, AudioSources, UI views, and input devices are not exposed through the core service.

---
## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install `com.echodevgames.echo-feedback` through a supported route.
2. Open **Tools > EchoDevGames > Impact > Setup**.
3. Select standalone or external time-authority mode.
4. Select project defaults for capacity, timeouts, safety caps, scales, and direct-scene development.
5. Preview the plan and exact paths.
6. Create project-owned configuration, root prefab, empty signal catalog, and optional starter recipes.
7. Import the Impact Laboratory sample separately.
8. Run validation.
9. Open the Laboratory and execute the minimum functional workflow.
10. Save the setup report in project documentation when desired.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create configuration | Project-owned configuration/safety/scale assets | Nothing existing by default | Yes | Undo for newly created assets | Paths, IDs, defaults |
| Create root prefab | Project-owned root prefab | Nothing existing by default | Yes | Undo/new asset deletion | Components and assignments |
| Add root to Boot scene | One configured root instance | Selected scene only | Yes with duplicate detection | Unity Undo and scene dirty report | Existing/new authority result |
| Create starter signals/recipes | Optional project-owned sample assets | Nothing existing by default | Yes with ID/path checks | Undo/new assets | IDs and skipped conflicts |
| Repair references | Missing assignments selected by user | Selected prefab/configuration | Yes | Undo | Before/after fields |
| Migrate released schema | Staged replacement or updated copy | Explicit selected assets | Repeatable migration rules | Backup/source preservation | Version path and changes |
| Remove generated setup | Removal plan only by default | Selected generated/adopted objects | Requires explicit confirmation | Backup/receipt | Safe/unsafe/remain list |

All setup operations implement the ADR-001 Editor setup facade protocol. The Workshop may request a plan and invoke approved operations, but the EchoFeedback Editor assembly owns the actual logic.

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Impact Setup | Installer | Plan, create, validate, and repair project setup | No |
| Recipe Inspector | Designer | Edit timeline offsets, channels, signals, intensities, policies, and duration visualization | No |
| Recipe Preview | Designer/tester | Run simulated providers and inspect step order/cancellation | No |
| Signal Browser | Designer | Search stable signal IDs, references, and provider mappings | No |
| Provider Mapping Inspector | Integrator | Validate bridge/provider signal mappings | Owned by provider/bridge |
| Impact Runtime Monitor | Developer | Inspect active instances/providers/scales during Play Mode | No production dependency |
| Validation Window | Maintainer | Run package/project checks and export report | No |
| Laboratory Launcher | Tester | Import/open instructions and scene | Sample only |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| `EFB-VAL-001` | Missing root configuration | Blocker | Yes | Create only |
| `EFB-VAL-002` | Duplicate root in planned production scenes | Error | Yes | No, user selects authority |
| `EFB-VAL-003` | Empty or duplicate recipe/signal ID | Error | Yes | Only unreleased/new assets |
| `EFB-VAL-004` | Duplicate step ID within recipe | Error | Yes | Regenerate only with confirmation |
| `EFB-VAL-005` | Negative offset/duration or invalid intensity | Error | Yes | Clamp only in explicit repair |
| `EFB-VAL-006` | Recipe declared duration exceeds project safety cap | Error | No | No |
| `EFB-VAL-007` | Required provider channel has no planned mapping | Error | No | No |
| `EFB-VAL-008` | Optional provider channel unavailable | Warning | No | No |
| `EFB-VAL-009` | Standalone time provider and Pulse bridge both planned | Blocker | Yes | No, choose one authority path |
| `EFB-VAL-010` | Active/step capacity is zero or unsafe | Error | Yes | Safe defaults with confirmation |
| `EFB-VAL-011` | Provider timeout below minimum cleanup allowance | Warning/Error | Yes | Safe default with confirmation |
| `EFB-VAL-012` | Accessibility channel cannot be suppressed/scaled by provider | Warning | No | Provider update required |
| `EFB-VAL-013` | Recipe references missing signal asset | Error | No | No |
| `EFB-VAL-014` | Signal ID is mapped ambiguously inside one provider | Error | No | No |
| `EFB-VAL-015` | Sample/Laboratory script referenced by production asset | Blocker | No | Manual removal |
| `EFB-VAL-016` | Runtime assembly references Editor/peer package | Blocker | No | Assembly correction |
| `EFB-VAL-017` | Public asset `.meta` missing or GUID changed unintentionally | Blocker | No | Restore source control identity |
| `EFB-VAL-018` | Unbounded diagnostic history or capacity setting | Error | Yes | Safe bounded default |

Repair is explicit and non-destructive by default. Validation never mutates production assets silently.

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Planned supported routes:

- embedded package development;
- local UPM path;
- Git URL/tag;
- distributable tarball;
- Workshop selection after the package’s setup facade is implemented;
- registry distribution only after the suite’s release strategy is approved.

Each advertised route requires separate SFGSS-004 evidence before being marked Supported.

### 12.2 Minimal scene setup

Minimum standalone setup:

1. One project-owned `FeedbackConfiguration`.
2. One `EchoFeedbackRoot` referencing that configuration.
3. At least one `FeedbackRecipe` and `FeedbackSignalDefinition`.
4. At least one registered provider. This may be a simulated Laboratory provider or the explicit standalone time provider.
5. One project script or Laboratory control that submits a `FeedbackRequest`.

No EventSystem, Canvas, camera backend, audio mixer, input action asset, save file, or peer package is required by the core.

### 12.3 Boot-scene setup

Normal production options:

- place the root in a canonical Boot/preload scene; or
- let a First Light startup step create/initialize the root; or
- create the root from explicit project composition code.

Only one path may claim authority. The setup validator reports overlapping plans.

### 12.4 Direct-scene setup

`EchoFeedbackDirectSceneInitializer` is a development helper that:

- checks for an existing authority first;
- creates only the configured minimal root when absent;
- identifies the session as development initialization;
- refuses to create a second time authority;
- is disabled or excluded from release builds by default;
- never creates camera, audio, UI, input, or settings authorities.

### 12.5 Scene isolation rule

The Standalone Laboratory may use sample-only simulated providers to make channels visible. Those providers must live in sample assemblies and cannot be referenced by production runtime assets. Production bridge proof belongs in separate Integration Laboratories owned by the bridge/provider artifact.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Test Lab purpose

The **Impact Laboratory** proves that EchoFeedback can coordinate, scale, cancel, bound, diagnose, and recover transient feedback without any unrelated Sperk’s Forge package.

It uses simple redistributable shapes and simulated provider panels for camera, flash, UI, audio, and haptics. The standalone time provider may affect the Laboratory’s moving test object. Simulated channels make lifecycle and intensity visible but do not claim production integration support.

### 13.2 Required Laboratory contents

- A clear README and in-scene instruction panel.
- One development root and project-owned sample configuration.
- Sample semantic signals for light hit, heavy hit, denied action, explosion, victory, and warning.
- Recipes demonstrating sequential and parallel steps.
- Simulated camera, flash, UI, audio-meter, and haptic-meter providers.
- The standalone Unity time provider with a moving test object and unscaled status clock.
- Controls for play, cancel, spam, replace, disable channel, remove provider, lose target, and reset.
- Per-channel scale and suppression controls.
- Active instance, step, provider, handle, capacity, and diagnostic readouts.
- Duplicate-root test control.
- Focus-loss/device-loss simulation controls where real platform events cannot be forced safely.
- A reset action that restores time, providers, scales, histories, and scene state.

### 13.3 Laboratory acceptance checklist

| Test ID | Action | Expected result | Mode | Status |
|---|---|---|---|---|
| `EFB-LAB-001` | Enter scene directly | One development root initializes | Manual | Not run |
| `EFB-LAB-002` | Play light-hit recipe | Configured channels begin and complete | Manual | Not run |
| `EFB-LAB-003` | Play heavy-hit recipe | Greater but clamped response is visible | Manual | Not run |
| `EFB-LAB-004` | Play chained recipe | Step offsets execute in unscaled order | Manual | Not run |
| `EFB-LAB-005` | Play parallel recipe | Equal-offset steps begin deterministically | Manual | Not run |
| `EFB-LAB-006` | Cancel live handle | Remaining steps and providers settle by policy | Manual | Not run |
| `EFB-LAB-007` | Reuse handle slot then cancel stale handle | New instance remains unaffected | Manual | Not run |
| `EFB-LAB-008` | Spam below capacity | All admitted work remains bounded | Manual | Not run |
| `EFB-LAB-009` | Exceed capacity | Requests reject/replace according to policy | Manual | Not run |
| `EFB-LAB-010` | Replace same overlap group | Old instance cancels and new one runs | Manual | Not run |
| `EFB-LAB-011` | Ignore active overlap group | New request is rejected with code | Manual | Not run |
| `EFB-LAB-012` | Disable camera channel | Camera provider receives no effective request | Manual | Not run |
| `EFB-LAB-013` | Scale haptics to 25% | Meter/provider receives scaled values | Manual | Not run |
| `EFB-LAB-014` | Suppress flashes | Flash step skips safely | Manual | Not run |
| `EFB-LAB-015` | Apply master request scale | All channel intensities scale within caps | Manual | Not run |
| `EFB-LAB-016` | Remove optional provider | Optional step warns/skips; others continue | Manual | Not run |
| `EFB-LAB-017` | Remove required provider | Recipe fails according to policy | Manual | Not run |
| `EFB-LAB-018` | Force provider exception | Failure is isolated and diagnosed | Manual | Not run |
| `EFB-LAB-019` | Force provider timeout | Token cancels and recipe settles | Manual | Not run |
| `EFB-LAB-020` | Run hit stop | Scaled object pauses/slows while unscaled clock and release continue | Manual | Not run |
| `EFB-LAB-021` | Cancel hit stop | Exact provider-owned time state restores | Manual | Not run |
| `EFB-LAB-022` | Simulate external time drift | Declared drift policy and diagnostic occur | Manual | Not run |
| `EFB-LAB-023` | Destroy target during effect | Target-dependent step settles safely | Manual | Not run |
| `EFB-LAB-024` | Simulate focus loss | Haptics reset and configured policies execute | Manual | Not run |
| `EFB-LAB-025` | Introduce duplicate root | Duplicate has no side effects | Manual | Not run |
| `EFB-LAB-026` | Cancel by owner | Only matching instances cancel | Manual | Not run |
| `EFB-LAB-027` | Cancel by group | Group cancels; unrelated group continues | Manual | Not run |
| `EFB-LAB-028` | Cancel channel | Matching provider executions settle | Manual | Not run |
| `EFB-LAB-029` | Change scales while active | New requests use atomic new snapshot; active policy follows spec | Manual | Not run |
| `EFB-LAB-030` | Reset Laboratory | Time, providers, scales, handles, and state return to baseline | Manual | Not run |
| `EFB-LAB-031` | Exit Play Mode during hit stop/rumble | Synchronous reset prevents stuck external state | Manual | Not run |
| `EFB-LAB-032` | Delete sample folder | Core package still compiles | Manual | Not run |

### 13.4 Optional integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| Impact + Jukebot | EchoFeedback, Jukebot, bridge | Map audio signal to Jukebot cue | Depends on two authorities and bridge |
| Impact + Pulse | EchoFeedback, EchoGameState, bridge | Compose transient time multiplier with base/pause policy | Depends on Pulse |
| Impact + Looking Glass | EchoFeedback, EchoUI, bridge | UI punch and screen-flash provider | Depends on EchoUI hierarchy/presenter |
| Impact + Eye | EchoFeedback, EchoCamera, bridge | Camera impulse provider | Depends on future EchoCamera |
| Impact + Accord | EchoFeedback, EchoSettings, bridge | Apply reduced-motion/flash/rumble/shake preferences | Depends on settings authority |
| Impact Input System Haptics | Core plus provider package | Production dual-motor haptics | Depends on Input System and device resolution |

Samples are separately importable/removable and never core runtime requirements.

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The core is nonvisual. It exposes recipe, instance, provider, scale, and diagnostic state. Production camera, flash, UI, audio, and haptic presentation belongs to provider or bridge artifacts.

The Impact Laboratory may use uGUI/TextMeshPro or simple scene visuals inside sample-only assemblies. Those dependencies do not enter the neutral runtime assembly.

### 14.2 Required states

Presenters and provider diagnostics must be able to represent:

- Ready.
- Busy/active.
- Disabled/suppressed.
- Provider unavailable.
- Capacity limited.
- Warning/advisory.
- Failed.
- Cancelling.
- Completed.
- Unsupported on current device/platform.

### 14.3 Accessibility requirements

- Every channel can be scaled or suppressed independently where meaningful.
- Reduced-motion policy can affect camera, UI motion, flash motion, and time effects without muting unrelated audio automatically.
- Flash providers expose intensity/duration caps and a non-flashing fallback seam.
- Haptic providers expose enable, intensity, duration, and device-supported state.
- Time effects can be disabled or reduced independently from camera shake.
- Important gameplay information must not depend on feedback completion or one sensory channel.
- Provider UIs and Laboratory controls support keyboard navigation, controller navigation when the selected sample includes it, readable contrast, scalable text, and color-independent status.
- Settings initialization must not play feedback or create preview side effects unless the user explicitly requests preview.
- Project-defined maximums remain in force even when a user preference would otherwise increase intensity.

### 14.4 Scale composition

The effective intensity for one channel is conceptually:

```text
recipe intensity
    x request scale
    x project channel scale
    x accessibility channel scale
    x provider/audience scale
    -> safety clamp
```

Each layer is validated and bounded. A disabled layer produces zero effective intensity and a documented skipped/suppressed result. The package does not silently reinterpret a zero value as provider failure.

### 14.5 Visual customization

All production visuals, curves, colors, materials, prefabs, animations, and mappings are project-owned or provider-owned. Replacing them must not require editing the core package.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost policy |
|---|---|---|---|
| Initialization state | API/Inspector/log | Editor, development, release-safe summary | Event-driven |
| Root identity and configuration | API/Inspector | Development/release-safe IDs only | On demand |
| Registered providers | API/monitor | Development; redacted release option | On change |
| Effective channel scales | API/monitor | Development; release-safe normalized values | On change |
| Active recipe/step counts | API/monitor | Development/release optional | Bounded sampling |
| Capacity and rejection counters | API/monitor | Development | Accumulated bounded counters |
| Recent recipe results | Bounded history | Development | Fixed capacity |
| Provider failures/timeouts | Result/log/history | All builds with configurable detail | Event-driven |
| Time-provider restoration/drift | Result/log/status | All builds | Event-driven |
| Local support snapshot | Explicit export | Editor/development; release only if approved | On demand |

### 15.2 Structured status

`FeedbackStatusSnapshot` includes:

- package version;
- initialization state;
- authority instance ID suitable for local diagnostics;
- configuration domain ID;
- active provider channel IDs and health;
- standalone/external time-authority mode;
- effective channel enable/scale values;
- active and peak instance/step counts;
- capacity limits;
- admitted, rejected, completed, cancelled, failed, and timed-out counters;
- recent diagnostic codes;
- application focus state;
- last shutdown/restoration result when available.

It excludes full project content, rendered text, device serial numbers, typed input, save data, and arbitrary provider payloads.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| `EFB-ROOT-001` | Warning | Duplicate root rejected | Keep one authority or fix scene/setup plan |
| `EFB-CFG-001` | Blocker | Required configuration missing/invalid | Run setup/validation |
| `EFB-ID-001` | Error | Stable ID empty or duplicated | Repair unreleased ID or add migration alias |
| `EFB-RCP-001` | Error | Recipe invalid at request time | Fix recipe validation errors |
| `EFB-CAP-001` | Advisory/Error | Capacity limit reached | Tune policy/limits or reduce request volume |
| `EFB-PROV-001` | Error | Required provider missing | Install/register bridge/provider or change requiredness |
| `EFB-PROV-002` | Advisory | Optional provider missing | Install provider if effect is desired |
| `EFB-PROV-003` | Error | Provider channel registration collision | Keep one provider per channel ID |
| `EFB-PROV-004` | Error | Provider failed or threw | Inspect provider-specific diagnostics |
| `EFB-PROV-005` | Error | Provider timed out | Fix provider cleanup/completion contract |
| `EFB-HND-001` | Advisory | Handle invalid, stale, or expired | Do not retain handles beyond lifetime |
| `EFB-TIME-001` | Warning/Error | External drift detected by standalone time provider | Choose drift policy or use external authority bridge |
| `EFB-TIME-002` | Critical | Owned time values failed to restore | Reset time authority and inspect provider |
| `EFB-HAP-001` | Advisory | Haptic device lost/unsupported | Use fallback or disable channel |
| `EFB-CTX-001` | Advisory | Target/context became unavailable | Choose step requiredness/fallback |
| `EFB-FOCUS-001` | Info/Warning | Focus policy changed/cancelled effects | Verify intended focus policy |
| `EFB-DIAG-001` | Warning | Diagnostic listener failed | Fix listener; feedback continued |
| `EFB-LIFE-001` | Info/Warning | Request arrived during shutdown | Retry only after valid initialization |

### 15.4 Observatory bridge

A separate bridge exposes a provider-neutral diagnostic panel with:

- root health;
- provider inventory and health;
- channel scales and suppression;
- active/peak counts;
- request result counters;
- recent failures/cancellations;
- standalone/external time-provider state;
- optional bounded recipe timeline trace.

EchoFeedback does not depend on The Observatory.

### 15.5 Logging policy

- Logs use category and stable diagnostic code.
- Normal successful requests do not produce per-step Console spam.
- High-volume traces are opt-in and bounded.
- Provider payloads are summarized, not dumped.
- Release logging omits project asset paths, object hierarchy paths, device identifiers, and arbitrary context data.
- Structured results remain the primary programmatic failure surface.

---
## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Recipe/signal/configuration assets | Project definition | Project/EchoFeedback authoring | Unity asset serialization | Unity project assets |
| Effective accessibility/channel scales | Global preference/runtime | The Accord or project; applied to EchoFeedback | Not by EchoFeedback | External settings backend |
| Active instances and handles | Session/transient | EchoFeedback | No | None |
| Provider registrations | Session | EchoFeedback/provider | No | None |
| Diagnostic history | Bounded session | EchoFeedback | No by default | Explicit local support export only |
| Provider mapping assets | Project definition | Bridge/provider/project | Unity asset serialization | Unity project assets |

### 16.2 Standalone behavior

Without EchoSave or EchoSettings:

- project-authored defaults initialize all channels;
- runtime code may apply nonpersistent channel scales;
- active feedback is discarded on shutdown;
- no save slot or global preference file is created;
- no claim is made that user accessibility choices survive sessions.

### 16.3 Optional settings contract

The Accord bridge may define a versioned EchoFeedback settings section containing values such as:

- master feedback enable/scale;
- camera-shake enable/scale;
- hit-stop/time-effect enable/scale;
- haptic enable/scale;
- flash enable/scale;
- UI-motion enable/scale;
- channel-specific duration limits where approved.

The section owner, schema, migration, unknown-field behavior, and persistence remain with The Accord bridge. EchoFeedback receives an effective detached scale snapshot and never writes the settings document directly.

### 16.4 Save integration

EchoSave integration is not required for the MVP. Active feedback must not be captured as durable gameplay state. A project loading a save may cancel all scene- or owner-scoped feedback before applying authoritative state, but that coordination belongs to project code or a later explicit integration.

### 16.5 Failure and recovery

- Missing settings data uses project defaults.
- Invalid scale values are rejected or clamped by explicit policy and diagnosed.
- Removing the Accord bridge preserves its opaque settings section according to SFGSS-003.
- Reinstalling the bridge may reapply preserved compatible values.
- A failed settings apply leaves the previous effective scale snapshot active.
- No persistence failure may leave time or haptics active.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Optional integrations are explicit, removable, versioned, and visible in package manifests and assembly references. The core publishes neutral provider and scale contracts. Each bridge owns translation between EchoFeedback signals and the concrete authority it connects.

### 17.2 Planned integrations

| Other authority/provider | Connection type | Owner of artifact | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| First Light | Tiny startup integration or separate bridge per SFGSS-002 review | Integration owner | First Light -> EchoFeedback | Initialize/shutdown status | No |
| The Observatory | Separate bridge | Bridge package | EchoFeedback -> Observatory | Status snapshots and bounded events | No |
| The Accord | Separate two-package bridge | Bridge package | Accord -> EchoFeedback | Effective channel/accessibility scale snapshot | No |
| The Passage | Project adapter or later bridge | Project/bridge | Passage -> EchoFeedback | Cancel scene-scoped groups on transition | No |
| The Pulse | Separate two-package bridge | Bridge package | EchoFeedback -> Pulse | Transient time multiplier requests/results | No |
| Jukebot | Separate two-package bridge | Bridge package | EchoFeedback -> Jukebot | Signal ID, intensity, context -> cue request | No |
| The Will | Project adapter or haptics-provider bridge | Provider/bridge | EchoFeedback -> Will/device resolver | Audience/device resolution only | No |
| The Looking Glass | Separate bridge or project provider | Bridge/project | EchoFeedback -> EchoUI | Flash/UI impulse signal and context | No |
| The Workshop | Editor setup facade | EchoFeedback Editor assembly | Workshop -> EchoFeedback | Plan/apply/validate/repair operations | No runtime dependency |
| EchoCamera | Separate bridge | Bridge package | EchoFeedback -> EchoCamera | Camera impulse signal/context | No |
| Unity Input System | Separate provider package | EchoFeedback provider family | EchoFeedback -> Input System | Resolved device and bounded haptic request | No |
| Project custom provider | Interface implementation | Project | EchoFeedback -> project system | Channel request/result | No |

### 17.3 Bridge placement decisions

- Peer-to-peer Echo integrations that reference both packages ship separately when including them in either core would create a hard dependency.
- The Input System haptics implementation is a separate provider package because the core must not own device selection or force Input System on every consumer.
- Laboratory simulated providers remain sample-only.
- The standalone Unity time provider may live in the core package because it depends only on Unity and is explicitly exclusive/optional.
- Game-specific translation remains project-local even when a sample demonstrates the pattern.

Proposed artifact IDs, subject to later integration specifications:

```text
com.echodevgames.echo-feedback.input-system
com.echodevgames.echo-feedback.echo-game-state
com.echodevgames.echo-feedback.jukebot
com.echodevgames.echo-feedback.echo-ui
com.echodevgames.echo-feedback.echo-settings
com.echodevgames.echo-feedback.echo-camera
com.echodevgames.echo-feedback.echo-diagnostics
```

### 17.4 Provider contract requirements

Every provider must declare:

- exact stable channel ID;
- package/provider version;
- whether it supports cancellation, release, focus loss, audience routing, spatial context, and live rescaling;
- accepted signal mappings;
- intensity/duration bounds;
- timeout and shutdown behavior;
- main-thread/background rules;
- unsupported platform/device result;
- diagnostics and privacy behavior;
- removal instructions.

Provider registration returns a disposable handle. Disposal rejects new steps. Active step behavior is explicit: finish, cancel, or transfer is never guessed. In the MVP, transfer is unsupported.

### 17.5 Integration failure behavior

| Failure | Required behavior |
|---|---|
| Peer absent | Bridge package is not installed; core remains unchanged |
| Provider missing | Step follows requiredness; no reflection discovery |
| Version mismatch | Registration fails with compatibility code; no partial connection |
| Provider removed while active | New steps reject; existing steps cancel/finish by provider declaration |
| Initialization order differs | Late registration is allowed; current recipes do not retroactively start skipped steps |
| Settings arrive late | Atomic new scale snapshot affects new requests; active rescale only if provider/recipe permits |
| Pulse removed | Project must select standalone time provider or disable time channel explicitly |
| Jukebot removed | Audio steps become unavailable; recipes and signals remain |
| Input device removed | Haptics provider resets device and reports cancellation/unsupported state |
| Shutdown order | Bridges/providers unregister before core root disposal where possible; core still performs bounded cleanup |

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

Targets are design goals and remain `Not run` until measured.

| Metric | Planned target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Idle core allocations | Zero recurring managed allocation per frame after initialization | Profiler in empty Laboratory | Must pass measured stable gate |
| Play admission | Bounded work proportional to recipe step count | EditMode benchmark/Profiler | No scene-wide search or reflection |
| Active scheduling | Linear in active steps with configured hard cap | Stress Laboratory | No unbounded growth |
| Diagnostic storage | Fixed-capacity buffers | Runtime monitor | Never exceeds configured cap |
| Provider registration lookup | O(1) average exact channel lookup | Unit benchmark | No assembly scan |
| Cancellation | Bounded by matching active instances/steps | Stress test | Completes within configured cleanup timeout |
| Standalone time provider | Constant update cost | Profiler | No allocation in hot update path |

### 18.2 Allocation policy

- Pre-size active instance, handle, and diagnostic collections from configuration.
- Avoid LINQ, reflection, closures, and string formatting in hot request/update paths.
- Reuse internal execution records through bounded pools owned by EchoFeedback, not EchoPool.
- Do not expose pooled mutable objects publicly.
- Copy small immutable request structs; avoid cloning provider-specific assets.
- Diagnostic message formatting may be deferred until a detailed view is requested.
- Providers document their own allocation budget.

### 18.3 Scene and domain reload behavior

- All event subscriptions and provider registrations are disposed cleanly.
- Static convenience access resets through documented runtime initialization.
- Enter Play Mode options with domain reload disabled require explicit static reset tests.
- On Play Mode exit, standalone time and haptic providers perform synchronous best-effort reset.
- Destroyed scene targets are weakly observed or validated before provider use.
- Persistent root survives scene changes only when configured.
- Direct-scene helper never duplicates an existing persistent root.

### 18.4 Scalability limits

The configuration must declare hard limits for:

- active recipes;
- active steps;
- queued/admitted requests per frame;
- providers;
- per-group concurrency;
- maximum recipe duration;
- provider timeout;
- terminal handle history;
- diagnostic history;
- maximum tags/context items.

Advertised values are not finalized until implementation measurements. Exceeding a limit produces a deterministic reject/replace result rather than dynamic unbounded expansion.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

EchoFeedback normally handles no credentials, personal information, network messages, or durable player records. Runtime context may indirectly contain project object references or audience keys, so diagnostics must not serialize arbitrary context payloads.

Haptics providers must not retain device serial numbers, platform account IDs, or raw input history. Support snapshots include only provider type/version, channel availability, normalized request counts, and redacted audience summaries.

### 19.2 Trust boundaries

- Recipe and mapping assets are project-authored input and must be validated for bounds and IDs.
- Custom providers are trusted code but isolated behind timeouts, cancellation, and structured failure.
- External/network events must be validated by their gameplay/network authority before requesting local feedback.
- A recipe cannot invoke arbitrary methods or deserialize executable type names.
- Support exports are explicit and local; the package never transmits them automatically.
- Provider-specific native plugins require separate security/licensing review.

### 19.3 Platform behavior

| Platform | Planned core status | Special behavior | Evidence required |
|---|---|---|---|
| Windows | Planned | Full core; haptics depend on device/provider | Clean build, Laboratory, focus/device tests |
| macOS | Planned | Full core; haptics support may differ | Clean build and provider capability tests |
| Linux | Planned | Full core; device/provider variation | Clean build and provider capability tests |
| WebGL | Planned core, provider-limited | Haptics/time/focus behavior may differ | Explicit browser/device matrix |
| Mobile | Planned core | Vibration/haptics require platform providers; focus/suspend policy critical | Device tests |
| Console | Unknown until approved access/testing | Platform SDK haptics provider required | Provider-specific evidence |
| XR | Deferred provider | Device-specific haptics and comfort policy | Dedicated design and device tests |

No platform moves from Planned to Supported without exact SFGSS-004 evidence.

### 19.4 Focus, suspend, and shutdown safety

- Haptics providers reset on device removal, application focus loss, suspend, provider disposal, and shutdown unless a platform specification requires another safe sequence.
- Standalone time provider restores owned values on cancellation, focus policy, disable, destroy, Play Mode exit, and shutdown.
- Visual providers must not leave permanent overlay/material state after cancellation.
- Provider cleanup is idempotent.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-feedback/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   └── Developer/
│       ├── Architecture.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Runtime/
│   ├── Core/
│   ├── Data/
│   ├── Configuration/
│   ├── Providers/
│   │   └── StandaloneTime/
│   ├── Diagnostics/
│   └── EchoDevGames.EchoFeedback.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Preview/
│   ├── Inspectors/
│   └── EchoDevGames.EchoFeedback.Editor.asmdef
├── Samples~/
│   └── Impact Laboratory/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoFeedbackRoot.cs
│   ├── EchoFeedbackRuntime.cs
│   ├── IFeedbackService.cs
│   ├── FeedbackRunner.cs
│   ├── FeedbackProviderRegistry.cs
│   ├── FeedbackScaleResolver.cs
│   ├── FeedbackAdmissionPolicy.cs
│   └── FeedbackDirectSceneInitializer.cs
├── Data/
│   ├── FeedbackRecipe.cs
│   ├── FeedbackSignalDefinition.cs
│   ├── FeedbackStepDefinition.cs
│   ├── FeedbackEnvelope.cs
│   └── FeedbackIds.cs
├── Configuration/
│   ├── FeedbackConfiguration.cs
│   ├── FeedbackSafetyProfile.cs
│   └── FeedbackScaleDefaults.cs
├── Requests/
│   ├── FeedbackRequest.cs
│   ├── FeedbackContext.cs
│   ├── FeedbackPlayResult.cs
│   ├── FeedbackHandle.cs
│   └── FeedbackCompletionResult.cs
├── Providers/
│   ├── IFeedbackChannelProvider.cs
│   ├── FeedbackChannelRequest.cs
│   ├── FeedbackProviderResult.cs
│   ├── FeedbackProviderRegistration.cs
│   └── StandaloneTime/
│       ├── StandaloneUnityTimeFeedbackProvider.cs
│       └── StandaloneTimeProviderConfiguration.cs
└── Diagnostics/
    ├── FeedbackStatusSnapshot.cs
    ├── FeedbackDiagnosticCode.cs
    └── FeedbackDiagnosticBuffer.cs

Editor/
├── Setup/
│   ├── EchoFeedbackSetupWindow.cs
│   ├── EchoFeedbackSetupFacade.cs
│   └── EchoFeedbackSetupOperations.cs
├── Validation/
│   ├── EchoFeedbackValidator.cs
│   └── EchoFeedbackValidationWindow.cs
├── Preview/
│   ├── FeedbackRecipePreviewWindow.cs
│   └── SimulatedFeedbackProviders.cs
└── Inspectors/
    ├── FeedbackRecipeEditor.cs
    └── FeedbackSignalDefinitionEditor.cs
```

Exact filenames remain implementation details unless public Unity asset identity requires stability.

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoFeedback.Runtime` | Runtime | Unity modules only | Yes | Neutral runtime/data/provider contracts |
| `EchoDevGames.EchoFeedback.Editor` | Editor | Runtime, UnityEditor | No | Setup, validation, preview, inspectors, ADR-001 facade |
| `EchoDevGames.EchoFeedback.Tests.Editor` | Editor tests | Runtime, Editor, Test Framework | No | Validation/data/tool tests |
| `EchoDevGames.EchoFeedback.Tests.Runtime` | PlayMode tests | Runtime, Test Framework | No | Lifecycle, runner, provider, time, stress tests |
| Laboratory sample assembly | Sample | Runtime and sample-selected presentation dependencies | No | Isolated demonstration only |

Separate provider/bridge packages own their own assemblies and declare concrete dependencies under SFGSS-002.

### 20.4 Repository files

- Root README and package README.
- Installation and five-minute quick start.
- Recipe and signal authoring guide.
- Provider implementation guide.
- Standalone time authority guide and warnings.
- Accessibility/channel-scale guide.
- Impact Laboratory guide.
- Integration index.
- Diagnostics and code reference.
- Testing/release checklist.
- Migration/upgrade guide.
- License, contribution, support, security, credits, and third-party notices.
- Stable `.meta` files for public scripts, templates, samples, and assets.

---
## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | Planned 6000.0 | Not run; development baseline 6000.3.8f1 | Exact support claim requires clean-project evidence |
| Unity Test Framework | Verify at implementation | Not run | Test assemblies only |
| Input System haptics provider | Planned Unity 6 released version | Not run | Separate provider package, not core dependency |
| Peer bridges | Per bridge specification | Not run | Core remains independent |

### 21.2 Semantic versioning policy

Patch changes may:

- fix behavior without changing public contracts;
- improve diagnostics or validation;
- add nonbreaking provider metadata;
- correct documentation or samples.

Minor changes may:

- add optional channels, result fields with safe defaults, provider capabilities, setup operations, or authoring tools;
- add new recipe fields when older assets remain valid and migration is documented;
- add separate provider/bridge packages.

Major changes include:

- changing recipe or signal identity semantics;
- changing default overlap/cancellation behavior for existing assets;
- removing or renaming public types/members without compatibility path;
- changing provider registration/channel contracts incompatibly;
- changing serialized schema without migration;
- changing the package’s authority boundary.

### 21.3 Deprecation policy

- Deprecated APIs are documented with replacement and diagnostic/compiler guidance.
- Public serialized fields/types remain readable for at least the documented migration window.
- Provider channel IDs and signal aliases follow SFGSS-003.
- Removal requires a major release unless the surface was explicitly Experimental.
- No obsolete API silently changes authority or cancellation behavior.

### 21.4 GUID and asset compatibility

Public scripts, template assets, prefabs, sample assets, asmdefs, and setup outputs preserve committed `.meta` files. Moves and renames retain GUIDs when identity should survive. Domain stable IDs remain independent from Unity GUIDs.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview, ownership, and non-goals.
- Installation routes and exact dependency visibility.
- Five-minute quick start.
- Root/configuration setup.
- Recipe and signal authoring.
- Timeline offsets, requiredness, overlap, priority, capacity, and cancellation.
- Channel scales and accessibility.
- Standalone time-provider selection and conflict warnings.
- Impact Laboratory guide.
- Diagnostics and troubleshooting.
- Provider/bridge installation index.
- Upgrade/migration and removal.
- Known limitations and unsupported channels/platforms.
- License, credits, and third-party notices.

### 22.2 Required developer documentation

- Authority and lifecycle model.
- Provider interface and registration rules.
- Unscaled clock and cancellation model.
- Stable ID and data model.
- Time-authority integration guidance.
- Scale/safety composition.
- Diagnostics and privacy model.
- Testing strategy and evidence registry.
- Release workflow.
- ADRs, current checkpoint/status, and linked Current Notes.

### 22.3 Documentation truth rule

- Examples must compile against the documented version once implementation exists.
- Planned tests remain `Not run` until executed.
- Platform and haptics support remain Planned/Unknown until exact evidence exists.
- Simulated Laboratory providers are labeled simulation, not production support.
- Menu paths, screenshots, code, and setup receipts must match the released build.
- Measured performance numbers cannot be inserted before measurement.

### 22.4 Living repository and Obsidian workflow

Documentation lives in Git beside implementation. Obsidian opens the same Markdown files directly.

At each meaningful checkpoint:

1. Review Current Notes.
2. Promote durable decisions into this specification or an ADR.
3. Move defects and evidence into permanent test/issue records.
4. Update guides and changelog for behavior/setup changes.
5. Update checkpoint status and next action.
6. Condense promoted notes and rely on Git history.
7. Commit documentation with or immediately adjacent to implementation when implementation begins.

### 22.5 Repository scan and handoff order

1. Repository README.
2. SFGSS-000.
3. SFGSS-002, SFGSS-003, and SFGSS-004.
4. This EchoFeedback specification.
5. Applicable provider/bridge specifications and ADRs.
6. Current Notes.
7. Current checkpoint, tests, issue log, and changelog.
8. Relevant implementation and automated tests.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, recipe validation, scale composition, overlap/admission policies | Invalid offsets, duplicate IDs, clamps, deterministic ordering | Yes |
| PlayMode unit/integration | Root lifecycle, runner, providers, cancellation, standalone time | Duplicate root, time release at zero scale, provider timeout | Yes |
| Standalone Laboratory | Isolated user-visible core loop | Multi-channel recipes, suppression, spam, focus, reset | Yes |
| Provider/Bridge Integration Laboratory | One explicit integration | Jukebot, Pulse, EchoUI, Input System, EchoCamera | When artifact ships |
| Showcase | Combined game-feel presentation | Several integrations together | No |
| Clean-project install | Packaging and missing-dependency proof | Git/local/tarball/import/remove | Yes |
| Existing-project migration | Incremental adoption | Rescuers2D and Don’t Get Vince’d | Before integration claim |
| Performance/stress | Bounded capacity and allocations | Rapid-hit floods and long sessions | Yes for stable release |

### 23.2 Required test categories

- Clean installation and compilation.
- Duplicate authority before and during scene changes.
- Direct-scene initialization.
- Missing and invalid configuration.
- Empty/invalid recipes and signals.
- Stable ID duplicates and aliases after release.
- Parallel/sequential deterministic scheduling.
- Unscaled execution while time scale is zero.
- Handle generation and stale-handle rejection.
- Cancellation by handle, owner, group, channel, scene, and shutdown.
- Provider registration, collision, removal, exception, timeout, and late registration.
- Optional versus required provider behavior.
- Capacity, overlap, replacement, and stress.
- Channel scale composition, suppression, and accessibility.
- Standalone time drift, restoration, Play Mode exit, and authority conflict.
- Haptic focus/device-loss cleanup in provider tests.
- Target destruction and invalid context.
- Domain reload disabled and static reset.
- Sample removal.
- Bridge install/removal/reinstall.
- Documentation/setup repeatability.
- Performance, allocations, platform, privacy, and release evidence.

### 23.3 Planned test case registry

Every status is `Not run` until execution.

| Test ID | Requirement | Setup/action summary | Expected result | Automation | Status |
|---|---|---|---|---|---|
| `EFB-T-INST-001` | Clean install | Install core in clean Unity project | Zero compile errors | Manual/CI later | Not run |
| `EFB-T-INST-002` | Local path | Install through local UPM path | Package resolves and compiles | Manual | Not run |
| `EFB-T-INST-003` | Tarball | Install distribution tarball | Package resolves and compiles | Manual/CI later | Not run |
| `EFB-T-INST-004` | Embedded | Embed package for development | Assemblies compile | Manual | Not run |
| `EFB-T-INST-005` | Remove | Remove core with no dependent bridge | Project compiles; project assets remain | Manual | Not run |
| `EFB-T-INST-006` | Reinstall | Reinstall compatible version | Existing assets validate | Manual | Not run |
| `EFB-T-INST-007` | No Editor refs | Build runtime assembly | No `UnityEditor` reference | Automated | Not run |
| `EFB-T-INST-008` | Delete samples | Remove `Samples~` import | Runtime/Editor compile | Automated/manual | Not run |
| `EFB-T-LIFE-001` | Initialize | Valid configuration | Ready once | PlayMode | Not run |
| `EFB-T-LIFE-002` | Idempotent init | Initialize twice | Same authority/result, no duplicate services | PlayMode | Not run |
| `EFB-T-LIFE-003` | No First Light | Standalone initialize | Ready | PlayMode | Not run |
| `EFB-T-LIFE-004` | Duplicate root | Two roots in scene | Duplicate no side effects | PlayMode | Not run |
| `EFB-T-LIFE-005` | Scene persistence | Change scene with persistent root | One authority remains | PlayMode | Not run |
| `EFB-T-LIFE-006` | Direct scene | Enter Laboratory directly | Development root once | Manual/PlayMode | Not run |
| `EFB-T-LIFE-007` | Shutdown | Shutdown with active work | Cleanup/restoration and terminal results | PlayMode | Not run |
| `EFB-T-LIFE-008` | Play Mode exit | Exit during time/haptic effect | Best-effort external reset | Manual | Not run |
| `EFB-T-CFG-001` | Missing config | Start root without config | Failed with `EFB-CFG-001` | PlayMode | Not run |
| `EFB-T-CFG-002` | Invalid limits | Zero/negative/unsafe limits | Validation blocks or repair planned | EditMode | Not run |
| `EFB-T-ID-001` | Duplicate recipe ID | Two recipes same ID | Validation error | EditMode | Not run |
| `EFB-T-ID-002` | Duplicate signal ID | Two signals same ID | Validation error | EditMode | Not run |
| `EFB-T-ID-003` | Duplicate step ID | One recipe duplicates step | Validation error | EditMode | Not run |
| `EFB-T-RCP-001` | Empty recipe | Play recipe with no steps | Declared empty behavior/result | EditMode/PlayMode | Not run |
| `EFB-T-RCP-002` | Invalid offset | Negative start offset | Rejected/validation error | EditMode | Not run |
| `EFB-T-RCP-003` | Deterministic equal offsets | Several equal-offset steps | Serialized start order | PlayMode | Not run |
| `EFB-T-RCP-004` | Sequential offsets | Increasing offsets | Unscaled timeline order | PlayMode | Not run |
| `EFB-T-RCP-005` | Parallel steps | Equal offset providers | All admitted same frame/order | PlayMode | Not run |
| `EFB-T-RCP-006` | Time scale zero | Run recipe while scaled time zero | Timeline still completes | PlayMode | Not run |
| `EFB-T-RCP-007` | Recipe duration cap | Exceed max | Validation/request rejected | EditMode/PlayMode | Not run |
| `EFB-T-HND-001` | Handle query | Play then query | Correct active snapshot | PlayMode | Not run |
| `EFB-T-HND-002` | Cancel handle | Cancel active | Terminal cancelled result | PlayMode | Not run |
| `EFB-T-HND-003` | Stale generation | Reuse slot then old cancel | New instance unaffected | PlayMode | Not run |
| `EFB-T-HND-004` | Terminal expiry | Evict terminal history | Handle reports Expired | PlayMode | Not run |
| `EFB-T-CAN-001` | Cancel owner | Multiple owners | Only matching work cancelled | PlayMode | Not run |
| `EFB-T-CAN-002` | Cancel group | Multiple groups | Only group cancelled | PlayMode | Not run |
| `EFB-T-CAN-003` | Cancel channel | Mixed channels | Matching provider steps cancelled | PlayMode | Not run |
| `EFB-T-CAN-004` | Repeat cancel | Cancel twice | Idempotent terminal result | PlayMode | Not run |
| `EFB-T-CAN-005` | Unsupported cancel | Provider cannot cancel immediately | Structured deferred/unsupported result | PlayMode | Not run |
| `EFB-T-PROV-001` | Register provider | Valid channel | Registration succeeds | EditMode/PlayMode | Not run |
| `EFB-T-PROV-002` | Duplicate provider | Same exact channel | New registration rejected | PlayMode | Not run |
| `EFB-T-PROV-003` | Dispose provider | Dispose registration | New requests unavailable | PlayMode | Not run |
| `EFB-T-PROV-004` | Provider exception | Throw during execute | Isolated failure and cleanup | PlayMode | Not run |
| `EFB-T-PROV-005` | Provider timeout | Never complete | Timeout, cancellation, terminal recipe | PlayMode | Not run |
| `EFB-T-PROV-006` | Late provider | Register after initialization | New requests use provider | PlayMode | Not run |
| `EFB-T-PROV-007` | Missing optional | Optional channel absent | Skip/advisory, recipe continues | PlayMode | Not run |
| `EFB-T-PROV-008` | Missing required | Required channel absent | Recipe fails by policy | PlayMode | Not run |
| `EFB-T-PROV-009` | Remove while active | Dispose active provider | Declared finish/cancel behavior | PlayMode | Not run |
| `EFB-T-PROV-010` | Listener failure | Diagnostic/event listener throws | Feedback continues | PlayMode | Not run |
| `EFB-T-ADM-001` | Active capacity | Fill active slots | Further request rejected/replaced | PlayMode | Not run |
| `EFB-T-ADM-002` | Step capacity | Recipe exceeds active-step budget | Deterministic admission result | PlayMode | Not run |
| `EFB-T-ADM-003` | Stack | Same group Stack | Both active within cap | PlayMode | Not run |
| `EFB-T-ADM-004` | Replace | Same group Replace | Old cancelled, new admitted | PlayMode | Not run |
| `EFB-T-ADM-005` | Ignore | Same group Ignore | New rejected | PlayMode | Not run |
| `EFB-T-ADM-006` | Priority replacement | Capacity with priorities | Deterministic candidate/result | PlayMode | Not run |
| `EFB-T-SCALE-001` | Defaults | No Accord/source | Project defaults effective | EditMode/PlayMode | Not run |
| `EFB-T-SCALE-002` | Multiplication | Recipe/request/project/accessibility scales | Correct bounded value | EditMode | Not run |
| `EFB-T-SCALE-003` | Suppression | Channel disabled | Provider not invoked/effect suppressed | PlayMode | Not run |
| `EFB-T-SCALE-004` | Invalid scale | NaN/out of range | Rejected or clamped by policy | EditMode | Not run |
| `EFB-T-SCALE-005` | Atomic update | Apply new scale set | Whole snapshot changes once | PlayMode | Not run |
| `EFB-T-SCALE-006` | Live update policy | Scale changes during active effect | Active/new behavior matches declaration | PlayMode | Not run |
| `EFB-T-TIME-001` | Standalone hit stop | Time provider enabled | Multiplier applies/releases unscaled | PlayMode | Not run |
| `EFB-T-TIME-002` | Cancel time effect | Cancel during hold | Exact owned state restores | PlayMode | Not run |
| `EFB-T-TIME-003` | Overlap time effects | Multiple groups/priorities | Deterministic effective multiplier | PlayMode | Not run |
| `EFB-T-TIME-004` | External drift | Mutate time externally | Drift policy and code | PlayMode | Not run |
| `EFB-T-TIME-005` | Provider conflict | Standalone plus external bridge plan | Validation blocker | EditMode | Not run |
| `EFB-T-TIME-006` | Fixed timestep policy | Preserve/scale selected | Exact documented behavior/restoration | PlayMode | Not run |
| `EFB-T-CTX-001` | Spatial context | Position/direction supplied | Provider receives detached values | PlayMode | Not run |
| `EFB-T-CTX-002` | Target destroyed | Destroy target mid-step | Safe settle by policy | PlayMode | Not run |
| `EFB-T-CTX-003` | Audience unsupported | Provider cannot resolve audience | Structured unsupported result | PlayMode | Not run |
| `EFB-T-FOCUS-001` | Focus loss | Simulate/trigger focus loss | Configured cancellation/reset | Manual/PlayMode | Not run |
| `EFB-T-FOCUS-002` | Focus regain | Regain focus | No stale haptics/time/visual state | Manual | Not run |
| `EFB-T-DIAG-001` | Snapshot | Capture status under load | Bounded detached state | PlayMode | Not run |
| `EFB-T-DIAG-002` | History bound | Exceed event capacity | Oldest evicted, memory bounded | PlayMode | Not run |
| `EFB-T-DIAG-003` | Privacy | Export snapshot | No arbitrary context/device/private paths | EditMode/manual | Not run |
| `EFB-T-EDIT-001` | Setup repeat | Run setup twice | No duplicates/overwrites | Editor/manual | Not run |
| `EFB-T-EDIT-002` | Repair preview | Missing refs | Exact non-destructive plan | Editor | Not run |
| `EFB-T-EDIT-003` | Recipe preview | Simulated providers | Asset unchanged after preview | Editor | Not run |
| `EFB-T-EDIT-004` | ADR-001 facade | Workshop-compatible plan/apply/validate | Stable receipt and result | Editor | Not run |
| `EFB-T-DATA-001` | Asset immutability | Stress Play Mode | Definitions unchanged | EditMode/PlayMode | Not run |
| `EFB-T-DATA-002` | Rename display | Change display name only | Stable ID unchanged | EditMode | Not run |
| `EFB-T-DATA-003` | Alias migration | Released ID alias | Old reference resolves/report | EditMode | Not run |
| `EFB-T-REM-001` | Remove optional bridge | Remove bridge package | Core compiles; recipes remain | Manual/CI later | Not run |
| `EFB-T-REM-002` | Remove provider | Remove haptics provider | Core compiles; channel unavailable | Manual | Not run |
| `EFB-T-REM-003` | Reinstall provider | Reinstall compatible provider | Mappings validate/reconnect | Manual | Not run |
| `EFB-T-REM-004` | Remove core safely | No dependent bridge | Generated/project assets handled by removal guide | Manual | Not run |
| `EFB-T-PERF-001` | Idle allocations | Ready with no work | Meets measured budget | Profiler | Not run |
| `EFB-T-PERF-002` | Stress requests | Sustained rapid recipes | Bounded counts and acceptable frame cost | Profiler/Lab | Not run |
| `EFB-T-PERF-003` | Long session | Repeated completion/history eviction | No growth/leak | Profiler | Not run |
| `EFB-T-PLAT-001` | Windows build | Build/run minimum workflow | Planned behavior verified | Manual/CI later | Not run |
| `EFB-T-PLAT-002` | macOS build | Build/run minimum workflow | Planned behavior verified | Manual/CI later | Not run |
| `EFB-T-PLAT-003` | Linux build | Build/run minimum workflow | Planned behavior verified | Manual/CI later | Not run |
| `EFB-T-PLAT-004` | WebGL build | Build/run supported core workflow | Provider limitations explicit | Manual | Not run |
| `EFB-T-PLAT-005` | Mobile focus/suspend | Run on device | Safe cleanup and provider behavior | Manual | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and non-ownership approved.
- [x] MVP and deferred scope separated.
- [x] Core and provider dependencies explicit.
- [x] Recipe, signal, runtime, handle, provider, scale, and time models defined.
- [x] Standalone Laboratory designed.
- [x] Planned tests registered and marked Not run.
- [x] No release-blocking architecture question remains.

### 24.2 Implementation gate

- [ ] Runtime code compiles with declared dependencies only.
- [ ] Editor code is isolated from runtime.
- [ ] Setup facade follows ADR-001.
- [ ] Duplicate root rejects before side effects.
- [ ] Recipes remain immutable in Play Mode.
- [ ] Provider timeout/cancellation and external-state restoration are implemented.
- [ ] Public API matches specification or authority is revised first.

### 24.3 Standalone gate

- [ ] Clean-project install succeeds.
- [ ] Core works without peer packages.
- [ ] Impact Laboratory passes required cases.
- [ ] Direct-scene entry behaves as documented.
- [ ] Samples can be removed safely.
- [ ] Standalone time provider conflict/restoration tests pass.

### 24.4 Beta gate

- [ ] MVP automated tests pass except explicitly waived nonblocking cases.
- [ ] Laboratory happy path, missing provider, cancellation, scaling, capacity, and reset pass.
- [ ] No Blocker or Critical defect remains.
- [ ] Known limitations and provider availability are documented.
- [ ] Clean local/tarball installation evidence exists.
- [ ] Diagnostics and setup guidance are usable.

### 24.5 Release-candidate gate

- [ ] Full required automated registry passes.
- [ ] Stress, focus, domain reload, provider failure, removal/reinstall, and accessibility tests pass.
- [ ] Measured performance budgets are established and pass.
- [ ] First real-project integration passes without removing old implementation prematurely.
- [ ] Documentation examples compile and screenshots/menu paths are current.
- [ ] Licenses and notices are complete.

### 24.6 Stable distribution gate

- [ ] Exact supported Unity/platform matrix has evidence.
- [ ] Git and tarball installs pass in external clean projects.
- [ ] Upgrade from previous supported version passes when applicable.
- [ ] All advertised provider/bridge artifacts have their own Integration Laboratory evidence.
- [ ] No required flaky or quarantined test is counted as pass.
- [ ] Package manifest, SemVer, changelog, tag, release, and compatibility catalog are prepared.
- [ ] Current Notes and documentation are reconciled and pushed.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Rescuers2D | Direct character/destruction/UI/audio feedback calls and project-specific camera/time effects | Install core and Laboratory; map one semantic event/channel at a time | Existing effect parity plus cancellation/accessibility | Keep old listeners disabled behind project toggle until parity |
| Don’t Get Vince’d | Combat hit, combo, boss, pickup, invincibility, and victory feedback listeners | Begin with light/heavy hit recipes, then add audio/camera/UI bridges | Combat result unchanged; presentation parity and stress pass | Restore old event listeners |
| Echo Systems Lab | Weapon/mission event feedback | Use as architecture proof and portfolio case study | Definition/runtime/presentation separation remains intact | Keep current system available |
| Hackulos | Future combat/spell/UI/environment feedback | Adopt only after combat/abilities/camera authorities exist | Recipe/provider boundaries fit RPG foundation | Project-local adapters remain possible |

### 25.2 Preserve-until-parity rule

1. Keep original project feedback active and source-controlled.
2. Validate EchoFeedback in its Standalone Laboratory.
3. Add one project semantic event and one provider.
4. Compare timing, intensity, accessibility, cancellation, and performance.
5. Migrate remaining channels incrementally.
6. Remove old code only after parity evidence and rollback commit exist.

### 25.3 Migration tooling

The package may provide later detection/report helpers for common direct calls such as project-owned shake/time/rumble methods, but it must not automatically rewrite gameplay code. Migration tooling focuses on:

- asset/configuration conversion where a known schema exists;
- preview and backup;
- stable signal ID creation;
- provider mapping assistance;
- validation and parity checklists;
- removal receipts.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EFB-R-001 | Scope inflates into tween/VFX/cinematic framework | High | High | Flat semantic timeline and strict non-goals | Any branching/arbitrary-action proposal |
| EFB-R-002 | Core bypasses camera/audio/UI/input authorities | Medium | High | Provider-only execution and separate bridges | Direct peer type/reference proposal |
| EFB-R-003 | Standalone time provider conflicts with Pulse/project time | High | Critical | Explicit exclusive modes, validator blocker, drift diagnostics | More than one final writer planned |
| EFB-R-004 | Hit stop prevents its own release | Medium | Critical | Injected unscaled clock and unscaled cancellation | Any scaled delay in core |
| EFB-R-005 | Haptics remain active after focus/device loss | Medium | High | Provider reset hooks and device/focus tests | Provider cannot prove reset |
| EFB-R-006 | Excess stacking creates inaccessible/extreme effects | High | High | Hard capacities, scales, suppression, safety caps, overlap policy | Stress/advisory threshold |
| EFB-R-007 | Generic signal contract becomes an untyped data bag | Medium | High | Small normalized common request, provider-owned mappings, no arbitrary dictionary | New payload fields proposed |
| EFB-R-008 | Provider failure stalls recipe forever | Medium | High | Timeout, cancellation token, isolation, terminal result | Provider exceeds timeout |
| EFB-R-009 | Recipe assets accumulate runtime state | Medium | High | Immutable assets and contamination tests | Asset changes after Play Mode |
| EFB-R-010 | Stale handles cancel reused instances | Medium | High | Generational slots and tests | Handle registry design change |
| EFB-R-011 | Laboratory simulation is mistaken for production support | Medium | Medium | Labels, separate assemblies, provider evidence rules | Documentation/release review |
| EFB-R-012 | Platform/device haptics are overclaimed | High | Medium | Planned/Unknown labels and exact provider matrices | Any blanket “supported” claim |
| EFB-R-013 | Diagnostic trace allocates/spams | Medium | Medium | Bounded buffers and opt-in detailed tracing | Performance tests |
| EFB-R-014 | Accessibility scales double-apply with Jukebot/UI | Medium | Medium | Bridge contracts define each scale owner and avoid master-volume duplication | Integration spec review |
| EFB-R-015 | Provider package removal leaves broken recipes | Medium | Medium | Semantic IDs preserved, missing-provider policy, bridge-first removal | Removal tests |
| EFB-R-016 | Unity API/package drift affects Awaitable or haptics | Medium | Medium | Exact version verification during implementation/release | Unity upgrade |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EFB-D-001 | EchoFeedback owns coordination, not channel authorities | Approved | Preserves suite ownership matrix | All production effects use providers/bridges | No |
| EFB-D-002 | Recipes are immutable flat timelines | Approved | Chained/parallel response without general scripting | No nesting/branching in MVP | No |
| EFB-D-003 | Recipes use semantic channel and signal IDs | Approved | Keeps peer/provider types out of core | Provider mapping assets required | No |
| EFB-D-004 | Scheduling uses an injected unscaled clock | Approved | Hit stop/cancellation must continue at zero scale | Testable and deterministic timing seam | No |
| EFB-D-005 | Provider execution returns fresh `Awaitable` results | Approved | Unity 6 async baseline and provider timeout/cancellation | Providers follow async contract | No |
| EFB-D-006 | One provider per exact channel ID in MVP | Approved | Simple deterministic ownership | Composite/multiple providers require distinct IDs | No |
| EFB-D-007 | Standalone Unity time provider is opt-in and exclusive | Approved | Core remains useful alone without creating silent second authority | Validator blocks Pulse conflict | No |
| EFB-D-008 | Pulse bridge owns integrated final time composition | Approved | Pulse retains pause/base time authority | Separate bridge package | Yes when bridge is specified |
| EFB-D-009 | Input System haptics ship as separate provider artifact | Approved | Core avoids device/input dependency and ownership | Provider specification/evidence required | No |
| EFB-D-010 | Accessibility scales are applied before provider calls | Approved | Uniform suppression and safety boundary | Providers receive effective values | No |
| EFB-D-011 | Active feedback is never save state | Approved | Feedback is transient presentation | Load/transition code cancels as needed | No |
| EFB-D-012 | Laboratory uses simulated providers | Approved | Standalone visible proof without peer dependencies | Simulation never counts as bridge support | No |
| EFB-D-013 | Diagnostic prefix is `EFB-*` | Approved | Unique searchable package namespace | Reserved in later naming registry | No |

### 27.2 Release-blocking questions

None remain for specification approval.

Before implementation, checkpoint planning must still verify:

- exact Unity 6000.x API behavior and package versions;
- whether the standalone time provider’s fixed-timestep default should be PreserveBaseline or ScaleWithTime after prototype evidence;
- exact internal pooling/data-structure budgets from measured tests;
- final package naming/versioning for separate bridge/provider artifacts.

These are implementation or integration-specification questions, not authority blockers.

### 27.3 Non-blocking later questions

- Should nested recipes ever be added, or should composition remain project-level?
- Should rendering/post-processing providers share one contract or remain backend-specific?
- Should live rescaling of active effects be a provider capability in v1 or later?
- Which XR haptic providers are worthwhile?
- Should a network presentation adapter deduplicate predicted and confirmed feedback events?

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved package contract | Design only | This approved document |
| M1 - Skeleton | Installable package anatomy | Manifest, assemblies, docs shell | Clean compile/install |
| M2 - Data and runtime core | Recipes, signals, requests, runner, handles, scales | Core non-provider behavior | Automated unit/PlayMode tests |
| M3 - Provider contracts and standalone time | Registry, provider async/cancel, time provider | Provider and restoration behavior | PlayMode tests |
| M4 - Impact Laboratory | Isolated visible proof | Simulated channels, controls, diagnostics, stress | Laboratory checklist |
| M5 - Editor tooling | Setup facade, validation, preview, repair | Repeatable authoring | Editor tests |
| M6 - First provider/bridges | Input System haptics and selected Foundation bridges | Explicit optional integrations | Integration Laboratories |
| M7 - Real-project adoption | One target project migration | Incremental parity | Parity/rollback report |
| M8 - Release | Distribution-ready package | Docs, tests, licenses, artifacts | Clean external installs and release gates |

### 28.2 Checkpoint rule

Every implementation milestone uses SFGSS-005. Code remains locked until SUITE-DOC-33. When authorized, each checkpoint must show complete compile-ready files in the conversation, explain each file and design choice, provide exact Editor setup and tests, and stop at a proof boundary so Jesse can enter and understand the code.

### 28.3 First recommended implementation checkpoint

After the full-suite documentation gate:

> **EFB-M1-01 - EchoFeedback Package Skeleton**

This checkpoint creates package metadata, asmdefs, documentation shell, empty test assemblies, and installation evidence only. It does not create runtime C# behavior, recipes, providers, scenes, or assets.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk’s Forge.

Treat SFGSS-000 as suite authority, SFGSS-002 as dependency/assembly authority,
SFGSS-003 as data/identity/migration authority, SFGSS-004 as test/evidence authority,
and the approved Impact - EchoFeedback specification as the Level 2 authority for
coordinated feedback behavior.

Current package: EchoFeedback
Current specification: v1.0.0 Approved
Current documentation checkpoint: <CHECKPOINT>
Current implementation checkpoint: Locked until SUITE-DOC-33
Unity baseline: 6000.3.8f1
Known blockers: <BLOCKERS>

Before proposing implementation:
1. Preserve gameplay, camera, audio, UI, input, settings, game-state, and save authorities.
2. Keep recipes semantic and immutable.
3. Keep production channel execution behind explicit providers or bridges.
4. Use unscaled scheduling and preserve one final time authority.
5. Mark unexecuted evidence Not run.
6. When code is authorized, show complete files and explain every step so Jesse can enter them manually.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package specification | 1.0.0 Approved |
| Completed checkpoint | SUITE-DOC-05 - EchoFeedback package specification |
| Files/assets created | Documentation only |
| Tests passed | Documentation structure and consistency audit only; runtime tests Not run |
| Tests failed | None executed |
| Known issues | Exact Unity/provider compatibility and performance remain evidence-pending |
| Decisions added | EFB-D-001 through EFB-D-013 |
| Next documentation checkpoint | SUITE-DOC-06 - EchoPool: The Wellspring specification |
| Implementation status | Not started; locked until SUITE-DOC-33 |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Package identity and responsibility are clear.
- [x] Ownership and non-ownership align with SFGSS-000.
- [x] Dependencies and bridge directions align with SFGSS-002.
- [x] Definitions, stable IDs, runtime state, and migration align with SFGSS-003.
- [x] Planned tests, Laboratories, evidence labels, and release gates align with SFGSS-004.
- [x] Core independence is credible.
- [x] MVP is useful without becoming a general effects framework.
- [x] Recipe, timeline, provider, scale, cancellation, capacity, and diagnostics models are defined.
- [x] Time authority conflict is explicitly prevented.
- [x] Accessibility and channel suppression are package-level requirements.
- [x] Standalone Laboratory is fully designed.
- [x] Optional integrations are removable and explicit.
- [x] No Isekai Studios identity or ownership is introduced.
- [x] Runtime evidence remains honestly Not run.
- [x] Jesse’s package-first documentation gate remains intact.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Package implementation remains prohibited until SUITE-DOC-33. Provider and bridge artifacts require their own explicit contracts and evidence before being advertised as supported.

---

## Specification Completion Record

A new collaborator can answer:

1. EchoFeedback owns coordinated transient feedback recipes and execution.
2. It refuses gameplay, camera, audio, UI, input, settings, save, and pause authority.
3. The MVP is a flat semantic timeline with provider execution, scaling, cancellation, diagnostics, and a Laboratory.
4. It works alone through simulated providers and an opt-in standalone time provider.
5. Recipes/signals are immutable definitions; instances, handles, scales, providers, and histories are runtime state.
6. The public API uses structured requests/results, generational handles, explicit providers, and fresh awaitables.
7. Missing providers, capacity, time drift, focus loss, target loss, timeouts, and shutdown have defined behavior.
8. Setup, validation, preview, and the Impact Laboratory are specified.
9. Optional systems connect through separate bridges/providers.
10. Release requires executed clean-install, lifecycle, stress, accessibility, provider, performance, platform, removal, and documentation evidence.

The specification is therefore complete and **Approved** as a pre-code package foundation.

---

## Appendix A - Unity implementation basis to verify during implementation

The implementation plan is grounded in these Unity 6 concepts, which must be reverified against the exact supported Editor/package versions when coding begins:

- `Time.timeScale` controls scaled game time, while `Time.unscaledTime` and `Time.unscaledDeltaTime` remain independent. EchoFeedback therefore schedules recipe lifecycle on an unscaled clock.
- Unity `Awaitable` is the approved Unity 6 async primitive, and pooled awaitable instances must not be awaited more than once. Every public/provider operation returns a fresh instance.
- The Input System exposes gamepad haptics through normalized dual-motor speeds and reset/pause/resume operations. EchoFeedback keeps this in a separate provider so unsupported devices and project device ownership remain explicit.
- Application focus callbacks may vary by platform and Editor context. Focus-loss cleanup is tested as a provider safety behavior rather than assumed from one call pattern.


---

## Graph Navigation

#sfgss/package #sfgss/wave/expansion #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
