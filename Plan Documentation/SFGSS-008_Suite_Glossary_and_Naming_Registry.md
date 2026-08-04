# The Sperk’s Forge – Suite Glossary and Naming Registry

**Document ID:** SFGSS-008  
**Version:** 1.0.0  
**Status:** Approved naming and terminology standard  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000 v0.17.0  
**Related authorities:** SFGSS-001 through SFGSS-007, SFGSS-ADR-001 through SFGSS-ADR-003, and the Foundation, Expansion, and Advanced integration matrices  
**Current development baseline:** Unity 6000.3.8f1  
**Last updated:** August 4, 2026

> Give every concept one clear name, and make every name reveal which authority owns it.

---

## Contents

1. Purpose and authority
2. Scope and non-goals
3. Naming principles
4. Suite identity and typography
5. Canonical package identity registry
6. Documentation identifiers and filenames
7. Package IDs, repositories, assemblies, and namespaces
8. Public API type-name vocabulary
9. Stable identifiers and qualified identity names
10. Diagnostic, test, Laboratory, and validation prefixes
11. Bridge, provider, adapter, and presentation naming
12. Scenes, samples, Laboratories, and generated content
13. Canonical suite glossary
14. Ambiguous terms that require qualification
15. Reserved, prohibited, deprecated, and historical names
16. Synonyms, aliases, and deprecation rules
17. Capitalization, punctuation, pluralization, and abbreviations
18. Naming validation and registry maintenance
19. Migration and compatibility behavior
20. Current reconciliation findings
21. Approval

---

## 1. Purpose and authority

SFGSS-008 is the canonical glossary and naming registry for **The Sperk’s Forge – EchoDevGames Game Systems Suite**. It gives every durable package identity, document family, namespace family, diagnostic prefix, API suffix, lifecycle term, integration artifact, and frequently overloaded concept one approved meaning.

This standard exists because naming is architecture wearing a readable coat. A type called `GameManager`, a generic `PlayerId`, or an unqualified `State` hides authority instead of revealing it. A public title that changes punctuation or ordering between files makes one product look like several. A diagnostic prefix reused by two packages turns a searchable failure into a scavenger hunt.

### 1.1 Authority order

When names disagree, use this order:

1. SFGSS-000 for suite identity, package ownership, and public package families.
2. The approved package specification for package-local public APIs and data concepts.
3. This standard for cross-suite spelling, qualification, naming patterns, prefixes, reserved terms, and canonical package identity metadata.
4. SFGSS-002 for dependency, package, assembly, bridge, and provider structure.
5. SFGSS-003 for stable identity semantics and serialization compatibility.
6. SFGSS-004 for test, Laboratory, evidence, and release identifiers.
7. Accepted ADRs and integration specifications.
8. Guides, samples, reports, and Current Notes.

SFGSS-008 does not silently rename an approved public API. When an existing API conflicts with this standard, the package specification or an ADR records the migration before code or serialized data changes.

### 1.2 Registry companion

`SFGSS-008_Package_Naming_Registry.json` is a documentation companion containing the twenty-eight package identity records in machine-readable form. The Markdown standard remains authoritative. The JSON file may later support validators, documentation generation, compatibility catalogs, or Workshop planning, but it is not runtime configuration.

---

## 2. Scope and non-goals

### 2.1 This standard governs

- Suite, package, bridge, provider, adapter, document, repository, assembly, namespace, diagnostic, test, Laboratory, setup-facade, and identifier naming.
- Canonical public titles and technical identifiers.
- API suffixes that communicate data and lifecycle semantics.
- Qualification rules for overloaded terms such as player, target, state, profile, participant, owner, focus, and selection.
- Reserved and deprecated names.
- Alias and terminology migration rules.
- Obsidian link labels and documentation filenames.

### 2.2 This standard does not govern

- Game-specific character, item, quest, ability, location, faction, or narrative names.
- Package icons, logos, color palettes, or full visual identity systems.
- Translation choices inside localized project content.
- C# formatting beyond naming surfaces that affect suite consistency.
- Provider trademarks beyond accurate adapter naming and required notices.
- Final repository URLs that have not yet been created or approved.

---

## 3. Naming principles

### 3.1 Reveal authority

Names should answer at least one of these questions:

- Which package owns this truth?
- Is this authored definition, project configuration, mutable runtime state, or detached durable data?
- Is this a request, result, snapshot, handle, lease, registration, provider, bridge, or presenter?
- Is this identity durable, session-only, provider-local, or presentation-only?

### 3.2 One canonical name per concept

A concept may have a public title, technical identifier, and localized display label, but each layer has one canonical registry entry. Alternative typography and historical labels are aliases, not new products.

### 3.3 Technical neutrality

Runtime and Editor APIs use direct technical names. Verse flavor belongs in public titles, documentation headings, icons, optional samples, and restrained setup copy. No developer must understand Hackulos lore to identify a service, result, configuration asset, or failure.

### 3.4 Qualification beats abbreviation

Prefer `CharacterRuntimeInstanceId` over `ActorId` when several actor-like identities coexist. Prefer `SceneTransitionRequest` over `TransitionRequest` when the containing API does not make the domain obvious. Abbreviations are reserved for globally registered prefixes and widely understood platform terms.

### 3.5 Names survive presentation changes

Display labels, filenames, asset paths, scene names, and localized strings may change without changing domain IDs, package IDs, document IDs, or public API identities.

### 3.6 No accidental framework language

Names must not imply that one package owns the whole game. Generic terms such as `GameManager`, `GlobalManager`, `MasterController`, `CoreSystem`, or `UniversalService` are prohibited in public suite APIs unless an ADR proves an exact, bounded responsibility that cannot be named more clearly.

---

## 4. Suite identity and typography

### 4.1 Canonical suite identity

| Surface | Canonical value |
|---|---|
| Formal public title | **The Sperk’s Forge – EchoDevGames Game Systems Suite** |
| Short public title | **The Sperk’s Forge** |
| Accepted short alias | **Sperk’s Forge** |
| Continuity line | **Forged in the Hackulos Verse.** |
| Owner/publisher | **Jesse “Echo” Adams / EchoDevGames** |
| Documentation prefix | `SFGSS` |
| Package prefix | `com.echodevgames` |
| Assembly/namespace root | `EchoDevGames` |

### 4.2 Apostrophes and separators

- Public prose uses the typographic apostrophe in `Sperk’s` when supported.
- ASCII-only filenames, command lines, package IDs, and code use safe ASCII characters.
- The canonical formal package-title separator is a spaced en dash: `Title – Responsibility`.
- ASCII-only surfaces may use `Title - Responsibility`.
- An em dash, colon, or reversed title order found in an older document is a typography alias until SUITE-DOC-30 normalizes the source; it does not create a second package name.

### 4.3 Brand boundary

- `EchoDevGames` capitalization is exact.
- The suite is not an Isekai Studios product.
- `Isekai`, `Hackulos`, `Sperk`, or other Verse terms must not appear in package IDs, namespaces, assembly names, stable technical IDs, or mandatory game content unless a later accepted ADR changes the boundary.
- `Jukebot` is the intentional technical-name exception. Do not rewrite it as `EchoAudio`, `JukeBot`, or `Juke Bot`.

