# ESV-M5-03 — Chronicle Save Browser, Generation Inspector, and Migration Graph — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Milestone:** M5 — Tooling and Laboratory
**Checkpoint:** ESV-M5-03
**Status:** COMPLETE
**Planning baseline:** `b4d4d0b` — `Close out ESV-M5-02 full setup and repair previews`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.49.0 / ESV-D-039
**Incoming focused Chronicle floor:** **724 / 724 passed, 0 failed**
**Unity baseline:** 6000.3.8f1

## 1. Purpose

M5-01 established safe Editor infrastructure.
M5-02 completed bounded project configuration authoring and selected-reference repair.

M5-03 now makes Chronicle's durable state observable without granting new mutation authority.

The governing rule is:

> inspection may reveal Chronicle truth, but it may not repair, recover, migrate, erase, restore, or otherwise change that truth.

## 2. In scope

### 2.1 Save Browser

Provide an Editor window/service that:
- refreshes from real Chronicle catalog/slot truth;
- lists discovered slots deterministically;
- shows stable slot identity and project-facing display metadata separately;
- shows current/head generation identity where present;
- shows high-level health/recovery/trash state;
- selects one slot for deeper inspection;
- performs zero writes during refresh, sorting, filtering, or selection.

### 2.2 Generation Inspector

For a selected slot:
- enumerate committed generation directories/evidence deterministically;
- identify current head versus historical generations;
- show immutable generation metadata;
- inspect manifest/package-document versions;
- show verification/support status;
- show recovery candidacy/reasoning already owned by Chronicle;
- surface incomplete/corrupt/unsupported-newer/missing-migration/stale-head truth.

No generation or package document may be rewritten.

### 2.3 Migration Graph

Provide a deterministic representation of registered migration truth:
- migration step source version;
- target version;
- registration identity;
- edges;
- reachable path(s) to current package-document version;
- gaps;
- duplicates/ambiguity;
- cycles if structurally possible;
- unsupported-newer states.

The graph never executes production migration.

### 2.4 Narrow read-only runtime seam

When required, add additive read-only DTO/query surfaces that:
- expose copied immutable snapshots;
- do not leak mutable internal collections;
- do not add mutation commands;
- do not change `IEchoSaveService` operational semantics;
- do not reference UnityEditor from Runtime.

## 3. Explicitly out of scope

M5-03 does not implement:
- create/duplicate/delete/restore/recover operations from the Browser;
- head mutation;
- production migration execution;
- Failure Simulator;
- Recovery Planner execution UI;
- Test Data Generator;
- support/redacted export;
- persistent `catalog.cache.json`;
- cleanup/quarantine/permanent erase/restore-from-trash;
- direct-scene Save Laboratory;
- LAB-001 through LAB-032;
- scene travel, peer bridges, service-locator behavior, or Chronicle-owned/project-wide DDOL;
- package document version changes;
- participant contract changes.

## 4. Browser truth rules

- physical paths come only from Chronicle slot/generation authority;
- display labels never become path authority;
- sorting/filtering never alters identity;
- stale/corrupt entries remain visible with explicit status instead of being silently dropped when doing so would hide evidence;
- refresh is deterministic for identical underlying files;
- Browser health wording maps to structured Chronicle truth rather than ad-hoc string heuristics.

## 5. Inspector truth rules

- committed generation bytes are immutable evidence;
- the current head is displayed as a pointer relationship, not as ownership of generation bytes;
- manifest/package-document reads are read-only;
- failed/unsupported reads preserve reason codes;
- recovery candidacy may be shown only from existing recovery-plan logic or an equivalent read-only snapshot;
- inspection never changes modification timestamps or durable files.

## 6. Migration graph rules

- graph nodes are package-document versions actually represented by migration authority;
- graph edges are registered steps, not inferred semantic guesses;
- reachable-path display must be deterministic;
- missing path is explicit;
- duplicate/ambiguous edge registration is explicit;
- future/unsupported-newer versions fail closed;
- graph visualization is separate from executing migration.

## 7. Editor UX

Target menu surfaces:

- `Tools > Sperk’s Forge > The Chronicle > Save Browser`
- Generation Inspector integrated into or opened from Browser selection
- Migration Graph integrated as a tab/panel or dedicated window

The UI should make read-only status unmistakable.

Useful actions:
- Refresh
- select slot
- select generation
- copy stable identifiers/details
- open migration graph

Forbidden actions in M5-03:
- Repair
- Recover
- Delete
- Restore
- Change Head
- Run Migration
- Rewrite Manifest

## 8. Test plan

Focused tests should cover at least:

### Read-only query/model
- deterministic slot snapshot ordering;
- snapshot collection cannot mutate runtime internals;
- head/generation relationship truth;
- incomplete/corrupt slot remains represented;
- unsupported-newer package document is explicit;
- missing migration path is explicit.

### Browser
- refresh performs zero writes;
- selection performs zero writes;
- filtering/sorting preserves slot identity;
- display label never changes physical identity/path;
- stale/invalid entries receive deterministic status.

