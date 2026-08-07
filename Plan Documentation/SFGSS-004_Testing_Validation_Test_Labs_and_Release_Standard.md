# The Sperk’s Forge – Testing, Validation, Test Labs, and Release Standard

**Document ID:** SFGSS-004  
**Version:** 1.3.0
**Status:** Approved architecture and quality standard  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000 v0.24.0
**Related authorities:** SFGSS-001, SFGSS-002, SFGSS-003, SFGSS-005, SFGSS-008, SFGSS-ADR-001, SFGSS-ADR-002, SFGSS-ADR-005
**Current development baseline:** Unity 6000.3.8f1  
**Minimum planned public Unity floor:** Unity 6000.0  
**Last updated:** August 7, 2026

> A blueprint may predict where the bridge should stand. Only evidence proves that it holds weight.

> **v1.1.0 naming reconciliation:** EchoSave test examples now use the package-approved `ESV` prefix, and SFGSS-008 is registered as the canonical suite naming authority. No test behavior or release gate changed.

---

## Contents

1. Purpose and authority
2. Scope and non-goals
3. Terminology
4. Governing principles
5. Evidence states and honesty rules
6. Claims, confidence, and compatibility language
7. Test identifiers and registry structure
8. Test taxonomy and required layers
9. Static, documentation, and package validation
10. EditMode and pure-policy testing
11. PlayMode and runtime behavior testing
12. Editor tooling, setup, repair, and migration testing
13. Standalone Laboratory standard
14. Integration Laboratory standard
15. Showcase and sample verification
16. Clean-project installation and package-route proof
17. Upgrade, migration, removal, and reinstall proof
18. Lifecycle, direct-scene, and domain-reload proof
19. Negative, failure, recovery, and resilience testing
20. Performance, allocation, stress, and capacity evidence
21. Platform, build, device, and provider compatibility
22. Accessibility, usability, privacy, and security review
23. Test data, fixtures, determinism, and reproducibility
24. Defect classification and issue handling
25. Flaky tests, quarantine, retries, and exclusions
26. Test reports, evidence storage, and traceability
27. Release stages and quality gates
28. Automation, continuous integration, and local execution
29. Foundation application matrix and reconciliation queue
30. Approval

---

## 1. Purpose and authority

SFGSS-004 defines the canonical testing, validation, laboratory, evidence, defect, compatibility, and release-quality rules for **The Sperk’s Forge – EchoDevGames Game Systems Suite**.

SFGSS-000 establishes that every package must be independently installable, diagnosable, testable, removable, and honestly documented. SFGSS-001 requires each package specification to define a test strategy and measurable release gates. SFGSS-005 defines how one implementation checkpoint is executed and closed. This standard supplies the shared evidence system that makes those promises comparable across packages.

This document answers questions that otherwise become slippery during implementation:

- What is the difference between a planned test and a passed test?
- Which behaviors belong in EditMode, PlayMode, an Editor Laboratory, a Standalone Laboratory, or an Integration Laboratory?
- What evidence is required before a package may claim support for an installation route, Unity version, platform, device family, provider, bridge, or migration path?
- When is a warning only advisory, and when does it block release?
- How are failures, flaky tests, retries, exclusions, and known limitations recorded without sanding away inconvenient evidence?
- What must pass before a package moves from internal development to beta, release candidate, or stable release?

### 1.1 Authority order

When testing or release documents disagree, use this order:

1. SFGSS-000 suite boundaries and approved ownership.
2. The active approved package specification.
3. SFGSS-002 for dependency, assembly, sample, provider, and removal boundaries.
4. SFGSS-003 for data, identity, serialization, migration, transaction, and recovery behavior.
5. This standard.
6. Accepted ADRs, bridge specifications, provider specifications, and research protocols.
7. SFGSS-005 checkpoint workflow.
8. Checkpoint plans, test plans, test reports, issue records, release records, and Current Notes.

A lower document may make a test more specific. It must not silently weaken the package specification, reclassify a failed release requirement as optional, or claim support without evidence.

### 1.2 Requirement language

- **Must** means release-blocking.
- **Must not** means prohibited unless a higher authority or accepted ADR grants a named exception.
- **Should** means the default choice; deviation requires a written reason.
- **May** means optional.

---

## 2. Scope and non-goals

### 2.1 In scope

This standard governs:

- Planned and executed test records.
- Evidence states and compatibility language.
- Static validation, EditMode, PlayMode, Editor, Laboratory, installation, migration, platform, performance, and release testing.
- Standalone, Integration, and Showcase evidence boundaries.
- Test IDs, issue IDs, validation IDs, and traceability.
- Clean-project, local, embedded, Git, and tarball package proof.
- Repeatability, non-destructive setup, repair, migration, removal, and reinstall proof.
- Defect severity, flaky-test handling, approved exclusions, and release blockers.
- Test reports, environment capture, artifacts, and evidence retention.
- Package beta, release-candidate, and stable quality gates.

### 2.2 Not in scope

This standard does not define:

- An individual package’s runtime behavior or public API.
- Exact test-framework package versions before they are verified in implementation.
- One mandatory continuous-integration vendor.
- A replacement for Unity’s Console, Profiler, Test Runner, build system, or platform certification.
- Fabricated compile, performance, migration, device, provider, or platform results before implementation exists.
- External storefront or console certification policy.
- The repository tagging and artifact-publishing process beyond quality-facing requirements, which belongs to SFGSS-009.

---

## 3. Terminology

| Term | Meaning |
|---|---|
| **Requirement** | A behavior, constraint, or quality promise owned by an approved authority. |
| **Test case** | A stable, repeatable setup, action, and expected result that verifies one or more requirements. |
| **Test execution** | One recorded attempt to run a test case in a named environment. |
| **Evidence** | The retained observation or artifact supporting an execution result. |
| **Validation check** | A deterministic inspection of configuration, files, references, package state, or project state. |
| **Validator severity** | The seriousness of a detected configuration condition, not the severity of a code defect. |
| **Defect** | Observed behavior that violates an approved requirement or documented expectation. |
| **Issue record** | A durable record that tracks a defect, investigation, documentation problem, risk, or architecture conflict. |
| **Standalone Laboratory** | The isolated user-visible proof of one package or independently selectable feature with only declared hard dependencies. |
| **Editor Laboratory** | An Editor-only interactive proof for a package whose central behavior is not a runtime scene. |
| **Integration Laboratory** | A separate proof owned by a bridge/provider artifact and containing every declared peer/provider dependency. |
| **Showcase** | A combined presentation scene or project that demonstrates composition after standalone and integration evidence exists. |
| **Clean project** | A disposable Unity project created from a known template with no unrelated Sperk’s Forge package or project code. |
| **Installation route** | Embedded, local path, Git URL, tarball, registry, or Workshop-driven installation path. |
| **Compatibility claim** | A statement that a package works with a specific Unity version, platform, dependency version, provider, device family, or project context. |
| **Release gate** | A mandatory evidence set that must pass before a package enters a named release stage. |
| **Flaky test** | A test whose result varies without an approved input or environment change. |
| **Quarantine** | A temporary state that isolates a known unreliable automated test while preserving its failure visibility and issue ownership. |
| **Approved exclusion** | A documented scope decision that a test or platform does not apply to a release. It is not a hidden skip. |
| **Advisory** | A non-blocking finding that remains visible and has an owner or documented acceptance. |

