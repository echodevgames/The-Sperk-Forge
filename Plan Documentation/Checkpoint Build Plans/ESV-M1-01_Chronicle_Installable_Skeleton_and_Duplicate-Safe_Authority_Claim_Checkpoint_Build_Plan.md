---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-09
---

# ESV-M1-01 — Chronicle Installable Skeleton and Duplicate-Safe Authority Claim

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M1-01
**Milestone:** M1 — Skeleton
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.2.0
**Suite authorities:** SFGSS-000 v0.26.0; SFGSS-001 v1.5.0; SFGSS-ADR-006; SFGSS-INT-SUITE-001 v1.1.0
**Learning prerequisite:** PKG-LEARN-009 — **Complete**
**Implementation permission:** Authorized 2026-08-09 after completed teach-back and Jesse's explicit activation
**Unity baseline at scaffold time:** 6000.3.8f1; must be reverified when activated

> **Activation record:** PKG-LEARN-009 completed on 2026-08-09. Jesse explicitly activated Chronicle implementation by asking to begin building. This checkpoint is now the active implementation boundary.

## 1. Activation gate

**Gate result: PASSED 2026-08-09.**

The following activation conditions are satisfied before production Chronicle code begins:

1. PKG-LEARN-009 is Complete.
2. Jesse has completed the Chronicle teach-back.
3. Any source conflict discovered by the review is reconciled into authority.
4. Jesse explicitly says to activate ESV-M1-01.
5. The repository and Unity project are rehydrated at the then-current exact baseline.
6. The working tree is clean or intentionally partitioned under checkpoint rules.

## 2. Intended outcome

Create the smallest installable Chronicle package shell that proves **package-local runtime authority** without performing real durable storage.

The checkpoint should eventually prove:

```text
package installs
    ↓
one EchoSave authority claims
    ↓
duplicate loses before side effects
    ↓
explicit initialize / shutdown lifecycle
    ↓
neutral provider/value/result contracts compile
    ↓
NO real save file written
```

## 3. Authorized implementation scope

Authorized now:

- package manifest and asmdefs;
- package README / Documentation shell / package Current Notes;
- project-owned Chronicle configuration type;
- package-local `EchoSaveRoot` / service lifecycle shell;
- explicit initialization and shutdown state;
- duplicate-safe authority claim before storage/path/callback/participant side effects;
- neutral stable value IDs/results/provider interfaces needed to define the skeleton boundary;
- injection seams required to test lifecycle without touching real storage;
- focused EditMode/lifecycle tests;
- minimal setup/documentation necessary to instantiate the package-local authority without another Echo package.

## 4. Explicitly out of scope for ESV-M1-01

This checkpoint does **not** authorize:

- actual save-file writes or reads;
- slot directories;
- immutable generation creation;
- head publication;
- manifests or payload files;
- serializer implementation or serializer-provider selection work;
- migration chains;
- integrity hashing;
- backup, recovery, quarantine, or trash;
- autosave;
- prepared-load implementation;
- real participant capture/apply;
- Inventory/Progression/Objectives/Characters/World adapters;
- Accord preference integration;
- Looking Glass save UI;
- Passage scene-flow integration;
- First Light bridge implementation;
- cloud/encryption/compression providers;
- a project-wide `DontDestroyOnLoad` service root;
- a universal service registry/locator;
- Chronicle parenting or owning peer services.

If any of those become necessary merely to make the skeleton compile, stop and revisit the checkpoint boundary.

## 5. ADR-006 proof obligations

The activated checkpoint must demonstrate:

1. `EchoSaveRoot` authority is scoped to EchoSave only.
2. A duplicate loses before any path creation, catalog scan, callback registration, participant registration, or operation admission.
3. No peer Echo package is required.
4. No project-wide service locator is introduced.
5. Scene-surviving lifetime, if implemented at M1, is documented/tested as Unity object lifetime only.
6. No durable file operation is required to prove the lifecycle.
7. Shutdown releases the package-local claim so the Laboratory can reset deterministically.

## 6. Proposed focused tests

All statuses are `Not run` until implementation is explicitly activated and executed.

| Test intent | Planned result | Status |
|---|---|---|
| One configured root claims authority | Exactly one authority | Not run |
| Duplicate root appears | Duplicate exits before side effects | Not run |
| Initialize twice | Deterministic idempotent/structured behavior per spec | Not run |
| Shutdown | Admission stops and authority clears | Not run |
| Reinitialize after clean shutdown | New valid authority may claim | Not run |
| Missing/invalid config | Structured failure, no storage side effects | Not run |
| Peer packages absent | Package compiles/initializes standalone | Not run |
| Filesystem spy | Zero real save files written by M1 | Not run |
| Scene-lifetime path if included | Root lifetime works without becoming project composition authority | Not run |

## 7. Intended file families

Exact filenames remain an implementation decision, but scope should stay within:

```text
Packages/com.echodevgames.echo-save/
├── package.json
├── README.md
├── CHANGELOG.md
├── Documentation~/
├── Runtime/
│   ├── Core/
│   ├── Configuration/
│   └── Contracts/
├── Editor/
└── Tests/
    └── Editor/
```

Do not create bridges/integration packages in this checkpoint.

## 8. Stop conditions

Stop and return to authority if implementation pressure suggests:

- `EchoSave` must reference Accord, Looking Glass, First Light, Passage, Inventory, or another peer core;
- Chronicle should own a generic service registry;
- First Light should remain the parent of Chronicle after launch;
- a mandatory suite-wide persistent-root package is required;
- a serializer/file format must be chosen to prove duplicate-safe lifecycle;
- package-local lifetime and project composition cannot be kept separate;
- M1 requires actual player save data.

## 9. Evidence record

| Evidence | Status |
|---|---|
| PKG-LEARN-009 | Complete |
| Teach-back | Complete |
| Implementation commit | Not started |
| Compile result | Not run |
| Focused EditMode tests | Not run |
| Standalone Laboratory | Not run |
| Reference Showcase | Not run |
| Clean-project proof | Not run |
| Distribution Kit | Not run |

## 10. Next action

Implement the bounded ESV-M1-01 skeleton against the exact rehydrated repository/Unity baseline. Keep all durable file I/O, serializer selection, slot/generation behavior, peer bridges, and project-wide DDOL composition out of scope.
