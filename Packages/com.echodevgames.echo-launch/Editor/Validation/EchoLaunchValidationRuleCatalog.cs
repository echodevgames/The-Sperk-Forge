using System;
using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal static class EchoLaunchValidationRuleCatalog
    {
        internal static IReadOnlyList<EchoLaunchValidationFinding> Evaluate(
            EchoLaunchValidationEvidence evidence)
        {
            if (evidence == null)
            {
                throw new ArgumentNullException(nameof(evidence));
            }

            List<EchoLaunchValidationFinding> findings =
                new List<EchoLaunchValidationFinding>();

            ValidatePackageTemplate(evidence, findings);
            ValidateBootScene(evidence, findings);
            ValidateDuplicateRoots(evidence, findings);
            ValidateRootConfiguration(evidence, findings);
            ValidateConfiguration(evidence, findings);
            ValidateStartupSequence(evidence, findings);
            ValidateDuplicateIds(evidence, findings);
            ValidateDestination(evidence, findings);
            ValidateBootBuildSettings(evidence, findings);
            ValidatePresentation(evidence, findings);
            ValidateSplash(evidence, findings);
            ValidatePolicies(evidence, findings);
            ValidatePackageOwnedReferences(evidence, findings);
            ValidateCollectionIssues(evidence, findings);

            return findings;
        }

        private static void ValidatePackageTemplate(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            if (evidence.PackageTemplateAvailable)
            {
                return;
            }

            Add(
                findings,
                EchoLaunchValidationDiagnosticCodes.EvidenceUnavailable,
                EchoLaunchValidationSeverity.Blocker,
                "Package root template is unavailable",
                "The shipped First Light root prefab template could not be loaded.",
                EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                "PackageTemplate=Missing.",
                "Repair or reinstall the package before validating project-owned setup.");
        }

        private static void ValidateBootScene(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            EchoLaunchValidationSceneEvidence boot =
                evidence.FindScene(evidence.Paths.BootScenePath);

            if (!boot.Exists)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.MissingBootScene,
                    EchoLaunchValidationSeverity.Blocker,
                    "Canonical Boot scene is missing",
                    "The canonical First Light Boot scene does not exist as a valid Unity scene asset.",
                    evidence.Paths.BootScenePath,
                    "Expected one scene asset at the canonical Boot path.",
                    "Open First Light Setup and review the create-only plan.");
            }
        }

        private static void ValidateDuplicateRoots(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            int totalRoots = 0;

            for (int index = 0;
                 index < evidence.SceneEvidence.Count;
                 index++)
            {
                totalRoots += evidence.SceneEvidence[index].Roots.Count;
            }

            if (totalRoots > 1)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.DuplicateRoots,
                    EchoLaunchValidationSeverity.Blocker,
                    "Multiple effective launch roots found",
                    "More than one EchoLaunchRoot exists across the canonical Boot scene and enabled Build Settings scenes.",
                    evidence.Paths.BootScenePath,
                    "EffectiveRootCount=" + totalRoots + ".",
                    "Resolve duplicate roots manually. The Validator never deletes scene content.");
            }
        }

        private static void ValidateRootConfiguration(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            if (!evidence.RootPrefab.Exists ||
                evidence.RootPrefab.Roots.Count != 1)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.RootConfiguration,
                    EchoLaunchValidationSeverity.Blocker,
                    "Canonical root prefab is invalid",
                    "The canonical project root prefab must contain exactly one EchoLaunchRoot.",
                    evidence.Paths.RootPrefabPath,
                    "RootCount=" + evidence.RootPrefab.Roots.Count + ".",
                    "Open First Light Setup and review the repair plan.");
            }
            else
            {
                EchoLaunchValidationRootEvidence root =
                    evidence.RootPrefab.Roots[0];

                if (!evidence.RootPrefab.ReachesPackageTemplate ||
                    !string.Equals(
                        root.ConfigurationPath,
                        evidence.Paths.ConfigurationAssetPath,
                        StringComparison.Ordinal))
                {
                    Add(
                        findings,
                        EchoLaunchValidationDiagnosticCodes.RootConfiguration,
                        EchoLaunchValidationSeverity.Blocker,
                        "Canonical root prefab binding is invalid",
                        "The canonical root prefab must retain package-template lineage and bind the canonical project configuration.",
                        evidence.Paths.RootPrefabPath,
                        "Configuration=" + root.ConfigurationPath +
                        "; Lineage=" +
                        (evidence.RootPrefab.ReachesPackageTemplate
                            ? "Verified"
                            : "Unverified") +
                        ".",
                        "Open First Light Setup and review the explicit repair plan.");
                }
            }

            EchoLaunchValidationSceneEvidence boot =
                evidence.FindScene(evidence.Paths.BootScenePath);

            if (!boot.Exists ||
                !boot.Inspected)
            {
                return;
            }

            if (boot.Roots.Count != 1)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.RootConfiguration,
                    EchoLaunchValidationSeverity.Blocker,
                    "Boot scene launch root is missing or ambiguous",
                    "The canonical Boot scene must contain exactly one EchoLaunchRoot.",
                    evidence.Paths.BootScenePath,
                    "RootCount=" + boot.Roots.Count + ".",
                    "Open First Light Setup and review the explicit repair plan.");

                return;
            }

            EchoLaunchValidationRootEvidence sceneRoot = boot.Roots[0];

            if (!string.Equals(
                    sceneRoot.ConfigurationPath,
                    evidence.Paths.ConfigurationAssetPath,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    sceneRoot.PrefabSourcePath,
                    evidence.Paths.RootPrefabPath,
                    StringComparison.Ordinal))
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.RootConfiguration,
                    EchoLaunchValidationSeverity.Blocker,
                    "Boot scene launch root binding is invalid",
                    "The Boot scene root must come from the canonical project root prefab and bind its canonical configuration.",
                    evidence.Paths.BootScenePath,
                    "Configuration=" + sceneRoot.ConfigurationPath +
                    "; PrefabSource=" + sceneRoot.PrefabSourcePath + ".",
                    "Review the scene and First Light Setup repair evidence.");
            }
        }

        private static void ValidateConfiguration(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            EchoLaunchValidationAssetEvidence configuration =
                evidence.Configuration;

            if (!configuration.Exists ||
                !configuration.IsType<EchoLaunchConfiguration>() ||
                !IsCanonicalId(configuration.StableId) ||
                configuration.SchemaVersion !=
                    EchoLaunchConfiguration.CurrentSchemaVersion)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.Configuration,
                    EchoLaunchValidationSeverity.Blocker,
                    "Launch configuration is missing or unsupported",
                    "The canonical launch configuration must have the exact type, a canonical stable ID, and the current supported schema.",
                    configuration.Path,
                    "Type=" + configuration.TypeName +
                    "; Schema=" + configuration.SchemaVersion +
                    "; Identity=" +
                    (IsCanonicalId(configuration.StableId)
                        ? "Valid"
                        : "Invalid") +
                    ".",
                    "Use Setup only for approved reference repair. Historical schema changes require migration authority.");

                return;
            }

            if (!string.Equals(
                    evidence.ConfigurationStartupSequencePath,
                    evidence.Paths.StartupSequenceAssetPath,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    evidence.ConfigurationDestinationPath,
                    evidence.Paths.LaunchDestinationAssetPath,
                    StringComparison.Ordinal))
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.Configuration,
                    EchoLaunchValidationSeverity.Blocker,
                    "Launch configuration references are incomplete",
                    "The canonical configuration must reference the canonical startup sequence and launch destination.",
                    configuration.Path,
                    "Sequence=" + evidence.ConfigurationStartupSequencePath +
                    "; Destination=" + evidence.ConfigurationDestinationPath +
                    ".",
                    "Open First Light Setup and review the explicit repair plan.");
            }
        }

        private static void ValidateStartupSequence(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            EchoLaunchValidationAssetEvidence sequence =
                evidence.StartupSequence;

            if (!sequence.Exists ||
                !sequence.IsType<StartupSequence>() ||
                !IsCanonicalId(sequence.StableId) ||
                sequence.SchemaVersion != StartupSequence.CurrentSchemaVersion)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.StartupSequence,
                    EchoLaunchValidationSeverity.Error,
                    "Startup sequence is missing or invalid",
                    "The canonical startup sequence must have the exact type, a canonical stable ID, and the current schema.",
                    sequence.Path,
                    "Type=" + sequence.TypeName +
                    "; Schema=" + sequence.SchemaVersion + ".",
                    "Create or author a supported project-owned startup sequence.");

                return;
            }

            for (int index = 0;
                 index < evidence.SequenceEntries.Count;
                 index++)
            {
                EchoLaunchValidationSequenceEntryEvidence entry =
                    evidence.SequenceEntries[index];

                bool missingDefinition =
                    entry.Enabled &&
                    string.IsNullOrEmpty(entry.DefinitionPath);

                bool invalidEntry =
                    !IsCanonicalId(entry.EntryId);

                bool invalidDefinition =
                    !string.IsNullOrEmpty(entry.DefinitionPath) &&
                    (!IsCanonicalId(entry.DefinitionId) ||
                     entry.DefinitionSchema !=
                        StartupStepDefinition.CurrentSchemaVersion);

                if (missingDefinition ||
                    invalidEntry ||
                    invalidDefinition)
                {
                    Add(
                        findings,
                        EchoLaunchValidationDiagnosticCodes.StartupSequence,
                        EchoLaunchValidationSeverity.Error,
                        "Startup sequence entry is invalid",
                        "One authored startup entry or referenced step definition is incomplete or unsupported.",
                        sequence.Path,
                        "EntryIndex=" + entry.Index +
                        "; EntryId=" + entry.EntryId +
                        "; Definition=" + entry.DefinitionPath + ".",
                        "Correct the authored entry explicitly. The Validator does not delete or rewrite entries.");
                }
            }
        }

        private static void ValidateDuplicateIds(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            HashSet<string> entryIds =
                new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> definitionIds =
                new HashSet<string>(StringComparer.Ordinal);

            for (int index = 0;
                 index < evidence.SequenceEntries.Count;
                 index++)
            {
                EchoLaunchValidationSequenceEntryEvidence entry =
                    evidence.SequenceEntries[index];

                if (IsCanonicalId(entry.EntryId) &&
                    !entryIds.Add(entry.EntryId))
                {
                    Add(
                        findings,
                        EchoLaunchValidationDiagnosticCodes.DuplicateIds,
                        EchoLaunchValidationSeverity.Blocker,
                        "Duplicate startup entry ID",
                        "Two startup-sequence entries use the same stable identity.",
                        evidence.StartupSequence.Path,
                        "EntryId=" + entry.EntryId + ".",
                        "Correct the duplicated authored identity explicitly.");
                }

                if (IsCanonicalId(entry.DefinitionId) &&
                    !definitionIds.Add(entry.DefinitionId))
                {
                    Add(
                        findings,
                        EchoLaunchValidationDiagnosticCodes.DuplicateIds,
                        EchoLaunchValidationSeverity.Blocker,
                        "Duplicate startup step ID",
                        "Two referenced startup definitions use the same stable identity.",
                        entry.DefinitionPath,
                        "StepId=" + entry.DefinitionId + ".",
                        "Correct the duplicated authored identity explicitly.");
                }
            }
        }

        private static void ValidateDestination(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            EchoLaunchValidationAssetEvidence destination =
                evidence.Destination;

            int enabledCount =
                CountBuildScene(
                    evidence,
                    evidence.DestinationScenePath,
                    true);

            bool sceneExists =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    evidence.DestinationScenePath) != null;

            bool valid =
                destination.Exists &&
                destination.IsType<LaunchDestination>() &&
                IsCanonicalId(destination.StableId) &&
                destination.SchemaVersion ==
                    LaunchDestination.CurrentSchemaVersion &&
                IsProjectScenePath(evidence.DestinationScenePath) &&
                !string.IsNullOrWhiteSpace(
                    evidence.DestinationDisplayName) &&
                sceneExists &&
                enabledCount == 1;

            if (!valid)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.Destination,
                    EchoLaunchValidationSeverity.Blocker,
                    "Initial destination is not launch-ready",
                    "The configured destination must be a valid project scene and appear exactly once as enabled in Build Settings.",
                    destination.Path,
                    "Scene=" + evidence.DestinationScenePath +
                    "; EnabledCount=" + enabledCount +
                    "; SceneAsset=" + (sceneExists ? "Present" : "Missing") +
                    ".",
                    "Correct the destination asset or Build Settings explicitly.");
            }
        }

        private static void ValidateBootBuildSettings(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            int allCount =
                CountBuildScene(
                    evidence,
                    evidence.Paths.BootScenePath,
                    null);

            int enabledCount =
                CountBuildScene(
                    evidence,
                    evidence.Paths.BootScenePath,
                    true);

            if (allCount != 1 ||
                enabledCount != 1)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.BootBuildSettings,
                    EchoLaunchValidationSeverity.Blocker,
                    "Boot scene Build Settings entry is invalid",
                    "The canonical Boot scene must appear exactly once and be enabled in Editor Build Settings.",
                    evidence.Paths.BootScenePath,
                    "TotalEntries=" + allCount +
                    "; EnabledEntries=" + enabledCount + ".",
                    "Open First Light Setup and review the approved Build Settings operation.");
            }
        }

        private static void ValidatePresentation(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            if (string.IsNullOrEmpty(evidence.ConfigurationSplashPath) ||
                evidence.RootPrefab.Roots.Count != 1)
            {
                return;
            }

            EchoLaunchValidationRootEvidence root =
                evidence.RootPrefab.Roots[0];

            if (!root.HasStatusPresenter ||
                !root.HasImageSplashPresenter)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.Presentation,
                    EchoLaunchValidationSeverity.Warning,
                    "Configured visual launch presentation is unavailable",
                    "A splash is configured, but the canonical root presenter does not provide both launch-status and image-splash presentation.",
                    evidence.Paths.RootPrefabPath,
                    "StatusPresenter=" +
                    (root.HasStatusPresenter ? "Yes" : "No") +
                    "; ImageSplashPresenter=" +
                    (root.HasImageSplashPresenter ? "Yes" : "No") +
                    ".",
                    "Assign a compatible project-owned presenter or intentionally remove the splash phase.");
            }
        }

        private static void ValidateSplash(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            if (string.IsNullOrEmpty(
                    evidence.ConfigurationSplashPath))
            {
                return;
            }

            EchoLaunchValidationAssetEvidence splash =
                evidence.SplashSequence;

            if (!splash.Exists ||
                !splash.IsType<SplashSequence>() ||
                !IsCanonicalId(splash.StableId) ||
                splash.SchemaVersion != SplashSequence.CurrentSchemaVersion)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.Splash,
                    EchoLaunchValidationSeverity.Error,
                    "Splash sequence is missing or unsupported",
                    "The configured splash sequence must have the exact type, a canonical identity, and the current schema.",
                    splash.Path,
                    "Type=" + splash.TypeName +
                    "; Schema=" + splash.SchemaVersion + ".",
                    "Correct the project-owned splash asset explicitly.");

                return;
            }

            HashSet<string> entryIds =
                new HashSet<string>(StringComparer.Ordinal);

            for (int index = 0;
                 index < evidence.SplashEntries.Count;
                 index++)
            {
                EchoLaunchValidationSplashEntryEvidence entry =
                    evidence.SplashEntries[index];

                bool valid =
                    IsCanonicalId(entry.EntryId) &&
                    !string.IsNullOrEmpty(entry.ImagePath) &&
                    IsFiniteNonnegative(entry.FadeInSeconds) &&
                    IsFiniteNonnegative(entry.HoldSeconds) &&
                    IsFiniteNonnegative(entry.FadeOutSeconds) &&
                    IsFiniteNonnegative(entry.MinimumDisplaySeconds) &&
                    Enum.IsDefined(
                        typeof(SplashSkipPolicy),
                        entry.SkipPolicyValue) &&
                    entryIds.Add(entry.EntryId);

                if (!valid)
                {
                    Add(
                        findings,
                        EchoLaunchValidationDiagnosticCodes.Splash,
                        EchoLaunchValidationSeverity.Error,
                        "Splash entry is invalid",
                        "One configured splash entry has invalid identity, image, timing, skip policy, or duplicate identity.",
                        splash.Path,
                        "EntryIndex=" + entry.Index +
                        "; EntryId=" + entry.EntryId +
                        "; Image=" + entry.ImagePath + ".",
                        "Correct the authored splash entry explicitly.");
                }
            }
        }

        private static void ValidatePolicies(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            for (int index = 0;
                 index < evidence.SequenceEntries.Count;
                 index++)
            {
                EchoLaunchValidationSequenceEntryEvidence entry =
                    evidence.SequenceEntries[index];

                if (!entry.Enabled)
                {
                    continue;
                }

                bool knownRequirement =
                    entry.IsRequired ^ entry.IsOptional;

                bool knownFailure =
                    Enum.IsDefined(
                        typeof(StartupStepFailureAction),
                        entry.FailureActionValue);

                bool safeRequiredPolicy =
                    !entry.IsRequired ||
                    entry.FailureActionValue ==
                        (int)StartupStepFailureAction.BlockLaunch;

                bool valid =
                    knownRequirement &&
                    knownFailure &&
                    IsFiniteNonnegative(entry.TimeoutSeconds) &&
                    safeRequiredPolicy;

                if (!valid)
                {
                    Add(
                        findings,
                        EchoLaunchValidationDiagnosticCodes.StepPolicy,
                        EchoLaunchValidationSeverity.Error,
                        "Startup step policy is unsafe",
                        "An enabled startup entry has an unsupported or contradictory requirement, failure, or timeout policy.",
                        evidence.StartupSequence.Path,
                        "EntryIndex=" + entry.Index +
                        "; Required=" + entry.IsRequired +
                        "; FailureAction=" + entry.FailureActionValue +
                        "; Timeout=" + entry.TimeoutSeconds + ".",
                        "Correct the authored startup-step policy explicitly.");
                }
            }
        }

        private static void ValidatePackageOwnedReferences(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            AddPackageReferenceFinding(
                evidence.ConfigurationStartupSequencePath,
                evidence.Configuration.Path,
                "startup sequence",
                findings);

            AddPackageReferenceFinding(
                evidence.ConfigurationDestinationPath,
                evidence.Configuration.Path,
                "launch destination",
                findings);

            AddPackageReferenceFinding(
                evidence.ConfigurationSplashPath,
                evidence.Configuration.Path,
                "splash sequence",
                findings);

            for (int index = 0;
                 index < evidence.SequenceEntries.Count;
                 index++)
            {
                AddPackageReferenceFinding(
                    evidence.SequenceEntries[index].DefinitionPath,
                    evidence.StartupSequence.Path,
                    "startup step definition",
                    findings);
            }
        }

        private static void ValidateCollectionIssues(
            EchoLaunchValidationEvidence evidence,
            List<EchoLaunchValidationFinding> findings)
        {
            for (int index = 0;
                 index < evidence.CollectionIssues.Count;
                 index++)
            {
                Add(
                    findings,
                    EchoLaunchValidationDiagnosticCodes.EvidenceUnavailable,
                    EchoLaunchValidationSeverity.Blocker,
                    "Validation evidence is incomplete",
                    evidence.CollectionIssues[index],
                    evidence.Request.ProjectRootPath,
                    "A required read-only inspection did not settle safely.",
                    "Resolve the scene, asset, import, or Editor-state issue and validate again.");
            }
        }

        private static void AddPackageReferenceFinding(
            string referencedPath,
            string ownerPath,
            string role,
            List<EchoLaunchValidationFinding> findings)
        {
            if (string.IsNullOrEmpty(referencedPath) ||
                !referencedPath.StartsWith(
                    "Packages/",
                    StringComparison.Ordinal))
            {
                return;
            }

            Add(
                findings,
                EchoLaunchValidationDiagnosticCodes.PackageOwnedReference,
                EchoLaunchValidationSeverity.Error,
                "Project configuration references immutable package content",
                "Project-owned First Light configuration must not use a package-owned " +
                role +
                " as its authored project content.",
                ownerPath,
                "Reference=" + referencedPath + ".",
                "Create or move the authored content beneath Assets/ without changing stable identity.");
        }

        private static int CountBuildScene(
            EchoLaunchValidationEvidence evidence,
            string path,
            bool? enabled)
        {
            int count = 0;

            for (int index = 0;
                 index < evidence.BuildSettingsScenes.Count;
                 index++)
            {
                EchoLaunchValidationBuildSceneEvidence scene =
                    evidence.BuildSettingsScenes[index];

                if (string.Equals(
                        scene.Path,
                        path,
                        StringComparison.Ordinal) &&
                    (!enabled.HasValue ||
                     scene.Enabled == enabled.Value))
                {
                    count++;
                }
            }

            return count;
        }

        private static bool IsProjectScenePath(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.StartsWith("Assets/", StringComparison.Ordinal) &&
                   value.EndsWith(
                       ".unity",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCanonicalId(string value)
        {
            if (string.IsNullOrEmpty(value) ||
                value.Length != 32)
            {
                return false;
            }

            for (int index = 0; index < value.Length; index++)
            {
                char character = value[index];

                bool digit =
                    character >= '0' &&
                    character <= '9';

                bool lowerHex =
                    character >= 'a' &&
                    character <= 'f';

                if (!digit && !lowerHex)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsFiniteNonnegative(double value)
        {
            return !double.IsNaN(value) &&
                   !double.IsInfinity(value) &&
                   value >= 0d;
        }

        private static void Add(
            List<EchoLaunchValidationFinding> findings,
            string code,
            EchoLaunchValidationSeverity severity,
            string title,
            string message,
            string path,
            string evidence,
            string suggestedAction)
        {
            findings.Add(
                new EchoLaunchValidationFinding(
                    code,
                    severity,
                    title,
                    message,
                    path,
                    evidence,
                    suggestedAction));
        }
    }
}
