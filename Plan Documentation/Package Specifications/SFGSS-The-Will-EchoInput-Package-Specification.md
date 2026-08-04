# The Will – Input Infrastructure Package Specification

**Working document ID:** SFGSS-PKG-ECHOINPUT-001  
**Specification version:** 1.1.0
**Status:** Approved  
**Technical package name:** EchoInput  
**Public title:** The Will – Input Infrastructure
**Package ID:** `com.echodevgames.echo-input`  
**Runtime namespace:** `EchoDevGames.EchoInput`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoInput`
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Required Unity package:** Input System `com.unity.inputsystem` 1.17.0 or later within the supported 1.x line  
**Parent authority:** SFGSS-000 and SFGSS-001  
**Last updated:** August 4, 2026

> “Let intention cross the threshold cleanly, without mistaking the hand for the hero.”

> **Approval rule:** This specification is approved as the authoritative package design. Runtime implementation remains intentionally deferred until all ten Foundation Wave specifications and the cross-package consistency review are approved.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-03 | Proposed | Initial complete specification derived from SFGSS-000 v0.6.0, SFGSS-001 v1.1.0, and the six previously approved Foundation specifications | Pending |
| 1.0.0 | 2026-08-03 | Approved | Approved the duplicate-safe input root, owned runtime action clone, primary and override contexts, reason-based locks, meaningful-device detection, transactional rebinding, conflict analysis, versioned override documents, glyph resolution, controls-display data, diagnostics, tooling, and isolated Input Laboratory | Jesse “Echo” Adams |
| 1.1.0 | 2026-08-04 | Approved | Clarified Unity asset GUID, Input System GUID, and project-authored domain identity roles; Made unknown extension-data preservation an explicit serializer/opaque-record requirement. Also normalized registry metadata and evidence interpretation. | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Will – Input Infrastructure
**Technical identifier:** EchoInput  
**Flavor line:** Translate the player’s intent without deciding the game’s response.  
**Plain-language subtitle:** Input contexts, device state, rebinding, binding overrides, glyphs, lock reasons, and controller-independent input infrastructure.

**One-sentence ownership contract:**

> EchoInput owns the runtime coordination of project-authored Input System actions, input contexts, reason-based locks, active-device and control-scheme state, rebinding transactions, binding-override data, and glyph/prompt resolution; it does not own movement physics, combat or interaction meaning, UI screen and EventSystem authority, high-level game-state truth, save-slot progress, audio feedback, networking authority, or one mandatory gameplay action map.

### 1.1 Elevator summary

The Will provides one reliable input-infrastructure authority for projects using Unity’s Input System. A project supplies its own `.inputactions` asset and defines the actions that make sense for that game. EchoInput creates and owns a runtime action collection, resolves which maps and actions are currently available, tracks meaningful device activity, exposes control-scheme and prompt state, coordinates interactive rebinding, validates conflicts, and produces a versioned binding-override document that can be persisted by The Accord or project code.

The package separates **player intention infrastructure** from **gameplay interpretation**. EchoInput can report that the project’s `Jump`, `Interact`, `Pause`, or `Navigate` action performed, but it does not decide what jumping, interacting, pausing, or navigating means. A character controller, menu presenter, project adapter, or gameplay system consumes the action and owns the resulting rule.

The package works without First Light, The Observatory, The Accord, The Passage, The Pulse, Resonance, The Looking Glass, The Chronicle, or The Workshop. Optional bridges may initialize it, persist its preferences, map game-state requests into contexts, present rebind screens, or publish diagnostics, but those peers never become hidden runtime requirements.

### 1.2 Why this belongs in The Sperk’s Forge

Input infrastructure has repeatedly appeared in EchoDevGames projects as generated action wrappers, `PlayerInput` components, centralized input readers, action-map switching, controller/keyboard support, shared character routing, and menu/gameplay lockouts. These approaches prove the value of one translation layer, but they also expose recurring problems:

- gameplay and menu maps are enabled or disabled by several unrelated objects;
- a single Boolean lock is cleared by the wrong caller;
- gamepad stick drift changes prompts while the player is using keyboard and mouse;
- rebind screens modify actions without a conflict or rollback policy;
- binding overrides are stored as an opaque string with no project identity or migration rule;
- action names and binding indexes are treated as durable IDs even though authoring changes can reorder or rename them;
- device loss, control-scheme changes, and direct-scene testing are handled differently in every project;
- `PlayerInput`, generated wrappers, and direct `InputActionReference` use can create multiple runtime copies with inconsistent enablement;
- UI code becomes the only place where rebinding or input locks exist;
- controller code absorbs context switching and global preference persistence;
- sample input assets quietly become production requirements.

The Will preserves centralized translation and Unity Input System strengths while adding explicit ownership, stable references, transaction boundaries, bounded runtime state, direct-scene support, isolated tests, and removable bridges.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | “The Will” must be paired with “Input Infrastructure” in formal surfaces. |
| Setup guidance/tooltips | Yes | Flavor may discuss intention, focus, or command, but every setting must be technically explicit. |
| Samples | Optional | Verse visuals and copy must be removable and replaceable. |
| Runtime API/type names | No lore-only names | Types describe contexts, locks, actions, users, devices, rebinds, overrides, prompts, and results directly. |
| Project data | No required Hackulos content | The project owns action names, control schemes, bindings, icons, controller rules, and gameplay meaning. |

---

## 2. Problem Statement

### 2.1 Current problem

Unity’s Input System provides actions, action maps, bindings, interactions, processors, control schemes, users, device pairing, UI modules, and interactive rebinding. It does not decide how one project coordinates those primitives across scenes and systems. Without a package-level authority:

- several objects may enable and disable the same action map;
- pause, dialogue, menus, cutscenes, and rebinding may each invent a different lock model;
- a caller may unlock gameplay while another caller still requires it locked;
- direct-scene testing may lack the expected input asset or create a second input authority;
- active-device prompts may oscillate because of analog noise, pointer jitter, virtual devices, or synthetic events;
- controller disconnects can leave prompts and control schemes stale;
- rebind operations may accept forbidden controls, overwrite another action, or leave partial composite bindings;
- cancellation or timeout may fail to restore the previous effective action state;
- binding overrides may become invalid after action-asset edits with no useful report;
- input preferences may be stored in save slots even though they are global preferences;
- project UI can become inseparable from input logic;
- generated wrappers and `InputActionReference` assets can point at a different action collection from the one the runtime authority controls.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | One `PlayerInputReader` routes generated CharacterActions to Firefighter, Riot Officer, and Rescue Specialist controllers | Central action translation and shared character routing | Remove controller-specific meaning from the reusable package and replace Boolean locks with reason leases |
| Don’t Get Vince’d | Generated Input System actions feed movement, combo attacks, air kick, and UI/game flow | Generated-action clarity and event-driven consumers | Separate action availability from beat-’em-up rules and preserve existing controller code until parity |
| Echo Systems Lab | Central input reader and New Input System usage support modular missions and weapons | Focused translation layer | Add package isolation, stable IDs, direct-scene proof, and persistence boundaries |
| DeverQuest | Editor input, shortcuts, and UI interactions show the danger of controls being coupled to product-specific windows | Clear user actions | Keep Editor-product input outside the runtime package |
| Hackulos | WASD, click-to-move, attack toggles, spells, kick interrupts, and future control-scheme options need configurable actions | Project-owned action semantics and multiple schemes | Avoid embedding RPG or controller behavior in EchoInput |
| First Light v1.0.0 | Startup may initialize selected services | Ordered optional initialization | EchoInput remains independently initializable and direct-scene safe |
| Observatory v1.0.0 | Input state needs bounded provider snapshots | Structured diagnostics | Separate provider bridge, no mandatory dependency |
| Accord v1.0.0 | Rebinds, dead zones, sensitivity, and hold/toggle preferences are global | Transactional global preference authority | EchoInput owns application/validation of input data, not disk persistence |
| Passage v1.0.0 | Transitions need duplicate-request protection and input blocking | Serialized lifecycle and explicit reasons | Passage requests locks through a bridge rather than toggling maps directly |
| Pulse v1.0.0 | High-level state exposes neutral input-context intent | Primary plus leased override model | Pulse remains state authority; EchoInput executes context and lock changes |
| Resonance v1.0.0 | Input-driven UI/game events may request audio | Semantic requests and independent authority | No direct audio dependency or feedback ownership |

### 2.3 Consequences of doing nothing

- Every project rebuilds context switching, locks, device detection, and rebind persistence.
- Menu, gameplay, dialogue, and transition systems compete over action-map state.
- Binding changes become fragile after action-asset edits.
- Device prompts flicker or remain stale.
- Input bugs become difficult to reproduce because runtime copies and enabled maps are unclear.
- A reusable controller package would inherit project-specific input assumptions.
- Local multiplayer and later networking integrations would begin on an inconsistent single-player foundation.
- Accessibility preferences such as hold/toggle behavior remain scattered across gameplay scripts.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Maintain exactly one authoritative EchoInput root for the configured application session.
- Reject duplicate roots before subscribing to Input System callbacks, creating runtime actions/users, pairing devices, loading overrides, or enabling maps.
- Use a project-owned `InputActionAsset` as immutable authoring data and create an owned runtime copy by default.
- Resolve one primary context plus leased override contexts deterministically.
- Lock all input, selected maps, or selected actions by reason through idempotent leases.
- Track meaningful active-device activity without treating analog drift or pointer jitter as intentional switching.
- Expose per-user active device family and active control-scheme state.
- Perform interactive rebinding as a reversible transaction with cancellation, timeout, filtering, conflict analysis, and composite support.
- Identify actions, maps, and bindings by stable Unity GUIDs wrapped in package value types rather than by display names or array positions.
- Export and import versioned binding-override documents without owning permanent storage.
- Resolve neutral prompt text and project-authored glyphs through a fallback chain.
- Supply controls-display data suitable for a two-page or multi-page controls screen without owning that screen.
- Remain diagnosable without The Observatory.
- Provide a standalone Input Laboratory that proves contexts, locks, device switching, rebinding, conflicts, glyphs, direct-scene entry, and duplicate safety.
- Keep movement, combat, camera, UI focus, audio, game state, scene travel, saves, and networking outside the core.

### 3.2 Non-goals

- No universal `PlayerController` or movement motor.
- No definition of what project actions mean.
- No production Main Menu, pause menu, controls screen, or rebinding screen.
- No ownership of the uGUI or UI Toolkit EventSystem.
- No ownership of high-level game state, pause, time scale, or cursor state.
- No save-slot storage or global-settings file backend.
- No audio, haptics, animation, VFX, or gameplay feedback authority in the MVP.
- No automatic multiplayer lobby, network ownership, or remote-input replication.
- No requirement that projects use one generated C# wrapper, one `PlayerInput` notification mode, or one action-map naming convention.
- No mandatory branded Xbox, PlayStation, Nintendo, Steam, or third-party glyph art.
- No keylogging, raw text capture, or diagnostic recording of typed content.
- No promise that arbitrary runtime mutation of the project’s action asset is safe.
- No replacement for Unity’s Input Debugger, Input System settings, or Input System documentation.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity 6 project with an Input Action Asset | Generate a root/configuration, open the lab, and see input state without writing a manager |
| Gameplay programmer | Project actions already exist | Resolve actions by stable reference and subscribe without owning map lifecycle |
| UI programmer | Needs controls and rebinding screens | Request prompt data and rebind transactions without embedding input authority in views |
| Designer | Authors keyboard/gamepad bindings | Validate contexts, schemes, conflicts, prompts, and control pages before runtime |
| Accessibility designer | Needs alternate bindings and hold/toggle options | Use global preference bridges and clear extension points without editing gameplay controllers |
| Tester | Reproduces device/rebind/context failures | Inspect active device, contexts, locks, overrides, and stable diagnostic codes |
| Package maintainer | Changes action or binding architecture | Detect ID drift, migration needs, and package-removal boundaries |
| Local multiplayer developer | Needs device ownership seams | Use explicit user/device contracts without making multiplayer mandatory |

### 3.4 Measurable success criteria

- Package installs into a clean supported Unity project with zero compile errors.
- Runtime assembly depends only on Unity modules, Input System, and package-owned assemblies.
- Core runs with no other Sperk’s Forge package installed.
- Duplicate root is rejected before Input System side effects.
- Project action asset remains unchanged during normal Play Mode execution.
- Context and lock resolution produces deterministic enabled-map/action results.
- Releasing one lock never clears another caller’s lock.
- Analog drift and sub-threshold pointer movement do not switch active prompts.
- Rebind cancel, timeout, conflict rejection, and failure restore the previous bindings and effective action state.
- Overrides round-trip through the package document and are matched by stable IDs.
- Unknown/orphaned override entries are reported and preserved rather than silently discarded.
- Glyph resolution always returns a structured result with icon, text fallback, or explicit unavailable state.
- Sample control-display screen can be removed without breaking runtime code.
- Setup and repair tools are repeatable and non-destructive by default.
- Standalone Test Lab passes without unrelated Echo packages.
- At least one real project proves adoption without replacing its movement/controller rules prematurely.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo Unity developers and small teams.
- Gameplay and systems programmers.
- UI and accessibility implementers.
- Technical designers authoring action assets and control pages.
- QA testers reproducing device, context, and rebind issues.
- Maintainers integrating Echo packages and existing game projects.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| UC-001 | Initialize standalone input runtime | Developer | Valid configuration and action asset | One ready authority with runtime action copy and initial context | MVP |
| UC-002 | Enter gameplay context | Project code | Ready root and valid context ID | Gameplay maps enabled according to context policy | MVP |
| UC-003 | Open a menu overlay | UI/project adapter | Gameplay primary context active | Menu override enables UI maps and suppresses configured gameplay maps | MVP |
| UC-004 | Lock gameplay during transition | Scene-flow adapter | Target map/action groups configured | Lease prevents target input until that owner releases it | MVP |
| UC-005 | Nest two lock reasons | Two systems | Both acquire leases | Releasing either one leaves the other effective | MVP |
| UC-006 | Switch prompts to gamepad | Player | Meaningful paired gamepad input | Active device/scheme and prompt data update once | MVP |
| UC-007 | Ignore stick drift | Player | Gamepad resting near center | Active device does not change | MVP |
| UC-008 | Rebind a button | Player/UI | Valid action and binding ID | Transaction commits new override after validation | MVP |
| UC-009 | Cancel rebinding | Player/UI | Rebind active | Original binding and context state restored | MVP |
| UC-010 | Rebind composite parts | Player/UI | Composite action selected | Guided part sequence commits atomically or rolls back fully | MVP |
| UC-011 | Reject a conflict | Player/UI | Candidate overlaps an enabled action | Structured conflict result leaves bindings unchanged | MVP |
| UC-012 | Swap conflicting bindings | Player/UI | Project policy allows swap | Both overrides update as one transaction | MVP |
| UC-013 | Export binding overrides | Project/Accord bridge | Overrides exist | Versioned document returned without disk ownership | MVP |
| UC-014 | Import older override document | Project/bridge | Supported schema/migration | Known entries apply, orphans preserved, report produced | MVP |
| UC-015 | Render controls page | UI/sample | Control-display definition and glyph library exist | View receives text/icon prompt data without owning input logic | MVP |
| UC-016 | Lose active gamepad | Player | Gamepad disconnected | Device-lost event and configured fallback/unavailable state | MVP |
| UC-017 | Start a lab scene directly | Developer | No production root exists | Development initializer creates only EchoInput | MVP |
| UC-018 | Inspect runtime status | Tester | Authority ready | Snapshot shows contexts, locks, maps, user/device, scheme, rebind, overrides | MVP |
| UC-019 | Assign explicit devices to local user | Local multiplayer adapter | User seam enabled | User/device registration succeeds without full join system | Later |
| UC-020 | Apply sensitivity/dead-zone preference | Accord bridge | Tuning extension installed | Runtime processors or value policy update safely | Later |

### 4.3 Explicitly unsupported use cases

- Calling EchoInput to move a Rigidbody, CharacterController, pawn, camera, or cursor.
- Treating action names as stable save/network identifiers.
- Enabling/disabling package-managed maps directly from arbitrary consumers.
- Persisting binding overrides in EchoSave slot payloads by default.
- Capturing raw keyboard text or secret input for diagnostics.
- Automatically pairing network players or validating remote commands.
- Shipping proprietary controller-brand glyphs without project-provided rights.
- Expecting rebind operations to invent a usable production UI.
- Using sample action maps as a hidden dependency of project gameplay.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- EchoInput runtime authority and lifecycle.
- Runtime copy or injected collection coordination for configured actions.
- Primary context and override-context lease registry.
- Reason-based lock registry and effective action availability.
- Managed action-map/action enablement.
- Primary input-user state for the standalone MVP.
- Meaningful-device activity classification and active-device state.
- Active control-scheme selection/state for managed users.
- Interactive rebind operation lifecycle and transaction state.
- Binding conflict analysis and policy execution.
- Versioned binding-override document generation, validation, application, reset, and migration seams.
- Stable action/map/binding references and validation.
- Glyph/prompt resolution and fallback.
- Controls-display definition data.
- Input diagnostics, snapshots, validation, setup, and Test Lab.

### 5.2 The package does not own

- Movement, aiming, jumping, crawling, swimming, climbing, attacks, spells, inventory use, dialogue choices, or other gameplay meaning.
- Character roster, possession, selection, or controller implementation.
- UI root, screen stack, focus memory, modal stack, EventSystem, or visual styling.
- Game state, pause, time scale, cursor, or scene transition lifecycle.
- Global preference disk storage or save slots.
- Music, SFX, rumble, camera shake, animation, or other feedback.
- Network sessions, authority, prediction, reconciliation, or remote-input transport.
- Project action names, control-scheme names, bindings, glyph art, or control-page content.
- Platform account login or controller firmware behavior.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoInput interacts |
|---|---|---|
| Startup order | First Light or project | Optional startup-step bridge initializes/reports EchoInput |
| Diagnostics dashboard | Observatory | Optional provider bridge publishes neutral snapshots |
| Global input preferences | Accord | Separate bridge persists override/tuning documents and applies changes |
| Normal scene travel | Passage | Passage bridge acquires/releases input locks by reason |
| Runtime mode/pause | Pulse | Pulse bridge maps effective input intent to contexts/locks |
| Audio feedback | Jukebot/project | Presentation requests audio; EchoInput has no playback dependency |
| Screens/rebind UI/focus | Looking Glass or project UI | UI consumes rebind/prompt APIs; bridge coordinates UI context |
| Save files/slots | Chronicle | No default integration; bindings are global preferences |
| Player movement | EchoControllers or project | Controller consumes actions/intent; EchoInput never moves the pawn |
| Character ownership | EchoCharacters or project | Character system selects controller target; no action authority transfer in core |
| Local/online session | EchoMultiplayer or project | Later adapter assigns users/devices and validates ownership |
| Project generation | Workshop | Generates references/configuration visibly and reports selections |

### 5.4 Boundary tests

A capability belongs in EchoInput only when all of the following remain true:

1. It coordinates how project-authored input becomes available, identified, rebound, or presented.
2. It remains useful without one movement controller or gameplay genre.
3. It does not require a production UI to complete authoritative work.
4. It does not persist data to a peer-owned backend directly.
5. It can be tested with neutral sample actions.
6. It does not make high-level state or scene decisions.
7. It can fail with a structured result without gameplay listeners being present.
8. Removing a peer bridge leaves the core functional.

Examples:

- “Disable gameplay actions while a modal is open” belongs to EchoInput execution, but the fact that a modal is open belongs to EchoUI or project state.
- “Player pressed Jump” is input data; applying upward velocity belongs to a controller.
- “Store binding overrides” belongs to The Accord; producing and applying the override document belongs to EchoInput.
- “Show a controller-button icon” is prompt presentation data; the project supplies licensed art and EchoUI renders it.

---

## 6. Independence Contract

Independence is a release gate.

### 6.1 Standalone guarantees

EchoInput must:

- Compile with only its declared Unity/Input System dependencies.
- Initialize without First Light.
- Operate without any other Sperk’s Forge runtime package.
- Create no EventSystem, Canvas, audio authority, game-state authority, scene-flow authority, save/settings backend, or controller.
- Use project-owned action/configuration assets outside immutable package source.
- Expose direct setup and direct-scene paths.
- Expose interfaces and injected backends/clocks for tests.
- Fail visibly and safely when optional collaborators are absent.
- Continue operating when a diagnostics or presentation listener fails.
- Permit sample removal without runtime compile or data failure.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | Core compiles and setup window opens | Clean-project test |
| Enter Input Laboratory directly | Dev initializer creates only EchoInput | LAB and PlayMode test |
| First Light absent | Root self-initializes from scene/prefab | PlayMode test |
| Accord absent | Overrides remain in memory/export API; no disk persistence promised | Unit/PlayMode test |
| Pulse absent | Project sets contexts/locks directly | PlayMode test |
| EchoUI absent | Rebind and prompt APIs still work; sample view optional | PlayMode test |
| Observatory absent | Local snapshots/logs remain available | Diagnostic test |
| Duplicate root present | Later root self-rejects before Input System side effects | Lifecycle test |
| Required configuration missing | Authority enters Failed/Unavailable with code | Failure test |
| Action asset invalid | No maps enabled; actionable report | Validation test |
| Sample content deleted | Runtime package compiles and APIs remain usable | Removal test |
| Optional bridge removed | Core and stored unknown settings remain intact | Clean-removal test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Editor/Engine | Platform | Yes | 6000.0 | Runtime and Editor host | Package cannot function without Unity |
| `com.unity.inputsystem` | Platform package | Yes | 1.17.0 | Actions, maps, bindings, devices, users, rebinding, processors | Package reports missing hard dependency at install/compile time |
| `com.unity.test-framework` | Development/test | Tests only | Unity-supported | EditMode/PlayMode tests | Runtime unaffected |
| uGUI/TextMeshPro | Sample only | No | Project-supported | Optional sample controls/rebind display | Core unaffected; sample import declares requirement |

The package follows the current Unity 6 Input System release line. A future package release may raise the minimum Input System version only with compatibility notes and test evidence.

### 6.4 Forbidden dependencies

- Any project gameplay assembly.
- Any other Sperk’s Forge runtime package in the core assembly.
- `UnityEditor` from runtime assemblies.
- Sample assemblies/assets from runtime code.
- Generated project action-wrapper types in the package core.
- A required EventSystem, `PlayerInput`, `PlayerInputManager`, Canvas, or one notification mode.
- Hidden action-map names such as `Player`, `UI`, `Gameplay`, or `Menus`.
- Hidden tags, layers, scene names, Resources paths, StreamingAssets paths, or save filenames.
- Unlicensed controller-brand icons.
- Reflection-based peer-package discovery for normal operation.

---
## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| CAP-001 | Duplicate-safe root | One application-session input authority | Approved | Yes | Runtime | Claims before side effects |
| CAP-002 | Owned runtime actions | Clone project action asset into runtime-owned collection | Approved | Yes | Runtime | Original asset remains authoring data |
| CAP-003 | Stable references | GUID-based action/map/binding value types and assets | Approved | Yes | Runtime/Editor | Names remain display data |
| CAP-004 | Primary context | Select one base input context | Approved | Yes | Runtime | Deterministic map directives |
| CAP-005 | Override context leases | Layer temporary contexts by priority and acquisition order | Approved | Yes | Runtime | Out-of-order release safe |
| CAP-006 | Reason-based locks | All/map/action locks through leases | Approved | Yes | Runtime | Additive ownership |
| CAP-007 | Effective map/action resolution | Compute and apply minimal enable/disable diff | Approved | Yes | Runtime | Locks win after context resolution |
| CAP-008 | Active-device detection | Meaningful activity, thresholds, hysteresis, filtering | Approved | Yes | Runtime | Keyboard and mouse may form one family |
| CAP-009 | Control-scheme state | Per-managed-user scheme selection and snapshots | Approved | Yes | Runtime | Explicit policy |
| CAP-010 | Device loss/regain | Structured lost, fallback, and recovery behavior | Approved | Yes | Runtime | No gameplay meaning |
| CAP-011 | Interactive rebinding | Cancel, timeout, filtering, composite sequence | Approved | Yes | Runtime | Fresh Awaitable per operation |
| CAP-012 | Conflict analysis | Context-aware duplicate/control conflict model | Approved | Yes | Runtime/Editor | Reject is safe default |
| CAP-013 | Rebind transactions | Stage, validate, commit, or restore atomically | Approved | Yes | Runtime | No partial composite commit |
| CAP-014 | Override documents | Versioned stable-ID export/import/reset | Approved | Yes | Runtime | No permanent backend |
| CAP-015 | Orphan preservation | Preserve unknown/missing entries with warnings | Approved | Yes | Runtime | Supports clean removal and later restoration |
| CAP-016 | Glyph library | Project-authored control glyphs and fallback chains | Approved | Yes | Runtime/Editor | No branded art shipped |
| CAP-017 | Prompt resolution | Text/icon prompt data from action/binding/device state | Approved | Yes | Runtime | UI-neutral result |
| CAP-018 | Controls-display definitions | Pages/groups/action references for controls screens | Approved | Yes | Runtime/Editor/Sample | Production UI external |
| CAP-019 | Local diagnostics | Snapshot, status, codes, bounded event history | Approved | Yes | Runtime/Editor | Privacy filtered |
| CAP-020 | Setup and validation | Create/adopt/preview/repair/report | Approved | Yes | Editor | Non-destructive by default |
| CAP-021 | Direct-scene initializer | Development-only minimal root creation | Approved | Yes | Runtime/Sample | Blocked in release by default |
| CAP-022 | Input Laboratory | Isolated contexts, devices, rebinds, conflicts, glyphs | Approved | Yes | Sample | No peer Echo package |
| CAP-023 | Local-user registry | Multiple explicit users and device assignment | Deferred | No | Runtime | After single-user core proves stable |
| CAP-024 | Sensitivity/dead-zone service | Typed tuning application and preferences | Deferred | No | Runtime/Bridge | Accord integration later |
| CAP-025 | Input buffering helpers | Bounded temporal intent buffers | Deferred | No | Runtime | Controller/ability needs must prove neutrality |
| CAP-026 | Helper interactions | Toggle/repeat/chord helpers beyond built-in Input System | Deferred | No | Runtime | Avoid duplicating built-ins |
| CAP-027 | On-screen controls | Touch/on-screen control adapters and samples | Deferred | No | Runtime/Sample | Mobile-focused expansion |
| CAP-028 | Genre action templates | Platformer/top-down/FPS/etc. sample assets | Deferred | No | Samples | Never production dependencies |
| CAP-029 | XR and specialized devices | Rich XR/wheel/flight-controller policy/glyphs | Deferred | No | Runtime/Samples | Provider-specific research |
| CAP-030 | Haptics coordination | Rumble output policy | Rejected from core MVP | No | Other package/bridge | Output feedback belongs elsewhere |

### 7.2 MVP capability set

The smallest complete first release includes:

1. One duplicate-safe persistent root.
2. One project-owned configuration referencing one primary Input Action Asset.
3. An owned runtime copy of that asset.
4. Stable action, map, and binding references based on Unity GUIDs.
5. One primary context and leased override contexts.
6. Reason-based all/map/action locks.
7. Meaningful active-device and control-scheme state for one primary user.
8. Device loss/regain handling.
9. Transactional interactive rebinding with composite support.
10. Context-aware conflict analysis with Reject, AllowWithWarning, Swap, and explicit UnbindExisting policies.
11. Versioned binding-override documents, reset, validation, partial apply, orphan preservation, and migration seams.
12. Project-authored glyph library and prompt-resolution API.
13. Controls-display page/group data and one removable sample screen.
14. Local diagnostics, setup/validation tools, direct-scene support, and one Input Laboratory.

This MVP is useful without solving local multiplayer, touch gesture systems, controller movement, or production UI.

### 7.3 Later capability set

Approved later candidates include:

- multiple explicit local input users and split-device assignment;
- local join/leave provider contracts;
- per-user binding-override documents;
- sensitivity, inversion, dead-zone, and hold/toggle preference application;
- bounded action buffering helpers;
- custom chord/repeat/toggle helpers where Input System built-ins are insufficient;
- on-screen control adapters and mobile Test Lab;
- action-map template samples for common genres;
- UI Toolkit sample presentation;
- XR, wheel, HOTAS, accessibility device, and specialized glyph providers;
- richer device-layout/vendor aliasing;
- input recording/replay for tests only, with strict privacy and build exclusion.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Universal gameplay controller | Rejected | Violates package authority and genre neutrality | Never in EchoInput core |
| Production controls/rebind UI | Deferred to EchoUI/project | Presentation authority belongs elsewhere | Looking Glass bridge specification |
| Full local multiplayer auto-join | Deferred | Requires user/device/session policy workshop | EchoMultiplayer/local-player design |
| Persist overrides directly to disk | Rejected from core | Accord owns global persistence | Project may provide backend adapter |
| Branded glyph pack | Deferred | Trademark/licensing and platform requirements | Rights and provider plan approved |
| Reimplement hold/tap/multitap/deadzone | Rejected by default | Input System already owns these primitives | Only add helpers for proven gaps |
| Global raw input event history | Rejected | Privacy, noise, and performance risk | Never in release runtime |
| Haptics authority | Rejected | Output feedback is not input availability | EchoFeedback/provider specification |
| Runtime editing of source asset | Rejected | Risks shared asset contamination | Use owned runtime copy/overrides |
| Reflection-based action discovery | Rejected | Fragile and unnecessary | Explicit configuration remains standard |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | `EchoInputConfiguration`, source asset reference, context definitions, stable references, device policy, conflict policy, glyph library, control-display definitions, migration maps | Active contexts, pressed controls, rebind sessions, runtime overrides, paired device instances |
| Runtime state/behavior | Root, runtime action clone, user state, context/lock registries, device activity monitor, rebind service, override registry, prompt resolver, snapshots | Editor APIs, production UI, movement/scene/game-state rules, permanent storage |
| Presentation/feedback | Optional sample controls/rebind screen, lab readout, project/EchoUI presenters | Input authority, binding commit rules, disk persistence, gameplay meaning |

### 8.2 Component topology

```text
Project InputActionAsset (authoring only)
                |
                v
