# The Sperk's Forge - New-Project Guided Pathways Standard

**Document ID:** SFGSS-006  
**Version:** 1.1.0
**Status:** Approved guided-composition and project-start standard  
**Owner:** Jesse "Echo" Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000 v0.24.0
**Related authorities:** SFGSS-001, SFGSS-002, SFGSS-003, SFGSS-004, SFGSS-005, SFGSS-ADR-001, SFGSS-ADR-002, SFGSS-ADR-003, SFGSS-ADR-005, SFGSS-INT-FOUNDATION-001, SFGSS-INT-EXPANSION-001, SFGSS-INT-ADVANCED-001
**Primary composer:** The Workshop (`EchoGameStarter`)  
**Current development baseline:** Unity 6000.3.8f1  
**Minimum planned public Unity floor:** Unity 6000.0  
**Last updated:** August 7, 2026

> A pathway is a lantern and a map, not a wagon that secretly carries the whole Forge.

---

## Contents

1. Purpose and authority
2. Scope and non-goals
3. Terminology
4. Governing principles
5. Pathway maturity and evidence language
6. Guided pathway record
7. Entry questionnaire
8. Authority-first package selection map
9. Selection tiers and staged adoption
10. Application-shell profiles
11. Persistence pathways
12. Scene, world, and travel pathways
13. Input, character, and controller pathways
14. Presentation, audio, accessibility, and localization pathways
15. Gameplay-system selection rules
16. Advanced and provider-backed selection rules
17. PATH-000 - Blank Modular Starter
18. PATH-001 - Package Laboratory and Portfolio System
19. PATH-010 - Minimal Audiovisual Prototype
20. PATH-020 - Game Jam Quickstart
21. PATH-030 - Puzzle and Tabletop Game
22. PATH-040 - Password-Based Puzzle Platformer
23. PATH-050 - Save-Based Adventure
24. PATH-060 - Narrative Game
25. PATH-070 - Action Combat Prototype
26. PATH-080 - RPG Foundation
27. PATH-090 - Local Multiplayer Prototype
28. PATH-100 - Online Multiplayer Research Prototype
29. PATH-110 - Existing-Project Incremental Adoption
30. Workshop mapping and dry-run requirements
31. Bridge, provider, and adapter selection
32. Laboratory, validation, and readiness requirements
33. Removal, downgrade, and pathway changes
34. Graph, documentation, and handoff rules
35. Suite application matrix
36. Approval

---

## 1. Purpose and authority

SFGSS-006 defines how a developer selects and stages packages from **The Sperk's Forge - EchoDevGames Game Systems Suite** when beginning a new Unity project or adopting the suite in an existing one.

The suite contains twenty-eight approved package specifications or provider-neutral foundations. That catalog is intentionally modular, but a large modular catalog can still overwhelm a developer who does not yet know which authorities a project needs. This standard converts the approved package catalog into transparent pathways without turning recommendations into mandatory bundles.

This document answers:

- Which questions should be answered before selecting packages?
- Which package owns each commonly requested project capability?
- What is the smallest useful package set for a given project type?
- Which additions are recommended, optional, deferred, or explicitly excluded?
- Which bridges or providers are required only when two selected authorities must collaborate?
- Which project choices remain owned by the game rather than by the suite?
- How should The Workshop present, plan, generate, validate, and remove a pathway?
- How can a developer stage a large pathway such as an RPG without installing every future system on day one?

### 1.1 Authority order

When pathway documents disagree with package architecture, use this order:

1. SFGSS-000 suite authority and ownership boundaries.
2. The active approved package specification or Advanced foundation.
3. SFGSS-002 for dependencies, bridges, assemblies, providers, samples, and removal.
4. SFGSS-003 for data, stable IDs, serialization, migration, transactions, and unknown-data preservation.
5. SFGSS-004 for Laboratories, testing, evidence, compatibility, and release claims.
6. This standard.
7. Accepted ADRs and integration specifications.
8. SFGSS-005 checkpoint workflow.
9. Workshop preset data, project-specific pathway records, setup reports, and Current Notes.

A pathway may narrow a project choice. It must not transfer authority, create a hidden dependency, weaken a package's independence contract, or claim empirical support that has not been tested.

### 1.2 Requirement language

- **Must** means required for a conforming guided pathway or supported Workshop preset.
- **Must not** means prohibited unless a higher authority or accepted ADR grants a named exception.
- **Should** means the default guidance; a project may differ with a recorded reason.
- **May** means optional.

---

## 2. Scope and non-goals

### 2.1 In scope

This standard governs:

- New-project package selection.
- Existing-project adoption sequencing.
- Pathway and preset records.
- Minimum, recommended, optional, deferred, and excluded selections.
- Package, bridge, provider, adapter, scene, persistence, and Laboratory visibility.
- Workshop dry-run requirements for pathways.
- Staged vertical slices and stop points.
- Removal and pathway-change guidance.
- Documentation and Graph View navigation for compositions.

### 2.2 Not in scope

This standard does not:

- Make any Echo package mandatory for every project.
- Approve a networking, navigation, behavior, inference, cloud, platform, or camera provider without evidence.
- Replace an individual package specification.
- Define a game's story, genre rules, content, balance, art, level design, or production schedule.
- Guarantee that every listed pathway is already implemented or tested as a Workshop preset.
- Authorize package code while the Full Suite Documentation and Learning Gate remains locked.
- Require a project to use The Workshop. Manual composition remains supported.
- Treat a pathway as one monolithic runtime framework.

---

