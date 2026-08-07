using System;
using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupApplyIntegrationTests
    {
        private string projectRoot;
        private string destinationFolder;
        private string destinationScenePath;
        private EchoLaunchSetupRequest request;
        private EchoLaunchSetupPathSet paths;
        private EchoLaunchSetupApplyResult firstResult;
        private EchoLaunchSetupApplyResult secondResult;
        private EchoLaunchSetupApplyResult thirdResult;
        private EchoLaunchSetupPlan finalPlan;
        private EditorBuildSettingsScene[] originalBuildSettings;
        private Scene activeBefore;
        private int sceneCountBefore;
        private bool packageTemplateDirtyBefore;
        private Dictionary<string, string> createdGuids;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            string id = Guid.NewGuid().ToString("N");

            projectRoot =
                "Assets/__EchoLaunch_FL_M5_02_Tests_" + id;

            destinationFolder =
                "Assets/__EchoLaunch_FL_M5_02_Destination_" + id;

            destinationScenePath =
                destinationFolder + "/Destination.unity";

            originalBuildSettings =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);

            activeBefore = SceneManager.GetActiveScene();
            sceneCountBefore = SceneManager.sceneCount;

            GameObject packageTemplate =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath);

            packageTemplateDirtyBefore =
                packageTemplate != null &&
                EditorUtility.IsDirty(packageTemplate);

            try
            {
                CreateDestinationScene();

                request =
                    new EchoLaunchSetupRequest(
                        projectRoot,
                        projectRoot + "/Scenes/Boot.unity",
                        destinationScenePath,
                        true,
                        EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);

                paths =
                    new EchoLaunchSetupPathSet(
                        projectRoot,
                        request.BootScenePath);

                EchoLaunchSetupPlan displayedPlan = RefreshPlan();

                EchoLaunchSetupApplyService service =
                    EchoLaunchSetupTestFactory.CreateIsolatedApplyService();

                firstResult =
                    service.Apply(
                        new EchoLaunchSetupApplyRequest(
                            displayedPlan,
                            true,
                            false));

                if (firstResult.Status !=
                    EchoLaunchSetupApplyStatus.Succeeded)
                {
                    throw new InvalidOperationException(
                        firstResult.Message);
                }

                createdGuids = CaptureCreatedGuids();

                EchoLaunchSetupPlan secondPlan = RefreshPlan();

                secondResult =
                    service.Apply(
                        new EchoLaunchSetupApplyRequest(
                            secondPlan,
                            true,
                            false));

                EchoLaunchSetupPlan thirdPlan = RefreshPlan();

                thirdResult =
                    service.Apply(
                        new EchoLaunchSetupApplyRequest(
                            thirdPlan,
                            true,
                            false));

                finalPlan = RefreshPlan();
            }
            catch
            {
                Cleanup();
                throw;
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Cleanup();
        }

        [Test]
        public void FirstApplySucceeds()
        {
            Assert.That(
                firstResult.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.Succeeded));
        }

        [Test]
        public void SecondApplyIsNoChanges()
        {
            Assert.That(
                secondResult.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.NoChanges));
        }

        [Test]
        public void ThirdApplyIsNoChanges()
        {
            Assert.That(
                thirdResult.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.NoChanges));
        }

        [Test]
        public void RequiredFoldersExist()
        {
            Assert.That(
                AssetDatabase.IsValidFolder(projectRoot),
                Is.True);

            Assert.That(
                AssetDatabase.IsValidFolder(paths.ConfigurationFolderPath),
                Is.True);

            Assert.That(
                AssetDatabase.IsValidFolder(paths.PrefabsFolderPath),
                Is.True);

            Assert.That(
                AssetDatabase.IsValidFolder(paths.ScenesFolderPath),
                Is.True);
        }

        [Test]
        public void StartupSequenceAssetExists()
        {
            Assert.That(
                AssetDatabase.LoadAssetAtPath<StartupSequence>(
                    paths.StartupSequenceAssetPath),
                Is.Not.Null);
        }

        [Test]
        public void DestinationAssetExists()
        {
            Assert.That(
                AssetDatabase.LoadAssetAtPath<LaunchDestination>(
                    paths.LaunchDestinationAssetPath),
                Is.Not.Null);
        }

        [Test]
        public void SplashSequenceAssetExists()
        {
            Assert.That(
                AssetDatabase.LoadAssetAtPath<SplashSequence>(
                    paths.SplashSequenceAssetPath),
                Is.Not.Null);
        }

        [Test]
        public void ConfigurationReferencesResolvedAssets()
        {
            EchoLaunchConfiguration configuration =
                LoadConfiguration();

            Assert.That(configuration.StartupSequence, Is.Not.Null);
            Assert.That(configuration.InitialDestination, Is.Not.Null);
            Assert.That(configuration.SplashSequence, Is.Not.Null);
        }

        [Test]
        public void DestinationPointsToSelectedScene()
        {
            Assert.That(
                LoadConfiguration().InitialDestination.ScenePath,
                Is.EqualTo(destinationScenePath));
        }

        [Test]
        public void DestinationDisplayNameUsesSceneName()
        {
            Assert.That(
                LoadConfiguration().InitialDestination.DisplayName,
                Is.EqualTo("Destination"));
        }

        [Test]
        public void RootPrefabExists()
        {
            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    paths.RootPrefabPath),
                Is.Not.Null);
        }

        [Test]
        public void RootPrefabIsVariant()
        {
            GameObject root =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    paths.RootPrefabPath);

            Assert.That(
                PrefabUtility.GetPrefabAssetType(root),
                Is.EqualTo(PrefabAssetType.Variant));
        }

        [Test]
        public void RootPrefabIsBoundToConfiguration()
        {
            GameObject rootObject =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    paths.RootPrefabPath);

            EchoLaunchRoot root =
                rootObject.GetComponent<EchoLaunchRoot>();

            SerializedObject serialized =
                new SerializedObject(root);

            Assert.That(
                serialized.FindProperty("configuration")
                    .objectReferenceValue,
                Is.SameAs(LoadConfiguration()));
        }

        [Test]
        public void RootPrefabPreservesNestedStatusPresenter()
        {
            GameObject contents =
                PrefabUtility.LoadPrefabContents(
                    paths.RootPrefabPath);

            bool foundNestedStatusPrefab = false;

            try
            {
                Transform[] transforms =
                    contents.GetComponentsInChildren<Transform>(true);

                for (int index = 0; index < transforms.Length; index++)
                {
                    string nestedPath =
                        PrefabUtility
                            .GetPrefabAssetPathOfNearestInstanceRoot(
                                transforms[index].gameObject);

                    if (nestedPath.EndsWith(
                            "/EchoLaunchStatusView.prefab",
                            StringComparison.Ordinal))
                    {
                        foundNestedStatusPrefab = true;
                        break;
                    }
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }

            Assert.That(foundNestedStatusPrefab, Is.True);
        }

        [Test]
        public void PackageTemplateDirtyStateIsPreserved()
        {
            GameObject packageTemplate =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath);

            Assert.That(packageTemplate, Is.Not.Null);
            Assert.That(
                EditorUtility.IsDirty(packageTemplate),
                Is.EqualTo(packageTemplateDirtyBefore));
        }

        [Test]
        public void BootSceneAssetExists()
        {
            Assert.That(
                AssetDatabase.GetMainAssetTypeAtPath(
                    paths.BootScenePath),
                Is.EqualTo(typeof(SceneAsset)));
        }

        [Test]
        public void BootSceneContainsExactlyOneRoot()
        {
            Assert.That(
                InspectBootScene().RootCount,
                Is.EqualTo(1));
        }

        [Test]
        public void BootSceneContainsNoEventSystem()
        {
            Assert.That(
                InspectBootScene().EventSystemCount,
                Is.EqualTo(0));
        }

        [Test]
        public void OpenSceneCountIsPreserved()
        {
            Assert.That(
                SceneManager.sceneCount,
                Is.EqualTo(sceneCountBefore));
        }

        [Test]
        public void ActiveSceneIsPreserved()
        {
            Assert.That(
                SceneManager.GetActiveScene(),
                Is.EqualTo(activeBefore));
        }

        [Test]
        public void BuildSettingsContainsOneBootEntry()
        {
            Assert.That(CountBootEntries(), Is.EqualTo(1));
        }

        [Test]
        public void BootBuildEntryIsEnabled()
        {
            EditorBuildSettingsScene[] scenes =
                EditorBuildSettings.scenes;

            Assert.That(
                scenes[scenes.Length - 1].path,
                Is.EqualTo(paths.BootScenePath));

            Assert.That(
                scenes[scenes.Length - 1].enabled,
                Is.True);
        }

        [Test]
        public void UnrelatedBuildSettingsOrderIsPreserved()
        {
            EditorBuildSettingsScene[] scenes =
                EditorBuildSettings.scenes;

            for (int index = 0;
                 index < originalBuildSettings.Length;
                 index++)
            {
                Assert.That(
                    scenes[index].path,
                    Is.EqualTo(originalBuildSettings[index].path));

                Assert.That(
                    scenes[index].enabled,
                    Is.EqualTo(originalBuildSettings[index].enabled));
            }
        }

        [Test]
        public void CreatedGuidsRemainStableAcrossReruns()
        {
            Dictionary<string, string> current =
                CaptureCreatedGuids();

            foreach (
                KeyValuePair<string, string> pair in createdGuids)
            {
                Assert.That(
                    current[pair.Key],
                    Is.EqualTo(pair.Value));
            }
        }

        [Test]
        public void FinalPlanContainsNoCreateOperations()
        {
            Assert.That(
                finalPlan.CountDisposition(
                    EchoLaunchSetupOperationDisposition.Create),
                Is.EqualTo(0));
        }

        private EchoLaunchSetupPlan RefreshPlan()
        {
            EchoLaunchProjectSnapshot snapshot =
                EchoLaunchSetupTestFactory.CreateIsolatedSnapshot(request);

            return new EchoLaunchSetupPlanner().CreatePlan(
                request,
                snapshot);
        }

        private EchoLaunchConfiguration LoadConfiguration()
        {
            EchoLaunchConfiguration configuration =
                AssetDatabase.LoadAssetAtPath<EchoLaunchConfiguration>(
                    paths.ConfigurationAssetPath);

            Assert.That(configuration, Is.Not.Null);
            return configuration;
        }

        private void CreateDestinationScene()
        {
            AssetDatabase.CreateFolder(
                "Assets",
                destinationFolder.Substring("Assets/".Length));

            Scene previousActive = SceneManager.GetActiveScene();

            using (EchoLaunchUntitledSceneLease.Acquire())
            {
                Scene scene =
                    EditorSceneManager.NewScene(
                        NewSceneSetup.EmptyScene,
                        NewSceneMode.Additive);

                try
            {
                Assert.That(
                    EditorSceneManager.SaveScene(
                        scene,
                        destinationScenePath,
                        false),
                    Is.True);
            }
            finally
            {
                if (scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }

                if (previousActive.IsValid() &&
                    previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
                }
            }

            AssetDatabase.ImportAsset(
                destinationScenePath,
                ImportAssetOptions.ForceSynchronousImport);
        }

        private Dictionary<string, string> CaptureCreatedGuids()
        {
            string[] tracked =
            {
                paths.StartupSequenceAssetPath,
                paths.LaunchDestinationAssetPath,
                paths.SplashSequenceAssetPath,
                paths.ConfigurationAssetPath,
                paths.RootPrefabPath,
                paths.BootScenePath
            };

            Dictionary<string, string> result =
                new Dictionary<string, string>();

            for (int index = 0; index < tracked.Length; index++)
            {
                result[tracked[index]] =
                    AssetDatabase.AssetPathToGUID(tracked[index]);
            }

            return result;
        }

        private BootSceneInspection InspectBootScene()
        {
            Scene previousActive = SceneManager.GetActiveScene();

            Scene scene =
                EditorSceneManager.OpenScene(
                    paths.BootScenePath,
                    OpenSceneMode.Additive);

            try
            {
                int rootCount = 0;
                int eventSystemCount = 0;

                GameObject[] rootObjects =
                    scene.GetRootGameObjects();

                for (int rootIndex = 0;
                     rootIndex < rootObjects.Length;
                     rootIndex++)
                {
                    rootCount +=
                        rootObjects[rootIndex]
                            .GetComponentsInChildren<EchoLaunchRoot>(true)
                            .Length;

                    MonoBehaviour[] behaviours =
                        rootObjects[rootIndex]
                            .GetComponentsInChildren<MonoBehaviour>(true);

                    for (int behaviourIndex = 0;
                         behaviourIndex < behaviours.Length;
                         behaviourIndex++)
                    {
                        MonoBehaviour behaviour = behaviours[behaviourIndex];

                        if (behaviour != null &&
                            behaviour.GetType().FullName ==
                            "UnityEngine.EventSystems.EventSystem")
                        {
                            eventSystemCount++;
                        }
                    }
                }

                return new BootSceneInspection(
                    rootCount,
                    eventSystemCount);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);

                if (previousActive.IsValid() &&
                    previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
            }
        }

        private int CountBootEntries()
        {
            int count = 0;
            EditorBuildSettingsScene[] scenes =
                EditorBuildSettings.scenes;

            for (int index = 0; index < scenes.Length; index++)
            {
                if (scenes[index].path == paths.BootScenePath)
                {
                    count++;
                }
            }

            return count;
        }

        private void Cleanup()
        {
            EditorBuildSettings.scenes =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    originalBuildSettings);

            if (!string.IsNullOrEmpty(projectRoot))
            {
                AssetDatabase.DeleteAsset(projectRoot);
            }

            if (!string.IsNullOrEmpty(destinationFolder))
            {
                AssetDatabase.DeleteAsset(destinationFolder);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (activeBefore.IsValid() && activeBefore.isLoaded)
            {
                SceneManager.SetActiveScene(activeBefore);
            }
        }

        private sealed class BootSceneInspection
        {
            internal BootSceneInspection(
                int rootCount,
                int eventSystemCount)
            {
                RootCount = rootCount;
                EventSystemCount = eventSystemCount;
            }

            internal int RootCount { get; }
            internal int EventSystemCount { get; }
        }
    }
}
