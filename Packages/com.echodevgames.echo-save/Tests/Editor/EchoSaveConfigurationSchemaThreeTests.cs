using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveConfigurationSchemaThreeTests
    {
        [Test]
        public void CurrentConfigurationResolvesSchemaThreeDefaults()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            try
            {
                Assert.That(
                    EchoSaveConfiguration.CurrentSchemaVersion,
                    Is.EqualTo(3));
                Assert.That(
                    configuration.TryResolveRuntimePolicy(
                        out EchoSaveRuntimePolicy policy,
                        out string message),
                    Is.True,
                    message);
                Assert.That(
                    policy.SourceConfigurationSchema,
                    Is.EqualTo(3));
                Assert.That(
                    policy.CompatibilityMapped,
                    Is.False);
                Assert.That(
                    policy.RetentionPolicy.MaxTotalGenerations,
                    Is.EqualTo(
                        SaveRetentionPolicy
                            .DefaultTotalGenerations));
                Assert.That(
                    policy.Limits.CatalogScanLimit,
                    Is.EqualTo(
                        SaveLimitPolicy
                            .DefaultCatalogScanLimit));
                Assert.That(
                    policy.SerializerProviderId,
                    Is.EqualTo(
                        EchoSaveConfiguration
                            .DefaultSerializerProviderId));
                Assert.That(
                    policy.StorageProviderId,
                    Is.EqualTo(
                        EchoSaveConfiguration
                            .DefaultStorageProviderId));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [TestCase(EchoSaveConfiguration.LegacySchemaVersion)]
        [TestCase(EchoSaveConfiguration.SlotPolicySchemaVersion)]
        public void OlderSchemasResolveCompatibilityDefaultsWithoutMutation(
            int schema)
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            try
            {
                configuration.SetDefinitionForTesting(
                    schema,
                    "EchoSave");
                configuration.SetRuntimePolicyForTesting(
                    99,
                    "unavailable.serializer",
                    "unavailable.storage",
                    0,
                    0,
                    0,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                Assert.That(
                    configuration.TryResolveRuntimePolicy(
                        out EchoSaveRuntimePolicy policy,
                        out string message),
                    Is.True,
                    message);
                Assert.That(
                    policy.SourceConfigurationSchema,
                    Is.EqualTo(schema));
                Assert.That(policy.CompatibilityMapped, Is.True);
                Assert.That(
                    policy.RetentionPolicy.MaxTotalGenerations,
                    Is.EqualTo(
                        SaveRetentionPolicy
                            .DefaultTotalGenerations));
                Assert.That(
                    policy.Limits.RecoveryDiscoveryLimit,
                    Is.EqualTo(
                        SaveLimitPolicy
                            .DefaultRecoveryDiscoveryLimit));
                Assert.That(
                    configuration.SchemaVersion,
                    Is.EqualTo(schema));
                Assert.That(
                    configuration.MaxTotalGenerations,
                    Is.EqualTo(99));
                Assert.That(
                    configuration.SerializerProviderId,
                    Is.EqualTo("unavailable.serializer"));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void SchemaThreeResolvesConfiguredRetentionAndLimits()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    9,
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    128,
                    300,
                    301,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                Assert.That(
                    configuration.TryResolveRuntimePolicy(
                        out EchoSaveRuntimePolicy policy,
                        out string message),
                    Is.True,
                    message);
                Assert.That(
                    policy.RetentionPolicy.MaxTotalGenerations,
                    Is.EqualTo(9));
                Assert.That(
                    policy.Limits.CatalogScanLimit,
                    Is.EqualTo(128));
                Assert.That(
                    policy.Limits.RetentionDiscoveryLimit,
                    Is.EqualTo(300));
                Assert.That(
                    policy.Limits.RecoveryDiscoveryLimit,
                    Is.EqualTo(301));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [TestCase(1)]
        [TestCase(257)]
        public void SchemaThreeRejectsInvalidRetention(
            int maxGenerations)
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    maxGenerations,
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    256,
                    512,
                    512,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                Assert.That(
                    configuration.TryResolveRuntimePolicy(
                        out _,
                        out string message),
                    Is.False);
                Assert.That(message, Does.Contain("retention"));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void SchemaThreeRejectsUnavailableSerializerProvider()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    5,
                    "missing.serializer",
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    256,
                    512,
                    512,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                Assert.That(
                    configuration.TryResolveRuntimePolicy(
                        out _,
                        out string message),
                    Is.False);
                Assert.That(message, Does.Contain("serializer"));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void SchemaThreeRejectsUnavailableStorageProvider()
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    5,
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    "missing.storage",
                    256,
                    512,
                    512,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                Assert.That(
                    configuration.TryResolveRuntimePolicy(
                        out _,
                        out string message),
                    Is.False);
                Assert.That(message, Does.Contain("storage"));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [TestCase(0, 512, 512)]
        [TestCase(256, 0, 512)]
        [TestCase(256, 512, 5000)]
        public void SchemaThreeRejectsInvalidDiscoveryLimits(
            int catalog,
            int retention,
            int recovery)
        {
            EchoSaveConfiguration configuration =
                CreateCurrent();

            try
            {
                configuration.SetRuntimePolicyForTesting(
                    5,
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    catalog,
                    retention,
                    recovery,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                Assert.That(
                    configuration.TryResolveRuntimePolicy(
                        out _,
                        out string message),
                    Is.False);
                Assert.That(message, Does.Contain("limits"));
            }
            finally
            {
                Object.DestroyImmediate(configuration);
            }
        }

        [Test]
        public void DuplicateFixedSlotTemplateIdsRejectAuthoringMetadata()
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
                first.SetDefinitionForTesting(
                    "profile-a",
                    "A",
                    0);
                second.SetDefinitionForTesting(
                    "profile-a",
                    "B",
                    1);
                configuration.SetFixedSlotTemplatesForTesting(
                    first,
                    second);

                Assert.That(
                    configuration.TryValidateFixedSlotTemplates(
                        out string message),
                    Is.False);
                Assert.That(message, Does.Contain("duplicated"));
            }
            finally
            {
                Object.DestroyImmediate(first);
                Object.DestroyImmediate(second);
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
