using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveSlotPolicyConfigurationTests
    {
        [Test]
        public void SchemaOneMapsToLegacyCapacityWithoutMutatingSerializedPolicy()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();
            try
            {
                configuration.SetDefinitionForTesting(
                    EchoSaveConfiguration.LegacySchemaVersion,
                    "EchoSave");
                configuration.SetSlotPolicyForTesting(
                    SaveSlotPolicyMode.SingleSlot,
                    17,
                    23,
                    31);

                Assert.That(
                    configuration.TryResolveSlotPolicy(
                        out SaveSlotPolicy policy,
                        out string message),
                    Is.True,
                    message);

                Assert.That(
                    policy.Mode,
                    Is.EqualTo(
                        SaveSlotPolicyMode.ConfigurableMultiSlot));
                Assert.That(policy.EffectiveCapacity, Is.EqualTo(64));
                Assert.That(
                    policy.SourceConfigurationSchema,
                    Is.EqualTo(
                        EchoSaveConfiguration.LegacySchemaVersion));
                Assert.That(policy.CompatibilityMapped, Is.True);

                Assert.That(
                    configuration.SlotPolicyMode,
                    Is.EqualTo(SaveSlotPolicyMode.SingleSlot));
                Assert.That(configuration.FixedSlotCount, Is.EqualTo(17));
                Assert.That(configuration.ConfiguredSlotLimit, Is.EqualTo(23));
                Assert.That(configuration.ProfileSafetyLimit, Is.EqualTo(31));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [TestCase(
            SaveSlotPolicyMode.SingleSlot,
            -5,
            -7,
            -9,
            1)]
        [TestCase(
            SaveSlotPolicyMode.FixedMultiSlot,
            3,
            -7,
            -9,
            3)]
        [TestCase(
            SaveSlotPolicyMode.ConfigurableMultiSlot,
            -5,
            5,
            -9,
            5)]
        [TestCase(
            SaveSlotPolicyMode.BoundedProfiles,
            -5,
            -7,
            7,
            7)]
        public void SchemaTwoResolvesOnlyTheActiveCapacityField(
            SaveSlotPolicyMode mode,
            int fixedCount,
            int configuredLimit,
            int profileLimit,
            int expectedCapacity)
        {
            EchoSaveConfiguration configuration =
                CreateSchemaTwoConfiguration(
                    mode,
                    fixedCount,
                    configuredLimit,
                    profileLimit);
            try
            {
                Assert.That(
                    configuration.TryResolveSlotPolicy(
                        out SaveSlotPolicy policy,
                        out string message),
                    Is.True,
                    message);

                Assert.That(policy.Mode, Is.EqualTo(mode));
                Assert.That(
                    policy.EffectiveCapacity,
                    Is.EqualTo(expectedCapacity));
                Assert.That(
                    policy.SourceConfigurationSchema,
                    Is.EqualTo(
                        EchoSaveConfiguration.SlotPolicySchemaVersion));
                Assert.That(policy.CompatibilityMapped, Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [TestCase(
            SaveSlotPolicyMode.FixedMultiSlot,
            1,
            4,
            4)]
        [TestCase(
            SaveSlotPolicyMode.ConfigurableMultiSlot,
            4,
            0,
            4)]
        [TestCase(
            SaveSlotPolicyMode.BoundedProfiles,
            4,
            4,
            0)]
        public void SchemaTwoRejectsInvalidActiveCapacity(
            SaveSlotPolicyMode mode,
            int fixedCount,
            int configuredLimit,
            int profileLimit)
        {
            EchoSaveConfiguration configuration =
                CreateSchemaTwoConfiguration(
                    mode,
                    fixedCount,
                    configuredLimit,
                    profileLimit);
            try
            {
                Assert.That(
                    configuration.TryResolveSlotPolicy(
                        out SaveSlotPolicy policy,
                        out string message),
                    Is.False);
                Assert.That(policy, Is.Null);
                Assert.That(message, Is.Not.Empty);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void SchemaTwoRejectsUndefinedPolicyMode()
        {
            EchoSaveConfiguration configuration =
                CreateSchemaTwoConfiguration(
                    (SaveSlotPolicyMode)999,
                    4,
                    4,
                    4);
            try
            {
                Assert.That(
                    configuration.TryResolveSlotPolicy(
                        out SaveSlotPolicy policy,
                        out string message),
                    Is.False);
                Assert.That(policy, Is.Null);
                Assert.That(message, Does.Contain("undefined"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void FutureConfigurationSchemaFailsClosed()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();
            try
            {
                configuration.SetDefinitionForTesting(
                    EchoSaveConfiguration.CurrentSchemaVersion + 1,
                    "EchoSave");

                Assert.That(
                    configuration.TryResolveSlotPolicy(
                        out SaveSlotPolicy policy,
                        out string message),
                    Is.False);
                Assert.That(policy, Is.Null);
                Assert.That(message, Does.Contain("unsupported"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void InvalidSchemaTwoPolicyBlocksBeforeStorageFactorySideEffects()
        {
            using (AutosaveServiceTestEnvironment env =
                   new AutosaveServiceTestEnvironment())
            {
                env.Configuration.SetSlotPolicyForTesting(
                    SaveSlotPolicyMode.FixedMultiSlot,
                    1,
                    4,
                    4);

                CountingBackendFactory factory =
                    new CountingBackendFactory(
                        env.Storage.Backend);
                env.Service.SetStorageBackendFactory(
                    factory);

                EchoSaveLifecycleResult result =
                    env.Service.InitializeCore();

                Assert.That(
                    result.Status,
                    Is.EqualTo(EchoSaveLifecycleStatus.Blocked));
                Assert.That(factory.TryCreateCalls, Is.Zero);
                Assert.That(
                    env.Service.SlotPolicyForTesting,
                    Is.Null);
            }
        }

        [TestCase(
            SaveSlotPolicyMode.SingleSlot,
            9,
            9,
            9,
            1)]
        [TestCase(
            SaveSlotPolicyMode.FixedMultiSlot,
            2,
            9,
            9,
            2)]
        [TestCase(
            SaveSlotPolicyMode.ConfigurableMultiSlot,
            9,
            2,
            9,
            2)]
        [TestCase(
            SaveSlotPolicyMode.BoundedProfiles,
            9,
            9,
            2,
            2)]
        public void ServiceEnforcesResolvedCapacityForEveryMode(
            SaveSlotPolicyMode mode,
            int fixedCount,
            int configuredLimit,
            int profileLimit,
            int expectedCapacity)
        {
            using (AutosaveServiceTestEnvironment env =
                   new AutosaveServiceTestEnvironment())
            {
                env.Configuration.SetSlotPolicyForTesting(
                    mode,
                    fixedCount,
                    configuredLimit,
                    profileLimit);

                Assert.That(
                    env.Initialize(false).Succeeded,
                    Is.True);
                Assert.That(
                    env.Service.SlotPolicyForTesting.EffectiveCapacity,
                    Is.EqualTo(expectedCapacity));

                for (int i = 0; i < expectedCapacity; i++)
                {
                    SaveSlotCreateResult created =
                        env.Service.CreateSlotSynchronouslyForTesting(
                            Request("Policy Slot " + i));

                    Assert.That(
                        created.Status,
                        Is.EqualTo(SaveSlotCreateStatus.Succeeded),
                        created.Message);
                }

                SaveSlotCreateResult rejected =
                    env.Service.CreateSlotSynchronouslyForTesting(
                        Request("Over Capacity"));

                Assert.That(
                    rejected.Status,
                    Is.EqualTo(
                        SaveSlotCreateStatus.CapacityReached));
                Assert.That(
                    env.Service.GetCatalogSnapshot().Count,
                    Is.EqualTo(expectedCapacity));
            }
        }

        [Test]
        public void CreateAndDuplicateShareSingleSessionCapacityAuthority()
        {
            using (AutosaveServiceTestEnvironment env =
                   new AutosaveServiceTestEnvironment())
            {
                env.Configuration.SetSlotPolicyForTesting(
                    SaveSlotPolicyMode.SingleSlot,
                    99,
                    99,
                    99);

                Assert.That(
                    env.Initialize(false).Succeeded,
                    Is.True);

                SaveSlotCreateResult created =
                    env.Service.CreateSlotSynchronouslyForTesting(
                        Request("Only Slot"));
                Assert.That(created.Succeeded, Is.True, created.Message);

                SaveSlotDuplicateResult duplicate =
                    env.Service.DuplicateSlotSynchronouslyForTesting(
                        new SaveSlotDuplicateRequest(
                            created.SlotId));
                SaveSlotCreateResult secondCreate =
                    env.Service.CreateSlotSynchronouslyForTesting(
                        Request("Second Slot"));

                Assert.That(
                    duplicate.Status,
                    Is.EqualTo(
                        SaveSlotDuplicateStatus.CapacityReached));
                Assert.That(
                    secondCreate.Status,
                    Is.EqualTo(
                        SaveSlotCreateStatus.CapacityReached));
                Assert.That(
                    env.Service.GetCatalogSnapshot().Count,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void InitializedPolicySnapshotDoesNotFollowLaterConfigurationMutation()
        {
            using (AutosaveServiceTestEnvironment env =
                   new AutosaveServiceTestEnvironment())
            {
                env.Configuration.SetSlotPolicyForTesting(
                    SaveSlotPolicyMode.ConfigurableMultiSlot,
                    9,
                    2,
                    9);

                Assert.That(
                    env.Initialize(false).Succeeded,
                    Is.True);

                SaveSlotPolicy sessionPolicy =
                    env.Service.SlotPolicyForTesting;
                Assert.That(sessionPolicy.EffectiveCapacity, Is.EqualTo(2));

                env.Configuration.SetSlotPolicyForTesting(
                    SaveSlotPolicyMode.ConfigurableMultiSlot,
                    9,
                    5,
                    9);

                Assert.That(
                    env.Service.SlotPolicyForTesting,
                    Is.SameAs(sessionPolicy));
                Assert.That(
                    env.Service.SlotPolicyForTesting.EffectiveCapacity,
                    Is.EqualTo(2));

                Assert.That(
                    env.Service.CreateSlotSynchronouslyForTesting(
                        Request("Frozen A")).Succeeded,
                    Is.True);
                Assert.That(
                    env.Service.CreateSlotSynchronouslyForTesting(
                        Request("Frozen B")).Succeeded,
                    Is.True);

                Assert.That(
                    env.Service.CreateSlotSynchronouslyForTesting(
                        Request("Frozen C")).Status,
                    Is.EqualTo(
                        SaveSlotCreateStatus.CapacityReached));
            }
        }

        [Test]
        public void InitializedRuntimePolicySnapshotDoesNotFollowLaterConfigurationMutation()
        {
            using (AutosaveServiceTestEnvironment env =
                   new AutosaveServiceTestEnvironment())
            {
                env.Configuration.SetRuntimePolicyForTesting(
                    7,
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    111,
                    222,
                    333,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                Assert.That(env.Initialize(false).Succeeded, Is.True);

                EchoSaveRuntimePolicy sessionPolicy =
                    env.Service.RuntimePolicyForTesting;

                Assert.That(sessionPolicy, Is.Not.Null);
                Assert.That(
                    sessionPolicy.RetentionPolicy.MaxTotalGenerations,
                    Is.EqualTo(7));
                Assert.That(
                    sessionPolicy.Limits.CatalogScanLimit,
                    Is.EqualTo(111));

                env.Configuration.SetRuntimePolicyForTesting(
                    12,
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    444,
                    555,
                    666,
                    EchoSaveRecoveryPolicyMode.ManualOnly);

                Assert.That(
                    env.Service.RuntimePolicyForTesting,
                    Is.SameAs(sessionPolicy));
                Assert.That(
                    env.Service.RuntimePolicyForTesting
                        .RetentionPolicy.MaxTotalGenerations,
                    Is.EqualTo(7));
                Assert.That(
                    env.Service.RuntimePolicyForTesting
                        .Limits.CatalogScanLimit,
                    Is.EqualTo(111));
            }
        }

        [Test]
        public void TrashDoesNotCountAndConfirmedDeleteFreesCapacity()
        {
            using (SaveSlotDeletionTestEnvironment env =
                   new SaveSlotDeletionTestEnvironment())
            {
                SaveSlotDeletionTestEnvironment.CreatedSource source =
                    env.CreateSource(
                        "Capacity Source");

                SaveSlotDeletionCoordinator deletion =
                    env.Coordinator();
                SaveDeletionPlan plan =
                    deletion.Prepare(
                        source.SlotId);
                SaveSlotDeleteResult deleted =
                    deletion.Confirm(
                        plan);

                Assert.That(deleted.Succeeded, Is.True);
                Assert.That(deleted.DeleteCommitted, Is.True);
                Assert.That(deleted.CatalogReconciled, Is.True);
                Assert.That(env.TrashRecordCount(), Is.EqualTo(1));
                Assert.That(env.Catalog.Snapshot.Count, Is.Zero);

                SaveTechnicalSlotCreationCoordinator creation =
                    new SaveTechnicalSlotCreationCoordinator(
                        env.Catalog,
                        env.Publication,
                        1,
                        2,
                        SaveSlotId.NewId);
                SaveTechnicalSlotCreateResult replacement =
                    creation.Create(
                        SlotCreationTestEnvironment.Request(
                            "Replacement",
                            "com.example.slot-policy",
                            "1.0.0",
                            "r2-delete"));

                Assert.That(
                    replacement.Status,
                    Is.EqualTo(
                        SaveTechnicalSlotCreateStatus.Succeeded),
                    replacement.Message);
                Assert.That(env.TrashRecordCount(), Is.EqualTo(1));
                Assert.That(env.Catalog.Snapshot.Count, Is.EqualTo(1));
            }
        }

        private static EchoSaveConfiguration
            CreateSchemaTwoConfiguration(
                SaveSlotPolicyMode mode,
                int fixedCount,
                int configuredLimit,
                int profileLimit)
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();
            configuration.SetDefinitionForTesting(
                EchoSaveConfiguration.SlotPolicySchemaVersion,
                "EchoSave");
            configuration.SetSlotPolicyForTesting(
                mode,
                fixedCount,
                configuredLimit,
                profileLimit);

            return configuration;
        }

        private static SaveSlotCreateRequest Request(
            string displayName) =>
            new SaveSlotCreateRequest(
                displayName,
                "com.example.slot-policy",
                "1.0.0",
                "r2");

        private sealed class CountingBackendFactory :
            IEchoSaveStorageBackendFactory
        {
            private readonly ISaveStorageBackend backend;

            internal CountingBackendFactory(
                ISaveStorageBackend backend)
            {
                this.backend = backend;
            }

            internal int TryCreateCalls { get; private set; }

            public SaveStorageResult TryCreate(
                EchoSaveConfiguration configuration,
                out ISaveStorageBackend created)
            {
                TryCreateCalls++;
                created = backend;

                return SaveStorageResult.Success(
                    "Slot-policy test backend created.");
            }
        }
    }
}
