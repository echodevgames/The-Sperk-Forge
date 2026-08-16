# SFGSS-ADR-007 — Green Path Self-Validating Checkpoint Execution

**Document ID:** SFGSS-ADR-007
**Version:** 1.1.0
**Status:** Accepted
**Decision date:** August 13, 2026
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Decision scope:** Checkpoint execution automation, evidence, Git boundaries, and conversational handoffs
**Related authorities:** SFGSS-000, SFGSS-005 v1.7.0, SFGSS-ADR-004

> Keep the proof. Remove the toll booths.

## Context

The checkpoint process has produced strong architecture, unusually useful Git history, exact test evidence, and reliable rollback behavior. The cost is conversational latency: routine successful staging, whitespace, commit, push, and evidence gates often require a separate ChatGPT response even when the next action is deterministic.

The desired change is **not** fewer tests, fewer commits, weaker documentation, or larger speculative slices. The desired change is to let a locally self-validating checkpoint contract advance through expected green gates without waiting for conversational approval after each one.

## Decision

1. The suite adopts **Green Path** as the preferred routine execution mode after a checkpoint completes Learn → Declare → Authorize.
2. A Green Path kit may automate exact-scope file application, validation, staging, commits, pushes, evidence parsing, closeout documentation, and final clean-state verification.
3. Small checkpoint boundaries remain mandatory. Green Path reduces interactions, not checkpoint rigor.
4. Meaningful Git archaeology is preserved. Authority/activation, implementation, and closeout remain separate commits when those boundaries carry distinct information.
5. Successful expected gates may continue locally without ChatGPT approval between phases.
6. Any compile/test/manual-proof failure, repository mismatch, rollback failure, unexpected file scope, or authority-changing discovery stops immediately and returns to review.
7. Rollback is successful only after post-rollback verification of HEAD/origin, staged state, expected working-tree state, and checkpoint-owned files.
8. Generated text payloads are sanitized before packaging: trailing spaces/tabs removed, final newline preserved, and generator-side validation run. `git diff --check` remains a repository safety net.
9. Apply helpers must create parent directories before copying nested new files.
10. A Green Path helper may commit/push only after machine-verifiable gates pass. It must report the resulting hashes and final repository state.
11. Automation never fabricates Unity evidence. Required compile, automated-test, player, Inspector, and manual visual proof remain explicit gates.
12. Routine bounded implementation details, test maintenance, documentation closeout, and compile corrections remain pre-approved when they do not change authority. Fundamental package ownership, dependency, serialized-compatibility, or public-contract changes return to Jesse.

## Connected-repository clarification — August 16, 2026

- Green Path remains visible and slice-bounded: ask/confirm the slice, implement it, test and correct it, push the implementation boundary, prove it manually when required, close it out, then present the next slice.
- Direct repository access may replace manual source uploads and Git-status shuttling. It does not replace Unity evidence that only Jesse's local Editor can produce.
- Before a remote write, ChatGPT announces the exact phase and scope, verifies the branch head, and advances it without force from that exact parent.
- `green` confirms only the requested gate. `go` advances only a slice or phase already presented. Neither phrase silently activates an adjacent checkpoint.
- Authority, implementation, and closeout remain distinct evidence boundaries.

## Consequences

### Positive

- Waiting on routine conversational handoffs drops sharply.
- The same small-slice discipline can produce substantially faster development.
- Machine-collected commit/test/stat evidence reduces transcription errors.
- Rollback, whitespace, and clean-state checks become standardized inside the kit instead of remembered manually.
- ChatGPT time is concentrated on architecture, failures, and the next useful slice.

### Costs

- Execution helpers become real engineering artifacts and require their own validation.
- A bad helper can make several local changes quickly, so exact scope and verified rollback are mandatory.
- Manual evidence must remain explicit so automation does not turn “should work” into “passed.”

## Rejected alternative

**Collapse checkpoints into large feature batches.** Rejected. The current small-slice Git/documentation model is valuable; only the conversational serialization is being removed.

## Review trigger

Revisit after three packages use Green Path, after a rollback verification failure, or if helpers become more complex than the checkpoints they execute.

## Approval

**Decision:** Accepted
**Approved by:** Jesse “Echo” Adams / EchoDevGames
**Date:** August 13, 2026