## 3. Terminology

| Term | Meaning |
|---|---|
| **Pathway** | An approved, visible composition guide for reaching one project outcome in stages. |
| **Preset** | A versioned Workshop definition that implements a pathway or project-specific variant. |
| **Composition** | The actual packages, bridges, providers, adapters, project assets, scenes, and options selected for one project. |
| **Minimum selection** | The smallest set that can prove the pathway's named vertical slice. |
| **Recommended selection** | The default additions that make the pathway safer, clearer, or more complete without becoming universal requirements. |
| **Optional branch** | A visible capability choice selected only when the project needs it. |
| **Explicit exclusion** | A package or capability intentionally omitted from the current stage so scope remains understandable. |
| **Stage** | A bounded composition milestone with one playable or testable outcome. |
| **Application shell** | The selected startup, diagnostics, settings, scene, state, audio, input, UI, and persistence authorities surrounding gameplay. |
| **Authority owner** | The package or project system that owns one concern's truth. |
| **Bridge** | A separate, removable integration artifact connecting two selected authorities. |
| **Provider adapter** | A separately versioned artifact connecting a provider-neutral package to a technical or vendor backend. |
| **Project adapter** | Project-owned translation code for game-specific behavior. |
| **Pathway manifest** | The project-owned record of selected pathway version, packages, bridges, options, stages, and deviations. |
| **Manual composition** | Selecting and configuring packages without The Workshop. |
| **Workshop composition** | Using The Workshop to produce a visible dry-run plan and invoke package-owned Editor setup facades. |
| **Research pathway** | A composition intended to compare or prototype providers without making a production support claim. |

---

## 4. Governing principles

### 4.1 A pathway is guidance, not a hidden bundle

Every package, bridge, provider, sample, scene, and generated asset must remain visible in the plan. A user must be able to understand why it was selected.

### 4.2 Start from the required authority

Selection begins with the project's needed truths, not with package popularity. If the project needs save slots, select The Chronicle. If it does not, do not install it merely because another pathway commonly does.

### 4.3 Minimum first, then stages

A pathway begins with the smallest complete vertical slice. Later systems are added only after the current stage passes its own Laboratory and project acceptance checks.

### 4.4 Recommended is not mandatory

A recommended package may improve the default experience. It must remain removable unless another selected artifact declares a real dependency.

### 4.5 Bridges appear only when behavior crosses authorities

Installing both peer packages does not automatically require every possible bridge. Select the bridge only when the project needs the named collaboration.

### 4.6 Project-owned content remains outside package source

Pathways may create project configuration, scenes, adapters, profiles, and templates. Concrete game content remains project-owned.

### 4.7 Advanced foundations remain evidence-gated

A provider-neutral foundation may appear in a research or staged pathway. It must not be presented as production-ready until its provider, compatibility, performance, security, and migration evidence exists.

### 4.8 Existing projects adopt incrementally

A working project keeps its old system until the replacement package proves standalone behavior and project parity.

### 4.9 Removal is part of selection

Every pathway record includes how optional packages, bridges, providers, samples, and generated assets can be removed or replaced.

### 4.10 Learning is a pathway output

A successful pathway leaves the developer able to name each selected authority, explain why it exists, and identify the project's own remaining responsibilities.

### 4.11 Package pre-release and project adoption are separate stages

A package may complete its clean-project standalone pre-release before any
existing project selects PATH-110. PATH-110 begins only when a working project
deliberately chooses an adoption target. It is not a universal prerequisite for
advancing to the next package.

---

## 5. Pathway maturity and evidence language

Pathways and presets use these states:

| State | Meaning |
|---|---|
| **Proposed guidance** | Composition idea under review. |
| **Approved guidance** | Architecture and staged selection agree with current authorities. |
| **Workshop candidate** | Approved guidance mapped to a planned Workshop preset, but not implemented. |
| **Implemented preset** | Workshop preset exists and produces a plan/output; evidence may still be incomplete. |
| **Tested preset** | Required clean-project and pathway vertical-slice evidence has passed in a named environment. |
| **Supported preset** | Release-stage compatibility, upgrade, removal, documentation, and known-limitations evidence satisfies SFGSS-004. |
| **Experimental preset** | Available with explicit limitations and no compatibility guarantee. |
| **Deprecated preset** | Still readable for migration/removal but scheduled for replacement. |

This document approves **guidance**. It does not claim any Workshop preset is implemented, tested, or supported.

---

## 6. Guided pathway record

Every durable pathway or Workshop preset must declare:

| Field | Requirement |
|---|---|
| Pathway ID and version | Stable, package-independent identity such as `PATH-020` v1.0.0. |
| Public title and plain purpose | Immediately understandable without Verse knowledge. |
| Intended user/project | New project, existing project, package author, game jam, research, or another named audience. |
| Maturity state | Approved guidance, Workshop candidate, tested preset, and so on. |
| Starting conditions | Unity version target, repository state, existing systems, and required decisions. |
| First vertical slice | One playable or testable outcome. |
| Minimum selection | Every package genuinely required for that first slice. |
| Recommended selection | Default additions with rationale. |
| Optional branches | Capability-driven additions, each visible and removable. |
| Explicit exclusions | Systems intentionally postponed or rejected for the current stage. |
| Required bridges/providers | Only integrations needed by selected behavior. |
| Project-owned assets | Scenes, profiles, adapters, content, schemas, and presentation the game must supply. |
| Persistence choice | None, Accord preference, Ascent standalone record, Chronicle save, authoritative multiplayer save, or project backend. |
| Scene/world choice | Boot, direct scene, Passage routes, Atlas locations, or project-specific flow. |
| Laboratory/evidence plan | Standalone, integration, clean-project, and vertical-slice proof. |
| Removal/rollback | How to remove packages, bridge artifacts, samples, or generated output. |
| Deviations | Project-specific changes from approved guidance and why. |

