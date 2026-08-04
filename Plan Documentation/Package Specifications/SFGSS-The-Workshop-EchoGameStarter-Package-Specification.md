# The Workshop - Project Starter Package Specification

**Working document ID:** SFGSS-PKG-ECHOGAMESTARTER-001  
**Specification version:** 1.1.0  
**Status:** Approved  
**Technical package name:** EchoGameStarter  
**Public title:** The Workshop - Project Starter  
**Package ID:** `com.echodevgames.echo-game-starter`  
**Editor namespace:** `EchoDevGames.EchoGameStarter`  
**Runtime namespace:** Not applicable; the MVP is Editor-only  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoGameStarter`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Primary Editor UI:** UI Toolkit `EditorWindow`  
**Default generated root:** `Assets/<Game>/`  
**Transient transaction root:** `Library/EchoGameStarter/Transactions/`  
**Durable generation record:** Project-owned `WorkshopGenerationManifest` beneath the generated project root  
**Parent authority:** SFGSS-000 and SFGSS-001  
**Last updated:** August 3, 2026

> “Choose the tools, inspect every strike, and leave the new project knowing exactly how it was forged.”

> **Approval rule:** This specification is approved as the package authority. Runtime implementation remains intentionally deferred until FW-DOC-11 and FW-DOC-12 reconcile all ten Foundation specifications and authorize the first implementation checkpoint.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-03 | Proposed | Initial complete specification based on SFGSS-000 v0.6.0, SFGSS-001 v1.1.0, and the nine previously approved Foundation package specifications | Pending |
| 1.0.0 | 2026-08-03 | Approved | Approved Editor-only composition authority, package catalog, dry-run planning, resumable application, package setup-facade adapters, generated ownership records, repair/removal guidance, presets, diagnostics, and Workshop Laboratory | Jesse “Echo” Adams |
| 1.1.0 | 2026-08-03 | Approved | Incorporated FW-DOC-11, accepted SFGSS-ADR-001 as the exact Foundation setup-facade protocol, and closed the peer-facade reconciliation blocker without adding a shared package dependency | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Workshop - Project Starter  
**Technical identifier:** EchoGameStarter  
**Flavor line:** Assemble the foundation in plain sight; every generated piece keeps its name, owner, and exit route.  
**Plain-language subtitle:** Editor-only project composition, package selection, dry-run planning, safe generation, repair guidance, and readiness reporting.

**One-sentence ownership contract:**

> EchoGameStarter owns Editor-time selection, planning, installation coordination, project-foundation generation, package setup orchestration, conflict detection, generation receipts, repeat-run analysis, repair planning, removal guidance, and composition diagnostics; it does not own the runtime behavior of installed packages, package repositories or credentials, project gameplay rules, production content, builds, cloud services, user data, or a permanent runtime manager.

### 1.1 Elevator summary

The Workshop turns a set of independently installable Sperk’s Forge packages into a visible, reviewable project-foundation plan. A developer chooses a preset or a custom package set, selects project options, reviews every package, bridge, scene, folder, configuration asset, prefab, input template, UI template, and project-setting change, then explicitly applies the plan. The Workshop records what it requested, what Unity resolved, what it created, what it adopted, what it refused to touch, and what remains for the developer.

The Workshop is an **Editor-only composer**. It does not contain a runtime `GameManager`, service locator, bootstrap replacement, or hidden copy of another package’s setup logic. Each selected package remains responsible for its own configuration types, prefabs, validation rules, and setup behavior. After installation, The Workshop invokes an exact, versioned package setup facade through a named adapter descriptor. The invocation may use narrow Editor reflection because the packages must remain compile-time independent, but it never scans arbitrary assemblies looking for unknown providers and never invokes an endpoint not listed in the approved catalog.

Every apply operation begins as an immutable dry-run plan. Package changes are separated from asset generation because Unity Package Manager operations can trigger resolution, compilation, and domain reload. A small transient transaction journal under `Library/EchoGameStarter` allows the operation to resume or fail safely after reload. Durable generation history is written into a project-owned manifest beneath `Assets/<Game>/`. The manifest is evidence, not a claim that the Workshop permanently owns the game’s assets.

The MVP ships two presets: **Blank Modular Starter** and **Game Jam Quickstart**. Both expose every selected package and bridge. Blank Modular Starter can create only the project skeleton or any user-selected Foundation combination. Game Jam Quickstart proposes a visible application shell using First Light, the Observatory, the Accord, the Passage, the Pulse, Resonance, the Will, and the Looking Glass, while offering the Chronicle as an explicit save-model choice rather than silently forcing save files into every jam.

### 1.2 Why this belongs in The Sperk’s Forge

Repeated Unity projects spend their first hours rebuilding folder structures, Boot scenes, package roots, settings assets, audio mixers, input assets, UI canvases, scene lists, save configurations, and integration glue. Manual setup is especially costly when a package suite is intentionally modular: the user must understand which packages are optional, which bridges are separate, what startup order is valid, and which generated assets belong to the project.

DeverQuest demonstrates the value of guided generation, readiness reports, repair tools, clear first-run flows, and documentation-rich package delivery. Echo Systems Lab demonstrates checkpoint-driven foundations and explicit scenes. Rescuers2D demonstrates the failure modes of hidden boot assumptions and multiple persistent managers. The Workshop preserves guided setup while refusing to recreate those project-specific couplings.

The package is justified because composition itself is a recurring concern with a distinct authority. Without one composer, every package either grows its own incompatible starter wizard or documentation asks the user to perform a long manual ritual. With The Workshop, packages remain independently usable while the suite gains one optional path for quickly assembling known-good combinations.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Pair “The Workshop” with “Project Starter.” |
| Setup headings/tooltips | Yes | Every flavored line must immediately state the technical action. |
| Preset illustrations | Optional | Art is replaceable and does not alter generated output. |
| Generated sample copy | Optional | Sample text is clearly marked and removable. |
| Editor API/type names | No lore-only names | Types describe catalogs, plans, operations, manifests, adapters, conflicts, and reports. |
| Generated project data | No required Hackulos content | The game owns names, namespaces, scenes, copy, content, and presentation. |
| Runtime output | No Workshop branding requirement | Generated games do not need to show The Workshop or the Sperk. |

## 2. Problem Statement

### 2.1 Current problem

A package suite can be modular yet still be difficult to start. The common failures are not limited to forgetting a checkbox:

1. Package dependencies are selected without showing the actual packages and bridges that will enter the project.
2. Package versions float to branches or ranges without a reproducible record.
3. A wizard edits `manifest.json`, scenes, build lists, project settings, prefabs, and ScriptableObjects in one opaque operation.
4. Domain reload interrupts the wizard and loses progress.
5. Setup code is copied into a central composer, causing the composer to drift from package APIs.
6. A package gains a hard dependency on the composer merely to expose setup.
7. Existing assets are overwritten because a path matches a template path.
8. A rerun creates duplicate roots, duplicate EventSystems, duplicate input assets, or suffixed copies.
9. A repair tool cannot distinguish an unchanged generated asset from a project-authored modification.
10. Generated scenes contain hidden package assumptions or unrelated package code.
11. Build Settings and Unity 6 Build Profiles are modified without showing the previous scene list.
12. Package installation fails halfway and leaves no recovery instructions.
13. Removal instructions delete assets the project has since adopted or modified.
14. A preset silently installs runtime systems the project did not request.
15. The generated project depends on the Workshop at runtime.
16. Credentials, registry tokens, or private Git information are copied into reports.
17. A setup report says “success” even though compilation, package validation, or Test Labs fail.
18. Starter templates become a monolithic framework whose generated `GameManager` owns every future feature.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| DeverQuest | Guided setup, identity generation, repair, readiness checks, migration, detailed documentation | Product-like first-run experience and explicit reports | Avoid hidden user-machine state, project-specific runtime ownership, and destructive defaults |
| Echo Systems Lab | Checkpoint plans, predictable project folders, isolated system scenes | Testable foundations and visible project structure | Generate only approved modules and record every output |
| Rescuers2D | Multiple persistent systems, hard-coded boot assumptions, direct scene and input setup | Practical target configuration and direct-scene needs | Prevent duplicate authorities and hidden scene dependencies |
| Don’t Get Vince’d | Existing project must retain working systems while packages are introduced | Incremental adoption | Generate side-by-side and preserve-until-parity rather than replace blindly |
| Hackulos planning | Requires a selectable application shell plus later RPG modules | Modular composition | Keep game-specific RPG data and rules outside the starter |
| First Light | Own setup tool, Boot scene, launch configuration, steps, and destination | Package-owned setup authority | Workshop invokes the setup facade; it does not recreate launch assets itself |
| Observatory | Root, overlay preset, validation level, support report | Visible diagnostics choice | Development-only/player-accessible choices remain explicit and removable |
| Accord | Defaults, configuration, built-in sections, persistence policy | Global preference setup | Workshop never writes settings files or applies runtime settings |
| Passage | Scene catalog, routes, root, scene-list validation | Stable scene references and normal travel | Workshop proposes scenes/routes; Passage validates and owns them |
| Pulse | State definitions, policy assets, root | Explicit runtime-state model | Workshop cannot invent project win/loss/dialogue semantics beyond selected templates |
| Resonance | Mixer, configuration, root, profile families, Audio Laboratory | Data-driven audio foundation | Project selects semantic profile families and supplies production clips later |
| Will | Project-owned action asset, contexts, glyphs, root, rebind configuration | Input template choice | Workshop does not define gameplay action meaning or movement controllers |
| Looking Glass | UI root, layers, theme, selected screen templates | Modular visual shell | Templates remain replaceable views, not domain authorities |
| Chronicle | Save configuration, slot model, root, sample participants, sandbox | Versioned save foundation | Save remains optional and game-owned payloads remain outside the starter |

### 2.3 Consequences of doing nothing

- New projects repeatedly lose hours to infrastructure setup.
- Package documentation becomes a long sequence of manual cross-references.
- “Quickstart” means importing a monolith instead of selecting independent systems.
- Existing projects avoid adopting packages because migration feels irreversible.
- Setup drift produces support issues that are hard to reproduce.
- Starter combinations are never tested as products.
- The suite’s modularity exists in architecture diagrams but not in the first user experience.

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one optional Editor authority for composing selected Sperk’s Forge packages and project-owned foundation assets.
- Keep the MVP entirely outside Player builds and omit a runtime assembly.
- Offer a preset path and a fully custom path.
- Show every direct package, transitive package, bridge, template, and project change before application.
- Use exact package IDs and approved source/version policies.
- Separate package-resolution operations from asset-generation operations and resume safely after domain reload.
- Delegate package-specific creation and validation to exact package setup facades.
- Generate only beneath a user-approved project root unless a separately listed project-setting operation is approved.
- Default to create-only-safe behavior.
- Detect existing paths, GUIDs, roots, scenes, EventSystems, action assets, and project-setting conflicts.
- Record generated, adopted, skipped, conflicted, and modified items in a durable manifest.
- Make repeat runs idempotent where package setup facades support it and visibly non-destructive otherwise.
- Produce repair and removal plans that preserve modified or project-owned assets.
- Generate a readable setup report and project-readiness checklist.
- Support Unity 6 global scene lists and Build Profile scene overrides through an adapter.
- Provide a disposable Workshop Laboratory and clean-project generation tests.
- Keep generated projects operational when The Workshop package is removed.

### 3.2 Non-goals

- No permanent runtime root, `GameManager`, service locator, event bus, or generated gameplay framework.
- No ownership of another package’s configuration schema, prefab internals, validator, or runtime lifecycle.
- No automatic invention of game mechanics, levels, narrative, characters, items, objectives, combat, or art.
- No direct deployment, builds, version stamping, release upload, or storefront integration; those belong to The Foundry or provider tools.
- No package publishing, registry hosting, authentication, credential storage, or license approval.
- No unrestricted execution of arbitrary setup code discovered by scanning assemblies.
- No silent branch-based Git dependencies in recommended presets.
- No silent overwrite, delete, move, rename, merge, or reserialize of project-owned assets.
- No guaranteed binary merge of scenes, prefabs, input assets, or ScriptableObjects.
- No complete automatic uninstaller in the MVP.
- No assumption that every project wants Boot, Main Menu, Results, saves, audio, or diagnostics.
- No replacement of Unity Hub project creation or Unity Package Manager.
- No requirement that a project keep The Workshop after generation.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity 6 project | Select a preset, review the plan, generate a working foundation, open the first scene, and understand every created piece |
| Programmer | Existing project with custom structure | Select only needed packages, choose a safe target root, inspect conflicts, and adopt generated pieces incrementally |
| Designer/content author | Needs a playable shell, not architecture code | Choose screens, audio profiles, settings domains, save model, and scene flow through labeled options |
| Tester | Needs reproducible starter validation | Generate the same preset in a disposable project, compare manifests, and run readiness/Test Lab checks |
| Maintainer | Updates package or preset versions | See exact version drift, changed operations, conflicts, and safe upgrade/repair choices |
| Support/debug user | Receives a broken generated project | Read the generation receipt, installed-package snapshot, validation results, and unresolved manual steps |
| Package author | Provides setup for an Echo package | Expose a narrow versioned Editor setup facade without adding a runtime or core dependency on The Workshop |

### 3.4 Measurable success criteria

- Installs into a clean supported Unity project with zero runtime assembly contribution.
- Generates Blank Modular Starter with no peer Echo package selected.
- Generates Game Jam Quickstart with all chosen packages visible in the approved plan and report.
- Removing The Workshop after successful generation does not break the generated project.
- Re-running the same plan produces no duplicate roots, scenes, folders, assets, or scene-list entries.
- A project-modified generated asset is never overwritten by default.
- Package operations resume or report a recoverable failure after domain reload or Editor restart.
- Generated assets live only under the approved root except separately approved project/scene-list operations.
- The report identifies requested and resolved package versions.
- Unsupported or missing package setup facades block only affected features and leave clear manual instructions.
- A clean-project validation confirms the generated project compiles and its selected package validators pass.
- The Workshop Laboratory never modifies real project output outside its sandbox without explicit approval.

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo and small-team Unity developers.
- Game-jam teams creating a fast application shell.
- Programmers integrating Sperk’s Forge packages into existing games.
- Designers selecting package templates and profiles.
- QA testers validating starter combinations and repeatability.
- Maintainers authoring package setup facades and preset migrations.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| UC-001 | Open Workshop | Developer | Package installed in supported Editor | Current project is inspected without modification | MVP |
| UC-002 | Create custom plan | Developer | Package catalog available | User selects packages/options and receives a complete dry run | MVP |
| UC-003 | Select Blank Modular Starter | Developer | Valid project root | Minimal folders/assemblies and optional selected packages are planned | MVP |
| UC-004 | Select Game Jam Quickstart | Developer | Valid project root | Recommended application shell is proposed with every choice visible | MVP |
| UC-005 | Inspect package graph | Developer | Packages selected | Direct, transitive, bridge, source, version, and removal relationships are shown | MVP |
| UC-006 | Apply package changes | Developer | Approved package plan | UPM operation completes or resumes with exact result | MVP |
| UC-007 | Resume after domain reload | Workshop | Transaction journal exists | Plan resumes at the first unsettled phase without repeating settled work | MVP |
| UC-008 | Generate project skeleton | Developer | Root approved | Folders, optional asmdefs, docs, and project identity assets are created safely | MVP |
| UC-009 | Invoke package setup facade | Workshop | Selected package installed and compatible | Package-owned setup operations return structured results | MVP |
| UC-010 | Generate scenes and routes | Developer | Selected packages support scene setup | Approved scenes, definitions, and scene-list changes are produced | MVP |
| UC-011 | Generate UI/audio/input/save options | Developer | Corresponding packages selected | Package-owned assets are generated through their setup facades | MVP |
| UC-012 | Detect existing asset conflict | Workshop | Target path exists | Conflict is classified; no overwrite occurs | MVP |
| UC-013 | Repeat same generation | Developer | Prior manifest exists | Operations resolve to No Change, Repair Candidate, Adopted, or Conflict | MVP |
| UC-014 | Repair missing generated item | Developer | Manifest proves prior generation | Safe repair plan recreates only eligible missing item | MVP |
| UC-015 | Produce removal guide | Developer | Generation manifest exists | Report lists safe removable, modified-retain, manual, and package-dependency items | MVP |
| UC-016 | Validate generated foundation | Developer/tester | Generation complete | Package validators, compilation, scenes, references, and readiness checks run | MVP |
| UC-017 | Export support report | Developer | Explicit request | Redacted composition snapshot is written without credentials | MVP |
| UC-018 | Compare preset upgrade | Maintainer | New preset version available | Diff plan identifies additions, removals, replacements, and manual conflicts | MVP/basic |
| UC-019 | Apply complex upgrade automatically | Maintainer | Migration provider exists | Supported operations apply with backups and explicit confirmation | Later |
| UC-020 | Compose expansion packages | Developer | Expansion specifications/adapters approved | Workshop offers additional package families | Later |

### 4.3 Explicitly unsupported use cases

- Creating a complete genre game from one button.
- Installing untrusted arbitrary package sources without explicit source review.
- Merging arbitrary existing scenes or prefabs semantically.
- Removing modified project assets merely because they were initially generated.
- Generating runtime systems for packages not installed or not represented by a compatible setup adapter.
- Modifying Unity Editor installations, Hub templates, global UPM credentials, or user-wide registries.
- Treating a successful UPM request as proof that the generated project compiles or is ready.

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Starter preset definitions and package-selection UX.
- Project composition option capture.
- Package catalog/source/version descriptors used by the Workshop.
- Dependency and bridge plan visualization.
- Immutable dry-run plans and operation identities.
- Package Manager request coordination for approved package changes.
- Transaction journaling and resumable apply phases.
- Exact setup-facade adapter descriptors and invocation policy.
- Generic project-root, folder, assembly, documentation, and manifest generation.
- Composition conflict classification.
- Durable generation manifests and reports.
- Repeat-run comparison, repair planning, removal guidance, and basic preset-upgrade diffs.
- Workshop-specific validation, diagnostics, support snapshots, and laboratory fixtures.

### 5.2 The package does not own

- Runtime startup, diagnostics, preferences, scene travel, game state, audio, input, UI, or saves.
- Package-owned configuration types, roots, prefabs, scenes, validators, migrations, or runtime APIs.
- Unity Package Manager dependency resolution internals.
- Package source hosting, credentials, Git authentication, registry authentication, or lock-file semantics.
- Project gameplay, content, presentation, naming, or domain data.
- Production builds, deployment, release output, or external services.
- Global user preferences unrelated to the Workshop window.
- Any generated asset after the project adopts or modifies it.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How The Workshop interacts |
|---|---|---|
| Initial startup | First Light | Select/install and invoke its Editor setup facade; never implements startup |
| Runtime diagnostics | Observatory | Select overlay/validation preset and request package-owned setup |
| Global preferences | Accord | Select sections/default templates and request setup; never writes runtime settings |
| Scene travel | Passage | Propose scenes/routes and request Passage setup; never loads runtime scenes |
| Runtime mode/pause | Pulse | Select state/policy templates and request setup |
| Music/SFX/ambience | Resonance | Select mixer/profile families and request setup; project supplies production audio |
| Input contexts/rebinding | Will | Select action/context/glyph templates and request setup; never defines controller behavior |
| Screens/HUD/modals | Looking Glass | Select UI shell/templates/theme and request setup; never owns game rules |
| Save files/slots | Chronicle | Select save model and request setup; never defines participant payloads |
| Package installation/resolution | Unity Package Manager | Submit approved requests and report resolved results |
| Build scene lists | Unity Editor / Build Profiles | Use a replaceable adapter and explicit plan operations |
| Build/release output | EchoBuildTools/The Foundry | Outside MVP; later optional composition only |
| Game identity and content | Consumer project | Capture user choices and generate only templates/placeholders |
| Source control | Git/user workflow | Write files and reports; never commit or push automatically in MVP |

### 5.4 Boundary tests

A proposed Workshop feature remains in this package only when all are true:

1. It concerns **Editor-time composition**, not runtime behavior.
2. It can describe the exact change before applying it.
3. It leaves the authoritative package or project in control of the generated behavior.
4. It can fail without leaving an unreported hidden dependency.
5. It does not require another package to compile merely to open The Workshop.
6. It preserves project-owned assets by default.
7. It leaves a durable receipt and removal/repair explanation.
8. The generated project remains functional when The Workshop is removed.

## 6. Independence Contract

### 6.1 Standalone guarantees

The Workshop must:

- Compile with only its declared Unity Editor dependencies.
- Open and generate a Blank Modular Starter with no other Echo package installed.
- Contain no runtime assembly and contribute no Player code.
- Treat all Foundation packages as selectable peers rather than hard dependencies.
- Use catalog data without loading peer assemblies until the peer is installed.
- Fail safely when a package source, version, adapter, or setup facade is unavailable.
- Keep project content outside immutable package source.
- Use a sandbox root by default in its Laboratory.
- Leave generated projects functional after sample and Workshop removal.
- Avoid hidden tags, layers, scene names, action maps, or folder paths.
- Make package source and version choices visible.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | Window opens; Blank Modular Starter can generate generic project skeleton | Clean-project EditMode and Laboratory test |
| No Foundation peers installed | Catalog shows available choices but no peer types are loaded | Assembly/package inspection test |
| Package source unavailable | Plan blocks selected package with actionable source result | Offline/missing-source simulation |
| Setup facade missing | Package may remain installed; package-specific generation blocks and manual path is reported | Adapter mismatch test |
| Optional package deselected | No package, bridge, asset, scene, or setting for that feature is generated | Plan and manifest diff |
| Workshop removed after generation | Generated project compiles and runs according to selected packages | Clean removal test |
| Sample content deleted | Editor package and generated output remain valid | Sample-removal test |
| Transaction interrupted | Resume or rollback guidance appears; settled phases are not repeated | Domain-reload/restart fault test |
| Existing project root | Conflicts are classified; no silent overwrite | Existing-project fixture |
| Repeat run | Deterministic No Change/Repair/Conflict results | Idempotency test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Editor | Platform | Yes | Unity 6000.0 | Editor window, AssetDatabase, scenes, serialization, Package Manager client | Package cannot run outside Editor |
| UI Toolkit Editor APIs | Platform | Yes | Unity 6000.0 | Primary Workshop window and report UI | No runtime effect |
| Unity Package Manager Client API | Platform | Yes | Unity 6000.0 | List, add/remove, and resolve approved project dependencies | Existing dependencies remain in manifest |
| Unity Test Framework | Test | Development only | Compatible Unity 6 version | EditMode and generated-project tests | No production dependency |
| Foundation peer packages | Optional selected output | No | Catalog-defined | Installed/generated only when selected | Deselect or remove through explicit plan |
| Package-specific Workshop adapters | Optional Editor integration | No | Adapter-defined | Invoke exact package setup facade | Missing adapter blocks only affected setup |

### 6.4 Forbidden dependencies

- Any Foundation runtime or Editor assembly as a core compile-time dependency.
- Project assemblies.
- Samples as production implementation dependencies.
- Credentials, registry tokens, SSH keys, or user-specific absolute paths.
- An unapproved shared runtime `EchoCore`.
- A permanent generated dependency on The Workshop.
- Arbitrary reflection scanning as the normal package-provider mechanism.

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| CAP-001 | Project inspection | Read package, scene-list, root, compiler, and existing-output state without changes | Approved | Yes | Editor | First window phase |
| CAP-002 | Package catalog | Show approved IDs, sources, versions, bridges, compatibility, and trust notes | Approved | Yes | Editor/Data | No credentials |
| CAP-003 | Preset selection | Blank Modular Starter and Game Jam Quickstart | Approved | Yes | Editor/Data | All choices visible |
| CAP-004 | Custom composition | Select any compatible Foundation subset | Approved | Yes | Editor | No mandatory bundle |
| CAP-005 | Dry-run plan | Immutable ordered operations with reasons, risks, and conflicts | Approved | Yes | Editor/Core | Required before apply |
| CAP-006 | Package graph | Direct/transitive/bridge visualization and removal impact | Approved | Yes | Editor | Based on catalog + UPM result |
| CAP-007 | Package apply | Coordinated add/remove request with explicit confirmation | Approved | Yes | Editor | Removal conservative |
| CAP-008 | Reload resume | Journal persists phase across domain reload/Editor restart | Approved | Yes | Editor/Core | Library state |
| CAP-009 | Generic skeleton | Root folders, optional asmdefs, docs, identity/config directories | Approved | Yes | Editor | Project-owned output |
| CAP-010 | Setup facade adapters | Invoke exact package-owned setup endpoint | Approved | Yes | Editor/Integration | Named, versioned, bounded |
| CAP-011 | Scene composition | Create approved starter scenes and scene-list plan | Approved | Yes | Editor | Package adapters own internals |
| CAP-012 | Conflict classification | New, existing-match, modified, adopted, missing, incompatible, unsafe | Approved | Yes | Editor/Core | No silent overwrite |
| CAP-013 | Generation manifest | Durable record of outputs, origins, hashes, GUIDs, versions, and statuses | Approved | Yes | Data | No secrets |
| CAP-014 | Setup report | Human-readable Markdown/JSON summary and unresolved actions | Approved | Yes | Editor/Docs | Commit-friendly |
| CAP-015 | Readiness validation | Run Workshop and selected-package validators after generation | Approved | Yes | Editor | Success != package installed |
| CAP-016 | Repeat-run analysis | No Change, repair candidate, conflict, and drift results | Approved | Yes | Editor | Idempotent by design |
| CAP-017 | Repair plan | Recreate eligible missing unchanged outputs | Approved | Yes | Editor | User confirms apply |
| CAP-018 | Removal guide | Classify safe removable versus modified/manual items | Approved | Yes | Editor/Docs | Full auto-uninstall deferred |
| CAP-019 | Basic upgrade diff | Compare preset/catalog/adapter versions and plan changes | Approved | Yes | Editor | Automatic complex migration deferred |
| CAP-020 | Support snapshot | Redacted package/plan/manifest/validation export | Approved | Yes | Editor | No credentials/content payloads |
| CAP-021 | Workshop Laboratory | Sandbox generation, conflict, repeat, failure, and cleanup proof | Approved | Yes | Editor/Sample | No scene required for core proof |
| CAP-022 | Existing-project adoption | Side-by-side root and selective package generation | Approved | Yes | Editor | Preserve-until-parity |
| CAP-023 | Automatic binary merge | Merge arbitrary scenes/prefabs/assets | Rejected | No | N/A | Unsafe and misleading |
| CAP-024 | Automatic Git commit/push | Run source-control writes | Deferred | No | Provider | Requires separate reviewed integration |
| CAP-025 | Expansion presets | Controllers, RPG, multiplayer, narrative, etc. | Deferred | No | Editor | After package specs/adapters |
| CAP-026 | Full automatic uninstall | Remove packages/assets/settings without review | Deferred | No | Editor | Removal guide first |

### 7.2 MVP capability set

The smallest complete release includes:

1. Editor-only package anatomy and UI Toolkit Workshop window.
2. Approved package catalog for the nine Foundation peers.
3. Blank Modular Starter and Game Jam Quickstart.
4. Custom package selection and visible dependency/bridge graph.
5. Immutable dry-run plan.
6. UPM package add/remove coordination with domain-reload resume.
7. Generic project skeleton generation beneath a chosen root.
8. Exact package setup-facade adapter protocol.
9. Selected package setup orchestration.
10. Conflict detection and create-only-safe default behavior.
11. Generation manifest and Markdown/JSON report.
12. Repeat-run analysis, safe repair plan, and removal guide.
13. Readiness validation.
14. Workshop Laboratory and clean temporary-project acceptance tests.
15. Generated project independence after Workshop removal.

### 7.3 Later capability set

- Additional genre and expansion presets.
- Visual graph editing for custom presets.
- Automatic package-version upgrade migrations with package-owned migration providers.
- Full dependency-aware uninstall assistant.
- Source-control provider integrations.
- Team-shared preset registries.
- CI/headless generation mode after Editor API viability is proven.
- Template marketplace/catalog browsing, subject to trust and licensing review.
- Build Profile and platform-specific preset expansion through The Foundry.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| One generated universal `GameManager` | Rejected | Violates package authorities and creates a runtime monolith | Never under current suite architecture |
| Copy each package’s setup logic into Workshop | Rejected | Guaranteed drift and authority violation | Never; use setup facades |
| Open-ended reflection discovery | Rejected | Untrusted, nondeterministic, hard to version | Only exact catalog endpoints are allowed |
| Full semantic merge of binary Unity assets | Rejected | Cannot be made generally safe | Package-specific migration provider may handle known assets |
| Silent latest-branch package installs | Rejected | Not reproducible | Exact tags/versions required for recommended presets |
| Auto-commit and auto-push | Deferred | Source-control trust, credentials, branches, and user intent | Separate provider specification/ADR |
| Cloud-hosted preset service | Deferred | Network, authentication, privacy, availability | Provider research |
| Runtime starter manager | Rejected | Workshop is Editor-only composition | Never |
| Full automatic uninstall | Deferred | Modified asset ownership and package dependency risk | Proven manifest/repair system and dedicated design |
| Hub project templates | Deferred | Different distribution and upgrade model | After UPM workflow proves stable |

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Preset definitions, package catalog entries, source/version policies, adapter descriptors, generation schemas, project-option assets | Mutable apply state, credentials, live package requests, project-specific content |
| Editor runtime state/behavior | Project inspection, plan builder, conflict engine, transaction coordinator, UPM request wrapper, adapter invoker, generators, validators, report writers | Player runtime behavior, peer package authority, production game state |
| Presentation/feedback | Workshop window, plan diff, package graph, conflict browser, progress, report viewer, Laboratory controls | Hidden authoritative decisions or package setup logic |
| Generated project output | Project-owned folders, asmdefs, scenes, configurations, prefabs, adapters, docs, manifests | A permanent Workshop runtime dependency or concealed package selection |

### 8.2 Component topology

```text
WorkshopWindow
    |
    v
