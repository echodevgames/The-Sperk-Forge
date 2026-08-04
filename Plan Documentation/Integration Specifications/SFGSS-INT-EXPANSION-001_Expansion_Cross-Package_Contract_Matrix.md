# SFGSS-INT-EXPANSION-001 — Expansion Cross-Package Contract Matrix

**Version:** 1.0.0  
**Status:** Approved  
**Decision date:** August 4, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Scope:** The thirteen Expansion packages in SFGSS-000 Section 7.2, their Foundation seams, and their removal behavior  
**Parent authorities:** SFGSS-000 v0.13.0, the thirteen approved Expansion package specifications, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, SFGSS-005 v1.1.0, and SFGSS-ADR-001 v1.1.0  
**Evidence state:** Documentation review only; all implementation and compatibility evidence remains `Not run`

> Thirteen tools may share one project, but no two may quietly share one throne.

---

## 1. Purpose

This integration specification records the result of SUITE-DOC-23. It compares all thirteen Expansion package specifications against one another and against the approved Foundation authorities.

It answers:

- Which package owns each Expansion concern?
- Which similarly named concepts are actually different identities?
- How do persistent roots, actor-local hosts, and Editor-only tools coexist?
- Which package owns a transaction when several packages participate?
- Which package persists each durable record?
- Which side owns a reusable bridge?
- How are Standalone and Integration Laboratories separated?
- What removal order preserves compilation and project-owned data?

This document adds integration detail. It does not reduce the authority of an individual package specification over that package’s internal API or behavior.

---

## 2. Review result

**Result:** Passed after two documentation repairs.

| Gate | Result | Notes |
|---|---|---|
| One authority per concern | Pass | Completion ownership clarified between The Ascent and The Path |
| Core dependency direction | Pass | No Expansion core requires another Echo runtime core |
| Root and lifecycle topology | Pass | Eleven application-session roots, one actor-local library, one Editor-only package |
| Stable identity | Pass | Package IDs, namespaces, domain IDs, runtime IDs, and diagnostic prefixes remain distinct |
| Settings/save boundary | Pass | Locale preference belongs to The Accord; slot transport belongs to The Chronicle; package state remains participant-owned |
| Transaction ownership | Pass | No cross-package bridge claims a false distributed transaction |
| Workshop setup | Pass after repair | ADR-001 v1.1.0 registers all thirteen Expansion facades |
| Test Lab isolation | Pass | Integration proof remains outside standalone package proof |
| Diagnostics | Pass | All thirteen diagnostic prefixes are unique |
| Removal | Pass | Bridge/provider-first teardown and durable-data preservation are explicit |
| Implementation evidence | Not run | No code, package, scene, prefab, bridge, or provider was executed |

No package was removed, merged, or converted into a hard dependency.

---

## 3. Expansion authority matrix

