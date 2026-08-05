using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupFingerprintTests
    {
        [Test]
        public void EquivalentRequestsMatch()
        {
            Assert.That(
                EchoLaunchSetupFingerprint.ForRequest(
                    EchoLaunchSetupTestFactory.CreateRequest()),
                Is.EqualTo(
                    EchoLaunchSetupFingerprint.ForRequest(
                        EchoLaunchSetupTestFactory.CreateRequest())));
        }


        [Test]
        public void EquivalentSeparatorStylesMatchRequestFingerprint()
        {
            EchoLaunchSetupRequest first =
                new EchoLaunchSetupRequest(
                    @"Assets\Echo\FirstLight",
                    @"Assets\Echo\FirstLight\Scenes\Boot.unity",
                    @"Assets\Scenes\MainMenu.unity",
                    false,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);

            EchoLaunchSetupRequest second =
                new EchoLaunchSetupRequest(
                    "Assets/Echo/FirstLight",
                    "Assets/Echo/FirstLight/Scenes/Boot.unity",
                    "Assets/Scenes/MainMenu.unity",
                    false,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);

            Assert.That(
                EchoLaunchSetupFingerprint.ForRequest(first),
                Is.EqualTo(
                    EchoLaunchSetupFingerprint.ForRequest(second)));
        }

        [Test]
        public void DestinationChangeAltersRequestFingerprint()
        {
            string first =
                EchoLaunchSetupFingerprint.ForRequest(
                    EchoLaunchSetupTestFactory.CreateRequest());

            string second =
                EchoLaunchSetupFingerprint.ForRequest(
                    EchoLaunchSetupTestFactory.CreateRequest(
                        destinationPath:
                        "Assets/Scenes/Other.unity"));

            Assert.That(first, Is.Not.EqualTo(second));
        }

        [Test]
        public void SplashChoiceAltersRequestFingerprint()
        {
            Assert.That(
                EchoLaunchSetupFingerprint.ForRequest(
                    EchoLaunchSetupTestFactory.CreateRequest(false)),
                Is.Not.EqualTo(
                    EchoLaunchSetupFingerprint.ForRequest(
                        EchoLaunchSetupTestFactory.CreateRequest(true))));
        }

        [Test]
        public void BuildPolicyAltersRequestFingerprint()
        {
            Assert.That(
                EchoLaunchSetupFingerprint.ForRequest(
                    EchoLaunchSetupTestFactory.CreateRequest(
                        policy:
                        EchoLaunchBuildSettingsPolicy.DoNotChange)),
                Is.Not.EqualTo(
                    EchoLaunchSetupFingerprint.ForRequest(
                        EchoLaunchSetupTestFactory.CreateRequest(
                            policy:
                            EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd))));
        }

        [Test]
        public void SnapshotInputOrderDoesNotAlterFingerprint()
        {
            EchoLaunchProjectAssetFact first =
                EchoLaunchSetupTestFactory.Asset(
                    "Assets/A.asset",
                    EchoLaunchSetupAssetTypeNames.Configuration,
                    EchoLaunchConfiguration.CurrentSchemaVersion);

            EchoLaunchProjectAssetFact second =
                EchoLaunchSetupTestFactory.Asset(
                    "Assets/B.asset",
                    EchoLaunchSetupAssetTypeNames.StartupSequence);

            EchoLaunchProjectSnapshot left =
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    new[] { first, second });

            EchoLaunchProjectSnapshot right =
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    new[] { second, first });

            Assert.That(
                left.EvidenceFingerprint,
                Is.EqualTo(right.EvidenceFingerprint));
        }

        [Test]
        public void AssetGuidChangeAltersSnapshotFingerprint()
        {
            EchoLaunchProjectAssetFact first =
                new EchoLaunchProjectAssetFact(
                    "Assets/A.asset",
                    true,
                    false,
                    "guid-a",
                    EchoLaunchSetupAssetTypeNames.Configuration,
                    EchoLaunchConfiguration.CurrentSchemaVersion);

            EchoLaunchProjectAssetFact second =
                new EchoLaunchProjectAssetFact(
                    "Assets/A.asset",
                    true,
                    false,
                    "guid-b",
                    EchoLaunchSetupAssetTypeNames.Configuration,
                    EchoLaunchConfiguration.CurrentSchemaVersion);

            Assert.That(
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    new[] { first }).EvidenceFingerprint,
                Is.Not.EqualTo(
                    EchoLaunchSetupTestFactory.CreateSnapshot(
                        new[] { second }).EvidenceFingerprint));
        }

        [Test]
        public void BuildSettingsOrderAltersSnapshotFingerprint()
        {
            EchoLaunchProjectSnapshot first =
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/A.unity",
                            true,
                            0),
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/B.unity",
                            true,
                            1)
                    });

            EchoLaunchProjectSnapshot second =
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/B.unity",
                            true,
                            0),
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/A.unity",
                            true,
                            1)
                    });

            Assert.That(
                first.EvidenceFingerprint,
                Is.Not.EqualTo(second.EvidenceFingerprint));
        }

        [Test]
        public void BuildSettingsEnabledStateAltersFingerprint()
        {
            EchoLaunchProjectSnapshot first =
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/A.unity",
                            true,
                            0)
                    });

            EchoLaunchProjectSnapshot second =
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/A.unity",
                            false,
                            0)
                    });

            Assert.That(
                first.EvidenceFingerprint,
                Is.Not.EqualTo(second.EvidenceFingerprint));
        }

        [Test]
        public void TemplateGuidAltersFingerprint()
        {
            EchoLaunchProjectSnapshot first =
                new EchoLaunchProjectSnapshot(
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Scene(
                            EchoLaunchSetupTestFactory.DestinationScenePath)
                    },
                    null,
                    true,
                    "template-a");

            EchoLaunchProjectSnapshot second =
                new EchoLaunchProjectSnapshot(
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Scene(
                            EchoLaunchSetupTestFactory.DestinationScenePath)
                    },
                    null,
                    true,
                    "template-b");

            Assert.That(
                first.EvidenceFingerprint,
                Is.Not.EqualTo(second.EvidenceFingerprint));
        }

        [Test]
        public void CandidateSetAltersFingerprint()
        {
            Dictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> firstCandidates =
                    new Dictionary<
                        EchoLaunchSetupAssetRole,
                        IEnumerable<EchoLaunchProjectAssetFact>>();

            firstCandidates[EchoLaunchSetupAssetRole.Configuration] =
                new[]
                {
                    EchoLaunchSetupTestFactory.Asset(
                        "Assets/A.asset",
                        EchoLaunchSetupAssetTypeNames.Configuration,
                        EchoLaunchConfiguration.CurrentSchemaVersion)
                };

            Dictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> secondCandidates =
                    new Dictionary<
                        EchoLaunchSetupAssetRole,
                        IEnumerable<EchoLaunchProjectAssetFact>>();

            secondCandidates[EchoLaunchSetupAssetRole.Configuration] =
                new[]
                {
                    EchoLaunchSetupTestFactory.Asset(
                        "Assets/B.asset",
                        EchoLaunchSetupAssetTypeNames.Configuration,
                        EchoLaunchConfiguration.CurrentSchemaVersion)
                };

            Assert.That(
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    candidates:
                    firstCandidates).EvidenceFingerprint,
                Is.Not.EqualTo(
                    EchoLaunchSetupTestFactory.CreateSnapshot(
                        candidates:
                        secondCandidates).EvidenceFingerprint));
        }

        [Test]
        public void HashUsesLowercaseHex()
        {
            string hash = EchoLaunchSetupFingerprint.Hash("First Light");

            Assert.That(hash.Length, Is.EqualTo(64));
            Assert.That(hash, Is.EqualTo(hash.ToLowerInvariant()));
        }

        [Test]
        public void PlanReasonAltersPlanFingerprint()
        {
            EchoLaunchSetupOperation first =
                new EchoLaunchSetupOperation(
                    "key",
                    0,
                    EchoLaunchSetupOperationKind.ValidateRequest,
                    EchoLaunchSetupOperationDisposition.NoChange,
                    "Assets/Test",
                    "First.");

            EchoLaunchSetupOperation second =
                new EchoLaunchSetupOperation(
                    "key",
                    0,
                    EchoLaunchSetupOperationKind.ValidateRequest,
                    EchoLaunchSetupOperationDisposition.NoChange,
                    "Assets/Test",
                    "Second.");

            Assert.That(
                EchoLaunchSetupFingerprint.ForPlan(
                    "request",
                    "evidence",
                    EchoLaunchSetupPlanStatus.Ready,
                    new[] { first },
                    null),
                Is.Not.EqualTo(
                    EchoLaunchSetupFingerprint.ForPlan(
                        "request",
                        "evidence",
                        EchoLaunchSetupPlanStatus.Ready,
                        new[] { second },
                        null)));
        }
    }
}
