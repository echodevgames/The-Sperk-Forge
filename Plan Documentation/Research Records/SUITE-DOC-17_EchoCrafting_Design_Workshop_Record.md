# SUITE-DOC-17 - EchoCrafting Design Workshop Record

**Document role:** Required pre-specification design workshop and durable research/decision record  
**Status:** Approved workshop conclusions  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Package:** The Crucible (`EchoCrafting`)  
**Parent authorities:** SFGSS-000 v0.12.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0  
**Date:** August 4, 2026

> The first recipe must be small enough to prove. The architecture must be large enough not to regret.

---

## 1. Workshop purpose

SFGSS-000 deliberately deferred EchoCrafting until recipes, skills/professions, discovery, quality, stations, timing, queues, failure, repair, salvage, persistence, UI, and multiplayer authority could be considered together. This record satisfies that prerequisite before the package specification is approved.

The workshop preserves Hackulos's authored quest-combine bag as a first-class use case while preventing that mechanic from becoming the permanent architecture for every workstation, trade skill, repair bench, production queue, or multiplayer transaction.

No implementation, prototype, performance measurement, or provider compatibility test occurred during this workshop. All empirical claims remain `Not run`.

## 2. Problem framing

Crafting is not one operation. It is a family of related transformations:

- Exact authored combinations.
- Flexible recipes with alternatives and tags.
- Immediate transformations.
- Timed station work.
- Queued production.
- Recipe knowledge and discovery.
- Tools, stations, professions, and contextual requirements.
- Quality or failure calculation.
- Salvage, repair, and upgrades.
- Input consumption and output delivery.
- UI previews and progress presentation.
- Persistence and multiplayer validation.

A package that begins with a giant MMO profession model becomes unusable for a game jam. A package that begins with `if bag contains rat tail and human eye` cannot grow into a safe reusable system. The workshop therefore separates the **neutral transaction spine** from optional capability modules.

## 3. Workshop conclusions

### 3.1 Core ownership

**Decision:** EchoCrafting owns recipe-driven transformation truth: recipe definitions, matching, preview, requirement evaluation, provider-neutral resource plans, one-provider atomic execution, request idempotency, recipe knowledge, diagnostics, authoring, validation, and explicit extension seams.

It does not own item definitions, inventory storage, player skills, progression, production UI, audio, feedback, save files, networking authority, or the unique item-state rules behind repair and upgrades.

### 3.2 Simple combine versus standard crafting

**Decision:** The package exposes two authoring paths over the same transaction engine:

1. **Simple Combine** uses an exact or canonical authored input set and an immediate result. It is suitable for the Hackulos quest bag.
2. **Standard Recipe** uses explicit ingredient groups, alternatives, tags, quantities, tools, station requirements, output options, and project requirements.

Simple Combine is not a separate runtime authority. It is a constrained recipe profile with stricter matching and simpler presentation data.

### 3.3 Resource provider boundary

**Decision:** EchoCrafting does not require EchoInventory. The core talks to one mutation-capable `ICraftingResourceProvider` per MVP request.

The provider prepares an immutable resource plan, verifies expected revisions and capacity, creates a reversible reservation, and publishes consumption plus output grants atomically inside the provider's own authority.

Multiple read-only requirement providers may participate. Distributed mutation across several unrelated resource providers is not promised by the MVP because a general two-phase commit with crash recovery cannot be guaranteed across arbitrary game systems. A future coordinator may support providers that explicitly implement compatible prepare/commit/rollback contracts.

### 3.4 Ingredient language

**Decision:** Requirements can express:

- Exact definitions.
- Stable tags.
- One-of alternatives.
- Quantities.
- Consumable inputs.
- Non-consumable tools.
- Catalysts with authored consume policy.
- Exact-set matching for combine containers.
- Optional allowance or rejection of unrelated contents.

Provider ordering and canonicalization must make matching deterministic. Recipe definitions never hold live inventory state.

### 3.5 Stations and crafting context

**Decision:** Stations are runtime registrations backed by immutable station definitions and stale-safe handles. A station advertises stable capability tags and optional queue/capacity metadata.