EchoInputConfiguration ---- Context/Glyph/Control Definitions
                |
                v
EchoInputRoot (duplicate-safe authority)
├── RuntimeActionCollection (owned clone)
├── PrimaryInputUser
│   ├── Paired device state
│   ├── Active device family
│   └── Active control scheme
├── InputContextService
│   ├── Primary context
│   ├── Override leases
│   ├── Lock leases
│   └── Effective map/action resolver
├── DeviceActivityService
├── InputRebindService
│   ├── Rebind session
│   ├── Conflict analyzer
│   └── Transaction/rollback
├── BindingOverrideService
├── InputPromptService
└── Diagnostics/Snapshots

Optional bridges and project consumers
├── First Light startup step
├── Pulse context adapter
├── Passage lock adapter
├── Accord persistence adapter
├── Looking Glass presenters
├── Observatory provider
└── Project controllers and gameplay systems
```

The source `InputActionAsset` remains project-owned authoring data. Runtime operations occur on an owned clone by default. Consumers resolve runtime actions through stable package references or interfaces rather than assuming the source asset instance is the enabled collection.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | Yes for the standard runtime path |
| Root type | `EchoInputRoot` |
| Lifetime | Application session by default; configurable controlled shutdown for tests |
| Duplicate behavior | First valid claimant survives; later roots reject themselves before side effects |
| Initialization trigger | Explicit `Initialize` or controlled `Awake` auto-initialize after authority claim |
| Shutdown behavior | Cancel rebind, disable actions, release users/devices, unsubscribe, destroy runtime clone, invalidate leases |
| Direct-scene behavior | Development initializer creates root only when absent |
| Test injection seam | `IInputRuntimeBackend`, `IDeviceActivitySource`, `IInputClock`, `IInputActionCollectionFactory`, `IInputDiagnosticSink` |

The root owns all subsystem children. No child service is an independent persistent singleton.

### 8.4 Authority claim and duplicate protection

`Awake` performs only the minimum claim operation:

1. Check whether a living authority is already registered.
2. If another valid authority exists, mark this instance duplicate and destroy/disable it before subscriptions or runtime object creation.
3. If no authority exists, register this instance as claimant.
4. Do not instantiate the action asset, create `InputUser`, pair devices, subscribe to `InputSystem`, load overrides, enable maps, or emit ready events until validation begins.

A duplicate produces one bounded diagnostic result. It does not reapply configuration, merge roots, transfer listeners, or disturb the survivor.

### 8.5 Runtime action-collection ownership

The default mode is `OwnedClone`:

- configuration references one project-owned `InputActionAsset`;
- initialization instantiates a runtime copy;
- all map/action enablement and binding overrides apply to the runtime copy;
- source asset definitions remain unchanged;
- stable action/map/binding GUIDs map source references to the runtime copy;
- runtime clone is destroyed at shutdown.

An advanced `InjectedCollection` mode may accept an explicitly supplied `IInputActionCollection2`, but it is not the novice path and cannot claim the same isolation guarantees when another owner also changes that collection. Generated wrappers and `PlayerInput` integrations use explicit adapters rather than becoming core requirements.

### 8.6 Context model

The runtime has exactly one **primary context** plus zero or more **override context leases**.

- Primary context represents the base mode selected by project code, such as Gameplay, Menu, or Disabled.
- Override contexts represent temporary layers such as Dialogue, Modal, Cutscene, Rebinding, or Photo Mode.
- Each definition contains stable map directives: `Enable`, `Disable`, or `Unchanged`.
- Override contexts have priority and acquisition order.
- Resolution begins from configuration baseline directives, applies the primary context, then active overrides from lower to higher authority.
- When two active overrides set the same map, higher priority wins; equal priority uses later acquisition order.
- Every lease releases only its own layer and is idempotent.
- Releasing out of order is safe because effective state is recomputed from remaining records.

Context names are project data. The package does not hardcode Gameplay or UI maps.

### 8.7 Reason-based lock model

Locks are additive leases evaluated after context resolution:

- `AllManagedInput` lock disables every managed action except explicitly configured emergency/cancel exemptions.
- `MapSet` lock disables selected action maps.
- `ActionSet` lock disables selected actions while leaving their maps available.
- `UserScope` selects the primary or a later explicit local user.
- Each request includes reason, owner label, priority metadata, optional correlation ID, and target IDs.
- A lock never unlocks input owned by another lease.
- Destroying a requester does not silently release a lock unless an explicit owner-bound helper is used.
- Shutdown invalidates all leases and restores no external map state because the root owns its runtime copy.

An internal rebinding lock temporarily suppresses conflicting gameplay input while preserving configured cancel controls.

### 8.8 Effective enablement resolution

Effective map/action state is calculated only when relevant state changes:

1. Start from configuration baseline for every managed map.
2. Apply primary context directives.
3. Apply active override directives in deterministic order.
4. Apply map locks.
5. Apply action locks.
6. Apply internal rebind suppression/exemptions.
7. Compute the difference from current runtime enablement.
8. Disable actions/maps before enabling newly permitted ones when necessary to prevent one-frame leakage.
9. Emit one immutable effective-state result.

External code must not enable or disable package-managed maps/actions. Development diagnostics detect drift. The package does not poll and fight external code every frame; it reports the ownership violation and reapplies on the next controlled resolution or explicit repair call.

### 8.9 Active-device and control-scheme model

The primary user tracks:

- paired devices;
- device-lost and regained state;
- last meaningful device;
- normalized device family such as KeyboardMouse, Gamepad, Touch, Pen, XR, or Other;
- active control scheme/binding group;
- last meaningful activity time from an injected unscaled clock;
- reason for the last switch.

Meaningful activity policy filters:

- analog values below configurable magnitude thresholds;
- pointer deltas below configurable distance/time thresholds;
- noisy controls and unsupported sensors;
- synthetic/virtual devices unless explicitly allowed;
- events from devices not assigned to the user;
- repeated held-state events that do not represent new intent.

Keyboard and mouse may be grouped as one prompt family. Button presses may switch immediately. Analog/pointer activity may require threshold plus hysteresis. A device switch changes prompt/control-scheme state; it does not change gameplay context.

### 8.10 Primary-user device policy

The MVP supports one managed primary user with configurable policy:

- `AutoPairFirstEligible` is the default: keyboard and mouse are paired together when available, plus the first eligible gamepad.
- `RequireExplicitPairing` waits for project code/setup.
- `SingleUserAllEligible` may pair all eligible devices in a strictly single-player project.
- Unpaired-device takeover is disabled by default once a gamepad is assigned, preventing a second controller from stealing player-one prompts accidentally.
- Additional gamepads remain available for later local-player adapters.

Device pairing is an infrastructure decision, not proof that a gameplay player/character exists.

### 8.11 Rebinding transaction model

A rebind operation is a single-use session:

1. Validate user, action ID, binding ID, expected control type, scheme, and current authority state.
2. Snapshot target and related overrides plus effective context/lock state.
3. Acquire the internal rebind context/lock.
4. Disable the target action as required by the backend.
5. Start a configured `PerformInteractiveRebinding` backend operation.
6. Filter cancel controls, excluded paths, device families, noisy controls, and invalid candidates.
7. For composite bindings, collect each selected part into one staged transaction.
8. Build candidate override entries without committing them to the authoritative document.
9. Analyze conflicts using active-context overlap and binding-group rules.
10. Apply the selected conflict policy.
11. Commit all staged entries or restore the complete snapshot.
12. Restore effective context/action state.
13. Release internal leases and dispose the underlying operation.
14. Return a structured result.

Only one interactive rebind session may be active per managed user. The MVP primary user therefore has at most one active session. Re-entry returns Busy.

### 8.12 Conflict model

Two bindings conflict only when their controls can be active simultaneously under the configured project model. Analysis considers:

- normalized effective control path;
- binding group/control scheme;
- user scope;
- action expected control type;
- composite root and part identity;
- context overlap matrix;
- explicit shareable-action or shareable-binding markers;
- disabled/orphaned actions;
- reserved/cancel controls;
- project conflict policy.

A gameplay `Interact` binding and a UI `Submit` binding are not automatically a conflict when their contexts are proven mutually exclusive. Two gameplay actions on the same button in overlapping contexts are conflicts unless explicitly shareable.

Policies:

- `Reject` is the safe default.
- `AllowWithWarning` commits and records the conflict.
- `Swap` exchanges target and conflicting override paths as one transaction.
- `UnbindExisting` removes the other override only after explicit destructive confirmation by the caller.
- `CustomResolver` allows project code to return a documented decision without mutating authoritative state directly.

### 8.13 Binding-override document model

EchoInput does not use action names or binding indexes as durable identity. `BindingOverrideDocument` contains:

- document schema version;
- project/input configuration stable ID;
- source action-asset identity and authoring revision/fingerprint;
- created/updated timestamps;
- user/profile scope metadata where applicable;
- `BindingOverrideEntry` records keyed by action GUID and binding GUID;
- override path, interactions, processors, and optional display metadata;
- orphan/unknown entries preserved from previous loads;
- migration history and warnings;
- optional raw Unity override JSON only as an interchange/debug field, not authoritative identity.

Application occurs against a scratch runtime clone or staged table first. Known valid entries commit together according to policy. Unknown entries remain preserved and reported.

### 8.14 Glyph and prompt model

`InputGlyphLibrary` is project-owned and contains ordered glyph sets keyed by device layout, family, control scheme, vendor/product matcher, and fallback priority. A glyph entry maps a normalized control path or semantic alias to a sprite/icon reference and optional short text.

Prompt resolution uses:

1. requested action/binding and user;
2. active or explicitly requested control scheme/device family;
3. current binding override;
4. exact device/layout glyph set;
5. family glyph set;
6. generic control-path glyph;
7. human-readable binding text;
8. explicit unavailable result.

The runtime returns neutral prompt data. It does not instantiate UI. The package ships only generic, redistribution-safe placeholders and text fallback examples.

### 8.15 Lifecycle sequence

1. Claim authority.
2. Validate configuration, source asset, stable IDs, contexts, policies, and dependencies.
3. Create runtime action clone and package services.
4. Create primary `InputUser` and apply configured pairing policy.
5. Register device/activity callbacks.
6. Apply any override document supplied before readiness.
7. Select initial primary context and resolve effective enablement.
8. Mark Ready and emit immutable snapshot.
9. Process contexts, locks, device changes, rebinds, prompt queries, and override updates.
10. On shutdown, reject new requests, cancel rebind, disable actions, release users/devices, unsubscribe, invalidate handles, destroy clone, and clear static authority.

### 8.16 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Authority claim | Warning/report | Duplicate rejected; survivor unchanged | EIN-001 |
| Missing configuration | Preflight | Blocking status | No actions created | EIN-002 |
| Missing source action asset | Preflight | Blocking status | No actions created | EIN-003 |
| Invalid/duplicate stable reference | Validation | Blocking report | Invalid feature disabled or init fails | EIN-004 |
| Runtime clone creation fails | Initialization | Blocking status | Authority Failed | EIN-005 |
| No input device | Initialization/runtime | Unavailable device status | Context stays valid; waits for device | EIN-006 |
| Context ID unknown | Request | Rejected result | Effective state unchanged | EIN-101 |
| Context lease stale | Release | No-op/warning | Remaining contexts recomputed | EIN-102 |
| Lock target unknown | Request | Rejected result | State unchanged | EIN-111 |
| External map drift | Development audit | Warning | Reapply on explicit resolution | EIN-120 |
| Rebind already active | Rebind start | Busy result | Existing session continues | EIN-201 |
| Rebind candidate invalid | Candidate validation | Continue/reject result | Wait for valid control or timeout | EIN-202 |
| Rebind canceled | Session | Canceled result | Snapshot restored | EIN-203 |
| Rebind timeout | Session | Timed-out result | Snapshot restored | EIN-204 |
| Binding conflict rejected | Commit validation | Conflict result | Bindings unchanged | EIN-205 |
| Composite part failure | Session | Failed result | Entire sequence rolled back | EIN-206 |
| Override document corrupt | Import | Error/report | Preserve input; do not apply | EIN-301 |
| Newer unsupported document | Import | Unsupported result | Preserve document; defaults/current remain | EIN-302 |
| Source asset identity mismatch | Import | Migration-required result | Partial apply only by explicit policy | EIN-303 |
| Orphan override entry | Import | Warning | Preserve but do not apply | EIN-304 |
| Glyph missing | Prompt resolution | Text fallback/unavailable | No exception | EIN-401 |
| Device lost | Runtime callback | Device-lost event | Configured fallback or unavailable | EIN-501 |
| Diagnostic listener fails | Event dispatch | Development warning | Core continues | EIN-901 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable identity | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoInputConfiguration` | Selects source action asset, runtime ownership mode, context catalog, device rules, rebind policy, glyph libraries, limits, and diagnostics options | Unity asset GUID for Editor identity plus project-authored source identity/fingerprint | No | Yes |
| `InputContextCatalog` | Defines primary and override contexts plus map directives and dominance values | Unity asset GUID plus domain `InputContextId` values | No | Yes |
| `InputContextDefinition` | Describes one semantic input mode such as Gameplay, Menu, Dialogue, Cutscene, or Rebinding | Stable string/GUID ID | No | Yes |
| `InputMapDirective` | Declares Enable, Disable, or Unchanged for one action-map GUID | Map GUID | No | Yes |
| `InputLockPolicy` | Defines lock targets, exempt actions, and reason metadata | Stable policy ID | No | Yes |
| `InputDevicePolicy` | Defines supported layouts, meaningful-activity thresholds, pairing rules, and fallback order | Unity asset GUID for Editor identity; optional policy ID when externally referenced | No | Yes |
| `InputRebindPolicy` | Defines candidate filters, timeout, conflict strategy, reserved controls, and composite behavior | Unity asset GUID for Editor identity; optional policy ID when externally referenced | No | Yes |
| `InputBindingMetadataCatalog` | Adds project-authored labels, shareability, reservation, display grouping, and migration aliases to action/binding GUIDs | Action/binding GUID | No | Yes |
| `InputGlyphLibrary` | Maps control paths and device families to project-owned glyph references and text fallbacks | Unity asset GUID plus stable mapping keys | No | Yes |
| `ControlDisplayDefinition` | Defines pages, groups, labels, and action references for a controls guide | Unity asset GUID plus domain page/group IDs | No | Yes |
| `BindingOverrideDocument` | Serializable, versioned record of project/user override entries | Document ID plus action/binding GUID | Loaded into runtime model | Created by project/settings integration |
| `BindingOverrideMigrationMap` | Declares aliases or replacements for released action/binding IDs | Stable migration ID | No | Yes |

