
using System;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchProjectSnapshotCollectorTests
    {
        private const string UniqueRoot =
            "Assets/__EchoLaunch_FL_M5_01_NoWrite";

        [Test]
        public void CollectorDetectsPackageRootTemplate()
        {
            EchoLaunchProjectSnapshot snapshot = Collect();

            Assert.That(snapshot.PackageRootTemplateAvailable, Is.True);
            Assert.That(snapshot.PackageRootTemplateGuid, Is.Not.Empty);
        }

        [Test]
        public void CollectorReadsBuildSettingsCount()
        {
            EchoLaunchProjectSnapshot snapshot = Collect();

            Assert.That(
                snapshot.BuildSettingsScenes.Count,
                Is.EqualTo(EditorBuildSettings.scenes.Length));
        }

        [Test]
        public void CollectorReadsBuildSettingsOrder()
        {
            EditorBuildSettingsScene[] before = EditorBuildSettings.scenes;
            EchoLaunchProjectSnapshot snapshot = Collect();

            for (int index = 0; index < before.Length; index++)
            {
                Assert.That(
                    snapshot.BuildSettingsScenes[index].Path,
                    Is.EqualTo(before[index].path));

                Assert.That(
                    snapshot.BuildSettingsScenes[index].Index,
                    Is.EqualTo(index));
            }
        }

        [Test]
        public void CollectorDoesNotCreateRequestedRoot()
        {
            Assert.That(AssetDatabase.IsValidFolder(UniqueRoot), Is.False);

            Collect();

            Assert.That(AssetDatabase.IsValidFolder(UniqueRoot), Is.False);
        }

        [Test]
        public void CollectorDoesNotCreateBootScene()
        {
            string bootPath = UniqueRoot + "/Scenes/Boot.unity";

            Assert.That(
                AssetDatabase.GetMainAssetTypeAtPath(bootPath),
                Is.Null);

            Collect();

            Assert.That(
                AssetDatabase.GetMainAssetTypeAtPath(bootPath),
                Is.Null);
        }

        [Test]
        public void CollectorDoesNotChangeBuildSettings()
        {
            EditorBuildSettingsScene[] before = CloneBuildSettings();

            Collect();

            EditorBuildSettingsScene[] after = EditorBuildSettings.scenes;

            Assert.That(after.Length, Is.EqualTo(before.Length));

            for (int index = 0; index < before.Length; index++)
            {
                Assert.That(after[index].path, Is.EqualTo(before[index].path));
                Assert.That(
                    after[index].enabled,
                    Is.EqualTo(before[index].enabled));
            }
        }

        [Test]
        public void CollectorDoesNotChangeOpenSceneSetup()
        {
            SceneSetup[] before = EditorSceneManager.GetSceneManagerSetup();
            string activePath = SceneManager.GetActiveScene().path;

            Collect();

            SceneSetup[] after = EditorSceneManager.GetSceneManagerSetup();

            Assert.That(
                SceneManager.GetActiveScene().path,
                Is.EqualTo(activePath));

            Assert.That(after.Length, Is.EqualTo(before.Length));

            for (int index = 0; index < before.Length; index++)
            {
                Assert.That(after[index].path, Is.EqualTo(before[index].path));
                Assert.That(
                    after[index].isLoaded,
                    Is.EqualTo(before[index].isLoaded));
                Assert.That(
                    after[index].isActive,
                    Is.EqualTo(before[index].isActive));
            }
        }

        [Test]
        public void CollectorDoesNotDirtyPackageTemplate()
        {
            GameObject template =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath);

            Assert.That(template, Is.Not.Null);

            bool dirtyBefore = EditorUtility.IsDirty(template);

            Collect();

            Assert.That(
                EditorUtility.IsDirty(template),
                Is.EqualTo(dirtyBefore));
        }

        [Test]
        public void CollectorReturnsMissingDestinationFact()
        {
            EchoLaunchSetupRequest request =
                new EchoLaunchSetupRequest(
                    UniqueRoot,
                    UniqueRoot + "/Scenes/Boot.unity",
                    UniqueRoot + "/Scenes/Missing.unity",
                    false,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);

            EchoLaunchProjectSnapshot snapshot =
                new EchoLaunchProjectSnapshotCollector().Collect(request);

            Assert.That(
                snapshot.FindAssetFact(request.DestinationScenePath).Exists,
                Is.False);
        }


        [Test]
        public void CollectorCapturesConfigurationRepairEvidence()
        {
            string configurationPath =
                "Assets/__EchoLaunch_FL_M5_03_ConfigEvidence_" +
                Guid.NewGuid().ToString("N") + ".asset";
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<EchoLaunchConfiguration>();

            try
            {
                AssetDatabase.CreateAsset(configuration, configurationPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(
                    configurationPath,
                    ImportAssetOptions.ForceSynchronousImport);

                EchoLaunchSetupRequest request =
                    new EchoLaunchSetupRequest(
                        UniqueRoot,
                        UniqueRoot + "/Scenes/Boot.unity",
                        UniqueRoot + "/Scenes/Missing.unity",
                        false,
                        EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                        configurationPath);
                EchoLaunchProjectAssetFact fact =
                    new EchoLaunchProjectSnapshotCollector()
                        .Collect(request)
                        .FindAssetFact(configurationPath);

                Assert.That(fact.HasRepairEvidence, Is.True);
                Assert.That(
                    fact.ConfigurationSchemaVersion,
                    Is.EqualTo(
                        EchoLaunchConfiguration.CurrentSchemaVersion));
                Assert.That(fact.StableId, Has.Length.EqualTo(32));
            }
            finally
            {
                AssetDatabase.DeleteAsset(configurationPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [Test]
        public void CollectorProducesNonemptyEvidenceFingerprint()
        {
            Assert.That(
                Collect().EvidenceFingerprint,
                Is.Not.Empty);
        }

        [Test]
        public void RepeatedCollectionProducesSameFingerprint()
        {
            Assert.That(
                Collect().EvidenceFingerprint,
                Is.EqualTo(Collect().EvidenceFingerprint));
        }

        private static EchoLaunchProjectSnapshot Collect()
        {
            EchoLaunchSetupRequest request =
                new EchoLaunchSetupRequest(
                    UniqueRoot,
                    UniqueRoot + "/Scenes/Boot.unity",
                    UniqueRoot + "/Scenes/Missing.unity",
                    false,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);

            return new EchoLaunchProjectSnapshotCollector().Collect(request);
        }

        private static EditorBuildSettingsScene[] CloneBuildSettings()
        {
            EditorBuildSettingsScene[] source = EditorBuildSettings.scenes;
            EditorBuildSettingsScene[] clone =
                new EditorBuildSettingsScene[source.Length];

            for (int index = 0; index < source.Length; index++)
            {
                clone[index] =
                    new EditorBuildSettingsScene(
                        source[index].path,
                        source[index].enabled);
            }

            return clone;
        }
    }
}
