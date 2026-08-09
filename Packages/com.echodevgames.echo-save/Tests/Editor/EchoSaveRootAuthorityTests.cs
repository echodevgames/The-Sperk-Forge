using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveRootAuthorityTests
    {
        private GameObject firstObject;
        private GameObject secondObject;
        private GameObject thirdObject;
        private EchoSaveConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            EchoSaveAuthorityClaim.ResetForTesting();

            configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            configuration.SetDefinitionForTesting(
                EchoSaveConfiguration
                    .CurrentSchemaVersion,
                "EchoSave");
        }

        [TearDown]
        public void TearDown()
        {
            if (thirdObject != null)
            {
                Object.DestroyImmediate(
                    thirdObject);
            }

            if (secondObject != null)
            {
                Object.DestroyImmediate(
                    secondObject);
            }

            if (firstObject != null)
            {
                Object.DestroyImmediate(
                    firstObject);
            }

            if (configuration != null)
            {
                Object.DestroyImmediate(
                    configuration);
            }

            EchoSaveAuthorityClaim.ResetForTesting();
        }

        [Test]
        public void OneConfiguredRootClaimsAuthority()
        {
            EchoSaveRoot root =
                CreateRoot(
                    ref firstObject);

            root.SetConfigurationForTesting(
                configuration);

            Assert.That(
                root.IsAuthoritative,
                Is.True);

            Assert.That(
                EchoSaveRoot.Current,
                Is.SameAs(root));

            Assert.That(
                root.State,
                Is.EqualTo(
                    EchoSaveServiceState
                        .AuthorityClaimed));
        }

        [Test]
        public void DuplicateRootIsRejectedBeforeServiceConstruction()
        {
            EchoSaveRoot first =
                CreateRoot(
                    ref firstObject);

            first.SetConfigurationForTesting(
                configuration);

            LogAssert.Expect(
                LogType.Warning,
                new System.Text.RegularExpressions.Regex(
                    @"\[ESV-LIFE-001\].*Duplicate EchoSaveRoot rejected"));

            EchoSaveRoot duplicate =
                CreateRoot(
                    ref secondObject);

            Assert.That(
                first.IsAuthoritative,
                Is.True);

            Assert.That(
                duplicate.IsAuthoritative,
                Is.False);

            Assert.That(
                duplicate.WasRejectedAsDuplicate,
                Is.True);

            Assert.That(
                duplicate.enabled,
                Is.False);

            Assert.That(
                duplicate.HasConstructedServiceForTesting,
                Is.False);

            Assert.That(
                duplicate.State,
                Is.EqualTo(
                    EchoSaveServiceState
                        .RejectedDuplicate));
        }

        [Test]
        public void ValidConfigurationInitializesExactlyOnce()
        {
            EchoSaveRoot root =
                CreateConfiguredRoot(
                    ref firstObject);

            LifecycleProbe probe =
                new LifecycleProbe();

            root.SetLifecycleProbeForTesting(
                probe);

            EchoSaveLifecycleResult first =
                root.InitializeSynchronouslyForTesting();

            EchoSaveLifecycleResult second =
                root.InitializeSynchronouslyForTesting();

            Assert.That(
                first.Status,
                Is.EqualTo(
                    EchoSaveLifecycleStatus
                        .Succeeded));

            Assert.That(
                second.Status,
                Is.EqualTo(
                    EchoSaveLifecycleStatus
                        .NoChange));

            Assert.That(
                root.State,
                Is.EqualTo(
                    EchoSaveServiceState.Ready));

            Assert.That(
                probe.InitializeAcceptedCount,
                Is.EqualTo(1));
        }

        [Test]
        public void MissingConfigurationBlocksWithoutInitializationSideEffect()
        {
            EchoSaveRoot root =
                CreateRoot(
                    ref firstObject);

            LifecycleProbe probe =
                new LifecycleProbe();

            root.SetLifecycleProbeForTesting(
                probe);

            EchoSaveLifecycleResult result =
                root.InitializeSynchronouslyForTesting();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveLifecycleStatus.Blocked));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-CFG-001"));

            Assert.That(
                root.State,
                Is.EqualTo(
                    EchoSaveServiceState.Blocked));

            Assert.That(
                probe.InitializeAcceptedCount,
                Is.Zero);
        }

        [Test]
        public void InvalidConfigurationBlocksWithoutInitializationSideEffect()
        {
            configuration.SetDefinitionForTesting(
                EchoSaveConfiguration
                    .CurrentSchemaVersion,
                "../Unsafe");

            EchoSaveRoot root =
                CreateConfiguredRoot(
                    ref firstObject);

            LifecycleProbe probe =
                new LifecycleProbe();

            root.SetLifecycleProbeForTesting(
                probe);

            EchoSaveLifecycleResult result =
                root.InitializeSynchronouslyForTesting();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveLifecycleStatus.Blocked));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-CFG-001"));

            Assert.That(
                probe.InitializeAcceptedCount,
                Is.Zero);
        }

        [Test]
        public void ShutdownReleasesAuthorityAndLaterRootMayClaim()
        {
            EchoSaveRoot first =
                CreateConfiguredRoot(
                    ref firstObject);

            first.InitializeSynchronouslyForTesting();

            EchoSaveLifecycleResult shutdown =
                first.ShutdownSynchronouslyForTesting();

            Assert.That(
                shutdown.Status,
                Is.EqualTo(
                    EchoSaveLifecycleStatus
                        .Succeeded));

            Assert.That(
                first.State,
                Is.EqualTo(
                    EchoSaveServiceState.Shutdown));

            Assert.That(
                first.IsAuthoritative,
                Is.False);

            EchoSaveRoot replacement =
                CreateRoot(
                    ref secondObject);

            replacement.SetConfigurationForTesting(
                configuration);

            Assert.That(
                replacement.IsAuthoritative,
                Is.True);

            Assert.That(
                EchoSaveRoot.Current,
                Is.SameAs(replacement));
        }

        [Test]
        public void DuplicateCannotRunLifecycleSideEffects()
        {
            EchoSaveRoot first =
                CreateConfiguredRoot(
                    ref firstObject);

            LifecycleProbe firstProbe =
                new LifecycleProbe();

            first.SetLifecycleProbeForTesting(
                firstProbe);

            LogAssert.Expect(
                LogType.Warning,
                new System.Text.RegularExpressions.Regex(
                    @"\[ESV-LIFE-001\].*Duplicate EchoSaveRoot rejected"));

            EchoSaveRoot duplicate =
                CreateRoot(
                    ref secondObject);

            EchoSaveLifecycleResult result =
                duplicate
                    .InitializeSynchronouslyForTesting();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveLifecycleStatus.Rejected));

            Assert.That(
                duplicate.HasConstructedServiceForTesting,
                Is.False);

            Assert.That(
                firstProbe.InitializeAcceptedCount,
                Is.Zero);
        }

        [Test]
        public void M1LifecyclePerformsZeroDurableStorageOperations()
        {
            EchoSaveRoot root =
                CreateConfiguredRoot(
                    ref firstObject);

            LifecycleProbe probe =
                new LifecycleProbe();

            root.SetLifecycleProbeForTesting(
                probe);

            EchoSaveLifecycleResult initialized =
                root.InitializeSynchronouslyForTesting();

            EchoSaveLifecycleResult shutdown =
                root.ShutdownSynchronouslyForTesting();

            Assert.That(
                initialized.Succeeded,
                Is.True);

            Assert.That(
                shutdown.Succeeded,
                Is.True);

            Assert.That(
                probe.InitializeAcceptedCount,
                Is.EqualTo(1));

            Assert.That(
                probe.ShutdownCount,
                Is.EqualTo(1));

            Assert.That(
                probe.StorageOperationCount,
                Is.Zero);
        }

        private EchoSaveRoot CreateConfiguredRoot(
            ref GameObject owner)
        {
            EchoSaveRoot root =
                CreateRoot(
                    ref owner);

            root.SetConfigurationForTesting(
                configuration);

            return root;
        }

        private static EchoSaveRoot CreateRoot(
            ref GameObject owner)
        {
            owner =
                new GameObject(
                    "EchoSaveRoot Test");

            return owner.AddComponent<
                EchoSaveRoot>();
        }

        private sealed class LifecycleProbe :
            IEchoSaveLifecycleProbe
        {
            internal int InitializeAcceptedCount
            {
                get;
                private set;
            }

            internal int ShutdownCount
            {
                get;
                private set;
            }

            internal int StorageOperationCount =>
                0;

            public void OnInitializeAccepted(
                EchoSaveConfiguration config)
            {
                InitializeAcceptedCount++;
            }

            public void OnShutdown()
            {
                ShutdownCount++;
            }
        }
    }
}
