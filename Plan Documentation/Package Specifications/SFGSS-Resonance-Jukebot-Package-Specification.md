# Resonance - Audio Runtime Package Specification

**Working document ID:** SFGSS-PKG-JUKEBOT-001  
**Specification version:** 1.0.0  
**Status:** Approved  
**Technical package name:** Jukebot  
**Public title:** Resonance - Audio Runtime  
**Package ID:** `com.echodevgames.jukebot`  
**Runtime namespace:** `EchoDevGames.Jukebot`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/Jukebot`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 and SFGSS-001  
**Last updated:** August 3, 2026

> “Give every world a voice without letting the soundtrack become the game’s hidden ruler.”

> **Approval rule:** This specification is approved as the authoritative package design. Runtime implementation remains intentionally deferred until all ten Foundation Wave specifications and the cross-package consistency review are approved.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-03 | Proposed | Initial complete specification derived from SFGSS-000 v0.6.0, SFGSS-001 v1.1.0, and the five previously approved Foundation specifications | Pending |
| 1.0.0 | 2026-08-03 | Approved | Approved the duplicate-safe audio root, independent music/SFX/ambience services, DSP-aware music scheduling, pooled SFX voices, generational handles, deterministic concurrency, mixer routing, profile schema model, diagnostics, tooling, and isolated Audio Laboratory | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** Resonance - Audio Runtime  
**Technical identifier:** Jukebot  
**Flavor line:** Tune the music, effects, ambience, and mix without binding the game to one genre.  
**Plain-language subtitle:** Persistent music, sound-effect, ambience, routing, playback-handle, and audio-runtime infrastructure.

**One-sentence ownership contract:**

> Jukebot owns runtime music, sound-effect, ambience, voice-pool, playback-handle, and mixer-routing execution; it does not own global preference persistence, gameplay decisions about when sounds occur, UI navigation, application state, scene-travel rules, save files, project audio content, or the rules that create semantic gameplay events.

### 1.1 Elevator summary

Resonance provides one reliable audio authority that can be installed into a clean Unity project and used immediately without First Light, The Accord, The Pulse, The Looking Glass, or The Observatory. It supplies a duplicate-safe persistent root, independent music, SFX, and ambience players, two-source music transitions, pooled 2D and positional voices, immutable cue definitions, stoppable looping handles, mixer routing, structured results, and an Audio Laboratory that proves the full MVP in isolation.

The package intentionally separates **what a sound means** from **how it is played**. A game, UI, character controller, objective system, or project adapter decides that a jump, menu selection, weapon fire, door opening, victory, or environmental change occurred. Jukebot receives a semantic audio request and applies the configured clip selection, variation, volume, pitch, spatialization, concurrency, cooldown, routing, and lifetime policy.

Jukebot is an intentional naming exception inside The Sperk’s Forge. Its technical name and public identity already communicate the product clearly, while the public package listing uses the Resonance title to fit the suite’s Verse-flavored presentation.

### 1.2 Why this belongs in The Sperk’s Forge

Audio infrastructure has been rebuilt repeatedly across Rescuers2D, Don’t Get Vince’d, Echo Systems Lab, DeverQuest, and other Unity projects. Existing implementations proved the value of persistent playback, music transport, semantic cue assets, playlists, ambience, mixer control, and audio requests. They also exposed recurring failure patterns:

- several persistent audio managers surviving the same scene transition;
- music and ambience sharing transport state accidentally;
- Play, Stop, Previous, and Next operations allowing several tracks to continue simultaneously;
- mutable playlist and cue state living in shared assets;
- user volume persistence becoming tangled with the player that applies it;
- scene names or game states deciding music inside the audio package;
- one-shot playback that cannot be identified, stopped, diagnosed, or constrained;
- package content depending on project clips or proprietary audio libraries;
- samples that work only because unrelated project code is present.

Resonance preserves the successful data-driven and event-driven patterns while replacing hidden coupling with one authoritative root, owned child services, explicit requests, bounded runtime state, deterministic arbitration, clean package data, and removable integrations.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | “Resonance” must be paired with “Audio Runtime” in formal surfaces. |
| Setup guidance/tooltips | Yes | Flavor may discuss tuning or resonance, but every action must remain technically explicit. |
| Samples | Optional | Verse presentation must be replaceable and removable. |
| Runtime API/type names | No lore-only names | Types describe tracks, cues, profiles, voices, handles, routing, and results directly. |
| Project data | No required Hackulos content | Consumer projects own clips, music, ambience, semantic events, and presentation identity. |

---

## 2. Problem Statement

### 2.1 Current problem

Unity supplies capable audio primitives, but project-level audio behavior is commonly assembled through scattered `AudioSource` components and scene scripts. Without one runtime authority:

- persistent music can duplicate after scene loads;
- one transport button may control the wrong channel or leave hidden sources playing;
- music, ambience, and SFX can compete for the same source or state;
- overlapping one-shots are difficult to identify or stop individually;
- unlimited playback requests can exhaust voices or produce clipping;
- cooldown, variation, and concurrency behavior is copied into gameplay scripts;
- looping sounds may survive destroyed owners or lose their stop reference;
- audio volume and mute values may be persisted by one project script and applied by another;
- crossfades can become frame-dependent or race with rapid track changes;
- definition assets can retain runtime indexes, timestamps, or shuffle state between sessions;
- scene-specific audio decisions enter the package and damage reuse;
- configuration failures remain silent until the game produces no sound.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Persistent audio work exposed bootstrap conflicts, direct-scene needs, character/environment cue families, and menu consistency concerns | Persistent playback, semantic profiles, isolated replacement | One root, owned children, no project scene or character assumptions |
| Don’t Get Vince’d | Combat, dialogue, boss, pickup, and feedback events need layered but decoupled audio | Event-driven requests | Keep combat and gameplay rules outside the audio authority |
| Echo Systems Lab | Audio definitions, requests, managers, and feedback listeners demonstrate focused components | Definition/runtime/presentation separation | Package boundaries, stable IDs, assembly isolation, Test Lab proof |
| DeverQuest | Music playlists, ambience profiles, transport controls, and two-channel previewing revealed overlapping playback and transport defects | Playlists, independent channels, preview tooling | Deterministic transport state, cancellation, tests, no Editor-product data at runtime |
| First Light v1.0.0 | Startup can initialize selected services and may present splash audio later | Explicit startup integration | Jukebot remains independently initializable |
| Observatory v1.0.0 | Runtime health needs provider snapshots and bounded history | Structured diagnostics | Separate bridge rather than mandatory dependency |
| Accord v1.0.0 | Global audio preferences require transactional persistence | Settings authority and typed sections | Jukebot applies values but never persists them |
| Passage v1.0.0 | Scene travel may request audio transitions | Serialized transition lifecycle | Project or bridge selects profiles; Jukebot never maps scene names itself |
| Pulse v1.0.0 | Pause and mode changes need neutral audio coordination | Semantic policy intent | Pulse requests; Jukebot executes according to configured bridge policy |
| Hackulos | RPG music, spells, footsteps, ambience, UI, combat, and creatures need many semantic cue groups | Composable profile families | Keep RPG content and trigger rules outside the general package |

### 2.3 Consequences of doing nothing

- Every game rebuilds music, SFX, and ambience foundations.
- Persistent audio defects recur during scene changes and direct-scene testing.
- Unlimited or untracked playback becomes difficult to diagnose and optimize.
- UI, gameplay, and settings code become coupled to raw `AudioSource` objects.
- Audio definitions accumulate mutable state and become unsafe to share.
- Clean package removal and independent testing remain impossible.
- Real-project replacement work becomes risky because behavior is undocumented.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Maintain exactly one authoritative Jukebot runtime root when persistence is enabled.
- Reject duplicates before event subscriptions, source creation, mixer changes, or playback side effects.
- Keep music, SFX, and ambience as independent owned services with independent transport and state.
- Provide two-source music crossfades and deterministic rapid-request behavior.
- Use DSP-time scheduling where it materially improves music start and handoff accuracy.
- Provide pooled 2D and positional SFX voices.
- Provide generational playback handles that cannot accidentally control a reused voice.
- Support direct cue playback, weighted/random/sequential/shuffle-bag variations, volume and pitch ranges, cooldowns, per-cue and group concurrency, priorities, and deterministic voice stealing.
- Support stoppable looping sounds and owner-following positional playback.
- Support ambience profiles containing independently blended looping layers.
- Route music, SFX, ambience, voice, and UI buses through project-owned mixer bindings.
- Apply normalized volume and mute values without persisting preferences.
- Keep cue, track, playlist, profile, and routing assets immutable at runtime.
- Keep mutable indexes, histories, timestamps, queues, handles, and active voice state in the runtime root.
- Expose structured results, snapshots, events, and stable diagnostic codes.
- Provide repeatable setup, validation, repair, preview, stress testing, and an isolated Audio Laboratory.
- Remain useful without any peer Sperk’s Forge package.

### 3.2 Non-goals

- Jukebot does not decide that the player jumped, attacked, entered a zone, changed scenes, won, lost, opened a menu, or selected a dialogue choice.
- It does not own settings storage or save files.
- It does not own input actions, menu navigation, pause truth, game state, scene flow, or UI presentation.
- It does not ship copyrighted or non-redistributable music and sound libraries.
- It does not author a game’s complete audio content.
- It does not replace Unity’s audio importer, Audio Mixer window, Profiler, or third-party middleware.
- It does not promise sample-accurate dynamic music composition, arbitrary stem synchronization, beat-matched transitions, procedural DSP, microphone capture, voice chat, or runtime audio editing in the MVP.
- It does not use negative pitch as a universal reverse-playback guarantee.
- It does not make Addressables, localization, Timeline, Cinemachine, or another Echo package mandatory.
- It does not create a second general object-pooling authority for non-audio objects.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project with clips | Generate a root/configuration, assign clips, and hear music, SFX, and ambience in the lab |
| Programmer | Gameplay emits semantic events | Request playback through stable APIs and receive clear results/handles |
| Designer/audio author | Clips need reusable behavior | Author tracks, cues, variations, playlists, ambience, and profile mappings without code |
| Integrator | Settings, pause, scene flow, launch, and UI must connect | Add removable bridges without changing Jukebot core |
| Tester | Audio overlaps, disappears, or will not stop | Inspect active voices, transports, routing, limits, handles, and diagnostic history |
| Maintainer | Package must ship independently | Validate clean install, upgrade, removal, GUID stability, samples, and tarball |

### 3.4 Measurable success criteria

- Installs in a clean supported Unity project with zero compile errors.
- Core runtime works with no other Sperk’s Forge package installed.
- A duplicate root cannot create sources, subscribe, change mixer values, or begin playback.
- Music transport never has more active sources than its declared two-source transition model.
- Rapid Play/Next/Previous/Stop requests produce one deterministic final transport state.
- SFX pool usage never exceeds configured capacity.
- A stale playback handle cannot stop or modify a later sound that reused the same voice.
- Concurrency, cooldown, and voice-stealing results are deterministic for identical runtime state.
- Definitions remain unchanged after Play Mode and repeated tests.
- Ambience and music transport operate independently.
- Removing settings, diagnostics, UI, launch, game-state, or scene-flow bridges does not break Jukebot.
- Standalone Audio Laboratory proves the MVP without unrelated Echo packages.
- Setup and repair are repeatable and non-destructive.
- Samples can be removed without breaking runtime assemblies.
- Configuration and runtime failures produce actionable results and diagnostic codes.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers.
- Gameplay and systems programmers.
- Audio designers and technical audio implementers.
- UI, character, combat, environment, and narrative developers making semantic audio requests.
- Testers diagnosing music, voice, routing, concurrency, and persistence defects.
- The Workshop when composing a project foundation.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Phase |
|---|---|---|---|---|---|
| UC-001 | Initialize Jukebot | Root/installer | Valid configuration; no authority | Root claims, builds players/pools, applies routing defaults, becomes Ready | MVP |
| UC-002 | Reject duplicate | Scene/root | Existing authority active | Duplicate exits before side effects | MVP |
| UC-003 | Play music track | Project code | Root Ready; valid track | Track starts using requested/default transition | MVP |
| UC-004 | Crossfade music | Project code | Music active | Incoming source schedules/starts; outgoing fades; one becomes authoritative | MVP |
| UC-005 | Stop music | Project code | Any music transport state | Pending start/crossfade cancels and both sources settle Silent | MVP |
| UC-006 | Use playlist transport | Project/UI adapter | Valid playlist | Play, pause, resume, next, previous, shuffle, and repeat remain deterministic | MVP |
| UC-007 | Play 2D SFX | Gameplay/UI code | Valid cue | Variation selected and voice starts or structured denial returned | MVP |
| UC-008 | Play positional SFX | Gameplay code | Position or follow target supplied | Pooled spatial voice starts with configured routing | MVP |
| UC-009 | Stop looping SFX | Project code | Live handle | Correct generation stops/fades and voice returns | MVP |
| UC-010 | Enforce cooldown | Rapid caller | Cue cooling down | Request denied without consuming a voice | MVP |
| UC-011 | Enforce concurrency | Rapid caller | Cue/group at limit | Reject or steal according to policy | MVP |
| UC-012 | Blend ambience profile | Environment/project adapter | Valid profile | Layer set fades independently of music | MVP |
| UC-013 | Apply bus volumes | Settings/project adapter | Mixer bindings valid | Normalized levels and mute state apply without persistence | MVP |
| UC-014 | Pause selected domains | Game-state/project adapter | Runtime Ready | Configured players pause/resume without changing game-state truth | MVP |
| UC-015 | Direct scene entry | Developer | Helper enabled; no authority | Minimal configured root initializes once | MVP |
| UC-016 | Preview asset | Designer | Editor tool open | Cue/track/profile previews without entering production scene | MVP |
| UC-017 | Stress voice pool | Tester | Audio Laboratory imported | Capacity, denial, stealing, and cleanup are visible/repeatable | MVP |
| UC-018 | Export diagnostic snapshot | Tester | Runtime Ready | Local structured audio snapshot generated with no clip data transmission | MVP |
| UC-019 | Scene profile request | SceneFlow/project adapter | Explicit mapping exists | Adapter requests music/ambience; Jukebot remains scene-agnostic | Integration |
| UC-020 | Apply saved settings | Accord bridge | EchoSettings installed | Bridge maps committed preferences into bus controls | Integration |

### 4.3 Explicitly unsupported use cases

- Scene-name-to-track mapping inside Jukebot core.
- Gameplay rules or animation events implemented inside cue assets.
- A production menu or audio-options screen owned by Jukebot.
- Arbitrary gameplay code retaining raw pool `AudioSource` references.
- Using a shared ScriptableObject as the current playlist, cooldown store, or active voice registry.
- Unlimited voice allocation.
- Guaranteeing gapless playback for clips/import settings that cannot support it.
- Runtime downloading, streaming, decrypting, or licensing of external audio in the MVP.
- Network voice chat or synchronized network playback.
- Full adaptive-music graph authoring in the first release.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Runtime audio authority and root lifecycle.
- Music transport and two-source transition execution.
- Playlist runtime order, history, shuffle, repeat, and queue state.
- SFX request validation, variation selection, voice allocation, concurrency, cooldown, and voice return.
- Looping playback handles and handle validity.
- Ambience profile/layer runtime blending.
- Project-selected mixer routing and exposed-parameter application.
- Package-local pause, mute, and domain transport execution when requested.
- Runtime audio state, histories, counters, and standalone diagnostics.
- Audio definition validation and preview tooling.
- The isolated Audio Laboratory and redistributable sample content.

### 5.2 The package does not own

- Global preferences or persistence.
- Game-save slots or progress.
- Input actions and rebinding.
- UI navigation or screen presentation.
- Runtime-state and pause authority.
- Scene-transition execution or scene-to-audio rules.
- The gameplay event that caused a request.
- Character, weapon, surface, objective, dialogue, or environment rules.
- Third-party audio middleware or project audio licenses.
- General-purpose non-audio pooling.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | Jukebot interaction |
|---|---|---|
| Startup ordering | First Light when installed, otherwise Jukebot | Optional startup-step bridge invokes idempotent initialization |
| Diagnostic dashboard | The Observatory | Optional provider bridge maps Jukebot snapshot/events |
| Global audio preferences | The Accord | Bridge applies normalized bus values and mute state |
| High-level pause/mode | The Pulse | Bridge translates neutral audio policy into domain pause/mix requests |
| Scene transitions | The Passage | Project/bridge requests transitions; Jukebot contains no scene rules |
| UI screens/navigation | The Looking Glass | UI presenters request cues and display status; Jukebot owns playback only |
| Input | The Will | Input triggers project/UI requests; no core dependency |
| Save files | The Chronicle | No MVP integration; optional project state may persist track intent later |
| Project composition | The Workshop | Generates/links selected configuration, mixer, prefab, and sample |
| Gameplay semantics | Project or gameplay package | Emits cue/profile requests through direct API or adapter |

### 5.4 Boundary tests

A capability belongs in Jukebot only when all answers remain acceptable:

1. Does it execute, constrain, route, or report audio playback?
2. Can it work without knowing the game’s scene, character, quest, combat, or menu rules?
3. Does it keep project audio content outside immutable package source?
4. Does it avoid persisting player preferences or save progress?
5. Does it avoid turning a presenter or emitter into the playback authority?
6. Can optional peers remain absent without compile or runtime failure?
7. Would a project adapter or bridge express the semantic trigger more cleanly?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

Jukebot must:

- compile with only its declared Unity dependencies;
- initialize and function without First Light;
- function without The Observatory, The Accord, The Pulse, The Passage, The Will, The Looking Glass, The Chronicle, or The Workshop;
- avoid project assembly references;
- contain no required game-specific clips or semantic rules;
- expose a direct prefab/configuration setup path;
- expose interfaces and injected clocks/factories for test substitution;
- fail visibly and safely when mixer, clip, profile, or optional collaborators are absent;
- keep Editor preview code outside runtime assemblies;
- keep samples and test utilities out of production runtime requirements.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | Runtime and Editor assemblies compile; setup tool is available | Clean-project compile test |
| Enter Audio Laboratory directly | Development initializer creates one root if absent | LAB-JB-001 |
| First Light absent | Root uses standalone initialization path | JB-T-004 |
| EchoSettings absent | Default/project-applied bus values remain active; no persistence claimed | JB-T-032 |
| EchoDiagnostics absent | Local snapshot/log surfaces remain available | JB-T-033 |
| EchoGameState absent | No automatic pause policy; direct API remains available | JB-T-034 |
| Duplicate root present | Duplicate exits before source/mixer/playback side effects | JB-T-006 |
| Required config missing | Root enters Failed safely with diagnostic code | JB-T-008 |
| Sample deleted | Runtime and Editor assemblies remain valid | Package removal test |
| Optional bridge removed | Core behavior and serialization remain intact | Bridge removal test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine core | Platform | Yes | Unity 6000.0 | MonoBehaviour, ScriptableObject, GameObject lifecycle | Package cannot function |
| Unity Audio module | Platform | Yes | Unity 6000.0 | AudioSource, AudioClip, AudioListener, AudioMixer, routing | Package cannot function |
| Unity Test Framework | Test only | Yes for package tests | Compatible with baseline | EditMode and PlayMode tests | Runtime unaffected |
| uGUI/TextMeshPro | Sample/Editor only | No | Compatible | Optional Audio Laboratory controls/readouts | Runtime core unaffected |

### 6.4 Forbidden dependencies

- Another Sperk’s Forge runtime package in Jukebot core assemblies.
- Project-specific code, static databases, scene names, tags, layers, or input assets.
- EchoSettings as the storage backend.
- EchoUI as the only error/status surface.
- EchoPool as the internal SFX voice pool.
- Editor assemblies or sample assets at runtime.
- Reflection-based peer-package discovery for normal operation.
- Non-redistributable audio content.
- Hidden `Resources` paths required for core configuration.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| CAP-001 | Duplicate-safe root | One application-session authority, owned child services | Approved | Yes | Runtime | Claim before side effects |
| CAP-002 | Idempotent initialization | Standalone or First Light-triggered initialization | Approved | Yes | Runtime | `Awaitable<JukebotInitializationResult>` |
| CAP-003 | Music transport | Play, pause, resume, stop, switch | Approved | Yes | Runtime | Explicit state machine |
| CAP-004 | Two-source crossfade | Incoming/outgoing sources with deterministic cancellation | Approved | Yes | Runtime | DSP-aware starts |
| CAP-005 | Music playlists | Queue, next, previous, shuffle, repeat | Approved | Yes | Runtime | Runtime state only |
| CAP-006 | 2D SFX | Pooled nonspatial playback | Approved | Yes | Runtime | Structured requests/results |
| CAP-007 | Positional SFX | Position or follow-target playback | Approved | Yes | Runtime | No raw source exposure |
| CAP-008 | Cue variations | Weighted random, sequential, shuffle bag, random no-repeat | Approved | Yes | Runtime/Data | Mutable selection state in runtime |
| CAP-009 | Loop handles | Stop, fade, query, and invalidation | Approved | Yes | Runtime | Generational handle |
| CAP-010 | Cooldowns | Per-cue minimum interval | Approved | Yes | Runtime | DSP/unscaled clock |
| CAP-011 | Concurrency | Cue and group limits | Approved | Yes | Runtime/Data | Reject or steal |
| CAP-012 | Voice stealing | Deterministic priority/audibility/age policy | Approved | Yes | Runtime | Bounded pool |
| CAP-013 | Ambience profiles | Independent looping layers and crossfades | Approved | Yes | Runtime/Data | Separate transport |
| CAP-014 | Mixer routing | Project-owned mixer/group/parameter bindings | Approved | Yes | Runtime/Data | No persistence |
| CAP-015 | Bus volume/mute API | Master, music, SFX, ambience, voice, UI | Approved | Yes | Runtime | Normalized values |
| CAP-016 | Pause domains | Explicit music/SFX/ambience/UI/voice pause requests | Approved | Yes | Runtime | Does not own pause truth |
| CAP-017 | Audio profiles | Schema-backed semantic slot maps and composition | Approved | Yes | Data/Editor | Generic hybrid model |
| CAP-018 | Setup/repair | Create, validate, preview, repair without overwrite | Approved | Yes | Editor | Dry-run/report |
| CAP-019 | Asset preview | Track, cue, playlist, ambience, profile preview | Approved | Yes | Editor | Preview lifecycle isolated |
| CAP-020 | Audio Laboratory | Standalone runtime proof and stress scene | Approved | Yes | Sample | No peer packages |
| CAP-021 | Structured diagnostics | State snapshot, counters, events, codes | Approved | Yes | Runtime/Editor | Observatory optional |
| CAP-022 | Random ambience one-shots | Profile-driven intermittent one-shots | Deferred | No | Runtime/Data | Later minor release |
| CAP-023 | Intro/loop/outro tracks | Segmented music definitions | Deferred | No | Runtime/Data | Requires deeper import/scheduling tests |
| CAP-024 | Loop regions | Sample-accurate custom loop regions | Deferred | No | Runtime/Data | Platform/import caveats |
| CAP-025 | Adaptive stems | Synchronized layered music states | Deferred | No | Runtime/Data | Dedicated design checkpoint |
| CAP-026 | Mixer snapshots/ducking graph | Named mix states and snapshot blending | Deferred | No | Runtime/Data | Avoid conflict with exposed volumes |
| CAP-027 | Addressables/provider clips | Async clip provider abstraction | Deferred | No | Adapter | Core uses direct AudioClip refs |
| CAP-028 | Surface providers | Physics/material-to-profile adapters | Deferred | No | Bridge/project | Gameplay/environment ownership |
| CAP-029 | Reverse playback | Platform-dependent reverse clip support | Experimental/Deferred | No | Runtime | No release guarantee |

### 7.2 MVP capability set

The smallest complete release includes:

- one duplicate-safe persistent root;
- explicit or standalone idempotent initialization;
- separate music, SFX, and ambience services;
- two-source music transport with crossfade, playlist, shuffle, repeat, previous, next, pause, resume, and stop;
- pooled 2D and positional SFX;
- immutable cue/variation definitions;
- weighted/random/sequential/shuffle-bag selection;
- per-cue cooldowns;
- per-cue and group concurrency;
- deterministic rejection/voice stealing;
- generational looping handles;
- ambience profile layer blending;
- mixer routing and normalized bus volume/mute API;
- explicit audio-domain pause API;
- schema-backed semantic audio profiles;
- local diagnostics, setup, validation, repair, preview, and the Audio Laboratory;
- optional bridge specifications, but no mandatory peer dependency.

### 7.3 Later capability set

Approved later exploration includes:

- randomized ambience events;
- intro/loop/outro music segments;
- validated custom loop regions;
- layered adaptive music and stems;
- named mixer states, ducking, and snapshot blending;
- Addressables or custom clip-provider adapters;
- surface/material and animation-event adapters;
- Timeline/cutscene integration;
- localization-aware voice/audio assets;
- platform-specific resource and voice limits;
- richer waveform, loudness, and import analysis tooling.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| One universal AudioManager script | Rejected | Hides independent responsibilities and grows into a god object | Never without new suite ADR |
| Mutable cue/playlists assets | Rejected | Cross-session and concurrent-consumer contamination | Never |
| Scene-name music database in core | Rejected | SceneFlow/project owns scene semantics | Use project adapter |
| Save volume through PlayerPrefs | Rejected | The Accord owns global preferences | Accord bridge |
| Unlimited dynamic AudioSources | Rejected | Unbounded resource use and weak diagnostics | Never |
| Use PlayOneShot for all voices | Rejected | Cannot provide per-playback ownership/stop/concurrency guarantees | May remain preview-only |
| Reverse via negative pitch | Experimental/Deferred | Platform/import behavior is not a stable general contract | Dedicated validation |
| Full middleware replacement | Rejected | Scope and maintenance cost exceed package mission | Provider adapter may be separate |
| Beat graph/stem composer in MVP | Deferred | Would inflate first release and needs a dedicated authoring model | After core proves two games |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Jukebot configuration, tracks, playlists, cues, variations, ambience profiles, profile schemas, mixer bindings, policies | Active voice ownership, current indexes, timestamps, queues, handles, source instances |
| Runtime state/behavior | Root, music transport, voice pool, cue runtime state, ambience state, routing applier, clocks, handles, snapshots | Editor preview code, settings persistence, scene/game rules, production UI |
| Presentation/feedback | Editor windows, Audio Laboratory, optional debug presenter | Playback authority or project gameplay truth |

### 8.2 Component topology

```text
JukebotRoot
├── JukebotRuntimeContext
│   ├── IDspClock / IUnscaledClock
│   ├── IAudioSourceFactory
│   ├── IMixerParameterApplier
│   └── JukebotEventBuffer
├── MusicPlayer
│   ├── MusicSource A
│   ├── MusicSource B
│   └── MusicTransportState
├── SfxPlayer
│   ├── SfxVoicePool
│   ├── CueRuntimeStateRegistry
│   └── ConcurrencyRegistry
├── AmbiencePlayer
│   ├── AmbienceLayerVoicePool
│   └── AmbienceRuntimeState
├── AudioBusController
└── JukebotDiagnosticsState

