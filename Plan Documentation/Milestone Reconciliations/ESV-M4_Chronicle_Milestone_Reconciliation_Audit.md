# ESV — M4 Milestone Reconciliation Audit

**Package:** The Chronicle (`EchoSave`)
**Repository baseline audited:** `48454ea` — `Close out ESV-M4-10 destructive slot deletion and trash`
**Unity focused regression floor:** `587 / 587`
**Audit date:** 2026-08-11
**Audit type:** Documentation/authority/runtime-surface reconciliation
**Mutation performed:** None

## Executive conclusion

**M4 should NOT be declared complete yet.**

The M4-10 checkpoint itself is cleanly closed, but the milestone audit found older specification promises that do not match the current public runtime surface. This is exactly the kind of drift the dedicated reconciliation gate was intended to catch.

The most important finding is not a broken M4-10 implementation. It is that earlier foundational work was correctly implemented in narrow internal layers, while the package specification still describes a broader public MVP surface as though it already exists.

M5 should remain locked until these differences are resolved deliberately.

---

# 1. High-priority blockers

## A-01 — Public load facade is missing

The specification's public API contract includes:

- `PrepareLoadAsync(SaveLoadRequest)`
- `ApplyPreparedLoadAsync(PreparedSaveLoad, ApplyLoadOptions)`
- `LoadAndApplyAsync(SaveLoadRequest)`

The current `IEchoSaveService` does not expose any of those operations.

The repository does contain the internal prepared-load, preparation, migration, and apply machinery built during M3. The gap is therefore not "loading was never engineered." The gap is that the promised public service composition/facade was never completed before M3 was declared complete.

**Audit classification:** BLOCKER
**Affected capabilities:** CAP-012, CAP-013
**Recommended resolution:** implement the bounded public load facade over the already-proven M3 machinery rather than weakening the package specification.

---

## A-02 — Public catalog/create/select facade is incomplete

The specification's novice/public flow says a consumer should be able to:

1. create a slot,
2. select it,
3. save,
4. load it.

The current `IEchoSaveService` exposes save/autosave/recovery/rename/duplicate/delete, but does not expose the specified catalog snapshot, catalog refresh, slot creation, or active-slot selection operations.

M4-01 and M4-02 built the underlying catalog, selection, and technical slot-creation machinery. The missing piece is public composition.

**Audit classification:** BLOCKER
**Affected capabilities:** CAP-002, CAP-017
**Recommended resolution:** add a bounded public catalog/create/select surface over the already-proven internal coordinators.

---

## A-03 — Full slot-policy configuration is not implemented

The capability matrix promises:

- single-slot policy,
- fixed multi-slot policy,
- configurable multi-slot policy,
- bounded/unlimited-profile policy with safety bounds.

Current `EchoSaveConfiguration` schema version 1 contains only the storage-root directory name.

M4-02 currently uses bounded technical capacity internally, and multiple closeout documents explicitly say full slot-policy/configuration expansion remains deferred.

This means CAP-002 is not complete as currently worded.

**Audit classification:** BLOCKER / AUTHORITY DECISION
**Affected capability:** CAP-002
**Recommended resolution:** keep the broad slot-policy promise and implement the runtime configuration model before M4 closes. M5 Setup should author/edit that policy, not invent the runtime policy for the first time.

---

## A-04 — CAP-014 combines implemented participant migration with unimplemented document migration

The capability matrix defines CAP-014 as:

> Document and participant migration.

M3 implemented participant migration chains in depth.

Current documentation repeatedly and explicitly leaves **document migration** deferred.

Therefore CAP-014 cannot truthfully be considered fully implemented under its current wording.

**Audit classification:** BLOCKER / AUTHORITY DECISION
**Affected capability:** CAP-014
**Recommended resolution:** either:
1. implement bounded package-document migration before declaring the relevant MVP capability complete, or
2. formally split CAP-014 into separate participant-migration and package-document-migration capabilities with separate milestone ownership.