---

## 5. Canonical package identity registry

The following twenty-eight records are canonical. The short public title may appear alone in ordinary prose after the technical context is established. Formal listings use the full public title.

| Wave | Technical identifier | Canonical public title | Package ID | Namespace family | Diagnostic prefix |
|---|---|---|---|---|---|
| Foundation | `EchoLaunch` | **First Light – Startup and Launch** | `com.echodevgames.echo-launch` | `EchoDevGames.EchoLaunch` | `ELAUNCH` |
| Foundation | `EchoDiagnostics` | **The Observatory – Diagnostics and Runtime Inspection** | `com.echodevgames.echo-diagnostics` | `EchoDevGames.EchoDiagnostics` | `EDIAG` |
| Foundation | `EchoSettings` | **The Accord – Global Preferences** | `com.echodevgames.echo-settings` | `EchoDevGames.EchoSettings` | `ESET` |
| Foundation | `EchoSceneFlow` | **The Passage – Scene Flow** | `com.echodevgames.echo-scene-flow` | `EchoDevGames.EchoSceneFlow` | `ESF` |
| Foundation | `EchoGameState` | **The Pulse – Runtime State** | `com.echodevgames.echo-game-state` | `EchoDevGames.EchoGameState` | `EGSTATE` |
| Foundation | `Jukebot` | **Resonance – Audio Runtime** | `com.echodevgames.jukebot` | `EchoDevGames.Jukebot` | `JB` |
| Foundation | `EchoInput` | **The Will – Input Infrastructure** | `com.echodevgames.echo-input` | `EchoDevGames.EchoInput` | `EIN` |
| Foundation | `EchoUI` | **The Looking Glass – UI Framework** | `com.echodevgames.echo-ui` | `EchoDevGames.EchoUI` | `EUI` |
| Foundation | `EchoSave` | **The Chronicle – Save Infrastructure** | `com.echodevgames.echo-save` | `EchoDevGames.EchoSave` | `ESV` |
| Foundation | `EchoGameStarter` | **The Workshop – Project Starter** | `com.echodevgames.echo-game-starter` | `EchoDevGames.EchoGameStarter` | `EGS` |
| Expansion | `EchoFeedback` | **Impact – Coordinated Feedback** | `com.echodevgames.echo-feedback` | `EchoDevGames.EchoFeedback` | `EFB` |
| Expansion | `EchoPool` | **The Wellspring – Runtime Object Pooling** | `com.echodevgames.echo-pool` | `EchoDevGames.EchoPool` | `EPOOL` |
| Expansion | `EchoProgression` | **The Ascent – Progression, Unlocks, Passwords, and Checkpoints** | `com.echodevgames.echo-progression` | `EchoDevGames.EchoProgression` | `EPROG` |
| Expansion | `EchoBuildTools` | **The Foundry – Build Preparation, Validation, and Release Output** | `com.echodevgames.echo-build-tools` | `EchoDevGames.EchoBuildTools` | `EBUILD` |
| Expansion | `EchoLocalization` | **Many Tongues – Localization, Locale, and Regional Content** | `com.echodevgames.echo-localization` | `EchoDevGames.EchoLocalization` | `ELOC` |
| Expansion | `EchoDialogue` | **Voices – Dialogue and Conversation Flow** | `com.echodevgames.echo-dialogue` | `EchoDevGames.EchoDialogue` | `EDLG` |
| Expansion | `EchoObjectives` | **The Path – Objectives, Quests, and Tasks** | `com.echodevgames.echo-objectives` | `EchoDevGames.EchoObjectives` | `EOBJ` |
| Expansion | `EchoInventory` | **The Vault – Inventory and Item Containers** | `com.echodevgames.echo-inventory` | `EchoDevGames.EchoInventory` | `EINV` |
| Expansion | `EchoInteraction` | **The Hand – World Interaction** | `com.echodevgames.echo-interaction` | `EchoDevGames.EchoInteraction` | `EITR` |
| Expansion | `EchoCamera` | **The Eye – Camera Direction** | `com.echodevgames.echo-camera` | `EchoDevGames.EchoCamera` | `ECAM` |
| Expansion | `EchoCharacters` | **The Fellowship – Character Identity and Roster** | `com.echodevgames.echo-characters` | `EchoDevGames.EchoCharacters` | `ECHR` |
| Expansion | `EchoControllers` | **The Vessel – Player Controller Foundations** | `com.echodevgames.echo-controllers` | `EchoDevGames.EchoControllers` | `ECTR` |
| Expansion | `EchoCrafting` | **The Crucible – Recipe Transformation and Production** | `com.echodevgames.echo-crafting` | `EchoDevGames.EchoCrafting` | `ECRF` |
| Advanced | `EchoMultiplayer` | **The Convergence – Multiplayer Sessions and Authority** | `com.echodevgames.echo-multiplayer` | `EchoDevGames.EchoMultiplayer` | `EMUL` |
| Advanced | `EchoAI` | **Instinct – AI Perception, Decisions, and Behavior** | `com.echodevgames.echo-ai` | `EchoDevGames.EchoAI` | `EAI` |
| Advanced | `EchoCombat` | **Clash – Combat Messages, Targets, and Resolution** | `com.echodevgames.echo-combat` | `EchoDevGames.EchoCombat` | `ECLASH` |
| Advanced | `EchoAbilities` | **Arcana – Ability Activation and Effect Orchestration** | `com.echodevgames.echo-abilities` | `EchoDevGames.EchoAbilities` | `EABL` |
| Advanced | `EchoWorld` | **The Atlas – World Identity, Topology, and Travel Metadata** | `com.echodevgames.echo-world` | `EchoDevGames.EchoWorld` | `EWRLD` |

### 5.1 Registry guarantees

- Technical identifiers are globally unique inside the suite.
- Package IDs are globally unique and lowercase.
- Public short titles are unique inside the package catalog.
- Diagnostic prefixes are globally unique.
- Namespace families are package-qualified.
- A package title does not transfer technical ownership. `The Eye` is public identity; `EchoCamera` is the API family.

### 5.2 Editor-only package families

`EchoBuildTools` and `EchoGameStarter` are Editor-only in their approved MVPs. Their namespace families remain `EchoDevGames.EchoBuildTools` and `EchoDevGames.EchoGameStarter`, while their packages omit a production Runtime assembly unless a later specification revision authorizes one.

### 5.3 Advanced foundation status

The Convergence, Instinct, Clash, Arcana, and The Atlas are approved pre-code foundations. Their names are reserved and canonical even though provider, adapter, compatibility, and implementation evidence remains pending.

---

## 6. Documentation identifiers and filenames

### 6.1 Suite document families