WorkshopSession -----------------------------------------+
    |                                                     |
    +--> ProjectInspector                                 |
    +--> PresetCatalog / PackageCatalog                   |
    +--> CompositionSelection                             |
    +--> WorkshopPlanBuilder                              |
              |                                           |
              +--> ConflictAnalyzer                       |
              +--> PackageGraphResolver                   |
              +--> ProjectOperationPlanner                |
              +--> PackageAdapterPlanner                  |
    |                                                     |
    v                                                     |
WorkshopProjectPlan (immutable dry run)                   |
    |                                                     |
    v                                                     |
WorkshopTransactionCoordinator                            |
    +--> TransactionJournal (Library/)                    |
    +--> PackageManagerGateway --> Unity UPM              |
    +--> GenericProjectGenerator                          |
    +--> ExactSetupFacadeInvoker --> installed packages   |
    +--> BuildSceneListAdapter                            |
    +--> WorkshopValidator                                |
    +--> ManifestWriter / ReportWriter                    |
                                                          |
Generated Assets + WorkshopGenerationManifest <----------+
```

The plan builder is pure wherever practical. It consumes snapshots and definitions and returns an immutable ordered plan. Apply logic cannot silently add operations after the user approves a plan. If package resolution changes the actual graph, The Workshop pauses after reload, rebuilds the affected portion, displays the delta, and requires renewed approval before project generation continues.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent runtime root? | No |
| Editor authority | One active `WorkshopSession` per Workshop window/apply transaction |
| Duplicate behavior | Multiple read-only windows may inspect; only one mutating transaction may hold the project lock |
| Initialization trigger | Explicitly opening the Editor window or invoking an approved Editor API |
| Shutdown behavior | Cancel before irreversible phase when possible; persist journal; release project lock; never leave callbacks registered indefinitely |
| Direct-scene behavior | Not applicable; package has no runtime scene authority |
| Test injection seam | File system, package manager, clock, hashing, project settings, scene list, adapter invocation, and dialog services are interfaces |

### 8.4 Lifecycle sequence

1. **Open and inspect** - create a read-only project snapshot.
2. **Select** - choose project identity, root, preset, packages, bridges, and options.
3. **Validate selection** - resolve catalog compatibility, trust, and required choices.
4. **Build dry run** - produce immutable operations, risks, conflicts, and expected outputs.
5. **Review and approve** - user explicitly accepts package and project changes.
6. **Journal transaction** - write transaction ID, plan hash, current phase, and safe recovery data under `Library/`.
7. **Apply package phase** - submit one bounded Package Manager add/remove request where possible.
8. **Reload and reconcile** - after package resolution/domain reload, inspect resolved versions and adapter availability.
9. **Reapprove drift** - if actual resolution changes the approved plan materially, stop and show the delta.
10. **Apply generic project phase** - create root, folders, optional asmdefs, documentation shell, and manifest placeholder.
11. **Apply package setup phase** - invoke exact package setup facades in dependency/lifecycle order.
12. **Apply scene/project-setting phase** - create or modify only explicitly approved scene-list/project operations.
13. **Validate** - run Workshop and selected package validators.
14. **Publish manifest/report** - write durable records and unresolved manual actions.
15. **Complete** - mark journal complete and remove transient recovery data after a retention window.
16. **Repeat/repair/upgrade** - compare new plan against manifest and current project snapshot.

### 8.5 Operation model

Every `WorkshopOperation` has:

- Stable operation ID.
- Category and owning authority.
- Human-readable reason.
- Preconditions.
- Target package/path/setting.
- Expected before-state fingerprint when relevant.
- Expected output identity.
- Destructive classification.
- Requires-domain-reload flag.
- Reversibility classification.
- User-approval scope.
- Adapter/facade/schema version.
- Result and diagnostic code.

Approved operation categories:

| Category | Examples | Default safety |
|---|---|---|
| Inspect | Package list, paths, scenes, build profiles, roots | Read-only |
| Package add/remove | Add exact dependency, remove direct dependency | Explicit approval; dependency report |
| Create directory | `Assets/<Game>/Configuration` | Create only |
| Create generic asset | Identity/configuration marker, manifest | Create only |
| Create assembly definition | Project Runtime/Editor asmdefs | Create only; name conflict blocks |
| Invoke package setup | Create package-owned assets/prefabs/config | Package facade reports exact changes |
| Create scene | Boot/MainMenu/Game/Results | Create only; existing scene blocks/adopts |
| Modify scene list | Add/reorder selected generated scenes | Explicit before/after diff |
| Set serialized reference | Link generated assets | Only package-owned eligible target |
| Validate | Run checks | Read-only unless separate fix approved |
| Repair | Recreate missing eligible output | Explicit plan and manifest proof |
| Write report | Markdown/JSON under project docs | Create/versioned replace of Workshop-owned report only |

### 8.6 Transaction and domain-reload model

Unity Package Manager requests, compilation, and asset refresh can reload Editor assemblies. The Workshop therefore treats a composition as a resumable state machine rather than one long in-memory coroutine.

`WorkshopTransactionJournal` contains only non-sensitive recovery state:

- Transaction ID.
- Plan schema/version and plan hash.
- Project identifier and approved root.
- Current and completed phases.
- Approved package operations.
- Requested package specs.
- Resolved package snapshot after UPM.
- Pending reapproval reason.
- Generated operation receipts already settled.
- Last diagnostic result.
- Created backup/sandbox locations.

The journal lives beneath `Library/EchoGameStarter/Transactions`, is excluded from version control, and never becomes the sole durable record of completed generation. On successful completion, the project-owned manifest/report receives the final facts.

At Editor startup, The Workshop may detect an incomplete journal and offer **Resume**, **Inspect**, or **Abandon transaction record**. It must not resume mutation without user confirmation after an Editor restart. Abandoning the journal does not delete generated assets or packages; it produces recovery guidance.

### 8.7 Package setup facade architecture

The Workshop must not compile against every selected package and must not copy package setup code. The MVP uses an **exact named facade adapter**:

1. The approved `WorkshopPackageAdapterDefinition` identifies the package ID, compatible version range, exact Editor assembly-qualified facade type, facade schema version, and expected public methods.
2. After UPM resolution, The Workshop verifies the installed package ID/version and exact facade signature.
3. Invocation is limited to the listed type and methods. No broad assembly scan, convention guessing, or unknown plugin execution occurs.
4. Request and result data cross the boundary as package-owned JSON or other explicitly documented detached records.
5. The package facade remains the authority for its asset types, validation, idempotency, and repair.
6. The Workshop records the facade version and returned operation receipts.
7. A missing or incompatible facade blocks only that package’s automated setup and offers manual setup documentation.

Expected facade capabilities, when a package advertises them:

- Describe supported setup options and schema.
- Plan package-owned operations without mutation.
- Apply an approved package-owned plan.
- Validate package-owned output.
- Compare prior receipt/current state for repair.
- Produce package-specific removal guidance.

This contract is an implementation requirement to reconcile in FW-DOC-11. If implementation proves that at least three packages need a shared strongly typed Editor contract, a separate ADR may approve a tiny Editor-only contracts package. No such package is silently introduced by this specification.

### 8.8 Failure model

| Failure | Detection point | User-visible result | Safe behavior | Diagnostic code |
|---|---|---|---|---|
| Unsupported Unity version | Open/inspect | Blocking compatibility panel | No mutation | `EGS-COMP-001` |
| Project compiling or in Play Mode | Preflight | Busy/blocking result | Wait; no plan apply | `EGS-LIFE-001` |
| Another transaction owns lock | Apply | Existing transaction details | Read-only inspection only | `EGS-LIFE-002` |
| Package source unavailable | Package plan/apply | Source and package identified | Stop before asset generation | `EGS-PKG-001` |
| UPM request fails | Package phase | Exact UPM error and recovery steps | Journal preserved; no asset phase | `EGS-PKG-002` |
| Resolved version differs materially | Reconcile | Plan-drift diff | Require reapproval | `EGS-PKG-003` |
| Adapter missing/incompatible | Adapter validation | Package-specific automation unavailable | Keep package; offer manual setup | `EGS-ADP-001` |
| Existing path conflict | Plan/apply | Conflict browser | Create nothing at target | `EGS-CNF-001` |
| Project-modified generated item | Repeat/repair | Modified-retain status | Never overwrite by default | `EGS-CNF-002` |
| Path escapes approved root | Plan | Security blocker | Reject operation | `EGS-SEC-001` |
| Scene-list adapter ambiguity | Plan | Build Profile/global list choice required | No scene-list change | `EGS-SCN-001` |
| Package facade throws | Setup apply | Package, endpoint, operation reported | Stop dependent operations; journal state | `EGS-ADP-002` |
| Domain reload before journal | Apply preflight | Operation refused | Journal must exist first | `EGS-LIFE-003` |
| Validation fails | Final validation | Readiness report with blockers | Generation marked incomplete, not rolled back blindly | `EGS-VAL-001` |
| Report write fails | Finalize | Paths and error shown | Keep transaction receipts in journal | `EGS-REP-001` |
| Manifest schema newer | Inspect | Read-only unsupported record | Do not rewrite/downgrade | `EGS-MIG-001` |
| User cancels | Any cancellable phase | Cancelled result and settled operations | No new phase begins | `EGS-OP-001` |
| Cancellation too late | UPM/atomic package facade phase | Too Late result | Current bounded operation settles | `EGS-OP-002` |

## 9. Runtime Data and State Model

The package has no Player runtime model. This section defines Editor definitions, transient transaction state, and project-owned generation records.

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable during apply? | Project-owned instance? |
|---|---|---:|---:|---:|
| `WorkshopPresetDefinition` | Named preset, default selections, option schema, rationale | Yes | No | Package asset; project may clone custom preset later |
| `WorkshopPackageCatalog` | Approved package/source/version/bridge descriptors | Yes | No | Package default plus optional project override |
| `WorkshopPackageDescriptor` | One package identity, source policy, compatibility, docs, trust note | Yes | No | Usually package/catalog owned |
| `WorkshopBridgeDescriptor` | Optional bridge package and both peer requirements | Yes | No | Catalog owned |
| `WorkshopPackageAdapterDefinition` | Exact setup facade endpoint and schema compatibility | Yes | No | Workshop/package integration data |
| `WorkshopProjectTemplate` | Generic root/folder/asmdef/docs recipe | Yes | No | Package default; custom clone later |
| `WorkshopOptionDefinition` | Typed option, default, validation, visibility rule | Yes | No | Definition owned |
| `WorkshopValidationProfile` | Which post-generation checks are required | Yes | No | Definition/project choice |
| `WorkshopSourcePolicy` | Allowed source forms, exact-version rules, trust requirements | Yes | No | Package/project policy |

### 9.2 Editor runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `WorkshopSession` | Window/service | Window lifetime | Reinspect or close | Not durable |
| `WorkshopProjectSnapshot` | Inspector | One inspection revision | Replace atomically | Detached, testable record |
| `WorkshopCompositionSelection` | Session | Until changed/saved | User reset | May save as project preset asset |
| `WorkshopProjectPlan` | Plan builder | Immutable approved plan | Rebuild on changed snapshot/selection | JSON snapshot in journal/report |
| `WorkshopConflictSet` | Plan builder | Plan lifetime | Rebuild | Included in plan/report |
| `WorkshopTransaction` | Coordinator | Apply operation | Complete/abandon | Journal beneath Library |
| `WorkshopOperationReceipt` | Coordinator/facade | Transaction and final manifest | Append-only per transaction | Durable sanitized record |
| `WorkshopProjectLock` | Coordinator | Mutating transaction | Release on settle/reload recovery | Lock file + process/session metadata |
| `WorkshopProgressSnapshot` | Coordinator | Operation | Replace | Not required durable |

### 9.3 Durable project records

| Record | Location | Purpose | Project-owned? |
|---|---|---|---:|
| `WorkshopGenerationManifest` | `Assets/<Game>/Configuration/Workshop/` by default | Machine-readable origin, output, package, version, GUID, fingerprint, adoption, and status history | Yes |
| `WorkshopGenerationReport.md` | `Assets/<Game>/Documentation/Generated/` by default | Human-readable setup result, decisions, manual steps, removal guide | Yes |
| `WorkshopGenerationReport.json` | Same report folder | Support/automation-friendly sanitized result | Yes |
| `WorkshopProjectPreset` | Optional project configuration folder | Saves chosen options for repeat generation | Yes |
| `WorkshopRemovalGuide.md` | Generated docs | Explains package and asset removal classifications | Yes |

### 9.4 Stable identifiers

- Presets use stable IDs independent from display title.
- Catalog package identity is the UPM package ID plus source policy, not display name.
- Bridges use stable bridge IDs and explicit peer package IDs.
- Plans use a generated transaction/plan ID plus deterministic operation IDs derived from preset, operation owner, target identity, and schema version.
- Generated output records use Unity GUID when an asset exists, plus logical output ID and last known path.
- Paths are presentation/location data, not sole identity.
- A rename retains the GUID and updates manifest path on reconciliation.
- A generated scene or asset copied to a new GUID is treated as a new/adopted project item unless explicitly reconciled.
- Duplicate stable IDs block catalog/specification approval.

### 9.5 Ownership and fingerprint model

Every output receipt uses one classification:

| Classification | Meaning | Workshop behavior |
|---|---|---|
| Generated-managed | Created by approved operation and still matches last generated fingerprint | Eligible for safe no-change, repair, or explicit upgrade replacement |
| Generated-modified | Created by Workshop/package facade but fingerprint differs | Project-owned; preserve and require manual/side-by-side upgrade |
| Adopted-existing | Existed before generation and was explicitly adopted | Never overwrite/delete automatically |
| Package-owned | Lives beneath immutable package source | Not a generated project item |
| User-created | No Workshop origin | Never claim ownership |
| Missing-generated | Manifest records item but asset is absent | Eligible repair only when recipe/facade version is compatible |
| Unknown/orphaned | Manifest references unavailable adapter/package | Preserve record and offer manual guidance |

Fingerprints are advisory integrity signals, not security guarantees. Text files may use normalized content hashes. Unity assets use GUID, type, importer/serialized fingerprint where reliable, and package-facade-specific comparison. A changed fingerprint never authorizes destructive action.

### 9.6 ScriptableObject safety

Preset, catalog, adapter, and option assets are immutable definitions during an operation. The Workshop must not write current selection, progress, request handles, package resolution, or transaction state into package ScriptableObjects. Project preset copies may be edited deliberately, but live apply state remains detached.

### 9.7 Serialization and migration

- Plan, journal, manifest, report JSON, preset, and adapter schemas each declare versions.
- Transient journals support only current and explicitly approved previous schema versions; unsupported journals remain inspectable and abandonable.
- Durable generation manifests receive contiguous migrations with backup-before-write.
- Newer manifests open read-only; The Workshop must not downgrade them.
- Package facade receipts retain package ID/version, facade schema, and setup schema.
- Unknown receipt fields and unavailable package records are preserved when practical.
- Human-readable Markdown is regenerated from structured data and is not parsed as authority.

## 10. Public Editor API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `WorkshopWindow` | `EditorWindow` | Primary guided authoring UI | Unity Editor |
| `IWorkshopService` | Interface | Plan, apply, inspect, validate, repair, and report facade | Package service |
| `WorkshopProjectRequest` | Record/class | Project identity, root, preset, packages, and options | Caller |
| `WorkshopProjectSnapshot` | Immutable record | Current packages, scenes, roots, settings, and conflicts | Inspector |
| `WorkshopProjectPlan` | Immutable class | Approved ordered operations and plan hash | Plan builder |
| `WorkshopOperation` | Abstract record | One visible planned change | Plan builder |
| `WorkshopPlanResult` | Record | Success, blockers, warnings, conflicts, plan | Service |
| `WorkshopApplyResult` | Record | Final state, receipts, report, unresolved actions | Coordinator |
| `WorkshopTransactionStatus` | Enum | None, Planned, ApplyingPackages, AwaitingReload, AwaitingApproval, Generating, Validating, Completed, Failed, Cancelled, Abandoned | Coordinator |
| `WorkshopPackageCatalog` | ScriptableObject | Approved package and bridge data | Package/project |
| `WorkshopPresetDefinition` | ScriptableObject | Named preset and defaults | Package/project |
| `WorkshopGenerationManifest` | ScriptableObject or structured project asset | Durable generated-output record | Project |
| `WorkshopOperationReceipt` | Record | Settled operation evidence | Coordinator/facade |
| `WorkshopConflict` | Record | Existing state and available resolutions | Conflict analyzer |
| `WorkshopDiagnostic` | Record | Stable code, severity, owner, action | Any subsystem |
| `IWorkshopPackageManagerGateway` | Interface | List/add/remove package dependencies | Adapter/test seam |
| `IWorkshopFileSystem` | Interface | Safe project-relative file operations | Adapter/test seam |
| `IWorkshopAssetDatabase` | Interface | Asset creation, GUID/path, refresh, batching | Adapter/test seam |
| `IWorkshopSceneService` | Interface | Scene creation/save/inspection | Adapter/test seam |
| `IWorkshopBuildSceneListAdapter` | Interface | Global/Build Profile scene-list plan/apply | Adapter/test seam |
| `IWorkshopSetupFacadeInvoker` | Interface | Verify and invoke exact package setup endpoint | Adapter/test seam |
| `IWorkshopDialogService` | Interface | Confirmation/file choices without hard-coded UI | Adapter/test seam |
| `IWorkshopClock` | Interface | Timestamps/timeouts | Test seam |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/Editor rule |
|---|---|---|---|---|
| `InspectProject()` | Capture current read-only snapshot | Editor idle | Structured snapshot/result | Main Editor thread |
| `BuildPlan(WorkshopProjectRequest)` | Produce dry run | Valid snapshot/catalog | Immutable plan or blockers | Pure/main thread; no mutation |
| `ApplyPlanAsync(WorkshopProjectPlan, CancellationToken)` | Apply approved plan | Exact current plan hash; project lock available | Fresh async result; may pause across reload | Editor main thread coordination |
| `ResumeTransactionAsync(id, token)` | Resume explicit incomplete journal | Journal and project match | Resumes or reports mismatch | Explicit user confirmation after restart |
| `CancelTransaction(id)` | Request cancellation | Transaction active | Cancelled, Too Late, or Not Found | Does not abandon settled operations |
| `ValidateProjectAsync(profile, token)` | Run Workshop/peer validators | Project compiled; adapters available as needed | Structured readiness report | Main thread unless provider says detached |
| `BuildRepairPlan(manifest)` | Compare current state to prior generation | Supported manifest | Dry-run repair plan | No mutation |
| `BuildRemovalGuide(manifest)` | Classify removal effects | Manifest readable | Structured + Markdown guide | No mutation |
| `BuildUpgradePlan(manifest, targetPreset)` | Diff versions/options | Compatible definitions | Dry-run upgrade plan | No mutation |
| `ExportSupportSnapshot(path)` | Write redacted evidence | Explicit path/approval | Success or file diagnostic | Never includes credentials |
| `CurrentTransaction` | Inspect active transaction | None | Read-only snapshot | No mutation |
| `IsProjectMutationLocked` | Prevent competing operations | None | Boolean/reason | Editor state |

Public async methods return a fresh operation/awaitable instance per call. They never reuse Unity request objects or expose a mutable UPM request as the package’s public contract.

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `ProjectInspected` | Service | After snapshot published | Snapshot | Read-only |
| `PlanBuilt` | Service | After complete dry run | Plan result | No apply implied |
| `TransactionStateChanged` | Coordinator | After state transition | Status snapshot | May repeat after reload reconciliation |
| `OperationStarted` | Coordinator | Before one approved operation | Operation snapshot | Informational |
| `OperationSettled` | Coordinator | After receipt recorded | Receipt | State already authoritative |
| `PlanDriftDetected` | Coordinator | After UPM/rescan differs | Diff | Requires user decision |
| `ConflictDetected` | Planner/coordinator | After conflict recorded | Conflict | Listener cannot auto-resolve without explicit policy |
| `ValidationCompleted` | Validator | After report finalized | Readiness report | Generation may still be incomplete |
| `GenerationCompleted` | Coordinator | After manifest/report publication | Apply result | Does not mean every optional recommendation passed |
| `TransactionRecoveryAvailable` | Startup recovery | After journal detection | Recovery snapshot | Never auto-resumes mutation |

Events are raised after authoritative state is recorded. UI listeners are never required for a transaction to settle.

### 10.4 Async and cancellation policy

- Planning and comparison are synchronous/pure where practical.
- Package Manager operations are asynchronous and serialized.
- Only one mutating Workshop transaction runs per project.
- Cancellation is honored before a bounded operation begins and between phases.
- Package Manager requests and package-owned facade atomic operations may be Too Late once started.
- Domain reload is modeled as a pause/re-entry boundary, not cancellation.
- The transaction journal is written before any phase expected to reload assemblies.
- On cancellation or failure, settled package changes and generated outputs are reported; The Workshop does not pretend they never happened.
- Timeouts apply to facade/provider response and stale transaction locks, not to Unity compilation itself unless Unity provides reliable status.

### 10.5 API ergonomics

**Novice path:** Open The Workshop, choose Game Jam Quickstart, enter a project name/root, review the package and output plan, apply, resolve any manual items, and open the readiness report.

**Programmer path:** Construct a request, inspect/build plan through `IWorkshopService`, substitute gateways in tests, author custom project presets/catalog overrides, and consume structured reports without opening the window.

## 11. Editor Tooling and Authoring Experience

### 11.1 First-run workflow

1. Install The Workshop through supported UPM route.
2. Open **Sperk’s Forge > The Workshop - Project Starter**.
3. Review detected Unity version, project compilation state, package sources, installed Echo packages, active Build Profile/global scene-list state, and existing project roots.
4. Choose **Blank Modular Starter**, **Game Jam Quickstart**, or Custom.
5. Enter project display name, technical root folder, optional namespace/assembly names, and documentation location.
6. Select packages and visible bridges.
7. Configure package-facing options: startup, diagnostics, settings sections, scenes/routes, state model, audio profiles, input template, UI modules, and save model.
8. Build the dry run.
9. Review package graph, operation list, conflicts, destructive actions, and unresolved requirements.
10. Explicitly approve package changes.
11. Allow Unity Package Manager resolution and domain reload.
12. Review any resolved-version or adapter delta.
13. Explicitly approve project generation.
14. Run selected package setup facades.
15. Review validation and readiness results.
16. Open generated documentation, first scene, or package Test Labs.
17. Commit the generated output and report through the user’s normal Git workflow.

### 11.2 Window pages

| Page | Purpose | Mutates project? |
|---|---|---:|
| Welcome/Project Health | Explain authority and show current project readiness | No |
| Project Identity | Root, naming, namespace, assembly/document options | No |
| Preset | Choose Blank, Game Jam, or Custom | No |
| Packages | Select packages/bridges and source/version policy | No |
| Package Options | Collect adapter-described setup choices | No |
| Plan | Show every operation, reason, target, risk, and conflict | No |
| Apply | Confirm and execute bounded phases | Yes, explicit |
| Recovery | Resume/inspect/abandon incomplete transaction | Potentially, explicit |
| Validation | Run and inspect readiness checks | Read-only unless separate fix plan |
| Manifest/History | Inspect generated outputs and drift | No |
| Repair/Upgrade | Build dry-run repair or upgrade plans | No until approved |
| Removal Guide | Explain safe/manual package and asset removal | No |
| Laboratory | Run sandbox scenarios | Sandbox only by default |

### 11.3 Setup operations

| Operation | Creates/modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---:|---|---|
| Inspect project | Nothing | Yes | N/A | Snapshot diagnostics |
| Apply package graph | Project manifest/lock through UPM | Conditional | UPM/resolution record; reverse plan | Requested/resolved packages |
| Create root/folders | Project directories | Yes | Delete only if empty/new and operation fails before adoption | Folder receipts |
| Create asmdefs | Project assembly assets | Yes if unchanged | Backup/side-by-side on upgrade | GUID/path/reference receipt |
| Create docs shell | Markdown files | Yes if Workshop-owned unchanged | Versioned backup/diff | Docs receipt |
| Invoke setup facade | Package-owned declared outputs | Package contract | Facade-defined backup/receipt | Package operation receipts |
| Create scenes | Project scenes | Create-only safe | Save as new; no overwrite | Scene GUID/path receipt |
| Change scene list | Build Profile/global list | Yes with exact before-state | Snapshot previous list | Before/after list |
| Write manifest/report | Workshop-owned project records | Yes | Backup/migration | Final records |
| Repair | Eligible missing/unchanged outputs | Explicit | Plan-specific | Repair receipt |
| Removal guide | Nothing | Yes | N/A | Markdown/JSON guide |

### 11.4 Conflict resolutions

The user may choose only resolutions valid for a conflict type:

- **Keep existing and skip generation.**
- **Adopt existing** after package validator confirms compatibility.
- **Choose another root/name/path.**
- **Generate side-by-side candidate.**
- **Open package setup manually.**
- **Repair missing generated item.**
- **Replace unchanged generated-managed item** with explicit backup and approval.
- **Cancel the plan.**

“Overwrite anyway” is not a universal option. Package facades may expose a narrowly validated destructive migration, but the plan must identify it by name and preserve a backup where practical.

### 11.5 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| `EGS-VAL-001` | Unsupported Unity version | Blocker | No | No |
| `EGS-VAL-002` | Project has compile errors | Blocker | No | No |
| `EGS-VAL-003` | Approved root invalid/outside Assets | Blocker | Choose path | No |
| `EGS-VAL-004` | Duplicate package/catalog stable ID | Blocker | Edit catalog | No |
| `EGS-VAL-005` | Package source not approved/reachable | Blocker | Change source | No |
| `EGS-VAL-006` | Package version outside adapter compatibility | Blocker | Change version/adapter | No |
| `EGS-VAL-007` | Setup facade missing or wrong signature | Error | Manual setup/update adapter | No |
| `EGS-VAL-008` | Existing target path conflicts | Error | Skip/adopt/rename/side-by-side | No |
| `EGS-VAL-009` | Generated-managed asset missing | Warning | Repair plan | Yes after approval |
| `EGS-VAL-010` | Generated asset modified | Info/Warning | Manual merge/retain | No |
| `EGS-VAL-011` | Scene-list target ambiguous | Error | Select Build Profile/global target | No |
| `EGS-VAL-012` | Duplicate selected runtime authority | Blocker | Package validator/scene repair | Package-owned fix only |
| `EGS-VAL-013` | Required package validator fails | Blocker | Package-specific | Only separate approved fix |
| `EGS-VAL-014` | Report/manifest stale | Warning | Reconcile/regenerate | Yes if Workshop-owned unchanged |
| `EGS-VAL-015` | Workshop runtime assembly detected | Blocker | Packaging fix | No |
| `EGS-VAL-016` | Generated project depends on Workshop assembly | Blocker | Remove generated dependency | No |

## 12. Installation, Project Setup, and Direct Testing

### 12.1 Installation routes

Supported for MVP:

- Embedded package for Workshop development.
- Local path dependency.
- Local tarball installation.
- Git URL pinned to an approved tag or commit after release.
- Approved scoped registry when the registry strategy exists.

The Workshop does not install itself. It coordinates selected peer packages after it is already installed.

### 12.2 Minimal project setup

No scene or runtime GameObject is required. Minimum use requires:

1. Supported Unity Editor.
2. The Workshop package installed.
3. A compilable project.
4. An approved package catalog/source policy.
5. Write access to the project and `Library` directories.
6. User selection of a generated root beneath `Assets/`.

### 12.3 Production setup

Not applicable as a runtime setup. The Workshop’s output may include a production Boot scene and package roots only when those packages are selected. The Workshop package may then be removed.

### 12.4 Existing-project setup

Existing projects default to:

- A new side-by-side root.
- No package removal.
- No scene-list replacement.
- No project-setting overwrite.
- Existing package roots/configurations treated as adoption candidates only after the owning package validator confirms them.
- A generated integration report before old systems are removed.

### 12.5 Scene-list behavior

Unity 6 may use the global scene list or a Build Profile override. The Workshop uses `IWorkshopBuildSceneListAdapter` to inspect both and requires the user to choose the authoritative target when ambiguity exists. It records the complete before/after list. Generated scene order is not applied merely because filenames begin with numbers.

### 12.6 Project isolation rule

The Workshop must never require a specific existing project folder. Presets use tokens such as `<Game>`, `<Namespace>`, and `<Root>`. All token expansion is validated before planning. Absolute paths, `..`, package-cache paths, and paths outside approved roots are rejected unless an explicitly separate export operation is designed later.

## 13. Standalone Workshop Laboratory and Samples

### 13.1 Laboratory exception and purpose

The Workshop’s core behavior is Editor-time composition, so a runtime Standalone Test Lab scene would be decorative rather than meaningful. This specification records an approved scene-first exception: the package ships an isolated **Workshop Laboratory** Editor sample and disposable project fixtures instead of claiming a runtime scene proves generation safety.

The Laboratory proves:

- Plan generation without peer packages.
- Package graph and adapter validation through fake gateways.
- Generic project skeleton generation in a sandbox root.
- Conflict classification.
- Domain-reload journal resume simulation.
- Repeat-run and repair behavior.
- Manifest/report generation.
- Removal guidance.
- Clean removal of The Workshop after generated-output validation.

Generated scenes produced by Game Jam Quickstart are later tested as output and through the selected packages’ own Standalone and Integration Labs. They are not The Workshop’s standalone proof.

### 13.2 Required Laboratory contents

- `Workshop Laboratory` sample README.
- A dedicated UI Toolkit test window or Laboratory page.
- Sandbox root selector defaulting to `Assets/Workshop Laboratory Sandbox/`.
- Fake package manager gateway and package catalog.
- Fake setup facades that can succeed, warn, fail, reload, drift, or throw.
- Generic project-template fixtures.
- Existing/conflicting asset fixtures.
- Manifest versions and migration fixtures.
- Build scene-list adapter fixtures.
- Reset/cleanup control that only touches the sandbox and Laboratory-owned journals.
- Read-only mode against the real project.
- Exportable Laboratory result report.

### 13.3 Laboratory acceptance checklist

| Test | Action | Expected result | Type | Status |
|---|---|---|---|---|
| LAB-001 | Open package with no peers | Window and Blank preset operate | Manual/automated | Not run |
| LAB-002 | Build empty Blank plan | Generic skeleton only; no Echo peers | Automated | Not run |
| LAB-003 | Select Game Jam preset with fake peers | Every direct package/bridge is visible | Automated | Not run |
| LAB-004 | Reject plan | No project mutation | Automated | Not run |
| LAB-005 | Apply sandbox skeleton | Only sandbox root is created | Automated | Not run |
| LAB-006 | Repeat identical apply | No duplicates; No Change receipts | Automated | Not run |
| LAB-007 | Modify generated text asset | Repeat classifies Generated Modified | Automated | Not run |
| LAB-008 | Delete eligible generated asset | Repair plan identifies exact missing item | Automated | Not run |
| LAB-009 | Existing user asset at target | Conflict blocks overwrite | Automated | Not run |
| LAB-010 | Adopt compatible fake asset | Manifest records Adopted Existing | Manual/automated | Not run |
| LAB-011 | Simulate package resolution reload | Journal resumes at reconcile phase | Automated | Not run |
| LAB-012 | Simulate resolved version drift | Reapproval required | Automated | Not run |
| LAB-013 | Missing facade | Package setup blocks with manual guidance | Automated | Not run |
| LAB-014 | Facade throws | Dependent operations stop; journal/report survive | Automated | Not run |
| LAB-015 | Cancel before package phase | Cancelled with no mutation | Automated | Not run |
| LAB-016 | Cancel during too-late fake operation | Too Late; operation settles | Automated | Not run |
| LAB-017 | Competing transaction lock | Second apply rejected | Automated | Not run |
| LAB-018 | Restart with incomplete journal | Resume/Inspect/Abandon offered; no auto mutation | Manual/automated | Not run |
| LAB-019 | Invalid path traversal token | Plan blocks | Automated | Not run |
| LAB-020 | Symlink/junction escape fixture where supported | Unsafe target blocks | Platform manual | Not run |
| LAB-021 | Global scene list fixture | Before/after plan exact | Automated | Not run |
| LAB-022 | Build Profile override fixture | Correct authoritative target selected | Automated | Not run |
| LAB-023 | Ambiguous scene-list authority | User choice required | Automated | Not run |
| LAB-024 | Generate report | Markdown and JSON match receipts | Automated | Not run |
| LAB-025 | Report path conflict | No overwrite; alternate/resolve required | Automated | Not run |
| LAB-026 | Old manifest migration | Backup and contiguous migration succeed | Automated | Not run |
| LAB-027 | Newer manifest | Read-only unsupported state | Automated | Not run |
| LAB-028 | Removal guide unchanged assets | Safe removable list correct | Automated | Not run |
| LAB-029 | Removal guide modified assets | Modified items retained/manual | Automated | Not run |
| LAB-030 | Remove Workshop after sandbox generation | Generated fixture remains readable/compilable | Clean project | Not run |
| LAB-031 | Delete Laboratory sample | Workshop core compiles | Clean project | Not run |
| LAB-032 | Package validator warning | Generation completes with advisory status | Automated | Not run |
| LAB-033 | Package validator blocker | Generation marked incomplete | Automated | Not run |
| LAB-034 | UPM source unavailable fake | Asset generation never begins | Automated | Not run |
| LAB-035 | UPM partial graph change fake | Reconciliation reports exact state | Automated | Not run |
| LAB-036 | Redacted support export | No tokens, absolute private paths, or project content | Automated | Not run |
| LAB-037 | Large bounded plan | Window remains responsive and list virtualized | Performance/manual | Not run |
| LAB-038 | Asset batching failure | Stop/finally restores AssetDatabase editing state | Automated | Not run |
| LAB-039 | Editor quit during fake apply | Journal settles/recovery offered | Manual | Not run |
| LAB-040 | Reset Laboratory | Only sandbox and Laboratory journal removed | Automated | Not run |

### 13.4 Optional generated samples

| Sample | Packages involved | Purpose | Why it is not Workshop standalone proof |
|---|---|---|---|
| Blank Modular Output | Selected subset | Show minimal project structure and report | Output depends on selected package setup |
| Game Jam Quickstart Output | Selected Foundation packages | Show playable application shell | Combined integration evidence, not package independence |
| Existing Project Adoption Fixture | Workshop plus fake/project code | Demonstrate side-by-side generation | Migration scenario rather than clean standalone core |

Samples are removable and contain no restricted content.

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The Workshop’s Editor UI is core to the package because informed review and explicit approval are part of the safety contract. The UI presents plans and results; the structured service and plan remain authoritative. Closing the window must not corrupt an active journal or make a completed operation depend on view state.

### 14.2 Required UI states

- Project inspection in progress.
- Ready with no active transaction.
- Unsupported Unity/project state.
- Preset/custom selection.
- Package-source approval required.
- Dry-run plan ready.
- Conflicts requiring resolution.
- Applying package changes.
- Waiting for Unity resolution/compilation/domain reload.
- Resolved-plan drift requiring reapproval.
- Generating project output.
- Validation in progress.
- Completed with pass/advisory/blocker summary.
- Failed with settled-operation report.
- Cancelled.
- Recovery journal available.
- Read-only newer manifest.
- Empty catalog or no package selection.

### 14.3 Accessibility requirements

- Full keyboard navigation for all Workshop pages and dialogs implemented inside the window.
- Logical tab order and visible focus indicators.
- Text labels for all icons, package states, risks, conflicts, and operation statuses.
- Color-independent status using text, icons, and severity labels.
- Resizable window and layouts that remain usable at supported Editor scaling.
- Virtualized long operation/package lists to preserve responsiveness and navigation.
- No essential information only in hover tooltips.
- Reduced or disabled decorative animation; progress never relies on motion.
- Copyable diagnostic codes, paths, package IDs, and manual actions.
- Confirmation copy names the exact package/path/setting affected.
- Screen-reader-friendly Editor labels where Unity UI Toolkit accessibility support permits; limitations are documented honestly.

### 14.4 Visual customization

The Workshop ships an EchoDevGames/Sperk’s Forge visual identity appropriate for Editor tooling, but project generation is not affected by the selected window theme. Package icons and preset illustrations are optional presentation assets. The UI must remain legible in Unity light and dark Editor themes.

### 14.5 Confirmation design

Confirmations are scoped, not one giant “do everything” button:

1. Package source/trust changes.
2. Package add/remove graph.
3. Any project-setting or scene-list change.
4. Any operation classified Replace, Delete, Move, Migrate, or Prune.
5. Resuming mutation after Editor restart.
6. Repair or upgrade plan application.

Create-only operations may be approved as one grouped phase after the full list is visible.

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

The Workshop must remain diagnosable without The Observatory.

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Installed/requested/resolved package graph | Window/report/API | Editor | Low after UPM list |
| Project snapshot revision/hash | Window/API | Editor | Low |
| Active transaction and phase | Window/journal/API | Editor | Low |
| Plan operations and approvals | Plan/report/API | Editor | Bounded by plan size |
| Adapter/facade compatibility | Window/report | Editor | Low |
| Conflict set | Window/report/API | Editor | Bounded |
| Operation receipts | Window/manifest/report | Editor | Bounded history |
| Generated-output drift | Manifest comparison | Editor | Explicit scan |
| Validation results | Window/report/API | Editor | Explicit run |
| Support snapshot | File export | Editor | Explicit only |

### 15.2 Structured status

Expose:

- Workshop package/version and Unity version.
- Project identity and approved generated root.
- Installed Echo package IDs, requested specs, resolved versions, and source classes.
- Active Build Profile/global scene-list state without private machine paths.
- Current preset ID/version and project option schema.
- Plan ID/hash/revision and operation counts by category/risk.
- Active transaction ID/phase/start time/last settled operation.
- Package adapter/facade versions and compatibility.
- Conflict counts and unresolved blockers.
- Generated/adopted/modified/missing output counts.
- Validation pass/advisory/error/blocker counts.
- Report/manifest schema versions.

### 15.3 Diagnostic code families

| Prefix | Meaning |
|---|---|
| `EGS-COMP-*` | Unity/package compatibility |
| `EGS-LIFE-*` | Session, transaction, reload, and lock lifecycle |
| `EGS-PKG-*` | Package catalog, source, resolution, add/remove |
| `EGS-PLAN-*` | Selection and dry-run plan |
| `EGS-ADP-*` | Package setup adapters/facades |
| `EGS-GEN-*` | Generic/package generation |
| `EGS-CNF-*` | Conflicts, adoption, drift, ownership |
| `EGS-SCN-*` | Scenes and build scene lists |
| `EGS-VAL-*` | Readiness validation |
| `EGS-MAN-*` | Generation manifest |
| `EGS-REP-*` | Reports and support export |
| `EGS-MIG-*` | Manifest/preset/catalog migration |
| `EGS-SEC-*` | Paths, trust, credentials, unsafe sources |
| `EGS-OP-*` | Operation cancellation/result |
| `EGS-LAB-*` | Laboratory fixture/sandbox |

### 15.4 Observatory bridge

The Workshop is Editor-only, while The Observatory is primarily runtime diagnostics. A mandatory bridge is unnecessary. Later, an Editor integration may allow The Observatory’s validation vocabulary or package-health providers to appear in Workshop readiness reports, but The Workshop’s own diagnostics remain complete. Generated projects may select and configure The Observatory through its package setup facade.

### 15.5 Logging policy

- Every warning/error names the owning operation, package, target, and next action.
- No per-frame logging.
- UPM errors are wrapped with Workshop context without hiding Unity’s original message.
- Package facade exceptions include facade identity and operation ID, not full sensitive payloads.
- Normal successful generation writes a bounded summary, not thousands of Console lines.
- Verbose development logs are opt-in and excluded from support exports by default.
- Absolute user paths are redacted to project-relative paths where possible.

### 15.6 Report truth rule

A generation report distinguishes:

- Requested.
- Approved.
- Attempted.
- Resolved by Unity.
- Created.
- Adopted.
- Skipped.
- Failed.
- Manually required.
- Validated.

It must never collapse those into one “installed” or “success” label.

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Durable? | Backend |
|---|---|---|---:|---|
| Package catalog/presets/adapters | Package/project definition | Workshop | Yes | ScriptableObject/package assets |
| User-local window layout/recent page | User-local Editor preference | Workshop | Optional | `EditorPrefs` only for non-authoritative UI convenience |
| Current selection | Session/project | Workshop/user | Optional | Project preset asset when explicitly saved |
| Active transaction journal | Project-local transient | Workshop | Until settle/recovery | `Library/EchoGameStarter/Transactions` |
| Generation manifest | Project | Consumer project | Yes | Project asset under generated root |
| Generation report/removal guide | Project docs | Consumer project | Yes | Markdown/JSON under generated root |
| Game saves | Player runtime | Chronicle | Not applicable | Never handled by Workshop |
| Global player preferences | Player runtime | Accord | Not applicable | Never handled by Workshop |

### 16.2 Standalone behavior

The Workshop uses no EchoSave or Accord runtime storage. It does not create player data. It may generate configuration assets for those packages when selected. The transaction journal is Editor project state, not a game save.

### 16.3 Project settings and user settings

- Project composition choices that affect team output belong in project assets/reports and should be committed.
- Window size, last selected tab, and other harmless per-user convenience may use `EditorPrefs` but never determine generated truth.
- Package registry credentials and tokens remain in Unity/OS-supported configuration and are never copied into Workshop assets or reports.
- The Workshop does not edit `Packages/manifest.json` directly in the normal path; it uses the supported Package Manager client.

### 16.4 Failure and recovery

- Missing journal: inspect current project and prior durable manifest; do not guess a transaction.
- Corrupt journal: quarantine/rename it and provide manual reconciliation.
- Older journal: migrate only when supported; otherwise inspect/abandon.
- Newer journal: read-only diagnostic.
- Locked Library: block mutation.
- Missing manifest: generation can be inspected as untracked existing output; no destructive repair/removal assumptions.
- Corrupt manifest: preserve file, restore from Workshop-created backup if available, or rebuild only from explicit adoption review.

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

The Workshop is the one package whose purpose is to coordinate optional package selection, but coordination does not grant runtime authority. Every integration is Editor-only, visible in the plan, removable, versioned, and validated. Package core assemblies never depend on The Workshop merely to function.

### 17.2 Foundation setup integrations

| Package | Workshop choices | Package remains authority for | Required? |
|---|---|---|---:|
| First Light | Boot/preload pattern, splash/status choice, steps, destination, root lifetime | Launch config, root, steps, report, Boot scene validation | No |
| Observatory | Validation level, overlay off/dev/player/custom, root/config | Providers, overlay, validation, diagnostics | No; recommended with First Light |
| Accord | Audio/display/accessibility sections, storage path/policy defaults | Settings model, transactions, persistence, display safety | No |
| Passage | Scene definitions, routes, transition presentation hooks, scene list | Runtime scene travel and validation | No |
| Pulse | Primary states, override policies, pause/cursor defaults | Runtime state/pause authority | No |
| Resonance | Mixer template, buses, root/config, profile families, Audio Lab import | Audio playback and runtime state | No |
| Will | Input asset template, contexts, control schemes, glyph library, root/config | Input contexts, devices, rebinding, overrides | No |
| Looking Glass | Root/layers, theme, selected screen/HUD templates, EventSystem policy | Screen/modal/focus/UI authority | No |
| Chronicle | Save model, slot policy, retention, root/config, sandbox/sample participant | Files, slots, participants, migration, recovery | No |

### 17.3 Expected lifecycle order for composition

The Workshop plan respects package authority and expected setup dependencies without creating runtime hard dependencies:

1. Generic project identity/root/assemblies/docs.
2. Package installation and bridge resolution.
3. First Light and Observatory definitions needed for origin/diagnostics.
4. Accord, Passage, Pulse core configurations.
5. Resonance, Will, and Looking Glass project assets.
6. Chronicle configuration if selected.
7. Package-specific integration/bridge setup.
8. Scenes, scene definitions, routes, startup steps, UI presenters, and final references.
9. Build scene list.
10. Validators and report.

The exact operation graph comes from package facade plans. This list is a default ordering policy, not permission for The Workshop to edit peer internals.

### 17.4 Adapter descriptor contract

A descriptor records:

- Stable adapter ID/version.
- Target package ID and supported version range.
- Optional bridge package IDs.
- Exact facade type/assembly identity.
- Supported facade/setup schema versions.
- Required planning and apply endpoint signatures.
- Whether operation can reload/domain-reload.
- Declared output categories.
- Documentation fallback URL/path.
- Trust/owner identity.

Descriptors are approved package data. Project-added descriptors are clearly marked unverified and require explicit trust approval.

### 17.5 Integration failure behavior

- Missing peer: selection is unavailable or package operation is planned.
- Missing source: package phase blocks before generation.
- Incompatible version: plan blocks; no reflection invocation.
- Missing facade: package stays installed if UPM succeeded; automated package setup blocks with manual guide.
- Facade plan differs from previously approved adapter expectations: reapproval required.
- Facade apply fails: dependent operations stop; unrelated already-settled outputs remain reported.
- Peer package removed later: manifest keeps orphaned receipts; removal guide explains remaining project assets.
- Workshop removed: peer packages and generated project assets continue normally.

### 17.6 Bridge placement decision

The Workshop core contains no direct references to peer package assemblies. Exact facade adapters may ship:

1. As Workshop-owned descriptor data when the endpoint is stable and no code is needed.
2. As tiny Editor-only bridge packages when code is required and clean versioning/removal benefits.
3. As project-local adapters for game-specific composition.

No peer core package is made dependent on The Workshop. FW-DOC-11 decides which approved Foundation specs require addenda for public Editor setup facades.

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Target | Measurement | Release threshold |
|---|---|---|---|
| Window open/project basic inspection | Under 500 ms for ordinary project excluding UPM refresh/compile | Workshop Laboratory/real projects | No long unresponsive stall without progress |
| Dry-run plan for 500 operations | Under 250 ms after snapshots available | EditMode benchmark | 95th percentile under 500 ms |
| Long list UI | Virtualized and interactive | 1,000 operation fixture | No multi-second layout stall |
| Manifest comparison for 1,000 outputs | Under 1 second excluding asset import | Benchmark fixture | Advisory above threshold |
| Idle allocation | No recurring allocation/update loop while window inactive | Profiler | No per-frame service work |
| Apply memory | Bounded plan/receipt history; no full binary asset copies in memory | Stress fixture | No unbounded growth |
| Report size | Bounded/summarized with optional detailed attachment | 1,000 operation fixture | Main Markdown remains navigable |

Package download, Git/network latency, compilation, domain reload, and Unity asset import time are measured and reported but are not claimed as Workshop-controlled performance.

### 18.2 Allocation and batching policy

- Planning uses immutable/bounded collections and avoids repeated full-project scans.
- AssetDatabase batching may be used only around compatible asset operations and always within `try/finally` to call `StopAssetEditing`.
- Package Manager operations are never wrapped inside AssetDatabase editing batches.
- Large lists use UI Toolkit virtualization.
- File hashes are computed on demand and cached by snapshot revision.
- Binary assets are not duplicated into memory merely for diff display.
- Reflection endpoint metadata is cached only after exact adapter validation and cleared on domain reload.

### 18.3 Scene and domain reload behavior

- No static mutable state is the sole transaction authority.
- `[InitializeOnLoad]` recovery code only detects journals and schedules a safe prompt/state refresh; it does not auto-resume mutation.
- Event subscriptions unregister on window/service disposal.
- Project lock includes stale-lock detection and process/session evidence.
- Enter Play Mode blocks new mutations; active transaction behavior is explicit and normally prevents Play Mode until settled.
- Assembly reload callbacks flush journal state before reload where possible.

### 18.4 Scalability limits

MVP advertised/tested limits:

- Up to 64 selected package/bridge entries in a custom plan.
- Up to 2,000 planned operations.
- Up to 10,000 manifest output records before requiring archive/split guidance.
- Up to 50 retained generation histories in one manifest by default.
- One active mutating transaction.
- One pending recovery journal per project; additional journals are quarantined as conflicts.
- Package/facade timeouts are configurable and bounded; compilation waits are state-based rather than a fixed short timeout.

Exceeding a limit produces an actionable blocker/advisory, never silent truncation of destructive planning data.

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

The Workshop handles project paths, package sources, versions, generated asset identities, settings choices, and reports. It must not collect or export:

- Registry authentication tokens.
- Git credentials, SSH keys, credential-helper data, or private key paths.
- Full environment variables.
- User home directories when a project-relative path is sufficient.
- Game save payloads or player data.
- Typed input histories.
- Production content text merely to produce a support report.
- Source-code contents unless the user explicitly exports a specific diff later.

### 19.2 Trust boundaries

- Installing a package executes code in the Unity Editor. Every non-built-in source receives a visible trust warning and explicit approval.
- Built-in EchoDevGames catalog entries identify publisher, package ID, source class, and exact version policy.
- Project catalog overrides are marked project-provided/unverified.
- Exact adapter endpoints are allowlisted; no arbitrary plugin scan or method invocation.
- JSON/request data passed to facades is bounded and schema-validated.
- Paths are canonicalized, project-relative, and checked against approved roots.
- Symbolic links/junctions that escape approved roots are rejected where detection is supported; platform limitations are documented.
- Report filenames and project names are sanitized separately from display names.
- Package IDs and versions are validated before use in UPM requests.

### 19.3 Package source policy

Recommended presets must use one of:

- Approved registry package ID and exact version.
- Git URL pinned to approved tag or commit.
- Explicit local development path selected by the developer.
- Embedded package already present in the workspace.

Branches and floating ranges may be allowed only in an explicit development mode with a warning and a resolved-commit/version receipt. The Workshop never writes registry credentials.

### 19.4 Platform behavior

| Platform | Supported? | Special behavior | Validation required |
|---|---:|---|---|
| Windows Editor | Yes | Path normalization, junction checks, file locking | Primary baseline |
| macOS Editor | Planned/Yes after test | Case sensitivity and symlink behavior | Clean-project validation |
| Linux Editor | Planned/Yes after test | Case sensitivity, symlink, executable/tool assumptions | Clean-project validation |
| WebGL Player | Not applicable | Editor-only package | Generated peer packages test separately |
| Mobile Player | Not applicable | Editor-only package | Generated peer packages test separately |
| Console Player | Not applicable/unknown | Editor-only; package source/platform restrictions may apply | Provider/platform approval |
| Batchmode/CI | Deferred | UI/Package Manager/compilation constraints require research | Dedicated headless specification |

### 19.5 Destructive safety

- Deletions and replacements require prepared plans and explicit confirmation.
- Backups go to a reported project-local or Library transaction backup, never a hidden permanent cache.
- A backup is not claimed successful until verified.
- Failure to create a required backup blocks the destructive operation.
- Removing an operation from a plan invalidates dependent operations and rebuilds the plan.

## 20. Package and Repository Structure

### 20.1 Approved package anatomy

The Workshop is Editor-only. The normal Runtime directory and runtime assembly are intentionally omitted.

```text
Packages/com.echodevgames.echo-game-starter/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   │   ├── Installation.md
│   │   ├── Five-Minute-Quickstart.md
│   │   ├── Blank-Modular-Starter.md
│   │   ├── Game-Jam-Quickstart.md
│   │   ├── Existing-Project-Adoption.md
│   │   ├── Recovery-and-Repair.md
│   │   └── Removal-Guide.md
│   └── Developer/
│       ├── Architecture.md
│       ├── Setup-Facade-Adapter-Contract.md
│       ├── Preset-and-Catalog-Authoring.md
│       ├── Testing.md
│       ├── Release.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Editor/
│   ├── Core/
│   ├── Data/
│   ├── Planning/
│   ├── Transactions/
│   ├── PackageManagement/
│   ├── Generation/
│   ├── Adapters/
│   ├── Validation/
│   ├── Reporting/
│   ├── Recovery/
│   ├── UI/
│   └── EchoDevGames.EchoGameStarter.Editor.asmdef
├── Samples~/
│   └── Workshop Laboratory/
└── Tests/
    └── Editor/
        ├── Unit/
        ├── Integration/
        ├── Fixtures/
        └── EchoDevGames.EchoGameStarter.Tests.Editor.asmdef
