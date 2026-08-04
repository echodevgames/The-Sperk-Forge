# Voices – Dialogue and Conversation Flow Package Specification

**Working document ID:** SFGSS-PKG-ECHODIALOGUE-001  
**Specification version:** 1.0.1
**Status:** Approved  
**Technical package name:** EchoDialogue  
**Public title:** Voices – Dialogue and Conversation Flow
**Package ID:** `com.echodevgames.echo-dialogue`  
**Runtime namespace:** `EchoDevGames.EchoDialogue`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoDialogue`
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Last updated:** August 4, 2026

> “Give every speaker a place, every choice a consequence, and every silence an honest end.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoDialogue. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-005 and the approved Foundation and Expansion authorities through Many Tongues | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved speaker, conversation graph, line, choice, condition, command, interruption, history, snapshot, authoring, integration, Laboratory, and release contracts | Jesse “Echo” Adams |
| 1.0.1 | 2026-08-04 | Approved | Normalized registry metadata and formal title; added the SUITE-DOC-30 governing-authority, evidence, test-registry, and compatibility clarification without authorizing implementation. | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** Voices – Dialogue and Conversation Flow
**Technical identifier:** EchoDialogue  
**Flavor line:** Give every speaker a place, every choice a consequence, and every silence an honest end.  
**Plain-language subtitle:** A standalone Unity package for project-authored speakers, deterministic conversation graphs, lines, branching choices, conditions, project commands, interruption, semantic history, safe-session snapshots, diagnostics, and optional presentation/localization/audio/gameplay bridges.

**One-sentence ownership contract:**

> EchoDialogue owns project-authored speaker and conversation definitions, one deterministic foreground conversation authority, stable node traversal, line and choice flow, read-only condition evaluation, explicit project-command dispatch, session-local variables, interruption and suspension policy, semantic history, safe active-session snapshots, diagnostics, authoring, validation, and optional bridge seams; it does not own translation tables, production UI presentation, player input, audio playback, quest completion, inventory or character state, camera motion, global pause, scene travel, save-file transport, cinematic direction, or the game's complete narrative database.

### 1.1 Elevator summary

Voices provides the reusable machinery that turns authored conversation data into a deterministic runtime session. A conversation begins at one stable node, advances through line, choice, branch, command, local-state, wait, and end records, and produces structured results instead of hiding game logic inside buttons or animation callbacks. The package knows which line is current, which choices are valid, which command is awaiting completion, and why a session ended. It does not decide how a text box looks, which input action means Advance, how an AudioSource plays a voice clip, whether a quest completes, or where the camera moves.

The core stores provider-neutral references rather than hard dependencies. A line may carry an opaque `DialogueTextReference`, optional voice reference, portrait token, emotion token, and semantic camera/presentation cues. Source fallback text makes the package independently demonstrable. A Many Tongues bridge resolves production localized text; a Looking Glass bridge presents it; a Resonance bridge handles optional voice playback; a Will bridge translates input intent; a Pulse bridge acquires dialogue state/pause policy; Path and project command handlers perform quest or world actions; Eye adapters respond to camera cues; Chronicle stores snapshots. Removing any bridge returns Voices to documented standalone behavior.

The runtime defaults to one foreground conversation. That is an honest MVP boundary, not an assertion that games never need barks, ambient chatter, radio channels, multiplayer dialogue, or cinematic timelines. Those later capabilities require separate concurrency and authority designs. The first release instead makes one conversation reliable, interruptible by policy, saveable only at safe points, diagnosable, and independently testable.

### 1.2 Why this belongs in The Sperk's Forge

Dialogue systems repeatedly become accidental centers of gravity. UI scripts mutate quests, button callbacks set camera targets, text assets store mutable progress, and line indices become save identities. A reusable package is justified because the recurring engineering problem is not merely displaying sentences. It is preserving authorship, identity, branching, conditions, side-effect boundaries, interruption, persistence, validation, and presentation independence while the concrete story remains project-owned.

| Source project or authority | Existing need or failure pattern | Preserve | Improve |
|---|---|---|---|
| Hackulos and RPG work | Quest NPCs, vendors, class/spell handoffs, lore, and branching conversations | Data-driven narrative content | Stable graph IDs, provider conditions/commands, localization references, save-safe sessions |
| Rescuers2D and game-jam work | Brief instructions, survivor interactions, warnings, and results often live directly in UI | Fast authoring and scene feedback | Move flow out of prefabs and keep gameplay outcomes in project handlers |
| Many Tongues | Dialogue lines, speaker names, and optional voice assets need localization | Translation tables remain project-owned | Voices stores provider-neutral references and never becomes locale authority |
| The Looking Glass | Dialogue needs text, portraits, choices, history, and focus | UI remains modular | Presentation returns structured user results without owning graph truth |
| Resonance | Optional voice cues and dialogue sounds need playback | Jukebot owns audio | Voices requests semantic voice work through a bridge/provider |
| The Path | Dialogue may read or request objective state changes | Objectives remain authoritative | Conditions are read-only; commands request explicit project effects |
| The Chronicle | Active conversation sessions may need durable resume | Save files remain Chronicle's job | Voices exports/imports versioned safe-point snapshots |
| The Pulse and The Will | Dialogue often changes runtime mode and input context | One state and input authority | Bridges acquire scopes; core never changes time scale or reads action maps |

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---|---|
| Public title and documentation | Yes | “Voices” always appears beside the dialogue/conversation responsibility |
| Setup headings and tooltips | Yes | Flavor may decorate speaker, line, choice, and conversation language |
| Laboratory content | Optional | Sample speakers and lines remain redistributable, removable, and clearly non-production |
| Runtime API/type names | No lore-only names | Use `ConversationDefinition`, `DialogueSessionHandle`, `DialogueChoiceResult`, and direct technical names |
| Project narrative | No required Verse content | Consumer projects own every speaker, line, branch, portrait, voice, command, and story rule |

---

## 2. Problem Statement

### 2.1 Current problem

Without a declared dialogue authority and provider boundary, projects commonly accumulate:

- scene and prefab text objects that secretly determine story progression;
- line arrays addressed by fragile numeric indexes;
- speaker names, portraits, and audio clips duplicated across conversations;
- button callbacks that mutate quests, inventory, camera, audio, and save state directly;
- condition methods discovered by reflection or string method names;
- commands that cannot report failure, cancellation, timeout, or rollback boundaries;
- choices that remain selectable after the underlying option set changed;
- save files that restore into deleted or renamed lines;
- conversation history that stores resolved text in one locale and becomes stale after a locale change;
- dialogue managers that also pause the game, read input, animate UI, play audio, and load scenes;
- loops, broken edges, missing speakers, and unreachable nodes discovered only during play;
- no trustworthy explanation of why a conversation stopped.

### 2.2 Evidence from existing work

| Source project/authority | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Hackulos quest plans | NPCs deliver spells, rat-tail quests, combine instructions, and follow-ups | Authored sequences and project conditions | Neutral graph, explicit command/result seams, no quest authority inside dialogue |
| Rescuers2D | Level instructions and character interactions need short, readable conversations | Direct user feedback | Reusable presenter contract and safe input/state bridges |
| Echo Systems Lab | Focused components and semantic event flow | Runtime/presentation separation | Formalize dialogue authority and provider contracts |
| Many Tongues specification | Stable localized references and locale lifecycle | Localization remains authoritative | Dialogue stores opaque references and resolves through an optional bridge |
| The Chronicle specification | Versioned participants and prepared load/apply flow | Durable recovery discipline | Save only safe dialogue state, never UI animation or in-flight side effects |
| SFGSS-003 | Stable domain IDs and explicit migration | Identity separate from asset names | Conversation, speaker, node, choice, command, and provider IDs are validated and aliasable |

### 2.3 Consequences of doing nothing

- Narrative content remains coupled to one UI prefab and one input setup.
- Story progression becomes impossible to test without full scenes and art.
- Renaming or reordering content breaks saves.
- Project commands fail without structured recovery.
- Localization, audio, objective, camera, and save responsibilities become circular.
- A reusable dialogue package becomes a genre-locked narrative framework instead of a focused conversation authority.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide stable, project-authored speaker and conversation definitions.
- Execute one deterministic foreground conversation at a time.
- Support lines, choices, branches, local variables, waits, commands, and end results.
- Keep conditions read-only and project commands explicit, asynchronous, and failure-aware.
- Separate source/localized text resolution, presentation, voice, input, game state, camera, objectives, and persistence through providers/bridges.
- Support interruption, cancellation, suspension, and safe resume without stale handles.
- Produce bounded semantic history without recording resolved production text by default.
- Export/import active-session state only at approved safe points.
- Provide repeatable setup, graph/list authoring, validation, diagnostics, and a standalone Laboratory.

### 3.2 Non-goals

- Translation tables, locale selection, font fallback, or regional formatting.
- Production dialogue UI, navigation, focus, portraits, or typewriter rendering.
- Input action ownership or rebinding.
- Audio playback, mixing, lip synchronization, or voice generation.
- Quest/objective completion, inventory transfers, relationship systems, or character statistics.
- Camera movement, cutscene timeline direction, animation choreography, or scene loading.
- Save files, slots, cloud synchronization, or autosave policy.
- A full cinematic sequencer, screenplay editor, node scripting language, or universal visual-novel engine.
- Multiplayer dialogue authority in the MVP.
- Ambient bark/channel concurrency in the MVP.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean supported Unity project | Create a speaker, create a short conversation, run the Laboratory, and understand every required assignment |
| Narrative designer | Project with no reusable dialogue runtime | Author and validate branching conversation content without editing runtime code |
| Programmer | Project-specific conditions and outcomes | Register typed condition/command providers and receive structured results |
| UI developer | Existing dialogue view | Implement one presenter contract without becoming graph authority |
| Localization/audio developer | Optional Many Tongues or Resonance integration | Resolve/play content through removable bridges |
| Tester | Reproducible conversation fixture | Inspect current node, valid options, provider health, history, and failure codes |
| Maintainer | Package upgrade or content rename | Preserve stable IDs, aliases, snapshots, and migration evidence |

### 3.4 Measurable success criteria

- Clean-project installation compiles with declared dependencies only.
- Core Runtime compiles without any peer Echo package, uGUI, TextMeshPro, or project assembly.
- The standalone source-text path runs a complete conversation without Many Tongues.
- One active session never publishes two current nodes or two final results.
- Stale session/choice/advance handles cannot mutate current state.
- Invalid definitions fail before presentation or command side effects.
- Conditions cannot mutate dialogue or project state through the condition contract.
- Snapshot export is refused outside declared safe points.
- Setup/repair is repeatable and non-destructive.
- Laboratory and all release evidence remain `Not run` until execution.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo developers and small Unity teams.
- Narrative, quest, and content designers.
- Gameplay programmers integrating world state.
- UI, localization, audio, camera, and save-system developers.
- QA and maintainers diagnosing branching content.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| EDLG-UC-001 | Run a linear conversation | Project code | Valid root, conversation, source text provider, presenter | Lines advance and one completion result is returned | MVP |
| EDLG-UC-002 | Present conditional choices | Project code/designer | Valid choice node and condition providers | Only valid options are selectable; stale selection is rejected | MVP |
| EDLG-UC-003 | Execute a project command | Gameplay programmer | Registered handler and validated payload | Handler result follows authored success/failure policy | MVP |
| EDLG-UC-004 | Interrupt or cancel dialogue | Project/UI code | Active session and allowed policy | Session settles once, providers clean up, next request follows admission policy | MVP |
| EDLG-UC-005 | Suspend during pause/modal | Game-state/UI integration | Active session and suspension lease | Presentation pauses and resumes at the current safe node | MVP |
| EDLG-UC-006 | Save and restore active dialogue | Save integration | Session at a safe point | Versioned snapshot prepares and applies without replaying committed commands | MVP |
| EDLG-UC-007 | Resolve localized content | Many Tongues bridge | Registered text provider | Current locale text is supplied without changing flow authority | Later bridge |
| EDLG-UC-008 | Play localized voice | Resonance/Localization bridge | Voice reference and providers | Voice playback remains external; completion may inform advance policy | Later bridge |
| EDLG-UC-009 | Author and validate a graph | Designer | Editor package installed | Broken edges, missing speakers/providers, unsafe cycles, and unreachable nodes are visible | MVP |
| EDLG-UC-010 | Resume work in a direct test scene | Developer | Laboratory/direct initializer configured | Only the minimum development authority is created | MVP |

### 4.3 Explicitly unsupported use cases

- Concurrent multiplayer voting dialogue.
- Multiple independent foreground conversations in one player view.
- Network-replicated dialogue state.
- Automatic cinematic camera tracks or Timeline ownership.
- Runtime editing of production conversation definitions.
- Arbitrary C# method invocation by name.
- Executing raw script text, expressions, or reflection-authored commands.
- Treating dialogue conditions as permission to mutate world state.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Speaker definitions and stable speaker identity.
- Conversation definitions, stable graph identity, nodes, choices, and edges.
- One foreground session authority and deterministic node traversal.
- Line, choice, branch, wait, local mutation, command, and end-node runtime semantics.
- Session-local variables and visited/selected semantic records.
- Condition-provider registration/evaluation contract.
- Command-handler registration/execution contract.
- Admission, interruption, suspension, cancellation, timeout, and final result policy.
- Semantic conversation history and safe active-session snapshots.
- Dialogue-specific setup, authoring, validation, diagnostics, and Laboratory.

### 5.2 The package does not own

- Production text tables, locale selection, translation, fonts, or formatting.
- Screen layout, focus, navigation, typewriter visuals, portraits, or dialogue HUD.
- Input devices, action maps, or rebinding.
- Music, SFX, voice playback, mixer routing, or audio preferences.
- Objectives, quests, inventory, stats, relationships, rewards, or world truth.
- Camera movement, Timeline, animation state machines, or cutscene direction.
- Pause/time-scale/cursor authority.
- Scene transitions.
- Save slots/files, cloud storage, or autosave policy.
- Networking authority or synchronized player choices.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How Voices interacts |
|---|---|---|
| Locale and localized references | Many Tongues / project text provider | Optional provider bridge resolves `DialogueTextReference` and voice asset references |
| Screens, choices, history view, focus | The Looking Glass / project presenter | `IDialoguePresenter` receives immutable view models and returns structured results |
| Audio playback | Resonance / project voice provider | Optional `IDialogueVoiceProvider` request/handle; Voices never touches AudioSources |
| Global state and pause | The Pulse | Optional bridge acquires/releases a dialogue override/scope |
| Input contexts and actions | The Will | Optional bridge translates Advance/Skip/Choice/Cancel intent; core reads no input |
| Objectives/quests | The Path / project systems | Read-only condition providers and explicit command handlers/requests |
| Camera | The Eye / project camera adapter | Semantic speaker/camera cue events or explicit command handler |
| Save transport | The Chronicle | Separate participant bridge persists versioned safe snapshots |
| Startup | First Light | Optional startup-step integration creates/initializes the root |
| Diagnostics | The Observatory | Optional provider publishes redacted structured status |
| Setup composition | The Workshop | ADR-001 Editor setup facade |
| Scene travel | The Passage | Project command/adapter may request travel after dialogue settles |

### 5.4 Boundary tests

1. Does the feature determine conversation truth or only present/respond to it?
2. Can the core operate with source fallback text and a sample presenter alone?
3. Would the feature require a peer package, provider SDK, project database, or genre rule?
4. Is a condition read-only and a command side-effecting with an explicit result?
5. Could the behavior be expressed as a provider, bridge, semantic event, or project command handler?
6. Would removing the optional integration leave the dialogue graph valid and the package compilable?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

Voices must:

- Compile with only declared Unity/platform dependencies.
- Initialize without First Light or The Workshop.
- Run a source-fallback conversation without Many Tongues.
- Use a sample presenter without EchoUI or EchoInput.
- Run silently without Resonance.
- Evaluate built-in session conditions without Objectives, Inventory, Characters, or other gameplay packages.
- Export/import snapshots without Chronicle.
- Fail visibly and safely when required providers or references are absent.
- Keep project-authored speakers and conversations outside immutable package source.
- Expose `IDialogueService` and injected provider seams instead of requiring static access.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Required evidence |
|---|---|---|
| Installed alone | Runtime/Editor compile; source provider and sample presenter run the Laboratory | Clean project + Laboratory |
| Enter Laboratory directly | Development initializer creates/adopts only one root | Direct-scene lifecycle tests |
| Many Tongues absent | Source fallback/provider result is used; localized-only required content reports unavailable | PlayMode + Laboratory |
| Looking Glass absent | Sample/project presenter can operate | Standalone sample |
| Resonance absent | Voice references are ignored/advised according to policy; text flow continues | PlayMode |
| Chronicle absent | Session state remains runtime-only; explicit snapshot DTO still works | Unit/PlayMode |
| Duplicate root | Duplicate is rejected before presenter/provider registration | Lifecycle tests |
| Required configuration missing | Root fails before starting conversations and reports EDLG code | Validation/PlayMode |
| Samples deleted | Runtime and Editor packages compile | Clean project |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Planned version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity 6000.0+ core runtime | Platform | Yes | Exact support evidence pending | MonoBehaviour, ScriptableObject, serialization, scenes, Awaitable | Package cannot function without Unity |
| Unity Test Framework | Test only | Yes for tests | Exact version at implementation | EditMode/PlayMode evidence | Runtime unaffected |
| UI Toolkit Editor APIs | Editor | Planned | Unity baseline | Graph/list authoring, setup, validation | Runtime unaffected |
| uGUI/TextMeshPro | Sample or separate presentation assembly only | No | Pending SFGSS-002 packaging decision | Standalone sample presenter | Neutral Runtime remains independent |

### 6.4 Forbidden dependencies

- Direct runtime references to any optional Echo package.
- Runtime references to `UnityEditor`, samples, tests, project assemblies, or Workshop.
- Raw scene object names, tags, layers, build indexes, or input action names as hidden requirements.
- Reflection-based command discovery or arbitrary method invocation.
- Unity asset GUIDs as Player-runtime conversation, speaker, node, or choice identity.
- Production content stored only inside package samples.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| EDLG-CAP-001 | Runtime authority | Duplicate-safe application-session root and injectable service | Approved | Yes | Runtime | One foreground session |
| EDLG-CAP-002 | Speakers | Stable project-authored speaker definitions | Approved | Yes | Data/Editor | Display/presentation references remain provider-neutral |
| EDLG-CAP-003 | Conversation graph | Stable node records and deterministic edges | Approved | Yes | Data/Runtime | Explicit built-in node kinds |
| EDLG-CAP-004 | Lines | Speaker, text, presentation, optional voice, and advance policy | Approved | Yes | Runtime | Presenter/provider-driven |
| EDLG-CAP-005 | Choices | Stable options, availability policy, stale-selection rejection | Approved | Yes | Runtime | Choice generation token |
| EDLG-CAP-006 | Branches | Built-in and provider conditions | Approved | Yes | Runtime | Read-only contract |
| EDLG-CAP-007 | Commands | Typed provider dispatch, timeout, cancellation, commit boundary, failure route | Approved | Yes | Runtime | No reflection method names |
| EDLG-CAP-008 | Local variables | Bounded session-local tagged values | Approved | Yes | Runtime | Not world/save authority |
| EDLG-CAP-009 | Wait and end | Unscaled wait and structured completion | Approved | Yes | Runtime | No Timeline dependency |
| EDLG-CAP-010 | Admission/interruption | Reject, queue-latest, optional replace, cancellation | Approved | Yes | Runtime | Non-interruptible support |
| EDLG-CAP-011 | Suspension | Reason-based disposable leases | Approved | Yes | Runtime | Safe out-of-order release |
| EDLG-CAP-012 | Semantic history | Bounded reference-based history | Approved | Yes | Runtime | No raw production text by default |
| EDLG-CAP-013 | Safe snapshots | Versioned export/prepare/apply at approved points | Approved | Yes | Runtime | Chronicle optional |
| EDLG-CAP-014 | Diagnostics | Stable codes, snapshot, history, privacy policy | Approved | Yes | Runtime/Editor | Observatory optional |
| EDLG-CAP-015 | Authoring | Conversation graph/list editor and speaker inspector | Approved | Yes | Editor | Non-destructive operations |
| EDLG-CAP-016 | Validation | IDs, edges, providers, cycles, choices, migrations | Approved | Yes | Editor | Pre-Play/pre-build hooks later |
| EDLG-CAP-017 | Laboratory | Isolated conversation loop with fake providers | Approved | Yes | Sample | No peer Echo dependency |
| EDLG-CAP-018 | Many Tongues bridge | Localized text/name/voice resolution | Approved later | No | Bridge | Separate package/assembly decision |
| EDLG-CAP-019 | Looking Glass bridge | Production presenter | Approved later | No | Bridge | UI retains presentation authority |
| EDLG-CAP-020 | Resonance bridge | Voice playback provider | Approved later | No | Bridge | Audio remains Jukebot authority |
| EDLG-CAP-021 | Pulse/Will/Path/Eye/Chronicle bridges | State, input, objective, camera, save integration | Approved later | No | Bridges | Explicit and removable |

### 7.2 MVP capability set

The first complete release contains:

- one protected runtime root/service;
- speaker and conversation definitions with stable IDs;
- line, choice, branch, command, local mutation, wait, and end nodes;
- one foreground session with reject/queue-latest admission;
- optional replace-active policy for interruptible sessions;
- read-only conditions and explicit command handlers;
- source fallback text provider and sample presenter;
- suspension/cancellation, bounded history, safe snapshots, diagnostics;
- setup, authoring, validation, Laboratory, automated tests, and documentation.

### 7.3 Later capability set

- Multiple dialogue channels and ambient barks.
- Nested/sub-conversations and reusable conversation fragments.
- Advanced conversation variables and project fact-store adapters.
- Timeline/cinematic command packs.
- Lip-sync/phoneme provider contracts.
- Voice-over recording/export workflow.
- Localization spreadsheet/narrative-tool adapters.
- Multiplayer voting and authoritative synchronized sessions after EchoMultiplayer research.
- Runtime mod/content loading after a security and migration design.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| One dialogue manager that owns UI, input, audio, camera, quests, and saves | Rejected | Violates one-authority and package independence rules | Never without suite-wide architecture change |
| Arbitrary C# method names or reflection commands | Rejected | Fragile, unsafe, unvalidated, difficult to migrate | Never in core |
| Raw production text as graph identity | Rejected | Renames/locales break saves and references | Never |
| Multiple simultaneous foreground conversations | Deferred | Requires channel/focus/input and presentation authority design | Dedicated concurrency specification |
| Multiplayer dialogue voting | Deferred | Requires network authority/provider evidence | EchoMultiplayer approval |
| Full cinematic sequencer | Rejected from core | Camera/animation/timeline ownership exceeds dialogue authority | Separate integration/package |
| Runtime visual graph editing | Deferred | Security, migration, and content-loading scope | Modding design |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | `DialogueConfiguration`, `SpeakerDefinition`, `ConversationDefinition`, node/choice records, provider-neutral references, policies, aliases | Active node, current choices, command tasks, presenter state, scene objects |
| Runtime state/behavior | Root/service, session, node runner, provider registries, handles, variables, histories, snapshots, diagnostics | Editor APIs, production UI, audio sources, quest/inventory rules |
| Presentation/feedback | `IDialoguePresenter`, sample presenter, optional UI/audio/camera bridges | Authoritative graph traversal or durable world state |

### 8.2 Component topology

```text
Project/interaction requests conversation
                 |
                 v
        EchoDialogueRoot / IDialogueService
                 |
       +---------+----------+
       |                    |
 ConversationSession   Provider registries
       |                    |
       |          +---------+----------+----------+
       |          |                    |          |
   NodeRunner  Text/Presenter      Conditions   Commands
       |          |                    |          |
       +----------+--------------------+----------+
                 |
      Semantic events/results/snapshots
                 |
     Optional bridges and project adapters
