# The Foundry - Build Preparation, Validation, and Release Output Package Specification

**Working document ID:** SFGSS-PKG-ECHOBUILDTOOLS-001  
**Specification version:** 1.0.0  
**Status:** Approved  
**Technical package name:** EchoBuildTools  
**Public title:** The Foundry - Build Preparation, Validation, and Release Output  
**Package ID:** `com.echodevgames.echo-build-tools`  
**Editor namespace:** `EchoDevGames.EchoBuildTools`  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Planned repository:** `EchoDevGames/EchoBuildTools`  
**Current Notes:** `Plan Documentation/Current Notes.md` until the package repository is created, then `Documentation~/Developer/Current Notes.md`  
**Unity baseline:** Unity 6000.3.8f1  
**Minimum supported Unity version:** Unity 6000.0  
**Parent authority:** SFGSS-000 v0.12.0, SFGSS-001 v1.1.0, SFGSS-002 v1.0.0, SFGSS-003 v1.0.0, SFGSS-004 v1.0.0, and SFGSS-005 v1.1.0  
**Last updated:** August 4, 2026

> “Heat the plan, strike only the approved metal, and leave behind a mark that proves what was made.”

> **Approval rule:** This specification is approved as the Level 2 authority for EchoBuildTools. Package implementation remains locked until SUITE-DOC-33 passes.

---

## Revision History

| Version | Date | Status | Summary | Approved by |
|---|---|---|---|---|
| 0.1.0 | 2026-08-04 | Proposed | Initial complete specification derived from SFGSS-000 through SFGSS-005 and the approved Foundation, Impact, Wellspring, and Ascent authorities | Pending |
| 1.0.0 | 2026-08-04 | Approved | Approved Editor-only build recipe, planning, validation, identity, safe output, execution, reporting, artifact, CLI, integration, Laboratory, and release contracts | Jesse “Echo” Adams |

---

## 1. Package Identity and One-Sentence Contract

**Public title:** The Foundry - Build Preparation, Validation, and Release Output  
**Technical identifier:** EchoBuildTools  
**Flavor line:** Prepare the mold, test the alloy, and stamp every artifact with an honest receipt.  
**Plain-language subtitle:** An Editor-only package for repeatable Unity build recipes, preflight validation, version stamping, safe output, Player build execution, artifact manifests, checksums, and release evidence.

**One-sentence ownership contract:**

> EchoBuildTools owns project-authored build recipes, deterministic build planning, preflight validation orchestration, temporary version and platform stamping, safe output-path and cleaning policy, Unity Player build invocation, build receipts, package/license inventories, artifact manifests, checksums, release checklist generation, and build-specific diagnostics; it does not own runtime game flow, Unity’s Build Profile system, gameplay content, source control, CI vendors, external deployment, store submission, signing credentials, legal compliance decisions, or the domain truth validated by another package.

### 1.1 Elevator summary

The Foundry turns “click Build and hope” into a named, reviewable, repeatable Editor workflow. A project-owned `BuildRecipe` points at one explicit Unity 6 Build Profile and adds release intent around it: version identity, release channel, output template, validation policy, clean/incremental policy, required artifact processors, report destinations, and release checklist template. Before anything changes, the package resolves an immutable `BuildPlan` and fingerprint that shows exactly which profile, scenes, defines, version values, output path, validators, processors, and options will be used.

Unity’s Build Profile remains the authority for target platform, effective scene list, profile defines, and platform settings. The Foundry does not clone those settings into a competing framework. It verifies that the selected profile and compiled project state match the recipe, snapshots only the project settings it is approved to touch, applies temporary version/build stamps, invokes Unity’s Player build pipeline, processes the completed output, hashes the final artifacts, publishes a versioned receipt, and restores the project in a `finally`-style recovery path.

The package is Editor-only. It has no runtime root, scene object, save participant, UI runtime, or Player assembly. Package-specific validators and artifact processors connect through explicit bridges or providers. Git metadata, signing, notarization, CI, itch.io, storefronts, and other external actions remain optional providers that consume a successful Foundry artifact rather than contaminating the neutral core.

### 1.2 Why this belongs in The Sperk’s Forge

Build mistakes are expensive precisely because they occur after development feels finished. Projects repeatedly ship the wrong scene list, stale version, development define, missing license, unsafe output path, omitted file, corrupted post-build artifact, or unrecorded platform setting. Manual checklists help, but they drift unless the build itself produces evidence.

| Source project or authority | Existing need or failure pattern | Preserve | Improve |
|---|---|---|---|
| Rescuers2D and game-jam projects | Manual scene lists, platform output folders, passwords/build variants, itch uploads | Fast local builds and obvious output | Named recipes, preflight, safe cleaning, evidence, and no hidden deployment |
| Echo Systems Lab | Portfolio builds and repeatable system demonstrations | Checkpoint and portfolio discipline | Generate factual build receipts and portfolio checklist inputs |
| DeverQuest | Package readiness checks, documentation, credits, tarball testing, and Git awareness | Product-grade validation and reports | Separate build authority from editor productivity and credentials |
| The Workshop | Guided project composition and Build Profile creation | Visible generated setup | Workshop creates Foundry assets through ADR-001; Foundry owns builds afterward |
| SFGSS-002 | Explicit dependencies, providers, bridges, and Editor assemblies | Visible compile direction | No reflection-discovered validators or hidden SDKs |
| SFGSS-003 | Stable IDs, detached DTOs, canonical fingerprints, and recovery | Durable report integrity | Do not use display names or mutable assets as receipt identity |
| SFGSS-004 | Evidence states and release gates | Honest Not run/Pass/Fail records | A build receipt is evidence, not proof of unrelated runtime quality |

### 1.3 Verse identity boundary

| Surface | Flavor allowed? | Rule |
|---|---|---|
| Public title and documentation | Yes | “The Foundry” may lead only beside the build/release responsibility |
| Editor headings and tooltips | Yes | Forge language may decorate Plan, Validate, Build, Process, and Publish labels |
| Laboratory fixtures | Optional | Sample product and recipe names remain removable |
| Editor API/type names | No lore-only names | Use `BuildRecipe`, `BuildPlan`, `BuildReceipt`, and direct technical names |
| Generated project/release content | No required Verse copy | Projects own product names, channels, release notes, icons, and store language |

---

## 2. Problem Statement

### 2.1 Current problem

Common build workflows fail through scattered, invisible state:

- the active Unity Build Profile differs from the profile someone thought was selected;
- global and profile scene lists disagree;
- defines are changed immediately before a build even though recompilation has not occurred;
- version values are edited manually and left behind after a failed attempt;
- one output-clean command targets a parent directory instead of the intended build leaf;
- incremental, clean, development, and release options are remembered rather than recorded;
- package validators exist in separate menu items and are skipped;
- post-build processors mutate an artifact after its checksum was calculated;
- a successful Unity `BuildReport` is treated as proof that release notes, licenses, migrations, or gameplay tests passed;
- CI and local builds use different paths, profiles, and version sources;
- credentials or absolute machine paths leak into logs;
- the output is uploaded without a durable manifest describing what it contains;
- a release cannot be reconstructed because the recipe, profile, packages, scenes, and commit metadata were not recorded.

### 2.2 Evidence from existing work

| Source | Existing pattern or problem | Preserve | Improve |
|---|---|---|---|
| SFGSS-000 | EchoBuildTools owns build profiles, versioning, preflight, output safety, and release validation | One build-time authority | Define exact boundary with Unity Build Profiles and external deployers |
| Unity 6 Build Profiles | Profile assets can own scenes, scripting defines, Player overrides, and platform settings | Use supported Unity authority | Wrap instead of reimplementing |
| Foundation package specs | Each package defines validation and release gates | Package-owned truth | Bridge validators into one Foundry preflight |
| Workshop v1.1.0 | Editor composer uses package-owned setup facades | Explicit setup boundary | Workshop prepares; Foundry builds |
| SFGSS-004 | Test plans and executions are distinct | Evidence discipline | Receipt records build facts without claiming unrun tests passed |

### 2.3 Consequences of doing nothing

- Build configuration remains person-dependent and hard to review.
- CI and local release artifacts diverge.
- Unsafe cleaning can destroy unrelated files.
- Version and define drift enters public builds.
- Package release requirements are skipped because validators are fragmented.
- Failed builds leave the Editor in an unknown state.
- External deployment gains accidental authority over build preparation.
- Support cannot identify which scenes, packages, options, or artifact files were shipped.

---

## 3. Goals, Non-Goals, and Success Measures

### 3.1 Goals

- Provide one Editor-only authority for repeatable Player build preparation and evidence.
- Treat Unity Build Profile assets as the platform configuration authority.
- Resolve an immutable, reviewable plan before mutation.
- Prevent unsafe output deletion and path traversal.
- Run package/project/provider validators through explicit registration.
- Stamp version/build values temporarily and restore reliably.
- Produce factual success, failure, cancellation, processor, checksum, and restoration receipts.
- Support local UI and batch-mode execution from the same recipe model.
- Keep external deployment, signing secrets, source control, and legal judgment outside the core.

### 3.2 Non-goals

- Replacing Unity Build Profiles or the Unity Player build pipeline.
- Building AssetBundles, Addressables content, or custom patch systems unless a provider owns that work.
- Deploying to itch.io, Steam, consoles, mobile stores, cloud hosts, or web servers in the core.
- Storing signing certificates, API keys, passwords, tokens, or platform credentials.
- Owning Git branches, commits, tags, pushes, or repository cleanup.
- Proving gameplay, accessibility, migration, or platform quality merely because a Player build succeeded.
- Automatically incrementing or committing version files in the MVP.
- Running inside a Player build.

### 3.3 User outcomes

| User | Starting condition | Desired outcome |
|---|---|---|
| Novice installer | Package and Unity Build Profile exist | Create a safe recipe, preview it, validate it, build, and find the receipt |
| Programmer | Needs custom validation or output processing | Register a stable-ID provider without editing Foundry core |
| Release manager/solo developer | Preparing beta/RC/release | See exact evidence, blockers, artifact hashes, and checklist state |
| CI maintainer | Unity launched in batch mode on correct target | Run the same recipe and receive deterministic exit/report output |
| Tester/support | Given a build receipt | Identify exact build inputs and compare artifacts without exposing secrets |

### 3.4 Measurable success criteria

- Package installs into a clean supported Unity project with zero compile errors.
- No runtime assembly or Player dependency is created.
- A recipe cannot execute unless its immutable plan and validation report are current.
- Protected/unowned output paths are never deleted.
- Failed/cancelled builds attempt deterministic settings restoration and publish recovery evidence.
- A successful published build contains a receipt, artifact manifest, and checksums after all required processors.
- Local and batch entry points resolve the same plan from the same inputs.
- Optional providers can be removed without breaking core compilation or old receipts.
- Every advertised test remains `Not run` until executed under SFGSS-004.

---

## 4. Users and Primary Use Cases

### 4.1 Intended users

- Solo Unity developers, game-jam teams, package maintainers, release managers, and CI maintainers.
- Programmers authoring build validators, stamp adapters, inventory providers, or artifact processors.
- Testers and support users comparing receipts and validating artifact integrity.

### 4.2 Primary use cases