Because this changes durable capability authority, it should be decided explicitly rather than silently edited.

---

# 2. Capability reconciliation: CAP-002 through CAP-018

| Capability | Audit state | Finding |
|---|---|---|
| CAP-002 Slot policies | **Partial / blocker** | Bounded technical capacity exists; full single/fixed/configurable/unlimited policy model is absent from configuration/public surface. |
| CAP-003 Stable slot IDs | **Implemented** | Package-generated technical identity is independent from display metadata. |
| CAP-004 Immutable generations | **Implemented** | Generation-first/head-last model is established and reused across save/rename/duplicate. |
| CAP-005 Head pointer | **Implemented** | Current-generation selection and recovery repointing are implemented. |
| CAP-006 Independent manifests | **Implemented** | Catalog reconstruction reads head/current manifest without normal payload opening. |
| CAP-007 Participant registry | **Implemented** | Open-ended deterministic registration and ownership rules are present. |
| CAP-008 Unknown payload preservation | **Implemented core** | Opaque preservation/carry-forward is implemented; explicit prune remains deferred. |
| CAP-009 Default JSON serializer | **Implemented** | Package-owned Unity JSON serializer exists. |
| CAP-010 Serializer providers | **Implemented core / configuration surface incomplete** | Provider IDs/registries exist; broader configuration/Setup authoring is still deferred. |
| CAP-011 Checksums/size bounds | **Needs final evidence mapping** | Integrity/checksum and several bounded stores are implemented, but the broad registry claim needs explicit test/evidence reconciliation. |
| CAP-012 Two-phase loading | **Partial / blocker** | Internal prepare/apply machinery exists; promised public service facade is absent. |
| CAP-013 Convenience load | **Missing / blocker** | Spec promises `LoadAndApplyAsync`; current service interface has no such operation. |
| CAP-014 Migration chains | **Partial / blocker** | Participant migration exists; package-document migration is explicitly deferred. |
| CAP-015 Backup retention | **Implemented** | Bounded committed-generation retention exists with protected recovery history. |
| CAP-016 Autosave admission | **Implemented** | Explicit caller-triggered autosave plus one-pending latest-wins coalescing and retention reuse exist. |
| CAP-017 Slot operations | **Partial / blocker** | Rename/duplicate/prepare-delete/confirm-delete are public; create/catalog/select remain internal or absent from the service facade. |
| CAP-018 Recovery planning | **Implemented** | Public plan building and explicit execution are present with stale-plan/source revalidation. |

---

# 3. Test registry drift

The 100-case specification registry is substantially behind the actual 587-test implementation.

Examples of direct contradictions:

- ESV-T-019 Rename Slot is still marked `Planned`, while the M4-09 closeout explicitly records it complete.
- ESV-T-020 Duplicate Slot is still marked `Planned`, while the M4-09 closeout explicitly records it complete.
- ESV-T-024 Catalog/List Slots remains `Planned`, despite M4-01 payload-free catalog proof.
- ESV-T-043 Rapid Autosaves remains `Planned`, despite M4-05 latest-wins/coalescing proof.
- ESV-T-044 Autosave Retention remains `Planned`, despite M4-06 retention integration.
- ESV-T-074 through ESV-T-079 remain `Planned`, although M4-07/M4-08 implemented and tested recovery planning/execution/stale-plan behavior.
- Many M2/M3 registry rows likewise remain `Planned` even though the underlying checkpoints and focused suites are complete.

Only ESV-T-021 through ESV-T-023 were updated during M4-10 closeout.

**Audit classification:** DOCUMENTATION BLOCKER

### Required repair

Do not mass-change every `Planned` row to `Complete`.

Each applicable registry row should be assigned one of:

- `Complete`
- `Partially covered`
- `Deferred`
- `Not yet run`
- `Superseded/reframed`

and should point to the checkpoint/test class or retained evidence that justifies the state.

The specification's own consistency addendum says implementation registries should track individual execution status and evidence, so this repair is required before the paperwork can be called reconciled.

