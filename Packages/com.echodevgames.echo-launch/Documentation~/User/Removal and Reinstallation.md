# First Light Removal and Reinstallation

First Light package source, imported sample content, and project-owned setup
content are separate things. Remove only the layer you intend to remove.

## Remove the Standalone Test Lab sample

Delete only the imported folder beneath:

```text
Assets/Samples/First Light — Startup and Launch/<version>/First Light Standalone Test Lab
```

This preserves the package, Setup, Validator, Simulator, and any canonical
project-owned foundation. Reimport the sample from Package Manager for a clean
copy.

## Disable First Light without deleting project data

1. Stop Play Mode.
2. Restore the intended pre-First-Light startup scene order.
3. Remove or disable project-specific adapters that call First Light.
4. Remove First Light root/initializer components from project scenes only
   after verifying the rollback path.
5. Commit or back up the project before package removal.

## Remove the package

Use Package Manager to remove the package route that installed First Light.
For an embedded development package, repository/Git operations own the package
folder; do not delete it as though it were a consumer install.

Removing package code does not automatically delete:

```text
Assets/EchoDevGames/FirstLight
```

Those assets are project-owned. Keep them for reinstall, migration, evidence,
or manual cleanup. Because their scripts belong to the removed package, Unity
may show missing-script/import errors until First Light is reinstalled or the
project-owned content is deliberately removed.

## Reinstall

1. Reinstall the exact compatible `.tgz` or other previously used package
   revision.
2. Wait for compilation.
3. Confirm `com.echodevgames.echo-launch` appears in Package Manager.
4. Run the Validator against the preserved project root.
5. Open Setup and refresh the plan.
6. Expect compatible current-schema assets to be reused, not duplicated.
7. Use Repair only when its proof-backed plan explicitly authorizes the
   displayed changes.
8. Run canonical Boot again.

## Destructive cleanup boundary

First Light `0.1.0` has no automatic uninstall/reset/prune command. Deleting
project-owned assets is a separate destructive project decision and is not
performed by Setup, Repair, Validator, sample removal, or package removal.