| ID | Use case | Actor | Preconditions | Expected result | Release phase |
|---|---|---|---|---|---|
| EBUILD-UC-001 | Create Foundry configuration | Installer | Package installed | Project-owned configuration and report folders are created safely | MVP |
| EBUILD-UC-002 | Create a build recipe | Developer | Unity Build Profile asset exists | Recipe binds one explicit profile, identity policy, output policy, and validation profile | MVP |
| EBUILD-UC-003 | Preview a build plan | Developer | Recipe valid | Immutable plan shows target, scenes, defines, identity, output, validators, processors, and warnings | MVP |
| EBUILD-UC-004 | Validate without building | Developer/CI | Recipe selected | Preflight report is produced without modifying project or output | MVP |
| EBUILD-UC-005 | Build from the Foundry window | Developer | Preflight passes | Unity Player build runs and a complete receipt is produced | MVP |
| EBUILD-UC-006 | Build from batch mode | CI/local script | Correct platform/profile active before execution | Named recipe runs with deterministic exit code and report path | MVP |
| EBUILD-UC-007 | Stamp product version temporarily | Build service | Resolved identity valid | Build receives version while project settings restore afterward | MVP |
| EBUILD-UC-008 | Stamp platform build number | Build service | Adapter supports target | Target-specific build number is applied, recorded, and restored | MVP |
| EBUILD-UC-009 | Reject define drift | Validator | Recipe/profile required defines differ from compiled state | Build blocks before BuildPipeline invocation | MVP |
| EBUILD-UC-010 | Validate scene list | Validator | Build Profile resolved | Missing, disabled, duplicate, or invalid scenes produce structured findings | MVP |
| EBUILD-UC-011 | Plan safe output path | Developer/CI | Template and tokens provided | Canonical owned leaf path is produced without traversal or reserved targets | MVP |
| EBUILD-UC-012 | Clean an owned output folder | Build service | Ownership marker matches project and recipe | Only the exact approved leaf is deleted | MVP |
| EBUILD-UC-013 | Reject unsafe clean target | Build service | Path is root, ancestor, protected folder, symlink escape, or unowned nonempty folder | No deletion occurs and a blocker is reported | MVP |
| EBUILD-UC-014 | Create incremental build | Developer | Recipe permits incremental mode | Build options preserve cache and receipt records mode | MVP |
| EBUILD-UC-015 | Create clean build | Developer | Recipe requests clean mode | Unity clean-build option is used and recorded | MVP |
| EBUILD-UC-016 | Generate detailed build receipt | Build service | Build completes or fails | Result, duration, size, warnings, errors, scenes, options, versions, and paths are recorded | MVP |
| EBUILD-UC-017 | Generate artifact manifest and checksums | Build service | Build succeeds and artifact processing completes | Streaming SHA-256 manifest is published after final output mutation | MVP |
| EBUILD-UC-018 | Generate package inventory | Build service | Project package state readable | Resolved package names, versions, and source types are recorded | MVP |
| EBUILD-UC-019 | Generate license/notices report | Build service | Package metadata accessible | Presence and source of license/notice files are reported without legal conclusions | MVP |
| EBUILD-UC-020 | Generate release checklist | Developer | Build receipt available | Generic or project template is populated with evidence links and unresolved items | MVP |
| EBUILD-UC-021 | Validate changelog version | Validator | Changelog configured | Expected release heading is found or a warning/error is reported | MVP |
| EBUILD-UC-022 | Create a release-note stub | Developer | Version identity resolved | Project-owned Markdown stub contains factual build metadata and visible placeholders | MVP |
| EBUILD-UC-023 | Register package validator | Bridge/provider | Foundry installed | Stable validator ID participates in ordered preflight and teardown | MVP |
| EBUILD-UC-024 | Register artifact processor | Bridge/provider | Foundry installed | Required/optional processor runs before hashing with receipt evidence | MVP |
| EBUILD-UC-025 | Isolate optional validator failure | Build service | Provider throws or times out | Finding records provider failure and policy decides block/continue | MVP |
| EBUILD-UC-026 | Recover temporary settings after failed build | Build service | BuildPipeline fails or throws | Captured settings restore in finally and recovery report is written | MVP |
| EBUILD-UC-027 | Detect stale build lock | Developer/CI | Previous session ended unexpectedly | Lock is inspected and explicit recovery is offered; no concurrent build starts | MVP |
| EBUILD-UC-028 | Inspect previous receipts | Tester/release manager | Receipt archive exists | Reports can be filtered, opened, compared, and exported redacted | MVP |
| EBUILD-UC-029 | Redact support report | Support user | Receipt selected | Secrets, absolute paths, usernames, and environment values are excluded or transformed | MVP |
| EBUILD-UC-030 | Repeat the same recipe | Developer/CI | No source/config changes | Equivalent plan fingerprint and explicit new build identity are produced | MVP |
| EBUILD-UC-031 | Use Workshop-generated setup | Workshop bridge | Both packages installed | Package-owned setup facade creates Foundry assets without runtime dependency | Later bridge |
| EBUILD-UC-032 | Validate an Echo package | Package bridge | Peer package installed | Bridge contributes package-specific preflight findings | Later bridge |
| EBUILD-UC-033 | Read optional Git metadata | Git provider | Repository/provider available | Commit/branch/dirty state enriches identity without becoming required | Later provider |
| EBUILD-UC-034 | Deploy to itch.io | Deployment provider | Approved provider installed and credentials configured outside recipe | Provider consumes successful artifact; core never deploys | Later provider |
| EBUILD-UC-035 | Sign/notarize platform output | Platform provider | Approved SDK/toolchain available | Provider performs explicit external step with separate evidence | Later provider |
| EBUILD-UC-036 | Build multiple recipes | Batch orchestrator | Recipes independent | Explicit matrix runs sequentially with one receipt per build | Later |

### 4.3 Explicitly unsupported use cases

- Storing credentials in a recipe or passing secrets through ordinary command-line arguments.
- Using Foundry as a general CI server, source-control client, deployment platform, or store SDK.
- Deleting arbitrary user-selected directories with a “clean” button.
- Switching scripting defines and immediately building without recompilation.
- Treating an active platform profile as a reproducible release recipe when no explicit Build Profile asset is selected.
- Claiming a package/platform is supported merely because a build file was emitted.
- Mutating another package’s project data to “fix” validation without its approved setup/repair facade.

---

## 5. Authority and Ownership Boundaries

### 5.1 The package owns

- Foundry configuration, recipe schema, stable recipe IDs, version identity model, and plan fingerprint.
- Resolution of one recipe into one immutable build plan.
- Preflight validator registration, ordering, execution, timeout, and gate decisions.
- Build-specific output path templating, canonicalization, ownership markers, and clean safety.
- Temporary application/restoration of approved version/platform stamp fields.
- Invocation of Unity Player builds through explicit Build Profile options.
- Foundry build locks, session state, recovery journals, receipts, artifact manifests, checksums, and report retention.
- Package/license inventory reporting and release checklist/stub generation.
- Foundry-specific setup, validation, repair, diagnostics, Laboratory, and command-line behavior.

### 5.2 The package does not own

- Unity Build Profile content, platform module installation, compiler, Player build internals, or Unity Editor lifecycle.
- Runtime startup, scenes, gameplay state, saves, settings, audio, UI, input, progression, content, or game rules.
- Another package’s validation truth or repair operations.
- Source-control metadata or commands unless an optional provider supplies read-only facts.
- CI scheduling, runners, caches, agents, or credential stores.
- External deployment, signing, notarization, store submission, patch delivery, or server hosting.
- Legal determination that licenses/notices are compliant.
- Product marketing copy or release-note narrative.

### 5.3 Neighboring authorities

| Concern | Authoritative owner | How EchoBuildTools interacts |
|---|---|---|
| Platform/scenes/profile defines/Player overrides | Unity Build Profile | References one explicit asset, resolves its effective values, validates, and builds through Unity API |
| Runtime package configuration | Owning Echo package/project | Separate validator bridge reads public Editor validation API |
| Project composition | The Workshop | Workshop creates/repairs Foundry assets through ADR-001; no runtime dependency |
| Save/settings data correctness | Chronicle/Accord | Validator bridges report schema/migration readiness |
| Package tests and release evidence | SFGSS-004 and owning package | Foundry links evidence; it does not fabricate pass results |
| Source control | Git/provider/project workflow | Optional read-only metadata provider; no commits/pushes |
| Deployment/store upload | Provider adapter | Consumes successful artifact and receipt after Foundry completes |
| Signing/notarization credentials | Platform provider/CI secret store | Foundry passes opaque provider references only |
| Build-time diagnostics dashboard | Foundry and optional Observatory bridge | Foundry remains diagnosable alone; bridge may visualize facts |
| Package installation/composition report | Workshop/Package Manager | Foundry inventories resolved state but does not install packages during build |

### 5.4 Boundary tests

1. If a feature changes platform/scenes/defines, should it live in the Unity Build Profile rather than the Foundry recipe?
2. If a rule knows another package’s private data, should it be a bridge validator owned with that package?
3. If a step needs credentials or network transmission, should it be a separate provider after successful artifact publication?
4. If a result claims gameplay quality, where is the SFGSS-004 execution evidence?
5. If an operation deletes or overwrites data, can the exact owned target and rollback class be proven before execution?
6. If a value affects compilation, has the required domain reload already occurred?

---

## 6. Independence Contract

### 6.1 Standalone guarantees

EchoBuildTools must:

- Compile with Unity Editor assemblies and declared Unity dependencies only.
- Contain no runtime assembly and no `MonoBehaviour` root.
- Work without First Light, Observatory, Workshop, Chronicle, Accord, or any peer package.
- Require no Git repository, CI vendor, network access, signing tool, or deployment account.
- Use explicit provider/bridge registration and fail visibly when a recipe requires an absent provider.
- Preserve project-owned Build Profiles, recipes, version manifests, templates, receipts, and outputs when the package is removed.
- Never use samples or Laboratory fixtures as production dependencies.

### 6.2 Independence proof matrix

| Condition | Expected behavior | Test evidence |
|---|---|---|
| Installed alone | Create recipe around a project Unity Build Profile; plan/validate/build/report work | Clean-project Editor tests and Foundry Laboratory |
| No peer validators installed | Core validators run; no missing optional peer assumptions | Provider absence tests |
| No Git repository | Identity remains valid using manifest/manual/environment provider | Identity provider tests |
| No network access | Local build and evidence generation remain functional | Offline build fixture |
| No deployment provider | Artifact remains complete and usable locally | Removal/absence test |
| Package removed | Project-owned recipes/receipts/outputs remain; Editor compiles after references are removed | Removal test |
| Reinstalled | Supported recipe/receipt schemas reopen or migrate | Reinstall/migration test |

### 6.3 Allowed dependencies

| Dependency | Type | Required? | Minimum version | Reason | Removal behavior |
|---|---|---|---|---|---|
| Unity Editor / BuildPipeline / Build Reporting | Platform | Yes | Unity 6000.0 | Player build orchestration and reports | Package cannot operate without Unity Editor |
| Unity Build Profiles API | Platform | Yes | Unity 6000.0 | Explicit project-owned platform configuration | Recipe becomes invalid if profile asset removed |
| UI Toolkit Editor APIs | Platform | Yes | Unity 6000.0 | Foundry window and inspectors | CLI/service remains Editor code |
| System.IO / cryptography | Platform/.NET | Yes | Unity-supported profile | Streaming reports and SHA-256 | Package cannot generate manifests without supported APIs |
| Optional Echo/provider packages | Bridge/provider | No | Specified per bridge | Package-specific validation or external processing | Core remains functional; required recipe provider blocks visibly |

### 6.4 Forbidden dependencies

- Any runtime Echo package in the core Editor assembly.
- Project gameplay assemblies.
- Samples or test fixtures.
- Vendor deployment/signing SDKs in the neutral core.
- Reflection-based discovery of arbitrary validators/processors.
- A hidden dependency on Git, shell tools, network access, or a particular CI vendor.

---

## 7. Capability Scope

### 7.1 Capability matrix

