# The Sperk’s Forge — Package Specification Template

**Document ID:** SFGSS-001

**Version:** 1.4.0

**Status:** Approved documentation standard

**Owner:** Jesse “Echo” Adams / EchoDevGames

**Project boundary:** Independent solo project; not an Isekai Studios product

**Parent authority:** SFGSS-000 v0.25.0 — The Sperk’s Forge Game Systems Suite Bible

**Current development baseline:** Unity 6000.3.8f1

**Last updated:** August 9, 2026

> “The Sperk guides our design journey. His almighty singularity lights the way.”

> **v1.3.0 Reference Showcase reconciliation:** Every package specification now defines an in-house production-style Package Reference Showcase, or a justified non-scene equivalent, in addition to its Standalone Test Lab. The Reference Showcase is project-owned consumer proof and never replaces isolated Laboratory evidence.

> **v1.4.0 Distribution Kit reconciliation:** Every independently distributed package specification must now define its versioned repository Distribution Kit, including the exact artifact, complete user handout, manifest, integrity/build records, and honest qualification state. Kit creation prepares evidence; it never converts an untested installation route into a supported claim.

---

## 1. Purpose of This Template

This document is the required starting structure for every individual package specification in **The Sperk’s Forge — EchoDevGames Game Systems Suite**.

An individual package specification is the Level 2 authority defined by SFGSS-000. It translates the suite-wide architecture into a complete, package-specific contract before implementation begins. It must be detailed enough that Jesse, ChatGPT, another programmer, or a future maintainer can understand what the package owns, what it refuses to own, how it works by itself, how it integrates optionally, and how its release will be proven.

This template is not a request to fill every heading with speculative features. It is a guardrail against hidden assumptions. When a section does not apply, write **Not applicable** and explain why. Do not silently remove required headings.

### 1.1 How to use this template

1. Copy this file for the package being specified.
2. Replace every angle-bracket placeholder.
3. Keep the numbered section structure unless an approved Architecture Decision Record changes it.
4. Label every proposed capability with a lifecycle status.
5. Resolve release-blocking questions before implementation.
6. Separate the first viable release from later expansions.
7. Review the completed specification against SFGSS-000.
8. Mark the specification **Approved** only after Jesse accepts its boundaries, MVP, and release gates.
9. Build the package through checkpoint plans derived from the approved specification.
10. Open the repository documentation folder directly as an Obsidian vault or folder rather than maintaining a second documentation copy.
11. Capture active discoveries in the repository's linked `Current Notes.md` page.
12. At each meaningful checkpoint, reconcile current notes into the specification, ADRs, issue/test records, guides, changelog, or checkpoint status that owns the durable information.
13. Update the specification and record an ADR when implementation reveals a genuine architectural change.
14. Define the package's versioned Distribution Kit and complete user handout before clean-project/release qualification.

### 1.2 Lifecycle labels

Use these labels consistently:

- **Proposed** — under evaluation.
- **Approved** — accepted as intended design.
- **In Development** — currently being implemented.
- **Implemented** — present and validated.
- **Deferred** — valid but intentionally postponed.
- **Experimental** — available without a compatibility guarantee.
- **Deprecated** — supported temporarily while being replaced.
- **Removed** — no longer part of the package.

### 1.3 Requirement language

- **Must** indicates a release requirement.
- **Must not** indicates a prohibited design or behavior.
- **Should** indicates the expected choice unless the specification records a reason to differ.
- **May** indicates an optional capability.

---

# Package Specification

## Document Control

| Field | Value |
|---|---|
| Document ID | `<SFGSS-PACKAGE-ID>` |
| Specification version | `<VERSION>` |
| Status | `<PROPOSED / APPROVED / IN DEVELOPMENT / IMPLEMENTED>` |
| Technical package name | `<EchoPackage>` |
| Public title | `<VERSE TITLE — PLAIN RESPONSIBILITY>` |
| Package ID | `<com.echodevgames.package-name>` |
| Runtime namespace | `<EchoDevGames.PackageName>` |
| Owner | Jesse “Echo” Adams / EchoDevGames |
| Repository | `<REPOSITORY URL OR PLANNED NAME>` |
| Current Notes | `<REPOSITORY-RELATIVE PATH TO Current Notes.md>` |
| Unity baseline | `<SUPPORTED UNITY VERSION>` |
| Parent authority | SFGSS-000 and SFGSS-001 |
| Last updated | `<DATE>` |

### Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| `<0.1.0>` | `<DATE>` | Proposed | Initial package specification | `<NAME>` |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** `<TITLE>`

**Technical identifier:** `<IDENTIFIER>`

**Flavor line:** `<OPTIONAL SHORT HACKULOS/SPERK-FLAVORED LINE>`

**Plain-language subtitle:** `<CLEAR TECHNICAL RESPONSIBILITY>`

**One-sentence ownership contract:**

> `<PACKAGE>` owns `<EXACT AUTHORITY>` and does not own `<MOST IMPORTANT EXCLUDED RESPONSIBILITIES>`.

