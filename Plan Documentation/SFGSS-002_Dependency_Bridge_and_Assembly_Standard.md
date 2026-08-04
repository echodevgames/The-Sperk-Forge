# The Sperk’s Forge — Dependency, Bridge, and Assembly Standard

**Document ID:** SFGSS-002  
**Version:** 1.1.1
**Status:** Approved architecture standard  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000 v0.22.0  
**Related authorities:** SFGSS-001, SFGSS-ADR-001, SFGSS-ADR-002, SFGSS-ADR-004, SFGSS-INT-FOUNDATION-001  
**Current development baseline:** Unity 6000.3.8f1  
**Minimum planned public Unity floor:** Unity 6000.0  
**Last updated:** August 4, 2026

> A package should reveal its alliances at the gate, not smuggle them through a side window.

---

## Contents

1. Purpose and authority
2. Scope and non-goals
3. Terminology
4. Governing principles
5. Dependency taxonomy
6. Package dependency rules
7. Package manifest rules
8. Assembly architecture
9. Assembly reference direction
10. Assembly Definition property policy
11. Optional integration classification
12. Bridge package standard
13. Owner-contained integration standard
14. Provider adapter standard
15. Project adapter standard
16. Workshop Editor setup facades
17. Compile guards and version defines
18. Reflection and dynamic discovery
19. Samples, laboratories, and showcase dependencies
20. Test assembly standard
21. Platform and backend assemblies
22. Lifecycle, registration, and teardown
23. Failure and compatibility behavior
24. Clean removal and replacement
25. Naming registry
26. Documentation and compatibility records
27. Validation and release gates
28. Foundation application matrix
29. Reconciliation history
30. Approval

---

## 1. Purpose and authority

SFGSS-002 defines the canonical dependency, bridge, provider-adapter, Assembly Definition, compilation, and clean-removal rules for **The Sperk’s Forge — EchoDevGames Game Systems Suite**.

SFGSS-000 establishes that packages are standalone first, compose through explicit seams, and avoid hidden cross-package coupling. This standard turns those principles into concrete rules that package specifications, bridge specifications, implementation checkpoints, setup tools, repositories, and releases must follow.

This document answers the questions that otherwise tend to become invisible implementation accidents:

- When is a dependency genuinely hard?
- When must an integration become a separate package?
- Which assembly may reference which other assembly?
- How are optional providers isolated?
- When may compile guards or reflection be used?
- What must happen when a bridge or peer package is removed?
- How are samples and tests prevented from becoming production dependencies?
- How does The Workshop invoke package setup without becoming a compile-time dependency hub?

### 1.1 Authority order

When dependency documents disagree, use this order:

1. SFGSS-000 suite boundaries and approved ownership.
2. The package specifications for the packages involved.
3. This standard.
4. An accepted bridge/integration specification or provider-adapter specification.
5. An accepted ADR that explicitly changes a rule.
6. Checkpoint plans, setup guides, tests, release records, and Current Notes.

A lower document may add detail. It must not silently reverse dependency direction, create a new hard dependency, or weaken clean-removal guarantees.

### 1.2 Requirement language

- **Must** means release-blocking.
- **Must not** means prohibited unless a higher authority or accepted ADR grants an explicit exception.
- **Should** means the default choice; deviation requires a written reason.
- **May** means optional.

---

## 2. Scope and non-goals

### 2.1 In scope

This standard governs:

- UPM package dependency declarations.
- Runtime, Editor, presentation, backend, provider, bridge, sample, and test assemblies.
- Assembly Definition and Assembly Definition Reference use.
- Core-to-peer and bridge-to-peer reference direction.
- Package-presence and version-based compile behavior.
- Package installation, removal, replacement, and teardown order.
- Optional integration failure behavior.
- Workshop setup-facade boundaries.
- Cross-package compatibility documentation and validation.

### 2.2 Not in scope

This standard does not define:

- The detailed public API of an individual package.
- Stable data identifiers, serialization formats, or migration algorithms, which belong to SFGSS-003.
- The full test taxonomy and evidence model, which belong to SFGSS-004.
- Repository release/tagging policy beyond dependency-facing requirements, which belongs to SFGSS-009.
- The implementation of any bridge, provider, package, or source file.
- A mandatory shared `EchoCore` package.

---

## 3. Terminology

| Term | Meaning |
|---|---|
| **Core package** | A package whose central promise remains useful when installed without another Sperk’s Forge package. |
| **Peer package** | Another independently distributed Sperk’s Forge package. |
| **Platform dependency** | A Unity module or approved Unity package required for the central package feature. |
| **Hard Echo dependency** | An explicit UPM and assembly dependency on another Sperk’s Forge package without which the artifact cannot function. |
| **Bridge** | A translation artifact that connects two independently authoritative packages without replacing either authority. |
| **Owner-contained integration** | A very small optional integration shipped with the package that owns the extended behavior and completely isolated from its core assembly. |
| **Provider-neutral core** | A package that defines contracts without depending on one vendor/backend implementation. |
| **Provider adapter** | A separately distributed package that connects a provider-neutral core to a vendor, platform, SDK, storage backend, camera backend, network stack, or service. |
| **Project adapter** | Game-owned translation code under the consuming project rather than reusable package source. |
| **Presentation adapter** | An assembly that connects a package’s neutral runtime state to a specific UI/presentation technology. |
| **Backend adapter** | An assembly that connects a neutral capability to a technical backend such as filesystem, Addressables, Cinemachine, or networking. |
| **Setup facade** | The package-owned Editor endpoint governed by SFGSS-ADR-001 and invoked by The Workshop through a reviewed descriptor. |
| **Assembly Definition** | A Unity `.asmdef` asset that defines one managed compilation unit. |
| **Assembly Definition Reference** | A Unity `.asmref` asset that contributes scripts in another folder to an existing assembly. |
| **Predefined assembly** | Unity’s default project assemblies such as `Assembly-CSharp`, rather than an assembly created by an `.asmdef`. |
| **Compile guard** | A define constraint, version define, or preprocessor condition that includes/excludes code. |
| **Standalone proof** | Evidence that a package works with only declared platform dependencies. |
| **Integration proof** | Evidence that one explicit bridge or adapter works with all declared peers/providers present. |
| **Clean removal** | Removing an optional artifact without corrupting unrelated project data or leaving unrelated assemblies unable to compile. |

