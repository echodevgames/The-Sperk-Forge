---
tags:
  - sfgss/learning
  - sfgss/packages
status: planned
updated: 2026-08-04
---

# The Sperk’s Forge — Package Learning Review Catalog

**Purpose:** Provide a plain-language bridge between the architectural specifications and later implementation.  
**Status:** Planned; individual reviews have not yet been performed.  
**Gate:** All 28 reviews must be completed before SUITE-DOC-33 can authorize code.

## Review format for every package

Each package review will cover:

1. **Plain-English purpose:** what problem it solves.
2. **Real-world analogy:** a non-code comparison that makes the authority intuitive.
3. **Practical game example:** one concrete scenario from Rescuers2D, Hackulos, Don’t Get Vince’d, Echo Systems Lab, or a generic game.
4. **Owns / does not own:** the critical boundary in ordinary language.
5. **Definition versus runtime state:** what is authored and what changes during play.
6. **Lifecycle:** how it starts, operates, fails, resets, and shuts down.
7. **Important public concepts:** the small set of types and operations worth remembering.
8. **Bridges:** what other packages may connect and which side remains authoritative.
9. **Standalone Laboratory:** how the package proves itself alone.
10. **Teach-back check:** Jesse explains the package back in his own words and answers a few practical questions.

The review is educational, not an implementation checkpoint. Code may be shown only as tiny illustrative pseudocode unless a later authorized build checkpoint requires complete compile-ready files.

## Brief package review


### Foundation Wave

| Review ID | Package | Plain purpose | Practical application example |
|---|---|---|---|
| PKG-LEARN-F01 | [[Package Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification|First Light (`EchoLaunch`)]] | Coordinates the one reliable beginning of the application, ordered startup, launch status, diagnostics, and final handoff. | A Rescuers2D build opens through one Boot scene, validates configuration, shows the startup sequence, then hands off to the Main Menu. |
| PKG-LEARN-F02 | [[Package Specifications/SFGSS-The-Observatory-EchoDiagnostics-Package-Specification|The Observatory (`EchoDiagnostics`)]] | Collects and presents validation, runtime health, performance, and package diagnostics without owning the systems being observed. | A development overlay reveals a duplicate persistent root, the active scene, frame time, and why First Light failed a startup step. |
| PKG-LEARN-F03 | [[Package Specifications/SFGSS-The-Accord-EchoSettings-Package-Specification|The Accord (`EchoSettings`)]] | Owns global preferences, defaults, edit/apply/cancel transactions, validation, persistence, migration, and safe display rollback. | A player changes master volume, resolution, subtitles, and reduced motion; Cancel restores the previous effective settings safely. |
| PKG-LEARN-F04 | [[Package Specifications/SFGSS-The-Passage-EchoSceneFlow-Package-Specification|The Passage (`EchoSceneFlow`)]] | Owns normal runtime scene-transition requests, loading phases, progress, locking, recovery, and destination validation. | The player leaves the Main Menu, sees a fade and loading status, enters Level 1, and cannot trigger a second conflicting load. |
| PKG-LEARN-F05 | [[Package Specifications/SFGSS-The-Pulse-EchoGameState-Package-Specification|The Pulse (`EchoGameState`)]] | Owns high-level runtime state, temporary override scopes, nested pause reasons, time policy, and cursor coordination intent. | Opening a pause menu adds a pause scope; a confirmation modal adds another; closing them out of order does not unpause gameplay early. |
| PKG-LEARN-F06 | [[Package Specifications/SFGSS-Resonance-Jukebot-Package-Specification|Resonance (`Jukebot`)]] | Owns music, SFX, ambience, pooled voices, playback handles, concurrency, crossfades, and mixer routing. | Exploration music crossfades into danger music while a shield bash and ambience continue independently without duplicated tracks. |
| PKG-LEARN-F07 | [[Package Specifications/SFGSS-The-Will-EchoInput-Package-Specification|The Will (`EchoInput`)]] | Owns input contexts, reason-based locks, device detection, rebinding transactions, override data, and glyph resolution. | Opening inventory disables gameplay actions, enables UI navigation, and changes prompts from keyboard keys to controller glyphs. |
| PKG-LEARN-F08 | [[Package Specifications/SFGSS-The-Looking-Glass-EchoUI-Package-Specification|The Looking Glass (`EchoUI`)]] | Owns reusable screen, HUD, modal, notification, prompt, focus, navigation, theme, and UI lifecycle infrastructure. | A Main Menu opens Settings, a confirmation modal traps focus correctly, and returning restores the previously selected button. |
| PKG-LEARN-F09 | [[Package Specifications/SFGSS-The-Chronicle-EchoSave-Package-Specification|The Chronicle (`EchoSave`)]] | Owns save slots, immutable generations, manifests, participant payloads, migration, integrity, backup, and corruption recovery. | An autosave is interrupted; the next launch rejects the incomplete generation and restores the most recent verified save. |
| PKG-LEARN-F10 | [[Package Specifications/SFGSS-The-Workshop-EchoGameStarter-Package-Specification|The Workshop (`EchoGameStarter`)]] | Owns Editor-time package selection, dry-run planning, installation coordination, project generation, repair, receipts, and removal guidance. | A new game-jam project selects Launch, UI, Input, Audio, and no game saves, previews every file, then generates a visible starter foundation. |

