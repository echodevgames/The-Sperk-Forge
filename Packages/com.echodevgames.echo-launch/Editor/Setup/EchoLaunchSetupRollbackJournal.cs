using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupRollbackJournal
    {
        private readonly List<string> createdAssetPaths =
            new List<string>();

        private readonly List<string> createdFolderPaths =
            new List<string>();

        private readonly Dictionary<string, string> createdAssetGuids =
            new Dictionary<string, string>(StringComparer.Ordinal);

        private readonly Dictionary<string, string> createdFolderGuids =
            new Dictionary<string, string>(StringComparer.Ordinal);

        private EditorBuildSettingsScene[] originalBuildSettings =
            Array.Empty<EditorBuildSettingsScene>();

        private bool buildSettingsCaptured;
        private bool buildSettingsChanged;

        internal IReadOnlyList<string> CreatedAssetPaths =>
            createdAssetPaths;

        internal IReadOnlyList<string> CreatedFolderPaths =>
            createdFolderPaths;

        internal bool BuildSettingsChanged => buildSettingsChanged;

        internal string BuildSettingsBeforeSummary =>
            EchoLaunchSetupBuildSettingsWriter.Summarize(
                originalBuildSettings);

        internal void RecordCreatedAsset(string path)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            if (!createdAssetPaths.Contains(normalized))
            {
                createdAssetPaths.Add(normalized);
                createdAssetGuids[normalized] =
                    AssetDatabase.AssetPathToGUID(normalized);
            }
        }

        internal void RecordCreatedFolder(string path)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            if (!createdFolderPaths.Contains(normalized))
            {
                createdFolderPaths.Add(normalized);
                createdFolderGuids[normalized] =
                    AssetDatabase.AssetPathToGUID(normalized);
            }
        }

        internal void CaptureBuildSettings()
        {
            if (buildSettingsCaptured)
            {
                return;
            }

            originalBuildSettings =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);

            buildSettingsCaptured = true;
        }

        internal void MarkBuildSettingsChanged()
        {
            buildSettingsChanged = true;
        }

        internal EchoLaunchSetupRollbackResult Rollback()
        {
            List<string> manualRecoveryPaths =
                new List<string>();

            if (buildSettingsCaptured && buildSettingsChanged)
            {
                try
                {
                    EditorBuildSettings.scenes =
                        EchoLaunchSetupBuildSettingsWriter.Clone(
                            originalBuildSettings);
                }
                catch
                {
                    manualRecoveryPaths.Add("ProjectSettings/EditorBuildSettings.asset");
                }
            }

            for (int index = createdAssetPaths.Count - 1;
                 index >= 0;
                 index--)
            {
                string path = createdAssetPaths[index];

                try
                {
                    if (AssetDatabase.LoadMainAssetAtPath(path) == null)
                    {
                        continue;
                    }

                    string currentGuid =
                        AssetDatabase.AssetPathToGUID(path);

                    if (!createdAssetGuids.TryGetValue(
                            path,
                            out string createdGuid) ||
                        string.IsNullOrEmpty(createdGuid) ||
                        !string.Equals(
                            currentGuid,
                            createdGuid,
                            StringComparison.Ordinal))
                    {
                        manualRecoveryPaths.Add(path);
                        continue;
                    }

                    if (!AssetDatabase.DeleteAsset(path))
                    {
                        manualRecoveryPaths.Add(path);
                    }
                }
                catch
                {
                    manualRecoveryPaths.Add(path);
                }
            }

            createdFolderPaths.Sort(
                delegate(string left, string right)
                {
                    int lengthComparison =
                        right.Length.CompareTo(left.Length);

                    return lengthComparison != 0
                        ? lengthComparison
                        : string.Compare(
                            left,
                            right,
                            StringComparison.Ordinal);
                });

            for (int index = 0;
                 index < createdFolderPaths.Count;
                 index++)
            {
                string path = createdFolderPaths[index];

                try
                {
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        continue;
                    }

                    string currentGuid =
                        AssetDatabase.AssetPathToGUID(path);

                    if (!createdFolderGuids.TryGetValue(
                            path,
                            out string createdGuid) ||
                        string.IsNullOrEmpty(createdGuid) ||
                        !string.Equals(
                            currentGuid,
                            createdGuid,
                            StringComparison.Ordinal) ||
                        !IsFolderEmpty(path))
                    {
                        manualRecoveryPaths.Add(path);
                        continue;
                    }

                    if (!AssetDatabase.DeleteAsset(path))
                    {
                        manualRecoveryPaths.Add(path);
                    }
                }
                catch
                {
                    manualRecoveryPaths.Add(path);
                }
            }

            try
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch
            {
                manualRecoveryPaths.Add("AssetDatabase refresh");
            }

            manualRecoveryPaths.Sort(StringComparer.Ordinal);

            return new EchoLaunchSetupRollbackResult(
                manualRecoveryPaths.Count == 0,
                manualRecoveryPaths);
        }

        private static bool IsFolderEmpty(string projectPath)
        {
            string absolutePath =
                Path.GetFullPath(projectPath);

            if (!Directory.Exists(absolutePath))
            {
                return true;
            }

            string[] entries =
                Directory.GetFileSystemEntries(absolutePath);

            for (int index = 0; index < entries.Length; index++)
            {
                if (!entries[index].EndsWith(
                        ".meta",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
