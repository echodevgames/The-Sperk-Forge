# The Sperk’s Forge – Repository, Versioning, and Integration Workspace Standard

**Document ID:** SFGSS-009  
**Version:** 1.1.0
**Status:** Approved repository and release-governance standard  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Project boundary:** Independent solo project; not an Isekai Studios product  
**Parent authority:** SFGSS-000 v0.24.0
**Related authorities:** SFGSS-002, SFGSS-003, SFGSS-004 v1.3.0, SFGSS-005, SFGSS-007, SFGSS-008, SFGSS-ADR-001 through SFGSS-ADR-005, and the Foundation, Expansion, and Advanced integration matrices
**Current development baseline:** Unity 6000.3.8f1  
**Initial public Unity floor:** Unity 6000.0  
**Last updated:** August 7, 2026

> Give every artifact one home, every release one immutable marker, and every compatibility claim one reproducible workspace.

---

## Contents

1. Purpose and authority
2. Scope and non-goals
3. Terminology
4. Governing principles
5. Approved repository topology
6. Canonical repository registry
7. Package repository anatomy
8. Central suite catalog repository
9. Integration Lab repository
10. Local development workspace layout
11. Branch strategy
12. Commit and checkpoint policy
13. Semantic versioning policy
14. Pre-release channels and stability labels
15. Package manifest version policy
16. Tags and GitHub releases
17. Git dependency policy
18. Registry dependency and Git-only incubation constraints
19. Local-path, embedded, tarball, and Git development routes
20. Project manifests and lock files
21. Compatibility catalog and integration snapshots
22. Cross-package release coordination
23. Bridge and provider repository/versioning policy
24. Support lines, hotfixes, deprecation, and end of support
25. Repository protection, permissions, and secrets
26. Large files, Git LFS, binaries, and generated artifacts
27. Continuous integration and release automation design
28. Release artifact and tarball policy
29. Clone, setup, update, and recovery workflows
30. Archival, transfer, rename, and repository removal
31. Validation and release gates
32. Reconciliation findings
33. Approval

---

## 1. Purpose and authority

SFGSS-009 is the canonical repository, versioning, release-tag, package-source, integration-workspace, and compatibility-record standard for **The Sperk’s Forge – EchoDevGames Game Systems Suite**.

The suite deliberately uses many independently distributable packages. Without one repository standard, that independence could decay into inconsistent branches, mutable Git references, copied package folders, mismatched package versions, machine-specific paths, or an Integration Lab that passes only on the computer that assembled it. This document turns the hybrid multi-repository decision in SFGSS-000 into a reproducible operating model.

This standard answers:

- Which repository owns each package and release?
- How are package repositories named and structured?
- How are package versions, tags, releases, and changelogs related?
- How does a project consume an unpublished Git package safely?
- How are exact cross-package combinations reproduced?
- What belongs in the central suite repository versus the Integration Lab?
- When may local paths, submodules, embedded packages, tarballs, or Git URLs be used?
- How are branches, hotfixes, deprecations, compatibility snapshots, and archived repositories handled?

### 1.1 Authority order

When repository or versioning documents disagree, use this order:

1. SFGSS-000 for suite ownership, package boundaries, and the hybrid repository model.
2. The approved package specification for package-local public API and release obligations.
3. This standard for repository topology, branch/tag/version policy, package-source routes, and integration workspace behavior.
4. SFGSS-002 for dependency and assembly direction.
5. SFGSS-003 for durable data compatibility and migration.
6. SFGSS-004 for evidence and release gates.
7. SFGSS-007 and an accepted ADR that explicitly approves an exception.
8. Integration specifications, release plans, guides, reports, and Current Notes.

A repository convenience must never reverse package authority, hide a dependency, or weaken migration and release evidence.

### 1.2 Requirement language

- **Must** is release-blocking.
- **Must not** is prohibited unless a higher authority or accepted ADR grants an explicit exception.
- **Should** is the default; a deviation requires a recorded reason.
- **May** is optional.

---

## 2. Scope and non-goals

### 2.1 This standard governs

- Central, package, bridge, provider, documentation, and integration repositories.
- Repository names, ownership, visibility, lifecycle, and release responsibility.
- Git branches, commits, annotated tags, releases, and hotfix lines.
- Package Semantic Versioning and pre-release channels.
- UPM Git, registry, local-path, embedded, and tarball consumption.
- Unity project `manifest.json` and `packages-lock.json` policy.
- Integration workspace pinning and compatibility snapshots.
- Repository protection, secrets, large files, CI design, archives, and transfer.
- Cross-package release and support coordination.

### 2.2 This standard does not govern

- Public API design, which belongs to package specifications.
- Dependency/assembly direction, which belongs to SFGSS-002.
- Save or settings schema versions, which belong to SFGSS-003.
- Test execution states and release evidence, which belong to SFGSS-004.
- Deployment credentials, storefront accounts, signing keys, or provider secrets.
- A mandatory hosted package registry. Registry publication is a later distribution decision.
- Actual CI scripts or package implementation before the documentation gate opens.

---

## 3. Terminology

