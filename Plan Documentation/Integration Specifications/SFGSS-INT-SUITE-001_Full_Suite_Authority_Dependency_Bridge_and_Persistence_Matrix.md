---
tags:
  - sfgss/integration
  - sfgss/authority
  - sfgss/persistence
  - sfgss/bridge
status: approved
updated: 2026-08-09
---

# The Sperk’s Forge – Full Suite Authority, Dependency, Bridge, and Persistence Matrix

**Document ID:** SFGSS-INT-SUITE-001
**Version:** 1.1.0
**Status:** Approved full-suite integration baseline
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 9, 2026
**Parent authorities:** SFGSS-000 v0.26.0; SFGSS-001 v1.5.0; SFGSS-002 through SFGSS-010; SFGSS-ADR-001 through SFGSS-ADR-006; and the twenty-eight current package specifications/foundations
**Reconciles:** Foundation, Expansion, Advanced, and Standards/Package consistency matrices

> This is the suite wiring diagram. It summarizes the approved contracts; it does not replace the package specifications that define each component in full.

## 1. Purpose and authority

This document provides one cross-suite view of authority, runtime topology, permitted dependencies, optional bridges, commit ownership, persistence, diagnostics, Laboratories, and removal behavior across all twenty-eight packages.

When this matrix conflicts with a higher authority, stop and reconcile the higher authority. This matrix may clarify composition but may not silently change a package’s ownership contract.

## 2. Executive result

| Review area | Result |
|---|---|
| Sole authority per concern | Pass |
| Circular core Echo dependencies | None approved |
| Runtime package independence | Pass by design; empirical proof remains `Not run` |
| Bridge direction and commit ownership | Pass |
| Global preferences versus save data | Pass |
| Package-owned durable payload boundaries | Pass |
| Session-only state boundaries | Pass |
| Removal and reinstall behavior | Pass by design |
| Diagnostics/test prefix collisions | 0 |
| Provider/backend selection claims | Evidence-gated; no unsupported provider promoted |
| Implementation authorization | Still locked |

No package was merged, removed, or converted into a mandatory shared core. No release-blocking architecture collision remains for SUITE-DOC-31.

## 3. One-screen authority map

```mermaid
flowchart TB
  subgraph Shell[Foundation application shell]
    Launch[First Light] --> Settings[Accord]
    Launch --> Save[Chronicle]
    Launch --> Scene[Passage]
    Launch --> State[Pulse]
    Launch --> Input[Will]
    Launch --> Audio[Resonance]
    Launch --> UI[Looking Glass]
    Diag[Observatory] -. observes .-> Launch
    Workshop[Workshop] -. composes .-> Launch
  end
  subgraph Gameplay[Expansion gameplay infrastructure]
    Characters[Fellowship] --> Controllers[Vessel]
    Input --> Controllers
    Controllers --> Interaction[Hand]
    Interaction --> Dialogue[Voices]
    Interaction --> Inventory[Vault]
    Dialogue --> Objectives[Path]
    Objectives --> Progression[Ascent]
    Inventory --> Crafting[Crucible]
    Feedback[Impact] --> Camera[Eye]
    Feedback --> Audio
    Pool[Wellspring] -. reuse .-> Gameplay
    Localization[Many Tongues] --> UI
    Build[Foundry] --> Release[Release output]
  end
  subgraph Advanced[Advanced provider-neutral foundations]
    AI[Instinct] --> Abilities[Arcana]
    Abilities --> Combat[Clash]
    World[Atlas] --> Scene
    Multiplayer[Convergence] -. validates authority .-> Characters
    Multiplayer -. validates authority .-> World
    Multiplayer -. validates authority .-> Combat
  end
  Save -. transports package payloads .-> Gameplay
  Save -. transports approved payloads .-> Advanced
  Diag -. observes .-> Gameplay
  Diag -. observes .-> Advanced
```

## 4. Master package authority matrix

