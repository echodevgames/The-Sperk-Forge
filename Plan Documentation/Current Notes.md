# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page  
**Authority:** Working context only; SFGSS-000, approved package specifications, and accepted ADRs remain authoritative  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Last reconciled:** August 3, 2026  
**Current focus:** Foundation Wave package specifications  
**Current checkpoint:** FW-DOC-05 — Draft The Pulse (`EchoGameState`) package specification

> Capture quickly here. Promote deliberately at checkpoint closeout.

---

## How to Use This Page

Use this page for information discovered while designing, implementing, testing, or reviewing the suite:

- `[NOTE]` — useful observation or context.
- `[QUESTION]` — unresolved question requiring research or approval.
- `[PROPOSAL]` — suggested change that is not yet authoritative.
- `[DECISION]` — approved decision awaiting or confirming documentation promotion.
- `[TEST]` — test result, reproduction, or validation evidence.
- `[BUG]` — defect or regression awaiting issue-log placement.
- `[RISK]` — dependency, compatibility, schedule, or architecture concern.
- `[HANDOFF]` — context the next work session must see.

Keep entries dated. Link to the affected specification, ADR, checkpoint, test, issue, guide, or source file whenever possible.

Do not leave durable decisions only on this page. At checkpoint closeout, promote each material entry into the document that owns it and record the destination below.

---

## Current Focus

### Goal

Draft, reconcile, and approve complete SFGSS-001 package specifications for all ten Foundation Wave packages before any Foundation Wave runtime implementation begins.

### Active source documents

- `Echo_Game_Systems_Suite_Bible.md` — SFGSS-000 v0.6.0.
- `SFGSS-001_Package_Specification_Template.md` — v1.1.0.
- `Package Specifications/SFGSS-First-Light-EchoLaunch-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Observatory-EchoDiagnostics-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Accord-EchoSettings-Package-Specification.md` — v1.0.0 Approved.
- `Package Specifications/SFGSS-The-Passage-EchoSceneFlow-Package-Specification.md` — v1.0.0 Approved.
- `Foundation_Wave_Specification_Roadmap.md` — active Level 4 planning record.

### Next action

Draft the complete **The Pulse — Runtime State (`EchoGameState`) Package Specification** using SFGSS-001. Define high-level runtime modes, validated state transitions, temporary override/stack behavior, nested pause reasons, time-scale policy, cursor and input-context coordination requests, state history, direct-scene behavior, and optional bridges without allowing EchoGameState to absorb UI presentation, input binding ownership, audio playback, scene-transition execution, gameplay character states, or project-specific victory/defeat rules.

---

## Open Questions

- None blocking the start of the EchoGameState specification.
- Licensing remains a later suite-wide release decision and does not block the documentation pass.

---

## Active Notes

### August 3, 2026 — Living repository documentation

- `[DECISION]` Suite and package documentation will live in the Git repository beside development work.
- `[DECISION]` The repository documentation folder will be opened directly in Obsidian rather than copied into a separate vault.
- `[DECISION]` Every active repository will expose a linked `Current Notes.md` page for ongoing observations, proposals, tests, questions, and handoff context.
- `[DECISION]` At meaningful checkpoints, durable notes will be promoted into the bible, package specification, ADR, issue/test record, guide, changelog, or checkpoint status that owns them.
- `[DECISION]` Major documentation changes will be committed with the related code when practical, or in an immediately adjacent documentation commit.

**Promoted to:** SFGSS-000 v0.5.0 decision 31 and SFGSS-001 v1.1.0 documentation requirements.

### August 3, 2026 — Foundation Specification Pass

