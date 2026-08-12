# ESV-M5-05 — Chronicle Explicit Unknown-Payload Prune and Derived Catalog Cache/Rebuild Prerequisites — Checkpoint Build Plan

**Package:** The Chronicle (`EchoSave`)
**Package ID:** `com.echodevgames.echo-save`
**Milestone:** M5 — Tooling and Laboratory
**Checkpoint:** ESV-M5-05
**Status:** ACTIVE / AUTHORIZED
**Planning baseline:** `1111b46` — `Close out ESV-M5-04 QA recovery preview and support tooling`
**Authority:** SFGSS-PKG-ECHOSAVE-001 v1.52.0 / ESV-D-041
**Incoming focused Chronicle floor:** **746 / 746 passed, 0 failed**
**Unity baseline:** 6000.3.8f1

## 1. Why M5-05 exists before the Laboratory

The Chronicle Laboratory acceptance matrix already requires:

- **LAB-016:** explicitly prune one unknown entry;
- **LAB-029:** rebuild the catalog cache and match durable manifest truth.

Neither capability is complete at M5-04 closeout.

Therefore M5-05 closes these two already-approved MVP prerequisites before direct-scene Laboratory content begins.

The corrected sequence is:

- **M5-05:** explicit unknown prune + derived catalog cache/rebuild;
- **M5-06:** full Chronicle Save Laboratory and LAB-001 through LAB-032.

## 2. Unknown-payload prune authority

### 2.1 Default rule

Unknown payloads continue to round-trip unchanged unless an explicit prune plan names them.

### 2.2 Preview / plan

A prune Preview is read-only and produces an immutable plan containing:

- target slot ID;
- source current generation ID;
- source provenance/fingerprint sufficient for stale-plan rejection;
- exact requested unknown participant IDs;
- package/session binding;
- created/expiry time;
- one-use state;
- deterministic diagnostic/result preview.

No wildcard or implicit "prune all unknown" operation is authorized.

### 2.3 Confirmation

Confirm must:

1. acquire existing root-local mutation admission;
2. reject Busy rather than create a generic queue;
3. validate package/session/expiry/one-use;
4. re-read/revalidate exact source generation/provenance;
5. re-evaluate unknown-vs-known ownership;
6. reject if any requested ID is no longer unknown;
7. preserve every unnamed unknown entry byte-for-byte;
8. capture/merge known participant truth through existing save authority as required;
9. publish a new immutable generation;
10. update head only after successful generation verification;
11. reconcile catalog/live unknown snapshot only after durable truth permits;
12. never edit the source generation.

### 2.4 Result truth

Results distinguish at least:

- invalid request;
- no matching unknown IDs / no-op;
- stale plan;
- expired/used plan;
- Busy;
- requested ID now claimed;
- participant/capture/merge failure if the existing save transaction requires it;
- generation publication failure;
- head publication failure;
- catalog/cache maintenance degradation;
- success.

## 3. Derived catalog cache authority

### 3.1 Cache role

`catalog.cache.json` is a package-owned derived acceleration file.

It is never the source of truth for whether a slot/generation exists or is healthy.

### 3.2 Cache schema

The first cache schema must be explicitly versioned and bounded.

Representative data may include:

- cache schema/package version;
- generated/rebuilt UTC;
- configuration/root identity token if needed for validation;
- bounded slot entries copied from authoritative catalog metadata;
- per-entry head/current-generation provenance sufficient for freshness checks;
- cache-level fingerprint/provenance if useful.

It must not contain participant payload contents.

### 3.3 Load behavior

- missing cache -> bounded durable rebuild;
- valid/fresh cache -> may seed/accelerate catalog;
- corrupt cache -> ignore and rebuild;
- unsupported cache schema -> ignore and rebuild;
- stale cache -> invalidate and rebuild;
- partial/temp cache -> ignore;
- cache must never mask durable corruption/degradation.

### 3.4 Write behavior