| Wave | Package | Sole authority | Runtime topology | Durable-state boundary | Hard platform dependency |
|---|---|---|---|---|---|
| Foundation | **First Light** (`EchoLaunch`) | Initial startup claim, ordered initialization, launch reporting, launch-only presentation, and destination handoff | Until-handoff launch root | Session-only launch report; configuration is project-owned | Unity core/modules only; optional providers isolated |
| Foundation | **The Observatory** (`EchoDiagnostics`) | Validation vocabulary, runtime inspection, bounded diagnostics, provider aggregation, and support snapshots | Optional application-session root | Session/support diagnostics only; no gameplay save | Unity core/modules only; optional providers isolated |
| Foundation | **The Accord** (`EchoSettings`) | Global preferences, drafts, previews, apply/rollback transactions, validation, migration, and preference storage | Application-session root | Own JSON document for global preferences; unknown sections/fields preserved | Unity core/modules only; optional providers isolated |
| Foundation | **The Passage** (`EchoSceneFlow`) | Validated scene routes, transition admission, loading phases, activation, recovery, and travel results | Application-session root | Session-only route/transition state; route definitions are project assets | Unity core/modules only; optional providers isolated |
| Foundation | **The Pulse** (`EchoGameState`) | Primary runtime mode, temporary override scopes, pause/time/cursor policy, and effective state history | Application-session root | Session-only by default | Unity core/modules only; optional providers isolated |
| Foundation | **Resonance** (`Jukebot`) | Music, SFX, ambience, mixer routing, voices, transport, handles, and audio runtime diagnostics | Application-session root | Session-only playback state; global audio preferences live in Accord | Unity core/modules only; optional providers isolated |
| Foundation | **The Will** (`EchoInput`) | Input contexts, locks, device/control-scheme state, rebinding, binding overrides, and glyph data | Application-session root | Overrides/preferences persist through Accord or project provider; live contexts are session-only | Unity Input System |
| Foundation | **The Looking Glass** (`EchoUI`) | Screen, HUD, modal, notification, prompt, focus, navigation, and transition presentation infrastructure | Application-session root | Presentation state is session-only unless another authority persists domain data | uGUI and TextMeshPro presentation path |
| Foundation | **The Chronicle** (`EchoSave`) | Save files, slots, generations, manifests, participant routing, migration, backup, and recovery | Application-session root | Owns save transport; package participants own their versioned payload meaning | Unity core/modules only; optional providers isolated |
| Foundation | **The Workshop** (`EchoGameStarter`) | Visible project composition, package selection, dry-run planning, setup-facade orchestration, receipts, repair, and removal guidance | Editor-only Workshop session | Editor transaction journal and project-owned generation receipts; no runtime save | Unity core/modules only; optional providers isolated |
| Expansion | **Impact** (`EchoFeedback`) | Feedback recipes, timeline instances, channel arbitration, scaling, cancellation, and provider requests | Application-session root | Session-only instances/handles; preferences external | Unity core/modules only; optional providers isolated |
| Expansion | **The Wellspring** (`EchoPool`) | Pool definitions, prewarm, reuse leases, bounded growth, retention, return safety, scopes, and diagnostics | Application-session root | Session-only pool state | Unity core/modules only; optional providers isolated |
| Expansion | **The Ascent** (`EchoProgression`) | Unlocks, progression access, progression-node completion, passwords, checkpoints, and local records | Application-session root | Versioned progression state through Chronicle, standalone provider, or project, exactly one active source | Unity core/modules only; optional providers isolated |
| Expansion | **The Foundry** (`EchoBuildTools`) | Build recipes, preflight, versioning/stamping, output safety, build execution, receipts, checksums, and release preparation | Editor-only build session | Project assets and build receipts/manifests; never gameplay save data | Unity Editor Build Profiles / Build Pipeline |
| Expansion | **Many Tongues** (`EchoLocalization`) | Locale selection, localized-reference facade, fallback/missing policy, formatting, script/font metadata, and invalidation | Application-session root | Locale preference belongs to Accord; tables/assets are project content | Unity Localization package |
| Expansion | **Voices** (`EchoDialogue`) | Foreground conversation sessions, graph traversal, choices, conditions, typed commands, variables, and history | Application-session root | Optional safe-point session snapshot through Chronicle; committed commands are not replayed | Unity core/modules only; optional providers isolated |
| Expansion | **The Path** (`EchoObjectives`) | Objective definitions, runs, steps, progress graphs, tracking, completion, and reward-delivery ledgers | Application-session root | Objective state and reward ledger through Chronicle or project persistence | Unity core/modules only; optional providers isolated |
| Expansion | **The Vault** (`EchoInventory`) | Item definitions, stacks, unique instances, containers, equipment occupancy, and atomic inventory transactions | Application-session root | Versioned inventory state through Chronicle or project persistence | Unity core/modules only; optional providers isolated |
| Expansion | **The Hand** (`EchoInteraction`) | Interaction offers, detection candidates, focus, prompts, reservations, sessions, cancellation, and commit policy | Application-session root | Session-only focus/reservations; outcome owner persists durable consequences | Unity core/modules only; optional providers isolated |
| Expansion | **The Eye** (`EchoCamera`) | Camera channels, targets, groups, modes, blends, lens intent, bounds, zones, impulses, and backend execution | Application-session root | Session-only camera state | Unity core/modules only; optional providers isolated |
| Expansion | **The Fellowship** (`EchoCharacters`) | Character definitions/IDs, rosters, availability, selection, spawning, runtime actors, control ownership, and switching | Application-session root | Roster/availability/selection snapshot through Chronicle or project persistence | Unity core/modules only; optional providers isolated |
| Expansion | **The Vessel** (`EchoControllers`) | Normalized family intent, actor-local controller hosts, motors, locomotion state, capabilities, and semantic movement events | Actor-local host and motor | Session-only locomotion, intents, contacts, and leases | Unity core/modules only; optional providers isolated |
| Expansion | **The Crucible** (`EchoCrafting`) | Recipes, previews, requirements, stations, exact combines, atomic transformation requests, idempotency, and recipe knowledge | Application-session root | Recipe knowledge may persist; resources remain with provider such as Vault | Unity core/modules only; optional providers isolated |
| Advanced | **The Convergence** (`EchoMultiplayer`) | Session, participant, readiness, authority, reconnect, travel/spawn contracts, and provider capability facade | Application-session provider-neutral root | Live network state is session-only; authoritative shared saves use Chronicle | Unity core/modules only; optional providers isolated |
| Advanced | **Instinct** (`EchoAI`) | Stimuli, observations, perception memory, scoring, scheduler budgets, blackboards, behavior seams, and navigation contracts | Actor-local hosts plus explicit world/scene registries | Live AI state is session-only; optional approved snapshots at safe points | Unity core/modules only; optional providers isolated |
| Advanced | **Clash** (`EchoCombat`) | Combat requests, targetability, relations, deterministic modifiers, receiver transactions, outcomes, and combat events | Optional application-session service/root | Combat operations are session-only; target resource owners persist their own state | Unity core/modules only; optional providers isolated |
| Advanced | **Arcana** (`EchoAbilities`) | Ability grants/loadouts, activation, costs, charges, cooldowns, casts, channels, interruption, targeting, and effect orchestration | Application-session root with owner-scoped state | Grants/loadouts and optional charges/cooldowns may persist; active casts/effects never do | Unity core/modules only; optional providers isolated |
| Advanced | **The Atlas** (`EchoWorld`) | World/zone/location identity, topology, travel plans, scene bindings, entry/spawn markers, discovery, visitation, and world-state routing | Application-session root | Optional context/discovery/provider records through Chronicle; runtime markers/plans are session-only | Unity core/modules only; optional providers isolated |

