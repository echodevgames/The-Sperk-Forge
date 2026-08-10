# ESV-M4-03 — Manual Save Transaction Composition, Unknown Carry-Forward, and Catalog Reconciliation Foundation

**Status:** Complete
**Planning baseline:** `a3eba25`
**Planning/activation commit:** `2c325e9`
**Implementation commit:** `c8ea742`
**Unity baseline:** 6000.3.8f1
**Focused Chronicle Editor gate:** **439 / 439 passed, 0 failed**
**Prior regression floor:** **425 / 425**
**Net new focused tests:** **14**

## Completed boundary

ESV-M4-03 composes one deterministic internal manual-save transaction for the explicitly selected healthy active slot.

Completed flow:

```text
selected healthy active slot
→ current-generation validation
→ exact source provenance refresh
→ fresh known participant capture
→ opaque unknown carry-forward
→ collision-safe merge
→ expected-current-generation stale check
→ immutable participant-backed generation publication
→ final verification
→ head.json LAST
→ catalog reconciliation
→ truthful durable/head/catalog result
```

## Verified behavior

- no active slot or unhealthy/missing active slot fails before participant capture;
- current-generation validation binds the exact source slot/generation;
- failed source read does not replace trusted unknown-session state with untrusted data;
- fresh participant capture remains deterministic and all-or-nothing;
- valid opaque unknown entries survive carry-forward;
- unknown ownership collision and source-provenance mismatch block before publication;
- stale expected-current-generation publication is rejected;
- publication retains candidate verification, immutable generation publication, final verification, and `head.json` last;
- ordinary save preserves current display name;
- successful catalog reconciliation exposes the new current generation and preserves active selection;
- durable head success followed by catalog-refresh failure is reported truthfully without deleting committed state;
- participant Apply/default callbacks are not used.

## Deferred

M4-03 does not expose public `SaveAsync`, production operation admission/Busy/cancellation, autosave, retention, recovery, persistent catalog cache, rename/duplicate/delete/trash, full slot-policy assets, document migration, scene travel, peer bridges, service-locator behavior, or Chronicle-owned/project-wide DDOL.

## Stop point

The checkpoint is complete at `c8ea742`.

No follow-on M4 checkpoint is activated by this closeout. Further runtime implementation requires a new bounded Checkpoint Build Plan and must preserve the **439 / 439** focused regression floor.