---

# 4. Current Notes / health-document drift

## Root Current Notes

The top of the document correctly says M4 reconciliation is now the next gate, but a historical `Chronicle ESV-M4-10 Activation` block still says the checkpoint is active and references v1.33.0.

This is understandable history, but its present-tense wording can be mistaken for current authority.

**Repair:** label it explicitly as historical activation record or collapse it under the completed M4-10 history.

## Package Developer Current Notes

The early `M4 milestone state` summary still describes only the M4-01/M4-02-era capability set even though later sections contain M4-03 through M4-10 closeouts.

**Repair:** replace the stale milestone summary with one reconciled M4 capability ledger.

## Suite Health / README / package index

These are largely aligned after the M4-10 closeout:

- M4-10 is complete,
- `587 / 587` is recorded,
- M4 is not yet declared complete,
- M5 is not active.

They should be updated again only after the reconciliation outcome is known.

---

# 5. Public API versus specification

The current package specification describes a much broader primary public service than the committed interface currently exposes.

### Spec-promised public flow

- Register participant
- Get/refresh catalog
- Create slot
- Select slot
- Rename
- Duplicate
- Prepare/confirm delete
- Save
- Autosave
- Prepare load
- Apply prepared load
- One-step load/apply
- Recovery plan/execute
- Shutdown

### Current `IEchoSaveService`

- Initialize
- Save
- Request autosave
- Build recovery plan
- Execute recovery
- Rename
- Duplicate
- Prepare delete
- Confirm delete
- Shutdown

This is the central reconciliation finding.

M5 tooling should not be asked to paper over those missing runtime entry points. The Save Browser/Laboratory should consume a coherent package API rather than reaching into internals.

---

# 6. Recommended recovery sequence

## Reconciliation checkpoint R1 — Public runtime composition

Bounded runtime work only:

- expose catalog snapshot/refresh;
- expose technical slot creation;
- expose active-slot selection;
- expose participant registration through the intended public authority;
- expose two-phase prepare/apply loading;
- expose one-step same-scene convenience load;
- compose existing M3/M4 internal coordinators without changing their proven semantics;
- add focused public-surface and end-to-end service tests.

This is mostly composition over existing machinery, not a rewrite.

## Reconciliation checkpoint R2 — Slot policy/configuration authority

- decide final runtime slot-policy model;
- advance `EchoSaveConfiguration` schema deliberately;
- support the approved single/fixed/configurable/bounded-unlimited semantics;
- move M4-02's hard bounded-capacity assumption behind configuration/policy;
- add policy validation and focused tests;
- leave the M5 Setup window as the Editor authoring surface for this runtime model.

## Reconciliation checkpoint R3 — Migration authority decision

Choose one:

### Path A — Implement document migration now
Complete CAP-014 exactly as currently written.

### Path B — Split the capability
Record participant migration as implemented and explicitly move package-document migration to a later bounded checkpoint/release capability.

This is the only item in the audit that should be treated as an authority-level decision instead of an automatic cleanup.

## Reconciliation closeout — Registry + paperwork

After the runtime/authority gaps above are resolved:

- reconcile ESV-T-001 through ESV-T-100 against retained evidence;
- produce an M4 capability matrix with implementation/evidence status;
- rewrite stale milestone summaries;
- reconcile README, CHANGELOG, package index, root/package Current Notes, Suite Health, and handoff;
- rerun the focused Chronicle suite;
- close M4 only from the actual final total.

---

# 7. What is *not* wrong

This audit did **not** find a reason to roll back M4-10.

The following M4 work remains sound and should be preserved:

- immutable generation/head-last durable truth;
- catalog reconstruction;
- active-slot session semantics;
- manual save transaction;
- root-local mutation admission;
- autosave latest-wins coalescing;
- generation retention;
- recovery planning;
- explicit recovery execution;
- rename/duplicate;
- two-step recoverable deletion/trash;
- participant/scene/DDOL/storage-boundary rules;
- focused `587 / 587` evidence.

