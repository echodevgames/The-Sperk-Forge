# First Light - Current Notes

## Completed Checkpoint

- Checkpoint: `FL-M5-05`
- Title: Direct Scene Development Initializer
- Package version: `0.1.0`
- Specification: SFGSS-PKG-ECHOLAUNCH-001 v1.11.0
- ADR: EchoLaunch-ADR-008
- Authority commit: `d538b5a`
- Implementation commit: `4aa6ce7`
- Documentation closeout: pending this adjacent commit
- Status: Implemented, automated-tested, manually accepted, and pushed
- Compilation: `0` errors, `0` warnings
- Focused Direct Scene EditMode: `5` passed
- Focused Direct Scene PlayMode: `24` passed
- Complete EditMode: `266` passed
- Complete Runtime PlayMode: `503` passed
- Total automated: `769` passed

## Implemented Outcome

A directly opened gameplay or Test Lab scene may now enter the normal First
Light launch architecture through an explicitly authored project-owned
`DirectSceneConfiguration`.

The helper reuses existing authority or creates exactly one approved
`DirectSceneDevelopment` root. An already active destination settles without
reloading the scene being tested.

## Accepted Evidence

- Existing scene authority is reused before creation.
- One valid missing authority creates one approved root.
- Two initializers converge on one accepted authority.
- Direct destination already active completes without scene reload.
- Editor-only is the default.
- Development Builds require explicit opt-in.
- Non-development release-player root creation is impossible.
- `ELAUNCH-VAL-009` is active and read-only.
- Development-Build opt-in produces `NeedsAttention`.
- Restoring `EditorOnly` returns the exact original healthy fingerprints.
- Temporary acceptance content and project drift were removed before the
  implementation commit.

## Stable Healthy Fingerprints

```text
Request:
5c8748493af793488d04f400ac2dfd000645315706a0306aafd492ec92a2dfb0

Evidence:
64706f20f36d21d21bdb61d826f30c698fe7c9cead86109d3ec2132fe075d82e

Report:
cab6e106a92eda1da382133c809f2bc273c5e36ed279fe7bb37908353106aaa3
```

## Handoff

- Active checkpoint: None
- Next checkpoint: Not selected
- Next action: Commit and push this FL-M5-05 documentation closeout, then
  perform the next just-in-time learning and authority review