| Package | Sole Expansion authority | Explicitly remains outside the package | Runtime topology | Durable package state |
|---|---|---|---|---|
| Impact (`EchoFeedback`) | Feedback recipes, transient instances, channel scheduling/arbitration/scaling/cancellation | Camera movement, audio playback, UI state, input-device assignment, settings persistence, combat truth | One application-session root | None by default; configuration only |
| The Wellspring (`EchoPool`) | Reuse pools, leases, capacity, lifecycle, return safety, scopes, diagnostics | Spawn intent, waves, project reset rules, networking spawn authority, saved gameplay truth | One application-session root | None; pool state is session-only |
| The Ascent (`EchoProgression`) | Progression definitions, access/unlocks, progression-node completions, checkpoints, local ranks, password grants | Objective-run completion, scene travel, save transport, RPG XP/stats, platform achievements | One application-session root | Progression state document |
| The Foundry (`EchoBuildTools`) | Editor build recipes, plans, preflight orchestration, stamping, output policy, receipts, manifests | Runtime game flow, Unity Build Profile ownership, source control, deployment vendors, store policy | Editor-only; no runtime root | Project assets, receipts, manifests; not game-save data |
| Many Tongues (`EchoLocalization`) | Locale authority, localized-reference facade, fallback/missing policy, formatting, locale font/script metadata | Translation authorship, UI layout, dialogue flow, audio playback, settings/save transport | One application-session root | Locale preference is external; package configuration/project tables |
| Voices (`EchoDialogue`) | Foreground conversation flow, graph traversal, choices, conditions, typed commands, session-local variables/history | Localization tables, production UI, input, audio, objectives, camera, pause, scene travel, save files | One application-session root | Optional safe-point active-session document |
| The Path (`EchoObjectives`) | Objective definitions, objective-run/step truth, progress graphs, tracking, reward-delivery ledgers | Gameplay facts, inventory/progression mutation, dialogue flow, UI, scene travel, save transport | One application-session root | Objective runs, tracking, reward ledgers |
| The Vault (`EchoInventory`) | Item/container definitions, stacks, unique instances, containers, atomic item transactions, equipment occupancy | Crafting, vendors, combat effects, item-use effects, objectives, character progression, save transport | One application-session root | Inventory state document |
| The Hand (`EchoInteraction`) | Interaction offers, candidate evaluation, interaction focus, prompts, sessions, reservations, commit policy | Foreign gameplay outcome, input bindings, UI, audio/feedback, inventory/objectives/dialogue truth | One application-session root | None by default; active sessions/reservations are session-only |
| The Eye (`EchoCamera`) | Camera channels, targets/groups, modes, leases, blends, lens intent, bounds/zones, backend execution | Character/controller truth, gameplay events, feedback recipes, scene travel, input, UI, render-pipeline policy | One application-session root | None by default; camera runtime state is session-only |
| The Fellowship (`EchoCharacters`) | Character definitions/IDs, rosters, availability, selection, spawning, runtime actors, control ownership/switching | Movement, input devices, camera, combat, inventory contents, save transport, network provider authority | One application-session root | Roster/selection/availability snapshot |
| The Vessel (`EchoControllers`) | Actor-local controller hosts, normalized intent, motors, locomotion state/capabilities | Character roster/control ownership, devices/actions, camera, combat, AI, UI, save/network authority | No global root; one authoritative host/motor per actor | None; live locomotion state is session-only |
| The Crucible (`EchoCrafting`) | Recipes, previews, requirements, one-provider atomic execution, idempotency, recipe knowledge, stations | Inventory storage, skills/stats, UI, objectives, save transport, multiplayer authority, item mutation rules | One application-session root | Recipe knowledge/visibility state only |

---

## 4. Runtime topology and lifecycle

### 4.1 Topology classes

| Class | Packages | Rule |
|---|---|---|
| Application-session authority | Impact, Wellspring, Ascent, Many Tongues, Voices, Path, Vault, Hand, Eye, Fellowship, Crucible | First valid root claims before side effects; injected service remains possible where specified |
| Actor-local authority | Vessel | No application singleton; one validated controller host/motor per actor |
| Editor-only authority | Foundry | No Player assembly or runtime root |

### 4.2 Composition order

The suite does not create hard peer startup dependencies. When First Light composes a full project, the recommended ordering is:

1. Foundation preferences, persistence, state, scene, input, UI, audio, and diagnostics authorities selected by the project.
2. Definition/state authorities: Many Tongues, The Ascent, The Vault, The Fellowship, The Path, and The Crucible.
3. Runtime coordinators: The Wellspring, The Eye, The Hand, Impact, and Voices.
4. Actor-local Vessel hosts when actors spawn.
5. Optional bridges and presenters register after both peers are Ready.

Late registration must reconcile the current snapshot. It must not replay historical events unless the bridge specification explicitly declares replay-safe semantics.

### 4.3 Import and handoff rules

- Package definitions and migration providers initialize before their Chronicle participant applies durable state.
- The Chronicle remains the file/slot transport. Each Expansion package validates and applies only its own participant payload.
- A bridge may delay its own readiness until both peers are available, but neither peer core waits forever for the bridge.
- Shutdown removes bridge/provider registrations before peer roots where possible.
- A root disappearing unexpectedly invalidates its registrations and handles; peers enter a documented unavailable/degraded state rather than retaining stale references.

---

## 5. Identity and terminology registry

### 5.1 Diagnostic prefixes

