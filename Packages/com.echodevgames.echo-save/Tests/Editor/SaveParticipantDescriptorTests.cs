
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantDescriptorTests
    {
        [Test]
        public void ValidDescriptorPreservesPolicies()
        {
            SaveParticipantDescriptor descriptor =
                new SaveParticipantDescriptor(
                    Id("com.example.inventory"),
                    3,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy
                        .InitializeDefault,
                    default,
                    Id("com.example.items"));

            Assert.That(
                descriptor.TryValidate(
                    out _,
                    out _),
                Is.True);

            Assert.That(
                descriptor.CurrentSchemaVersion,
                Is.EqualTo(3));

            Assert.That(
                descriptor.Criticality,
                Is.EqualTo(
                    SaveParticipantCriticality
                        .Required));

            Assert.That(
                descriptor.MissingPayloadPolicy,
                Is.EqualTo(
                    SaveMissingPayloadPolicy
                        .InitializeDefault));

            Assert.That(
                descriptor.Aliases.Count,
                Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void NonPositiveSchemaVersionIsInvalid(
            int schemaVersion)
        {
            SaveParticipantDescriptor descriptor =
                Descriptor(
                    "com.example.inventory",
                    schemaVersion);

            Assert.That(
                descriptor.TryValidate(
                    out string diagnosticCode,
                    out _),
                Is.False);

            Assert.That(
                diagnosticCode,
                Is.EqualTo(
                    "ESV-PART-002"));
        }

        [Test]
        public void DefaultCanonicalIdIsInvalid()
        {
            SaveParticipantDescriptor descriptor =
                new SaveParticipantDescriptor(
                    default,
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy.Fail,
                    default);

            Assert.That(
                descriptor.TryValidate(
                    out string diagnosticCode,
                    out _),
                Is.False);

            Assert.That(
                diagnosticCode,
                Is.EqualTo(
                    "ESV-PART-001"));
        }

        [Test]
        public void InvalidCriticalityIsRejected()
        {
            SaveParticipantDescriptor descriptor =
                new SaveParticipantDescriptor(
                    Id("com.example.inventory"),
                    1,
                    (SaveParticipantCriticality)99,
                    SaveMissingPayloadPolicy.Fail,
                    default);

            Assert.That(
                descriptor.TryValidate(
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void InvalidMissingPayloadPolicyIsRejected()
        {
            SaveParticipantDescriptor descriptor =
                new SaveParticipantDescriptor(
                    Id("com.example.inventory"),
                    1,
                    SaveParticipantCriticality.Required,
                    (SaveMissingPayloadPolicy)99,
                    default);

            Assert.That(
                descriptor.TryValidate(
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void CanonicalIdCannotAlsoBeAlias()
        {
            SaveParticipantId id =
                Id(
                    "com.example.inventory");

            SaveParticipantDescriptor descriptor =
                new SaveParticipantDescriptor(
                    id,
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy.Fail,
                    default,
                    id);

            Assert.That(
                descriptor.TryValidate(
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void DuplicateAliasesAreRejected()
        {
            SaveParticipantId alias =
                Id(
                    "com.example.old-inventory");

            SaveParticipantDescriptor descriptor =
                new SaveParticipantDescriptor(
                    Id("com.example.inventory"),
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy.Fail,
                    default,
                    alias,
                    alias);

            Assert.That(
                descriptor.TryValidate(
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void AliasInputArrayIsDefensivelyCopied()
        {
            SaveParticipantId[] aliases =
            {
                Id("com.example.old-inventory")
            };

            SaveParticipantDescriptor descriptor =
                new SaveParticipantDescriptor(
                    Id("com.example.inventory"),
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy.Fail,
                    default,
                    aliases);

            aliases[0] =
                Id(
                    "com.example.changed");

            Assert.That(
                descriptor.Aliases[0].Value,
                Is.EqualTo(
                    "com.example.old-inventory"));
        }

        [Test]
        public void AliasCountIsBounded()
        {
            SaveParticipantId[] aliases =
                new SaveParticipantId[
                    SaveParticipantDescriptor.MaxAliases +
                    1];

            for (int i = 0;
                 i < aliases.Length;
                 i++)
            {
                aliases[i] =
                    Id(
                        $"com.example.old-{i}");
            }

            SaveParticipantDescriptor descriptor =
                new SaveParticipantDescriptor(
                    Id("com.example.inventory"),
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy.Fail,
                    default,
                    aliases);

            Assert.That(
                descriptor.TryValidate(
                    out _,
                    out _),
                Is.False);
        }

        private static SaveParticipantDescriptor
            Descriptor(
                string id,
                int schemaVersion) =>
            new SaveParticipantDescriptor(
                Id(id),
                schemaVersion,
                SaveParticipantCriticality.Required,
                SaveMissingPayloadPolicy.Fail,
                default);

        private static SaveParticipantId Id(
            string value) =>
            new SaveParticipantId(
                value);
    }
}
