---
tags:
  - sfgss/checkpoint-build-plan
  - sfgss/package/chronicle
  - sfgss/reconciliation
status: complete
updated: 2026-08-11
---
# ESV-M4-R2 — Chronicle Slot Policy Runtime Configuration and CAP-002 Reconciliation — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-R2
**Milestone:** M4 — Slots / Autosave / Recovery Reconciliation
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.38.0
**Authority decision:** ESV-D-034
**Planning baseline:** `176b240`
**Unity baseline:** 6000.3.8f1
**Incoming focused Chronicle Editor floor:** **618 / 618**
**M5 state:** **LOCKED**

## 1. Purpose

R2 resolves the remaining CAP-002 runtime gap identified by the M4 audit.

Chronicle currently enforces a hidden technical capacity of `64`. R2 moves that truth into explicit project-owned configuration without changing slot durability, slot identity, catalog counting, or the public operation model.

## 2. Configuration schema decision

Advance `EchoSaveConfiguration` serialized schema from **1** to **2**.

Schema 2 owns one authoritative slot-policy configuration.

The runtime must never silently rewrite a schema-1 asset.

### Schema 1 compatibility

A schema-1 configuration remains readable through an explicit compatibility mapping:

```text
Mode                  ConfigurableMultiSlot
EffectiveCapacity     64
CompatibilityMapped   true
SerializedMutation    none
```

Compatibility use should be diagnosable so M5 Setup can later offer an explicit upgrade.

Unsupported future configuration schemas fail closed.

Invalid schema-2 policy blocks initialization before storage/catalog side effects.

## 3. Policy types

### `SaveSlotPolicyMode`

```text
SingleSlot
FixedMultiSlot
ConfigurableMultiSlot
BoundedProfiles
```

Undefined enum values are invalid.

### `SaveSlotPolicy`

The runtime policy is immutable after successful initialization and exposes:

```text
Mode
FixedSlotCount
ConfiguredSlotLimit
ProfileSafetyLimit
EffectiveCapacity
SourceConfigurationSchema
CompatibilityMapped
```

Only the active mode's capacity field affects `EffectiveCapacity`.

## 4. Exact policy semantics

### SingleSlot

- effective capacity = `1`;
- public create can establish one live slot;
- create/duplicate reject when one canonical live slot already counts.

### FixedMultiSlot

- effective capacity = `FixedSlotCount`;
- legal minimum = `2`;
- describes a project whose live save-slot count is intentionally fixed;
- R2 enforces count only;
- no fixed slot auto-provisioning;
- no template identity persistence;
- no special rename/delete behavior.

### ConfigurableMultiSlot

- effective capacity = `ConfiguredSlotLimit`;
- legal minimum = `1`;
- describes a project-authored maximum that may be changed by editing/upgrading configuration between builds;
- no runtime mutable capacity API.

### BoundedProfiles

- effective capacity = `ProfileSafetyLimit`;
- legal minimum = `1`;
- intended for profile-style UX where the game does not present a small design-fixed save-slot count;
- safety limit remains finite and mandatory;
- “unlimited” means no design-visible fixed count, not infinite resources.

## 5. Capacity authority

After R2 there must be exactly one service-session capacity authority.

The historical `DefaultTechnicalSlotCapacity = 64` must no longer drive schema-2 behavior.

Allowed legacy use:
- one named compatibility constant/value used only when reading schema 1.

Not allowed:
- separate create and duplicate capacities;
- fallback to 64 for invalid schema 2;
- hardcoded policy limits scattered across coordinators/tests;
- dynamic runtime mutation of effective capacity.

## 6. Capacity consumers

The resolved effective capacity must feed:

1. public `CreateSlotAsync`;
2. M4-02 technical slot creation;
3. public `DuplicateSlotAsync`;
4. any shared capacity helper used by those paths.

Capacity rejection remains structured and mutation-free.

Rename, save, autosave, load, recovery, delete planning, participant registration, and prepared-load handling do not allocate a new slot and therefore do not consume a capacity admission.

## 7. Canonical live-slot counting

Preserve existing M4 catalog truth:

- every canonical live catalog entry counts;
- degraded canonical live slots count;
- healthy slots count;
- trash records do not count;
- incomplete generations do not count as independent live slots;
- deleting a live slot frees capacity only after the existing durable removal/catalog reconciliation path says it is no longer live.

R2 must not create a second filesystem-based slot counter.

## 8. Configuration validation

A schema-2 policy is valid only if:

- mode is defined;
- active mode resolves to one positive finite capacity;
- `SingleSlot` resolves exactly to 1;
- `FixedMultiSlot` has `FixedSlotCount >= 2`;
- `ConfigurableMultiSlot` has `ConfiguredSlotLimit >= 1`;
- `BoundedProfiles` has `ProfileSafetyLimit >= 1`;
- conversion/arithmetic cannot overflow;
- inactive fields do not alter effective capacity.

Validation must happen before storage root creation/catalog scan/participant registration or other initialization side effects.

## 9. Runtime snapshot/result truth

R2 should expose one normalized runtime policy snapshot/result for diagnostics/tests, containing at minimum:

- active mode;
- effective capacity;
- source configuration schema;
- legacy-compatibility flag.

This does not need to become a broad mutable policy service.

M5 tooling may later inspect the configuration and this normalized truth.

## 10. Fixed-slot template boundary

`SaveSlotTemplate` remains approved architecture, but R2 does not implement it as a runtime prerequisite.

R2 specifically does not:
- generate fixed slot IDs;
- auto-create fixed slots;
- add template IDs to manifests;
- reserve catalog entries for templates;
- change delete/rename semantics because a policy is fixed;
- create Editor Setup tooling.