---

## 4. Governing principles

### 4.1 Planned tests are not evidence

A specification may approve a test registry before code exists. Every such row remains **Not run** until execution produces evidence.

### 4.2 Claims cannot outrun evidence

A package may be designed for a Unity version, platform, device family, provider, installation route, or migration path. It may claim that target as **Supported** only after the required evidence exists.

### 4.3 Independence is tested continuously

Standalone behavior, missing optional peers, sample removal, bridge removal, duplicate authorities, direct-scene entry, and clean installation are checkpoint responsibilities, not release-week chores.

### 4.4 One failure is still evidence

A failed test is not erased by a later pass. The report records every relevant attempt, the fix or changed environment, and the final disposition.

### 4.5 Automated and manual evidence complement each other

Pure policies, invariants, lifecycle edges, migrations, and repeatable failure conditions should be automated. Visual quality, physical devices, interaction feel, accessibility, platform behavior, and guided setup often require manual evidence. Neither category is inherently second-class.

### 4.6 Laboratories prove contracts, not spectacle

A Laboratory exists to make state, actions, and failures observable. It may be visually polished, but decoration must not conceal the setup or become a runtime dependency.

### 4.7 Release evidence is reproducible

A test report names the exact code/package version, Unity version, dependency versions, platform, project template, settings, and fixtures needed to interpret the result.

### 4.8 Failures stop expansion

When a checkpoint-owned test fails, new feature work stops until the failure is reproduced, classified, resolved or explicitly deferred outside the checkpoint, and the full checkpoint acceptance set is rerun.

### 4.9 Evidence is preserved, not curated into fiction

Logs may be summarized, sensitive data may be redacted, and obsolete temporary artifacts may be pruned under policy. The project must not delete the only evidence of a failure merely because it complicates the release story.

### 4.10 Quality gates are scoped but real

A release is judged against its approved MVP and advertised compatibility, not every deferred feature. Within that scope, required gates are not optional.

---

## 5. Evidence states and honesty rules

### 5.1 Canonical execution states

Every durable test execution uses exactly one state:

| State | Meaning | Release treatment |
|---|---|---|
| **Not run** | The test is defined but has not been executed in the named environment. | No evidence; cannot support a claim. |
| **Pass** | The observed result matches the expected result with retained evidence. | Supports the exact tested claim. |
| **Pass with advisory** | The core expected result passed, but a non-blocking limitation or concern is recorded. | Supports the claim only with the advisory visible. |
| **Fail** | The expected result was not met. | Blocks the affected requirement/claim until resolved or removed from scope by higher-authority revision. |
| **Blocked** | A named prerequisite prevented execution. | Does not support the claim; blocker owner and next action required. |
| **Not applicable** | An approved authority establishes that the test does not apply to this artifact/release. | Requires reason and approving document; never used merely because execution is inconvenient. |

`Skipped`, `Ignored`, `Probably passes`, `Expected failure`, and blank status cells are not acceptable durable release states. Framework-specific skip results must be translated into **Blocked** or **Not applicable** with a reason.

### 5.2 Planned versus observed truth

Documentation before implementation may truthfully contain:

- Test IDs.
- Fixtures and expected results.
- Automation candidates.
- Performance measurement plans.
- Compatibility targets.
- Release gates.
- `Not run` reports and templates.

It must not claim:

- Successful compilation.
- A passing test run.
- Measured frame time, memory, allocations, latency, throughput, or file size.
- Verified Unity/package/platform/device compatibility.
- Successful migration, upgrade, downgrade, recovery, or external installation.
- Release readiness.

### 5.3 Evidence references

A `Pass`, `Pass with advisory`, `Fail`, or `Blocked` result must reference evidence such as:

- Automated test report and test name.
- Unity Console capture or exported log.
- Validator report.
- Package Manager state.
- Build output and player log.
- Laboratory checklist with observed result.
- Generated diagnostic/support report.
- Migration or setup receipt.
- Reproduction record.
- Screenshot or short recording where visual proof is necessary.
- Commit hash, tag, or package artifact checksum when available.

A screenshot alone is insufficient for logic that can be verified through structured output. A log alone is insufficient for a visual, input-device, or accessibility claim that requires human observation.

### 5.4 Pass with advisory

An advisory must state:

- What passed.
- What limitation was observed.
- Why the limitation does not violate the current release requirement.
- The issue/risk/known-limitation reference.
- Whether the advisory narrows compatibility or support language.

### 5.5 Blocked executions

A blocked test records:

- The missing prerequisite.
- Whether the prerequisite is project-owned, package-owned, provider-owned, or environment-owned.
- The owner responsible for unblocking it.
- The next planned execution point.

A package cannot enter stable release with a blocked required test.

---

## 6. Claims, confidence, and compatibility language

### 6.1 Canonical claim states

Compatibility and support tables use these states:

| Claim state | Meaning |
|---|---|
| **Unknown** | No design decision or evidence exists. |
| **Planned** | The architecture targets the environment, but execution evidence does not yet exist. |
| **Tested** | The named version/environment passed the required matrix. |
| **Supported** | The project commits to maintaining the named scope, backed by tested evidence and documented limitations. |
| **Experimental** | Evidence exists, but compatibility or API stability is intentionally not guaranteed. |
| **Unsupported** | The environment is intentionally outside scope or has confirmed incompatible behavior. |

`Yes`, `Should work`, `Compatible`, `Probably`, and `Where Unity supports it` must not appear as the only status in durable compatibility tables.

### 6.2 Exactness rule

A claim must name the tested dimensions that matter:

- Unity Editor version.
- Package version and dependency versions.
- Operating system.
- Build target and scripting backend when relevant.
- Rendering pipeline when relevant.
- Input device family or model category when relevant.
- Provider/SDK version when relevant.
- Enter Play Mode and domain-reload configuration when relevant.

Testing Unity 6000.3.8f1 does not automatically prove every Unity 6 release. Testing Windows Editor does not prove a Windows Player, WebGL, mobile, or console target.

### 6.3 Broader support than test coverage

A package may support a documented range broader than the exact matrix only when:

- SFGSS-009 defines the support policy.
- The public API uses supported contracts across that range.
- Representative boundary versions are tested.
- Known version-specific differences are documented.
- A user can report a failure inside the claimed support range and receive normal support treatment.

### 6.4 Documentation-only status

Before implementation, every compatibility target defaults to **Planned** or **Unknown**. It cannot be marked **Tested** or **Supported** based on design confidence alone.

