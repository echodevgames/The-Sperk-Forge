# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.3.0
**Completed checkpoint:** ESV-M1-01 — Installable Skeleton and Duplicate-Safe Authority Claim
**Current checkpoint:** ESV-M2-01 — Storage Root, Path Safety, and Local Backend Foundation
**Status:** ESV-M1-01 complete; ESV-M2-01 active / authorized

## ESV-M1-01 closeout

- Learning/activation commit `5b05d9d`.
- Package implementation commit `ecfa922`.
- Embedded package-resolution commit `2c70b1d`.
- Unity compile/import: **green**.
- Focused `EchoDevGames.EchoSave.Tests.Editor`: **all green**.
- Exact numeric test count was not captured and is not claimed.
- M1 performs zero durable-storage operations.

## Active ESV-M2-01 boundary

Authorized:
- configured storage-root resolution beneath `Application.persistentDataPath`;
- safe relative-path/key validation;
- replaceable local filesystem backend;
- first real storage-provider I/O;
- injected sandbox roots and focused storage tests;
- duplicate-before-storage-side-effect regression proof.

Still absent:
- save-envelope serialization;
- slot/catalog behavior;
- immutable generations/head publication;
- participants;
- migrations/checksums/recovery/autosave/prepared loads;
- peer-package adapters;
- Chronicle-owned or suite-wide DDOL composition.

The project owns long-lived service composition. Chronicle owns only Chronicle.