| Package | Prefix |
|---|---|
| Impact | `EFB-*` |
| The Wellspring | `EPOOL-*` |
| The Ascent | `EPROG-*` |
| The Foundry | `EBUILD-*` |
| Many Tongues | `ELOC-*` |
| Voices | `EDLG-*` |
| The Path | `EOBJ-*` |
| The Vault | `EINV-*` |
| The Hand | `EITR-*` |
| The Eye | `ECAM-*` |
| The Fellowship | `ECHR-*` |
| The Vessel | `ECTR-*` |
| The Crucible | `ECRF-*` |

Prefixes are globally unique. Package-local use-case, capability, Laboratory, operation, request, and provider IDs remain qualified by the package prefix.

### 5.2 Qualified terms

The following words must be qualified in APIs, events, diagnostics, and integration documents:

| Ambiguous word | Approved qualified concepts |
|---|---|
| Focus | `UiFocus`, `InteractionFocus`; camera uses target/shot/view terminology rather than a generic global focus |
| Completion | `ProgressionNodeCompletion`, `ObjectiveRunCompletion`, `ConversationCompletion`, `CraftingResult`, `BuildCompletion` |
| Owner | `InputUserId`, `ControlOwnerId`, `CharacterId`, `ControllerLeaseId`, `NetworkParticipantId`, `PoolOwnerScopeId` |
| Selection | UI selection, selected character, tracked objective, selected locale, selected camera mode, and selected interaction offer remain independent truths |
| Handle | Every package handle is namespace-qualified and generational where it controls recycled/session state; handles never cross package boundaries as durable IDs |
| Checkpoint | Progression checkpoint record, world entry marker, scene destination/route, and save checkpoint are separate contracts |

### 5.3 Character-control identity chain

```text
Input user/device (The Will)
    -> ControlOwnerId assignment (The Fellowship)
        -> actor-local control lease and normalized intent (The Vessel)
            -> runtime actor target snapshot (The Eye / The Hand)
```

No package may use one unqualified `PlayerId` to represent all four layers.

---

## 6. Persistence ownership

| Package/state | Durable? | Persistence owner and rule |
|---|---:|---|
| Impact active instances, cooldowns, provider handles | No | Session-only |
| Wellspring pool instances, leases, scene scopes | No | Session-only |
| Ascent progression state | Yes | One active persistence source: Chronicle bridge, progression-only provider, or project code; never competing providers |
| Foundry recipes and receipts | Project/editor | Stored as project-owned assets/reports, not a runtime save slot |
| Selected locale | Global preference | The Accord; not The Chronicle and not EchoLocalization’s hidden file |
| Localization tables/fonts/assets | Project content | Project-owned assets/backend data |
| Voices active session | Optional | The Chronicle bridge at declared safe points only; committed commands are not replayed |
| Path objective runs and reward ledger | Yes | The Chronicle bridge or project persistence |
| Vault containers, entries, instances | Yes | The Chronicle bridge or project persistence |
| Hand active focus/session/reservations | No | Session-only; durable world outcomes belong to the outcome owner |
| Eye modes, targets, blends, zones, impulses | No | Session-only |
| Fellowship roster, availability, selection | Yes | The Chronicle bridge or project persistence |
| Vessel intent, velocity, contacts, leases | No | Session-only |
| Crucible recipe knowledge | Optional durable | The Chronicle bridge or project persistence; inventory resources remain The Vault’s payload |

Unknown participant records remain opaque and preserved under SFGSS-003 when an optional package is removed.

---

## 7. Transaction and commit ownership

Cross-package operations must not claim distributed atomicity. Exactly one authority owns each commit.

| Workflow | Commit owner | Other participants |
|---|---|---|
| Inventory add/remove/transfer/equip | The Vault | UI, objectives, characters, interaction, crafting request or observe through bridges |
| Immediate craft using Inventory | The Vault resource-provider transaction | The Crucible validates recipe/request and settles its idempotency/result after provider commit |
| Objective completion | The Path | Reward executors deliver afterward using stable grant IDs |
| Progression mutation | The Ascent | Objectives/dialogue/password/platform bridges submit idempotent semantic requests |
| Interaction outcome | Registered endpoint/executor’s owning system | The Hand owns admission/session/commit boundary, not the foreign mutation |
| Camera impulse | The Eye | Impact supplies semantic feedback request through a bridge |
| Hit stop/time multiplier | The Pulse when installed; explicit Impact standalone provider otherwise | Impact owns recipe scheduling, not final global time truth when Pulse exists |
| Dialogue command | Registered command handler’s owning system | Voices owns dispatch, timeout, and route result, not foreign state |
| Build | The Foundry | Package validators report; Unity Build Pipeline performs Player build; providers perform optional post-processing |

