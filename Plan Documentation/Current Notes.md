# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page  
**Authority:** Working context only; SFGSS-000, approved package specifications, and accepted ADRs remain authoritative  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Last reconciled:** August 3, 2026  
**Current focus:** Foundation Wave package specifications  
**Current checkpoint:** FW-DOC-10 — Draft The Workshop (`EchoGameStarter`) package specification

> Capture quickly here. Promote deliberately at checkpoint closeout.

---

## How to Use This Page

Use this page for information discovered while designing, implementing, testing, or reviewing the suite:

- `[NOTE]` — useful observation or context.
- `[QUESTION]` — unresolved question requiring research or approval.
- `[PROPOSAL]` — suggested change that is not yet authoritative.
- `[DECISION]` — approved decision awaiting or confirming documentation promotion.
- `[TEST]` — test result, reproduction, or validation evidence.
- `[BUG]` — defect or regression awaiting issue-log placement.
- `[RISK]` — dependency, compatibility, schedule, or architecture concern.
- `[HANDOFF]` — context the next work session must see.

Keep entries dated. Link to the affected specification, ADR, checkpoint, test, issue, guide, or source file whenever possible.

Do not leave durable decisions only on this page. At checkpoint closeout, promote each material entry into the document that owns it and record the destination below.

---

## Current Focus

### Goal

Draft, reconcile, and approve complete SFGSS-001 package specifications for all ten Foundation Wave packages before any Foundation Wave runtime implementation begins.

### Active source documents

- `Echo_Game_Systems_Suite_Bible.md` — SFGSS-000 v0.6.0.
- `SFGSS-001_Package_Specification_Template.md` — v1.1.0.
- `Package Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Observatory-EchoDiagnostics-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Accord-EchoSettings-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Passage-EchoSceneFlow-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Pulse-EchoGameState-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-Resonance-Jukebot-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Will-EchoInput-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Looking-Glass-EchoUI-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Chronicle-EchoSave-Package-Specification.md` — v1.0.0 Approved.
- `Foundation_Wave_Specification_Roadmap.md` — active Level 4 planning record.

### Next action

Draft the complete **The Workshop — Project Starter (`EchoGameStarter`) Package Specification** using SFGSS-001. Define the Editor-only composer authority, package and bridge selection, dry-run planning, project-owned generated assets, safe create/repair behavior, preset contracts, scene/configuration output, dependency and compatibility reporting, rollback/removal guidance, an isolated generation laboratory, and release gates without allowing The Workshop to become a runtime manager, overwrite project content silently, conceal installed dependencies, or invent package contracts that the approved specifications do not provide.

---

## Open Questions

- None blocking the start of The Workshop specification.
- Licensing remains a later suite-wide release decision and does not block the documentation pass.

---

## Active Notes

### August 3, 2026 — Living repository documentation

- `[DECISION]` Suite and package documentation will live in the Git repository beside development work.
- `[DECISION]` The repository documentation folder will be opened directly in Obsidian rather than copied into a separate vault.
- `[DECISION]` Every active repository will expose a linked `Current Notes.md` page for ongoing observations, proposals, tests, questions, and handoff context.
- `[DECISION]` At meaningful checkpoints, durable notes will be promoted into the bible, package specification, ADR, issue/test record, guide, changelog, or checkpoint status that owns them.
- `[DECISION]` Major documentation changes will be committed with the related code when practical, or in an immediately adjacent documentation commit.

**Promoted to:** SFGSS-000 v0.5.0 decision 31 and SFGSS-001 v1.1.0 documentation requirements.

### August 3, 2026 — Foundation Specification Pass

- `[DECISION]` Complete and approve all ten Foundation Wave package specifications before beginning Foundation Wave runtime implementation.
- `[DECISION]` Run a cross-package consistency review after the tenth specification and before opening any M1 package skeleton checkpoint.
- `[DECISION]` First Light specification v1.0.0 is approved as the Level 2 package authority, but its implementation remains deferred by the suite documentation gate.
- `[DECISION]` First Light uses Unity `Awaitable<T>` for startup execution.
- `[DECISION]` First Light startup authoring uses immutable `StartupStepDefinition` ScriptableObjects that create separate single-use runtime executors.
- `[DECISION]` First Light ships a default uGUI status/image presenter isolated from its launch core.
- `[DECISION]` First Light root lifetime is configurable and defaults to `UntilHandoff`.
- `[DECISION]` The initial public Foundation package floor is Unity 6000.0, with Unity 6000.3.8f1 as the primary development baseline.

