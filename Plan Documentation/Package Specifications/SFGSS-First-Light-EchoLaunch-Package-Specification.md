# First Light – Startup and Launch Package Specification

**Working document ID:** SFGSS-PKG-ECHOLAUNCH-001
**Specification version:** 1.12.0
**Status:** Approved
**Technical package name:** EchoLaunch
**Public title:** First Light – Startup and Launch
**Package ID:** `com.echodevgames.echo-launch`
**Runtime namespace:** `EchoDevGames.EchoLaunch`
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Project boundary:** Independent solo project; not an Isekai Studios product
**Planned repository:** `EchoDevGames/EchoLaunch`
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`
**Unity baseline:** Unity 6000.3.8f1
**Parent authority:** SFGSS-000 and SFGSS-001
**Last updated:** August 6, 2026

> “Awaken the systems this project needs.”

> **Approval rule:** This specification is the approved package authority. Runtime and Editor implementation proceed only through an active SFGSS-005 Checkpoint Build Plan. FL-M5-06 has implemented and validated the explicit Editor-only Launch Simulator, transient immutable simulation requests/plans, real startup-sequence runner and policy execution, deterministic logical timing, stable simulation diagnostics, immutable schema-1 simulation reports, copyable text evidence, cancellation, and release-safe no-production-dependency boundary authorized by v1.12.0 and EchoLaunch-ADR-009.
No later checkpoint is currently authorized.
Standalone Laboratory scenes/assets, runtime sample step definitions, automatic scene installation, report export formats, build hooks, migration, receipt, uninstall, or recovery remain separately unauthorized.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-03 | Proposed | Initial complete package specification draft derived from SFGSS-000 v0.5.0 and SFGSS-001 v1.1.0 | Pending |
| 1.0.0 | 2026-08-03 | Approved | Resolved implementation-shaping decisions, approved the full package contract, and deferred implementation until the Foundation Wave specification pass is complete | Jesse “Echo” Adams |
| 1.1.0 | 2026-08-03 | Approved | Recorded FW-DOC-12 readiness approval, adopted SFGSS-005 as the implementation workflow authority, and selected FL-M1-01 Package Skeleton without changing runtime behavior or public API intent | Jesse “Echo” Adams |
| 1.2.0 | 2026-08-04 | Approved | Separated the default uGUI presenter from the neutral Runtime assembly; Set the Editor assembly to `autoReferenced: false`; Canonicalized immutable `StartupStepDefinition` versus runtime executor terminology. Also normalized registry metadata and evidence interpretation. | Jesse “Echo” Adams |
| 1.3.0 | 2026-08-04 | Approved | Recorded SUITE-DOC-33 activation of FL-M1-01, adopted the just-in-time package learning gate, and updated implementation status without changing runtime behavior or public API intent | Jesse “Echo” Adams |
| 1.4.0 | 2026-08-05 | Approved | Selected a standalone project-owned `LaunchDestination` ScriptableObject, assigned destination schema version 1, advanced `EchoLaunchConfiguration` to schema version 3, preserved schema 2 as the historical startup-sequence-only shape, and authorized FL-M3-08 destination handoff work | Jesse “Echo” Adams |
| 1.5.0 | 2026-08-05 | Approved | Advanced `EchoLaunchConfiguration` to schema version 4 with an optional project-owned `SplashSequence` reference and project-authored reduced-motion default; selected sequential root order of optional splash, startup steps, destination transition; preserved report schema 2; and authorized FL-M4-04 | Jesse “Echo” Adams |
| 1.6.0 | 2026-08-05 | Approved | Defined two immutable neutral package template prefabs, selected the package-owned Canvas hierarchy and defaults, preserved project ownership of branding/layout variants/input bindings, prohibited hidden prefab discovery or spawning, and authorized FL-M4-05 | Jesse “Echo” Adams |
| 1.7.0 | 2026-08-05 | Approved | Defined the read-only project snapshot, immutable setup request/plan/operation contracts, deterministic dry-run planner, preview-only Setup window, stable setup diagnostics, default project-owned paths, and explicit no-write boundary for FL-M5-01 | Jesse “Echo” Adams |
| 1.8.0 | 2026-08-05 | Approved | Authorized the fresh-plan-gated create-only setup apply service, deterministic asset/prefab/scene creation order, explicit Build Settings mutation policy, single-active apply gate, compensating rollback journal, immutable apply result, and repeat-safe no-op reruns for FL-M5-02 | Jesse “Echo” Adams |
| 1.9.0 | 2026-08-05 | Approved | Authorized explicit Setup Repair for narrowly provable current-schema drift, separate repair confirmation, ownership/shape gates, byte-preserving backup and rollback of modified project assets, immutable repair reporting, and repeat-safe reconciliation for FL-M5-03 | Jesse “Echo” Adams |
| 1.10.0 | 2026-08-06 | Approved | Authorized the explicit read-only First Light Validator, immutable schema-1 findings and project-health report, stable validation codes, scene-safe enabled-build-scene inspection, deterministic request/evidence/report fingerprints, and copyable project-relative text evidence for FL-M5-04 | Jesse “Echo” Adams |
| 1.11.0 | 2026-08-06 | Approved | Authorized the project-owned Direct Scene Development Initializer, Start-time authority reuse, active-destination no-reload handoff, Editor-only default policy, explicit Development-Build opt-in, unconditional non-development release prohibition, `DirectSceneDevelopment` report mode, and activated `ELAUNCH-VAL-009` for FL-M5-05 | Jesse “Echo” Adams |
| 1.12.0 | 2026-08-06 | Approved | Authorized the explicit Editor-only Launch Simulator, transient immutable scenario planning, real startup-sequence runner/policy execution, deterministic logical timing and progress, stable simulation diagnostics, immutable schema-1 simulation reports, copyable text evidence, cancellation, and zero production-runtime dependency for FL-M5-06 | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** First Light – Startup and Launch
**Technical identifier:** EchoLaunch
**Flavor line:** Awaken the systems this project needs.
**Plain-language subtitle:** Startup sequencing, launch diagnostics, splash presentation, and initial destination handoff.

**One-sentence ownership contract:**

> EchoLaunch owns the project’s initial runtime claim, ordered startup execution, startup-only presentation, structured launch reporting, and final launch handoff; it does not own audio playback, save data, menus, gameplay rules, normal mid-game scene travel, or arbitrary service location.

### 1.1 Elevator summary

First Light gives a Unity project one reliable beginning. It protects the startup authority from duplication, validates the launch configuration before side effects, runs a deterministic sequence of required and optional startup steps, presents simple launch status without requiring another Sperk’s Forge package, and hands control to one configured destination.

The package is useful by itself. A small project can use First Light only for a Boot scene, image splashes, startup checks, a final scene load, and a readable launch report. Larger projects can add explicit startup-step bridges for settings, saves, audio, UI, diagnostics, scene flow, or project-defined services without making those systems part of EchoLaunch.

### 1.2 Why this belongs in The Sperk’s Forge

Existing projects repeatedly need a persistent bootstrap, direct-scene development support, splash handling, setup validation, and a safe initial destination. They also expose the same failure pattern: multiple persistent managers can initialize independently, subscribe twice, play or load twice, and become difficult to diagnose.

First Light preserves the useful idea of a central application origin while correcting the “god manager” failure mode. It coordinates startup order and reporting but leaves every installed package in charge of its own behavior.

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | “First Light” must always be paired with “Startup and Launch” in formal package surfaces. |
| Setup guidance/tooltips | Yes | Flavor may introduce a step, but the technical action must be immediately clear. |
| Samples | Optional | Sample art and wording must be replaceable and removable. |
| Runtime API/type names | No lore-only names | Types must describe launch, startup, steps, reports, destinations, and lifecycle directly. |
| Project data | No required Hackulos content | The consuming game owns logos, legal screens, scenes, text, art, and terminology. |

---

## 2. Problem Statement

### 2.1 Current problem

Unity projects often begin with loosely coordinated persistent objects, hard-coded scene names, manually ordered `Awake` methods, duplicate `DontDestroyOnLoad` objects, and startup failures visible only as scattered Console messages. Directly opening a gameplay scene frequently creates a second unofficial bootstrap path that behaves differently from the production Boot scene.

This creates four recurring costs:

1. Initialization order becomes implicit and fragile.
2. Duplicate persistent authorities perform side effects before detecting one another.
3. Startup failures are difficult to summarize or reproduce.
4. Reusable packages become coupled to one project’s scene names, managers, or content.

### 2.2 Evidence from existing work

| Source project | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| Rescuers2D | Multiple persistent systems and bootstrap-conflict lessons | Persistent services and direct-scene development convenience | Reject duplicates before subscriptions, playback, loading, or other side effects; remove hard-coded scene assumptions |
| Echo Systems Lab | Application bootstrap, scene loading, save initialization, checkpoint workflow | Focused services, explicit initialization state, event-driven handoff | Separate composition from service ownership; replace project-specific destination and save knowledge |
| DeverQuest | Mature setup, validation, repair, documentation, and package anatomy | Product-grade tooling and repeatable setup | Keep Editor workflows out of runtime and avoid project-local identity state |
| Don’t Get Vince’d | Real-project integration target with different gameplay architecture | Incremental replacement and parity testing | Prove First Light is not shaped only for Rescuers2D |
| Hackulos | Future need for application-shell startup and optional RPG systems | Explicit package composition | Prevent RPG data or lore from entering the general launch package |

### 2.3 Consequences of doing nothing

- Every project recreates a boot manager and scene-loading convention.
- Duplicate persistent roots continue causing double subscriptions, overlapping audio, repeated initialization, or inconsistent state.
- Direct-scene tests remain less trustworthy than production boot tests.
- Startup failures remain scattered across logs rather than captured in one report.
- Other packages are pressured to invent their own startup ownership and ordering.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Claim exactly one launch authority before any startup side effect.
- Provide deterministic ordered startup execution.
- Support required and optional steps with explicit failure policy.
- Support both immediate and asynchronous startup work.
- Present image splashes and plain launch status without EchoUI.
- Produce one structured `LaunchReport` for every launch attempt.
- Load one validated final destination in the MVP.
- Support direct-scene development through the same runtime rules.
- Allow project-defined and bridge-provided startup steps without editing package source.
- Keep setup, repair, and validation repeatable and non-destructive.

### 3.2 Non-goals

- First Light does not play music or sound effects.
- First Light does not load or serialize save games.
- First Light does not implement a Main Menu or general screen stack.
- First Light does not own normal mid-game scene-transition policy.
- First Light does not own global settings, pause state, input contexts, or gameplay state.
- First Light does not become a general-purpose dependency-injection container or arbitrary service locator.
- First Light does not require the Observatory, Looking Glass, Passage, Chronicle, Accord, Pulse, Will, or Jukebot.
- The MVP does not include video splash playback, conditional continue/new-game selection, a visual dependency graph, or package-specific startup steps.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Clean supported Unity project | Create a valid Boot scene and run to one destination through a guided setup tool |
| Programmer | Project with custom startup work | Add a startup step through the documented extension contract without changing package code |
| Designer/content author | Project needs logos or legal images | Configure image entries, timing, fading, skip policy, and descriptive labels in project-owned assets |
| Tester | Startup problem or duplicate root | Reproduce the launch, inspect the active step and report, and identify a stable diagnostic code |
| Maintainer | Package upgrade or project repair | Re-run validation and repair without overwriting project-owned scenes or configuration silently |

### 3.4 Measurable success criteria

- EchoLaunch installs into a clean supported Unity project with zero compile errors.
- Its MVP runs with no other Sperk’s Forge runtime package installed.
- The Standalone Test Lab proves canonical Boot launch, direct-scene launch, duplicate rejection, success, warning, recoverable failure, and blocking failure.
- A duplicate root performs no startup step, scene load, event subscription, splash playback, or other launch side effect.
- Every launch attempt ends with a structured report, including blocked launches.
- Re-running setup creates no duplicate root, duplicate configuration, duplicate Boot scene, or duplicate build-settings entry.
- Deleting `Samples~` leaves runtime and Editor assemblies compiling.
- Removing every optional bridge leaves EchoLaunch compiling and functional.
- The package reaches one configured destination through `SceneManager.LoadSceneAsync` in standalone MVP operation.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo Unity developers starting new projects.
- Small teams needing a consistent startup contract.
- Package developers providing optional startup integrations.
- QA testers reproducing initialization failures.
- Maintainers migrating projects away from project-specific boot managers.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| UC-001 | Launch from canonical Boot scene | Player/developer | Valid root and configuration | One authority claims runtime, steps execute in order, destination loads, handoff completes | MVP |
| UC-002 | Present configured image splashes | Designer | Valid `SplashSequence` | Entries display in order with configured timing and skip rules | MVP |
| UC-003 | Run synchronous startup step | Programmer | Step included and enabled | Step reports success/warning/failure and execution continues or stops by policy | MVP |
| UC-004 | Run asynchronous startup step | Programmer | Step included and enabled | Progress and completion are tracked without freezing the main loop | MVP |
| UC-005 | Diagnose blocked startup | Tester | Invalid configuration or blocking step failure | Status view explains failure and report contains code, message, step, and timing | MVP |
| UC-006 | Start a Test Lab scene directly | Developer | Development initializer configured | Existing root is reused or minimum development root is created once and marked as direct-scene mode | MVP |
| UC-007 | Repair Boot setup | Maintainer | Partial or damaged configuration | Tool previews and safely repairs only missing/invalid generated pieces | MVP |
| UC-008 | Add optional package initialization | Package developer | Both packages installed | A bridge contributes an explicit startup step; neither core package gains a hidden dependency | Later/integration |
| UC-009 | Select destination by save or progression state | Project developer | Destination provider installed | Provider resolves main menu, continue, test, or project-defined target | Later |
| UC-010 | Visualize launch graph in Observatory | Tester | Optional bridge installed | Observatory shows phases, steps, timings, state, warnings, and handoff | Later/integration |

### 4.3 Explicitly unsupported use cases

- Treating EchoLaunch as the project’s universal runtime API registry.
- Using launch steps as a replacement for gameplay state machines or normal scene flow.
- Running arbitrary gameplay updates through the startup sequence after handoff.
- Loading unvalidated scenes by free-form production string from UI code.
- Making a peer package function only when EchoLaunch is present.
- Shipping a direct-scene development initializer enabled in release builds without explicit project approval.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- The initial claim of launch authority.
- Duplicate-root rejection before startup side effects.
- Preflight validation of the active launch configuration.
- Ordered execution of the configured startup sequence.
- Startup-step state, progress, timing, and results.
- Startup-only image splash and plain status presentation.
- Construction and publication of the structured launch report.
- The initial standalone destination load and launch handoff.
- Direct-scene development initialization rules.
- Setup, repair, validation, and failure-simulation tooling for EchoLaunch.

### 5.2 The package does not own

- Music, SFX, ambience, mixer routing, or volume preferences.
- Save-slot selection, save deserialization, progression, or continue rules.
- Main Menu, Pause Menu, settings screens, or general UI navigation.
- Normal scene transitions after launch.
- High-level runtime state or pause authority.
- Input contexts, rebinding, or gameplay controls.
- Project service registration for arbitrary gameplay access.
- Game-specific logos, legal copy, first-scene content, or gameplay rules.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoLaunch interacts |
|---|---|---|
| Audio playback | Jukebot | Optional bridge startup step requests Jukebot initialization; EchoLaunch never plays clips directly |
| Global preferences | EchoSettings | Optional bridge startup step initializes/loads settings |
| Save access | EchoSave | Optional bridge startup step initializes save infrastructure; destination selection remains separate |
| Normal scene travel | EchoSceneFlow | Optional final-transition bridge; standalone MVP uses a minimal internal initial load only |
| Runtime state/pause | EchoGameState | Optional bridge requests Booting/Loading/handoff states |
| Rebinding/input context | EchoInput | Optional bridge may establish startup input context |
| General UI | EchoUI | Optional presenter bridge may replace plain status presentation |
| Diagnostics dashboard | EchoDiagnostics | Optional provider bridge publishes launch graph/report/status |
| Project gameplay services | Project code | Project-defined startup steps with serialized dependencies or explicit adapters |

### 5.4 Boundary tests

A proposed feature belongs in EchoLaunch only when it directly supports initial authority, ordered startup, launch-only presentation, structured reporting, direct-scene development, or the final launch handoff. Features that remain useful throughout normal play usually belong to another authority.

---

## 6. Independence Contract

Independence is a release gate.

### 6.1 Standalone guarantees

EchoLaunch must:

- Compile with only its declared Unity dependencies.
- Initialize without any other Sperk’s Forge package.
- Load one final scene without EchoSceneFlow.
- Present status and image splashes without EchoUI.
- Produce diagnostics without EchoDiagnostics.
- Avoid direct references to project assemblies.
- Keep configured assets and scenes project-owned.
- Expose a documented custom-step seam.
- Fail visibly and safely when optional collaborators are absent.
- Allow tests to supply a controlled step runner, clock, destination loader, and presenter through explicit seams.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | Boot scene launches through built-in sequence and scene loader | Clean-project PlayMode + LAB-001 |
| Enter Standalone Test Lab directly | Development initializer creates one marked development launch only when none exists | LAB-008 |
| Optional bridge absent | No compile error, warning, reflection probe, or changed core behavior | EditMode dependency test |
| Optional package disabled | Bridge is absent/disabled; EchoLaunch continues using standalone behavior | Integration removal test |
| Duplicate root present | First claimant remains authority; duplicate stops before side effects and reports stable code | PlayMode duplicate test + LAB-006 |
| Required configuration missing | Launch blocks before steps and displays actionable status/report | LAB-004 |
| Sample content deleted | Runtime and Editor assemblies compile; package setup remains available | Clean-project sample-removal test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity Engine core/runtime | Platform | Yes | Unity 6000.0 | MonoBehaviour, ScriptableObject, Awaitable, time, logging, serialization | Package cannot function without Unity |
| Unity Scene Management | Platform | Yes | Included with supported Unity editor/runtime | Initial destination validation and load | Package cannot complete standalone handoff |
| Unity UI (uGUI) | Platform | Yes for MVP | Baseline-compatible package version captured at M1 | Default image splash and plain status presenter | The presentation assembly is isolated from the launch core; replacing the presenter does not transfer launch authority |
| Unity Test Framework | Test-only | Yes for development | Compatible baseline version | EditMode and PlayMode tests | No runtime dependency in player builds |

### 6.4 Forbidden dependencies

- Project-specific code or assemblies.
- Another Sperk’s Forge runtime package in the EchoLaunch core.
- Samples, tests, or Editor assemblies at runtime.
- `Resources` path conventions as a hidden production requirement.
- Hard-coded scene names, build indices, tags, layers, input maps, or save files.
- Reflection-based discovery of peer packages in the MVP.
- Unlicensed third-party media.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Surface | Notes |
|---|---|---|---|---:|---|---|
| CAP-001 | Authority claim | Exactly one launch root claims runtime before side effects | Approved by SFGSS-000 | Yes | Runtime | Duplicate policy is mandatory |
| CAP-002 | Preflight | Validate root, configuration, sequence, presentation, and destination | Approved | Yes | Runtime/Editor | Blocking failures prevent execution |
| CAP-003 | Ordered startup sequence | Execute enabled steps deterministically | Approved | Yes | Runtime | Stable order from project-owned sequence asset |
| CAP-004 | Immediate and asynchronous steps | Support work that completes now or over time | Approved | Yes | Runtime | Exact async primitive is a release-blocking API decision |
| CAP-005 | Step policy | Required/optional, warning/blocking, timeout, retry/skip metadata | Approved | Yes | Runtime/Data | MVP supports explicit policy; interactive retry UI is later |
| CAP-006 | Progress/status | Expose phase, active step, message, progress, and elapsed time | Approved | Yes | Runtime/Presentation | No per-frame log spam |
| CAP-007 | Image splash sequence | Image entries with fade, hold, minimum time, and skip policy | Approved | Yes | Runtime/Data/Presentation | Video and custom animation adapters deferred |
| CAP-008 | Plain status view | Minimal readable view without EchoUI | Approved | Yes | Runtime/Prefab | Project can replace visual skin |
| CAP-009 | Structured launch report | Immutable summary of launch attempt and each step result | Approved | Yes | Runtime | Generated on success and failure |
| CAP-010 | Final destination | Load one validated configured scene and complete handoff | Approved | Yes | Runtime | Standalone initial transition only |
| CAP-011 | Direct-scene initializer | Minimum development runtime only when authority is absent | Approved | Yes | Runtime/Sample | Disabled or excluded in release by default |
| CAP-012 | Setup/repair | Create/repair canonical Boot scene and project-owned assets | Approved | Yes | Editor | Preview and repeat-safe |
| CAP-013 | Validator | Detect duplicates, missing references, scene/build errors, and unsafe setup | Approved | Yes | Editor | Stable validation IDs |
| CAP-014 | Delay/failure simulation | Simulate deterministic success, timed progress, warnings, recoverable failures, blocking failures, timeouts, cancellation, and executor exceptions through the real sequence runner | Approved | Yes | Editor | Explicit invocation; transient in-memory data; no production-runtime dependency |
| CAP-015 | Portable report export | Export launch report for bug reports | Approved | Later | Editor/Runtime | JSON/text format decision later |
| CAP-016 | Conditional destination providers | Resolve Main Menu/new game/continue/test/project destination | Approved concept | No | Runtime/Bridge | Post-MVP |
| CAP-017 | Custom splash adapters | Animation/video/custom presenter adapters | Approved concept | No | Bridge/Runtime | Post-MVP |
| CAP-018 | Visual systems graph | Ordered dependency/status map | Approved concept | No | Editor/Observatory | Post-MVP |
| CAP-019 | Observatory bridge | Publish launch state and report to diagnostics dashboard | Approved concept | No | Separate bridge | Must remain removable |
| CAP-020 | EchoSceneFlow final transition bridge | Delegate final startup transition | Approved concept | No | Separate bridge | EchoSceneFlow remains normal travel authority |

### 7.2 MVP capability set

The smallest complete First Light release contains:

- One protected `EchoLaunchRoot`.
- One project-owned `EchoLaunchConfiguration`.
- One ordered `StartupSequence`.
- Custom startup-step extension contract.
- Required/optional behavior and explicit failure policy.
- Synchronous and asynchronous execution.
- Image-only splash entries.
- One plain startup status/splash presenter.
- One structured `LaunchReport`.
- One validated final scene destination.
- One direct-scene development initializer.
- One canonical Boot scene setup/repair workflow.
- One isolated Standalone Test Lab.
- EditMode and PlayMode coverage for lifecycle, validation, duplicate protection, step policy, and handoff.

### 7.3 Later capability set

- Conditional destination resolution.
- Continue/new-game/save-aware destination bridges.
- Video and animation splash adapters.
- Interactive retry/cancel/skip presentation.
- Exportable bug-report bundles.
- Visual dependency graph and Observatory dashboard integration.
- EchoSceneFlow transition bridge.
- Package-specific startup-step bridges.
- Startup profiles for multiple build targets or test modes.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| General service locator | Rejected | Violates focused authority and risks arbitrary global coupling | None without suite-wide ADR |
| Mandatory dependency-injection container | Rejected for MVP | Not required for launch promise and would create framework lock-in | Proven need across at least three packages and approved ADR |
| Built-in audio playback | Rejected | Jukebot owns audio | Jukebot bridge only |
| Full screen/navigation system | Rejected | EchoUI owns reusable UI | Presenter bridge only |
| Normal mid-game scene manager | Rejected | EchoSceneFlow owns normal travel | Final-transition bridge only |
| Video splash playback | Deferred | Expands dependencies and platform testing | Image MVP stable and adapter specification approved |
| Save-aware continue logic | Deferred | EchoSave/EchoProgression authority required | Bridge and destination-provider design approved |
| Runtime auto-repair | Rejected | Diagnostics may report; production data should not be silently modified | Explicit Editor repair only |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | `EchoLaunchConfiguration`, `StartupSequence`, step assets/metadata, `SplashSequence`, destination reference, policies | Active step index, elapsed time, cancellation state, scene objects, current report |
| Runtime state/behavior | `EchoLaunchRoot`, runner, active session state, step executions, report builder, destination loader | Editor-only setup, project-specific service rules, general UI navigation |
| Presentation/feedback | Minimal status/splash presenter and project-replaceable prefab | Startup truth, step completion authority, save/audio/gameplay behavior |

### 8.2 Component topology

```text
Project-owned assets
EchoLaunchConfiguration
├── StartupSequence
│   └── ordered StartupStep definitions
├── SplashSequence → optional project-owned image sequence
├── UseReducedMotionForSplash → project-authored default
├── InitialDestination → project-owned LaunchDestination asset
└── runtime and presentation policies

