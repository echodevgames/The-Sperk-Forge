# The Looking Glass – UI Framework Package Specification

**Working document ID:** SFGSS-PKG-ECHOUI-001
**Specification version:** 1.8.0
**Status:** Approved
**Technical package name:** EchoUI
**Public title:** The Looking Glass – UI Framework
**Package ID:** `com.echodevgames.echo-ui`
**Runtime namespace:** `EchoDevGames.EchoUI`
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Project boundary:** Independent solo project; not an Isekai Studios product
**Planned repository:** `EchoDevGames/EchoUI`
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`
**Unity baseline:** Unity 6000.3.8f1
**Minimum supported Unity version:** Unity 6000.0
**Required Unity package:** Unity UI (`com.unity.ugui`) `2.0.0`, verified from the Unity 6000.3.8f1 project manifest during PKG-LEARN-008
**Default text path:** uGUI-compatible project-owned text components; no separate text package dependency is required by EUI-M1-01
**Parent authority:** SFGSS-000 and SFGSS-001
**Last updated:** August 17, 2026

> “Let the game be seen clearly without mistaking the reflection for the world.”

> **Approval rule:** This specification is the package authority. PKG-LEARN-008 is complete through the bounded EUI-M4-02 JIT revisit. EUI-M1 through EUI-M4-01 are complete. EUI-M4-02 is package-locally ACTIVE / AUTHORIZED under SFGSS-005 from clean EUI-M4-01 closeout baseline `5e7ad92`; Runtime/root/presenter implementation is automated-green through `d93d0bd`, and Laboratory manual acceptance remains pending.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-03 | Proposed | Initial complete specification based on SFGSS-000 v0.6.0, SFGSS-001 v1.1.0, and seven approved Foundation specifications | Pending |
| 1.0.0 | 2026-08-03 | Approved | Approved the persistent root, UI layers, screen/modal lifecycle, HUD regions, notifications, prompts, focus/EventSystem coordination, view-presenter boundaries, motifs, accessibility seams, diagnostics, tooling, and isolated UI Laboratory | Jesse “Echo” Adams |
| 1.0.1 | 2026-08-04 | Approved | Normalized registry metadata and formal title; added the SUITE-DOC-30 governing-authority, evidence, test-registry, and compatibility clarification without authorizing implementation. | Jesse “Echo” Adams |
| 1.1.0 | 2026-08-13 | Approved | JIT architecture rebaseline: layered UI context, optional navigation scopes, independent windows, stable surface IDs/registry, hybrid Back/navigation, cascading visibility policy, input-aware default selection, Motif terminology, Lego-style primitives/Builder direction, project-owned lifetime/context authority, and EUI-M1-01 foundation activation. | Jesse “Echo” Adams |
| 1.2.0 | 2026-08-13 | Approved | EUI-M1-02 JIT reconciliation: designer-ordered per-surface external context responses, project-defined stable active/inactive context IDs, independent visibility/interactability/selection directives, no-intervention defaults, authored/local/runtime overrides, optional external participation, and externally supplied input-modality selection policy. Activates EUI-M1-02 while keeping presets, Motifs, Builder, MMO layout persistence, arbitrary context payloads, and peer bridges outside the slice. | Jesse “Echo” Adams |
| 1.3.0 | 2026-08-14 | Approved | EUI-M2-01 JIT reconciliation: replaces the fixed seven-layer runtime assumption with project-defined ordered layer topology plus convenience starter defaults; confirms RootOwned/SceneOwned/ExternalOwned screen ownership; defines designer-controlled suspended-screen visibility with scope-enforced non-interaction; locks bounded strict FIFO screen mutation ordering; and activates the screen-lifecycle-only M2-01 slice while deferring modal exact-once results to M2-02. | Jesse “Echo” Adams |
| 1.4.0 | 2026-08-14 | Approved | EUI-M2-02 JIT reconciliation: activates blocking modal lifecycle with stacked top-only interaction, project-defined stable result IDs, first-terminal-wins exact-once completion, structural Aborted outcomes, RootOwned/SceneOwned/ExternalOwned modal ownership, fresh awaiters, designer-authored Back dismissal, UI-only blocking that does not own gameplay input, and configurable Reject/Defer screen-mutation behavior while a blocking modal stack is active. Visual backdrop styling, full focus restoration, transitions, HUD/transients, Motifs, Builder, persistence, and peer bridges remain later slices. | Jesse “Echo” Adams |
| 1.4.1 | 2026-08-14 | Approved | Post-activation clarification: blocking Modal semantics apply only to the blocking Modal lifecycle, not to independent Window surfaces. Independent Windows remain non-blocking/coexistent by default. Future Back/Escape window dismissal uses a separate most-recent-eligible (LIFO) history with authored/runtime pin exclusions; this is distinct from M2-01 FIFO operation execution and remains outside EUI-M2-02 implementation. | Jesse “Echo” Adams |
| 1.5.0 | 2026-08-15 | Approved | EUI-M3-01 JIT reconciliation: explicit/non-destructive EventSystem coordination; per-entry focus memory with optional transient stable-surface session memory; policy-aware restoration/fallback/no-focus; pointer/navigation behavior; blocking-Modal focus containment; independent Window focus memory without a full Window manager; event-driven focus maintenance with explicit revalidation; stale-request protection; and optional use of the suite Unity-default input compatibility profile without transferring input ownership. Activates EUI-M3-01 while transitions, Motifs/accessibility presentation, HUD/transients, Window LIFO/pinning/layout, persistence, peer bridges, Builder, and primitive/9-slice work remain separately gated. | Jesse “Echo” Adams |
| 1.6.0 | 2026-08-15 | Approved | EUI-M3-02 JIT reconciliation: authoritative view lifecycle now includes replaceable transition execution inside admitted structural operations; drivers remain presentation-only, fresh-operation, unscaled-time, hard-bounded, generation-protected, and cancellable where possible; enter failure restores prior stable UI while exit failure forces deterministic closed/released state; project/default, per-definition, and transient operation override layers are supported; Immediate and CanvasGroup Fade are the built-in reference drivers while professional custom drivers retain curve/easing/timing and reduced-motion substitution seams. Also explicitly separates the future Primitive Warehouse, editable Panel/Menu Template Library, stable-ID Template Catalog, and Assembly Utilities from the later full Builder/Composer so snap-together UI authoring cannot disappear behind Builder scope. Activates EUI-M3-02 while Motifs/accessibility implementation, HUD/transients, full Window management, persistence, peer bridges, primitive/template implementation, Builder implementation, and polished showcase art remain later gated work. | Jesse “Echo” Adams |
| 1.7.0 | 2026-08-16 | Approved | EUI-M4-01 JIT reconciliation: activates named project-defined HUD regions, generation-safe widget leases, reason/owner visibility leases, deterministic effective visibility, bounded capacity, owner-loss cleanup, and retained Screen/Modal/Window independence. Notifications, prompts, tooltips, Motifs/accessibility, full Window management, persistence, authoring libraries, Builder, peer bridges, and release work remain separately gated. | Jesse “Echo” Adams |
| 1.8.0 | 2026-08-17 | Approved | EUI-M4-02 JIT reconciliation: activates project-defined bounded notification channels; priority with FIFO ties; non-preemptive visible entries; opt-in fresh-generation coalescing; reject-newest, drop-oldest-pending, and strict-outrank replace-lowest-priority-pending policies; unscaled/manual lifetime; generation-safe dismissal and owner cleanup; status/events; tests; and Laboratory proof. Prompts, tooltips, Motifs/accessibility implementation, safe area, full Window management, persistence, bridges, authoring libraries/Builder, integration, and release remain gated. | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Looking Glass – UI Framework
**Technical identifier:** EchoUI
**Flavor line:** Reveal the game’s state without becoming the state itself.
**Plain-language subtitle:** Runtime screens, HUD regions, modals, notifications, prompts, focus, navigation, motifs, and transition presentation.

**One-sentence ownership contract:**

> EchoUI owns reusable runtime UI presentation infrastructure, including package-local UI authority, stable surface registration, optional navigation scopes/history, independent windows, HUD/overlay presentation, focus/navigation coordination, view lifecycles, Motif application, and UI-specific diagnostics; it does not own gameplay/pause/cinematic/loading truth, input mappings, settings truth, save data, scene travel, audio playback, gameplay rules, localization content, project lifetime composition, or the project-specific state displayed by its views.

### 1.1 Elevator summary

The Looking Glass provides one coherent runtime surface for project-authored menus, screens, HUD modules, modal dialogs, notifications, tooltips, prompts, fades, and other interface presentation. It supplies a duplicate-safe root, named visual layers, deterministic operations, screen and modal histories, bounded queues, focus restoration, EventSystem coordination, replaceable transition drivers, project-owned motifs, and explicit presenter/view contracts.

The package separates **presentation state** from **game truth**. A settings screen displays an EchoSettings draft but does not become the settings authority. A save-slot screen presents EchoSave metadata but does not write files. A loading overlay observes EchoSceneFlow progress but does not load scenes. A pause screen may request a Pulse scope through a bridge but never sets `Time.timeScale`. A button may request a project command, but it cannot be the only place where the rule exists.

The first backend is Unity's GameObject-based uGUI system with TextMeshPro-compatible text. Public orchestration contracts avoid unnecessary backend leakage so a later UI Toolkit adapter can be researched without rewriting domain presenters. UI Toolkit and mixed-backend focus are outside the MVP until an adapter proves them.

EchoUI works without every other Sperk's Forge package. Bridges may initialize it, present settings/save/input data, coordinate pause or scene transitions, play semantic cues, or publish diagnostics, but no peer becomes a hidden requirement.

### 1.2 Why this belongs in The Sperk’s Forge

Reusable UI infrastructure appears in nearly every EchoDevGames project. The repeated cost is not drawing a panel. It is coordinating which screen is active, whether a modal blocks lower UI, which object receives controller focus, how focus restores, how transitions are interrupted, how transient messages stay bounded, how direct-scene testing works, and how settings, saves, input, audio, scene flow, and game state connect without UI absorbing their authority.

The package preserves familiar uGUI/TMP authoring while replacing one-off managers, duplicate EventSystems, direct callbacks into global objects, inconsistent focus, and project-specific persistence assumptions with explicit lifecycle contracts.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Pair “The Looking Glass” with “UI Framework.” |
| Setup guidance/tooltips | Yes | Flavor must sit beside direct technical meaning. |
| Samples | Optional | Verse styling must be removable. |
| Runtime API/type names | No lore-only names | Types describe screens, layers, modals, focus, and results. |
| Project data | No required Hackulos content | The project owns layouts, fonts, sprites, copy, and visual identity. |

---

## 2. Problem Statement

### 2.1 Current problem

Unity supplies Canvas, EventSystem, Selectable, GraphicRaycaster, layout, and animation primitives, but it does not define one project-wide UI lifecycle contract. Without one:

1. Scenes can contain incompatible Canvases and EventSystems.
2. Persistent and scene-local menus can overlap after travel.
3. Multiple scripts can open or close the same screen.
4. Back behavior depends on whichever object sees input first.
5. Modals can leak pointer or navigation input.
6. Focus disappears when a selected object is disabled or destroyed.
7. Modal close can restore stale focus.
8. Screen animations can overlap and leave CanvasGroups blocked.
9. View initialization can fire click sounds, saves, previews, or domain commands.
10. Loading overlays can accidentally become scene loaders.
11. Pause screens can fight other time-scale/cursor owners.
12. Notifications can grow without bounds or freeze under paused scaled time.
13. Tooltips and prompts become embedded in gameplay scripts.
14. Motifs require editing every prefab.
15. UI defects lack structured stack, focus, layer, and transition diagnostics.

### 2.2 Evidence from existing work

| Source | Existing pattern/problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Main, pause, win, settings, password, and HUD UI across scenes | Familiar uGUI/TMP workflow | Replace scene managers, duplicate EventSystems, and direct global callbacks |
| Don’t Get Vince’d | HUD, dialogue, boss state, pause, and results views | Event-driven presentation | Keep combat/game truth outside views and add deterministic lifecycle |
| Echo Systems Lab | Mission terminal, HUD, save/unlock and modular demos | Focused components and semantic events | Formalize reusable framework and package isolation |
| DeverQuest | Complex Editor UI exposes broad-manager and initialization problems | Rich status feedback and data-driven views | Keep editor product UI outside runtime; require silent binding |
| Hackulos | Future RPG screens for inventory, dialogue, vendor, spells, quests, and combine bag | Project-owned RPG presentation | Keep genre-specific screens outside the general package |
| First Light | Narrow startup status presenter | Package-local fallback surface | EchoUI bridge replaces presentation only |
| Observatory | Standalone diagnostic overlay | Structured view models | EchoUI may host panels without owning diagnostics |
| Accord | Draft/apply/cancel/confirmation workflow | Transactional settings truth | EchoUI presents without persistence ownership |
| Passage | Transition lifecycle/progress | Presenter registration | EchoUI supplies loading/fade visuals only |
| Pulse | Runtime state/pause/cursor policy | Scope ownership | UI requests scopes; never sets global time/cursor |
| Resonance | Audio authority and semantic cues | Cue requests | UI never owns AudioSources or volume persistence |
| Will | Contexts, locks, rebinds, device/glyph truth | Stable input contracts | EchoUI owns focus presentation, not input availability |

### 2.3 Consequences of doing nothing

- Each project rebuilds navigation, focus, modals, and notifications.
- UI becomes a second game-state or persistence system.
- Controller navigation defects appear late.
- Loading, settings, saves, and audio remain coupled to prefabs.
- Accessibility stays bolted-on.
- Project art and orchestration cannot evolve independently.
- Migration becomes an all-at-once rewrite.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one duplicate-safe UI authority with named ordered layers.
- Support screen history, modal ordering, HUD regions, bounded notifications, prompts/tooltips, transitions, and debug presentation.
- Keep production screens, presenters, motifs, text, icons, and art project-owned.
- Define deterministic Push, Replace, Reset, Back, Close, and restoration behavior.
- Define modal blocking and safe out-of-order owner disposal.
- Coordinate one adopted or root-owned uGUI EventSystem without owning gameplay input contexts.
- Restore valid focus after screens, modals, dynamic content, and device-mode changes.
- Use replaceable unscaled-time transition drivers with deterministic failure recovery.
- Provide bounded notifications with priority, coalescing, dismissal, and accessibility timing.
- Provide named HUD regions and project-neutral prompt/tooltip contracts.
- Provide project-owned motif tokens and accessibility presentation policies.
- Remain diagnosable without Observatory.
- Provide repeatable setup, repair, validation, and an isolated UI Laboratory.

### 3.2 Non-goals

- No ownership of gameplay, settings, save, input, scene, audio, localization, or game-state truth.
- No universal production art style.
- No mandatory MVVM, reactive, DI, or tween framework.
- No automatic persistence of screens, modals, notifications, or focus.
- No automatic pause or input-context switching.
- No direct scene loads, save-file writes, or AudioSource control.
- No native screen-reader guarantee in the MVP.
- No UI Toolkit or mixed-backend guarantee in the MVP.
- No universal world-space, drag/drop inventory, virtualized grid, or split-screen framework in the MVP.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project | Generate a validated root and run the UI Laboratory |
| UI designer | Project art/layouts | Create project-owned prefabs and motifs without editing package code |
| UI programmer | Needs reusable lifecycle | Open/close views with structured results and focus behavior |
| Gameplay programmer | Domain services exist | Add presenters so UI requests actions without owning rules |
| Accessibility designer | Needs adaptable UI | Apply text, motion, contrast, timing, and focus policies |
| Tester | UI defect | Inspect layers, histories, focus, queues, and diagnostic codes |
| Integrator | Other Foundation packages | Add/remove explicit bridges cleanly |
| Maintainer | Upgrade assets/API | Preserve GUIDs, stable IDs, and project content |

### 3.4 Measurable success criteria

- Clean installation and standalone operation with declared dependencies only.
- Duplicate root performs no EventSystem, Canvas, registration, focus, or transition side effects.
- Exactly one top screen is interactive after every accepted screen operation.
- Only the top modal is interactive; out-of-order lower-modal disposal is safe.
- Back resolves modal-first, then screen policy, or returns Unhandled.
- Hidden/blocked layers receive no normal interaction.
- Notification/history bounds never grow unbounded.
- Definitions/motifs remain immutable during play.
- Transition failure cannot leave UI permanently blocked.
- Missing/duplicate EventSystems are actionable.
- Samples can be removed safely.
- Setup/repair are repeatable and non-destructive.
- UI Laboratory passes without unrelated Echo packages.
- Existing-project adoption is incremental and reversible.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

Solo developers, small teams, UI programmers/designers, gameplay programmers, accessibility/localization implementers, QA testers, and package maintainers.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Phase |
|---|---|---|---|---|---|
| UC-001 | Initialize standalone root | Developer | Valid config/prefab | One ready authority and layer topology | MVP |
| UC-002 | Open initial screen | Project | Registered screen | Created, entered, focused, reported | MVP |
| UC-003 | Push screen | Project | Active screen | New top; prior entry retained/suspended | MVP |
| UC-004 | Replace top | Project | Active screen | Replacement without history growth | MVP |
| UC-005 | Reset to screen | Project | Any history | One new root screen | MVP |
| UC-006 | Go Back | Player/project | UI ready | Modal-first/screen policy/Unhandled | MVP |
| UC-007 | Show confirmation modal | Presenter | Valid definition | Blocking modal and awaitable result | MVP |
| UC-008 | Close lower modal by owner | Owner | Multiple modals | Safe removal, top unaffected | MVP |
| UC-009 | Restore focus | UI service | Prior target valid or fallback exists | Valid selection or explicit no-focus | MVP |
| UC-010 | Register HUD widget | Presenter | Region exists | Widget attached under lease | MVP |
| UC-011 | Show notification | Any service | Valid request | Bounded/coalesced queue behavior | MVP |
| UC-012 | Show tooltip/prompt | Adapter | Valid content/anchor | Safe positioned transient UI | MVP |
| UC-013 | Apply motif/accessibility | Project/Accord | Valid policy | Targets update without asset mutation | MVP/Bridge |
| UC-014 | Present loading | Passage bridge | Transition active | Progress/fade only; no load ownership | Bridge |
| UC-015 | Present settings | Accord bridge | Draft exists | Silent bind and structured apply/confirm UI | Bridge |
| UC-016 | Present rebind/glyphs | Will bridge | Session/device state exists | UI guides transaction and prompt fallback | Bridge |
| UC-017 | Present save slots | Chronicle bridge | Metadata exists | UI renders and requests operations | Bridge |
| UC-018 | Direct-scene entry | Developer | No root | Minimum development root/EventSystem path | MVP |
| UC-019 | Inspect status | Tester | Root initialized/faulted | Snapshot of layers/history/focus/queues | MVP |
| UC-020 | Embed diagnostics | Observatory bridge | Both packages | Panel hosted without authority transfer | Bridge |
| UC-021 | UI Toolkit backend | Project | Adapter exists | Equivalent lifecycle behind adapter | Deferred |

### 4.3 Explicitly unsupported use cases

- Generic view classes calling scene APIs, persistence, or gameplay managers.
- Treating view prefabs as the only implementation of a rule.
- Inferring pause/input/music behavior from screen names.
- Unlimited transient queues.
- Raw display labels as durable IDs.
- Advertised mixed uGUI/UI Toolkit navigation without an adapter and lab.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Root claim/lifecycle and child service ownership.
- Layer creation/adoption, order, visibility, and interaction gating.
- Screen registration, instantiation, history, activation, suspension, and release.
- Modal order, blocking, result completion, and owner cleanup.
- HUD region registrations.
- Notification, tooltip, and prompt presentation lifecycle.
- UI transition execution/interruption.
- EventSystem adoption/validation and focus coordination.
- Default selection, focus memory/fallback, and back routing.
- Motif/accessibility presentation application.
- UI-specific diagnostics, setup, validation, repair, and samples.

### 5.2 The package does not own

Global preferences, save files/slots, input action availability/rebinding, scene travel, high-level state/pause/time/cursor, audio playback, launch execution, diagnostics truth, localization data, gameplay systems, or project visual content.

### 5.3 Neighboring authorities

| Concern | Owner | EchoUI interaction |
|---|---|---|
| Startup | First Light | Optional initialization/status presenter bridge |
| Diagnostics | Observatory | Optional embedded panels/provider adapter |
| Preferences | Accord | Draft/result presenters and effective UI policy applier |
| Scene travel | Passage | Loading/fade presenter |
| State/pause | Pulse | Scope requests through bridge/project adapter |
| Audio | Resonance | Semantic UI cue requests |
| Input/rebinding/glyphs | Will | Navigation/rebind/prompt bridge |
| Saves | Chronicle | Slot/profile presenter bridge |
| Project composition | Workshop | Generates selected root/config/templates |
| Domain/gameplay | Project/later packages | Presenter reads state and submits commands |

### 5.4 Boundary tests

A feature belongs only if it directly supports presentation lifecycle, remains usable without peers, leaves domain truth external, uses runtime state rather than mutable assets, has an isolated test path, and does not create competing input, scene, pause, save, audio, or diagnostic authority.

---

## 6. Independence Contract

### 6.1 Standalone guarantees

EchoUI compiles with only declared Unity/uGUI dependencies, initializes without First Light, supports direct public navigation without Will, operates without all peers, keeps production content outside package source, exposes test seams, and fails safely when optional collaborators are absent.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Evidence |
|---|---|---|
| Installed alone | Core UI loop works | Clean-project lab |
| Direct Lab entry | Minimum dev root/EventSystem path | Lab test |
| First Light absent | Standalone init | PlayMode |
| Will absent | Public navigation/sample module | Lab |
| Accord absent | In-memory defaults only | Lab |
| Passage/Pulse/Resonance absent | No related bridge behavior; core works | Removal tests |
| Chronicle absent | No save UI in core | Removal test |
| Observatory absent | Local diagnostics remain | Diagnostics test |
| Duplicate root | Side-effect-free rejection | Lifecycle test |
| Missing/duplicate EventSystem | Configured failure/degraded path | Failure tests |
| Sample deleted | Runtime compiles | Removal test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Version | Reason |
|---|---|---:|---|---|
| Unity core | Platform | Yes | 6000.0+ | Runtime/assets/Awaitable/unscaled time |
| `com.unity.ugui` | Platform package | Yes | Released compatible version | Canvas/EventSystem/Selectable/layout |
| TextMeshPro-compatible path | Default presentation | Yes for shipped templates | Verify at M1 | Default text |
| Unity Test Framework | Test | Yes for tests | Compatible | Automation |
| Input System | Optional bridge/sample | No | Will-supported line | InputSystem UI module/glyph/rebind |

### 6.4 Forbidden dependencies

Project assemblies, peer Echo packages in core, sample dependencies, mandatory DI/reactive/tween/addressable/localization/networking packages, reflection discovery as required path, hidden Resources/scene/tag/layer conventions, and non-redistributable fonts/glyphs/art.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Status | MVP? | Surface |
|---|---|---|---:|---|
| CAP-001 | Duplicate-safe persistent root | Approved | Yes | Runtime |
| CAP-002 | Project-defined named ordered layer hosts with package-supplied starter defaults | Approved | Yes | Runtime/Prefab |
| CAP-003 | Stable-ID screen/modal registries | Approved | Yes | Runtime/Data |
| CAP-004 | Push/Replace/Reset/Back screen history | Approved | Yes | Runtime |
| CAP-005 | Blocking modal service with exact-once results | Approved | Yes | Runtime |
| CAP-006 | HUD regions and widget leases | Approved | Yes | Runtime |
| CAP-007 | Bounded notifications | Approved | Yes | Runtime |
| CAP-008 | Tooltip and prompt services | Approved | Yes | Runtime |
| CAP-009 | Focus/EventSystem coordinator | Approved | Yes | Runtime |
| CAP-010 | Replaceable transition drivers | Approved | Yes | Runtime |
| CAP-011 | Project motif tokens/targets | Approved | Yes | Runtime/Data |
| CAP-012 | Accessibility presentation policy | Approved | Yes | Runtime |
| CAP-013 | Safe-area adapter | Approved | Yes | Runtime |
| CAP-014 | Confirmation/modal conveniences | Approved | Yes | Runtime/Sample |
| CAP-015 | Main/Pause/Settings/loading templates | Approved | Yes | Samples |
| CAP-016 | Setup/repair/validation | Approved | Yes | Editor |
| CAP-017 | UI Laboratory | Approved | Yes | Sample |
| CAP-018 | Structured diagnostics | Approved | Yes | Runtime/Editor |
| CAP-019 | UI Toolkit adapter | Deferred | No | Adapter |
| CAP-020 | Native screen-reader provider | Deferred | No | Provider |
| CAP-021 | Virtualized lists/view pooling | Deferred | No | Runtime |
| CAP-022 | Split-screen/world-space framework | Deferred | No | Runtime/Bridge |
| CAP-023 | Independent window surfaces and optional navigation scopes | Approved | Yes | Runtime |
| CAP-024 | Stable surface registry/discoverability | Approved | Yes | Runtime/Editor |
| CAP-025 | Motif capture/apply/local-override authoring | Approved | Yes | Runtime/Data/Editor |
| CAP-026 | Lego primitive library and batch UI Builder | Approved | Yes | Prefab/Editor |
| CAP-027 | Editable panel/menu template library plus project-extensible stable-ID template catalog | Approved | Yes | Prefab/Data/Editor |
| CAP-028 | Lightweight UI assembly utilities independent of the full Builder/Composer | Approved | Yes | Editor |

### 7.2 MVP capability set

One protected root, layer topology, screen/modal lifecycle, HUD regions, bounded notifications, one tooltip and prompt channel, EventSystem/focus/back coordination, unscaled transition drivers, project motifs/accessibility/safe area, tooling, diagnostics, templates, and a standalone Lab.

### 7.3 Later capability set

UI Toolkit, native accessibility providers, virtualized lists, pooling, per-player roots, world-space helpers, localization refresh, rich animation adapters, and screenshot/layout regression tooling.

### 7.4 Deferred/rejected ideas

| Idea | Disposition | Reason |
|---|---|---|
| UI automatically pauses/switches input | Rejected | Pulse/Will authority |
| UI loads scenes or stores settings/saves | Rejected | Passage/Accord/Chronicle authority |
| Giant MenuManager | Rejected | Violates focused architecture |
| Reflection discovers screens | Rejected | Hidden coupling/AOT/removal risk |
| Mandatory tween/DI/reactive package | Rejected MVP | Dependency/style lock-in |
| UI Toolkit first/mixed backend | Deferred | Doubles MVP and focus complexity |
| Persist active screen stack | Deferred | Project-specific restore semantics |
| Native screen-reader promise | Deferred | Platform research required |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/config | Config, layer/screen/modal/region definitions, motifs, transition profiles | Active entries, focus, queues, subscriptions |
| Runtime behavior | Root, registries, navigators, services, transitions, focus, histories | Editor APIs, domain truth, persistence |
| Presentation | uGUI views, motif targets, sample/bridge presenters | Gameplay/save/scene/input/audio authority |

### 8.1A JIT architecture rebaseline — surfaces, context, scopes, and Lego authoring

The August 13, 2026 Learn → Declare → Authorize review replaces the earlier assumption that all ordinary UI participates in one global screen stack.

**UI context is layered, not one giant mutually-exclusive enum.** Looking Glass may consume project-supplied facts such as Frontend/Gameplay context and independent conditions such as Paused, Cinematic, Loading, or Saving. It reacts to those facts through visibility/navigation policy; it does not decide that the game is paused or cinematic.

**Surfaces are individually addressable.** Every authored surface uses a stable project-owned ID and a behavioral role. The initial roles are:

- `Screen` — exclusive only within an authored navigation scope.
- `Window` — independent and allowed to coexist with other windows/screens.
- `HUD` — persistent/reactive presentation whose visibility may respond to context policy.
- `Overlay` — additive temporary presentation above other surfaces.

A navigation scope is optional. Within one scope, at most one `Screen` is active. Back uses navigation history by default, while explicit Navigate To, Return To Root, Resume/Close Scope, and project commands remain available. Independent windows are never globally closed merely because another surface opens.

**Visibility policy cascades from context.** A HUD/window may declare policy such as hide while Paused or Cinematic. The package must not hardcode “Pause hides HUD.” Context values originate from project/controller/game-state adapters or a Laboratory helper.

**Stable IDs enable discovery.** The runtime exposes a queryable surface registry so project-authored launchers, scrolling selectors, debugging tools, and future “menus for menus” can enumerate/toggle surfaces without hierarchy-path coupling.

**Input-aware selection is separate from input ownership.** A surface may declare a default selected control. Future `Auto` selection policy distinguishes pointer presentation from keyboard/gamepad navigation so mouse users do not receive a visually selected button merely because the screen opened. Looking Glass consumes modality information or infers a safe fallback; it does not own input actions or require The Will.

**Authoring vocabulary is intentionally plain.** Samples/tools use `Type_DescriptiveName`, for example `Canvas_MasterCanvas`, `Panel_MenuRoot`, `Panel_SettingsMenu`, `Button_DefaultButton`, `Slider_DefaultSlider`, and `Toggle_DefaultToggle`. This convention is an authoring/sample language, not a requirement that runtime stable IDs equal hierarchy names.

**The package is a construction toolkit as well as runtime plumbing.** Its future authoring stack is explicitly layered rather than collapsed into one Builder feature:

1. **Primitive Warehouse** — package-owned, focused reusable prefab families such as default/close buttons, sliders, toggles, tabs, fields, dropdowns, scroll pieces, separators, progress indicators, panel surfaces, and scalable 9-sliced borders/backgrounds. Parallel visual families may share the same behavior.
2. **Panel/Menu Template Library** — ordinary editable prefab compositions assembled from primitives, including common menu, settings, pause, confirmation, inventory-style Window, character/journal/crafting, and list/detail starting points. Templates are copy-in/editable starting points, never opaque runtime objects.
3. **Stable-ID Template Catalog** — package starter definitions plus project-extensible catalogs that may add, remove, replace, regroup, or override available primitives/templates without rewriting package Runtime.
4. **Assembly Utilities** — lightweight Editor commands for create-from-template, add common groups, name/parent/validate, replace primitive families, and later apply Motifs. These utilities remain useful even if the full Builder is never opened.
5. **Builder / Composer** — a later richer point-and-click authoring surface that consumes the same catalog and creates ordinary editable project objects rather than proprietary locked compositions.

Reusable **Motif** assets remain a separate appearance system that capture/apply colors, Selectable states, sprites, typography, and decorative treatment without owning layout, navigation, domain commands, or game content. Local overrides must be preservable when Motifs are reapplied. Project-specific prefab variants remain free to share the same underlying Looking Glass behavior.

The package-local root does not call `DontDestroyOnLoad` merely because long-lived UI is possible. Unity object lifetime and cross-package composition remain project-owned under SFGSS-ADR-006.

### 8.1B EUI-M1-02 contract — designer-ordered external context response and input-aware selection

The bounded EUI-M1-02 JIT revisit refines the earlier layered-context direction into an implementation-authorizing package contract without transferring game-state or input authority into Looking Glass.

**Designer control is the default posture.** Looking Glass supplies small composable mechanisms and neutral defaults rather than one package-authored menu doctrine. Common templates, presets, primitive libraries, Motifs, and Builder workflows may later accelerate common paths, but convenience must not remove the designer's ability to configure individual surfaces and local cases.

**Context identity is project-defined and stable.** A UI context is identified by a project-authored stable ID and is active or inactive. Examples such as `pause`, `cinematic`, and `loading` are conventions, not reserved gameplay concepts. Project composition or optional adapters may map domain-specific names into these UI-facing IDs. EUI-M1-02 does not attach arbitrary domain payloads to a context ID; richer values such as loading progress, dialogue speaker data, save metadata, or gameplay state remain owned and transported by the systems that define them.

**Multiple contexts may coexist.** A surface may participate in any number of active contexts. Each surface owns an ordered list of response rules, and the designer controls that ordering. Rule priority may differ between surfaces, scene uses, prefab instances, or other project-authored cases. Looking Glass must not impose a universal precedence such as “Hide always wins.”

**Response resolution is per controlled dimension.** The first applicable active rule that explicitly supplies a value for a response dimension supplies that dimension. Evaluation may continue for other dimensions not specified by that rule. EUI-M1-02 recognizes at least:

- visibility intent;
- interaction intent;
- selection/focus intent.

Visibility, interaction, and selection are separate concepts. A surface may remain visible while non-interactable or unselected. If no applicable rule supplies a value for a dimension, Looking Glass performs no context-driven change to that dimension.

**External participation is optional per surface.** A surface may opt out of automatic external-context response. Opting out does not unregister the surface or prevent direct project/navigation operations; it only prevents external context evaluation from changing that surface automatically.

**Authored defaults and overrides remain distinct from runtime truth.** Reusable authored defaults may be refined by scene/local or individual instance overrides. Project code may also provide transient runtime overrides so highly configurable HUD/window experiences can change effective UI behavior without mutating authored assets. Runtime overrides are session state, not persistence. EUI-M1-02 does not add Chronicle or Accord ownership, serialization, profile storage, or durable window-layout persistence.

**Selection consumes modality; it does not own modality detection.** A surface may configure controller/keyboard opening behavior independently from pointer behavior. A controller-oriented surface may select a configured default when opened; pointer-oriented behavior may remain unselected. Designers may also configure controller opening to remain unselected. Closing a temporary surface defaults to no selected control rather than implicitly restoring historical selection. General focus-history restoration and focused-window arbitration remain later capabilities.

**Standalone proof may simulate external truth.** The Laboratory may provide tiny sample-owned controls that toggle context IDs and input modality solely to prove Looking Glass behavior in isolation. Those helpers are not production game-state, Controller, Will, Pulse, Chronicle, Accord, or other peer-package integrations.

**Preset/template direction is copy-in, not centralized live policy.** Future presets/templates may populate useful starting rules that become freely editable project configuration. EUI-M1-02 must not require a centralized policy subscription model, and actual preset/template authoring tooling is outside this checkpoint.

### 8.1C EUI-M2-01 contract — project-defined layers, screen ownership, suspension, and serialized operations

The August 14, 2026 bounded M2-01 revisit advances the proven surface/context foundation into an authoritative screen lifecycle without turning Looking Glass into a fixed menu shell.

**Layer topology is project-defined, ordered, and stable-addressed.** The earlier fixed “seven named root layers” statement is superseded. Looking Glass may ship a recommended starter layer arrangement as convenience/template content, but runtime correctness must not depend on a fixed layer count or reserved project layer names. Projects/designers may add, remove, reorder, or substitute layer definitions in authored configuration. Layers use stable IDs separate from display labels/hierarchy names. The resolved production topology is validated at initialization and is not casually reordered by runtime callers.

**Screen ownership is explicit.** `RootOwned`, `SceneOwned`, and `ExternalOwned` are all first-class lifecycle modes. Root-owned views may be created/released by Looking Glass from explicit project-authored definition/factory data. Scene-owned views are existing scene objects coordinated by Looking Glass without transferring destruction/lifetime ownership. External-owned views are explicitly supplied/registered by project code and remain externally lifetime-owned. All three still use Looking Glass screen history and lifecycle once admitted.

**Suspension presentation is designer-controlled but scope interaction remains authoritative.** When a pushed screen suspends the previous top entry, the designer may choose to hide the suspended screen, keep it visible, or preserve its authored/effective visibility. Regardless of visibility choice, a suspended `Screen` in that navigation scope is not interactive while another screen is the active top entry. This preserves the invariant that exactly one eligible screen per scope is interactive without imposing one visual doctrine.

**Screen structural mutations are serialized in strict submission order for M2-01.** Accepted Push/Navigate, Replace, Reset/Return-to-root, Back, and Close operations execute one at a time through a bounded FIFO admission path. M2-01 does not silently reorder, coalesce, replace, or drop accepted requests. When the bounded queue cannot admit work, the request is explicitly rejected without partial history/view mutation. Later optimization policies may be researched only behind a future declared contract.

**M2-01 is screen-only.** Modal stack ownership, blocking, exact-once modal completion, and modal result awaiters remain approved M2 capabilities but are intentionally deferred to a separate `EUI-M2-02` checkpoint. Likewise focus-history restoration and transition-driver execution remain later slices.

### 8.1D EUI-M2-02 contract — blocking modal lifecycle, exact-once results, and UI/input boundary

The August 14, 2026 bounded M2-02 revisit completes the second Runtime Core slice by adding authoritative blocking-modal lifecycle without turning Looking Glass into gameplay-input or pause authority.

**Blocking modals may stack.** A blocking modal stack is ordered. Only the top eligible modal receives normal Looking Glass interaction. Lower modals remain live entries and may be targeted safely by their handles, including out-of-order owner cleanup, without stealing interaction from the top entry.

**Semantic completion uses project-defined stable result IDs.** Looking Glass does not reserve game-specific meanings such as `yes`, `no`, `delete`, `easy`, or `hard`. A normal modal completion supplies a nonempty project-authored stable result ID. Result identity is separate from display text and survives prefab/hierarchy renames. EUI-M2-02 does not require arbitrary domain payload transport; richer typed payloads may be added only through a later declared contract if real use proves them necessary.

**Exact-once means first terminal completion wins.** Each admitted modal opening owns one fresh completion channel. The first valid terminal action commits the result and settles its awaiter exactly once. Later confirm/cancel/Back/owner attempts on the same generation are harmless structured stale/already-completed rejections and never invoke completion a second time.

**Structural loss is `Aborted`, not semantic Cancel.** Unexpected owner/view loss after admission, root shutdown, or equivalent lifecycle teardown settles the modal exactly once with a distinct structural `Aborted` outcome/reason. It must not fabricate a project semantic result ID such as `cancel`. Factory/validation failure before a modal becomes active returns an operation failure and leaves no live modal entry/awaiter leak.

**Modal view ownership reuses the established M2 ownership model.** `RootOwned`, `SceneOwned`, and `ExternalOwned` are first-class modal view modes with the same lifetime boundary proven for Screens. Looking Glass creates/releases only RootOwned instances. SceneOwned and ExternalOwned GameObjects remain owned by their scene/project provider even when their modal entries complete or abort.

**Every modal opening receives a fresh awaiter/handle generation.** Awaitables are never cached or reused. Handles identify one admitted modal generation so stale handles cannot complete a later reopening of the same definition ID.

**Back/dismiss behavior is designer-authored.** A modal may disable Back/dismissal or map Back to one configured project-defined stable result ID. Back routes to the top blocking modal before ordinary Screen history. A disabled Back policy leaves the modal active and returns a structured Blocked/Unhandled-style result rather than silently closing it.

**Looking Glass blocks Looking Glass UI, not gameplay input.** While a blocking modal is active, lower Looking Glass pointer/raycast interaction, UI navigation/submit, and ordinary UI Back routing are gated so clicks/navigation cannot leak through the modal. Looking Glass does not disable gameplay action maps, consume project WASD by authority, set pause/time scale, change cursor ownership, or decide whether the game simulation continues. Project code or optional future Will/Pulse/Vessel bridges may observe read-only modal blocking state and choose their own gameplay/input response. The standalone Laboratory may simulate an external gameplay action continuing while lower uGUI remains blocked.

**Blocking Modal semantics are role-specific.** These guarantees apply only to entries admitted through the blocking `Modal` lifecycle. They do not convert every floating `Window` into a modal. Independent `Window` surfaces remain non-blocking by default, may coexist with Screens and peer Windows, and do not automatically suppress interaction with those peer Windows or with project-owned gameplay behavior. A project may therefore keep inventory, character, crafting, skill, quest, launcher/tool-palette, or similar windows open while the player continues interacting with other windows and the game world. Actual control raycast behavior remains project/designer authored.

**Window dismissal history is separate from Screen-operation FIFO.** The M2-01 FIFO rule governs the order in which accepted structural operations execute. It does not mean Back/Escape should close UI in first-opened order. A future independent-window dismissal policy may maintain a separate **most-recent-eligible (LIFO)** history so Back/Escape closes the most recently opened/raised eligible Window first. Designers may exclude authored-default or runtime-pinned/locked Windows from automatic dismissal. Runtime pin state remains transient UI state; durable window-layout/pin persistence stays separately gated. EUI-M2-02 does not implement this future Window-manager capability.

**Screen mutation while a blocking modal is active has an explicit simple/advanced policy.** The safe default is `Reject`: ordinary Screen structural requests are rejected before mutation with a structured `BlockedByModal`-style result. Designers/projects that need deferred behavior may choose a bounded `DeferUntilModalStackClears` policy. Deferred Screen requests retain strict FIFO submission order, execute only after the blocking modal stack becomes empty, and remain subject to the normal bounded Screen-operation admission rules. EUI-M2-02 does not authorize silent background Screen mutation underneath an active blocking modal.

**Modal visuals remain designer/project owned.** Looking Glass guarantees lifecycle and interaction blocking, not one mandatory gray/dim/blur treatment. A project modal prefab may include its own backdrop and styling. Blur systems, animated transitions, generalized backdrop effects, and polished production art remain later work.

**Checkpoint scope remains narrow.** EUI-M2-02 proves modal definitions/entries/handles, stacking, ownership, blocking, stable result IDs, exact-once settlement, structural Aborted outcomes, fresh awaiters, Back policy, bounded capacity, Screen mutation Reject/Defer behavior, and retained M1/M2-01 behavior. It does not implement full focus-history restoration/EventSystem adoption, transition drivers, HUD regions, notifications, prompts/tooltips, Motifs, Builder, primitive-library expansion, persistence, peer bridges, automatic gameplay-input switching, or project-wide lifetime composition.

### 8.2 Component topology

```text
EchoUIRoot
├── UISurfaceRegistry
│   ├── Screen (optional navigation scope)
│   ├── Window (independent)
│   ├── HUD
│   └── Overlay
├── UIContextState + VisibilityPolicy (context is externally supplied)
├── UIScreenNavigator + Scope Registry + History
├── UILayerRegistry
│   ├── Modal
│   ├── Notification
│   ├── Tooltip + Prompt
│   ├── Transition
│   └── Debug
├── UIModalService + Registry + Entries
├── UIHudRegionService
├── UINotificationService
├── UITooltipService
├── UIPromptService
├── UIFocusCoordinator + EventSystemAdapter
├── UITransitionCoordinator
├── UIMotifService
├── UIAccessibilityService + SafeAreaAdapter
└── EchoUIStatus + bounded history
```

Domain authority -> presenter/view model -> view. View interaction -> presenter command -> domain result -> view update.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root? | Project-owned choice; EchoUI does not call `DontDestroyOnLoad` |
| Lifetime | Valid while the project-owned authoritative root exists |
| Duplicate behavior | Reject before surface-registry/navigation/EventSystem/layer/subscription/focus side effects |
| Initialization | Standalone explicit/project composition; optional bridges may request startup later |
| Shutdown | Reject new work, finish/cancel operations, complete awaiters, clear/release |
| Direct-scene | Development/Laboratory initializer may create minimum authority only if absent |
| Test seams | Surface registry/navigation first; later clock, transitions, EventSystem/focus/safe-area adapters |

### 8.4 Layer topology

Project-authored layer definitions are stable-ID-addressed and explicitly ordered. Looking Glass may provide a recommended starter arrangement for common Screen/Window/HUD/Modal/Notification/Tooltip-Prompt/Transition/Debug-style uses, but those names and that count are convenience defaults rather than runtime law. Projects may add, remove, reorder, or substitute authored layer definitions before initialization. Screen exclusivity applies only inside an authored navigation scope; independent windows and eligible HUD surfaces may remain interactive together. Modal/overlay policy may later gate lower interaction through CanvasGroup/raycast/navigation policy. Runtime callers cannot arbitrarily reorder the resolved production topology.

### 8.5 Screen history

Screen history is maintained per navigation scope. `NavigateTo`, `Replace`, `ReturnToRoot`, explicit close under policy, and history-based `Back` affect only the targeted scope. Independent Window/HUD/Overlay open state does not enter screen history. Later queued/animated mutations must remain bounded and non-conflicting per scope.

### 8.6 Modal model

Blocking modals form one ordered runtime stack and are keyed by runtime generation/owner handle so a lower modal may be targeted safely out of order. Only the top eligible modal receives normal Looking Glass interaction. Normal completion returns one project-defined stable result ID; the first valid terminal completion wins exactly once. Unexpected post-admission owner/view loss or shutdown returns structural `Aborted` rather than fabricating semantic Cancel. Back behavior is definition-authored. Lower Looking Glass interaction is gated while the stack is active, but gameplay input/pause authority remains external.

### 8.7 Focus and EventSystem

EUI-M3-01 activates the full bounded focus/EventSystem coordination slice while preserving project input authority.

**EventSystem coordination modes**

Configuration selects one explicit mode:

- `AdoptAssigned` — use exactly the EventSystem assigned by the project/designer;
- `AdoptExisting` — deterministically adopt one eligible existing EventSystem only when the result is unambiguous;
- `CreateIfMissing` — adopt an eligible existing EventSystem or create one only when none exists and creation is explicitly configured;
- `RequireExternal` — require project-supplied EventSystem authority and never create one.

EchoUI never silently destroys, disables, or steals an externally owned EventSystem. Multiple active eligible EventSystems produce actionable degraded/blocking focus status rather than an arbitrary winner.

**Focus memory and restoration**

Focus memory always exists per live UI runtime entry. A surface may additionally opt into transient root-session memory keyed by its stable surface ID so a later reopening can remember the last valid selected target during the current UI session. Reopening behavior is designer-selectable: fresh/default behavior remains available and session memory is never mandatory.

No focus memory is durable persistence. It is cleared by the appropriate entry/session lifetime and is not written to Chronicle, Accord, disk, or authored assets by EchoUI.

When a Screen resumes/Back exposes a prior entry, a blocking Modal completes, or an explicit focus request/revalidation occurs, Looking Glass resolves focus through the deterministic chain:

`explicit target -> valid remembered target -> authored default -> entry resolver -> global fallback -> legal no-focus`

Remembered/default targets that are destroyed, disabled, non-interactable, outside the eligible focus scope, or otherwise illegal are skipped rather than treated as corruption.

Pointer/navigation behavior remains designer-controlled. Pointer-driven openings/interactions may intentionally resolve to no selected object. Looking Glass does not clear selection merely because a pointer moved by a trivial amount. Navigation/controller modality may establish a configured eligible target when policy requires one.

**Blocking Modal containment**

While a blocking Modal stack is active, EventSystem selection may not legally escape the top eligible Modal into lower Looking Glass UI. Lower entries retain their focus memory, and when the Modal completes the newly exposed entry may restore remembered focus according to its current policy.

Independent Windows retain distinct focus memory without implying focused-window arbitration, z-order raising doctrine, most-recent-eligible Back/Escape history, pin/lock state, dragging/resizing, or persisted layout.

**Event-driven maintenance**

Focus coordination is event-driven by default. Relevant entry lifecycle, hierarchy/selection invalidation, modality changes, explicit focus requests, and explicit revalidation trigger work. Looking Glass exposes a bounded project-callable revalidation seam for highly dynamic interfaces. No per-frame full-scene selectable/EventSystem scan is required. A future opt-in tick/revalidation driver may be added only if profiling and a later checkpoint justify it.

Focus requests carry operation/generation identity. A stale request cannot overwrite a newer UI state.

**Optional suite input compatibility**

EchoUI core does not require Unity Input System action ownership or a generated input wrapper. Optional project/adapter wiring may default to the suite's SFGSS-000 Unity-default `UI/Navigate`, `UI/Submit`, `UI/Cancel`, `UI/Point`, `UI/Click`, and related action-name profile when present. Projects may override this mapping, and Looking Glass does not enable/disable gameplay or UI action maps by authority.

### 8.7A EUI-M3-02 contract — authoritative view lifecycle and replaceable transition execution

EUI-M3-02 activates the bounded transition/view-lifecycle slice while preserving all M3-01 focus and input-authority boundaries.

**Transitions settle inside admitted structural operations.** Once a Screen, blocking Modal, or independent Window structural operation is admitted, its authored enter/exit transition is part of completing that operation. The authoritative lifecycle does not report a successful terminal structural state while its required transition is still unresolved. Existing bounded/FIFO mutation rules remain authoritative, so a later accepted structural mutation cannot race through the same lifecycle halfway through transition settlement.

**Transition drivers own presentation only.** A driver may animate CanvasGroup alpha, scale, position, project materials/shaders, Animator state, or other project presentation details. It does not decide screen history, Modal semantic results, pause/time scale, input-map state, scene travel, settings/save truth, audio authority, gameplay rules, or project lifetime composition.

**Every execution is fresh and generation-bound.** Transition work uses a fresh operation/result per execution. Reused/cached awaitables are forbidden. Each operation carries identity/generation so a stale completion, cancellation callback, exception, or delayed noncancellable driver cannot rewind newer authoritative UI state.

**Cancellation is best-effort; safety is mandatory.** Drivers are cancellable where practical. When cancellation is unavailable or ignored, stale-generation rejection plus a hard safety bound prevents old work from blocking or mutating newer lifecycle truth indefinitely.

**Failure recovery is asymmetric and deterministic.**
- Enter/open failure aborts the incoming entry, releases any RootOwned partial instance, preserves/restores the prior known-stable UI state, and must not leave authoritative history half-mutated.
- A blocking Modal that fails after admission settles through structural `Aborted` rather than fabricating a semantic Cancel result.
- Exit/close failure forces the departing entry into its deterministic closed/released state and continues settlement so a broken fade-out cannot hold the UI hostage.
- Root shutdown/view destruction always wins and leaves no transition-owned temporary state behind.

**Transition policy resolves from layered authoring.** Effective transition behavior resolves from project/root default -> per-definition profile -> optional transient operation override. Runtime overrides are session state only and never mutate authored assets. A profile may independently describe enter and exit driver selection, timing/duration, optional curve/easing data, hard timeout/safety bounds, and reduced-motion substitution.

**The seam is surface-general; M3-02 wiring is bounded.** The contract is designed so later HUD/Overlay/transient services may consume the same transition seam, but this checkpoint wires only lifecycle machinery that already exists: Screens, blocking Modals, and independent Windows. M3-02 does not activate HUD/notification/tooltip/prompt services.

**Reference drivers stay small while the extension seam stays professional.** The package supplies deterministic `Immediate`/no-animation behavior and a simple unscaled `CanvasGroup` fade reference driver. Projects may provide Animator, tween-library, shader/dissolve, slide/scale, 3D, or other custom drivers without replacing Looking Glass lifecycle authority or adding a mandatory tween dependency.

**Reduced motion is architecturally supported but not fully implemented here.** Transition policy must permit a later accessibility/Motif layer to substitute Immediate or another approved reduced-motion path. M3-02 does not itself implement the broader Motif/accessibility service.

### 8.8 View/presenter separation

Views own controls, visual state, lifecycle hooks, and user interaction events. Presenters bind silently, interpret domain state, request domain commands, handle results, and unsubscribe. Views do not discover static managers or own persistence/game rules.

### 8.9 Transition model

`IUITransitionDriver` returns a fresh `Awaitable<UITransitionResult>` for every execution and uses unscaled time. Effective policy resolves from project/root default -> per-definition `UITransitionProfile` -> optional transient operation override. Drivers are cancellable where possible, generation-bound, and hard-bounded. Immediate/no-animation and CanvasGroup fade are the package reference drivers; professional projects may supply Animator/tween/shader/slide/scale/custom drivers without transferring lifecycle authority or creating a mandatory tween dependency. Enter failure restores the prior known-stable UI and cleans the incoming entry; exit failure forces the departing entry to deterministic closed/released state; stale completion cannot mutate newer UI truth.

### 8.10 Notifications/prompts/motifs

Notifications are bounded, prioritized, coalesced, dismissible, and unscaled. One tooltip and one prompt channel are the default MVP. Motifs are immutable project token assets applied to registered targets. Accessibility policy controls text scale, motion, contrast variant, focus indicator, and transient timing, with persistence owned by Accord/project.

### 8.11 Lifecycle

Claim package-local authority -> validate/register surfaces -> initialize scoped navigation -> Ready -> operate -> prune scene-owned registrations -> orderly shutdown. Later milestones may add layer/EventSystem/factory/Motif/accessibility initialization without changing project-owned lifetime composition.

### 8.12 Failure model

| Failure | Fallback | Code |
|---|---|---|
| Duplicate root | Duplicate exits before side effects | EUI-LIFE-001 |
| Missing config/layer | Fault/block or approved repair | EUI-CONFIG-001 / EUI-LAYER-001 |
| Missing/duplicate EventSystem | Noninteractive/degraded/block by policy | EUI-EVENT-001/002 |
| Missing screen/factory failure | No history mutation; clean partial instance | EUI-SCREEN-001/002 |
| Duplicate request/queue full | Coalesce/reject | EUI-SCREEN-003 / EUI-QUEUE-001 |
| Transition failure/timeout | Force safe final state | EUI-TRANS-001 |
| Modal owner/view loss | Exact-once structural `Aborted` result; no fabricated semantic Cancel | EUI-MODAL-001 |
| Invalid/lost focus | Fallback chain or no-focus | EUI-FOCUS-001/002 |
| Notification overflow | Explicit drop/replace policy | EUI-NOTIFY-001 |
| Motif/safe-area missing | Fallback/default | EUI-MOTIF-001 / EUI-SAFE-001 |
| Presenter exception | Isolate/report; authority remains alive | EUI-VIEW-001 |
## 9. Runtime Data and State Model

### 9.1 Definition and configuration assets

| Type | Purpose | Stable ID? | Runtime mutable? | Project-owned? |
|---|---|---:|---:|---:|
| `EchoUIConfiguration` | Root/layers, policies, limits, defaults, EventSystem policy | Yes | No | Yes |
| `UILayerDefinition` | Layer ID, order, visibility/interactivity | Yes | No | Yes/template |
| `UIScreenDefinition` | Screen identity, prefab/factory, focus/back/transition policy | Yes | No | Yes |
| `UIModalDefinition` | Modal identity, ownership/factory, Back result policy, Screen-mutation blocking policy | Yes | No | Yes |
| `UIHudRegionDefinition` | Named host and ordering policy | Yes | No | Yes |
| `UITransitionProfile` | Enter/exit drivers, timings, optional curve/easing, hard bound, reduced-motion substitution | Yes | No | Yes/template |
| `UIMotifDefinition` | Motif tokens/assets | Yes | No | Yes |
| `UIAccessibilityDefaults` | Default presentation policy | Yes | No | Yes |
| `UINotificationPolicy` | Bounds/coalescing/overflow/timing | Yes | No | Yes |
| `UITooltipPolicy` | Delay/placement/hide rules | Yes | No | Yes |
| `UISafeAreaPolicy` | Safe-area behavior | Yes | No | Yes |
| `UIScreenCatalog` / `UIModalCatalog` | Explicit registry assets | Yes | No | Yes |

### 9.2 Runtime state

| State | Owner | Lifetime | Reset | Serialization |
|---|---|---|---|---|
| `EchoUIRuntimeState` | Root | App session | Shutdown | Not saved |
| `UIScreenEntry` | Navigator | Open entry | Close/reset/shutdown | Not saved |
| `UIModalEntry` | Modal service | Open modal | Complete/dispose/shutdown | Not saved |
| `UIHudWidgetEntry` | HUD service | Lease | Dispose/owner loss | Not saved |
| `UINotificationEntry` | Notification service | Queue/display/history | Policy/reset | Not saved |
| `UITooltipEntry` / `UIPromptEntry` | Transient services | Active item | Hide/replace | Not saved |
| `UIFocusMemory` | Focus service | Live entry plus optional root-session stable-surface cache | Entry release / session reset according to authored policy | Not saved |
| `UITransitionOperation` | Coordinator | Operation | Terminal state | Not saved |
| Motif/accessibility effective state | Motif/accessibility services | Session | Policy change | Persisted externally |
| Diagnostic history | Root/status | Bounded session | Reset/shutdown | Export only |

### 9.3 Stable identifiers

IDs are normalized stable value types or strings separate from display labels. They are nonempty, collision-checked, preserved across prefab/file renames, included in results/diagnostics, and supported by aliases/migration after public release. They never depend only on hierarchy paths, build indexes, or scene names.

Suggested project naming is semantic, such as `screen.main-menu`, `modal.confirm`, `hud.status`, and `motif.default`, but the package does not impose genre vocabulary.

### 9.4 ScriptableObject safety

Definitions remain immutable during play. They never store active instances, focus targets, transition time, queue indexes, presenter subscriptions, current selected motif as mutable session truth, or changing scene-object references. Runtime models hold state keyed by definition/stable ID.

### 9.5 View ownership scope

- **RootOwned:** instantiated under the persistent root and released by EchoUI.
- **SceneOwned:** registered by a scene owner and pruned on owner/scene destruction.
- **ExternalOwned:** supplied by project code; EchoUI coordinates only while registered.
- **Pooled:** deferred until profiling justifies pooling.

Registration handles are idempotently disposable.

### 9.6 Screen state machine

```text
Requested -> Validating -> Creating -> Binding -> Entering -> Active
Active -> Suspending -> Suspended -> Resuming -> Active
Active/Suspended -> Exiting -> Releasing -> Closed
Any pre-active state -> Failed/Cancelled -> Releasing -> Closed
```

Only the active top entry receives normal interaction. Suspended views remain visible only by explicit definition policy.

### 9.7 Modal state machine

```text
Requested -> Validating -> Creating -> Entering -> Active
Active -> Completing -> Exiting -> Releasing -> Completed
Any state -> Cancelling/Failed -> Releasing -> Completed exactly once
```

### 9.8 Serialization and migration

Core does not persist navigation state. Configuration assets carry schema version and use explicit Editor migration. Motif/accessibility stable IDs may be persisted by Accord/project. Unknown IDs produce fallback/unavailable results without rewriting the external source.

---

## 10. Public Runtime API

Exact syntax may be refined during M1/M2, but authority and behavior cannot change silently.

### 10.1 Public types

| Type | Kind | Responsibility |
|---|---|---|
| `EchoUIRoot` | MonoBehaviour | Claims authority and owns child services |
| `IEchoUIService` | Interface | Public facade |
| `EchoUIConfiguration` | ScriptableObject | Project policies and references |
| Stable IDs | Value types | Layer/screen/modal/region/motif identity |
| Screen/modal definitions | ScriptableObjects | Project-authored lifecycle/factory policy |
| Screen/modal/HUD/notification handles | Struct/leases | Runtime ownership/identity |
| Screen/modal/notification/tooltip/prompt requests | Struct/classes | Validated operation input |
| Screen/modal/back/transition results | Struct/enums | Structured terminal results |
| `UIMotifDefinition` | ScriptableObject | Project motif tokens/assets |
| `UIAccessibilityPresentationPolicy` | Serializable value | Effective presentation settings |
| `EchoUIStatusSnapshot` | Immutable struct | Structured status |
| `IUIScreenView` / `IUIModalView` | Interfaces | View lifecycle/control contract; modal semantic completion supplies stable result IDs |
| `IUIScreenFactory` | Interface | Create/release screen instances |
| `IUITransitionDriver` | Interface | Open/close transition execution |
| `IUIEventSystemAdapter` | Interface | EventSystem selection/focus seam |
| `IUIClock` | Interface | Unscaled timing seam |
| `IUIMotifTarget` / `IUIAccessibilityTarget` | Interfaces | Apply effective presentation |

### 10.2 Public operations

| Member | Purpose | Preconditions | Result/failure | Loop rule |
|---|---|---|---|---|
| `InitializeAsync` | Initialize authority | Claim/config valid | Structured init result | Main thread for Unity objects |
| `OpenScreenAsync` | Push/replace/reset screen | Ready/valid definition | Fresh `Awaitable<UIScreenResult>` | Serialized mutation |
| `CloseScreenAsync` | Close owned/target entry | Live handle/policy | Idempotent structured result | Serialized |
| `GoBackAsync` | Route modal-first/back policy | Ready | Handled/Blocked/Unhandled/Failure | Main thread |
| `ShowModalAsync` | Show modal and await result | Valid definition/view contract and capacity | Fresh `Awaitable<UIModalResult>` plus generation handle | Main thread |
| `TryCompleteModal` | Complete one live modal generation | Live handle + valid project result ID | First terminal result wins exactly once | Main thread |
| `TryAbortModal` | Structural owner/lifecycle abort | Live handle | Exact-once `Aborted` result with reason | Main thread |
| `RegisterHudWidget` | Add widget to region | Valid region/view | Disposable handle | Main thread |
| `SetHudVisibility` | Change region visibility | Valid region | Structured result | Main thread |
| `EnqueueNotification` / `DismissNotification` | Manage bounded notifications | Valid request/handle | Handle/result | Main thread |
| `ShowTooltip` / `HideTooltip` | Manage tooltip | Valid anchor/content | Handle/result | Main thread |
| `SetPrompt` / `ClearPrompt` | Manage prompt | Ready | Handle/result | Main thread |
| `ApplyMotif` / `ApplyAccessibility` | Apply effective presentation | Valid values/targets | Report/fallback | Main thread |
| `RequestFocus` / `ClearFocus` | Set UI selection state | Interactive surface | Applied/deferred/fallback | Main thread/stale-safe |
| `CaptureStatus` | Snapshot state | Any init/fault state | Immutable snapshot | Side-effect free |
| `ResetRuntimeStateAsync` | Development/test reset | Allowed policy | Structured result | Main thread |
| `ShutdownAsync` | Orderly cleanup | Claimed state | Pending results completed | Main thread |

### 10.3 Screen request fields

Operation kind, stable definition/ID, typed project wrapper/context seam, duplicate policy, bounded admission policy, transition/focus override, prior-screen visibility policy, owner scope, and diagnostic correlation ID. Project data is never written into definition assets.

### 10.4 Events

| Event | Timing |
|---|---|
| `Initialized` | After Ready committed |
| `ScreenOperationStarted/Completed` | After admission/at terminal state |
| `ScreenChanged` | After active/history truth changes |
| `ModalChanged/Completed` | After modal order/result commits |
| `HudRegionChanged` | After registration/visibility commit |
| `NotificationChanged`, `TooltipChanged`, `PromptChanged` | After service truth changes |
| `FocusChanged` | After EventSystem selection commit |
| `MotifChanged`, `AccessibilityChanged` | After application attempts finish |
| `StatusChanged` | Meaningful health/state change only |

Listeners are never required for the authoritative operation to finish.

### 10.5 Async and cancellation

- Every public async call creates a fresh Unity `Awaitable<T>`.
- Unity GameObject/EventSystem work remains on the main thread.
- Transitions and transient timing use unscaled time.
- Pre-admission cancellation causes no mutation.
- Cancellable transitions receive cancellation; noncancellable drivers are hard-bounded and forced to a deterministic end state.
- Modal awaiters complete exactly once; the first valid terminal result wins and stale later attempts are rejected harmlessly.
- Modal owner/view loss and shutdown after admission return explicit structural `Aborted` outcomes rather than project semantic Cancel.
- Equivalent re-entry follows duplicate policy.
- Awaitables are never cached/reused.

### 10.6 Ergonomics

**Novice:** setup tool, root prefab, screen definition, `OpenScreenAsync`.
**Programmer:** injected factories, clocks, transitions, EventSystem/focus adapters, typed presenters, narrow interfaces.
**Designer:** project prefabs, motifs, focus targets, transitions, safe-area settings, and validation.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

Install -> open Looking Glass Setup -> create/select config -> choose root/EventSystem policy -> preview layers/files -> apply create-only changes -> import Lab/templates -> validate -> save report.

### 11.2 Setup operations

| Operation | Creates/modifies | Repeat-safe | Protection |
|---|---|---:|---|
| Create config/root/EventSystem prefab/catalog/motif | New project-owned assets | Yes | Undo and report |
| Register screen/modal | Catalog entry/reference | Yes | Duplicate detection |
| Import Lab/templates | Samples/project copies | Yes | Package Manager/sample semantics |
| Repair root | Previewed deterministic missing components/references | Yes where safe | Undo + optional backup |
| Migrate config | Versioned project asset changes | Idempotent | Backup/report |

No operation silently overwrites project screens, motifs, layouts, text, input modules, or EventSystems.

### 11.3 Windows and inspectors

- Looking Glass Setup.
- Configuration, Screen Catalog, Modal Catalog, and Motif inspectors.
- UI Validation window.
- Runtime Stack inspector.
- Focus Map visualizer.
- Transition simulator.
- Setup/migration report viewer.
- Looking Glass Builder — create/batch-create/name/parent standardized primitives and screen/window roots.
- Motif authoring tools — create, capture from selection, apply to selection/children, preview, and preserve explicit local overrides.
- Surface Registry inspector — inspect stable IDs, roles, scopes, categories, open state, and select the authored object.

### 11.4 Validation registry

| Check | Condition | Severity | Auto-fix |
|---|---|---|---:|
| EUI-VAL-001 | Config missing | Blocker | Create only |
| EUI-VAL-002 | Duplicate root | Blocker | No |
| EUI-VAL-003/004 | Required layer missing/collision | Error | Safe missing component only |
| EUI-VAL-005/006 | Empty/duplicate screen ID or missing prefab | Error | No |
| EUI-VAL-007 | Invalid default focus | Warning/Error | No |
| EUI-VAL-008 | Modal view contract mismatch | Error | No |
| EUI-VAL-009 | HUD region missing | Warning/Error | Create host after preview |
| EUI-VAL-010/011 | Missing/multiple EventSystem | Blocker/Error | No silent delete |
| EUI-VAL-012 | Input module missing/incompatible | Warning/Error | Explicit create only |
| EUI-VAL-013/014 | Raycaster/CanvasGroup missing | Error | Previewed safe fix |
| EUI-VAL-015 | Notification bounds invalid | Error | No silent clamp |
| EUI-VAL-016 | Transition timing/timeout invalid | Error | No |
| EUI-VAL-017/018 | Motif fallback/sample dependency issue | Warning | No |
| EUI-VAL-019 | Safe-area config invalid | Warning | No |
| EUI-VAL-020 | Runtime references UnityEditor | Blocker | No |
| EUI-VAL-021 | Project asset under immutable package source | Error | Guidance only |
| EUI-VAL-022 | ID alias collision | Error | No |
| EUI-VAL-023 | Runtime depends on sample | Error | No |
| EUI-VAL-024/025 | Blocking modal or focus graph has no escape | Warning/Error | Visualizer/guidance |
| EUI-VAL-026 | Debug layer unexpectedly enabled in release | Warning/Error | Build config fix |

Validation runs manually, before Play, and before build. Auto-repair is previewed and limited to deterministic project-owned changes.

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Embedded development package, local path, UPM Git URL/tag, tarball, and later Workshop selection.

### 12.2 Minimal setup

One root, one configuration asset, layer hosts, one valid EventSystem path, one project screen definition/view, and one explicit caller that opens the first screen. No hidden scene, build index, input asset, tag, layer, or Resources path.

### 12.3 Boot setup

Boot/First Light creates or contains the persistent root. Project opens its initial screen after Ready. First Light retains its narrow fallback presenter until an explicit bridge replaces it. Scene-owned widgets register/dispose with scenes.

### 12.4 Direct-scene setup

A development-only initializer reuses an existing authority or creates the configured minimum root, follows normal duplicate safety, uses the chosen EventSystem policy, reports development mode, and is disabled/excluded from release by default. It creates no peer package authorities.

### 12.5 EventSystem/input-module policy

Setup must show exactly which EventSystem/input module path is selected: project-assigned, sample standalone module, optional Input System module, or later Will bridge. Core never enables input maps or assumes action names.

### 12.6 Isolation

The UI Laboratory contains only EchoUI, declared Unity dependencies, and redistributable sample assets. Integration scenes remain separate.

---

## 13. Standalone Test Lab and Samples

### 13.1 Laboratory purpose

Prove package-local authority, stable surface registration, exclusive scoped navigation, independent windows, context-driven visibility, focus/EventSystem behavior, later modal/HUD/transient services, Motifs/accessibility, transition failures, direct entry, and shutdown without any other Echo package. Early checkpoints use a deliberately simple Laboratory helper to inject context until project/controller adapters exist.

### 13.2 Laboratory contents

- Root/config plus a project-authored ordered layer topology; the Laboratory may use package-recommended starter definitions, but the runtime does not require a fixed count.
- Standalone EventSystem/input-module sample path.
- Home, Gallery, Settings Shell, Long List, Empty, and Error screens.
- Confirmation/choice modals.
- HUD regions.
- Notification stress controls.
- Edge/corner tooltips and prompt fallbacks.
- Two neutral motifs.
- Text scale/reduced motion/contrast/focus/timing controls.
- Focus/current selected readout.
- Delay/failure/cancel simulation.
- Duplicate-root/EventSystem controls.
- Reset and in-scene instructions.

### 13.3 Lab acceptance checklist

| ID | Action | Expected |
|---|---|---|
| LAB-001/002 | Direct entry / duplicate root | One dev authority / duplicate side-effect-free |
| LAB-003-008 | Open/push/back/replace/reset/rapid requests | Correct history, focus, and coalescing |
| LAB-009-013 | Modal stack, lower-owner close, result, owner loss | Blocking and exact-once safe completion |
| LAB-014-017 | Disable/remove focus targets; navigate; pointer mode | Fallback/no-focus and deterministic navigation |
| LAB-018/019 | Register/dispose/hide HUD | Correct region/lease state |
| LAB-020-023 | Notification coalesce/overflow/unscaled/accessibility | Bounds and timing policy |
| LAB-024-026 | Tooltip edges/anchor loss/prompt replacement | Safe placement and lifecycle |
| LAB-027-030 | Motif/fallback/text scale/reduced motion | Project asset immutable and policy applied |
| LAB-031-033 | Open/close failure/cancel queued operation | Deterministic nonblocking terminal state |
| LAB-034/035 | Missing/duplicate EventSystem | Actionable configured behavior |
| LAB-036/037 | Safe-area change / scene travel | Correct anchors and persistent root cleanup |
| LAB-038/039 | Shutdown active work / domain reload disabled | Awaiters cleanup and static reset |
| LAB-040-042 | Remove samples / public-safe diagnostics / repeat setup | Runtime isolation, privacy, idempotence |

### 13.4 Integration samples

First Light, Observatory, Accord, Passage, Pulse, Resonance, Will, and Chronicle each receive a separate bridge lab when their bridge ships. No integration or showcase counts as standalone proof.

---

## 14. Presentation, UI, and Accessibility

### 14.1 Ownership

EchoUI owns framework presentation lifecycle, visibility/interactivity, focus/navigation, transitions, and motif/accessibility application. Project presenters own domain interpretation, commands, final copy, localization choices, and production styling.

### 14.2 Required view states

Uninitialized, Ready, Entering, Active, Suspended, Busy, Empty, Disabled/Unavailable, Warning, Failure, Exiting, Cancelled/Interrupted, No Focus/Input, and Reduced Motion where applicable.

### 14.3 Focus/navigation requirements

Every interactive view declares default focus and back policy. Modal focus cannot escape lower layers. Pointer hover does not automatically steal controller focus unless configured. Targets must be active/interactable/in-scope. Dynamic content has deterministic fallback. No-selection is legal and must not trigger per-frame reselection spam. Focus visuals cannot rely only on color.

### 14.4 Silent initialization

Presenter captures state/draft -> view suppresses user callbacks -> controls populate -> availability/validation applies -> user subscriptions activate. Opening settings, saves, audio, graphics, accessibility, or rebind views must not fire sounds, saves, previews, or domain commands from initial binding.

### 14.5 Accessibility

The MVP proves keyboard/controller/pointer navigation, focus indicators, scalable text, contrast variants, reduced motion, extended/manual transient timing, color-independent status, text fallback for glyphs, safe-area handling, flash/repeated-animation suppression seams, and assistive-label metadata extension points. It does not claim native screen-reader support.

### 14.6 Customization and scaling

Project owns prefabs, Canvas/layout choices, fonts/fallbacks, sprites/icons/materials, motifs, transitions, focus visuals, and localized text. Setup validates CanvasScaler/reference resolution/anchors/safe-area choices but does not impose one universal resolution. Default templates use TMP-compatible text; production font assets remain project-owned.

---

## 15. Diagnostics and Observability

### 15.1 Standalone surfaces

API/inspector/log for lifecycle, layers, screen/modal summary, focus/EventSystem, active transition, queue counts, motif/accessibility IDs, bounded history, and validation report.

### 15.2 Snapshot fields

Package/schema, root/config/mode, layers and interaction gates, screen history/active ID, modal count/top ID, HUD counts, notification counts/drops/coalesces, tooltip/prompt active state, EventSystem/input module type where safe, selected target debug ID where safe, transition correlation/timing, motif/accessibility summary, last codes, and bounded operation metrics.

Public-safe output excludes rendered text, arbitrary view-model values, hierarchy/file paths, save/profile names, and user input.

### 15.3 Diagnostic codes

`EUI-LIFE-*`, `EUI-CONFIG-*`, `EUI-LAYER-*`, `EUI-EVENT-*`, `EUI-SCREEN-*`, `EUI-MODAL-*`, `EUI-FOCUS-*`, `EUI-TRANS-*`, `EUI-NOTIFY-*`, `EUI-TOOLTIP-*`, `EUI-PROMPT-*`, `EUI-MOTIF-*`, `EUI-SAFE-*`, `EUI-VIEW-*`, and `EUI-QUEUE-*` are stable searchable families. Detailed meanings and remediation are documented in the package reference.

### 15.4 Observatory bridge

A separate bridge maps redacted EchoUI status/events into Observatory provider vocabulary. It registers explicitly, obeys privacy ceilings, exposes no user text, and unregisters cleanly.

### 15.5 Logging

No per-frame focus spam. Equivalent warnings are throttled/coalesced. User-entered text and arbitrary view data are never logged. Errors include stable IDs and correlation IDs rather than only hierarchy paths.

---

## 16. Persistence and Save Integration

### 16.1 Classification

| State | Scope | Owner | Saved? |
|---|---|---|---:|
| Config/motif definitions | Project assets | Project/EchoUI types | Asset serialization |
| Motif/accessibility selection | Global preference | Accord/project | Optional/expected |
| Screen/modal/HUD/transient/focus state | Session | EchoUI | No by core |
| Save-slot metadata displayed | Slot/profile | Chronicle | By Chronicle |

### 16.2 Standalone

Without Accord/Chronicle, configuration defaults apply in memory and nothing is written to PlayerPrefs/files. Navigation/focus are not restored after restart.

### 16.3 Preference bridge

Accord may persist UI/text scale, contrast/motif variant, reduced motion, transient timing, focus indicator, and animation/flash settings. EchoUI validates/applies effective values; Accord owns draft/commit/rollback/storage/migration.

### 16.4 Save bridge

A save presenter queries Chronicle and submits operations. EchoUI never reads/writes files or assumes slot count. Persisting navigation state is project-owned and outside core.

### 16.5 Failure/recovery

Unknown motif IDs fall back without rewriting external data. Newer preference schemas are preserved. Save failures are rendered from Chronicle results. Shutdown completes/cancels pending UI operations but does not persist transient state.

---

## 17. Integration and Bridge Contracts

### 17.1 Philosophy

All peer connections are explicit, removable, versioned, and noncircular. Missing peers never break the core.

### 17.2 Planned integrations

| Peer | Bridge role |
|---|---|
| First Light | Styled startup/status/splash presenter and skip request |
| Observatory | Embed diagnostic panels and publish redacted UI status |
| Accord | Settings draft/result/confirmation presenters and UI policy applier |
| Passage | Fade/loading/progress/error presenter |
| Pulse | Menu/pause scope coordination through explicit leases |
| Resonance | Semantic UI cue requests after real interactions |
| Will | UI context/lock coordination, navigation/rebind/glyph/prompt data |
| Chronicle | Save-slot metadata and operation presenters |
| Workshop | Generate selected root/config/templates/report |
| Localization | Later localized-reference/font/refresh adapter |

### 17.3 Placement

Any assembly referencing EchoUI plus a peer is separate by default. Game-specific translation remains project code. UI Toolkit/native accessibility are provider adapters.

### 17.4 Failure behavior

Missing peer: bridge absent. Late peer: bridge registers and applies current state. Version mismatch: inactive with diagnostic. Peer shutdown: bridge releases leases/registrations and leaves defined fallback. UI shutdown: bridge unregisters and completes/cancels presentation. Presenter failure never changes peer authority.

### 17.5 Hard boundaries

- Accord owns apply/rollback; UI only displays/counts down.
- Passage owns loading/activation/recovery; UI only presents.
- Pulse owns pause/time/cursor; screen names never imply pause.
- Will owns maps/locks/rebind/device/glyph truth; EchoUI owns EventSystem focus.
- Resonance owns playback; silent binding never emits cues.
- Chronicle owns files/slots; UI never accesses storage directly.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

EchoUI is infrastructure for ordinary menus and HUDs, not a promise that every project-authored canvas or view will be inexpensive. The package must keep its own coordination overhead bounded and make expensive project presentation visible.

| Metric | MVP target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Root idle update cost | No mandatory per-frame scan of all registered views | Profiler in empty and populated UI Laboratory | No package-wide polling loop in normal idle state |
| Screen operation allocations | No recurring managed allocation after warmup for a settled screen stack | Profiler/GC allocation recorder | Zero package-owned recurring allocations while idle |
| Modal operation allocations | Bounded allocations per open/close transaction; no recurring idle allocation | Modal stress case | No unbounded growth after repeated cycles |
| Focus resolution | Resolve only on relevant lifecycle, hierarchy, device, or selection changes | Focus stress case | No full-scene EventSystem search every frame |
| Notification queue | Capacity constrained by configuration | Queue stress case | Never exceed configured pending/visible limits |
| Transition time | Bounded by definition and hard safety limit | Delayed presenter case | A failed or stalled presenter cannot block forever |
| Registration tables | Bounded by registered screens, HUD regions, and active views | Registry stress case | Released entries are removed and stale handles are rejected |
| Diagnostic history | Bounded ring buffers | Observatory/provider stress case | Never grows without limit |
| Hidden-layer behavior | No package-owned animation or layout loop for fully inactive layers | Profiler | Hidden package layers do not perform recurring package work |
| Sample scene | Stable across repeated reset/open/close cycles | UI Laboratory | No cumulative object, subscription, or queue growth |

The M1-M3 implementation checkpoints must record measured editor and player results rather than converting these design targets into invented millisecond guarantees.

### 18.2 Allocation policy

- Runtime coordination must be event-driven.
- The root must not use `FindObjectOfType`, broad hierarchy scans, LINQ-heavy queries, reflection discovery, or string-based type lookup in hot paths.
- Screen, modal, HUD, notification, prompt, tooltip, and focus registries must use stable keys and bounded collections.
- Public snapshots should be immutable/read-only and may be pooled or copied according to documented ownership.
- Project-authored view animation may allocate, but package diagnostics should distinguish package coordination from presenter/view costs where practical.
- Repeated navigation must not accumulate delegates, coroutines, cancellation registrations, or retained view references.
- Transition drivers must release all temporary state on completion, cancellation, replacement, root shutdown, or view destruction.
- String formatting for diagnostics should be deferred until a report or visible panel requests it.

### 18.3 Canvas and layout policy

The package does not prescribe one Canvas topology for every game, but the default root should avoid gratuitous rebuild coupling.

- Each major layer may use its own Canvas when that improves isolation and sorting.
- The final Canvas split is recorded during implementation profiling, not selected by folklore.
- Layout Groups, Content Size Fitters, masks, and animated hierarchy changes are project-authored costs and must be exercised in the Laboratory.
- Views should prefer activation/lifecycle policies that do not repeatedly reconstruct expensive hierarchies without need.
- Pooling views is not an MVP requirement. It may be introduced only after lifecycle correctness is proven and profiling shows value.
- World-space UI is project-owned and may register through the same presenter/view contracts without becoming part of the persistent root.

### 18.4 Scene and domain reload behavior

- Every event, callback, cancellation source, lease, and registry entry must be released deterministically.
- Static convenience access must reset through subsystem registration and root destruction paths that support the approved Enter Play Mode configurations.
- A duplicate root must leave no partial EventSystem, layer, transition, or registry state behind.
- Scene-owned views must unregister when their scene unloads.
- Persistent view instances must be explicitly classified; scene objects do not become persistent merely because they registered.
- Direct-scene helpers must use the same authority-claim and shutdown rules as production.
- Tests must cover domain reload enabled and the supported domain-reload-disabled workflow.
- Root shutdown must cancel pending operations, complete/cancel modal results exactly once, clear selection safely, and leave no dangling static access.

### 18.5 Scalability limits

The initial advertised limits are deliberately conservative and configurable:

| Resource | Default design limit | Required behavior at limit |
|---|---:|---|
| Registered screen definitions | 128 | Reject duplicate IDs; report capacity/configuration problem |
| Active screen history entries | 32 | Reject or collapse according to explicit policy; never grow silently |
| Active modals | 8 | Reject new modal with a structured result unless replacement is explicit |
| Pending modal requests | 16 | Reject overflow deterministically |
| Registered HUD regions | 32 | Reject duplicate keys and report |
| Visible notifications | 4 | Apply configured overflow policy |
| Pending notifications | 32 | Drop/reject/replace according to declared queue policy and report |
| Active prompt/tooltip owner | 1 per configured channel | Resolve by explicit priority and ownership token |
| Concurrent UI operations | 1 serialized structural operation plus bounded queue | Reject/coalesce/queue according to policy |
| Diagnostic history entries | 256 per bounded category | Overwrite oldest entry |

These are safe starting defaults, not universal engine limits. Increasing them requires Test Lab and performance evidence.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

EchoUI should not own credentials, save payloads, payment data, chat logs, analytics identities, or platform-account secrets. It may temporarily display project-provided text and images, including potentially sensitive information, so the package must avoid silently retaining or exporting view content.

The package's own diagnostics may include:

- Stable UI definition IDs.
- Layer, screen, modal, and focus state.
- Operation timings and result codes.
- EventSystem ownership state.
- Queue counts and capacity.
- Package and Unity version information.

They must not include by default:

- Text typed into input fields.
- Passwords, authentication codes, or save names.
- Player chat or dialogue content.
- Full filesystem paths supplied by peer systems.
- Platform account identifiers.
- Screenshots or rendered textures.
- Arbitrary presenter model payloads.

### 19.2 Trust boundaries

- Project-provided screen/modal IDs, arguments, presenter data, localized strings, rich-text content, sprites, and callbacks are untrusted inputs from EchoUI's perspective.
- IDs are validated for emptiness, collision, and configured capacity.
- Rich text remains a project/content policy; EchoUI must not interpret arbitrary markup as commands.
- Modal completion callbacks must be exception-isolated and invoked exactly once.
- View presenters cannot gain authority over unrelated UI entries through a reference to the root.
- Destructive confirmations expose intent/result contracts but do not execute the destructive game operation themselves.
- External URLs, file pickers, platform overlays, purchases, and account actions are project/provider responsibilities.
- Diagnostic export is local and explicit. EchoUI never transmits data automatically.

### 19.3 Accessibility and safety

- Important state cannot rely on color, animation, or audio alone.
- Flashing, rapid motion, screen shake, auto-advance, and time-limited reading behavior are project presentation choices and must accept effective accessibility policy when used.
- The default package transitions use unscaled time and must support immediate completion when reduced motion is active.
- Focus must never be intentionally trapped without an available close/back route unless the project explicitly declares a non-dismissable legal or safety screen.
- Modal interaction blocking must be deterministic so input cannot leak into obscured gameplay or screens.
- UI timeouts must not silently discard user work. Confirmation countdowns belong to the owning system's contract and receive visible, accessible presentation.

### 19.4 Platform behavior

| Platform | Initial status | Special behavior | Validation required |
|---|---|---|---|
| Windows | Supported target | Mouse, keyboard, controller, window/resolution changes from peer systems | Editor and standalone player |
| macOS | Supported target | Focus and full-screen behavior may differ | Native player |
| Linux | Supported target | Pointer confinement, fonts, and controller navigation require verification | Native player |
| WebGL | Planned/conditional | Browser focus, cursor, memory, file access, and fullscreen restrictions | Player build before claim |
| Android/iOS | Planned/conditional | Touch selection, safe areas, soft keyboard, suspend/resume | Device tests before claim |
| Console | Unknown until platform access | Platform navigation, account, safe-area, and certification requirements | Provider/platform review |
| XR | Deferred | World-space interaction and XR-specific EventSystem modules | Separate specification/adapter |

The first public release may claim only platforms actually tested during M6.

### 19.5 Safe-area and display adaptation

MVP provides extension seams and a sample adapter, not a universal layout solver.

- Project views own responsive layout.
- A safe-area provider may report display insets to registered layout adapters.
- The package must not hardcode one aspect ratio, reference resolution, notch rule, or DPI assumption.
- Dynamic resolution, split-screen, multiple displays, and camera-space canvases require project or later adapter design.
- View definitions may declare supported orientation/layout profiles, but unsupported presentation must fail visibly rather than silently stretching critical controls off-screen.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-ui/
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
│   │   ├── Screens and Modals.md
│   │   ├── HUD Notifications and Prompts.md
│   │   ├── Motifs and Accessibility.md
│   │   ├── UI Laboratory.md
│   │   └── Troubleshooting.md
│   └── Developer/
│       ├── Architecture.md
│       ├── Public API.md
│       ├── Lifecycle and Focus.md
│       ├── Extension Points.md
│       ├── Testing and Release.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Runtime/
│   ├── Core/
│   ├── Screens/
│   ├── Modals/
│   ├── HUD/
│   ├── Notifications/
│   ├── Prompts/
│   ├── Focus/
│   ├── Presentation/
│   ├── Motifs/
│   ├── Accessibility/
│   ├── Diagnostics/
│   ├── Configuration/
│   ├── Prefabs/
│   └── EchoDevGames.EchoUI.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Inspectors/
│   ├── Simulation/
│   └── EchoDevGames.EchoUI.Editor.asmdef
├── Samples~/
│   └── Standalone Labs/
│       └── Looking Glass UI Laboratory/
└── Tests/
    ├── Editor/
    │   └── EchoDevGames.EchoUI.Tests.Editor.asmdef
    └── Runtime/
        └── EchoDevGames.EchoUI.Tests.Runtime.asmdef
```

