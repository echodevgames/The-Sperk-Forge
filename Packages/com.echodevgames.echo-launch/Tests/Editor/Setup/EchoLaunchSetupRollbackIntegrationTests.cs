using System;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupRollbackIntegrationTests
    {
        [Test]
        public void InjectedPrefabFailureRemovesActiveAttemptFoundation()
        {
            string id = Guid.NewGuid().ToString("N");
            string root =
                "Assets/__EchoLaunch_FL_M5_02_Rollback_" + id;

            string destinationFolder =
                "Assets/__EchoLaunch_FL_M5_02_RollbackDestination_" + id;

            string destinationPath =
                destinationFolder + "/Destination.unity";

            EditorBuildSettingsScene[] original =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);

            try
            {
                CreateDestination(destinationFolder, destinationPath);

                EchoLaunchSetupRequest request =
                    new EchoLaunchSetupRequest(
                        root,
                        root + "/Scenes/Boot.unity",
                        destinationPath,
                        false,
                        EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);

                EchoLaunchProjectSnapshot snapshot =
                    EchoLaunchSetupTestFactory.CreateIsolatedSnapshot(request);

                EchoLaunchSetupPlan plan =
                    new EchoLaunchSetupPlanner().CreatePlan(
                        request,
                        snapshot);

                EchoLaunchSetupFailureInjector injector =
                    new EchoLaunchSetupFailureInjector
                    {
                        FailureKind =
                            EchoLaunchSetupOperationKind
                                .ResolveRootPrefabVariant
                    };

                EchoLaunchSetupApplyService service =
                    EchoLaunchSetupTestFactory.CreateIsolatedApplyService(
                        injector);

                EchoLaunchSetupApplyResult result =
                    service.Apply(
                        new EchoLaunchSetupApplyRequest(
                            plan,
                            true,
                            false));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        EchoLaunchSetupApplyStatus.FailedRolledBack));

                Assert.That(
                    AssetDatabase.IsValidFolder(root),
                    Is.False);

                AssertBuildSettingsEqual(original);
            }
            finally
            {
                EditorBuildSettings.scenes =
                    EchoLaunchSetupBuildSettingsWriter.Clone(original);

                AssetDatabase.DeleteAsset(root);
                AssetDatabase.DeleteAsset(destinationFolder);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [Test]
        public void InjectedBuildSettingsFailureRemovesCreatedSceneAndAssets()
        {
            string id = Guid.NewGuid().ToString("N");
            string root =
                "Assets/__EchoLaunch_FL_M5_02_RollbackBuild_" + id;

            string destinationFolder =
                "Assets/__EchoLaunch_FL_M5_02_RollbackBuildDestination_" + id;

            string destinationPath =
                destinationFolder + "/Destination.unity";

            EditorBuildSettingsScene[] original =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);

            try
            {
                CreateDestination(destinationFolder, destinationPath);

                EchoLaunchSetupRequest request =
                    new EchoLaunchSetupRequest(
                        root,
                        root + "/Scenes/Boot.unity",
                        destinationPath,
                        false,
                        EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);

                EchoLaunchSetupPlan plan =
                    new EchoLaunchSetupPlanner().CreatePlan(
                        request,
                        EchoLaunchSetupTestFactory.CreateIsolatedSnapshot(
                            request));

                EchoLaunchSetupFailureInjector injector =
                    new EchoLaunchSetupFailureInjector
                    {
                        FailureKind =
                            EchoLaunchSetupOperationKind
                                .ResolveBuildSettings
                    };

                EchoLaunchSetupApplyService service =
                    EchoLaunchSetupTestFactory.CreateIsolatedApplyService(
                        injector);

                EchoLaunchSetupApplyResult result =
                    service.Apply(
                        new EchoLaunchSetupApplyRequest(
                            plan,
                            true,
                            false));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        EchoLaunchSetupApplyStatus.FailedRolledBack));

                Assert.That(
                    AssetDatabase.LoadMainAssetAtPath(
                        root + "/Scenes/Boot.unity"),
                    Is.Null);

                Assert.That(
                    AssetDatabase.IsValidFolder(root),
                    Is.False);

                AssertBuildSettingsEqual(original);
            }
            finally
            {
                EditorBuildSettings.scenes =
                    EchoLaunchSetupBuildSettingsWriter.Clone(original);

                AssetDatabase.DeleteAsset(root);
                AssetDatabase.DeleteAsset(destinationFolder);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
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
                Scene scene =
                    EditorSceneManager.NewScene(
                        NewSceneSetup.EmptyScene,
                        NewSceneMode.Additive);

                try
            {
                EditorSceneManager.SaveScene(scene, path, false);
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

            AssetDatabase.ImportAsset(
                path,
                ImportAssetOptions.ForceSynchronousImport);
        }

        private static void AssertBuildSettingsEqual(
            EditorBuildSettingsScene[] expected)
        {
            EditorBuildSettingsScene[] actual =
                EditorBuildSettings.scenes;

            Assert.That(actual.Length, Is.EqualTo(expected.Length));

            for (int index = 0; index < expected.Length; index++)
            {
                Assert.That(
                    actual[index].path,
                    Is.EqualTo(expected[index].path));

                Assert.That(
                    actual[index].enabled,
                    Is.EqualTo(expected[index].enabled));
            }
        }
    }
}