Scene authority
EchoLaunchRoot
├── AuthorityClaim
├── LaunchPreflight
├── SplashSequencePlayer
├── StartupSequenceRunner
├── LaunchSessionState
├── LaunchReportBuilder
├── ILaunchStatusPresenter (default: EchoLaunchStatusView)
├── IImageSplashPresenter (default: EchoLaunchStatusView or headless fallback)
└── IInitialDestinationLoader (default: Unity scene loader)

Development helper
EchoDirectSceneInitializer
├── waits until Start so scene roots claim in Awake first
├── reuses existing authority when present
├── otherwise instantiates one configured project-owned direct root prefab
├── permits Editor by default and Development Builds only by explicit opt-in
└── can never create a development root in a non-development release player
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | Yes for canonical startup; direct-scene helper creates it only when absent |
| Root type | `EchoLaunchRoot : MonoBehaviour` |
| Duplicate behavior | First valid claimant wins. A later root records/reports the duplicate and disables or destroys itself before validation, subscriptions, presentation, steps, or loads. |
| Initialization trigger | Root claims in `Awake`; execution begins through an explicit internal start gate after claim and serialized-reference validation |
| Shutdown behavior | Cancel active safe operations, finalize an interrupted report, detach presenter, release startup-only resources, clear authority claim at application/domain reset |
| Direct-scene behavior | Initializer settles once in `Start`, reuses authority claimed during scene `Awake`, otherwise instantiates one project-owned direct root prefab authored for `DirectSceneDevelopment`; when its configured destination is already active, handoff succeeds without reloading the scene |
| Test injection seam | Explicit clock, step executor/runner, presenter, and destination-loader interfaces or factories; serialized production defaults remain novice-friendly |

### 8.4 Lifecycle sequence

1. **Construct scene object** — Unity loads the Boot scene or direct-scene helper.
2. **Claim runtime** — root atomically claims launch authority before any side effect.
3. **Reject duplicate** — duplicate reports a stable code and exits immediately.
4. **Validate references** — root, configuration, startup sequence, optional splash sequence, presenter, and destination references are checked.
5. **Begin report** — launch mode, package version, timestamps, configuration identity, and preflight results are recorded.
6. **Preflight** — configuration, splash, startup sequence, destination, and build-scene validity are evaluated before launch work.
7. **Prepare presentation** — plain status and splash presenters are resolved; no gameplay/package authority is initialized by presentation.
8. **Play optional splash** — when schema-4 configuration assigns a `SplashSequence`, the root plays it once through `SplashSequencePlayer` using the launch clock, root cancellation token, configured reduced-motion default, and resolved splash presenter.
9. **Run startup steps** — only after splash completion or no-op omission, the configured startup sequence executes through the existing runner.
10. **Resolve final destination** — MVP uses one configured scene reference.
11. **Transition** — standalone loader performs validated asynchronous single-scene load.
12. **Handoff** — launch is marked complete after destination activation and handoff callbacks.
13. **Release startup-only resources** — splash/status objects are hidden or destroyed according to policy.
14. **Retain or release root** — root follows approved lifetime policy and exposes final immutable report while retained.
15. **Shutdown/reset** — active work is cancelled where safe, report is finalized, and static claim state is reset for domain-reload-disabled workflows.

Splash playback and startup-step execution are sequential in the MVP. They do not run concurrently, race for presentation ownership, or hide step-side effects behind an overlapping splash timeline.

### 8.5 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate root | Authority claim | Duplicate warning in status/Console when safe | First claimant continues; duplicate exits before side effects | ELAUNCH-ROOT-001 |
| Missing configuration | Root validation | Blocking status with repair guidance | No steps or scene load | ELAUNCH-CFG-001 |
| Empty required sequence | Preflight | Blocking or warning according to explicit configuration policy | Package may continue only when an empty sequence is explicitly allowed | ELAUNCH-SEQ-001 |
| Null step reference | Preflight | Identifies sequence index and asset | Block if required; skip only when explicitly optional | ELAUNCH-STEP-001 |
| Duplicate step ID | Preflight | Lists colliding assets/indices | Block report generation/execution | ELAUNCH-STEP-002 |
| Step timeout | Runtime | Active step, elapsed time, and policy shown | Continue, retry, or block according to step policy; MVP supports continue-or-block, not interactive retry | ELAUNCH-STEP-003 |
| Step exception | Runtime | Sanitized failure message and step ID | Convert to blocking or recoverable failure by policy; never crash silently | ELAUNCH-STEP-004 |
| Presenter missing | Preflight | Console/report warning | Use logging-only headless status path if presentation is optional; otherwise block | ELAUNCH-VIEW-001 |
| Splash sequence invalid | Preflight | Splash asset/index guidance | No splash, steps, or destination work; report blocks | ELAUNCH-SPLASH-001 |
| Splash playback failed | Splash phase | Blocking status with sanitized details | Stop before startup steps and destination; finalize failed report | ELAUNCH-SPLASH-002 |
| Splash visual presenter unavailable | Preflight/prepare presentation | Warning when a splash is configured | Preserve authored timing through `NullImageSplashPresenter`; continue headless | ELAUNCH-SPLASH-003 |
| Invalid destination | Preflight | Scene/path guidance | No transition; report blocks | ELAUNCH-DEST-001 |
| Destination load failure | Transition | Failure state and scene error | Remain in Boot/status scene; no false handoff | ELAUNCH-DEST-002 |
| Direct initializer unavailable in release | Direct-scene entry | Explicit Editor/development warning | Require canonical Boot scene | ELAUNCH-DIRECT-001 |
| Shutdown during launch | Application quit/destruction | No noisy user prompt | Cancel safe work, finalize interrupted report | ELAUNCH-LIFE-001 |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `EchoLaunchConfiguration` | Top-level project launch choices and references | Yes, configuration ID | No | Yes |
| `StartupSequence` | Ordered list of startup-step definitions | Yes | No | Yes |
| `StartupStep` | Base definition and execution contract for one launch operation | Yes, step ID | Definition no; active execution state is separate | Usually yes; package may ship safe test/sample steps |
| `SplashSequence` | Ordered image splash entries and sequence policy | Yes | No | Yes |
| `SplashEntry` | Image, label, timing, fade, minimum display, and skip policy | Entry ID required when referenced diagnostically | No | Stored in project-owned sequence |
| `LaunchDestination` | Validated initial scene reference and display metadata | Yes, destination ID | No | Yes; standalone project-owned ScriptableObject |
| `DirectSceneConfiguration` | Project-owned direct root prefab, environment policy, and stable identity | Yes | No | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `LaunchSession` | `EchoLaunchRoot` | One launch attempt | New instance for every canonical or direct launch | Not durable game data |
| `StartupStepExecution` | Sequence runner | One step attempt | Disposed after report capture | Copied into report summary only |
| `LaunchProgressSnapshot` | Launch session | Updated during active launch | Replaced atomically | Not persisted by EchoLaunch |
| `LaunchReportBuilder` | Launch root | One launch attempt | Finalized once | Produces immutable `LaunchReport` |
| `LaunchReport` | Root/caller | Session or exported support artifact | Immutable | Optional external export later; not EchoSave data |
| Authority claim state | Package lifecycle subsystem | Current play session/domain | Reset at subsystem registration and shutdown | Never serialized |