A pathway record may be represented as Markdown, a Workshop ScriptableObject, or a detached project manifest. The information contract remains the same.

---

## 7. Entry questionnaire

A new project or adoption session should answer these questions before selecting packages.

### 7.1 Project status

1. Is this a clean project, package Laboratory, game jam, portfolio prototype, existing game, or provider research project?
2. Is the goal one vertical slice, a reusable shell, or a production foundation?
3. Which Unity version and render/physics/input technologies are selected or still undecided?

### 7.2 Application lifecycle

4. Does the game need a canonical boot and startup sequence?
5. Does it travel between scenes?
6. Does it need high-level modes such as Playing, Paused, Loading, Dialogue, or Cutscene?
7. Which diagnostics must be visible during development or in Player builds?

### 7.3 Player-facing shell

8. Does the game need menus, HUDs, modals, prompts, loading presentation, or notifications?
9. Does it need music, SFX, ambience, routed mixer buses, or accessibility-aware feedback?
10. Does it need global settings, rebinding, localization, or persistent preferences?

### 7.4 Persistence

11. Does the project need no durable game state, passwords/unlocks only, one save, multiple slots, autosave, or authoritative multiplayer saves?
12. Which package owns each durable payload?
13. What data must survive removal and reinstallation of an optional package?

### 7.5 Gameplay

14. Does the project need characters, movement controllers, interactions, cameras, items, objectives, dialogue, crafting, progression, combat, abilities, AI, or world topology?
15. Which of those truths already exist in project code?
16. Which systems must collaborate, and therefore need a bridge or project adapter?

### 7.6 Players and authority

17. Is the project single-player, local multiplayer, online multiplayer research, or production multiplayer?
18. Who authoritatively commits shared-world actions?
19. Are provider choices supported by evidence, or still research candidates?

### 7.7 Delivery

20. Does the project need repeatable build recipes, version stamping, checksums, release reports, or a Workshop-generated foundation?
21. Which installation and removal routes will be claimed?
22. What is the first stop point where the project can be tested and understood?

Unanswered questions remain visible in the pathway plan. The Workshop must not invent a consequential answer silently.

---

## 8. Authority-first package selection map

Select the package that owns the truth the project needs.

| Project need | Authority/package | Do not substitute |
|---|---|---|
| Ordered application startup | First Light (`EchoLaunch`) | Scene names, menu scripts, or arbitrary managers |
| Runtime diagnostics and validation | The Observatory (`EchoDiagnostics`) | Ad hoc logs as the only status surface |
| Global preferences | The Accord (`EchoSettings`) | Save slots or UI controls |
| Unity scene transitions | The Passage (`EchoSceneFlow`) | Atlas routes or UI loading screens |
| High-level runtime modes and pause | The Pulse (`EchoGameState`) | Character state machines or pause-menu visuals |
| Music, SFX, and ambience | Resonance (`Jukebot`) | Settings persistence or gameplay event truth |
| Input contexts, devices, rebinding, glyphs | The Will (`EchoInput`) | Movement physics or menu visuals |
| Screens, HUDs, modals, prompts, focus | The Looking Glass (`EchoUI`) | Settings, saves, objectives, or gameplay rules |
| Save files, slots, generations, recovery | The Chronicle (`EchoSave`) | Progression records or world-state authority |
| Project composition and generation | The Workshop (`EchoGameStarter`) | Runtime authority or hidden package bundle |
| Coordinated feedback recipes | Impact (`EchoFeedback`) | Camera, audio, UI, input, or damage authority |
| General GameObject reuse | The Wellspring (`EchoPool`) | Spawn-wave or projectile gameplay rules |
| Unlocks, passwords, checkpoints, access | The Ascent (`EchoProgression`) | Objective-run completion or save transport |
| Repeatable build and release preparation | The Foundry (`EchoBuildTools`) | Runtime flow or automatic deployment by default |
| Locale and localized references | Many Tongues (`EchoLocalization`) | UI layout, dialogue flow, or translation authorship |
| Conversation definitions and flow | Voices (`EchoDialogue`) | Objective truth or production UI |
| Objectives, quests, tasks, progress | The Path (`EchoObjectives`) | Inventory or progression authority |
| Items, containers, stacks, equipment storage | The Vault (`EchoInventory`) | Crafting, combat stats, or vendor economics |
| Interaction discovery, focus, and request routing | The Hand (`EchoInteraction`) | Project-specific interaction outcome |
| Camera modes, targets, bounds, blends | The Eye (`EchoCamera`) | Character movement or scene loading |
| Character identity, roster, spawning, control ownership | The Fellowship (`EchoCharacters`) | Movement, combat, or input bindings |
| Reusable movement motors and controller families | The Vessel (`EchoControllers`) | Character roster, camera, or combat |
| Recipe validation and transformations | The Crucible (`EchoCrafting`) | Inventory storage or item art |
| Multiplayer sessions and authority contracts | The Convergence (`EchoMultiplayer`) | Networking SDK implementation |
| AI sensing, memory, scoring, and behavior seams | Instinct (`EchoAI`) | Enemy design, navigation backend, or combat authority |
| Instant damage/healing resolution | Clash (`EchoCombat`) | Ability timing or target-owned health model |
| Abilities, costs, cooldowns, casts, effects | Arcana (`EchoAbilities`) | Combat resolution or class content |
| World, zone, location, travel, and marker identity | The Atlas (`EchoWorld`) | Scene loading, movement, or level art |