An optional bridge or Integration Lab does not enter the core package merely for convenience. Its location follows the mixed bridge rule after both sides are specified.

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoUIRoot.cs
│   ├── EchoUIConfiguration.cs
│   ├── EchoUIRuntimeState.cs
│   ├── EchoUIOperation.cs
│   ├── EchoUIOperationResult.cs
│   ├── EchoUIOperationHandle.cs
│   └── EchoUIShutdownReason.cs
├── Screens/
│   ├── ScreenDefinition.cs
│   ├── ScreenRegistry.cs
│   ├── ScreenRequest.cs
│   ├── ScreenEntry.cs
│   ├── ScreenHistory.cs
│   ├── ScreenOperationPolicy.cs
│   └── IScreenPresenter.cs
├── Modals/
│   ├── ModalDefinition.cs
│   ├── ModalRequest.cs
│   ├── ModalEntry.cs
│   ├── ModalResult.cs
│   ├── ModalHandle.cs
│   └── IModalPresenter.cs
├── HUD/
│   ├── HudRegionDefinition.cs
│   ├── HudRegionRegistry.cs
│   ├── HudVisibilityRequest.cs
│   └── IHudRegionPresenter.cs
├── Notifications/
│   ├── NotificationRequest.cs
│   ├── NotificationEntry.cs
│   ├── NotificationQueue.cs
│   ├── NotificationHandle.cs
│   └── INotificationPresenter.cs
├── Prompts/
│   ├── PromptRequest.cs
│   ├── PromptEntry.cs
│   ├── PromptHandle.cs
│   ├── TooltipRequest.cs
│   └── IPromptPresenter.cs
├── Focus/
│   ├── UIFocusCoordinator.cs
│   ├── FocusRequest.cs
│   ├── FocusSnapshot.cs
│   ├── FocusFallbackPolicy.cs
│   ├── EventSystemPolicy.cs
│   └── IEventSystemAdapter.cs
├── Presentation/
│   ├── EchoUIView.cs
│   ├── UIViewLifecycleContext.cs
│   ├── UIViewOperationId.cs
│   ├── UIViewTransitionDefinition.cs
│   ├── IUIViewPresenter.cs
│   ├── IUIViewFactory.cs
│   ├── IUIViewTransitionDriver.cs
│   └── UGUI/
├── Motifs/
│   ├── UIMotif.cs
│   ├── UIMotifToken.cs
│   ├── UIMotifResolver.cs
│   └── IMotifConsumer.cs
├── Accessibility/
│   ├── UIAccessibilityPolicy.cs
│   ├── UIAccessibilitySnapshot.cs
│   └── IUIAccessibilityProvider.cs
├── Diagnostics/
│   ├── EchoUIDiagnosticCode.cs
│   ├── EchoUIStatusSnapshot.cs
│   ├── EchoUIOperationRecord.cs
│   └── IEchoUIDiagnosticsProvider.cs
├── Configuration/
│   ├── UIRegistryConfiguration.cs
│   ├── UILayerConfiguration.cs
│   ├── UICapacityConfiguration.cs
│   └── UITransitionPolicy.cs
└── Prefabs/
    └── EchoUIRoot.prefab
