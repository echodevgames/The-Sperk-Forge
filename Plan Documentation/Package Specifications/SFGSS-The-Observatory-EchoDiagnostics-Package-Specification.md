# The Observatory — Diagnostics Package Specification

**Working document ID:** SFGSS-PKG-ECHODIAGNOSTICS-001  
**Specification version:** 1.0.0  
**Status:** Approved  
**Technical package name:** EchoDiagnostics  
**Public title:** The Observatory — Diagnostics  
**Package ID:** `com.echodevgames.echo-diagnostics`  
**Runtime namespace:** `EchoDevGames.EchoDiagnostics`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoDiagnostics`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum public Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 and SFGSS-001  
**Last updated:** August 3, 2026

> “See what the runtime is doing beneath the surface.”

> **Approval rule:** This specification is approved as the authoritative package design. Runtime implementation remains intentionally deferred until the complete Foundation Wave specification pass and its cross-package consistency review are finished.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-03 | Proposed | Initial complete specification derived from SFGSS-000 v0.6.0, SFGSS-001 v1.1.0, and First Light v1.0.0 | Pending |
| 1.0.0 | 2026-08-03 | Approved | Approved diagnostics authority, provider model, overlay, validation, privacy, export, Test Lab, and First Light bridge boundary | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Observatory — Diagnostics  
**Technical identifier:** EchoDiagnostics  
**Flavor line:** See what the runtime is doing beneath the surface.  
**Plain-language subtitle:** Runtime health, validation, structured diagnostics, performance monitoring, and support snapshots.

**One-sentence ownership contract:**

> EchoDiagnostics owns opt-in collection, normalization, validation, local visualization, bounded history, and export of development/runtime diagnostic information; it does not own the behavior being observed, silently repair production state, replace Unity’s Console or Profiler, transmit telemetry, or become a required dependency of another package.

### 1.1 Elevator summary

The Observatory gives a Unity project one coherent place to answer practical debugging questions: which runtime authorities exist, what state they report, what scene and build are active, whether performance counters are available, which warnings occurred recently, and what evidence can be exported for a bug report.

It combines two independent surfaces. The Editor surface validates package setup, scenes, build configuration, duplicate authorities, and known compatibility rules. The runtime surface provides a duplicate-safe optional root, a bounded provider registry, low-overhead metric sampling, categorized recent events, a polished in-game overlay, and explicit local support-snapshot export.

The package works alone. Built-in Unity context/performance providers and Observatory self-health are sufficient for its Standalone Test Lab. Other Sperk’s Forge packages retain standalone diagnostics and connect only through separately installed bridges or project adapters.

### 1.2 Why this belongs in The Sperk’s Forge

Rescuers2D demonstrated how duplicate persistent systems can create overlapping behavior before the root cause is visible. Echo Systems Lab established focused runtime authorities and event-driven state, but its inspection remains project-local. DeverQuest demonstrated the value of readiness reports, categorized issues, repeatable validation, and support-oriented documentation.

The Observatory preserves those strengths without taking ownership away from the source system. A game or package remains authoritative for its own truth; the Observatory receives a safe read-only snapshot or event through an explicit adapter.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | “The Observatory” must be paired with “Diagnostics” on formal surfaces. |
| Setup guidance/tooltips | Yes | Flavor may introduce a section, but action, severity, and remedy remain explicit. |
| Samples | Optional | Observatory imagery must be replaceable and removable. |
| Runtime API/type names | No lore-only names | Types describe providers, snapshots, metrics, validation, events, privacy, and overlay state. |
| Project data | No required Hackulos content | The consuming project owns visuals, toggle commands, labels, and support policy. |

---

## 2. Problem Statement

### 2.1 Current problem

Unity projects often contain useful debugging information in disconnected inspectors, Console messages, temporary text, or manager fields. Common failures include:

1. Persistent authorities duplicate and perform side effects before the conflict is visible.
2. Testers cannot capture current scene, build, authority state, recent warnings, and timing in one artifact.
3. Temporary metric displays allocate, update too frequently, or show false zeroes when counters are unsupported.
4. Setup and pre-build validation are inconsistent across packages.
5. Debug overlays accidentally ship enabled or reveal internal paths and stack details.
6. Packages are tempted to depend on the dashboard merely to be diagnosable.

### 2.2 Evidence from existing work

| Source project | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Persistent bootstrap/audio conflicts and direct-scene testing | Visible state and duplicate protection | Detect/report authority conflicts before behavioral diagnosis |
| Echo Systems Lab | Focused services, events, HUD feedback, modular systems | Explicit state and narrow authorities | Replace project-local debug views with reusable diagnostic contracts |
| DeverQuest | Readiness reports, issue codes, setup/repair, test checklists | Product-grade validation and actionable reports | Separate Editor-product data from runtime diagnostics |
| Don’t Get Vince’d | Different event-rich gameplay architecture | Cross-project proof | Prevent Rescuers2D-specific assumptions |
| Hackulos | Future application shell and large data systems | Package health/support evidence | Keep RPG data and lore outside the general package |
| First Light | Structured launch report/progress/authority status | Immutable report and explicit state | Visualize through a separate bridge without dependency |

### 2.3 Consequences of doing nothing

- Every project builds another temporary FPS/debug panel.
- Runtime failures remain screenshots of scattered Console output.
- Duplicate authorities remain difficult to prove across scene transitions.
- Validation logic is repeated or omitted.
- Public builds risk exposing internal information.
- Later Foundation packages lack a shared target for optional diagnostic bridges.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide an optional duplicate-safe runtime diagnostics authority that persists across scenes when enabled.
- Define stable provider, snapshot, field, metric, event, health, availability, severity, and privacy contracts.
- Require explicit provider registration and clean unregistration rather than reflection discovery.
- Display FPS/frame time, memory, scene/build, authority health, recent events, and launch information when supplied.
- Use supported Unity instrumentation behind adapters and display `Unavailable` rather than fail.
- Provide hidden, compact, and expanded modes without requiring EchoUI or EchoInput.
- Provide package/scene/build validation manually, before Play Mode, and before builds.
- Provide bounded histories, configurable sampling, thresholds, and measurable overhead.
- Export a versioned local diagnostic snapshot only through an explicit action.
- Provide screenshot-safe and player-safe redaction policies.
- Remain useful with no other Sperk’s Forge package installed.

### 3.2 Non-goals

- Replace the Unity Console, Profiler, Memory Profiler, Frame Debugger, Rendering Debugger, or automated tests.
- Own or modify the runtime state reported by another package.
- Silently repair scene objects, configuration, settings, saves, or production state.
- Provide telemetry, analytics, crash upload, cloud storage, or automatic network transmission.
- Promise desktop hardware temperatures, fan speed, or vendor utilization in core.
- Become a cheat console, command shell, save editor, or gameplay administration system.
- Capture or replace every Unity log by default.
- Require another Echo package.
- Pause gameplay, change time scale, seize input, or create a project-wide EventSystem.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean project | Guided setup and an understandable Test Lab without code changes |
| Programmer | Custom authority | Register a bounded read-only provider without transferring ownership |
| Designer | Needs development overlay | Configure safe profile, panels, cadence, thresholds, and visuals |
| Tester | Reproduces bug | Capture build/scene/provider state, warnings, and supported metrics |
| Maintainer | Preparing Play/build | Run stable validation rules with actionable remedies |
| Support reviewer | Receives snapshot | Read schema-versioned, policy-filtered evidence |

### 3.4 Measurable success criteria

- Clean supported project installation produces zero compile errors.
- Runtime core and Lab work with no other Echo runtime package.
- Duplicate root performs no callback, recorder, provider, export, or overlay side effect.
- Unsupported metrics show explicit unavailable states.
- Provider failure affects that provider, not gameplay or the registry.
- Hidden mode produces no recurring managed allocation after warmup in the baseline Lab.
- Histories and event buffers remain fixed-capacity.
- Player-safe mode omits paths, stack traces, sensitive fields, and unrestricted export.
- Setup is repeatable and samples/bridges are removable.
- First Light launch data appears only through a separate bridge.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

Solo developers, small teams, programmers, designers, QA testers, maintainers, and support reviewers working in Unity projects from prototypes through full games.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| UC-001 | Create setup | Installer | Package installed | Project-owned config/root created without overwrite | MVP |
| UC-002 | Validate project | Maintainer | Editor loaded | Package/scene/authority/build/privacy findings with codes | MVP |
| UC-003 | Inspect overview | Tester | Root active | Scene/build, metrics, health, warnings visible | MVP |
| UC-004 | Toggle overlay | Project command | Public API available | Hidden/Compact/Expanded without input ownership | MVP |
| UC-005 | Register provider | Programmer | Registry active | Registration handle and visible provider snapshot | MVP |
| UC-006 | Detect duplicate provider/authority | Tester | Duplicate stable key | Second rejected; one actionable event | MVP |
| UC-007 | Inspect unsupported metric | Tester | Counter unavailable | `Unavailable` with reason | MVP |
| UC-008 | Export snapshot | Tester | Profile permits | Versioned redacted JSON result | MVP |
| UC-009 | Validate before Play | Maintainer | Hook enabled | Blocker may cancel Play; report retained | MVP |
| UC-010 | Validate before build | Maintainer | Build hook enabled | Unsafe release policy blocks build | MVP |
| UC-011 | Visualize First Light | Tester | Separate bridge installed | Launch panel reflects approved public launch state | MVP integration |
| UC-012 | Simulate failures | Tester | Lab imported | Faults, thresholds, duplicates, redaction, export cases | MVP |

### 4.3 Explicitly unsupported use cases

Remote telemetry, automatic crash upload, hidden production activation, runtime data repair, deep profiling replacement, reflection into arbitrary systems, credential/save-content capture, gameplay authorization, or universal hardware sensor coverage.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Optional runtime diagnostics root and lifecycle.
- Explicit provider registry and registration handles.
- Normalized descriptor/snapshot/section/field/metric/event/health/availability/privacy models.
- Built-in Unity context/performance/self-health providers.
- Bounded sampling, histories, event buffer, threshold evaluation, and provider quarantine.
- Observatory-only overlay and profile/redaction/export policy.
- Editor validation framework, Observatory rules, reports, execution hooks, and Echo package inventory.
- Versioned local snapshot creation/export.
- Neutral Launch panel model consumed by a First Light bridge.

### 5.2 The package does not own

Source package truth, runtime repair, general logging policy, global UI/EventSystem/input/pause/time/cursor preferences, save/settings persistence, build deployment, analytics/cloud/crash services, gameplay metrics, or deep Unity profiling.

### 5.3 Neighboring authorities

| Concern | Owner | Interaction |
|---|---|---|
| Initial launch | EchoLaunch | Separate bridge adapts report/state into Launch model |
| Global preferences | EchoSettings | Optional bridge may persist approved overlay preferences |
| Scene transitions | EchoSceneFlow | Bridge publishes state/timings only |
| Runtime state/pause | EchoGameState | Bridge reports state/time scale/reasons |
| Audio | Jukebot | Bridge reports voices/tracks/routing/health |
| Input | EchoInput/project | Invokes overlay API; reports context through bridge |
| General UI | EchoUI | Optional presenter/embed bridge; default stays independent |
| Saves | EchoSave | Sanitized health/path metadata only through bridge |
| Builds | EchoBuildTools | Later validation aggregation bridge |
| Starter composition | EchoGameStarter | Generates/selects Observatory assets visibly |
| Gameplay | Project code | Explicit bounded project adapters |

### 5.4 Boundary tests

A feature belongs here only when it collects, normalizes, validates, displays, briefly retains, or explicitly exports diagnostic evidence; leaves source authority unchanged; remains bounded/privacy-aware; and avoids hidden peer dependencies.

---

## 6. Independence Contract

### 6.1 Standalone guarantees

The package must compile and run with declared Unity dependencies only; provide built-in providers; expose public toggle commands; work without EchoUI/EchoInput; isolate presentation and Editor assemblies; use explicit registration; quarantine faulty providers; support headless operation; label unsupported metrics; and tolerate sample/bridge removal.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Evidence |
|---|---|---|
| Installed alone | Validator and built-in runtime overview function | Clean project/Lab |
| Lab entered directly | One dev root, metrics/context/events visible | PlayMode/Lab |
| Bridge absent | Related panel unavailable; no error | Clean install |
| EchoUI absent | Default presenter works | Lab |
| EchoInput absent | Public API/Lab controls work | API test |
| Duplicate root | Duplicate zero side effects | PlayMode |
| Missing config | Safe dev fallback or disable; release stays safe | Failure test |
| Counter unsupported | Explicit unavailable reason | Provider test |
| Provider throws | Fault/quarantine; other providers/game continue | Isolation test |
| Presenter absent | Headless registry/snapshot remains | Headless test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum | Reason |
|---|---|---:|---|---|
| Unity Engine core/runtime | Platform | Yes | 6000.0 | Runtime, scenes, serialization, time |
| Unity Profiling/CoreModule | Platform | Yes for metrics | Included | `ProfilerRecorder` adapter |
| uGUI | Presentation | Yes for default presenter | Baseline-compatible | Standalone overlay |
| TextMeshPro | Presentation | Yes for default presenter | Baseline-compatible | Readable scalable text |
| UnityEditor Package Manager API | Editor only | Yes | Included | Installed package inventory |
| Unity Test Framework | Test only | Yes | Baseline-compatible | EditMode/PlayMode tests |

### 6.4 Forbidden dependencies

Other Echo runtime packages in core, project assemblies, sample/test/Editor references from runtime, hard-coded keys/scenes/tags/layers/resources, required global EventSystem, reflection peer discovery, native sensor libraries, telemetry/cloud/network SDKs, and unlicensed content.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Status | MVP? | Surface |
|---|---|---|---:|---|
| CAP-001 | Duplicate-safe application-session diagnostics root | Approved | Yes | Runtime |
| CAP-002 | Immutable runtime/profile/privacy/threshold/panel configuration | Approved | Yes | Runtime/Editor |
| CAP-003 | Explicit provider registry with stable IDs/handles | Approved | Yes | Runtime |
| CAP-004 | Immutable normalized provider snapshots | Approved | Yes | Runtime |
| CAP-005 | Bounded categorized diagnostic events | Approved | Yes | Runtime |
| CAP-006 | Runtime context provider | Approved | Yes | Runtime |
| CAP-007 | ProfilerRecorder-backed performance provider | Approved | Yes | Runtime |
| CAP-008 | Fixed metric histories, thresholds, hysteresis | Approved | Yes | Runtime |
| CAP-009 | Standard authority health model | Approved | Yes | Runtime |
| CAP-010 | Hidden/Compact/Expanded overlay | Approved | Yes | Presentation |
| CAP-011 | Neutral Launch panel model | Approved | Yes | Runtime/Presentation |
| CAP-012 | Player-safe/screenshot-safe filtering | Approved | Yes | Runtime/Presentation |
| CAP-013 | Versioned in-memory diagnostic snapshot | Approved | Yes | Runtime |
| CAP-014 | Explicit local JSON export | Approved | Yes | Runtime/Editor |
| CAP-015 | Editor validator window | Approved | Yes | Editor |
| CAP-016 | Manual/pre-Play/pre-build validation hooks | Approved | Yes | Editor |
| CAP-017 | Editor Echo package inventory | Approved | Yes | Editor |
| CAP-018 | Duplicate root/provider/explicit authority checks | Approved | Yes | Runtime/Editor |
| CAP-019 | Lab failure/threshold/redaction simulations | Approved | Yes | Sample/Test |
| CAP-020 | Observatory self-health | Approved | Yes | Runtime |
| CAP-021 | Separate First Light bridge and Integration Lab | Approved | Yes, integration | Bridge |
| CAP-022 | Optional filtered Unity log callback capture | Deferred minor | No | Runtime |
| CAP-023 | Custom panel renderer SDK | Deferred | No | Presentation |
| CAP-024 | Generated Player package manifest | Deferred | No | Editor/Runtime |
| CAP-025 | UI Toolkit presenter | Deferred | No | Presentation |
| CAP-026 | Native hardware sensor providers | Deferred | No | Adapter |
| CAP-027 | Remote telemetry/support service | Rejected from core | No | Separate product |

### 7.2 MVP capability set

One protected root; provider registry; context/performance/self-health/event providers; bounded histories/buffers; uGUI/TMP overlay; Overview, Performance, Runtime Context, Authorities, Launch, Providers, Events, and Self-Health panels; safe modes; versioned JSON export; Editor validator/package inventory/build gates; isolated Lab; and separately packaged First Light bridge.

### 7.3 Later capability set

Filtered Unity log capture, custom renderers, UI Toolkit, generated runtime package manifest, screenshot/compressed support bundles, extra Unity subsystem providers, native sensor adapters, BuildTools aggregation, and settings/UI bridges.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason |
|---|---|---|
| Full Profiler replacement | Rejected | Duplicates Unity tooling and explodes scope |
| Automatic runtime repair | Rejected | Diagnostics must not mutate truth |
| Automatic telemetry upload | Rejected | Privacy/security/operations boundary |
| Hidden production hotkey | Rejected | Unsafe and unauditable |
| Reflection manager discovery | Rejected | Fragile hidden coupling |
| Unbounded history | Rejected | Memory/privacy risk |
| Global Unity logger replacement | Rejected | Behavior/recursion risk |
| Hardware sensors in core | Deferred | Native/vendor/platform burden |
| Runtime installed-package scan | Deferred | Requires generated build manifest |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | Configuration, profiles, thresholds, panels, redaction, counter catalog | Active providers, samples, histories, scene objects |
| Runtime state/behavior | Root, registry, sampler, histories, event buffer, snapshot/export, self-health | Editor APIs, general UI/input/pause, source authority |
| Presentation | Observatory overlay, graphs, tables, export controls | Source mutation, project screen stack, global input ownership |
| Editor tooling | Setup, validators, package inventory, Play/build hooks, snapshot viewer | Player runtime code or silent production repair |

### 8.2 Component topology

```text
Project/bridge providers ─┐
Built-in Unity providers ─┼─> DiagnosticProviderRegistry ─> immutable snapshots
Observatory self-provider ┘                    │
                                               ├─> DiagnosticSampler ─> fixed histories
