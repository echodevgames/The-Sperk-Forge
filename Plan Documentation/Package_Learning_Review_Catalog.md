---
tags:
  - sfgss/learning
  - sfgss/packages
status: active
updated: 2026-08-16
---

# The Sperk’s Forge – Package Learning Review Catalog

**Purpose:** Provide a plain-language bridge between the architectural specifications and later implementation.
**Status:** Active just-in-time Learn → Declare → Authorize program; PKG-LEARN-008 is complete and EUI-M4-01 is active with automated proof green and Laboratory/manual proof pending.
**Gate:** Reviews occur just in time before a package's first implementation and receive bounded revisits when a later checkpoint introduces materially new architecture. One completed review authorizes only its separately declared checkpoint.
**Tracker:** `Learning Reviews/PKG-LEARN-TRACKER.json`
**Template:** [[Learning Reviews/PKG-LEARN-TEMPLATE|Package Learning Review Template]]

## Review format for every package

Each package review covers:

1. Plain-English purpose.
2. A real-world analogy.
3. One practical game application.
4. What the package owns and explicitly refuses to own.
5. Definition/configuration versus mutable runtime state.
6. Lifecycle and failure behavior.
7. Important public concepts without requiring API memorization.
8. Optional bridges and which authority remains in control.
9. The package’s Standalone Laboratory.
10. A teach-back check in which Jesse explains the package in his own words.

The review is educational, not an implementation checkpoint. Code may appear only as tiny illustrative pseudocode until a later approved build checkpoint requires complete compile-ready files.

## Permanent ID rule

Learning reviews use one permanent ID sequence, `PKG-LEARN-001` through `PKG-LEARN-028`, but occur just in time before each package's first implementation checkpoint.

Earlier planning aliases such as `PKG-LEARN-F01`, `PKG-LEARN-E01`, and `PKG-LEARN-A01` are retired before execution. They were never completed review records and must not be used for new artifacts.

## Package review catalog

### Foundation Wave

