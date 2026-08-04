# Foundation Cross-Package Contract Matrix and Findings Record

**Document ID:** SFGSS-INT-FOUNDATION-001  
**Version:** 1.0.0  
**Status:** Approved cross-spec reconciliation baseline  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 3, 2026  
**Parent authorities:** SFGSS-000 v0.7.0, SFGSS-001 v1.1.0, and the ten approved Foundation package specifications  
**Related decision:** [[../Architecture Decision Records/SFGSS-ADR-001_Foundation_Editor_Setup_Facade_Protocol|SFGSS-ADR-001 — Foundation Editor Setup Facade Protocol]]  

> Ten blueprints entered the forge. This record checks that their doors open onto the same building rather than ten neighboring castles with suspiciously similar drawbridges.

---

## 1. Purpose and audit rule

This document is the required FW-DOC-11 collision test for the ten Foundation packages. It compares the approved specifications without silently replacing them. Where two specifications use different implementation detail without contradicting authority, the difference is recorded as compatible. Where a collision exists, the owning Level 2 specification or a Level 3 ADR is updated before the finding is closed.

The audit covers:

- Authority and non-ownership.
- Persistent roots, duplicate protection, initialization, handoff, and shutdown.
- Runtime and Editor dependency direction.
- Optional bridges and removal order.
- The Workshop setup-facade boundary.
- Global settings versus game-save persistence.
- UI, input, audio, timing, and accessibility coordination.
- Diagnostics and globally searchable identifiers.
- Standalone Test Labs and integration evidence.
- Clean package, bridge, sample, and Workshop removal.

This matrix does **not** authorize runtime implementation. FW-DOC-12 remains the implementation readiness gate.

---

## 2. Executive result

| Audit area | Result | Notes |
|---|---|---|
| Authority ownership | Pass | Exactly one owner remains for each Foundation concern |
| Runtime dependencies | Pass | Core runtime packages declare no peer Echo runtime dependency |
| Lifecycle and startup | Pass | Explicit initialization and standalone self-initialization avoid circular startup requirements |
| Duplicate safety | Pass | Every persistent authority rejects duplicates before side effects |
| Settings/save boundary | Pass | The Accord owns global preferences; The Chronicle owns slot/profile game saves |
| Presentation boundary | Pass | The Looking Glass presents peer state; it does not absorb peer rules |
| Diagnostics | Pass after correction | Pulse diagnostic prefix changed from `EGS-*` to globally unique `EGSTATE-*` |
| Setup facades | Pass after ADR | SFGSS-ADR-001 establishes the exact package-owned Editor facade protocol |
| Test Labs | Pass | Nine runtime packages have isolated labs; Workshop has an Editor Laboratory/clean-project fixture exception |
| Removal behavior | Pass with integration requirement | Bridge packages must be removed before either peer; core and generated projects remain independent |
| Implementation authorization | Still locked | FW-DOC-12 and the First Light M1 Checkpoint Build Plan remain outstanding |

### 2.1 Findings count

- **Blocking collisions found:** 2
- **Blocking collisions resolved in this checkpoint:** 2
- **Unresolved release-blocking questions:** 0
- **Non-blocking implementation advisories:** 4

---

## 3. Authority matrix

