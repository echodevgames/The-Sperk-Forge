# SUITE-DOC-20 - EchoCombat Feasibility and Boundary Record

**Status:** Approved pre-code research and boundary record  
**Package:** Clash (`EchoCombat`)  
**Date:** August 4, 2026  
**Authority relationship:** Supporting Level 4 record beneath SFGSS-000, SFGSS-002 through SFGSS-005, and the approved Clash foundation

## 1. Purpose

This record explains the design alternatives reviewed before approving Clash. It does not contain implementation evidence and does not select one genre formula, target resource model, physics technology, or networking provider.

## 2. Core feasibility conclusion

A reusable combat package is feasible only when it owns the *operation and evidence pipeline* rather than every combatant's mutable state. The stable reusable center is:

1. An immutable combat request.
2. Explicit source and target identities.
3. Targetability and relation evaluation.
4. Pure ordered resolution modifiers.
5. A target-owned prepare/commit receiver transaction.
6. A structured result and post-commit semantic events.

Health, armor, stats, shields, durability, posture, faction reputation, abilities, equipment, AI, presentation, saves, and networking remain separate authorities.

## 3. Alternatives reviewed

| Alternative | Decision | Reason |
|---|---|---|
| Universal `IDamageable.TakeDamage(float)` | Rejected as public foundation | Too little context, no structured failures, no idempotency, encourages direct mutation |
| Package-owned universal Health component | Rejected from core | Conflicts with project and RPG stat authorities; unsuitable for props, shields, phases, vehicles, and non-health targets |
| Signed float where negative damage means healing | Rejected | Ambiguous policy, overflow/precision differences, poor explanation and validation |
| Checked fixed-point magnitude plus semantic operation kind | Approved foundation | Deterministic, fractional-capable, explicit damage/healing distinction |
| Modifier delegates with arbitrary side effects | Rejected | Nondeterministic and impossible to explain/rollback safely |
| Pure ordered modifier records/providers | Approved | Deterministic, testable, traceable |
| Events before state mutation | Rejected | Presentation could become required authority and listeners observe uncommitted truth |
| Events after receiver commit | Approved | State truth precedes presentation and analytics |
| Hit detection inside neutral core | Rejected | Physics, melee timing, projectiles, lag compensation, and targeting differ by project/provider |
| Separate 2D/3D/custom hit adapters | Approved | Keeps resolution reusable and removal clean |
| Area attack as one distributed atomic transaction | Rejected for MVP | Multiple target authorities cannot promise universal rollback |
| Per-target transactions linked by batch/causality IDs | Approved | Honest partial outcomes and evidence |
| Client report directly applies damage | Rejected | Security and replay risk |
| Convergence/provider validates and submits authoritative request | Approved seam | Preserves network authority and provider neutrality |

## 4. Fixed-point feasibility note

The foundation proposes a checked 64-bit fixed-point magnitude. A project chooses a scale representing its smallest combat unit. This permits values such as fractional display damage while preserving deterministic integer arithmetic. Implementation prototypes must test ergonomics, conversion, overflow, rounding, inspector authoring, and network serialization before the choice graduates from approved design to supported evidence.

## 5. Receiver transaction boundary

`ICombatReceiver` prepares one application against target-owned state and returns a stale-safe application token. The receiver validates its own invariants, such as invulnerability, maximum resource, phase locks, shield layers, or destroyed state. The token commits exactly once. EchoCombat does not reach inside the target to subtract values.

The contract does not promise rollback after a successful commit. Cross-system side effects occur after commit through events or explicit orchestration.

## 6. Defeat boundary

Clash never infers defeat solely from a numeric value. The receiver reports the outcome produced by its authoritative state transition. A destructible crate may report `Destroyed`; a character may report `Defeated`; a training target may report no terminal outcome; a healing receiver may report `Recovered`. Respawn, corpse, loot, XP, objective rewards, and scene consequences remain outside Clash.

## 7. Relationship to Arcana and Instinct

Arcana will own ability activation, costs, cooldowns, cast/channel timing, interruption, targeting, and effect orchestration. One Arcana effect may submit a Clash request, but Clash does not become the ability system.

Instinct may consume committed combat outcomes as semantic observations or threat context. It does not decide authoritative damage, and Clash does not own AI decisions.

## 8. Relationship to The Convergence

The neutral core has no networking SDK. A Convergence/provider bridge decides whether the caller is authoritative, validates replay/ownership/range evidence, creates the authoritative request ID, submits the request on the authority, and replicates the result. Prediction, lag compensation, rollback, hit rewind, and reconciliation remain provider/genre-specific research.

## 9. Evidence status

- Request/result API: Approved design; Not implemented.
- Fixed-point magnitude: Approved design; Not prototyped.
- Receiver transaction: Approved design; Not prototyped.
- Modifier ordering: Approved design; Not benchmarked.
- Physics2D/3D adapters: Planned; Not implemented.
- Multiplayer authority: Planned bridge; provider not selected.
- Performance/platform/compatibility: Not run.

## 10. Recommendation

Approve the provider-neutral foundation and proceed to Arcana documentation. During final Advanced collision review, verify the exact ownership boundary among Clash instantaneous combat operations, Arcana ongoing effects/status, Instinct threat observations, The Vault/RPG stat modifiers, and The Convergence authority.