| Review ID | Package | Status | Plain purpose | Practical application example |
|---|---|---|---|---|
| PKG-LEARN-001 | [[Package Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification|First Light (`EchoLaunch`)]] | Complete | Coordinates the one reliable beginning of the application, ordered startup, launch status, diagnostics, and final handoff. | A Rescuers2D build opens through one Boot scene, validates configuration, shows the startup sequence, then hands off to the Main Menu. |
| PKG-LEARN-002 | [[Package Specifications/SFGSS-The-Observatory-EchoDiagnostics-Package-Specification|The Observatory (`EchoDiagnostics`)]] | Paused until implementation | Collects and presents validation, runtime health, performance, and package diagnostics without owning the systems being observed. | A development overlay reveals a duplicate persistent root, the active scene, frame time, and why First Light failed a startup step. |
| PKG-LEARN-003 | [[Package Specifications/SFGSS-The-Accord-EchoSettings-Package-Specification|The Accord (`EchoSettings`)]] | Not started | Owns global preferences, defaults, edit/apply/cancel transactions, validation, persistence, migration, and safe display rollback. | A player changes master volume, resolution, subtitles, and reduced motion; Cancel restores the previous effective settings safely. |
| PKG-LEARN-004 | [[Package Specifications/SFGSS-The-Passage-EchoSceneFlow-Package-Specification|The Passage (`EchoSceneFlow`)]] | Not started | Owns normal runtime scene-transition requests, loading phases, progress, locking, recovery, and destination validation. | The player leaves the Main Menu, sees a fade and loading status, enters Level 1, and cannot trigger a second conflicting load. |
| PKG-LEARN-005 | [[Package Specifications/SFGSS-The-Pulse-EchoGameState-Package-Specification|The Pulse (`EchoGameState`)]] | Not started | Owns high-level runtime state, temporary override scopes, nested pause reasons, time policy, and cursor coordination intent. | Opening a pause menu adds a pause scope; a confirmation modal adds another; closing them out of order does not unpause gameplay early. |
| PKG-LEARN-006 | [[Package Specifications/SFGSS-Resonance-Jukebot-Package-Specification|Resonance (`Jukebot`)]] | Not started | Owns music, SFX, ambience, pooled voices, playback handles, concurrency, crossfades, and mixer routing. | Exploration music crossfades into danger music while a shield bash and ambience continue independently without duplicated tracks. |
| PKG-LEARN-007 | [[Package Specifications/SFGSS-The-Will-EchoInput-Package-Specification|The Will (`EchoInput`)]] | Not started | Owns input contexts, reason-based locks, device detection, rebinding transactions, override data, and glyph resolution. | Opening inventory disables gameplay actions, enables UI navigation, and changes prompts from keyboard keys to controller glyphs. |
| PKG-LEARN-008 | [[Package Specifications/SFGSS-The-Looking-Glass-EchoUI-Package-Specification|The Looking Glass (`EchoUI`)]] | **Complete** | Owns stable UI surfaces, scoped navigation, independent windows/HUD/overlays, focus/visibility presentation, Motifs, primitives, and authoring tooling without owning game/input/domain truth. | Main Menu → Settings → Back coexists with an independently toggled window; stable IDs later enable a project-authored Menu for Menus. |
| PKG-LEARN-009 | [[Package Specifications/SFGSS-The-Chronicle-EchoSave-Package-Specification|The Chronicle (`EchoSave`)]] | **Complete** | Owns save slots, immutable generations, manifests, participant payload transport, migration, integrity, backup, and corruption recovery without becoming participant runtime truth or project-wide service composition. | An autosave is interrupted; the next launch rejects the incomplete generation and restores the most recent verified save. |
| PKG-LEARN-010 | [[Package Specifications/SFGSS-The-Workshop-EchoGameStarter-Package-Specification|The Workshop (`EchoGameStarter`)]] | Not started | Owns Editor-time package selection, dry-run planning, installation coordination, project generation, repair, receipts, and removal guidance. | A new game-jam project selects Launch, UI, Input, Audio, and no game saves, previews every file, then generates a visible starter foundation. |

### Expansion Wave

