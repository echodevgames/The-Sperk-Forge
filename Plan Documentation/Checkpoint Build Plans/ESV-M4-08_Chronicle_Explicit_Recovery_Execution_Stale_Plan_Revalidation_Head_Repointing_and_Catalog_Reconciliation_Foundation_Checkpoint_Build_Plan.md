---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-10
---
# ESV-M4-08 — Chronicle Explicit Recovery Execution, Stale-Plan Revalidation, Head Repointing, and Catalog Reconciliation Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-08
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.30.0
**Decision:** ESV-D-030
**Prior checkpoint:** ESV-M4-07 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **524 / 524**
**Exact implementation baseline:** `0396adb`

## 1. Intent

Execute one explicit M4-07 recovery choice without weakening Chronicle's immutable-generation model.

M4-08 answers one bounded question:

> Given an immutable recovery plan and one caller-selected candidate from that plan, can Chronicle serialize the mutation, prove the plan is still fresh, prove the candidate is still valid, repoint only the small head pointer to that already committed generation, and reconcile the catalog without ever rewriting the generation or fabricating rollback?

```text
SaveRecoveryPlan + selected candidate
             ↓
service Ready
             ↓
root-local mutation admission
             ├── occupied → Busy
             └── admitted
                    ↓
fresh BuildRecoveryPlan
                    ↓
exact source-provenance compare
             ├── mismatch → STALE / NO MUTATION
             └── match
                    ↓
candidate still verified + present?
             ├── no → REJECT / NO MUTATION
             └── yes
                    ↓
construct validated recovery head
                    ↓
publish head.json only
                    ↓
RECOVERY DURABLY COMMITTED
                    ↓
catalog refresh
             ├── fail → committed / unreconciled truth
             └── pass → complete success
```

Automatic recovery remains outside this checkpoint.

## 2. Carried-forward authority

Chronicle already proves:
- one root-local mutating-operation authority;
- immediate Busy behavior for nonqueued manual mutation;
- immutable committed generations;
- provider-neutral small-current-object publication;
- head-last durable truth;
- slot catalog refresh/reconciliation;
- retention of verified committed history;
- read-only M4-07 recovery planning;
- full candidate manifest/payload/integrity verification;
- deterministic newest-valid ordering;
- immutable payload-free recovery plans;
- deterministic technical source-provenance fingerprints;
- focused Chronicle Editor **524 / 524**.

The approved API map already specifies:

`ExecuteRecoveryAsync(SaveRecoveryPlan, candidate)`

as an async/exclusive operation that publishes a selected verified generation and returns a recovery result.

### ESV-D-030 — explicit recovery execution revalidates before mutation

> Explicit recovery execution is one root-local mutating operation. Chronicle must first prove that the supplied immutable plan still describes the exact current source state and that the selected candidate remains a fully verified candidate in a freshly rebuilt plan. Any stale plan, mismatched slot, or no-longer-valid candidate fails before head mutation. Successful head publication is durable recovery truth; later catalog reconciliation failure is reported separately and never fabricates rollback.

## 3. Authorized implementation scope

### Public operation

Add:

`ExecuteRecoveryAsync(SaveRecoveryPlan plan, SaveRecoveryCandidate candidate)`

The public operation:
- requires Ready;
- accepts only one explicit immutable plan and one explicit candidate;
- returns a fresh `Awaitable<SaveRecoveryResult>`;
- does not auto-use `PreferredCandidate`;
- does not infer permission from gameplay state;
- does not run participant capture/apply;
- does not select an active slot.

### Recovery result truth

Add a bounded immutable/public recovery result/status surface sufficient to report:
- invalid request;
- service not ready;
- admission closed;
- Busy;
- stale plan;
- invalid/no-longer-valid candidate;
- head publication failure;
- recovery committed but catalog reconciliation failed;
- complete success;
- target slot;
- selected generation;
- `HeadPublished`;
- `CatalogReconciled`;
- stable diagnostic/message.

A committed head must never be reported as rolled back because catalog refresh later fails.

### Admission

Execution reuses the M4-04/M4-05 root-local `SaveOperationAdmissionCoordinator`.

Rules:
- one admitted mutation at a time;
- occupied admission returns Busy immediately;
- no hidden recovery queue;
- shutdown/closed admission rejects;
- the admission lease spans freshness revalidation, head publication, and catalog reconciliation;
- read-only `BuildRecoveryPlanAsync` remains outside mutation admission.

M4-08 does not add a new caller cancellation overload.

### Plan and candidate validation

Before mutation:
- `plan` must be non-null and from a successful recovery-required M4-07 state;
- plan must carry one valid target `SaveSlotId`;
- candidate generation ID must be valid;
- candidate must be present in the supplied plan's immutable candidate list;
- candidate slot is implicitly the plan slot; no caller-controlled path exists;
- `RecoveryNotRequired` / `NoValidCandidate` plans cannot execute.

### Freshness fence

After admission and before head publication:
1. rebuild a fresh M4-07 recovery plan for the same slot;
2. require a recovery-required fresh plan;
3. compare the fresh `SourceProvenanceFingerprint` exactly to the supplied plan;
4. require the selected candidate to remain present in the fresh verified candidate list;
5. require selected candidate technical summary identity to match the fresh candidate;
6. reject stale/mismatched state before any write/publication call.

This is ESV-T-079.

### Head repointing

Execution does not create a new generation.

