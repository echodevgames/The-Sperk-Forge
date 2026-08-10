---
tags:
  - sfgss/checkpoint
  - sfgss/package/chronicle
  - sfgss/implementation
status: complete
updated: 2026-08-10
---

# ESV-M4-02 — Chronicle Technical Slot Creation, Capacity Enforcement, Initial Empty Generation, and Catalog Reconciliation Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-02
**Milestone:** M4 — Slots / Autosave / Recovery
**Status:** **COMPLETE**
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.17.0
**Prior checkpoint:** ESV-M4-01 — **Complete**
**Unity baseline:** 6000.3.8f1
**Regression baseline:** focused Chronicle Editor **403 / 403**
**Exact implementation baseline:** `62e8a54`
**Implementation commit:** `d8d5c18`
**Final focused gate:** **425 / 425 passed, 0 failed**

## 1. Intent

Establish the first physical slot-creation path on top of M4-01's proven provider-neutral catalog without prematurely absorbing rename, duplicate, delete, persistent catalog cache, production operation admission, autosave, retention, or recovery.

A Chronicle slot is not considered successfully created merely because a directory exists. M4-02 creates one new technical `SaveSlotId`, publishes one verified empty immutable generation using the existing generation-first/head-last transaction, and then reconciles the M4-01 catalog.

```text
trustworthy current catalog
        ↓
capacity check
        ↓
fresh canonical SaveSlotId
        ↓
bounded collision check
        ↓
initial empty immutable generation
        ↓
candidate verification
        ↓
generation publication
        ↓
published-generation revalidation
        ↓
head.json LAST
        ↓
catalog refresh
        ↓
created slot metadata
```

## 2. Carried-forward authority

Chronicle already establishes:
- `SaveSlotId` is package-generated technical identity;
- display names are metadata and never physical directory names;
- immutable generations and `head.json` last are the durability model;
- M4-01 catalog entries include degraded canonical slots rather than hiding them;
- active selection is session-only and never auto-selects;
- capacity must remain bounded even for an "unlimited profiles" future policy;
- one mutating operation globally is the eventual production policy, but the production operation-admission coordinator is not yet authorized.

### ESV-D-024 — a created slot is a committed generation, not a directory

M4-02 records:

> Successful slot creation requires one verified committed immutable generation and a successfully published `head.json`. Directory creation alone is never successful slot creation. Every discovered canonical technical slot, healthy or degraded, counts against the M4-02 capacity bound. A newly created slot is not auto-selected.

Consequences:
- incomplete/orphan directory material never becomes a fake healthy slot;
- a degraded technical slot cannot be ignored to bypass capacity;
- display metadata remains decoupled from technical path identity;
- publication failure never fabricates catalog success;
- catalog-refresh failure after a successful head publication is reported as partial truth rather than rolled back by fiction.

## 3. Authorized implementation scope

### Bounded creation request

Add a narrow technical creation request/result contract sufficient for this checkpoint.

The request may carry:
- display name;
- project ID;
- project version;
- build ID.

Rules:
- fields are bounded and validated before mutation;
- display name is never used as a storage key;
- caller does not supply an arbitrary physical path;
- the normal public creation path receives a package-generated `SaveSlotId`.

A package-internal deterministic ID factory may be injected for tests.

### Capacity enforcement

M4-02 uses one positive technical capacity bound.

Rules:
- creation requires one trustworthy current catalog snapshot;
- every discovered canonical technical slot counts, including degraded entries;
- invalid non-slot child names do not count;
- when current count is at capacity, creation fails before publication mutation;
- capacity is a primitive for later single/fixed/configurable/profile policy expansion, not the final configuration surface.

### Fresh technical identity

- generate canonical `SaveSlotId`;
- reject any generated ID already present in the trustworthy catalog;
- retry generation only up to one positive bounded attempt count;
- exhausting the attempt bound fails before durable mutation;
- no display string influences technical identity.

### Initial immutable generation

Reuse the existing M2/M3 generation publication machinery.

M4-02 may add one narrow initial-publication entry point whose invariant is:

> No existing current head may be present for this slot.

The transaction must preserve:
- empty package payload document;
- empty participant inventory;
- manifest metadata from the create request;
- candidate write/read-back verification;
- immutable generation publication;
- final published-generation revalidation;
- `head.json` publication last.