```

### 20.2 Proposed source tree

```text
Editor/
├── Core/
│   ├── WorkshopService.cs
│   ├── WorkshopSession.cs
│   ├── WorkshopResult.cs
│   ├── WorkshopDiagnostic.cs
│   └── WorkshopConstants.cs
├── Data/
│   ├── WorkshopPresetDefinition.cs
│   ├── WorkshopPackageCatalog.cs
│   ├── WorkshopPackageDescriptor.cs
│   ├── WorkshopBridgeDescriptor.cs
│   ├── WorkshopPackageAdapterDefinition.cs
│   ├── WorkshopProjectTemplate.cs
│   ├── WorkshopOptionDefinition.cs
│   └── WorkshopGenerationManifest.cs
├── Planning/
│   ├── WorkshopProjectInspector.cs
│   ├── WorkshopProjectSnapshot.cs
│   ├── WorkshopPlanBuilder.cs
│   ├── WorkshopProjectPlan.cs
│   ├── WorkshopOperation.cs
│   ├── WorkshopConflictAnalyzer.cs
│   ├── WorkshopConflict.cs
│   ├── WorkshopPackageGraphResolver.cs
│   └── WorkshopPlanHasher.cs
├── Transactions/
│   ├── WorkshopTransactionCoordinator.cs
│   ├── WorkshopTransactionJournal.cs
│   ├── WorkshopProjectLock.cs
│   ├── WorkshopOperationReceipt.cs
│   ├── WorkshopRecoveryService.cs
│   └── WorkshopCancellationPolicy.cs
├── PackageManagement/
│   ├── IWorkshopPackageManagerGateway.cs
│   ├── UnityPackageManagerGateway.cs
│   ├── WorkshopPackageRequestBuilder.cs
│   ├── WorkshopPackageResolutionSnapshot.cs
│   └── WorkshopPackageSourceValidator.cs
├── Generation/
│   ├── WorkshopGenericProjectGenerator.cs
│   ├── WorkshopFolderGenerator.cs
│   ├── WorkshopAssemblyGenerator.cs
│   ├── WorkshopDocumentationGenerator.cs
│   ├── WorkshopSceneGenerator.cs
│   ├── WorkshopBuildSceneListAdapter.cs
│   └── WorkshopSafePath.cs
├── Adapters/
│   ├── IWorkshopSetupFacadeInvoker.cs
│   ├── ExactWorkshopSetupFacadeInvoker.cs
│   ├── WorkshopFacadeSignatureValidator.cs
│   ├── WorkshopFacadeRequestEnvelope.cs
│   └── WorkshopFacadeResultEnvelope.cs
├── Validation/
│   ├── WorkshopValidator.cs
│   ├── WorkshopValidationProfile.cs
│   ├── WorkshopReadinessReport.cs
│   └── Rules/
├── Reporting/
│   ├── WorkshopManifestWriter.cs
│   ├── WorkshopReportWriter.cs
│   ├── WorkshopRemovalGuideBuilder.cs
│   ├── WorkshopSupportSnapshotExporter.cs
│   └── WorkshopRedactionPolicy.cs
├── Recovery/
│   ├── WorkshopRepairPlanBuilder.cs
│   ├── WorkshopUpgradePlanBuilder.cs
│   ├── WorkshopManifestMigrator.cs
│   └── WorkshopBackupService.cs
└── UI/
    ├── WorkshopWindow.cs
    ├── WorkshopWindowState.cs
    ├── Pages/
    ├── Controls/
    └── Resources/
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoGameStarter.Editor` | Editor only | Unity Editor, UI Toolkit Editor modules, package management APIs | Yes | Workshop core and UI |
| `EchoDevGames.EchoGameStarter.Tests.Editor` | Editor tests | Workshop Editor assembly, Unity Test Framework | No | Unit/integration/Laboratory tests |

No runtime assembly is produced. Package-specific adapter code, when required, ships in separate Editor-only bridge assemblies/packages or project adapters.

### 20.4 Repository files

- Concise README with authority/non-goals.
- Complete `Documentation~` suite.
- Package catalog/preset compatibility table.
- Setup facade adapter contract.
- Recovery and manual reconciliation guide.
- Known limitations.
- Changelog.
- License and third-party notices.
- Security/trust guidance for package sources.
- Contribution/development guidance if public contributions are accepted.
- Release checklist.
- Stable `.meta` files and GUIDs.

### 20.5 Generated project anatomy

Default Game Jam Quickstart proposal:

```text
Assets/<Game>/
├── Scenes/
│   ├── 00_Boot.unity          # only when First Light selected
│   ├── 01_MainMenu.unity      # only when UI/menu selected
│   ├── 02_Game.unity
│   └── 03_Results.unity       # optional
├── Configuration/
│   ├── Launch/
│   ├── Diagnostics/
│   ├── Settings/
│   ├── SceneFlow/
│   ├── GameState/
│   ├── Audio/
│   ├── Input/
│   ├── UI/
│   ├── Save/
│   └── Workshop/
│       └── WorkshopGenerationManifest.asset
├── Audio/
├── UI/
├── Input/
├── Save/
├── Runtime/
├── Editor/
├── Tests/
│   ├── Standalone Labs/
│   └── Integration Labs/
└── Documentation/
    └── Generated/
        ├── WorkshopGenerationReport.md
        ├── WorkshopGenerationReport.json
        └── WorkshopRemovalGuide.md
