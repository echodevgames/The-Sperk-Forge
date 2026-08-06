using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchProjectAssetFact :
        IEquatable<EchoLaunchProjectAssetFact>
    {
        internal EchoLaunchProjectAssetFact(
            string path,
            bool exists,
            bool isFolder,
            string guid,
            string mainAssetTypeName,
            int? configurationSchemaVersion = null,
            bool hasRepairEvidence = false,
            string stableId = null,
            string startupSequencePath = null,
            string launchDestinationPath = null,
            string splashSequencePath = null,
            string destinationScenePath = null,
            string destinationDisplayName = null,
            string prefabAssetType = null,
            string prefabSourcePath = null,
            bool prefabLineageMatchesTemplate = false,
            int? echoLaunchRootCount = null,
            string rootConfigurationPath = null,
            bool sceneInspectionSafe = true,
            string sceneInspectionMessage = null,
            bool sceneWasOpen = false)
        {
            Path = EchoLaunchSetupPathUtility.NormalizeSeparators(path);
            Exists = exists;
            IsFolder = isFolder;
            Guid = guid ?? string.Empty;
            MainAssetTypeName = mainAssetTypeName ?? string.Empty;
            ConfigurationSchemaVersion = configurationSchemaVersion;
            HasRepairEvidence = hasRepairEvidence;
            StableId = stableId ?? string.Empty;
            StartupSequencePath = NormalizeOptional(startupSequencePath);
            LaunchDestinationPath = NormalizeOptional(launchDestinationPath);
            SplashSequencePath = NormalizeOptional(splashSequencePath);
            DestinationScenePath = NormalizeOptional(destinationScenePath);
            DestinationDisplayName = destinationDisplayName ?? string.Empty;
            PrefabAssetType = prefabAssetType ?? string.Empty;
            PrefabSourcePath = NormalizeOptional(prefabSourcePath);
            PrefabLineageMatchesTemplate = prefabLineageMatchesTemplate;
            EchoLaunchRootCount = echoLaunchRootCount;
            RootConfigurationPath = NormalizeOptional(rootConfigurationPath);
            SceneInspectionSafe = sceneInspectionSafe;
            SceneInspectionMessage = sceneInspectionMessage ?? string.Empty;
            SceneWasOpen = sceneWasOpen;
        }

        internal string Path { get; }
        internal bool Exists { get; }
        internal bool IsFolder { get; }
        internal string Guid { get; }
        internal string MainAssetTypeName { get; }
        internal int? ConfigurationSchemaVersion { get; }
        internal bool HasRepairEvidence { get; }
        internal string StableId { get; }
        internal string StartupSequencePath { get; }
        internal string LaunchDestinationPath { get; }
        internal string SplashSequencePath { get; }
        internal string DestinationScenePath { get; }
        internal string DestinationDisplayName { get; }
        internal string PrefabAssetType { get; }
        internal string PrefabSourcePath { get; }
        internal bool PrefabLineageMatchesTemplate { get; }
        internal int? EchoLaunchRootCount { get; }
        internal string RootConfigurationPath { get; }
        internal bool SceneInspectionSafe { get; }
        internal string SceneInspectionMessage { get; }
        internal bool SceneWasOpen { get; }

        internal bool IsType(string fullTypeName)
        {
            return Exists &&
                   string.Equals(
                       MainAssetTypeName,
                       fullTypeName,
                       StringComparison.Ordinal);
        }

        public bool Equals(EchoLaunchProjectAssetFact other)
        {
            return other != null &&
                   string.Equals(Path, other.Path, StringComparison.Ordinal) &&
                   Exists == other.Exists &&
                   IsFolder == other.IsFolder &&
                   string.Equals(Guid, other.Guid, StringComparison.Ordinal) &&
                   string.Equals(
                       MainAssetTypeName,
                       other.MainAssetTypeName,
                       StringComparison.Ordinal) &&
                   ConfigurationSchemaVersion == other.ConfigurationSchemaVersion &&
                   HasRepairEvidence == other.HasRepairEvidence &&
                   string.Equals(StableId, other.StableId, StringComparison.Ordinal) &&
                   string.Equals(StartupSequencePath, other.StartupSequencePath, StringComparison.Ordinal) &&
                   string.Equals(LaunchDestinationPath, other.LaunchDestinationPath, StringComparison.Ordinal) &&
                   string.Equals(SplashSequencePath, other.SplashSequencePath, StringComparison.Ordinal) &&
                   string.Equals(DestinationScenePath, other.DestinationScenePath, StringComparison.Ordinal) &&
                   string.Equals(DestinationDisplayName, other.DestinationDisplayName, StringComparison.Ordinal) &&
                   string.Equals(PrefabAssetType, other.PrefabAssetType, StringComparison.Ordinal) &&
                   string.Equals(PrefabSourcePath, other.PrefabSourcePath, StringComparison.Ordinal) &&
                   PrefabLineageMatchesTemplate == other.PrefabLineageMatchesTemplate &&
                   EchoLaunchRootCount == other.EchoLaunchRootCount &&
                   string.Equals(RootConfigurationPath, other.RootConfigurationPath, StringComparison.Ordinal) &&
                   SceneInspectionSafe == other.SceneInspectionSafe &&
                   string.Equals(SceneInspectionMessage, other.SceneInspectionMessage, StringComparison.Ordinal) &&
                   SceneWasOpen == other.SceneWasOpen;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchProjectAssetFact);
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : EchoLaunchSetupPathUtility.NormalizeSeparators(value);
        }
    }

    internal sealed class EchoLaunchBuildSettingsSceneFact :
        IEquatable<EchoLaunchBuildSettingsSceneFact>
    {
        internal EchoLaunchBuildSettingsSceneFact(
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

        public bool Equals(EchoLaunchBuildSettingsSceneFact other)
        {
            return other != null &&
                   string.Equals(Path, other.Path, StringComparison.Ordinal) &&
                   Enabled == other.Enabled &&
                   Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchBuildSettingsSceneFact);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Path.GetHashCode() * 397) ^ Index;
            }
        }
    }

    internal static class EchoLaunchSetupAssetTypeNames
    {
        internal static readonly string Configuration =
            typeof(EchoLaunchConfiguration).FullName;

        internal static readonly string StartupSequence =
            typeof(StartupSequence).FullName;

        internal static readonly string LaunchDestination =
            typeof(LaunchDestination).FullName;

        internal static readonly string SplashSequence =
            typeof(SplashSequence).FullName;

        internal const string GameObject = "UnityEngine.GameObject";
        internal const string SceneAsset = "UnityEditor.SceneAsset";
        internal const string Folder = "UnityEditor.DefaultAsset";
    }

    internal sealed class EchoLaunchProjectSnapshot
    {
        private readonly ReadOnlyCollection<EchoLaunchProjectAssetFact> assetFacts;
        private readonly ReadOnlyCollection<EchoLaunchBuildSettingsSceneFact>
            buildSettingsScenes;

        private readonly Dictionary<
            EchoLaunchSetupAssetRole,
            ReadOnlyCollection<EchoLaunchProjectAssetFact>> candidatesByRole;

        internal EchoLaunchProjectSnapshot(
            IEnumerable<EchoLaunchProjectAssetFact> assetFacts,
            IEnumerable<EchoLaunchBuildSettingsSceneFact> buildSettingsScenes,
            bool packageRootTemplateAvailable,
            string packageRootTemplateGuid,
            IDictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> candidatesByRole = null)
        {
            this.assetFacts = new ReadOnlyCollection<EchoLaunchProjectAssetFact>(
                CopyAndSortAssetFacts(assetFacts));

            this.buildSettingsScenes =
                new ReadOnlyCollection<EchoLaunchBuildSettingsSceneFact>(
                    CopyAndSortBuildScenes(buildSettingsScenes));

            PackageRootTemplateAvailable = packageRootTemplateAvailable;
            PackageRootTemplateGuid = packageRootTemplateGuid ?? string.Empty;
            this.candidatesByRole = CopyCandidates(candidatesByRole);
        }

        internal IReadOnlyList<EchoLaunchProjectAssetFact> AssetFacts =>
            assetFacts;

        internal IReadOnlyList<EchoLaunchBuildSettingsSceneFact>
            BuildSettingsScenes => buildSettingsScenes;

        internal bool PackageRootTemplateAvailable { get; }
        internal string PackageRootTemplateGuid { get; }

        internal EchoLaunchProjectAssetFact FindAssetFact(string path)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            for (int index = 0; index < assetFacts.Count; index++)
            {
                if (string.Equals(
                        assetFacts[index].Path,
                        normalized,
                        StringComparison.Ordinal))
                {
                    return assetFacts[index];
                }
            }

            return new EchoLaunchProjectAssetFact(
                normalized,
                false,
                false,
                string.Empty,
                string.Empty);
        }

        internal IReadOnlyList<EchoLaunchProjectAssetFact> GetCandidates(
            EchoLaunchSetupAssetRole role)
        {
            if (candidatesByRole.TryGetValue(
                    role,
                    out ReadOnlyCollection<EchoLaunchProjectAssetFact> candidates))
            {
                return candidates;
            }

            return Array.Empty<EchoLaunchProjectAssetFact>();
        }

        internal int FindBuildSettingsIndex(string scenePath)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(scenePath);

            for (int index = 0; index < buildSettingsScenes.Count; index++)
            {
                if (string.Equals(
                        buildSettingsScenes[index].Path,
                        normalized,
                        StringComparison.Ordinal))
                {
                    return buildSettingsScenes[index].Index;
                }
            }

            return -1;
        }

        internal int CountBuildSettingsEntries(string scenePath)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(scenePath);
            int count = 0;

            for (int index = 0; index < buildSettingsScenes.Count; index++)
            {
                if (string.Equals(
                        buildSettingsScenes[index].Path,
                        normalized,
                        StringComparison.Ordinal))
                {
                    count++;
                }
            }

            return count;
        }

        internal EchoLaunchBuildSettingsSceneFact FindFirstBuildSettingsFact(
            string scenePath)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(scenePath);

            for (int index = 0; index < buildSettingsScenes.Count; index++)
            {
                if (string.Equals(
                        buildSettingsScenes[index].Path,
                        normalized,
                        StringComparison.Ordinal))
                {
                    return buildSettingsScenes[index];
                }
            }

            return null;
        }

        internal string EvidenceFingerprint =>
            EchoLaunchSetupFingerprint.ForSnapshot(this);

        internal string CreateEvidenceSummary()
        {
            int repairEvidenceCount = 0;
            for (int index = 0; index < assetFacts.Count; index++)
            {
                if (assetFacts[index].HasRepairEvidence)
                {
                    repairEvidenceCount++;
                }
            }

            return "Assets=" + assetFacts.Count +
                   ";BuildScenes=" + buildSettingsScenes.Count +
                   ";RepairEvidence=" + repairEvidenceCount +
                   ";Template=" +
                   (PackageRootTemplateAvailable ? "Present" : "Missing");
        }

        private static List<EchoLaunchProjectAssetFact> CopyAndSortAssetFacts(
            IEnumerable<EchoLaunchProjectAssetFact> source)
        {
            List<EchoLaunchProjectAssetFact> result =
                source == null
                    ? new List<EchoLaunchProjectAssetFact>()
                    : new List<EchoLaunchProjectAssetFact>(source);

            result.Sort(
                delegate(
                    EchoLaunchProjectAssetFact left,
                    EchoLaunchProjectAssetFact right)
                {
                    return string.Compare(
                        left.Path,
                        right.Path,
                        StringComparison.Ordinal);
                });

            return result;
        }

        private static List<EchoLaunchBuildSettingsSceneFact>
            CopyAndSortBuildScenes(
                IEnumerable<EchoLaunchBuildSettingsSceneFact> source)
        {
            List<EchoLaunchBuildSettingsSceneFact> result =
                source == null
                    ? new List<EchoLaunchBuildSettingsSceneFact>()
                    : new List<EchoLaunchBuildSettingsSceneFact>(source);

            result.Sort(
                delegate(
                    EchoLaunchBuildSettingsSceneFact left,
                    EchoLaunchBuildSettingsSceneFact right)
                {
                    int comparison = left.Index.CompareTo(right.Index);

                    return comparison != 0
                        ? comparison
                        : string.Compare(
                            left.Path,
                            right.Path,
                            StringComparison.Ordinal);
                });

            return result;
        }

        private static Dictionary<
            EchoLaunchSetupAssetRole,
            ReadOnlyCollection<EchoLaunchProjectAssetFact>> CopyCandidates(
                IDictionary<
                    EchoLaunchSetupAssetRole,
                    IEnumerable<EchoLaunchProjectAssetFact>> source)
        {
            Dictionary<
                EchoLaunchSetupAssetRole,
                ReadOnlyCollection<EchoLaunchProjectAssetFact>> result =
                    new Dictionary<
                        EchoLaunchSetupAssetRole,
                        ReadOnlyCollection<EchoLaunchProjectAssetFact>>();

            if (source == null)
            {
                return result;
            }

            foreach (
                KeyValuePair<
                    EchoLaunchSetupAssetRole,
                    IEnumerable<EchoLaunchProjectAssetFact>> pair in source)
            {
                result[pair.Key] =
                    new ReadOnlyCollection<EchoLaunchProjectAssetFact>(
                        CopyAndSortAssetFacts(pair.Value));
            }

            return result;
        }
    }
}