| Concern | Sole authority | May request/present/observe | Must not become a second authority |
|---|---|---|---|
| Initial application startup | First Light (`EchoLaunch`) | Workshop configures; Observatory observes; peers contribute startup steps through bridges | Passage cannot replace launch orchestration; UI cannot decide readiness |
| Diagnostics and validation vocabulary | The Observatory (`EchoDiagnostics`) | Every package exposes standalone status; bridges publish neutral snapshots | Observatory cannot repair production state or own peer behavior |
| Global preferences | The Accord (`EchoSettings`) | UI edits; Jukebot/Input/localization/feedback apply through bridges | Save slots and project gameplay cannot become preference sections merely for convenience |
| Normal scene travel | The Passage (`EchoSceneFlow`) | First Light may delegate final load; UI presents; Pulse coordinates policy | First Light’s one-time standalone loader cannot become the normal travel API |
| High-level runtime mode and pause | The Pulse (`EchoGameState`) | UI, Passage, Save, dialogue/cutscene project code request states/scopes | UI, Input, and Audio cannot set global pause/time/cursor truth independently |
| Music, SFX, ambience, mixer routing | Resonance (`Jukebot`) | Settings applies bus values; gameplay/UI request semantic cues; Pulse requests policy | UI and GameState cannot own playback sources or mixer persistence |
| Input contexts, locks, devices, rebinding, glyph data | The Will (`EchoInput`) | UI presents; Pulse requests context/locks; Settings persists overrides | Controllers and UI navigation cannot turn EchoInput into movement/gameplay authority |
| Screen/HUD/modal/notification/prompt presentation | The Looking Glass (`EchoUI`) | Every domain provides presenters/adapters; Will supplies navigation data | UI cannot own settings, saves, state, scene loading, audio, or gameplay rules |
| Save files, slots, generations, participants, recovery | The Chronicle (`EchoSave`) | UI presents; Passage coordinates prepared loads; gameplay packages contribute payloads | Settings and Workshop cannot store game progress as a shortcut |
| Editor-time starter composition | The Workshop (`EchoGameStarter`) | Package-owned Editor facades plan/apply package assets | Workshop cannot become a runtime manager or duplicate peer setup logic |

**Finding:** No duplicate runtime authority remains after reconciliation.

---

## 4. Runtime roots and lifecycle matrix

| Package | Root/authority | Default lifetime | Claim/init rule | Duplicate behavior | Direct-scene rule |
|---|---|---|---|---|---|
| First Light | `EchoLaunchRoot` | Until handoff by default; configurable | Claim in `Awake`, execute after validation | Later root exits before presentation, steps, subscriptions, or load | Development initializer creates launch root only when absent |
| Observatory | `EchoDiagnosticsRoot` | Application session when enabled | Claim first, initialize owned providers/presenter later | Reject before recorders, callbacks, export, or overlay side effects | Creates only Observatory when absent |
| Accord | `EchoSettingsRoot` | Application session | Explicit `InitializeAsync`; standalone auto-init optional | Reject before load, registration, apply, timers, or writes | Creates only Settings when absent |
| Passage | `EchoSceneFlowRoot` | Application session | Explicit init or controlled root auto-init | Reject before transition state, presenters, or scene work | Creates only SceneFlow when absent |
| Pulse | `EchoGameStateRoot` | Application session | Explicit init through root/bridge/helper | Reject before adapters, state mutation, or subscriptions | Creates only GameState when absent |
| Resonance | `JukebotRoot` | Application session | `Awake` claims; `InitializeAsync` performs audio side effects | Reject before sources, mixer, pools, subscriptions, or playback | Creates only Jukebot when absent |
| Will | `EchoInputRoot` | Application session | Claim, then explicit or controlled auto-init | Reject before action cloning, subscriptions, pairing, overrides, or map changes | Creates only Input when absent |
| Looking Glass | `EchoUIRoot` | Application session | Standalone explicit path or First Light bridge | Reject before EventSystem, layers, focus, transitions, or registrations | Creates only UI when absent |
| Chronicle | `EchoSaveRoot` | Application session | Explicit `InitializeAsync`; optional controlled auto-init | Reject before paths, scans, callbacks, registrations, or operations | Creates only Save when absent |
| Workshop | `WorkshopSession` Editor transaction | Window/transaction only | Explicit Editor action | One mutating project lock; read-only windows may coexist | Runtime scene rule not applicable |

### 4.1 Shared root rule

Every runtime bridge or First Light startup step must **adopt and initialize an existing valid authority when one is already present**. It must not instantiate a second root blindly. Development initializers create only their own package’s minimum missing root and remain disabled or excluded from release by default.

### 4.2 Root hierarchy rule

Persistent package roots are independent top-level authorities. One package root must not become a child whose lifetime is silently controlled by another package root. First Light coordinates initialization but does not parent, own, or destroy peer authorities.

---

## 5. Recommended startup and handoff order

This order is a default composition plan, not a hard runtime dependency graph. A project may omit any optional package.

