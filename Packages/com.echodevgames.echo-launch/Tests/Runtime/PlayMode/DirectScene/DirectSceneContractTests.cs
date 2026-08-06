//----- DirectSceneContractTests.cs START -----

using System;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class DirectSceneContractTests
    {
        private DirectSceneTestFixture fixture;

        [SetUp]
        public void SetUp()
        {
            LaunchAuthorityClaim.Reset();
            fixture = new DirectSceneTestFixture();
        }

        [TearDown]
        public void TearDown()
        {
            fixture.Dispose();
            fixture = null;
        }

        [Test]
        public void NewConfigurationUsesSchemaOneAndEditorOnly()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(null);

            Assert.That(
                DirectSceneConfiguration.CurrentSchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                configuration.EntryPolicy,
                Is.EqualTo(
                    DirectSceneEntryPolicy.EditorOnly));

            Assert.That(configuration.HasSupportedSchema, Is.True);
            Assert.That(configuration.HasSupportedPolicy, Is.True);
        }

        [Test]
        public void NewConfigurationIdentityUsesCanonicalFormat()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(null);

            Assert.That(
                configuration.DirectSceneConfigurationId,
                Does.Match("^[0-9a-f]{32}$"));

            Assert.That(configuration.HasValidIdentity, Is.True);
        }

        [Test]
        public void UnknownPolicyIsRejectedWithoutRewrite()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(null);

            fixture.SetDirectPolicy(
                configuration,
                (DirectSceneEntryPolicy)99);

            Assert.That(configuration.HasSupportedPolicy, Is.False);
            Assert.That(
                (int)configuration.EntryPolicy,
                Is.EqualTo(99));
        }

        [Test]
        public void InvalidIdentityIsRejectedWithoutRewrite()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(null);

            fixture.SetDirectId(
                configuration,
                "NOT-A-DIRECT-CONFIGURATION-ID");

            Assert.That(configuration.HasValidIdentity, Is.False);
            Assert.That(
                configuration.DirectSceneConfigurationId,
                Is.EqualTo(
                    "NOT-A-DIRECT-CONFIGURATION-ID"));
        }

        [Test]
        public void SettledResultNormalizesTextAndPath()
        {
            DirectSceneInitializationResult result =
                new DirectSceneInitializationResult(
                    DirectSceneInitializationStatus
                        .BlockedByEnvironment,
                    DirectSceneEntryPolicy.EditorOnly,
                    "  ELAUNCH-DIRECT-001  ",
                    "  Direct entry blocked.\nTry Boot.  ",
                    " Assets\\Scenes\\Direct.unity ",
                    null,
                    false,
                    false);

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo("ELAUNCH-DIRECT-001"));

            Assert.That(
                result.Message,
                Is.EqualTo(
                    "Direct entry blocked. Try Boot."));

            Assert.That(
                result.ContainingScenePath,
                Is.EqualTo("Assets/Scenes/Direct.unity"));

            Assert.That(result.IsSuccessful, Is.False);
        }

        [Test]
        public void NotStartedCannotBeUsedAsSettledResult()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new DirectSceneInitializationResult(
                    DirectSceneInitializationStatus.NotStarted,
                    DirectSceneEntryPolicy.EditorOnly,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    null,
                    false,
                    false));
        }
    }
}

//----- DirectSceneContractTests.cs END -----
