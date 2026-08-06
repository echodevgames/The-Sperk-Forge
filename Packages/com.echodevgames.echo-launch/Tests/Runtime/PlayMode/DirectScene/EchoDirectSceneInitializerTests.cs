//----- EchoDirectSceneInitializerTests.cs START -----

using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class EchoDirectSceneInitializerTests
    {
        private DirectSceneTestFixture fixture;
        private RecordingDirectSceneRootFactory factory;

        [SetUp]
        public void SetUp()
        {
            LaunchAuthorityClaim.Reset();
            fixture = new DirectSceneTestFixture();
            factory =
                new RecordingDirectSceneRootFactory(fixture);
        }

        [TearDown]
        public void TearDown()
        {
            fixture.Dispose();
            fixture = null;
            factory = null;
        }

        [Test]
        public void ExistingAuthorityIsReusedBeforeConfigurationValidation()
        {
            EchoLaunchRoot authority =
                fixture.CreateRoot(
                    fixture.CreateLaunchConfiguration(),
                    LaunchMode.CanonicalBoot,
                    keepAuthority: true,
                    name: "Existing Authority");

            EchoDirectSceneInitializer initializer =
                fixture.CreateInitializer(
                    null,
                    new DirectSceneTestEnvironment(true, true),
                    factory);

            DirectSceneInitializationResult result =
                initializer.EnsureDevelopmentLaunch();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .ReusedExistingAuthority));

            Assert.That(result.AuthoritativeRoot, Is.SameAs(authority));
            Assert.That(result.CreatedRoot, Is.False);
            Assert.That(result.ReusedExistingAuthority, Is.True);
            Assert.That(factory.InstantiateCallCount, Is.Zero);
        }

        [Test]
        public void ValidEditorPolicyCreatesExactlyOneAuthority()
        {
            EchoLaunchRoot template =
                fixture.CreateDirectRootTemplate();

            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(template);

            string configurationId =
                configuration.DirectSceneConfigurationId;

            EchoDirectSceneInitializer initializer =
                fixture.CreateInitializer(
                    configuration,
                    new DirectSceneTestEnvironment(true, true),
                    factory);

            DirectSceneInitializationResult result =
                initializer.EnsureDevelopmentLaunch();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .CreatedDevelopmentAuthority));

            Assert.That(factory.InstantiateCallCount, Is.EqualTo(1));
            Assert.That(result.AuthoritativeRoot, Is.Not.Null);
            Assert.That(
                result.AuthoritativeRoot,
                Is.SameAs(EchoLaunchRoot.Current));

            Assert.That(
                result.AuthoritativeRoot.AuthoredLaunchMode,
                Is.EqualTo(
                    LaunchMode.DirectSceneDevelopment));

            Assert.That(result.CreatedRoot, Is.True);
            Assert.That(result.ReusedExistingAuthority, Is.False);
            Assert.That(initializer.enabled, Is.False);

            Assert.That(
                configuration.DirectSceneConfigurationId,
                Is.EqualTo(configurationId));

            Assert.That(configuration.RootPrefab, Is.SameAs(template));
            Assert.That(
                configuration.EntryPolicy,
                Is.EqualTo(DirectSceneEntryPolicy.EditorOnly));
        }

        [Test]
        public void MultipleInitializersConvergeOnOneAuthority()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    fixture.CreateDirectRootTemplate());

            EchoDirectSceneInitializer first =
                fixture.CreateInitializer(
                    configuration,
                    new DirectSceneTestEnvironment(true, true),
                    factory);

            EchoDirectSceneInitializer second =
                fixture.CreateInitializer(
                    configuration,
                    new DirectSceneTestEnvironment(true, true),
                    factory);

            DirectSceneInitializationResult firstResult =
                first.EnsureDevelopmentLaunch();

            DirectSceneInitializationResult secondResult =
                second.EnsureDevelopmentLaunch();

            Assert.That(
                firstResult.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .CreatedDevelopmentAuthority));

            Assert.That(
                secondResult.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .ReusedExistingAuthority));

            Assert.That(
                secondResult.AuthoritativeRoot,
                Is.SameAs(firstResult.AuthoritativeRoot));

            Assert.That(factory.InstantiateCallCount, Is.EqualTo(1));
        }

        [Test]
        public void BootRequiredBlocksBeforePrefabValidation()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    null,
                    DirectSceneEntryPolicy.BootRequired);

            DirectSceneInitializationResult result =
                fixture.CreateInitializer(
                        configuration,
                        new DirectSceneTestEnvironment(true, true),
                        factory)
                    .EnsureDevelopmentLaunch();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .BlockedByPolicy));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    EchoDirectSceneInitializer.PolicyDiagnosticCode));

            Assert.That(factory.InstantiateCallCount, Is.Zero);
        }

        [Test]
        public void EditorOnlyBlocksDevelopmentPlayer()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    null,
                    DirectSceneEntryPolicy.EditorOnly);

            DirectSceneInitializationResult result =
                fixture.CreateInitializer(
                        configuration,
                        new DirectSceneTestEnvironment(false, true),
                        factory)
                    .EnsureDevelopmentLaunch();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .BlockedByEnvironment));

            Assert.That(factory.InstantiateCallCount, Is.Zero);
        }

        [Test]
        public void DevelopmentBuildOptInAllowsDevelopmentPlayer()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    fixture.CreateDirectRootTemplate(),
                    DirectSceneEntryPolicy
                        .EditorAndDevelopmentBuilds);

            DirectSceneInitializationResult result =
                fixture.CreateInitializer(
                        configuration,
                        new DirectSceneTestEnvironment(false, true),
                        factory)
                    .EnsureDevelopmentLaunch();

            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.CreatedRoot, Is.True);
            Assert.That(factory.InstantiateCallCount, Is.EqualTo(1));
        }

        [TestCase(DirectSceneEntryPolicy.EditorOnly)]
        [TestCase(DirectSceneEntryPolicy.EditorAndDevelopmentBuilds)]
        [TestCase(DirectSceneEntryPolicy.BootRequired)]
        public void NonDevelopmentReleaseNeverCreatesRoot(
            DirectSceneEntryPolicy policy)
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(null, policy);

            DirectSceneInitializationResult result =
                fixture.CreateInitializer(
                        configuration,
                        new DirectSceneTestEnvironment(false, false),
                        factory)
                    .EnsureDevelopmentLaunch();

            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    EchoDirectSceneInitializer.PolicyDiagnosticCode));

            Assert.That(factory.InstantiateCallCount, Is.Zero);
            Assert.That(EchoLaunchRoot.Current, Is.Null);
        }

        [Test]
        public void WrongLaunchModeBlocksBeforeInstantiation()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    fixture.CreateDirectRootTemplate(
                        launchMode: LaunchMode.CanonicalBoot));

            DirectSceneInitializationResult result =
                fixture.CreateInitializer(
                        configuration,
                        new DirectSceneTestEnvironment(true, true),
                        factory)
                    .EnsureDevelopmentLaunch();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .InvalidConfiguration));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    EchoDirectSceneInitializer
                        .ConfigurationDiagnosticCode));

            Assert.That(factory.InstantiateCallCount, Is.Zero);
        }

        [Test]
        public void DestinationMismatchBlocksBeforeInstantiation()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    fixture.CreateDirectRootTemplate(
                        "Assets/Scenes/OtherScene.unity"));

            DirectSceneInitializationResult result =
                fixture.CreateInitializer(
                        configuration,
                        new DirectSceneTestEnvironment(true, true),
                        factory,
                        fixture.ScenePath)
                    .EnsureDevelopmentLaunch();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .InvalidConfiguration));

            Assert.That(factory.InstantiateCallCount, Is.Zero);
        }

        [Test]
        public void InstantiationExceptionSettlesAsDirect003()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    fixture.CreateDirectRootTemplate());

            factory.ThrowOnInstantiate = true;

            DirectSceneInitializationResult result =
                fixture.CreateInitializer(
                        configuration,
                        new DirectSceneTestEnvironment(true, true),
                        factory)
                    .EnsureDevelopmentLaunch();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    DirectSceneInitializationStatus
                        .InstantiationFailed));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    EchoDirectSceneInitializer
                        .InstantiationDiagnosticCode));

            Assert.That(factory.InstantiateCallCount, Is.EqualTo(1));
        }

        [Test]
        public void RepeatedEnsureReturnsSameSettlement()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    fixture.CreateDirectRootTemplate());

            EchoDirectSceneInitializer initializer =
                fixture.CreateInitializer(
                    configuration,
                    new DirectSceneTestEnvironment(true, true),
                    factory);

            DirectSceneInitializationResult first =
                initializer.EnsureDevelopmentLaunch();

            DirectSceneInitializationResult second =
                initializer.EnsureDevelopmentLaunch();

            Assert.That(second, Is.SameAs(first));
            Assert.That(factory.InstantiateCallCount, Is.EqualTo(1));
        }

        [Test]
        public void CreatedRootProducesDirectSceneDevelopmentReport()
        {
            DirectSceneConfiguration configuration =
                fixture.CreateDirectConfiguration(
                    fixture.CreateDirectRootTemplate());

            DirectSceneInitializationResult initialization =
                fixture.CreateInitializer(
                        configuration,
                        new DirectSceneTestEnvironment(true, true),
                        factory)
                    .EnsureDevelopmentLaunch();

            EchoLaunchRoot root =
                initialization.AuthoritativeRoot;

            root.SetInitialDestinationLoaderForTesting(
                ImmediateSuccessInitialDestinationLoader.Shared);

            Awaitable<StartupSequenceRunResult>.Awaiter awaiter =
                root.StartLaunchAsync().GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "The immediate direct-scene fixture must settle synchronously.");

            awaiter.GetResult();

            Assert.That(root.LastReport, Is.Not.Null);
            Assert.That(
                root.LastReport.LaunchMode,
                Is.EqualTo(
                    LaunchMode.DirectSceneDevelopment));

            Assert.That(
                LaunchReport.CurrentSchemaVersion,
                Is.EqualTo(2));
        }
    }
}

//----- EchoDirectSceneInitializerTests.cs END -----