### 9.3 Stable identifiers

- Configuration, sequence, steps, and diagnostically referenced splash entries require stable string IDs.
- IDs are generated by Editor tooling and validated for empty values and collisions.
- Display names are separate and may change without changing IDs.
- An ID change after public release requires an alias or migration note when reports, configuration, or external integrations depend on it.
- Scene destination identity must not rely only on a display label.
- The MVP does not store save-game references, but stable IDs still support tests, reports, and future bridges.

### 9.4 ScriptableObject safety

All configuration, sequence, step, splash, and destination assets are treated as immutable during play. Active index, progress, timestamps, cancellation, retries, exception data, results, and scene-loading state remain in runtime-owned objects. Tests must verify that a completed Play Mode run does not dirty or modify configuration or destination assets.

### 9.4.1 Initial destination asset decision

- `LaunchDestination` is a standalone project-owned `ScriptableObject`.
- `LaunchDestination.CurrentSchemaVersion` begins at `1`.
- The asset owns a stable destination ID, a user-facing display label, and runtime-safe initial scene metadata.
- Scene destination identity does not depend on the display label.
- `EchoLaunchConfiguration` stores one serialized `LaunchDestination` reference named for the initial destination role.
- Runtime code reads but never rewrites either asset.
- Editor tooling may later use `SceneAsset` authoring support, but the neutral Runtime assembly stores only runtime-safe scene metadata and does not reference `UnityEditor`.
- Conditional or save-aware destination providers remain deferred.

### 9.4.2 Splash configuration and root-order decision

- `SplashSequence` remains a standalone project-owned `ScriptableObject`.
- `EchoLaunchConfiguration` stores one optional serialized `SplashSequence` reference.
- A null splash reference means the splash phase is intentionally omitted and produces no warning or failure.
- An assigned empty but valid sequence is a legal no-op.
- `EchoLaunchConfiguration` stores one project-authored `UseReducedMotionForSplash` default.
- The root passes that value directly to `SplashSequencePlayer`.
- Runtime preference-provider or EchoSettings overrides remain deferred.
- Canonical Boot and direct-scene development launches use the same configured splash contract.
- The root plays the optional splash before startup steps.
- Splash playback and startup-step execution are not concurrent in the MVP.
- Successful splash completion clears splash presentation before startup-step presentation begins.
- Runtime reads but never rewrites configuration or splash assets.

### 9.5 Serialization and migration

- `EchoLaunchConfiguration.CurrentSchemaVersion` is `4`.
- Configuration schema `2` remains the historical startup-sequence-only serialized shape.
- Configuration schema `3` remains the historical startup-sequence-plus-initial-destination shape.
- Configuration schema `4` adds the optional serialized project-owned `SplashSequence` reference and the project-authored reduced-motion default.
- `LaunchDestination.CurrentSchemaVersion` remains `1`.
- `SplashSequence.CurrentSchemaVersion` remains `1`.
- `LaunchReport.CurrentSchemaVersion` remains `2`.
- Successful splash timing contributes to the report's existing total launch elapsed time.
- Splash failures use the existing immutable final-result code/message surface; report schema 2 gains no splash-specific fields.
- Editor migration owns supported asset upgrades.
- Runtime blocks unsupported older/newer versions through `ELAUNCH-CFG-002` and must not silently rewrite assets.
- Migration previews the affected assets and preserves a backup when a destructive structure change is required.
- Unknown future versions block Editor repair and produce a clear compatibility result.
- `LaunchReport` includes a report schema version for future export compatibility.

---

## 10. Public Runtime API

> The semantic contract below is approved. First Light uses Unity `Awaitable` for asynchronous execution, while immutable ScriptableObject definitions create separate runtime executor instances so active state never lives in shared assets.

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `EchoLaunchRoot` | Class/MonoBehaviour | Authoritative launch lifecycle and public status surface | Scene/prefab; one authority |
| `EchoLaunchConfiguration` | ScriptableObject | Project-owned launch configuration | Created by setup tool or Create menu |
| `StartupSequence` | ScriptableObject | Ordered immutable startup-step list | Project-owned |
| `StartupStepDefinition` | Abstract ScriptableObject | Immutable authored definition and factory for one runtime executor | Project/bridge asset; never stores active execution state |
| `IStartupStepExecutor` | Interface | Executes one startup operation using an approved Unity `Awaitable` contract | New runtime instance created for each step attempt |
| `StartupStepContext` | Class/readonly context | Read-only launch context, progress reporter, cancellation token, diagnostics | Created by runner per execution |
| `StartupStepResult` | Immutable struct/class | Success, warning, recoverable failure, blocking failure, code, message, details | Returned by step execution |
| `StartupStepPolicy` | Serializable struct/class | Required/optional, failure action, timeout, skip/retry metadata | Stored in sequence entry |
| `SplashSequence` | ScriptableObject | Ordered image splash configuration | Project-owned |
| `SplashEntry` | Serializable definition | Image and timing/skip metadata | Owned by sequence |
| `SplashSequencePlayer` | Class | Deterministic clock-driven traversal of one assigned sequence | Root-owned per launch attempt |
| `IImageSplashPresenter` | Interface | Receives immutable splash frames and project-routed skip requests | Default uGUI view, project adapter, or headless fallback |
| `SplashPlaybackResult` | Immutable class | Completed splash traversal summary | Produced by player; retained only as temporary root execution evidence in FL-M4-04 |
| `LaunchDestination` | ScriptableObject | Initial validated scene target with stable identity and runtime-safe scene metadata | Project-owned |
| `LaunchReport` | Immutable class | Complete launch summary and step results | Finalized by root |
| `LaunchProgressSnapshot` | Immutable struct | Current phase, step, progress, message, elapsed time | Published by root |
| `EchoDirectSceneInitializer` | MonoBehaviour | Development-only minimum launch creation | Sample/project scene helper |
| `ILaunchStatusPresenter` | Interface | Receives status/splash/report presentation requests | Default presenter or project/test adapter |
| `IInitialDestinationLoader` | Interface | Validates and performs initial destination load | Default Unity loader or test/bridge adapter |
| `ILaunchClock` | Interface | Supplies monotonic time for timeout and test determinism | Default Unity clock or test fake |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/main-loop rule |
|---|---|---|---|---|
| `EchoLaunchRoot.Current` | Convenience access to current authority | Authority claimed | Null when no root; must not be only test seam | Main thread access |
| `EchoLaunchRoot.State` | Current launch lifecycle state | Root exists | Read-only enum/snapshot | Main thread; snapshots safe to copy |
| `EchoLaunchRoot.Progress` | Current immutable progress snapshot | Root exists | Always available after claim | Main thread publication |
| `EchoLaunchRoot.LastReport` | Final report after completion/failure | Report finalized | Null while active unless an interim snapshot API is approved | Read-only |
| `EchoLaunchRoot.LaunchCompleted` | Subscribe to successful handoff | Listener unsubscribes cleanly | Raised after destination activation and authoritative state update | Main thread |
| `EchoLaunchRoot.LaunchFailed` | Subscribe to blocking failure | Listener unsubscribes cleanly | Raised after report records failure | Main thread |
| `EchoLaunchRoot.CancelLaunch(reason)` | Request safe cancellation | Active launch and policy allows cancellation | Produces cancelled/interrupted result; cannot fake success | Main thread request |
| `StartupStepDefinition.CreateExecutor()` | Create a fresh runtime executor | Valid immutable definition | Returns one `IStartupStepExecutor`; null/exception blocks preflight | Main thread |
| `IStartupStepExecutor.ExecuteAsync(context)` | Perform one step | Valid context and one active execution | Returns `Awaitable<StartupStepResult>`; exceptions converted by runner | Begins on Unity main thread; executor must explicitly marshal any background result before Unity API use |
| `IInitialDestinationLoader.LoadAsync(destination, progress, cancellation)` | Load initial destination | Valid destination | Returns `Awaitable<InitialDestinationLoadResult>` | Starts on Unity main thread |
| `EchoDirectSceneInitializer.EnsureDevelopmentLaunch()` | Idempotently settle direct-scene authority | Component has not settled | Reuses existing authority, creates one approved root, or returns one blocked/failed result | Main thread; `Start` calls it once |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `AuthorityClaimed` | Root | After successful claim, before preflight | Root identity and launch mode | Informational; cannot veto claim |
| `LaunchStateChanged` | Root | After authoritative state changes | Previous/new state and snapshot | Presentation not required for completion |
| `StepStarted` | Runner/root | After active step is recorded | Step ID, index, metadata | No mutation of sequence asset |
| `StepProgressChanged` | Runner/root | When meaningful progress changes | Step ID, normalized/indeterminate progress, message | Throttled; no per-frame requirement |
| `StepCompleted` | Runner/root | After result is recorded | Immutable step report | Listeners cannot alter completed result |
| `LaunchCompleted` | Root | After destination activation and report completion | Final report | One event per launch |
| `LaunchFailed` | Root | After blocking failure and report completion | Final report | One event per launch |
| `LaunchInterrupted` | Root | After cancellation/shutdown report finalization | Final report | May occur on teardown |

### 10.4 Async and cancellation policy

- The public asynchronous primitive is Unity `Awaitable<T>`, supported by the approved Unity 6 floor.
- Every `StartupStepDefinition` creates a fresh `IStartupStepExecutor`; the executor returns `Awaitable<StartupStepResult>`.
- Immediate work may complete synchronously through an already-completed Awaitable, so a second synchronous API is unnecessary.
- Startup execution must not block the Unity player loop while awaiting asynchronous work.
- Every active step receives a `CancellationToken` and a package-owned progress reporter through `StartupStepContext`.
- Required legal/minimum-display splash time cannot be bypassed by general cancellation unless the project explicitly marks it cancellable.
- Timeout is measured with a monotonic, unscaled clock so pause/time scale cannot freeze launch policy.
- Cancellation is cooperative. A step that cannot safely cancel must declare that fact and finish or fail according to policy.
- Executor instances are single-use. Reusing an executor across launch attempts is invalid.
- Scene destruction or application quit finalizes an interrupted report and suppresses noisy follow-up errors.
- Re-entry is prohibited: one root cannot start a second launch session before the first reaches a terminal state.
- MVP failure actions are `ContinueWithWarning` and `BlockLaunch`; automatic retry and interactive retry UI are deferred.

### 10.5 API ergonomics

**Novice path:** Run setup, assign a configuration, add built-in/sample steps from a validated list, configure image splashes and destination, press Play.

**Programmer path:** Implement/derive a custom startup step or provide an explicit adapter, return structured results, report progress, and test with injected clock/presenter/destination loader.


---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

Create-only Setup Apply and explicit Setup Repair are separate user actions over the
same immutable request, recollected snapshot, and deterministic plan.

Create workflow:

1. Install or embed `com.echodevgames.echo-launch`.
2. Open **Tools > Sperk’s Forge > First Light > Setup**.
3. Select project root, Boot path, existing destination scene, optional splash,
   and Build Settings policy.
4. Refresh the read-only snapshot and review the deterministic plan.
5. Press **Apply Plan...** only for `Create`, `Reuse`, and `NoChange` operations.
6. Confirm the exact mutation summary.
7. The service recollects and replans before writes.
8. Create only missing project-owned targets.
9. Refresh and repeat until the result is `NoChanges`.

Repair workflow:

1. Refresh the same Setup plan after existing First Light assets drift.
2. Review every operation marked `Repair` and its before/after explanation.
3. Resolve every blocker or ambiguous ownership/shape result manually.
4. Press **Repair Plan...**.
5. Confirm the exact repair paths, fields, Build Settings changes, and backup policy.
6. The service recollects and replans, then verifies the displayed repair fingerprint.
7. Back up every existing asset that will be modified before the first repair write.
8. Apply only the approved narrow repairs.
9. Refresh and repeat until the result is `NoChanges`.

`Apply Plan...` remains create-only. It must never become a hidden alias for repair.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| Create First Light foundation | Project folders, definitions, root prefab variant, Boot scene | Build Settings only after fresh preview/approval | Yes; second/third apply are no-op | Failure rollback journal; Undo may supplement where supported | Immutable apply result |
| Repair approved generated references and canonical setup drift | Missing project-owned targets when the plan also contains repair; no replacement types | Only exact current-schema fields, verified prefab binding, verified Boot root presence, and approved Build Settings state | Yes; second/third repair are no-op | Byte-preserving asset + `.meta` backup before modification; Build Settings snapshot; rollback result | Immutable repair result with before/after records |
| Validate project | Nothing | Nothing | Yes | N/A | Validation report |
| Add built-in step | Step asset/sequence entry | Selected sequence | Yes | Undo | Change report |
| Create direct-scene helper | Project-owned helper | Selected scene after confirmation | Yes | Undo | Setup report |
| Simulate failure/delay | Temporary/in-memory test state | No production assets by default | Yes | Reset | Test report |
| Migrate schema | Upgraded project-owned assets | Selected assets after preview | Per version | Destructive migration backup | Migration report |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| First Light Setup | Novice/maintainer | Preview, create, and explicitly repair the canonical foundation | No |
| Launch Configuration Inspector | Designer/programmer | Edit references/policies with validation | No |
| Startup Sequence Inspector | Programmer | Reorder and validate steps | No |
| Splash Sequence Inspector | Designer | Preview image order/timing | No |
| First Light Validator | Tester/maintainer | Run configuration/scene/build/schema checks | No |
| Launch Simulator | Tester | Explicitly run transient deterministic startup-step scenarios and copy immutable evidence | Editor assembly only; no authored asset, scene, Build Settings, or player dependency |
| Launch Report Viewer | Tester | Inspect/copy launch reports | No |

### 11.4 Validation and repair

Validation is a separate read-only authority from Setup Apply and Repair.

The dedicated Validator runs only after explicit user action. It never invokes
mutation, auto-fix, migration, or Build Settings writes.

| Check ID | Condition | Severity | Fix boundary |
|---|---|---|---|
| ELAUNCH-VAL-001 | Canonical Boot scene missing or invalid | Blocker | Setup Apply or explicit project choice |
| ELAUNCH-VAL-002 | Multiple effective launch roots across Boot/enabled scenes | Blocker | Manual resolution; never auto-delete |
| ELAUNCH-VAL-003 | Canonical root configuration missing or mismatched | Blocker | Explicit Setup Repair when eligible |
| ELAUNCH-VAL-004 | Configuration missing, wrong type/identity, or unsupported schema | Blocker | Repair only for authorized refs; migration otherwise |
| ELAUNCH-VAL-005 | Startup sequence/entry/definition incomplete or invalid | Error | Explicit content edit |
| ELAUNCH-VAL-006 | Duplicate stable step/definition ID | Blocker | Explicit ID/content correction |
| ELAUNCH-VAL-007 | Destination missing, invalid, or not uniquely enabled | Blocker | Build Settings/project correction |
| ELAUNCH-VAL-008 | Boot entry missing, disabled, or duplicated in Build Settings | Blocker | Setup Apply/Repair policy |
| ELAUNCH-VAL-009 | Direct helper is structurally invalid, targets the wrong scene, appears in Boot, or opts into Development Builds in an enabled build scene | Warning/Blocker | Correct helper/configuration/policy; runtime release creation remains hard-prohibited |
| ELAUNCH-VAL-010 | Configured visual presentation unavailable | Warning | Assign/repair project root presentation |
| ELAUNCH-VAL-011 | Splash identity, refs, schema, or timing invalid | Error | Explicit content edit |
| ELAUNCH-VAL-012 | Required step failure/timeout policy contradictory or unsafe | Error | Explicit policy edit |
| ELAUNCH-VAL-013 | Project-owned configuration content resolves inside package source | Error | Later GUID-preserving move/manual correction |
| ELAUNCH-VAL-014 | Required evidence could not be inspected safely | Blocker | Resolve scene/asset/import failure |
| ELAUNCH-VAL-015 | Validation run already active | Warning | Wait for current run |