If an existing head is discovered inside the publication transaction, creation fails closed rather than silently turning "create" into "save existing slot."

### Catalog reconciliation

After a successful initial head publication:
- refresh the M4-01 catalog;
- return the created technical slot ID and generation ID;
- return healthy created metadata when reconciliation succeeds;
- do not auto-select the slot;
- if catalog refresh fails after successful publication, return a structured terminal result that truthfully records **slot published / catalog reconciliation failed**;
- do not delete or rewrite the committed slot merely to pretend the whole operation rolled back.

## 4. Explicit non-scope

Do not add:
- persistent `catalog.cache.json`;
- rename;
- duplicate;
- delete or deletion plans;
- trash/quarantine policy;
- full `EchoSaveConfiguration` slot-mode expansion;
- fixed-slot template assets;
- public production `CreateSlotAsync` admission/queue ownership;
- concurrent public mutation scheduling;
- participant capture/apply/default callbacks;
- `SaveAsync`;
- autosave;
- retention cleanup;
- recovery;
- document migration;
- scene travel;
- peer bridges;
- Chronicle-owned/project-wide DDOL/service locator.

## 5. Failure and safety invariants

Tests must prove:
- invalid request fails before storage mutation;
- zero/negative capacity is invalid;
- capacity-full fails before publication mutation;
- degraded canonical slots count toward capacity;
- invalid child directories do not count toward capacity;
- generated ID collision retries deterministically;
- collision retry bound is enforced;
- display name never becomes a storage key;
- create path invokes zero participant callbacks;
- initial generation contains zero participant entries;
- initial generation uses immutable generation-first/head-last publication;
- an existing current head causes create failure rather than overwrite/update;
- pre-head publication failure does not publish a new current head;
- head publication success creates one catalog-discoverable slot;
- successful creation does not auto-select;
- post-publication catalog refresh success exposes the new healthy entry;
- post-publication catalog refresh failure reports publication truth and preserves committed durable state;
- M4-01 payload-free catalog behavior remains green;
- no persistent cache, rename, duplicate, delete, autosave, retention, recovery, scene, or DDOL scope enters;
- all prior **403 / 403** Chronicle tests remain green.

## 6. Proposed focused proof

- request bounds;
- capacity primitive validation;
- healthy+degraded slot capacity accounting;
- invalid child exclusion;
- generated ID success;
- generated ID collision retry;
- retry exhaustion;
- display-name/path separation;
- initial empty payload/inventory;
- existing-head rejection;
- candidate-write failure;
- candidate verification failure;
- generation publication failure;
- final verification failure;
- head publication failure;
- successful head-last initial creation;
- catalog reconciliation success;
- catalog reconciliation failure after durable success;
- no auto-select;
- zero participant callbacks;
- prior **403 / 403** regression floor.

Executed totals are recorded from Unity, never predicted.

## 7. Stop point

Stop when Chronicle can create one bounded technical slot as one real committed empty generation, enforce capacity without ignoring degraded technical slots, refuse collisions, reconcile the catalog after publication, and report partial publication/catalog truth accurately.

Do not rename, duplicate, or delete slots yet.

Do not add full slot-mode configuration assets yet.

Do not add production operation admission or concurrent mutation ownership yet.

Do not add autosave, retention, recovery, or persistent catalog cache yet.

## 8. Completion evidence

ESV-M4-02 closed on 2026-08-10 with:
- implementation commit `d8d5c18`;
- Unity 6000.3.8f1 compile/import green;
- focused `EchoDevGames.EchoSave.Tests.Editor` **425 / 425 passed, 0 failed**;
- prior **403 / 403** regression floor preserved;
- 22 net new focused tests;
- exact repository implementation scope of 17 files, 1831 insertions, and 1 deletion;
- package-generated technical identity, bounded capacity, collision retry, initial empty immutable generation, existing-head rejection, head-last publication, and catalog reconciliation all proven;
- post-publication catalog-refresh failure preserving truthful durable state;
- no automatic active-slot selection;
- no participant callbacks or deferred M4 scope introduced.

The final gate supersedes intermediate test-only accessibility/discovery and expected-value repairs.

No follow-on M4 checkpoint is activated by this completion record. A new bounded Checkpoint Build Plan is required before further Chronicle implementation.