---

## 9. Selection tiers and staged adoption

Every pathway separates four selection tiers.

### 9.1 Minimum

Required to prove the named first vertical slice. Removing one breaks that outcome.

### 9.2 Recommended

The default additions that improve diagnostics, safety, accessibility, or common workflow. Each recommendation includes a reason and remains visible.

### 9.3 Optional

Selected only for a named capability branch. Optional packages do not become transitive assumptions in project code.

### 9.4 Explicitly deferred or excluded

Systems postponed until a later stage or rejected for this project. This section protects the project from accidental scope expansion.

### 9.5 Stage rules

Each stage must:

1. Have one user-visible or testable outcome.
2. Name its package and bridge additions.
3. Preserve previously passing outcomes.
4. Run new package Standalone Laboratories before integration claims.
5. Include a stop point and rollback path.
6. Reconcile the pathway manifest after changes.

---

## 10. Application-shell profiles

These are composition profiles, not mandatory bundles.

| Shell profile | Typical selection | Use |
|---|---|---|
| **Shell 0 - Package Alone** | One package plus its declared hard dependencies | Package Laboratory, isolated prototype, or portfolio proof |
| **Shell 1 - Minimal Origin** | First Light; Observatory recommended; Passage optional | One boot path and one destination with visible startup status |
| **Shell 2 - Interactive Application** | First Light, Observatory, Accord, Passage, Pulse, Resonance, Will, Looking Glass | Menus, settings, scene travel, pause, input, and audio without game-save slots |
| **Shell 3 - Persistent Application** | Shell 2 plus Chronicle | Save-based games requiring slots, autosave, migrations, and recovery |
| **Shell 4 - Composed Starter** | The Workshop at Editor time plus a visible selection from Shell 0-3 | Generated project foundation; Workshop remains removable |

The Chronicle is never silently forced into a pathway that needs only passwords or global preferences.

---

## 11. Persistence pathways

Choose one primary durable-game-state approach per stage.

| Need | Selection | Notes |
|---|---|---|
| No durable game state | No Chronicle; Accord may still persist global preferences | Suitable for temporary prototypes and some arcade loops |
| Global preferences only | The Accord | Not a save slot |
| Password/unlock progression | The Ascent standalone persistence or project backend; Chronicle optional | Codes are convenience grants, not credentials |
| Single or multi-slot saves | The Chronicle plus package-owned participant payloads | Chronicle owns transport, generations, backup, and recovery |
| World/location state | The Atlas participant payload through Chronicle | Atlas does not own save files |
| Inventory/objectives/dialogue/characters | Each package exports its own versioned payload through Chronicle | No central god-schema inside Chronicle |
| Shared multiplayer save | Chronicle on authoritative host/server through a Convergence bridge | Clients do not publish shared truth |

A pathway must list every durable package payload and the authority that owns it.

---

## 12. Scene, world, and travel pathways

| Need | Selection |
|---|---|
| One scene, direct Play Mode | No Passage required; package or project direct-scene setup |
| Boot to one destination | First Light; minimal internal handoff or Passage bridge when selected |
| Normal menu/game/results travel | The Passage |
| Semantic worlds, zones, locations, discovery, and arrival markers | The Atlas |
| Online synchronized travel | Atlas prepares; Convergence coordinates; Passage executes; Atlas commits; Fellowship/project spawns |

A scene path, build index, world location, travel route, AI navigation path, and controller movement command are separate contracts.

---

## 13. Input, character, and controller pathways

| Need | Selection |
|---|---|
| Device/context/rebinding/glyph authority | The Will |
| Character identity, roster, selection, spawning, and control ownership | The Fellowship |
| Side-view or top-down 2D movement motor | The Vessel |
| Camera follow/modes/bounds | The Eye |
| Interaction discovery and semantic actions | The Hand |

The recommended control chain is:

```text
Input user/device
    -> Fellowship ControlOwnerId
        -> Vessel actor-local control lease
            -> actor-local motor
                -> Eye and Hand consume explicit actor targets
```

For a single unnamed pawn, The Vessel may operate without The Fellowship. For custom controllers, The Fellowship may operate without The Vessel.

---

## 14. Presentation, audio, accessibility, and localization pathways

- The Looking Glass owns screens, HUDs, modals, prompts, focus, and notifications.
- Resonance owns audio playback and mixer application.
- Impact coordinates feedback requests but delegates to channel authorities.
- The Accord owns persistent accessibility and preference values.
- Many Tongues owns locale selection and localized-reference resolution.
- The Will owns device-aware glyph selection data; UI presents it.

A pathway must identify which package owns the truth and which package only presents or applies it.

---

## 15. Gameplay-system selection rules

### 15.1 Objectives and progression

- Select The Path for objective-run and step progress.
- Select The Ascent for unlocks, access, checkpoints, passwords, and progression-node completion.
- Add a bridge only when a completed objective should issue an idempotent progression mutation.

### 15.2 Inventory and crafting

- Select The Vault for item/container truth.
- Select The Crucible for recipe transformation.
- Add their bridge when inventory-backed crafting is required.
- Item stats, vendor economics, and combat effects remain separate.

### 15.3 Dialogue and localization

