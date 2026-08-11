# ESV-M4-R3 — Chronicle Package-Document Migration and CAP-014 Reconciliation — Checkpoint Build Plan

**Package:** `com.echodevgames.echo-save`
**Public title:** The Chronicle — Save Infrastructure
**Checkpoint:** ESV-M4-R3 — Package-Document Migration and CAP-014 Reconciliation
**Milestone:** M4 — Slots / Autosave / Recovery Reconciliation
**Status:** **ACTIVE / AUTHORIZED**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.39.0
**Authority decision:** ESV-D-035
**Clean planning baseline:** `0ebf1a1`
**Incoming focused Chronicle regression floor:** **636 / 636 passed, 0 failed**
**Unity baseline:** 6000.3.8f1
**M4 state:** Open
**M5 state:** **LOCKED**
**Updated:** August 11, 2026

---

## 1. Purpose

ESV-M4-R3 closes audit blocker **A-04** by implementing the missing package-document half of CAP-014.

CAP-014 remains intact:

```text
Migration chains
  document migration
  participant migration
  contiguous upgrade only
```

Participant migration already exists. R3 adds migration for Chronicle-owned package documents while preserving:

- immutable source generations;
- deterministic contiguous upgrade chains;
- exact package-document version identity;
- existing current-version validators;
- existing participant migration;
- existing two-phase load semantics;
- existing catalog/recovery read-only behavior;
- R1/R2 public API compatibility;
- existing integrity and head-last publication truth.

R3 does not add M5 tooling, scene ownership, generic queues, automatic rewrite-on-load, downgrade, or peer-package dependencies.

---

## 2. Activation Baseline

R3 begins from:

```text
0ebf1a1  Close out ESV-M4-R2 slot policy runtime configuration
```

Starting truth:

- local and remote `main` synchronized at `0ebf1a1`;
- working tree clean;
- R2 complete;
- CAP-002 reconciled;
- focused Chronicle Editor floor **636 / 636**;
- A-04 / CAP-014 package-document migration still open;
- final 100-case registry/document reconciliation still required after R3;
- M4 open;
- M5 locked.

This activation advances Chronicle authority to **v1.39.0** and records **ESV-D-035**.

---

## 3. Audit Disposition

The M4 audit offered:

- Path A: implement package-document migration and preserve CAP-014;
- Path B: split CAP-014 into participant/document capabilities.

R3 selects **Path A**.

R3 must not weaken the approved package capability just to simplify milestone closure.

---

## 4. Migration Ownership Boundary

### Package-document migration

Chronicle-owned because Chronicle owns these document shapes:

- `SaveDocumentEnvelope`;
- `SaveManifest`;
- `SavePayloadDocument`;
- `SaveHeadPointer`.

Package-document migration upgrades Chronicle-owned serialized structure/version.

### Participant migration

Existing participant migration remains separate.

It upgrades:

- participant IDs/aliases;
- participant schema versions;
- participant-owned payload meaning.

### Ordering

When both are needed:

```text
read/verify source
  -> package-document migration in memory
  -> exact-current package validation
  -> participant payload view
  -> participant migration
  -> prepare/preflight
  -> participant apply
```

Package migration never invokes participant callbacks.

---

## 5. Exact Package Version Model

Each package document kind has its own exact:

```text
Major.Minor.Revision
```

Current production versions remain:

```text
Envelope    1.0.0
Manifest    1.0.0
Payload     1.0.0
HeadPointer 1.0.0
```

R3 may add a small immutable version value such as:

```text
SavePackageDocumentVersion
```

Required behavior:

- exact equality;
- deterministic comparison;
- non-negative bounded components;
- stable diagnostic formatting;
- no relationship to participant schema version numbers.

Document kind is always part of migration lookup identity.

---

## 6. No Synthetic Format Bump

R3 must **not** bump production package documents from `1.0.0` merely to create a test fixture.

A production format version is durable save-contract truth.

Therefore:

- production migration infrastructure is implemented now;
- the built-in production step set may be empty at R3 closeout;
- focused tests may inject internal deterministic fixture steps;
- fixtures must be clearly test-only;
- a future real document-shape change owns the real version bump and production step.

This prevents fictional format history.

---

## 7. Migration Step Contract

One package migration step represents:

```text
DocumentKind + ExactSourceVersion -> ExactTargetVersion
```