Health is derived from the highest finding severity:

```text
Blocker -> Blocked
Error   -> Invalid
Warning -> NeedsAttention
Info    -> Healthy
```

The Validator may suggest opening Setup, but it never performs Apply or Repair.

### 11.5 Setup architecture

```text
setup collector -> setup snapshot -> planner -> plan
    -> create-only apply service
        -> create writers
        -> Build Settings writer
        -> create rollback journal
    -> explicit repair service
        -> repair eligibility/ownership proof
        -> repair backup store
        -> asset/prefab/scene/Build Settings repair writers
        -> repair rollback
    -> immutable apply/repair result

validation evidence collector
    -> ordered read-only validation rules
    -> immutable schema-1 validation report
    -> deterministic text formatter
    -> dedicated Validator window
```

Observation and planning remain side-effect free.

Mutation begins only after confirmation, freshness validation, and the correct
create-versus-repair authority gate.

### 11.6 Immutable contracts

Planning contracts remain approved.

FL-M5-02 owns immutable apply request/approval, status, change record, result,
and request/snapshot/plan fingerprints.

FL-M5-03 adds immutable repair approval, repair candidate, repair change,
backup record, and repair result values. Results defensively copy collections
and contain project-relative paths and sanitized data, not mutable Unity objects.


### 11.6.1 Validator contracts and determinism

FL-M5-04 adds immutable `EchoLaunchValidationRequest`,
`EchoLaunchValidationFinding`, and `EchoLaunchValidationReport` values.

The report schema is version `1`. It records health, severity counts, stable
findings, target root, and request/evidence/report fingerprints. It contains no
mutable Unity objects, absolute machine paths, wall-clock timestamp, random ID,
scene handle, or object instance ID.

Unchanged evidence and the same request produce the same finding order,
fingerprints, and copied text. Validation may inspect closed scenes additively,
but it must preserve the user's open scene set, active scene, dirty states,
assets, prefabs, scenes, and Build Settings exactly.

### 11.7 Default paths

```text
Assets/EchoDevGames/FirstLight
```

Targets:

```text
Configuration/EchoLaunchConfiguration.asset
Configuration/StartupSequence.asset
Configuration/LaunchDestination.asset
Configuration/SplashSequence.asset
Prefabs/EchoLaunchRoot.prefab
Scenes/Boot.unity
```

Splash is optional. Destination scene already exists and is never modified.

Root is a project-owned variant of the stable package root template.

### 11.8 Apply and repair eligibility

Create Apply is executable only when the plan is `Ready`, or approved
`ReadyWithWarnings`, with no conflict, unsupported operation, ambiguous decision,
or repair operation.

Create dispositions:

```text
Create
Reuse
NoChange
```

Repair is executable only when:

- The plan is `Ready`, or approved `ReadyWithWarnings`.
- At least one operation is `Repair`.
- Every repair candidate has proven current type, supported schema, expected
  project-owned path, and the required prefab/scene shape where applicable.
- Every repair operation is explicitly approved.
- No `Conflict`, `Unsupported`, unresolved `ManualDecision`, migration, or
  ambiguous ownership result remains.
- No other setup mutation is active.

A repair plan may also execute `Create`, `Reuse`, and `NoChange` operations so a
partial foundation can be reconciled in one explicit repair transaction.

### 11.9 Freshness and single mutation authority

The displayed plan carries deterministic request, evidence, plan, and repair
fingerprints.

Immediately before any create or repair write, the service recollects and
replans from the same request. A mismatch aborts with `ELAUNCH-SETUP-008` before
backup or mutation.

Only one Setup mutation may be active across Apply and Repair. Re-entry is
rejected with `ELAUNCH-SETUP-009` before writes.

### 11.10 Authorized FL-M5-03 repairs

FL-M5-03 may modify only these narrowly defined surfaces:

1. **`EchoLaunchConfiguration` reference reconciliation**
   - Asset type and supported current schema must already be valid.
   - Rebind only `StartupSequence`, `LaunchDestination`, and optional
     `SplashSequence` references to the uniquely resolved planned assets.
   - Preserve stable ID, schema, root-lifetime policy, reduced-motion default,
     and every unrelated serialized value.
2. **`LaunchDestination` scene reconciliation**
   - Asset type, stable identity, and current schema must be valid.
   - Reconcile only the runtime scene path to the explicitly selected existing
     destination scene.
   - Fill the display label only when empty; never overwrite a non-empty
     project-authored label.
3. **Project root prefab binding reconciliation**
   - The asset must be a prefab variant whose lineage resolves to the stable
     First Light package root template.
   - It must contain exactly one `EchoLaunchRoot`.
   - Rebind only the root configuration reference.
   - Preserve nested presenter connection and every unrelated override.
4. **Boot scene root-presence reconciliation**
   - The scene must exist at the exact planned project path.
   - When the scene contains zero `EchoLaunchRoot` components, add one instance
     of the uniquely resolved project root prefab.
   - Preserve all unrelated scene objects, open-scene set, active scene, and
     dirty states.
   - Multiple roots, an unpacked/wrong root, or ambiguous hierarchy block repair.
5. **Build Settings reconciliation**
   - Follow the selected approved policy.
   - Add a missing canonical Boot entry or enable one uniquely identified
     disabled canonical Boot entry.
   - Place-first remains separately approved.
   - Preserve unrelated order and enabled state.
   - Duplicate or ambiguous canonical entries block repair unless an existing
     approved place-first operation already defines the exact normalization.

### 11.11 Forbidden repair and migration boundary

FL-M5-03 must not:

- Change or migrate unsupported/older/newer schema versions.
- Regenerate or replace stable IDs.
- Replace an asset with another type.
- Edit entries inside `StartupSequence` or `SplashSequence`.
- Delete duplicate roots or scene objects.
- Rebase, unpack, replace, or structurally rewrite a prefab.
- Move, rename, delete, or relocate assets.
- Modify the selected destination scene.
- Repair arbitrary project scenes or project-authored presentation.
- Persist a setup receipt, own uninstall/reset, or perform automatic crash recovery.

Ambiguity is a blocker, not permission to guess.

### 11.12 Ownership and shape proof

A target is repairable only when the package can prove all required facts from
current project evidence:

- Exact project-relative planned path.
- Expected Unity type and loadable identity.
- Supported current schema where the type is versioned.
- Unique role resolution.
- Stable package-template lineage for the root prefab.
- Exact root-count and prefab-instance shape for Boot-scene repair.

A matching filename, label, or folder is insufficient proof by itself.

### 11.13 Repair backup and rollback

Before modifying any existing asset, prefab, or scene, the service copies the
exact asset bytes and matching `.meta` bytes to:

```text
Library/EchoDevGames/FirstLight/RepairBackups/<repair-id>/
```

Rules:

- Backup is outside `Assets` and is never imported as project content.
- A failed backup aborts before the first repair write with
  `ELAUNCH-SETUP-014`.
- Build Settings is captured as a complete ordered scene array.
- On failure, modified files and `.meta` files are restored byte-for-byte,
  Build Settings is restored, and the AssetDatabase is refreshed/reimported.
- Newly created paths from the same repair transaction are removed using the
  existing active-attempt rollback rules.
- A complete successful repair removes its temporary backup directory.
- Incomplete rollback retains the backup and reports its path for manual recovery.
- Crash-persistent automatic recovery remains deferred; a surviving backup is
  evidence for manual recovery, not permission for silent startup mutation.

### 11.14 Repair result and repeatability

The immutable result records:

- Status and message.
- Displayed and final plan/repair fingerprints.
- Created, reused, repaired, and unchanged paths.
- Per-repair field/surface summary with sanitized before/after values.
- Build Settings before/after summary.
- Whether rollback completed.
- Retained backup/manual-recovery paths, when any.

After a successful repair, second and third Repair return `NoChanges`, GUIDs and
stable IDs remain unchanged, no duplicate Boot root or Build Settings entry is
created, and package templates remain not dirty.

### 11.15 Setup window

The Setup window may display:

- Create and repair eligibility separately.
- `Apply Plan...` for create-only work.
- `Repair Plan...` for explicitly repairable work.
- Per-repair before/after explanations.
- Required approval and final confirmation.
- Result, Copy Result, and project-asset pinging.

It may not silently repair during Refresh, ordinary inspector drawing, package
import, Play Mode entry, or create-only Apply.

### 11.16 Stable setup diagnostics

- `ELAUNCH-SETUP-001` invalid path/request.
- `ELAUNCH-SETUP-002` incompatible target.
- `ELAUNCH-SETUP-003` migration required.
- `ELAUNCH-SETUP-004` Build Settings reorder approval.
- `ELAUNCH-SETUP-005` ambiguous candidates.
- `ELAUNCH-SETUP-006` package prerequisite unavailable.
- `ELAUNCH-SETUP-007` compatible asset reuse.
- `ELAUNCH-SETUP-008` stale plan.
- `ELAUNCH-SETUP-009` another Setup mutation is active.
- `ELAUNCH-SETUP-010` create Apply failed; rollback completed.
- `ELAUNCH-SETUP-011` create rollback incomplete.
- `ELAUNCH-SETUP-012` unauthorized operation for the selected action.
- `ELAUNCH-SETUP-013` explicit repair approval required.
- `ELAUNCH-SETUP-014` repair backup could not be secured.
- `ELAUNCH-SETUP-015` repair ownership or shape cannot be proven.
- `ELAUNCH-SETUP-016` repair failed; rollback completed.
- `ELAUNCH-SETUP-017` repair rollback incomplete; backup retained.
---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

MVP-supported routes:

- Embedded package during package development.
- Local disk package reference.
- Generated `.tgz` tarball installed through Unity Package Manager.
- Git URL installation after repository/tag release.

The Workshop route is later and not required for First Light release.

### 12.2 Minimal scene setup

A production Boot scene requires:

- One active `EchoLaunchRoot`.
- One assigned project-owned `EchoLaunchConfiguration`.
- One status/splash presenter instance or root-owned default presenter prefab.
- A valid startup sequence.
- A valid final destination included in build scenes.
- No unrelated Sperk’s Forge package.

### 12.3 Boot-scene setup

- The setup tool creates or validates a canonical Boot scene selected by the project.
- The scene name and path are project-owned and never hard-coded in runtime APIs.
- The Boot scene may be build index zero, but the tool must not silently reorder an existing project without preview and approval.
- The root claims authority in `Awake`, validates, then starts launch through the approved start gate.
- The root persists at least until destination activation and handoff.

### 12.4 Direct-scene setup

A project may add `EchoDirectSceneInitializer` to a gameplay or Test Lab scene.

#### Runtime order

1. Scene-authored `EchoLaunchRoot` objects claim authority in `Awake`.
2. The initializer settles once in `Start`.
3. If `EchoLaunchRoot.Current` already exists, the initializer reuses it and creates nothing.
4. If no authority exists, the initializer validates its environment, policy, project-owned `DirectSceneConfiguration`, direct root prefab, launch mode, configuration, and containing-scene destination.
5. The initializer instantiates exactly one direct root prefab.
6. The instantiated root claims through the normal `EchoLaunchRoot.Awake` path and runs the same splash, sequence, report, destination, duplicate, and lifetime rules as canonical Boot.

Multiple initializers cannot create multiple accepted roots. The first accepted prefab claims in its own `Awake`; later initializers reuse that authority.

#### Project-owned direct configuration

`DirectSceneConfiguration` is a project-owned immutable ScriptableObject with schema version `1`, a stable configuration ID, one explicit project-owned direct root prefab, and one `DirectSceneEntryPolicy`.

The referenced prefab must:

- Contain exactly one active `EchoLaunchRoot`.
- Be authored with `LaunchMode.DirectSceneDevelopment`.
- Reference a supported project-owned `EchoLaunchConfiguration`.
- Use a destination whose scene path exactly matches the scene containing the initializer.
- Retain approved package-template lineage when created from the standard project root.
- Be assigned explicitly, never discovered through `Resources`, labels, filenames, reflection, or scene-wide search.

The helper never rewrites the prefab, launch configuration, destination, or scene at runtime.

#### Environment policy

Supported policy values:

```text
EditorOnly
EditorAndDevelopmentBuilds
BootRequired
```

`EditorOnly` is the default.

- `EditorOnly` permits creation only while running in the Unity Editor.
- `EditorAndDevelopmentBuilds` also permits creation when `Debug.isDebugBuild == true`; this is an explicit project opt-in.
- `BootRequired` never creates a root and emits `ELAUNCH-DIRECT-001`.
- A non-development player build is prohibited unconditionally. No serialized value enables release execution.
- Existing authority reuse creates no development root and remains safe in every environment.

No build hook is added by FL-M5-05. Runtime code itself makes release root creation impossible.

#### Active-destination handoff

A direct configuration targets the scene already open for direct testing.

`UnityInitialDestinationLoader` treats an already loaded, active configured destination as a successful no-reload handoff:

- Progress settles to `1`.
- No `LoadSceneAsync` operation begins.
- The scene is not unloaded or reloaded.
- The final report remains schema version `2`.
- `LaunchReport.LaunchMode` is `DirectSceneDevelopment`.
- Destination identity and display metadata remain authored values.

Canonical Boot behavior remains unchanged because its destination is not already active during normal startup.

#### Observable settlement

Stable statuses:

```text
NotStarted
ReusedExistingAuthority
CreatedDevelopmentAuthority
BlockedByPolicy
BlockedByEnvironment
InvalidConfiguration
InstantiationFailed
```

Stable runtime diagnostics:

- `ELAUNCH-DIRECT-001` policy or environment prohibits direct entry.
- `ELAUNCH-DIRECT-002` direct configuration, prefab, launch mode, launch configuration, or destination is invalid.
- `ELAUNCH-DIRECT-003` direct root instantiation failed unexpectedly.

The helper records one immutable/read-only settlement result, logs at most one sanitized message, and disables further helper behavior. It does not become a persistent service.

### 12.6 Launch Simulator

The **First Light Launch Simulator** is an explicit Editor-only diagnostic tool.

Open:

```text
Tools > Sperk's Forge > First Light > Simulator
```

Opening, repainting, reloading, entering Play Mode, or importing assets does not
start a simulation. The user must press `Run Simulation`.

#### Scope boundary

The Simulator proves startup-step execution semantics. It does not claim launch
authority, play splash presentation, load a destination, modify Build Settings,
or pretend that a full Boot launch completed.

The later Standalone Laboratory remains responsible for visible root,
presentation, destination, duplicate, and Boot-to-destination acceptance.

#### Transient simulation model

Every run builds transient `HideAndDontSave` configuration, sequence, entry, and
step-definition objects in memory. The Simulator never:

- Writes a project asset.
- Edits an authored `EchoLaunchConfiguration`.
- Edits an authored `StartupSequence`.
- Adds a scene object.
- Saves or dirties a scene.
- Changes Build Settings.
- Enters Play Mode automatically.
- Stores simulation state in runtime ScriptableObjects.

