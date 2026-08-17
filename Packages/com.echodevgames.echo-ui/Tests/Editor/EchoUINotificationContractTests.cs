using NUnit.Framework;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationContractTests
    {
        [Test]
        public void StableIdentitiesNormalizeAndCompareOrdinally()
        {
            UINotificationChannelId first =
                new UINotificationChannelId(" notification.combat ");

            UINotificationChannelId second =
                new UINotificationChannelId("notification.combat");

            Assert.That(first.IsValid, Is.True);
            Assert.That(first, Is.EqualTo(second));
            Assert.That(first.Value, Is.EqualTo("notification.combat"));
            Assert.That(
                first,
                Is.Not.EqualTo(
                    new UINotificationChannelId("Notification.Combat")));
        }

        [Test]
        public void OptionalKeysNormalizeEmptyValues()
        {
            UINotificationCoalescingKey coalescing =
                new UINotificationCoalescingKey("   ");

            UINotificationCorrelationId correlation =
                new UINotificationCorrelationId(null);

            Assert.That(coalescing.IsEmpty, Is.True);
            Assert.That(coalescing.Value, Is.Empty);
            Assert.That(correlation.IsEmpty, Is.True);
            Assert.That(correlation.Value, Is.Empty);
        }

        [Test]
        public void ChannelDefinitionRetainsIndependentBoundsAndPolicy()
        {
            UINotificationChannelDefinition definition =
                new UINotificationChannelDefinition(
                    "notification.system",
                    visibleCapacity: 2,
                    pendingCapacity: 5,
                    defaultLifetimeSeconds: 3.5f,
                    overflowPolicy:
                        UINotificationOverflowPolicy.DropOldestPending);

            Assert.That(
                definition.ChannelId.Value,
                Is.EqualTo("notification.system"));

            Assert.That(definition.VisibleCapacity, Is.EqualTo(2));
            Assert.That(definition.PendingCapacity, Is.EqualTo(5));
            Assert.That(definition.DefaultLifetimeSeconds, Is.EqualTo(3.5f));
            Assert.That(
                definition.OverflowPolicy,
                Is.EqualTo(
                    UINotificationOverflowPolicy.DropOldestPending));
        }

        [Test]
        public void RequestCarriesOpaquePresentationWithoutDomainInterpretation()
        {
            object presentation =
                new object();

            UINotificationRequest request =
                new UINotificationRequest(
                    "notification.quest",
                    presentation,
                    priority: 25,
                    coalescingKey: "quest.updated",
                    correlationId: "quest-17");

            Assert.That(request.Presentation, Is.SameAs(presentation));
            Assert.That(request.Priority, Is.EqualTo(25));
            Assert.That(
                request.CoalescingKey.Value,
                Is.EqualTo("quest.updated"));
            Assert.That(
                request.CorrelationId.Value,
                Is.EqualTo("quest-17"));
        }

        [Test]
        public void AutomaticRequestDistinguishesDefaultAndOverrideLifetime()
        {
            UINotificationRequest useDefault =
                new UINotificationRequest(
                    "notification.default",
                    new object());

            UINotificationRequest useOverride =
                new UINotificationRequest(
                    "notification.default",
                    new object(),
                    durationSeconds: 1.25f);

            Assert.That(useDefault.UsesChannelDefaultLifetime, Is.True);
            Assert.That(useDefault.HasLifetimeOverride, Is.False);
            Assert.That(useOverride.UsesChannelDefaultLifetime, Is.False);
            Assert.That(useOverride.HasLifetimeOverride, Is.True);
        }

        [Test]
        public void OnlyAdmittedAndCoalescedAdmissionStatusesSucceed()
        {
            UINotificationChannelId channelId =
                new UINotificationChannelId("notification.default");

            UINotificationAdmissionResult admitted =
                new UINotificationAdmissionResult(
                    UINotificationAdmissionStatus.Admitted,
                    channelId,
                    generation: 1);

            UINotificationAdmissionResult coalesced =
                new UINotificationAdmissionResult(
                    UINotificationAdmissionStatus.Coalesced,
                    channelId,
                    generation: 2);

            UINotificationAdmissionResult rejected =
                new UINotificationAdmissionResult(
                    UINotificationAdmissionStatus.CapacityExceeded,
                    channelId);

            Assert.That(admitted.Succeeded, Is.True);
            Assert.That(coalesced.Succeeded, Is.True);
            Assert.That(rejected.Succeeded, Is.False);
        }
    }
}
