# The Chronicle — Developer Current Notes

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Current checkpoint:** ESV-M1-01 — Installable Skeleton and Duplicate-Safe Authority Claim
**Status:** Implementation candidate applied; Unity compile/tests pending

## Active boundary

This checkpoint implements only:

- package shell;
- `EchoSaveConfiguration`;
- package-local duplicate-safe `EchoSaveRoot`;
- explicit initialize/shutdown lifecycle;
- neutral provider/value/result contracts needed for the skeleton;
- deterministic lifecycle test seams;
- focused Editor tests.

## Explicitly absent

No real save-file I/O, slot directories, generations, manifests, serializer implementation, migration, integrity hashing, recovery, autosave, prepared loads, participant persistence, peer-package adapters, or project-owned DDOL composition is implemented here.

## Acceptance target

The package must compile standalone and the focused ESV-M1-01 tests must pass before the checkpoint is committed.