---

## 4. Governing principles

### 4.1 Dependencies are part of the public contract

A dependency changes installation, compilation, upgrade, licensing, build size, platform reach, removal, and support. It must never be treated as an implementation detail that can be added casually.

### 4.2 Core packages point inward, bridges point outward

A core package references only:

- Unity/runtime APIs required by its own promise.
- Its own assemblies.
- Approved third-party/platform dependencies declared in its manifest.

A separate bridge references the two or more packages it connects. The connected cores do not reference the bridge.

### 4.3 One authority remains one authority

A bridge may translate, register, request, observe, or apply. It must not create a competing save service, audio player, UI root, scene authority, state authority, input authority, or other peer-owned truth.

### 4.4 Absence is a supported state

Optional integrations must fail by becoming unavailable, not by producing missing-reference compile errors in a core package.

### 4.5 Compilation structure must explain architecture

Assembly boundaries are not decorative folders. The reference graph must visibly match the approved authority graph.

### 4.6 Removal is designed before installation

Every optional dependency, bridge, provider, sample, and generated adapter must define how it is detached and removed before it is release-ready.

### 4.7 Compile guards do not erase dependency truth

A symbol can hide code from the compiler. It cannot transform an undeclared vendor SDK, peer assembly, license, or runtime expectation into a dependency-free feature.

### 4.8 Shared code requires evidence

The suite does not create a common contracts package merely to avoid repeating a few small types. A shared package is approved only when at least three independent packages need the same genuinely neutral contract and the versioning cost is lower than local ownership.

---

## 5. Dependency taxonomy

| Class | Declared in package manifest? | Direct asmdef reference? | Separate artifact? | Typical use |
|---|---:|---:|---:|---|
| Unity module/API | As required by Unity/package rules | Yes when assembly reference is needed | No | Scene management, audio, core engine |
| Approved Unity package | Yes | Yes | No | Input System, uGUI, Test Framework for tests |
| Required third-party package/SDK | Yes | Yes | Usually provider package preferred | A package’s central feature genuinely requires it |
| Hard Echo dependency | Yes | Yes | Artifact is classified bridge/composer/provider or explicitly approved exception | Workshop Editor package, future provider family |
| Separate two-package bridge | Yes, both peers | Yes, both peer assemblies | Yes | Accord + Jukebot application bridge |
| Owner-contained integration | Dependency declared if direct types are used | Isolated assembly only | No, but isolated from core | Small integration with no independent release burden |
| Provider adapter | Yes, neutral core + provider | Yes | Yes | EchoMultiplayer provider, cloud save provider |
| Project adapter | Project manifest owns package dependencies | Project asmdef references peers | Game-owned | Rescuers2D-specific translation |
| Sample dependency | Must not leak into core; either already a hard dependency or separately isolated | Sample assembly only | Sometimes separate sample companion | Lab controls/presentation |
| Test dependency | Development/test declaration as supported | Test assembly only | No | Unity Test Framework |
| Editor-only dependency | Yes if external package is required | Editor assembly only | No or separate Editor adapter | UI Toolkit Editor APIs, package manager tooling |

### 5.1 Default classification

When a connection could be classified multiple ways, choose the artifact with the smallest truthful dependency footprint:

1. Project adapter for game-specific behavior.
2. Separate bridge for reusable two-package behavior.
3. Provider adapter for vendor/backend behavior.
4. Owner-contained integration only when it remains tiny, compile-isolated, license-neutral, and release-cadence neutral.
5. Hard dependency only when the artifact’s central promise cannot exist without it.

---

## 6. Package dependency rules

### 6.1 Core package independence

A core runtime package must not declare another Sperk’s Forge runtime package as a dependency unless SFGSS-000 or an accepted ADR explicitly reclassifies that artifact.

Foundation, Expansion, and Advanced package specifications may describe optional integrations, but their core manifests and runtime assemblies remain peer-independent.

### 6.2 Allowed hard dependencies

A hard dependency is allowed only when all are true:

- The package cannot satisfy its one-sentence contract without it.
- The dependency is named in the package specification.
- The manifest and assembly table expose it.
- Clean removal and upgrade behavior are documented.
- Licensing and platform impact are understood.
- The dependency is exercised by clean-project tests.

### 6.3 No transitive wishful thinking

A package must declare every direct package it relies upon. It must not assume a transitive dependency will always remain present because another package currently brings it into the project.

### 6.4 No circular package graph

Circular UPM package dependencies are prohibited. When two packages appear to require one another:

1. Identify the actual authority.
2. Move translation into a bridge.
3. Move game-specific behavior into project code.
4. Consider a tiny neutral contract only if the SFGSS-000 shared-contract test is satisfied.

### 6.5 No dependency by scene accident

A package is not allowed to “depend” on another package merely because a sample scene contains both. The dependency must exist in the manifest/assembly graph or remain a documented sample/integration prerequisite.

### 6.6 No dependency by static discovery

A core package must not rely on another package being discoverable through a static singleton, Resources lookup, scene-name convention, tag, layer, serialized project reference, or reflection scan unless an approved integration artifact owns that behavior.