| Artifact | Canonical ID pattern | Example |
|---|---|---|
| Suite Bible | `SFGSS-000` | `Echo_Game_Systems_Suite_Bible.md` |
| Suite standard | `SFGSS-###` | `SFGSS-008_Suite_Glossary_and_Naming_Registry.md` |
| Package authority | `SFGSS-PKG-<TECHNICAL>-###` | `SFGSS-PKG-ECHOLAUNCH-001` |
| Suite ADR | `SFGSS-ADR-###` | `SFGSS-ADR-003` |
| Integration specification | `SFGSS-INT-<SCOPE>-###` | `SFGSS-INT-EXPANSION-001` |
| Guided pathway | `PATH-###` | `PATH-080` |
| Documentation checkpoint | `SUITE-DOC-##` | `SUITE-DOC-27` |
| Historical Foundation checkpoint | `FW-DOC-##` | `FW-DOC-12` |
| Package learning review | `PKG-LEARN-###` | `PKG-LEARN-001` |
| Test report | `<CHECKPOINT>_<SUBJECT>_Audit_Report` | `SUITE-DOC-27_SFGSS-008_Naming_Audit_Report.md` |
| Artifact manifest | `<CHECKPOINT>_Artifact_Manifest.json` | `SUITE-DOC-27_Artifact_Manifest.json` |

### 6.2 Package document registry

| Technical identifier | Current document ID | Test ID pattern | Laboratory ID pattern | Workshop setup facade |
|---|---|---|---|---|
| `EchoLaunch` | `SFGSS-PKG-ECHOLAUNCH-001` | `ELAUNCH-T-###` | `ELAUNCH-LAB-###` | `EchoDevGames.EchoLaunch.Editor.Workshop.EchoLaunchWorkshopSetupFacade` |
| `EchoDiagnostics` | `SFGSS-PKG-ECHODIAGNOSTICS-001` | `EDIAG-T-###` | `EDIAG-LAB-###` | `EchoDevGames.EchoDiagnostics.Editor.Workshop.EchoDiagnosticsWorkshopSetupFacade` |
| `EchoSettings` | `SFGSS-PKG-ECHOSETTINGS-001` | `ESET-T-###` | `ESET-LAB-###` | `EchoDevGames.EchoSettings.Editor.Workshop.EchoSettingsWorkshopSetupFacade` |
| `EchoSceneFlow` | `SFGSS-PKG-ECHOSCENEFLOW-001` | `ESF-T-###` | `ESF-LAB-###` | `EchoDevGames.EchoSceneFlow.Editor.Workshop.EchoSceneFlowWorkshopSetupFacade` |
| `EchoGameState` | `SFGSS-PKG-ECHOGAMESTATE-001` | `EGSTATE-T-###` | `EGSTATE-LAB-###` | `EchoDevGames.EchoGameState.Editor.Workshop.EchoGameStateWorkshopSetupFacade` |
| `Jukebot` | `SFGSS-PKG-JUKEBOT-001` | `JB-T-###` | `JB-LAB-###` | `EchoDevGames.Jukebot.Editor.Workshop.JukebotWorkshopSetupFacade` |
| `EchoInput` | `SFGSS-PKG-ECHOINPUT-001` | `EIN-T-###` | `EIN-LAB-###` | `EchoDevGames.EchoInput.Editor.Workshop.EchoInputWorkshopSetupFacade` |
| `EchoUI` | `SFGSS-PKG-ECHOUI-001` | `EUI-T-###` | `EUI-LAB-###` | `EchoDevGames.EchoUI.Editor.Workshop.EchoUIWorkshopSetupFacade` |
| `EchoSave` | `SFGSS-PKG-ECHOSAVE-001` | `ESV-T-###` | `ESV-LAB-###` | `EchoDevGames.EchoSave.Editor.Workshop.EchoSaveWorkshopSetupFacade` |
| `EchoGameStarter` | `SFGSS-PKG-ECHOGAMESTARTER-001` | `EGS-T-###` | `EGS-LAB-###` | `EchoDevGames.EchoGameStarter.Editor.Workshop.EchoGameStarterWorkshopSetupFacade` |
| `EchoFeedback` | `SFGSS-PKG-ECHOFEEDBACK-001` | `EFB-T-###` | `EFB-LAB-###` | `EchoDevGames.EchoFeedback.Editor.Workshop.EchoFeedbackWorkshopSetupFacade` |
| `EchoPool` | `SFGSS-PKG-ECHOPOOL-001` | `EPOOL-T-###` | `EPOOL-LAB-###` | `EchoDevGames.EchoPool.Editor.Workshop.EchoPoolWorkshopSetupFacade` |
| `EchoProgression` | `SFGSS-PKG-ECHOPROGRESSION-001` | `EPROG-T-###` | `EPROG-LAB-###` | `EchoDevGames.EchoProgression.Editor.Workshop.EchoProgressionWorkshopSetupFacade` |
| `EchoBuildTools` | `SFGSS-PKG-ECHOBUILDTOOLS-001` | `EBUILD-T-###` | `EBUILD-LAB-###` | `EchoDevGames.EchoBuildTools.Editor.Workshop.EchoBuildToolsWorkshopSetupFacade` |
| `EchoLocalization` | `SFGSS-PKG-ECHOLOCALIZATION-001` | `ELOC-T-###` | `ELOC-LAB-###` | `EchoDevGames.EchoLocalization.Editor.Workshop.EchoLocalizationWorkshopSetupFacade` |
| `EchoDialogue` | `SFGSS-PKG-ECHODIALOGUE-001` | `EDLG-T-###` | `EDLG-LAB-###` | `EchoDevGames.EchoDialogue.Editor.Workshop.EchoDialogueWorkshopSetupFacade` |
| `EchoObjectives` | `SFGSS-PKG-ECHOOBJECTIVES-001` | `EOBJ-T-###` | `EOBJ-LAB-###` | `EchoDevGames.EchoObjectives.Editor.Workshop.EchoObjectivesWorkshopSetupFacade` |
| `EchoInventory` | `SFGSS-PKG-ECHOINVENTORY-001` | `EINV-T-###` | `EINV-LAB-###` | `EchoDevGames.EchoInventory.Editor.Workshop.EchoInventoryWorkshopSetupFacade` |
| `EchoInteraction` | `SFGSS-PKG-ECHOINTERACTION-001` | `EITR-T-###` | `EITR-LAB-###` | `EchoDevGames.EchoInteraction.Editor.Workshop.EchoInteractionWorkshopSetupFacade` |
| `EchoCamera` | `SFGSS-PKG-ECHOCAMERA-001` | `ECAM-T-###` | `ECAM-LAB-###` | `EchoDevGames.EchoCamera.Editor.Workshop.EchoCameraWorkshopSetupFacade` |
| `EchoCharacters` | `SFGSS-PKG-ECHOCHARACTERS-001` | `ECHR-T-###` | `ECHR-LAB-###` | `EchoDevGames.EchoCharacters.Editor.Workshop.EchoCharactersWorkshopSetupFacade` |
| `EchoControllers` | `SFGSS-PKG-ECHOCONTROLLERS-001` | `ECTR-T-###` | `ECTR-LAB-###` | `EchoDevGames.EchoControllers.Editor.Workshop.EchoControllersWorkshopSetupFacade` |
| `EchoCrafting` | `SFGSS-PKG-ECHOCRAFTING` | `ECRF-T-###` | `ECRF-LAB-###` | `EchoDevGames.EchoCrafting.Editor.Workshop.EchoCraftingWorkshopSetupFacade` |
| `EchoMultiplayer` | `SFGSS-PKG-ECHOMULTIPLAYER` | `EMUL-T-###` | `EMUL-LAB-###` | `EchoDevGames.EchoMultiplayer.Editor.Workshop.EchoMultiplayerWorkshopSetupFacade` |
| `EchoAI` | `SFGSS-PKG-ECHOAI` | `EAI-T-###` | `EAI-LAB-###` | `EchoDevGames.EchoAI.Editor.Workshop.EchoAIWorkshopSetupFacade` |
| `EchoCombat` | `SFGSS-PKG-ECHOCOMBAT` | `ECLASH-T-###` | `ECLASH-LAB-###` | `EchoDevGames.EchoCombat.Editor.Workshop.EchoCombatWorkshopSetupFacade` |
| `EchoAbilities` | `SFGSS-PKG-ECHOABILITIES` | `EABL-T-###` | `EABL-LAB-###` | `EchoDevGames.EchoAbilities.Editor.Workshop.EchoAbilitiesWorkshopSetupFacade` |
| `EchoWorld` | `SFGSS-PKG-ECHOWORLD` | `EWRLD-T-###` | `EWRLD-LAB-###` | `EchoDevGames.EchoWorld.Editor.Workshop.EchoWorldWorkshopSetupFacade` |