### 4.1 Authority rule

A bridge may translate, coordinate, validate, observe, retry, or present. It may not become a second owner of either peer’s truth. A multi-package workflow always identifies one commit owner for each mutation.

## 5. Runtime topology and lifecycle classes

| Topology class | Packages | Lifecycle rule |
|---|---|---|
| Until-handoff launch authority | First Light | Claims before side effects, coordinates startup, then releases startup-only resources according to configuration |
| Application-session persistent authority | Observatory, Accord, Passage, Pulse, Resonance, Will, Looking Glass, Chronicle, Impact, Wellspring, Ascent, Many Tongues, Voices, Path, Vault, Hand, Eye, Fellowship, Crucible, Convergence, Arcana, Atlas | First valid root claims before side effects; duplicate exits; explicit init and shutdown; standalone path cannot require a peer |
| Optional/injectable application service | Clash | May use a root or injected service; one active authority per configured scope |
| Actor-local authority | Vessel; Instinct agent hosts | One validated host/motor or agent host per actor; no global controller/brain singleton |
| Editor-only authority | Workshop; Foundry | No Player/runtime root; one mutating Editor transaction at a time |

### 5.1 Lifetime-separation rule

The phrase **application-session root/authority** in this matrix describes Unity object/service lifetime only. It does not assign durable persistence ownership.

- Chronicle may survive scene changes, but that does not make it the runtime owner of another package's state.
- Accord may survive scene changes and owns global preference truth/persistence independently of Chronicle.
- Inventory, Objectives, Progression, Characters, World, and other participants own their live state and payload meaning even when Chronicle transports snapshots.
- The consumer project may compose multiple long-lived roots beneath a project-owned `DontDestroyOnLoad` composition object. That hierarchy does not transfer authority and is not a service locator.
- First Light initializes/discovers selected authorities during startup and then hands off; it does not become the permanent parent of those services.
- Peer persistence remains a bridge/participant-adapter concern unless an artifact is explicitly classified as such.

See SFGSS-ADR-006.

### 5.2 Recommended composition order

This is an initialization plan, not a hard dependency graph. Omitted packages are simply skipped.

1. First Light claims launch and starts the structured report.
2. Observatory initializes when selected so later startup health is visible.
3. Accord loads global preferences; Chronicle loads catalog/slot metadata when selected.
4. Passage, Pulse, Resonance, Will, Looking Glass, and other selected Foundation authorities initialize independently.
5. Definition/state authorities initialize: Many Tongues, Ascent, Vault, Fellowship, Path, Crucible, Arcana, Atlas, and project participants.
6. Runtime coordinators initialize: Wellspring, Eye, Hand, Impact, Voices, Convergence, Clash, and shared AI registries/schedulers when selected.
7. Actor-local Vessel and Instinct hosts initialize when their actors spawn.
8. Optional bridges/providers register only after both peers are Ready and reconcile the current snapshot rather than replaying unsafe history.
9. First Light delegates the final transition when Passage is installed and completes handoff.

## 6. Dependency and assembly matrix

### 6.1 Core direction

```text
Unity/platform package
    -> one package Runtime/core assembly
        -> package presentation/provider assemblies when explicitly required
        -> package Editor, Tests, and Samples

Peer Runtime A + Peer Runtime B
    -> separate bridge assembly/package
```

| Dependency class | Approved rule |
|---|---|
| Echo core-to-core hard dependency | Prohibited unless SFGSS-000 explicitly classifies the artifact as a bridge, provider adapter, or composer |
| Required Unity package/module | Declared at the package manifest and assembly level with a concrete version |
| Optional Echo integration | Separate bridge or completely compile-isolated owner integration |
| Vendor/platform backend | Separate provider adapter with its own dependency, license, version, Laboratory, and removal path |
| Samples and Laboratories | May depend only on the package under test and declared sample/test dependencies; Integration Labs declare all peers explicitly |
| Workshop | Editor-time exact facade protocol only; no Runtime package depends on Workshop |
| Diagnostics | Each core exposes standalone status; Observatory connection is optional |

### 6.2 Required official/package backends currently planned

| Package | Required/planned platform dependency | Evidence |
|---|---|---|
| The Will | Unity Input System | Planned; exact combination remains `Not run` |
| The Looking Glass | uGUI and TextMeshPro initial presentation path | Planned; exact combination remains `Not run` |
| Many Tongues | Unity Localization | Planned backend; compatibility remains `Not run` |
| The Foundry | Unity Editor Build Profiles and Build Pipeline | Planned; build evidence remains `Not run` |
| First Light default presenter | Separate uGUI presentation assembly | Planned; neutral Runtime remains presentation-backend independent |
| Convergence, Instinct, Eye, and other provider-capable packages | No provider selected in neutral core | Provider-specific evidence remains `Not run` |

## 7. Canonical bridge catalog