---

## 7. Package manifest rules

### 7.1 Manifest truth

Every distributable UPM package has one root `package.json`. The manifest must accurately describe the package name, version, Unity floor, required dependencies, and public metadata.

Unity’s package manifest accepts specific SemVer dependency values rather than range syntax. Therefore:

- `dependencies` records a concrete package version string.
- A broader compatible/tested range belongs in package documentation and the suite compatibility catalog.
- The release pipeline must test the exact declared versions and any additional versions publicly claimed as compatible.

### 7.2 Required manifest fields

At minimum:

```json
{
  "name": "com.echodevgames.echo-example",
  "version": "0.1.0",
  "displayName": "Verse Title — Plain Responsibility",
  "description": "Clear technical package description.",
  "unity": "6000.0",
  "author": {
    "name": "Jesse Adams / EchoDevGames"
  }
}
```

License and documentation fields follow SFGSS-009 and package release policy.

### 7.3 Dependency declarations

```json
{
  "dependencies": {
    "com.unity.inputsystem": "<verified-version>",
    "com.echodevgames.echo-a": "<tested-version>",
    "com.echodevgames.echo-b": "<tested-version>"
  }
}
```

Rules:

- A core package lists only hard dependencies.
- A two-package bridge lists both peers.
- A provider adapter lists the neutral core and provider package/SDK.
- The Workshop lists only its own genuine Editor dependencies; selected generated packages belong to the consuming project plan, not The Workshop’s own manifest.
- Samples do not justify silently adding an otherwise unnecessary hard dependency.

### 7.4 Git and local development references

The integration workspace may use local paths, embedded packages, Git references, or submodules for development. Release manifests must not expose a developer-machine path.

### 7.5 Package lock and reproducibility

The consuming project’s `packages-lock.json` is project-owned evidence of resolved versions. A package repository does not treat one consuming project’s lock file as a substitute for its own manifest and compatibility documentation.

### 7.6 Manifest change classification

- Adding a required dependency is at least a minor release before 1.0 and normally a major compatibility change after 1.0.
- Removing a dependency may still be breaking if public types disappear.
- Changing a peer/provider version requires compatibility tests and changelog notes.
- Moving optional behavior into a separate bridge is a migration event and must include guidance.

---

## 8. Assembly architecture

### 8.1 Default package assembly set

A typical runtime package begins with:

```text
EchoDevGames.<Package>.Runtime
EchoDevGames.<Package>.Editor
EchoDevGames.<Package>.Tests.Runtime
EchoDevGames.<Package>.Tests.Editor
```

A package adds more assemblies only for a documented reason:

- Optional presentation technology.
- Optional provider/backend.
- Platform-specific code.
- A separately removable feature.
- A pure C# layer that materially improves portability/testing.
- A sample or Integration Lab.
- Compilation isolation supported by measured evidence later.

### 8.2 Avoid assembly confetti

Do not split every folder into an assembly. Each split adds reference management, compilation overhead, testing combinations, and versioning surfaces.

### 8.3 Runtime core

The main runtime assembly owns the package’s neutral public contracts and central behavior. It must not reference:

- `UnityEditor`.
- Test assemblies.
- Samples.
- The Workshop.
- Optional peers.
- Optional providers/backends.
- Project assemblies.

### 8.4 Editor assembly

The Editor assembly may reference the package Runtime assembly and approved Editor APIs. It must be limited to the Editor platform and must not be required by Player builds.

Setup, validation, migration, inspectors, asset creation, and the SFGSS-ADR-001 setup facade live here or in another package-owned Editor assembly.

### 8.5 Presentation/backend assembly

A dependency that is not central to the runtime contract belongs in a separate assembly when practical.

Examples:

```text
EchoDevGames.EchoDiagnostics.Presentation.UGUI
EchoDevGames.EchoLaunch.Presentation.UGUI
EchoDevGames.EchoCamera.Cinemachine
EchoDevGames.EchoSave.<Provider>
```

The neutral Runtime assembly exposes the presenter/provider contract. The specific assembly implements it.

### 8.6 No runtime-to-Editor path

No chain of runtime assembly references may reach an Editor assembly. An indirect path is as invalid as a direct reference.

---

## 9. Assembly reference direction

### 9.1 Canonical direction

```text
Unity / approved hard dependency
             ↓
Package Runtime Core
             ↓
Package Editor / Presentation / Backend / Sample / Tests

Peer A Runtime ← Separate Bridge Runtime → Peer B Runtime
Peer A Editor  ← Separate Bridge Editor  → Peer B Editor (only when needed)
```

The arrows indicate “may reference.” Core packages do not point back toward bridges.

### 9.2 Reference matrix

| From | May reference | Must not reference |
|---|---|---|
| Core Runtime | Unity required APIs, own lower-level runtime assemblies, approved hard dependency | Peer cores, bridge, Editor, tests, samples, project code |
| Optional Presentation Runtime | Own Runtime, presentation backend dependencies | Editor, unrelated peers unless it is explicitly a bridge |
| Package Editor | Own Runtime, own presentation/backend, UnityEditor, approved Editor dependencies | Test assembly, project code, Workshop unless separately classified |
| Bridge Runtime | Both peer Runtime assemblies, declared provider-neutral contracts | Peer Editor assemblies, competing authority |
| Bridge Editor | Bridge Runtime, peer Editor assemblies only when required | Player compilation |
| Provider Adapter Runtime | Neutral core, provider runtime SDK | Provider Editor APIs, unrelated peers |
| Project Adapter | Project assemblies and installed peer Runtime assemblies | Package source ownership claims |
| Runtime Tests | Runtime assemblies under test, Test Framework | Editor assembly unless test is Editor-only |
| Editor Tests | Runtime + Editor assemblies under test, Test Framework | Production Player build |
| Sample assembly | Package Runtime and declared sample dependencies | Package Editor in runtime sample code |