Structured event publishers ──────────────────> DiagnosticEventBuffer
                                               │
                                               ├─> DiagnosticSnapshotBuilder ─> explicit export
                                               └─> IDiagnosticOverlayPresenter ─> uGUI/TMP overlay

Editor rules ─> DiagnosticValidator ─> Validator Window
                                   ├─> pre-Play policy
                                   └─> pre-build policy
```

The root owns all runtime children. Registry, sampler, providers, buffers, exporter, and presenter are ordinary owned objects, not independent persistent singletons.

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Persistent root required? | Runtime session/overlay: yes when enabled. Editor validation: no. |
| Root type | `EchoDiagnosticsRoot` |
| Default lifetime | `ApplicationSession` |
| Duplicate behavior | First valid root claims before callbacks, recorders, providers, export, or presenter side effects; duplicates are rejected |
| Initialization trigger | `Awake` claim, then explicit owned initialization from configuration |
| Shutdown | Stop/dispose recorders, unsubscribe callbacks, unregister providers, clear static access, release presenter/buffers |
| Direct-scene behavior | Dev helper creates configured root only when absent; release-disabled by default |
| Test seams | Registry, provider, clock, counter source, exporter, presenter, and validation-rule interfaces |

`EchoDiagnosticsRoot.Current` may exist as convenience access, but dependency injection through narrow interfaces remains supported.

### 8.4 Provider and capture model

1. `IDiagnosticProvider` exposes an immutable descriptor and synchronous `CaptureSnapshot`.
2. Registration is explicit and returns an idempotent disposable handle.
3. Sampling occurs only at configured cadence or coalesced refresh request.
4. Capture is non-blocking and budgeted. Async sources cache their authoritative current state and return immediately.
5. Exceptions become faulted provider snapshots/events and may quarantine or throttle only that provider.
6. Duplicate stable provider IDs are rejected.
7. Published snapshots contain immutable DTOs, not mutable Unity objects.
8. The Observatory may render/export a snapshot but cannot change the provider’s source state.

### 8.5 Standard vocabulary

| Concept | Values/purpose |
|---|---|
| `DiagnosticAvailability` | `Available`, `Unavailable`, `Disabled`, `Restricted`, `Unknown` |
| `DiagnosticHealth` | `Unknown`, `Healthy`, `Degraded`, `Faulted`, `Blocked`, `Disabled` |
| `DiagnosticSeverity` | `Trace`, `Info`, `Warning`, `Error`, `Blocker` |
| `DiagnosticPrivacy` | `Public`, `ProjectInternal`, `Sensitive` |
| `DiagnosticProviderKind` | `Overview`, `Performance`, `RuntimeContext`, `Authority`, `Launch`, `EventStream`, `Custom` |
| `DiagnosticValueKind` | Text, bool, integer, decimal, duration, bytes, percentage, identifier, version, timestamp |

Health describes current condition. Severity describes an event/finding. Availability describes whether a value exists and may be shown.

### 8.6 Lifecycle sequence

1. Claim authority before side effects.
2. Resolve configuration/profile and build visibility.
3. Validate presenter, capacities, sampling, privacy, and export policy.
4. Create registry, buffers, histories, snapshot service, and self-health.
5. Register built-in providers and supported counters.
6. Subscribe approved callbacks.
7. Bind/create presenter when allowed.
8. Sample and render at separate bounded cadences.
9. Capture/export only by explicit request and active privacy policy.
10. Shutdown cleanly and dispose all owned resources.

### 8.7 Failure model

| Failure | Detection | Result | Fallback | Code |
|---|---|---|---|---|
| Duplicate root | Claim | One actionable warning/error | Duplicate zero side effects | EDIAG-ROOT-001 |
| Missing config | Init/Editor | Dev warning; validator finding | Safe dev fallback or disabled by build policy | EDIAG-CFG-001 |
| Unsupported config schema | Init/Editor | Blocker | Diagnostics disabled; game continues | EDIAG-CFG-002 |
| Invalid capacities/intervals | Validation | Error/remedy | Safe clamp only when explicit; otherwise feature disabled | EDIAG-CFG-003 |
| Duplicate provider ID | Registration | Second rejected | Existing provider remains | EDIAG-PROV-001 |
| Provider exception | Sampling | Provider faulted | Quarantine/throttle; others continue | EDIAG-PROV-002 |
| Slow provider | Sampling | Self-health warning | Throttle/reduce cadence | EDIAG-PROV-003 |
| Counter unavailable | Provider init | `Unavailable` row/reason | Other metrics continue | EDIAG-METRIC-001 |
| Recorder failure | Init/runtime | Performance degraded | Dispose failed recorder | EDIAG-METRIC-002 |
| Presenter missing/faulted | Init/render | Headless warning | Registry/snapshot/export continue | EDIAG-VIEW-001 |
| No EventSystem | Presentation | Read-only warning | Non-interactive overlay | EDIAG-VIEW-002 |
| Export prohibited/unavailable | Request | Structured failure | In-memory capture if allowed | EDIAG-EXPORT-001 |
| Export write failure | Request | Error result | No partial file treated as success | EDIAG-EXPORT-002 |
| Validation rule throws | Editor | Rule failure shown | Remaining rules continue | EDIAG-VAL-001 |
| Unsafe release visibility | Pre-build | Build blocker | Build stops until resolved | EDIAG-BUILD-001 |
| Sensitive field in safe mode | Capture/view | Omitted/redacted | Sanitized output continues | EDIAG-PRIV-001 |

Runtime diagnostics failures degrade/disable diagnostics, never gameplay. Editor build validation may intentionally block an unsafe build.

### 8.8 Unity integration basis

The performance provider uses Unity’s supported `ProfilerRecorder` API behind an Observatory-owned adapter. Counter names and availability are validated per Unity version/platform; unsupported counters become explicit unavailable values. Editor package inventory uses `UnityEditor.PackageManager.PackageInfo`. Pre-build validation uses `IPreprocessBuildWithReport`. Optional later log capture uses `Application.logMessageReceived` with recursion, privacy, and capacity controls.

Implementation references:

- [Unity ProfilerRecorder](https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.html)
- [Unity Profiler counters](https://docs.unity3d.com/Manual/profiler-counters-reference.html)
- [Unity PackageInfo](https://docs.unity3d.com/ScriptReference/PackageManager.PackageInfo.html)
- [Unity log callback](https://docs.unity3d.com/ScriptReference/Application-logMessageReceived.html)
- [Unity pre-build callback](https://docs.unity3d.com/ScriptReference/Build.IPreprocessBuildWithReport.html)

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Runtime mutable? | Project-owned? |
|---|---|---:|---:|---:|
| `EchoDiagnosticsConfiguration` | Root configuration and profile/presenter references | Yes | No | Yes |
| `DiagnosticRuntimeProfile` | Build visibility, cadence, capacities, privacy, export, overlay policy | Yes | No | Yes |
| `DiagnosticThresholdProfile` | Warning/error thresholds and hysteresis | Yes | No | Yes |
| `DiagnosticPanelProfile` | Panel visibility/order/compact summary/refresh | Yes | No | Yes |
| `DiagnosticRedactionProfile` | Privacy ceiling, path/stack/detail/screenshot/export rules | Yes | No | Yes |
| `DiagnosticCounterCatalog` | Metric descriptors and candidate Unity counter mappings | Yes | No | Package template/project override |
| `DiagnosticPresenterConfiguration` | Visual/layout references | Optional | No | Yes |

Runtime overlay mode and temporary filters are session state. Persistence requires a later EchoSettings bridge.

### 9.2 Runtime state

| State | Owner | Lifetime | Reset | Serialization |
|---|---|---|---|---|
| `DiagnosticSession` | Root | Application/root session | New root | Snapshot DTO only |
| `DiagnosticProviderRegistry` | Root | Session | Dispose/shutdown | No |
| `DiagnosticRegistration` | Registry/caller | Registration | Dispose/unregister | No |
| `DiagnosticProviderSnapshot` | Registry | Until replaced | Immutable replacement | Export optional |
| Metric histories | Sampler/history store | Session | Fixed ring reset | Export by policy |
| Event buffer | Root | Session | Fixed ring reset | Export by policy |
| Overlay state | Presenter/controller | Session | Profile default | No MVP persistence |
| Self-health | Root | Session | New root | Exported |
| `DiagnosticSnapshot` | Snapshot service/caller | Immutable artifact | New capture | Versioned JSON |
| Profiler recorder handles | Performance provider | Session | Dispose | Never |

### 9.3 Stable identifiers

- Provider IDs are lowercase dot-delimited strings, normally package/project-prefixed, such as `com.echodevgames.echo-diagnostics.performance`.
- Multi-instance providers append an approved stable instance segment when comparison matters.
- Section/field IDs are stable within the provider.
- Diagnostic codes follow uppercase `<PACKAGE>-<DOMAIN>-<NNN>`.
- Display names remain separate and replaceable.
- Empty, invalid, or duplicate IDs are rejected.
- Released IDs/codes are never reused for a different meaning; aliases/migration notes cover renames.
- Exports never depend on transient Unity instance IDs or live object references.

### 9.4 ScriptableObject safety

Configuration assets remain immutable during Play Mode. Active providers, samples, histories, threshold state, overlay state, export paths, and last results live in runtime objects. Runtime use must not dirty assets.

### 9.5 Serialization and migration

- Snapshot schema starts at version `1`, independent from package SemVer.
- JSON contains plain DTOs, stable IDs, units, timestamps, health/availability/privacy outcomes, and approved build/package metadata.
- Unity object references, credentials, save payloads, raw assets, and arbitrary binary data are excluded.
- Unknown future fields should be ignored where practical.
- Breaking schema changes receive a major schema version and reader/migration notes.
- Configuration schemas use explicit Editor migration with preview/backup; downgrade is not promised.
- Snapshots are support artifacts, not EchoSave data.

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility |
|---|---|---|
| `EchoDiagnosticsRoot` | MonoBehaviour | Runtime authority/lifecycle |
| `EchoDiagnosticsConfiguration` | ScriptableObject | Project configuration entry |
| `DiagnosticRuntimeProfile` | ScriptableObject | Visibility/performance/privacy/export policy |
| `IDiagnosticRegistry` | Interface | Narrow provider registration/query |
| `DiagnosticProviderRegistry` | Class | Register/reject/sample/remove providers |
| `IDiagnosticProvider` | Interface | Descriptor plus bounded synchronous capture |
| `DiagnosticProviderDescriptor` | Immutable data | ID/source/kind/version/privacy/order |
| `DiagnosticCaptureContext` | Read-only data | Time/detail/privacy/reason/budget |
| `DiagnosticProviderSnapshot` | Immutable data | Availability/health/summary/sections/metrics/fields |
| `DiagnosticSection` | Immutable data | Stable group of values |
| `DiagnosticField` | Immutable data | Typed labeled value/unit/health/privacy/code |
| `DiagnosticMetric` | Immutable data | Numeric value/unit/availability/threshold/health |
| `DiagnosticEvent` | Immutable data | Time/source/code/severity/message/subsystem/scene/privacy |
| `IDiagnosticEventSink` | Interface | Publish structured bounded events |
| `DiagnosticRegistration` | Disposable handle | Own one provider registration |
| `DiagnosticSnapshot` | Immutable data | Versioned support capture |
| `DiagnosticSnapshotRequest` | Immutable data | Detail/history/event/privacy/export choices |
| `DiagnosticExportResult` | Immutable result | Success/code/path identifier/bytes/message |
| `IDiagnosticSnapshotExporter` | Interface | Export prepared snapshot |
| `IDiagnosticOverlayPresenter` | Interface | Render diagnostics without owning truth |
| `DiagnosticOverlayMode` | Enum | Hidden/Compact/Expanded |
| `DiagnosticAvailability` | Enum | Availability vocabulary |
| `DiagnosticHealth` | Enum | Health vocabulary |
| `DiagnosticSeverity` | Enum | Event/finding severity |
| `DiagnosticPrivacy` | Enum | Data sensitivity |
| `DiagnosticProviderKind` | Enum | Presentation category |
| `DiagnosticAuthoritySnapshot` | Model | Authority identity/lifecycle/duplicate/last result |
| `LaunchDiagnosticSnapshot` | Model | Neutral launch phase/step/progress/timing/result |
| `IDiagnosticClock` | Interface | Monotonic sampling/test time |
| `IDiagnosticCounterSource` | Interface | Start/read/dispose counters |
| `EchoDiagnosticsDirectSceneInitializer` | MonoBehaviour | Development-only root creation |

### 10.2 Public members

| Member | Purpose | Result/rule |
|---|---|---|
| `EchoDiagnosticsRoot.Current` | Convenience root access | Null when absent; not only test seam |
| `State`, `Registry`, `Events`, `OverlayMode` | Read-only/narrow runtime surfaces | Main-thread immutable access |
| `SetOverlayMode(mode)` / `ToggleOverlay()` | Project-command visibility | No input/pause ownership; structured result |
| `RequestRefresh(providerId)` | Coalesced safe refresh | Unknown/quarantined returns failure |
| `IDiagnosticRegistry.Register(provider)` | Explicit registration | Returns result plus handle; duplicates rejected |
| `DiagnosticRegistration.Dispose()` | Unregister | Idempotent |
| `IDiagnosticProvider.CaptureSnapshot(context)` | Return current bounded snapshot | Synchronous/non-blocking; exceptions isolated |
| `IDiagnosticEventSink.Publish(event)` | Submit event | Filters/redacts/caps/recursion guards |
| `CaptureSnapshot(request)` | Build in-memory artifact | Immutable result or structured failure |
| `ExportSnapshotAsync(request, cancellation)` | Explicit JSON export | `Awaitable<DiagnosticExportResult>` |
| `EnsureDevelopmentDiagnostics()` | Direct-scene create/reuse | Editor/dev only by default |

### 10.3 Events

`AuthorityClaimed`, `DiagnosticsStateChanged`, `ProviderRegistered`, `ProviderUnregistered`, `ProviderSnapshotUpdated`, `DiagnosticEventPublished`, `OverlayModeChanged`, `SnapshotCaptured`, and `SnapshotExportCompleted` are raised only after authoritative Observatory state changes. Presentation listeners are never required for completion.

### 10.4 Async and cancellation policy

Provider capture is synchronous. Export uses Unity `Awaitable`; immutable non-Unity DTOs are captured on main thread before background serialization/write. Background work cannot call Unity APIs. Cancellation never reports a partial file as success. MVP allows one concurrent export. Refresh requests are coalesced. Quit cancels pending export and disposes owned resources.

### 10.5 API ergonomics

**Novice:** Setup, choose Development Only/Player Safe, add root, open Lab, use UI controls.  
**Programmer:** Implement/register `IDiagnosticProvider`, publish structured events, classify privacy, and inject fake clocks/counters/exporters/presenters in tests.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install package.
2. Open **Tools > Sperk’s Forge > The Observatory > Setup**.
3. Select project-owned configuration location.
4. Choose **Development Only**, **Player Safe**, or **Custom**.
5. Review cadence, capacities, panels, privacy, export, presenter.
6. Preview changes.
7. Create/repair assets and root placement.
8. Run Validator and resolve blockers.
9. Open Lab.
10. Validate normal, unavailable, warning, fault, duplicate, redaction, and export cases.

### 11.2 Setup operations

| Operation | Creates/modifies | Repeat-safe? | Protection/report |
|---|---|---:|---|
| Create configuration/profiles | New project assets | Yes | Conflict prompt, Undo, paths/IDs |
| Create root prefab | New prefab | Yes | Undo, GUID/path report |
| Add root to scene | Selected scene only | Yes | Preview, Undo, scene dirty prompt |
| Repair references | Explicit selected fields | Yes | Preview/backup when needed |
| Create player-safe profile | New asset | Yes | Redaction/visibility summary |
| Configure validation hooks | Project Editor settings | Yes | Policy report |

No silent overwrite of project-authored assets or scenes.

### 11.3 Inspectors and windows

Observatory Setup, Validator, Runtime Profile Inspector, Threshold Profile Inspector, Panel Profile Inspector, Snapshot Inspector, and Provider Simulator. All are Editor-only.

### 11.4 Validation framework

`IDiagnosticValidationRule` implementations return immutable results. Rule exceptions are isolated. Modes are manual, optional pre-Play, and pre-build through `IPreprocessBuildWithReport`. Validation reports but never silently repairs; safe fixes require explicit invocation.

### 11.5 Validation registry

| Check | Condition | Severity |
|---|---|---|
| EDIAG-VAL-001 | Missing configuration | Error/Blocker |
| EDIAG-VAL-002 | Unsupported config schema | Blocker |
| EDIAG-VAL-003 | Invalid cadence/capacity | Error |
| EDIAG-VAL-004 | Multiple Observatory roots | Error/Blocker |
| EDIAG-VAL-005 | Enabled diagnostics missing presenter | Warning/Error |
| EDIAG-VAL-006 | Interactive overlay lacks EventSystem | Warning |
| EDIAG-VAL-007 | Player-safe profile exposes internal/sensitive detail | Blocker |
| EDIAG-VAL-008 | Release allows unsafe export/log capture | Blocker/Warning |
| EDIAG-VAL-009 | Invalid/duplicate IDs | Error |
| EDIAG-VAL-010 | Invalid built-in counter descriptor | Error |
| EDIAG-VAL-011 | Package compatibility finding | Warning/Error |
| EDIAG-VAL-012 | Direct helper enabled for release | Warning/Blocker |
| EDIAG-VAL-013 | Unsafe/unavailable export destination | Warning/Error |
| EDIAG-VAL-014 | Overlay visible by default in public build | Blocker |
| EDIAG-VAL-015 | First Light bridge version mismatch | Error |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

Embedded, local path, local tarball, Git URL after release, and Workshop selection. Samples remain optional.

### 12.2 Minimal scene setup

One configuration, one root, optional default/alternate presenter, a project command calling overlay methods, and a project EventSystem only when interactive controls are desired. Headless use omits presenter.

### 12.3 Boot/preload setup

Place one root in Boot/preload and persist for the application session when profile-enabled. EchoLaunch is not required. If both exist, creation may be scene-based or an explicit bridge/startup step; duplicate protection remains authoritative.

### 12.4 Direct-scene setup

`EchoDiagnosticsDirectSceneInitializer` checks for an existing root, creates only Observatory when absent, records development mode, and is disabled in release by default.

### 12.5 Scene isolation

The Lab includes no other Echo package or project runtime assembly. Mock providers and controls live in sample/test assemblies.

---

## 13. Standalone Test Lab and Samples

### 13.1 Purpose

Enter one scene directly, create/reuse one dev root, inspect all overlay modes, supported/unavailable metrics, health/events/thresholds/faults/duplicates, safe-mode redaction, export, reset, and repeat without peers.

### 13.2 Required contents

Instructions, test config, default presenter, sample-local EventSystem, mode/refresh/export/safe/reset controls, built-in providers, mock health/authority/launch providers, bounded stress controls, severity/privacy event publishers, provider fault/slow simulations, fake unavailable counters, export success/failure/cancel simulations, and self-health readout.

### 13.3 Acceptance checklist

| ID | Action | Expected |
|---|---|---|
| LAB-001 | Enter directly | One dev root and overview |
| LAB-002 | Spawn duplicate | Rejected before side effects |
| LAB-003 | Cycle modes | Hidden/Compact/Expanded; no pause |
| LAB-004 | Register healthy provider | Appears once healthy |
| LAB-005 | Duplicate provider ID | Second rejected |
| LAB-006 | Dispose twice | One removal, no exception |
| LAB-007 | Throw capture | Provider faulted; others continue |
| LAB-008 | Slow provider | Warning/throttle |
| LAB-009 | Counter unavailable | Explicit unavailable |
| LAB-010 | Cross threshold | Health/event with hysteresis |
| LAB-011 | Fill event buffer | Fixed capacity/oldest rollover |
| LAB-012 | Screenshot safe | Internal/sensitive redacted |
| LAB-013 | Player safe | Path/stack/export policy enforced |
| LAB-014 | Export | Valid versioned JSON |
| LAB-015 | Cancel/fail export | No false success/partial final file |
| LAB-016 | Remove presenter | Headless diagnostics continue |
| LAB-017 | Reset | Known baseline, no asset mutation |
| LAB-018 | Domain reload disabled cycle | Static state resets |
| LAB-019 | Delete sample | Core compiles |

### 13.4 Integration samples

First Light + Observatory Integration Lab, a later Foundation Shell Showcase, and a project-adapter example. None substitutes for standalone proof.

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The package owns only its diagnostics overlay. Default uGUI/TextMeshPro presentation lives in an isolated assembly behind `IDiagnosticOverlayPresenter`. It does not own a global EventSystem, UI root, input, pause, modal stack, or notifications.

### 14.2 Required modes/states

Hidden, Compact, Expanded; initializing, ready/no peers, healthy, degraded, faulted, blocked, unavailable, disabled/restricted, empty events, export busy/success/failure/cancel, player-safe, screenshot-safe, and headless.

MVP panels: Overview, Performance, Runtime Context, Authorities, Launch, Providers, Recent Events, Self-Health.

### 14.3 Accessibility

Text/icon plus color, numeric graph alternatives, configurable text/panel scale, readable contrast, minimal/disableable motion, no audio dependency, public command methods, optional keyboard/controller navigation through project EventSystem, no automatic pause/input seizure, explicit safe-mode indicator, and unavailable values never shown as zero.

### 14.4 Visual customization

Fonts, sizes, spacing, icons, backgrounds, graph appearance, opacity, anchors, compact fields, panel order, and labels are project-owned/replaceable without changing diagnostic truth.

---

## 15. Diagnostics and Observability

### 15.1 Self-diagnostics

Root/profile/build mode, provider counts/IDs/kinds, capture durations, slow/quarantined providers, counter availability, buffer capacity/use, dropped/coalesced refreshes, presenter/render state, last export, validation summary, and package/schema version.

### 15.2 Structured status

Package/schema version, root identity/lifecycle/mode, config/profile IDs, presenter/mode, provider health distribution, sampling/render cadence, capture duration/budget, buffer use, unavailable/faulted metrics/providers, scene/build/platform context after filtering, last export, and active privacy ceiling.

### 15.3 Diagnostic codes

| Code | Meaning |
|---|---|
| EDIAG-ROOT-001 | Duplicate root rejected |
| EDIAG-CFG-001/002/003 | Missing, unsupported, or invalid configuration |
| EDIAG-PROV-001/002/003/004 | Duplicate ID, fault, slow provider, registration leak |
| EDIAG-METRIC-001/002 | Counter unavailable or recorder failure |
| EDIAG-EVENT-001 | Invalid/rejected/recursive event |
| EDIAG-VIEW-001/002 | Presenter failure or missing EventSystem |
| EDIAG-EXPORT-001/002 | Export unavailable/prohibited or write failed |
| EDIAG-VAL-001 | Validation rule failure |
| EDIAG-BUILD-001 | Unsafe release configuration |
| EDIAG-PRIV-001 | Field redacted by policy |
| EDIAG-LIFE-001 | Session interrupted during teardown |

### 15.4 Observatory bridge

Not applicable as an external dependency because this package is the Observatory. It defines the provider target used by bridges. A bridge references both packages, translates public standalone status, registers explicitly, and unregisters cleanly. The peer never references EchoDiagnostics. Observatory self-health uses the same provider path.

### 15.5 Logging policy

Stable codes, no per-frame logging, deduped repeated faults, bounded event buffer, no logger replacement, optional log capture deferred and default-off, release redaction, and recursion prevention.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Configuration/profiles | Project asset | Project/EchoDiagnostics schema | Yes | Unity assets |
| Provider registry | Session | Root | No | Memory |
| Metric histories | Session | History store | No | Preallocated memory |
| Recent events | Session | Event buffer | No | Preallocated memory |
| Overlay mode/filter | Session | Presenter/controller | No in MVP | Memory |
| Diagnostic snapshot | Support artifact | Requester | Explicit export only | Versioned local JSON |
| Editor window convenience | Editor | Editor tool | Optional | EditorPrefs/project settings |
| Validation settings | Project Editor config | Project | Yes | Project asset |

### 16.2 Standalone behavior

EchoSave and EchoSettings are not required. Histories reset per session, overlay starts from profile defaults, and export is explicit rather than autosave. A later EchoSettings bridge may persist approved overlay preferences.

### 16.3 Optional participant/provider contract

No EchoSave participant exists in MVP. Peer bridges may report sanitized persistence health/metadata, but no save payload contents are captured by default.

### 16.4 Failure and recovery

Configuration failures use validation and safe disable/fallback, not save recovery. Failed exports do not affect runtime state. Partial files are temporary/non-successful. Full ring buffers intentionally overwrite oldest entries. Faulted providers may be removed/re-registered after source correction.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Connections are explicit, removable, versioned, and observational. The source package owns its state and standalone diagnostics. A bridge converts public source status into Observatory models. The Observatory never reaches into peer internals.

### 17.2 Planned integrations

| Authority | Connection | Direction/data | Required? |
|---|---|---|---:|
| EchoLaunch | Separate bridge | Launch authority/mode/phase/step/progress/timings/report → Launch panel | No; MVP integration deliverable |
| EchoSettings | Bridge/project adapter | Settings health; optional overlay preference persistence | No |
| EchoSceneFlow | Separate bridge | Transition state, queue/lock, destination, progress, timing/failure | No |
| EchoGameState | Separate bridge | Runtime state, pause reasons, time scale, coordination status | No |
| Jukebot | Separate bridge | Tracks, voices, pools, routing, warnings, health | No |
| EchoInput | Bridge/project adapter | Context/device/locks plus project overlay command | No |
| EchoUI | Presenter/embed bridge | Diagnostic view models into project UI | No |
| EchoSave | Separate bridge | Backend/slot/migration/recovery health and sanitized path | No |
| EchoBuildTools | Separate bridge | Validator/build-gate aggregation | No |
| EchoGameStarter | Editor integration | Generated config/root/profile/setup report | No |
| Project systems | Project adapter | Bounded public snapshots/events | No |

### 17.3 Bridge placement

Two-package Echo bridges ship separately. First Light bridge is separate. Project-specific providers live in project code. Native/platform sensor providers are separate. The Workshop lists every selected artifact even when presenting one checkbox.

### 17.4 Integration failure behavior

Missing peer means bridge absent; missing Observatory leaves source unaffected; version mismatch produces Editor validation; duplicate provider ID rejects second; absent source reports unavailable; teardown unregisters/subscription-cleans; provider fault affects bridge only; stricter privacy policy wins.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

Targets must be measured before release claims.

| Metric | Target | Release threshold |
|---|---|---|
| Hidden CPU | <0.20 ms/frame average at default 4 Hz | No sustained >0.30 ms without documented reason |
| Compact CPU | <0.40 ms/frame at 60 FPS | No sustained >0.60 ms |
| Expanded CPU | <0.80 ms/frame at 60 FPS/default panels | No sustained >1.20 ms |
| Steady-state allocation | Zero recurring managed allocation hidden; zero/near-zero visible target | No unbounded/per-frame pattern |
| Default sampling | 4 Hz, configurable 1–10 Hz | Bounded and validated |
| Provider capture | <0.25 ms average/provider; <1.5 ms total sample | Slow provider flagged/throttled |
| Event capacity | 500 | Fixed |
| Metric history | 120 samples/metric | Fixed/preallocated |
| Buffer/presenter memory | <4 MB excluding fonts/UI assets | No unbounded growth |
| Export | Explicit operation | No long undocumented main-thread stall |

### 18.2 Allocation policy

Fixed ring buffers, reusable UI rows/graph points, no unmeasured LINQ/reflection in hot paths, cached/deferred formatting, fault-only detail capture, and explicit-operation export allocations only.

### 18.3 Scene/domain reload behavior

Persistent root survives scenes; scene provider follows authoritative scene events; owners unregister; static state resets with domain reload disabled; recorders dispose; callbacks unsubscribe once; presenter does not duplicate; direct helper reuses existing root.

### 18.4 Scalability limits

MVP defaults: 64 providers, 32 values/provider before truncation policy, 500 events, 64 histories, 120 samples/history, one export, and 10 Hz maximum normal profile. Limits reject/truncate/aggregate/roll over with one bounded finding rather than unbounded growth.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Potential internal data includes scene names, package/build versions, build IDs, paths, exceptions, stack traces, provider IDs, and sanitized save/settings path metadata. Credentials, tokens, personal data, save payloads, chat, network payloads, and analytics identifiers are not intentionally collected.

### 19.2 Trust boundaries

Providers are trusted code but untrusted for performance, stability, and privacy. Registry validates/catches/budgets and applies privacy ceiling. Every value/event declares privacy; missing class defaults to `ProjectInternal`. Player-safe and screenshot-safe modes filter. Export is explicit/local. Imported JSON is untrusted, size/depth limited, and never instantiates arbitrary types. Snapshots are evidence, not gameplay authorization.

### 19.3 Build visibility profiles

| Profile | Overlay | Internal/sensitive detail | Export | Log capture |
|---|---|---|---|---|
| Development Only | Allowed | Internal; sensitive only explicitly | Explicitly allowed | Deferred/default-off |
| Player Safe | Hidden by default; allowed panels only | Redacted/omitted | Disabled or sanitized explicit | Disabled |
| Disabled in Release | Disabled | None | Disabled | Disabled |
| Custom | Explicit policy | Cannot exceed approved privacy ceiling | Validated | Later feature only |

Unsafe release combinations are build blockers.

### 19.4 Platform behavior

| Platform | Status | Special behavior |
|---|---|---|
| Windows | Initial baseline | Full overlay/local export after tests |
| macOS/Linux | Planned MVP claim after validation | Counter/path/permission variation |
| WebGL | Core/overlay planned | File export limited; unsupported counters explicit |
| Mobile | Planned after devices | Safe area, lifecycle, touch/project commands |
| Headless/server | Core/snapshot possible | No default presenter |
| Console | Unknown/later | Certification/debug restrictions |

No platform claim is made before its gate passes.

---

## 20. Package and Repository Structure

### 20.1 Required anatomy

```text
Packages/com.echodevgames.echo-diagnostics/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   └── Developer/
│       ├── Architecture.md
│       ├── Provider API.md
│       ├── Validation API.md
│       ├── Performance and Privacy.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Runtime/
│   ├── Core/
│   ├── Configuration/
│   ├── Data/
│   ├── Providers/
│   ├── Sampling/
│   ├── Events/
│   ├── Snapshots/
│   ├── Export/
│   ├── DirectScene/
│   ├── Presentation/UGUI/
│   ├── Prefabs/
│   ├── EchoDevGames.EchoDiagnostics.Runtime.asmdef
│   └── EchoDevGames.EchoDiagnostics.Presentation.UGUI.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── PackageInventory/
│   ├── Inspectors/
│   ├── SnapshotViewer/
│   └── EchoDevGames.EchoDiagnostics.Editor.asmdef
├── Samples~/Standalone Labs/The Observatory Lab/
└── Tests/
    ├── Editor/
    └── Runtime/