Project code / emitters / bridges
        │ explicit requests
        ▼
IJukebotService
        │
        ├── music request/result/events
        ├── SFX request/result/handle/events
        ├── ambience request/result/events
        └── bus/pause request/result/events
```

The child services are ordinary objects or components owned by the root. They are not independent persistent singletons. The root is the only persistent authority and exposes a documented convenience accessor plus an injectable interface.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | Yes for the standard runtime; isolated nonpersistent test construction remains supported |
| Root type | `JukebotRoot` |
| Duplicate behavior | First valid claimant wins; duplicates disable/destroy themselves before any source, mixer, subscription, or playback side effect |
| Initialization trigger | `Awake` claims only; explicit `InitializeAsync` or standalone `Start` performs initialization |
| Default lifetime | Application session via `DontDestroyOnLoad` |
| Shutdown behavior | Stop/cancel transports, invalidate handles, return/destroy owned voices, clear state/subscriptions, release authority |
| Direct-scene behavior | Development helper creates configured root only when absent |
| Test injection seam | `IJukebotService`, source factory, clocks, random source, mixer applier, and runtime constructor seams |

### 8.4 Lifecycle sequence

1. **Construct/deserialize** - obtain configuration reference without mutating audio state.
2. **Claim authority** - reject duplicates immediately.
3. **Preflight validation** - verify configuration, source templates, mixer bindings, pool limits, and defaults.
4. **Create runtime context** - clocks, random source, factories, event buffer, and state registries.
5. **Create owned services** - music, SFX, ambience, and bus controller.
6. **Build/prewarm pools** - allocate the configured initial voice counts without playback.
7. **Apply routing/default levels** - occur no earlier than the safe post-`Awake` initialization path.
8. **Ready** - accept requests and publish initialization snapshot/event.
9. **Normal operation** - validate requests, arbitrate voices, update transitions/follow targets, publish bounded status.
10. **Scene changes** - persistent root and active policy continue unless explicitly requested otherwise.
11. **Suspend/focus changes** - no automatic project policy beyond documented Unity lifecycle handling; bridges/project decide behavior.
12. **Shutdown** - reject new work, stop/cancel, invalidate handles, release voices and registrations, restore package-owned preview state, clear authority.

### 8.5 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Claim | Duplicate warning/status | Duplicate performs no side effects | `JB-ROOT-001` |
| Missing configuration | Preflight | Blocking setup error | Root enters Failed, no playback | `JB-CFG-001` |
| Invalid pool capacity | Validation | Error with field | Clamp only in explicit repair; runtime fails safe | `JB-CFG-002` |
| Missing AudioClip in track/cue | Asset validation/request | Request rejected | Existing audio continues | `JB-ASSET-001` |
| Missing mixer/group | Initialization | Warning or error by requiredness | Fallback direct routing when approved | `JB-MIX-001` |
| Missing exposed parameter | Apply bus value | Structured failure | Previous value remains | `JB-MIX-002` |
| Music request during transition | Request | Deterministic replace/queue/reject result | Current transport remains coherent | `JB-MUS-001` |
| No available SFX voice | Allocation | Rejected or stolen result | No unbounded source creation | `JB-SFX-001` |
| Cue cooldown active | Request | Denied result with remaining time | No voice consumed | `JB-SFX-002` |
| Concurrency limit reached | Request | Reject/steal result | Existing voices follow policy | `JB-SFX-003` |
| Stale handle used | Handle operation | False/invalid result | Reused voice unaffected | `JB-HND-001` |
| Follow target destroyed | Update | Voice detaches/stops per cue policy | Voice returns safely | `JB-SFX-004` |
| Ambience profile invalid | Request | Rejected with details | Current profile continues | `JB-AMB-001` |
| Diagnostic listener fails | Event publish | Development warning | Audio operation still completes | `JB-DIAG-001` |
| Shutdown during request | API | Rejected/Cancelled result | Cleanup completes | `JB-LIFE-001` |

### 8.6 Unity implementation basis

The MVP uses Unity audio primitives behind package-owned abstractions rather than exposing them as the public architecture:

- Unity documents `AudioSource.PlayScheduled` as the preferred way to stitch music clips because it uses the absolute DSP timeline, is independent of frame rate, and gives the audio system preparation time. Jukebot therefore uses an injected DSP clock and scheduled starts where they improve track handoff, while avoiding an unqualified promise of gapless playback for every clip/import/platform combination. See [Unity 6 AudioSource.PlayScheduled](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AudioSource.PlayScheduled.html).
- Unity documents that `AudioMixer.SetFloat` can fail when a parameter is unavailable and that calling it during early lifecycle events such as `Awake` or `OnEnable` can produce unexpected behavior. Jukebot therefore claims authority in `Awake` but applies mixer values during its later initialization phase, and every apply operation returns a structured result. See [Unity 6 AudioMixer.SetFloat](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Audio.AudioMixer.SetFloat.html).
- Unity documents that `AudioSource.PlayOneShot` can overlap existing playback on the same source. Because Jukebot promises individual handles, concurrency limits, deterministic stopping, and voice ownership, the production SFX path uses explicit pooled voices rather than treating one-shot overlap as the package authority. See [Unity 6 AudioSource.PlayOneShot](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AudioSource.PlayOneShot.html).

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `JukebotConfiguration` | Root policies, pool sizes, defaults, routing, initial profile references | Yes | No | Yes |
| `MusicTrack` | Clip, display metadata, routing, loop, default fades, gain | Yes | No | Yes |
| `MusicPlaylist` | Ordered track references and default playback policy | Yes | No | Yes |
| `SfxCue` | Selection, variation, spatial, concurrency, cooldown, priority, routing, lifetime policy | Yes | No | Yes |
| `SfxVariation` | Clip and per-variation weight/gain/pitch offsets | Optional child ID | No | Contained/project-owned |
| `AmbienceProfile` | Looping layer definitions and transition defaults | Yes | No | Yes |
| `AmbienceLayerDefinition` | Clip, gain, routing, loop, optional spatial anchor policy | Optional child ID | No | Contained/project-owned |
| `AudioMixerRoutingConfiguration` | Mixer/group refs, exposed parameter bindings, ranges, requiredness | Yes | No | Yes |
| `AudioProfileSchema` | Stable semantic slot definitions such as UI or movement | Yes | No | Package template or project-owned |
| `AudioProfile` | Project mapping from schema slots to cues | Yes | No | Yes |
| `AudioProfileSet` | Composition of several profiles with conflict policy | Yes | No | Yes |
| `MusicTransitionPolicy` | Fade durations/curves and replacement behavior | Yes/embedded | No | Project-owned |
| `VoicePoolConfiguration` | Initial/max voice counts and exhaustion policy | Embedded | No | Project-owned |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `JukebotRuntimeState` | Root | Application session | Recreated at initialization | Not saved by core |
| `MusicTransportState` | MusicPlayer | Application session | Clears on shutdown/reset | Not stored in `MusicTrack` |
| `PlaylistSessionState` | MusicPlayer | Active playlist session | Rebuilt when playlist changes | Not stored in `MusicPlaylist` |
| `MusicSourceState[2]` | MusicPlayer | Runtime | Reset after source becomes idle | Not serialized |
| `CueRuntimeState` | SfxPlayer | Root session per cue ID | Clears on reset/shutdown | Holds indexes, bag, timestamps, counts |
| `SfxVoiceState` | SfxVoicePool | One allocation generation | Reset on return | Never exposed as durable data |
| `ConcurrencyGroupState` | SfxPlayer | Root session | Clears as voices finish/reset | Not serialized |
| `AmbienceRuntimeState` | AmbiencePlayer | Root session | Replaced/faded by profile requests | Not stored in profile assets |
| `AudioBusRuntimeState` | BusController | Root session | Defaults then explicit values | Preferences persisted elsewhere |
| `PlaybackHandleRegistry` | Root/SfxPlayer | Root session | Generation increments per reuse | Not serialized |
| `JukebotEventBuffer` | Root | Root session, bounded | Clears on reset/shutdown | Export snapshot only |

### 9.3 Stable identifiers

- Public definition assets use serialized lowercase GUID-style IDs generated by Editor tooling.
- IDs are independent of asset names, file paths, display names, and clip names.
- Empty and duplicate IDs are blocking validation errors for assets used by configuration or profiles.
- Renaming or moving an asset preserves its ID and Unity GUID.
- Released ID changes require an alias/migration map when diagnostic, profile, project-save, or external references may exist.
- Child variation/layer entries may use parent-relative IDs when they need diagnostic identity.
- Runtime registries key mutable state by stable definition ID plus runtime instance/generation where needed.

### 9.4 ScriptableObject safety

Definitions remain immutable during Play Mode. The following values are explicitly forbidden in shared assets:

- current playlist index;
- shuffle order/history;
- sequential variation index;
- shuffle-bag contents;
- cooldown timestamps;
- active voice counts;
- current crossfade progress;
- current ambience weights;
- current normalized volumes;
- runtime source references;
- playback handles or generations;
- follow targets.

Editor preview state belongs to the preview service, not the asset. PlayMode tests compare serialized definition state before and after stress operations.

### 9.5 Serialization and migration

Jukebot core does not persist live playback state in the MVP. Definition/configuration assets use an integer schema version where migration is necessary.

- Editor migration tools operate on project-owned assets only after preview and explicit confirmation.
- Backups are created before destructive schema conversion when practical.
- Unknown newer asset schemas are reported and not silently rewritten.
- Removed optional profile schemas do not delete project profile assets.
- EchoSettings persists user bus preferences; its bridge reapplies them after Jukebot initialization.
- A later save/resume integration may persist project-defined music intent, but not raw `AudioSource.time`, voice handles, or active transient SFX by default.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `IJukebotService` | Interface | Unified read/request surface for music, SFX, ambience, buses, and status | Implemented by root/runtime facade |
| `JukebotRoot` | Sealed MonoBehaviour | Claim authority, own services/lifecycle, expose facade | Prefab/setup tool/project |
| `JukebotConfiguration` | ScriptableObject | Project configuration | Project asset |
| `JukebotInitializationState` | Enum | Uninitialized, Initializing, Ready, Failed, ShuttingDown, Shutdown | Runtime |
| `JukebotInitializationResult` | Struct | Initialization outcome and diagnostics | Returned by root |
| `MusicTrack` | ScriptableObject | Immutable track definition | Project asset |
| `MusicPlaylist` | ScriptableObject | Immutable playlist definition | Project asset |
| `MusicPlayRequest` | Struct | Track/playlist and transition request | Caller |
| `MusicRequestResult` | Struct | Accepted/rejected/replaced result | MusicPlayer |
| `MusicTransportSnapshot` | Struct | Read-only transport state | MusicPlayer |
| `MusicTransportState` | Enum | Silent, Starting, Playing, Paused, Crossfading, Stopping, Failed | Runtime |
| `SfxCue` | ScriptableObject | Immutable cue definition | Project asset |
| `SfxPlayRequest` | Struct | Cue, position/follow target, overrides, context | Caller |
| `SfxPlayResult` | Struct | Accepted/denied plus handle and reason | SfxPlayer |
| `SfxPlaybackHandle` | Readonly struct | Generational control/query token | SfxPlayer |
| `SfxDenialReason` | Enum | Missing cue, cooldown, concurrency, capacity, shutdown, invalid request | Runtime |
| `AmbienceProfile` | ScriptableObject | Immutable layer definition | Project asset |
| `AmbienceRequest` | Struct | Profile and transition request | Caller |
| `AmbienceRequestResult` | Struct | Accepted/rejected result | AmbiencePlayer |
| `AudioBusId` | Serializable value | Stable bus identity | Routing config/API |
| `AudioBusValue` | Struct | Normalized level/mute pair | Caller/runtime |
| `AudioPauseDomain` | Flags enum | Music, SFX, Ambience, Voice, UI, All | Runtime/API |
| `AudioPauseLease` | Readonly struct/IDisposable | Idempotent reason-based domain pause token | Root |
| `JukebotSnapshot` | Struct/class DTO | Structured current status and counters | Root |
| `AudioProfileSchema` | ScriptableObject | Semantic slot contract | Package/project asset |
| `AudioProfile` | ScriptableObject | Slot-to-cue mapping | Project asset |
| `AudioProfileResolver` | Runtime service | Resolve semantic key through composed profiles | Root/project adapter |
| `JukebotResultCode` | Value/enum | Stable operation code | Runtime |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure | Loop rule |
|---|---|---|---|---|
| `JukebotRoot.Current` | Convenience authority access | Root claimed | Null when absent; never creates hidden root | Main thread |
| `InitializeAsync()` | Idempotently initialize | Claimed root | Existing result returned if complete; concurrent callers share operation | Main thread; Unity `Awaitable` |
| `State` | Read initialization state | None | Read-only | Main thread |
| `Snapshot` | Capture bounded current state | Initialized or failed | Safe snapshot; unsupported fields explicit | Main thread |
| `PlayMusic(MusicPlayRequest)` | Start/switch/playlist request | Ready; valid definition | Structured accepted/rejected/replaced result | Main thread |
| `PauseMusic()` / `ResumeMusic()` | Direct transport control | Ready | Idempotent result | Main thread |
| `StopMusic(MusicStopRequest)` | Cancel pending and stop/fade | Ready | Completes into Silent | Main thread |
| `NextTrack()` / `PreviousTrack()` | Navigate active playlist | Playlist active | Structured no-playlist/boundary result | Main thread |
| `PlaySfx(SfxPlayRequest)` | Validate and allocate voice | Ready | Result with valid/invalid handle | Main thread |
| `Stop(SfxPlaybackHandle, float fadeSeconds)` | Stop exact generation | Live handle | False for stale/finished handle | Main thread |
| `SetHandleVolume(...)` | Adjust one live voice | Live handle | False if stale | Main thread |
| `IsPlaying(SfxPlaybackHandle)` | Query handle | None | False if stale/finished | Main thread |
| `SetAmbience(AmbienceRequest)` | Blend to profile | Ready | Structured result | Main thread |
| `StopAmbience(float fadeSeconds)` | Fade all ambience layers | Ready | Idempotent result | Main thread |
| `SetBusValue(AudioBusId, AudioBusValue)` | Apply level/mute | Ready; binding exists | Failure leaves previous known value | Main thread, not in `Awake` |
| `AcquirePause(AudioPauseDomain, string reason)` | Pause selected domains | Ready | Disposable idempotent lease | Main thread |
| `ResolveCue(AudioProfileSet, AudioCueKey)` | Resolve semantic slot | Valid profile set | Cue or structured missing/ambiguous result | Main thread |
| `ResetRuntimeState(ResetOptions)` | Development/test reset | Dev/test or explicit project permission | Stops voices, clears bounded mutable state | Main thread |
| `ShutdownAsync()` | Controlled shutdown | Any claimed state | Idempotent completion | Main thread; Unity `Awaitable` |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `Initialized` | Root | After all required services ready | Initialization result | Listener not required for completion |
| `InitializationFailed` | Root | After failure state committed | Failure snapshot | Listener exceptions isolated |
| `MusicStateChanged` | MusicPlayer | After transport state commits | Previous/current snapshot | Ordered within music service |
| `TrackChanged` | MusicPlayer | After incoming track becomes authoritative | Previous/current track IDs | Crossfade may still be finishing only when documented |
| `MusicRequestDenied` | MusicPlayer | After denial recorded | Request/result | Development/reporting use |
| `SfxVoiceStarted` | SfxPlayer | After source configured and playback accepted | Voice snapshot/handle | Not raised for denied request |
| `SfxVoiceEnded` | SfxPlayer | After voice stops and before/after return as specified | End reason and former handle ID | Handle becomes invalid deterministically |
| `SfxRequestDenied` | SfxPlayer | After denial recorded | Cue ID/reason | Bounded logging policy |
| `AmbienceChanged` | AmbiencePlayer | After target profile commits | Previous/current profile IDs | Presentation not required |
| `BusValueChanged` | BusController | After mixer application succeeds | Bus/previous/current | Persistence listener optional |
| `PauseDomainsChanged` | Root | After effective pause set changes | Snapshot | Bridge/project may observe |
| `DiagnosticEventRecorded` | Root | After bounded event write | Event DTO | Must not recurse into audio operation |
| `ShuttingDown` | Root | Before owned services stop | Snapshot | New requests already rejected |
| `ShutdownCompleted` | Root | After authority release | Result | No further events |

Events are raised only after authoritative state changes. Listener failures are isolated, reported in development, and cannot roll back completed playback operations.

### 10.4 Async and cancellation policy

- Public asynchronous lifecycle operations use Unity `Awaitable<T>` and create a fresh operation for each new lifecycle execution.
- Normal playback requests are synchronous request/result operations that schedule or configure Unity audio work.
- Music requests may replace pending scheduled starts before they become authoritative.
- Once Unity has begun playing a source, cancellation is expressed as an explicit stop/fade operation rather than pretending the audio engine never received it.
- Shutdown cancels pending transport work, rejects new requests, then stops active sources according to shutdown policy.
- Scene destruction of a follow target uses cue policy: stop, detach at last position, or continue at fixed position.
- No operation blocks the main thread waiting for clip completion.
- Direct AudioClip references are considered loaded by the project; asynchronous provider loading is deferred to an adapter specification.

### 10.5 API ergonomics

**Novice path:** create project configuration and root through the setup window, assign a track and cue, open the Audio Laboratory, and call `JukebotRoot.Current.PlayMusic(...)` or use sample request buttons.

**Programmer path:** depend on `IJukebotService`, inject clocks/random/source factories in tests, construct typed request objects, receive structured results and handles, and register explicit adapters without using global access.

Raw owned `AudioSource` components, mutable voice records, and internal pool objects are never part of the public gameplay API.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install the package through a supported route.
2. Open **Tools > EchoDevGames > Resonance > Setup**.
3. Select or create a project destination folder.
4. Select an existing AudioMixer or create the safe Jukebot mixer template.
5. Preview the configuration asset, root prefab, mixer bindings, and optional sample profile templates.
6. Apply only create-safe operations.
7. Add the root prefab to the project’s Boot scene or choose development auto-creation for the Audio Laboratory only.
8. Import/open the standalone Audio Laboratory.
9. Run the validator and save the generated setup report.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report |
|---|---|---|---:|---|---|
| Create configuration | `JukebotConfiguration` | None | Yes, detects existing | Undo/create-only | Paths and IDs |
| Create root prefab | Root and owned source templates | None | Yes | Undo/create-only | Components/config refs |
| Create mixer template | Mixer, groups, exposed volume params | None | Yes, never overwrites | Create-only | Group/parameter list |
| Bind existing mixer | Routing asset | No mixer edits unless explicit | Yes | Undo for routing asset | Validation findings |
| Create profile templates | Schemas and empty project profiles | None | Yes | Create-only | Assets created/skipped |
| Add root to scene | Scene instance/prefab ref | Selected scene | Yes, duplicate aware | Unity Undo | Scene changes |
| Repair missing owned children | Prefab child services/sources | Selected root/prefab | Yes | Backup/Undo | Exact changes |
| Regenerate IDs | Only explicitly selected invalid assets | Project assets | Conditional | Backup | Old/new map |
| Run validation | Report only | Nothing | Yes | Not applicable | Structured report |

No operation silently replaces a project mixer, clips, cues, playlists, profiles, prefabs, or scene content.

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Resonance Setup | Installer | Create/configure root, routing, defaults, sample schemas | No |
| Resonance Validator | Maintainer/tester | Validate assets, mixer, root, pools, IDs, samples, build readiness | No |
| Music Transport Lab | Designer/tester | Preview tracks, playlists, fades, rapid transport commands | No production dependency |
| SFX Cue Previewer | Designer | Preview variation selection, pitch/gain, cooldown/concurrency simulations | No |
| Voice Pool Stress Tool | Tester | Generate controlled bursts and inspect arbitration | No |
| Ambience Profile Previewer | Designer | Blend profiles/layers independently from music | No |
| Audio Profile Inspector | Designer | Resolve schema slots and profile composition conflicts | No |
| Runtime Audio Monitor | Developer | Inspect active transports/voices/buses/handles/events | Runtime read-only |
| Setup Report Viewer | Maintainer | Review generated/modified/skipped items | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| `JB-VAL-001` | Missing configuration | Blocker | Yes | Create only |
| `JB-VAL-002` | Duplicate root in scene/prefab plan | Error | Yes | No, user selects authority |
| `JB-VAL-003` | Empty/duplicate stable ID | Error | Yes | Only new/unreleased assets |
| `JB-VAL-004` | Missing required track/cue clip | Error | No | No |
| `JB-VAL-005` | Empty optional cue variation | Warning | Yes | Remove only with confirmation |
| `JB-VAL-006` | Initial voice count exceeds max | Error | Yes | Clamp only with confirmation |
| `JB-VAL-007` | Required mixer group missing | Error | Yes | Create only in generated mixer |
| `JB-VAL-008` | Exposed parameter missing | Error | Guidance/manual | No |
| `JB-VAL-009` | Exposed volume parameter conflicts with approved snapshot policy | Warning/Error | Guidance | No |
| `JB-VAL-010` | Music track uses invalid fade/loop values | Error | Yes | Clamp with confirmation |
| `JB-VAL-011` | Concurrency group ID empty while limit enabled | Error | Yes | Generate explicit ID |
| `JB-VAL-012` | Cue max voices exceeds pool max | Warning/Error | Guidance | No |
| `JB-VAL-013` | 3D cue uses stereo clip/import pattern that may spatialize poorly | Warning | Guidance | No |
| `JB-VAL-014` | Profile missing required schema slot | Warning/Error by schema | No | No |
| `JB-VAL-015` | Profile set has ambiguous duplicate slot | Error | No | No |
| `JB-VAL-016` | Sample references project/nonredistributable clip | Blocker | No | No |
| `JB-VAL-017` | Runtime assembly references Editor/sample assembly | Blocker | No | No |
| `JB-VAL-018` | Definition changed by PlayMode test | Blocker | Investigate | No |
| `JB-VAL-019` | Configuration relies on hidden Resources path | Error | Guidance | No |
| `JB-VAL-020` | Missing current documentation/status link | Warning | Yes | Create/link only |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

MVP-supported routes:

- embedded package development;
- local path reference;
- distributable tarball;
- Git URL/tag installation;
- The Workshop selection after its specification/implementation is approved.

Registry distribution is not required for the first release.

### 12.2 Minimal scene setup

Production minimum:

1. One project-owned `JukebotConfiguration`.
2. One `JukebotRoot` prefab or scene object referencing it.
3. One valid mixer-routing configuration or an explicitly approved direct-output fallback.
4. At least one valid track/cue/profile for the feature being used.
5. One AudioListener supplied by the project’s camera/listener architecture.

Jukebot does not create or arbitrate the game’s AudioListener in core. Validation reports zero or several enabled listeners where observable, but the project owns listener placement and camera behavior.

### 12.3 Boot-scene setup

Normal standalone production setup places the root prefab in the canonical Boot/preload scene. `Awake` claims authority only; `Start` initializes when no explicit initializer has already done so. When First Light is installed, a separate startup-step bridge may invoke `InitializeAsync` and verify the result before handoff.

The root persists across scene transitions by default. Scene-loaded roots lose the duplicate claim before they can build sources or play audio.

### 12.4 Direct-scene setup

`JukebotDirectSceneInitializer` is a development/sample helper:

- it checks for an existing authority;
- creates only the configured Jukebot root when absent;
- clearly records development initialization in the snapshot;
- uses the same duplicate claim and initialization API as production;
- may be disabled through configuration and excluded from release builds;
- never creates settings, UI, game-state, diagnostics, or scene-flow authorities.

### 12.5 Scene isolation rule

The Audio Laboratory contains only Jukebot, Unity dependencies, sample-local controls/readouts, redistributable generated tones/noises or owned clips, and simple spatial test geometry. It contains no First Light, Observatory, Accord, Passage, Pulse, Will, Looking Glass, Chronicle, Workshop, or game-specific code.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Test Lab purpose

The **Resonance Audio Laboratory** proves that Jukebot can initialize, play, transition, constrain, route, pause, diagnose, reset, and shut down entirely by itself. It must make hidden state visible enough that transport races, pool exhaustion, stale handles, ambience/music coupling, and mixer failures can be reproduced deliberately rather than inferred by ear alone.

### 13.2 Required Test Lab contents

- setup instructions visible in-scene and in a sample README;
- one sample mixer with Master, Music, SFX, Ambience, Voice, and UI groups;
- sample tracks safe for redistribution;
- cues covering weighted, sequential, shuffle-bag, loop, cooldown, and concurrency behavior;
- one positional test source and movable listener/object;
- two ambience profiles with at least two layers each;
- one profile schema and composed profile set;
- music transport controls including rapid scripted command sequences;
- SFX burst/stress controls;
- bus level/mute controls;
- pause-domain controls;
- runtime status, active voices, handle generations, denial reasons, and bounded event readout;
- duplicate-root injection control;
- missing/invalid configuration simulations;
- reset and shutdown/reinitialize controls;
- no peer Echo packages.

### 13.3 Test Lab acceptance checklist

| Test | Action | Expected result | Type | Status |
|---|---|---|---|---|
| LAB-JB-001 | Enter lab directly | Exactly one development-initialized Ready root | Manual/PlayMode | Not run |
| LAB-JB-002 | Inject duplicate root | Duplicate reports and produces no source/playback changes | Manual/PlayMode | Not run |
| LAB-JB-003 | Play track A | One music source becomes authoritative | Manual | Not run |
| LAB-JB-004 | Crossfade A to B | A fades out, B fades in, final state B only | Manual/PlayMode | Not run |
| LAB-JB-005 | Spam Next/Previous/Stop | Final state matches last accepted request; no hidden tracks | Manual/PlayMode | Not run |
| LAB-JB-006 | Pause/resume music | Position resumes without starting duplicate source | Manual | Not run |
| LAB-JB-007 | Run sequential cue | Variation order repeats according to definition | Manual/Unit | Not run |
| LAB-JB-008 | Run shuffle-bag cue | Every eligible variation plays before bag rebuild | Manual/Unit | Not run |
| LAB-JB-009 | Trigger cooldown burst | Denials occur before voice allocation | Manual/Unit | Not run |
| LAB-JB-010 | Reach cue concurrency | Configured reject/steal policy occurs deterministically | Manual/PlayMode | Not run |
| LAB-JB-011 | Exhaust entire pool | Capacity remains bounded; result explains rejection/steal | Manual/PlayMode | Not run |
| LAB-JB-012 | Stop live loop handle | Correct voice fades/stops and returns | Manual/PlayMode | Not run |
| LAB-JB-013 | Reuse voice then use stale handle | New sound is unaffected | Manual/Unit | Not run |
| LAB-JB-014 | Destroy follow target | Cue policy executes without exception/leak | Manual/PlayMode | Not run |
| LAB-JB-015 | Switch ambience profiles | Layers blend; music transport is unchanged | Manual | Not run |
| LAB-JB-016 | Stop music during ambience | Ambience remains active | Manual | Not run |
| LAB-JB-017 | Adjust/mute each bus | Only routed domain changes | Manual | Not run |
| LAB-JB-018 | Pause selected domains | Only selected services pause | Manual/PlayMode | Not run |
| LAB-JB-019 | Remove exposed parameter | Structured routing failure; previous value remains | Manual | Not run |
| LAB-JB-020 | Reset lab repeatedly | No duplicate sources, subscriptions, or asset mutation | Manual/PlayMode | Not run |
| LAB-JB-021 | Shutdown with active voices | Handles invalidate; voices stop/return; authority releases | Manual/PlayMode | Not run |
| LAB-JB-022 | Reinitialize after controlled shutdown | One clean Ready runtime is created | Manual/PlayMode | Not run |
| LAB-JB-023 | Delete optional presenter/readout | Runtime playback still works | Manual | Not run |
| LAB-JB-024 | Compare assets before/after stress | Serialized definitions remain unchanged | Automated | Not run |

### 13.4 Optional showcase and integration samples

| Sample | Packages | Purpose | Why not standalone proof |
|---|---|---|---|
| First Light + Resonance | EchoLaunch, Jukebot, bridge | Initialize audio during startup and play splash/menu track | Depends on two authorities |
| Accord + Resonance | EchoSettings, Jukebot, bridge | Persist/apply audio settings | Depends on settings authority |
| Pulse + Resonance | EchoGameState, Jukebot, bridge | Apply pause/mode audio policy | Depends on game-state authority |
| Passage + Resonance | EchoSceneFlow, Jukebot, project adapter | Request scene-transition music/ambience | Includes project mapping |
| Looking Glass + Resonance | EchoUI, Jukebot, bridge/project presenter | UI cue requests and audio settings screen | UI presentation is separate |
| Rescuers2D adoption lab | Jukebot plus project adapters | Prove replacement parity | Project-specific integration |
| Don’t Get Vince’d adoption lab | Jukebot plus project adapters | Prove genre independence | Project-specific integration |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

Jukebot is primarily nonvisual. Runtime playback does not require a production UI. The package owns only:

- optional minimal development status/readout;
- Editor preview windows;
- sample Audio Laboratory controls;
- structured state that project UI or EchoUI may present.

A production audio settings menu belongs visually to EchoUI/project code, stores values through EchoSettings, and applies them through a Jukebot bridge.

### 14.2 Required states

Presenters and diagnostics must distinguish:

- Uninitialized;
- Initializing;
- Ready/Silent;
- Playing;
- Paused;
- Transitioning;
- Muted;
- Empty/no configured content;
- Voice-limited/denied;
- Warning/degraded routing;
- Failed;
- Shutting down/Shutdown.

### 14.3 Accessibility requirements

- Important gameplay information must not rely on audio alone; Jukebot exposes semantic request/result events so project UI/captions can mirror meaning.
- Voice, music, SFX, ambience, and UI buses remain independently controllable when configured.
- Mute and normalized level controls must support keyboard/controller-accessible UI through project/EchoUI presentation.
- Audio previews must not fire while a settings screen silently initializes controls.
- Reduced sensory-load projects may suppress nonessential cue groups through project/settings integration.
- Dialogue/voice cue definitions may expose caption/localization keys later, but Jukebot does not own subtitle rendering.
- Random pitch/volume variation should be bounded to avoid unexpectedly harsh output.
- Diagnostics must not rely on color alone.

### 14.4 Visual customization

All runtime UI/readouts are sample or optional presenter assets. Project visuals, fonts, layout, icons, and terminology are replaceable without changing runtime audio code.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Initialization state/result | API/Inspector/log | Editor/Development/Release-safe summary | Negligible |
| Root identity/config source | API/Inspector | All builds, path redacted as needed | Negligible |
| Music transport snapshot | API/monitor | All builds | Constant |
| Active SFX voices/pool counts | API/monitor | All builds summary | Constant/bounded |
| Cue cooldown/concurrency summary | API/monitor | Development detailed | Bounded |
| Ambience profile/layers | API/monitor | All builds summary | Bounded |
| Bus levels/mutes/routing health | API/monitor | All builds summary | Constant |
| Recent audio events/denials | Bounded buffer/export | Development by default | Configurable |
| Asset/config validation | Editor report | Editor | Manual/pre-Play/pre-build |
| Definition mutation check | Automated test | Test | Test-only |
| Voice-pool stress metrics | Audio Laboratory | Sample/development | Opt-in |

### 15.2 Structured status

`JukebotSnapshot` includes:

- package version and schema version;
- initialization state and origin (production, First Light, direct-scene, test);
- authority instance identity;
- configuration stable ID;
- music transport state, current/pending track IDs, playlist/repeat/shuffle state, and transition timing;
- active/paused/available SFX voice counts and pool limits;
- denied request counts by reason;
- active cue/group concurrency summaries bounded by configuration;
- active looping handle count;
- ambience current/target profile IDs and active layer count;
- bus values, mute flags, and routing availability;
- active pause-domain reasons/counts;
- last warning/error codes;
- bounded recent event metadata without clip binary data.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| `JB-ROOT-001` | Warning | Duplicate root rejected | Remove duplicate prefab/scene root |
| `JB-CFG-001` | Blocker | Configuration missing | Assign/create configuration |
| `JB-CFG-002` | Error | Invalid pool/capacity policy | Fix configuration and validate |
| `JB-ASSET-001` | Error | Required clip/definition missing | Assign valid asset |
| `JB-ASSET-002` | Error | Empty/duplicate stable ID | Repair ID before release |
| `JB-MIX-001` | Warning/Error | Mixer/group unavailable | Assign routing or approve fallback |
| `JB-MIX-002` | Error | Exposed parameter missing/apply failed | Fix mixer binding |
| `JB-MUS-001` | Info/Warning | Music request replaced/rejected during transition | Review admission policy/caller |
| `JB-MUS-002` | Error | Track cannot play | Validate clip/import/routing |
| `JB-SFX-001` | Info/Warning | Voice pool exhausted | Tune capacity/priority/steal policy |
| `JB-SFX-002` | Info | Cue request denied by cooldown | Expected unless caller is spamming |
| `JB-SFX-003` | Info/Warning | Concurrency limit reached | Tune limit/policy |
| `JB-SFX-004` | Info | Follow target ended | Review cue target-loss policy |
| `JB-HND-001` | Info/Warning | Stale handle operation | Release old handle references |
| `JB-AMB-001` | Error | Ambience profile invalid | Validate profile/layers |
| `JB-PRO-001` | Error | Audio profile slot missing/ambiguous | Fix schema/profile composition |
| `JB-DIAG-001` | Warning | Diagnostic listener/provider failed | Fix listener; playback remains active |
| `JB-LIFE-001` | Info/Warning | Request rejected during shutdown | Correct lifecycle ordering |

### 15.4 Observatory bridge

A separate Jukebot-Observatory bridge:

- registers a stable provider ID;
- maps `JukebotSnapshot` into neutral audio/service-health panels;
- exposes bounded music, voice, ambience, routing, and denial metrics;
- translates Jukebot events without making the sampler await playback;
- redacts project paths and sensitive labels according to Observatory privacy modes;
- removes itself cleanly when either peer is absent or shutting down.

Jukebot never references EchoDiagnostics in core.

### 15.5 Logging policy

- Stable package/category/code prefixes.
- No per-frame or per-sample logs in normal operation.
- Expected cooldown/concurrency denials are counters/events, not Console spam by default.
- Blocking configuration failures log once with an actionable fix.
- Listener exceptions are isolated and rate-limited.
- Clip names and project paths may be omitted/redacted in release snapshots.
- Development verbosity is independently configurable from release-safe summaries.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Project tracks/cues/profiles/routing | Project configuration | Project/Jukebot definitions | As Unity assets | Asset serialization |
| Current music/playlist/crossfade | Session | Jukebot runtime | No by core | None |
| Active SFX/handles/cooldowns | Session | Jukebot runtime | No | None |
| Active ambience state | Session | Jukebot runtime | No by core | None |
| Bus preference values | Global preference | EchoSettings when installed | Yes by Accord | Accord backend |
| Project-selected runtime defaults | Project config | Jukebot | Asset | Unity asset |
| Diagnostic event history | Session/development | Jukebot | Export only | Snapshot file when explicit |

### 16.2 Standalone behavior

Without EchoSettings, Jukebot uses values from project configuration and any direct runtime API requests. It does not claim those values persist across application launches. Without EchoSave, it begins with configured music/ambience intent or remains silent until requested.

### 16.3 Optional participant/provider contract

- EchoSettings bridge registers/applies the audio preference section after both authorities are ready.
- The bridge maps master/music/SFX/ambience/voice/UI normalized values and mute flags to stable bus IDs.
- Jukebot reports apply success/failure; it does not write the settings document.
- A later EchoSave/project participant may persist semantic music intent, such as a stable track/profile ID and logical playback mode, only after an integration specification approves restore semantics.
- Raw active handles, source indexes, DSP timestamps, transient SFX, and exact crossfade progress are not durable save data.

### 16.4 Failure and recovery

- Missing settings bridge: use configuration/default values.
- Unknown bus from stored settings: bridge preserves unknown settings data and reports unavailable binding.
- Mixer apply failure: previous known runtime bus value remains authoritative when possible; failure is returned.
- Missing saved track/profile in a future adapter: project fallback policy chooses silence/default and records migration issue.
- Newer settings/save data remains owned by its persistence package and is not rewritten by Jukebot.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Jukebot exposes semantic, peer-neutral audio APIs. Optional connections are explicit, removable, versioned, and owned by bridges or project adapters. Installing a peer does not silently change audio behavior.

### 17.2 Planned integrations

| Other authority | Connection | Bridge owner | Direction | Data/events | Required? |
|---|---|---|---|---|---:|
| First Light | Startup-step bridge | Separate/tiny bridge | EchoLaunch -> Jukebot | Initialize, readiness, result | No |
| Observatory | Provider bridge | Separate bridge | Jukebot -> Diagnostics | Snapshot, counters, events | No |
| Accord | Settings applier bridge | Separate bridge | Settings -> Jukebot | Bus values/mutes/apply results | No |
| Passage | Project adapter or bridge sample | Project/bridge | SceneFlow events -> project mapping -> Jukebot | Music/ambience requests | No |
| Pulse | Audio-policy bridge | Separate bridge | GameState intent -> Jukebot | Domain pause/mix requests | No |
| Will | Project/UI adapter | Project | Input action -> request | Transport/debug/sample controls | No |
| Looking Glass | UI request/settings bridges | Separate/project | UI -> Jukebot; Jukebot -> presenter | UI cues, settings, status | No |
| Chronicle | Future semantic-state participant | Separate/project | Save <-> project adapter | Stable music/ambience intent | No |
| Workshop | Editor composition | Workshop | Editor -> assets/scenes | Generate/link config, mixer, root, labs | No runtime dependency |
| Gameplay packages | Project adapters/emitters | Project or owning gameplay package | Semantic events -> Jukebot | Cue/profile requests | No |

### 17.3 Bridge placement decision

- First Light/Jukebot, Accord/Jukebot, Pulse/Jukebot, Observatory/Jukebot, and EchoUI/Jukebot integrations should be separate two-package bridges when they directly reference both public APIs.
- Tiny emitters that reference only Jukebot and Unity may ship as optional Jukebot components when they do not import another package or own game rules.
- Character, combat, surface, objective, dialogue, and scene-specific translation remains project code or belongs to the package that owns the semantic event.
- Provider/Addressables/middleware adapters ship separately.

### 17.4 Integration failure behavior

- Missing peer: bridge assembly/package is absent; Jukebot core is unchanged.
- Version mismatch: bridge fails validation and does not register partially.
- Peer initializes later: bridge registers through explicit lifecycle events or retries within a bounded policy.
- Peer shuts down first: bridge disposes registration and Jukebot continues standalone.
- Jukebot shuts down first: bridge receives/rechecks state and stops forwarding.
- Settings apply fails: EchoSettings transaction receives a structured failure and may roll back.
- Scene/profile mapping missing: project adapter reports missing mapping and preserves current audio or uses explicit fallback.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Target | Measurement | Release threshold |
|---|---|---|---|
| Duplicate claim | Before any source/pool/mixer side effect | PlayMode instrumentation | 100% of duplicate tests |
| SFX pool size | Never exceed configured maximum | Voice stress lab | Zero excess sources |
| Steady-state SFX request allocations | No managed allocation after warmup for common direct-cue request path | Profiler/automated benchmark | No recurring GC allocation in validated path |
| Runtime update cost | Bounded by active transitions/follow voices, not definition count | Audio Laboratory | No unbounded full-catalog scans |
| Diagnostic history | Fixed configurable capacity | Unit/stress tests | Never grows beyond capacity |
| Music transition sources | Exactly two owned music sources | Runtime snapshot/test | Never more than two |
| Handle lookup | Constant-time generation/index validation | Unit benchmark | No catalog scan |
| Cue state lookup | Stable-ID/reference keyed dictionary initialized once | Unit benchmark | No asset mutation |
| Stress scenario | 64 active SFX voices plus music and ambience on baseline development desktop | Profiler capture | No exceptions, leaks, or sustained package spike above documented beta budget |

Exact millisecond budgets are recorded during implementation on named hardware and become release evidence rather than being invented during specification.

### 18.2 Allocation policy

- Prewarm initial SFX and ambience voice pools.
- Pool growth, when allowed, stops at configured maximum.
- Reuse request/result/runtime records where safe without exposing mutable internals.
- Avoid LINQ, reflection, string formatting, and closure allocation in hot playback/update paths.
- Cache mixer bindings, hashes/IDs, and definition runtime state.
- Do not poll every definition each frame.
- Follow-target updates process active following voices only.
- Diagnostic detailed snapshots may allocate when explicitly requested; routine counters remain bounded/lightweight.

### 18.3 Scene and domain reload behavior

- Static authority access resets through SubsystemRegistration-compatible hooks.
- Root unsubscribes and releases all registrations on controlled shutdown/destruction.
- Enter Play Mode options with domain reload disabled are explicitly tested.
- Editor preview audio stops on assembly reload, play-mode transition, window close, and package disable.
- Definition runtime registries are rebuilt per root session.
- Duplicate cleanup leaves no static reference to destroyed objects.

### 18.4 Scalability limits

- Default initial/max SFX pool sizes are project-configured and validated.
- The package advertises tested voice counts, not unlimited voices.
- Concurrency group and active handle registries are bounded by active pool capacity.
- Music transport supports one authoritative music program with two transition sources in MVP.
- Ambience supports a configured maximum active layer count; requests exceeding it fail validation or degrade according to explicit policy.
- Very large catalogs remain authoring data and do not create runtime state until referenced/prepared.
- Platform voice virtualization and hardware limits remain Unity/platform concerns; Jukebot reports its own logical voices honestly.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Jukebot handles project asset references, mixer parameter names, runtime playback state, and local diagnostic metadata. It does not require credentials, analytics, microphone data, network data, or personal information.

Explicit diagnostic exports may contain project-defined display names, stable IDs, package version, scene name supplied by Unity, and local configuration status. Release-safe export modes redact local file paths and optionally project labels. No data is transmitted automatically.

### 19.2 Trust boundaries

- Project-authored assets are validated before runtime use.
- Stable IDs and profile keys are treated as untrusted configuration input and checked for emptiness/collision.
- Mixer parameter names are validated and failure-returning APIs are respected.
- External/provider-loaded clips are out of MVP and require a separate adapter trust model.
- Runtime callers cannot access internal AudioSources or return voices manually.
- Handle generation prevents an old caller from controlling a recycled voice.
- Diagnostic listeners cannot alter completed playback truth through exceptions.

### 19.3 Platform behavior

| Platform | Initial status | Special behavior | Validation |
|---|---:|---|---|
| Windows | Supported | Primary development and external install target | Full automated/manual lab |
| macOS | Supported target | Mixer/audio-device differences may require verification | Clean install and lab |
| Linux | Supported target | Device/backend differences | Clean install and lab |
| WebGL | Planned/conditional | Browser audio unlock, focus, streaming, and timing restrictions | Separate platform matrix before claim |
| Android/iOS | Planned/conditional | Suspend/resume, device interruption, memory/voice limits | Device tests before claim |
| Console | Unknown/planned | Certification, output device, middleware, resource policy | Provider/platform approval |

No platform is advertised beyond completed validation evidence.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.jukebot/
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
├── Editor/
├── Samples~/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── JukebotRoot.cs
│   ├── IJukebotService.cs
│   ├── JukebotRuntime.cs
│   ├── JukebotInitializationState.cs
│   ├── JukebotInitializationResult.cs
│   └── JukebotSnapshot.cs
├── Configuration/
│   ├── JukebotConfiguration.cs
│   ├── VoicePoolConfiguration.cs
│   ├── MusicTransitionPolicy.cs
│   └── AudioMixerRoutingConfiguration.cs
├── Music/
│   ├── MusicTrack.cs
│   ├── MusicPlaylist.cs
│   ├── MusicPlayer.cs
│   ├── MusicPlayRequest.cs
│   ├── MusicRequestResult.cs
│   └── MusicTransportSnapshot.cs
├── Sfx/
│   ├── SfxCue.cs
│   ├── SfxVariation.cs
│   ├── SfxPlayer.cs
│   ├── SfxVoicePool.cs
│   ├── SfxVoice.cs
│   ├── SfxPlayRequest.cs
│   ├── SfxPlayResult.cs
│   └── SfxPlaybackHandle.cs
├── Ambience/
│   ├── AmbienceProfile.cs
│   ├── AmbienceLayerDefinition.cs
│   ├── AmbiencePlayer.cs
│   └── AmbienceRequest.cs
├── Profiles/
│   ├── AudioProfileSchema.cs
│   ├── AudioProfile.cs
│   ├── AudioProfileSet.cs
│   ├── AudioCueKey.cs
│   └── AudioProfileResolver.cs
├── Routing/
│   ├── AudioBusId.cs
│   ├── AudioBusController.cs
│   ├── AudioBusValue.cs
│   └── IMixerParameterApplier.cs
├── Lifecycle/
│   ├── JukebotDirectSceneInitializer.cs
│   ├── AudioPauseDomain.cs
│   └── AudioPauseLease.cs
├── Diagnostics/
│   ├── JukebotDiagnosticCode.cs
│   ├── JukebotDiagnosticEvent.cs
│   └── JukebotEventBuffer.cs
├── Infrastructure/
│   ├── IDspClock.cs
│   ├── IUnscaledClock.cs
│   ├── IRandomSource.cs
│   ├── IAudioSourceFactory.cs
│   └── GenerationalHandleTable.cs
├── Prefabs/
│   └── JukebotRoot.prefab
└── EchoDevGames.Jukebot.Runtime.asmdef

Editor/
├── Setup/
│   ├── ResonanceSetupWindow.cs
│   ├── JukebotAssetFactory.cs
│   └── JukebotSetupReport.cs
├── Validation/
│   ├── JukebotValidatorWindow.cs
│   ├── JukebotValidationRule.cs
│   └── JukebotBuildValidator.cs
├── Preview/
│   ├── MusicTransportPreviewWindow.cs
│   ├── SfxCuePreviewWindow.cs
│   ├── AmbiencePreviewWindow.cs
│   └── EditorAudioPreviewService.cs
├── Inspectors/
│   ├── MusicTrackEditor.cs
│   ├── SfxCueEditor.cs
│   ├── AudioProfileEditor.cs
│   └── JukebotConfigurationEditor.cs
└── EchoDevGames.Jukebot.Editor.asmdef

Samples~/
└── Resonance Audio Laboratory/
    ├── README.md
    ├── Scenes/
    ├── Audio/
    ├── Configuration/
    ├── Profiles/
    └── Scripts/

Tests/
├── Editor/
│   └── EchoDevGames.Jukebot.Tests.Editor.asmdef
└── Runtime/
    ├── EditMode/
    ├── PlayMode/
    └── EchoDevGames.Jukebot.Tests.Runtime.asmdef
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.Jukebot.Runtime` | Runtime | Unity engine/audio modules only | Yes | Public runtime/data API |
| `EchoDevGames.Jukebot.Editor` | Editor | Runtime, UnityEditor | No | Setup, validation, preview, inspectors |
| `EchoDevGames.Jukebot.Tests.Runtime` | Test | Runtime, Test Framework | No | EditMode/PlayMode runtime tests |
| `EchoDevGames.Jukebot.Tests.Editor` | Editor test | Runtime, Editor, Test Framework | No | Authoring/validation/migration tests |
| Sample assembly | Sample | Runtime, optional uGUI/TMP | No | Audio Laboratory controls/readout |

