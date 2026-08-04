# The Crucible – Recipe Transformation and Production Package Specification

**Document ID:** SFGSS-PKG-ECHOCRAFTING  
**Specification version:** 1.0.1
**Status:** Approved  
**Technical package name:** EchoCrafting  
**Public title:** The Crucible – Recipe Transformation and Production
**Package ID:** `com.echodevgames.echo-crafting`  
**Runtime namespace:** `EchoDevGames.EchoCrafting`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Repository:** Planned `EchoDevGames/EchoCrafting`; actual remote not yet evidenced
**Current Notes:** `../Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1; public minimum remains a release-time compatibility decision  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, SFGSS-005 v1.1.0  
**Required workshop record:** `../Research Records/SUITE-DOC-17_EchoCrafting_Design_Workshop_Record.md`  
**Last updated:** August 4, 2026

> “Bring only what the recipe asks. The Crucible will decide what may become.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoCrafting. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification after the required crafting design workshop | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved provider-neutral recipe transaction engine, Simple Combine and standard immediate MVP, recipe knowledge, station/context requirements, deterministic preview, atomic one-provider execution, diagnostics, authoring, Laboratory, and later capability seams | Jesse “Echo” Adams |
| 1.0.1 | 2026-08-04 | Approved | Normalized registry metadata and formal title; added the SUITE-DOC-30 governing-authority, evidence, test-registry, and compatibility clarification without authorizing implementation. | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Crucible – Recipe Transformation and Production
**Technical identifier:** EchoCrafting  
**Flavor line:** Bring only what the recipe asks. The Crucible will decide what may become.  
**Plain-language subtitle:** A standalone Unity package for authored recipe matching, crafting previews, provider-neutral requirements, atomic immediate transformations, recipe knowledge, stations and contexts, diagnostics, validation, and deliberate production-extension seams.

**One-sentence ownership contract:**

> EchoCrafting owns recipe definitions and catalogs, Simple Combine and standard recipe matching, side-effect-free previews, provider-neutral crafting contexts and requirements, one-resource-provider reservation and atomic immediate execution, request idempotency, recipe knowledge, station registration, semantic results, diagnostics, authoring, validation, the standalone Crucible Laboratory, and explicit seams for timed jobs, queues, quality, failure, salvage, repair, upgrade, persistence, UI, and multiplayer; it does not own item definitions or inventory storage, player skills or professions, production UI, audio or feedback playback, save files, character state, objective truth, networking authority, or project-specific item mutation rules.

### 1.1 Elevator summary

The Crucible turns authored requirements into an explicit transformation plan. A game asks to craft one recipe in one context. EchoCrafting resolves the recipe, evaluates read-only conditions, asks one mutation-capable resource provider to prepare a deterministic plan, and commits inputs plus outputs atomically inside that provider's authority. The package publishes immutable previews, results, knowledge state, diagnostics, and events. It never reaches into an inventory, character, skill database, UI, or save file by assumption.

The MVP intentionally serves two scales. **Simple Combine** supports exact authored combinations such as Hackulos's rat-tail-and-human-eye quest bag. **Standard Immediate Crafting** supports quantities, alternatives, tags, non-consumable tools, station/context requirements, batches, outputs, byproducts, and recipe knowledge. Both use the same transaction spine, so the small quest mechanic is not discarded when a future game adds workshops or professions.

Timed production, station queues, quality, random or conditional failure, salvage, repair, upgrades, and offline completion are designed as later capabilities. Their seams are defined now, but they do not inflate the first release or masquerade as tested functionality.

### 1.2 Why this belongs in The Sperk's Forge

Hackulos already needs an exact combine bag. Future RPGs and adventure games may need workstations, recipes, tools, professions, queues, repair, or salvage. Game jams may only need two tokens converted into one result. Without a neutral package, every project either hard-codes recipe comparisons in UI buttons or imports an RPG-sized crafting manager that owns inventories, skills, menus, and item stats.

The package is justified because the reusable part is not “a blacksmith screen.” It is the careful transformation contract: stable recipes, deterministic matching, side-effect-free preview, resource reservation, atomic commit, idempotency, diagnostics, and explicit authority boundaries.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Always paired with “Recipe Transformation and Production.” |
| Setup guidance/tooltips | Yes | Must explain recipe, provider, station, requirement, and transaction behavior plainly. |
| Samples | Optional | Crucible imagery may decorate the Laboratory but is removable. |
| Runtime API/type names | No lore-only names | Use `CraftingRecipeDefinition`, `CraftingPreview`, and `ICraftingResourceProvider`. |
| Project data | No required Verse content | Games own ingredients, items, stations, professions, art, UI, and outcomes. |

---

## 2. Problem Statement

### 2.1 Current problem

Crafting code often begins in a UI button: read one inventory, compare item names, subtract inputs, add output, play an animation. The first failure immediately exposes hidden coupling. Output capacity may fail after inputs are removed. Two clicks may grant twice. A recipe rename may break a save. A timed queue may reserve resources forever. A multiplayer client may assert ingredients it does not own. Repair and upgrade may mutate unique items through a stack API that cannot roll back.

The opposite failure is a universal crafting framework that requires items, skills, professions, stations, quality, randomness, UI, and saves before one exact quest recipe can work. It solves every imagined economy and no actual small project.

EchoCrafting must provide a small but durable transaction core. It must work with a simulated provider in isolation, connect to The Vault through a separate bridge, and leave item, skill, UI, save, audio, feedback, and network truth with their existing authorities.

### 2.2 Evidence from existing work

| Source | Existing need or problem | Preserve | Improve |
|---|---|---|---|
| Hackulos | Exact quest recipe combines rat tail and human eye in a bag | Authored, intentionally tiny quest mechanic | Route through stable recipe IDs, preview, atomic provider transaction, and semantic result |
| DeverQuest / EchoRPG.Foundation direction | Large data catalogs and future tradeskills | Data-driven definitions | Keep RPG skills and items outside general crafting core |
| The Vault | Atomic inventory transactions, stable item IDs, unique instances, capacity | Provider can prepare inputs and outputs together | Use a separate bridge; do not create an inventory dependency |
| The Path | Objectives may require or reward crafting | Semantic result events and reward requests | Objectives never become the recipe engine |
| The Ascent | Recipe discovery may be granted by progression | Stable unlock requests | Crafting owns recipe knowledge; Progression owns broader unlock truth |
| Voices / The Hand | Dialogue or interaction may start crafting | Explicit requests | They do not mutate resources directly |
| The Chronicle | Durable state and migrations | Versioned detached snapshots | Save transport remains outside EchoCrafting |
| Convergence candidate | Server-authoritative transactions | Provider-neutral request/result contracts | Never trust client preview as authority |

### 2.3 Consequences of doing nothing

- Recipe logic remains duplicated in UI or quest scripts.
- Inputs can be consumed when outputs cannot be granted.
- Duplicate requests create duplicate items.
- Item names, scene objects, and UI selections become durable IDs.
- Crafting becomes inseparable from one inventory or RPG data model.
- Timed jobs and queues are added without reservation or cancellation policy.
- Repair and upgrades corrupt unique item state.
- Multiplayer crafting trusts unverified clients.
- Exact quest recipes either become throwaway code or drag in a giant system.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide stable, immutable recipe and catalog definitions.
- Support Simple Combine and standard immediate recipes through one transaction engine.
- Keep previews read-only, deterministic, and revision-aware.
- Keep resource mutation behind an explicit provider contract.
- Guarantee atomic immediate mutation when one provider owns inputs and outputs.
- Preserve standalone use without The Vault or any other Echo package.
- Support exact definitions, tags, alternatives, quantities, tools, catalysts, stations, contexts, outputs, byproducts, and batches.
- Provide recipe knowledge and visibility policies without owning the event that grants discovery.
- Prevent duplicate execution through request IDs and bounded idempotency records.
- Expose clear cancellation and commit-point behavior.
- Define honest extension seams for timed crafting, queues, quality, failure, salvage, repair, upgrades, persistence, UI, and multiplayer.
- Provide actionable diagnostics, authoring, validation, and a standalone Laboratory.

### 3.2 Non-goals

- Define items, currencies, skills, professions, attributes, or character stats.
- Store inventory containers or equipment.
- Render the production crafting UI.
- Play audio, camera, haptic, or VFX feedback.
- Own quest or objective state.
- Own save files or slots.
- Provide multiplayer transport or authority.
- Promise distributed atomicity across arbitrary providers.
- Ship random quality or failure in the MVP.
- Ship timed queues, offline completion, repair, or upgrade in the MVP.
- Force every recipe into an RPG profession model.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean Unity project | Import the Laboratory, inspect one recipe catalog, preview, combine, reset, and understand each result |
| Programmer | Custom tokens or inventory | Implement one resource provider without editing package source |
| Designer | Authored content | Create recipes, ingredient groups, stations, outputs, and visibility policies with validation |
| Quest designer | Exact combine mechanic | Author one strict combination without building a full tradeskill UI |
| Systems designer | Larger crafting plan | Add provider-backed requirements and later production modules without replacing the transaction core |
| Tester | Package under review | Exercise success, missing data, stale revisions, cancellation, provider failures, duplicates, and removal in isolation |

### 3.4 Measurable success criteria

- Clean supported project installation produces zero compile errors.
- Core runs without The Vault, The Chronicle, The Looking Glass, or any other Echo runtime package.
- One exact combine and one standard immediate recipe pass the Laboratory acceptance plan.
- Preview never mutates provider or crafting state.
- One-provider immediate transactions never consume inputs without granting approved outputs.
- Duplicate request IDs never grant duplicate results.
- Missing providers and requirements fail visibly and safely.
- Recipe and knowledge IDs survive display-name and asset-path changes.
- Samples are removable without breaking runtime assemblies.
- Every empirical result remains `Not run` until executed.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo Unity developers.
- Gameplay and systems programmers.
- Content and quest designers.
- RPG, adventure, puzzle, survival, and game-jam teams.
- Testers and maintainers validating provider transactions.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| ECRF-UC-001 | Preview exact combine | Player/project | Recipe known or match-probe allowed; provider context available | Immutable eligibility and resource plan | MVP |
| ECRF-UC-002 | Execute exact combine | Player/project | Exact inputs and output capacity valid | Atomic input/output commit | MVP |
| ECRF-UC-003 | Preview standard recipe | Player/project | Recipe and providers registered | Missing requirements, selected resources, outputs, and max batch reported | MVP |
| ECRF-UC-004 | Execute immediate recipe | Player/project | Fresh preview or revalidation succeeds | One idempotent atomic transaction | MVP |
| ECRF-UC-005 | Discover recipe | Project/bridge | Valid RecipeId and authorization | Knowledge state commits once | MVP |
| ECRF-UC-006 | Require station/tool/context | Designer/project | Providers and station registered | Eligibility reflects authored requirements | MVP |
| ECRF-UC-007 | Craft batch | Player/project | Recipe and provider limits allow | One scaled atomic transaction | MVP |
| ECRF-UC-008 | Timed station craft | Player/project | Timed module installed | Reserve, run, complete, or cancel under explicit policy | Later |
| ECRF-UC-009 | Queue production | Player/project | Queue module installed | Bounded queue with revalidation on promotion | Later |
| ECRF-UC-010 | Salvage item | Player/project | Provider can consume target and grant outputs | Generic transformation result | Later |
| ECRF-UC-011 | Repair/upgrade unique item | Player/project | Mutation-capable module/provider installed | Atomic item-state mutation and resource plan | Deferred module |
| ECRF-UC-012 | Server-authoritative craft | Network bridge | Authoritative peer and provider adapters present | Server validates and replicates result | Advanced bridge |

### 4.3 Explicitly unsupported use cases

- Treating a client-side preview as multiplayer authorization.
- Mutating two arbitrary resource authorities under a false universal atomic guarantee.
- Saving live provider objects, station GameObjects, reservation handles, or active tasks.
- Using display names, asset paths, or scene hierarchy as recipe identity.
- Letting recipe evaluation mutate skills, inventories, quests, or world state.
- Applying repair, upgrade, affix, or durability rules without a mutation-capable provider.
- Assuming offline queues progress safely from wall-clock time without a designed trust policy.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Recipe and catalog definition contracts.
- Recipe matching and canonical signatures.
- Crafting contexts, requests, previews, plans, and results.
- Requirement orchestration and provider availability.
- One-resource-provider reservation and immediate commit lifecycle.
- Stable request idempotency.
- Recipe knowledge and visibility state.
- Station registration and capability snapshots.
- Cancellation and commit-point semantics.
- Semantic events and diagnostics.
- Authoring, validation, setup, repair, Laboratory, and release gates.
- Extension contracts for later crafting modules.

### 5.2 The package does not own

- Item definitions, stacks, instances, containers, equipment, or capacity.
- Skills, professions, attributes, classes, or progression values.
- Production menus, drag/drop, prompts, or notifications.
- Sound, music, haptics, camera shake, flashes, or VFX.
- Objective or quest truth.
- Dialogue sequencing.
- Scene travel or world state.
- Save-file paths, slots, cloud transport, or backups.
- Network sessions, authority, prediction, or reconciliation.
- Project-specific quality, failure, repair, upgrade, or item-stat formulas.

### 5.3 Neighboring authorities

| Concern | Authority | EchoCrafting interaction |
|---|---|---|
| Item/container mutation | The Vault or project provider | Separate resource-provider bridge |
| Recipe discovery trigger | The Ascent, The Path, Voices, project | Submit `DiscoverRecipe` request |
| Crafting UI | The Looking Glass/project | Consume snapshots/results; submit semantic requests |
| Input binding | The Will/project | Translate actions to UI or request commands |
| Feedback | Impact | React to semantic result events |
| Audio | Resonance | React to semantic result events |
| Character/skill data | Fellowship, project RPG data, future Arcana/Clash | Read-only requirement providers |
| Save transport | The Chronicle | Export/import detached crafting state |
| Scene/station travel | The Passage/Atlas/project | Register current station/context; no scene load from core |
| Objectives | The Path | Observe success or require crafting through bridge |
| Multiplayer | The Convergence | Authoritative request/result bridge |
| Diagnostics | The Observatory | Optional status provider |
| Setup composition | The Workshop | Invoke Editor setup facade |

### 5.4 Boundary tests

A feature belongs in EchoCrafting only when it directly participates in recipe definition, matching, requirement evaluation, transformation planning, provider transaction lifecycle, crafting-specific knowledge, job orchestration, or diagnostics. Item ownership, broader progression, UI, presentation, save transport, networking, and genre-specific formulas remain elsewhere.

---

## 6. Independence Contract

### 6.1 Standalone guarantees

The package must:

- Compile with declared Unity dependencies only.
- Initialize without First Light.
- Work without The Vault through a simulated or project resource provider.
- Work without The Looking Glass through API and Laboratory controls.
- Work without The Chronicle; knowledge remains session state until a persistence adapter is supplied.
- Keep game-specific recipes and data outside immutable package source.
- Avoid assumptions about item classes, inventory APIs, scene names, input maps, tags, layers, or save paths.
- Expose injection seams for service, clock, provider, ID generation, and diagnostics tests.
- Fail safely when optional collaborators are absent.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Evidence |
|---|---|---|
| Installed alone | Core and Editor assemblies compile | Clean-project test; Not run |
| Enter Laboratory directly | Development initializer creates one root if absent | PlayMode/Lab; Not run |
| Vault bridge absent | Simulated/custom provider path remains complete | Lab; Not run |
| Chronicle absent | Runtime works; no durable knowledge transport selected | Lab; Not run |
| Optional requirement provider absent | Requirement returns Unavailable under authored policy | Unit/Lab; Not run |
| Duplicate root present | Duplicate exits before side effects | PlayMode/Lab; Not run |
| Required configuration missing | Root reports Blocked and performs no mutation | PlayMode/Lab; Not run |
| Samples deleted | Runtime/Editor package remains compilable | Clean-project; Not run |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Core modules | Platform | Yes | Supported baseline | MonoBehaviour, ScriptableObject, serialization, scene lifecycle | Package cannot function without Unity |
| Unity Test Framework | Test only | Yes for tests | Verified at implementation | Automated tests | Runtime unaffected if tests excluded |
| uGUI/TMP | Sample/presentation only | No | Verified at implementation | Optional combine-bag sample | Core remains compile-safe without sample |

### 6.4 Forbidden dependencies

- Direct runtime dependency on EchoInventory, EchoSave, EchoUI, EchoProgression, or any peer package.
- Runtime dependency on `UnityEditor`.
- Samples or tests referenced by runtime assemblies.
- Reflection-based discovery of arbitrary project methods as recipe actions.
- Hidden `Resources` paths or project singleton assumptions.
- Unlicensed content.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface |
|---|---|---|---|---:|---|
| ECRF-CAP-001 | Recipe catalogs | Stable-ID immutable recipe registry | Approved | Yes | Runtime/Editor |
| ECRF-CAP-002 | Simple Combine | Exact/canonical authored input-set transformation | Approved | Yes | Runtime/Sample |
| ECRF-CAP-003 | Standard recipes | Quantities, alternatives, tags, tools, contexts | Approved | Yes | Runtime |
| ECRF-CAP-004 | Side-effect-free preview | Eligibility, plan, capacity, revision, outputs | Approved | Yes | Runtime |
| ECRF-CAP-005 | Provider transaction | Prepare, reserve, atomic commit, release | Approved | Yes | Runtime |
| ECRF-CAP-006 | Request idempotency | Duplicate-safe execution results | Approved | Yes | Runtime |
| ECRF-CAP-007 | Station registration | Stable capabilities and stale-safe handles | Approved | Yes | Runtime |
| ECRF-CAP-008 | Requirement providers | Read-only project skill/context/tool checks | Approved | Yes | Runtime |
| ECRF-CAP-009 | Recipe knowledge | Discover, visibility, export/import | Approved | Yes | Runtime |
| ECRF-CAP-010 | Immediate batches | Bounded all-or-nothing batch plan | Approved | Yes | Runtime |
| ECRF-CAP-011 | Outputs/byproducts/options | Atomic grant plan | Approved | Yes | Runtime |
| ECRF-CAP-012 | Timed jobs | Active reservation and progress lifecycle | Approved later | No | Module |
| ECRF-CAP-013 | Station queues | Bounded queue and promotion revalidation | Approved later | No | Module |
| ECRF-CAP-014 | Quality | Provider-driven quality result | Approved later | No | Module/provider |
| ECRF-CAP-015 | Failure policy | Provider-driven pre-commit resolution | Approved later | No | Module/provider |
| ECRF-CAP-016 | Salvage | Generic transformation profile | Approved later | No | Runtime/profile |
| ECRF-CAP-017 | Repair/upgrade | Unique item mutation transaction | Deferred | No | Module/provider |
| ECRF-CAP-018 | Offline production | Durable elapsed-time jobs | Deferred research | No | Provider/module |
| ECRF-CAP-019 | Diagnostics | Structured status, errors, plans, provider health | Approved | Yes | Runtime/Editor |
| ECRF-CAP-020 | Laboratory | Standalone exact and standard crafting proof | Approved | Yes | Sample |

### 7.2 MVP capability set

The MVP includes catalogs, stable IDs, exact and standard immediate recipes, deterministic preview, one mutation provider, read-only requirement providers, station/context checks, tools/catalysts, batches, outputs/byproducts, knowledge, idempotency, cancellation before commit, setup, validation, diagnostics, and the Laboratory.

### 7.3 Later capability set

Timed jobs, station queues, quality, failure, salvage, unique-item repair/upgrade, offline production, multi-provider transaction coordination, and network-authoritative adapters remain later work with explicit seams.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Core Inventory dependency | Rejected | Breaks provider neutrality | Never without suite authority change |
| Arbitrary distributed atomic transaction | Rejected for MVP | Cannot guarantee unknown provider recovery | Compatible coordinator research |
| Mandatory professions | Rejected | Genre lock | Project provider supplies requirement |
| Random failure in core | Rejected | Not universally desired | Later provider module |
| Mandatory quality enum | Rejected | Item-model dependent | Later typed provider contract |
| Offline queue in first release | Deferred | Time and persistence trust unresolved | Dedicated research and prototype |
| Full repair/upgrade in core MVP | Deferred | Unique item mutation needs explicit provider | Vault/item-state bridge design |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Recipes, ingredient groups, outputs, station definitions, catalogs, policies | Live resources, active reservations, provider objects, scene references |
| Runtime state/behavior | Root, service, provider/station registries, knowledge, previews, reservations, idempotency | Editor APIs, production UI, item ownership |
| Presentation/feedback | Optional sample presenter, project UI snapshots, bridge reactions | Authoritative crafting/resource state |

### 8.2 Component topology

```text
CraftingRecipeCatalog
        |
        v
