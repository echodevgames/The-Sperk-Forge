# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Package version:** `0.1.0`
**Specification:** SFGSS-PKG-ECHOSAVE-001 v1.5.0
**Completed checkpoint:** ESV-M2-02 — Document Contracts and Unity JSON Serializer Foundation
**Current checkpoint:** ESV-M2-03 — Generation Identity, Integrity, and Commit-Document Foundation
**Status:** ESV-M2-02 complete; ESV-M2-03 active / authorized

## ESV-M2-02 closeout

Implementation commit: `6404037`.

Evidence:
- Unity compile/import: **green**;
- focused `EchoDevGames.EchoSave.Tests.Editor`: **57 / 57 passed, 0 failed**;
- package-owned document identity/version contracts;
- `SaveDocumentEnvelope`;
- structured serializer status/result contracts;
- `SaveSerializerRegistry`;
- package-owned default `UnityJsonSaveSerializer`;
- supported DTO/envelope round trips;
- null/empty/malformed-input rejection;
- unsupported package-document version/kind rejection;
- serializer layer performs no filesystem I/O;
- all M1/M2-01 storage/lifecycle regressions remain green.

## Active ESV-M2-03 boundary

Authorized:
- `SaveSlotId` and `SaveGenerationId` technical value types only;
- `SaveManifest`, `SavePayloadDocument`, `SavePayloadEntry`, and `SaveHeadPointer` package DTO/version contracts;
- `IIntegrityProvider`;
- default `Sha256IntegrityProvider`;
- detached checksum calculation/verification;
- explicit integrity algorithm identity;
- document-agreement checks in memory;
- serializer compatibility for the new package documents;
- focused tests preserving the 57-test regression floor.

Still absent:
- physical generation directory creation/publication;
- `head.json` replacement/mutation;
- slot catalog/policy/active selection;
- participant registry/capture/apply;
- project gameplay payload ownership;
- migration/recovery/retention/autosave;
- prepared loads;
- peer-package bridges;
- Chronicle-owned/project-wide DDOL composition.

This split is deliberate: a generation cannot honestly become eligible until required documents can be described and checksummed. M2-03 establishes those prerequisites; the later publication checkpoint will prove candidate → verified generation → head-last commit behavior.