| Term | Meaning |
|---|---|
| **Central suite repository** | The documentation, catalog, roadmap, compatibility, and package-discovery repository for The Sperk’s Forge. It is not a runtime dependency. |
| **Package repository** | The Git repository whose root is one independently releasable UPM package. |
| **Bridge repository** | A repository whose root package connects two or more independent package authorities. |
| **Provider repository** | A repository whose root package connects a provider-neutral core to a vendor, backend, platform, or service. |
| **Integration Lab repository** | A Unity project that checks exact package combinations, bridges, providers, pathways, removal, migration, and release compatibility. |
| **Workspace** | A local directory containing the central repository, Integration Lab, and package clones/submodules in a known relative layout. |
| **Release commit** | The exact commit whose package manifest, changelog, documentation, tests, and notices represent a release. |
| **Annotated release tag** | An immutable signed or annotated Git tag naming a released package version. |
| **Compatibility snapshot** | A retained Integration Lab state that records exact package sources, revisions, Unity version, tests, and evidence. |
| **Support line** | A maintained major or minor release series receiving approved fixes. |
| **Git dependency** | A project-level UPM dependency fetched from a Git URL and pinned by Unity’s lock file. |
| **Registry dependency** | A package version resolved from an approved UPM-compatible registry. |
| **Local dependency** | A project manifest entry referencing a package folder or tarball path. |
| **Embedded package** | A mutable package copied into a Unity project’s `Packages/` folder. |
| **Source revision** | A branch, tag, or commit hash used by a Git dependency. |
| **Release channel** | Alpha, beta, release candidate, stable, or another explicitly registered pre-release stage. |

---

## 4. Governing principles

### 4.1 One releasable artifact, one owning repository

A package, bridge, or provider artifact receives one owning repository and one independent version history. The central suite repository catalogs releases but does not impersonate their source history.

### 4.2 Package versions are independent

The suite has no synchronized global runtime version. `EchoLaunch 1.2.0` may coexist with `Jukebot 0.8.1` and `EchoUI 1.0.3` when the compatibility catalog and evidence support that combination.

### 4.3 Released revisions are immutable

A released tag must never be moved, deleted, or reused to point at different content. A faulty release is corrected by a new version.

### 4.4 Reproducibility beats “latest”

Consumer projects, compatibility snapshots, and release tests pin exact package versions, tags, or commits. A default branch without an explicit revision is a development convenience, not a release dependency.

### 4.5 Git history is evidence, not a package registry

Git installation is approved for incubation, testing, and direct distribution, but it does not provide transitive Git dependencies between packages. Git-only packages that require peers must be selected explicitly by the consuming project or installed through a visible Workshop plan.

### 4.6 The Integration Lab proves composition

Individual package repositories prove standalone behavior. The Integration Lab proves selected combinations, bridges, providers, pathways, upgrades, and clean removal.

### 4.7 Machine-specific paths do not enter shared history

Committed manifests, scripts, and documentation must not contain absolute local paths, usernames, drive letters, credentials, or device-specific workspace state.

### 4.8 Documentation and release state travel together

A package release commit includes matching manifest version, changelog, public documentation, tests, licenses/notices, and migration guidance. Documentation-only planning may advance separately before implementation, but a release cannot knowingly ship stale API documentation.

### 4.9 Repository complexity must earn itself

Submodules, long-lived release branches, multiple package roots, registries, CI matrices, and provider repositories are introduced only when their reproducibility or distribution value exceeds their maintenance cost.

---

## 5. Approved repository topology

The approved topology is:

```text
EchoDevGames organization/account
├── <central suite repository>
│   ├── Plan Documentation/
│   ├── package catalog
│   ├── compatibility catalog
│   ├── guided pathways
│   └── links to package, bridge, provider, and Integration Lab repositories
├── Sperks-Forge-Integration-Lab
│   └── Unity project that pins exact package combinations
├── EchoLaunch
├── EchoDiagnostics
├── EchoSettings
├── ... one repository per major package ...
├── Jukebot
├── <PackageA>-<PackageB>-Bridge
└── <NeutralPackage>-<Provider>-Adapter
```

### 5.1 Central repository

The current documentation repository is the central suite repository regardless of its present GitHub slug. Its canonical public slug remains **to be confirmed from the actual remote** and must not be invented from documentation alone. The preferred public slug is `The-Sperks-Forge` if no existing repository identity must be preserved.

### 5.2 Package repositories

The canonical planned package repository slug is the technical identifier from SFGSS-008:

- `EchoLaunch`
- `EchoDiagnostics`
- `EchoSettings`
- `Jukebot`
- and so on through all twenty-eight package foundations.

The package repository root should be the UPM package root unless an accepted ADR approves a multi-package repository exception.

### 5.3 Integration Lab

The preferred repository slug is `Sperks-Forge-Integration-Lab`. The actual remote is recorded when created and must not be inferred before then.

### 5.4 No runtime dependency on repository topology

A game consumes package artifacts. It does not require the central documentation repository, the Integration Lab repository, Git submodules, or the development workspace layout at runtime.

---

## 6. Canonical repository registry

`SFGSS-009_Repository_Registry.json` is the machine-readable companion to this standard. It records:

- Technical identifier
- Package ID
- Repository owner
- Planned repository slug
- Repository class
- UPM-root policy
- Current creation/publication state
- Release-tag pattern
- Default branch