EchoCraftingRoot -> EchoCraftingService
        |              |
        |              +-> RecipeMatcher / PreviewBuilder
        |              +-> RequirementProviderRegistry
        |              +-> ResourceProviderRegistry
        |              +-> StationRegistry
        |              +-> RecipeKnowledgeState
        |              +-> IdempotencyLedger
        |
        +-> Diagnostics / semantic events

Project or sample UI
        -> CraftingRequest
            -> preview/revalidate
                -> one ICraftingResourceProvider reservation
                    -> atomic commit or release
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root? | Yes, one application-session root by default |
| Root type | `EchoCraftingRoot` |
| Duplicate behavior | Reject duplicate before providers, stations, events, or transactions |
| Initialization trigger | Awake claim; explicit initialization after validation |
| Shutdown | Stop accepting work, cancel reversible operations, release reservations, unregister providers/stations, clear session handles |
| Direct-scene behavior | Development initializer creates configured root only when absent |
| Test seam | `IEchoCraftingService`, provider interfaces, clock, ID generator, diagnostic sink |

### 8.4 Lifecycle sequence

1. Claim root authority.
2. Validate configuration and catalogs.
3. Build immutable recipe and alias registry.
4. Initialize knowledge and idempotency state.
5. Register built-in policies and explicit providers.
6. Enter Ready.
7. Accept previews, knowledge requests, station/provider registrations, and immediate crafting requests.
8. On scene changes, reconcile station registrations while root state persists if configured.
9. On shutdown, reject new work, close reversible reservations, publish final diagnostics, and clear runtime registrations.

### 8.5 Failure model