A step has:

- stable package-owned step ID;
- exact document kind;
- exact source version;
- exact target version;
- deterministic transformation of detached serialized package-document content.

A step must not:

- write storage;
- publish a generation;
- rewrite head;
- invoke participants;
- access scenes/project services;
- depend on peer packages;
- mutate runtime configuration.

Step output must:

- be nonempty;
- remain bounded;
- preserve document kind;
- declare exactly the step target version;
- be eligible for the next exact step or final current validation.

Exceptions or invalid output become structured migration failure.

---

## 8. Package-Owned Registry and Chain Rules

Package migration steps are package-owned.

R3 must not expose arbitrary consumer/project registration for Chronicle document migration.

The internal registry/chain authority must:

- resolve by document kind + exact source version;
- permit at most one outbound edge for an exact source;
- reject duplicate/ambiguous edges;
- enforce exact continuity;
- detect loops;
- be bounded;
- terminate only at the exact current version;
- reject downgrade edges.

Legal chain:

```text
stored version
  -> exact step target
  -> exact next source
  -> ...
  -> exact current version
```

No gap, branch, implicit skip, or best-effort partial chain is allowed.

---

## 9. Migration-Aware Read Seam

Preferred architecture is one internal package-document read seam.

Conceptually:

```text
bounded source bytes/text
  -> probe document kind + exact stored version
  -> current?
       yes: existing deserialize + current validation
       no: resolve complete package-owned chain
           -> execute in memory
           -> validate each step result
           -> deserialize exact-current result
           -> existing current validation
  -> existing catalog/load/recovery consumer
```

Historical normalization happens **before** exact-current DTO validation.

`SavePackageDocumentValidator.ValidateCurrent` remains strict.

R3 should compose around current validation, not weaken it.

---

## 10. Version Probe

The reader needs a bounded way to identify:

- package document kind;
- major;
- minor;
- revision.

The probe must not require successful deserialization into the exact-current DTO shape.

Requirements:

- bounded input;
- structured malformed-version failure;
- no storage mutation;
- no arbitrary large allocation;
- deterministic result.

A small package-owned probe DTO/parser is acceptable.

---

## 11. Serializer Boundary

`UnityJsonSaveSerializer` currently validates package documents as current during package document serialization/deserialization.

R3 may:

- place a migration-aware reader around it; or
- add a narrowly scoped internal historical-normalization seam needed before current deserialization.

R3 may not:

- weaken public/current deserialize validation;
- expose a general unvalidated public deserialize bypass;
- convert package migration into participant serializer policy.

The exact implementation should minimize churn.

---

## 12. Source Immutability

Migration is read-time normalization only.

On both success and failure, R3 must not rewrite:

- package document source files;
- generation directory identity;
- source generation ID;
- source slot ID;
- source timestamps;
- `head.json`;
- prior generations.

A successful migrated read does not publish an upgraded generation.

The next separately requested successful save writes current package documents through the existing immutable-generation/head-last pipeline.

No “load and upgrade in place” behavior is authorized.

---

## 13. Integrity and Commit Truth

Migration cannot bypass source verification.

Preserve existing:

- file/input bounds;
- checksums/integrity;
- commit-state requirements;
- slot/generation identity checks;
- manifest/payload relationships;
- participant-entry bounds/checksums;
- current package-document validation.

Where an integrity assertion is defined over stored bytes, validate it against the stored bytes.

After migration, the resulting current document must still pass the applicable current-shape validators before use.

---

## 14. Failure and Newer-Version Behavior

R3 fails closed when:

- kind/version cannot be probed;
- stored version is unsupported/newer;
- no complete contiguous chain exists;
- registry has duplicate/ambiguous edges;
- chain loops/exceeds its bound;
- a step reports failure;
- a step throws;
- step output is empty/oversized;
- output changes document kind;
- output version differs from declared target;
- final current deserialization/validation fails;
- existing downstream integrity/commit validation fails.

Failure preserves the source.

No downgrade is authorized.

R3 also does not invent forward-compatible reading of unknown newer package versions. Any future forward-tolerance policy requires separate explicit authority.

---

## 15. Diagnostics and Provenance

Each executed step should produce bounded diagnostic/provenance context:

- stable step ID;
- document kind;
- exact source version;
- exact target version;
- chain position/count where useful;
- terminal status/failure category.