The project’s `InputActionAsset` remains the authoritative authoring definition. By default, EchoInput clones that asset at runtime and owns the cloned action collection. It never writes runtime enablement or binding overrides back into the project asset.

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `EchoInputRuntimeState` | `EchoInputRoot` | Application session | New authority/reset command | Never serialized directly |
| Runtime action collection | `EchoInputRoot` or injected adapter | Application session | Re-clone/reinject only while inactive | Source asset serialized by project; runtime copy is not |
| Primary context state | Context service | Application session | Configuration default or explicit request | Optional project preference only if separately designed |
| Active override leases | Context service | Until released/root shutdown | All leases invalidated on reset | Never persisted |
| Active lock leases | Lock service | Until released/root shutdown | All leases invalidated on reset | Never persisted |
| Effective map/action enablement | Context/lock resolver | Recomputed after semantic changes | Recalculate from source policy | Never persisted |
| Active input user | User/device service | Application session | Device/user reset | Pairing preference may be exported; live handles are not |
| Active device/control scheme | Device service | Application session | Meaningful input or explicit selection | Optional last-used preference through project integration |
| Rebind session | Rebind service | One transaction | Commit/cancel/fail/timeout/root shutdown | Only committed override entries export |
| Binding override model | Override service | Application session | Import/reset/change | Export as `BindingOverrideDocument` |
| Conflict analysis | Rebind service | One candidate/commit attempt | Recomputed | Never persisted |
| Prompt-resolution cache | Glyph service | Bounded session cache | Device/scheme/library change | Never persisted |
| Diagnostics counters/history | Root diagnostics | Bounded application session | Reset/root shutdown | Export only in redacted diagnostic snapshot |

### 9.3 Stable identifiers

EchoInput uses Unity Input System identifiers wherever Unity already provides stable identity:

- Actions are keyed by `InputAction.id`.
- Action maps are keyed by `InputActionMap.id`.
- Bindings are keyed by `InputBinding.id`.
- Composite roots and parts retain their individual binding IDs.
- Control schemes use a project-authored stable scheme key plus their display name.
- EchoInput context, page, group, policy, and migration records use validated package/project IDs independent of display labels.

Names and array indexes are authoring conveniences, never persistence authority. Renaming an action, moving it between maps, changing its display label, or reordering bindings must not silently orphan a released override when the underlying GUID remains stable.

Validation must detect:

- Empty or duplicate EchoInput IDs.
- Missing or duplicated Input System GUID references.
- Binding metadata that points to absent actions or bindings.
- Context directives targeting absent maps.
- Migration maps with cycles or ambiguous destinations.
- Control-display entries whose actions no longer exist.
- Multiple project assets claiming the same released source identity.

When an intentional breaking asset replacement changes Unity GUIDs, the project supplies a migration map. Unknown or orphaned override entries remain in the document so reinstalling an optional map/package can restore them.

### 9.4 ScriptableObject safety

Configuration assets are immutable during ordinary play. In particular, they must not store:

- Current context or active context leases.
- Active lock counts or owners.
- Runtime enabled/disabled map state.
- Current device or paired-user handles.
- Rebind operation objects.
- Current candidate controls.
- Runtime binding overrides.
- Prompt cache entries.
- Last-input timestamps or raw input history.

Editor preview tooling may construct temporary runtime copies. It must never contaminate committed project assets unless the user explicitly applies an authoring change through Unity’s normal serialized workflow.

### 9.5 Binding override document

The package-owned document is explicit rather than opaque:

```text
BindingOverrideDocument
├── documentSchemaVersion
├── packageVersion
├── sourceAssetIdentity
├── sourceAssetFingerprint
├── userProfileId (optional project-safe identifier)
├── generatedUtc
├── entries[]
│   ├── actionId
│   ├── bindingId
│   ├── overridePath
│   ├── overrideProcessors
│   ├── overrideInteractions
│   ├── groups
│   ├── compositeRelationship
│   └── metadata
├── orphanEntries[]
└── extensionData
```

Rules:

1. Empty fields mean “no override,” not “guess from the current index.”
2. Entries are normalized and validated before application.
3. Import produces a report with applied, skipped, migrated, orphaned, conflicting, and invalid counts.
4. Unknown extension data is preserved through an opaque raw record or an extension-data-capable serializer; `JsonUtility` round-tripping alone does not satisfy this rule.
5. Newer unsupported documents are not destructively rewritten.
6. A partial import never silently discards entries that could not be applied.
7. Export is deterministic enough for useful diffing and support review.
8. The project decides the actual storage profile, encryption, cloud, or account boundary.

