//----- LaunchSessionProgressTests.cs START -----

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class LaunchSessionProgressTests
    {
        private readonly List<GameObject> createdObjects =
            new List<GameObject>();

        [SetUp]
        public void SetUp()
        {
            LaunchAuthorityClaim.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            for (int index = createdObjects.Count - 1;
                 index >= 0;
                 index--)
            {
                GameObject target = createdObjects[index];

                if (target != null)
                {
                    Object.DestroyImmediate(target);
                }
            }

            createdObjects.Clear();
            LaunchAuthorityClaim.Reset();

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void AuthorityCreatesFreshSession()
        {
            EchoLaunchRoot root =
                CreateRoot("Authoritative Root");

            Assert.That(
                root.IsAuthoritative,
                Is.True);

            Assert.That(
                root.State,
                Is.EqualTo(LaunchStatus.AuthorityClaimed));

            Assert.That(
                root.Progress.Status,
                Is.EqualTo(LaunchStatus.AuthorityClaimed));
        }

        [Test]
        public void InitialProgressSnapshotIsCanonical()
        {
            EchoLaunchRoot root =
                CreateRoot("Canonical Root");

            LaunchProgressSnapshot progress =
                root.Progress;

            Assert.That(
                progress.Mode,
                Is.EqualTo(LaunchMode.CanonicalBoot));

            Assert.That(
                progress.Status,
                Is.EqualTo(LaunchStatus.AuthorityClaimed));

            Assert.That(
                progress.ActiveStepId,
                Is.Empty);

            Assert.That(
                progress.ActiveStepIndex,
                Is.EqualTo(-1));

            Assert.That(
                progress.TotalStepCount,
                Is.EqualTo(0));

            Assert.That(
                progress.Progress01,
                Is.EqualTo(0f));

            Assert.That(
                progress.IsProgressIndeterminate,
                Is.True);

            Assert.That(
                progress.Message,
                Is.EqualTo("Launch authority claimed."));

            Assert.That(
                progress.ElapsedSeconds,
                Is.EqualTo(0d));

            Assert.That(
                progress.LastResult,
                Is.Null);
        }

        [Test]
        public void SessionUsesSuppliedLaunchMode()
        {
            LaunchSession session =
                new LaunchSession(
                    LaunchMode.DirectSceneDevelopment);

            Assert.That(
                session.Mode,
                Is.EqualTo(
                    LaunchMode.DirectSceneDevelopment));

            Assert.That(
                session.Progress.Mode,
                Is.EqualTo(
                    LaunchMode.DirectSceneDevelopment));

            Assert.That(
                session.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));
        }

        [Test]
        public void EmptySnapshotIsNormalizedAndSafe()
        {
            LaunchProgressSnapshot empty =
                LaunchProgressSnapshot.Empty;

            Assert.That(
                empty.Mode,
                Is.EqualTo(LaunchMode.Unknown));

            Assert.That(
                empty.Status,
                Is.EqualTo(LaunchStatus.None));

            Assert.That(
                empty.ActiveStepId,
                Is.Empty);

            Assert.That(
                empty.ActiveStepIndex,
                Is.EqualTo(-1));

            Assert.That(
                empty.TotalStepCount,
                Is.EqualTo(0));

            Assert.That(
                empty.Progress01,
                Is.EqualTo(0f));

            Assert.That(
                empty.IsProgressIndeterminate,
                Is.True);

            Assert.That(
                empty.Message,
                Is.Empty);

            Assert.That(
                empty.ElapsedSeconds,
                Is.EqualTo(0d));

            Assert.That(
                empty.LastResult,
                Is.Null);
        }

        [Test]
        public void DuplicateRootExposesNoSessionState()
        {
            EchoLaunchRoot authority =
                CreateRoot("Authority");

            ExpectDuplicateWarning();

            EchoLaunchRoot duplicate =
                CreateRoot("Duplicate");

            Assert.That(
                authority.IsAuthoritative,
                Is.True);

            Assert.That(
                duplicate.IsAuthoritative,
                Is.False);

            Assert.That(
                duplicate.State,
                Is.EqualTo(LaunchStatus.None));

            AssertSnapshotIsEmpty(
                duplicate.Progress);
        }

        [Test]
        public void PublishingSnapshotReplacesRootStateAndProgress()
        {
            EchoLaunchRoot root =
                CreateRoot("Publishing Root");

            StartupStepResult result =
                StartupStepResult.Success(
                    "Settings initialized.");

            LaunchProgressSnapshot replacement =
                CreateSnapshot(
                    status: LaunchStatus.Running,
                    activeStepId: "initialize-settings",
                    activeStepIndex: 1,
                    totalStepCount: 4,
                    progress01: 0.5f,
                    message: "Initializing settings.",
                    elapsedSeconds: 1.25d,
                    lastResult: result);

            root.PublishProgress(replacement);

            Assert.That(
                root.State,
                Is.EqualTo(LaunchStatus.Running));

            Assert.That(
                root.Progress.ActiveStepId,
                Is.EqualTo("initialize-settings"));

            Assert.That(
                root.Progress.ActiveStepIndex,
                Is.EqualTo(1));

            Assert.That(
                root.Progress.TotalStepCount,
                Is.EqualTo(4));

            Assert.That(
                root.Progress.Progress01,
                Is.EqualTo(0.5f));

            Assert.That(
                root.Progress.Message,
                Is.EqualTo("Initializing settings."));

            Assert.That(
                root.Progress.ElapsedSeconds,
                Is.EqualTo(1.25d));

            Assert.That(
                root.Progress.LastResult,
                Is.SameAs(result));
        }

        [Test]
        public void SameStatePublicationCanReplaceProgress()
        {
            EchoLaunchRoot root =
                CreateRoot("Same State Root");

            LaunchProgressSnapshot replacement =
                CreateSnapshot(
                    status: LaunchStatus.AuthorityClaimed,
                    activeStepId: string.Empty,
                    activeStepIndex: -1,
                    totalStepCount: 3,
                    progress01: 0.25f,
                    message: "Authority confirmed.",
                    elapsedSeconds: 0.2d,
                    lastResult: null);

            root.PublishProgress(replacement);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            Assert.That(
                root.Progress.TotalStepCount,
                Is.EqualTo(3));

            Assert.That(
                root.Progress.Progress01,
                Is.EqualTo(0.25f));

            Assert.That(
                root.Progress.Message,
                Is.EqualTo("Authority confirmed."));
        }

        [Test]
        public void PreviousSnapshotRemainsUnchangedAfterPublication()
        {
            EchoLaunchRoot root =
                CreateRoot("Immutable Snapshot Root");

            LaunchProgressSnapshot before =
                root.Progress;

            LaunchProgressSnapshot after =
                CreateSnapshot(
                    status: LaunchStatus.Validating,
                    activeStepId: "validate",
                    activeStepIndex: 0,
                    totalStepCount: 2,
                    progress01: 0.25f,
                    message: "Validating.",
                    elapsedSeconds: 0.5d,
                    lastResult: null);

            root.PublishProgress(after);

            Assert.That(
                before.Status,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            Assert.That(
                before.ActiveStepId,
                Is.Empty);

            Assert.That(
                before.ActiveStepIndex,
                Is.EqualTo(-1));

            Assert.That(
                before.TotalStepCount,
                Is.EqualTo(0));

            Assert.That(
                before.Progress01,
                Is.EqualTo(0f));

            Assert.That(
                before.Message,
                Is.EqualTo("Launch authority claimed."));

            Assert.That(
                root.Progress.Status,
                Is.EqualTo(LaunchStatus.Validating));
        }

        [Test]
        public void SessionRejectsModeMismatch()
        {
            LaunchSession session =
                new LaunchSession(
                    LaunchMode.CanonicalBoot);

            LaunchProgressSnapshot snapshot =
                new LaunchProgressSnapshot(
                    LaunchMode.DirectSceneDevelopment,
                    LaunchStatus.Running,
                    string.Empty,
                    -1,
                    0,
                    0f,
                    true,
                    string.Empty,
                    0d,
                    null);

            Assert.Throws<ArgumentException>(
                () => session.Publish(snapshot));
        }

        [Test]
        public void SessionRejectsNoneStatus()
        {
            LaunchSession session =
                new LaunchSession(
                    LaunchMode.CanonicalBoot);

            LaunchProgressSnapshot snapshot =
                new LaunchProgressSnapshot(
                    LaunchMode.CanonicalBoot,
                    LaunchStatus.None,
                    string.Empty,
                    -1,
                    0,
                    0f,
                    true,
                    string.Empty,
                    0d,
                    null);

            Assert.Throws<ArgumentException>(
                () => session.Publish(snapshot));
        }

        [Test]
        public void SessionRejectsUndefinedMode()
        {
            LaunchMode undefinedMode =
                (LaunchMode)999;

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LaunchSession(
                    undefinedMode));
        }

        [Test]
        public void DuplicateRootCannotPublish()
        {
            CreateRoot("Authority");

            ExpectDuplicateWarning();

            EchoLaunchRoot duplicate =
                CreateRoot("Duplicate");

            LaunchProgressSnapshot snapshot =
                CreateSnapshot(
                    status: LaunchStatus.Running,
                    activeStepId: "forged-step",
                    activeStepIndex: 0,
                    totalStepCount: 1,
                    progress01: 0.5f,
                    message: "Forged progress.",
                    elapsedSeconds: 0.25d,
                    lastResult: null);

            Assert.Throws<InvalidOperationException>(
                () => duplicate.PublishProgress(
                    snapshot));
        }

        [Test]
        public void StaticResetHidesStaleSession()
        {
            EchoLaunchRoot root =
                CreateRoot("Stale Root");

            LaunchProgressSnapshot running =
                CreateSnapshot(
                    status: LaunchStatus.Running,
                    activeStepId: "initialize",
                    activeStepIndex: 0,
                    totalStepCount: 1,
                    progress01: 0.5f,
                    message: "Initializing.",
                    elapsedSeconds: 0.5d,
                    lastResult: null);

            root.PublishProgress(running);

            LaunchAuthorityClaim.Reset();

            Assert.That(
                root.IsAuthoritative,
                Is.False);

            Assert.That(
                root.State,
                Is.EqualTo(LaunchStatus.None));

            AssertSnapshotIsEmpty(
                root.Progress);

            Assert.Throws<InvalidOperationException>(
                () => root.PublishProgress(
                    running));
        }

        [Test]
        public void FreshAuthorityReceivesFreshSession()
        {
            EchoLaunchRoot first =
                CreateRoot("First Authority");

            LaunchProgressSnapshot completed =
                CreateSnapshot(
                    status: LaunchStatus.Completed,
                    activeStepId: string.Empty,
                    activeStepIndex: -1,
                    totalStepCount: 2,
                    progress01: 1f,
                    message: "Launch completed.",
                    elapsedSeconds: 2d,
                    lastResult:
                        StartupStepResult.Success(
                            "Complete."));

            first.PublishProgress(completed);

            GameObject firstObject =
                first.gameObject;

            Object.DestroyImmediate(firstObject);

            EchoLaunchRoot second =
                CreateRoot("Second Authority");

            Assert.That(
                EchoLaunchRoot.Current,
                Is.SameAs(second));

            Assert.That(
                second.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            Assert.That(
                second.Progress.Progress01,
                Is.EqualTo(0f));

            Assert.That(
                second.Progress.TotalStepCount,
                Is.EqualTo(0));

            Assert.That(
                second.Progress.LastResult,
                Is.Null);

            Assert.That(
                second.Progress.Message,
                Is.EqualTo(
                    "Launch authority claimed."));
        }

        private EchoLaunchRoot CreateRoot(
            string name)
        {
            GameObject target =
                new GameObject(name);

            createdObjects.Add(target);

            return target.AddComponent<EchoLaunchRoot>();
        }

        private static LaunchProgressSnapshot
            CreateSnapshot(
                LaunchStatus status,
                string activeStepId,
                int activeStepIndex,
                int totalStepCount,
                float progress01,
                string message,
                double elapsedSeconds,
                StartupStepResult lastResult)
        {
            return new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                status,
                activeStepId,
                activeStepIndex,
                totalStepCount,
                progress01,
                false,
                message,
                elapsedSeconds,
                lastResult);
        }

        private static void AssertSnapshotIsEmpty(
            LaunchProgressSnapshot snapshot)
        {
            Assert.That(
                snapshot.Mode,
                Is.EqualTo(LaunchMode.Unknown));

            Assert.That(
                snapshot.Status,
                Is.EqualTo(LaunchStatus.None));

            Assert.That(
                snapshot.ActiveStepId,
                Is.Empty);

            Assert.That(
                snapshot.ActiveStepIndex,
                Is.EqualTo(-1));

            Assert.That(
                snapshot.TotalStepCount,
                Is.EqualTo(0));

            Assert.That(
                snapshot.Progress01,
                Is.EqualTo(0f));

            Assert.That(
                snapshot.IsProgressIndeterminate,
                Is.True);

            Assert.That(
                snapshot.Message,
                Is.Empty);

            Assert.That(
                snapshot.ElapsedSeconds,
                Is.EqualTo(0d));

            Assert.That(
                snapshot.LastResult,
                Is.Null);
        }

        private static void ExpectDuplicateWarning()
        {
            LogAssert.Expect(
                LogType.Warning,
                "[ELAUNCH-ROOT-001] " +
                "Duplicate EchoLaunchRoot rejected. " +
                "The first valid root remains authoritative.");
        }
    }
}

//----- LaunchSessionProgressTests.cs END -----