| Order | Authority | Reason |
|---:|---|---|
| 1 | First Light claim/preflight/report | Establish one launch session before startup side effects |
| 2 | Observatory, when selected | Makes later startup health visible; never required for success |
| 3 | Accord | Loads global preferences before audio/input/UI adapters apply them |
| 4 | Chronicle, when selected | Builds slot/catalog access before continue/destination decisions |
| 5 | Pulse | Establishes Booting/Loading policy and neutral coordination intents |
| 6 | Passage | Prepares the normal travel authority and final-load bridge |
| 7 | Resonance | Applies configured/default audio and optional Accord values |
| 8 | Will | Establishes contexts, device state, and binding overrides |
| 9 | Looking Glass | Creates presentation/navigation after optional input/settings data exists |
| 10 | First Light presentation and destination handoff | Run configured launch presentation, delegate final load when Passage bridge exists, then complete handoff |

### 5.1 Initial load boundary

- First Light owns launch orchestration until its handoff is complete.
- Without Passage, First Light may perform exactly its approved standalone initial destination load.
- With the First Light–Passage bridge, Passage executes the final scene operation while First Light remains the launch coordinator.
- After handoff, all normal scene travel belongs to Passage.
- First Light’s internal destination loader is not exposed as a reusable mid-game travel service.

### 5.2 No circular readiness

No package may require another peer to initialize its own core. Bridges wait for or register against explicit peer lifecycle state. A missing bridge leaves both cores in their documented standalone behavior.

---

## 6. Dependency and assembly direction

### 6.1 Core rule

```text
Unity/platform dependencies
        ↓
Package Runtime assembly
        ↓
Package Editor / Samples / Tests
```

No Foundation Runtime assembly references another Foundation Runtime assembly. Optional cross-package behavior lives in a bridge package/assembly or project adapter.

### 6.2 Bridge rule

A separately distributed two-package bridge:

- declares compatible hard package dependencies on both peers;
- contains no authoritative replacement service;
- translates requests/results/events only;
- disposes registrations and leases idempotently;
- fails validation rather than partially registering on version mismatch;
- is removed before either peer is removed;
- never makes either peer require the bridge for standalone operation.

Tiny integrations may live with an owning package only when compile-time exclusion is complete and removing the peer cannot leave compile errors. Provider/vendor adapters remain separate.

### 6.3 Workshop Editor direction

The Workshop core does not reference peer assemblies. Exact facade descriptors and narrow reflection invoke package-owned Editor facades under SFGSS-ADR-001. No runtime assembly references The Workshop.

---

## 7. Primary bridge contract matrix

The table records direction and ownership. It does not claim every bridge ships in the first package release.

| Integration | Bridge/adaptor direction | Truth owner | Bridge placement |
|---|---|---|---|
| Launch → Diagnostics | Launch status/report becomes neutral provider snapshot | First Light owns launch; Observatory owns dashboard vocabulary | Separate bridge |
| Launch → Settings | First Light requests initialization/load result | Accord | Separate or tiny compile-safe bridge, finalized by integration spec |
| Launch → Save | First Light requests catalog/init and receives continue candidates | Chronicle | Separate bridge |
| Launch → SceneFlow | First Light delegates final transition | Passage executes travel; First Light owns launch handoff | Separate bridge |
| Launch → GameState | Booting/Loading/handoff requests and results | Pulse | Separate bridge |
| Launch → Jukebot | Initialization/readiness and optional startup cue requests | Resonance | Separate bridge |
| Launch → Input | Startup context/init request | Will | Separate bridge |
| Launch ↔ UI | UI presenter reads launch status and may request allowed skip | First Light owns launch truth; UI owns presentation | Separate bridge |
| Settings → Jukebot | Apply global bus/mute values | Accord persists; Resonance applies | Separate bridge |
| Settings → Input | Store/apply binding override and input preferences | Accord persists; Will validates/applies input data | Separate bridge |
| Settings ↔ UI | Draft/apply/cancel/confirmation presentation | Accord owns transactions; UI presents | Separate bridge |
| SceneFlow ↔ GameState | Loading scope/state requests and result disposal | Passage owns travel; Pulse owns state/time policy | Separate bridge |
| SceneFlow ↔ UI | Fade/loading presenter and progress | Passage owns transition; UI owns visuals | Separate bridge |
| SceneFlow → Jukebot | Project mapping translates route events into audio intent | Resonance owns playback | Project adapter or later bridge |
| GameState → Input | Effective input intent/context/lock request | Pulse owns policy intent; Will applies | Separate bridge |
| GameState → Jukebot | Effective audio pause/mix intent | Pulse owns policy intent; Resonance applies | Separate bridge |
| GameState ↔ UI | Modal/pause/dialogue screen leases and state presentation | Pulse owns state; UI owns screen/modal lifecycle | Separate bridge |
| Input ↔ UI | Navigation, glyphs, rebinding operations, focus-safe locks | Will owns input; UI owns views/focus | Separate bridge |
| Jukebot ↔ UI | UI cue requests, audio status/settings presentation | Resonance owns playback; UI owns views | Separate/project bridge |
| Save ↔ UI | Slot metadata, progress, commands, recovery choices | Chronicle owns save operations; UI owns views | Separate bridge |
| Save ↔ SceneFlow | Prepared load handle, destination metadata, transition, apply | Chronicle owns payload/apply; Passage owns travel | Separate/project adapter |
| Save ↔ GameState | Permission and Loading scope | Chronicle owns save operation; Pulse owns state policy | Separate bridge |
| Every runtime package → Diagnostics | Package snapshot/event mapped into neutral provider | Package owns source status; Observatory owns aggregation | Separate bridge |
| Workshop → every package Editor facade | Setup describe/plan/apply/validate/compare/removal guidance | Peer package owns setup behavior | Exact Editor facade adapter |