- Select Voices for conversation flow.
- Select Many Tongues for localized text and assets.
- Select Looking Glass or project UI for presentation.
- Add bridges only for required behavior.

### 15.4 Combat, abilities, and AI

- Select Clash for instantaneous combat resolution.
- Select Arcana for ability lifecycle and effect orchestration.
- Select Instinct for AI perception and choice.
- The typical chain is Instinct choice -> Arcana activation -> Clash request -> target-owned mutation.

### 15.5 World and scene

- Select Atlas for semantic world identity.
- Select Passage for Unity scene transitions.
- Select Fellowship/project code for spawning or relocation.

---

## 16. Advanced and provider-backed selection rules

Advanced packages may enter a pathway only with their current maturity visible.

### 16.1 Neutral core versus provider

Selecting a provider-neutral core does not select a backend. A separate adapter, exact version, license review, Integration Laboratory, and compatibility record are required.

### 16.2 Research pathways

A research pathway may compare providers, topologies, or backends. It must:

- Use identical vertical slices where comparison is intended.
- Keep all empirical results `Not run` until executed.
- Preserve provider cost, license, lock-in, hosting, platform, migration, and security unknowns.
- Avoid presenting the winner before evidence and an ADR.

### 16.3 Production pathways

A production pathway may name an Advanced provider only after its support claim satisfies SFGSS-004 and the package's research protocol.

---

## 17. PATH-000 - Blank Modular Starter

**Purpose:** Create a clean project-owned folder and documentation foundation without selecting any runtime Echo package.

| Tier | Selection |
|---|---|
| Minimum | The Workshop at Editor time, or manual project setup |
| Recommended | Repository README, Current Notes, project configuration root, test folders |
| Optional | Any later package selected visibly |
| Excluded initially | Every runtime package |

**First vertical slice:** The project opens, compiles, contains the planned project-owned folder structure, and reports exactly what was generated.

**Project-owned work:** Game identity, scenes, code, content, and future package choices.

**Removal:** Remove The Workshop after generation; generated project content remains owned by the project.

**Maturity:** Approved guidance; Workshop candidate. The existing Blank Modular preset remains unexecuted.

---

## 18. PATH-001 - Package Laboratory and Portfolio System

**Purpose:** Prove one package or controller family independently and document it as a portfolio-quality system.

| Tier | Selection |
|---|---|
| Minimum | The package under test plus declared hard dependencies and its Standalone/Editor Laboratory |
| Recommended | The Observatory only when the package's optional bridge is being tested separately |
| Optional | One explicit Integration Laboratory |
| Excluded initially | Unrelated gameplay packages and combined showcase dependencies |

**First vertical slice:** The package's smallest advertised workflow passes in isolation.

**Evidence:** Clean install, direct Laboratory entry, duplicate/missing configuration behavior, sample removal, and package removal.

**Use cases:** Echo Systems Lab case studies, package development, controller preset proof, audio lab, save lab, or provider simulation.

---

## 19. PATH-010 - Minimal Audiovisual Prototype

**Purpose:** Boot to one playable or visual scene with music/SFX and clear startup diagnostics.

| Tier | Selection |
|---|---|
| Minimum | First Light, Resonance |
| Recommended | The Observatory, The Passage |
| Optional | Impact, The Looking Glass |
| Excluded initially | Chronicle, Accord, Will, Pulse, and gameplay modules unless the prototype needs them |

**First vertical slice:** Boot -> startup status -> one destination -> one music track -> one SFX cue.

**Project-owned work:** Destination scene, audio clips/profiles, trigger script, and presentation art.

**Removal:** Resonance and its content references can be removed without breaking First Light. Passage remains optional.

---

## 20. PATH-020 - Game Jam Quickstart

**Purpose:** Produce a visible, understandable application shell for a short game jam without forcing save infrastructure or a genre controller.

| Tier | Selection |
|---|---|
| Minimum | First Light, The Passage, The Pulse, Resonance, The Will, The Looking Glass |
| Recommended | The Observatory, The Accord |
| Optional | The Chronicle, Impact, The Wellspring, The Foundry, Many Tongues, one controller/character branch |
| Explicit choices | Save model, controller family, input actions, scenes, settings categories, audio profiles |
| Excluded initially | Advanced packages and broad RPG systems |

**First vertical slice:** Boot -> Main Menu -> Game -> Pause -> Results or Menu, with input context switching and audio.

**Chronicle rule:** No save package is silently selected. Choose none, preferences only, password progression, or save slots.

**Workshop rule:** Every selected package and bridge appears in the dry-run plan and generation report.

---

## 21. PATH-030 - Puzzle and Tabletop Game

**Purpose:** Support board, token, turn, card, match, or puzzle interactions without importing a movement controller or RPG inventory by default.

| Tier | Selection |
|---|---|
| Minimum | Interactive Application Shell, The Hand |
| Recommended | Impact, Resonance tabletop/UI profiles, The Ascent or The Chronicle according to persistence needs |
| Optional | The Path, The Vault for token/card ownership, Many Tongues, The Foundry |
| Excluded initially | The Vessel, Clash, Arcana, Instinct, and The Atlas unless the game proves a need |

**First vertical slice:** Start puzzle -> select/manipulate one semantic piece -> validate one move -> present success/failure feedback -> reset or continue.

**Authority note:** Puzzle rules remain project code. The Hand discovers/routes interactions; Impact and Resonance present feedback.

---

## 22. PATH-040 - Password-Based Puzzle Platformer

**Purpose:** Build a side-view puzzle/platform game with level passwords instead of full save slots.