| ID | Capability | Description | Status | MVP? | Runtime/Editor/Sample |
|---|---|---|---|---|---|
| EBUILD-CAP-001 | Editor-only package | No runtime root or Player assembly | Approved | Yes | Editor |
| EBUILD-CAP-002 | Explicit Unity Build Profile binding | Recipe references one project-owned Unity Build Profile asset | Approved | Yes | Editor/Data |
| EBUILD-CAP-003 | Immutable build recipes | Project-owned recipes describe release intent and policies | Approved | Yes | Editor/Data |
| EBUILD-CAP-004 | Build plan preview | Resolved immutable plan before mutation | Approved | Yes | Editor |
| EBUILD-CAP-005 | Canonical plan fingerprint | SHA-256 over normalized plan inputs | Approved | Yes | Editor |
| EBUILD-CAP-006 | Validation orchestration | Ordered package/project/provider validators with stable IDs | Approved | Yes | Editor |
| EBUILD-CAP-007 | Severity and override policy | Blocker/error/warning/info behavior varies by release channel | Approved | Yes | Editor |
| EBUILD-CAP-008 | Scene-list validation | Uses effective Unity Build Profile scene list | Approved | Yes | Editor |
| EBUILD-CAP-009 | Define-state validation | Blocks last-second define mutations and stale compilation | Approved | Yes | Editor |
| EBUILD-CAP-010 | Version identity resolution | Semantic version, channel, build sequence, optional metadata | Approved | Yes | Editor |
| EBUILD-CAP-011 | Temporary PlayerSettings stamping | Apply only approved settings and restore after build | Approved | Yes | Editor |
| EBUILD-CAP-012 | Platform stamp adapters | Target-specific build number/application metadata seams | Approved | Yes | Editor/Provider |
| EBUILD-CAP-013 | Safe output templating | Sanitized tokens and canonical path validation | Approved | Yes | Editor |
| EBUILD-CAP-014 | Owned-output marker | Clean/delete only exact approved owned leaf | Approved | Yes | Editor |
| EBUILD-CAP-015 | Build lock | Prevents concurrent Foundry sessions in one project | Approved | Yes | Editor |
| EBUILD-CAP-016 | BuildPipeline orchestration | Invokes Unity build from explicit plan/profile | Approved | Yes | Editor |
| EBUILD-CAP-017 | Clean/incremental policy | Recipe selects approved Unity BuildOptions behavior | Approved | Yes | Editor |
| EBUILD-CAP-018 | Detailed Unity BuildReport capture | Summary, steps, files, warnings, errors, size, and timing | Approved | Yes | Editor |
| EBUILD-CAP-019 | Artifact processors | Ordered explicit post-build output mutation before hashing | Approved | Yes | Editor/Bridge |
| EBUILD-CAP-020 | Artifact manifest | Versioned project-readable build artifact record | Approved | Yes | Editor |
| EBUILD-CAP-021 | Streaming checksums | SHA-256 per file plus aggregate manifest hash | Approved | Yes | Editor |
| EBUILD-CAP-022 | Package inventory | Resolved packages and source types | Approved | Yes | Editor |
| EBUILD-CAP-023 | License/notices inventory | Presence and path reporting without legal judgement | Approved | Yes | Editor |
| EBUILD-CAP-024 | Release checklist generation | Generic, itch, portfolio, and project templates | Approved | Yes | Editor |
| EBUILD-CAP-025 | Changelog validation | Expected version heading and configured files | Approved | Yes | Editor |
| EBUILD-CAP-026 | Release-note stub | Factual metadata plus unresolved placeholders | Approved | Yes | Editor |
| EBUILD-CAP-027 | Receipt archive | Success/failure/cancelled receipts outside output | Approved | Yes | Editor |
| EBUILD-CAP-028 | Redacted support export | Privacy-safe diagnostic package | Approved | Yes | Editor |
| EBUILD-CAP-029 | Batch-mode entry point | Deterministic arguments, exit codes, and report output | Approved | Yes | Editor |
| EBUILD-CAP-030 | Settings recovery journal | Before/after snapshots and failed-restore guidance | Approved | Yes | Editor |
| EBUILD-CAP-031 | Stale-lock recovery | Explicit inspection and takeover flow | Approved | Yes | Editor |
| EBUILD-CAP-032 | Standalone Foundry Laboratory | Editor-only fixtures and build simulations | Approved | Yes | Sample/Tests |
| EBUILD-CAP-033 | Workshop setup facade | ADR-001 compliant setup/repair/validation endpoint | Approved | No | Editor Integration |
| EBUILD-CAP-034 | Peer package validators | Separate bridges depend on Foundry and peer Editor assemblies | Approved | No | Bridge |
| EBUILD-CAP-035 | Git metadata provider | Commit/branch/dirty metadata | Deferred | No | Provider |
| EBUILD-CAP-036 | External deployment providers | Itch/store/CI publish operations | Deferred | No | Provider |
| EBUILD-CAP-037 | Signing/notarization providers | Platform-specific secret-bearing steps | Deferred | No | Provider |
| EBUILD-CAP-038 | Multi-recipe build matrix | Sequential orchestration and aggregate report | Deferred | No | Editor |
| EBUILD-CAP-039 | AssetBundle/Addressables build authority | Belongs to their package/provider pipelines | Rejected | No | Other |
| EBUILD-CAP-040 | Automatic source-control commits | Not owned by Foundry | Rejected | No | Other |

### 7.2 MVP capability set

The MVP is one complete release-preparation path:

1. Create project-owned Foundry configuration, version manifest, validation profile, checklist template, and recipe.
2. Bind the recipe to one explicit Unity Build Profile.
3. Resolve and preview an immutable plan and fingerprint.
4. Run Foundry and registered validators without mutation.
5. Validate the effective scene list, define state, version identity, output safety, and required providers.
6. Snapshot approved project settings, apply temporary stamps, and invoke Unity’s Player build pipeline.
7. Run required artifact processors, create package/license inventory, stream hashes, and publish receipt/manifest/checklist.
8. Restore project settings and report restoration status even after failure/cancellation.
9. Prove the same path through the Editor Laboratory and batch-mode fixture.

### 7.3 Later capability set

- Git metadata provider.
- Signing/notarization providers.
- Itch/store/deployment providers.
- Multi-recipe matrix builds.
- Release-note/changelog integrations beyond factual stubs and validation.
- Optional guard for builds launched outside Foundry.
- Rich CI provider adapters and remote artifact stores.

### 7.4 Deferred and rejected ideas

| Idea | Disposition | Reason | Revisit trigger |
|---|---|---|---|
| Automatic Git commit/tag/push | Rejected | Source control remains explicit owner workflow | Separate source-control product/adapter proposal |
| Credentials in BuildRecipe | Rejected | Recipes are project assets and reports may be shared | Never in core |
| Last-second define mutation | Rejected | Unity recompiles on domain reload; immediate build can compile wrong symbols | Never in execute path |
| Core itch/store deployment | Deferred provider | Network/vendor credentials and release policy are external | Approved provider specification |
| Automatic build-number increment/writeback | Deferred | Creates source-control races and dirty projects | Version reservation design with explicit transaction |
| AssetBundle/Addressables pipeline | Rejected from core | Different authority and artifacts | Provider/owning package integration |
| Closed-platform universal support claim | Rejected | Requires licensed modules, docs, and evidence | Per-platform provider/evidence |

---

## 8. Architecture Overview

### 8.1 Design model

| Layer | Contains | Must not contain |
|---|---|---|
| Definition/configuration | BuildRecipe, version manifest, validation profile, output policy, checklist template, provider requirements | Active sessions, open streams, environment values, BuildReport objects |
| Editor session/behavior | Planner, validators, locks, stamp snapshots, build coordinator, processors, hasher, receipt publisher | Runtime gameplay, scene objects, secrets in assets |
| Presentation/reporting | UI Toolkit window, inspectors, validation dashboard, receipt viewer, redacted export | Build authority duplicated in views |

### 8.2 Component topology

```text
BuildRecipe + Unity BuildProfile + request overrides
        |
        v
BuildPlanner -> canonical BuildPlan + fingerprint
        |
        v
ValidationCoordinator <--- explicit validators/bridges
        |
        v
Approval + BuildLock
        |
        v
SettingsSnapshot -> temporary stamps -> BuildPipeline.BuildPlayer
        |                                      |
        |                                      v
        |                                Unity BuildReport
        v                                      |
restore <--- ArtifactProcessors <--- output ----+
                         |
                         v
              inventory -> checksums -> manifest
                         |
                         v
                BuildReceipt + checklist
```

### 8.3 Authoritative root

| Question | Decision |
|---|---|
| Does the package require a persistent root? | No. It is Editor-only. |
| Authority type | One `EchoBuildService` per Unity project/editor process, created by explicit Editor bootstrap/service access |
| Duplicate behavior | Build lock and service registration reject concurrent sessions |
| Initialization trigger | Open window, call service, or batch entry point |
| Shutdown behavior | Dispose providers/streams, release lock, preserve journals/receipts, attempt restoration |
| Direct-scene behavior | Not applicable; no scene dependency |
| Test injection seam | Interfaces for filesystem, clock, BuildPipeline adapter, validators, stamps, processors, inventory, hashing |

### 8.4 Lifecycle sequence

1. Load configuration and provider registry.
2. Select recipe and create request.
3. Resolve Unity Build Profile, effective scenes/defines, version identity, output, validators, and processors.
4. Canonicalize the plan and calculate fingerprint.
5. Run validation and produce gate decision.
6. Require explicit approval matching the plan fingerprint.
7. Acquire build lock and create recovery journal.
8. Snapshot and apply approved temporary settings.
9. Invoke Unity build with detailed reporting.
10. Run ordered artifact processors after Unity completes.
11. Generate package/license inventory, hashes, manifest, and checklist.
12. Restore settings and close lock.
13. Publish final receipt and events.

### 8.5 Failure model

| Failure | Detection point | User-visible result | Runtime fallback | Diagnostic code |
|---|---|---|---|---|
| Duplicate active build session | Build lock acquisition | Build does not start; existing session details shown | Wait, inspect, or explicitly recover stale lock | EBUILD-001 |
| Missing recipe/configuration | Planning | Window and CLI fail with setup guidance | No mutation | EBUILD-002 |
| Missing Unity Build Profile | Planning | Recipe blocker | No fallback to active profile for release | EBUILD-003 |
| Build Profile targets inactive platform in batch mode | Planning | Batch build blocks with launch guidance | Require correct active profile/target at process start | EBUILD-004 |
| Scene missing/invalid | Validation | Blocker lists scene and source | No build | EBUILD-005 |
| Compiled defines differ from recipe/profile requirements | Validation | Blocker requests profile change and domain reload | No last-second symbol mutation | EBUILD-006 |
| Unsafe output path | Planning/clean | Blocker displays canonical path and reason | Create unique safe leaf or change recipe | EBUILD-007 |
| Unowned nonempty output folder | Preparation | Build blocks or creates unique sibling by policy | Never delete unowned contents | EBUILD-008 |
| Version identity unresolved | Planning | Blocker identifies missing provider/value | No default fabricated release version | EBUILD-009 |
| Temporary stamp application fails | Preparation | Build aborts before BuildPlayer | Restore any partial changes | EBUILD-010 |
| BuildPipeline reports failure | Building | Failure receipt and Unity summary | Restore settings; preserve output for inspection per policy | EBUILD-011 |
| Build cancelled by Unity/user | Building | Cancelled receipt where evidence available | Restore settings; mark output incomplete | EBUILD-012 |
| Required artifact processor fails | Processing | Build result becomes failed for release publication | Do not publish checksums/release manifest | EBUILD-013 |
| Optional artifact processor fails | Processing | Warning or error per recipe; receipt records omission | Continue only when policy permits | EBUILD-014 |
| Checksum generation fails | Hashing | Release publication blocked | Preserve build and write failure receipt | EBUILD-015 |
| Settings restoration fails | Restoring | Critical visible recovery report | Keep snapshot and exact manual repair steps | EBUILD-016 |
| Receipt publication fails | Finalization | Build artifact remains but release gate fails | Write emergency report under Library when possible | EBUILD-017 |
| Provider throws/times out | Validation/processing | Provider-specific finding | Required blocks; optional follows recipe policy | EBUILD-018 |

### 8.6 Unity Build Profile boundary

Unity 6 `BuildProfile` assets remain the source of target platform, profile scenes, scripting defines, and platform/Player overrides. A Foundry recipe records an Editor asset reference and validates the effective profile. It does not mirror every field into package data.

For reproducible MVP release recipes, an explicit Build Profile asset is required. An active platform profile may be inspected for diagnostics, but it is not silently accepted as a release recipe. Batch mode must start Unity on the correct target/profile; Foundry does not promise to switch a non-active platform profile after the batch process has begun.