### Expansion Wave

| Review ID | Package | Plain purpose | Practical application example |
|---|---|---|---|
| PKG-LEARN-E01 | [[Package Specifications/SFGSS-Impact-EchoFeedback-Package-Specification|Impact (`EchoFeedback`)]] | Coordinates semantic feedback recipes across camera, timing, haptics, flashes, UI, and audio providers without owning those providers. | A C4 explosion requests one recipe that triggers camera shake, hit stop, rumble, flash, UI punch, and a Jukebot cue. |
| PKG-LEARN-E02 | [[Package Specifications/SFGSS-The-Wellspring-EchoPool-Package-Specification|The Wellspring (`EchoPool`)]] | Owns reusable object pools, prewarming, generational leases, return safety, capacity, exhaustion policy, scopes, and diagnostics. | Bullets, rescue markers, and temporary VFX are reused instead of repeatedly instantiated and destroyed during a busy level. |
| PKG-LEARN-E03 | [[Package Specifications/SFGSS-The-Ascent-EchoProgression-Package-Specification|The Ascent (`EchoProgression`)]] | Owns unlocks, access rules, checkpoints, progression-node completion, local rank snapshots, password grants, and progression state. | Rescuers2D validates a level password, unlocks Level 4, records the checkpoint, and exposes access state to the level-select screen. |
| PKG-LEARN-E04 | [[Package Specifications/SFGSS-The-Foundry-EchoBuildTools-Package-Specification|The Foundry (`EchoBuildTools`)]] | Owns repeatable build recipes, preflight validation, version stamping, safe output handling, receipts, manifests, and release preparation. | A WebGL itch.io build validates scenes and licenses, stamps the version, cleans only its owned folder, builds, and writes checksums. |
| PKG-LEARN-E05 | [[Package Specifications/SFGSS-Many-Tongues-EchoLocalization-Package-Specification|Many Tongues (`EchoLocalization`)]] | Owns locale authority, localized-reference resolution, fallback, regional formatting, font/script policy, pseudolocalization, and diagnostics. | Changing to Spanish refreshes UI and dialogue references, applies regional number formats, and reports a missing glyph before release. |
| PKG-LEARN-E06 | [[Package Specifications/SFGSS-Voices-EchoDialogue-Package-Specification|Voices (`EchoDialogue`)]] | Owns speaker and conversation definitions, deterministic node traversal, lines, choices, conditions, commands, history, and session snapshots. | A Hackulos spell vendor greets the player, checks a quest condition, offers two choices, and dispatches a typed command to grant spells. |
| PKG-LEARN-E07 | [[Package Specifications/SFGSS-The-Path-EchoObjectives-Package-Specification|The Path (`EchoObjectives`)]] | Owns objectives, runs, graph progress, counters, flags, timers, repeatability, tracking, completion, and idempotent reward delivery ledgers. | The rat-tail quest tracks a kill, item collection, return, exact combine, and bag placement without storing inventory or dialogue truth. |
| PKG-LEARN-E08 | [[Package Specifications/SFGSS-The-Vault-EchoInventory-Package-Specification|The Vault (`EchoInventory`)]] | Owns item/container definitions, fungible stacks, unique item instances, capacity, filters, queries, atomic transfers, and generic equipment storage. | A bag holds stacked rat tails and one uniquely damaged sword; moving or equipping them succeeds completely or makes no change. |
| PKG-LEARN-E09 | [[Package Specifications/SFGSS-The-Hand-EchoInteraction-Package-Specification|The Hand (`EchoInteraction`)]] | Owns interaction offers, detection, availability, focus, prompts, session lifetimes, holds, cancellation, and execution routing. | The same ladder offers Pick Up or Climb depending on context; the package selects the offer while project code performs the outcome. |
| PKG-LEARN-E10 | [[Package Specifications/SFGSS-The-Eye-EchoCamera-Package-Specification|The Eye (`EchoCamera`)]] | Owns camera channels, target registration, modes, priorities, blends, bounds, zones, impulses, and backend negotiation. | Switching from Firefighter to Specialist changes the camera target while a rescue zone adds bounds and Impact requests a shake. |
| PKG-LEARN-E11 | [[Package Specifications/SFGSS-The-Fellowship-EchoCharacters-Package-Specification|The Fellowship (`EchoCharacters`)]] | Owns character definitions, durable roster members, selection contexts, groups, spawning, runtime actors, respawn, and exclusive control ownership. | Rescuers2D switches Firefighter, Riot Officer, and Specialist while preserving one roster and handing control to the selected actor. |
| PKG-LEARN-E12 | [[Package Specifications/SFGSS-The-Vessel-EchoControllers-Package-Specification|The Vessel (`EchoControllers`)]] | Owns actor-bound controller hosts, normalized intent, local motors, locomotion state, focused capabilities, physics policy, and semantic movement events. | A side-view specialist walks and jumps using the platformer preset while Hackulos uses an independent top-down 2D preset. |
| PKG-LEARN-E13 | [[Package Specifications/SFGSS-The-Crucible-EchoCrafting-Package-Specification|The Crucible (`EchoCrafting`)]] | Owns recipe definitions, exact combines, standard matching, previews, requirements, idempotency, and one-provider atomic crafting transactions. | Hackulos combines a rat tail and human eye inside a quest bag without importing a full profession or production-queue system. |