```

Names remain subject to implementation-level refinement without changing the approved responsibilities.

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoUI.Runtime` | Runtime | UnityEngine, `UnityEngine.UI`; TMP-compatible assembly only when verified for baseline | Yes | Core runtime and default uGUI presenters |
| `EchoDevGames.EchoUI.Editor` | Editor | Runtime assembly, UnityEditor, Unity UI editor APIs | No | Setup, validation, custom inspectors, simulation |
| `EchoDevGames.EchoUI.Tests.Editor` | Editor tests | Runtime and Editor assemblies, Test Framework | No | Validation and authoring tests |
| `EchoDevGames.EchoUI.Tests.Runtime` | PlayMode tests | Runtime assembly, Test Framework | No | Lifecycle and behavior tests |

The default package may split backend-specific presenters into a second runtime assembly if doing so materially improves dependency isolation. That is an implementation packaging choice, not permission to make UI Toolkit a second MVP backend.

### 20.4 Repository files

The package repository must include:

- Concise routed README.
- Complete `Documentation~`.
- Visible Current Notes link.
- Changelog.
- License and third-party notices.
- Contribution/development guidance.
- Security/support reporting guidance.
- Release checklist.
- Stable committed `.meta` files.
- Package manifest and samples metadata.
- Compatibility and integration index.
- Obsidian-compatible links among specification, ADRs, checkpoints, tests, guides, and issue records.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Primary tested baseline | Notes |
|---|---|---|---|
| Unity | 6000.0 | 6000.3.8f1 | Exact public support matrix is revalidated at M1 and M6 |
| Unity UI / uGUI | Unity 6 compatible released package | Version bundled/verified with baseline | Exact manifest version is recorded at M1 |
| TextMeshPro-compatible uGUI text | Unity 6 compatible path | Verified with baseline | Exact assembly/package relationship is recorded at M1 |
| Unity Test Framework | Compatible with supported Unity | Baseline project version | Test-only |
| Other Sperk’s Forge packages | None | N/A | Optional bridges only |

