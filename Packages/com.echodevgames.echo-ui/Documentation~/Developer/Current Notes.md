# The Looking Glass — Developer Current Notes

**Package:** `com.echodevgames.echo-ui`
**Package version:** 0.1.0
**Authority:** SFGSS-PKG-ECHOUI-001 v1.2.0
**Current checkpoint:** EUI-M1-02 — ACTIVE / AUTHORIZED
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0

## Current boundary

EUI-M1-01 is complete and remains the proven foundation: package-local root authority, stable surface registration, one exclusive screen scope, Back history, and independent-window coexistence.

EUI-M1-02 is now the active bounded implementation slice. It adds externally supplied active/inactive context IDs, designer-ordered per-surface response rules, independent visibility/interactability/selection intents, local/runtime overrides, and input-aware selection policy while preserving package independence.

Motifs, Builder tooling, actual preset/template tooling, broad primitive expansion, arbitrary context payloads, automatic input detection, movable/resizable/persisted MMO layouts, peer bridges, richer modal/HUD/transient systems, and release qualification remain outside EUI-M1-02.

## EUI-M1-01 completion

- Activation: `83d3f9e`.
- Implementation: `e6b651f`.
- Partial closeout: `4f94c3b`; final recovery/reconciliation: `57a4fa4`.
- Full EditMode: **1113 / 1113 passed, 0 failed**.
- Manual Laboratory: **5 / 5 passed**.
- Finished package sample: `Samples~/LookingGlass_UI_Foundation_Laboratory/`.
- Project Laboratory uses standard TextMesh Pro UI labels with TMP Essential Resources; this does not add a runtime dependency from EchoUI to another Echo package.
- EUI-M1-02 JIT reconciliation: specification v1.2.0; **ACTIVE / AUTHORIZED** from baseline `57a4fa4`.
- EUI-M1-02 implementation status at activation: **Not started**.
- First implementation gate: re-establish the retained **1113 / 1113** full EditMode floor before runtime changes.