### 1.1 Elevator summary

`<Explain what the package does, who it helps, and why it exists in two or three short paragraphs.>`

### 1.2 Why this belongs in The Sperk’s Forge

`<Identify the repeated problem seen across Rescuers2D, Don’t Get Vince’d, Echo Systems Lab, DeverQuest, Hackulos, or future projects. Explain why a reusable package is justified.>`

### 1.3 Verse identity boundary

Document where flavor may appear and confirm that runtime use does not require Hackulos lore.

| Surface | Flavor allowed? | Rule |
|---|---:|---|
| Public title | Yes | Must include a plain technical subtitle. |
| Setup guidance/tooltips | Yes | Must remain immediately understandable. |
| Samples | Optional | Must be replaceable and removable. |
| Runtime API/type names | No lore-only names | Technical meaning must be clear. |
| Project data | No required Hackulos content | Consumer owns game identity and content. |

---

## 2. Problem Statement

### 2.1 Current problem

`<Describe the repeated development cost, fragility, coupling, missing tooling, or user pain.>`

### 2.2 Evidence from existing work

| Source project | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| `<PROJECT>` | `<EVIDENCE>` | `<GOOD PRACTICE>` | `<LIMITATION TO REMOVE>` |

### 2.3 Consequences of doing nothing

- `<REPEATED COST>`
- `<COMMON FAILURE>`
- `<LIMIT TO REUSE OR TESTING>`

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- `<GOAL>`

### 3.2 Non-goals

- `<NON-GOAL>`

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | `<CONDITION>` | `<OUTCOME>` |
| Programmer | `<CONDITION>` | `<OUTCOME>` |
| Designer/content author | `<CONDITION>` | `<OUTCOME>` |
| Tester | `<CONDITION>` | `<OUTCOME>` |

### 3.4 Measurable success criteria

- `<PACKAGE>` installs into a clean supported Unity project with zero compile errors.
- Its core feature runs with no other Sperk’s Forge runtime package installed.
- Its Standalone Test Lab proves the advertised MVP.
- Its Package Reference Showcase demonstrates the normal consumer-facing happy path through documented public setup and APIs.
- Removing its samples does not break runtime code.
- Removing optional bridges does not break the package.
- Setup and repair operations are repeatable and non-destructive by default.
- `<PACKAGE-SPECIFIC MEASURE>`

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- `<USER TYPE>`

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| UC-001 | `<ACTION>` | `<ACTOR>` | `<PRECONDITION>` | `<RESULT>` | `<MVP/LATER>` |

### 4.3 Explicitly unsupported use cases

- `<UNSUPPORTED CASE AND REASON>`

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- `<OWNED RESPONSIBILITY>`

### 5.2 The package does not own

- `<EXCLUDED RESPONSIBILITY>`

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How this package interacts |
|---|---|---|
| `<CONCERN>` | `<PACKAGE OR PROJECT>` | `<EVENT / INTERFACE / BRIDGE / NONE>` |

### 5.4 Boundary tests

For every proposed feature, ask:

1. Does it directly support the one-sentence ownership contract?
2. Would a consumer reasonably expect to use it without installing another package?
3. Does adding it make this package aware of project-specific rules?
4. Is it definition/configuration, runtime state, or presentation?
5. Does a neighboring package already own the concern?
6. Would an optional bridge be cleaner than a dependency?

Features that fail these tests move to project code, a bridge, another package, or a deferred candidate list.

---

## 6. Independence Contract

Independence is a release gate, not a preference.

### 6.1 Standalone guarantees

The package must:

- Compile with only its declared Unity/platform dependencies.
- Initialize without First Light unless First Light is explicitly classified as mandatory in SFGSS-000.
- Function without Jukebot, EchoUI, EchoSave, EchoSettings, or other peers unless this is a bridge package.
- Avoid direct references to project assemblies.
- Keep game-specific data outside immutable package source.
- Expose a direct, documented setup path.
- Expose test injection or an explicit adapter seam where global access exists.
- Fail visibly and safely when optional collaborators are absent.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | `<EXPECTED>` | `<TEST>` |
| Enter Standalone Test Lab directly | `<EXPECTED>` | `<TEST>` |
| Optional bridge absent | `<EXPECTED>` | `<TEST>` |
| Optional package disabled | `<EXPECTED>` | `<TEST>` |
| Duplicate root present | `<EXPECTED>` | `<TEST>` |
| Required configuration missing | `<EXPECTED>` | `<TEST>` |
| Sample content deleted | `<EXPECTED>` | `<TEST>` |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---:|---|---|---|
| Unity module/package | Platform | `<YES/NO>` | `<VERSION>` | `<REASON>` | `<BEHAVIOR>` |

### 6.4 Forbidden dependencies