### 7.1 Integration specification gate

A bridge must receive its own integration specification before it is advertised as release-ready. Core package M1 skeleton work does not require every bridge to be implemented.

---

## 8. Settings, save, and durable-data boundaries

| Data | Authority/storage | Explicit exclusion |
|---|---|---|
| Master/music/SFX/ambience/UI volume and mute | Accord document; Resonance bridge applies | Not an EchoSave slot payload |
| Display, accessibility, locale, global gameplay preference | Accord | Not duplicated per save unless a game explicitly defines separate save-owned state |
| Input binding overrides and input preferences | Accord or project-supplied global preference backend; Will owns validation/application | Will does not silently choose a second durable file |
| Last selected save slot | Optional Accord setting through bridge | Slot contents remain Chronicle-owned |
| Slot metadata, generations, participant payloads, recovery history | Chronicle | Not stored in Accord or Workshop manifests |
| Project gameplay state | Owning gameplay package/project participant payload | Chronicle does not know concrete game databases |
| Launch report | First Light session diagnostic record | Not a game save or preference document |
| Diagnostic snapshot/export | Observatory bounded support record | Not a save file and never transmitted automatically |
| Workshop transaction journal | `Library/` transient Editor recovery state | Not committed and not runtime data |
| Workshop generation manifest/report | Project-owned Editor documentation/receipt | Not a source of gameplay truth |

### 8.1 Unknown data preservation

- Accord preserves unknown optional-package settings sections.
- Chronicle preserves unknown participant payloads when a package is absent.
- Removing a bridge or optional package must not silently erase durable records that may become readable again after reinstall.
- Schema migration remains with the document owner and, for participant payloads, the participant owner.

---

## 9. Presentation, input, audio, and accessibility boundary

| Concern | Owner | Coordination rule |
|---|---|---|
| EventSystem, focus, screen/modal stack, navigation containment | Looking Glass | May consume Will adapters but must function through its own documented standalone path |
| Input context, locks, rebinding, active device, glyph resolution | Will | UI requests; Pulse supplies policy intent through a bridge |
| Time scale, cursor policy, pause reasons | Pulse | UI cannot directly set global truth; it acquires/disposes scopes through a bridge |
| Music/SFX/ambience sources and mixer application | Resonance | UI/gameplay requests semantic cues; Accord persists volume preferences |
| Scene transition operation and progress | Passage | UI presents progress/fade; it does not activate scenes |
| Launch splash/status | First Light minimal presenter | Looking Glass may replace presentation through a bridge but does not own launch readiness |
| Runtime diagnostics overlay | Observatory presenter | May later be hosted by UI, but Observatory remains usable without UI |
| Reduced motion, flashes, timing, scalable text, contrast | Accord stores preferences; relevant presenter/feedback authority applies | No package assumes color or audio alone communicates critical state |

---

## 10. Diagnostics and identifier namespace

### 10.1 Unique diagnostic prefixes

