
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupPlanner :
        IEchoLaunchSetupPlanSource
    {
        private const int ValidatePhase = 0;
        private const int FolderPhase = 10;
        private const int DefinitionPhase = 20;
        private const int PrefabPhase = 30;
        private const int ScenePhase = 40;
        private const int BuildSettingsPhase = 50;

        public EchoLaunchSetupPlan CreatePlan(
            EchoLaunchSetupRequest request,
            EchoLaunchProjectSnapshot snapshot)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            List<EchoLaunchSetupOperation> operations =
                new List<EchoLaunchSetupOperation>();

            List<EchoLaunchSetupDiagnostic> diagnostics =
                new List<EchoLaunchSetupDiagnostic>();

            bool pathsValid =
                EchoLaunchSetupPathUtility.TryCreatePathSet(
                    request.ProjectRootPath,
                    request.BootScenePath,
                    out EchoLaunchSetupPathSet paths,
                    out string pathError);

            if (!pathsValid)
            {
                AddInvalidRequest(
                    operations,
                    diagnostics,
                    "request-paths",
                    request.ProjectRootPath,
                    pathError);
            }
            else
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        "request.valid",
                        ValidatePhase,
                        EchoLaunchSetupOperationKind.ValidateRequest,
                        EchoLaunchSetupOperationDisposition.NoChange,
                        paths.ProjectRootPath,
                        "The requested project-owned paths are valid."));
            }

            ValidateDestinationScene(request, snapshot, operations, diagnostics);
            ValidatePackageTemplate(snapshot, operations, diagnostics);

            if (pathsValid)
            {
                ResolveFolder(
                    paths.ProjectRootPath,
                    "folder.project-root",
                    snapshot,
                    operations,
                    diagnostics);

                ResolveFolder(
                    paths.ConfigurationFolderPath,
                    "folder.configuration",
                    snapshot,
                    operations,
                    diagnostics);

                ResolveFolder(
                    paths.PrefabsFolderPath,
                    "folder.prefabs",
                    snapshot,
                    operations,
                    diagnostics);

                ResolveFolder(
                    paths.ScenesFolderPath,
                    "folder.scenes",
                    snapshot,
                    operations,
                    diagnostics);

                ResolveAsset(
                    EchoLaunchSetupAssetRole.Configuration,
                    EchoLaunchSetupOperationKind.ResolveConfiguration,
                    "asset.configuration",
                    paths.ConfigurationAssetPath,
                    EchoLaunchSetupAssetTypeNames.Configuration,
                    request,
                    snapshot,
                    operations,
                    diagnostics,
                    true);

                ResolveAsset(
                    EchoLaunchSetupAssetRole.StartupSequence,
                    EchoLaunchSetupOperationKind.ResolveStartupSequence,
                    "asset.startup-sequence",
                    paths.StartupSequenceAssetPath,
                    EchoLaunchSetupAssetTypeNames.StartupSequence,
                    request,
                    snapshot,
                    operations,
                    diagnostics,
                    false);

                ResolveAsset(
                    EchoLaunchSetupAssetRole.LaunchDestination,
                    EchoLaunchSetupOperationKind.ResolveLaunchDestination,
                    "asset.launch-destination",
                    paths.LaunchDestinationAssetPath,
                    EchoLaunchSetupAssetTypeNames.LaunchDestination,
                    request,
                    snapshot,
                    operations,
                    diagnostics,
                    false);

                if (request.CreateSplashSequence)
                {
                    ResolveAsset(
                        EchoLaunchSetupAssetRole.SplashSequence,
                        EchoLaunchSetupOperationKind.ResolveSplashSequence,
                        "asset.splash-sequence",
                        paths.SplashSequenceAssetPath,
                        EchoLaunchSetupAssetTypeNames.SplashSequence,
                        request,
                        snapshot,
                        operations,
                        diagnostics,
                        false);
                }

                ResolveAsset(
                    EchoLaunchSetupAssetRole.RootPrefab,
                    EchoLaunchSetupOperationKind.ResolveRootPrefabVariant,
                    "prefab.root",
                    paths.RootPrefabPath,
                    EchoLaunchSetupAssetTypeNames.GameObject,
                    request,
                    snapshot,
                    operations,
                    diagnostics,
                    false);

                ResolveBootScene(paths, snapshot, operations, diagnostics);
                ResolveBuildSettings(
                    request,
                    paths,
                    snapshot,
                    operations,
                    diagnostics);
            }

            SortOperations(operations);
            SortDiagnostics(diagnostics);

            EchoLaunchSetupPlanStatus status =
                DetermineStatus(operations, diagnostics);

            string requestFingerprint =
                EchoLaunchSetupFingerprint.ForRequest(request);

            string evidenceFingerprint =
                snapshot.EvidenceFingerprint;

            string planFingerprint =
                EchoLaunchSetupFingerprint.ForPlan(
                    requestFingerprint,
                    evidenceFingerprint,
                    status,
                    operations,
                    diagnostics);

            return new EchoLaunchSetupPlan(
                request,
                paths,
                snapshot.CreateEvidenceSummary(),
                status,
                operations,
                diagnostics,
                requestFingerprint,
                evidenceFingerprint,
                planFingerprint);
        }

        private static void ValidateDestinationScene(
            EchoLaunchSetupRequest request,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            if (string.IsNullOrWhiteSpace(request.DestinationScenePath))
            {
                AddInvalidRequest(
                    operations,
                    diagnostics,
                    "destination.required",
                    string.Empty,
                    "Select an existing project destination scene.");
                return;
            }

            if (!EchoLaunchSetupPathUtility.TryNormalizeProjectAssetPath(
                    request.DestinationScenePath,
                    ".unity",
                    out string normalized,
                    out string error))
            {
                AddInvalidRequest(
                    operations,
                    diagnostics,
                    "destination.invalid-path",
                    request.DestinationScenePath,
                    error);
                return;
            }

            EchoLaunchProjectAssetFact fact = snapshot.FindAssetFact(normalized);

            if (!fact.Exists ||
                !fact.IsType(EchoLaunchSetupAssetTypeNames.SceneAsset))
            {
                diagnostics.Add(
                    new EchoLaunchSetupDiagnostic(
                        EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                        EchoLaunchSetupDiagnosticSeverity.Blocker,
                        "The selected destination scene does not exist " +
                        "as a project SceneAsset.",
                        normalized));

                operations.Add(
                    new EchoLaunchSetupOperation(
                        "destination.scene",
                        ValidatePhase,
                        EchoLaunchSetupOperationKind.ValidateDestinationScene,
                        EchoLaunchSetupOperationDisposition.Conflict,
                        normalized,
                        "A valid existing destination scene is required.",
                        EchoLaunchSetupDiagnosticCodes.InvalidRequest));

                return;
            }

            operations.Add(
                new EchoLaunchSetupOperation(
                    "destination.scene",
                    ValidatePhase,
                    EchoLaunchSetupOperationKind.ValidateDestinationScene,
                    EchoLaunchSetupOperationDisposition.Reuse,
                    normalized,
                    "The selected destination scene already exists."));
        }

        private static void ValidatePackageTemplate(
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            if (snapshot.PackageRootTemplateAvailable)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        "package.root-template",
                        ValidatePhase,
                        EchoLaunchSetupOperationKind.ValidatePackageTemplate,
                        EchoLaunchSetupOperationDisposition.NoChange,
                        EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                        "The package root prefab template is available."));
                return;
            }

            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.PackagePrerequisiteMissing,
                    EchoLaunchSetupDiagnosticSeverity.Blocker,
                    "The package root prefab template is unavailable.",
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath));

            operations.Add(
                new EchoLaunchSetupOperation(
                    "package.root-template",
                    ValidatePhase,
                    EchoLaunchSetupOperationKind.ValidatePackageTemplate,
                    EchoLaunchSetupOperationDisposition.Unsupported,
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                    "Repair or reinstall the package before setup.",
                    EchoLaunchSetupDiagnosticCodes.PackagePrerequisiteMissing));
        }

        private static void ResolveFolder(
            string path,
            string key,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            EchoLaunchProjectAssetFact fact = snapshot.FindAssetFact(path);

            if (!fact.Exists)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        key,
                        FolderPhase,
                        EchoLaunchSetupOperationKind.EnsureFolder,
                        EchoLaunchSetupOperationDisposition.Create,
                        path,
                        "The project-owned setup folder is missing " +
                        "and would be created."));
                return;
            }

            if (fact.IsFolder)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        key,
                        FolderPhase,
                        EchoLaunchSetupOperationKind.EnsureFolder,
                        EchoLaunchSetupOperationDisposition.NoChange,
                        path,
                        "The project-owned setup folder already exists."));
                return;
            }

            AddConflict(
                key,
                EchoLaunchSetupOperationKind.EnsureFolder,
                path,
                "A non-folder asset already occupies the required folder path.",
                FolderPhase,
                operations,
                diagnostics);
        }

        private static void ResolveAsset(
            EchoLaunchSetupAssetRole role,
            EchoLaunchSetupOperationKind kind,
            string key,
            string targetPath,
            string expectedTypeName,
            EchoLaunchSetupRequest request,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics,
            bool validateConfigurationSchema)
        {
            int phase =
                role == EchoLaunchSetupAssetRole.RootPrefab
                    ? PrefabPhase
                    : DefinitionPhase;

            EchoLaunchProjectAssetFact target =
                snapshot.FindAssetFact(targetPath);

            if (target.Exists)
            {
                if (!target.IsType(expectedTypeName))
                {
                    AddConflict(
                        key,
                        kind,
                        targetPath,
                        "An incompatible asset already occupies the " +
                        "requested target.",
                        phase,
                        operations,
                        diagnostics);
                    return;
                }

                if (HasUnsupportedSchema(
                        target,
                        validateConfigurationSchema))
                {
                    AddUnsupportedSchema(
                        key,
                        kind,
                        target,
                        phase,
                        operations,
                        diagnostics);
                    return;
                }

                AddReuse(
                    key,
                    kind,
                    target.Path,
                    "The compatible target asset already exists.",
                    phase,
                    operations,
                    diagnostics);
                return;
            }

            string selectedPath = request.GetSelectedPath(role);

            if (!string.IsNullOrWhiteSpace(selectedPath))
            {
                EchoLaunchProjectAssetFact selected =
                    snapshot.FindAssetFact(selectedPath);

                if (!selected.Exists ||
                    !selected.IsType(expectedTypeName))
                {
                    AddConflict(
                        key,
                        kind,
                        selectedPath,
                        "The explicitly selected asset is missing " +
                        "or incompatible.",
                        phase,
                        operations,
                        diagnostics);
                    return;
                }

                if (HasUnsupportedSchema(
                        selected,
                        validateConfigurationSchema))
                {
                    AddUnsupportedSchema(
                        key,
                        kind,
                        selected,
                        phase,
                        operations,
                        diagnostics);
                    return;
                }

                AddReuse(
                    key,
                    kind,
                    selected.Path,
                    "The explicitly selected compatible asset would be reused.",
                    phase,
                    operations,
                    diagnostics);
                return;
            }

            IReadOnlyList<EchoLaunchProjectAssetFact> candidates =
                snapshot.GetCandidates(role);

            if (candidates.Count == 1)
            {
                EchoLaunchProjectAssetFact candidate = candidates[0];

                if (HasUnsupportedSchema(
                        candidate,
                        validateConfigurationSchema))
                {
                    AddUnsupportedSchema(
                        key,
                        kind,
                        candidate,
                        phase,
                        operations,
                        diagnostics);
                    return;
                }

                AddReuse(
                    key,
                    kind,
                    candidate.Path,
                    "One compatible existing project asset was found " +
                    "and would be reused.",
                    phase,
                    operations,
                    diagnostics);
                return;
            }

            if (candidates.Count > 1)
            {
                diagnostics.Add(
                    new EchoLaunchSetupDiagnostic(
                        EchoLaunchSetupDiagnosticCodes.AmbiguousCandidates,
                        EchoLaunchSetupDiagnosticSeverity.Blocker,
                        "Multiple compatible project assets exist. " +
                        "Select the intended asset explicitly.",
                        targetPath));

                operations.Add(
                    new EchoLaunchSetupOperation(
                        key,
                        phase,
                        kind,
                        EchoLaunchSetupOperationDisposition.ManualDecision,
                        targetPath,
                        "Setup cannot choose among " +
                        candidates.Count +
                        " compatible candidates.",
                        EchoLaunchSetupDiagnosticCodes.AmbiguousCandidates));
                return;
            }

            operations.Add(
                new EchoLaunchSetupOperation(
                    key,
                    phase,
                    kind,
                    EchoLaunchSetupOperationDisposition.Create,
                    targetPath,
                    "The project-owned asset is missing and would be created."));
        }

        private static bool HasUnsupportedSchema(
            EchoLaunchProjectAssetFact fact,
            bool validate)
        {
            return validate &&
                   fact.ConfigurationSchemaVersion.HasValue &&
                   fact.ConfigurationSchemaVersion.Value !=
                   EchoLaunchConfiguration.CurrentSchemaVersion;
        }

        private static void ResolveBootScene(
            EchoLaunchSetupPathSet paths,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            EchoLaunchProjectAssetFact fact =
                snapshot.FindAssetFact(paths.BootScenePath);

            if (!fact.Exists)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        "scene.boot",
                        ScenePhase,
                        EchoLaunchSetupOperationKind.ResolveBootScene,
                        EchoLaunchSetupOperationDisposition.Create,
                        paths.BootScenePath,
                        "The project-owned Boot scene is missing " +
                        "and would be created."));
                return;
            }

            if (fact.IsType(EchoLaunchSetupAssetTypeNames.SceneAsset))
            {
                AddReuse(
                    "scene.boot",
                    EchoLaunchSetupOperationKind.ResolveBootScene,
                    paths.BootScenePath,
                    "The project-owned Boot scene already exists.",
                    ScenePhase,
                    operations,
                    diagnostics);
                return;
            }

            AddConflict(
                "scene.boot",
                EchoLaunchSetupOperationKind.ResolveBootScene,
                paths.BootScenePath,
                "An incompatible asset occupies the requested Boot scene path.",
                ScenePhase,
                operations,
                diagnostics);
        }

        private static void ResolveBuildSettings(
            EchoLaunchSetupRequest request,
            EchoLaunchSetupPathSet paths,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            int existingIndex =
                snapshot.FindBuildSettingsIndex(paths.BootScenePath);

            if (request.BuildSettingsPolicy ==
                EchoLaunchBuildSettingsPolicy.DoNotChange)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        "build-settings.boot",
                        BuildSettingsPhase,
                        EchoLaunchSetupOperationKind.ResolveBuildSettings,
                        EchoLaunchSetupOperationDisposition.NoChange,
                        paths.BootScenePath,
                        "The request explicitly leaves Build Settings unchanged."));
                return;
            }

            if (request.BuildSettingsPolicy ==
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        "build-settings.boot",
                        BuildSettingsPhase,
                        EchoLaunchSetupOperationKind.ResolveBuildSettings,
                        existingIndex >= 0
                            ? EchoLaunchSetupOperationDisposition.NoChange
                            : EchoLaunchSetupOperationDisposition.Create,
                        paths.BootScenePath,
                        existingIndex >= 0
                            ? "The Boot scene already exists in Build Settings " +
                              "at index " + existingIndex + "."
                            : "The Boot scene would be appended after all " +
                              "existing Build Settings scenes."));
                return;
            }

            if (existingIndex == 0)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        "build-settings.boot",
                        BuildSettingsPhase,
                        EchoLaunchSetupOperationKind.ResolveBuildSettings,
                        EchoLaunchSetupOperationDisposition.NoChange,
                        paths.BootScenePath,
                        "The Boot scene is already first in Build Settings."));
                return;
            }

            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.BuildSettingsApproval,
                    EchoLaunchSetupDiagnosticSeverity.Warning,
                    "Moving the Boot scene to index zero requires explicit " +
                    "approval. Unrelated scene order must remain unchanged.",
                    paths.BootScenePath));

            operations.Add(
                new EchoLaunchSetupOperation(
                    "build-settings.boot",
                    BuildSettingsPhase,
                    EchoLaunchSetupOperationKind.ResolveBuildSettings,
                    EchoLaunchSetupOperationDisposition.ManualDecision,
                    paths.BootScenePath,
                    existingIndex < 0
                        ? "The Boot scene would be inserted at index zero " +
                          "after approval."
                        : "The Boot scene would move from index " +
                          existingIndex +
                          " to index zero after approval.",
                    EchoLaunchSetupDiagnosticCodes.BuildSettingsApproval,
                    true));
        }

        private static void AddInvalidRequest(
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics,
            string key,
            string targetPath,
            string message)
        {
            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    EchoLaunchSetupDiagnosticSeverity.Blocker,
                    message,
                    targetPath));

            operations.Add(
                new EchoLaunchSetupOperation(
                    key,
                    ValidatePhase,
                    EchoLaunchSetupOperationKind.ValidateRequest,
                    EchoLaunchSetupOperationDisposition.Conflict,
                    targetPath,
                    message,
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest));
        }

        private static void AddConflict(
            string key,
            EchoLaunchSetupOperationKind kind,
            string path,
            string message,
            int phase,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.IncompatibleTarget,
                    EchoLaunchSetupDiagnosticSeverity.Blocker,
                    message,
                    path));

            operations.Add(
                new EchoLaunchSetupOperation(
                    key,
                    phase,
                    kind,
                    EchoLaunchSetupOperationDisposition.Conflict,
                    path,
                    message,
                    EchoLaunchSetupDiagnosticCodes.IncompatibleTarget));
        }

        private static void AddUnsupportedSchema(
            string key,
            EchoLaunchSetupOperationKind kind,
            EchoLaunchProjectAssetFact fact,
            int phase,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            string message =
                "The existing configuration schema " +
                (fact.ConfigurationSchemaVersion.HasValue
                    ? fact.ConfigurationSchemaVersion.Value.ToString()
                    : "is unknown") +
                " requires a separately approved migration.";

            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.UnsupportedMigration,
                    EchoLaunchSetupDiagnosticSeverity.Blocker,
                    message,
                    fact.Path));

            operations.Add(
                new EchoLaunchSetupOperation(
                    key,
                    phase,
                    kind,
                    EchoLaunchSetupOperationDisposition.Unsupported,
                    fact.Path,
                    message,
                    EchoLaunchSetupDiagnosticCodes.UnsupportedMigration));
        }

        private static void AddReuse(
            string key,
            EchoLaunchSetupOperationKind kind,
            string path,
            string reason,
            int phase,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.CompatibleAssetReused,
                    EchoLaunchSetupDiagnosticSeverity.Information,
                    reason,
                    path));

            operations.Add(
                new EchoLaunchSetupOperation(
                    key,
                    phase,
                    kind,
                    EchoLaunchSetupOperationDisposition.Reuse,
                    path,
                    reason,
                    EchoLaunchSetupDiagnosticCodes.CompatibleAssetReused));
        }

        private static EchoLaunchSetupPlanStatus DetermineStatus(
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            bool hasWarning = false;

            for (int index = 0; index < diagnostics.Count; index++)
            {
                if (diagnostics[index].Severity ==
                    EchoLaunchSetupDiagnosticSeverity.Blocker)
                {
                    return EchoLaunchSetupPlanStatus.Blocked;
                }

                if (diagnostics[index].Severity ==
                    EchoLaunchSetupDiagnosticSeverity.Warning)
                {
                    hasWarning = true;
                }
            }

            for (int index = 0; index < operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = operations[index];

                if (operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.Conflict ||
                    operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.Unsupported)
                {
                    return EchoLaunchSetupPlanStatus.Blocked;
                }

                if (operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.ManualDecision &&
                    operation.Kind !=
                        EchoLaunchSetupOperationKind.ResolveBuildSettings)
                {
                    return EchoLaunchSetupPlanStatus.Blocked;
                }

                if (operation.RequiresExplicitApproval)
                {
                    hasWarning = true;
                }
            }

            return hasWarning
                ? EchoLaunchSetupPlanStatus.ReadyWithWarnings
                : EchoLaunchSetupPlanStatus.Ready;
        }

        private static void SortOperations(
            List<EchoLaunchSetupOperation> operations)
        {
            operations.Sort(
                delegate(
                    EchoLaunchSetupOperation left,
                    EchoLaunchSetupOperation right)
                {
                    int phaseComparison = left.Phase.CompareTo(right.Phase);

                    if (phaseComparison != 0)
                    {
                        return phaseComparison;
                    }

                    int pathComparison = string.Compare(
                        left.TargetPath,
                        right.TargetPath,
                        StringComparison.Ordinal);

                    return pathComparison != 0
                        ? pathComparison
                        : string.Compare(
                            left.Key,
                            right.Key,
                            StringComparison.Ordinal);
                });
        }

        private static void SortDiagnostics(
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            diagnostics.Sort(
                delegate(
                    EchoLaunchSetupDiagnostic left,
                    EchoLaunchSetupDiagnostic right)
                {
                    int severityComparison =
                        right.Severity.CompareTo(left.Severity);

                    if (severityComparison != 0)
                    {
                        return severityComparison;
                    }

                    int codeComparison = string.Compare(
                        left.Code,
                        right.Code,
                        StringComparison.Ordinal);

                    return codeComparison != 0
                        ? codeComparison
                        : string.Compare(
                            left.TargetPath,
                            right.TargetPath,
                            StringComparison.Ordinal);
                });
        }
    }
}