The catalog records approved integration intent, not a promise that every bridge ships in the first release. One reusable package pair and behavior should have one named bridge artifact, not two mirror bridges.

| Peer A | Peer B | Integration purpose | Commit/truth owner | Artifact class |
|---|---|---|---|---|
| `EchoLaunch` | `EchoDiagnostics` | Publish launch graph, progress, timing, and report | EchoLaunch owns launch; EchoDiagnostics owns aggregation | Separate bridge |
| `EchoLaunch` | `EchoSettings` | Initialize/load global preferences | EchoSettings | Separate/tiny approved bridge |
| `EchoLaunch` | `EchoSave` | Initialize save catalog and expose continue candidates | EchoSave | Separate bridge |
| `EchoLaunch` | `EchoSceneFlow` | Delegate final launch transition | EchoSceneFlow executes; EchoLaunch owns handoff | Separate bridge |
| `EchoLaunch` | `EchoGameState` | Request Booting/Loading/handoff policy | EchoGameState | Separate bridge |
| `EchoLaunch` | `Jukebot` | Initialize audio and optional startup cues | Jukebot | Separate bridge |
| `EchoLaunch` | `EchoInput` | Initialize input authority/startup context | EchoInput | Separate bridge |
| `EchoLaunch` | `EchoUI` | Replace minimal status/splash presentation and allowed skip UI | EchoLaunch owns readiness; EchoUI owns presentation | Separate bridge |
| `EchoSettings` | `Jukebot` | Apply audio preference snapshot | EchoSettings persists; Jukebot applies | Separate bridge |
| `EchoSettings` | `EchoInput` | Persist/apply binding overrides and input preferences | EchoSettings persists; EchoInput validates/applies | Separate bridge |
| `EchoSettings` | `EchoLocalization` | Persist/apply preferred locale | EchoSettings persists; EchoLocalization selects/applies | Separate bridge |
| `EchoSettings` | `EchoFeedback` | Apply reduced-motion/flash/shake scales | EchoSettings persists; EchoFeedback applies | Separate bridge/project adapter |
| `EchoSettings` | `EchoUI` | Draft/apply/cancel/confirm settings presentation | EchoSettings | Separate bridge |
| `EchoSceneFlow` | `EchoGameState` | Acquire/release Loading state or scope | EchoSceneFlow owns travel; EchoGameState owns state policy | Separate bridge |
| `EchoSceneFlow` | `EchoUI` | Fade/loading/progress presentation | EchoSceneFlow owns transition; EchoUI presents | Separate bridge |
| `EchoSceneFlow` | `EchoSave` | Prepared-load destination transition before participant apply | EchoSave owns load; EchoSceneFlow owns scene operation | Separate bridge/project adapter |
| `EchoSceneFlow` | `EchoWorld` | Execute semantic world travel plan | EchoWorld plans/commits context after EchoSceneFlow succeeds | Separate bridge |
| `EchoSceneFlow` | `EchoMultiplayer` | Coordinate synchronized provider-authorized scene travel | EchoMultiplayer validates session; EchoSceneFlow executes local scene operation | Provider bridge |
| `EchoGameState` | `EchoInput` | Apply effective input context/lock intent | EchoGameState owns policy; EchoInput applies | Separate bridge |
| `EchoGameState` | `Jukebot` | Apply pause/mix policy intent | EchoGameState owns policy; Jukebot applies | Separate bridge |
| `EchoGameState` | `EchoUI` | Coordinate pause/modal/dialogue state with screen leases | EchoGameState owns state; EchoUI owns presentation | Separate bridge |
| `EchoGameState` | `EchoDialogue` | Acquire/release dialogue state scope | EchoGameState owns state; EchoDialogue owns conversation | Separate bridge |
| `EchoInput` | `EchoUI` | Navigation, glyphs, rebinding, and focus-safe locks | EchoInput owns input; EchoUI owns focus/views | Separate bridge |
| `EchoInput` | `EchoCharacters` | Map local input user/device to control owner | EchoCharacters owns control assignment | Separate bridge |
| `EchoInput` | `EchoControllers` | Translate actions to normalized controller intent | EchoControllers owns motor; EchoInput owns device/action state | Adapter package |
| `EchoInput` | `EchoMultiplayer` | Map local input user to network participant authority | EchoMultiplayer owns session participant; EchoInput owns device | Provider bridge |
| `EchoUI` | `EchoSave` | Save-slot views, commands, recovery choices | EchoSave | Separate bridge |
| `EchoUI` | `EchoProgression` | Progression/access/password presentation | EchoProgression | Separate bridge |
| `EchoUI` | `EchoDialogue` | Conversation presenter and choice commands | EchoDialogue | Separate bridge |
| `EchoUI` | `EchoObjectives` | Objective/tracking/reward status presentation | EchoObjectives | Separate bridge |
| `EchoUI` | `EchoInventory` | Inventory/equipment views and transaction commands | EchoInventory | Separate bridge |
| `EchoUI` | `EchoInteraction` | Interaction prompt and hold/timed presentation | EchoInteraction | Separate bridge |
| `EchoUI` | `EchoAbilities` | Loadout, charge, cooldown, cast, and target presentation | EchoAbilities | Separate bridge |
| `EchoUI` | `EchoWorld` | World map, discovery, destination, and travel-plan presentation | EchoWorld | Separate bridge |
| `EchoSave` | `EchoProgression` | Transport progression state participant | EchoProgression owns payload; EchoSave transports | Separate bridge |
| `EchoSave` | `EchoDialogue` | Transport safe-point conversation snapshot | EchoDialogue owns payload; EchoSave transports | Separate bridge |
| `EchoSave` | `EchoObjectives` | Transport objective runs and reward ledger | EchoObjectives owns payload; EchoSave transports | Separate bridge |
| `EchoSave` | `EchoInventory` | Transport inventory state | EchoInventory owns payload; EchoSave transports | Separate bridge |
| `EchoSave` | `EchoCharacters` | Transport roster/availability/selection snapshot | EchoCharacters owns payload; EchoSave transports | Separate bridge |
| `EchoSave` | `EchoCrafting` | Transport recipe knowledge | EchoCrafting owns payload; EchoSave transports | Separate bridge |
| `EchoSave` | `EchoAbilities` | Transport grants/loadouts and approved cooldown/charge state | EchoAbilities owns payload; EchoSave transports | Separate bridge |
| `EchoSave` | `EchoWorld` | Transport world context/discovery/provider records | EchoWorld/provider owns payload; EchoSave transports | Separate bridge |
| `EchoSave` | `EchoAI` | Transport optional approved AI snapshot at safe point | EchoAI/project owns payload; EchoSave transports | Separate bridge/project participant |
| `EchoFeedback` | `EchoCamera` | Submit semantic camera impulse requests | EchoCamera executes camera motion | Separate bridge |
| `EchoFeedback` | `Jukebot` | Map semantic feedback signals to audio cues | Jukebot executes playback | Separate bridge |
| `EchoFeedback` | `EchoUI` | Map flash/UI impulse channels to presentation | EchoUI executes presentation | Separate bridge/project provider |
| `EchoFeedback` | `EchoInput` | Resolve haptic audience/device | EchoInput resolves device; haptics provider executes | Provider bridge |
| `EchoFeedback` | `EchoGameState` | Request bounded time modifiers | EchoGameState owns time policy | Separate bridge |
| `EchoPool` | `EchoSceneFlow` | Flush/close scene-scoped pools around transitions | EchoPool owns pools; EchoSceneFlow owns transition | Separate bridge |
| `EchoPool` | `EchoFeedback` | Provide reusable VFX objects | EchoPool owns reuse; EchoFeedback owns feedback instance | Provider/project adapter |
| `EchoLocalization` | `EchoUI` | Localized text/assets, direction, font profile, invalidation | EchoLocalization owns localized resolution; EchoUI presents | Separate bridge |
| `EchoLocalization` | `EchoDialogue` | Resolve speaker/line/choice references and arguments | EchoLocalization resolves; EchoDialogue owns flow | Separate bridge |
| `EchoLocalization` | `Jukebot` | Resolve localized audio references | EchoLocalization resolves asset; Jukebot plays | Separate bridge/project adapter |
| `EchoLocalization` | `EchoBuildTools` | Run locale/table/font/pseudo-locale release validation | EchoBuildTools owns build gate; EchoLocalization owns validation data | Editor bridge |
| `EchoDialogue` | `EchoObjectives` | Read objective conditions and submit explicit objective commands | Each authority owns its own state; command target commits | Separate bridge/project handler |
| `EchoDialogue` | `EchoCamera` | Submit speaker/cinematic view intents | EchoCamera | Separate bridge/project handler |
| `EchoDialogue` | `Jukebot` | Submit voice/audio cue requests | Jukebot | Separate bridge/provider |
| `EchoObjectives` | `EchoInventory` | Read item facts and deliver idempotent item rewards | EchoObjectives commits completion; EchoInventory commits grant | Separate bridge |
| `EchoObjectives` | `EchoProgression` | Submit idempotent progression rewards/conditions | EchoObjectives commits completion; EchoProgression commits mutation | Separate bridge |
| `EchoInventory` | `EchoCrafting` | Query ingredients and commit atomic consume/grant transaction | EchoInventory/resource provider commits resources; EchoCrafting owns recipe result | Separate bridge |
| `EchoInventory` | `EchoCharacters` | Map character ownership to containers/equipment | EchoCharacters owns identity; EchoInventory owns containers | Separate bridge/project adapter |
| `EchoInventory` | `EchoInteraction` | Pickup/drop availability and commands | EchoInteraction owns session; EchoInventory commits item transaction | Separate bridge/project adapter |
| `EchoInventory` | `EchoAbilities` | Supply item/ammunition cost provider | EchoInventory commits cost; EchoAbilities commits activation | Separate bridge |
| `EchoInteraction` | `EchoDialogue` | Offer/execute Speak or conversation actions | EchoInteraction owns interaction session; EchoDialogue commits conversation start | Separate bridge/project adapter |
| `EchoInteraction` | `EchoObjectives` | Expose objective-dependent offers and submit interaction progress | Each authority owns its own commit | Separate bridge/project adapter |
| `EchoInteraction` | `EchoCharacters` | Use actor identity/availability for interactor and target | EchoCharacters owns identity; EchoInteraction owns offer/session | Separate bridge |
| `EchoInteraction` | `EchoControllers` | Acquire movement suspension and consume facing/origin snapshots | EchoInteraction owns session; EchoControllers owns locomotion | Project adapter |
| `EchoCamera` | `EchoCharacters` | Resolve character/runtime actor targets | EchoCharacters owns actor identity; EchoCamera owns view | Separate bridge |
| `EchoCamera` | `EchoControllers` | Consume pose, velocity, facing, and warp revision | EchoControllers owns motor state; EchoCamera owns view | Separate bridge |
| `EchoCamera` | `EchoWorld` | Resolve world camera bounds/zones/semantic locations | EchoWorld owns world identity; EchoCamera owns view | Separate bridge/project adapter |
| `EchoCharacters` | `EchoControllers` | Assign control ownership to actor-local controller lease | EchoCharacters owns control assignment; EchoControllers owns motor | Separate bridge |
| `EchoCharacters` | `EchoAbilities` | Map durable character to ability owner/loadout lifecycle | EchoCharacters owns identity; EchoAbilities owns ability state | Separate bridge |
| `EchoCharacters` | `EchoAI` | Map actors/characters to AI agent lifecycle | EchoCharacters owns actor identity; EchoAI owns thinking | Separate bridge |
| `EchoCharacters` | `EchoMultiplayer` | Map network participant ownership to character spawn/control | EchoMultiplayer validates authority; EchoCharacters owns roster/spawn | Separate provider bridge |
| `EchoControllers` | `EchoAI` | Translate AI navigation/locomotion intent to motor | EchoAI chooses; EchoControllers moves | Separate bridge/project adapter |
| `EchoControllers` | `EchoCombat` | Apply explicit post-commit external-motion/knockback request | EchoCombat reports outcome; EchoControllers applies motion | Separate bridge/project adapter |
| `EchoCrafting` | `EchoObjectives` | Submit committed craft progress/results | EchoCrafting owns transform; EchoObjectives owns progress | Separate bridge |
| `EchoCrafting` | `EchoProgression` | Check recipe knowledge/unlocks or submit progression result | Each authority owns its own state | Separate bridge/project adapter |
| `EchoCrafting` | `EchoMultiplayer` | Validate authoritative crafting requests/results | EchoMultiplayer validates authority; EchoCrafting/resource provider commits | Provider bridge |
| `EchoAI` | `EchoCombat` | Use combat observations/threat and submit combat requests | EchoAI chooses; EchoCombat resolves | Separate bridge |
| `EchoAI` | `EchoAbilities` | Query and request ability activations | EchoAI chooses; EchoAbilities commits activation | Separate bridge |
| `EchoAI` | `EchoWorld` | Consume zone/location context and patrol/tactical points | EchoWorld owns world identity; EchoAI owns behavior | Separate bridge |
| `EchoCombat` | `EchoAbilities` | Resolve instantaneous combat effects submitted by committed abilities | EchoAbilities commits activation; EchoCombat receiver commits resource mutation | Separate bridge |
| `EchoCombat` | `EchoMultiplayer` | Validate/replicate authoritative combat requests and results | EchoMultiplayer validates authority; EchoCombat/domain receiver owns result | Provider bridge |
| `EchoAbilities` | `EchoMultiplayer` | Validate/replicate activation, sequence, and reconciliation | EchoMultiplayer validates authority; EchoAbilities owns ability state | Provider bridge |
| `EchoWorld` | `EchoMultiplayer` | Coordinate authoritative world context and synchronized travel | EchoMultiplayer validates session; EchoWorld owns semantic context | Provider bridge |
| `EchoWorld` | `EchoCharacters` | Resolve arrival/spawn marker then spawn/relocate actors | EchoWorld selects metadata; EchoCharacters/project performs actor operation | Separate bridge |

