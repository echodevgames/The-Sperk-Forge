---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-10
---
# ESV-M4-07 — Chronicle Recovery Candidate Discovery, Immutable Recovery Plan Truth, and Deterministic Fallback Selection Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-07
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.27.0
**Decision:** ESV-D-029
**Prior checkpoint:** ESV-M4-06 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **497 / 497**
**Exact implementation baseline:** `9695450`

## 1. Intent

Build a trustworthy **read-only recovery plan** before Chronicle is allowed to repair anything.

M4-07 answers one bounded question:

> Given one technical slot whose head may be missing, malformed, unsupported, or pointing at an unreadable/corrupt generation, can Chronicle inspect retained immutable generations, fully verify recovery candidates, deterministically select the newest valid fallback, and return an immutable provenance-bound plan without changing a single durable byte?

```text
technical SaveSlotId
      ↓
read/inspect head
      ↓
classify observed source state
      ├── healthy current
      ├── missing head
      ├── invalid/unreadable head
      └── current generation invalid
      ↓
bounded generation discovery
      ↓
candidate full verification
  manifest + payload + integrity
      ↓
exclude/preserve bad evidence
      ↓
deterministic newest-valid ordering
      ↓
immutable recovery plan
      ├── recovery not required
      ├── recoverable + preferred candidate
      └── no valid candidate
      ↓
ZERO DURABLE MUTATION
```

Actual recovery publication remains a separate checkpoint.

## 2. Carried-forward authority

Chronicle already proves:
- immutable committed generations;
- head-last publication;
- structural head validation;
- full manifest/payload/integrity agreement validation;
- provider-neutral bounded child-directory discovery;
- retention-protected current + immediate predecessor history;
- unknown payload preservation;
- public save/autosave result truth;
- focused Chronicle Editor **497 / 497**.

The package specification already requires:
- `BuildRecoveryPlanAsync(SaveSlotId)` as a read-oriented public surface;
- missing head with valid generations to produce a recovery plan;
- corrupt current generation never to be overwritten before a verified candidate is chosen;
- recovery candidate search after checksum/integrity failure;
- no valid recovery generation to preserve evidence and avoid overwrite;
- deterministic ordering across multiple valid candidates;
- later execution to reject stale recovery plans.

### ESV-D-029 — recovery planning is read-only and provenance-bound

> Chronicle recovery planning may inspect damaged head/current-generation state and retained generations, but it performs zero durable mutation. Only fully verified committed generations may become candidates. Candidate ordering is deterministic newest-valid first. The immutable plan records exact observed source provenance so a later execution checkpoint can reject stale evidence before publication.

## 3. Authorized implementation scope

### Public read-only recovery planning

Add the already-specified public service surface:

`BuildRecoveryPlanAsync(SaveSlotId)`

The public operation:
- requires service Ready;
- validates technical slot identity;
- performs bounded reads only;
- does not require an active-slot selection;
- does not capture/apply participants;
- does not take the mutating-operation admission lease merely to inspect;
- returns structured immutable plan/result truth.

Do not add `ExecuteRecoveryAsync` in M4-07.

### Observed head classification

Planning must distinguish:
- valid head + fully valid current generation: recovery not required;
- missing `head.json`;
- unreadable head;
- malformed/unsupported head;
- structurally valid head referencing a missing generation;
- structurally valid head referencing a generation whose manifest/payload/integrity validation fails.

The raw damaged source remains untouched.

### Candidate discovery

Use provider-neutral bounded immediate-child discovery beneath:

`slots/<slot-id>/generations`

Rules:
- no direct `System.IO` in recovery core;
- only canonical `SaveGenerationId` child names enter candidate verification;
- noncanonical children are ignored/preserved;
- a discovery-limit or provider failure produces bounded failure truth and zero mutation.

### Full candidate verification

A recovery candidate is eligible only when Chronicle can establish all of the following:
- canonical generation ID;
- readable `manifest.json`;
- readable `payload.json`;
- supported package document versions;
- matching target slot ID;
- matching manifest/payload generation ID;
- `Committed` generation state;
- manifest/payload inventory agreement;
- payload byte length agreement;
- active integrity-provider agreement;
- payload integrity/checksum success;
- valid deterministic technical timestamp/order key.

