# SUITE-DOC-18 - EchoMultiplayer Provider Research Plan and Dated Matrix

**Status:** Approved research foundation; provider decision pending  
**Research date:** August 4, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Authority relationship:** Supports `SFGSS-The-Convergence-EchoMultiplayer-Package-Foundation.md`; does not select a provider  
**Evidence rule:** Documentation and vendor claims are research inputs, not prototype results.

---

## 1. Purpose

Compare viable Unity multiplayer providers for The Sperk's Forge without converting documentation claims into production guarantees. The outcome of this record is a shortlist, prototype order, evidence rubric, and explicit unknowns. A provider may be approved only after at least two disposable implementations execute the same protocol.

## 2. Candidate shortlist

### Candidate A - Unity Netcode for GameObjects + Multiplayer Services SDK

**Research snapshot:**

- Unity's current package documentation identifies Netcode for GameObjects 2.13.1 as a high-level GameObject networking library.
- Current NGO documentation includes client-server and distributed-authority topologies, integrated scene management, ownership, spawning, and dedicated-server samples.
- Unity explicitly states that the NGO client-server path provides client anticipation rather than full client-side prediction and reconciliation.
- The Multiplayer Services SDK unifies Sessions with Lobby, Relay, and Matchmaker workflows.
- Unity's Sessions host-migration documentation distinguishes host election from game-state migration; default data migration is documented for Netcode for Entities, while NGO projects are directed toward distributed authority for migration-related operations.

**Strengths to test:** first-party Unity alignment, Package Manager integration, current Unity 6 documentation, integrated services, scene management, distributed authority option.

**Risks/unknowns:** client-server prediction requirements, service cost and lock-in, distributed-authority cheat model, host-migration semantics, provider/service version churn, dedicated-server operations.

**Prototype slot:** Mandatory Prototype A.

### Candidate B - FishNet

**Research snapshot:**

- Official repository/docs report FishNet 4.7.2R as the latest release dated April 17, 2026.
- Official documentation describes client-server networking, host and dedicated server operation, prediction, synchronized scene management, ownership, area-of-interest systems, and self-selected hosting.
- Official documentation states FishNet does not currently include built-in host migration.
- The repository uses a custom license. It grants no-charge/royalty-free use for games but includes exclusions for competing networking solutions. A public EchoMultiplayer adapter therefore requires explicit license review before approval.

**Strengths to test:** prediction and reconciliation tools, scene manager, observer/AOI features, self-host flexibility, feature-rich free runtime.

**Risks/unknowns:** custom-license compatibility with a provider-neutral adapter product, host migration absence, versioning/distribution workflow, provider-specific lifecycle complexity, console/platform path.

**Prototype slot:** Conditional Prototype B if license review clears; otherwise replace with Mirror.

### Candidate C - Mirror

**Research snapshot:**

- Official releases list Mirror 96.11.1 dated July 26, 2026.
- Mirror is MIT licensed and describes itself as server-authoritative with host and dedicated-server patterns and transport flexibility.
- Mirror provides interest-management and scene examples.
- Mirror's client-side prediction documentation labels the current prediction work experimental and specifically warns that predicted player movement remains unproven.

**Strengths to test:** clear permissive license, active releases, source access, flexible hosting/transports, mature server-authoritative model.

**Risks/unknowns:** prediction fit for action gameplay, reconnect/host migration implementation burden, service integration, adapter boilerplate, future API churn.

**Prototype slot:** Mandatory fallback/control baseline for Prototype B when FishNet license review does not clear; optional third prototype otherwise.

### Candidate D - Photon Fusion 2

**Research snapshot:**

- Fusion 2 documentation covers Host, Server, and Shared modes, prediction, lag compensation, interest management, dedicated-server samples, and built-in network-condition simulation.
- Photon Cloud pricing on August 4, 2026 lists a development-only 20 CCU tier and a one-app free launch tier of 100 CCU, with paid plans above that.
- Fusion uses Photon services and App IDs, creating a managed-cloud and vendor-cost dimension distinct from self-hosted libraries.

**Strengths to test:** mature prediction/lag compensation, multiple topologies, tooling, managed cloud, action-game samples, network-condition tools.

**Risks/unknowns:** service lock-in, CCU/traffic cost, offline/self-host requirements, licensing/redistribution, provider-native architecture leakage, console/native socket requirements.

**Prototype slot:** Conditional Prototype C when fast action, lag compensation, or managed cloud is a leading requirement.

### Screened candidate - Netcode for Entities

Netcode for Entities is not an initial provider prototype because the current suite and intended games use GameObject/MonoBehaviour architecture. It may be researched for an ECS-specific project. Its existence must not pressure the general suite into DOTS dependencies.

## 3. Comparison matrix