```

Folders appear only when selected output requires them. Empty decorative directories are not created solely to match this diagram.

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Primary tested | Notes |
|---|---|---|---|
| Unity | 6000.0 | 6000.3.8f1 | Public floor remains subject to clean-project validation |
| Unity Package Manager Client API | Unity 6000.0 supplied | 6000.3.8f1 | Exact request behavior validated per supported Editor |
| UI Toolkit Editor APIs | Unity 6000.0 supplied | 6000.3.8f1 | Runtime UI Toolkit is unrelated |
| Foundation peer packages | Adapter/catalog-defined | Approved released versions | No peer is a core dependency |
| Git/scoped registry/local package sources | Unity-supported | Per release matrix | Credentials remain outside Workshop |

### 21.2 Semantic versioning policy

**Patch:**

- Fixes planning, UI, validation, redaction, reporting, or generation behavior without changing public operation/preset/manifest/facade schemas.
- Adds backward-compatible diagnostic codes or validator checks.
- Corrects documentation or compatible catalog metadata.

**Minor:**

- Adds optional presets, package descriptors, operations, report fields, or adapter capabilities while preserving old plans/manifests.
- Adds backward-compatible project template options.
- Adds support for another compatible Unity 6 version.

**Major:**

- Breaks public Editor API.
- Changes plan, manifest, journal, preset, catalog, or facade adapter schemas without automatic migration.
- Changes generated asset identity or default ownership rules.
- Removes a preset/operation relied upon by existing generation records.
- Changes safety defaults from create-only/preserve to destructive behavior.

Catalog-only source/version changes that alter resolved package output require a visible catalog revision and changelog even when Workshop code is unchanged.

### 21.3 Deprecation policy

- Public API, preset IDs, option IDs, catalog IDs, adapter IDs, operation IDs, and manifest fields receive at least one minor-release deprecation window when practical.
- Deprecated presets remain readable for upgrade/removal guidance during the support window.
- Removed package sources or adapters keep a tombstone record explaining migration.
- Generated project assets are never deleted because a preset is deprecated.
- A package facade endpoint change requires compatible adapter overlap or a documented manual migration.
- Breaking generation changes require side-by-side preview and release notes.

### 21.4 GUID and asset compatibility

- Public preset, catalog, template, UI, sample, and test assets preserve `.meta` files.
- Generated project asset GUIDs are created once and recorded; reruns reference existing GUIDs rather than regenerate them.
- Moving a generated asset retains GUID and manifest reconciliation updates path.
- Template asset GUIDs are not copied into project output when Unity requires unique identity; the generation receipt records template origin separately.
- Package update must not overwrite project-owned generated instances.

### 21.5 Catalog and adapter compatibility

The catalog records:

- Workshop version range.
- Target package version range.
- Adapter schema version.
- Setup facade schema version.
- Preset compatibility.
- Known incompatible combinations.
- Last validation date and Unity version.

A package may install successfully yet remain blocked for automated setup when adapter compatibility is unknown.

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview, authority, and explicit non-goals.
- Installation routes.
- Five-minute Blank Modular Starter quick start.
- Game Jam Quickstart guide.
- Custom package-selection guide.
- Package source/trust explanation.
- Dry-run plan and conflict-resolution guide.
- Existing-project adoption guide.
- Package-resolution/domain-reload recovery guide.
- Generated manifest/report guide.
- Repeat-run and repair guide.
- Removal guide.
- Upgrade diff guide.
- Workshop Laboratory guide.
- Diagnostic code reference.
- Known limitations.
- License, credits, and third-party notices.

### 22.2 Required developer/package-author documentation

- Architecture and transaction lifecycle.
- Plan/operation/result schemas.
- Package catalog and preset authoring.
- Exact setup facade adapter contract.
- Package facade planning/apply/idempotency requirements.
- Path, ownership, fingerprint, and conflict rules.
- Build scene-list adapter behavior.
- Manifest/journal migration.
- Testing and fixture architecture.
- Release workflow and compatibility matrix.
- Security/redaction policy.
- Current checkpoint/status and linked `Current Notes.md`.

### 22.3 Documentation truth rule

- Every screenshot/menu path matches supported Unity and current UI.
- Every package/preset list matches the shipped catalog.
- Generated sample reports are labeled examples, not guarantees.
- The quick start names every package selected by the preset.
- Manual fallback steps are tested.
- A setup facade advertised as automated must pass its adapter tests.
- The report schema and diagnostic code tables match implementation.

### 22.4 Living repository and Obsidian workflow

The Workshop repository follows SFGSS-000/SFGSS-001 documentation-as-code rules. Discoveries enter `Current Notes.md`, then durable changes move into this specification, ADRs, adapter contracts, catalog compatibility, tests, guides, changelog, and release records.

At meaningful checkpoints:

1. Reconcile current notes.
2. Promote package-author contract changes.
3. Update preset/catalog compatibility.
4. Update fixture reports and diagnostic codes.
5. Verify generated examples against the current build.
6. Commit documentation with or adjacent to implementation.

### 22.5 Generated documentation rules

Generated reports are project records, not package documentation. They must:

- Use project-relative links where practical.
- Name generated package IDs/versions.
- Link to package setup/troubleshooting docs.
- Distinguish automatic output from manual next steps.
- Avoid absolute private paths and credentials.
- Be safe to commit unless the user deliberately includes sensitive project metadata.
- State the plan and report schema versions.

### 22.6 Repository scan and handoff order

Before changing The Workshop:

1. Repository README.
2. SFGSS-000.
3. This specification.
4. FW-DOC-11 contract matrix and relevant ADRs.
5. Package setup facade adapter contract.
6. `Current Notes.md`.
7. Current checkpoint, tests, issue log, catalog compatibility, changelog.
8. Relevant Editor implementation and fixtures.
9. Affected peer package specifications/setup contracts.

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | Plan purity, IDs, paths, conflicts, schemas, redaction, migrations | Token/path validation, operation ordering, hash stability | Yes |
| Editor integration | AssetDatabase, scenes, build lists, reports, journals | Sandbox generation and repair | Yes |
| Fake UPM integration | Request sequencing, reload state, resolution drift | Package gateway fixtures | Yes |
| Exact facade integration | Signature validation, plan/apply/result/error behavior | Fake and real package adapters | Yes |
| Workshop Laboratory | User-visible Editor flow and failure simulation | Forty LAB cases | Yes |
| Temporary clean project | Installation, package resolution, generation, compile, removal | Blank and Game Jam outputs | Yes |
| Existing-project fixture | Conflicts, adoption, side-by-side output | Migration/adoption matrix | Yes |
| Peer package validation | Selected package setup outputs and validators | All advertised Foundation integrations | Required before integration claim |
| Performance/stress | Large plans/manifests/catalogs and UI virtualization | 2,000 operations, 10,000 receipts | Yes |
| Platform Editor matrix | Windows/macOS/Linux behavior | Paths, symlinks, file locks, UPM | Before platform support claim |

### 23.2 Required test categories

- Clean install and removal.
- Editor-only assembly/build exclusion.
- Blank preset with no peers.
- Game Jam preset package visibility.
- Custom selection combinations.
- Package source trust and exact version policy.
- Package Manager success/failure/resolution drift.
- Domain reload and Editor restart recovery.
- Project lock and competing transaction behavior.
- Path/token security.
- Existing path/GUID/root conflicts.
- Create-only generation.
- Package facade signature/plan/apply/failure.
- Scene and Build Profile behavior.
- Repeat runs, modified assets, repair, upgrade diff, and removal guide.
- Manifest/journal migrations.
- Validation and support redaction.
- Workshop removal after generation.
- Sample removal.
- Large plan/manifest performance.

### 23.3 Test case registry

| EGS-T-001 | Editor-only package | Clean Unity project | Install local/tarball package | Compiles with no runtime assembly or Player code | Yes | Not run |
| EGS-T-002 | Embedded development | Package embedded | Open Workshop | Window loads and package paths are correct | Yes | Not run |
| EGS-T-003 | Git installation | Approved tagged Git source | Install package | Exact release resolves and docs are available | Yes | Not run |
| EGS-T-004 | Remove Workshop | Generated Blank output exists | Remove Workshop package | Generated project remains compilable | Yes | Not run |
| EGS-T-005 | Remove sample | Workshop installed | Delete imported Laboratory sample | Core package compiles | Yes | Not run |
| EGS-T-006 | Unsupported Unity | Editor below floor fixture | Open service | Compatibility blocker and no mutation | Yes | Not run |
| EGS-T-007 | Play Mode guard | Editor in Play Mode | Attempt apply | Mutation is blocked | Yes | Not run |
| EGS-T-008 | Compile error guard | Project compile errors | Attempt apply | Plan may inspect but apply blocks | Yes | Not run |
| EGS-T-009 | Catalog IDs | Catalog with duplicate ID | Validate catalog | Blocker identifies duplicates | Yes | Not run |
| EGS-T-010 | Package ID validation | Malformed package ID | Build plan | Selection blocks | Yes | Not run |
| EGS-T-011 | Exact source policy | Recommended preset with branch source | Validate | Branch warning/blocker per policy | Yes | Not run |
| EGS-T-012 | Blank preset | No peers installed | Build Blank plan | Only selected generic output appears | Yes | Not run |
| EGS-T-013 | Game Jam visibility | Game Jam preset | Build plan | Every selected package/bridge is listed | Yes | Not run |
| EGS-T-014 | Chronicle optional | Game Jam defaults | Inspect save choice | No save is silently forced | Yes | Not run |
| EGS-T-015 | Custom empty selection | Custom preset | Select no packages | Valid generic skeleton plan | Yes | Not run |
| EGS-T-016 | Custom package subset | Three compatible peers | Build plan | Only chosen packages and required bridges appear | Yes | Not run |
| EGS-T-017 | Incompatible package pair | Catalog incompatibility fixture | Build plan | Conflict blocks apply | Yes | Not run |
| EGS-T-018 | Missing source | Unavailable package source | Build plan/apply | Asset phase never begins | Yes | Not run |
| EGS-T-019 | Project catalog override | Unverified override | Select package | Trust approval required | Yes | Not run |
| EGS-T-020 | Preset schema old | Old project preset | Open/migrate | Supported migration preserves choices | Yes | Not run |
| EGS-T-021 | Preset schema newer | Newer project preset | Open | Read-only unsupported result | Yes | Not run |
| EGS-T-022 | Plan purity | Stable snapshot/request | Build plan twice | Plan hash and operations are identical | Yes | Not run |
| EGS-T-023 | Snapshot revision | Project changes after plan | Apply old plan | Stale plan rejected | Yes | Not run |
| EGS-T-024 | Operation IDs | Two plans same logical output | Compare IDs | Deterministic IDs match | Yes | Not run |
| EGS-T-025 | Visible mutation | Plan with package/scene/settings changes | Inspect plan | Every mutation has target/reason/risk | Yes | Not run |
| EGS-T-026 | Destructive classification | Replace operation fixture | Build plan | Explicit destructive approval required | Yes | Not run |
| EGS-T-027 | Dependency ordering | Operations with prerequisites | Build plan | Topological order is deterministic | Yes | Not run |
| EGS-T-028 | Cycle detection | Cyclic fake operations | Build plan | Blocker reports cycle | Yes | Not run |
| EGS-T-029 | Remove operation dependency | Approved plan | Deselect prerequisite | Dependent operations rebuild/remove | Yes | Not run |
| EGS-T-030 | Plan drift | UPM resolves different compatible version | Reconcile | Material delta shown and reapproval required | Yes | Not run |
| EGS-T-031 | Non-material drift | Only timestamp/request metadata changes | Reconcile | No false destructive delta | Yes | Not run |
| EGS-T-032 | Root under Assets | Valid root | Build plan | Path canonicalized and accepted | Yes | Not run |
| EGS-T-033 | Root outside Assets | Absolute external path | Build plan | Blocked | Yes | Not run |
| EGS-T-034 | Parent traversal | Root contains .. | Build plan | Blocked | Yes | Not run |
| EGS-T-035 | Reserved/invalid name | Invalid folder token | Build plan | Sanitized suggestion or blocker | Yes | Not run |
| EGS-T-036 | Case collision | Case-sensitive fixture | Build paths differing only case | Collision reported per platform | Yes | Not run |
| EGS-T-037 | Package cache target | Target under Packages/cache | Build plan | Blocked | Yes | Not run |
| EGS-T-038 | Symlink escape | Sandbox symlink escape where supported | Build plan | Blocked | Platform | Not run |
| EGS-T-039 | Credential redaction | Source error contains token-like value | Export report | Secret is redacted | Yes | Not run |
| EGS-T-040 | Private path redaction | Absolute user path in error | Export support snapshot | Project-relative/redacted path | Yes | Not run |
| EGS-T-041 | List packages | Fake UPM graph | Inspect project | Direct/resolved versions captured | Yes | Not run |
| EGS-T-042 | Single package add | Approved plan | Apply package phase | One request settles and journal advances | Yes | Not run |
| EGS-T-043 | Add and remove set | Approved bounded graph change | Apply | One coherent request where supported | Yes | Not run |
| EGS-T-044 | UPM failure | Gateway fails | Apply | Journal preserved; no generation | Yes | Not run |
| EGS-T-045 | UPM timeout/stall presentation | Long fake request | Observe | Progress/cancel policy remains responsive | Yes | Not run |
| EGS-T-046 | Domain reload journal | Apply package request | Simulate reload | Transaction resumes at reconcile | Yes | Not run |
| EGS-T-047 | Editor restart recovery | Incomplete journal | Restart/open Workshop | Resume/Inspect/Abandon offered | Yes | Not run |
| EGS-T-048 | No auto-resume mutation | Incomplete journal after restart | Open project | No mutation until confirmation | Yes | Not run |
| EGS-T-049 | Corrupt journal | Invalid journal | Recover | Quarantined and manual reconciliation offered | Yes | Not run |
| EGS-T-050 | Old journal migration | Supported old journal | Recover | Migrates safely | Yes | Not run |
| EGS-T-051 | Newer journal | Unsupported newer journal | Recover | Read-only/abandon only | Yes | Not run |
| EGS-T-052 | Competing lock | Transaction active | Start second apply | Rejected | Yes | Not run |
| EGS-T-053 | Stale lock | Dead-process/stale fixture | Inspect | Recovery path requires confirmation | Yes | Not run |
| EGS-T-054 | Cancel before UPM | Approved plan not started | Cancel | No mutation | Yes | Not run |
| EGS-T-055 | Cancel too late UPM | Request in atomic fake phase | Cancel | Too Late; request settles and reports | Yes | Not run |
| EGS-T-056 | Create root | Empty sandbox | Apply skeleton | Root and selected folders created | Yes | Not run |
| EGS-T-057 | No empty decorative folders | Minimal selection | Apply | Unused folders absent | Yes | Not run |
| EGS-T-058 | Create asmdefs | Asmdef options selected | Apply | Valid project-owned asmdefs created | Yes | Not run |
| EGS-T-059 | Asmdef name conflict | Existing different asmdef | Plan/apply | Conflict; no overwrite | Yes | Not run |
| EGS-T-060 | Docs shell | Docs option selected | Apply | Expected Markdown files created | Yes | Not run |
| EGS-T-061 | Existing identical Workshop output | Matching fingerprint | Repeat | No Change | Yes | Not run |
| EGS-T-062 | Existing modified output | Changed generated file | Repeat | Generated Modified; preserve | Yes | Not run |
| EGS-T-063 | Existing user output | Preexisting untracked file | Plan | Conflict/adoption choice | Yes | Not run |
| EGS-T-064 | Adopt compatible output | Validator-approved existing item | Adopt | Manifest records adopted without rewrite | Yes | Not run |
| EGS-T-065 | Missing generated output | Manifest item deleted | Repair plan | Exact repair candidate | Yes | Not run |
| EGS-T-066 | Repair apply | Eligible missing output | Apply repair | Recreated with new receipt | Yes | Not run |
| EGS-T-067 | Repair incompatible recipe | Old unavailable adapter | Build repair | Manual/orphan guidance | Yes | Not run |
| EGS-T-068 | Side-by-side generation | Modified binary asset | Choose side-by-side | Candidate created at unique path | Yes | Not run |
| EGS-T-069 | Batch exception cleanup | Asset operation throws | Apply | AssetDatabase editing state restored | Yes | Not run |
| EGS-T-070 | Report conflict | User file at report path | Finalize | No overwrite; resolve path | Yes | Not run |
| EGS-T-071 | Exact facade success | Compatible fake facade | Plan/apply | Structured package receipts returned | Yes | Not run |
| EGS-T-072 | No broad scanning | Unlisted provider type exists | Resolve adapters | Provider is ignored | Yes | Not run |
| EGS-T-073 | Missing facade type | Installed package missing type | Validate | Automated setup blocked; manual guide | Yes | Not run |
| EGS-T-074 | Wrong facade signature | Type exists wrong methods | Validate | Compatibility blocker | Yes | Not run |
| EGS-T-075 | Unsupported facade schema | New schema fixture | Validate | Blocked/read-only descriptor result | Yes | Not run |
| EGS-T-076 | Facade plan mismatch | Facade returns unexpected operation category | Plan | Reapproval/blocker | Yes | Not run |
| EGS-T-077 | Facade apply throws | Fake exception | Apply | Dependent operations stop; receipt/journal retained | Yes | Not run |
| EGS-T-078 | Facade partial result | Some operations settle then fail | Apply | Exact settled/failed items reported | Yes | Not run |
| EGS-T-079 | Facade cancellation before atomic work | Cancellable fake facade | Cancel | Cancelled safely | Yes | Not run |
| EGS-T-080 | Facade cancellation too late | Atomic fake facade phase | Cancel | Too Late and final result recorded | Yes | Not run |
| EGS-T-081 | Package-specific validation pass | Generated fake package | Validate | Pass appears in readiness | Yes | Not run |
| EGS-T-082 | Package-specific validation blocker | Broken fake output | Validate | Generation incomplete/blocker | Yes | Not run |
| EGS-T-083 | Adapter version receipt | Facade succeeds | Finalize | Manifest records adapter/facade/setup versions | Yes | Not run |
| EGS-T-084 | Create scene | No target scene | Apply | Scene saved at approved path | Yes | Not run |
| EGS-T-085 | Existing scene path | User scene exists | Plan | No overwrite; conflict | Yes | Not run |
| EGS-T-086 | Global scene list | No Build Profile override | Apply approved list | Exact before/after global list | Yes | Not run |
| EGS-T-087 | Build Profile override | Active profile overrides scenes | Apply approved list | Profile list changed, global preserved | Yes | Not run |
| EGS-T-088 | Ambiguous scene target | Multiple relevant profiles fixture | Plan | User must select target | Yes | Not run |
| EGS-T-089 | Duplicate scene list entry | Scene already listed | Repeat | No duplicate | Yes | Not run |
| EGS-T-090 | Scene reorder | Explicit reorder selected | Apply | Before/after recorded | Yes | Not run |
| EGS-T-091 | No implicit numeric order | Numbered scene paths | Plan without reorder | Existing order preserved | Yes | Not run |
| EGS-T-092 | Scene save failure | Fake scene service fails | Apply | Receipt failed; dependent references stop | Yes | Not run |
| EGS-T-093 | Manifest creation | Generation complete | Finalize | Project-owned manifest created | Yes | Not run |
| EGS-T-094 | Manifest output identity | Generated asset | Inspect manifest | Logical ID, GUID, path, origin, fingerprint recorded | Yes | Not run |
| EGS-T-095 | Manifest repeat history | Two generations | Finalize | Bounded history appended | Yes | Not run |
| EGS-T-096 | Manifest history pruning | Beyond retention | Finalize | Old history archived/pruned per policy | Yes | Not run |
| EGS-T-097 | Old manifest migration | Supported old version | Open | Backup then contiguous migration | Yes | Not run |
| EGS-T-098 | Missing migration step | Gap fixture | Open | Blocks safely; source unchanged | Yes | Not run |
| EGS-T-099 | Newer manifest read-only | Future version | Open | No downgrade/write | Yes | Not run |
| EGS-T-100 | Markdown/JSON consistency | Completed generation | Compare reports | Counts/statuses match structured receipts | Yes | Not run |
| EGS-T-101 | Requested vs resolved truth | Version resolution differs | Inspect report | Both values shown | Yes | Not run |
| EGS-T-102 | Manual action list | Skipped/missing facade items | Finalize | Actionable manual list present | Yes | Not run |
| EGS-T-103 | Support export payload | Manifest/report available | Export | Only approved redacted fields | Yes | Not run |
| EGS-T-104 | Removal unchanged | Generated-managed unchanged outputs | Build guide | Safe removable classification | Yes | Not run |
| EGS-T-105 | Removal modified | Generated-modified outputs | Build guide | Retain/manual classification | Yes | Not run |
| EGS-T-106 | Removal package dependency | Peer required by another package | Build guide | Package removal blocked/explained | Yes | Not run |
| EGS-T-107 | Upgrade diff add | New preset adds output | Build upgrade plan | Create operation shown | Yes | Not run |
| EGS-T-108 | Upgrade diff remove | New preset removes output | Build plan | No automatic delete; deprecation/removal guidance | Yes | Not run |
| EGS-T-109 | Upgrade modified asset | User-modified old output | Build upgrade plan | Side-by-side/manual merge | Yes | Not run |
| EGS-T-110 | Readiness all pass | Valid generated foundation | Validate | Complete/pass status | Yes | Not run |
| EGS-T-111 | Readiness advisory | Optional recommendation missing | Validate | Advisory without false blocker | Yes | Not run |
| EGS-T-112 | Readiness blocker | Duplicate authority fixture | Validate | Incomplete/blocker | Yes | Not run |
| EGS-T-113 | Workshop dependency scan | Generated assemblies | Validate | No Workshop reference | Yes | Not run |
| EGS-T-114 | Runtime assembly scan | Package build | Validate | Workshop contributes no runtime assembly | Yes | Not run |
| EGS-T-115 | Remove Workshop after Game Jam | Generated temporary project | Remove package/recompile | Selected project remains valid | Yes | Not run |
| EGS-T-116 | Repeat Game Jam generation | Completed temporary project | Run same preset | No duplicates or changed output | Yes | Not run |
| EGS-T-117 | Large plan | 2,000 operation fixture | Build/display | Within budget and UI responsive | Yes | Not run |
| EGS-T-118 | Large manifest | 10,000 record fixture | Compare | Bounded performance/advisory | Yes | Not run |
| EGS-T-119 | Idle window | Window inactive | Profile | No per-frame work/allocation loop | Yes | Not run |
| EGS-T-120 | Editor quit during apply | Fake long phase | Quit/restart | Recovery journal available | Yes | Not run |
| EGS-T-121 | Laboratory reset scope | Sandbox + real project fixture | Reset | Only Laboratory-owned sandbox/journal removed | Yes | Not run |

### 23.4 Test registry count

This approved design registers **121 implementation test cases** in addition to the 40 Workshop Laboratory scenarios. IDs remain stable during implementation; cases may split into narrower tests without reusing an existing ID.

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Editor-only authority and non-authority are approved.
- [x] Blank and Game Jam MVP presets are separated from later presets.
- [x] Package catalog/source/version policy is defined.
- [x] Dry-run and transaction/reload architecture is defined.
- [x] Exact setup facade adapter strategy is defined.
- [x] Ownership, conflict, manifest, repair, and removal rules are defined.
- [x] Workshop Laboratory and clean-project proof are designed.
- [x] Release-blocking design questions are resolved for this specification.
- [x] FW-DOC-11 reconciles facade requirements through SFGSS-ADR-001.

### 24.2 Implementation gate

- [ ] Package contains no runtime assembly.
- [ ] Editor code compiles with declared Unity dependencies only.
- [ ] Window opens in a clean project with no peers.
- [ ] Planning is mutation-free and deterministic.
- [ ] Package operations are serialized and resumable after reload.
- [ ] Project lock and recovery journal pass fault tests.
- [ ] Exact facade invocation rejects unlisted/incompatible endpoints.
- [ ] Generic generation is create-only safe by default.
- [ ] AssetDatabase batching restores state after exceptions.
- [ ] Public APIs/results/diagnostics match this specification or approved ADR/spec revision.

### 24.3 Standalone gate

- [ ] Blank Modular Starter generates with no peer Echo packages.
- [ ] Workshop Laboratory passes all required scenarios.
- [ ] Laboratory sample can be removed.
- [ ] Package removal leaves generated output intact.
- [ ] No generated assembly references The Workshop.
- [ ] No Workshop code enters Player builds.

### 24.4 Composition gate

- [ ] Every advertised Foundation peer has a compatible tested adapter/facade or is clearly manual-only.
- [ ] Game Jam Quickstart shows every selected package and bridge.
- [ ] Package resolution drift triggers reapproval.
- [ ] Selected package validators run.
- [ ] Global/Build Profile scene-list behavior passes.
- [ ] Existing-project side-by-side adoption passes.
- [ ] Repeat generation produces no duplicates.
- [ ] Modified project assets are preserved.

### 24.5 Quality gate

- [ ] Automated tests pass.
- [ ] Manual Laboratory checklist passes.
- [ ] Clean temporary-project generation tests pass.
- [ ] No blocker/critical defect remains.
- [ ] Performance targets pass.
- [ ] Reports and diagnostics are actionable and redacted.
- [ ] Documentation matches the build.
- [ ] `Current Notes.md` is reconciled.
- [ ] Licenses and notices are complete.

### 24.6 Distribution gate

- [ ] Valid `package.json` with Editor-only intent documented.
- [ ] Version/changelog updated.
- [ ] Stable `.meta` files included.
- [ ] Tarball and tagged Git installation tested externally.
- [ ] Package removal tested.
- [ ] Catalog/preset compatibility matrix published.
- [ ] Package setup facade authoring guide published.
- [ ] Repository tag/release prepared.
- [ ] Central suite catalog updated.

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Target | Starting condition | Adoption strategy | Parity/readiness gate | Rollback/preservation |
|---|---|---|---|---|
| Clean Workshop development project | No generated foundation | Blank Modular Starter first | Repeat run, manifest, report, removal | Delete sandbox/generated root and remove package |
| Sperk’s Forge Integration Lab | All local package repos available | Game Jam Quickstart with local-path catalog | All package validators and integration labs | Preserve pre-generation branch/worktree |
| Rescuers2D | Existing bootstrap/audio/UI/input/save pieces | Side-by-side generated root; integrate one authority at a time | Per-package parity and project tests | Keep original systems until parity |
| Don’t Get Vince’d | Existing gameplay project | Select only needed shell/audio/UI pieces | No regression to current game loop | Remove generated candidates/bridges |
| Hackulos clean project | Planned top-down RPG consumer | Start with approved Foundation shell; later add RPG packages | Foundation readiness before gameplay modules | Regenerate clean root or remove candidates |

### 25.2 Preserve-until-parity rule

The Workshop does not migrate or delete existing systems merely because a new package is selected. Existing project code, scenes, prefabs, settings, and data remain intact until:

1. The selected package passes its own Standalone Test Lab.
2. The generated candidate passes Workshop/package validation.
3. One project feature category is connected.
4. Parity/regression tests pass.
5. The user explicitly removes the old system through a separate project checkpoint.

### 25.3 Existing project adoption modes

| Mode | Behavior | Intended use |
|---|---|---|
| Side-by-side | Generate under new root with no adoption | Safest default |
| Adopt validated | Record compatible existing package assets after owner validator approval | Project already uses package manually |
| Fill missing only | Create absent eligible assets, preserve all existing | Partial setup |
| Candidate upgrade | Generate side-by-side upgraded assets and comparison report | Modified/binary assets |
| Manual plan only | Produce instructions without mutation | Sensitive/complex project |

### 25.4 Migration tooling

The MVP provides:

- Project snapshot and conflict report.
- Existing package/version detection.
- Existing root and authority detection through package validators.
- Side-by-side output.
- Explicit adoption records.
- Basic preset/catalog/manifest version diff.
- Repair of eligible missing generated-managed items.
- Removal guidance.

It does not promise automatic conversion of project-specific managers, scenes, save schemas, input maps, or UI prefabs. Those require package/project migration specifications.

### 25.5 Workshop version upgrades

On Workshop update:

1. Open existing manifest read-only.
2. Migrate manifest schema only with backup and explicit supported path.
3. Compare current catalog/preset/adapter versions.
4. Build an upgrade dry run.
5. Preserve modified/adopted assets.
6. Use package-owned migration providers when approved.
7. Write a new generation history record rather than erasing the old receipt.

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| R-001 | Composer becomes a runtime framework | Medium | Critical | Editor-only package, no runtime assembly, generated project dependency scan | Any runtime type/reference proposal |
| R-002 | Workshop duplicates package setup logic | High | High | Exact package-owned setup facades and adapter descriptors | Drift or copied asset schema |
| R-003 | Package setup facade contract was not in earlier approved specs | High | Medium | FW-DOC-11 addenda/reconciliation before implementation | Contract matrix |
| R-004 | Reflection endpoint is unsafe/fragile | Medium | High | Exact allowlisted type/signature/schema; no discovery scan; fail closed | Adapter validation |
| R-005 | Package resolution/domain reload loses transaction | High | High | Journal before request, resumable phases, explicit restart recovery | Reload fault tests |
| R-006 | UPM source/version is not reproducible | Medium | High | Exact tags/versions; record requested/resolved; dev branches visibly flagged | Catalog review |
| R-007 | Existing project assets overwritten | Medium | Critical | Create-only default, fingerprints, adoption classification, explicit destructive plans | Conflict tests |
| R-008 | Modified generated asset mistaken for safe replacement | Medium | Critical | Any drift means preserve/manual by default | Repair/upgrade review |
| R-009 | Binary Unity assets cannot be merged | High | Medium | Side-by-side candidates; package-specific migration only | Scene/prefab/input upgrade |
| R-010 | Build Profile/global scenes are changed incorrectly | Medium | High | Adapter inspects both; exact before/after; ambiguity blocks | Unity 6 profile tests |
| R-011 | Generated output retains Workshop dependency | Low | Critical | No runtime contracts; dependency scan and removal test | Composition gate |
| R-012 | Setup succeeds but generated project is broken | Medium | High | Selected package validators, compilation/readiness gate, report truth states | Final validation |
| R-013 | Removal guide deletes adopted/modified content | Medium | Critical | Guide classification only; no automatic delete in MVP | Removal design/tests |
| R-014 | Credentials/private paths leak into reports | Medium | High | Redaction policy, tests, no credential access/storage | Support export |
| R-015 | Package catalog becomes stale | High | Medium | Versioned compatibility matrix, release validation dates, fail closed on unknown | Package releases |
| R-016 | Too many preset options overwhelm novice | Medium | Medium | Two clear MVP presets, progressive disclosure, plain technical descriptions | UX tests |
| R-017 | AssetDatabase batching leaves Editor stuck | Low | High | Small batches, `try/finally`, fault tests | Generation exception |
| R-018 | Transaction lock becomes stale | Medium | Medium | Process/session evidence, timed recovery, explicit unlock review | Restart/crash |
| R-019 | Workshop reports false rollback | Medium | High | Honest settled-operation receipts; no “all undone” claim | Failure/cancel paths |
| R-020 | Scope expands into every expansion package before MVP | High | Medium | Foundation-only catalog/presets; later adapters deferred | Roadmap review |
| R-021 | UI Toolkit Editor behavior changes across Unity 6 | Medium | Medium | Public floor tests, isolated view/service, compatibility matrix | Unity update |
| R-022 | User expects Workshop to commit/push | Medium | Low | Report/checklist only; source-control provider deferred | Documentation/UX |
| R-023 | Path/symlink escape modifies unintended files | Low/Medium | Critical | Canonicalization, root allowlist, link checks, explicit export APIs only | Security tests |
| R-024 | Manifest becomes permanent ownership claim | Medium | Medium | Project-owned record, adoption/modified classes, user can remove Workshop | Docs/API language |
| R-025 | Temporary journal becomes source of truth | Low | High | Final manifest/report required; journal is recovery only | Finalize tests |

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| EGS-D-001 | The Workshop is Editor-only and ships no runtime assembly | Approved | Composer is not a game authority | Generated projects do not depend on it | No |
| EGS-D-002 | One mutating transaction per project; multiple read-only views allowed | Approved | Prevent conflicting writes while preserving inspection | Project lock and journal required | No |
| EGS-D-003 | Every apply begins from an immutable dry-run plan | Approved | Visibility and reproducibility are safety requirements | Plan drift invalidates approval | No |
| EGS-D-004 | Package operations and asset generation are separate resumable phases | Approved | UPM causes resolution/compilation/reload | Journal/reconciliation required | No |
| EGS-D-005 | Normal package changes use Unity Package Manager Client APIs, not direct manifest editing | Approved | Supported resolution path and clearer error behavior | UPM request abstraction/test seam | No |
| EGS-D-006 | Recommended sources use exact versions/tags/commits; branches are dev-only warnings | Approved | Reproducible starters | Record requested and resolved versions | No |
| EGS-D-007 | Package-specific setup remains in the owning package | Approved | Prevent drift and authority theft | Workshop needs adapters/facades | No |
| EGS-D-008 | MVP invokes exact allowlisted Editor setup facades through versioned descriptors | Approved | Avoid compile-time peer dependencies without open discovery | Resolved by SFGSS-ADR-001 and FW-DOC-11 | Yes if cross-spec contract is suite-wide |
| EGS-D-009 | No open-ended reflection discovery | Approved | Security, determinism, versioning | Missing endpoint fails closed | No |
| EGS-D-010 | A shared Editor setup-contracts package is not introduced by this spec | Approved | Avoid silent new mandatory package | Revisit only via ADR after implementation evidence | Future ADR only |
| EGS-D-011 | Generated output is project-owned; manifest records origin but not perpetual control | Approved | Projects must be free to evolve | Modified/adopted assets preserved | No |
| EGS-D-012 | Create-only safe is the default | Approved | Existing project safety | Replace/delete/migrate require explicit plan | No |
| EGS-D-013 | Fingerprint drift always removes automatic overwrite eligibility | Approved | False negatives are safer than data loss | Manual/side-by-side upgrades | No |
| EGS-D-014 | Full automatic uninstall is deferred; MVP produces a removal guide | Approved | Ownership/dependency risk | User reviews removal | No |
| EGS-D-015 | Blank Modular Starter may select no peer packages | Approved | Proves Workshop independence | Generic skeleton is useful alone | No |
| EGS-D-016 | Game Jam Quickstart shows all peers; Chronicle is an explicit save choice | Approved | No hidden bundle/save assumption | Default shell remains fast and transparent | No |
| EGS-D-017 | Observatory is recommended with First Light but removable/visible | Approved | Matches SFGSS-000 | Preset shows both separately | No |
| EGS-D-018 | UI Toolkit is the MVP Editor UI | Approved | Modern Editor tooling, resizable/virtualized workflow | Unity 6 compatibility tests required | No |
| EGS-D-019 | Workshop Laboratory replaces a meaningless runtime Test Lab scene | Approved | Core behavior is Editor generation | Clean-project fixtures are mandatory | No |
| EGS-D-020 | Global scene lists and Build Profile overrides use an adapter and explicit target | Approved | Unity 6 has multiple scene-list authorities | Ambiguity blocks | No |
| EGS-D-021 | Transaction journals live under Library and never auto-resume mutation after restart | Approved | Recovery without surprise writes | User confirms Resume | No |
| EGS-D-022 | Durable manifest/report live with project output and are commit-friendly | Approved | Documentation-as-code and reproducibility | Redaction/truth rules required | No |
| EGS-D-023 | The Workshop never commits or pushes in MVP | Approved | Source-control intent/credentials are separate | Checklist/report only | No |
| EGS-D-024 | Foundation implementation remains locked after this approval | Approved | FW-DOC-11/12 are mandatory gates | No runtime code begins yet | No |

### 27.2 Release-blocking questions

No unresolved question blocks this package or Foundation implementation planning. FW-DOC-11 resolved the peer setup-facade contract through **SFGSS-ADR-001 — Foundation Editor Setup Facade Protocol**.

The ADR fixes the exact facade identity, six-method static surface, detached JSON envelopes, plan/apply handshake, receipts, reload/cancellation rules, manual fallback, and compatibility gate. Peer packages remain independently releasable without a facade, but may not advertise automated Workshop setup until their facade and adapter tests pass.

### 27.3 Non-blocking later questions

- Whether a tiny shared Editor-only setup contracts package becomes justified after facade prototypes.
- Whether headless/batchmode composition is supportable across Unity versions.
- Whether package source catalogs can be signed or verified beyond normal Git/registry trust.
- Whether full uninstall and package-owned migration providers belong in Workshop v2.
- Which expansion presets graduate after their package specifications exist.
- Whether a source-control provider package is worthwhile.

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved package contract | Design only | This approved document + FW-DOC-11 reconciliation |
| M1 - Editor skeleton | Installable Editor-only package/window | Manifest, asmdef, docs shell, empty Blank flow | Clean compile; no Player assembly |
| M2 - Planning core | Inspection, catalog, presets, immutable plan/conflicts | Pure services and fake gateways | EditMode tests |
| M3 - Generic generator | Safe root/folders/asmdefs/docs/manifest | Create-only output and receipts | Workshop Laboratory |
| M4 - Transaction/UPM | Package graph apply and reload recovery | Gateway, journal, lock, reconcile | Fault and temp-project tests |
| M5 - Facade protocol | Exact adapter verification/invocation | Fake facade plus First Light/Observatory prototypes | Adapter integration tests |
| M6 - Blank Modular Starter | Complete selectable minimal composer | Generic + selected peer setup | Clean project/remove Workshop |
| M7 - Game Jam Quickstart | Visible application shell preset | Foundation peers/options/scenes/report | Clean generated project/readiness |
| M8 - Repair/readiness | Repeat, repair plan, removal guide, validation | Manifest drift and reports | Existing-project fixtures |
| M9 - Release | Distribution-ready beta/1.0 | Docs, compatibility, tarball/tag | External install and generation |

### 28.2 Checkpoint rule

Every milestone is split into SFGSS-005 Checkpoint Build Plans with:

- One user-visible/testable outcome.
- Exact files and Editor operations.
- Scope exclusions.
- Automated and manual tests.
- Fault injection.
- Documentation/Current Notes reconciliation.
- Commit/push checkpoint.
- Safe rollback or recovery.

### 28.3 First recommended implementation checkpoint

After FW-DOC-11 and FW-DOC-12 authorize implementation:

> **EGS-M1-01 - Editor-Only Package Skeleton and Empty Workshop Window**: create the UPM package anatomy, Editor asmdef, package metadata/docs shell, UI Toolkit window, package version/status display, and an automated proof that no runtime assembly or Player code is produced. Do not implement package installation or project generation yet.

The suite-wide implementation order may still begin with First Light rather than The Workshop. This is the first Workshop checkpoint, not necessarily the first Foundation code checkpoint.

## 29. New-Conversation Handoff

Use this prompt with SFGSS-000, this specification, FW-DOC-11/12, SFGSS-005, Current Notes, and the active checkpoint:

```text
We are continuing development of The Sperk’s Forge - EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the authority for suite-wide package boundaries and architecture.
Treat The Workshop (EchoGameStarter) Specification v1.0.0 as the authority for
Editor-time composition, package selection, dry-run planning, generation,
transaction recovery, setup-facade adapters, manifests, repair/removal guidance,
and release gates. Follow the Foundation contract matrix and SFGSS-005.

