using System;
using System.Collections.Generic;
using System.IO;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupRepairIntegrationTests
    {
        private const string RepairMarkerName =
            "__FL_M5_03_UnrelatedMarker";

        [Test]
        public void ApprovedDriftRepairsOnceThenSettlesWithoutIdentityChanges()
        {
            string id = Guid.NewGuid().ToString("N");
            string projectRoot =
                "Assets/__EchoLaunch_FL_M5_03_Repair_" + id;
            string destinationFolder =
                "Assets/__EchoLaunch_FL_M5_03_Destination_" + id;
            string destinationScenePath =
                destinationFolder + "/Destination.unity";
            EchoLaunchSetupPathSet paths =
                new EchoLaunchSetupPathSet(
                    projectRoot,
                    projectRoot + "/Scenes/Boot.unity");
            EchoLaunchSetupRequest request =
                new EchoLaunchSetupRequest(
                    projectRoot,
                    paths.BootScenePath,
                    destinationScenePath,
                    true,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);
            EditorBuildSettingsScene[] originalBuildSettings =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);
            List<string> backupDirectories = new List<string>();

            try
            {
                CreateDestination(destinationFolder, destinationScenePath);
                EchoLaunchSetupPlan createPlan = Refresh(request);
                EchoLaunchSetupApplyResult applyResult =
                    new EchoLaunchSetupApplyService().Apply(
                        new EchoLaunchSetupApplyRequest(
                            createPlan,
                            true,
                            false));
                Assert.That(
                    applyResult.Status,
                    Is.EqualTo(EchoLaunchSetupApplyStatus.Succeeded));

                Dictionary<string, string> guidsBefore =
                    CaptureGuids(paths);
                string configurationIdBefore = ReadString(
                    paths.ConfigurationAssetPath,
                    "configurationId");
                string destinationIdBefore = ReadString(
                    paths.LaunchDestinationAssetPath,
                    "destinationId");
                byte[] destinationSceneBytes =
                    ReadProjectBytes(destinationScenePath);
                byte[] destinationSceneMetaBytes =
                    ReadProjectBytes(destinationScenePath + ".meta");
                GameObject packageTemplate =
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        EchoLaunchSetupPathSet
                            .PackageRootPrefabTemplatePath);
                Assert.That(packageTemplate, Is.Not.Null);
                bool packageTemplateDirtyBefore =
                    EditorUtility.IsDirty(packageTemplate);

                IntroduceApprovedDrift(paths);

                EchoLaunchSetupPlan repairPlan = Refresh(request);
                Assert.That(repairPlan.HasRepairs, Is.True);
                Assert.That(
                    repairPlan.CountDisposition(
                        EchoLaunchSetupOperationDisposition.Repair),
                    Is.GreaterThanOrEqualTo(5));
                Assert.That(
                    EchoLaunchSetupApplyService.EvaluateEligibility(
                        repairPlan,
                        false).CanApply,
                    Is.False);

                EchoLaunchSetupRepairService service =
                    new EchoLaunchSetupRepairService();
                EchoLaunchSetupRepairResult first =
                    service.Repair(
                        new EchoLaunchSetupRepairRequest(
                            repairPlan,
                            true,
                            false));
                if (!string.IsNullOrEmpty(first.BackupDirectory))
                {
                    backupDirectories.Add(first.BackupDirectory);
                }

                Assert.That(
                    first.Status,
                    Is.EqualTo(EchoLaunchSetupRepairStatus.Succeeded));
                Assert.That(first.BackupDirectory, Is.Empty);
                Assert.That(first.ManualRecoveryPaths, Is.Empty);
                Assert.That(first.RepairedPaths.Count, Is.GreaterThanOrEqualTo(5));
                Assert.That(CaptureGuids(paths), Is.EqualTo(guidsBefore));
                Assert.That(
                    ReadString(
                        paths.ConfigurationAssetPath,
                        "configurationId"),
                    Is.EqualTo(configurationIdBefore));
                Assert.That(
                    ReadString(
                        paths.LaunchDestinationAssetPath,
                        "destinationId"),
                    Is.EqualTo(destinationIdBefore));
                Assert.That(
                    ReadProjectBytes(destinationScenePath),
                    Is.EqualTo(destinationSceneBytes));
                Assert.That(
                    ReadProjectBytes(destinationScenePath + ".meta"),
                    Is.EqualTo(destinationSceneMetaBytes));
                Assert.That(
                    EditorUtility.IsDirty(packageTemplate),
                    Is.EqualTo(packageTemplateDirtyBefore));

                EchoLaunchSetupRepairResult second =
                    service.Repair(
                        new EchoLaunchSetupRepairRequest(
                            Refresh(request),
                            true,
                            false));
                EchoLaunchSetupRepairResult third =
                    service.Repair(
                        new EchoLaunchSetupRepairRequest(
                            Refresh(request),
                            true,
                            false));

                Assert.That(
                    second.Status,
                    Is.EqualTo(EchoLaunchSetupRepairStatus.NoChanges));
                Assert.That(
                    third.Status,
                    Is.EqualTo(EchoLaunchSetupRepairStatus.NoChanges));
                Assert.That(CountBootEntries(paths.BootScenePath), Is.EqualTo(1));
                Assert.That(CountBootRoots(paths.BootScenePath), Is.EqualTo(1));
                Assert.That(
                    BootSceneContainsObject(
                        paths.BootScenePath,
                        RepairMarkerName),
                    Is.True);
            }
            finally
            {
                EditorBuildSettings.scenes =
                    EchoLaunchSetupBuildSettingsWriter.Clone(
                        originalBuildSettings);
                AssetDatabase.DeleteAsset(projectRoot);
                AssetDatabase.DeleteAsset(destinationFolder);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                for (int index = 0; index < backupDirectories.Count; index++)
                {
                    string absolute = ProjectAbsolute(
                        backupDirectories[index]);
                    if (Directory.Exists(absolute))
                    {
                        Directory.Delete(absolute, true);
                    }
                }
            }
        }

        [Test]
        public void InjectedMixedCreateAndRepairFailureRestoresExactState()
        {
            string id = Guid.NewGuid().ToString("N");
            string projectRoot =
                "Assets/__EchoLaunch_FL_M5_03_Rollback_" + id;
            string destinationFolder =
                "Assets/__EchoLaunch_FL_M5_03_RollbackDestination_" + id;
            string destinationScenePath =
                destinationFolder + "/Destination.unity";
            EchoLaunchSetupPathSet paths =
                new EchoLaunchSetupPathSet(
                    projectRoot,
                    projectRoot + "/Scenes/Boot.unity");
            EchoLaunchSetupRequest createRequest =
                new EchoLaunchSetupRequest(
                    projectRoot,
                    paths.BootScenePath,
                    destinationScenePath,
                    false,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);
            EchoLaunchSetupRequest repairRequest =
                new EchoLaunchSetupRequest(
                    projectRoot,
                    paths.BootScenePath,
                    destinationScenePath,
                    true,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);
            EditorBuildSettingsScene[] originalBuildSettings =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);

            try
            {
                CreateDestination(destinationFolder, destinationScenePath);
                EchoLaunchSetupApplyResult apply =
                    new EchoLaunchSetupApplyService().Apply(
                        new EchoLaunchSetupApplyRequest(
                            Refresh(createRequest),
                            true,
                            false));
                Assert.That(
                    apply.Status,
                    Is.EqualTo(EchoLaunchSetupApplyStatus.Succeeded));

                IntroduceConfigurationAndPrefabDrift(paths);

                byte[] configurationBytes = ReadProjectBytes(
                    paths.ConfigurationAssetPath);
                byte[] configurationMetaBytes = ReadProjectBytes(
                    paths.ConfigurationAssetPath + ".meta");
                byte[] prefabBytes = ReadProjectBytes(paths.RootPrefabPath);
                byte[] prefabMetaBytes = ReadProjectBytes(
                    paths.RootPrefabPath + ".meta");

                EchoLaunchSetupPlan repairPlan = Refresh(repairRequest);
                Assert.That(repairPlan.HasRepairs, Is.True);
                Assert.That(repairPlan.HasCreates, Is.True);
                Assert.That(
                    EchoLaunchSetupTestFactory.FindOperation(
                        repairPlan,
                        EchoLaunchSetupOperationKind.ResolveSplashSequence)
                        .Disposition,
                    Is.EqualTo(
                        EchoLaunchSetupOperationDisposition.Create));
                Assert.That(
                    EchoLaunchSetupTestFactory.FindOperation(
                        repairPlan,
                        EchoLaunchSetupOperationKind.ResolveConfiguration)
                        .Disposition,
                    Is.EqualTo(
                        EchoLaunchSetupOperationDisposition.Repair));
                Assert.That(
                    EchoLaunchSetupTestFactory.FindOperation(
                        repairPlan,
                        EchoLaunchSetupOperationKind.ResolveRootPrefabVariant)
                        .Disposition,
                    Is.EqualTo(
                        EchoLaunchSetupOperationDisposition.Repair));

                EchoLaunchSetupFailureInjector injector =
                    new EchoLaunchSetupFailureInjector
                    {
                        FailureKind =
                            EchoLaunchSetupOperationKind
                                .ResolveRootPrefabVariant
                    };
                EchoLaunchSetupRepairService service =
                    new EchoLaunchSetupRepairService(
                        new EchoLaunchProjectSnapshotCollector(),
                        new EchoLaunchSetupPlanner(),
                        new EchoLaunchSetupAssetWriter(),
                        new EchoLaunchSetupPrefabWriter(),
                        new EchoLaunchSetupSceneWriter(),
                        new EchoLaunchSetupBuildSettingsWriter(),
                        new EchoLaunchSetupRepairBackupStore(),
                        injector,
                        delegate { return false; });

                EchoLaunchSetupRepairResult result =
                    service.Repair(
                        new EchoLaunchSetupRepairRequest(
                            repairPlan,
                            true,
                            false));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        EchoLaunchSetupRepairStatus.FailedRolledBack));
                Assert.That(
                    result.DiagnosticCode,
                    Is.EqualTo(
                        EchoLaunchSetupDiagnosticCodes
                            .RepairFailedRolledBack));
                Assert.That(result.RollbackCompleted, Is.True);
                Assert.That(result.BackupDirectory, Is.Empty);
                Assert.That(result.ManualRecoveryPaths, Is.Empty);
                Assert.That(
                    AssetDatabase.LoadMainAssetAtPath(
                        paths.SplashSequenceAssetPath),
                    Is.Null);
                Assert.That(
                    ReadProjectBytes(paths.ConfigurationAssetPath),
                    Is.EqualTo(configurationBytes));
                Assert.That(
                    ReadProjectBytes(paths.ConfigurationAssetPath + ".meta"),
                    Is.EqualTo(configurationMetaBytes));
                Assert.That(
                    ReadProjectBytes(paths.RootPrefabPath),
                    Is.EqualTo(prefabBytes));
                Assert.That(
                    ReadProjectBytes(paths.RootPrefabPath + ".meta"),
                    Is.EqualTo(prefabMetaBytes));
            }
            finally
            {
                EditorBuildSettings.scenes =
                    EchoLaunchSetupBuildSettingsWriter.Clone(
                        originalBuildSettings);
                AssetDatabase.DeleteAsset(projectRoot);
                AssetDatabase.DeleteAsset(destinationFolder);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private static EchoLaunchSetupPlan Refresh(
            EchoLaunchSetupRequest request)
        {
            EchoLaunchProjectSnapshot snapshot =
                new EchoLaunchProjectSnapshotCollector().Collect(request);
            return new EchoLaunchSetupPlanner().CreatePlan(request, snapshot);
        }

        private static void IntroduceConfigurationAndPrefabDrift(
            EchoLaunchSetupPathSet paths)
        {
            SetObjectReference(
                paths.ConfigurationAssetPath,
                "startupSequence",
                null);
            SetObjectReference(
                paths.ConfigurationAssetPath,
                "initialDestination",
                null);

            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    paths.RootPrefabPath);
            EchoLaunchRoot root =
                prefab.GetComponentInChildren<EchoLaunchRoot>(true);
            SerializedObject serialized = new SerializedObject(root);
            SerializedProperty property =
                serialized.FindProperty("configuration");
            property.objectReferenceValue = null;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(root);
            PrefabUtility.SavePrefabAsset(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(
                ImportAssetOptions.ForceSynchronousImport);
        }

        private static void IntroduceApprovedDrift(
            EchoLaunchSetupPathSet paths)
        {
            SetObjectReference(
                paths.ConfigurationAssetPath,
                "startupSequence",
                null);
            SetObjectReference(
                paths.ConfigurationAssetPath,
                "initialDestination",
                null);
            SetObjectReference(
                paths.ConfigurationAssetPath,
                "splashSequence",
                null);
            SetString(paths.LaunchDestinationAssetPath, "scenePath", string.Empty);
            SetString(paths.LaunchDestinationAssetPath, "displayName", string.Empty);

            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    paths.RootPrefabPath);
            EchoLaunchRoot root =
                prefab.GetComponentInChildren<EchoLaunchRoot>(true);
            SerializedObject rootSerialized = new SerializedObject(root);
            rootSerialized.FindProperty("configuration").objectReferenceValue =
                null;
            rootSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(root);
            PrefabUtility.SavePrefabAsset(prefab);

            Scene previousActive = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.OpenScene(
                paths.BootScenePath,
                OpenSceneMode.Additive);
            try
            {
                EchoLaunchRoot[] sceneRoots =
                    FindSceneRoots(scene).ToArray();
                for (int index = 0; index < sceneRoots.Length; index++)
                {
                    GameObject nearest =
                        PrefabUtility.GetNearestPrefabInstanceRoot(
                            sceneRoots[index].gameObject);
                    UnityEngine.Object.DestroyImmediate(
                        nearest == null
                            ? sceneRoots[index].gameObject
                            : nearest);
                }

                GameObject marker = new GameObject(RepairMarkerName);
                SceneManager.MoveGameObjectToScene(marker, scene);
                EditorSceneManager.SaveScene(scene, paths.BootScenePath, false);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
                if (previousActive.IsValid() && previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
            }

            EditorBuildSettingsScene[] scenes =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);
            for (int index = 0; index < scenes.Length; index++)
            {
                if (scenes[index].path == paths.BootScenePath)
                {
                    scenes[index] =
                        new EditorBuildSettingsScene(
                            scenes[index].path,
                            false);
                }
            }
            EditorBuildSettings.scenes = scenes;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(
                ImportAssetOptions.ForceSynchronousImport);
        }

        private static void SetObjectReference(
            string assetPath,
            string propertyName,
            UnityEngine.Object value)
        {
            UnityEngine.Object asset =
                AssetDatabase.LoadMainAssetAtPath(assetPath);
            SerializedObject serialized = new SerializedObject(asset);
            serialized.FindProperty(propertyName).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
        }

        private static void SetString(
            string assetPath,
            string propertyName,
            string value)
        {
            UnityEngine.Object asset =
                AssetDatabase.LoadMainAssetAtPath(assetPath);
            SerializedObject serialized = new SerializedObject(asset);
            serialized.FindProperty(propertyName).stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
        }

        private static string ReadString(
            string assetPath,
            string propertyName)
        {
            UnityEngine.Object asset =
                AssetDatabase.LoadMainAssetAtPath(assetPath);
            SerializedObject serialized = new SerializedObject(asset);
            return serialized.FindProperty(propertyName).stringValue;
        }

        private static Dictionary<string, string> CaptureGuids(
            EchoLaunchSetupPathSet paths)
        {
            string[] assetPaths =
            {
                paths.ConfigurationAssetPath,
                paths.StartupSequenceAssetPath,
                paths.LaunchDestinationAssetPath,
                paths.SplashSequenceAssetPath,
                paths.RootPrefabPath,
                paths.BootScenePath
            };
            Dictionary<string, string> result =
                new Dictionary<string, string>();
            for (int index = 0; index < assetPaths.Length; index++)
            {
                result[assetPaths[index]] =
                    AssetDatabase.AssetPathToGUID(assetPaths[index]);
            }
            return result;
        }

        private static int CountBootEntries(string bootPath)
        {
            int count = 0;
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int index = 0; index < scenes.Length; index++)
            {
                if (scenes[index].path == bootPath)
                {
                    count++;
                }
            }
            return count;
        }

        private static int CountBootRoots(string scenePath)
        {
            Scene previousActive = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.OpenScene(
                scenePath,
                OpenSceneMode.Additive);
            try
            {
                return FindSceneRoots(scene).Count;
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
                if (previousActive.IsValid() && previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
            }
        }

        private static bool BootSceneContainsObject(
            string scenePath,
            string objectName)
        {
            Scene previousActive = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.OpenScene(
                scenePath,
                OpenSceneMode.Additive);
            try
            {
                GameObject[] roots = scene.GetRootGameObjects();
                for (int index = 0; index < roots.Length; index++)
                {
                    if (string.Equals(
                            roots[index].name,
                            objectName,
                            StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
                if (previousActive.IsValid() && previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
            }
        }

        private static List<EchoLaunchRoot> FindSceneRoots(Scene scene)
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

        private static void CreateDestination(
            string folder,
            string path)
        {
            AssetDatabase.CreateFolder(
                "Assets",
                folder.Substring("Assets/".Length));
            Scene previousActive = SceneManager.GetActiveScene();
            using (EchoLaunchUntitledSceneLease.Acquire())
            {
                Scene scene = EditorSceneManager.NewScene(
                    NewSceneSetup.EmptyScene,
                    NewSceneMode.Additive);
                try
                {
                    EditorSceneManager.SaveScene(scene, path, false);
                }
                finally
                {
                    EditorSceneManager.CloseScene(scene, true);
                    if (previousActive.IsValid() && previousActive.isLoaded)
                    {
                        SceneManager.SetActiveScene(previousActive);
                    }
                }
            }
            AssetDatabase.ImportAsset(
                path,
                ImportAssetOptions.ForceSynchronousImport);
        }

        private static byte[] ReadProjectBytes(string relative)
        {
            return File.ReadAllBytes(ProjectAbsolute(relative));
        }

        private static string ProjectAbsolute(string relative)
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            return Path.GetFullPath(Path.Combine(root, relative));
        }
    }

}
