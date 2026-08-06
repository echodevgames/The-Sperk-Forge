# The Sperk’s Forge — Current Notes

**Document role:** Living development capture page
**Authority:** Working context only
**Owner:** Jesse “Echo” Adams / EchoDevGames
**Last reconciled:** August 6, 2026
**Current focus:** First Light FL-M5-05 documentation closeout
**Current checkpoint:** FL-M5-05 — Direct Scene Development Initializer

> Capture quickly here. Promote deliberately at checkpoint closeout. Git history preserves the compacted record.

---

## Completed Implementation

- Authority commit: `d538b5a`
- Implementation commit: `4aa6ce7`
- Branch: `main`
- `main` equals `origin/main`
- Working tree after implementation commit: clean
- Compilation: `0` errors, `0` warnings
- Focused Direct Scene EditMode: `5` passed
- Focused Direct Scene PlayMode: `24` passed
- Complete EditMode: `266` passed
- Complete Runtime PlayMode: `503` passed
- Total automated: `769` passed
- Failed: `0`
- Ignored: `0`

## Accepted FL-M5-05 Outcome

First Light now supports directly opening an explicitly configured gameplay or
Test Lab scene and entering the normal startup architecture.

- Existing authority is reused.
- Missing authority creates one approved direct root.
- Multiple initializers converge on one accepted authority.
- Direct destination already active completes without scene reload.
- Editor-only is the default.
- Development Builds require explicit opt-in.
- Non-development release-player root creation is impossible.
- Validator code `ELAUNCH-VAL-009` is active and read-only.

## Manual Acceptance

Healthy baseline:

```text
Request:
5c8748493af793488d04f400ac2dfd000645315706a0306aafd492ec92a2dfb0

Evidence:
64706f20f36d21d21bdb61d826f30c698fe7c9cead86109d3ec2132fe075d82e

Report:
cab6e106a92eda1da382133c809f2bc273c5e36ed279fe7bb37908353106aaa3
```

Direct Play created one authority, kept `OutdoorsScene` active, and completed
without destination reload. Existing authority was reused without another clone.
Two initializers produced one created settlement, one reused settlement, and one
accepted authority.

Development-Build opt-in returned `NeedsAttention` with one
`ELAUNCH-VAL-009` Warning. Restoring `EditorOnly` reproduced the exact original
healthy fingerprints.

## Cleanup and Preservation

- Generated acceptance content removed.
- `OutdoorsScene`, Build Settings, and solution drift restored.
- Only approved package runtime/editor/tests/metadata committed.
- No build hook or automatic helper installation added.

## Next Action

Apply, review, commit, and push the eleven-file documentation closeout:

```text
Close out FL-M5-05 direct scene initializer checkpoint
```

## Handoff

**Checkpoint:** FL-M5-05
**Authority:** `d538b5a`
**Implementation:** `4aa6ce7`
**Documentation:** Pending adjacent closeout commit
**Blockers:** None recorded
**Active next checkpoint:** None