### Generation Inspector
- enumerates immutable generations;
- current head relationship shown accurately;
- historical generation remains inspectable;
- corrupt/incomplete generation reason displayed;
- recovery-candidate snapshot is read-only;
- inspection does not alter timestamps/files.

### Migration Graph
- empty registry/current-version-only state;
- single-step path;
- multi-step path;
- missing path;
- duplicate/ambiguous edge;
- cycle/invalid registration handling where applicable;
- unsupported-newer source.

### Regression
- full `EchoDevGames.EchoSave.Tests.Editor` remains green at actual discovered total;
- total must not fall below **724 / 724**.

## 9. Manual Unity proof

After automated green:

1. Create disposable Chronicle save data through an existing safe test/manual flow.
2. Open Save Browser and Refresh.
3. Capture selected slot showing stable identity/head/health truth.
4. Open Generation Inspector and inspect at least one committed generation.
5. Confirm generation/package-document inspection causes no project/save-file mutation.
6. Open Migration Graph and capture the actual registered graph/path state.
7. Refresh repeatedly and verify deterministic ordering/state.
8. Remove disposable proof data.
9. Verify `git status --short` and `git diff --check` are clean except intended implementation changes.

## 10. Closeout requirements

M5-03 closes only when:
- activation authority is committed;
- implementation is committed;
- Unity compiles cleanly;
- focused Chronicle Editor suite is green and not below 724;
- manual Browser/Inspector/Graph proof is recorded;
- zero-write inspection proof is recorded;
- README/CHANGELOG/Documentation Index/both Current Notes/Suite Health/specification/checkpoint are reconciled;
- no Simulator/Laboratory mutation behavior is introduced.

M5-03 completion does not complete M5.

## 11. Next-gate rule

M5-04 — Failure Simulator, Recovery Planner, Test Data, and Support Snapshot tooling — requires a separate activation after M5-03 closeout.


## 12. Closeout record

**Activation commit:** `e805ae3`
**Implementation commit:** `9c3771c`
**Closing authority:** SFGSS-PKG-ECHOSAVE-001 v1.49.0 / ESV-D-039
**Focused Chronicle Editor gate:** **735 / 735 passed, 0 failed**
**Incoming floor:** **724 / 724**
**Net-new focused tests:** **11**
**Implementation scope:** **26 files**, `2419` insertions, `0` deletions

### Automated evidence

- Unity compiled cleanly.
- `EchoDevGames.EchoSave.Tests.Editor` passed **735 / 735**, `0` failed.
- M5-03 added **11** focused tests over the incoming `724 / 724` floor.
- The implementation retained zero Runtime `UnityEditor` references.
- Missing-root read-only inspection, deterministic refresh, migration-graph current-only state, and fail-closed inspection behavior are covered by focused tests.

### Manual Browser proof

With a disposable schema-3 `EchoSaveConfiguration` selected, Save Browser refresh against an absent production root reported:

- `Catalog: SucceededEmpty`;
- `Slots (0)`;
- no discovered technical slots;
- explicit message that the production save root was absent and no directory was created.

This proves the Browser's missing-root path is read-only and no-create.

### Manual Migration Graph proof

The production Migration Graph reported:

- `Registry: Valid`;
- `Registered Edges: 0`;
- `echosave.envelope` current version `1.0.0`, registered edges `0`;
- `echosave.manifest` current version `1.0.0`, registered edges `0`;
- `echosave.payload` current version `1.0.0`, registered edges `0`;
- `echosave.head` current version `1.0.0`, registered edges `0`.

This is the truthful current-only package-document state. No migration was executed.

### Manual Generation Inspector proof

Because Chronicle intentionally has no player-facing save UI in this package, one temporary Editor-only proof seeder was used solely to create a disposable real technical slot through Chronicle's existing slot-creation authority.

Proof identifiers:

- slot: `c4623739-1627-4556-af58-77a5fb7df34b`;
- generation: `20260811T2226264786596Z-0000000000000001-b565851fd2294a49b10043e48139435f`.

Save Browser reported:

- `Slots (1)`;
- slot health `Healthy`;
- current generation matching the seeded generation;
- `0 / 238 bytes`.

Generation Inspector reported:

- status `Succeeded`;
- generation marked `CURRENT`;
- generation status `Healthy`;
- manifest version `1.0.0 -> 1.0.0`;
- `Migrated In Memory: No`;
- commit state `Committed`;
- `0 participants / 238 bytes`.

### Cleanup proof

After the screenshot evidence:

- the recorded disposable slot was removed;
- the production `EchoSave` proof root returned to absent/empty;
- Browser again reported `SucceededEmpty` / `Slots (0)`;
- the temporary Editor proof seeder `.cs` and `.meta` were removed;
- the disposable `EchoSaveConfiguration.asset` and `.meta` were removed;
- the temporarily dirtied First Light showcase scene was restored;
- generated `The Sperk Forge.slnx` noise was restored;
- final `git status --short` emitted no output;
- final `git diff --check` emitted no output.

### Disposition

All M5-03 closeout requirements are satisfied.

**ESV-M5-03 is Complete.**

M5 remains open. M5-04 is not activated by this closeout and requires a separate authority checkpoint.
