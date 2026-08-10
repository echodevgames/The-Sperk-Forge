using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantMigrationIdTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("UPPER.case")]
        [TestCase("migration/id")]
        [TestCase("migration:id")]
        public void InvalidValuesReject(
            string value)
        {
            Assert.That(
                SaveParticipantMigrationId.TryParse(
                    value,
                    out _),
                Is.False);
        }

        [Test]
        public void ValidIdsUseOrdinalValueIdentity()
        {
            SaveParticipantMigrationId first =
                new SaveParticipantMigrationId(
                    "com.example.inventory.v1-v2");

            SaveParticipantMigrationId second =
                new SaveParticipantMigrationId(
                    "com.example.inventory.v1-v2");

            Assert.That(
                first,
                Is.EqualTo(
                    second));

            Assert.That(
                first.GetHashCode(),
                Is.EqualTo(
                    second.GetHashCode()));
        }

        [Test]
        public void ConstructorRejectsInvalidId()
        {
            Assert.Throws<ArgumentException>(
                () =>
                    new SaveParticipantMigrationId(
                        "Bad Id"));
        }
    }
}
