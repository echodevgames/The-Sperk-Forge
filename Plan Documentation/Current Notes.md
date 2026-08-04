# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page  
**Authority:** Working context only; SFGSS-000, approved package specifications, accepted ADRs, integration specifications, and approved Checkpoint Build Plans remain authoritative  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Last reconciled:** August 4, 2026  
**Current focus:** EchoProgression - The Ascent package specification
**Current checkpoint:** SUITE-DOC-07 - EchoProgression: The Ascent Package Specification

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

Complete every Expansion and Advanced package foundation in SFGSS-000 Sections 7.2 and 7.3 before package implementation begins, then finish the remaining standards and final reconciliation. Preserve honest `Not run` states for evidence that requires implementation.

### Active source documents

- `Echo_Game_Systems_Suite_Bible.md` — SFGSS-000 v0.12.0.
- `SFGSS-002_Dependency_Bridge_and_Assembly_Standard.md` — v1.0.0 Approved.
- `SFGSS-003_Data_IDs_Serialization_and_Migration_Standard.md` — v1.0.0 Approved.
- `SFGSS-004_Testing_Validation_Test_Labs_and_Release_Standard.md` — v1.0.0 Approved.
- `SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules.md` — v1.1.0 Approved.
- `Architecture Decision Records/SFGSS-ADR-002_Full_Suite_Documentation_Gate_and_Learning_Implementation.md` — Accepted.
- `Full_Suite_Documentation_Program_Roadmap.md` — active roadmap.
- `Test Reports/Full_Suite_Documentation_Rebaseline_Report.md` — SUITE-DOC-01 Passed.
- Foundation package specifications, ADR-001, and the Foundation cross-package matrix — approved baseline.
- `Checkpoint Build Plans/First_Light_M1_Package_Skeleton_Checkpoint_Build_Plan.md` — approved but dormant.

### Next action

Draft and approve the complete **EchoProgression - The Ascent Package Specification** using SFGSS-001. Define unlocks, passwords, checkpoints, level access, completion records, ranking snapshots, project-defined conditions, standalone persistence boundaries, Chronicle participation, UI/scene integrations, stable progression IDs, migration, diagnostics, and an isolated Standalone Laboratory without absorbing inventory, character statistics, platform achievements, or general save-file transport.

---

## Open Questions

- Licensing remains a later suite-wide release decision.
- Final Multiplayer provider approval requires disposable prototype evidence and cannot be truthfully completed during the pre-code documentation gate.
- Empirical compatibility, performance, migration, screenshot, and release evidence remains `Not run` until implementation.
- No question currently blocks SUITE-DOC-06.

---

## Active Notes

### August 4, 2026 - The Wellspring (`EchoPool`) package specification