Portable crafting and exact combine recipes may explicitly require no station. The Hackulos quest bag supplies a crafting context and resource source/destination; it does not need to masquerade as a full persistent workstation.

### 3.6 Skills, professions, tools, and project conditions

**Decision:** EchoCrafting owns requirement declarations and evaluation orchestration, not player skills or professions. Read-only requirement providers evaluate stable, typed requirement payloads against the current crafting context.

The Ascent, Fellowship, project RPG data, or future systems may provide values. Missing providers produce `Unavailable`, not silent success or failure.

### 3.7 Discovery and recipe knowledge

**Decision:** Crafting-specific knowledge is package truth. EchoCrafting may record whether a recipe is known, hidden, or discovered and export/import that state.

The event that grants knowledge remains project-owned or arrives through a bridge from The Ascent, The Path, Voices, an item, or a world interaction. Authored visibility policies include always visible, hidden until known, hidden until exact match, and provider-controlled.

### 3.8 Preview and execution

**Decision:** Preview is immutable and side-effect-free. It includes eligibility, missing requirements, selected resources, expected revisions, capacity, outputs, byproducts, timing metadata, and a canonical plan fingerprint.

Execution must revalidate or compare expected revisions. A stale preview cannot blindly consume changed resources.

### 3.9 Immediate transaction MVP

**Decision:** The MVP supports immediate, atomic provider transactions. Cancellation is honored before the declared provider commit point. After publication, cancellation returns `TooLate` and the committed result remains authoritative.

Every request carries a stable `CraftingRequestId`; bounded idempotency history prevents retries, repeated UI clicks, or reconnect logic from granting duplicate outputs.

### 3.10 Timing and queues

**Decision:** Timed crafting and station queues are approved later capabilities, not MVP requirements.

The planned contract is:

- Queued jobs do not reserve resources while merely waiting.
- A job revalidates when promoted to active.
- The active job creates a reservation.
- Inputs and outputs commit atomically on completion by default.
- Cancellation before commit releases the reservation.
- Queue capacity, active slots, clock choice, and pause policy are explicit.
- Offline progress is not implied.

The specification documents these seams now so the immediate core does not block them later.

### 3.11 Quality

**Decision:** Quality is provider-driven and deferred from the MVP. The core may carry a typed quality result and pass it into an output plan, but it does not invent stats, rarity, random affixes, or item mutation rules.

A quality provider must be deterministic for the same canonical context unless the project explicitly supplies an authoritative random seed and records it.

### 3.12 Failure

**Decision:** The MVP has deterministic validation failure but no random craft-failure mechanic.

A later failure provider may decide success, failure, alternate outputs, or input-loss policy before resource commit. EchoCrafting itself does not roll random success chances. Failure after an atomic provider commit is a provider defect or committed-with-diagnostic state, not permission to retry.

### 3.13 Salvage, repair, and upgrades

**Decision:** Salvage can often use the generic consume-and-output recipe model. Repair and upgrade generally mutate one unique item instance and therefore require a mutation-capable provider and a separate later module.

The MVP does not pretend generic stack transactions solve durability, affixes, sockets, item level, enchantments, or equipment effects.

### 3.14 Persistence

**Decision:** Recipe knowledge and bounded idempotency information may be exported as detached package state. Live providers, scene stations, active reservations, and session handles are never serialized.

MVP active crafting is immediate, so there is no durable queue. Later timed jobs may be persisted only when the job context and provider reservation can be reconstructed safely. Otherwise save is allowed only at declared safe points.

The Chronicle owns save-file transport.

### 3.15 UI and presentation

**Decision:** EchoCrafting publishes recipe-browser snapshots, previews, requirement lines, result records, and later job/queue snapshots. It does not own the production crafting screen, combine-bag layout, drag/drop behavior, animations, audio, or feedback.

The simple combine sample may include a removable uGUI presentation in Samples, but the runtime core remains nonvisual.

### 3.16 Multiplayer

