# The Chronicle — Save Laboratory

This sample is the deliberately minimal direct-scene engineering laboratory for The Chronicle (`EchoSave`).

It is not a production save menu, a UI framework, or the Chronicle Reference Showcase.

## Import

1. Open **Window → Package Manager**.
2. Select **The Chronicle — Save Infrastructure**.
3. Open **Samples**.
4. Import **Chronicle Save Laboratory**.
5. Open `Scenes/Chronicle_Save_Laboratory.unity`.
6. Press Play.

No Build Settings change is required for direct-scene testing.

## What the panel proves

The IMGUI panel is intentionally crude. It drives the real public Chronicle runtime surface.

The centerpiece flow is:

1. **CREATE SLOT**
2. mutate `SPERK-001`
3. **SAVE**
4. **MUTATE VALUES WITHOUT SAVING**
5. **LOAD & APPLY**
6. verify `RESULT: THE CHRONICLE REMEMBERS.`

The visible state is intentionally tiny:

- Sperk Level
- Galactic Rupees
- Anvil Temperature
- Has Forbidden Key
- Reality Damage

Additional controls expose slot selection, rename, duplicate, delete Preview/Confirm, prepared-load Prepare/Apply/Dispose, catalog refresh, and owned Laboratory reset.

## Isolation

The sample configuration uses exactly:

`EchoSave-M5-06-Laboratory`

under `Application.persistentDataPath`.

On successful initialization the Laboratory writes an ownership marker:

`m506-laboratory-owned.txt`

with value:

`ECHOSAVE-M5-06-LABORATORY`

**RESET LAB** shuts Chronicle down and deletes the root only when both the exact root name and ownership marker are verified.

The Laboratory refuses to delete any other root.

## Acceptance boundary

LAB-001 through LAB-032 remain the M5 acceptance matrix, but this scene does not pretend every row needs a dedicated polished screen.

Human-visible flows live here when they add integration value. Existing focused automated tests remain the correct proof for concurrency, cancellation, fault injection, migration, oversized payload, and other cases that are more truthful under deterministic automation.

The sample must never print PASS without checking the real Chronicle result/state behind that line.

## Presentation boundary

The comic-relief labels are sample-owned.

Runtime APIs and Chronicle technical diagnostics remain neutral.

Do not reuse this IMGUI panel as a production save menu.

Polished examples such as:

- single-slot Continue/New Game;
- recognizable three-slot fantasy-adventure files;
- configurable fixed slot counts;
- broad manual-save browsers;
- large technical slot catalogs;
- manual/autosave/checkpoint UX variants;

belong to the later **Chronicle Reference Showcase**, preferably after **The Looking Glass** and **Resonance** are packaged.

## Reset

Use:

**DO NOT PRESS UNLESS REALITY IS BACKED UP — RESET LAB**

The button verifies ownership, removes only the M5-06 Laboratory root, and reports post-cleanup truth.

After reset, stop Play Mode and press Play again for a fresh Laboratory session.
