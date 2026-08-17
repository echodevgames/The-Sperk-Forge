using NUnit.Framework;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationCoalescingTests
    {
        [Test]
        public void VisibleCoalescingReplacesOneGenerationInPlace()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 0);

            UINotificationHandle prior =
                AdmitKeyed(
                    service,
                    "prior",
                    "objective.updated");

            UINotificationHandle replacement =
                AdmitKeyed(
                    service,
                    "replacement",
                    "objective.updated");

            Assert.That(prior.IsCompleted, Is.True);
            Assert.That(
                prior.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Superseded));

            Assert.That(
                replacement.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Coalesced));

            Assert.That(
                replacement.Generation,
                Is.GreaterThan(prior.Generation));

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);

            AssertCounts(service, visible: 1, pending: 0);
        }

        [Test]
        public void PendingCoalescingPreservesEqualPriorityFifoPosition()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 3);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle prior =
                AdmitKeyed(
                    service,
                    "prior",
                    "objective.updated",
                    priority: 50);

            UINotificationHandle peer =
                Admit(
                    service,
                    "peer",
                    priority: 50);

            UINotificationHandle replacement =
                AdmitKeyed(
                    service,
                    "replacement",
                    "objective.updated",
                    priority: 50);

            Assert.That(prior.IsCompleted, Is.True);
            AssertCounts(service, visible: 1, pending: 2);

            service.Dismiss(visible);

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                peer,
                UINotificationEntryState.Pending);
        }

        [Test]
        public void PendingCoalescingUsesReplacementPriority()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 3);

            UINotificationHandle visible =
                Admit(service, "visible");

            AdmitKeyed(
                service,
                "prior",
                "objective.updated",
                priority: 1);

            UINotificationHandle peer =
                Admit(
                    service,
                    "peer",
                    priority: 10);

            UINotificationHandle replacement =
                AdmitKeyed(
                    service,
                    "replacement",
                    "objective.updated",
                    priority: 100);

            service.Dismiss(visible);

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                peer,
                UINotificationEntryState.Pending);
        }

        [Test]
        public void SameKeyIsScopedIndependentlyPerChannel()
        {
            UINotificationService service =
                CreateTwoChannelService();

            UINotificationHandle statusPrior =
                AdmitKeyed(
                    service,
                    "status-prior",
                    "shared.key",
                    channelId: "notification.status");

            UINotificationHandle utility =
                AdmitKeyed(
                    service,
                    "utility",
                    "shared.key",
                    channelId: "notification.utility");

            UINotificationHandle statusReplacement =
                AdmitKeyed(
                    service,
                    "status-replacement",
                    "shared.key",
                    channelId: "notification.status");

            Assert.That(statusPrior.IsCompleted, Is.True);
            Assert.That(utility.IsCompleted, Is.False);

            AssertState(
                service,
                statusReplacement,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                utility,
                UINotificationEntryState.Visible);

            AssertCounts(service, visible: 2, pending: 0);
        }

        [Test]
        public void DistinctKeysDoNotCoalesce()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 2,
                    pendingCapacity: 0);

            UINotificationHandle first =
                AdmitKeyed(
                    service,
                    "first",
                    "key.first");

            UINotificationHandle second =
                AdmitKeyed(
                    service,
                    "second",
                    "key.second");

            Assert.That(first.IsCompleted, Is.False);
            Assert.That(second.IsCompleted, Is.False);
            AssertCounts(service, visible: 2, pending: 0);
        }

        [Test]
        public void RepeatedCoalescingNeverMultipliesLiveEntries()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle first =
                AdmitKeyed(
                    service,
                    "first",
                    "shared.key");

            UINotificationHandle second =
                AdmitKeyed(
                    service,
                    "second",
                    "shared.key");

            UINotificationHandle third =
                AdmitKeyed(
                    service,
                    "third",
                    "shared.key");

            Assert.That(first.IsCompleted, Is.True);
            Assert.That(second.IsCompleted, Is.True);
            Assert.That(third.IsCompleted, Is.False);

            Assert.That(
                third.Generation,
                Is.GreaterThan(second.Generation));

            AssertCounts(service, visible: 1, pending: 0);
        }

        [Test]
        public void SupersededHandleIsStaleAndCannotDismissReplacement()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle prior =
                AdmitKeyed(
                    service,
                    "prior",
                    "shared.key");

            UINotificationHandle replacement =
                AdmitKeyed(
                    service,
                    "replacement",
                    "shared.key");

            Assert.That(
                service.Dismiss(prior).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Stale));

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);

            AssertCounts(service, visible: 1, pending: 0);
        }

        [Test]
        public void SupersededCompletionCannotReenterCoalescingMutation()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle prior =
                AdmitKeyed(
                    service,
                    "prior",
                    "shared.key");

            UINotificationHandle reentrant =
                null;

            UINotificationOperationStatus staleStatus =
                default;

            prior.Completed += _ =>
            {
                staleStatus =
                    service.Dismiss(prior).Status;

                reentrant =
                    service.Admit(
                        new UINotificationRequest(
                            ChannelId,
                            "reentrant",
                            coalescingKey: "shared.key"));
            };

            UINotificationHandle replacement =
                AdmitKeyed(
                    service,
                    "replacement",
                    "shared.key");

            Assert.That(reentrant, Is.Not.Null);
            Assert.That(reentrant.Accepted, Is.False);
            Assert.That(
                reentrant.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Unavailable));

            Assert.That(
                staleStatus,
                Is.EqualTo(
                    UINotificationOperationStatus.Stale));

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);

            AssertCounts(service, visible: 1, pending: 0);
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
                            pendingCapacity: 1),
                        new UINotificationChannelDefinition(
                            "notification.utility",
                            visibleCapacity: 1,
                            pendingCapacity: 1)
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

        private static UINotificationHandle AdmitKeyed(
            UINotificationService service,
            string presentation,
            string key,
            string channelId = ChannelId,
            int priority = 0)
        {
            UINotificationHandle handle =
                service.Admit(
                    new UINotificationRequest(
                        channelId,
                        presentation,
                        priority,
                        coalescingKey: key));

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
