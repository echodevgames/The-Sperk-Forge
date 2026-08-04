# The Sperk’s Forge — EchoDevGames Game Systems Suite Bible

**Document ID:** SFGSS-000  
**Version:** 0.23.0
**Status:** Approved lead architecture baseline; implementation program activated under checkpoint control  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Current development baseline:** Unity 6000.3.8f1  
**Last updated:** August 4, 2026

> “The Sperk guides our design journey. His almighty singularity lights the way.”

---

## Contents

1. Purpose of This Document
2. Document Authority and Change Control
3. Identity: Technical Neutrality with Verse Flavor
4. Suite Design Principles
5. Standard Package Anatomy
6. Runtime Topology and Lifecycle
7. Package Portfolio at a Glance
8. Foundation Package Capabilities
9. Expansion Package Capabilities
10. Advanced Package Capabilities
11. Cross-Package Ownership Matrix
12. Dependency and Integration Policy
13. Data, Persistence, and Serialization Rules
14. UI, Input, and Accessibility Rules
15. Testing and Release Standard
16. Recommended Development Waves
17. Relationship to Existing Projects
18. Documentation Suite to Produce
19. Guided Workflow Principles
20. New ChatGPT Conversation Handoff Protocol
21. Approved Architecture Decisions
22. Open Decisions Requiring Later Approval
23. Definition of Success
24. Immediate Next Step

---

## 1. Purpose of This Document

This document is the lead source of truth for **The Sperk’s Forge — EchoDevGames Game Systems Suite**, a family of modular Unity packages intended to accelerate prototypes, game jams, portfolio projects, and full games without forcing every project to inherit the same genre, art direction, or codebase.

It defines:

- The vision and design rules shared by the entire suite.
- The responsibility and authority of every planned package.
- The boundaries between packages.
- The permitted dependency and integration patterns.
- The distinction between runtime systems, editor tooling, samples, project content, and starter templates.
- The package-development waves and recommended implementation order.
- The documentation hierarchy that future package specifications must follow.
- The context required to resume the suite safely in a new ChatGPT conversation.

This is not the complete low-level specification for every package. Each package will receive its own detailed design document after this lead bible is approved. Those documents may expand implementation details, but they must not silently contradict the ownership boundaries established here.

### 1.1 Primary goal

The suite should allow a developer to begin a clean Unity project, select only the systems that project needs, configure them through clear assets and setup tools, and reach a stable playable foundation much faster than rebuilding the same infrastructure from scratch.

### 1.2 Secondary goals

- Preserve lessons learned in Rescuers2D, Don’t Get Vince’d, Echo Systems Lab, DeverQuest, and Hackulos without importing their accumulated project-specific dependencies.
- Make every package useful by itself.
- Make compatible packages feel intentionally connected when installed together.
- Support both novice-friendly setup and programmer-controlled extension.
- Create portfolio-quality examples of reusable gameplay-systems engineering.
- Establish documentation that can be supplied to ChatGPT or another collaborator without relying on the history of an older conversation.

### 1.3 Non-goal

The suite is not one universal game framework that owns the entire project. It is a catalog of focused systems. The game remains in charge of its own rules, content, scenes, characters, narrative, and presentation.

---

## 2. Document Authority and Change Control

The documentation suite has four authority levels.

| Level | Document type | Authority |
|---|---|---|
| 1 | Package Suite Bible | Owns suite-wide vision, vocabulary, package boundaries, dependency rules, and cross-package architecture. |
| 2 | Individual Package Specification | Owns the detailed public API, data model, runtime behavior, editor tooling, samples, tests, and acceptance criteria for one package. |
| 3 | Architecture Decision Records and Integration Specifications | Record specific decisions and define optional connections between two or more packages. |
| 4 | Checkpoint Build Plans, setup guides, test plans, and release notes | Explain how approved designs are implemented, configured, validated, and shipped. |

When two documents disagree:

1. Stop and identify the conflict.
2. Do not silently choose whichever document is newer.
3. Decide whether the suite boundary or only a package implementation has changed.
4. Update the higher-authority document first when its rule has genuinely changed.
5. Record major architectural changes in an Architecture Decision Record.

Every planned feature should carry one of these lifecycle labels:

- **Proposed** — an idea included for evaluation.
- **Approved** — accepted as part of the intended design.
- **In Development** — currently being implemented.
- **Implemented** — present in the package and validated.
- **Deferred** — still valid, but intentionally postponed.
- **Experimental** — available without a compatibility guarantee.
- **Deprecated** — supported temporarily while being replaced.
- **Removed** — no longer part of the suite.

This approved baseline uses package waves to indicate priority, not implementation status.

### 2.1 Repository-hosted living documentation

The suite uses **documentation as code**. During development, the Markdown documentation lives in the same Git repository as the work it describes. That repository is the durable source for review, history, comparison, and handoff. Obsidian is opened directly against the repository documentation folder or vault; it is an authoring and navigation surface over those files, not a separate copy or competing source of truth.

Every active suite or package repository must provide a clearly linked **`Current Notes.md`** page. This page is the first capture surface for:

- Work-in-progress observations.
- Questions and uncertainties.
- Proposed changes.
- Test results and failure symptoms.
- Newly discovered risks or dependencies.
- ChatGPT handoff notes.
- Items that may need promotion into an authoritative document.

`Current Notes.md` is intentionally quick to edit, but it is not automatically authoritative. An entry becomes binding only when it is reconciled into the correct destination:

| Note type | Permanent destination |
|---|---|
| Suite-wide architectural change | SFGSS-000 and, when material, an ADR |
| Package behavior/API change | Active package specification and, when material, an ADR |
| Implementation progress | Current checkpoint/status record |
| Defect or regression | Issue log/test record |
| User-facing release change | Changelog/release notes |
| Setup or usage discovery | User/developer guide |

At every meaningful checkpoint, `Current Notes.md` must be reviewed, resolved items must be marked or promoted, affected authoritative documents must be updated, and the documentation changes must be committed to the repository. Whenever practical, behavior and its documentation should enter the same commit; otherwise, use an immediately adjacent, clearly labeled documentation commit.

Git history is the archive. Resolved working notes may be condensed or removed after promotion instead of being allowed to grow forever. Device-specific Obsidian workspace state and personal UI preferences should not be committed unless the repository deliberately adopts them as shared configuration.

---

## 3. Identity: Technical Neutrality with Verse Flavor

The connected fantasy/RPG continuity surrounding **Hackulos**, the Sperk, and related imagery may provide personality to the suite. It must not make the packages harder to understand or make them appear genre-locked. The suite is authored independently by Jesse “Echo” Adams under EchoDevGames; Hackulos supplies creative continuity, not a runtime framework or organizational dependency.

### 3.1 Identity layers

The suite uses four separate identity layers:

| Layer | Purpose | Example |
|---|---|---|
| Suite brand | Names the place where reusable EchoDevGames systems are made | The Sperk’s Forge |
| Public package title | Memorable user-facing product identity | First Light, The Accord, The Crucible |
| Technical identifier | Clear API, assembly, namespace, and package identification | `EchoLaunch`, `EchoSettings`, `EchoCrafting` |
| Project identity | The actual game’s own terminology and content | Rescuers2D, Hackulos, Don’t Get Vince’d |

The public display names should not simply append `Echo` to every tool. Public package listings, setup headings, icons, tooltips, and guidance prompts may lead with the Verse title, followed by a plain technical subtitle. Runtime types remain understandable without Verse knowledge and use names such as `EchoLaunchRoot`, `SaveSlotConfiguration`, `CharacterRoster`, and `CraftingRecipe`, not lore-only names.

Examples:

- **First Light — Startup and Launch**: “Awaken the systems this project needs.”
- **The Observatory — Diagnostics**: “See what the runtime is doing beneath the surface.”
- **The Chronicle — Save Infrastructure**: “Record the state that must endure.”
- **Jukebot — Resonance**: “Tune the music, effects, ambience, and mix.”

Flavor text must always be paired with an immediately clear technical label or explanation. A user should never need to understand the Hackulos fiction to configure a package correctly.

Verse flavor may appear in:

- Documentation introductions.
- Package icons and splash illustrations.
- Internal codenames.
- Release names.
- Optional sample scenes.
- Setup-wizard headings and small flavor lines.
- EchoDevGames marketing and portfolio case studies.

Verse flavor must not appear as:

- Required gameplay lore.
- Hard-coded user-facing text.
- Mandatory asset names in another game.
- Obscure class names that hide technical responsibility.
- A dependency on Hackulos data or runtime code.

### 3.2 Approved suite identity

| Identity | Approved usage |
|---|---|
| Formal public title | **The Sperk’s Forge — EchoDevGames Game Systems Suite** |
| Short title | **The Sperk’s Forge** or **Sperk’s Forge** |
| Lore continuity line | **Forged in the Hackulos Verse.** |
| Publisher and owner | **Jesse “Echo” Adams / EchoDevGames** |
| Documentation prefix | **SFGSS** |
| Unity package prefix | **`com.echodevgames`** |

**The Sperk’s Forge** communicates the suite’s actual purpose: it is the place where reusable foundations, tools, and game systems are designed, tested, and prepared for new projects. **EchoDevGames Game Systems Suite** immediately explains what the product is and who makes it. **Hackulos** remains present through the continuity line, package flavor, visual language, guidance copy, and the mythology of the Sperk without making “Hackulos” part of every technical identifier.

This naming system is intentionally layered. Public documentation may use the full title on covers and the short title in ordinary prose. Package listings lead with their memorable package title and follow it with a plain responsibility. Technical APIs, assemblies, namespaces, and package IDs remain direct and use the established EchoDevGames identity.

The suite is independent of Isekai Studios. Isekai branding, repositories, organization identifiers, credits, ownership language, and package prefixes must not be added unless Jesse explicitly changes that boundary in a later recorded decision.

### 3.3 Approved public package-title map

SFGSS-008 owns the canonical spelling, punctuation, identifiers, package IDs, namespaces, and prefixes. This summary records the approved public-title layer.

| Technical identifier | Public short title | Plain responsibility |
|---|---|---|
| EchoLaunch | First Light | Startup and Launch |
| EchoDiagnostics | The Observatory | Diagnostics and Runtime Inspection |
| EchoSettings | The Accord | Global Preferences |
| EchoSceneFlow | The Passage | Scene Flow |
| EchoGameState | The Pulse | Runtime State |
| Jukebot | Resonance | Audio Runtime |
| EchoInput | The Will | Input Infrastructure |
| EchoUI | The Looking Glass | UI Framework |
| EchoSave | The Chronicle | Save Infrastructure |
| EchoGameStarter | The Workshop | Project Starter |
| EchoFeedback | Impact | Coordinated Feedback |
| EchoPool | The Wellspring | Runtime Object Pooling |
| EchoProgression | The Ascent | Progression, Unlocks, Passwords, and Checkpoints |
| EchoBuildTools | The Foundry | Build Preparation, Validation, and Release Output |
| EchoLocalization | Many Tongues | Localization, Locale, and Regional Content |
| EchoDialogue | Voices | Dialogue and Conversation Flow |
| EchoObjectives | The Path | Objectives, Quests, and Tasks |
| EchoInventory | The Vault | Inventory and Item Containers |
| EchoInteraction | The Hand | World Interaction |
| EchoCamera | The Eye | Camera Direction |
| EchoCharacters | The Fellowship | Character Identity and Roster |
| EchoControllers | The Vessel | Player Controller Foundations |
| EchoCrafting | The Crucible | Recipe Transformation and Production |
| EchoMultiplayer | The Convergence | Multiplayer Sessions and Authority |
| EchoAI | Instinct | AI Perception, Decisions, and Behavior |
| EchoCombat | Clash | Combat Messages, Targets, and Resolution |
| EchoAbilities | Arcana | Ability Activation and Effect Orchestration |
| EchoWorld | The Atlas | World Identity, Topology, and Travel Metadata |

Formal public titles use `Short Title – Plain Responsibility`. ASCII-only surfaces may use a spaced hyphen. Public titles do not replace technical identifiers in code, packages, assemblies, namespaces, serialization, or provider protocols.

---

## 4. Suite Design Principles

### 4.1 One clear authority per concern

Every runtime concern has exactly one authoritative owner.

- Jukebot owns audio playback.
- EchoSettings owns global preferences.
- EchoSave owns durable game-save files and slot management.
- EchoSceneFlow owns scene-transition execution.
- EchoGameState owns the current high-level runtime state and pause authority.
- EchoUI owns reusable screen and HUD presentation infrastructure.

Other packages may request an action from the authority; they must not create a competing version of it.

### 4.2 Standalone first, composition second

Every package must operate without EchoLaunch or EchoGameStarter unless its purpose is specifically to compose other packages.

When installed alone, a package may use its own duplicate-safe runtime root. When EchoLaunch is installed, that root can be created or initialized through an EchoLaunch integration step. The package retains authority over its own behavior either way.

### 4.3 Minimal core, expandable edges

The runtime core contains only behavior required for the package’s central promise. Optional inspectors, laboratories, examples, profiles, adapters, integrations, and genre presets live at the edges.

### 4.4 Project-owned content

Packages provide types, templates, empty profiles, safe sample data, and authoring tools. Each game owns its concrete content assets.

Examples of project-owned content include:

- Audio clips and filled audio profiles.
- Game scenes and scene names.
- Character definitions and artwork.
- Save-data schemas.
- Crafting recipes.
- Inventory item catalogs.
- Dialogue scripts.
- Input action choices.
- UI styling and game-specific screens.

### 4.5 Data-driven configuration

Use ScriptableObjects for reusable definitions, catalogs, configurations, profiles, and presets when they improve authoring and validation. Use serializable runtime/save models for mutable state. Do not store changing player state directly in shared design assets.

### 4.6 Easy path and advanced path

Each package should offer:

- A safe prefab or setup command for fast installation.
- Inspector-driven configuration for common uses.
- A documented public API for programmers.
- Interfaces or events for extension.
- Diagnostics for configuration mistakes.

### 4.7 No hidden scene assumptions

A package must not silently depend on a particular scene name, build index, tag, layer, input asset, Resources path, or manually enabled object unless that requirement is declared and validated.

### 4.8 Duplicate safety before side effects

Any persistent authority must reject duplicates before it subscribes to events, changes settings, creates pools, plays audio, loads data, or begins asynchronous work.

This rule directly addresses the conflicting-bootstrap failure discovered during Rescuers2D audio development.

### 4.9 Samples are not production dependencies

Samples demonstrate setup but are never required by runtime assemblies. Sample scenes, sample clips, test controllers, and demo assets belong in `Samples~` or a separate sample package.

### 4.10 Optional integrations remain optional

Installing Jukebot must not require EchoUI. Installing EchoSave must not require EchoCharacters. Integration is provided through one of these mechanisms:

1. A bridge assembly or bridge package that depends on both systems.
2. An interface implemented by project code.
3. A configuration reference explicitly supplied by the project.
4. A compile-time optional integration guarded by assembly/version definitions.

Reflection-based discovery should be used sparingly and must fail safely.

### 4.11 Clean removal

Removing one optional package should not corrupt unrelated project data or leave other runtime assemblies unable to compile. Bridge packages must make dependency direction visible.

### 4.12 Honest scope

The suite should expose extension seams without claiming to solve every genre. A reusable foundation is successful when it removes repeated infrastructure work while leaving game-specific creativity and rules in the game.

### 4.13 Definition, runtime, presentation, and feedback separation

Echo Systems Lab repeatedly separates configurable definitions from runtime controllers, presentation, and feedback. The package suite adopts that as a formal rule:

```text
Definition/configuration -> Runtime authority -> State/events -> Presentation and feedback
```

- ScriptableObjects and configuration assets describe reusable rules and tuning.
- Runtime services/controllers own changing state and execute behavior.
- Events, interfaces, requests, and results expose meaningful changes.
- UI, audio, animation, VFX, and other feedback respond without becoming the rule authority.

A presentation component may request an action, but it must not become the only place where a gameplay or persistence rule exists.

### 4.14 Focused components and explicit collaboration

Prefer several components with narrow, named responsibilities over one manager that owns unrelated domains. Components should collaborate through:

- Narrow interfaces for capabilities.
- Events for meaningful state changes.
- Request/result APIs for operations that can fail.
- Serialized configuration for project-selected dependencies.
- Optional bridge assemblies for cross-package behavior.

Event emitters and animation relays translate observed actions into semantic signals. They do not silently own the timing, rules, save state, or low-level service they request.

### 4.15 Repeatable and non-destructive authoring

Setup, generation, validation, repair, and migration tools must be safe to run again. When a tool would replace or transform project-owned data, it must preview the target, preserve a backup where practical, report exactly what changed, and require an explicit destructive choice.

Package updates must preserve stable Unity asset identities. Scripts, templates, and independently creatable ScriptableObject types must keep valid `.meta` files and GUIDs across releases.

### 4.16 Documentation is part of the product

Documentation is a required package surface, not an afterthought. Each package should support three readers:

- A user seeking the shortest successful setup path.
- A developer extending or integrating the public API.
- A maintainer diagnosing, migrating, validating, or releasing the package.

The package README remains concise and routes readers into `Documentation~`. Project-development notes may live in an Obsidian vault or repository documentation folder, but runtime packages must not require an Obsidian vault to function.

### 4.17 Preserve intent, not every historical implementation

Existing projects are evidence of design intent and proven workflows, not templates to copy blindly. The suite preserves their successful separation, data-driven authoring, event flow, stable IDs, and focused components while correcting accumulated project shortcuts such as multiple independent persistent managers, hard-coded scene names, fixed save filenames, project-specific database references, and mutable runtime state stored in shared definition assets.

