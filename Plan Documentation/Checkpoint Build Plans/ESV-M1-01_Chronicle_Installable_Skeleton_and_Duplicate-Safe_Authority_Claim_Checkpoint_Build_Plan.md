---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M1-01 — Chronicle Installable Skeleton and Duplicate-Safe Authority Claim

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M1-01
**Milestone:** M1 — Skeleton
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.2.0
**Suite authorities:** SFGSS-000 v0.26.0; SFGSS-001 v1.5.0; SFGSS-ADR-006; SFGSS-INT-SUITE-001 v1.1.0
**Learning prerequisite:** PKG-LEARN-009 — **Complete**
**Implementation permission:** Authorized 2026-08-09 after completed teach-back and Jesse's explicit activation
**Unity baseline at scaffold time:** 6000.3.8f1; must be reverified when activated

> **Closeout record:** PKG-LEARN-009 completed and activated ESV-M1-01 on 2026-08-09. Implementation committed at `ecfa922`; embedded Package Manager resolution committed at `2c70b1d`. Unity compile/import and the focused Chronicle Editor gate were reported all green. No real durable save I/O was introduced.

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

The implementation is complete. The focused Chronicle Editor gate was reported all green; the exact numeric test count was not captured, so this record does not invent one.

| Test intent | Planned result | Status |
|---|---|---|
| One configured root claims authority | Exactly one authority | **Pass** |
| Duplicate root appears | Duplicate exits before side effects | **Pass** |
| Initialize twice | Deterministic idempotent/structured behavior per spec | **Pass** |
| Shutdown | Admission stops and authority clears | **Pass** |
| Reinitialize after clean shutdown | New valid authority may claim | **Pass** |
| Missing/invalid config | Structured failure, no storage side effects | **Pass** |
| Peer packages absent | Package compiles/initializes standalone | **Pass** |
| Filesystem spy | Zero real save files written by M1 | **Pass** |
| Scene-lifetime path if included | Not included; project-owned DDOL composition remains out of M1 | **Not applicable** |

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
| Implementation commit | `ecfa922` |
| Compile result | **Pass** — Unity compile/import green |
| Focused EditMode tests | **Pass** — all green; exact numeric count not captured |
| Standalone Laboratory | Deferred to Chronicle M5 tooling/Laboratory milestone |
| Reference Showcase | Future package graduation stage |
| Clean-project proof | Future package graduation/release stage |
| Distribution Kit | Future package graduation/release stage |

## 10. Next action

ESV-M1-01 is closed. Continue with `ESV-M2-01 — Chronicle Storage Root, Path Safety, and Local Backend Foundation`. M2-01 is the first checkpoint allowed to introduce real storage-provider I/O, but it must not yet implement save slots, Chronicle documents, serializer payloads, generations, head publication, participants, or recovery.
