using System;
using System.Linq;
using EchoDevGames.EchoSave.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveValidationTests
    {
        private const string TestFolder =
            "Assets/__EchoSaveM5_01ValidationTests";
        private const string ConfigPath =
            TestFolder + "/Configuration.asset";

        private EchoSaveValidationService service;
        private GameObject firstRootObject;
        private GameObject secondRootObject;

        [SetUp]
        public void SetUp()
        {
            DeleteTestFolder();

            AssetDatabase.CreateFolder(
                "Assets",
                "__EchoSaveM5_01ValidationTests");

            service =
                new EchoSaveValidationService();
        }

        [TearDown]
        public void TearDown()
        {
            if (firstRootObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    firstRootObject);
            }

            if (secondRootObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    secondRootObject);
            }

            DeleteTestFolder();
        }

        [Test]
        public void MissingConfigurationProducesVal001()
        {
            EchoSaveValidationReport report =
                service.Validate(null);

            Assert.That(
                report.Issues.Any(
                    issue =>
                        issue.CheckId ==
                        EchoSaveValidationService
                            .MissingConfigurationCheckId),
                Is.True);
        }

        [TestCase("")]
        [TestCase("../Unsafe")]
        [TestCase("Unsafe/Child")]
        public void UnsafeOrEmptyRootProducesVal002(
            string root)
        {
            EchoSaveConfiguration configuration =
                CreateConfiguration();

            SetSerializedString(
                configuration,
                "storageRootDirectoryName",
                root);

            EchoSaveValidationReport report =
                service.Validate(
                    configuration);

            Assert.That(
                report.Issues.Any(
                    issue =>
                        issue.CheckId ==
                        EchoSaveValidationService
                            .UnsafeStorageRootCheckId),
                Is.True);
        }

        [Test]
        public void DuplicateLoadedSceneRootsProduceVal003()
        {
            firstRootObject =
                new GameObject(
                    "ChronicleRootA");
            firstRootObject.SetActive(false);
            firstRootObject.AddComponent<
                EchoSaveRoot>();

            secondRootObject =
                new GameObject(
                    "ChronicleRootB");
            secondRootObject.SetActive(false);
            secondRootObject.AddComponent<
                EchoSaveRoot>();

            EchoSaveValidationReport report =
                service.Validate(
                    CreateConfiguration());

            Assert.That(
                report.Issues.Any(
                    issue =>
                        issue.CheckId ==
                        EchoSaveValidationService
                            .DuplicateRootsCheckId),
                Is.True);
        }

        [Test]
        public void RuntimeAssemblyIsolationPassesVal009()
        {
            EchoSaveValidationReport report =
                service.Validate(
                    CreateConfiguration());

            Assert.That(
                report.Issues.Any(
                    issue =>
                        issue.CheckId ==
                        EchoSaveValidationService
                            .RuntimeEditorReferenceCheckId),
                Is.False);
        }

        [Test]
        public void InvalidCurrentSlotPolicyProducesVal015()
        {
            EchoSaveConfiguration configuration =
                CreateConfiguration();

            SetSerializedInt(
                configuration,
                "configuredSlotLimit",
                0);

            EchoSaveValidationReport report =
                service.Validate(
                    configuration);

            Assert.That(
                report.Issues.Any(
                    issue =>
                        issue.CheckId ==
                        EchoSaveValidationService
                            .InvalidSlotPolicyCheckId),
                Is.True);
        }



        [Test]
        public void UndefinedCurrentSlotPolicyProducesVal015()
        {
            EchoSaveConfiguration configuration =
                CreateConfiguration();

            System.Reflection.FieldInfo field =
                typeof(EchoSaveConfiguration)
                    .GetField(
                        "slotPolicyMode",
                        System.Reflection.BindingFlags
                            .Instance |
                        System.Reflection.BindingFlags
                            .NonPublic);

            Assert.That(field, Is.Not.Null);

            field.SetValue(
                configuration,
                (SaveSlotPolicyMode)999);

            EchoSaveValidationReport report =
                service.Validate(
                    configuration);

            Assert.That(
                report.Issues.Any(
                    issue =>
                        issue.CheckId ==
                        EchoSaveValidationService
                            .InvalidSlotPolicyCheckId),
                Is.True);
        }

        [Test]
        public void IssueOrderingIsDeterministic()
        {
            EchoSaveConfiguration configuration =
                CreateConfiguration();

            SetSerializedString(
                configuration,
                "storageRootDirectoryName",
                string.Empty);
            SetSerializedInt(
                configuration,
                "configuredSlotLimit",
                0);

            EchoSaveValidationReport first =
                service.Validate(
                    configuration);
            EchoSaveValidationReport second =
                service.Validate(
                    configuration);

            string[] firstTruth =
                first.Issues
                    .Select(ToStableTruth)
                    .ToArray();
            string[] secondTruth =
                second.Issues
                    .Select(ToStableTruth)
                    .ToArray();

            Assert.That(
                secondTruth,
                Is.EqualTo(firstTruth));

            string[] sorted =
                first.Issues
                    .OrderBy(
                        issue =>
                            issue.Severity)
                    .ThenBy(
                        issue =>
                            issue.CheckId,
                        StringComparer.Ordinal)
                    .ThenBy(
                        issue =>
                            issue.Context,
                        StringComparer.Ordinal)
                    .Select(ToStableTruth)
                    .ToArray();

            Assert.That(
                firstTruth,
                Is.EqualTo(sorted));
        }

        [Test]
        public void ValidationPerformsZeroProjectMutation()
        {
            EchoSaveConfiguration configuration =
                CreateConfiguration();

            string beforeJson =
                EditorJsonUtility.ToJson(
                    configuration);
            string[] beforeAssets =
                AssetDatabase
                    .GetAllAssetPaths()
                    .OrderBy(
                        path => path,
                        StringComparer.Ordinal)
                    .ToArray();

            service.Validate(
                configuration);
            service.Validate(
                configuration);

            string afterJson =
                EditorJsonUtility.ToJson(
                    configuration);
            string[] afterAssets =
                AssetDatabase
                    .GetAllAssetPaths()
                    .OrderBy(
                        path => path,
                        StringComparer.Ordinal)
                    .ToArray();

            Assert.That(
                afterJson,
                Is.EqualTo(beforeJson));
            Assert.That(
                afterAssets,
                Is.EqualTo(beforeAssets));
        }

        private static EchoSaveConfiguration
            CreateConfiguration()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            AssetDatabase.CreateAsset(
                configuration,
                ConfigPath);
            AssetDatabase.SaveAssets();

            return configuration;
        }

        private static void SetSerializedString(
            UnityEngine.Object target,
            string propertyName,
            string value)
        {
            var serialized =
                new SerializedObject(target);

            serialized
                .FindProperty(propertyName)
                .stringValue =
                value;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        private static void SetSerializedInt(
            UnityEngine.Object target,
            string propertyName,
            int value)
        {
            var serialized =
                new SerializedObject(target);

            serialized
                .FindProperty(propertyName)
                .intValue =
                value;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        private static string ToStableTruth(
            EchoSaveValidationIssue issue)
        {
            return string.Concat(
                ((int)issue.Severity).ToString(),
                "|",
                issue.CheckId,
                "|",
                issue.Context,
                "|",
                issue.Message);
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