| Failure | Detection | User-visible result | Runtime fallback | Code family |
|---|---|---|---|---|
| Duplicate root | Awake claim | Duplicate rejected | Existing root remains | ECRF-ROOT-* |
| Invalid catalog | Initialization/validation | Blocked | No recipe registry | ECRF-CAT-* |
| Unknown recipe | Request | Unavailable | No mutation | ECRF-REC-* |
| Missing provider | Evaluation | Unavailable | No mutation | ECRF-PROV-* |
| Stale preview | Revalidation | Stale | Request new preview | ECRF-REV-* |
| Resource prepare failure | Provider | FailedBeforeCommit | Release partial reservation | ECRF-TXN-* |
| Cancellation after commit | Execution | TooLate | Preserve committed truth | ECRF-CAN-* |
| Provider defect after commit | Execution | CommittedWithDiagnostic | Never auto-retry | ECRF-TXN-* |
| Unknown knowledge record | Import | Advisory/orphaned | Preserve opaque record | ECRF-MIG-* |
| Deferred capability request | Request | Unavailable | Explain module requirement | ECRF-CAP-* |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoCraftingConfiguration` | Root policies, catalog references, bounds | Configuration identity | No | Yes |
| `CraftingRecipeCatalog` | Recipe registry and aliases | CatalogId | No | Yes |
| `CraftingRecipeDefinition` | Requirements, outputs, policies, metadata | RecipeId | No | Yes |
| `IngredientRequirementDefinition` | Exact/tag/alternative quantity rule | RequirementId within recipe | No | Yes |
| `CraftingOutputDefinition` | Output or byproduct request | OutputId within recipe | No | Yes |
| `CraftingStationDefinition` | Capability tags and later queue policy | StationDefinitionId | No | Yes |
| `CraftingRequirementDefinition` | Typed provider payload | RequirementId/provider type | No | Yes |
| `CraftingVisibilityPolicy` | Browsing/discovery behavior | Policy ID when extensible | No | Yes |

### 9.2 Runtime state

| State | Owner | Lifetime | Reset | Serialization |
|---|---|---|---|---|
| Recipe registry | Root/service | Session | Rebuild on initialize | Definitions only |
| Provider registrations | Service | Session/handle | Unregister/shutdown | Never |
| Station registrations | Service | Scene/session/handle | Unregister/scene/shutdown | Never |
| Recipe knowledge | Service | Session/durable snapshot | Explicit reset/import | Versioned detached state |
| Active preview | Caller/service record | Short-lived | Expire/revision change | Never authoritative save state |
| Resource reservation | Provider | Request | Commit/release/shutdown | Never in MVP |
| Idempotency ledger | Service | Bounded session/durable optional | Prune/reset/import | Versioned records, no live objects |
| Later jobs/queues | Timed module | Session or safe snapshot | Policy-specific | Deferred |

### 9.3 Stable identifiers

- `RecipeId`, `CatalogId`, `StationDefinitionId`, `StationInstanceId`, `CraftingRequestId`, `CraftingJobId`, `RequirementId`, `OutputId`, and provider IDs are domain IDs, not Unity AssetDatabase GUIDs.
- Display names and localization keys may change without changing IDs.
- Duplicate and empty IDs block release validation.
- Aliases resolve legacy IDs during migration; aliases cannot form chains or cycles at runtime.
- Runtime station and provider handles carry generations and root identity.

### 9.4 ScriptableObject safety

Definitions never store current ingredient counts, known state, selected alternatives, provider revisions, queue progress, active jobs, reservations, result history, or scene objects. Runtime mutation of shared recipe assets is prohibited.

### 9.5 Serialization and migration

`CraftingStateDocument` contains document version, package version, configuration identity, known recipe records, aliases applied, orphan records, and optional bounded idempotency records. Migrations are contiguous, staged, validated, and atomic. Unknown future records are preserved when the selected serializer supports opaque retention.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Ownership |
|---|---|---|---|
| `IEchoCraftingService` | Interface | Main preview, execute, knowledge, station/provider, state API | Root/injected implementation |
| `EchoCraftingRoot` | MonoBehaviour | Duplicate-safe lifecycle owner | Scene/prefab |
| `CraftingRecipeDefinition` | ScriptableObject | Immutable authored recipe | Project |
| `CraftingRecipeCatalog` | ScriptableObject | Recipe and alias registry | Project |
| `CraftingRequest` | Struct/class | Recipe, context, batch, expected revisions, request ID | Caller |
| `CraftingContext` | Struct/class | Actor, station, provider, source/destination, typed metadata | Caller |
| `CraftingPreview` | Immutable result | Eligibility, plan, missing requirements, outputs, fingerprint | Service |
| `CraftingResult` | Immutable result | Final status, commit truth, outputs, diagnostics | Service |
| `CraftingResourcePlan` | Immutable DTO | Provider-facing input/output operations | Service/provider |
| `ICraftingResourceProvider` | Interface | Prepare one atomic resource transaction | Project/bridge |
| `ICraftingReservation` | Interface/handle | Reversible reservation and commit boundary | Provider |
| `ICraftingRequirementProvider` | Interface | Read-only typed condition evaluation | Project/bridge |
| `CraftingStationHandle` | Struct | Stale-safe station registration | Service |
| `CraftingProviderHandle` | Struct | Stale-safe provider registration | Service |
| `RecipeKnowledgeSnapshot` | Immutable DTO | Known/hidden/discovered state | Service |
| `CraftingStateDocument` | Serializable DTO | Detached durable state | Service/Chronicle bridge |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure | Main-loop rule |
|---|---|---|---|---|
| `CraftingPreview Evaluate(in CraftingRequest request)` | Side-effect-free synchronous preview when providers support sync reads | Ready; valid request | Structured eligibility/staleness | Main thread by default |
| `Awaitable<CraftingPreview> EvaluateAsync(...)` | Async provider preview | Ready | Cancellation before result publication | Unity Awaitable policy |
| `Awaitable<CraftingResult> ExecuteImmediateAsync(...)` | Revalidate, prepare, reserve, and commit one provider plan | Eligible; capability present | Atomic result or safe failure | Main-thread provider calls unless provider declares detached work |
| `CraftingKnowledgeResult DiscoverRecipe(...)` | Commit recipe knowledge | Valid RecipeId/request | Idempotent result | Main thread |
| `CraftingStationHandle RegisterStation(...)` | Register runtime station | Valid definition/snapshot provider | Stale-safe handle | Main thread |
| `CraftingProviderHandle RegisterResourceProvider(...)` | Register one mutation provider | Unique provider ID/capabilities | Stale-safe handle | Main thread |
| `CraftingProviderHandle RegisterRequirementProvider(...)` | Register read-only provider | Unique provider/type | Stale-safe handle | Main thread |
| `CraftingStateDocument ExportState()` | Detached package state | Safe point | Structured snapshot or unsafe-point result | Main thread capture; detached serialize allowed |
| `PreparedCraftingImport PrepareImport(...)` | Validate/migrate without mutation | Valid document | Prepared result | Detached work allowed after DTO capture |
| `CraftingImportResult CommitImport(...)` | Atomically publish prepared state | Matching root/config revision | Commit or reject | Main thread |

### 10.3 Events and callbacks

| Event | Timing | Payload | Rule |
|---|---|---|---|
| `ReadyChanged` | After state commits | Initialization snapshot | Presentation not required |
| `RecipeKnowledgeChanged` | After knowledge commit | RecipeId and old/new state | One event per committed batch |
| `CraftingStarted` | After reservation accepted | Request/session metadata | No resources necessarily committed |
| `CraftingCompleted` | After provider commit | Authoritative result | Never raised twice for same request ID |
| `CraftingFailed` | After terminal failure | Structured result | Includes before/after commit truth |
| `StationRegistryChanged` | After registration state commit | Snapshot/revision | Bounded/coalescible |
| `ProviderRegistryChanged` | After registration state commit | Snapshot/revision | Bounded/coalescible |

### 10.4 Async and cancellation policy

- Every public async operation uses a fresh awaitable.
- Cancellation is cooperative before the provider commit point.
- A reservation must expose whether release remains reversible.
- After commit publication, cancellation returns `TooLate`.
- Scene destruction or root shutdown cancels reversible work and never auto-retries committed work.
- Timeouts are explicit and provider-specific; timeout never implies rollback after commit.

### 10.5 API ergonomics

The novice path uses one root/configuration, one catalog, one simulated or Vault resource provider, and the Laboratory. The advanced path uses explicit providers, station handles, custom typed requirements, detached state, and later modules without editing core code.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install package.
2. Open **Tools > EchoDevGames > The Crucible > Setup**.
3. Choose standalone sample, empty project configuration, or connect-existing-data guidance.
4. Preview created assets and folders.
5. Create configuration, root prefab, empty catalog, and optional Laboratory sample references.
6. Open Recipe Authoring or import sample.
7. Run validation.
8. Open Laboratory and execute the exact combine workflow.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeat-safe? | Backup/undo | Report |
|---|---|---|---:|---|---|
| Create configuration | Project asset | Nothing existing by default | Yes | Undo/create-only | Setup receipt |
| Create root prefab | Project prefab | Nothing existing by default | Yes | Undo/create-only | Receipt |
| Create recipe catalog | Project asset | Optional explicit catalog assignment | Yes | Undo | Receipt |
| Add recipe template | Project recipe asset | Catalog only after confirmation | Yes | Undo | Receipt |
| Repair missing assignment | Nothing new unless approved | Selected configuration/prefab | Yes | Undo/preview | Repair receipt |
| Generate Laboratory data | Sample/project fixtures | Sample folder only | Yes | Delete/regenerate | Sample report |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| Crucible Setup | Installer | Create/repair configuration and root | No |
| Recipe Authoring Window | Designer | Author requirements, outputs, policies, IDs | No |
| Recipe Signature Inspector | Designer/tester | Show canonical exact-match signature and ambiguity | No |
| Catalog Validator | Maintainer | IDs, aliases, references, providers, policies | No |
| Transaction Simulator | Designer/tester | Preview with simulated resources without Play Mode mutation | No |
| Knowledge Inspector | Designer/tester | Inspect visibility/discovery fixtures | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix? | Safe auto-fix? |
|---|---|---|---:|---:|
| ECRF-VAL-001 | Missing configuration | Blocker | Yes | No |
| ECRF-VAL-002 | Empty/duplicate RecipeId | Blocker | Yes | Only before external use, explicit |
| ECRF-VAL-003 | Alias collision/cycle | Blocker | Manual | No |
| ECRF-VAL-004 | Ambiguous exact recipe signature | Blocker | Manual | No |
| ECRF-VAL-005 | Missing output | Error | Manual | No |
| ECRF-VAL-006 | Invalid quantity/batch bound | Error | Yes | Explicit clamp only |
| ECRF-VAL-007 | Unknown required provider type | Error/Warning by policy | No | No |
| ECRF-VAL-008 | Unsupported timed/quality/failure capability in MVP | Warning/Error | Guidance | No |
| ECRF-VAL-009 | Mutable runtime reference on definition | Blocker | Manual | No |
| ECRF-VAL-010 | Sample or Editor assembly leaked into runtime | Blocker | Manual | No |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Planned routes are embedded/local package during development, Git URL, tarball, and Workshop selection. Registry distribution remains a later release choice. Each claimed route requires separate SFGSS-004 evidence.

### 12.2 Minimal scene setup

- One `EchoCraftingRoot` referencing `EchoCraftingConfiguration`.
- One recipe catalog.
- At least one explicit resource provider registration.
- Optional station registrations.
- Project or sample requester.

### 12.3 Boot-scene setup

First Light may initialize or adopt the root through an explicit startup step/bridge. EchoCrafting remains independently initialized without First Light.

### 12.4 Direct-scene setup

`EchoCraftingDirectSceneInitializer` may create the configured development root only when none exists. It identifies development initialization and is excluded or disabled in release builds by default.

### 12.5 Scene isolation rule

The Laboratory may use simulated providers and redistributable sample data only. It cannot require The Vault, The Looking Glass, Resonance, Impact, or project code.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Laboratory purpose

The **Crucible Recipe Transformation Laboratory** proves exact combine, standard recipe preview, atomic immediate execution, cancellation, stale revisions, recipe knowledge, station/context requirements, provider failures, state export/import, and reset without another Echo package.

### 13.2 Required Laboratory contents

- Duplicate-safe root and configuration.
- Sample catalog with exact combine, alternative, tag, tool, station, batch, and byproduct recipes.
- Simulated resource provider with visible counts, capacity, revisions, reservations, and injected failures.
- Simulated read-only requirement providers.
- Sample station registration controls.
- Recipe browser and preview readout using removable sample presentation.
- Combine-bag scenario matching the architectural quest use case without copyrighted/project content.
- Knowledge discover/reset controls.
- Cancellation and failure injection controls.
- State export/import fixture controls.
- Reset and duplicate-root controls.

### 13.3 Laboratory acceptance checklist

| Test ID | Scenario | Action | Expected | Type | Status |
|---|---|---|---|---|---|
| ECRF-LAB-001 | Initialize Crucible Laboratory | Open the standalone Crucible Laboratory with the sample catalog and simulated resource provider. | The root, catalog, provider registry, station registry, diagnostics, and readout become Ready without another Echo package. | Manual/automatable | Not run |
| ECRF-LAB-002 | Missing configuration | Remove the EchoCraftingConfiguration reference. | Initialization is blocked with an actionable ECRF diagnostic and no provider or recipe side effects. | Manual/automatable | Not run |
| ECRF-LAB-003 | Duplicate root | Introduce a second EchoCraftingRoot before Play Mode. | The duplicate rejects itself before provider registration, reservations, jobs, or events. | Manual/automatable | Not run |
| ECRF-LAB-004 | Duplicate recipe ID | Add two recipes with the same stable RecipeId. | Validation blocks the conflicting catalog and reports both assets. | Manual/automatable | Not run |
| ECRF-LAB-005 | Missing recipe ID | Clear one authored RecipeId. | Validation blocks release use and offers a safe Editor repair operation. | Manual/automatable | Not run |
| ECRF-LAB-006 | Unknown recipe request | Request a recipe ID that is not registered. | The service returns Unavailable without mutating resources. | Manual/automatable | Not run |
| ECRF-LAB-007 | Exact combine success | Place the exact authored quest ingredients in the simulated combine bag and execute Combine. | One immediate transaction consumes the authored inputs and grants the authored output. | Manual/automatable | Not run |
| ECRF-LAB-008 | Exact combine missing ingredient | Remove one required ingredient from the bag. | Preview reports the missing requirement and execution does not reserve or consume anything. | Manual/automatable | Not run |
| ECRF-LAB-009 | Exact combine extra ingredient rejected | Add an unapproved extra item to an exact-set recipe. | The exact-set matcher rejects the combination without guessing another recipe. | Manual/automatable | Not run |
| ECRF-LAB-010 | Exact combine extra ingredient allowed | Use a recipe configured to ignore unrelated contents. | Only declared inputs participate and the remaining contents are unchanged. | Manual/automatable | Not run |
| ECRF-LAB-011 | Exact recipe ambiguity | Author two exact recipes that match the same canonical input signature. | Validation blocks the ambiguity before runtime. | Manual/automatable | Not run |
| ECRF-LAB-012 | Tag ingredient match | Craft a recipe that accepts any item carrying the approved material tag. | The provider resolves one deterministic eligible resource set. | Manual/automatable | Not run |
| ECRF-LAB-013 | Alternative ingredient group | Satisfy one-of several authored alternatives. | Exactly one alternative is selected according to deterministic provider ordering. | Manual/automatable | Not run |
| ECRF-LAB-014 | Multiple ingredient groups | Satisfy exact, tag, and alternative requirements in one recipe. | Preview produces one complete deterministic resource plan. | Manual/automatable | Not run |
| ECRF-LAB-015 | Insufficient quantity | Provide fewer units than a requirement demands. | Preview reports required, available, and missing quantities without mutation. | Manual/automatable | Not run |
| ECRF-LAB-016 | Non-consumable tool requirement | Provide a required tool marked non-consumable. | The tool is validated but not consumed by the transaction. | Manual/automatable | Not run |
| ECRF-LAB-017 | Catalyst requirement | Provide a catalyst with a configured consume policy. | The preview and transaction apply the authored catalyst rule exactly once. | Manual/automatable | Not run |
| ECRF-LAB-018 | Station tag requirement | Attempt a forge-only recipe at a station without the forge capability tag. | The request is denied before resource preparation. | Manual/automatable | Not run |
| ECRF-LAB-019 | Station requirement success | Use the same recipe at a registered forge-capable station. | The station requirement passes and crafting can proceed. | Manual/automatable | Not run |
| ECRF-LAB-020 | No-station recipe | Execute an immediate recipe that explicitly allows portable crafting. | The recipe succeeds without inventing a station object. | Manual/automatable | Not run |
| ECRF-LAB-021 | Unknown station | Submit an unregistered StationId. | The request returns Unavailable and no resources are touched. | Manual/automatable | Not run |
| ECRF-LAB-022 | Stale station handle | Unregister and re-register a station, then use the old handle generation. | The stale station handle is rejected safely. | Manual/automatable | Not run |
| ECRF-LAB-023 | Context requirement success | Supply a project context value accepted by a registered requirement provider. | The provider returns Satisfied and the recipe continues. | Manual/automatable | Not run |
| ECRF-LAB-024 | Context requirement unavailable | Remove the required provider. | The recipe reports Unavailable rather than treating the requirement as false or satisfied. | Manual/automatable | Not run |
| ECRF-LAB-025 | Skill requirement provider | Evaluate a recipe requiring a project-owned profession level. | The provider determines eligibility without EchoCrafting owning skill state. | Manual/automatable | Not run |
| ECRF-LAB-026 | Read-only requirement enforcement | Use a provider that attempts mutation during evaluation. | The provider contract is rejected or diagnosed; evaluation remains side-effect-free. | Manual/automatable | Not run |
| ECRF-LAB-027 | Known recipe visible | Mark a recipe known in the package knowledge registry. | Recipe browsing and direct evaluation report Known. | Manual/automatable | Not run |
| ECRF-LAB-028 | Unknown recipe hidden | Use a hidden-until-known recipe without discovery. | Normal browsing omits it while exact-match probing follows the configured secrecy policy. | Manual/automatable | Not run |
| ECRF-LAB-029 | Discover recipe | Submit a valid DiscoverRecipe request. | Knowledge commits once and emits one semantic event. | Manual/automatable | Not run |
| ECRF-LAB-030 | Duplicate discovery request | Repeat the same discovery request ID. | The operation is idempotent and does not duplicate events or durable records. | Manual/automatable | Not run |
| ECRF-LAB-031 | Revoke recipe knowledge | Use an explicitly authorized development reset operation. | Knowledge is removed only through the documented reset path. | Manual/automatable | Not run |
| ECRF-LAB-032 | Preview without mutation | Run the same preview repeatedly. | Resource counts, item instances, knowledge, queues, and stations remain unchanged. | Manual/automatable | Not run |
| ECRF-LAB-033 | Preview fingerprint stability | Repeat a preview against unchanged provider revisions. | The canonical plan fingerprint remains stable. | Manual/automatable | Not run |
| ECRF-LAB-034 | Stale preview revision | Change provider contents after preview and execute with the old expected revision. | Execution is rejected and requires re-preview or explicit re-evaluation. | Manual/automatable | Not run |
| ECRF-LAB-035 | Immediate craft success | Execute an eligible immediate recipe. | One provider reservation commits inputs and outputs atomically. | Manual/automatable | Not run |
| ECRF-LAB-036 | Immediate provider prepare failure | Force the resource provider to reject preparation. | The request fails before the commit point and resources remain unchanged. | Manual/automatable | Not run |
| ECRF-LAB-037 | Immediate provider commit failure | Force a provider failure before atomic commit publication. | The reservation rolls back and the result reports no committed mutation. | Manual/automatable | Not run |
| ECRF-LAB-038 | Provider failure after commit | Simulate a provider that reports an error after its declared commit point. | The result is CommittedWithDiagnostic and does not retry or duplicate outputs. | Manual/automatable | Not run |
| ECRF-LAB-039 | Cancel before prepare | Cancel while the request is queued for evaluation. | The operation ends Cancelled with no provider reservation. | Manual/automatable | Not run |
| ECRF-LAB-040 | Cancel after reservation before commit | Cancel an immediate operation while a reversible reservation is held. | The reservation is released and no resources are consumed or granted. | Manual/automatable | Not run |
| ECRF-LAB-041 | Cancel after commit point | Request cancellation after the provider publishes the atomic transaction. | The result is TooLate and the committed crafting result remains authoritative. | Manual/automatable | Not run |
| ECRF-LAB-042 | Request idempotency | Repeat a completed CraftingRequestId. | The original result is returned without executing another resource mutation. | Manual/automatable | Not run |
| ECRF-LAB-043 | Batch craft success | Craft a supported immediate batch within recipe and provider limits. | Quantities scale deterministically and commit as one atomic resource plan. | Manual/automatable | Not run |
| ECRF-LAB-044 | Batch exceeds recipe limit | Request more batches than the recipe allows. | Validation rejects the request before provider preparation. | Manual/automatable | Not run |
| ECRF-LAB-045 | Batch exceeds available resources | Request a valid batch count with insufficient inputs. | Preview reports the maximum craftable count and execution does not partially craft. | Manual/automatable | Not run |
| ECRF-LAB-046 | Output destination full | Use a provider destination with no capacity. | Preparation fails before inputs are consumed. | Manual/automatable | Not run |
| ECRF-LAB-047 | Byproduct grant | Craft a recipe with one byproduct. | Primary outputs and byproducts are included in the same provider transaction. | Manual/automatable | Not run |
| ECRF-LAB-048 | Output choice policy | Use a recipe with one project-selected output option. | The selected authored option is validated and granted exactly once. | Manual/automatable | Not run |
| ECRF-LAB-049 | Invalid output choice | Submit an output option not authored by the recipe. | The request is rejected before mutation. | Manual/automatable | Not run |
| ECRF-LAB-050 | Single resource provider rule | Attempt an MVP craft that would mutate two unrelated resource providers. | The core rejects unsupported distributed mutation and explains the single-provider MVP boundary. | Manual/automatable | Not run |
| ECRF-LAB-051 | Cross-provider read-only requirement | Use several read-only requirement providers with one resource provider. | All requirements may participate without weakening resource atomicity. | Manual/automatable | Not run |
| ECRF-LAB-052 | Provider unregister during evaluation | Remove a requirement provider while a request is evaluating. | The request uses its captured provider generation or fails Unavailable without undefined behavior. | Manual/automatable | Not run |
| ECRF-LAB-053 | Resource provider unregister with reservation | Attempt to unregister a resource provider that owns an active reservation. | Unregistration is blocked or deferred until the reservation closes. | Manual/automatable | Not run |
| ECRF-LAB-054 | Timed recipe preview | Preview a recipe configured for a later timed-crafting module. | The core reports the timing policy but execution returns DeferredCapability in the MVP. | Manual/automatable | Not run |
| ECRF-LAB-055 | Queue request in MVP | Submit a queued-production request before the timed module is installed. | The request returns Unavailable with an explicit capability diagnostic. | Manual/automatable | Not run |
| ECRF-LAB-056 | Simulated timed reservation lifecycle | Use the Laboratory-only simulated timed coordinator. | Reservation, running, completion, and release states follow the documented later-module contract without claiming production evidence. | Manual/automatable | Not run |
| ECRF-LAB-057 | Timed cancellation before commit | Cancel the simulated active job before completion. | The reservation releases according to policy and no output commits. | Manual/automatable | Not run |
| ECRF-LAB-058 | Queue does not reserve early | Place two simulated jobs in a station queue. | Only the active job holds a reservation; queued jobs revalidate when promoted. | Manual/automatable | Not run |
| ECRF-LAB-059 | Station queue capacity | Fill the simulated queue to its configured bound. | Further enqueue requests are rejected predictably. | Manual/automatable | Not run |
| ECRF-LAB-060 | Quality provider absent | Evaluate a recipe that requests an optional quality provider. | The policy follows its authored unavailable behavior and does not fabricate quality. | Manual/automatable | Not run |
| ECRF-LAB-061 | Quality provider deterministic result | Use a fake quality provider with fixed inputs. | The same canonical context produces the same quality result. | Manual/automatable | Not run |
| ECRF-LAB-062 | Failure provider absent | Evaluate a recipe requiring an optional failure provider. | The policy follows its authored unavailable behavior; the core does not roll random success itself. | Manual/automatable | Not run |
| ECRF-LAB-063 | Failure before resource commit | Return a deterministic failure result before provider preparation. | The craft fails without consuming or granting resources. | Manual/automatable | Not run |
| ECRF-LAB-064 | Salvage transformation preview | Preview a later salvage recipe through the generic transformation model. | Inputs and outputs can be represented without EchoCrafting owning item data. | Manual/automatable | Not run |
| ECRF-LAB-065 | Repair mutation capability unavailable | Request a unique-item repair without a mutation-capable provider. | The request returns Unavailable and preserves the item state. | Manual/automatable | Not run |
| ECRF-LAB-066 | Upgrade mutation capability unavailable | Request an upgrade without the later mutation module. | The request is rejected without consuming ingredients. | Manual/automatable | Not run |
| ECRF-LAB-067 | Export knowledge state | Export discovered recipe knowledge. | A detached versioned snapshot contains stable IDs and no live Unity objects. | Manual/automatable | Not run |
| ECRF-LAB-068 | Import known recipe state | Prepare and commit a valid knowledge snapshot. | Known recipes restore atomically and one post-commit event is raised. | Manual/automatable | Not run |
| ECRF-LAB-069 | Import unknown recipe ID | Import knowledge for a recipe not currently installed. | The unknown record is preserved as orphaned data rather than deleted. | Manual/automatable | Not run |
| ECRF-LAB-070 | Alias migration | Import state using an approved legacy RecipeId alias. | Migration resolves the canonical ID and records the migration result. | Manual/automatable | Not run |
| ECRF-LAB-071 | Active reservation save boundary | Attempt state export while a provider reservation is active. | The snapshot reports an unsafe point or excludes the live reservation according to policy. | Manual/automatable | Not run |
| ECRF-LAB-072 | Direct-scene initialization | Enter the Laboratory scene directly without First Light. | The configured development initializer creates one root only when absent. | Manual/automatable | Not run |
| ECRF-LAB-073 | Scene transition with root persistence | Load another scene while the root is configured to persist. | Catalog, providers, knowledge, and bounded idempotency history survive without duplicate roots. | Manual/automatable | Not run |
| ECRF-LAB-074 | Reset and reload Laboratory | Run, reset, and reload the Laboratory repeatedly. | Reservations, providers, station handles, knowledge fixtures, subscriptions, and diagnostics return to the known baseline. | Manual/automatable | Not run |

### 13.4 Optional Integration Laboratories

| Sample | Packages | Purpose | Why not standalone proof |
|---|---|---|---|
| Crucible + Vault | EchoCrafting, EchoInventory, bridge | Item-backed ingredient/output transaction | Depends on both authorities |
| Crucible + Looking Glass | EchoCrafting, EchoUI, bridge/project presenter | Production-style recipe browser/combine bag | UI is optional |
| Crucible + Path | EchoCrafting, EchoObjectives, bridge | Craft completion objective progress | Objective authority external |
| Crucible + Chronicle | EchoCrafting, EchoSave, bridge | Persist recipe knowledge | Save transport external |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The runtime core is nonvisual. It exposes recipe list snapshots, preview lines, availability, selected inputs, outputs, max batch, knowledge state, results, and later job/queue snapshots. Production screens belong to The Looking Glass or project UI.

### 14.2 Required states

- Ready.
- No recipes.
- Unknown/hidden recipe.
- Eligible.
- Missing ingredients.
- Missing tool/station/provider.
- Output blocked/full.
- Stale preview.
- Preparing/reserved/committing.
- Completed.
- Failed before commit.
- Committed with diagnostic.
- Cancelled.
- Too late to cancel.
- Deferred capability unavailable.

### 14.3 Accessibility requirements

- Semantic status must not rely on color or sound alone.
- Requirement lines expose text and structured reason IDs.
- Timed/queued later presenters must support reduced motion and configurable transient timing.
- Input modality and focus remain outside core but sample UI must be keyboard/controller navigable when those controls are included.
- Progress displays expose numeric/text alternatives.

### 14.4 Visual customization

Project visuals, icons, fonts, animations, layout, recipe cards, station art, and combine bag are replaceable without editing runtime code.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Availability | Cost |
|---|---|---|---|
| Root/config/catalog status | API/Inspector | Editor/Development/Release-safe summary | Low |
| Provider/station registry | API/Inspector | Development; redacted release summary | Low |
| Preview/result reason codes | API | All builds | Low |
| Reservation/request state | API/bounded history | Development | Bounded |
| Idempotency hits | Counter/history | Development | Bounded |
| Validation report | Editor window | Editor | On demand |

### 15.2 Structured status

Status includes root identity, initialization state, package/config version, catalog revision, recipe count, known recipe count, provider and station health, active reversible operations, bounded idempotency count, last result, warnings, and capabilities installed.

### 15.3 Diagnostic codes

| Prefix | Meaning |
|---|---|
| ECRF-ROOT | Root/lifecycle/duplicate |
| ECRF-CFG | Configuration |
| ECRF-CAT | Catalog/ID/alias |
| ECRF-REC | Recipe/matching |
| ECRF-REQ | Requirement/provider evaluation |
| ECRF-PROV | Provider registry/capabilities |
| ECRF-STN | Station registry/capabilities |
| ECRF-REV | Revision/stale preview |
| ECRF-TXN | Reservation/commit/rollback |
| ECRF-CAN | Cancellation/too-late |
| ECRF-KNW | Knowledge/visibility |
| ECRF-MIG | State import/migration |
| ECRF-CAP | Deferred/unsupported capability |
| ECRF-PERF | Capacity/performance |

### 15.4 Observatory bridge

A separate bridge may publish package inventory, root health, catalog/knowledge counts, provider/station health, active operations, last results, errors, and bounded timings. EchoCrafting never requires The Observatory.

### 15.5 Logging policy

Logs are categorized, rate-limited, and actionable. They do not expose resolved production text, private player data, complete inventory contents, secret seeds, or raw opaque provider payloads. Per-frame queue/progress spam is prohibited.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Recipes/catalogs | Project definition | Project | Assets | Unity serialization |
| Recipe knowledge | Profile/slot/project choice | EchoCrafting | Optional | Detached state via Chronicle/project |
| Idempotency records | Session; optional durable boundary | EchoCrafting | Bounded optional | Detached state |
| Providers/stations | Session/scene | Project/service | No | N/A |
| Active reservations | Request/session | Provider | No MVP | N/A |
| Immediate results | Bounded history | Service | Optional support evidence only | Diagnostics |
| Timed jobs/queues | Later module | EchoCrafting/module | Only at declared safe points | Deferred |

### 16.2 Standalone behavior

Without The Chronicle, EchoCrafting works normally. Knowledge and bounded idempotency state last for the current application session unless a project imports/exports them.

### 16.3 Optional participant/provider contract

A Chronicle bridge contributes a versioned `CraftingStateDocument`. It knows no save slots internally. Unknown recipe IDs and extension records remain preserved. Active live reservations are never serialized.

### 16.4 Failure and recovery

Missing state uses configured defaults. Corrupt or invalid documents fail preparation without mutating current state. Older supported versions migrate through contiguous steps. Newer incompatible data is preserved and reported. Import commits atomically.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Optional connections are explicit, removable, versioned, and separately tested. Installing a peer package does not silently change recipe behavior.

### 17.2 Planned integrations

| Authority | Connection | Bridge owner | Direction | Data | Required? |
|---|---|---|---|---|---:|
| The Vault | Resource provider | Separate bridge | Crafting requests provider plan | Inputs, outputs, capacity, revisions, transaction result | No |
| The Ascent | Knowledge grant/requirement | Separate bridge/project | Bidirectional requests/read-only | Recipe discovery, unlock/skill conditions | No |
| The Path | Progress/reward executor | Separate bridge | Craft result to objective; reward to discover/craft request | Stable IDs/results | No |
| Voices | Command/condition | Separate bridge | Dialogue requests craft/knowledge or reads state | Stable IDs/results | No |
| The Hand | Interaction executor | Project/bridge | Interaction requests station/combine action | Context/request/result | No |
| The Looking Glass | Presenter | Separate bridge/project | UI consumes snapshots and submits requests | Preview/result/job data | No |
| Resonance | Audio reaction | Bridge | Craft events to cues | Semantic event IDs | No |
| Impact | Feedback reaction | Bridge | Craft events to recipes | Semantic event IDs | No |
| The Chronicle | Save participant | Separate bridge | Export/import state | Versioned document | No |
| The Observatory | Diagnostics provider | Bridge | Status outward | Health/counters | No |
| The Convergence | Authority adapter | Future separate bridge | Requests to authority; results back | Request/context/result IDs | No |
| The Workshop | Editor setup facade | Package-owned Editor contract | Workshop invokes setup | Plans/receipts | No runtime dependency |

### 17.3 Bridge placement

Vault, Chronicle, Looking Glass, and Convergence integrations are separate packages because they depend on two optional authorities. Tiny presentation reactions may live in the owner package only if compile-safe and dependency-free under SFGSS-002; otherwise they are separate bridges.

### 17.4 Integration failure behavior

Missing peers leave core behavior unchanged. Version mismatch disables the bridge and reports actionable diagnostics. Provider teardown closes or blocks active reservations before unregistering. Bridge removal never deletes project recipes or crafting state automatically.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Planned target | Measurement | Release threshold |
|---|---|---|---|
| Preview allocation | Bounded and documented; no unbounded per-frame use | Profiler in Laboratory | Not yet measured |
| Recipe lookup | O(1) by RecipeId; indexed candidate matching | EditMode/Profiler | Not yet measured |
| Exact signature lookup | Indexed canonical signature | Lab/Profiler | Not yet measured |
| Active immediate operations | Configured bound | Stress Lab | Not yet measured |
| Diagnostic history | Fixed capacity | Unit/Profiler | No growth beyond bound |

### 18.2 Allocation policy

No LINQ or reflection is assumed in hot execution paths. Immutable snapshots may allocate at request boundaries but not per frame. Collections are bounded. Provider payloads are typed and cached only within declared lifetimes.

### 18.3 Scene and domain reload behavior

Static access resets under supported Enter Play Mode options. Registrations unsubscribe and handles invalidate. Root shutdown releases reversible reservations. Direct-scene helpers do not create a second authority.

### 18.4 Scalability limits

Recipe counts, requirements per recipe, outputs per recipe, aliases, providers, stations, active operations, batch size, known records, and histories have configured/tested bounds. Graceful rejection replaces unbounded growth.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Core data is usually game content, but contexts and provider payloads may contain profile or account-adjacent values. Diagnostics redact opaque payloads and do not export complete inventories, private player names, credentials, or raw network tokens.

### 19.2 Trust boundaries

- Definitions and imported state are validated.
- Provider results are treated as authoritative only for the provider's declared domain.
- Client requests are untrusted in multiplayer.
- Request IDs prevent accidental duplicate mutation but are not security credentials.
- Quality/failure seeds require authoritative generation in networked contexts.
- Editor imports cannot execute arbitrary code from recipe data.

### 19.3 Platform behavior

| Platform | Planned support | Special behavior | Required evidence |
|---|---:|---|---|
| Windows/macOS/Linux | Planned | Standard Unity runtime | Clean build/Lab |
| WebGL | Planned/conditional | Threading/file limitations; immediate core only | Platform test |
| Mobile | Planned | Lifecycle, memory, touch presentation outside core | Platform test |
| Console | Unknown/planned | Certification/storage/network policies | Provider/platform test |

No platform is marked Supported before execution evidence exists.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-crafting/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
├── Runtime/
│   ├── Core/
│   ├── Definitions/
│   ├── Matching/
│   ├── Requirements/
│   ├── Providers/
│   ├── Transactions/
│   ├── Knowledge/
│   ├── Stations/
│   ├── Persistence/
│   └── EchoDevGames.EchoCrafting.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Authoring/
│   ├── Validation/
│   ├── Simulation/
│   └── EchoDevGames.EchoCrafting.Editor.asmdef
├── Samples~/
│   └── Crucible Recipe Transformation Laboratory/
└── Tests/
    ├── Editor/
    └── Runtime/
```