It publishes one validated `SaveHeadPointer` whose:
- `slotId` is the plan slot;
- `currentGenerationId` is the selected verified generation;
- `previousGenerationId` is empty because the damaged/untrusted source current generation is not promoted as known-good history;
- `updateSequence` advances from a structurally valid trustworthy old head when possible, otherwise starts at a safe initial value;
- package document version/kind remain current;
- head is validated and serialized before publication.

Publication uses the existing provider-neutral `ISaveStoragePublicationBackend.PublishCurrentObject` capability.

No generation payload/manifest bytes are written, copied, migrated, or deleted.

### Durable truth

Mutation boundary:

```text
fresh plan/candidate proof
        ↓
head serialization/validation
        ↓
PublishCurrentObject(head.json)
        ↓
DURABLE RECOVERY COMMIT
```

Before successful head publication:
- failure means recovery not committed.

After successful head publication:
- the selected generation is authoritative current truth;
- cancellation/rollback fiction is forbidden;
- later failures are maintenance/reconciliation truth only.

### Catalog reconciliation

After head publication:
- refresh the existing session slot catalog;
- require the recovered slot to reconcile to the selected generation if refresh succeeds;
- preserve display metadata derived from the selected generation manifest;
- do not auto-select the slot;
- if refresh fails, return `HeadPublished = true`, `CatalogReconciled = false`;
- do not republish the old/broken head to make the result look tidy.

### Failure safety

Tests must prove:
- invalid request performs zero mutation;
- Busy performs zero mutation;
- stale plan performs zero mutation;
- candidate not in plan performs zero mutation;
- candidate removed/corrupted after planning performs zero mutation;
- head serialization/validation failure performs zero head publication;
- head publication failure leaves source state as it was before execution;
- successful head publication followed by catalog failure reports committed/unreconciled truth;
- success updates catalog current-generation truth;
- source generation files are byte-identical before/after recovery;
- broken/nonselected evidence is preserved;
- active slot selection does not change;
- no participant callback occurs;
- no retention cleanup runs merely because recovery executed;
- no quarantine/delete/tree-delete authority is introduced.

## 4. Explicit non-scope

Do not add:
- automatic/configured fallback execution;
- implicit recovery during load/prepare;
- caller-facing recovery cancellation overload;
- generic operation queue/capacity/overflow;
- quarantine movement;
- incomplete/corrupt generation cleanup;
- slot rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- automatic autosave timers/gameplay triggers;
- permission-provider production facade wiring;
- full recovery policy/configuration/Setup authoring;
- participant/document migration during recovery;
- scene travel;
- bridges;
- service locator;
- Chronicle-owned/project-wide DDOL.

## 5. Safety invariants

Focused tests must establish:
- ESV-T-078 explicit plan execution publishes the selected verified generation;
- ESV-T-079 stale plan rejects before mutation;
- Busy rejects before recovery reads that could mutate anything;
- only the plan slot is targetable;
- only a candidate contained in the plan and fresh rebuilt plan is targetable;
- selected generation bytes never change;
- nonselected/broken generations remain untouched;
- valid current/no-recovery plan cannot execute;
- no-candidate plan cannot execute;
- head publication is the commit boundary;
- catalog failure after head publication does not fabricate rollback;
- active-slot session selection remains unchanged;
- planner remains read-only;
- base storage contracts remain unchanged;
- no quarantine/destructive-slot/generic-queue creep enters the checkpoint;
- all prior **524 / 524** Chronicle tests remain green.

## 6. Proposed focused proof

Primary registry mapping:
- ESV-T-078 — execute plan → head/catalog update safely;
- ESV-T-079 — stale plan → rejected.

Additional focused proofs:
- successful missing-head recovery;
- successful corrupt-current recovery;
- explicit nonpreferred valid candidate selection;
- candidate not in supplied plan;
- candidate corrupted after planning;
- new generation appears after planning → stale;
- source head changes after planning → stale;
- overlapping mutation → Busy;
- admission closed/shutdown;
- head publication failure;
- catalog refresh failure after head success;
- source generation byte immutability;
- active-slot nonmutation;
- participant callback absence;
- no retention/quarantine/delete calls.

Executed Unity totals are recorded, never predicted.

## 7. Stop point

Stop when one explicit immutable M4-07 plan can be safely executed so that Chronicle:

1. serializes the mutation through the existing admission authority;
2. rejects stale source evidence before mutation;
3. rejects an invalid/no-longer-valid selected candidate before mutation;
4. republishes only `head.json` to the already verified committed candidate;
5. never rewrites generation contents;
6. reconciles the catalog after the head commit;
7. reports committed-head and catalog-reconciliation truth separately.

Do **not** add automatic fallback or quarantine yet.


## 8. Completion evidence

**Planning/activation commit:** `c324aa4`

**Implementation commit:** `1985fb0`

**Final focused Chronicle Editor gate:** **540 / 540 passed, 0 failed**

**Net new focused tests:** **16**

**Committed implementation/test scope:** **18 files**, `1846` insertions, `10` deletions

The implementation reached the intended stop point: recovery execution reuses shared admission, rejects stale plan/candidate state before mutation, republishes only `head.json`, preserves generation bytes, and reconciles the catalog with truthful durable partial-state reporting.

One compile-only test maintenance correction changed two new references from `FakeManualSaveTransactionExecutor.CallCount` to the existing `Calls` property. Runtime implementation, public API, architecture, ESV-D-030 authority, recovery behavior, test intent, and discovery shape were unchanged.

No follow-on M4 checkpoint is activated by this closeout.
