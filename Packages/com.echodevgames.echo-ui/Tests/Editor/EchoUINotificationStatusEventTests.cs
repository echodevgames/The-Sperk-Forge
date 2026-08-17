using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationStatusEventTests
    {
        private sealed class FakeClock :
            IUINotificationClock
        {
            public double NowSeconds { get; set; }
        }

        private readonly List<GameObject> owners =
            new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            for (int index = owners.Count - 1;
                 index >= 0;
                 index--)
            {
                if (owners[index] != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        owners[index]);
                }
            }

            owners.Clear();
        }

        [Test]
        public void VisibleAdmissionPublishesCommittedChannelTruth()
        {
            UINotificationService service =
                CreateService();

            List<UINotificationChannelSnapshot> observed =
                Observe(service);

            UINotificationHandle handle =
                Admit(
                    service,
                    PrimaryChannel,
                    "visible");

            Assert.That(observed.Count, Is.EqualTo(1));

            AssertSnapshot(
                observed[0],
                PrimaryChannel,
                visible: 1,
                pending: 0);

            AssertState(
                service,
                handle,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void AdmissionsPublishIndependentlyPerAffectedChannel()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            List<UINotificationChannelSnapshot> observed =
                Observe(service);

            Admit(
                service,
                PrimaryChannel,
                "primary-visible");

            Admit(
                service,
                PrimaryChannel,
                "primary-pending");

            Admit(
                service,
                SecondaryChannel,
                "secondary-visible");

            Assert.That(observed.Count, Is.EqualTo(3));

            AssertSnapshot(
                observed[0],
                PrimaryChannel,
                visible: 1,
                pending: 0);

            AssertSnapshot(
                observed[1],
                PrimaryChannel,
                visible: 1,
                pending: 1);

            AssertSnapshot(
                observed[2],
                SecondaryChannel,
                visible: 1,
                pending: 0);
        }

        [Test]
        public void ReadsAndIdleAdvancementPublishNothing()
        {
            UINotificationService service =
                CreateService();

            List<UINotificationChannelSnapshot> observed =
                Observe(service);

            Assert.That(
                service.TryGetDefinition(
                    PrimaryChannel,
                    out _),
                Is.True);

            Assert.That(
                service.TryGetSnapshot(
                    PrimaryChannel,
                    out _),
                Is.True);

            Assert.That(service.Tick(), Is.EqualTo(0));

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(0));

            Assert.That(service.Reset(), Is.EqualTo(0));
            Assert.That(observed, Is.Empty);
        }

        [Test]
        public void RejectionsPublishNothingAndPreserveTruth()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 0);

            UINotificationHandle retained =
                Admit(
                    service,
                    PrimaryChannel,
                    "retained");

            List<UINotificationChannelSnapshot> observed =
                Observe(service);

            UINotificationHandle unknown =
                service.Admit(
                    new UINotificationRequest(
                        "notification.unknown",
                        "unknown"));

            UINotificationHandle full =
                service.Admit(
                    new UINotificationRequest(
                        PrimaryChannel,
                        "full"));

            Assert.That(unknown.Accepted, Is.False);
            Assert.That(full.Accepted, Is.False);

            Assert.That(
                full.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.CapacityExceeded));

            Assert.That(
                service.Dismiss(null).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Invalid));

            Assert.That(observed, Is.Empty);

            AssertState(
                service,
                retained,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void CoalescingPublishesAfterPriorGenerationSettles()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle prior =
                Admit(
                    service,
                    PrimaryChannel,
                    "prior",
                    coalescingKey: "stable-key");

            int eventCount = 0;
            bool priorSettledAtEvent = false;
            UINotificationChannelSnapshot observed =
                default;

            service.ChannelChanged += snapshot =>
            {
                eventCount++;
                priorSettledAtEvent =
                    prior.IsCompleted;
                observed = snapshot;
            };

            UINotificationHandle replacement =
                Admit(
                    service,
                    PrimaryChannel,
                    "replacement",
                    coalescingKey: "stable-key");

            Assert.That(eventCount, Is.EqualTo(1));
            Assert.That(priorSettledAtEvent, Is.True);

            Assert.That(
                prior.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Superseded));

            AssertSnapshot(
                observed,
                PrimaryChannel,
                visible: 1,
                pending: 0);

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void OverflowReplacementPublishesAfterVictimSettles()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1,
                    overflowPolicy:
                        UINotificationOverflowPolicy.DropOldestPending);

            Admit(
                service,
                PrimaryChannel,
                "visible");

            UINotificationHandle victim =
                Admit(
                    service,
                    PrimaryChannel,
                    "victim");

            int eventCount = 0;
            bool victimSettledAtEvent = false;
            UINotificationChannelSnapshot observed =
                default;

            service.ChannelChanged += snapshot =>
            {
                eventCount++;
                victimSettledAtEvent =
                    victim.IsCompleted;
                observed = snapshot;
            };

            UINotificationHandle replacement =
                Admit(
                    service,
                    PrimaryChannel,
                    "replacement");

            Assert.That(eventCount, Is.EqualTo(1));
            Assert.That(victimSettledAtEvent, Is.True);

            Assert.That(
                victim.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.OverflowEvicted));

            AssertSnapshot(
                observed,
                PrimaryChannel,
                visible: 1,
                pending: 1);

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Pending);
        }

        [Test]
        public void DismissalPublishesAfterDeterministicPromotion()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(
                    service,
                    PrimaryChannel,
                    "visible");

            UINotificationHandle lower =
                Admit(
                    service,
                    PrimaryChannel,
                    "lower",
                    priority: 1);

            UINotificationHandle higher =
                Admit(
                    service,
                    PrimaryChannel,
                    "higher",
                    priority: 10);

            List<UINotificationChannelSnapshot> observed =
                Observe(service);

            Assert.That(
                service.Dismiss(visible).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            Assert.That(observed.Count, Is.EqualTo(1));

            AssertSnapshot(
                observed[0],
                PrimaryChannel,
                visible: 1,
                pending: 1);

            Assert.That(visible.IsCompleted, Is.True);

            AssertState(
                service,
                higher,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                lower,
                UINotificationEntryState.Pending);
        }

        [Test]
        public void TickBatchesOneFinalEventPerChangedChannel()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    visibleCapacity: 2,
                    pendingCapacity: 2);

            UINotificationHandle firstExpired =
                Admit(
                    service,
                    PrimaryChannel,
                    "first-expired",
                    lifetimeMode:
                        UINotificationLifetimeMode.Automatic,
                    durationSeconds: 1f);

            UINotificationHandle secondExpired =
                Admit(
                    service,
                    PrimaryChannel,
                    "second-expired",
                    lifetimeMode:
                        UINotificationLifetimeMode.Automatic,
                    durationSeconds: 1f);

            UINotificationHandle lower =
                Admit(
                    service,
                    PrimaryChannel,
                    "lower",
                    priority: 1);

            UINotificationHandle higher =
                Admit(
                    service,
                    PrimaryChannel,
                    "higher",
                    priority: 10);

            List<UINotificationChannelSnapshot> observed =
                Observe(service);

            clock.NowSeconds = 1d;

            Assert.That(service.Tick(), Is.EqualTo(2));
            Assert.That(observed.Count, Is.EqualTo(1));

            AssertSnapshot(
                observed[0],
                PrimaryChannel,
                visible: 2,
                pending: 0);

            Assert.That(firstExpired.IsCompleted, Is.True);
            Assert.That(secondExpired.IsCompleted, Is.True);

            AssertState(
                service,
                higher,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                lower,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void OwnerRefreshPublishesOnlyAffectedChannel()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            GameObject owner =
                CreateOwner("notification-owner");

            UINotificationHandle lost =
                Admit(
                    service,
                    PrimaryChannel,
                    "lost",
                    owner: owner);

            UINotificationHandle promoted =
                Admit(
                    service,
                    PrimaryChannel,
                    "promoted");

            UINotificationHandle secondary =
                Admit(
                    service,
                    SecondaryChannel,
                    "secondary");

            List<UINotificationChannelSnapshot> observed =
                Observe(service);

            UnityEngine.Object.DestroyImmediate(owner);

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(1));

            Assert.That(observed.Count, Is.EqualTo(1));

            AssertSnapshot(
                observed[0],
                PrimaryChannel,
                visible: 1,
                pending: 0);

            Assert.That(lost.IsCompleted, Is.True);

            AssertState(
                service,
                promoted,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                secondary,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void ResetPublishesEmptyTruthForChangedChannels()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            UINotificationHandle[] handles =
            {
                Admit(
                    service,
                    PrimaryChannel,
                    "primary-visible"),
                Admit(
                    service,
                    PrimaryChannel,
                    "primary-pending"),
                Admit(
                    service,
                    SecondaryChannel,
                    "secondary-visible")
            };

            bool allSettledAtEvent = false;
            List<UINotificationChannelSnapshot> observed =
                new List<UINotificationChannelSnapshot>();

            service.ChannelChanged += snapshot =>
            {
                allSettledAtEvent =
                    handles[0].IsCompleted &&
                    handles[1].IsCompleted &&
                    handles[2].IsCompleted;

                observed.Add(snapshot);
            };

            Assert.That(service.Reset(), Is.EqualTo(3));
            Assert.That(allSettledAtEvent, Is.True);
            Assert.That(observed.Count, Is.EqualTo(2));

            AssertObservedSnapshot(
                observed,
                PrimaryChannel,
                visible: 0,
                pending: 0);

            AssertObservedSnapshot(
                observed,
                SecondaryChannel,
                visible: 0,
                pending: 0);
        }

        [Test]
        public void ShutdownPublishesFinalEmptyTruthForEveryChannel()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle live =
                Admit(
                    service,
                    PrimaryChannel,
                    "live");

            bool finalStateObserved = false;
            List<UINotificationChannelSnapshot> observed =
                new List<UINotificationChannelSnapshot>();

            service.ChannelChanged += snapshot =>
            {
                finalStateObserved =
                    service.IsShutdown &&
                    !service.IsValid &&
                    service.ChannelCount == 0 &&
                    live.IsCompleted;

                observed.Add(snapshot);
            };

            Assert.That(service.Shutdown(), Is.EqualTo(1));
            Assert.That(finalStateObserved, Is.True);
            Assert.That(observed.Count, Is.EqualTo(2));

            AssertObservedSnapshot(
                observed,
                PrimaryChannel,
                visible: 0,
                pending: 0);

            AssertObservedSnapshot(
                observed,
                SecondaryChannel,
                visible: 0,
                pending: 0);

            Assert.That(
                service.TryGetSnapshot(
                    PrimaryChannel,
                    out _),
                Is.False);
        }

        [Test]
        public void ListenerFailureCannotRollbackOrBlockLaterListeners()
        {
            UINotificationService service =
                CreateService();

            int healthyListenerCount = 0;
            UINotificationChannelSnapshot observed =
                default;

            service.ChannelChanged += _ =>
                throw new InvalidOperationException(
                    "notification-observer");

            service.ChannelChanged += snapshot =>
            {
                healthyListenerCount++;
                observed = snapshot;
            };

            LogAssert.Expect(
                LogType.Exception,
                new Regex(
                    "InvalidOperationException: notification-observer"));

            UINotificationHandle handle =
                Admit(
                    service,
                    PrimaryChannel,
                    "committed");

            Assert.That(healthyListenerCount, Is.EqualTo(1));

            AssertSnapshot(
                observed,
                PrimaryChannel,
                visible: 1,
                pending: 0);

            AssertState(
                service,
                handle,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void EventCallbackCannotReenterCommittedMutation()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            UINotificationHandle visible =
                Admit(
                    service,
                    PrimaryChannel,
                    "visible");

            UINotificationHandle promoted =
                Admit(
                    service,
                    PrimaryChannel,
                    "promoted");

            int eventCount = 0;
            UINotificationOperationStatus settledStatus =
                default;

            UINotificationOperationStatus promotedStatus =
                default;

            UINotificationHandle reentrant =
                null;

            service.ChannelChanged += _ =>
            {
                eventCount++;

                settledStatus =
                    service.Dismiss(visible).Status;

                promotedStatus =
                    service.Dismiss(promoted).Status;

                reentrant =
                    service.Admit(
                        new UINotificationRequest(
                            PrimaryChannel,
                            "reentrant"));
            };

            Assert.That(
                service.Dismiss(visible).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            Assert.That(eventCount, Is.EqualTo(1));

            Assert.That(
                settledStatus,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            Assert.That(
                promotedStatus,
                Is.EqualTo(
                    UINotificationOperationStatus.Unavailable));

            Assert.That(reentrant.Accepted, Is.False);

            Assert.That(
                reentrant.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Unavailable));

            AssertState(
                service,
                promoted,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void ChannelSnapshotPublicSurfaceIsPayloadFree()
        {
            PropertyInfo[] properties =
                typeof(UINotificationChannelSnapshot)
                    .GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public);

            List<string> names =
                new List<string>();

            for (int index = 0;
                 index < properties.Length;
                 index++)
            {
                names.Add(
                    properties[index].Name);
            }

            names.Sort(
                StringComparer.Ordinal);

            CollectionAssert.AreEqual(
                new[]
                {
                    "ChannelId",
                    "OverflowPolicy",
                    "PendingCapacity",
                    "PendingCount",
                    "VisibleCapacity",
                    "VisibleCount"
                },
                names);
        }

        [Test]
        public void SubscriptionHasNoReplayAndCanBeReleased()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle handle =
                Admit(
                    service,
                    PrimaryChannel,
                    "before-subscription");

            List<UINotificationChannelSnapshot> observed =
                new List<UINotificationChannelSnapshot>();

            Action<UINotificationChannelSnapshot> listener =
                observed.Add;

            service.ChannelChanged += listener;

            Assert.That(observed, Is.Empty);

            Assert.That(
                service.Dismiss(handle).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            Assert.That(observed.Count, Is.EqualTo(1));

            service.ChannelChanged -= listener;

            Admit(
                service,
                PrimaryChannel,
                "after-release");

            Assert.That(observed.Count, Is.EqualTo(1));
        }

        private const string PrimaryChannel =
            "notification.primary";

        private const string SecondaryChannel =
            "notification.secondary";

        private GameObject CreateOwner(
            string name)
        {
            GameObject owner =
                new GameObject(name);

            owners.Add(owner);
            return owner;
        }

        private static UINotificationService CreateService(
            FakeClock clock = null,
            int visibleCapacity = 1,
            int pendingCapacity = 2,
            UINotificationOverflowPolicy overflowPolicy =
                UINotificationOverflowPolicy.RejectNewest)
        {
            if (clock == null)
            {
                clock = new FakeClock();
            }

            UINotificationService service =
                new UINotificationService(
                    new[]
                    {
                        new UINotificationChannelDefinition(
                            PrimaryChannel,
                            visibleCapacity,
                            pendingCapacity,
                            overflowPolicy: overflowPolicy),
                        new UINotificationChannelDefinition(
                            SecondaryChannel,
                            visibleCapacity,
                            pendingCapacity,
                            overflowPolicy: overflowPolicy)
                    },
                    clock,
                    out string validationError);

            Assert.That(validationError, Is.Empty);
            return service;
        }

        private static List<UINotificationChannelSnapshot> Observe(
            UINotificationService service)
        {
            List<UINotificationChannelSnapshot> observed =
                new List<UINotificationChannelSnapshot>();

            service.ChannelChanged += observed.Add;
            return observed;
        }

        private static UINotificationHandle Admit(
            UINotificationService service,
            string channelId,
            string presentation,
            int priority = 0,
            UINotificationLifetimeMode lifetimeMode =
                UINotificationLifetimeMode.Manual,
            float durationSeconds = 0f,
            string coalescingKey = "",
            UnityEngine.Object owner = null)
        {
            UINotificationHandle handle =
                service.Admit(
                    new UINotificationRequest(
                        channelId,
                        presentation,
                        priority,
                        lifetimeMode,
                        durationSeconds,
                        coalescingKey,
                        owner));

            Assert.That(handle.Accepted, Is.True);
            return handle;
        }

        private static void AssertSnapshot(
            UINotificationChannelSnapshot snapshot,
            string channelId,
            int visible,
            int pending)
        {
            Assert.That(
                snapshot.ChannelId,
                Is.EqualTo(
                    new UINotificationChannelId(channelId)));

            Assert.That(snapshot.VisibleCount, Is.EqualTo(visible));
            Assert.That(snapshot.PendingCount, Is.EqualTo(pending));
        }

        private static void AssertState(
            UINotificationService service,
            UINotificationHandle handle,
            UINotificationEntryState expected)
        {
            Assert.That(
                service.TryGetEntryState(
                    handle,
                    out UINotificationEntryState state),
                Is.True);

            Assert.That(state, Is.EqualTo(expected));
        }

        private static void AssertObservedSnapshot(
            List<UINotificationChannelSnapshot> observed,
            string channelId,
            int visible,
            int pending)
        {
            for (int index = 0;
                 index < observed.Count;
                 index++)
            {
                if (observed[index].ChannelId ==
                    new UINotificationChannelId(channelId))
                {
                    AssertSnapshot(
                        observed[index],
                        channelId,
                        visible,
                        pending);

                    return;
                }
            }

            Assert.Fail(
                "Expected channel snapshot was not observed: " +
                channelId);
        }
    }
}