The problem is milestone composition and documentation authority, not that those implementations should be discarded.

---

# Audit disposition

**ESV-M4-10:** COMPLETE
**Chronicle focused gate:** 587 / 587
**M4 milestone:** NOT READY TO CLOSE
**M5:** LOCKED
**Next action:** Resolve the four high-priority reconciliation blockers, then perform registry/document closeout.

The audit deliberately makes no repository changes.


# Approved reconciliation disposition

**Approved:** 2026-08-11

Jesse approved the recommended repair direction.

The authority path is now:

1. **ESV-M4-R1 — Public Runtime Composition and Consumer Facade Reconciliation**
   - participant registration facade;
   - catalog snapshot/refresh facade;
   - public create/select;
   - two-phase prepared load/apply;
   - same-scene convenience load.

2. **M4 Reconciliation R2 — Slot Policy Runtime Configuration**
   - preserve CAP-002 as written;
   - advance `EchoSaveConfiguration` deliberately;
   - replace the hard technical capacity default with project-owned policy truth.

3. **M4 Reconciliation R3 — Package-Document Migration**
   - preserve CAP-014 intact;
   - implement document migration rather than weakening the capability to participant migration only.

4. **Final M4 Reconciliation Closeout**
   - reconcile the 100-case test registry against retained evidence;
   - reconcile all Current Notes, README, CHANGELOG, package index, Suite Health, checkpoint reports, and handoff truth;
   - rerun the focused Chronicle suite;
   - declare M4 complete only from actual final evidence.

**M5 remains locked through all four steps.**


# R1 completion update

**R1 status:** COMPLETE
**Planning/activation commit:** `bdb0c00`
**Implementation commit:** `ab18361`
**Focused Chronicle Editor evidence:** **618 / 618 passed, 0 failed**
**Prior floor:** **587 / 587**
**Net new focused tests:** **31**

R1 resolves:
- **A-01 — Public load facade incomplete**
- **A-02 — Public catalog/create/select facade incomplete**

Resolved public composition:
- participant registration;
- catalog snapshot/refresh;
- slot create/select;
- prepared-load creation;
- prepared-load apply;
- same-scene convenience load.

Still open:
- **A-03 — CAP-002 slot-policy runtime configuration** → R2;
- **A-04 — CAP-014 package-document migration** → R3;
- stale 100-case registry/document evidence mapping → final reconciliation.

**M4 remains open. M5 remains locked.**


# R2 completion update

**R2 status:** COMPLETE
**Planning baseline:** `176b240`
**Planning/activation commit:** `428369e`
**Implementation commit:** `8a8e7e7`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.38.0 / ESV-D-034
**Focused Chronicle Editor evidence:** **636 / 636 passed, 0 failed**
**Prior floor:** **618 / 618**
**Net new focused R2 tests:** **18**

R2 resolves:
- **A-03 — CAP-002 slot-policy runtime configuration**.

Resolved CAP-002 truth:
- `EchoSaveConfiguration` schema 2 owns project-authored slot policy;
- all four approved policy modes resolve to one finite immutable service-session capacity;
- schema-1 configurations retain non-mutating historical capacity-64 compatibility;
- create and duplicate consume the same resolved effective capacity;
- canonical live-slot counting remains catalog-owned, including degraded slots and excluding trash;
- ESV-T-015 through ESV-T-018 are complete;
- the final Chronicle Editor gate is **636 / 636**.

Implementation-history note: one pre-commit compile compatibility correction restored the existing internal `DefaultTechnicalSlotCapacity` symbol as an alias to the legacy schema-1 value. It does not restore hardcoded schema-2 authority; runtime create and duplicate continue to consume `SaveSlotPolicy.EffectiveCapacity`.

Still open:
- **A-04 — CAP-014 package-document migration** → R3;
- final 100-case registry/document evidence mapping → final reconciliation.

**M4 remains open. R3 is next but not activated. M5 remains locked.**