- `[DECISION]` The Wellspring (`EchoPool`) Package Specification v1.0.0 is approved as the Level 2 authority for general-purpose GameObject and Component reuse; implementation remains locked until SUITE-DOC-33.
- `[DECISION]` EchoPool owns pool definitions, runtime pool instances, prewarming, acquisition, generational leases, validated return, capacity/growth/exhaustion policy, scope ownership, lifecycle callbacks, automatic return, external-destruction reconciliation, and diagnostics.
- `[DECISION]` EchoPool does not own spawn intent, encounters, projectile behavior, audio voices, UI virtualization, network spawning authority, save truth, or project-specific reset semantics.
- `[DECISION]` The default runtime uses one duplicate-safe application-session `EchoPoolRoot` implementing an injectable `IEchoPoolService`; scene and owner pools remain explicit child scopes.
- `[DECISION]` Every successful spawn returns a session-local generational handle. Stale, foreign, double-returned, lost, and destroyed handles fail without mutating the current instance use.
- `[DECISION]` `PoolDefinition` and catalog assets are immutable project-owned definitions with stable domain IDs. Active counts, records, generations, schedules, scenes, scopes, and statistics remain runtime state.
- `[DECISION]` Exhaustion defaults to safe rejection. Bounded temporary overflow is opt-in and is destroyed rather than retained on return. Forced reclamation of active instances is deferred.
- `[DECISION]` The core resets only generic parent, transform, active state, scene, and lease metadata. Project-specific state resets through `IPoolable` or explicit optional adapters; reflection-based universal reset is rejected.
- `[DECISION]` Application, scene, and owner-lease scopes are approved. Standalone scene-unload reconciliation remains available, while a separate Passage bridge may coordinate pre-unload cleanup.
- `[DECISION]` Manual, scaled-duration, unscaled-duration, and generic completion-signal returns are approved. Every schedule/signal binds to the current generation.
- `[DECISION]` Active pool handles and runtime instances are never saved. Gameplay authorities save semantic state and reconstruct objects through their own factories.
- `[DECISION]` Jukebot retains its internal audio voice pool, and network object reuse requires a provider-specific adapter.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 36 unique Laboratory scenarios, and 118 unique planned package test IDs.
- `[TEST]` Every runtime, installation, scene, performance, compatibility, provider, removal, and release test remains `Not run` under SFGSS-004.
- `[NOTE]` SFGSS-000 remains v0.12.0 because the specification refines the already-approved EchoPool authority without changing a suite-wide ownership boundary.
- `[HANDOFF]` SUITE-DOC-07 drafts EchoProgression (`The Ascent`) next. Preserve unlock/checkpoint/progression authority without absorbing save-file transport, inventory, character statistics, or platform achievements.

**Promoted to:** The Wellspring (`EchoPool`) Package Specification v1.0.0, SUITE-DOC-06 audit report, README, roadmap, artifact manifest, and Current Notes handoff.

### August 4, 2026 — Impact (`EchoFeedback`) package specification

- `[DECISION]` Impact (`EchoFeedback`) Package Specification v1.0.0 is approved as the Level 2 authority for coordinated transient feedback; implementation remains locked until SUITE-DOC-33.
- `[DECISION]` EchoFeedback owns immutable feedback recipes, transient instance execution, unscaled scheduling, channel scaling, arbitration, cancellation, provider coordination, and bounded diagnostics.
- `[DECISION]` EchoFeedback does not own gameplay results, camera movement, audio playback, UI state, input-device assignment, settings persistence, save data, or final pause/time-scale authority.
- `[DECISION]` The MVP uses a flat semantic timeline. Parallel and sequential behavior are expressed through start offsets rather than a branching graph or general visual-scripting language.
- `[DECISION]` Production effects execute only through explicitly registered channel providers. The core remains independent of EchoCamera, Jukebot, EchoUI, EchoInput, The Accord, and The Pulse.
- `[DECISION]` Recipe and signal ScriptableObjects are immutable definitions with stable domain IDs. Active instances, handles, clocks, providers, scales, and histories are runtime-owned state.
- `[DECISION]` Public feedback handles are generational so stale handles cannot cancel recycled instances.
- `[DECISION]` Scheduling, cancellation, restoration, and diagnostics use an unscaled clock so feedback remains controllable while scaled game time is zero.
- `[DECISION]` A standalone Unity time provider is opt-in and exclusive. When The Pulse is installed, a separate bridge preserves The Pulse as the final time authority.
- `[DECISION]` Input System haptics belong to a separate provider artifact rather than the core package.
- `[DECISION]` Project safety caps, accessibility scales, and channel suppression apply before provider execution. Providers receive already-resolved effective values.
- `[DECISION]` The isolated Impact Laboratory uses simulated providers. Simulation proves the core package but does not count as support evidence for optional bridges or hardware providers.
- `[TEST]` The specification contains all 30 SFGSS-001 sections, 32 unique Laboratory scenarios, and 92 unique planned test IDs.
- `[TEST]` Every runtime, provider, compatibility, performance, platform, removal, and release test remains `Not run` under SFGSS-004.
- `[NOTE]` SFGSS-000 remains v0.12.0 because the Impact specification refines the already-approved EchoFeedback authority without changing suite-wide ownership.
- `[HANDOFF]` SUITE-DOC-06 drafts EchoPool (`The Wellspring`) next. Preserve general-purpose object reuse authority without absorbing enemy spawning, projectile rules, audio voice pooling, or other package-owned behavior.

