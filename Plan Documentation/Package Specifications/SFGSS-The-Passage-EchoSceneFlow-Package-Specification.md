# The Passage – Scene Flow Package Specification

**Working document ID:** SFGSS-PKG-ECHOSCENEFLOW-001  
**Specification version:** 1.1.0
**Status:** Approved  
**Technical package name:** EchoSceneFlow  
**Public title:** The Passage – Scene Flow
**Package ID:** `com.echodevgames.echo-scene-flow`  
**Runtime namespace:** `EchoDevGames.EchoSceneFlow`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoSceneFlow`
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum public Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 and SFGSS-001  
**Last updated:** August 4, 2026

> “Open one known path, cross it safely, and leave the world in a state the next scene can trust.”

> **Approval rule:** This specification is approved as the authoritative package design. Runtime implementation remains intentionally deferred until the complete Foundation Wave specification pass and its cross-package consistency review are finished.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-03 | Proposed | Initial complete specification derived from SFGSS-000 v0.6.0, SFGSS-001 v1.1.0, First Light v1.0.0, The Observatory v1.0.0, and The Accord v1.0.0 | Pending |
| 1.0.0 | 2026-08-03 | Approved | Approved normal scene-travel authority, stable scene/route data, serialized transition pipeline, async backend, locking/queueing, progress, activation, recovery, direct-scene behavior, Test Lab, and bridge boundaries | Jesse “Echo” Adams |
| 1.1.0 | 2026-08-04 | Approved | Confirmed `SceneId` as the durable runtime identity, separate from Editor source GUID/path metadata. Also normalized registry metadata and evidence interpretation. | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Passage – Scene Flow
**Technical identifier:** EchoSceneFlow  
**Flavor line:** Open one known path, cross it safely, and leave the world in a state the next scene can trust.  
**Plain-language subtitle:** Validated asynchronous scene travel, transition lifecycle, progress, locking, recovery, and optional presentation hooks.

**One-sentence ownership contract:**

> EchoSceneFlow owns normal runtime scene-transition requests, validation, serialization, loading lifecycle, progress, activation, transition locking or bounded queuing, route helpers, recovery results, and scene-flow diagnostics after First Light handoff; it does not own startup orchestration, game-state rules, menu or loading-screen presentation, save policy, audio playback, gameplay completion, multiplayer authority, or scene content.

### 1.1 Elevator summary

The Passage provides one safe authority for ordinary scene travel after startup. A caller requests a project-defined route or destination. EchoSceneFlow validates that destination, decides whether the request can start or enter the configured queue, runs the transition through one explicit lifecycle, reports progress, invokes registered lifecycle participants, delegates visual presentation through an optional presenter contract, and returns a structured result.

The package replaces scattered calls to `SceneManager.LoadScene`, hard-coded scene strings, one-off fade coroutines, repeated button locks, and scene-specific transition managers with one auditable pipeline. Its runtime core uses a replaceable scene-operation backend. The MVP backend wraps Unity’s built-in `SceneManager` asynchronous loading without exposing `AsyncOperation` as the package’s public contract.

Project-owned `SceneDefinition`, `SceneRouteDefinition`, `SceneTransitionProfile`, and `SceneCatalog` assets separate stable project intent from mutable transition state. Stable IDs survive display-name and asset-path changes. Editor tooling uses a SceneAsset picker to record the source asset GUID and synchronized runtime path, then validates that path against the active Unity Build Profile before a build or Play Mode session. The runtime assembly never references `UnityEditor.SceneAsset`.

### 1.2 Why this belongs in The Sperk’s Forge

Rescuers2D and Echo Systems Lab both required reusable scene-loading services, return-to-menu or hub behavior, persistent services, and protection against repeated button presses. Those implementations proved the need but also carried project-specific destination names, bootstrap assumptions, save knowledge, and presentation coupling. First Light now owns initial startup and handoff, making a separate post-launch travel authority necessary.

The Passage turns that recurring infrastructure into a standalone package while keeping the game in control of why travel happens. A victory controller, objective system, menu presenter, password screen, respawn system, or multiplayer adapter may request a route. EchoSceneFlow owns whether and how that route crosses scenes safely. It never decides that the player won, which level unlocks next, what music should play, or whether a save should occur.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | “The Passage” must be paired with “Scene Flow” on formal surfaces. |
| Setup guidance/tooltips | Yes | Flavor may introduce a route or transition, but actions and failures remain technically explicit. |
| Samples | Optional | Passage-themed sample labels and visuals must be replaceable and removable. |
| Runtime API/type names | No lore-only names | Types describe scenes, routes, transitions, phases, progress, participants, presenters, results, and recovery. |
| Project data | No required Hackulos content | The game owns scenes, route names, destination roles, art, loading copy, and transition style. |

---

## 2. Problem Statement

### 2.1 Current problem

Scene travel is often implemented directly from UI buttons or gameplay scripts with a scene name string and a call to Unity’s scene API. That shortcut creates recurring failures:

1. Double-clicks or repeated completion events start competing transitions.
2. Scene names, paths, and build inclusion drift without validation.
3. A loading screen, fade, input lock, audio change, state change, and save request become tangled in one scene-specific coroutine.
4. Each scene creates its own loader or persistent singleton.
5. Additive and persistent scenes are unloaded by objects that do not own them.
6. Progress is reported inconsistently or treats Unity’s pre-activation `0.9` value as completion.
7. Cancellation is promised even after Unity has started an operation that cannot be safely aborted.
8. A failed transition can leave the screen black, the input locked, or the active request permanently busy.
9. Return-to-menu and reload logic uses hidden scene strings in multiple scripts.
10. Direct-scene Play Mode testing creates duplicate persistent authorities or bypasses required setup silently.
11. Save, game-state, UI, and audio systems begin owning pieces of scene travel because no clear transition contract exists.
12. Support logs identify only “scene load failed” without the accepted route, phase, source, destination, queue state, or recovery attempt.

### 2.2 Evidence from existing work

| Source project/package | Existing pattern or requirement | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | `SceneLoadService`, menu travel, persistent systems, return behavior, and bootstrap conflict lessons | Centralized travel and persistent authority | Remove hard-coded destinations, project save knowledge, duplicate roots, and UI ownership |
| Echo Systems Lab | `GameSceneLoader`, Hub-to-Trial flow, mission progress, and event-driven architecture | Clear service boundary and semantic requests | Add stable route data, validation, structured results, queue policy, and failure recovery |
| Don’t Get Vince’d | Scene and results flow in a jam-style project | Fast project integration | Avoid game-specific static managers and scattered direct loads |
| First Light | Initial destination selection and optional final transition bridge | Explicit startup handoff and launch report | Keep startup-only fallback loading separate from normal travel authority |
| The Observatory | Explicit providers, bounded status, stable diagnostics, privacy, and optional bridges | Structured status without mandatory coupling | Publish scene-flow state through a separate bridge |
| The Accord | Explicit authority, transactional state, late registration, and Unity `Awaitable<T>` baseline | Deterministic public async/result contracts | Keep global settings and scene travel independent |
| Future EchoGameState | Loading state, pause, cursor, and input coordination | One high-level state authority | Scene flow requests coordination; it does not change time/input directly |
| Future EchoUI | Loading, fade, and transition presentation | Replaceable visual presentation | UI must not become the only way to request or complete travel |
| Future EchoSave | Save participants and slot state | Optional pre-travel save/flush adapter | Scene flow does not own when or what to save |

### 2.3 Consequences of doing nothing

- Every project rebuilds another fade-and-load coroutine.
- Scene references fail only after entering Play Mode or building.
- Competing transitions cause black screens, skipped results, or incorrect destinations.
- Packages cannot agree where startup handoff ends and normal travel begins.
- UI, audio, game state, and save systems become circularly dependent.
- Direct-scene testing hides lifecycle defects until late integration.
- Additive-scene work begins without ownership rules.
- Support reports cannot reconstruct a failed transition.
- Existing games remain difficult to migrate because transition rules are embedded in buttons and level scripts.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Own normal scene-transition execution after First Light handoff.
- Provide stable, project-owned scene and route definitions instead of scattered runtime strings.
- Validate destinations, active Build Profile inclusion, duplicate IDs, route loops, fallbacks, and configuration before travel.
- Serialize all scene operations through one duplicate-safe application-session authority.
- Provide deterministic transition phases and structured progress.
- Support asynchronous single-scene loading as the production-default MVP path.
- Prevent repeated requests through duplicate coalescing, rejection, or a bounded configurable queue.
- Provide cooperative cancellation during safe pre-load phases and state clearly when cancellation is no longer possible.
- Support optional transition participants for guarded pre-load preparation and post-activation finalization.
- Support optional presentation through a neutral presenter contract without requiring EchoUI or uGUI in the runtime core.
- Provide reload, configured Main Menu, configured Hub, and arbitrary route helpers without hard-coded scene names.
- Provide one-attempt fallback recovery with loop protection.
- Expose current source, destination, route, phase, progress, queue, timing, and last result for standalone diagnostics.
- Remain usable without EchoLaunch, EchoDiagnostics, EchoSettings, EchoGameState, Jukebot, EchoInput, EchoUI, EchoSave, or EchoGameStarter.
- Provide safe setup, repair, Build Profile validation, simulation, and an isolated multi-scene Standalone Test Lab.
- Establish extension seams for later additive scenes, persistent scene sets, custom providers, and multiplayer coordination without inflating the MVP.

### 3.2 Non-goals

- Decide when a level is won, lost, unlocked, selected, or replayed.
- Own initial package startup ordering or replace First Light.
- Render production loading screens, menus, fade art, tips, progress bars, or navigation.
- Pause the game, set time scale, control cursor state, or switch input contexts directly.
- Play music, ambience, SFX, or mixer snapshots.
- Save game data, select save slots, or invent autosave policy.
- Own scene-specific enemies, objectives, spawn logic, cameras, or content initialization.
- Synchronize networked clients or decide multiplayer scene authority.
- Require Addressables, a custom asset bundle system, or a proprietary scene provider in the MVP.
- Guarantee true cancellation after Unity’s scene load operation has begun.
- Start multiple Unity scene operations while a delayed activation gate is holding the operation queue.
- Use a raw scene name or path supplied by arbitrary external input as a production destination.
- Automatically discover arbitrary scene callbacks through reflection.
- Convert every gameplay scene into a persistent scene or keep every root alive.
- Replace Unity’s scene APIs, Build Profiles, Profiler, or Console.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean project with two scenes | Create catalog/config/root, add routes, open the Lab, and travel safely without code |
| Programmer | Gameplay or UI needs travel | Submit a typed route request and receive progress plus a structured result |
| Designer | Project scenes change names or paths | Update the Scene asset reference and retain stable route identity |
| UI developer | Building loading/fade presentation | Implement or bridge one presenter contract without owning scene execution |
| Save developer | Needs a pre-travel flush | Register an explicit participant or bridge without making save mandatory |
| Game-state developer | Needs Loading coordination | Observe transition lifecycle and request state through a bridge |
| Tester | Reproducing race/failure behavior | Simulate delay, validation failure, queue pressure, presenter failure, participant timeout, and fallback |
| Maintainer | Migrating direct loads | Replace one caller at a time while keeping existing scene content and rules intact |

### 3.4 Measurable success criteria

- A clean supported project installs with zero compile errors.
- The package performs an asynchronous single-scene transition with no other Sperk’s Forge package installed.
- Every runtime destination is resolved from a validated project-owned definition or route.
- Duplicate roots perform no subscriptions, registrations, loads, queue mutations, or presenter side effects.
- Two identical rapid requests produce one active transition and one shared/coalesced result.
- A different request during an active transition follows the configured reject or bounded-queue policy deterministically.
- Invalid, missing, disabled, or duplicate scene definitions are detected before the load begins.
- Progress is monotonic per request and identifies its active phase.
- Pre-load cancellation completes without starting a Unity scene load.
- Cancellation requested after the load starts returns an explicit non-cancellable-phase result rather than claiming success.
- A presenter or optional participant failure cannot permanently leave the service busy.
- A configured fallback is attempted at most once and cannot recurse into a route loop.
- Reload, Main Menu, and Hub helpers resolve project-configured routes without hidden strings.
- Idle runtime produces no recurring managed allocation after initialization.
- Samples can be removed without breaking runtime assemblies.
- Removing every optional bridge leaves EchoSceneFlow compiling and functioning.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers.
- Gameplay, UI, tools, save, audio, input, and systems programmers.
- Designers maintaining scene catalogs and route presets.
- QA testers validating travel lifecycle and failure recovery.
- Package maintainers migrating existing scene services.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| UC-001 | Initialize standalone authority | Developer/runtime | Valid configuration/root | Root claims authority and service becomes Ready | MVP |
| UC-002 | Travel by route | Gameplay/UI | Valid route and idle service | Request validates, runs phases, activates destination, and completes | MVP |
| UC-003 | Travel by scene definition | Programmer | Valid destination | Default transition profile is applied | MVP |
| UC-004 | Reload current scene | Player/gameplay | Current scene maps to catalog | Current definition is requested through normal pipeline | MVP |
| UC-005 | Return to Main Menu | UI/gameplay | Main Menu route configured | Configured route runs; no scene string exists in caller | MVP |
| UC-006 | Return to Hub | UI/gameplay | Hub route configured | Configured route runs | MVP |
| UC-007 | Reject competing request | Caller | Active transition; Reject policy | Structured Busy result; active operation unaffected | MVP |
| UC-008 | Queue competing request | Caller | Active transition; queue enabled and space available | Request enters bounded FIFO queue and starts later | MVP |
| UC-009 | Coalesce duplicate | Caller | Same operation/destination already active or queued | Existing handle/result is returned; no duplicate load | MVP |
| UC-010 | Cancel queued request | Caller | Request has not started | Request leaves queue and completes Cancelled | MVP |
| UC-011 | Cancel pre-load request | Caller | Request is validating/preparing/fading out | Pipeline unwinds presentation and completes Cancelled | MVP |
| UC-012 | Request cancellation during load | Caller | Unity backend already loading | Cancellation is recorded but operation completes or recovers; result explains unsafe phase | MVP |
| UC-013 | Observe progress | Presenter/tester | Transition active | Monotonic phase and overall progress are published | MVP |
| UC-014 | Run pre-load participant | Save/project bridge | Registered participant | Required preparation finishes before load or blocks with result | MVP |
| UC-015 | Run post-activation participant | Project bridge | Destination active | Required finalization runs before fade-in completion | MVP |
| UC-016 | Recover to fallback | Runtime | Primary failure and valid fallback | One recovery transition runs and final result identifies recovery | MVP |
| UC-017 | Direct-scene Play Mode | Developer | Development initializer enabled | Minimum root is created only when absent and session is marked development-initialized | MVP |
| UC-018 | Validate Build Profile | Designer/tester | Scene definitions exist | Missing/disabled paths and duplicate IDs are reported before build | MVP |
| UC-019 | Load additive scene | Programmer | Additive module approved | Scene loads with ownership lease | Later |
| UC-020 | Unload owned additive scene | Programmer | Valid lease exists | Only owned scene unloads and lease completes | Later |
| UC-021 | Use Addressables provider | Project | Adapter installed | SceneFlow authority uses provider without changing callers | Later |
| UC-022 | Coordinate network travel | Multiplayer adapter | Provider and bridge installed | Network authority controls request acceptance and client synchronization | Later |

### 4.3 Explicitly unsupported use cases

- Calling a public `LoadScene(string arbitraryName)` production API.
- Treating a transition request as proof that gameplay completion, save, or unlock rules succeeded.
- Interrupting an active Unity scene load with another destination.
- Holding `allowSceneActivation` indefinitely while accepting more scene operations.
- Unloading a scene the service did not load, claim, or receive explicit ownership for.
- Using the Test Lab presenter as a required production UI framework.
- Using scene path or build index as a save-file identity.
- Running scene travel from a duplicate root that lost authority.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- The normal runtime scene-flow authority after launch handoff.
- Duplicate-safe root claiming before transition side effects.
- Project-owned scene, route, catalog, transition-profile, and flow configuration types.
- Stable scene and route identity validation.
- Runtime destination resolution to the selected backend.
- One active transition pipeline and its bounded pending queue.
- Duplicate request coalescing and busy/queue policy.
- Transition phase, progress, timing, result, and recovery state.
- Safe cancellation rules and cancellation result reporting.
- Scene-operation backend abstraction and the built-in Unity `SceneManager` backend.
- Pre-load and post-activation participant registration/execution.
- Optional presenter registration and presenter-failure containment.
- Configured reload, Main Menu, Hub, and arbitrary route helpers.
- One-attempt fallback recovery and loop protection.
- Standalone diagnostics, validation codes, setup, repair, simulation, and Test Lab tooling.

### 5.2 The package does not own

- First Light startup steps, splash sequencing, initial destination choice, or launch report.
- High-level game state, pause reasons, time scale, cursor, or input-context authority.
- Menu, loading-screen, fade, tip, notification, modal, HUD, or navigation ownership.
- Music, ambience, SFX, snapshots, audio profiles, or playback policy.
- Save files, slots, autosave timing, checkpoint rules, or save payloads.
- Level unlocks, objective completion, victory, defeat, respawn, or world-travel rules.
- Scene contents, entry spawn selection, dependency injection, or arbitrary project service location.
- Network host/client authority or synchronized scene state.
- Addressables or provider-specific asset ownership in the core package.
- Persistent scene-set behavior in the MVP.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoSceneFlow interacts |
|---|---|---|
| Initial startup and destination handoff | EchoLaunch | Separate bridge invokes SceneFlow for final transition when installed; First Light retains standalone fallback loader |
| Runtime diagnostics/dashboard | EchoDiagnostics | Separate provider bridge maps public status, queue, progress, timing, and results |
| Global preferences | EchoSettings | No core dependency; a future UI transition preference may be applied through UI/project code |
| High-level Loading state/pause/cursor/input coordination | EchoGameState | Separate bridge observes transition lifecycle and requests state changes |
| Loading/fade presentation | EchoUI or project presentation | Presenter bridge implements `ISceneTransitionPresenter`; EchoSceneFlow remains authority |
| Audio transitions | Jukebot | Bridge-owned route-to-audio mapping requests Jukebot changes; SceneFlow stores no clips |
| Save/flush behavior | EchoSave/project code | Optional participant bridge runs project-approved save preparation; no automatic save in core |
| Input locks | EchoInput/EchoGameState | Bridge or project adapter requests lock; SceneFlow does not disable actions directly |
| Starter composition | EchoGameStarter | Editor integration creates config/catalog/root/routes and reports selected bridges |
| Multiplayer synchronization | EchoMultiplayer | Later provider/coordination adapter validates authority and synchronized travel |
| Gameplay completion and route choice | Project systems/EchoObjectives/EchoProgression | Caller chooses route after its own rules succeed |

### 5.4 Boundary tests

A proposed feature belongs in EchoSceneFlow only when all of the following are true:

1. It is required to validate, serialize, execute, observe, or recover a scene operation.
2. It can function without knowing why the game requested the destination.
3. It does not require a production UI, audio, save, input, state, or multiplayer package.
4. It can be expressed using project-owned definitions and structured requests/results.
5. It does not turn a scene path, display name, or build index into durable game identity.
6. It does not require arbitrary reflection over scene objects.
7. It preserves one active scene-operation authority.
8. A missing optional collaborator produces a safe no-op, warning, or rejected integration rather than a core failure.

Features that fail these tests belong to project code, a neighboring package, a bridge, a provider adapter, or a later specification.

---

## 6. Independence Contract

Independence is a release gate.

### 6.1 Standalone guarantees

EchoSceneFlow must:

- Compile with Unity core and scene-management modules only in its runtime assembly.
- Initialize without First Light or The Workshop.
- Travel without The Observatory, The Accord, The Pulse, Jukebot, The Will, The Looking Glass, The Chronicle, or any other Echo package.
- Avoid direct references to project assemblies.
- Keep all concrete scenes, route assets, fallback choices, and presentation art project-owned.
- Provide a documented prefab/setup path and an explicit construction/test-injection path.
- Use one duplicate-safe root when persistence is configured.
- Reject duplicates before event subscription, participant registration, queue creation, or scene API use.
- Expose structured status and results without requiring The Observatory.
- Continue travel when no presenter is registered.
- Continue travel when only optional participants fail.
- Fail safely before load when required configuration, destination, participant, or backend is unavailable.
- Remove cleanly when no bridge depends on it.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Required evidence |
|---|---|---|
| Installed alone | Runtime and Editor assemblies compile; core transition works | Clean-project install and PlayMode test |
| Enter Standalone Test Lab directly | Development initializer creates one root only when absent | LAB-001, LAB-018 |
| First Light absent | Root initializes from scene/prefab and owns normal travel | PlayMode test |
| Observatory absent | Status API/logs remain available; no compile/runtime error | Assembly/removal test |
| EchoUI absent | Transition runs with no presenter or sample-only presenter | LAB-003 |
| GameState absent | Scene flow does not change time/cursor/input itself | Integration-absence test |
| Save absent | No autosave occurs; participants list may be empty | Integration-absence test |
| Duplicate root present | First root remains authority; duplicate destroys/disables before side effects | LAB-016 |
| Required configuration missing | Root reports blocker and performs no travel | LAB-012 |
| Destination invalid | Request is rejected before fade/load | LAB-009 |
| Sample content deleted | Runtime and Editor package code still compiles | Package test |
| Optional bridge removed | Core compiles and route data remains readable | Removal test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine core | Platform | Yes | 6000.0 | MonoBehaviour, ScriptableObject, Awaitable, time, logging, serialization | Package cannot run without Unity |
| Unity Scene Management module | Platform | Yes | 6000.0 | Scene query, asynchronous load, activation, unload extension seam | Package cannot perform built-in scene travel |
| Unity Editor APIs | Editor-only | Yes for tooling | 6000.0 | SceneAsset authoring, Build Profile validation, setup, inspectors | Player runtime unaffected |
| Unity Test Framework | Test-only | Yes for package tests | Compatible Unity 6 version | EditMode and PlayMode coverage | Runtime unaffected |
| uGUI/TextMeshPro | Sample-only | No core dependency | Project-selected compatible version | Standalone Lab controls and visual readout | Deleting sample leaves runtime intact |

### 6.4 Forbidden dependencies

- Another Sperk’s Forge runtime package in the core runtime assembly.
- `UnityEditor` references from runtime assemblies.
- Project-specific assemblies, scenes, tags, layers, input assets, save schemas, or service locators.
- Addressables, Cinemachine, networking SDKs, platform services, or proprietary loaders in the core.
- Runtime dependency on Samples~, Tests, or Editor tooling.
- Hard-coded scene names, build indices, Resources paths, or folder conventions.
- Reflection-based discovery as the normal participant or presenter path.
- Mutable active transition state stored in ScriptableObject definitions.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| CAP-001 | Duplicate-safe root | One application-session authority rejects duplicates before side effects | Approved | Yes | Runtime | Persistent by default |
| CAP-002 | Scene definitions | Stable-ID project assets resolve selected Scene assets to runtime paths | Approved | Yes | Runtime/Editor | Path is implementation locator, not durable identity |
| CAP-003 | Scene catalog | Validated registry of scene definitions | Approved | Yes | Runtime/Editor | No reflection discovery |
| CAP-004 | Route definitions | Stable project routes select destination/profile/fallback | Approved | Yes | Runtime/Editor | Callers may request route or scene |
| CAP-005 | Transition profiles | Project-owned phase, queue, timeout, progress, and presenter policy | Approved | Yes | Runtime/Editor | No mutable state in asset |
| CAP-006 | Async single load | Unity SceneManager backend loads one destination asynchronously | Approved | Yes | Runtime | Production default |
| CAP-007 | Serialized pipeline | One active transition operation at a time | Approved | Yes | Runtime | Prevents operation races |
| CAP-008 | Duplicate coalescing | Equivalent active/queued requests share one operation | Approved | Yes | Runtime | Double-click protection |
| CAP-009 | Busy policy | Reject new, queue FIFO, or replace pending requests according to config | Approved | Yes | Runtime | Active load never replaced |
| CAP-010 | Bounded queue | Configurable finite pending queue with status/events | Approved | Yes | Runtime | Default policy is RejectNew |
| CAP-011 | Transition phases | Validate, prepare, present-out, load, activate, finalize, present-in, complete | Approved | Yes | Runtime | Recover/fail/cancel terminal paths |
| CAP-012 | Progress | Phase and normalized overall progress with indeterminate state | Approved | Yes | Runtime | Monotonic per request |
| CAP-013 | Presenter contract | Optional non-authoritative presentation hooks | Approved | Yes | Runtime API | No uGUI dependency in core |
| CAP-014 | Participants | Ordered required/optional pre-load and post-activation hooks | Approved | Yes | Runtime API | Explicit registration handles |
| CAP-015 | Cancellation | Safe queued/pre-load cancellation and honest unsafe-phase reporting | Approved | Yes | Runtime | No false mid-load cancellation promise |
| CAP-016 | Activation policy | Immediate activation default; optional bounded activation gate | Approved | Yes | Runtime | No indefinite gate |
| CAP-017 | Route helpers | Reload, Main Menu, Hub, and arbitrary route helpers | Approved | Yes | Runtime | Project-configured references |
| CAP-018 | Recovery fallback | One attempt with loop detection and structured final result | Approved | Yes | Runtime | No recursive chain |
| CAP-019 | Status/history | Current status, queue, active request, timings, last bounded results | Approved | Yes | Runtime | Standalone diagnostics |
| CAP-020 | Direct-scene initializer | Development-only minimum authority creation | Approved | Yes | Runtime/Editor/Sample | Disabled from release by default |
| CAP-021 | Setup/repair | Create config, catalog, root prefab, definitions, routes, and lab | Approved | Yes | Editor | Preview and repeat safely |
| CAP-022 | Build Profile validation | Detect missing/disabled scenes, duplicate IDs, invalid fallbacks/routes | Approved | Yes | Editor | Manual, pre-Play, pre-build |
| CAP-023 | Failure simulation | Delays, rejection, participant/presenter/backend/fallback failures | Approved | Yes | Editor/Sample | No production mutation |
| CAP-024 | Additive loading | Load additional scene without replacing current | Deferred | No | Runtime | Requires ownership lease model |
| CAP-025 | Owned unload | Unload only scenes loaded/claimed by service | Deferred | No | Runtime | Paired with additive module |
| CAP-026 | Persistent scene sets | Shell/content/overlay scene composition | Deferred | No | Runtime/Editor | Separate expansion checkpoint |
| CAP-027 | Addressables backend | Provider adapter for addressable scenes | Deferred | No | Adapter | Separate package |
| CAP-028 | Dedicated loading scene | Persistent loading-shell strategy | Deferred | No | Runtime/Sample | Not needed for MVP |
| CAP-029 | Multiplayer coordination | Network authority and synchronized travel | Deferred | No | Bridge/provider | EchoMultiplayer research dependent |
| CAP-030 | Synchronous path | Explicit test/fallback-only immediate load | Deferred | No | Internal/Editor | Not production public default |

### 7.2 MVP capability set

The smallest complete release includes:

1. One duplicate-safe persistent `EchoSceneFlowRoot`.
2. One `EchoSceneFlowConfiguration`, `SceneCatalog`, `SceneDefinition`, `SceneRouteDefinition`, and `SceneTransitionProfile` data model.
3. Explicit initialization and service access/test-injection contracts.
4. A Unity `SceneManager` backend for asynchronous single-scene loads.
5. One serialized transition runner with phases, progress, timings, results, and cleanup.
6. Duplicate request coalescing.
7. Configurable `RejectNew`, bounded `QueueFifo`, and `ReplacePending` behavior, defaulting to `RejectNew`.
8. Optional presenter and participant registration through disposable handles.
9. Safe cancellation before the backend begins loading, with explicit unsafe-phase behavior afterward.
10. Immediate activation by default and an optional short bounded activation gate.
11. Reload, Main Menu, Hub, and general route helpers.
12. One-attempt fallback recovery with loop detection.
13. Standalone status, bounded history, diagnostic codes, and logs.
14. Setup, repair, validation, simulation, and direct-scene tooling.
15. A multi-scene Standalone Test Lab proving happy, duplicate, queue, cancel, invalid, failure, fallback, and direct-entry paths.

### 7.3 Later capability set

Approved later design areas, not MVP commitments:

- Additive load requests.
- Explicit additive scene ownership leases and safe unload.
- Persistent scene sets and active-scene policy.
- Backend adapters such as Addressables.
- Dependency/preload groups.
- Dedicated loading-shell scene pattern.
- Scene warmup/finalization providers.
- Route graph visualization.
- Transition-history export.
- Networked scene-flow coordination.
- Platform-specific loading optimizations.
- Optional memory cleanup policy hooks.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Public raw string load API | Rejected | Reintroduces hidden strings and bypasses validation/stable IDs | Never for production core |
| Build index as identity | Rejected | Ordering is mutable and unsuitable for durable references | Never |
| Scene loader component in every scene | Rejected | Creates competing authorities and duplicate state | Never |
| Automatic save before every transition | Rejected | Save policy belongs to project/EchoSave | Bridge/project requirement |
| Direct audio playback from route | Rejected | Jukebot owns playback | Separate bridge |
| Direct pause/time/input changes | Rejected | EchoGameState/Input own those concerns | Separate bridge |
| Unbounded queue | Rejected | Can execute stale navigation long after intent changes | Never |
| Replace active load | Rejected | Unity operation cannot be safely interrupted | Never |
| Indefinite activation hold | Rejected | Stalls Unity async operation queue | Never |
| Additive scene ownership in MVP | Deferred | Needs lease/unload/persistent-set design and broader tests | After single-load release |
| Addressables hard dependency | Rejected for core | Makes a provider choice mandatory | Separate adapter |
| Reflection-based scene participants | Rejected | Hidden behavior and removal fragility | Explicit registration remains standard |
| Loading scene as mandatory topology | Rejected | Imposes project structure and inflates MVP | Optional later pattern |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Scene definitions, catalog, routes, transition profiles, flow configuration, stable IDs, policies, timeouts, fallbacks | Active request, queue, progress, participant instances, presenter instances, Unity operation objects |
| Runtime state/behavior | Root, service, resolver, validator, queue, runner, backend, context, handles, results, histories, registrations | Editor APIs, production UI assumptions, save/audio/game-state rules |
| Presentation/feedback | Presenter interface, sample Lab presenter, optional EchoUI bridge | Scene loading authority, route validation, queue ownership, completion truth |

### 8.2 Component topology

```text
Project-owned assets
├── EchoSceneFlowConfiguration
│   ├── SceneCatalog
│   ├── Default SceneTransitionProfile
│   ├── Main Menu route (optional)
│   ├── Hub route (optional)
│   ├── queue/recovery/history policy
│   └── direct-scene development policy
├── SceneDefinition assets
├── SceneRouteDefinition assets
└── SceneTransitionProfile assets

