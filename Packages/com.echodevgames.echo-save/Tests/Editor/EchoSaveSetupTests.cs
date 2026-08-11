using System.Linq;
using EchoDevGames.EchoSave.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveSetupTests
    {
        private const string TestFolder =
            "Assets/__EchoSaveM5_01Tests";
        private const string TargetPath =
            TestFolder + "/EchoSaveConfiguration.asset";

        private EchoSaveSetupService service;

        [SetUp]
        public void SetUp()
        {
            DeleteTestFolder();

            AssetDatabase.CreateFolder(
                "Assets",
                "__EchoSaveM5_01Tests");

            service =
                new EchoSaveSetupService();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTestFolder();
        }

        [Test]
        public void Preview_PerformsZeroAssetWrites()
        {
            string[] before =
                AssetsUnderTestFolder();

            EchoSaveSetupPlan plan =
                service.Preview(
                    ValidRequest());

            string[] after =
                AssetsUnderTestFolder();

            Assert.That(plan.CanApply, Is.True);
            Assert.That(after, Is.EqualTo(before));
            Assert.That(
                AssetDatabase.LoadMainAssetAtPath(
                    TargetPath),
                Is.Null);
        }

        [TestCase("")]
        [TestCase("Packages/com.foo/Test.asset")]
        [TestCase("Assets/../Test.asset")]
        [TestCase("Assets/Test.txt")]
        public void Preview_InvalidTarget_Rejects(
            string target)
        {
            EchoSaveSetupPlan plan =
                service.Preview(
                    new EchoSaveSetupRequest(
                        target,
                        "EchoSave",
                        SaveSlotPolicyMode
                            .ConfigurableMultiSlot,
                        4,
                        64,
                        64));

            Assert.That(plan.CanApply, Is.False);
            Assert.That(
                plan.Disposition,
                Is.EqualTo(
                    EchoSaveSetupDisposition.Rejected));
        }


        [TestCase("")]
        [TestCase("..")]
        [TestCase("Unsafe/Child")]
        [TestCase("Unsafe\\Child")]
        [TestCase("C:Unsafe")]
        public void Preview_UnsafeStorageRootRejects(
            string storageRoot)
        {
            EchoSaveSetupPlan plan =
                service.Preview(
                    new EchoSaveSetupRequest(
                        TargetPath,
                        storageRoot,
                        SaveSlotPolicyMode
                            .ConfigurableMultiSlot,
                        4,
                        64,
                        64));

            Assert.That(plan.CanApply, Is.False);
        }

        [Test]
        public void Preview_OccupiedTargetRejectsWithoutClobbering()
        {
            EchoSaveConfiguration existing =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            AssetDatabase.CreateAsset(
                existing,
                TargetPath);
            AssetDatabase.SaveAssets();

            string before =
                EditorJsonUtility.ToJson(
                    existing);

            EchoSaveSetupPlan plan =
                service.Preview(
                    ValidRequest());

            EchoSaveSetupResult result =
                service.Apply(plan);

            EchoSaveConfiguration after =
                AssetDatabase.LoadAssetAtPath<
                    EchoSaveConfiguration>(
                    TargetPath);

            Assert.That(plan.CanApply, Is.False);
            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Rejected));
            Assert.That(
                EditorJsonUtility.ToJson(after),
                Is.EqualTo(before));
        }

        [TestCase(
            SaveSlotPolicyMode.SingleSlot,
            4,
            64,
            64,
            1)]
        [TestCase(
            SaveSlotPolicyMode.FixedMultiSlot,
            8,
            64,
            64,
            8)]
        [TestCase(
            SaveSlotPolicyMode.ConfigurableMultiSlot,
            4,
            12,
            64,
            12)]
        [TestCase(
            SaveSlotPolicyMode.BoundedProfiles,
            4,
            64,
            7,
            7)]
        public void Preview_ValidPoliciesMatchRuntimeEffectiveCapacity(
            SaveSlotPolicyMode mode,
            int fixedCount,
            int configuredLimit,
            int profileLimit,
            int expectedCapacity)
        {
            EchoSaveSetupPlan plan =
                service.Preview(
                    new EchoSaveSetupRequest(
                        TargetPath,
                        "EchoSave",
                        mode,
                        fixedCount,
                        configuredLimit,
                        profileLimit));

            Assert.That(plan.CanApply, Is.True);
            Assert.That(
                plan.SchemaVersion,
                Is.EqualTo(
                    EchoSaveConfiguration
                        .CurrentSchemaVersion));
            Assert.That(
                plan.EffectiveCapacity,
                Is.EqualTo(
                    expectedCapacity));
        }

        [TestCase(
            SaveSlotPolicyMode.FixedMultiSlot,
            1,
            64,
            64)]
        [TestCase(
            SaveSlotPolicyMode.ConfigurableMultiSlot,
            4,
            0,
            64)]
        [TestCase(
            SaveSlotPolicyMode.BoundedProfiles,
            4,
            64,
            0)]
        public void Preview_InvalidActivePolicyBoundBlocksApply(
            SaveSlotPolicyMode mode,
            int fixedCount,
            int configuredLimit,
            int profileLimit)
        {
            EchoSaveSetupPlan plan =
                service.Preview(
                    new EchoSaveSetupRequest(
                        TargetPath,
                        "EchoSave",
                        mode,
                        fixedCount,
                        configuredLimit,
                        profileLimit));

            Assert.That(plan.CanApply, Is.False);
        }


        [Test]
        public void Preview_UndefinedSlotPolicyRejects()
        {
            EchoSaveSetupPlan plan =
                service.Preview(
                    new EchoSaveSetupRequest(
                        TargetPath,
                        "EchoSave",
                        (SaveSlotPolicyMode)999,
                        4,
                        64,
                        64));

            Assert.That(plan.CanApply, Is.False);
        }

        [Test]
        public void Apply_CreatesExactlyOneCurrentSchemaConfiguration()
        {
            int rootCountBefore =
                Resources.FindObjectsOfTypeAll<
                    EchoSaveRoot>().Length;

            EchoSaveSetupPlan plan =
                service.Preview(
                    new EchoSaveSetupRequest(
                        TargetPath,
                        "ChronicleData",
                        SaveSlotPolicyMode
                            .FixedMultiSlot,
                        6,
                        64,
                        64));

            EchoSaveSetupResult result =
                service.Apply(plan);

            EchoSaveConfiguration created =
                AssetDatabase.LoadAssetAtPath<
                    EchoSaveConfiguration>(
                    TargetPath);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Created));
            Assert.That(created, Is.Not.Null);
            Assert.That(
                created.SchemaVersion,
                Is.EqualTo(
                    EchoSaveConfiguration.CurrentSchemaVersion));
            Assert.That(
                created.StorageRootDirectoryName,
                Is.EqualTo("ChronicleData"));
            Assert.That(
                created.SlotPolicyMode,
                Is.EqualTo(
                    SaveSlotPolicyMode.FixedMultiSlot));
            Assert.That(
                created.FixedSlotCount,
                Is.EqualTo(6));

            Assert.That(
                AssetsUnderTestFolder()
                    .Count(
                        path =>
                            path.EndsWith(
                                ".asset",
                                System.StringComparison
                                    .OrdinalIgnoreCase)),
                Is.EqualTo(1));

            Assert.That(
                Resources.FindObjectsOfTypeAll<
                    EchoSaveRoot>().Length,
                Is.EqualTo(rootCountBefore));
        }


        [Test]
        public void Apply_DoesNotCreateProductionStorageDirectory()
        {
            string uniqueRoot =
                "EchoSaveM501_" +
                System.Guid.NewGuid()
                    .ToString("N");

            string absoluteStoragePath =
                System.IO.Path.Combine(
                    Application.persistentDataPath,
                    uniqueRoot);

            Assert.That(
                System.IO.Directory.Exists(
                    absoluteStoragePath),
                Is.False);

            EchoSaveSetupPlan plan =
                service.Preview(
                    new EchoSaveSetupRequest(
                        TargetPath,
                        uniqueRoot,
                        SaveSlotPolicyMode
                            .SingleSlot,
                        4,
                        64,
                        64));

            EchoSaveSetupResult result =
                service.Apply(plan);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Created));
            Assert.That(
                System.IO.Directory.Exists(
                    absoluteStoragePath),
                Is.False);
        }

        [Test]
        public void Apply_SecondAttemptDoesNotOverwriteExistingAsset()
        {
            EchoSaveSetupPlan firstPlan =
                service.Preview(
                    ValidRequest());
            EchoSaveSetupResult first =
                service.Apply(firstPlan);

            EchoSaveConfiguration created =
                first.CreatedConfiguration;
            string before =
                EditorJsonUtility.ToJson(
                    created);

            EchoSaveSetupPlan secondPlan =
                service.Preview(
                    new EchoSaveSetupRequest(
                        TargetPath,
                        "DifferentRoot",
                        SaveSlotPolicyMode
                            .SingleSlot,
                        2,
                        3,
                        4));

            EchoSaveSetupResult second =
                service.Apply(secondPlan);

            EchoSaveConfiguration after =
                AssetDatabase.LoadAssetAtPath<
                    EchoSaveConfiguration>(
                    TargetPath);

            Assert.That(
                second.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Rejected));
            Assert.That(
                EditorJsonUtility.ToJson(after),
                Is.EqualTo(before));
        }

        [Test]
        public void Apply_StalePreviewRejectsWhenTargetBecomesOccupied()
        {
            EchoSaveSetupPlan plan =
                service.Preview(
                    ValidRequest());

            EchoSaveConfiguration competing =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();
            AssetDatabase.CreateAsset(
                competing,
                TargetPath);
            AssetDatabase.SaveAssets();

            EchoSaveSetupResult result =
                service.Apply(plan);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveSetupResultStatus.Rejected));
        }

        private static EchoSaveSetupRequest ValidRequest()
        {
            return new EchoSaveSetupRequest(
                TargetPath,
                "EchoSave",
                SaveSlotPolicyMode
                    .ConfigurableMultiSlot,
                4,
                64,
                64);
        }

        private static string[] AssetsUnderTestFolder()
        {
            return AssetDatabase
                .GetAllAssetPaths()
                .Where(
                    path =>
                        path.StartsWith(
                            TestFolder,
                            System.StringComparison
                                .Ordinal))
                .OrderBy(
                    path => path,
                    System.StringComparer.Ordinal)
                .ToArray();
        }

        private static void DeleteTestFolder()
        {
            if (AssetDatabase.IsValidFolder(
                    TestFolder))
            {
                AssetDatabase.DeleteAsset(
                    TestFolder);
                AssetDatabase.Refresh();
            }
        }
    }
}