Transient objects are destroyed after settlement, including cancellation and
failure paths.

#### Real runner, separate report truth

Simulation uses the real `StartupSequenceRunner`, `StartupStepPolicy`,
`StartupStepResult`, progress gate, timeout monitor, exception conversion, and
sequence traversal behavior.

It produces `LaunchSimulationReport` schema version `1`, not `LaunchReport`.

This separation is mandatory because the Simulator does not claim a root,
activate presentation, or complete a destination handoff. A simulation report
must not falsely claim a completed launch.

#### Built-in scenario presets

The first version supports:

```text
ImmediateSuccess
TimedProgressSuccess
WarningContinues
RecoverableFailureContinues
BlockingFailureStops
TimeoutStops
ExecutorExceptionStops
Cancellation
```

Presets may expose bounded parameters such as logical duration, progress sample
count, timeout, and message text. Unsupported values block before transient
objects are created.

Warning and recoverable-failure presets include a later success step so
continuation is observable. Blocking, timeout, exception, and cancellation
presets include an unvisited later step so stopping behavior is observable.

#### Deterministic logical timing

Simulation timing is logical rather than wall-clock evidence.

- A simulation clock begins at `0`.
- Timed executors advance the clock through deterministic scheduled samples.
- Progress samples have authored logical timestamps.
- Timeout settlement uses the same runner timeout contract.
- Identical accepted requests produce identical semantic reports,
  fingerprints, ordering, progress samples, and copied text.
- UI repaint frequency, machine performance, and wall-clock date are excluded
  from report truth.

The window may animate logical progress for readability, but animation does not
change accepted evidence.

#### Immutable contracts

`LaunchSimulationRequest` contains:

- Report schema version.
- Scenario preset.
- Logical duration.
- Progress sample count.
- Timeout.
- Stable optional message.
- Deterministic request fingerprint.

`LaunchSimulationReport` contains:

- Schema version `1`.
- Simulator status.
- Request, plan, and report fingerprints.
- Scenario preset and normalized parameters.
- Authored, disabled, attempted, and unvisited counts.
- Ordered immutable step evidence.
- Ordered immutable progress evidence.
- Final effective result.
- Cancellation state.
- Sanitized simulator diagnostic.
- Deterministic copied text.

No Unity object reference survives report construction.

#### Status and diagnostics

Stable statuses:

```text
NotRun
Completed
Cancelled
InvalidRequest
Busy
InfrastructureFailure
```

Stable diagnostics:

- `ELAUNCH-SIM-001` invalid or unsupported request.
- `ELAUNCH-SIM-002` a simulation is already active.
- `ELAUNCH-SIM-003` simulation cancelled by the user.
- `ELAUNCH-SIM-004` transient-plan or simulator infrastructure failure.

Built-in simulated step results use:

- `ELAUNCH-SIM-STEP-001` warning.
- `ELAUNCH-SIM-STEP-002` recoverable failure.
- `ELAUNCH-SIM-STEP-003` blocking failure.

Timeout and executor exceptions continue through the existing canonical step
diagnostics and exception conversion. The Simulator does not create competing
timeout or exception semantics.

#### Single-active-run and cancellation

Only one simulation may run at a time.

- Re-entry returns a structured `Busy` report with `ELAUNCH-SIM-002`.
- `Cancel Simulation` requests cooperative cancellation.
- Cancellation settles through the real runner cancellation path.
- The window never abandons transient objects or an active runner.
- Closing the window requests cancellation and completes cleanup without
  logging an unhandled exception.

#### Release and dependency boundary

All Simulator window, transient authoring, scenario executor, logical clock,
formatter, and orchestration code lives in the Editor assembly.

The Runtime assembly may grant the package Editor assembly internal access to
the existing runner/reporting seams, but FL-M5-06 adds no simulator type or
player behavior to the Runtime assembly.

The Simulator:

- Is absent from player builds.
- Adds no scripting define.
- Adds no build hook.
- Adds no peer-package dependency.
- Adds no reflection or hidden discovery.
- Adds no dependency on Samples or the later Laboratory.

### 12.5 Scene isolation rule

The Standalone Test Lab contains no Jukebot, EchoUI, EchoSave, EchoSettings, EchoSceneFlow, EchoGameState, EchoInput, EchoDiagnostics, or project-specific runtime assembly. Test utilities live only in the sample/test assemblies.

---

## 13. Standalone Test Lab and Samples

### 13.1 Standalone Test Lab purpose

The **First Light Standalone Test Lab** proves the complete MVP launch loop in isolation: claim authority, validate, show image/status presentation, run ordered steps, handle warnings/failures, generate a report, load a destination scene, and support direct-scene development.

### 13.2 Required Test Lab contents

- `FirstLight_Boot_Lab.unity`.
- `FirstLight_Destination_Lab.unity`.
- Optional `FirstLight_DirectScene_Lab.unity` if direct entry cannot be demonstrated cleanly in the destination scene.
- Minimal project-owned sample configuration and sequence assets.
- Redistributable placeholder image splashes.
- Built-in sample steps: immediate success, timed progress, warning, recoverable failure, blocking failure.
- On-screen plain state readout.
- Controls to select a scenario before launch or via separate validated configurations.
- Duplicate-root test setup.
- Reset instructions.
- Sample README with exact import/setup/test steps.

### 13.3 Test Lab acceptance checklist

| Test | Action | Expected result | Automated/manual | Status |
|---|---|---|---|---|
| LAB-001 | Play Boot Lab with success configuration | Steps execute in order, destination activates, report is successful | Both | Not run |
| LAB-002 | Use timed progress step | Status remains responsive and progress/timing are recorded | Both | Not run |
| LAB-003 | Run warning step | Launch continues and report records warning | Both | Not run |
| LAB-004 | Remove required configuration | Launch blocks before steps with ELAUNCH-CFG-001 | Both | Not run |
| LAB-005 | Run blocking failure step | Destination does not load; report ends failed | Both | Not run |
| LAB-006 | Enable two roots | First claimant runs once; duplicate has zero side effects and ELAUNCH-ROOT-001 | Both | Not run |
| LAB-007 | Configure invalid destination | Preflight blocks with ELAUNCH-DEST-001 | Both | Not run |
| LAB-008 | Open destination/direct scene and press Play | One development root is created, report marks direct-scene mode | Both | Not run |
| LAB-009 | Start direct scene with existing root | Helper reuses authority and creates no duplicate | Both | Not run |
| LAB-010 | Attempt skip before minimum splash duration | Entry remains until policy permits skip | Manual + PlayMode timing test | Not run |
| LAB-011 | Delete sample assets after import | Runtime package remains compiling and setup tool opens | Manual clean-project | Not run |
| LAB-012 | Re-run setup and repair three times | No duplicates or silent overwrite | Manual + EditMode where practical | Not run |

### 13.4 Optional showcase and integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| First Light + Observatory | EchoLaunch, EchoDiagnostics, bridge | Visualize launch graph/report in runtime dashboard | Requires another package |
| First Light + Passage | EchoLaunch, EchoSceneFlow, bridge | Delegate final transition and loading presentation | Requires scene-flow authority |
| First Light + Resonance | EchoLaunch, Jukebot, bridge | Initialize audio and request startup music through Jukebot | Requires audio package |
| First Light + Looking Glass | EchoLaunch, EchoUI, bridge | Replace plain presenter with styled startup screen | Requires UI package |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

A minimal plain startup status and image splash presenter is part of the EchoLaunch MVP because the package must explain startup without EchoUI or EchoDiagnostics. It is deliberately narrow and startup-only. General screen navigation, menus, themes, HUDs, modal stacks, and notifications remain outside the package.

### 14.2 Required states

- Authority claimed.
- Preflight/validating.
- Ready to begin.
- Showing splash.
- Running step with determinate progress.
- Running step with indeterminate progress.
- Warning/continuing.
- Failure/blocked.
- Loading destination.
- Launch complete/handoff.
- Development direct-scene mode.
- Disabled/unavailable presenter fallback.

### 14.3 Accessibility requirements

- Status must include text, not color alone.
- Progress must support determinate and indeterminate states.
- Text must be replaceable/localization-ready even before EchoLocalization integration.
- Minimum display timing must be configurable.
- Skip controls must be configurable and cannot bypass required/legal minimum duration.
- Reduced-motion mode must permit immediate state changes or simple fades instead of animated movement.
- Fade duration may be zero.
- Default visuals must use readable contrast and scalable layout anchors.
- Keyboard/controller skip input is project-configured; the core presenter must also expose a public skip-request method so it does not require EchoInput.
- The package must not require audio cues to understand status.

### 14.4 Visual customization

- Project logos, branded backgrounds, project fonts, production colors, final layout variants, and localized copy are project-owned.
- EchoLaunch may ship immutable neutral template prefabs that provide a readable structural starting point without claiming project branding.
- Projects customize through a copied prefab, prefab variant, or replacement presenter rather than editing immutable package assets.
- Runtime behavior binds through `ILaunchStatusPresenter`; replacing presentation must not require editing sequence-runner code.
- Sample art is removable and not referenced by production defaults.

### 14.5 Default package prefab templates

EchoLaunch ships two stable package assets:

```text
Presentation.UGUI/Prefabs/EchoLaunchStatusView.prefab
Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab
```

`EchoLaunchStatusView.prefab` is a self-contained neutral Screen Space Overlay
Canvas template containing the existing `EchoLaunchStatusView` and all required
serialized references.

`EchoLaunchRoot.prefab` contains one `EchoLaunchRoot` and a nested instance of
`EchoLaunchStatusView.prefab`. The presenter reference is wired, while the
project-owned launch configuration remains intentionally unassigned.

Template rules:

- No project logo, branded image, project font, or project-owned asset.
- No TextMeshPro dependency.
- No `EventSystem`, input module, `Button`, or package-owned skip binding.
- No `Resources`, Addressables, scene search, or hidden runtime instantiation.
- All display graphics are non-raycast targets.
- The progress slider is non-interactable.
- The Canvas begins hidden through `CanvasGroup`.
- The package uses readable neutral contrast, scalable anchors, and replaceable
  serialized copy.
- Setup tooling may later copy or instantiate these templates into project-owned
  assets and scenes without replacing existing project variants.
- Both prefab `.meta` files are committed and their GUIDs are preserved.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| Current launch state | Read-only API + status view | All builds when presenter enabled | Negligible |
| Active step ID/name/index | API + status view + report | All builds | Negligible |
| Step progress and elapsed time | API + status view | All builds | Low, throttled |
| Configuration and sequence identity | Report/Inspector | Development and release-safe IDs | Negligible |
| Warnings/failures with codes | Report + categorized logs | All builds | Event-driven |
| Duplicate authority record | Log/report | All builds | Negligible |
| Step timing summary | Final report | All builds, configurable detail | Low |
| Full exception details | Editor/development only by default | Development | On failure only |
| Build/scene validation | Editor validator | Editor only | Manual/preflight |

### 15.2 Structured status

EchoLaunch exposes:

- Package version.
- Launch report schema version.
- Authority instance identity.
- Launch mode: canonical Boot or direct-scene development.
- Configuration and sequence stable IDs.
- Lifecycle state and current phase.
- Active step ID, index, count, description, required/optional status.
- Normalized or indeterminate progress.
- Elapsed launch and step time.
- Current warning/error code and sanitized message.
- Final destination identity.
- Final launch outcome.

### 15.3 Diagnostic codes

| Code | Severity | Meaning | User action |
|---|---|---|---|
| ELAUNCH-ROOT-001 | Warning/Error | Duplicate launch root rejected | Remove duplicate scene/prefab root or confirm direct helper configuration |
| ELAUNCH-CFG-001 | Blocker | Configuration missing | Assign/create `EchoLaunchConfiguration` |
| ELAUNCH-CFG-002 | Blocker | Configuration schema unsupported | Run approved migration or use compatible package version |
| ELAUNCH-SEQ-001 | Blocker/Warning | Startup sequence missing or invalid | Assign/repair sequence and policy |
| ELAUNCH-STEP-001 | Error | Null/invalid step entry | Repair sequence entry |
| ELAUNCH-STEP-002 | Blocker | Duplicate step ID | Resolve IDs before Play/build |
| ELAUNCH-STEP-003 | Error | Step timed out | Inspect step policy/dependency and timeout |
| ELAUNCH-STEP-004 | Error | Step threw/unhandled failure | Inspect development details and step implementation |
| ELAUNCH-VIEW-001 | Warning/Blocker | Required presenter unavailable | Assign default or project presenter |
| ELAUNCH-SPLASH-001 | Blocker | Assigned splash sequence is invalid or unsupported | Repair/remove the assigned sequence or use compatible assets |
| ELAUNCH-SPLASH-002 | Blocker | Splash playback failed unexpectedly | Inspect presenter, clock, cancellation, and development details |
| ELAUNCH-SPLASH-003 | Warning | Splash is configured without a visual splash presenter | Assign an `IImageSplashPresenter` or accept headless timing |
| ELAUNCH-DEST-001 | Blocker | Destination invalid/not build-loadable | Assign a valid scene and update build settings |
| ELAUNCH-DEST-002 | Blocker | Destination load failed | Inspect scene/build/platform error |
| ELAUNCH-DIRECT-001 | Warning | Direct-scene entry is prohibited by policy or runtime environment | Start from Boot or use an approved Editor/development policy |
| ELAUNCH-DIRECT-002 | Blocker | Direct configuration, prefab, launch mode, launch configuration, or destination is invalid | Assign a supported project-owned direct configuration |
| ELAUNCH-DIRECT-003 | Error | Direct root instantiation failed unexpectedly | Inspect the prefab/runtime condition and start from Boot while unresolved |
| ELAUNCH-SIM-001 | Blocker | Launch Simulator request is invalid or unsupported | Correct the selected scenario and bounded parameters |
| ELAUNCH-SIM-002 | Warning | A Launch Simulator run is already active | Wait for settlement or cancel the active simulation |
| ELAUNCH-SIM-003 | Information | Launch Simulator run was cancelled by the user | Run the scenario again when ready |
| ELAUNCH-SIM-004 | Error | Transient simulation planning or execution infrastructure failed | Copy the sanitized report and inspect package implementation |
| ELAUNCH-LIFE-001 | Info/Warning | Launch interrupted during shutdown/destruction | Review lifecycle only if unexpected |
| ELAUNCH-SETUP-001 | Blocker | Setup request contains an invalid or non-project asset path | Correct the project-owned path before apply |
| ELAUNCH-SETUP-002 | Blocker | Planned target path contains an incompatible existing asset | Select another path or resolve the conflict manually |
| ELAUNCH-SETUP-003 | Blocker | Existing configuration schema requires an unsupported migration | Run a separately approved migration workflow |
| ELAUNCH-SETUP-004 | Warning/Manual | Requested Build Settings policy would reorder existing scenes | Review and explicitly approve placement before apply |
| ELAUNCH-SETUP-005 | Warning/Manual | More than one compatible candidate exists for a required role | Select the intended project asset |
| ELAUNCH-SETUP-006 | Blocker | Required package template or script identity is unavailable | Repair/reinstall the package before setup |
| ELAUNCH-SETUP-007 | Info | Existing compatible project asset will be reused | Review the planned reference target |
| ELAUNCH-SETUP-008 | Blocker | Project evidence changed after preview | Refresh and review the new plan |
| ELAUNCH-SETUP-009 | Warning | Another setup apply is active | Wait for settlement |
| ELAUNCH-SETUP-010 | Error | Apply failed and active-attempt changes were rolled back | Review failure and refresh |
| ELAUNCH-SETUP-011 | Blocker | Rollback was incomplete | Recover named paths manually |
| ELAUNCH-SETUP-012 | Blocker | Plan contains an operation outside the selected create/repair authority | Use the correct explicit action or a later migration workflow |
| ELAUNCH-SETUP-013 | Warning/Manual | One or more safe current-schema repair operations require explicit approval | Review every repair and confirm Repair Plan |
| ELAUNCH-SETUP-014 | Blocker | Required repair backup could not be secured | Resolve filesystem/access issue before repair |
| ELAUNCH-SETUP-015 | Blocker | Existing target ownership or shape cannot be proven safe for repair | Resolve manually or select a canonical compatible target |
| ELAUNCH-SETUP-016 | Error | Repair failed and modified/created content was rolled back | Review failure, refresh, and retry only after correction |
| ELAUNCH-SETUP-017 | Blocker | Repair rollback was incomplete and backup was retained | Recover from the named backup paths before continuing |


