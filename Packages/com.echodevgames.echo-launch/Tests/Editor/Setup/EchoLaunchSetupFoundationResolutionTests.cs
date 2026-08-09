using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupFoundationResolutionTests
    {
        [Test]
        public void LegacyRequestDefaultsToReuseCompatibleAssets()
        {
            EchoLaunchSetupRequest request =
                CreateRequest();

            Assert.That(
                request.FoundationResolutionPolicy,
                Is.EqualTo(
                    EchoLaunchSetupFoundationResolutionPolicy
                        .ReuseCompatibleAssets));
        }

        [Test]
        public void DefaultPolicyStillReusesSingleCompatibleCandidate()
        {
            Dictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> candidates =
                    CreateCandidateSet();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    CreateRequest(),
                    candidates: candidates);

            EchoLaunchSetupOperation operation =
                EchoLaunchSetupTestFactory.FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveConfiguration);

            Assert.That(
                operation.Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Reuse));

            Assert.That(
                operation.TargetPath,
                Is.EqualTo(
                    "Assets/Existing/EchoLaunchConfiguration.asset"));
        }

        [Test]
        public void CreateProjectOwnedSetupCreatesMissingFoundationDespiteCandidates()
        {
            EchoLaunchSetupRequest request =
                CreateRequest(
                    true,
                    EchoLaunchSetupFoundationResolutionPolicy
                        .CreateProjectOwnedSetup);

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    request,
                    candidates:
                        CreateCandidateSet());

            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            AssertCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveConfiguration,
                paths.ConfigurationAssetPath);

            AssertCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveStartupSequence,
                paths.StartupSequenceAssetPath);

            AssertCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveLaunchDestination,
                paths.LaunchDestinationAssetPath);

            AssertCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveSplashSequence,
                paths.SplashSequenceAssetPath);

            AssertCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveRootPrefabVariant,
                paths.RootPrefabPath);

            Assert.That(
                plan.Status,
                Is.Not.EqualTo(
                    EchoLaunchSetupPlanStatus.Blocked));
        }

        [Test]
        public void CreateProjectOwnedSetupKeepsCompatibleRequestedTarget()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    CreateRequest(
                        false,
                        EchoLaunchSetupFoundationResolutionPolicy
                            .CreateProjectOwnedSetup),
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.ConfigurationAssetPath,
                            EchoLaunchSetupAssetTypeNames.Configuration,
                            EchoLaunchConfiguration.CurrentSchemaVersion)
                    },
                    candidates:
                        CreateCandidateSet());

            EchoLaunchSetupOperation operation =
                EchoLaunchSetupTestFactory.FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveConfiguration);

            Assert.That(
                operation.Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Reuse));

            Assert.That(
                operation.TargetPath,
                Is.EqualTo(
                    paths.ConfigurationAssetPath));
        }

        [Test]
        public void CreateProjectOwnedSetupStillBlocksIncompatibleRequestedTarget()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    CreateRequest(
                        false,
                        EchoLaunchSetupFoundationResolutionPolicy
                            .CreateProjectOwnedSetup),
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Scene(
                            paths.ConfigurationAssetPath)
                    });

            EchoLaunchSetupOperation operation =
                EchoLaunchSetupTestFactory.FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveConfiguration);

            Assert.That(
                operation.Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Conflict));

            Assert.That(
                plan.Status,
                Is.EqualTo(
                    EchoLaunchSetupPlanStatus.Blocked));
        }

        [Test]
        public void CreateProjectOwnedSetupIgnoresExplicitOffRootFoundationSelection()
        {
            const string selectedPath =
                "Assets/Existing/EchoLaunchConfiguration.asset";

            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupRequest request =
                new EchoLaunchSetupRequest(
                    paths.ProjectRootPath,
                    paths.BootScenePath,
                    EchoLaunchSetupTestFactory.DestinationScenePath,
                    false,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                    selectedConfigurationPath:
                        selectedPath,
                    foundationResolutionPolicy:
                        EchoLaunchSetupFoundationResolutionPolicy
                            .CreateProjectOwnedSetup);

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    request,
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            selectedPath,
                            EchoLaunchSetupAssetTypeNames.Configuration,
                            EchoLaunchConfiguration.CurrentSchemaVersion)
                    });

            EchoLaunchSetupOperation operation =
                EchoLaunchSetupTestFactory.FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveConfiguration);

            Assert.That(
                operation.Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Create));

            Assert.That(
                operation.TargetPath,
                Is.EqualTo(
                    paths.ConfigurationAssetPath));
        }

        [Test]
        public void CreateProjectOwnedSetupStillReusesExplicitDestinationScene()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    CreateRequest(
                        false,
                        EchoLaunchSetupFoundationResolutionPolicy
                            .CreateProjectOwnedSetup));

            EchoLaunchSetupOperation operation =
                EchoLaunchSetupTestFactory.FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ValidateDestinationScene);

            Assert.That(
                operation.Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Reuse));

            Assert.That(
                operation.TargetPath,
                Is.EqualTo(
                    EchoLaunchSetupTestFactory.DestinationScenePath));
        }

        [Test]
        public void ResolutionPolicyParticipatesInRequestEquality()
        {
            EchoLaunchSetupRequest reuseRequest =
                CreateRequest();

            EchoLaunchSetupRequest createRequest =
                CreateRequest(
                    false,
                    EchoLaunchSetupFoundationResolutionPolicy
                        .CreateProjectOwnedSetup);

            Assert.That(
                reuseRequest.Equals(createRequest),
                Is.False);
        }

        [Test]
        public void ResolutionPolicyChangesRequestAndPlanFingerprints()
        {
            EchoLaunchSetupPlan reusePlan =
                CreatePlan(
                    CreateRequest());

            EchoLaunchSetupPlan createPlan =
                CreatePlan(
                    CreateRequest(
                        false,
                        EchoLaunchSetupFoundationResolutionPolicy
                            .CreateProjectOwnedSetup));

            Assert.That(
                reusePlan.RequestFingerprint,
                Is.Not.EqualTo(
                    createPlan.RequestFingerprint));

            Assert.That(
                reusePlan.PlanFingerprint,
                Is.Not.EqualTo(
                    createPlan.PlanFingerprint));
        }

        [Test]
        public void PlanTextReportsCreateProjectOwnedSetup()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    CreateRequest(
                        false,
                        EchoLaunchSetupFoundationResolutionPolicy
                            .CreateProjectOwnedSetup));

            string report =
                new EchoLaunchSetupPlanTextFormatter()
                    .Format(
                        plan);

            StringAssert.Contains(
                "Foundation asset resolution: Create Project-Owned Setup",
                report);
        }

        private static EchoLaunchSetupRequest CreateRequest(
            bool createSplashSequence = false,
            EchoLaunchSetupFoundationResolutionPolicy
                foundationResolutionPolicy =
                    EchoLaunchSetupFoundationResolutionPolicy
                        .ReuseCompatibleAssets)
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            return new EchoLaunchSetupRequest(
                paths.ProjectRootPath,
                paths.BootScenePath,
                EchoLaunchSetupTestFactory.DestinationScenePath,
                createSplashSequence,
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                foundationResolutionPolicy:
                    foundationResolutionPolicy);
        }

        private static EchoLaunchSetupPlan CreatePlan(
            EchoLaunchSetupRequest request,
            IEnumerable<EchoLaunchProjectAssetFact> facts = null,
            IDictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> candidates = null)
        {
            EchoLaunchProjectSnapshot snapshot =
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    facts,
                    null,
                    true,
                    candidates);

            return new EchoLaunchSetupPlanner()
                .CreatePlan(
                    request,
                    snapshot);
        }

        private static Dictionary<
            EchoLaunchSetupAssetRole,
            IEnumerable<EchoLaunchProjectAssetFact>>
            CreateCandidateSet()
        {
            return new Dictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>>
            {
                {
                    EchoLaunchSetupAssetRole.Configuration,
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            "Assets/Existing/EchoLaunchConfiguration.asset",
                            EchoLaunchSetupAssetTypeNames.Configuration,
                            EchoLaunchConfiguration.CurrentSchemaVersion)
                    }
                },
                {
                    EchoLaunchSetupAssetRole.StartupSequence,
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            "Assets/Existing/StartupSequence.asset",
                            EchoLaunchSetupAssetTypeNames.StartupSequence)
                    }
                },
                {
                    EchoLaunchSetupAssetRole.LaunchDestination,
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            "Assets/Existing/LaunchDestination.asset",
                            EchoLaunchSetupAssetTypeNames.LaunchDestination)
                    }
                },
                {
                    EchoLaunchSetupAssetRole.SplashSequence,
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            "Assets/Existing/SplashSequence.asset",
                            EchoLaunchSetupAssetTypeNames.SplashSequence)
                    }
                },
                {
                    EchoLaunchSetupAssetRole.RootPrefab,
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            "Assets/Existing/EchoLaunchRoot.prefab",
                            EchoLaunchSetupAssetTypeNames.GameObject)
                    }
                }
            };
        }

        private static void AssertCreate(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind,
            string expectedPath)
        {
            EchoLaunchSetupOperation operation =
                EchoLaunchSetupTestFactory.FindOperation(
                    plan,
                    kind);

            Assert.That(
                operation,
                Is.Not.Null);

            Assert.That(
                operation.Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Create));

            Assert.That(
                operation.TargetPath,
                Is.EqualTo(
                    expectedPath));
        }
    }
}
