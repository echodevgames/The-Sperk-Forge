# The Sperk’s Forge – Plan Documentation

This folder is the live Git-backed Obsidian vault for **The Sperk’s Forge – EchoDevGames Game Systems Suite**.

## Current authority state

| Area | Current authority |
|---|---|
| Suite Bible | `Echo_Game_Systems_Suite_Bible.md` v0.23.0 |
| Standards | SFGSS-001 through SFGSS-010 complete |
| Package authorities | 28 of 28 approved and consistency-reviewed |
| Integration matrices | Foundation, Expansion, Advanced, Consistency, and Full Suite passed |
| Documentation handoff | SUITE-DOC-32 passed |
| Learning workflow | Just-in-time package-local gate under ADR-004 |
| Package learning reviews | 1 of 28 complete |
| First Light learning | Complete |
| Observatory learning | Paused until EchoDiagnostics implementation |
| Package implementation | Activated only for FL-M1-01; not started |
| Active checkpoint | **FL-M1-01 – First Light Package Skeleton** |
| Active implementation plan | **FL-M1-01 v1.3.0 – First Light Package Skeleton** |

## Start here

1. [SUITE-DOC-33 Initial Implementation Readiness Gate](Test%20Reports/SUITE-DOC-33_Initial_Implementation_Readiness_Gate_Report.md)
2. [FL-M1-01 First Light Package Skeleton Plan](Checkpoint%20Build%20Plans/First_Light_M1_Package_Skeleton_Checkpoint_Build_Plan.md)
3. [Full Suite Documentation and Learning Handoff Guide](Full_Suite_Documentation_and_Learning_Handoff_Guide.md)
4. [Suite Graph Roadmap](Suite_Graph_Roadmap.md)
5. [Suite Health Check](Suite_Health_Check_and_Remaining_Documentation.md)
6. [Suite Bible](Echo_Game_Systems_Suite_Bible.md)
7. [Current Notes](Current%20Notes.md)
8. [Documentation Program Roadmap](Full_Suite_Documentation_Program_Roadmap.md)
9. [Full Suite Integration Matrix](Integration%20Specifications/SFGSS-INT-SUITE-001_Full_Suite_Authority_Dependency_Bridge_and_Persistence_Matrix.md)
10. [Package Learning Review Catalog](Package_Learning_Review_Catalog.md)
11. [Learning Reviews Index](Learning%20Reviews/README.md)

## Authority order

1. SFGSS-000 Suite Bible.
2. Approved package specification/foundation.
3. Accepted ADR or approved integration specification.
4. Standards, checkpoint plans, guides, test reports, research records, and release records according to their owned concern.
5. Current Notes and navigation hubs.

## Just-in-time learning rule

Every package must pass its own learning review immediately before its first implementation checkpoint. The suite no longer requires all twenty-eight reviews before First Light begins. Completing one review never unlocks another package.

## Current checkpoint result

SUITE-DOC-33 passed with advisory and activated only FL-M1-01. PKG-LEARN-001 satisfies First Light's local learning gate. PKG-LEARN-002 remains paused, and every package other than First Light remains locally locked.

FL-M1-01 authorizes the embedded UPM package manifest, four assembly definitions, package documentation shell, generated `.meta` files, and bounded validation. It authorizes no C# file, scene, prefab, ScriptableObject, sample, setup tool, bridge, or launch behavior.

The live Unity compile, Git state, package path, and exact uGUI version must be verified before the first skeleton file is created.

## Checkpoint rule

At every meaningful checkpoint:

1. Reconcile `Current Notes.md`.
2. Promote durable truth into the owning authority or permanent record.
3. Update README, roadmap, graph, health check, tests/issues, learning tracker, and handoff as applicable.
4. Verify documentation matches actual evidence.
5. Commit and push the documentation checkpoint before advancing when practical.
