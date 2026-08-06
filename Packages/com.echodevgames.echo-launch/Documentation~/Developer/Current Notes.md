# First Light - Current Notes

## Current Status

- Last reconciled: August 6, 2026
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.9.0
- Completed checkpoint: `FL-M5-03`
- Title: Explicit Setup Repair and Existing-Asset Reconciliation
- ADR: EchoLaunch-ADR-006
- Authority commit: `6615c8f`
- Implementation commit: `dd15768`
- Status: Implemented, automated-tested, manually accepted, and ready for documentation closeout

## Delivered FL-M5-03 Boundary

First Light now exposes a separate explicit repair transaction without weakening
create-only Apply.

Implemented repair surfaces:

- Reconcile configuration sequence, destination, and optional splash references.
- Reconcile the current-schema destination scene path while preserving an
  authored non-empty label.
- Rebind configuration on a verified package-template root-prefab variant.
- Add one project-root-prefab instance to the exact canonical Boot scene only
  when zero roots are present.
- Add or enable the uniquely identified canonical Boot Build Settings entry
  under the approved policy.
- Create still-missing foundation targets inside the same explicitly approved
  repair transaction when needed.

Every existing-file repair requires fresh evidence, explicit confirmation,
proven type/schema/identity/lineage/shape, and byte-for-byte asset plus `.meta`
backup before modification.

## Validation

- Compilation: `0` errors, `0` warnings
- EditMode: `236` passed, `0` failed, `0` ignored
- Runtime Play Mode: `479` passed, `0` failed, `0` ignored
- Total automated: `715` passed
- Manual Repair 1: `Succeeded`
- Manual Repair 2: `NoChanges`
- Manual Repair 3: `NoChanges`
- Stable accepted fingerprint:
  `56526ade68938e38bb6e87fde77d17b6f89329731a813fdf5a36c1a1c57bf77f`
- Stable IDs and Unity GUIDs preserved
- Unrelated configuration values, prefab content, Boot-scene marker object,
  destination scene, package root template, and unrelated Build Settings state
  preserved
- Successful transaction left no retained backup or manual recovery path
- Generated acceptance assets and Build Settings drift removed before staging

The first focused EditMode run exposed two defects: project-path validation
occurred after filesystem lookup, and Build Settings repair reused the Boot
scene path in repaired-path reporting. Both were corrected before the final
complete EditMode and Runtime Play Mode gates.

## Preserved Boundary

FL-M5-03 does not authorize:

- Runtime changes.
- Schema migration or stable-ID regeneration.
- Type replacement.
- Sequence or splash content repair.
- Duplicate-root deletion or arbitrary scene cleanup.
- Prefab replacement, rebase, unpack, or structural rewrite.
- Move, rename, delete, or relocation.
- Destination-scene modification.
- Receipts, uninstall/reset, automatic crash recovery, Direct Scene, Validator,
  or Laboratory.

## Next Action

Close out FL-M5-03 documentation adjacent to implementation commit `dd15768`.
After closeout, select and approve the next bounded First Light checkpoint before
any further implementation. No later M5 capability is authorized by this note.