**Promoted to:** Impact (`EchoFeedback`) Package Specification v1.0.0, SUITE-DOC-05 audit report, README, roadmap, and Current Notes handoff.

### August 4, 2026 — Package specification priority clarification

- `[DECISION]` The owner clarified that “continue until all documentation is ready instead of just 7.1” primarily means completing the package foundations in SFGSS-000 Sections 7.2 Expansion and 7.3 Advanced before implementation.
- `[DECISION]` Remaining general standards no longer block the start of Expansion package specifications. They move after the package foundations, except where a standard is directly required to resolve an active package decision.
- `[DECISION]` SFGSS-002, SFGSS-003, and SFGSS-004 remain approved and become the dependency, data, and evidence guardrails for every remaining package specification.
- `[DECISION]` Expansion specifications follow the owner’s listed order beginning with EchoFeedback, then EchoPool, EchoProgression, EchoBuildTools, EchoLocalization, EchoDialogue, EchoObjectives, EchoInventory, EchoInteraction, EchoCamera, EchoCharacters, EchoControllers, and EchoCrafting.
- `[DECISION]` Advanced package foundations follow with EchoMultiplayer, EchoAI, EchoCombat, EchoAbilities, and EchoWorld.
- `[DECISION]` EchoCrafting’s checkpoint must include its required design-workshop record before approving the package contract. EchoMultiplayer remains evidence-honest and may approve research, neutral contracts, and prototype criteria without claiming unperformed provider prototypes.
- `[DECISION]` The final documentation unlock gate is renumbered to SUITE-DOC-33 after the roadmap is condensed around package-first work.

**Promoted to:** Full Suite Documentation Program Roadmap, Package Specification Priority Rebaseline Report, README, and Current Notes handoff.

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


### August 3, 2026 — The Workshop specification

