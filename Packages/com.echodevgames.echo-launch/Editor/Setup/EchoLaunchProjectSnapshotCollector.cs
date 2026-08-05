
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchProjectSnapshotCollector
    {
        internal EchoLaunchProjectSnapshot Collect(
            EchoLaunchSetupRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            List<EchoLaunchProjectAssetFact> facts =
                new List<EchoLaunchProjectAssetFact>();

            if (EchoLaunchSetupPathUtility.TryCreatePathSet(
                    request.ProjectRootPath,
                    request.BootScenePath,
                    out EchoLaunchSetupPathSet paths,
                    out _))
            {
                AddFact(facts, paths.ProjectRootPath);
                AddFact(facts, paths.ConfigurationFolderPath);
                AddFact(facts, paths.PrefabsFolderPath);
                AddFact(facts, paths.ScenesFolderPath);
                AddFact(facts, paths.ConfigurationAssetPath);
                AddFact(facts, paths.StartupSequenceAssetPath);
                AddFact(facts, paths.LaunchDestinationAssetPath);
                AddFact(facts, paths.SplashSequenceAssetPath);
                AddFact(facts, paths.RootPrefabPath);
                AddFact(facts, paths.BootScenePath);
            }

            AddOptionalFact(facts, request.DestinationScenePath);
            AddOptionalFact(facts, request.SelectedConfigurationPath);
            AddOptionalFact(facts, request.SelectedStartupSequencePath);
            AddOptionalFact(facts, request.SelectedLaunchDestinationPath);
            AddOptionalFact(facts, request.SelectedSplashSequencePath);
            AddOptionalFact(facts, request.SelectedRootPrefabPath);

            bool templateAvailable =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath) != null;

            string templateGuid =
                AssetDatabase.AssetPathToGUID(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath);

            Dictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> candidates =
                    new Dictionary<
                        EchoLaunchSetupAssetRole,
                        IEnumerable<EchoLaunchProjectAssetFact>>();

            candidates[EchoLaunchSetupAssetRole.Configuration] =
                CollectTypedCandidates<EchoLaunchConfiguration>();

            candidates[EchoLaunchSetupAssetRole.StartupSequence] =
                CollectTypedCandidates<StartupSequence>();

            candidates[EchoLaunchSetupAssetRole.LaunchDestination] =
                CollectTypedCandidates<LaunchDestination>();

            candidates[EchoLaunchSetupAssetRole.SplashSequence] =
                CollectTypedCandidates<SplashSequence>();

            candidates[EchoLaunchSetupAssetRole.RootPrefab] =
                CollectRootPrefabCandidates();

            EditorBuildSettingsScene[] editorScenes = EditorBuildSettings.scenes;

            List<EchoLaunchBuildSettingsSceneFact> buildScenes =
                new List<EchoLaunchBuildSettingsSceneFact>(editorScenes.Length);

            for (int index = 0; index < editorScenes.Length; index++)
            {
                buildScenes.Add(
                    new EchoLaunchBuildSettingsSceneFact(
                        editorScenes[index].path,
                        editorScenes[index].enabled,
                        index));
            }

            return new EchoLaunchProjectSnapshot(
                facts,
                buildScenes,
                templateAvailable,
                templateGuid,
                candidates);
        }

        private static void AddOptionalFact(
            List<EchoLaunchProjectAssetFact> facts,
            string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                AddFact(facts, path);
            }
        }

        private static void AddFact(
            List<EchoLaunchProjectAssetFact> facts,
            string path)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            for (int index = 0; index < facts.Count; index++)
            {
                if (string.Equals(
                        facts[index].Path,
                        normalized,
                        StringComparison.Ordinal))
                {
                    return;
                }
            }

            facts.Add(CreateAssetFact(normalized));
        }

        private static EchoLaunchProjectAssetFact CreateAssetFact(string path)
        {
            bool isFolder = AssetDatabase.IsValidFolder(path);
            Type mainType = AssetDatabase.GetMainAssetTypeAtPath(path);
            bool exists = isFolder || mainType != null;

            string guid =
                exists
                    ? AssetDatabase.AssetPathToGUID(path)
                    : string.Empty;

            int? schemaVersion = null;

            if (mainType == typeof(EchoLaunchConfiguration))
            {
                EchoLaunchConfiguration configuration =
                    AssetDatabase.LoadAssetAtPath<EchoLaunchConfiguration>(path);

                if (configuration != null)
                {
                    schemaVersion = configuration.SchemaVersion;
                }
            }

            return new EchoLaunchProjectAssetFact(
                path,
                exists,
                isFolder,
                guid,
                mainType == null ? string.Empty : mainType.FullName,
                schemaVersion);
        }

        private static List<EchoLaunchProjectAssetFact>
            CollectTypedCandidates<T>()
            where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            List<EchoLaunchProjectAssetFact> result =
                new List<EchoLaunchProjectAssetFact>(guids.Length);

            for (int index = 0; index < guids.Length; index++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[index]);

                if (!path.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    continue;
                }

                if (AssetDatabase.LoadAssetAtPath<T>(path) != null)
                {
                    result.Add(CreateAssetFact(path));
                }
            }

            return result;
        }

        private static List<EchoLaunchProjectAssetFact>
            CollectRootPrefabCandidates()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab EchoLaunch");
            List<EchoLaunchProjectAssetFact> result =
                new List<EchoLaunchProjectAssetFact>();

            for (int index = 0; index < guids.Length; index++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[index]);

                if (!path.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    continue;
                }

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null &&
                    prefab.GetComponent<EchoLaunchRoot>() != null)
                {
                    result.Add(CreateAssetFact(path));
                }
            }

            return result;
        }
    }
}