### 20.4 Repository files

- concise root README and five-minute quick start;
- package specification and architecture guide;
- linked `Current Notes.md`;
- user setup, Audio Laboratory, profile authoring, routing, and troubleshooting guides;
- API examples and integration index;
- diagnostic-code reference;
- test strategy and release checklist;
- changelog, license, credits, and third-party notices;
- stable `.meta` files/GUIDs for public scripts, templates, prefabs, schemas, and samples;
- contribution/security/support guidance appropriate to release audience.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Primary tested | Notes |
|---|---|---|---|
| Unity | 6000.0 | 6000.3.8f1 | Public floor inherited from Foundation decision |
| Unity Audio module | Baseline bundled version | 6000.3.8f1 | Required |
| Unity Test Framework | Compatible baseline version | Project-resolved | Test only |
| uGUI/TMP | Compatible baseline version | Project-resolved | Sample/optional presenter only |

### 21.2 Semantic versioning policy

- **Patch:** bug fixes, diagnostics, docs, validation rules, and internal changes that preserve public API, serialized fields, stable IDs, prefab/asset identity, and behavior contracts.
- **Minor:** additive public APIs, new cue/profile policies, new optional tools/samples/bridges, or backward-compatible serialized fields with migration/defaults.
- **Major:** public API removal/change, handle semantics, root lifecycle, music/SFX/ambience authority changes, serialized schema incompatibility, routing contract changes, or behavior that requires consumer migration.