| Package | Prefix |
|---|---|
| First Light | `ELAUNCH-*` |
| Observatory | `EDIAG-*` |
| Accord | `ESET-*` |
| Passage | `ESF-*` |
| Pulse | `EGSTATE-*` |
| Resonance | `JB-*` |
| Will | `EIN-*` |
| Looking Glass | `EUI-*` |
| Chronicle | `ESV-*` |
| Workshop | `EGS-*` |

The original Pulse `EGS-*` prefix collided with EchoGameStarter. Pulse v1.1.0 changes its package-local identifiers to `EGSTATE-*` before implementation, so no migration of shipped data or external logs is required.

### 10.2 Composite references

Use cases, capabilities, Lab entries, operation IDs, and other locally numbered records may retain concise local IDs. Any cross-package report, manifest, link, or dashboard stores them as:

```text
<package-id>::<local-id>
```

Example:

```text
com.echodevgames.echo-ui::LAB-004
```

This prevents generic `UC-001`, `CAP-001`, or `LAB-001` labels from colliding when aggregated without forcing a large rewrite of approved local registries.

### 10.3 Observatory rule

Every package remains diagnosable alone. An Observatory bridge maps cached/synchronous bounded status into the neutral provider model. The sampler never blocks gameplay awaiting peer work, and diagnostics failure degrades diagnostics rather than the authority being observed.

---

## 11. Workshop setup-facade matrix

SFGSS-ADR-001 resolves the only missing cross-spec Editor endpoint. Current status is documentation-approved, implementation-pending.

| Package | Package-owned Editor setup domains | Facade compatibility at start of implementation |
|---|---|---|
| EchoLaunch | Boot scene, configuration, sequence, presenter, root, destination, direct helper | Manual until facade implemented |
| EchoDiagnostics | Configuration, root, overlay, validation profiles, privacy/export policy | Manual until facade implemented |
| EchoSettings | Configuration, defaults, built-in sections, storage policy, root | Manual until facade implemented |
| EchoSceneFlow | Scene catalog, routes, root, transition/recovery policy | Manual until facade implemented |
| EchoGameState | State definitions, policies, initial state, root, adapters | Manual until facade implemented |
| Jukebot | Configuration, mixer/routing, root, profile templates, laboratory assets | Manual until facade implemented |
| EchoInput | Action asset/template, contexts, glyphs, rebinding policy, root | Manual until facade implemented |
| EchoUI | Root/layers, EventSystem policy, themes, templates, accessibility defaults | Manual until facade implemented |
| EchoSave | Configuration, slot model, root, sandbox/sample participant options | Manual until facade implemented |

Automated Workshop compatibility is earned per package after its facade and adapter tests pass. Package installation and manual package setup remain independent.

---

## 12. Standalone Test Lab and integration evidence matrix

| Package | Standalone proof | Peer packages allowed in standalone proof? | Integration evidence |
|---|---|---:|---|
| First Light | Boot/status/step/failure/report/destination Lab | No | Separate launch bridges |
| Observatory | Runtime overlay/provider/validation/failure Lab | No | Provider bridges per package |
| Accord | Defaults/edit/apply/cancel/display rollback/storage Lab | No | Settings consumer bridges |
| Passage | Isolated multi-scene transition/recovery Lab | No | UI/GameState/Launch/Save bridges |
| Pulse | State/scope/pause/time/cursor policy Lab | No | Input/Audio/UI/SceneFlow bridges |
| Resonance | Audio Laboratory | No | Settings/GameState/UI/Launch bridges |
| Will | Input Laboratory | No | UI/Settings/GameState bridges |
| Looking Glass | UI Laboratory | No | Settings/Input/Save/SceneFlow/etc. presenters |
| Chronicle | Save Laboratory with sandbox backend | No | UI/SceneFlow/GameState/participant bridges |
| Workshop | Editor Laboratory plus disposable clean-project fixtures | No runtime scene required | Real package facade adapters and generated-project tests |

A showcase never substitutes for standalone or bridge proof. Each bridge Integration Lab names both peers and can be removed without damaging either package core.

---

## 13. Removal behavior matrix

