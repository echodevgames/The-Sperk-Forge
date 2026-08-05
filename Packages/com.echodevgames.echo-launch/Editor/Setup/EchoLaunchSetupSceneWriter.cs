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