### 20.2 Proposed source tree

```text
Runtime/Core: root, service, requests, previews, results, events
Runtime/Definitions: recipes, catalogs, ingredients, outputs, station definitions
Runtime/Matching: canonical signatures, candidate indexes, selection policies
Runtime/Requirements: requirement DTOs, provider contracts, evaluation results
Runtime/Providers: registrations, capabilities, handles
Runtime/Transactions: resource plans, reservations, idempotency, cancellation
Runtime/Knowledge: visibility and discovered recipe state
Runtime/Stations: registration and capability snapshots
Runtime/Persistence: detached state DTOs and migration seams
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoCrafting.Runtime` | Runtime | Unity modules only | Yes | Neutral core |
| `EchoDevGames.EchoCrafting.Editor` | Editor | Runtime, UnityEditor | No | Setup/authoring/validation |
| `EchoDevGames.EchoCrafting.Tests.Editor` | Editor tests | Runtime, Editor, Test Framework | No | Pure/Editor tests |
| `EchoDevGames.EchoCrafting.Tests.Runtime` | PlayMode tests | Runtime, Test Framework | No | Lifecycle/transaction tests |

Optional UGUI sample, Vault bridge, Chronicle bridge, and timed/quality/failure modules have separate assemblies/packages when introduced.

### 20.4 Repository files

README, documentation index, user/developer guides, Current Notes link, architecture, workshop record, ADRs, tests, release checklist, changelog, license, notices, stable `.meta` files, and compatibility records are required.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | Release decision pending | 6000.3.8f1 planned baseline | Not run |
| Test Framework | Implementation selection | Not run | Tests only |
| Optional peers | Bridge specifications | Not run | Never core dependencies |

### 21.2 Semantic versioning policy

Patch: diagnostics, docs, bug fixes without behavior/API/schema break.  
Minor: backward-compatible recipes, providers, policies, or modules.  
Major: public API, stable ID semantics, recipe schema, transaction contract, durable state, or setup-output break requiring migration.

### 21.3 Deprecation policy

Deprecated members remain documented with migration guidance for at least one appropriate minor cycle unless security/data corruption requires faster removal. Stable aliases and migrations precede ID or schema removal.

### 21.4 GUID and asset compatibility

Public scripts, templates, sample assets, prefabs, and configuration assets preserve committed `.meta` GUIDs. Unity GUIDs are asset identity only, never the recipe/domain identity exposed to saves or providers.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview and boundaries.
- Installation and five-minute quick start.
- Simple Combine guide.
- Standard recipe authoring guide.
- Provider implementation guide.
- Station/context and requirement guide.
- Recipe knowledge guide.
- Laboratory guide.
- Diagnostics and troubleshooting.
- Optional bridge index.
- Known limitations and deferred capabilities.
- Migration/upgrade guide.
- License and notices.

### 22.2 Required developer documentation

- Workshop conclusions.
- Architecture and transaction lifecycle.
- Stable IDs and canonical matching.
- Provider/reservation contract.
- Cancellation and commit points.
- Persistence/migration.
- Testing and release workflow.
- Extension module design.
- Current status and Current Notes.

### 22.3 Documentation truth rule

Examples must compile when implementation exists. Planned behavior is labeled planned. Screenshots, performance, platform compatibility, provider interoperability, migration, and release claims remain `Not run` until evidence exists.

### 22.4 Living repository workflow

Current Notes captures active observations. Durable decisions are promoted to this specification, the workshop record, ADRs, bridge specs, tests, guides, or changelog. Git stores history; Obsidian edits the same files.

### 22.5 Handoff order

README, SFGSS-000, SFGSS-002–005, this specification, workshop record, applicable bridge/ADR records, Current Notes, roadmap, audit, then implementation/tests when they exist.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | MVP required? |
|---|---|---|---:|
| EditMode definition | IDs, canonical signatures, policies, migrations | Duplicate recipe, ambiguity, alias | Yes |
| EditMode provider/transaction | Plans, revisions, reservations, idempotency | Prepare/commit/release | Yes |
| PlayMode | Root, providers, handles, cancellation, scene | Duplicate root, shutdown | Yes |
| Standalone Laboratory | Visible exact and standard loop | Combine, preview, failures | Yes |
| Integration Lab | Optional bridges | Vault transaction | When bridge ships |
| Clean-project | Packaging/removal | Install, sample removal, re-add | Yes |
| Existing-project adoption | Real game parity | Hackulos combine bag | Before adoption claim |

### 23.2 Required categories

Happy path, missing/invalid configuration, empty catalogs, duplicate IDs, ambiguous matching, provider absence, stale revisions, preparation/commit failure, cancellation, duplicate requests, batches, output capacity, station/context requirements, knowledge, import/migration, direct-scene entry, scene transitions, sample removal, optional integrations, performance bounds, and platform builds.

### 23.3 Test case registry