No broader platform, render pipeline, or Unity-minor compatibility is claimed until tested.

### 21.2 Semantic versioning policy

**Patch version:**

- Bug fixes that preserve documented API and serialized meaning.
- Additional diagnostic messages.
- Safe validation improvements.
- Sample/documentation corrections.
- Performance improvements with unchanged visible contract.

**Minor version:**

- Backward-compatible screen/modal/HUD capabilities.
- New optional presenters, focus policies, notification policies, motif tokens, or diagnostics.
- New optional bridge support.
- New serialized fields with safe defaults.
- Additional sample scenarios.

**Major version:**

- Breaking public API changes.
- Changes to lifecycle ordering, focus semantics, modal completion, history behavior, or ownership.
- Incompatible serialized configuration or definition changes.
- Required dependency changes.
- Removal or reinterpretation of stable IDs.
- A backend architecture change that invalidates existing project views.

### 21.3 Deprecation policy

- Deprecated public APIs receive compiler/documentation warnings for at least one compatible minor release when practical.
- Replacement and migration guidance must exist before deprecation.
- Serialized fields are retained or migrated without losing project configuration.
- Removal occurs only in a major version unless the API is unsafe or unusable and an ADR records the exception.
- Deprecated sample patterns are clearly labeled and removed only after the supported migration path is validated.
- The changelog identifies behavior changes even when source compatibility is preserved.