### 8.7 Mutation and restoration boundary

Foundry may temporarily change only fields declared by the resolved stamp adapters. It captures exact before-values before the first mutation, records each successful mutation, and restores in reverse order after success, failure, or cancellation. Scripting defines are excluded from temporary mutation because their effect requires recompilation/domain reload.

---

## 9. Runtime Data and State Model

> EchoBuildTools has no Player-runtime data. This section defines Editor configuration, active Editor-session state, and durable build evidence.

### 9.1 Definitions and configuration assets

| Type | Purpose | Stable ID? | Mutable during build? | Project-owned instance? |
|---|---|---|---|---|
| EchoBuildToolsConfiguration | Limits, report roots, redaction, lock, protected paths, retention | ConfigurationId | No | Yes |
| BuildRecipe | Profile binding and identity/output/validation/build/checklist policy | BuildRecipeId | No | Yes |
| BuildVersionManifest | Semantic version and optional manual sequence source | VersionManifestId | No during attempt | Yes |
| BuildValidationProfile | Required validators, severities, override policy, timeouts | ValidationProfileId | No | Yes |
| BuildOutputPolicy | Root/template/clean/ownership/retention rules | OutputPolicyId | No | Yes |
| ReleaseChecklistTemplate | Project-owned Markdown checklist structure | TemplateId | No | Yes |

### 9.2 Editor session state

| State object | Owner | Lifetime | Reset rule | Serialization rule |
|---|---|---|---|---|
| ActiveBuildSession | EchoBuildService | One attempt | Disposed after final receipt | Never serialized directly |
| BuildProviderRegistry | EchoBuildService | Editor process/domain | Rebuilt through explicit registrations | Provider IDs/config only in receipts |
| BuildSettingsSnapshot | Active session | Until restoration verified | Destroyed after success retention policy | Recovery journal contains detached before/after values |
| BuildLockRecord | EchoBuildService | Until finalization or crash recovery | Explicit release/stale recovery | Small JSON under `Library/EchoBuildTools` |
| Validation cache | Planner/validator service | Until relevant asset/project change | Invalidated by dependency fingerprint | Derived; safe to delete |
| Receipt index | Report repository | Project history | Rebuild from receipt files | Derived index plus durable receipts |

### 9.3 Stable identifiers

- `BuildRecipeId`, validator IDs, processor IDs, stamp adapter IDs, and template IDs use package/domain stable IDs under SFGSS-003.
- Unity asset GUIDs identify Editor assets such as the Build Profile reference; they are not package-domain recipe identity.
- Display names, profile names, output folder names, and recipe asset paths may change without changing stable IDs.
- Released ID changes require aliases/migration; retired IDs are never reused.
- Receipt IDs and build IDs are unique per attempt and include collision-resistant generated identity.

### 9.4 ScriptableObject safety

Configuration assets remain immutable for the duration of a plan and build attempt. Active locks, BuildReport references, resolved environment values, open streams, current output paths, validation histories, and restoration state never live in ScriptableObjects. If an asset changes after planning, the fingerprint changes and execution requires a new approval.

### 9.5 Durable documents and migration

Durable documents include:

- `EchoBuildReceipt`;
- `EchoBuildArtifactManifest`;
- `EchoBuildRecoveryJournal`;
- `EchoBuildSupportExportManifest`;
- derived receipt index.

Each declares format ID, schema version, tool/package version, UTC timestamps, and bounded sections. Receipts preserve unknown extension records as opaque non-executable data. Migrations are contiguous forward steps on staged copies; original evidence remains untouched until the migrated document validates.

### 9.6 Canonical plan fingerprint

The plan fingerprint is SHA-256 over canonical UTF-8 data including recipe ID/schema, Build Profile asset GUID and resolved effective values, scene list/order, profile defines, target, options, resolved identity and provenance, canonical output path, validators/processors and versions, package versions relevant to planning, and policy versions. Machine-specific secrets and absolute paths are normalized or excluded according to the fingerprint contract.

---

## 10. Public Editor API

### 10.1 Public types

| Type | Kind | Responsibility | Construction/ownership |
|---|---|---|---|
| EchoBuildToolsConfiguration | ScriptableObject | Global editor-only limits, report locations, redaction, lock, and default policies | Project-owned asset |
| BuildRecipe | ScriptableObject | Binds Unity Build Profile to identity, output, validation, build, and checklist policies | Project-owned asset |
| BuildRecipeId | readonly struct | Stable package-domain recipe identity | Serialized in recipe and receipts |
| BuildVersionManifest | ScriptableObject | Project-owned semantic version and optional manual build sequence | Project-owned asset |
| BuildIdentity | readonly struct | Resolved version, channel, build number, label, timestamp, and provider provenance | Created per plan |
| BuildRequest | sealed class/record | Recipe plus explicit request overrides and mode | Caller-owned input |
| BuildPlan | sealed immutable class | Fully resolved preflight/build/output plan and fingerprint | Created by service |
| BuildPlanFingerprint | readonly struct | Canonical SHA-256 plan identity | Created by planner |
| BuildExecutionResult | sealed class | Final status, receipt path, report summary, restoration state, and errors | Returned by service |
| BuildReceipt | versioned DTO | Durable factual record of one attempted build | Foundry-owned report archive |
| BuildArtifactManifest | versioned DTO | Published artifact files, sizes, checksums, and aggregate hash | Written after successful final processing |
| BuildValidationFinding | readonly struct | Validator ID, code, severity, summary, details, fix, and override policy | Produced by validators |
| BuildValidationReport | sealed immutable class | Ordered findings and gate decision | Created per plan/session |
| IBuildValidator | interface | Explicit preflight validation provider | Package/bridge/project implementation |
| IBuildIdentityProvider | interface | Optional deterministic identity metadata source | Provider implementation |
| IBuildStampAdapter | interface | Target-specific temporary setting application/restoration | Package/provider implementation |
| IBuildArtifactProcessor | interface | Required or optional output mutation before hashing | Bridge/provider implementation |
| IBuildInventoryProvider | interface | Package/toolchain/license inventory contribution | Package/provider implementation |
| IEchoBuildService | interface | Plan, validate, execute, inspect, and export operations | Implemented by Editor service |
| BuildProviderRegistration | IDisposable handle | Owns validator/processor/provider registration lifecycle | Returned by service |
| BuildSessionState | enum | Idle, Planning, Validating, AwaitingApproval, Preparing, Building, Processing, Hashing, Restoring, Completed, Failed, Cancelled | Editor session state |
| BuildChannel | enum | Development, Beta, ReleaseCandidate, Release, Custom | Serialized public API |
| BuildResultStatus | enum | Succeeded, Failed, Cancelled, Blocked | Receipt/result state |
| EchoBuildCommandLine | static class | Batch-mode entry point and exit-code mapping | Editor assembly |

### 10.2 Public methods and properties

| Member | Purpose | Preconditions | Result/failure behavior | Thread/main-loop rule |
|---|---|---|---|---|
| BuildPlan CreatePlan(BuildRequest request) | Resolve profile, scenes, defines, identity, output, providers, and fingerprint without mutation | Valid recipe and project state | Plan or structured planning failure | Editor main thread |
| BuildValidationReport Validate(BuildPlan plan) | Run ordered preflight validators | Plan fingerprint current | Immutable report and gate decision | Editor main thread; validators declare async subwork |
| BuildExecutionResult Execute(BuildPlan plan, BuildApproval approval) | Execute one approved build session | Matching plan fingerprint, passing gate, no active lock | Final receipt even on failure when possible | Editor main thread; BuildPlayer blocks |
| bool TryCancelPending(BuildSessionId id) | Cancel before BuildPipeline begins or during cancellable processing | Session exists and phase cancellable | True only when cancellation accepted | Editor main thread |
| BuildReceipt LoadReceipt(string receiptId) | Read one versioned receipt | Receipt exists and schema supported | Receipt or structured read failure | Editor/main or detached I/O |
| BuildComparison CompareReceipts(string leftId, string rightId) | Compare identities, inputs, validation, files, size, and timing | Both receipts readable | Detached comparison model | Editor |
| SupportExportResult ExportRedactedReport(string receiptId, string destination) | Create privacy-safe support package | Destination validated | Written file list and redaction report | Detached I/O after validation |
| BuildProviderRegistration RegisterValidator(IBuildValidator validator) | Register one explicit stable-ID validator | Unique ID and compatible schema | Disposable registration or rejection | Editor main thread |
| BuildProviderRegistration RegisterArtifactProcessor(IBuildArtifactProcessor processor) | Register one ordered processor | Unique ID and declared requirement | Disposable registration or rejection | Editor main thread |
| OutputSafetyResult ValidateOutputTarget(string path, BuildRecipeId recipeId) | Canonicalize and evaluate output/clean safety | Path provided | Allowed, create-new, or blocked result | Editor |

### 10.3 Events and callbacks

| Event | Raised by | Timing | Payload | Listener assumptions |
|---|---|---|---|---|
| PlanCreated | Build service | After immutable plan and fingerprint are complete | BuildPlanSummary | Listeners cannot alter plan |
| ValidationCompleted | Build service | After all validators finish or timeout | BuildValidationReport | Raised before approval |
| BuildStateChanged | Build service | After authoritative session state transition | BuildStateChange | Presentation only |
| BuildCompleted | Build service | After receipt and restoration attempt complete | BuildExecutionResult | Raised for success/failure/cancel |
| ReceiptPublished | Build service | After durable receipt publication | BuildReceiptSummary | No full secrets/absolute paths in default payload |
| ProviderRejected | Build service | After duplicate/incompatible registration attempt | ProviderRegistrationFailure | Diagnostics only |

Events are raised only after the authoritative state or durable publication changes. A listener cannot be required for restoration or receipt creation.

### 10.4 Async and cancellation policy

`BuildPipeline.BuildPlayer` is treated as a blocking Editor operation. The UI may present a stateful session and use asynchronous/background work for detached hashing, inventory reads, or report serialization where safe, but the package does not pretend the Player build itself is freely cancellable or non-blocking.

Cancellation is accepted only during planning, validation, awaiting approval, preparation before irreversible mutation, and cancellable detached processing. Once Unity begins building, the final result follows Unity’s success/failure/cancellation report. Required artifact publication cannot be cancelled into a false success state.

### 10.5 API ergonomics

The novice path is: choose recipe, Preview, Validate, Build, Open Receipt. The advanced path injects services and registers providers explicitly. The Editor window is a client of `IEchoBuildService`, not the only API.

---

## 11. Editor Tooling and Authoring Experience

### 11.1 Setup workflow

1. Install EchoBuildTools.
2. Open **Tools > The Sperk’s Forge > The Foundry**.
3. Run setup preview.
4. Create project-owned configuration, reports folder, version manifest, checklist template, and first recipe.
5. Select one existing Unity Build Profile asset.
6. Preview the resolved plan.
7. Run validation without building.
8. Open the Foundry Laboratory before attempting production output.

### 11.2 Setup operations

| Operation | Creates | Modifies | Repeats safely? | Undo/backup | Report output |
|---|---|---|---|---|---|
| Create configuration | Configuration asset and report directories | Nothing existing | Yes | Undo for asset creation | Setup receipt |
| Create version manifest | Project-owned version asset | Nothing existing | Yes | Undo | Setup receipt |
| Create recipe | BuildRecipe asset | Nothing existing | Yes | Undo | Setup receipt |
| Create checklist templates | Project-owned Markdown templates | Nothing existing | Yes | Backup before explicit replacement | Setup receipt |
| Repair references | Missing links chosen by user | Selected project assets | Yes | Preview + Undo where supported | Repair receipt |
| Create output ownership marker | Marker inside approved output leaf | Only new/empty leaf | Yes | Delete marker/leaf if empty | Build receipt |

### 11.3 Inspectors and windows

