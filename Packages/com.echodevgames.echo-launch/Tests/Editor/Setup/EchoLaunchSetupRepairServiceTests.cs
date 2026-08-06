using System;
using System.IO;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupRepairServiceTests
    {
        [TearDown]
        public void TearDown()
        {
            EchoLaunchSetupApplyService.SetApplyActiveForTests(false);
        }

        [Test]
        public void RepairPlanIsEligible()
        {
            Assert.That(
                EchoLaunchSetupRepairService.EvaluateEligibility(
                    CreateRepairPlan(false),
                    false).CanRepair,
                Is.True);
        }

        [Test]
        public void CreateOnlyPlanMustUseApply()
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest();
            EchoLaunchSetupPlan plan =
                new EchoLaunchSetupPlanner().CreatePlan(
                    request,
                    EchoLaunchSetupTestFactory.CreateSnapshot());

            EchoLaunchSetupRepairEligibility eligibility =
                EchoLaunchSetupRepairService.EvaluateEligibility(
                    plan,
                    false);

            Assert.That(eligibility.CanRepair, Is.False);
            Assert.That(
                eligibility.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes.RepairApprovalRequired));
        }

        [Test]
        public void CreateOnlyApplyRejectsRepairDisposition()
        {
            EchoLaunchSetupApplyEligibility eligibility =
                EchoLaunchSetupApplyService.EvaluateEligibility(
                    CreateRepairPlan(false),
                    false);

            Assert.That(eligibility.CanApply, Is.False);
            Assert.That(
                eligibility.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes
                        .UnauthorizedApplyOperation));
        }

        [Test]
        public void UnauthorizedRepairKindIsRejected()
        {
            EchoLaunchSetupPlan plan =
                new EchoLaunchSetupPlan(
                    EchoLaunchSetupTestFactory.CreateRequest(),
                    EchoLaunchSetupPathSet.CreateDefault(),
                    "Evidence",
                    EchoLaunchSetupPlanStatus.Ready,
                    new[]
                    {
                        new EchoLaunchSetupOperation(
                            "repair.folder",
                            10,
                            EchoLaunchSetupOperationKind.EnsureFolder,
                            EchoLaunchSetupOperationDisposition.Repair,
                            "Assets/Folder",
                            "Repair")
                    },
                    null);

            EchoLaunchSetupRepairEligibility eligibility =
                EchoLaunchSetupRepairService.EvaluateEligibility(
                    plan,
                    false);

            Assert.That(eligibility.CanRepair, Is.False);
            Assert.That(
                eligibility.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes
                        .UnauthorizedApplyOperation));
        }

        [Test]
        public void PlaceFirstRepairRequiresApproval()
        {
            Assert.That(
                EchoLaunchSetupRepairService.EvaluateEligibility(
                    CreateRepairPlan(true),
                    false).CanRepair,
                Is.False);

            Assert.That(
                EchoLaunchSetupRepairService.EvaluateEligibility(
                    CreateRepairPlan(true),
                    true).CanRepair,
                Is.True);
        }

        [Test]
        public void SettledPlanCanProveNoChanges()
        {
            EchoLaunchSetupPlan plan =
                new EchoLaunchSetupPlan(
                    EchoLaunchSetupTestFactory.CreateRequest(),
                    EchoLaunchSetupPathSet.CreateDefault(),
                    "Evidence",
                    EchoLaunchSetupPlanStatus.Ready,
                    null,
                    null);

            Assert.That(
                EchoLaunchSetupRepairService.EvaluateEligibility(
                    plan,
                    false).CanRepair,
                Is.True);
        }

        [Test]
        public void ActiveApplyBlocksRepairAuthority()
        {
            EchoLaunchSetupApplyService.SetApplyActiveForTests(true);
            Assert.That(
                EchoLaunchSetupApplyService.IsMutationActive,
                Is.True);
            Assert.That(
                EchoLaunchSetupApplyService.TryEnterMutationAuthority(),
                Is.False);
        }

        [Test]
        public void FreshPlanMismatchReturnsStaleBeforeBackup()
        {
            EchoLaunchSetupPlan displayed = CreateRepairPlan(false);
            EchoLaunchSetupPlan fresh =
                new EchoLaunchSetupPlan(
                    displayed.Request,
                    displayed.Paths,
                    "Fresh evidence",
                    EchoLaunchSetupPlanStatus.Ready,
                    new[]
                    {
                        new EchoLaunchSetupOperation(
                            "repair",
                            20,
                            EchoLaunchSetupOperationKind.ResolveConfiguration,
                            EchoLaunchSetupOperationDisposition.Repair,
                            "Assets/Configuration.asset",
                            "Changed proof",
                            EchoLaunchSetupDiagnosticCodes
                                .RepairApprovalRequired,
                            false,
                            "Before",
                            "After",
                            "Fresh proof")
                    },
                    null);
            EchoLaunchSetupRepairService service =
                new EchoLaunchSetupRepairService(
                    new StaticSnapshotSource(
                        EchoLaunchSetupTestFactory.CreateSnapshot()),
                    new StaticPlanSource(fresh),
                    new EchoLaunchSetupAssetWriter(),
                    new EchoLaunchSetupPrefabWriter(),
                    new EchoLaunchSetupSceneWriter(),
                    new EchoLaunchSetupBuildSettingsWriter(),
                    new ThrowingBackupStore(),
                    new EchoLaunchSetupNoFailureInjector(),
                    delegate { return false; });

            EchoLaunchSetupRepairResult result =
                service.Repair(
                    new EchoLaunchSetupRepairRequest(
                        displayed,
                        true,
                        false));

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupRepairStatus.StalePlan));
            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(EchoLaunchSetupDiagnosticCodes.StalePlan));
        }

        [Test]
        public void BackupFailureAbortsBeforeMutationWithStableDiagnostic()
        {
            EchoLaunchSetupPlan plan = CreateRepairPlan(false);
            EchoLaunchSetupRepairService service =
                new EchoLaunchSetupRepairService(
                    new StaticSnapshotSource(
                        EchoLaunchSetupTestFactory.CreateSnapshot()),
                    new StaticPlanSource(plan),
                    new EchoLaunchSetupAssetWriter(),
                    new EchoLaunchSetupPrefabWriter(),
                    new EchoLaunchSetupSceneWriter(),
                    new EchoLaunchSetupBuildSettingsWriter(),
                    new ThrowingBackupStore(),
                    new EchoLaunchSetupNoFailureInjector(),
                    delegate { return false; });

            EchoLaunchSetupRepairResult result =
                service.Repair(
                    new EchoLaunchSetupRepairRequest(
                        plan,
                        true,
                        false));

            Assert.That(
                result.Status,
                Is.EqualTo(EchoLaunchSetupRepairStatus.BackupFailed));
            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    EchoLaunchSetupDiagnosticCodes.RepairBackupFailed));
            Assert.That(result.Changes, Is.Empty);
            Assert.That(result.BackupDirectory, Is.Empty);
        }

        [Test]
        public void IncompleteRollbackRetainsBackupAndUsesStableDiagnostic()
        {
            string repairId = "test_" + Guid.NewGuid().ToString("N");
            string backupDirectory =
                EchoLaunchSetupRepairBackupStore.BackupRoot + "/" + repairId;
            string absoluteBackup = ProjectAbsolute(backupDirectory);
            Directory.CreateDirectory(absoluteBackup);
            File.WriteAllText(
                Path.Combine(absoluteBackup, "manifest.txt"),
                "retained test backup");

            try
            {
                EchoLaunchSetupPlan plan = CreateRepairPlan(false);
                EchoLaunchSetupRepairBackupSession session =
                    new EchoLaunchSetupRepairBackupSession(
                        repairId,
                        backupDirectory,
                        new[]
                        {
                            new EchoLaunchSetupRepairBackupEntry(
                                "Assets/__MissingRepair.asset",
                                Path.Combine(
                                    absoluteBackup,
                                    "missing.assetbytes"),
                                Path.Combine(
                                    absoluteBackup,
                                    "missing.metabytes"),
                                true,
                                new string('0', 64),
                                new string('1', 64))
                        });
                EchoLaunchSetupFailureInjector injector =
                    new EchoLaunchSetupFailureInjector
                    {
                        FailureKind =
                            EchoLaunchSetupOperationKind
                                .ResolveConfiguration
                    };
                EchoLaunchSetupRepairService service =
                    new EchoLaunchSetupRepairService(
                        new StaticSnapshotSource(
                            EchoLaunchSetupTestFactory.CreateSnapshot()),
                        new StaticPlanSource(plan),
                        new EchoLaunchSetupAssetWriter(),
                        new EchoLaunchSetupPrefabWriter(),
                        new EchoLaunchSetupSceneWriter(),
                        new EchoLaunchSetupBuildSettingsWriter(),
                        new StaticBackupStore(session),
                        injector,
                        delegate { return false; });

                EchoLaunchSetupRepairResult result =
                    service.Repair(
                        new EchoLaunchSetupRepairRequest(
                            plan,
                            true,
                            false));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        EchoLaunchSetupRepairStatus
                            .FailedRollbackIncomplete));
                Assert.That(
                    result.DiagnosticCode,
                    Is.EqualTo(
                        EchoLaunchSetupDiagnosticCodes
                            .RepairRollbackIncomplete));
                Assert.That(result.RollbackCompleted, Is.False);
                Assert.That(
                    result.BackupDirectory,
                    Is.EqualTo(backupDirectory));
                Assert.That(
                    result.ManualRecoveryPaths,
                    Does.Contain("Assets/__MissingRepair.asset"));
                Assert.That(
                    result.ManualRecoveryPaths,
                    Does.Contain(backupDirectory));
                Assert.That(Directory.Exists(absoluteBackup), Is.True);
            }
            finally
            {
                if (Directory.Exists(absoluteBackup))
                {
                    Directory.Delete(absoluteBackup, true);
                }
            }
        }

        private static EchoLaunchSetupPlan CreateRepairPlan(
            bool requiresApproval)
        {
            return new EchoLaunchSetupPlan(
                EchoLaunchSetupTestFactory.CreateRequest(
                    policy: requiresApproval
                        ? EchoLaunchBuildSettingsPolicy
                            .PlaceFirstAfterApproval
                        : EchoLaunchBuildSettingsPolicy
                            .AddIfMissingAtEnd),
                EchoLaunchSetupPathSet.CreateDefault(),
                "Evidence",
                requiresApproval
                    ? EchoLaunchSetupPlanStatus.ReadyWithWarnings
                    : EchoLaunchSetupPlanStatus.Ready,
                new[]
                {
                    new EchoLaunchSetupOperation(
                        "repair",
                        20,
                        requiresApproval
                            ? EchoLaunchSetupOperationKind
                                .ResolveBuildSettings
                            : EchoLaunchSetupOperationKind
                                .ResolveConfiguration,
                        EchoLaunchSetupOperationDisposition.Repair,
                        requiresApproval
                            ? "Assets/Boot.unity"
                            : "Assets/Configuration.asset",
                        "Repair",
                        requiresApproval
                            ? EchoLaunchSetupDiagnosticCodes
                                .BuildSettingsApproval
                            : EchoLaunchSetupDiagnosticCodes
                                .RepairApprovalRequired,
                        requiresApproval,
                        "Before",
                        "After",
                        "Proof")
                },
                null);
        }
        private static string ProjectAbsolute(string relative)
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            return Path.GetFullPath(Path.Combine(root, relative));
        }

        private sealed class StaticSnapshotSource :
            IEchoLaunchSetupSnapshotSource
        {
            private readonly EchoLaunchProjectSnapshot snapshot;

            internal StaticSnapshotSource(EchoLaunchProjectSnapshot snapshot)
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

        private sealed class ThrowingBackupStore :
            IEchoLaunchSetupRepairBackupStore
        {
            public EchoLaunchSetupRepairBackupSession CreateBackup(
                System.Collections.Generic.IEnumerable<string>
                    projectAssetPaths)
            {
                throw new InvalidOperationException("backup failed");
            }
        }

        private sealed class StaticBackupStore :
            IEchoLaunchSetupRepairBackupStore
        {
            private readonly EchoLaunchSetupRepairBackupSession session;

            internal StaticBackupStore(
                EchoLaunchSetupRepairBackupSession session)
            {
                this.session = session;
            }

            public EchoLaunchSetupRepairBackupSession CreateBackup(
                System.Collections.Generic.IEnumerable<string>
                    projectAssetPaths)
            {
                return session;
            }
        }

    }
}