```

First Light bridge remains a separate package/integration artifact.

### 20.2 Proposed source tree

Core types include root/state/registry/provider/registration/result; data includes descriptor/snapshot/section/field/metric/event/authority/launch/enums; configuration includes runtime/threshold/panel/redaction/counter profiles; providers include runtime context/performance/self-health; sampling includes clock/history; events include sink/buffer/filter; snapshots/export include builder/schema/JSON exporter; presentation includes overlay controller/panels/graphs; Editor includes setup/validation/package inventory/viewer.

### 20.3 Assemblies

| Assembly | Platform | Purpose |
|---|---|---|
| `EchoDevGames.EchoDiagnostics.Runtime` | Runtime | Core models, registry, sampling, events, snapshots/export |
| `EchoDevGames.EchoDiagnostics.Presentation.UGUI` | Runtime | Default overlay/panels/graphs |
| `EchoDevGames.EchoDiagnostics.Editor` | Editor | Setup, validation, inventory, inspectors, viewer, build hooks |
| `EchoDevGames.EchoDiagnostics.Tests.Runtime` | Tests | Runtime/PlayMode tests |
| `EchoDevGames.EchoDiagnostics.Tests.Editor` | Tests | Editor/setup/validation tests |

No runtime assembly references `UnityEditor`.

### 20.4 Repository files

README, full spec, docs index/current notes, provider/validation APIs, profile/privacy/export guide, code reference, Test Lab guide, changelog, license, notices, support/security guidance, contribution notes, release checklist, and stable `.meta` files/GUIDs.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

Unity 6000.0 minimum; 6000.3.8f1 primary tested baseline. uGUI, TextMeshPro, and Test Framework versions are captured during M1 and claimed only after validation.

### 21.2 Semantic versioning

- **Patch:** compatible fixes, performance improvements, additive codes/rules, presentation/doc fixes.
- **Minor:** additive providers, metrics, panels, profiles, rules, and compatible APIs/schema fields.
- **Major:** provider interface break, changed ID/code meaning, incompatible snapshot/config schema, more permissive privacy defaults, changed duplicate semantics, or removed supported behavior.

### 21.3 Deprecation

Public APIs/IDs/codes/schema receive warning and migration guidance. Codes are never reused. Security/privacy fixes may shorten support windows. Older snapshot readers receive compatibility notes.

### 21.4 GUID compatibility

Public scripts, prefabs, profiles/templates, presenter assets, and samples preserve `.meta` GUIDs. Project-owned generated config is never overwritten by updates. Migrations are explicit and reported.

---

## 22. Documentation Requirements

### 22.1 User documentation

Overview/boundaries, installation, five-minute start, runtime profiles, Boot/direct setup, overlay/panels, project-command integration, validator/build gate, snapshot/privacy/sharing, Test Lab, codes/troubleshooting, limitations/counter availability, migration, bridge index, license/notices/support/security.

### 22.2 Developer documentation

Architecture/lifecycle, provider model, IDs/codes/privacy, counters, event recursion, export schema, validation API, presenter replacement, test seams, First Light bridge, performance/privacy release gates, ADRs/status/current notes.

### 22.3 Truth rule

Examples compile; menu paths, profile/panel names, codes, schemas, counters, screenshots, setup output, and Lab instructions match the release. Metrics/platforms and public-safe policies are documented only after tests.

### 22.4 Repository/Obsidian workflow

Git-hosted Markdown opened directly in Obsidian. Current Notes captures proposals/tests/risks; checkpoints promote durable behavior, codes, performance/privacy/platform findings, and test evidence into authoritative records. Git remains archive.

### 22.5 Scan order

README, SFGSS-000, this spec, ADR/bridges, Current Notes, checkpoint/tests/issues/changelog, then implementation/tests.

---

## 23. Testing Strategy

### 23.1 Layers

| Layer | Scope | Required? |
|---|---|---:|
| EditMode unit | IDs, privacy, thresholds, buffers, snapshots, validation/export DTOs | Yes |
| PlayMode | Root, registry, providers, metrics adapters, overlay, lifecycle | Yes |
| Standalone Lab | Visible isolated loop | Yes |
| Bridge Lab | First Light launch panel | Yes before beta exit |
| Showcase | Combined dashboard | No |
| Clean install | Dependency/package proof | Yes |
| Existing-project adoption | Rescuers2D/Echo Systems Lab | Before adoption claim |

### 23.2 Required categories

Install/compile; root/static/duplicates; config versions; build profiles; provider lifecycle/fault/slow behavior; counters; fixed capacities/hysteresis; overlay/headless/EventSystem; scene/direct/domain reload; privacy/redaction; snapshot/export/import safety; manual/pre-Play/pre-build validation; inventory/version findings; sample/bridge removal; First Light bridge lifecycle; performance/allocation/platform builds.

### 23.3 Test registry

| ID | Requirement | Expected |
|---|---|---|
| EDIAG-T-001 | Two roots | One initializes; duplicate zero side effects |
| EDIAG-T-002 | Domain reload disabled | Fresh session each Play cycle |
| EDIAG-T-003 | No peers | Built-in Lab works |
| EDIAG-T-004 | Duplicate provider ID | Second rejected |
| EDIAG-T-005 | Registration dispose twice | Idempotent removal |
| EDIAG-T-006 | Throwing provider | Fault isolated |
| EDIAG-T-007 | Slow provider | Warn/throttle |
| EDIAG-T-008 | Unsupported counter | Unavailable, no exception |
| EDIAG-T-009 | Shutdown | Recorders/callbacks disposed |
| EDIAG-T-010 | Event overflow | Fixed capacity rollover |
| EDIAG-T-011 | Threshold hysteresis | No event spam |
| EDIAG-T-012 | Runtime use | Assets remain immutable |
| EDIAG-T-013 | Overlay modes | Correct modes; no pause |
| EDIAG-T-014 | No presenter | Headless works |
| EDIAG-T-015 | No EventSystem | Read-only plus warning |
| EDIAG-T-016 | Player safe | Sensitive/internal data filtered |
| EDIAG-T-017 | Screenshot safe | Visible redaction |
| EDIAG-T-018 | Export success | Valid schema JSON |
| EDIAG-T-019 | Export cancel/fail | No false success |
| EDIAG-T-020 | Throwing validation rule | Other rules continue |
| EDIAG-T-021 | Pre-Play blocker | Entry follows policy |
| EDIAG-T-022 | Unsafe release profile | Build blocked |
| EDIAG-T-023 | Setup repeated | No duplicates/overwrite |
| EDIAG-T-024 | Package inventory | Correct IDs/versions/sources |
| EDIAG-T-025 | Direct helper | One dev root |
| EDIAG-T-026 | Release helper | Validation blocks/disables |
| EDIAG-T-027 | First Light bridge | Launch panel mirrors public data |
| EDIAG-T-028 | Bridge removed | Cores compile/run |
| EDIAG-T-029 | Performance | Targets pass/documented blocker |
| EDIAG-T-030 | Sample removed/tarball install | Core compile/quick start |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership/non-ownership approved.
- [x] MVP separated from later/deferred scope.
- [x] Provider, snapshot, event, health, availability, privacy, export contracts defined.
- [x] Root, duplicate behavior, sampling, fault isolation defined.
- [x] Presentation independence from EchoUI/EchoInput defined.
- [x] Editor validation/build gates defined.
- [x] Standalone and First Light Integration Labs designed.
- [x] No release-blocking design question remains.

### 24.2 Implementation gate

- [ ] Runtime core compiles with declared dependencies only.
- [ ] Presentation and Editor assemblies are isolated.
- [ ] Duplicate root rejects before all side effects.
- [ ] Provider faults/unavailable counters degrade safely.
- [ ] Buffers are bounded and assets immutable.
- [ ] Setup/repair is repeatable/non-destructive.
- [ ] API matches spec or spec/ADR changes first.

### 24.3 Standalone gate

- [ ] Clean install succeeds.
- [ ] Validator/Lab work without peers.
- [ ] All overlay/headless/unavailable/fault/safe/export states pass.
- [ ] Direct scene behaves as documented.
- [ ] Samples and bridges remove safely.

### 24.4 Quality gate

- [ ] Automated/manual tests pass.
- [ ] No blocker/critical defect.
- [ ] Performance/allocation/capacity targets pass.
- [ ] Privacy/redaction/export/build tests pass.
- [ ] Codes/remedies actionable.
- [ ] Documentation matches implementation/counter/platform support.
- [ ] Current Notes reconciled and decisions promoted.
- [ ] Licenses/notices complete.

### 24.5 Distribution gate

- [ ] Manifest/dependencies valid.
- [ ] Version/changelog/meta files updated.
- [ ] Local/tarball/Git/reinstall/upgrade/removal tests pass.
- [ ] First Light bridge/Lab pass without hidden dependency.
- [ ] Tag/release/docs/status/compatibility catalog prepared and pushed.

---

## 25. Adoption and Migration Plan

### 25.1 Initial targets

| Project | Adoption strategy | Parity gate | Rollback |
|---|---|---|---|
| First Light | Separate bridge maps approved launch report/status | Both standalone packages still pass and panel mirrors public data | Remove bridge/Observatory; First Light unchanged |
| Rescuers2D | Dev-only root; adapt bootstrap/audio health one authority at a time | No gameplay/audio change; snapshot accurately identifies state | Remove adapters/root |
| Echo Systems Lab | Project adapters for application/save/scene/mission status | Existing loops unchanged; diagnostics agree with authority | Remove adapters |
| Don’t Get Vince’d | Bounded combat/project provider/event adapters | No timing/allocation regression | Remove adapters/root |
| DeverQuest | Reuse validation/report lessons only | No runtime DeverQuest dependency | No runtime adoption required |

### 25.2 Preserve-until-parity

Existing debug displays/logs/inspectors remain until Observatory evidence proves equivalent usefulness and acceptable overhead. Source authorities are never removed merely because a panel displays them. Adapters are added one at a time and remain reversible.

### 25.3 Migration tooling

Detect existing versions, preview and back up configuration migration, preserve IDs/GUIDs, validate provider/code/privacy compatibility, document rollback, and avoid automatic conversion of arbitrary project debug scripts.

---

## 26. Risks and Mitigations

| ID | Risk | L | I | Mitigation |
|---|---|---|---|---|
| R-001 | Scope becomes full profiler/console/cheat suite | High | High | Enforce authority/non-goals/MVP |
| R-002 | Hidden peer dependency | Med | High | Clean-project tests, separate bridges, no reflection |
| R-003 | Duplicate root side effects | Med | High | Claim first and test zero-side-effect duplicate |
| R-004 | Provider stalls gameplay | Med | High | Bounded sync contract, budgets, throttle/quarantine |
| R-005 | Overlay allocates every frame | Med | High | Ring buffers/reused views/performance gate |
| R-006 | Unsupported counters shown as zero | Med | High | Availability model/adapters |
| R-007 | Sensitive data ships publicly | Med | High | Privacy classes/safe profiles/build blocker |
| R-008 | Overlay accidentally visible in release | Med | High | Hidden/disabled defaults/pre-build validation |
| R-009 | Event capture recurses/spams | Med | Med | Bounded/deduped events; no logger replacement |
| R-010 | Provider model becomes unstructured soup | Med | Med | Standard vocabulary and typed values |
| R-011 | Model too rigid for future packages | Med | Med | Generic sections plus proven neutral specialized models |
| R-012 | Overlay conflicts with UI/input | Med | High | No EventSystem/input/pause ownership |
| R-013 | Export stalls/leaves partial files | Med | Med | Immutable capture, async write, temp/final policy |
| R-014 | Schema breaks support tooling | Low | High | Independent schema version/tests/notes |
| R-015 | Editor vs Player package inventory mismatch | High | Low | Editor truth in MVP; runtime manifest deferred |
| R-016 | Hardware sensors add platform burden | Med | High | Separate deferred providers |
| R-017 | Diagnostics failure affects gameplay | Low | Critical | Catch/isolate/degrade diagnostics only |
| R-018 | Setup overwrites project assets | Low | High | Create-only, preview, Undo/backup, repeat tests |
| R-019 | First Light bridge leaks hard dependency | Med | High | Separate package/removal compile tests |
| R-020 | Docs claim unvalidated counters/platforms | Med | Med | Availability matrix and documentation truth gate |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| ID | Decision | Status | Reason/consequence |
|---|---|---|---|
| EDIAG-D-001 | Own observability/validation, not source truth | Approved | Preserves one authority per concern |
| EDIAG-D-002 | One application-session runtime root; Editor validation independent | Approved | Cross-scene view without runtime requirement for validation |
| EDIAG-D-003 | Peers never reference Observatory; bridges adapt status | Approved | Independent install/removal |
| EDIAG-D-004 | Explicit stable-ID registration and disposable handles; no reflection | Approved | Predictable lifecycle/dependencies |
| EDIAG-D-005 | Provider capture synchronous, bounded, non-blocking | Approved | Async sources cache current state |
| EDIAG-D-006 | Immutable normalized DTOs with health/availability/severity/privacy | Approved | Safe view/export and adapter mapping |
| EDIAG-D-007 | ProfilerRecorder adapter; unsupported means unavailable | Approved | Supported Unity instrumentation without false data |
| EDIAG-D-008 | Isolated uGUI/TMP presenter; no EventSystem/input/pause ownership | Approved | Standalone visual proof with clean boundary |
| EDIAG-D-009 | Fixed ring buffers and bounded cadences | Approved | Predictable memory/overhead/privacy |
| EDIAG-D-010 | Diagnostics failures never become gameplay failures | Approved | Build validation may still block unsafe build |
| EDIAG-D-011 | Explicit local versioned JSON export; no automatic transmission | Approved | Clear privacy/operations boundary |
| EDIAG-D-012 | Safe filtering enforced; stricter policy wins | Approved | Missing privacy defaults stricter |
| EDIAG-D-013 | No logger replacement/all-log capture in MVP | Approved | Avoid behavior/recursion/privacy cost |
| EDIAG-D-014 | Overlay preferences session-only; persistence through later Settings bridge | Approved | Preserves settings authority |
| EDIAG-D-015 | Manual/pre-Play/pre-build validation; rule faults isolated | Approved | Early actionable failure detection |
| EDIAG-D-016 | Editor package inventory MVP; Player manifest deferred | Approved | Avoid generated build coupling now |
| EDIAG-D-017 | Neutral Launch model in core; First Light mapping separate | Approved | Launch panel without hard dependency |
| EDIAG-D-018 | Unity 6000.0 floor; 6000.3.8f1 baseline | Approved | Foundation alignment |

### 27.2 Release-blocking questions

None. Authority, MVP, provider model, presentation, privacy, export, validation, and bridge direction are approved.

### 27.3 Non-blocking later questions

License/contribution policy; exact validated counter catalog; UI Toolkit adapter; screenshot/compressed bundle; native sensors; next Foundation bridge after First Light; optional Unity log capture filters; final SFGSS-008 document ID convention.

---

## 28. Milestones and Checkpoint Path

### 28.1 Approved milestones

| Milestone | Outcome | Evidence |
|---|---|---|
| M0 Specification | Approved package contract | Approval record |
| M1 Skeleton | Manifest/assemblies/docs shell | Clean embedded/local/tarball compile |
| M2 Core vocabulary/authority | Root, DTOs, registry, events, self-health | EditMode/PlayMode tests |
| M3 Sampling/providers | Context/performance/history/thresholds | Counter/availability/performance tests |
| M4 Snapshot/export | Versioned capture/privacy/JSON | Schema/redaction/export tests |
| M5 Runtime presentation | Complete standalone dashboard | Lab/performance checklist |
| M6 Editor tooling | Setup/validator/inventory/Play/build gates | Editor/repeatability tests |
| M7 First Light integration | Separate bridge/Lab | Removal/version/teardown tests |
| M8 Beta/adoption | Real-project adapter | Adoption/performance report |
| M9 Release | Distribution-ready | External clean install/release gate |

### 28.2 Checkpoint rule

Each milestone becomes small SFGSS-005 Checkpoint Build Plans with exact files, Editor work, tests, rollback, documentation, and commit/push stop points. No implementation begins before FW-DOC-12.

### 28.3 First recommended implementation checkpoint

**EDIAG-M1-01:** Create the installable package skeleton, assembly boundaries, documentation shell, and clean-project compile proof without runtime behavior. The suite still expects First Light M1 first unless FW-DOC-12 changes order.

---

## 29. New-Conversation Handoff

```text
We are continuing development of The Sperk’s Forge — EchoDevGames Game Systems Suite.