### 4.18 Scene-first isolation standard

Every package and independently selectable feature must be demonstrable in its own isolated Unity scene wherever a scene is technically meaningful.

- A **Standalone Test Lab** scene may depend only on the package under test, its declared Unity dependencies, and redistributable sample assets.
- An **Integration Lab** scene tests one explicit bridge or selected package combination and remains separate from standalone proof.
- A **Showcase** scene may present many systems together, but it never replaces isolated testing.
- Each controller preset in the controller library receives its own scene rather than sharing one controller-dependent test level.
- Direct-scene development helpers may create only the minimum authority required for that lab and must use the same duplicate-safety rules as production.
- Lightweight project adapters translate between packages; they must not recreate either package’s authority.
- A package is not release-ready if its sample works only because unrelated package code happens to be present in the project.

The preferred sample layout is:

```text
Samples~/
├── Standalone Labs/
│   └── <Feature> Lab/
├── Integration Labs/
│   └── <Package A> + <Package B>/
└── Showcase/
```

### 4.19 Diagnostics without mandatory coupling

Every runtime authority must expose enough structured state for a developer to understand whether it initialized, what it currently owns, and why its last request succeeded or failed. The Observatory can collect and visualize that state when installed, but no package may require the Observatory merely to function.

First Light always produces a structured launch report. The Observatory owns the richer ongoing runtime dashboard. The Workshop may select both by default for a new project, but the generated project must make the choice visible and removable.

---

## 5. Standard Package Anatomy

Unless a package specification approves an exception, a runtime package follows this layout:

```text
Packages/com.echodevgames.<package-id>/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
├── Runtime/
│   ├── Core/
│   ├── Data/
│   ├── Configuration/
│   ├── Features/
│   ├── Integrations/
│   ├── Prefabs/
│   └── EchoDevGames.<Package>.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Inspectors/
│   └── EchoDevGames.<Package>.Editor.asmdef
├── Samples~/
│   └── <Package> Test Lab/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 5.1 Naming convention

| Element | Pattern | Example |
|---|---|---|
| Package ID | `com.echodevgames.<kebab-name>` | `com.echodevgames.echo-launch` |
| Display name | Verse title plus plain responsibility | `First Light — Startup and Launch` |
| Runtime assembly | `EchoDevGames.<Package>.Runtime` | `EchoDevGames.Jukebot.Runtime` |
| Editor assembly | `EchoDevGames.<Package>.Editor` | `EchoDevGames.Jukebot.Editor` |
| Namespace | `EchoDevGames.<Package>` | `EchoDevGames.EchoSave` |
| Test assembly | `EchoDevGames.<Package>.Tests.*` | `EchoDevGames.EchoUI.Tests.Runtime` |

`Echo...` identifiers remain provisional technical handles. Public product listings lead with each distinctive Sperk’s Forge package title. `Jukebot` is an intentional technical and product-name exception.

### 5.2 Required package deliverables

Before a package can be called release-ready, it should contain:

- A valid UPM manifest.
- Runtime and Editor assembly definitions where applicable.
- A setup guide.
- A public API overview.
- A configuration reference.
- A troubleshooting section.
- A known-limitations and scope statement.
- Support, security-reporting, and contribution guidance appropriate to the release audience.
- A changelog.
- Licensing and third-party notices.
- A release-readiness checklist.
- At least one independently importable sample when visual setup is useful.
- Validation tools for common configuration failures.
- Automated tests for critical nonvisual behavior.
- A clean-project installation test.
- A tarball installation test.
- A project-specific integration test in at least one real game.
- At least one isolated Standalone Test Lab scene whenever the package has scene-visible behavior.
- Separate Integration Lab scenes for any advertised bridge; a combined showcase is optional and never substitutes for isolation.

### 5.3 Configuration ownership

Runtime packages may ship default configuration templates, but the configured instances used by a game belong to the game project. Updating a package must not overwrite a game’s filled profiles, save schema, scene list, recipes, or presentation assets.

### 5.4 Source and asset conventions

- Organize package code by feature/domain inside the required Runtime and Editor boundaries; avoid a single miscellaneous `Managers` or `Utility` folder becoming the architecture.
- Each independently creatable ScriptableObject type lives in a matching source file so Unity can associate assets reliably.
- Preserve committed `.meta` files and GUIDs for public scripts, templates, prefabs, configuration assets, and samples.
- Runtime assemblies use the approved package namespace even when source projects historically used the global namespace.
- Editor-only authoring, setup, migration, and validation code must not leak into runtime assemblies.
- Generated project content belongs outside immutable package source unless it is an intentionally shipped sample.

### 5.5 Repository strategy

The approved working model is a **hybrid multi-repository workspace**:

- Each major independently distributed runtime or Editor package receives its own public repository so visitors can inspect, star, clone, document, and version that system independently.
- A central **The Sperk’s Forge** repository owns the suite bible, catalog, roadmap, package compatibility matrix, and links to each package; it does not become a required runtime dependency.
- A separate **Sperk’s Forge Integration Lab** repository may check out released or local package versions side by side and test complete starter combinations.
- Developers may use local UPM path references or Git submodules inside the integration workspace, but package consumers are not required to understand that development arrangement.
- Small compile-safe integrations may live in a core package repository under the mixed bridge rule. Provider adapters and independently versioned bridge packages receive their own repositories when promoted to distribution.
- Tags and releases belong to the package repository whose artifact they ship. The central catalog records compatibility; it does not pretend every package shares one version number.

This model creates the independent GitHub presentation Jesse wants while preserving one practical workspace for cross-package development and testing.

SFGSS-009 makes this model operational: each package repository is a UPM package at repository root by default; package versions and release tags remain independent; the central repository catalogs rather than synchronizes package versions; the Integration Lab pins exact package revisions and commits both its Unity project manifest and lock file; Git-only peer dependencies are selected explicitly at the project level because Unity does not support Git dependencies between packages.

---

## 6. Runtime Topology and Lifecycle

### 6.1 The composition root

`EchoLaunch` is the suite’s optional composition root. It establishes the first persistent runtime and runs an ordered initialization sequence. It is not a container for every service and does not replace the individual package authorities.

### 6.2 Persistent roots

The suite does not require every component to be a singleton. It permits one authoritative persistent root per concern where persistence is necessary.

```text
EchoLaunchRoot                    optional suite composition root
├── startup sequence
├── service references/registry
└── launch diagnostics

Jukebot                           audio authority
├── MusicPlayer
├── SfxPlayer
└── AmbiencePlayer

