# SUITE-DOC-18 - EchoMultiplayer Disposable Prototype Protocol

**Status:** Approved protocol; all executions Not run  
**Date:** August 4, 2026  
**Purpose:** Compare candidate providers through the same tiny vertical slice without allowing prototype code to become production package code automatically.

---

## 1. Prototype rule

Each provider receives a disposable Unity project or isolated branch containing the same project-owned neutral gameplay slice. Provider-specific code lives only in one adapter/integration area. The project may be deleted after evidence capture.

At least two providers must complete the protocol before a production provider is approved.

## 2. Fixed vertical slice

1. Start offline and display provider/capability status.
2. Create a session as host or server.
3. Join from a second standalone Player build.
4. Show two participants and ready states.
5. Start the match only when both are ready.
6. Synchronize travel from Lobby to Game scene.
7. Spawn one selectable character/pawn per participant.
8. Move one capsule or sprite through a provider-appropriate authority path.
9. Execute one authoritative action: interact with a switch that increments a server/authority-owned counter.
10. Reject one invalid client request.
11. Disconnect one client and attempt rejoin/reconnect.
12. End or interrupt the host and record migration/recovery behavior.
13. Return to lobby or shut down cleanly.
14. Produce a redacted diagnostic/evidence bundle.

## 3. Fixed project boundaries

- Passage remains the project scene-transition authority; provider code coordinates through a prototype bridge.
- Fellowship-style durable character identity remains separate from provider object IDs.
- Gameplay counter truth changes only at the provider's authoritative location.
- UI consumes neutral snapshots.
- No provider SDK type appears in the project-owned gameplay core.
- No prototype may modify approved package specifications silently.

## 4. Required topologies

Every provider prototype must record which roles/topology it actually tests. At minimum:

- One player-hosted host/client path or equivalent.
- One separate client Player build.
- Dedicated server path where the candidate claims it and the project considers it relevant.
- Distributed/shared authority only as a separate measured variant.

## 5. Network-condition matrix

Run the fixed slice under:

| Profile | RTT target | Jitter | Loss | Purpose |
|---|---:|---:|---:|---|
| Local clean | Minimal | Minimal | 0% | Baseline |
| Good internet | 50 ms | 5 ms | 0.1% | Normal play |
| Moderate | 100 ms | 15 ms | 1% | Realistic stress |
| Poor | 180 ms | 30 ms | 3% | Degradation behavior |
| Interruption | Temporary disconnect | N/A | 100% window | Reconnect/recovery |

Provider-native simulation may be used when documented; otherwise an external network-condition tool is recorded.

## 6. Evidence to capture

- Exact Unity, provider, transport, service, and adapter versions.
- Install source, license, and package manifest.
- Time to first successful host/join.
- Provider-specific source files and lines of code.
- Neutral gameplay files changed.
- Roles/topology and authority decisions.
- Build profiles and command-line/server setup.
- Session, scene, spawn, reconnect, and shutdown logs.
- CPU, memory, allocations, bandwidth, RTT, jitter, loss, and correction metrics where available.
- Screenshots/video of each protocol phase.
- Failure symptoms and workarounds.
- Service/hosting/pricing assumptions.
- Removal test and project compile after provider deletion.

## 7. Scoring

Score 0-5 with evidence for each research criterion. A score without a linked test/log/screenshot/source is invalid. Weighted scoring may be applied only after project requirements are written.

## 8. Security tests

- Client attempts authority-only counter mutation.
- Client submits out-of-range or malformed request.
- Duplicate/replayed request ID.
- Client claims another participant's character/entity.
- Disconnect during committed action.
- Session metadata exceeds bounds.
- Diagnostic export contains no token/ticket/secret/private identifier.

## 9. Exit report

Each prototype ends with:

- Pass/fail per protocol step.
- Capability gaps.
- Measured results.
- Provider-specific code map.
- License/cost/hosting note.
- Security findings.
- Removal result.
- Recommendation: advance, hold, or reject.

The report does not modify the production provider decision. A separate comparison review and ADR make that decision.

## 10. Current execution state

| Prototype | Candidate | Status |
|---|---|---|
| A | NGO + Multiplayer Services SDK | Not run |
| B | FishNet after license review, otherwise Mirror | Not run |
| C | Photon Fusion 2 conditional | Not run |