- Project-specific code or assemblies.
- Another Sperk’s Forge runtime package unless SFGSS-000 classifies this artifact as a bridge/composer/provider adapter.
- Samples, test assets, or Editor assemblies at runtime.
- Unlicensed or non-redistributable third-party content.
- Hidden assumptions about scene names, build indices, input maps, save files, tags, layers, or folder locations.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Runtime/Editor/Sample | Notes |
|---|---|---|---|---:|---|---|
| CAP-001 | `<NAME>` | `<DESCRIPTION>` | Proposed | Yes | Runtime | `<NOTES>` |

### 7.2 MVP capability set

`<Define the smallest complete release that is genuinely useful and independently testable.>`

### 7.3 Later capability set

`<List approved later phases without allowing them to inflate the MVP.>`

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| `<IDEA>` | `<DEFERRED/REJECTED>` | `<REASON>` | `<TRIGGER>` |

---

## 8. Architecture Overview

### 8.1 Design model

Describe the package in three distinct layers:

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | ScriptableObjects, immutable definitions, policies, authoring data | Per-session mutable state or active scene references |
| Runtime state/behavior | Active services, state machines, queues, handles, session values | Editor-only logic or UI presentation assumptions |
| Presentation/feedback | Views, presenters, overlays, optional sample UI | Authoritative game state or persistence ownership |

### 8.2 Component topology

`<Describe the smallest useful component relationship. Add a compact Mermaid diagram only when ownership or lifecycle is materially clearer visually.>`

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | `<YES/NO>` |
| Root type | `<TYPE OR N/A>` |
| Duplicate behavior | `<REJECT / REUSE / ERROR POLICY>` |
| Initialization trigger | `<AWAKE / EXPLICIT / STARTUP STEP / OTHER>` |
| Shutdown behavior | `<BEHAVIOR>` |
| Direct-scene behavior | `<BEHAVIOR>` |
| Test injection seam | `<INTERFACE / FACTORY / ADAPTER>` |

If the package has a persistent root, subsystem children must be owned by that root rather than becoming independent persistent singletons.

### 8.4 Lifecycle sequence

1. `<CREATION>`
2. `<VALIDATION>`
3. `<INITIALIZATION>`
4. `<READY>`
5. `<NORMAL OPERATION>`
6. `<SUSPEND/SCENE CHANGE IF RELEVANT>`
7. `<SHUTDOWN>`

### 8.5 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| `<FAILURE>` | `<POINT>` | `<RESULT>` | `<FALLBACK>` | `<CODE>` |

---

## 9. Runtime Data and State Model

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable at runtime? | Project-owned instance? |
|---|---|---:|---:|---:|
| `<TYPE>` | `<PURPOSE>` | `<YES/NO>` | No | Yes |

### 9.2 Runtime state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| `<STATE>` | `<OWNER>` | `<LIFETIME>` | `<RESET>` | `<RULE>` |

### 9.3 Stable identifiers

Document:

- ID format and generation.
- Duplicate detection.
- Rename behavior.
- Alias/migration policy.
- Human-readable display-name separation.
- Validation rules.

### 9.4 ScriptableObject safety

ScriptableObjects may hold reusable definitions and configuration. They must not hold mutable session state such as active cooldown timestamps, sequential playback indices, runtime ownership, current save values, or scene object references unless the specification explicitly proves why the asset is intentionally runtime-mutated and how contamination is prevented.

### 9.5 Serialization and migration

`<Define version fields, compatibility expectations, migration ownership, and behavior for unknown or older data.>`

---

## 10. Public Runtime API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| `<TYPE>` | `<CLASS/INTERFACE/STRUCT/ENUM>` | `<RESPONSIBILITY>` | `<OWNER>` |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/main-loop rule |
|---|---|---|---|---|
| `<SIGNATURE>` | `<PURPOSE>` | `<PRECONDITION>` | `<RESULT>` | `<RULE>` |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| `<EVENT>` | `<OWNER>` | `<TIMING>` | `<PAYLOAD>` | `<ASSUMPTIONS>` |

Events must be raised after authoritative state changes. Presentation listeners must never be required for a state change to complete.

### 10.4 Async and cancellation policy

`<Document Tasks/coroutines, cancellation, scene destruction, timeouts, re-entry, and completion callbacks.>`

### 10.5 API ergonomics

Provide one novice-friendly path and one extensible programmer path. Convenience access must not be the only way to test or substitute the implementation.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

Describe the exact first-run experience:

1. `<INSTALL>`
2. `<OPEN SETUP TOOL OR CREATE ASSET>`
3. `<SELECT OPTIONS>`
4. `<PREVIEW CHANGES>`
5. `<GENERATE/APPLY>`
6. `<OPEN STANDALONE TEST LAB>`
7. `<VALIDATE>`

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---:|---|---|
| `<OPERATION>` | `<ITEMS>` | `<ITEMS>` | Yes | `<METHOD>` | `<REPORT>` |

Setup, generation, migration, and repair tools must be non-destructive by default. They must preview or clearly report changes and must not overwrite project-authored assets silently.

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---:|
| `<TOOL>` | `<USER>` | `<PURPOSE>` | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---:|---:|
| `<PKG>-VAL-001` | `<CONDITION>` | `<INFO/WARNING/ERROR/BLOCKER>` | `<YES/NO>` | `<YES/NO>` |