### 6.3 Grandfathered Advanced document IDs

The five Advanced foundations and The Crucible currently use approved document IDs without the final `-001` numeric suffix. Those IDs remain valid and must not be rewritten silently. SUITE-DOC-30 will decide whether to preserve them permanently as grandfathered IDs or revise the documents through explicit compatible metadata updates.

### 6.4 Filenames

- Repository filenames use portable ASCII where practical.
- Use underscores or hyphens consistently inside one document family.
- Filenames must not be treated as domain IDs.
- Renaming a documentation file requires updating Obsidian links and the graph roadmap in the same checkpoint.
- Version numbers normally live inside the document, not in the current authoritative filename. Historical exported checkpoint files may include a version suffix outside the repository.

---

## 7. Package IDs, repositories, assemblies, and namespaces

### 7.1 Package IDs

Core pattern:

```text
com.echodevgames.<kebab-name>
```

Provider pattern:

```text
com.echodevgames.<family>.<provider>
```

Bridge pattern:

```text
com.echodevgames.<package-a>-<package-b>
```

The integration specification fixes package ordering. A reverse-order duplicate is prohibited.

### 7.2 Repository names

Repository names use the technical identifier by default:

| Technical identifier | Planned repository | Package ID | Namespace family |
|---|---|---|---|
| `EchoLaunch` | `EchoDevGames/EchoLaunch` | `com.echodevgames.echo-launch` | `EchoDevGames.EchoLaunch` |
| `EchoDiagnostics` | `EchoDevGames/EchoDiagnostics` | `com.echodevgames.echo-diagnostics` | `EchoDevGames.EchoDiagnostics` |
| `EchoSettings` | `EchoDevGames/EchoSettings` | `com.echodevgames.echo-settings` | `EchoDevGames.EchoSettings` |
| `EchoSceneFlow` | `EchoDevGames/EchoSceneFlow` | `com.echodevgames.echo-scene-flow` | `EchoDevGames.EchoSceneFlow` |
| `EchoGameState` | `EchoDevGames/EchoGameState` | `com.echodevgames.echo-game-state` | `EchoDevGames.EchoGameState` |
| `Jukebot` | `EchoDevGames/Jukebot` | `com.echodevgames.jukebot` | `EchoDevGames.Jukebot` |
| `EchoInput` | `EchoDevGames/EchoInput` | `com.echodevgames.echo-input` | `EchoDevGames.EchoInput` |
| `EchoUI` | `EchoDevGames/EchoUI` | `com.echodevgames.echo-ui` | `EchoDevGames.EchoUI` |
| `EchoSave` | `EchoDevGames/EchoSave` | `com.echodevgames.echo-save` | `EchoDevGames.EchoSave` |
| `EchoGameStarter` | `EchoDevGames/EchoGameStarter` | `com.echodevgames.echo-game-starter` | `EchoDevGames.EchoGameStarter` |
| `EchoFeedback` | `EchoDevGames/EchoFeedback` | `com.echodevgames.echo-feedback` | `EchoDevGames.EchoFeedback` |
| `EchoPool` | `EchoDevGames/EchoPool` | `com.echodevgames.echo-pool` | `EchoDevGames.EchoPool` |
| `EchoProgression` | `EchoDevGames/EchoProgression` | `com.echodevgames.echo-progression` | `EchoDevGames.EchoProgression` |
| `EchoBuildTools` | `EchoDevGames/EchoBuildTools` | `com.echodevgames.echo-build-tools` | `EchoDevGames.EchoBuildTools` |
| `EchoLocalization` | `EchoDevGames/EchoLocalization` | `com.echodevgames.echo-localization` | `EchoDevGames.EchoLocalization` |
| `EchoDialogue` | `EchoDevGames/EchoDialogue` | `com.echodevgames.echo-dialogue` | `EchoDevGames.EchoDialogue` |
| `EchoObjectives` | `EchoDevGames/EchoObjectives` | `com.echodevgames.echo-objectives` | `EchoDevGames.EchoObjectives` |
| `EchoInventory` | `EchoDevGames/EchoInventory` | `com.echodevgames.echo-inventory` | `EchoDevGames.EchoInventory` |
| `EchoInteraction` | `EchoDevGames/EchoInteraction` | `com.echodevgames.echo-interaction` | `EchoDevGames.EchoInteraction` |
| `EchoCamera` | `EchoDevGames/EchoCamera` | `com.echodevgames.echo-camera` | `EchoDevGames.EchoCamera` |
| `EchoCharacters` | `EchoDevGames/EchoCharacters` | `com.echodevgames.echo-characters` | `EchoDevGames.EchoCharacters` |
| `EchoControllers` | `EchoDevGames/EchoControllers` | `com.echodevgames.echo-controllers` | `EchoDevGames.EchoControllers` |
| `EchoCrafting` | `Not yet recorded` | `com.echodevgames.echo-crafting` | `EchoDevGames.EchoCrafting` |
| `EchoMultiplayer` | `Not yet recorded` | `com.echodevgames.echo-multiplayer` | `EchoDevGames.EchoMultiplayer` |
| `EchoAI` | `Not yet recorded` | `com.echodevgames.echo-ai` | `EchoDevGames.EchoAI` |
| `EchoCombat` | `Not yet recorded` | `com.echodevgames.echo-combat` | `EchoDevGames.EchoCombat` |
| `EchoAbilities` | `Not yet recorded` | `com.echodevgames.echo-abilities` | `EchoDevGames.EchoAbilities` |
| `EchoWorld` | `Not yet recorded` | `com.echodevgames.echo-world` | `EchoDevGames.EchoWorld` |

