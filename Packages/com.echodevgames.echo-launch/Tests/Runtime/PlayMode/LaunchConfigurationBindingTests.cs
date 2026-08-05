//----- LaunchConfigurationBindingTests.cs START -----

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class LaunchConfigurationBindingTests
    {
        private static readonly FieldInfo
            RootConfigurationField =
                typeof(EchoLaunchRoot).GetField(
                    "configuration",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationIdField =
                typeof(EchoLaunchConfiguration).GetField(
                    "configurationId",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            SchemaVersionField =
                typeof(EchoLaunchConfiguration).GetField(
                    "schemaVersion",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationDestinationField =
                typeof(EchoLaunchConfiguration).GetField(
                    "initialDestination",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationIdField =
                typeof(LaunchDestination).GetField(
                    "destinationId",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationSchemaVersionField =
                typeof(LaunchDestination).GetField(
                    "schemaVersion",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationDisplayNameField =
                typeof(LaunchDestination).GetField(
                    "displayName",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationScenePathField =
                typeof(LaunchDestination).GetField(
                    "scenePath",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private readonly List<GameObject> createdObjects =
            new List<GameObject>();

        private readonly List<EchoLaunchConfiguration>
            createdConfigurations =
                new List<EchoLaunchConfiguration>();

        private readonly List<LaunchDestination>
            createdDestinations =
                new List<LaunchDestination>();

        [SetUp]
        public void SetUp()
        {
            LaunchAuthorityClaim.Reset();

            Assert.That(
                RootConfigurationField,
                Is.Not.Null);

            Assert.That(
                ConfigurationIdField,
                Is.Not.Null);

            Assert.That(
                SchemaVersionField,
                Is.Not.Null);

            Assert.That(
                ConfigurationDestinationField,
                Is.Not.Null);

            Assert.That(
                DestinationIdField,
                Is.Not.Null);

            Assert.That(
                DestinationSchemaVersionField,
                Is.Not.Null);

            Assert.That(
                DestinationDisplayNameField,
                Is.Not.Null);

            Assert.That(
                DestinationScenePathField,
                Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            for (int index = createdObjects.Count - 1;
                 index >= 0;
                 index--)
            {
                GameObject target =
                    createdObjects[index];

                if (target != null)
                {
                    Object.DestroyImmediate(target);
                }
            }

            for (int index =
                     createdConfigurations.Count - 1;
                 index >= 0;
                 index--)
            {
                EchoLaunchConfiguration configuration =
                    createdConfigurations[index];

                if (configuration != null)
                {
                    Object.DestroyImmediate(
                        configuration);
                }
            }

            for (int index =
                     createdDestinations.Count - 1;
                 index >= 0;
                 index--)
            {
                LaunchDestination destination =
                    createdDestinations[index];

                if (destination != null)
                {
                    Object.DestroyImmediate(
                        destination);
                }
            }

            createdObjects.Clear();
            createdConfigurations.Clear();
            createdDestinations.Clear();

            LaunchAuthorityClaim.Reset();

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void NewConfigurationIdUsesCanonicalFormat()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            Assert.That(
                configuration.ConfigurationId,
                Does.Match("^[0-9a-f]{32}$"));
        }

        [Test]
        public void SeparateConfigurationsReceiveDifferentIds()
        {
            EchoLaunchConfiguration first =
                CreateConfiguration();

            EchoLaunchConfiguration second =
                CreateConfiguration();

            Assert.That(
                second.ConfigurationId,
                Is.Not.EqualTo(
                    first.ConfigurationId));
        }

        [Test]
        public void ConfigurationIdRemainsStableAcrossRepeatedReads()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            string firstRead =
                configuration.ConfigurationId;

            string secondRead =
                configuration.ConfigurationId;

            Assert.That(
                secondRead,
                Is.EqualTo(firstRead));
        }

        [Test]
        public void NewConfigurationUsesCurrentSchemaVersion()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(
                    EchoLaunchConfiguration
                        .CurrentSchemaVersion));
        }

        [Test]
        public void CurrentConfigurationSchemaIsFour()
        {
            Assert.That(
                EchoLaunchConfiguration
                    .CurrentSchemaVersion,
                Is.EqualTo(4));
        }

        [Test]
        public void HistoricalSchemaTwoIsUnsupportedWithoutRewrite()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            SchemaVersionField.SetValue(
                configuration,
                2);

            Assert.That(
                configuration.HasSupportedSchema,
                Is.False);

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(2));
        }

        [Test]
        public void HistoricalSchemaThreeIsUnsupportedWithoutRewrite()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            SchemaVersionField.SetValue(
                configuration,
                3);

            Assert.That(
                configuration.HasSupportedSchema,
                Is.False);

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(3));
        }

        [Test]
        public void NewDestinationUsesSchemaOneAndCanonicalIdentity()
        {
            LaunchDestination destination =
                CreateDestination();

            Assert.That(
                LaunchDestination.CurrentSchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                destination.SchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                destination.DestinationId,
                Does.Match("^[0-9a-f]{32}$"));

            Assert.That(
                destination.HasValidIdentity,
                Is.True);
        }

        [Test]
        public void ConfigurationExposesAssignedInitialDestination()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            LaunchDestination destination =
                CreateDestination();

            ConfigurationDestinationField.SetValue(
                configuration,
                destination);

            Assert.That(
                configuration.InitialDestination,
                Is.SameAs(destination));
        }

        [Test]
        public void NewConfigurationIdentityIsValid()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            Assert.That(
                configuration.HasValidIdentity,
                Is.True);
        }

        [Test]
        public void NewConfigurationSchemaIsSupported()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            Assert.That(
                configuration.HasSupportedSchema,
                Is.True);
        }

        [Test]
        public void MalformedIdentityIsInvalidWithoutRepair()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            const string malformedId =
                "NOT-A-CANONICAL-CONFIGURATION-ID";

            ConfigurationIdField.SetValue(
                configuration,
                malformedId);

            Assert.That(
                configuration.HasValidIdentity,
                Is.False);

            Assert.That(
                configuration.ConfigurationId,
                Is.EqualTo(malformedId));
        }

        [Test]
        public void UnsupportedSchemaIsUnsupportedWithoutRewrite()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            int unsupportedVersion =
                EchoLaunchConfiguration
                    .CurrentSchemaVersion +
                1;

            SchemaVersionField.SetValue(
                configuration,
                unsupportedVersion);

            Assert.That(
                configuration.HasSupportedSchema,
                Is.False);

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(
                    unsupportedVersion));
        }

        [Test]
        public void AuthorityExposesAssignedConfiguration()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            EchoLaunchRoot authority =
                CreateRoot(
                    "Configured Authority",
                    configuration);

            Assert.That(
                authority.Configuration,
                Is.SameAs(configuration));
        }

        [Test]
        public void AuthorityWithoutAssignmentExposesNull()
        {
            EchoLaunchRoot authority =
                CreateRoot(
                    "Unconfigured Authority",
                    null);

            Assert.That(
                authority.IsAuthoritative,
                Is.True);

            Assert.That(
                authority.Configuration,
                Is.Null);
        }

        [Test]
        public void DuplicateRootWithAssignmentExposesNull()
        {
            EchoLaunchConfiguration authorityConfiguration =
                CreateConfiguration();

            EchoLaunchConfiguration duplicateConfiguration =
                CreateConfiguration();

            CreateRoot(
                "Authority",
                authorityConfiguration);

            ExpectDuplicateWarning();

            EchoLaunchRoot duplicate =
                CreateRoot(
                    "Duplicate",
                    duplicateConfiguration);

            Assert.That(
                duplicate.IsAuthoritative,
                Is.False);

            Assert.That(
                duplicate.Configuration,
                Is.Null);
        }

        [Test]
        public void DuplicateDoesNotReplaceAuthorityConfiguration()
        {
            EchoLaunchConfiguration authorityConfiguration =
                CreateConfiguration();

            EchoLaunchConfiguration duplicateConfiguration =
                CreateConfiguration();

            EchoLaunchRoot authority =
                CreateRoot(
                    "Authority",
                    authorityConfiguration);

            ExpectDuplicateWarning();

            CreateRoot(
                "Duplicate",
                duplicateConfiguration);

            Assert.That(
                EchoLaunchRoot.Current,
                Is.SameAs(authority));

            Assert.That(
                authority.Configuration,
                Is.SameAs(
                    authorityConfiguration));
        }

        [Test]
        public void ResetHidesFormerAuthorityConfiguration()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            EchoLaunchRoot formerAuthority =
                CreateRoot(
                    "Former Authority",
                    configuration);

            LaunchAuthorityClaim.Reset();

            Assert.That(
                formerAuthority.IsAuthoritative,
                Is.False);

            Assert.That(
                formerAuthority.Configuration,
                Is.Null);
        }

        [Test]
        public void FreshRootAfterResetExposesOwnConfiguration()
        {
            EchoLaunchConfiguration firstConfiguration =
                CreateConfiguration();

            EchoLaunchConfiguration secondConfiguration =
                CreateConfiguration();

            EchoLaunchRoot first =
                CreateRoot(
                    "First Authority",
                    firstConfiguration);

            LaunchAuthorityClaim.Reset();

            EchoLaunchRoot second =
                CreateRoot(
                    "Second Authority",
                    secondConfiguration);

            Assert.That(
                first.Configuration,
                Is.Null);

            Assert.That(
                second.IsAuthoritative,
                Is.True);

            Assert.That(
                second.Configuration,
                Is.SameAs(
                    secondConfiguration));
        }

        [Test]
        public void AuthorityExposesAssignedInitialDestination()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            LaunchDestination destination =
                CreateDestination();

            ConfigurationDestinationField.SetValue(
                configuration,
                destination);

            EchoLaunchRoot authority =
                CreateRoot(
                    "Destination Authority",
                    configuration);

            Assert.That(
                authority.InitialDestination,
                Is.SameAs(destination));
        }

        [Test]
        public void DuplicateRootExposesNoInitialDestination()
        {
            EchoLaunchConfiguration authorityConfiguration =
                CreateConfiguration();

            ConfigurationDestinationField.SetValue(
                authorityConfiguration,
                CreateDestination());

            EchoLaunchConfiguration duplicateConfiguration =
                CreateConfiguration();

            ConfigurationDestinationField.SetValue(
                duplicateConfiguration,
                CreateDestination());

            CreateRoot(
                "Destination Authority",
                authorityConfiguration);

            ExpectDuplicateWarning();

            EchoLaunchRoot duplicate =
                CreateRoot(
                    "Destination Duplicate",
                    duplicateConfiguration);

            Assert.That(
                duplicate.InitialDestination,
                Is.Null);
        }

        [Test]
        public void RootLifecycleDoesNotMutateDestination()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            LaunchDestination destination =
                CreateDestination();

            ConfigurationDestinationField.SetValue(
                configuration,
                destination);

            string originalId =
                destination.DestinationId;

            int originalSchema =
                destination.SchemaVersion;

            string originalDisplayName =
                destination.DisplayName;

            string originalScenePath =
                destination.ScenePath;

            EchoLaunchRoot root =
                CreateRoot(
                    "Destination Lifecycle Root",
                    configuration);

            Assert.That(
                root.InitialDestination,
                Is.SameAs(destination));

            Object.DestroyImmediate(
                root.gameObject);

            Assert.That(
                destination.DestinationId,
                Is.EqualTo(originalId));

            Assert.That(
                destination.SchemaVersion,
                Is.EqualTo(originalSchema));

            Assert.That(
                destination.DisplayName,
                Is.EqualTo(originalDisplayName));

            Assert.That(
                destination.ScenePath,
                Is.EqualTo(originalScenePath));
        }

        [Test]
        public void RootLifecycleDoesNotMutateConfiguration()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration();

            string originalId =
                configuration.ConfigurationId;

            int originalSchema =
                configuration.SchemaVersion;

            EchoLaunchRoot root =
                CreateRoot(
                    "Lifecycle Root",
                    configuration);

            Assert.That(
                root.Configuration,
                Is.SameAs(configuration));

            Object.DestroyImmediate(
                root.gameObject);

            Assert.That(
                configuration.ConfigurationId,
                Is.EqualTo(originalId));

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(originalSchema));
        }

        private EchoLaunchConfiguration
            CreateConfiguration()
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            createdConfigurations.Add(
                configuration);

            return configuration;
        }

        private LaunchDestination CreateDestination()
        {
            LaunchDestination destination =
                ScriptableObject.CreateInstance<
                    LaunchDestination>();

            createdDestinations.Add(
                destination);

            DestinationDisplayNameField.SetValue(
                destination,
                "Configuration Destination");

            DestinationScenePathField.SetValue(
                destination,
                "Assets/Scenes/ConfigurationDestination.unity");

            return destination;
        }

        private EchoLaunchRoot CreateRoot(
            string name,
            EchoLaunchConfiguration configuration)
        {
            GameObject target =
                new GameObject(name);

            target.SetActive(false);

            createdObjects.Add(target);

            EchoLaunchRoot root =
                target.AddComponent<EchoLaunchRoot>();

            RootConfigurationField.SetValue(
                root,
                configuration);

            target.SetActive(true);

            return root;
        }

        private static void ExpectDuplicateWarning()
        {
            LogAssert.Expect(
                LogType.Warning,
                "[ELAUNCH-ROOT-001] " +
                "Duplicate EchoLaunchRoot rejected. " +
                "The first valid root remains authoritative.");
        }
    }
}

//----- LaunchConfigurationBindingTests.cs END -----