The Markdown standard remains authoritative.

### 6.1 Repository classes

| Class | Default root contents | Versioned artifact |
|---|---|---|
| Central suite | Documentation/catalog | Catalog and documentation baseline |
| Runtime package | UPM package at repository root | One package |
| Editor package | UPM package at repository root | One Editor package |
| Bridge | UPM bridge package at repository root | One bridge |
| Provider adapter | UPM adapter package at repository root | One provider adapter |
| Integration Lab | Unity project | Compatibility snapshot, not a runtime package |
| Research prototype | Disposable project/repository | Evidence only, never production package by default |

### 6.2 Repository states

- **Planned** — name reserved in documentation; repository may not exist.
- **Incubating** — repository exists but no public compatibility promise.
- **Alpha** — early public/testing releases; breaking changes expected and documented.
- **Beta** — MVP substantially present; migration and release evidence still maturing.
- **Release Candidate** — intended stable content under final evidence review.
- **Stable** — documented public API and release support policy.
- **Maintenance** — no planned feature growth; supported fixes only.
- **Archived** — read-only historical repository; replacement or end-of-support notice required.

Repository state and package version are related but not identical.

---

## 7. Package repository anatomy

The default package repository is itself a valid UPM package at its root:

```text
<PackageRepository>/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Third Party Notices.md
├── Runtime/
├── Editor/
├── Documentation~/
│   ├── Index.md
│   ├── User/
│   └── Developer/
│       ├── Architecture.md
│       ├── Current Notes.md
│       ├── ADR/
│       └── Checkpoints/
├── Samples~/
├── Tests/
├── .gitignore
├── .gitattributes
└── .github/                 optional when the repository uses GitHub workflows/templates
```

### 7.1 Root-package rule

The package lives at repository root so consumers can use a simple Git URL. A `?path=` Git dependency is allowed only for an approved multi-package repository, compatibility harness, or migration exception.

### 7.2 Development-only project content

A package repository must not hide a Unity game project around the package root. Package development and cross-package scene testing belong in the Integration Lab or a disposable fixture project.

### 7.3 Repository documentation

Each active package repository maintains:

- A concise root README.
- `Documentation~` user and developer guidance.
- A linked `Current Notes.md` development page.
- Changelog and migration notes.
- License and third-party notices.
- Current package status and compatibility links.

### 7.4 Stable Unity asset identity

Public `.meta` files are committed. Moves and renames preserve GUIDs when identity is intended to survive. Generated or user-specific cache files are ignored.

---

## 8. Central suite catalog repository

The central repository owns:

- SFGSS-000 through SFGSS-010.
- The package catalog and naming registry.
- Suite ADR log and suite-wide ADRs.
- Guided pathways.
- Foundation, Expansion, Advanced, and final integration matrices.
- Suite Graph Roadmap and learning catalog.
- Compatibility catalog and links to package releases.
- Documentation checkpoints and readiness reports.

It does not own:

- Package runtime source.
- Package release tags.
- Provider SDKs.
- Game-specific assets.
- A shared suite runtime version.

### 8.1 Central documentation tags

The central repository may create annotated documentation/catalog tags using:

```text
catalog-vMAJOR.MINOR.PATCH
```

These tags identify a suite documentation/catalog baseline. They do not imply that every package shares that version.

### 8.2 Central repository releases

A central GitHub release, when used, contains documentation bundles, catalog snapshots, compatibility summaries, or learning materials. It does not redistribute package source unless the package licenses and release plan explicitly allow it.

---

## 9. Integration Lab repository

The Integration Lab is a real Unity project whose purpose is reproducible composition evidence.

### 9.1 It owns

- Exact selected package sources and revisions.
- Project `Packages/manifest.json` and `packages-lock.json`.
- Foundation, Expansion, Advanced, pathway, bridge, provider, upgrade, and removal fixtures.
- Integration Laboratories and showcase scenes.
- Compatibility execution records.
- Clean clone/setup instructions.
- No production game content beyond redistributable test fixtures.

### 9.2 It does not own

- Package runtime authority.
- Package release versions.
- User projects.
- Provider credentials.
- Standalone package proof that belongs to package repositories.

### 9.3 Canonical shared mode

The committed Integration Lab uses exact Git tags, registry versions, tarballs, or commit hashes that another machine can resolve. It must not commit machine-local `file:` paths.

### 9.4 Editable local-package mode

For active cross-package development, a developer may switch selected dependencies to relative local paths or submodule checkouts in a documented local workspace. Those edits are local or live on a dedicated development branch and must not be confused with the canonical compatibility snapshot.

### 9.5 Compatibility snapshots

A retained snapshot records:

- Snapshot ID and tag.
- Unity Editor version.
- OS and build target where relevant.
- Project manifest and lock file hashes.
- Direct package sources and revisions.
- Bridge/provider versions.
- Executed test records and evidence links.
- Known failures, advisories, and unsupported combinations.

Integration Lab snapshot tags use:

```text
compat-YYYY.MM.DD.N
```

where `N` distinguishes multiple snapshots on one date.

---

## 10. Local development workspace layout

The recommended local layout is:

```text
SperksForgeWorkspace/
├── The-Sperks-Forge/                 central documentation/catalog clone
├── Sperks-Forge-Integration-Lab/     Unity integration project clone
└── Packages/
    ├── EchoLaunch/
    ├── EchoDiagnostics/
    ├── EchoSettings/
    ├── Jukebot/
    └── ... additional package, bridge, or provider clones ...
```

### 10.1 Relative paths only

Local-path manifests use relative paths from the Integration Lab project to the workspace package clones. No absolute path, drive letter, username, or machine-specific symlink target is committed.

### 10.2 Sibling clones as the novice-friendly default

Independent sibling clones are the default local development model because they remain ordinary Git repositories and are easier to inspect, commit, push, and recover than nested repository machinery.

### 10.3 Submodules as an optional pinned workspace mode

The Integration Lab may use Git submodules when exact package commits and one-command workspace reconstruction justify the extra maintenance. A submodule is never required for package consumers. If adopted, the repository must include initialization/update instructions and verify that submodule pointers match the compatibility snapshot.

### 10.4 Worktrees

Git worktrees may be used inside one package repository to maintain a release/hotfix branch beside active development. Worktrees do not combine separate package repositories and do not replace the Integration Lab.

---

## 11. Branch strategy

### 11.1 Default branch

Every active repository uses `main` as its default branch unless an existing repository requires a documented migration.

### 11.2 Main branch expectation

`main` should remain:

- Compilable for its declared package state.
- Internally consistent with its documentation.
- Free of known release blockers that are not prominently recorded.
- Protected against accidental force-push and deletion when repository tooling permits.

`main` need not represent a stable public release during incubation.

### 11.3 Short-lived work branches

Preferred patterns:

```text
feature/<checkpoint-or-topic>
fix/<issue-or-topic>
docs/<checkpoint-or-topic>
research/<topic>
release/<major.minor>
hotfix/<version-or-issue>
```

Branches are short-lived by default. The suite does not require a permanent `develop` branch.

### 11.4 Release branches

A `release/<major.minor>` branch is created only when:

- A release candidate needs stabilization while later work continues.
- A supported release line needs patch maintenance.
- Parallel provider/platform certification requires a frozen line.

Do not create empty ceremonial release branches.

### 11.5 Force-push and history rewriting

- Force-push is prohibited on protected `main`, release branches, and released tags.
- Private short-lived branches may be rebased before merge.
- Published history is rewritten only through an approved recovery/security procedure.

---

## 12. Commit and checkpoint policy

### 12.1 Commit shape

Commits should be small enough to explain one coherent change and large enough to remain buildable or honestly documented.

Preferred summary pattern:

```text
<type>(<scope>): <imperative summary>
```

Recommended types:

- `feat`
- `fix`
- `docs`
- `test`
- `refactor`
- `build`
- `chore`
- `research`
- `release`

Example:

```text
docs(sfgss-009): approve repository and versioning standard
```

### 12.2 Checkpoint identification

The commit body or adjacent checkpoint record includes the checkpoint ID, tests/evidence state, and any intentionally deferred work.

### 12.3 Documentation adjacency

Behavior and its documentation should enter the same commit when practical. Otherwise use an immediately adjacent clearly labeled documentation commit, as required by SFGSS-000 and SFGSS-005.

### 12.4 Generated bulk changes

Generated registries, manifests, and link updates must be reviewed and committed with the source authority that generated them. A generated diff does not excuse unreadable history.

### 12.5 Secrets and private data

Commits must not contain credentials, tokens, private keys, account IDs, provider tickets, personal file paths, private save contents, or confidential platform documentation.

---

## 13. Semantic versioning policy

All released UPM packages follow Semantic Versioning 2.0.0:

```text
MAJOR.MINOR.PATCH[-PRERELEASE][+BUILD]
```

### 13.1 Initial development

- Begin implementation at `0.1.0` unless a package specification approves a different starting point.
- Before `1.0.0`, a **minor** increase may contain a breaking public change.
- Before `1.0.0`, a **patch** remains backward-compatible within that minor line unless an emergency security correction documents otherwise.

### 13.2 Stable releases

After `1.0.0`:

- **MAJOR**: incompatible public API, serialization, setup output, package identity, required dependency, or migration behavior.
- **MINOR**: backward-compatible capability, optional API, new provider seam, sample, or nonbreaking configuration extension.
- **PATCH**: backward-compatible defect, documentation correction, test fix, performance improvement, or internal refactor.

### 13.3 Public compatibility surface

SemVer applies to more than C# signatures. Breaking surfaces include:

- Public runtime and Editor APIs.
- Serialized field/type identity.
- Stable IDs and aliases.
- Package/assembly names.
- Configuration asset semantics.
- Setup/repair output that consumers depend on.
- Save/settings/receipt formats.
- Sample contracts advertised as supported.
- Required dependencies and minimum versions.
- Removal and migration guarantees.

### 13.4 Documentation versions

SFGSS document versions are independent from package release versions. Updating a design document from v1.0.0 to v1.1.0 does not release package v1.1.0.

---

## 14. Pre-release channels and stability labels

Approved package pre-release identifiers:

```text
alpha.N
beta.N
rc.N
```

Examples:

```text
0.1.0-alpha.1
0.5.0-beta.2
1.0.0-rc.1
```

### 14.1 Alpha

- Architecture or API may still change substantially.
- Standalone core proof may be incomplete.
- Migration compatibility may be limited.

### 14.2 Beta

- MVP is substantially implemented.
- Private or public testing is underway against a reproducible artifact.
- Real-project integration may remain deferred unless the beta advertises a
  named adoption, adapter, bridge, or parity claim.
- Known limitations and migration behavior are documented.

### 14.3 Release candidate

- Intended stable API and artifact.
- Only release-blocking fixes, evidence, documentation, and packaging changes should enter.

### 14.4 Stable

- Stable release gates in SFGSS-004 pass.
- Public API and durable formats carry the documented SemVer promise.

### 14.5 Channel reset

A new major/minor line starts its own pre-release counter. Released pre-release identifiers are never reused for different content.

---

## 15. Package manifest version policy

For a release commit:

- `package.json` `name` matches SFGSS-008.
- `version` exactly matches the release version without the Git tag’s leading `v`.
- `displayName`, description, Unity floor, dependencies, licenses/notices, documentation URL, and changelog are current.
- The manifest contains no machine-local path, branch name, or unreleased peer Git URL.

### 15.1 Version consistency

For tag `v1.2.3`, the package manifest must contain:

```json
"version": "1.2.3"
```

A mismatch blocks the release.

### 15.2 Unity version field

The manifest’s Unity floor records the minimum supported line, not merely the newest Editor used by the maintainer. Compatibility beyond that floor remains `Planned`, `Tested`, or `Supported` according to SFGSS-004 evidence.

### 15.3 Dependency versions

Registry dependencies in package manifests use concrete minimum versions supported by Unity’s package dependency model and SFGSS-002. Broader compatibility ranges belong in the compatibility catalog and require evidence.

---

## 16. Tags and GitHub releases

### 16.1 Package release tags

Package, bridge, and provider repositories use annotated release tags:

```text
vMAJOR.MINOR.PATCH[-PRERELEASE]
```

Annotated tags are preferred because they carry tagger identity, date, and a release message. Signed tags may be adopted when key management is ready.

### 16.2 Tag contents

A release tag points to a commit containing:

- Matching package manifest version.
- Updated changelog.
- Current documentation and migration notes.
- Licenses/notices.
- Required test and release reports.
- Stable `.meta` files.
- No known unrecorded blocker.

### 16.3 GitHub releases

A GitHub release is created from the annotated tag and may contain:

- Release notes.
- Known limitations.
- Compatibility summary.
- Installation examples.
- Checksummed `.tgz` package artifact when produced.
- Migration and rollback notes.

GitHub release state does not replace package tests or the tag.

### 16.4 Tag protection

When repository tooling permits, `v*`, `catalog-v*`, and `compat-*` tags are protected from deletion and force-updates.

---

## 17. Git dependency policy

Unity project Git dependencies live in the project’s `Packages/manifest.json`, not inside another package’s `package.json`.

Approved forms:

```json
"com.echodevgames.echo-launch": "https://github.com/echodevgames/EchoLaunch.git#v1.0.0"
```

or, for a deliberate evidence snapshot:

```json
"com.echodevgames.echo-launch": "https://github.com/echodevgames/EchoLaunch.git#<full-commit-hash>"
```

### 17.1 Release consumption

- Stable/release-candidate use pins an annotated tag or exact commit.
- Branch references are allowed only for active development or disposable research.
- Omitting a revision and following the default branch is prohibited for compatibility claims and release evidence.

### 17.2 Commit hashes

A compatibility snapshot records the full resolved commit hash even when the manifest uses a tag. Unity’s lock file is committed so resolution is reproducible.

### 17.3 Subfolder syntax

`?path=` is prohibited for normal package repositories because the package is at repository root. It may be used only for an approved multi-package repository or research fixture.

### 17.4 Git LFS prerequisites

A Git package repository using LFS must declare the requirement. Consumers without Git LFS may receive pointer files rather than package content, so LFS-backed package releases require explicit clean-install evidence.

---

## 18. Registry dependency and Git-only incubation constraints

Unity does not support Git dependencies between packages. A package manifest cannot declare a peer by Git URL.

Therefore, during Git-only incubation:

- Core packages remain independent as already required.
- A bridge/provider’s required peer package names and versions remain in its `package.json`, but the consuming project must make those packages available from an approved registry or install them explicitly.
- The Workshop dry-run lists every peer, bridge, and provider operation.
- Installation guides show the complete project-level manifest set.
- A Git-installed bridge must not pretend its Git peers are automatically transitive.

### 18.1 Registry promotion

Before publishing interdependent package families to a scoped registry, the release process must verify:

- Exact package names and versions.
- Registry ownership and access controls.
- Transitive dependency resolution.
- Package immutability.
- Removal and upgrade behavior.
- License/notices availability.
- Compatibility catalog synchronization.

A registry provider and publishing workflow require an ADR before becoming the suite’s public default.

---

## 19. Local-path, embedded, tarball, and Git development routes