Those concerns belong to later authoring/tooling work if still required.

## 11. Public API compatibility

R1 public APIs remain source-compatible.

R2 may add read-only policy inspection/result types if needed.

R2 must not remove or rename:
- registration;
- catalog snapshot/refresh;
- create/select;
- save/autosave;
- prepared load/apply/convenience load;
- recovery;
- rename/duplicate;
- prepare/confirm delete.

## 12. Diagnostics

At minimum distinguish:

- legacy schema-1 compatibility in use;
- invalid schema-2 slot policy;
- unsupported configuration schema;
- capacity reached under the resolved policy.

Existing structured capacity-reached result/diagnostic truth remains authoritative; R2 does not renumber established runtime diagnostics.

Do not turn legacy schema 1 into a runtime failure solely because R2 adds schema 2.

## 13. Focused test registry reconciliation

R2 directly owns:

- **ESV-T-015 — Create single slot**
  - first create succeeds;
  - second create/duplicate rejects at capacity 1.

- **ESV-T-016 — Fixed slot capacity**
  - fixed-count capacity is enforced.

- **ESV-T-017 — Configurable capacity**
  - authored configurable limit is enforced.

- **ESV-T-018 — Unlimited policy safety cap**
  - bounded-profile policy enforces its finite safety limit.

Additional required tests:

- schema-1 configuration maps to effective capacity 64;
- schema-1 mapping does not mutate serialized configuration;
- schema-2 configuration ignores historical hardcoded 64;
- invalid schema-2 policy blocks before initialization side effects;
- unsupported future schema fails closed;
- create and duplicate share the same effective capacity;
- degraded canonical slots count;
- trash does not count;
- delete frees capacity through existing catalog truth;
- inactive policy fields do not affect capacity;
- runtime policy remains immutable for service session;
- R1 public facade regressions remain green;
- focused Chronicle floor stays at or above **618 / 618**.

## 14. Expected implementation areas

Likely package files:

- `Runtime/Configuration/EchoSaveConfiguration.cs`;
- new `Runtime/Configuration/SaveSlotPolicyMode.cs`;
- new `Runtime/Configuration/SaveSlotPolicy.cs`;
- optional normalized policy validation/snapshot/result types;
- `Runtime/Core/EchoSaveService.cs`;
- technical slot creation capacity wiring if still constructor/request based;
- duplicate slot capacity wiring;
- configuration tests;
- public create/duplicate policy integration tests.

No Editor assembly work belongs in R2.

## 15. Architecture invariants

Preserve:

- package-local duplicate-safe authority;
- base `ISaveStorageBackend`;
- base `ISaveParticipant`;
- immutable generations;
- head-last publication;
- canonical catalog-count capacity truth;
- no gameplay schemas;
- no direct scene authority;
- no generic service locator;
- no project-wide DDOL ownership;
- no peer-package runtime dependency;
- no automatic recovery fallback;
- no generic operation queue.

## 16. Explicitly deferred

- R3 package-document migration;
- fixed-slot auto-provisioning;
- persistent fixed-template identity;
- M5 Setup/Validator/Browser/Simulator/Laboratory;
- runtime policy mutation;
- persistent `catalog.cache.json`;
- automatic autosave timers;
- automatic/configured recovery fallback;
- permission-provider production wiring;
- quarantine/incomplete cleanup;
- public trash restore/permanent erase;
- scene travel/bridges/DDOL.

## 17. Documentation closeout

On R2 completion record:

- exact planning commit;
- exact implementation commit;
- actual Unity discovered total;
- ESV-T-015 through ESV-T-018 evidence state;
- configuration schema 2 behavior;
- schema-1 compatibility truth;
- implementation file scope;
- R3 as next gate, not automatically active;
- M4 still open;
- M5 still locked.

## 18. Completion rule

`ESV-M4-R2` is complete only when:

1. schema-2 configuration owns slot policy;
2. schema-1 compatibility preserves historical 64 behavior without mutation;
3. all four policy modes resolve correctly;
4. create and duplicate use one effective capacity;
5. no hidden schema-2 hardcoded 64 authority remains;
6. focused CAP-002 tests pass;
7. the entire Chronicle focused suite passes at or above **618 / 618**;
8. documentation records actual evidence;
9. R3 remains next;
10. M5 remains locked.

## 19. Completion evidence

**Status:** **COMPLETE**
**Planning baseline:** `176b240`
**Planning/activation commit:** `428369e`
**Implementation commit:** `8a8e7e7`
**Unity baseline:** 6000.3.8f1
**Final focused Chronicle Editor gate:** **636 / 636 passed, 0 failed**
**Incoming focused floor:** **618 / 618**
**Net new focused R2 tests:** **18**
**Implementation/test scope:** **8 files**, `768` insertions, `13` deletions

Completion truth:
- `EchoSaveConfiguration` schema 2 owns project-authored slot policy;
- all four approved modes resolve to one finite immutable session `EffectiveCapacity`;
- schema 1 maps read-only to `ConfigurableMultiSlot` capacity `64` without asset mutation;
- invalid schema 2 and unsupported future schemas fail before storage/backend side effects;
- create and duplicate receive the same resolved session capacity;
- degraded canonical live slots count, trash does not, and confirmed deletion frees capacity through catalog truth;
- ESV-T-015 through ESV-T-018 are complete;
- the compatibility-only `DefaultTechnicalSlotCapacity` symbol was restored before the implementation commit as an alias to the legacy schema-1 value and does not drive schema-2 runtime behavior.

R2 closes audit blocker **A-03 / CAP-002**. R3 package-document migration is next but is **not activated** by this closeout. M4 remains open and M5 remains locked.