### 21.4 GUID and asset compatibility

- Public scripts, prefabs, default configurations, definitions, motifs, samples, and asmdefs keep committed `.meta` files.
- File moves and renames retain GUIDs when identity survives.
- Screen/modal definition stable IDs do not derive solely from display names or paths.
- Duplicate ID validation blocks release.
- An ID changed after release requires alias/migration support.
- Package updates must not overwrite project-owned screens, motifs, prefabs, or generated configuration.

### 21.5 Backend compatibility policy

uGUI is the first approved runtime backend. UI Toolkit support is deferred to a separately approved adapter or major expansion.

- Public core contracts should avoid leaking unnecessary uGUI implementation details.
- The default release may expose uGUI-specific components where they are genuinely presentation types.
- Projects may implement custom presenters without replacing the root's authority.
- Mixed uGUI/UI Toolkit projects require explicit focus and sorting integration; they are not silently treated as fully supported.
- Backend adapters must preserve screen, modal, result, focus, diagnostics, and accessibility semantics.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview, authority, and exclusions.
- Installation through Git, tarball/local path, embedded development, and Workshop when available.
- Five-minute quick start.
- Creating configuration and root prefab.
- Registering the first screen.
- Push, replace, reset, and back behavior.
- Creating and awaiting a modal result.
- Registering HUD regions.
- Notifications, prompts, and tooltips.
- EventSystem policies and focus troubleshooting.
- Motif creation and project-owned customization.
- Accessibility policy and reduced-motion behavior.
- Direct-scene initialization.
- UI Laboratory guide.
- Diagnostic code reference.
- Known limitations.
- Upgrade/migration guide.
- Optional integration index.
- License, credits, and third-party notices.

### 22.2 Required developer documentation

- Layer and authority topology.
- Screen, modal, and operation lifecycles.
- View/presenter separation.
- EventSystem and focus algorithm.
- Transition cancellation/replacement semantics.
- Stable ID and registry rules.
- Motif and accessibility contracts.
- Extension interfaces and custom presenter examples.
- Diagnostics provider contract.
- Testing strategy and release workflow.
- Architecture decisions and ADR index.
- Current checkpoint/status record.
- Linked `Current Notes.md`.

### 22.3 Documentation truth rule

- All code examples must compile against the documented release.
- Setup menu paths and screenshots must match the tested Unity baseline.
- Lifecycle ordering must match automated tests.
- Diagnostic code tables must match runtime constants.
- Sample controls must match the imported Laboratory.
- No feature may be advertised as backend-neutral, accessible, platform-supported, or integration-ready without corresponding evidence.
- A specification change discovered during implementation must be approved and documented before the code silently diverges.

### 22.4 Living repository and Obsidian workflow

- Package documentation lives in Git beside implementation.
- Obsidian opens those exact Markdown files.
- `Current Notes.md` captures active observations, proposals, tests, defects, risks, and handoff information.
- Proposals remain visibly provisional until approved.
- At meaningful checkpoints, durable decisions move into this specification or an ADR; defects and evidence move into issue/test records; user-visible changes move into guides and changelog.
- Resolved notes may be condensed after promotion because Git preserves history.
- Device-specific Obsidian state remains untracked unless explicitly adopted by the repository.
- Documentation changes are committed with or immediately adjacent to the implementation they describe.

### 22.5 Repository scan and handoff order

Before changing EchoUI:

1. Read repository README/index.
2. Read SFGSS-000.
3. Read this approved specification.
4. Read applicable ADRs and integration specifications.
5. Read `Current Notes.md`.
6. Read current checkpoint, tests, issue log, and changelog.
7. Inspect relevant implementation and automated tests.

### 22.6 Required diagrams and references

Developer documentation should include:

- Root/layer topology.
- Screen push/replace/reset/back sequence.
- Modal open/result/close lifecycle.
- Focus resolution fallback.
- Structural operation serialization.
- View-presenter-data direction.
- Optional bridge map.
- Duplicate-root claim and teardown.
- Direct-scene helper decision flow.

Diagrams explain ownership and timing; they do not replace normative prose and tests.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | Definitions, stable IDs, validation, policies, bounded collections, focus selection rules | Registry collision, queue overflow, transition policy, motif fallback | Yes |
| PlayMode unit/integration | Root lifecycle, screen/modal operations, view transitions, EventSystem/focus, owner loss | Duplicate root, push/back, out-of-order modal close, stale handles | Yes |
| Standalone Test Lab | User-visible isolated UI framework loop | Screens, HUD, modals, notifications, prompts, focus, motifs, accessibility | Yes |
| Bridge Integration Lab | One explicit peer connection | Pulse scope, Will context, Passage loading, Accord settings | When bridge ships |
| Showcase | Combined presentation and portfolio polish | Multi-package shell | No |
| Clean-project install | Packaging and dependency independence | Git/local/tarball install, sample import/removal | Yes |
| Existing-project migration | Adoption without regressions | One Rescuers2D or Echo Systems Lab UI slice | Before integration claim |
| Performance/profile | Bounded overhead and resource cleanup | Idle allocation, stress cycles, Canvas/layout profile | Yes |

### 23.2 Required test categories

- Clean compilation and all supported installation routes.
- Runtime/Editor assembly isolation.
- Canonical and direct-scene startup.
- Duplicate roots before and after initialization.
- Missing/invalid configuration.
- Duplicate/missing stable IDs.
- Screen push, replace, reset, back, coalescing, queueing, cancellation, and failure.
- Modal ordering, exact-once results, owner loss, capacity, and shutdown.
- HUD registration and visibility ownership.
- Notification, prompt, and tooltip bounds/lifecycle.
- EventSystem adoption, creation, conflict, and missing-system policies.
- Focus defaults, fallback, restoration, containment, and device-mode changes.
- Silent initialization and callback suppression.
- Motif immutability, fallback, scaling, contrast, and reduced motion.
- Transition cancellation, stale completion, timeout, and exception isolation.
- Scene unload, domain reload options, and application shutdown.
- Redacted diagnostics and bounded histories.
- Sample removal and setup repeatability.
- Optional bridges absent/present.
- Upgrade, migration, package removal, and real-project adoption.
- Idle allocation and repeated stress cycles.

### 23.3 Test environments

At minimum, release evidence includes:

1. Supported clean Unity project.
2. Embedded package development workspace.
3. Tarball/local package installation in a second clean project.
4. Standalone UI Laboratory imported from `Samples~`.
5. Player build on every platform claimed by the release.
6. One real EchoDevGames project integration after standalone proof.
7. Supported Enter Play Mode settings, including the documented no-domain-reload path.
8. Input with mouse, keyboard, and controller when those devices are claimed by the sample.

### 23.4 Test case registry