---

## 12. Installation, Scene Setup, and Direct Testing

### 12.1 Installation routes

- Unity Package Manager Git URL.
- Local package/tarball during development.
- Embedded package for package development.
- The Workshop selection when available.

Every independently distributed package also defines a repository-owned versioned Distribution Kit. The kit is the handoff surface for the exact artifact being evaluated; it is not itself an installation-support claim.

Document which routes are Supported, Planned, Unknown, or Not applicable for the current release.

### 12.1.1 Distribution Kit

Record:

- Repository-relative kit root: `Distributions/<Public Title>/<Package Version>/` unless the package authority approves another deterministic path.
- Exact artifact filename and format.
- Complete user handout filename.
- Distribution manifest filename.
- SHA-256 checksum record.
- Build record containing source baseline, package identity/version, artifact size, and artifact hash.
- Whether the artifact has completed clean-project installation proof.
- Which release/support claims remain pending.

The kit must remain self-contained enough that a recipient can identify the artifact, verify integrity, understand installation/setup/capabilities/limitations, and report a useful issue without requiring the historical development conversation.

For one retained package version, do not silently replace an already-published kit with a materially different artifact. Create a new package version or an explicitly recorded corrected artifact according to release authority.

### 12.2 Minimal scene setup

`<List the minimum GameObjects, components, assets, and assignments required.>`

### 12.3 Boot-scene setup

`<Explain the normal production setup, or mark not applicable.>`

### 12.4 Direct-scene setup

`<Explain how a developer enters an isolated gameplay/test scene without silently creating duplicate authorities.>`

### 12.5 Scene isolation rule

Every scene-visible package feature must be provable in an isolated scene that contains no unrelated package code. Lightweight test utilities may be included in the sample but cannot become production requirements.

---

## 13. Standalone Test Lab, Package Reference Showcase, and Samples
### 13.1 Standalone Test Lab purpose

`<Define the one scene or minimal scene set that proves the package’s core loop by itself as engineering evidence.>`

### 13.2 Required Test Lab contents

- Plain setup instructions visible in the scene or sample README.
- Minimal configuration assets.
- Test controls that do not require another Echo package.
- Visual state readout where useful.
- Success, empty, invalid, and failure cases.
- Duplicate-root test when a persistent authority exists.
- Reset control for repeatable testing.
- No project-owned or restricted content.

### 13.3 Test Lab acceptance checklist

| Test | Action | Expected result | Automated/manual | Status |
|---|---|---|---|---|
| LAB-001 | `<ACTION>` | `<RESULT>` | `<TYPE>` | Not run |

### 13.4 Required in-house Package Reference Showcase

Define the smallest clean project-owned scene, scene set, or Editor workspace that demonstrates how a real consumer uses the package correctly after the Standalone Test Lab has passed.

The Reference Showcase must:

- use only documented public package setup, configuration, prefabs, APIs, and extension seams;
- avoid test-only APIs, hidden internals, privileged repository state, and unrelated package dependencies;
- default to the front-facing production-style experience rather than diagnostic instrumentation;
- keep game/studio art, scenes, configuration, and branding project-owned;
- live outside immutable package source, normally under `Assets/EchoDevGames/SuiteShowcase/<Package>/` in the integration/development workspace;
- remain conceptually reproducible in a clean consumer project;
- document when one scene is insufficient and a minimal scene set is required;
- use a non-scene equivalent only when the package is genuinely Editor-only or otherwise has no meaningful runtime scene.

| Reference Showcase field | Required definition |
|---|---|
| Project-owned path | `<PATH>` |
| Scene(s)/workspace | `<SCENES OR EDITOR SURFACE>` |
| Consumer setup path exercised | `<SETUP / ASSET / PREFAB / API>` |
| Front-facing happy path | `<WHAT A USER/PLAYER SEES>` |
| Diagnostics default | `<HIDDEN / OPTIONAL / DEVELOPMENT>` |
| Clean-project reproduction | `<HOW THIS FLOW IS REPRODUCED>` |
| Not a proof of | `<STANDALONE EDGE CASES / BRIDGE / PROVIDER / OTHER>` |

### 13.5 Optional distributed showcase and integration samples

A package may additionally distribute a showcase or integration sample when useful. That sample is a package distribution surface and remains separate from the required in-house Reference Showcase.

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| `<SAMPLE>` | `<PACKAGES>` | `<PURPOSE>` | `<REASON>` |

Samples must be separately importable and removable.

### 13.6 Suite Showcase Hub relationship

The project-owned **Suite Showcase Hub** may later link or launch package Reference Showcases and combined demonstrations. It is an integration/presentation surface, not a runtime package, dependency, or substitute for package-specific evidence.

## 14. Presentation, UI, and Accessibility

If the package is nonvisual, define only the minimal status/error surface and optional presenters.

### 14.1 Presentation ownership

