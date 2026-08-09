---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active-authorized
updated: 2026-08-09
---

# ESV-M2-01 — Chronicle Storage Root, Path Safety, and Local Backend Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M2-01
**Milestone:** M2 — Document / Storage Core
**Status:** **ACTIVE / AUTHORIZED**
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

## Proposed focused proof

- production-root resolution;
- rooted/traversal/mixed-separator escape rejection;
- safe nested key;
- sandbox root creation;
- duplicate creates no root;
- exact-byte write/read round trip;
- not-found result;
- create-only conflict preserving existing bytes;
- injected backend failure;
- shutdown and M1 regressions.

The executed test count will be recorded from Unity, not pre-claimed.

## Stop point

Stop once safe sandboxed backend primitives and all M1 + M2-01 focused tests pass. Do not continue into save documents or generations without the next bounded checkpoint.
