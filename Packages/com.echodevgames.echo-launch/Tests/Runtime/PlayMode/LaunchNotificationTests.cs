//----- LaunchNotificationTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class LaunchNotificationTests
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
        public void AcceptedLifecycleChangeRaisesStateEventOnce()
        {
            EchoLaunchRoot root =
                CreateRoot("State Event Root");

            int callCount = 0;

            root.LaunchStateChanged +=
                _ => callCount++;

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));

            Assert.That(
                callCount,
                Is.EqualTo(1));
        }

        [Test]
        public void AcceptedLifecycleChangeRaisesProgressEventOnce()
        {
            EchoLaunchRoot root =
                CreateRoot("Progress Event Root");

            int callCount = 0;

            root.LaunchProgressChanged +=
                _ => callCount++;

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));

            Assert.That(
                callCount,
                Is.EqualTo(1));
        }

        [Test]
        public void SameStatePublicationRaisesOnlyProgressEvent()
        {
            EchoLaunchRoot root =
                CreateRoot("Same State Root");

            int stateCalls = 0;
            int progressCalls = 0;

            root.LaunchStateChanged +=
                _ => stateCalls++;

            root.LaunchProgressChanged +=
                _ => progressCalls++;

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.AuthorityClaimed,
                    "Authority confirmed.",
                    progress01: 0.25f));

            Assert.That(
                stateCalls,
                Is.EqualTo(0));

            Assert.That(
                progressCalls,
                Is.EqualTo(1));
        }

        [Test]
        public void StateEventOccursBeforeProgressEvent()
        {
            EchoLaunchRoot root =
                CreateRoot("Ordered Event Root");

            List<string> order =
                new List<string>();

            root.LaunchStateChanged +=
                _ => order.Add("state");

            root.LaunchProgressChanged +=
                _ => order.Add("progress");

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));

            Assert.That(
                order,
                Is.EqualTo(
                    new[]
                    {
                        "state",
                        "progress"
                    }));
        }

        [Test]
        public void CallbacksObserveAcceptedRootStateAndProgress()
        {
            EchoLaunchRoot root =
                CreateRoot("Accepted State Root");

            LaunchStatus stateObservedByStateListener =
                LaunchStatus.None;

            LaunchStatus stateObservedByProgressListener =
                LaunchStatus.None;

            string messageObservedByStateListener =
                string.Empty;

            string messageObservedByProgressListener =
                string.Empty;

            root.LaunchStateChanged +=
                _ =>
                {
                    stateObservedByStateListener =
                        root.State;

                    messageObservedByStateListener =
                        root.Progress.Message;
                };

            root.LaunchProgressChanged +=
                _ =>
                {
                    stateObservedByProgressListener =
                        root.State;

                    messageObservedByProgressListener =
                        root.Progress.Message;
                };

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Accepted before notification."));

            Assert.That(
                stateObservedByStateListener,
                Is.EqualTo(LaunchStatus.Validating));

            Assert.That(
                stateObservedByProgressListener,
                Is.EqualTo(LaunchStatus.Validating));

            Assert.That(
                messageObservedByStateListener,
                Is.EqualTo(
                    "Accepted before notification."));

            Assert.That(
                messageObservedByProgressListener,
                Is.EqualTo(
                    "Accepted before notification."));
        }

        [Test]
        public void StatePayloadContainsPreviousAndCurrentValues()
        {
            EchoLaunchRoot root =
                CreateRoot("State Payload Root");

            LaunchStateChangedEvent received =
                default;

            int callCount = 0;

            root.LaunchStateChanged +=
                payload =>
                {
                    received = payload;
                    callCount++;
                };

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));

            Assert.That(
                callCount,
                Is.EqualTo(1));

            Assert.That(
                received.PreviousState,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            Assert.That(
                received.CurrentState,
                Is.EqualTo(
                    LaunchStatus.Validating));

            Assert.That(
                received.Progress.Status,
                Is.EqualTo(
                    LaunchStatus.Validating));

            Assert.That(
                received.Progress.Message,
                Is.EqualTo("Validating."));
        }

        [Test]
        public void ProgressPayloadContainsPreviousAndCurrentSnapshots()
        {
            EchoLaunchRoot root =
                CreateRoot("Progress Payload Root");

            LaunchProgressChangedEvent received =
                default;

            int callCount = 0;

            root.LaunchProgressChanged +=
                payload =>
                {
                    received = payload;
                    callCount++;
                };

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating.",
                    progress01: 0.25f));

            Assert.That(
                callCount,
                Is.EqualTo(1));

            Assert.That(
                received.Previous.Status,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            Assert.That(
                received.Previous.Message,
                Is.EqualTo(
                    "Launch authority claimed."));

            Assert.That(
                received.Current.Status,
                Is.EqualTo(
                    LaunchStatus.Validating));

            Assert.That(
                received.Current.Progress01,
                Is.EqualTo(0.25f));

            Assert.That(
                received.Current.Message,
                Is.EqualTo("Validating."));
        }

        [Test]
        public void InvalidTransitionRaisesNoNotifications()
        {
            EchoLaunchRoot root =
                CreateRoot("Invalid Transition Root");

            int stateCalls = 0;
            int progressCalls = 0;

            root.LaunchStateChanged +=
                _ => stateCalls++;

            root.LaunchProgressChanged +=
                _ => progressCalls++;

            Assert.Throws<InvalidOperationException>(
                () => root.PublishProgress(
                    CreateSnapshot(
                        LaunchStatus.Running,
                        "Skipped validation.")));

            Assert.That(
                stateCalls,
                Is.EqualTo(0));

            Assert.That(
                progressCalls,
                Is.EqualTo(0));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));
        }

        [Test]
        public void ModeMismatchRaisesNoNotifications()
        {
            EchoLaunchRoot root =
                CreateRoot("Mode Mismatch Root");

            int stateCalls = 0;
            int progressCalls = 0;

            root.LaunchStateChanged +=
                _ => stateCalls++;

            root.LaunchProgressChanged +=
                _ => progressCalls++;

            LaunchProgressSnapshot mismatched =
                new LaunchProgressSnapshot(
                    LaunchMode.DirectSceneDevelopment,
                    LaunchStatus.Validating,
                    string.Empty,
                    -1,
                    0,
                    0f,
                    true,
                    "Wrong mode.",
                    0d,
                    null);

            Assert.Throws<ArgumentException>(
                () => root.PublishProgress(
                    mismatched));

            Assert.That(
                stateCalls,
                Is.EqualTo(0));

            Assert.That(
                progressCalls,
                Is.EqualTo(0));
        }

        [Test]
        public void TerminalRewriteRaisesNoNotifications()
        {
            EchoLaunchRoot root =
                CreateRoot("Terminal Root");

            AdvanceToCompleted(root);

            int stateCalls = 0;
            int progressCalls = 0;

            root.LaunchStateChanged +=
                _ => stateCalls++;

            root.LaunchProgressChanged +=
                _ => progressCalls++;

            Assert.Throws<InvalidOperationException>(
                () => root.PublishProgress(
                    CreateSnapshot(
                        LaunchStatus.Completed,
                        "Attempted rewrite.",
                        progress01: 1f)));

            Assert.That(
                stateCalls,
                Is.EqualTo(0));

            Assert.That(
                progressCalls,
                Is.EqualTo(0));

            Assert.That(
                root.Progress.Message,
                Is.EqualTo("Completed."));
        }

        [Test]
        public void UnsubscribedListenerIsNotInvoked()
        {
            EchoLaunchRoot root =
                CreateRoot("Unsubscribe Root");

            int callCount = 0;

            Action<LaunchProgressChangedEvent> listener =
                _ => callCount++;

            root.LaunchProgressChanged += listener;
            root.LaunchProgressChanged -= listener;

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));

            Assert.That(
                callCount,
                Is.EqualTo(0));
        }

        [Test]
        public void FailingStateListenerDoesNotPreventLaterStateListener()
        {
            EchoLaunchRoot root =
                CreateRoot("State Isolation Root");

            int laterCalls = 0;

            ExpectListenerFailure(
                "LaunchStateChanged",
                "state boom");

            root.LaunchStateChanged +=
                _ => throw new InvalidOperationException(
                    "state boom");

            root.LaunchStateChanged +=
                _ => laterCalls++;

            Assert.DoesNotThrow(
                () => root.PublishProgress(
                    CreateSnapshot(
                        LaunchStatus.Validating,
                        "Validating.")));

            Assert.That(
                laterCalls,
                Is.EqualTo(1));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Validating));
        }

        [Test]
        public void FailingStateListenerDoesNotPreventProgressNotification()
        {
            EchoLaunchRoot root =
                CreateRoot("Cross Event Isolation Root");

            int progressCalls = 0;

            ExpectListenerFailure(
                "LaunchStateChanged",
                "state failed");

            root.LaunchStateChanged +=
                _ => throw new InvalidOperationException(
                    "state failed");

            root.LaunchProgressChanged +=
                _ => progressCalls++;

            Assert.DoesNotThrow(
                () => root.PublishProgress(
                    CreateSnapshot(
                        LaunchStatus.Validating,
                        "Validating.")));

            Assert.That(
                progressCalls,
                Is.EqualTo(1));
        }

        [Test]
        public void FailingProgressListenerDoesNotPreventLaterProgressListener()
        {
            EchoLaunchRoot root =
                CreateRoot("Progress Isolation Root");

            int laterCalls = 0;

            ExpectListenerFailure(
                "LaunchProgressChanged",
                "progress boom");

            root.LaunchProgressChanged +=
                _ => throw new InvalidOperationException(
                    "progress boom");

            root.LaunchProgressChanged +=
                _ => laterCalls++;

            Assert.DoesNotThrow(
                () => root.PublishProgress(
                    CreateSnapshot(
                        LaunchStatus.Validating,
                        "Validating.")));

            Assert.That(
                laterCalls,
                Is.EqualTo(1));

            Assert.That(
                root.Progress.Message,
                Is.EqualTo("Validating."));
        }

        [Test]
        public void StateListenerFailureLogsStableDiagnostic()
        {
            EchoLaunchRoot root =
                CreateRoot("State Diagnostic Root");

            ExpectListenerFailure(
                "LaunchStateChanged",
                "diagnostic state failure");

            root.LaunchStateChanged +=
                _ => throw new InvalidOperationException(
                    "diagnostic state failure");

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));
        }

        [Test]
        public void ProgressListenerFailureLogsStableDiagnostic()
        {
            EchoLaunchRoot root =
                CreateRoot("Progress Diagnostic Root");

            ExpectListenerFailure(
                "LaunchProgressChanged",
                "diagnostic progress failure");

            root.LaunchProgressChanged +=
                _ => throw new InvalidOperationException(
                    "diagnostic progress failure");

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));
        }

        [Test]
        public void DuplicatePublicationAttemptRaisesNoNotifications()
        {
            EchoLaunchRoot authority =
                CreateRoot("Authority");

            ExpectDuplicateWarning();

            EchoLaunchRoot duplicate =
                CreateRoot("Duplicate");

            int duplicateStateCalls = 0;
            int duplicateProgressCalls = 0;

            duplicate.LaunchStateChanged +=
                _ => duplicateStateCalls++;

            duplicate.LaunchProgressChanged +=
                _ => duplicateProgressCalls++;

            Assert.Throws<InvalidOperationException>(
                () => duplicate.PublishProgress(
                    CreateSnapshot(
                        LaunchStatus.Validating,
                        "Forged progress.")));

            Assert.That(
                duplicateStateCalls,
                Is.EqualTo(0));

            Assert.That(
                duplicateProgressCalls,
                Is.EqualTo(0));

            Assert.That(
                authority.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));
        }

        [Test]
        public void NoListenersPublicationSucceedsNormally()
        {
            EchoLaunchRoot root =
                CreateRoot("No Listener Root");

            Assert.DoesNotThrow(
                () => root.PublishProgress(
                    CreateSnapshot(
                        LaunchStatus.Validating,
                        "Validating.")));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Validating));
        }

        [Test]
        public void DestroyedRootSubscriptionsDoNotTransfer()
        {
            EchoLaunchRoot first =
                CreateRoot("First Root");

            int oldListenerCalls = 0;

            first.LaunchProgressChanged +=
                _ => oldListenerCalls++;

            Object.DestroyImmediate(
                first.gameObject);

            EchoLaunchRoot second =
                CreateRoot("Second Root");

            second.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));

            Assert.That(
                oldListenerCalls,
                Is.EqualTo(0));

            Assert.That(
                second.State,
                Is.EqualTo(
                    LaunchStatus.Validating));
        }

        [Test]
        public void DestroyedRootClearsEventDelegateFields()
        {
            EchoLaunchRoot root =
                CreateRoot("Cleared Subscription Root");

            root.LaunchStateChanged +=
                _ => { };

            root.LaunchProgressChanged +=
                _ => { };

            FieldInfo stateField =
                typeof(EchoLaunchRoot).GetField(
                    "LaunchStateChanged",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            FieldInfo progressField =
                typeof(EchoLaunchRoot).GetField(
                    "LaunchProgressChanged",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.That(
                stateField,
                Is.Not.Null);

            Assert.That(
                progressField,
                Is.Not.Null);

            object managedRoot = root;

            Assert.That(
                stateField.GetValue(managedRoot),
                Is.Not.Null);

            Assert.That(
                progressField.GetValue(managedRoot),
                Is.Not.Null);

            Object.DestroyImmediate(
                root.gameObject);

            Assert.That(
                stateField.GetValue(managedRoot),
                Is.Null);

            Assert.That(
                progressField.GetValue(managedRoot),
                Is.Null);
        }

        private EchoLaunchRoot CreateRoot(
            string name)
        {
            GameObject target =
                new GameObject(name);

            createdObjects.Add(target);

            return target.AddComponent<EchoLaunchRoot>();
        }

        private static LaunchProgressSnapshot CreateSnapshot(
            LaunchStatus status,
            string message,
            float progress01 = 0f)
        {
            return new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                status,
                string.Empty,
                -1,
                0,
                progress01,
                true,
                message,
                0d,
                null);
        }

        private static void AdvanceToCompleted(
            EchoLaunchRoot root)
        {
            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating."));

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Running,
                    "Running."));

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Transitioning,
                    "Transitioning.",
                    progress01: 1f));

            root.PublishProgress(
                CreateSnapshot(
                    LaunchStatus.Completed,
                    "Completed.",
                    progress01: 1f));
        }

        private static void ExpectListenerFailure(
            string eventName,
            string message)
        {
            string pattern =
                @"\[ELAUNCH-EVENT-001\]" +
                @".*" +
                Regex.Escape(eventName) +
                @".*InvalidOperationException.*" +
                Regex.Escape(message);

            LogAssert.Expect(
                LogType.Warning,
                new Regex(pattern));
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

//----- LaunchNotificationTests.cs END -----