`Not yet recorded` is an honest planning state, not permission to invent a URL. SFGSS-009 will finalize repository creation, versioning, and compatibility-catalog behavior.

### 7.3 Assembly families

| Assembly role | Pattern |
|---|---|
| Neutral runtime | `EchoDevGames.<Package>.Runtime` |
| Editor tooling | `EchoDevGames.<Package>.Editor` |
| Runtime tests | `EchoDevGames.<Package>.Tests.Runtime` |
| Editor tests | `EchoDevGames.<Package>.Tests.Editor` |
| Presentation adapter | `EchoDevGames.<Package>.Presentation.<Technology>` |
| Backend/provider adapter | `EchoDevGames.<Package>.<Provider>` |
| Bridge runtime | `EchoDevGames.<PackageA>.<PackageB>.Runtime` |
| Sample/Laboratory | `EchoDevGames.<Package>.Samples.<LabName>` |

### 7.4 Namespace families

- Public types begin under `EchoDevGames.<TechnicalIdentifier>`.
- Runtime, Editor, provider, presentation, bridge, sample, and test namespaces remain inside that family or the exact integration family.
- Do not use the global namespace in distributed package code.
- Do not place `Runtime`, `Editor`, or `Tests` into the core public namespace merely because the assembly uses that suffix; subnamespaces should reflect actual API organization.

---

## 8. Public API type-name vocabulary

The suffixes below carry suite-wide meaning. A package may add domain-specific names, but it must not redefine these suffixes to mean something incompatible.

| Suffix/term | Canonical meaning | Example |
|---|---|---|
| `Root` | Package-level lifetime and composition authority, usually one per declared scope | `EchoLaunchRoot` |
| `Service` | Public behavior API that owns or coordinates one concern | `IEchoInventoryService` |
| `Definition` | Immutable authored rule or reusable identity-bearing concept | `CraftingRecipeDefinition` |
| `Configuration` | Project-selected policy and references used to initialize behavior | `EchoLaunchConfiguration` |
| `Catalog` | Authored collection of definitions or schemas | `ItemCatalog` |
| `Registry` | Runtime or Editor lookup/registration authority | `ProviderRegistry` |
| `State` | Mutable authoritative values for one declared scope | `AbilityOwnerState` |
| `Snapshot` | Immutable detached read model captured at one revision/time | `RosterSnapshot` |
| `Record` | Durable, historical, diagnostic, or ledger entry | `RewardDeliveryRecord` |
| `Request` | Caller-provided intent that has not yet committed | `SceneTransitionRequest` |
| `Result` | Structured operation outcome, including failure/unavailable states | `CraftingResult` |
| `Plan` | Side-effect-free prepared operations or route proposal | `WorldTravelPlan` |
| `Prepared...` | Validated staged operation awaiting explicit commit/apply | `PreparedLoad` |
| `Receipt` | Durable evidence describing an attempted or completed operation | `BuildReceipt` |
| `Handle` | Stale-safe reference to an active runtime object/operation | `PoolHandle` |
| `Lease` | Ownership right that must be released and may be disposed out of order | `PauseLease` |
| `Registration` | Stale-safe provider/listener/participant registration token | `ProviderRegistration` |
| `Provider` | Implementation of a neutral capability or external backend | `ISceneLoadProvider` |
| `Adapter` | Translation to a technology, provider, project model, or presentation path | `CinemachineCameraAdapter` |
| `Bridge` | Reusable artifact connecting two independent package authorities | `EchoSettingsJukebotBridge` |
| `Presenter` | Converts package state into a display surface without owning domain truth | `LaunchStatusPresenter` |
| `Controller` | Actor-local behavior/control component, not a suite-wide authority | `TopDownController` |
| `Coordinator` | Orders work across collaborators while leaving truth with named owners | `TransitionCoordinator` |
| `Resolver` | Deterministically evaluates or resolves a request without hidden ownership | `CombatResolver` |
| `Policy` | Immutable or configuration-owned decision rules | `PoolExhaustionPolicy` |
| `Descriptor` | Metadata used to describe a registered or external capability | `PackageAdapterDescriptor` |
| `Context` | Bounded data supplied for one evaluation or operation | `CraftingContext` |
| `Intent` | Normalized input or desired action before package-specific execution | `TopDownMovementIntent` |
| `Participant` | A qualified contributor/member in a named domain | `SaveParticipant`, `SessionParticipant` |
| `Entry` | One authored or runtime member of a collection | `SplashEntry` |
| `Profile` | A qualified authored grouping or policy set; never use unqualified in cross-package APIs | `AudioProfile`, `BuildProfile` |

### 8.1 Interfaces

- Public capability interfaces begin with `I`.
- Interface names describe the capability, not the implementing technology, unless the interface belongs to a provider adapter.
- Avoid empty marker interfaces unless the package specification defines a validated need.

### 8.2 Async methods

Public methods that return `Awaitable`, `Task`, or another asynchronous result should use the `Async` suffix unless they implement a Unity message, exact provider protocol, or existing framework interface that fixes the name.

### 8.3 Events

- Event names describe the completed semantic change, such as `StateChanged`, `SceneTransitionCompleted`, or `CharacterSelected`.
- Avoid using `On` as part of a public event name; `On...` may remain the private handler method that responds to the event.
- Events publish after authoritative state changes unless the event is explicitly named as a preview, request, or preparation stage.

---

## 9. Stable identifiers and qualified identity names

### 9.1 ID type rules

- Public APIs prefer validated wrapper types such as `CharacterId`, `SceneId`, or `SaveParticipantId` over undifferentiated strings.
- A property named only `Id` is acceptable inside a type whose domain is unambiguous. Cross-package requests and reports use the qualified name.
- Durable IDs, runtime instance IDs, provider IDs, Unity asset GUIDs, and display names remain distinct.
- An ID suffix does not guarantee durability. The owning specification defines lifetime and domain.

### 9.2 Required identity qualifications

| Ambiguous concept | Approved qualified examples |
|---|---|
| Player | `InputUserId`, `SessionParticipantId`, `ControlOwnerId`, `CharacterId` |
| Actor | `CharacterRuntimeInstanceId`, `AIAgentId`, `CombatTargetId` |
| Target | `CameraTargetId`, `InteractionEndpointId`, `AbilityTargetId`, `CombatTargetId` |
| Location | `WorldLocationId`, `SceneId`, `EntryMarkerId`, `SpawnMarkerId` |
| State | `GameStateId`, `SettingsSectionState`, `AbilityOwnerState`, `ObjectiveRunState` |
| Profile | `AudioProfileId`, `BuildProfileId`, `LocaleProfileId` |
| Participant | `SaveParticipantId`, `SessionParticipantId`, `WorldStateParticipantId` |
| Focus | `UIFocus`, `InteractionFocus`, `CameraFocusTarget` |
| Selection | `CharacterSelectionContext`, `TrackedObjectiveId`, `SelectedLocaleId` |
| Owner | `ControlOwnerId`, `ContainerOwnerId`, `AuthorityOwner`, `ProjectOwner` |