| Criterion | NGO + MPS | FishNet | Mirror | Photon Fusion 2 |
|---|---|---|---|---|
| First-party Unity alignment | Strong | Community/vendor | Community | Commercial vendor |
| License/service model | Unity packages + optional UGS services | Custom source license; self-host capable | MIT; self-host capable | Commercial SDK/cloud plans |
| Host/client | Documented | Documented | Documented | Documented |
| Dedicated server | Documented samples | Documented | Supported pattern | Documented samples |
| Distributed/shared authority | NGO distributed authority | Client-server focus | Server-authoritative focus | Shared Mode available |
| Full prediction/reconciliation | NGO client-server: no; anticipation only | Documented prediction | Experimental prediction | Documented prediction |
| Lag compensation | Prototype/research required | Research required | Research required | Documented feature |
| Scene synchronization | Integrated NGO scene management | Built-in scene manager | NetworkManager/examples | Provider-native scene support |
| Interest management | Research/provider features | Observer system | Interest-management system | AOI documented |
| Host migration | Complex/topology-dependent; MPS distinction applies | Not built in per docs | Project/provider work; not approved | Topology/service-specific; prototype required |
| Relay/lobby/matchmaking | MPS SDK | Separate services/transports | Separate services/transports | Photon Cloud/session services |
| Self-host flexibility | Yes for client-server/dedicated; service choices vary | High | High | Dedicated possible, cloud dependency/topology details require proof |
| Current version snapshot | NGO 2.13.1 docs | 4.7.2R | 96.11.1 | Fusion 2 current docs |
| Adapter license risk | Normal Unity/provider terms review | **High: custom license review required** | Low relative: MIT | Commercial terms review required |
| Current suite fit | Strong baseline candidate | Strong technical candidate if license clears | Strong open-source control baseline | Strong conditional action/cloud candidate |
| Prototype result | Not run | Not run | Not run | Not run |

## 4. Required prototype order

1. **Prototype A:** NGO 2.13.1 plus current Multiplayer Services SDK, using one explicitly selected topology and documented session/relay path.
2. **Prototype B:** FishNet 4.7.2R only after license review; otherwise Mirror 96.11.1.
3. **Prototype C, conditional:** Photon Fusion 2 when the target slice requires mature prediction/lag compensation or a managed-cloud comparison.

At least two prototypes must complete. The same gameplay-neutral slice and scoring rubric applies to every candidate.

## 5. Decision rubric

Each candidate receives evidence-backed scores for:

- Learning curve and setup clarity.
- Provider-specific code volume.
- Authority model clarity.
- Host/client/dedicated support.
- Prediction, reconciliation, lag compensation, and physics fit.
- Scene synchronization and late join.
- Reconnect and host migration.
- Spawn/ownership and interest management.
- Diagnostics, network simulation, and debugging.
- Performance and bandwidth.
- Clean package/adapter isolation and removal.
- Platform/build support.
- Documentation and maintenance signals.
- License, pricing, hosting, service lock-in, and operating burden.
- Fit for Jesse's likely small co-op/action/RPG projects.

No category may use a marketing claim as a passing result.

## 6. Explicit unknowns

- Production topology.
- Required player count and tick rate.
- Prediction/lag-compensation requirement.
- Hosting budget and operational tolerance.
- Relay/lobby/matchmaking requirements.
- Host migration requirement.
- Console/mobile/WebGL target requirements.
- Provider legal/license acceptance.
- Dedicated-server deployment provider.
- Whether Steam or platform networking is required.

## 7. Official source register

All links were reviewed August 4, 2026.

- Unity NGO current manual: https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@latest/
- NGO latency and client anticipation: https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.13/manual/latency-performance.html
- NGO topology comparison: https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.13/manual/terms-concepts/network-topologies.html
- NGO scene management: https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.13/manual/basics/scenemanagement/scene-management-overview.html
- Unity Multiplayer Services SDK: https://docs.unity.com/en-us/mps-sdk
- Unity Sessions host migration: https://docs.unity.com/en-us/mps-sdk/session-host-migration
- FishNet official docs: https://fish-networking.gitbook.io/docs
- FishNet prediction: https://fish-networking.gitbook.io/docs/guides/features/prediction
- FishNet networking models/host migration statement: https://fish-networking.gitbook.io/docs/guides/high-level-overview/networking-models
- FishNet source/license: https://github.com/FirstGearGames/FishNet
- Mirror official docs: https://mirror-networking.gitbook.io/docs
- Mirror source/releases/license: https://github.com/MirrorNetworking/Mirror
- Mirror prediction status: https://mirror-networking.gitbook.io/docs/manual/general/client-side-prediction
- Photon Fusion 2 docs: https://doc.photonengine.com/fusion/current/getting-started/fusion-intro
- Photon network-condition simulation: https://doc.photonengine.com/fusion/current/manual/testing-and-tooling/simulating-network-conditions
- Photon Fusion pricing: https://www.photonengine.com/Fusion/Pricing

## 8. Research conclusion

No provider is approved. NGO/MPS is the mandatory first-party baseline. The second prototype slot is FishNet only if license review accepts a public EchoMultiplayer adapter; otherwise Mirror supplies the open-source control baseline. Photon Fusion remains a conditional third candidate for action/prediction/cloud comparison. Provider selection requires executed evidence and a separate ADR.
