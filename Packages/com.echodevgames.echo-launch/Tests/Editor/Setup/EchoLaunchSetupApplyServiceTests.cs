using System;
using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupApplyServiceTests
    {
        [TearDown]
        public void TearDown()
        {
            EchoLaunchSetupApplyService.SetApplyActiveForTests(false);
        }

        [Test]
        public void NullApplyRequestIsBlocked()
        {
            EchoLaunchSetupApplyResult result =
                CreateHarness(CreateCreatePlan())
                    .Service.Apply(null);

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.Blocked));
        }

        [Test]
        public void MissingPlanIsBlocked()
        {
            EchoLaunchSetupApplyResult result =
                CreateHarness(CreateCreatePlan())
                    .Service.Apply(
                        new EchoLaunchSetupApplyRequest(
                            null,
                            true,
                            false));

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.Blocked));
        }

        [Test]
        public void CancelledRequestWritesNothing()
        {
            Harness harness = CreateHarness(CreateCreatePlan());

            EchoLaunchSetupApplyResult result =
                harness.Service.Apply(
                    new EchoLaunchSetupApplyRequest(
                        harness.DisplayedPlan,
                        false,
                        false));

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.Cancelled));

            Assert.That(harness.Calls.Count, Is.EqualTo(0));
        }

        [Test]
        public void BusyEditorBlocksBeforeWrites()
        {
            Harness harness =
                CreateHarness(
                    CreateCreatePlan(),
                    busy: true);

            EchoLaunchSetupApplyResult result =
                ApplyConfirmed(harness);

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.Blocked));

            Assert.That(harness.Calls.Count, Is.EqualTo(0));
        }

        [Test]
        public void BlockedPlanIsRejected()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    EchoLaunchSetupPlanStatus.Blocked,
                    new[]
                    {
                        Operation(
                            EchoLaunchSetupOperationKind.ValidateRequest,
                            EchoLaunchSetupOperationDisposition.Conflict,
                            "Assets/Blocked",
                            EchoLaunchSetupDiagnosticCodes.InvalidRequest)
                    });

            Assert.That(
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    plan,
                    false).CanApply,
                Is.False);
        }

        [Test]
        public void ReadyPlanIsEligible()
        {
            Assert.That(
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    CreateCreatePlan(),
                    false).CanApply,
                Is.True);
        }

        [Test]
        public void PlaceFirstRequiresApproval()
        {
            EchoLaunchSetupPlan plan = CreatePlaceFirstPlan();

            EchoLaunchSetupApplyEligibility eligibility =
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    plan,
                    false);

            Assert.That(eligibility.CanApply, Is.False);
            Assert.That(
                eligibility.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes.BuildSettingsApproval));
        }

        [Test]
        public void PlaceFirstBecomesEligibleWithApproval()
        {
            Assert.That(
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    CreatePlaceFirstPlan(),
                    true).CanApply,
                Is.True);
        }

        [Test]
        public void ConflictOperationIsRejected()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    EchoLaunchSetupPlanStatus.Ready,
                    new[]
                    {
                        Operation(
                            EchoLaunchSetupOperationKind.ResolveConfiguration,
                            EchoLaunchSetupOperationDisposition.Conflict,
                            "Assets/Config.asset",
                            EchoLaunchSetupDiagnosticCodes.IncompatibleTarget)
                    });

            Assert.That(
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    plan,
                    false).CanApply,
                Is.False);
        }

        [Test]
        public void UnsupportedOperationIsRejected()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    EchoLaunchSetupPlanStatus.Ready,
                    new[]
                    {
                        Operation(
                            EchoLaunchSetupOperationKind.ResolveConfiguration,
                            EchoLaunchSetupOperationDisposition.Unsupported,
                            "Assets/Config.asset",
                            EchoLaunchSetupDiagnosticCodes.UnsupportedMigration)
                    });

            Assert.That(
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    plan,
                    false).CanApply,
                Is.False);
        }

        [Test]
        public void AmbiguousManualDecisionIsRejected()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    EchoLaunchSetupPlanStatus.ReadyWithWarnings,
                    new[]
                    {
                        Operation(
                            EchoLaunchSetupOperationKind.ResolveConfiguration,
                            EchoLaunchSetupOperationDisposition.ManualDecision,
                            "Assets/Config.asset",
                            EchoLaunchSetupDiagnosticCodes.AmbiguousCandidates)
                    });

            Assert.That(
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    plan,
                    true).CanApply,
                Is.False);
        }

        [Test]
        public void UnknownOperationKindIsRejected()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    EchoLaunchSetupPlanStatus.Ready,
                    new[]
                    {
                        Operation(
                            (EchoLaunchSetupOperationKind)999,
                            EchoLaunchSetupOperationDisposition.NoChange,
                            "Assets/Unknown",
                            string.Empty)
                    });

            EchoLaunchSetupApplyEligibility eligibility =
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    plan,
                    false);

            Assert.That(eligibility.CanApply, Is.False);
            Assert.That(
                eligibility.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes
                        .UnauthorizedApplyOperation));
        }


        [Test]
        public void CreateDispositionOnValidationOperationIsRejected()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    EchoLaunchSetupPlanStatus.Ready,
                    new[]
                    {
                        Operation(
                            EchoLaunchSetupOperationKind.ValidateRequest,
                            EchoLaunchSetupOperationDisposition.Create,
                            "Assets/Invalid")
                    });

            EchoLaunchSetupApplyEligibility eligibility =
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    plan,
                    false);

            Assert.That(eligibility.CanApply, Is.False);
            Assert.That(
                eligibility.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes
                        .UnauthorizedApplyOperation));
        }

        [Test]
        public void ActiveApplyIsRejected()
        {
            Harness harness = CreateHarness(CreateCreatePlan());
            EchoLaunchSetupApplyService.SetApplyActiveForTests(true);

            EchoLaunchSetupApplyResult result =
                ApplyConfirmed(harness);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoLaunchSetupApplyStatus.AlreadyRunning));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes.ApplyAlreadyRunning));
        }

        [Test]
        public void StalePlanAbortsBeforeWriterCalls()
        {
            EchoLaunchSetupPlan displayed = CreateCreatePlan();
            EchoLaunchSetupPlan fresh = CreateNoChangesPlan();
            Harness harness = CreateHarness(displayed, freshPlan: fresh);

            EchoLaunchSetupApplyResult result =
                ApplyConfirmed(harness);

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.StalePlan));

            Assert.That(harness.Calls.Count, Is.EqualTo(0));
        }

        [Test]
        public void NoChangePlanReturnsNoChanges()
        {
            EchoLaunchSetupPlan plan = CreateNoChangesPlan();
            Harness harness = CreateHarness(plan);

            EchoLaunchSetupApplyResult result =
                ApplyConfirmed(harness);

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.NoChanges));

            Assert.That(harness.Calls.Count, Is.EqualTo(0));
        }

        [Test]
        public void NoChangePlanReportsReusedPaths()
        {
            EchoLaunchSetupPlan plan = CreateNoChangesPlan();
            Harness harness = CreateHarness(plan);

            EchoLaunchSetupApplyResult result =
                ApplyConfirmed(harness);

            Assert.That(
                result.ReusedPaths,
                Does.Contain(plan.Paths.ConfigurationAssetPath));
        }

        [Test]
        public void CreatePlanReturnsSucceeded()
        {
            Harness harness = CreateHarness(CreateCreatePlan());

            EchoLaunchSetupApplyResult result =
                ApplyConfirmed(harness);

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.Succeeded));
        }

        [Test]
        public void AssetPhasesUseApprovedOrder()
        {
            Harness harness = CreateHarness(CreateCreatePlan());

            ApplyConfirmed(harness);

            Assert.That(
                harness.Calls.IndexOf("startup"),
                Is.LessThan(harness.Calls.IndexOf("destination")));

            Assert.That(
                harness.Calls.IndexOf("destination"),
                Is.LessThan(harness.Calls.IndexOf("configuration")));

            Assert.That(
                harness.Calls.IndexOf("configuration"),
                Is.LessThan(harness.Calls.IndexOf("prefab")));

            Assert.That(
                harness.Calls.IndexOf("prefab"),
                Is.LessThan(harness.Calls.IndexOf("scene")));
        }

        [Test]
        public void BuildSettingsRunsLast()
        {
            Harness harness = CreateHarness(CreateCreatePlan());

            ApplyConfirmed(harness);

            Assert.That(
                harness.Calls[harness.Calls.Count - 1],
                Is.EqualTo("build"));
        }

        [Test]
        public void OptionalSplashRunsBeforeConfiguration()
        {
            Harness harness =
                CreateHarness(CreateCreatePlan(includeSplash: true));

            ApplyConfirmed(harness);

            Assert.That(
                harness.Calls.IndexOf("splash"),
                Is.LessThan(harness.Calls.IndexOf("configuration")));
        }

        [Test]
        public void FailureBeforePrefabReturnsRolledBack()
        {
            EchoLaunchSetupFailureInjector injector =
                new EchoLaunchSetupFailureInjector
                {
                    FailureKind =
                        EchoLaunchSetupOperationKind
                            .ResolveRootPrefabVariant
                };

            Harness harness =
                CreateHarness(
                    CreateCreatePlan(),
                    failureInjector: injector);

            EchoLaunchSetupApplyResult result =
                ApplyConfirmed(harness);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoLaunchSetupApplyStatus.FailedRolledBack));

            Assert.That(result.RollbackCompleted, Is.True);
        }

        [Test]
        public void FailureResultUsesStableDiagnostic()
        {
            EchoLaunchSetupFailureInjector injector =
                new EchoLaunchSetupFailureInjector
                {
                    FailureKind =
                        EchoLaunchSetupOperationKind
                            .ResolveBootScene
                };

            Harness harness =
                CreateHarness(
                    CreateCreatePlan(),
                    failureInjector: injector);

            EchoLaunchSetupApplyResult result =
                ApplyConfirmed(harness);

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes
                        .ApplyFailedRolledBack));
        }

        [Test]
        public void ApplyAuthorityResetsAfterFailure()
        {
            EchoLaunchSetupFailureInjector injector =
                new EchoLaunchSetupFailureInjector
                {
                    FailureKind =
                        EchoLaunchSetupOperationKind
                            .ResolveConfiguration
                };

            Harness harness =
                CreateHarness(
                    CreateCreatePlan(),
                    failureInjector: injector);

            ApplyConfirmed(harness);

            Assert.That(
                EchoLaunchSetupApplyService.IsApplyActive,
                Is.False);
        }

        [Test]
        public void PlaceFirstApprovalReachesBuildWriter()
        {
            Harness harness =
                CreateHarness(CreatePlaceFirstPlan());

            EchoLaunchSetupApplyResult result =
                harness.Service.Apply(
                    new EchoLaunchSetupApplyRequest(
                        harness.DisplayedPlan,
                        true,
                        true));

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupApplyStatus.Succeeded));

            Assert.That(harness.BuildWriter.ApprovalReceived, Is.True);
        }

        private static EchoLaunchSetupApplyResult ApplyConfirmed(
            Harness harness)
        {
            return harness.Service.Apply(
                new EchoLaunchSetupApplyRequest(
                    harness.DisplayedPlan,
                    true,
                    false));
        }

        private static Harness CreateHarness(
            EchoLaunchSetupPlan displayedPlan,
            EchoLaunchSetupPlan freshPlan = null,
            bool busy = false,
            EchoLaunchSetupFailureInjector failureInjector = null)
        {
            List<string> calls = new List<string>();

            StaticSnapshotSource snapshotSource =
                new StaticSnapshotSource(
                    EchoLaunchSetupTestFactory.CreateSnapshot());

            StaticPlanSource planSource =
                new StaticPlanSource(
                    freshPlan ?? displayedPlan);

            FakeAssetWriter assetWriter =
                new FakeAssetWriter(calls);

            FakePrefabWriter prefabWriter =
                new FakePrefabWriter(calls);

            FakeSceneWriter sceneWriter =
                new FakeSceneWriter(calls);

            FakeBuildSettingsWriter buildWriter =
                new FakeBuildSettingsWriter(calls);

            EchoLaunchSetupApplyService service =
                new EchoLaunchSetupApplyService(
                    snapshotSource,
                    planSource,
                    assetWriter,
                    prefabWriter,
                    sceneWriter,
                    buildWriter,
                    failureInjector ??
                    new EchoLaunchSetupFailureInjector(),
                    delegate { return busy; });

            return new Harness(
                displayedPlan,
                service,
                calls,
                buildWriter);
        }

        private static EchoLaunchSetupPlan CreateCreatePlan(
            bool includeSplash = false)
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            List<EchoLaunchSetupOperation> operations =
                CreateBaseOperations(paths);

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.EnsureFolder,
                    EchoLaunchSetupOperationDisposition.Create,
                    paths.ProjectRootPath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveStartupSequence,
                    EchoLaunchSetupOperationDisposition.Create,
                    paths.StartupSequenceAssetPath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveLaunchDestination,
                    EchoLaunchSetupOperationDisposition.Create,
                    paths.LaunchDestinationAssetPath));

            if (includeSplash)
            {
                operations.Add(
                    Operation(
                        EchoLaunchSetupOperationKind.ResolveSplashSequence,
                        EchoLaunchSetupOperationDisposition.Create,
                        paths.SplashSequenceAssetPath));
            }

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveConfiguration,
                    EchoLaunchSetupOperationDisposition.Create,
                    paths.ConfigurationAssetPath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveRootPrefabVariant,
                    EchoLaunchSetupOperationDisposition.Create,
                    paths.RootPrefabPath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveBootScene,
                    EchoLaunchSetupOperationDisposition.Create,
                    paths.BootScenePath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveBuildSettings,
                    EchoLaunchSetupOperationDisposition.Create,
                    paths.BootScenePath));

            return CreatePlan(
                EchoLaunchSetupPlanStatus.Ready,
                operations,
                includeSplash);
        }

        private static EchoLaunchSetupPlan CreateNoChangesPlan()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            List<EchoLaunchSetupOperation> operations =
                CreateBaseOperations(paths);

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveStartupSequence,
                    EchoLaunchSetupOperationDisposition.Reuse,
                    paths.StartupSequenceAssetPath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveLaunchDestination,
                    EchoLaunchSetupOperationDisposition.Reuse,
                    paths.LaunchDestinationAssetPath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveConfiguration,
                    EchoLaunchSetupOperationDisposition.Reuse,
                    paths.ConfigurationAssetPath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveRootPrefabVariant,
                    EchoLaunchSetupOperationDisposition.Reuse,
                    paths.RootPrefabPath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveBootScene,
                    EchoLaunchSetupOperationDisposition.Reuse,
                    paths.BootScenePath));

            operations.Add(
                Operation(
                    EchoLaunchSetupOperationKind.ResolveBuildSettings,
                    EchoLaunchSetupOperationDisposition.NoChange,
                    paths.BootScenePath));

            return CreatePlan(
                EchoLaunchSetupPlanStatus.Ready,
                operations);
        }

        private static EchoLaunchSetupPlan CreatePlaceFirstPlan()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            List<EchoLaunchSetupOperation> operations =
                CreateBaseOperations(paths);

            operations.Add(
                new EchoLaunchSetupOperation(
                    "build",
                    50,
                    EchoLaunchSetupOperationKind.ResolveBuildSettings,
                    EchoLaunchSetupOperationDisposition.ManualDecision,
                    paths.BootScenePath,
                    "Place Boot first.",
                    EchoLaunchSetupDiagnosticCodes.BuildSettingsApproval,
                    true));

            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest(
                    policy:
                    EchoLaunchBuildSettingsPolicy.PlaceFirstAfterApproval);

            return new EchoLaunchSetupPlan(
                request,
                paths,
                "Evidence",
                EchoLaunchSetupPlanStatus.ReadyWithWarnings,
                operations,
                new[]
                {
                    new EchoLaunchSetupDiagnostic(
                        EchoLaunchSetupDiagnosticCodes.BuildSettingsApproval,
                        EchoLaunchSetupDiagnosticSeverity.Warning,
                        "Approval required.",
                        paths.BootScenePath)
                },
                EchoLaunchSetupFingerprint.ForRequest(request),
                "evidence",
                null);
        }

        private static List<EchoLaunchSetupOperation> CreateBaseOperations(
            EchoLaunchSetupPathSet paths)
        {
            return new List<EchoLaunchSetupOperation>
            {
                Operation(
                    EchoLaunchSetupOperationKind.ValidateRequest,
                    EchoLaunchSetupOperationDisposition.NoChange,
                    paths.ProjectRootPath),
                Operation(
                    EchoLaunchSetupOperationKind.ValidatePackageTemplate,
                    EchoLaunchSetupOperationDisposition.NoChange,
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath),
                Operation(
                    EchoLaunchSetupOperationKind.ValidateDestinationScene,
                    EchoLaunchSetupOperationDisposition.Reuse,
                    EchoLaunchSetupTestFactory.DestinationScenePath)
            };
        }

        private static EchoLaunchSetupPlan CreatePlan(
            EchoLaunchSetupPlanStatus status,
            IEnumerable<EchoLaunchSetupOperation> operations,
            bool createSplash = false)
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest(
                    createSplash);

            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            return new EchoLaunchSetupPlan(
                request,
                paths,
                "Evidence",
                status,
                operations,
                null,
                EchoLaunchSetupFingerprint.ForRequest(request),
                "evidence",
                null);
        }

        private static EchoLaunchSetupOperation Operation(
            EchoLaunchSetupOperationKind kind,
            EchoLaunchSetupOperationDisposition disposition,
            string path,
            string code = "")
        {
            return new EchoLaunchSetupOperation(
                kind.ToString(),
                (int)kind * 10,
                kind,
                disposition,
                path,
                disposition.ToString(),
                code);
        }

        private sealed class Harness
        {
            internal Harness(
                EchoLaunchSetupPlan displayedPlan,
                EchoLaunchSetupApplyService service,
                List<string> calls,
                FakeBuildSettingsWriter buildWriter)
            {
                DisplayedPlan = displayedPlan;
                Service = service;
                Calls = calls;
                BuildWriter = buildWriter;
            }

            internal EchoLaunchSetupPlan DisplayedPlan { get; }
            internal EchoLaunchSetupApplyService Service { get; }
            internal List<string> Calls { get; }
            internal FakeBuildSettingsWriter BuildWriter { get; }
        }

        private sealed class StaticSnapshotSource :
            IEchoLaunchSetupSnapshotSource
        {
            private readonly EchoLaunchProjectSnapshot snapshot;

            internal StaticSnapshotSource(
                EchoLaunchProjectSnapshot snapshot)
            {
                this.snapshot = snapshot;
            }

            public EchoLaunchProjectSnapshot Collect(
                EchoLaunchSetupRequest request)
            {
                return snapshot;
            }
        }

        private sealed class StaticPlanSource :
            IEchoLaunchSetupPlanSource
        {
            private readonly EchoLaunchSetupPlan plan;

            internal StaticPlanSource(EchoLaunchSetupPlan plan)
            {
                this.plan = plan;
            }

            public EchoLaunchSetupPlan CreatePlan(
                EchoLaunchSetupRequest request,
                EchoLaunchProjectSnapshot snapshot)
            {
                return plan;
            }
        }

        private sealed class FakeAssetWriter :
            IEchoLaunchSetupAssetWriter
        {
            private readonly List<string> calls;

            internal FakeAssetWriter(List<string> calls)
            {
                this.calls = calls;
            }

            public void EnsureFolder(
                string path,
                EchoLaunchSetupRollbackJournal journal,
                EchoLaunchSetupExecutionLog log)
            {
                calls.Add("folder");
            }

            public void CreateStartupSequence(
                string path,
                EchoLaunchSetupRollbackJournal journal,
                EchoLaunchSetupExecutionLog log)
            {
                calls.Add("startup");
            }

            public void CreateLaunchDestination(
                string path,
                string destinationScenePath,
                EchoLaunchSetupRollbackJournal journal,
                EchoLaunchSetupExecutionLog log)
            {
                calls.Add("destination");
            }

            public void CreateSplashSequence(
                string path,
                EchoLaunchSetupRollbackJournal journal,
                EchoLaunchSetupExecutionLog log)
            {
                calls.Add("splash");
            }

            public void CreateConfiguration(
                string path,
                string startupSequencePath,
                string launchDestinationPath,
                string splashSequencePath,
                EchoLaunchSetupRollbackJournal journal,
                EchoLaunchSetupExecutionLog log)
            {
                calls.Add("configuration");
            }
        }

        private sealed class FakePrefabWriter :
            IEchoLaunchSetupPrefabWriter
        {
            private readonly List<string> calls;

            internal FakePrefabWriter(List<string> calls)
            {
                this.calls = calls;
            }

            public void CreateRootVariant(
                string templatePath,
                string targetPath,
                string configurationPath,
                EchoLaunchSetupRollbackJournal journal,
                EchoLaunchSetupExecutionLog log)
            {
                calls.Add("prefab");
            }
        }

        private sealed class FakeSceneWriter :
            IEchoLaunchSetupSceneWriter
        {
            private readonly List<string> calls;

            internal FakeSceneWriter(List<string> calls)
            {
                this.calls = calls;
            }

            public void CreateBootScene(
                string scenePath,
                string rootPrefabPath,
                EchoLaunchSetupRollbackJournal journal,
                EchoLaunchSetupExecutionLog log)
            {
                calls.Add("scene");
            }
        }

        private sealed class FakeBuildSettingsWriter :
            IEchoLaunchSetupBuildSettingsWriter
        {
            private readonly List<string> calls;

            internal FakeBuildSettingsWriter(List<string> calls)
            {
                this.calls = calls;
            }

            internal bool ApprovalReceived { get; private set; }

            public bool Apply(
                EchoLaunchBuildSettingsPolicy policy,
                string bootScenePath,
                bool approvePlaceFirst,
                EchoLaunchSetupRollbackJournal journal,
                EchoLaunchSetupExecutionLog log)
            {
                calls.Add("build");
                ApprovalReceived = approvePlaceFirst;
                return true;
            }
        }
    }
}
