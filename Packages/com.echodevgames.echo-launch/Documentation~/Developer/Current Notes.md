# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-03`
- Title: Explicit Setup Repair and Existing-Asset Reconciliation
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.9.0
- ADR: EchoLaunch-ADR-006
- Required baseline: `2ef594c`
- Status: Authority prepared; implementation begins only after authority commit/push

## Last Completed Checkpoint

FL-M5-02 is implemented, tested, manually accepted, documented, and pushed.

- Authority: `208ee71`
- Implementation: `f05b95c`
- Documentation closeout: `2ef594c`
- EditMode: `197` passed
- Runtime Play Mode: `479` passed
- Compilation: `0` errors, `0` warnings
- Manual Apply sequence: `Succeeded`, `NoChanges`, `NoChanges`

## Approved FL-M5-03 Boundary

Repair is separate from create-only Apply.

Authorized only for provable current-schema canonical drift:

- Reconcile configuration sequence/destination/optional-splash references.
- Reconcile destination scene path; fill label only when empty.
- Rebind configuration on a verified package-template root-prefab variant.
- Add one project-root-prefab instance to an exact canonical Boot scene with zero roots.
- Add/enable/reposition the canonical Boot Build Settings entry only under the approved policy.
- Create missing foundation targets in the same explicitly approved repair transaction when needed.

Every repair requires a fresh plan, explicit confirmation, proven type/schema/
identity/lineage/shape, and backup of existing asset + `.meta` bytes before
modification.

## Preserved Boundary

FL-M5-03 does not authorize:

- Runtime changes.
- Schema migration or stable-ID regeneration.
- Type replacement.
- Sequence/splash content repair.
- Duplicate-root deletion or arbitrary scene cleanup.
- Prefab replacement/rebase/unpack/structural rewrite.
- Move/rename/delete/relocation.
- Destination-scene modification.
- Receipts, uninstall/reset, automatic crash recovery, Direct Scene, Validator,
  or Laboratory.

## Diagnostics Added to Authority

- `ELAUNCH-SETUP-013` explicit repair approval required.
- `ELAUNCH-SETUP-014` repair backup unavailable.
- `ELAUNCH-SETUP-015` ownership/shape cannot be proven.
- `ELAUNCH-SETUP-016` repair failed; rollback completed.
- `ELAUNCH-SETUP-017` rollback incomplete; backup retained.

## Next Action

1. Review this authority bundle.
2. Commit and push:

```text
echo-launch: approve FL-M5-03 explicit setup repair
```

3. Begin implementation only from the approved FL-M5-03 plan.
