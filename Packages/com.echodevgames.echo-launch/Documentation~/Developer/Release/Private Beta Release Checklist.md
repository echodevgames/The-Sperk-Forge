# First Light Private Beta Release Checklist

**Target:** `0.1.0-beta.1`
**Audience:** invited private testers
**Claimed install route:** local `.tgz` only
**Stable/public release:** not claimed

## Candidate preparation

- [ ] Clean working tree at the intended candidate commit.
- [ ] `package.json` version is `0.1.0-beta.1`.
- [ ] Changelog, README, documentation index, Current Notes, license, and notices
      agree with the candidate.
- [ ] Package manifest metadata contains no machine-local or mutable source.
- [ ] Full development-repository test matrix passes from the candidate commit.
- [ ] Candidate `.tgz`, size, SHA-256, source commit, and creation tool are
      recorded.

## Clean-project proof

- [ ] New Windows Unity `6000.3.8f1` project contains no unrelated Echo package.
- [ ] Candidate installs from `.tgz` with the expected version and uGUI `2.0.0`.
- [ ] Console compiles cleanly.
- [ ] Quick Start is completed using only shipped documentation.
- [ ] Setup first pass succeeds and repeat pass returns `NoChanges`.
- [ ] Validator reaches the documented accepted state.
- [ ] Canonical Boot reaches the selected destination.
- [ ] Standalone Test Lab imports and required scenarios pass.
- [ ] Sample removal and reimport pass.
- [ ] Package removal/reinstall preserves and reuses project-owned data as
      documented.
- [ ] Non-development Windows player begins at Boot and reaches Destination.
- [ ] Direct Scene development creation remains prohibited in that release
      player.

## Private tester proof

- [ ] Tester environment and candidate checksum are recorded.
- [ ] Tester completes the documented path without undocumented assistance.
- [ ] Confusion, defects, and advisories have durable records.
- [ ] Every blocker/critical/major issue in advertised scope is resolved or the
      candidate is rejected.
- [ ] Affected gates are repeated after each correction.

## Closeout

- [ ] Final package version matches annotated tag `v0.1.0-beta.1`.
- [ ] Final `.tgz` is built from the tagged clean commit.
- [ ] Artifact checksum and size match the private release record.
- [ ] Known limitations and private licensing boundary are visible.
- [ ] Git URL, registry, broad Unity support, stable API, adoption, bridge,
      migration, and performance claims remain unclaimed.
- [ ] `main`, `origin/main`, and the release tag agree with the closeout record.
- [ ] Working tree is clean.