| Tool | User | Purpose | Runtime dependency? |
|---|---|---|---|
| Foundry Window | Developer/release manager | Recipe selection, plan, validation, build, reports, checklists | No |
| BuildRecipe Inspector | Developer | Profile binding and policy editing with validation | No |
| Version Manifest Inspector | Release manager | Semantic version and explicit sequence source | No |
| Validation Dashboard | Developer/tester | Grouped findings, provider state, fixes, evidence links | No |
| Receipt Viewer/Comparer | Release/support | Inspect and compare attempts and artifacts | No |
| Output Safety Inspector | Developer | Explain canonical target, marker, and clean eligibility | No |
| Foundry Laboratory | Maintainer/tester | Run safe fixture scenarios | No |

### 11.4 Validation and repair

| Check ID | Condition | Severity | Fix available? | Safe auto-fix? |
|---|---|---|---|---|
| EBUILD-VAL-001 | Foundry configuration missing | Blocker | Yes | Yes |
| EBUILD-VAL-002 | Duplicate BuildRecipeId | Blocker | Yes | No |
| EBUILD-VAL-003 | Unity Build Profile reference missing | Blocker | Yes | No |
| EBUILD-VAL-004 | Build Profile target unavailable/module missing | Blocker | No | No |
| EBUILD-VAL-005 | Effective scene list empty | Blocker | Yes | No |
| EBUILD-VAL-006 | Scene path missing or excluded unexpectedly | Error | Yes | No |
| EBUILD-VAL-007 | Duplicate scene entry | Warning | Yes | No |
| EBUILD-VAL-008 | Required scene order mismatch | Error | Yes | No |
| EBUILD-VAL-009 | Define mismatch requiring domain reload | Blocker | Yes | No |
| EBUILD-VAL-010 | Release identity unresolved | Blocker | Yes | No |
| EBUILD-VAL-011 | Invalid semantic version/channel/build number | Blocker | Yes | No |
| EBUILD-VAL-012 | Unsafe or protected output path | Blocker | Yes | No |
| EBUILD-VAL-013 | Unowned nonempty output target | Blocker | Yes | No |
| EBUILD-VAL-014 | Dirty unsaved scene | Error | Yes | No |
| EBUILD-VAL-015 | Script compilation failed | Blocker | No | No |
| EBUILD-VAL-016 | Required changelog heading missing | Error | Yes | No |
| EBUILD-VAL-017 | Package license/notice missing | Warning | No | No |
| EBUILD-VAL-018 | Required validator provider absent | Blocker | No | No |
| EBUILD-VAL-019 | Receipt/report destination unsafe | Blocker | Yes | No |
| EBUILD-VAL-020 | Stale active-build lock | Error | Yes | No |

Auto-fix is limited to package-owned configuration and safe deterministic edits. Another package’s data is repaired only through its documented setup facade or bridge.

### 11.5 Build plan approval

The UI must show at least:

- recipe and stable ID;
- Unity Build Profile asset and target;
- effective scene order;
- effective defines and required/forbidden define checks;
- version/channel/build number and source provenance;
- clean/incremental/development options;
- canonical output path and ownership state;
- validators/processors with required/optional status;
- package/tool versions;
- plan fingerprint;
- blocking findings and acknowledgements.

Execution approval binds to that fingerprint. Any change requires a new plan and approval.

---

## 12. Installation, Build Setup, and Direct Testing

### 12.1 Installation routes

- Unity Package Manager Git URL.
- Local path or embedded package during development.
- Tarball installation.
- Registry distribution if approved later.
- Workshop selection when available.

Every advertised route requires separate SFGSS-004 evidence.

### 12.2 Minimal project setup

- One Unity 6 Build Profile asset targeting an installed platform module.
- One Foundry configuration asset.
- One version manifest or explicit identity provider.
- One BuildRecipe referencing the profile.
- One safe output root/template.
- At least one enabled scene in the effective profile list.

### 12.3 Production build setup

A production build must run through a reviewed recipe and fresh plan. Release Candidate and Release channels cannot bypass Blocker or Error findings. The recipe identifies required validators, processors, receipt location, output policy, and checklist template.

### 12.4 Batch-mode setup

Batch execution uses Unity’s `-executeMethod` entry point plus Foundry arguments. Unity must start with the appropriate target/profile already active when cross-platform recompilation would be required. The command-line runner validates this state and exits before mutation when it is wrong.

### 12.5 Scene isolation rule

No scene is required to use Foundry. Build smoke-test scenes belong to disposable fixture projects or the package’s Editor Laboratory. Foundry does not ship a production runtime scene.

---

## 13. Standalone Foundry Laboratory and Samples

### 13.1 Laboratory purpose

The standalone proof is an Editor Laboratory plus disposable fixture projects, not a runtime showcase. It must exercise planning, validation, safe paths, stamps, BuildPipeline adapters, failure recovery, artifact processing, hashing, reports, CLI, removal, and reinstall without another Echo package.

### 13.2 Required Laboratory contents

- Tiny redistributable fixture scenes and Build Profile.
- Valid, warning, blocker, failure, cancellation, and stale-lock recipes.
- Fake/injected BuildPipeline adapter for destructive/failure unit scenarios.
- Optional real small Player build fixture for supported test platform.
- Controlled artifact processor fixtures.
- Protected/unowned/owned path fixtures under a disposable root.
- Receipt/manifest verification utility.
- Reset control that deletes only Laboratory-owned files.

### 13.3 Laboratory acceptance checklist

| Test | Action | Expected result | Automated/manual | Status |
|---|---|---|---|---|
| EBUILD-LAB-001 | Open Foundry with no configuration | Setup guidance appears; no assets are created automatically | Manual | Not run |
| EBUILD-LAB-002 | Create default configuration twice | Second run is idempotent and preserves edits | Manual | Not run |
| EBUILD-LAB-003 | Bind valid Unity Build Profile | Recipe resolves target and scene list | Manual | Not run |
| EBUILD-LAB-004 | Remove bound profile | Planning blocks with EBUILD-003 | Manual | Not run |
| EBUILD-LAB-005 | Preview plan | All resolved inputs and fingerprint are visible | Manual | Not run |
| EBUILD-LAB-006 | Change recipe after plan approval | Execution rejects stale fingerprint | Manual | Not run |
| EBUILD-LAB-007 | Run validation only | No output/project mutation occurs | Manual | Not run |
| EBUILD-LAB-008 | Use empty scene list | Blocker prevents build | Manual | Not run |
| EBUILD-LAB-009 | Use missing scene | Blocker identifies path | Manual | Not run |
| EBUILD-LAB-010 | Create define mismatch | Build blocks before BuildPipeline | Manual | Not run |
| EBUILD-LAB-011 | Resolve identity from manifest | Version/channel/build number match preview | Manual | Not run |
| EBUILD-LAB-012 | Resolve identity from environment provider | Provenance recorded without exposing unrelated environment data | Manual | Not run |
| EBUILD-LAB-013 | Apply temporary stamp | Built Player receives stamp and project restores | Manual | Not run |
| EBUILD-LAB-014 | Force stamp failure | No build starts; partial changes restore | Manual | Not run |
| EBUILD-LAB-015 | Plan safe default output | Path is under approved Builds leaf | Manual | Not run |
| EBUILD-LAB-016 | Attempt project-root clean | Deletion blocked | Manual | Not run |
| EBUILD-LAB-017 | Attempt Assets/Packages/Library clean | Deletion blocked for each protected path | Manual | Not run |
| EBUILD-LAB-018 | Attempt traversal/symlink escape | Canonical target blocked | Manual | Not run |
| EBUILD-LAB-019 | Clean matching owned leaf | Only exact leaf is removed | Manual | Not run |
| EBUILD-LAB-020 | Encounter unowned nonempty leaf | No deletion; policy creates sibling or blocks | Manual | Not run |
| EBUILD-LAB-021 | Run successful development build | Receipt, manifest, checksums, and checklist are produced | Manual | Not run |
| EBUILD-LAB-022 | Run failing build fixture | Failure receipt and restoration evidence are produced | Manual | Not run |
| EBUILD-LAB-023 | Cancel before BuildPlayer | Session cancels without output mutation | Manual | Not run |
| EBUILD-LAB-024 | Cancel during BuildPlayer | Result reflects Unity cancellation boundary honestly | Manual | Not run |
| EBUILD-LAB-025 | Fail required artifact processor | Release publication is blocked | Manual | Not run |
| EBUILD-LAB-026 | Fail optional artifact processor | Recipe policy controls warning/error and receipt records it | Manual | Not run |
| EBUILD-LAB-027 | Hash multi-file output | Per-file and aggregate SHA-256 values verify | Manual | Not run |
| EBUILD-LAB-028 | Modify output after hashing attempt | Manifest verification fails and publication is invalidated | Manual | Not run |
| EBUILD-LAB-029 | Generate package/license inventory | Resolved package facts and missing notices appear | Manual | Not run |
| EBUILD-LAB-030 | Generate generic/itch/portfolio checklists | Each template remains factual and editable | Manual | Not run |
| EBUILD-LAB-031 | Create stale lock fixture | Recovery requires explicit confirmation | Manual | Not run |
| EBUILD-LAB-032 | Start concurrent session | Second session is rejected | Manual | Not run |
| EBUILD-LAB-033 | Simulate restoration failure | Critical report includes exact snapshot/manual repair | Manual | Not run |
| EBUILD-LAB-034 | Export redacted support report | Absolute paths, usernames, and environment values are absent | Manual | Not run |
| EBUILD-LAB-035 | Run command-line success fixture | Exit code/report path are deterministic | Automated/manual | Not run |
| EBUILD-LAB-036 | Run command-line blocker fixture | Nonzero exit and validation report are deterministic | Automated/manual | Not run |
| EBUILD-LAB-037 | Remove Foundry after a build | Built artifact and project-owned receipts remain; project compiles | Manual | Not run |
| EBUILD-LAB-038 | Reinstall Foundry | Existing recipes/receipts reopen without migration loss | Manual | Not run |
| EBUILD-LAB-039 | Run external Unity build without Foundry | Documented bypass behavior occurs; optional guard policy is explicit | Manual | Not run |
| EBUILD-LAB-040 | Compare two receipts | Differences in plan, identity, validation, size, files, and duration are visible | Manual | Not run |

### 13.4 Optional integration samples

| Sample | Packages involved | Purpose | Why it is not standalone proof |
|---|---|---|---|
| Workshop + Foundry Setup Lab | Workshop, Foundry | Generate and validate a starter build recipe | Depends on composer and setup facade |
| Foundation Validation Lab | Foundry plus selected peer bridge | Aggregate package-specific validators | Depends on peer package |
| Itch Deployment Lab | Foundry plus approved provider | Consume successful artifact and upload in test account | External network/provider dependency |
| Git Identity Lab | Foundry plus Git provider | Enrich receipt with repository facts | Repository/provider dependency |

---

## 14. Presentation, UI, and Accessibility

### 14.1 Presentation ownership

The core service is nonvisual Editor logic. The package-owned UI Toolkit window and inspectors present plans, findings, progress, receipts, comparisons, and recovery steps. Runtime UI is not applicable.

### 14.2 Required states

- No configuration.
- Ready/idle.
- Planning.
- Validating.
- Awaiting approval.
- Blocked.
- Preparing.
- Building.
- Processing artifacts.
- Hashing.
- Restoring.
- Succeeded.
- Failed.
- Cancelled.
- Recovery required.

### 14.3 Accessibility requirements

- Complete keyboard navigation and sensible focus order.
- Status never communicated by color alone.
- Scalable Editor text and layouts that tolerate large fonts.
- Copyable plain-text finding details and paths.
- Progress labels paired with numeric/current-step text where available.
- Reduced/disabled decorative animation.
- Confirmation dialogs name the exact output target and consequence.

### 14.4 Visual customization

Package UI uses restrained Editor styling. Project branding appears only in generated release templates or configured product metadata, not by editing core service code.

---

## 15. Diagnostics and Observability

### 15.1 Standalone diagnostics