### 15.4 Observatory bridge

A separate optional bridge will expose a read-only provider containing the configured step graph, current state, timing, warnings, failures, authority identity, launch mode, and final report. EchoLaunch must not reference EchoDiagnostics assemblies.

### 15.5 Logging policy

- Logs use package category `EchoLaunch` and stable codes.
- Normal state changes are not logged every frame.
- One duplicate produces one actionable record.
- Release logs omit file-system paths, stack traces, and sensitive project details unless explicitly enabled.
- Full exception/step detail is available in Editor/development reports.
- Logs supplement, not replace, the structured result/report API.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| Launch configuration assets | Project configuration | Project/EchoLaunch tooling | Unity asset serialization | ScriptableObject assets |
| Active launch session | Session | EchoLaunchRoot | No | Runtime memory |
| Final launch report | Session/support artifact | EchoLaunchRoot/caller | No by default | Runtime memory; later optional export |
| Direct-scene mode selection | Project/development | Project | Asset/editor setting | Project-owned configuration |
| User game/save state | Slot/profile | EchoSave/project | Not by EchoLaunch | External authority |

### 16.2 Standalone behavior

Without EchoSave or EchoSettings, EchoLaunch runs entirely from project-owned launch assets and session state. It does not create save files, `PlayerPrefs` keys, user profiles, or settings records in the MVP.

### 16.3 Optional participant/provider contract

Not applicable for MVP. Future save-aware destination selection occurs through a bridge/provider that asks EchoSave or project code for a destination decision. EchoLaunch receives only a structured destination result and does not inspect save payloads.

### 16.4 Failure and recovery

- Missing or corrupt project assets are detected by Editor validation and runtime preflight.
- Runtime never silently rewrites configuration.
- Unsupported asset versions block with a migration instruction.
- A failed destination resolution or load leaves the Boot/status scene active and preserves the report.
- A later export feature may write report files, but file path, privacy, overwrite, and schema policy require a separate approved design.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Optional connections are explicit, removable, and versioned. Installing another package does not silently alter the EchoLaunch core. A bridge contributes a startup step, destination loader/resolver, presenter, or read-only diagnostics provider through documented seams.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| EchoDiagnostics | Separate two-package bridge | Bridge repository/package | EchoLaunch → Diagnostics | State, graph, progress, timings, report | No |
| EchoSceneFlow | Separate two-package bridge | Bridge repository/package | EchoLaunch → SceneFlow | Final transition request/progress/result | No |
| EchoSettings | Tiny bridge or separate bridge, to be decided in integration spec | Integration owner | EchoLaunch → Settings | Initialize/load request and structured result | No |
| EchoSave | Separate bridge | Integration package | EchoLaunch → Save | Initialize access and result; no save schema knowledge | No |
| Jukebot | Separate bridge when it depends directly on both packages | Integration package | EchoLaunch → Jukebot | Initialize request/result; optional startup audio requests remain Jukebot authority | No |
| EchoUI | Separate bridge | Integration package | EchoUI presenter reads EchoLaunch status | Status, progress, errors, skip request | No |
| EchoGameState | Bridge | Integration package | Bidirectional requests/events | Booting/loading/handoff state requests and results | No |
| Project code | Project-local adapter/custom step | Project | Project-defined | Narrow serialized references and structured step result | No |

### 17.3 Bridge placement decision

Default rule:

- A bridge directly referencing EchoLaunch and another optional Echo package ships separately unless compile-time exclusion fully prevents the second dependency and clean removal remains obvious.
- Project-specific initialization remains project-local.
- Provider/vendor integrations never enter EchoLaunch core.

### 17.4 Integration failure behavior

- Missing peer: bridge package should not be installed; EchoLaunch core is unaffected.
- Disabled peer: contributed step returns an actionable warning or blocking result according to project policy.
- Version mismatch: bridge validation blocks setup/build with compatibility guidance.
- Initialization order: bridge step’s sequence position and declared dependencies are visible; no hidden `Awake` ordering assumption.
- Teardown: bridge unsubscribes/cancels cleanly and does not destroy the peer’s authority.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Idle allocations after handoff | 0 B/frame attributable to EchoLaunch | Profiler in destination lab | Required |
| Active status update allocations | No avoidable per-frame GC; progress events throttled | Profiler in timed-step lab | No recurring GC spikes from status polling |
| Duplicate detection | Constant-time authority claim | PlayMode test/profiler | No scene-wide repeated search per frame |
| Launch report growth | Linear in step count with bounded message/detail fields | Stress sequence test | Stable at advertised max step count |
| Root overhead after handoff | No Update loop unless explicitly required by retained diagnostics | Destination lab | Near-zero CPU |
| Preflight | Completes within one frame for ordinary MVP sequence sizes or reports async validation policy | 100-step synthetic test | No user-visible freeze at advertised limit |

### 18.2 Allocation policy

- No LINQ in per-frame or progress-hot paths.
- No reflection-based package discovery in MVP.
- Event payloads use immutable snapshots and avoid uncontrolled string rebuilding.
- Logs are event-driven.
- Presentation polling is avoided; root pushes meaningful changes.
- Report detail strings are bounded/sanitized to prevent runaway memory from third-party steps.

### 18.3 Scene and domain reload behavior

- Static authority claim resets through a Unity subsystem-registration hook suitable for domain-reload-disabled Play Mode.
- All event subscriptions are removed on shutdown/destruction.
- A destroyed duplicate cannot leave static ownership pointing to itself.
- Re-entering Play Mode after an interrupted launch starts with a clean authority state.
- Tests cover domain reload enabled and disabled configurations supported by the baseline.

### 18.4 Scalability limits

Approved initial MVP limits, subject to validation before the public release claim:

- Recommended: up to 32 startup steps and 16 splash entries.
- Tested: at least 100 startup steps for report/preflight behavior and 32 splash entries for sequence handling.
- The package must degrade through longer startup time and larger reports, not corrupted order or skipped results.
- Exact public limits are confirmed by M3 stress tests before release documentation is finalized.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

EchoLaunch does not intentionally handle credentials, personal data, analytics, network identities, or save payloads. Reports may contain project scene identifiers, package versions, step IDs, timings, and sanitized failure messages. Full file paths and stack traces are development-only by default.

### 19.2 Trust boundaries

- Custom steps are trusted project/package code but their exceptions and messages are sanitized before release-facing presentation.
- Scene destinations are validated against configured build-loadable scenes.
- Runtime does not execute file paths, URLs, or arbitrary serialized method names.
- Future report export must validate destination path and avoid silent overwrite.
- Bridge/provider responses are converted to structured results; EchoLaunch does not accept them as proof of gameplay authority.

### 19.3 Platform behavior

| Platform | Supported? | Special behavior | Validation required |
|---|---:|---|---|
| Windows | Yes for initial development baseline | Standard image splashes and async scene load | Editor + player build |
| macOS | Planned for MVP if supported by approved Unity floor | Same core behavior; file/report export later needs path review | Clean build/test |
| Linux | Planned for MVP if supported by approved Unity floor | Same core behavior | Clean build/test |
| WebGL | Planned, subject to async primitive and build validation | No blocking waits; report export limitations | Player test |
| Mobile | Planned, subject to lifecycle tests | Suspend/resume during launch and touch skip input | Device test before claim |
| Console | Unknown/planned later | Platform legal-screen and certification requirements may require adapters | Provider/platform approval |

No platform is claimed in release notes until its gate passes.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-launch/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   │   ├── Installation.md
│   │   ├── Quick Start.md
│   │   ├── Setup and Repair.md
│   │   ├── Standalone Test Lab.md
│   │   └── Troubleshooting.md
│   └── Developer/
│       ├── Architecture.md
│       ├── Public API.md
│       ├── Diagnostics.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Runtime/
│   ├── Core/
│   ├── Configuration/
│   ├── Steps/
│   ├── Reporting/
│   ├── Presentation/
│   ├── SceneLoading/
│   ├── Development/
│   ├── Prefabs/
│   └── EchoDevGames.EchoLaunch.Runtime.asmdef
├── Editor/
│   ├── Setup/
│   ├── Validation/
│   ├── Inspectors/
│   ├── Simulation/
│   ├── Migration/
│   └── EchoDevGames.EchoLaunch.Editor.asmdef
├── Samples~/
│   └── First Light Standalone Test Lab/
│       ├── README.md
│       ├── Scenes/
│       ├── Configuration/
│       ├── Steps/
│       └── Art/
└── Tests/
    ├── Editor/
    │   └── EchoDevGames.EchoLaunch.Tests.Editor.asmdef
    └── Runtime/
        ├── EditMode/
        ├── PlayMode/
        └── EchoDevGames.EchoLaunch.Tests.Runtime.asmdef
```

### 20.2 Proposed source tree

```text
Runtime/
├── Core/
│   ├── EchoLaunchRoot.cs
│   ├── EchoLaunchState.cs
│   ├── LaunchMode.cs
│   ├── LaunchSession.cs
│   └── LaunchAuthorityClaim.cs
├── Configuration/
│   ├── EchoLaunchConfiguration.cs
│   ├── RootLifetimePolicy.cs
│   └── DirectSceneConfiguration.cs
├── Steps/
│   ├── StartupSequence.cs
│   ├── StartupSequenceEntry.cs
│   ├── StartupStepDefinition.cs
│   ├── IStartupStepExecutor.cs
│   ├── StartupStepContext.cs
│   ├── StartupStepPolicy.cs
│   ├── StartupStepResult.cs
│   └── StartupSequenceRunner.cs
├── Reporting/
│   ├── LaunchProgressSnapshot.cs
│   ├── LaunchReport.cs
│   ├── StartupStepReport.cs
│   └── LaunchReportBuilder.cs
├── Presentation/
│   ├── ILaunchStatusPresenter.cs
│   ├── SplashSequence.cs
│   └── SplashEntry.cs
├── SceneLoading/
│   ├── LaunchDestination.cs
│   ├── IInitialDestinationLoader.cs
│   ├── UnityInitialDestinationLoader.cs
│   └── InitialDestinationLoadResult.cs
└── Development/
    └── EchoDirectSceneInitializer.cs

Presentation.UGUI/
├── EchoLaunchStatusView.cs
└── Prefabs/
    ├── EchoLaunchStatusView.prefab
    └── EchoLaunchRoot.prefab

Editor/
└── Setup/
    ├── EchoLaunchProjectSnapshot.cs
    ├── EchoLaunchProjectSnapshotCollector.cs
    ├── EchoLaunchSetupDiagnosticCodes.cs
    ├── EchoLaunchSetupEnums.cs
    ├── EchoLaunchSetupPaths.cs
    ├── EchoLaunchSetupPlanModels.cs
    ├── EchoLaunchSetupPlanTextFormatter.cs
    ├── EchoLaunchSetupPlanner.cs
    ├── EchoLaunchSetupRequest.cs
    ├── EchoLaunchSetupFingerprint.cs
    ├── EchoLaunchSetupApplyModels.cs
    ├── EchoLaunchSetupApplyService.cs
    ├── EchoLaunchSetupAssetWriter.cs
    ├── EchoLaunchSetupPrefabWriter.cs
    ├── EchoLaunchSetupSceneWriter.cs
    ├── EchoLaunchSetupBuildSettingsWriter.cs
    ├── EchoLaunchSetupRollbackJournal.cs
    ├── EchoLaunchSetupApplyResultFormatter.cs
    └── EchoLaunchSetupWindow.cs
