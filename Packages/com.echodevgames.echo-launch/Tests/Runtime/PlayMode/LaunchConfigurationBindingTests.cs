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

        private readonly List<GameObject> createdObjects =
            new List<GameObject>();

        private readonly List<EchoLaunchConfiguration>
            createdConfigurations =
                new List<EchoLaunchConfiguration>();

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

            createdObjects.Clear();
            createdConfigurations.Clear();

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