Do not log:

- participant payload contents;
- arbitrary serialized document contents;
- secrets/tokens.

Migration provenance is read/operation evidence. It does not imply durable source rewrite.

---

## 16. Integration Targets

R3 reviews every existing path that reads package-owned versioned documents.

At minimum inspect:

- prepared/convenience load;
- catalog manifest/head reads;
- recovery candidate verification;
- shared serializer/document helpers.

### Catalog

Catalog remains observational:

- no write because migration was needed;
- no participant callbacks;
- no automatic recovery;
- unsupported/unmigratable source becomes truthful degraded/unsupported evidence.

### Recovery

Recovery planning remains read-only:

- migration may normalize a candidate in memory;
- migration does not publish head;
- migration does not choose/execute recovery;
- candidate source remains immutable.

---

## 17. Document-Kind Boundaries

### Head pointer

May migrate package-owned pointer structure only.

In-memory head migration never republishes `head.json`.

### Manifest

May normalize package-owned manifest structure only.

It must preserve truthful slot/generation identity and source payload provenance.

### Payload document

May normalize the package-owned payload container shape.

It must not reinterpret participant payload semantics.

After package normalization, participant IDs/schema versions remain inputs to existing participant migration.

### Envelope

Uses the same exact-version rules and must not become a generic object-migration facility.

---

## 18. Public API Compatibility

R3 should not require a breaking `IEchoSaveService` change.

Preferred implementation is internal migration/read infrastructure composed below existing public methods.

R3 must not:

- expose consumer package-migration registration;
- change `ISaveParticipant`;
- redesign participant migration;
- change slot policy;
- add scene/DDOL authority;
- add generic queues;
- add M5 tooling.

Additive diagnostics/result data is allowed only if genuinely needed and source-compatible.

---

## 19. R3 Test Registry Ownership

R3 directly targets:

- **ESV-T-067** — current package version, no migration;
- **ESV-T-068** — contiguous document chain migrates in memory;
- **ESV-T-069** — missing document step blocks, source unchanged;
- **ESV-T-072 (document side)** — migration step failure/throw blocks, source unchanged;
- **ESV-T-073** — newer package format refused/preserved.

Existing participant migration evidence remains separate for:

- **ESV-T-070** — participant chain;
- **ESV-T-071** — participant alias ID.

Final registry/document reconciliation after R3 owns the authoritative evidence mapping across the full applicable 100-case registry.

---

## 20. Additional Focused Proof

Focused tests should prove:

1. exact-current path executes zero steps;
2. one-step chain;
3. multi-step contiguous chain;
4. missing first/middle step;
5. duplicate/ambiguous edge rejection;
6. loop/step-count safety;
7. wrong-kind output rejection;
8. wrong-target-version rejection;
9. empty/invalid output rejection;
10. step failure result;
11. step exception conversion;
12. unsupported newer version refusal;
13. source unchanged after migration success;
14. source unchanged after migration failure;
15. final migrated document passes exact-current validation;
16. direct current validator remains strict;
17. package migration precedes participant migration;
18. no participant callbacks during package migration;
19. catalog integration remains mutation-free;
20. recovery planning remains mutation-free;
21. R1/R2 public service compatibility;
22. focused Chronicle pass count remains at or above **636 / 636**.

Exact test count is not pre-authorized or predicted. Unity-discovered evidence is recorded at closeout.

---

## 21. Test Fixture Strategy

Because all current production package documents are `1.0.0`, focused migration engine tests should use test-only internal fixtures.

Preferred pattern:

```text
test source version A
  -> fixture step
  -> test version B
  -> fixture step
  -> test current target
```

Tests should use production registry/planner/executor/reader machinery with controlled internal step sets.

Do not edit `SaveDocumentVersions` merely for tests.

Integration fixtures may contain historical-shaped serialized text only when clearly test-local.

---

## 22. Expected Implementation Areas

Likely new package-owned types may include equivalents of:

```text
SavePackageDocumentVersion
ISavePackageDocumentMigrationStep
SavePackageDocumentMigrationRegistry
SavePackageDocumentMigrationPlan
SavePackageDocumentMigrationResult
SavePackageDocumentMigrationProvenance
SavePackageDocumentMigrationCoordinator
SavePackageDocumentVersionProbe
SavePackageDocumentReader
```