- `[DECISION]` Complete and approve all ten Foundation Wave package specifications before beginning Foundation Wave runtime implementation.
- `[DECISION]` Run a cross-package consistency review after the tenth specification and before opening any M1 package skeleton checkpoint.
- `[DECISION]` First Light specification v1.0.0 is approved as the Level 2 package authority, but its implementation remains deferred by the suite documentation gate.
- `[DECISION]` First Light uses Unity `Awaitable<T>` for startup execution.
- `[DECISION]` First Light startup authoring uses immutable `StartupStepDefinition` ScriptableObjects that create separate single-use runtime executors.
- `[DECISION]` First Light ships a default uGUI status/image presenter isolated from its launch core.
- `[DECISION]` First Light root lifetime is configurable and defaults to `UntilHandoff`.
- `[DECISION]` The initial public Foundation package floor is Unity 6000.0, with Unity 6000.3.8f1 as the primary development baseline.

**Promoted to:** SFGSS-000 v0.6.0 decisions 32–33, First Light specification v1.0.0, and the Foundation Wave Specification Roadmap.

### August 3, 2026 — The Observatory specification

- `[DECISION]` The Observatory (`EchoDiagnostics`) specification v1.0.0 is approved as the Level 2 authority for diagnostics and validation; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoDiagnostics observes and reports package/runtime health but never becomes the source of truth for the behavior it observes and never silently repairs production state.
- `[DECISION]` Runtime integrations use explicit provider registration with stable provider IDs and disposable registration handles. Reflection-based discovery is not required for normal operation.
- `[DECISION]` Providers capture bounded, synchronous snapshots. Systems with asynchronous work cache their latest safe status rather than making the diagnostic sampler await or block gameplay.
- `[DECISION]` Diagnostic snapshots use normalized availability, health, severity, and privacy classifications so unsupported information is reported as unavailable rather than as a misleading zero or success.
- `[DECISION]` The runtime root is duplicate-safe, persists for the application session when enabled, and owns its sampler, histories, registry, event buffer, and overlay services. Editor validation can run without a runtime root.
- `[DECISION]` Runtime metric/event histories use bounded buffers and configurable update rates; diagnostic failure must degrade diagnostics rather than gameplay.
- `[DECISION]` The initial overlay uses uGUI and TextMeshPro but remains an isolated presenter over neutral diagnostic state. It does not own general UI navigation, the EventSystem, input contexts, game pause, or gameplay time scale.
- `[DECISION]` Local support-snapshot export is explicit, versioned, privacy-filtered, and never transmitted automatically.
- `[DECISION]` Editor validation supports manual, pre-Play, and pre-build execution. Repairs remain explicit and non-destructive; validation itself does not mutate production configuration.
- `[DECISION]` First Light remains independent. A separate First Light–Observatory bridge maps concrete launch status and reports into the Observatory’s neutral launch model.
- `[DECISION]` Package inventory is an Editor capability in the MVP. A Player-build package manifest is deferred until a safe build-time generation design is approved.
- `[DECISION]` The Observatory does not replace Unity’s Console or Profiler, does not promise hardware sensor support, and does not globally intercept all logs in its MVP.