EchoUIRoot                        persistent UI authority when configured
├── screen layer
├── HUD layer
├── modal layer
└── transition layer
```

Child players, pools, screens, emitters, voices, widgets, save serializers, and transition effects remain ordinary objects.

An authoritative persistent root may expose a documented convenience access point, but independently persistent child managers are not allowed. The root owns initialization order, child lifetime, and shutdown. A package must also permit test injection or an explicit project adapter so a global access point does not become the only usable API.

### 6.3 Initialization phases

EchoLaunch should support a deterministic sequence resembling:

1. **Claim runtime** — reject duplicate launch roots.
2. **Preflight** — verify required configuration and display a useful failure report.
3. **Diagnostics** — establish logging and the startup report.
4. **Preferences** — load global settings.
5. **Persistence** — initialize save and progression access if installed.
6. **Core services** — initialize audio, scene flow, game state, input context, and UI as selected.
7. **Presentation** — run studio, engine, publisher, accessibility, and game splash entries.
8. **Destination selection** — choose main menu, continue destination, test scene, or configured first scene.
9. **Transition** — load the destination through EchoSceneFlow when available or a minimal internal launch transition when standalone.
10. **Handoff** — mark launch complete and release startup-only resources.

Steps must declare whether they are required, optional, retryable, skippable, or allowed to fail with a warning.

### 6.4 Direct-scene testing

Direct-scene testing is a development feature, not a second production bootstrap.

When a developer starts a gameplay scene directly:

- Detect whether the required authority already exists.
- Create the configured development runtime only when absent.
- Reject duplicates before initialization.
- Clearly identify that the session used development initialization.
- Allow projects to require the canonical Boot scene for sensitive tests.
- Exclude or disable development helpers in release builds unless explicitly approved.

---

## 7. Package Portfolio at a Glance

### 7.1 Foundation wave

| Package | Core responsibility | Explicitly does not own |
|---|---|---|
| EchoLaunch | Startup, splash sequencing, ordered initialization, initial destination | Audio, saves, menus, gameplay rules |
| EchoDiagnostics | Validation, runtime inspection, categorized diagnostics | Production gameplay behavior |
| EchoSettings | Global preferences and preference persistence | Per-save game progress |
| EchoSceneFlow | Safe scene transitions after or during launch | Game-state rules or scene content |
| EchoGameState | Runtime modes, pause authority, time/cursor/input coordination requests | Individual menu visuals or input bindings |
| Jukebot | Music, SFX, ambience, mixer routing | Game-specific audio assets and gameplay decisions |
| EchoInput | Input contexts, rebinding, device awareness, glyph data | Genre-specific character movement |
| EchoUI | Screen/HUD framework, navigation, modal and notification layers | Game rules and permanent save ownership |
| EchoSave | Save files, slots, serialization, migration, recovery | The design of every game’s mutable data |
| EchoGameStarter | Editor-driven project composition and preset generation | A permanent runtime god-manager |

### 7.2 Expansion wave

| Package | Core responsibility | Explicitly does not own |
|---|---|---|
| EchoFeedback | Coordinated camera shake, hit stop, rumble, flashes, and response recipes | Damage calculation or audio playback internals |
| EchoPool | General-purpose object reuse and pool diagnostics | Game-specific spawning rules |
| EchoProgression | Unlocks, passwords, checkpoints, level access | General save-file transport |
| EchoBuildTools | Build profiles, versioning, preflight, release validation | Runtime game flow |
| EchoLocalization | Locale tables, localized references, fonts, formatting | Dialogue logic or UI layout |
| EchoDialogue | Conversation data and flow | Quest logic or complete cinematic direction |
| EchoObjectives | Objectives, quests, tasks, progress conditions | Inventory storage or dialogue rendering |
| EchoInventory | Items, containers, stacks, transfers, queries | Crafting transformations or equipment combat rules |
| EchoInteraction | World interaction detection and request routing | The unique result of every interaction |
| EchoCamera | Camera modes, targets, bounds, blends, and requests | Character control or scene loading |
| EchoCharacters | Character identity, roster, selection, spawning, switching, possession contracts | Genre movement, combat, or animation logic |
| EchoControllers | Modular player-movement foundations and genre controller presets | Character roster, combat, camera ownership, or one mandatory input map |
| EchoCrafting | Recipes, requirements, stations, processing, and outputs | Inventory implementation or item art |

### 7.3 Advanced and adapter wave

| Package | Core responsibility | Explicitly does not own |
|---|---|---|
| EchoMultiplayer | Network-agnostic session/gameplay contracts and provider adapters | A proprietary replacement for all networking libraries |
| EchoAI | Reusable sensing, decision, and behavior foundations | Every game’s enemy design |
| EchoCombat | Damage messages, teams, targets, and combat-resolution foundations | Animation, VFX, or one universal combat style |
| EchoAbilities | Ability definitions, activation rules, costs, cooldowns, and execution hooks | Class fantasy, spells, or specific ability content |
| EchoWorld | Shared world/zone identity, travel metadata, spawn markers, and world-state contracts | Full procedural generation or scene art |

The advanced packages are candidates until their individual specifications are approved. They should not delay the foundation wave.

### 7.4 Example package combinations

These are selectable compositions, not new packages or mandatory bundles.

| Project type | Recommended starting packages |
|---|---|
| Minimal audiovisual prototype | EchoLaunch, Jukebot, EchoSceneFlow, EchoDiagnostics |
| Game Jam Quickstart | EchoLaunch, Jukebot, EchoSettings, EchoSceneFlow, EchoGameState, EchoInput, EchoUI, EchoDiagnostics |
| Password-based puzzle platformer | Game Jam Quickstart plus EchoProgression, EchoCharacters, EchoControllers 2D Platformer preset, EchoInteraction |
| Save-based adventure | Game Jam Quickstart plus EchoSave, EchoProgression, EchoObjectives, EchoCharacters, and a selected EchoControllers preset |
| Puzzle/tabletop game | Game Jam Quickstart plus Jukebot Tabletop profiles, EchoSave or EchoProgression, EchoFeedback |
| Narrative game | Application shell plus EchoDialogue, EchoLocalization, EchoObjectives, EchoSave |
| RPG foundation | Application shell plus EchoSave, EchoCharacters, a selected EchoControllers preset, EchoInventory, EchoCrafting, EchoDialogue, EchoObjectives, and optional EchoRPG.Foundation |
| Local multiplayer prototype | Application shell plus EchoCharacters, selected EchoControllers presets, and EchoMultiplayer local-player/device adapters |
| Online multiplayer prototype | Application shell plus EchoCharacters, selected EchoControllers presets, EchoMultiplayer, one networking-provider adapter, and authoritative bridges for the selected gameplay systems |

---

## 8. Foundation Package Capabilities

## 8.1 EchoLaunch — First Light

### Mission

Provide one reliable, configurable beginning for every game without absorbing the systems it starts.

### Runtime capabilities

- Claim exactly one launch authority.
- Reject duplicate launch objects before side effects.
- Persist through the startup-to-destination transition when configured.
- Execute ordered startup steps.
- Support synchronous and asynchronous steps.
- Report progress, warnings, recoverable failures, and blocking failures.
- Run configurable splash sequences with image, animation, or video adapters.
- Configure minimum display duration, fade duration, skip rules, and legal-screen requirements.
- Select the initial destination: Main Menu, new game, continue, test scene, or a project-defined destination.
- Provide cancellation and timeout policy for startup steps where safe.
- Expose a launch-complete event and a structured launch report.
- Expose the current startup phase, active step, elapsed time, dependency status, and last result for diagnostics.
- Provide an optional plain startup-status presentation that can explain what is loading even when EchoUI and EchoDiagnostics are absent.
- Support boot-scene and preload-scene patterns.
- Support safe direct-scene development initialization.
- Allow a project to insert custom startup steps without editing package code.

### Core assets and types

- `EchoLaunchRoot`
- `EchoLaunchConfiguration`
- `StartupSequence`
- `StartupStep`
- `StartupStepResult`
- `SplashSequence`
- `SplashEntry`
- `LaunchDestination`
- `LaunchReport`
- `EchoDirectSceneInitializer`

### Editor capabilities

- Create or repair a canonical Boot scene.
- Create the launch configuration and prefab.
- Add approved startup steps.
- Validate build settings, destination scenes, duplicate roots, and missing references.
- Simulate startup failures and delays.
- Display an ordered startup graph or list.
- Display a visual systems map showing configured startup steps, declared dependencies, current state, warnings, and failures.
- Generate a portable launch report for bug reports.

### Integrations

EchoLaunch can initialize other packages through explicit startup-step integrations. Those integrations depend on EchoLaunch; the other packages do not require EchoLaunch to function. When the Observatory is installed, a bridge publishes the launch graph, active step, service health, timings, warnings, and final handoff into the runtime diagnostic dashboard.

### Non-goals

- It does not play music directly.
- It does not serialize save data.
- It does not implement the Main Menu.
- It does not own normal mid-game scene travel rules.
- It does not become a service locator for arbitrary gameplay code.

### Minimum first release

One Boot scene, one protected root, configurable image splash entries, ordered startup steps, a readable startup-status view, structured launch reporting, direct-scene testing, and one final destination.

---

## 8.2 EchoDiagnostics — The Observatory

### Mission

Make package setup and runtime failures visible before they become hours of debugging.

### Capabilities

- Categorized logging with package, severity, scene, and subsystem context.
- Installed Echo package inventory.
- Runtime authority inspector.
- Visually polished, configurable in-game overlay inspired by dedicated performance-monitor dashboards.
- Compact, expanded, and hidden display modes with a project-selected toggle binding or UI command.
- Dockable or selectable panels for performance, launch, services, scenes, audio, input, UI, saves, builds, and package health.
- Frame rate, frame time, memory, rendering counters, and other supported Unity performance metrics with explicit “unavailable” states when a platform does not expose a counter.
- Current scene, build/version, runtime mode, time scale, active input context, and persistent-authority summary.
- Warning thresholds, rolling graphs, recent event history, and low-overhead update-rate controls.
- Duplicate persistent-object detection.
- Missing-reference and invalid-configuration reports.
- Scene and Build Settings validation.
- Package version compatibility checks.
- Save/settings path display without exposing sensitive contents.
- Audio voice and music-state monitoring through a Jukebot bridge.
- UI stack, input-context, game-state, and scene-transition monitoring through optional bridges.
- Development overlay with configurable panels.
- Structured diagnostic snapshot export.
- Optional screenshot-safe and player-safe modes that omit internal paths or sensitive diagnostic details.
- Validation rules that can run manually, before Play Mode, or before a build.
- Simulation controls for missing data, startup failure, low voice count, and other approved test cases.

### Unity integration basis

The first implementation should use supported Unity runtime instrumentation rather than reproduce the entire Unity Profiler. Unity’s [`ProfilerRecorder` API](https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Unity.Profiling.ProfilerRecorder.html) can expose Profiler counters in Editor and Player builds, while tools such as the [URP Rendering Debugger](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/features/rendering-debugger-use.html) remain optional pipeline-specific integrations. The Observatory should wrap available counters behind its own provider interface so unsupported platforms degrade clearly instead of breaking the overlay.

The core overlay does not promise desktop-wide hardware sensors such as CPU/GPU temperature, fan speed, or utilization for every vendor. Those require later platform/native providers. The initial promise is a polished in-game view of game and Unity runtime health.

### Non-goals

- Diagnostics may observe and report; they should not silently repair production data during runtime.
- The package does not replace Unity’s Console, Profiler, or formal automated tests.
- It must not require every Echo package to be installed.
- Production visibility is explicitly configured; expensive or sensitive panels must not silently ship enabled in public builds.

### Minimum first release

Validation-result types, a package/scene validator window, duplicate authority checks, categorized logs, and a polished runtime overlay with FPS/frame-time, memory, current scene/build, authority health, and First Light startup panels.

---

## 8.3 EchoSettings — The Accord

### Mission

Own global player preferences that survive sessions independently of game-save slots.

### Settings domains

- Master, music, SFX, ambience, voice, and UI volume.
- Mute and dynamic-range preferences.
- Window mode, resolution, display, quality, VSync, and frame cap.
- Accessibility options such as subtitles, screen shake, flashes, hold/toggle behavior, text speed, high contrast, and reduced motion.
- Language and locale selection.
- Input preferences and rebinding references.
- Gameplay preferences that are global rather than save-specific.

### Capabilities

- Typed settings keys or strongly typed settings sections.
- Default values and reset-by-category.
- Load, edit, apply, cancel, and confirm workflows.
- Preview changes with safe rollback for display settings.
- Silent UI initialization that does not fire user-feedback sounds or save repeatedly.
- Change events scoped by setting or category.
- Storage abstraction with an approved default backend.
- Versioning and migration of settings data.
- Validation and repair of out-of-range values.
- Global settings plus optional profile-specific preference layers.
- Import/export for development and support when appropriate.

### Integrations

- Jukebot applies audio preferences.
- EchoUI presents settings screens.
- EchoInput stores and restores rebinding data.
- EchoLocalization applies locale changes.
- EchoFeedback observes reduced-motion and screen-shake preferences.

These connections should live in bridges or project integration code.

### Non-goals

- Story progress, inventory contents, checkpoints, and character stats belong to save/progression systems.
- EchoSettings does not own the visual menu.
- It does not hardcode one graphics pipeline.

### Minimum first release

Audio, display, and basic accessibility settings; one storage backend; defaults; reset; change events; and validation.

---

## 8.4 EchoSceneFlow — The Passage

### Mission

Provide one safe authority for scene travel, loading presentation, and transition locking.

### Capabilities

- Scene references that are safer than scattered string literals.
- Asynchronous load requests as the production default, with synchronous loading allowed only for an explicitly documented small-project, fallback, or test path.
- Single and additive loading.
- Loading progress reporting.
- Fade-out, load, activation, and fade-in phases.
- Loading-screen presentation hooks.
- Transition request queueing or rejection policy.
- Protection against double-click or duplicate transition requests.
- Current-scene reload.
- Return-to-menu and return-to-hub helpers configured by the project.
- Persistent-scene support.
- Unload requests and additive-scene ownership.
- Destination validation before transition.
- Transition failure reports and safe fallback destinations.
- Scene-entry and scene-exit lifecycle hooks.

### Integrations

- EchoLaunch may use EchoSceneFlow for the final startup transition.
- EchoUI may provide loading/fade presentation.
- EchoGameState may enter `Loading` during a transition.
- Jukebot may receive scene audio-profile requests.
- EchoMultiplayer adapters may coordinate synchronized scene travel.

### Non-goals

- It does not decide when a level is won.
- It does not contain the game’s level-selection rules.
- It does not own scene-specific enemies, objectives, or music assets.

### Minimum first release

Validated scene references, async single-scene loading, fade hooks, progress events, duplicate-request protection, and reload/return helpers.

---

## 8.5 EchoGameState — The Pulse

### Mission

Provide one readable authority for the game’s high-level runtime condition and for pause-related coordination.

### Typical states

- Booting
- Main Menu
- Loading
- Playing
- Paused
- Cutscene
- Dialogue
- Victory
- Defeat
- Shutting Down

Projects can add their own states without editing package source.

### Capabilities

- State definitions and validated transitions.
- State-entry and state-exit events.
- A stack or override model for temporary modes such as Pause, Modal, Dialogue, or Cutscene.
- Central pause authority.
- Time-scale policy.
- Cursor visibility and lock requests.
- Gameplay/UI input-context requests.
- Audio snapshot or pause-policy requests.
- Clear handling of nested pause reasons.
- State transition diagnostics.
- Optional state history for debugging.
- Prevention of invalid or repeated state transitions.

### Non-goals

- It does not render pause menus.
- It does not define every character state such as jumping or crawling.
- It does not replace a finite-state machine inside an enemy or player controller.
- It does not directly own every package it coordinates.

### Minimum first release

High-level state asset/configuration, transition authority, nested pause requests, time-scale policy, and cursor/input integration hooks.

---

## 8.6 Jukebot — Resonance

### Mission

Provide a clean, reusable, data-driven audio runtime that can be installed alone, tested in isolation, and connected to any project by swapping configuration assets rather than rewriting playback logic.

### Music capabilities

- Play, pause, resume, stop, and switch tracks.
- Two-source crossfading.
- Looping and loop-region support where technically appropriate.
- Playlists, queues, shuffle, previous, next, and repeat modes.
- Track metadata and optional intro/loop/outro sections.
- Scene or state music requests without embedding scene rules in Jukebot.
- Music priority and interruption policy.
- Music-volume and mixer routing.
- Safe persistence across scenes.

### SFX capabilities

- One-shot 2D playback.
- Positional playback.
- Pooled voices.
- Random, sequential, shuffle-bag, and weighted variations.
- Pitch, volume, delay, spatial, and mixer variation.
- Looping sounds with stoppable playback handles.
- Per-cue cooldowns.
- Global and per-cue concurrency limits.
- Voice-stealing policy.
- Follow-target playback.
- Surface/material selection hooks.
- Layered cue playback.
- Reverse playback as an experimental, platform-dependent feature only where supported and validated.

### Ambience capabilities

- Ambient beds and looping layers.
- Zone or profile requests.
- Blend and crossfade control.
- Optional randomized one-shots.
- Independence from the music transport.

### Audio profile templates

Jukebot should provide empty or safely populated profile templates. A game chooses the profiles that match its needs and fills the cue slots.

| Profile family | Representative cue slots |
|---|---|
| UI Audio | Navigate, confirm, cancel, denied, open, close, tab, slider tick, notification |
| Modern UI | Digital hover, select, error, panel, data, boot, success |
| Fantasy UI | Parchment, rune, coin, inventory, quest, spell, warning |
| Tabletop/Puzzle | Select, place, rotate, swap, match, clear, combo, invalid move, board shuffle |
| Platformer Character | Footstep, jump, land, climb, crawl, swim, hurt, death, interaction |
| Shooter Character | Movement, stance, equipment, reactions, abilities, low health |
| Gun Handling | Draw, holster, fire, dry fire, reload, magazine, chamber, safety, jam |
| Melee Combat | Swing, hit, block, parry, guard break, stagger, critical hit |
| Outdoor Movement | Grass, dirt, gravel, mud, shallow water, snow, foliage |
| Indoor Movement | Wood, tile, concrete, metal, carpet, stairs, vents |
| Environment Actions | Door, switch, lever, pickup, drop, break, repair, power, alarm |
| Spatial Environment | Wind, rain, machinery, fire, water, wildlife, crowds, distant events |
| Vehicle | Start, idle, acceleration, brake, impact, warning, enter, exit |
| Creature | Idle, alert, attack, hurt, death, movement, vocal variation |

Profiles define semantic slots, not game-specific clips. Composite character profiles should be allowed so a shooter character can combine movement, gun handling, reactions, and environmental-surface profiles without duplicating every cue.

Proposed template asset names include:

- `UIAudioProfile`
- `ModernUIAudioProfile`
- `FantasyUIAudioProfile`
- `TabletopSfxProfile`
- `CharacterSfxProfile`
- `MovementSurfaceAudioProfile`
- `GunHandlingAudioProfile`
- `MeleeCombatAudioProfile`
- `EnvironmentActionAudioProfile`
- `SpatialEnvironmentAudioProfile`

The Jukebot package specification will decide whether these are distinct ScriptableObject types, composable slot groups, presets built on one generic profile type, or a hybrid. The lead requirement is that the user can assemble only the semantic cue groups the game needs.

### Core assets and types

- `Jukebot`
- `JukebotConfiguration`
- `MusicTrack`
- `MusicPlaylist`
- `MusicPlayer`
- `SfxCue`
- `SfxVariation`
- `SfxPlayer`
- `SfxVoice`
- `SfxPlaybackHandle`
- `AmbienceProfile`
- `AudioProfile`
- Profile-family assets and emitters

### Editor and test tools

- Jukebot setup window.
- Configuration validator.
- Audio Laboratory.
- Music transport and crossfade tester.
- Playlist inspector.
- SFX cue previewer.
- Voice-pool stress test.
- Spatial playback test area.
- Library/profile completeness inspection.
- Runtime voice and routing diagnostics.

Cue assets remain definitions. Sequential indices, shuffle bags, cooldown timestamps, concurrency counts, and other mutable playback state belong to the active Jukebot runtime and are keyed by the cue reference or stable cue ID.

### Non-goals

- Jukebot does not decide that the player jumped or a door opened.
- It does not own menu navigation.
- It does not store user volume preferences permanently.
- It does not ship copyrighted game audio without redistribution rights.

### Minimum first release

Persistent duplicate-safe root, two-source music crossfade, pooled SFX, 2D/positional playback, variations, looping handles, mixer volume API, track/cue/configuration assets, and a test lab.

---

## 8.7 EchoInput — The Will

### Mission

Provide reusable input infrastructure around control schemes, contexts, rebinding, and presentation without pretending one gameplay controller fits every genre.

### Capabilities

- Active-device and control-scheme detection.
- Keyboard, mouse, and controller support.
- Input context switching for gameplay, menus, dialogue, cutscenes, rebinding, and disabled states.
- Rebinding UI data and rebinding workflow services.
- Binding-conflict detection and resolution policy.
- Saveable override data.
- Input glyph references and active-glyph switching.
- Hold, press, release, toggle, repeat, double-tap, and chord helper logic.
- Input buffering helpers where appropriate.
- Dead-zone and sensitivity preferences.
- Player input locking by reason rather than one fragile boolean.
- Local-player device assignment hooks.
- Two-page or multi-page control-schema presentation data.
- Optional action-map templates for platformer, top-down, first-person, third-person, and puzzle/tabletop prototypes.

### Non-goals

- No universal `PlayerController` belongs in the core.
- EchoInput does not calculate movement physics.
- It does not decide what an action means in a particular game.
- Multiplayer device pairing beyond local input assignment belongs to EchoMultiplayer integrations.

### Minimum first release

Active-device detection, context authority, rebinding storage model, conflict checking, glyph data, and a sample control-display screen.

---

## 8.8 EchoUI — The Looking Glass

### Mission

Provide one optional persistent UI root and a catalog of modular screens, HUD regions, navigation tools, and presentation services that a project can select without importing a monolithic menu manager.

### Recommended root layers

```text
EchoUIRoot
├── Screen Layer
├── HUD Layer
├── Modal Layer
├── Notification Layer
├── Tooltip and Prompt Layer
├── Transition Layer
└── Debug Layer
```

### Core capabilities

- Screen registration, opening, closing, replacement, and back navigation.
- Modal stack and input blocking.
- Focus memory and default selection.
- Keyboard, mouse, and controller navigation.
- Safe EventSystem coordination.
- UI transition hooks.
- Notification/toast queue.
- Confirmation dialogs.
- Tooltips and contextual prompts.
- Loading and fade presentation.
- Style/theme assets.
- Screen-reader and accessibility extension points where feasible.
- Separation between view prefabs and game-specific presenters/controllers.

### Optional screen templates

- Splash presentation.
- Main Menu.
- Pause Menu.
- Settings hub.
- Audio settings.
- Graphics/display settings.
- Accessibility settings.
- Controls and rebinding.
- Save-slot/profile selection.
- Password entry.
- Loading screen.
- Victory/level completion.
- Defeat/game over.
- Credits.
- Confirmation and warning dialogs.

### Optional HUD modules

- Health/status region.
- Timer region.
- Inventory/quick-slot region.
- Objectives region.
- Dialogue region.
- Tutorial region.
- Interaction prompt.
- Temporary notification feed.
- System-message area.
- Character/party region.

These templates expose data and presentation contracts; they do not impose the actual game logic behind health, inventory, quests, or characters.

### Initial UI technology

The first supported presentation path should be Unity uGUI with TextMeshPro. UI Toolkit support is a later adapter or major expansion, not a requirement for the first release.

### Non-goals

- EchoUI does not own settings persistence.
- It does not serialize saves.
- It does not calculate health or inventory rules.
- It does not require every template to be included in every project.

### Minimum first release

UI root, screen stack, modal stack, navigation/focus handling, notification service, Main/Pause/Settings templates, and integration examples.

---

## 8.9 EchoSave — The Chronicle

### Mission

Provide reliable, versioned, recoverable game-save infrastructure while allowing each game to define its own mutable data.

### Supported save models

| Model | Intended use |
|---|---|
| No game save | Arcade or password-only games with global settings only |
| Single slot | Short narrative games, jams, linear adventures |
| Fixed multi-slot | Zelda-style presentation with a configured maximum |
| Configurable multi-slot | Project chooses the slot limit |
| Unlimited profiles | Player-created profiles subject to platform/storage limits |
| Checkpoint autosave | Linear action and platforming games |
| Manual plus autosave | Larger adventures and RPGs |
| Session/run save | Roguelikes or games with temporary run state plus meta progression |

### Capabilities

- Slot/profile creation, selection, rename, duplicate, and deletion policies.
- Slot metadata independent from full save payloads.
- Display name, timestamp, playtime, progress summary, location, level, and optional thumbnail.
- Project-defined save-data contracts.
- Registration of narrow save participants/contributors so independent game systems can read and write their own versioned payload without EchoSave knowing project-specific databases or static stores.
- Serializer abstraction with one approved default.
- Atomic writes where the platform allows.
- Temporary file and replacement policy.
- Backup and recovery.
- Corruption detection and useful error results.
- Save version numbers and migration pipeline.
- Autosave rotation.
- Manual-save restrictions and validation.
- Platform path abstraction.
- Async save/load API where useful.
- Save request coalescing or locking.
- Development inspection and test-data generation.
- Clear separation between settings data and game-save data.

### Password-only games

EchoSave can be absent in a password-only project. EchoProgression can own password generation/validation and unlocked-level state, while EchoSettings stores only global preferences. If the game persists unlocked passwords locally, the chosen project integration determines whether that small record belongs to EchoProgression storage or an EchoSave payload.

### Non-goals

- EchoSave does not invent the game’s player stats or inventory model.
- It does not automatically serialize arbitrary scene objects.
- It does not promise cross-device cloud saves without a provider adapter.
- It does not hardcode three slots or character names.

### Minimum first release

Single/fixed/configurable slot modes, JSON default serializer, metadata, atomic write strategy, backup, version/migration hooks, autosave, and a Save Test Lab.

---

## 8.10 EchoGameStarter — The Workshop

### Mission

Compose selected packages, scenes, configurations, profiles, and sample integrations into a ready-to-develop project foundation.

EchoGameStarter is primarily an Editor package. It generates and configures project-owned assets; it is not the runtime owner of the systems it installs.

### Setup choices

- Project genre/preset.
- Selected Echo packages.
- Boot and splash configuration.
- Main Menu and UI modules.
- Settings categories.
- Save model.
- Input template.
- Jukebot profile families.
- Starting scenes and destination.
- Game-state model.
- Scene-transition style.
- Diagnostics level.
- Observatory overlay preset: off, development-only, player-accessible, or custom.
- Optional controller family and its isolated Test Lab.
- Optional expansion systems.

### Candidate starter presets

- Blank Modular Starter.
- Game Jam Quickstart.
- 2D Platformer.
- Top-Down Adventure.
- Puzzle/Tabletop.
- First-Person Prototype.
- Third-Person Prototype.
- Local Multiplayer Prototype.
- RPG Systems Starter.

### Generated project foundation

```text
Assets/<Game>/
├── Scenes/
│   ├── 00_Boot.unity
│   ├── 01_MainMenu.unity
│   ├── 02_Game.unity
│   └── 03_Results.unity
├── Configuration/
├── Audio/
├── UI/
├── Input/
├── Save/
├── Runtime/
└── Tests/
    ├── Standalone Labs/
    └── Integration Labs/
