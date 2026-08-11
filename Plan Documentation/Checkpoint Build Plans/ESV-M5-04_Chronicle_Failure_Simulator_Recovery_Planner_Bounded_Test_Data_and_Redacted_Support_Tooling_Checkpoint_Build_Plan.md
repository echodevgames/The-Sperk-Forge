# ESV-M5-04 — Chronicle Failure Simulator, Recovery Planner, bounded Test Data, and Redacted Support Tooling — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Milestone:** M5 — Tooling and Laboratory
**Checkpoint:** ESV-M5-04
**Status:** ACTIVE / AUTHORIZED
**Planning baseline:** `ffff18f` — `Close out ESV-M5-03 browser inspector and migration graph`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.50.0 / ESV-D-040
**Incoming focused Chronicle floor:** **735 / 735 passed, 0 failed**
**Unity baseline:** 6000.3.8f1

## 1. Purpose

M5-03 made Chronicle durable truth observable without mutation.

M5-04 now adds the QA/support tools that intentionally manufacture failures or package diagnostic evidence, but only behind explicit sandbox and privacy boundaries.

The governing rule is:

> destructive-looking QA tooling may mutate only an isolated sandbox; production recovery remains preview-only; support export contains no participant payload data.

## 2. In scope

### 2.1 Failure Simulator

Editor tooling for bounded sandbox fixtures such as:
- truncated documents;
- missing manifest/payload/head files;
- orphan generations;
- stale or missing head pointers;
- deterministic integrity mismatches;
- unsupported-newer package-document fixtures;
- aged ordering/timestamp fixtures;
- bounded file-lock/unavailable-state simulation where supported.

Every mutation must identify the sandbox target before Apply.

### 2.2 Recovery Planner

Editor preview over Chronicle's existing recovery-plan authority:
- selected slot;
- current/head diagnosis;
- candidate generation ordering;
- source/provenance evidence;
- exclusion diagnostics;
- proposed explicit recovery result.

No Apply/Recover action is authorized in this checkpoint.

### 2.3 Bounded Test Data Generator

Deterministic synthetic sandbox generation with:
- maximum slot count;
- maximum generations per slot;
- maximum payload/fixture byte size;
- optional deterministic seed;
- explicit Preview before generation;
- explicit cleanup of generator-owned fixtures.

### 2.4 Redacted Snapshot Exporter

Explicit support export containing bounded diagnostic/manifest health truth only.

Required redactions:
- no participant payload contents;
- no unknown payload bytes;
- no full local filesystem paths by default;
- no credentials/secrets;
- technical slot identity hashed/redacted in support mode.

## 3. Sandbox authority

The sandbox root:
- is explicitly resolved and displayed;
- must be outside the configured production Chronicle root;
- must not contain the production root;
- must not be contained by the production root;
- must fail closed if canonical-path comparison is ambiguous;
- is safe to delete only when the tool can prove ownership of the exact generated fixture subtree.

No simulator or generator operation may accept the production root.

## 4. Failure Simulator truth rules

- Preview before mutation.
- Mutation scope is exact and bounded.
- Corruption is deliberate and labeled.
- Generated invalid/future-version JSON may bypass production serializers only in test/sandbox fixture authoring, never production runtime.
- Simulator cleanup deletes only generator-owned sandbox fixture paths.
- Tool reports any residue instead of claiming cleanup succeeded.

## 5. Recovery Planner truth rules

- Planner uses existing recovery authority or an additive read-only DTO over it.
- Candidate ordering matches runtime recovery-plan ordering.
- Unsupported/newer/corrupt candidates remain visible with exclusion reasons.
- Planner does not mutate a head, candidate generation, trash state, or catalog.
- Repeated preview over unchanged files is deterministic.

## 6. Test Data Generator bounds

Required configurable bounds:
- slot count >= 1 and finite;
- generation count >= 1 and finite;
- per-fixture/aggregate byte caps finite;
- no unbounded loops or "unlimited" setting.

Generation must be deterministic for the same seed/options when IDs/timestamps are explicitly normalized for comparison.

## 7. Redacted Snapshot schema

Allowed representative fields:
- Chronicle/package version;
- configuration schema version;
- serializer/storage/integrity provider IDs;
- slot policy mode and capacity;
- catalog health counts;
- selected slot health with hashed/redacted identity;
- selected generation ID only if redacted/support-safe or explicitly development-only;
- package-document versions;
- migration/recovery diagnostic codes;
- root token/path hash rather than full local path;
- exported-at UTC timestamp;
- tool schema version.

Forbidden:
- participant serialized payload;
- unknown payload raw data;
- credentials;
- absolute filesystem paths in support mode.

## 8. Editor surfaces

Target menu entries:

- `Tools > Sperk’s Forge > The Chronicle > Failure Simulator`
- `Tools > Sperk’s Forge > The Chronicle > Recovery Planner`
- `Tools > Sperk’s Forge > The Chronicle > Test Data Generator`
- `Tools > Sperk’s Forge > The Chronicle > Export Redacted Snapshot`

Read-only vs mutating intent must be visually obvious.

## 9. Focused test plan

### Sandbox
- canonical collision with production root refused;
- nested-inside production refused;
- contains-production refused;
- sibling sandbox accepted;
- cleanup verifies no owned residue.

### Failure Simulator
- preview causes zero writes;
- each bounded mutation affects only target sandbox fixture;
- unsupported-newer fixture remains sandbox-only;
- missing/truncated/corrupt scenarios remain inspectable.

### Recovery Planner
- preview causes zero writes;
- candidate order matches runtime authority;
- exclusion reasons deterministic;
- stale-head/current diagnosis accurate.

### Test Data
- configured counts/bytes enforced;
- deterministic seed behavior;
- no production-root writes;
- cleanup deletes only owned fixtures.

### Support export
- payload marker bytes never appear;
- absolute production path never appears in support mode;
- raw technical slot ID never appears in support mode;
- output size bounded;
- schema deterministic.

### Regression
- full `EchoDevGames.EchoSave.Tests.Editor` remains green at actual discovered total;
- total must not fall below **735 / 735**.

## 10. Manual Unity proof

After automated green:

1. Configure a disposable M5-04 sandbox distinct from production.
2. Failure Simulator Preview one bounded corruption.
3. Apply it and inspect the sandbox-only effect.
4. Open Recovery Planner and capture candidate/exclusion preview with no Apply/Recover control.
5. Generate a tiny bounded deterministic test-data set and verify counts/limits.
6. Export one redacted support snapshot.
7. Inspect export text for no payload contents, no absolute path, and no raw slot ID in support mode.
8. Cleanup simulator/generator sandbox state.
9. Verify production Chronicle data was untouched.
10. Verify Git/worktree cleanup.

## 11. Explicitly deferred

M5-04 does not activate:
- production recovery execution;
- catalog persistent cache;
- quarantine/unknown prune/permanent erase/restore-from-trash;
- direct-scene Save Laboratory;
- LAB-001 through LAB-032;
- M5-05 implementation.

## 12. Closeout

Required:
- activation authority committed;
- implementation committed;
- clean compile;
- focused Chronicle tests green >= 735;
- sandbox collision/cleanup tests green;
- privacy/export tests green;
- manual Simulator/Planner/Test Data/Export proof complete;
- documentation reconciled;
- repository clean.

M5 remains open after M5-04.