Current package: The Workshop (EchoGameStarter)
Current specification version: 1.0.0
Current milestone/checkpoint: <CHECKPOINT>
Current Unity version: <VERSION>
Current repository/workspace: <PROJECT>
Current implementation status: <STATUS>
Known blockers: <BLOCKERS>
Current Notes reviewed through: <DATE/COMMIT>

Before writing code:
1. Confirm the Workshop remains Editor-only and generates no runtime manager.
2. Confirm every mutation appears in an immutable approved dry-run plan.
3. Keep package-specific setup in package-owned Editor facades/adapters.
4. Preserve project-owned and modified assets by default.
5. Model package resolution/domain reload as a resumable transaction.
6. Verify generated projects remain valid after Workshop removal.
7. Continue using the Checkpoint Build Plan format.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | Specification v1.0.0; implementation not started |
| Completed checkpoint | FW-DOC-10 - complete Workshop package specification |
| Files/assets created | Specification and documentation checkpoint files only |
| Tests passed | Documentation structure/consistency checks only |
| Tests failed | None; implementation tests not run |
| Known issues | No release-blocking Workshop architecture issue; facade implementation remains milestone work under SFGSS-ADR-001 |
| Decisions added | EGS-D-001 through EGS-D-024 |
| Next checkpoint | FW-DOC-11 - Foundation cross-package contract matrix |

