# First Light - Current Notes

## Active Checkpoint

- Checkpoint: `FL-M5-01`
- Title: Editor Setup Foundation and Non-Destructive Project Plan
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.7.0
- ADR: EchoLaunch-ADR-004
- Authority commit: `b6a4f27`
- Implementation commit: `453bc14`
- Previous documentation commit: `8bd2a57`
- Implementation status: Complete and pushed
- Documentation closeout: Pending adjacent commit
- EditMode result: 93 passed, 0 failed, 0 ignored
- Runtime Play Mode result: 479 passed, 0 failed, 0 ignored
- Compilation result: 0 errors, 0 compiler warnings

## Completed Result

Implemented:

- Read-only project snapshot collector
- Immutable setup request, facts, operations, diagnostics, and plan
- Deterministic pure setup planner
- Approved project-owned path defaults
- Path normalization and safety validation
- Compatible reuse, conflict, migration-block, and ambiguity planning
- Package-template prerequisite planning
- Append-safe and explicit-approval Build Settings policies
- Deterministic plain-text formatter
- Preview-only Setup window
- Stable setup diagnostics
- Sixty-six focused Editor tests
- Retained twenty-seven prefab tests
- Retained four-hundred-seventy-nine Runtime tests

## Evidence Summary

- Authority `b6a4f27`
- Implementation `453bc14`
- `main` equals `origin/main`
- Working tree clean
- EditMode: 93 passed
- Runtime Play Mode: 479 passed
- Total: 572 passed
- Compilation: 0 errors, 0 warnings
- Cached whitespace check passed
- Generated solution noise restored
- Folder metadata whitespace repaired

## No-Write Boundary

The window can inspect, plan, display, and copy.

It cannot Apply, Repair, Migrate, create assets/scenes/prefab variants, change
Build Settings, or store project identity in EditorPrefs.

## Not Run

- Setup apply/repair
- Asset/scene creation
- Build Settings mutation
- Migration
- Direct-scene initializer
- Standalone Laboratory
- Player builds
- Clean-project/external adoption
- Performance measurements

## Handoff Snapshot

FL-M5-01 implementation is complete and pushed at `453bc14`.

The adjacent FL-M5-01 documentation closeout is the only active repository
work.

Tentative next checkpoint: FL-M5-02 - Approved Setup Apply Engine and
Repeat-Safe Asset Creation.
