# SUITE-DOC-21 - EchoAbilities Feasibility and Boundary Record

**Status:** Approved  
**Date:** August 4, 2026  
**Owner:** Jesse “Echo” Adams / EchoDevGames  
**Related authority:** `../Package Specifications/SFGSS-Arcana-EchoAbilities-Package-Foundation.md`

## 1. Purpose

This record captures the feasibility reasoning used to approve Arcana as a provider-neutral Advanced package foundation without approving implementation or specific game content.

## 2. Feasibility conclusion

A reusable ability package is feasible when it owns the lifecycle of an ability rather than the meaning of every effect. The stable center is:

- Ability definitions and owner-scoped runtime state.
- Grants and loadouts.
- Activation validation and availability reasons.
- One resource-cost transaction boundary.
- Charges, cooldown groups, cast/channel timing, interruption, and recovery.
- Provider-neutral target snapshots.
- Explicit typed effect execution.
- Stable events, diagnostics, persistence seams, and multiplayer authority gates.

The package becomes infeasible or genre-locked if it also owns universal resources, stats, health, classes, item effects, status stacking, animation, VFX, audio, camera, input, UI, networking, or every specific spell and attack.

## 3. Key boundary conclusions

### 3.1 Clash

Clash owns instantaneous combat requests and target-owned combat-resource transactions. Arcana may execute an effect that submits a Clash request after Arcana commitment. Arcana does not duplicate damage formulas, team checks, targetability, or defeat outcomes.

### 3.2 The Vault and project resources

Arcana coordinates one mutation-capable cost provider per activation in the MVP. The provider may own mana, stamina, ammunition, item charges, or another project resource and may atomically commit multiple cost lines. Arcana never reaches into a container or stat object directly.

### 3.3 The Fellowship

Arcana uses an `AbilityOwnerId`. A Fellowship bridge may map a durable `CharacterId` to an owner and manage spawn/despawn/loadout lifecycle. Arcana does not own rosters, character availability, or possession.

### 3.4 The Will and The Looking Glass

Input adapters submit semantic activate, cancel, interrupt, and targeting commands. UI reads snapshots and lifecycle events. Neither input nor UI becomes required by the core.

### 3.5 Impact, Eye, Resonance, and presentation

Presentation effects are explicit non-authoritative bridges. Arcana's lifecycle completes even if camera, audio, VFX, animation, or feedback listeners are absent or fail.

### 3.6 The Chronicle

Durable state may contain grants, loadouts, and optionally cooldown/charge data. Active targets, casts, channels, queues, prepared costs, effect tickets, and provider handles are session-only.

### 3.7 The Convergence

Shared-world activations default to server/host authority. Clients may predict presentation only unless a provider-specific adapter proves stronger prediction and reconciliation. The neutral core contains no networking SDK.

## 4. Commit and interruption conclusion

Two commit policies cover the MVP:

- Commit at cast start.
- Commit at cast completion.

Before commitment, cancellation or interruption may leave resources, charges, cooldowns, and effects unchanged. After commitment, Arcana stops future work when allowed but does not promise automatic rollback of committed costs or external effects.

This distinction is required for trustworthy debugging and multiplayer authority.

## 5. Cost-provider conclusion

A fully generic multi-provider transaction would require distributed transaction semantics, compensation, ordering, recovery, and provider failure policy across inventory, stats, currencies, and network authorities. That is too large for the MVP.

The approved first release permits one mutation-capable resource provider per activation. It may aggregate several cost lines under one authority. Additional providers may contribute read-only requirements. A future multi-provider transaction protocol requires a dedicated ADR and implementation evidence.

## 6. Effect-execution conclusion

Effects use explicit stable executor IDs and typed payloads. Reflection, arbitrary method names, and open assembly scanning are prohibited. Effect steps carry:

- Target scope.
- Execution ordering.
- Blocking or non-blocking behavior.
- Timeout.
- Cancellation support.
- Failure policy.
- Causality and activation IDs.
- Commit status.

This allows combat, feedback, spawning, movement, world, objective, and project effects to remain separate authorities.

## 7. Status-effect conclusion

Durations, stacking, refresh rules, auras, dispels, immunities, periodic ticks, persistence, networking, UI, and removal form a coherent system of their own. They are intentionally deferred. Arcana may later host or bridge a status-effect module, but the active-ability MVP does not pretend one effect ticket solves that domain.

## 8. Feasibility risks

| Risk | Conclusion |
|---|---|
| Universal-framework growth | Controlled by strict authority boundaries and deferred modules |
| Cross-provider atomicity | Controlled by one mutation provider in MVP |
| Cast interruption ambiguity | Controlled by explicit commit policy and pre/post-commit results |
| Targeting coupling | Controlled by neutral snapshots and separate adapters |
| Reflection/tool fragility | Prohibited; explicit typed registrations required |
| Multiplayer cheating | Controlled by authoritative validation through Convergence bridge |
| Save corruption | Controlled by detached safe-point snapshots and unknown-data preservation |

## 9. Approved feasibility statement

Arcana is approved as an Advanced provider-neutral package foundation. This approval covers documentation contracts only. Implementation, status effects, passive abilities, multi-provider transactions, networking prediction, performance, platform compatibility, and integration evidence remain `Not run` or deferred.
