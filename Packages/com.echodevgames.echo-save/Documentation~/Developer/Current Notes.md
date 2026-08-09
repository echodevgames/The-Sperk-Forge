# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.4.0
**Completed checkpoint:** ESV-M2-01 — Storage Root, Path Safety, and Local Backend Foundation
**Current checkpoint:** ESV-M2-02 — Document Contracts and Unity JSON Serializer Foundation
**Status:** ESV-M2-01 complete; ESV-M2-02 active / authorized

## ESV-M2-01 closeout

Implementation commit: `e4ef76c`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **40 / 40 passed, 0 failed**;
- safe relative storage keys and physical root containment;
- configured production root beneath `Application.persistentDataPath`;
- injected sandbox roots for automated tests;
- replaceable `LocalFileSaveStorageBackend`;
- exact-byte read/write proof;
- create-only conflict preserves existing bytes;
- structured not-found/backend failure behavior;
- duplicate root cannot create the storage root;
- M1 authority/lifecycle regressions remain green.

Development correction:
- the first focused run was `29 / 40` because direct EditMode `AddComponent<EchoSaveRoot>()` tests could not rely on automatic `Awake()` dispatch;
- a narrow internal test activation seam now invokes the exact production `Awake()` path only when needed;
- the final rerun passed `40 / 40`;
- storage semantics and production authority behavior were unchanged.

## Active ESV-M2-02 boundary

Authorized:
- package-owned save-document DTO contracts;
- explicit package document-format versions;
- `UnityJsonSaveSerializer` using Unity `JsonUtility`;
- serializer provider registry/lookup;
- structured serializer results/diagnostics;
- package-owned DTO round-trip tests;
- malformed-input and unsupported-version rejection at the serializer/document boundary.

Still absent:
- immutable generation commit/publication;
- head publication;
- slot catalog/active-slot operations;
- participant capture/apply;
- participant payload migration/preservation;
- checksums/integrity/recovery/autosave;
- prepared loads;
- peer-package persistence bridges;
- Chronicle-owned/project-wide DDOL composition.

Chronicle owns only Chronicle. Project-owned runtime state and long-lived service composition remain outside this package.
