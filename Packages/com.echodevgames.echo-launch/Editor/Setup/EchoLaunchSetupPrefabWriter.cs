using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal interface IEchoLaunchSetupPrefabWriter
    {
        void CreateRootVariant(
            string templatePath,
            string targetPath,
            string configurationPath,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log);
    }

    internal sealed class EchoLaunchSetupPrefabWriter :
        IEchoLaunchSetupPrefabWriter
    {
        public void CreateRootVariant(
            string templatePath,
            string targetPath,
            string configurationPath,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            string normalizedTarget =
                EchoLaunchSetupPathUtility.NormalizeSeparators(targetPath);

            if (AssetDatabase.LoadMainAssetAtPath(normalizedTarget) != null ||
                AssetDatabase.IsValidFolder(normalizedTarget))
            {
                throw new InvalidOperationException(
                    "Create-only setup cannot overwrite prefab '" +
                    normalizedTarget +
                    "'.");
            }

            GameObject template =
                AssetDatabase.LoadAssetAtPath<GameObject>(templatePath);

            EchoLaunchConfiguration configuration =
                AssetDatabase.LoadAssetAtPath<EchoLaunchConfiguration>(
                    configurationPath);

            if (template == null)
            {
                throw new InvalidOperationException(
                    "The package root prefab template is unavailable.");
            }

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "The resolved launch configuration is unavailable.");
            }

            Scene previousActive = SceneManager.GetActiveScene();
            Scene temporaryScene = default(Scene);
            GameObject instance = null;

            using (EchoLaunchUntitledSceneLease.Acquire())
            try
            {
                temporaryScene =
                    EditorSceneManager.NewScene(
                        NewSceneSetup.EmptyScene,
                        NewSceneMode.Additive);

                instance =
                    PrefabUtility.InstantiatePrefab(
                        template,
                        temporaryScene)
                    as GameObject;

                if (instance == null)
                {
                    throw new InvalidOperationException(
                        "Unity could not instantiate the package root template.");
                }

                EchoLaunchRoot root =
                    instance.GetComponent<EchoLaunchRoot>();

                if (root == null)
                {
                    throw new InvalidOperationException(
                        "The package root template contains no EchoLaunchRoot.");
                }

                SerializedObject serialized = new SerializedObject(root);
                SerializedProperty configurationProperty =
                    serialized.FindProperty("configuration");

                if (configurationProperty == null)
                {
                    throw new InvalidOperationException(
                        "EchoLaunchRoot.configuration is unavailable.");
                }

                configurationProperty.objectReferenceValue = configuration;
                serialized.ApplyModifiedPropertiesWithoutUndo();

                GameObject saved =
                    PrefabUtility.SaveAsPrefabAssetAndConnect(
                        instance,
                        normalizedTarget,
                        InteractionMode.AutomatedAction);

                if (saved == null)
                {
                    throw new InvalidOperationException(
                        "Unity did not save the project root prefab variant.");
                }

                journal.RecordCreatedAsset(normalizedTarget);

                AssetDatabase.ImportAsset(
                    normalizedTarget,
                    ImportAssetOptions.ForceSynchronousImport);

                GameObject loaded =
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        normalizedTarget);

                if (loaded == null ||
                    PrefabUtility.GetPrefabAssetType(loaded) !=
                    PrefabAssetType.Variant)
                {
                    throw new InvalidOperationException(
                        "The project root asset is not a prefab variant.");
                }

                if (loaded.GetComponentsInChildren<EchoLaunchRoot>(true).Length != 1)
                {
                    throw new InvalidOperationException(
                        "The project root variant must contain exactly one EchoLaunchRoot.");
                }

                EchoLaunchRoot loadedRoot =
                    loaded.GetComponentInChildren<EchoLaunchRoot>(true);

                SerializedObject loadedSerialized =
                    new SerializedObject(loadedRoot);

                SerializedProperty loadedConfiguration =
                    loadedSerialized.FindProperty("configuration");

                if (loadedConfiguration == null ||
                    loadedConfiguration.objectReferenceValue != configuration)
                {
                    throw new InvalidOperationException(
                        "The project root variant did not retain its configuration.");
                }

                log.Add(
                    EchoLaunchSetupChangeKind.CreatedPrefabVariant,
                    normalizedTarget,
                    "Created project-owned root prefab variant.");
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
        }
    }
}
