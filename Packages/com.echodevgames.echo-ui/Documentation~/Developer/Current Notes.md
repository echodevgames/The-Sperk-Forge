# The Looking Glass — Developer Current Notes

**Package:** `com.echodevgames.echo-ui`
**Package version:** `0.1.0`
**Authority:** SFGSS-PKG-ECHOUI-001 v1.9.0
**Suite authority:** SFGSS-000 v0.27.0
**Workflow authority:** SFGSS-005 v1.7.0
**Unity baseline:** 6000.3.8f1
**uGUI:** 2.0.0
**Last reconciled:** August 18, 2026
**Current checkpoint:** EUI-M5-01 Primitive Warehouse Foundation — ACTIVE / AUTHORIZED; implementation not started

> This is a living current-state page. Durable architecture remains in the package specification and accepted checkpoint plans/decisions. Resolved detail may be compacted after promotion because Git preserves archaeology.

## Current boundary

EUI-M1-01 through EUI-M4-03 are complete. EUI-M5-01 is now the active bounded authoring checkpoint.

The package continues to own UI presentation infrastructure rather than gameplay, pause/time scale, input maps, scene travel, persistence, audio, localization, domain truth, or project-wide lifetime composition. Runtime package version remains `0.1.0`, the required Unity package remains `com.unity.ugui` `2.0.0`, and EUI-M5-01 adds no hard peer Echo dependency.

## EUI-M5-01 ACTIVE / AUTHORIZED

**Checkpoint:** Primitive Warehouse Foundation, Screen/Panel/Primitive Taxonomy, and Drag-and-Drop Starter Family

**Activation parent:** `a8f70f855715bcf48be1e96fb694c41867282125`

**Package authority:** SFGSS-PKG-ECHOUI-001 v1.9.0 — unchanged. The live package authority already contains the durable Assembly Library promise, including Lego primitives, ordinary editable Panel/Menu templates, project-owned composition, project-extensible later catalogs, independent Assembly Utilities, and a later Builder/Composer. EUI-M5-01 is a bounded realization of that approved direction rather than a new Level-2 architecture.

### Incoming accepted floor

The final post-EUI-M4-03 run on the activation parent is:

- Full Foundry EditMode: **1445 / 1445 PASS**.
- EchoUI Editor: **339 / 339 PASS**.
- All Motif fixtures: **62 / 62 PASS**.
- Root fixture: **12 / 12 PASS**.
- Failed / skipped / inconclusive: **0 / 0 / 0**.

Manual EUI-M4-03 Motif Laboratory remains **6 / 6 PASS**, 180-frame idle quiescence PASS, retained M4-02 through M1 smoke green, and package/imported Motif proof parity verified.

### Warehouse taxonomy

The approved composition model is:

```text
Screens / Screen Templates
    whole editable starting compositions
        ↓
Panels / Panel Modules
    reusable functional compositions
        ↓
Primitives
    focused reusable pieces
```

Motifs are **orthogonal** to all three construction tiers. Motifs control participating appearance; they do not become another hierarchy level and do not own layout, navigation, content, or domain meaning.

The primary Warehouse workflow is intentionally plain: browse `Packages/com.echodevgames.echo-ui/.../Warehouse/` in Unity and drag ordinary prefabs into a Canvas/project UI hierarchy. Users may then use standard prefab overrides, variants, unpacking, copying, reparenting, and project-owned customization.

The Warehouse is designed to grow organically and may eventually become very large. Real projects may later identify generic reusable assets that can be deliberately cleaned, validated, and promoted into package source. No automatic promotion tooling is active now.

### First authorized starter family

Exactly five roles are authorized for the first implementation slice:

1. default square Button primitive;
2. square Close Button visual primitive, with actual close command left to project/surface wiring;
3. default panel-surface Image primitive;
4. Button Group Panel composed from starter primitives;
5. Basic Menu Screen Template assembled from the starter Panel/Primitives.

The initial package paths are defined by the EUI-M5-01 Checkpoint Build Plan under `Runtime/Prefabs/Warehouse/{Primitives,Panels,Screens}`.

### Motif relationship

The starter family should use the already-proven EUI-M4-03 Motif contract where useful. Existing `UseMotif` / `KeepLocal` bindings are available. EUI-M5-01 does not authorize automatic hierarchy scanning, a new Motif registration authority, a Motif Palette, capture/apply/preview tools, or final typography/provider schema.

The first family may intentionally avoid locking a text-provider choice by exposing project-populated content/label anchors rather than adding a mandatory text dependency.

### Explicitly excluded

Motif Palette; Motif authoring tools; Template Catalog; Assembly Utilities; Builder/Composer; automatic project-to-package promotion; community Creator Lab; broad Warehouse population; production/final 9-slice art family; final typography-provider schema; full accessibility; prompts/tooltips; safe area; richer Window management; persistence; peer bridges; integration; clean-project qualification; release.

### Exact resume phase

**Implementation has not started.** Present the first five-role implementation slice again and wait for Jesse's explicit `go` before creating any prefab/test assets.

## EUI-M4-03 retained closeout

EUI-M4-03 Runtime Motif Foundation is COMPLETE / CLOSED. Final accepted evidence: Foundry **1445 / 1445**, EchoUI **339 / 339**, Motifs **62 / 62**, root **12 / 12**, 0/0/0 failed/skipped/inconclusive; manual Motif Laboratory **6 / 6 PASS**; 180-frame idle PASS; retained smoke green; package/imported parity verified.

Durable chain: `435fc66` activation → `d67550d` contracts → `172d230` catalog/fallback → `43da17a` service → `efbc503` targets → `e17d816` bindings → `ab5906c` root integration → `d291885` test-only teardown correction → `b48eae68` Laboratory → `7f9272bd` / `8188b91c` bounded proof corrections → `a8f70f8` closeout.

## Retained checkpoint summary

- EUI-M1-01: 1113 / 1113; Lab 5 / 5.
- EUI-M1-02: 1130 / 1130; EchoUI 24 / 24; Lab 10 / 10.
- EUI-M2-01: 1153 / 1153; EchoUI 47 / 47; focused 23 / 23; Lab 10 / 10.
- EUI-M2-02: 1181 / 1181; EchoUI 75 / 75; focused 28 / 28; Lab 12 / 12.
- EUI-M3-01: 1205 / 1205; EchoUI 99 / 99; focused 24 / 24; Lab 12 / 12.
- EUI-M3-02: 1246 / 1246; EchoUI 140 / 140; Lab 14 / 14.
- EUI-M4-01: HUD Lab 5 / 5; retained smoke green.
- EUI-M4-02: 1383 / 1383; EchoUI 277 / 277; notifications 125 / 125; presenter 17 / 17; Lab 6 / 6.
- EUI-M4-03: 1445 / 1445; EchoUI 339 / 339; Motifs 62 / 62; root 12 / 12; Lab 6 / 6.
- **EUI-M5-01: ACTIVE / AUTHORIZED; implementation not started.**