### 7.1 Generic suite integrations

- Every runtime package may expose a separate Observatory provider bridge. The source package owns its status; Observatory owns aggregation and presentation.
- The Workshop may call every package’s exact Editor setup facade under ADR-001. The peer package owns setup behavior and the Workshop owns the composition transaction.
- The Chronicle may transport any approved versioned package participant payload. The participant owns schema meaning, migration, validation, and apply behavior.
- The Foundry may consume package validator providers at build time. The package owns validation knowledge; Foundry owns the build gate and receipt.

## 8. Multi-package commit-ownership workflows

| Workflow | Ordered authority chain | Commit rule |
|---|---|---|
| Application startup | First Light -> selected package startup steps -> Passage final transition -> First Light handoff | Each package commits only its own initialization; Passage commits scene operation; First Light commits launch completion |
| Settings change | Looking Glass draft -> Accord plan/apply -> peer appliers | Accord commits preference document only after required appliers succeed or documented rollback completes |
| Save/load with scene travel | Chronicle prepares payload -> Passage transitions -> package participants apply | Chronicle owns load transaction; Passage owns scene operation; each participant commits its own state |
| Semantic world travel | Atlas prepares plan -> Convergence validates when networked -> Passage loads -> Atlas commits context -> Fellowship/project positions actors | No package performs another package’s step |
| Character control | Will user/device -> Fellowship `ControlOwnerId` -> Vessel actor-local lease -> Eye/Hand target snapshots | Fellowship commits control ownership; Vessel commits locomotion state |
| Interaction outcome | Hand offer/session -> project or peer executor | Hand commits interaction session state; outcome owner commits durable gameplay result |
| Dialogue command | Voices traverses node -> typed command handler -> target package/project | Voices commits conversation cursor; command target commits its own mutation |
| Objective reward | Path commits objective completion -> reward grant ID -> target executor | Path completion never rolls back because a foreign reward fails; each reward owner commits idempotently |
| Crafting | Crucible validates recipe/preview -> one mutation-capable resource provider commits consume/grant | Crucible owns transformation result; resource provider owns atomic resource mutation |
| Ability and combat | Arcana validates/casts/commits -> effect submits Clash request -> target receiver commits resource change | Arcana and Clash have separate commitment points; neither rewinds the other after foreign commit |
| AI action | Instinct observes/scores/chooses -> Arcana/Hand/Vessel/Clash request | Instinct chooses; requested authority validates and commits |
| Multiplayer gameplay | Convergence validates participant/provider authority -> domain package executes | Convergence does not replace domain truth; server/host validates domain requests |
| Feedback | Domain event -> Impact timeline -> Eye/Jukebot/UI/haptics/Pulse providers | Domain event is already committed; each provider executes only its channel |
| Build | Foundry immutable plan -> validators -> temporary stamp -> Unity build -> receipts/restoration | Foundry owns build transaction; Unity Build Profile remains platform configuration authority |
| Project generation | Workshop dry-run -> package facades -> receipts | Workshop owns composition transaction; each package facade owns its setup operation |

