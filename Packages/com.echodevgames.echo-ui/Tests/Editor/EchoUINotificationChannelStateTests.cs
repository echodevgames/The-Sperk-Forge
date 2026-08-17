using NUnit.Framework;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationChannelStateTests
    {
        [Test]
        public void ValidDefinitionsInitializeIndependentBoundedChannels()
        {
            UINotificationService service =
                CreateService(
                    out string validationError,
                    new UINotificationChannelDefinition(
                        "notification.status",
                        visibleCapacity: 2,
                        pendingCapacity: 4),
                    new UINotificationChannelDefinition(
                        "notification.utility",
                        visibleCapacity: 1,
                        pendingCapacity: 0,
                        overflowPolicy:
                            UINotificationOverflowPolicy.DropOldestPending));

            Assert.That(validationError, Is.Empty);
            Assert.That(service.IsValid, Is.True);
            Assert.That(service.ChannelCount, Is.EqualTo(2));
            Assert.That(service.VisibleCount, Is.EqualTo(0));
            Assert.That(service.PendingCount, Is.EqualTo(0));

            Assert.That(
                service.TryGetSnapshot(
                    "notification.status",
                    out UINotificationChannelSnapshot status),
                Is.True);

            Assert.That(status.VisibleCapacity, Is.EqualTo(2));
            Assert.That(status.PendingCapacity, Is.EqualTo(4));
            Assert.That(status.VisibleCount, Is.EqualTo(0));
            Assert.That(status.PendingCount, Is.EqualTo(0));

            Assert.That(
                service.TryGetSnapshot(
                    "notification.utility",
                    out UINotificationChannelSnapshot utility),
                Is.True);

            Assert.That(utility.VisibleCapacity, Is.EqualTo(1));
            Assert.That(utility.PendingCapacity, Is.EqualTo(0));
            Assert.That(
                utility.OverflowPolicy,
                Is.EqualTo(
                    UINotificationOverflowPolicy.DropOldestPending));
        }

        [Test]
        public void NullSourceRejectsWithoutPartialState()
        {
            UINotificationService service =
                new UINotificationService(
                    null,
                    out string validationError);

            Assert.That(validationError, Is.Not.Empty);
            AssertInvalidEmpty(service);
        }

        [Test]
        public void EmptySourceRejectsWithoutPartialState()
        {
            UINotificationService service =
                CreateService(
                    out string validationError);

            Assert.That(validationError, Is.Not.Empty);
            AssertInvalidEmpty(service);
        }

        [Test]
        public void MissingDefinitionRejectsWithoutPartialState()
        {
            UINotificationService service =
                CreateService(
                    out string validationError,
                    new UINotificationChannelDefinition(
                        "notification.valid"),
                    null);

            Assert.That(validationError, Is.Not.Empty);
            AssertInvalidEmpty(service);
        }

        [Test]
        public void DuplicateNormalizedIdRejectsWithoutPartialState()
        {
            UINotificationService service =
                CreateService(
                    out string validationError,
                    new UINotificationChannelDefinition(
                        "notification.duplicate"),
                    new UINotificationChannelDefinition(
                        " notification.duplicate "));

            Assert.That(validationError, Does.Contain("Duplicate"));
            AssertInvalidEmpty(service);
        }

        [TestCase(0, 1, 4f)]
        [TestCase(1, -1, 4f)]
        [TestCase(1, 1, 0f)]
        [TestCase(1, 1, -1f)]
        public void InvalidBoundsOrLifetimeRejectWithoutPartialState(
            int visibleCapacity,
            int pendingCapacity,
            float lifetime)
        {
            UINotificationService service =
                CreateService(
                    out string validationError,
                    new UINotificationChannelDefinition(
                        "notification.valid"),
                    new UINotificationChannelDefinition(
                        "notification.invalid",
                        visibleCapacity,
                        pendingCapacity,
                        lifetime));

            Assert.That(validationError, Is.Not.Empty);
            AssertInvalidEmpty(service);
        }

        [Test]
        public void NonFiniteLifetimeRejectsWithoutPartialState()
        {
            UINotificationService service =
                CreateService(
                    out string validationError,
                    new UINotificationChannelDefinition(
                        "notification.invalid",
                        defaultLifetimeSeconds: float.NaN));

            Assert.That(validationError, Does.Contain("finite"));
            AssertInvalidEmpty(service);
        }

        [Test]
        public void UnsupportedOverflowPolicyRejectsWithoutPartialState()
        {
            UINotificationService service =
                CreateService(
                    out string validationError,
                    new UINotificationChannelDefinition(
                        "notification.invalid",
                        overflowPolicy:
                            (UINotificationOverflowPolicy)99));

            Assert.That(validationError, Does.Contain("unsupported"));
            AssertInvalidEmpty(service);
        }

        [Test]
        public void DefinitionsAreSnapshottedAtInitialization()
        {
            UINotificationChannelDefinition authored =
                new UINotificationChannelDefinition(
                    "notification.snapshot",
                    visibleCapacity: 2,
                    pendingCapacity: 3);

            UINotificationService service =
                CreateService(
                    out string validationError,
                    authored);

            Assert.That(validationError, Is.Empty);
            Assert.That(
                service.TryGetDefinition(
                    "notification.snapshot",
                    out UINotificationChannelDefinition runtime),
                Is.True);

            Assert.That(runtime, Is.Not.SameAs(authored));
            Assert.That(runtime.ChannelId, Is.EqualTo(authored.ChannelId));
            Assert.That(runtime.VisibleCapacity, Is.EqualTo(2));
            Assert.That(runtime.PendingCapacity, Is.EqualTo(3));
        }

        [Test]
        public void UnknownOrEmptyLookupDoesNotMutateState()
        {
            UINotificationService service =
                CreateService(
                    out string validationError,
                    new UINotificationChannelDefinition(
                        "notification.known"));

            Assert.That(validationError, Is.Empty);
            Assert.That(
                service.TryGetSnapshot(
                    "notification.unknown",
                    out _),
                Is.False);

            Assert.That(
                service.TryGetDefinition(
                    "   ",
                    out _),
                Is.False);

            Assert.That(service.ChannelCount, Is.EqualTo(1));
            Assert.That(service.VisibleCount, Is.EqualTo(0));
            Assert.That(service.PendingCount, Is.EqualTo(0));
        }

        private static UINotificationService CreateService(
            out string validationError,
            params UINotificationChannelDefinition[] definitions) =>
            new UINotificationService(
                definitions,
                out validationError);

        private static void AssertInvalidEmpty(
            UINotificationService service)
        {
            Assert.That(service.IsValid, Is.False);
            Assert.That(service.ChannelCount, Is.EqualTo(0));
            Assert.That(service.VisibleCount, Is.EqualTo(0));
            Assert.That(service.PendingCount, Is.EqualTo(0));
        }
    }
}