| Test ID | Requirement | Layer | Planned proof | Automation | Status |
|---|---|---|---|---|---|
| ECRF-T-001 | ECRF-LAB-001 / Initialize Crucible Laboratory | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The root, catalog, provider registry, station registry, diagnostics, and readout become Ready without another Echo package. | Planned | Not run |
| ECRF-T-002 | ECRF-LAB-001 / Initialize Crucible Laboratory | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The root, catalog, provider registry, station registry, diagnostics, and readout become Ready without another Echo package. | Planned | Not run |
| ECRF-T-003 | ECRF-LAB-001 / Initialize Crucible Laboratory | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The root, catalog, provider registry, station registry, diagnostics, and readout become Ready without another Echo package. | Planned | Not run |
| ECRF-T-004 | ECRF-LAB-001 / Initialize Crucible Laboratory | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The root, catalog, provider registry, station registry, diagnostics, and readout become Ready without another Echo package. | Planned | Not run |
| ECRF-T-005 | ECRF-LAB-001 / Initialize Crucible Laboratory | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The root, catalog, provider registry, station registry, diagnostics, and readout become Ready without another Echo package. | Planned | Not run |
| ECRF-T-006 | ECRF-LAB-001 / Initialize Crucible Laboratory | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The root, catalog, provider registry, station registry, diagnostics, and readout become Ready without another Echo package. | Planned | Not run |
| ECRF-T-007 | ECRF-LAB-002 / Missing configuration | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Initialization is blocked with an actionable ECRF diagnostic and no provider or recipe side effects. | Planned | Not run |
| ECRF-T-008 | ECRF-LAB-002 / Missing configuration | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Initialization is blocked with an actionable ECRF diagnostic and no provider or recipe side effects. | Planned | Not run |
| ECRF-T-009 | ECRF-LAB-002 / Missing configuration | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Initialization is blocked with an actionable ECRF diagnostic and no provider or recipe side effects. | Planned | Not run |
| ECRF-T-010 | ECRF-LAB-002 / Missing configuration | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Initialization is blocked with an actionable ECRF diagnostic and no provider or recipe side effects. | Planned | Not run |
| ECRF-T-011 | ECRF-LAB-002 / Missing configuration | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Initialization is blocked with an actionable ECRF diagnostic and no provider or recipe side effects. | Planned | Not run |
| ECRF-T-012 | ECRF-LAB-002 / Missing configuration | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Initialization is blocked with an actionable ECRF diagnostic and no provider or recipe side effects. | Planned | Not run |
| ECRF-T-013 | ECRF-LAB-003 / Duplicate root | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The duplicate rejects itself before provider registration, reservations, jobs, or events. | Planned | Not run |
| ECRF-T-014 | ECRF-LAB-003 / Duplicate root | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The duplicate rejects itself before provider registration, reservations, jobs, or events. | Planned | Not run |
| ECRF-T-015 | ECRF-LAB-003 / Duplicate root | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The duplicate rejects itself before provider registration, reservations, jobs, or events. | Planned | Not run |
| ECRF-T-016 | ECRF-LAB-003 / Duplicate root | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The duplicate rejects itself before provider registration, reservations, jobs, or events. | Planned | Not run |
| ECRF-T-017 | ECRF-LAB-003 / Duplicate root | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The duplicate rejects itself before provider registration, reservations, jobs, or events. | Planned | Not run |
| ECRF-T-018 | ECRF-LAB-003 / Duplicate root | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The duplicate rejects itself before provider registration, reservations, jobs, or events. | Planned | Not run |
| ECRF-T-019 | ECRF-LAB-004 / Duplicate recipe ID | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Validation blocks the conflicting catalog and reports both assets. | Planned | Not run |
| ECRF-T-020 | ECRF-LAB-004 / Duplicate recipe ID | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Validation blocks the conflicting catalog and reports both assets. | Planned | Not run |
| ECRF-T-021 | ECRF-LAB-004 / Duplicate recipe ID | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Validation blocks the conflicting catalog and reports both assets. | Planned | Not run |
| ECRF-T-022 | ECRF-LAB-004 / Duplicate recipe ID | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Validation blocks the conflicting catalog and reports both assets. | Planned | Not run |
| ECRF-T-023 | ECRF-LAB-004 / Duplicate recipe ID | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Validation blocks the conflicting catalog and reports both assets. | Planned | Not run |
| ECRF-T-024 | ECRF-LAB-004 / Duplicate recipe ID | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Validation blocks the conflicting catalog and reports both assets. | Planned | Not run |
| ECRF-T-025 | ECRF-LAB-005 / Missing recipe ID | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Validation blocks release use and offers a safe Editor repair operation. | Planned | Not run |
| ECRF-T-026 | ECRF-LAB-005 / Missing recipe ID | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Validation blocks release use and offers a safe Editor repair operation. | Planned | Not run |
| ECRF-T-027 | ECRF-LAB-005 / Missing recipe ID | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Validation blocks release use and offers a safe Editor repair operation. | Planned | Not run |
| ECRF-T-028 | ECRF-LAB-005 / Missing recipe ID | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Validation blocks release use and offers a safe Editor repair operation. | Planned | Not run |
| ECRF-T-029 | ECRF-LAB-005 / Missing recipe ID | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Validation blocks release use and offers a safe Editor repair operation. | Planned | Not run |
| ECRF-T-030 | ECRF-LAB-005 / Missing recipe ID | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Validation blocks release use and offers a safe Editor repair operation. | Planned | Not run |
| ECRF-T-031 | ECRF-LAB-006 / Unknown recipe request | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The service returns Unavailable without mutating resources. | Planned | Not run |
| ECRF-T-032 | ECRF-LAB-006 / Unknown recipe request | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The service returns Unavailable without mutating resources. | Planned | Not run |
| ECRF-T-033 | ECRF-LAB-006 / Unknown recipe request | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The service returns Unavailable without mutating resources. | Planned | Not run |
| ECRF-T-034 | ECRF-LAB-006 / Unknown recipe request | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The service returns Unavailable without mutating resources. | Planned | Not run |
| ECRF-T-035 | ECRF-LAB-006 / Unknown recipe request | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The service returns Unavailable without mutating resources. | Planned | Not run |
| ECRF-T-036 | ECRF-LAB-006 / Unknown recipe request | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The service returns Unavailable without mutating resources. | Planned | Not run |
| ECRF-T-037 | ECRF-LAB-007 / Exact combine success | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: One immediate transaction consumes the authored inputs and grants the authored output. | Planned | Not run |
| ECRF-T-038 | ECRF-LAB-007 / Exact combine success | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: One immediate transaction consumes the authored inputs and grants the authored output. | Planned | Not run |
| ECRF-T-039 | ECRF-LAB-007 / Exact combine success | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: One immediate transaction consumes the authored inputs and grants the authored output. | Planned | Not run |
| ECRF-T-040 | ECRF-LAB-007 / Exact combine success | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: One immediate transaction consumes the authored inputs and grants the authored output. | Planned | Not run |
| ECRF-T-041 | ECRF-LAB-007 / Exact combine success | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: One immediate transaction consumes the authored inputs and grants the authored output. | Planned | Not run |
| ECRF-T-042 | ECRF-LAB-007 / Exact combine success | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: One immediate transaction consumes the authored inputs and grants the authored output. | Planned | Not run |
| ECRF-T-043 | ECRF-LAB-008 / Exact combine missing ingredient | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Preview reports the missing requirement and execution does not reserve or consume anything. | Planned | Not run |
| ECRF-T-044 | ECRF-LAB-008 / Exact combine missing ingredient | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Preview reports the missing requirement and execution does not reserve or consume anything. | Planned | Not run |
| ECRF-T-045 | ECRF-LAB-008 / Exact combine missing ingredient | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Preview reports the missing requirement and execution does not reserve or consume anything. | Planned | Not run |
| ECRF-T-046 | ECRF-LAB-008 / Exact combine missing ingredient | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Preview reports the missing requirement and execution does not reserve or consume anything. | Planned | Not run |
| ECRF-T-047 | ECRF-LAB-008 / Exact combine missing ingredient | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Preview reports the missing requirement and execution does not reserve or consume anything. | Planned | Not run |
| ECRF-T-048 | ECRF-LAB-008 / Exact combine missing ingredient | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Preview reports the missing requirement and execution does not reserve or consume anything. | Planned | Not run |
| ECRF-T-049 | ECRF-LAB-009 / Exact combine extra ingredient rejected | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The exact-set matcher rejects the combination without guessing another recipe. | Planned | Not run |
| ECRF-T-050 | ECRF-LAB-009 / Exact combine extra ingredient rejected | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The exact-set matcher rejects the combination without guessing another recipe. | Planned | Not run |
| ECRF-T-051 | ECRF-LAB-009 / Exact combine extra ingredient rejected | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The exact-set matcher rejects the combination without guessing another recipe. | Planned | Not run |
| ECRF-T-052 | ECRF-LAB-009 / Exact combine extra ingredient rejected | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The exact-set matcher rejects the combination without guessing another recipe. | Planned | Not run |
| ECRF-T-053 | ECRF-LAB-009 / Exact combine extra ingredient rejected | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The exact-set matcher rejects the combination without guessing another recipe. | Planned | Not run |
| ECRF-T-054 | ECRF-LAB-009 / Exact combine extra ingredient rejected | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The exact-set matcher rejects the combination without guessing another recipe. | Planned | Not run |
| ECRF-T-055 | ECRF-LAB-010 / Exact combine extra ingredient allowed | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Only declared inputs participate and the remaining contents are unchanged. | Planned | Not run |
| ECRF-T-056 | ECRF-LAB-010 / Exact combine extra ingredient allowed | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Only declared inputs participate and the remaining contents are unchanged. | Planned | Not run |
| ECRF-T-057 | ECRF-LAB-010 / Exact combine extra ingredient allowed | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Only declared inputs participate and the remaining contents are unchanged. | Planned | Not run |
| ECRF-T-058 | ECRF-LAB-010 / Exact combine extra ingredient allowed | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Only declared inputs participate and the remaining contents are unchanged. | Planned | Not run |
| ECRF-T-059 | ECRF-LAB-010 / Exact combine extra ingredient allowed | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Only declared inputs participate and the remaining contents are unchanged. | Planned | Not run |
| ECRF-T-060 | ECRF-LAB-010 / Exact combine extra ingredient allowed | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Only declared inputs participate and the remaining contents are unchanged. | Planned | Not run |
| ECRF-T-061 | ECRF-LAB-011 / Exact recipe ambiguity | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Validation blocks the ambiguity before runtime. | Planned | Not run |
| ECRF-T-062 | ECRF-LAB-011 / Exact recipe ambiguity | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Validation blocks the ambiguity before runtime. | Planned | Not run |
| ECRF-T-063 | ECRF-LAB-011 / Exact recipe ambiguity | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Validation blocks the ambiguity before runtime. | Planned | Not run |
| ECRF-T-064 | ECRF-LAB-011 / Exact recipe ambiguity | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Validation blocks the ambiguity before runtime. | Planned | Not run |
| ECRF-T-065 | ECRF-LAB-011 / Exact recipe ambiguity | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Validation blocks the ambiguity before runtime. | Planned | Not run |
| ECRF-T-066 | ECRF-LAB-011 / Exact recipe ambiguity | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Validation blocks the ambiguity before runtime. | Planned | Not run |
| ECRF-T-067 | ECRF-LAB-012 / Tag ingredient match | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The provider resolves one deterministic eligible resource set. | Planned | Not run |
| ECRF-T-068 | ECRF-LAB-012 / Tag ingredient match | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The provider resolves one deterministic eligible resource set. | Planned | Not run |
| ECRF-T-069 | ECRF-LAB-012 / Tag ingredient match | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The provider resolves one deterministic eligible resource set. | Planned | Not run |
| ECRF-T-070 | ECRF-LAB-012 / Tag ingredient match | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The provider resolves one deterministic eligible resource set. | Planned | Not run |
| ECRF-T-071 | ECRF-LAB-012 / Tag ingredient match | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The provider resolves one deterministic eligible resource set. | Planned | Not run |
| ECRF-T-072 | ECRF-LAB-012 / Tag ingredient match | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The provider resolves one deterministic eligible resource set. | Planned | Not run |
| ECRF-T-073 | ECRF-LAB-013 / Alternative ingredient group | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Exactly one alternative is selected according to deterministic provider ordering. | Planned | Not run |
| ECRF-T-074 | ECRF-LAB-013 / Alternative ingredient group | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Exactly one alternative is selected according to deterministic provider ordering. | Planned | Not run |
| ECRF-T-075 | ECRF-LAB-013 / Alternative ingredient group | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Exactly one alternative is selected according to deterministic provider ordering. | Planned | Not run |
| ECRF-T-076 | ECRF-LAB-013 / Alternative ingredient group | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Exactly one alternative is selected according to deterministic provider ordering. | Planned | Not run |
| ECRF-T-077 | ECRF-LAB-013 / Alternative ingredient group | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Exactly one alternative is selected according to deterministic provider ordering. | Planned | Not run |
| ECRF-T-078 | ECRF-LAB-013 / Alternative ingredient group | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Exactly one alternative is selected according to deterministic provider ordering. | Planned | Not run |
| ECRF-T-079 | ECRF-LAB-014 / Multiple ingredient groups | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Preview produces one complete deterministic resource plan. | Planned | Not run |
| ECRF-T-080 | ECRF-LAB-014 / Multiple ingredient groups | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Preview produces one complete deterministic resource plan. | Planned | Not run |
| ECRF-T-081 | ECRF-LAB-014 / Multiple ingredient groups | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Preview produces one complete deterministic resource plan. | Planned | Not run |
| ECRF-T-082 | ECRF-LAB-014 / Multiple ingredient groups | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Preview produces one complete deterministic resource plan. | Planned | Not run |
| ECRF-T-083 | ECRF-LAB-014 / Multiple ingredient groups | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Preview produces one complete deterministic resource plan. | Planned | Not run |
| ECRF-T-084 | ECRF-LAB-014 / Multiple ingredient groups | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Preview produces one complete deterministic resource plan. | Planned | Not run |
| ECRF-T-085 | ECRF-LAB-015 / Insufficient quantity | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Preview reports required, available, and missing quantities without mutation. | Planned | Not run |
| ECRF-T-086 | ECRF-LAB-015 / Insufficient quantity | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Preview reports required, available, and missing quantities without mutation. | Planned | Not run |
| ECRF-T-087 | ECRF-LAB-015 / Insufficient quantity | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Preview reports required, available, and missing quantities without mutation. | Planned | Not run |
| ECRF-T-088 | ECRF-LAB-015 / Insufficient quantity | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Preview reports required, available, and missing quantities without mutation. | Planned | Not run |
| ECRF-T-089 | ECRF-LAB-015 / Insufficient quantity | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Preview reports required, available, and missing quantities without mutation. | Planned | Not run |
| ECRF-T-090 | ECRF-LAB-015 / Insufficient quantity | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Preview reports required, available, and missing quantities without mutation. | Planned | Not run |
| ECRF-T-091 | ECRF-LAB-016 / Non-consumable tool requirement | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The tool is validated but not consumed by the transaction. | Planned | Not run |
| ECRF-T-092 | ECRF-LAB-016 / Non-consumable tool requirement | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The tool is validated but not consumed by the transaction. | Planned | Not run |
| ECRF-T-093 | ECRF-LAB-016 / Non-consumable tool requirement | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The tool is validated but not consumed by the transaction. | Planned | Not run |
| ECRF-T-094 | ECRF-LAB-016 / Non-consumable tool requirement | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The tool is validated but not consumed by the transaction. | Planned | Not run |
| ECRF-T-095 | ECRF-LAB-016 / Non-consumable tool requirement | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The tool is validated but not consumed by the transaction. | Planned | Not run |
| ECRF-T-096 | ECRF-LAB-016 / Non-consumable tool requirement | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The tool is validated but not consumed by the transaction. | Planned | Not run |
| ECRF-T-097 | ECRF-LAB-017 / Catalyst requirement | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The preview and transaction apply the authored catalyst rule exactly once. | Planned | Not run |
| ECRF-T-098 | ECRF-LAB-017 / Catalyst requirement | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The preview and transaction apply the authored catalyst rule exactly once. | Planned | Not run |
| ECRF-T-099 | ECRF-LAB-017 / Catalyst requirement | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The preview and transaction apply the authored catalyst rule exactly once. | Planned | Not run |
| ECRF-T-100 | ECRF-LAB-017 / Catalyst requirement | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The preview and transaction apply the authored catalyst rule exactly once. | Planned | Not run |
| ECRF-T-101 | ECRF-LAB-017 / Catalyst requirement | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The preview and transaction apply the authored catalyst rule exactly once. | Planned | Not run |
| ECRF-T-102 | ECRF-LAB-017 / Catalyst requirement | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The preview and transaction apply the authored catalyst rule exactly once. | Planned | Not run |
| ECRF-T-103 | ECRF-LAB-018 / Station tag requirement | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The request is denied before resource preparation. | Planned | Not run |
| ECRF-T-104 | ECRF-LAB-018 / Station tag requirement | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The request is denied before resource preparation. | Planned | Not run |
| ECRF-T-105 | ECRF-LAB-018 / Station tag requirement | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The request is denied before resource preparation. | Planned | Not run |
| ECRF-T-106 | ECRF-LAB-018 / Station tag requirement | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The request is denied before resource preparation. | Planned | Not run |
| ECRF-T-107 | ECRF-LAB-018 / Station tag requirement | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The request is denied before resource preparation. | Planned | Not run |
| ECRF-T-108 | ECRF-LAB-018 / Station tag requirement | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The request is denied before resource preparation. | Planned | Not run |
| ECRF-T-109 | ECRF-LAB-019 / Station requirement success | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The station requirement passes and crafting can proceed. | Planned | Not run |
| ECRF-T-110 | ECRF-LAB-019 / Station requirement success | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The station requirement passes and crafting can proceed. | Planned | Not run |
| ECRF-T-111 | ECRF-LAB-019 / Station requirement success | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The station requirement passes and crafting can proceed. | Planned | Not run |
| ECRF-T-112 | ECRF-LAB-019 / Station requirement success | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The station requirement passes and crafting can proceed. | Planned | Not run |
| ECRF-T-113 | ECRF-LAB-019 / Station requirement success | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The station requirement passes and crafting can proceed. | Planned | Not run |
| ECRF-T-114 | ECRF-LAB-019 / Station requirement success | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The station requirement passes and crafting can proceed. | Planned | Not run |
| ECRF-T-115 | ECRF-LAB-020 / No-station recipe | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The recipe succeeds without inventing a station object. | Planned | Not run |
| ECRF-T-116 | ECRF-LAB-020 / No-station recipe | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The recipe succeeds without inventing a station object. | Planned | Not run |
| ECRF-T-117 | ECRF-LAB-020 / No-station recipe | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The recipe succeeds without inventing a station object. | Planned | Not run |
| ECRF-T-118 | ECRF-LAB-020 / No-station recipe | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The recipe succeeds without inventing a station object. | Planned | Not run |
| ECRF-T-119 | ECRF-LAB-020 / No-station recipe | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The recipe succeeds without inventing a station object. | Planned | Not run |
| ECRF-T-120 | ECRF-LAB-020 / No-station recipe | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The recipe succeeds without inventing a station object. | Planned | Not run |
| ECRF-T-121 | ECRF-LAB-021 / Unknown station | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The request returns Unavailable and no resources are touched. | Planned | Not run |
| ECRF-T-122 | ECRF-LAB-021 / Unknown station | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The request returns Unavailable and no resources are touched. | Planned | Not run |
| ECRF-T-123 | ECRF-LAB-021 / Unknown station | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The request returns Unavailable and no resources are touched. | Planned | Not run |
| ECRF-T-124 | ECRF-LAB-021 / Unknown station | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The request returns Unavailable and no resources are touched. | Planned | Not run |
| ECRF-T-125 | ECRF-LAB-021 / Unknown station | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The request returns Unavailable and no resources are touched. | Planned | Not run |
| ECRF-T-126 | ECRF-LAB-021 / Unknown station | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The request returns Unavailable and no resources are touched. | Planned | Not run |
| ECRF-T-127 | ECRF-LAB-022 / Stale station handle | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The stale station handle is rejected safely. | Planned | Not run |
| ECRF-T-128 | ECRF-LAB-022 / Stale station handle | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The stale station handle is rejected safely. | Planned | Not run |
| ECRF-T-129 | ECRF-LAB-022 / Stale station handle | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The stale station handle is rejected safely. | Planned | Not run |
| ECRF-T-130 | ECRF-LAB-022 / Stale station handle | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The stale station handle is rejected safely. | Planned | Not run |
| ECRF-T-131 | ECRF-LAB-022 / Stale station handle | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The stale station handle is rejected safely. | Planned | Not run |
| ECRF-T-132 | ECRF-LAB-022 / Stale station handle | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The stale station handle is rejected safely. | Planned | Not run |
| ECRF-T-133 | ECRF-LAB-023 / Context requirement success | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The provider returns Satisfied and the recipe continues. | Planned | Not run |
| ECRF-T-134 | ECRF-LAB-023 / Context requirement success | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The provider returns Satisfied and the recipe continues. | Planned | Not run |
| ECRF-T-135 | ECRF-LAB-023 / Context requirement success | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The provider returns Satisfied and the recipe continues. | Planned | Not run |
| ECRF-T-136 | ECRF-LAB-023 / Context requirement success | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The provider returns Satisfied and the recipe continues. | Planned | Not run |
| ECRF-T-137 | ECRF-LAB-023 / Context requirement success | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The provider returns Satisfied and the recipe continues. | Planned | Not run |
| ECRF-T-138 | ECRF-LAB-023 / Context requirement success | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The provider returns Satisfied and the recipe continues. | Planned | Not run |
| ECRF-T-139 | ECRF-LAB-024 / Context requirement unavailable | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The recipe reports Unavailable rather than treating the requirement as false or satisfied. | Planned | Not run |
| ECRF-T-140 | ECRF-LAB-024 / Context requirement unavailable | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The recipe reports Unavailable rather than treating the requirement as false or satisfied. | Planned | Not run |
| ECRF-T-141 | ECRF-LAB-024 / Context requirement unavailable | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The recipe reports Unavailable rather than treating the requirement as false or satisfied. | Planned | Not run |
| ECRF-T-142 | ECRF-LAB-024 / Context requirement unavailable | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The recipe reports Unavailable rather than treating the requirement as false or satisfied. | Planned | Not run |
| ECRF-T-143 | ECRF-LAB-024 / Context requirement unavailable | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The recipe reports Unavailable rather than treating the requirement as false or satisfied. | Planned | Not run |
| ECRF-T-144 | ECRF-LAB-024 / Context requirement unavailable | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The recipe reports Unavailable rather than treating the requirement as false or satisfied. | Planned | Not run |
| ECRF-T-145 | ECRF-LAB-025 / Skill requirement provider | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The provider determines eligibility without EchoCrafting owning skill state. | Planned | Not run |
| ECRF-T-146 | ECRF-LAB-025 / Skill requirement provider | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The provider determines eligibility without EchoCrafting owning skill state. | Planned | Not run |
| ECRF-T-147 | ECRF-LAB-025 / Skill requirement provider | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The provider determines eligibility without EchoCrafting owning skill state. | Planned | Not run |
| ECRF-T-148 | ECRF-LAB-025 / Skill requirement provider | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The provider determines eligibility without EchoCrafting owning skill state. | Planned | Not run |
| ECRF-T-149 | ECRF-LAB-025 / Skill requirement provider | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The provider determines eligibility without EchoCrafting owning skill state. | Planned | Not run |
| ECRF-T-150 | ECRF-LAB-025 / Skill requirement provider | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The provider determines eligibility without EchoCrafting owning skill state. | Planned | Not run |
| ECRF-T-151 | ECRF-LAB-026 / Read-only requirement enforcement | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The provider contract is rejected or diagnosed; evaluation remains side-effect-free. | Planned | Not run |
| ECRF-T-152 | ECRF-LAB-026 / Read-only requirement enforcement | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The provider contract is rejected or diagnosed; evaluation remains side-effect-free. | Planned | Not run |
| ECRF-T-153 | ECRF-LAB-026 / Read-only requirement enforcement | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The provider contract is rejected or diagnosed; evaluation remains side-effect-free. | Planned | Not run |
| ECRF-T-154 | ECRF-LAB-026 / Read-only requirement enforcement | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The provider contract is rejected or diagnosed; evaluation remains side-effect-free. | Planned | Not run |
| ECRF-T-155 | ECRF-LAB-026 / Read-only requirement enforcement | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The provider contract is rejected or diagnosed; evaluation remains side-effect-free. | Planned | Not run |
| ECRF-T-156 | ECRF-LAB-026 / Read-only requirement enforcement | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The provider contract is rejected or diagnosed; evaluation remains side-effect-free. | Planned | Not run |
| ECRF-T-157 | ECRF-LAB-027 / Known recipe visible | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Recipe browsing and direct evaluation report Known. | Planned | Not run |
| ECRF-T-158 | ECRF-LAB-027 / Known recipe visible | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Recipe browsing and direct evaluation report Known. | Planned | Not run |
| ECRF-T-159 | ECRF-LAB-027 / Known recipe visible | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Recipe browsing and direct evaluation report Known. | Planned | Not run |
| ECRF-T-160 | ECRF-LAB-027 / Known recipe visible | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Recipe browsing and direct evaluation report Known. | Planned | Not run |
| ECRF-T-161 | ECRF-LAB-027 / Known recipe visible | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Recipe browsing and direct evaluation report Known. | Planned | Not run |
| ECRF-T-162 | ECRF-LAB-027 / Known recipe visible | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Recipe browsing and direct evaluation report Known. | Planned | Not run |
| ECRF-T-163 | ECRF-LAB-028 / Unknown recipe hidden | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Normal browsing omits it while exact-match probing follows the configured secrecy policy. | Planned | Not run |
| ECRF-T-164 | ECRF-LAB-028 / Unknown recipe hidden | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Normal browsing omits it while exact-match probing follows the configured secrecy policy. | Planned | Not run |
| ECRF-T-165 | ECRF-LAB-028 / Unknown recipe hidden | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Normal browsing omits it while exact-match probing follows the configured secrecy policy. | Planned | Not run |
| ECRF-T-166 | ECRF-LAB-028 / Unknown recipe hidden | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Normal browsing omits it while exact-match probing follows the configured secrecy policy. | Planned | Not run |
| ECRF-T-167 | ECRF-LAB-028 / Unknown recipe hidden | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Normal browsing omits it while exact-match probing follows the configured secrecy policy. | Planned | Not run |
| ECRF-T-168 | ECRF-LAB-028 / Unknown recipe hidden | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Normal browsing omits it while exact-match probing follows the configured secrecy policy. | Planned | Not run |
| ECRF-T-169 | ECRF-LAB-029 / Discover recipe | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Knowledge commits once and emits one semantic event. | Planned | Not run |
| ECRF-T-170 | ECRF-LAB-029 / Discover recipe | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Knowledge commits once and emits one semantic event. | Planned | Not run |
| ECRF-T-171 | ECRF-LAB-029 / Discover recipe | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Knowledge commits once and emits one semantic event. | Planned | Not run |
| ECRF-T-172 | ECRF-LAB-029 / Discover recipe | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Knowledge commits once and emits one semantic event. | Planned | Not run |
| ECRF-T-173 | ECRF-LAB-029 / Discover recipe | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Knowledge commits once and emits one semantic event. | Planned | Not run |
| ECRF-T-174 | ECRF-LAB-029 / Discover recipe | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Knowledge commits once and emits one semantic event. | Planned | Not run |
| ECRF-T-175 | ECRF-LAB-030 / Duplicate discovery request | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The operation is idempotent and does not duplicate events or durable records. | Planned | Not run |
| ECRF-T-176 | ECRF-LAB-030 / Duplicate discovery request | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The operation is idempotent and does not duplicate events or durable records. | Planned | Not run |
| ECRF-T-177 | ECRF-LAB-030 / Duplicate discovery request | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The operation is idempotent and does not duplicate events or durable records. | Planned | Not run |
| ECRF-T-178 | ECRF-LAB-030 / Duplicate discovery request | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The operation is idempotent and does not duplicate events or durable records. | Planned | Not run |
| ECRF-T-179 | ECRF-LAB-030 / Duplicate discovery request | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The operation is idempotent and does not duplicate events or durable records. | Planned | Not run |
| ECRF-T-180 | ECRF-LAB-030 / Duplicate discovery request | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The operation is idempotent and does not duplicate events or durable records. | Planned | Not run |
| ECRF-T-181 | ECRF-LAB-031 / Revoke recipe knowledge | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Knowledge is removed only through the documented reset path. | Planned | Not run |
| ECRF-T-182 | ECRF-LAB-031 / Revoke recipe knowledge | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Knowledge is removed only through the documented reset path. | Planned | Not run |
| ECRF-T-183 | ECRF-LAB-031 / Revoke recipe knowledge | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Knowledge is removed only through the documented reset path. | Planned | Not run |
| ECRF-T-184 | ECRF-LAB-031 / Revoke recipe knowledge | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Knowledge is removed only through the documented reset path. | Planned | Not run |
| ECRF-T-185 | ECRF-LAB-031 / Revoke recipe knowledge | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Knowledge is removed only through the documented reset path. | Planned | Not run |
| ECRF-T-186 | ECRF-LAB-031 / Revoke recipe knowledge | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Knowledge is removed only through the documented reset path. | Planned | Not run |
| ECRF-T-187 | ECRF-LAB-032 / Preview without mutation | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Resource counts, item instances, knowledge, queues, and stations remain unchanged. | Planned | Not run |
| ECRF-T-188 | ECRF-LAB-032 / Preview without mutation | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Resource counts, item instances, knowledge, queues, and stations remain unchanged. | Planned | Not run |
| ECRF-T-189 | ECRF-LAB-032 / Preview without mutation | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Resource counts, item instances, knowledge, queues, and stations remain unchanged. | Planned | Not run |
| ECRF-T-190 | ECRF-LAB-032 / Preview without mutation | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Resource counts, item instances, knowledge, queues, and stations remain unchanged. | Planned | Not run |
| ECRF-T-191 | ECRF-LAB-032 / Preview without mutation | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Resource counts, item instances, knowledge, queues, and stations remain unchanged. | Planned | Not run |
| ECRF-T-192 | ECRF-LAB-032 / Preview without mutation | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Resource counts, item instances, knowledge, queues, and stations remain unchanged. | Planned | Not run |
| ECRF-T-193 | ECRF-LAB-033 / Preview fingerprint stability | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The canonical plan fingerprint remains stable. | Planned | Not run |
| ECRF-T-194 | ECRF-LAB-033 / Preview fingerprint stability | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The canonical plan fingerprint remains stable. | Planned | Not run |
| ECRF-T-195 | ECRF-LAB-033 / Preview fingerprint stability | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The canonical plan fingerprint remains stable. | Planned | Not run |
| ECRF-T-196 | ECRF-LAB-033 / Preview fingerprint stability | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The canonical plan fingerprint remains stable. | Planned | Not run |
| ECRF-T-197 | ECRF-LAB-033 / Preview fingerprint stability | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The canonical plan fingerprint remains stable. | Planned | Not run |
| ECRF-T-198 | ECRF-LAB-033 / Preview fingerprint stability | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The canonical plan fingerprint remains stable. | Planned | Not run |
| ECRF-T-199 | ECRF-LAB-034 / Stale preview revision | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Execution is rejected and requires re-preview or explicit re-evaluation. | Planned | Not run |
| ECRF-T-200 | ECRF-LAB-034 / Stale preview revision | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Execution is rejected and requires re-preview or explicit re-evaluation. | Planned | Not run |
| ECRF-T-201 | ECRF-LAB-034 / Stale preview revision | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Execution is rejected and requires re-preview or explicit re-evaluation. | Planned | Not run |
| ECRF-T-202 | ECRF-LAB-034 / Stale preview revision | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Execution is rejected and requires re-preview or explicit re-evaluation. | Planned | Not run |
| ECRF-T-203 | ECRF-LAB-034 / Stale preview revision | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Execution is rejected and requires re-preview or explicit re-evaluation. | Planned | Not run |
| ECRF-T-204 | ECRF-LAB-034 / Stale preview revision | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Execution is rejected and requires re-preview or explicit re-evaluation. | Planned | Not run |
| ECRF-T-205 | ECRF-LAB-035 / Immediate craft success | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: One provider reservation commits inputs and outputs atomically. | Planned | Not run |
| ECRF-T-206 | ECRF-LAB-035 / Immediate craft success | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: One provider reservation commits inputs and outputs atomically. | Planned | Not run |
| ECRF-T-207 | ECRF-LAB-035 / Immediate craft success | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: One provider reservation commits inputs and outputs atomically. | Planned | Not run |
| ECRF-T-208 | ECRF-LAB-035 / Immediate craft success | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: One provider reservation commits inputs and outputs atomically. | Planned | Not run |
| ECRF-T-209 | ECRF-LAB-035 / Immediate craft success | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: One provider reservation commits inputs and outputs atomically. | Planned | Not run |
| ECRF-T-210 | ECRF-LAB-035 / Immediate craft success | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: One provider reservation commits inputs and outputs atomically. | Planned | Not run |
| ECRF-T-211 | ECRF-LAB-036 / Immediate provider prepare failure | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The request fails before the commit point and resources remain unchanged. | Planned | Not run |
| ECRF-T-212 | ECRF-LAB-036 / Immediate provider prepare failure | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The request fails before the commit point and resources remain unchanged. | Planned | Not run |
| ECRF-T-213 | ECRF-LAB-036 / Immediate provider prepare failure | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The request fails before the commit point and resources remain unchanged. | Planned | Not run |
| ECRF-T-214 | ECRF-LAB-036 / Immediate provider prepare failure | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The request fails before the commit point and resources remain unchanged. | Planned | Not run |
| ECRF-T-215 | ECRF-LAB-036 / Immediate provider prepare failure | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The request fails before the commit point and resources remain unchanged. | Planned | Not run |
| ECRF-T-216 | ECRF-LAB-036 / Immediate provider prepare failure | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The request fails before the commit point and resources remain unchanged. | Planned | Not run |
| ECRF-T-217 | ECRF-LAB-037 / Immediate provider commit failure | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The reservation rolls back and the result reports no committed mutation. | Planned | Not run |
| ECRF-T-218 | ECRF-LAB-037 / Immediate provider commit failure | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The reservation rolls back and the result reports no committed mutation. | Planned | Not run |
| ECRF-T-219 | ECRF-LAB-037 / Immediate provider commit failure | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The reservation rolls back and the result reports no committed mutation. | Planned | Not run |
| ECRF-T-220 | ECRF-LAB-037 / Immediate provider commit failure | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The reservation rolls back and the result reports no committed mutation. | Planned | Not run |
| ECRF-T-221 | ECRF-LAB-037 / Immediate provider commit failure | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The reservation rolls back and the result reports no committed mutation. | Planned | Not run |
| ECRF-T-222 | ECRF-LAB-037 / Immediate provider commit failure | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The reservation rolls back and the result reports no committed mutation. | Planned | Not run |
| ECRF-T-223 | ECRF-LAB-038 / Provider failure after commit | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The result is CommittedWithDiagnostic and does not retry or duplicate outputs. | Planned | Not run |
| ECRF-T-224 | ECRF-LAB-038 / Provider failure after commit | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The result is CommittedWithDiagnostic and does not retry or duplicate outputs. | Planned | Not run |
| ECRF-T-225 | ECRF-LAB-038 / Provider failure after commit | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The result is CommittedWithDiagnostic and does not retry or duplicate outputs. | Planned | Not run |
| ECRF-T-226 | ECRF-LAB-038 / Provider failure after commit | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The result is CommittedWithDiagnostic and does not retry or duplicate outputs. | Planned | Not run |
| ECRF-T-227 | ECRF-LAB-038 / Provider failure after commit | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The result is CommittedWithDiagnostic and does not retry or duplicate outputs. | Planned | Not run |
| ECRF-T-228 | ECRF-LAB-038 / Provider failure after commit | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The result is CommittedWithDiagnostic and does not retry or duplicate outputs. | Planned | Not run |
| ECRF-T-229 | ECRF-LAB-039 / Cancel before prepare | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The operation ends Cancelled with no provider reservation. | Planned | Not run |
| ECRF-T-230 | ECRF-LAB-039 / Cancel before prepare | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The operation ends Cancelled with no provider reservation. | Planned | Not run |
| ECRF-T-231 | ECRF-LAB-039 / Cancel before prepare | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The operation ends Cancelled with no provider reservation. | Planned | Not run |
| ECRF-T-232 | ECRF-LAB-039 / Cancel before prepare | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The operation ends Cancelled with no provider reservation. | Planned | Not run |
| ECRF-T-233 | ECRF-LAB-039 / Cancel before prepare | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The operation ends Cancelled with no provider reservation. | Planned | Not run |
| ECRF-T-234 | ECRF-LAB-039 / Cancel before prepare | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The operation ends Cancelled with no provider reservation. | Planned | Not run |
| ECRF-T-235 | ECRF-LAB-040 / Cancel after reservation before commit | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The reservation is released and no resources are consumed or granted. | Planned | Not run |
| ECRF-T-236 | ECRF-LAB-040 / Cancel after reservation before commit | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The reservation is released and no resources are consumed or granted. | Planned | Not run |
| ECRF-T-237 | ECRF-LAB-040 / Cancel after reservation before commit | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The reservation is released and no resources are consumed or granted. | Planned | Not run |
| ECRF-T-238 | ECRF-LAB-040 / Cancel after reservation before commit | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The reservation is released and no resources are consumed or granted. | Planned | Not run |
| ECRF-T-239 | ECRF-LAB-040 / Cancel after reservation before commit | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The reservation is released and no resources are consumed or granted. | Planned | Not run |
| ECRF-T-240 | ECRF-LAB-040 / Cancel after reservation before commit | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The reservation is released and no resources are consumed or granted. | Planned | Not run |
| ECRF-T-241 | ECRF-LAB-041 / Cancel after commit point | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The result is TooLate and the committed crafting result remains authoritative. | Planned | Not run |
| ECRF-T-242 | ECRF-LAB-041 / Cancel after commit point | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The result is TooLate and the committed crafting result remains authoritative. | Planned | Not run |
| ECRF-T-243 | ECRF-LAB-041 / Cancel after commit point | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The result is TooLate and the committed crafting result remains authoritative. | Planned | Not run |
| ECRF-T-244 | ECRF-LAB-041 / Cancel after commit point | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The result is TooLate and the committed crafting result remains authoritative. | Planned | Not run |
| ECRF-T-245 | ECRF-LAB-041 / Cancel after commit point | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The result is TooLate and the committed crafting result remains authoritative. | Planned | Not run |
| ECRF-T-246 | ECRF-LAB-041 / Cancel after commit point | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The result is TooLate and the committed crafting result remains authoritative. | Planned | Not run |
| ECRF-T-247 | ECRF-LAB-042 / Request idempotency | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The original result is returned without executing another resource mutation. | Planned | Not run |
| ECRF-T-248 | ECRF-LAB-042 / Request idempotency | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The original result is returned without executing another resource mutation. | Planned | Not run |
| ECRF-T-249 | ECRF-LAB-042 / Request idempotency | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The original result is returned without executing another resource mutation. | Planned | Not run |
| ECRF-T-250 | ECRF-LAB-042 / Request idempotency | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The original result is returned without executing another resource mutation. | Planned | Not run |
| ECRF-T-251 | ECRF-LAB-042 / Request idempotency | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The original result is returned without executing another resource mutation. | Planned | Not run |
| ECRF-T-252 | ECRF-LAB-042 / Request idempotency | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The original result is returned without executing another resource mutation. | Planned | Not run |
| ECRF-T-253 | ECRF-LAB-043 / Batch craft success | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Quantities scale deterministically and commit as one atomic resource plan. | Planned | Not run |
| ECRF-T-254 | ECRF-LAB-043 / Batch craft success | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Quantities scale deterministically and commit as one atomic resource plan. | Planned | Not run |
| ECRF-T-255 | ECRF-LAB-043 / Batch craft success | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Quantities scale deterministically and commit as one atomic resource plan. | Planned | Not run |
| ECRF-T-256 | ECRF-LAB-043 / Batch craft success | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Quantities scale deterministically and commit as one atomic resource plan. | Planned | Not run |
| ECRF-T-257 | ECRF-LAB-043 / Batch craft success | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Quantities scale deterministically and commit as one atomic resource plan. | Planned | Not run |
| ECRF-T-258 | ECRF-LAB-043 / Batch craft success | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Quantities scale deterministically and commit as one atomic resource plan. | Planned | Not run |
| ECRF-T-259 | ECRF-LAB-044 / Batch exceeds recipe limit | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Validation rejects the request before provider preparation. | Planned | Not run |
| ECRF-T-260 | ECRF-LAB-044 / Batch exceeds recipe limit | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Validation rejects the request before provider preparation. | Planned | Not run |
| ECRF-T-261 | ECRF-LAB-044 / Batch exceeds recipe limit | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Validation rejects the request before provider preparation. | Planned | Not run |
| ECRF-T-262 | ECRF-LAB-044 / Batch exceeds recipe limit | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Validation rejects the request before provider preparation. | Planned | Not run |
| ECRF-T-263 | ECRF-LAB-044 / Batch exceeds recipe limit | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Validation rejects the request before provider preparation. | Planned | Not run |
| ECRF-T-264 | ECRF-LAB-044 / Batch exceeds recipe limit | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Validation rejects the request before provider preparation. | Planned | Not run |
| ECRF-T-265 | ECRF-LAB-045 / Batch exceeds available resources | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Preview reports the maximum craftable count and execution does not partially craft. | Planned | Not run |
| ECRF-T-266 | ECRF-LAB-045 / Batch exceeds available resources | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Preview reports the maximum craftable count and execution does not partially craft. | Planned | Not run |
| ECRF-T-267 | ECRF-LAB-045 / Batch exceeds available resources | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Preview reports the maximum craftable count and execution does not partially craft. | Planned | Not run |
| ECRF-T-268 | ECRF-LAB-045 / Batch exceeds available resources | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Preview reports the maximum craftable count and execution does not partially craft. | Planned | Not run |
| ECRF-T-269 | ECRF-LAB-045 / Batch exceeds available resources | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Preview reports the maximum craftable count and execution does not partially craft. | Planned | Not run |
| ECRF-T-270 | ECRF-LAB-045 / Batch exceeds available resources | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Preview reports the maximum craftable count and execution does not partially craft. | Planned | Not run |
| ECRF-T-271 | ECRF-LAB-046 / Output destination full | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Preparation fails before inputs are consumed. | Planned | Not run |
| ECRF-T-272 | ECRF-LAB-046 / Output destination full | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Preparation fails before inputs are consumed. | Planned | Not run |
| ECRF-T-273 | ECRF-LAB-046 / Output destination full | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Preparation fails before inputs are consumed. | Planned | Not run |
| ECRF-T-274 | ECRF-LAB-046 / Output destination full | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Preparation fails before inputs are consumed. | Planned | Not run |
| ECRF-T-275 | ECRF-LAB-046 / Output destination full | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Preparation fails before inputs are consumed. | Planned | Not run |
| ECRF-T-276 | ECRF-LAB-046 / Output destination full | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Preparation fails before inputs are consumed. | Planned | Not run |
| ECRF-T-277 | ECRF-LAB-047 / Byproduct grant | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Primary outputs and byproducts are included in the same provider transaction. | Planned | Not run |
| ECRF-T-278 | ECRF-LAB-047 / Byproduct grant | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Primary outputs and byproducts are included in the same provider transaction. | Planned | Not run |
| ECRF-T-279 | ECRF-LAB-047 / Byproduct grant | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Primary outputs and byproducts are included in the same provider transaction. | Planned | Not run |
| ECRF-T-280 | ECRF-LAB-047 / Byproduct grant | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Primary outputs and byproducts are included in the same provider transaction. | Planned | Not run |
| ECRF-T-281 | ECRF-LAB-047 / Byproduct grant | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Primary outputs and byproducts are included in the same provider transaction. | Planned | Not run |
| ECRF-T-282 | ECRF-LAB-047 / Byproduct grant | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Primary outputs and byproducts are included in the same provider transaction. | Planned | Not run |
| ECRF-T-283 | ECRF-LAB-048 / Output choice policy | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The selected authored option is validated and granted exactly once. | Planned | Not run |
| ECRF-T-284 | ECRF-LAB-048 / Output choice policy | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The selected authored option is validated and granted exactly once. | Planned | Not run |
| ECRF-T-285 | ECRF-LAB-048 / Output choice policy | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The selected authored option is validated and granted exactly once. | Planned | Not run |
| ECRF-T-286 | ECRF-LAB-048 / Output choice policy | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The selected authored option is validated and granted exactly once. | Planned | Not run |
| ECRF-T-287 | ECRF-LAB-048 / Output choice policy | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The selected authored option is validated and granted exactly once. | Planned | Not run |
| ECRF-T-288 | ECRF-LAB-048 / Output choice policy | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The selected authored option is validated and granted exactly once. | Planned | Not run |
| ECRF-T-289 | ECRF-LAB-049 / Invalid output choice | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The request is rejected before mutation. | Planned | Not run |
| ECRF-T-290 | ECRF-LAB-049 / Invalid output choice | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The request is rejected before mutation. | Planned | Not run |
| ECRF-T-291 | ECRF-LAB-049 / Invalid output choice | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The request is rejected before mutation. | Planned | Not run |
| ECRF-T-292 | ECRF-LAB-049 / Invalid output choice | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The request is rejected before mutation. | Planned | Not run |
| ECRF-T-293 | ECRF-LAB-049 / Invalid output choice | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The request is rejected before mutation. | Planned | Not run |
| ECRF-T-294 | ECRF-LAB-049 / Invalid output choice | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The request is rejected before mutation. | Planned | Not run |
| ECRF-T-295 | ECRF-LAB-050 / Single resource provider rule | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The core rejects unsupported distributed mutation and explains the single-provider MVP boundary. | Planned | Not run |
| ECRF-T-296 | ECRF-LAB-050 / Single resource provider rule | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The core rejects unsupported distributed mutation and explains the single-provider MVP boundary. | Planned | Not run |
| ECRF-T-297 | ECRF-LAB-050 / Single resource provider rule | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The core rejects unsupported distributed mutation and explains the single-provider MVP boundary. | Planned | Not run |
| ECRF-T-298 | ECRF-LAB-050 / Single resource provider rule | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The core rejects unsupported distributed mutation and explains the single-provider MVP boundary. | Planned | Not run |
| ECRF-T-299 | ECRF-LAB-050 / Single resource provider rule | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The core rejects unsupported distributed mutation and explains the single-provider MVP boundary. | Planned | Not run |
| ECRF-T-300 | ECRF-LAB-050 / Single resource provider rule | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The core rejects unsupported distributed mutation and explains the single-provider MVP boundary. | Planned | Not run |
| ECRF-T-301 | ECRF-LAB-051 / Cross-provider read-only requirement | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: All requirements may participate without weakening resource atomicity. | Planned | Not run |
| ECRF-T-302 | ECRF-LAB-051 / Cross-provider read-only requirement | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: All requirements may participate without weakening resource atomicity. | Planned | Not run |
| ECRF-T-303 | ECRF-LAB-051 / Cross-provider read-only requirement | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: All requirements may participate without weakening resource atomicity. | Planned | Not run |
| ECRF-T-304 | ECRF-LAB-051 / Cross-provider read-only requirement | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: All requirements may participate without weakening resource atomicity. | Planned | Not run |
| ECRF-T-305 | ECRF-LAB-051 / Cross-provider read-only requirement | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: All requirements may participate without weakening resource atomicity. | Planned | Not run |
| ECRF-T-306 | ECRF-LAB-051 / Cross-provider read-only requirement | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: All requirements may participate without weakening resource atomicity. | Planned | Not run |
| ECRF-T-307 | ECRF-LAB-052 / Provider unregister during evaluation | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The request uses its captured provider generation or fails Unavailable without undefined behavior. | Planned | Not run |
| ECRF-T-308 | ECRF-LAB-052 / Provider unregister during evaluation | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The request uses its captured provider generation or fails Unavailable without undefined behavior. | Planned | Not run |
| ECRF-T-309 | ECRF-LAB-052 / Provider unregister during evaluation | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The request uses its captured provider generation or fails Unavailable without undefined behavior. | Planned | Not run |
| ECRF-T-310 | ECRF-LAB-052 / Provider unregister during evaluation | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The request uses its captured provider generation or fails Unavailable without undefined behavior. | Planned | Not run |
| ECRF-T-311 | ECRF-LAB-052 / Provider unregister during evaluation | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The request uses its captured provider generation or fails Unavailable without undefined behavior. | Planned | Not run |
| ECRF-T-312 | ECRF-LAB-052 / Provider unregister during evaluation | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The request uses its captured provider generation or fails Unavailable without undefined behavior. | Planned | Not run |
| ECRF-T-313 | ECRF-LAB-053 / Resource provider unregister with reservation | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Unregistration is blocked or deferred until the reservation closes. | Planned | Not run |
| ECRF-T-314 | ECRF-LAB-053 / Resource provider unregister with reservation | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Unregistration is blocked or deferred until the reservation closes. | Planned | Not run |
| ECRF-T-315 | ECRF-LAB-053 / Resource provider unregister with reservation | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Unregistration is blocked or deferred until the reservation closes. | Planned | Not run |
| ECRF-T-316 | ECRF-LAB-053 / Resource provider unregister with reservation | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Unregistration is blocked or deferred until the reservation closes. | Planned | Not run |
| ECRF-T-317 | ECRF-LAB-053 / Resource provider unregister with reservation | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Unregistration is blocked or deferred until the reservation closes. | Planned | Not run |
| ECRF-T-318 | ECRF-LAB-053 / Resource provider unregister with reservation | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Unregistration is blocked or deferred until the reservation closes. | Planned | Not run |
| ECRF-T-319 | ECRF-LAB-054 / Timed recipe preview | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The core reports the timing policy but execution returns DeferredCapability in the MVP. | Planned | Not run |
| ECRF-T-320 | ECRF-LAB-054 / Timed recipe preview | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The core reports the timing policy but execution returns DeferredCapability in the MVP. | Planned | Not run |
| ECRF-T-321 | ECRF-LAB-054 / Timed recipe preview | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The core reports the timing policy but execution returns DeferredCapability in the MVP. | Planned | Not run |
| ECRF-T-322 | ECRF-LAB-054 / Timed recipe preview | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The core reports the timing policy but execution returns DeferredCapability in the MVP. | Planned | Not run |
| ECRF-T-323 | ECRF-LAB-054 / Timed recipe preview | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The core reports the timing policy but execution returns DeferredCapability in the MVP. | Planned | Not run |
| ECRF-T-324 | ECRF-LAB-054 / Timed recipe preview | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The core reports the timing policy but execution returns DeferredCapability in the MVP. | Planned | Not run |
| ECRF-T-325 | ECRF-LAB-055 / Queue request in MVP | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The request returns Unavailable with an explicit capability diagnostic. | Planned | Not run |
| ECRF-T-326 | ECRF-LAB-055 / Queue request in MVP | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The request returns Unavailable with an explicit capability diagnostic. | Planned | Not run |
| ECRF-T-327 | ECRF-LAB-055 / Queue request in MVP | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The request returns Unavailable with an explicit capability diagnostic. | Planned | Not run |
| ECRF-T-328 | ECRF-LAB-055 / Queue request in MVP | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The request returns Unavailable with an explicit capability diagnostic. | Planned | Not run |
| ECRF-T-329 | ECRF-LAB-055 / Queue request in MVP | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The request returns Unavailable with an explicit capability diagnostic. | Planned | Not run |
| ECRF-T-330 | ECRF-LAB-055 / Queue request in MVP | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The request returns Unavailable with an explicit capability diagnostic. | Planned | Not run |
| ECRF-T-331 | ECRF-LAB-056 / Simulated timed reservation lifecycle | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Reservation, running, completion, and release states follow the documented later-module contract without claiming production evidence. | Planned | Not run |
| ECRF-T-332 | ECRF-LAB-056 / Simulated timed reservation lifecycle | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Reservation, running, completion, and release states follow the documented later-module contract without claiming production evidence. | Planned | Not run |
| ECRF-T-333 | ECRF-LAB-056 / Simulated timed reservation lifecycle | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Reservation, running, completion, and release states follow the documented later-module contract without claiming production evidence. | Planned | Not run |
| ECRF-T-334 | ECRF-LAB-056 / Simulated timed reservation lifecycle | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Reservation, running, completion, and release states follow the documented later-module contract without claiming production evidence. | Planned | Not run |
| ECRF-T-335 | ECRF-LAB-056 / Simulated timed reservation lifecycle | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Reservation, running, completion, and release states follow the documented later-module contract without claiming production evidence. | Planned | Not run |
| ECRF-T-336 | ECRF-LAB-056 / Simulated timed reservation lifecycle | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Reservation, running, completion, and release states follow the documented later-module contract without claiming production evidence. | Planned | Not run |
| ECRF-T-337 | ECRF-LAB-057 / Timed cancellation before commit | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The reservation releases according to policy and no output commits. | Planned | Not run |
| ECRF-T-338 | ECRF-LAB-057 / Timed cancellation before commit | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The reservation releases according to policy and no output commits. | Planned | Not run |
| ECRF-T-339 | ECRF-LAB-057 / Timed cancellation before commit | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The reservation releases according to policy and no output commits. | Planned | Not run |
| ECRF-T-340 | ECRF-LAB-057 / Timed cancellation before commit | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The reservation releases according to policy and no output commits. | Planned | Not run |
| ECRF-T-341 | ECRF-LAB-057 / Timed cancellation before commit | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The reservation releases according to policy and no output commits. | Planned | Not run |
| ECRF-T-342 | ECRF-LAB-057 / Timed cancellation before commit | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The reservation releases according to policy and no output commits. | Planned | Not run |
| ECRF-T-343 | ECRF-LAB-058 / Queue does not reserve early | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Only the active job holds a reservation; queued jobs revalidate when promoted. | Planned | Not run |
| ECRF-T-344 | ECRF-LAB-058 / Queue does not reserve early | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Only the active job holds a reservation; queued jobs revalidate when promoted. | Planned | Not run |
| ECRF-T-345 | ECRF-LAB-058 / Queue does not reserve early | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Only the active job holds a reservation; queued jobs revalidate when promoted. | Planned | Not run |
| ECRF-T-346 | ECRF-LAB-058 / Queue does not reserve early | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Only the active job holds a reservation; queued jobs revalidate when promoted. | Planned | Not run |
| ECRF-T-347 | ECRF-LAB-058 / Queue does not reserve early | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Only the active job holds a reservation; queued jobs revalidate when promoted. | Planned | Not run |
| ECRF-T-348 | ECRF-LAB-058 / Queue does not reserve early | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Only the active job holds a reservation; queued jobs revalidate when promoted. | Planned | Not run |
| ECRF-T-349 | ECRF-LAB-059 / Station queue capacity | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Further enqueue requests are rejected predictably. | Planned | Not run |
| ECRF-T-350 | ECRF-LAB-059 / Station queue capacity | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Further enqueue requests are rejected predictably. | Planned | Not run |
| ECRF-T-351 | ECRF-LAB-059 / Station queue capacity | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Further enqueue requests are rejected predictably. | Planned | Not run |
| ECRF-T-352 | ECRF-LAB-059 / Station queue capacity | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Further enqueue requests are rejected predictably. | Planned | Not run |
| ECRF-T-353 | ECRF-LAB-059 / Station queue capacity | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Further enqueue requests are rejected predictably. | Planned | Not run |
| ECRF-T-354 | ECRF-LAB-059 / Station queue capacity | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Further enqueue requests are rejected predictably. | Planned | Not run |
| ECRF-T-355 | ECRF-LAB-060 / Quality provider absent | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The policy follows its authored unavailable behavior and does not fabricate quality. | Planned | Not run |
| ECRF-T-356 | ECRF-LAB-060 / Quality provider absent | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The policy follows its authored unavailable behavior and does not fabricate quality. | Planned | Not run |
| ECRF-T-357 | ECRF-LAB-060 / Quality provider absent | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The policy follows its authored unavailable behavior and does not fabricate quality. | Planned | Not run |
| ECRF-T-358 | ECRF-LAB-060 / Quality provider absent | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The policy follows its authored unavailable behavior and does not fabricate quality. | Planned | Not run |
| ECRF-T-359 | ECRF-LAB-060 / Quality provider absent | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The policy follows its authored unavailable behavior and does not fabricate quality. | Planned | Not run |
| ECRF-T-360 | ECRF-LAB-060 / Quality provider absent | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The policy follows its authored unavailable behavior and does not fabricate quality. | Planned | Not run |
| ECRF-T-361 | ECRF-LAB-061 / Quality provider deterministic result | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The same canonical context produces the same quality result. | Planned | Not run |
| ECRF-T-362 | ECRF-LAB-061 / Quality provider deterministic result | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The same canonical context produces the same quality result. | Planned | Not run |
| ECRF-T-363 | ECRF-LAB-061 / Quality provider deterministic result | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The same canonical context produces the same quality result. | Planned | Not run |
| ECRF-T-364 | ECRF-LAB-061 / Quality provider deterministic result | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The same canonical context produces the same quality result. | Planned | Not run |
| ECRF-T-365 | ECRF-LAB-061 / Quality provider deterministic result | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The same canonical context produces the same quality result. | Planned | Not run |
| ECRF-T-366 | ECRF-LAB-061 / Quality provider deterministic result | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The same canonical context produces the same quality result. | Planned | Not run |
| ECRF-T-367 | ECRF-LAB-062 / Failure provider absent | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The policy follows its authored unavailable behavior; the core does not roll random success itself. | Planned | Not run |
| ECRF-T-368 | ECRF-LAB-062 / Failure provider absent | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The policy follows its authored unavailable behavior; the core does not roll random success itself. | Planned | Not run |
| ECRF-T-369 | ECRF-LAB-062 / Failure provider absent | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The policy follows its authored unavailable behavior; the core does not roll random success itself. | Planned | Not run |
| ECRF-T-370 | ECRF-LAB-062 / Failure provider absent | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The policy follows its authored unavailable behavior; the core does not roll random success itself. | Planned | Not run |
| ECRF-T-371 | ECRF-LAB-062 / Failure provider absent | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The policy follows its authored unavailable behavior; the core does not roll random success itself. | Planned | Not run |
| ECRF-T-372 | ECRF-LAB-062 / Failure provider absent | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The policy follows its authored unavailable behavior; the core does not roll random success itself. | Planned | Not run |
| ECRF-T-373 | ECRF-LAB-063 / Failure before resource commit | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The craft fails without consuming or granting resources. | Planned | Not run |
| ECRF-T-374 | ECRF-LAB-063 / Failure before resource commit | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The craft fails without consuming or granting resources. | Planned | Not run |
| ECRF-T-375 | ECRF-LAB-063 / Failure before resource commit | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The craft fails without consuming or granting resources. | Planned | Not run |
| ECRF-T-376 | ECRF-LAB-063 / Failure before resource commit | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The craft fails without consuming or granting resources. | Planned | Not run |
| ECRF-T-377 | ECRF-LAB-063 / Failure before resource commit | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The craft fails without consuming or granting resources. | Planned | Not run |
| ECRF-T-378 | ECRF-LAB-063 / Failure before resource commit | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The craft fails without consuming or granting resources. | Planned | Not run |
| ECRF-T-379 | ECRF-LAB-064 / Salvage transformation preview | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Inputs and outputs can be represented without EchoCrafting owning item data. | Planned | Not run |
| ECRF-T-380 | ECRF-LAB-064 / Salvage transformation preview | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Inputs and outputs can be represented without EchoCrafting owning item data. | Planned | Not run |
| ECRF-T-381 | ECRF-LAB-064 / Salvage transformation preview | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Inputs and outputs can be represented without EchoCrafting owning item data. | Planned | Not run |
| ECRF-T-382 | ECRF-LAB-064 / Salvage transformation preview | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Inputs and outputs can be represented without EchoCrafting owning item data. | Planned | Not run |
| ECRF-T-383 | ECRF-LAB-064 / Salvage transformation preview | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Inputs and outputs can be represented without EchoCrafting owning item data. | Planned | Not run |
| ECRF-T-384 | ECRF-LAB-064 / Salvage transformation preview | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Inputs and outputs can be represented without EchoCrafting owning item data. | Planned | Not run |
| ECRF-T-385 | ECRF-LAB-065 / Repair mutation capability unavailable | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The request returns Unavailable and preserves the item state. | Planned | Not run |
| ECRF-T-386 | ECRF-LAB-065 / Repair mutation capability unavailable | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The request returns Unavailable and preserves the item state. | Planned | Not run |
| ECRF-T-387 | ECRF-LAB-065 / Repair mutation capability unavailable | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The request returns Unavailable and preserves the item state. | Planned | Not run |
| ECRF-T-388 | ECRF-LAB-065 / Repair mutation capability unavailable | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The request returns Unavailable and preserves the item state. | Planned | Not run |
| ECRF-T-389 | ECRF-LAB-065 / Repair mutation capability unavailable | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The request returns Unavailable and preserves the item state. | Planned | Not run |
| ECRF-T-390 | ECRF-LAB-065 / Repair mutation capability unavailable | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The request returns Unavailable and preserves the item state. | Planned | Not run |
| ECRF-T-391 | ECRF-LAB-066 / Upgrade mutation capability unavailable | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The request is rejected without consuming ingredients. | Planned | Not run |
| ECRF-T-392 | ECRF-LAB-066 / Upgrade mutation capability unavailable | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The request is rejected without consuming ingredients. | Planned | Not run |
| ECRF-T-393 | ECRF-LAB-066 / Upgrade mutation capability unavailable | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The request is rejected without consuming ingredients. | Planned | Not run |
| ECRF-T-394 | ECRF-LAB-066 / Upgrade mutation capability unavailable | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The request is rejected without consuming ingredients. | Planned | Not run |
| ECRF-T-395 | ECRF-LAB-066 / Upgrade mutation capability unavailable | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The request is rejected without consuming ingredients. | Planned | Not run |
| ECRF-T-396 | ECRF-LAB-066 / Upgrade mutation capability unavailable | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The request is rejected without consuming ingredients. | Planned | Not run |
| ECRF-T-397 | ECRF-LAB-067 / Export knowledge state | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: A detached versioned snapshot contains stable IDs and no live Unity objects. | Planned | Not run |
| ECRF-T-398 | ECRF-LAB-067 / Export knowledge state | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: A detached versioned snapshot contains stable IDs and no live Unity objects. | Planned | Not run |
| ECRF-T-399 | ECRF-LAB-067 / Export knowledge state | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: A detached versioned snapshot contains stable IDs and no live Unity objects. | Planned | Not run |
| ECRF-T-400 | ECRF-LAB-067 / Export knowledge state | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: A detached versioned snapshot contains stable IDs and no live Unity objects. | Planned | Not run |
| ECRF-T-401 | ECRF-LAB-067 / Export knowledge state | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: A detached versioned snapshot contains stable IDs and no live Unity objects. | Planned | Not run |
| ECRF-T-402 | ECRF-LAB-067 / Export knowledge state | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: A detached versioned snapshot contains stable IDs and no live Unity objects. | Planned | Not run |
| ECRF-T-403 | ECRF-LAB-068 / Import known recipe state | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Known recipes restore atomically and one post-commit event is raised. | Planned | Not run |
| ECRF-T-404 | ECRF-LAB-068 / Import known recipe state | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Known recipes restore atomically and one post-commit event is raised. | Planned | Not run |
| ECRF-T-405 | ECRF-LAB-068 / Import known recipe state | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Known recipes restore atomically and one post-commit event is raised. | Planned | Not run |
| ECRF-T-406 | ECRF-LAB-068 / Import known recipe state | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Known recipes restore atomically and one post-commit event is raised. | Planned | Not run |
| ECRF-T-407 | ECRF-LAB-068 / Import known recipe state | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Known recipes restore atomically and one post-commit event is raised. | Planned | Not run |
| ECRF-T-408 | ECRF-LAB-068 / Import known recipe state | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Known recipes restore atomically and one post-commit event is raised. | Planned | Not run |
| ECRF-T-409 | ECRF-LAB-069 / Import unknown recipe ID | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The unknown record is preserved as orphaned data rather than deleted. | Planned | Not run |
| ECRF-T-410 | ECRF-LAB-069 / Import unknown recipe ID | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The unknown record is preserved as orphaned data rather than deleted. | Planned | Not run |
| ECRF-T-411 | ECRF-LAB-069 / Import unknown recipe ID | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The unknown record is preserved as orphaned data rather than deleted. | Planned | Not run |
| ECRF-T-412 | ECRF-LAB-069 / Import unknown recipe ID | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The unknown record is preserved as orphaned data rather than deleted. | Planned | Not run |
| ECRF-T-413 | ECRF-LAB-069 / Import unknown recipe ID | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The unknown record is preserved as orphaned data rather than deleted. | Planned | Not run |
| ECRF-T-414 | ECRF-LAB-069 / Import unknown recipe ID | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The unknown record is preserved as orphaned data rather than deleted. | Planned | Not run |
| ECRF-T-415 | ECRF-LAB-070 / Alias migration | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Migration resolves the canonical ID and records the migration result. | Planned | Not run |
| ECRF-T-416 | ECRF-LAB-070 / Alias migration | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Migration resolves the canonical ID and records the migration result. | Planned | Not run |
| ECRF-T-417 | ECRF-LAB-070 / Alias migration | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Migration resolves the canonical ID and records the migration result. | Planned | Not run |
| ECRF-T-418 | ECRF-LAB-070 / Alias migration | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Migration resolves the canonical ID and records the migration result. | Planned | Not run |
| ECRF-T-419 | ECRF-LAB-070 / Alias migration | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Migration resolves the canonical ID and records the migration result. | Planned | Not run |
| ECRF-T-420 | ECRF-LAB-070 / Alias migration | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Migration resolves the canonical ID and records the migration result. | Planned | Not run |
| ECRF-T-421 | ECRF-LAB-071 / Active reservation save boundary | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The snapshot reports an unsafe point or excludes the live reservation according to policy. | Planned | Not run |
| ECRF-T-422 | ECRF-LAB-071 / Active reservation save boundary | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The snapshot reports an unsafe point or excludes the live reservation according to policy. | Planned | Not run |
| ECRF-T-423 | ECRF-LAB-071 / Active reservation save boundary | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The snapshot reports an unsafe point or excludes the live reservation according to policy. | Planned | Not run |
| ECRF-T-424 | ECRF-LAB-071 / Active reservation save boundary | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The snapshot reports an unsafe point or excludes the live reservation according to policy. | Planned | Not run |
| ECRF-T-425 | ECRF-LAB-071 / Active reservation save boundary | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The snapshot reports an unsafe point or excludes the live reservation according to policy. | Planned | Not run |
| ECRF-T-426 | ECRF-LAB-071 / Active reservation save boundary | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The snapshot reports an unsafe point or excludes the live reservation according to policy. | Planned | Not run |
| ECRF-T-427 | ECRF-LAB-072 / Direct-scene initialization | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: The configured development initializer creates one root only when absent. | Planned | Not run |
| ECRF-T-428 | ECRF-LAB-072 / Direct-scene initialization | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: The configured development initializer creates one root only when absent. | Planned | Not run |
| ECRF-T-429 | ECRF-LAB-072 / Direct-scene initialization | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: The configured development initializer creates one root only when absent. | Planned | Not run |
| ECRF-T-430 | ECRF-LAB-072 / Direct-scene initialization | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: The configured development initializer creates one root only when absent. | Planned | Not run |
| ECRF-T-431 | ECRF-LAB-072 / Direct-scene initialization | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: The configured development initializer creates one root only when absent. | Planned | Not run |
| ECRF-T-432 | ECRF-LAB-072 / Direct-scene initialization | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: The configured development initializer creates one root only when absent. | Planned | Not run |
| ECRF-T-433 | ECRF-LAB-073 / Scene transition with root persistence | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Catalog, providers, knowledge, and bounded idempotency history survive without duplicate roots. | Planned | Not run |
| ECRF-T-434 | ECRF-LAB-073 / Scene transition with root persistence | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Catalog, providers, knowledge, and bounded idempotency history survive without duplicate roots. | Planned | Not run |
| ECRF-T-435 | ECRF-LAB-073 / Scene transition with root persistence | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Catalog, providers, knowledge, and bounded idempotency history survive without duplicate roots. | Planned | Not run |
| ECRF-T-436 | ECRF-LAB-073 / Scene transition with root persistence | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Catalog, providers, knowledge, and bounded idempotency history survive without duplicate roots. | Planned | Not run |
| ECRF-T-437 | ECRF-LAB-073 / Scene transition with root persistence | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Catalog, providers, knowledge, and bounded idempotency history survive without duplicate roots. | Planned | Not run |
| ECRF-T-438 | ECRF-LAB-073 / Scene transition with root persistence | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Catalog, providers, knowledge, and bounded idempotency history survive without duplicate roots. | Planned | Not run |
| ECRF-T-439 | ECRF-LAB-074 / Reset and reload Laboratory | EditMode definition/validation | Validate definitions, IDs, policies, canonicalization, or pure decision logic. Expected: Reservations, providers, station handles, knowledge fixtures, subscriptions, and diagnostics return to the known baseline. | Planned | Not run |
| ECRF-T-440 | ECRF-LAB-074 / Reset and reload Laboratory | EditMode provider/transaction | Validate provider contracts, reservations, revisions, idempotency, rollback, or migration in isolation. Expected: Reservations, providers, station handles, knowledge fixtures, subscriptions, and diagnostics return to the known baseline. | Planned | Not run |
| ECRF-T-441 | ECRF-LAB-074 / Reset and reload Laboratory | PlayMode lifecycle | Validate root, service, handles, events, cancellation, scene, or shutdown behavior. Expected: Reservations, providers, station handles, knowledge fixtures, subscriptions, and diagnostics return to the known baseline. | Planned | Not run |
| ECRF-T-442 | ECRF-LAB-074 / Reset and reload Laboratory | Standalone Laboratory | Execute the matching visible Laboratory workflow and capture evidence. Expected: Reservations, providers, station handles, knowledge fixtures, subscriptions, and diagnostics return to the known baseline. | Planned | Not run |
| ECRF-T-443 | ECRF-LAB-074 / Reset and reload Laboratory | Clean-project/package | Repeat the requirement in a clean supported project and declared installation route. Expected: Reservations, providers, station handles, knowledge fixtures, subscriptions, and diagnostics return to the known baseline. | Planned | Not run |
| ECRF-T-444 | ECRF-LAB-074 / Reset and reload Laboratory | Regression/release | Retain a package-qualified regression case and release-gate reference. Expected: Reservations, providers, station handles, knowledge fixtures, subscriptions, and diagnostics return to the known baseline. | Planned | Not run |