| Tier | Selection |
|---|---|
| Minimum | Game Jam Quickstart, The Ascent, The Fellowship, The Vessel side-view 2D family, The Hand |
| Recommended | The Eye, Impact, The Wellspring |
| Optional | The Path, Many Tongues, The Foundry, The Chronicle if later save slots are required |
| Excluded initially | The Vault, The Crucible, Clash, Arcana, Instinct, Convergence |

**First vertical slice:** Boot -> select/start level -> move/jump -> interact with objective -> complete level -> generate/accept next-level password.

**Persistence:** The Ascent standalone record or project backend may persist unlocks. The Chronicle is optional.

---

## 23. PATH-050 - Save-Based Adventure

**Purpose:** Build a single-player adventure with scene travel, characters, objectives, interactions, progression, and durable saves.

| Tier | Selection |
|---|---|
| Minimum | Persistent Application Shell, The Fellowship, one Vessel controller family, The Hand, The Eye, The Path |
| Recommended | The Ascent, Impact, The Wellspring, Many Tongues, The Foundry |
| Optional | Voices, The Vault, The Crucible, The Atlas, Clash, Arcana, Instinct |
| Excluded initially | The Convergence unless multiplayer research is a named goal |

**First vertical slice:** Create/load slot -> spawn selected character -> travel/interact -> complete one objective -> save -> quit -> reload at approved resume state.

**Staging:** Add inventory, crafting, dialogue, combat, AI, and world topology only after the exploration/save loop passes.

---

## 24. PATH-060 - Narrative Game

**Purpose:** Build a conversation-driven game with localization, choices, objective consequences, and durable session state.

| Tier | Selection |
|---|---|
| Minimum | Interactive or Persistent Application Shell, Voices, The Looking Glass |
| Recommended | Many Tongues, The Chronicle, The Path, Resonance |
| Optional | The Eye, The Pulse bridge, The Ascent, The Atlas, Impact |
| Excluded initially | The Vessel, Clash, Arcana, Instinct, The Vault, The Crucible unless the narrative design needs them |

**First vertical slice:** Start conversation -> present localized line -> choose branch -> execute typed project command -> record safe conversation/objective state -> save/reload if selected.

**Authority note:** Voices controls conversation flow. The Path owns objectives. Project commands own story-specific consequences.

---

## 25. PATH-070 - Action Combat Prototype

**Purpose:** Build an actor-controlled combat arena that preserves separation between movement, abilities, combat resolution, AI, feedback, and pooling.

| Tier | Selection |
|---|---|
| Minimum | Interactive Application Shell, The Fellowship, one Vessel family, The Eye, Clash |
| Recommended | Arcana, Impact, The Wellspring, Instinct, Resonance |
| Optional | The Hand, The Vault, The Ascent, The Path, The Chronicle, The Atlas |
| Excluded initially | Online provider integration unless this is explicitly a multiplayer research prototype |

**First vertical slice:** Spawn player and target -> move/aim -> activate one ability or attack -> Clash commits one target result -> present Impact/Resonance feedback -> reset encounter.

**Authority note:** Clash does not own the project's health model. Arcana does not own damage resolution. Instinct chooses actions but does not commit combat outcomes.

---

## 26. PATH-080 - RPG Foundation

**Purpose:** Stage a broad RPG foundation without installing or implementing every RPG-adjacent system at once.

### Stage 1 - Explore and save

**Selection:** Persistent Application Shell, The Fellowship, The Vessel top-down 2D or project controller, The Eye, The Hand, The Chronicle.

**Vertical slice:** Create character/slot -> spawn -> move -> interact -> save -> reload.

### Stage 2 - Quest and content loop

**Add:** The Path, Voices, The Ascent, The Vault, Many Tongues.

**Vertical slice:** Accept objective -> collect/record item -> converse -> complete objective -> grant idempotent progression/inventory result -> save/reload.

### Stage 3 - Crafting and production

**Add:** The Crucible and Vault bridge.

**Vertical slice:** Exact combine or immediate recipe transaction -> output -> objective observation -> save/reload.

### Stage 4 - Combat and abilities

**Add:** Clash, Arcana, Impact, The Wellspring, Resonance integration.

**Vertical slice:** Activate ability -> commit combat result -> defeat/resolve target -> feedback -> persist approved progression only.

### Stage 5 - AI and world structure

**Add:** Instinct, The Atlas, optional `EchoRPG.Foundation` and provider adapters.

**Vertical slice:** AI perceives/chooses -> world route/location context -> travel -> spawn/arrival -> objective or combat loop.

### Optional multiplayer stage

Add The Convergence only after the single-player authority chain is clear and provider research becomes a named project goal.

**Important:** `EchoRPG.Foundation` owns genre-specific ancestries, classes, faiths, stats, spells, items, monsters, and content definitions. The general suite remains genre-neutral.

---

## 27. PATH-090 - Local Multiplayer Prototype

**Purpose:** Support multiple local users and characters without requiring an online networking provider.

| Tier | Selection |
|---|---|
| Minimum | Interactive Application Shell, The Will, The Fellowship, The Vessel |
| Recommended | The Eye with multiple channels or project split-screen adapter, The Looking Glass local-player UI, Impact |
| Optional | The Convergence neutral local-session contracts, The Chronicle for profile/save data, Clash/Arcana for gameplay |
| Excluded initially | Online transport, relay, matchmaking, authentication, and cloud hosting |

**First vertical slice:** Assign two local devices -> select/spawn two characters -> grant separate control leases -> move independently -> pause/disconnect one local player safely.