EchoSceneFlowRoot (persistent authority)
└── SceneFlowService
    ├── SceneResolver / Validator
    ├── SceneTransitionQueue
    ├── SceneTransitionRunner
    │   ├── ISceneLoadBackend (UnitySceneManagerBackend)
    │   ├── participant registry
    │   ├── presenter registration
    │   └── recovery controller
    ├── current SceneFlowStatus
    └── bounded result/history buffer

Optional external pieces
├── sample presenter
├── First Light bridge
├── Observatory bridge
├── Game State bridge
├── EchoUI presenter bridge
├── Save participant bridge
├── Jukebot route-audio bridge
└── project adapters
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | Yes for the standard runtime path |
| Root type | `EchoSceneFlowRoot` |
| Lifetime | Application session by default; survives single-scene travel |
| Duplicate behavior | First valid root claims authority; duplicate rejects itself before side effects and reports `ESF-002` |
| Initialization trigger | Explicit `InitializeAsync` called by root on `Awake` when configured, or by First Light bridge/project code |
| Shutdown behavior | Reject new requests, cancel queued/pre-load work, allow non-cancellable active load to finish or mark shutdown state, dispose registrations, clear static convenience access |
| Direct-scene behavior | Development initializer creates configured root only when absent; marks status as development initialized |
| Test injection seam | `ISceneFlowService`, `ISceneLoadBackend`, clock, logger, and configuration passed through factory/constructor path |

