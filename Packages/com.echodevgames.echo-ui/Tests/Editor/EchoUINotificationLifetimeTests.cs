using NUnit.Framework;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationLifetimeTests
    {
        private sealed class FakeClock :
            IUINotificationClock
        {
            public double NowSeconds { get; set; }
        }

        [Test]
        public void DefaultAutomaticLifetimeExpiresAtBoundary()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 4f);

            UINotificationHandle handle =
                Admit(service, "automatic");

            clock.NowSeconds = 3.999d;

            Assert.That(service.Tick(), Is.EqualTo(0));
            Assert.That(handle.IsCompleted, Is.False);

            clock.NowSeconds = 4d;

            Assert.That(service.Tick(), Is.EqualTo(1));
            Assert.That(handle.IsCompleted, Is.True);
            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));

            Assert.That(service.Tick(), Is.EqualTo(0));
            Assert.That(
                service.Dismiss(handle).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            AssertCounts(service, visible: 0, pending: 0);
        }

        [Test]
        public void AutomaticLifetimeOverrideReplacesChannelDefault()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 10f);

            UINotificationHandle handle =
                Admit(
                    service,
                    "override",
                    durationSeconds: 2f);

            clock.NowSeconds = 1.999d;

            Assert.That(service.Tick(), Is.EqualTo(0));
            Assert.That(handle.IsCompleted, Is.False);

            clock.NowSeconds = 2d;

            Assert.That(service.Tick(), Is.EqualTo(1));
            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));
        }

        [Test]
        public void PendingTimeDoesNotCountBeforePromotion()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 2f,
                    pendingCapacity: 1);

            UINotificationHandle visible =
                Admit(
                    service,
                    "manual-visible",
                    lifetimeMode:
                        UINotificationLifetimeMode.Manual);

            UINotificationHandle pending =
                Admit(service, "pending");

            clock.NowSeconds = 100d;

            Assert.That(service.Tick(), Is.EqualTo(0));

            AssertState(
                service,
                pending,
                UINotificationEntryState.Pending);

            Assert.That(
                service.Dismiss(visible).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            AssertState(
                service,
                pending,
                UINotificationEntryState.Visible);

            clock.NowSeconds = 101.999d;

            Assert.That(service.Tick(), Is.EqualTo(0));

            clock.NowSeconds = 102d;

            Assert.That(service.Tick(), Is.EqualTo(1));
            Assert.That(
                pending.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));
        }

        [Test]
        public void ManualEntryNeverExpiresAutomatically()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(clock);

            UINotificationHandle handle =
                Admit(
                    service,
                    "manual",
                    lifetimeMode:
                        UINotificationLifetimeMode.Manual);

            clock.NowSeconds = 1000000d;

            Assert.That(service.Tick(), Is.EqualTo(0));
            Assert.That(handle.IsCompleted, Is.False);

            AssertState(
                service,
                handle,
                UINotificationEntryState.Visible);

            Assert.That(
                service.Dismiss(handle).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Dismissed));
        }

        [Test]
        public void VisibleCoalescingRestartsReplacementLifetime()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 4f);

            UINotificationHandle prior =
                Admit(
                    service,
                    "prior",
                    coalescingKey: "shared.key");

            clock.NowSeconds = 3.5d;

            Assert.That(service.Tick(), Is.EqualTo(0));

            UINotificationHandle replacement =
                Admit(
                    service,
                    "replacement",
                    durationSeconds: 2f,
                    coalescingKey: "shared.key");

            Assert.That(
                prior.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Superseded));

            clock.NowSeconds = 5.499d;

            Assert.That(service.Tick(), Is.EqualTo(0));
            Assert.That(replacement.IsCompleted, Is.False);

            clock.NowSeconds = 5.5d;

            Assert.That(service.Tick(), Is.EqualTo(1));
            Assert.That(
                replacement.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));
        }

        [Test]
        public void PendingCoalescingStillWaitsForVisibility()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 2f,
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(
                    service,
                    "manual-visible",
                    lifetimeMode:
                        UINotificationLifetimeMode.Manual);

            UINotificationHandle prior =
                Admit(
                    service,
                    "prior",
                    coalescingKey: "shared.key");

            clock.NowSeconds = 50d;

            UINotificationHandle replacement =
                Admit(
                    service,
                    "replacement",
                    coalescingKey: "shared.key");

            Assert.That(
                prior.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Superseded));

            clock.NowSeconds = 100d;

            Assert.That(service.Tick(), Is.EqualTo(0));

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Pending);

            service.Dismiss(visible);

            clock.NowSeconds = 101.999d;

            Assert.That(service.Tick(), Is.EqualTo(0));

            clock.NowSeconds = 102d;

            Assert.That(service.Tick(), Is.EqualTo(1));
            Assert.That(
                replacement.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));
        }

        [Test]
        public void ExpirationPromotesHighestPriorityAndStartsItsLifetime()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 1f,
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(service, "visible");

            UINotificationHandle lower =
                Admit(
                    service,
                    "lower",
                    priority: 1);

            UINotificationHandle higher =
                Admit(
                    service,
                    "higher",
                    priority: 10);

            clock.NowSeconds = 1d;

            Assert.That(service.Tick(), Is.EqualTo(1));
            Assert.That(
                visible.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));

            AssertState(
                service,
                higher,
                UINotificationEntryState.Visible);

            AssertState(
                service,
                lower,
                UINotificationEntryState.Pending);

            clock.NowSeconds = 1.999d;

            Assert.That(service.Tick(), Is.EqualTo(0));

            clock.NowSeconds = 2d;

            Assert.That(service.Tick(), Is.EqualTo(1));
            Assert.That(
                higher.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));

            AssertState(
                service,
                lower,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void RegressingClockTickDoesNotMutateState()
        {
            FakeClock clock =
                new FakeClock
                {
                    NowSeconds = 10d
                };

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 5f);

            UINotificationHandle handle =
                Admit(service, "automatic");

            clock.NowSeconds = 13d;

            Assert.That(service.Tick(), Is.EqualTo(0));

            clock.NowSeconds = 9d;

            Assert.That(service.Tick(), Is.EqualTo(0));
            Assert.That(handle.IsCompleted, Is.False);

            AssertState(
                service,
                handle,
                UINotificationEntryState.Visible);

            clock.NowSeconds = 15d;

            Assert.That(service.Tick(), Is.EqualTo(1));
        }

        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void NonFiniteClockTickDoesNotMutateState(
            double invalidNow)
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 1f);

            UINotificationHandle handle =
                Admit(service, "automatic");

            clock.NowSeconds = invalidNow;

            Assert.That(service.Tick(), Is.EqualTo(0));
            Assert.That(handle.IsCompleted, Is.False);

            AssertState(
                service,
                handle,
                UINotificationEntryState.Visible);

            clock.NowSeconds = 1d;

            Assert.That(service.Tick(), Is.EqualTo(1));
        }

        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(-1d)]
        public void InvalidInitialClockRejectsWithoutPartialState(
            double invalidNow)
        {
            FakeClock clock =
                new FakeClock
                {
                    NowSeconds = invalidNow
                };

            UINotificationService service =
                new UINotificationService(
                    CreateDefinitions(),
                    clock,
                    out string validationError);

            Assert.That(validationError, Is.Not.Empty);
            Assert.That(service.IsValid, Is.False);
            Assert.That(service.ChannelCount, Is.EqualTo(0));
            Assert.That(service.Tick(), Is.EqualTo(0));
        }

        [Test]
        public void MissingInitialClockRejectsWithoutPartialState()
        {
            UINotificationService service =
                new UINotificationService(
                    CreateDefinitions(),
                    clock: null,
                    out string validationError);

            Assert.That(validationError, Is.Not.Empty);
            Assert.That(service.IsValid, Is.False);
            Assert.That(service.ChannelCount, Is.EqualTo(0));
        }

        [Test]
        public void ExpirationSettlementCannotReenterMutation()
        {
            FakeClock clock =
                new FakeClock();

            UINotificationService service =
                CreateService(
                    clock,
                    defaultLifetimeSeconds: 1f);

            UINotificationHandle expired =
                Admit(service, "expired");

            UINotificationHandle reentrant =
                null;

            UINotificationOperationStatus settledStatus =
                default;

            expired.Completed += _ =>
            {
                settledStatus =
                    service.Dismiss(expired).Status;

                reentrant =
                    service.Admit(
                        new UINotificationRequest(
                            ChannelId,
                            "reentrant"));
            };

            clock.NowSeconds = 1d;

            Assert.That(service.Tick(), Is.EqualTo(1));
            Assert.That(
                expired.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));

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

            AssertCounts(service, visible: 0, pending: 0);
        }

        private const string ChannelId =
            "notification.default";

        private static UINotificationService CreateService(
            FakeClock clock,
            float defaultLifetimeSeconds = 4f,
            int visibleCapacity = 1,
            int pendingCapacity = 3)
        {
            UINotificationService service =
                new UINotificationService(
                    CreateDefinitions(
                        defaultLifetimeSeconds,
                        visibleCapacity,
                        pendingCapacity),
                    clock,
                    out string validationError);

            Assert.That(validationError, Is.Empty);
            return service;
        }

        private static UINotificationChannelDefinition[] CreateDefinitions(
            float defaultLifetimeSeconds = 4f,
            int visibleCapacity = 1,
            int pendingCapacity = 3) =>
            new[]
            {
                new UINotificationChannelDefinition(
                    ChannelId,
                    visibleCapacity,
                    pendingCapacity,
                    defaultLifetimeSeconds)
            };

        private static UINotificationHandle Admit(
            UINotificationService service,
            string presentation,
            int priority = 0,
            UINotificationLifetimeMode lifetimeMode =
                UINotificationLifetimeMode.Automatic,
            float durationSeconds = 0f,
            string coalescingKey = "")
        {
            UINotificationHandle handle =
                service.Admit(
                    new UINotificationRequest(
                        ChannelId,
                        presentation,
                        priority,
                        lifetimeMode,
                        durationSeconds,
                        coalescingKey));

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
