using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal interface IEchoLaunchSetupSceneWriter
    {
        void CreateBootScene(
            string scenePath,
            string rootPrefabPath,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log);

        bool RepairBootSceneWithRoot(
            string scenePath,
            string rootPrefabPath,
            EchoLaunchSetupExecutionLog log);
    }

    internal sealed class EchoLaunchSetupSceneWriter :
        IEchoLaunchSetupSceneWriter
    {
        public void CreateBootScene(
            string scenePath,
            string rootPrefabPath,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            string normalizedScenePath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(scenePath);

            if (AssetDatabase.LoadMainAssetAtPath(normalizedScenePath) != null ||
                AssetDatabase.IsValidFolder(normalizedScenePath))
            {
                throw new InvalidOperationException(
                    "Create-only setup cannot overwrite Boot scene '" +
                    normalizedScenePath +
                    "'.");
            }

            GameObject rootPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(rootPrefabPath);

            if (rootPrefab == null)
            {
                throw new InvalidOperationException(
                    "The resolved project root prefab is unavailable.");
            }

            List<SceneState> existingScenes = CaptureSceneState();
            Scene previousActive = SceneManager.GetActiveScene();
            Scene temporaryScene = default(Scene);
            bool sceneWasSaved = false;

            using (EchoLaunchUntitledSceneLease.Acquire())
            try
            {
                temporaryScene =
                    EditorSceneManager.NewScene(
                        NewSceneSetup.EmptyScene,
                        NewSceneMode.Additive);

                GameObject instance =
                    PrefabUtility.InstantiatePrefab(
                        rootPrefab,
                        temporaryScene)
                    as GameObject;

                if (instance == null)
                {
                    throw new InvalidOperationException(
                        "Unity could not instantiate the project root prefab.");
                }

                if (instance.GetComponentsInChildren<EchoLaunchRoot>(true).Length != 1)
                {
                    throw new InvalidOperationException(
                        "The Boot scene root instance is invalid.");
                }

                if (!EditorSceneManager.SaveScene(
                        temporaryScene,
                        normalizedScenePath,
                        false))
                {
                    throw new InvalidOperationException(
                        "Unity could not save the Boot scene.");
                }

                journal.RecordCreatedAsset(normalizedScenePath);
                sceneWasSaved = true;
            }
            finally
            {
                if (temporaryScene.IsValid() && temporaryScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(temporaryScene, true);
                }

                if (previousActive.IsValid() && previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
            }

            if (sceneWasSaved)
            {
                AssetDatabase.ImportAsset(
                    normalizedScenePath,
                    ImportAssetOptions.ForceSynchronousImport);

                if (AssetDatabase.GetMainAssetTypeAtPath(normalizedScenePath) !=
                    typeof(SceneAsset))
                {
                    throw new InvalidOperationException(
                        "The Boot scene asset did not import correctly.");
                }

                log.Add(
                    EchoLaunchSetupChangeKind.CreatedScene,
                    normalizedScenePath,
                    "Created Boot scene with one project root instance.");
            }

            VerifySceneState(existingScenes, previousActive);
        }

        public bool RepairBootSceneWithRoot(
            string scenePath,
            string rootPrefabPath,
            EchoLaunchSetupExecutionLog log)
        {
            string normalizedScenePath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(scenePath);
            string normalizedRootPrefabPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(rootPrefabPath);
            GameObject rootPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    normalizedRootPrefabPath);
            if (AssetDatabase.GetMainAssetTypeAtPath(normalizedScenePath) !=
                    typeof(SceneAsset) ||
                rootPrefab == null)
            {
                throw new InvalidOperationException(
                    "The Boot scene or verified root prefab is unavailable for repair.");
            }

            VerifyRootPrefabContract(rootPrefab);

            Scene previousActive = SceneManager.GetActiveScene();
            Scene scene = SceneManager.GetSceneByPath(normalizedScenePath);
            bool openedByRepair = !scene.IsValid() || !scene.isLoaded;

            if (!openedByRepair && scene.isDirty)
            {
                throw new InvalidOperationException(
                    "Save or close the dirty Boot scene before repair.");
            }

            try
            {
                if (openedByRepair)
                {
                    scene = EditorSceneManager.OpenScene(
                        normalizedScenePath,
                        OpenSceneMode.Additive);
                }

                List<EchoLaunchRoot> roots = FindRoots(scene);
                if (roots.Count == 1)
                {
                    string sourcePath =
                        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(
                            roots[0].gameObject);
                    if (!string.Equals(
                            sourcePath,
                            normalizedRootPrefabPath,
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "The existing Boot scene root is unpacked or comes from another prefab.");
                    }

                    return false;
                }

                if (roots.Count != 0)
                {
                    throw new InvalidOperationException(
                        "Repair cannot reconcile a Boot scene with multiple EchoLaunchRoot components.");
                }

                GameObject instance =
                    PrefabUtility.InstantiatePrefab(rootPrefab, scene)
                    as GameObject;
                if (instance == null ||
                    instance.GetComponentsInChildren<EchoLaunchRoot>(true).Length != 1)
                {
                    throw new InvalidOperationException(
                        "Unity could not add one verified root prefab instance to the Boot scene.");
                }

                if (!EditorSceneManager.SaveScene(
                        scene,
                        normalizedScenePath,
                        false))
                {
                    throw new InvalidOperationException(
                        "Unity could not save the repaired Boot scene.");
                }

                log.Add(
                    EchoLaunchSetupChangeKind.RepairedScene,
                    normalizedScenePath,
                    "Added one verified project-root prefab instance to the canonical Boot scene.");
                return true;
            }
            finally
            {
                if (openedByRepair && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }

                if (previousActive.IsValid() && previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
            }
        }


        private static void VerifyRootPrefabContract(GameObject rootPrefab)
        {
            if (PrefabUtility.GetPrefabAssetType(rootPrefab) !=
                    PrefabAssetType.Variant ||
                !LineageContains(
                    rootPrefab,
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath))
            {
                throw new InvalidOperationException(
                    "Boot scene repair requires a verified root-prefab variant whose lineage reaches the package template.");
            }

            if (rootPrefab.GetComponentsInChildren<EchoLaunchRoot>(true).Length != 1)
            {
                throw new InvalidOperationException(
                    "Boot scene repair requires a root prefab with exactly one EchoLaunchRoot.");
            }
        }

        private static bool LineageContains(
            GameObject prefab,
            string expectedPath)
        {
            string normalizedExpected =
                EchoLaunchSetupPathUtility.NormalizeSeparators(expectedPath);
            GameObject current = prefab;
            int guard = 0;
            while (current != null && guard++ < 64)
            {
                if (string.Equals(
                        AssetDatabase.GetAssetPath(current),
                        normalizedExpected,
                        StringComparison.Ordinal))
                {
                    return true;
                }

                current =
                    PrefabUtility.GetCorrespondingObjectFromSource(current);
            }

            return false;
        }

        private static List<EchoLaunchRoot> FindRoots(Scene scene)
        {
            List<EchoLaunchRoot> result = new List<EchoLaunchRoot>();
            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                result.AddRange(
                    roots[index].GetComponentsInChildren<EchoLaunchRoot>(true));
            }

            return result;
        }

        private static List<SceneState> CaptureSceneState()
        {
            List<SceneState> result = new List<SceneState>();

            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                Scene scene = SceneManager.GetSceneAt(index);

                result.Add(
                    new SceneState(
                        scene,
                        scene.isLoaded,
                        scene.isDirty));
            }

            return result;
        }

        private static void VerifySceneState(
            List<SceneState> expected,
            Scene expectedActive)
        {
            if (SceneManager.sceneCount != expected.Count)
            {
                throw new InvalidOperationException(
                    "Boot scene creation changed the open-scene count.");
            }

            for (int index = 0; index < expected.Count; index++)
            {
                SceneState state = expected[index];
                Scene actual = SceneManager.GetSceneAt(index);

                if (actual != state.Scene ||
                    actual.isLoaded != state.WasLoaded ||
                    actual.isDirty != state.WasDirty)
                {
                    throw new InvalidOperationException(
                        "Boot scene creation changed pre-existing scene state.");
                }
            }

            if (expectedActive.IsValid() &&
                SceneManager.GetActiveScene() != expectedActive)
            {
                throw new InvalidOperationException(
                    "Boot scene creation changed the active scene.");
            }
        }

        private sealed class SceneState
        {
            internal SceneState(
                Scene scene,
                bool wasLoaded,
                bool wasDirty)
            {
                Scene = scene;
                WasLoaded = wasLoaded;
                WasDirty = wasDirty;
            }

            internal Scene Scene { get; }
            internal bool WasLoaded { get; }
            internal bool WasDirty { get; }
        }
    }
}