### 9.3 GUID references

EchoDevGames assembly definitions should serialize references to other committed EchoDevGames `.asmdef` assets by GUID when practical. This preserves references across assembly asset renames or moves, provided `.meta` files and GUIDs are preserved.

Documentation tables still list human-readable assembly names.

### 9.4 Assembly Definition References

`.asmref` may be used to place scripts in another folder into an existing assembly within the same owned package/repository.

`.asmref` must not be used to:

- Hide a cross-package dependency.
- Merge project scripts into package assemblies.
- Merge optional integration code into a core assembly.
- Evade the bridge classification rules.

---

## 10. Assembly Definition property policy

### 10.1 `autoReferenced`

Default policy:

| Assembly class | Default |
|---|---:|
| Primary public Runtime | `true` |
| Public project-facing presentation Runtime | `true` when direct project use is intended; otherwise `false` |
| Internal support Runtime | `false` |
| Editor | `false` |
| Bridge Runtime | `false` unless its public API is intentionally used by project scripts |
| Provider Adapter | `false` unless intentionally public |
| Tests | `false` |
| Samples/Labs | `false` |

Rationale: the public Runtime remains easy to use from predefined project assemblies. Optional, Editor, test, and sample assemblies do not create invisible project-wide compile coupling.

A package specification may approve a different value, but it must explain the user path and removal impact.

### 10.2 `includePlatforms` and `excludePlatforms`

- Editor assemblies use `includePlatforms: ["Editor"]`.
- Platform-specific assemblies name only supported targets.
- Do not use platform exclusion as a substitute for a provider abstraction.
- Platform claims remain pending until tested.

### 10.3 `overrideReferences`

Leave disabled unless the assembly consumes precompiled plugin DLLs and explicit control is useful. When enabled:

- List only required precompiled assemblies.
- Validate every supported platform.
- Record the reason in the package specification.
- Do not assume a DLL available in the Editor is available in every Player target.

### 10.4 `allowUnsafeCode`

Default `false`. Enabling unsafe code requires:

- An accepted ADR or package decision.
- Security and platform review.
- Test coverage.
- A clear reason the behavior cannot be isolated in a provider adapter.

### 10.5 `noEngineReferences`

May be `true` for a pure C# assembly that genuinely uses no UnityEngine or UnityEditor APIs. Do not create a pure assembly solely for aesthetic architecture.

### 10.6 `defineConstraints`

Use only to exclude an assembly under a real supported condition. Constraints must be documented and tested in both included and excluded states.

### 10.7 `versionDefines`

Version defines may express API differences for a declared dependency or Unity package. They do not make a direct assembly dependency optional by magic.

### 10.8 Assembly names and namespaces

Assembly and namespace names follow Section 25. Renaming a public assembly is a breaking change unless a migration/shim strategy is approved.

---

## 11. Optional integration classification

Use this decision path:

1. **Is the behavior unique to one game?** Use a project adapter.
2. **Does it directly reference two independent Echo packages?** Use a separate bridge package by default.
3. **Does it connect to a vendor/platform/backend SDK?** Use a provider adapter.
4. **Is it tiny, owner-specific, license-neutral, release-cadence neutral, and completely isolated from core compilation?** An owner-contained integration may be approved.
5. **Would removing it leave either core unable to compile?** The classification is wrong unless it is an explicitly approved hard dependency.
6. **Does it create a second authority?** Reject the design.

### 11.1 Bridge placement record

Every integration specification records:

- Connected authorities.
- Package/repository owner.
- Runtime and Editor assemblies.
- Manifest dependencies.
- Initialization and teardown owner.
- Data/events translated.
- Failure behavior.
- Removal order.
- Tested compatibility versions.
- Integration Lab.

---

## 12. Bridge package standard

### 12.1 Purpose

A bridge connects two independently useful packages. It translates between their public contracts while preserving their authority boundaries.

### 12.2 Required package shape

```text
Packages/com.echodevgames.<a>-<b>/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Runtime/
│   └── EchoDevGames.<A>.<B>.Runtime.asmdef
├── Editor/                         # only when needed
│   └── EchoDevGames.<A>.<B>.Editor.asmdef
├── Documentation~/
├── Samples~/Integration Labs/
└── Tests/
```

### 12.3 Manifest

The bridge declares concrete dependencies on both peers. It does not rely on The Workshop or a project preset to make those dependencies “probably present.”

### 12.4 Runtime behavior

A bridge must:

- Detect both valid authorities.
- Attach idempotently.
- Translate through public contracts.
- Dispose registrations, leases, event subscriptions, and callbacks cleanly.
- Reattach safely after supported lifecycle changes.
- Report unavailable/version-mismatch states without breaking either core.

A bridge must not:

- Own duplicate persistent roots for peer concerns.
- Directly mutate peer internals.
- Require a particular scene name.
- Become the only way either peer works.
- Persist the same data twice under competing owners.

### 12.5 Bridge initialization

Default bridge initialization is explicit registration by a serialized integration component, package-owned bootstrap hook approved by the integration specification, or First Light startup step.

Broad runtime assembly scans are prohibited.

### 12.6 Bridge teardown

The bridge owns its translation state and tears it down before either peer. Teardown is safe if called repeatedly or after one peer has already become unavailable.

### 12.7 Bridge diagnostics

The bridge has its own globally unique diagnostic prefix. It does not reuse either peer’s codes for bridge-owned failures.

### 12.8 Bridge release readiness

A bridge is not advertised as supported until its integration specification and Integration Lab pass.