---

## 7. Test identifiers and registry structure

### 7.1 Stable test IDs

Durable package tests use:

```text
<PACKAGE-PREFIX>-T-###
```

Examples:

```text
ELAUNCH-T-001
JB-T-010
EIN-T-045
ESV-T-100
```

Laboratory actions use:

```text
<PACKAGE-PREFIX>-LAB-###
```

Bridge/provider tests qualify the artifact:

```text
<BRIDGE-PREFIX>-T-###
<PROVIDER-PREFIX>-LAB-###
```

Bare `LAB-001`, `T-001`, or `TEST-001` may appear in a local view only when the containing report automatically supplies the package qualification. The canonical registry and cross-package reports must use the fully qualified ID.

### 7.2 IDs are never recycled

When a test is removed or superseded:

- Preserve the ID in history.
- Mark it Deprecated, Superseded, Removed, or Not applicable with reason.
- Link the replacement test when one exists.
- Never assign the old ID to a different requirement.

### 7.3 Minimum registry fields

Each test case records:

| Field | Required content |
|---|---|
| Test ID | Stable package/bridge/provider-qualified ID |
| Requirement references | Capability, decision, diagnostic, risk, API, or release-gate IDs |
| Title | Short behavior-oriented name |
| Layer | Static, EditMode, PlayMode, Editor, Lab, install, migration, performance, platform, etc. |
| Preconditions | Required packages, data, scene, settings, and environment |
| Setup | Exact fixture and starting state |
| Action | One reproducible operation or sequence |
| Expected result | Observable success condition and forbidden side effects |
| Automation class | Automated, manual, hybrid, research/prototype |
| Required stage | Checkpoint, beta, release candidate, stable, provider approval, etc. |
| Current state | Canonical execution state |
| Evidence reference | Report/log/artifact path or pending marker |
| Issue reference | Required when failed, blocked, advisory, flaky, or excluded |

### 7.4 Execution records are separate from test definitions

One stable test case may have many executions across Unity versions, platforms, commits, and package versions. Do not overwrite historical executions with the latest result.

### 7.5 Traceability

Every advertised MVP capability and release gate must map to at least one test. Every test must map to at least one approved requirement or risk. Orphan tests and untested release requirements are validation findings.

---

## 8. Test taxonomy and required layers

### 8.1 Canonical layers

| Layer | Primary purpose | Typical owner | MVP requirement |
|---|---|---|---:|
| Static/documentation validation | Manifest, assemblies, references, IDs, docs, licenses, package anatomy | Editor/test tooling | Yes |
| EditMode/pure-policy | Deterministic rules, validation, algorithms, DTOs, migrations | Package tests | Yes where applicable |
| PlayMode/runtime | Lifecycle, authority, state, events, Unity object behavior | Package tests | Yes for runtime packages |
| Editor integration | Setup, repair, generation, migration, inspectors, package operations | Editor package tests | Yes when Editor tooling ships |
| Standalone Laboratory | User-visible isolated MVP proof | Core package/sample | Yes when behavior is user-observable |
| Integration Laboratory | One explicit bridge/provider composition | Bridge/provider artifact | When integration is advertised |
| Showcase | Combined presentation and portfolio proof | Integration workspace/sample | Optional |
| Clean-project installation | Packaging and hidden-dependency proof | Release/integration workspace | Yes |
| Upgrade/migration/recovery | Durable compatibility and failure recovery | Owning package | When durable data or prior versions exist |
| Existing-project adoption/parity | Replacement without regression | Package + target project | Before real-project integration claim |
| Build/platform/device | Player behavior on claimed targets | Release testing | Before platform/device support claim |
| Performance/stress/capacity | Budgets, bounded degradation, leaks | Package/release testing | Before performance claim |
| Accessibility/usability/privacy/security | Human interaction and sensitive-data promises | Package/release testing | When relevant to package surface |
| Research/prototype | Provider comparison or feasibility evidence | Research record | When an approval gate requires it |

### 8.2 Package-specific requirement

A package specification may mark a layer **Not applicable** only with an explanation. Examples:

- An Editor-only composer uses an Editor Laboratory rather than a decorative runtime scene.
- A pure data contract package may use EditMode tests and a small sample project instead of a persistent runtime Laboratory.
- A bridge has no standalone runtime authority and therefore proves itself through an Integration Laboratory.

### 8.3 Vertical-slice rule

The first complete use path must pass in isolation before the package expands into a large catalog. An Integration Laboratory or Showcase cannot retroactively count as standalone proof.

---

## 9. Static, documentation, and package validation

### 9.1 Static validation scope

Static validation should inspect, as applicable:

- `package.json` identity, version, dependencies, samples, and Unity floor.
- Required package files, license, notices, changelog, README, and `Documentation~` routes.
- Runtime/Editor/test/sample assembly boundaries under SFGSS-002.
- Runtime references to `UnityEditor`, tests, samples, project assemblies, or optional peers.
- Missing or duplicate stable IDs under SFGSS-003.
- Missing `.meta` files or changed public GUIDs.
- Samples referenced from runtime assemblies.
- Missing configuration, scenes, routes, assets, exposed mixer parameters, input actions, or provider descriptors.
- Test registry coverage and unresolved release blockers.
- Documentation examples, menu paths, diagnostic code tables, and version references.

### 9.2 Validator severities

Validator result severity is separate from defect severity:

| Severity | Meaning | Typical effect |
|---|---|---|
| **Info** | Context or healthy state. | No action required. |
| **Advisory** | Improvement or limitation that does not violate a release requirement. | Record/consider. |
| **Warning** | Configuration is risky, incomplete, or outside recommended practice. | Release may proceed only if the owning gate permits and limitation is visible. |
| **Error** | A required feature/configuration cannot operate as intended. | Blocks the affected test or feature. |
| **Blocker** | Package cannot safely compile, install, build, run, migrate, or preserve data. | Blocks checkpoint/release. |

### 9.3 Auto-fix rules

A validator may offer an auto-fix only when the operation is:

- Deterministic.
- Non-destructive by default.
- Previewed or exactly reported.
- Repeatable.
- Covered by tests.
- Safe for project-owned assets and Git-tracked files.

Auto-fixes must not silently delete, overwrite, rename, migrate, or reserialize project-owned data.

### 9.4 Validation execution points

Checks may run:

- Manually.
- During setup/repair.
- Before Play Mode.
- Before a build.
- During release preparation.
- In continuous integration.

Expensive checks must not run continuously without user control.

---

## 10. EditMode and pure-policy testing

### 10.1 Preferred scope

EditMode or pure C# tests should verify:

- Validation policies.
- State-transition guards and composers.
- Stable-ID parsing, aliases, collisions, and tombstones.
- Queue, concurrency, ordering, selection, and replacement rules.
- DTO serialization boundaries and migrations.
- Hash/fingerprint canonicalization.
- Transaction planning and rollback decisions.
- Configuration defaults and range clamping.
- Diagnostic-code mapping.
- Path and manifest planning without touching live project content.