- `[DECISION]` The Workshop (`EchoGameStarter`) specification v1.0.0 is approved as the Level 2 authority for Editor-time package selection, composition planning, package-operation coordination, safe project generation, generation records, repair planning, removal guidance, and readiness reporting. Implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` The Workshop is Editor-only and ships no runtime assembly, persistent root, `GameManager`, service locator, or Player dependency. Generated projects must remain valid after The Workshop is removed.
- `[DECISION]` Every apply operation begins from an immutable dry-run plan that exposes package changes, bridges, scenes, folders, assets, project settings, risk, ownership, and reversibility. A materially changed resolved package graph invalidates approval and requires review.
- `[DECISION]` Package Manager operations and asset generation are separate resumable phases. A transient journal under `Library/EchoGameStarter/Transactions` records recovery state across domain reload or Editor restart, but never auto-resumes mutation after restart.
- `[DECISION]` Normal package changes use Unity Package Manager Client APIs rather than direct editing of `Packages/manifest.json`. Recommended sources use exact versions, tags, or commits; development branches remain visibly non-reproducible choices.
- `[DECISION]` Package-specific setup remains owned by the selected package. The Workshop uses exact allowlisted, versioned Editor setup-facade adapter descriptors and does not perform open-ended assembly discovery or copy package setup logic.
- `[RESOLVED]` FW-DOC-11 reconciled the setup-facade contract through SFGSS-ADR-001. No shared Editor-only contracts package was introduced.
- `[DECISION]` Generated output is project-owned. A durable manifest records logical IDs, GUIDs, paths, origins, versions, fingerprints, adoption, modification, and operation receipts without granting The Workshop perpetual control.
- `[DECISION]` Create-only-safe behavior is the default. Existing, adopted, or modified assets are preserved. Any fingerprint drift removes automatic overwrite eligibility and moves upgrades to manual or side-by-side handling.
- `[DECISION]` The MVP ships Blank Modular Starter and Game Jam Quickstart. Blank may select no peer packages. Game Jam shows every selected package and bridge; the Chronicle is an explicit save-model choice rather than a hidden requirement.
- `[DECISION]` The MVP provides repeat-run analysis, safe repair plans for eligible missing outputs, a basic upgrade diff, and a removal guide. Full automatic uninstall remains deferred.
- `[DECISION]` Unity 6 global scene lists and Build Profile overrides are handled through an explicit adapter with complete before/after reporting; ambiguity blocks modification.
- `[DECISION]` UI Toolkit is the approved Editor UI. The core standalone proof is an isolated Workshop Laboratory and disposable clean-project fixtures rather than a meaningless runtime scene.
- `[DECISION]` The Laboratory defines 40 acceptance scenarios and the specification registers 121 implementation tests spanning package resolution, reload recovery, planning, security, generation, setup facades, scenes, reports, migration, repeatability, removal, and performance.
- `[DECISION]` The Workshop never commits or pushes source control in the MVP. It writes commit-friendly reports and leaves Git actions to the user or a future explicit provider.
- `[RESOLVED]` The Workshop package decisions originally required no SFGSS-000 revision. FW-DOC-11 later promoted the suite-wide facade and collision rules into SFGSS-000 v0.7.0.

**Promoted to:** The Workshop (`EchoGameStarter`) Package Specification v1.0.0.


### August 3, 2026 — Foundation cross-package collision review

- `[DECISION]` SFGSS-INT-FOUNDATION-001 is approved as the Foundation authority/lifecycle/dependency/bridge/data/Test Lab/removal reconciliation record.
- `[TEST]` All ten specifications retain exactly one Foundation authority per concern and no peer runtime dependency in core assemblies.
- `[BUG]` The Pulse and The Workshop both used the `EGS-*` diagnostic namespace.
- `[DECISION]` The Pulse specification advances to v1.1.0 and uses the globally unique `EGSTATE-*` namespace. EchoGameStarter retains `EGS-*`.
- `[RISK]` The nine peer packages defined setup tools but no exact Editor endpoint for The Workshop.
- `[DECISION]` SFGSS-ADR-001 accepts a package-owned exact Editor setup facade protocol with allowlisted types, six static JSON methods, plan/apply hashes, receipts, bounded reflection, and manual fallback.
- `[DECISION]` The Workshop specification advances to v1.1.0 and records SFGSS-ADR-001 as the resolved facade contract.
- `[DECISION]` Separate bridge packages declare dependencies on both peers and are removed before either peer. Core packages remain independently functional.
- `[DECISION]` Direct-scene helpers create only their own minimum missing root; First Light bridges adopt existing valid peer authorities.
- `[DECISION]` Cross-package reports qualify locally repeated `UC-*`, `CAP-*`, and `LAB-*` identifiers with the package ID.
- `[TEST]` Settings/save boundaries, launch-to-Passage handoff, UI/input/state/audio boundaries, diagnostics bridges, standalone laboratories, and removal behavior pass the documentation collision review.
- `[HANDOFF]` No runtime implementation is authorized yet. FW-DOC-12 is the final documentation gate.

**Promoted to:** SFGSS-000 v0.7.0 decisions 34–38, The Pulse specification v1.1.0, The Workshop specification v1.1.0, SFGSS-ADR-001, and SFGSS-INT-FOUNDATION-001.



### August 3, 2026 — Foundation Documentation Readiness Gate

- `[TEST]` FW-DOC-12 verified ten Approved Foundation specifications with all thirty SFGSS-001 sections present.
- `[TEST]` SFGSS-ADR-001 and SFGSS-INT-FOUNDATION-001 are present and aligned with Pulse v1.1.0 and Workshop v1.1.0.
- `[BUG]` The repository referenced SFGSS-005 without containing the workflow document.
- `[DECISION]` SFGSS-005 v1.0.0 is approved as the Checkpoint Build Workflow and ChatGPT Collaboration authority.
- `[BUG]` First Light v1.0.0 still pointed to the completed documentation gate rather than the first implementation checkpoint.
- `[DECISION]` First Light advances to v1.1.0 for status/workflow reconciliation only; runtime behavior and API intent are unchanged.
- `[DECISION]` FW-DOC-12 passes. FL-M1-01 — First Light Package Skeleton is the first authorized implementation checkpoint.
- `[DECISION]` FL-M1-01 authorizes package files and validation only. It authorizes no C# script, authority root, ScriptableObject, prefab, scene, sample, setup tool, or bridge.
- `[HANDOFF]` Import this checkpoint, commit/push it, then execute `Checkpoint Build Plans/First_Light_M1_Package_Skeleton_Checkpoint_Build_Plan.md`.

**Promoted to:** SFGSS-000 v0.8.0 decisions 39–40, SFGSS-005 v1.0.0, First Light v1.1.0, the Foundation Documentation Readiness Report, and FL-M1-01 Checkpoint Build Plan.

---


### August 3, 2026 — Full Suite Documentation Rebaseline

- `[DECISION]` Extend the documentation-first gate from the Foundation Wave to the complete planned documentation program in SFGSS-000 Section 18.
- `[DECISION]` Preserve all Foundation approvals and readiness evidence; FL-M1-01 remains the first queued implementation checkpoint but is dormant until SUITE-DOC-36.
- `[DECISION]` Complete architecture standards, Expansion specifications, Advanced design/research records, and final full-suite collision/readiness reviews before code.
- `[DECISION]` Distinguish design-complete documentation from implementation evidence. Do not invent compile results, screenshots, performance measurements, compatibility validation, migration evidence, release notes, or prototype findings.
- `[DECISION]` When implementation begins, ChatGPT must show complete compile-ready code in the conversation, explain each file and important section, provide exact Unity Editor steps, and teach the architectural reason for the choice.
- `[DECISION]` Jesse enters the code himself by default. Generated source files or direct edits occur only when explicitly requested and do not replace visible code/explanations.
- `[HANDOFF]` The active checkpoint is SUITE-DOC-02 — SFGSS-002 Dependency, Bridge, and Assembly Standard.

**Promoted to:** SFGSS-000 v0.9.0 decisions 41–43, SFGSS-ADR-002, SFGSS-005 v1.1.0, Full Suite Documentation Program Roadmap, and the SUITE-DOC-01 Rebaseline Report.

---


### August 4, 2026 — Dependency, Bridge, and Assembly Standard

- `[DECISION]` SFGSS-002 v1.0.0 is approved as the canonical package-manifest, assembly-direction, bridge/provider, compile-guard, sample/test dependency, compatibility, and clean-removal standard.
- `[DECISION]` Core runtime packages do not reference optional peer Echo packages. A separate bridge declares dependencies on every peer it connects; peers never reference the bridge.
- `[DECISION]` UPM manifests record concrete required dependency versions. Broader compatible/tested ranges live in documentation and the suite compatibility catalog and remain pending until evidenced.
- `[DECISION]` Runtime assemblies cannot reference Editor, test, sample, Workshop, project, or optional-peer assemblies. Optional presentation/backend/provider technologies are isolated when they are not central hard dependencies.
- `[DECISION]` Primary public Runtime assemblies may remain Auto Referenced for novice usability. Editor, tests, samples, and optional bridge/provider assemblies default to non-auto-referenced unless a documented public use case requires otherwise.
- `[DECISION]` Compile symbols, version defines, `.asmref` files, and reflection cannot conceal dependency truth or replace a proper bridge/provider package.
- `[DECISION]` Exact allowlisted Editor reflection remains permitted for ADR-001 setup facades; broad assembly scans remain prohibited.
- `[DECISION]` Standalone Labs use only the package and hard dependencies. Integration Labs belong to the bridge/provider artifact.
- `[DECISION]` Optional artifacts follow bridge-first teardown/removal and own all registrations, leases, subscriptions, and adapter resources they create.
- `[TEST]` The standard was reconciled against SFGSS-000, SFGSS-001, ADR-001, ADR-002, the Foundation contract matrix, and all ten Foundation assembly/dependency tables.
- `[RISK]` First Light’s approved assembly table still places proposed uGUI in the neutral Runtime assembly; SFGSS-002 prefers a separate presentation assembly. Reconcile during SUITE-DOC-10 before code.
- `[RISK]` Several Foundation specifications list Editor assemblies as Auto Referenced or describe optional sample uGUI/TMP dependencies without a final compile-safe packaging decision. Reconcile during SUITE-DOC-10.
- `[HANDOFF]` SUITE-DOC-03 must align stable IDs, DTOs, unknown-data preservation, aliases, migrations, transactions, and provider/package removal with SFGSS-002.

**Promoted to:** SFGSS-000 v0.10.0 decisions 44–51, SFGSS-002 v1.0.0, the SUITE-DOC-02 audit report, README, and the full-suite roadmap.

---


### August 4, 2026 — Data, IDs, Serialization, and Migration Standard

- `[DECISION]` SFGSS-003 v1.0.0 is approved as the canonical data classification, identity, Unity GUID, serialization, migration, unknown-data, transaction, recovery, and durable-removal standard.
- `[DECISION]` Unity asset GUIDs, package/project domain stable IDs, and runtime instance IDs are separate contracts. AssetDatabase identity is Editor-only unless explicitly copied into a runtime-safe build record.
- `[DECISION]` Stable domain IDs use either approved opaque generated IDs or package/project-qualified semantic IDs. Names, paths, indexes, timestamps alone, runtime instance IDs, and CLR type names are not durable identity.
- `[DECISION]` Shared ScriptableObjects and configuration assets remain immutable runtime inputs. Mutable session state lives in authority-owned runtime records; durable state uses detached DTOs or opaque payloads.
- `[DECISION]` Durable documents declare a format ID and schema version independently from package SemVer. Serializer providers state supported shapes, bounds, unknown-field behavior, determinism, and failure behavior.
- `[DECISION]` Unity JsonUtility is approved for simple DTOs only. It does not by itself provide dictionary, general polymorphism, or unknown-field round-trip guarantees.
- `[DECISION]` Supported migrations are explicit contiguous forward steps on staged data. They preserve the source until verified publication, report changes, and do not promise downgrade.
- `[DECISION]` Released ID changes use aliases or tombstones. Alias cycles, ambiguous mappings, and reuse of retired IDs are prohibited.
- `[DECISION]` Unknown optional settings, save, provider, and generated records remain bounded, opaque, preserved, and non-executable through package absence/reinstallation.
- `[DECISION]` Data-changing operations validate and stage before one documented publication point. Each package states its real rollback class and never labels a partial apply as atomic.
- `[TEST]` SFGSS-003 was reconciled against SFGSS-000, SFGSS-001, SFGSS-002, ADR-001, ADR-002, the Foundation matrix, and all ten Foundation package data sections.
- `[RISK]` Accord and Chronicle use “Asset GUID” wording for configuration identity. Clarify Unity asset identity versus runtime domain identity during SUITE-DOC-10.
- `[RISK]` Accord and Will unknown-field preservation requires an explicit opaque-record or extension-capable serializer strategy before implementation.
- `[RISK]` Foundation public serialized enums and fingerprints require compatibility/canonicalization review during SUITE-DOC-10.
- `[HANDOFF]` SUITE-DOC-04 must turn existing package test lists into one canonical evidence, laboratory, compatibility, defect, and release standard without claiming tests have run.

**Promoted to:** SFGSS-000 v0.11.0 decisions 52–61, SFGSS-003 v1.0.0, the SUITE-DOC-03 audit report, README, and the full-suite roadmap.

---

### August 4, 2026 – SFGSS-004 Testing, Validation, Test Labs, and Release Standard

- `[DECISION]` SFGSS-004 v1.0.0 is approved as the suite test, evidence, Laboratory, compatibility, defect, and release-quality authority.
- `[DECISION]` Durable test results use Not run, Pass, Pass with advisory, Fail, Blocked, or Not applicable.
- `[DECISION]` Compatibility language uses Unknown, Planned, Tested, Supported, Experimental, or Unsupported and must name the exact environment covered.
- `[DECISION]` Stable test IDs are package/bridge/provider-qualified and are never recycled.
- `[DECISION]` Test definitions and executions are separate records. A planned registry is not passing evidence.
- `[DECISION]` Standalone Laboratories prove one package; Integration Laboratories belong to bridges/providers; Showcases do not replace either proof.
- `[DECISION]` Clean import/compile must be followed by the smallest functional workflow for each advertised installation route.
- `[DECISION]` Setup, repair, migration, removal, reinstall, failure recovery, performance, platform, accessibility, privacy, and security evidence are explicit release concerns when applicable.
- `[DECISION]` Defect severity is Blocker, Critical, Major, Minor, or Advisory and remains separate from priority.
- `[DECISION]` Flaky/quarantined required tests and retry-hidden failures cannot count as passing stable release evidence.
- `[DECISION]` Beta, release-candidate, and stable gates require progressively stronger evidence.
- `[TEST]` SFGSS-004 was reconciled against SFGSS-000, SFGSS-001, SFGSS-002, SFGSS-003, SFGSS-005, ADR-001, ADR-002, the Foundation matrix, and all ten Foundation package test/release sections.
- `[RISK]` Bare Laboratory IDs, mixed automation fields, compressed Will test ranges, broad platform wording, combined distribution gates, and missing evidence/issue columns require normalization during SUITE-DOC-10.
- `[HANDOFF]` SUITE-DOC-05 must turn package-selection guidance into explicit user pathways without creating hidden hard dependencies or pretending every project needs the full Foundation set.

**Promoted to:** SFGSS-000 v0.12.0 decisions 62–71, SFGSS-004 v1.0.0, the SUITE-DOC-04 audit report, README, and the full-suite roadmap.

---

## Promotion Queue

| Date | Entry | Destination | Status |
|---|---|---|---|
| 2026-08-04 | EchoPool definitions, generational leases, capacity, exhaustion, scopes, callbacks, automatic return, reconciliation, diagnostics, and Laboratory | EchoPool Package Specification v1.0.0 | Promoted |
| 2026-08-04 | Impact recipes, providers, timeline, cancellation, channel scales, time boundary, diagnostics, and Laboratory | EchoFeedback Package Specification v1.0.0 | Promoted |
| 2026-08-04 | Testing taxonomy, evidence states, Laboratories, compatibility, defects, performance, and release gates | SFGSS-000 v0.12.0 and SFGSS-004 v1.0.0 | Promoted |
| 2026-08-04 | Data classification, stable IDs, Unity GUIDs, DTOs, serializers, migrations, aliases, unknown data, transactions, and recovery | SFGSS-000 v0.11.0 and SFGSS-003 v1.0.0 | Promoted |
| 2026-08-04 | Dependency, bridge, provider, assembly, compile-guard, sample/test, and clean-removal rules | SFGSS-000 v0.10.0 and SFGSS-002 v1.0.0 | Promoted |
| 2026-08-03 | Full Suite Documentation Gate and learning-oriented implementation | SFGSS-000 v0.9.0, SFGSS-ADR-002, SFGSS-005 v1.1.0, full-suite roadmap | Promoted |
| 2026-08-03 | Foundation Documentation Readiness Gate | SFGSS-000 v0.8.0 and readiness report | Promoted |
| 2026-08-03 | Checkpoint Build Workflow and ChatGPT collaboration rules | SFGSS-005 v1.0.0 | Promoted |
| 2026-08-03 | First Light implementation handoff and FL-M1-01 selection | First Light v1.1.0 and FL-M1-01 plan | Promoted |
| 2026-08-03 | Foundation authority/lifecycle/dependency/data/Test Lab/removal collision review | SFGSS-INT-FOUNDATION-001 and SFGSS-000 v0.7.0 | Promoted |
| 2026-08-03 | Package-owned Editor setup facade protocol | SFGSS-ADR-001 and Workshop v1.1.0 | Promoted |
| 2026-08-03 | EchoGameState/EchoGameStarter diagnostic namespace collision | Pulse v1.1.0 and SFGSS-000 v0.7.0 | Promoted |
| 2026-08-03 | Foundation Specification Pass before implementation | SFGSS-000 v0.6.0 and roadmap | Promoted |
| 2026-08-03 | Repository/Obsidian living-documentation workflow | SFGSS-000 and SFGSS-001 | Promoted |

---

## Latest Validation Snapshot

| Area | Result | Evidence/notes |
|---|---|---|
| Suite bible | Approved and test-standard-reconciled | v0.12.0; decisions 62–71 approve SFGSS-004 rules |
| Package specification template | Approved | SFGSS-001 v1.1.0 |
| Checkpoint workflow | Approved | SFGSS-005 v1.1.0; complete visible code and teaching rules added |
| Foundation package specifications | Approved | Ten of ten |
| Foundation architecture/integration | Approved | ADR-001 and Foundation matrix |
| Full-suite gate decision | Accepted | SFGSS-ADR-002 |
| Active roadmap | Approved | `Full_Suite_Documentation_Program_Roadmap.md` |
| Foundation readiness | Historically passed | Immediate activation superseded, evidence retained |
| First implementation plan | Approved but dormant | FL-M1-01 v1.1.0 |
| Package implementation | Not started | No package files or code authorized |
| Dependency/bridge/assembly standard | Approved | SFGSS-002 v1.0.0 |
| Data/IDs/serialization/migration standard | Approved | SFGSS-003 v1.0.0 |
| Testing/validation/Laboratory/release standard | Approved | SFGSS-004 v1.0.0 |
| Impact package specification | Approved | v1.0.0; 30 sections; 32 Laboratory scenarios; 92 planned tests, all Not run |
| EchoPool package specification | Approved | v1.0.0; 30 sections; 36 Laboratory scenarios; 118 planned tests, all Not run |
| Current checkpoint | Active | SUITE-DOC-07 |
| Known blockers | None | Multiplayer empirical provider approval intentionally remains later |

---

## Checkpoint Closeout Checklist

- [x] Reconcile EchoPool against SFGSS-000 through SFGSS-005 and approved Foundation/Impact authorities.
- [x] Approve reuse authority without absorbing gameplay spawn intent or peer-owned internal pools.
- [x] Separate immutable definitions from runtime pool, record, lease, schedule, scope, and diagnostic state.
- [x] Define generational handles, callback order, capacity, growth, exhaustion, overflow, retention, automatic return, and external-destruction behavior.
- [x] Define application, scene, and owner-lease scopes plus standalone scene reconciliation and optional Passage coordination.
- [x] Design the isolated Wellspring Laboratory and package-qualified planned test registry.
- [x] Keep every unexecuted runtime, performance, platform, compatibility, integration, and release result `Not run`.
- [x] Update README, Current Notes, roadmap, audit report, and artifact manifest.
- [x] Confirm no manifest, asmdef, C# file, scene, prefab, ScriptableObject, setup tool, sample, bridge, or provider implementation was created.
- [ ] Commit and push SUITE-DOC-06.
- [x] Stop before EchoProgression specification work.

---

## Handoff Snapshot

**Completed checkpoint:** SUITE-DOC-06 - The Wellspring (`EchoPool`) Package Specification  
**Result:** Approved v1.0.0  
**Current focus:** EchoProgression - The Ascent  
**Active checkpoint:** SUITE-DOC-07 - EchoProgression Package Specification  
**Expansion specifications:** 2 of 13 approved  
**Package implementation:** Not started  
**First queued implementation:** FL-M1-01 - First Light Package Skeleton  
**Runtime authorization:** None  
**Known blockers:** None  
**Prior checkpoint:** SUITE-DOC-05 confirmed committed/pushed by owner  
**Commit/push:** SUITE-DOC-06 pending user confirmation  
**Stop point:** Before any package manifest, asmdef, C# file, scene, prefab, ScriptableObject, setup tool, sample, bridge, provider adapter, or gameplay implementation