The root owns the service, queue, backend lifetime, histories, presenter registration, participants, and update loop. Those children are not independent persistent singletons.

### 8.4 Transition lifecycle sequence

1. **Claim and initialize**
   - Root claims authority before subscriptions or allocations that can affect the project.
   - Configuration and catalog validate.
   - Backend initializes and current scene is resolved when possible.

2. **Request**
   - Caller creates a `SceneTransitionRequest` or requests a route helper.
   - Service resolves route, destination, profile, and fallback.
   - Request receives a runtime transition ID and coalescing key.

3. **Admission**
   - Equivalent active/queued request coalesces.
   - Otherwise request starts, queues, replaces only pending work, or rejects according to policy.

4. **Validate**
   - Destination, backend support, catalog membership, route/fallback loops, current-scene rules, and participant availability validate.
   - Synchronous guards remain side-effect free.

5. **Prepare**
   - Required and optional pre-load participants run in deterministic order with individual timeout/cancellation rules.
   - Failure before load leaves the source scene active.

6. **Present out**
   - Optional presenter receives transition context and obscures or prepares the outgoing scene.
   - No presenter is a valid path.

7. **Load**
   - Backend starts exactly one asynchronous scene operation.
   - Request enters a non-cancellable phase once Unity loading begins.
   - Load progress is normalized and reported.

8. **Await activation**
   - Default profile activates immediately when ready.
   - Optional bounded activation gate may satisfy a short minimum presentation or readiness condition.
   - The gate has a hard timeout and accepts no second scene operation.

9. **Activate**
   - Backend activates destination.
   - Current/previous scene runtime records update after authoritative activation.

10. **Finalize**
    - Required and optional post-activation participants run.
    - Failures are converted into warning, failure, or recovery according to registration/profile policy.

11. **Present in**
    - Optional presenter reveals the destination.
    - Presenter failure is reported and service performs forced cleanup so the queue cannot remain locked.

12. **Complete**
    - Final result and timings publish after service state changes.
    - Registrations remain; per-request runtime objects release.
    - Next valid queued request may start after a configured one-frame boundary.

13. **Recover/fail/cancel**
    - Pre-load failure/cancellation unwinds presentation and remains in source scene.
    - Post-load failure may attempt one configured fallback.
    - Every terminal path releases locks and produces one structured result.

### 8.5 Transition phase model

| Phase | Meaning | Cancellation | Progress behavior |
|---|---|---|---|
| Queued | Accepted but waiting | Safe | Queue position, no overall advance |
| Validating | Pure request/config/backend checks | Safe | Determinate or immediate |
| Preparing | Awaiting pre-load participants | Safe when participant honors cancellation | Weighted phase progress |
| PresentingOut | Optional fade/loading presentation begins | Safe with presenter cleanup | Weighted phase progress |
| Loading | Unity/backend operation active | Not guaranteed; MVP treats as non-cancellable | Backend progress normalized |
| AwaitingActivation | Backend ready, bounded gate open/closed | Cancellation becomes recovery request, not abort | Load phase complete; activation pending |
| Activating | Destination becomes active | Not cancellable | Short/indeterminate |
| Finalizing | Post-activation participants | Cannot restore source automatically | Weighted phase progress |
| PresentingIn | Reveal destination | No travel cancellation; presenter cleanup only | Weighted phase progress |
| Recovering | One fallback attempt | Follows recovery request rules | New internal recovery progress linked to parent |
| Completed | Destination active and pipeline cleaned | Terminal | 1.0 |
| Cancelled | Cancelled before backend load | Terminal | Last progress retained |
| Failed | No successful destination/recovery | Terminal | Last progress retained |

### 8.6 Unity backend basis

The MVP backend uses Unity’s asynchronous scene API. Unity documents that `SceneManager.LoadSceneAsync` returns an `AsyncOperation`, and that `LoadSceneMode.Single` unloads current scenes as the new scene is loaded. The package therefore validates the destination before starting the operation and keeps its authority root outside scene lifetime.

When `AsyncOperation.allowSceneActivation` is false, Unity holds progress at `0.9`, keeps `isDone` false, and stalls subsequent async operations in the queue. EchoSceneFlow therefore uses delayed activation only as a short bounded gate, never as an indefinite waiting room and never while launching another scene operation.

Reference basis:

- [Unity 6 `SceneManager.LoadSceneAsync`](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html)
- [Unity 6 `AsyncOperation`](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AsyncOperation.html)
- [Unity `AsyncOperation.allowSceneActivation`](https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html)
- [Unity 6 `SceneManager.SetActiveScene`](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SceneManagement.SceneManager.SetActiveScene.html)

### 8.7 Failure model