```

The exact structure remains configurable and should respect an existing project rather than blindly overwriting it.

### Capabilities

- Preset selection and package dependency report.
- Dry-run plan before file generation.
- Create-only-safe default behavior.
- Detection of existing assets and naming conflicts.
- Idempotent repair where practical.
- Generated setup report.
- Optional removal guide for generated pieces.
- Upgrade assistant when a starter template changes.
- Project readiness checklist.

### Non-goals

- It does not contain a permanent `GameManager` that every project must use.
- It does not overwrite existing scenes or configuration without explicit confirmation.
- It does not copy copyrighted sample content into a project.
- It does not conceal which packages and assets were installed.

### Minimum first release

Blank Modular Starter and Game Jam Quickstart, with EchoLaunch, Jukebot, EchoSettings, EchoSceneFlow, EchoGameState, EchoInput, EchoUI, EchoSave selection, and a readable generation report.

---

## 9. Expansion Package Capabilities

## 9.1 EchoFeedback — Impact

Coordinates reusable feedback recipes such as camera shake requests, hit stop, controller rumble, screen flashes, UI punch, time dilation, and chained responses.

Key rules:

- Feedback recipes are data-driven.
- Accessibility preferences can suppress or scale individual channels.
- Jukebot remains the audio authority; a feedback recipe requests an audio cue through a bridge.
- EchoCamera remains the camera authority; a recipe requests shake rather than moving the camera itself.
- Gameplay code reports meaningful events such as light hit, heavy hit, denied action, victory, or explosion.

Minimum release: feedback recipe, runner, channel scaling, cancellation, camera/audio/UI bridge examples, and stress testing.

---

## 9.2 EchoPool — The Wellspring

Provides reusable GameObject/component pooling with prewarm, maximum size, growth policy, return rules, diagnostics, and scene-transition handling.

Key capabilities:

- Pool definitions and catalogs.
- Spawn/return handles.
- `IPoolable` lifecycle callbacks.
- Automatic return by time or completion signal.
- Exhaustion policy.
- Pool ownership by scene or persistent runtime.
- Leak and double-return diagnostics.
- Statistics and stress tests.

EchoPool does not decide when enemies spawn, how projectiles deal damage, or which audio voices Jukebot uses internally.

---

## 9.3 EchoProgression — The Ascent

Owns access and advancement structures that are broader than one save write and smaller than a full RPG-stat system.

Capabilities may include:

- Level passwords and code validation.
- Checkpoints.
- Unlocked stages, modes, characters, difficulties, and bonuses.
- Completion records and rankings.
- Collectible/achievement-like progress flags without replacing platform achievements.
- New-game-plus flags.
- Progression snapshots for UI.
- Project-defined unlock conditions.

EchoProgression supplies serializable state that can be persisted by EchoSave or a small approved standalone backend. It does not own inventory, character stats, or platform achievements.

---

## 9.4 EchoBuildTools — The Foundry

Provides build preparation and repeatable release validation.

Capabilities may include:

- Named build profiles.
- Scene-list validation.
- Version and build-number stamping.
- Development/release defines.
- Package and license report.
- Missing-reference preflight.
- Platform-specific output naming.
- Clean output-folder policy with safe target validation.
- Changelog/release-note helpers.
- Itch.io and portfolio build checklist generation.
- Post-build manifest and checksum generation.

It does not deploy externally unless a separately approved provider integration is installed and explicitly invoked.

---

## 9.5 EchoLocalization — Many Tongues

Provides locale-aware content references and presentation support.

Capabilities may include:

- Localized string, asset, audio, image, and font references.
- Locale selection and fallback chains.
- Smart formatting and plural rules through an approved backend.
- Font/fallback configuration.
- Right-to-left extension points.
- Missing-key reports.
- Table import/export.
- Pseudolocalization and layout stress testing.
- Locale-change events.

Localization data remains project-owned. EchoLocalization does not author every translation and does not become the dialogue system.

---

## 9.6 EchoDialogue — Voices

Provides reusable conversation definitions and runtime flow.

Capabilities may include:

- Speaker definitions.
- Lines, sequences, branches, and choices.
- Conditions and project-defined commands.
- Typewriter and skip behavior contracts.
- Conversation history.
- Localization keys.
- Voice cue references.
- Portrait/emotion references.
- Conversation-state save data.
- Graph or structured-list editor.
- Validation of broken branches and missing speakers.

EchoDialogue does not own quest completion, cinematic camera direction, or the game’s entire narrative database. Those connect through integrations.

---

## 9.7 EchoObjectives — The Path

Provides objective, task, quest, and completion tracking.

Capabilities may include:

- Objective and quest definitions.
- Sequential, parallel, optional, hidden, and repeatable steps.
- Counters, flags, timers, and project-defined conditions.
- Prerequisites and dependency graphs.
- Rewards expressed as requests/contracts rather than hard-coded inventory changes.
- Tracked-objective selection.
- Progress snapshots for UI.
- Saveable objective state.
- Editor validation for unreachable or circular objectives.

EchoObjectives does not render dialogue, store inventory, or decide the implementation of every reward.

---

## 9.8 EchoInventory — The Vault

Provides a generic item/container foundation without forcing an RPG inventory on every game.

Capabilities may include:

- Item definitions and stable identifiers.
- Item instances for mutable per-item state.
- Stack rules.
- Containers, slots, capacity, weight, filters, and tags.
- Add, remove, move, split, merge, transfer, and query operations.
- Transactional operations that either fully succeed or make no change.
- Shared, personal, world, chest, vendor, and temporary container patterns.
- Item serialization contracts.
- Inventory change events.
- Validation and debug inspectors.
- Optional equipment-container concepts without owning combat statistics.

The first version should support simple jam inventories without requiring durability, randomized affixes, grids, equipment, vendors, or RPG stats. Advanced item behavior belongs in optional modules or `EchoRPG.Foundation`.

### Equipment boundary

EchoInventory may own generic equipment storage mechanics: named equipment slots, compatible-item filters/tags, equip/unequip transactions, occupied-slot rules, and equipment change events. It does not calculate armor, damage, attributes, class restrictions, set bonuses, encumbrance effects, or combat outcomes.

Those genre rules belong to project code or `EchoRPG.Foundation`. An optional bridge listens to successful equipment transactions and applies/removes RPG stat or ability effects. This keeps a cosmetic loadout, puzzle-tool belt, or simple jam equipment screen usable without importing an RPG statistics model.

---

## 9.9 EchoInteraction — The Hand

Provides consistent world-interaction discovery, selection, prompts, and execution requests.

Capabilities may include:

- 2D and 3D interaction detection adapters.
- Focused/selected interactable tracking.
- Range, angle, line-of-sight, priority, and availability evaluation.
- Prompt data.
- Hold, tap, toggle, timed, and repeated interactions.
- Interaction cancellation.
- Interactor and interactable interfaces.
- Multiplayer authority hooks.
- Input, UI, audio, objective, and feedback bridges.

The package does not decide what opening a specific door, rescuing a survivor, or placing C4 actually does. Project code implements the interaction result.

---

## 9.10 EchoCamera — The Eye

Provides requests and data for common camera behaviors while allowing a selected camera backend.

Capabilities may include:

- Camera target registration.
- Follow targets and groups.
- Bounds and confiners.
- Camera modes and priority.
- Blends and transitions.
- Look-ahead, dead zones, offsets, and zoom requests.
- Camera zones.
- Split-screen adapter hooks.
- Shake requests through EchoFeedback.
- Cutscene/dialogue camera hooks.
- Cinemachine adapter without making Cinemachine mandatory for every core type.

The package does not own player movement or level layout.

---

## 9.11 EchoCharacters — The Fellowship

### Mission

Provide neutral character identity, roster, selection, spawning, switching, and control-ownership infrastructure that can serve a single hero, a switchable rescue team, a party RPG, local multiplayer, or networked players.

### Capabilities

- Stable character definitions and identifiers.
- Character display metadata: name, portrait, icon, prefab reference, tags, and presentation data.
- Runtime character instances distinct from definitions.
- Roster creation and queries.
- Locked, unlocked, available, injured, missing, defeated, or project-defined availability states.
- Selected-character and active-character tracking.
- Switch-to-next, switch-to-specific, and selection-request validation.
- Character spawning and despawning through provider interfaces.
- Spawn-point selection contracts.
- Possession/control-owner assignment.
- Local player-to-character assignment.
- Party or squad grouping.
- Character replacement and respawn requests.
- Persistent roster-state snapshots.
- Character-selection UI data.
- Animation/controller handoff events without owning animation graphs.
- Camera-target and input-target handoff hooks.
- Multiplayer ownership hooks.

### Example uses

- Rescuers2D cycles Firefighter, Riot Officer, Rescue Specialist, and Dog while retaining one shared team state.
- A party RPG selects one active party member while displaying the full roster.
- A fighting game maps local players to selected character definitions.
- A multiplayer game lets the server validate which character prefab a player owns and spawns.

### Non-goals

- EchoCharacters does not implement walking, jumping, swimming, attacks, spells, or AI.
- Reusable movement implementations belong to the standalone EchoControllers companion package; project-specific controllers may implement the same control-ownership contracts without installing it.
- It does not own health, equipment, inventory, stats, or animation state machines.
- It does not assume every game allows switching.
- It does not store full save files; it provides serializable character/roster state to EchoSave.

### Minimum first release

Character definitions, runtime roster, active selection, validated switching, spawn provider, control-owner handoff, save snapshot, and a switchable-character sample.

---

## 9.12 EchoControllers — The Vessel

### Mission

Provide a growing catalog of reusable player-controller foundations without forcing roster management, combat, camera policy, animation graphs, or one project’s input map into every controller.

The long-term intent is to cover the controller families Jesse repeatedly needs across projects. “Every controller type” is a roadmap direction, not a promise to ship an untested monolith in the first release.

### Architectural model

Each controller preset follows the same separation:

```text
Input adapter -> normalized intent -> movement motor/state -> semantic events -> presentation
```

- A controller accepts normalized movement/action intent and does not require EchoInput.
- Optional Input System and EchoInput adapters translate actions into that intent.
- The motor owns movement execution and local locomotion state.
- Animation, audio, VFX, UI, camera, and character-roster systems consume events or use explicit bridges.
- Shared utilities remain lightweight: grounding, slope queries, facing, movement constraints, jump buffering, coyote time, and capability gates.
- Features such as climbing, crawling, swimming, ladders, knockback, or click-to-move are composable capabilities when practical, not boolean growth inside one universal controller.

### Planned controller families

- Side-view 2D platformer.
- Side-view 2D action/puzzle controller with switchable characters.
- Top-down 2D four-direction and eight-direction movement.
- Top-down 3D movement.
- Twin-stick movement/aiming.
- First-person movement and look.
- Third-person movement relative to camera facing.
- Point-and-click or click-to-move navigation.
- Grid/turn-based movement.
- Flying and zero-gravity movement.
- Swimming-focused movement.
- Vehicle/pawn control adapters where a future specification proves they belong here rather than in a vehicle package.

### Controller Test Lab rule

Every controller family ships with its own isolated scene, configuration preset, input adapter, visual debug readout, and acceptance checklist. One controller’s scene must not require another controller module, EchoCharacters, EchoLaunch, Jukebot, EchoUI, or project code.

An additional integration scene may demonstrate EchoCharacters selection and control handoff, but that scene is bridge evidence rather than the controller’s standalone proof.

### Relationship to EchoCharacters

EchoCharacters remains the authority for identity, roster, selection, spawning, and control ownership. EchoControllers owns reusable movement/control implementations. A lightweight bridge passes control ownership and selected-character intent to a controller instance.

This separation lets a game use:

- EchoCharacters with entirely custom controllers.
- EchoControllers for a single unnamed pawn with no roster system.
- Both packages together for switchable heroes, parties, local multiplayer, or possession.

### Non-goals

- EchoControllers does not calculate damage or implement combat rules.
- It does not own a character roster or save file.
- It does not require one animator controller, camera package, or input-actions asset.
- It does not promise identical physics behavior across Rigidbody2D, Rigidbody, CharacterController, and navigation-based presets.
- It does not place every locomotion mode into one all-purpose class.

### Minimum first release

A shared intent/motor contract, a side-view 2D controller, a top-down 2D controller, independent Test Lab scenes for both, Input System sample adapters, visual movement diagnostics, and one optional EchoCharacters control-handoff integration scene.

---

## 9.13 EchoCrafting — The Crucible

**Priority:** Deferred / deliberately low priority pending a dedicated crafting-design workshop and approved package specification.

### Mission

Provide recipe-driven transformation of inputs into outputs while supporting both a tiny quest-combine bag and a larger workstation or production system.

Crafting is expected to become a major system family across future games. For that reason, the suite must resist implementing a narrow first idea as the permanent foundation. The capability tiers below preserve the intended range, but implementation does not begin until recipes, skills/professions, discovery, quality, stations, queues, failure, repair, salvage, persistence, UI, and multiplayer authority have been explored as one coherent design.

### Capability tiers

#### Simple combine

- Exact authored ingredient sets.
- Container or bag-based combine request.
- Valid/invalid result feedback.
- Immediate output.
- Ideal for Hackulos’s authored quest recipe without requiring a full trade-skill system.

#### Standard crafting

- Recipe definitions.
- Ingredient quantities and alternatives.
- Tool, station, skill, tag, condition, and unlock requirements.
- Preview of outputs and byproducts.
- Immediate or timed completion.
- Consumable and non-consumable inputs.
- Batch crafting.
- Craft success/failure policy.

#### Production expansion

- Craft queues.
- Cancel/refund policy.
- Workstation capacity.
- Offline or elapsed-time completion only when explicitly designed.
- Quality, variants, and project-defined modifiers.
- Discoverable or hidden recipes.
- Salvage, dismantle, repair, and upgrade operations as optional modules.

### Data and runtime concepts

- `CraftingRecipe`
- `IngredientRequirement`
- `CraftingOutput`
- `CraftingStationDefinition`
- `CraftingContext`
- `CraftingRequest`
- `CraftingResult`
- `CraftingQueue`
- Requirement and output provider interfaces

### Integration model

EchoCrafting must not require EchoInventory. It consumes ingredients and grants results through provider interfaces. The EchoInventory bridge supplies the normal item-backed implementation. A puzzle game can provide its own token/grid implementation instead.

### Non-goals

- It does not define all item data.
- It does not render the crafting screen.
- It does not own player skills or progression.
- It does not force random success chances or MMO-style professions.

### Minimum first release

No implementation is approved by this lead bible alone. After the dedicated design workshop, the likely first vertical slice is exact recipes, ingredient/output providers, immediate crafting, validation, transaction safety, one simple combine UI sample, and one inventory bridge example.

---

## 10. Advanced Package Capabilities

## 10.1 EchoMultiplayer — The Convergence

**Priority:** Research/incubation. No networking provider or production implementation is approved by this lead bible.

### Mission

Reduce the repeated integration work required to make Echo systems multiplayer-aware without pretending to replace Netcode for GameObjects, Mirror, FishNet, Photon, Steam transport, platform services, or future networking solutions.

This package carries a deliberately high research burden because networking architecture, authority, transport, platform services, hosting cost, security, and player experience cannot be selected safely from naming familiarity alone. The first deliverable is a research and prototype program, not a reusable production package.

### Research and approval gates

Before selecting the first provider, produce a dated comparison covering:

- Supported Unity versions and maintenance outlook.
- Host/client, dedicated-server, peer, offline, and local-player models.
- Transport, relay, lobby, authentication, matchmaking, and platform-service options.
- Server authority, client prediction, reconciliation, interpolation, ownership, and late join.
- Scene synchronization, object spawning, disconnect/reconnect, and host migration.
- Pricing, licensing, service lock-in, deployment requirements, and operating cost.
- Documentation quality, diagnostics, test tooling, community maturity, and upgrade history.
- Fit for the studio’s likely game sizes, genres, platforms, and team experience.
- Security boundaries and authoritative validation requirements.

At least two disposable prototypes should test the same tiny vertical slice before one provider adapter is approved: host, join, ready, load a scene, spawn/select a character, perform one authoritative action, disconnect, and recover cleanly. Prototype code is evidence and may be discarded; it does not automatically become package code.

### Architectural model

EchoMultiplayer should be split into:

1. A small provider-neutral contracts package.
2. Separate adapters for approved networking stacks.
3. Optional bridges connecting multiplayer authority to Echo packages.

Possible package IDs:

```text
com.echodevgames.echo-multiplayer
com.echodevgames.echo-multiplayer.netcode-gameobjects
com.echodevgames.echo-multiplayer.steam
```

Actual provider names and licensing must be verified before release.

### Provider-neutral capabilities

- Session create, join, leave, and shutdown requests.
- Host, client, dedicated server, offline, and local-session roles.
- Player identity and local/remote participant records.
- Lobby/room metadata contracts.
- Ready-state tracking.
- Connection approval and rejection results.
- Reconnect and disconnect policy hooks.
- Scene-travel coordination contracts.
- Spawn/ownership request contracts.
- Authority checks expressed through interfaces.
- Network time abstraction.
- Replicated settings/rules snapshot contracts.
- Error and status events suitable for UI.
- Local multiplayer fallback concepts where appropriate.

### Echo package bridges

- EchoCharacters: player-to-character ownership, selection, spawn, and respawn.
- EchoSceneFlow: synchronized scene transitions and late-join scene state.
- EchoGameState: lobby, loading, playing, paused, disconnected, host migration policy.
- EchoUI: lobby, connection, ready, error, and reconnect screens.
- EchoInput: local-player devices and input authority.
- EchoSave: host/server save authority and player-profile data boundaries.
- EchoObjectives: server-authoritative shared objective progress.
- EchoInventory/Crafting: authoritative transactions.
- Jukebot: local presentation driven by networked events, never network-streamed audio playback state unless specifically required.

### Security and authority rules

- Important state changes are validated by the selected authoritative peer/server model.
- UI button presses and local effects are not treated as proof of a valid gameplay action.
- Save files are not blindly accepted as authoritative shared-world state.
- Crafting, inventory transfers, character selection, and objective completion expose validation seams.
- Provider adapters document which guarantees they can and cannot provide.

### Non-goals

- No promise of one-click multiplayer conversion.
- No custom low-level transport in the first versions.
- No hidden dependence on one vendor in provider-neutral assemblies.
- No universal matchmaking backend.
- No guarantee that all single-player game code is automatically safe to replicate.

### Minimum first release

After the research program and provider approval: provider-neutral session/player/authority contracts, one approved network-stack adapter, one lobby sample, EchoCharacters ownership bridge, synchronized scene-flow sample, and failure/reconnect diagnostics. Until then, the package remains an architectural candidate.

---

## 10.2 EchoAI — Instinct (Candidate)

Potential scope:

- Sensor and stimulus contracts.
- Perception memory.
- Target scoring.
- Reusable state-machine or behavior-node foundations.
- Navigation-provider abstraction.
- Blackboard/context data.
- Debug visualization.
- Common patrol, investigate, chase, flee, and return concepts as samples.

It should not ship a universal enemy brain or force one navigation technology.

---

## 10.3 EchoCombat — Clash (Candidate)

Potential scope:

- Damage/healing messages.
- Damageable and targetable interfaces.
- Teams/factions.
- Hit results and combat event data.
- Resistance/modifier extension points.
- Death/defeat events.
- Combat log data.
- 2D/3D hit-provider adapters.

It should not dictate one action, shooter, RPG, or fighting-game formula.

---

## 10.4 EchoAbilities — Arcana (Candidate)

Potential scope:

- Ability definitions and runtime instances.
- Activation conditions.
- Costs, charges, cooldowns, cast/channel timing, and interruption.
- Targeting-provider interfaces.
- Effect execution hooks.
- Ability bars/loadouts as data, with UI bridges.
- Save and multiplayer authority hooks.

Specific spells, attacks, classes, and effects remain game-owned or belong to `EchoRPG.Foundation` content.

---

## 10.5 EchoWorld — The Atlas (Candidate)

Potential scope:

- Stable world, zone, and location identifiers.
- Travel connections and destination metadata.
- Spawn markers and entry points.
- Scene-to-location mapping.
- Persistent world-state snapshot contracts.
- Fast-travel and discovery hooks.
- Minimap/map provider interfaces.

It should not become a full procedural-generation or level-design system.

---

## 11. Cross-Package Ownership Matrix

This matrix is authoritative when deciding where a new feature belongs.

| Concern | Authority | Common consumers/integrations |
|---|---|---|
| Initial game startup | EchoLaunch | All installed packages through startup steps |
| Normal scene travel | EchoSceneFlow | Launch, UI, objectives, multiplayer |
| Current runtime mode/pause | EchoGameState | Input, UI, Jukebot, scene flow |
| Music/SFX/ambience playback | Jukebot | UI, characters, feedback, environment, dialogue |
| Global preferences | EchoSettings | Jukebot, UI, input, localization, feedback |
| Game-save files and slots | EchoSave | Progression, characters, inventory, objectives |
| Input contexts/rebinding/glyphs | EchoInput | UI, characters, dialogue, multiplayer |
| Screens/HUD/modals/prompts | EchoUI | All gameplay systems through presenters |
| Validation/runtime inspection | EchoDiagnostics | Every installed package through optional panels |
| Starter generation | EchoGameStarter | Editor-time composition of selected packages |
| Unlocks/passwords/checkpoints | EchoProgression | UI, save, scene flow, objectives |
| Item/container ownership | EchoInventory | UI, crafting, objectives, save |
| Recipe execution | EchoCrafting | Inventory providers, UI, objectives, progression |
| Character roster/selection/spawn | EchoCharacters | Input, UI, save, camera, multiplayer |
| Reusable player movement/controllers | EchoControllers | Input adapters, characters, camera, animation, project gameplay |
| World interaction selection | EchoInteraction | Input, UI, audio, objectives, project gameplay |
| Dialogue sequence/choices | EchoDialogue | UI, localization, audio, objectives |
| Quest/objective progress | EchoObjectives | UI, save, dialogue, inventory |
| Camera modes and targets | EchoCamera | Characters, feedback, dialogue, scene flow |
| Coordinated game feel | EchoFeedback | Camera, Jukebot, UI, settings |
| General GameObject reuse | EchoPool | Projectiles, VFX, world objects |
| Locale and localized references | EchoLocalization | UI, dialogue, audio, settings |
| Multiplayer session/authority contracts | EchoMultiplayer | Characters, scene flow, game state, UI, save |
| Build preparation and validation | EchoBuildTools | All packages at build time |

### 11.1 Boundary test

When a feature appears to belong to multiple packages, ask:

1. Which package owns the underlying truth?
2. Which package only presents or requests that truth?
3. Can the connection be expressed as a small bridge?
4. Would either package still compile and remain useful without the other?

Example: an Audio Settings menu belongs visually to EchoUI, stores values through EchoSettings, and applies them through Jukebot. None of those packages should absorb the other two responsibilities.

---

## 12. Dependency and Integration Policy

**Canonical implementation standard:** SFGSS-002 — Dependency, Bridge, and Assembly Standard. This section owns the suite-level boundary; SFGSS-002 defines the package-manifest, asmdef, bridge/provider, compile-guard, test-assembly, and clean-removal rules that implement it.

### 12.1 Dependency types

| Type | Meaning | Rule |
|---|---|---|
| Platform dependency | Unity module or approved Unity package required for the central feature | Must be declared in `package.json` and package docs. |
| Hard Echo dependency | One Echo package genuinely cannot function without another | Avoid in core packages; requires approval in the package bible/spec. |
| Optional integration | Adds behavior when another package is installed | Use a bridge assembly/package or explicit project adapter. |
| Sample dependency | Needed only by a sample | Must not leak into production runtime assemblies. |
| Provider adapter | Connects an abstract package to a specific backend | Shipped separately and versioned independently when practical. |

### 12.2 No mandatory EchoCore at the beginning

The suite should not create a mandatory `EchoCore` or `EchoFoundation` package merely to hold a few helpers. A shared core easily becomes an accidental dependency for everything and makes versioning harder.

Create a shared contracts package only if:

- The same truly neutral contract is needed by at least three independent packages.
- Duplicating or locally owning the contract creates real incompatibility.
- The shared package can remain tiny, stable, runtime-safe, and genre-neutral.
- Its versioning and migration cost is documented.

`EchoRPG.Foundation` is different: it is an optional genre/data family for RPG projects, not a mandatory dependency of the general suite.

### 12.3 Mixed bridge and provider packaging rule

The suite uses a mixed rule chosen for simple installation without hidden dependencies:

1. A tiny integration may ship inside the package that owns the behavior when it can be completely excluded at compile time and does not add a provider SDK, separate license, or independent release burden.
2. A bridge that directly depends on two optional Echo packages ships as a small separate UPM package when including it inside either core would complicate installation, removal, or versioning.
3. Networking, cloud, platform, storefront, analytics, camera-backend, and other vendor/provider adapters ship separately from the provider-neutral core.
4. Game-specific translation remains project adapter code, even when a package sample demonstrates the pattern.
5. The Workshop may present these pieces as one checkbox or preset, but its generation report must show every package and bridge it selected.

The default decision test is usability: if a user who does not need the integration would inherit another package/SDK or face compile errors after removal, the integration must be separate.

EchoSave owns local save contracts, files, slots, migration, and provider-neutral synchronization seams. Cloud-save and platform-service implementations form separate adapter packages under the EchoSave family rather than a second competing save authority. Their technical IDs follow a pattern such as `com.echodevgames.echo-save.<provider>` after the provider and license are approved.

### 12.4 Recommended bridge naming

Examples:

```text
EchoDevGames.EchoSettings.Jukebot
EchoDevGames.EchoUI.EchoSettings
EchoDevGames.EchoUI.EchoSave
EchoDevGames.EchoGameState.EchoInput
EchoDevGames.EchoCrafting.EchoInventory
EchoDevGames.EchoCharacters.EchoControllers
EchoDevGames.EchoCharacters.EchoMultiplayer
```

The exact package-vs-assembly choice is decided in the relevant integration specification.

### 12.5 Canonical dependency and assembly direction

- Core runtime packages do not directly reference optional peer packages.
- A separate bridge declares concrete dependencies on every peer it connects; peers do not reference the bridge.
- Provider adapters declare the provider-neutral core and provider SDK/package, and remain separately removable.
- Project-specific translations remain in project-owned adapter assemblies.
- Runtime assemblies cannot reference Editor, test, sample, Workshop, project, or optional-peer assemblies.
- The public Runtime assembly may remain Auto Referenced for novice usability; Editor, tests, samples, and optional bridge/provider assemblies default to non-auto-referenced.
- Optional presentation or backend technologies are isolated from the neutral Runtime assembly when they are not central hard dependencies.
- Compile symbols and reflection cannot be used to conceal undeclared dependency truth.

### 12.6 Manifest and compatibility truth

UPM manifests declare concrete dependency versions. Tested compatibility ranges and additional verified combinations belong in documentation and the suite compatibility catalog. A claimed combination remains pending until observed in a clean project.

### 12.7 Bridge-first removal

A bridge/provider is detached and removed before either peer/core, or in the same approved package operation. The integration owns its registrations and teardown; removing it returns peers to documented standalone behavior.

### 12.8 Assembly and sample isolation

Runtime, Editor, presentation, provider, bridge, tests, samples, and platform-specific code use explicit assemblies whose references match authority direction. Standalone Labs use only the package and its hard dependencies. Integration Labs belong to the bridge/provider artifact.

### 12.9 Public API rules

- Prefer narrow interfaces and request/result objects.
- Avoid exposing internal scene objects unnecessarily.
- Return meaningful failure results instead of only logging.
- Events must unsubscribe cleanly.
- Async operations need cancellation and lifecycle policy.
- Public identifiers must be stable and serialization-safe.
- Avoid static mutable state except the documented authority access point.
- Convenience access must not prevent test injection or project adapters.
- Raise semantic events after authoritative state changes; presentation listeners must not be required for the change to complete.
- Keep generated-input wrappers, scene loaders, save participants, and service facades focused on translation/coordination rather than absorbing game-specific behavior.
- Breaking changes follow semantic versioning and include migration notes.

---

## 13. Data, Persistence, and Serialization Rules

**Canonical implementation standard:** SFGSS-003 — Data, IDs, Serialization, and Migration Standard. This section owns the suite-level boundary; SFGSS-003 defines the identifier domains, Unity GUID policy, DTO and serializer contracts, schema versions, migrations, aliases, unknown-data preservation, transactions, recovery, and removal behavior that implement it.

### 13.1 Data classification

| Data kind | Typical storage | Example |
|---|---|---|
| Immutable/shared definition | ScriptableObject | Music track, item definition, crafting recipe, character definition |
| Project configuration | ScriptableObject or project asset | Launch configuration, save configuration, UI theme |
| Mutable runtime state | Runtime class/struct | Current health, active track state, current objective count |
| Durable game state | Detached serializable DTO/payload | Inventory contents, unlocked characters, checkpoint |
| Global preference | Accord document/section DTO | Master volume, resolution, subtitles |
| Generated project record | Manifest, receipt, journal | Workshop generation history |
| Diagnostic history | Bounded immutable record | Launch report, validation results |
| Unknown optional data | Bounded opaque record | Absent settings section or save participant payload |

Shared ScriptableObjects must not be used as live mutable player/session state.

### 13.2 Identity domains

The suite distinguishes:

1. **Unity asset GUID:** project/package asset identity stored in `.meta`, primarily resolved by Editor tooling.
2. **Domain stable ID:** runtime/save/export/network-safe identity owned by a package or project.
3. **Runtime instance ID:** temporary identity for one handle, lease, voice, request, transition, or operation.
4. **Display name/path/index:** presentation or location data, never sole durable identity.

A Unity asset GUID does not automatically become a Player-runtime or save identifier. Definitions referenced by saves, exports, reports, or network messages require an explicit runtime-safe domain ID or another approved catalog/address contract.

### 13.3 Stable identity

- Stable IDs do not depend on asset names, scene hierarchy paths, localized labels, registration order, indexes, timestamps alone, or CLR type names.
- IDs are validated for emptiness, format, collision, and unsafe path characters.
- Renaming or moving content preserves its stable identity.
- Released ID changes require aliases, tombstones, or an explicit migration map.
- Alias cycles and ID reuse are prohibited.
- Duplicate copied assets do not silently regenerate IDs; Editor repair asks which identity is retained.

### 13.4 Definitions versus mutable state

ScriptableObjects and configuration assets describe rules, references, defaults, limits, and authoring data. Active indexes, cooldowns, queues, drafts, handles, current progress, selected slots, runtime references, and operation state live in authority-owned runtime models keyed by definition ID/reference.

Runtime services must not write active state back into shared assets.

### 13.5 Unity asset compatibility

- Public package/project asset identity preserves committed `.meta` files and GUIDs.
- Move/rename preserves identity; delete/recreate does not.
- Serialized field/type/enum changes follow Unity-compatible migration rules and fixtures.
- Asset GUIDs may be recorded in Editor manifests, but AssetDatabase lookup is not a Player runtime dependency.
- Direct Unity asset references and domain IDs serve different contracts and may coexist.

### 13.6 Durable document contracts

Every durable/upgradeable document declares:

- format ID;
- schema version;
- producer package/project identity and version;
- document/revision identity when meaningful;
- bounded payload/entry model;
- serializer/provider identity when replaceable;
- unsupported older/newer behavior;
- integrity/recovery policy where applicable.

Durable DTOs remain detached from live Unity objects, services, scene references, tasks, delegates, and provider SDK objects.

### 13.7 Serialization providers

A serializer documents supported DTO shapes, collections, polymorphism, unknown-field behavior, determinism, limits, threading, and error results.

Unity `JsonUtility` is acceptable for simple package-owned DTOs under Unity serialization rules. It is not assumed to preserve unknown fields, support dictionaries, or provide a universal polymorphic format. Formats requiring unknown-data round trips preserve opaque records or use an explicit extension-capable provider.

### 13.8 Migration

Every durable format declares:

- current schema version;
- supported older versions;
- contiguous forward migration path;
- migration owner;
- failure behavior;
- source preservation and backup/recovery behavior;
- whether an automatic upgrade write occurs;
- unsupported newer behavior;
- whether downgrade is supported.

Migration occurs on detached/staged data before authoritative apply/publication. Downgrade is not promised. Newer unsupported data remains preserved and unavailable/read-only.

### 13.9 Aliases and unknown data

- Aliases map old stable IDs to one current canonical ID.
- Tombstones reserve intentionally retired IDs and prevent reuse.
- Unknown optional-package settings/save/provider records are bounded, integrity-checked where possible, preserved opaquely, and never executed.
- Removing an optional package does not silently delete valid project-owned durable records.
- Reinstallation may reclaim preserved records only after identity, schema, and migration validation.

### 13.10 Transactions and publication

Operations involving multiple durable or gameplay changes validate and stage first, then publish one authoritative result where practical.

The owning package documents whether it guarantees:

- full rollback;
- compensating rollback;
- publish-last safety;
- honest partial apply;
- or an explicitly irreversible operation with backup/confirmation.

Authoritative events occur after publication. Cancellation is honored only through safe documented boundaries.

### 13.11 Integrity and recovery

Integrity hashes detect corruption; they do not prove trust unless a separate security design adds authentication. Backups, immutable generations, staging files, replacement, or provider transactions are package-specific strategies. The package must define the publication point, fallback, quarantine, and recovery order without silently overwriting the only failed/unsupported evidence.

### 13.12 Removal and replacement

Removing package code does not imply deleting project-owned configuration, preferences, saves, generated records, migration backups, or unknown optional payloads. SFGSS-002 owns bridge/provider teardown direction; SFGSS-003 owns durable-data survival and reclamation.

---

## 14. UI, Input, and Accessibility Rules

- Every interactive template must support mouse and keyboard navigation; controller support is required when the selected input configuration includes a controller.
- A screen must define its default selected element and back/cancel behavior.
- Opening a modal must not allow input to leak to gameplay.
- Loading existing settings into controls must not produce click sounds, previews, or redundant writes.
- Rebinding must provide conflict feedback and a cancellation path.
- Feedback systems must respect reduced motion, screen shake, flash, rumble, and volume preferences.
- Important information should not rely on color or audio alone.
- Packages should expose localization-friendly text references rather than hard-coded production text.
- The project decides final art direction; package templates should be reskinnable.

---

## 15. Testing and Release Standard

SFGSS-004 is the detailed suite authority for test taxonomy, evidence states, validators, Laboratories, clean-project proof, compatibility claims, defects, performance evidence, and release gates.

Every planned test begins as **Not run**. A design target is **Planned**, not **Tested** or **Supported**, until the exact environment passes the required evidence matrix. Durable results use the canonical states **Not run**, **Pass**, **Pass with advisory**, **Fail**, **Blocked**, or **Not applicable**.

Every package specification must define tests in these categories.

### 15.1 Compilation and installation

- Clean Unity project installation.
- Embedded package development.
- Local/tarball UPM installation.
- Reinstall and upgrade from the previous supported version.
- Removal when no bridge still depends on it.
- No runtime assembly reference to `UnityEditor`.

### 15.2 Lifecycle

- Start through the canonical Boot scene.
- Start a gameplay scene directly when the feature supports it.
- Scene transition with the runtime persistent.
- Duplicate root present before Play Mode.
- Duplicate root introduced during scene load.
- Domain reload/configuration appropriate to supported Editor settings.
- Application quit and cleanup.

### 15.3 Configuration failure

- Missing configuration.
- Missing prefab or scene reference.
- Invalid values.
- Empty catalogs and profiles.
- Unsupported optional integration absent.
- Corrupt or old persistence data where applicable.

### 15.4 Functional and stress tests

- Public API happy path.
- Cancellation and repeated requests.
- Maximum configured capacity.
- Pool/queue exhaustion.
- Rapid scene changes.
- Repeated menu navigation.
- Save during/near transitions according to approved policy.
- Multiplayer disconnect or authority rejection where applicable.

### 15.5 Sample verification

- Sample imports without modifying package source.
- Sample contains only redistributable content.
- Sample instructions work from a clean project.
- Sample can be removed without breaking the runtime package.
- Every Standalone Test Lab passes with no unrelated Sperk’s Forge package installed.
- Every advertised bridge has a separate Integration Lab that declares both sides explicitly.
- Showcase scenes are tested only after the standalone and bridge labs pass.
- Each controller preset passes in its own scene with only its declared input/physics dependencies.

### 15.6 Release gate

A package does not leave beta until:

- Its MVP capabilities pass automated and manual acceptance tests.
- Its clean-project and real-project integrations both work.
- Its documentation matches the shipped API.
- Known limitations are listed.
- Upgrade/migration behavior is tested.
- Licenses and credits are complete.
- Stable `.meta` files and GUIDs are preserved or intentionally migrated.
- Setup, generation, repair, and migration actions pass a repeat-run test without duplicating or overwriting project-owned content.
- Standalone Test Lab and advertised Integration Lab scenes pass from a clean project.
- The distributable tarball installs successfully.

---

## 16. Recommended Development Waves

### Wave 0 — Documentation and standards

1. Approve this Package Suite Bible.
2. Create the Package Specification template.
3. Create the Architecture Decision Record template.
4. Create the Checkpoint Build Plan and workflow-rules document.
5. Define package versioning, repository, test, and release conventions.

### Foundation Specification Pass — Documentation before runtime implementation

Before any Foundation Wave runtime package enters implementation, complete and approve one full SFGSS-001 package specification for each package listed in Section 7.1:

1. EchoLaunch — First Light.
2. EchoDiagnostics — The Observatory.
3. EchoSettings — The Accord.
4. EchoSceneFlow — The Passage.
5. EchoGameState — The Pulse.
6. Jukebot — Resonance.
7. EchoInput — The Will.
8. EchoUI — The Looking Glass.
9. EchoSave — The Chronicle.
10. EchoGameStarter — The Workshop.

After all ten specifications are approved, perform a cross-package consistency review covering authority, data ownership, lifecycle order, optional bridges, assembly direction, persistence boundaries, direct-scene behavior, Test Lab independence, diagnostics, and removal behavior. Implementation begins only after that review confirms there are no contradictory authorities or hidden hard dependencies.

Specifications may identify later questions that do not affect the MVP or neighboring package contracts. Those questions remain visibly deferred rather than blocking the documentation pass. Package implementation order remains governed by the waves below after the documentation gate is complete.

### Full Suite Documentation Pass — current implementation gate

After the Foundation documentation gate passed, the owner elected to continue the documentation-first workflow across the complete planned suite before beginning package implementation.

The active pre-code program therefore includes:

1. The remaining architecture and workflow standards in Section 18.1.
2. Every Expansion package specification in Section 18.3.
3. The Crafting design workshop and package specification.
4. The Multiplayer research plan, source-based provider comparison, provider-neutral contract design, and adapter strategy, while reserving final provider approval for disposable prototype evidence.
5. The EchoAI, EchoCombat, EchoAbilities, and EchoWorld feasibility specifications.
6. Expansion, advanced, and final full-suite authority/dependency collision reviews.
7. A final Full Suite Documentation Readiness Gate.

The Foundation specifications, matrix, ADRs, and readiness report remain approved. FL-M1-01 remains the first planned implementation checkpoint, but it is dormant and not executable until the final full-suite documentation gate passes.

Documentation readiness is honest rather than theatrical. Before code, the suite can approve authority, lifecycle, public contracts, data models, failure behavior, setup designs, test registries, research plans, migration policies, and acceptance gates. Actual compile results, screenshots, measured performance, verified package versions, migration evidence, release notes, and provider-prototype findings remain explicitly pending until the relevant implementation or research checkpoint produces evidence.


### Wave 1 — Runtime origin and dual proof packages

1. EchoLaunch.
2. EchoDiagnostics startup visualization and minimum runtime overlay.
3. Jukebot standalone audio foundation.
4. EchoUI standalone root, screen stack, and visual shell foundation.
5. Jukebot integration into Rescuers2D.
6. Jukebot integration into Don’t Get Vince’d.
7. EchoUI integration into Rescuers2D without making Jukebot or EchoLaunch mandatory.

Jukebot and EchoUI proceed side by side after the smallest viable First Light foundation. Each must pass its own Standalone Test Lab before either is connected to the other. This proves both a nonvisual runtime authority and a visual framework can operate alone, integrate through First Light when selected, and replace project-specific infrastructure.

### Wave 2 — Shared application shell

1. EchoSettings.
2. EchoSceneFlow.
3. EchoGameState.
4. EchoInput.
5. EchoSave.
6. EchoProgression.

This wave produces the reusable startup/menu/settings/save shell needed by most projects.

### Wave 3 — Game Jam composition

1. EchoGameStarter Blank Modular Starter.
2. EchoGameStarter Game Jam Quickstart.
3. EchoBuildTools.
4. EchoFeedback.
5. EchoPool.
6. Integration and readiness reports.

This wave turns individual packages into a fast, guided project-starting workflow.

### Wave 4 — General gameplay modules

1. EchoInteraction.
2. EchoCharacters.
3. EchoControllers, beginning with side-view 2D and top-down 2D labs.
4. EchoInventory.
5. EchoObjectives.
6. EchoDialogue.
7. EchoCamera.
8. EchoLocalization.

### Wave 5 — Deferred design and advanced research

1. Conduct the dedicated EchoCrafting design workshop and approve its full package specification before implementation.
2. Produce the dated EchoMultiplayer provider research matrix.
3. Build disposable networking comparison prototypes.
4. Approve a provider strategy only after the prototypes and research are reviewed.
5. Build provider-neutral multiplayer contracts and one approved provider adapter.
6. Add EchoCharacters, EchoSceneFlow, EchoGameState, and EchoUI multiplayer bridges.
7. Evaluate EchoAI, EchoCombat, EchoAbilities, and EchoWorld specifications.

Crafting remains part of the intended suite, but it is intentionally excluded from the general-gameplay implementation wave because its eventual importance deserves deeper design first. Multiplayer likewise begins with research rather than production code. Waves describe dependency, priority, and learning order—not a promise to finish every listed package before using completed packages in games.

---

## 17. Relationship to Existing Projects

### 17.1 Rescuers2D

Rescuers2D is the primary source of practical lessons for:

- Bootstrap conflict prevention.
- Persistent audio.
- Main/Pause/Win menu consistency.
- Shared settings.
- Password-based progression.
- Switchable characters.
- Side-view controller capabilities such as walking, jumping, crawling, swimming, climbing, ladders, and role-specific control handoff.
- Character and environmental audio profiles.
- Direct-scene testing.
- Destructible and character interaction feedback.

Its current scripts are reference implementations and requirements evidence. They should not be copied wholesale into clean packages without dependency review.

### 17.2 Don’t Get Vince’d

This project is a strong second integration target for Jukebot and later EchoFeedback, EchoPool, EchoCombat, EchoCharacters, and a beat-’em-up controller preset or project adapter. Successful integration will demonstrate that the packages are not secretly shaped only for Rescuers2D.

### 17.3 Echo Systems Lab

Echo Systems Lab supplies workflow precedent, especially the Checkpoint Build Plan format, modular systems presentation, safe implementation checkpoints, and portfolio case-study expectations.

### 17.4 Hackulos: The Children of the Sperk

Hackulos is a future consumer of:

- EchoLaunch and the application shell.
- Jukebot.
- EchoCharacters.
- EchoControllers top-down 2D or click-to-move presets.
- EchoInventory.
- EchoCrafting, beginning with an exact quest-combine bag.
- EchoObjectives and EchoDialogue.
- EchoAbilities and optional RPG integrations.

Hackulos-specific ancestries, classes, faiths, items, spells, monsters, and other RPG definitions belong to the separate runtime-safe `EchoRPG.Foundation` family. The general suite must remain usable by non-RPG games.

### 17.5 DeverQuest

DeverQuest remains an Editor-focused productivity and authoring product. Its guild identity, timers, compensation, Chronicle, and other editor-only systems do not become runtime game dependencies. It may author or generate compatible neutral data where explicitly designed.

### 17.6 Repository alignment audit — August 3, 2026

Before approving this bible as the lead baseline, the public EchoDevGames repositories and portfolio documentation were reviewed, with Echo Systems Lab used as the primary runtime-architecture reference and DeverQuest used as the primary reusable-package and documentation reference.

Reviewed sources:

- `https://github.com/echodevgames/Echo-Systems-Lab`
- `https://github.com/echodevgames/DeverQuest`
- `https://echodevgames.github.io/JesseAdams_Portfolio/systems/echo-systems-lab-overview.html`
- `https://echodevgames.github.io/JesseAdams_Portfolio/systems/audio-subsystem.html`
- `https://echodevgames.github.io/JesseAdams_Portfolio/systems/target-range-mission-framework.html`