| Test ID | Requirement | Setup/action | Expected result | Automated? | Status |
|---|---|---|---|---:|---|
| EUI-T-001 | Clean install | Install package in supported clean project | Project compiles with declared dependencies only | Yes | Not run |
| EUI-T-002 | Embedded package | Embed package for development | Runtime and Editor assemblies compile without circular references | Yes | Not run |
| EUI-T-003 | Tarball install | Install generated tarball | Manifest, samples, documentation, and assemblies import cleanly | Manual | Not run |
| EUI-T-004 | Samples removed | Remove all imported samples | Runtime package continues to compile and function | Yes | Not run |
| EUI-T-005 | Peer packages absent | Install EchoUI alone | Standalone root and Laboratory function with no peer Echo package | Yes | Not run |
| EUI-T-006 | Canonical root startup | Enter configured boot scene | Exactly one authority initializes and reaches Ready | Yes | Not run |
| EUI-T-007 | Duplicate pre-existing root | Place two roots before play | One claims authority; duplicate performs no side effects and is removed/rejected | Yes | Not run |
| EUI-T-008 | Duplicate introduced later | Load scene containing second root | Existing authority survives; duplicate is rejected without registry/EventSystem changes | Yes | Not run |
| EUI-T-009 | Direct-scene initializer | Enter Laboratory without canonical root | Development root is created once and marked development-initialized | Yes | Not run |
| EUI-T-010 | Shutdown | Destroy/quit with active operations | Operations terminate, modal awaiters complete/cancel once, registries/static access clear | Yes | Not run |
| EUI-T-011 | Missing configuration | Start root without required configuration | Initialization fails visibly with structured blocker and no partial authority state | Yes | Not run |
| EUI-T-012 | Invalid capacity | Configure zero/negative/out-of-range capacity | Validation blocks or safely clamps according to documented rule | Yes | Not run |
| EUI-T-013 | Duplicate stable screen ID | Register conflicting definitions | Conflict is rejected and both sources are identified | Yes | Not run |
| EUI-T-014 | Duplicate modal ID | Register conflicting modal definitions | Conflict is rejected with diagnostic | Yes | Not run |
| EUI-T-015 | Missing view prefab/factory | Request definition without creatable view | Request returns failure; history is unchanged | Yes | Not run |
| EUI-T-016 | Safe setup repeat | Run Create/Repair twice | Second run makes no duplicate root, layers, EventSystem, or project assets | Yes | Not run |
| EUI-T-017 | Repair preview | Run repair on damaged root | Preview lists exact changes before apply and preserves project-owned objects | Manual | Not run |
| EUI-T-018 | Registry disposal | Dispose a dynamic registration | Entry becomes unavailable; stale registration token cannot remove a replacement | Yes | Not run |
| EUI-T-019 | Open first screen | Open registered screen from empty history | View enters, becomes active, receives focus, and history has one entry | Yes | Not run |
| EUI-T-020 | Push screen | Push second screen | Previous screen follows suspension policy; new screen becomes active | Yes | Not run |
| EUI-T-021 | Back navigation | Request Back with two entries | Top exits, previous resumes, focus restores deterministically | Yes | Not run |
| EUI-T-022 | Back at root | Request Back with one non-dismissible root screen | Structured unavailable/rejected result; history remains valid | Yes | Not run |
| EUI-T-023 | Replace screen | Replace active screen | Old entry exits and is removed; replacement occupies its history position | Yes | Not run |
| EUI-T-024 | Reset screen history | Reset to target | All prior entries terminate in order and one target remains | Yes | Not run |
| EUI-T-025 | Repeated same request | Issue duplicate open requests rapidly | Configured coalescing/rejection prevents duplicate entries | Yes | Not run |
| EUI-T-026 | Queued structural requests | Submit bounded sequence during transition | Requests execute in declared order or return declared overflow result | Yes | Not run |
| EUI-T-027 | Cancelled queued request | Cancel request before execution | Request never changes visible state and returns Cancelled | Yes | Not run |
| EUI-T-028 | View entry failure | Presenter/view throws during enter | Operation fails, partial view cleans up, prior stable state remains or recovers | Yes | Not run |
| EUI-T-029 | View exit failure | Presenter/view throws during exit | Operation reaches bounded terminal failure and root remains usable | Yes | Not run |
| EUI-T-030 | Scene-owned screen unload | Unload scene containing registered/active view | Registration and active entry resolve according to owner-loss policy without dangling focus | Yes | Not run |
| EUI-T-031 | Open blocking modal | Open one modal | Top modal becomes interactive; lower Looking Glass pointer/navigation interaction is blocked without changing gameplay-input authority | Yes | Not run |
| EUI-T-032 | Nested modals | Open second modal | Top modal alone is interactive; lower modal remains registered | Yes | Not run |
| EUI-T-033 | Complete top modal | Complete top modal with project result ID | Exact stable result ID is delivered once; lower modal resumes eligibility | Yes | Not run |
| EUI-T-034 | Close lower modal by handle | Dispose/complete lower modal out of order | Only targeted entry closes; top remains correct; stack/order stays valid | Yes | Not run |
| EUI-T-035 | Repeated modal completion | Complete same modal twice / race completion paths | First terminal result wins; later attempts return stale/already-completed result and cannot settle again | Yes | Not run |
| EUI-T-036 | Modal owner/view lost | Destroy admitted modal view unexpectedly | Awaiter completes exactly once as structural `Aborted`; no semantic Cancel ID is fabricated; blocking clears safely | Yes | Not run |
| EUI-T-037 | Modal capacity | Open beyond configured active limit | Overflow request is rejected without disturbing existing modals | Yes | Not run |
| EUI-T-038 | Modal queue overflow | Queue beyond configured limit | Deterministic overflow result and diagnostic; no memory growth | Yes | Not run |
| EUI-T-039 | Modal Back policy | Request Back on dismissible and non-dismissible definitions | Dismissible Back completes with configured stable result ID; disabled Back leaves modal active and returns blocked/unhandled result | Yes | Not run |
| EUI-T-040 | Root shutdown with modal | Shutdown with awaited modal | Awaiter completes exactly once as structural `Aborted` with shutdown reason | Yes | Not run |
| EUI-T-041 | HUD registration | Register HUD region | Region becomes addressable by stable ID with initial visibility policy | Yes | Not run |
| EUI-T-042 | HUD visibility lease | Acquire and release visibility request | Effective visibility follows priority/policy and restores after release | Yes | Not run |
| EUI-T-043 | HUD owner loss | Destroy registered HUD region | Registry removes it and reports unavailable without root failure | Yes | Not run |
| EUI-T-044 | Notification enqueue | Post notification | Notification displays in configured channel and completes/removes on policy | Yes | Not run |
| EUI-T-045 | Notification coalescing | Post same coalescible key repeatedly | Visible/pending entry updates according to policy instead of multiplying | Yes | Not run |
| EUI-T-046 | Notification overflow | Exceed queue capacity | Configured reject/drop/replace policy applies and diagnostic count increments | Yes | Not run |
| EUI-T-047 | Notification unscaled timing | Pause game time while notification active | Configured real-time duration continues using unscaled clock | Yes | Not run |
| EUI-T-048 | Manual-duration notification | Apply accessibility manual-dismiss policy | Notification remains until explicit dismissal | Yes | Not run |
| EUI-T-049 | Prompt replacement | Submit higher-priority prompt | Ownership changes deterministically; stale handle cannot clear replacement | Yes | Not run |
| EUI-T-050 | Prompt owner loss | Destroy prompt owner | Prompt clears or falls back safely | Yes | Not run |
| EUI-T-051 | Tooltip edge placement | Show tooltip near screen boundary | Placement adapter keeps content within configured safe area | Manual | Not run |
| EUI-T-052 | Tooltip anchor loss | Destroy anchor while visible | Tooltip closes safely without recurring errors | Yes | Not run |
| EUI-T-053 | Adopt assigned EventSystem | Configure assigned system | Root uses it and does not create another | Yes | Not run |
| EUI-T-054 | Adopt existing EventSystem | No assigned system; one valid scene system exists | Root adopts it according to policy | Yes | Not run |
| EUI-T-055 | Create missing EventSystem | No system exists and CreateIfMissing is selected | One configured system/module is created and reported | Yes | Not run |
| EUI-T-056 | Require external EventSystem | No system exists and RequireExternal is selected | Initialization/view interaction reports blocker without hidden creation | Yes | Not run |
| EUI-T-057 | Multiple EventSystems | Two active systems exist | Root reports conflict and follows configured non-destructive policy | Yes | Not run |
| EUI-T-058 | Default focus | Open interactive view | Declared valid default target is selected after entry | Yes | Not run |
| EUI-T-059 | Invalid default focus | Default target disabled/non-interactable | Fallback chain selects first valid candidate or legal no-selection state | Yes | Not run |
| EUI-T-060 | Focus restoration | Push and back | Previously valid target restores; invalid target follows fallback | Yes | Not run |
| EUI-T-061 | Pointer/controller switching | Alternate pointer and navigation input | Focus visuals/selection follow configured policy without jitter or unwanted stealing | Manual | Not run |
| EUI-T-062 | Modal focus containment | Navigate while modal open | Selection cannot escape configured modal scope | Yes | Not run |
| EUI-T-063 | Motif application | Assign project motif | Registered consumers receive resolved tokens without mutating motif asset | Yes | Not run |
| EUI-T-064 | Missing motif token | Request absent token | Fallback/diagnostic applies without null crash | Yes | Not run |
| EUI-T-065 | Runtime motif asset safety | Exercise views repeatedly | ScriptableObject motif remains unchanged after play/reset | Yes | Not run |
| EUI-T-066 | Text scale policy | Apply larger effective scale | Participating views update and remain navigable/readable in Laboratory | Manual | Not run |
| EUI-T-067 | Reduced motion | Enable reduced motion | Transitions complete immediately or through approved reduced variant | Yes | Not run |
| EUI-T-068 | Transition cancellation | Replace/cancel during enter | Stale transition cannot complete newer operation; final state is deterministic | Yes | Not run |
| EUI-T-069 | Transition timeout | Use presenter that never reports completion | Hard bound produces failure/recovery and root remains usable | Yes | Not run |
| EUI-T-070 | Silent data binding | Populate controls from presenter state | User callbacks, audio requests, persistence, and commands do not fire | Yes | Not run |
| EUI-T-071 | Status snapshot | Request diagnostics | Snapshot accurately reports redacted layer/history/modal/focus/queue state | Yes | Not run |
| EUI-T-072 | Bounded history | Exceed diagnostic history capacity | Oldest entries roll off; memory remains bounded | Yes | Not run |
| EUI-T-073 | Privacy-safe export | Export public-safe snapshot | No visible text, typed input, arbitrary model data, profile names, or hierarchy paths appear | Yes | Not run |
| EUI-T-074 | Idle allocation | Leave settled UI active after warmup | No recurring package-owned GC allocation in measured idle window | Manual | Not run |
| EUI-T-075 | Stress cycles | Run 1,000 open/close/modal/notification cycles | No cumulative registrations, subscriptions, views, queues, or handles | Manual | Not run |
| EUI-T-076 | Diagnostic listener failure | Observer throws during status/event callback | UI operation succeeds/fails on its own merits; observer is isolated | Yes | Not run |
| EUI-T-077 | First Light bridge absent/present | Run alone then with bridge | Core behavior unchanged; bridge adds only explicit startup presentation | Manual | Not run |
| EUI-T-078 | Pulse bridge lease cleanup | Open/close pause UI through bridge | Pulse scope is acquired/released exactly once; EchoUI does not set timeScale | Yes | Not run |
| EUI-T-079 | Will bridge cleanup | Open/close modal through bridge | Input context/lock leases release on success, failure, owner loss, and shutdown | Yes | Not run |
| EUI-T-080 | Passage bridge failure | Simulate scene transition failure | UI presents result but Passage remains authority and can recover independently | Manual | Not run |
| EUI-T-081 | Accord settings integration | Open/cancel/apply settings presenter | EchoUI does not persist or apply settings; silent binding and result flow are correct | Manual | Not run |
| EUI-T-082 | Upgrade previous version | Upgrade supported prior package/configuration | Assets preserve GUIDs and migration/validation reports exact changes | Manual | Not run |
| EUI-T-083 | Removal | Remove EchoUI after removing bridges/project references | Unrelated packages compile; project-owned data is not deleted | Manual | Not run |
| EUI-T-084 | External project adoption | Integrate one screen/modal/HUD slice into real project | Parity checklist passes and original UI remains available for rollback | Manual | Not run |
| EUI-T-085 | Blocking modal preserves external gameplay authority | Keep a Laboratory-owned external action source active while modal is open | External project action may continue; Looking Glass blocks only its lower UI interaction and does not switch gameplay input/pause state | Manual | Not run |
| EUI-T-086 | Screen request rejected by modal | Use default Reject policy and submit Screen mutation while blocking modal is active | Request returns explicit modal-blocked result with no Screen history mutation | Yes | Not run |
| EUI-T-087 | Screen request deferred by modal | Use DeferUntilModalStackClears and submit multiple Screen requests | Deferred requests remain bounded and settle in original FIFO order only after modal stack empties | Yes | Not run |
| EUI-T-088 | Stale modal handle generation | Reopen same modal definition after prior completion and reuse old handle | Old handle cannot complete/abort the new generation | Yes | Not run |

### 23.5 Evidence records

Each release candidate records:

- Unity/editor and package versions.
- Commit and package version.
- Test platform and input devices.
- Automated test result file.
- Laboratory checklist.
- Clean-install and tarball-install report.
- Performance captures and observed limits.
- Known failures, deferrals, and issue links.
- Screenshots/video only as supplementary evidence.
- Documentation commit confirming examples and menu paths.

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Identity, authority, and exclusions align with SFGSS-000.
- [x] Independence contract is explicit.
- [x] MVP and deferred scope are separated.
- [x] Persistent-root lifecycle and duplicate protection are defined.
- [x] Screen, modal, HUD, notification, prompt, focus, motif, and transition contracts are defined.
- [x] Public API direction and stable-ID rules are defined.
- [x] Standalone Laboratory and test registry are designed.
- [x] Required dependencies and backend boundary are explicit.
- [x] No release-blocking architecture question remains.
- [x] Specification is approved for future implementation.
- [x] Historical Foundation documentation/consistency gate is satisfied; current implementation is package-locally gated by JIT review + checkpoint authority.

### 24.2 Implementation gate

- [ ] Package manifest and assemblies compile with declared dependencies only.
- [ ] Editor code does not leak into runtime assemblies.
- [ ] Root claims authority before side effects.
- [ ] Registries and stable-ID validation are implemented.
- [ ] Screen operations are serialized and bounded.
- [ ] Modal results complete exactly once.
- [ ] Focus/EventSystem policies match specification.
- [ ] Transitions have cancellation, stale-operation, and timeout protection.
- [ ] Motif/accessibility assets remain immutable at runtime.
- [ ] Setup/repair operations are previewable, repeatable, and non-destructive.
- [ ] Public API matches this specification or the specification/ADR was updated first.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Package works without unrelated Sperk’s Forge packages.
- [ ] UI Laboratory imports and passes.
- [ ] Sample deletion does not break runtime.
- [ ] Direct-scene entry behaves as documented.
- [ ] Duplicate-root scenarios pass.
- [ ] Missing/invalid configuration scenarios pass.
- [ ] No project-specific scene name, input asset, save file, or content is required.

### 24.4 Quality gate

- [ ] Required automated tests pass.
- [ ] All 42 Laboratory scenarios pass.
- [ ] Relevant entries in the 84-case registry pass.
- [ ] No blocker or critical defect remains.
- [ ] Idle and stress performance evidence passes.
- [ ] Diagnostics are actionable and privacy-safe.
- [ ] Accessibility MVP checklist passes.
- [ ] Documentation examples compile and match behavior.
- [ ] Current Notes is reconciled.
- [ ] Durable decisions are promoted to specification/ADR.
- [ ] Licenses and notices are complete.

### 24.5 Distribution gate

- [ ] Package manifest is valid.
- [ ] Version and changelog are updated.
- [ ] Public assets retain stable `.meta` files.
- [ ] Git/local/tarball installations pass in another clean project.
- [ ] Upgrade from the previous supported version passes.
- [ ] Package removal behavior passes.
- [ ] Repository tag/release is prepared.
- [ ] Documentation/status is committed and pushed.
- [ ] Central compatibility catalog is updated.
- [ ] Only tested platforms are claimed.

### 24.6 Definition of done

The MVP is done when a clean project can install EchoUI alone, create or repair one protected root, register project-owned screens and a motif, push/back/replace/reset screens, open and resolve modals, display HUD regions, notifications, prompts, and tooltips, coordinate deterministic focus and EventSystem behavior, apply accessibility-aware transitions, inspect actionable diagnostics, and prove the full loop in an isolated Laboratory without any peer Echo package.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing UI surface | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Rescuers2D | Main, Pause, Win, settings, password/progression, shared HUD/menu coordination | Introduce EchoUI root in isolation; migrate one noncritical screen, then modal/focus, then one HUD region; connect peers only through explicit adapters | Existing navigation, focus, pause/settings behavior, and presentation remain functional in player build | Preserve original prefabs/controllers and switch via scene/config flag until parity |
| Echo Systems Lab | Hub/mission/HUD/menu presentation | Use as second proof of neutral presenter/data separation and portfolio documentation | One screen plus HUD slice uses EchoUI without project rule migration into package | Keep original UI scene/prefabs and integration branch |
| Don’t Get Vince’d | Dialogue, HUD, pause/results presentation candidates | Later diversity target after core and first integration | Beat-’em-up flow proves package is not shaped only for menu-heavy projects | Original managers/prefabs retained |

No project migration begins before the standalone package reaches the applicable checkpoint gate.

### 25.2 Preserve-until-parity rule

1. Inventory existing screens, presenters/controllers, EventSystem, Canvas hierarchy, navigation, settings/save/pause/audio connections, and scene assumptions.
2. Preserve the existing UI intact.
3. Install and pass EchoUI in isolation.
4. Choose one low-risk vertical slice.
5. Build a project-local adapter rather than copying project rules into the package.
6. Compare navigation, focus, pointer/controller behavior, visual states, accessibility, scene transitions, and failure cases.
7. Record parity gaps.
8. Remove the old slice only after parity and rollback tests pass.
9. Repeat by feature category.
10. Delete legacy authority only after the replacement checkpoint is committed and documented.

### 25.3 Migration tooling

Potential migration tools may:

- Detect existing EventSystems and Canvas roots.
- Inventory screens/prefabs and common navigation references.
- Create project-owned definitions that reference existing view prefabs.
- Generate a dry-run mapping report.
- Preserve original prefabs and controllers.
- Detect duplicate stable IDs and missing focus targets.
- Validate bridge/package dependencies.
- Create backups before transforming project-owned assets.
- Produce an explicit rollback report.

They must not:

- Rewrite project UI scripts automatically without a narrowly approved converter.
- Delete old prefabs/scenes.
- Reparent arbitrary hierarchies silently.
- Replace fonts, motifs, navigation, or input modules without preview.
- Treat a successful import as parity.

### 25.4 Adoption evidence

An integration claim requires:

- Before/after architecture note.
- Migrated feature inventory.
- Parity checklist.
- Player-build validation.
- Rollback test.
- Performance comparison where relevant.
- Project-specific issues separated from package defects.
- Documentation and changelog update.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EUI-R-001 | Scope inflation into a complete game shell | High | High | Enforce ownership contract, MVP matrix, and bridge boundaries | Any request to add save/settings/pause/game rules; Jesse |
| EUI-R-002 | UI becomes authority for domain state | Medium | High | Presenter/request/result contracts; tests prove listeners are not required for domain completion | Presenter begins mutating peer truth directly |
| EUI-R-003 | Duplicate persistent roots/EventSystems | High | High | Claim before side effects; explicit EventSystem policy; duplicate tests | Multiple scenes/imported samples |
| EUI-R-004 | Focus instability across dynamic content/devices | High | High | Deterministic fallback, stale operation IDs, containment, no per-frame reselection | Selection loss/jitter reports |
| EUI-R-005 | Transition races leave views half active | Medium | High | Serialized structural operations, cancellation, operation IDs, hard timeout, rollback | Rapid navigation/exception |
| EUI-R-006 | Modal awaiters leak or complete twice | Medium | High | Generational handles and exact-once terminal state tests | Owner loss/out-of-order close |
| EUI-R-007 | Hidden uGUI/package-version drift | Medium | Medium | Verify exact Unity/package relationship at M1/M6; avoid guessed manifest values | Unity baseline changes |
| EUI-R-008 | Canvas/layout performance regressions | Medium | Medium | Profile real Laboratory and project views; keep coordinator event-driven | Large HUD/menu adoption |
| EUI-R-009 | Mutable shared motif/config assets | Medium | High | Runtime resolved snapshots; immutability tests | Play Mode contamination |
| EUI-R-010 | Setup overwrites project content | Low | High | Dry run, create-safe defaults, backups, exact report, repeat tests | Repair/migration action |
| EUI-R-011 | Sample becomes runtime dependency | Low | High | Separate assemblies and sample-removal gate | Runtime reference to sample |
| EUI-R-012 | UI backend leaks into core contracts | Medium | Medium | Narrow presenter interfaces and documented uGUI-specific surface | UI Toolkit adapter work |
| EUI-R-013 | Accessibility claims exceed evidence | Medium | High | Explicit MVP/limitations; test actual navigation/scaling/motion; no false screen-reader claim | Marketing/docs review |
| EUI-R-014 | Diagnostics leak displayed/user data | Low | High | Redacted snapshots and privacy tests | Support export additions |
| EUI-R-015 | Peer bridge creates circular dependency | Medium | High | Separate bridge packages and mixed bridge rule | Integration assembly design |
| EUI-R-016 | Stable IDs/GUIDs break on update | Low | High | Collision validation, committed metas, migration aliases | Rename/move/release |
| EUI-R-017 | Existing project regression during migration | Medium | High | Preserve-until-parity and reversible vertical slices | First Rescuers2D adoption |
| EUI-R-018 | Notification/prompt queues grow without bound | Medium | Medium | Configured capacities and overflow policies | Stress tests |
| EUI-R-019 | Domain-reload-disabled statics retain stale state | Medium | High | Subsystem reset and lifecycle tests | Enter Play Mode configuration |
| EUI-R-020 | Mixed pointer/controller semantics frustrate users | Medium | Medium | Configurable focus policy and meaningful-device data from Will bridge | User testing |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EUI-D-001 | EchoUI owns presentation infrastructure only | Approved | Prevents UI from becoming game authority | Domain operations require presenters/adapters | No |
| EUI-D-002 | Use one duplicate-safe application-session root | Approved | Consistent layers, focus, navigation, and diagnostics | Root lifecycle must be rigorously tested | No |
| EUI-D-003 | First backend is uGUI with TMP-compatible text | Approved | Matches suite baseline and practical runtime needs | UI Toolkit is deferred adapter/expansion | No |
| EUI-D-004 | Use seven named root layers | Superseded 2026-08-14 | Fixed count is unnecessarily restrictive for a designer-first toolkit | Replaced by EUI-D-042 project-defined ordered layer topology | No |
| EUI-D-005 | Screen history supports Push, Replace, Reset, and Back | Approved | Covers common menu flow without game-specific rules | Structural operations must be serialized | No |
| EUI-D-006 | Modal entries use exact-once results and owned handles | Approved | Safe await/callback lifecycle and out-of-order cleanup | Stale handles need generation/identity validation | No |
| EUI-D-007 | EventSystem policy is explicit and non-destructive | Approved | Avoids hidden duplicates and deleted project setup | Conflicts may block until user resolves them | No |
| EUI-D-008 | Focus resolution is deterministic and event-driven | Approved | Reduces selection jitter and frame polling | Views must declare/default/fallback focus metadata | No |
| EUI-D-009 | Project code owns final views, motifs, copy, and domain presenters | Approved | Preserves project identity and package update safety | Package ships templates, not mandatory production art | No |
| EUI-D-010 | Structural UI operations are serialized with bounded admission | Approved | Prevents transition races | Requests can be rejected/queued/coalesced explicitly | No |
| EUI-D-011 | UI transitions use unscaled time and hard bounds | Approved | Works during pause and cannot hang forever | Transition drivers need cancellation/timeout contracts | No |
| EUI-D-012 | Runtime motif/configuration assets remain immutable | Approved | Prevents Play Mode contamination | Runtime uses resolved state/snapshots | No |
| EUI-D-013 | Accessibility policy is effective runtime input, persistence remains external | Approved | Keeps Accord as preference authority | Standalone defaults and bridge/provider seams required | No |
| EUI-D-014 | Notifications, prompts, histories, queues, and diagnostics are bounded | Approved | Prevents quiet resource growth | Overflow policies are configurable and observable | No |
| EUI-D-015 | Peer integrations ship separately by default | Approved | Preserves standalone installation/removal | More explicit bridge artifacts and labs | No |
| EUI-D-016 | UI Toolkit, native screen-reader providers, XR, and advanced virtualization are deferred | Approved | Protects MVP and avoids unproven multi-backend scope | Later work requires specification/ADR | No |
| EUI-D-017 | Historical Foundation implementation lock after initial specification | Superseded 2026-08-13 | The suite documentation gate and JIT package learning program have since passed | Replaced by package-local PKG-LEARN-008 + EUI-M1-01 activation | No |
| EUI-D-018 | UI context is layered rather than one global mutually-exclusive UI enum | Approved | Frontend/gameplay context, pause/cinematic/loading conditions, navigation, HUD, and overlays may coexist | Visibility policy requires explicit inputs | No |
| EUI-D-019 | One screen is active at a time only inside an authored navigation scope | Approved | Supports console-style menus without forbidding MMO-style windows | Scope identity becomes public configuration | No |
| EUI-D-020 | Back uses history by default with explicit Navigate To / Return Root / scope actions | Approved | Predictable default plus designer control | Stale history must prune safely | No |
| EUI-D-021 | Surface visibility reacts to cascading externally supplied context policy | Approved | HUD/windows can independently hide for Pause/Cinematic/etc. | Looking Glass must not become pause/game-state authority | No |
| EUI-D-022 | Every surface has a stable project-authored ID independent of hierarchy path | Approved | Enables cross-system hooks and refactor-safe addressing | Duplicate/invalid IDs require validation | No |
| EUI-D-023 | Default selection is optional and input-modality aware | Approved | Avoids mouse menus showing arbitrary selection while preserving controller/keyboard focus | Modality provider remains external/optional | No |
| EUI-D-024 | Pause/cinematic/loading/input modality truth is external to Looking Glass | Approved | Preserves package independence and future Controller/Will authority | Laboratory supplies a simple local context driver | No |
| EUI-D-025 | Independent window surfaces may coexist with screens and each other | Approved | Supports EverQuest/WoW/desktop-style interfaces | Window layout/persistence remains later/project-owned | No |
| EUI-D-026 | Surface registry is queryable/discoverable by ID/role/scope/category/state | Approved | Enables generic selectors, tooling, and Menu-for-Menus interfaces | Registry may expose presentation metadata, not domain truth | No |
| EUI-D-027 | Sample/editor hierarchy convention is `Type_DescriptiveName` | Approved | Scene hierarchy remains simple, searchable, and immediately readable | Runtime IDs remain separate | No |
| EUI-D-028 | Standardized UI primitives behave as Lego pieces; visual prefab variants share behavior | Approved | Reuse without forcing one art style | Primitive behavior must remain focused | No |
| EUI-D-029 | Motif is the front-facing reusable appearance recipe with capture/apply and local overrides | Approved | Reuse colors/states/sprites/typography across separately authored menus | Motif owns appearance only | No |
| EUI-D-030 | Looking Glass Builder automates create/batch-create/name/parent/style/validate operations | Approved | Removes boilerplate while preserving hand-authored composition | Builder may not overwrite project content silently | No |
| EUI-D-031 | Final screen composition and UI object lifetime are project/designer owned | Approved | Package supplies bricks and runtime plumbing rather than a mandatory game shell | Setup/Lab must remain explicit | No |

| EUI-D-032 | External UI contexts use project-defined stable IDs with active/inactive truth | Approved | Projects retain their own domain vocabulary while Looking Glass receives a neutral UI-facing condition | Adapters/project composition map domain truth without transferring ownership | No |
| EUI-D-033 | External context rules are ordered per surface and designer-controlled | Approved | Different surfaces/scenes may need different precedence | Looking Glass has no hardcoded global winner such as “Hide always wins” | No |
| EUI-D-034 | Context response resolves independently per controlled dimension; unspecified dimensions receive no intervention | Approved | A high-priority rule may control visibility while a lower rule controls interaction | Response values need an explicit no-change state | No |
| EUI-D-035 | Visibility, interaction, and selection/focus are independent UI dimensions | Approved | Visible does not necessarily mean interactive or focused | Context application must not conflate these states | No |
| EUI-D-036 | A surface may opt out of automatic external-context handling | Approved | Designers need explicit escape hatches for always-on or manually controlled UI | Opt-out affects context response only, not registration/direct operations | No |
| EUI-D-037 | Effective surface behavior resolves from authored defaults plus local/instance and transient project runtime overrides | Approved | Reuse should reduce authoring work without preventing case-specific behavior or future flexible HUD customization | Runtime overrides must not mutate authored assets and are not persistence | No |
| EUI-D-038 | Input modality remains external; each surface owns its selection-on-open policy | Approved | Controller defaults and pointer-unselected behavior can coexist without EchoUI owning input | No hard dependency on The Will, Vessel, Controller, or Input System modality authority | No |
| EUI-D-039 | Closing a temporary surface defaults to no selected control; prior selection is not implicitly restored | Superseded 2026-08-15 by EUI-D-061/EUI-D-062 | M1-02 intentionally deferred focus-history restoration; M3-01 now supplies the explicit capability while retaining fresh/no-focus as designer-selectable behavior | Historical checkpoint behavior remains valid for M1-02 | No |
| EUI-D-040 | EUI-M1-02 context IDs carry no arbitrary domain payload | Approved | Prevents UI context from becoming a generic cross-package data bus | Rich domain values remain with their owning systems/providers | No |
| EUI-D-041 | Future presets/templates are editable copy-in starting points rather than mandatory live centralized policies | Approved | Fast setup and full designer control can share one architecture | Preset/template authoring tooling remains outside EUI-M1-02 | No |
| EUI-D-042 | Layer topology is project-defined and ordered; any package starter layer set is editable convenience rather than a fixed runtime count | Approved | Designers need to add/remove/reorder layer definitions without fighting framework law | Layer IDs/config become stable authored data; runtime validates resolved topology and does not assume seven | No |
| EUI-D-043 | RootOwned, SceneOwned, and ExternalOwned are first-class screen view ownership modes | Approved | Projects need prefab-spawned, scene-authored, and externally composed views without lifecycle confusion | Looking Glass coordinates screen lifecycle but destroys/releases only what its ownership mode permits | No |
| EUI-D-044 | Suspended-screen visibility is designer-controlled, while suspended Screens are non-interactive within their navigation scope | Approved | Designers may want hidden, visible-behind, or authored-visibility presentations | Screen scope still guarantees one interactive top entry | No |
| EUI-D-045 | M2-01 screen structural mutations use bounded strict FIFO execution in request submission order | Approved | Determinism is safer than racing Push/Replace/Back requests | Queue overflow/rejection is explicit; no silent M2-01 coalescing/reordering | No |
| EUI-D-046 | Accepted screen-operation failure cannot partially mutate authoritative history or ownership state | Approved | Lifecycle authority must remain trustworthy under invalid IDs/factory loss/queue rejection | Operations require preflight/settlement and structured terminal results | No |
| EUI-D-047 | EUI-M2-01 is screen-lifecycle-only; modal exact-once result lifecycle is deferred to EUI-M2-02 | Approved | Keeps the first Runtime Core slice small and independently provable | M2 remains open after M2-01; modal implementation is not authorized by M2-01 | No |
| EUI-D-048 | Blocking modals may stack; only the top eligible modal receives normal Looking Glass interaction | Approved | Nested confirmations and owner-driven dialogs are legitimate while lower entries must remain safe | Lower entries stay addressable by generational handles and may be cleaned up out of order | No |
| EUI-D-049 | Semantic modal completion uses project-defined stable result IDs | Approved | Keeps package vocabulary neutral and refactor-safe | Display labels and gameplay meanings remain project-owned; arbitrary typed payload transport is not required by M2-02 | No |
| EUI-D-050 | First valid terminal modal completion wins exactly once | Approved | Prevents confirm/cancel/Back/owner races from double-calling consumers | Later attempts return stale/already-completed structured results | No |
| EUI-D-051 | Unexpected post-admission modal owner/view loss and shutdown produce structural `Aborted`, distinct from semantic Cancel | Approved | Infrastructure teardown must not pretend the user chose a gameplay/UI answer | Semantic cancel remains a project-defined result ID; pre-active failures return operation failure | No |
| EUI-D-052 | Modal ownership reuses RootOwned, SceneOwned, and ExternalOwned lifetime rules | Approved | One ownership model is easier to reason about and preserves project composition | Looking Glass destroys/releases only RootOwned modal instances | No |
| EUI-D-053 | Blocking modal state gates Looking Glass UI interaction but never owns gameplay input/pause/time/cursor truth | Approved | Projects may intentionally keep WASD/gameplay behavior active while menu clicks/navigation are blocked | External systems/bridges may observe modal state and decide their own gameplay-input policy | No |
| EUI-D-054 | Screen mutations during a blocking modal use explicit `Reject` default or bounded `DeferUntilModalStackClears` policy | Approved | Simple projects need a safe default while advanced flows sometimes need deterministic deferred navigation | No silent background Screen mutation; deferred work preserves FIFO and normal bounds | No |
| EUI-D-055 | Modal Back behavior is designer-authored: disabled or complete with one configured stable result ID | Approved | Escape/Back semantics vary by confirmation severity and project UX | Back routes modal-first; disabled policy leaves modal active | No |
| EUI-D-056 | Modal visuals/backdrops remain project-authored; M2-02 guarantees lifecycle/blocking rather than a mandatory dim/blur style | Approved | Designer control and package neutrality | Blur/transitions/general backdrop effects remain later work | No |
| EUI-D-057 | Blocking Modal semantics apply only to the blocking Modal lifecycle; independent Window surfaces remain non-blocking/coexistent by default | Approved | Supports EverQuest/WoW/desktop-style multi-window UI without weakening confirmation/dialog blocking | Window controls may still choose their own raycast/interactivity policy | No |
| EUI-D-058 | Screen-operation FIFO and Back/Escape dismissal order are separate concepts; future independent-window dismissal uses most-recent-eligible LIFO history | Approved | Accepted operations should execute deterministically while user-facing Back naturally unwinds the latest eligible surface | Does not change M2-01 FIFO execution contract | No |
| EUI-D-059 | Future independent Windows may be excluded from automatic Back/Escape by authored defaults or runtime pin/lock state | Approved | Supports project defaults plus user-controlled pinned windows | Runtime pin state is transient; durable persistence remains separately gated | No |
| EUI-D-060 | EventSystem coordination is explicit and non-destructive through AdoptAssigned, deterministic AdoptExisting, CreateIfMissing, and RequireExternal modes | Approved | Projects need predictable ownership without Looking Glass deleting or arbitrarily choosing external EventSystems | Multiple eligible active EventSystems produce degraded/blocking focus status | No |
| EUI-D-061 | Focus memory is per live runtime entry with optional transient root-session memory keyed by stable surface ID | Approved | Natural resume/modal restoration and optional reopen memory can coexist without persistence | Designer may choose fresh reopen; session memory never mutates authored assets | No |
| EUI-D-062 | Focus restoration uses explicit -> remembered -> authored default -> entry resolver -> global fallback -> legal no-focus | Approved | Restoration should be useful without preserving an obsolete M1-02 limitation or forcing a target | Invalid/ineligible targets fall through safely | No |
| EUI-D-063 | Pointer/navigation focus behavior remains designer-controlled; trivial pointer movement does not force selection clearing | Approved | Mouse-like unselected UX and controller deterministic focus can coexist without jitter | Input modality remains external/project supplied | No |
| EUI-D-064 | A blocking Modal structurally contains Looking Glass focus to the top eligible Modal while preserving lower-entry focus memory | Approved | Modal UI must prevent lower-UI focus leakage while permitting deterministic restoration | Gameplay input remains external/project-owned | No |
| EUI-D-065 | Independent Windows may keep distinct focus memory without activating a full focused-window manager | Approved | MMO/desktop-style coexistence needs focus memory without prematurely coupling z-order, Back history, pins, or layout | Window LIFO/pin/drag/layout work remains later | No |
| EUI-D-066 | Focus maintenance is event-driven by default with an explicit project-callable revalidation seam | Approved | Avoids universal per-frame scanning while supporting dynamic/touch-heavy UI that knows when its hierarchy changes | Future opt-in tick driver requires separate evidence/authorization | No |
| EUI-D-067 | Focus requests carry operation/generation identity and stale requests cannot override newer UI state | Approved | Async/lifecycle ordering must not teleport selection backward in time | Diagnostics should expose rejected stale work | No |
| EUI-D-068 | Optional package input adapters may default to the suite Unity-default Input Actions compatibility profile, but EchoUI core does not require the generated wrapper or own input maps | Approved | Standard baseline action names save project setup without creating a hidden peer/input dependency | Projects retain explicit override/adapters and may add actions | No |
| EUI-D-069 | Transition execution is part of the admitted structural UI operation and must settle before that operation reports terminal success | Approved | Prevents lifecycle truth from racing ahead of presentation and preserves serialized mutation semantics | Transition-aware operations remain bounded and deterministic | No |
| EUI-D-070 | Transition drivers own presentation only and cannot own navigation history, Modal meaning, game state, persistence, scene, audio, or input-map truth | Approved | Replaceable presentation must not become a second authority layer | Projects can use rich custom drivers without transferring domain ownership | No |
| EUI-D-071 | Every transition execution uses a fresh awaitable/result plus operation/generation identity; stale completion is harmless | Approved | Cached/reused async work and late callbacks can otherwise rewind UI state | Drivers and coordinator expose structured stale/cancel/timeout outcomes | No |
| EUI-D-072 | Enter failure restores the prior stable UI and cleans the incoming entry; exit failure forces deterministic closed/released settlement | Approved | A failed open must not corrupt history and a failed close must not hold UI hostage | Recovery paths are asymmetric but deterministic and testable | No |
| EUI-D-073 | Transition cancellation is best-effort, but stale-generation protection and a hard safety bound are mandatory even for noncancellable drivers | Approved | Third-party animation systems vary in cancellation support | Looking Glass remains usable without requiring perfect driver cancellation | No |
| EUI-D-074 | Effective transition policy resolves project/root default -> per-definition profile -> transient operation override without mutating authored assets | Approved | Simple defaults and advanced case-specific behavior must coexist | Runtime overrides remain session-only and nonpersistent | No |
| EUI-D-075 | The transition seam is surface-general, while EUI-M3-02 wires only Screens, blocking Modals, and independent Windows | Approved | Reuses one architecture without prematurely activating M4 transient/HUD services | Later surfaces may consume the seam through separately authorized checkpoints | No |
| EUI-D-076 | Built-in transition drivers are Immediate and unscaled CanvasGroup Fade; custom professional drivers remain first-class without a mandatory tween dependency | Approved | Useful defaults should not cap advanced presentation | Profiles retain enter/exit timing, optional curve/easing, timeout, and substitution seams | No |
| EUI-D-077 | Reduced-motion substitution is supported by transition policy now, while Motif/accessibility service implementation remains a separate checkpoint | Approved | Avoids a later transition rewrite without collapsing two M3 slices together | Accessibility policy may later choose Immediate/reduced variants | No |
| EUI-D-078 | Primitive Warehouse, editable Panel/Menu Template Library, stable-ID Template Catalog, and Assembly Utilities are durable authoring capabilities distinct from the later full Builder/Composer | Approved | Snap-together menu construction must not disappear into an ambiguous Builder backlog | Templates are ordinary editable prefab compositions; catalogs are project-extensible; Builder consumes the same underlying library | No |

### 27.2 Release-blocking questions

None. Exact Unity UI/TMP dependency versions are verification tasks for M1, not unresolved ownership decisions.

### 27.3 Non-blocking later questions