### 9.3 Identity aliases

Aliases preserve compatibility and map a retired stable ID to one canonical current ID. They are not display synonyms. Alias cycles, ambiguous fan-in, and reuse of tombstoned IDs remain prohibited by SFGSS-003.

---

## 10. Diagnostic, test, Laboratory, and validation prefixes

### 10.1 Canonical package prefixes

The package registry in Section 5 reserves each diagnostic prefix. Package diagnostics use:

```text
<PREFIX>-<CATEGORY>-###
```

or, where the package already approved a flat form:

```text
<PREFIX>-###
```

Tests and Laboratories use:

```text
<PREFIX>-T-###
<PREFIX>-LAB-###
```

### 10.2 Prefix rules

- Prefixes are uppercase ASCII.
- Prefixes are globally unique and never recycled.
- A prefix identifies the artifact that owns the diagnostic, not the package that happened to observe it.
- Bridge and provider prefixes must be registered before release.
- Do not construct a bridge prefix by casually concatenating initials; the integration specification selects a readable unique prefix.
- Category segments such as `CFG`, `ROOT`, `LIFE`, `DATA`, or `MIG` may repeat across packages because the package prefix owns global uniqueness.

### 10.3 EchoSave correction

`ESV` is the canonical EchoSave prefix. The historical `ESAVE-T-100` example in SFGSS-004 was illustrative but conflicted with the approved package specification. SFGSS-004 v1.1.0 corrects that example to `ESV-T-100`.

### 10.4 Validation check IDs

Editor validation findings may use:

```text
<PREFIX>-VAL-###
```

when the package needs a separate validator registry. A package may instead reuse stable diagnostic codes when the same meaning applies in Editor and runtime. The package specification states which model it uses.

---

## 11. Bridge, provider, adapter, and presentation naming

### 11.1 Bridge ordering

A bridge has one canonical package order fixed by its integration specification. Prefer the behavioral owner first when one package clearly owns the integration behavior. Do not publish both `EchoSettings.Jukebot` and `Jukebot.EchoSettings` for the same connection.

### 11.2 Bridge types

```text
EchoDevGames.<PackageA>.<PackageB>
EchoDevGames.<PackageA>.<PackageB>.Runtime
com.echodevgames.<package-a>-<package-b>
```

Types should describe the connected behavior, not merely repeat both package names when a more precise name exists.

### 11.3 Provider adapters

```text
EchoDevGames.<Family>.<Provider>
com.echodevgames.<family>.<provider>
```

Use the provider’s official capitalization in display documentation and the approved safe slug in package IDs. Provider trademarks do not change the neutral core’s public API names.

### 11.4 Presentation adapters

```text
EchoDevGames.<Package>.Presentation.UGUI
EchoDevGames.<Package>.Presentation.UIToolkit
```

Presentation assemblies use exact technology labels registered by the package specification. `UI` alone is too ambiguous for an adapter assembly.

### 11.5 Project adapters

Project-owned adapters live under the project namespace, not `EchoDevGames`. Example:

```text
Rescuers2D.Integrations.EchoCharactersControllerAdapter
```

---

## 12. Scenes, samples, Laboratories, and generated content

### 12.1 Canonical evidence terms

| Term | Meaning |
|---|---|
| **Standalone Laboratory** | Isolated proof of one package or independently selectable feature with only declared hard dependencies. |
| **Integration Laboratory** | Proof owned by a bridge/provider artifact connecting explicit authorities. |
| **Showcase** | Combined presentation that demonstrates composition but does not replace isolated proof. |
| **Sample** | Importable/removable example content. A sample may contain a Laboratory or showcase. |
| **Fixture** | Deterministic data or project state used by automated/manual tests. |

`Test Lab` is an accepted short prose alias. Formal headings, registries, and release evidence use `Standalone Laboratory` or `Integration Laboratory`.

### 12.2 Scene names

Recommended sample-scene names:

```text
<PackageShortTitle> Standalone Laboratory
<PackageA> + <PackageB> Integration Laboratory
<Feature> Showcase
```

Scene names are presentation and project assets, not durable scene or world IDs.

### 12.3 Generated content

Generated project files use project-owned namespaces, folders, and IDs. The Workshop receipt records which package operation created them. Generated content must not impersonate immutable package source.

---

## 13. Canonical suite glossary

| Term | Canonical meaning |
|---|---|
| **Authority** | The one package/project component that owns the current truth and commits changes for a concern. |
| **Authority root** | The package-level lifetime/composition object that claims authority before side effects. |
| **Application-session** | Lifetime from package initialization until application shutdown or explicit package shutdown. |
| **Actor** | A spawned runtime entity capable of movement, behavior, control, interaction, or combat; not automatically a durable character. |
| **Adapter** | Translation between a neutral contract and a technology, provider, presentation path, or project model. |
| **Backend** | Technical implementation behind a provider-neutral capability, such as filesystem, camera, navigation, or networking. |
| **Bridge** | Reusable integration artifact connecting two independent authorities without replacing either. |
| **Capability** | A bounded behavior exposed by a package/provider and described by explicit contracts. |
| **Catalog** | Authored collection of definitions, schemas, or entries. |
| **Commit** | The declared point at which an authority publishes an irreversible or authoritative state change. |
| **Commit owner** | The one authority responsible for committing a multi-package workflow’s primary truth. |
| **Configuration** | Project-owned authored policy and references used to initialize or tune a package. |
| **Context** | Bounded inputs supplied for one evaluation, request, preview, or operation. |
| **Definition** | Immutable authored reusable rule or identity-bearing concept. |
| **Descriptor** | Metadata that describes a package, provider, adapter, setup facade, or registered capability. |
| **Diagnostic** | Structured package-owned information explaining health, state, warnings, failures, or recent operations. |
| **Domain ID** | Stable identifier defined by the owning package/project domain, separate from Unity asset identity and display text. |
| **DTO** | Detached serializable data-transfer object containing no live Unity/service/provider object graph. |
| **Evidence** | Retained observation from an exact test environment, not a planned test or design claim. |
| **Executor** | Typed handler that performs one explicitly registered operation or effect. |
| **Facade** | Narrow public entry surface that hides internal composition without hiding ownership; Workshop setup facades follow ADR-001. |
| **Fallback** | Explicit lower-priority alternative used when the preferred resource, provider, locale, route, or operation is unavailable. |
| **Generation** | Monotonic/stale-safety value that invalidates reused handles or records; not automatically a software version. |
| **Handle** | Stale-safe reference to an active instance/operation; does not necessarily own release. |
| **Idempotency** | Repeating the same identified request does not duplicate its committed result. |
| **Integration** | Optional behavior created by connecting named authorities through a bridge, adapter, or project code. |
| **Laboratory** | Controlled evidence surface for isolated or integrated package behavior. |
| **Lease** | Explicit ownership/right that remains active until released or disposed. |
| **Lifecycle** | Declared creation, validation, ready, operation, scene/reload, shutdown, and cleanup sequence. |
| **Migration** | Explicit forward conversion from an older supported schema/identity to a newer one. |
| **Modifier** | Ordered, bounded transform applied during an evaluation without silently becoming the underlying authority. |
| **Participant** | Qualified contributor/member in a named domain, such as save or multiplayer. |
| **Pathway** | Visible staged package-selection guidance defined by SFGSS-006, not a hidden bundle. |
| **Plan** | Side-effect-free description of proposed operations, routes, or changes. |
| **Prepared operation** | Validated staged operation awaiting explicit commit or apply. |
| **Presenter** | Presentation component that converts domain state to a visible surface without owning the domain truth. |
| **Profile** | Qualified authored set of related policy/data, such as audio or build settings. |
| **Provider** | Registered implementation of a neutral capability or connection to a backend/vendor. |
| **Publication point** | Moment when staged work becomes authoritative/visible and rollback guarantees change. |
| **Receipt** | Durable evidence describing what an operation attempted, changed, produced, restored, or failed to do. |
| **Record** | Durable, historical, ledger, or diagnostic entry with declared ownership and versioning. |
| **Registration** | Stale-safe token representing an active provider, listener, participant, target, or adapter registration. |
| **Registry** | Authority that validates, stores, and resolves registered entries. |
| **Request** | Structured caller intent before commitment. |
| **Result** | Structured outcome including success, denial, unavailability, failure, cancellation, or stale state. |
| **Root** | Package-level composition/lifetime authority. Not every component is a root. |
| **Runtime instance ID** | Session-only identity for one active spawned/created instance. |
| **Semantic event** | Meaningful package-owned event published after a truth change or at a clearly named preview stage. |
| **Service** | Public behavior API for one authority/capability. |
| **Snapshot** | Immutable detached view captured at one revision or time. |
| **Stable ID** | Identifier whose owning contract preserves meaning across rename, move, serialization, and supported migration. |
| **State** | Mutable values owned by one declared authority and scope. |
| **Tombstone** | Preserved retired identity preventing accidental reuse and explaining removal/migration. |
| **Transaction** | Validated staged mutation with one real commit owner and documented rollback class. |
| **Unknown data** | Preserved opaque optional record whose owner/schema is unavailable; never executed blindly. |
| **Validation** | Side-effect-free inspection against documented rules. |
| **Version** | Qualified compatibility number such as package SemVer, schema version, protocol version, or receipt format version. |
| **Wave** | Documentation/development priority grouping, not implementation or compatibility status. |