#### Established practices preserved by the suite

| Existing practice | Evidence in current work | Suite standard |
|---|---|---|
| Data-driven authoring | Mission, weapon, audio, music, ambience, UI, footstep, and other ScriptableObject assets | Definitions/configuration remain separate from changing runtime state. |
| Focused runtime components | Mission controllers, target groups, input readers, interactables, audio requesters, playback managers, and UI listeners have distinct jobs | Package code uses narrow authorities and explicit collaborators. |
| Event-driven feedback | Mission state, weapon events, HUD updates, and audio request flows | Runtime authorities raise semantic events; UI/audio/VFX remain consumers. |
| Stable identifiers | Mission completion and unlocks are stored by mission IDs | All save/network-relevant definitions require stable validated IDs and migration. |
| Centralized input translation | Generated Input System actions are wrapped by a player input reader | EchoInput translates devices/actions and context; it does not own movement or combat. |
| Persistent systems | Save, scene, music, and audio services persist across scenes | Persistence is retained, but subordinate services are owned by one duplicate-safe package root. |
| Feature/domain organization | Echo Systems Lab groups scripts and assets into Audio, Combat, Input, Missions, Save, SceneManagement, UI, Weapons, and related domains | Packages and project integrations use clear feature folders inside assembly/package boundaries. |
| Integrated design documentation | Echo Systems Lab carries an Obsidian vault and portfolio system pages | Suite docs remain Markdown-first and handoff-ready; package user docs ship in `Documentation~`. |
| Product-grade package anatomy | DeverQuest separates `Runtime`, `Editor`, `Documentation~`, manifests, changelogs, credits, and third-party notices | Every reusable suite package follows the same mature delivery shape. |
| Safe setup and repair | DeverQuest documents setup, validation, repair, migration, backups, readiness, and repeatable generation | Authoring tools must be non-destructive, idempotent where possible, and explicit about changes. |
| Checkpoint workflow | Echo Systems Lab planning uses scoped checkpoints, testing, commits, devlogs, and portfolio capture | SFGSS-005 formalizes this as the suite implementation workflow. |