Unity’s binding-override JSON may be accepted through a conversion/import utility for interoperability. It is not the package’s long-term authority because the package needs stable migration reporting, orphan preservation, and project-specific metadata.

### 9.6 Source asset identity and fingerprint

The configuration stores:

- A stable project-authored source identity.
- The source `InputActionAsset` GUID when available in Editor tooling.
- A deterministic fingerprint derived from action/map/binding IDs and relevant control-scheme structure.
- The last compatible fingerprint range or migration version when released.

A changed fingerprint is not automatically an error. It triggers compatibility analysis. Added bindings may require no migration; removed or replaced IDs may produce orphans; changed expected control types may reject old paths.

### 9.7 Control-display and prompt data

Control-display definitions reference semantic actions rather than hard-coded glyphs. At runtime, the glyph service resolves:

```text
Action + binding selection policy + active scheme/device
    -> effective control path
    -> exact glyph
    -> device-family glyph
    -> generic glyph
    -> localized/project text fallback
```

A missing art asset is never a fatal input failure. The returned prompt result states its source and availability so UI can present a glyph, text label, generic control family, or an explicit unavailable state.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `EchoInputRoot` | `MonoBehaviour` | Claims authority and owns package runtime services | Scene/prefab/setup tool |
| `IEchoInputService` | Interface | Main programmer-facing input infrastructure facade | Implemented by root/runtime |
| `IEchoInputActions` | Interface | Read-only access/query seam for the owned runtime action collection | Root-owned or injected |
| `IInputContextService` | Interface | Primary context and leased override management | Root-owned |
| `IInputLockService` | Interface | Reason-based map/action/all-input locks | Root-owned |
| `IInputDeviceService` | Interface | Active device, scheme, user, and meaningful-activity state | Root-owned |
| `IInputRebindService` | Interface | Transactional interactive rebinding | Root-owned |
| `IBindingOverrideService` | Interface | Import, export, reset, query, and apply override model | Root-owned |
| `IInputPromptService` | Interface | Resolve action prompts/glyphs/text | Root-owned |
| `IInputConflictResolver` | Interface | Optional project conflict strategy | Injected/configured |
| `IInputClock` | Interface | Unscaled timeout/timestamp seam | Injected/default Unity implementation |
| `InputContextLease` | Disposable struct/class | Owns one override-context request | Returned by context service |
| `InputLockLease` | Disposable struct/class | Owns one lock request | Returned by lock service |
| `InputRebindSession` | Class/handle | Controls and observes one rebind transaction | Rebind service |
| `InputContextId` | Value type | Stable semantic context identity | Project/configuration |
| `InputLockRequest` | Struct | Lock target, reason, owner, and exemptions | Caller-created |
| `InputPromptRequest` | Struct | Action, binding, scheme, player, and fallback selection | Caller-created |
| `InputPromptResult` | Struct | Resolved glyph/text/control-path state | Service-created |
| `InputConflictReport` | Immutable result | Candidate conflict details and possible actions | Rebind service |
| `InputRebindResult` | Immutable result | Commit/cancel/failure outcome and changes | Rebind service |
| `BindingOverrideImportReport` | Immutable result | Applied/migrated/orphaned/rejected entries | Override service |
| `EchoInputSnapshot` | Immutable snapshot | Structured current status for diagnostics | Root-created |

### 10.2 Representative public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Main-loop rule |
|---|---|---|---|---|
| `EchoInputRoot.Current` | Optional convenience access to claimed authority | Root exists | `null`/try pattern when absent | Main thread |
| `IEchoInputService.IsInitialized` | Read initialization state | None | Read-only | Main thread |
| `IEchoInputService.Actions` | Access owned/injected runtime action collection | Initialized | Throws only for programmer misuse or returns unavailable result, per final API style | Main thread |
| `SetPrimaryContext(InputContextId)` | Replace primary semantic context | Valid configured context | Structured result; no partial map state | Main thread |
| `AcquireOverride(InputContextId, owner, reason)` | Add temporary higher-order context | Initialized and valid | Lease or failure result | Main thread |
| `AcquireLock(InputLockRequest)` | Add reason-based lock | Initialized and valid target | Lease or failure result | Main thread |
| `GetEffectiveMapState(mapId)` | Query resolved map state and reasons | Initialized | Structured state | Main thread |
| `TryGetAction(actionId)` | Resolve action by stable ID | Initialized | `false` when unavailable | Main thread |
| `BeginRebindAsync(request, token)` | Run transactional interactive rebind | No conflicting session for user | Fresh `Awaitable<InputRebindResult>` | Starts/completes on Unity main thread unless documented |
| `CancelRebind(sessionId)` | Cancel active session | Matching session exists | Idempotent result | Main thread |
| `AnalyzeConflict(candidate)` | Inspect conflict without committing | Valid candidate | Immutable report | Main thread |
| `ImportOverrides(document, policy)` | Validate/migrate/apply override document | Initialized and not actively rebinding unless policy allows | Atomic report; prior state retained on fatal failure | Main thread |
| `ExportOverrides()` | Create current versioned document | Initialized | New document snapshot | Main thread |
| `ResetOverrides(scope)` | Clear selected committed runtime overrides | Initialized | Result plus changed bindings | Main thread |
| `ResolvePrompt(request)` | Produce glyph/text prompt data | Valid action or control reference | Fallback-aware result | Main thread |
| `PairDevice(request)` | Explicitly pair eligible device/user | Device present and policy allows | Structured pairing result | Main thread |
| `GetSnapshot()` | Read diagnostic status | Root available | Immutable bounded snapshot | Main thread |

Final signatures may be adjusted during implementation only when the specification or an ADR is updated first.

### 10.3 Result conventions

Operational APIs return explicit outcomes instead of relying on Console messages:

- Success
- No change
- Rejected
- Invalid request
- Unavailable
- Conflict
- Busy
- Canceled
- Timed out
- Unsupported
- Migration required
- Failed

Programmer contract violations may use exceptions in development builds when they cannot be represented as normal user input, but ordinary absence, lock, conflict, or device-loss conditions are results.

### 10.4 Lease behavior

Context and lock leases are:

- Unique and traceable by opaque ID.
- Safe to dispose more than once.
- Safe to release out of acquisition order.
- Invalid after authority reset or shutdown.
- Diagnosable by reason/owner without retaining scene-object references indefinitely.
- Automatically released only when an explicitly configured owner-lifetime adapter is used; ordinary callers remain responsible for disposal.

A leaked lease appears in development diagnostics. The core does not secretly expire a valid gameplay lock by wall-clock time unless the request explicitly asks for a bounded timeout.

### 10.5 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `Initialized` | Root | After full successful initialization | Snapshot/report | Listener optional |
| `InitializationFailed` | Root | After blocking failure | Failure report | Listener optional |
| `PrimaryContextChanged` | Context service | After state and map directives commit | Previous/current context | Do not mutate source assets |
| `EffectiveContextChanged` | Context service | After override resolution changes | Effective context stack/summary | May update UI |
| `EffectiveMapStateChanged` | Context/lock resolver | After map/action enablement commits | Changed map/action states and reasons | Semantic change only |
| `LockAcquired` / `LockReleased` | Lock service | After lease state changes | Safe metadata | Development/bridges |
| `ActiveDeviceChanged` | Device service | After meaningful-device filter accepts change | Previous/current device family/layout | Prompt/UI listeners |
| `ControlSchemeChanged` | Device service | After scheme selection commits | Previous/current scheme | Prompt/UI listeners |
| `DeviceAvailabilityChanged` | Device service | After add/remove/loss/regain | Safe device summary | No raw identifiers |
| `RebindStarted` | Rebind service | After snapshot and internal lock commit | Session summary | Optional UI |
| `RebindCandidateChanged` | Rebind service | After candidate validation | Sanitized candidate | No raw event stream |
| `RebindConflictDetected` | Rebind service | Before commit | Conflict report | UI may request resolution |
| `RebindCompleted` | Rebind service | After atomic commit | Rebind result | Save bridge may export |
| `RebindCanceled` / `RebindFailed` | Rebind service | After rollback | Result | UI may close |
| `BindingOverridesChanged` | Override service | After committed model changes | Changed stable IDs/revision | Persistence bridge may save |
| `PromptInvalidated` | Prompt service | After scheme/device/library/override change | Scope of invalidation | UI refresh |
| `DiagnosticEventRaised` | Root | After bounded record addition | Redacted diagnostic event | Listener failure isolated |
| `ShuttingDown` | Root | Before leases/actions/users dispose | Shutdown reason | No new requests accepted |

Events occur after authoritative state changes. A listener is never required for input state to commit.

### 10.6 Action consumption boundary

EchoInput does not wrap every gameplay action in custom events. Consumers may:

- Receive the owned runtime `InputAction` references through stable lookup.
- Use project-generated wrappers through an explicit adapter that targets the runtime collection.
- Use a project-owned `PlayerInput` adapter in advanced mode.
- Subscribe directly to action phases and dispose cleanly.
- Read values through project controller/input-reader code.

EchoInput owns action availability, context, pairing, rebind state, and prompt metadata. It does not interpret “Jump,” “Attack,” “Interact,” or movement values as gameplay behavior.

### 10.7 Async and cancellation policy

Interactive rebinding uses a fresh Unity `Awaitable<T>` per operation. It supports:

- Caller cancellation.
- Explicit cancel controls.
- Unscaled timeout.
- Root shutdown cancellation.
- Device loss.
- Candidate exclusion/cancel matching.
- Composite multi-step sessions.
- One active rebind session per user by default.
- Atomic commit only after all required parts and conflicts are resolved.

A canceled or failed operation restores the exact pre-session runtime override snapshot. Completed `Awaitable` instances are never cached or awaited twice.

### 10.8 Conflict resolver contract

A custom resolver receives an immutable report and returns one of the allowed resolution commands. It must not directly mutate the action asset or override model. EchoInput validates the command, previews affected entries, and performs the atomic commit/rollback.

### 10.9 API ergonomics

Novice path:

1. Install package and project Input System dependency.
2. Run **Tools > EchoDevGames > The Will > Setup**.
3. Select the project action asset.
4. Generate a configuration and root prefab.
5. Open the Input Laboratory.
6. Use stable action references/prompts through sample components.

Advanced path:

- Inject an action-collection adapter.
- Supply custom device policy, pairing provider, clock, conflict resolver, glyph provider, or persistence bridge.
- Subscribe to structured state and consume native Input System actions.
- Test services without relying on the static convenience access point.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install/verify the supported Unity Input System package.
2. Open **Tools > EchoDevGames > The Will > Setup**.
3. Select or create the project-owned `InputActionAsset`.
4. Choose runtime ownership mode, with **Owned Runtime Clone** recommended.
5. Create/select the EchoInput configuration, context catalog, device policy, rebind policy, glyph library, and controls-display definition.
6. Preview all assets, prefab, sample, and project-setting changes.
7. Apply create-only-safe changes.
8. Add the root prefab to the project’s Boot scene or use the documented standalone route.
9. Import/open the Input Laboratory.
10. Run validation and export the setup report.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create configuration | Project-owned configuration assets | Nothing existing by default | Yes | Unity Undo | Asset paths/IDs |
| Create root prefab | Root and owned service hierarchy | Nothing existing | Yes | Unity Undo | Components/references |
| Generate context catalog | Starter contexts/directives from selected maps | New asset only | Yes | Unity Undo | Map coverage |
| Generate binding metadata | Entries for action/binding GUIDs | New asset or previewed merge | Yes | Backup + Undo | Added/orphaned entries |
| Generate controls display | Starter pages/groups from selected maps | New asset only | Yes | Unity Undo | Page/action coverage |
| Validate source asset | Nothing | Nothing | Yes | Not applicable | Validation report |
| Repair missing references | User-selected safe references | Configuration/prefab | Yes | Preview + Undo | Exact changes |
| Migrate identifiers | Alias map/document preview | Project-owned migration asset/document | Yes when idempotent | Backup required | Migrated/orphaned IDs |
| Convert Unity override JSON | New package document | Nothing unless user applies | Yes | Source retained | Conversion report |
| Create Boot integration | Optional scene object/prefab instance | Selected scene | Yes | Unity Undo | Scene change report |
| Reset generated sample data | Sample-only assets/state | Sample content | Yes | Sample reset | Reset report |

No tool silently changes **Active Input Handling**, replaces a project action asset, edits gameplay maps, removes bindings, or overwrites project-authored glyphs.

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| The Will Setup | Installer | Guided configuration and root creation | No |
| Configuration inspector | Programmer/designer | Show source identity, mode, policies, limits, and health | No |
| Context Catalog inspector | Designer/programmer | Edit/preview map directives and dominance | No |
| Binding Metadata inspector | Designer | Author labels, shareability, reservations, and aliases | No |
| Rebind Policy inspector | Programmer/designer | Candidate filters and conflict behavior | No |
| Glyph Library inspector | UI designer | Map controls/families to project assets/text | No |
| Controls Display editor | UI designer | Organize pages/groups/actions | No |
| Override Document inspector | Developer/support | Review redacted stable entries and migration state | No |
| Input Runtime Monitor | Developer | Observe contexts, locks, device family, sessions, and action enablement | Development runtime |
| Validation window | Installer/release engineer | Run package/project checks | No |
| Migration preview | Maintainer | Compare source asset versions and map old IDs | No |
| Input Laboratory launcher | Tester | Import/open/reset sample | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| EIN-VAL-001 | Input System dependency absent/unsupported | Blocker | Guidance | No |
| EIN-VAL-002 | Project Active Input Handling incompatible | Error | Guidance | No |
| EIN-VAL-003 | Configuration missing | Blocker | Yes | Yes, create-only |
| EIN-VAL-004 | Source action asset missing | Blocker | Select | No |
| EIN-VAL-005 | Source identity empty/duplicate | Error | Generate/select | With confirmation |
| EIN-VAL-006 | Runtime ownership mode invalid | Error | Yes | Yes |
| EIN-VAL-007 | Context catalog missing | Error | Create | Yes |
| EIN-VAL-008 | Duplicate/empty context ID | Error | Suggest | No |
| EIN-VAL-009 | Context map directive target missing | Error | Remove/remap | No |
| EIN-VAL-010 | No default primary context | Error | Select | No |
| EIN-VAL-011 | Multiple default primary contexts | Error | Select one | No |
| EIN-VAL-012 | Dominance tie lacks deterministic order | Error | Generate order | Yes |
| EIN-VAL-013 | Required cancel/emergency path disabled by all policies | Blocker | Guidance | No |
| EIN-VAL-014 | Lock policy target missing | Error | Remap | No |
| EIN-VAL-015 | Device policy missing/empty | Warning/Error | Create | Yes |
| EIN-VAL-016 | No keyboard/controller path where project claims support | Warning | Guidance | No |
| EIN-VAL-017 | Meaningful-input thresholds invalid | Error | Clamp suggestion | With confirmation |
| EIN-VAL-018 | Rebind policy missing | Error | Create | Yes |
| EIN-VAL-019 | Rebind timeout nonpositive/excessive | Error/Warning | Normalize | With confirmation |
| EIN-VAL-020 | Reserved control rule invalid | Error | Edit | No |
| EIN-VAL-021 | Default conflict policy destructive without confirmation | Blocker | Change | Yes |
| EIN-VAL-022 | Binding metadata action/binding missing | Warning | Orphan/remove/remap | No |
| EIN-VAL-023 | Duplicate binding metadata key | Error | Merge guidance | No |
| EIN-VAL-024 | Migration alias cycle/ambiguity | Blocker | Guidance | No |
| EIN-VAL-025 | Glyph library missing | Warning | Create | Yes |
| EIN-VAL-026 | Glyph entry duplicate/unreachable | Warning | Guidance | No |
| EIN-VAL-027 | No text fallback for advertised control | Warning | Generate label | With confirmation |
| EIN-VAL-028 | Control display references missing action | Error | Remove/remap | No |
| EIN-VAL-029 | Control display page/group IDs duplicate | Error | Suggest | No |
| EIN-VAL-030 | Root absent from configured Boot scene | Warning/Error by policy | Add | With scene confirmation |
| EIN-VAL-031 | Multiple roots in Boot/build scenes | Blocker | Locate/remove guidance | No |
| EIN-VAL-032 | Direct-scene initializer enabled for release | Error | Disable | Yes |
| EIN-VAL-033 | Runtime assembly references Editor/sample/UI assembly | Blocker | Guidance | No |
| EIN-VAL-034 | Project action source appears runtime-mutated in asset | Warning | Reimport/reset guidance | No |
| EIN-VAL-035 | Unsupported binding override document schema | Error | Migration guidance | No |
| EIN-VAL-036 | Branded glyph asset licensing notice missing | Error | Guidance | No |