| Failure | Detection point | User-visible/runtime result | Fallback/cleanup | Code |
|---|---|---|---|---|
| Missing configuration | Initialization | Authority NotReady; requests reject | No load; setup guidance | ESF-001 |
| Duplicate root | `Awake` claim | Duplicate disables/destroys | Existing authority unchanged | ESF-002 |
| Unknown scene ID | Resolution | Request rejected | Source remains | ESF-003 |
| Scene asset/path missing | Validation | Request rejected | Source remains | ESF-004 |
| Scene not enabled in Build Profile | Editor/preflight | Build blocker or request rejection | Source remains | ESF-005 |
| Busy policy rejection | Admission | Structured Busy result | Active unchanged | ESF-006 |
| Queue full | Admission | Structured QueueFull result | Queue unchanged | ESF-007 |
| Required participant failure/timeout | Prepare/finalize | Reject, fail, or recover by phase | Unwind or fallback | ESF-008 |
| Presenter failure | Present out/in | Warning or failure by phase | Force presenter cleanup; release lock | ESF-009 |
| Backend cannot start | Load start | Failed result | Reveal source; optional fallback only if safe | ESF-010 |
| Activation gate timeout | Await activation | Force activation by default or fail per explicit profile | No indefinite stall | ESF-011 |
| Post-activation failure | Finalize | Failed or recovered result | One fallback attempt | ESF-012 |
| Fallback missing/invalid | Recovery planning | Original failure returned | No recursion | ESF-013 |
| Fallback loop | Validation/recovery | Recovery rejected | Final failure | ESF-014 |
| Cancellation unsafe in phase | Load/activation | CancellationNotSupportedInPhase status | Operation continues | ESF-015 |
| Development initializer used | Direct scene | Informational status/banner | Continue with marked mode | ESF-016 |
| Configured route alias missing | Helper call | Request rejected | No hidden default | ESF-017 |
| Duplicate scene ID | Editor/init validation | Blocker | No Ready state | ESF-018 |
| Duplicate route ID | Editor/init validation | Blocker | No ambiguous resolution | ESF-019 |
| Unexpected exception | Any runtime phase | Converted to structured failure | Finally cleanup; optional one fallback | ESF-020 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoSceneFlowConfiguration` | Root policy, catalog, defaults, queue, recovery, history, direct-scene configuration | Optional config ID | No | Yes |
| `SceneCatalog` | Explicit registry of valid scene definitions | Yes | No | Yes |
| `SceneDefinition` | Domain `SceneId` plus separate Editor source GUID/path metadata and runtime locator | Yes (`SceneId`) | No | Yes |
| `SceneRouteDefinition` | Stable named route to destination/profile/fallback | Yes | No | Yes |
| `SceneTransitionProfile` | Phase weights, presenter key, timeouts, queue override, activation/recovery policy | Yes | No | Yes |
| `SceneFlowDevelopmentConfiguration` | Direct-scene initializer behavior and development-only root reference | Optional | No | Yes |

#### `SceneDefinition`

Minimum fields:

- Stable `SceneId` generated once and validated for collision.
- Human-readable display name.
- Editor source asset GUID stored as a plain string for authoring/repair only.
- Serialized full project-relative scene path for the built-in backend.
- The custom Editor inspector presents a `SceneAsset` picker and writes GUID/path data without placing a `UnityEditor` type in the runtime assembly.
- Optional category/tags for tooling only.
- Enabled/disabled project availability flag.
- Optional documentation note.

The path is a runtime locator, not the durable identity. Moving or renaming the Scene asset is detected from its recorded editor source GUID, and Editor tooling updates the path while retaining `SceneId`.

#### `SceneRouteDefinition`

Minimum fields:

- Stable `RouteId`.
- Human-readable display name.
- Destination `SceneDefinition`.
- Optional transition-profile override.
- Optional fallback route.
- Same-destination behavior: reject, coalesce, or explicit reload.
- Optional project-defined tags/metadata that core does not interpret.

Routes do not contain save, audio, unlock, or game-state rules. Bridges may own separate mappings keyed by `RouteId`.

#### `SceneTransitionProfile`

Minimum fields:

- Stable profile ID and display name.
- Presenter key/reference contract, not a scene object.
- Phase weights for overall progress.
- Participant timeout defaults.
- Presenter timeout/cleanup policy.
- Activation policy: Immediate or BoundedGate.
- Maximum activation-gate duration.
- Busy/queue override, if allowed by configuration.
- Fallback policy and post-activation failure classification.
- Progress notification rate.
- One-frame separation between queued operations.

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `SceneFlowService` | Root | Authority lifetime | Shutdown/domain reset | Not durable |
| `SceneFlowStatus` | Service | Replaced/updated through lifetime | New initialization | Snapshot only |
| `SceneTransitionOperation` | Runner | One accepted transition | Released after bounded history capture | Never serialized |
| `SceneTransitionQueue` | Service | Authority lifetime | Cleared on shutdown | Never serialized |
| `SceneTransitionHandle` | Service/caller | Accepted request through terminal result | Terminal and caller release | Never serialized |
| `SceneTransitionContext` | Runner | One operation | Released on terminal state | Never serialized |
| `SceneTransitionProgress` | Runner | One operation updates | Terminal result | Snapshot only |
| Participant registration | Service | Until handle disposed/root shutdown | Explicit disposal | Never serialized |
| Presenter registration | Service | Until replaced/disposed/root shutdown | Explicit disposal | Never serialized |
| Bounded transition history | Service | Configured application-session count | Root shutdown | Optional diagnostic export only |
| Current/previous scene record | Service | Application session | Updated after activation | Exposed for save adapter, not saved by core |

### 9.3 Stable identifiers

- `SceneId`, `RouteId`, and `TransitionProfileId` are opaque stable strings generated by Editor tooling.
- IDs must be non-empty, normalized, and unique within their catalog/domain.
- Display names and asset filenames are not IDs.
- Moving or renaming a Scene asset does not change its `SceneId`.
- Changing an ID after release requires an alias/migration record in project tooling.
- Runtime requests from external or saved data resolve an ID through the catalog. They do not use an arbitrary path.
- Runtime transition IDs are generated per accepted operation and are not durable content IDs.

### 9.4 ScriptableObject safety

Definition assets hold immutable design/configuration data only. They must not store:

- Current active phase or progress.
- Queue contents.
- Last loaded scene runtime object.
- Participant/presenter instances.
- AsyncOperation references.
- Cancellation state.
- Runtime timings or counters.
- Current transition result.

All changing values live in service-owned runtime objects keyed by stable definition IDs or references.

### 9.5 Request equivalence and coalescing

The service computes a coalescing key from:

- Operation kind.
- Destination `SceneId` or `RouteId` resolved destination.
- Load mode.
- Explicit reload flag.
- Transition-profile ID when behavior materially differs.
- Caller-supplied coalescing scope when approved.

Equivalent active or queued requests receive the existing handle. A caller may opt out only for an explicit reload operation. Coalescing never merges different destinations or different load modes.

### 9.6 Serialization and migration

EchoSceneFlow does not persist active runtime operations. Project-owned assets use Unity serialization and stable `.meta` GUIDs. The package must version:

- `EchoSceneFlowConfiguration` schema.
- Scene definition schema.
- Route definition schema.
- Transition profile schema.
- Diagnostic snapshot/result schema.

Editor migration tooling previews asset changes, backs up when practical, preserves IDs/GUIDs, and reports every modified asset. Unknown newer schema versions are not rewritten silently.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `EchoSceneFlowRoot` | MonoBehaviour | Claims persistent authority and owns service lifetime | Project prefab/scene or development initializer |
| `ISceneFlowService` | Interface | Testable public transition authority | Implemented by `SceneFlowService` |
| `EchoSceneFlowConfiguration` | ScriptableObject | Project root policy and references | Project-owned asset |
| `SceneCatalog` | ScriptableObject | Scene-definition registry | Project-owned asset |
| `SceneDefinition` | ScriptableObject | Stable scene identity and runtime locator | Project-owned asset |
| `SceneRouteDefinition` | ScriptableObject | Reusable destination/profile/fallback route | Project-owned asset |
| `SceneTransitionProfile` | ScriptableObject | Transition lifecycle policy | Project-owned asset |
| `SceneTransitionRequest` | Immutable struct/class | One caller request | Caller/factory |
| `SceneTransitionHandle` | Sealed class/read-only interface | Observe accepted/queued/active operation | Service-owned, caller reference |
| `SceneTransitionResult` | Immutable struct | Terminal outcome and diagnostics | Runner-created |
| `SceneTransitionProgress` | Immutable struct | Phase, phase progress, overall progress, message code | Runner-created |
| `SceneFlowStatus` | Immutable snapshot | Authority, config, current scene, active request, queue, last result | Service-created |
| `SceneTransitionContext` | Read-only class | Data exposed to presenter/participants | Runner-owned |
| `SceneTransitionPhase` | Enum | Normalized lifecycle phase | Package |
| `SceneTransitionOutcome` | Enum | Completed, warning, rejected, cancelled, failed, recovered | Package |
| `SceneRequestAdmission` | Enum | Started, queued, coalesced, rejected | Package |
| `SceneRequestPolicy` | Enum | RejectNew, QueueFifo, ReplacePending | Configuration/profile |
| `SceneActivationPolicy` | Enum | Immediate, BoundedGate | Profile |
| `ISceneLoadBackend` | Interface | Abstract scene query/load/activation backend | Default backend or injected adapter |
| `ISceneTransitionPresenter` | Interface | Optional presentation lifecycle | Sample, project, or EchoUI bridge |
| `ISceneTransitionParticipant` | Interface | Optional ordered prepare/finalize work | Bridge/project implementation |
| `SceneTransitionParticipantDescriptor` | Immutable data | ID, order, required flag, timeout, phases | Registration |
| `SceneFlowRegistration` | Disposable handle | Removes participant/presenter safely | Service-created |
| `EchoSceneFlowDirectInitializer` | MonoBehaviour | Development-only absent-root creation | Sample/project development scene |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/main-loop rule |
|---|---|---|---|---|
| `InitializeAsync(...)` | Claim/validate/initialize service | Authority and configuration available | Fresh `Awaitable<SceneFlowInitializationResult>` | Unity calls on main thread |
| `RequestTransition(request)` | Admit a destination request and return handle immediately | Service Ready | Handle indicates Started/Queued/Coalesced/Rejected | Main thread |
| `RequestTransitionAsync(request, cancellation)` | Admit and await terminal result | Service Ready | Fresh `Awaitable<SceneTransitionResult>` | Main thread; backend controls Unity operation |
| `RequestRouteAsync(route, cancellation)` | Travel using project route | Valid route | Structured terminal result | Main thread |
| `ReloadCurrentAsync(profile, cancellation)` | Explicit reload through pipeline | Current scene resolves | Rejects if current scene absent/unknown | Main thread |
| `ReturnToMainMenuAsync(cancellation)` | Use configured route alias | Main Menu route configured | `ESF-017` if absent | Main thread |
| `ReturnToHubAsync(cancellation)` | Use configured route alias | Hub route configured | `ESF-017` if absent | Main thread |
| `TryCancel(transitionId)` | Cancel queued or safely pre-load request | Matching active/queued request | Returns cancellation disposition | Main thread |
| `RegisterParticipant(participant, descriptor)` | Add explicit lifecycle participant | Unique ID and compatible service | Disposable handle or structured rejection | Main thread |
| `RegisterPresenter(presenter, key, replacePolicy)` | Add/select optional presenter | Compatible presenter | Disposable handle; no ownership transfer | Main thread |
| `GetStatus()` | Obtain immutable current snapshot | Any initialized state | Never exposes mutable internals | Main thread or copied snapshot |
| `TryResolveScene(sceneId, out definition)` | Resolve stable ID | Catalog loaded | False on unknown/disabled | Main thread |
| `TryResolveRoute(routeId, out route)` | Resolve stable route | Catalog/config loaded | False on unknown/disabled | Main thread |
| `ShutdownAsync(...)` | Stop admission, dispose and clean service | Authority | Fresh result; active non-cancellable operation handled explicitly | Main thread |

### 10.3 Request construction

`SceneTransitionRequest` minimum data:

- Destination scene or route reference.
- Operation kind: Travel or ExplicitReload in MVP.
- Optional transition-profile override.
- Optional request-policy override when configuration permits it.
- Optional correlation ID for logs/support.
- Optional presenter key.
- Optional project payload object constrained to non-durable, read-only context use.
- Cancellation token for queued/pre-load phases.

The request does not contain arbitrary scene path, save payload, audio clip, UI object, or gameplay completion flag.

### 10.4 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `AuthorityChanged` | Root/service | After authority state changes | Authority snapshot | Informational only |
| `Initialized` | Service | After Ready state committed | Initialization result | Listener not required |
| `TransitionAdmitted` | Service | After admission state committed | Handle/admission | May be Started/Queued/Coalesced |
| `QueueChanged` | Service | After queue mutation | Bounded queue snapshot | No mutable queue access |
| `TransitionStarted` | Runner | After active operation set | Context snapshot | Cannot veto; guards already ran |
| `PhaseChanged` | Runner | After phase state changes | Phase/progress snapshot | Presentation listener optional |
| `ProgressChanged` | Runner | At configured bounded rate | Progress snapshot | Must not block runner |
| `DestinationActivated` | Runner | After backend confirms activation/state update | Source/destination context | Gameplay object `Start` timing is not guaranteed by this event alone |
| `TransitionCompleted` | Service | After terminal state/history/lock cleanup | Result | Listener not required for completion |
| `TransitionFailed` | Service | After failure state/history/cleanup | Result | Recovery status included |
| `TransitionCancelled` | Service | After safe cancellation cleanup | Result | Only pre-load cancellation is true cancellation |
| `ServiceShutdown` | Service | After shutdown state committed | Shutdown result | Listeners must unsubscribe/dispose |

Events are semantic notifications after authoritative state changes. They are never required to make a transition finish.

### 10.5 Participant contract

`ISceneTransitionParticipant` exposes package-owned async callbacks for the phases it registers:

- `PrepareAsync(SceneTransitionContext, CancellationToken)` before the backend load begins.
- `FinalizeAsync(SceneTransitionContext, CancellationToken)` after destination activation and before final reveal completes.
- Optional synchronous terminal notification through a separate observer method or event subscription.

Registration defines:

- Stable participant ID.
- Deterministic order.
- Required or optional classification per callback.
- Timeout.
- Failure severity and whether post-activation failure requests fallback.

Rules:

- Participants register explicitly and receive disposable handles.
- Required participant failure blocks before load when possible.
- Optional participant failure becomes warning and does not stop travel.
- Participants must not recursively call scene travel during their own callback.
- The service detects re-entry and returns a structured rejection.
- A participant cannot retain mutable context after completion.

### 10.6 Presenter contract

`ISceneTransitionPresenter` may receive:

- `BeginAsync(context, cancellation)` for present-out/loading state.
- `UpdateProgress(progress)` as a bounded non-blocking callback.
- `RevealAsync(context, cancellation)` after finalization.
- `ForceCleanup(context, reason)` on cancellation/failure/timeout.

Rules:

- No presenter is valid.
- Presenter failure cannot retain the transition lock indefinitely.
- A presenter does not call Unity scene APIs.
- A presenter does not decide destination, admission, result, or recovery.
- Production UI navigation remains with EchoUI/project code.

### 10.7 Async and cancellation policy

- Public async methods use fresh Unity `Awaitable<T>` instances, consistent with the approved Foundation Unity 6 baseline.
- Unity scene APIs run on the main thread.
- Cancellation is cooperative while a request is queued, validating, preparing, or presenting out.
- Once `ISceneLoadBackend.BeginLoad` succeeds, the MVP operation is considered non-cancellable because Unity’s built-in load operation has no safe general abort contract.
- A cancellation request during Loading, AwaitingActivation, Activating, Finalizing, or PresentingIn is recorded and returned as `CancellationNotSupportedInCurrentPhase`; the pipeline continues to a safe terminal state or recovery.
- Timeouts never abandon a live Unity operation. They change result/recovery policy and force cleanup when the backend reaches a safe boundary.
- Delayed activation is bounded. On gate timeout the default policy forces activation to release Unity’s operation queue, then reports warning/failure according to profile.
- Awaitable instances are not cached or awaited repeatedly.

### 10.8 API ergonomics

Novice path:

1. Use setup tool.
2. Select Scene assets.
3. Create route assets.
4. Place root prefab.
5. Call a UnityEvent-friendly route requester component or sample button.

Programmer path:

1. Inject or resolve `ISceneFlowService`.
2. Build typed request or reference route asset.
3. Receive handle immediately or await structured result.
4. Register participants/presenter explicitly when needed.
5. Inspect immutable status and diagnostics.

Static convenience access may exist on the root, but test injection and explicit service references remain first-class.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install package through supported UPM route.
2. Open **Tools > EchoDevGames > The Passage > Setup**.
3. Select or create a project folder for generated configuration assets.
4. Preview creation of configuration, catalog, default transition profile, root prefab, and optional route definitions.
5. Add Scene assets to the catalog through object fields, not typed paths.
6. Mark optional Main Menu and Hub routes.
7. Preview Build Profile changes separately; never silently enable scenes.
8. Apply create-only-safe changes.
9. Import/open the Standalone Test Lab.
10. Run validation and failure simulations.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create core assets | Config, catalog, default profile | None unless selected | Yes | Unity Undo/create report | Asset paths and IDs |
| Create root prefab | Root prefab wired to config | Optional selected scene instance | Yes | Undo and preview | Created/reused object |
| Add scene definition | Definition asset from SceneAsset picker | Catalog list | Yes; duplicate detection | Undo | Scene ID/source GUID/path/build state |
| Create route | Route definition | Optional config aliases | Yes | Undo | Route/destination/profile |
| Repair scene locator | None | Runtime path from selected SceneAsset | Yes | Undo, before/after report | Changed definitions |
| Validate Build Profile | None | None | Yes | Not applicable | Errors/warnings |
| Add scenes to Build Profile | None | Active Build Profile scene list | Explicit only | Preview and backup/list diff | Added/enabled scenes |
| Create Test Lab copy | Sample project assets | Project sample folder | Yes with conflict report | Create-only | Imported scenes/assets |
| Generate support report | Report file | None | Yes | User-selected output | Versioned sanitized snapshot |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Passage Setup Window | Installer | Create/repair config, catalog, root, routes, and report | No |
| Scene Definition Inspector | Designer | Pick a SceneAsset, record source GUID/path, and view stable ID/build status | No |
| Scene Catalog Inspector | Designer/maintainer | Search, sort, detect duplicates, show missing assets | No |
| Route Inspector | Designer | Select destination/profile/fallback and detect loops | No |
| Transition Profile Inspector | Designer | Edit phase weights, timeouts, activation, queue, progress policy | No |
| Scene Flow Validator | Tester/maintainer | Run package/project checks manually, pre-Play, pre-build | No |
| Transition Simulator | Tester | Inject delays/failures without changing production scenes | No |
| Runtime Status Inspector | Developer | View root, phase, queue, timings, last results in Play Mode | No external package |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| ESF-VAL-001 | Missing configuration | Blocker | Yes | Create only |
| ESF-VAL-002 | More than one enabled root in canonical setup | Error | Guided | No destructive auto-fix |
| ESF-VAL-003 | Missing catalog | Blocker | Yes | Create/assign only |
| ESF-VAL-004 | Empty scene ID | Error | Yes | Generate with confirmation |
| ESF-VAL-005 | Duplicate scene ID | Blocker | Yes | No silent ID rewrite |
| ESF-VAL-006 | Recorded source GUID/path cannot resolve to a Scene asset | Error | Guided | No |
| ESF-VAL-007 | Runtime path stale | Error | Yes | Yes when recorded source GUID resolves |
| ESF-VAL-008 | Scene missing from active Build Profile | Error/build blocker | Yes | Explicit preview only |
| ESF-VAL-009 | Scene disabled in active Build Profile | Error/build blocker | Yes | Explicit preview only |
| ESF-VAL-010 | Duplicate route ID | Blocker | Yes | No silent ID rewrite |
| ESF-VAL-011 | Route destination missing/disabled | Error | Guided | No |
| ESF-VAL-012 | Fallback route loop | Blocker | Guided | No |
| ESF-VAL-013 | Main Menu/Hub alias unassigned | Info/Warning | Yes | No |
| ESF-VAL-014 | Invalid phase weights | Error | Yes | Normalize with preview |
| ESF-VAL-015 | Indefinite/unsafe activation gate | Blocker | Yes | Clamp only with confirmation |
| ESF-VAL-016 | Queue capacity outside range | Error | Yes | Clamp with preview |
| ESF-VAL-017 | Runtime assembly references Editor | Blocker | Developer fix | No |
| ESF-VAL-018 | Direct initializer enabled for release | Warning/Blocker by policy | Yes | Disable with confirmation |
| ESF-VAL-019 | Required participant ID duplicate in configured bootstrap | Error | Guided | No |
| ESF-VAL-020 | Sample presenter referenced as production dependency | Warning | Guided | No |

Validation itself does not mutate project data. Repairs are explicit, previewed, and reported.

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Supported for initial release:

- Embedded package during development.
- Local path package.
- Local `.tgz` package.
- Git URL/tag after repository release.
- The Workshop selection when EchoGameStarter is available.

Every route must be tested in a clean Unity 6000.0+ project before release.

### 12.2 Minimal production scene setup

The minimal standalone setup requires:

1. One project-owned `EchoSceneFlowConfiguration`.
2. One `SceneCatalog` containing source/destination definitions.
3. One default `SceneTransitionProfile`.
4. One `EchoSceneFlowRoot` prefab instance in the canonical starting scene or another explicit creation path.
5. Destination scenes enabled in the active Build Profile.
6. At least one caller referencing a `SceneDefinition` or `SceneRouteDefinition`.

A presenter is optional. No EventSystem, Canvas, input asset, audio object, save system, or First Light root is required.

### 12.3 Boot-scene setup

When First Light is installed:

- First Light remains the initial composition authority.
- A separate First Light–Passage startup bridge creates/initializes EchoSceneFlow during the Core Services phase.
- First Light may use EchoSceneFlow for its final destination transition.
- EchoSceneFlow becomes the normal travel authority after handoff.
- If the bridge is absent, First Light uses its own minimal initial load and EchoSceneFlow may initialize later in the destination scene.
- Neither core package references the other directly.

### 12.4 Direct-scene setup

`EchoSceneFlowDirectInitializer` is a development feature:

- It checks for an existing authority before creating anything.
- It references a project-owned development configuration/root prefab.
- It rejects duplicates using the same claim path as production.
- It marks `SceneFlowStatus.InitializationMode` as DirectSceneDevelopment.
- It may display a clear development indicator through the sample presenter or logs.
- It is disabled/excluded in release builds by default.
- Projects may require the canonical Boot scene and disable direct initialization for sensitive tests.

### 12.5 Scene isolation rule

Every scene-visible feature is proven in isolated sample scenes containing only EchoSceneFlow, declared Unity dependencies, and redistributable sample assets. Integration scenes for First Light, EchoGameState, EchoUI, EchoSave, or Jukebot remain separate and do not count as standalone proof.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Test Lab purpose

The Standalone Test Lab proves the complete MVP without any other Sperk’s Forge package. It uses multiple small scenes because a scene-transition package cannot be proven honestly inside one scene.

Proposed sample layout:

```text
Samples~/Standalone Labs/The Passage Lab/
├── Scenes/
│   ├── Passage_Lab_A.unity
│   ├── Passage_Lab_B.unity
│   └── Passage_Lab_Fallback.unity
├── Configuration/
├── Definitions/
├── Routes/
├── Presentation/
├── Scripts/
└── README.md
```

`Passage_Lab_A` contains the root, sample-only uGUI/TMP controls, status readout, queue controls, duplicate-root generator, failure toggles, and route requests. `Passage_Lab_B` proves destination activation and return travel. `Passage_Lab_Fallback` proves recovery. None is a production dependency.

### 13.2 Required Test Lab contents

- Visible current scene ID/name and initialization mode.
- Active transition ID, route, source, destination, phase, phase progress, overall progress, elapsed time, and queue.
- Buttons for travel A/B, reload, Main Menu alias, Hub alias, rapid duplicate, competing request, queued cancel, and reset.
- Toggle for RejectNew versus QueueFifo versus ReplacePending.
- Simulated slow presenter.
- Simulated presenter failure.
- Required/optional participant delay, failure, timeout, and disposal.
- Missing/invalid route request.
- Activation-gate test with hard timeout.
- Fallback success and fallback-loop rejection.
- Duplicate root creation.
- Direct-scene entry instructions.
- Clear success/failure history.
- No project-owned or restricted content.

### 13.3 Test Lab acceptance checklist

| Test | Action | Expected result | Automated/manual | Status |
|---|---|---|---|---|
| LAB-001 | Enter Lab A directly | One development-initialized root becomes Ready | Manual/PlayMode | Not run |
| LAB-002 | Request Lab B route | Phases run and B becomes destination | Both | Not run |
| LAB-003 | Remove/disable presenter and travel | Travel completes with no visual presenter | Both | Not run |
| LAB-004 | Rapid-click same route | One operation; later callers coalesce | Both | Not run |
| LAB-005 | Request different route while busy under RejectNew | Second returns Busy | Both | Not run |
| LAB-006 | Enable QueueFifo and submit two routes | Second queues and starts after first terminal cleanup | Both | Not run |
| LAB-007 | Fill bounded queue | Excess request returns QueueFull | Both | Not run |
| LAB-008 | Replace pending route | Pending request cancels/replaces; active operation unchanged | Both | Not run |
| LAB-009 | Request invalid scene/route | Rejected before presenter/load | Both | Not run |
| LAB-010 | Cancel queued request | Queue removes it; terminal Cancelled result | Both | Not run |
| LAB-011 | Cancel during preparation | Participant/presenter cleanup runs; source remains | Both | Not run |
| LAB-012 | Cancel during Loading | Status says cancellation unsafe; operation reaches safe terminal state | Both | Not run |
| LAB-013 | Fail required pre-load participant | No load starts; source remains | Both | Not run |
| LAB-014 | Fail optional participant | Transition completes with warning | Both | Not run |
| LAB-015 | Timeout activation gate | Gate releases according to policy; no permanent stall | Both | Not run |
| LAB-016 | Spawn duplicate root | Duplicate performs no transition side effects | Both | Not run |
| LAB-017 | Trigger presenter failure | Service force-cleans and unlocks queue | Both | Not run |
| LAB-018 | Open Lab B directly | Direct initializer creates one root and return route works | Manual | Not run |
| LAB-019 | Trigger primary failure with fallback | Fallback scene loads once; result says Recovered | Both | Not run |
| LAB-020 | Configure fallback loop | Validation rejects loop before runtime recovery | EditMode/manual | Not run |
| LAB-021 | Reload current scene | Explicit reload runs instead of coalescing as same destination | Both | Not run |
| LAB-022 | Use missing Main Menu alias | Structured alias-missing result | Both | Not run |
| LAB-023 | Dispose participant registration | Participant no longer runs and no callback targets destroyed object | Both | Not run |
| LAB-024 | Complete repeated 100 transitions | No lock leak, duplicate root, queue corruption, or unbounded history | Stress/manual | Not run |
| LAB-025 | Delete sample folder | Runtime package still compiles | Package/manual | Not run |

### 13.4 Optional integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| First Light + Passage | EchoLaunch, EchoSceneFlow, bridge | Final launch transition and handoff | Depends on two authorities |
| Pulse + Passage | EchoGameState, EchoSceneFlow, bridge | Loading-state coordination | Depends on state authority |
| Looking Glass + Passage | EchoUI, EchoSceneFlow, bridge | Production loading/fade presenter | Depends on UI framework |
| Chronicle + Passage | EchoSave, EchoSceneFlow, bridge | Pre-travel save participant | Depends on save authority and project policy |
| Resonance + Passage | Jukebot, EchoSceneFlow, bridge | Route-to-audio transition mapping | Depends on audio authority |
| Observatory + Passage | EchoDiagnostics, EchoSceneFlow, bridge | Runtime panel and snapshot | Depends on diagnostics package |

Samples are separately importable and removable.

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The runtime core is nonvisual. It exposes an optional presenter contract. The Standalone Lab ships a sample-only uGUI/TextMeshPro presenter. EchoUI or project code owns production loading screens, fades, navigation, focus, tips, animation, and art.

EchoSceneFlow owns only the timing boundary at which a presenter is asked to obscure or reveal the scene and the structured progress it receives.

### 14.2 Required presentation states

A presenter integration must be able to represent:

- Idle/hidden.
- Queued.
- Validating/preparing.
- Loading determinate.
- Loading indeterminate.
- Awaiting activation.
- Finalizing.
- Recovering.
- Warning.
- Blocking failure.
- Cancelled before load.
- Completed/reveal.

No state may rely on color alone.

### 14.3 Accessibility requirements

- Core transition completion must never require animation or a visual listener.
- Presenter animation must use unscaled timing when a bridge has paused gameplay.
- Reduced-motion presenters must be possible without changing scene-flow core behavior.
- Progress text and status labels must have screen-reader/assistive labeling where the selected UI technology supports it.
- Text must remain scalable and readable with sufficient contrast.
- Loading tips must not contain essential information that disappears before it can be read.
- Minimum display duration is presentation policy and must not become an indefinite activation gate.
- Audio-only feedback must have visual/status equivalents.
- A failed presenter must not trap the user behind an opaque screen.

### 14.4 Visual customization

Project-specific visuals, loading copy, progress style, animation curves, logos, and tips are supplied by the project or UI bridge. Replacing them requires no modification to EchoSceneFlow runtime code.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| Initialization/authority status | API and inspector/log | Development and release-safe summary | Constant |
| Active request/phase/progress | API/events | Development; release-safe if project exposes | Bounded update rate |
| Queue state | API/events | Development and optional release | Bounded capacity |
| Current/previous scene IDs | API | Release-safe stable IDs | Constant |
| Timings and terminal result | API/history | Development and support | Bounded history |
| Validation report | Editor window/pre-Play/pre-build | Editor only | On demand |
| Diagnostic codes | Structured result/log | All builds | Event based |
| Backend/provider identity | Status | Development/support | Constant |
| Direct-scene mode | Status/log | Development; optional release warning | Constant |

### 15.2 Structured status

`SceneFlowStatus` includes at minimum:

- Package version.
- Authority state and root instance identity.
- Initialization mode.
- Configuration/catalog/profile IDs.
- Backend ID and availability.
- Current active scene stable ID/path availability.
- Previous successful scene stable ID.
- Active transition ID, route, source, destination, phase, elapsed time, and progress.
- Cancellation disposition.
- Queue policy, capacity, and count.
- Registered participant/presenter counts and selected presenter key.
- Recovery attempt state.
- Current warnings/errors.
- Last terminal result summary and timing.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| ESF-001 | Blocker | Configuration missing/invalid | Run setup/assign valid config |
| ESF-002 | Error | Duplicate root rejected | Remove duplicate canonical root |
| ESF-003 | Error | Unknown scene ID | Add/fix definition/catalog |
| ESF-004 | Error | Scene asset/path missing | Repair definition locator |
| ESF-005 | Error/Blocker | Scene unavailable in active Build Profile | Add/enable scene explicitly |
| ESF-006 | Info/Warning | Request rejected because busy | Handle result or choose queue policy |
| ESF-007 | Warning | Pending queue full | Reduce callers/increase bounded capacity intentionally |
| ESF-008 | Error | Required participant failed/timed out | Inspect participant ID and policy |
| ESF-009 | Warning/Error | Presenter failed/timed out | Inspect presenter; core forced cleanup |
| ESF-010 | Error | Backend could not start load | Verify destination/backend/platform |
| ESF-011 | Warning/Error | Activation gate timed out | Shorten gate/fix readiness condition |
| ESF-012 | Error | Required post-activation finalization failed | Inspect participant/recovery result |
| ESF-013 | Error | Fallback absent/invalid | Configure safe fallback or remove policy |
| ESF-014 | Blocker | Fallback loop detected | Break route cycle |
| ESF-015 | Info/Warning | Cancellation requested in unsafe phase | Do not promise mid-load cancellation |
| ESF-016 | Info | Direct-scene development initialization used | Use Boot path for canonical test when required |
| ESF-017 | Warning/Error | Configured route alias absent | Assign Main Menu/Hub route |
| ESF-018 | Blocker | Duplicate scene ID | Repair ID with migration plan |
| ESF-019 | Blocker | Duplicate route ID | Repair ID with migration plan |
| ESF-020 | Error | Unexpected transition exception | Export status/result and inspect phase |

### 15.4 Observatory bridge

A separate EchoSceneFlow–Observatory bridge registers one explicit provider. It maps public scene-flow status into neutral diagnostics:

- Authority health.
- Current/previous scene.
- Active route/destination and phase.
- Queue policy/count.
- Phase/overall progress.
- Phase and total timings.
- Participant/presenter/backend warnings.
- Last result and recovery status.

The bridge does not expose mutable service objects or make Observatory required.

### 15.5 Logging policy

- Every accepted transition receives a transition ID.
- Logs use package/category/transition/route/phase context.
- Expected busy or coalesced results are not errors.
- No per-frame logging or progress spam.
- Production logs prefer stable IDs and sanitized display labels; full project paths are development-only by default.
- Exceptions are converted into results and logged once at the owning boundary.
- Caller mistakes return actionable results instead of log-only behavior.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Scene/route/profile definitions | Project configuration | Project/EchoSceneFlow types | Unity asset serialization | Unity assets |
| Active transition/queue/progress | Session | EchoSceneFlow | No | Runtime memory |
| Current/previous scene record | Session | EchoSceneFlow | No by core | Runtime memory |
| Last results/history | Session/support | EchoSceneFlow | No by core | Bounded memory/export |
| Saved player location | Save slot | Project/EchoSave participant | Optional | EchoSave/project backend |
| Global transition visual preference | Global preference | EchoSettings/UI project integration | Optional | EchoSettings |

### 16.2 Standalone behavior

Without EchoSave, EchoSceneFlow travels normally and keeps only session state. It never creates a save file or assumes current scene should be durable.

### 16.3 Optional participant/provider contract

A separate EchoSave bridge or project adapter may:

- Register a required/optional pre-load participant.
- Request a project-approved save, flush, or checkpoint before travel.
- Read a stable destination/route ID for a save participant.
- Restore a saved location by resolving stable ID through the project catalog and then submitting a normal request.

The bridge does not allow EchoSceneFlow to inspect save payloads or select slots.

### 16.4 Failure and recovery

- Save participant failure before load follows its required/optional policy.
- Scene travel does not roll back a save that has already committed unless the save bridge explicitly owns a compensating policy.
- Missing saved scene IDs return a project/EchoSave migration or fallback decision; EchoSceneFlow only reports unknown ID.
- A newer save schema is not a scene-flow concern.
- Active transitions are not serialized or resumed after process restart.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

The source of truth stays with the owning package. EchoSceneFlow owns transition execution. Peers may request, present, observe, prepare, or react through explicit bridges. Installing a peer does not silently change core scene-flow policy.

### 17.2 Planned integrations

| Other authority | Connection | Bridge owner/placement | Direction/data | Required? |
|---|---|---|---|---:|
| EchoLaunch | Final startup transition/startup step | Separate two-package bridge | Launch initializes service and submits final route; result returns to launch report | No |
| EchoDiagnostics | Status provider | Separate two-package bridge | SceneFlow status/results → Observatory | No |
| EchoSettings | Optional presentation preference | UI/project bridge | Settings values influence presenter, not core route truth | No |
| EchoGameState | Loading-state coordinator | Separate two-package bridge | Transition lifecycle → state requests; state result/warnings back | No |
| Jukebot | Route audio mapping | Separate bridge | Route/scene IDs → Jukebot requests; playback status optional | No |
| EchoInput | Input-lock coordination | Usually GameState bridge/project adapter | Transition lifecycle → lock reason | No |
| EchoUI | Presenter/view-model | Separate two-package bridge or UI-owned integration | Context/progress/results ↔ loading/fade presentation | No |
| EchoSave | Transition participant | Separate two-package bridge/project policy | Pre-load save/flush and stable destination snapshot | No |
| EchoGameStarter | Editor composition | Workshop Editor integration | Generate config/catalog/root/routes/lab/report | No runtime dependency |
| EchoMultiplayer | Provider/authority coordinator | Later separate adapter | Server/session authority, synchronized request, late join | No |
| Project systems | Caller/participant/presenter | Project adapter | Route requests and explicit lifecycle work | No |

### 17.3 Bridge placement decisions

- First Light, Observatory, GameState, Jukebot, EchoUI, and EchoSave connections directly depend on two optional packages and therefore belong in separate bridge packages unless later compile/version analysis proves an owner-contained bridge cleaner.
- Input locking usually belongs to the GameState/Input integration rather than a direct SceneFlow dependency.
- Project-specific victory, respawn, world map, or route selection stays in project code.
- The Workshop owns Editor-time composition only.
- Addressables and multiplayer integrations are provider adapters, not core dependencies.

### 17.4 Initialization and late registration

EchoSceneFlow must initialize independently. Bridges may register later:

- Presenter registration affects future phases/requests only unless it explicitly adopts the active context safely.
- Participants registered after a request starts do not join that request by default.
- Observatory registration receives current status immediately.
- GameState/Jukebot/UI bridges receive future events and may query current status.
- First Light bridge initializes before final launch transition when configured.

### 17.5 Integration failure behavior

- Missing peer: bridge is absent; core remains Ready.
- Missing SceneFlow: peer bridge reports unavailable and does not create competing travel logic.
- Version mismatch: bridge refuses registration with actionable validation; cores remain independent.
- Peer initializes late: bridge receives current status and applies only safe current-state synchronization.
- Peer shuts down first: registration handle disposes and callbacks stop.
- Required bridge participant fails before load: transition rejects according to policy.
- Optional bridge fails: warning only; core continues.
- Circular request: a participant or presenter attempting a nested transition receives re-entry rejection.
- Removing bridge: core assemblies compile; scene/route assets remain valid.

### 17.6 First Light handoff contract

- First Light owns destination choice during startup.
- EchoSceneFlow owns the actual final scene transition only when the bridge is selected.
- The bridge maps First Light’s destination model to a `SceneDefinition` or route.
- EchoSceneFlow result/timings become one launch-step result and launch-report contribution.
- After successful handoff, normal travel requests go directly to EchoSceneFlow.
- First Light’s minimal internal loader remains available when EchoSceneFlow is not installed.
- No circular requirement exists: EchoSceneFlow does not require First Light to initialize.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Target | Measurement | Release threshold |
|---|---|---|---|
| Idle managed allocation | 0 B/frame after initialization | Profiler in Lab A | Required |
| Idle CPU | No meaningful recurring work beyond lightweight status/queue checks | Profiler | No sustained package spike |
| Request validation | Under 1 ms for catalog of 256 scenes on baseline desktop | Automated benchmark | Target; investigate regression |
| Progress callback rate | Configurable, default max 30 Hz | Lab/Profiler | No unbounded per-frame listener churn |
| Queue capacity | Default 0 under RejectNew; configured max default 4, hard validated upper bound 16 for MVP | Validation/test | No unbounded queue |
| Result history | Default 16, validated max 128 | Memory test | Bounded |
| Transition object lifetime | Released after terminal capture/handles | Memory/GC test | No growth across 100 transitions |
| Presenter/participant timeout | Configured and finite | Failure simulation | No permanent lock |

Scene loading cost is dominated by project scene content and Unity. The package measures and reports overhead but does not claim to make heavy scenes cheap.

### 18.2 Allocation policy

- No LINQ, reflection discovery, string formatting, or collection resizing in the progress hot path.
- Preallocate bounded queue/history collections from configuration.
- Reuse internal progress/state buffers where safe while publishing immutable snapshots.
- Do not pool public result objects in a way that lets data mutate after publication.
- Avoid per-frame logs and closures.
- Presenter and bridge allocations are measured separately from core.

### 18.3 Scene and domain reload behavior

- Root unsubscribes from Unity scene events and disposes registrations on shutdown/destruction.
- Static convenience access resets through subsystem registration and domain-reload-safe hooks.
- Enter Play Mode with domain reload disabled must not preserve stale authority, queue, participant, presenter, or history state.
- Direct initializer uses the same claim/reset path.
- Scene callbacks are used as verification signals, not the sole source of transition truth.
- Unexpected external scene loads update status as ExternalSceneChange warnings without pretending EchoSceneFlow requested them.

Unity documents `SceneManager.sceneLoaded` as occurring after `OnEnable` and before `Start` for scene objects. EchoSceneFlow does not promise that all destination `Start` methods have run when the scene-loaded callback fires. Required project warmup that must precede reveal belongs in explicit post-activation participants.

Reference: [Unity `SceneManager.sceneLoaded`](https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html)

### 18.4 Scalability limits

MVP advertised/tested limits:

- 256 scene definitions.
- 512 route definitions.
- 16 pending requests hard maximum.
- 128 registered participants hard maximum, with practical projects expected to use far fewer.
- One active presenter key.
- One active scene operation.
- 128 result-history entries maximum.

Exceeding configured limits returns validation or admission failures. It does not grow collections without bound.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

EchoSceneFlow handles local project scene metadata, runtime route IDs, internal scene paths, timings, and errors. It does not handle credentials, personal data, analytics, network traffic, or save payloads.

Full asset paths may reveal project structure in support reports. Release-safe diagnostics default to stable IDs/display labels and redact full paths unless explicitly enabled for development/support.

### 19.2 Trust boundaries

- External strings, command-line values, save data, network messages, or user-authored content cannot become a runtime scene path directly.
- They must resolve through an approved stable ID/catalog or provider adapter.
- A caller cannot bypass catalog validation using a public raw path API.
- Presenter/participant implementations are trusted project code but are isolated by timeout, exception conversion, and registration lifetime.
- Support snapshots are local and explicit; nothing is transmitted automatically.

### 19.3 Platform behavior

| Platform | Supported? | Special behavior | Validation required |
|---|---:|---|---|
| Windows | Yes | Built-in async scene backend; desktop development baseline | Clean build and stress lab |
| macOS | Planned supported | Same semantic contract; timing/content differs | Clean build and lab |
| Linux | Planned supported | Same semantic contract | Clean build and lab |
| WebGL | Planned supported | No background-thread assumption; scene load timing/browser suspension differs | WebGL build and lifecycle test |
| Mobile | Planned supported | App pause/background during load; memory pressure | Device lifecycle and recovery test |
| Console | Planned/unknown | Platform certification, memory, suspend/resume policies | Provider/platform validation before claim |

Core behavior degrades through explicit backend capability reporting. Unsupported provider features are unavailable, not silently simulated.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-scene-flow/
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
│   ├── API/
│   ├── Core/
│   ├── Configuration/
│   ├── Data/
│   ├── Backend/
│   ├── Participants/
│   ├── Presentation/
│   ├── Diagnostics/
│   ├── DirectScene/
│   ├── Prefabs/
│   └── EchoDevGames.EchoSceneFlow.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Inspectors/
│   ├── Simulation/
│   ├── Migration/
│   └── EchoDevGames.EchoSceneFlow.Editor.asmdef
├── Samples~/
│   └── Standalone Labs/
│       └── The Passage Lab/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Proposed source tree

```text
Runtime/
├── API/
│   ├── ISceneFlowService.cs
│   ├── ISceneLoadBackend.cs
│   ├── ISceneTransitionParticipant.cs
│   ├── ISceneTransitionPresenter.cs
│   ├── SceneTransitionRequest.cs
│   ├── SceneTransitionHandle.cs
│   ├── SceneTransitionResult.cs
│   ├── SceneTransitionProgress.cs
│   └── SceneFlowStatus.cs
├── Core/
│   ├── EchoSceneFlowRoot.cs
│   ├── SceneFlowService.cs
│   ├── SceneTransitionRunner.cs
│   ├── SceneTransitionQueue.cs
│   ├── SceneResolver.cs
│   ├── SceneRequestAdmissionService.cs
│   └── SceneRecoveryController.cs
├── Configuration/
│   ├── EchoSceneFlowConfiguration.cs
│   ├── SceneTransitionProfile.cs
│   └── SceneFlowDevelopmentConfiguration.cs
├── Data/
│   ├── SceneDefinition.cs
│   ├── SceneCatalog.cs
│   ├── SceneRouteDefinition.cs
│   └── SceneStableId.cs
├── Backend/
│   ├── UnitySceneManagerBackend.cs
│   ├── UnitySceneOperation.cs
│   └── SceneBackendCapabilities.cs
├── Participants/
│   ├── SceneTransitionParticipantDescriptor.cs
│   └── SceneFlowRegistration.cs
├── Presentation/
│   └── ScenePresenterContracts.cs
├── Diagnostics/
│   ├── SceneFlowDiagnosticCodes.cs
│   └── SceneTransitionHistory.cs
├── DirectScene/
│   └── EchoSceneFlowDirectInitializer.cs
└── Prefabs/
    └── EchoSceneFlowRoot.prefab
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoSceneFlow.Runtime` | Runtime | Unity core/scene modules only | Yes | Public API and runtime authority |
| `EchoDevGames.EchoSceneFlow.Editor` | Editor | Runtime, UnityEditor | No | Setup, inspectors, validation, migration, simulation |
| `EchoDevGames.EchoSceneFlow.Tests.Editor` | Editor tests | Runtime, Editor, Test Framework | No | Pure validation/data/tooling tests |
| `EchoDevGames.EchoSceneFlow.Tests.Runtime` | PlayMode tests | Runtime, Test Framework | No | Lifecycle/backend/queue/failure tests |
| `EchoDevGames.EchoSceneFlow.Samples.PassageLab` | Sample | Runtime, project uGUI/TMP | No | Standalone Lab only |

### 20.4 Repository files

- Concise README with package boundaries and five-minute setup.
- Documentation index visibly linking Current Notes.
- Package specification and architecture overview.
- User setup, scene/route authoring, Test Lab, troubleshooting, and diagnostic-code guides.
- Developer API, backend, participant, presenter, testing, and migration guides.
- Changelog.
- License and third-party notices.
- Contribution/security/support guidance appropriate to public release.
- Release checklist.
- Stable `.meta` files for public scripts, definitions, prefab, and samples.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Primary tested | Notes |
|---|---|---|---|
| Unity | 6000.0 | 6000.3.8f1 | Additional Unity 6 versions listed only after validation |
| Unity scene management | Included with supported Unity | 6000.3.8f1 | Built-in MVP backend |
| Unity Test Framework | Compatible Unity 6 package | Project baseline | Tests only |
| uGUI/TextMeshPro | Not required by core | Project baseline | Sample/optional integration only |

### 21.2 Semantic versioning policy

- **Patch:** Bug fixes, diagnostics, validation improvements, internal performance changes, and non-breaking documentation corrections.
- **Minor:** Backward-compatible request/result fields, new validation rules, new optional policies, additive-scene module when public contracts remain compatible, or new adapters.
- **Major:** Breaking public API, route/scene/profile schema incompatibility, changed transition-phase semantics, changed default admission/recovery contract, stable-ID format break, or migration removal.

### 21.3 Deprecation policy

- Public API and serialized-field deprecations receive compiler/inspector warnings and migration guidance for at least one supported minor line where practical.
- Old route/scene/profile schemas migrate through explicit Editor tooling.
- Removed diagnostic codes remain documented as historical aliases when useful.
- A default-policy change that can alter runtime navigation requires major version review even if method signatures remain.

### 21.4 GUID and asset compatibility

- Public scripts, prefab, templates, and samples preserve committed `.meta` GUIDs.
- Project-owned asset GUIDs are never regenerated by package update.
- Moves/renames preserve GUIDs.
- Stable scene and route IDs remain independent from asset GUIDs but must also be preserved.
- Migration tooling reports identity changes and supports aliases where released content depends on them.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview and authority boundaries.
- Installation routes.
- Five-minute quick start.
- Full setup and root configuration.
- Scene definition and catalog authoring.
- Route and transition-profile authoring.
- Main Menu, Hub, and reload helpers.
- Presenter-free use.
- Standalone Test Lab guide.
- Direct-scene testing guide.
- Failure/recovery and diagnostic-code reference.
- Build Profile validation and troubleshooting.
- Upgrade/migration guide.
- Optional integration index.
- Known limitations, including no true mid-load cancellation.
- License, credits, and third-party notices.

### 22.2 Required developer documentation

- Authority/root/lifecycle architecture.
- Transition phase and progress semantics.
- Request admission/coalescing/queue policy.
- Backend abstraction and Unity backend limitations.
- Participant and presenter contracts.
- Cancellation, activation, timeout, and recovery rules.
- Stable scene/route identity and migration.
- Diagnostics/status schema.
- Test injection and automated testing.
- Bridge ownership/direction.
- Release workflow and architecture decisions.
- Current checkpoint/status and linked Current Notes.

### 22.3 Documentation truth rule

- Code examples compile against the documented release.
- Unity menu paths and Build Profile terminology match the supported Unity baseline.
- Progress documentation explains the backend’s pre-activation behavior instead of presenting raw `0.9` as completion.
- Cancellation documentation clearly identifies safe and unsafe phases.
- Samples never imply that uGUI/TMP is a core runtime dependency.
- A feature is not release-ready when its route, setup, result, diagnostic, or Test Lab documentation is stale.

### 22.4 Living repository and Obsidian workflow

Documentation lives in Git with implementation. Obsidian opens the same Markdown files directly. `Current Notes.md` captures provisional observations, tests, risks, and handoff state. At each meaningful checkpoint:

1. Review current notes.
2. Promote durable behavior/API decisions into this specification or an ADR.
3. Move defects/test evidence into permanent records.
4. Update guides/changelog for user-visible behavior.
5. Update checkpoint status and next action.
6. Condense resolved notes.
7. Commit documentation with or immediately adjacent to implementation.

### 22.5 Repository scan and handoff order

1. Repository README/index.
2. SFGSS-000.
3. This EchoSceneFlow specification.
4. Applicable ADRs/bridge specifications.
5. Current Notes.
6. Current checkpoint, tests, issue log, and changelog.
7. Relevant runtime, Editor, and test code.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, catalogs, routes, loops, profiles, admission policy, validation | Duplicate IDs, stale path, phase weights, fallback cycle | Yes |
| PlayMode unit/integration | Root claim, queue, runner, fake backend, participants, presenter, cancellation | Duplicate root, coalescing, cleanup, timeout | Yes |
| Standalone Test Lab | User-visible isolated multi-scene core loop | A/B travel, reload, queue, fallback, direct entry | Yes |
| Bridge Integration Lab | Optional package connection | First Light, Pulse, UI, Save, Jukebot, Observatory | When bridge ships |
| Showcase | Combined application shell | Full startup/menu/loading/audio/save flow | No |
| Clean-project install | Packaging/missing-dependency proof | Local, tgz, Git install | Yes |
| Existing-project migration | Replace project loader without regression | Echo Systems Lab, Rescuers2D | Before adoption claim |

### 23.2 Required test categories

- Happy-path initialization and transition.
- Missing/invalid configuration.
- Empty catalog.
- Missing/disabled Build Profile scene.
- Duplicate scene/route IDs.
- Duplicate roots before Play Mode and during scene load.
- Direct-scene entry.
- Request coalescing.
- Reject, queue, replace-pending, and queue-full admission.
- Queued/pre-load cancellation.
- Unsafe-phase cancellation report.
- Participant success, optional failure, required failure, timeout, disposal, and re-entry rejection.
- Presenter absent, success, failure, timeout, and forced cleanup.
- Activation immediate and bounded-gate timeout.
- Fallback success, missing fallback, and loop detection.
- Reload and configured route helpers.
- External scene change observation.
- Enter Play Mode with domain reload enabled/disabled.
- Application quit/shutdown during idle, queued, pre-load, and active-load phases.
- Sample removal.
- Optional integrations absent/present.
- Clean install, upgrade, reinstall, tarball, and removal.
- Performance and 100-transition stress loop.

### 23.3 Test case registry

| Test ID | Requirement | Setup | Action | Expected result | Automated? | Status |
|---|---|---|---|---|---:|---|
| ESF-T-001 | Authority claim | Two roots | Enter Play | First claims; duplicate no side effects | Yes | Not run |
| ESF-T-002 | Missing config | Root null config | Initialize | NotReady and ESF-001 | Yes | Not run |
| ESF-T-003 | Valid catalog | Two definitions | Validate | Pass | Yes | Not run |
| ESF-T-004 | Duplicate scene ID | Two same IDs | Validate | Blocker ESF-018 | Yes | Not run |
| ESF-T-005 | Duplicate route ID | Two same IDs | Validate | Blocker ESF-019 | Yes | Not run |
| ESF-T-006 | Fallback loop | A→B→A | Validate | Blocker ESF-014 | Yes | Not run |
| ESF-T-007 | Happy transition | Fake/Unity backend valid | Request B | Completed result and state update | Yes | Not run |
| ESF-T-008 | Duplicate coalescing | Active B | Request B again | Same handle/one backend call | Yes | Not run |
| ESF-T-009 | Busy rejection | Active B/Reject | Request A | Rejected ESF-006 | Yes | Not run |
| ESF-T-010 | FIFO queue | Active B/Queue | Request A then B | Ordered execution | Yes | Not run |
| ESF-T-011 | Queue full | Capacity reached | Submit extra | ESF-007 | Yes | Not run |
| ESF-T-012 | Replace pending | Active plus pending | Replace request | Pending terminal Cancelled/Replaced; active unchanged | Yes | Not run |
| ESF-T-013 | Cancel queued | Queued request | Cancel | Removed/Cancelled | Yes | Not run |
| ESF-T-014 | Cancel prepare | Delayed participant | Cancel | Unwind/no backend start | Yes | Not run |
| ESF-T-015 | Cancel loading | Backend started | Cancel | Unsafe-phase result; backend continues | Yes | Not run |
| ESF-T-016 | Required participant fail | Required prepare participant | Request | Failed before load | Yes | Not run |
| ESF-T-017 | Optional participant fail | Optional participant | Request | CompletedWithWarnings | Yes | Not run |
| ESF-T-018 | Participant timeout | Never completes | Request | ESF-008 and cleanup | Yes | Not run |
| ESF-T-019 | Re-entry | Participant requests transition | Execute | Nested rejected | Yes | Not run |
| ESF-T-020 | No presenter | None registered | Request | Completed | Yes | Not run |
| ESF-T-021 | Presenter fail out | Failing presenter | Request | Source revealed/lock released | Yes | Not run |
| ESF-T-022 | Presenter fail in | Destination active | Reveal fails | Result warning/error; lock released | Yes | Not run |
| ESF-T-023 | Activation timeout | Gate never opens | Request | Force activation or configured terminal result | Yes | Not run |
| ESF-T-024 | Fallback success | Primary post-load failure | Request | Recovered result/fallback active | Yes | Not run |
| ESF-T-025 | Fallback invalid | Missing fallback | Trigger recovery | ESF-013 final failure | Yes | Not run |
| ESF-T-026 | Reload | Current definition valid | Reload | New explicit operation | Yes | Not run |
| ESF-T-027 | Main Menu absent | Alias null | Helper | ESF-017 | Yes | Not run |
| ESF-T-028 | Direct initializer | No root | Enter lab scene | One marked development root | Yes | Not run |
| ESF-T-029 | Domain reload off | Editor option | Re-enter Play | No stale static/queue state | Manual/Yes | Not run |
| ESF-T-030 | External load | Direct Unity load in test | Observe | Status warning; no false request result | Yes | Not run |
| ESF-T-031 | 100 transitions | Fake/light scenes | Loop | No history/handle/registration growth | Yes | Not run |
| ESF-T-032 | Sample removal | Delete Samples | Compile | Runtime succeeds | Manual | Not run |
| ESF-T-033 | Clean tgz install | Blank project | Install/import lab | Compile and lab pass | Manual | Not run |
| ESF-T-034 | Bridge absent | Core only | Compile/run | No missing references | Yes | Not run |
| ESF-T-035 | Build Profile validation | Scene disabled | Validate/build | Blocker before build | Yes/manual | Not run |
| ESF-T-036 | Stale scene path repair | Move Scene asset with recorded source GUID | Run repair | Path updates, stable ID unchanged | Yes/manual | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and non-ownership are approved.
- [x] MVP and deferred scope are separated.
- [x] Required dependencies are explicit.
- [x] Public API and data model are defined.
- [x] Transition lifecycle, admission, cancellation, activation, and recovery are defined.
- [x] Standalone Test Lab is designed.
- [x] Release-blocking design questions are resolved.
- [x] Implementation remains locked by Foundation documentation gate.

### 24.2 Implementation gate

- [ ] Runtime code compiles with declared Unity dependencies only.
- [ ] Editor code is isolated from runtime.
- [ ] Root duplicate protection occurs before side effects.
- [ ] Scene/route/profile definitions preserve stable IDs and GUIDs.
- [ ] Unity backend obeys one-operation and activation-gate rules.
- [ ] Setup/repair is repeatable and non-destructive.
- [ ] Public API matches this specification or specification/ADR changes first.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Core works without other Echo packages.
- [ ] All Passage Lab scenes/checklist pass.
- [ ] Samples can be removed safely.
- [ ] Direct-scene entry behaves as documented.
- [ ] No uGUI/TMP dependency leaks into core runtime.

### 24.4 Quality gate

- [ ] Automated EditMode/PlayMode tests pass.
- [ ] Manual checklist passes.
- [ ] No blocker/critical defect remains.
- [ ] Performance/allocation/history/queue targets pass.
- [ ] All terminal paths release transition locks.
- [ ] Diagnostics are actionable.
- [ ] Documentation matches build.
- [ ] Current Notes reconciled.
- [ ] Durable decisions promoted.
- [ ] Licenses/notices complete.

### 24.5 Distribution gate

- [ ] Package manifest valid.
- [ ] Version/changelog updated.
- [ ] Stable `.meta` files included.
- [ ] Local/tarball/Git install tested externally.
- [ ] Upgrade/reinstall/removal tested.
- [ ] Repository tag/release prepared.
- [ ] Documentation/status committed and pushed.
- [ ] Suite compatibility catalog updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Echo Systems Lab | `GameSceneLoader` and Hub/Trial flow | Install Passage standalone, recreate routes, replace one caller at a time | Hub→Trial→Hub, reload, failure, progress, no save regression | Restore original loader/callers |
| Rescuers2D | `SceneLoadService`, Main/Pause/results travel | Keep original service, map project destinations, bridge one menu path at a time | Existing menu/continue/password flow unchanged; duplicate roots absent | Re-enable original service/prefab |
| Don’t Get Vince’d | Jam scene/results logic | Add package after standalone proof and replace direct calls incrementally | Level/results/menu flow parity | Restore prior scripts |
| Future Hackulos | New application shell | Use approved route/catalog model from start | Top-down zone/menu travel in isolated project | Remove package and use project loader |

### 25.2 Preserve-until-parity rule

Existing working scene services remain available until EchoSceneFlow passes:

1. Standalone Lab.
2. Clean-project installation.
3. One real-project route.
4. Full project travel parity.
5. Save/state/UI/audio regression checks.
6. Rollback rehearsal.

No project-specific scene rules are copied into package source.

### 25.3 Migration tooling

MVP migration tooling may:

- Scan project scripts/prefabs/scenes for obvious direct `SceneManager.LoadScene` references and report them without rewriting code automatically.
- Import a user-selected list of Scene assets into definitions/catalog.
- Create route assets from user-approved destination mappings.
- Preview replacement of serialized scene-name fields where a safe known adapter exists.
- Preserve original scenes/prefabs/scripts and create backups for transformed project assets.
- Produce a migration checklist and unresolved-call report.

Automatic arbitrary code rewriting is not approved.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| R-001 | Scope expands into a universal scene/world framework | High | High | MVP single-load only; defer additive/persistent sets | Specification review |
| R-002 | Raw strings leak into public API | Medium | High | Stable definitions/routes; validator; no production raw-path method | API review |
| R-003 | Duplicate persistent roots start side effects | Medium | High | Claim before subscriptions/queue/backend | Lifecycle tests |
| R-004 | Queue executes stale navigation intent | Medium | High | Default RejectNew, bounded queue, replace pending, visible status | Admission tests |
| R-005 | Caller assumes mid-load cancellation | High | High | Explicit phase contract/results/docs | API/docs tests |
| R-006 | Activation gate stalls Unity async operations | Medium | High | Immediate default, bounded gate, hard timeout, one operation | Backend tests |
| R-007 | Presenter failure leaves black screen | Medium | High | ForceCleanup, timeout, no presenter valid, result code | Lab failure tests |
| R-008 | Participant deadlock blocks travel | Medium | High | Finite timeout, required/optional policy, cancellation, re-entry rejection | Participant tests |
| R-009 | Fallback loops forever | Low | Critical | One attempt, visited route set, validation | Recovery tests |
| R-010 | Scene path changes break assets | Medium | Medium | SceneAsset-based Editor sync, stable IDs, validation/repair | Editor tests |
| R-011 | Build Profile API/terminology drifts | Medium | Medium | Unity 6 version tests and isolated Editor adapter | Unity upgrade review |
| R-012 | SceneFlow absorbs UI/GameState/Save/Audio | Medium | High | Explicit bridges and ownership matrix | Cross-spec review |
| R-013 | Unity backend errors lack detail | Medium | Medium | Strong preflight, phase/context diagnostics, provider result abstraction | Backend tests |
| R-014 | External direct loads desynchronize status | Medium | Medium | Observe Unity events, mark external change, document unsupported bypass | Lifecycle tests |
| R-015 | Mutable runtime state contaminates assets | Low | High | Runtime operation objects only, ScriptableObject safety tests | Code review |
| R-016 | Additive future API conflicts with MVP | Medium | Medium | Backend/request abstractions leave seam; do not expose premature types | Later design workshop |
| R-017 | Sample becomes production dependency | Low | High | Separate sample assembly; removal gate | Package test |
| R-018 | Performance history/queue grows unbounded | Low | Medium | Validated fixed capacities | Stress test |
| R-019 | Direct-scene helper ships enabled | Medium | Medium | Build validation and development define/default | Distribution gate |
| R-020 | Existing project migration breaks flow | Medium | High | Preserve-until-parity and reversible caller-by-caller adoption | Integration owner |
| R-021 | Stable ID changes orphan saves/routes | Low | High | ID immutability, aliases, migration report | Maintainer |
| R-022 | Shutdown during active load leaves static state | Medium | Medium | Explicit non-cancellable shutdown policy/finally cleanup/domain tests | Lifecycle tests |
| R-023 | Caller blocks progress event | Medium | Medium | Bounded callback policy, docs, diagnostics | Performance tests |
| R-024 | Provider adapter bypasses core safety | Medium | High | Backend contract conformance suite | Adapter release gate |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| ESF-D-001 | EchoSceneFlow owns normal scene travel after First Light handoff | Approved | One clear authority | Startup and travel remain separate | No |
| ESF-D-002 | Runtime destinations use stable SceneDefinition/RouteDefinition assets, not public raw strings | Approved | Validation and durable identity | Editor locator synchronization required | No |
| ESF-D-003 | One duplicate-safe application-session root serializes scene operations | Approved | Prevent races and survive single loads | Root owns queue/backend/history | No |
| ESF-D-004 | Unity SceneManager asynchronous single-load backend is MVP | Approved | Smallest useful platform-native path | Additive/providers deferred | No |
| ESF-D-005 | Public async operations use fresh Unity `Awaitable<T>` instances | Approved | Foundation Unity 6 consistency | Main-thread boundaries documented | No |
| ESF-D-006 | Default admission is RejectNew; optional FIFO queue is bounded; active load is never replaced | Approved | Safety over stale navigation | Callers handle structured Busy/QueueFull | No |
| ESF-D-007 | Equivalent active/queued requests coalesce | Approved | Double-click protection | Explicit reload bypasses equivalence | No |
| ESF-D-008 | Cancellation is cooperative before backend load and not promised afterward | Approved | Honest Unity limitation | Result exposes unsafe-phase request | No |
| ESF-D-009 | Immediate activation is default; delayed activation is short and hard-bounded | Approved | Avoid async queue stall | Presenter minimum duration cannot be indefinite | No |
| ESF-D-010 | Presenter and participants register explicitly through disposable handles | Approved | Visible optional behavior/removal | No reflection discovery | No |
| ESF-D-011 | Core runtime is nonvisual and has no uGUI/TMP dependency | Approved | Package independence | Sample/UI bridge supplies presentation | No |
| ESF-D-012 | Recovery attempts one configured fallback with loop protection | Approved | Useful safe failure without recursion | Final result records parent/recovery | No |
| ESF-D-013 | Main Menu/Hub helpers resolve configured route assets | Approved | Convenience without hidden scene names | Missing alias returns structured result | No |
| ESF-D-014 | Additive load/unload and persistent scene sets are deferred | Approved | Need separate ownership/lease design | MVP remains small | No |
| ESF-D-015 | Direct-scene initializer is development-only by default | Approved | Fast testing without second production bootstrap | Build validation required | No |
| ESF-D-016 | Full project paths are development diagnostics, not release-default support output | Approved | Avoid leaking project structure | Stable IDs used in release-safe status | No |
| ESF-D-017 | No SFGSS-000 change is required for this package specification | Approved | Decisions refine existing SceneFlow authority | Promote only into this Level 2 spec | No |

### 27.2 Release-blocking questions

None. Jesse delegated implementation-shaping choices to the most durable long-term architecture, and the decisions above resolve the MVP contract.

### 27.3 Non-blocking later questions

- Exact additive scene lease/ownership model.
- Whether additive and persistent-set features remain one package or a module.
- First provider adapter after built-in SceneManager, potentially Addressables.
- Dedicated loading-shell scene topology.
- Memory cleanup/unload-unused-assets policy and ownership.
- Multiplayer synchronized scene-flow provider contract.
- Route graph visualization scope.
- Platform-specific background/suspend behavior after device testing.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 – Specification | Approved package contract | This document | Approved v1.0.0 |
| M1 – Skeleton | Installable package anatomy | Manifest, assemblies, docs shell, root/config stubs | Clean compile |
| M2 – Data and validation | Stable definitions/catalog/routes/profiles | IDs, resolver, Editor inspectors/validator | EditMode tests |
| M3 – Runtime core | One async serialized transition | Root, service, backend, runner, progress/results | PlayMode tests |
| M4 – Admission and failure | Coalescing, queue, cancel, participants, presenter, recovery | Complete terminal paths | Automated failure suite |
| M5 – Test Lab/tooling | Isolated multi-scene proof | Setup, simulation, Lab, direct entry | Manual/automated checklist |
| M6 – Integration/adoption | First bridge and real-project migration | First Light bridge plus one project | Integration Lab/parity report |
| M7 – Release | Distribution-ready version | Docs, licenses, package, clean install | External tgz/Git validation |

### 28.2 Checkpoint rule

Each milestone is divided into small SFGSS-005 Checkpoint Build Plans. Every checkpoint names exact scope, files, Editor setup, tests, completion criteria, rollback, documentation updates, commit, and push. No implementation checkpoint starts until FW-DOC-12 passes.

### 28.3 First recommended checkpoint after documentation gate

`EchoSceneFlow M1-01 – Package Skeleton and Assembly Isolation`:

- Create package manifest and folder anatomy.
- Create Runtime/Editor/Test asmdefs.
- Create documentation shell and Current Notes.
- Add no functional scene travel beyond compile-safe type placeholders approved by the checkpoint.
- Validate clean install, Editor/runtime separation, GUID/meta stability, and sample absence.

This is not the suite’s first expected implementation checkpoint. First Light M1 remains expected first unless FW-DOC-12 changes the sequence.

---

## 29. New-Conversation Handoff

```text
We are continuing development of The Sperk’s Forge – EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the authority for suite-wide boundaries and architecture.
Treat The Passage – Scene Flow Specification v1.0.0 as the authority for
EchoSceneFlow behavior, public API, data model, transition lifecycle, tooling,
Test Lab, diagnostics, and release gates. Follow SFGSS-005 for implementation
checkpoints after the Foundation documentation gate is approved.