`<State whether presentation is core, optional, sample-only, or owned by EchoUI through a bridge.>`

### 14.2 Required states

- Ready.
- Busy/loading.
- Empty.
- Disabled/unavailable.
- Warning.
- Failure.
- `<PACKAGE-SPECIFIC STATE>`

### 14.3 Accessibility requirements

Consider:

- Keyboard/controller navigation.
- Screen-reader/assistive labeling where supported.
- Reduced motion.
- Scalable text and readable contrast.
- Color-independent status indicators.
- Subtitle/caption hooks for audiovisual content.
- User-configurable timing where relevant.

### 14.4 Visual customization

Project-specific visuals must be replaceable without editing package runtime code.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

The package must remain diagnosable without the Observatory installed.

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| `<STATUS>` | `<INSPECTOR/REPORT/LOG/API>` | `<EDITOR/DEV/RELEASE>` | `<COST>` |

### 15.2 Structured status

Define:

- Initialization state.
- Authority/root identity.
- Configuration source.
- Current warnings/errors.
- Version/build information.
- Runtime counters relevant to the package.

### 15.3 Diagnostic codes

Use stable, searchable codes:

| Code | Severity | Meaning | User action |
|---|---|---|---|
| `<PKG>-001` | `<SEVERITY>` | `<MEANING>` | `<ACTION>` |

### 15.4 Observatory bridge

`<Define the optional metrics/status provider exposed to EchoDiagnostics. The package must not depend on the Observatory.>`

### 15.5 Logging policy

- Categorized and searchable.
- No per-frame spam in normal operation.
- Actionable warnings.
- Sensitive/user data excluded or redacted.
- Development verbosity separable from release reporting.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---:|---|
| `<STATE>` | `<SESSION/GLOBAL/PROFILE/SLOT>` | `<OWNER>` | `<YES/NO>` | `<BACKEND>` |

### 16.2 Standalone behavior

`<Explain how the package behaves without EchoSave or EchoSettings.>`

### 16.3 Optional participant/provider contract

`<Define versioned save/settings contributions without direct knowledge of project databases or static stores.>`

### 16.4 Failure and recovery

`<Define missing, corrupt, older, newer, locked, and partially written data behavior.>`

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Optional connections must be explicit, documented, removable, and versioned. Installing a peer package must not silently change core behavior.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---:|
| `<PACKAGE>` | `<INTERFACE/BRIDGE/PROJECT ADAPTER>` | `<OWNER>` | `<DIRECTION>` | `<DATA>` | No |

### 17.3 Bridge placement decision

Choose and explain one:

- Tiny compile-safe optional integration inside an owner package.
- Separate two-package bridge.
- Provider adapter package.
- Project-local adapter.

### 17.4 Integration failure behavior

`<Define version mismatch, missing peer, disabled peer, initialization order, and teardown behavior.>`

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| `<METRIC>` | `<TARGET>` | `<METHOD>` | `<THRESHOLD>` |

### 18.2 Allocation policy

`<Document hot paths, pooling, allocations, LINQ/reflection restrictions if relevant, and profiling requirements.>`

### 18.3 Scene and domain reload behavior

`<Document event unsubscription, static reset, Enter Play Mode options, domain reload, and duplicate cleanup.>`

### 18.4 Scalability limits

`<Document advertised limits, tested limits, and graceful degradation.>`

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

`<State whether the package handles personal, credential, network, analytics, filesystem, or platform data.>`

### 19.2 Trust boundaries

`<Define validation of external files, network input, user-authored data, and provider responses.>`

### 19.3 Platform behavior

| Platform | Supported? | Special behavior | Validation required |
|---|---:|---|---|
| Windows | `<YES/NO>` | `<BEHAVIOR>` | `<TEST>` |
| macOS | `<YES/NO>` | `<BEHAVIOR>` | `<TEST>` |
| Linux | `<YES/NO>` | `<BEHAVIOR>` | `<TEST>` |
| WebGL | `<YES/NO>` | `<BEHAVIOR>` | `<TEST>` |
| Mobile | `<YES/NO>` | `<BEHAVIOR>` | `<TEST>` |
| Console | `<PLANNED/UNKNOWN>` | `<BEHAVIOR>` | `<TEST>` |

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.<package-id>/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Runtime/
│   └── EchoDevGames.<Package>.Runtime.asmdef
├── Editor/
│   └── EchoDevGames.<Package>.Editor.asmdef
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   └── Developer/
│       ├── Architecture.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Samples~/
└── Tests/
    ├── Editor/
    └── Runtime/
```

Remove a directory only when the package specification explicitly establishes that it is unnecessary.

### 20.2 Proposed source tree

```text
<INSERT PACKAGE-SPECIFIC TREE>
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---:|---|
| `<ASSEMBLY>` | `<RUNTIME/EDITOR>` | `<REFERENCES>` | `<YES/NO>` | `<PURPOSE>` |

### 20.4 Repository files

