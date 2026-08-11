---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: active
updated: 2026-08-10
---
# ESV-M4-09 — Chronicle Slot Rename, Full-State Duplication, Stable Identity, and Catalog Reconciliation Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-09
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.31.0
**Decision:** ESV-D-031
**Prior checkpoint:** ESV-M4-08 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **540 / 540**
**Exact implementation baseline:** `07bbd2b`

## 1. Checkpoint purpose

Complete the non-destructive remainder of Chronicle CAP-017 before destructive delete/trash work.

This checkpoint introduces:
1. public slot rename;
2. public slot duplication;
3. stable-identity/path preservation;
4. immutable generation publication for metadata-only rename;
5. verified source-state cloning for duplication;
6. reuse of the existing root-local mutation admission authority;
7. truthful post-publication retention/catalog reconciliation.

The checkpoint deliberately does **not** combine destructive delete planning/trash into the same mutation slice.

## 2. Authority invariant

ESV-D-031:

> Slot rename and duplication are non-destructive admitted mutations over Chronicle's immutable-generation model. Rename never changes `SaveSlotId` or the physical slot path and never edits committed generation files in place. Duplicate never mutates the source, requires capacity, creates a new package-generated slot/generation identity, copies only a fully verified current source state without participant callbacks, and revalidates the bound source before destination publication.

Consequences:
- display names remain presentation metadata, never path identity;
- committed generations remain immutable;
- source state cannot silently change midway through rename/duplicate;
- duplicate cannot bypass capacity through degraded-slot accounting;
- a directory existing is never sufficient success truth;
- catalog/maintenance failure after head commit cannot fabricate rollback.

## 3. Public surface

M4-09 may add bounded neutral runtime contracts such as:
- `SaveSlotRenameRequest`;
- `SaveSlotRenameResult`;
- `SaveSlotRenameStatus`;
- `SaveSlotDuplicateRequest`;
- `SaveSlotDuplicateResult`;
- `SaveSlotDuplicateStatus`;
- `IEchoSaveService.RenameSlotAsync(...)`;
- `IEchoSaveService.DuplicateSlotAsync(...)`.

Exact names may be adjusted only for consistency with existing Chronicle naming.

Do not add:
- generic operation queues;
- caller-provided physical paths;
- caller-provided technical slot IDs for normal duplication;
- destructive delete APIs;
- trash APIs;
- recovery policy automation;
- new scene/lifetime authority.

## 4. Rename transaction

### 4.1 Preflight

Before durable mutation:
1. service must be Ready;
2. acquire existing root-local mutating admission;
3. reject Busy rather than queue;
4. validate request slot ID and display name;
5. refresh/reconcile catalog truth;
6. require one canonical healthy source slot;
7. read and fully verify the current generation;
8. bind exact source slot/current-generation provenance.

Invalid/missing/degraded sources fail before mutation.

### 4.2 Rename publication

Rename must:
1. preserve source `SaveSlotId`;
2. preserve physical slot path;
3. preserve participant payload bytes/state;
4. preserve participant inventory semantics;
5. create a new `SaveGenerationId`;
6. create a new manifest with updated display metadata and destination generation identity;
7. never modify the source committed generation files;
8. revalidate expected current head immediately before publication;
9. publish the new immutable generation;
10. publish `head.json` last.

### 4.3 Rename post-commit maintenance

After head publication:
1. recovery/previous-generation truth remains valid through the normal head model;
2. run existing M4-06 retention maintenance;
3. refresh the M4-01 catalog;
4. preserve active-slot identity if the renamed slot is active;
5. report retention/catalog partial truth separately from committed rename truth.

A retention or catalog failure after head commit does not roll rename back.

## 5. Duplicate transaction

### 5.1 Source and capacity preflight

Before durable destination mutation:
1. service must be Ready;
2. acquire existing root-local mutation admission;
3. reject Busy rather than queue;
4. refresh catalog;
5. require source canonical and healthy;
6. apply existing M4-02 capacity counting, including degraded canonical technical slots;
7. read and fully verify source current generation;
8. bind exact source slot/current-generation provenance.

### 5.2 Destination identity

Duplicate must:
- generate a new package canonical `SaveSlotId`;
- use bounded collision retry consistent with M4-02;
- generate a new `SaveGenerationId`;
- never reuse source physical slot path;
- copy source display metadata by default;
- allow later rename rather than silently coupling duplicate identity to display text.

### 5.3 State copy

Duplicate must copy the fully verified current source state without participant callbacks.

