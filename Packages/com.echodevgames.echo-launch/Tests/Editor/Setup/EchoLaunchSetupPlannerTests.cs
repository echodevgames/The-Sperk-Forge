
using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupPlannerTests
    {
        [Test]
        public void EmptyProjectProducesCreateProposals()
        {
            EchoLaunchSetupPlan plan = CreatePlan();

            Assert.That(
                plan.CountDisposition(
                    EchoLaunchSetupOperationDisposition.Create),
                Is.GreaterThanOrEqualTo(8));
        }

        [Test]
        public void OptionalSplashIsOmittedByDefault()
        {
            Assert.That(
                EchoLaunchSetupTestFactory.FindOperation(
                    CreatePlan(),
                    EchoLaunchSetupOperationKind.ResolveSplashSequence),
                Is.Null);
        }

        [Test]
        public void OptionalSplashIsIncludedWhenRequested()
        {
            Assert.That(
                EchoLaunchSetupTestFactory.FindOperation(
                    CreatePlan(
                        request:
                        EchoLaunchSetupTestFactory.CreateRequest(true)),
                    EchoLaunchSetupOperationKind.ResolveSplashSequence),
                Is.Not.Null);
        }

        [Test]
        public void ExistingConfigurationIsReused()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            AssertDisposition(
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.ConfigurationAssetPath,
                            EchoLaunchSetupAssetTypeNames.Configuration,
                            EchoLaunchConfiguration.CurrentSchemaVersion)
                    }),
                EchoLaunchSetupOperationKind.ResolveConfiguration,
                EchoLaunchSetupOperationDisposition.Reuse);
        }

        [Test]
        public void ExistingStartupSequenceIsReused()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            AssertDisposition(
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.StartupSequenceAssetPath,
                            EchoLaunchSetupAssetTypeNames.StartupSequence)
                    }),
                EchoLaunchSetupOperationKind.ResolveStartupSequence,
                EchoLaunchSetupOperationDisposition.Reuse);
        }

        [Test]
        public void ExistingLaunchDestinationIsReused()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            AssertDisposition(
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.LaunchDestinationAssetPath,
                            EchoLaunchSetupAssetTypeNames.LaunchDestination)
                    }),
                EchoLaunchSetupOperationKind.ResolveLaunchDestination,
                EchoLaunchSetupOperationDisposition.Reuse);
        }

        [Test]
        public void ExistingRootVariantIsReused()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            AssertDisposition(
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.RootPrefabPath,
                            EchoLaunchSetupAssetTypeNames.GameObject)
                    }),
                EchoLaunchSetupOperationKind.ResolveRootPrefabVariant,
                EchoLaunchSetupOperationDisposition.Reuse);
        }

        [Test]
        public void ExistingBootSceneIsReused()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            AssertDisposition(
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Scene(paths.BootScenePath)
                    }),
                EchoLaunchSetupOperationKind.ResolveBootScene,
                EchoLaunchSetupOperationDisposition.Reuse);
        }

        [Test]
        public void WrongConfigurationTypeBlocks()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Scene(
                            paths.ConfigurationAssetPath)
                    });

            Assert.That(plan.Status, Is.EqualTo(EchoLaunchSetupPlanStatus.Blocked));
            Assert.That(
                EchoLaunchSetupTestFactory.HasDiagnostic(
                    plan,
                    EchoLaunchSetupDiagnosticCodes.IncompatibleTarget),
                Is.True);
        }

        [Test]
        public void WrongBootTypeBlocks()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            AssertDisposition(
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.BootScenePath,
                            EchoLaunchSetupAssetTypeNames.Configuration)
                    }),
                EchoLaunchSetupOperationKind.ResolveBootScene,
                EchoLaunchSetupOperationDisposition.Conflict);
        }

        [Test]
        public void UnsupportedSchemaBlocks()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.ConfigurationAssetPath,
                            EchoLaunchSetupAssetTypeNames.Configuration,
                            3)
                    });

            Assert.That(
                EchoLaunchSetupTestFactory.HasDiagnostic(
                    plan,
                    EchoLaunchSetupDiagnosticCodes.UnsupportedMigration),
                Is.True);

            Assert.That(plan.Status, Is.EqualTo(EchoLaunchSetupPlanStatus.Blocked));
        }

        [Test]
        public void MissingPackageTemplateBlocks()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(templateAvailable: false);

            Assert.That(
                EchoLaunchSetupTestFactory.HasDiagnostic(
                    plan,
                    EchoLaunchSetupDiagnosticCodes.PackagePrerequisiteMissing),
                Is.True);
        }

        [Test]
        public void MultipleCandidatesRequireDecision()
        {
            Dictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> candidates =
                    new Dictionary<
                        EchoLaunchSetupAssetRole,
                        IEnumerable<EchoLaunchProjectAssetFact>>();

            candidates[EchoLaunchSetupAssetRole.Configuration] =
                new[]
                {
                    EchoLaunchSetupTestFactory.Asset(
                        "Assets/A.asset",
                        EchoLaunchSetupAssetTypeNames.Configuration,
                        EchoLaunchConfiguration.CurrentSchemaVersion),
                    EchoLaunchSetupTestFactory.Asset(
                        "Assets/B.asset",
                        EchoLaunchSetupAssetTypeNames.Configuration,
                        EchoLaunchConfiguration.CurrentSchemaVersion)
                };

            EchoLaunchSetupPlan plan =
                CreatePlan(candidates: candidates);

            AssertDisposition(
                plan,
                EchoLaunchSetupOperationKind.ResolveConfiguration,
                EchoLaunchSetupOperationDisposition.ManualDecision);

            Assert.That(plan.Status, Is.EqualTo(EchoLaunchSetupPlanStatus.Blocked));
        }

        [Test]
        public void ExplicitSelectedConfigurationIsReused()
        {
            const string selectedPath =
                "Assets/Existing/Launch.asset";

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    EchoLaunchSetupTestFactory.CreateRequest(
                        selectedConfiguration: selectedPath),
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

            Assert.That(operation.TargetPath, Is.EqualTo(selectedPath));
            Assert.That(
                operation.Disposition,
                Is.EqualTo(EchoLaunchSetupOperationDisposition.Reuse));
        }

        [Test]
        public void SingleCandidateIsReused()
        {
            Dictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> candidates =
                    new Dictionary<
                        EchoLaunchSetupAssetRole,
                        IEnumerable<EchoLaunchProjectAssetFact>>();

            candidates[EchoLaunchSetupAssetRole.StartupSequence] =
                new[]
                {
                    EchoLaunchSetupTestFactory.Asset(
                        "Assets/Existing/Sequence.asset",
                        EchoLaunchSetupAssetTypeNames.StartupSequence)
                };

            AssertDisposition(
                CreatePlan(candidates: candidates),
                EchoLaunchSetupOperationKind.ResolveStartupSequence,
                EchoLaunchSetupOperationDisposition.Reuse);
        }

        [Test]
        public void DefaultBuildPolicyAppendsMissingBoot()
        {
            AssertDisposition(
                CreatePlan(),
                EchoLaunchSetupOperationKind.ResolveBuildSettings,
                EchoLaunchSetupOperationDisposition.Create);
        }

        [Test]
        public void ExistingBuildEntryProducesNoChange()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            AssertDisposition(
                CreatePlan(
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            paths.BootScenePath,
                            true,
                            0)
                    }),
                EchoLaunchSetupOperationKind.ResolveBuildSettings,
                EchoLaunchSetupOperationDisposition.NoChange);
        }

        [Test]
        public void PlaceFirstRequiresApproval()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    request:
                    EchoLaunchSetupTestFactory.CreateRequest(
                        policy:
                        EchoLaunchBuildSettingsPolicy.PlaceFirstAfterApproval),
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/Scenes/Other.unity",
                            true,
                            0)
                    });

            Assert.That(plan.RequiresExplicitApproval, Is.True);
            Assert.That(
                plan.Status,
                Is.EqualTo(EchoLaunchSetupPlanStatus.ReadyWithWarnings));
        }

        [Test]
        public void PlaceFirstDoesNothingWhenAlreadyFirst()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    request:
                    EchoLaunchSetupTestFactory.CreateRequest(
                        policy:
                        EchoLaunchBuildSettingsPolicy.PlaceFirstAfterApproval),
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            paths.BootScenePath,
                            true,
                            0),
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/Scenes/Other.unity",
                            true,
                            1)
                    });

            Assert.That(plan.RequiresExplicitApproval, Is.False);

            AssertDisposition(
                plan,
                EchoLaunchSetupOperationKind.ResolveBuildSettings,
                EchoLaunchSetupOperationDisposition.NoChange);
        }

        [Test]
        public void DoNotChangeBuildSettingsIsNoChange()
        {
            AssertDisposition(
                CreatePlan(
                    request:
                    EchoLaunchSetupTestFactory.CreateRequest(
                        policy:
                        EchoLaunchBuildSettingsPolicy.DoNotChange)),
                EchoLaunchSetupOperationKind.ResolveBuildSettings,
                EchoLaunchSetupOperationDisposition.NoChange);
        }

        [Test]
        public void SameInputsProduceEquivalentPlans()
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest();

            EchoLaunchProjectSnapshot snapshot =
                EchoLaunchSetupTestFactory.CreateSnapshot();

            EchoLaunchSetupPlanner planner = new EchoLaunchSetupPlanner();

            Assert.That(
                planner.CreatePlan(request, snapshot).ValueEquals(
                    planner.CreatePlan(request, snapshot)),
                Is.True);
        }

        [Test]
        public void OperationsUseNondecreasingPhaseOrder()
        {
            EchoLaunchSetupPlan plan = CreatePlan();

            for (int index = 1; index < plan.Operations.Count; index++)
            {
                Assert.That(
                    plan.Operations[index].Phase,
                    Is.GreaterThanOrEqualTo(
                        plan.Operations[index - 1].Phase));
            }
        }

        [Test]
        public void InvalidRootBlocks()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    request:
                    EchoLaunchSetupTestFactory.CreateRequest(
                        projectRoot: "Packages/NotAllowed"));

            Assert.That(plan.Status, Is.EqualTo(EchoLaunchSetupPlanStatus.Blocked));
        }

        [Test]
        public void MissingDestinationSelectionBlocks()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    request:
                    EchoLaunchSetupTestFactory.CreateRequest(
                        destinationPath: string.Empty));

            Assert.That(plan.Status, Is.EqualTo(EchoLaunchSetupPlanStatus.Blocked));
        }

        [Test]
        public void MissingDestinationFactBlocks()
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest(
                    destinationPath:
                    "Assets/Scenes/Missing.unity");

            EchoLaunchSetupPlan plan =
                new EchoLaunchSetupPlanner().CreatePlan(
                    request,
                    EchoLaunchSetupTestFactory.CreateSnapshot());

            Assert.That(plan.Status, Is.EqualTo(EchoLaunchSetupPlanStatus.Blocked));
        }

        [Test]
        public void CleanPlanIsReady()
        {
            Assert.That(
                CreatePlan().Status,
                Is.EqualTo(EchoLaunchSetupPlanStatus.Ready));
        }

        [Test]
        public void PlanDefensivelyCopiesCollections()
        {
            List<EchoLaunchSetupOperation> operations =
                new List<EchoLaunchSetupOperation>
                {
                    new EchoLaunchSetupOperation(
                        "one",
                        0,
                        EchoLaunchSetupOperationKind.ValidateRequest,
                        EchoLaunchSetupOperationDisposition.NoChange,
                        "Assets/Echo",
                        "Ready.")
                };

            List<EchoLaunchSetupDiagnostic> diagnostics =
                new List<EchoLaunchSetupDiagnostic>();

            EchoLaunchSetupPlan plan =
                new EchoLaunchSetupPlan(
                    EchoLaunchSetupTestFactory.CreateRequest(),
                    EchoLaunchSetupPathSet.CreateDefault(),
                    "Evidence",
                    EchoLaunchSetupPlanStatus.Ready,
                    operations,
                    diagnostics);

            operations.Clear();
            diagnostics.Add(
                new EchoLaunchSetupDiagnostic(
                    "TEST",
                    EchoLaunchSetupDiagnosticSeverity.Blocker,
                    "Changed."));

            Assert.That(plan.Operations.Count, Is.EqualTo(1));
            Assert.That(plan.Diagnostics.Count, Is.EqualTo(0));
        }

        [Test]
        public void ReuseProducesInformationalDiagnostic()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.StartupSequenceAssetPath,
                            EchoLaunchSetupAssetTypeNames.StartupSequence)
                    });

            Assert.That(
                EchoLaunchSetupTestFactory.HasDiagnostic(
                    plan,
                    EchoLaunchSetupDiagnosticCodes.CompatibleAssetReused),
                Is.True);
        }

        [Test]
        public void ExistingFolderProducesNoChange()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Folder(paths.ProjectRootPath)
                    });

            Assert.That(
                plan.CountDisposition(
                    EchoLaunchSetupOperationDisposition.NoChange),
                Is.GreaterThan(0));
        }

        [Test]
        public void FileAtFolderPathBlocks()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    facts:
                    new[]
                    {
                        EchoLaunchSetupTestFactory.Asset(
                            paths.ProjectRootPath,
                            EchoLaunchSetupAssetTypeNames.Configuration)
                    });

            Assert.That(plan.Status, Is.EqualTo(EchoLaunchSetupPlanStatus.Blocked));
        }


        [Test]
        public void PlanFingerprintsAreNonempty()
        {
            EchoLaunchSetupPlan plan = CreatePlan();

            Assert.That(plan.RequestFingerprint, Is.Not.Empty);
            Assert.That(plan.EvidenceFingerprint, Is.Not.Empty);
            Assert.That(plan.PlanFingerprint, Is.Not.Empty);
        }

        [Test]
        public void EquivalentPlansHaveEqualFingerprints()
        {
            EchoLaunchSetupPlan first = CreatePlan();
            EchoLaunchSetupPlan second = CreatePlan();

            Assert.That(
                first.PlanFingerprint,
                Is.EqualTo(second.PlanFingerprint));
        }

        [Test]
        public void BuildSettingsOrderChangesPlanFingerprint()
        {
            EchoLaunchSetupPlan first =
                CreatePlan(
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/Scenes/A.unity",
                            true,
                            0),
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/Scenes/B.unity",
                            true,
                            1)
                    });

            EchoLaunchSetupPlan second =
                CreatePlan(
                    buildScenes:
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/Scenes/B.unity",
                            true,
                            0),
                        new EchoLaunchBuildSettingsSceneFact(
                            "Assets/Scenes/A.unity",
                            true,
                            1)
                    });

            Assert.That(
                first.PlanFingerprint,
                Is.Not.EqualTo(second.PlanFingerprint));
        }

        private static EchoLaunchSetupPlan CreatePlan(
            EchoLaunchSetupRequest request = null,
            IEnumerable<EchoLaunchProjectAssetFact> facts = null,
            IEnumerable<EchoLaunchBuildSettingsSceneFact> buildScenes = null,
            bool templateAvailable = true,
            IDictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> candidates = null)
        {
            EchoLaunchSetupRequest resolvedRequest =
                request ?? EchoLaunchSetupTestFactory.CreateRequest();

            EchoLaunchProjectSnapshot snapshot =
                EchoLaunchSetupTestFactory.CreateSnapshot(
                    facts,
                    buildScenes,
                    templateAvailable,
                    candidates);

            return new EchoLaunchSetupPlanner().CreatePlan(
                resolvedRequest,
                snapshot);
        }

        private static void AssertDisposition(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind,
            EchoLaunchSetupOperationDisposition disposition)
        {
            EchoLaunchSetupOperation operation =
                EchoLaunchSetupTestFactory.FindOperation(plan, kind);

            Assert.That(operation, Is.Not.Null);
            Assert.That(operation.Disposition, Is.EqualTo(disposition));
        }
    }
}
