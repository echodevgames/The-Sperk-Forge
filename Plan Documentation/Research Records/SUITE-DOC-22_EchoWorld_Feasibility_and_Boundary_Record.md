# SUITE-DOC-22 - EchoWorld Feasibility and Boundary Record

**Status:** Approved  
**Date:** August 4, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Related authority:** `../Package Specifications/SFGSS-The-Atlas-EchoWorld-Package-Foundation.md`

## 1. Purpose

This record captures the feasibility reasoning used to approve The Atlas as a provider-neutral Advanced package foundation without approving scene loading, level generation, world simulation, map presentation, or provider-specific implementation.

## 2. Feasibility conclusion

A reusable world package is feasible when it owns semantic identity and topology rather than the physical world itself. The stable center is:

- Stable world, zone, location, connection, binding, marker, and provider IDs.
- Immutable hierarchy and travel graph definitions.
- One revisioned semantic current context.
- Travel availability and deterministic route-plan contracts.
- Scene-binding tokens without scene-loading execution.
- Entry/spawn marker registration and deterministic selection.
- Discovery, visitation, and fast-travel semantic policy.
- Provider-neutral map and world-state snapshots.
- Diagnostics, validation, migrations, and optional bridges.

The package becomes a god-framework if it also owns scene streaming, procedural generation, level geometry, navigation, characters, objectives, map UI, save files, or multiplayer transport.

## 3. Location-versus-scene conclusion

A `LocationId` is durable semantic identity. A Unity scene is one possible content container. The following must all remain valid:

- One scene contains several semantic locations.
- One location has several scene variants or platform-specific bindings.
- A location exists without a loaded scene.
- A scene is renamed without changing save identity.
- A direct-development scene establishes context through explicit binding.

A `SceneBindingId` and opaque reference token connect the two domains. Unity asset GUIDs remain Editor/source identity only.

## 4. Passage boundary

Atlas prepares a `WorldTravelPlan` containing origin, destination, ordered semantic legs, binding token, marker criteria, and fingerprint. Passage or project code executes the actual scene transition and reports success/failure. Atlas commits context only after the approved handoff succeeds.

This prevents two scene authorities and keeps Atlas usable with custom streaming or non-scene worlds.

## 5. Marker and character boundary

Atlas registers and selects marker snapshots. The Fellowship or project code decides whether to spawn, teleport, replace, possess, or ignore a character. Marker handles are scene/session state and are never saved.

## 6. Discovery, progression, and objectives

Discovery means the player/system knows a place exists. Visitation means committed presence was recorded. Progression access, quest completion, rewards, and level unlocks remain separate authorities.

Fast travel may evaluate read-only Progression, Objective, inventory, or project conditions, but it does not mutate those systems.

## 7. World-state boundary

Atlas owns core context, discovery, and visit records. Project-owned world facts use versioned participant records. Atlas routes and preserves those records but does not interpret a door, boss, harvested node, destructible wall, weather state, or NPC schedule.

Chronicle owns files, slots, backups, and recovery transport. Atlas exports/imports detached state only.

## 8. Map boundary

Atlas may expose a semantic `WorldMapSnapshot` containing stable nodes, edges, discovery, availability, and project-authored layout metadata. Looking Glass or project code renders the map. Atlas does not require a map projection, UI framework, minimap technology, or camera backend.

## 9. Multiplayer boundary

Shared-world context and durable shared state default to server/host authority through a Convergence bridge. Provider network entity IDs never become World IDs. Personal discovery may remain client/profile-owned by project policy, but the neutral core does not select a networking provider or topology.

## 10. Large-world and provider conclusion

The MVP targets bounded authored topology. Addressables, streaming cells, generated worlds, server shards, and hierarchical path planning require separate providers or later modules. Their feasibility and performance remain `Not run`.

## 11. Key risks

| Risk | Conclusion |
|---|---|
| Scene manager duplication | Prevented by Passage execution boundary |
| Save-system duplication | Prevented by detached snapshots and Chronicle transport |
| Universal world god-object | Prevented by typed participants and strict non-goals |
| Scene-name save breakage | Prevented by domain IDs and bindings |
| Marker leaks | Controlled by generational handles and scene cleanup |
| Map/UI coupling | Prevented by neutral snapshots |
| Multiplayer client authority | Prevented by Convergence authority gate |
| Large-world performance | Remains evidence-pending and bounded in MVP |

## 12. Approved feasibility statement

The Atlas is approved as an Advanced provider-neutral package foundation. This approval covers documentation contracts only. Implementation, scene/provider adapters, large-world performance, streaming, procedural generation, platform compatibility, multiplayer behavior, and integration evidence remain `Not run` or deferred.