### 10.2 Determinism

Tests involving randomness, time, file paths, devices, providers, or environment must use injected clocks, seeded random sources, temporary directories, fakes, or controlled adapters when practical.

### 10.3 Unity object use

EditMode tests may use Unity objects when required, but pure policies should remain testable without scene loading or frame timing. Do not force every rule through a `MonoBehaviour` merely because the package runs in Unity.

### 10.4 Durable-data fixtures

Migration and serialization tests must preserve immutable source fixtures and compare the resulting staged data against expected canonical output. A test must not rewrite the only copy of a historical fixture.

---

## 11. PlayMode and runtime behavior testing

### 11.1 Required runtime concerns

Runtime packages test, as applicable:

- Authority claim before side effects.
- Duplicate roots present before Play Mode and introduced during scene load.
- Standalone initialization without First Light.
- First Light bridge initialization when advertised.
- Direct-scene development initialization.
- Scene transitions and persistent-root survival.
- Events raised after authoritative state changes.
- Listener exceptions and safe unsubscription.
- Cancellation, timeout, replacement, queueing, and repeated requests.
- Shutdown, quit, scene destruction, reset, and reinitialization.
- Missing/invalid/empty configuration.
- Optional peers absent, present, disabled, and removed.
- Bounded handles, leases, queues, pools, histories, and capacity.
- Shared ScriptableObject immutability.

### 11.2 Timing and frame assumptions

Tests must state whether they rely on:

- Scaled or unscaled time.
- FixedUpdate, Update, LateUpdate, coroutines, tasks, or Unity Awaitable behavior.
- Scene activation callbacks.
- Audio DSP time.
- Input-event timing.
- Editor-only frame behavior.

A test that passes only because of unspecified frame ordering is incomplete.

### 11.3 Player-build proof

Editor PlayMode evidence does not automatically prove Player behavior. Features that depend on filesystem paths, build scenes, platform APIs, stripping, IL2CPP, WebGL, mobile lifecycle, device access, or provider SDKs require Player-build evidence before support is claimed.

---

## 12. Editor tooling, setup, repair, and migration testing

### 12.1 Required Editor operations

Every shipped setup, generation, repair, migration, validation, or removal tool tests:

- Clean first run.
- Preview/dry run.
- Apply.
- Exact report/receipt.
- Second and third repeat runs.
- Existing matching content.
- Existing modified/adopted content.
- Naming/path conflict.
- Missing optional package/provider.
- Domain reload or Editor restart during a resumable operation.
- Cancellation before publication.
- Failure after partial staging.
- Safe rollback or recovery.
- No runtime dependency on Editor code.

### 12.2 Repeatability

Repeat-running an operation must not:

- Duplicate roots, scenes, assets, settings, registrations, packages, or build entries.
- Replace project-authored content silently.
- Regenerate stable IDs unnecessarily.
- Change output when inputs and tool version are unchanged, except for documented timestamps or non-semantic metadata.

### 12.3 Workshop setup facades

Each package facade governed by SFGSS-ADR-001 must have:

- Contract/descriptor validation.
- Plan generation tests.
- Plan-hash stability tests.
- Apply and receipt tests.
- Version mismatch behavior.
- Missing facade/manual fallback behavior.
- Domain-reload continuation behavior where supported.
- Removal and project-owned output survival tests.

### 12.4 Undo and backup honesty

A tool must state whether it supports Unity Undo, file backup, Git rollback, transaction rollback, or only a generated report. Tests must verify the real mechanism and must not call an operation “undoable” merely because the user can restore a Git commit.

---

## 13. Standalone Laboratory standard

### 13.1 Purpose

A Standalone Laboratory proves the package’s central promise without unrelated Sperk’s Forge packages or project code.

### 13.2 Allowed dependencies

A Standalone Laboratory may depend only on:

- The package under test.
- Its declared hard Unity/platform dependencies.
- Its own sample/test assembly.
- Redistributable sample assets.
- Small test-only adapters that do not recreate another package authority.

### 13.3 Required contents

A runtime Standalone Laboratory includes, as relevant:

- Visible instructions and controls.
- Current authority identity and initialization state.
- Current configuration source.
- Success, empty, unavailable, invalid, warning, and failure states.
- Duplicate-authority demonstration.
- Direct-scene entry demonstration.
- Reset/repeat control.
- Bounded diagnostic history.
- No project-owned or restricted content.

An Editor Laboratory includes equivalent instructions, state readout, dry-run/apply/reset workflows, disposable fixtures, and failure simulations.

### 13.4 Lab actions and test cases

Laboratory actions are human-readable demonstrations. They may map to one or more automated tests, but a Laboratory checklist does not replace the underlying test registry.

### 13.5 Reset contract

Reset must return the Laboratory to a known state without requiring manual hierarchy surgery, deleting package source, or restarting Unity unless the test specifically covers restart behavior.

### 13.6 Release evidence

The Laboratory must be imported into a clean project and executed from its documented entry point. Merely opening the scene in the package-development project is insufficient for final package proof.

---

## 14. Integration Laboratory standard

### 14.1 Ownership

The bridge or provider-adapter artifact owns the Integration Laboratory. Neither peer’s Standalone Laboratory becomes responsible for the other package.

### 14.2 Required proof

An Integration Laboratory verifies:

- Every declared package/provider dependency is present at an exact tested version.
- Each peer works alone before the bridge is enabled.
- Registration occurs once and in the documented direction.
- Initialization-order differences fail safely.
- Data/events/requests translate correctly.
- The bridge owns and releases its subscriptions, leases, handles, adapters, and generated resources.
- Disabling/removing the bridge returns both peers to standalone behavior.
- Removing a peer while the bridge remains produces a clear dependency failure rather than hidden partial operation.
- Version mismatch and missing provider behavior are actionable.

### 14.3 Integration does not prove standalone quality

A combined scene cannot satisfy a peer package’s standalone gate. Integration evidence begins only after each peer’s relevant standalone evidence passes.

### 14.4 Game-specific adapters

Project-local adapters are tested in the consuming project and may use the Integration Laboratory pattern. They do not create a reusable-package support claim unless promoted into a distributed bridge.

---

## 15. Showcase and sample verification

### 15.1 Showcase role

A Showcase demonstrates composition, visual polish, portfolio value, and realistic workflow. It is optional and never substitutes for Standalone or Integration Laboratories.

### 15.2 Sample import rules

Every sample tests that:

- Import does not modify immutable package source.
- Required dependencies are declared and explained.
- Assets have redistribution rights.
- Instructions match the current package version.
- Sample assembly references do not leak into runtime assemblies.
- Deleting the sample leaves the package compiling and functional.
- Reimport does not duplicate project-owned data unexpectedly.

### 15.3 Sample state