**Promoted to:** SFGSS-000 v0.6.0 decisions 32–33, First Light specification v1.0.0, and the Foundation Wave Specification Roadmap.

### August 3, 2026 — The Observatory specification

- `[DECISION]` The Observatory (`EchoDiagnostics`) specification v1.0.0 is approved as the Level 2 authority for diagnostics and validation; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoDiagnostics observes and reports package/runtime health but never becomes the source of truth for the behavior it observes and never silently repairs production state.
- `[DECISION]` Runtime integrations use explicit provider registration with stable provider IDs and disposable registration handles. Reflection-based discovery is not required for normal operation.
- `[DECISION]` Providers capture bounded, synchronous snapshots. Systems with asynchronous work cache their latest safe status rather than making the diagnostic sampler await or block gameplay.
- `[DECISION]` Diagnostic snapshots use normalized availability, health, severity, and privacy classifications so unsupported information is reported as unavailable rather than as a misleading zero or success.
- `[DECISION]` The runtime root is duplicate-safe, persists for the application session when enabled, and owns its sampler, histories, registry, event buffer, and overlay services. Editor validation can run without a runtime root.
- `[DECISION]` Runtime metric/event histories use bounded buffers and configurable update rates; diagnostic failure must degrade diagnostics rather than gameplay.
- `[DECISION]` The initial overlay uses uGUI and TextMeshPro but remains an isolated presenter over neutral diagnostic state. It does not own general UI navigation, the EventSystem, input contexts, game pause, or gameplay time scale.
- `[DECISION]` Local support-snapshot export is explicit, versioned, privacy-filtered, and never transmitted automatically.
- `[DECISION]` Editor validation supports manual, pre-Play, and pre-build execution. Repairs remain explicit and non-destructive; validation itself does not mutate production configuration.
- `[DECISION]` First Light remains independent. A separate First Light–Observatory bridge maps concrete launch status and reports into the Observatory’s neutral launch model.
- `[DECISION]` Package inventory is an Editor capability in the MVP. A Player-build package manifest is deferred until a safe build-time generation design is approved.
- `[DECISION]` The Observatory does not replace Unity’s Console or Profiler, does not promise hardware sensor support, and does not globally intercept all logs in its MVP.