Validation results are structured, stable-coded, filterable, and exportable. Auto-fixes operate only where intent is unambiguous and non-destructive.

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Supported for v1.0.0:

- Embedded package development.
- Local folder reference.
- UPM Git URL after repository release.
- UPM tarball.
- The Workshop selection when that package exists.

The package manifest declares the supported Unity Input System dependency. Samples remain optional imports.

### 12.2 Minimal scene setup

Minimum production configuration:

1. One project-owned `InputActionAsset`.
2. One `EchoInputConfiguration`.
3. One `EchoInputRoot` prefab/scene instance.
4. The selected context and device policies.
5. A default primary context.
6. A Boot scene or equivalent application-session creation point.
7. Project gameplay code that consumes native actions or stable lookups.

Glyphs, controls-display definitions, rebind UI, persistence bridges, and peer integrations are optional.

### 12.3 Boot-scene setup

The normal path places one configured root in the canonical Boot scene:

1. `Awake` claims authority.
2. A duplicate destroys/disables itself before Input System side effects.
3. The winner clones or accepts the runtime action collection.
4. It validates stable identity and policies.
5. It creates the primary input user/device service if enabled.
6. It applies imported override data supplied before or during initialization.
7. It resolves the default primary context and lock state.
8. It begins meaningful-device monitoring.
9. It marks itself Ready and publishes the snapshot.

First Light may initialize the root through an optional startup-step bridge. EchoInput must still be able to initialize without First Light.

### 12.4 Direct-scene setup

A development-only `EchoInputDirectSceneInitializer` may:

- Detect an existing authority.
- Instantiate the configured development root only when absent.
- Use the same duplicate-claim path as production.
- Clearly report development initialization.
- Apply a configured test context.
- Disable itself outside Editor/development builds unless explicitly approved.
- Tear down only the authority it created.

It does not create an alternate gameplay input system or use different context rules.

### 12.5 Generated wrappers and `PlayerInput`

Project-generated action wrappers are supported through project adapters. The package core does not compile against generated types.

`PlayerInput` is optional. A project may use:

- EchoInput-owned runtime actions without `PlayerInput`.
- A project adapter that binds generated wrappers to the owned runtime collection.
- Advanced injected mode in which a project-owned `PlayerInput`/`InputUser` collection is supplied to EchoInput.

The selected mode must be explicit because multiple runtime copies of an `InputActionAsset` can produce confusingly separate enablement and override state.

### 12.6 Scene isolation rule

The Input Laboratory contains only:

- EchoInput
- The declared Input System dependency
- Redistributable sample UI/assets
- Lightweight sample consumers

It must not require First Light, The Observatory, The Accord, The Passage, The Pulse, Resonance, EchoUI, EchoCharacters, EchoControllers, or project gameplay assemblies.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Test Lab purpose

The **Will Input Laboratory** proves the package’s complete MVP without any other Sperk’s Forge package. It demonstrates that one project action asset can be cloned safely, activated through semantic contexts, locked by reason, paired to supported devices, rebound transactionally, persisted as a portable override document, and presented through project-replaceable prompt data.

The laboratory is evidence of package behavior, not a production input template.

### 13.2 Required Test Lab contents

- One sample action asset with Gameplay, UI, Dialogue, and Rebinding maps.
- Keyboard/mouse and generic gamepad control schemes.
- One configured `EchoInputRoot`.
- Primary-context controls.
- Override-context acquisition/release controls.
- Map/action/all-input lock controls.
- Current effective context and map-state readout.
- Meaningful-device and control-scheme readout.
- Device add/remove/pair simulation where supported.
- Transactional single binding rebind.
- Composite movement rebind.
- Candidate, conflict, cancel, timeout, commit, and rollback displays.
- Override export/import/reset controls.
- Prompt/glyph/text fallback display.
- Duplicate-root spawn control.
- Runtime source-asset immutability check.
- Reset control that returns the sample to a deterministic baseline.
- Diagnostic snapshot/export control.
- No copyrighted or vendor-restricted glyph art.

### 13.3 Test Lab acceptance checklist

| Test | Action | Expected result | Automated/manual | Status |
|---|---|---|---|---|
| LAB-001 | Enter laboratory directly | One authority initializes and default context becomes Ready | Both | Not run |
| LAB-002 | Start through sample Boot scene | Same authority and state as direct entry | Manual | Not run |
| LAB-003 | Spawn duplicate root before initialization | Duplicate exits before actions, callbacks, or users duplicate | Both | Not run |
| LAB-004 | Spawn duplicate root after Ready | Winner remains unchanged; duplicate reports rejection | Both | Not run |
| LAB-005 | Remove configuration | Root fails visibly with no partially enabled maps | Both | Not run |
| LAB-006 | Remove source action asset | Blocking result identifies missing source | Both | Not run |
| LAB-007 | Verify runtime ownership | Runtime collection is distinct from source asset in default mode | Automated | Not run |
| LAB-008 | Change primary Gameplay to UI | Directives commit atomically and event fires once | Both | Not run |
| LAB-009 | Acquire Dialogue override | Effective context changes while primary remains Gameplay | Both | Not run |
| LAB-010 | Acquire higher-priority Rebinding override | Rebinding dominates deterministically | Both | Not run |
| LAB-011 | Release lower lease out of order | Remaining effective context stays correct | Both | Not run |
| LAB-012 | Dispose a context lease twice | No underflow, exception, or state corruption | Automated | Not run |
| LAB-013 | Acquire action lock | Target action becomes unavailable while other actions remain | Both | Not run |
| LAB-014 | Acquire map lock | Target map becomes unavailable with reason visible | Both | Not run |
| LAB-015 | Acquire global lock with cancel exemption | All managed input except emergency/cancel path locks | Both | Not run |
| LAB-016 | Release locks out of order | Effective state recomputes correctly | Both | Not run |
| LAB-017 | Externally enable a package-disabled map | Drift is detected and reported according to policy | Both | Not run |
| LAB-018 | Move gamepad stick below threshold | Active device does not switch from keyboard/mouse | Both | Not run |
| LAB-019 | Press gamepad button | Active device/scheme changes once | Both | Not run |
| LAB-020 | Move mouse by tiny jitter | Prompt device does not flutter | Both | Not run |
| LAB-021 | Produce meaningful mouse activity | Active device switches to keyboard/mouse family | Both | Not run |
| LAB-022 | Remove active gamepad | Availability event fires and configured fallback applies | Both | Not run |
| LAB-023 | Reconnect eligible gamepad | Device can be re-paired without duplicate callbacks | Both | Not run |
| LAB-024 | Begin valid single rebind | Internal lock/context applies and candidate is displayed | Both | Not run |
| LAB-025 | Complete valid single rebind | Override commits atomically and prompt updates | Both | Not run |
| LAB-026 | Cancel single rebind | Exact prior override snapshot is restored | Both | Not run |
| LAB-027 | Let rebind timeout | Session ends and prior state restores using unscaled time | Both | Not run |
| LAB-028 | Submit excluded control | Candidate is rejected and session continues | Both | Not run |
| LAB-029 | Submit reserved control | Rebind does not commit; actionable conflict appears | Both | Not run |
| LAB-030 | Create ordinary conflict | Default Reject policy preserves both prior bindings | Both | Not run |
| LAB-031 | Choose explicit Swap | Both affected bindings update in one transaction | Both | Not run |
| LAB-032 | Cancel during conflict resolution | Entire transaction rolls back | Both | Not run |
| LAB-033 | Rebind multi-part composite | All required parts commit together | Both | Not run |
| LAB-034 | Fail/cancel composite midway | Every part returns to prior state | Both | Not run |
| LAB-035 | Export then reset/import overrides | Stable document restores committed bindings | Both | Not run |
| LAB-036 | Import document with orphan entry | Valid entries apply; orphan is preserved/reported | Both | Not run |
| LAB-037 | Resolve prompts without glyph art | Human-readable text fallback appears | Both | Not run |
| LAB-038 | Reset laboratory repeatedly | No duplicated roots, leases, callbacks, users, or source mutations | Both | Not run |

### 13.4 Optional integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| The Will + The Accord | EchoInput, EchoSettings | Persist override document and input preferences | Requires preference authority |
| The Will + The Pulse | EchoInput, EchoGameState | Translate effective state intents into contexts/locks | Tests bridge policy |
| The Will + The Looking Glass | EchoInput, EchoUI | Production-style rebind and controls screens | Requires UI authority |
| The Will + Resonance | EchoInput, Jukebot | Request optional rebind/navigation feedback | Requires audio authority |
| The Will + EchoCharacters | EchoInput, EchoCharacters | Assign local player/device to character ownership | Requires roster authority |
| The Will + EchoControllers | EchoInput, EchoControllers | Translate actions into normalized movement intent | Requires controller implementation |
| Local multiplayer exploration | EchoInput, EchoCharacters, future EchoMultiplayer | Demonstrate multiple local users when later approved | Beyond MVP single-user contract |

Samples are imported separately and removable without affecting the core package.

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

EchoInput is nonvisual at its core. It owns data and workflow state required to present controls and rebinding, but it does not own production screens, navigation layout, animation, theme, modal presentation, or persistent UI hierarchy.

Presentation options:

- Sample-only uGUI/TextMeshPro views in `Samples~`.
- Project-authored UI consuming public services.
- A later EchoUI bridge and templates.
- Editor/runtime diagnostic monitor for development.

### 14.2 Required presentation states

Any rebind or control-display presenter must be able to represent:

- Ready.
- Waiting for input.
- Candidate detected.
- Candidate invalid/excluded.
- Conflict detected.
- Resolution required.
- Committing.
- Completed.
- Canceled.
- Timed out.
- Device unavailable.
- Scheme unavailable.
- Binding unassigned.
- Prompt glyph available.
- Prompt text fallback.
- Warning.
- Failure.
- Read-only/locked binding.
- Composite part progress.

### 14.3 Accessibility requirements

The package design must support:

- Complete keyboard and controller operation for any shipped sample UI.
- A defined cancel path during rebinding.
- User-adjustable rebind timeout or a no-timeout policy where the project chooses.
- Text labels alongside or instead of glyphs.
- Color-independent conflict, success, and failure indicators.
- Scalable text through the presentation layer.
- No mandatory animation or time-sensitive visual-only feedback.
- Hold/toggle preference metadata and project integration seams.
- Dead-zone and sensitivity configuration seams without silently changing gameplay processors.
- Rebind exclusions for accessibility-reserved controls.
- Multiple bindings for one action where the project permits them.
- Clear announcement-ready state descriptions for future assistive-label adapters.
- Input prompts that can be localized by project/UI integrations.
- A path for alternative devices/layouts through the underlying Input System and project-authored policies.

The core does not claim full screen-reader support by itself because it owns no production visual tree.

### 14.4 Visual customization

All glyph art, fonts, labels, panel visuals, animations, and layout are project-owned or sample-only. Replacing them never requires editing runtime code.

Glyph mapping supports:

1. Exact control path.
2. Device layout.
3. Device family.
4. Generic control category.
5. Project/localized text fallback.

The core package ships no platform-holder logos or branded controller-face artwork unless redistribution rights and notices are explicitly documented.

### 14.5 UI event-system boundary

The package does not create or manage an EventSystem. When a project uses Unity UI with the Input System, the project or EchoUI configures `InputSystemUIInputModule`. EchoInput may provide validation/adapters for shared action references, but it does not become the UI navigation authority.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| Initialization state/report | API, inspector, log | All builds with safe detail | Negligible |
| Authority identity and duplicate count | API, inspector | Development; safe summary in release | Negligible |
| Source asset identity/fingerprint | API/report | Development/support | Low |
| Active primary/effective contexts | API/monitor | Development; optional safe release | Low |
| Active lock summary | API/monitor | Development; reason-redacted release | Low |
| Effective map/action state | API/monitor | Development | On demand |
| Active device family/scheme | API/events | All builds | Low |
| Device availability/pairing summary | API/monitor | Development/support | Low |
| Active rebind state | API/events | All builds as workflow state | Low |
| Conflict/import reports | Result objects | All builds | Operation-scoped |
| Override document health | API/report | Development/support | On demand |
| Prompt-resolution source | API/monitor | Development | On demand |
| Event/listener count | Monitor | Development | Low |
| Bounded semantic event history | Snapshot/report | Development/support | Configurable |
| Raw input event history | Not provided | Never | Not applicable |

### 15.2 Structured status

`EchoInputSnapshot` includes, subject to privacy/redaction mode:

- Package version and initialization state.
- Root instance identity and creation mode.
- Source action identity/fingerprint.
- Runtime ownership mode.
- Number of action maps/actions/bindings.
- Primary and effective context IDs.
- Active override and lock counts.
- Effective enabled-map count.
- Active device family/layout category and scheme.
- Number of paired/available eligible devices.
- Rebind session phase without entered raw values.
- Override revision and applied/orphan counts.
- Prompt-library availability.
- Recent diagnostic codes and counters.
- Last semantic transition timestamps from the injected unscaled clock.

It excludes key sequences, text input, full device serials, account IDs, and continuous axis histories.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| EIN-001 | Info | Authority initialized | None |
| EIN-002 | Warning | Duplicate authority rejected | Remove duplicate scene/prefab root |
| EIN-003 | Blocker | Configuration missing | Assign/create configuration |
| EIN-004 | Blocker | Source action asset missing | Assign project asset |
| EIN-005 | Error | Source identity/fingerprint invalid | Validate/migrate asset |
| EIN-006 | Error | Runtime action collection creation failed | Review dependency/asset |
| EIN-007 | Warning | External managed-map drift detected | Remove competing enablement owner |
| EIN-101 | Error | Context ID invalid | Fix request/catalog |
| EIN-102 | Error | Context directive target missing | Repair catalog |
| EIN-103 | Warning | Context lease leaked/owner unavailable | Dispose lease |
| EIN-104 | Warning | Context dominance tie resolved by fallback order | Set explicit order |
| EIN-111 | Error | Lock target invalid | Fix policy/request |
| EIN-112 | Warning | Lock lease leaked | Dispose lease |
| EIN-113 | Blocker | All cancel/emergency paths locked | Repair policy |
| EIN-201 | Warning | Rebind already active | Finish/cancel prior session |
| EIN-202 | Info | Rebind candidate rejected | Enter an allowed control |
| EIN-203 | Info | Rebind canceled | None |
| EIN-204 | Warning | Rebind timed out | Retry/adjust timeout |
| EIN-205 | Warning | Binding conflict rejected | Choose another control or explicit resolution |
| EIN-206 | Error | Composite rebind rolled back | Retry and inspect report |
| EIN-207 | Error | Rebind rollback failed | Reset/import known-good overrides |
| EIN-301 | Error | Override document corrupt | Preserve file and restore backup/defaults |
| EIN-302 | Error | Override document version newer than supported | Upgrade package or use compatible data |
| EIN-303 | Warning | Source identity mismatch | Run migration/compatibility review |
| EIN-304 | Warning | Override entry orphaned | Map ID or preserve until feature returns |
| EIN-305 | Warning | Override entry conflict/invalid | Review import report |
| EIN-401 | Warning | Glyph unavailable | Add library entry or text fallback |
| EIN-402 | Warning | Control display action unavailable | Repair display definition |
| EIN-501 | Warning | Active device lost | Reconnect/select fallback |
| EIN-502 | Warning | No eligible device available | Connect supported device |
| EIN-503 | Warning | Pairing request rejected | Review user/device policy |
| EIN-601 | Warning | Input System/UI action ownership conflict detected | Align project adapters |
| EIN-901 | Warning | Diagnostic listener failed | Fix listener; core continued |
| EIN-999 | Error | Unexpected internal failure | Export redacted snapshot and report |

### 15.4 Observatory bridge

A separate bridge maps `EchoInputSnapshot`, structured events, and validation results into The Observatory panels. It may expose:

- Authority health.
- Context and lock state.
- Active device/scheme.
- Rebind workflow state.
- Override health/migration counts.
- Managed-map drift.
- Bounded semantic event history.

The bridge must not subscribe to or export raw input events. EchoInput compiles and functions when The Observatory is absent.

### 15.5 Logging policy

- Stable `EIN-*` codes prefix actionable messages.
- Normal action value changes do not log.
- Device jitter and candidate scanning do not spam the Console.
- Raw keys, typed text, passwords, and full control sequences are not logged.
- Development verbosity is configurable.
- Release logs redact project paths, user/profile identifiers, and device-specific identifiers.
- Repeated equivalent warnings are rate-limited or aggregated.
- Diagnostic listeners are isolated so their exceptions do not interrupt input.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Source action definitions | Project | Project/Input System | Yes as asset | Unity asset |
| Context/device/rebind policies | Project | Project/EchoInput definitions | Yes as assets | Unity asset |
| Current primary/effective context | Session | EchoInput | No by default | None |
| Active context/lock leases | Session | EchoInput | Never | None |
| Active device/scheme | Session/global preference candidate | EchoInput runtime; project decides persistence | Optional | Accord/project |
| Binding overrides | Global/profile preference | EchoInput model | Yes when project chooses | Accord/project adapter |
| Dead-zone/sensitivity preferences | Global/profile preference | Accord/project; applied through adapter | Optional | Accord/project |
| Glyph/control-display definitions | Project content | Project | Yes as assets | Unity asset |
| Rebind session/candidate | Operation | EchoInput | Never | None |
| Diagnostics history | Session | EchoInput | Export only by explicit action | Support snapshot |

### 16.2 Standalone behavior

Without The Accord or EchoSave:

- EchoInput works fully for the current application session.
- Runtime rebinding commits to the in-memory override model.
- The project can export/import `BindingOverrideDocument` through the public API.
- The package does not silently choose a filename, `PlayerPrefs`, or account/profile boundary.
- Sample tooling may offer explicit local import/export for laboratory testing only, clearly labeled as sample support.

### 16.3 The Accord bridge

The Accord remains the authority for durable global preferences. The bridge:

1. Registers a versioned input-preference section/applier.
2. Supplies an override document and selected global input preferences during initialization.
3. Asks EchoInput to validate/migrate/apply them.
4. Exports the current committed document when overrides change.
5. Preserves unknown/orphan data when EchoInput or an optional map is absent.
6. Reports failures transactionally so The Accord can retain prior committed data.
7. Does not allow The Accord to mutate active rebind sessions directly.

Whether bindings are global, per local profile, or per platform account is a project decision recorded in the integration specification.

### 16.4 EchoSave boundary

Input bindings are normally global/profile preferences, not game-save-slot progress. EchoSave should not store them unless a specific game deliberately defines per-save controls and records that unusual policy.

No Chronicle dependency belongs in EchoInput core.

### 16.5 Failure and recovery

- Missing document: use source/default bindings and report no saved overrides.
- Empty document: valid “no overrides.”
- Corrupt document: reject atomically; preserve current working bindings.
- Older supported schema: migrate in memory and return a report; persistence owner decides when to commit upgraded data.
- Newer unsupported schema: reject without rewriting.
- Source mismatch: analyze and apply only under explicit compatible/partial policy.
- Orphan entries: preserve and report.
- Invalid control path: skip/reject per import policy; never crash initialization.
- Interrupted persistence: handled by The Accord/project backend, not EchoInput.
- Failed post-import apply: restore the exact previous runtime override snapshot.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Optional peers exchange semantic intent and structured results. No installed package silently changes EchoInput’s configuration or action maps. Bridges are explicit, removable, and versioned.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| First Light | Startup-step bridge | Separate/tiny owner-approved bridge | Launch -> Input | Initialize, load supplied override data, health/result | No |
| The Observatory | Diagnostics provider bridge | Separate bridge | Input -> Diagnostics | Snapshot, events, validation health | No |
| The Accord | Settings participant/applier bridge | Separate bridge | Bidirectional | Override document, device/preferences, apply/import results | No |
| The Passage | Transition context/lock project bridge | Project/separate bridge | Scene Flow -> Input | Acquire/release transition lock/context | No |
| The Pulse | State-to-input policy bridge | Separate bridge | Game State -> Input | Effective state/context/lock intents | No |
| Resonance | Feedback bridge/project presenter | Project/UI bridge | Input workflow -> Audio | Semantic navigation/rebind result cue requests | No |
| The Looking Glass | UI presenter/adapter bridge | Separate bridge | Bidirectional | Rebind workflow, prompts, control pages, navigation context | No |
| The Workshop | Editor composer | Workshop | Editor -> Project | Select package, create config/root/sample, report | No |
| EchoCharacters | Player/device ownership bridge | Separate bridge | Bidirectional | Local player/user assignment, possession ownership | No |
| EchoControllers | Input adapter | Controllers/project | Input -> Controller | Native actions to normalized movement/action intent | No |
| EchoMultiplayer | Local/remote ownership adapter | Future separate provider bridge | Bidirectional | Local user identity and authority boundaries | No |
| EchoLocalization | Prompt-label adapter | Separate/project | Localization -> UI prompt data | Localized action/control labels | No |

### 17.3 Bridge placement decisions

- First Light and Observatory connections should be separate compile-safe bridge assemblies/packages when direct references would otherwise create a hard peer dependency.
- The Accord, Pulse, and EchoUI bridges directly depend on both sides and therefore normally ship separately.
- EchoControllers owns or demonstrates the adapter from native action values into controller intent because it owns the receiving movement contract.
- Game-specific meaning, map choices, and state mapping remain project adapters.
- Vendor/platform input providers ship separately if they introduce SDK dependencies or licensing.

### 17.4 Pulse integration behavior

The Pulse bridge maps effective game-state policy into EchoInput requests without taking ownership of either package.

Example:

```text
Pulse effective state: Paused
    -> bridge acquires EchoInput "Menu" override
    -> bridge acquires gameplay-map lock
    -> UI actions remain enabled by policy
    -> Pulse changes again
    -> bridge releases/replaces only its own leases
```

The bridge must:

- Use leases rather than direct map enable/disable calls.
- Release its leases on disable/shutdown.
- Rebuild from current Pulse state after initialization order changes.
- Avoid feedback loops where input context changes game state automatically.
- Leave project-defined exceptional actions configurable.

### 17.5 UI integration behavior

The EchoUI bridge supplies views/presenters with:

- Control-display pages.
- Resolved prompts.
- Rebind session state.
- Candidate/conflict reports.
- Commit/cancel commands.
- Device/scheme changes.
- Navigation-context lease helpers.

EchoUI remains responsible for focus, modal stack, visual navigation, confirmations, and accessibility presentation. EchoInput remains responsible for whether the rebind transaction can commit.

### 17.6 Integration failure behavior

| Failure | Required behavior |
|---|---|
| Peer absent | Core continues with standalone behavior |
| Peer disabled after initialization | Bridge releases its leases/subscriptions and reports unavailable |
| Bridge version mismatch | Bridge disables itself with actionable diagnostic; cores continue |
| Initialization order reversed | Bridge waits/registers through explicit lifecycle or rebuilds from current snapshot |
| Settings import fails | Previous bindings remain; The Accord receives failure result |
| UI closes during rebind | Presenter requests cancel or rebind continues only under explicitly documented policy |
| Pulse disappears with active leases | Bridge releases only its leases |
| Scene transition destroys presenter | Core rebind session follows configured cancellation policy; no orphaned UI dependency |
| Device/user ownership changes | Bridge submits explicit reassignment; no silent takeover |
| Shutdown | Bridges unsubscribe and release leases before their authority disappears |

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Idle package update cost | No per-frame full asset/map/binding scan | Input Laboratory + Unity Profiler | No avoidable recurring scan |
| Context/lock recomputation | Proportional to managed maps/actions and only after semantic change | Automated stress test | Within one frame for advertised limits |
| Device-change reaction | Event-driven and bounded | Device simulation + Profiler | No recurring device enumeration in normal path |
| Meaningful-device filtering | No managed allocation per accepted event after warmup | Input Laboratory | Zero avoidable GC in steady state |
| Prompt lookup | Cached/bounded and no asset-wide scan per UI frame | Prompt stress panel | Stable at advertised page size |
| Rebind candidate processing | Input System operation plus bounded policy checks | Rebind stress test | Responsive without frame hitch at advertised binding count |
| Override import/export | Linear in document entries and explicitly invoked | 1,000-entry synthetic test | Completes without freezing ordinary supported project sizes |
| Diagnostic history | Fixed configurable capacity | Long session test | Capacity remains bounded |
| Lease operations | Bounded and allocation-conscious | Context/lock stress test | No unbounded growth after release |
| Initialization | One runtime clone plus validation, no hidden scene search loop | Clean Boot test | Actionable report; no duplicate side effects |

Initial numerical budgets are benchmarked during M2/M3 and promoted into this table before beta. The specification refuses to invent unsupported millisecond guarantees before implementation profiling exists.

### 18.2 Allocation policy

- Subscribe to Input System/device changes once and unsubscribe deterministically.
- Avoid LINQ in input-event, context-resolution, prompt-resolution, and rebind hot paths.
- Cache action/map/binding GUID lookups after initialization.
- Use bounded dictionaries/lists/pools for leases, prompt results, and diagnostics.
- Do not allocate a new full snapshot every frame.
- Generate immutable snapshots on request or low-frequency diagnostic cadence.
- Do not retain `InputEventPtr`, raw event buffers, or control-value history.
- Rebind operations are operation-scoped and disposed after completion.
- Glyph providers return references/keys, not duplicated textures/sprites.
- Reflection is not used for ordinary action discovery or peer integration.
- String formatting for diagnostics is deferred until presentation/export where practical.

### 18.3 Advertised initial limits

| Resource | Default | Tested target before beta | Behavior at limit |
|---|---:|---:|---|
| Managed action maps | Project-defined | 64 | Validation warning beyond tested range |
| Managed actions | Project-defined | 512 | Continue with warning/profiling guidance |
| Managed bindings | Project-defined | 2,048 | Continue with warning; import/rebind remains bounded |
| Active override-context leases | 32 | 128 | Reject new lease with diagnostic at configured cap |
| Active lock leases | 64 | 256 | Reject new lease with diagnostic at configured cap |
| Input users in MVP | 1 | 1 authoritative + device pool | Additional users require later/local-multiplayer expansion |
| Paired devices for primary user | Policy-defined | 8 | Reject/ignore beyond configured cap |
| Concurrent rebind sessions | 1 per user, 1 total in MVP | 1 | Return Busy |
| Override entries | Project-defined | 2,048 | Import report warns beyond tested range |
| Prompt-cache entries | 256 | 1,024 | Deterministic eviction |
| Diagnostic events | 128 | 1,024 configurable | Oldest evicted |

Limits are configuration and validation contracts, not reasons to crash.

### 18.4 Input backend and update mode

EchoInput works with the supported Input System update modes selected by the project. It does not change project-wide update mode silently.

The package must document and test:

- Dynamic update.
- Fixed update where meaningful.
- Manual update as advanced/project-controlled and not the default.
- Editor vs Player behavior.
- Focus/background behavior.
- UI module update interaction.

Consumers remain responsible for reading action values in an appropriate gameplay loop. EchoInput’s semantic state changes occur on the Unity main thread.

### 18.5 Scene and domain reload behavior

- Static convenience state resets through subsystem registration and root lifecycle hooks.
- Duplicate detection remains correct with supported Enter Play Mode options.
- Input System callbacks unsubscribe on shutdown.
- Runtime-cloned action collections are disabled/disposed according to supported Unity APIs.
- Active rebind operations cancel and dispose.
- Input users/devices are unpaired only when owned by this root.
- Context and lock leases are invalidated without invoking destroyed scene owners.
- Prompt caches and histories clear.
- Source ScriptableObjects remain unmodified.
- Direct-scene helpers identify and clean up only roots they created.

### 18.6 Graceful degradation

- Missing glyph art becomes text fallback.
- Unsupported device layout becomes generic layout/family or unavailable prompt.
- No eligible device becomes explicit unavailable state; the package does not spin.
- Rebind unsupported for a binding/control becomes a rejected result.
- Large catalogs remain usable with warnings and on-demand diagnostics.
- Failed optional bridge leaves standalone input operational.
- Unsupported platform behavior is declared rather than guessed.
- Diagnostics may reduce sampling/detail without changing input behavior.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Input infrastructure can accidentally reveal sensitive behavior. EchoInput therefore treats the following as sensitive or unnecessary:

- Typed text and character sequences.
- Password/PIN entry.
- Raw key/button timelines.
- Full device serial numbers or hardware account identifiers.
- Platform-user/account tokens.
- OS-level accessibility/device details not needed for function.
- Absolute project/user filesystem paths in release reports.

The package stores semantic binding overrides and safe device family/layout categories, not behavioral surveillance data.

### 19.2 Trust boundaries

| Input/source | Validation |
|---|---|
| Project action asset | Stable IDs, expected types, maps, schemes, binding structure |
| Binding override document | Schema, source identity, path syntax, limits, conflicts, reserved controls |
| Interactive candidate | Allowed layouts/paths, expected control type, exclusions, reservation, conflict policy |
| Custom conflict resolver | Allowed command set; no direct mutation authority |
| Glyph provider | Null/missing/invalid reference fallback |
| Device/provider callbacks | Eligibility, pairing, noise/synthetic filtering |
| Project adapter requests | Valid stable IDs, ownership, capacity, lifecycle |
| Imported Unity JSON | Conversion into validated package document; source retained |
| Diagnostic export | Redaction and explicit user/developer action |

Malformed data must produce a structured failure, never arbitrary code execution or silent destructive rewrite.

### 19.3 Platform behavior

| Platform | Supported? | Special behavior | Validation required |
|---|---:|---|---|
| Windows | Yes | Keyboard/mouse and common gamepads; cursor/focus handled by project peers | Clean Player/device tests |
| macOS | Yes | Layout/device names and permissions may differ | Clean Player/device tests |
| Linux | Yes where Unity/Input System supports target devices | Controller mappings vary | Clean Player/device tests |
| WebGL | Yes with limitations | Browser focus, gamepad availability, and persistence/pointer behavior vary | Browser-specific manual tests |
| Android/iOS | Planned/supported where Input System controls apply | Touch, sensors, soft keyboard, lifecycle, and controller behavior vary | Device tests before claim |
| Consoles | Planned/unknown until licensed provider testing | Platform SDK, user pairing, glyph/legal rules | Provider/platform approval |
| XR | Not MVP | Requires dedicated action/device/presentation design | Later specification |
| Cloud/remote streaming | Not guaranteed | Device identity and latency may be virtualized | Provider testing |

The package documentation distinguishes “Input System supports a device class” from “this package release has been validated on a particular model/platform.”

### 19.4 Focus and background behavior

Focus-loss policy is project-owned or handled through a dedicated bridge with The Pulse/UI. EchoInput reports device/focus-relevant availability but does not decide to pause the game or open a menu.

Rebind sessions default to cancel or suspend safely on focus loss according to configured policy. No candidate commits while the application cannot reliably confirm intended input.

### 19.5 Device identification and glyph licensing

