
using System;
using System.IO;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class UnityJsonSaveSerializerTests
    {
        private UnityJsonSaveSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer =
                new UnityJsonSaveSerializer();
        }

        [Test]
        public void PlainDtoRoundTripPreservesFields()
        {
            PlainDto source =
                new PlainDto
                {
                    number = 42,
                    label = "Chronicle",
                    enabled = true
                };

            SaveSerializerResult serializedResult =
                serializer.Serialize(
                    source,
                    out string json);

            SaveSerializerResult deserializedResult =
                serializer.Deserialize(
                    json,
                    out PlainDto restored);

            Assert.That(
                serializedResult.Succeeded,
                Is.True);

            Assert.That(
                deserializedResult.Succeeded,
                Is.True);

            Assert.That(
                restored.number,
                Is.EqualTo(42));

            Assert.That(
                restored.label,
                Is.EqualTo(
                    "Chronicle"));

            Assert.That(
                restored.enabled,
                Is.True);
        }

        [Test]
        public void PackageEnvelopeRoundTripPreservesAuthoredFields()
        {
            SaveDocumentEnvelope source =
                new SaveDocumentEnvelope
                {
                    documentId =
                        "document-001",
                    technicalTimestampUtc =
                        "2026-08-09T16:00:00Z"
                };

            SaveSerializerResult serializedResult =
                serializer.Serialize(
                    source,
                    out string json);

            SaveSerializerResult deserializedResult =
                serializer.Deserialize(
                    json,
                    out SaveDocumentEnvelope restored);

            Assert.That(
                serializedResult.Succeeded,
                Is.True);

            Assert.That(
                deserializedResult.Succeeded,
                Is.True);

            Assert.That(
                restored.documentKind,
                Is.EqualTo(
                    SaveDocumentKinds.Envelope));

            Assert.That(
                restored.formatMajor,
                Is.EqualTo(
                    SaveDocumentVersions
                        .EnvelopeMajor));

            Assert.That(
                restored.serializerId,
                Is.EqualTo(
                    UnityJsonSaveSerializer
                        .StableId));

            Assert.That(
                restored.documentId,
                Is.EqualTo(
                    "document-001"));

            Assert.That(
                restored.technicalTimestampUtc,
                Is.EqualTo(
                    "2026-08-09T16:00:00Z"));
        }

        [Test]
        public void NullSerializeRequestIsRejected()
        {
            SaveSerializerResult result =
                serializer
                    .Serialize<SaveDocumentEnvelope>(
                        null,
                        out string json);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .InvalidRequest));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-SERIAL-001"));

            Assert.That(
                json,
                Is.Empty);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void EmptyDeserializeRequestIsRejected(
            string input)
        {
            SaveSerializerResult result =
                serializer
                    .Deserialize<SaveDocumentEnvelope>(
                        input,
                        out _);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .InvalidRequest));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-SERIAL-001"));
        }

        [TestCase("not-json")]
        [TestCase("{")]
        [TestCase("[]")]
        public void ObviousMalformedJsonIsRejected(
            string input)
        {
            SaveSerializerResult result =
                serializer
                    .Deserialize<SaveDocumentEnvelope>(
                        input,
                        out _);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .MalformedData));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-SERIAL-002"));
        }

        [Test]
        public void SerializerPerformsNoFilesystemIo()
        {
            string sentinelParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-Serializer-NoIo-" +
                    Guid.NewGuid()
                        .ToString("N"));

            string sentinelFile =
                Path.Combine(
                    sentinelParent,
                    "unexpected.json");

            SaveDocumentEnvelope source =
                new SaveDocumentEnvelope();

            SaveSerializerResult result =
                serializer.Serialize(
                    source,
                    out string json);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                json,
                Is.Not.Empty);

            Assert.That(
                Directory.Exists(
                    sentinelParent),
                Is.False);

            Assert.That(
                File.Exists(
                    sentinelFile),
                Is.False);
        }

        [Serializable]
        public sealed class PlainDto
        {
            public int number;
            public string label;
            public bool enabled;
        }
    }
}