M4-07 may reuse/extract an internal read-only generation-verification primitive if doing so reduces duplicate validation logic. It must not weaken existing publication/current-reader validation semantics.

Do not deserialize participant payload DTOs merely to choose recovery candidates.

### Candidate preservation and exclusion

These remain in place and are excluded from valid recovery candidates:
- noncanonical generation directories;
- missing manifest or payload;
- malformed documents;
- unsupported newer document versions;
- slot/generation identity mismatch;
- candidate/uncommitted state;
- payload inventory mismatch;
- payload length mismatch;
- checksum/integrity failure;
- invalid technical timestamp.

M4-07 does not quarantine or delete them.

### Deterministic candidate ordering

Valid candidates are ordered newest first by:
1. validated manifest technical timestamp;
2. canonical generation ID ordinal tie-break.

If recovery is required, the first candidate is the preferred fallback.

If the observed broken head references a generation that happens to remain discoverable but fails validation, that generation is never preferred.

### Healthy-current behavior

When head and current generation are fully trustworthy:
- plan status reports recovery not required;
- no recovery mutation is proposed;
- no candidate is silently published;
- diagnostics may expose bounded valid-history information, but no fallback execution is implied.

### Missing-head behavior

When head is absent:
- scan retained canonical generations;
- fully verify candidates;
- prefer the newest valid committed generation;
- if none exists, return `NoValidCandidate`;
- preserve every source file.

### Invalid/corrupt-current behavior

When head is invalid, or the referenced current generation cannot be fully verified:
- scan retained canonical generations;
- exclude unverified evidence;
- choose newest valid candidate;
- report the exact observed failure class separately from candidate choice;
- preserve damaged source evidence.

### Immutable recovery plan truth

The plan/result must provide enough technical truth for support/UI and later execution without exposing payload data:
- target `SaveSlotId`;
- plan status;
- stable diagnostic code;
- observed head condition;
- observed current generation ID when trustworthy;
- ordered immutable candidate summaries;
- preferred candidate when applicable;
- verified/rejected/ignored counts;
- bounded messages;
- source-provenance fingerprint/token.

Candidate summaries should contain technical generation identity and safe metadata needed for selection, but no participant payload content or full filesystem path.

### Source provenance

A recovery plan is a snapshot, not timeless authority.

M4-07 must bind the plan to the exact observed source state using package-owned technical evidence such as:
- slot identity;
- exact head presence/state and serialized-byte fingerprint when readable;
- observed current-generation identity when trustworthy;
- deterministic ordered discovered canonical generation identity set;
- selected candidate generation identity;
- a package-owned plan fingerprint/token.

The exact representation is implementation-owned, but it must be deterministic and bounded.

M4-07 captures this provenance. The follow-on execution checkpoint performs stale-plan revalidation and rejection.

## 4. Explicit non-scope

Do not add:
- `ExecuteRecoveryAsync`;
- writing/replacing/deleting `head.json`;
- republishing an existing generation as current;
- catalog refresh caused by recovery execution;
- active-slot selection changes;
- automatic recovery/fallback policy execution;
- recovery mutation admission/Busy/cancellation;
- quarantine movement;
- corrupt/incomplete generation cleanup;
- rename/duplicate/delete/trash;
- persistent `catalog.cache.json`;
- generic operation queues;
- automatic autosave timers;
- full `SaveRecoveryPolicy` or configuration/Setup authoring;
- document or participant migrations as a recovery-selection side effect;
- scene travel;
- bridges;
- service locator;
- Chronicle-owned/project-wide DDOL.

## 5. Safety invariants

