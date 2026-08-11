using System.Linq;
using EchoDevGames.EchoSave.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveSetupM502Tests
    {
        private const string TestFolder =
            "Assets/__EchoSaveM5_02Tests";
        private const string TargetPath =
            TestFolder + "/EchoSaveConfiguration.asset";

        private EchoSaveSetupService service;

        [SetUp]
        public void SetUp()
        {
            DeleteTestFolder();
            AssetDatabase.CreateFolder(
                "Assets",
                "__EchoSaveM5_02Tests");
            service = new EchoSaveSetupService();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTestFolder();
            EchoSaveRoot[] roots =
                Resources.FindObjectsOfTypeAll<
                    EchoSaveRoot>();

            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i] != null &&
                    roots[i].gameObject != null &&
                    roots[i].gameObject.name.StartsWith(
                        "__M502Root",
                        System.StringComparison.Ordinal))
                {
                    Object.DestroyImmediate(
                        roots[i].gameObject);
                }
            }
        }

        [Test]
        public void CreateApplyAuthorsCurrentSchemaThreeDefaults()
        {
            EchoSaveSetupResult result =
                service.Apply(
                    service.Preview(
                        LegacyRequest(
                            "Assets/Assets/__EchoSaveM5_02Tests/EchoSaveConfiguration.asset")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Created));
            Assert.That(
                result.AssetPath,
                Is.EqualTo(TargetPath));
            Assert.That(
                result.Configuration.SchemaVersion,
                Is.EqualTo(
                    EchoSaveConfiguration.CurrentSchemaVersion));
            Assert.That(
                result.Configuration.MaxTotalGenerations,
                Is.EqualTo(
                    SaveRetentionPolicy.DefaultTotalGenerations));
        }

        [Test]
        public void RelativeAssetsPathNormalizesUnderAssetsWithoutDuplication()
        {
            EchoSaveSetupPlan plan =
                service.Preview(
                    LegacyRequest(
                        "__EchoSaveM5_02Tests/EchoSaveConfiguration.asset"));

            Assert.That(plan.CanApply, Is.True);
            Assert.That(
                plan.NormalizedAssetPath,
                Is.EqualTo(TargetPath));
        }

        [Test]
        public void EditSchemaTwoPreviewIsZeroWriteAndShowsUpgrade()
        {
            EchoSaveConfiguration configuration =
                CreateSchemaTwoAsset();
            string before =
                EditorJsonUtility.ToJson(configuration);

            EchoSaveSetupPlan plan =
                service.Preview(
                    FullRequest(
                        configuration,
                        9,
                        128,
                        300,
                        301));

            Assert.That(plan.CanApply, Is.True);
            Assert.That(
                plan.Disposition,
                Is.EqualTo(
                    EchoSaveSetupDisposition.Update));
            Assert.That(
                plan.SourceSchemaVersion,
                Is.EqualTo(
                    EchoSaveConfiguration.SlotPolicySchemaVersion));
            Assert.That(
                plan.Changes.Any(
                    change =>
                        change.PropertyName == "Schema Version"),
                Is.True);
            Assert.That(
                EditorJsonUtility.ToJson(configuration),
                Is.EqualTo(before));
        }

        [Test]
        public void EditSchemaTwoApplyUpgradesExplicitlyToSchemaThree()
        {
            EchoSaveConfiguration configuration =
                CreateSchemaTwoAsset();

            EchoSaveSetupResult result =
                service.Apply(
                    service.Preview(
                        FullRequest(
                            configuration,
                            9,
                            128,
                            300,
                            301)));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Updated));
            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(
                    EchoSaveConfiguration.CurrentSchemaVersion));
            Assert.That(
                configuration.MaxTotalGenerations,
                Is.EqualTo(9));
            Assert.That(
                configuration.CatalogScanLimit,
                Is.EqualTo(128));
        }

        [Test]
        public void EditApplyRejectsStalePreviewAfterTargetMutation()
        {
            EchoSaveConfiguration configuration =
                CreateSchemaTwoAsset();
            EchoSaveSetupPlan plan =
                service.Preview(
                    FullRequest(
                        configuration,
                        9,
                        128,
                        300,
                        301));

            var serialized =
                new SerializedObject(configuration);
            serialized
                .FindProperty("storageRootDirectoryName")
                .stringValue = "ChangedAfterPreview";
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EchoSaveSetupResult result =
                service.Apply(plan);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Rejected));
            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(
                    EchoSaveConfiguration.SlotPolicySchemaVersion));
        }

        [Test]
        public void RootRepairPreviewIsZeroMutationAndApplyChangesOnlyReference()
        {
            EchoSaveConfiguration configuration =
                CreateCurrentAsset();
            GameObject gameObject =
                new GameObject("__M502RootOne");
            EchoSaveRoot root =
                gameObject.AddComponent<EchoSaveRoot>();

            EchoSaveRootRepairPlan plan =
                service.PreviewRootRepair(
                    root,
                    configuration);

            Assert.That(plan.CanApply, Is.True);
            Assert.That(
                ReadSerializedConfiguration(root),
                Is.Null);

            EchoSaveRootRepairResult result =
                service.ApplyRootRepair(plan);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Updated));
            Assert.That(
                ReadSerializedConfiguration(root),
                Is.SameAs(configuration));

            Undo.PerformUndo();
            Assert.That(
                ReadSerializedConfiguration(root),
                Is.Null);
        }

        [Test]
        public void RootRepairWithDuplicateLoadedRootsFailsClosed()
        {
            EchoSaveConfiguration configuration =
                CreateCurrentAsset();
            EchoSaveRoot first =
                new GameObject("__M502RootFirst")
                    .AddComponent<EchoSaveRoot>();
            new GameObject("__M502RootSecond")
                .AddComponent<EchoSaveRoot>();

            EchoSaveRootRepairPlan plan =
                service.PreviewRootRepair(
                    first,
                    configuration);

            Assert.That(plan.CanApply, Is.False);
            Assert.That(
                plan.Disposition,
                Is.EqualTo(
                    EchoSaveSetupDisposition.Rejected));
            Assert.That(
                ReadSerializedConfiguration(first),
                Is.Null);
        }

        private static EchoSaveConfiguration
            ReadSerializedConfiguration(
                EchoSaveRoot root)
        {
            var serialized =
                new SerializedObject(root);
            return serialized
                .FindProperty("configuration")
                .objectReferenceValue as
                EchoSaveConfiguration;
        }

        [TestCase(1)]
        [TestCase(257)]
        public void InvalidRetentionBlocksSetupPreview(
            int maxGenerations)
        {
            EchoSaveSetupPlan plan =
                service.Preview(
                    FullRequest(
                        null,
                        maxGenerations,
                        256,
                        512,
                        512));

            Assert.That(plan.CanApply, Is.False);
        }

        private static EchoSaveSetupRequest LegacyRequest(
            string path)
        {
            return new EchoSaveSetupRequest(
                path,
                "EchoSave",
                SaveSlotPolicyMode.ConfigurableMultiSlot,
                4,
                64,
                64);
        }

        private static EchoSaveSetupRequest FullRequest(
            EchoSaveConfiguration configuration,
            int maxGenerations,
            int catalog,
            int retention,
            int recovery)
        {
            return new EchoSaveSetupRequest(
                configuration,
                configuration != null
                    ? AssetDatabase.GetAssetPath(configuration)
                    : TargetPath,
                "EchoSave",
                SaveSlotPolicyMode.ConfigurableMultiSlot,
                4,
                64,
                64,
                maxGenerations,
                EchoSaveConfiguration.DefaultSerializerProviderId,
                EchoSaveConfiguration.DefaultStorageProviderId,
                catalog,
                retention,
                recovery,
                EchoSaveRecoveryPolicyMode.ManualOnly,
                System.Array.Empty<SaveSlotTemplate>());
        }

        private static EchoSaveConfiguration CreateSchemaTwoAsset()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();
            configuration.SetDefinitionForTesting(
                EchoSaveConfiguration.SlotPolicySchemaVersion,
                "EchoSave");
            AssetDatabase.CreateAsset(
                configuration,
                TargetPath);
            AssetDatabase.SaveAssets();
            return configuration;
        }

        private static EchoSaveConfiguration CreateCurrentAsset()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();
            AssetDatabase.CreateAsset(
                configuration,
                TargetPath);
            AssetDatabase.SaveAssets();
            return configuration;
        }

        private static void DeleteTestFolder()
        {
            if (AssetDatabase.IsValidFolder(TestFolder))
            {
                AssetDatabase.DeleteAsset(TestFolder);
                AssetDatabase.Refresh();
            }
        }
    }
}
