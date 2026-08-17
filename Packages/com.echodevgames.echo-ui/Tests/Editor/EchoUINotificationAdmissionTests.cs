using NUnit.Framework;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationAdmissionTests
    {
        [Test]
        public void AdmissionUsesVisibleCapacityBeforePendingCapacity()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 2,
                    pendingCapacity: 2);

            UINotificationHandle first =
                Admit(service, "first");

            UINotificationHandle second =
                Admit(service, "second");

            UINotificationHandle third =
                Admit(service, "third");

            AssertState(
                service,
                first,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                second,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                third,
                UINotificationEntryState.Pending);

            AssertCounts(service, visible: 2, pending: 1);
        }

        [Test]
        public void HigherPriorityPendingPromotesWithoutVisiblePreemption()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 3);

            UINotificationHandle visible =
                Admit(service, "visible", priority: 0);

            UINotificationHandle lower =
                Admit(service, "lower", priority: 10);

            UINotificationHandle higher =
                Admit(service, "higher", priority: 100);

            AssertState(
                service,
                visible,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                higher,
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
                lower,
                UINotificationEntryState.Pending);

            Assert.That(
                visible.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Dismissed));
        }

        [Test]
        public void EqualPriorityPendingPromotesInAdmissionFifoOrder()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 3);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle firstPending =
                Admit(service, "first-pending", priority: 50);

            UINotificationHandle secondPending =
                Admit(service, "second-pending", priority: 50);

            service.Dismiss(visible);

            AssertState(
                service,
                firstPending,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                secondPending,
                UINotificationEntryState.Pending);

            service.Dismiss(firstPending);

            AssertState(
                service,
                secondPending,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void ChannelsScheduleIndependently()
        {
            UINotificationService service =
                CreateTwoChannelService();

            UINotificationHandle statusVisible =
                Admit(
                    service,
                    "status-visible",
                    channelId: "notification.status");

            UINotificationHandle statusPending =
                Admit(
                    service,
                    "status-pending",
                    channelId: "notification.status",
                    priority: 100);

            UINotificationHandle utilityVisible =
                Admit(
                    service,
                    "utility-visible",
                    channelId: "notification.utility");

            service.Dismiss(utilityVisible);

            AssertState(
                service,
                statusVisible,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                statusPending,
                UINotificationEntryState.Pending);

            Assert.That(
                service.TryGetSnapshot(
                    "notification.status",
                    out UINotificationChannelSnapshot status),
                Is.True);

            Assert.That(status.VisibleCount, Is.EqualTo(1));
            Assert.That(status.PendingCount, Is.EqualTo(1));

            Assert.That(
                service.TryGetSnapshot(
                    "notification.utility",
                    out UINotificationChannelSnapshot utility),
                Is.True);

            Assert.That(utility.VisibleCount, Is.EqualTo(0));
            Assert.That(utility.PendingCount, Is.EqualTo(0));
        }

        [Test]
        public void FullDefaultChannelRejectsNewestWithoutMutation()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 1);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle pending =
                Admit(service, "pending");

            UINotificationHandle rejected =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        "rejected"));

            Assert.That(rejected.Accepted, Is.False);
            Assert.That(rejected.IsCompleted, Is.True);
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
        public void InvalidRequestsRejectWithoutMutation()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle missing =
                service.Admit(null);

            UINotificationHandle unknown =
                service.Admit(
                    new UINotificationRequest(
                        "notification.unknown",
                        new object()));

            UINotificationHandle noPresentation =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        null));

            UINotificationHandle invalidDuration =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        new object(),
                        durationSeconds: -1f));

            Assert.That(
                missing.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Invalid));

            Assert.That(
                unknown.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.UnknownChannel));

            Assert.That(
                noPresentation.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Invalid));

            Assert.That(
                invalidDuration.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Invalid));

            AssertCounts(service, visible: 0, pending: 0);
        }

        [Test]
        public void FirstKeyedRequestAdmitsWithoutMatchingLiveEntry()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle handle =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        new object(),
                        coalescingKey: "objective.updated"));

            Assert.That(handle.Accepted, Is.True);
            Assert.That(
                handle.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Admitted));

            AssertState(
                service,
                handle,
                UINotificationEntryState.Visible);

            AssertCounts(service, visible: 1, pending: 0);
        }

        [Test]
        public void DismissingPendingSettlesOnlyThatGeneration()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle pending =
                Admit(service, "pending");

            Assert.That(
                service.Dismiss(pending).Succeeded,
                Is.True);

            Assert.That(pending.IsCompleted, Is.True);
            Assert.That(visible.IsCompleted, Is.False);

            AssertState(
                service,
                visible,
                UINotificationEntryState.Visible);

            AssertCounts(service, visible: 1, pending: 0);
        }

        [Test]
        public void DismissalIsIdempotentAndForeignHandleIsStale()
        {
            UINotificationService service =
                CreateService();

            UINotificationService foreignService =
                CreateService();

            UINotificationHandle local =
                Admit(service, "local");

            UINotificationHandle foreign =
                Admit(foreignService, "foreign");

            Assert.That(
                service.Dismiss(foreign).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Stale));

            AssertState(
                service,
                local,
                UINotificationEntryState.Visible);

            Assert.That(
                service.Dismiss(local).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            Assert.That(
                service.Dismiss(local).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));
        }

        [Test]
        public void EveryAttemptReceivesFreshMonotonicGeneration()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 0);

            UINotificationHandle accepted =
                Admit(service, "accepted");

            UINotificationHandle rejected =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        "rejected"));

            service.Dismiss(accepted);

            UINotificationHandle replacement =
                Admit(service, "replacement");

            Assert.That(
                rejected.Generation,
                Is.GreaterThan(accepted.Generation));

            Assert.That(
                replacement.Generation,
                Is.GreaterThan(rejected.Generation));

            Assert.That(replacement.Accepted, Is.True);
        }

        private const string ChannelId =
            "notification.default";

        private static UINotificationService CreateService(
            int visibleCapacity = 1,
            int pendingCapacity = 3)
        {
            UINotificationService service =
                new UINotificationService(
                    new[]
                    {
                        new UINotificationChannelDefinition(
                            ChannelId,
                            visibleCapacity,
                            pendingCapacity)
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
                            pendingCapacity: 2),
                        new UINotificationChannelDefinition(
                            "notification.utility",
                            visibleCapacity: 1,
                            pendingCapacity: 2)
                    },
                    out string validationError);

            Assert.That(validationError, Is.Empty);
            return service;
        }

        private static UINotificationHandle Admit(
            UINotificationService service,
            string presentation,
            string channelId = ChannelId,
            int priority = 0)
        {
            UINotificationHandle handle =
                service.Admit(
                    new UINotificationRequest(
                        channelId,
                        presentation,
                        priority));

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