**Decision:** The core is provider-neutral and local. A future Convergence bridge sends requests to the selected authoritative peer/server, revalidates resources and requirements there, assigns request IDs, and replicates results.

A local preview is never proof that a networked craft is authorized. Clients cannot be trusted to assert ingredient ownership, skill values, station access, quality, failure, or output grants.

## 4. Approved capability tiers

| Tier | Status | Scope |
|---|---|---|
| Simple Combine | MVP | Exact authored input sets, immediate atomic transformation, strict or permissive extra-content policy, combine-context sample |
| Standard Immediate Crafting | MVP | Ingredient groups, alternatives, tags, quantities, tools, station/context requirements, outputs/byproducts, preview, batches, idempotent execution |
| Recipe Knowledge | MVP | Visibility policies, discovery requests, detached state export/import |
| Timed Crafting | Approved later | Active reservations, clocks, progress, cancellation, completion |
| Station Queues | Approved later | Bounded queues, promotion revalidation, active slots, queue snapshots |
| Quality | Approved later | Provider-driven quality result and output interpretation |
| Failure | Approved later | Provider-driven deterministic/seeded resolution before commit |
| Salvage | Approved later | Generic transformations where provider semantics are sufficient |
| Repair/Upgrade | Deferred module | Unique-item mutation contracts, durability/upgrade provider, migration and rollback rules |
| Offline production | Deferred research | Durable time source, tamper policy, provider reconstruction, platform clock semantics |

## 5. MVP vertical slice

The first useful release proves:

1. Load one recipe catalog.
2. Register one simulated resource provider.
3. Preview an exact combine recipe.
4. Reject missing, extra, stale, or ambiguous inputs.
5. Commit one atomic immediate transformation.
6. Preview and execute one standard recipe with alternatives and a non-consumable tool.
7. Preserve resource state on preparation or cancellation failure.
8. Discover one recipe and export/import knowledge state.
9. Run the standalone Crucible Laboratory.
10. Demonstrate one separate Vault bridge design for item-backed crafting without making the bridge an MVP core dependency.

## 6. Hackulos quest-combine mapping

| Quest need | EchoCrafting mapping |
|---|---|
| Player receives a bag | The Vault/project owns the container and UI presentation |
| Rat tail plus human eye | Exact ingredient requirements using stable item definition IDs |
| Combine button | Project/Looking Glass submits a semantic immediate crafting request |
| Exact authored recipe | Simple Combine recipe profile with strict extra-content policy |
| Bag of goo output | Atomic output grant through the resource provider |
| Invalid combination feedback | Structured preview/result consumed by UI, Resonance, or Impact bridges |
| Quest advancement | The Path listens to successful semantic result through a bridge/project adapter |

The quest remains small. The package remains reusable.

## 7. Rejected or deferred alternatives

| Alternative | Disposition | Reason |
|---|---|---|
| EchoCrafting directly depends on EchoInventory | Rejected | Violates standalone-first and prevents token/grid/custom providers |
| Core mutates several arbitrary providers atomically | Rejected for MVP | Cannot guarantee distributed atomicity or recovery across unknown authorities |
| Recipes store live inventory/container references | Rejected | Breaks asset immutability, tests, saves, and scene independence |
| One giant crafting-manager ScriptableObject stores queue state | Rejected | Mixes definitions and mutable runtime truth |
| Random failure built into every recipe | Rejected | Not every game wants randomness; provider-owned later policy is cleaner |
| Quality as a mandatory enum | Rejected | Quality semantics are genre and item-model dependent |
| Repair and upgrade treated as simple stack recipes | Deferred | Unique item mutation needs explicit provider contracts |
| Offline queue completion in first release | Deferred | Requires trustworthy time, persistence, provider reconstruction, and platform policy |
| Combine bag becomes a separate package | Rejected | It is a presentation/context profile over the same transaction engine |

## 8. Workshop exit decision

**Decision:** The workshop prerequisite is satisfied. The attached `SFGSS-The-Crucible-EchoCrafting-Package-Specification.md` may be approved as the Level 2 authority, provided it preserves the conclusions above and all empirical evidence remains `Not run`.