**Promoted to:** The Observatory (`EchoDiagnostics`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the already-approved diagnostics authority without changing suite ownership boundaries.


### August 3, 2026 — The Accord specification

- `[DECISION]` The Accord (`EchoSettings`) specification v1.0.0 is approved as the Level 2 authority for global preferences; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoSettings owns global preference truth, defaults, committed/effective values, drafts, validation, application coordination, versioned persistence, migration, and safe display confirmation. It does not own production settings UI, audio playback, input execution, localization content, save-slot progress, pause, or gameplay rules.
- `[DECISION]` The runtime model separates project defaults, committed settings, effective settings, editable drafts, and preserved unknown section records.
- `[DECISION]` The package uses explicit stable-ID typed section registration with independent document and section schema versions. Reflection-based settings discovery is not approved.
- `[DECISION]` Unknown optional-package section payloads are preserved when their definition or bridge is absent so clean package removal does not erase data.
- `[DECISION]` Edit sessions record the committed revision they started from. A stale draft returns a conflict rather than overwriting a newer commit silently.
- `[DECISION]` Settings application is transactional. Required appliers run provisionally in deterministic order and previously applied sections revert in reverse order when a required step fails.
- `[DECISION]` Risky display changes remain provisional until confirmed. Cancel, unscaled timeout, shutdown, application failure, or persistence failure restores the previous effective platform state.
- `[DECISION]` The default backend is a versioned structured JSON document stored beneath `Application.persistentDataPath`; `PlayerPrefs` is not the default backend.
- `[DECISION]` Corrupt, unsupported-old, and newer files are preserved for recovery. Recovery/default use does not silently overwrite evidence or a newer schema.
- `[DECISION]` The MVP built-in sections are Audio, Display, and basic Accessibility. EchoSettings stores audio/accessibility preference values; Jukebot, feedback, UI, input, and localization behavior remains in optional bridges or project adapters.
- `[DECISION]` The built-in display adapter is replaceable and capability-aware. Unsupported platform fields report unavailable rather than false success.
- `[DECISION]` Optional consumers may register appliers after settings initialization and receive the current effective values, avoiding circular startup requirements.
- `[DECISION]` The core is nonvisual. A sample or EchoUI presenter owns controls, silent binding, prompts, navigation, and display-confirmation presentation.
- `[DECISION]` Named profiles, import/export, monitor selection, HDR/dynamic-resolution options, cloud synchronization, and secure storage remain deferred or outside the MVP.
- `[DECISION]` Public asynchronous operations use fresh Unity `Awaitable<T>` instances, consistent with the Foundation Unity 6 baseline.

**Promoted to:** The Accord (`EchoSettings`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the existing global-preference authority and preserve the approved cross-package ownership matrix.

### August 3, 2026 — The Passage specification

- `[DECISION]` The Passage (`EchoSceneFlow`) specification v1.0.0 is approved as the Level 2 authority for normal scene travel after First Light handoff; implementation remains deferred by the Foundation documentation gate.
- `[DECISION]` EchoSceneFlow owns destination validation, request admission, one serialized transition pipeline, progress, activation, route helpers, recovery results, and scene-flow diagnostics. It does not own startup orchestration, game-state rules, production UI, save policy, audio playback, gameplay completion, multiplayer authority, or scene content.
- `[DECISION]` Runtime destinations use project-owned `SceneDefinition` and `SceneRouteDefinition` assets with stable IDs. Scene asset paths are backend locators maintained by Editor tooling, not durable identity or a public raw-string API.
- `[DECISION]` One duplicate-safe application-session `EchoSceneFlowRoot` owns the service, backend, queue, runner, participants, presenter registration, status, and bounded history. Duplicate rejection occurs before subscriptions or scene-operation side effects.
- `[DECISION]` The MVP backend uses Unity `SceneManager.LoadSceneAsync` for asynchronous single-scene loading behind an `ISceneLoadBackend` seam. Additive loading, owned unload, persistent scene sets, Addressables, and multiplayer providers remain deferred.
- `[DECISION]` Public asynchronous operations use fresh Unity `Awaitable<T>` instances and execute Unity scene APIs on the main thread.
- `[DECISION]` Only one scene operation may be active. The default admission policy is `RejectNew`; optional FIFO queuing is bounded, pending requests may be replaced by policy, and the active load is never replaced.
- `[DECISION]` Equivalent active or queued requests coalesce to one operation. Explicit reload remains a distinct operation.
- `[DECISION]` Cancellation is cooperative while queued or before the backend begins loading. After Unity loading starts, cancellation is reported as unsupported in the current phase and the operation continues to a safe terminal state or recovery.
- `[DECISION]` Immediate scene activation is the default. Optional delayed activation is short, hard-bounded, and never permitted to stall Unity's async operation queue indefinitely.
- `[DECISION]` Transition presenters and lifecycle participants register explicitly through disposable handles. No presenter is a valid core path; reflection discovery is not required.
- `[DECISION]` The runtime core is nonvisual and does not depend on uGUI or TextMeshPro. The Standalone Test Lab may use a sample-only presenter, and EchoUI may provide a separate production presenter bridge.
- `[DECISION]` Recovery may attempt one configured fallback only, with validation and runtime loop protection.
- `[DECISION]` Reload, Main Menu, and Hub helpers resolve project-configured route assets rather than hidden scene names.
- `[DECISION]` The direct-scene initializer is development-only by default and creates the minimum root only when an authority is absent.

**Promoted to:** The Passage (`EchoSceneFlow`) Package Specification v1.0.0. No SFGSS-000 revision was required because these decisions refine the existing normal-scene-travel authority and preserve the approved cross-package ownership matrix.

---

## Promotion Queue

| Date | Entry | Destination | Status |
|---|---|---|---|
| 2026-08-03 | Repository/Obsidian living-documentation workflow | SFGSS-000 and SFGSS-001 | Promoted |
| 2026-08-03 | Foundation Specification Pass before implementation | SFGSS-000 v0.6.0 and roadmap | Promoted |
| 2026-08-03 | First Light implementation-shaping choices | First Light specification v1.0.0 | Promoted |
| 2026-08-03 | Unity 6 package floor | SFGSS-000 v0.6.0 | Promoted |
| 2026-08-03 | Observatory authority, provider, overlay, validation, privacy, and bridge decisions | EchoDiagnostics specification v1.0.0 | Promoted |
| 2026-08-03 | Accord authority, section, transaction, display-safety, persistence, migration, and bridge decisions | EchoSettings specification v1.0.0 | Promoted |
| 2026-08-03 | Passage authority, stable scene/route data, transition pipeline, admission, cancellation, activation, recovery, and bridge decisions | EchoSceneFlow specification v1.0.0 | Promoted |

---

## Latest Validation Snapshot

| Area | Result | Evidence/notes |
|---|---|---|
| Suite bible | Approved and unchanged this checkpoint | v0.6.0; Passage decisions fit existing normal-scene-travel ownership |
| Package specification template | Approved and unchanged | v1.1.0 |
| First Light specification | Approved | v1.0.0; no release-blocking design questions |
| Observatory specification | Approved | v1.0.0; no release-blocking design questions |
| Accord specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; no release-blocking design questions |
| Passage specification | Approved | v1.0.0; all 30 SFGSS-001 sections completed; no release-blocking design questions |
| Foundation documentation gate | Active | 4 of 10 package specifications approved |
| Implementation | Not started by design | No package code begins before FW-DOC-12 passes |

---

## Checkpoint Closeout Checklist

- [x] Review every note added during the checkpoint.
- [x] Separate confirmed facts from proposals and unresolved questions.
- [x] Promote architecture changes into the bible/specification and an ADR when needed.
- [x] Move bugs and test evidence into their permanent records. No implementation defects exist yet.
- [x] Update guides and changelog for user-visible changes. Not applicable before package implementation.
- [x] Update the current checkpoint, blockers, and next action.
- [x] Confirm documentation matches committed implementation and tests. Implementation has not started.
- [ ] Commit and push the documentation update.
- [x] Condense or remove resolved notes after promotion.

---

## Handoff Snapshot

**Current program:** Foundation Specification Pass  
**Completed package specifications:** First Light (`EchoLaunch`) v1.0.0 Approved; The Observatory (`EchoDiagnostics`) v1.0.0 Approved; The Accord (`EchoSettings`) v1.0.0 Approved; The Passage (`EchoSceneFlow`) v1.0.0 Approved  
**Current package:** The Pulse (`EchoGameState`)  
**Current stage:** Specification not yet drafted  
**Last completed documentation change:** Passage normal-scene-travel authority, stable scene/route data, serialized transition lifecycle, bounded admission, honest cancellation, activation safety, recovery, Test Lab, and optional bridge boundaries approved  
**Known blockers:** None  
**Next checkpoint:** FW-DOC-05 — Draft and review the complete EchoGameState package specification