| Review ID | Package | Status | Plain purpose | Practical application example |
|---|---|---|---|---|
| PKG-LEARN-011 | [[Package Specifications/SFGSS-Impact-EchoFeedback-Package-Specification|Impact (`EchoFeedback`)]] | Not started | Coordinates semantic feedback recipes across camera, timing, haptics, flashes, UI, and audio providers without owning those providers. | A C4 explosion requests one recipe that triggers camera shake, hit stop, rumble, flash, UI punch, and a Jukebot cue. |
| PKG-LEARN-012 | [[Package Specifications/SFGSS-The-Wellspring-EchoPool-Package-Specification|The Wellspring (`EchoPool`)]] | Not started | Owns reusable object pools, prewarming, generational leases, return safety, capacity, exhaustion policy, scopes, and diagnostics. | Bullets, rescue markers, and temporary VFX are reused instead of repeatedly instantiated and destroyed during a busy level. |
| PKG-LEARN-013 | [[Package Specifications/SFGSS-The-Ascent-EchoProgression-Package-Specification|The Ascent (`EchoProgression`)]] | Not started | Owns unlocks, access rules, checkpoints, progression-node completion, local rank snapshots, password grants, and progression state. | Rescuers2D validates a level password, unlocks Level 4, records the checkpoint, and exposes access state to the level-select screen. |
| PKG-LEARN-014 | [[Package Specifications/SFGSS-The-Foundry-EchoBuildTools-Package-Specification|The Foundry (`EchoBuildTools`)]] | Not started | Owns repeatable build recipes, preflight validation, version stamping, safe output handling, receipts, manifests, and release preparation. | A WebGL itch.io build validates scenes and licenses, stamps the version, cleans only its owned folder, builds, and writes checksums. |
| PKG-LEARN-015 | [[Package Specifications/SFGSS-Many-Tongues-EchoLocalization-Package-Specification|Many Tongues (`EchoLocalization`)]] | Not started | Owns locale authority, localized-reference resolution, fallback, regional formatting, font/script policy, pseudolocalization, and diagnostics. | Changing to Spanish refreshes UI and dialogue references, applies regional number formats, and reports a missing glyph before release. |
| PKG-LEARN-016 | [[Package Specifications/SFGSS-Voices-EchoDialogue-Package-Specification|Voices (`EchoDialogue`)]] | Not started | Owns speaker and conversation definitions, deterministic node traversal, lines, choices, conditions, commands, history, and session snapshots. | A Hackulos spell vendor greets the player, checks a quest condition, offers two choices, and dispatches a typed command to grant spells. |
| PKG-LEARN-017 | [[Package Specifications/SFGSS-The-Path-EchoObjectives-Package-Specification|The Path (`EchoObjectives`)]] | Not started | Owns objectives, runs, graph progress, counters, flags, timers, repeatability, tracking, completion, and idempotent reward delivery ledgers. | The rat-tail quest tracks a kill, item collection, return, exact combine, and bag placement without storing inventory or dialogue truth. |
| PKG-LEARN-018 | [[Package Specifications/SFGSS-The-Vault-EchoInventory-Package-Specification|The Vault (`EchoInventory`)]] | Not started | Owns item/container definitions, fungible stacks, unique item instances, capacity, filters, queries, atomic transfers, and generic equipment storage. | A bag holds stacked rat tails and one uniquely damaged sword; moving or equipping them succeeds completely or makes no change. |
| PKG-LEARN-019 | [[Package Specifications/SFGSS-The-Hand-EchoInteraction-Package-Specification|The Hand (`EchoInteraction`)]] | Not started | Owns interaction offers, detection, availability, focus, prompts, session lifetimes, holds, cancellation, and execution routing. | The same ladder offers Pick Up or Climb depending on context; the package selects the offer while project code performs the outcome. |
| PKG-LEARN-020 | [[Package Specifications/SFGSS-The-Eye-EchoCamera-Package-Specification|The Eye (`EchoCamera`)]] | Not started | Owns camera channels, target registration, modes, priorities, blends, bounds, zones, impulses, and backend negotiation. | Switching from Firefighter to Specialist changes the camera target while a rescue zone adds bounds and Impact requests a shake. |
| PKG-LEARN-021 | [[Package Specifications/SFGSS-The-Fellowship-EchoCharacters-Package-Specification|The Fellowship (`EchoCharacters`)]] | Not started | Owns character definitions, durable roster members, selection contexts, groups, spawning, runtime actors, respawn, and exclusive control ownership. | Rescuers2D switches Firefighter, Riot Officer, and Specialist while preserving one roster and handing control to the selected actor. |
| PKG-LEARN-022 | [[Package Specifications/SFGSS-The-Vessel-EchoControllers-Package-Specification|The Vessel (`EchoControllers`)]] | Not started | Owns actor-bound controller hosts, normalized intent, local motors, locomotion state, focused capabilities, physics policy, and semantic movement events. | A side-view specialist walks and jumps using the platformer preset while Hackulos uses an independent top-down 2D preset. |
| PKG-LEARN-023 | [[Package Specifications/SFGSS-The-Crucible-EchoCrafting-Package-Specification|The Crucible (`EchoCrafting`)]] | Not started | Owns recipe definitions, exact combines, standard matching, previews, requirements, idempotency, and one-provider atomic crafting transactions. | Hackulos combines a rat tail and human eye inside a quest bag without importing a full profession or production-queue system. |

### Advanced Wave