```

The root owns the session and registries. Providers do not own traversal. A session changes authoritative phase before it raises semantic events. Presentation can fail, cancel, or return a choice, but it cannot choose a node that is not currently valid.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root required? | Yes for the default runtime; explicit injected service is supported for tests/project composition |
| Root type | `EchoDialogueRoot` |
| Lifetime | Application session by default; configurable scene-bound advanced use may be considered later |
| Duplicate behavior | First valid configured root claims authority; duplicates destroy/disable before registrations or sessions |
| Initialization trigger | Explicit `InitializeAsync`; optional standalone Awake/Start path and First Light bridge |
| Shutdown | Stop admissions, cancel/safely settle session, dismiss presenter, release registrations, clear state |
| Direct-scene behavior | Development initializer creates/adopts only when absent and marks development mode |
| Test injection | `IDialogueService`, fake presenter/text/condition/command/clock providers |

### 8.4 Lifecycle sequence

1. Claim runtime authority.
2. Validate configuration and source provider.
3. Create service, registries, limits, histories, and diagnostic state.
4. Register built-in source text, local conditions, and local commands.
5. Enter Ready/Idle.
6. Admit one conversation request.
7. Validate definition and runtime context.
8. Create a generational session and enter the entry node.
9. Execute node loop, yielding for presentation, choice, wait, or command as required.
10. Settle through Completed, Cancelled, Interrupted, or Failed exactly once.
11. Dismiss providers and return to Idle or admit the pending request.
12. Shutdown and release authority.

### 8.5 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Claim | Duplicate removed/disabled | Existing authority continues | EDLG-ROOT-001 |
| Missing/invalid configuration | Initialize | Initialization failure | No conversations accepted | EDLG-CONFIG-001 |
| Invalid conversation graph | Admission | Start rejected with validation summary | No presenter/command side effects | EDLG-GRAPH-001 |
| Missing required presenter | Line/choice entry | Session fails | Dismiss any active providers | EDLG-PRES-001 |
| Missing optional text/voice provider | Resolution | Fallback/advisory by policy | Source/silent path | EDLG-PROV-001 |
| Missing required condition/command provider | Validation/execution | Branch/failure/end by authored policy | Structured result | EDLG-PROV-002 |
| Stale handle/choice generation | Request | Request rejected | Current session unchanged | EDLG-HANDLE-001 |
| Immediate cycle/step-budget exceeded | Node loop | Session fails with graph location | Stop before unbounded loop | EDLG-FLOW-001 |
| Presenter exception | Await presenter | Session fails/dismisses | Provider isolated | EDLG-PRES-002 |
| Command timeout/failure | Command node | Authored continue/branch/end policy | Deterministic settlement | EDLG-CMD-001 |
| Unsafe snapshot request | Export | Export rejected | Live session unchanged | EDLG-SAVE-001 |
| Unsupported snapshot version | Prepare import | Import rejected | Preserve source data | EDLG-MIG-001 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable domain ID? | Runtime mutable? | Project-owned instance? |
|---|---|---:|---:|---:|
| `DialogueConfiguration` | Limits, policies, default providers, histories, timeouts, direct-scene policy | Optional ConfigId | No | Yes |
| `SpeakerDefinition` | Speaker identity, display-name text ref, semantic presentation defaults | `SpeakerId` | No | Yes |
| `ConversationDefinition` | Conversation metadata, entry node, node records, aliases, interruption/snapshot policy | `ConversationId` | No | Yes |
| `DialogueNodeRecord` | Stable node kind, payload, edges, authoring metadata | `NodeId` within conversation | No | Embedded project data |
| `DialogueChoiceRecord` | Stable choice, text ref, condition, availability, target | `ChoiceId` within conversation | No | Embedded project data |
| `DialogueTextReference` | Provider ID, stable content ID, optional source fallback | Provider/content identity | No | Embedded/project data |
| `DialogueVoiceReference` | Provider-neutral voice cue/asset identity | Provider/content identity | No | Embedded/project data |
| `DialogueConditionReference` | Provider ID, condition ID, bounded typed arguments, unavailable policy | Provider/condition identity | No | Embedded/project data |
| `DialogueCommandReference` | Handler ID, command ID, typed payload, timeout/failure policy | Handler/command identity | No | Embedded/project data |
| `DialoguePresentationToken` | Portrait/emotion/camera/semantic presentation ID | Domain ID/tag | No | Embedded/project data |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `DialogueRuntimeState` | Root/service | Root lifetime | Fresh initialize/shutdown | Not durable directly |
| `DialogueSession` | Service | One admitted conversation | Settled then released | Export through DTO only |
| `DialogueNodeCursor` | Session | Active node execution | Replaced after committed transition | Snapshot at safe points |
| `DialogueChoiceSnapshot` | Session | One choice presentation generation | Rebuilt on node re-entry/restore | Choice IDs may be snapshotted |
| `DialogueVariableStore` | Session | Conversation run | Cleared on settlement | Bounded typed DTO |
| `DialogueHistoryBuffer` | Service/session | Configured runtime window | Ring-buffer overwrite/reset | Optional semantic snapshot |
| Provider/handler registry | Root | Root lifetime | Dispose/unregister/shutdown | Not saved |
| Suspension leases | Session | Until disposed/settled | Invalidated by settlement | Not saved directly |
| Pending request | Service | Until admitted/replaced/rejected | Cleared on settlement/shutdown | Not saved |

### 9.3 Stable identifiers

- `SpeakerId`, `ConversationId`, `NodeId`, and `ChoiceId` are normalized stable domain IDs.
- Asset names, hierarchy paths, list indexes, source text, and Unity asset GUIDs never become Player-runtime identity.
- Node/choice IDs are unique inside their owning conversation and are qualified by `ConversationId` in diagnostics and snapshots.
- Renames preserve IDs.
- Duplicate IDs block release and normally block conversation start.
- Definitions may carry bounded alias maps for migrated Conversation/Node/Choice IDs.
- Removed public IDs receive documented tombstone/migration behavior rather than silent reuse.
- Session and handle IDs are runtime-generational and are never saved as durable identity.

### 9.4 Conversation record model

The MVP uses an explicit stable union record rather than reflection-discovered polymorphic node classes. `DialogueNodeKind` identifies the built-in payload schema:

- `Line`
- `Choice`
- `Branch`
- `Command`
- `LocalMutation`
- `Wait`
- `End`

Each record stores only the payload relevant to its kind, stable edges, and authoring metadata. Unknown future node kinds remain unsupported rather than silently deserializing into the wrong behavior. Extensibility occurs through condition and command providers, not arbitrary custom runtime node types.

### 9.5 ScriptableObject safety

- Definitions remain immutable during play.
- Current node, history, choice generation, variables, provider status, and command tasks live in runtime objects.
- Editor previews clone/detach state and never mark production definitions dirty during Play Mode.
- Runtime changes never write back into shared conversation or speaker assets.
- Scene objects and Unity components do not appear in durable definition records unless an approved provider reference explicitly owns them.

### 9.6 Dialogue values and payloads

`DialogueValue` is a bounded tagged value supporting approved durable primitives such as Boolean, signed integer, finite floating value, invariant string, and stable domain ID. It does not serialize arbitrary Unity objects, delegates, interfaces, dictionaries, or managed graphs. Command and condition schemas declare expected keys/types, optionality, limits, and privacy classification.

### 9.7 Serialization and migration

- Configuration, speaker, conversation, node payload, and session snapshot schemas carry explicit versions.
- Migrations run on detached data before publication.
- Alias resolution records which old ID mapped to which current ID.
- Unknown extension records are preserved where the envelope supports them.
- Unsupported newer data is not destructively downgraded.
- Safe snapshot import follows Parse -> Validate -> Migrate -> Resolve definitions/providers -> Prepare -> Apply.
- A snapshot never replays a command already recorded as committed.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `EchoDialogueRoot` | sealed MonoBehaviour | Claim authority, initialize, own service/registries, shutdown | Package prefab/project scene |
| `IDialogueService` | interface | Start, inspect, advance, choose, suspend, cancel, snapshot, events | Root or injected implementation |
| `DialogueConfiguration` | ScriptableObject | Policies, capacities, timeouts, direct-scene behavior | Project-owned |
| `SpeakerDefinition` | ScriptableObject | Stable speaker identity and semantic presentation defaults | Project-owned |
| `ConversationDefinition` | ScriptableObject | Stable graph and policies | Project-owned |
| `DialogueStartRequest` | immutable request | Conversation, context, reason, admission, optional initial variables | Caller-created |
| `DialogueStartResult` | immutable result | Accepted/rejected status, handle, validation/admission details | Service result |
| `DialogueSessionHandle` | struct | Generational session observation/control identity | Service-owned |
| `DialogueAdvanceRequest` | immutable request | Session handle and current presentation generation | Caller/presenter |
| `DialogueChoiceSelection` | immutable request | Session handle, option generation, stable ChoiceId | Caller/presenter |
| `DialogueSuspendLease` | disposable struct/class | Reason-based suspension lease | Service-owned |
| `DialogueSessionResult` | immutable result | Completed/cancelled/interrupted/failed outcome and codes | Service event/result |
| `DialogueTextReference` | serializable value | Provider-neutral text identity and optional source fallback | Definition/project data |
| `DialogueResolvedText` | immutable value | Provider/source, resolved text, status, metadata | Text provider result |
| `DialogueLineViewModel` | immutable DTO | Current speaker/text/presentation/voice/advance data | Service -> presenter |
| `DialogueChoiceViewModel` | immutable DTO | Stable option ID, text, state, disabled reason, generation | Service -> presenter |
| `IDialoguePresenter` | interface | Present/dismiss line and choice UI, return user/presentation result | Sample/UI bridge/project |
| `IDialogueTextProvider` | interface | Resolve text references | Source provider/Many Tongues bridge |
| `IDialogueVoiceProvider` | interface | Optional voice request/completion handle | Resonance/project bridge |
| `IDialogueConditionProvider` | interface | Read-only condition evaluation | Project/bridge |
| `IDialogueCommandHandler` | interface | Explicit side-effecting command execution | Project/bridge |
| `DialogueCommandContext` | immutable context | Session IDs, variables snapshot, cancellation, commit reporting | Service-created |
| `DialogueCommandResult` | immutable result | Success/failure/cancel/too-late, optional branch output | Handler result |
| `DialogueSessionSnapshot` | serializable DTO | Versioned safe active-session state | Service export/import |
| `DialogueHistoryRecord` | immutable DTO | Semantic line/choice/command/session record | Bounded service history |
| `DialogueDiagnosticSnapshot` | immutable DTO | Redacted runtime/provider/session health | Produced on request |
| `IDialogueClock` | interface | Unscaled time, delay, timeout testing | Injected/default Unity implementation |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/main-loop rule |
|---|---|---|---|---|
| `Awaitable<DialogueInitializeResult> InitializeAsync(CancellationToken)` | Validate and initialize authority | Claimed root/config | Ready or structured failure | Main-thread entry/completion |
| `Awaitable<DialogueStartResult> StartConversationAsync(DialogueStartRequest, CancellationToken)` | Admit/start conversation | Ready, valid definition/context | Accepted with handle or structured rejection | Main thread |
| `DialogueAdvanceResult RequestAdvance(DialogueAdvanceRequest)` | Advance current line/presentation state | Awaiting valid generation | Accepted/rejected/stale | Main thread |
| `DialogueChoiceResult SelectChoice(DialogueChoiceSelection)` | Select one current option | Awaiting current choice snapshot | Accepted/rejected/stale/disabled | Main thread |
| `DialogueSuspendLease Suspend(DialogueSessionHandle, DialogueSuspendReason)` | Suspend active session | Valid active handle | Lease or structured failure | Main thread |
| `Awaitable<DialogueCancelResult> CancelAsync(DialogueSessionHandle, DialogueCancelReason, CancellationToken)` | Cancel/interruption request | Valid active handle | Cancelled, TooLate, stale, failed | Main thread |
| `DialogueSessionSnapshotResult ExportSnapshot(DialogueSessionHandle)` | Export safe state | Valid safe point | Snapshot or UnsafePoint | Main thread |
| `DialoguePreparedSnapshotResult PrepareSnapshot(DialogueSessionSnapshot)` | Parse/validate/migrate/resolve | Ready | Prepared result without mutation | Main thread/background detached work as approved |
| `Awaitable<DialogueApplySnapshotResult> ApplyPreparedSnapshotAsync(...)` | Atomically restore session | Idle/approved replacement policy | Applied or failed without partial publication | Main thread |
| `DialogueDiagnosticSnapshot CaptureSnapshot()` | Capture redacted health | Any root state | Immutable snapshot | Main thread |
| `IDisposable RegisterPresenter(IDialoguePresenter)` | Register one active presenter policy | Ready/initializing | Registration result/handle | Main thread |
| `IDisposable RegisterTextProvider(IDialogueTextProvider)` | Register provider by stable ProviderId | Ready/initializing | Duplicate rejected | Main thread |
| `IDisposable RegisterConditionProvider(IDialogueConditionProvider)` | Register read-only provider | Ready/initializing | Duplicate rejected | Main thread |
| `IDisposable RegisterCommandHandler(IDialogueCommandHandler)` | Register command handler/schema | Ready/initializing | Duplicate rejected | Main thread |
| `Awaitable ShutdownAsync()` | Stop admissions and release authority | Root exists | Idempotent completion | Main thread |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `InitializationStateChanged` | Root | After state publication | Previous/current state and code | Diagnostics only |
| `ConversationAdmitted` | Service | After handle/session publication | Conversation/session IDs, reason | No reentrant mutation |
| `NodeEntered` | Session | After current node changes | Conversation/node/kind/generation | Presentation/diagnostics may observe |
| `LinePresentationStarted` | Session | After line view model is current | Redacted IDs and presentation generation | Audio/camera bridges may respond |
| `ChoicesPublished` | Session | After valid option snapshot is stored | Choice IDs/states/generation | UI may present; stale selections rejected |
| `CommandCompleted` | Session | After command result/commit state is stored | Handler/command IDs, result status | No payload values in ordinary event logs |
| `DialogueSuspensionChanged` | Session | After lease state changes | Suspended/reasons/count | Presentation may pause/resume |
| `ConversationCompleted` | Service | Exactly once after settlement | `DialogueSessionResult` | Listeners not required for completion |
| `DialogueWarningRaised` | Service | After bounded record created | Stable diagnostic code/redacted context | Listener failures isolated |

### 10.4 Async and cancellation policy

- Public asynchronous methods return fresh Unity `Awaitable<T>` instances.
- Authority entry and completion occur on the Unity main thread.
- Conditions are synchronous/read-only in the MVP to keep option publication deterministic. Expensive state must be cached by the provider or handled through an explicit pre-command step.
- Commands, text, voice, and presenter operations may be asynchronous.
- Every asynchronous provider receives a bounded cancellation token and, where relevant, timeout.
- Commands declare a commit point. Cancellation before commit prevents publication; after commit returns `TooLate` and the actual outcome settles.
- One foreground session runs at a time. Admission defaults to RejectNew, with bounded QueueLatest available. ReplaceActive is opt-in and requires the active definition to allow interruption.
- Session settlement dismisses presenter/voice providers exactly once and invalidates handles/leases.
- Caller destruction does not implicitly cancel root-owned work without a supplied token or explicit cancel request.

### 10.5 API ergonomics

**Novice path**

1. Run Setup.
2. Create one speaker and one conversation.
3. Add Line -> End.
4. Assign source fallback text.
5. Open the Laboratory and press Start/Advance.
6. Run validation.

**Programmer path**

- Inject/use `IDialogueService`.
- Register explicit presenter, text, condition, command, and optional voice providers.
- Start through structured requests and retain the generational handle.
- Respond to immutable view models and semantic events.
- Export/import safe snapshots through a bridge or project persistence.
- Unit-test through fake clock/providers with no scenes or peer packages.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install EchoDialogue.
2. Open **Tools > EchoDevGames > Voices > Setup**.
3. Scan for configuration, roots, speaker/conversation folders, source provider, and existing definitions.
4. Choose Adopt Existing or Create Missing.
5. Preview every asset, folder, prefab, and scene change.
6. Create/apply only the approved plan.
7. Open the conversation authoring window.
8. Create a speaker and sample/source conversation if desired.
9. Open the Standalone Laboratory.
10. Run validation and export the readiness report.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create configuration | Project-owned config | Nothing existing | Yes | Unity Undo/create receipt | Setup receipt |
| Create root prefab | Project-owned configured prefab | Nothing existing | Yes, duplicate-aware | Unity Undo | Prefab receipt |
| Add root to approved Boot scene | Scene instance | Approved scene only | Yes | Scene backup/Undo | Scene receipt |
| Create speaker/conversation folders | Project folders | Nothing | Yes | Directory receipt | Setup report |
| Create source text provider profile | Project-owned provider data | Configuration reference | Yes | Undo | Provider report |
| Create speaker template | Empty definition | Nothing | Yes | Undo | Asset receipt |
| Create conversation template | Empty valid skeleton | Nothing | Yes | Undo | Asset receipt |
| Import Laboratory | Sample assets | Project sample area | Package Manager sample rules | Removable | Import report |
| Repair eligible references | Echo-owned/config references only | Approved targets | Yes | Dry-run/Undo | Repair report |
| Generate validation report | Report file/asset | Nothing | Yes | N/A | Versioned report |

No setup operation silently rewrites production lines, assigns localization tables, creates gameplay commands, changes input actions, edits save data, or adds peer packages without an explicit Workshop/package plan.

### 11.3 Conversation authoring window

The Editor provides a structured graph/list authoring surface that:

- shows stable conversation/node/choice IDs separately from display labels;
- creates only approved built-in node kinds;
- generates fresh IDs when duplicating content;
- previews inbound/outbound edge impact before deletion;
- searches speakers, text providers, conditions, commands, and presentation tokens;
- shows required/optional provider status;
- provides a read-only traversal preview with fake conditions;
- never executes project commands during authoring preview;
- preserves Unity Undo, dirty-state, `.meta`, and GUID behavior;
- supports large conversations through search, filtering, minimap/list, and validation navigation.

The exact Editor canvas technology is an implementation choice; this specification does not require an unstable graph API.

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| EDLG-VAL-001 | Missing/duplicate runtime root | Error/Blocker | Yes | Add/remove only Echo-owned duplicate |
| EDLG-VAL-002 | Missing configuration/source provider | Error | Yes | Create/link after preview |
| EDLG-VAL-003 | Duplicate/empty SpeakerId or ConversationId | Error | Yes | Fresh ID only for unreferenced/new content |
| EDLG-VAL-004 | Duplicate/empty NodeId or ChoiceId | Error | Yes | Preview reference migration required |
| EDLG-VAL-005 | Broken edge/entry node | Blocker | No automatic semantic repair | No |
| EDLG-VAL-006 | Missing required speaker/text/provider/handler | Error | Link/create provider or change policy | Limited |
| EDLG-VAL-007 | Unreachable node | Warning/Error by policy | Manual review | No |
| EDLG-VAL-008 | Immediate cycle or path exceeds budget | Error | Manual graph repair | No |
| EDLG-VAL-009 | Choice node has no enabled/fallback path | Error | Manual authoring | No |
| EDLG-VAL-010 | Command/condition payload violates schema | Error | Inspector guidance | Limited |
| EDLG-VAL-011 | Unsupported snapshot/config version | Blocker | Migration when available | Never destructive |
| EDLG-VAL-012 | Privacy-sensitive diagnostic policy disabled in release profile | Warning/Error | Yes | Explicit project choice |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Planned routes, each pending SFGSS-004 evidence:

- Unity Package Manager Git URL.
- Local package path.
- Tarball.
- Embedded package development.
- The Workshop selection after setup facade implementation.

### 12.2 Minimal scene setup

- One `EchoDialogueRoot` with `DialogueConfiguration`.
- One registered presenter. The Laboratory supplies a sample presenter.
- Built-in source text provider enabled or another required text provider registered.
- One `SpeakerDefinition` and one valid `ConversationDefinition`.
- A test launcher or project script that issues a `DialogueStartRequest`.

No EventSystem, input action, audio source, camera, save system, or peer Echo package is required by the neutral core.

### 12.3 Boot-scene setup

Production may place the root in a Boot scene or initialize it through First Light. The root claims before provider registration and persists according to configuration. First Light remains optional; its integration only orders initialization.

### 12.4 Direct-scene setup

`EchoDialogueDirectSceneInitializer` may create/adopt the configured development root only when absent. It marks diagnostics as development initialization, uses the same duplicate claim path, and may be excluded or disabled in release builds.

### 12.5 Scene isolation rule

The Standalone Laboratory contains no peer Echo package code. Fake text, presenter, condition, command, voice, and clock providers demonstrate contracts. Integration Labs belong to bridge artifacts and declare both peers.

---

## 13. Standalone Test Lab and Samples

### 13.1 Laboratory purpose

The **Voices Conversation Laboratory** proves one complete isolated dialogue loop: initialize, start, present lines, evaluate choices, execute fake commands, suspend/cancel, export/restore a safe snapshot, inspect history/diagnostics, reset, and repeat without any unrelated package.

### 13.2 Required contents

- Minimal configured root and direct-scene initializer.
- Source fallback text provider.
- Minimal sample presenter with explicit Advance, Skip, Choice, Cancel, Suspend, Resume, Save Snapshot, Restore, and Reset controls.
- Simulated condition and command providers.
- Optional simulated voice-completion provider.
- Speakers and conversations for linear, branching, failure, cycle, and restore cases.
- Visual readout of session handle, phase, node, generation, options, variables, histories, provider health, and last code.
- No restricted audio/art or project content.

### 13.3 Laboratory acceptance checklist

| Test | Action/fixture | Expected result | Evidence type | Status |
|---|---|---|---|---|
| EDLG-LAB-001 | Initialize a configured root in the Laboratory and display Ready state. | Manual/automated | Not run |
| EDLG-LAB-002 | Place two roots in one scene and reject the duplicate before presenter or provider registration. | Manual/automated | Not run |
| EDLG-LAB-003 | Enter the Laboratory directly with no root and create only the development authority. | Manual/automated | Not run |
| EDLG-LAB-004 | Start a one-line conversation and complete it through manual advance. | Manual/automated | Not run |
| EDLG-LAB-005 | Run a multi-line sequence with different speakers and deterministic next-node traversal. | Manual/automated | Not run |
| EDLG-LAB-006 | Use automatic line advance after an approved unscaled delay. | Manual/automated | Not run |
| EDLG-LAB-007 | Exercise typewriter-complete, skip, and advance as separate presenter results. | Manual/automated | Not run |
| EDLG-LAB-008 | Resolve source fallback text with no optional localization provider installed. | Manual/automated | Not run |
| EDLG-LAB-009 | Resolve text through a simulated external text provider and report the provider source. | Manual/automated | Not run |
| EDLG-LAB-010 | Show speaker display-name, portrait-token, and emotion-token data without loading project art in the core. | Manual/automated | Not run |
| EDLG-LAB-011 | Present a line with an optional voice reference through a simulated voice provider. | Manual/automated | Not run |
| EDLG-LAB-012 | Auto-advance after simulated voice completion without making audio playback authoritative. | Manual/automated | Not run |
| EDLG-LAB-013 | Present a choice node with three enabled options and accept one current-generation selection. | Manual/automated | Not run |
| EDLG-LAB-014 | Hide a choice whose condition evaluates false under Hide policy. | Manual/automated | Not run |
| EDLG-LAB-015 | Display a disabled choice and disabled-reason reference under Disable policy. | Manual/automated | Not run |
| EDLG-LAB-016 | Reject a stale choice selection after the option generation changes. | Manual/automated | Not run |
| EDLG-LAB-017 | Take the true branch of a built-in session-variable condition. | Manual/automated | Not run |
| EDLG-LAB-018 | Take the false branch of a project condition result. | Manual/automated | Not run |
| EDLG-LAB-019 | Handle an unavailable condition provider through the authored unavailable branch. | Manual/automated | Not run |
| EDLG-LAB-020 | Execute a required command successfully and continue. | Manual/automated | Not run |
| EDLG-LAB-021 | Continue after an optional command failure according to authored policy. | Manual/automated | Not run |
| EDLG-LAB-022 | Route to a failure node after a required command failure. | Manual/automated | Not run |
| EDLG-LAB-023 | Time out a command handler and settle with one deterministic result. | Manual/automated | Not run |
| EDLG-LAB-024 | Mutate a conversation-local variable without changing project or save authority. | Manual/automated | Not run |
| EDLG-LAB-025 | Reject a second conversation while one is active under RejectNew admission. | Manual/automated | Not run |
| EDLG-LAB-026 | Queue only the latest pending conversation under QueueLatest admission. | Manual/automated | Not run |
| EDLG-LAB-027 | Cancel and replace an interruptible conversation under ReplaceActive admission. | Manual/automated | Not run |
| EDLG-LAB-028 | Reject replacement when the active conversation is marked non-interruptible. | Manual/automated | Not run |
| EDLG-LAB-029 | Suspend and resume an active conversation through an explicit reason lease. | Manual/automated | Not run |
| EDLG-LAB-030 | Dispose suspension leases out of order without resuming too early. | Manual/automated | Not run |
| EDLG-LAB-031 | Cancel during line presentation and dismiss the presenter exactly once. | Manual/automated | Not run |
| EDLG-LAB-032 | Cancel before command publication and prevent the command from applying. | Manual/automated | Not run |
| EDLG-LAB-033 | Attempt cancellation after an irreversible command commit point and report TooLate. | Manual/automated | Not run |
| EDLG-LAB-034 | Export an active-session snapshot at an approved safe point. | Manual/automated | Not run |
| EDLG-LAB-035 | Restore a snapshot to a line node and re-present it deterministically. | Manual/automated | Not run |
| EDLG-LAB-036 | Restore a snapshot to a choice node and rebuild the valid option generation. | Manual/automated | Not run |
| EDLG-LAB-037 | Preserve bounded semantic conversation history without storing resolved production text. | Manual/automated | Not run |
| EDLG-LAB-038 | Resolve current-locale history text on demand through the text provider. | Manual/automated | Not run |
| EDLG-LAB-039 | Detect a missing speaker reference during validation. | Manual/automated | Not run |
| EDLG-LAB-040 | Detect a broken node edge and refuse to start the invalid conversation. | Manual/automated | Not run |
| EDLG-LAB-041 | Detect an immediate transition cycle that exceeds the configured step budget. | Manual/automated | Not run |
| EDLG-LAB-042 | Isolate a presenter exception and end the session with a structured failure. | Manual/automated | Not run |
| EDLG-LAB-043 | Run without GameState, UI, Localization, Audio, Objectives, Camera, Input, or Save packages installed. | Manual/automated | Not run |
| EDLG-LAB-044 | Reset the Laboratory repeatedly and verify no listeners, providers, sessions, or histories leak. | Manual/automated | Not run |

### 13.4 Optional integration samples

| Sample | Packages involved | Purpose | Why not standalone proof |
|---|---|---|---|
| Voices + Many Tongues | EchoDialogue, EchoLocalization bridge | Localized speaker/line/choice/voice references | Requires two authorities and bridge |
| Voices + Looking Glass | EchoDialogue, EchoUI bridge | Production screen, focus, choices, history | UI is optional presentation authority |
| Voices + Resonance | EchoDialogue, Jukebot bridge | Voice playback and completion | Audio authority is external |
| Voices + Pulse + Will | Dialogue, game-state, input bridges | Dialogue mode/input context | Multiple peer authorities |
| Voices + Path + Chronicle | Dialogue, objectives, save bridges | Quest request and active-session persistence | Gameplay/save integration evidence |

Samples are separately importable/removable and never become Runtime dependencies.

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

Production presentation is optional and external. The core defines immutable presenter contracts and semantic models. The Looking Glass bridge or project presenter owns:

- dialogue window layout;
- speaker name, portrait, emotion, and history rendering;
- typewriter animation and visual skip behavior;
- choice button layout, focus, navigation, and disabled styling;
- modal/background blocking;
- responsive layout and themes.

Voices validates current generations and flow. It does not manipulate canvases, GameObjects, selection, or input modules.

### 14.2 Required presentation states

- Hidden/Idle.
- Preparing.
- Showing line.
- Awaiting advance.
- Showing choices.
- Awaiting choice.
- Suspended.
- Busy executing command.
- Warning/fallback.
- Failure/ending.

### 14.3 Typewriter, skip, and advance contract

The presenter returns distinct results:

- `TextPresentationCompleted`
- `SkipPresentationRequested`
- `AdvanceRequested`
- `ChoiceSelected`
- `CancelRequested`
- `PresentationFailed`

A first press may finish typewriter presentation without advancing. The project/presenter chooses policy. The core rejects stale generations and never assumes a visual animation completed merely because a frame elapsed.

### 14.4 Accessibility requirements

- Full keyboard/controller navigation belongs to the presenter/input integration.
- Lines must be available without voice.
- Voice must not be the sole carrier of required information.
- Typewriter effects must be skippable and may be disabled/reduced.
- Auto-advance timing must be configurable and may be overridden by accessibility preferences through a bridge/project policy.
- Choices must communicate disabled state without color alone.
- Text scaling, contrast, focus, screen-reader labels, subtitles, and RTL layout belong to UI/localization integrations.
- Timed choices are deferred from MVP because they require explicit accessibility and pause policy.

### 14.5 Visual customization

All production visual assets, layouts, portraits, fonts, animations, and transitions are project-owned and replaceable without editing EchoDialogue Runtime.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost/policy |
|---|---|---|---|
| Initialization/root state | API/Inspector/log | Editor/Development/Release-safe summary | On change/request |
| Current conversation/node/phase | API/overlay sample | Development; redacted release option | No raw text |
| Provider/handler registry | API/Inspector | Development | Bounded counts/IDs |
| Session queue/suspensions | API | Development | Bounded |
| Last start/command/presenter/snapshot result | API/log | All with redaction | On event |
| Semantic history | API/Laboratory | Configured | Ring buffer |
| Definition validation | Editor report | Editor | Manual/pre-Play/pre-build hook later |
| Support snapshot | Explicit export | Editor/Development/approved release | Local only, redacted |

### 15.2 Structured status

A diagnostic snapshot includes:

- package/version/build;
- root identity and development/canonical initialization;
- configuration identity/schema;
- service state;
- current ConversationId, NodeId, NodeKind, phase, and generations;
- session/queue/suspension/history counts;
- presenter/text/voice/condition/command provider IDs and health;
- last result/diagnostic code and bounded timings;
- definition/snapshot version and migration status.

It excludes resolved production text, speaker display names, player names, formatting argument values, command payload values, save payloads, and absolute paths.

### 15.3 Diagnostic codes

| Code family | Meaning | User action |
|---|---|---|
| `EDLG-ROOT-*` | Authority/duplicate/lifecycle | Keep one valid root and inspect setup |
| `EDLG-CONFIG-*` | Configuration/policy/limits | Run validation/repair |
| `EDLG-GRAPH-*` | Conversation/node/edge/ID problem | Open authoring validator |
| `EDLG-PROV-*` | Missing/duplicate/unhealthy provider | Install/register bridge/provider or change policy |
| `EDLG-PRES-*` | Presenter failure or stale presentation | Inspect presenter and generation |
| `EDLG-COND-*` | Condition unavailable/error/schema issue | Inspect read-only provider |
| `EDLG-CMD-*` | Command failure/timeout/commit issue | Inspect handler and authored failure route |
| `EDLG-HANDLE-*` | Stale/foreign/double lease/request | Use current service result/handle |
| `EDLG-FLOW-*` | Invalid transition/cycle/budget | Repair conversation graph |
| `EDLG-SAVE-*` | Unsafe snapshot/restore issue | Move to safe point or migrate data |
| `EDLG-MIG-*` | Unsupported/missing migration | Preserve data and install compatible version |
| `EDLG-PRIV-*` | Diagnostic/export privacy policy | Redact/reconfigure before release |

### 15.4 Observatory bridge

A separate bridge publishes the structured snapshot and bounded events. EchoDialogue does not reference Observatory. Removing the bridge changes no dialogue behavior.

### 15.5 Logging policy

- Categorized, stable codes and package context.
- No per-frame spam.
- Repeated provider/content warnings are rate-limited and summarized.
- No resolved line/choice text, player-entered values, command payload values, or full snapshot data in ordinary logs.
- Development verbosity is separate from release-safe diagnostics.
- Listener/log sink exceptions never break session settlement.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Speaker/conversation definitions | Project content | Project/EchoDialogue types | Build asset, not mutable save | Unity assets |
| Active session cursor/variables/history | Session | EchoDialogue | Optional safe snapshot | Explicit export/Chronicle bridge |
| World facts used by conditions | Game/project authority | Objectives/inventory/characters/project | Not by EchoDialogue | Owning system |
| Dialogue UI/typewriter/focus | Presentation | UI presenter | No | Reconstructed |
| Provider registrations/handles/tasks | Runtime | Root/providers | No | Recreated |
| Completed quest/reward state | Gameplay authority | Path/project systems | No | Owning system |

### 16.2 Standalone behavior

Without Chronicle, EchoDialogue is session-only but can export/import a detached `DialogueSessionSnapshot`. It selects no filename, slot, cloud provider, autosave cadence, or hidden `PlayerPrefs` storage.

### 16.3 Snapshot safe points

Approved snapshot points include:

- before a node begins;
- while awaiting manual advance after the current line is fully published;
- while awaiting a published choice snapshot;
- while suspended at one of those points;
- after a command result has fully committed and before the next node begins.

Unsafe points include:

- while a command has an unresolved commit state;
- while presenter/voice provider publication is partial;
- during root shutdown or session settlement;
- while definition/provider migration is unresolved.

A restored line may be re-presented. A committed command is never replayed merely to reconstruct presentation.

### 16.4 Chronicle bridge

The separate bridge registers a versioned participant that captures the safe snapshot or records that no active safe session exists. Load uses Chronicle's prepare/apply flow. The bridge does not serialize UI GameObjects, provider instances, async tasks, or runtime handles.

### 16.5 Failure and recovery

- Missing snapshot: start no active conversation.
- Corrupt snapshot: reject and report; Chronicle recovery policy remains external.
- Older supported snapshot: migrate detached data.
- Newer unsupported snapshot: preserve/reject, never destructive downgrade.
- Missing conversation/node: use explicit alias map or fail safely.
- Missing optional provider after restore: apply authored fallback/unavailable policy.
- Missing required command/condition provider: restoration fails before session publication.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Optional connections are explicit, removable, versioned, and owned by the bridge/provider. Installing a peer package does not silently change core behavior. Core references only neutral provider interfaces and semantic data.

### 17.2 Planned integrations

| Other authority | Connection type | Bridge owner | Direction/data | Required? |
|---|---|---|---|---:|
| Many Tongues | Separate bridge/provider | Dialogue + Localization integration artifact | Text/name/voice localized references and arguments | No |
| The Looking Glass | Separate bridge | Dialogue + UI integration artifact | Presenter view models/results/history | No |
| Resonance | Separate bridge/provider | Dialogue + Jukebot integration artifact | Voice cue request, handle, completion | No |
| The Pulse | Separate bridge | Dialogue + GameState integration artifact | Acquire/release dialogue scope | No |
| The Will | Separate bridge | Dialogue + Input integration artifact | Advance/skip/choice/cancel intent | No |
| The Path | Separate bridge/project handlers | Objectives integration artifact | Read conditions and request objective commands | No |
| The Eye | Separate bridge/project handler | Camera integration artifact | Speaker/camera semantic cues | No |
| The Chronicle | Separate bridge | Save integration artifact | Versioned participant snapshot | No |
| First Light | Tiny/separate startup integration per SFGSS-002 | Owner decision later | Ordered initialization | No |
| The Observatory | Separate provider bridge | Diagnostics integration artifact | Redacted status/metrics | No |
| The Workshop | ADR-001 Editor facade | EchoDialogue Editor | Dry-run setup plan/apply/receipt | No runtime dependency |

### 17.3 Provider registration rules

- Providers have stable Provider/Handler IDs and declared schemas/capabilities.
- Duplicate IDs are rejected deterministically.
- Registrations return disposable handles owned by the registering integration.
- Removing a provider invalidates future requests; active operations settle according to policy.
- Conditions are read-only and synchronous in MVP.
- Commands are side-effecting and asynchronous with explicit commit state.
- Presenter registration policy allows exactly one authoritative foreground presenter by default.
- Reflection scanning is not used for production discovery.

### 17.4 Integration failure behavior

- Missing optional provider: source/silent/fallback behavior by authored policy.
- Missing required provider: validation/start/restore fails before side effects.
- Version mismatch: bridge refuses registration and reports a stable compatibility code.
- Peer disabled/shutdown: bridge unregisters and active work settles safely.
- Initialization order: providers may register before/after root Ready; root exposes explicit health changes.
- Teardown: bridge detaches before peer/core removal.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

All targets are planned and remain `Not run`.

| Metric | Planned target | Measurement fixture | Release threshold |
|---|---|---|---|
| Idle runtime work | No per-frame polling while Idle | Profiler/Laboratory | No package Update work/allocation without active session |
| Node transition | Bounded deterministic processing | 100-node fixture | Project-defined budget; no unbounded loop |
| Immediate transition chain | Enforce configured step budget | Cycle fixture | Settle/fail before frame hang |
| Choice evaluation | Linear in bounded choices/conditions | Maximum-choice fixture | Within configured project budget |
| Provider registration | Bounded explicit registries | Stress fixture | No unbounded registry/history |
| History | Fixed-capacity ring buffer | Long conversation | Never grows without bound |
| Snapshot export/import | Detached bounded DTO | Maximum state fixture | No scene/object graph capture |
| Diagnostics | On-change/request, rate-limited | Stress fixture | No per-frame spam |

### 18.2 Allocation policy

- No LINQ in hot node/choice/condition paths unless profiling approves it.
- Immutable public view models/results do not expose internal pooled mutable buffers.
- Copy/bound provider payloads before asynchronous work.
- Reuse internal node lookup maps and buffers per immutable definition/version.
- Do not build diagnostic strings unless recording/exporting.
- No finalizer-based cleanup for handles, leases, presenter, or voice work.
- Editor graph scans may allocate but must expose progress/cancellation for large projects.

### 18.3 Scene and domain reload behavior

- Unsubscribe all providers/listeners on shutdown/domain reload.
- Reset static access under normal and disabled-domain-reload Play Mode.
- Reject duplicates before registrations.
- Cancel/safely settle session and clear pending requests, handles, histories, variables, and provider registries on a fresh root.
- Never write runtime state into definitions.
- Direct-scene helper cleans only its own development authority.

### 18.4 Scalability limits

Configuration must bound:

- nodes per conversation;
- choices per choice node;
- immediate transitions per yield/frame;
- session variables and payload entries;
- history entries;
- pending requests;
- suspension leases;
- provider/handler registrations;
- command/text/voice/presenter timeouts;
- snapshot and report size;
- alias/migration entries;
- diagnostic history/rate limits.

Advertised limits are finalized only after implementation stress evidence.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Dialogue content may contain unreleased story text, player names, pronouns, choices, accessibility-relevant copy, private narrative flags, and command arguments. Ordinary diagnostics therefore use stable IDs, counts, statuses, and codes rather than resolved text or payload values.

### 19.2 Trust boundaries

- Definitions are trusted project/build content, not arbitrary player scripts.
- No raw script, expression language, reflection method name, or arbitrary assembly invocation exists in core commands.
- Provider IDs and typed payloads are validated and bounded.
- Conditions are side-effect-free by contract; commands are explicit side-effect boundaries.
- Runtime import/mod content is absent from MVP.
- Snapshot inputs are versioned, size-limited, validated, migrated, and prepared before apply.
- Support exports redact production text, player-derived values, command payloads, save contents, and absolute paths.
- Dialogue choice does not imply purchase, legal consent, authentication, or entitlement unless a separate authoritative system validates it.

### 19.3 Platform behavior

| Platform | Compatibility status | Planned behavior | Required evidence |
|---|---|---|---|
| Windows | Planned | Core flow, sample presenter, providers, snapshots | Clean install/Player/Lab |
| macOS | Planned | Same neutral runtime behavior | Clean install/Player/Lab |
| Linux | Planned | Same neutral runtime behavior | Clean install/Player/Lab |
| WebGL | Planned | Async/provider and storage-bridge constraints | Browser Player tests |
| Android | Planned | Touch/UI handled by presenter/input bridge | Device/Player tests |
| iOS | Planned | Touch/UI handled by presenter/input bridge | Device/Player tests |
| Consoles | Unknown | Platform/UI/input/provider restrictions | Licensed platform evidence |

No platform becomes Supported before SFGSS-004 evidence.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-dialogue/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   └── Developer/
├── Runtime/
│   ├── Core/
│   ├── Definitions/
│   ├── Flow/
│   ├── Providers/
│   ├── Persistence/
│   ├── Diagnostics/
│   └── EchoDevGames.EchoDialogue.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Authoring/
│   ├── Validation/
│   ├── Inspectors/
│   └── EchoDevGames.EchoDialogue.Editor.asmdef
├── Samples~/
│   └── Voices Conversation Laboratory/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoDialogueRoot.cs
│   ├── IDialogueService.cs
│   ├── DialogueService.cs
│   ├── DialogueSession.cs
│   ├── DialogueSessionHandle.cs
│   └── DialogueResults.cs
├── Definitions/
│   ├── DialogueConfiguration.cs
│   ├── SpeakerDefinition.cs
│   ├── ConversationDefinition.cs
│   ├── DialogueNodeRecord.cs
│   ├── DialogueChoiceRecord.cs
│   └── DialogueReferences.cs
├── Flow/
│   ├── DialogueNodeRunner.cs
│   ├── DialogueChoiceEvaluator.cs
│   ├── DialogueVariableStore.cs
│   ├── DialogueAdmissionPolicy.cs
│   └── DialogueHistoryBuffer.cs
├── Providers/
│   ├── IDialoguePresenter.cs
│   ├── IDialogueTextProvider.cs
│   ├── IDialogueVoiceProvider.cs
│   ├── IDialogueConditionProvider.cs
│   ├── IDialogueCommandHandler.cs
│   └── SourceDialogueTextProvider.cs
├── Persistence/
│   ├── DialogueSessionSnapshot.cs
│   ├── DialogueSnapshotMigrator.cs
│   └── DialoguePreparedSnapshot.cs
└── Diagnostics/
    ├── DialogueDiagnosticCode.cs
    └── DialogueDiagnosticSnapshot.cs
```

