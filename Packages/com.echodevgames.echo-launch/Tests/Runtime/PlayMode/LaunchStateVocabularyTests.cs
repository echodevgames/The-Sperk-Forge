//----- LaunchStateVocabularyTests.cs START -----

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class LaunchStateVocabularyTests
    {
        private static IEnumerable<float> InvalidProgressValues
        {
            get
            {
                yield return -0.01f;
                yield return 1.01f;
                yield return float.NaN;
                yield return float.PositiveInfinity;
                yield return float.NegativeInfinity;
            }
        }

        private static IEnumerable<double> InvalidElapsedValues
        {
            get
            {
                yield return -0.01d;
                yield return double.NaN;
                yield return double.PositiveInfinity;
                yield return double.NegativeInfinity;
            }
        }

        [Test]
        public void EnumValuesRemainStable()
        {
            Assert.That((int)LaunchMode.Unknown, Is.EqualTo(0));
            Assert.That((int)LaunchMode.CanonicalBoot, Is.EqualTo(1));
            Assert.That((int)LaunchMode.DirectSceneDevelopment, Is.EqualTo(2));

            Assert.That((int)LaunchStatus.None, Is.EqualTo(0));
            Assert.That((int)LaunchStatus.AuthorityClaimed, Is.EqualTo(1));
            Assert.That((int)LaunchStatus.Validating, Is.EqualTo(2));
            Assert.That((int)LaunchStatus.Running, Is.EqualTo(3));
            Assert.That((int)LaunchStatus.Transitioning, Is.EqualTo(4));
            Assert.That((int)LaunchStatus.Completed, Is.EqualTo(5));
            Assert.That((int)LaunchStatus.Failed, Is.EqualTo(6));
            Assert.That((int)LaunchStatus.Interrupted, Is.EqualTo(7));

            Assert.That((int)StartupStepStatus.NotStarted, Is.EqualTo(0));
            Assert.That((int)StartupStepStatus.Running, Is.EqualTo(1));
            Assert.That((int)StartupStepStatus.Succeeded, Is.EqualTo(2));
            Assert.That((int)StartupStepStatus.Warning, Is.EqualTo(3));
            Assert.That((int)StartupStepStatus.RecoverableFailure, Is.EqualTo(4));
            Assert.That((int)StartupStepStatus.BlockingFailure, Is.EqualTo(5));
            Assert.That((int)StartupStepStatus.Skipped, Is.EqualTo(6));
            Assert.That((int)StartupStepStatus.TimedOut, Is.EqualTo(7));
            Assert.That((int)StartupStepStatus.Cancelled, Is.EqualTo(8));
        }

        [Test]
        public void FactoriesCreateExpectedStatuses()
        {
            Assert.That(
                StartupStepResult.Success().Status,
                Is.EqualTo(StartupStepStatus.Succeeded));

            Assert.That(
                StartupStepResult.Warning("WARN-001", "Warning").Status,
                Is.EqualTo(StartupStepStatus.Warning));

            Assert.That(
                StartupStepResult.RecoverableFailure(
                    "FAIL-001",
                    "Recoverable failure").Status,
                Is.EqualTo(StartupStepStatus.RecoverableFailure));

            Assert.That(
                StartupStepResult.BlockingFailure(
                    "FAIL-002",
                    "Blocking failure").Status,
                Is.EqualTo(StartupStepStatus.BlockingFailure));

            Assert.That(
                StartupStepResult.Skipped().Status,
                Is.EqualTo(StartupStepStatus.Skipped));

            Assert.That(
                StartupStepResult.TimedOut(
                    "TIME-001",
                    "Timed out").Status,
                Is.EqualTo(StartupStepStatus.TimedOut));

            Assert.That(
                StartupStepResult.Cancelled(
                    "CANCEL-001",
                    "Cancelled").Status,
                Is.EqualTo(StartupStepStatus.Cancelled));
        }

        [TestCase(StartupStepStatus.Succeeded, true, false, false)]
        [TestCase(StartupStepStatus.Warning, true, false, false)]
        [TestCase(StartupStepStatus.RecoverableFailure, false, true, false)]
        [TestCase(StartupStepStatus.BlockingFailure, false, true, true)]
        [TestCase(StartupStepStatus.Skipped, false, false, false)]
        [TestCase(StartupStepStatus.TimedOut, false, false, false)]
        [TestCase(StartupStepStatus.Cancelled, false, false, false)]
        public void ClassificationMatchesPolicyNeutralSemantics(
            StartupStepStatus status,
            bool expectedSuccessful,
            bool expectedFailure,
            bool expectedBlocking)
        {
            StartupStepResult result = CreateResultForStatus(status);

            Assert.That(result.IsSuccessful, Is.EqualTo(expectedSuccessful));
            Assert.That(result.IsFailure, Is.EqualTo(expectedFailure));
            Assert.That(result.IsBlocking, Is.EqualTo(expectedBlocking));
        }

        [Test]
        public void ResultTextIsNormalized()
        {
            StartupStepResult warning = StartupStepResult.Warning(
                "  WARN-001  ",
                "  Something happened.  ",
                "  Extra details.  ");

            Assert.That(warning.Code, Is.EqualTo("WARN-001"));
            Assert.That(warning.Message, Is.EqualTo("Something happened."));
            Assert.That(warning.Details, Is.EqualTo("Extra details."));

            StartupStepResult success = StartupStepResult.Success(null, null);

            Assert.That(success.Code, Is.Empty);
            Assert.That(success.Message, Is.Empty);
            Assert.That(success.Details, Is.Empty);
        }

        [TestCase(StartupStepStatus.NotStarted)]
        [TestCase(StartupStepStatus.Running)]
        public void CompletedResultRejectsActiveStatus(
            StartupStepStatus status)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new StartupStepResult(
                    status,
                    string.Empty,
                    string.Empty,
                    string.Empty));
        }

        [Test]
        public void CompletedResultRejectsUndefinedStatus()
        {
            StartupStepStatus invalidStatus = (StartupStepStatus)999;

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new StartupStepResult(
                    invalidStatus,
                    "CODE",
                    "Message",
                    string.Empty));
        }

        [TestCase(StartupStepStatus.Warning)]
        [TestCase(StartupStepStatus.RecoverableFailure)]
        [TestCase(StartupStepStatus.BlockingFailure)]
        [TestCase(StartupStepStatus.TimedOut)]
        [TestCase(StartupStepStatus.Cancelled)]
        public void DiagnosticStatusRequiresCode(
            StartupStepStatus status)
        {
            Assert.Throws<ArgumentException>(
                () => new StartupStepResult(
                    status,
                    "   ",
                    "Message",
                    string.Empty));
        }

        [TestCase(StartupStepStatus.Warning)]
        [TestCase(StartupStepStatus.RecoverableFailure)]
        [TestCase(StartupStepStatus.BlockingFailure)]
        [TestCase(StartupStepStatus.TimedOut)]
        [TestCase(StartupStepStatus.Cancelled)]
        public void DiagnosticStatusRequiresMessage(
            StartupStepStatus status)
        {
            Assert.Throws<ArgumentException>(
                () => new StartupStepResult(
                    status,
                    "CODE",
                    "   ",
                    string.Empty));
        }

        [Test]
        public void SnapshotRepresentsNoActiveStep()
        {
            LaunchProgressSnapshot snapshot = new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                LaunchStatus.AuthorityClaimed,
                null,
                -1,
                3,
                0f,
                true,
                null,
                0d,
                null);

            Assert.That(snapshot.Mode, Is.EqualTo(LaunchMode.CanonicalBoot));
            Assert.That(
                snapshot.Status,
                Is.EqualTo(LaunchStatus.AuthorityClaimed));
            Assert.That(snapshot.ActiveStepId, Is.Empty);
            Assert.That(snapshot.ActiveStepIndex, Is.EqualTo(-1));
            Assert.That(snapshot.TotalStepCount, Is.EqualTo(3));
            Assert.That(snapshot.Progress01, Is.EqualTo(0f));
            Assert.That(snapshot.IsProgressIndeterminate, Is.True);
            Assert.That(snapshot.Message, Is.Empty);
            Assert.That(snapshot.ElapsedSeconds, Is.EqualTo(0d));
            Assert.That(snapshot.LastResult, Is.Null);
        }

        [Test]
        public void SnapshotRepresentsActiveStep()
        {
            StartupStepResult result = StartupStepResult.Success(
                "Settings initialized.");

            LaunchProgressSnapshot snapshot = new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                LaunchStatus.Running,
                "initialize-settings",
                1,
                4,
                0.5f,
                false,
                "Initializing settings.",
                1.25d,
                result);

            Assert.That(
                snapshot.ActiveStepId,
                Is.EqualTo("initialize-settings"));
            Assert.That(snapshot.ActiveStepIndex, Is.EqualTo(1));
            Assert.That(snapshot.TotalStepCount, Is.EqualTo(4));
            Assert.That(snapshot.Progress01, Is.EqualTo(0.5f));
            Assert.That(snapshot.IsProgressIndeterminate, Is.False);
            Assert.That(
                snapshot.Message,
                Is.EqualTo("Initializing settings."));
            Assert.That(snapshot.ElapsedSeconds, Is.EqualTo(1.25d));
            Assert.That(snapshot.LastResult, Is.SameAs(result));
        }

        [Test]
        public void SnapshotNormalizesNullStrings()
        {
            LaunchProgressSnapshot snapshot = new LaunchProgressSnapshot(
                LaunchMode.Unknown,
                LaunchStatus.None,
                null,
                -1,
                0,
                0f,
                true,
                null,
                0d,
                null);

            Assert.That(snapshot.ActiveStepId, Is.Empty);
            Assert.That(snapshot.Message, Is.Empty);
        }

        [Test]
        public void SnapshotRejectsNegativeTotalStepCount()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => CreateSnapshot(
                    activeStepIndex: -1,
                    totalStepCount: -1));
        }

        [Test]
        public void SnapshotRejectsIndexBelowMinusOne()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => CreateSnapshot(
                    activeStepIndex: -2,
                    totalStepCount: 3));
        }

        [Test]
        public void SnapshotRejectsIndexOutsideTotalCount()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => CreateSnapshot(
                    activeStepIndex: 3,
                    totalStepCount: 3));
        }

        [TestCaseSource(nameof(InvalidProgressValues))]
        public void SnapshotRejectsInvalidProgress(float progress01)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => CreateSnapshot(progress01: progress01));
        }

        [TestCaseSource(nameof(InvalidElapsedValues))]
        public void SnapshotRejectsInvalidElapsedTime(double elapsedSeconds)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => CreateSnapshot(elapsedSeconds: elapsedSeconds));
        }

        [Test]
        public void CreatingNewSnapshotDoesNotMutatePreviousSnapshot()
        {
            LaunchProgressSnapshot first = new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                LaunchStatus.Validating,
                "validate",
                0,
                2,
                0.25f,
                false,
                "Validating.",
                0.5d,
                null);

            StartupStepResult result = StartupStepResult.Success(
                "Validation complete.");

            LaunchProgressSnapshot second = new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                LaunchStatus.Running,
                "initialize",
                1,
                2,
                0.75f,
                false,
                "Initializing.",
                1.5d,
                result);

            Assert.That(first.Status, Is.EqualTo(LaunchStatus.Validating));
            Assert.That(first.ActiveStepId, Is.EqualTo("validate"));
            Assert.That(first.ActiveStepIndex, Is.EqualTo(0));
            Assert.That(first.Progress01, Is.EqualTo(0.25f));
            Assert.That(first.ElapsedSeconds, Is.EqualTo(0.5d));
            Assert.That(first.LastResult, Is.Null);

            Assert.That(second.Status, Is.EqualTo(LaunchStatus.Running));
            Assert.That(second.LastResult, Is.SameAs(result));
        }

        private static StartupStepResult CreateResultForStatus(
            StartupStepStatus status)
        {
            switch (status)
            {
                case StartupStepStatus.Succeeded:
                    return StartupStepResult.Success();

                case StartupStepStatus.Warning:
                    return StartupStepResult.Warning(
                        "WARN-001",
                        "Warning");

                case StartupStepStatus.RecoverableFailure:
                    return StartupStepResult.RecoverableFailure(
                        "FAIL-001",
                        "Recoverable failure");

                case StartupStepStatus.BlockingFailure:
                    return StartupStepResult.BlockingFailure(
                        "FAIL-002",
                        "Blocking failure");

                case StartupStepStatus.Skipped:
                    return StartupStepResult.Skipped();

                case StartupStepStatus.TimedOut:
                    return StartupStepResult.TimedOut(
                        "TIME-001",
                        "Timed out");

                case StartupStepStatus.Cancelled:
                    return StartupStepResult.Cancelled(
                        "CANCEL-001",
                        "Cancelled");

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(status),
                        status,
                        "The test does not support this status.");
            }
        }

        private static LaunchProgressSnapshot CreateSnapshot(
            int activeStepIndex = -1,
            int totalStepCount = 0,
            float progress01 = 0f,
            double elapsedSeconds = 0d)
        {
            return new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                LaunchStatus.Running,
                string.Empty,
                activeStepIndex,
                totalStepCount,
                progress01,
                false,
                string.Empty,
                elapsedSeconds,
                null);
        }
    }
}

//----- LaunchStateVocabularyTests.cs END -----