Current package: EchoSceneFlow
Current specification version: 1.0.0 Approved
Current milestone/checkpoint: <CHECKPOINT>
Current Unity version: 6000.3.8f1
Current project/repository: <PROJECT>
Current implementation status: Not started unless later status says otherwise
Known blockers: <BLOCKERS>

Before writing code:
1. Summarize scene-flow ownership and independence constraints.
2. Preserve stable scene/route identity and the one-active-operation rule.
3. Do not promise cancellation after the backend load begins.
4. Keep UI, state, save, audio, input, and multiplayer behind explicit bridges.
5. Preserve existing project loaders until migration parity passes.
6. Continue using the Checkpoint Build Plan format.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | Specification 1.0.0; runtime package not created |
| Completed checkpoint | FW-DOC-04 – Package specification approved |
| Files/assets created | This Markdown specification and updated planning checkpoint only |
| Tests passed | Documentation structure/consistency review only; no runtime tests |
| Tests failed | None; implementation not started |
| Known issues | None blocking; suite licensing remains a later release decision |
| Decisions added | ESF-D-001 through ESF-D-017 |
| Next suite checkpoint | FW-DOC-05 – Draft The Pulse (`EchoGameState`) specification |
| Implementation authorization | Locked until FW-DOC-12 |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Package identity and plain responsibility are clear.
- [x] Ownership/non-ownership align with SFGSS-000.
- [x] Independence proof is credible.
- [x] MVP is small enough to complete and useful enough to ship.
- [x] Scene/route identity, runtime state, public API, and lifecycle are specified.
- [x] Admission, queueing, cancellation, activation, failure, and recovery are explicit.
- [x] Setup and direct-scene workflows are understandable.
- [x] Standalone multi-scene Test Lab is fully defined.
- [x] Diagnostics exist without The Observatory.
- [x] Optional integrations are separated.
- [x] Test and release gates are measurable.
- [x] No Isekai Studios identity or ownership is introduced.
- [x] No unresolved release-blocking question remains.
- [x] Jesse delegated implementation-shaping choices and authorized the documentation pass to continue.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams  
**Date:** August 3, 2026  
**Conditions or notes:** This specification is authoritative for package design. No runtime implementation begins until all ten Foundation specifications and FW-DOC-11/FW-DOC-12 consistency gates are approved.

---

## Specification Completion Check

A new collaborator can answer from this page:

1. EchoSceneFlow owns normal scene-transition execution after First Light handoff.
2. It refuses startup, UI, save, audio, state, input, multiplayer, and gameplay-rule authority.
3. Its MVP is a validated, asynchronous, single-scene, one-operation pipeline with progress, locking/queueing, participants, presentation hooks, and recovery.
4. It works alone through its own root, backend, configuration, diagnostics, and Test Lab.
5. Scene/route/profile assets are definitions; active request, queue, progress, backend operation, and history are runtime state.
6. Public API uses typed definitions/requests, structured handles/results, explicit registration, and fresh Unity Awaitables.
7. Failures produce stable codes, terminal cleanup, and at most one fallback attempt.
8. The package is configured and proven through an isolated multi-scene Lab.
9. Optional packages connect through removable bridges.
10. Release requires clean installation, lifecycle/failure tests, real-project parity, complete docs, and distribution validation.

The specification is therefore complete and **Approved**.


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

**Package-specific repairs:**

- Confirmed `SceneId` as the durable runtime identity, separate from Editor source GUID/path metadata.

## Graph Navigation

#sfgss/package #sfgss/wave/foundation #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
