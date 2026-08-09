
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantRegistryTests
    {
        [Test]
        public void UniqueParticipantRegisters()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            TestParticipant participant =
                Participant(
                    "com.example.inventory");

            SaveParticipantRegistrationResult result =
                registry.Register(
                    participant);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                registry.Count,
                Is.EqualTo(1));

            Assert.That(
                result.Registration.IsActive,
                Is.True);
        }

        [Test]
        public void ArbitraryFutureParticipantRequiresNoPredeclaredCatalog()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            TestParticipant futureSystem =
                Participant(
                    "com.echodevgames.echo-pets");

            SaveParticipantRegistrationResult result =
                registry.Register(
                    futureSystem);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                registry.TryResolve(
                    futureSystem.Descriptor.Id,
                    out ISaveParticipant resolved),
                Is.True);

            Assert.That(
                resolved,
                Is.SameAs(
                    futureSystem));
        }

        [Test]
        public void NullParticipantIsRejectedWithoutMutation()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            SaveParticipantRegistrationResult result =
                registry.Register(
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantRegistrationStatus
                        .InvalidParticipant));

            Assert.That(
                registry.Count,
                Is.Zero);
        }

        [Test]
        public void InvalidDescriptorIsRejectedWithoutMutation()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            TestParticipant participant =
                new TestParticipant(
                    new SaveParticipantDescriptor(
                        Id("com.example.inventory"),
                        0,
                        SaveParticipantCriticality.Required,
                        SaveMissingPayloadPolicy.Fail,
                        default));

            SaveParticipantRegistrationResult result =
                registry.Register(
                    participant);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantRegistrationStatus
                        .InvalidDescriptor));

            Assert.That(
                registry.Count,
                Is.Zero);
        }

        [Test]
        public void DuplicateCanonicalIdRejectsLaterRegistration()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            TestParticipant first =
                Participant(
                    "com.example.inventory");

            TestParticipant second =
                Participant(
                    "com.example.inventory");

            Assert.That(
                registry.Register(
                    first)
                    .Succeeded,
                Is.True);

            SaveParticipantRegistrationResult duplicate =
                registry.Register(
                    second);

            Assert.That(
                duplicate.Status,
                Is.EqualTo(
                    SaveParticipantRegistrationStatus
                        .DuplicateId));

            Assert.That(
                registry.Count,
                Is.EqualTo(1));

            Assert.That(
                registry.TryResolve(
                    first.Descriptor.Id,
                    out ISaveParticipant resolved),
                Is.True);

            Assert.That(
                resolved,
                Is.SameAs(
                    first));
        }

        [Test]
        public void CanonicalIdCollidingWithActiveAliasIsRejected()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            TestParticipant first =
                Participant(
                    "com.example.inventory",
                    "com.example.items");

            Assert.That(
                registry.Register(
                    first)
                    .Succeeded,
                Is.True);

            SaveParticipantRegistrationResult collision =
                registry.Register(
                    Participant(
                        "com.example.items"));

            Assert.That(
                collision.Status,
                Is.EqualTo(
                    SaveParticipantRegistrationStatus
                        .AliasCollision));

            Assert.That(
                registry.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void AliasCollidingWithActiveCanonicalIdIsRejected()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            Assert.That(
                registry.Register(
                    Participant(
                        "com.example.inventory"))
                    .Succeeded,
                Is.True);

            SaveParticipantRegistrationResult collision =
                registry.Register(
                    Participant(
                        "com.example.quests",
                        "com.example.inventory"));

            Assert.That(
                collision.Status,
                Is.EqualTo(
                    SaveParticipantRegistrationStatus
                        .AliasCollision));

            Assert.That(
                registry.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void AliasCollidingWithActiveAliasIsRejected()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            Assert.That(
                registry.Register(
                    Participant(
                        "com.example.inventory",
                        "com.example.old-state"))
                    .Succeeded,
                Is.True);

            SaveParticipantRegistrationResult collision =
                registry.Register(
                    Participant(
                        "com.example.quests",
                        "com.example.old-state"));

            Assert.That(
                collision.Status,
                Is.EqualTo(
                    SaveParticipantRegistrationStatus
                        .AliasCollision));

            Assert.That(
                registry.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void CanonicalAndAliasLookupResolveSameParticipant()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            TestParticipant participant =
                Participant(
                    "com.example.inventory",
                    "com.example.items");

            registry.Register(
                participant);

            Assert.That(
                registry.TryResolve(
                    Id("com.example.inventory"),
                    out ISaveParticipant canonical),
                Is.True);

            Assert.That(
                registry.TryResolve(
                    Id("com.example.items"),
                    out ISaveParticipant alias),
                Is.True);

            Assert.That(
                canonical,
                Is.SameAs(
                    participant));

            Assert.That(
                alias,
                Is.SameAs(
                    participant));
        }

        [Test]
        public void SnapshotOrderIsCanonicalAndNotRegistrationOrder()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            registry.Register(
                Participant(
                    "com.example.zeta"));

            registry.Register(
                Participant(
                    "com.example.alpha"));

            registry.Register(
                Participant(
                    "com.example.middle"));

            SaveParticipantRegistrySnapshot snapshot =
                registry.GetSnapshot();

            Assert.That(
                snapshot.Count,
                Is.EqualTo(3));

            Assert.That(
                snapshot.Participants[0].Id.Value,
                Is.EqualTo(
                    "com.example.alpha"));

            Assert.That(
                snapshot.Participants[1].Id.Value,
                Is.EqualTo(
                    "com.example.middle"));

            Assert.That(
                snapshot.Participants[2].Id.Value,
                Is.EqualTo(
                    "com.example.zeta"));
        }

        [Test]
        public void AliasDoesNotCreateSecondSnapshotEntry()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            registry.Register(
                Participant(
                    "com.example.inventory",
                    "com.example.items"));

            Assert.That(
                registry.GetSnapshot().Count,
                Is.EqualTo(1));
        }

        [Test]
        public void SnapshotCannotBeMutatedThroughListInterface()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            registry.Register(
                Participant(
                    "com.example.inventory"));

            SaveParticipantRegistrySnapshot snapshot =
                registry.GetSnapshot();

            IList<SaveParticipantDescriptor> list =
                snapshot.Participants as
                    IList<SaveParticipantDescriptor>;

            Assert.That(
                list,
                Is.Not.Null);

            Assert.Throws<NotSupportedException>(
                () =>
                    list.Add(
                        Participant(
                            "com.example.quests")
                            .Descriptor));

            Assert.That(
                registry.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void DisposeUnregistersExactlyOwnedParticipant()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            SaveParticipantRegistration registration =
                registry.Register(
                    Participant(
                        "com.example.inventory"))
                    .Registration;

            registration.Dispose();

            Assert.That(
                registration.IsActive,
                Is.False);

            Assert.That(
                registry.Count,
                Is.Zero);
        }

        [Test]
        public void DisposeIsIdempotent()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            SaveParticipantRegistration registration =
                registry.Register(
                    Participant(
                        "com.example.inventory"))
                    .Registration;

            registration.Dispose();
            registration.Dispose();

            Assert.That(
                registry.Count,
                Is.Zero);
        }

        [Test]
        public void StaleRegistrationCannotRemoveReplacementParticipant()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            SaveParticipantRegistration first =
                registry.Register(
                    Participant(
                        "com.example.inventory"))
                    .Registration;

            first.Dispose();

            TestParticipant replacement =
                Participant(
                    "com.example.inventory");

            SaveParticipantRegistration second =
                registry.Register(
                    replacement)
                    .Registration;

            first.Dispose();

            Assert.That(
                second.IsActive,
                Is.True);

            Assert.That(
                registry.Count,
                Is.EqualTo(1));

            Assert.That(
                registry.TryResolve(
                    replacement.Descriptor.Id,
                    out ISaveParticipant resolved),
                Is.True);

            Assert.That(
                resolved,
                Is.SameAs(
                    replacement));
        }

        [Test]
        public void ClearMakesExistingHandlesStale()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            SaveParticipantRegistration first =
                registry.Register(
                    Participant(
                        "com.example.inventory"))
                    .Registration;

            registry.Clear();

            Assert.That(
                first.IsActive,
                Is.False);

            SaveParticipantRegistration replacement =
                registry.Register(
                    Participant(
                        "com.example.inventory"))
                    .Registration;

            first.Dispose();

            Assert.That(
                replacement.IsActive,
                Is.True);

            Assert.That(
                registry.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void RegisterAndLookupNeverInvokeCaptureOrApply()
        {
            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            TestParticipant participant =
                Participant(
                    "com.example.inventory");

            registry.Register(
                participant);

            registry.TryResolve(
                participant.Descriptor.Id,
                out _);

            registry.GetSnapshot();

            Assert.That(
                participant.CaptureCalls,
                Is.Zero);

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);
        }

        private static TestParticipant Participant(
            string canonical,
            params string[] aliases)
        {
            SaveParticipantId[] aliasIds =
                new SaveParticipantId[
                    aliases == null
                        ? 0
                        : aliases.Length];

            for (int i = 0;
                 i < aliasIds.Length;
                 i++)
            {
                aliasIds[i] =
                    Id(
                        aliases[i]);
            }

            return new TestParticipant(
                new SaveParticipantDescriptor(
                    Id(canonical),
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy
                        .InitializeDefault,
                    default,
                    aliasIds));
        }

        private static SaveParticipantId Id(
            string value) =>
            new SaveParticipantId(
                value);

        private sealed class TestParticipant :
            ISaveParticipant
        {
            internal TestParticipant(
                SaveParticipantDescriptor descriptor)
            {
                Descriptor =
                    descriptor;
            }

            public SaveParticipantDescriptor Descriptor
            {
                get;
            }

            internal int CaptureCalls { get; private set; }

            internal int ApplyCalls { get; private set; }

            public SaveParticipantCaptureResult Capture()
            {
                CaptureCalls++;

                return SaveParticipantCaptureResult.Success(
                    new object());
            }

            public SaveParticipantApplyResult Apply(
                object detachedState)
            {
                ApplyCalls++;

                return SaveParticipantApplyResult.Success();
            }
        }
    }
}
