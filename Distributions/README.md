# The Sperk's Forge - Package Distribution Kits

This directory is the repository-owned handoff surface for versioned Sperk's Forge package artifacts.

Every independently distributed package receives a **Distribution Kit** after its in-house Package Reference Showcase (or approved non-scene equivalent) and before the artifact is relied on for clean-project/release qualification.

## Standard Layout

```text
Distributions/
├── README.md
├── _Template/
│   ├── COMPLETE_USER_HANDOUT_TEMPLATE.md
│   └── DISTRIBUTION_MANIFEST_TEMPLATE.md
└── <Public Title>/
    └── <Package Version>/
        ├── README.md
        ├── <package-artifact>.tgz
        ├── <COMPLETE_USER_HANDOUT>.md
        ├── DISTRIBUTION_MANIFEST.md
        ├── DISTRIBUTION_BUILD_RECORD.txt
        └── SHA256SUMS.txt
```

## Required Kit Contents

A versioned kit contains:

1. **Exact package artifact** - normally the UPM `.tgz` tarball with one `package/` root.
2. **Complete user handout** - detailed installation, setup, all implemented capabilities, workflows, diagnostics, troubleshooting, limitations, evidence/qualification state, removal/reinstall guidance, reference examples, and issue-reporting requirements.
3. **Distribution manifest** - human-readable identity, contents, scope, exclusions, and qualification boundary.
4. **SHA-256 record** - integrity hashes for the retained kit files.
5. **Build record** - package/source baseline, artifact size, and artifact SHA-256.
6. **Kit README** - short "start here" routing page.

## Artifact Presence Is Not a Support Claim

A kit freezes and identifies what will be evaluated next.

It does **not** mean that:

- tarball clean-project installation has passed;
- Git/tag/registry routes are supported;
- player builds have passed;
- performance targets have passed;
- beta, release-candidate, stable, or catalog/tag gates have passed.

SFGSS-004 remains the evidence authority. SFGSS-009 retains final release publishing/tag/catalog authority.

A route that has not passed its required proof remains Planned or Unknown.

## Version Immutability

Once a versioned kit has been retained and used for evidence, do not silently replace it with a materially different artifact under the same package version.

If the artifact changes materially:

- create the appropriate new package version, or
- use an explicitly recorded corrected-candidate process authorized by release policy;
- rerun affected evidence.

## Project-Owned Showcase Content

A package's in-house Reference Showcase/Gallery is not automatically bundled into the package artifact.

Only package-owned Runtime/Editor/presentation/docs/tests/samples and other approved package contents belong in the tarball.

Project-owned branding, consumer scenes, and production reference assets remain outside the artifact unless the package specification explicitly promotes them into a redistributable sample.

## Git LFS

Binary tarballs are repository artifacts and should use Git LFS.

The repository `.gitattributes` includes `*.tgz` for this purpose.

## First Kit

First Light `0.1.0` is the first package using this standard:

```text
Distributions/First Light/0.1.0/
```
