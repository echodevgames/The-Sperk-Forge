using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationLifecycleTests
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
                    Object.DestroyImmediate(
                        owners[index]);
                }
            }

            owners.Clear();
        }

        [Test]
        public void DestroyedVisibleOwnerSettlesAndStartsPromotedLifetime()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    pendingCapacity: 2);

            GameObject owner =
                CreateOwner("visible-owner");

            UINotificationHandle lost =
                Admit(
                    service,
                    PrimaryChannel,
                    "lost",
                    owner: owner);

            UINotificationHandle lower =
                Admit(
                    service,
                    PrimaryChannel,
                    "lower",
                    priority: 1);

            UINotificationHandle winner =
                Admit(
                    service,
                    PrimaryChannel,
                    "winner",
                    priority: 5,
                    lifetimeMode:
                        UINotificationLifetimeMode.Automatic,
                    durationSeconds: 2f);

            clock.NowSeconds = 100d;
            Object.DestroyImmediate(owner);

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(1));

            AssertOutcome(
                lost,
                UINotificationOutcome.OwnerLost);

            AssertState(
                service,
                winner,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                lower,
                UINotificationEntryState.Pending);

            clock.NowSeconds = 101.999d;
            Assert.That(service.Tick(), Is.EqualTo(0));

            clock.NowSeconds = 102d;
            Assert.That(service.Tick(), Is.EqualTo(1));

            AssertOutcome(
                winner,
                UINotificationOutcome.Expired);

            AssertState(
                service,
                lower,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void DestroyedPendingOwnerLeavesVisibleTruthUnchanged()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(
                    service,
                    PrimaryChannel,
                    "visible");

            GameObject owner =
                CreateOwner("pending-owner");

            UINotificationHandle lost =
                Admit(
                    service,
                    PrimaryChannel,
                    "lost-pending",
                    priority: 20,
                    owner: owner);

            UINotificationHandle retained =
                Admit(
                    service,
                    PrimaryChannel,
                    "retained-pending",
                    priority: 1);

            Object.DestroyImmediate(owner);

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(1));

            AssertOutcome(
                lost,
                UINotificationOutcome.OwnerLost);

            AssertState(
                service,
                visible,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                retained,
                UINotificationEntryState.Pending);

            AssertCounts(
                service,
                visible: 1,
                pending: 1);
        }

        [Test]
        public void LiveAndUnownedGenerationsSurviveOwnerRefresh()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 2,
                    pendingCapacity: 0);

            GameObject owner =
                CreateOwner("live-owner");

            UINotificationHandle owned =
                Admit(
                    service,
                    PrimaryChannel,
                    "owned",
                    owner: owner);

            UINotificationHandle unowned =
                Admit(
                    service,
                    PrimaryChannel,
                    "unowned");

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(0));

            AssertState(
                service,
                owned,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                unowned,
                UINotificationEntryState.Visible);

            AssertCounts(
                service,
                visible: 2,
                pending: 0);
        }

        [Test]
        public void OwnerRefreshPreservesChannelIsolation()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            GameObject owner =
                CreateOwner("primary-owner");

            UINotificationHandle lost =
                Admit(
                    service,
                    PrimaryChannel,
                    "primary-lost",
                    owner: owner);

            UINotificationHandle primaryPending =
                Admit(
                    service,
                    PrimaryChannel,
                    "primary-pending");

            UINotificationHandle secondaryVisible =
                Admit(
                    service,
                    SecondaryChannel,
                    "secondary-visible");

            UINotificationHandle secondaryPending =
                Admit(
                    service,
                    SecondaryChannel,
                    "secondary-pending");

            Object.DestroyImmediate(owner);

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(1));

            AssertOutcome(
                lost,
                UINotificationOutcome.OwnerLost);

            AssertState(
                service,
                primaryPending,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                secondaryVisible,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                secondaryPending,
                UINotificationEntryState.Pending);
        }

        [Test]
        public void SupersededOwnedGenerationCannotRemoveReplacement()
        {
            UINotificationService service =
                CreateService();

            GameObject owner =
                CreateOwner("superseded-owner");

            UINotificationHandle prior =
                Admit(
                    service,
                    PrimaryChannel,
                    "prior",
                    coalescingKey: "stable-key",
                    owner: owner);

            UINotificationHandle replacement =
                Admit(
                    service,
                    PrimaryChannel,
                    "replacement",
                    coalescingKey: "stable-key");

            AssertOutcome(
                prior,
                UINotificationOutcome.Superseded);

            Object.DestroyImmediate(owner);

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(0));

            Assert.That(replacement.IsCompleted, Is.False);

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);

            Assert.That(
                service.Dismiss(prior).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Stale));
        }

        [Test]
        public void OwnerLossSettlementCannotReenterMutation()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            GameObject owner =
                CreateOwner("callback-owner");

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

            UINotificationOperationStatus settledStatus =
                default;

            UINotificationHandle reentrant =
                null;

            UINotificationChannelSnapshot callbackSnapshot =
                default;

            bool hasCallbackSnapshot =
                false;

            lost.Completed += _ =>
            {
                settledStatus =
                    service.Dismiss(lost).Status;

                reentrant =
                    service.Admit(
                        new UINotificationRequest(
                            PrimaryChannel,
                            "reentrant"));

                hasCallbackSnapshot =
                    service.TryGetSnapshot(
                        PrimaryChannel,
                        out callbackSnapshot);
            };

            Object.DestroyImmediate(owner);

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(1));

            Assert.That(
                settledStatus,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            Assert.That(reentrant, Is.Not.Null);
            Assert.That(reentrant.Accepted, Is.False);

            Assert.That(
                reentrant.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Unavailable));

            Assert.That(hasCallbackSnapshot, Is.True);
            Assert.That(callbackSnapshot.VisibleCount, Is.EqualTo(1));
            Assert.That(callbackSnapshot.PendingCount, Is.EqualTo(0));

            AssertState(
                service,
                promoted,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void OwnerRefreshIsIdempotentAndSettlesExactlyOnce()
        {
            UINotificationService service =
                CreateService();

            GameObject owner =
                CreateOwner("idempotent-owner");

            UINotificationHandle handle =
                Admit(
                    service,
                    PrimaryChannel,
                    "owned",
                    owner: owner);

            int completionCount = 0;
            handle.Completed += _ => completionCount++;

            Object.DestroyImmediate(owner);

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(1));

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(0));

            Assert.That(completionCount, Is.EqualTo(1));

            AssertOutcome(
                handle,
                UINotificationOutcome.OwnerLost);

            AssertCounts(
                service,
                visible: 0,
                pending: 0);
        }

        [Test]
        public void ResetSettlesVisibleAndPendingAcrossChannels()
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
                    "secondary-visible"),
                Admit(
                    service,
                    SecondaryChannel,
                    "secondary-pending")
            };

            Assert.That(service.Reset(), Is.EqualTo(4));

            for (int index = 0;
                 index < handles.Length;
                 index++)
            {
                AssertOutcome(
                    handles[index],
                    UINotificationOutcome.Reset);
            }

            AssertCounts(
                service,
                visible: 0,
                pending: 0);

            AssertSnapshotCounts(
                service,
                PrimaryChannel,
                visible: 0,
                pending: 0);

            AssertSnapshotCounts(
                service,
                SecondaryChannel,
                visible: 0,
                pending: 0);
        }

        [Test]
        public void ResetIsIdempotentAndServiceReusesFreshGeneration()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle prior =
                Admit(
                    service,
                    PrimaryChannel,
                    "prior");

            Assert.That(service.Reset(), Is.EqualTo(1));
            Assert.That(service.Reset(), Is.EqualTo(0));

            UINotificationHandle replacement =
                Admit(
                    service,
                    PrimaryChannel,
                    "replacement");

            Assert.That(
                replacement.Generation,
                Is.GreaterThan(prior.Generation));

            Assert.That(
                service.Dismiss(prior).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);

            Assert.That(service.IsValid, Is.True);
            Assert.That(service.IsShutdown, Is.False);
        }

        [Test]
        public void ResetSettlementCannotReenterMutation()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            UINotificationHandle first =
                Admit(
                    service,
                    PrimaryChannel,
                    "first");

            UINotificationHandle second =
                Admit(
                    service,
                    PrimaryChannel,
                    "second");

            UINotificationOperationStatus firstStatus =
                default;

            UINotificationOperationStatus secondStatus =
                default;

            UINotificationHandle reentrant =
                null;

            UINotificationChannelSnapshot callbackSnapshot =
                default;

            first.Completed += _ =>
            {
                firstStatus =
                    service.Dismiss(first).Status;

                secondStatus =
                    service.Dismiss(second).Status;

                reentrant =
                    service.Admit(
                        new UINotificationRequest(
                            PrimaryChannel,
                            "reentrant"));

                service.TryGetSnapshot(
                    PrimaryChannel,
                    out callbackSnapshot);
            };

            Assert.That(service.Reset(), Is.EqualTo(2));

            Assert.That(
                firstStatus,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            Assert.That(
                secondStatus,
                Is.EqualTo(
                    UINotificationOperationStatus.Unavailable));

            Assert.That(reentrant.Accepted, Is.False);

            Assert.That(
                reentrant.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Unavailable));

            Assert.That(callbackSnapshot.VisibleCount, Is.EqualTo(0));
            Assert.That(callbackSnapshot.PendingCount, Is.EqualTo(0));

            AssertOutcome(
                second,
                UINotificationOutcome.Reset);

            UINotificationHandle afterReset =
                Admit(
                    service,
                    PrimaryChannel,
                    "after-reset");

            AssertState(
                service,
                afterReset,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void ShutdownSettlesAllAndReleasesChannelState()
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
                    "secondary-visible"),
                Admit(
                    service,
                    SecondaryChannel,
                    "secondary-pending")
            };

            Assert.That(service.Shutdown(), Is.EqualTo(4));

            for (int index = 0;
                 index < handles.Length;
                 index++)
            {
                AssertOutcome(
                    handles[index],
                    UINotificationOutcome.Shutdown);
            }

            Assert.That(service.IsShutdown, Is.True);
            Assert.That(service.IsValid, Is.False);
            Assert.That(service.ChannelCount, Is.EqualTo(0));

            AssertCounts(
                service,
                visible: 0,
                pending: 0);

            Assert.That(
                service.TryGetSnapshot(
                    PrimaryChannel,
                    out _),
                Is.False);
        }

        [Test]
        public void ShutdownIsIdempotentAndRejectsFurtherWork()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(clock);

            UINotificationHandle live =
                Admit(
                    service,
                    PrimaryChannel,
                    "live");

            Assert.That(service.Shutdown(), Is.EqualTo(1));
            Assert.That(service.Shutdown(), Is.EqualTo(0));
            Assert.That(service.Reset(), Is.EqualTo(0));

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(0));

            clock.NowSeconds = 100d;
            Assert.That(service.Tick(), Is.EqualTo(0));

            UINotificationHandle rejected =
                service.Admit(
                    new UINotificationRequest(
                        PrimaryChannel,
                        "rejected"));

            Assert.That(rejected.Accepted, Is.False);

            Assert.That(
                rejected.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Shutdown));

            AssertOutcome(
                rejected,
                UINotificationOutcome.Rejected);

            Assert.That(
                service.Dismiss(live).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            Assert.That(
                service.Dismiss(null).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Shutdown));
        }

        [Test]
        public void ShutdownSettlementCommitsTruthBeforeCallbacks()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            UINotificationHandle first =
                Admit(
                    service,
                    PrimaryChannel,
                    "first");

            UINotificationHandle second =
                Admit(
                    service,
                    PrimaryChannel,
                    "second");

            UINotificationOperationStatus firstStatus =
                default;

            UINotificationOperationStatus secondStatus =
                default;

            UINotificationHandle reentrant =
                null;

            bool callbackSawShutdown =
                false;

            first.Completed += _ =>
            {
                callbackSawShutdown =
                    service.IsShutdown &&
                    !service.IsValid &&
                    service.ChannelCount == 0;

                firstStatus =
                    service.Dismiss(first).Status;

                secondStatus =
                    service.Dismiss(second).Status;

                reentrant =
                    service.Admit(
                        new UINotificationRequest(
                            PrimaryChannel,
                            "reentrant"));
            };

            Assert.That(service.Shutdown(), Is.EqualTo(2));

            Assert.That(callbackSawShutdown, Is.True);

            Assert.That(
                firstStatus,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            Assert.That(
                secondStatus,
                Is.EqualTo(
                    UINotificationOperationStatus.Shutdown));

            Assert.That(reentrant.Accepted, Is.False);

            Assert.That(
                reentrant.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Shutdown));

            AssertOutcome(
                second,
                UINotificationOutcome.Shutdown);
        }

        [Test]
        public void ShutDownServiceRejectsForeignLiveHandle()
        {
            UINotificationService service =
                CreateService();

            UINotificationService foreignService =
                CreateService();

            UINotificationHandle foreign =
                Admit(
                    foreignService,
                    PrimaryChannel,
                    "foreign");

            Assert.That(service.Shutdown(), Is.EqualTo(0));

            Assert.That(
                service.Dismiss(foreign).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Shutdown));

            AssertState(
                foreignService,
                foreign,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void OwnerLossPromotionPreservesFifoPriorityTies()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 2);

            GameObject owner =
                CreateOwner("fifo-owner");

            UINotificationHandle lost =
                Admit(
                    service,
                    PrimaryChannel,
                    "lost",
                    owner: owner);

            UINotificationHandle first =
                Admit(
                    service,
                    PrimaryChannel,
                    "first",
                    priority: 10);

            UINotificationHandle second =
                Admit(
                    service,
                    PrimaryChannel,
                    "second",
                    priority: 10);

            Object.DestroyImmediate(owner);

            Assert.That(
                service.RefreshDestroyedOwners(),
                Is.EqualTo(1));

            AssertOutcome(
                lost,
                UINotificationOutcome.OwnerLost);

            AssertState(
                service,
                first,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                second,
                UINotificationEntryState.Pending);

            Assert.That(
                service.Dismiss(first).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            AssertState(
                service,
                second,
                UINotificationEntryState.Visible);
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
            int pendingCapacity = 3)
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
                            pendingCapacity),
                        new UINotificationChannelDefinition(
                            SecondaryChannel,
                            visibleCapacity,
                            pendingCapacity)
                    },
                    clock,
                    out string validationError);

            Assert.That(validationError, Is.Empty);
            Assert.That(service.IsValid, Is.True);
            return service;
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
            Object owner = null)
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

        private static void AssertOutcome(
            UINotificationHandle handle,
            UINotificationOutcome expected)
        {
            Assert.That(handle.IsCompleted, Is.True);
            Assert.That(handle.Result.Outcome, Is.EqualTo(expected));
            Assert.That(
                handle.Result.Generation,
                Is.EqualTo(handle.Generation));
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

        private static void AssertCounts(
            UINotificationService service,
            int visible,
            int pending)
        {
            Assert.That(service.VisibleCount, Is.EqualTo(visible));
            Assert.That(service.PendingCount, Is.EqualTo(pending));
        }

        private static void AssertSnapshotCounts(
            UINotificationService service,
            string channelId,
            int visible,
            int pending)
        {
            Assert.That(
                service.TryGetSnapshot(
                    channelId,
                    out UINotificationChannelSnapshot snapshot),
                Is.True);

            Assert.That(snapshot.VisibleCount, Is.EqualTo(visible));
            Assert.That(snapshot.PendingCount, Is.EqualTo(pending));
        }
    }
}
