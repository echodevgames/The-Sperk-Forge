
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveDocumentValidationTests
    {
        private UnityJsonSaveSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer =
                new UnityJsonSaveSerializer();
        }

        [Test]
        public void OlderPackageDocumentVersionIsRejectedBeforeSerialize()
        {
            SaveDocumentEnvelope document =
                new SaveDocumentEnvelope
                {
                    formatMajor = 0
                };

            SaveSerializerResult result =
                serializer.Serialize(
                    document,
                    out string json);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .UnsupportedDocumentVersion));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-SERIAL-003"));

            Assert.That(
                json,
                Is.Empty);
        }

        [Test]
        public void NewerPackageDocumentVersionIsRejectedAfterDeserialize()
        {
            SaveDocumentEnvelope unsupported =
                new SaveDocumentEnvelope
                {
                    formatMajor =
                        SaveDocumentVersions
                            .EnvelopeMajor + 1
                };

            string json =
                JsonUtility.ToJson(
                    unsupported);

            SaveSerializerResult result =
                serializer
                    .Deserialize<SaveDocumentEnvelope>(
                        json,
                        out SaveDocumentEnvelope restored);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .UnsupportedDocumentVersion));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-SERIAL-003"));

            Assert.That(
                restored,
                Is.Null);
        }

        [Test]
        public void UnsupportedDocumentKindIsRejected()
        {
            SaveDocumentEnvelope document =
                new SaveDocumentEnvelope
                {
                    documentKind =
                        "echosave.unknown"
                };

            SaveSerializerResult result =
                serializer.Serialize(
                    document,
                    out _);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus.Failed));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-SERIAL-007"));
        }
    }
}