Tests must prove:
- valid current head/generation reports recovery not required;
- missing head + valid generations selects newest valid candidate;
- corrupt current + prior valid generation offers prior candidate;
- multiple valid candidates order deterministically newest first;
- timestamp ties break by canonical generation ID deterministically;
- no valid candidate preserves all source files;
- checksum mismatch excludes candidate;
- manifest/payload mismatch excludes candidate;
- unsupported newer document is preserved/excluded;
- candidate/uncommitted generation is preserved/excluded;
- noncanonical child is preserved/excluded;
- discovery-limit failure produces no mutation;
- provider discovery/read failure produces no mutation;
- planner performs zero writes/deletes/tree deletes/publications;
- planner never calls participant capture/apply/default;
- planner does not migrate documents or participant payloads;
- base storage contracts remain unchanged unless an already-approved read capability is reused;
- immutable plan provenance changes when observed head/discovered canonical generation state changes;
- all prior **497 / 497** Chronicle tests remain green.

## 6. Proposed focused proof

Primary registry mapping:
- ESV-T-074 — missing head valid generations → newest valid plan candidate;
- ESV-T-075 — corrupt current prior valid → prior candidate offered;
- ESV-T-076 — multiple valid candidates → deterministic order;
- ESV-T-077 — no candidate → files preserved.

Additional focused proofs:
- healthy current → recovery not required;
- invalid/malformed head;
- current missing;
- payload checksum mismatch;
- manifest/payload inventory mismatch;
- unsupported newer generation;
- uncommitted candidate exclusion;
- noncanonical child preservation;
- bounded discovery failure;
- provider read failure;
- zero mutation audit;
- participant callback absence;
- provenance/fingerprint determinism and change detection.

Executed totals are recorded from Unity, never predicted.

## 7. Stop point

Stop when Chronicle can produce one trustworthy immutable read-only recovery plan that:

1. classifies the observed head/current state;
2. discovers retained history through bounded provider-neutral reads;
3. admits only fully verified committed generations;
4. orders candidates deterministically newest-valid first;
5. chooses a preferred fallback only when recovery is required;
6. carries enough exact source provenance for later stale-plan rejection;
7. changes no durable data.

Do **not** execute recovery yet.

Do **not** rewrite `head.json`, reconcile catalog state, quarantine evidence, or add destructive slot operations.


## 8. Completion Evidence

**Planning baseline:** `9695450`

**Planning/activation commit:** `7b00503`

**Implementation commit:** `9f68555`

**Final effective runtime baseline:** `9f68555`

**Unity compile/import:** Green

**Focused Chronicle Editor gate:** **524 / 524 passed, 0 failed**

**Prior focused regression floor:** **497 / 497**

**Net new focused tests:** **27**

**Committed implementation/test scope:** **22 files**, `2912` insertions, `6` deletions

Observed completion:
- public read-only `BuildRecoveryPlanAsync(SaveSlotId)`;
- explicit healthy/missing/invalid/current-missing/current-invalid diagnosis;
- bounded provider-neutral generation discovery;
- full candidate verification using Chronicle document and integrity rules;
- preservation/exclusion of invalid, unsupported, uncommitted, corrupt, and noncanonical evidence;
- deterministic newest-valid candidate ordering;
- immutable payload-free recovery plan and candidate summaries;
- exact technical source-provenance fingerprint for later stale-plan rejection;
- no durable storage mutation;
- no participant callbacks or migration side effects;
- no recovery execution/head rewrite/catalog reconciliation.

### Test-fixture corrections

1. New test support initially referenced nonexistent `SaveDocumentVersions.HeadMajor`; it was corrected to Chronicle's authoritative `HeadPointerMajor` constant. Runtime/API/architecture/test intent were unchanged.
2. The first focused run discovered **524** tests with **522 passed / 2 failed** because two intentionally unsupported-version fixtures used Chronicle's guarded production serializer, which correctly rejected those documents before serialization. Only the fixture was corrected: supported fixtures still use Chronicle's serializer, while intentionally unsupported future-version JSON is authored directly with Unity `JsonUtility` so the recovery planner can inspect and preserve/exclude it.

Final rerun: **524 / 524 passed, 0 failed**.

No follow-on M4 checkpoint is activated by this closeout.
