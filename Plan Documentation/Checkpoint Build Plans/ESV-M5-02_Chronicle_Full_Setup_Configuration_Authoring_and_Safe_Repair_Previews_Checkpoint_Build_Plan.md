# ESV-M5-02 — Chronicle Full Setup/Configuration Authoring and Safe Repair Previews — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Milestone:** M5 — Tooling and Laboratory
**Checkpoint:** ESV-M5-02
**Status:** ACTIVE / AUTHORIZED
**Planning baseline:** `8774dd2` — `Close out ESV-M5-01 editor tooling foundation`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.46.0 / ESV-D-038
**Incoming focused Chronicle floor:** **697 / 697 passed, 0 failed**
**Unity baseline:** 6000.3.8f1

## 1. Purpose

M5-01 proved the Editor assembly, Preview-before-mutation discipline, create-only schema-2 configuration authoring, and initial read-only Validator.

M5-02 is the next bounded risk: editing or upgrading **existing project-owned Chronicle configuration and selected setup references** without turning Setup into a silent migration/repair engine.

The checkpoint therefore completes the Setup/configuration-authoring layer under one rule:

> every project mutation is explicit, previewed, target-bound, revalidated immediately before Apply, and blocked when authority or data safety is ambiguous.

## 2. Authority decision — ESV-D-038

`EchoSaveConfiguration` may advance to serialized schema 3 so Chronicle can represent the approved project configuration domains that M5 Setup must author.

Schema 3 is not permission to add decorative settings. A value is authorable only when Runtime or Editor tooling can consume and validate it as real truth.

Required compatibility:
- schema 1 remains readable through the historical 64-slot mapping plus deterministic defaults for later fields;
- schema 2 remains readable with exact R2 slot-policy semantics plus deterministic defaults for later fields;
- schema 3 becomes the current authoring target;
- future schemas fail closed;
- runtime never rewrites schema 1/2 assets;
- only an explicit Editor upgrade action may write schema 3.

## 3. In scope

### 3.1 Configuration authoring

M5-02 may add/author the approved configuration domains:

1. storage root;
2. slot policy;
3. bounded retention policy;
4. serializer provider selection;
5. storage backend provider selection;
6. bounded limit policy;
7. recovery policy/preferences;
8. optional fixed-slot template metadata.

A domain is only exposed if its consuming implementation is present in the same checkpoint.

### 3.2 Compatibility defaults

Older configurations resolve deterministic defaults equivalent to current package behavior.

At minimum:
- M4 slot policy behavior remains unchanged;
- retention defaults remain equivalent to the current runtime default;
- default serializer remains the package Unity JSON serializer;
- default storage remains package local storage;
- recovery remains no more aggressive than current explicit/manual behavior unless schema-3 policy explicitly authorizes an existing verified recovery path;
- existing security/bounded limits remain no weaker than current runtime constants;
- fixed-slot templates remain optional metadata and never become runtime slot-path authority.

### 3.3 Setup editing

Setup may:
- select an existing Chronicle configuration asset;
- Preview an edit/upgrade;
- display exact schema before/after;
- display exact changed fields;
- display dependent asset/reference actions;
- explicitly Apply after revalidation;
- use Undo/dirty/save handling appropriate to Unity project assets.

Setup must refuse stale previews.

### 3.4 Safe repair previews

Allowed repair targets are project-owned configuration/root/reference state only.

Examples:
- assign a selected configuration to a selected Chronicle root;
- repair a missing selected configuration reference;
- create an optional root prefab and assign the selected configuration;
- create fixed-slot authoring templates with stable IDs;
- repair missing default provider descriptors when the package default is unambiguous;
- normalize invalid retention/limit values only through an explicit user-approved preview.

Forbidden automatic repairs:
- choosing one duplicate root as authority;
- deleting/disabling another root;
- moving/renaming production save directories;
- rewriting slot IDs once durable evidence may exist;
- changing production head/generation/trash/quarantine data;
- executing recovery;
- erasing or restoring save data.

### 3.5 Validator expansion

Add deterministic read-only checks made truthful by M5-02, including as applicable:
- invalid configuration schema/current upgrade need;
- invalid retention bounds (`ESV-VAL-005`);
- missing selected/default provider (`ESV-VAL-006`);
- duplicate fixed slot IDs (`ESV-VAL-004`);
- production root versus future Laboratory sandbox collision (`ESV-VAL-010`) only if a sandbox path becomes representable;
- invalid configured limits;
- unsafe/ambiguous repair targets.

Validation remains zero-mutation.

## 4. Explicitly out of scope

M5-02 does not implement:
- Save Browser;
- Generation Inspector;
- Migration Graph UI;
- Failure Simulator;
- Recovery Planner UI;
- Test Data Generator;
- Redacted Snapshot Exporter;
- persistent catalog cache;
- quarantine/incomplete cleanup;
- permanent erase;
- public restore-from-trash;
- Laboratory sandbox/direct-scene initializer/sample UI;
- LAB-001 through LAB-032 execution;
- peer integration, scene travel, service location, or package-owned project-wide DDOL;
- package document version changes;
- participant contract changes.

## 5. Public/runtime compatibility rules