---

## 14. Ambiguous terms that require qualification

The following unqualified terms are discouraged or prohibited in public cross-package APIs and durable reports.

| Avoid alone | Why ambiguous | Use instead |
|---|---|---|
| `Player` / `PlayerId` | May mean input user, network participant, character, controller owner, or save profile | Name the exact identity domain |
| `Target` / `TargetId` | Camera, interaction, ability, combat, AI, and objective targets differ | `CombatTargetId`, `CameraTargetId`, etc. |
| `State` | Every package owns different state | `GameState`, `AbilityOwnerState`, `SaveOperationState` |
| `Profile` | Audio, build, settings, locale, character, and platform profiles differ | Qualify the profile |
| `Manager` | Hides whether the type owns truth, coordinates work, stores data, or presents UI | `Root`, `Service`, `Registry`, `Coordinator`, `Presenter`, or domain-specific name |
| `Controller` | May mean player motor, UI presenter, input adapter, or orchestration component | Reserve for actor/local control or qualify the domain |
| `Owner` | Project owner, control owner, resource owner, and authority owner differ | Use qualified owner name |
| `Current` | Current according to which authority/revision/session? | Name the source and revision where relevant |
| `Active` | May mean selected, running, spawned, enabled, focused, or authoritative | Use the exact lifecycle/state term |
| `Global` | Often disguises an unbounded singleton | Name the scope: application-session, project, slot, scene, actor, or provider |
| `Core` | May imply mandatory shared dependency | Use the exact neutral package or contract family |
| `System` | Too broad for a public type | Name the concern: save service, scene flow, inventory, etc. |
| `Data` | Does not reveal definition/configuration/state/snapshot/record | Use the classified term |
| `Save` | May mean settings, progression, slot transport, snapshot, or write operation | `SaveSlot`, `SettingsDocument`, `RosterSnapshot`, etc. |
| `Event` | Does not reveal semantic timing | `CharacterSelected`, `RequestCommitted`, `PreviewUpdated` |
| `Callback` | Hides ownership and lifecycle | Prefer event, handler, provider, executor, or completion result |
| `Helper` / `Utility` | Tends to become miscellaneous ownership | Name the exact pure operation or service |

---

## 15. Reserved, prohibited, deprecated, and historical names

### 15.1 Reserved suite identities

- All twenty-eight technical identifiers and public short titles in Section 5.
- `The Sperk’s Forge`, `Sperk’s Forge`, `EchoDevGames`, `SFGSS`, and `com.echodevgames`.
- `EchoRPG.Foundation` as the optional genre-specific RPG data family outside the general suite core.
- `SFGSS-ADR-004` as the next available suite ADR only after a candidate enters Proposed review.

### 15.2 Prohibited without an ADR

- `EchoCore`, `EchoFoundation`, or another mandatory general shared-core package.
- Technical package IDs or namespaces containing `Hackulos`, `Sperk`, or `Isekai`.
- `GameManager`, `GlobalManager`, `MasterManager`, or `UniversalManager` as public suite authority types.
- A second package using a registered diagnostic prefix, package ID, technical identifier, or public short title.
- Reverse-order duplicate bridge packages for one behavior.

### 15.3 Deprecated or historical names

| Historical/deprecated term | Canonical replacement | Rule |
|---|---|---|
| `EchoBootstrap` | `EchoLaunch` / First Light | Removed as a separate concept; do not create new APIs or packages under the old name. |
| `Bootstrap` as the package title | First Light – Startup and Launch | May appear descriptively for a boot pattern, not as the product identity. |
| `Test Lab` | Standalone Laboratory or Integration Laboratory | Accepted short prose alias; formal evidence uses the qualified Laboratory term. |
| `Singleton` as an authority name | Package root/service with declared scope | Singleton describes an implementation pattern, not the owned concern. |
| `GameManager` | Exact package/project authority name | Existing project code may retain it temporarily during migration, but new suite APIs do not. |
| `JukeBot` / `Juke Bot` | `Jukebot` | Capitalization and spacing are fixed. |
| `ESAVE` diagnostic/test prefix | `ESV` | Historical example only; never allocate new IDs under `ESAVE`. |

### 15.4 Provider and vendor names

Provider adapters use the provider’s official display spelling and a stable lowercase package slug. Provider names are not reserved for neutral core APIs and do not imply endorsement, support, or compatibility until evidence exists.

---

## 16. Synonyms, aliases, and deprecation rules

### 16.1 Documentation synonyms

A documentation synonym may improve readability but must link or resolve to one canonical term. Example: `Test Lab` may link to `Standalone Laboratory`.

### 16.2 API aliases

Public API aliases require:

- a package specification revision;
- a deprecation message naming the replacement;
- migration guidance;
- a removal version or review trigger;
- serialization and reflection impact review;
- tests proving old and new surfaces behave as documented during the compatibility window.

### 16.3 Stable ID aliases

Stable ID aliases follow SFGSS-003 and are data migrations, not wording changes. Documentation synonyms must never be loaded as ID aliases automatically.

### 16.4 Public title changes

Changing a package’s public short title requires an ADR because it affects repository presentation, Package Manager display, documentation links, icons, portfolio materials, release history, and user recognition. Technical identifier changes additionally require package, namespace, assembly, serialization, migration, and distribution planning.

### 16.5 Deprecation language

Use exact lifecycle words:

- **Deprecated:** supported temporarily with a replacement and removal plan.
- **Superseded:** a document/decision is replaced but preserved historically.
- **Removed:** no longer part of the supported surface.
- **Historical alias:** appears in old material but is not an active supported API.

---

## 17. Capitalization, punctuation, pluralization, and abbreviations

### 17.1 Capitalization

- `EchoDevGames`, `Jukebot`, `ScriptableObject`, `GameObject`, `TextMeshPro`, `UI Toolkit`, and Unity product names retain official casing.
- C# public types and members use PascalCase.
- Local/private implementation naming is governed by the eventual code style, but public serialized field names and DTO member changes require compatibility review.
- Package IDs and domain semantic IDs use lowercase according to their contracts.

### 17.2 Pluralization

- Types representing one item use singular nouns: `ItemDefinition`, `SceneRoute`, `SaveSlot`.
- Collections use plural nouns or a collection suffix: `Items`, `SceneRoutes`, `ItemCatalog`.
- Enum type names are singular unless the enum is explicitly marked as a flags set.
- Avoid inconsistent singular/plural pairs such as `ObjectivesService` versus `ObjectiveService`; use the technical package family when appropriate, such as `IEchoObjectivesService`.

### 17.3 Abbreviations

Approved common abbreviations include `ID` in prose, `Id` in C# type/member names, `UI`, `SFX`, `VFX`, `FPS`, `DTO`, `ADR`, `API`, `SDK`, `UPM`, and platform/provider official abbreviations. Unregistered package initials do not become public prefixes automatically.

### 17.4 Numbers and versions

- Package versions use SemVer.
- Schema, protocol, and document versions state their own format and are not inferred from package SemVer.
- Stable IDs and test IDs use zero-padded numeric segments where the registry specifies them.
- Do not encode changing version numbers into namespaces or package IDs unless a major protocol/provider family explicitly requires it.

---

## 18. Naming validation and registry maintenance

### 18.1 Required static checks

Before a specification or release is approved, validate:

- technical identifier uniqueness;
- public short-title uniqueness;
- package ID uniqueness and lowercase format;
- namespace/assembly family consistency;
- diagnostic, test, Laboratory, provider, and bridge prefix uniqueness;
- document ID uniqueness;
- exact Workshop setup facade identity;
- no prohibited/reserved-name misuse;
- no unqualified cross-package `PlayerId`, `TargetId`, or equivalent ambiguous identity;
- filenames and Obsidian links resolve;
- deprecated names include migration/replacement information.

### 18.2 Registry updates

A new package, bridge, provider, or public diagnostic family updates SFGSS-008 and its JSON companion in the same checkpoint as the approving specification or ADR.

### 18.3 Validation severity

| Finding | Default severity |
|---|---|
| Duplicate package ID, technical identifier, diagnostic prefix, or document ID | Blocker |
| Prohibited mandatory-core or ownership-obscuring name | Blocker |
| Namespace/assembly mismatch that breaks compile direction | Error/Blocker |
| Unqualified identity likely to cross package boundaries | Error |
| Public-title punctuation alias | Advisory until normalization checkpoint |
| Missing planned repository | Advisory before repository creation; blocker before distribution |
| Deprecated name without replacement/migration | Error |
| Broken Obsidian link after rename | Error |

---

## 19. Migration and compatibility behavior

- Names that participate in serialization, reflection, setup-facade lookup, provider protocols, package manifests, asmdefs, documentation links, or user workflows are compatibility surfaces.
- Renaming a C# serialized field uses Unity-compatible migration support and retained tests.
- Renaming a public type or namespace requires a package specification revision and migration plan.
- Renaming an assembly or package ID is a major distribution event and normally requires an ADR.
- Renaming a display label does not change a stable domain ID.
- Diagnostic and test IDs are never recycled after release.
- Historical package names remain searchable in changelogs and migration guides.

---

## 20. Current reconciliation findings

The SUITE-DOC-27 audit produced these results:

1. All twenty-eight technical identifiers, package IDs, namespace families, public short titles, and diagnostic prefixes are unique.
2. Public-title separators vary among existing package documents. SFGSS-008 registers the en-dash form as canonical and treats current punctuation as aliases until SUITE-DOC-30 normalizes source documents.
3. SFGSS-004 contained one `ESAVE-T-100` example that conflicted with EchoSave’s approved `ESV` prefix. SFGSS-004 v1.1.0 corrects the example.
4. The five Advanced package foundations and The Crucible use grandfathered document IDs without `-001`. They remain valid pending SUITE-DOC-30 review.
5. EchoBuildTools and EchoGameStarter are Editor-only in their MVPs. Their namespace families are now registered without implying a runtime assembly.
6. Several planned Advanced repositories are not yet recorded. SFGSS-009 owns repository creation and versioning decisions.
7. The stale Crafting open-decision wording in SFGSS-000 remains queued for SUITE-DOC-30, as previously recorded. This naming checkpoint does not silently alter an unrelated decision-history item.

---

## 21. Approval

### 21.1 Approval checklist

- [x] Twenty-eight package identity records are complete and unique.
- [x] Public title, technical identifier, package ID, namespace family, document ID, diagnostic prefix, test prefix, and setup-facade identities are registered.
- [x] Core API suffixes and lifecycle terms have canonical meanings.
- [x] Ambiguous cross-package terms require qualification.
- [x] Reserved, prohibited, deprecated, and historical names are documented.
- [x] Alias and naming-migration rules are explicit.
- [x] SFGSS-004’s EchoSave prefix example is corrected.
- [x] Machine-readable registry companion generated.
- [x] No implementation or empirical evidence was created.

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** 2026-08-04  
**Next checkpoint:** SUITE-DOC-28 – SFGSS-009 Repository, Versioning, and Integration Workspace Standard

---

## Graph Navigation

#sfgss/standard #sfgss/naming #sfgss/glossary

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-002_Dependency_Bridge_and_Assembly_Standard|SFGSS-002 Dependency and Assembly Standard]]
- [[SFGSS-003_Data_IDs_Serialization_and_Migration_Standard|SFGSS-003 Data and Identity Standard]]
- [[SFGSS-004_Testing_Validation_Test_Labs_and_Release_Standard|SFGSS-004 Testing Standard]]
- [[Architecture Decision Records/SFGSS-ADR-LOG_Suite_Decision_Log|Suite Decision Log]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
