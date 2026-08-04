# SUITE-DOC-19 - EchoAI Feasibility and Provider Record

**Status:** Approved research foundation; implementation and provider adapters remain Not run  
**Date:** August 4, 2026  
**Package:** Instinct (`EchoAI`)  
**Authority relationship:** Supports `Package Specifications/SFGSS-Instinct-EchoAI-Package-Foundation.md`; it does not override SFGSS-000 or select a mandatory backend.

## 1. Feasibility conclusion

A provider-neutral EchoAI package is feasible and useful when it stops at reusable sensing, memory, scoring, typed context, scheduling, behavior lifecycle, navigation request contracts, and diagnostics. It becomes harmful when it attempts to own enemy personality, movement physics, combat, every behavior architecture, every pathfinding system, or neural inference.

The approved foundation therefore uses actor-local hosts, scene/world registries, explicit providers, bounded state, deterministic fixtures, and separate adapters. EchoAI remains an Advanced candidate until implementation and project adoption prove the design.

## 2. Backend landscape reviewed on August 4, 2026

| Backend | Official package/documented version observed | Useful role | Foundation decision |
|---|---|---|---|
| Unity AI Navigation | `com.unity.ai.navigation` 2.0.14 documentation | Build/use NavMeshes, agents, links, and obstacles for 3D navigation | Candidate separate NavMesh provider adapter; never a core dependency |
| Unity Behavior | `com.unity.behavior` 1.0.16 documentation | Visual graph-based behavior authoring, runtime graph execution, and blackboards | Candidate separate visual behavior adapter after neutral core proof |
| Unity Inference Engine | `com.unity.ai.inference` 2.6.1 documentation | Import and execute trained neural-network models at runtime | Experimental later adapter only; EchoAI does not train models or trust raw inference as authority |
| Project/custom navigation | Project-defined | 2D grids, A*, waypoint networks, steering, flight, bespoke movement | First-class provider seam; no vendor assumption |
| Simulated providers | EchoAI test fixtures | Deterministic observations, actions, and navigation | Mandatory standalone Laboratory path |

These versions are dated observations, not package manifest pins or compatibility claims. They must be rechecked when an adapter checkpoint begins.

## 3. Why AI Navigation is not mandatory

AI Navigation is a strong 3D NavMesh option, but the suite also targets top-down 2D, side-view 2D, grid movement, click-to-move, flying, swimming, vehicles, and custom controllers. A mandatory NavMesh dependency would silently narrow EchoAI's genre and technology boundary.

The neutral navigation contract therefore exposes destination requests, status, arrival, cancellation, warp, partial-path policy, and provider capabilities. The adapter decides how those map to a NavMeshAgent, grid solver, steering system, controller motor, or simulated fixture.

## 4. Why Unity Behavior is not the neutral contract

Unity Behavior provides a useful visual and modular behavior-graph workflow. However, graph assets, blackboard binding, node lifecycle, and provider APIs belong to that package. Making them the EchoAI core would force projects to install and version the visual backend even when a small state machine, utility selector, custom tree, or project planner is sufficient.

The planned adapter maps neutral observations/context and explicit action/condition providers to a Behavior graph. The exact mapping remains Not run.

## 5. Why inference is deferred

Runtime neural inference introduces model files, operator support, backend selection, platform performance, determinism, frame slicing, security, and authority questions. It also does not replace game-authored validation. An inferred action or score is advisory until normal gameplay authority accepts it.

The first EchoAI release therefore contains no neural inference dependency. A later experimental provider requires its own adapter specification, performance evidence, fallback behavior, and multiplayer/security review.

## 6. Recommended implementation order after SUITE-DOC-33

1. Pure stable IDs, memory, scoring, blackboard, and deterministic policies.
2. Actor-local host, scene/world registry, scheduler, and simulated providers.
3. Lightweight state-machine and utility behavior execution.
4. Standalone Laboratory and debugging.
5. One narrow project adoption.
6. First navigation adapter selected from actual project need, likely Unity AI Navigation for a 3D/NavMesh use case or a project 2D provider for Hackulos.
7. Unity Behavior adapter only after the neutral behavior/action contracts are stable.
8. Inference only as a later experimental adapter.

## 7. Explicit unknowns

- Measured agent counts and budgets.
- First production navigation backend.
- Final 2D navigation strategy.
- Whether the first visual behavior adapter is valuable enough to maintain.
- Durable AI snapshot requirements.
- Cross-platform determinism limits.
- Server/client AI presentation strategy under the selected multiplayer provider.
- Inference platform/performance/security value.

Every unknown remains Not run or undecided until its checkpoint produces evidence.

## 8. Official sources reviewed

- Unity AI Navigation 2.0.14 manual: https://docs.unity3d.com/Packages/com.unity.ai.navigation%402.0/manual/index.html
- Unity Behavior 1.0.16 manual: https://docs.unity3d.com/Packages/com.unity.behavior%401.0/manual/index.html
- Unity Behavior graph and blackboard documentation: https://docs.unity3d.com/Packages/com.unity.behavior%401.0/manual/behavior-graph.html and https://docs.unity3d.com/Packages/com.unity.behavior%401.0/manual/blackboard-variables.html
- Unity Inference Engine 2.6.1 manual: https://docs.unity3d.com/Packages/com.unity.ai.inference%402.6/manual/index.html
- Unity NavMeshAgent API: https://docs.unity3d.com/6000.1/Documentation/ScriptReference/AI.NavMeshAgent.html

## 9. Research approval

**Decision:** Approved as the dated feasibility/provider record for SUITE-DOC-19.  
**Provider selected:** None.  
**Implementation evidence:** Not run.  
**Next research trigger:** First navigation/behavior/inference adapter checkpoint after core implementation is authorized and proven.