---

## 13. Owner-contained integration standard

An integration may ship inside the owner package only when all conditions are true:

- The integration extends behavior owned by that package.
- It is small enough to maintain with the owner’s release cadence.
- It introduces no provider SDK, separate license, or material platform restriction.
- It is isolated in a separate assembly and folder.
- The core Runtime and Editor assemblies compile when the integration is excluded.
- Package removal remains clean.
- The package specification explicitly approves the placement.

If the integration directly requires another optional Echo package, a separate bridge is still the default. “Owner-contained” is an exception, not a loophole.

### 13.1 Compile-isolation rule

The integration assembly may use version defines/constraints only when its dependency declaration and missing-package behavior are correct. If users who do not need the integration inherit the peer package anyway, the integration is not genuinely optional.

---

## 14. Provider adapter standard

### 14.1 Separation

The provider-neutral core owns contracts and provider-independent behavior. The adapter owns SDK-specific translation, lifecycle, errors, capabilities, and version compatibility.

### 14.2 Packaging

Provider adapters are separate packages when they introduce:

- A vendor SDK.
- Network/service credentials or setup.
- Platform restrictions.
- Independent licensing.
- Independent upgrade cadence.
- Native plugins.
- Significant build size.

### 14.3 Naming

Preferred package form:

```text
com.echodevgames.<family>.<provider>
```

Examples remain provisional until provider approval:

```text
com.echodevgames.echo-save.<provider>
com.echodevgames.echo-multiplayer.<provider>
com.echodevgames.echo-camera.cinemachine
```

### 14.4 Provider capability truth

The adapter must expose unavailable/unsupported capability states. It must not pretend every provider offers identical guarantees.

### 14.5 Provider removal

Removing the provider adapter returns the neutral core to its documented no-provider state. Durable local data must not become unreadable merely because a cloud/platform adapter was removed, unless the integration specification explicitly documents a provider-owned encrypted/remote-only format.

---

## 15. Project adapter standard

Project adapters are the correct home for game-specific translation such as:

- Mapping Rescuers2D role changes to package requests.
- Applying Hackulos-specific class restrictions.
- Translating a project’s mission result into progression/objective requests.
- Resolving project-owned databases, animation controllers, scenes, prefabs, or narrative state.

### 15.1 Location

Suggested project structure:

```text
Assets/<Game>/Runtime/Integrations/
Assets/<Game>/Editor/Integrations/
Assets/<Game>/Tests/Integrations/
```

Project adapters live in project-owned assemblies and may reference installed packages. Packages never reference the project adapter.

### 15.2 Promotion

A project adapter becomes a reusable bridge candidate only after at least two independent projects demonstrate the same neutral translation and the ownership remains clear.

---

## 16. Workshop Editor setup facades

SFGSS-ADR-001 remains the authority for package setup facades.

Dependency rules:

- The package owns its facade in its Editor assembly.
- The facade does not reference The Workshop.
- The Workshop does not compile-reference every peer Editor assembly.
- The Workshop invokes only exact allowlisted assembly-qualified types from reviewed adapter descriptors.
- A missing, incompatible, or unsupported facade produces a visible manual setup path.
- Facade requests/results remain JSON envelopes at the protocol boundary.
- Setup facades do not create runtime package dependencies.

The setup facade is not a general plugin system and does not authorize broad assembly discovery.

---

## 17. Compile guards and version defines

### 17.1 Approved uses

Compile guards may be used for:

- Unity version/API differences.
- Platform-specific implementation assemblies.
- A declared package/SDK version difference.
- Development/test-only instrumentation.
- Excluding an owner-contained integration assembly when its approved dependency is absent.

### 17.2 Prohibited uses

Compile guards must not:

- Hide an undeclared direct dependency.
- Make core source reference a missing peer type.
- Replace a separate bridge that should declare both peers.
- Create materially different ownership behavior without documentation.
- Require consumers to manually add a mysterious scripting define to enable basic package behavior.

### 17.3 Symbol naming

Package-owned symbols use uppercase technical IDs:

```text
ECHODEVGAMES_ECHOLAUNCH
ECHODEVGAMES_ECHODIAGNOSTICS
ECHODEVGAMES_JUKEBOT
```

Provider/bridge symbols include the full family/provider or peer pair. Symbols are documented in the relevant specification.

### 17.4 Both-path validation

Every conditional assembly/code path requires tests with the condition present and absent.

---

## 18. Reflection and dynamic discovery

### 18.1 Default rule

Reflection is not the default integration mechanism.

### 18.2 Allowed reflection

Reflection is allowed only when:

- The target is an exact allowlisted type/member.
- The owner cannot take a compile-time dependency by design.
- The contract is versioned.
- Failure is bounded, visible, and non-destructive.
- The result is cached rather than repeatedly scanned.
- IL2CPP/linker implications are documented for runtime use.

SFGSS-ADR-001 is the approved Foundation example: The Workshop calls exact package-owned Editor facades without scanning arbitrary assemblies.

### 18.3 Prohibited reflection

- Broad `AppDomain` scans for “anything implementing interface X” across all project assemblies.
- Convention-only discovery of peer packages.
- Private-field access into another package.
- Runtime service location by type name.
- Reflection used to avoid publishing a real public contract.

### 18.4 Runtime versus Editor

Runtime reflection carries build, stripping, performance, and platform costs. Editor-only exact reflection remains preferable when the integration problem is strictly authoring/setup.

---

## 19. Samples, laboratories, and showcase dependencies

### 19.1 Standalone Lab

A Standalone Test Lab may depend only on:

- The package under test.
- Its declared hard Unity/platform dependencies.
- Redistributable sample assets.
- Sample-local test/readout code.