- Runtime logic may use Input System layouts/usages/control paths.
- User-visible device names are sanitized/project-controlled.
- Full serial numbers are never required for ordinary pairing.
- Branded button glyph packs are not bundled without redistribution rights.
- Platform certification terminology and legal marks remain project/platform adapter concerns.
- Generic text labels ensure functionality without proprietary art.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-input/
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
│   │   ├── Contexts and Locks.md
│   │   ├── Rebinding.md
│   │   ├── Binding Persistence.md
│   │   ├── Prompts and Glyphs.md
│   │   ├── Input Laboratory.md
│   │   └── Troubleshooting.md
│   └── Developer/
│       ├── Architecture.md
│       ├── Lifecycle.md
│       ├── Public API.md
│       ├── Data and Migration.md
│       ├── Integration Index.md
│       ├── Testing.md
│       ├── Release.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Runtime/
│   ├── Core/
│   ├── Configuration/
│   ├── Contexts/
│   ├── Locks/
│   ├── Devices/
│   ├── Rebinding/
│   ├── Overrides/
│   ├── Prompts/
│   ├── Diagnostics/
│   ├── Development/
│   ├── Prefabs/
│   └── EchoDevGames.EchoInput.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Inspectors/
│   ├── Migration/
│   ├── Monitoring/
│   └── EchoDevGames.EchoInput.Editor.asmdef
├── Samples~/
│   └── Standalone Labs/
│       └── The Will Input Laboratory/
├── Tests/
│   ├── Editor/
│   │   └── EchoDevGames.EchoInput.Tests.Editor.asmdef
│   └── Runtime/
│       └── EchoDevGames.EchoInput.Tests.Runtime.asmdef
└── .meta files preserved
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoInputRoot.cs
│   ├── EchoInputRuntime.cs
│   ├── IEchoInputService.cs
│   ├── EchoInputInitializationState.cs
│   └── EchoInputResult.cs
├── Configuration/
│   ├── EchoInputConfiguration.cs
│   ├── InputDevicePolicy.cs
│   ├── InputRebindPolicy.cs
│   └── InputRuntimeOwnershipMode.cs
├── Contexts/
│   ├── InputContextCatalog.cs
│   ├── InputContextDefinition.cs
│   ├── InputMapDirective.cs
│   ├── InputContextService.cs
│   ├── InputContextLease.cs
│   └── EffectiveInputContext.cs
├── Locks/
│   ├── InputLockService.cs
│   ├── InputLockRequest.cs
│   ├── InputLockLease.cs
│   └── EffectiveInputLockState.cs
├── Devices/
│   ├── InputDeviceService.cs
│   ├── InputDeviceActivityFilter.cs
│   ├── InputUserAdapter.cs
│   ├── InputDeviceSummary.cs
│   └── InputPairingResult.cs
├── Rebinding/
│   ├── InputRebindService.cs
│   ├── InputRebindRequest.cs
│   ├── InputRebindSession.cs
│   ├── InputRebindResult.cs
│   ├── InputConflictAnalyzer.cs
│   ├── InputConflictReport.cs
│   └── IInputConflictResolver.cs
├── Overrides/
│   ├── BindingOverrideService.cs
│   ├── BindingOverrideDocument.cs
│   ├── BindingOverrideEntry.cs
│   ├── BindingOverrideImportReport.cs
│   ├── BindingOverrideMigrationMap.cs
│   └── InputSourceFingerprint.cs
├── Prompts/
│   ├── InputBindingMetadataCatalog.cs
│   ├── InputGlyphLibrary.cs
│   ├── ControlDisplayDefinition.cs
│   ├── InputPromptService.cs
│   ├── InputPromptRequest.cs
│   └── InputPromptResult.cs
├── Diagnostics/
│   ├── EchoInputSnapshot.cs
│   ├── EchoInputDiagnosticEvent.cs
│   └── EchoInputDiagnosticCodes.cs
└── Development/
    └── EchoInputDirectSceneInitializer.cs
```

This is a proposed implementation map, not permission to begin coding before the Foundation gate opens.

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoInput.Runtime` | Runtime | UnityEngine, Unity Input System | Yes | Standalone package runtime |
| `EchoDevGames.EchoInput.Editor` | Editor | Runtime, UnityEditor, Input System Editor APIs where supported | No/Editor | Setup, validation, inspectors, migration |
| `EchoDevGames.EchoInput.Tests.Runtime` | Test | Runtime, Unity Test Framework, Input System test support when redistributable/supported | No | PlayMode/runtime tests |
| `EchoDevGames.EchoInput.Tests.Editor` | Editor test | Runtime, Editor, Unity Test Framework | No | Validation/migration tests |
| `EchoDevGames.EchoInput.Samples.InputLab` | Sample | Runtime, Input System, optional uGUI/TMP declared by sample | No | Standalone laboratory |

No core assembly references another Echo package.

### 20.4 Repository files

The package repository must include:

- Concise routed README.
- Full documentation in `Documentation~`.
- Package specification and relevant ADRs/checkpoints in repository planning documentation.
- Linked `Current Notes.md`.
- Changelog.
- License and third-party notices.
- Input System dependency/version statement.
- Glyph/art licensing policy.
- Contribution/security/support guidance.
- Release checklist.
- Stable `.meta` files and GUIDs.
- Sample licenses/notices.
- Compatibility and migration reference.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Primary tested baseline | Notes |
|---|---|---|---|
| Unity | 6000.0 | 6000.3.8f1 | Exact minor/editor matrix revalidated before release |
| Unity Input System | 1.17.0 planned floor | Version compatible with primary Unity baseline | Manifest and APIs reverified at M1/release |
| Unity Test Framework | Baseline-compatible | Project baseline | Development/test only |
| uGUI/TextMeshPro | Sample-only where used | Baseline-compatible | Not a runtime-core dependency |

The currently approved planning floor uses the Unity 6 released Input System line. Package availability and exact versions must be verified against official Unity package documentation when implementation and distribution begin.

### 21.2 Technical basis

The specification deliberately builds on supported Input System concepts:

- Action assets, action maps, actions, bindings, and control schemes.
- Runtime enable/disable behavior.
- `InputUser` device pairing and control-scheme association.
- Interactive rebinding operations.
- Binding override application and serialization interoperability.
- Device change notifications.
- Built-in interactions such as hold, tap, and multi-tap where they meet the requirement.
- Built-in processors such as dead zones where they meet the requirement.
- Optional `InputSystemUIInputModule` configured by UI/project integration.

EchoInput adds ownership, deterministic context/lock policy, transactional workflow, migration-aware persistence, diagnostics, and package boundaries. It does not fork or replace Unity’s input backend.

### 21.3 Semantic versioning policy

**Patch** may include:

- Internal fixes with unchanged public contracts.
- Additional diagnostics/validation.
- Sample/documentation fixes.
- New nonbreaking device layout aliases or glyph fallback metadata.
- Performance improvements preserving behavior.

**Minor** may include:

- New optional contexts/policies/providers.
- New nonbreaking public API.
- New import/migration support.
- New sample or bridge compatibility.
- Additional conflict strategy that does not change defaults.
- Expanded platform/device support.

**Major** includes:

- Breaking public API or assembly changes.
- Changed default context/lock/rebind semantics.
- Breaking override-document schema without automatic migration.
- Changed stable-ID authority.
- Changed runtime action ownership default.
- Changed dependency floor that excludes previously supported Unity projects.
- Removed public types/events/providers.

### 21.4 Deprecation policy

- Mark deprecated APIs with compiler guidance and documentation.
- Keep a practical transition period across at least one minor release when feasible.
- Provide replacement and migration examples.
- Never silently reinterpret persisted binding entries under an old schema.
- Record asset/source-tree migrations and preserve GUIDs when identity survives.
- Remove only in a documented major release unless security/platform necessity requires faster action.

### 21.5 GUID and asset compatibility

Public scripts, configuration templates, prefabs, sample definitions, and independently creatable ScriptableObject types retain committed `.meta` files. Moves and renames preserve GUIDs.

Released project-facing stable IDs receive:

- Duplicate validation.
- Alias/migration support.
- Changelog entries when replaced.
- Tests proving old override documents migrate or remain safely orphaned.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview and authority boundary.
- Supported Unity/Input System versions.
- Installation routes.
- Five-minute quick start.
- Root/configuration setup.
- Runtime action-ownership modes.
- Contexts and leased overrides.
- Locks and emergency exemptions.
- Device/scheme detection and meaningful-activity rules.
- Pairing policy and single-user MVP limit.
- Rebinding workflow.
- Conflict policies and destructive-confirmation rules.
- Composite rebinding.
- Override export/import/persistence.
- Glyph libraries and text fallback.
- Controls-display authoring.
- Input Laboratory guide.
- Direct-scene development setup.
- Troubleshooting and diagnostic-code reference.
- Migration/upgrade guide.
- Known limitations.
- Bridge/integration index.
- License, credits, and third-party notices.

### 22.2 Required developer documentation

- Architecture and ownership model.
- Initialization and shutdown sequence.
- Runtime clone versus injected mode.
- Stable identifiers and source fingerprint.
- Context dominance and directive algorithm.
- Lock resolution order.
- Device activity filter and pairing lifecycle.
- Rebind transaction state machine.
- Conflict analysis model.
- Binding document schema and migration.
- Prompt resolution fallback chain.
- Public APIs, events, results, and leases.
- Test injection seams.
- Performance/resource limits.
- Privacy/redaction model.
- Testing strategy and fixtures.
- Release workflow.
- ADRs, checkpoint records, and linked Current Notes.

### 22.3 Documentation truth rule

Examples must compile against the documented release and use the documented Input System version. Screenshots/menu paths must match the current supported Unity editor. Generated action-wrapper examples must be clearly project-local rather than package-core dependencies.

A feature is not release-ready when the documentation:

- Uses action names where stable IDs are required.
- Suggests direct source-asset runtime mutation.
- Claims mid-operation persistence without rollback.
- Omits destructive conflict consequences.
- Claims platform/device support not tested.
- Shows proprietary glyphs without licensing.
- Requires a peer package while calling the core standalone.

### 22.4 Living repository and Obsidian workflow

The specification, ADRs, test evidence, checkpoint plans, issue records, and Current Notes live in Git beside the package work. Obsidian opens the same files directly.

At each checkpoint:

1. Reconcile new Current Notes.
2. Promote durable behavior/API/data decisions into this specification or an ADR.
3. Move defects and evidence into issue/test records.
4. Update setup/API/migration/troubleshooting documentation.
5. Update changelog and current status.
6. Confirm docs match committed implementation.
7. Commit documentation with or immediately adjacent to code.
8. Push the checkpoint before beginning the next one when practical.

### 22.5 Repository scan and handoff order

1. Repository README/documentation index.
2. SFGSS-000.
3. This approved package specification.
4. Applicable ADRs and bridge specifications.
5. `Current Notes.md`.
6. Current checkpoint, test report, issue log, and changelog.
7. Relevant runtime/editor code and tests.
8. Current Input System compatibility notes.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, context/lock resolution, conflict rules, document migration, validation | Deterministic priority, orphan preservation, alias cycles | Yes |
| PlayMode unit/integration | Root lifecycle, runtime clone, devices, rebind operation, events | Duplicate root, device loss, rollback, shutdown | Yes |
| Standalone Test Lab | User-visible isolated workflow | Contexts, locks, prompts, rebind, import/export | Yes |
| Bridge Integration Lab | Optional peer behavior | Accord persistence, Pulse policy, EchoUI screens | When bridge ships |
| Showcase | Combined application shell | Multi-package input experience | No |
| Clean-project install | Packaging and hard-dependency proof | Git/tarball/local install | Yes |
| Existing-project migration | Preserve working project controls | Rescuers2D/DGV adapter parity | Before adoption claim |
| Real-device matrix | Physical keyboard/mouse/gamepads and target platforms | Connect/disconnect/layout/focus | Yes for advertised support |

### 23.2 Required test categories

- Clean compile and package installation.
- Missing/invalid configuration.
- Runtime-clone source immutability.
- Duplicate roots before and after Ready.
- Domain reload and Enter Play Mode options.
- Context priority, acquisition order, and out-of-order release.
- Lock target resolution and emergency exemptions.
- External enablement drift.
- Meaningful-device filtering and jitter.
- Device add/remove/loss/reconnect.
- Primary InputUser pairing and rejection.
- Rebind happy path, cancellation, timeout, device loss, and shutdown.
- Composite atomicity.
- Conflict detection and every advertised resolution policy.
- Reserved and excluded controls.
- Override export/import/reset, migration, orphans, source mismatch, and corrupt/newer documents.
- Prompt resolution and fallback.
- Sample removal.
- Optional peer absent/present.
- Performance and configured capacity.
- Privacy/redaction.
- Build validation on supported platforms.

### 23.3 Test case registry

| Range | Coverage | Count |
|---|---|---:|
| EIN-T-001 to 008 | Installation, assembly isolation, configuration, source immutability | 8 |
| EIN-T-009 to 016 | Authority lifecycle, duplicates, reset, shutdown, domain reload | 8 |
| EIN-T-017 to 026 | Primary/override contexts, directives, ordering, leaked leases | 10 |
| EIN-T-027 to 036 | Action/map/global locks, exemptions, drift, capacity | 10 |
| EIN-T-037 to 044 | Meaningful-device detection, pairing, loss, reconnect | 8 |
| EIN-T-045 to 058 | Rebind, cancel, timeout, composite, conflicts, rollback | 14 |
| EIN-T-059 to 064 | Override document import/export/migration/orphans | 6 |
| EIN-T-065 to 068 | Glyph/prompt/control-display fallbacks | 4 |
| EIN-T-069 to 070 | Privacy snapshot and sustained performance/capacity | 2 |
| **Total** |  | **70** |

Every test receives setup, action, expected result, automation status, evidence, and release association in the implementation test registry.

### 23.4 Real hardware and simulation

Input System test fixtures may simulate many controls, but release claims require physical-device confirmation for each advertised family. The matrix records:

- Device model/family without publishing personal serial data.
- OS/platform and Unity build.
- Wired/wireless connection where relevant.
- Connect/disconnect/reconnect.
- Prompt/layout mapping.
- Composite and ordinary rebind.
- Focus/background behavior.
- Known platform limitations.

Simulation evidence supplements rather than replaces physical verification.

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and exclusions align with SFGSS-000.
- [x] Independence and hard dependency are explicit.
- [x] MVP and deferred scope are separated.
- [x] Root lifecycle and duplicate behavior are defined.
- [x] Context, lock, device, rebind, override, and prompt models are defined.
- [x] Standalone Laboratory is designed.
- [x] Release-blocking design questions are resolved.
- [x] Jesse authorized durable long-term design choices.
- [x] Package specification approved for future implementation.
- [ ] Foundation documentation gate opened for code.

### 24.2 Implementation gate

- [ ] Runtime package compiles against declared Unity/Input System versions only.
- [ ] Runtime code contains no Editor/sample/peer-package references.
- [ ] Source action asset remains unchanged by runtime tests.
- [ ] Duplicate rejection occurs before Input System side effects.
- [ ] Context and lock algorithms match approved contracts.
- [ ] Device filter/pairing behavior is deterministic and tested.
- [ ] Rebind transactions restore exact prior state on every failure path.
- [ ] Override document migration/orphan behavior matches the specification.
- [ ] Setup/repair repeats safely.
- [ ] Public API matches specification or approved ADR revision.

### 24.3 Standalone gate

- [ ] Clean-project install succeeds.
- [ ] Input System dependency resolves correctly.
- [ ] Package works without another Echo package.
- [ ] Input Laboratory passes all MVP scenarios.
- [ ] Sample removal leaves core operational.
- [ ] Direct-scene initializer behaves as documented.
- [ ] No hidden action/map names, EventSystem, or generated wrapper is required.

### 24.4 Quality gate

- [ ] All 70 registered tests pass or have approved platform exclusions.
- [ ] Physical-device matrix passes for advertised device families.
- [ ] No blocker/critical defect remains.
- [ ] Performance and resource limits are measured and documented.
- [ ] Diagnostics are actionable and privacy-safe.
- [ ] Migration from the previous supported version passes.
- [ ] User/developer documentation matches the build.
- [ ] Current Notes is reconciled.
- [ ] Licenses, glyph notices, and third-party notices are complete.

### 24.5 Distribution gate

- [ ] Manifest/version/dependencies are valid.
- [ ] Changelog is current.
- [ ] Stable `.meta` files are included.
- [ ] Git and tarball installations pass in another clean project.
- [ ] Upgrade/reinstall/removal pass.
- [ ] Repository tag/release is prepared.
- [ ] Compatibility catalog is updated.
- [ ] Documentation and current status are committed and pushed.
- [ ] No sample or bridge dependency leaked into runtime.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Rescuers2D | Generated `CharacterActions`, `PlayerInputReader`, direct role forwarding | Keep generated wrapper and gameplay reader; introduce EchoInput for contexts, locks, prompts, and persisted overrides through adapter | All character switching/actions behave identically; direct-scene tests pass | Disable adapter/remove EchoInput root; original asset/wrapper unchanged |
| Don’t Get Vince’d | Project Input System actions and player controller subscriptions | Add runtime ownership/context service around existing asset; adapt actions incrementally | Movement/combat/combo/pause parity and no duplicate subscriptions | Restore original scene/bootstrap and subscriptions |
| Echo Systems Lab | Central generated input wrapper/readers | Use as architecture proof for centralized translation while replacing global assumptions with context/lock services | Hub, terminal, trial, UI, and save flows retain behavior | Keep original reader/bootstrap until full parity |
| Hackulos | Planned top-down/WASD/click-to-move/action scheme | Adopt from clean start only after package release; project owns action meanings/controllers | Input Laboratory plus game-specific controller adapter tests | Use project-local Input System setup |
| Future Workshop preset | Generated starter input foundation | Workshop composes configuration/assets visibly | Clean generated project report and removal test | Remove generated pieces using report |

