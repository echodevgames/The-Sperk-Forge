//----- LaunchLifecycleTransitionTests.cs START -----

using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class LaunchLifecycleTransitionTests
    {
        [TestCase(LaunchStatus.Completed)]
        [TestCase(LaunchStatus.Failed)]
        [TestCase(LaunchStatus.Interrupted)]
        public void TerminalStatusIsRecognized(
            LaunchStatus status)
        {
            Assert.That(
                LaunchStateTransitionRules.IsTerminal(status),
                Is.True);
        }

        [Test]
        public void ActiveStatusesAreNotTerminal()
        {
            LaunchStatus[] activeStatuses =
            {
                LaunchStatus.None,
                LaunchStatus.AuthorityClaimed,
                LaunchStatus.Validating,
                LaunchStatus.Running,
                LaunchStatus.Transitioning
            };

            foreach (LaunchStatus status in activeStatuses)
            {
                Assert.That(
                    LaunchStateTransitionRules.IsTerminal(status),
                    Is.False,
                    status.ToString());
            }
        }

        [TestCase(
            LaunchStatus.None,
            LaunchStatus.AuthorityClaimed)]
        [TestCase(
            LaunchStatus.AuthorityClaimed,
            LaunchStatus.Validating)]
        [TestCase(
            LaunchStatus.Validating,
            LaunchStatus.Running)]
        [TestCase(
            LaunchStatus.Running,
            LaunchStatus.Transitioning)]
        [TestCase(
            LaunchStatus.Transitioning,
            LaunchStatus.Completed)]
        public void ApprovedForwardTransitionIsAllowed(
            LaunchStatus current,
            LaunchStatus next)
        {
            Assert.That(
                LaunchStateTransitionRules.CanTransition(
                    current,
                    next),
                Is.True);

            Assert.DoesNotThrow(
                () => LaunchStateTransitionRules
                    .EnsureCanPublish(
                        current,
                        next));
        }

        [TestCase(LaunchStatus.AuthorityClaimed)]
        [TestCase(LaunchStatus.Validating)]
        [TestCase(LaunchStatus.Running)]
        [TestCase(LaunchStatus.Transitioning)]
        public void SameActiveStatePublicationIsAllowed(
            LaunchStatus status)
        {
            Assert.That(
                LaunchStateTransitionRules.CanTransition(
                    status,
                    status),
                Is.True);
        }

        [TestCase(LaunchStatus.AuthorityClaimed)]
        [TestCase(LaunchStatus.Validating)]
        [TestCase(LaunchStatus.Running)]
        [TestCase(LaunchStatus.Transitioning)]
        public void ActiveStateCanFailOrBeInterrupted(
            LaunchStatus status)
        {
            Assert.That(
                LaunchStateTransitionRules.CanTransition(
                    status,
                    LaunchStatus.Failed),
                Is.True);

            Assert.That(
                LaunchStateTransitionRules.CanTransition(
                    status,
                    LaunchStatus.Interrupted),
                Is.True);
        }

        [TestCase(
            LaunchStatus.Validating,
            LaunchStatus.AuthorityClaimed)]
        [TestCase(
            LaunchStatus.Running,
            LaunchStatus.Validating)]
        [TestCase(
            LaunchStatus.AuthorityClaimed,
            LaunchStatus.Running)]
        public void BackwardOrSkippedTransitionIsRejected(
            LaunchStatus current,
            LaunchStatus next)
        {
            Assert.That(
                LaunchStateTransitionRules.CanTransition(
                    current,
                    next),
                Is.False);

            Assert.Throws<InvalidOperationException>(
                () => LaunchStateTransitionRules
                    .EnsureCanPublish(
                        current,
                        next));
        }

        [Test]
        public void UndefinedStatusesAreRejected()
        {
            LaunchStatus undefined =
                (LaunchStatus)999;

            Assert.Throws<ArgumentOutOfRangeException>(
                () => LaunchStateTransitionRules
                    .CanTransition(
                        undefined,
                        LaunchStatus.AuthorityClaimed));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => LaunchStateTransitionRules
                    .CanTransition(
                        LaunchStatus.AuthorityClaimed,
                        undefined));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => LaunchStateTransitionRules
                    .EnsureCanPublish(
                        undefined,
                        LaunchStatus.AuthorityClaimed));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => LaunchStateTransitionRules
                    .EnsureCanPublish(
                        LaunchStatus.AuthorityClaimed,
                        undefined));
        }

        [Test]
        public void SessionAndRootGuardAreTransactional()
        {
            LaunchSession noneSession =
                new LaunchSession(
                    LaunchMode.CanonicalBoot);

            LaunchProgressSnapshot noneBefore =
                noneSession.Progress;

            Assert.Throws<InvalidOperationException>(
                () => noneSession.Publish(
                    CreateSnapshot(
                        LaunchStatus.None,
                        "Illegal None")));

            AssertSnapshotUnchanged(
                noneBefore,
                noneSession.Progress);

            LaunchSession skippedSession =
                new LaunchSession(
                    LaunchMode.CanonicalBoot);

            LaunchProgressSnapshot skippedBefore =
                skippedSession.Progress;

            Assert.Throws<InvalidOperationException>(
                () => skippedSession.Publish(
                    CreateSnapshot(
                        LaunchStatus.Running,
                        "Skipped validation")));

            AssertSnapshotUnchanged(
                skippedBefore,
                skippedSession.Progress);

            VerifyTerminalSessionIsFrozen(
                LaunchStatus.Completed);

            VerifyTerminalSessionIsFrozen(
                LaunchStatus.Failed);

            VerifyTerminalSessionIsFrozen(
                LaunchStatus.Interrupted);

            LaunchAuthorityClaim.Reset();

            GameObject target =
                new GameObject("Guarded Root");

            try
            {
                EchoLaunchRoot root =
                    target.AddComponent<EchoLaunchRoot>();

                LaunchProgressSnapshot before =
                    root.Progress;

                Assert.Throws<InvalidOperationException>(
                    () => root.PublishProgress(
                        CreateSnapshot(
                            LaunchStatus.Running,
                            "Root skipped validation")));

                Assert.That(
                    root.State,
                    Is.EqualTo(
                        LaunchStatus.AuthorityClaimed));

                AssertSnapshotUnchanged(
                    before,
                    root.Progress);

                root.PublishProgress(
                    CreateSnapshot(
                        LaunchStatus.Validating,
                        "Validating"));

                Assert.That(
                    root.State,
                    Is.EqualTo(
                        LaunchStatus.Validating));

                Assert.That(
                    root.Progress.Message,
                    Is.EqualTo("Validating"));
            }
            finally
            {
                Object.DestroyImmediate(target);
                LaunchAuthorityClaim.Reset();
            }
        }

        private static void VerifyTerminalSessionIsFrozen(
            LaunchStatus terminalStatus)
        {
            LaunchSession session =
                new LaunchSession(
                    LaunchMode.CanonicalBoot);

            AdvanceToTerminal(
                session,
                terminalStatus);

            LaunchProgressSnapshot before =
                session.Progress;

            Assert.Throws<InvalidOperationException>(
                () => session.Publish(
                    CreateSnapshot(
                        terminalStatus,
                        "Attempted terminal rewrite")));

            AssertSnapshotUnchanged(
                before,
                session.Progress);
        }

        private static void AdvanceToTerminal(
            LaunchSession session,
            LaunchStatus terminalStatus)
        {
            if (terminalStatus == LaunchStatus.Failed ||
                terminalStatus == LaunchStatus.Interrupted)
            {
                session.Publish(
                    CreateSnapshot(
                        terminalStatus,
                        terminalStatus.ToString()));

                return;
            }

            session.Publish(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating"));

            session.Publish(
                CreateSnapshot(
                    LaunchStatus.Running,
                    "Running"));

            session.Publish(
                CreateSnapshot(
                    LaunchStatus.Transitioning,
                    "Transitioning"));

            session.Publish(
                CreateSnapshot(
                    LaunchStatus.Completed,
                    "Completed"));
        }

        private static LaunchProgressSnapshot CreateSnapshot(
            LaunchStatus status,
            string message)
        {
            return new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                status,
                string.Empty,
                -1,
                0,
                status == LaunchStatus.Completed
                    ? 1f
                    : 0f,
                true,
                message,
                0d,
                null);
        }

        private static void AssertSnapshotUnchanged(
            LaunchProgressSnapshot expected,
            LaunchProgressSnapshot actual)
        {
            Assert.That(
                actual.Mode,
                Is.EqualTo(expected.Mode));

            Assert.That(
                actual.Status,
                Is.EqualTo(expected.Status));

            Assert.That(
                actual.ActiveStepId,
                Is.EqualTo(expected.ActiveStepId));

            Assert.That(
                actual.ActiveStepIndex,
                Is.EqualTo(expected.ActiveStepIndex));

            Assert.That(
                actual.TotalStepCount,
                Is.EqualTo(expected.TotalStepCount));

            Assert.That(
                actual.Progress01,
                Is.EqualTo(expected.Progress01));

            Assert.That(
                actual.IsProgressIndeterminate,
                Is.EqualTo(
                    expected.IsProgressIndeterminate));

            Assert.That(
                actual.Message,
                Is.EqualTo(expected.Message));

            Assert.That(
                actual.ElapsedSeconds,
                Is.EqualTo(expected.ElapsedSeconds));

            Assert.That(
                actual.LastResult,
                Is.SameAs(expected.LastResult));
        }
    }
}

//----- LaunchLifecycleTransitionTests.cs END -----