| Route | Approved purpose | Commit policy | Release claim |
|---|---|---|---|
| Relative local path | Active package development in known workspace | Local/dev branch only unless path is portable fixture | Development evidence only |
| Embedded package | Temporary debugging/customization in a disposable or migration project | Never mistaken for upstream package release | No upstream release claim |
| Tarball | Distribution and clean-install proof | Artifact and checksum retained in release evidence | Allowed when tested |
| Git tag | Direct package distribution/incubation | Project manifest and lock file committed | Allowed when tested |
| Git commit | Exact compatibility/research snapshot | Full hash retained | Experimental/test evidence |
| Registry version | Mature distribution and transitive dependency resolution | Manifest and lock file committed | Allowed when registry evidence passes |

### 19.1 Relative local paths

Only relative paths may be committed, and only inside a repository designed around that portable relative layout. Personal absolute paths remain untracked.

### 19.2 Embedded packages

Embedding changes package source ownership. The project must document whether it is a temporary debug copy, a project fork, or a migration experiment. Improvements intended for the suite return to the owning package repository through normal commits.

### 19.3 Tarballs

A release tarball is generated from the release commit, not from an uncommitted workspace. Its contents and checksum are retained with the release report.

---

## 20. Project manifests and lock files

Unity projects commit:

- `Packages/manifest.json`
- `Packages/packages-lock.json`

unless a platform/provider policy explicitly prohibits some generated secret-bearing content, which normal Unity package manifests do not contain.

### 20.1 Why the lock file is committed

The lock file records resolved dependency revisions and supports deterministic restoration. Deleting it to “fix” resolution is a diagnosed recovery action, not routine housekeeping.

### 20.2 Manifest review

Every direct package addition, removal, source change, revision change, scoped registry change, or local-path substitution is reviewed as architecture-affecting project configuration.

### 20.3 Lock drift

Unexpected lock-file changes require investigation. A release/compatibility commit must not contain unrelated dependency churn.

### 20.4 Local override hygiene

Before producing compatibility evidence or a consumer release test, replace local-path or mutable-branch dependencies with the exact claimed source route and regenerate/verify the lock file.

---

## 21. Compatibility catalog and integration snapshots

The central suite repository maintains a compatibility catalog. At minimum each record contains:

- Package/artifact IDs and versions.
- Source tags/commit hashes.
- Unity version.
- Required Unity/third-party package versions.
- Bridge/provider versions.
- Test environment.
- Evidence state and report links.
- Known limitations.
- Upgrade and removal notes.

### 21.1 Compatibility is multidimensional

A package version is not simply “compatible with the suite.” Claims may differ by:

- Unity version.
- Operating system.
- Build target.
- Peer package version.
- Bridge/provider version.
- Installation route.
- Existing-project migration path.

### 21.2 Catalog truth

Only observed evidence may be marked `Tested` or `Supported`. Planned combinations remain `Planned` or `Unknown`.

### 21.3 Snapshot immutability

Once a compatibility snapshot is published, its source revisions and evidence links do not change. New evidence creates a new snapshot or an explicitly versioned report correction.

---

## 22. Cross-package release coordination

### 22.1 No synchronized version bump

A package release does not force unrelated package version changes.

### 22.2 Bridge compatibility

A bridge release documents the peer versions it was tested against. If a peer introduces a breaking major version, the bridge receives a compatible release or remains explicitly incompatible.

### 22.3 Coordinated release plan

When multiple artifacts must ship together:

1. Prepare each artifact in its owning repository.
2. Test exact release candidates in the Integration Lab.
3. Record the compatibility snapshot.
4. Tag each artifact independently.
5. Publish package releases.
6. Update the central compatibility catalog.

### 22.4 Release order

Provider and bridge packages are published only after their required neutral cores and peers are available at the referenced versions.

---

## 23. Bridge and provider repository/versioning policy

### 23.1 Separate repository trigger

A bridge/provider receives its own repository when it has any of these:

- Direct dependencies on two optional Echo packages.
- Vendor SDK or platform dependency.
- Independent license/notices.
- Independent compatibility or release cadence.
- Separate sample/Integration Laboratory.
- Clean removal or provider-selection importance.

### 23.2 Naming

Preferred repository slugs:

```text
<PackageA>-<PackageB>-Bridge
<NeutralPackage>-<Provider>-Adapter
```

The exact public slug and package ID are registered through SFGSS-008 and SFGSS-002 before release.

### 23.3 Version independence

Bridge/provider versions are independent from every peer. A bridge does not inherit the version of either connected package.

### 23.4 Provider SDK version changes

A provider SDK major-version change may require a bridge/provider major release even when the neutral core is unchanged.

### 23.5 Provider retirement

Retiring a provider adapter must not deprecate the provider-neutral core. The central catalog records replacement options and migration status.

---

## 24. Support lines, hotfixes, deprecation, and end of support

### 24.1 Default support policy

Before stable `1.0.0`, the package normally supports only the latest released minor line unless a release record states otherwise.

After `1.0.0`, the package specification or release policy states how many major/minor lines are maintained. The default solo-maintainer policy is:

- Current stable major/minor: active fixes.
- Previous stable minor on the same major: critical/blocker fixes when practical.
- Older lines: unsupported unless explicitly listed.

### 24.2 Hotfix flow

