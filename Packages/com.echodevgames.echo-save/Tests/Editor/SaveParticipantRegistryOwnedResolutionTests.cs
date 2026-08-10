
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantRegistryOwnedResolutionTests
    {
        [Test]
        public void OwnedResolutionReturnsLiveParticipantDescriptorAndToken()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            ParticipantApplyTestParticipant participant =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory");

            SaveParticipantRegistration registration =
                registry.Register(
                    participant)
                    .Registration;

            Assert.That(
                registry.TryResolveOwned(
                    participant.Descriptor.Id,
                    out ISaveParticipant resolved,
                    out SaveParticipantDescriptor descriptor,
                    out long token),
                Is.True);

            Assert.That(
                resolved,
                Is.SameAs(
                    participant));

            Assert.That(
                descriptor.Id,
                Is.EqualTo(
                    participant.Descriptor.Id));

            Assert.That(
                token,
                Is.GreaterThan(0));

            Assert.That(
                registry.Owns(
                    descriptor.Id,
                    token),
                Is.True);

            registration.Dispose();
        }

        [Test]
        public void ReplacementGetsDifferentOwnershipToken()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            ParticipantApplyTestParticipant first =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory");

            SaveParticipantRegistration firstRegistration =
                registry.Register(
                    first)
                    .Registration;

            registry.TryResolveOwned(
                first.Descriptor.Id,
                out _,
                out _,
                out long firstToken);

            firstRegistration.Dispose();

            ParticipantApplyTestParticipant replacement =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory");

            registry.Register(
                replacement);

            registry.TryResolveOwned(
                replacement.Descriptor.Id,
                out _,
                out _,
                out long replacementToken);

            Assert.That(
                replacementToken,
                Is.Not.EqualTo(
                    firstToken));
        }
    }
}