Samples may contain safe placeholder definitions and configuration. They must not be mistaken for production project data, migration fixtures, or package-owned mutable state.

### 15.4 Controller and feature isolation

Every independently selectable controller preset or feature receives its own Laboratory when SFGSS-000 or the package specification requires it. A large omnibus sample does not prove each module independently.

---

## 16. Clean-project installation and package-route proof

### 16.1 Clean-project baseline

A clean-project report records:

- Unity Editor version.
- Project template/render pipeline.
- Operating system.
- Target platform.
- Package source/artifact and checksum when available.
- Dependency versions resolved by Unity.
- Whether the Package Manager lockfile already existed.
- Enter Play Mode options when relevant.

### 16.2 Installation routes

The package specification identifies which routes it advertises:

| Route | Purpose | Minimum proof |
|---|---|---|
| Embedded | Package development inside a Unity project | Compiles, tests discover, Editor/runtime boundaries hold |
| Local path | Workspace development and integration | Add/remove/re-add, compile, tests, sample import |
| Git URL | Consumer source install | Clean install, dependency resolution, sample import, removal |
| Tarball | Release-candidate artifact proof | External clean install, checksum, quick start/Lab, removal/reinstall |
| Registry | Later distribution path if adopted | Exact registry/version resolution and same release matrix |
| Workshop | Guided composition | Exact plan, resolved package graph, facade execution, generated report |

A route not tested remains **Planned** or **Unknown**, not supported.

### 16.3 Hidden-dependency proof

The clean project must not contain unrelated Echo packages, old project assemblies, cached generated assets, manually copied DLLs, or scene objects that make the package appear healthier than the artifact actually is.

### 16.4 Compile is necessary but insufficient

A successful import must be followed by the package’s smallest documented functional proof, validator pass, or Laboratory workflow.

### 16.5 Package artifact integrity

The release report should record artifact size and checksum. SFGSS-009 owns final release publishing and catalog policy.

---

## 17. Upgrade, migration, removal, and reinstall proof

### 17.1 Upgrade matrix

Before a package claims upgrade support, test:

- Previous supported version to candidate version.
- Current candidate reinstall over itself where applicable.
- Project-owned configuration/assets preserved.
- Stable public GUIDs preserved or intentionally migrated.
- Durable documents migrated under SFGSS-003.
- Setup/repair tools do not duplicate generated output.
- Deprecated APIs/assets produce documented guidance.

### 17.2 Migration fixtures

Historical fixtures are immutable, version-labeled, and representative of released formats. Fabricated future-version fixtures may test rejection behavior but cannot prove compatibility with an unreleased format.

### 17.3 Downgrade

Downgrade support is not assumed. If unsupported, the package must preserve newer data, fail safely, and explain recovery. A downgrade test verifies the refusal/preservation behavior rather than pretending reverse migration exists.

### 17.4 Removal matrix

Test removal in this order:

1. Disable and detach project integration.
2. Remove bridge/provider artifacts before peers.
3. Remove samples and generated disposable content.
4. Remove the package.
5. Compile/build the remaining project.
6. Confirm project-owned configuration, saves, settings, generated receipts, and migration evidence survive unless the user explicitly chose deletion.

### 17.5 Reinstallation

Reinstalling a compatible package must:

- Detect preserved project-owned data.
- Validate and migrate before reclaiming it.
- Avoid duplicating roots/assets/registrations.
- Report unsupported newer or corrupt data without overwriting it.

---

## 18. Lifecycle, direct-scene, and domain-reload proof

### 18.1 Canonical lifecycle matrix

Runtime authorities test:

- Cold start through the canonical Boot path.
- Standalone start without First Light.
- Direct-scene development start.
- Existing authority adoption.
- Duplicate present before Play Mode.
- Duplicate introduced during scene load.
- Additive scene behavior when supported.
- Shutdown and application quit.
- Reinitialize after controlled shutdown when supported.

### 18.2 Enter Play Mode configurations

Where static state, domain reload, or scene reload matters, test the supported combinations explicitly. At minimum, record whether tests were run with:

- Domain reload enabled.
- Domain reload disabled.
- Scene reload enabled/disabled when supported and relevant.

A package may narrow support, but the limitation must be documented.

### 18.3 Static reset

Any static authority access point, registry, cache, event, or test seam must reset predictably across supported Editor configurations and player lifecycle.

### 18.4 Direct-scene helpers

Development helpers test that they:

- Create only their own minimum missing authority.
- Adopt an existing valid authority.
- Reject duplicates before side effects.
- Mark the session as development-initialized.
- Are disabled or explicitly approved for release builds.

---

## 19. Negative, failure, recovery, and resilience testing

### 19.1 Required failure families

As applicable, packages test:

- Missing configuration.
- Invalid values and references.
- Empty catalogs/profiles/registries.
- Unsupported optional peer/provider absent.
- Version mismatch.
- Duplicate IDs and authorities.
- Queue/pool/history/capacity exhaustion.
- Cancellation and timeout.
- Exceptions from user callbacks/listeners/providers.
- Interrupted setup or migration.
- Corrupt, older, and newer durable data.
- Filesystem/path/permission failure.
- Scene/build reference failure.
- Device disconnect or provider unavailability.
- Recovery failure and fallback-loop prevention.

### 19.2 Structured failure behavior

A failure test verifies:

- Stable result/diagnostic code.
- No unauthorized side effect before validation/publication.
- No unhandled exception crossing the public boundary unless explicitly documented.
- Clear user/developer action.
- Preserved evidence and source data.
- Deterministic fallback or safe unavailable state.

### 19.3 Fault injection

Packages should provide test seams or Laboratory controls for approved failures rather than requiring testers to corrupt production assets manually. Fault injection must remain test/development-only and must not ship enabled accidentally.

### 19.4 Recovery evidence

A recovery test must first prove the failure condition, then prove the recovery path and final authoritative state. Merely logging “recovered” is insufficient.

---

## 20. Performance, allocation, stress, and capacity evidence

### 20.1 Pre-code measurement plans

Specifications may define:

- Metrics.
- Scenarios.
- Expected capacity.
- Profiling tools.
- Hardware/build environment fields.
- Pass/fail method.

They must not invent measured values.

### 20.2 Required measurement context

Performance evidence records:

- Package and commit/version.
- Unity version.
- Editor or Player build.
- Development/release build settings.
- Platform, hardware class, and scripting backend when relevant.
- Warmup duration.
- Sample duration/count.
- Input data size and configured capacity.
- Baseline comparison.
- Metric source and unavailable metrics.

### 20.3 Metrics

Relevant metrics may include:

- Initialization time.
- Frame time and worst-percentile spikes.
- Managed allocations and garbage collection.
- Memory footprint.
- Audio voice count.
- Queue/pool/history size.
- File size and save/load duration.
- Scene transition duration.
- Input/rebind latency.
- Validation/generation duration.
- Provider/network latency and throughput when later applicable.

### 20.4 Stress versus support

