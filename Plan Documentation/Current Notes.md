# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 8, 2026
**Current focus:** Suite Reference Showcase authority and First Light production-use phase
**Current checkpoint:** Suite authority update — SFGSS-ADR-005; FL-M6-01 authority follows after commit

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Current State

- First Light FL-M5-07 is complete and closed at documentation commit `710aec3`.
- Final FL-M5-07 automated evidence: `809 / 809`.
- Final FL-M5-07 manual Laboratory evidence: `12 / 12`.
- Repository baseline for this authority change: exact clean `710aec3`.
- First Light package version remains `0.1.0`.
- First Light package specification remains v1.13.0 until the fresh M6 package-authority revision is approved.

## Newly Approved Suite Direction

`[DECISION]` Every package will receive a clean in-house **Package Reference Showcase** after its isolated Standalone Test Lab proof.

The intended evidence layers are now:

```text
Package implementation
        ↓
Standalone Test Lab
engineering microscope
        ↓
Package Reference Showcase
production-style display case
        ↓
Clean-project reproduction
outside-consumer proof
        ↓
Integration demonstrations / the Suite Showcase Hub
suite collection and composition
```

The Reference Showcase:

- is project-owned rather than immutable package source;
- uses the same documented public setup/configuration/API surfaces available to an outside consumer;
- must not depend on test-only APIs, hidden internals, or unrelated package code;
- shows the normal front-facing happy path by default;
- may use one scene or the smallest scene set required by the real workflow;
- has an Editor/workspace equivalent for genuinely non-scene packages;
- normally lives under `Assets/EchoDevGames/SuiteShowcase/<Package>/`.

`[DECISION]` A future project-owned **Suite Showcase Hub** will link or launch package Reference Showcases and integrated demonstrations but is not a runtime package, authority, or substitute for package evidence. **Suite Showcase Hub is a functional placeholder name only.** Its final lore-facing name remains intentionally unresolved and may draw from the Hackulos / Sperk-galaxy large computer-brain cosmology.

## Official Package Graduation Loop

```text
Learning / package authority
        ↓
Implementation + automated regression
        ↓
Standalone Test Lab
        ↓
Package Reference Showcase
        ↓
Clean-project reproduction
        ↓
Release qualification
        ↓
Private beta / external adoption
```

This is the suite-wide development-to-release rhythm. The display case is the bridge between engineering proof and real consumer/release proof, not a substitute for either.

## First Light Next Phase

The next First Light work will establish the pattern.

The planned M6 sequence is:

1. **FL-M6-01 — First Light Production Reference Showcase**
   - create the first in-house display case;
   - use project-owned real/front-facing splash configuration;
   - show normal Boot → splash(es) → startup → destination behavior;
   - keep diagnostics secondary;
   - use only public First Light consumer surfaces.

2. **FL-M6-02 — Clean-Project Consumer Reproduction**
   - install First Light into a genuinely clean Unity project;
   - reproduce the same production-style startup flow;
   - prove no hidden repository-local dependency.

3. Later M6/M7 release qualification
   - Git/tarball route proof;
   - player builds;
   - performance evidence;
   - packaging/version/release candidate;
   - private-beta handoff.

This ordering keeps the exciting visible proof small and understandable before returning to heavier release qualification.

## Authority Boundary

This suite authority change does **not** authorize FL-M6-01 implementation by itself.

After the suite authority commit:

- update First Light package specification from the synchronized baseline;
- create a fresh FL-M6-01 Checkpoint Build Plan;
- perform another drift audit before implementation;
- do not resurrect discarded post-rewind M6 work unless a specific item is deliberately reviewed and reintroduced.

## Next Action

1. Audit exact clean `710aec3`.
2. Apply and review SFGSS-000 v0.24.0, SFGSS-001 v1.3.0, SFGSS-004 v1.3.0, SFGSS-ADR-005, ADR log, Suite Graph Roadmap, and this Current Notes update.
3. Commit and push the suite Reference Showcase authority.
4. Build fresh FL-M6-01 First Light authority from the resulting synchronized commit.