All test states are `Not run`. The registry defines intended proof; it is not execution evidence.

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Workshop prerequisite completed.
- [x] Ownership/non-ownership approved.
- [x] MVP and later modules separated.
- [x] Provider atomicity boundary explicit.
- [x] Simple Combine preserved without genre lock.
- [x] Data, IDs, lifecycle, API, Laboratory, diagnostics, persistence, bridges, and risks specified.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared dependencies only.
- [ ] Editor code isolated.
- [ ] Provider contract and one simulated provider implemented.
- [ ] Exact and standard immediate recipes implemented.
- [ ] Duplicate, cancellation, revision, idempotency, and shutdown tests pass.
- [ ] Public API matches specification or authority is updated first.

### 24.3 Standalone gate

- [ ] Clean installation succeeds.
- [ ] Laboratory passes without peer packages.
- [ ] Samples remove cleanly.
- [ ] Direct-scene behavior works.
- [ ] Simulated provider proves atomic success and safe failure.

### 24.4 Quality gate

- [ ] Automated tests pass.
- [ ] Manual checklist passes.
- [ ] No Blocker/Critical defect remains.
- [ ] Measured budgets pass.
- [ ] Diagnostics are actionable.
- [ ] Docs match implementation.
- [ ] Current Notes reconciled.
- [ ] Licenses/notices complete.