It may reconstruct destination package documents, but:
- participant payload content must remain state-equivalent;
- unknown/opaque entries remain opaque;
- source files remain unchanged;
- no runtime participant capture occurs;
- no participant migration/apply/default occurs merely because a duplicate is requested.

### 5.4 Stale-source protection

Immediately before destination publication, Chronicle must prove the source still has the bound current generation.

If the source changed:
- reject as stale;
- do not publish destination head;
- do not report a duplicate success.

### 5.5 Destination publication

Duplicate success requires:
1. complete destination generation documents;
2. full destination generation verification;
3. immutable destination generation publication;
4. destination `head.json` publication last;
5. destination catalog reconciliation.

A mere destination directory or orphan generation is not success.

The duplicated slot is not automatically selected.

## 6. No participant side effects

Rename and duplicate must not:
- capture participants;
- apply participants;
- initialize defaults;
- mutate the session unknown-payload store;
- trigger scene travel;
- change project-owned gameplay state.

These are durable transport/slot operations only.

## 7. Failure truth

Required status distinctions should include:
- invalid request;
- service not ready;
- admission closed;
- Busy;
- slot not found;
- source degraded/invalid;
- capacity reached;
- source stale;
- identity collision exhaustion;
- publication failure;
- retention maintenance failure where applicable;
- catalog reconciliation failure;
- success.

After durable head publication, maintenance failure must preserve committed truth.

## 8. Required automated proof

### Registry-aligned proofs

- **ESV-T-019** — Rename slot: display metadata changes; ID/path remain stable.
- **ESV-T-020** — Duplicate slot: new ID receives equivalent fully verified state.

### Additional focused proofs

Rename:
- invalid display metadata rejects before mutation;
- missing/degraded source rejects before mutation;
- source payload bytes remain unchanged;
- source slot directory identity remains unchanged;
- new rename generation gets new generation ID;
- stale current-head change rejects;
- repeated rename respects retention bounds;
- active renamed slot remains active;
- catalog failure after commit reports committed/unreconciled truth.

Duplicate:
- capacity reached rejects with zero durable destination mutation;
- degraded source counts/blocks appropriately;
- new destination slot ID differs from source;
- new destination generation ID differs from source;
- destination payload state is equivalent;
- source files remain byte-identical;
- stale source before publication rejects;
- destination head is last durable commit;
- duplicate does not auto-select;
- catalog failure after destination head reports durable duplicate truth.

Shared:
- overlapping mutation returns Busy;
- pre-Ready returns ServiceNotReady;
- post-shutdown returns AdmissionClosed;
- no participant callbacks;
- no generic queue introduced;
- no delete/trash APIs introduced.

## 9. Implementation constraints

Preserve:
- base `ISaveStorageBackend` unless a genuinely unavoidable provider capability gap is discovered;
- existing path-safety rules;
- existing immutable-generation verification;
- M4-02 capacity semantics;
- M4-04 admission semantics;
- M4-06 retention semantics;
- M4-01 catalog truth;
- public neutral technical API naming.

If a new optional provider capability is truly required, stop and justify it before widening the base storage contract.

## 10. Explicit non-goals

Not in M4-09:
- prepare-delete / confirm-delete;
- trash or restore-from-trash;
- trash retention;
- quarantine/incomplete-generation cleanup;
- persistent `catalog.cache.json`;
- automatic/configured recovery fallback;
- recovery-on-load;
- generic operation queues/capacity/overflow;
- new recovery cancellation API;
- automatic timer/checkpoint autosave triggers;
- permission-provider production wiring;
- full `EchoSaveConfiguration` / Setup asset expansion;
- scene travel;
- peer bridges;
- service locator;
- project-wide or Chronicle-owned DDOL composition.

## 11. Completion gate

M4-09 may close only when:
1. Unity compiles cleanly;
2. the entire focused `EchoDevGames.EchoSave.Tests.Editor` assembly is green;
3. the prior **540 / 540** floor remains green;
4. ESV-T-019 and ESV-T-020 are implemented;
5. new rename/duplicate failure and race proofs are green;
6. tracked/new implementation scope is reviewed explicitly;
7. runtime/public API changes match ESV-D-031;
8. no delete/trash/cache/automatic-recovery/generic-queue scope crept in;
9. documentation closeout records actual discovered test totals;
10. no follow-on checkpoint is auto-activated.

## 12. Expected workflow

```text
planning/authority activation
        ↓
commit + push planning
        ↓
implementation bundle
        ↓
Unity compile + focused gate
        ↓
bounded hotfix only if evidence demands it
        ↓
pre-commit scope review
        ↓
implementation commit + push
        ↓
documentation/authority closeout
```

Do not predict the final discovered test count.
