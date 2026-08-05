
using System;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupPathSet : IEquatable<EchoLaunchSetupPathSet>
    {
        internal const string DefaultProjectRootPath =
            "Assets/EchoDevGames/FirstLight";

        internal const string PackageRootPrefabTemplatePath =
            "Packages/com.echodevgames.echo-launch/" +
            "Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab";

        internal EchoLaunchSetupPathSet(string projectRootPath, string bootScenePath)
        {
            ProjectRootPath = projectRootPath ?? string.Empty;
            ConfigurationFolderPath = ProjectRootPath + "/Configuration";
            PrefabsFolderPath = ProjectRootPath + "/Prefabs";
            ScenesFolderPath = ProjectRootPath + "/Scenes";
            ConfigurationAssetPath =
                ConfigurationFolderPath + "/EchoLaunchConfiguration.asset";
            StartupSequenceAssetPath =
                ConfigurationFolderPath + "/StartupSequence.asset";
            LaunchDestinationAssetPath =
                ConfigurationFolderPath + "/LaunchDestination.asset";
            SplashSequenceAssetPath =
                ConfigurationFolderPath + "/SplashSequence.asset";
            RootPrefabPath = PrefabsFolderPath + "/EchoLaunchRoot.prefab";
            BootScenePath = bootScenePath ?? string.Empty;
        }

        internal string ProjectRootPath { get; }
        internal string ConfigurationFolderPath { get; }
        internal string PrefabsFolderPath { get; }
        internal string ScenesFolderPath { get; }
        internal string ConfigurationAssetPath { get; }
        internal string StartupSequenceAssetPath { get; }
        internal string LaunchDestinationAssetPath { get; }
        internal string SplashSequenceAssetPath { get; }
        internal string RootPrefabPath { get; }
        internal string BootScenePath { get; }

        internal static EchoLaunchSetupPathSet CreateDefault()
        {
            return new EchoLaunchSetupPathSet(
                DefaultProjectRootPath,
                DefaultProjectRootPath + "/Scenes/Boot.unity");
        }

        public bool Equals(EchoLaunchSetupPathSet other)
        {
            return other != null &&
                   string.Equals(
                       ProjectRootPath,
                       other.ProjectRootPath,
                       StringComparison.Ordinal) &&
                   string.Equals(
                       BootScenePath,
                       other.BootScenePath,
                       StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchSetupPathSet);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ProjectRootPath.GetHashCode() * 397) ^
                       BootScenePath.GetHashCode();
            }
        }
    }

    internal static class EchoLaunchSetupPathUtility
    {
        internal static bool TryCreatePathSet(
            string projectRootPath,
            string bootScenePath,
            out EchoLaunchSetupPathSet pathSet,
            out string errorMessage)
        {
            pathSet = null;

            if (!TryNormalizeProjectRoot(
                    projectRootPath,
                    out string normalizedRoot,
                    out errorMessage))
            {
                return false;
            }

            string requestedBootPath =
                string.IsNullOrWhiteSpace(bootScenePath)
                    ? normalizedRoot + "/Scenes/Boot.unity"
                    : bootScenePath;

            if (!TryNormalizeProjectAssetPath(
                    requestedBootPath,
                    ".unity",
                    out string normalizedBootPath,
                    out errorMessage))
            {
                return false;
            }

            pathSet = new EchoLaunchSetupPathSet(
                normalizedRoot,
                normalizedBootPath);

            errorMessage = string.Empty;
            return true;
        }

        internal static bool TryNormalizeProjectRoot(
            string value,
            out string normalized,
            out string errorMessage)
        {
            normalized = NormalizeSeparators(value);

            if (!TryValidateCommonProjectPath(normalized, out errorMessage))
            {
                return false;
            }

            if (string.Equals(normalized, "Assets", StringComparison.Ordinal))
            {
                errorMessage =
                    "The project root must be below Assets, not Assets itself.";
                return false;
            }

            int slashIndex = normalized.LastIndexOf('/');
            int dotIndex = normalized.LastIndexOf('.');

            if (dotIndex > slashIndex)
            {
                errorMessage = "The project root must be a folder path.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        internal static bool TryNormalizeProjectAssetPath(
            string value,
            string requiredExtension,
            out string normalized,
            out string errorMessage)
        {
            normalized = NormalizeSeparators(value);

            if (!TryValidateCommonProjectPath(normalized, out errorMessage))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(requiredExtension) &&
                !normalized.EndsWith(
                    requiredExtension,
                    StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "The path must end with " + requiredExtension + ".";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        internal static string NormalizeSeparators(string value)
        {
            string normalized =
                (value ?? string.Empty).Trim().Replace('\\', '/');

            while (normalized.Contains("//"))
            {
                normalized = normalized.Replace("//", "/");
            }

            return normalized.TrimEnd('/');
        }

        private static bool TryValidateCommonProjectPath(
            string normalized,
            out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                errorMessage = "A project asset path is required.";
                return false;
            }

            if (normalized.IndexOf(':') >= 0 ||
                normalized.StartsWith("/", StringComparison.Ordinal))
            {
                errorMessage = "Absolute filesystem paths are not allowed.";
                return false;
            }

            if (!normalized.StartsWith("Assets/", StringComparison.Ordinal))
            {
                errorMessage =
                    "Generated setup paths must be below Assets/.";
                return false;
            }

            if (normalized.Contains("/../") ||
                normalized.EndsWith("/..", StringComparison.Ordinal))
            {
                errorMessage = "Path traversal segments are not allowed.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
