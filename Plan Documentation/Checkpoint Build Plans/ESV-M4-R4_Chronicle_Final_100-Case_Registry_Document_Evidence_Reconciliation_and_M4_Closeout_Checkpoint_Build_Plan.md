# ESV-M4-R4 — Chronicle Final 100-Case Registry, Documentation Evidence Reconciliation, and M4 Closeout — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Milestone:** M4 — Slots / Autosave / Recovery Reconciliation
**Checkpoint:** ESV-M4-R4
**Status:** COMPLETE / M4 COMPLETE
**Planning date:** 2026-08-11
**Clean planning baseline:** `e3d7a2e` — `Close out ESV-M4-R3 package-document migration`
**Retained R3 implementation baseline:** `c6ba1ad`
**Incoming focused Chronicle Editor floor:** **660 / 660 passed, 0 failed**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.43.0 / ESV-D-036
**Runtime package version:** `0.1.0`
**Unity baseline:** 6000.3.8f1
**Runtime implementation changes authorized:** **No**
**Test-code changes authorized:** **No**
**M4 status at activation:** OPEN
**M5 status at activation:** LOCKED

---

## 1. Why this checkpoint exists

ESV-M4-R1, R2, and R3 repaired the four runtime/capability blockers discovered by the M4 milestone audit:

- A-01 — public load facade;
- A-02 — public catalog/create/select facade;
- A-03 — CAP-002 slot-policy runtime configuration;
- A-04 — CAP-014 package-document migration.

The remaining M4 blocker is not another known runtime feature gap. It is **evidence/documentation drift**.

The Chronicle package specification contains a 100-case registry spanning installation, lifecycle, configuration, slots, catalog, participants, unknown payloads, save/autosave/commit, load, integrity, migration, recovery, serializers, security/privacy, performance/stress, direct-scene, integration/adoption, and release qualification.

That registry is currently stale in both directions:

1. some behaviors already proven by completed M1-M4 checkpoints are still marked `Planned`;
2. many rows intentionally belong to later M5/Laboratory/release/adoption work and must **not** be marked Complete merely to close M4.

R4 exists to make that distinction explicit, row by row, and to synchronize all current-state documentation before the milestone is allowed to close.

---

## 2. Governing authority decision — ESV-D-036

Final M4 reconciliation is an **evidence classification and documentation parity gate**, not a demand that all 100 lifetime/release registry rows become Complete during M4.

The rules are:

- every ESV-T-001 through ESV-T-100 row is reviewed individually;
- a row becomes `Complete` only from retained direct evidence;
- later M5, Laboratory, clean-project, release, performance/stress, integration/adoption, and otherwise deferred rows remain explicitly not complete;
- R4 may not invent evidence;
- R4 may not alter runtime or test code;
- an M4-applicable row with insufficient proof becomes a blocker and stops closeout;
- a blocker requiring code/test changes receives a separate bounded repair checkpoint;
- M4 closes only after the full registry/document map is reconciled and the final focused Chronicle Editor suite is green at the actual closing total;
- M5 remains locked until the committed R4 closeout records M4 complete.

---

## 3. Inputs that must be treated as retained evidence

R4 must use repository-owned evidence, not recollection.

Primary evidence sources include:

- the Chronicle package specification and revision history;
- all completed ESV-M1 through ESV-M4 checkpoint build plans/closeouts;
- the M4 milestone reconciliation audit and R1/R2/R3 updates;
- current source and test files at `e3d7a2e`;
- retained Unity test totals and named test classes;
- `Plan Documentation/Current Notes.md`;
- package `Documentation~/Developer/Current Notes.md`;
- package README / CHANGELOG / documentation index;
- Suite Health;
- Git commit history for the completed Chronicle checkpoints.

A historical checkpoint total can support a registry row only when the checkpoint's actual tested behavior directly matches that row.

---

## 4. Row-by-row reconciliation record

R4 must produce an explicit disposition for **all 100 rows**.

The reconciliation record for each row must contain at least:

| Field | Required meaning |
|---|---|
| Test ID | Exact `ESV-T-###` |
| Registry scenario | Existing scenario name |
| Expected result | Existing registry expectation |
| Existing status | Current specification status before R4 |
| M4 applicability | Yes / No / Later gate |
| Retained evidence | Named class/checkpoint/evidence record or `None` |
| Reconciled status | Complete / Planned / Deferred / Release-gated / Blocked, as appropriate |
| Evidence note | Short reason with no invented claim |

The original test intent must not be silently rewritten to match whatever happened to be implemented.

If an existing automated test only partially overlaps the registry row, that is not direct proof unless the expected result is actually exercised.

---

## 5. Status rules

### 5.1 Complete

Use `Complete` only when retained evidence directly proves the registry expectation.

Examples of valid proof:

- a named green NUnit test that exercises the scenario/result;
- a completed checkpoint with retained execution evidence explicitly covering that exact case;
- an authoritative retained manual test record for a manual-only scenario.

