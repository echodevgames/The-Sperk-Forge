---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-09
---

# ESV-M2-01 — Chronicle Storage Root, Path Safety, and Local Backend Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-01
**Milestone:** M2 — Document / Storage Core
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.3.0
**Prior checkpoint:** ESV-M1-01 — **Complete**
**Unity baseline:** 6000.3.8f1

## Intent

Introduce Chronicle's first real storage-provider I/O without yet implementing a save-game document model.

## Authorized scope

- configured default local root beneath `Application.persistentDataPath`;
- injected sandbox root seam for tests;
- safe relative storage path/key model;
- rooted/traversal/root-escape rejection;
- default replaceable local filesystem backend behind `ISaveStorageBackend`;
- only the primitive storage operations/results/diagnostics needed for later immutable-generation work;
- exact-byte round-trip proof in sandbox storage;
- duplicate rejection before storage-root/backend side effects;
- all M1 lifecycle regressions.

## Out of scope

- save-envelope JSON / serializer implementation;
- slots/catalogs;
- manifests/payload documents;
- immutable generations/head pointers;
- checksums;
- participants;
- migrations;
- backup/recovery/autosave/prepared loads;
- UI/tooling/Laboratory;
- peer persistence bridges;
- cloud/encryption/compression;
- Chronicle-owned DDOL/project-wide service composition.

## Path-safety rules

1. Production local root is a configured child of `Application.persistentDataPath`.
2. Tests may inject an explicit sandbox root.
3. Storage keys cannot become absolute paths.
4. `.` and `..` traversal segments are rejected.
5. Normalization cannot escape the backend root.
6. Display names never become physical path authority.
7. Invalid input fails before mutation.
8. Backend operations re-check containment.

## Executed proof

- production-root resolution — **Pass**;
- rooted/traversal/mixed-separator escape rejection — **Pass**;
- safe nested key — **Pass**;
- sandbox root creation — **Pass**;
- duplicate creates no root — **Pass**;
- exact-byte write/read round trip — **Pass**;
- not-found result — **Pass**;
- create-only conflict preserving existing bytes — **Pass**;
- injected backend failure — **Pass**;
- shutdown and M1 regressions — **Pass**.

Unity focused gate: **40 / 40 passed, 0 failed**.

Development note: the first focused run reached `29 / 40` because direct `AddComponent<EchoSaveRoot>()` EditMode tests could not rely on automatic `Awake()` dispatch. A narrow test-only activation seam was added that invokes the exact production authority path only when Unity has not already done so. The rerun passed `40 / 40`; storage semantics and production `Awake()` behavior were unchanged.

## Stop point

**Reached.** Safe sandboxed backend primitives and all M1 + M2-01 focused tests pass.

Implementation commit: `e4ef76c`.

Next bounded checkpoint: `ESV-M2-02 — Chronicle Document Contracts and Unity JSON Serializer Foundation`.

Do not continue into immutable generation publication, slot catalogs, participants, recovery, or autosave from this closeout.