### 21.3 Deprecation policy

- Mark APIs/assets obsolete with a documented replacement for at least one minor release when practical.
- Provide migration notes and tooling for serialized project assets before removal.
- Preserve deprecated behavior only when it does not violate safety/authority rules.
- Remove in a major release unless a security/data-loss defect requires earlier action.
- Update diagnostics and examples to direct users to the replacement.

### 21.4 GUID and asset compatibility

Public scripts, root prefabs, configuration templates, mixer template, profile schemas, sample assets, and independently creatable ScriptableObject types preserve committed `.meta` files. Moves/renames retain GUIDs. Intentional replacement includes an explicit migration map and release note.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview, ownership, and non-goals.
- Supported Unity/platform matrix.
- Installation routes.
- Five-minute quick start.
- Full setup and routing guide.
- Music track/playlist authoring guide.
- SFX cue, variation, spatial, cooldown, concurrency, and handle guide.
- Ambience profile guide.
- Audio profile schema/composition guide.
- Audio Laboratory guide.
- Bus volume/mute and settings integration overview.
- Direct-scene development guide.
- Troubleshooting and diagnostic codes.
- Performance/voice-budget guidance.
- Upgrade/migration/removal guide.
- Known limitations.
- License, credits, and third-party notices.

### 22.2 Required developer documentation