| Diagnostic | Surface | Release availability | Cost |
|---|---|---|---|
| Configuration/profile health | Window/validator API/report | Editor | On demand |
| Active session/lock | Window/lock file/API | Editor | Low |
| Plan/fingerprint | Plan viewer/receipt | Editor | On plan |
| Validation findings | Dashboard/report/CLI | Editor | On demand |
| Build progress/state | Window/API | Editor | During attempt |
| Restoration state | Receipt/recovery journal | Editor | During/final |
| Artifact/file/hash inventory | Manifest/receipt | Editor/output | Post-build streaming cost |
| Provider registry health | Window/API | Editor | Low |

### 15.2 Structured status

Expose:

- package/schema version;
- selected recipe/profile IDs;
- active session state and lock owner;
- plan fingerprint and approval state;
- target/profile/scenes/options;
- version identity and provider provenance;
- canonical output and ownership state;
- validator/processor registry and last outcomes;
- BuildReport summary;
- artifact/manifest/checksum publication state;
- settings restoration state;
- receipt ID/path and redaction status.

### 15.3 Diagnostic codes

The package reserves `EBUILD-*`. Codes EBUILD-001 through EBUILD-018 are defined in the failure model. Additional stable codes require documented meaning and migration; codes are never recycled.

### 15.4 Observatory bridge

A separate Editor/support bridge may publish Foundry readiness, last build result, package inventory summary, validation counts, and receipt links to Observatory tooling. Foundry does not depend on Observatory, and no runtime dashboard is required.

### 15.5 Logging policy

- No secrets, raw environment dump, signing identity, access token, or credential path.
- Absolute paths are local-only and redacted in support exports.
- One structured message per state/finding rather than progress spam.
- Unity build errors remain linked to the BuildReport.
- Provider logs are namespaced and bounded.

---

## 16. Persistence and Save Integration

### 16.1 Persistence classification

| State | Scope | Owner | Saved? | Backend |
|---|---|---|---|---|
| Recipes/config/version/checklists | Project/Editor | Project + Foundry schema | Yes | Project assets/Markdown |
| Active session | Editor process | Foundry | No | Memory |
| Build lock/recovery journal | Editor recovery | Foundry | Temporarily | `Library/EchoBuildTools` |
| Receipts/manifests/checksums | Project/release evidence | Foundry/project | Yes | Configured report/output folders |
| Receipt index/cache | Editor derived | Foundry | Derived | Library or report cache |
| Credentials | External provider/CI | Not Foundry | Never in Foundry docs | Provider secret store |

### 16.2 Standalone behavior

EchoSave and EchoSettings are irrelevant to Foundry persistence. Build evidence uses editor/project files. Removing Foundry does not delete project-owned receipts, recipes, version manifests, checklists, or build outputs.

### 16.3 Optional participant/provider contract

Not applicable to Chronicle save participants. Providers contribute build-time validation, identity metadata, stamps, inventory, or artifact processing through Foundry Editor interfaces. Provider configuration must reference secrets opaquely and document its own storage.

### 16.4 Failure and recovery

- Corrupt receipts are quarantined and excluded from the index, not silently overwritten.
- Unknown newer schemas open read-only with explicit limitation when possible.
- A recovery journal survives abrupt Editor exit until settings are verified/restored.
- Stale locks require explicit inspection and takeover.
- Build outputs are never deleted as “recovery” unless exact ownership is proven.

---

## 17. Integration and Bridge Contracts

### 17.1 Integration philosophy

Foundry is the aggregation point for build-time evidence, not the owner of every package’s validation logic. Peer-package bridges depend on Foundry and the peer Editor assembly, register explicit providers, and tear down cleanly. External network/signing actions live in provider adapters and occur only after a successful, verified artifact exists.

### 17.2 Planned integrations

| Other authority | Connection type | Owner of bridge | Direction | Data/events exchanged | Required? |
|---|---|---|---|---|---|
| First Light / all runtime packages | Validator bridge | Separate bridge or package-owned Editor integration | Peer Editor -> Foundry | Package configuration health and release blockers | No |
| The Workshop | ADR-001 setup facade | EchoBuildTools owner integration | Workshop -> Foundry Editor | Create/validate/repair Foundry configuration and recipes | No |
| The Observatory | Report/import bridge | Separate bridge | Foundry -> Observatory Editor/support | Build facts and package health; no runtime dependency | No |
| The Accord | Validator bridge | Separate bridge | Accord Editor -> Foundry | Settings schemas/migrations/defaults validation | No |
| The Chronicle | Validator bridge | Separate bridge | Chronicle Editor -> Foundry | Save schema/migration/recovery readiness | No |
| Git provider | Provider adapter | Separate provider package/project adapter | Provider -> Foundry | Commit, branch, dirty state, tags | No |
| Itch/store deployer | Provider adapter | Separate provider package | Foundry artifact -> provider | Artifact path, manifest, version, channel | No |
| CI systems | Command-line/project adapter | Project/CI-owned | CI -> Foundry CLI | Recipe ID, output override, identity override, report path | No |
| Signing/notarization | Platform provider | Separate provider package | Foundry -> provider -> artifact | Credential references and signed output evidence | No |

### 17.3 Bridge placement decision

- Package-specific validators: separate bridge when they depend on both Foundry and a peer package.
- Small package-owned Editor validation that references only Foundry contracts may live in an owner integration assembly if removal stays clean.
- Git, deployment, signing, notarization, closed-platform, and CI SDK integrations are separate providers.
- Project-specific release rules remain project adapters/configuration.

### 17.4 Integration failure behavior

- Missing optional provider: omitted and reported.
- Missing required recipe provider: blocker before mutation.
- Version mismatch: provider registration rejected with stable diagnostic.
- Provider exception/timeout: isolated and recorded; required/optional policy decides gate.
- Provider removed: old receipt extension remains opaque; core still opens known sections.
- Processor teardown: registrations dispose before package removal/domain reload.

---

## 18. Performance and Resource Policy

### 18.1 Performance targets

| Metric | Target | Measurement scene/tool | Release threshold |
|---|---|---|---|
| Window idle update | No recurring full-project scan; <1 ms average editor update target | Profiler in Foundry Laboratory | Not measured |
| Plan creation | <250 ms for ordinary project excluding provider work | Stopwatch plus project fixtures | Not measured |
| Core preflight | <2 s for ordinary project excluding explicit deep validators | Validation fixture projects | Not measured |
| Receipt serialization | Streaming/low-memory; <250 ms for normal report | Build fixture | Not measured |
| Checksum memory | O(buffer) with configurable buffer; no whole-file load | Large artifact fixture | Not measured |
| Report history | Bounded by count/age/size policy | Receipt stress fixture | Not measured |

### 18.2 Allocation policy

- No project/package/asset scan every Editor repaint.
- File hashing uses bounded streaming buffers and bounded concurrency.
- BuildReport details are transformed into detached summaries before long-term retention.
- Validator deep scans are explicit and cacheable with declared invalidation.
- Receipt indexes page/lazily load large histories.
- Reflection is not used for provider discovery.

### 18.3 Domain reload and Editor restart behavior

Provider registrations rebuild after domain reload. Active plans become stale if the domain/project state changes. If reload occurs during pre-build preparation, the session fails safely and recovery journal remains. The package does not attempt to resume a Unity Player build across domain reload.

### 18.4 Scalability limits

Configuration must bound:

- receipt count/age/disk usage;
- validator and processor count;
- provider timeout;
- inventory entries;
- checksum file count and concurrency;
- report extension payload size;
- Laboratory fixture output;
- queued batch recipes when later supported.

---

## 19. Security, Privacy, and Platform Considerations

### 19.1 Data sensitivity

Foundry handles filesystem paths, environment-derived metadata, package inventories, build configuration, and optional provider references. It must treat usernames, machine paths, repository remotes, environment variables, signing identities, and credentials as sensitive.

### 19.2 Trust boundaries

- Canonicalize paths before create/delete/copy.
- Reject traversal, protected roots, project ancestors, unsafe symlink/reparse-point escapes, and unowned clean targets.
- Sanitize output-template tokens and filenames per target platform.
- Never execute shell commands or provider-supplied strings in the core.
- Never serialize secret values into assets, receipts, manifests, logs, or CLI examples.
- Provider output is validated before it enters the receipt or modifies artifacts.
- Checksums provide integrity evidence, not authenticity or code signing.
- License inventory reports presence/facts, not legal approval.

### 19.3 Platform behavior

| Platform | Supported? | Special behavior | Validation required |
|---|---|---|---|
| Windows | Planned MVP test target | Build Profile and platform module required | Clean/incremental build, path, stamp, manifest, launch smoke test |
| macOS | Planned | Requires macOS editor/module and optional signing provider | Native test evidence required |
| Linux | Planned | Requires Linux module | Native build and launch evidence required |
| WebGL/Web | Planned | Directory artifact hashing and web template considerations | Build and local-host smoke test required |
| Android | Planned | Platform build number/signing handled through adapter/provider | Device/install evidence required |
| iOS/tvOS | Planned | Produces Xcode project; signing/export provider separate | macOS/Xcode evidence required |
| Console/closed platforms | Unknown until approved access | Provider/docs cannot be public without platform rules | Platform-holder evidence required |

### 19.4 Batch and CI security

Command-line arguments are assumed visible to local process inspection and logs. They may carry recipe IDs, versions, channels, nonsecret output paths, and report paths, but not credentials. Providers obtain secrets from their approved environment/secret store and redact all output.

---

## 20. Package and Repository Structure

### 20.1 Required package anatomy

```text
Packages/com.echodevgames.echo-build-tools/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   └── Developer/
├── Editor/
│   ├── Core/
│   ├── Data/
│   ├── Planning/
│   ├── Validation/
│   ├── Versioning/
│   ├── Output/
│   ├── Execution/
│   ├── Reporting/
│   ├── Providers/
│   ├── Setup/
│   ├── UI/
│   └── EchoDevGames.EchoBuildTools.Editor.asmdef
├── Samples~/
│   └── The Foundry Laboratory/
└── Tests/
    └── Editor/
        └── EchoDevGames.EchoBuildTools.Tests.Editor.asmdef
```

There is no `Runtime/` directory or runtime assembly in the approved MVP.

### 20.2 Proposed source tree

```text
Editor/
├── Core/               IEchoBuildService, sessions, registrations, states
├── Data/               recipes, configuration, version/checklist policies
├── Planning/           resolver, canonicalizer, fingerprint
├── Validation/         coordinator, built-in validators, findings
├── Versioning/         identity providers, stamp adapters, snapshots
├── Output/             path safety, marker, clean policy, artifact processors
├── Execution/          BuildPipeline adapter, locks, recovery journal, CLI
├── Reporting/          receipts, manifests, checksums, inventory, comparison
├── Providers/          explicit provider contracts/registry
├── Setup/              ADR-001 facade, setup/repair/validation
└── UI/                 UI Toolkit window, inspectors, report viewer
```

### 20.3 Assembly definitions

| Assembly | Platform | References | Auto referenced? | Purpose |
|---|---|---|---|---|
| EchoDevGames.EchoBuildTools.Editor | Editor | UnityEditor, UnityEngine, supported .NET | False | Core Editor-only service, data, tools, UI, CLI |
| EchoDevGames.EchoBuildTools.Tests.Editor | Editor/Test | Editor assembly, Unity Test Framework | False | EditMode and fixture tests |
| EchoDevGames.EchoBuildTools.<Peer>.Editor | Editor bridge | Foundry Editor + peer Editor/runtime as approved | False | Package-specific validator integration |
| EchoDevGames.EchoBuildTools.<Provider>.Editor | Editor provider | Foundry Editor + provider SDK | False | Git/deploy/signing/toolchain adapter |

### 20.4 Repository files

- README and five-minute build path.
- Full configuration/recipe/profile boundary reference.
- Output safety and recovery guide.
- CLI/CI guide.
- Provider/validator/processor developer guide.
- Receipt/manifest schema and redaction guide.
- Foundry Laboratory guide.
- Changelog, license, notices, support/security guidance, release checklist, and Current Notes.

