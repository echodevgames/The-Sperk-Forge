
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
        public void CollectorExcludesImportedSamplesFromAutomaticCandidates()
        {
            bool samplesRootExisted =
                AssetDatabase.IsValidFolder("Assets/Samples");

            string testRoot =
                "Assets/Samples/__EchoLaunch_FL_M5_07_CandidateIsolation_" +
                Guid.NewGuid().ToString("N");

            string configurationPath =
                testRoot + "/ImportedConfiguration.asset";

            string startupSequencePath =
                testRoot + "/ImportedStartupSequence.asset";

            string destinationPath =
                testRoot + "/ImportedDestination.asset";

            string splashPath =
                testRoot + "/ImportedSplashSequence.asset";

            string rootPrefabPath =
                testRoot + "/EchoLaunchRoot_ImportedSampleCandidate.prefab";

            try
            {
                EnsureFolder("Assets/Samples");
                EnsureFolder(testRoot);

                CreateAsset<EchoLaunchConfiguration>(
                    configurationPath);

                CreateAsset<StartupSequence>(
                    startupSequencePath);

                CreateAsset<LaunchDestination>(
                    destinationPath);

                CreateAsset<SplashSequence>(
                    splashPath);

                Assert.That(
                    AssetDatabase.CopyAsset(
                        EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                        rootPrefabPath),
                    Is.True);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EchoLaunchProjectSnapshot snapshot =
                    Collect();

                AssertCandidatePathAbsent(
                    snapshot,
                    EchoLaunchSetupAssetRole.Configuration,
                    configurationPath);

                AssertCandidatePathAbsent(
                    snapshot,
                    EchoLaunchSetupAssetRole.StartupSequence,
                    startupSequencePath);

                AssertCandidatePathAbsent(
                    snapshot,
                    EchoLaunchSetupAssetRole.LaunchDestination,
                    destinationPath);

                AssertCandidatePathAbsent(
                    snapshot,
                    EchoLaunchSetupAssetRole.SplashSequence,
                    splashPath);

                AssertCandidatePathAbsent(
                    snapshot,
                    EchoLaunchSetupAssetRole.RootPrefab,
                    rootPrefabPath);

                AssertNoAutomaticCandidateUnderImportedSamples(
                    snapshot,
                    EchoLaunchSetupAssetRole.Configuration);

                AssertNoAutomaticCandidateUnderImportedSamples(
                    snapshot,
                    EchoLaunchSetupAssetRole.StartupSequence);

                AssertNoAutomaticCandidateUnderImportedSamples(
                    snapshot,
                    EchoLaunchSetupAssetRole.LaunchDestination);

                AssertNoAutomaticCandidateUnderImportedSamples(
                    snapshot,
                    EchoLaunchSetupAssetRole.SplashSequence);

                AssertNoAutomaticCandidateUnderImportedSamples(
                    snapshot,
                    EchoLaunchSetupAssetRole.RootPrefab);
            }
            finally
            {
                CleanupImportedSampleTestRoot(
                    testRoot,
                    samplesRootExisted);
            }
        }

        [Test]
        public void ExplicitImportedSampleSelectionRemainsAddressable()
        {
            bool samplesRootExisted =
                AssetDatabase.IsValidFolder("Assets/Samples");

            string testRoot =
                "Assets/Samples/__EchoLaunch_FL_M5_07_ExplicitSelection_" +
                Guid.NewGuid().ToString("N");

            string configurationPath =
                testRoot + "/ExplicitConfiguration.asset";

            try
            {
                EnsureFolder("Assets/Samples");
                EnsureFolder(testRoot);

                CreateAsset<EchoLaunchConfiguration>(
                    configurationPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EchoLaunchSetupRequest request =
                    new EchoLaunchSetupRequest(
                        UniqueRoot,
                        UniqueRoot + "/Scenes/Boot.unity",
                        UniqueRoot + "/Scenes/Missing.unity",
                        false,
                        EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                        configurationPath);

                EchoLaunchProjectSnapshot snapshot =
                    new EchoLaunchProjectSnapshotCollector()
                        .Collect(request);

                EchoLaunchProjectAssetFact selectedFact =
                    snapshot.FindAssetFact(configurationPath);

                Assert.That(selectedFact.Exists, Is.True);
                Assert.That(
                    selectedFact.Path,
                    Is.EqualTo(configurationPath));

                AssertCandidatePathAbsent(
                    snapshot,
                    EchoLaunchSetupAssetRole.Configuration,
                    configurationPath);
            }
            finally
            {
                CleanupImportedSampleTestRoot(
                    testRoot,
                    samplesRootExisted);
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

        private static void CreateAsset<T>(
            string path)
            where T : ScriptableObject
        {
            T asset =
                ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(
                asset,
                path);
        }

        private static void EnsureFolder(
            string path)
        {
            string normalized =
                path.Replace('\\', '/');

            string[] segments =
                normalized.Split('/');

            string current =
                segments[0];

            for (int index = 1;
                index < segments.Length;
                index++)
            {
                string next =
                    current + "/" + segments[index];

                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(
                        current,
                        segments[index]);
                }

                current = next;
            }
        }

        private static void AssertCandidatePathAbsent(
            EchoLaunchProjectSnapshot snapshot,
            EchoLaunchSetupAssetRole role,
            string path)
        {
            IReadOnlyList<EchoLaunchProjectAssetFact> candidates =
                snapshot.GetCandidates(role);

            for (int index = 0;
                index < candidates.Count;
                index++)
            {
                Assert.That(
                    candidates[index].Path,
                    Is.Not.EqualTo(path),
                    role + " should not automatically discover imported sample content.");
            }
        }

        private static void AssertNoAutomaticCandidateUnderImportedSamples(
            EchoLaunchProjectSnapshot snapshot,
            EchoLaunchSetupAssetRole role)
        {
            IReadOnlyList<EchoLaunchProjectAssetFact> candidates =
                snapshot.GetCandidates(role);

            for (int index = 0;
                index < candidates.Count;
                index++)
            {
                Assert.That(
                    candidates[index].Path.StartsWith(
                        "Assets/Samples/",
                        StringComparison.OrdinalIgnoreCase),
                    Is.False,
                    role + " candidate escaped the imported-sample isolation boundary.");
            }
        }

        private static void CleanupImportedSampleTestRoot(
            string testRoot,
            bool samplesRootExisted)
        {
            AssetDatabase.DeleteAsset(testRoot);

            if (!samplesRootExisted &&
                AssetDatabase.IsValidFolder("Assets/Samples"))
            {
                AssetDatabase.DeleteAsset("Assets/Samples");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