- Root and owned-service architecture.
- Initialization and shutdown lifecycle.
- Music transport state machine and request replacement rules.
- Voice-pool, concurrency, stealing, and generational-handle design.
- Definition/runtime-state separation.
- Mixer binding and normalized-volume conversion policy.
- Extension interfaces and adapter examples.
- Testing strategy and baseline profiling method.
- Release workflow and adoption parity process.
- ADRs and decision log.
- Current checkpoint/status and linked Current Notes.

### 22.3 Documentation truth rule

Every code example must compile against the documented release. Setup screenshots, menu paths, inspector fields, diagnostic codes, asset names, and sample controls must match the shipped package. Audio behavior that depends on import settings or platform limitations must be stated explicitly rather than promised broadly.

### 22.4 Living repository and Obsidian workflow

Documentation lives in Git with implementation and is opened directly in Obsidian. During design/implementation:

1. capture discoveries in `Current Notes.md`;
2. label facts, proposals, questions, tests, defects, and risks;
3. promote durable package behavior into this specification or an ADR;
4. move bugs/tests into permanent records;
5. update guides/changelog for user-visible changes;
6. reconcile notes at every meaningful checkpoint;
7. commit documentation with code or in an immediately adjacent documentation commit.

### 22.5 Repository scan and handoff order

