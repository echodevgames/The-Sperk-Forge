using NUnit.Framework;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationOverflowTests
    {
        [Test]
        public void RejectNewestLeavesFullChannelUnchanged()
        {
            UINotificationService service =
                CreateService(
                    UINotificationOverflowPolicy.RejectNewest,
                    pendingCapacity: 1);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle pending =
                Admit(service, "pending");

            UINotificationHandle rejected =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        "rejected",
                        priority: 100));

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(
                rejected.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.CapacityExceeded));

            Assert.That(
                rejected.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Rejected));

            AssertState(
                service,
                visible,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                pending,
                UINotificationEntryState.Pending);

            AssertCounts(service, visible: 1, pending: 1);
        }

        [Test]
        public void DropOldestPendingIgnoresPriorityAndSettlesVictim()
        {
            UINotificationService service =
                CreateService(
                    UINotificationOverflowPolicy.DropOldestPending,
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle oldest =
                Admit(
                    service,
                    "oldest",
                    priority: 100);

            UINotificationHandle newer =
                Admit(
                    service,
                    "newer",
                    priority: 10);

            UINotificationHandle incoming =
                Admit(
                    service,
                    "incoming",
                    priority: 1);

            Assert.That(oldest.IsCompleted, Is.True);
            Assert.That(
                oldest.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.OverflowEvicted));

            Assert.That(
                service.Dismiss(oldest).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            AssertState(
                service,
                visible,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                newer,
                UINotificationEntryState.Pending);

            AssertState(
                service,
                incoming,
                UINotificationEntryState.Pending);

            AssertCounts(service, visible: 1, pending: 2);
        }

        [Test]
        public void ReplaceLowestPriorityPendingAcceptsStrictlyHigherPriority()
        {
            UINotificationService service =
                CreateService(
                    UINotificationOverflowPolicy.ReplaceLowestPriorityPending,
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle lowest =
                Admit(
                    service,
                    "lowest",
                    priority: 5);

            UINotificationHandle higher =
                Admit(
                    service,
                    "higher",
                    priority: 20);

            UINotificationHandle incoming =
                Admit(
                    service,
                    "incoming",
                    priority: 6);

            Assert.That(lowest.IsCompleted, Is.True);
            Assert.That(
                lowest.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.OverflowEvicted));

            Assert.That(
                incoming.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Admitted));

            Assert.That(
                incoming.Generation,
                Is.GreaterThan(lowest.Generation));

            AssertState(
                service,
                higher,
                UINotificationEntryState.Pending);

            AssertState(
                service,
                incoming,
                UINotificationEntryState.Pending);

            Assert.That(
                service.Dismiss(visible).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            AssertState(
                service,
                higher,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                incoming,
                UINotificationEntryState.Pending);
        }

        [TestCase(10)]
        [TestCase(9)]
        public void ReplaceLowestPriorityPendingRejectsEqualOrLowerPriority(
            int incomingPriority)
        {
            UINotificationService service =
                CreateService(
                    UINotificationOverflowPolicy.ReplaceLowestPriorityPending,
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle lowest =
                Admit(
                    service,
                    "lowest",
                    priority: 10);

            UINotificationHandle higher =
                Admit(
                    service,
                    "higher",
                    priority: 20);

            UINotificationHandle rejected =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        "rejected",
                        incomingPriority));

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(
                rejected.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.InsufficientPriority));

            Assert.That(visible.IsCompleted, Is.False);
            Assert.That(lowest.IsCompleted, Is.False);
            Assert.That(higher.IsCompleted, Is.False);

            AssertState(
                service,
                lowest,
                UINotificationEntryState.Pending);

            AssertState(
                service,
                higher,
                UINotificationEntryState.Pending);

            AssertCounts(service, visible: 1, pending: 2);
        }

        [Test]
        public void ReplaceLowestPriorityTieEvictsNewestLowestEntry()
        {
            UINotificationService service =
                CreateService(
                    UINotificationOverflowPolicy.ReplaceLowestPriorityPending,
                    pendingCapacity: 3);

            Admit(service, "visible");

            UINotificationHandle olderLowest =
                Admit(
                    service,
                    "older-lowest",
                    priority: 5);

            UINotificationHandle newerLowest =
                Admit(
                    service,
                    "newer-lowest",
                    priority: 5);

            UINotificationHandle higher =
                Admit(
                    service,
                    "higher",
                    priority: 20);

            UINotificationHandle incoming =
                Admit(
                    service,
                    "incoming",
                    priority: 6);

            Assert.That(olderLowest.IsCompleted, Is.False);
            Assert.That(newerLowest.IsCompleted, Is.True);
            Assert.That(
                newerLowest.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.OverflowEvicted));

            AssertState(
                service,
                olderLowest,
                UINotificationEntryState.Pending);

            AssertState(
                service,
                higher,
                UINotificationEntryState.Pending);

            AssertState(
                service,
                incoming,
                UINotificationEntryState.Pending);

            AssertCounts(service, visible: 1, pending: 3);
        }

        [TestCase(UINotificationOverflowPolicy.DropOldestPending)]
        [TestCase(UINotificationOverflowPolicy.ReplaceLowestPriorityPending)]
        public void EvictingPolicyWithZeroPendingCapacityRejectsSafely(
            UINotificationOverflowPolicy policy)
        {
            UINotificationService service =
                CreateService(
                    policy,
                    pendingCapacity: 0);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle rejected =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        "rejected",
                        priority: 100));

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(
                rejected.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.CapacityExceeded));

            AssertState(
                service,
                visible,
                UINotificationEntryState.Visible);

            AssertCounts(service, visible: 1, pending: 0);
        }

        [Test]
        public void OverflowPolicyIsScopedToItsChannel()
        {
            UINotificationService service =
                CreateTwoChannelService();

            Admit(
                service,
                "status-visible",
                channelId: "notification.status");

            UINotificationHandle statusPending =
                Admit(
                    service,
                    "status-pending",
                    channelId: "notification.status");

            Admit(
                service,
                "utility-visible",
                channelId: "notification.utility");

            UINotificationHandle utilityPending =
                Admit(
                    service,
                    "utility-pending",
                    channelId: "notification.utility");

            UINotificationHandle statusIncoming =
                Admit(
                    service,
                    "status-incoming",
                    channelId: "notification.status");

            UINotificationHandle utilityRejected =
                service.Admit(
                    new UINotificationRequest(
                        "notification.utility",
                        "utility-rejected"));

            Assert.That(statusPending.IsCompleted, Is.True);
            Assert.That(utilityPending.IsCompleted, Is.False);

            Assert.That(utilityRejected.Accepted, Is.False);
            Assert.That(
                utilityRejected.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.CapacityExceeded));

            AssertState(
                service,
                statusIncoming,
                UINotificationEntryState.Pending);

            AssertState(
                service,
                utilityPending,
                UINotificationEntryState.Pending);

            AssertCounts(service, visible: 2, pending: 2);
        }

        [Test]
        public void CoalescingPrecedesOverflowAtFullCapacity()
        {
            UINotificationService service =
                CreateService(
                    UINotificationOverflowPolicy.DropOldestPending,
                    pendingCapacity: 2);

            Admit(service, "visible");

            UINotificationHandle prior =
                Admit(
                    service,
                    "prior",
                    coalescingKey: "shared.key");

            UINotificationHandle unrelated =
                Admit(service, "unrelated");

            UINotificationHandle replacement =
                Admit(
                    service,
                    "replacement",
                    coalescingKey: "shared.key");

            Assert.That(prior.IsCompleted, Is.True);
            Assert.That(
                prior.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Superseded));

            Assert.That(unrelated.IsCompleted, Is.False);
            Assert.That(
                replacement.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Coalesced));

            AssertState(
                service,
                unrelated,
                UINotificationEntryState.Pending);

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Pending);

            AssertCounts(service, visible: 1, pending: 2);
        }

        [Test]
        public void OverflowSettlementCannotReenterMutation()
        {
            UINotificationService service =
                CreateService(
                    UINotificationOverflowPolicy.DropOldestPending,
                    pendingCapacity: 1);

            Admit(service, "visible");

            UINotificationHandle victim =
                Admit(service, "victim");

            UINotificationHandle reentrant =
                null;

            UINotificationOperationStatus settledStatus =
                default;

            victim.Completed += _ =>
            {
                settledStatus =
                    service.Dismiss(victim).Status;

                reentrant =
                    service.Admit(
                        new UINotificationRequest(
                            ChannelId,
                            "reentrant"));
            };

            UINotificationHandle incoming =
                Admit(service, "incoming");

            Assert.That(
                victim.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.OverflowEvicted));

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

            AssertState(
                service,
                incoming,
                UINotificationEntryState.Pending);

            AssertCounts(service, visible: 1, pending: 1);
        }

        private const string ChannelId =
            "notification.default";

        private static UINotificationService CreateService(
            UINotificationOverflowPolicy policy,
            int visibleCapacity = 1,
            int pendingCapacity = 2)
        {
            UINotificationService service =
                new UINotificationService(
                    new[]
                    {
                        new UINotificationChannelDefinition(
                            ChannelId,
                            visibleCapacity,
                            pendingCapacity,
                            overflowPolicy: policy)
                    },
                    out string validationError);

            Assert.That(validationError, Is.Empty);
            return service;
        }

        private static UINotificationService CreateTwoChannelService()
        {
            UINotificationService service =
                new UINotificationService(
                    new[]
                    {
                        new UINotificationChannelDefinition(
                            "notification.status",
                            visibleCapacity: 1,
                            pendingCapacity: 1,
                            overflowPolicy:
                                UINotificationOverflowPolicy.DropOldestPending),
                        new UINotificationChannelDefinition(
                            "notification.utility",
                            visibleCapacity: 1,
                            pendingCapacity: 1,
                            overflowPolicy:
                                UINotificationOverflowPolicy.RejectNewest)
                    },
                    out string validationError);

            Assert.That(validationError, Is.Empty);
            return service;
        }

        private static UINotificationHandle Admit(
            UINotificationService service,
            string presentation,
            string channelId = ChannelId,
            int priority = 0,
            string coalescingKey = "")
        {
            UINotificationHandle handle =
                service.Admit(
                    new UINotificationRequest(
                        channelId,
                        presentation,
                        priority,
                        coalescingKey: coalescingKey));

            Assert.That(handle.Accepted, Is.True);
            return handle;
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
    }
}