File names are proposed and may change only through checkpoint/spec reconciliation. No files are authorized by this document alone.

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoDialogue.Runtime` | Runtime | Unity core only | Yes | Neutral public/runtime authority |
| `EchoDevGames.EchoDialogue.Editor` | Editor | Runtime, UnityEditor | No | Setup, authoring, validation, ADR-001 facade |
| `EchoDevGames.EchoDialogue.Tests.Runtime` | Tests | Runtime, Test Framework | No | EditMode/PlayMode runtime tests |
| `EchoDevGames.EchoDialogue.Tests.Editor` | Editor tests | Runtime, Editor, Test Framework | No | Authoring/setup/validation tests |
| `EchoDevGames.EchoDialogue.Samples.Laboratory` | Sample | Runtime plus declared sample presentation dependencies | No | Standalone Laboratory only |

Optional bridge/provider assemblies live separately under SFGSS-002 when they create peer dependencies.

### 20.4 Repository files

- Concise README routing to documentation.
- Installation, quick start, Laboratory, API, authoring, provider, troubleshooting, migration, removal, and limitation guides.
- Current Notes and checkpoint history.
- Changelog, license, third-party notices, support/security guidance.
- Release checklist and test evidence registry.
- Stable `.meta` files and GUIDs for public scripts, assets, samples, and templates.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Status/notes |
|---|---|---|---|
| Unity | 6000.0 planned | 6000.3.8f1 planned baseline | No clean-project evidence yet |
| SFGSS standards | 002/003/004 v1.0.0, 005 v1.1.0 | Documentation review | Design authority only |
| Optional peers | Per future bridge manifest/specification | Not tested | No support claim yet |

### 21.2 Semantic versioning policy

- Patch: defect/documentation fixes with no public behavior, schema, ID, or migration break.
- Minor: backward-compatible node/provider/API additions, new validators, samples, or optional integrations.
- Major: breaking API, node schema, provider contract, snapshot, stable-ID, definition, or behavior changes.
- Package source version and durable schema versions are separate.

### 21.3 Deprecation policy

- Public API/serialized field/node/provider deprecations are documented and preserved for at least one supported minor line when practical.
- Obsolete APIs emit compile/editor guidance, not per-frame runtime spam.
- Stable ID changes require aliases/migrations.
- Removed node kinds or payloads require migration or explicit unsupported behavior.
- Major removal notes include bridge/project migration and rollback.

### 21.4 GUID and asset compatibility

Public scripts, definitions, root prefabs, templates, and samples preserve `.meta` GUIDs when identity survives. Moves/renames retain GUIDs. Unity asset GUID preservation does not replace domain stable IDs.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview, boundaries, and terminology.
- Installation and five-minute linear conversation quick start.
- Speaker and conversation authoring guide.
- Line, choice, condition, command, interruption, and snapshot guide.
- Standalone Laboratory guide.
- Presenter and provider integration examples.
- Diagnostics code reference and troubleshooting.
- Upgrade/migration/removal guide.
- Optional bridge index.
- Known limitations, license, credits, notices.

### 22.2 Required developer documentation

- Authority/root/session lifecycle.
- Stable IDs and node record schema.
- Provider registration and teardown.
- Condition purity and command commit contract.
- Presenter generations and stale-result handling.
- Snapshot safe points/migration.
- Testing/evidence strategy.
- Release workflow, ADRs, Current Notes, checkpoint status.

### 22.3 Documentation truth rule

Examples must compile against the documented release. Screenshots/menu paths must match the tested Unity baseline. Planned tests, platform claims, performance limits, and bridge behavior remain `Not run`/Planned until evidence exists.

### 22.4 Living repository and Obsidian workflow

Use the repository files directly in Obsidian. Capture discoveries in `Current Notes.md`, then promote durable decisions into the specification/ADR/test/issue/guide/changelog at checkpoint closeout. Git history is the archive.

### 22.5 Repository scan and handoff order

1. README.
2. SFGSS-000.
3. This approved package specification.
4. SFGSS-002, SFGSS-003, SFGSS-004, SFGSS-005.
5. Applicable bridge/integration specifications and ADRs.
6. Current Notes and active checkpoint/test records.
7. Relevant implementation and tests once authorized.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, graph validation, policies, values, migrations | Pure fixtures | Yes |
| PlayMode unit/integration | Root, session, providers, presentation, commands, snapshots | Fake provider fixtures | Yes |
| Standalone Laboratory | User-visible isolated conversation loop | 44 scenarios | Yes |
| Bridge Integration Lab | Localization/UI/audio/state/input/objective/save/camera | Separate bridge evidence | When bridge ships |
| Showcase | Combined narrative scene | Demonstration only | No |
| Clean-project install | Manifest/asmdef/setup/removal | Git/local/tarball | Yes |
| Existing-project adoption | Import/convert content without regression | Project migration fixture | Before adoption claim |
| Platform Player | Runtime/provider/presenter behavior | Windows/macOS/Linux/WebGL/mobile | Before support claim |

### 23.2 Required test categories

- Installation and assembly isolation.
- Authority, duplicate protection, lifecycle, direct-scene, domain reload.
- Definitions, IDs, aliases, graph integrity, validation.
- Lines, choices, conditions, commands, local state, waits, ends.
- Admission, queueing, interruption, suspension, cancellation, timeouts.
- Presenter/text/voice provider failures and stale generations.
- Semantic history, safe snapshots, migration, Chronicle bridge.
- Setup, authoring, repair, removal, reinstall.
- Optional bridge absence/presence and bridge-first removal.
- Diagnostics, privacy, failure isolation.
- Performance, allocations, scale, platform builds.

### 23.3 Test case registry

| Test ID | Category | Requirement/action | Status |
|---|---|---|---|
| EDLG-T-001 | Installation and assembly | Install from a Git URL into a clean supported Unity project. | Not run |
| EDLG-T-002 | Installation and assembly | Install from a local package path into a clean project. | Not run |
| EDLG-T-003 | Installation and assembly | Install from a tarball into a clean project. | Not run |
| EDLG-T-004 | Installation and assembly | Embed the package for package development. | Not run |
| EDLG-T-005 | Installation and assembly | Compile Runtime without any peer Echo package installed. | Not run |
| EDLG-T-006 | Installation and assembly | Compile Editor without any peer Echo package installed. | Not run |
| EDLG-T-007 | Installation and assembly | Compile Runtime with no UnityEditor reference. | Not run |
| EDLG-T-008 | Installation and assembly | Verify the neutral Runtime assembly does not reference uGUI, TextMeshPro, EchoUI, EchoLocalization, Jukebot, EchoObjectives, EchoCamera, EchoGameState, EchoInput, or EchoSave. | Not run |
| EDLG-T-009 | Installation and assembly | Import the Standalone Laboratory sample without changing package source. | Not run |
| EDLG-T-010 | Installation and assembly | Remove the Standalone Laboratory sample without breaking Runtime or Editor assemblies. | Not run |
| EDLG-T-011 | Installation and assembly | Remove EchoDialogue after removing its bridges and confirm the project compiles. | Not run |
| EDLG-T-012 | Installation and assembly | Reinstall EchoDialogue and reopen supported project-owned definitions. | Not run |
| EDLG-T-013 | Authority and lifecycle | Create one configured root and initialize successfully. | Not run |
| EDLG-T-014 | Authority and lifecycle | Reject a duplicate root in the same scene before providers or presenters register. | Not run |
| EDLG-T-015 | Authority and lifecycle | Reject a duplicate root introduced during scene load before side effects. | Not run |
| EDLG-T-016 | Authority and lifecycle | Persist the configured application-session authority across scene transitions. | Not run |
| EDLG-T-017 | Authority and lifecycle | Enter the Standalone Laboratory directly and create only the configured development authority. | Not run |
| EDLG-T-018 | Authority and lifecycle | Adopt an already valid root during direct-scene entry. | Not run |
| EDLG-T-019 | Authority and lifecycle | Disable direct-scene initialization and report a missing authority. | Not run |
| EDLG-T-020 | Authority and lifecycle | Shutdown with no active session. | Not run |
| EDLG-T-021 | Authority and lifecycle | Shutdown during line presentation and settle the session exactly once. | Not run |
| EDLG-T-022 | Authority and lifecycle | Shutdown during a command and honor the command commit boundary. | Not run |
| EDLG-T-023 | Authority and lifecycle | Reinitialize after a clean shutdown where supported. | Not run |
| EDLG-T-024 | Authority and lifecycle | Run with domain reload enabled. | Not run |
| EDLG-T-025 | Authority and lifecycle | Run with domain reload disabled and reset package static state. | Not run |
| EDLG-T-026 | Authority and lifecycle | Unsubscribe all registered callbacks on shutdown. | Not run |
| EDLG-T-027 | Authority and lifecycle | Reject API calls after shutdown with structured results. | Not run |
| EDLG-T-028 | Definitions, IDs, and validation | Create a SpeakerDefinition with a generated stable SpeakerId. | Not run |
| EDLG-T-029 | Definitions, IDs, and validation | Detect duplicate SpeakerIds across the project scan. | Not run |
| EDLG-T-030 | Definitions, IDs, and validation | Create a ConversationDefinition with a generated stable ConversationId. | Not run |
| EDLG-T-031 | Definitions, IDs, and validation | Detect duplicate ConversationIds across the project scan. | Not run |
| EDLG-T-032 | Definitions, IDs, and validation | Generate stable NodeIds that survive display-name changes. | Not run |
| EDLG-T-033 | Definitions, IDs, and validation | Detect duplicate NodeIds inside one conversation. | Not run |
| EDLG-T-034 | Definitions, IDs, and validation | Detect an empty or missing entry NodeId. | Not run |
| EDLG-T-035 | Definitions, IDs, and validation | Detect a next-node reference that targets no node. | Not run |
| EDLG-T-036 | Definitions, IDs, and validation | Detect a choice edge that targets no node. | Not run |
| EDLG-T-037 | Definitions, IDs, and validation | Detect a required speaker reference that cannot be resolved. | Not run |
| EDLG-T-038 | Definitions, IDs, and validation | Detect a text reference with an unregistered required provider. | Not run |
| EDLG-T-039 | Definitions, IDs, and validation | Detect a command reference with an unregistered required handler. | Not run |
| EDLG-T-040 | Definitions, IDs, and validation | Detect a condition reference with an unregistered required provider. | Not run |
| EDLG-T-041 | Definitions, IDs, and validation | Detect an immediate transition cycle that has no yield and no explicit allowance. | Not run |
| EDLG-T-042 | Definitions, IDs, and validation | Permit an authored loop that yields and remains inside configured budgets. | Not run |
| EDLG-T-043 | Definitions, IDs, and validation | Preserve stable IDs when assets and display labels are renamed. | Not run |
| EDLG-T-044 | Conversation flow and line presentation | Start a valid conversation at its entry node. | Not run |
| EDLG-T-045 | Conversation flow and line presentation | Complete a one-line conversation. | Not run |
| EDLG-T-046 | Conversation flow and line presentation | Traverse a deterministic multi-line sequence. | Not run |
| EDLG-T-047 | Conversation flow and line presentation | Present line data with speaker, text, portrait token, emotion token, and optional voice token. | Not run |
| EDLG-T-048 | Conversation flow and line presentation | Resolve source fallback text without an external provider. | Not run |
| EDLG-T-049 | Conversation flow and line presentation | Resolve text through a registered provider. | Not run |
| EDLG-T-050 | Conversation flow and line presentation | Report a missing optional text entry without crashing the session. | Not run |
| EDLG-T-051 | Conversation flow and line presentation | Fail a required text entry according to the authored policy. | Not run |
| EDLG-T-052 | Conversation flow and line presentation | Advance manually after presenter completion. | Not run |
| EDLG-T-053 | Conversation flow and line presentation | Auto-advance after an unscaled duration. | Not run |
| EDLG-T-054 | Conversation flow and line presentation | Distinguish typewriter completion from line advance. | Not run |
| EDLG-T-055 | Conversation flow and line presentation | Distinguish skip-current-line from advance-to-next-node. | Not run |
| EDLG-T-056 | Conversation flow and line presentation | Reject advance while no line is awaiting advance. | Not run |
| EDLG-T-057 | Conversation flow and line presentation | Reject a stale advance generation. | Not run |
| EDLG-T-058 | Conversation flow and line presentation | Execute a Wait node using the injected unscaled clock. | Not run |
| EDLG-T-059 | Conversation flow and line presentation | Cancel a Wait node before completion. | Not run |
| EDLG-T-060 | Conversation flow and line presentation | Enforce the immediate-transition step budget. | Not run |
| EDLG-T-061 | Conversation flow and line presentation | End with a project-defined completion result code. | Not run |
| EDLG-T-062 | Conversation flow and line presentation | Raise semantic line-started and line-completed events after state publication. | Not run |
| EDLG-T-063 | Conversation flow and line presentation | Isolate listener exceptions from authoritative flow. | Not run |
| EDLG-T-064 | Conversation flow and line presentation | Handle a missing presenter with a structured failure. | Not run |
| EDLG-T-065 | Conversation flow and line presentation | Recover to Idle after a complete conversation. | Not run |
| EDLG-T-066 | Choices and conditions | Present enabled choices in authored stable order. | Not run |
| EDLG-T-067 | Choices and conditions | Hide a choice when its condition is false and policy is Hide. | Not run |
| EDLG-T-068 | Choices and conditions | Disable a choice when its condition is false and policy is Disable. | Not run |
| EDLG-T-069 | Choices and conditions | Resolve a disabled-reason text reference. | Not run |
| EDLG-T-070 | Choices and conditions | Accept a valid current-generation choice selection. | Not run |
| EDLG-T-071 | Choices and conditions | Reject a choice ID that is not in the current option snapshot. | Not run |
| EDLG-T-072 | Choices and conditions | Reject a stale choice-generation selection. | Not run |
| EDLG-T-073 | Choices and conditions | Reject selection while not awaiting a choice. | Not run |
| EDLG-T-074 | Choices and conditions | Route to the selected choice target node. | Not run |
| EDLG-T-075 | Choices and conditions | Use a built-in session-variable equality condition. | Not run |
| EDLG-T-076 | Choices and conditions | Use a built-in visited-node condition. | Not run |
| EDLG-T-077 | Choices and conditions | Use a built-in prior-choice condition. | Not run |
| EDLG-T-078 | Choices and conditions | Evaluate a registered project condition provider. | Not run |
| EDLG-T-079 | Choices and conditions | Handle provider result True. | Not run |
| EDLG-T-080 | Choices and conditions | Handle provider result False. | Not run |
| EDLG-T-081 | Choices and conditions | Handle provider result Unavailable through authored policy. | Not run |
| EDLG-T-082 | Choices and conditions | Handle provider result Error through authored policy. | Not run |
| EDLG-T-083 | Choices and conditions | Prevent conditions from mutating conversation or project state. | Not run |
| EDLG-T-084 | Choices and conditions | Rebuild choices after snapshot restore and issue a new generation. | Not run |
| EDLG-T-085 | Choices and conditions | Use an authored fallback when no choice is enabled. | Not run |
| EDLG-T-086 | Commands and local state | Execute a required registered command handler. | Not run |
| EDLG-T-087 | Commands and local state | Execute an optional registered command handler. | Not run |
| EDLG-T-088 | Commands and local state | Continue after optional command failure when configured. | Not run |
| EDLG-T-089 | Commands and local state | Route after required command failure when configured. | Not run |
| EDLG-T-090 | Commands and local state | End the conversation after blocking command failure when configured. | Not run |
| EDLG-T-091 | Commands and local state | Time out an asynchronous command through the injected clock. | Not run |
| EDLG-T-092 | Commands and local state | Cancel a command before its declared commit point. | Not run |
| EDLG-T-093 | Commands and local state | Report TooLate after a handler passes its irreversible commit point. | Not run |
| EDLG-T-094 | Commands and local state | Reject a missing required command handler. | Not run |
| EDLG-T-095 | Commands and local state | Skip a missing optional command handler with an advisory. | Not run |
| EDLG-T-096 | Commands and local state | Validate command payload values against the handler schema. | Not run |
| EDLG-T-097 | Commands and local state | Redact command payload values from ordinary diagnostics. | Not run |
| EDLG-T-098 | Commands and local state | Set a conversation-local Boolean variable. | Not run |
| EDLG-T-099 | Commands and local state | Set a conversation-local integer variable. | Not run |
| EDLG-T-100 | Commands and local state | Set a conversation-local stable-ID value. | Not run |
| EDLG-T-101 | Commands and local state | Clear a conversation-local variable. | Not run |
| EDLG-T-102 | Commands and local state | Enforce the configured local-variable limit. | Not run |
| EDLG-T-103 | Commands and local state | Preserve local variables in an approved active-session snapshot. | Not run |
| EDLG-T-104 | Commands and local state | Prevent a command handler from reentering the same session synchronously. | Not run |
| EDLG-T-105 | Commands and local state | Raise command-completed events after the handler result is committed. | Not run |
| EDLG-T-106 | Admission, interruption, suspension, and cancellation | Reject a new conversation under RejectNew while one is active. | Not run |
| EDLG-T-107 | Admission, interruption, suspension, and cancellation | Queue the latest request under QueueLatest. | Not run |
| EDLG-T-108 | Admission, interruption, suspension, and cancellation | Replace an older pending request under QueueLatest. | Not run |
| EDLG-T-109 | Admission, interruption, suspension, and cancellation | Reject queue admission when the configured queue is full. | Not run |
| EDLG-T-110 | Admission, interruption, suspension, and cancellation | Replace an active interruptible conversation under ReplaceActive. | Not run |
| EDLG-T-111 | Admission, interruption, suspension, and cancellation | Reject replacement of a non-interruptible conversation. | Not run |
| EDLG-T-112 | Admission, interruption, suspension, and cancellation | Cancel an active session by current generational handle. | Not run |
| EDLG-T-113 | Admission, interruption, suspension, and cancellation | Reject cancellation through a stale session handle. | Not run |
| EDLG-T-114 | Admission, interruption, suspension, and cancellation | Reject cancellation through a foreign-root handle. | Not run |
| EDLG-T-115 | Admission, interruption, suspension, and cancellation | Acquire one suspension lease and suspend presentation. | Not run |
| EDLG-T-116 | Admission, interruption, suspension, and cancellation | Acquire multiple suspension leases and resume only after all release. | Not run |
| EDLG-T-117 | Admission, interruption, suspension, and cancellation | Release suspension leases out of order safely. | Not run |
| EDLG-T-118 | Admission, interruption, suspension, and cancellation | Dispose a suspension lease twice safely. | Not run |
| EDLG-T-119 | Admission, interruption, suspension, and cancellation | Reject new advance or choice input while suspended. | Not run |
| EDLG-T-120 | Admission, interruption, suspension, and cancellation | Re-present the current safe node after resume when required. | Not run |
| EDLG-T-121 | Admission, interruption, suspension, and cancellation | Settle each admitted request exactly once during shutdown. | Not run |
| EDLG-T-122 | History, snapshots, and persistence | Append a semantic line history record without resolved production text. | Not run |
| EDLG-T-123 | History, snapshots, and persistence | Append a semantic choice history record. | Not run |
| EDLG-T-124 | History, snapshots, and persistence | Append a command result history record with redacted payload. | Not run |
| EDLG-T-125 | History, snapshots, and persistence | Bound the history to configured capacity. | Not run |
| EDLG-T-126 | History, snapshots, and persistence | Resolve display history on demand through the current text provider. | Not run |
| EDLG-T-127 | History, snapshots, and persistence | Export a snapshot at a line safe point. | Not run |
| EDLG-T-128 | History, snapshots, and persistence | Export a snapshot at a choice safe point. | Not run |
| EDLG-T-129 | History, snapshots, and persistence | Reject snapshot export during an unsafe command commit phase. | Not run |
| EDLG-T-130 | History, snapshots, and persistence | Prepare and validate a supported snapshot without mutating live state. | Not run |
| EDLG-T-131 | History, snapshots, and persistence | Apply a prepared snapshot atomically. | Not run |
| EDLG-T-132 | History, snapshots, and persistence | Restore conversation ID, node ID, local variables, history, and authored metadata. | Not run |
| EDLG-T-133 | History, snapshots, and persistence | Reissue line and choice generations after restore. | Not run |
| EDLG-T-134 | History, snapshots, and persistence | Handle a missing conversation definition during restore. | Not run |
| EDLG-T-135 | History, snapshots, and persistence | Handle a missing node through alias/migration lookup. | Not run |
| EDLG-T-136 | History, snapshots, and persistence | Preserve unknown extension records in the snapshot. | Not run |
| EDLG-T-137 | History, snapshots, and persistence | Migrate from the immediately previous snapshot schema fixture. | Not run |
| EDLG-T-138 | History, snapshots, and persistence | Reject an unsupported newer snapshot without destructive downgrade. | Not run |
| EDLG-T-139 | History, snapshots, and persistence | Integrate with Chronicle through a separate participant bridge. | Not run |
| EDLG-T-140 | Editor authoring and setup | Create a package configuration through the setup window. | Not run |
| EDLG-T-141 | Editor authoring and setup | Create a root prefab without overwriting an existing project root. | Not run |
| EDLG-T-142 | Editor authoring and setup | Create an empty SpeakerDefinition template. | Not run |
| EDLG-T-143 | Editor authoring and setup | Create an empty ConversationDefinition template. | Not run |
| EDLG-T-144 | Editor authoring and setup | Create a sample source-text provider and sample conversation only after approval. | Not run |
| EDLG-T-145 | Editor authoring and setup | Repeat setup without duplicating assets or roots. | Not run |
| EDLG-T-146 | Editor authoring and setup | Open the conversation graph/list editor. | Not run |
| EDLG-T-147 | Editor authoring and setup | Create and connect built-in node kinds. | Not run |
| EDLG-T-148 | Editor authoring and setup | Duplicate a node while generating a fresh NodeId. | Not run |
| EDLG-T-149 | Editor authoring and setup | Delete a node only after showing inbound edge impact. | Not run |
| EDLG-T-150 | Editor authoring and setup | Validate unreachable nodes. | Not run |
| EDLG-T-151 | Editor authoring and setup | Validate broken edges. | Not run |
| EDLG-T-152 | Editor authoring and setup | Validate missing speakers and providers. | Not run |
| EDLG-T-153 | Editor authoring and setup | Validate choice nodes with no enabled/fallback route. | Not run |
| EDLG-T-154 | Editor authoring and setup | Validate immediate cycles and step budgets. | Not run |
| EDLG-T-155 | Editor authoring and setup | Preview a repair plan before changing project-owned definitions. | Not run |
| EDLG-T-156 | Editor authoring and setup | Generate a redacted validation report. | Not run |
| EDLG-T-157 | Editor authoring and setup | Expose the ADR-001 Workshop setup facade. | Not run |
| EDLG-T-158 | Integrations and removal | Run standalone with no optional peer package installed. | Not run |
| EDLG-T-159 | Integrations and removal | Resolve text through the Many Tongues bridge without changing dialogue flow ownership. | Not run |
| EDLG-T-160 | Integrations and removal | Present lines and choices through the Looking Glass bridge without giving UI dialogue authority. | Not run |
| EDLG-T-161 | Integrations and removal | Play optional voice cues through the Resonance bridge without moving playback into EchoDialogue. | Not run |
| EDLG-T-162 | Integrations and removal | Acquire and release a Pulse dialogue scope through a bridge. | Not run |
| EDLG-T-163 | Integrations and removal | Route player advance and choice intent through a Will bridge without reading input in the core. | Not run |
| EDLG-T-164 | Integrations and removal | Publish dialogue-driven objective requests through a Path bridge without completing quests in the core. | Not run |
| EDLG-T-165 | Integrations and removal | Publish semantic camera cue requests through an Eye bridge without moving camera authority. | Not run |
| EDLG-T-166 | Integrations and removal | Persist an active-session snapshot through the Chronicle bridge. | Not run |
| EDLG-T-167 | Integrations and removal | Expose package health through an Observatory provider bridge. | Not run |
| EDLG-T-168 | Integrations and removal | Initialize the root through a First Light startup-step bridge. | Not run |
| EDLG-T-169 | Integrations and removal | Expose setup planning through the Workshop facade. | Not run |
| EDLG-T-170 | Integrations and removal | Remove the Many Tongues bridge and retain source-fallback dialogue. | Not run |
| EDLG-T-171 | Integrations and removal | Remove the Looking Glass bridge and retain core compilation. | Not run |
| EDLG-T-172 | Integrations and removal | Remove the Resonance bridge and retain silent dialogue flow. | Not run |
| EDLG-T-173 | Integrations and removal | Remove the Chronicle bridge and preserve standalone snapshot export. | Not run |
| EDLG-T-174 | Integrations and removal | Remove all bridges before removing EchoDialogue. | Not run |
| EDLG-T-175 | Integrations and removal | Reinstall EchoDialogue and reopen project-owned definitions. | Not run |
| EDLG-T-176 | Diagnostics, privacy, and failure isolation | Capture a snapshot while uninitialized. | Not run |
| EDLG-T-177 | Diagnostics, privacy, and failure isolation | Capture a snapshot while Idle. | Not run |
| EDLG-T-178 | Diagnostics, privacy, and failure isolation | Capture a snapshot during line presentation. | Not run |
| EDLG-T-179 | Diagnostics, privacy, and failure isolation | Capture a snapshot while awaiting a choice. | Not run |
| EDLG-T-180 | Diagnostics, privacy, and failure isolation | Report root ID, package version, conversation ID, node ID, phase, queue count, suspension count, and last result. | Not run |
| EDLG-T-181 | Diagnostics, privacy, and failure isolation | Report provider and handler registration health. | Not run |
| EDLG-T-182 | Diagnostics, privacy, and failure isolation | Report bounded history and active-session counts. | Not run |
| EDLG-T-183 | Diagnostics, privacy, and failure isolation | Exclude resolved production text from ordinary diagnostics. | Not run |
| EDLG-T-184 | Diagnostics, privacy, and failure isolation | Exclude formatting arguments and command payload values from ordinary diagnostics. | Not run |
| EDLG-T-185 | Diagnostics, privacy, and failure isolation | Redact absolute project paths from support exports. | Not run |
| EDLG-T-186 | Diagnostics, privacy, and failure isolation | Rate-limit repeated missing-provider warnings. | Not run |
| EDLG-T-187 | Diagnostics, privacy, and failure isolation | Bound diagnostic history. | Not run |
| EDLG-T-188 | Diagnostics, privacy, and failure isolation | Isolate a diagnostics listener exception. | Not run |
| EDLG-T-189 | Diagnostics, privacy, and failure isolation | Generate a portable redacted support snapshot. | Not run |
| EDLG-T-190 | Performance and scalability | Perform no per-frame package polling while Idle. | Not run |
| EDLG-T-191 | Performance and scalability | Run a minimum one-line conversation and record allocations/timing. | Not run |
| EDLG-T-192 | Performance and scalability | Run a 100-node conversation fixture and record traversal timing. | Not run |
| EDLG-T-193 | Performance and scalability | Validate the advertised maximum-node fixture without unbounded memory growth. | Not run |
| EDLG-T-194 | Performance and scalability | Present the advertised maximum choices in one node. | Not run |
| EDLG-T-195 | Performance and scalability | Evaluate the advertised condition count within configured budgets. | Not run |
| EDLG-T-196 | Performance and scalability | Execute repeated immediate nodes and enforce the step budget. | Not run |
| EDLG-T-197 | Performance and scalability | Bound the pending conversation queue. | Not run |
| EDLG-T-198 | Performance and scalability | Bound registered providers, handlers, presenters, variables, and history. | Not run |
| EDLG-T-199 | Performance and scalability | Cancel and restart conversations repeatedly without leaked registrations. | Not run |
| EDLG-T-200 | Performance and scalability | Profile source-text resolution and external-provider resolution separately. | Not run |
| EDLG-T-201 | Performance and scalability | Profile snapshot export/import at the advertised state size. | Not run |
| EDLG-T-202 | Migration, platform, and release | Migrate package configuration from the immediately previous schema fixture. | Not run |
| EDLG-T-203 | Migration, platform, and release | Migrate SpeakerDefinition aliases from the immediately previous fixture. | Not run |
| EDLG-T-204 | Migration, platform, and release | Migrate ConversationDefinition and NodeId aliases from the immediately previous fixture. | Not run |
| EDLG-T-205 | Migration, platform, and release | Preserve unknown configuration extension records. | Not run |
| EDLG-T-206 | Migration, platform, and release | Preserve project-owned definitions during package upgrade. | Not run |
| EDLG-T-207 | Migration, platform, and release | Detect unsupported newer configuration without destructive downgrade. | Not run |
| EDLG-T-208 | Migration, platform, and release | Remove EchoDialogue through an explicit bridge-first removal plan. | Not run |
| EDLG-T-209 | Migration, platform, and release | Reinstall and reopen supported project-owned data. | Not run |
| EDLG-T-210 | Migration, platform, and release | Run the Windows clean-project and Player fixture. | Not run |
| EDLG-T-211 | Migration, platform, and release | Run the macOS clean-project and Player fixture. | Not run |
| EDLG-T-212 | Migration, platform, and release | Run the Linux clean-project and Player fixture. | Not run |
| EDLG-T-213 | Migration, platform, and release | Run the WebGL Player fixture. | Not run |
| EDLG-T-214 | Migration, platform, and release | Run the Android Player fixture. | Not run |
| EDLG-T-215 | Migration, platform, and release | Run the iOS Player fixture. | Not run |
| EDLG-T-216 | Migration, platform, and release | Record console compatibility as Unknown until platform evidence exists. | Not run |
| EDLG-T-217 | Migration, platform, and release | Verify release documentation does not claim unexecuted support. | Not run |

### 23.4 Evidence rules

- Every row is a planned definition, not a pass.
- Each execution records environment, Unity/package versions, commit, result, evidence, and issues.
- Retrying does not erase failure history.
- A conversation completing does not prove localization, UI, audio, objective, save, or platform integration.
- A bridge compile does not prove runtime behavior.
- Platform status remains Planned/Unknown until its execution set passes.

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership/non-ownership approved.
- [x] Standalone source-provider and presenter path approved.
- [x] Stable speaker/conversation/node/choice identity approved.
- [x] Node, condition, command, interruption, history, snapshot, diagnostics, and Laboratory contracts approved.
- [x] Optional integrations separated.
- [x] Planned evidence remains Not run.
- [x] Jesse approved through the documentation-first workflow.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared hard dependencies only.
- [ ] Editor code is isolated.
- [ ] Duplicate protection occurs before side effects.
- [ ] Provider/handler registrations and teardown are exact.
- [ ] Commands honor timeout/cancellation/commit contracts.
- [ ] Public API and schemas match specification or docs/ADR change first.

### 24.3 Standalone gate

- [ ] Clean installation succeeds.
- [ ] Source text + sample presenter complete core loop alone.
- [ ] Laboratory scenarios pass.
- [ ] Samples remove safely.
- [ ] Direct-scene behavior passes.

### 24.4 Quality gate

- [ ] Automated tests pass.
- [ ] Manual checklists pass.
- [ ] No blocker/critical defects remain.
- [ ] Planned performance limits have evidence.
- [ ] Diagnostics are actionable/privacy-safe.
- [ ] Documentation matches build.
- [ ] Current Notes reconciled.
- [ ] Licenses/notices complete.

### 24.5 Distribution gate

- [ ] Manifest/version/changelog valid.
- [ ] Stable `.meta` files present.
- [ ] Git/local/tarball installs tested.
- [ ] Removal/reinstall tested.
- [ ] Migration fixtures pass.
- [ ] Beta, release-candidate, and stable SFGSS-004 evidence gates are satisfied for claimed release stage.
- [ ] Compatibility catalog updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Hackulos | Planned quest/NPC dialogue | Build standalone package, then one quest-giver conversation through project conditions/commands | Line/choice/quest request/save parity | Keep project dialogue scripts/data until proven |
| Rescuers2D | Scene/UI instructions and interactions | Integrate one survivor/instruction flow without replacing unrelated UI | Same user-visible flow and input behavior | Retain old presenter/trigger |
| Echo Systems Lab | Portfolio interaction/mission text | Demonstrate package/provider separation | Isolated Lab and one project adapter | Remove adapter/package |

### 25.2 Preserve-until-parity rule

Working project dialogue remains intact until Voices passes standalone proof and one feature category at a time in the target project. Content conversion is previewed, backed up, reversible, and never silently deletes source scripts/assets.

### 25.3 Migration tooling

Later migration tools may:

- scan known project-specific line/speaker structures;
- generate a dry-run conversion report;
- create new project-owned definitions with fresh stable IDs;
- map old indexes/names to aliases;
- preserve source assets/backups;
- validate graph and provider dependencies;
- never execute gameplay commands during conversion.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EDLG-R-001 | Scope expands into cinematic/narrative framework | High | High | One foreground conversation MVP; explicit non-goals/providers | Specification review |
| EDLG-R-002 | UI becomes required authority | Medium | High | Neutral presenter contract and standalone sample | Assembly/Lab tests |
| EDLG-R-003 | Conditions mutate world state | Medium | High | Read-only synchronous contract; commands separate | Provider review/tests |
| EDLG-R-004 | Commands cannot cancel/rollback safely | Medium | High | Explicit commit point, timeout, failure policy | Handler tests/docs |
| EDLG-R-005 | Node IDs break saves after edits | Medium | High | Stable IDs, aliases, validation, migration fixtures | Authoring/migration tests |
| EDLG-R-006 | Immediate cycles hang a frame | Medium | High | Step budget, validation, yield rules | Stress tests |
| EDLG-R-007 | Stale UI selections mutate new state | Medium | High | Generational presenter/choice tokens | PlayMode tests |
| EDLG-R-008 | Raw narrative text leaks into logs | Medium | Medium/High | ID-only diagnostics, redacted exports | Privacy tests |
| EDLG-R-009 | Snapshot replays side effects | Medium | Critical | Safe points and committed-command markers | Save tests |
| EDLG-R-010 | Optional peers create circular dependencies | Medium | High | SFGSS-002 bridge separation | Assembly audit |
| EDLG-R-011 | Authoring tool corrupts project content | Low/Medium | High | Undo, dry-run, backups, no semantic auto-repair | Editor tests |
| EDLG-R-012 | One active session is too narrow | Medium | Medium | Honest MVP; channel concurrency deferred | Revisit after real integrations |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EDLG-D-001 | One foreground conversation session in MVP | Approved | Keeps focus/input/presentation deterministic | Barks/channels deferred | No |
| EDLG-D-002 | Stable explicit node-record union, not reflection-polymorphic command graph | Approved | Migration/validation clarity | New node kinds require schema/version work | No |
| EDLG-D-003 | Conditions are read-only and synchronous; commands own side effects | Approved | Preserves authority and option determinism | Expensive conditions must cache/precompute | No |
| EDLG-D-004 | Production presentation is external through one presenter contract | Approved | Core remains nonvisual/standalone | Sample presenter and UI bridge required separately | No |
| EDLG-D-005 | Source fallback text is supported without localization | Approved | Standalone proof and novice path | Production localization remains optional bridge |
| EDLG-D-006 | Semantic history stores references/IDs, not resolved text by default | Approved | Locale changes/privacy | UI resolves history on demand |
| EDLG-D-007 | Active-session saves only at explicit safe points | Approved | Prevent replay/partial state | Some moments cannot save dialogue state |
| EDLG-D-008 | Stale session/presenter/choice requests use generations | Approved | Prevent recycled/current-state corruption | Callers must retain current models/handles |
| EDLG-D-009 | Command handlers declare commit state | Approved | Honest cancellation and side-effect reporting | Handler documentation/testing burden |
| EDLG-D-010 | Diagnostic prefix is `EDLG-*` | Approved | Unique searchable package codes | Reserved across suite |

### 27.2 Release-blocking questions

None remain for specification approval. Implementation must still verify:

- exact sample presentation dependency packaging under SFGSS-002;
- final serialized tagged-value representation under Unity 6000;
- practical graph/list Editor technology;
- measured node/choice/snapshot limits;
- first bridge package IDs and compatibility ranges.

These are implementation/checkpoint decisions and evidence, not permission to alter authority silently.

### 27.3 Non-blocking later questions

- Whether ambient bark channels belong in EchoDialogue or a small companion module.
- Whether reusable sub-conversations/fragments warrant a minor release.
- Which narrative import/export formats deserve provider adapters.
- Whether lip-sync and VO recording workflows become separate packages/providers.
- Multiplayer dialogue voting after provider research.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved contract | This document | Approved v1.0.0 |
| M1 - Skeleton | Installable package anatomy | Manifest, asmdefs, docs shell | Clean compile/install |
| M2 - Definitions/validation | Speakers, conversations, nodes, IDs, validators | Data + EditMode | Unit tests |
| M3 - Runtime core | Root, session, line/choice/branch/local/end | PlayMode core | Automated tests |
| M4 - Providers/commands | Presenter/text/condition/command/voice contracts | Failure/cancel/timeout | Provider tests |
| M5 - Interruption/history/snapshots | Admission, suspension, history, safe persistence | Lifecycle/migration | Tests |
| M6 - Laboratory/tooling | Authoring, setup, repair, isolated Lab | Repeatability/manual evidence | Lab checklist |
| M7 - First bridges/adoption | Selected UI/localization/audio/save/project integration | Separate bridge/project parity | Integration evidence |
| M8 - Release | Distribution-ready package | Docs, notices, compatibility, tarball | Release gates |

### 28.2 Checkpoint rule

Each implementation milestone is split into SFGSS-005 learning-oriented checkpoints with complete visible code, exact file paths, architectural explanations, Editor setup, tests, stop points, Current Notes reconciliation, and Git closeout.

### 28.3 First recommended implementation checkpoint

Dormant until SUITE-DOC-33:

> **EDLG-M1-01 - Voices Package Skeleton**: create only package manifest, Runtime/Editor/test asmdefs, documentation shell, and compile/install tests. No dialogue C# behavior until that checkpoint is approved and executed.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.

Treat SFGSS-000 as suite authority and the approved Voices (`EchoDialogue`)
Package Specification as the Level 2 authority for speakers, conversations,
node flow, choices, conditions, commands, interruption, history, snapshots,
authoring, diagnostics, Laboratories, and release gates. Follow SFGSS-002 for
dependencies/bridges/assemblies, SFGSS-003 for IDs/data/migration, SFGSS-004
for evidence/release truth, and SFGSS-005 for implementation teaching workflow.

Current package: EchoDialogue
Specification: v1.0.0 Approved
Implementation: locked until SUITE-DOC-33
Current documentation checkpoint: SUITE-DOC-11 - EchoObjectives (`The Path`)

Before writing code:
1. Preserve one foreground conversation authority.
2. Keep conditions read-only and commands explicit/failure-aware.
3. Keep UI, localization, audio, input, game state, objectives, camera, scene
   travel, and save transport behind providers/bridges.
4. Preserve stable speaker/conversation/node/choice IDs and safe snapshot points.
5. Keep all empirical evidence Not run until observed.
6. When code is eventually authorized, show complete files and explain every step.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | 1.0.0 Approved |
| Completed checkpoint | SUITE-DOC-10 - Voices specification |
| Files/assets created | Documentation only |
| Tests passed | Documentation structure/ID/archive audits only |
| Tests failed | None in documentation audit |
| Runtime tests | All Not run |
| Known issues | Implementation evidence and bridge IDs pending |
| Next checkpoint | SUITE-DOC-11 - The Path (`EchoObjectives`) specification |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity and responsibility are clear.
- [x] Ownership/non-ownership align with SFGSS-000.
- [x] Independence proof is credible.
- [x] MVP is bounded and useful.
- [x] Definitions, IDs, node model, lifecycle, providers, public API, failure behavior, snapshots, tooling, Laboratory, diagnostics, tests, and release gates are defined.
- [x] Optional integrations are separated.
- [x] No Isekai identity/dependency introduced.
- [x] Implementation remains locked.
- [x] Jesse approved through the active documentation workflow.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** All implementation, compatibility, performance, migration, platform, bridge, adoption, and release evidence remains `Not run`. Any authority/schema change requires specification/ADR reconciliation before code.

---

## Specification Completion Statement

A new collaborator can determine from this document:

1. what Voices owns and refuses to own;
2. why one foreground conversation is the MVP;
3. how speakers, conversations, nodes, choices, conditions, commands, and local variables are identified and executed;
4. how source/localized text, UI, audio, input, state, objectives, camera, save, and setup connect without core dependencies;
5. how interruption, suspension, cancellation, timeouts, and stale generations behave;
6. what history and active-session data may be saved and at which safe points;
7. how invalid graphs and provider failures surface;
8. how the package proves itself in isolation;
9. what evidence is still unperformed;
10. which package specification is next.

The specification is therefore **Approved**, while implementation remains locked until SUITE-DOC-33.


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