A stress test explores failure and degradation beyond normal configuration. Passing a stress test does not automatically raise the advertised supported capacity. The specification or release record states the supported and tested limits separately.

### 20.5 Bounded degradation

At capacity, a package must reject, queue, steal, prune, throttle, or become unavailable according to its specification. It must not silently allocate without bound, freeze indefinitely, corrupt data, or create duplicate authorities.

---

## 21. Platform, build, device, and provider compatibility

### 21.1 Compatibility matrix fields

| Field | Example |
|---|---|
| Artifact | Package/bridge/provider version |
| Unity | Exact Editor version |
| Dependency versions | Input System, uGUI, provider SDK, etc. |
| Environment | Editor/Player, OS, build target, scripting backend |
| Hardware/device/provider | Named family/category and connection/context |
| Claim state | Planned/Tested/Supported/Experimental/Unsupported |
| Test set | Required test IDs |
| Result | Canonical execution state |
| Evidence | Report/build/log reference |
| Limitations | Visible conditions and exclusions |

### 21.2 Build validation

Before claiming a platform, verify:

- Player build succeeds.
- Runtime assemblies contain no Editor dependency.
- Required scenes/build profiles are correct.
- Managed stripping does not remove required code.
- File/path/storage behavior matches the platform.
- Input, audio, UI, timing, and lifecycle behavior relevant to the package works.
- Diagnostics avoid sensitive paths/details in release mode.

### 21.3 Physical devices

Simulation may verify policies. Physical device evidence is required for advertised device families where device-specific behavior matters, including controller layouts, disconnect/reconnect, mobile lifecycle, touch, browser focus, or platform services.

### 21.4 Providers

A provider adapter is not approved from documentation or mocks alone when its central promise depends on a live SDK/service. It requires the provider-specific research/prototype/release evidence defined by its specification.

### 21.5 External outages

A service outage may block a provider test. Record **Blocked**, preserve local evidence, and rerun. Do not convert the absence of provider evidence into a pass.

---

## 22. Accessibility, usability, privacy, and security review

### 22.1 Accessibility evidence

Packages with visual, audible, input, timing, or interaction surfaces review, as applicable:

- Keyboard, mouse, and controller navigation.
- Default focus and back/cancel behavior.
- Text scaling and layout resilience.
- Readable contrast and color-independent status.
- Reduced motion, shake, flash, rumble, and timing preferences.
- Subtitle/caption hooks.
- Human-readable glyph/text fallback.
- Error and warning communication without audio/color alone.

Automated checks may find missing labels or configuration. Manual observation remains necessary for interaction and readability claims.

### 22.2 Usability

The five-minute quick start, setup window, Laboratory instructions, repair flow, and error actions should be tested by following the documentation exactly from a clean state. A maintainer’s memory is not an acceptable hidden setup step.

### 22.3 Privacy

Diagnostics and support exports test that they exclude or redact:

- Credentials and tokens.
- Personal account identifiers.
- Typed text or raw key histories.
- Full save payloads unless explicitly requested in a safe support flow.
- Private filesystem/user paths when release-safe mode requires redaction.
- Device serial numbers and unrelated provider data.

### 22.4 Security boundaries

Where external files, network/provider responses, or user-authored data exist, tests cover malformed input, size limits, unsupported versions, path safety, authority validation, and safe rejection. This standard does not replace a dedicated security design for high-risk systems.

---

## 23. Test data, fixtures, determinism, and reproducibility

### 23.1 Fixture ownership

Fixtures are classified as:

- Package-owned test fixture.
- Sample-owned demonstrator data.
- Project-owned integration fixture.
- Historical migration fixture.
- Provider research fixture.
- Generated temporary fixture.

Runtime package code must not depend on test fixtures.

### 23.2 Deterministic inputs

Record or inject:

- Random seed.
- Clock/time source.
- Locale and culture.
- Time zone when relevant.
- File path root.
- Scene/build profile.
- Device/provider simulation state.
- Catalog/profile ordering.

### 23.3 Temporary isolation

Tests that touch files, project settings, package manifests, Build Profiles, scenes, input assets, or generated content must use disposable copies or an explicitly isolated clean project. Cleanup failure is itself a test failure/advisory.

### 23.4 Fixture evolution

When a released data format changes, retain prior fixtures needed by the supported migration window. Do not silently rewrite old expected outputs to match a new implementation without recording the schema/requirement change.

### 23.5 Reproduction packet

A difficult failure should be reducible to a packet containing:

- Exact package/commit/version.
- Minimal project or fixture.
- Environment matrix.
- Test/issue ID.
- Reproduction steps.
- Expected and observed result.
- Logs/reports with sensitive data redacted.

---

## 24. Defect classification and issue handling

### 24.1 Defect severity

| Severity | Meaning | Release effect |
|---|---|---|
| **Blocker** | Prevents install, compile, build, startup, testing, safe migration, or release; or risks unrecoverable data loss/security breach with no safe workaround. | Blocks all affected checkpoints/releases. |
| **Critical** | Crashes, corrupts data, violates authority/duplicate safety, exposes sensitive data, or breaks a central MVP path. | Blocks beta/RC/stable for affected scope. |
| **Major** | Significant advertised behavior fails or produces materially wrong state; workaround may exist. | Blocks stable and usually beta for affected MVP claim. |
| **Minor** | Limited defect with low-impact workaround; core promise remains intact. | May ship only when documented and accepted by release gate. |
| **Advisory** | Improvement, usability concern, documentation gap, or measured limitation that does not violate current requirement. | Does not block by itself; remains visible. |

Severity describes impact. Priority describes when the team intends to fix it. Do not lower severity merely because the schedule is tight.

### 24.2 Issue classes

Issue records distinguish:

- Implementation defect.
- Setup/tooling defect.
- Test defect.
- Documentation defect.
- Compatibility defect.
- Migration/data defect.
- Performance defect.
- Accessibility/usability issue.
- Privacy/security issue.
- Architecture conflict.
- External provider/platform blocker.

### 24.3 Minimum issue fields

- Stable issue ID.
- Title and class.
- Severity and priority.
- Affected versions/environments.
- Requirement and test references.
- Reproduction steps.
- Expected and observed result.
- Evidence.
- Workaround, if any.
- Owner and status.
- Resolution and regression tests.

### 24.4 Architecture conflicts

If a test failure reveals that the specification itself is wrong or contradictory:

1. Stop the affected implementation.
2. Record the failure and conflict.
3. Update the owning specification, SFGSS standard, integration record, or ADR.
4. Approve the documentation change.
5. Revise tests and implementation.
6. Preserve the original failure evidence and supersession link.

---

## 25. Flaky tests, quarantine, retries, and exclusions

### 25.1 Flaky is not pass

A test that sometimes fails under unchanged approved inputs is defective. Its latest green run does not erase the flakiness.

### 25.2 Retry policy