It must not depend on another Echo package.

### 19.2 Integration Lab

An Integration Lab belongs to the bridge/provider artifact and declares all peers/providers explicitly. It does not count as standalone proof for either peer.

### 19.3 Showcase

A showcase may combine many packages. It remains presentation evidence only.

### 19.4 Optional sample technologies

A sample-only UI/backend dependency must not silently become a core hard dependency. Choose one:

1. Use technology already required by the package.
2. Isolate the sample behind a correctly conditional sample assembly and clear prerequisite.
3. Ship a separate sample companion/integration package.
4. Use a dependency-free minimal test surface.

Imported samples must not leave compile errors when their documented prerequisites are absent.

### 19.5 Sample assemblies

Sample assemblies are `autoReferenced: false`, reference only documented dependencies, and remain removable.

---

## 20. Test assembly standard

### 20.1 Required separation

```text
Tests/Runtime/EchoDevGames.<Package>.Tests.Runtime.asmdef
Tests/Editor/EchoDevGames.<Package>.Tests.Editor.asmdef
```

Bridge/provider packages use equivalent package-qualified names.

### 20.2 Runtime tests

Runtime tests reference the Runtime assemblies under test and the Unity Test Framework. They do not reference the package Editor assembly.

### 20.3 Editor tests

Editor tests may reference Runtime and Editor assemblies under test.

### 20.4 Test visibility

Production APIs should be tested through public behavior where practical. `InternalsVisibleTo` may expose internals only to exact package-owned test assemblies when:

- The internal seam is important to verify.
- Making it public would weaken the API.
- The friend assembly list is documented.

Do not grant internal access to project code, bridges, samples, or unrelated packages.

### 20.5 Tests never become Player dependencies

Test assemblies remain non-auto-referenced and excluded from normal Player builds.

### 20.6 Required dependency tests

Every package/bridge/provider implementation eventually tests:

- Declared dependencies present at approved versions.
- Missing optional bridge/provider.
- Removal and re-add.
- Upgrade from previous supported version.
- No circular assembly reference.
- No runtime reference to Editor.
- Standalone compilation without unrelated peers.
- Conditional assembly included and excluded.
- Sample removal.

Observed results remain `Not run` until implementation exists.

---

## 21. Platform and backend assemblies

### 21.1 Platform split

Platform-specific code belongs in a separate assembly when it directly uses platform-only APIs or native plugins.

```text
EchoDevGames.<Package>.Windows
EchoDevGames.<Package>.WebGL
EchoDevGames.<Package>.Mobile
```

Do not create these assemblies before a real implementation need exists.

### 21.2 Backend split

Backend-specific assemblies use names that expose the dependency:

```text
EchoDevGames.EchoCamera.Cinemachine
EchoDevGames.EchoSave.FileSystem
EchoDevGames.EchoLocalization.<Backend>
```

The package specification decides whether one backend is the approved built-in default or a separate adapter.

### 21.3 Capability surface

Provider/backend contracts expose capabilities and unsupported states. The core must not call provider-only methods through downcasts or reflection.

### 21.4 Native plugins

Native plugin import settings, CPU architecture, platform compatibility, licensing, and removal are part of the provider adapter release gate.

---

## 22. Lifecycle, registration, and teardown

### 22.1 Registration ownership

The artifact that creates a registration owns its handle and disposes it.

Examples:

- An Observatory bridge owns its diagnostic provider registration.
- An Accord consumer bridge owns its settings-applier registration.
- A Pulse/Will bridge owns the input-policy lease/registration it creates.
- A Chronicle participant adapter owns participant registration.

### 22.2 Idempotency

Attach, detach, setup, validation, and repair paths are safe to repeat. Duplicate integration components must not register twice without detection.

### 22.3 Authority availability

A bridge may wait for peers through explicit readiness events or a bounded initialization sequence. It must not poll every frame forever or create a missing peer authority unless its approved contract specifically owns that creation path.

### 22.4 First Light

First Light startup bridges coordinate initialization. They do not transfer ownership into EchoLaunch. A peer remains usable without First Light and may already exist; the bridge adopts a valid existing authority.

### 22.5 Direct-scene development

A package’s direct-scene initializer creates only that package’s minimum missing authority. It does not install or spawn unrelated peers.

### 22.6 Teardown order

Default order:

1. Stop accepting new bridge/provider requests.
2. Dispose integration registrations and leases.
3. Unsubscribe callbacks.
4. Release adapter/provider resources.
5. Allow peer authorities to shut down independently.

---

## 23. Failure and compatibility behavior

### 23.1 Missing optional artifact

Core package behavior remains available. The optional feature reports unavailable through its own setup/diagnostics surface.

### 23.2 Missing required dependency

A distributable package with a missing hard dependency is an installation/configuration failure and must not be represented as a supported standalone state.

### 23.3 Version mismatch

A bridge/provider detects incompatible versions through declared package metadata, adapter descriptors, schema/protocol versions, or explicit runtime compatibility checks. It must:

- Refuse unsafe attachment.
- Preserve both cores.
- Emit a stable diagnostic.
- Give actionable upgrade/downgrade guidance.

### 23.4 Partial initialization

If one peer initializes and the other fails, the bridge remains detached or rolls back its own partial work. It does not roll back or destroy a valid peer authority it does not own.

### 23.5 Optional feature downgrade

When an optional integration is absent, behavior returns to the documented standalone path, not a half-configured shadow mode.

### 23.6 Data preservation

Removing optional settings consumers, save participants, providers, or bridges must preserve unknown durable records when their owning data standard requires preservation. SFGSS-003 defines the exact rules.

---

## 24. Clean removal and replacement

### 24.1 Required removal order

For a separate bridge or provider:

1. Export/record any project-specific configuration guidance.
2. Remove or disable the bridge/provider.
3. Verify both peers/neutral core compile and run standalone.
4. Remove a peer only after no installed artifact depends on it.
5. Remove generated project adapters/assets only through project-owned guidance.

The Workshop may perform compatible package operations together, but its plan must still show this dependency order.

### 24.2 Bridge-first rule

A peer package must not be removed while a bridge that manifest-depends upon it remains installed, unless both are removed in one approved atomic package operation.

### 24.3 Project assets

Removing a package must not automatically delete project-owned configuration, save files, generated scenes, prefabs, input assets, or migrated content. Removal guidance classifies them as package-owned, generated-project-owned, adopted, modified, unknown, or manual.

### 24.4 Replacement

Replacing a project system with a package follows preserve-until-parity:

- Keep the original.
- Prove the package standalone.
- Add the project adapter/bridge.
- Verify parity.
- Remove old code only after rollback is understood.

### 24.5 Orphan detection

Validation should identify:

- Installed bridge with missing peer.
- Peer removal blocked by dependent bridge/provider.
- Project asmdef referencing a removed assembly.
- Serialized components whose scripts belong to a removed package.
- Durable unknown data retained intentionally.

---

## 25. Naming registry

### 25.1 Core packages

| Surface | Pattern | Example |
|---|---|---|
| Package ID | `com.echodevgames.<kebab-name>` | `com.echodevgames.echo-scene-flow` |
| Runtime assembly | `EchoDevGames.<Package>.Runtime` | `EchoDevGames.EchoSceneFlow.Runtime` |
| Editor assembly | `EchoDevGames.<Package>.Editor` | `EchoDevGames.EchoSceneFlow.Editor` |
| Runtime tests | `EchoDevGames.<Package>.Tests.Runtime` | `EchoDevGames.EchoSceneFlow.Tests.Runtime` |
| Editor tests | `EchoDevGames.<Package>.Tests.Editor` | `EchoDevGames.EchoSceneFlow.Tests.Editor` |
| Namespace | `EchoDevGames.<Package>` | `EchoDevGames.EchoSceneFlow` |

### 25.2 Presentation/backend assemblies

```text
EchoDevGames.<Package>.Presentation.<Technology>
EchoDevGames.<Package>.<Backend>
EchoDevGames.<Package>.<Platform>
```

### 25.3 Bridge packages

Preferred package ID:

```text
com.echodevgames.<package-a>-<package-b>
```

Preferred assembly/namespace:

```text
EchoDevGames.<PackageA>.<PackageB>.Runtime
EchoDevGames.<PackageA>.<PackageB>
```

The integration specification fixes ordering and owner naming. It must not create two differently ordered packages for the same bridge.

### 25.4 Provider adapters

```text
com.echodevgames.<family>.<provider>
EchoDevGames.<Family>.<Provider>
```

### 25.5 Samples and Labs

```text
EchoDevGames.<Package>.Samples.<LabName>
EchoDevGames.<PackageA>.<PackageB>.Samples.<LabName>
```

### 25.6 Diagnostic prefixes

Bridge/provider diagnostic prefixes are globally unique and registered through SFGSS-008. Do not concatenate ambiguous initials casually.

---

## 26. Documentation and compatibility records

Every package/bridge/provider specification includes:

- Complete dependency table.
- Manifest dependency list.
- Assembly table with reference direction and Auto Referenced policy.
- Optional integration classification.
- Removal behavior.
- Supported/tested version table.
- Known platform/provider restrictions.
- Integration Lab requirements.
- Upgrade/migration notes.

### 26.1 Compatibility catalog

The central suite repository records:

- Package versions.
- Exact versions tested together.
- Declared manifest versions.
- Supported Unity versions backed by evidence.
- Bridge/provider protocol versions.
- Known incompatibilities.
- Pending/not-run combinations.

A compatibility claim is evidence, not optimism.

### 26.2 Dependency diagram

Every bridge/integration specification includes a compact dependency diagram. Core package specifications include one when the assembly graph contains more than the default Runtime/Editor/tests set.

### 26.3 Changelog

Dependency, assembly, provider, package ID, namespace, or removal changes appear in the changelog and migration guide.

---

## 27. Validation and release gates

### 27.1 Static validation

Before release, validation confirms:

- Package manifest parses and names exact required dependencies.
- No circular UPM dependency.
- No circular assembly reference.
- Runtime cannot reach `UnityEditor`.
- Core Runtime does not reference optional peers.
- Bridge/provider manifests include every direct dependency.
- Editor/test/sample assemblies use correct platforms and auto-reference policy.
- `.meta` files and asmdef GUIDs are present and stable.
- Conditional assemblies have documented symbols/version defines.
- Assembly names and namespaces match SFGSS-008.

### 27.2 Clean-project matrix

Observed later during implementation:

| Scenario | Required result |
|---|---|
| Core installed alone | Compiles and Standalone Lab runs |
| Core + unrelated Echo package | No behavior change or hidden coupling |
| Both peers without bridge | Both operate independently |
| Bridge + both peers | Integration works |
| Bridge removed | Both peers return to standalone behavior |
| Provider removed | Neutral core exposes unavailable/no-provider state |
| Sample removed | Runtime remains intact |
| Editor assembly excluded from Player | Player build compiles |
| Conditional dependency absent | Core and unrelated assemblies compile |
| Package upgrade | Manifest/asmdef/public API migration behaves as documented |

### 27.3 Documentation gate

An artifact cannot enter implementation until its specification names its dependency class, assembly graph, removal behavior, and test plan.

### 27.4 Release gate

A bridge/provider cannot be release-ready without:

- Its own specification.
- Clean install/removal tests.
- Integration Lab.
- Compatibility evidence.
- License/notices.
- Failure and teardown diagnostics.

---

## 28. Foundation application matrix

This table applies the standard to the ten approved Foundation packages without changing their ownership contracts.

| Package | Core hard Echo dependencies | Approved direct Unity/package dependency intent | Optional split/bridge direction |
|---|---|---|---|
| First Light | None | Unity core/scene APIs; presentation dependency to be isolated | Startup bridges depend on First Light + peer; uGUI presenter should be separate from neutral Runtime |
| Observatory | None | Unity profiling/core; isolated uGUI/TMP presentation | One provider bridge per peer or small reviewed bridge group |
| Accord | None | Unity core/filesystem/display adapters | Consumer bridges apply committed settings to peers |
| Passage | None | Unity scene-management core | Launch, UI, Pulse, Chronicle, multiplayer bridges remain separate |
| Pulse | None | Unity core/time/cursor adapters | Input, audio, UI, scene bridges remain separate |
| Resonance | None | Unity audio core; project-owned mixer/assets | Accord/Pulse/UI/First Light bridges separate |
| Will | None | Unity Input System is a real hard platform dependency | UI/Accord/Pulse bridges separate |
| Looking Glass | None | uGUI is central to first backend; TMP exact version verified later | Domain presenters and peer bridges remain separate/project-owned |
| Chronicle | None | Unity core/filesystem; default serializer | UI/Passage/Pulse participant bridges; cloud/platform providers separate |
| Workshop | No runtime package dependencies; Editor composer behavior governed by descriptors | Unity Editor, Package Manager APIs, UI Toolkit Editor | Invokes package-owned facades through ADR-001; generated selections are not Workshop manifest dependencies |

### 28.1 SUITE-DOC-30 Foundation assembly resolution

The consistency review resolved the Foundation assembly advisories:

- First Light’s neutral Runtime assembly is uGUI-free; the default presenter moves to `EchoDevGames.EchoLaunch.Presentation.UGUI`.
- Foundation Editor assemblies use `autoReferenced: false`.
- Samples and Laboratories keep optional presentation dependencies outside core runtime assemblies.
- Public primary Runtime assemblies remain `autoReferenced: true` for ordinary project usability.
- Bridge/provider package IDs and exact assembly ordering remain owned by their integration/provider specifications.
- Exact Unity package versions remain evidence-pending until implementation validation.

---

## 29. Reconciliation history

### 29.1 SUITE-DOC-03 input

SFGSS-003 must align unknown-data preservation, provider removal, settings records, save participant payloads, stable IDs, aliases, and migration ownership with the clean-removal rules here.

### 29.2 SUITE-DOC-04 input

SFGSS-004 must formalize the dependency test matrix, Integration Lab evidence, missing-peer states, conditional-assembly tests, clean removal, and compatibility evidence states.

### 29.3 SUITE-DOC-08 input

SFGSS-009 must define release tags, exact package version publication, compatibility-catalog updates, local path development, Git references, tarballs, and multi-repository bridge/provider releases.

### 29.4 SUITE-DOC-10 consistency review

The standards consistency review must reconcile:

- SFGSS-001 dependency and assembly tables.
- All ten Foundation package assembly tables.
- First Light presentation isolation.
- Editor/sample Auto Referenced defaults.
- Package/bridge/provider naming in SFGSS-008.
- Compatibility claims in SFGSS-009.

No package code is authorized by this queue.

---

## 30. Approval

### 30.1 Approval checklist

- [x] Dependency classes are explicit.
- [x] Core package independence is preserved.
- [x] Hard dependency approval is constrained.
- [x] Package manifest rules are defined.
- [x] Runtime, Editor, presentation, backend, bridge, provider, sample, and test assembly direction is defined.
- [x] Assembly Definition defaults are defined.
- [x] Compile guards and reflection are bounded.
- [x] Workshop setup facades remain governed by ADR-001.
- [x] Standalone and Integration Lab dependency rules are distinct.
- [x] Clean removal and bridge-first teardown are defined.
- [x] Foundation specifications have an explicit reconciliation queue.
- [x] No implementation evidence has been invented.
- [x] Package implementation remains locked until SUITE-DOC-33 activates the program; each later package remains locally locked until its just-in-time learning review passes.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Checkpoint:** SUITE-DOC-02

---

## External technical basis

This standard was checked against Unity 6 documentation describing:

- Package manifests, exact SemVer dependency values, and the lack of range syntax in package dependencies.
- Official package layout conventions for Runtime, Editor, Tests, Samples~, and Documentation~.
- Assembly Definition references, Auto Referenced behavior, circular-reference restrictions, GUID references, platform inclusion, and precompiled-reference overrides.
- Version defines and define constraints.
- Test assembly recognition and isolation.

Official references:

- https://docs.unity3d.com/6000.0/Documentation/Manual/upm-manifestPkg.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/cus-layout.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/assembly-definitions-referencing.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/assembly-definition-file-format.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/assembly-definitions-creating.html


---


## 31. SUITE-DOC-30 Consistency Resolution

SFGSS-002 was checked against all twenty-eight package foundations and the three approved integration matrices.

- No circular core dependency was found.
- No package gained a hidden peer dependency.
- Editor, test, sample, internal, bridge, and provider assembly defaults are consistent.
- First Light’s default uGUI presenter is isolated from the neutral Runtime assembly.
- Workshop invokes exact package-owned Editor facades through ADR-001 rather than open reflection discovery.
- Git-distributed peer packages remain visible project-level selections under SFGSS-009.

All compile, removal, installation, provider, and compatibility evidence remains `Not run`.

## Graph Navigation

#sfgss/authority #sfgss/navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