Treat SFGSS-000 as suite authority and the approved EchoDiagnostics Specification
as authority for The Observatory’s provider model, root, overlay, validation,
privacy, export, Test Lab, and release gates. Follow SFGSS-005.

Current package: EchoDiagnostics — The Observatory
Specification: 1.0.0
Checkpoint: <CHECKPOINT>
Unity: <VERSION>
Repository: <PROJECT>
Status: <STATUS>
Blockers: <BLOCKERS>

Before code:
1. Summarize observability-only ownership and independence.
2. Preserve explicit registration, bounded capture, privacy, and fault isolation.
3. Never make a peer depend on Observatory.
4. Keep peer integrations in bridges/project adapters.
5. Do not mutate gameplay, own input/UI/pause, transmit data, or replace Unity profiling.
6. Use the Checkpoint Build Plan format.
```

### 29.1 Current status

| Field | Value |
|---|---|
| Package version | Not implemented; approved spec 1.0.0 |
| Completed checkpoint | FW-DOC-02 / EDIAG-M0 approved |
| Files/assets | Specification only |
| Tests | None; implementation not started |
| Known issues | None blocking; held by Foundation documentation gate |
| Decisions | EDIAG-D-001 through D-018 |
| Next checkpoint | FW-DOC-03 EchoSettings; Observatory M1 deferred to FW-DOC-12 |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Identity/responsibility and boundaries are clear.
- [x] Standalone independence and bridge direction are credible.
- [x] MVP is useful and bounded.
- [x] Root, duplicates, providers, failures, shutdown are specified.
- [x] Snapshot/event/metric/health/availability/privacy/export models are specified.
- [x] Presentation does not own EventSystem/input/pause.
- [x] Unsupported metrics degrade visibly.
- [x] Setup, validation, direct scene, privacy, and build gates are specified.
- [x] Standalone and First Light Integration Labs are defined.
- [x] Tests/performance/privacy/distribution evidence is measurable.
- [x] No Isekai identity introduced.
- [x] Long-term implementation-shaping choices are delegated/approved.
- [x] Implementation remains deferred until FW-DOC-12.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams  
**Date:** August 3, 2026  
**Conditions:** Continue the Foundation Specification Pass. Do not begin Foundation runtime implementation until all ten specifications and the cross-package consistency review are approved.

---

## Template Completion Review

A new collaborator can determine what the package owns/refuses, its standalone MVP, configuration-versus-runtime data, public API/lifecycle/failure behavior, isolated Lab, bridge direction, privacy/export rules, and release evidence without consulting an old chat. This document is **Approved** as the Level 2 authority for The Observatory; implementation remains deferred by the Foundation documentation gate.


---

## Graph Navigation

#sfgss/package #sfgss/wave/foundation #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
