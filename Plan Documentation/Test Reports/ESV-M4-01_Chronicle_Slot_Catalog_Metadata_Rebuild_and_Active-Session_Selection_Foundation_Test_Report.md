# ESV-M4-01 Test Report — Slot Catalog, Metadata Rebuild, and Active-Session Selection Foundation

**Package:** The Chronicle (`EchoSave`)
**Checkpoint:** ESV-M4-01
**Unity:** 6000.3.8f1
**Implementation commit:** `62e8a54`
**Final result:** **PASS**
**Focused Chronicle Editor gate:** **403 / 403**
**Failures:** **0**

## Regression accounting

| Gate | Result |
|---|---:|
| Incoming Chronicle regression floor | 366 / 366 |
| Final Chronicle focused gate | 403 / 403 |
| Net new passing M4-01 tests | 37 |
| Final failures | 0 |

## Proven behaviors

The final gate proves:
- base `ISaveStorageBackend` remains unchanged;
- provider-neutral discovery is additive;
- local discovery remains beneath the configured root;
- missing `slots` root is a successful empty catalog;
- invalid technical children do not become catalog slots;
- provider enumeration order cannot change canonical catalog order;
- discovery bounds fail safely;
- healthy head/current-manifest pairs produce selectable metadata;
- normal catalog listing reads no participant payload file;
- display names do not become physical keys;
- missing/invalid/unsupported head or manifest produces degraded non-selectable state;
- identity disagreement degrades only the affected technical slot;
- one degraded slot does not erase unrelated healthy slots;
- overall untrustworthy refresh failure preserves the prior complete snapshot;
- public catalog snapshots are immutable/defensive;
- active slot begins unset and never auto-selects;
- known healthy slot selection works;
- unknown/unhealthy selection rejects without changing prior state;
- same-slot selection is no-change;
- explicit clear works;
- successful refresh removal/unhealthiness clears stale selection;
- active selection performs zero durable writes;
- catalog/session code invokes zero participant callbacks;
- catalog/session code owns no scene/DDOL authority.

## Repair evidence

### Distribution patch v1

The first archive stopped during `git apply --check` because the generated tracked-file hunk did not match the exact baseline. It reported failure before repository mutation. No staging, commit, or push occurred.

### Corrected v2

The replacement archive was generated against the exact baseline tracked files and applied all 43 named implementation files. Boundary checks and `git diff --check` passed.

### Compile hotfix

Unity then reported six test-only `CS1503` errors caused by the NUnit surface resolving `Does.Not.Contain(typeof(...))` through a string-oriented overload. Two test files were corrected to use `Array.IndexOf(..., typeof(...)) == -1`. Runtime files and test intent were unchanged.

### Fixture hotfix

The first executed M4-01 gate discovered **403** tests with **400 passed / 3 failed**. The three failures were test-fixture defects:
- a supposedly invalid uppercase slot ID contained only digits and was therefore unchanged by uppercasing;
- unsupported-head JSON guessed package document identity instead of mutating one valid serialized current document;
- unsupported-manifest JSON had the same fixture problem.

One test file was corrected. Runtime behavior and architecture remained unchanged.

### Final gate

The rerun passed **403 / 403**, failures **0**.

## Conclusion

ESV-M4-01 satisfies its authorized stop point and is ready for documentation closeout.