- README.
- Package documentation.
- Documentation index with a visible link to `Current Notes.md`.
- Obsidian-compatible Markdown links between specifications, ADRs, checkpoints, tests, and guides.
- Changelog.
- License.
- Third-party notices.
- Contribution/development notes if public collaboration is allowed.
- Release checklist.
- Stable `.meta` files and GUIDs.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | `<VERSION>` | `<VERSIONS>` | `<NOTES>` |

### 21.2 Semantic versioning policy

`<Define what counts as patch, minor, and major for public APIs, assets, serialization, scenes, and setup output.>`

### 21.3 Deprecation policy

`<Define warning period, migration path, removal threshold, and documentation behavior.>`

### 21.4 GUID and asset compatibility

Public scripts, prefabs, templates, definitions, and samples must preserve committed `.meta` files. Moves and renames must retain GUIDs whenever the asset identity is intended to survive.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Package overview and boundaries.
- Installation.
- **Complete user handout** covering installation, setup, all implemented capabilities, workflows, diagnostics, troubleshooting, limitations, evidence/qualification status, removal/reinstall, reference examples, and support-reporting requirements.
- Five-minute quick start.
- Full setup guide.
- Standalone Test Lab guide.
- Runtime API examples.
- Configuration/data authoring guide.
- Troubleshooting and diagnostic-code reference.
- Migration/upgrade guide.
- Optional integration guide index.
- Known limitations.
- License, credits, and third-party notices.

### 22.2 Required developer documentation

- Architecture overview.
- Lifecycle and authority model.
- Extension points.
- Testing strategy.
- Release workflow.
- Architecture decisions.
- Current checkpoint/status record.
- Linked `Current Notes.md` living-development page.

### 22.3 Documentation truth rule

Documentation examples must compile against the documented release. Setup screenshots and menu paths must match the current Unity baseline. A feature is not release-ready when its user documentation is knowingly stale.

### 22.4 Living repository and Obsidian workflow

Documentation must live in the Git repository with the implementation it describes. Obsidian must open those same Markdown files directly; it must not become an untracked duplicate-documentation system.

`Current Notes.md` is the rolling capture surface for active observations, proposed decisions, questions, test findings, defects, risks, and handoff details. Entries must clearly distinguish facts from proposals. The page is not a higher authority than SFGSS-000, the approved package specification, or an ADR.

At each meaningful checkpoint:

1. Review all current notes added since the previous checkpoint.
2. Promote durable architectural or behavioral decisions into the appropriate specification or ADR.
3. Move defects and test evidence into the issue/test record.
4. Move user-visible changes into guides and the changelog.
5. Update the current checkpoint/status record and next action.
6. Mark, condense, or remove resolved notes after promotion; rely on Git history for archival detail.
7. Commit the documentation with the related implementation when practical, or in an immediately adjacent clearly labeled documentation commit.

Device-specific Obsidian workspace state should remain untracked unless the repository deliberately adopts a shared configuration.

### 22.5 Repository scan and handoff order

Before changing a package, a developer or new ChatGPT conversation should scan:

1. Repository README/documentation index.
2. SFGSS-000.
3. This package specification.
4. Applicable ADRs and bridge specifications.
5. `Current Notes.md`.
6. Current checkpoint, tests, issue log, and changelog.
7. Relevant implementation and automated tests.

This scan order must be stated in the package's contributor/development guide.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---:|
| EditMode unit | Definitions, validation, pure policies | `<EXAMPLES>` | Yes |
| PlayMode unit/integration | Runtime lifecycle and behavior | `<EXAMPLES>` | Yes |
| Standalone Test Lab | User-visible isolated engineering proof | `<EXAMPLES>` | Yes |
| Package Reference Showcase | Project-owned consumer-style happy path | `<EXAMPLES>` | Yes before external beta unless justified Not applicable |
| Bridge Integration Lab | Optional package connection | `<EXAMPLES>` | When bridge ships |
| Combined/Suite Showcase | Multi-system presentation such as the Suite Showcase Hub | `<EXAMPLES>` | No; later integration evidence |
| Clean-project install | Packaging and missing-dependency proof | `<EXAMPLES>` | Yes |
| Existing-project migration | Adoption without regressions | `<EXAMPLES>` | Before integration claim |

### 23.2 Required test categories

- Happy path.
- Missing configuration.
- Invalid configuration.
- Empty data.
- Duplicate authorities.
- Scene transitions.
- Repeated initialization and teardown.
- Direct-scene entry.
- Sample removal.
- Optional integration absent/present.
- Enter Play Mode configuration where relevant.
- Build validation on supported platforms.
- Performance budget.
- Serialization/version migration where relevant.

### 23.3 Test case registry

| Test ID | Requirement | Setup | Action | Expected result | Automated? | Status |
|---|---|---|---|---|---:|---|
| `<PKG>-T-001` | `<REQ>` | `<SETUP>` | `<ACTION>` | `<RESULT>` | `<YES/NO>` | Not run |

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [ ] Ownership and non-ownership are approved.
- [ ] MVP and deferred scope are separated.
- [ ] Required dependencies are explicit.
- [ ] Public API and data model are defined.
- [ ] Standalone Test Lab is designed.
- [ ] Package Reference Showcase or justified non-scene equivalent is defined.
- [ ] Release-blocking questions are resolved.

