
using System;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupRequest : IEquatable<EchoLaunchSetupRequest>
    {
        internal EchoLaunchSetupRequest(
            string projectRootPath,
            string bootScenePath,
            string destinationScenePath,
            bool createSplashSequence,
            EchoLaunchBuildSettingsPolicy buildSettingsPolicy,
            string selectedConfigurationPath = null,
            string selectedStartupSequencePath = null,
            string selectedLaunchDestinationPath = null,
            string selectedSplashSequencePath = null,
            string selectedRootPrefabPath = null,
            EchoLaunchSetupSplashAuthoringRequest splashAuthoring = null)
        {
            ProjectRootPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    projectRootPath);

            BootScenePath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    bootScenePath);
            DestinationScenePath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(destinationScenePath);
            CreateSplashSequence = createSplashSequence;
            BuildSettingsPolicy = buildSettingsPolicy;
            SelectedConfigurationPath = NormalizeOptional(selectedConfigurationPath);
            SelectedStartupSequencePath = NormalizeOptional(selectedStartupSequencePath);
            SelectedLaunchDestinationPath =
                NormalizeOptional(selectedLaunchDestinationPath);
            SelectedSplashSequencePath = NormalizeOptional(selectedSplashSequencePath);
            SelectedRootPrefabPath = NormalizeOptional(selectedRootPrefabPath);
            SplashAuthoring = splashAuthoring;
        }

        internal string ProjectRootPath { get; }
        internal string BootScenePath { get; }
        internal string DestinationScenePath { get; }
        internal bool CreateSplashSequence { get; }
        internal EchoLaunchBuildSettingsPolicy BuildSettingsPolicy { get; }
        internal string SelectedConfigurationPath { get; }
        internal string SelectedStartupSequencePath { get; }
        internal string SelectedLaunchDestinationPath { get; }
        internal string SelectedSplashSequencePath { get; }
        internal string SelectedRootPrefabPath { get; }
        internal EchoLaunchSetupSplashAuthoringRequest SplashAuthoring { get; }

        internal static EchoLaunchSetupRequest CreateDefault()
        {
            EchoLaunchSetupPathSet paths = EchoLaunchSetupPathSet.CreateDefault();

            return new EchoLaunchSetupRequest(
                paths.ProjectRootPath,
                paths.BootScenePath,
                string.Empty,
                false,
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);
        }

        internal string GetSelectedPath(EchoLaunchSetupAssetRole role)
        {
            switch (role)
            {
                case EchoLaunchSetupAssetRole.Configuration:
                    return SelectedConfigurationPath;
                case EchoLaunchSetupAssetRole.StartupSequence:
                    return SelectedStartupSequencePath;
                case EchoLaunchSetupAssetRole.LaunchDestination:
                    return SelectedLaunchDestinationPath;
                case EchoLaunchSetupAssetRole.SplashSequence:
                    return SelectedSplashSequencePath;
                case EchoLaunchSetupAssetRole.RootPrefab:
                    return SelectedRootPrefabPath;
                default:
                    return string.Empty;
            }
        }

        public bool Equals(EchoLaunchSetupRequest other)
        {
            return other != null &&
                   string.Equals(ProjectRootPath, other.ProjectRootPath, StringComparison.Ordinal) &&
                   string.Equals(BootScenePath, other.BootScenePath, StringComparison.Ordinal) &&
                   string.Equals(
                       DestinationScenePath,
                       other.DestinationScenePath,
                       StringComparison.Ordinal) &&
                   CreateSplashSequence == other.CreateSplashSequence &&
                   BuildSettingsPolicy == other.BuildSettingsPolicy &&
                   string.Equals(
                       SelectedConfigurationPath,
                       other.SelectedConfigurationPath,
                       StringComparison.Ordinal) &&
                   string.Equals(
                       SelectedStartupSequencePath,
                       other.SelectedStartupSequencePath,
                       StringComparison.Ordinal) &&
                   string.Equals(
                       SelectedLaunchDestinationPath,
                       other.SelectedLaunchDestinationPath,
                       StringComparison.Ordinal) &&
                   string.Equals(
                       SelectedSplashSequencePath,
                       other.SelectedSplashSequencePath,
                       StringComparison.Ordinal) &&
                   string.Equals(
                       SelectedRootPrefabPath,
                       other.SelectedRootPrefabPath,
                       StringComparison.Ordinal) &&
                   Equals(
                       SplashAuthoring,
                       other.SplashAuthoring);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchSetupRequest);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = ProjectRootPath.GetHashCode();
                hash = (hash * 397) ^ BootScenePath.GetHashCode();
                hash = (hash * 397) ^ DestinationScenePath.GetHashCode();
                hash = (hash * 397) ^ CreateSplashSequence.GetHashCode();
                hash = (hash * 397) ^ BuildSettingsPolicy.GetHashCode();

                if (SplashAuthoring != null)
                {
                    hash =
                        (hash * 397) ^
                        SplashAuthoring.GetHashCode();
                }

                return hash;
            }
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : EchoLaunchSetupPathUtility.NormalizeSeparators(value);
        }
    }
}
