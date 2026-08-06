using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EchoDevGames.EchoLaunch.Editor.Setup;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal sealed class EchoLaunchValidationAssetEvidence
    {
        internal EchoLaunchValidationAssetEvidence(
            string path,
            bool exists,
            string typeName,
            string stableId,
            int schemaVersion)
        {
            Path = Normalize(path);
            Exists = exists;
            TypeName = typeName ?? string.Empty;
            StableId = stableId ?? string.Empty;
            SchemaVersion = schemaVersion;
        }

        internal string Path { get; }
        internal bool Exists { get; }
        internal string TypeName { get; }
        internal string StableId { get; }
        internal int SchemaVersion { get; }

        internal bool IsType<T>()
        {
            return Exists &&
                   string.Equals(
                       TypeName,
                       typeof(T).FullName,
                       StringComparison.Ordinal);
        }

        private static string Normalize(string value)
        {
            return EchoLaunchSetupPathUtility.NormalizeSeparators(value);
        }
    }

    internal sealed class EchoLaunchValidationBuildSceneEvidence
    {
        internal EchoLaunchValidationBuildSceneEvidence(
            string path,
            bool enabled,
            int index)
        {
            Path = EchoLaunchSetupPathUtility.NormalizeSeparators(path);
            Enabled = enabled;
            Index = index;
        }

        internal string Path { get; }
        internal bool Enabled { get; }
        internal int Index { get; }
    }

    internal sealed class EchoLaunchValidationRootEvidence
    {
        internal EchoLaunchValidationRootEvidence(
            string configurationPath,
            string prefabSourcePath,
            bool hasStatusPresenter,
            bool hasImageSplashPresenter)
        {
            ConfigurationPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    configurationPath);

            PrefabSourcePath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    prefabSourcePath);

            HasStatusPresenter = hasStatusPresenter;
            HasImageSplashPresenter = hasImageSplashPresenter;
        }

        internal string ConfigurationPath { get; }
        internal string PrefabSourcePath { get; }
        internal bool HasStatusPresenter { get; }
        internal bool HasImageSplashPresenter { get; }
    }

    internal sealed class EchoLaunchValidationRootPrefabEvidence
    {
        private readonly ReadOnlyCollection<EchoLaunchValidationRootEvidence>
            roots;

        internal EchoLaunchValidationRootPrefabEvidence(
            string path,
            bool exists,
            bool reachesPackageTemplate,
            IEnumerable<EchoLaunchValidationRootEvidence> roots)
        {
            Path = EchoLaunchSetupPathUtility.NormalizeSeparators(path);
            Exists = exists;
            ReachesPackageTemplate = reachesPackageTemplate;
            this.roots =
                new ReadOnlyCollection<EchoLaunchValidationRootEvidence>(
                    roots == null
                        ? new List<EchoLaunchValidationRootEvidence>()
                        : new List<EchoLaunchValidationRootEvidence>(roots));
        }

        internal string Path { get; }
        internal bool Exists { get; }
        internal bool ReachesPackageTemplate { get; }
        internal IReadOnlyList<EchoLaunchValidationRootEvidence> Roots => roots;
    }

    internal sealed class EchoLaunchValidationDirectSceneEvidence
    {
        internal EchoLaunchValidationDirectSceneEvidence(
            string containingScenePath,
            bool componentEnabled,
            int policyValue,
            string directConfigurationPath,
            string directConfigurationTypeName,
            string directConfigurationId,
            int directConfigurationSchema,
            string rootPrefabPath,
            int rootCount,
            int activeRootCount,
            bool reachesPackageTemplate,
            int launchModeValue,
            string launchConfigurationPath,
            int launchConfigurationSchema,
            string destinationAssetPath,
            int destinationSchema,
            string destinationScenePath)
        {
            ContainingScenePath = Normalize(containingScenePath);
            ComponentEnabled = componentEnabled;
            PolicyValue = policyValue;
            DirectConfigurationPath = Normalize(directConfigurationPath);
            DirectConfigurationTypeName =
                directConfigurationTypeName ?? string.Empty;
            DirectConfigurationId =
                directConfigurationId ?? string.Empty;
            DirectConfigurationSchema = directConfigurationSchema;
            RootPrefabPath = Normalize(rootPrefabPath);
            RootCount = rootCount;
            ActiveRootCount = activeRootCount;
            ReachesPackageTemplate = reachesPackageTemplate;
            LaunchModeValue = launchModeValue;
            LaunchConfigurationPath = Normalize(launchConfigurationPath);
            LaunchConfigurationSchema = launchConfigurationSchema;
            DestinationAssetPath = Normalize(destinationAssetPath);
            DestinationSchema = destinationSchema;
            DestinationScenePath = Normalize(destinationScenePath);
        }

        internal string ContainingScenePath { get; }
        internal bool ComponentEnabled { get; }
        internal int PolicyValue { get; }
        internal string DirectConfigurationPath { get; }
        internal string DirectConfigurationTypeName { get; }
        internal string DirectConfigurationId { get; }
        internal int DirectConfigurationSchema { get; }
        internal string RootPrefabPath { get; }
        internal int RootCount { get; }
        internal int ActiveRootCount { get; }
        internal bool ReachesPackageTemplate { get; }
        internal int LaunchModeValue { get; }
        internal string LaunchConfigurationPath { get; }
        internal int LaunchConfigurationSchema { get; }
        internal string DestinationAssetPath { get; }
        internal int DestinationSchema { get; }
        internal string DestinationScenePath { get; }

        private static string Normalize(string value)
        {
            return EchoLaunchSetupPathUtility.NormalizeSeparators(value);
        }
    }

    internal sealed class EchoLaunchValidationSceneEvidence
    {
        private readonly ReadOnlyCollection<EchoLaunchValidationRootEvidence>
            roots;

        private readonly ReadOnlyCollection<
            EchoLaunchValidationDirectSceneEvidence> directInitializers;

        internal EchoLaunchValidationSceneEvidence(
            string path,
            bool exists,
            bool inspected,
            IEnumerable<EchoLaunchValidationRootEvidence> roots,
            IEnumerable<EchoLaunchValidationDirectSceneEvidence>
                directInitializers = null)
        {
            Path = EchoLaunchSetupPathUtility.NormalizeSeparators(path);
            Exists = exists;
            Inspected = inspected;
            this.roots =
                new ReadOnlyCollection<EchoLaunchValidationRootEvidence>(
                    roots == null
                        ? new List<EchoLaunchValidationRootEvidence>()
                        : new List<EchoLaunchValidationRootEvidence>(roots));

            this.directInitializers =
                new ReadOnlyCollection<
                    EchoLaunchValidationDirectSceneEvidence>(
                        directInitializers == null
                            ? new List<
                                EchoLaunchValidationDirectSceneEvidence>()
                            : new List<
                                EchoLaunchValidationDirectSceneEvidence>(
                                    directInitializers));
        }

        internal string Path { get; }
        internal bool Exists { get; }
        internal bool Inspected { get; }
        internal IReadOnlyList<EchoLaunchValidationRootEvidence> Roots => roots;

        internal IReadOnlyList<EchoLaunchValidationDirectSceneEvidence>
            DirectInitializers => directInitializers;
    }

    internal sealed class EchoLaunchValidationSequenceEntryEvidence
    {
        internal EchoLaunchValidationSequenceEntryEvidence(
            int index,
            string entryId,
            bool enabled,
            string definitionPath,
            string definitionId,
            int definitionSchema,
            bool isRequired,
            bool isOptional,
            int failureActionValue,
            double timeoutSeconds,
            bool supportsCancellation)
        {
            Index = index;
            EntryId = entryId ?? string.Empty;
            Enabled = enabled;
            DefinitionPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    definitionPath);

            DefinitionId = definitionId ?? string.Empty;
            DefinitionSchema = definitionSchema;
            IsRequired = isRequired;
            IsOptional = isOptional;
            FailureActionValue = failureActionValue;
            TimeoutSeconds = timeoutSeconds;
            SupportsCancellation = supportsCancellation;
        }

        internal int Index { get; }
        internal string EntryId { get; }
        internal bool Enabled { get; }
        internal string DefinitionPath { get; }
        internal string DefinitionId { get; }
        internal int DefinitionSchema { get; }
        internal bool IsRequired { get; }
        internal bool IsOptional { get; }
        internal int FailureActionValue { get; }
        internal double TimeoutSeconds { get; }
        internal bool SupportsCancellation { get; }
    }

    internal sealed class EchoLaunchValidationSplashEntryEvidence
    {
        internal EchoLaunchValidationSplashEntryEvidence(
            int index,
            string entryId,
            string imagePath,
            double fadeInSeconds,
            double holdSeconds,
            double fadeOutSeconds,
            double minimumDisplaySeconds,
            int skipPolicyValue)
        {
            Index = index;
            EntryId = entryId ?? string.Empty;
            ImagePath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(imagePath);

            FadeInSeconds = fadeInSeconds;
            HoldSeconds = holdSeconds;
            FadeOutSeconds = fadeOutSeconds;
            MinimumDisplaySeconds = minimumDisplaySeconds;
            SkipPolicyValue = skipPolicyValue;
        }

        internal int Index { get; }
        internal string EntryId { get; }
        internal string ImagePath { get; }
        internal double FadeInSeconds { get; }
        internal double HoldSeconds { get; }
        internal double FadeOutSeconds { get; }
        internal double MinimumDisplaySeconds { get; }
        internal int SkipPolicyValue { get; }
    }

    internal sealed class EchoLaunchValidationEvidence
    {
        private readonly ReadOnlyCollection<
            EchoLaunchValidationSequenceEntryEvidence> sequenceEntries;
        private readonly ReadOnlyCollection<
            EchoLaunchValidationSplashEntryEvidence> splashEntries;
        private readonly ReadOnlyCollection<
            EchoLaunchValidationBuildSceneEvidence> buildSettingsScenes;
        private readonly ReadOnlyCollection<
            EchoLaunchValidationSceneEvidence> sceneEvidence;
        private readonly ReadOnlyCollection<string> collectionIssues;

        internal EchoLaunchValidationEvidence(
            EchoLaunchValidationRequest request,
            EchoLaunchSetupPathSet paths,
            bool packageTemplateAvailable,
            EchoLaunchValidationAssetEvidence configuration,
            EchoLaunchValidationAssetEvidence startupSequence,
            EchoLaunchValidationAssetEvidence destination,
            EchoLaunchValidationAssetEvidence splashSequence,
            EchoLaunchValidationRootPrefabEvidence rootPrefab,
            string configurationStartupSequencePath,
            string configurationDestinationPath,
            string configurationSplashPath,
            IEnumerable<EchoLaunchValidationSequenceEntryEvidence> sequenceEntries,
            string destinationScenePath,
            string destinationDisplayName,
            IEnumerable<EchoLaunchValidationSplashEntryEvidence> splashEntries,
            IEnumerable<EchoLaunchValidationBuildSceneEvidence> buildSettingsScenes,
            IEnumerable<EchoLaunchValidationSceneEvidence> sceneEvidence,
            IEnumerable<string> collectionIssues)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            Paths = paths ?? throw new ArgumentNullException(nameof(paths));
            PackageTemplateAvailable = packageTemplateAvailable;
            Configuration =
                configuration ??
                throw new ArgumentNullException(nameof(configuration));
            StartupSequence =
                startupSequence ??
                throw new ArgumentNullException(nameof(startupSequence));
            Destination =
                destination ??
                throw new ArgumentNullException(nameof(destination));
            SplashSequence =
                splashSequence ??
                throw new ArgumentNullException(nameof(splashSequence));
            RootPrefab =
                rootPrefab ??
                throw new ArgumentNullException(nameof(rootPrefab));

            ConfigurationStartupSequencePath =
                Normalize(configurationStartupSequencePath);
            ConfigurationDestinationPath =
                Normalize(configurationDestinationPath);
            ConfigurationSplashPath =
                Normalize(configurationSplashPath);
            this.sequenceEntries = Copy(sequenceEntries);
            DestinationScenePath = Normalize(destinationScenePath);
            DestinationDisplayName = destinationDisplayName ?? string.Empty;
            this.splashEntries = Copy(splashEntries);
            this.buildSettingsScenes = Copy(buildSettingsScenes);
            this.sceneEvidence = Copy(sceneEvidence);
            this.collectionIssues =
                new ReadOnlyCollection<string>(
                    collectionIssues == null
                        ? new List<string>()
                        : new List<string>(collectionIssues));
        }

        internal EchoLaunchValidationRequest Request { get; }
        internal EchoLaunchSetupPathSet Paths { get; }
        internal bool PackageTemplateAvailable { get; }
        internal EchoLaunchValidationAssetEvidence Configuration { get; }
        internal EchoLaunchValidationAssetEvidence StartupSequence { get; }
        internal EchoLaunchValidationAssetEvidence Destination { get; }
        internal EchoLaunchValidationAssetEvidence SplashSequence { get; }
        internal EchoLaunchValidationRootPrefabEvidence RootPrefab { get; }
        internal string ConfigurationStartupSequencePath { get; }
        internal string ConfigurationDestinationPath { get; }
        internal string ConfigurationSplashPath { get; }
        internal IReadOnlyList<EchoLaunchValidationSequenceEntryEvidence>
            SequenceEntries => sequenceEntries;
        internal string DestinationScenePath { get; }
        internal string DestinationDisplayName { get; }
        internal IReadOnlyList<EchoLaunchValidationSplashEntryEvidence>
            SplashEntries => splashEntries;
        internal IReadOnlyList<EchoLaunchValidationBuildSceneEvidence>
            BuildSettingsScenes => buildSettingsScenes;
        internal IReadOnlyList<EchoLaunchValidationSceneEvidence>
            SceneEvidence => sceneEvidence;
        internal IReadOnlyList<string> CollectionIssues => collectionIssues;
        internal string EvidenceFingerprint =>
            EchoLaunchValidationFingerprint.ForEvidence(this);

        internal EchoLaunchValidationSceneEvidence FindScene(string path)
        {
            string normalized = Normalize(path);

            for (int index = 0; index < sceneEvidence.Count; index++)
            {
                if (string.Equals(
                        sceneEvidence[index].Path,
                        normalized,
                        StringComparison.Ordinal))
                {
                    return sceneEvidence[index];
                }
            }

            return new EchoLaunchValidationSceneEvidence(
                normalized,
                false,
                false,
                Array.Empty<EchoLaunchValidationRootEvidence>());
        }

        private static ReadOnlyCollection<T> Copy<T>(
            IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(
                source == null
                    ? new List<T>()
                    : new List<T>(source));
        }

        private static string Normalize(string value)
        {
            return EchoLaunchSetupPathUtility.NormalizeSeparators(value);
        }
    }
}