## 9. Full-suite persistence matrix

### 9.1 Persistence layers

| Layer | Sole authority | Rule |
|---|---|---|
| Global preferences | Accord | Installation/player/device-wide preferences; not duplicated into save slots by convenience |
| Save files, slots, generations, backups, recovery | Chronicle | Transport and container only; never interprets every package’s domain model |
| Package durable payload | Owning package participant | Owns schema, stable IDs, validation, migration, aliases, unknown-data policy, and apply |
| Project durable state | Project participant/provider | Remains outside package source and uses explicit contracts |
| Session-only state | Owning runtime authority | Never serialized unless a package specification approves a safe detached snapshot |
| Editor project records | Workshop/Foundry/project tools | Not gameplay save data; Library journals are transient and project receipts are explicit |

### 9.2 Package persistence ownership

| Package | Durable state | Transport/backend rule | Must never persist |
|---|---|---|---|
| **First Light** | Session-only launch report; configuration is project-owned | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | active launch steps, scene objects, or startup-only resources |
| **The Observatory** | Session/support diagnostics only; no gameplay save | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | unbounded logs, private payloads, or gameplay truth |
| **The Accord** | Own JSON document for global preferences; unknown sections/fields preserved | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | save-slot progress or foreign package meaning |
| **The Passage** | Session-only route/transition state; route definitions are project assets | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | live transition handles or scene-object references |
| **The Pulse** | Session-only by default | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | override/pause leases or derived effective policy |
| **Resonance** | Session-only playback state; global audio preferences live in Accord | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | active voices, handles, cooldowns, or crossfades |
| **The Will** | Overrides/preferences persist through Accord or project provider; live contexts are session-only | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | active contexts, locks, devices, or rebind operations |
| **The Looking Glass** | Presentation state is session-only unless another authority persists domain data | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | open screen/modal handles, focus objects, or presenter references |
| **The Chronicle** | Owns save transport; package participants own their versioned payload meaning | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | project-specific mutable models inside the save core |
| **The Workshop** | Editor transaction journal and project-owned generation receipts; no runtime save | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | runtime game state or private project credentials |
| **Impact** | Session-only instances/handles; preferences external | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | active feedback instances/provider handles |
| **The Wellspring** | Session-only pool state | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | pooled object instances/leases |
| **The Ascent** | Versioned progression state through Chronicle, standalone provider, or project, exactly one active source | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | objective-run truth, scene operations, or RPG stat state |
| **The Foundry** | Project assets and build receipts/manifests; never gameplay save data | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | runtime gameplay state |
| **Many Tongues** | Locale preference belongs to Accord; tables/assets are project content | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | hidden locale preference file, resolved production text history, or UI state |
| **Voices** | Optional safe-point session snapshot through Chronicle; committed commands are not replayed | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | provider handles, scene objects, or replayed committed commands |
| **The Path** | Objective state and reward ledger through Chronicle or project persistence | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | foreign reward resources or gameplay fact databases |
| **The Vault** | Versioned inventory state through Chronicle or project persistence | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | scene GameObjects, foreign combat effects, or save files |
| **The Hand** | Session-only focus/reservations; outcome owner persists durable consequences | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | active offers/focus/reservations or foreign outcome truth |
| **The Eye** | Session-only camera state | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | targets, blends, zones, impulses, or backend handles |
| **The Fellowship** | Roster/availability/selection snapshot through Chronicle or project persistence | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | runtime actor GameObjects or controller/input/network handles |
| **The Vessel** | Session-only locomotion, intents, contacts, and leases | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | velocity, contacts, intent buffers, or control leases |
| **The Crucible** | Recipe knowledge may persist; resources remain with provider such as Vault | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | active transactions/provider handles or inventory resources |
| **The Convergence** | Live network state is session-only; authoritative shared saves use Chronicle | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | live provider objects, credentials, tickets, or client-trusted shared truth |
| **Instinct** | Live AI state is session-only; optional approved snapshots at safe points | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | observations, paths, behavior tickets, or provider handles |
| **Clash** | Combat operations are session-only; target resource owners persist their own state | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | in-flight requests, modifiers, hit-provider handles, or unbounded logs |
| **Arcana** | Grants/loadouts and optional charges/cooldowns may persist; active casts/effects never do | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | active casts, channels, targets, effect tickets, or provider handles |
| **The Atlas** | Optional context/discovery/provider records through Chronicle; runtime markers/plans are session-only | Chronicle/project/provider according to the package contract; unknown optional records are preserved where specified | runtime markers, scene objects, prepared travel plans, or provider handles |