1. Repository README/index.
2. SFGSS-000.
3. This Jukebot specification.
4. Applicable ADRs and bridge specifications.
5. `Current Notes.md`.
6. Current checkpoint, tests, issue log, and changelog.
7. Relevant runtime/editor code and tests.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | MVP required? |
|---|---|---|---:|
| EditMode unit | Selection, cooldown, concurrency, handles, IDs, policies, validation | Shuffle bag, stale generation, arbitration ordering | Yes |
| PlayMode unit/integration | Root lifecycle, sources, transport, pool, scene persistence | Duplicate claim, crossfade, follow target, shutdown | Yes |
| Standalone Audio Laboratory | User-visible isolated core loop | Music/SFX/ambience/routing/stress | Yes |
| Bridge Integration Lab | Optional peer connection | Accord bus apply, Pulse pause, Observatory panel | When bridge ships |
| Showcase | Combined project shell | Startup/menu/settings/audio | No |
| Clean-project install | Package and missing-dependency proof | Git/tarball/local/embedded | Yes |
| Existing-project migration | Safe adoption/parity | Rescuers2D, Don’t Get Vince’d | Before integration claim |

### 23.2 Required test categories

- Clean compile and install.
- Missing/invalid configuration.
- Duplicate root before Play Mode and during scene load.
- Standalone and First Light initialization paths.
- Domain reload enabled/disabled.
- Music play/pause/resume/stop/switch.
- Rapid, conflicting transport requests.
- Playlist sequential/shuffle/repeat/previous history.
- DSP scheduled start and late request fallback.
- SFX 2D/positional/follow.
- Variation modes and deterministic seeded random tests.
- Cooldown before allocation.
- Cue/group concurrency.
- Voice exhaustion and stealing tie-breaks.
- Loop handle lifecycle, stale generation, double stop.
- Follow-target destruction.
- Ambience profile transitions and music independence.
- Bus routing, mute, invalid exposed parameter, and rollback.
- Domain pause nesting and out-of-order lease release.
- Definition immutability.
- Event listener exceptions.
- Reset, shutdown, reinitialize, scene persistence.
- Sample removal.
- Optional peer absent/present/removed.
- Performance and allocation budgets.
- Supported platform builds.

