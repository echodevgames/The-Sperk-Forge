
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

                ReconcileExistingTargets(
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
            ResolveBootBuildSettings(
                request,
                paths,
                snapshot,
                operations,
                diagnostics);

            if (!EchoLaunchSetupPathUtility.TryNormalizeProjectAssetPath(
                    request.DestinationScenePath,
                    ".unity",
                    out string destinationScenePath,
                    out _))
            {
                return;
            }

            ResolveDestinationBuildSettings(
                request,
                destinationScenePath,
                snapshot,
                operations,
                diagnostics);
        }

        private static void ResolveBootBuildSettings(
            EchoLaunchSetupRequest request,
            EchoLaunchSetupPathSet paths,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            int count =
                snapshot.CountBuildSettingsEntries(
                    paths.BootScenePath);

            EchoLaunchBuildSettingsSceneFact first =
                snapshot.FindFirstBuildSettingsFact(
                    paths.BootScenePath);

            if (request.BuildSettingsPolicy ==
                EchoLaunchBuildSettingsPolicy.DoNotChange)
            {
                AddDoNotChangeBuildSettingsRequirement(
                    "build-settings.boot",
                    EchoLaunchSetupOperationKind.ResolveBuildSettings,
                    paths.BootScenePath,
                    "Boot",
                    count,
                    first,
                    operations,
                    diagnostics);
                return;
            }

            if (request.BuildSettingsPolicy ==
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd)
            {
                ResolveAppendRequiredBuildSettingsScene(
                    "build-settings.boot",
                    EchoLaunchSetupOperationKind.ResolveBuildSettings,
                    paths.BootScenePath,
                    "Boot",
                    count,
                    first,
                    operations,
                    diagnostics);
                return;
            }

            bool alreadyCanonical =
                count == 1 &&
                first != null &&
                first.Index == 0 &&
                first.Enabled;

            if (alreadyCanonical)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        "build-settings.boot",
                        BuildSettingsPhase,
                        EchoLaunchSetupOperationKind.ResolveBuildSettings,
                        EchoLaunchSetupOperationDisposition.NoChange,
                        paths.BootScenePath,
                        "One enabled Boot scene entry is already first in Build Settings."));
                return;
            }

            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.BuildSettingsApproval,
                    EchoLaunchSetupDiagnosticSeverity.Warning,
                    "Normalizing one enabled Boot scene entry at index zero requires explicit approval. Unrelated order and enabled states will be preserved.",
                    paths.BootScenePath));

            operations.Add(
                new EchoLaunchSetupOperation(
                    "build-settings.boot",
                    BuildSettingsPhase,
                    EchoLaunchSetupOperationKind.ResolveBuildSettings,
                    EchoLaunchSetupOperationDisposition.ManualDecision,
                    paths.BootScenePath,
                    "Build Settings can be normalized to one enabled Boot entry at index zero after approval.",
                    EchoLaunchSetupDiagnosticCodes.BuildSettingsApproval,
                    true,
                    count == 0
                        ? "Boot entry missing"
                        : count + " Boot entry/entries; first index " + first.Index,
                    "One enabled Boot entry at index zero",
                    "The exact Boot scene path and unrelated scene order are known."));
        }

        private static void ResolveDestinationBuildSettings(
            EchoLaunchSetupRequest request,
            string destinationScenePath,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            int count =
                snapshot.CountBuildSettingsEntries(
                    destinationScenePath);

            EchoLaunchBuildSettingsSceneFact first =
                snapshot.FindFirstBuildSettingsFact(
                    destinationScenePath);

            if (request.BuildSettingsPolicy ==
                EchoLaunchBuildSettingsPolicy.DoNotChange)
            {
                AddDoNotChangeBuildSettingsRequirement(
                    "build-settings.destination",
                    EchoLaunchSetupOperationKind
                        .ResolveDestinationBuildSettings,
                    destinationScenePath,
                    "destination",
                    count,
                    first,
                    operations,
                    diagnostics);
                return;
            }

            ResolveAppendRequiredBuildSettingsScene(
                "build-settings.destination",
                EchoLaunchSetupOperationKind
                    .ResolveDestinationBuildSettings,
                destinationScenePath,
                "destination",
                count,
                first,
                operations,
                diagnostics);
        }

        private static void ResolveAppendRequiredBuildSettingsScene(
            string key,
            EchoLaunchSetupOperationKind kind,
            string targetPath,
            string sceneLabel,
            int count,
            EchoLaunchBuildSettingsSceneFact first,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            if (count == 0)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        key,
                        BuildSettingsPhase,
                        kind,
                        EchoLaunchSetupOperationDisposition.Create,
                        targetPath,
                        "The " + sceneLabel +
                        " scene would be appended after all existing Build Settings scenes."));
                return;
            }

            if (count == 1 &&
                first != null &&
                first.Enabled)
            {
                operations.Add(
                    new EchoLaunchSetupOperation(
                        key,
                        BuildSettingsPhase,
                        kind,
                        EchoLaunchSetupOperationDisposition.NoChange,
                        targetPath,
                        "One enabled " + sceneLabel +
                        " scene entry already exists at index " +
                        first.Index + "."));
                return;
            }

            if (count == 1 &&
                first != null)
            {
                AddRepair(
                    key,
                    kind,
                    targetPath,
                    "The unique " + sceneLabel +
                    " scene entry is disabled and can be enabled without changing unrelated entries.",
                    BuildSettingsPhase,
                    "Index " + first.Index + ": disabled",
                    "Index " + first.Index + ": enabled",
                    "One exact-path Build Settings entry exists.",
                    false,
                    operations,
                    diagnostics);
                return;
            }

            AddRepairConflict(
                key,
                kind,
                targetPath,
                "Multiple " + sceneLabel +
                " scene entries are ambiguous under the append policy.",
                BuildSettingsPhase,
                operations,
                diagnostics);
        }

        private static void AddDoNotChangeBuildSettingsRequirement(
            string key,
            EchoLaunchSetupOperationKind kind,
            string targetPath,
            string sceneLabel,
            int count,
            EchoLaunchBuildSettingsSceneFact first,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            bool satisfied =
                count == 1 &&
                first != null &&
                first.Enabled;

            operations.Add(
                new EchoLaunchSetupOperation(
                    key,
                    BuildSettingsPhase,
                    kind,
                    EchoLaunchSetupOperationDisposition.NoChange,
                    targetPath,
                    satisfied
                        ? "The request leaves Build Settings unchanged and one enabled " +
                          sceneLabel + " scene entry already exists."
                        : "The request leaves Build Settings unchanged, but the required " +
                          sceneLabel + " scene entry is not currently canonical.",
                    satisfied
                        ? string.Empty
                        : EchoLaunchSetupDiagnosticCodes.InvalidRequest));

            if (satisfied)
            {
                return;
            }

            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    EchoLaunchSetupDiagnosticSeverity.Blocker,
                    "Build Settings policy is Do Not Change, but First Light requires exactly one enabled " +
                    sceneLabel + " scene entry before Setup can succeed.",
                    targetPath));
        }

        private static void ReconcileExistingTargets(
            EchoLaunchSetupRequest request,
            EchoLaunchSetupPathSet paths,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            ReconcileConfiguration(
                request,
                paths,
                snapshot,
                operations,
                diagnostics);

            ReconcileDestination(
                request,
                paths,
                snapshot,
                operations,
                diagnostics);

            ReconcileRootPrefab(
                paths,
                snapshot,
                operations,
                diagnostics);

            ReconcileBootScene(
                paths,
                snapshot,
                operations,
                diagnostics);

            ReconcileBuildSettingsDecision(operations);
        }

        private static void ReconcileConfiguration(
            EchoLaunchSetupRequest request,
            EchoLaunchSetupPathSet paths,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            int index = FindOperationIndex(
                operations,
                EchoLaunchSetupOperationKind.ResolveConfiguration);
            if (index < 0)
            {
                return;
            }

            EchoLaunchSetupOperation operation = operations[index];
            if (operation.Disposition != EchoLaunchSetupOperationDisposition.Reuse ||
                !string.Equals(operation.TargetPath, paths.ConfigurationAssetPath, StringComparison.Ordinal))
            {
                return;
            }

            EchoLaunchProjectAssetFact fact =
                snapshot.FindAssetFact(operation.TargetPath);
            if (!fact.HasRepairEvidence)
            {
                return;
            }

            if (!IsCanonicalStableId(fact.StableId))
            {
                ReplaceWithRepairConflict(
                    index,
                    operation,
                    "The existing configuration has no valid stable ID.",
                    operations,
                    diagnostics);
                return;
            }

            string desiredSequence = ResolveOperationPath(
                operations,
                EchoLaunchSetupOperationKind.ResolveStartupSequence);
            string desiredDestination = ResolveOperationPath(
                operations,
                EchoLaunchSetupOperationKind.ResolveLaunchDestination);
            string desiredSplash = request.CreateSplashSequence
                ? ResolveOperationPath(
                    operations,
                    EchoLaunchSetupOperationKind.ResolveSplashSequence)
                : string.Empty;

            if (PathEquals(fact.StartupSequencePath, desiredSequence) &&
                PathEquals(fact.LaunchDestinationPath, desiredDestination) &&
                PathEquals(fact.SplashSequencePath, desiredSplash))
            {
                return;
            }

            RemoveCompatibleReuseDiagnostic(
                diagnostics,
                operation.TargetPath);
            operations[index] = CreateRepairOperation(
                operation,
                "The current-schema configuration references can be rebound without changing identity or unrelated settings.",
                "Sequence=" + fact.StartupSequencePath +
                "; Destination=" + fact.LaunchDestinationPath +
                "; Splash=" + NullLabel(fact.SplashSequencePath),
                "Sequence=" + desiredSequence +
                "; Destination=" + desiredDestination +
                "; Splash=" + NullLabel(desiredSplash),
                "Exact type, current schema, nonempty stable ID, and unique resolved dependencies were proven.");
        }

        private static void ReconcileDestination(
            EchoLaunchSetupRequest request,
            EchoLaunchSetupPathSet paths,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            int index = FindOperationIndex(
                operations,
                EchoLaunchSetupOperationKind.ResolveLaunchDestination);
            if (index < 0)
            {
                return;
            }

            EchoLaunchSetupOperation operation = operations[index];
            if (operation.Disposition != EchoLaunchSetupOperationDisposition.Reuse ||
                !string.Equals(operation.TargetPath, paths.LaunchDestinationAssetPath, StringComparison.Ordinal))
            {
                return;
            }

            EchoLaunchProjectAssetFact fact =
                snapshot.FindAssetFact(operation.TargetPath);
            if (!fact.HasRepairEvidence)
            {
                return;
            }

            if (fact.ConfigurationSchemaVersion.HasValue &&
                fact.ConfigurationSchemaVersion.Value !=
                    LaunchDestination.CurrentSchemaVersion)
            {
                ReplaceWithUnsupportedMigration(
                    index,
                    operation,
                    fact.ConfigurationSchemaVersion.Value,
                    operations,
                    diagnostics);
                return;
            }

            if (!IsCanonicalStableId(fact.StableId))
            {
                ReplaceWithRepairConflict(
                    index,
                    operation,
                    "The existing launch destination has no valid stable ID.",
                    operations,
                    diagnostics);
                return;
            }

            string desiredPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    request.DestinationScenePath);
            string desiredLabel = string.IsNullOrWhiteSpace(fact.DestinationDisplayName)
                ? System.IO.Path.GetFileNameWithoutExtension(desiredPath)
                : fact.DestinationDisplayName;

            if (PathEquals(fact.DestinationScenePath, desiredPath) &&
                string.Equals(
                    fact.DestinationDisplayName,
                    desiredLabel,
                    StringComparison.Ordinal))
            {
                return;
            }

            RemoveCompatibleReuseDiagnostic(
                diagnostics,
                operation.TargetPath);
            operations[index] = CreateRepairOperation(
                operation,
                "The current-schema destination can reconcile its scene path and fill an empty label while preserving identity.",
                "Scene=" + fact.DestinationScenePath +
                "; Label=" + fact.DestinationDisplayName,
                "Scene=" + desiredPath +
                "; Label=" + desiredLabel,
                "Exact type, current schema, nonempty stable ID, and selected existing scene were proven.");
        }

        private static void ReconcileRootPrefab(
            EchoLaunchSetupPathSet paths,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            int index = FindOperationIndex(
                operations,
                EchoLaunchSetupOperationKind.ResolveRootPrefabVariant);
            if (index < 0)
            {
                return;
            }

            EchoLaunchSetupOperation operation = operations[index];
            if (operation.Disposition != EchoLaunchSetupOperationDisposition.Reuse ||
                !string.Equals(operation.TargetPath, paths.RootPrefabPath, StringComparison.Ordinal))
            {
                return;
            }

            EchoLaunchProjectAssetFact fact =
                snapshot.FindAssetFact(operation.TargetPath);
            if (!fact.HasRepairEvidence)
            {
                return;
            }

            bool proven =
                string.Equals(fact.PrefabAssetType, "Variant", StringComparison.Ordinal) &&
                fact.PrefabLineageMatchesTemplate &&
                fact.EchoLaunchRootCount == 1;

            if (!proven)
            {
                ReplaceWithRepairConflict(
                    index,
                    operation,
                    "The root prefab is not a proven one-root variant of the package template.",
                    operations,
                    diagnostics);
                return;
            }

            string desiredConfiguration = ResolveOperationPath(
                operations,
                EchoLaunchSetupOperationKind.ResolveConfiguration);
            if (PathEquals(fact.RootConfigurationPath, desiredConfiguration))
            {
                return;
            }

            RemoveCompatibleReuseDiagnostic(
                diagnostics,
                operation.TargetPath);
            operations[index] = CreateRepairOperation(
                operation,
                "The verified root variant can rebind only EchoLaunchRoot.configuration.",
                "Configuration=" + fact.RootConfigurationPath,
                "Configuration=" + desiredConfiguration,
                "Variant lineage reaches the package template and exactly one EchoLaunchRoot exists.");
        }

        private static void ReconcileBootScene(
            EchoLaunchSetupPathSet paths,
            EchoLaunchProjectSnapshot snapshot,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            int index = FindOperationIndex(
                operations,
                EchoLaunchSetupOperationKind.ResolveBootScene);
            if (index < 0)
            {
                return;
            }

            EchoLaunchSetupOperation operation = operations[index];
            if (operation.Disposition != EchoLaunchSetupOperationDisposition.Reuse)
            {
                return;
            }

            EchoLaunchProjectAssetFact fact =
                snapshot.FindAssetFact(paths.BootScenePath);
            if (!fact.SceneInspectionSafe)
            {
                ReplaceWithRepairConflict(
                    index,
                    operation,
                    string.IsNullOrEmpty(fact.SceneInspectionMessage)
                        ? "The Boot scene could not be inspected safely."
                        : fact.SceneInspectionMessage,
                    operations,
                    diagnostics);
                return;
            }

            if (!fact.HasRepairEvidence || !fact.EchoLaunchRootCount.HasValue)
            {
                return;
            }

            if (fact.EchoLaunchRootCount.Value == 0)
            {
                if (fact.SceneWasOpen)
                {
                    ReplaceWithRepairConflict(
                        index,
                        operation,
                        "Close the canonical Boot scene before adding its missing launch root so rollback can restore exact scene bytes safely.",
                        operations,
                        diagnostics);
                    return;
                }

                RemoveCompatibleReuseDiagnostic(
                    diagnostics,
                    operation.TargetPath);
                string desiredRoot = ResolveOperationPath(
                    operations,
                    EchoLaunchSetupOperationKind.ResolveRootPrefabVariant);
                operations[index] = CreateRepairOperation(
                    operation,
                    "The exact canonical Boot scene contains zero launch roots; one verified project-root prefab instance can be added.",
                    "EchoLaunchRoot count=0",
                    "EchoLaunchRoot count=1; source=" + desiredRoot,
                    "The scene loaded safely and no existing EchoLaunchRoot was found.");
                return;
            }

            if (fact.EchoLaunchRootCount.Value != 1)
            {
                ReplaceWithRepairConflict(
                    index,
                    operation,
                    "The Boot scene contains multiple EchoLaunchRoot components.",
                    operations,
                    diagnostics);
                return;
            }

            string expectedRoot = ResolveOperationPath(
                operations,
                EchoLaunchSetupOperationKind.ResolveRootPrefabVariant);
            if (!PathEquals(fact.PrefabSourcePath, expectedRoot))
            {
                ReplaceWithRepairConflict(
                    index,
                    operation,
                    "The Boot scene root is unpacked or comes from a different prefab.",
                    operations,
                    diagnostics);
            }
        }

        private static void ReconcileBuildSettingsDecision(
            List<EchoLaunchSetupOperation> operations)
        {
            int buildIndex = FindOperationIndex(
                operations,
                EchoLaunchSetupOperationKind.ResolveBuildSettings);
            if (buildIndex < 0 ||
                operations[buildIndex].Disposition !=
                    EchoLaunchSetupOperationDisposition.ManualDecision ||
                !HasExistingFoundationEvidence(operations))
            {
                return;
            }

            EchoLaunchSetupOperation source = operations[buildIndex];
            operations[buildIndex] = new EchoLaunchSetupOperation(
                source.Key,
                source.Phase,
                source.Kind,
                EchoLaunchSetupOperationDisposition.Repair,
                source.TargetPath,
                source.Reason,
                source.DiagnosticCode,
                true,
                source.ExistingState,
                source.ProposedState,
                source.ProofSummary);
        }

        private static bool HasExistingFoundationEvidence(
            List<EchoLaunchSetupOperation> operations)
        {
            for (int index = 0; index < operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = operations[index];
                bool coreArtifact =
                    operation.Kind == EchoLaunchSetupOperationKind.ResolveConfiguration ||
                    operation.Kind == EchoLaunchSetupOperationKind.ResolveStartupSequence ||
                    operation.Kind == EchoLaunchSetupOperationKind.ResolveLaunchDestination ||
                    operation.Kind == EchoLaunchSetupOperationKind.ResolveSplashSequence ||
                    operation.Kind == EchoLaunchSetupOperationKind.ResolveRootPrefabVariant ||
                    operation.Kind == EchoLaunchSetupOperationKind.ResolveBootScene;
                if (coreArtifact &&
                    (operation.Disposition ==
                         EchoLaunchSetupOperationDisposition.Reuse ||
                     operation.Disposition ==
                         EchoLaunchSetupOperationDisposition.Repair))
                {
                    return true;
                }
            }

            return false;
        }

        private static EchoLaunchSetupOperation CreateRepairOperation(
            EchoLaunchSetupOperation source,
            string reason,
            string existingState,
            string proposedState,
            string proofSummary)
        {
            return new EchoLaunchSetupOperation(
                source.Key,
                source.Phase,
                source.Kind,
                EchoLaunchSetupOperationDisposition.Repair,
                source.TargetPath,
                reason,
                EchoLaunchSetupDiagnosticCodes.RepairApprovalRequired,
                source.RequiresExplicitApproval,
                existingState,
                proposedState,
                proofSummary);
        }

        private static void AddRepair(
            string key,
            EchoLaunchSetupOperationKind kind,
            string path,
            string reason,
            int phase,
            string existingState,
            string proposedState,
            string proofSummary,
            bool requiresExplicitApproval,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            operations.Add(
                new EchoLaunchSetupOperation(
                    key,
                    phase,
                    kind,
                    EchoLaunchSetupOperationDisposition.Repair,
                    path,
                    reason,
                    EchoLaunchSetupDiagnosticCodes.RepairApprovalRequired,
                    requiresExplicitApproval,
                    existingState,
                    proposedState,
                    proofSummary));
        }

        private static void ReplaceWithRepairConflict(
            int index,
            EchoLaunchSetupOperation source,
            string message,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            RemoveCompatibleReuseDiagnostic(
                diagnostics,
                source.TargetPath);
            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.AmbiguousRepairEvidence,
                    EchoLaunchSetupDiagnosticSeverity.Blocker,
                    message,
                    source.TargetPath));

            operations[index] =
                new EchoLaunchSetupOperation(
                    source.Key,
                    source.Phase,
                    source.Kind,
                    EchoLaunchSetupOperationDisposition.Conflict,
                    source.TargetPath,
                    message,
                    EchoLaunchSetupDiagnosticCodes.AmbiguousRepairEvidence);
        }

        private static void RemoveCompatibleReuseDiagnostic(
            List<EchoLaunchSetupDiagnostic> diagnostics,
            string path)
        {
            for (int index = diagnostics.Count - 1; index >= 0; index--)
            {
                if (diagnostics[index].Code ==
                        EchoLaunchSetupDiagnosticCodes.CompatibleAssetReused &&
                    string.Equals(
                        diagnostics[index].TargetPath,
                        path,
                        StringComparison.Ordinal))
                {
                    diagnostics.RemoveAt(index);
                }
            }
        }

        private static void ReplaceWithUnsupportedMigration(
            int index,
            EchoLaunchSetupOperation source,
            int schemaVersion,
            List<EchoLaunchSetupOperation> operations,
            List<EchoLaunchSetupDiagnostic> diagnostics)
        {
            RemoveCompatibleReuseDiagnostic(
                diagnostics,
                source.TargetPath);
            string message =
                "The existing asset schema " + schemaVersion +
                " requires a separately approved migration.";
            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    EchoLaunchSetupDiagnosticCodes.UnsupportedMigration,
                    EchoLaunchSetupDiagnosticSeverity.Blocker,
                    message,
                    source.TargetPath));
            operations[index] =
                new EchoLaunchSetupOperation(
                    source.Key,
                    source.Phase,
                    source.Kind,
                    EchoLaunchSetupOperationDisposition.Unsupported,
                    source.TargetPath,
                    message,
                    EchoLaunchSetupDiagnosticCodes.UnsupportedMigration);
        }

        private static void AddRepairConflict(
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
                    EchoLaunchSetupDiagnosticCodes.AmbiguousRepairEvidence,
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
                    EchoLaunchSetupDiagnosticCodes.AmbiguousRepairEvidence));
        }

        private static int FindOperationIndex(
            List<EchoLaunchSetupOperation> operations,
            EchoLaunchSetupOperationKind kind)
        {
            for (int index = 0; index < operations.Count; index++)
            {
                if (operations[index].Kind == kind)
                {
                    return index;
                }
            }

            return -1;
        }

        private static string ResolveOperationPath(
            List<EchoLaunchSetupOperation> operations,
            EchoLaunchSetupOperationKind kind)
        {
            int index = FindOperationIndex(operations, kind);
            return index < 0 ? string.Empty : operations[index].TargetPath;
        }

        private static bool IsCanonicalStableId(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 32)
            {
                return false;
            }

            for (int index = 0; index < value.Length; index++)
            {
                char character = value[index];
                bool isDigit = character >= '0' && character <= '9';
                bool isLowerHex = character >= 'a' && character <= 'f';
                if (!isDigit && !isLowerHex)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PathEquals(string left, string right)
        {
            return string.Equals(
                EchoLaunchSetupPathUtility.NormalizeSeparators(left),
                EchoLaunchSetupPathUtility.NormalizeSeparators(right),
                StringComparison.Ordinal);
        }

        private static string NullLabel(string value)
        {
            return string.IsNullOrEmpty(value) ? "None" : value;
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