### 9.3 Unknown data and removal

- Accord preserves unknown optional sections and unknown fields through opaque/extension-capable serialization.
- Chronicle preserves unknown participant payloads without interpreting or deleting them.
- Package participants preserve unknown definitions, aliases, tombstones, or extension records according to SFGSS-003.
- Removing a bridge/provider does not delete peer configuration or durable records.
- Reinstalling a compatible package may reclaim its preserved records after validation and migration.

## 10. Identity crosswalk

These identities may be related by a bridge but are never interchangeable:

| Domain | Qualified identity | Owner |
|---|---|---|
| Input | `InputUserId`, device/control-scheme identity | Will |
| Multiplayer | `NetworkParticipantId`, provider network entity | Convergence/provider |
| Character | `CharacterDefinitionId`, durable `CharacterId`, runtime instance ID, `ControlOwnerId` | Fellowship |
| Controller | Controller host, intent-source lease, controller control lease | Vessel |
| AI | Agent ID, observation ID, behavior ticket | Instinct |
| Combat | Combat request ID, causality ID, target ID | Clash/target authority |
| Ability | Ability definition/owner/activation IDs | Arcana |
| Inventory | Item definition, entry, instance, container, slot, transaction IDs | Vault |
| Objective | Objective definition, run, node, progress request, reward grant IDs | Path |
| Progression | Progression definition/node/checkpoint/password scheme IDs | Ascent |
| World | World, zone, location, connection, scene binding, entry/spawn marker IDs | Atlas |
| Scene flow | Scene definition, route, transition request IDs | Passage |
| UI | Screen/modal/notification/focus handles | Looking Glass |
| Interaction | Interactor, endpoint, offer, session, reservation IDs | Hand |
| Save | Slot, generation, participant, prepared-load IDs | Chronicle |