- Whether backend-neutral core contracts justify a separate contracts assembly.
- Whether UI Toolkit becomes a first-party adapter or a future major package revision.
- Whether view pooling is valuable after profiling.
- Whether advanced list virtualization belongs in EchoUI or a specialized extension.
- Which native accessibility providers are feasible per platform.
- How split-screen/local-player UI roots should be modeled with EchoMultiplayer.
- Whether world-space UI needs a dedicated adapter package.
- Whether navigation graphs require a visual Editor tool beyond Unity’s existing navigation surface.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved package contract | All design sections, tests, risks, decisions | This approved document |
| M1 - Surface Foundation | Installable package plus smallest useful UI behavior | Manifest/asmdefs/docs, package-local root claim, stable surface registry, one exclusive navigation scope, independent window | Clean compile, duplicate-claim, navigation, Back, and coexistence tests |
| M2 - Runtime core | Smallest authoritative lifecycle | Root/layers, registries, screen history, modal results, operation serialization | EditMode/PlayMode tests |
| M3 - Focus and presentation | Usable uGUI framework | EventSystem policy, focus, view lifecycle, transitions, motif/accessibility seams | Automated tests and profiler capture |
| M4 - Complete MVP surfaces | HUD, notifications, prompts, tooltips, diagnostics | Bounded queues/leases, status snapshots | Laboratory scenarios |
| M5 - Tooling and Lab | Safe setup/repair/validation and standalone proof | Editor window, validators, simulators, sample | Repeatability and clean-project report |
| M6 - First integration | One explicit bridge/project vertical slice | Project adapter and parity work | Integration Lab/parity/rollback report |
| M7 - Release | Distribution-ready beta/stable candidate | Docs, license, notices, changelog, package artifact | External install and release gates |

### 28.2 Historical Foundation documentation gate — satisfied

The original v1.0.x specification correctly blocked implementation until the Foundation documentation program and cross-package consistency work completed. That suite gate has since been satisfied. Current implementation permission is package-local: PKG-LEARN-008 must be complete and an explicit Checkpoint Build Plan must be active under SFGSS-005. EUI-M1-01 completed and was finally reconciled at `57a4fa4`; EUI-M1-02 completed at closeout `c114ba2` with final full EditMode **1130 / 1130**, focused EchoUI **24 / 24**, and manual Laboratory **10 / 10**. The bounded PKG-LEARN-008 EUI-M2-01 revisit is complete, this specification is reconciled to v1.3.0, and EUI-M2-01 is the active package-local checkpoint as of August 14, 2026.

### 28.3 First recommended implementation checkpoint after the gate

**EUI-M1-01 - Installable Surface Foundation, Scoped Navigation, and Independent Window Proof**

Outcome:

- Create the embedded package manifest, runtime/test assembly boundaries, documentation shell, and minimal UPM sample instructions.
- Add a package-local `EchoUIRoot` authority that rejects duplicates before surface-registry/navigation side effects and does not claim project `DontDestroyOnLoad` composition.
- Register stable project-authored surface IDs and distinguish `Screen`, `Window`, `HUD`, and `Overlay` roles.
- Prove one exclusive `frontend` navigation scope: `main-menu -> settings -> Back -> main-menu`.
- Prove one independent `default-window` can open/close without changing the active `frontend` screen.
- Add a minimal uGUI navigation-button adapter so the Laboratory can wire real Buttons without another Echo package.
- Keep context conditions, visibility-rule evaluation, input-modality focus behavior, Motifs, primitive prefab library, Builder, modal/HUD/transient services, and persistence outside this slice.

Stop point: package compiles in Unity 6000.3.8f1 with uGUI 2.0.0, focused tests pass, direct-scene manual proof shows scoped navigation + Back + independent-window coexistence, duplicate authority remains side-effect-free, and documentation matches the committed state.

### 28.4 Completed implementation checkpoint — EUI-M1-02

**EUI-M1-02 - External UI Context, Ordered Surface Response Rules, and Input-Aware Selection Contract** is **COMPLETE**. Activation `f0b97ff`; implementation `1c0a46a`; closeout `c114ba2`; final full EditMode **1130 / 1130**; focused EchoUI **24 / 24**; manual Laboratory **10 / 10**.

Authorized outcome:

- consume project-defined stable active/inactive context IDs without owning the underlying game truth;
- permit multiple simultaneous active contexts;
- evaluate designer-ordered per-surface response rules independently for visibility, interaction, and selection/focus intent;
- leave a response dimension unchanged when no applicable rule supplies a value;
- allow a surface to opt out of automatic external-context handling;
- support reusable authored defaults with local/instance and transient project runtime overrides without mutating authored definitions or claiming persistence;
- consume externally supplied input modality and apply per-surface selection-on-open policy, including controller default-selection and pointer/unselected configurations;
- default temporary-surface close to no selected control rather than implicit selection-history restoration;
- prove the behavior in the standalone Looking Glass Laboratory with sample-owned context/modality simulation only.

Explicitly excluded from EUI-M1-02: arbitrary context payloads; automatic input-device detection; focused-window arbitration; selection-history restoration; presets/template authoring tooling; Motifs; Builder; broad primitive-library expansion; draggable/resizable/persisted MMO window layouts; modal/notification/tooltip/prompt/full-HUD frameworks; peer-package bridges; Chronicle/Accord persistence; project-wide lifetime composition; and production showcase art.

Stop point: satisfied at `c114ba2`; EUI-M1-02 is closed and no excluded capability was activated by that closeout.

### 28.5 Completed implementation checkpoint — EUI-M2-01

**EUI-M2-01 — Authoritative Screen Lifecycle, Project-Defined Layers, and Serialized Screen Operations** is **COMPLETE** at closeout `d5b9a73`.

Retained proof:
- activation `0c11262`;
- implementation `8dc9c71`;
- focused EchoUI **47 / 47**;
- EUI-M2-01 focused **23 / 23**;
- manual Laboratory **10 / 10**;
- final full EditMode **1153 / 1153**, 0 failed.

### 28.6 Completed implementation checkpoint — EUI-M2-02

**EUI-M2-02 — Blocking Modal Lifecycle, Exact-Once Results, and UI-Scoped Interaction Blocking** is **COMPLETE**.

Durable evidence:
- activation `e2145ab`;
- Modal/Window clarification `b6fc160`;
- implementation `5ab34b3`;
- closeout `7f5ad40`;
- focused EUI-M2-02 **28 / 28 passed**;
- EchoUI EditMode **75 / 75 passed**;
- final Foundry EditMode **1181 / 1181 passed**;
- manual Laboratory **12 / 12 PASS**;
- retained M2-01 Screens and M1 proof tabs PASS.

M2-02 established stacked blocking Modal lifecycle, project-defined stable result IDs, exact-once settlement, structural aborts, ownership modes, Back policy, UI-only blocking, and Screen Reject/Defer behavior. Blocking Modal semantics remain distinct from independent Window coexistence.

### 28.7 Completed implementation checkpoint — EUI-M3-01

**EUI-M3-01 — EventSystem Coordination, Focus Memory/Restoration, and Modal Focus Containment** is **COMPLETE**.

Durable evidence:
- activation `292cb66`;
- implementation `f08c926`;
- closeout `0c58240`;
- focused EUI-M3-01 **24 / 24 passed**;
- EchoUI EditMode **99 / 99 passed**;
- final Foundry EditMode **1205 / 1205 passed**;
- manual Laboratory **12 / 12 PASS**;
- bounded event-driven focus performance evidence PASS;
- retained M2-02/M2-01/M1 tabs PASS;
- package/imported Laboratory parity verified.

M3-01 established explicit/non-destructive EventSystem coordination, transient live/session focus memory, deterministic restoration/fallback/no-focus, blocking-Modal containment, independent Window focus memory, event-driven revalidation, and stale-generation protection without transferring input authority.

### 28.8 Completed implementation checkpoint — EUI-M3-02

**EUI-M3-02 — View Lifecycle, Replaceable Transition Drivers, and Deterministic Transition Recovery** is **COMPLETE / CLOSED**. Activation: `ee9d3ff`; implementation: `c919238`; closeout: `0affb7d`; final full EditMode **1246 / 1246**; EchoUI Editor **140 / 140**; Laboratory **14 / 14**.

Authorized outcome:
- transition execution becomes part of admitted Screen/Modal/Window structural lifecycle settlement;
- replaceable presentation-only transition drivers with no domain/navigation/input/persistence authority;
- fresh awaitable/result execution with operation/generation identity and stale-completion rejection;
- unscaled-time execution, best-effort cancellation, and mandatory hard safety bounds;
- deterministic enter rollback to the prior stable UI and deterministic exit force-close/release recovery;
- structural Modal `Aborted` settlement for admitted open failures rather than fabricated semantic Cancel;
- effective transition policy resolution through project/root default -> per-definition profile -> transient operation override;
- independent enter/exit driver/timing data with optional curve/easing, timeout, and reduced-motion substitution seams;
- package reference drivers limited to Immediate and CanvasGroup Fade while professional custom drivers remain first-class;
- surface-general transition contracts wired only to existing Screen, blocking Modal, and independent Window lifecycle in this slice;
- retained M3-01 focus/EventSystem behavior and existing M2 operation serialization/exact-once contracts.

Authoring promise recorded alongside this activation, but not implemented by M3-02:
- a package-owned Primitive Warehouse including scalable 9-slice-capable primitive families;
- ordinary editable Panel/Menu Templates assembled from those primitives;
- a project-extensible stable-ID Template Catalog;
- lightweight Assembly Utilities that remain useful without the later full Builder/Composer;
- a later Builder/Composer that consumes the same library rather than replacing it.

Explicitly excluded from EUI-M3-02 implementation: Motif/accessibility service implementation beyond reduced-motion transition substitution seams; generalized dim/blur; HUD regions/notifications/tooltips/prompts; full Window LIFO/pin/drag-resize/layout management; persistence; peer bridges; implementation of the Primitive Warehouse/Template Catalog/Assembly Utilities/Builder; automatic gameplay-input/UI-map switching; project-wide lifetime composition; polished showcase art.

Historical stop point satisfied at closeout `0affb7d`. The completed checkpoint preserved Motif/accessibility service and M4 HUD/transient work as separate activation boundaries.

---

### 28.9 Completed implementation checkpoint — EUI-M4-01

**EUI-M4-01 — Named HUD Regions, Widget Leases, and Deterministic Visibility Authority** is **COMPLETE / CLOSED**.

Durable evidence:
- activation `ce30ac6`;
- retained-floor timing stabilization `dbdf6bd`;
- Runtime/tests `df9e2be`;
- bounded corrections `81f9625`, `3992bbc`, and `e47d43b`;
- implementation/Laboratory seal `29573ef`;
- requested focused/full automated gate user-confirmed green, with exact post-M4 NUnit totals not captured and retained `1246 / 1246` preserved as the pre-M4 floor;
- manual HUD Laboratory **5 / 5 PASS**;
- retained M3-02/M3-01/M2-02/M2-01/M1 smoke user-confirmed green;
- package/imported Laboratory parity verified.

The completed slice supplies stable named HUD regions, bounded widget registration, generation-safe widget/visibility leases, deterministic effective visibility, owner-loss/shutdown cleanup, and structured status/events without changing Screen/Modal/Window authority or claiming gameplay truth. At the EUI-M4-01 closeout boundary, notifications and all later capabilities remained inactive; the EUI-M4-02 activation below supersedes notification status only.

### 28.10 Active implementation checkpoint — EUI-M4-02

**EUI-M4-02 — Bounded Notification Channels, Priority, Coalescing, Overflow, and Unscaled Lifetime** is **ACTIVE / AUTHORIZED** from clean EUI-M4-01 closeout baseline `5e7ad92`.

The slice owns project-defined stable channels, independent visible/pending bounds, higher-priority/FIFO-tie pending promotion, non-preemptive visible entries, opt-in fresh-generation coalescing with default lifetime restart, explicit pending overflow policies, unscaled/manual lifetime, generation-safe handles, owner-loss/reset/shutdown settlement, status/events, focused tests, and Laboratory proof.

Default overflow is `RejectNewest`; authored alternatives are `DropOldestPending` and strict-outrank `ReplaceLowestPriorityPending`. Notifications remain transient presentation and cannot own durable history, localization/audio/analytics, gameplay/domain truth, input, pause/time scale, cursor, persistence, or project lifetime composition.

The activation baseline was recorded before Runtime edits at full Foundry EditMode **1258 / 1258** and EchoUI Editor **152 / 152**. Runtime/root/presenter implementation is accepted through `d93d0bd`. Final automated evidence is full Foundry EditMode **1383 / 1383**, EchoUI Editor **277 / 277**, aggregate notification fixtures **125 / 125**, and presenter fixture **17 / 17**, with zero failed/skipped/inconclusive. The mirrored Laboratory now implements three authored channels, a sample-owned plain presenter, six bounded checks, and retained tabs; Unity manual acceptance and retained smoke remain pending. Prompts, tooltips, Motifs/accessibility implementation, safe area, full Window management, persistence, bridges, authoring libraries/Builder, integration, and release remain inactive.

---

## 29. New-Conversation Handoff

```text
Continue The Sperk's Forge from the latest live main and verify the branch head.

Authority:
- SFGSS-000 v0.27.0
- The Looking Glass package specification v1.8.0
- SFGSS-005 v1.7.0 and SFGSS-ADR-007 v1.1.0
- Active plan: EUI-M4-02

Completed:
- EUI-M1 through EUI-M4-01
- EUI-M4-01 documentation closeout 5e7ad92
- EUI-M4-01 implementation/Laboratory seal 29573ef
- automated gate user-confirmed green; exact post-M4 totals not captured
- manual HUD Laboratory 5 / 5 PASS
- retained prior-tab smoke green; package/imported parity verified

Active phase:
- EUI-M4-02 Runtime/root/presenter implementation accepted through d93d0bd
- final automated gate: 1383 / 1383 full Foundry EditMode; 277 / 277 EchoUI
- mirrored M4-02 Laboratory implemented with three channels and checks 1-6
- Unity manual checks 1-6 and retained prior-tab smoke are pending

Resume only the EUI-M4-02 Unity Laboratory acceptance and retained smoke. After
user-confirmed green, reconcile and publish final closeout documentation. Do not begin prompts, tooltips,
Motifs/accessibility, safe area, full Window management, persistence, bridges,
authoring libraries/Builder, integration, or release work.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Runtime package version | `0.1.0` |
| Package authority | SFGSS-PKG-ECHOUI-001 v1.8.0 Approved |
| Suite authority | SFGSS-000 v0.27.0 Approved |
| Completed implementation checkpoints | EUI-M1-01 through EUI-M4-01; EUI-M4-02 Runtime/root/presenter automated implementation complete, checkpoint not yet closed |
| EUI-M3-02 retained evidence | Activation `ee9d3ff`; implementation `c919238`; closeout `0affb7d`; full EditMode **1246 / 1246**; EchoUI Editor **140 / 140**; Laboratory **14 / 14** |
| EUI-M4-01 completion evidence | Activation `ce30ac6`; Runtime/tests `df9e2be`; corrections through `e47d43b`; implementation/Laboratory seal `29573ef`; automated gate green; manual HUD Laboratory **5 / 5 PASS**; retained smoke green; parity verified |
| Active implementation checkpoint | EUI-M4-02 — Bounded Notification Channels, Priority, Coalescing, Overflow, and Unscaled Lifetime |
| Active phase | Mirrored M4-02 Laboratory implemented; Unity manual acceptance and retained smoke pending |
| EUI-M4-02 automated evidence | Activation baseline **1258 / 1258** full and **152 / 152** EchoUI; final **1383 / 1383** full, **277 / 277** EchoUI, **125 / 125** notification, **17 / 17** presenter; zero failed/skipped/inconclusive |
| Exact resume phase | Run M4-02 Laboratory checks 1-6 and retained M4-01/M3-02/M3-01/M2-02/M2-01/M1 smoke; then reconcile closeout |
| Excluded next work | Prompts, tooltips, Motifs/accessibility, safe area, Window management, persistence, bridges, authoring libraries/Builder, integration, and release remain inactive |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Package identity and plain responsibility are clear.
- [x] Ownership and exclusions align with SFGSS-000.
- [x] Independence proof is credible.
- [x] Configuration and runtime state are separated.
- [x] Project-owned views/motifs/content remain outside immutable package source.
- [x] EventSystem ownership is explicit and non-destructive.
- [x] Focus memory/restoration is transient, policy-aware, and permits legal no-focus.
- [x] Blocking Modal focus containment does not transfer gameplay-input authority.
- [x] Independent Window focus memory does not activate the full Window manager.
- [x] Focus maintenance is event-driven by default with explicit revalidation available.
- [x] Optional Unity-default action-name conveniences do not create a generated-wrapper or peer-package dependency.
- [x] EUI-M1-01, EUI-M1-02, EUI-M2-01, EUI-M2-02, and EUI-M3-01 are complete.
- [x] Transition execution preserves structural-operation authority, uses fresh generation-bound work, and has deterministic failure recovery.
- [x] Immediate/CanvasGroup Fade reference drivers do not cap professional custom transition drivers or introduce a mandatory tween dependency.
- [x] Reduced-motion substitution is architecturally available without prematurely implementing the Motif/accessibility service.
- [x] Primitive Warehouse, editable Panel/Menu Templates, stable-ID Template Catalog, and Assembly Utilities are explicitly preserved as durable authoring capabilities distinct from the full Builder/Composer.
- [x] PKG-LEARN-008 bounded EUI-M4-02 notification revisit is complete.
- [x] EUI-M3-02 is complete and closed at `0affb7d`.
- [x] EUI-M4-01 is COMPLETE / CLOSED with automated gate green, manual HUD Laboratory 5 / 5 PASS, retained smoke green, and package/imported parity verified.
- [x] EUI-M4-02 is package-locally ACTIVE / AUTHORIZED under SFGSS-005 and its exact Checkpoint Build Plan; Runtime/root/presenter implementation is automated-green through `d93d0bd`, with Laboratory manual acceptance pending.

### 30.2 Approval record

**Decision:** Approved / EUI-M4-02 reconciled and active
**Approved by:** Jesse “Echo” Adams
**Original approval date:** August 3, 2026
**JIT EUI-M1-01 rebaseline date:** August 13, 2026
**EUI-M1-02 JIT reconciliation and explicit authorization date:** August 13, 2026
**EUI-M2-01 JIT reconciliation and explicit authorization date:** August 14, 2026
**EUI-M2-02 JIT reconciliation and explicit authorization date:** August 14, 2026
**EUI-M3-01 JIT reconciliation and explicit authorization date:** August 15, 2026
**EUI-M3-02 JIT reconciliation and explicit authorization date:** August 15, 2026
**EUI-M4-01 JIT reconciliation and explicit authorization date:** August 16, 2026
**EUI-M4-02 JIT reconciliation and explicit authorization date:** August 17, 2026
**Conditions:** Package architecture remains authoritative. EUI-M4-01 is complete at implementation/Laboratory seal `29573ef`. EUI-M4-02 is authorized only within its active Checkpoint Build Plan from baseline `5e7ad92`; its activation baseline and final automated evidence are recorded above, Runtime/root/presenter implementation is accepted through `d93d0bd`, and manual Laboratory acceptance remains required before closeout. The Assembly Library promise remains durable authority, but Primitive Warehouse/template/catalog/utility/Builder implementation remains outside EUI-M4-02. Prompts, tooltips, Motifs/accessibility, safe area, Window management, persistence, bridges, integration, and release remain inactive. Any discovery that changes package ownership, peer dependencies, serialized compatibility, public contracts, or suite authority stops the Green Path and returns to the owning authority.

---

## Historical Specification Completion Record (August 3, 2026)

> Historical approval provenance retained for archaeology. Status-only statements in this record are superseded by the later v1.1.0 and v1.2.0 JIT reconciliation/activation records above.


A new collaborator can answer from this document:

1. EchoUI owns presentation infrastructure, not game truth.
2. It refuses settings, saves, input authority, pause, scene travel, audio, and gameplay ownership.
3. Its MVP is one protected uGUI root with layered screens, modals, HUD, notifications, prompts, focus, motifs, diagnostics, tooling, and an isolated Laboratory.
4. It installs and runs alone.
5. Definitions/configuration stay immutable; registries, histories, operations, handles, focus, and queues are runtime state.
6. Its public lifecycle, requests, results, events, and adapter seams are specified.
7. Setup and runtime failures return structured results and stable diagnostics.
8. Its Laboratory proves success, invalid, empty, failure, duplicate, stress, and cleanup cases.
9. Peer packages connect through separate bridges or project adapters.
10. Release requires clean installation, 42 Laboratory scenarios, the applicable 84-case registry, performance/privacy/accessibility evidence, documentation parity, and external package validation.

The Looking Glass specification is therefore complete and **Approved v1.0.0**. The next documentation checkpoint is **FW-DOC-09 - The Chronicle (`EchoSave`)**.


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

#sfgss/package #sfgss/wave/foundation #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]