---

## 21. Compatibility, Versioning, and Deprecation

### 21.1 Supported versions

| Dependency | Minimum | Tested | Notes |
|---|---|---|---|
| Unity | 6000.0 | Not run; development baseline 6000.3.8f1 | Build Profile API required |
| Editor OS | Windows first | Not run | Other desktop Editor platforms require evidence |
| Target modules | Per recipe | Not run | Package does not install Unity modules |
| Optional providers | Per provider specification | Not run | Exact versions declared in provider package |

### 21.2 Semantic versioning policy

- Patch: bug fixes that do not change public API/schema/receipt meaning.
- Minor: backward-compatible validators, optional fields, providers, templates, or tools.
- Major: breaking public Editor API, recipe schema, receipt/manifest semantics, output marker, CLI contract, or provider contract.
- Build receipt and manifest schema versions remain separate from package SemVer.

### 21.3 Deprecation policy

Deprecated recipe fields, CLI arguments, provider APIs, and schemas remain readable for at least one documented migration window. Warnings name replacement and deadline. Durable evidence is never rewritten in place merely to silence deprecation.

### 21.4 GUID and asset compatibility

Public scripts, setup templates, sample fixtures, and configuration types preserve committed `.meta` files. Project-owned recipes and Build Profiles retain their own GUIDs. Moves/renames preserve GUID when identity continues.

---

## 22. Documentation Requirements

### 22.1 Required user documentation

- Overview, authority, and non-goals.
- Install and five-minute recipe/build guide.
- Unity Build Profile versus Foundry recipe explanation.
- Version/channel/build-number guide.
- Output path and cleaning safety guide.
- Validation/severity/override guide.
- Receipt, manifest, checksum, package/license, and checklist guide.
- Batch-mode/CI guide.
- Failure recovery and settings restoration guide.
- Laboratory guide, troubleshooting, diagnostic codes, limitations, and removal.

### 22.2 Required developer documentation

- Service lifecycle and state machine.
- Plan canonicalization/fingerprint contract.
- Validator, identity provider, stamp adapter, inventory provider, and artifact processor APIs.
- Provider security/redaction requirements.
- Schema/migration rules.
- Test strategy and release workflow.
- ADRs, Current Notes, and checkpoint records.

### 22.3 Documentation truth rule

Examples must compile against the documented Unity/package release. Menu names, Build Profile behavior, CLI arguments, receipt fields, protected paths, and screenshots must match evidence. A generated build receipt never substitutes for missing runtime test evidence.

### 22.4 Living repository and Obsidian workflow

Use the repository-first Current Notes and promotion rules from SFGSS-000/SFGSS-005. Build findings discovered during implementation enter Current Notes, then migrate to the specification, ADR, test/issue report, guide, or changelog that owns them.

### 22.5 Repository scan and handoff order

1. README/documentation index.
2. SFGSS-000 through SFGSS-005.
3. This specification.
4. Build/provider ADRs and bridge specs.
5. Current Notes.
6. Active checkpoint, tests, issue log, changelog.
7. Relevant Editor implementation and fixtures.

---

## 23. Testing Strategy

### 23.1 Test layers

| Layer | Scope | Examples | Required for MVP? |
|---|---|---|---|
| EditMode unit | Pure plans, IDs, canonicalization, paths, policies, DTOs | Fingerprint, token sanitization, ownership marker, severity gate | Yes |
| Editor integration | Unity assets/APIs and injected BuildPipeline | Profile/scenes, stamps, restoration, BuildReport mapping | Yes |
| Foundry Laboratory | User-visible isolated workflow | Plan, validate, build fixture, reports, recovery | Yes |
| Bridge/provider Integration Lab | One optional connection | Peer validator, Git metadata, deploy/sign provider | When shipped |
| Clean-project install | Packaging and default setup | Git/local/tarball/registry route | Yes |
| Batch-mode fixture | CLI and active profile/target behavior | Success/block/failure exit/report | Yes |
| Existing-project adoption | Manual build workflow migration | Parallel Foundry recipe, rollback, parity | Before adoption claim |

### 23.2 Required test categories

- Recipe and configuration validation.
- Build Profile binding and scene resolution.
- Plan canonicalization, fingerprint stability, and stale approval.
- Validator registration, severity, ordering, timeout, failure isolation, and override policy.
- Version identity/provider provenance and invalid values.
- Output path traversal/protected-root/marker/clean safety.
- Settings snapshot/stamping/restoration under success/failure/cancel/crash simulation.
- Build options and BuildReport mapping.
- Artifact processor ordering and required/optional failures.
- Streaming checksums and manifest verification.
- Package/license inventory and legal-disclaimer wording.
- Receipt retention, corruption, migration, comparison, and redaction.
- Batch mode, domain reload, locks, removal, and reinstall.
- Performance, privacy, security, and platform evidence.

### 23.3 Test case registry

| Test ID | Case | Requirement scope | Setup | Action | Expected result | Automation | Status |
|---|---|---|---|---|---|---|---|
| EBUILD-T-001 | PLAN-01 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-002 | PLAN-02 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-003 | PLAN-03 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-004 | PLAN-04 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-005 | PLAN-05 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-006 | PLAN-06 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-007 | PLAN-07 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-008 | PLAN-08 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-009 | PLAN-09 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-010 | PLAN-10 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-011 | PLAN-11 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-012 | PLAN-12 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-013 | PLAN-13 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-014 | PLAN-14 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-015 | PLAN-15 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-016 | PLAN-16 | Planning, fingerprinting, stale plan, profile binding, token resolution | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-017 | VAL-01 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-018 | VAL-02 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-019 | VAL-03 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-020 | VAL-04 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-021 | VAL-05 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-022 | VAL-06 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-023 | VAL-07 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-024 | VAL-08 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-025 | VAL-09 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-026 | VAL-10 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-027 | VAL-11 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-028 | VAL-12 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-029 | VAL-13 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-030 | VAL-14 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-031 | VAL-15 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-032 | VAL-16 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-033 | VAL-17 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-034 | VAL-18 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-035 | VAL-19 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-036 | VAL-20 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-037 | VAL-21 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-038 | VAL-22 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-039 | VAL-23 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-040 | VAL-24 | Built-in/provider validation, severity, override, timeout, isolation | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-041 | ID-01 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-042 | ID-02 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-043 | ID-03 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-044 | ID-04 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-045 | ID-05 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-046 | ID-06 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-047 | ID-07 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-048 | ID-08 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-049 | ID-09 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-050 | ID-10 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-051 | ID-11 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-052 | ID-12 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-053 | ID-13 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-054 | ID-14 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-055 | ID-15 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-056 | ID-16 | Version/channel/build-number resolution, provenance, invalid values | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-057 | PATH-01 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-058 | PATH-02 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-059 | PATH-03 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-060 | PATH-04 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-061 | PATH-05 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-062 | PATH-06 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-063 | PATH-07 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-064 | PATH-08 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-065 | PATH-09 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-066 | PATH-10 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-067 | PATH-11 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-068 | PATH-12 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-069 | PATH-13 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-070 | PATH-14 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-071 | PATH-15 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-072 | PATH-16 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-073 | PATH-17 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-074 | PATH-18 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-075 | PATH-19 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-076 | PATH-20 | Canonical paths, traversal, protected targets, ownership markers, cleaning | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-077 | STAMP-01 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-078 | STAMP-02 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-079 | STAMP-03 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-080 | STAMP-04 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-081 | STAMP-05 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-082 | STAMP-06 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-083 | STAMP-07 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-084 | STAMP-08 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-085 | STAMP-09 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-086 | STAMP-10 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-087 | STAMP-11 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-088 | STAMP-12 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-089 | STAMP-13 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-090 | STAMP-14 | Temporary PlayerSettings/platform stamping and restoration | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-091 | BUILD-01 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-092 | BUILD-02 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-093 | BUILD-03 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-094 | BUILD-04 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-095 | BUILD-05 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-096 | BUILD-06 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-097 | BUILD-07 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-098 | BUILD-08 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-099 | BUILD-09 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-100 | BUILD-10 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-101 | BUILD-11 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-102 | BUILD-12 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-103 | BUILD-13 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-104 | BUILD-14 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-105 | BUILD-15 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-106 | BUILD-16 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-107 | BUILD-17 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-108 | BUILD-18 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-109 | BUILD-19 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-110 | BUILD-20 | BuildPipeline execution, options, result mapping, cancellation, locks | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-111 | ART-01 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-112 | ART-02 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-113 | ART-03 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-114 | ART-04 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-115 | ART-05 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-116 | ART-06 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-117 | ART-07 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-118 | ART-08 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-119 | ART-09 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-120 | ART-10 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-121 | ART-11 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-122 | ART-12 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-123 | ART-13 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-124 | ART-14 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-125 | ART-15 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-126 | ART-16 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-127 | ART-17 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-128 | ART-18 | Artifact processors, manifest, checksums, mutation ordering, verification | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-129 | REPORT-01 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-130 | REPORT-02 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-131 | REPORT-03 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-132 | REPORT-04 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-133 | REPORT-05 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-134 | REPORT-06 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-135 | REPORT-07 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-136 | REPORT-08 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-137 | REPORT-09 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-138 | REPORT-10 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-139 | REPORT-11 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-140 | REPORT-12 | Receipts, package/license inventory, checklists, redaction, comparison | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-141 | CLI-01 | Batch arguments, active profile constraints, exit codes, report destinations | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-142 | CLI-02 | Batch arguments, active profile constraints, exit codes, report destinations | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-143 | CLI-03 | Batch arguments, active profile constraints, exit codes, report destinations | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-144 | CLI-04 | Batch arguments, active profile constraints, exit codes, report destinations | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-145 | CLI-05 | Batch arguments, active profile constraints, exit codes, report destinations | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-146 | CLI-06 | Batch arguments, active profile constraints, exit codes, report destinations | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-147 | CLI-07 | Batch arguments, active profile constraints, exit codes, report destinations | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-148 | CLI-08 | Batch arguments, active profile constraints, exit codes, report destinations | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-149 | LIFE-01 | Install, repeat setup, removal, reinstall, migration, domain reload | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-150 | LIFE-02 | Install, repeat setup, removal, reinstall, migration, domain reload | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-151 | LIFE-03 | Install, repeat setup, removal, reinstall, migration, domain reload | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-152 | LIFE-04 | Install, repeat setup, removal, reinstall, migration, domain reload | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-153 | LIFE-05 | Install, repeat setup, removal, reinstall, migration, domain reload | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-154 | LIFE-06 | Install, repeat setup, removal, reinstall, migration, domain reload | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-155 | LIFE-07 | Install, repeat setup, removal, reinstall, migration, domain reload | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |
| EBUILD-T-156 | LIFE-08 | Install, repeat setup, removal, reinstall, migration, domain reload | Defined fixture/input | Execute documented operation | Structured expected result for this case | Mixed per implementation | Not run |

All 156 planned tests are `Not run` until implementation produces execution records.

---

## 24. Release Gates and Definition of Done

### 24.1 Specification gate

- [x] Ownership and non-ownership approved.
- [x] Unity Build Profile boundary explicit.
- [x] MVP and deferred providers separated.
- [x] Data, APIs, lifecycle, safety, restoration, diagnostics, Laboratory, and tests defined.
- [x] No release-blocking question remains for specification approval.

### 24.2 Implementation gate

- [ ] Editor-only package compiles with declared dependencies.
- [ ] No runtime assembly or Player dependency exists.
- [ ] Plan/validation/build/report APIs match specification.
- [ ] Output cleaning and settings restoration pass destructive fixtures.
- [ ] Provider registrations are explicit and removable.
- [ ] Public schemas and GUIDs are stable.

### 24.3 Standalone gate

- [ ] Clean-project installation succeeds.
- [ ] Foundry works without another Echo package, Git, network, CI, or deployer.
- [ ] Foundry Laboratory passes.
- [ ] Samples/fixtures can be removed safely.
- [ ] Batch mode and local UI produce equivalent plan inputs.