Retries may gather evidence about intermittency. They must not automatically convert an initial failure into a pass. Reports show every attempt and final classification.

### 25.3 Quarantine requirements

A quarantined test has:

- Stable test and issue IDs.
- Named owner.
- Reason and first observed date.
- Affected environments.
- Maximum quarantine review date or milestone.
- Replacement coverage, if any.
- Visible exclusion from release pass counts.

A required test in quarantine blocks stable release unless a higher authority removes or replaces the underlying requirement.

### 25.4 Approved exclusions

An exclusion must state:

- Exact environment/feature excluded.
- Reason.
- Approving specification/ADR/release record.
- User-visible limitation.
- Revisit trigger.

Wildcards such as “skip on CI,” “skip on mobile,” or “ignore when flaky” are prohibited without precise scope.

### 25.5 Expected failures

A test may intentionally verify safe failure behavior. Its expected result is the structured rejection/recovery and the test passes when that behavior occurs. Do not mark it as an expected failing test in the release report.

---

## 26. Test reports, evidence storage, and traceability

### 26.1 Report location

Repository planning/integration records live under:

```text
Plan Documentation/Test Reports/
```

Package repositories should mirror an understandable structure such as:

```text
Documentation~/Developer/Tests/
Tests/
TestReports~/           optional ignored/generated output
```

Generated raw output may remain outside Git when large or machine-specific, but the durable summary and evidence reference must be committed.

### 26.2 Test report header

Every durable report records:

- Report ID and title.
- Package/artifact and version.
- Commit/tag/artifact checksum when available.
- Date and executor.
- Unity version.
- Dependency versions.
- OS/build target/scripting backend.
- Project template and render pipeline when relevant.
- Domain/scene reload settings when relevant.
- Test selection.
- Overall result.
- Known limitations and excluded scope.

### 26.3 Summary fields

| Metric | Required |
|---|---:|
| Total defined | Yes |
| Executed | Yes |
| Pass | Yes |
| Pass with advisory | Yes |
| Fail | Yes |
| Blocked | Yes |
| Not run | Yes |
| Not applicable | Yes |
| Flaky/quarantined | Yes |
| Release blockers | Yes |

### 26.4 Traceability matrix

Release evidence must allow a reviewer to move between:

```text
Requirement -> Test case -> Execution -> Evidence -> Issue -> Fix -> Regression execution -> Release gate
```

### 26.5 Evidence retention

At minimum, retain:

- Release-candidate and stable test summaries.
- Migration and recovery evidence for supported versions.
- Compatibility matrices supporting public claims.
- Blocker/critical failure and resolution records.
- Package artifact checksum and clean-install report.
- External real-project adoption evidence when claimed.

SFGSS-009 defines final retention/tag/release policy.

---

## 27. Release stages and quality gates

### 27.1 Specification approved

Required evidence:

- Ownership, MVP, API, lifecycle, data, failure behavior, Laboratory design, test registry, risks, and release gates approved.
- Every test remains honestly `Not run` unless separate prototype evidence exists.

### 27.2 Internal/alpha implementation

Required evidence:

- Current checkpoint gates pass.
- Core assemblies compile with declared dependencies.
- Critical invariants and lifecycle tests exist.
- Known failures are recorded.
- No claim of consumer readiness.

### 27.3 Beta

A package may enter beta when:

- MVP automated and manual tests pass.
- Standalone Laboratory passes from a clean project.
- Required setup/repair repeatability passes.
- Git/local/tarball routes claimed for beta pass.
- No Blocker or Critical defect remains.
- No Major defect remains inside the advertised MVP unless the beta scope explicitly excludes the feature before release.
- Known limitations, diagnostics, installation, quick start, and troubleshooting are accurate.
- Licenses and third-party notices are complete.
- Upgrade behavior from any previous public beta in the supported window is tested.
- Real-project adoption is not required unless the beta advertises a named
  project, adapter, bridge, or parity claim.

### 27.4 Release candidate

A release candidate additionally requires:

- Version/changelog/release record prepared.
- Full required automated matrix passes from the release commit/artifact.
- External clean-project tarball installation passes.
- Claimed Integration Laboratories pass.
- Claimed platforms/devices/providers pass their matrix.
- Performance/capacity targets have observed evidence.
- Migration/recovery fixtures pass.
- Sample removal and package removal/reinstall pass.
- Documentation examples and menu paths are verified against the candidate.
- No required test is blocked, flaky, or quarantined.

### 27.5 Stable

Stable release requires:

- Release-candidate gate passes without unresolved Blocker, Critical, or Major defects in supported scope.
- Real-project integration/adoption evidence exists for at least one target when the package specification requires it.
- Compatibility/support language matches tested evidence.
- Repository tag/artifact/catalog records are prepared under SFGSS-009.
- Current Notes, issue records, test report, changelog, migration guide, known limitations, licenses, and release record agree.

### 27.6 Experimental features

Experimental capability may ship only when:

- It is clearly labeled.
- Core stable behavior does not depend on it.
- Its failure cannot corrupt stable data or authority.
- Its evidence and limitations are separate from stable claims.
- Removal/disable behavior is tested.

### 27.7 Release gate result

A gate result is one of:

- **Pass**.
- **Pass with advisories**.
- **Fail**.
- **Blocked**.

The gate report lists every unmet requirement. A percentage score cannot override a failed mandatory item.

---

## 28. Automation, continuous integration, and local execution

### 28.1 Automation goals

Automation should make regressions visible across:

- Static/package validation.
- EditMode tests.
- PlayMode tests where supported.
- Build validation.
- Package artifact creation.
- Clean-project import smoke tests.
- Test registry/report generation.

### 28.2 Local-first reproducibility

A developer must be able to run the checkpoint’s required tests locally with documented steps. Continuous integration must not be the only place the suite knows how to validate itself.

### 28.3 CI provider neutrality

The suite does not require one CI vendor. Scripts, commands, inputs, outputs, and environment assumptions should remain documented independently from the chosen service.

### 28.4 Generated reports

Automation may generate machine-readable XML/JSON and human-readable Markdown summaries. The durable report must preserve the canonical evidence states and environment fields defined here.

### 28.5 Secrets and providers

Provider tests requiring credentials use protected environment configuration and redact output. Secrets must not enter fixtures, logs, screenshots, repositories, package artifacts, or support reports.

### 28.6 Automation limitations

If an environment cannot execute a required visual, device, service, or platform test, the report marks it **Blocked** or delegates it to a named manual matrix. It does not silently omit it from totals.

---

## 29. Foundation application matrix and reconciliation queue

### 29.1 Foundation test-shape matrix