#### Historical shortcuts the suite must improve

| Current-project shortcut | Package-suite correction |
|---|---|
| Several independent `DontDestroyOnLoad` singletons | One duplicate-safe persistent root per installed package concern; ordinary owned child services. |
| Hard-coded scene names such as hub/menu destinations | Project-owned scene configuration and validated references. |
| Fixed save filename and direct knowledge of weapon/mission/player stores | Configurable storage policy plus versioned save-participant contracts. |
| Synchronous scene transitions as the only path | Async-first transition lifecycle with progress, locking, cancellation/failure policy, and presentation hooks. |
| Mutable selection/cooldown state inside audio definition assets | Runtime playback state keyed by immutable cue reference or stable ID. |
| Static mutable progression stores | Explicit runtime service/state instance with reset, load, save, and change-event lifecycle. |
| Global namespace and project assemblies | Per-package namespaces and assembly definitions for UPM isolation. |
| Minimal root README in a systems project | DeverQuest-style routed documentation, known limitations, troubleshooting, and release readiness. |

#### Alignment conclusion

The suite architecture is aligned with the common direction of the existing work. It does not discard the Echo Systems Lab approach; it formalizes that approach for reuse and deliberately removes the project-specific coupling that would make a package fragile. No package should copy an existing manager solely because it already works in one game. Existing code supplies requirements, naming evidence, failure cases, and migration targets; the approved package specifications determine the clean implementation.

---

## 18. Documentation Suite to Produce

After this lead bible is reviewed, create the following documents.

### 18.1 Architecture and workflow documents

1. **SFGSS-001 — Package Specification Template**
2. **SFGSS-002 — Dependency, Bridge, and Assembly Standard**
3. **SFGSS-003 — Data, IDs, Serialization, and Migration Standard**
4. **SFGSS-004 — Testing, Validation, Test Labs, and Release Standard**
5. **SFGSS-005 — Checkpoint Build Workflow and ChatGPT Collaboration Rules**
6. **SFGSS-006 — New-Project Guided Pathways**
7. **SFGSS-007 — Architecture Decision Record Log**
8. **SFGSS-008 — Suite Glossary and Naming Registry**
9. **SFGSS-009 — Repository, Versioning, and Integration Workspace Standard**
10. **SFGSS-010 — Living Documentation, Current Notes, and Obsidian Workflow Standard**

**Current documentation status:** SFGSS-001, SFGSS-002, SFGSS-003, and SFGSS-005 are approved. SFGSS-ADR-001, SFGSS-ADR-002, and SFGSS-INT-FOUNDATION-001 are accepted/approved. The complete Foundation specification set is approved. The remaining architecture/workflow standards, Expansion specifications, Advanced design/research records, and final full-suite collision/readiness gates are required before implementation begins.

### 18.2 Foundation package specifications

1. EchoLaunch.
2. EchoDiagnostics.
3. Jukebot.
4. EchoSettings.
5. EchoSceneFlow.
6. EchoGameState.
7. EchoInput.
8. EchoUI.
9. EchoSave.
10. EchoGameStarter.

### 18.3 Expansion package specifications

1. EchoProgression.
2. EchoBuildTools.
3. EchoFeedback.
4. EchoPool.
5. EchoInteraction.
6. EchoCharacters.
7. EchoControllers plus controller-preset specification template.
8. EchoInventory.
9. EchoObjectives.
10. EchoDialogue.
11. EchoCamera.
12. EchoLocalization.

### 18.4 Advanced package specifications

1. EchoCrafting design-workshop record and full package specification.
2. EchoMultiplayer research plan, findings, and provider-adapter strategy.
3. EchoAI feasibility specification.
4. EchoCombat feasibility specification.
5. EchoAbilities feasibility specification.
6. EchoWorld feasibility specification.

---

## 19. Guided Workflow Principles

The detailed implementation-checkpoint workflow is approved in SFGSS-005. New-project and existing-project composition guidance is approved in SFGSS-006. The following principles remain authoritative.

### 19.1 Design before implementation

For a new package:

1. Approve its responsibility and non-goals.
2. Identify consumers and optional integrations.
3. Define runtime authority and lifecycle.
4. Define configuration and mutable state separately.
5. Define the smallest complete MVP.
6. Define tests and clean-project acceptance gates.
7. Only then create scripts and assets.

### 19.2 Checkpoint Build Plan structure

Each implementation checkpoint should state:

- Purpose and user-visible outcome.
- Starting conditions.
- Scope and explicit exclusions.
- Files to create or modify.
- Complete, visible, compile-ready scripts for that checkpoint, followed by file-by-file and step-by-step explanations so Jesse can implement and understand the work himself.
- Unity Editor setup steps in exact order.
- Validation tests and expected results.
- Common failure symptoms and fixes.
- Checkpoint completion criteria.
- Safe rollback or recovery notes.
- The next recommended checkpoint.

### 19.3 One verified vertical slice before expansion

Each package should prove one complete use path before adding a large option catalog. Examples:

- Jukebot: one music track, one crossfade, one SFX cue, one scene transition.
- EchoSave: create slot, save, quit, reload, migrate one version.
- EchoCrafting: validate ingredients, consume transactionally, grant output, save result.
- EchoCharacters: register roster, select, spawn, switch control, save selection.
- EchoControllers: accept normalized intent, move, ground/fall, exercise the preset’s core capability, and publish state in that preset’s isolated lab scene.
- EchoMultiplayer: host, join, select character, synchronized scene load, disconnect cleanly.

The vertical slice must pass alone before an integration scene is allowed to count toward the checkpoint. A combined scene is evidence of composition, not evidence that either package is independently sound.

### 19.4 Preserve working projects during replacement

When replacing a project-specific system:

1. Keep the original system available.
2. Install and validate the clean package in isolation.
3. Connect one feature category at a time.
4. Confirm parity.
5. Remove the old implementation only after the replacement passes the real-project checklist.

### 19.5 Repository-first documentation workflow

The documentation and implementation evolve together in the repository.

Before beginning or resuming work, review in this order:

1. Repository documentation index or README.
2. SFGSS-000 when suite boundaries are relevant.
3. The active package specification.
4. Applicable ADRs and integration specifications.
5. `Current Notes.md`.
6. The current checkpoint, test results, issue log, and changelog.
7. The implementation and tests affected by the request.

During work:

- Record discoveries and provisional thoughts in `Current Notes.md` as they occur.
- Keep proposed decisions visibly labeled as proposed until approved.
- Update the active checkpoint status when implementation state changes.
- Update user-facing and developer-facing documentation when behavior, setup, diagnostics, dependencies, or limitations change.
- Use lightweight links between notes, specifications, ADRs, tests, and relevant source locations so the repository can be scanned efficiently in GitHub or navigated as an Obsidian vault.

At checkpoint closeout:

1. Reconcile every material current note.
2. Promote durable decisions into the correct authoritative document.
3. Record unresolved blockers and the next checkpoint.
4. Verify that documentation describes the code and tests that are actually committed.
5. Commit and push the documentation update with the checkpoint.

This rule makes a repository scan the default way to rebuild context. Chat history may help, but it must not be the only place where a technical decision survives.

### 19.6 Guided composition pathways

SFGSS-006 is the canonical standard for selecting and staging packages in new and existing projects.

- A pathway is an approved composition guide, not a hidden bundle or runtime dependency.
- Every pathway separates minimum, recommended, optional, and explicitly excluded selections.
- Every selected package, bridge, provider, adapter, scene, persistence choice, and generated asset remains visible in the plan.
- Pathways begin with one bounded vertical slice and add later systems only after the current stage is proven.
- The Workshop may implement a pathway as a versioned preset, but manual composition remains supported and package-owned setup facades remain authoritative.
- Provider-backed and Advanced pathways preserve research, license, cost, compatibility, security, and evidence uncertainty until executed proof and an ADR support a stronger claim.
- Existing projects adopt packages incrementally and preserve working systems until replacement parity is verified.

---

## 20. New ChatGPT Conversation Handoff Protocol

This documentation suite is designed to let work resume without relying on old chat history.

### 20.1 Documents to provide

At minimum, attach or paste:

1. This Package Suite Bible.
2. The specification for the package currently being built.
3. The Checkpoint Build Workflow and ChatGPT Collaboration Rules.
4. The repository's `Current Notes.md` page.
5. The current checkpoint/status record.
6. Any relevant Architecture Decision Records.
7. Current scripts or project files required for the immediate task.

Do not load every package specification when only one package is in scope. Supply the lead bible plus the active package and integration documents.

### 20.2 Recommended opening prompt

```text
We are continuing development of The Sperk’s Forge — EchoDevGames Game Systems Suite.

Treat the attached Sperk’s Forge Game Systems Suite Bible as the lead source of truth for
package boundaries, terminology, dependency policy, and suite-wide architecture.
Treat the attached <PACKAGE> Specification as the source of truth for that
package's behavior and public surface. Follow the attached Checkpoint Build
Workflow for implementation steps.

Current package: <PACKAGE>
Current milestone/checkpoint: <CHECKPOINT>
Current Unity version: <VERSION>
Current project/repository: <PROJECT>
Current implementation status: <STATUS>
Known blockers: <BLOCKERS>
Current Notes reviewed through: <DATE/COMMIT>

Before writing code:
1. Summarize the relevant architectural constraints.
2. Identify any conflict or missing decision that would materially change the work.
3. Keep the package standalone and place optional integrations behind bridges.
4. Preserve existing working project code until replacement parity is verified.
5. Then continue using the Checkpoint Build Plan format.
```

### 20.3 Context update discipline

At the end of a meaningful checkpoint, update a short status record containing:

- Package and version.
- Completed checkpoint.
- Files/assets created.
- Tests passed and failed.
- Current known issues.
- Architectural decisions made.
- Next checkpoint.

This status record is more useful to a fresh conversation than a raw transcript.

Working discoveries should first be captured in `Current Notes.md`. Before the handoff is considered complete, reconcile those notes into the status record, specification, ADR, issue log, changelog, or guide that owns the durable information. A new conversation should be able to scan the repository documentation and distinguish approved truth, current implementation state, open questions, and historical Git changes without reconstructing them from chat.

---

## 21. Approved Architecture Decisions

The following decisions form the approved starting baseline for the suite:

1. The suite’s formal public name is **The Sperk’s Forge — EchoDevGames Game Systems Suite**, shortened to **The Sperk’s Forge** or **Sperk’s Forge**.
2. Hackulos and the Sperk influence public package titles, icons, documentation headings, setup guidance, tooltips, optional samples, and visual continuity; they are not runtime or game-lore dependencies.
3. The suite is Jesse “Echo” Adams’s independent EchoDevGames project. It is not owned, published, branded, or technically identified as an Isekai Studios product.
4. Public package titles lead with distinctive identities such as **First Light**, **The Observatory**, **The Chronicle**, and **The Vessel** rather than displaying `Echo` on every product. Technical identifiers remain plain and understandable.
5. `EchoLaunch` replaces the separate `EchoBootstrap` concept; its public package title is **First Light — Startup and Launch**.
6. First Light coordinates startup but does not own every service.
7. First Light always emits structured launch diagnostics; the standalone Observatory package owns the richer ongoing runtime overlay and package-health dashboard.
8. The Workshop may install First Light and the Observatory together as the recommended default, but both remain visible, removable selections.
9. `EchoGameStarter` is an Editor-time composer/generator, not a runtime manager; its public title is **The Workshop — Project Starter**.
10. Every runtime package must work independently unless explicitly classified as a bridge, provider adapter, or composer.
11. Every package and independently selectable feature receives its own Standalone Test Lab scene where scene-visible behavior exists. Integration and showcase scenes are separate evidence.
12. Optional package connections use documented bridges or project adapters.
13. No mandatory `EchoCore` package will be created at the beginning.
14. Project-specific data remains outside package source.
15. Jukebot and EchoUI are the two first major standalone proof packages after the minimum First Light/Observatory foundation; neither depends on the other.
16. `EchoCharacters` owns identity, roster, selection, spawning, switching, and control ownership—not movement or combat.
17. `EchoControllers` is introduced as the standalone player-controller library, with modular genre presets and an isolated scene for every controller family.
18. `EchoCrafting` remains in the intended suite but is deliberately low priority. No implementation begins until a dedicated design workshop and full package specification are approved.
19. `EchoMultiplayer` is a research/incubation package that uses provider-neutral contracts and approved adapters; it does not replace established networking stacks or begin production implementation before provider comparison prototypes.
20. `EchoRPG.Foundation` remains an optional genre-specific family outside the general suite core.
21. The suite uses individual package specifications and a checkpoint-driven implementation workflow.
22. Definition/configuration, runtime state, and presentation/feedback remain separate layers.
23. Persistent subsystem children are owned by one duplicate-safe package root rather than becoming independent persistent singletons.
24. Save integration uses versioned participants/contributors instead of direct knowledge of project-specific databases and static stores.
25. Setup, generation, migration, and repair tooling must be repeatable, non-destructive by default, and explicit about changes.
26. Echo Systems Lab is the runtime-architecture precedent; DeverQuest is the package-delivery and documentation precedent. Historical shortcuts in either project are evidence to improve, not requirements to preserve.
27. Repository development uses a hybrid multi-repository model: one public repository per major distributable package, a central Sperk’s Forge catalog/documentation repository, and a separate integration workspace.
28. Bridges use a mixed packaging rule: tiny compile-safe integrations may ship with an owner; two-package bridges and provider adapters ship separately when needed for clean install/removal and versioning.
29. EchoSave owns local save authority and provider-neutral synchronization seams; cloud/platform implementations ship as separate EchoSave-family adapter packages.
30. EchoInventory may own generic equipment slots and equip transactions; RPG statistics, restrictions, bonuses, and combat effects remain in project code or `EchoRPG.Foundation`.
31. Suite documentation lives with development in Git and is opened directly as an Obsidian-compatible Markdown vault or folder. Every active repository maintains a linked `Current Notes.md` capture page; meaningful notes are reconciled into the proper authoritative documents at checkpoints, and major documentation changes are committed alongside or immediately adjacent to the code they describe.
32. The complete Foundation Wave specification set must be drafted, reviewed, and approved before any Foundation Wave runtime implementation begins. A cross-package consistency review is the final documentation gate.
33. The initial public Unity floor for Foundation packages is Unity 6000.0. Unity 6000.3.8f1 remains the primary development baseline; additional Unity 6 versions are listed as tested only after validation.
34. Foundation diagnostic code namespaces must be globally unique. EchoGameState uses `EGSTATE-*`; EchoGameStarter retains `EGS-*`. Cross-package reports qualify package-local use-case, capability, Lab, and operation identifiers with the package ID.
35. Foundation runtime bridges are optional translation artifacts. A separate two-package bridge declares compatible dependencies on both peers, is removed before either peer, owns no competing authority, and leaves both cores functional when absent.
36. Development initializers create only their own minimum missing authority. First Light startup bridges adopt an existing valid peer root and never instantiate a second authority blindly.
37. The Workshop invokes package-specific setup only through exact, allowlisted, package-owned Editor facades governed by SFGSS-ADR-001. The protocol adds no runtime dependency and no mandatory shared core package; a missing facade produces a visible manual setup path.
38. The ten Foundation package specifications and SFGSS-INT-FOUNDATION-001 pass the authority, lifecycle, dependency, settings/save, diagnostics, Test Lab, and removal collision review. FW-DOC-12 remains the final authorization gate before implementation.
39. SFGSS-005 is the approved workflow authority for implementation checkpoints. A Checkpoint Build Plan authorizes only its named outcome, files, tests, and Editor work; it cannot overrule a package specification, ADR, integration specification, or SFGSS-000.
40. FW-DOC-12 passes the Foundation Documentation Readiness Gate. First Light FL-M1-01 Package Skeleton is the first authorized implementation checkpoint. The gate authorizes no C# runtime behavior, scene, prefab, ScriptableObject, setup tool, sample, bridge, or peer-package implementation.
41. The owner supersedes the immediate activation of FL-M1-01 and re-locks package implementation until the complete pre-code documentation program in Section 18 passes a final Full Suite Documentation Readiness Gate. Foundation approval remains valid; FL-M1-01 is queued rather than cancelled.
42. Documentation readiness distinguishes planned truth from observed evidence. Architecture, specifications, contracts, research plans, acceptance tests, setup designs, and migration policies may be completed before code. Compile results, screenshots, measured performance, verified compatibility, release notes, and prototype-dependent findings must remain marked pending until evidence exists.
43. Implementation is learning-oriented and user-driven. When code checkpoints begin, ChatGPT must show complete compile-ready files in the conversation, explain the purpose and architecture of each file, walk through important sections and Unity lifecycle behavior, provide exact Editor setup, and state how each test proves the concept. Jesse implements the code himself by default unless he explicitly requests generated files or direct editing.

44. SFGSS-002 is the approved Dependency, Bridge, and Assembly Standard. It governs package manifests, assembly reference direction, optional integrations, provider adapters, compile guards, test/sample assemblies, compatibility records, and clean removal.
45. Core runtime packages do not directly depend on optional peer Echo packages. Separate bridges depend on every peer they connect; peers never reference the bridge.
46. A UPM package manifest declares concrete required dependency versions. Broader compatibility claims live in documentation/catalog records and require evidence.
47. Runtime assemblies cannot reference Editor, test, sample, Workshop, project, or optional-peer assemblies. Optional presentation, backend, platform, and provider dependencies are isolated when they are not central hard dependencies.
48. The primary public Runtime assembly may remain Auto Referenced for novice usability. Editor, test, sample, and optional bridge/provider assemblies default to non-auto-referenced unless their specification documents a public project-code use case.
49. Compile guards, version defines, Assembly Definition References, or reflection must not hide undeclared package/SDK dependencies or replace a bridge that should declare both peers. Reflection is limited to exact versioned allowlists such as the ADR-001 Editor setup facade protocol.
50. Standalone Labs use only the package and its declared hard dependencies. Integration Labs belong to the bridge/provider artifact and never substitute for either peer’s standalone proof.
51. Optional integrations follow bridge-first teardown and removal. The integration owns every registration, lease, subscription, and adapter resource it creates, and removal returns peers/neutral cores to documented standalone behavior.
52. SFGSS-003 is the approved Data, IDs, Serialization, and Migration Standard. It governs data classification, identifier domains, Unity asset identity, definition/runtime separation, DTOs, serializer providers, versions, migrations, aliases, unknown data, transactions, recovery, and durable-data survival.
53. Unity asset GUIDs, domain stable IDs, and runtime instance IDs are distinct contracts. Editor AssetDatabase identity must not be used implicitly as a Player runtime, save, export, or network identity.
54. Shared ScriptableObjects and configuration assets remain immutable runtime inputs. Changing state lives in authority-owned runtime models or detached durable DTOs.
55. Durable documents declare a stable format ID and schema version independently from package SemVer. Durable DTOs contain no live Unity object graph, service, scene object, provider object, task, delegate, or runtime handle.
56. Serializer providers document supported data shapes, limits, unknown-field behavior, determinism, and failure behavior. Unity JsonUtility is approved for simple DTOs only and does not by itself satisfy unknown-field round-trip preservation.
57. Supported migrations are explicit forward steps on staged data, preserve the source until verified publication, report every conversion, and do not promise downgrade. Unsupported newer data remains preserved and unavailable/read-only.
58. Released identity changes use validated aliases or tombstones. Alias cycles, ambiguous mappings, and reuse of retired IDs are prohibited.
59. Unknown optional settings sections, save participant payloads, provider records, and generated receipts are bounded, preserved opaquely where required, never executed, and reclaimed only after the owner returns with a compatible schema.
60. Data-changing operations validate and stage before publication. Each package states its real rollback class and must not describe partial application as atomic.
61. Removing package or bridge code does not authorize deletion of project-owned configuration, preferences, saves, generated records, or migration evidence. Reinstallation validates and migrates preserved data before reclaiming it.
62. SFGSS-004 is the approved Testing, Validation, Test Labs, and Release Standard. It governs evidence states, compatibility language, test registries, validators, Laboratories, installation routes, migration proof, defects, performance, platform evidence, and release gates.
63. Planned tests and approved acceptance criteria are not execution evidence. Every pre-code test remains Not run until an exact environment produces retained evidence.
64. Durable test executions use the canonical states Not run, Pass, Pass with advisory, Fail, Blocked, or Not applicable. Framework skips and retries must be translated honestly and do not erase failures.
65. Public compatibility claims use Unknown, Planned, Tested, Supported, Experimental, or Unsupported. A claim names the exact Unity, dependency, platform, build, device, or provider dimensions that its evidence covers.
66. Standalone Laboratories prove one package with only declared hard dependencies. Integration Laboratories belong to bridge/provider artifacts. Showcases demonstrate composition only after standalone and integration evidence passes.
67. Clean-project proof includes the claimed installation route plus the smallest functional workflow. Compilation alone does not satisfy package independence or release readiness.
68. Setup, generation, repair, migration, removal, and reinstall tools must pass dry-run/apply/report/repeat/conflict/interruption/recovery evidence appropriate to their real rollback guarantees.
69. Defect severity is independent from schedule priority. Blocker, Critical, Major, Minor, and Advisory findings have explicit release effects; flaky or quarantined required tests cannot silently count as passes.
70. Beta, release-candidate, and stable gates require progressively stronger clean-install, Laboratory, compatibility, performance, migration, removal, documentation, license, and issue evidence. No percentage score overrides a failed mandatory gate.
71. Release reports preserve traceability from requirement to test case, execution, evidence, issue, fix, regression, and gate decision. Compatibility and measured-performance claims remain pending until observed.

72. The thirteen Expansion package specifications pass SFGSS-INT-EXPANSION-001 after the SUITE-DOC-23 authority, lifecycle, dependency, persistence, transaction, diagnostics, Test Lab, setup-facade, and removal review.
73. EchoProgression completion records apply only to registered progression definitions such as stages, modes, challenges, and comparable access nodes. EchoObjectives remains the sole authority for objective-run and step completion; integrations translate outcomes through idempotent semantic requests rather than mirroring one record in both packages.
74. Cross-package operations have one commit owner. Bridges may validate, orchestrate, ledger, retry, or observe, but they must not claim distributed atomic rollback after a foreign authority reports commitment.
75. One peer pair may have only one reusable bridge artifact for the same integration behavior. The integration specification names the canonical behavioral owner; mirror bridges are prohibited.
76. Input users/devices, Fellowship `ControlOwnerId` assignments, Vessel actor-local control leases, character identities, camera targets, and network participants are distinct identities and must not be collapsed into one unqualified player ID.
77. UI focus, interaction focus, camera targets/shots, selected characters, tracked objectives, and other selection concepts remain qualified package-local truths. Public APIs and diagnostics must not imply one global focus authority.
78. SFGSS-ADR-001 v1.1.0 extends the exact Workshop setup-facade registry and minimum planning domains through all thirteen Expansion packages while preserving manual setup and package independence.