Unity asset GUIDs, asset paths, scene names, build indexes, hierarchy paths, display labels, and runtime instance IDs do not replace stable domain identities.

## 11. Diagnostics and evidence

- All twenty-eight packages use unique SFGSS-008 diagnostic/test prefixes.
- Every package remains diagnosable without Observatory; Observatory bridges aggregate neutral snapshots and bounded events.
- Standalone Laboratories prove one package only. Integration Laboratories prove one explicit bridge/provider pairing.
- Showcases never substitute for standalone or integration evidence.
- Every implementation, installation, migration, platform, provider, performance, compatibility, and release result remains `Not run` until a retained execution record exists.

## 12. Removal and replacement order

1. Disable the integration path and stop creating new work.
2. Settle or cancel uncommitted operations according to the owning package’s commit boundary.
3. Dispose bridge/provider registrations, leases, callbacks, and subscriptions.
4. Remove Integration Laboratory samples for that bridge/provider.
5. Remove the bridge/provider package.
6. Preserve project-owned configuration, definitions, payloads, aliases, reports, and migration evidence unless explicit pruning is approved.
7. Remove or replace the core package only after no remaining artifact depends on it.
8. Recompile and run surviving peers’ standalone validation and removal tests.
9. Update the Workshop generation/removal receipt, compatibility catalog, Current Notes, and affected guides.

The Workshop never silently deletes modified or adopted project assets. The Foundry never cleans an unowned or protected output path. Chronicle and Accord do not erase unknown records merely because a peer is absent.

## 13. Research and open decisions

| Topic | Current state | Release gate |
|---|---|---|
| Multiplayer provider | Not selected | At least two disposable comparison prototypes plus provider-selection ADR |
| AI navigation/behavior/inference adapters | Candidates only | Separate adapter specifications, license review, Laboratories, and evidence |
| Cinemachine and other camera backends | Optional provider | Exact Unity/package compatibility evidence |
| EchoControllers package split | Open later decision | Dependency/release-cadence evidence and ADR |
| Package source/sample licenses | Open | Approved licensing decision before public release |
| Hardware-sensor diagnostics providers | Optional research | Platform/native provider specification and evidence |

None of these open questions blocks the documentation handoff audit. They block only affected implementation or release claims.

## 14. Gate result

- [x] One authoritative owner remains for every listed concern.
- [x] No circular core Echo runtime dependency is approved.
- [x] Optional bridges/providers are visible and removable.
- [x] Multi-package operations identify commit owners.
- [x] Global settings, save transport, package payloads, session state, and Editor records are separated.
- [x] Durable persistence, mutable runtime truth, and Unity object lifetime are independently assigned; no universal persistent root/service locator is approved.
- [x] Unknown durable data survives optional peer removal where required.
- [x] Standalone and Integration Laboratory responsibilities are explicit.
- [x] Diagnostics and identity namespaces remain qualified and collision-free.
- [x] No empirical result was promoted without evidence.
- [x] Package implementation remains locked.

**Decision:** Approved. v1.1.0 reconciles SFGSS-ADR-006 and the Chronicle learning activation without changing the package dependency graph. Empirical implementation evidence remains gated.

## Graph Navigation

#sfgss/integration #sfgss/authority #sfgss/persistence #sfgss/bridge

- [[../Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[Foundation_Cross-Package_Contract_Matrix|Foundation Matrix]]
- [[SFGSS-INT-EXPANSION-001_Expansion_Cross-Package_Contract_Matrix|Expansion Matrix]]
- [[SFGSS-INT-ADVANCED-001_Advanced_Cross-Package_and_Research_Contract_Matrix|Advanced Matrix]]
- [[SFGSS-INT-CONSISTENCY-001_Standards_and_Package_Consistency_Matrix|Consistency Matrix]]
- [[../Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[../Package_Learning_Review_Catalog|Package Learning Review Catalog]]
