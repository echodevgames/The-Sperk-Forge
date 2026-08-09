# <Package Public Title> Distribution Manifest

## Identity

| Field | Value |
|---|---|
| Public title | `<TITLE>` |
| Technical package | `<TECHNICAL ID>` |
| Package ID | `<PACKAGE ID>` |
| Package version | `<VERSION>` |
| Artifact | `<FILENAME>` |
| Source baseline | `<COMMIT/TAG/BASELINE>` |
| Kit path | `Distributions/<Title>/<Version>/` |

## Kit Contents

- `<artifact>`
- `<complete handout>`
- `DISTRIBUTION_MANIFEST.md`
- `DISTRIBUTION_BUILD_RECORD.txt`
- `SHA256SUMS.txt`
- `README.md`

## Artifact Scope

Describe what the artifact contains.

## Explicit Exclusions

List repository/showcase/project content that is intentionally not bundled.

## Integrity

Use `SHA256SUMS.txt` and `DISTRIBUTION_BUILD_RECORD.txt`.

## Qualification State

| Gate | State | Evidence |
|---|---|---|
| Artifact prepared | `<STATE>` | `<EVIDENCE>` |
| Embedded/local development | `<STATE>` | `<EVIDENCE>` |
| Clean-project tarball install | `<STATE>` | `<EVIDENCE>` |
| Removal/reinstall | `<STATE>` | `<EVIDENCE>` |
| Player build | `<STATE>` | `<EVIDENCE>` |
| Performance | `<STATE>` | `<EVIDENCE>` |
| Release/private beta | `<STATE>` | `<EVIDENCE>` |

Artifact presence alone is not a support claim.