| Review ID | Package | Status | Plain purpose | Practical application example |
|---|---|---|---|---|
| PKG-LEARN-024 | [[Package Specifications/SFGSS-The-Convergence-EchoMultiplayer-Package-Foundation|The Convergence (`EchoMultiplayer`)]] | Not started | Defines provider-neutral sessions, participants, readiness, roles, authority, travel, spawn/ownership, security, adapters, and prototype gates. | A future co-op prototype hosts a lobby, synchronizes a scene change, assigns characters, validates an interaction, and handles disconnects. |
| PKG-LEARN-025 | [[Package Specifications/SFGSS-Instinct-EchoAI-Package-Foundation|Instinct (`EchoAI`)]] | Not started | Defines reusable stimuli, perception memory, target scoring, typed context, scheduling, behavior seams, navigation contracts, and AI diagnostics. | A passive rat notices nearby food while an aggressive humanoid scores threats, chases, and later flees at low health using project-authored rules. |
| PKG-LEARN-026 | [[Package Specifications/SFGSS-Clash-EchoCombat-Package-Foundation|Clash (`EchoCombat`)]] | Not started | Defines provider-neutral combat requests, targetability, relations, deterministic modifiers, target-owned transactions, outcomes, and defeat events. | A shield bash submits damage and stagger requests; the target’s health/posture authority commits the actual resource changes. |
| PKG-LEARN-027 | [[Package Specifications/SFGSS-Arcana-EchoAbilities-Package-Foundation|Arcana (`EchoAbilities`)]] | Not started | Defines ability grants, loadouts, activation, costs, charges, cooldowns, casting, interruption, targeting, effects, persistence, and authority seams. | A Necromancer begins Life Drain, commits mana at the chosen point, channels repeated effects, and can be interrupted under authored rules. |
| PKG-LEARN-028 | [[Package Specifications/SFGSS-The-Atlas-EchoWorld-Package-Foundation|The Atlas (`EchoWorld`)]] | Not started | Defines durable worlds, zones, locations, travel graphs, scene bindings, entry/spawn markers, discovery, fast travel, map snapshots, and world-state routing. | Devroth contains semantic towns and wilderness locations that survive scene renames and route travel through The Passage. |

## Just-in-time learning order

The numeric IDs remain permanent navigation identities. They no longer force one uninterrupted study sequence.

The normal rule is:

1. Choose the next approved implementation checkpoint.
2. Complete or refresh the related package review.
3. Record the teach-back and remaining questions.
4. Authorize only that package's checkpoint.
5. Repeat when the implementation roadmap reaches another package.

PKG-LEARN-001 is complete for First Light. PKG-LEARN-002 is paused until EchoDiagnostics implementation approaches.

## Completion tracking

| Wave | Complete | Total |
|---|---:|---:|
| Foundation | 3 | 10 |
| Expansion | 0 | 13 |
| Advanced | 0 | 5 |
| **Total** | **3** | **28** |

## Current learning state

- Complete: **PKG-LEARN-001 – First Light (`EchoLaunch`)**, **PKG-LEARN-008 – The Looking Glass (`EchoUI`)**, and **PKG-LEARN-009 – The Chronicle (`EchoSave`)**
- Paused: **PKG-LEARN-002 – The Observatory (`EchoDiagnostics`)**
- Active learning review: **None; PKG-LEARN-008 is complete**
- Current initiative: **Looking Glass foundation** — the UI package was intentionally pulled forward after Chronicle M5 so the project can begin assembling its reusable front door incrementally
- Looking Glass implementation checkpoint: **EUI-M4-01 active / authorized under Green Path**; activation `ce30ac6`, Runtime/tests `df9e2be`, corrections through `e47d43b`, automated gate green, Laboratory/manual proof pending
- Next package review trigger: selected deliberately after EUI-M4-01; no unrelated package is unlocked by this review

## Navigation

- [[Full_Suite_Documentation_and_Learning_Handoff_Guide|Full Suite Handoff Guide]]
- [[Learning Reviews/README|Learning Reviews Index]]
- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Suite_Health_Check_and_Remaining_Documentation|Suite Health Check]]
- [[SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules|SFGSS-005 Learning Workflow]]