### Advanced Wave

| Review ID | Package | Plain purpose | Practical application example |
|---|---|---|---|
| PKG-LEARN-A01 | [[Package Specifications/SFGSS-The-Convergence-EchoMultiplayer-Package-Foundation|The Convergence (`EchoMultiplayer`)]] | Defines provider-neutral sessions, participants, readiness, roles, authority, travel, spawn/ownership, security, adapters, and prototype gates. | A future co-op prototype hosts a lobby, synchronizes a scene change, assigns characters, validates an interaction, and handles disconnects. |
| PKG-LEARN-A02 | [[Package Specifications/SFGSS-Instinct-EchoAI-Package-Foundation|Instinct (`EchoAI`)]] | Defines reusable stimuli, perception memory, target scoring, typed context, scheduling, behavior seams, navigation contracts, and AI diagnostics. | A passive rat notices nearby food while an aggressive humanoid scores threats, chases, and later flees at low health using project-authored rules. |
| PKG-LEARN-A03 | [[Package Specifications/SFGSS-Clash-EchoCombat-Package-Foundation|Clash (`EchoCombat`)]] | Defines provider-neutral combat requests, targetability, relations, deterministic modifiers, target-owned transactions, outcomes, and defeat events. | A shield bash submits damage and stagger requests; the target’s health/posture authority commits the actual resource changes. |
| PKG-LEARN-A04 | [[Package Specifications/SFGSS-Arcana-EchoAbilities-Package-Foundation|Arcana (`EchoAbilities`)]] | Defines ability grants, loadouts, activation, costs, charges, cooldowns, casting, interruption, targeting, effects, persistence, and authority seams. | A Necromancer begins Life Drain, commits mana at the chosen point, channels repeated effects, and can be interrupted under authored rules. |
| PKG-LEARN-A05 | [[Package Specifications/SFGSS-The-Atlas-EchoWorld-Package-Foundation|The Atlas (`EchoWorld`)]] | Defines durable worlds, zones, locations, travel graphs, scene bindings, entry/spawn markers, discovery, fast travel, map snapshots, and world-state routing. | Devroth contains semantic towns and wilderness locations that survive scene renames and route travel through The Passage. |


## Suggested learning order

The default order follows architectural dependency and learning value:

1. First Light, Observatory, Accord, Passage, Pulse.
2. Resonance, Will, Looking Glass, Chronicle, Workshop.
3. Impact, Wellspring, Ascent, Foundry, Many Tongues.
4. Voices, Path, Vault, Hand, Eye.
5. Fellowship, Vessel, Crucible.
6. Convergence, Instinct, Clash, Arcana, Atlas.

This sequence moves from application shell to user-facing systems, then reusable gameplay, then advanced authority and provider boundaries.

## Completion tracking

| Wave | Complete | Total |
|---|---:|---:|
| Foundation | 0 | 10 |
| Expansion | 0 | 13 |
| Advanced | 0 | 5 |
| **Total** | **0** | **28** |

## Navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Suite_Health_Check_and_Remaining_Documentation|Suite Health Check]]
- [[SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules|SFGSS-005 Learning Workflow]]