Rollback never crosses a foreign authority after that authority reports its commit point. Retries use stable request, operation, transaction, or grant IDs.

---

## 8. Canonical bridge pairing

A peer pair receives at most one reusable bridge artifact for the same behavior. Do not create mirror packages that each translate the same contract in opposite directions.

| Peer pair | Canonical behavioral owner | Canonical bridge responsibility |
|---|---|---|
| Impact + Eye | Impact integration family | Translate feedback camera channel into Eye impulse request |
| Impact + Pulse | Impact integration family | Translate feedback time channel into Pulse multiplier/scope request |
| Impact + Resonance | Impact integration family | Map feedback audio signal to Jukebot cue request |
| Impact + Will/Input System | Impact provider family | Resolve audience/device and perform bounded haptics |
| Wellspring + Passage | Wellspring integration family | Prepare/close scene-scoped pools around transitions |
| Path + Ascent | Path integration family | Query progression conditions and deliver progression reward grants |
| Fellowship + Ascent | Fellowship integration family | Map progression nodes to character availability |
| Looking Glass + Many Tongues | UI/Localization integration artifact | Apply locale direction/font/reference invalidation to UI presentation |
| Voices + Many Tongues | Voices integration family | Resolve speaker/line/choice/voice references |
| Voices + Looking Glass | Voices integration family | Provide the production dialogue presenter |
| Voices + Resonance | Voices integration family | Play voice cues and return completion/stop handles |
| Voices + Path | Voices integration family | Expose objective conditions and explicit commands |
| Path + Vault | Path integration family | Item conditions and idempotent inventory reward grants |
| Vault + Crucible | Crucible integration family | Implement the crafting resource provider through one Vault transaction |
| Vault + Fellowship | Vault integration family | Map character IDs to owned containers/equipment stores |
| Hand + Will | Hand integration family | Translate semantic input phases into interaction commands |
| Hand + Looking Glass | Hand integration family | Present focus/session snapshots and submit commands |
| Eye + Fellowship | Eye integration family | Map active runtime character actors to camera targets/warp revisions |
| Eye + Vessel | Eye integration family | Consume controller pose, velocity, facing, and look-ahead snapshots |
| Fellowship + Vessel | Fellowship integration family | Translate control ownership/switching into actor-local controller leases |

Other pairs remain project adapters until at least two independent projects prove a reusable neutral translation. Bridge package IDs and SemVer compatibility are finalized later under SFGSS-009 and the relevant integration specification.

---

## 9. High-risk composition workflows

### 9.1 Switchable character control

1. The Fellowship validates the requested character and prepares/spawns its actor.
2. The Fellowship assigns the durable `ControlOwnerId` only after required handoff participants prepare successfully.
3. The Fellowship/Vessel bridge releases the old actor-local controller lease and acquires the new lease.
4. The Will adapter routes the matching input user’s normalized intent to the leased Vessel host.
5. The Fellowship/Eye bridge updates camera target ownership after successful control commit.
6. The Hand receives new interactor ownership metadata; stale interaction sessions settle according to policy.

The Fellowship owns the switch commit. The Vessel does not select characters, and The Will does not decide possession.

### 9.2 World pickup interaction

1. The Hand selects the `PickUp` interaction offer.
2. The endpoint executor asks The Vault to validate and commit the add/transfer.
3. The Hand settles the interaction result using the Vault transaction result.
4. The Path bridge may submit progress with a stable request ID.
5. Impact/Resonance bridges may present feedback after the authoritative result.

The Hand never edits inventory directly.

### 9.3 Objective reward delivery

1. The Path commits objective completion once.
2. Each reward receives a deterministic grant ID and ledger entry.
3. Vault, Ascent, Crucible, Fellowship, or project reward executors own their own mutations.
4. The Path records each executor result and may retry only with the same grant ID.
5. A failed reward does not erase objective completion or duplicate successful rewards.

### 9.4 Crafting with inventory

