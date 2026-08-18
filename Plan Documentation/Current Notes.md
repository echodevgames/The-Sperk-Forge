# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 18, 2026
**Current focus:** The Looking Glass EUI-M5-01 Primitive Warehouse Foundation
**Current checkpoint:** EUI-M5-01 — ACTIVE / AUTHORIZED; implementation not started

> Durable authority lives in the suite/package specifications, accepted decisions, checkpoint plans, tests, and Git history. This living page stays current-state biased and may compact resolved history after promotion.

---

## Looking Glass EUI-M5-01 activation — August 18, 2026

- Activation parent: `a8f70f855715bcf48be1e96fb694c41867282125` — EUI-M4-03 final documentation closeout.
- Final post-closeout Unity evidence on that parent: full Foundry EditMode **1445 / 1445**, EchoUI Editor **339 / 339**, Motifs **62 / 62**, root **12 / 12**, failed/skipped/inconclusive **0 / 0 / 0**.
- Suite authority remains **SFGSS-000 v0.27.0**.
- Package authority remains **SFGSS-PKG-ECHOUI-001 v1.9.0**. No package-authority version bump is required because the live specification already authorizes Lego primitives, ordinary editable Panel/Menu templates, project-owned composition, Motifs as appearance, and separation of Warehouse/Template/Catalog/Utilities from the later Builder/Composer.
- Workflow authority remains **SFGSS-005 v1.7.0**.
- Package remains `com.echodevgames.echo-ui` `0.1.0` on Unity **6000.3.8f1** with required `com.unity.ugui` **2.0.0**.
- **EUI-M5-01 is ACTIVE / AUTHORIZED. Implementation has not started.**

### Bounded JIT declaration

The Primitive Warehouse is an intentionally open-ended, organically growing authored asset collection. Its success is not measured by staying small forever. It should eventually become a very large collection of proven reusable UI pieces, with organization and discoverability solving scale.

The concrete three-tier taxonomy is:

```text
Screens / Screen Templates
    whole editable starting compositions
    built from Panels and Primitives

Panels / Panel Modules
    reusable functional groups
    built from Primitives and optionally other Panels

Primitives
    focused reusable UI pieces with one obvious job
```

**Motifs are orthogonal to all three tiers.** They control participating appearance and do not become a fourth construction tier.

The primary no-tool workflow is direct Unity Project-window browsing and **dragging package prefabs into the project Canvas/hierarchy**. A later Motif Palette, Template Catalog, Assembly Utilities, and Builder/Composer may accelerate this workflow, but the Warehouse must remain useful without any of them.

All Warehouse assets remain ordinary editable Unity prefabs. Projects may override, variant, unpack, duplicate, rearrange, and customize them with normal Unity workflows. The package does not introduce an opaque generated-object format.

Real project work may later produce generic candidates such as audio-slider panels, player-info panels, item groups, save panels, or new primitive families. Those may be deliberately cleaned, validated, and promoted into the package through future bounded work. Automatic promotion and community/Creator Lab workflows are future ideas only.

### First authorized starter family

EUI-M5-01 authorizes exactly five roles:

- Primitive: default square Button.
- Primitive: square Close Button visual starting point; actual close meaning remains project/surface wiring.
- Primitive: default panel-surface Image.
- Panel: Button Group Panel composed from starter primitives.
- Screen Template: Basic Menu composition assembled from the starter Panel/Primitives.

The first slice deliberately avoids locking the final typography/provider schema. Existing EUI-M4-03 Motif bindings may be used where useful, but no new automatic Motif registration model or scene scan is authorized.

### Explicitly still inactive

Motif Palette; Motif capture/apply/preview Editor tooling; Template Catalog; Assembly Utilities; Builder/Composer; automatic project-to-package promotion; Creator Lab/community ingestion; broad Warehouse population; production art/final 9-slice library; final typography/provider contract; full accessibility; prompts/tooltips; safe area; richer Window management; persistence; peer-package bridges; integration; clean-project qualification; release.

### Exact resume phase

Activation is documentation-only. Before any Warehouse prefab/test asset is created, present the first implementation slice and wait for Jesse's explicit `go`.

---

## EUI-M4-03 retained closeout

EUI-M4-03 Runtime Motif Foundation is **COMPLETE / CLOSED**.

Durable chain: `435fc66` activation → `d67550d` contracts → `172d230` catalog/fallback → `43da17a` session service → `efbc503` targets → `e17d816` bindings → `ab5906c` root integration → `d291885` test-only teardown correction → `b48eae68` Laboratory → `7f9272bd` Check 3 proof correction → `8188b91c` Check 4 proof correction → `a8f70f8` final closeout.

Accepted evidence:

- full Foundry **1445 / 1445**;
- EchoUI **339 / 339**;
- Motifs **62 / 62**;
- root **12 / 12**;
- failed/skipped/inconclusive **0 / 0 / 0**;
- Motif Laboratory **6 / 6 PASS**;
- 180-frame idle quiescence PASS;
- retained M4-02 through M1 representative smoke user-confirmed green;
- package/imported Motif proof parity verified.

Intentional observer/target exception logs remain accepted isolation proofs. Check 5 deliberately exercised the broken target during registration and switching, producing two caught target exception logs.

---

## Retained Looking Glass chain

- EUI-M1-01 complete: full 1113 / 1113; Lab 5 / 5.
- EUI-M1-02 complete: full 1130 / 1130; EchoUI 24 / 24; Lab 10 / 10.
- EUI-M2-01 complete: full 1153 / 1153; EchoUI 47 / 47; focused 23 / 23; Lab 10 / 10.
- EUI-M2-02 complete: full 1181 / 1181; EchoUI 75 / 75; focused 28 / 28; Lab 12 / 12.
- EUI-M3-01 complete: full 1205 / 1205; EchoUI 99 / 99; focused 24 / 24; Lab 12 / 12.
- EUI-M3-02 complete: full 1246 / 1246; EchoUI 140 / 140; Lab 14 / 14.
- EUI-M4-01 complete: HUD Lab 5 / 5; retained smoke green.
- EUI-M4-02 complete: full 1383 / 1383; EchoUI 277 / 277; notifications 125 / 125; presenter 17 / 17; Lab 6 / 6.
- EUI-M4-03 complete: full 1445 / 1445; EchoUI 339 / 339; Motifs 62 / 62; root 12 / 12; Lab 6 / 6.
- **EUI-M5-01 active: Primitive Warehouse foundation; implementation not started.**

## Suite boundary reminder

Looking Glass remains presentation infrastructure. Chronicle owns save transport, Passage owns scene flow, Accord owns preferences, Resonance owns audio, Will owns input, Pulse/project state owns pause/game-state truth, and project composition owns long-lived Unity composition. Future whole-game UI may present those systems through optional bridges/presenters without transferring their authority into Warehouse prefabs.