| Removal target | Required result |
|---|---|
| Sample/Lab | Core package still compiles and functions; sample data is removable |
| Optional bridge | Both peers return to documented standalone behavior; registrations/leases dispose cleanly |
| One peer with bridge installed | Remove the bridge first or in the same approved package operation; no orphan compile dependency |
| Runtime package | Unrelated packages compile; project-owned assets/data are preserved for explicit cleanup/migration |
| Observatory | Providers/bridges are removed; peer diagnostics continue through standalone surfaces |
| Accord consumer bridge | Consumer uses configured/default runtime values; unknown settings records remain preserved |
| Chronicle participant package | Unknown participant payload remains preserved; package removal does not corrupt slot catalog |
| Workshop | Generated project continues to compile/run; manifest/report remain project-owned documentation |
| Workshop manifest | Removal is explicit; generated assets do not become package-owned again or auto-delete |
| Direct-scene helper | Production Boot path remains canonical; no runtime authority depends on the helper |

The Workshop’s removal guide orders bridge removal before peer removal and classifies generated assets as created, adopted, modified, missing, manual, or blocked.

---

## 14. Findings register

### FND-F-001 — Diagnostic prefix collision

- **Severity:** Blocker before implementation
- **Found:** The Pulse and The Workshop both used `EGS-*`.
- **Resolution:** The Pulse specification advances to v1.1.0 and uses `EGSTATE-*`.
- **Authority changed:** Identifier namespace only. Runtime ownership and MVP behavior are unchanged.
- **Status:** Resolved.

### FND-F-002 — Missing Workshop peer facade contract

- **Severity:** Blocker before automated Workshop setup
- **Found:** Nine peers define setup tools but no exact versioned endpoint for Workshop invocation.
- **Resolution:** SFGSS-ADR-001 establishes exact package-owned static Editor facades with detached JSON protocol, plan/apply handshake, receipts, and manual fallback.
- **Workshop update:** Workshop specification advances to v1.1.0 and records the ADR as the accepted protocol.
- **Status:** Resolved.

### FND-A-001 — Bridge specifications remain future deliverables

- **Severity:** Advisory
- **Meaning:** Core package contracts are coherent, but a bridge is not release-ready until its own integration specification and Integration Lab exist.
- **Effect on First Light M1:** None. Runtime skeleton work can begin after FW-DOC-12.

### FND-A-002 — Exact Unity package versions require M1 verification

- **Severity:** Advisory
- **Meaning:** Unity 6000.0 is the approved floor, while exact compatible released versions for uGUI, TextMeshPro relationships, Input System, and Test Framework must be captured during package skeleton validation.
- **Effect:** No architecture conflict; package manifests must not guess.

### FND-A-003 — First Light presentation assembly separation requires implementation confirmation

- **Severity:** Advisory
- **Meaning:** First Light approves an isolated uGUI presenter but its proposed assembly table currently allows the Runtime assembly to reference uGUI. M1 should choose a compile-safe core/presentation assembly split or document why one Runtime assembly remains sufficiently isolated.
- **Effect:** Does not change First Light’s authority or MVP.

### FND-A-004 — Local registry IDs require package qualification in aggregate tools

- **Severity:** Advisory
- **Meaning:** `UC-*`, `CAP-*`, and `LAB-*` repeat intentionally inside package specifications.
- **Resolution rule:** Workshop, Observatory, reports, and cross-links use `<package-id>::<local-id>`.

---

## 15. FW-DOC-12 readiness input

The collision test provides the following evidence to FW-DOC-12:

- [x] Ten package specifications are approved.
- [x] No duplicate Foundation runtime authority remains.
- [x] No circular core initialization dependency remains.
- [x] All persistent roots reject duplicates before side effects.
- [x] Direct-scene helpers create only their own minimum absent authority.
- [x] Settings and save ownership are coherent.
- [x] Optional bridges and removal direction are explicit.
- [x] The Workshop setup-facade blocker is resolved by ADR.
- [x] Diagnostic namespaces are unique.
- [x] Every package has an honest standalone proof model.
- [x] No release-blocking cross-spec question remains.
- [ ] The checkpoint is committed and pushed.
- [ ] FW-DOC-12 is approved.
- [ ] First Light M1 — Package Skeleton is written as a Checkpoint Build Plan.

**FW-DOC-11 decision:** Approved. Proceed to FW-DOC-12. Runtime implementation remains locked until FW-DOC-12 explicitly authorizes First Light M1.


---

## Graph Navigation

#sfgss/authority #sfgss/navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