1. The Crucible builds a side-effect-free preview against a Vault resource-provider snapshot/revision.
2. At execution, the Crucible revalidates recipe, requirements, request ID, and provider revision.
3. The Vault provider atomically consumes inputs and grants outputs.
4. The Crucible settles recipe idempotency and recipe-knowledge/events from the provider result.
5. The Path, Impact, Resonance, and UI integrations observe the final result.

### 9.5 Localized voiced dialogue

1. Voices owns graph traversal and current line/choice identity.
2. Many Tongues resolves localized references and locale metadata.
3. The Looking Glass presenter owns layout, focus, typewriter visuals, and user-visible controls.
4. Resonance owns voice playback.
5. The Will supplies semantic advance/skip/choice intent.
6. The Pulse supplies optional dialogue state/pause scope.
7. The Path and foreign systems receive only explicit condition/command requests.

### 9.6 Build and setup

- The Workshop installs packages and invokes exact package-owned ADR-001 facades.
- Each package facade plans/applies/validates only package-owned assets.
- The Foundry orchestrates build preflight by invoking package-owned validators or explicit validator bridges.
- Neither The Workshop nor The Foundry reimplements another package’s validation or migration logic.

---

## 10. Workshop facade registry

ADR-001 v1.1.0 is the exact registry. Every Expansion package remains usable without a facade, but it may not advertise automated Workshop setup until its facade and adapter evidence pass.

| Package | Exact facade type |
|---|---|
| Impact | `EchoDevGames.EchoFeedback.Editor.Workshop.EchoFeedbackWorkshopSetupFacade` |
| The Wellspring | `EchoDevGames.EchoPool.Editor.Workshop.EchoPoolWorkshopSetupFacade` |
| The Ascent | `EchoDevGames.EchoProgression.Editor.Workshop.EchoProgressionWorkshopSetupFacade` |
| The Foundry | `EchoDevGames.EchoBuildTools.Editor.Workshop.EchoBuildToolsWorkshopSetupFacade` |
| Many Tongues | `EchoDevGames.EchoLocalization.Editor.Workshop.EchoLocalizationWorkshopSetupFacade` |
| Voices | `EchoDevGames.EchoDialogue.Editor.Workshop.EchoDialogueWorkshopSetupFacade` |
| The Path | `EchoDevGames.EchoObjectives.Editor.Workshop.EchoObjectivesWorkshopSetupFacade` |
| The Vault | `EchoDevGames.EchoInventory.Editor.Workshop.EchoInventoryWorkshopSetupFacade` |
| The Hand | `EchoDevGames.EchoInteraction.Editor.Workshop.EchoInteractionWorkshopSetupFacade` |
| The Eye | `EchoDevGames.EchoCamera.Editor.Workshop.EchoCameraWorkshopSetupFacade` |
| The Fellowship | `EchoDevGames.EchoCharacters.Editor.Workshop.EchoCharactersWorkshopSetupFacade` |
| The Vessel | `EchoDevGames.EchoControllers.Editor.Workshop.EchoControllersWorkshopSetupFacade` |
| The Crucible | `EchoDevGames.EchoCrafting.Editor.Workshop.EchoCraftingWorkshopSetupFacade` |

---

## 11. Standalone and Integration Laboratories

| Package | Standalone proof |
|---|---|
| Impact | Simulated provider timeline/channel Laboratory |
| The Wellspring | Object reuse/capacity/lease Laboratory |
| The Ascent | Progression, checkpoint, metric, password Laboratory |
| The Foundry | Editor Laboratory and disposable build fixtures |
| Many Tongues | Locale/fallback/font/pseudo-localization Laboratory |
| Voices | Conversation Laboratory with fake providers |
| The Path | Objective graph/reward-ledger Laboratory |
| The Vault | Inventory transaction/equipment Laboratory |
| The Hand | Separate 2D and 3D Interaction Laboratories |
| The Eye | Separate 2D and 3D Camera Laboratories |
| The Fellowship | Roster/spawn/possession Laboratory |
| The Vessel | Independent Side-View 2D and Top-Down 2D Laboratories |
| The Crucible | Recipe preview/transaction Laboratory with simulated provider |

Rules:

- A Standalone Laboratory imports no unrelated Echo package.
- A bridge/provider owns its Integration Laboratory and declares every peer dependency.
- A showcase cannot satisfy either package’s standalone gate.
- Planned Laboratory cases remain `Not run` until executed under SFGSS-004.