### 23.3 Test case registry

| Test ID | Requirement | Setup | Action | Expected | Automated? | Status |
|---|---|---|---|---|---:|---|
| JB-T-001 | Valid initialization | Root + valid config | Initialize | Ready once | Yes | Not run |
| JB-T-002 | Idempotent initialization | Ready root | Initialize twice | Same completed state/result | Yes | Not run |
| JB-T-003 | Concurrent initialization callers | Initializing root | Call twice | Shared coherent completion | Yes | Not run |
| JB-T-004 | Standalone path | No First Light | Enter scene | Root initializes in Start | Yes | Not run |
| JB-T-005 | Direct-scene helper | No root | Enter lab | One development root | Yes | Not run |
| JB-T-006 | Duplicate rejection | Active root | Add duplicate | No duplicate side effects | Yes | Not run |
| JB-T-007 | Duplicate during scene load | Persistent root + scene root | Load scene | Original remains only authority | Yes | Not run |
| JB-T-008 | Missing config | Root without config | Initialize | Failed safely, `JB-CFG-001` | Yes | Not run |
| JB-T-009 | Music play | Ready root + track | Play | Playing correct track | Yes | Not run |
| JB-T-010 | Crossfade | Track A active | Play B | Final B only, sources bounded | Yes | Not run |
| JB-T-011 | Replace transition | Crossfading A->B | Request C | Policy yields deterministic C final state | Yes | Not run |
| JB-T-012 | Stop during scheduled start | Starting track | Stop | Silent, no late playback | Yes | Not run |
| JB-T-013 | Pause/resume | Track active | Pause/resume | No duplicate/restart | Yes | Not run |
| JB-T-014 | Playlist next/previous | Playlist active | Navigate | Correct history/order | Yes | Not run |
| JB-T-015 | Shuffle reproducibility | Seeded random | Run playlist | Expected permutation/history | Yes | Not run |
| JB-T-016 | Repeat modes | Playlist boundary | Advance | None/One/All correct | Yes | Not run |
| JB-T-017 | Weighted variation | Seeded cue | Request many | Selection matches deterministic algorithm | Yes | Not run |
| JB-T-018 | Sequential variation | Sequential cue | Request sequence | Index cycles in runtime only | Yes | Not run |
| JB-T-019 | Shuffle bag | Bag cue | Request N | All entries before refill | Yes | Not run |
| JB-T-020 | Cooldown | Cooldown cue | Request rapidly | Denial before allocation | Yes | Not run |
| JB-T-021 | Cue concurrency reject | Limit reached | Request | Denied with reason | Yes | Not run |
| JB-T-022 | Cue concurrency steal | Limit reached | Higher-priority request | Correct voice stolen | Yes | Not run |
| JB-T-023 | Group concurrency | Two cues same group | Fill/request | Shared policy applies | Yes | Not run |
| JB-T-024 | Pool exhaustion | Max voices active | Request | Bounded reject/steal | Yes | Not run |
| JB-T-025 | Positional playback | Position supplied | Play | Source spatialized at position | Yes | Not run |
| JB-T-026 | Follow target | Target supplied | Move target | Voice follows active target | Yes | Not run |
| JB-T-027 | Target destroyed | Follow voice | Destroy target | Configured stop/detach behavior | Yes | Not run |
| JB-T-028 | Loop handle stop | Live loop | Stop handle | Correct generation returns | Yes | Not run |
| JB-T-029 | Stale handle | Reuse voice | Stop old handle | New voice unaffected | Yes | Not run |
| JB-T-030 | Double stop | Finished handle | Stop twice | Safe false/idempotent | Yes | Not run |
| JB-T-031 | Ambience transition | Profile A active | Set B | Layers blend, music unchanged | Yes | Not run |
| JB-T-032 | Settings absent | No Accord | Initialize | Defaults/direct API work | Yes | Not run |
| JB-T-033 | Diagnostics absent | No Observatory | Snapshot | Local snapshot works | Yes | Not run |
| JB-T-034 | GameState absent | No Pulse | Pause API | Direct leases work | Yes | Not run |
| JB-T-035 | Bus apply | Valid binding | Set value | Mixer and state update | Yes | Not run |
| JB-T-036 | Missing exposed parameter | Invalid binding | Apply | Failure, previous state retained | Yes | Not run |
| JB-T-037 | Nested pause leases | Two reasons | Release one | Domains remain paused | Yes | Not run |
| JB-T-038 | Out-of-order pause release | Three leases | Release middle/first/last | Effective set correct | Yes | Not run |
| JB-T-039 | Definition immutability | Snapshot assets | Stress playback | No serialized changes | Yes | Not run |
| JB-T-040 | Listener exception | Throwing event listener | Play cue | Playback completes, diagnostic records | Yes | Not run |
| JB-T-041 | Scene persistence | Music active | Load scene | Root/music survive once | Yes | Not run |
| JB-T-042 | Shutdown active voices | Music/SFX/ambience active | Shutdown | Handles invalid, sources stop, authority releases | Yes | Not run |
| JB-T-043 | Reinitialize after shutdown | Controlled shutdown | New root init | Clean Ready runtime | Yes | Not run |
| JB-T-044 | Domain reload disabled | Editor option | Enter/exit repeatedly | Static authority resets correctly | Yes | Not run |
| JB-T-045 | Sample removal | Package + removed sample | Compile/build | Runtime unaffected | Manual/CI | Not run |
| JB-T-046 | Tarball install | Clean project | Install artifact | Compile/lab import works | Manual/CI | Not run |
| JB-T-047 | Bridge removal | Core + removed bridge | Compile/run | Jukebot standalone works | Manual/CI | Not run |
| JB-T-048 | Performance stress | 64 voices + music/ambience | Run profile | Bounded/no leaks; budget recorded | Automated/manual | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and non-ownership approved.
- [x] MVP and deferred scope separated.
- [x] Dependencies explicit.
- [x] Public API and data model defined.
- [x] Music/SFX/ambience lifecycle and failure behavior defined.
- [x] Voice pool, concurrency, stealing, and handle semantics defined.
- [x] Audio profile data model selected.
- [x] Standalone Audio Laboratory designed.
- [x] No release-blocking design question remains for the documentation pass.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared Unity dependencies only.
- [ ] Editor code is isolated.
- [ ] Duplicate claim precedes every side effect.
- [ ] Definitions remain immutable in Play Mode.
- [ ] Music transport race tests pass.
- [ ] Voice allocation, handle, concurrency, and stealing tests pass.
- [ ] Setup/repair are repeatable and non-destructive.
- [ ] Public API matches this specification or specification/ADR changes first.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Jukebot works without unrelated Echo packages.
- [ ] Audio Laboratory passes.
- [ ] Samples remove safely.
- [ ] Direct-scene initialization behaves as documented.
- [ ] Mixer and no-peer fallback paths are documented and tested.

### 24.4 Quality gate

- [ ] Automated tests pass.
- [ ] Manual Audio Laboratory checklist passes.
- [ ] No blocker/critical defect remains.
- [ ] Performance and allocation evidence is recorded.
- [ ] Diagnostics are actionable and bounded.
- [ ] Documentation matches build/API/tools.
- [ ] Current Notes reconciled.
- [ ] Decisions promoted to specification/ADRs.
- [ ] Licenses and audio-content notices complete.
- [ ] Definition mutation test passes.

### 24.5 Distribution gate

- [ ] Manifest valid.
- [ ] Version/changelog updated.
- [ ] Stable `.meta` files included.
- [ ] Git/local/tarball installation tested externally.
- [ ] Sample contains only redistributable content.
- [ ] Repository tag/release prepared.
- [ ] Documentation/status committed and pushed.
- [ ] Central compatibility catalog updated.
- [ ] Rescuers2D and Don’t Get Vince’d integration evidence exists before broad replacement claims.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Rescuers2D | Existing persistent/project audio, menu/settings cues, character/environment audio | Install standalone, map one category at a time, keep original disabled but available | Music, SFX, ambience, settings, scene persistence, direct-scene tests | Re-enable original category/system |
| Don’t Get Vince’d | Project combat/dialogue/music/effect playback | Integrate after standalone proof to verify different genre/event patterns | Beat-’em-up music, combat cues, dialogue/feedback, scene persistence | Restore original request path |
| Echo Systems Lab | Reference architecture and portfolio evidence | Use as design/code comparison, not wholesale copy | Focused API and package isolation | No replacement required initially |
| DeverQuest | Editor-only playlist/ambience lessons | Do not import Editor productivity/guild data; reuse only general lessons | No runtime dependency introduced | Keep products separate |
| Hackulos | Future RPG consumer | Adopt after general package is stable | UI, movement, spells, combat, ambience, creature/profile composition | Project adapter rollback |

### 25.2 Preserve-until-parity rule

1. Keep the working project audio implementation available.
2. Install Jukebot and pass the Audio Laboratory alone.
3. Create project-owned configuration and adapters.
4. Replace one category at a time: music, then SFX groups, then ambience, then settings/scene/state bridges.
5. Compare functional behavior, duplicate safety, direct-scene entry, scene persistence, pause, volume, and build output.
6. Remove old code only after documented parity.
7. Preserve project clips, mixer, semantic mappings, and presentation as project-owned content.

