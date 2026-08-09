
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveTechnicalIdTests
    {
        [Test]
        public void NewSlotIdIsCanonicalLowercaseGuid()
        {
            SaveSlotId id =
                SaveSlotId.NewId();

            Assert.That(
                SaveSlotId.TryParse(
                    id.Value,
                    out SaveSlotId parsed),
                Is.True);

            Assert.That(
                parsed,
                Is.EqualTo(id));

            Assert.That(
                id.Value,
                Is.EqualTo(
                    id.Value.ToLowerInvariant()));
        }

        [TestCase("")]
        [TestCase("not-a-guid")]
        [TestCase("../slot")]
        [TestCase("A2F7744B-0B69-4D21-8E78-7A6D149CBE9C")]
        public void UnsafeOrNonCanonicalSlotIdIsRejected(
            string value)
        {
            Assert.That(
                SaveSlotId.TryParse(
                    value,
                    out _),
                Is.False);
        }

        [Test]
        public void GenerationIdContainsSequenceAndRandomness()
        {
            SaveGenerationId id =
                SaveGenerationId.CreateForTesting(
                    new DateTime(
                        2026,
                        8,
                        9,
                        16,
                        53,
                        0,
                        DateTimeKind.Utc),
                    42,
                    Guid.Parse(
                        "00112233-4455-6677-8899-aabbccddeeff"));

            Assert.That(
                id.Value,
                Does.Contain(
                    "-0000000000000042-"));

            Assert.That(
                id.Value,
                Does.EndWith(
                    "00112233445566778899aabbccddeeff"));

            Assert.That(
                SaveGenerationId.TryParse(
                    id.Value,
                    out SaveGenerationId parsed),
                Is.True);

            Assert.That(
                parsed,
                Is.EqualTo(id));
        }

        [Test]
        public void GenerationIdsAreUnique()
        {
            SaveGenerationId first =
                SaveGenerationId.NewId();

            SaveGenerationId second =
                SaveGenerationId.NewId();

            Assert.That(
                second,
                Is.Not.EqualTo(first));
        }

        [Test]
        public void GenerationIdsSortByCanonicalValue()
        {
            DateTime timestamp =
                new DateTime(
                    2026,
                    8,
                    9,
                    16,
                    53,
                    0,
                    DateTimeKind.Utc);

            SaveGenerationId first =
                SaveGenerationId.CreateForTesting(
                    timestamp,
                    1,
                    Guid.Empty);

            SaveGenerationId second =
                SaveGenerationId.CreateForTesting(
                    timestamp,
                    2,
                    Guid.Empty);

            Assert.That(
                first.CompareTo(second),
                Is.LessThan(0));
        }

        [TestCase("")]
        [TestCase("../generation")]
        [TestCase("20260809T1653000000000Z-0000000000000000-00112233445566778899aabbccddeeff")]
        [TestCase("20260809T1653000000000Z-0000000000000001-00112233-4455-6677-8899-aabbccddeeff")]
        public void UnsafeGenerationIdIsRejected(
            string value)
        {
            Assert.That(
                SaveGenerationId.TryParse(
                    value,
                    out _),
                Is.False);
        }
    }
}
