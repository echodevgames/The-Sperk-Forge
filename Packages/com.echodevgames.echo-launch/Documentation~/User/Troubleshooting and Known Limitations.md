# First Light Troubleshooting and Known Limitations

## Start with these three checks

1. Confirm the Console has no compile errors.
2. Run **Tools > Sperk's Forge > First Light > Validator**.
3. Copy the Validator report before changing the project.

Use Setup to create or narrowly repair the canonical project-owned foundation.
Do not delete/recreate assets merely to silence a diagnostic; stable IDs and
Unity GUIDs are part of the package contract.

## Common setup problems

| Symptom or code | Meaning | First action |
|---|---|---|
| `ELAUNCH-CFG-001` | Root configuration is missing or invalid. | Assign the project-owned current-schema configuration; refresh Setup/Validator. |
| `ELAUNCH-ROOT-001` | A duplicate launch root lost the authority claim. | Keep one intended canonical authority; inspect enabled scenes and prefabs. |
| `ELAUNCH-DEST-001` | Destination is invalid or not build-loadable. | Save the destination and add it exactly once, enabled, to the Scene List. |
| `ELAUNCH-SETUP-002` | A target path contains incompatible content. | Preserve it; choose a different root/path or resolve the conflict manually. |
| `ELAUNCH-SETUP-003` | Historical or unsupported schema requires migration. | Stop. Current Repair does not migrate schemas. |
| `ELAUNCH-SETUP-004` | Moving Boot first needs explicit approval. | Review scene order, select `PlaceFirstAfterApproval`, and check the approval box only when intended. |
| `ELAUNCH-SETUP-008` | The displayed plan became stale. | Press **Refresh Plan** and review the new plan. |
| `ELAUNCH-SETUP-015` | Repair ownership/shape cannot be proven. | Do not force Repair; inspect the named asset/scene/prefab manually. |
| `ELAUNCH-VAL-009` | Direct Scene authoring or policy needs attention. | Prefer `EditorOnly`; verify configuration, prefab, destination, and scene ownership. |
| `ELAUNCH-DIRECT-001` | Direct entry is blocked by policy/environment. | Use canonical Boot or select an explicitly approved development policy. |
| `ELAUNCH-STEP-003` | A step crossed its timeout. | Inspect timeout, cancellation support, and executor settlement. |
| `ELAUNCH-STEP-005` | Caller cancellation interrupted startup. | Inspect the cancellation owner and retained step report. |

## Setup button is disabled

Apply or Repair can be unavailable when:

- Unity is compiling, updating, entering Play Mode, or already playing;
- another Setup mutation is active;
- the plan is blocked, stale, or requires a manual decision;
- place-first approval is required but not checked; or
- the plan contains operations not authorized for the selected action.

Wait for Unity to settle, refresh, and read the displayed diagnostics.

## Boot opens but the destination does not load

- Confirm the configured destination asset points to the intended scene path.
- Confirm that scene exists and is enabled exactly once in the Scene List.
- Confirm the root references the canonical project configuration.
- Check the final immutable launch report for the stopping step and diagnostic.
- Use the Laboratory invalid-destination fixture to compare expected failure
  presentation.

## Direct Scene does nothing in a release player

That is intentional. A non-development player can never create a Direct Scene
development root. A production player must start through canonical Boot.

## Known limitations in `0.1.0`

- Clean-project tarball installation and Windows player proof remain `Not run`
  until FL-M6-02.
- No Git URL, registry, Workshop, or public distribution route is claimed.
- No historical configuration migration is implemented.
- Apply is create-only; Repair is limited to approved current-schema drift.
- No automatic uninstall/reset or project-content pruning exists.
- No crash-persistent setup transaction recovery or receipt system exists.
- No automatic Direct Scene installation or build hook exists.
- No persistent-root lifetime implementation beyond the current handoff
  behavior is claimed.
- No normal mid-game scene travel, audio, save, settings, menu, input, or
  gameplay authority belongs to First Light.
- No automatic retry, retry backoff, interactive retry, or retry/skip UI exists.
- No peer-package bridge or existing-project adoption claim exists.
- Performance/capacity evidence remains `Not run`.
- Only Unity `6000.3.8f1` with uGUI `2.0.0` is the current tested development
  baseline; the manifest's `6000.0` value is a compatibility floor, not broad
  tested-version evidence.
