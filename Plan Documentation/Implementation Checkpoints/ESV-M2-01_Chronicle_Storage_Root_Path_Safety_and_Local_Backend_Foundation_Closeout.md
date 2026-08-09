---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
  - sfgss/closeout
status: complete
updated: 2026-08-09
---

# ESV-M2-01 — Chronicle Storage Root, Path Safety, and Local Backend Foundation — Closeout

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Runtime version:** `0.1.0`
**Implementation commit:** `e4ef76c`
**Unity:** 6000.3.8f1
**Result:** **Complete**

## Outcome

ESV-M2-01 introduced Chronicle's first real filesystem-backed storage primitives without crossing into save-game document or generation publication authority.

Delivered:

- safe relative `SaveStorageKey`;
- root/path normalization and containment re-checks;
- configured production root beneath `Application.persistentDataPath`;
- injected sandbox backend factory seam;
- replaceable `LocalFileSaveStorageBackend`;
- structured storage/read results and stable diagnostics;
- initialize, exists, exact-byte read, create-only write, delete, and shutdown primitives;
- Chronicle lifecycle integration with backend initialization/shutdown;
- duplicate rejection before storage-root/backend side effects;
- focused storage/path/lifecycle tests.

## Test correction during development

The first focused Chronicle run was `29 / 40`.

All 11 failures were concentrated in root/lifecycle EditMode tests. Direct `AddComponent<EchoSaveRoot>()` construction could not rely on Unity automatically dispatching `Awake()` before the test attempted authority-dependent injection or assertions.

The correction was deliberately narrow:

- add internal `EnsureAuthorityClaimedForTesting()`;
- if Unity already established/rejected authority, do nothing;
- otherwise invoke the exact production `Awake()` authority path;
- update direct EditMode root helpers to call the seam.

No production `Awake()` policy, storage contract, local-backend behavior, configuration schema, DDOL policy, or peer-package dependency changed.

Final rerun: **40 / 40 passed, 0 failed**.

## Boundary preserved

ESV-M2-01 does not implement:

- Chronicle save-envelope/document serialization;
- default serializer implementation;
- slot catalog/active-slot behavior;
- immutable generation publication or head pointer;
- participants;
- migrations/checksums/recovery/autosave;
- prepared loads;
- peer bridges;
- Chronicle-owned/project-wide DDOL composition.

## Closeout decision

**ESV-M2-01 is complete.**

Next bounded checkpoint:

`ESV-M2-02 — Chronicle Document Contracts and Unity JSON Serializer Foundation`