**Identity rule:** Input users, control owners, characters, controller leases, camera channels, and session participants remain separate mappings.

---

## 28. PATH-100 - Online Multiplayer Research Prototype

**Purpose:** Compare provider adapters through the approved Convergence disposable-prototype protocol without making a production support claim.

| Tier | Selection |
|---|---|
| Minimum | The Convergence neutral core, one candidate provider adapter, The Fellowship, The Passage, The Pulse, The Looking Glass |
| Recommended | The Will, The Observatory, The Atlas, The Chronicle authoritative-host seam |
| Optional | The Vessel, Clash, Arcana, The Path, The Vault, The Crucible according to the exact comparison slice |
| Required research artifacts | Provider matrix, license/cost review, prototype receipt, failure evidence, compatibility record |
| Excluded claim | "Production-ready multiplayer" before evidence and ADR approval |

**First vertical slice:** Host -> join -> ready -> synchronized travel -> spawn/ownership -> one authoritative action -> invalid request rejection -> disconnect/reconnect -> host interruption -> clean shutdown.

At least two providers execute the same slice before provider selection.

---

## 29. PATH-110 - Existing-Project Incremental Adoption

**Purpose:** Replace a project-specific system without destabilizing the working game.

This pathway is optional after the replacement package's applicable standalone
and clean-project evidence passes. Selecting a package beta does not silently
select an adoption target.

### Required sequence

1. Inventory the current authority, data, scenes, prefabs, API callers, and known behavior.
2. Select one replacement package only.
3. Install and validate it in its Standalone Laboratory.
4. Build a project adapter or approved bridge without deleting the old system.
5. Migrate one feature category or scene.
6. Compare parity, diagnostics, performance, persistence, and removal.
7. Keep a rollback branch or backup.
8. Remove the old system only after project acceptance evidence passes.
9. Reconcile documentation and migration records.

### Adoption examples

- Replace project audio with Resonance one cue/profile family at a time.
- Replace a save manager with Chronicle after slot, migration, recovery, and rollback proof.
- Introduce Fellowship around existing custom controllers before considering The Vessel.
- Adopt The Hand for interaction discovery while preserving project-specific interaction outcomes.

The Workshop may inspect and plan existing-project changes, but destructive replacement remains explicit and reversible.

---

## 30. Workshop mapping and dry-run requirements

The Workshop may implement a pathway as a preset only when it can produce an immutable dry-run plan containing:

- Pathway ID/version and preset ID/version.
- Selected shell profile.
- Every package, bridge, provider, adapter, and sample source/version.
- Every package-owned setup facade and selected setup domain.
- Every project folder, configuration asset, scene, route, profile, and generated report.
- Every persistence choice and durable participant.
- Every unresolved question or manual step.
- Every conflict, destructive operation, backup, and reversibility class.
- Every required Laboratory and readiness check.
- Every removal/decommission step.

If package resolution changes the plan materially after a domain reload, the Workshop invalidates approval and requests reapproval.

The Workshop does not own pathway runtime behavior. It invokes exact package-owned setup facades and records the result.

---

## 31. Bridge, provider, and adapter selection

A pathway must list integrations separately from core packages.

### 31.1 Bridge rules

- Both peer packages must be selected.
- The pathway must need the named cross-authority behavior.
- One bridge artifact owns one reusable peer-pair behavior.
- The bridge declares both package dependencies and is removed first.
- The bridge owns no competing durable truth.

### 31.2 Provider rules

- Provider package and exact version remain visible.
- License, cost, platform, hosting, migration, and lock-in considerations remain visible.
- Neutral-core selection never implies provider selection.
- Unsupported capabilities return unavailable rather than silently degrading into different semantics.

### 31.3 Project adapter rules

Use project code when integration depends on game-specific content, rules, or data ownership. A project adapter must not be disguised as reusable suite authority.

---

## 32. Laboratory, validation, and readiness requirements

A pathway is not proven by a combined showcase alone.

Required evidence sequence:

1. Every selected package's Standalone or Editor Laboratory passes for the claimed release/version.
2. Every selected bridge/provider Integration Laboratory passes.
3. The pathway's clean-project installation route succeeds.
4. The named first vertical slice succeeds.
5. Direct-scene, canonical boot, duplicate-authority, missing-configuration, removal, and reinstall behavior match the selected packages.
6. Project-owned content can be removed from samples without breaking package code.
7. Documentation and the pathway manifest match the tested composition.

Before implementation, every such result remains `Not run`.

---

## 33. Removal, downgrade, and pathway changes

### 33.1 Remove a bridge first

Remove the bridge/provider adapter before either core peer.

### 33.2 Preserve project-owned data

Removing package code does not automatically delete project configuration, saves, settings sections, generated receipts, migration backups, unknown payloads, or game content.

### 33.3 Replan composition changes

Adding or removing a package creates a new pathway plan/revision. The project must review:

- Compile dependencies.
- Scene and startup changes.
- Data ownership and unknown-record retention.
- Bridge removal/addition.
- Tests and migration.
- Documentation and rollback.

### 33.4 No silent downgrade

A preset or package downgrade is supported only when its migration and compatibility evidence explicitly says so.

---

## 34. Graph, documentation, and handoff rules

Every approved pathway must link to:

- [[Suite_Graph_Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap]]
- [[Current Notes]]
- Relevant package specifications
- Required integration matrices, ADRs, research records, and Laboratories

Project pathway manifests should become graph nodes linking the project to each selected package and bridge.