```

The exact file list is not implementation authorization. It is a proposed ownership map to review at M0.

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `EchoDevGames.EchoLaunch.Runtime` | Runtime | Unity core and scene management only | Yes | Neutral launch authority, definitions, reports, destination contracts, and presentation interfaces |
| `EchoDevGames.EchoLaunch.Presentation.UGUI` | Runtime presentation | Runtime assembly and Unity UI | Yes | Default status/splash presenter and prefabs; removable without changing launch authority |
| `EchoDevGames.EchoLaunch.Editor` | Editor | Runtime, optional UGUI presentation metadata, and UnityEditor APIs | No | Setup, validation, inspectors, simulation, migration, and Workshop facade |
| `EchoDevGames.EchoLaunch.Tests.Runtime` | Editor test runner/player test as configured | Runtime assembly and Unity Test Framework | No | EditMode/PlayMode neutral runtime tests |
| `EchoDevGames.EchoLaunch.Tests.Presentation.UGUI` | Runtime presentation tests | Runtime, UGUI presentation, and Unity Test Framework | No | Presenter, splash, and removal-boundary tests |
| `EchoDevGames.EchoLaunch.Tests.Editor` | Editor | Runtime + Editor assemblies and Unity Test Framework | No | Setup/validation/migration tests |

### 20.4 Repository files

The repository must include README, package documentation, visible Current Notes link, Obsidian-compatible links, changelog, license, third-party notices, contribution/development notes, release checklist, stable `.meta` files, and a compatibility entry for the central Sperk’s Forge catalog.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | 6000.0 | 6000.3.8f1 primary development baseline; additional Unity 6 versions added only after validation | The package manifest uses the Unity 6 floor without claiming untested versions |
| Unity UI (uGUI) | Baseline-compatible Unity 6 package | Version shipped with/tested against 6000.3.8f1 | Required only by `EchoDevGames.EchoLaunch.Presentation.UGUI`; the neutral Runtime assembly remains uGUI-free |
| Unity Test Framework | Baseline-compatible | Baseline version | Development/test only |

### 21.2 Semantic versioning policy

- **Patch:** bug fixes, diagnostics wording/codes additions that do not change meaning, Editor validation improvements, documentation fixes, and non-breaking presentation fixes.
- **Minor:** additive startup-step API, new optional policy, new report fields with backward-compatible schema handling, new setup tools, or new sample capability.
- **Major:** breaking public API, changed step execution contract, changed asset serialization without automatic migration, changed diagnostic-code meaning, changed root/duplicate semantics, or removal of supported behavior.
- Package version and configuration/report schema versions are related but not assumed identical.

### 21.3 Deprecation policy

- Public API deprecations receive compiler warnings and migration documentation for at least one supported minor release unless a critical defect requires faster removal.
- Serialized fields/types retain migration support through the documented compatibility window.
- Diagnostic codes are not reused for different meanings.
- Removed behavior is listed in changelog and upgrade guide.

### 21.4 GUID and asset compatibility

Public scripts, prefabs, templates, definitions, and samples preserve committed `.meta` files. Moves and renames retain GUIDs whenever identity is intended to survive. Setup-generated project assets are never replaced merely to adopt a new package template.

`EchoLaunchStatusView.prefab` and `EchoLaunchRoot.prefab` are stable public
package assets after FL-M4-05. Future structural revisions preserve their GUIDs.
Projects must not depend on internal child-file IDs or exact decorative values;
the supported contract is the prefab identity, required components, serialized
presenter wiring, and documented hierarchy roles.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview and authority boundaries.
- Supported installation routes.
- Five-minute quick start.
- Full setup/repair guide.
- Boot scene and direct-scene guide.
- Startup step authoring guide.
- Splash/status configuration guide.
- Standalone Test Lab guide.
- Public runtime API examples.
- Diagnostic-code and troubleshooting reference.
- Upgrade/migration guide.
- Optional integration index.
- Known limitations.
- License, credits, and third-party notices.

### 22.2 Required developer documentation

- Architecture and lifecycle.
- Duplicate-claim and static-reset rules.
- Step execution/failure/timeout/cancellation contract.
- Report schema and observability.
- Extension and test-injection points.
- Editor setup/repair/migration behavior.
- Testing strategy and release workflow.
- ADRs and bridge specifications.
- Current checkpoint/status record.
- Linked `Current Notes.md`.

### 22.3 Documentation truth rule

All code examples must compile against the documented release. Menu paths, screenshots, setup output, diagnostic codes, and Test Lab instructions must match the current Unity baseline and package build.

### 22.4 Living repository and Obsidian workflow

- Documentation lives in Git beside implementation.
- Obsidian opens those same Markdown files directly.
- `Current Notes.md` captures observations, proposals, questions, tests, bugs, risks, and handoff details.
- Every checkpoint reconciles notes into this specification, ADRs, tests/issues, guides, changelog, or status records.
- Resolved notes may be condensed after promotion; Git is the archive.
- Documentation changes are committed with or immediately adjacent to related code.

### 22.5 Repository scan and handoff order

1. Repository README/documentation index.
2. SFGSS-000.
3. This EchoLaunch specification.
4. Applicable ADRs/bridge specifications.
5. `Current Notes.md`.
6. Current checkpoint, tests, issue log, and changelog.
7. Relevant implementation and automated tests.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | IDs, policies, validation, report builder, ordering | Duplicate ID, timeout policy validation, immutable report | Yes |
| PlayMode unit/integration | Root claim, lifecycle, step execution, cancellation, scene handoff | Duplicate root, async progress, block/continue | Yes |
| Standalone Test Lab | Visible isolated launch loop | Success, warning, failure, direct scene, duplicate | Yes |
| Bridge Integration Lab | Optional package connection | Observatory or SceneFlow bridge | When bridge ships |
| Showcase | Combined application shell | Multi-package polished startup | No |
| Clean-project install | Packaging and dependency proof | Local, tarball, Git install | Yes |
| Existing-project migration | Replacement without regressions | Rescuers2D or Echo Systems Lab bootstrap adoption | Before adoption claim |

### 23.2 Required test categories

- Successful canonical Boot launch.
- Missing root/configuration/sequence/presenter/destination.
- Invalid values and unsupported schema.
- Empty sequence allowed/disallowed policy.
- Duplicate root before Play and introduced during load.
- Immediate, timed, warning, recoverable, blocking, throwing, timeout, and cancelled steps.
- Deterministic ordering.
- Direct-scene entry with and without existing root.
- Scene transition and destination activation.
- Repeated initialization and teardown.
- Domain reload enabled/disabled.
- Sample removal.
- Optional integration absent/present.
- Build validation.
- Asset immutability during play.
- Setup/repair repeatability.
- Performance and advertised limits.

### 23.3 Test case registry

| Test ID | Requirement | Setup | Action | Expected result | Automated? | Status |
|---|---|---|---|---|---:|---|
| ELAUNCH-T-001 | One authority | Scene with two roots | Enter Play | One root executes; duplicate zero side effects | Yes | Not run |
| ELAUNCH-T-002 | Static reset | Domain reload disabled | Enter/exit Play twice | Fresh claim each session | Yes | Not run |
| ELAUNCH-T-003 | Ordered steps | Three deterministic steps | Launch | Results order equals sequence order | Yes | Not run |
| ELAUNCH-T-004 | Warning continuation | Warning step then success | Launch | Both execute; report success-with-warning | Yes | Not run |
| ELAUNCH-T-005 | Blocking failure | Blocking step before destination | Launch | Later steps per policy stop; destination not loaded | Yes | Not run |
| ELAUNCH-T-006 | Timeout | Non-completing test step | Launch | Timeout result/code and policy applied | Yes | Not run |
| ELAUNCH-T-007 | Exception conversion | Throwing test step | Launch | Structured failure, no unhandled crash | Yes | Not run |
| ELAUNCH-T-008 | Asset safety | Sequence/config assets | Launch | Assets remain unchanged/not dirty | Yes | Not run |
| ELAUNCH-T-009 | Direct scene | Scene with helper, no root | Enter Play | Development root created once and mode recorded | Yes | Not run |
| ELAUNCH-T-010 | Existing authority direct scene | Existing root + helper | Enter Play | No duplicate created | Yes | Not run |
| ELAUNCH-T-011 | Invalid destination | Scene not build-loadable | Launch | Preflight blocks with ELAUNCH-DEST-001 | Yes | Not run |
| ELAUNCH-T-012 | Successful handoff | Valid destination | Launch | Destination activates; one completion event/report | Yes | Not run |
| ELAUNCH-T-013 | Setup repeatability | Clean temporary root | Apply setup 3 times | One config/root/Boot entry; second/third apply NoChanges | Yes | Not run |
| ELAUNCH-T-016 | Setup planning purity | Snapshot and request | Generate plan repeatedly | No writes; plans value-equivalent | Yes | Pass in FL-M5-01 |
| ELAUNCH-T-017 | Existing compatible assets | Matching targets | Plan/apply | Reuse/NoChange; never overwrite or dirty | Yes | Planning pass; apply not run |
| ELAUNCH-T-018 | Setup path conflict | Wrong type at target | Plan/apply | ELAUNCH-SETUP-002; no writes | Yes | Planning pass; apply not run |
| ELAUNCH-T-019 | Build Settings order safety | Existing unrelated order | Apply append/promotion | Append default; approval for promotion; unrelated order preserved | Yes | Planning pass; apply not run |
| ELAUNCH-T-020 | Stale plan safety | Preview then change evidence | Apply displayed plan | ELAUNCH-SETUP-008 before writes | Yes | Not run |
| ELAUNCH-T-021 | Apply rollback | Inject failure after partial creation | Apply | Active-attempt content removed and settings restored | Yes | Not run |
| ELAUNCH-T-022 | Prefab variant adoption | Package template + new root | Apply | Valid bound project variant; template not dirty | Yes | Not run |
| ELAUNCH-T-023 | Scene-state preservation | Open/active/dirty scenes | Create Boot | Existing scene state unchanged | Yes | Not run |
| ELAUNCH-T-024 | Explicit repair gate | Repairable current-schema drift | Refresh then press Apply | Create Apply rejects repair; Repair requires confirmation | Yes | Not run |
| ELAUNCH-T-025 | Configuration reference repair | Valid schema-4 config with wrong/null canonical refs | Repair | Only three approved references reconcile; other values and stable ID unchanged | Yes | Not run |
| ELAUNCH-T-026 | Destination repair | Valid schema-1 destination with stale path | Repair | Scene path reconciles; non-empty authored label preserved | Yes | Not run |
| ELAUNCH-T-027 | Prefab binding repair | Verified template variant with wrong/null config | Repair | Config binding reconciles; other overrides/presenter preserved; template not dirty | Yes | Not run |
| ELAUNCH-T-028 | Boot root-presence repair | Exact Boot scene with zero roots and unrelated objects | Repair | One project-prefab root added; unrelated scene/open/active/dirty state preserved | Yes | Not run |
| ELAUNCH-T-029 | Ambiguous/unsafe repair rejection | Multiple roots, wrong prefab lineage, wrong type, or unsupported schema | Plan/repair | ELAUNCH-SETUP-003/015; no backup or writes | Yes | Not run |
| ELAUNCH-T-030 | Repair backup and rollback | Existing repairable files + injected failure | Repair | Exact bytes/meta/settings restored; ELAUNCH-SETUP-016 | Yes | Not run |
| ELAUNCH-T-031 | Incomplete repair rollback | Inject restore failure | Repair | ELAUNCH-SETUP-017 and retained backup/manual paths | Yes | Not run |
| ELAUNCH-T-032 | Repair repeatability | Repairable partial foundation | Repair 3 times | First succeeds; second/third NoChanges; GUIDs/IDs stable; no duplicates | Yes | Not run |
| ELAUNCH-T-014 | Sample removal | Installed package + imported sample | Delete sample | Runtime/Editor compile | Manual/CI | Not run |
| ELAUNCH-T-015 | Clean tarball install | New project | Install `.tgz` | Zero compile errors; quick start succeeds | Manual/CI | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Package identity is approved; `SFGSS-PKG-ECHOLAUNCH-001` remains the accepted working document ID until SFGSS-008 formalizes the registry.
- [x] Ownership and non-ownership are approved.
- [x] MVP and deferred scope are separated.
- [x] Required dependencies are explicit.
- [x] Async execution primitive and custom-step API are approved.
- [x] Root lifetime policy is approved.
- [x] Default presenter technology/dependency is approved.
- [x] Public API and data model are approved.
- [x] Standalone Test Lab is approved.
- [x] Release-blocking questions are resolved.

### 24.2 Implementation gate

- [ ] Runtime compiles with declared dependencies only.
- [ ] Editor code is isolated.
- [ ] Duplicate claim occurs before side effects.
- [ ] Setup/repair is repeatable and non-destructive.
- [ ] Configuration assets remain immutable at runtime.
- [ ] Public API matches this specification or specification/ADR changed first.

### 24.3 Standalone gate

- [ ] Clean-project install succeeds.
- [ ] No other Sperk’s Forge runtime package is installed.
- [ ] Standalone Test Lab passes.
- [ ] Samples can be removed safely.
- [ ] Direct-scene behavior matches documentation.
- [ ] Final destination loads through standalone loader.

### 24.4 Quality gate

- [ ] Automated tests pass.
- [ ] Manual Test Lab checklist passes.
- [ ] No blocker or critical defect remains.
- [ ] Performance targets pass.
- [ ] Diagnostic codes are actionable and stable.
- [ ] Documentation matches build/API.
- [ ] `Current Notes.md` is reconciled.
- [ ] Durable decisions are promoted to specification/ADRs.
- [ ] License and notices are complete.

### 24.5 Distribution gate

- [ ] Manifest is valid.
- [ ] Supported Unity floor and exact dependencies are declared.
- [ ] Version and changelog are updated.
- [ ] Stable `.meta` files are included.
- [ ] Local, tarball, and Git installation routes are tested as claimed.
- [ ] Repository tag/release is prepared.
- [ ] Documentation/current status are committed and pushed.
- [ ] Central compatibility catalog is updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Echo Systems Lab | `ApplicationBootstrap`-style startup and scene/save coordination | Install First Light in isolation; reproduce only startup ownership/order/reporting; retain existing save/scene services behind project steps/adapters | Existing project launches, save/scene behavior unchanged, duplicate safety proven | Re-enable original bootstrap scene/prefab and remove First Light adapter |
| Rescuers2D | Project-specific persistent bootstrap/services | Add First Light Boot scene and bridge one initialization concern at a time without removing originals | Canonical and direct-scene tests pass; no duplicate persistent service; behavior parity documented | Restore original Boot entry and disable First Light root |
| Don’t Get Vince’d | Project startup flow | Use as second architecture-diverse adoption after standalone release | Launch flow works without Rescuers2D assumptions | Revert scene/build-settings commit |

### 25.2 Preserve-until-parity rule

Existing boot managers remain available until First Light passes in isolation and then reproduces the target project’s startup behavior through explicit project adapters. First Light must not absorb target-project service code merely to achieve parity.

### 25.3 Migration tooling

The MVP does not promise automatic conversion of arbitrary bootstrap scripts. Adoption support provides:

- Duplicate/bootstrap detector.
- Scene/build-settings inventory.
- Dry-run migration checklist.
- Creation of new Boot scene/configuration without deleting the old one.
- Side-by-side test mode.
- Validation report.
- Explicit rollback instructions.

Automated script rewriting is rejected unless a later migration specification proves it safe.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| R-001 | Scope inflates into application framework | High | High | Enforce one-sentence contract and MVP matrix; reject normal-runtime features | Any feature outside initial launch/handoff; Jesse |
| R-002 | Hidden cross-package dependency | Medium | High | Clean-project tests with only EchoLaunch; bridges separate | Compile/install failure after peer removal |
| R-003 | Duplicate root performs side effects | Medium | Critical | Claim in `Awake` before subscriptions/presentation/steps; dedicated tests | Any double event/load/log sequence |
| R-004 | ScriptableObject step/config state mutates | Medium | High | Runtime session objects; asset-dirty tests | Asset changes after Play Mode |
| R-005 | Async primitive limits supported Unity versions/platforms | High until decided | High | Approve Unity floor and async contract before code | M0 API decision |
| R-006 | uGUI dependency undermines minimal package | Medium | Medium | Approve dependency explicitly or choose alternate presenter strategy | M0 presentation decision |
| R-007 | Root survives too long and becomes service locator | Medium | High | Narrow public API, root lifetime policy, release startup resources | Requests to add arbitrary service access |
| R-008 | Destination loading duplicates EchoSceneFlow | Medium | Medium | Limit built-in loader to initial standalone handoff; use bridge later | Mid-game API requests |
| R-009 | Setup overwrites project scenes/build order | Medium | High | Dry-run, create-only defaults, preview, Undo/backup | Existing project adoption |
| R-010 | Sample becomes runtime requirement | Low | High | Sample-removal gate and no runtime references | Clean-project CI |
| R-011 | Report leaks project paths/details | Medium | Medium | Sanitized release report and development verbosity split | External bug-report use |
| R-012 | Direct-scene helper ships in release unintentionally | Medium | High | Build validator and disabled-by-default inclusion policy | Release preflight |
| R-013 | GUID breakage during package reorganization | Medium | High | Commit `.meta` files and retain GUIDs on moves | Refactor/migration |
| R-014 | Existing-project regression | Medium | High | Preserve-until-parity and reversible scene/build commits | First adoption |
| R-015 | Diagnostics too vague to fix launch | Medium | High | Stable codes, step IDs, preflight details, Test Lab failure cases | Tester cannot identify action |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| ELAUNCH-D-001 | EchoLaunch owns only initial startup and handoff, not normal runtime authorities | Approved by SFGSS-000 | Preserves package boundaries | Requires explicit bridges/project steps | No, already suite authority |
| ELAUNCH-D-002 | First valid root claims authority before any side effect | Approved by SFGSS-000 | Prevents duplicate bootstrap failures | Claim/reset behavior is release-critical | No, already suite authority |
| ELAUNCH-D-003 | MVP includes one standalone initial scene loader | Approved | Keeps package independently useful without EchoSceneFlow | Must remain limited to launch handoff | No |
| ELAUNCH-D-004 | MVP includes a default uGUI status/image presenter in an isolated presentation assembly | Approved | SFGSS-000 requires readable status without EchoUI; uGUI is the first suite presentation path | Adds one declared Unity dependency while preserving core separation | No |
| ELAUNCH-D-005 | Configuration and step definitions are project-owned immutable assets; active state is runtime-only | Approved | Prevents shared asset contamination | Requires explicit session/execution objects | No |
| ELAUNCH-D-006 | Custom startup operations use Unity `Awaitable<T>` with progress, cancellation, timeout, and structured result | Approved | Unity-native Unity 6 async behavior with deterministic diagnostics | Bridge APIs must use the same contract | No; package-specific decision |
| ELAUNCH-D-007 | Direct-scene initialization uses the same root/sequence/report rules and is disabled in release by default | Approved | Avoids a second bootstrap architecture | Requires build validation | No |
| ELAUNCH-D-008 | Root lifetime is configurable; the default is `UntilHandoff`, with `ApplicationSession` available explicitly | Approved | Prevents service-locator growth while allowing projects that need retained launch status | Consumers needing later access must opt in or copy the immutable report | No |
| ELAUNCH-D-009 | MVP failure actions are continue-with-warning or block; interactive retry is later | Approved | Keeps first release complete and small | Retry metadata/UI deferred | No |
| ELAUNCH-D-010 | No reflection-based peer discovery | Approved | Makes dependencies explicit and removal safe | Bridges must be installed deliberately | No |
| ELAUNCH-D-011 | Startup authoring uses immutable `StartupStepDefinition` ScriptableObjects that create single-use `IStartupStepExecutor` runtime instances | Approved | Combines inspector-friendly assets with strict definition/runtime-state separation | Custom steps require a definition and executor pair | No |
| ELAUNCH-D-012 | Minimum public Unity floor is 6000.0; 6000.3.8f1 is the primary development baseline | Approved | Avoids false precision while retaining an honest tested baseline | Additional Unity 6 versions require validation before being listed as tested | No; also recorded suite-wide |
| ELAUNCH-D-013 | Setup Repair is a separate explicitly approved transaction limited to provable current-schema canonical drift, with pre-write byte/meta backup and rollback | Approved | Prevents create-only Apply from silently becoming destructive while making damaged generated foundations recoverable | Ambiguous ownership, structural edits, and schema changes remain manual or migration work | Yes; EchoLaunch-ADR-006 |
| ELAUNCH-D-014 | Project health validation is an explicit read-only Editor transaction with immutable schema-1 findings/report, stable codes, scene-safe inspection, and deterministic fingerprints/text | Approved | Keeps diagnosis trustworthy and separate from mutation while preparing release-safety checks for Direct Scene | No auto-fix, build hook, runtime overlay, or direct-helper implementation in FL-M5-04 | Yes; EchoLaunch-ADR-007 |
| ELAUNCH-D-015 | Direct Scene uses a project-owned immutable direct configuration, Start-time authority reuse, a pre-authored direct root prefab, active-destination no-reload handoff, Editor-only default policy, explicit Development-Build opt-in, and an unconditional non-development release gate | Approved | Preserves one startup architecture while preventing development bootstrap behavior from running in release | No hidden discovery, runtime asset rewrite, automatic installation, or release enablement | Yes; EchoLaunch-ADR-008 |
| ELAUNCH-D-016 | Launch simulation is an explicit Editor-only transaction that builds transient in-memory scenario data, executes the real startup-sequence runner and policy contracts against deterministic logical time, and emits a separate immutable schema-1 simulation report | Approved | Proves step behavior without mutating authored content or falsely claiming a full root/destination launch | No persistent scenario assets, runtime simulator code, Play Mode automation, scene mutation, build hooks, or Laboratory implementation | Yes; EchoLaunch-ADR-009 |

### 27.2 Release-blocking questions

None. The implementation-shaping questions from specification 0.1.0 were resolved at approval:

| Former question | Approved resolution |
|---|---|
| Async primitive | Unity `Awaitable<T>` with `CancellationToken` and package-owned progress reporting |
| MVP presentation dependency | Declared uGUI dependency with the default presenter isolated from the launch core |
| Default post-handoff lifetime | Configurable, defaulting to `UntilHandoff` |
| Startup-step authoring model | Immutable ScriptableObject definition plus single-use runtime executor |
| Minimum Unity version | Unity 6000.0 public floor; Unity 6000.3.8f1 primary development baseline |

### 27.3 Non-blocking later questions

- Final package-spec document ID convention under SFGSS-008.
- Final license model and public contribution policy before distribution.
- Exact report export format and support-bundle privacy policy.
- Whether legal-screen entries need a distinct type or remain splash-policy flags.
- Whether automatic retry belongs in core after MVP.
- Whether destination definitions are standalone assets or embedded configuration data.
- Which bridge is the first integration proof after standalone release.

---

## 28. Milestones and Checkpoint Path

### 28.1 Approved milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 — Specification | Approved First Light contract | All sections, resolved blockers, MVP/API/Test Lab/release gates | Jesse approval record |
| M1 — Skeleton | Installable package anatomy | Manifest, assemblies, docs shell, namespaces, no runtime behavior | Clean compile in embedded/local/tarball routes |
| M2 — Authority and report core | One protected root and deterministic report lifecycle | Claim/reset, preflight shell, session/report types, duplicate tests | EditMode/PlayMode tests |
| M3 — Startup sequence | Ordered immediate/async steps and failure policy | Step API, runner, progress, timeout/cancellation, result handling | Automated tests and headless lab proof |
| M4 — Presentation and handoff | Complete standalone MVP user loop | Image splashes, status view, destination load, handoff | Standalone Test Lab checklist |
| M5 — Tooling and direct scene | Safe setup/repair/validation and development entry | Setup, validator, simulator, direct initializer | Repeatability and direct-scene tests |
| M6 — First adoption/integration | One real-project adoption or optional bridge | Project adapter or first bridge without core dependency | Parity/integration report |
| M7 — Release | Distribution-ready beta/stable candidate | Docs, license, tarball, clean install, catalog | Release checklist and external clean-project test |

### 28.2 Checkpoint rule

Every milestone is divided into small Checkpoint Build Plans. Each checkpoint reconciles Current Notes, promotes durable decisions, updates tests/issues/docs/changelog, verifies documentation against committed implementation, and commits/pushes documentation with or immediately adjacent to code.

### 28.3 First implementation checkpoint

**Checkpoint FL-M1-01 — Package Skeleton**

Authorized outcome:

- Create the installable UPM package directory and manifest.
- Create compile-safe Runtime, Editor, and test assembly definitions.
- Create the package README, changelog, development license notice, third-party notice, and documentation shell.
- Verify the package is recognized by Unity and compiles without runtime behavior.

Explicit stop point:

- Do not create `EchoLaunchRoot`, startup steps, ScriptableObjects, prefabs, scenes, presenters, setup tools, or launch behavior during FL-M1-01.

The complete plan lives at `Checkpoint Build Plans/First_Light_M1_Package_Skeleton_Checkpoint_Build_Plan.md` and is governed by SFGSS-005.

---

## 29. New-Conversation Handoff

```text
We are continuing development of The Sperk’s Forge — EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the authority for suite-wide boundaries and architecture.
Treat the approved First Light — Startup and Launch (EchoLaunch) Package
Specification as the authority for this package’s behavior, public API, data model,
tooling, Standalone Test Lab, and release gates. Follow SFGSS-005 for all implementation checkpoints and use the active approved
Checkpoint Build Plan as the only implementation authorization.