Exact names/file split are implementation-detail flexible.

Likely existing seams to review:

```text
Runtime/Documents/SaveDocumentVersions.cs
Runtime/Documents/ISavePackageDocument.cs
Runtime/Documents/SavePackageDocumentValidator.cs
Runtime/Serialization/UnityJsonSaveSerializer.cs
Runtime/Preparation/*
Runtime/Catalog/*
Runtime/Recovery/*
Tests/Editor/*
```

Modify only the seams genuinely required.

---

## 23. Recommended Implementation Sequence

1. version value;
2. migration step contract;
3. registry validation;
4. deterministic chain planner;
5. executor/result/provenance;
6. bounded kind/version probe;
7. migration-aware package reader;
8. chain-engine focused tests;
9. prepared-load integration;
10. catalog/recovery integration where required;
11. integration/regression tests;
12. Unity compile;
13. focused Chronicle EditMode gate;
14. implementation commit;
15. adjacent documentation closeout.

Failure behavior should be proved before broad integration.

---

## 24. Gates

### Compile gate

After implementation apply:

- allow Unity to refresh completely;
- stop on any red compile error;
- repair only the bounded issue;
- rerun affected tests after a code hotfix;
- do not commit until compile is green.

Restore Unity-generated `.slnx`/project noise unless separately intentional.

### Focused test gate

Required:

```text
0 failed
actual passing total >= 636
```

Record actual Unity-discovered totals. Do not predict the final count.

### Git scope gate

Before implementation commit:

- `git diff --check`;
- inspect exact changed paths;
- stage only R3 implementation/test files;
- keep activation, implementation, and closeout commits separate.

Recommended implementation commit:

```text
Implement ESV-M4-R3 package-document migration
```

---

## 25. Documentation Closeout

After green implementation evidence, reconcile:

- Chronicle package specification;
- root Current Notes;
- package Developer Current Notes;
- M4 reconciliation audit;
- Suite Health;
- this R3 checkpoint plan.

Record:

- activation commit;
- implementation commit;
- actual focused test total;
- actual new R3 test count;
- implementation file scope;
- any bounded compile/test hotfix;
- A-04/CAP-014 outcome;
- ESV-T-067/068/069/072(document)/073 evidence;
- remaining final registry/document reconciliation.

---

## 26. Completion Rule and Remaining Lock

R3 closes only when:

- production package-document migration infrastructure exists;
- exact-current documents remain a zero-migration path;
- supported older package documents traverse deterministic contiguous chains in memory;
- invalid/missing/newer paths fail closed;
- source generations remain unchanged;
- package migration precedes separate participant migration;
- required catalog/load/recovery reads use the migration-aware seam;
- focused Chronicle tests are green at or above **636 / 636**;
- R3-owned registry cases have retained evidence;
- documentation matches the committed runtime truth.

R3 closeout does **not** complete M4 automatically.

After R3, one final reconciliation pass must:

- map the applicable 100-case registry to retained evidence;
- repair stale test/document status;
- record final Chronicle regression totals;
- verify no approved M4 capability remains partial;
- then decide whether M4 can close.

M5 remains locked until clean M4 closeout.

---

## 27. Explicitly Out of R3

- fabricated package-document version bump solely for tests;
- participant migration redesign;
- project/consumer package-document migration registration;
- automatic rewrite-on-load;
- downgrade;
- fixed-slot provisioning;
- runtime slot-policy mutation;
- automatic recovery fallback;
- persistent `catalog.cache.json`;
- permanent erase/public trash restore;
- quarantine cleanup;
- generic operation queues;
- automatic autosave timers;
- permission-provider production wiring;
- Setup/Validator/Browser/Simulator/Laboratory UI;
- scene travel;
- peer-package bridges;
- service-locator behavior;
- Chronicle-owned/project-wide DDOL;
- M5 implementation.

---

## 28. Activation Record

**Activated:** August 11, 2026
**Clean baseline:** `0ebf1a1`
**Incoming focused floor:** **636 / 636**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.39.0 / ESV-D-035
**Target:** A-04 / CAP-014 package-document migration
**R3 state:** ACTIVE / AUTHORIZED
**M4 state:** OPEN
**M5 state:** LOCKED

No runtime files are changed by this activation bundle.