### 24.5 Distribution gate

- [ ] Manifest/version/changelog valid.
- [ ] Stable `.meta` files included.
- [ ] Git and tarball installation tested externally.
- [ ] Removal/re-add tested.
- [ ] Beta, RC, and stable gates pass under SFGSS-004.
- [ ] Compatibility catalog updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing mechanic | Replacement | Parity gate | Rollback |
|---|---|---|---|---|
| Hackulos | Exact quest-combine bag | Simple Combine recipe plus Vault/project provider and project UI | Exact ingredients, invalid combination, atomic output, quest event | Keep original quest script until parity |
| Future RPG | Project crafting logic | Standard provider-neutral immediate recipes | Preview/transaction/provider parity | Feature flag and original system |
| Puzzle/jam prototype | Token conversion | Custom resource provider | Small exact recipe works without RPG packages | Remove package/provider |

### 25.2 Preserve-until-parity rule

Existing working crafting remains intact until standalone Laboratory and target-project parity pass. Migration is incremental and reversible.

### 25.3 Migration tooling

Planned tooling detects legacy recipe IDs, previews aliases, validates signatures, preserves backups, imports detached knowledge, and reports unsupported runtime state. It never deletes project crafting code automatically.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| ECRF-R-001 | Scope inflation into MMO crafting | High | High | Strict MVP/later modules | Spec review |
| ECRF-R-002 | Hidden Inventory dependency | Medium | High | Simulated provider standalone gate | Package owner |
| ECRF-R-003 | Inputs consumed without outputs | Medium | Critical | One-provider atomic plan/reservation | Transaction tests |
| ECRF-R-004 | Duplicate output grants | Medium | Critical | Request IDs/idempotency | Regression tests |
| ECRF-R-005 | Ambiguous recipe match | Medium | Major | Canonical signatures/validation | Editor validator |
| ECRF-R-006 | Mutable ScriptableObject state | Medium | Major | Runtime state ownership tests | Data audit |
| ECRF-R-007 | Stale UI preview | High | Major | Provider revisions/fingerprint | Execute tests |
| ECRF-R-008 | Queue hoards resources | Medium | Major | Reserve only on activation | Timed module gate |
| ECRF-R-009 | Repair corrupts unique items | Medium | Critical | Deferred mutation module/provider | Future spec |
| ECRF-R-010 | Multiplayer trusts client | Medium | Critical | Authoritative bridge/security tests | Convergence review |
| ECRF-R-011 | Unknown recipe state deleted | Low | Major | Orphan preservation/migrations | Import tests |
| ECRF-R-012 | UI becomes runtime authority | Medium | Major | Snapshot/command boundary | Bridge review |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequence | ADR? |
|---|---|---|---|---|---:|
| ECRF-D-001 | One-provider atomic mutation per MVP craft | Approved | Honest atomicity boundary | Multi-provider mutation deferred | No |
| ECRF-D-002 | Simple Combine is a constrained recipe profile | Approved | Preserves small use case without duplicate authority | Same transaction engine |
| ECRF-D-003 | Preview is side-effect-free and revision-aware | Approved | Prevent stale/destructive UI logic | Revalidation required |
| ECRF-D-004 | Recipe knowledge is crafting-specific state | Approved | Clear ownership | Discovery triggers remain external |
| ECRF-D-005 | Timed jobs/queues are approved later modules | Approved | Prevent MVP inflation | Seams defined now |
| ECRF-D-006 | Quality/failure are provider-driven later | Approved | Genre neutrality | No core random roll |
| ECRF-D-007 | Repair/upgrade require unique-item mutation module | Approved | Stack transaction insufficient | Deferred |
| ECRF-D-008 | Live reservations are not saved in MVP | Approved | Cannot safely reconstruct arbitrary provider state | Immediate MVP only |
| ECRF-D-009 | Root owns session registry/knowledge, not resources | Approved | One authority per concern | Providers retain resource truth |
| ECRF-D-010 | Exact quest bag maps to project UI + provider context | Approved | UI/inventory boundaries preserved | No bag-specific core class required |

