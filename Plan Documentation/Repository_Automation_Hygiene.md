# SFGSS Repository Automation Hygiene

**Status:** Active suite workflow rule  
**Established:** 2026-08-15

This document records repository-level automation hygiene for The Sperk's Systems Foundry. It exists to prevent repeated apply/seal failures caused by line-ending churn, brittle path parsing, generated Unity files, sample-import metadata, and hand-built patch assumptions.

## 1. Line endings

The root `.gitattributes` file is the repository authority for tracked-file line endings.

- C#, Markdown, ordinary data/config text, Unity source text, UI Toolkit text, and Unity YAML/JSON content use LF in Git-managed text.
- Windows Command Prompt scripts (`.cmd`, `.bat`) use CRLF in the working tree.
- Visual Studio solution files (`.sln`, `.slnx`) use CRLF in the working tree.
- Repository-local Git should use `core.autocrlf=false` so `.gitattributes` owns conversion policy.
- Generated text payloads must be normalized before packaging and must contain exactly one terminal newline.

Do not hide line-ending problems by redirecting Git stderr. Fix the policy or payload instead.

## 2. Generated bundle construction

Generated Foundry apply/seal/closeout bundles must:

1. verify the exact expected Git baseline and synchronized `origin/main`;
2. require a clean index before applying a new scope;
3. prefer complete known-baseline file payloads over hand-written unified-diff patches when exact source files are available;
4. never emit malformed abbreviated `@@` patch hunks;
5. never parse quoted `git status --porcelain` paths as plain filenames;
6. use exact per-path Git checks and individual `git add -- "path"` commands;
7. avoid multiline CMD caret continuations for critical Git staging;
8. normalize Markdown/text EOFs before zipping;
9. verify package/imported sample parity where both copies are intentionally present;
10. leave implementation, closeout, and repository-hygiene commits as separate boundaries.
11. resolve/capture the helper's absolute bundle directory before any `cd`;
12. never use byte-for-byte comparison of text working-copy files as a baseline check when Git line-ending normalization may differ; exact HEAD + clean working tree is the baseline proof.

## 3. Dirty-scope validation

Do not rely only on brittle absolute dirty-file counts when Unity may create known generated churn.

Use:

- explicit allowed-path manifests for authored scope;
- exact expected tracked/untracked classes;
- allow-listed generated churn handling only.

Never broadly restore unknown dirty files.

## 4. Known generated churn

The helper may automatically restore only a deliberately recognized generated class, such as:

- Unity/IDE regeneration of the tracked solution file when it is outside the checkpoint scope;
- the exact known imported-sample container `.meta` churn created by Package Manager sample reimport.

Any unexpected generated-file class must stop the helper rather than being silently discarded.

## 5. Package sample authoring

For package-distributed samples:

- `Assets/Samples/...` is the editable imported project working copy;
- `Packages/<package>/Samples~/...` is the package-distributed source of truth;
- intentional hand-authored changes made in the imported copy must be saved and synchronized back to `Samples~` before Package Manager Reimport or implementation sealing;
- package/imported parity must be verified before the implementation commit.

## 6. Failure behavior

A failed preflight must leave the repository unchanged whenever practical.

If a helper has already applied an intentional dirty scope and a later formatting/validation gate fails, the continuation helper should accept that exact interrupted state rather than forcing the user to redo validated work.

The goal is strict safety without turning every checkpoint into a ritual of avoidable retries.
