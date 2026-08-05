using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupApplyModelTests
    {
        [Test]
        public void ApplyRequestRetainsPlan()
        {
            EchoLaunchSetupPlan plan = CreatePlan();

            Assert.That(
                new EchoLaunchSetupApplyRequest(plan, true, false)
                    .DisplayedPlan,
                Is.SameAs(plan));
        }

        [Test]
        public void ApplyRequestRetainsConfirmation()
        {
            Assert.That(
                new EchoLaunchSetupApplyRequest(
                    CreatePlan(),
                    true,
                    false).Confirmed,
                Is.True);
        }

        [Test]
        public void ApplyRequestRetainsApproval()
        {
            Assert.That(
                new EchoLaunchSetupApplyRequest(
                    CreatePlan(),
                    true,
                    true).ApprovePlaceFirst,
                Is.True);
        }

        [Test]
        public void ChangeNormalizesPath()
        {
            EchoLaunchSetupChange change =
                new EchoLaunchSetupChange(
                    EchoLaunchSetupChangeKind.CreatedAsset,
                    @"Assets\Test.asset",
                    "Created.");

            Assert.That(change.Path, Is.EqualTo("Assets/Test.asset"));
        }

        [Test]
        public void EqualChangesCompareEqual()
        {
            EchoLaunchSetupChange first =
                new EchoLaunchSetupChange(
                    EchoLaunchSetupChangeKind.CreatedAsset,
                    "Assets/Test.asset",
                    "Created.");

            EchoLaunchSetupChange second =
                new EchoLaunchSetupChange(
                    EchoLaunchSetupChangeKind.CreatedAsset,
                    "Assets/Test.asset",
                    "Created.");

            Assert.That(first.Equals(second), Is.True);
        }

        [Test]
        public void RollbackResultDefensivelyCopiesPaths()
        {
            List<string> paths =
                new List<string> { "Assets/A.asset" };

            EchoLaunchSetupRollbackResult result =
                new EchoLaunchSetupRollbackResult(false, paths);

            paths.Clear();

            Assert.That(
                result.ManualRecoveryPaths.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void ApplyResultDefensivelyCopiesChanges()
        {
            List<EchoLaunchSetupChange> changes =
                new List<EchoLaunchSetupChange>
                {
                    new EchoLaunchSetupChange(
                        EchoLaunchSetupChangeKind.CreatedAsset,
                        "Assets/A.asset",
                        "Created.")
                };

            EchoLaunchSetupApplyResult result =
                CreateResult(changes, null, null);

            changes.Clear();

            Assert.That(result.Changes.Count, Is.EqualTo(1));
        }

        [Test]
        public void ApplyResultDefensivelyCopiesCreatedPaths()
        {
            List<string> created =
                new List<string> { "Assets/A.asset" };

            EchoLaunchSetupApplyResult result =
                CreateResult(null, created, null);

            created.Clear();

            Assert.That(result.CreatedPaths.Count, Is.EqualTo(1));
        }

        [Test]
        public void ApplyResultDefensivelyCopiesReusedPaths()
        {
            List<string> reused =
                new List<string> { "Assets/B.asset" };

            EchoLaunchSetupApplyResult result =
                CreateResult(null, null, reused);

            reused.Clear();

            Assert.That(result.ReusedPaths.Count, Is.EqualTo(1));
        }

        [Test]
        public void SimpleResultCarriesPlanStatus()
        {
            EchoLaunchSetupPlan plan = CreatePlan();

            EchoLaunchSetupApplyResult result =
                EchoLaunchSetupApplyResult.Simple(
                    EchoLaunchSetupApplyStatus.Blocked,
                    "CODE",
                    "Blocked.",
                    plan);

            Assert.That(
                result.FinalPlanStatus,
                Is.EqualTo(plan.Status));

            Assert.That(
                result.FinalPlanFingerprint,
                Is.EqualTo(plan.PlanFingerprint));
        }

        [Test]
        public void StatusVocabularyIsStable()
        {
            Assert.That(
                (int)EchoLaunchSetupApplyStatus.Succeeded,
                Is.EqualTo(0));

            Assert.That(
                (int)EchoLaunchSetupApplyStatus.NoChanges,
                Is.EqualTo(1));

            Assert.That(
                (int)EchoLaunchSetupApplyStatus.FailedRollbackIncomplete,
                Is.EqualTo(7));
        }

        [Test]
        public void ApplyDiagnosticsAreStable()
        {
            Assert.That(
                EchoLaunchSetupDiagnosticCodes.StalePlan,
                Is.EqualTo("ELAUNCH-SETUP-008"));

            Assert.That(
                EchoLaunchSetupDiagnosticCodes.ApplyAlreadyRunning,
                Is.EqualTo("ELAUNCH-SETUP-009"));

            Assert.That(
                EchoLaunchSetupDiagnosticCodes.ApplyFailedRolledBack,
                Is.EqualTo("ELAUNCH-SETUP-010"));

            Assert.That(
                EchoLaunchSetupDiagnosticCodes.RollbackIncomplete,
                Is.EqualTo("ELAUNCH-SETUP-011"));

            Assert.That(
                EchoLaunchSetupDiagnosticCodes.UnauthorizedApplyOperation,
                Is.EqualTo("ELAUNCH-SETUP-012"));
        }

        private static EchoLaunchSetupPlan CreatePlan()
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest();

            return new EchoLaunchSetupPlanner().CreatePlan(
                request,
                EchoLaunchSetupTestFactory.CreateSnapshot());
        }

        private static EchoLaunchSetupApplyResult CreateResult(
            IEnumerable<EchoLaunchSetupChange> changes,
            IEnumerable<string> created,
            IEnumerable<string> reused)
        {
            return new EchoLaunchSetupApplyResult(
                EchoLaunchSetupApplyStatus.Succeeded,
                string.Empty,
                "Done.",
                changes,
                created,
                reused,
                "Before",
                "After",
                false,
                null,
                EchoLaunchSetupPlanStatus.Ready,
                "fingerprint");
        }
    }
}
