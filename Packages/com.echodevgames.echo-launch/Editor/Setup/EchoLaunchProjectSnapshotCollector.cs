using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchProjectSnapshotCollector :
        IEchoLaunchSetupSnapshotSource
    {
        public EchoLaunchProjectSnapshot Collect(
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
                AddFact(facts, paths.ProjectRootPath, false);
                AddFact(facts, paths.ConfigurationFolderPath, false);
                AddFact(facts, paths.PrefabsFolderPath, false);
                AddFact(facts, paths.ScenesFolderPath, false);
                AddFact(facts, paths.ConfigurationAssetPath, false);
                AddFact(facts, paths.StartupSequenceAssetPath, false);
                AddFact(facts, paths.LaunchDestinationAssetPath, false);
                AddFact(facts, paths.SplashSequenceAssetPath, false);
                AddFact(facts, paths.RootPrefabPath, false);
                AddFact(facts, paths.BootScenePath, true);
            }

            AddOptionalFact(facts, request.DestinationScenePath, false);
            AddOptionalFact(facts, request.SelectedConfigurationPath, false);
            AddOptionalFact(facts, request.SelectedStartupSequencePath, false);
            AddOptionalFact(facts, request.SelectedLaunchDestinationPath, false);
            AddOptionalFact(facts, request.SelectedSplashSequencePath, false);
            AddOptionalFact(facts, request.SelectedRootPrefabPath, false);

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
            string path,
            bool inspectScene)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                AddFact(facts, path, inspectScene);
            }
        }

        private static void AddFact(
            List<EchoLaunchProjectAssetFact> facts,
            string path,
            bool inspectScene)
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

            facts.Add(CreateAssetFact(normalized, inspectScene));
        }

        private static EchoLaunchProjectAssetFact CreateAssetFact(
            string path,
            bool inspectScene = false)
        {
            bool isFolder = AssetDatabase.IsValidFolder(path);
            Type mainType = AssetDatabase.GetMainAssetTypeAtPath(path);
            bool exists = isFolder || mainType != null;
            string guid = exists
                ? AssetDatabase.AssetPathToGUID(path)
                : string.Empty;

            int? schemaVersion = null;
            bool hasRepairEvidence = false;
            string stableId = string.Empty;
            string startupSequencePath = string.Empty;
            string launchDestinationPath = string.Empty;
            string splashSequencePath = string.Empty;
            string destinationScenePath = string.Empty;
            string destinationDisplayName = string.Empty;
            string prefabAssetType = string.Empty;
            string prefabSourcePath = string.Empty;
            bool prefabLineageMatchesTemplate = false;
            int? rootCount = null;
            string rootConfigurationPath = string.Empty;
            bool sceneInspectionSafe = true;
            string sceneInspectionMessage = string.Empty;
            bool sceneWasOpen = false;

            if (mainType == typeof(EchoLaunchConfiguration))
            {
                EchoLaunchConfiguration configuration =
                    AssetDatabase.LoadAssetAtPath<EchoLaunchConfiguration>(path);

                if (configuration != null)
                {
                    SerializedObject serialized =
                        new SerializedObject(configuration);
                    schemaVersion = ReadInt(serialized, "schemaVersion");
                    stableId = ReadString(serialized, "configurationId");
                    startupSequencePath = ReadObjectPath(
                        serialized,
                        "startupSequence");
                    launchDestinationPath = ReadObjectPath(
                        serialized,
                        "initialDestination");
                    splashSequencePath = ReadObjectPath(
                        serialized,
                        "splashSequence");
                    hasRepairEvidence = true;
                }
            }
            else if (mainType == typeof(LaunchDestination))
            {
                LaunchDestination destination =
                    AssetDatabase.LoadAssetAtPath<LaunchDestination>(path);

                if (destination != null)
                {
                    SerializedObject serialized =
                        new SerializedObject(destination);
                    schemaVersion = ReadInt(serialized, "schemaVersion");
                    stableId = ReadString(serialized, "destinationId");
                    destinationScenePath = ReadString(serialized, "scenePath");
                    destinationDisplayName = ReadString(serialized, "displayName");
                    hasRepairEvidence = true;
                }
            }
            else if (mainType == typeof(StartupSequence))
            {
                UnityEngine.Object sequence =
                    AssetDatabase.LoadMainAssetAtPath(path);
                if (sequence != null)
                {
                    SerializedObject serialized = new SerializedObject(sequence);
                    schemaVersion = ReadInt(serialized, "schemaVersion");
                    stableId = ReadString(serialized, "sequenceId");
                    hasRepairEvidence = true;
                }
            }
            else if (mainType == typeof(SplashSequence))
            {
                UnityEngine.Object sequence =
                    AssetDatabase.LoadMainAssetAtPath(path);
                if (sequence != null)
                {
                    SerializedObject serialized = new SerializedObject(sequence);
                    schemaVersion = ReadInt(serialized, "schemaVersion");
                    stableId = ReadString(serialized, "sequenceId");
                    hasRepairEvidence = true;
                }
            }
            else if (mainType == typeof(GameObject))
            {
                GameObject prefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    prefabAssetType =
                        PrefabUtility.GetPrefabAssetType(prefab).ToString();
                    EchoLaunchRoot[] roots =
                        prefab.GetComponentsInChildren<EchoLaunchRoot>(true);
                    rootCount = roots.Length;
                    rootConfigurationPath = roots.Length == 1
                        ? ReadRootConfigurationPath(roots[0])
                        : string.Empty;
                    prefabSourcePath = GetDirectPrefabSourcePath(prefab);
                    prefabLineageMatchesTemplate =
                        LineageContainsTemplate(prefab);
                    hasRepairEvidence = true;
                }
            }
            else if (mainType == typeof(SceneAsset) && inspectScene)
            {
                CollectSceneEvidence(
                    path,
                    out rootCount,
                    out prefabSourcePath,
                    out rootConfigurationPath,
                    out sceneInspectionSafe,
                    out sceneInspectionMessage,
                    out sceneWasOpen);
                hasRepairEvidence = sceneInspectionSafe;
            }

            return new EchoLaunchProjectAssetFact(
                path,
                exists,
                isFolder,
                guid,
                mainType == null ? string.Empty : mainType.FullName,
                schemaVersion,
                hasRepairEvidence,
                stableId,
                startupSequencePath,
                launchDestinationPath,
                splashSequencePath,
                destinationScenePath,
                destinationDisplayName,
                prefabAssetType,
                prefabSourcePath,
                prefabLineageMatchesTemplate,
                rootCount,
                rootConfigurationPath,
                sceneInspectionSafe,
                sceneInspectionMessage,
                sceneWasOpen);
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

                if (!IsAutomaticDiscoveryCandidatePath(path))
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

                if (!IsAutomaticDiscoveryCandidatePath(path))
                {
                    continue;
                }

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null &&
                    prefab.GetComponentsInChildren<EchoLaunchRoot>(true).Length > 0)
                {
                    result.Add(CreateAssetFact(path));
                }
            }

            return result;
        }

        private static bool IsAutomaticDiscoveryCandidatePath(
            string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            if (!normalized.StartsWith(
                    "Assets/",
                    StringComparison.Ordinal))
            {
                return false;
            }

            return !string.Equals(
                       normalized,
                       "Assets/Samples",
                       StringComparison.OrdinalIgnoreCase) &&
                   !normalized.StartsWith(
                       "Assets/Samples/",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static int? ReadInt(
            SerializedObject serialized,
            string propertyName)
        {
            SerializedProperty property =
                serialized.FindProperty(propertyName);
            return property == null ? (int?)null : property.intValue;
        }

        private static string ReadString(
            SerializedObject serialized,
            string propertyName)
        {
            SerializedProperty property =
                serialized.FindProperty(propertyName);
            return property == null ? string.Empty : property.stringValue;
        }

        private static string ReadObjectPath(
            SerializedObject serialized,
            string propertyName)
        {
            SerializedProperty property =
                serialized.FindProperty(propertyName);
            return property == null || property.objectReferenceValue == null
                ? string.Empty
                : AssetDatabase.GetAssetPath(property.objectReferenceValue);
        }

        private static string ReadRootConfigurationPath(EchoLaunchRoot root)
        {
            if (root == null)
            {
                return string.Empty;
            }

            SerializedObject serialized = new SerializedObject(root);
            return ReadObjectPath(serialized, "configuration");
        }

        private static string GetDirectPrefabSourcePath(GameObject prefab)
        {
            if (prefab == null)
            {
                return string.Empty;
            }

            GameObject source =
                PrefabUtility.GetCorrespondingObjectFromSource(prefab);
            return source == null
                ? string.Empty
                : AssetDatabase.GetAssetPath(source);
        }

        private static bool LineageContainsTemplate(GameObject prefab)
        {
            GameObject current = prefab;
            int guard = 0;

            while (current != null && guard++ < 64)
            {
                string path = AssetDatabase.GetAssetPath(current);
                if (string.Equals(
                        path,
                        EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                        StringComparison.Ordinal))
                {
                    return true;
                }

                current =
                    PrefabUtility.GetCorrespondingObjectFromSource(current);
            }

            return false;
        }

        private static void CollectSceneEvidence(
            string path,
            out int? rootCount,
            out string prefabSourcePath,
            out string rootConfigurationPath,
            out bool inspectionSafe,
            out string inspectionMessage,
            out bool sceneWasOpen)
        {
            rootCount = null;
            prefabSourcePath = string.Empty;
            rootConfigurationPath = string.Empty;
            inspectionSafe = true;
            inspectionMessage = string.Empty;

            Scene previousActive = SceneManager.GetActiveScene();
            Scene scene = SceneManager.GetSceneByPath(path);
            bool openedByCollector = !scene.IsValid() || !scene.isLoaded;
            sceneWasOpen = !openedByCollector;

            if (!openedByCollector && scene.isDirty)
            {
                inspectionSafe = false;
                inspectionMessage =
                    "The Boot scene is open with unsaved changes. Save or close it before repair planning.";
                return;
            }

            try
            {
                if (openedByCollector)
                {
                    scene = EditorSceneManager.OpenScene(
                        path,
                        OpenSceneMode.Additive);
                }

                List<EchoLaunchRoot> roots = new List<EchoLaunchRoot>();
                GameObject[] sceneRoots = scene.GetRootGameObjects();
                for (int index = 0; index < sceneRoots.Length; index++)
                {
                    roots.AddRange(
                        sceneRoots[index]
                            .GetComponentsInChildren<EchoLaunchRoot>(true));
                }

                rootCount = roots.Count;
                if (roots.Count == 1)
                {
                    prefabSourcePath =
                        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(
                            roots[0].gameObject);
                    rootConfigurationPath =
                        ReadRootConfigurationPath(roots[0]);
                }
            }
            catch (Exception exception)
            {
                inspectionSafe = false;
                inspectionMessage = exception.Message;
            }
            finally
            {
                if (openedByCollector && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }

                if (previousActive.IsValid() && previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
            }
        }
    }
}