Current package: EchoLaunch / First Light
Current specification version: <VERSION>
Current milestone/checkpoint: <CHECKPOINT>
Current Unity version: <VERSION>
Current project/repository: <PROJECT>
Current implementation status: <STATUS>
Known blockers: <BLOCKERS>
Current Notes reviewed through: <DATE/COMMIT>

Before writing code:
1. Summarize First Light’s startup-only ownership and independence constraints.
2. Identify any conflict or unresolved decision that materially affects the checkpoint.
3. Keep optional integrations behind documented bridges or project adapters.
4. Preserve existing project boot systems until replacement parity is proven.
5. Continue using the Checkpoint Build Plan format.
6. Reconcile Current Notes and documentation at checkpoint closeout.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | `0.1.0` embedded package implementation |
| Completed checkpoint | FL-M5-06 — Launch Simulator and Deterministic Failure Injection |
| Active authorized checkpoint | None |
| FL-M5-06 authority commit | `a159349` |
| FL-M5-05 authority commit | `d538b5a` |
| FL-M5-04 authority commit | `c2397c9` |
| FL-M5-03 authority commit | `6615c8f` |
| Last implementation commit | `956c381` |
| Last documentation commit | `b6df92d` before this closeout |
| Runtime tests passed | 503 Runtime Play Mode tests |
| EditMode tests passed | 290 total: 209 setup/apply/repair, 25 Validator, 5 Direct Scene Validator, 24 Launch Simulator, and 27 prefab asset tests |
| Total automated tests | 793 passed, 0 failed, 0 ignored |
| Compilation | 0 errors and 0 compiler warnings |
| FL-M5-03 evidence | Separate explicit Repair; proof-backed current-schema eligibility; fresh-plan gate; exact asset + `.meta` backup; narrow repair; first Repair succeeded; second and third Repair returned NoChanges; stable IDs/GUIDs and unrelated content preserved |
| FL-M5-04 evidence | Dedicated explicit read-only Validator; immutable schema-1 report; stable validation codes; scene-safe inspection; deterministic healthy report; deliberate blocked report with `002`, path-specific `003`, and `008`; exact restored healthy fingerprints |
| FL-M5-05 evidence | Project-owned direct configuration; Start-time reuse/create; exactly-one authority; active-destination no reload; unconditional release-player prohibition; truthful direct mode; activated `VAL-009`; manual creation/reuse/convergence; Development-Build Warning; exact restored healthy fingerprints |
| FL-M5-06 evidence | Explicit Editor-only Simulator; real runner and policy execution; transient authored shape; eight accepted presets; deterministic logical progress; clean expected-failure Console behavior; single-active-run and cancellation; human-click elapsed filtered from copied cancellation evidence; three identical accepted cancellation report fingerprints |
| Default project root | `Assets/EchoDevGames/FirstLight` |
| Evidence gaps | Historical schema migration, receipts, uninstall/reset, crash-persistent recovery, automatic Direct Scene installation, build hooks, Laboratory, player builds, clean install, external adoption, and performance evidence remain not run |
| Next action | Commit and push the FL-M5-06 documentation closeout, then select and authorize the next bounded First Light checkpoint |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Package identity and plain responsibility are clear.
- [x] Ownership and non-ownership align with SFGSS-000.
- [x] Independence proof is credible.
- [x] MVP is small enough to complete and large enough to be useful.
- [x] Public API, data, lifecycle, and failure behavior are specified.
- [x] Async primitive and step authoring model are approved.
- [x] Minimal presenter dependency/path is approved.
- [x] Root lifetime default is approved.
- [x] Setup and direct-scene workflows are understandable.
- [x] Standalone Test Lab is fully defined.
- [x] Diagnostics exist without the Observatory.
- [x] Optional integrations are separated.
- [x] Test and release gates are measurable.
- [x] No Isekai Studios identity or ownership has been introduced.
- [x] `Current Notes.md` has been reconciled after approval.
- [x] Jesse has approved the specification as the package authority; implementation proceeds only through approved SFGSS-005 Checkpoint Build Plans.

### 30.2 Approval record

**Decision:** Approved
**Approved by:** Jesse “Echo” Adams
**Date:** August 3, 2026
**Conditions or notes:** The design is approved. FL-M4-04 may implement configuration schema 4 and the accepted sequential optional-splash-before-startup root contract only through its approved Checkpoint Build Plan. Report schema 2 remains unchanged. Runtime migration, concurrent splash/step execution, and silent asset rewriting remain prohibited.

---

## Template Completion Review

A new collaborator can determine from this approved specification:

1. EchoLaunch owns only initial launch authority, sequencing, launch-only presentation, reporting, direct-scene development rules, and handoff.
2. It explicitly refuses audio, saves, menus, normal scene flow, gameplay rules, and arbitrary service location.
3. The MVP is one protected root, ordered steps, image splashes, plain status, report, direct-scene support, and one destination.
4. It works without any other Sperk’s Forge runtime package.
5. Definitions/configuration remain immutable assets; active execution/report-building state is runtime-only.
6. The approved public types, lifecycle, events, failure model, and test seams are documented.
7. Missing setup, duplicate roots, invalid steps, timeouts, exceptions, and destination failures have explicit outcomes and codes.
8. The isolated Test Lab and acceptance registry are defined.
9. Optional packages connect only through bridges or project adapters.
10. Release evidence is defined across specification, implementation, standalone, quality, distribution, adoption, and documentation gates.

The document is **Approved** as the Level 2 authority for First Light. FL-M5-01 implemented the read-only snapshot and dry-run planner. FL-M5-02 implemented and validated the fresh-plan-gated create-only apply service, deterministic foundation creation, approved Build Settings mutation, compensating rollback, immutable results, and repeat-safe no-op reruns defined by EchoLaunch-ADR-005. FL-M5-03 implemented and validated the separate explicit current-schema repair, ownership/shape proof, byte-preserving backup, rollback, immutable result, and repeatability boundary defined by EchoLaunch-ADR-006 and its SFGSS-005 plan.
FL-M5-04 implemented and validated the explicit read-only Validator, immutable schema-1 project-health findings/report, stable validation rules, scene-safe enabled-build-scene inspection, deterministic fingerprints, and copyable project-relative text defined by EchoLaunch-ADR-007 and its approved plan. FL-M5-05 implemented and validated the project-owned Direct Scene Development Initializer, Start-time authority reuse, active-destination no-reload handoff, explicit Editor/development environment policy, `DirectSceneDevelopment` report mode, unconditional non-development release-player creation prohibition, and activated `ELAUNCH-VAL-009` checks defined by EchoLaunch-ADR-008 and its approved plan. FL-M5-06 implemented and validated the explicit Editor-only Launch Simulator, transient deterministic scenario planning, real startup-sequence runner/policy execution, immutable schema-1 simulation reporting, copyable evidence, single-active-run protection, cancellation, cancellation-evidence determinism correction, and no-production-dependency boundary defined by EchoLaunch-ADR-009 and its approved plan. Schema migration, receipts, uninstall/reset, crash-persistent recovery, automatic helper installation, build hooks, Laboratory, player-build evidence, clean external installation, and performance claims remain unauthorized.


---


## SUITE-DOC-30 Consistency Addendum

**Review status:** Passed
**Review date:** August 4, 2026
**Current governing authorities:** SFGSS-000 v0.20.0; SFGSS-001 v1.2.0; SFGSS-002 v1.1.0; SFGSS-003 v1.1.0; SFGSS-004 v1.2.0; SFGSS-005 v1.4.0; SFGSS-006 through SFGSS-010; SFGSS-ADR-001 through SFGSS-ADR-004; and the approved Foundation, Expansion, and Advanced integration matrices.

The original parent-authority header remains approval provenance. This addendum records the standards that govern the specification after the full consistency review.

- The formal public title, technical identifier, package ID, namespace family, document ID, diagnostic/test prefix, setup facade, and planned repository were checked against SFGSS-008 and SFGSS-009.
- All implementation, compatibility, platform, performance, migration, Laboratory, provider, and release evidence remains `Not run` unless a retained execution record says otherwise.
- Package-qualified test and Laboratory IDs are authoritative. Pre-code range tables are planning shorthand only; implementation registries must expand them into individual definitions with separate automation class, execution status, evidence reference, and issue reference fields.
- A platform cell written as `Yes` in an older pre-code table means **planned design support**, not `Tested` or `Supported`, until SFGSS-004 evidence exists.
- Primary public Runtime assemblies may remain `autoReferenced: true`; Editor, test, sample, internal support, bridge, and provider assemblies default to `false` under SFGSS-002 unless this specification explicitly records a justified exception.
- Current Notes captures future discoveries, but durable changes return to this specification or an ADR before implementation advances.

**Package-specific repairs:**

- Separated the default uGUI presenter from the neutral Runtime assembly.
- Set the Editor assembly to `autoReferenced: false`.
- Canonicalized immutable `StartupStepDefinition` versus runtime executor terminology.

## Graph Navigation

#sfgss/package #sfgss/wave/foundation #sfgss/status/approved

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Echo_Game_Systems_Suite_Bible|SFGSS-000 Suite Bible]]
- [[SFGSS-001_Package_Specification_Template|SFGSS-001 Package Template]]
- [[Package_Learning_Review_Catalog|Package Learning Review Catalog]]