### 24.2 Implementation gate

- [ ] Runtime code compiles with declared dependencies only.
- [ ] Editor code is isolated from runtime assemblies.
- [ ] Setup is repeatable and non-destructive.
- [ ] Duplicate and lifecycle behavior are validated.
- [ ] Public API matches the specification or the specification/ADR was updated first.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Package works without unrelated Sperk’s Forge packages.
- [ ] Standalone Test Lab passes.
- [ ] Samples can be removed safely.
- [ ] Direct-scene entry behaves as documented.

### 24.4 Quality gate

- [ ] Automated tests pass.
- [ ] Manual checklist passes.
- [ ] No known blocker or critical defect remains.
- [ ] Performance targets pass.
- [ ] Diagnostics are actionable.
- [ ] Package Reference Showcase demonstrates the normal happy path through documented public consumer surfaces.
- [ ] Documentation matches the build.
- [ ] `Current Notes.md` has been reconciled for this checkpoint.
- [ ] Durable decisions have been promoted into the specification or ADRs.
- [ ] Licenses and notices are complete.

### 24.5 Distribution gate

- [ ] Package manifest is valid.
- [ ] Version and changelog are updated.
- [ ] Stable `.meta` files are included.
- [ ] Versioned repository Distribution Kit exists with exact artifact, complete user handout, manifest, SHA-256 record, and build record.
- [ ] Distribution Kit handout and manifest describe the current artifact/version and do not overstate untested support.
- [ ] Tarball/Git installation is tested in another clean project before those routes are claimed Supported.
- [ ] Repository tag/release is prepared.
- [ ] Repository documentation and current status are committed and pushed.
- [ ] Compatibility catalog is updated.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| `<PROJECT>` | `<SYSTEM>` | `<STRATEGY>` | `<GATE>` | `<METHOD>` |

### 25.2 Preserve-until-parity rule

Existing working project code remains intact until the package proves feature parity in isolation and then in the target project. Replacement must be incremental and reversible. Project-specific assets remain owned by the project.

### 25.3 Migration tooling

`<Define detection, preview, backup, conversion, validation, repair, and rollback.>`

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| R-001 | `<RISK>` | `<LOW/MED/HIGH>` | `<LOW/MED/HIGH>` | `<MITIGATION>` | `<TRIGGER>` |

Required risks to evaluate:

- Scope inflation.
- Hidden cross-package dependency.
- Persistent-root duplication.
- Mutable shared asset state.
- Unity version/API drift.
- Package GUID breakage.
- Setup overwriting project content.
- Sample becoming a runtime requirement.
- Insufficient diagnostics.
- Existing-project regression.

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---:|
| `<PKG>-D-001` | `<DECISION>` | `<PROPOSED/APPROVED>` | `<REASON>` | `<CONSEQUENCE>` | `<YES/NO>` |

### 27.2 Release-blocking questions

| Question | Why it blocks | Options | Owner | Due before |
|---|---|---|---|---|
| `<QUESTION>` | `<REASON>` | `<OPTIONS>` | Jesse | `<MILESTONE>` |

### 27.3 Non-blocking later questions

- `<QUESTION>`

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 — Specification | Approved package contract | Design only | Approved document |
| M1 — Skeleton | Installable package anatomy | Manifest, assemblies, docs shell | Clean compile |
| M2 — Runtime core | Smallest authoritative behavior | `<CAPABILITIES>` | Automated tests |
| M3 — Test Lab | Isolated usable proof | `<CAPABILITIES>` | Manual/automated checklist |
| M4 — Tooling | Safe setup and validation | `<CAPABILITIES>` | Repeatability tests |
| M5 — Integration | First optional bridge/project adoption | `<CAPABILITIES>` | Integration Lab/parity report |
| M6 — Release | Distribution-ready version | Docs, licenses, package | Clean external install |

### 28.2 Checkpoint rule

Every milestone is split into small Checkpoint Build Plans using SFGSS-005. A checkpoint must produce one testable outcome, specify exact files and Editor work, include a stop point, and end with test, commit, push, devlog, and documentation updates as appropriate.

### 28.3 First recommended checkpoint

`<DEFINE THE SMALLEST SAFE, TESTABLE FIRST CHECKPOINT AFTER APPROVAL.>`

---

## 29. New-Conversation Handoff

Use this package-specific prompt with the suite bible, this approved specification, SFGSS-005, and the latest checkpoint status:

```text
We are continuing development of The Sperk’s Forge — EchoDevGames Game Systems Suite.

Treat SFGSS-000 as the authority for suite-wide boundaries and architecture.
Treat this <PACKAGE> Specification as the authority for this package’s behavior,
public API, data model, tooling, Test Lab, and release gates. Follow SFGSS-005
for implementation checkpoints.

Current package: <PACKAGE>
Current specification version: <VERSION>
Current milestone/checkpoint: <CHECKPOINT>
Current Unity version: <VERSION>
Current project/repository: <PROJECT>
Current implementation status: <STATUS>
Known blockers: <BLOCKERS>

Before writing code:
1. Summarize the package’s ownership and independence constraints.
2. Identify any conflict or unresolved decision that materially affects this checkpoint.
3. Keep optional integrations behind documented bridges or project adapters.
4. Preserve existing project systems until replacement parity is proven.
5. Continue using the Checkpoint Build Plan format.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | `<VERSION>` |
| Completed checkpoint | `<CHECKPOINT>` |
| Files/assets created | `<LIST>` |
| Tests passed | `<LIST>` |
| Tests failed | `<LIST>` |
| Known issues | `<LIST>` |
| Decisions added | `<LIST>` |
| Next checkpoint | `<CHECKPOINT>` |

---

## 29.1 Package Graduation Path

Each package specification must map its work onto the suite graduation loop:

| Graduation stage | Package-specific proof |
|---|---|
| Learning / research | `<PACKAGE LEARNING REVIEW>` |
| Authority / plan | `<SPEC VERSION + CHECKPOINT BUILD PLAN>` |
| Implementation / regression | `<AUTOMATED TEST GATES>` |
| Standalone Test Lab | `<ISOLATED ENGINEERING PROOF>` |
| Package Reference Showcase | `<IN-HOUSE CONSUMER HAPPY PATH>` |
| Versioned Distribution Kit | `<EXACT ARTIFACT + COMPLETE HANDOUT + MANIFEST + SHA256/BUILD RECORD>` |
| Clean-project reproduction | `<OUTSIDE-REPOSITORY CONSUMER PROOF USING THE INTENDED ARTIFACT>` |
| Release qualification | `<INSTALL ROUTES / PLAYER BUILDS / PERFORMANCE / VERSIONING AS APPLICABLE>` |
| Private beta / external adoption | `<HANDOFF + FEEDBACK PLAN>` |

A package may add stages, but it may not collapse the Standalone Test Lab, Package Reference Showcase, and clean-project reproduction into one artifact or one proof.

## 30. Approval

### 30.1 Approval checklist

- [ ] Package identity and plain responsibility are clear.
- [ ] Ownership and non-ownership boundaries align with SFGSS-000.
- [ ] Independence proof is credible.
- [ ] MVP is small enough to complete and large enough to be useful.
- [ ] Public API, data, lifecycle, and failure behavior are specified.
- [ ] Setup and direct-scene workflows are understandable.
- [ ] Standalone Test Lab is fully defined.
- [ ] Package Reference Showcase or justified non-scene equivalent is fully defined.
- [ ] Diagnostics exist without requiring the Observatory.
- [ ] Optional integrations are explicitly separated.
- [ ] Test and release gates are measurable.
- [ ] No Isekai Studios identity or ownership has been introduced.
- [ ] Jesse has approved the specification for implementation.

### 30.2 Approval record

**Decision:** `<APPROVED / REVISE / DEFERRED>`

**Approved by:** `<NAME>`

**Date:** `<DATE>`

**Conditions or notes:** `<NOTES>`

---

## Template Completion Rule

A package specification is complete when a new collaborator can answer all of the following without consulting an old conversation:

1. What does the package own?
2. What does it explicitly refuse to own?
3. What is the smallest useful release?
4. How does it work when installed alone?
5. What data is definition/configuration versus mutable runtime state?
6. What is the public API and lifecycle?
7. What happens when setup, configuration, or runtime behavior fails?
8. How is the package configured and tested in an isolated scene?
9. How do optional packages connect without becoming dependencies?
10. What evidence is required before the package can be released or adopted by an existing game?

If any answer is still implicit, the specification remains **Proposed**.


---


## SUITE-DOC-30 Template Consistency Amendment

This template now assumes the complete standards set SFGSS-002 through SFGSS-010.

- Formal public titles use the SFGSS-008 canonical spaced en dash; ASCII-only surfaces may use the registered spaced-hyphen fallback.
- Package metadata must include the registered document ID, package ID, namespace family, diagnostic/test prefix, setup facade, and planned repository.
- Parent-authority headers record approval provenance. Later standards alignment is recorded through revision history and a consistency addendum rather than rewriting historical approval context.
- Editor, test, sample, bridge, provider, and internal-support assemblies default to `autoReferenced: false`; primary public Runtime assemblies default to `true` under SFGSS-002.
- Test definitions separate automation class, execution status, evidence reference, and issue reference under SFGSS-004.
- Unity asset GUIDs, domain IDs, and runtime instance IDs remain distinct under SFGSS-003.
- Unknown optional data and unknown fields require explicit opaque-record or extension-capable preservation strategies.
- Every approved package maintains Graph Navigation and participates in the pre-implementation learning review.

## Graph Navigation

#sfgss/authority #sfgss/navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