### 25.2 Preserve-until-parity rule

For existing games:

1. Record the current action asset, wrappers, maps, consumers, UI module, device behavior, and save format.
2. Install EchoInput without removing existing code.
3. Validate it alone in the Input Laboratory.
4. Use an adapter around one context or prompt feature first.
5. Confirm no duplicate action-asset runtime copy is being consumed accidentally.
6. Add rebind/persistence only after context parity.
7. Migrate one screen/controller flow at a time.
8. Keep original source asset and bindings under version control.
9. Remove old authority only after a parity report and rollback checkpoint.
10. Update project docs and Current Notes with every promoted decision.

### 25.3 Migration tooling

Planned tools:

- Existing action-asset analyzer.
- Map/action/binding GUID inventory.
- Generated-wrapper coexistence report.
- `PlayerInput`/UI module ownership detector.
- Direct `Enable`/`Disable` usage scanner for likely competing authority.
- Binding-override JSON converter.
- Source fingerprint comparator.
- Alias/migration-map authoring preview.
- Orphan/conflict report.
- Context-catalog starter generator.
- Controls-display starter generator.
- Backup/export before override transformation.
- Post-migration validation and rollback report.

Tooling never rewrites gameplay scripts automatically in v1.0.0. It identifies locations and generates explicit adapter/checkpoint work.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EIN-R-001 | Package becomes a universal gameplay controller | Medium | High | Enforce infrastructure-only boundary and native action consumption | Spec/API review |
| EIN-R-002 | Source action asset is mutated at runtime | Medium | High | Default owned runtime clone; immutability tests | Runtime tests |
| EIN-R-003 | Multiple action-asset copies create split state | High | High | Explicit ownership mode, diagnostics, adapter docs | Setup validation |
| EIN-R-004 | Duplicate root performs Input System side effects | Medium | High | Claim in `Awake` before clone/subscription/pairing | Lifecycle tests |
| EIN-R-005 | Context model grows into arbitrary game-state engine | Medium | High | Map directives only; Pulse/project owns state meaning | Boundary review |
| EIN-R-006 | Locks strand all input | Medium | High | Emergency exemptions, validation, lease diagnostics | Policy tests |
| EIN-R-007 | Leaked leases permanently disable actions | Medium | High | Idempotent handles, owner/reason diagnostics, caps | Stress tests |
| EIN-R-008 | External code fights map enablement | High | Medium | Managed-map ownership contract and drift detection | Integration audit |
| EIN-R-009 | Analog drift causes prompt flicker | High | Medium | Meaningful-activity thresholds and noisy/synthetic filtering | Device tests |
| EIN-R-010 | Pairing steals a device from another user | Medium | High | Conservative eligibility and explicit reassignment | Pairing tests |
| EIN-R-011 | Rebind leaves partial composite state | Medium | High | Snapshot and all-or-nothing commit/rollback | Rebind tests |
| EIN-R-012 | Destructive conflict resolution surprises users | Medium | High | Reject default and explicit confirmation | UI/contract tests |
| EIN-R-013 | Binding IDs change across project revisions | High | High | GUID identity, fingerprint, migration aliases, orphans | Migration tests |
| EIN-R-014 | Opaque Unity override JSON cannot be migrated clearly | Medium | Medium | Package-owned explicit document plus converter | Data tests |
| EIN-R-015 | Optional package removal deletes its bindings | Medium | Medium | Preserve orphan/unknown entries | Accord bridge tests |
| EIN-R-016 | Glyph assets create licensing problems | Medium | High | Generic/text fallback; project-owned branded art | Release review |
| EIN-R-017 | Core gains hard uGUI/EventSystem dependency | Medium | Medium | Sample/bridge-only UI | Assembly tests |
| EIN-R-018 | Diagnostics resemble keylogging | Low/Medium | Critical | No raw input/text history; redaction tests | Security review |
| EIN-R-019 | Unity/Input System API/version drift | Medium | High | Official version matrix and release revalidation | M1/release owner |
| EIN-R-020 | WebGL/mobile/console behavior is overclaimed | Medium | High | Platform-specific evidence before support claim | Release gate |
| EIN-R-021 | Input System interactions/processors are needlessly duplicated | Medium | Medium | Prefer built-ins; add helpers only after gap proof | API review |
| EIN-R-022 | Rebind UI closes without canceling operation | Medium | Medium | Session ownership/cancellation contract | EchoUI bridge tests |
| EIN-R-023 | Domain reload leaves subscriptions/users behind | Medium | High | Static reset and teardown tests | Lifecycle tests |
| EIN-R-024 | Large action catalogs cause initialization hitch | Medium | Medium | Cache once, profile, advertise limits | Performance tests |
| EIN-R-025 | Single-user MVP blocks later local multiplayer | Medium | Medium | InputUser-based seams and no static player assumptions | Architecture review |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EIN-D-001 | EchoInput owns input infrastructure, not gameplay action meaning | Approved | Preserves package neutrality | Controllers/project consume actions | No |
| EIN-D-002 | Unity Input System is a declared hard platform dependency | Approved | Central package purpose depends on its action/device/rebind model | Manifest/version matrix required | No |
| EIN-D-003 | One duplicate-safe application-session root owns runtime services | Approved | Clear authority and lifecycle | Duplicate claim precedes side effects | No |
| EIN-D-004 | Default mode clones the project action asset at runtime | Approved | Protects source asset and isolates mutable state | Consumers must use owned collection/adapter | Yes if changed |
| EIN-D-005 | Advanced injected action-collection mode is optional | Approved | Supports existing PlayerInput/wrapper projects | Lower isolation must be explicit | No |
| EIN-D-006 | Unity action/map/binding GUIDs are persistence authority | Approved | Names/indexes are unstable | Migration maps handle intentional replacement | Yes if changed |
| EIN-D-007 | One primary context plus leased override contexts | Approved | Preserves underlying mode and safe temporary layers | No destructive stack restoration | No |
| EIN-D-008 | Context directives are Enable, Disable, or Unchanged | Approved | Prevents every context from needing full map knowledge | Effective state is deterministic | No |
| EIN-D-009 | Dominance uses priority then acquisition order | Approved | Deterministic ties | Documented ordering tests | No |
| EIN-D-010 | Locks are additive, reason-based, and leased | Approved | Nested systems cannot overwrite one Boolean | Out-of-order release safe | No |
| EIN-D-011 | Lock resolution occurs after context resolution | Approved | Locks act as restrictive overlay | Emergency exemptions validated | No |
| EIN-D-012 | EchoInput owns enablement only for configured maps/actions | Approved | Avoids fighting unrelated project code | Drift is reported, not per-frame battled | No |
| EIN-D-013 | MVP supports one primary InputUser with conservative pairing | Approved | Completes single-player foundation without fake multiplayer scope | Multi-user expansion deferred | Yes before multiplayer expansion |
| EIN-D-014 | Active-device switching requires meaningful activity | Approved | Prevents analog drift/pointer jitter flicker | Threshold policy and tests required | No |
| EIN-D-015 | Device changes do not automatically change gameplay context | Approved | Device and game mode are separate truths | UI prompts may update independently | No |
| EIN-D-016 | Rebinding is transactional with exact snapshot rollback | Approved | Prevents partial/corrupt controls | Operation owns temporary lock/context | No |
| EIN-D-017 | Composite rebinding commits all parts atomically | Approved | Partial composites are unusable | Multi-step session required | No |
| EIN-D-018 | Default conflict policy is Reject | Approved | Safest and least surprising | Destructive alternatives need confirmation | No |
| EIN-D-019 | Conflict analysis includes context overlap and shareability metadata | Approved | Path equality alone creates false positives/negatives | Metadata catalog required | No |
| EIN-D-020 | Binding overrides use a versioned package-owned document | Approved | Enables migration, reports, orphans, and project metadata | Unity JSON is interoperability only | Yes if schema authority changes |
| EIN-D-021 | Orphan and unknown override entries are preserved | Approved | Optional removal/reinstall and migration safety | Persistence owner must retain extension data | No |
| EIN-D-022 | The Accord/project owns durable storage | Approved | Settings authority remains separate | Core has session import/export only | No |
| EIN-D-023 | Glyph art is project-owned with text fallback | Approved | Avoids branding/licensing dependency | Missing art never blocks input | No |
| EIN-D-024 | Production UI belongs to EchoUI/project | Approved | Core remains nonvisual and standalone | Sample UI only | No |
| EIN-D-025 | Built-in Input System interactions/processors are preferred | Approved | Avoids duplicate semantics | Custom helpers require documented gap | No |
| EIN-D-026 | Diagnostics never retain raw text/key/value histories | Approved | Privacy and security | Semantic events only | Yes if changed |
| EIN-D-027 | Implementation remains blocked until Foundation review completes | Approved | User chose documentation-first wave | Specification approval opens no code checkpoint | No |

### 27.2 Release-blocking questions

None remain for specification approval.

Before implementation/release, the team must reverify rather than redesign:

- Exact Unity/Input System version compatibility.
- Supported Input System test utilities for package tests.
- Platform/device matrix available for public claims.
- Whether any implementation detail requires an ADR while preserving these contracts.

### 27.3 Non-blocking later questions

- Multi-user/local multiplayer expansion and `PlayerInputManager` adapter strategy.
- XR, touch-first, sensor, virtual keyboard, and accessibility-device presets.
- Platform-specific glyph/provider packages.
- Binding-cloud synchronization boundary.
- Whether control-schema templates belong in this package or The Workshop.
- Advanced chords, repeat, buffering, and double-tap helpers beyond built-in interactions.
- Per-profile versus global default policy for particular future games.
- Input recording/replay for QA, which would require a separate privacy and architecture specification.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved package contract | This document | Approval record |
| M1 - Package Skeleton | Installable manifest, assemblies, docs shell | Dependency, namespaces, root/config stubs | Clean compile/install |
| M2 - Runtime Ownership | Duplicate-safe root and owned runtime action collection | Claim, clone/inject, validation, shutdown | Lifecycle/immutability tests |
| M3 - Contexts and Locks | Deterministic semantic availability | Primary/override contexts, directives, leases, locks | Unit/PlayMode tests |
| M4 - Devices and Users | Meaningful device/scheme and primary user | Filters, pairing, loss/reconnect | Simulated + physical tests |
| M5 - Rebinding and Overrides | Transactional rebind and migration-aware data | Conflict policies, composites, import/export | Rebind/data tests |
| M6 - Prompts and Authoring | Glyph/text prompts and controls-display data | Libraries, metadata, setup/validation | Editor tests |
| M7 - Input Laboratory | Isolated complete MVP proof | All standalone scenarios | 38-item lab checklist |
| M8 - First Integration | One approved bridge/project adoption | Likely Accord or Rescuers2D | Integration/parity report |
| M9 - Release | Distribution-ready package | Docs, licenses, 70 tests, install matrix | External install/tag |

### 28.2 Checkpoint rule

Each milestone is divided into SFGSS-005 Checkpoint Build Plans with:

- One testable outcome.
- Exact files and Editor operations.
- Scope exclusions.
- Complete code only for the active checkpoint when requested.
- Automated/manual tests and expected results.
- Failure symptoms and recovery.
- Current Notes reconciliation.
- Documentation and changelog updates.
- Commit/push stop point.

### 28.3 First recommended implementation checkpoint

Only after all Foundation specifications and their cross-package review are approved:

> **EIN-M1-01 - Create the package skeleton, manifest, assembly definitions, namespace, documentation shell, validation constants, and a compile-only root/configuration stub.**

This checkpoint must not enable actions, pair devices, or begin rebind work.

### 28.4 Foundation documentation handoff

With The Will approved:

- Foundation specifications approved: 7 of 10.
- Implementation gate: closed.
- Next checkpoint: **FW-DOC-08 - Draft The Looking Glass (`EchoUI`) Package Specification**.
- Remaining after EchoUI: The Chronicle (`EchoSave`) and The Workshop (`EchoGameStarter`).
- Final documentation gate: cross-package consistency review and reconciliation.

---

## 29. New-Conversation Handoff

```text
We are continuing development of The Sperk’s Forge - EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the suite-wide authority. Treat the approved The Will
(EchoInput) Package Specification as the authority for input contexts, locks,
device/control-scheme awareness, pairing, rebinding, binding overrides,
prompt data, diagnostics, Test Lab, and release gates.

The Foundation Wave is being fully specified before implementation. Do not
write EchoInput code until all ten Foundation specifications and the final
cross-package review are approved.

Current package checkpoint: FW-DOC-08 - The Looking Glass (EchoUI)
EchoInput specification: v1.0.0 Approved
Unity baseline: 6000.3.8f1
Planned Unity floor: 6000.0
Planned Input System floor: 1.17.0, reverify before implementation/release
Implementation status: Not started
Known blockers: None

Before changing EchoInput later:
1. Preserve its infrastructure-only authority.
2. Keep the source InputActionAsset immutable at runtime.
3. Use stable action/map/binding GUIDs.
4. Preserve leased contexts/locks and transactional rebinding.
5. Keep durable storage, production UI, gameplay behavior, and game state in
   their owning packages/project.
6. Reconcile Current Notes and update specification/ADRs before architectural
   code changes.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package | The Will (`EchoInput`) |
| Specification version | 1.0.0 |
| Specification status | Approved |
| Completed checkpoint | FW-DOC-07 |
| Files/assets created | Package specification and Foundation checkpoint documentation only |
| Tests passed | Documentation structure/consistency checks |
| Tests failed | None |
| Runtime implementation | Not started |
| Known issues | None |
| Decisions added | EIN-D-001 through EIN-D-027 |
| Next checkpoint | FW-DOC-08 - The Looking Glass (`EchoUI`) specification |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Public identity and technical responsibility are clear.
- [x] Authority and exclusions align with SFGSS-000.
- [x] Unity Input System dependency is explicit.
- [x] Standalone independence remains credible.
- [x] Runtime action ownership and source immutability are specified.
- [x] Stable identifier and migration rules are explicit.
- [x] Context and lock behavior is deterministic and lease-safe.
- [x] Device/scheme activity and pairing rules are defined.
- [x] Rebinding is transactional and conflict-aware.
- [x] Composite rollback is explicit.
- [x] Override persistence format and owner boundaries are defined.
- [x] Prompt/glyph fallback remains project-neutral.
- [x] Production UI remains optional and externally owned.
- [x] Standalone diagnostics do not require The Observatory.
- [x] Privacy rules prohibit raw input history.
- [x] Input Laboratory and 70-test strategy are measurable.
- [x] Optional integrations are explicit bridges/adapters.
- [x] No Isekai Studios identity or ownership is introduced.
- [x] Implementation remains blocked by the Foundation documentation gate.
- [x] Jesse authorized the most effective long-term choices and continued the documentation pass.

### 30.2 Approval record

**Decision:** Approved

**Approved by:** Jesse “Echo” Adams / EchoDevGames

**Date:** August 3, 2026

**Specification version:** 1.0.0

**Conditions:**

1. This approval authorizes the package contract, not implementation.
2. All ten Foundation specifications and the cross-package review must be completed first.
3. Unity and Input System version claims must be reverified before M1 and public release.
4. Any implementation discovery that changes authority, stable identity, action ownership, rebind transaction semantics, persistence ownership, or privacy requires specification revision and potentially an ADR.
5. Current Notes and documentation must be reconciled at every meaningful checkpoint.

---

## Specification Completion Statement

A new collaborator can determine from this specification:

- What The Will owns and refuses to own.
- How it operates independently.
- How it protects the source action asset.
- How contexts and locks resolve safely.
- How devices, schemes, and the primary input user are managed.
- How rebinding commits, conflicts, cancels, and rolls back.
- How stable binding overrides persist and migrate.
- How prompts fall back without proprietary art.
- How optional packages connect through bridges.
- What evidence is required before implementation, adoption, and release.

The Will package specification is therefore complete and **Approved v1.0.0**. Runtime work remains intentionally unopened.


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

- Clarified Unity asset GUID, Input System GUID, and project-authored domain identity roles.
- Made unknown extension-data preservation an explicit serializer/opaque-record requirement.

## Graph Navigation

#sfgss/package #sfgss/wave/foundation #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