- live durable-derived catalog truth exists first;
- cache write is post-truth maintenance;
- use safe temp/replace semantics where supported;
- write failure reports maintenance degradation;
- write failure does not invalidate a truthful in-memory catalog;
- no generation/head/payload mutation is permitted during cache rebuild.

## 4. Editor rebuild tool

Add a bounded Editor surface under The Chronicle tooling.

Required behavior:

1. select/assign current Chronicle configuration;
2. Preview current cache state versus durable catalog;
3. report Missing / Valid / Stale / Corrupt / Incompatible;
4. Preview exact cache target;
5. explicit **Rebuild Catalog Cache** action;
6. after Apply, show resulting cache state/count/fingerprint;
7. no other production file may change;
8. no silent quarantine/cleanup.

## 5. Focused test matrix

### Unknown prune
- Preview zero writes.
- Exact ID list normalized deterministically.
- Duplicate requested IDs handled deterministically.
- Empty request rejected/no-op per approved result.
- Named unknown removed only in new generation.
- Unnamed unknown byte-identical.
- Known entry cannot be pruned by unknown-prune API.
- ID becoming known after Preview rejects Confirm.
- Source head/generation change rejects stale plan.
- Expired/used plan rejects.
- Historical source generation bytes unchanged.
- Head-last publication preserved.
- Catalog/live unknown snapshot updated only after durable publication.
- Busy/no-generic-queue behavior preserved.

### Cache
- Missing cache rebuild.
- Fresh valid cache accepted.
- Corrupt JSON ignored/rebuilt.
- Unsupported cache schema ignored/rebuilt.
- Stale head/current generation invalidates cache.
- Cache cannot mask a degraded durable slot.
- Bounded count/size.
- Cache write failure leaves live catalog truthful.
- Temp/partial file ignored.
- Explicit rebuild changes only cache.
- Rebuilt cache catalog equals bounded durable rebuild.

### Regression
- full `EchoDevGames.EchoSave.Tests.Editor` green at actual discovered total;
- total must not fall below **746 / 746**.

## 6. Manual Unity proof

### Unknown prune proof
1. Create one disposable sandbox/technical slot with at least one known and two unknown payload entries using existing test/evidence tooling.
2. Preview prune of exactly one unknown ID.
3. Capture source generation/provenance + requested ID list.
4. Confirm prune.
5. Prove:
   - new current generation exists;
   - named unknown is absent;
   - second unknown remains;
   - known payload remains;
   - old generation bytes remain unchanged.

### Catalog cache proof
1. Use a disposable Chronicle root with at least one slot.
2. Preview cache state with cache absent.
3. Explicitly Rebuild Catalog Cache.
4. Inspect `catalog.cache.json` existence/state/count.
5. Change durable slot/head state through normal Chronicle authority.
6. Preview stale cache detection.
7. Rebuild.
8. Prove rebuilt cache matches durable Browser/catalog truth.
9. No generation/head/payload document changed merely from cache rebuild.

## 7. Explicitly deferred

M5-05 does not implement or activate:

- production quarantine/incomplete cleanup;
- public restore-from-trash;
- permanent erase;
- automatic recovery fallback/recovery-on-load;
- generic operation queueing;
- automatic autosave timers/triggers;
- permission-provider production wiring;
- Save Laboratory scene/sample/direct-scene initializer;
- LAB-001 through LAB-032 full execution;
- M5-06.

## 8. Closeout

M5-05 closes only when:

- implementation committed;
- Unity compiles cleanly;
- focused Chronicle Editor suite is green >= 746;
- unknown-prune safety/durability tests green;
- cache invalidation/rebuild/non-authority tests green;
- manual unknown-prune proof complete;
- manual catalog-cache Preview/Rebuild/stale-rebuild proof complete;
- documentation reconciled;
- repository clean.

After M5-05 closeout, **M5-06 Save Laboratory** becomes eligible for separate activation.