79. The five Advanced package foundations pass SFGSS-INT-ADVANCED-001 after the SUITE-DOC-24 authority, identity, lifecycle, dependency, provider, persistence, transaction, diagnostics, Laboratory, research, and removal review.
80. Multiplayer semantic travel follows one ordered authority chain: Atlas prepares the semantic plan; Convergence/provider coordinates authority and readiness; Passage executes the Unity scene transition; Atlas commits semantic context only after approved success; Atlas selects arrival metadata; Fellowship or project code spawns or relocates actors.
81. Session participant, provider network entity, durable character, runtime actor, control owner, input user, AI agent, ability owner, combat target, world location, scene binding, and marker identities remain qualified and separate. Bridges store explicit mappings rather than collapsing them into one generic player, actor, target, or location ID.
82. Instinct owns perception, scoring, scheduling, and semantic AI choice. Arcana owns ability activation and effect orchestration. Clash owns instantaneous combat resolution and target-receiver transaction coordination. A decision request, ability commit, and combat-resource commit are separate authority events.
83. Clash combat relation and targetability may be consumed read-only by Instinct and Arcana. AI scoring and ability targeting must not create competing team, faction, targetability, damage, healing, or defeat truth.
84. Arcana permits one mutation-capable cost provider per MVP activation. Cross-system atomicity requires a real coordinating transaction owner; configurations that promise rollback across independent foreign authorities are rejected.
85. Clash defeat outcomes do not directly mutate Fellowship availability, Path objectives, Ascent progression, Vault loot, Atlas world state, or respawn behavior. Those authorities receive separate idempotent requests or observe committed events.
86. Atlas semantic travel routes, Instinct local navigation paths, Passage scene transitions, and Vessel actor movement commands are distinct contracts and cannot substitute for one another.
87. Shared multiplayer save publication belongs to The Chronicle on the authoritative host/server. Advanced packages contribute only their versioned payloads. Live sessions, AI observations, combat requests, active abilities, markers, and prepared travel operations remain session-only unless the package explicitly defines a safe detached snapshot.
88. Advanced cores remain provider-neutral. Networking, navigation, behavior, inference, hit, world, hosting, and platform providers require separate adapters, explicit dependencies, Integration Laboratories, and retained compatibility evidence.
89. SFGSS-ADR-001 v1.2.0 extends the exact Workshop setup-facade registry and minimum planning domains through all five Advanced package foundations. Provider installation remains an explicit selected operation and is never inferred from the neutral core.
90. No networking or AI provider, topology, hosting service, prediction model, navigation backend, behavior graph, inference engine, hit provider, status-effect framework, scene-streaming backend, or large-world strategy is approved by documentation alone. Candidate and research language remains visible until executed evidence supports a stronger claim.
91. SFGSS-006 is the canonical guided-composition standard. A pathway recommends a visible staged package selection and never creates a hidden bundle, implicit runtime dependency, or silent authority transfer.
92. Every pathway distinguishes minimum, recommended, optional, and explicitly excluded selections; names its first vertical slice, persistence choice, bridges/providers, project-owned work, evidence path, and removal story.
93. Package selection begins from the authority the project needs. Recommended packages remain removable unless an approved artifact declares a real dependency, and bridges are selected only for named cross-authority behavior.
94. The Workshop may implement approved pathways as versioned presets only through immutable dry-run plans and exact package-owned setup facades. Manual composition remains supported, and material plan drift requires reapproval.
95. Advanced and provider-backed pathways remain research or experimental until provider, license, cost, hosting, platform, compatibility, security, performance, migration, and Laboratory evidence supports a stronger claim.
96. Existing-project pathways preserve working systems until standalone proof, project integration, parity, rollback, migration, and removal evidence passes for the replacement.

97. SFGSS-007 is the canonical Architecture Decision Record standard and decision register for the suite. It defines ADR requirement tests, scopes, identifiers, metadata, statuses, evidence maturity, revision, supersession, indexing, approval, and graph links.
98. An ADR preserves reasoning and consequences but never silently overrides SFGSS-000 or an approved package specification. Any accepted decision that changes higher-authority truth updates that authority in the same checkpoint.
99. Suite ADR IDs use the permanent `SFGSS-ADR-###` sequence. Package-local and integration ADRs use scoped sequences, remain owned by their repositories, and are linked into the central decision log when they affect suite compatibility or pathways.
100. ADR status and evidence maturity are separate. An Accepted ADR may remain design-approved with runtime, compatibility, performance, migration, or provider evidence still `Not run`.
101. Rejected, withdrawn, and superseded ADR IDs are never reused or deleted. Substantive reversals create a new superseding ADR; compatible clarification or registry expansion may revise the existing ADR.
102. Jesse “Echo” Adams / EchoDevGames is the approval authority for suite ADRs. ChatGPT and collaborators may research, draft, compare, and recommend but do not silently approve architectural decisions.
103. The central suite decision register currently contains SFGSS-ADR-001 through SFGSS-ADR-003. The next available suite ADR is SFGSS-ADR-004; candidate decisions receive no ID until they enter Proposed review.
104. Every ADR defines concrete review triggers, affected authorities, implementation/migration impact, evidence plan, removal/reversal behavior, and Obsidian graph links.
105. SFGSS-008 is the canonical Suite Glossary and Naming Registry. It governs suite/package identity layers, terminology, technical identifiers, package IDs, namespaces, diagnostic/test/Laboratory prefixes, reserved names, aliases, deprecations, and naming validation.
106. The twenty-eight approved package foundations have one registered technical identifier, public short title, formal title, plain responsibility, package ID, namespace family, document ID, diagnostic prefix, test/Laboratory prefix, and Workshop setup-facade identity.
107. Formal public package titles use a spaced en dash between the short title and plain responsibility. ASCII-only surfaces may use a spaced hyphen. Older punctuation variants are typography aliases, not separate products.
108. Runtime and Editor APIs remain technically neutral. Verse flavor may name public products and optional presentation surfaces but must not enter package IDs, namespaces, assemblies, durable technical IDs, or mandatory game content. Jukebot remains the intentional technical-name exception.
109. Public type suffixes carry stable suite-wide meanings, including Root, Service, Definition, Configuration, State, Snapshot, Request, Result, Plan, Receipt, Handle, Lease, Registration, Provider, Adapter, Bridge, Presenter, and Coordinator.
110. Ambiguous cross-package terms such as Player, Target, State, Profile, Participant, Focus, Selection, Owner, Active, and Current must be qualified by their owning domain in public APIs and durable reports.
111. Package, document, diagnostic, test, Laboratory, bridge, and provider identifiers are permanent registry entries and are never silently recycled. Renames and aliases follow SFGSS-003 and the approving package specification or ADR.
112. `EchoCore`, `EchoFoundation`, `EchoBootstrap`, generic public `GameManager` authorities, Isekai-branded technical identities, and Hackulos/Sperk technical package dependencies remain prohibited or historical unless a later accepted ADR changes the boundary.

113. SFGSS-009 is the canonical Repository, Versioning, and Integration Workspace Standard. It governs central, package, bridge, provider, and Integration Lab repositories; branches, commits, SemVer, tags, releases, Git/local/tarball/registry sources, lock files, compatibility snapshots, support lines, secrets, large files, CI design, and archival.
114. Each independently releasable package, bridge, or provider artifact owns one repository and one independent version history. Package repositories contain one UPM package at repository root by default; the central suite repository catalogs releases and never becomes a runtime dependency.
115. Package, bridge, and provider releases use immutable annotated `vMAJOR.MINOR.PATCH[-PRERELEASE]` tags whose version matches `package.json`. Released tags are never moved or reused; corrections create a new version.
116. Consumer projects and compatibility snapshots pin registry versions, release tags, tarballs, or exact commits and commit both `Packages/manifest.json` and `Packages/packages-lock.json`. Mutable default branches and machine-local paths do not support release or compatibility claims.
117. Unity Git dependencies are project-level only; package manifests cannot declare Git URLs for transitive Git peers. During Git-only incubation, bridges/providers and The Workshop visibly install every required peer at the project level until an approved registry provides transitive version resolution.
118. The Integration Lab is the authority for cross-package compatibility evidence, exact revision combinations, pathway fixtures, bridges/providers, upgrades, and clean removal. Package repositories remain the authority for standalone proof.
119. The preferred local workspace uses independent sibling clones and portable relative paths. Submodules and worktrees are optional reproducibility/maintenance tools and are never consumer requirements.
120. Package versions are independent. Coordinated releases tag each artifact in its own repository and publish one compatibility snapshot; no global suite runtime version is implied.
121. SFGSS-010 is the canonical Living Documentation, Current Notes, and Obsidian Workflow Standard. It governs vault structure, canonical entry points, Current Notes, link/graph behavior, promotion, handoff, compaction, documentation commits, attachments, validation, and archival.
122. The repository documentation folder is the one live Obsidian vault. Copied vaults, duplicate current authorities, and version-suffixed live filenames are prohibited; document versions live in headers and Git history preserves prior states.
123. `Current Notes.md` is a working capture surface only. It may record observations, questions, proposals, decisions awaiting promotion, tests, bugs, risks, and handoff context, but it never becomes the sole authority or permanent evidence store.
124. Every material Current Notes entry is classified, routed, promoted into its owning authority or permanent record, linked to its destination, and compacted after checkpoint closeout when no longer active.
125. Every active repository exposes README and Current Notes entry points. The central suite additionally maintains the Graph Roadmap, health check, documentation program roadmap, learning catalog, decision log, and integration matrices as navigation and status surfaces.
126. Essential documentation remains readable in ordinary UTF-8 Markdown without an Obsidian plugin. Relative Markdown links are preferred for new critical cross-surface navigation; existing Obsidian wikilinks remain approved and may be normalized during consistency review.
127. Obsidian tags, Graph View, backlinks, Mermaid diagrams, and maps of content are navigation aids only. They never override SFGSS-000, standards, package specifications, ADRs, integration specifications, or retained evidence.
128. Device-specific Obsidian workspace state, personal themes, local paths, credentials, secrets, caches, and private support/player data are not committed unless an explicit authority approves a safe shared form.
129. Every meaningful checkpoint reconciles Current Notes, updates affected authorities and navigation hubs, verifies one current handoff, validates links/status/evidence, and commits documentation with or immediately adjacent to the work it describes.
130. Git history, checkpoint reports, ADRs, research records, test reports, changelogs, and release records are the durable archive. Current Notes is compacted rather than allowed to grow indefinitely; checkpoint ZIPs are transport artifacts, not competing sources of truth.

131. SUITE-DOC-30 passes the Standards and Package Consistency Review across SFGSS-001 through SFGSS-010, all twenty-eight package foundations, ADR-001 through ADR-003, and the Foundation, Expansion, and Advanced integration matrices.
132. SFGSS-008 formal public titles use the canonical spaced en dash in current package metadata. Historical typography variants remain aliases and do not create new product identities.
133. Existing package document IDs are permanent. The Crucible and the five Advanced foundations retain their grandfathered non-`-001` IDs rather than receiving retroactive replacements.
134. Primary public Runtime assemblies default to `autoReferenced: true`; Editor, test, sample, internal support, bridge, and provider assemblies default to `false` unless an approved specification records a justified exception.
135. First Light keeps launch authority and presenter contracts in its neutral Runtime assembly while the default uGUI implementation lives in `EchoDevGames.EchoLaunch.Presentation.UGUI`.
136. Accord, Chronicle, Passage, and Will explicitly distinguish Unity asset GUIDs from runtime-safe domain identities, fingerprints, and source metadata.
137. Accord and Will preserve unknown optional data and unknown fields through opaque records or extension-data-capable serializers; Unity JSON round-tripping alone is not accepted as preservation proof.
138. Package parent-authority headers preserve approval provenance. A SUITE-DOC-30 consistency addendum in each package identifies the standards currently governing implementation.
139. The central Current Notes page is compacted at SUITE-DOC-30. Git history and promoted records preserve earlier checkpoint detail; only active context and one current handoff remain on the workbench.
140. Package implementation remains locked. SUITE-DOC-31 creates the full-suite authority, dependency, bridge, and persistence matrix using the reconciled standards and package versions.
141. SFGSS-INT-SUITE-001 is the approved full-suite authority, dependency, bridge, commit, persistence, identity, diagnostics, Laboratory, and removal matrix for all twenty-eight packages. It summarizes approved contracts and never overrides a higher authority.
142. Core Echo runtime packages retain standalone ownership and do not acquire hidden peer dependencies through the full-suite matrix. Optional cross-package behavior remains in visible bridges, provider adapters, or project adapters.
143. Every multi-package workflow names one commit owner for each mutation. A bridge may coordinate or validate, but it cannot promise rollback after a foreign authority has committed unless it owns a real transaction across those resources.
144. Global preferences belong to The Accord; save files, slots, generations, backup, and recovery transport belong to The Chronicle; each package participant/provider owns the schema and meaning of its durable payload.
145. Session-only roots, handles, leases, provider objects, scene references, runtime actor objects, active casts, active interactions, camera blends, pool contents, and network session objects are never treated as durable state without an explicitly approved detached snapshot.
146. Package identities remain qualified by domain. Input users, multiplayer participants, characters, control owners, controller leases, AI agents, ability owners, combat targets, inventory items, objective runs, world locations, scenes, UI focus, and interaction focus are related only through explicit mappings.
147. The recommended composition order is an initialization plan, not a hard dependency graph. Package cores initialize independently; optional bridges register after both peers are Ready and reconcile current snapshots without replaying unsafe history.
148. Bridge and provider artifacts are removed before their peers. Project-owned definitions, configuration, durable payloads, aliases, receipts, and migration evidence remain preserved unless an explicit destructive prune is approved.
149. Standalone Laboratories remain package-owned proof, Integration Laboratories remain bridge/provider proof, and the Integration Lab remains the authority for tested package combinations and compatibility snapshots.
150. SUITE-DOC-31 passes with twenty-eight package authority rows, a canonical bridge catalog, explicit persistence layers, zero approved core dependency cycles, zero authority collisions, and no promoted empirical evidence. Package implementation remains locked pending SUITE-DOC-32, the learning reviews, and SUITE-DOC-33.
151. Package learning reviews use a just-in-time local gate. Each package must complete its own learning review immediately before its first implementation checkpoint; all twenty-eight reviews are no longer required before First Light implementation. SUITE-DOC-33 may authorize FL-M1-01 because PKG-LEARN-001 is complete, while every other package remains individually locked until its review passes.
152. SUITE-DOC-33 passes with advisory and activates the implementation program only through FL-M1-01 – First Light Package Skeleton.
153. Initial implementation activation is checkpoint-specific. It does not authorize a milestone, package family, later First Light behavior, or another package.
154. FL-M1-01 must verify the live Unity compile, working tree, package path, and exact baseline uGUI version before creating files; these empirical checks remain `Not run` at documentation-gate closeout.
155. First Light may proceed because PKG-LEARN-001 is complete. Every later package remains locally locked until its own just-in-time learning review and readiness decision activate an approved Checkpoint Build Plan.

---

## 22. Open Decisions Requiring Later Approval

These questions do not block approval of the overall architecture, but they must be resolved before affected releases.

1. Final package icons, art treatments, and restrained package-specific flavor vocabulary. Public package titles are registered by SFGSS-008.
2. Licensing model for package source and samples.
3. Whether EchoControllers remains one modular UPM package or graduates into a small family of controller packages as dependencies and release cadence become clearer.
4. The first approved networking provider for EchoMultiplayer after dated research and comparison prototypes.
5. Which platform/native providers, if any, extend the Observatory with desktop hardware sensors beyond Unity runtime counters.
6. Whether EchoAI, EchoCombat, EchoAbilities, and EchoWorld graduate from candidates to committed packages.

---

## 23. Definition of Success

The Sperk’s Forge Game Systems Suite succeeds when:

- A clean project can install one package without inheriting unrelated systems.
- A developer can assemble a game-jam foundation quickly and still understand every generated piece.
- Startup and persistent services remain duplicate-safe across scenes.
- Settings, saves, UI, input, scene flow, game state, and audio have clear authorities.
- Jukebot can replace audio foundations in both Rescuers2D and Don’t Get Vince’d without game-specific code entering the package.
- Hackulos can use the general packages plus optional RPG data without forcing RPG concepts into every game.
- Character management, crafting, and multiplayer connect through explicit contracts rather than circular dependencies.
- Every package and controller preset proves itself in an isolated Test Lab before combined showcase scenes are treated as success.
- A developer can enable a polished in-game diagnostic overlay that clearly shows startup, package health, performance, scene, and runtime state without making diagnostics a mandatory dependency.
- Package samples prove behavior but never become production requirements.
- Documentation is accurate enough for a fresh collaborator or ChatGPT conversation to continue from the current checkpoint.
- Completed packages reduce repeated infrastructure work while leaving each game free to establish its own identity.

---

## 24. Immediate Next Step

The suite identity and the one hundred fifty-five decisions in Section 21 are approved. The complete documentation program, integration matrices, handoff audit, SFGSS-ADR-004, and PKG-LEARN-001 support the initial implementation gate.

SUITE-DOC-33 passes with advisory and activates only:

```text
FL-M1-01 – First Light Package Skeleton
```

Follow `Checkpoint Build Plans/First_Light_M1_Package_Skeleton_Checkpoint_Build_Plan.md` v1.3.0 and SFGSS-005 v1.4.0.

Before creating files:

1. Open the Unity 6000.3.8f1 project and confirm a clean Console.
2. Review Git status and preserve unrelated work.
3. Confirm the First Light package path is absent or safely reviewed.
4. Inspect the exact baseline `com.unity.ugui` version.
5. Stop if any starting condition conflicts with the approved plan.

FL-M1-01 authorizes only the package manifest, four assembly definitions, documentation shell, generated `.meta` files, and bounded validation. It authorizes no C# file or launch behavior.

After FL-M1-01 closes, do not begin FL-M2-01 automatically. Create and approve its Checkpoint Build Plan. Before another package begins implementation, complete that package's own just-in-time learning review.


---

## Graph Navigation

#sfgss/authority #sfgss/navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Integration Specifications/SFGSS-INT-SUITE-001_Full_Suite_Authority_Dependency_Bridge_and_Persistence_Matrix|Full Suite Integration Matrix]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