**Promoted to:** The Observatory (`EchoDiagnostics`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the already-approved diagnostics authority without changing suite ownership boundaries.


### August 3, 2026 — The Accord specification

- `[DECISION]` The Accord (`EchoSettings`) specification v1.0.0 is approved as the Level 2 authority for global preferences; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoSettings owns global preference truth, defaults, committed/effective values, drafts, validation, application coordination, versioned persistence, migration, and safe display confirmation. It does not own production settings UI, audio playback, input execution, localization content, save-slot progress, pause, or gameplay rules.
- `[DECISION]` The runtime model separates project defaults, committed settings, effective settings, editable drafts, and preserved unknown section records.
- `[DECISION]` The package uses explicit stable-ID typed section registration with independent document and section schema versions. Reflection-based settings discovery is not approved.
- `[DECISION]` Unknown optional-package section payloads are preserved when their definition or bridge is absent so clean package removal does not erase data.
- `[DECISION]` Edit sessions record the committed revision they started from. A stale draft returns a conflict rather than overwriting a newer commit silently.
- `[DECISION]` Settings application is transactional. Required appliers run provisionally in deterministic order and previously applied sections revert in reverse order when a required step fails.
- `[DECISION]` Risky display changes remain provisional until confirmed. Cancel, unscaled timeout, shutdown, application failure, or persistence failure restores the previous effective platform state.
- `[DECISION]` The default backend is a versioned structured JSON document stored beneath `Application.persistentDataPath`; `PlayerPrefs` is not the default backend.
- `[DECISION]` Corrupt, unsupported-old, and newer files are preserved for recovery. Recovery/default use does not silently overwrite evidence or a newer schema.
- `[DECISION]` The MVP built-in sections are Audio, Display, and basic Accessibility. EchoSettings stores audio/accessibility preference values; Jukebot, feedback, UI, input, and localization behavior remains in optional bridges or project adapters.
- `[DECISION]` The built-in display adapter is replaceable and capability-aware. Unsupported platform fields report unavailable rather than false success.
- `[DECISION]` Optional consumers may register appliers after settings initialization and receive the current effective values, avoiding circular startup requirements.
- `[DECISION]` The core is nonvisual. A sample or EchoUI presenter owns controls, silent binding, prompts, navigation, and display-confirmation presentation.
- `[DECISION]` Named profiles, import/export, monitor selection, HDR/dynamic-resolution options, cloud synchronization, and secure storage remain deferred or outside the MVP.
- `[DECISION]` Public asynchronous operations use fresh Unity `Awaitable<T>` instances, consistent with the Foundation Unity 6 baseline.

**Promoted to:** The Accord (`EchoSettings`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the existing global-preference authority and preserve the approved cross-package ownership matrix.

### August 3, 2026 — The Passage specification

- `[DECISION]` The Passage (`EchoSceneFlow`) specification v1.0.0 is approved as the Level 2 authority for normal scene travel after First Light handoff; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoSceneFlow owns destination validation, request admission, one serialized transition pipeline, progress, activation, route helpers, recovery results, and scene-flow diagnostics. It does not own startup orchestration, game-state rules, production UI, save policy, audio playback, gameplay completion, multiplayer authority, or scene content.
- `[DECISION]` Runtime destinations use project-owned `SceneDefinition` and `SceneRouteDefinition` assets with stable IDs. Scene asset paths are backend locators maintained by Editor tooling, not durable identity or a public raw-string API.
- `[DECISION]` One duplicate-safe application-session `EchoSceneFlowRoot` owns the service, backend, queue, runner, participants, presenter registration, status, and bounded history. Duplicate rejection occurs before subscriptions or scene-operation side effects.
- `[DECISION]` The MVP backend uses Unity `SceneManager.LoadSceneAsync` for asynchronous single-scene loading behind an `ISceneLoadBackend` seam. Additive loading, owned unload, persistent scene sets, Addressables, and multiplayer providers remain deferred.
- `[DECISION]` Public asynchronous operations use fresh Unity `Awaitable<T>` instances and execute Unity scene APIs on the main thread.
- `[DECISION]` Only one scene operation may be active. The default admission policy is `RejectNew`; optional FIFO queuing is bounded, pending requests may be replaced by policy, and the active load is never replaced.
- `[DECISION]` Equivalent active or queued requests coalesce to one operation. Explicit reload remains a distinct operation.
- `[DECISION]` Cancellation is cooperative while queued or before the backend begins loading. After Unity loading starts, cancellation is reported as unsupported in the current phase and the operation continues to a safe terminal state or recovery.
- `[DECISION]` Immediate scene activation is the default. Optional delayed activation is short, hard-bounded, and never permitted to stall Unity's async operation queue indefinitely.
- `[DECISION]` Transition presenters and lifecycle participants register explicitly through disposable handles. No presenter is a valid core path; reflection discovery is not required.
- `[DECISION]` The runtime core is nonvisual and does not depend on uGUI or TextMeshPro. The Standalone Test Lab may use a sample-only presenter, and EchoUI may provide a separate production presenter bridge.
- `[DECISION]` Recovery may attempt one configured fallback only, with validation and runtime loop protection.
- `[DECISION]` Reload, Main Menu, and Hub helpers resolve project-configured route assets rather than hidden scene names.
- `[DECISION]` The direct-scene initializer is development-only by default and creates the minimum root only when an authority is absent.

**Promoted to:** The Passage (`EchoSceneFlow`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the existing normal-scene-travel authority and preserve the approved cross-package ownership matrix.


### August 3, 2026 — The Pulse specification

- `[DECISION]` The Pulse (`EchoGameState`) specification v1.0.0 is approved as the Level 2 authority for high-level runtime state, validated primary transitions, temporary override scopes, nested pause reasons, and resulting global time/cursor policy; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoGameState owns exactly one primary application state plus zero or more leased override scopes. It does not own menu presentation, input bindings, audio playback, scene loading, character/enemy state machines, save transport, or project-specific victory/defeat rules.
- `[DECISION]` Override scopes are stored as a keyed set rather than a strict last-in-first-out stack so owners may release their own scopes safely and out of order.
- `[DECISION]` Override dominance is deterministic: higher explicit priority wins and acquisition sequence breaks equal-priority ties.
- `[DECISION]` Pause is derived from active states, policies, and leases. The package exposes no fragile global pause boolean and no caller-managed increment/decrement counter.
- `[DECISION]` Any active pause requirement wins. One running scope cannot cancel another owner’s pause requirement.
- `[DECISION]` Cursor, input-intent, and audio-intent channels select the highest-priority explicit policy while remaining neutral requests for peer packages to apply through bridges.
- `[DECISION]` Primary-state transitions are synchronous and atomic. Transition guards are explicit, synchronous, deterministic, and side-effect-free; asynchronous preparation remains outside the state mutation.
- `[DECISION]` Effective policy is recomputed from current primary state and active scopes rather than restored from a previous-value stack, preventing stale restoration after out-of-order releases.
- `[DECISION]` One duplicate-safe application-session `EchoGameStateRoot` owns the state service, policy composer, Unity time/cursor adapters, scope registry, bounded history, diagnostics, and cleanup. Duplicate rejection occurs before side effects.
- `[DECISION]` Unity time and cursor behavior are behind replaceable adapters. Fixed-step scaling is configurable and disabled by default because the correct physics policy is project-specific.
- `[DECISION]` State timing, history, timeout, and diagnostics use an injected unscaled clock so they remain observable while gameplay time is paused.
- `[DECISION]` Input and audio coordination remain semantic intents. EchoInput and Jukebot retain authority over input execution and audio behavior through optional bridges.
- `[DECISION]` The runtime core is nonvisual and has no uGUI, TextMeshPro, Input System, networking, or other Echo-package dependency. The Standalone Test Lab uses removable sample-only controls and readouts.
- `[DECISION]` Primary state, active scopes, and runtime history are session state and are not automatically saved. Future persistence may store validated project-defined hints, never live lease handles.
- `[DECISION]` Direct-scene initialization is development-only by default and creates the minimum authority only when absent.
- `[DECISION]` Active scopes and state history are bounded, diagnostic provider failures remain isolated, and drift reconciliation occurs at explicit lifecycle points rather than through hidden per-frame contention.
- `[DECISION]` Slow motion, hit stop, photo-mode time modifiers, focus-loss policy, and multiplayer authority remain deferred until their neighboring package and integration contracts are approved.

**Promoted to:** The Pulse (`EchoGameState`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the already-approved runtime-state and pause authority without changing the suite ownership matrix.

### August 3, 2026 — Resonance specification

- `[DECISION]` Resonance (`Jukebot`) specification v1.0.0 is approved as the Level 2 authority for runtime music, SFX, ambience, voice-pool, playback-handle, and mixer-routing execution; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` One duplicate-safe application-session `JukebotRoot` owns ordinary music, SFX, ambience, bus, diagnostics, and runtime-state children. `Awake` claims authority only; initialization performs side effects later.
- `[DECISION]` Music, SFX, and ambience remain independent services and transports so one channel cannot accidentally stop, pause, replace, or duplicate another.
- `[DECISION]` The MVP music player uses exactly two owned sources and a deterministic transport state machine for play, pause, resume, stop, playlist navigation, rapid replacement, and crossfade behavior.
- `[DECISION]` Music starts and handoffs use Unity DSP time where scheduling improves consistency, but the package makes no universal gapless-playback claim across all clips, import settings, and platforms.
- `[DECISION]` SFX playback uses a bounded owned voice pool rather than relying on one untracked `PlayOneShot` source for the production path.
- `[DECISION]` SFX playback handles are generational so stale handles cannot stop or modify a later sound that reused the same voice.
- `[DECISION]` Cue cooldown and concurrency are validated before allocation where possible. Cue and group limits use explicit reject-or-steal policies.
- `[DECISION]` Voice stealing is deterministic by configured priority, audibility estimate, age, and stable voice index as the final tie-break.
- `[DECISION]` `MusicTrack`, `MusicPlaylist`, `SfxCue`, variations, ambience profiles, routing, and audio-profile assets remain immutable. Playlist indexes, shuffle bags, cooldown timestamps, active counts, handles, queues, and transition state are runtime-owned.
- `[DECISION]` Audio profiles use a hybrid schema-and-instance model: package/project schemas define stable semantic slots, project profiles map those slots to cues, and profile sets compose only the groups a game needs.
- `[DECISION]` Project-owned mixer routing exposes stable bus bindings. Jukebot applies normalized values and mute state but never persists global preferences; The Accord retains persistence authority.
- `[DECISION]` Jukebot does not own the project AudioListener, scene-to-music mapping, pause truth, production settings UI, gameplay triggers, or save files.
- `[DECISION]` The runtime core is nonvisual and has no uGUI, TextMeshPro, or peer Echo-package dependency. Editor preview tools and the standalone Audio Laboratory may use removable presentation dependencies.
- `[DECISION]` Mixer snapshot/ducking graphs, random ambience one-shots, segmented tracks, custom loop regions, adaptive stems, Addressables/provider clips, and reverse playback remain deferred or experimental.
- `[DECISION]` First Light, Observatory, Accord, Pulse, Passage, Looking Glass, and later gameplay connections remain explicit bridges or project adapters with clean missing-peer and removal behavior.
- `[DECISION]` The standalone Resonance Audio Laboratory must prove music transport races, pooled voices, stale handles, concurrency, ambience independence, routing, domain pause, diagnostics, reset, shutdown, and definition immutability without unrelated Echo packages.

**Promoted to:** Resonance (`Jukebot`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the already-approved audio authority without changing the suite ownership matrix.


### August 3, 2026 — The Will specification

- `[DECISION]` The Will (`EchoInput`) specification v1.0.0 is approved as the Level 2 authority for input contexts, reason-based locks, active-device/control-scheme awareness, primary-user pairing, rebinding, binding-override models, prompt/glyph data, and input diagnostics; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoInput owns input infrastructure but does not own movement physics, gameplay action meaning, controller behavior, production UI screens, high-level game state, audio feedback, scene travel, or durable preference storage.
- `[DECISION]` One duplicate-safe application-session `EchoInputRoot` owns runtime services, and duplicate rejection occurs before action cloning, subscriptions, device pairing, override application, or map enablement.
- `[DECISION]` The project-owned `InputActionAsset` remains immutable authoring data. The default runtime mode clones it into an owned action collection; advanced injected collection mode is explicit and lower-isolation.
- `[DECISION]` Actions, maps, and bindings use Unity Input System GUIDs as persistence authority. Names and indexes are never stable save identifiers.
- `[DECISION]` Context state uses one primary context plus independently leased override contexts. Map directives are Enable, Disable, or Unchanged, with deterministic priority and acquisition-order resolution.
- `[DECISION]` Input locks are additive, reason-based leases that can target all input, maps, or actions. They resolve after context directives and release safely out of order.
- `[DECISION]` EchoInput owns enablement only for configured maps/actions. External drift is detected and reported rather than fought every frame.
- `[DECISION]` The MVP supports one primary `InputUser` with conservative pairing. Device/scheme changes require meaningful input and filter analog drift, pointer jitter, noisy/synthetic events, and unassigned devices.
- `[DECISION]` Device changes never automatically change gameplay context; prompt presentation and gameplay mode remain separate truths.
- `[DECISION]` Interactive rebinding is transactional: snapshot, internal lock/context, candidate validation, conflict analysis, atomic commit, or exact rollback. One session per user is allowed by default.
- `[DECISION]` Composite rebinding commits all required parts together or restores every part.
- `[DECISION]` Conflict analysis considers normalized path, control scheme/group, user, expected type, composite identity, context overlap, shareability metadata, and reserved controls. The safe default is Reject.
- `[DECISION]` Binding overrides use a versioned package-owned document keyed by stable action/binding GUIDs, with source identity/fingerprint, migration reporting, and preserved orphan/unknown entries. Unity’s opaque override JSON is interoperability input, not the long-term authority.
- `[DECISION]` The Accord or project integration owns durable storage. EchoInput core provides session import/export and never silently chooses `PlayerPrefs`, a filename, or a profile boundary.
- `[DECISION]` Glyph libraries and control displays are project-owned. Resolution falls back from exact glyph to family/generic/text, and the core ships no unlicensed branded controller art.
- `[DECISION]` The runtime core is nonvisual and has no required uGUI, TextMeshPro, EventSystem, `PlayerInput`, generated-wrapper, or peer Echo-package dependency.
- `[DECISION]` Built-in Input System interactions and processors are preferred before custom hold/tap/multi-tap/dead-zone helpers are introduced.
- `[DECISION]` Diagnostics expose semantic state only and never retain raw typed text, key sequences, continuous input histories, full device serials, or platform-account identifiers.
- `[DECISION]` Unity 6000.0 and Input System 1.17.0 are the planned public floors, with Unity 6000.3.8f1 as the primary development baseline; exact compatibility must be reverified before implementation and release.
- `[DECISION]` The standalone Will Input Laboratory must prove contexts, locks, device filtering, pairing, transactional rebinding, composites, conflict resolution, override migration, prompt fallback, duplicate safety, reset, shutdown, and source-asset immutability without unrelated Echo packages.
- `[DECISION]` The implementation test registry contains 70 planned cases.
- `[DECISION]` No SFGSS-000 revision is required because these choices refine the already-approved EchoInput authority without changing the suite ownership matrix.

**Promoted to:** The Will (`EchoInput`) Package Specification v1.0.0.

---


### August 3, 2026 — The Looking Glass specification

- `[DECISION]` The Looking Glass (`EchoUI`) specification v1.0.0 is approved as the Level 2 authority for reusable runtime UI presentation infrastructure; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoUI owns its persistent layer root, screen history, modal ordering/results, HUD region coordination, bounded notifications, prompts/tooltips, EventSystem/focus coordination, view lifecycles, theme application, accessibility-aware presentation, and UI diagnostics. It does not own settings truth, save files, input-context authority, scene travel, pause/time, audio playback, localization content, or gameplay rules.
- `[DECISION]` The runtime uses one duplicate-safe application-session `EchoUIRoot`. Authority is claimed before EventSystem adoption/creation, layer setup, registry mutation, focus work, subscriptions, or transitions.
- `[DECISION]` The root exposes seven explicit layers: Screen, HUD, Modal, Notification, Tooltip/Prompt, Transition, and Debug.
- `[DECISION]` Screen history supports Push, Replace, Reset, and Back. Structural operations are serialized with bounded admission, explicit coalescing/rejection/queue behavior, cancellation, stale-operation protection, and hard transition bounds.
- `[DECISION]` Modal entries use owned generational handles and exact-once terminal results. Out-of-order close, owner loss, repeated completion, capacity, queue overflow, and shutdown behavior are defined.
- `[DECISION]` EventSystem behavior is an explicit non-destructive policy: adopt assigned, adopt one valid existing system, create when missing, or require an external system. EchoUI reports conflicts and never silently deletes project EventSystems.
- `[DECISION]` Focus is event-driven and deterministic, with declared defaults, restoration, scoped containment, fallback, and a legal no-selection state. The package does not perform broad hierarchy searches or force reselection every frame.
- `[DECISION]` Project-owned view prefabs and presenters interpret domain data and commands. EchoUI owns lifecycle and presentation coordination, never the domain state displayed by a view.
- `[DECISION]` HUD regions, notification queues, prompts, tooltips, screen history, modal queues, and diagnostic histories are bounded. Overflow behavior is explicit and observable.
- `[DECISION]` Runtime themes/configuration remain immutable. Effective accessibility policy may scale text, extend/manualize transient timing, suppress/reduce motion, and select contrast/fallback presentation; The Accord remains the persistence authority.
- `[DECISION]` uGUI with TextMeshPro-compatible text is the first approved backend. Exact Unity 6 dependency versions are verified at M1 rather than guessed. UI Toolkit, native screen-reader providers, XR, and advanced virtualization remain deferred.
- `[DECISION]` EchoUI diagnostics are privacy-safe and do not retain or export rendered text, typed input, arbitrary view-model payloads, profile names, screenshots, or hierarchy/file paths by default.
- `[DECISION]` Peer integrations are explicit removable bridges or project adapters. EchoUI presents settings, saves, scene loading, pause, input, and audio state without absorbing their authority.
- `[DECISION]` The isolated Looking Glass UI Laboratory defines 42 manual scenarios and the specification registers 84 implementation tests across installation, lifecycle, screens, modals, focus, accessibility, diagnostics, stress, integration, migration, and removal.

**Promoted to:** The Looking Glass (`EchoUI`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the existing UI presentation authority and preserve the approved cross-package ownership matrix.


### August 3, 2026 — The Chronicle specification

- `[DECISION]` The Chronicle (`EchoSave`) specification v1.0.0 is approved as the Level 2 authority for durable local game-save documents, slots, generations, participant payload transport, migration, recovery, and save-operation diagnostics; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoSave owns save files, slot and generation management, save/load orchestration, participant contracts, serializer/storage seams, integrity checks, backup retention, corruption recovery, and save-specific tooling. It does not own global preferences, project gameplay schemas, automatic scene-object serialization, production save UI, game-state rules, scene travel, cloud synchronization, or platform accounts.
- `[DECISION]` One duplicate-safe application-session `EchoSaveRoot` claims authority before paths, callbacks, catalog scans, participant registration, or file operations.
- `[DECISION]` The MVP uses immutable save generations. A complete generation is written, flushed where supported, checksummed, re-read/verified, and only then published as current through a small head pointer.
- `[DECISION]` Slot metadata lives in a manifest separate from the participant payload so slot lists do not deserialize full game state. The catalog cache is derived and rebuildable, never the sole authority.
- `[DECISION]` Slots use stable package-generated IDs independent from display names. Display names never become physical directory names.
- `[DECISION]` Independent game systems register narrow, stable-ID, versioned participants. EchoSave transports detached DTOs without knowing the project’s inventory, character, quest, world, or progression models.
- `[DECISION]` Unknown or temporarily unclaimed participant payloads are preserved opaquely across a load-save round trip by default. Removal requires an explicit bounded prune plan.
- `[DECISION]` Loading is two-phase: `PrepareLoadAsync` validates, recovers, deserializes, and migrates into a disposable handle; `ApplyPreparedLoadAsync` applies only after required participants exist. A one-step convenience path remains available for same-scene loads.
- `[DECISION]` Package document migrations and participant payload migrations are separate contiguous upgrade chains. Missing steps block safely, source records remain unchanged, unsupported newer formats are preserved, and downgrade is not promised.
- `[DECISION]` The default serializer uses Unity `JsonUtility` for package envelopes and plain serializable DTOs. Unsupported dictionaries, polymorphic graphs, interfaces, and durable Unity object references are documented; custom serializers use explicit provider IDs.
- `[DECISION]` Participant capture and apply occur on the main thread by default. Detached serialization, hashing, and local file I/O may run in the background when provider capability allows. Public async operations return fresh `Awaitable<T>` instances and complete on the main thread.
- `[DECISION]` One mutating operation runs globally in the MVP. Manual requests reject while busy by default, while autosaves coalesce into at most one pending latest request.
- `[DECISION]` Cancellation is honored before publication. Once head publication begins, cancellation is Too Late and the operation settles to a known committed or failed state without abandoning the prior valid generation.
- `[DECISION]` Create, rename, duplicate, select, prepare-delete, confirm-delete, prepare-load, apply-load, recovery planning, and redacted support export have structured request/result contracts. Destructive actions require explicit two-step plans.
- `[DECISION]` Checksums detect accidental corruption but are not encryption, authentication, or anti-cheat. Payload sizes, counts, migration depth, histories, queues, and paths are bounded and validated.
- `[DECISION]` Cloud/platform storage, cross-device merge, compression, encryption, streaming/chunked worlds, and multiplayer save authority remain deferred provider or future-specification work.
- `[DECISION]` The isolated Chronicle Save Laboratory defines 32 acceptance scenarios and the implementation registry contains 100 planned cases, including fault injection at each generation-publication boundary.
- `[DECISION]` No SFGSS-000 revision is required because these choices refine the already-approved EchoSave authority and preserve the global settings/save boundary.

**Promoted to:** The Chronicle (`EchoSave`) Package Specification v1.0.0.

## Promotion Queue

| Date | Entry | Destination | Status |
|---|---|---|---|
| 2026-08-03 | Repository/Obsidian living-documentation workflow | SFGSS-000 and SFGSS-001 | Promoted |
| 2026-08-03 | Foundation Specification Pass before implementation | SFGSS-000 v0.6.0 and roadmap | Promoted |
| 2026-08-03 | First Light implementation-shaping choices | First Light specification v1.0.0 | Promoted |
| 2026-08-03 | Unity 6 package floor | SFGSS-000 v0.6.0 | Promoted |
| 2026-08-03 | Observatory authority, provider, overlay, validation, privacy, and bridge decisions | EchoDiagnostics specification v1.0.0 | Promoted |
| 2026-08-03 | Accord authority, section, transaction, display-safety, persistence, migration, and bridge decisions | EchoSettings specification v1.0.0 | Promoted |
| 2026-08-03 | Passage authority, stable scene/route data, transition pipeline, admission, cancellation, activation, recovery, and bridge decisions | EchoSceneFlow specification v1.0.0 | Promoted |
| 2026-08-03 | Pulse authority, primary/override model, nested pause, policy composition, Unity adapters, diagnostics, Test Lab, and bridge decisions | EchoGameState specification v1.0.0 | Promoted |
| 2026-08-03 | Resonance authority, transport, voice-pool, handle, concurrency, routing, profile, diagnostics, Audio Laboratory, and bridge decisions | Jukebot specification v1.0.0 | Promoted |
| 2026-08-03 | Will authority, runtime action ownership, contexts, locks, device/user, transactional rebind, override, prompt, privacy, Input Laboratory, and bridge decisions | EchoInput specification v1.0.0 | Promoted |
| 2026-08-03 | Looking Glass authority, layers, screen/modal lifecycle, focus/EventSystem, HUD/notification/prompt, theme/accessibility, diagnostics, UI Laboratory, and bridge decisions | EchoUI specification v1.0.0 | Promoted |
| 2026-08-03 | Chronicle authority, immutable generations, head publication, participant payloads, two-phase load, migration, recovery, autosave, security, Save Laboratory, and bridge decisions | EchoSave specification v1.0.0 | Promoted |

---

## Latest Validation Snapshot

| Area | Result | Evidence/notes |
|---|---|---|
| Suite bible | Approved and unchanged this checkpoint | v0.6.0; Chronicle decisions refine the existing save authority without changing suite ownership or the settings/save boundary |
| Package specification template | Approved and unchanged | v1.1.0 |
| First Light specification | Approved | v1.0.0; no release-blocking design questions |
| Observatory specification | Approved | v1.0.0; no release-blocking design questions |
| Accord specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; no release-blocking design questions |
| Passage specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; no release-blocking design questions |
| Pulse specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; no release-blocking design questions |
| Resonance specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; no release-blocking design questions |
| Will specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; 70-test registry; no release-blocking design questions |
| Looking Glass specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; 42 Laboratory scenarios and 84-test registry; no release-blocking design questions |
| Chronicle specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; 32 Laboratory scenarios and 100-test registry; no release-blocking design questions |
| Foundation documentation gate | Active | 9 of 10 package specifications approved |
| Implementation | Not started by design | No package code begins before FW-DOC-12 passes |

---

## Checkpoint Closeout Checklist

- [x] Review every note added during the checkpoint.
- [x] Separate confirmed facts from proposals and unresolved questions.
- [x] Promote architecture changes into the bible/specification and an ADR when needed.
- [x] Move bugs and test evidence into their permanent records. No implementation defects exist yet.
- [x] Update guides and changelog for user-visible changes. Not applicable before package implementation.
- [x] Update the current checkpoint, blockers, and next action.
- [x] Confirm documentation matches committed implementation and tests. Implementation has not started.
- [ ] Commit and push the documentation update.
- [x] Condense or remove resolved notes after promotion.

---

## Handoff Snapshot

**Current program:** Foundation Specification Pass  
**Completed package specifications:** First Light (`EchoLaunch`) v1.0.0 Approved; The Observatory (`EchoDiagnostics`) v1.0.0 Approved; The Accord (`EchoSettings`) v1.0.0 Approved; The Passage (`EchoSceneFlow`) v1.0.0 Approved; The Pulse (`EchoGameState`) v1.0.0 Approved; Resonance (`Jukebot`) v1.0.0 Approved; The Will (`EchoInput`) v1.0.0 Approved; The Looking Glass (`EchoUI`) v1.0.0 Approved; The Chronicle (`EchoSave`) v1.0.0 Approved  
**Current package:** The Workshop (`EchoGameStarter`)  
**Current stage:** Specification not yet drafted  
**Last completed documentation change:** EchoSave duplicate-safe root, immutable generation/head publication model, independent manifests, versioned participant payloads, unknown payload preservation, two-phase load, serializer/storage seams, migration, autosave admission, corruption recovery, security limits, isolated Save Laboratory, and optional bridge boundaries approved  
**Known blockers:** None  
**Next checkpoint:** FW-DOC-10 — Draft and review the complete EchoGameStarter package specification