### 25.3 Migration tooling

Initial migration tooling should:

- detect common project `AudioSource`/manager patterns without claiming universal conversion;
- inventory clips, mixers, groups, exposed parameters, scene roots, and persistent objects;
- preview proposed tracks/cues/playlists/profiles and routing assets;
- never move/delete source project assets automatically;
- create new assets beside originals;
- generate a mapping/parity report;
- validate duplicate persistent roots;
- provide rollback instructions;
- require manual review for semantic cue/profile assignment.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| R-001 | Scope expands into full middleware/adaptive composer | High | High | Enforce MVP/deferred matrix and separate design checkpoints | Any new graph/stem/DSP request |
| R-002 | Music transport race recreates overlapping-track bug | Medium | High | Explicit state machine, two sources only, replacement tests | Rapid command failure |
| R-003 | Duplicate root performs side effects before rejection | Medium | High | Awake claim-only and instrumentation tests | Any duplicate source/mixer change |
| R-004 | Shared asset stores mutable cue/playlist state | Medium | High | Runtime registries and mutation test | Serialized diff after PlayMode |
| R-005 | Voice stealing sounds inconsistent | Medium | Medium | Deterministic documented scoring/tie-breaks and seeded tests | Different result from same state |
| R-006 | Stale handles control reused voices | Medium | High | Generational handle table and tests | Old handle affects current voice |
| R-007 | Mixer exposed values conflict with snapshots | Medium | Medium | Validate/document separated gain parameters; defer snapshot system | Snapshot/volume mismatch |
| R-008 | Pool defaults are too small/large across platforms | Medium | Medium | Project configuration, stress lab, platform evidence | Denials/memory issues |
| R-009 | AudioListener ownership becomes hidden package assumption | Medium | Medium | Project owns listener; validator reports problems | Zero/multiple listener defect |
| R-010 | Setup overwrites project mixer/content | Low | High | Create-only defaults, preview, Undo/backup, explicit binding | Existing target conflict |
| R-011 | Sample audio cannot be redistributed | Low | High | Owned/generated/public-license assets and notice audit | License uncertainty |
| R-012 | Optional bridge becomes hidden hard dependency | Medium | High | Separate packages/asmdefs and removal tests | Core reference to peer |
| R-013 | DSP/import/platform differences break “gapless” claims | Medium | High | Promise scheduled/crossfade behavior, not universal gapless; test claims | Audible gap/platform failure |
| R-014 | Pause behavior conflicts with game policy | Medium | Medium | Jukebot executes explicit domains; Pulse/project owns decision | Automatic undesired pause |
| R-015 | Diagnostics or events create hot-path allocation/log spam | Medium | Medium | Bounded counters/events, rate limits, opt-in detail | Profiler/log flood |
| R-016 | Unity API/version drift | Medium | Medium | Unity 6000 floor, wrapper interfaces, compatibility testing | Upgrade warning/failure |
| R-017 | GUID breakage invalidates project assets | Low | High | Commit/preserve `.meta`, migration map | Broken refs after upgrade |
| R-018 | Existing-project replacement regresses working audio | Medium | High | Preserve-until-parity incremental adoption | Failed parity checklist |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR? |
|---|---|---|---|---|---:|
| JB-D-001 | One persistent duplicate-safe `JukebotRoot` owns ordinary music/SFX/ambience children | Approved | Prevent competing managers and lifecycle drift | Root handles claim, initialization, shutdown | No |
| JB-D-002 | `Awake` claims only; explicit/Start initialization performs side effects | Approved | Duplicate rejection must precede side effects and mixer application | Slightly staged lifecycle | No |
| JB-D-003 | Music, SFX, and ambience have independent transport/state | Approved | Prevent cross-channel control defects | Separate services and tests | No |
| JB-D-004 | Music MVP uses exactly two sources and a deterministic transport state machine | Approved | Sufficient for reliable crossfades without source proliferation | One program at a time | No |
| JB-D-005 | DSP time schedules music starts/handoffs where appropriate | Approved | Frame-rate-independent scheduling and buffering | Requires injected DSP clock/tests | No |
| JB-D-006 | SFX uses an owned bounded voice pool rather than universal `PlayOneShot` | Approved | Enables handles, limits, stop, diagnostics, deterministic cleanup | More explicit source management | No |
| JB-D-007 | Playback handles are generational | Approved | Prevent stale-handle control of reused voices | Handle table and generation checks | No |
| JB-D-008 | Cooldown/concurrency checks happen before new allocation when possible | Approved | Avoid consuming voices for denied requests | Structured denial result | No |
| JB-D-009 | Voice stealing is deterministic by configured priority, audibility estimate, age, then stable voice index | Approved | Repeatable tests and predictable behavior | Policy documented and tunable | No |
| JB-D-010 | Definitions are immutable; all mutable selection/timing/ownership state is runtime-owned | Approved | Safe sharing and repeatable tests | Runtime registries required | No |
| JB-D-011 | Audio profiles use a hybrid schema-and-instance model | Approved | Strong semantic slots without one C# type per profile family | Package ships schemas; projects map cues |
| JB-D-012 | Mixer routing is project-owned; Jukebot applies but does not persist normalized bus values | Approved | Clean boundary with The Accord | Settings bridge required for persistence | No |
| JB-D-013 | Jukebot does not own the project AudioListener | Approved | Camera/listener architecture is project-specific | Validator reports listener issues only | No |
| JB-D-014 | Mixer snapshot/ducking system is deferred | Approved | Avoid hidden conflict with exposed bus values and MVP inflation | Pulse bridge initially uses pause/domain APIs | No |
| JB-D-015 | Random ambience one-shots, segmented music, loop regions, stems, and Addressables are deferred | Approved | Preserve achievable first release | Later design milestones | No |
| JB-D-016 | Integration with peers is explicit through separate bridges/project adapters | Approved | Preserve standalone operation/removal | More small artifacts, clean dependencies | No |
| JB-D-017 | Core runtime has no uGUI/TMP dependency | Approved | Audio authority is nonvisual | Sample/editor may use UI packages | No |
| JB-D-018 | No universal “gapless playback” claim | Approved | Clip import/platform behavior varies | Claim only tested scheduled/crossfade behavior | No |

### 27.2 Release-blocking questions

None remain for the documentation pass. Implementation must still record measured defaults for pool sizes, transition curves, normalized-to-decibel mapping, and platform validation before release. Those are implementation evidence, not authority blockers.

### 27.3 Non-blocking later questions

- Which profile schemas ship in MVP beyond UI, Character Movement, Environment Actions, and a generic schema?
- Should advanced profile schemas live in Jukebot or small optional content-template packages?
- Which mixer snapshot/ducking model avoids fighting exposed user-volume parameters?
- What minimum import validation is reliable across compressed/streamed clips and platforms?
- When should segmented music and adaptive stems receive a dedicated design specification?
- Which clip-provider adapter is first: Addressables, custom provider, or none until a consumer proves need?
- Should WebGL/mobile use different default pool and streaming presets?

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Evidence |
|---|---|---|---|
| M0 - Specification | Approved package contract | This document | Approval record |
| M1 - Package Skeleton | Installable anatomy | Manifest, asmdefs, docs shell, test assemblies | Clean compile/install |
| M2 - Root and Music Core | Duplicate-safe root and deterministic two-source transport | Claim/init/shutdown, track, crossfade, stop, diagnostics | Unit/PlayMode tests |
| M3 - SFX Runtime | Bounded voices and handles | Cue variations, 2D/3D, pool, cooldown, concurrency, stealing | Stress/handle tests |
| M4 - Ambience and Routing | Independent ambience and bus control | Profiles/layers, mixer bindings, bus API, pause domains | PlayMode tests |
| M5 - Profiles and Audio Laboratory | Authoring model and isolated proof | Schemas/profiles, setup, validator, previews, lab | Full lab checklist |
| M6 - First Bridges | Explicit integrations | First Light, Accord, Pulse, Observatory as approved | Integration Labs/removal tests |
| M7 - Real-Project Adoption | Genre-independent parity | Rescuers2D and Don’t Get Vince’d adapters | Parity reports/rollback proof |
| M8 - Beta/Release | Distribution-ready package | Docs, licenses, migrations, compatibility, artifact | External clean install/tag |

### 28.2 Checkpoint rule

Every implementation milestone is split into SFGSS-005 Checkpoint Build Plans. Each checkpoint must produce one testable result, list exact files/assets and Editor setup, include expected tests/failures/rollback, and close with Current Notes reconciliation, documentation updates, commit, push, and release/devlog records as applicable.

### 28.3 First recommended checkpoint

After the complete Foundation documentation gate and cross-spec consistency review pass:

> **Jukebot M1-01 - Package Skeleton and Clean Compile:** create only the UPM manifest, runtime/editor/test assembly definitions, namespace folders, documentation shell, changelog/license/notices placeholders, and an empty compile-safe configuration/root type surface approved by the implementation plan. No playback behavior enters this checkpoint.

The expected first suite implementation remains First Light M1 unless FW-DOC-11 changes the order. Jukebot M1-01 follows according to the approved implementation roadmap.

---

## 29. New-Conversation Handoff

```text
We are continuing development of The Sperk’s Forge - EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the authority for suite-wide boundaries and architecture.
Treat the Resonance (Jukebot) Package Specification as the authority for
runtime audio behavior, public API, data, tooling, Audio Laboratory, and release gates.
Follow SFGSS-005 for implementation checkpoints after the Foundation documentation gate opens.

Current package: Jukebot
Current specification version: 1.0.0 Approved
Current milestone/checkpoint: <CHECKPOINT>
Current Unity version: 6000.3.8f1
Current project/repository: <PROJECT>
Current implementation status: Specification approved; runtime implementation locked until FW-DOC-12
Known blockers: <BLOCKERS>

Before writing code:
1. Summarize Jukebot’s audio authority and independence constraints.
2. Confirm music, SFX, ambience, settings persistence, game state, scene flow, and project trigger boundaries.
3. Preserve immutable definitions and runtime-owned mutable playback state.
4. Keep optional integrations behind explicit bridges or project adapters.
5. Preserve existing project audio until replacement parity is proven.
6. Continue using the Checkpoint Build Plan format.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | 1.0.0 specification |
| Completed checkpoint | FW-DOC-06 - Resonance specification |
| Files/assets created | Package specification and reconciled Foundation documentation checkpoint |
| Tests passed | Specification structure and documentation reconciliation validation |
| Tests failed | None; runtime tests not started |
| Known issues | Licensing remains suite-wide; implementation defaults/platform evidence pending |
| Decisions added | JB-D-001 through JB-D-018 |
| Next checkpoint | FW-DOC-07 - The Will (`EchoInput`) package specification |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Package identity and responsibility are clear.
- [x] Ownership/non-ownership align with SFGSS-000.
- [x] Independence proof is credible.
- [x] Music, SFX, and ambience remain independent owned services.
- [x] MVP is complete but bounded.
- [x] Definitions and runtime state are separated.
- [x] Music transport and rapid-request policy are specified.
- [x] Voice pool, concurrency, stealing, cooldown, and handle behavior are specified.
- [x] Mixer/settings boundary is explicit.
- [x] Audio profile model is selected.
- [x] Direct-scene and Audio Laboratory workflows are defined.
- [x] Diagnostics do not require The Observatory.
- [x] Optional integrations are explicit/removable.
- [x] Test and release gates are measurable.
- [x] No Isekai Studios identity or ownership introduced.
- [x] Foundation runtime implementation remains locked.
- [x] Jesse approved the long-term-effective architecture policy for this documentation pass.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 3, 2026  
**Conditions or notes:** This specification is approved as the Level 2 authority for Jukebot. No runtime package code, Unity scenes, prefabs, mixers, or assets may be implemented until all ten Foundation specifications and the FW-DOC-11 cross-package consistency review are approved. Implementation measurements may refine non-authoritative defaults but may not silently change ownership, independence, state, handle, or integration contracts.

---

## Specification Completion Statement

A new collaborator can determine from this document:

1. Jukebot owns runtime music, SFX, ambience, voice, handle, and mixer execution.
2. It refuses settings persistence, gameplay triggers, UI, state, scene, and save ownership.
3. Its MVP is one duplicate-safe root, two-source music, bounded pooled SFX, ambience, routing, profiles, diagnostics, tooling, and a standalone laboratory.
4. It works independently and exposes explicit integration seams.
5. Definitions remain immutable while all changing playback state belongs to the runtime.
6. Its public API uses request/result objects, generational handles, snapshots, and lifecycle operations.
7. Configuration and runtime failures return structured outcomes and stable codes.
8. The Audio Laboratory proves the package without peer Echo packages.
9. Bridges connect optional peers without introducing hidden dependencies.
10. Clean install, stress, immutability, parity, documentation, licensing, and distribution evidence are required before release.

The Resonance (`Jukebot`) package specification is therefore complete and **Approved**.