### 27.2 Release-blocking questions

None remain for specification approval. Implementation must still select exact serializer/backend details and verify Unity/package compatibility through evidence.

### 27.3 Non-blocking later questions

- Whether timed crafting ships inside the core package or a separate module.
- Whether recipe knowledge storage defaults to package state or Ascent-backed projects in Workshop presets.
- Exact quality/failure provider schemas.
- Durable timed-job reconstruction and offline trust policy.
- Multi-provider coordinator feasibility.
- Repair/upgrade transaction format for unique item state.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included | Evidence |
|---|---|---|---|
| M0 | Approved contract | Workshop + spec | Approved documents |
| M1 | Package skeleton | Manifest, asmdefs, docs shell | Clean compile |
| M2 | Definitions/validation | IDs, recipes, catalogs, signatures | EditMode tests |
| M3 | Preview/providers | Context, requirements, simulated provider | Unit tests |
| M4 | Immediate transaction | Reservation, atomic commit, idempotency | PlayMode tests |
| M5 | Knowledge/state | Discovery, export/import, migration | Tests |
| M6 | Laboratory/tooling | Setup, authoring, validator, sample | Lab checklist |
| M7 | Vault bridge/adoption | Separate bridge and Hackulos parity | Integration evidence |
| M8 | Release | Docs, packaging, external install | Release gates |

### 28.2 Checkpoint rule

Every implementation checkpoint follows SFGSS-005, displays complete code in the conversation, explains each file and decision, includes exact Editor steps, stops at a proof boundary, and reconciles documentation.

### 28.3 First recommended checkpoint

After SUITE-DOC-33 unlocks implementation: **ECRF-M1-01 - EchoCrafting Package Skeleton**, creating only package anatomy, manifests, asmdefs, and documentation shell before runtime code.

---

## 29. New-Conversation Handoff

```text
We are continuing documentation-first development of The Sperk's Forge.
Treat SFGSS-000 and SFGSS-002 through SFGSS-005 as suite authorities.
Treat The Crucible (`EchoCrafting`) Package Specification v1.0.0 and the
SUITE-DOC-17 crafting design workshop record as the authority for crafting.
Package implementation remains locked until SUITE-DOC-33.

Current package: EchoCrafting
Current specification: v1.0.0 Approved
Current checkpoint: SUITE-DOC-18 - EchoMultiplayer / The Convergence foundation
Implementation status: Not started
Evidence status: Not run

Preserve the one-provider atomic transaction boundary, Simple Combine path,
side-effect-free preview, recipe knowledge ownership, provider-neutral core,
and explicit later seams. Do not introduce an Inventory dependency or claim
multi-provider, timed, quality, failure, repair, offline, or multiplayer evidence.
When implementation is eventually authorized, show complete code and explain
each file and step so Jesse can enter and understand it himself.
```

### 29.1 Current status record

| Field | Value |
|---|---|
| Package version | 1.0.0 Approved |
| Completed checkpoint | SUITE-DOC-17 |
| Files/assets created | Documentation only |
| Tests passed | None; planned only |
| Tests failed | None; not run |
| Known issues | Empirical evidence pending; later modules deferred |
| Decisions added | ECRF-D-001 through ECRF-D-010 |
| Next checkpoint | SUITE-DOC-18 - The Convergence |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Workshop prerequisite complete.
- [x] Identity and responsibility clear.
- [x] Ownership and non-ownership align with SFGSS-000.
- [x] Core remains independent from The Vault and peers.
- [x] Simple Combine and standard immediate MVP are complete and bounded.
- [x] Data, IDs, lifecycle, API, failure, diagnostics, setup, Laboratory, persistence, and bridges specified.
- [x] Timed, queue, quality, failure, salvage, repair, upgrade, offline, and multiplayer seams are explicit without false implementation claims.
- [x] Tests and release gates are measurable and `Not run`.
- [x] No Isekai Studios ownership or technical identity introduced.
- [x] Jesse approved the documentation-first package direction.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Implementation remains locked until SUITE-DOC-33. Timed crafting, queues, quality, failure, repair, upgrade, offline production, distributed transactions, and multiplayer adapters require later checkpoints and evidence.

---

## Template Completion Rule

A new collaborator can identify the package authority, non-goals, MVP, standalone behavior, data/state separation, public API, provider transaction boundary, failure behavior, setup, Laboratory, bridges, later modules, and release evidence without consulting chat history. The specification is therefore complete as an approved pre-code authority.


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