---

## 12. Removal behavior

### 12.1 Universal order

1. Stop the feature and settle/cancel active operations at safe points.
2. Remove or disable bridge/provider packages first.
3. Remove samples and Integration Laboratories.
4. Remove the core package.
5. Preserve project-owned configuration and durable data unless the user explicitly approves deletion.
6. Recompile and validate remaining packages in standalone mode.

### 12.2 Package-specific durable effects

| Removed package | Required behavior |
|---|---|
| Impact | Active transient effects stop/settle; no durable state deletion |
| Wellspring | Borrowed instances/scopes settle before removal; no save payload exists |
| Ascent | Chronicle/standalone progression records remain preserved and unavailable until a compatible owner returns |
| Foundry | Build recipes/reports remain project-owned; external deploy/signing providers are removed first |
| Many Tongues | Project tables/fonts/assets remain; remove EchoLocalization bridges before the core and remove Unity Localization only after dependents are gone |
| Voices | Active sessions stop; Chronicle payload remains opaque; dialogue bridges removed first |
| Path | Objective/reward-ledger payload remains opaque; reward bridges removed first |
| Vault | Inventory payload remains opaque; Crafting/Objectives/Characters/UI bridges removed first |
| Hand | Active sessions/reservations settle; no durable interaction-session payload is deleted |
| Eye | Modes/leases/backends settle; provider and peer bridges removed first |
| Fellowship | Control and spawn bridges settle; roster payload remains opaque |
| Vessel | Fellowship/Will/Eye bridges removed first; actor-local components become unavailable without creating a replacement authority |
| Crucible | Inventory/Path/UI/provider bridges removed first; recipe-knowledge payload remains opaque |

---

## 13. Findings and repairs

### EXP-COLL-001 — Progression completion versus objective completion

**Finding:** The Ascent v1.0.0 used generic “completion record” wording that could be read as owning The Path’s objective-run completion.

**Repair:** The Ascent v1.1.0 now limits its completion records to registered progression definitions. The Path remains the sole authority for objective-run and step completion. Bridges translate outcomes through idempotent semantic requests rather than mirroring one record in both packages.

### EXP-COLL-002 — Expansion Workshop facade registry missing

**Finding:** ADR-001 v1.0.0 registered only the nine Foundation runtime facades, while Expansion specifications increasingly referenced automated Workshop setup.

**Repair:** ADR-001 v1.1.0 extends the exact facade registry and minimum setup domains through all thirteen Expansion packages. A missing facade still produces manual setup and does not reduce package independence.

### EXP-COLL-003 — Mirror bridge risk

**Finding:** Several package specifications described both directions of the same peer relationship, creating a risk that two mirror bridge packages could be implemented later.

**Repair:** Section 8 establishes one canonical behavioral owner and one reusable bridge artifact per peer pair/behavior. The peers never reference the bridge.

### EXP-COLL-004 — Cross-package transaction language

**Finding:** Crafting, objective rewards, progression grants, interactions, and inventory transfers participate in multi-package workflows and could be misdescribed later as one distributed atomic transaction.

**Repair:** Section 7 establishes one commit owner per mutation. Other packages orchestrate, ledger, or observe through idempotent requests and honest commit boundaries.

---

## 14. Non-blocking advisories

- Exact bridge package IDs, assembly names, and compatible SemVer ranges remain to be finalized in package-specific integration specifications and SFGSS-009.
- Exact Chronicle participant IDs and durable format IDs require the final suite identity/naming registry review.
- Advanced seams involving The Convergence, Instinct, Clash, Arcana, and The Atlas are reviewed in SUITE-DOC-24, not silently approved here.
- Unity package/backend versions, performance, platform compatibility, installation, migration, and removal evidence remain `Not run`.
- A shared Editor facade contracts package remains rejected for now; ADR-001 may be reconsidered only after real implementations provide evidence.

---

## 15. Approval

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Package implementation remains locked. This matrix authorizes no package manifest, asmdef, source file, asset, scene, prefab, setup facade, bridge, provider, or test execution.

**Next checkpoint:** SUITE-DOC-24 — Advanced Cross-Package and Research Review.
