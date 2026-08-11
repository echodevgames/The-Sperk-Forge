using System.Linq;
using EchoDevGames.EchoSave.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveValidationM502Tests
    {
        private EchoSaveValidationService service;

        [SetUp]
        public void SetUp()
        {
            service =
                new EchoSaveValidationService();
        }

        [Test]
        public void InvalidRetentionProducesVal005()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    1,
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    256,
                    512,
                    512,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                EchoSaveValidationReport report =
                    service.Validate(configuration);

                Assert.That(
                    report.Issues.Any(
                        issue =>
                            issue.CheckId ==
                            EchoSaveValidationService
                                .InvalidRetentionCheckId),
                    Is.True);
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void UnavailableProvidersProduceVal006()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    5,
                    "missing.serializer",
                    "missing.storage",
                    256,
                    512,
                    512,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                EchoSaveValidationReport report =
                    service.Validate(configuration);

                Assert.That(
                    report.Issues.Count(
                        issue =>
                            issue.CheckId ==
                            EchoSaveValidationService
                                .MissingProviderCheckId),
                    Is.EqualTo(2));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void DuplicateFixedSlotTemplateIdsProduceVal004()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();
            SaveSlotTemplate first =
                ScriptableObject.CreateInstance<
                    SaveSlotTemplate>();
            SaveSlotTemplate second =
                ScriptableObject.CreateInstance<
                    SaveSlotTemplate>();

            try
            {
                first.SetDefinitionForTesting("same", "A", 0);
                second.SetDefinitionForTesting("same", "B", 1);
                configuration.SetFixedSlotTemplatesForTesting(
                    first,
                    second);

                EchoSaveValidationReport report =
                    service.Validate(configuration);

                Assert.That(
                    report.Issues.Any(
                        issue =>
                            issue.CheckId ==
                            EchoSaveValidationService
                                .DuplicateFixedSlotIdsCheckId),
                    Is.True);
            }
            finally
            {
                Object.DestroyImmediate(first);
                Object.DestroyImmediate(second);
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void InvalidDiscoveryLimitsProduceVal016()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    5,
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    0,
                    512,
                    512,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                EchoSaveValidationReport report =
                    service.Validate(configuration);

                Assert.That(
                    report.Issues.Any(
                        issue =>
                            issue.CheckId ==
                            EchoSaveValidationService
                                .InvalidLimitPolicyCheckId),
                    Is.True);
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void SchemaTwoCompatibilityDoesNotValidateUnserializedM502Fields()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetDefinitionForTesting(
                    EchoSaveConfiguration.SlotPolicySchemaVersion,
                    "EchoSave");
                configuration.SetRuntimePolicyForTesting(
                    1,
                    "missing.serializer",
                    "missing.storage",
                    0,
                    0,
                    0,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                EchoSaveValidationReport report =
                    service.Validate(configuration);

                Assert.That(
                    report.Issues.Any(
                        issue =>
                            issue.CheckId ==
                            EchoSaveValidationService.InvalidRetentionCheckId ||
                            issue.CheckId ==
                            EchoSaveValidationService.MissingProviderCheckId ||
                            issue.CheckId ==
                            EchoSaveValidationService.InvalidLimitPolicyCheckId),
                    Is.False);
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void ValidationPerformsZeroConfigurationMutation()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    1,
                    "missing.serializer",
                    "missing.storage",
                    0,
                    0,
                    0,
                    EchoSaveRecoveryPolicyMode.ManualOnly);
                string before =
                    EditorJsonUtility.ToJson(configuration);

                service.Validate(configuration);

                Assert.That(
                    EditorJsonUtility.ToJson(configuration),
                    Is.EqualTo(before));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        private static EchoSaveConfiguration CreateCurrent()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();
            configuration.SetDefinitionForTesting(
                EchoSaveConfiguration.CurrentSchemaVersion,
                "EchoSave");
            return configuration;
        }
    }
}