| Package | Primary automated emphasis | Laboratory emphasis | Critical release evidence |
|---|---|---|---|
| First Light | Authority claim, ordered steps, failure policy, immutable assets | Boot/splash/status/direct-scene loop | Clean install, duplicate zero-side-effect, destination handoff |
| Observatory | Validators, provider registration, bounded sampling, redaction | Editor validation plus runtime overlay | Failure isolation, privacy-safe snapshot, performance overhead |
| Accord | Draft/commit/preview/rollback, storage/migration | Settings transaction and display rollback | Unknown-data preservation, recovery, platform display behavior |
| Passage | Admission, queueing, cancellation boundaries, recovery | Multi-scene transition workflow | Build-scene validation, activation/recovery, direct-scene behavior |
| Pulse | State rules, scope leases, policy composition | State/override/pause readout | Time/cursor adapter behavior, out-of-order release, static reset |
| Resonance | Transport state, handles, concurrency, selection policies | Audio Laboratory | Mixer routing, voice bounds, DSP/player behavior, asset immutability |
| Will | Context/lock/rebind/data policies | Input Laboratory and physical devices | Source immutability, pairing/reconnect, privacy, real-device matrix |
| Looking Glass | Navigation/modal/focus/operation policies | UI Laboratory | EventSystem/focus, accessibility, sample removal, platform UI behavior |
| Chronicle | Generations, manifests, migrations, recovery | Save Laboratory | Interrupted writes, corruption recovery, unknown payloads, external clean install |
| Workshop | Plan/fingerprint/receipt/facade policy | Editor Laboratory and disposable projects | Reload recovery, conflict safety, repeat runs, generated-project survival after removal |

### 29.2 Reconciliation findings for SUITE-DOC-30

The Foundation specifications remain architecturally compatible, but the standards consistency review must normalize:

1. **Laboratory IDs:** several specifications use bare `LAB-###`; durable registries/reports must use package-qualified IDs.
2. **Automation field:** some registries mix `Yes`, `Manual/CI`, and `Automated/manual` in one column. Separate automation class from execution status.
3. **Compressed registries:** The Will describes test ranges rather than every full row. Implementation must create individual definitions with setup/action/expected/evidence fields.
4. **Compatibility language:** several platform tables use `Yes`, `Planned/supported`, or “where Unity supports it.” Convert to Unknown/Planned/Tested/Supported/Experimental/Unsupported.
5. **Performance claims:** all measured values remain pending; each package must preserve method/environment fields before implementation.
6. **Release gates:** package checklists must distinguish beta, release-candidate, and stable evidence where they currently use one combined distribution gate.
7. **Defect severity:** package-specific “blocker/critical” language must map to the canonical severity table.
8. **Evidence columns:** every implementation registry must add evidence and issue references, even when the pre-code specification currently shows only status.
9. **Editor-only Laboratory naming:** Workshop’s Editor Laboratory remains a valid standalone proof and must not be forced into a runtime scene.
10. **Physical/provider evidence:** Will device claims and future Multiplayer/provider claims require simulation plus real environment evidence where central behavior depends on it.

These are documentation reconciliation items. They do not authorize implementation or invalidate the approved package authorities.

### 29.3 Current implementation evidence boundary

The original SUITE-DOC-30 statement that all Foundation implementation evidence
was `Not run` is historical. First Light now has package-local implementation,
automated tests, manual acceptance, setup/repair/Validator/Direct Scene/
Simulator evidence, and one importable Standalone Laboratory through FL-M5-07.

For First Light, clean-project tarball installation, private tester execution,
Windows player build, performance, historical migration, and real-project
adoption remain `Not run` until their named M6 or later records pass. Every
other package retains its own current evidence state; no First Light result is
promoted to another package.

### 29.4 Release before optional adoption

Under SFGSS-ADR-005, a clean-project package pre-release may precede
existing-project adoption. Adoption evidence remains mandatory before an
adoption/parity claim and when an approved package stable gate explicitly
requires it.

---

## 30. Approval

### 30.1 Approval checklist

- [x] Canonical evidence states are defined.
- [x] Planned truth is separated from observed evidence.
- [x] Compatibility language and exactness rules are defined.
- [x] Test IDs, registries, execution records, and traceability are defined.
- [x] Static, EditMode, PlayMode, Editor, Laboratory, installation, migration, lifecycle, failure, performance, platform, accessibility, and security layers are defined.
- [x] Standalone, Integration, and Showcase boundaries align with SFGSS-000 and SFGSS-002.
- [x] Durable-data and migration testing align with SFGSS-003.
- [x] Defect severity, flaky-test, quarantine, retry, and exclusion policies are defined.
- [x] Test report and evidence requirements are defined.
- [x] Beta, release-candidate, and stable gates are measurable.
- [x] Foundation reconciliation items are recorded for SUITE-DOC-30.
- [x] No implementation evidence has been invented.
- [x] At original approval, package implementation remained locked by SFGSS-ADR-002; subsequent implementation requires checkpoint-local authorization.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions:** Reconcile the Foundation package specifications and SFGSS-001/SFGSS-005 terminology during SUITE-DOC-30. Exact Unity Test Framework/package versions and all execution evidence remain pending until implementation.

---

## Standard completion rule

SFGSS-004 is complete when a fresh collaborator can determine:

1. Which test layers prove a package, bridge, provider, sample, migration, or release claim.
2. Whether a result is planned, passed, advisory, failed, blocked, or not applicable.
3. What evidence supports each public compatibility and release statement.
4. Why a Standalone Laboratory, Integration Laboratory, and Showcase are different evidence.
5. How clean installation, repeatability, upgrade, removal, reinstall, performance, platform, accessibility, privacy, and recovery are proven.
6. How defects, flaky tests, retries, exclusions, and advisories affect release.
7. What must pass before beta, release candidate, and stable release.
8. Which Foundation specification wording still requires later consistency reconciliation.

If a release claim cannot be traced to a requirement, test execution, evidence artifact, and gate decision, the claim remains unproven.


---


## 31. SUITE-DOC-30 Registry and Compatibility Resolution

The consistency review resolved the Foundation-wide test-shape queue without claiming execution:

- Package and Laboratory IDs are package-qualified in durable records.
- Older range summaries remain planning shorthand; implementation registries expand them into individual cases.
- Automation class and execution status are separate fields.
- Evidence and issue references are mandatory in execution records.
- Older platform-table `Yes` values are interpreted as `Planned`, never `Tested` or `Supported`, until retained evidence exists.
- Beta, release-candidate, and stable gates remain distinct even when an older package checklist grouped distribution tasks.
- Workshop’s Editor Laboratory remains valid standalone proof for an Editor-only package.
- Real device/provider/platform evidence is required wherever simulation cannot prove the public claim.

At SUITE-DOC-30, all package tests, Laboratories, compatibility claims, and
release gates remained `Not run`. That sentence is historical. First Light's
current evidence and remaining gaps are recorded in Section 29.3; other
packages require their own retained execution records.

## Graph Navigation

#sfgss/authority #sfgss/navigation

- [[Suite_Graph_Roadmap|Suite Graph Roadmap]]
- [[Full_Suite_Documentation_Program_Roadmap|Documentation Program Roadmap]]
- [[Current Notes]]