### 5.2 Planned / Deferred

A row remains not complete when:

- the scenario belongs to M5 Editor tooling or Save Laboratory;
- it requires clean-project reproduction;
- it requires external package installation;
- it requires platform qualification;
- it requires performance/stress measurements not yet run;
- it requires project integration/adoption not yet performed;
- it belongs to release/private-beta qualification;
- the capability itself is intentionally deferred beyond M4.

The exact label used in the final registry/document map must be clear and consistent, but no label may imply executed evidence that does not exist.

### 5.3 Blocked

Use `Blocked` in the reconciliation record when:

- the row is truly M4-applicable;
- its expected behavior should already be implemented;
- retained evidence is missing or contradicts the claim.

A blocked M4-applicable row prevents M4 closeout.

---

## 6. No mass-marking rule

R4 must **not** perform bulk status promotion based on category, checkpoint number, or test count.

Forbidden examples:

- “M3 is complete, therefore every Participant/Load row is Complete.”
- “660 tests pass, therefore every registry row is Complete.”
- “M4 is functionally done, therefore release/install/performance rows are Complete.”
- “A related test exists, therefore the exact registry expectation is proven.”

Each status change must be justified individually.

---

## 7. Runtime and test-code freeze

R4 itself authorizes **zero runtime code changes** and **zero test-code changes**.

Allowed changes are documentation/evidence records only.

If reconciliation discovers a genuine M4-applicable code or evidence gap:

1. record the gap;
2. stop R4 closeout;
3. keep M4 open;
4. create a separately bounded repair checkpoint;
5. implement/test/close that repair;
6. resume R4 from the repaired clean baseline.

This prevents a “documentation closeout” from becoming an unreviewed implementation grab bag.

---

## 8. Documentation parity sweep

R4 must reconcile current-state claims across at least:

1. `Plan Documentation/Package Specifications/SFGSS-The-Chronicle-EchoSave-Package-Specification.md`
2. `Plan Documentation/Current Notes.md`
3. `Packages/com.echodevgames.echo-save/Documentation~/Developer/Current Notes.md`
4. `Plan Documentation/Milestone Reconciliations/ESV-M4_Chronicle_Milestone_Reconciliation_Audit.md`
5. `Plan Documentation/Suite_Health_Check_and_Remaining_Documentation.md`
6. `Packages/com.echodevgames.echo-save/README.md`
7. `Packages/com.echodevgames.echo-save/CHANGELOG.md`
8. package documentation index/navigation files that describe current implementation state
9. R1/R2/R3/final checkpoint records where current-state wording is stale
10. the package specification new-conversation handoff/current status record

Historical records should remain historical. Current-state contradictions should be repaired without rewriting history.

---

## 9. Specific stale documentation already known at activation

The repository already shows at least these known drift classes:

- the 100-case registry still has many `Planned` rows for behaviors already proven by M3/M4;
- package README current-boundary text is behind the R2/R3 closeout state;
- package documentation/CHANGELOG/index current-state claims require reconciliation against the final M4 capability map;
- the audit must transition from “runtime blockers plus registry drift” to the final resolved capability/evidence truth if R4 passes;
- Suite Health and both Current Notes records must agree on whether M4 is open/closed and what comes next.

R4 must discover the exact affected files/rows rather than assuming this list is exhaustive.

---

## 10. Required final focused test gate

After the documentation/evidence map is prepared, rerun:

```text
EchoDevGames.EchoSave.Tests.Editor
```

Incoming floor:

```text
660 / 660 passed
0 failed
```

R4 must record the **actual discovered closing total**.

Because R4 adds no test code, the count is normally expected to remain 660. That is an expectation, not an authority claim.

If discovery differs:

- record the actual total;
- determine why;
- do not edit documentation to force the old count;
- do not close M4 while unexplained failures or missing tests remain.

---

## 11. M4 capability closeout matrix

R4 must produce one final M4 capability/evidence matrix covering, at minimum:

- package-local lifecycle/duplicate authority;
- path-safe local storage;
- package documents/serializer/integrity;
- immutable generation/head-last publication;
- participant registry/capture/publication;
- unknown payload preservation/carry-forward;
- participant preparation/migration/prepared-load/apply;
- catalog discovery/active selection;
- slot creation/capacity;
- manual save/public admission/cancellation;
- autosave coalescing;
- generation retention;
- recovery planning/execution;
- rename/duplicate;
- delete planning/recoverable trash/trash retention;
- public R1 facade composition;
- schema-2 slot policy / schema-1 compatibility;
- package-document migration;
- explicit deferred M5/release/integration/performance work.

The matrix must reference retained evidence and must not imply deferred capabilities exist.

---

## 12. M4 close conditions

M4 may be declared **Complete** only when every item below is satisfied:

- [x] A-01 remains resolved.
- [x] A-02 remains resolved.
- [x] A-03 / CAP-002 remains resolved.
- [x] A-04 / CAP-014 remains resolved.
- [x] ESV-T-001 through ESV-T-100 each has an explicit reconciled disposition.
- [x] Every M4-applicable row has retained direct evidence.
- [x] No later M5/release/adoption row is falsely marked Complete.
- [x] Package README current boundary is accurate.
- [x] CHANGELOG current state is accurate.
- [x] Package documentation index/navigation current state is accurate.
- [x] Current Notes records agree.
- [x] Suite Health agrees.
- [x] M4 audit final state agrees.
- [x] Package-specification handoff/current-status records agree.
- [x] Final M4 capability/evidence matrix is complete.
- [ ] Final focused Chronicle Editor suite is green at the actual closing total.
- [x] No runtime/test-code change was smuggled into R4.
- [x] Git closeout scope is documented; the closeout commit/push is the final repository action for this record.

If any checkbox remains unresolved, M4 stays open.

---

## 13. M5 gate

M5 is not activated by:

- R4 activation;
- completion of the row map;
- a green Unity rerun;
- a local documentation edit.

M5 becomes eligible only **after the committed R4 closeout records M4 complete**.

Any M5 work then begins through its own authority/checkpoint activation.

---

## 14. Expected Git sequence

### Activation

Documentation authority only:

```text
e3d7a2e
  -> Activate ESV-M4-R4 final registry/document reconciliation
```

### Reconciliation

Expected to be documentation-only unless a separate repair checkpoint becomes necessary.

The final reconciliation commit should include the row mapping, current-state documentation parity, and actual final focused test evidence.

### M4 closeout

Preferred commit message:

```text
Close out ESV-M4-R4 and Chronicle M4 reconciliation
```

Only use that message if the M4 close conditions actually pass.

---

## 15. Rollback rule

R4 documentation helpers must:

- require the expected clean baseline;
- refuse unrelated tracked/staged/untracked changes;
- create parent directories for new files;
- validate generated text for trailing whitespace and final newline;
- run `git diff --check`;
- verify exact changed-file scope;
- leave changes unstaged;
- restore the exact baseline on failure;
- verify rollback succeeded before reporting success.

Rollback is not considered successful merely because restore/delete commands were issued.

---

## 16. Activation completion record

When this plan is committed, record:

- planning baseline `e3d7a2e`;
- activation commit;
- specification v1.41.0 / ESV-D-036;
- incoming focused floor `660 / 660`;
- documentation-only authority scope;
- R4 active / M4 open / M5 locked.

Do **not** record:

- final row dispositions before they are actually reconciled;
- a final Unity total before it is rerun;
- M4 complete before every close condition passes;
- M5 active before a later explicit activation.

---

## 17. Immediate next action after activation

1. Build the 100-row evidence map from retained repository evidence.
2. Identify every stale registry status.
3. Identify every row intentionally owned by later gates.
4. Identify any true M4-applicable evidence gap.
5. If no gap exists, prepare the documentation reconciliation bundle.
6. Rerun the focused Chronicle Editor suite.
7. Close R4/M4 only from the actual resulting evidence.

## 18. R4 evidence reconciliation record

**Activation commit:** `81c53dd`
**Evidence-map authority:** SFGSS-PKG-ECHOSAVE-001 v1.42.0 / ESV-D-036
**Registry result:** **61 Complete / 39 Deferred / 0 Blocked**
**Runtime changes:** none
**Test-code changes:** none

The 100-row pass is retained in `Plan Documentation/Milestone Reconciliations/ESV-M4-R4_Chronicle_100-Case_Registry_Evidence_Matrix.md`.

The 39 Deferred rows are all assigned to later explicit gates. No M4-applicable row lacks retained direct evidence, so R4 does not activate a repair checkpoint.

Documentation parity is reconciled across the package README, CHANGELOG, documentation index, root/package Current Notes, Suite Health, milestone audit, R4 plan, and package specification.

The fresh focused Chronicle Editor rerun passed **660 / 660**, with **0 failed**. The row map remains **61 Complete / 39 Deferred / 0 Blocked** and no M4-applicable repair checkpoint is required. This R4 record closes Chronicle M4. M5 is eligible for separate activation but is not automatically active.

## 19. Final R4 / M4 closeout evidence

**Fresh focused Chronicle Editor rerun:** **660 / 660 passed, 0 failed**
**Test Runner discovery context:** **1005 EditMode tests discovered in the project; 345 outside the selected Chronicle assembly were not run in this focused gate**
**Final registry:** **61 Complete / 39 Deferred / 0 Blocked**
**Runtime changes in R4:** none
**Test-code changes in R4:** none
**Repair checkpoint required:** no
**M4 disposition:** **Complete**
**M5 disposition:** eligible for separate activation; not automatically active

The 39 Deferred rows remain real later-gate obligations. This closeout does not claim Laboratory, clean-project, distribution, performance/stress, integration/adoption, or release qualification that has not yet been executed.