## 30. Approval

### 30.1 Approval checklist

- [x] Package identity and plain responsibility are clear.
- [x] Editor-only ownership aligns with SFGSS-000.
- [x] The package does not become a runtime manager or peer authority.
- [x] Independence without Foundation peers is credible.
- [x] Blank and Game Jam MVP presets are defined.
- [x] Package source/version visibility and trust rules are defined.
- [x] Dry-run, operation, transaction, reload, and failure behavior are specified.
- [x] Package-specific setup remains owned by peer packages.
- [x] Exact facade adapter invocation is bounded and fail-closed.
- [x] Project ownership, conflict, repair, upgrade, and removal rules are specified.
- [x] Workshop Laboratory and clean-project proof are defined.
- [x] Diagnostics, security, privacy, performance, and versioning are measurable.
- [x] No Isekai Studios ownership/identity has been introduced.
- [x] Jesse has authorized the most effective durable choices for the documentation-first pass.
- [x] Approval does not authorize runtime implementation.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams  
**Date:** August 3, 2026  
**Conditions:** FW-DOC-11 resolved package Editor setup-facade requirements through SFGSS-ADR-001. FW-DOC-12 must still pass before any Foundation runtime implementation begins.

---

## Specification Completion Record

A new collaborator can answer:

1. The Workshop owns visible Editor-time composition, not runtime systems.
2. It refuses to own peer package setup internals, gameplay, builds, source hosting, or a runtime manager.
3. Its smallest useful release is Blank Modular Starter plus Game Jam Quickstart, dry-run/apply, safe generation, reports, and readiness.
4. It works alone by generating a generic skeleton and using no peer compile-time dependency.
5. Definitions, Editor transaction state, and project-owned generated records are separate.
6. Its public Editor API, plan, operation, journal, and facade adapter lifecycle are explicit.
7. Failures preserve settled truth, stop dependent operations, and produce recovery guidance.
8. Its standalone proof is an Editor Laboratory and disposable clean-project fixtures.
9. Peer packages connect through exact Editor setup facades/adapters without runtime dependency.
10. Release requires repeat-run, removal, clean-project, adapter, reload, security, and generated-project independence evidence.

The Workshop specification is complete and **Approved v1.0.0**. All ten Foundation package specifications are now approved. Runtime implementation remains locked while FW-DOC-11 builds the cross-package contract matrix and FW-DOC-12 performs the documentation readiness/implementation authorization review.


---

## Appendix A — Accepted Foundation Facade Protocol

The Workshop’s setup-facade integration contract is governed by [[../Architecture Decision Records/SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol|SFGSS-ADR-001]].

The Workshop verifies exact package ID/version, assembly-qualified facade type, protocol version, facade schema, setup schema, and six public static methods: `Describe`, `Plan`, `Apply`, `Validate`, `Compare`, and `RemovalGuidance`. Each method accepts and returns bounded detached JSON strings. Apply requires the matching approved plan hash and operation IDs. The Workshop never performs broad reflection discovery and never adds a runtime dependency to generated projects.


---

## Graph Navigation

#sfgss/package #sfgss/wave/foundation #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