A hotfix starts from the affected release tag or support branch, receives the smallest safe correction and regression evidence, then releases a new patch version.

### 24.3 Deprecation

Deprecation records:

- Replacement.
- Warning behavior.
- Migration instructions.
- Earliest removal version.
- Data/asset compatibility behavior.

### 24.4 End of support

An unsupported release remains available unless security, legal, or licensing concerns require withdrawal. Documentation clearly labels support state and replacement path.

---

## 25. Repository protection, permissions, and secrets

### 25.1 Branch and tag protection

When supported by the hosting plan, use repository rulesets or protection rules to prevent deletion/force-push of:

- `main`
- Active release branches
- `v*` package tags
- `catalog-v*` documentation tags
- `compat-*` Integration Lab tags

### 25.2 Required review

As a solo project, Jesse may self-approve releases, but protected workflows should still require successful status checks when CI exists. Collaborator repositories may add pull-request review requirements.

### 25.3 Least privilege

Provider, registry, release, and CI credentials receive the minimum permissions necessary and are stored in approved secret stores, never repository files.

### 25.4 Security incidents

A leaked secret is revoked immediately. Git history rewriting is not a substitute for revocation. A security incident record identifies affected repositories, releases, and consumer guidance.

---

## 26. Large files, Git LFS, binaries, and generated artifacts

### 26.1 Source-first repositories

Package repositories should remain source-oriented. Avoid committing:

- Unity `Library/`, `Temp/`, `Logs/`, and build outputs.
- IDE caches.
- Generated packages/tarballs beside source.
- Copyrighted or non-redistributable media.
- Large demo assets that do not prove the package contract.

### 26.2 Git LFS

Git LFS may be used for large redistributable sample assets only when:

- The asset is necessary.
- Licensing allows redistribution.
- LFS bandwidth/storage cost is understood.
- Package installation from Git and tarball is tested with LFS.
- Pointer-file failure is documented.

### 26.3 Release artifacts

Generated tarballs, checksums, compatibility exports, and build outputs attach to releases or retained evidence storage. They are not committed repeatedly to package source history unless the repository is explicitly an artifact repository.

---

## 27. Continuous integration and release automation design

No CI implementation is authorized by this standard, but future automation must preserve these stages:

1. Validate repository and package structure.
2. Validate manifest/version/tag consistency.
3. Restore exact dependencies and lock state.
4. Compile package assemblies.
5. Execute required tests and Laboratories supported by automation.
6. Validate documentation, links, IDs, changelog, licenses, and notices.
7. Build a tarball from the exact release commit.
8. Verify tarball installation in a clean project.
9. Produce checksums, reports, and artifact manifest.
10. Require human approval before public release unless an ADR approves another model.

### 27.1 CI does not create truth retroactively

A green workflow supports evidence. It does not repair an unapproved API, missing migration, incorrect ownership boundary, or undocumented breaking change.

### 27.2 Matrix growth

CI matrices grow from observed support commitments, not theoretical platform lists. Unsupported or untested environments remain labeled honestly.

---

## 28. Release artifact and tarball policy

### 28.1 Artifact source

A package tarball is produced from the tagged release commit with a clean working tree.

### 28.2 Naming

Preferred filename:

```text
<package-id>-<version>.tgz
```

Example:

```text
com.echodevgames.echo-launch-1.0.0.tgz
```

### 28.3 Required artifact evidence

- SHA-256 checksum.
- File size.
- Source commit and tag.
- Package manifest version.
- Creation tool/version.
- Clean-project install result.
- Contents/notice validation.

### 28.4 Rebuilding

If a release artifact is rebuilt, it must reproduce the same content or receive a new package version. Never replace an existing release asset silently with different bytes.

---

## 29. Clone, setup, update, and recovery workflows

### 29.1 Package repository clone

1. Clone the package repository.
2. Confirm the expected branch/tag.
3. Read README, package specification, Current Notes, and active checkpoint.
4. Open the Integration Lab or fixture project that references the package.
5. Verify dependency source and lock state before editing.

### 29.2 Integration workspace clone

1. Clone the central suite repository.
2. Clone the Integration Lab.
3. Initialize submodules only if that workspace version uses them.
4. Restore the canonical manifest/lock snapshot.
5. Clone editable package repositories into the documented relative `Packages/` folder when local development is required.
6. Switch only the intended dependencies to relative local paths.
7. Do not commit personal path changes to a compatibility snapshot.

### 29.3 Updating a Git dependency

- Change the project manifest to the intended new tag/commit.
- Allow Unity to update the lock file.
- Review the resolved revision.
- Execute upgrade/migration and compatibility evidence.
- Commit manifest and lock changes together.

### 29.4 Recovery from bad dependency state

Recovery proceeds from evidence:

1. Preserve the current manifest and lock file.
2. Inspect source URLs, revisions, and lock entries.
3. Compare with the last passing snapshot.
4. Restore the exact known-good revisions.
5. Regenerate the lock file only when the reason is understood.
6. Record the failure and fix.

Deleting lock files, caches, or package folders without capturing the cause is a last-resort diagnostic action, not the default remedy.

---

## 30. Archival, transfer, rename, and repository removal

### 30.1 Archival

An archived repository includes:

- Read-only status.
- Last supported version.
- Replacement/migration link.
- Security/contact guidance.
- Retained releases and tags where legally possible.

### 30.2 Repository rename

A repository rename updates:

- Central registry and catalog.
- Git URLs in guides and manifests.
- Integration snapshots for future runs.
- Package manifest/documentation URLs.
- Redirect or migration instructions.

The package ID and durable technical identity do not change merely because the repository slug changes.

### 30.3 Ownership transfer

Transfer outside EchoDevGames requires Jesse’s explicit approval and an ADR when it affects suite ownership, licensing, package IDs, release keys, or support.

### 30.4 Repository deletion

Released repositories are not deleted casually. If deletion is legally or security-required, retain replacement, migration, and release-integrity records in the central catalog.

---

## 31. Validation and release gates

### 31.1 Static repository checks

- Repository class and owner recorded.
- Package root policy satisfied.
- Canonical package/repository identity matches SFGSS-008.
- No duplicate package ID or release tag.
- Manifest version matches intended tag.
- Required files and `.meta` identities present.
- No machine-specific paths or secrets.
- README and Current Notes links work.
- Changelog, license, notices, and migration notes are current.

### 31.2 Git checks

- Working tree clean for release.
- Release commit reachable from the intended branch.
- Annotated tag points to the release commit.
- Protected tags/branches are not rewritten.
- Submodule pointers, when used, are initialized and recorded.
- Large-file requirements are declared and validated.

### 31.3 Unity package checks

- `package.json` parses and version matches tag.
- Claimed Git, local, embedded, tarball, and/or registry routes pass SFGSS-004 evidence.
- Project manifest and lock file reproduce the exact package graph.
- Git-only bridge/provider installation lists every required direct project dependency.
- Package removal and re-add preserve documented project-owned data.

### 31.4 Integration checks

- Compatibility snapshot records exact package revisions.
- Standalone evidence belongs to package repositories.
- Integration evidence belongs to bridge/provider or Integration Lab artifacts.
- No local-path dependency remains in a release compatibility snapshot.
- Catalog compatibility state matches retained evidence.

### 31.5 Release gate

A package release is blocked when:

- Manifest, tag, changelog, or documentation versions disagree.
- A released tag would be reused or moved.
- Required dependency sources cannot be reproduced.
- Required migration/removal evidence is missing.
- Licenses/notices are incomplete.
- A local path, mutable branch, secret, or unowned binary is present in the release artifact.
- The compatibility claim exceeds the executed evidence.

---

## 32. Reconciliation findings

The SUITE-DOC-28 repository audit found no authority collision. It recorded these consistency tasks for SUITE-DOC-30:

1. The actual current central Git remote/slug is not present in the supplied documentation archive. SFGSS-009 therefore records the current repository as authoritative without inventing its URL and lists `The-Sperks-Forge` only as a preferred slug if no existing identity must be preserved.
2. Several package specifications do not yet show their planned repository in Document Control even though SFGSS-008’s machine-readable registry contains all twenty-eight planned `EchoDevGames/<TechnicalIdentifier>` records.
3. Git-only distribution cannot provide transitive Git dependencies between packages. Future bridge/provider installation guides and Workshop plans must list every required project-level Git dependency until a scoped registry is approved.
4. The Integration Lab repository and compatibility catalog are approved concepts but have not yet been created or empirically validated.
5. Exact repository rulesets, CI providers, signing keys, package registry, and release automation remain `Not run` or unselected.
6. The stale Crafting open-decision wording and grandfathered Advanced document IDs remain queued for SUITE-DOC-30, as previously recorded.

---

## 33. Approval

### 33.1 Approval checklist

- [x] Repository classes and ownership are explicit.
- [x] Package repositories remain independent and UPM-rooted by default.
- [x] Central catalog and Integration Lab responsibilities are separate.
- [x] Local development has a portable relative workspace model.
- [x] Branch, commit, tag, release, and hotfix rules are defined.
- [x] SemVer and pre-release behavior are defined.
- [x] Git-only dependency limitations are explicit.
- [x] Project manifest and lock-file policy is defined.
- [x] Compatibility snapshots and cross-package release coordination are defined.
- [x] Bridge/provider versioning and removal remain independent.
- [x] Secrets, LFS, artifacts, CI, archives, and transfer are covered.
- [x] Unexecuted repository, registry, CI, and release claims remain `Not run`.
- [x] No implementation artifact was created.

### 33.2 Approval record

**Decision:** Approved  
**Approved by:** Jesse “Echo” Adams / EchoDevGames  
**Date:** August 4, 2026  
**Conditions at original approval:** The actual central and Integration Lab remote URLs, repository protections, CI, package registry, and release automation become factual only after creation and retained evidence. Package implementation was then still locked; later checkpoint records now govern implemented packages.

---

## External references reviewed

- Unity Package Manager: package manifests, project manifests, Git dependencies, lock files, dependency resolution, and package development workflow.
- Git: annotated tags, branches, worktrees, and submodules.
- GitHub: releases, branch protection, and repository rulesets.
- Semantic Versioning 2.0.0.

These references support the technical model. Exact behavior against the selected Unity/Git/GitHub versions remains subject to implementation and release evidence under SFGSS-004.