### 24.4 Quality gate

- [ ] Required automated tests pass with execution records.
- [ ] Manual Laboratory checklist passes.
- [ ] No Blocker/Critical defect remains.
- [ ] Performance and resource targets have measured evidence.
- [ ] Privacy/security/path-safety tests pass.
- [ ] Documentation matches current UI/API/schema.
- [ ] Current Notes reconciled and licenses/notices complete.

### 24.5 Distribution gate

- [ ] Manifest/version/changelog valid.
- [ ] Git/local/tarball install tested independently.
- [ ] At least one real small Player build succeeds through Foundry on a supported target.
- [ ] Failure/cancellation/restoration evidence passes.
- [ ] Receipt/manifest/checksum verification passes.
- [ ] Beta, RC, and stable gates follow SFGSS-004 separately.
- [ ] Compatibility catalog updated with exact tested environments.

---

## 25. Adoption and Migration Plan

### 25.1 Initial integration targets

| Project | Existing system | Replacement strategy | Parity gate | Rollback |
|---|---|---|---|---|
| Echo Systems Lab | Manual Unity builds and portfolio checklist | Create Foundry recipe beside current process; compare artifacts | Same scenes/version/options/output plus receipt | Use original Build Profile/window |
| Rescuers2D | Manual platform/itch build workflow | Add development and release recipes without changing gameplay | Existing build still works and Foundry output launches | Keep manual workflow/profile |
| DeverQuest/package repos | Manual package/readiness release checks | Use package validators and artifact report without absorbing DeverQuest workflow | Tarball/release evidence parity | Keep existing readiness scripts |
| Future Workshop starters | Generated Build Profiles and project setup | Workshop creates optional Foundry recipe through facade | Generated project builds after Workshop removal | Remove Foundry assets/package |

### 25.2 Preserve-until-parity rule

Existing Build Profiles and manual build instructions remain intact. Foundry is introduced as a parallel path, validated against the same target/scenes/options, and becomes the recommended release path only after artifact and launch parity are proven. It never deletes or replaces a Build Profile without explicit setup/repair approval.

### 25.3 Migration tooling

Migration tools may import documented project-owned values into recipes, but must preview every field. They do not infer secrets, vendor deployment policy, or legal compliance. Legacy receipts are copied and migrated on staged data, never rewritten in place.

---

## 26. Risks and Mitigations

| Risk ID | Risk | Likelihood | Impact | Mitigation | Trigger/owner |
|---|---|---|---|---|---|
| EBUILD-R-001 | Foundry duplicates Unity Build Profiles instead of wrapping them | Medium | High | Make BuildProfile the platform/scenes/defines authority; recipes add only release intent | Any duplicated platform setting |
| EBUILD-R-002 | Unsafe output cleaning deletes project or user data | Low | Critical | Canonical path checks, protected roots, ownership marker, exact-leaf deletion, tests | Any recursive delete code |
| EBUILD-R-003 | Temporary PlayerSettings remain changed after failure | Medium | High | Snapshot only touched fields, finally restoration, recovery journal, critical gate | Any mutation before snapshot |
| EBUILD-R-004 | Last-second define changes produce wrong compilation | High | High | Never mutate defines during execute; block and require profile change/domain reload | Define mismatch |
| EBUILD-R-005 | Build receipt claims success before postprocessors/hashes finish | Medium | High | Publication state machine; success only after required processors and manifest | Early success event |
| EBUILD-R-006 | Checksums become stale after external mutation | Medium | Medium | Hash after all registered processors; provide verify command; record boundary | Output changes after manifest |
| EBUILD-R-007 | Secrets leak into recipes, command lines, logs, or reports | Medium | Critical | No secret fields; provider-owned credential stores; redaction and tests | Credential-like key/value |
| EBUILD-R-008 | Batch mode attempts unsupported profile/platform switching | High | Medium | Require process launched with correct target/active profile; validate and block | Non-active target in batch |
| EBUILD-R-009 | Optional validators become hidden hard dependencies | Medium | High | Explicit bridge/provider registration and missing-provider policy | Core reference to peer package |
| EBUILD-R-010 | Build lock survives crash and blocks project forever | Medium | Medium | PID/session/timestamp record, stale inspection, explicit recovery | Stale lock |
| EBUILD-R-011 | Version stamping dirties source unexpectedly | Medium | Medium | Temporary by default; no auto-increment/writeback in MVP | Dirty version asset after build |
| EBUILD-R-012 | License report is mistaken for legal compliance | Medium | High | Report presence/facts only; explicit disclaimer and human review gate | “Compliant” label |
| EBUILD-R-013 | Large output hashing freezes Editor or consumes memory | Medium | Medium | Streaming I/O, progress, bounded concurrency, cancellation before publication | Whole-file buffering |
| EBUILD-R-014 | Manual Unity builds bypass Foundry validation | High | Medium | Document boundary; optional explicit guard later; release process requires Foundry receipt | Build without receipt |
| EBUILD-R-015 | Closed-platform requirements are falsely advertised | Medium | High | Provider-specific docs/evidence; mark unknown until tested | Unsupported platform claim |
| EBUILD-R-016 | Project-specific release policy contaminates package core | Medium | Medium | Templates/providers/project adapters; neutral recipe model | Hard-coded store/studio rules |

---

## 27. Architecture Decisions and Open Questions

### 27.1 Package decisions

| Decision ID | Decision | Status | Reason | Consequences | ADR required? |
|---|---|---|---|---|---|
| EBUILD-D-001 | EchoBuildTools is Editor-only and has no runtime root | Approved | Build preparation is not runtime gameplay | No Player assembly or scene authority | No |
| EBUILD-D-002 | Unity Build Profile assets own target, scenes, profile defines, and platform settings | Approved | Avoid competing build-profile systems | Foundry recipe wraps one explicit profile | No |
| EBUILD-D-003 | A Foundry BuildRecipe owns release intent around one profile | Approved | Need repeatable identity/output/validation policies | Recipe remains project-owned and immutable during execution | No |
| EBUILD-D-004 | Release recipes require explicit BuildProfile assets for MVP | Approved | Active platform profile is insufficiently reproducible | No silent fallback for release builds | No |
| EBUILD-D-005 | Foundry never changes scripting defines during build execution | Approved | Unity applies define changes after recompilation/domain reload | Mismatch blocks before BuildPlayer | No |
| EBUILD-D-006 | Temporary version/platform stamps restore after every attempt | Approved | Builds need metadata without hidden source edits | Snapshot and recovery journal required | No |
| EBUILD-D-007 | MVP does not auto-increment or commit version files | Approved | Avoid dirty project and source-control races | Build number comes from explicit manifest/request/environment provider | No |
| EBUILD-D-008 | Output cleaning requires exact owned leaf or empty target | Approved | Prevent catastrophic deletion | Marker includes project and recipe identity | No |
| EBUILD-D-009 | Build success publishes only after required artifact processors and checksums | Approved | Artifact truth includes final output state | BuildPipeline success alone is not release success | No |
| EBUILD-D-010 | Core does not deploy externally | Approved | Deployment requires provider credentials and platform policy | Separate providers consume successful artifacts | No |
| EBUILD-D-011 | Validator and processor discovery is explicit registration | Approved | Avoid reflection and hidden optional dependencies | Bridges/providers own registrations | No |
| EBUILD-D-012 | All unexecuted build/platform evidence remains Not run | Approved | Documentation cannot prove a build | SFGSS-004 evidence gates apply | No |

### 27.2 Release-blocking questions

None for specification approval. Implementation may begin only after SUITE-DOC-33.

### 27.3 Non-blocking later questions

- Whether the optional external-build guard belongs in core, a project adapter, or a separate package.
- Which exact platform stamp adapters enter the first public release after evidence.
- Whether version reservation/writeback deserves a separate transaction design.
- Which Git metadata provider and deployment providers are approved.
- Whether aggregate multi-recipe builds belong in Foundry core after single-recipe evidence.
- Exact package/license report normalization across embedded, Git, registry, built-in, and local packages.

---

## 28. Milestones and Checkpoint Path

### 28.1 Proposed milestones

| Milestone | Outcome | Included capabilities | Required evidence |
|---|---|---|---|
| M0 - Specification | Approved package contract | Design only | Approved v1.0.0 document |
| M1 - Skeleton | Installable Editor-only package | Manifest, Editor asmdef, tests/docs shell | Clean compile/remove/readd |
| M2 - Planning and validation core | Recipe, plan, fingerprint, validators, output safety | Pure/EditMode tests |
| M3 - Build execution | Stamp snapshot, BuildPipeline, restoration, receipts | Player build fixtures |
| M4 - Artifact publication | Processors, inventory, manifest, checksums, checklists | Artifact verification fixtures |
| M5 - Laboratory and CLI | Editor Laboratory, batch runner, failure simulations | Manual/automated registry evidence |
| M6 - Integrations | Workshop facade and first peer validator bridge | Integration Lab evidence |
| M7 - Release | Docs, package, tarball/Git install, external clean-project proof | SFGSS-004 gates |

### 28.2 Checkpoint rule

Every milestone becomes a small SFGSS-005 Checkpoint Build Plan with visible complete code, explanation, exact Unity Editor steps, tests, recovery, stop point, documentation reconciliation, and commit guidance. No code checkpoint begins before SUITE-DOC-33.

### 28.3 First recommended checkpoint

After the final documentation gate: **EBUILD-M1-01 - Editor-only package skeleton**. It creates only manifest, Editor/test asmdefs, docs shell, and clean installation evidence. No BuildPipeline logic or destructive path code enters the skeleton checkpoint.

---

## 29. New-Conversation Handoff

```text
We are continuing The Sperk’s Forge package-first documentation program.

Treat SFGSS-000 through SFGSS-005 as suite standards and this approved
Foundry Specification as the Level 2 authority for EchoBuildTools.

Package: EchoBuildTools / The Foundry
Specification: v1.0.0 Approved
Implementation: locked until SUITE-DOC-33
Authority: Editor-only build recipes, planning, validation, temporary stamping,
safe output, Unity Player build execution, receipts, manifests, checksums,
and release evidence.

Before any future code:
1. Preserve Unity Build Profiles as the target/scenes/defines authority.
2. Do not create a runtime root or runtime assembly.
3. Do not store credentials or deploy externally from core.
4. Keep output cleaning exact, owned, and test-first.
5. Show all code and explain every implementation step when the gate opens.
```

### 29.1 Current status record

| Field | Current value |
|---|---|
| Package version | Specification v1.0.0; implementation version not started |
| Completed checkpoint | SUITE-DOC-08 specification |
| Files/assets created | Documentation only |
| Tests passed | None; all planned tests Not run |
| Tests failed | None; no execution |
| Known issues | Implementation/platform evidence pending |
| Decisions added | EBUILD-D-001 through EBUILD-D-012 |
| Next checkpoint | SUITE-DOC-09 - Many Tongues (`EchoLocalization`) specification |

---

## 30. Approval

### 30.1 Approval checklist

- [x] Build preparation authority is clear and Editor-only.
- [x] Unity Build Profile ownership is preserved.
- [x] Output cleaning, version stamping, restoration, and receipt boundaries are explicit.
- [x] Providers/bridges remain optional and removable.
- [x] MVP is complete without external deployment or credentials.
- [x] Laboratory and planned evidence are measurable and honest.
- [x] No Isekai Studios ownership or identity introduced.
- [x] Jesse’s package-first documentation gate remains locked.

### 30.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** 2026-08-04  
**Conditions or notes:** Implementation remains prohibited until SUITE-DOC-33. Exact Unity/platform compatibility, performance, real build, restoration, and provider evidence remain `Not run`.

---

## Template Completion Rule

A new collaborator can identify what Foundry owns, how it differs from Unity Build Profiles, how plans and fingerprints work, what may be mutated, how output deletion is constrained, how a build is validated and executed, when success is published, how evidence is generated, how providers connect, and what remains untested. The specification is therefore complete as a pre-code Level 2 authority.