At checkpoint closeout:

1. Reconcile Current Notes.
2. Update the pathway manifest and stage status.
3. Promote new durable decisions into the proper authority or ADR.
4. Update tests, issues, migrations, and removal guidance.
5. Commit the pathway documentation with or immediately adjacent to the related project change.

A fresh collaborator should be able to answer what was selected, why, what remains project-owned, what has passed, and what can be removed without reconstructing an old chat.

---

## 35. Suite application matrix

| Package | Common pathways | Never silently selected because... |
|---|---|---|
| First Light | Minimal AV, Game Jam, Adventure, Narrative, RPG | Some package Labs and direct-scene prototypes do not need a boot composition root |
| Observatory | Recommended across development pathways | Diagnostics remains optional and removable |
| Accord | Interactive shells and accessibility/settings paths | A prototype may need no persistent preferences |
| Passage | Multi-scene projects | One-scene and direct-Lab projects may not need scene travel authority |
| Pulse | Interactive shells, dialogue, multiplayer, pause | A simple isolated package may not need high-level modes |
| Resonance | AV, Jam, Narrative, Action, RPG | Not every package or headless simulation needs audio |
| Will | Interactive player projects | Scripted Laboratory drivers and noninteractive tools do not need input authority |
| Looking Glass | Player-facing applications | Nonvisual packages and headless tests do not need UI |
| Chronicle | Save-based Adventure, Narrative, RPG | Password-only and temporary prototypes do not need save slots |
| Workshop | New-project composition | Manual composition remains supported; Workshop is Editor-only |
| Impact | Puzzle, Action, Adventure, RPG | Feedback channels are selected only when required |
| Wellspring | Action, projectile/VFX-heavy projects | Reuse does not own spawning intent |
| Ascent | Password, Adventure, Puzzle, RPG | Objective truth and save transport belong elsewhere |
| Foundry | Release-bound projects | Early Laboratory work may not need release tooling yet |
| Many Tongues | Narrative, RPG, supported multilingual projects | Localization is a project decision, not a hidden UI dependency |
| Voices | Narrative, Adventure, RPG | Conversation flow is not required by every project |
| Path | Adventure, Narrative, RPG | Not every prototype needs objectives |
| Vault | Adventure/RPG/item puzzles | Items are not required by every game |
| Hand | Adventure, Platformer, Puzzle, RPG | Direct action games may use project-specific triggers instead |
| Eye | Character-driven games | Static-camera and nonvisual projects may omit it |
| Fellowship | Character rosters, switching, multiplayer | A single unnamed pawn may not need roster authority |
| Vessel | Platformer, top-down, Action, RPG | Custom/project controllers remain supported |
| Crucible | RPG, crafting games, exact combine quests | Inventory does not imply crafting |
| Convergence | Online research, future multiplayer | No provider or production support is implied by the neutral core |
| Instinct | AI-enabled Action/RPG/world simulations | Enemy design and navigation backend remain project/provider decisions |
| Clash | Combat projects | It does not own target health or every gameplay interaction |
| Arcana | Ability-driven Action/RPG | Basic attacks may submit Clash requests without a full ability system |
| Atlas | World/zone/travel-heavy projects | A scene list does not automatically require semantic world topology |

---

## 36. Approval

### 36.1 Approval checklist

- [x] Pathways preserve one authority per concern.
- [x] No pathway is a hidden mandatory bundle.
- [x] Minimum, recommended, optional, and excluded selections are visible.
- [x] Every Foundation, Expansion, and Advanced package appears in the selection map.
- [x] Persistence choices distinguish Accord, Ascent, Chronicle, Atlas, and multiplayer authority.
- [x] Scene travel, world travel, navigation, and movement remain separate.
- [x] Bridges and providers remain explicit artifacts.
- [x] Advanced provider claims remain evidence-gated.
- [x] Existing-project adoption preserves working systems until parity.
- [x] Workshop mapping preserves dry-run visibility and manual composition.
- [x] Laboratories and evidence remain `Not run` until executed.
- [x] Package implementation remains checkpoint-controlled and every new
      package retains its just-in-time learning gate.

### 36.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse "Echo" Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Workshop presets remain candidates until implemented and tested. Provider-backed pathways remain research or experimental until their required evidence and ADRs exist.

---

## Graph Navigation

#sfgss/standard #sfgss/pathways #sfgss/workshop #sfgss/composition

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-002_Dependency_Bridge_and_Assembly_Standard|SFGSS-002 Dependencies and Bridges]]
- [[SFGSS-003_Data_IDs_Serialization_and_Migration_Standard|SFGSS-003 Data and Migration]]
- [[SFGSS-004_Testing_Validation_Test_Labs_and_Release_Standard|SFGSS-004 Testing and Release]]
- [[SFGSS-005_Checkpoint_Build_Workflow_and_ChatGPT_Collaboration_Rules|SFGSS-005 Checkpoint Workflow]]
- [[Package Specifications/SFGSS-The-Workshop-EchoGameStarter-Package-Specification|The Workshop Specification]]
- [[Integration Specifications/Foundation_Cross-Package_Contract_Matrix|Foundation Contract Matrix]]
- [[Integration Specifications/SFGSS-INT-EXPANSION-001_Expansion_Cross-Package_Contract_Matrix|Expansion Contract Matrix]]
- [[Integration Specifications/SFGSS-INT-ADVANCED-001_Advanced_Cross-Package_and_Research_Contract_Matrix|Advanced Contract Matrix]]
- [[Current Notes]]