M5-02 may change Runtime configuration/policy types only where needed to consume schema-3 authored truth.

It must preserve:
- `IEchoSaveService` operation contract unless a separately recorded authority amendment is required;
- immutable committed generations;
- head-last publication;
- explicit recovery truth;
- participant ownership/migration contracts;
- package document versions;
- M4 slot identity and capacity behavior.

No existing schema-1 or schema-2 asset may become unreadable merely because M5-02 exists.

## 6. Schema-3 session snapshot

Runtime must resolve configuration once per successful initialization into immutable session truth.

The session snapshot may include:
- effective slot policy/capacity;
- retention policy;
- selected serializer/storage provider IDs and resolved providers;
- bounded limits;
- recovery policy;
- authoring-only fixed slot metadata separately from runtime slot authority.

Later mutation of the ScriptableObject must not silently mutate a live service session.

## 7. Provider rules

- package defaults remain available;
- a provider may be selected only by stable ID;
- missing configured provider blocks initialization with structured truth;
- Setup only lists/selects providers the package can actually resolve;
- custom-provider extension remains package-neutral and must not introduce peer-package dependencies;
- provider selection may not permit arbitrary reflection/type activation from save data.

## 8. Recovery-policy rules

M5-02 may expose recovery policy only over existing verified M4 recovery machinery.

Any automatic/configured fallback behavior must:
- run only when the current canonical generation is not safely loadable;
- use a fresh read-only recovery plan;
- select only a fully verified candidate;
- revalidate before head mutation;
- preserve immutable generation bytes;
- never guess through unsupported-newer/missing-migration data;
- report recovery truth separately from later participant Apply.

If that complete behavior cannot be implemented safely in M5-02, Setup must expose only the currently truthful manual policy and defer automatic fallback instead of creating a dead setting.

## 9. Fixed-slot template rules

`SaveSlotTemplate` is authoring metadata:
- stable template ID;
- display label/order;
- optional project-facing default slot ID when safe;
- no display text in physical paths;
- no automatic runtime slot provisioning required;
- duplicate IDs block validation;
- auto-regeneration is allowed only before durable save evidence can exist and only after explicit preview.

## 10. Repair transaction rules

Every Apply:
1. binds one preview identity;
2. records target GUID/object identity and expected current values;
3. revalidates target state immediately before mutation;
4. aborts stale/changed targets;
5. applies only the previewed changes;
6. registers Undo for supported project/scene objects;
7. marks exact assets/scenes dirty;
8. saves only the intended assets when required;
9. returns a structured report of changed and unchanged targets.

No hidden “repair all” operation exists in M5-02.

## 11. Test plan

Focused tests should cover at least:

### Configuration/schema
- current schema-3 defaults;
- schema-1 compatibility;
- schema-2 compatibility;
- future schema rejection;
- no runtime rewrite of old assets;
- live-session snapshot frozen after initialization;
- valid/invalid retention;
- provider selection and missing-provider block;
- valid/invalid limit policy;
- recovery-policy truth;
- fixed-template duplicate detection.

### Setup preview/apply
- existing asset edit Preview performs zero mutation;
- explicit upgrade Preview shows exact before/after;
- Apply writes only the selected asset/reference;
- stale Preview rejects;
- repeated Preview is deterministic;
- repeated Apply becomes no-change or explicit occupied/current result;
- optional leading `Assets/` input does not produce ambiguous `Assets/Assets/...` output;
- selected-root reference repair is previewed and Undo-safe;
- duplicate root is guidance/block, never auto-chosen;
- no production save-root mutation.

### Validator
- ESV-VAL-004;
- ESV-VAL-005;
- ESV-VAL-006;
- existing M5-01 rules remain green;
- deterministic issue ordering;
- validation performs zero writes.

### Regression
- full `EchoDevGames.EchoSave.Tests.Editor` assembly must remain green at actual discovered total;
- count may increase but must not fall below **697 / 697**.

## 12. Manual Unity proof

After automated green:

1. Create a disposable schema-2 configuration with M5-01 Setup.
2. Select it in M5-02 Setup.
3. Preview schema-3 upgrade/edit and capture screenshot.
4. Confirm no asset mutation before Apply.
5. Apply and inspect exact resulting configuration values.
6. Preview/apply one safe selected root/reference repair and confirm Undo can restore it.
7. Run Validator and capture deterministic result.
8. Delete disposable test assets/scene objects.
9. Confirm `git status --short` and `git diff --check` are clean except intended implementation changes.

## 13. Closeout requirements

M5-02 closes only when:
- activation authority is committed;
- implementation is committed;
- Unity compiles cleanly;
- focused Chronicle Editor tests are green at actual discovered total and not below 697;
- manual schema-upgrade/edit and safe-repair proof is recorded;
- README/CHANGELOG/Documentation Index and both Current Notes are reconciled;
- Suite Health/specification/checkpoint reflect actual implementation;
- no Browser/Simulator/Laboratory capability is smuggled in.

M5-02 completion does not complete M5.

## 14. Next-gate rule

M5-03 — Browser, Generation Inspector, and Migration Graph — requires a separate activation after M5-02 closeout.
